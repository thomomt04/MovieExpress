/// <reference path="Singular.Core.js" />
Singular.Validation = (function () {
  var self = {};

  //Error tooltip reference.
  var _ErrorPopup;

  //Control that hosts the summary message.
  var _MessageControl = null;

  //Summary message control type (fixed height or expanding div)
  var _MessageControlType = 1;

  //Indicates that validation has been run for the first time.
  var _DoneFullValidation = false;

  var _SuspendValidationSummaryUpdating = 0;

  self.IgnoreAsyncRules = false;
  var _ExcludeCheckedRules = false;
  var _AsyncProcessingFinished;

  //Specifies when the validation happens.
  //1 = OnFirstChange, 2 = OnLoad, 4 = OnSubmit
  self.ValidationMode = 0;

  //Specifies how to display errors.
  //1 = Controls, 2 = SubmitMessage, 4 = ValidationSummary
  //7 (1+2+4) = Normal, 5 (1+4) = NoSubmitMessage
  self.ValidationDisplayMode = 0;

  //0 = Right, 1 = Top, 2 = Bottom
  self.ErrorTooltipPosition = 0;
  self.PopupDuration = 300;
  self.PopupBorderWidth = 3;

  self.PreErrorHtml = '';
  self.PreWarningHtml = '';
  self.PreInfoHtml = '';

  //Setup
  $(window).load(function () {

    //Create the popup tooltip.
    $('body').append('<div class="ErrorPopup"><div class="Child Error"></div><div class="Child Warn"></div><div class="Child Info"></div></div>');
    _ErrorPopup = $('.ErrorPopup');
    _ErrorPopup.hide();

  });

  Singular.OnPageLoad(function () {
    SetupValidationSummary();
  });

  self.SuspendValidationSummary = function () {
    _SuspendValidationSummaryUpdating += 1;
  };
  self.ResumeValidationSummary = function (RootObject) {
    _SuspendValidationSummaryUpdating -= 1;
    if (_SuspendValidationSummaryUpdating == 0) {
      self.UpdateValidationSummary(RootObject);
    }
  };
  self.Reset = function () {
    _DoneFullValidation = false;
  }
  self.HasValidated = function () {
    return _DoneFullValidation;
  }

  //#region Rule Object

  self.RuleObject = function (Property, RuleHandler, RuleParameters, AffectedProperty) {

    this.Property = Property;
    this.Object = Property.AttachedTo;
    this.RuleHandler = RuleHandler;
    this.RuleParameters = (RuleParameters ? RuleParameters : {});
    this.RuleError = ko.observable('');
    this._RuleError = '';
    this.AsyncLevel = ko.observable(0);
    this.IgnoreResults = false;
    this.HasRun = false;
    this.IsAsyncRule = false;
    this.Severity = ko.observable(1);
    this._Severity = 1;
    this.AffectedProperty = AffectedProperty;

    this.InAsync = ko.computed(function () {
      return this.AsyncLevel() > 0;
    }, this);

  };

  self.RuleObject.prototype.DiscoverRuleProperties = function () {
    this.IgnoreResults = true;
    try {
      this.RuleHandler(this.Property(), this.RuleParameters, this);
    } finally {
      this.IgnoreResults = false;
    }
  };
  self.RuleObject.prototype.CheckRule = function () {

    var Object = this.Property.AttachedTo;

    if (!_DoneFullValidation && self.ValidationMode != 4) {
      _DoneFullValidation = true;
      //First rule being checked - after this, check all rules.
      setTimeout(function () {
        self.CheckRules(Singular.GetRootObject(Object), false, true);
      }, 0);
    }

    //Set the error to nothing
    if (!this.IsAsyncRule || !self.IgnoreAsyncRules)
      this._RuleError = '';

    //Run the rule handler
    this.RuleHandler(this.Property(), this.RuleParameters, this);

    this.FlushErrorText();

    //mark that this rule has been checked.
    this.HasRun = true;

    //Refresh the error icon, and the validation summary.
    Singular.Validation.RefreshBrokenRuleDisplay(this.AffectedProperty ? this.AffectedProperty : this.Property);

  };
  self.RuleObject.prototype.AddError = function (ErrorText) {
    if (!this.IgnoreResults) {
      var Obj = this.Property.AttachedTo;
      this._Severity = Obj.ErrorsAsWarnings && Obj.ErrorsAsWarnings() ? 2 : 1;
      this._RuleError = ErrorText;
    }
  };
  self.RuleObject.prototype.AddWarning = function (ErrorText) {
    if (!this.IgnoreResults) {
      this._Severity = 2;
      this._RuleError = ErrorText;
    }
  };
  self.RuleObject.prototype.AddInfo = function (ErrorText) {
    if (!this.IgnoreResults) {
      this._Severity = 3;
      this._RuleError = ErrorText;
    }
  };
  self.RuleObject.prototype.ClearError = function () {
    this.AddError('');
  }
  self.RuleObject.prototype.FlushErrorText = function () {
    //If the temp rule error is different to the actual rule error, then set it.
    if (this._RuleError != this.RuleError.peek()) {
      if (this._RuleError == null) this._RuleError = '';
      this.RuleError(this._RuleError);
    }
    this.Severity(this._Severity);
  }
  self.RuleObject.prototype.BeginAsync = function (BusyText) {
    //if (!this.IgnoreResults && !self.IgnoreAsyncRules) {
    this.AsyncLevel(this.AsyncLevel.peek() + 1);
    this.IsAsyncRule = true;
    this.AddError(BusyText ? BusyText : LocalStrings.ASyncRule);
    return true;
    //} else { return false; }
  }
  self.RuleObject.prototype.EndAsync = function () {
    //if (!this.IgnoreResults && !self.IgnoreAsyncRules) {
    this.AsyncLevel(this.AsyncLevel.peek() - 1);

    if (this.AsyncLevel.peek() == 0) {
      this.FlushErrorText();
      self.RefreshBrokenRuleDisplay(this.AffectedProperty ? this.AffectedProperty : this.Property);
      if (_AsyncProcessingFinished) {
        _AsyncProcessingFinished();
      }
      if (AsyncRuleCompletedHandler) {
        AsyncRuleCompletedHandler();
      }
    }
    //}
  }
  self.RuleObject.prototype.CheckRuleASync = function (BusyText, RuleName, Object, AlwaysCheck, DirtyOnly) {
    if (!this.IgnoreResults && (!self.IgnoreAsyncRules || AlwaysCheck) && (!DirtyOnly || (this.Object.IsDirty && this.Object.IsDirty()))) {
      var rSelf = this,
        KeyProperty = this.Object.SInfo.KeyProperty;
      this.BeginAsync(BusyText);

      if (KeyProperty) Object[KeyProperty.PropertyName] = KeyProperty.KeyValue;

      var SendData = {
        Object: Object,
        RuleName: RuleName,
        ObjectType: this.Object.constructor.Type
      };
      if (this.Object.__ContainerProperty) {
        SendData.ContainerGuid = this.Object.__ContainerProperty.AttachedTo.Guid.peek();
        SendData.ContainerProperty = this.Object.__ContainerProperty.PropertyName;
      }
      SendData.Object.Guid = this.Object.Guid();


      Singular.AJAXCall('VM_CheckRulesAsync', SendData, function (Data) {
        var obj = JSON.parse(Data);
        if (obj.Severity == 1) {
          rSelf.AddError(obj.Error);
        }
        if (obj.Severity == 2) {
          rSelf.AddError("");
          rSelf.AddWarning(obj.Error);
        }
        if (obj.Severity == 3) {
          rSelf.AddError("");
          rSelf.AddInfo(obj.Error);
        }
        rSelf.EndAsync();
      }, undefined, 2);
    }
  };
  self.RuleObject.prototype.GetFieldName = function () {
    if (this.Property && this.Property.PropertyName) {
      return this.Property.PropertyName;
    } else {
      return 'Field';
    }
  }

  //#endregion

  var _PendingSummaryRefresh = false;

  var WaitRefreshControlErrorIcon = function (Property) {
    if (!Property.PendingRefresh) {
      Property.PendingRefresh = true;
      setTimeout(function () {
        Property.PendingRefresh = false;
        RefreshControlErrorIcon(Property);
      }, 0)
    }
  }
  var WaitUpdateValidationSummary = function (Object) {
    if (!_PendingSummaryRefresh) {
      _PendingSummaryRefresh = true;
      setTimeout(function () {
        _PendingSummaryRefresh = false;
        Singular.Validation.UpdateValidationSummary(Object);
      }, 0)
    }
  }

  self.RefreshBrokenRuleDisplay = function (Property) {
    if (self.ValidationMode != 4) {
      WaitRefreshControlErrorIcon(Property);
      WaitUpdateValidationSummary(Property.AttachedTo);
    }
  }

  var CheckRulesInternal = function (Object) {
    //loop through the properties on the object
    for (var i = 0; i < Object.SInfo.Properties.length; i++) {
      var prop = Object.SInfo.Properties[i];
      if (prop.Rules) {
        for (var j = 0; j < prop.Rules.length; j++) {
          if (!(_ExcludeCheckedRules && prop.Rules[j].HasRun)) {
            prop.Rules[j].CheckRule();
          }
        }
      }
    }

    //Check all the child objects rules as well.
    for (prop in Object.SInfo.Mapping) {
      var ChildObj = ko.utils.unwrapObservable(Object[prop]);
      if (ChildObj) {
        if (Object.SInfo.Mapping[prop].IsArray) {
          for (var i = 0; i < ChildObj.length; i++) {
            CheckRulesInternal(ChildObj[i]);
          }
        } else {
          CheckRulesInternal(ChildObj);
        }
      }
    }

  }

  //Check rules for a specific property only
  self.CheckObjectPropertyRules = function (property) {
    if (property) {
      if (property.Rules) {
        property.Rules.Iterate(function (itm, ind) {
          itm.CheckRule();
        });
      }
    }
  };

  self.CheckRules = function (Object, RunAsyncRules, ExcludeCheckedRules) {

    self.IgnoreAsyncRules = !RunAsyncRules;
    _ExcludeCheckedRules = ExcludeCheckedRules;

    //if the ValidationMode is OnSumbit, then change it to OnUpdate, so that the user now sees the validation errors clearing as they enter data.
    if (self.ValidationMode == 4) {
      self.ValidationMode = 1;
    }

    _DoneFullValidation = true;

    self.SuspendValidationSummary();

    CheckRulesInternal(Object);

    //Update the summary message.
    self.ResumeValidationSummary(Object);

    self.IgnoreAsyncRules = false;
    _ExcludeCheckedRules = false;

  };

  self.Validate = function (Object, CallBack) {

    if (!Object) Object = ViewModel; //compatibility for old version.

    self.CheckRules(Object);
    if (Object.IsBusy()) {
      Singular.ShowLoadingBar(100, LocalStrings.ASyncRules);
      _AsyncProcessingFinished = function () {
        Singular.HideLoadingBar();
        _AsyncProcessingFinished = null;
        CallBack(Object.IsValid());
      }
    } else {
      if (CallBack) CallBack(Object.IsValid());
    }

    return Object.IsValid(); //compatibility for old version.
  };

  self.IfValid = function (Object, CallBack) {
    var Prompt = LocalStrings.JSValidationPrompt,
      IncludeError = false;
    if (arguments[2] === true) {
      IncludeError = true;
    } else if (arguments[2]) {
      Prompt = arguments[2];
    }

    self.Validate(Object, function (valid) {
      if (valid) {
        CallBack();
      } else {
        if (IncludeError) Prompt += '<br /><br />' + self.GetBrokenRulesHTML(Object);
        Singular.ShowMessage(LocalStrings.JSValidation, Prompt);
      }
    });
  }

  self.RuleCheckComplete = function () {
    return !_PendingSummaryRefresh
  }

  self.UpdateValidationSummary = function (Object, clear) {

    if (_SuspendValidationSummaryUpdating)
      return;

    if ((self.ValidationDisplayMode & 4) == 0)
      return;

    if (_MessageControl == null) {
      //Loop through all the message controls, and find the best one.
      var MessageControls = $("[data-validation-summary]");
      for (var i = 0; i < MessageControls.length; i++) {
        if (_MessageControl == undefined || $(MessageControls[i]).attr('data-validation-summary') > _MessageControlType) {
          _MessageControl = $(MessageControls[i]);
          _MessageControlType = $(MessageControls[i]).attr('data-validation-summary');
        }
      }
    }

    var ViewModel = Singular.GetRootObject(Object);
    var Severity = ViewModel._Severity();

    if (_MessageControl) {
      _MessageControl.removeClass('Msg-Success').removeClass('Msg-Validation').removeClass('Msg-Warning').removeClass('Msg-Information');

      if (Severity < 4) {
        //Errors or warnings
        var Heading = Severity == 1 ? LocalStrings.JSValidationError : (LocalStrings.PleaseNote + ':');
        _MessageControl.html('<strong>' + Heading + '</strong> <br />' + self.GetBrokenRulesHTML(ViewModel));

        _MessageControl.stop();
        _MessageControl.css({ display: '', opacity: 1 });
        if (_MessageControlType != 1) {
          _MessageControl.css('visibility', 'visible');

          _MessageControl.addClass(Severity == 1 ? 'Msg-Validation' : Severity == 2 ? 'Msg-Warning' : 'Msg-Information');
        }
      } else {
        //Valid.
        if (_MessageControlType == 1) {
          _MessageControl.css('display', 'none');
        } else {
          _MessageControl.html('<strong>' + LocalStrings.JSValidationSuccess + '</strong>')
          _MessageControl.addClass('Msg-Success');

          if (clear) {
            _MessageControl.css('visibility', 'hidden');
          } else {
            setTimeout(function () {
              if (_MessageControl.hasClass('Msg-Success')) {
                _MessageControl.fadeOut();
              }
            }, 1000);
          }
        }
      }
    }

  };

  self.GetBrokenRulesHTML = function (RootObject) {
    var ErrorObj = { ErrorHTML: '<ul>' };

    GetBrokenRuleHTMLInternal(RootObject, ErrorObj);

    return ErrorObj.ErrorHTML + '</ul>';
  };

  var EncoderDiv = $('<div/>');
  var GetBrokenRuleHTMLInternal = function (Object, ErrorObj) {
    var PrL = Singular.PrimaryProperties,
      IsVM = Object == ViewModel && PrL.length > 0;

    if (Object._Severity(true) < 4) {
      ErrorObj.ErrorHTML += '<li><strong>' + Object.ToString() + '</strong><ul>';
    }

    //Property errors
    for (var i = 0; i < Object.SInfo.Properties.length; i++) {
      var prop = Object.SInfo.Properties[i];
      if (IsVM && PrL.indexOf(prop.PropertyName) < 0) continue;
      if (prop.Rules && prop.Rules.BrokenRulesString() != '') {
        var Encoded = EncoderDiv.text(prop.Rules.BrokenRulesString()).html();
        ErrorObj.ErrorHTML += '<li class="liVal"><span class="liImg ' + prop.Rules.IconClass() + '"><label for="' + GetControlID(prop) + '" style="cursor: pointer">' + Encoded + '</label></span></li>';
      }
    };

    //Child object errors.
    for (prop in Object.SInfo.Mapping) {
      if (IsVM && PrL.indexOf(prop) < 0) continue;

      if (!Object[prop]._NoRules) {
        var ChildObj = ko.utils.unwrapObservable(Object[prop]);
        if (ChildObj) {
          if (Object.SInfo.Mapping[prop].IsArray) {
            for (var i = 0; i < ChildObj.length; i++) {
              GetBrokenRuleHTMLInternal(ChildObj[i], ErrorObj);
            }
          } else {
            GetBrokenRuleHTMLInternal(ChildObj, ErrorObj);
          }
        }
      }
    }

    if (Object._Severity(true) < 4) {
      ErrorObj.ErrorHTML += '</ul></li>';
    }

  };

  var RefreshControlErrorIcon = function (property) {

    if (self.ValidationDisplayMode == 0) return;

    if (property.BoundElements) {
      var IsValid = property.Rules.BrokenRulesStringIncludingSecondary.peek() == '';

      var PopupControl = _ErrorPopup ? Singular.GetSValue(_ErrorPopup, 'Current') : null;

      for (var i = 0; i < property.BoundElements.length; i++) {
        var jControl = $(property.BoundElements[i]);

        //get max severity
        var MaxSeverity = property.Rules.Severity.peek();

        //Check the type of error
        if (Singular.GetSValue(jControl[0], 'RuleType') == 1) {

          //Icon
          var ImgIcon = Singular.GetSValue(jControl[0], 'RuleIcon');

          if (i < property.BoundElements.length - 1 && jControl[0]._IsDate && property.BoundElements[i + 1]._IsTime) {
            //if there is a date and time editor next to each other, skip the rule on the date
            if (jControl.siblings('input')[0] == property.BoundElements[i + 1]) {
              ImgIcon.hide();
              ImgIcon._LinkedTo = Singular.GetSValue(property.BoundElements[i + 1], 'RuleIcon');
              continue;
            }
          }

          if (!IsValid) {
            ImgIcon._Valid = false;
            ImgIcon.stop();

            //TODO: BM to check for Emile
            var Rules = property.Rules;
            var ErrorText = '', WarnText = '', InfoText = '';
            for (var ii = 0; ii < Rules.length; ii++) {
              var Error = Rules[ii]._RuleError;
              if (Rules[ii]._Severity == 1) ErrorText += (ErrorText == '' || Error == '' ? '' : ', ') + Error;
              if (Rules[ii]._Severity == 2) WarnText += (WarnText == '' || Error == '' ? '' : ', ') + Error;
              if (Rules[ii]._Severity == 3) InfoText += (InfoText == '' || Error == '' ? '' : ', ') + Error;
            }
            //End TODO

            //if Bootstrap styling
            if (Singular.UIStyle && Singular.UIStyle == 2) {
              if (jControl.parent().hasClass('input-group')) {
                //only add the border to the input groups
                jControl.addClass(property.Rules.BorderClass());
                var addOnBtn = jControl.parent().find('.input-group-btn');
                if (addOnBtn.length > 0) {
                  var btn = $(addOnBtn).find('button');
                  var origBtnClass = $(btn).attr('data-original-button-class');
                  if (origBtnClass) {
                    $(btn).removeClass(origBtnClass);
                  }
                  $(btn).addClass('btn-danger');
                }
              } else {
                //normal input control
                if (jControl.parent().hasClass('form-group')) {
                  jControl.parent().addClass('has-error');
                } else {
                  //funky structure
                  if (jControl.closest('div.form-group').length == 1) {
                    jControl.closest('div.form-group').addClass('has-error');
                  }
                }
                //TODO: BM to check for Emile
                //Set Error
                if (self.ValidationDisplayMode === 8) {
                  ImgIcon.attr('class', 'ErrorControlMsg');
                  if (!(ImgIcon.prev('input').hasClass('ErrorControlMsgInput'))) {
                    ImgIcon.prev('input').addClass('ErrorControlMsgInput');
                  }
                  ImgIcon.html(ErrorText);
                } else {
                  //otherwise add the error icon
                  ImgIcon.css({ 'vertical-align': '', display: 'block', opacity: 1 });
                  ImgIcon.attr('class', 'ImgIcon ' + property.Rules.IconClass());
                }
                //End TODO
              }
            } else {
              //if JQuery-UI styling 
              ImgIcon.css({ 'vertical-align': '', 'margin-top': 0, display: 'inline-block', opacity: 1 });
              //TODO: BM to check for Emile
              //Set Error
              if (self.ValidationDisplayMode === 8) {
                ImgIcon.attr('class', 'ErrorControlMsg');
                if (!(ImgIcon.prev('input').hasClass('ErrorControlMsgInput'))) {
                  ImgIcon.prev('input').addClass('ErrorControlMsgInput');
                }
                ImgIcon.html(ErrorText);
              } else {
                ImgIcon.attr('class', 'ImgIcon ' + property.Rules.IconClass());
                if (jControl.height()) {//if the control is not visible height will be 0, which will make alignment worse.
                  var Offset = (jControl.offset().top - jControl.height() / 2) - (ImgIcon.offset().top - ImgIcon.height() / 2);
                  ImgIcon.css('margin-top', Offset);
                } else {
                  ImgIcon.css('vertical-align', 'text-top');
                }
              }
              //End TODO
            }

          } else {
            //is valid.
            if ((ImgIcon.prev('input').hasClass('ErrorControlMsgInput'))) {
              ImgIcon.prev('input').removeClass('ErrorControlMsgInput');
            }
            if (ImgIcon._Valid != undefined) {
              ImgIcon._Valid = true;
              if (self.ValidationDisplayMode !== 8) {
                ImgIcon.attr('class', 'ImgIcon ImgOk');
              }
              function FadeIcon(IconLocal) {
                setTimeout(function () {
                  if (IconLocal._Valid) {
                    IconLocal.fadeOut(300);
                  }
                }, 300);
              }
              FadeIcon(ImgIcon);
            }
            if (Singular.UIStyle && Singular.UIStyle == 2) {

              jControl.removeClass('ValBorderErr').removeClass('ValBorderWarn').removeClass('ValBorderInfo');

              //input-group control
              var addOnBtn = jControl.parent().find('.input-group-btn');
              if (addOnBtn.length > 0) {
                var btn = $(addOnBtn).find('button');
                $(btn).removeClass('btn-danger');
                var origBtnClass = $(btn).attr('data-original-button-class');
                if (origBtnClass) {
                  $(btn).addClass(origBtnClass);
                }
              }

              //normal input control
              if (jControl.parent().hasClass('form-group')) {
                //.hasClass('form-group')
                jControl.parent().removeClass('has-error');
              } else {
                //funky structure
                if (jControl.closest('div.form-group').length == 1) {
                  jControl.closest('div.form-group').removeClass('has-error');
                }
              }

            }
          }

        } else {
          //Red Border
          jControl.removeClass('ValBorderErr').removeClass('ValBorderWarn').removeClass('ValBorderInfo');
          if (!IsValid)
            jControl.addClass(property.Rules.BorderClass());
        }

        //Store the error text on the control.
        Singular.SetSValue(jControl[0], 'Rules', property.Rules);

        //show / hide popup tooltip 
        if (PopupControl == property.BoundElements[i]) {

          if (IsValid) {
            _ErrorPopup.hide();
          } else {
            _ErrorPopup.show();
          }
          if (property.BoundElements[i]._RefreshErrors) property.BoundElements[i]._RefreshErrors();
        }

      }
    }

  };

  var SetupValidationSummary = function () {

    $('.ValidationPopup').each(function () {

      var popupDiv = $(this);

      var Padding = popupDiv.parent().css('padding-top');
      var OriginalHeight = popupDiv.parent().height();
      var HeightDiff = popupDiv.outerHeight() - popupDiv.height();
      popupDiv.css({ 'margin-top': 0, top: Padding, 'box-sizing': 'content-box' });

      popupDiv.height(OriginalHeight - HeightDiff);//this will make the height the same as the content height of the parent, even if using border-box sizing.

      popupDiv.on("mouseenter", function () {
        var InnerHeight = popupDiv[0].scrollHeight;
        if (InnerHeight > OriginalHeight) {
          popupDiv.stop()
            .animate({ height: InnerHeight }, { duration: self.PopupDuration })
            .animate({ duration: self.PopupDuration, queue: false });
        }
      });
      popupDiv.on("mouseleave", function () {
        popupDiv.stop()
          //.animate({ 'border-width': '' }, { duration: self.PopupDuration / 3 })
          .animate({ height: (OriginalHeight - HeightDiff) }, { duration: self.PopupDuration / 2 });
      });

    });

  };

  var AsyncRuleCompletedHandler;

  self.SetAsyncRuleCompletedHandler = function (handler) {
    AsyncRuleCompletedHandler = handler;
  }
  //Sets up a control to be able to show a rule is broken.
  //e.g. adds an error icon to the right of an element.
  self.SetupControl = function (Control, Property) {

    if (self.ValidationDisplayMode == 0) return;
    var jControl = $(Control);
    var ImgIcon;

    //Check if a parent of this control has the 
    // SUI-RuleIcon = 1, or 
    // SUI-RuleBorder = 2 class.
    var RuleTypeControl = jControl.closest('.SUI-RuleBorder, .SUI-RuleIcon')[0];
    var RuleType = 1;
    if (RuleTypeControl && $(RuleTypeControl).hasClass('SUI-RuleBorder')) {
      RuleType = 2;
    }

    if (Singular.UIStyle && Singular.UIStyle == 2) {

    } else {
      //If ruletype is 1, then create the error icon.
      if (RuleType == 1) {
        ImgIcon = $(document.createElement('span'));
        ImgIcon.attr('class', 'ImgIcon');
        if (self.ValidationDisplayMode !== 8) {
          ImgIcon.css({ 'margin-left': '3px' });
        }
        //insert after the input control
        var parent = jControl.parent('.Trigger');
        if (parent.length) {
          parent.append(ImgIcon[0]);
        } else {
          jControl.after(ImgIcon[0]);
        }

        //make it vertically centered.
        ImgIcon.css({ display: 'none' });
        //leave a reference to the icon with the input element.
        Singular.SetSValue(Control, 'RuleType', RuleType);
        Singular.SetSValue(Control, 'RuleIcon', ImgIcon);
      }
    }

    //Setup the error tooltip triggers.
    function MouseEnter() {
      if (ImgIcon && ImgIcon._LinkedTo) ImgIcon = ImgIcon._LinkedTo;
      var CtlPoint = ImgIcon ? ImgIcon : jControl
      var Rules = Singular.GetSValue(Control, 'Rules');
      var ErrorText = '', WarnText = '', InfoText = '';
      for (var i = 0; i < Rules.length; i++) {
        var Error = Rules[i]._RuleError;
        if (Rules[i]._Severity == 1) ErrorText += (ErrorText == '' || Error == '' ? '' : ', ') + Error;
        if (Rules[i]._Severity == 2) WarnText += (WarnText == '' || Error == '' ? '' : ', ') + Error;
        if (Rules[i]._Severity == 3) InfoText += (InfoText == '' || Error == '' ? '' : ', ') + Error;
      }

      if ((ErrorText || WarnText || InfoText) && jControl.is(':visible')) {
        Singular.SetSValue(_ErrorPopup, 'Current', Control);
        _ErrorPopup.children('.Error').css('display', ErrorText == '' ? 'none' : '').html(self.PreErrorHtml + ErrorText);
        _ErrorPopup.children('.Warn').css('display', WarnText == '' ? 'none' : '').html(self.PreWarningHtml + WarnText);
        _ErrorPopup.children('.Info').css('display', InfoText == '' ? 'none' : '').html(self.PreInfoHtml + InfoText);

        _ErrorPopup.show();
        if (self.ErrorTooltipPosition == 0) {
          //Show to the right
          _ErrorPopup.css({ left: 0 }); //Set it to position 0 so we know the height calc will be correct.
          var CSSObj = {};

          var Left = CtlPoint.offset().left + CtlPoint.outerWidth() + 5;
          if ($(document).scrollLeft() + Left + 120 > $(document).width()) {
            //too close to the edge, align it right instead
            _ErrorPopup.css({
              top: CtlPoint.offset().top - _ErrorPopup.children(':visible').outerHeight(),
              left: Left - _ErrorPopup.outerWidth()
            });
          } else {
            _ErrorPopup.css({
              top: CtlPoint.offset().top - ((_ErrorPopup.children(':visible').outerHeight() - CtlPoint.outerHeight()) / 2),
              left: Left
            });
          }
        } else if (self.ErrorTooltipPosition == 1) {
          //Show top
          _ErrorPopup.css({
            left: CtlPoint.offset().left,
            top: CtlPoint.offset().top - _ErrorPopup.outerHeight()
          });
        } else {
          //Show bottom
          _ErrorPopup.css({
            left: CtlPoint.offset().left,
            top: CtlPoint.offset().top + CtlPoint.outerHeight()
          });
        }

      }
    }
    Control._RefreshErrors = MouseEnter;
    function MouseLeave() {
      function HideErrorPopup() {
        _ErrorPopup.hide();
        Singular.SetSValue(_ErrorPopup, 'Current', null);
      }
      try {
        if (Control != document.activeElement) {
          HideErrorPopup();
        }
      } catch (e) {
        HideErrorPopup()
      }
    }

    //TODO: BM to check for Emile
    if (self.ValidationDisplayMode !== 8 || RuleType !== 1) {
      jControl.on('mouseenter focus', MouseEnter);
      jControl.on('mouseleave blur', MouseLeave);
      //also show the tooltip on the input element next to the error icon.
      if (ImgIcon) {
        ImgIcon.on('mouseenter', MouseEnter);
        ImgIcon.on('mouseleave', MouseLeave);
      }
    }
    //End TODO

    RefreshControlErrorIcon(Property);

  };

  self.HideErrorPopup = function () {
    if (_ErrorPopup) {
      _ErrorPopup.hide();
      Singular.SetSValue(_ErrorPopup, 'Current', null);
    }
  }

  //#region Custom methods added by Taariq

  var GetBrokenRulesInfoInternal = function (Object, parent) {

    var info = {
      Guid: Object.Guid(),
      Title: Object.ToString(),
      Message: '',
      Errors: [],
      Warnings: [],
      Information: [],
      ChildList: []
    }
    if (parent) {
      var found = false;
      parent.ChildList.Iterate(function (item, indx) {
        if (item.Guid == Object.Guid()) {
          info = item
          found = true
        }
      })
      if (!found) {
        parent.ChildList.push(info)
      }
    }

    //for each property in the object
    for (var i = 0; i < Object.SInfo.Properties.length; i++) {
      //get the property information
      var prop = Object.SInfo.Properties[i]
      if (prop.Rules) {
        //for each rule in the property
        for (var j = 0; j < prop.Rules.length; j++) {
          var rule = prop.Rules[j]
          //if the rule has errors
          if (rule._RuleError != '') {
            switch (rule._Severity) {
              case 1:
                //error
                if (info.Errors.indexOf(rule._RuleError) < 0) {
                  info.Errors.push(rule._RuleError)
                }
                break;
              case 2:
                //warning
                if (info.Warnings.indexOf(rule._RuleError) < 0) {
                  info.Warnings.push(rule._RuleError)
                }
                break;
              case 3:
                //information
                if (info.Information.indexOf(rule._RuleError) < 0) {
                  info.Information.push(rule._RuleError)
                }
                break;
              default:
                break;
            }
          }
        }
      }
    }

    //Child object errors.
    for (prop in Object.SInfo.Mapping) {
      if (!Object[prop]._NoRules) {
        var ChildObj = ko.utils.unwrapObservable(Object[prop]);
        if (ChildObj) {
          if (Object.SInfo.Mapping[prop].IsArray) {
            for (var i = 0; i < ChildObj.length; i++) {
              GetBrokenRulesInfoInternal(ChildObj[i], info)
            }
          } else {
            GetBrokenRulesInfoInternal(ChildObj, info)
          }
        }
      }
    }

    return info;

  };

  self.HasBrokenRules = function (Object) {

    //Property errors
    for (var i = 0; i < Object.SInfo.Properties.length; i++) {
      var prop = Object.SInfo.Properties[i];
      if (prop.Rules && prop.Rules.BrokenRulesString.peek() != '') {
        return true;
      }
    };

    //Child object errors.
    for (prop in Object.SInfo.Mapping) {
      if (!Object[prop]._NoRules) {
        var ChildObj = ko.utils.unwrapObservable(Object[prop]);
        if (ChildObj) {
          if (Object.SInfo.Mapping[prop].IsArray) {
            for (var i = 0; i < ChildObj.length; i++) {
              if (self.HasBrokenRules(ChildObj[i])) {
                return true;
              }
            }
          } else {
            if (self.HasBrokenRules(ChildObj)) {
              return true;
            }
          }
        }
      }
    }
    return false;
  }

  self.GetBrokenRulesInfo = function (RootObject) {
    var info = GetBrokenRulesInfoInternal(RootObject, null)
    return info
  };

  self.GetBrokenRulesBootstrap = function (RootObject, element) {
    console.log(this);
    var ErrorObject = { ErrorHTML: '', ErrorCount: 0 };
    //<ul></ul>
    GetBrokenRulesInternalBootstrap(RootObject, ErrorObject);
    if (ErrorObject.ErrorHTML.length > 0) {
      ErrorObject.ErrorHTML = '<ul class="error-block">' + ErrorObject.ErrorHTML + '</ul>';
    };
    return ErrorObject.ErrorHTML;
  };

  var EncoderDivBootstrap = $('<div/>');
  var GetBrokenRulesInternalBootstrap = function (Object, ErrorObj) {
    var PrL = Singular.PrimaryProperties,
      IsVM = Object == ViewModel && PrL.length > 0,
      errorCount = 0;

    if (Object._Severity(true) < 4) {
      ErrorObj.ErrorHTML += '<li class="object-toString"><strong>' + Object.ToString() + '</strong><ul class="object-errors">';
    }

    //Property errors
    for (var i = 0; i < Object.SInfo.Properties.length; i++) {
      var prop = Object.SInfo.Properties[i];
      if (IsVM && PrL.indexOf(prop.PropertyName) < 0) continue;
      if (prop.Rules && prop.Rules.BrokenRulesString() != '') {
        var Encoded = EncoderDivBootstrap.text(prop.Rules.BrokenRulesString()).html();
        ErrorObj.ErrorHTML += '<li class="error-item"><span class="error-icon ' + prop.Rules.IconClass() + '"><label class="error-text" for="' + GetControlID(prop) + '">' + Encoded + '</label></span></li>';
      }
    };

    //Child object errors.
    for (prop in Object.SInfo.Mapping) {
      if (IsVM && PrL.indexOf(prop) < 0) continue;

      if (!Object[prop]._NoRules) {
        var ChildObj = ko.utils.unwrapObservable(Object[prop]);
        if (ChildObj) {
          if (Object.SInfo.Mapping[prop].IsArray) {
            for (var i = 0; i < ChildObj.length; i++) {
              GetBrokenRulesInternalBootstrap(ChildObj[i], ErrorObj);
            }
          } else {
            GetBrokenRulesInternalBootstrap(ChildObj, ErrorObj);
          }
        }
      }
    }

    if (Object._Severity(true) < 4) {
      ErrorObj.ErrorHTML += '</ul></li>';
    }

  };

  //#endregion

  //#region  Built in Rules 

  self.Rules = function () {
    var Self = this;

  };



  self.Rules["Required"] = function (Value, Rule, CtlError) {

    //check if this is a required if visible or enabled.
    if (Rule.Args) {
      if (typeof (Rule.Args) == 'function') {
        var Result = Rule.Args.apply(CtlError.Object);
        if (!Result) return;
      }
      if (Rule.Args & 1 == 1 && !$(CtlError.Control).is(':visible')) {
        return;
      }
      if (Rule.Args & 2 == 2 && CtlError.Control.disabled) {
        return;
      }
    }
    //do the check
    if (!$.trim(Value)) {
      if (!Rule.Description)
        CtlError.AddError(CtlError.GetFieldName() + ' is required.');
      else
        CtlError.AddError(Rule.Description);
    };

  };
  self.Rules["MinLength"] = function (Value, Rule, CtlError) {

    if ($.trim(Value).length < Rule.Args) {
      if (Rule.Description == '')
        CtlError.AddError(CtlError.GetFieldName() + ' must be at least ' + Rule.Args + ' characters in length.');
      else
        CtlError.AddError(Rule.Description);
    };

  };
  self.Rules["Number"] = function (Value, Rule, CtlError) {

    if ($.trim(Value).length > 0 && (isNaN(parseFloat(Value)) || !isFinite(Value))) {
      if (!Rule.Description) {
        CtlError.AddError(CtlError.GetFieldName() + ' must be numeric');
        return false;
      } else {
        CtlError.AddError(Rule.Description);
        return false;
      };
    };
    return true;
  };
  self.Rules["Email"] = function (Value, Rule, CtlError) {

    if ($.trim(Value)) {
      //if the control has a value
      var regex = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      if (!regex.test(Value)) {
        if (!Rule.Description)
          CtlError.AddError('Invalid Email Address');
        else
          CtlError.AddError(Rule.Description);
      }
    }


  };
  self.Rules["RegEx"] = function (Value, Rule, CtlError) {

    //if the control has a value
    var regex = new RegExp(Rule.Args);

    if (!regex.test(Value)) {
      if (!Rule.Description)
        CtlError.AddError('Invalid Format');
      else
        CtlError.AddError(Rule.Description);
    }

  };
  self.Rules["Range"] = function (Value, Rule, CtlError) {

    //Needs the number to be numeric.
    if (self.Rules["Number"](Value, Rule, CtlError)) {

      var Min = parseFloat(Rule.Args.split(",")[0]);
      var Max = parseFloat(Rule.Args.split(",")[1]);
      var CtlValue = $.trim(Value);
      CtlValue = CtlValue == '' ? '0' : CtlValue;

      if (CtlValue.length > 0) {
        CtlValue = parseFloat(CtlValue)
        if (CtlValue < Min || CtlValue > Max) {
          if (!Rule.Description)
            CtlError.AddError(CtlError.GetFieldName() + ' must be between ' + Min + ' and ' + Max);
          else
            CtlError.AddError(Rule.Description);
        };
      };
    }

  };

  self.Rules["Rounded"] = function (Value, Rule, CtlError) {

    //Needs the number to be numeric.
    if (self.Rules["Number"](Value, Rule, CtlError)) {

      var Decimals = parseFloat(Rule.Args);
      var DecimalPos = Value.toString().indexOf('.');
      if (DecimalPos > 0 && Value.toString().length - DecimalPos - 1 > Decimals) {
        if (!Rule.Description)
          CtlError.AddError(CtlError.GetFieldName() + ' can only have ' + Decimals + ' decimal places');
        else
          CtlError.AddError(Rule.Description);
      }

    }

  };

  self.Rules.CheckDuplicate = function (Value, Args, CtlError) {
    var sep = String.fromCharCode(30);
    var UIndex = {};
    for (var i = 0; i < Args.List.length; i++) {
      var Key = '';
      Args.Properties.Iterate(function (name) {
        var Val = Args.List[i][name]();
        Key += (Val ? Val.toString().toLowerCase() : '') + sep;
      });
      if (UIndex[Key]) {
        CtlError.AddError(Args.Desc);
        return;
      } else {
        UIndex[Key] = 1;
      }
    }
  }

  //#endregion

  return self;
})();