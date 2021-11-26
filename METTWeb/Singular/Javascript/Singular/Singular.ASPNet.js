// Singular Library
// This file contains code specific to full web applications using the Singular Asp.Net web forms library.
// Version 1.2.214

//Initialise the Page.
$(window).load(function () {

  var MainForm = $('form');

  if (GetIEVersion() < Singular.MinIEVersion) {
    MainForm.prepend('<div class="OldBrowser">' + LocalStrings['OldBrowser'] + '</div>');
  }
  //Check if using old css file
  $('link').each(function(){
    var sfx = 'SingularControls.css';
    if (this.href && this.href.indexOf(sfx, this.href.length - sfx.length) !== -1) {
      alert('WARNING: Old library css file.'); 
    }
  });

  window.onerror = function (msg, url, line) {
    Singular.AddLogEntry('Error: ' + msg + " - " + url + ":" + line);
  };

  $(document).keyup(function(e){
    if (e.keyCode == 113 && e.altKey) {
      alert('Debug info:\n' + Singular.Log);
    }
  });

  Singular.IsStateless = document.getElementById('hPageGuid') == null;
  KOFormatter.IncludeIsNew = Singular.IsStateless;
  KOFormatter.IncludeClean = !Singular.IsStateless;

  //handle the submit event;
  MainForm.submit(Singular.SerialiseModel);
  //Autocomplete causes issues with binding.
  MainForm.attr('autocomplete', 'off');

  Singular.CreateLoadingBar();
  Singular.ContextMenu.Create();
  
  Singular.OnPageLoad();
  Singular.ShowSessionExpired = false;

  var skaTimer;
  function SessionKeepAlive() {
    Singular.AJAXCall('KeepAlive', {}, function (Data) {
      //Only call again if successful.
      skaTimer = setTimeout(SessionKeepAlive, 600 * 1000);
    }, function () {
      clearTimeout(skaTimer);
      if (Singular.ShowSessionExpired == undefined) {
        alert('Your Session has expired, the page will now reload.');
        window.location.reload();
      }
    });
  }

  if (Singular.IsAuthenticated && !Singular.IsStateless) {
    //Session Keep Alive. 
    skaTimer = setTimeout(SessionKeepAlive, 600 * 1000);
  }
});

Singular.LastButtonClicked = null;
Singular.LastSelectControl = null;
Singular.SuspendLayout = false;
Singular.SaveDelay = 1000;
Singular.IsAuthenticated = false;
Singular.MinIEVersion = 9;

Singular.Init = function (IsAuthenticated, SaveMode, DeleteMode, ValidationDisplayMode, ValidationMode) {

  Singular.SaveMode = SaveMode;
  Singular.DeleteMode = DeleteMode;
  Singular.Validation.ValidationDisplayMode = ValidationDisplayMode;
  Singular.Validation.ValidationMode = ValidationMode;
  Singular.IsAuthenticated = IsAuthenticated;

  if (SaveMode == 2) {
    Singular.AnyValueChanged = function (prop, args) {
      Singular.SaveObjectDelayed(prop.AttachedTo, Singular.SaveDelay);
    };
  }

};

Singular.PreBind = function (callback) {
  if (!Singular._pvt.OnPreBindCallbacks) Singular._pvt.OnPreBindCallbacks = [];
  Singular._pvt.OnPreBindCallbacks.push(callback);
  return;
}

Singular.OnPageLoad = function () {

  var IsCommonDataLoaded = false;
  var IsDynamicHTMLLoaded = false;

  if (arguments.length >= 1) {
    var eventType = (arguments.length == 2 && arguments[1]) ? 'AfterLoadCallbacks': 'OnLoadCallbacks';
   
    if (!Singular._pvt[eventType]) Singular._pvt[eventType] = [];
    Singular._pvt[eventType].push(arguments[0]);
    return;
  }
  function ShowUI() {
    var jContainer = $('.UIContainer');
    jContainer.show();
    jContainer.css({ visibility: 'visible' });
  }

  function DataLoaded() {
    if (IsCommonDataLoaded && IsDynamicHTMLLoaded) {
         
      //Convert the Json Model into a Knockout View Model.
      //Check for window.ClientData in case Singular.Init wasnt called.
      if (window.ClientData && ClientData.ViewModel) {
        Singular.AddLogEntry('Calling Pre Bind events');
        if (Singular._pvt.OnPreBindCallbacks) {
          for (var i = 0; i < Singular._pvt.OnPreBindCallbacks.length; i++) {
            Singular._pvt.OnPreBindCallbacks[i]();
          }
        }
        Singular.AddLogEntry('Deserialising model');
        Singular.DeserialiseModel(ClientData.ViewModel);
        ShowUI();
        if (Singular.OnModelBound) {
          Singular.OnModelBound();
        }
        if (Singular._pvt.OnLoadCallbacks) {
          for (var i = 0; i < Singular._pvt.OnLoadCallbacks.length; i++) {
            Singular._pvt.OnLoadCallbacks[i]();
          }
        }
      } else {
        ShowUI();
      }

      //Setup the controls on the page.
      Singular.AddLogEntry('Setting up controls');
      Singular.SetupControls();

      if (Singular._pvt.AfterLoadCallbacks) {
        for (var i = 0; i < Singular._pvt.AfterLoadCallbacks.length; i++) {
          Singular._pvt.AfterLoadCallbacks[i]();
        }
      }

      Singular.AddLogEntry('Setup complete');
    }
  }
  
  //Fetch commondata, then setup the viewmodel
  CommonData.FetchClientData(function () {
    IsCommonDataLoaded = true;
    Singular.AddLogEntry('Data fetched');
    DataLoaded();
  });
  DLHTML.LoadControls(function () {
    IsDynamicHTMLLoaded = true;
    Singular.AddLogEntry('Controls fetched');
    DataLoaded();
  });

  window.onbeforeunload = function () {

    if (document.activeElement) {
      $(document.activeElement).blur();
    }

    if (window.DirtyWarning && Singular.CheckVMDirty()) {
      return window.DirtyWarning;
    }
   

    if (Singular.HasPendingSave()) {
      Singular.FlushSavingObjects();
      return "Please wait while the changes you have made are saved."
    }

    if (window.beforeClose) { return window.beforeClose() }
  };

};
Singular.CheckVMDirty = function () {
  var pp = Singular.PrimaryProperties;
  if (pp.length) {
    for (var i = 0; i < pp.length; i++) if (ViewModel[pp[i]]().IsDirty()) return true;
  } else {
    return ViewModel.IsDirty.peek();
  }
}

//Serialises the ViewModel to JSon and stores it in the hidden field.
Singular.IsSerialising = false;
Singular.SerialiseModel = function () {

  if (Singular.IsSerialising) return true;

  function SetModelField() {
    if (window.ViewModel && !Singular.IsStateless) {
      document.getElementById('hModelData').value = JSON.stringify(KOFormatter.Serialise(ViewModel)); //ko.mapping.toJSON(ViewModel);
    };

    //Fix ie<8
    if (Singular.LastButtonClicked && GetIEVersion() < 8) {
      Singular.LastButtonClicked.value = Singular.LastButtonClicked.innerText;
    };

    //Show Loading bar
    Singular.ShowLoadingBar(200);

    //If this post-back is returning a file, we need a timer to keep checking for the presence of the file download token.
    Singular.OnDownloadComplete(function () {
      Singular.HideLoadingBar();
    });
   
  };

  //Encode html editor content
  $('[data-htmlEdit]').each(function () {
    $(this).val($(this).html());
  });

  var PreventPost = Singular.LastButtonClicked && $(Singular.LastButtonClicked).attr('data-validate') == 'PreventPost';
  if (PreventPost) {
    //Re-check rules.
    Singular.CheckPendingFileOperation(function () {
      Singular.Validation.Validate(ViewModel, function (IsValid) {
        if (IsValid) {
          SetModelField();
          Singular.IsSerialising = true;
          $('form').submit();
          Singular.IsSerialising = false;
        } else {
          if ((Singular.Validation.ValidationDisplayMode & 2) != 0) {
            Singular.ShowMessage(LocalStrings.Submit, LocalStrings.JSValidationError2, 2);
          }
        }
      });
    });
    return false;
  } else {
    SetModelField();
    return true;
  }

};




//Gets the JSon from the hidden field, and copies it into the ViewModel.
Singular.DeserialiseModel = function (ModelData, ApplyBindings) {

  //Singular.Validation.DeActivate();
  Singular.SuspendLayout = true;

  if (typeof ModelData == 'string') {
    KOFormatter.Deserialise(ko.utils.parseJson(ModelData), ViewModel);
  } else {
    KOFormatter.Deserialise(ModelData, ViewModel);
  }

  if (ApplyBindings == undefined || ApplyBindings) {
    ko.applyBindings(ViewModel);
  }
  Singular.SuspendLayout = false;
  
  if (Singular.Validation.ValidationMode == 2) {
    Singular.Validation.CheckRules(ViewModel);
  } else {
    Singular.Validation.Reset();
  }

  //Singular.Validation.Activate();
};

Singular.SetEventTarget = function (id, argument) {
  var Target = $("#__EVENTTARGET")[0];
  var Argument = $("#__EVENTARGUMENT")[0];
  if (Target == undefined) {
    $('form').append('<input type="hidden" id="__EVENTTARGET" name="__EVENTTARGET" />');
    $('form').append('<input type="hidden" id="__EVENTARGUMENT" name="__EVENTARGUMENT" />');
    Target = $("#__EVENTTARGET")[0];
    Argument = $("#__EVENTARGUMENT")[0];
  }
  if (id) {
    Target.value = id;
    Argument.value = argument;
  }
};
Singular.DoPostBack = function (id, argument, Validate) {
  if (arguments.length == 3 && !Validate) {
    //Improve in future.
    Singular.LastButtonClicked = undefined;
  }
  Singular.SetEventTarget(id, argument);
  $('form').submit();
};

//Singular.SetLastClickedButton = function (button) {
//  Singular.LastButtonClicked = button;
//  if (button.id) {
//    var arg = button.ButtonArgument ? button.ButtonArgument : '';
//    Singular.SetEventTarget(button.id, arg);
//  }
//}

Singular.OnButtonClick = function (e) {
  Singular.LastButtonClicked = this;
  //Singular.SetLastClickedButton(this);

};
Singular.ButtonClickPost = function (event) {
  var button = event.currentTarget;
  var Validate = $(button).attr('data-validate');
  var Prompt = $(button).attr('data-prompt');

  Singular.LastButtonClicked = button;

  function DoPost() {
    Singular.DoPostBack(button.id, button.ButtonArgument ? button.ButtonArgument : '', Validate);
  }

  if (Prompt) {
    Singular.ShowMessageQuestion('', Prompt, DoPost);
  } else {
    DoPost();
  }
}
Singular.ButtonClickAjax = function (event) {
  var button = event.currentTarget;
  var Validate = $(button).attr('data-validate');
  var LoadText = $(button).attr('data-loadText');
  var Prompt = $(button).attr('data-prompt');

  Singular.LastButtonClicked = button;

  function DoClick() {
    Singular.SendCommand(button.id, { Validate: Validate, LoadingText: LoadText });
  }

  if (Prompt) {
    Singular.ShowMessageQuestion('', Prompt, DoClick);
  } else {
    DoClick()
  }

  //event.stopPropagation();
}