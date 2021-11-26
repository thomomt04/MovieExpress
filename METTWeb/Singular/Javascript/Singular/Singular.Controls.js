/// <reference path="Singular.Core.js" /> 
/// <reference path="Singular.Data.js" /> 

//#region  Setup 

var nbsp = String.fromCharCode(160); //non breaking space

Singular.SetupControls = function () {

  $(document).on('click', "button, input[type='button'], input[type='submit']", Singular.OnButtonClick);

  //prevent backspace causing the browser to go back a page.
  Singular.PreventBackspace();

  Singular.ConvertControlsAll();

  //Force numeric input where the class is Numeric Editor
  //$(".NumericEditor").live("keydown", function () {
  //  ForceNumericInput(this, $(this).attr('data-type') != 'I', true);
  //});

  //Highlight hovered buttons:
  if (!Singular.BSButtons) {
    $(document).on('mouseenter', 'button', function () {
      $(this).addClass('ui-state-hover');
    })
    $(document).on('mouseleave', 'button', function () {
      $(this).removeClass('ui-state-hover');
    });
  }

  $(document).on('change', "button, input[type='file']", Singular.HandleFileSelected);

  $(document).on("change", "[data-AutoPost]", function () {
    var Element = this;
    var CommandName = $(Element).attr('data-AutoPost');
    Singular.SetEventTarget(CommandName ? CommandName : Element.id, $(Element).val());
    $("form").trigger('submit');
  });

  $(document).bind('contextmenu', Singular.HandleContextMenu);

};

//This gets called on an async postback.
Singular.ConvertControlsAll = function () {

  //Singular.SetupSearchScreens();
  Singular.ConvertControls();
  Singular.SetupContextMenus();

};
//This gets called whenever a button is clicked, in case new controls are added which need to be converted.
Singular.ConvertControls = function (root) {

  root = $(root ? root : document);

  //Select based on Control ID.
  root.find('[data-TabControl]').each(function () {
    var div = $(this);
    if (!div.data().tabs) {
      div.tabs();
      div.tabs("option", "active", div.attr('data-TabControl'));
    }
  });

  root.find(".Sortable").each(function () {
    Singular.Sortable(this);
  });

  root.find('[data-ScrollHeight]').each(function () {
    Singular.ScrollableTable(this);
  });

  root.find("[data-popup]").each(function () {

    var Div = $(this).children('div');
    Div.dialog({ modal: true, autoOpen: false, title: $(this).attr('data-popup') });
    Div.hide();

    $(this).children('.PopupImage').on('click', function () {
      Div.dialog('open');
    });

  });
};

Singular.AfterTemplateRender = function (controls, object) {
  if (!Singular.SuspendLayout) {
    if (controls instanceof Array && controls.length > 0) {
      var isInRow = controls[0].parentNode.nodeName == 'TR';
      for (var i = 0; i < controls.length; i++) {
        var element = controls[i];
        if (element.nodeType != 3) { //ignore text nodes.
          //convert control
          if (!isInRow) {
            Singular.ConvertControls(element);
          }
          //Check if child control needs to be animated.
          var AnimType = parseInt($(element).attr('data-tmplAnimate'));
          if (AnimType) {
            var container = element;
            if (element.nodeName == 'TD') {
              var div = $(element).children('table').wrapAll('<div style="overflow: hidden"></div>').parent()[0];
              if (div) container = div;
            }
            container.style.display = 'none';
            if (object.SInfo) {
              object.SInfo.AnimatedElement = container;
              object.SInfo.AnimatedType = AnimType;
            }
            ko.bindingHandlers.visibleA.ShowHide(container, true, null, AnimType, 200);
          }

          //Check if control contains inputs
          if (Singular.FocusAfterRender) {
            var inputs = $(element).find('input, select, textarea').filter(':visible');
            if (inputs.length > 0) {
              Singular.FocusAfterRender = false;
              var ParentTable = $(inputs[0]).parents('[data-ScrollHeight]')[0];
              if (ParentTable) {
                var TBody = $(ParentTable).find('tbody')[0];
                TBody.scrollTop = TBody.scrollHeight;
              }

              $(inputs[0]).focus();
            }
          }
        }
      }
    } else {
      if (controls.nodeName != 'TR') {
        Singular.ConvertControls(controls);
      }

    }
    //Singular.SetupFindScreen();
  };
};
Singular.FocusAfterRender = false;
Singular.AddAndFocus = function (Property, options) {
  Singular.FocusAfterRender = true;
  var newObject = Property.AddNew();
  if (options && options.afterItemAdded) {
    options.afterItemAdded.call(Singular, newObject)
  }
};

//#endregion

//#region  Find Screen 

//Added to override the callback on selecting something on the find screen
Singular.FindSelect = null;

/*NOTE
  There are 2 methods to use, ShowScreenFind and ShowFindScreen... they are different!
  Use 'ShowFindScreen' below this method from stateless JS.
*/
Singular.ShowScreenFind = function (Data, Criteria, Key, IsAsync, AutoPopulate, PreFind, MultiSelect, OnRowSelect, PreSearch) {

  var RowClick;
  if (typeof OnRowSelect == 'function') {
    RowClick = OnRowSelect;
  } else {
    RowClick = function (ID, Obj, dlg) {

      var cont = true;

      if (Singular.FindSelect) {
        cont = Singular.FindSelect(Data, Obj, Key, function () {
          dlg.dialog('close');
        });
      }
      if (cont) {
        //Post Back with the selected ID.
        if (IsAsync) {
          //Async postback
          Singular.SendCommand(Key, ID, function () {
            dlg.dialog('close');
          });
        } else {
          //Full postback
          Singular.DoPostBack(Key, ID);
        }
      }
    }
  };

  Singular.ShowFindDialog(Data, Criteria, Key, AutoPopulate, PreFind, RowClick, MultiSelect, PreSearch);
}

Singular.ShowFindScreen = function (CriteriaProperty /* e.g. ViewModel.DebtorSearchCriteria*/, BeforeOpen, OnFind, RowSelect, MultiSelect) {
  Singular.ShowFindDialog(CriteriaProperty.AttachedTo, CriteriaProperty, '', false, BeforeOpen, RowSelect, MultiSelect, OnFind);
}

Singular.ShowFindDialog = function (Data, Criteria, Key, AutoPopulate, PreFind, RowClick, MultiSelect, PreSearch) {

  var JDialog = $('#' + Criteria.PropertyName),
    JTable = JDialog.find('table'),
    LastRowSelected,
    SearchButton,
    AcceptButton;
  Criteria._RowClick = RowClick;

  if (PreFind) {
    var Args = { Control: null /*needed?*/, Data: Data, Object: Data, AutoPopulate: AutoPopulate, FindDialog: JDialog, Criteria: Criteria(), ClearTable: false };
    PreFind(Args);
    AutoPopulate = Args.AutoPopulate;

    if (Args.ClearTable) Singular.ClearTable(JTable[0]);
  }

  function FindScreenSearchClick() {
    //Serialise the criteria
    SearchButton.prop('disabled', true);
    var Args = KOFormatter.Serialise(Criteria.peek());
    Args.Context = Key;
    Args.CriteriaTypeCode = Criteria.ChildType.Type;

    var pfArgs = { Data: Args, OnRowCreate: null, DataTransform: null, Object: Data };
    if (PreSearch) PreSearch(pfArgs);

    Singular.GetDataStateless(Criteria.ChildType.Type, pfArgs.Data, function (result) {
      SearchButton.prop('disabled', false);
      if (result.Success) {
        ObjectSchema.Apply(result.TypeName, result.Data);
        if (pfArgs.DataTransform) pfArgs.DataTransform(result.Data);
        Singular.CreateTable(JTable[0], result.Data, function (ID, Obj, row) {
          if (MultiSelect) {
            if (window.event.shiftKey && LastRowSelected) {
              row._Selected = LastRowSelected._Selected;
            } else {
              row._Selected = !row._Selected;
            }
            Criteria._IDs = [];
            $(row).toggleClass('Selected', row._Selected);

            var Selecting = 0, Selected = 0;
            JTable.find('tbody tr').each(function () {
              if (window.event.shiftKey && LastRowSelected) {
                if (this == LastRowSelected || this == row) {
                  Selecting += 1;
                }
                if (Selecting == 1) {
                  $(this).toggleClass('Selected', row._Selected);
                  this._Selected = row._Selected;
                }
              }
              if ($(this).hasClass('Selected')) {
                Selected += 1;
                Criteria._IDs.push(this.DataID);
              }

            });
            LastRowSelected = row;
            //Criteria.SelectedCount(Selected);
          } else {
            Criteria._RowClick(ID, Obj, JDialog);
          }
        }, pfArgs.OnRowCreate);
      } else {
        Singular.ShowMessage('Error', result.ErrorText);
      }
    })
  };

  //Handle the Search Button Click.
  SearchButton = JDialog.find('#dlg-Search');
  SearchButton.unbind('click').click(FindScreenSearchClick);
  AcceptButton = JDialog.find('#dlg-Accept');
  AcceptButton.unbind('click').click(function () {
    if (Criteria._IDs && Criteria._IDs.length) {
      Criteria._RowClick(Criteria._IDs, null, JDialog);
    } else {
      alert('Please select at least one row first.');
    }
  });

  var ClearButton = JDialog.find('#dlg-Clear');
  ClearButton.unbind('click').click(function () {
    Criteria.Set();
  });

  //Handle Enter Key
  JDialog.unbind('keydown').keydown(function (e) {
    if (e.keyCode == 13) {
      e.target.blur();
      FindScreenSearchClick();
    }
  });

  if (AutoPopulate) FindScreenSearchClick();

  //Convert the div into a dialog.
  if (JDialog.hasClass('ui-dialog-content')) {
    JDialog.dialog('open');
  } else {
    var width = JDialog[0].style.width;
    JDialog.dialog({ modal: true, /*height: 480,*/ width: width ? width : 640 });
  }
}

//#endregion

//#region  File Upload / Download 

Singular.PendingFileOperations = [];

Singular.CheckPendingFileOperation = function (CallBack) {

  if (Singular.PendingFileOperations.length > 0) {
    Singular.ShowMessage(LocalStrings.Submit, LocalStrings.JSValidationError3, 2);
  } else {
    CallBack();
  }
}
Singular.FileProgress = function (Doc) {
  var UploadPerc = Doc.UploadPercent();
  if (UploadPerc >= 0) {
    return 'Uploading File (' + (UploadPerc * 100).toFixed(0) + '%)';
  } else {
    return 'Uploading...';
  }
}

Singular.HandleFileSelected = function () {

  var fu = this,
    Doc = ko.dataFor(fu),
    ExtAllowed = $(fu).attr('data-extAllowed');

  if (!$(fu).attr('data-SIgnore')) {
    Singular.UploadFile(fu.files ? fu.files[0] : fu, Doc, ExtAllowed, function (Result) {
      if (!Result.Success) alert(Result.Response);
    });
  }
};

Singular._pvt.fSelector = null;
Singular.ShowFileDialog = function (CallBack, accept) {
  var fs = Singular._pvt.fSelector;
  if (fs) {
    //original control must be removed, otherwise adding another file with the same name doesnt invoke the changed event.
    $(fs).unbind('change', Singular._pvt.fSelectorCallBack);
    try {
      document.body.removeChild(fs);
    }
    catch (err) {
      // do nothing
    }
  }
  fs = document.createElement('input');
  fs.type = 'file';
  if (accept) fs.accept = accept;
  fs.style.position = 'absolute';
  fs.style.left = '-1000px';
  fs.style.zIndex = '999999';
  $(fs).attr({ 'data-SIgnore': true });
  document.body.appendChild(fs);
  Singular._pvt.fSelector = fs;
  Singular._pvt.fSelectorCallBack = CallBack
  $(fs).change(CallBack);
  fs.click();
}
Singular.BrowseAndUpload = function (Doc, CallBack, FileTypes, QS) {
  Singular.ShowFileDialog(function (e) {
    Singular.UploadFile(e.target.files[0], Doc, FileTypes, CallBack, QS);
  });
}


Singular.FileUploadedSuccessHandler = null;

Singular.UploadFile = function (File, Doc, AllowedExtensions, CallBack, OtherQSParams, Options) {

  var fu;
  if (File.nodeType) {
    fu = File;
    File = null;
  }
  if (!Options) Options = {};

  var AjaxSender,
    UploadPath,
    FileName = encodeURIComponent(File ? File.name : fu.value);

  if ($('#hPageGuid').length == 1 && (!OtherQSParams || OtherQSParams.indexOf('Stateless') < 0)) {
    //uses session
    UploadPath = Singular.RootPath + "/Library/FileUpload?PageGuid=" + $('#hPageGuid').val() + '&';
  } else {
    //stateless
    UploadPath = Singular.RootPath + "/Library/FileUpload.ashx?"
  }

  UploadPath += 'DocumentGuid=' + (Doc ? Doc.Guid() : Singular.NewGuid()) + (OtherQSParams ? '&' + OtherQSParams : '');

  var ReturnError = function (ErrorText) {
    if (CallBack) CallBack({ Success: false, Response: ErrorText });
  }

  var ParseResponse = function (ResponseText) {
    var Result;
    try {
      var Result = jQuery.parseJSON(ResponseText);
    } catch (e) {
      Result = { Success: false };
      if (ResponseText.indexOf('content length exceeds') > 0) {
        Result.Response = 'Error: file is too large.';
      } else {
        Result.Response = 'Error uploading document';
      }
    }
    return Result;
  }


  var HandleResult = function (Result, FailCB) {
    if (Doc) {
      if (Result.Success) {
        //Set the objects file name so that the file can be downloaded.
        Doc.DocumentName(Result.Response);
        if (Result.DocumentID) {
          Doc.DocumentID(Result.DocumentID);
          Doc.DocumentID.KeyValue = Result.KeyValue;
          Doc.IsNew(false);
          var Parent = Doc.GetParent();
          if (Parent) {
            Parent.IsSelfDirty(true); //usually the document is saved as part of the parent object, so just mark it as dirty.
            if (Parent.DocumentID) {
              Parent.DocumentID(Result.DocumentID);
            }
          }
          if (Singular.FileUploadedSuccessHandler) Singular.FileUploadedSuccessHandler(Doc);
        }
        Doc.ExistsOnServer(3);
      } else {
        Doc.ExistsOnServer(0);
        if (FailCB) FailCB(Result.Response);
      }
      Singular.PendingFileOperations.RemoveItem(Doc);
      if (Doc.SInfo) Singular.Validation.CheckRules(Doc);
    }
    if (Options.OnProgress) Options.OnProgress(1);
    if (CallBack) CallBack(Result);
  }

  var FileTypeAllowed = function () {
    var ActualExtension = '';
    if (AllowedExtensions) {
      var DotIdx = FileName.lastIndexOf('.');
      if (DotIdx > 0) {
        ActualExtension = FileName.substr(DotIdx + 1);
      }
      if (AllowedExtensions.toLowerCase().split(',').indexOf(ActualExtension.toLowerCase()) == -1) {
        ReturnError('Only the following file types are allowed: ' + AllowedExtensions);
        return false;
      }
    }
    return true;
  }

  if (File) {
    AjaxSender = new XMLHttpRequest();
    if (!AjaxSender.upload) AjaxSender = null;
  }

  if (AjaxSender) {
    //send using ajax (new browsers).
    if (FileTypeAllowed()) {
      if (File.size > Singular.MaxRequestSizeKB * 1024) {
        ReturnError('Error, file is too large. Max file size is ' + (Singular.MaxRequestSizeKB / 1024).toFixed(2) + 'MB');
      } else {
        AjaxSender.open('POST', UploadPath, true);
        AjaxSender.setRequestHeader("X_FILENAME", File.name);

        AjaxSender.upload.addEventListener("progress", function (e) {
          if (Doc) Doc.UploadPercent(e.loaded / e.total);
          if (Options.OnProgress) Options.OnProgress(e.loaded / e.total);
        }, false);
        AjaxSender.onreadystatechange = function (e) {
          if (AjaxSender.readyState == 4) {
            HandleResult(ParseResponse(AjaxSender.responseText));
          }
        };
        if (Doc) {
          Singular.PendingFileOperations.push(Doc);
          Doc.UploadPercent(0);
          Doc.ExistsOnServer(2);
        }
        AjaxSender.send(File);
      }
    }

  } else {

    //send by creating a new iframe and form (old browsers).
    var fuParent = $(fu).parent()[0];
    if (!fu.id) fu.id = 'fuTemp';
    if (!fu.name) fu.name = 'fuTemp';

    var Container = $('#divAsyncFileUploader')[0];
    if (!Container) {
      Container = document.createElement('divAsyncFileUploader');
      Container.style.display = 'none';
      document.body.appendChild(Container);
    }

    //Create an iframe to send the file.
    var iframe;
    if (GetIEVersion() <= 8) {
      iframe = document.createElement('div');
      iframe.innerHTML = '<iframe name="uploadFrame' + fu.id + '" id="uploadFrame' + fu.id + '" style="display:none"></iframe>';
      iframe = iframe.childNodes[0];
    } else {
      var iframe = document.createElement('iframe');
      iframe.setAttribute('id', 'uploadFrame' + fu.id);
      iframe.setAttribute('name', 'uploadFrame' + fu.id);
      iframe.style.display = "none";
    }

    //Create a form to submit the file.
    var form = document.createElement('form');
    form.method = "POST";
    form.action = UploadPath + '&FileName=' + FileName;
    form.id = "uploadForm";


    $(form).attr('target', 'uploadFrame' + fu.id)
      .attr('enctype', 'multipart/form-data')
      .attr('encoding', 'multipart/form-data');

    Container.appendChild(form);
    form.appendChild(fu);
    form.appendChild(iframe);

    //Check Extension
    if (FileTypeAllowed()) {

      //Change the object state to 'Uploading File'
      $(iframe).load(function () {
        if (!Doc || !Doc.__Deleted) {

          var Result = ParseResponse($(iframe).contents()[0].body.innerHTML);

          setTimeout(function () {
            HandleResult(Result, function () {
              form.reset();
              fuParent.appendChild(fu);
            });
          }, 0)

        };

        form.removeChild(iframe);
        Container.removeChild(form);

      });
      if (Doc) {
        Doc.ExistsOnServer(2);
        Singular.PendingFileOperations.push(Doc);
        Doc.UploadPercent(-1);
      }
      form.submit();

    } else {
      form.reset();
      fuParent.appendChild(fu);
      form.removeChild(iframe);
      Container.removeChild(form);
    }

  }

}
Singular.getDownloadPath = function () {
  return Singular.RootPath + '/Library/FileDownloader.ashx'
}
/*
  Either:
     Singular.DownloadFile (DocObject, TempGuid)
  or Singular.DownloadFile (Type, MethodName, NamedArgs, OnComplete)
*/
Singular.DownloadFile = function () {

  if (typeof arguments[0] == 'string') {
    //Calling server method

    var CookieName = 'FDT' + Singular.NewGuid(),
      OnComplete = arguments[3];
    var form = $(
      '<form method="POST" action="' + Singular.RootPath + '/Library/FileDownloader.ashx">' +
      '<input type="hidden" name="post-data" value="' + encodeURIComponent(JSON.stringify(arguments[2])) + '" />' +
      '<input type="hidden" name="method-name" value="' + arguments[1] + '" />' +
      '<input type="hidden" name="type" value="' + arguments[0] + '" />' +
      '<input type="hidden" name="fCookie" value="' + CookieName + '" />' +
      '</form>').appendTo('body');

    //wait for cookie to tell if file is downloaded
    var Timer = setInterval(function () {
      if (document.cookie.indexOf(CookieName) >= 0) {
        document.cookie = CookieName + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC';
        clearInterval(Timer);
        if (OnComplete) OnComplete();
      }
    }, 500);

    form.submit();

  } else {
    //Passed in Document Object
    var Obj = arguments[0],
      TempGuid = arguments[1],
      ExtraQS = arguments[2];

    //Construct the download url.
    var Url;
    if (TempGuid) {
      Url = Singular.getDownloadPath() + '?TempGUID=' + TempGuid;
    } else {
      Url = Singular.DownloadPath(Obj) + (ExtraQS ? '&' + ExtraQS : '');
    }

    window.location = Url;
  }

};
Singular.DownloadTempFile = function (TempGuid) {
  Singular.DownloadFile({}, TempGuid);
}

Singular.DownloadPath = function (doc) {
  var pguid = $('#hPageGuid').val();
  if (typeof doc != 'object') {
    doc = { DocumentID: Singular.UnwrapFunction(doc) };
  }
  var dPath = Singular.getDownloadPath() + '?';
  if (pguid) {
    return dPath + 'Type=Document&PageGuid=' + $('#hPageGuid').val() + '&DocumentGuid=' + Singular.UnwrapFunction(doc.Guid);
  } else {
    var docID = Singular.UnwrapFunction(doc.DocumentID);
    if (docID) {
      return dPath + 'DocumentID=' + docID;
    } else {
      return dPath + 'DocumentGuid=' + Singular.UnwrapFunction(doc.Guid);
    }
  }
};

Singular.GetImage = function (HashOrID, IsBackgroundImage) {
  HashOrID = Singular.UnwrapFunction(HashOrID);
  if (HashOrID) {
    var Path = Singular.getDownloadPath() + '?' + (HashOrID.length >= 32 ? 'DocumentHash' : 'ImageID') + '=' + HashOrID;
    if (IsBackgroundImage) {
      return "url('" + Path + "')";
    } else {
      return Path;
    }
  } else {
    if (IsBackgroundImage) {
      return 'none';
    } else {
      return '';
    }
  }
}

//#endregion

//#region  Message Box / Dialog 

Singular.ShowMessageWidth = 400;

//IconTypes - 1: Info, 2: Warning
Singular.ShowMessage = function (Title, Message, IconType, AfterCloseHandler) {

  var div = document.createElement('div');
  var IconClass = '';
  if (IconType == 1 || IconType == undefined)
    IconClass = 'ui-icon-info';
  if (IconType == 2)
    IconClass = 'ui-icon-alert';
  if (IconClass != '')
    div.innerHTML = '<div style="position: relative;"><span class="ui-icon ' + IconClass + ' MsgBoxIcon" style="position:absolute; margin:2px 7px 5px 0;"></span>';

  if (!AfterCloseHandler)
    AfterCloseHandler = function () { };

  div.innerHTML += '<div style="margin-left:30px;" class="MsgBoxMessage">' + Message + '<div></div>';

  Singular.ShowDialog(div, { title: Title, modal: true, width: Math.min(Singular.ShowMessageWidth, window.innerWidth - 20/*scrollbar*/) + 'px', buttons: [{ text: "Ok", click: function () { div.DlgClose(); AfterCloseHandler() } }], destroyAfter: true });
};


Singular.ShowMessageQuestion = function (Title, Message, YesHandler, NoHandler, ShowCancel, Options) {
  Options = Options || {};

  var div = document.createElement('div');
  div.innerHTML = '<div style="position: relative;" class="QuestionBox"><span class="ui-icon ui-icon-help MsgBoxIcon" style="position:absolute; margin:2px 7px 5px 0;"></span><div style="margin-left:30px;" class="MsgBoxMessage">' + Message + '</div></div>';

  if (!NoHandler)
    NoHandler = function () { };

  var Buttons = [];
  Buttons.push({ text: Options.YesText ? Options.YesText : LocalStrings["Yes"], click: function () { div.DlgClose(); YesHandler() }, destroyAfter: true });
  Buttons.push({ text: Options.NoText ? Options.NoText : LocalStrings["No"], click: function () { div.DlgClose(); NoHandler() }, destroyAfter: true });
  if (ShowCancel) {
    Buttons.push({ text: LocalStrings["Cancel"], click: function () { div.DlgClose(); }, destroyAfter: true });
  }

  //$(div).dialog({ title: Title, modal: true, width: "400px", buttons: Buttons });
  Singular.ShowDialog(div, { title: Title, modal: true, width: "400px", buttons: Buttons });
  return div;
};

Singular.ShowDialog = function (element, options) {

  if (options && options.Custom) {
    options.Animate = options.Animate == undefined ? true : options.Animate;
    //custom divs etc
    var inner = $(element);
    if (!element._DlgSetup) {
      element._DlgSetup = true;
      var children = inner.children();
      $(inner).prepend('<div class="Overlay"><div class="Popup"></div></div>');
      children.appendTo(inner.find('.Popup'));
    }
    element.style.display = 'block';

    var overlay = inner.find('.Overlay');
    var popup = inner.find('.Popup');

    var Height = popup.outerHeight();
    popup.css({ left: (($(document).width() - popup.outerWidth()) / 2) + 'px', top: (options.Animate ? (-Height + 'px') : '100px') });
    overlay.css({ height: $(document).outerHeight(true) + 'px' });

    if (options.Animate) {
      overlay.css({ opacity: 0 });
      overlay.animate({ opacity: 1 }, 100);
      popup.animate({ top: '100px' }, 200);
    } else {
      overlay.show();
    }


  } else {
    //jquery dialog
    var jElement = $(element)
    element.DlgClose = function (copt) {
      var Destroy = (copt && copt.destroy) || (options && options.destroyAfter);
      jElement.dialog(Destroy ? 'destroy' : 'close');
      if (Destroy && element.parentNode) {
        document.body.removeChild(element);
      }
    }
    if (options && element.style.width && !options.width) {
      options.width = parseInt(element.style.width);
    }
    jElement.dialog(options);
  }

}

//#endregion

//#region  On Page Messages 

Singular.MessageTypes = ['Information', 'Error', 'Validation', 'Success', 'Warning'];
Singular.MessageType = {
  Information: 0,
  Error: 1,
  Validation: 2,
  Success: 3,
  Warning: 4
}

Singular.AddMessage = function (MessageType) {

  var Title, Message, Holder = 'Main', ArgLength = arguments.length;
  var KeepMessages = false;
  for (var i = 0; i < arguments.length; i++) {
    //hack to allow old messages not to be cleared.
    if (arguments[i] === false) {
      KeepMessages = true;
      ArgLength -= 1;
    }
  }
  if (ArgLength == 2) {
    Message = arguments[1];
  } else if (ArgLength == 3) {
    Title = arguments[1];
    Message = arguments[2];
  } else {
    Holder = arguments[1];
    Title = arguments[2];
    Message = arguments[3];
  }


  return Singular.CreateMessages([{ MessageHolderName: Holder, MessageType: MessageType + 1, MessageTitle: Title, Message: Message }], KeepMessages);

}
Singular.ClearMessages = function (HolderName) {
  var OldList = Singular.CreateMessages.MessageList, NewList = [];
  if (HolderName && OldList) {
    for (var i = 0; i < OldList.length; i++) {
      if (OldList[i].MessageHolderName != HolderName) {
        NewList.push(OldList[i]);
      }
    }
  }
  Singular.CreateMessages(NewList);
}

Singular.InlineMessage = function (Element) {
  //this.Element = Element;
  this.Fade = function (After) {
    setTimeout(function () {
      $(Element).fadeOut(null, function () {
        if (Element.parentNode) Element.parentNode.removeChild(Element);
      });

    }, After);
  }
}

Singular.ClearMessageContainer = function (Name) {
  var Container = $('#MsgControl' + Name);
  Container[0].innerHTML = '';
}

Singular.CreateMessages = function (MessageList, DontClear) {
  //First clear all the message holders.
  if (!DontClear) {
    $('.Msg').each(function () {
      var jMsg = $(this);
      if (!jMsg.attr('data-ClientOnly')) {
        jMsg.children().each(function () {
          this.innerHTML = '';
          $(this).hide();
        });
      }
    });
  }

  Singular.CreateMessages.MessageList = MessageList;
  var MessageControlList = [];
  MessageControlList.Fade = function (After) {
    for (var i = 0; i < this.length; i++) {
      this[i].Fade(After);
    }
  }

  //Then add the messages.
  for (var i = 0; i < MessageList.length; i++) {
    var msg = MessageList[i];
    //Find the message holder.
    var Container = $('#MsgControl' + msg.MessageHolderName);
    if (Container[0]) {
      var MsgDiv = null;
      if (!Container.attr("data-ClientOnly")) MsgDiv = Container.find('.Msg-' + Singular.MessageTypes[msg.MessageType - 1])[0];
      if (!MsgDiv) {
        //Create the message div.
        var MsgDiv = document.createElement('div');
        MsgDiv.setAttribute('class', 'Msg-' + Singular.MessageTypes[msg.MessageType - 1]);
        Container[0].appendChild(MsgDiv);
      }
      var im = new Singular.InlineMessage(MsgDiv);
      MessageControlList.push(im);
      if (msg.FadeAfter && msg.FadeAfter > 0) {
        im.Fade(msg.FadeAfter);
      }

      //populate the content.
      if (msg.MessageTitle != '') {
        MsgDiv.innerHTML = '<strong>' + msg.MessageTitle + '</strong><br/>';
      }
      MsgDiv.innerHTML += msg.Message;
      $(MsgDiv).show();
    }
  }
  return MessageControlList;
};

//#endregion

//#region  Hover Menu 

Singular.SetupContextMenus = function () {

  //This isnt a context menu... its a hover menu.
  $("[data-ContextMenu]").each(function () {
    var MenuContainer = $(this);
    var MenuOptions = MenuContainer.attr("data-ContextMenu").split("|");
    var MenuDiv = $('#' + MenuOptions[0]);
    var MenuClosing;

    //append the popup menu to the body, so it isn't affected by its parents styles.
    $('body').append(MenuDiv);

    MenuContainer.on("mouseenter", function (e) {
      MenuDiv.css({ top: '0px', right: '0px' }); //prevent scrollbar due to menu being off the screen.
      var padding = MenuDiv.outerWidth() - MenuDiv.width();
      var Top = MenuContainer.offset().top + MenuContainer.height();
      var Right = $(window).width() - MenuContainer.offset().left - MenuContainer.outerWidth() - padding;

      MenuDiv.css({ 'top': Top + 'px', 'right': Right + 'px' }).fadeIn();
      MenuClosing = false;

    });
    MenuDiv.on("mouseenter", function (e) { MenuClosing = false; });

    var MouseLeave = function () {
      MenuClosing = true;
      setTimeout(function () {
        if (MenuClosing) {
          MenuDiv.fadeOut();
        }
      }, 500);
    };
    MenuContainer.on("mouseleave", MouseLeave);
    MenuDiv.on("mouseleave", MouseLeave);
  });

  $(document).on("contextmenu", function (e) {
    if (window.DrawUtils) DrawUtils.FixEvent(e);
    if ($(e.target).hasClass('S-CtxMenu') || $(e.target).parents().hasClass('S-CtxMenu')) {
      e.preventDefault();
    }
  });
};

Singular.ContextMenu = function (Items, Left, Top, OnClick) {
  var self = this;

  self.Items = ko.observableArray(Items);
  self.SubMenu = ko.observable();
  self.Left = ko.observable(Left);
  self.Top = ko.observable(Top);
  self.Right = ko.observable();
  self.HasChildren = function () {
    for (var i = 0; i < Items.length; i++) {
      if (Items[i].Items) return true;
    }
    return false;
  }

  self.ItemHover = function (Item, e) {

    if (!self.SubMenu() || self.SubMenu().Items() != Item.Items) {
      self.SubMenu(null);
      if (Item.Items) {
        var SelectedItem = $(e.target);
        var ParentMenu = SelectedItem.parents('.S-CtxMenu'),
          offset = ParentMenu.offset();
        var NewMenu = new Singular.ContextMenu(Item.Items, 0, offset.top, OnClick);
        self.SubMenu(NewMenu);
        Singular.ContextMenu.CheckPosition(offset.left + ParentMenu.outerWidth(), offset.top);
      }
    }

  };

  self.ItemClick = function (Item, e) {
    if (Item.Text && OnClick && (Item.Selectable == undefined || Item.Selectable)) OnClick(Item, arguments);
  }

  self.FadeIn = function (elems) {
    $(elems[0]).parent().children('div').hide().fadeIn();
  }

  if (!Singular.ContextMenu.EventsSetup) {
    Singular.ContextMenu.EventsSetup = true;
    $(document).mousedown(function (e) {
      var RootMenu = $('.S-CtxMenu')[0];
      if ($(e.target).parents('.S-CtxMenu').length == 0) {
        Singular.ContextMenu.Hide();
      }
    })
  }
}

Singular.ContextMenu.Show = function (Items, Left, Top, OnClick) {

  if (arguments.length == 3) {
    //passed through control
    var ctl = $(Left),
      OnClick = Top;

    Left = ctl.offset().left;
    Top = ctl.offset().top + ctl.outerHeight(true);
  }

  Singular.ContextMenu.Current(new Singular.ContextMenu(Items, 0, Math.abs(Top), OnClick));
  Singular.ContextMenu.CheckPosition(Left, Top);
}
Singular.ContextMenu.CheckPosition = function (Left, Top) {
  var menu = $('.S-CtxMenu').last();
  var mHeight = menu.outerHeight() + 2, mWidth = menu.outerWidth() + 2;
  var wHeight = $(document).height(), wWidth = $(document).width();
  if (Top < 0) {
    menu.css('top', (-Top - mHeight) + 'px');
  } else if (mHeight + Top > wHeight) {
    menu.css('top', wHeight - mHeight + 'px');
  }
  menu.css('left', Math.min(Left, wWidth - mWidth) + 'px');
}
Singular.ContextMenu.Hide = function () {
  Singular.ContextMenu.Current(null);
}
Singular.ContextMenu.Current = ko.observable(null);
Singular.ContextMenu.Create = function () {
  var ctxMenu = document.createElement('div');
  ctxMenu.setAttribute('data-bind', "template: { 'if': Singular.ContextMenu.Current, name: 'SMenuTemplate', data: Singular.ContextMenu.Current, afterRender: $data.FadeIn }");
  document.body.appendChild(ctxMenu);
  Singular.ContextMenu.Instance = ctxMenu;
}

Singular.HandleContextMenu = function (e) {

  if (Singular.OnContextMenu && !Singular.OnContextMenu(e)) return false;

  //Check if the control is in a grid, and not an editor.
  if (e.srcElement != null && e.srcElement.nodeName != 'INPUT') {
    var Table = $(e.srcElement).closest('table')[0];
    if (Table && Table.SInfo && Table.SInfo.ProcessProperty) {
      var List = Table.SInfo.ProcessProperty.peek();
      if (List.length > 0) {
        return Singular.ShowGridContextMenu(e, List);
      }

    }
  }
  return true;
};
Singular.ContextMenuContainer = null;
Singular.ShowGridContextMenu = function (e, List) {
  var ShowMenu = false;

  //Check if there is an expand property.
  if (List[0].Expanded || List[0].IsExpanded) {

    function Expand(Type) {
      var ExpandName = List[0].Expanded ? 'Expanded' : 'IsExpanded';
      for (var i = 0; i < List.length; i++) {
        if (Type == -1) {
          List[i][ExpandName](!List[i][ExpandName].peek());
        } else if (Type == 0) {
          List[i][ExpandName](false);
        } else {
          List[i][ExpandName](true);
        }
      }
    }

    var Options = [{ Text: 'Expand All', ID: 1 }, { Text: 'Expand None', ID: 0 }, { Text: 'Invert Expansion', ID: -1 }]
    Singular.ContextMenu.Show(Options, e.pageX, e.pageY, function (Item, Args) {
      Singular.ContextMenu.Hide();
      Expand(Item.ID);
    });
    return false;
  }
  return true;

}
//Allow custom context menus
Singular.OnContextMenu = null;


Singular.CMIconStyle = function (Item) {
  var obj = { visibility: 'visible' };
  if (!Item.Icon) {
    obj.visibility = 'hidden';
    obj.width = '16px';
  } else if (Item.Icon instanceof Array) {
    obj['background-position'] = '-' + Item.Icon[0] * 16 + 'px -' + Item.Icon[1] * 16 + 'px';
  }
  return obj;
}
Singular.CMIconClass = function (Item) {
  if (!Item.Icon || Item.Icon instanceof Array) {
    return 'Icon';
  } else {
    return Item.Icon;
  }
}

//#endregion

//#region  Tables 

Singular.CreateTable = function (Table, List, SelectCallBack, OnRowCreate /* fn(IsHeader, row)*/) {

  Singular.ClearTable(Table);

  //Header
  if (List.length > 0) {
    var thead = document.createElement('thead');
    var thr = document.createElement('tr');
    for (var i = 0; i < List.Properties.length; i++) {
      if (!List.Properties[i].Hidden) {
        Singular.CreateTableHeader(thr, List.Properties[i]);
      };
    };
    if (OnRowCreate) OnRowCreate(true, thr, List);
    thead.appendChild(thr);

    //Body
    var tbody = document.createElement('tbody');
    tbody.setAttribute('class', 'GridHL');

    for (var i = 0; i < List.length; i++) {

      var tr = document.createElement('tr');
      tr.ListObject = List[i];
      tr.DataID = List[i][List.IDProperty];

      for (var j = 0; j < List.Properties.length; j++) {
        if (!List.Properties[j].Hidden) {
          Singular.CreateTableCell(tr, List[i][List.Properties[j].Name], List.Properties[j]);
        };
      };
      if (!OnRowCreate || OnRowCreate(false, tr, List[i]) !== false) { //returning false on RowCreate function prevents row being added.
        $(tr).click(function () {
          SelectCallBack(this.ListObject[List.IDProperty], this.ListObject, this);
          return false;
        });
        tbody.appendChild(tr);
      }

    };

    Table.appendChild(thead);
    Table.appendChild(tbody);
    Singular.FormatTableBody(tbody);

    Singular.Sortable(Table, true);
  } else {
    var tr = document.createElement('tr');
    var td = document.createElement('td');
    var text = document.createTextNode('No data found');
    td.appendChild(text);
    tr.appendChild(td);
    Table.appendChild(tr);
  };

};
Singular.CreateTableCell = function (Row, CellValue, PropertyInfo) {
  //Creates a table cell with the correct formatting for the data type
  var td = document.createElement('td');
  Row.appendChild(td);

  PropertyInfo = PropertyInfo || { Type: 's' };
  if (PropertyInfo.Type == 'b') {
    //Boolean
    td.style.textAlign = 'center';
    var cb = document.createElement('input');
    cb.setAttribute('type', 'checkbox');
    cb.setAttribute('disabled', 'disabled');
    if (CellValue == 'true' || CellValue == true)
      cb.setAttribute('checked', 'checked');
    td.appendChild(cb);
  } else if (PropertyInfo.Type == 'n') {
    //Number
    td.style.textAlign = 'right';
    var value;
    if (PropertyInfo.Format) {
      if (isNaN(parseInt(PropertyInfo.Format[0]))) {
        value = parseFloat(CellValue).formatDotNet(PropertyInfo.Format);
      } else {
        var params = eval('[' + PropertyInfo.Format + ']');
        if (params.length == 1) {
          value = parseFloat(CellValue).formatMoney(params[0]);
        } else {
          value = parseFloat(CellValue).formatMoney(params[0], params[1]);
        }
      }
    } else {
      value = parseFloat(CellValue).formatMoney(2, nbsp);
    }
    td.innerHTML = value;

  } else if (PropertyInfo.Type == 'd') {
    //Date
    var format = PropertyInfo.Format ? PropertyInfo.Format : 'dd' + nbsp + 'MMM' + nbsp + 'yyyy';
    td.style.textAlign = 'center';
    td.innerHTML = CellValue == null ? '' : new Date(CellValue).format(format);
  } else {
    //Text
    td.innerHTML = CellValue;
  }
  if (PropertyInfo.Align) {
    td.style.textAlign = PropertyInfo.Align;
  }
  return td;
};
Singular.CreateTableHeader = function (Row, PropertyInfo) {
  //Creates a table header with the correct formatting for the data type.
  var th = document.createElement('th');
  Row.appendChild(th);
  th.innerHTML = PropertyInfo.Display;
  th.setAttribute('data-type', PropertyInfo.Type);

  if (PropertyInfo.Align) {
    th.style.textAlign = PropertyInfo.Align;
  } else {
    if (PropertyInfo.Type == "n") {
      th.style.textAlign = 'right';
    }
    if (PropertyInfo.Type == "d") {
      th.style.textAlign = 'center';
    }
  }
  if (PropertyInfo.Width != undefined) {
    th.style.width = PropertyInfo.Width + 'px';
  }
  return th;
};

Singular.ClearTable = function (Table) {
  while (Table.firstChild) {
    Table.removeChild(Table.firstChild);
  };
}

/*
Proper way of sorting by sorting the data items in a filter / sort function
that is between the actual data source and the grid.
*/
Singular.MakeTableSortable = function (Table) {

  $(Table).find('th').each(function () {

    var Header = $(this);
    if (Header.attr('data-Property')) {
      this.style.cursor = 'pointer';
      Header.click(function (event) {

        var Property = $(this).attr('data-Property');
        var Table = $(this).closest('table')[0];
        var SourceProperty = Singular.GetSValue(Table, 'ProcessProperty');
        var SortOptions = SourceProperty.ProcessOptions.SortOptions.peek();
        if (SortOptions.SortProperty == Property) {
          SortOptions.SortAsc = !SortOptions.SortAsc;
        } else {
          SortOptions.SortProperty = Property;
          SortOptions.SortAsc = true;
        }
        //Re-sort the list. 
        SourceProperty.ProcessOptions.SortOptions(SortOptions);

        //Call global onSort
        var Args = { SetIcon: true, FormatAltRows: true, Table: Table, DataSourceProperty: SourceProperty, SortProperty: SortOptions.SortProperty, SortAsc: SortOptions.SortAsc };
        if (Singular.OnTableSort) Singular.OnTableSort(Args)

        //Set the sort icon
        if (Args.SetIcon) Singular.FormatSortableHeader(Table, this, SortOptions.SortAsc);

        //Re-format the alt rows.
        if (Args.FormatAltRows) Singular.FormatTableBody($(Table).find('tbody')[0]);

      });
    }

  });

}
//Simple way of sorting, sorts the actual tr elements.
Singular.Sortable = function (Table, Force) {

  if (!Table.SortableApplied || Force) {
    $(Table).find('th').each(function () {
      $(this).css('cursor', 'pointer');
      $(this).bind('click', function () {
        Singular.SortTable(this);
      });
    });
    Table.SortableApplied = true;
  };
};

Singular.SortTable = function (Header) {

  var Direction = 1; //1=Asc, 0=Desc;
  if (Header.SortDirection) {
    Direction = 0
  } else {
    Direction = 1;
  }
  Header.SortDirection = Direction;

  //Get the headers index so we know which cell to compare in each row.
  var HeaderIndex = 0;
  var HeaderRow = $(Header).parent()[0]
  for (var Idx = 0; Idx < HeaderRow.cells.length; Idx++) {
    if (HeaderRow.cells[Idx] == Header) {
      HeaderIndex = Idx;
    } else {
      HeaderRow.cells[Idx].SortDirection = null; //reset the sort on other headers.
    };
  };
  //The header cell must be part of a thead, and the cells to sort must be part of a tbody.
  var THead = $(HeaderRow).parent('thead');
  if (THead.length == 1) {

    var TBody = THead.next()[0];
    if (TBody) {
      var TFoot = $(TBody).next();

      //Default Value Accessor will just return the cell text.
      var ValueAccessor = function (row) {
        return row.cells[HeaderIndex].innerHTML;
      };

      //See if we need a more complex value accessor.
      if (TBody.children[0]) {
        //if there is at least 1 row

        //If there is an editor
        var Controls = TBody.children[0].cells[HeaderIndex].children;
        if (Controls.length > 0) {
          //Check which propery to use
          var Prop = TBody.children[0].cells[HeaderIndex].children[0].value == undefined ? 'innerHTML' : 'value';

          ValueAccessor = function (row) {
            return row.cells[HeaderIndex].children[0][Prop];
          };
          //Check the type
          switch ($(HeaderRow.cells[HeaderIndex]).attr('data-type')) {
            case "d":
              ValueAccessor = function (row) {
                var strVal = row.cells[HeaderIndex].children[0][Prop];
                return new Date(strVal ? strVal.replace(/&nbsp;/g, ' ') : 0);
              }; break;
            case "n":
              ValueAccessor = function (row) {
                var Val = row.cells[HeaderIndex].children[0][Prop];
                Val = Val.replace('(', '-').replace(')', '');
                return parseFloat(Val.replace(/[^0-9\.\-]+/g, ''));
              }; break;
            case "b":
              ValueAccessor = function (row) {
                return row.cells[HeaderIndex].children[0].checked;
              }; break;
          };
        } else {

          //Check the type
          switch ($(HeaderRow.cells[HeaderIndex]).attr('data-type')) {
            case "d":
              ValueAccessor = function (row) {
                return new Date(row.cells[HeaderIndex].innerHTML.replace(/&nbsp;/g, ' '));
              }; break;
            case "n":
              ValueAccessor = function (row) {
                var Val = row.cells[HeaderIndex].innerHTML;
                Val = Val.replace('(', '-').replace(')', '');
                return parseFloat(Val.replace(/[^0-9\.\-]+/g, ''));
              }; break;
          };
        };
      };

      //Merge sort the rows
      var newList = Singular.MergeSort($(TBody).find('tr'), ValueAccessor, Direction, HeaderIndex);
      //Have to create a new TBody element, and add the sorted rows to it manually.
      var TBodyNew = TBody.cloneNode(false);
      var Table = $(TBody).parent()[0];

      //add the sorted rows to the new table body.
      //$(TBodyNew).append(newList);
      for (var i = 0; i < newList.length; i++) {
        var child = TBodyNew.appendChild(newList[i]);
      }
      Singular.FormatTableBody(TBodyNew);
      //replace the old rows with the sorted rows.
      Table.replaceChild(TBodyNew, TBody);


    };
  };
};

Singular.ApplyRowClass = function (JRow, Alt, First, Last) {
  if (Alt) {
    JRow.addClass('GridAltRow');
  } else {
    JRow.removeClass('GridAltRow');
  }
  if (First) {
    JRow.addClass(Alt ? 'GirdRowAltFIG' : 'GridRowFIG').removeClass(!Alt ? 'GirdRowAltFIG' : 'GridRowFIG');
  }
  if (Last) {
    JRow.addClass(Alt ? 'GirdRowAltLIG' : 'GridRowLIG').removeClass(!Alt ? 'GirdRowAltLIG' : 'GridRowLIG');
  }
};
Singular.FormatTableBody = function (tbody) {
  var Rows = $(tbody).children('tr');
  var Alt = true;
  var LastObject = null;
  for (var i = 0; i < Rows.length; i++) {
    var CurrentObject = ko.dataFor(Rows[i]);
    var NextObject = (i == (Rows.length - 1) ? null : ko.dataFor(Rows[i + 1]));
    var First = LastObject == null || CurrentObject == null || LastObject != CurrentObject;
    var Last = NextObject == null || CurrentObject == null || CurrentObject != NextObject;
    LastObject = CurrentObject;
    if (First) Alt = !Alt;

    Singular.ApplyRowClass($(Rows[i]), Alt, First, Last);

  }
};
Singular.FormatTableRow = function (controls, object) {
  var tBody = controls[0].parentNode;

  if (Singular.GetSValue(tBody.parentNode, 'ScrollTableFix')) {
    Singular.ScrollableTable(tBody.parentNode);
  }

  var isFirst = true;
  var Row;
  for (var i = 0; i < controls.length; i++) {
    if (controls[i].nodeName == 'TR') {
      Row = $(controls[i]);
      Singular.ApplyRowClass(Row, tBody.__AltRow, isFirst, false);
      isFirst = false;
    };
  };
  Singular.ApplyRowClass(Row, tBody.__AltRow, false, true);
  tBody.__AltRow = !tBody.__AltRow;
};

Singular.FormatSortableHeader = function (Table, Header, Asc) {

  //if we are in bootstrap mode, do the following and then return to prevent default behaviour
  if (Singular.UIStyle == 2 || Singular.SortIconUsesColumnType) {
    //If another header was sorted, remove the icon.
    var PrevHead = Singular.GetSValue(Table, 'SortHeader');
    if (PrevHead) {
      $(PrevHead).removeClass('column-sorted');
      if (PrevHead._SortImg) {
        PrevHead.removeChild(PrevHead._SortImg)
        PrevHead._SortImg = null
      }
    }
    //Create sort icon
    if (!Header._SortImg) {
      var img = document.createElement('i');
      Header.appendChild(img)
      Header._SortImg = img
    }

    var sortDesc = '';

    if (Singular.SortIconUsesColumnType) {
      var firstRow = $(Header).parents('table').find('tbody tr')[0];
      var colType = '';
      if (firstRow) {
        var boundTo = ko.dataFor(firstRow);
        if (boundTo) colType = boundTo[$(Header).attr('data-property')].DataType;
      }
      if (colType == 'n') sortDesc = 'amount-';
      if (colType == 's') sortDesc = 'alpha-';

      Header._SortImg.className = 'fa fa-sort-' + sortDesc + (Asc ? 'asc' : 'desc') + ' sort-icon';
    } else {
      Header._SortImg.className = 'fa fa-chevron-' + sortDesc + (Asc ? 'up' : 'down') + ' sort-icon';
    }

    $(Header).addClass('column-sorted');
    Singular.SetSValue(Table, 'SortHeader', Header);
    return;
  }

  //If another header was sorted, remove the icon.
  var PrevHead = Singular.GetSValue(Table, 'SortHeader');
  if (PrevHead) {
    $(PrevHead).removeClass('Highlight');
  }
  //Create sort icon
  //Header.style.position = 'relative';
  if (!Header._SortImg) {
    $(Header).wrapInner('<div class="SortDiv"></div>');

    var img = document.createElement('span');
    img.className = 'SortIcon';
    Header.childNodes[0].appendChild(img);
    Header._SortImg = img
  }
  $(Header).addClass('Highlight');
  Header._SortImg.style.top = (($(Header._SortImg.parentNode).outerHeight() / 2) - (Asc ? 5 : 1)) + 'px'; //center the icon.
  Header._SortImg.style.backgroundPositionY = Asc ? '0' : '6px'; //choose asc or desc

  //if (Header.style.textAlign == 'right') { //add some padding for right align text boxes.
  //  Header.style.boxSizing = 'border-box';
  //  Header.style.paddingRight = '12px'
  //}


  //Header._SortIcon = img;
  Singular.SetSValue(Table, 'SortHeader', Header);
}


Singular.ScrollableTable = function (Table) {

  if (Singular.GetSValue(Table, 'ScollSetup')) return;

  var jTable = $(Table);
  var params = jTable.attr('data-ScrollHeight').split('|');
  var MinHeight = parseInt(params[0]), MaxHeight = parseInt(params[1]);
  var TableWidth = jTable.width();
  var HasCustomScroller = jTable.find('tbody > div').length > 0;

  if (!jTable.is(':visible')) {
    Singular.SetSValue(Table, 'ScrollTableFix', true);
    return;
  };

  //if we are working with bootstrap, store the column widths before doing any formatting changes
  var boostrapColumnWidths = [];
  // && document.body.getBoundingClientRect
  if (Singular.UIStyle && Singular.UIStyle === 2) {
    var firstTableBodyRow = jTable.find('tbody tr:first');
    if (firstTableBodyRow) {
      var firstRowCells = $(firstTableBodyRow).children("td");
      for (var i = 0; i < firstRowCells.length; i++) {
        $(firstRowCells[i]).outerWidth();
        //boostrapColumnWidths.push(firstRowCells[i].getBoundingClientRect().width);
      };
    };
  };

  //set the css for the rows in the header and footer
  jTable.find('thead tr, tfoot tr').css({ display: 'block' });


  //set the css for the table body to give it a max-height.
  var tbody = jTable.children('tbody');
  tbody.css({
    width: 'auto', 'max-height': MaxHeight ? MaxHeight : '', 'min-height': MinHeight ? MinHeight : '',
    display: 'block', 'overflow-y': (MaxHeight && !HasCustomScroller) ? 'scroll' : ''
  });

  if (GetIEVersion() <= 9) {
    tbody.css({ float: 'left' });
    jTable.find('thead').css({ float: 'left' });
    jTable.css({ clear: 'both' });
  }

  var ScrollWidth = HasCustomScroller ? 0 : (tbody[0].offsetWidth - tbody[0].clientWidth);

  var BodyRows = HasCustomScroller ? tbody.find('tr') : tbody.children('tr');
  var FirstRow = $(BodyRows[0]);

  //Add the scroll width to the last cell in the header.
  var RowIndex = 0, HeaderWidth = 0, HasRows = false;
  jTable.find('thead tr').each(function () {
    var HeaderCells = $(this).children('th:visible');

    //Set the header widths equal to the row widths.
    //Only if the no of header cells matches the no of body cells for this row.
    if (BodyRows.length > RowIndex) {
      var BodyCells = $(BodyRows[RowIndex]).children('td:visible');
      if (HeaderCells.length == BodyCells.length) {
        var tdWidth = null;
        for (var i = 0; i < HeaderCells.length; i++) {
          tdWidth = $(BodyCells[i]).outerWidth();
          if (Singular.UIStyle && Singular.UIStyle === 2) {
            tdWidth = boostrapColumnWidths[i];
          };
          $(HeaderCells[i]).css({ 'box-sizing': 'border-box', width: tdWidth + 'px' });
          if (RowIndex == 0) HeaderWidth += tdWidth;
        };
        //update all row widths for now as a quick hack, i will fix this to be a legit solution a bit later
        if (Singular.UIStyle && Singular.UIStyle === 2) {
          for (var k = 0; k < BodyRows.length; k++) {
            var RowCells = $(BodyRows[i]).children('td');
            for (var l = 0; l < RowCells.length; l++) {
              tdWidth = boostrapColumnWidths[l];
              $(RowCells[l]).css({ 'box-sizing': 'border-box', width: tdWidth + 'px' });
            }
          }
        };
      }
      Singular.SetSValue(Table, 'ScrollTableFix', false);
      HasRows = true;
    } else {
      //the table has no rows.. call this again when the first row is added.
      Singular.SetSValue(Table, 'ScrollTableFix', true);
    }

    RowIndex += 1;
  });

  //If the table has a css width set, then reduce the size of the table by the scrollbar width.
  if (Table.style.width) {
    jTable.css('width', jTable.outerWidth() - ScrollWidth + 'px');
  }
  if (!HasCustomScroller) {
    var tBodyWidth = (HeaderWidth == 0 ? TableWidth : HeaderWidth) + ScrollWidth;
    tbody.css('width', tBodyWidth);
  }

  if (HasRows) {
    Singular.SetSValue(Table, 'ScollSetup', true);
  }


  //Old browsers will make each row the same height as the body height, this will fix that.
  if (FirstRow.outerHeight() >= MaxHeight - 10) {
    $('tbody tr').each(function () {
      $(this).css({ height: 'auto' });
    });
  }

}

Singular.Expand = function (prop) {
  var Value = prop();
  if (Value) {
    if (prop.AttachedTo && prop.AttachedTo.SInfo.AnimatedElement) {
      ko.bindingHandlers.visibleA.ShowHide(prop.AttachedTo.SInfo.AnimatedElement, false, function () { prop(false); }, prop.AttachedTo.SInfo.AnimatedType, 100);
    } else {
      prop(false);
    }
  } else {
    prop(true);
  }
}

//#endregion

//#region  Loading bar 

Singular.LoadingCount = 0;
Singular.LoadingGifUrl = '/LibResources/img?file=Loading.gif';
Singular.CreateLoadingBar = function () {

  var OverlayDiv = document.createElement('div');
  OverlayDiv.setAttribute('class', 'LoadingOverlay');
  OverlayDiv.style.display = 'none';
  document.body.appendChild(OverlayDiv);

  var ContainerDiv = document.createElement('div');
  ContainerDiv.setAttribute('class', 'ProgressDiv');
  ContainerDiv.style.display = 'block';
  ContainerDiv.style.position = 'absolute';
  OverlayDiv.appendChild(ContainerDiv);

  var span = document.createElement('span');
  $(span).css({ 'white-space': 'normal' });
  ContainerDiv.appendChild(span);

  var img = document.createElement('img');
  img.src = Singular.RootPath + Singular.LoadingGifUrl;
  img.alt = 'Loading...'
  $(img).css({ 'vertical-align': 'middle' });
  //img.className("loadingImg");
  ContainerDiv.appendChild(img);

  var ProgDiv = document.createElement('div');
  $(ProgDiv).css({ display: 'none', backgroundColor: '#eee' });
  ContainerDiv.appendChild(ProgDiv);

  var ProgDivInner = document.createElement('div');
  ProgDiv.appendChild(ProgDivInner);
  $(ProgDivInner).css({ display: 'none', height: '20px', backgroundColor: '#004B93', width: '1px' });

  Singular.LoadingDiv = OverlayDiv;
  Singular.LoadingDiv.TextSpan = span;
  Singular.LoadingDiv.LoadImage = img;
  Singular.LoadingDiv.ProgressDiv = ProgDivInner;
  Singular.LoadingDiv.OnShow = function () {

    $(ContainerDiv).css({
      left: (($(window).width() - $(ContainerDiv).outerWidth(true)) / 2) + 'px',
      top: (($(window).height() - $(ContainerDiv).outerHeight(true)) / 3) + 'px'
    });
    document.body.style.overflow = 'hidden';

  };
}

Singular.LoadingBarMinDelayComplete = false;
Singular.LoadingBarHideComplete = false;

Singular.ShowLoadingBar = function (Delay, Text, minDelay) {

  Delay = Delay ? Delay : 100;
  minDelay = minDelay ? minDelay : 0;

  Singular.LoadingCount += 1;
  if (Singular.LoadingCount == 1) {

    Singular.LoadingBarMinDelayComplete = false;
    Singular.LoadingBarHideComplete = false;

    var h = setTimeout(function () {

      if (Singular.LoadingCount > 0) {

        Singular.UpdateLoadingBar(Text ? Text : Singular.DefaultLoadingText);
        $(Singular.LoadingDiv).show();
        setTimeout(function () {
          Singular.LoadingBarMinDelayComplete = true;
          if (Singular.LoadingBarHideComplete) {
            Singular.ActuallyHideLoadingBar()
          }
        }, minDelay);

        if (Singular.LoadingDiv.OnShow) Singular.LoadingDiv.OnShow();

      }

    }, Delay);
  }

};
Singular.DefaultLoadingText = '';

Singular.UpdateLoadingBar = function (Text, Step, MaxStep) {
  var elSpan = Singular.LoadingDiv.TextSpan;
  if (Text && elSpan) {
    $(elSpan).text(Text);
    $(elSpan).show();
  } else {
    $(elSpan).hide();
  }
  if (Step != undefined && MaxStep) {
    $(Singular.LoadingDiv.LoadImage).hide();
    $(Singular.LoadingDiv.ProgressDiv).parent().show();
    $(Singular.LoadingDiv.ProgressDiv).css({ display: 'block', width: ((Step / MaxStep) * 100) + '%' });

  } else {
    $(Singular.LoadingDiv.LoadImage).show();
    $(Singular.LoadingDiv.ProgressDiv).hide();
  }
}

Singular.HideLoadingBarCallBackList = [];

Singular.ActuallyHideLoadingBar = function () {
  $(Singular.LoadingDiv).hide();
  Singular.LoadingDiv._Hidden = true;
  document.body.style.overflow = '';
  Singular.LoadingBarHideComplete = false;
  Singular.LoadingBarMinDelayComplete = false;
  for (var i = 0; i < Singular.HideLoadingBarCallBackList.length; i++) {
    Singular.HideLoadingBarCallBackList[i]();
  }
  Singular.HideLoadingBarCallBackList = [];
}

Singular.HideLoadingBar = function (callBack) {
  if (callBack && typeof callBack == 'function') {
    var addCallBack = 1;
    for (var i = 0; i < Singular.HideLoadingBarCallBackList.length; i++) {
      if (Singular.HideLoadingBarCallBackList[i].toString() == callBack.toString()) {
        addCallBack = 0;
        break;
      }
    }
    if (addCallBack == 1) {
      Singular.HideLoadingBarCallBackList.push(callBack);
    }
  }
  if (Singular.LoadingCount > 0) Singular.LoadingCount -= 1;
  if (Singular.LoadingCount == 0) {
    Singular.LoadingBarHideComplete = true;
    if (Singular.LoadingBarMinDelayComplete) {
      Singular.ActuallyHideLoadingBar()
    }
  }
};

Singular.InlineLoadingBarShown = function (element, showing, finished) {
  if (showing && !finished) {
    element.style.display = '';
    var Overlay = $(element);
    var Outer = Overlay.find('.Outer');
    Outer.css({ display: '', marginTop: (Overlay.height() - Outer.outerHeight()) / 2 });
  }
}

//#endregion

//#region  Misc 
Singular.GetNextEditor = function (CurrentControl) {
  var Editable = $(CurrentControl).parents('form:eq(0),body').find('button,input,textarea,select').filter(':visible');;
  var index = Editable.index(CurrentControl);
  if (index > -1 && (index + 1) < Editable.length) {
    return Editable.eq(index + 1)[0];
  };
}
Singular.FocusNextControl = function (CurrentControl) {
  var Editor = Singular.GetNextEditor(CurrentControl);
  if (Editor) {
    Editor.focus();
  }
};
Singular.ClearDropDown = function (button) {
  var DropDown = $(button).prev();
  DropDown[0].value = null;
  ko.utils.triggerEvent(DropDown[0], 'change');
};

Singular.PreventBackspace = function () {
  $(document).unbind('keydown').bind('keydown', function (event) {
    var doPrevent = false;
    if (event.keyCode === 8) {
      var d = event.srcElement || event.target;
      if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD'))
        || d.tagName.toUpperCase() === 'TEXTAREA') {
        doPrevent = d.readOnly || d.disabled;
      }
      else {
        doPrevent = true;
      }
    }

    if (doPrevent) {
      event.preventDefault();
    }
  });
};

Singular.GetAuthImage = function (AuthValue) {
  AuthValue = Singular.UnwrapFunction(AuthValue);
  if (AuthValue == null)
    return '';
  else
    return Singular.RootPath + '/LibResources/img?file=' + (AuthValue == 0 ? 'IconEdit' : (AuthValue == 1 ? 'IconAuth' : 'IconError')) + '.png';
}
Singular.GetAuthTT = function (AuthValue) {
  AuthValue = Singular.UnwrapFunction(AuthValue);
  return AuthValue == 0 ? 'Pending' : (AuthValue == 1 ? 'Authorised' : 'Rejected');
}

//#endregion

//#region  Dynamic HTML  

var DLHTML = (function () {
  var self = {};

  var LoadingCount = 0;
  self.LoadControls = function (OnComplete) {

    var ToLoad = $('[data-AsyncContent]');

    Singular.AddLogEntry('Control fetch count: ' + ToLoad.length);

    if (ToLoad.length == 0) {
      OnComplete();
    } else {
      ToLoad.each(function () {
        LoadingCount += 1;
        var Control = $(this);
        var HashCode = Control.attr('data-AsyncContent');

        $.ajax({
          type: "GET",
          url: Singular.RootPath + '/Library/GetHTML/' + HashCode,
          async: true,
          success: function (Data) {

            try {
              if (Control[0].nodeName == 'SCRIPT') {
                Control[0].appendChild(document.createTextNode(Data));
              } else {
                Control[0].innerHTML = Data;
              }


              //Control[0].innerHTML = Data;
            } catch (e) {
              try {
                Control[0].text = Data;
              } catch (e) { }
            }

            LoadingCount -= 1;
            if (LoadingCount == 0) {
              OnComplete();
            }
          },
          error: function () {
            alert('Error loading content.');
          }
        });

      });
    }

  };

  return self;
})();

//#endregion
