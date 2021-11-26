"use strict";

ko.bindingHandlers.Rule = {

  update: function (element, valueAccessor, allBindingsAccessor) {
    if (!element.__RulesSetup) {
      var CE = Singular.Validation.RegisterRule(element, valueAccessor().Property.AttachedTo, valueAccessor().Rules);
      element.__RulesSetup = true;
    };
    //Only if the validation mode is OnFirstChange or OnLoad
    if ((Singular.Validation.ValidationMode & 3) == Singular.Validation.ValidationMode) {
      element.__TempValue = valueAccessor().Property();
      Singular.Validation.ValidateControl(element);
      element.__TempValue = undefined;
    }

  }
};

var CreateRule = function (Rules, Property) {
  //Check if there are other trigger properties.
  if (arguments.length > 2) {
    for (var i = 2; i < arguments.length; i++) {
      arguments[i](); //Get the other properties value to trigger a knockout dependency.
    }
  }
  for (var i = 0; i < Rules.Rules.length; i++) {
    if (typeof (Rules.Rules[i].Arg) == 'function') {
      Rules.Rules[i].Arg();
    }
  }

  return { Rules: Rules, Property: Property };
}

Singular.Validation = (function () {
  var self = {};

  //*************************************************************** Private Methods and Variables.
  var MessageControl = null;
  var MessageControlType = 1;
  var DoneFullValidation = false;
  var IsActive = false;
  var ErrorPopup;

  var ErrorContainer = function () {
    var Self = this;
    var Errors = [];

    Self.HasErrors = function () {

      for (var i = 0; i < Errors.length; i++) {
        if (Errors[i].HasErrors())
          return true;
      };
      return false;

    };
    Self.ClearErrors = function () {

      var NewErrors = [];

      for (var i = 0; i < Errors.length; i++) {
        if (ModelBinder.HasGuid(Errors[i].Object.Guid())) {
          NewErrors.push(Errors[i]);
        };
      };

      Errors = NewErrors;
    };

    Self.FindObject = function (Object) {
      for (var i = 0; i < Errors.length; i++) {
        if (Errors[i].Object == Object) {
          return Errors[i];
        };
      };
    };

    Self.FindControl = function (Control) {
      var CtlError;
      for (var i = 0; i < Errors.length && !CtlError; i++) {
        if (!Errors[i].Object.__Deleted) {
          CtlError = Errors[i].ControlErrorList.Find('Control', Control);
        }
      }
      return CtlError;
    };
    Self.CheckAll = function () {

      for (var i = 0; i < Errors.length; i++) {
        for (var j = 0; j < Errors[i].ControlErrorList.length; j++) {
          self.ValidateControl(Errors[i].ControlErrorList[j].Control, Errors[i].ControlErrorList[j])
        };
      };
    };

    Self.AddItem = function (item) {
      Errors.push(item);
    }

    Self.GetErrorHTML = function () {
      var HTML = '<ul>'

      for (var i = 0; i < Errors.length; i++) {
        var ObjError = Errors[i];
        if (ObjError.HasErrors()) {

          var ObjDesc;
          if (ObjError.Object.ToString) {
            ObjDesc = ObjError.Object.ToString.peek();
            ObjDesc = ObjDesc == '' ? 'Blank Item' : ObjDesc;
          }
          HTML += '<li><strong>' + ObjDesc + '</strong></li><ul>';
          for (var j = 0; j < ObjError.ControlErrorList.length; j++) {
            var CtlError = ObjError.ControlErrorList[j];
            if (CtlError.HasErrors()) {
              HTML += CtlError.GetErrorHTML();
            };
          };

          HTML += '</ul>'
        }
      }
      return HTML + '</ul>';
    };


  };

  var ObjectError = function (Object) {
    var Self = this;

    Self.Object = Object;
    Self.ControlErrorList = [];

    Self.HasErrors = function () {
      if (Self.Object.__Deleted) {
        Self.ControlErrorList = [];
        return false;
      }
      Self.Object.IsValid = false;
      for (var i = 0; i < Self.ControlErrorList.length; i++) {
        if (Self.ControlErrorList[i].HasErrors())
          return true;
      };
      Self.Object.IsValid = true;
      return false;
    };

  };

  var AsyncCheckLevel = 0;
  var AsyncProcessingFinished;
  var PageBeginASyncRule = function () {
    AsyncCheckLevel += 1;
  };
  var PageEndASyncRule = function () {
    AsyncCheckLevel -= 1;
    if (AsyncProcessingFinished != undefined) {
      AsyncProcessingFinished();
    }
  };

  var ControlError = function (Parent, Control, RuleOptions) {
    var Self = this;

    Self.Control = Control;
    Self.Errors = '';
    Self.Rules = [];
    Self.Object = Parent.Object;
    Self.AsyncLevel = 0;

    if (RuleOptions) {

      //The Rule Mode
      //  1 = Normal (The error picture is next to the editor.)
      //  2 = Cell (There is no error picture, the editor border must be changed to red.)
      Self.RuleMode = RuleOptions.Mode;

      for (var i = 0; i < RuleOptions.Rules.length; i++) {

        var Rule = {};
        Rule.Name = RuleOptions.Rules[i].Rule;
        Rule.Args = RuleOptions.Rules[i].Arg;
        Rule.Description = RuleOptions.Rules[i].Desc;
        Self.Rules.push(Rule);

        //Check if this rule has a default handler, or if the object handles the rules.
        var RuleHandler = self.Rules[$.trim(Rule.Name)];
        if (RuleHandler) {
          Rule.RuleHandler = RuleHandler;
        } else {
          for (var j = 0; j < Parent.Object.Rules.length; j++) {
            if (Parent.Object.Rules[j].RuleName == Rule.Name) {
              Rule.RuleHandler = Parent.Object.Rules[j].RuleHandler;
              //Ignore this rule in the object rule check, as it is now a control rule.
              Parent.Object.Rules[j].Ignore = true;
              break;
            };
          };
        };

      };

    };

    Self.CheckRuleASync = function (CheckingText, Rule, SendData) {

      //If all the rules are being checked, ignore async rules. Only check if the actual property has changed.
      if (self.SuspendValidationSummaryUpdating) {
        if (Self.AsyncLevel > 0) {
          Self.AddError(CheckingText);
        }
        return;
      }

      BeginAsyncRule(CheckingText);
      SendData.RuleName = Rule.Name;
      SendData.ObjectType = Self.Object.constructor.Type;
      SendData.Guid = Self.Object.Guid();
      Singular.AJAXCall('CheckRulesAsync', SendData, function (Data) {

        EndAsyncRule(Rule, Data);

      });

    };

    var BeginAsyncRule = function (CheckingText) {
      PageBeginASyncRule();
      Self.AsyncLevel += 1;
      Self.AddError(CheckingText);
    }
    var EndAsyncRule = function (Rule, ErrorMessage) {
      Self.Errors = '';
      Rule.LastError = ErrorMessage;
      for (var i = 0; i < Self.Rules.length; i++) {
        if (Self.Rules[i].LastError) {
          Self.Errors += Self.Errors == '' ? Self.Rules[i].LastError : ', ' + Self.Rules[i].LastError;
        };
      };
      Self.AsyncLevel -= 1;
      //Rule.AsyncFinished = true;
      Self.ShowErrorIcon();
      PageEndASyncRule();
    };

    Self.AddError = function (err) {
      //Add the error to the string.
      Self.Errors += Self.Errors == '' ? err : ', ' + err;
      //Set the last error on the error object.
      Self._CurrentRule.LastError = err;
    };
    Self.ClearErrors = function () {
      Self.Errors = '';
    };
    Self.HasErrors = function () {
      return Self.Errors.length > 0;
    };
    Self.GetErrorHTML = function () {
      if (Self.Control == null) {
        return '<li>' + Self.Errors + '</li>';
      } else {
        return '<li><label for="' + Self.Control.id + '" style="cursor:pointer">' + Self.Errors + '</label></li>';
      };

    };

    Self.ShowErrorIcon = function () {

      //Set the Controls Message to hidden / visible.
      if (Self.Control) {

        self.IsValid = !Self.HasErrors();

        var ErrElement;
        if (Self.RuleMode == 1) {
          //Normal Controls with error icon on the right.
          ErrElement = $(Self.Control).next();
          if (!ErrElement[0])
            return;
          ErrElement[0].__ErrorSource = Self.Control;

          ErrElement.css('display', self.IsValid ? 'none' : 'inline-block');
          if (Self.AsyncLevel == 0) {
            ErrElement.removeClass('ImgLoading');
          } else {
            ErrElement.addClass('ImgLoading');
          }

        } else {
          //Cell Controls where the control has a red border.
          ErrElement = $(Self.Control);

          ErrElement.css({ borderColor: self.IsValid ? '' : '#D22' });
          ErrElement.css({ borderStyle: Self.AsyncLevel == 0 ? 'solid' : 'dashed' });
        }

        self.SetControlTooltip(ErrElement[0], self.IsValid ? '' : Self.Errors);


      };

      if (!self.SuspendValidationSummaryUpdating) {
        self.UpdateValidationSummary();
      };

    };

  };

  var ErrorList = new ErrorContainer();

  //*************************************************************** Public Methods and Variables.
  self.IsValid = true;
  self.ValidationDisplayMode = 0;
  self.ValidationMode = 0;

  self.Setup = function () {

    self.SetupValidationSummary();

    self.PendingAsyncOperations = [];

  };

  self.DeActivate = function () {
    IsActive = false;

  }
  self.Activate = function () {

    IsActive = true;
    DoneFullValidation = false;

    //Clears errors for objects that don't exist anymore.
    ErrorList.ClearErrors();

    self.UpdateValidationSummary(true);

  }
  self.IsActive = function () {
    return IsActive;
  }

  self.SetupValidationSummary = function () {

    $('.ValidationPopup').each(function () {

      var popupDiv = $(this);
      var Padding = popupDiv.innerHeight() - popupDiv.height();
      var OuterPadding = popupDiv.parent().innerHeight() - popupDiv.parent().height();
      var OriginalHeight = popupDiv.height() - OuterPadding - 2; //2 for border..
      popupDiv.height(OriginalHeight);

      popupDiv.live("mouseenter", function () {
        var InnerHeight = popupDiv[0].scrollHeight - Padding;
        if (InnerHeight > OriginalHeight) {
          popupDiv.animate({ height: InnerHeight }, { duration: 300 }).animate({ 'border-width': 3 }, { duration: 300, queue: false });
        }
      });
      popupDiv.live("mouseleave", function () {
        popupDiv.animate({ 'border-width': 1 }, { duration: 100 }).animate({ height: OriginalHeight }, { duration: 200 });
      });

    });

  };

  self.SetControlTooltip = function (control, error) {

    if (!ErrorPopup) {
      var x = $('body').append('<div class="ErrorPopup"></div>');
      ErrorPopup = $('.ErrorPopup')[0];
    }
    control.__ErrorTitle = error;
    if (!error) {
      $(ErrorPopup).hide();
    }

    function MouseEnter() {
      if (control.__ErrorTitle && $(control).is(':visible')) {
        ErrorPopup.innerHTML = control.__ErrorTitle;
        $(ErrorPopup).show();
        $(ErrorPopup).css({ 'left': $(control).offset().left + $(control).outerWidth() + 5,
          'top': $(control).offset().top - (($(ErrorPopup).outerHeight() - $(control).outerHeight()) / 2)
        });
      }
    }
    function MouseLeave() {
      $(ErrorPopup).hide();
    }

    if (!control.__ErrorTitleSetup) {

      $(control).live('mouseenter', MouseEnter);
      $(control).live('mouseleave', MouseLeave);
      //also show the tooltip on the input element next to the error icon.
      if (control.__ErrorSource) {
        $(control.__ErrorSource).on('mouseenter focus', MouseEnter);
        $(control.__ErrorSource).on('mouseleave blur', MouseLeave);
      } else {
        $(control).live('focus', MouseEnter);
        $(control).live('blur', MouseLeave);
      }

      control.__ErrorTitleSetup = true;
    }

  };

  self.UpdateValidationSummary = function (clear) {

    if (!IsActive || (self.ValidationDisplayMode & 4) == 0)
      return;

    if (MessageControl == null) {
      //Loop through all the message controls, and find the best one.
      var MessageControls = $("[data-validation-summary]");
      for (var i = 0; i < MessageControls.length; i++) {
        if (MessageControl == undefined || $(MessageControls[i]).attr('data-validation-summary') > MessageControlType) {
          MessageControl = $(MessageControls[i]);
          MessageControlType = $(MessageControls[i]).attr('data-validation-summary');
        }
      }
    }

    var HasErrors = ErrorList.HasErrors()
    if (MessageControl) {
      if (HasErrors) {
        MessageControl.html('<strong>' + LocalStrings.JSValidationError + '</strong> <br />' + ErrorList.GetErrorHTML());

        MessageControl.css('display', '');
        if (MessageControlType != 1) {
          MessageControl.css('visibility', 'visible');
          MessageControl.removeClass('Msg-Success').addClass('Msg-Validation');
        }
      } else {
        if (MessageControlType == 1) {
          MessageControl.css('display', 'none');
        } else {
          MessageControl.html('<strong>' + LocalStrings.JSValidationSuccess + '</strong>')
          MessageControl.removeClass('Msg-Validation').addClass('Msg-Success');

          if (clear) {
            MessageControl.css('visibility', 'hidden');
          } else {
            setTimeout(function () {
              if (MessageControl.hasClass('Msg-Success')) {
                MessageControl.fadeOut();
              }
            }, 1000);
          }

        }
      }
    }

  };

  self.RegisterRule = function (Control, Object, Rules) {

    //Find the object that the control is bound to.
    var BoundObj;
    if (Object != null) {
      BoundObj = Object;
    } else if (Control.__ValueObservable && Control.__ValueObservable.AttachedTo) {
      BoundObj = Control.__ValueObservable.AttachedTo;
    } else {
      BoundObj = ko.dataFor(Control);
    }
    //Get / create the rule object responsible for this object.
    var ObjError = ErrorList.FindObject(BoundObj);
    if (!ObjError) {
      ObjError = new ObjectError(BoundObj);
      ErrorList.AddItem(ObjError);
    }
    //Create the control rule handler for the control.
    var CtlError = new ControlError(ObjError, Control, Rules);
    ObjError.ControlErrorList.push(CtlError);
    if (Control != null) {
      $(Control).attr("data-rules", "true");
    }
    return CtlError;

  };

  self.Validate = function (ShowMessage, OnComplete) {

    //var Control = this;
    self.IsValid = true;
    DoneFullValidation = true;

    self.SuspendValidationSummaryUpdating = true;

    //if the ValidationMode is OnSumbit, then change it to OnUpdate, so that the user now sees the validation errors clearing as they enter data.
    if (self.ValidationMode == 4) {
      self.ValidationMode = 1;
    }

    ErrorList.CheckAll();

    //Update the summary message.
    self.UpdateValidationSummary();

    self.SuspendValidationSummaryUpdating = false;

    function FinishedValidating() {

      self.IsValid = !(ErrorList.HasErrors() || self.PendingAsyncOperations.length > 0);

      //Return false if this needs to prevent post-back.
      if (ShowMessage) {
        if (ErrorList.HasErrors()) {
          if ((self.ValidationDisplayMode & 2) != 0) {
            Singular.ShowMessage(LocalStrings.Submit, LocalStrings.JSValidationError2, 2);
          }
        } else {
          //Make sure there are no async operations in progress.
          if (self.PendingAsyncOperations.length > 0) {
            Singular.ShowMessage(LocalStrings.Submit, LocalStrings.JSValidationError3, 2);
          }
        }

      };
      if (OnComplete) {
        OnComplete(self.IsValid);
      }
    }

    if (AsyncCheckLevel == 0) {
      FinishedValidating();
      return self.IsValid;
    } else {
      AsyncProcessingFinished = FinishedValidating;
    }

    //In case people call if (Validate()) instead of Validate(, onCompleteFunction)
    return false;

  };

  self.ValidateControl = function (Control, CtlError) {

    if (!IsActive && self.ValidationMode != 2)
      return;

    if (!CtlError) {
      CtlError = ErrorList.FindControl(Control);
    }
    if (!CtlError) {

      $(Control).removeAttr("data-rules");
      return;
    }
    CtlError.ClearErrors();

    var Value;
    if (Control) {
      if (Control.__TempValue == undefined) {
        Value = Control.value;
      } else {
        Value = Control.__TempValue;
      };
    };
    for (var idx = 0; idx < CtlError.Rules.length; idx++) {
      //If all the rules are being checked, then skip the async rules that have already passed validation.
      //      if (self.SuspendValidationSummaryUpdating && CtlError.Rules[idx].AsyncFinished && !CtlError.Rules[idx].LastError) {
      //        continue;
      //      }
      CtlError._CurrentRule = CtlError.Rules[idx];
      CtlError._CurrentRule.LastError = '';
      CtlError.Rules[idx].RuleHandler(Value, CtlError.Rules[idx], CtlError);
    };

    CtlError.ShowErrorIcon();

    if (!DoneFullValidation) {
      self.Validate();
      return;
    }

  };

  self.SuspendValidationSummaryUpdating = false;

  self.Rules = function () {
    var Self = this;

  };

  self.Rules["Required"] = function (Value, Rule, CtlError) {

    //check if this is a required if visible or enabled.
    if (Rule.Args) {
      if (typeof (Rule.Args) == 'function') {
        var Result = Rule.Args();
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
        CtlError.AddError('This field is required.');
      else
        CtlError.AddError(Rule.Description);
    };

  };
  self.Rules["MinLength"] = function (Value, Rule, CtlError) {

    if ($.trim(Value).length < Rule.Args) {
      if (Rule.Description == '')
        CtlError.AddError('This field must be at least ' + Rule.Args + ' characters in length.');
      else
        CtlError.AddError(Rule.Description);
    };

  };
  self.Rules["Number"] = function (Value, Rule, CtlError) {

    if ($.trim(Value).length > 0 && (isNaN(parseFloat(Value)) || !isFinite(Value))) {
      if (!Rule.Description) {
        CtlError.AddError('Field must be numeric');
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
            CtlError.AddError('Field must be between ' + Min + ' and ' + Max);
          else
            CtlError.AddError(Rule.Description);
        };
      };

    }

  };

  return self;
})();



