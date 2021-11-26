/// <reference path="Singular.Core.js" /> 
/// <reference path="Singular.Validation2.js" />

ko.bindingHandlers.SValue = {
  init: function (element, valueAccessor, allBindings) {

    ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
    SetupSValue(element, valueAccessor, allBindings);

    //Inline label
    if (allBindings().InlineLabel) {
      var jEl = $(element);

      jEl.focus(function () {
        if (element._NoSelection) {
          element._NoSelection = false;
          element.value = '';
          jEl.removeClass('Blank');
        }
        element._Focused = true;
      }).blur(function () {
        element._Focused = false;
        ko.bindingHandlers.SValue.SetLabelText(element, allBindings);
      });

    }
  },
  update: function (element, valueAccessor, allBindings) {

    if (element.nodeName == 'SELECT' && element.selectedIndex >= 0) {
      valueAccessor().DropDownText = element[element.selectedIndex].text;
    }
    ko.bindingHandlers.value.update(element, valueAccessor, allBindings);
    if (allBindings().InlineLabel) {
      ko.bindingHandlers.SValue.SetLabelText(element, allBindings);
    }

    //Data localisation
    //When a property is changed, record the original english value if this is the first time it is being given a localised value.
    if (valueAccessor().AttachedTo && valueAccessor().AttachedTo.SInfo) {
      var objLocData = valueAccessor().AttachedTo.SInfo.LocalisedData;
      if (objLocData) {
        var PropName = valueAccessor().PropertyName
        if (valueAccessor().AttachedTo.constructor.LocalisedProperties[PropName] && !objLocData[PropName]) {
          if (Singular.SuspendLayout) {
            valueAccessor().OriginalValue = valueAccessor()();
          } else {
            objLocData[PropName] = { Val: valueAccessor().OriginalValue };
          }
        }
      }
    }
  },
  SetLabelText: function (element, ab) {
    if (!element._Focused) {
      element._NoSelection = element.value == '';
      if (element.value == '') {
        element.value = ko.utils.unwrapObservable(ab().InlineLabel);
        $(element).addClass('Blank');
      } else {
        $(element).removeClass('Blank');
      }
    }
  }

}

ko.bindingHandlers.AnyValue = {
  init: function (element, valueAccessor, allBindings) {
    var binding = Singular.UnwrapFunction(valueAccessor());

    if (binding.Type == 's') {
      ko.bindingHandlers.SValue.init(element, function () { return binding.Value }, allBindings);
    } else if (binding.Type == 'n') {
      ko.bindingHandlers.NValue.init(element, function () { return { Value: binding.Value, Format: binding.Format } }, allBindings);
    } else {
      ko.bindingHandlers.DateValue.init(element, function () { return { Value: binding.Value, Format: binding.Format, Simple: true } }, allBindings);
    }
  },
  update: function (element, valueAccessor, allBindings) {
    var binding = Singular.UnwrapFunction(valueAccessor());

    if (binding.Type == 's') {
      ko.bindingHandlers.SValue.update(element, function () { return binding.Value }, allBindings);
    } else if (binding.Type == 'n') {
      ko.bindingHandlers.NValue.update(element, function () { return { Value: binding.Value, Format: binding.Format } }, allBindings);
    } else {
      ko.bindingHandlers.DateValue.update(element, function () { return { Value: binding.Value, Format: binding.Format, Simple: true } }, allBindings);
    }

  }

}

ko.bindingHandlers.SChecked = {
  init: function (element, valueAccessor, allBindings) {
    if (typeof valueAccessor() == "object") {
      //three state
      $(element).click(function (e) {
        var elemValue = element.checked;
        var propValue = valueAccessor().Value.peek();
        if (elemValue == false && propValue == true) {
          valueAccessor().Value(null); //indeterminate is between going from true to false.
        } else if (propValue === null) {
          valueAccessor().Value(false);
        } else {
          valueAccessor().Value(elemValue);
        }
      });
      SetupSValue(element, function () { return valueAccessor().Value }, allBindings);
    } else {
      ko.bindingHandlers.checked.init(element, valueAccessor, allBindings);
      SetupSValue(element, valueAccessor, allBindings);
    }
  },
  update: function (element, valueAccessor, allBindings) {
    var Binding = typeof valueAccessor() == "object" ? function () { return valueAccessor().Value } : valueAccessor;
    var Value = ko.utils.unwrapObservable(Binding());
    element.indeterminate = Value === null;
    if (Value != null) {
      if (ko.bindingHandlers.checked.update) {
        ko.bindingHandlers.checked.update(element, Binding, allBindings);
      } else {
        ko.bindingHandlers.checkedValue.update(element, Binding, allBindings);
      }
    }
  }
}
ko.bindingHandlers.Rule = {
  init: function (element, valueAccessor, allBindings) {

    var Property = valueAccessor();

    Singular.AddElementToProperty(Property, element);

    if (Property.Rules && Property.Rules.length > 0) {
      //if the property has rules
      Singular.Validation.SetupControl(element, Property);
    }

  }
};

Singular.AddElementToProperty = function (Property, Element) {

  if (Property.BoundElements) {
    var IsAdded = false;
    for (var i = Property.BoundElements.length - 1; i >= 0; i--) {
      if (elementInDocument(Property.BoundElements[i])) {
        if (Property.BoundElements[i] == Element) IsAdded = true;
      } else {
        Property.BoundElements.splice(i, 1);
      }
    }
    if (!IsAdded) {
      Property.BoundElements.push(Element);
    }
  }

};

ko.bindingHandlers.SFilter = {
  init: function (element, valueAccessor, allBindingsAccessor) {

    var Table = $(element).closest('table')[0];
    Singular.MakeTableSortable(Table);
    Singular.SetSValue(Table, 'ProcessProperty', valueAccessor());

  }
};

function SetupSValue(element, valueAccessor, allBindings) {

  //Give control a unique id.
  ko.bindingHandlers.UID.init(element, valueAccessor, allBindings);

  var Property = valueAccessor();

  //Remember the controls bound to this property.
  Singular.AddElementToProperty(Property, element);
  $(element).attr("data-datafield", Property.PropertyName);
  element.__BoundProp = Property;

  ko.bindingHandlers.Rule.init(element, valueAccessor, allBindings);

};

ko.bindingHandlers.DropDown = {
  update: function (element, valueAccessor, allBindingsAccessor) {

    var Options = ko.utils.unwrapObservable(valueAccessor());
    var ABA = ko.utils.unwrapObservable(allBindingsAccessor());
    var HasValue = false;
    var Observable = typeof ABA.SValue == "function" ? ABA.SValue : null;
    var ObsValue = Observable ? Observable.peek() : null;

    function CreateOption(Value, Display, Title) {
      var option = document.createElement("OPTION");
      Value = ko.utils.unwrapObservable(Value);
      Display = ko.utils.unwrapObservable(Display);

      //option.innerHTML = Display.toString();
      if (ObsValue == Value) {
        HasValue = true;
        if (Observable) {
          Observable.DropDownText = Display;
        }
      }
      option.innerHTML = Display;
      if (Title) {
        option.title = ko.utils.unwrapObservable(Title);
      }
      //option.value = Value;
      ko.selectExtensions.writeValue(option, Value);
      return option;

    };

    function BuildOptions(Source, ClearIfMissing) {

      var RequiresRebuild = false;

      //Check if the new list is different to the old list.
      if (element.PrevList) {
        if (element.PrevList.length != Source.length) {
          RequiresRebuild = true;
        } else {
          for (var i = 0; i < Source.length && !RequiresRebuild; i++) {
            if (ABA.optionsGroupList) {
              var ChildList1 = Source[i][ABA.optionsGroupList];
              var ChildList2 = element.PrevList[i][ABA.optionsGroupList];
              if (ChildList1.length != ChildList2.length) {
                RequiresRebuild = true;
              } else {
                for (var j = 0; j < ChildList1.length; j++) {
                  if (ChildList1[j][ABA.optionsValue] != ChildList2[j][ABA.optionsValue]) {
                    RequiresRebuild = true;
                    break;
                  }
                }
              }
            } else {
              if (element.PrevList[i][ABA.optionsValue] != Source[i][ABA.optionsValue]) {
                RequiresRebuild = true;
              }
            }

          }
        }
      } else {
        RequiresRebuild = true;
      }

      if (RequiresRebuild) {
        element.PrevList = Source;
        var PrevValue = element.value;

        // Clear existing elements
        if (element.length > 0) {
          ko.cleanNode(element.options[0]);
          element.remove(0);
        }
        element.innerHTML = "";

        //Create caption
        if (ABA.optionsCaption) {
          var option = document.createElement("OPTION");
          option.innerHTML = ABA.optionsCaption;
          ko.selectExtensions.writeValue(option, undefined);
          element.appendChild(option);
        };

        //Create options
        for (var i = 0; i < Source.length; i++) {

          if (ABA.optionsGroupList) {

            var optgroup = null;
            optgroup = document.createElement("OPTGROUP");
            optgroup.label = Options.Source[i][ABA.groupText];
            element.appendChild(optgroup);

            var ChildList = Source[i][ABA.optionsGroupList];

            for (var childIdx = 0; childIdx < ChildList.length; childIdx++) {
              optgroup.appendChild(CreateOption(ChildList[childIdx][ABA.optionsValue], ChildList[childIdx][ABA.optionsText]));
            };

          } else {
            if (ABA.attr && ABA.attr.itemTitle) {
              element.appendChild(CreateOption(Source[i][ABA.optionsValue], Source[i][ABA.optionsText], Source[i][ABA.attr.itemTitle]));
            } else {
              element.appendChild(CreateOption(Source[i][ABA.optionsValue], Source[i][ABA.optionsText]));
            }
          }

        };

        element.value = PrevValue;

        if (!HasValue && ObsValue && ClearIfMissing) {
          ABA.SValue(null);
        }

      };

    };



    if (Options.Source) {
      BuildOptions(Options.Source, true);
    } else if (Options.Filter) {

      if (Options.Filter()) {

        if ((!element.__AjaxFilterValue || element.__AjaxFilterValue != Options.Filter())) {
          element.__AjaxFilterValue = Options.Filter();

          var img;
          if ($(element).offset().top != 0) {
            img = document.createElement('img');
            img.src = Singular.RootPath + '/Singular/Images/LoadingSmall.gif';
            $(img).css({ position: 'fixed', top: $(element).offset().top + 2, left: $(element).offset().left + 4 });
            $(element).parent()[0].appendChild(img);
          }
          BuildOptions([], false);
          var Args = { Context: Options.AjaxName };
          Args[Options.FilterName] = Options.Filter();
          Singular.AJAXCall('GetData', Args, function (Data) {

            ClientData[Options.AjaxName] = JSON.parse(Data);
            BuildOptions(ClientData[Options.AjaxName], true);
            if (img) {
              $(element).parent()[0].removeChild(img);
            }

            if (Options.AfterFetchJS) Options.AfterFetchJS();

          });

        }
      } else {
        element.__AjaxFilterValue = null;
        BuildOptions([], false);
      }


    } else {
      throw "Can't find drop down datasource " + ABA.optionsValue;
    }

  }
};

//Numeric Value
ko.bindingHandlers.NValue = {
  init: function (element, valueAccessor, allBindings) {
    if (element.nodeName.toLowerCase() == 'input') {
      var binding = ko.isObservable(valueAccessor()) ? { Value: valueAccessor() } : valueAccessor();
      var IsPercent = ko.utils.unwrapObservable(binding.Format) && ko.utils.unwrapObservable(binding.Format).indexOf('%') >= 0;

      SetupSValue(element, function () { return binding.Value }, allBindings);

      var Event = 'change';
      if (allBindings().valueUpdate) {
        Event = 'keyup';
      }
      //Elements on changed event.
      $(element).on(Event, function (args) {
        //Handle 'null' value before replacing whitespace and ','
        var Value = this.value || '';
        Value = parseFloat(Value.replace(/\ |\,/g, ''));
        //var Value = parseFloat(this.value)
        if (ko.bindingHandlers.NValue.AutoPercent && IsPercent) {
          Value /= 100;
        }

        ko.bindingHandlers.NValue.SetObs(binding.Value, Value);
      });

      //select all text on focus
      var HadMouseFocus = false;
      $(element).mousedown(function () {
        HadMouseFocus = document.activeElement == element;
        if (ko.bindingHandlers.NValue.ClearOnFocus && element.value == 0) element.value = '';
      });
      $(element).focus(function () {
        var Val = ko.utils.unwrapObservable(binding.Value); //remove formatting.
        if (ko.bindingHandlers.NValue.AutoPercent && IsPercent && Val) Val *= 100;

        if (element.value !== '0') {
          element.value = Val ? Val.formatDotNet('#.##########;-#.##########') : '';
        }
        Singular.SetSValue(element, 'Focused', true);
        if (!IsMobile()) { //on mobile, this causes the 'copy paste etc' options to come up, which cant be dismissed. It is very annoying.
          element.select();
        }
      });
      $(element).mouseup(function (e) {
        if (!HadMouseFocus && !ko.bindingHandlers.NValue.ClearOnFocus) {
          e.preventDefault();
          element.select();
        }
      });


      //Make sure the elements value is up to date on de-focus.
      $(element).blur(function () {
        Singular.SetSValue(this, 'Focused', false);
        ko.bindingHandlers.NValue.SetDisplay(binding, this);
      });
    }

  },
  update: function (element, valueAccessor) {
    var binding = ko.isObservable(valueAccessor()) ? { Value: valueAccessor() } : valueAccessor();
    var value = ko.utils.unwrapObservable(binding.Value);

    ko.bindingHandlers.NValue.SetObs(binding.Value, value);

    //Only set the element value if the element is not focused.
    //If it is focused, it will be up to date, because the user is typing, and causing the value to be updated.
    if (!Singular.GetSValue(element, 'Focused')) {
      ko.bindingHandlers.NValue.SetDisplay(binding, element);
    }

  },
  SetObs: function (obs, value) {
    try {
      if (ko.isWriteableObservable(obs)) {
        //Make sure the value is numeric.
        value = parseFloat(value);
        if (value === null || isNaN(value)) {
          value = obs.AllowNulls ? null : 0;
        }
        obs(value);
      }
    } catch (e) {
      alert(e);
    }
  },
  SetDisplay: function (va, element) {
    var value = ko.utils.unwrapObservable(va.Value);
    var format = ko.utils.unwrapObservable(va.Format);
    if (va.CSymbol) {
      format = format.format(ko.utils.unwrapObservable(va.CSymbol)); //Format string will be in the format: '{0} #,##...'
    }
    var isEmpty = value == null || value === '';
    if (element.nodeName.toLowerCase() == 'input') {
      element.value = isEmpty ? '' : (format ? value.formatMoney(format) : value);
    } else {
      element.innerHTML = isEmpty ? '' : (format ? value.formatMoney(format) : value);
    }


  }
};
ko.bindingHandlers.NValue.ClearOnFocus = false;
ko.bindingHandlers.NValue.AutoPercent = true;

ko.bindingHandlers.ButtonArgument = {
  init: function (element, valueAccessor) {
    ko.bindingHandlers.ButtonArgument.update(element, valueAccessor);
  },
  update: function (element, valueAccessor) {
    var binding = ko.utils.unwrapObservable(valueAccessor());
    element.ButtonArgument = binding;
  }
};
//Animated Visible Binding
ko.bindingHandlers.visibleA = {
  init: function (element, valueAccessor) {
    //on initial load, dont do the animation.
    var Val = ko.utils.unwrapObservable(valueAccessor());
    $(element).css({ display: ((Val instanceof Object ? ko.utils.unwrapObservable(Val.Condition) : Val) ? '' : 'none') });
  },
  update: function (element, valueAccessor) {

    var MustDisplay, In = 4, Out = 3, Val = ko.utils.unwrapObservable(valueAccessor()),
      Handler;
    if (Val instanceof Object) {
      MustDisplay = ko.utils.unwrapObservable(Val.Condition);
      In = Val.In;
      Out = Val.Out;
      Handler = Val.Handler
    } else {
      MustDisplay = Val;
    }
    //if its a table row, find the table inside the row to animate.
    var ParentElem, DurationOut = 400, DurationIn = 400;
    if (element.nodeName == 'TR') {
      DurationIn = 200;
      DurationOut = 100;
      ParentElem = element;
      element.style.display = '';
      if (!element.__WrappedTable) {
        //var wrapper = document.createElement('div');
        //wrapper.style.overflow = 'hidden';
        //$(element).find('table').wrapAll(wrapper);
        //element.__WrappedTable = wrapper;
        element.__WrappedTable = $(element).find('table').wrapAll('<div style="overflow: hidden"></div>').parent()[0];
      }
      element = element.__WrappedTable
    }
    var HideComplete = function () {
      if (ParentElem) {
        ParentElem.style.display = 'none';
        element.style.display = 'none';
      }
      if (Handler) Handler(element, false, true);//hide complete
    };
    var ShowComplete = function () {
      if (Handler) Handler(element, true, true);//show complete
    };
    var BeginHandler = function (MustDisplay) {
      if (Handler) Handler(element, MustDisplay, false);//hide / show start
    }
    ko.bindingHandlers.visibleA.ShowHide(element, MustDisplay, MustDisplay ? ShowComplete : HideComplete, MustDisplay ? In : Out, MustDisplay ? DurationIn : DurationOut, BeginHandler);

  }
};
ko.bindingHandlers.visibleA.ShowHide = function (element, MustDisplay, OnComplete, Type, Duration, BeginHandler) {
  var IsDisplayed = element.style.display != 'none';
  if ((MustDisplay && (!IsDisplayed || element.__BusyFading == 'out'))) {
    if (BeginHandler) BeginHandler(true);
    switch (Type) {
      case 1: $(element).show(0, OnComplete); break;
      case 2: $(element).fadeIn(Duration, OnComplete); break;
      case 3: $(element).slideDown(Duration, OnComplete); break;
      case 4: $(element).toggle('drop'); break;
    }
    element.__BusyFading = 'in';
  } else if (!MustDisplay && (IsDisplayed || element.__BusyFading == 'in')) {
    if (BeginHandler) BeginHandler(false);
    switch (Type) {
      case 1: $(element).hide(); OnComplete(); break;
      case 2: $(element).fadeOut(Duration, OnComplete); break;
      case 3: $(element).slideUp(Duration, OnComplete); break;
      case 4: $(element).toggle('drop'); break;
    }
    element.__BusyFading = 'out';
  }
}

//#region  Dialog 

ko.bindingHandlers.dialog = {
  init: function (element, valueAccessor) {
    var jElement = $(element);

    Singular.SetSValue(element, 'DWidth', jElement.width());
    Singular.SetSValue(element, 'DHeight', element.style.height);
    jElement.hide();
    setTimeout(function () {
      jElement.dialog({ autoOpen: false });
      ko.bindingHandlers.dialog.ShowHide($(element), valueAccessor());
    }, 0);

  },
  update: function (element, valueAccessor) {
    ko.bindingHandlers.dialog.ShowHide($(element), valueAccessor());
  },
  ShowHide: function (jElement, binding) {
    var MustDisplay = ko.utils.unwrapObservable(binding.Show);
    if (jElement.hasClass('ui-dialog-content')) {

      var IsDisplayed = jElement.dialog("isOpen");
      var Title = ko.utils.unwrapObservable(binding.Title),
        classname = binding.DialogClass ? ko.utils.unwrapObservable(binding.DialogClass) : '',
        closeText = binding.CloseText ? ko.utils.unwrapObservable(binding.CloseText) : '',
        draggable = binding.Draggable == undefined ? true : binding.Draggable,
        resizable = binding.Resizable == undefined ? true : binding.Resizable,
        width = binding.Width ? ko.utils.unwrapObservable(binding.Width) : Singular.GetSValue(jElement[0], 'DWidth');

      if (MustDisplay && !IsDisplayed) {
        setTimeout(function () {
          //Set timeout is so that knockout can render the inner html before jquery calculates the height for centering.
          var options = { title: Title, modal: true, width: width, draggable: draggable, resizable: resizable };
          if (classname != '') {
            options.dialogClass = classname;
          }
          if (closeText != '') {
            options.closeText = closeText;
          }
          var Height = Singular.GetSValue(jElement[0], 'DHeight');
          if (Height && parseInt(Height) != 0) options.height = parseInt(Height);
          if (options.width == 0) options.width = 'auto';
          if (binding.beforeClose) options.close = binding.beforeClose;
          if (binding.onOpen) options.open = binding.onOpen;
          jElement.attr('data-dlg', 'opening');
          jElement.dialog(options);
          jElement.dialog('open');

          setTimeout(function () {
            jElement.attr('data-dlg', 'opened');
          }, 0);
        }, 0);
      }
      if (IsDisplayed) {
        if (MustDisplay) {
          jElement.dialog({ title: Title });
        } else {
          jElement.dialog('close');
        }
      }
    }
  }
};

//#endregion

ko.bindingHandlers.enable = {
  update: function (element, valueAccessor) {

    var Value = ko.utils.unwrapObservable(valueAccessor());
    if ($(element).is('button')) {
      if (Value) {
        $(element).removeClass('ui-state-disabled');
      } else {
        $(element).addClass('ui-state-disabled');
      }
    }

    if (Value) {
      $(element).removeAttr('disabled');
    } else {
      $(element).attr('disabled', 'disabled');
    }

  }
};

ko.bindingHandlers.enableChildren = {
  update: function (element, valueAccessor, allBindingsAccessor) {
    var Value = ko.utils.unwrapObservable(valueAccessor());
    var OldValue;
    if (element.__ChildrenEnabled == undefined) {
      OldValue = !Value;
    } else {
      OldValue = element.__ChildrenEnabled;
    }

    if (Value && !OldValue) {
      //Enable
      $(element).find('input, select, textarea, button').attr('disabled', false);
    } else {
      //Disable
      $(element).find('input, select, textarea, button').attr('disabled', true);
    }
  }
};

ko.bindingHandlers.ValueLookup = {
  init: function (element, valueAccessor, allBindingsAccessor, arg3, arg4) {

    SetupSValue(element, valueAccessor, allBindingsAccessor);

    $(element).on("change", function () {
      //If the value is '', then change it to null, since this is an ID property.
      valueAccessor()(element.value == '' ? null : element.value);
    });

    ko.bindingHandlers.ValueLookup.update(element, valueAccessor, allBindingsAccessor);
  },
  update: function (element, valueAccessor, allBindingsAccessor) {
    var bindingValue = ko.utils.unwrapObservable(valueAccessor());
    var List = allBindingsAccessor().LookupList;

    if (bindingValue != null) {
      var Obj = List.Find(allBindingsAccessor().optionsValue, bindingValue);
      if (Obj != null) {
        if (element.nodeName.toLowerCase() == 'span') {
          element.innerHTML = Obj[allBindingsAccessor().optionsText];
        } else {
          element.value = Obj[allBindingsAccessor().optionsText];
        }

      }
    } else {
      if (element.nodeName.toLowerCase() == 'span') {
        element.innerHTML = '';
      } else {
        element.value = '';
      }
    }

  }
};

//#region  Date 
Singular.DateFormat = 'dd MMM yyyy';
Singular.DateButton = false;
Singular.DateEditorOnReadOnly = false;
ko.bindingHandlers.DateValue = {
  init: function (element, valueAccessor, allBindingsAccessor, vm, arg4) {

    var binding = valueAccessor();
    var DateFormat = binding.Format ? binding.Format : Singular.DateFormat;
    Singular.SetSValue(element, 'Format', DateFormat);

    if (element.nodeName.toLowerCase() == 'input' || element.nodeName.toLowerCase() == 'div') {

      SetupSValue(element, function () { return binding.Value }, allBindingsAccessor);

      if (Singular.DateEditorOnReadOnly || !$(element).is('[readonly]')) {

        if (binding.Type != 'Time') {
          element._IsDate = true;

          if (!binding.Simple) {

            //Convert the .Net format to JQuery format
            var jqFmt = DateFormat_Net_to_JS(DateFormat);

            //if this needs a button to show the date picker.
            if (Singular.DateButton) {
              $(element).wrap('<div class="Trigger DateTrigger"></div>');
              var Span = document.createElement('span');
              $(Span).insertAfter(element);
              $(Span).click(function () {
                if (!element.disabled) {
                  $(element).datepicker('show');
                }
              });
              $(element).width($(element).outerWidth() - $(Span).outerWidth(true));
            }

            var dpOptions = {
              changeMonth: true, changeYear: true, dateFormat: jqFmt, showOn: (Singular.DateButton ? 'none' : 'focus'),
              constrainInput: false, showOtherMonths: true, selectOtherMonths: true
            };
            if (binding.Initial) {
              dpOptions.defaultDate = Singular.UnwrapFunction(binding.Initial, vm);
            }
            dpOptions.onSelect = function (newValue) {
              //When the user selects a date with the date picker.
              var jDp = $(this);
              ko.bindingHandlers.DateValue.SetObs(binding, jDp.datepicker("getDate"));
              
              jDp.parent().focus(); //hack to defocus input on jqueryui 1.12
            }
            //If there is a custom yeat range.
            if (binding.YearRange) {
              dpOptions.yearRange = binding.YearRange;
            }
            //if the date must change automatically when scrolling through months.
            var AutoChangeType = binding.AutoChange;
            if (AutoChangeType) {
              dpOptions.onChangeMonthYear = function (year, month, inst) {
                var NewDate = new Date(year, month - 1, 1);
                NewDate = AutoChangeType == 1 ? NewDate : Singular.DateMonthEnd(NewDate);
                ko.bindingHandlers.DateValue.SetObs(binding, NewDate);
                $(element).datepicker('setDate', NewDate);
              }
            }
            dpOptions.beforeShow = function (input, inst) {

              inst.dpDiv.removeClass('NoDays');
              if (binding.MonthOnly) inst.dpDiv.addClass('NoDays');
            }
            //Month only
            if (binding.MonthOnly) {
              dpOptions.showButtonPanel = true;
              dpOptions.onClose = function (v, inst) {

                if (inst.dpDiv.find('.ui-datepicker-close.ui-state-hover').length) { //(!inst.lastVal || element.value) {
                  //only change if the user hasn't cleared the input
                  ko.bindingHandlers.DateValue.SetObs(binding, new Date(inst.selectedYear, inst.selectedMonth, 1));
                }
              }
            }

            //select text on focus
            Singular.OnFocus(element, function () {
              if (!IsMobile()) {
                element.select();
              }
            });
            //Create the jquery datepicker.
            $(element).datepicker(dpOptions);

          }

        } else {
          element._IsTime = true;
          //For Time, just put a mask on the input.
          $(element).mask("99:99", '*');
          Singular.OnFocus(element, function () {
            element.select();
          });
        }

        $(element).change(function () {
          //When the user types in a date / time
          ko.bindingHandlers.DateValue.SetObs(binding, binding.Type != 'Time' ? (binding.DefaultValue != null ? this.value.ParseDate(DateFormat, binding.DefaultValue) : this.value.ParseDate(DateFormat)) : this.value);
          if (binding.Simple) {
            //element.value = binding.Value.peek().format(DateFormat);
            element.value = binding.Value.peek() ? binding.Value.peek().format(DateFormat) : '';
          }
        });
      }
    }
  },
  SetObs: function (binding, NewValue) {
    //Set the date / time while keeping the other half of the date / time component.
    var OldValue = binding.Value.peek();
    var IsTime = binding.Type == 'Time';

    NewValue = NewValue instanceof Date ? (IsTime ? NewValue.format('HH:mm:ss') : NewValue.format('dd MMM yyyy')) : NewValue;
    if (NewValue) {
      NewValue = GetUpdatedDateTimeProperty(NewValue, OldValue, binding.Type)
    }
    //Only allow in range of min and max dates.
    if (NewValue) {
      NewValue = GetDateInRange(NewValue, binding.MaxDate ? ko.utils.unwrapObservable(binding.MaxDate) : null, binding.MinDate ? ko.utils.unwrapObservable(binding.MinDate) : null);
    }

    if (NewValue && OldValue) {
      var nv = new Date(NewValue)
      var ov = new Date(OldValue)
      if (nv.getTime() != ov.getTime()) {
        binding.Value(NewValue);
      }
    } else {
      binding.Value(NewValue);
    }

  },
  update: function (element, valueAccessor, allBindingsAccessor) {

    var ObsVal = ko.utils.unwrapObservable(valueAccessor().Value);
    //Check that the value is a date, not a string.
    if (ObsVal === '') {
      ObsVal = null;
    }
    if (ObsVal != null) {
      if (!(ObsVal instanceof Date)) {
        ObsVal = new Date(ObsVal);
      }
    }

    if (element.nodeName.toLowerCase() == 'input' || element.nodeName.toLowerCase() == 'div') {

      if (valueAccessor().Type == 'Time') {
        element.value = ObsVal == null ? '' : ObsVal.format('HH:mm');
      } else {

        function SetMinMax(Type, NewValue) {
          var OldDate = $(element).datepicker("option", Type);
          if (!OldDate || OldDate.getTime() != NewValue.getTime()) {
            $(element).datepicker("option", Type, NewValue);
          }
        }

        if (valueAccessor().MaxDate) {
          SetMinMax('maxDate', new Date(ko.utils.unwrapObservable(valueAccessor().MaxDate)));
        }
        if (valueAccessor().MinDate) {
          SetMinMax('minDate', new Date(ko.utils.unwrapObservable(valueAccessor().MinDate)));
        }
        if (element.nodeName.toLowerCase() == 'input') {
          //Drop down from text box
          element.value = ObsVal == null ? '' : ObsVal.format(Singular.GetSValue(element, 'Format'));
        } else {
          //inline date picker.
          $(element).datepicker('setDate', ObsVal);
        }


      }
    } else {
      //not input
      element.innerHTML = ObsVal == null ? '' : ObsVal.format(Singular.GetSValue(element, 'Format'));
    }
  }
};

function GetUpdatedDateTimeProperty(NewValue, OldValue, BindingType) {
  if (OldValue) {
    OldValue = OldValue instanceof Date ? OldValue : new Date(OldValue);
    //Check is Valid Date - Needed if "OldValue" is formatted that Javascript cannot understand
    if (isNaN(OldValue.valueOf())) { OldValue = new Date(); };
    OldValue = BindingType == 'Time' ? OldValue.format('dd MMM yyyy') : OldValue.format('HH:mm:ss');
    if (BindingType == 'Time') {
      return new Date(OldValue + ' ' + NewValue);
    } else {
      return new Date(NewValue + ' ' + OldValue);
    }
  } else {
    return new Date(BindingType == 'Time' ? new Date().format('dd MMM yyyy') + ' ' + NewValue : NewValue);
  }
}

function GetDateInRange(Value, MaxValue, MinValue) {
  if (MaxValue && Value.getTime() > new Date(MaxValue).getTime()) {
    return new Date(MaxValue);
  }
  if (MinValue && Value.getTime() < new Date(MinValue).getTime()) {
    return new Date(MinValue);
  }
  return Value
}

function DateFormat_Net_to_JS(format) {
  var jqFmt = format.replace(/dddd/, 'DD').replace(/ddd/, 'D');
  if (jqFmt.indexOf('MMM') >= 0 || jqFmt.indexOf('MMMM') >= 0) {
    jqFmt = jqFmt.replace(/MMMM/, 'MM').replace(/MMM/, 'M');
  } else {
    jqFmt = jqFmt.replace(/MM/, 'mm').replace(/M/, 'm');
  }
  if (jqFmt.indexOf('yyyy') >= 0) {
    jqFmt = jqFmt.replace(/yyyy/, 'yy');
  } else {
    jqFmt = jqFmt.replace(/yy/, 'y');
  }
  return jqFmt;
}

//#endregion

ko.bindingHandlers.StarRating = {
  init: function (element, valueAccessor, allbindings) {
    $(element).addClass("StarRating");
    var Tooltips;
    if (valueAccessor().ToolTipDataSourceName) {
      Tooltips = ClientData[valueAccessor().ToolTipDataSourceName];
    };
    if (valueAccessor().ToolTipDataSource && !Tooltips) {
      Tooltips = valueAccessor().ToolTipDataSource;
    }
    for (var i = 0; i < valueAccessor().MaxPoints; i++) {
      $("<span title='" + (Tooltips ? (Tooltips[i].Val ? Tooltips[i].Val : Tooltips[i]) : (i + 1)) + "'>").appendTo(element);
    };
    //HAndle mouse events on the stars
    $("span", element).each(function (index) {

      $(this).hover(function () {
        if (allbindings().enable) {
          $("span", element).each(function (index) {
            $(this).addClass("NoHover");
          });
          $(this).prevAll().add(this).addClass("hoverChosen");
        };
      }, function () {
        if (allbindings().enable) {
          $("span", element).each(function (index) {
            $(this).removeClass("NoHover");
          });
          $(this).prevAll().add(this).removeClass("hoverChosen");
        };
      }).click(function () {
        if (allbindings().enable) {
          var observable = valueAccessor().Value;
          observable(index + 1);
        };
      });
    });
  },
  update: function (element, valueAccessor, allbindings) {
    //Give the first x stars the "chosen" class, where x <= rating
    var observable = valueAccessor().Value;
    $("span", element).each(function (index) {
      $(this).toggleClass("chosen", index < observable());
    });
  }
};

ko.bindingHandlers.Mask = {
  update: function (element, valueAccessor, allBindingsAccessor) {

    var mask, placeholder;

    if (typeof valueAccessor() == "object") {
      //More than just mask
      var binding = valueAccessor();
      mask = binding.Mask;
      placeholder = binding.Placeholder;
    } else {
      mask = ko.utils.unwrapObservable(valueAccessor());
    }

    if (mask) {
      if (placeholder) {
        $(element).mask(mask, { placeholder: placeholder });
      } else {
        $(element).mask(mask);
      }
    } else {
      $(element).unmask();
    }
  }
};
ko.bindingHandlers.UID = {
  init: function (element, valueAccessor, allBindingsAccessor) {
    var id = GetControlID(Singular.UnwrapFunction(valueAccessor));
    if (allBindingsAccessor().id_sfx) id += ko.utils.unwrapObservable(allBindingsAccessor().id_sfx);
    if (element.nodeName == 'LABEL') {
      element.setAttribute('for', id);
    } else {
      element.id = id;
    }
  }
};
//Gets a unique control id for the property / object combination.
function GetControlID(Obs) {
  if (typeof (Obs) == 'string') {
    return Obs;
  } else {
    if (!Obs.PropertyName) {
      return '';
    } else {
      return Obs.PropertyName + '_' + Obs.AttachedTo.Guid();
    }
  }

};

ko.bindingHandlers.Drag = {
  init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
    var dragElement = $(element);
    var binding = valueAccessor();

    var dragOptions = {
      helper: function (event) {
        //var DragText = 
        var div = $("<div class='Dragging' style='z-index: 9999'>" + binding.DragText + "</div>");
        div[0]._OrigText = binding.DragText;
        return div;
      },
      scope: binding.Scope,
      revert: true,
      revertDuration: 0,
      start: function () {
        if (binding.OnStart) binding.OnStart(element);
        _dragged = viewModel;
      },
      cursor: 'move',
      cursorAt: { left: -15 }
    };
    dragElement.draggable(dragOptions);//.disableSelection();
  }
};

ko.bindingHandlers.Drop = {
  init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
    var dropElement = $(element);
    var binding = valueAccessor();
    var dropOptions = {
      drop: function (event, ui) {
        setTimeout(function () {
          binding.OnDrop(_dragged, viewModel, event);
        }, 0);
      },
      over: function (event, ui) {
        if (binding.OnHover) binding.OnHover(event, ui, _dragged, viewModel);
      },
      out: function (event, ui) {
        ui.helper.html(ui.helper[0]._OrigText);
      },
      scope: binding.Scope,
    };
    dropElement.droppable(dropOptions);
  }
};

ko.bindingHandlers.Captcha = {
  init: function (element, valueAccessor, allBindingsAccessor, data) {

    var Button = $(element).find('button');
    var Img = $(element).find('img');

    function GetNewImage() {

      Singular.CallServerMethod('Singular.Captcha, Singular.Silverlight', 'NewCaptcha', {}, function (result) {
        Img[0].src = 'data:image/jpg;base64, ' + result.Data.Image;
        data.CaptchaSecret(result.Data.Secret);
        data.CaptchaText('');
      });

    }

    Button.on('click', GetNewImage);
    setTimeout(GetNewImage, 0);
  }
};

//#region  Date Range
ko.bindingHandlers.DateRangeValue = {
  init: function (element, valueAccessor, allBindingsAccessor, arg3, arg4) {

    var binding = valueAccessor();
    var DateFormat = binding.Format ? binding.Format : Singular.DateFormat;
    Singular.SetSValue(element, 'Format', DateFormat);

    if (element.nodeName.toLowerCase() == 'input' || element.nodeName.toLowerCase() == 'div') {

      //SetupSValue(element, function () { return binding.StartDate }, allBindingsAccessor);

      if (Singular.DateEditorOnReadOnly || !$(element).is('[readonly]')) {

        if (binding.Type != 'Time') {

          if (!binding.Simple) {

            //Convert the .Net format to JQuery format
            var jqFmt = DateFormat_Net_to_JS(DateFormat);

            //if this needs a button to show the date picker.
            if (Singular.DateButton) {
              $(element).wrap('<div class="Trigger DateTrigger"></div>');
              var Span = document.createElement('span');
              $(Span).insertAfter(element);
              $(Span).click(function () {
                if (!element.disabled) {
                  $(element).daterangepicker('show');
                }
              });
              $(element).width($(element).outerWidth() - $(Span).outerWidth(true));
            }

            //setup the options
            var dpOptions = {
              initialText: binding.InitialText,
              applyOnMenuSelect: binding.ApplyOnMenuSelect,
              bootstrap: binding.Bootstrap,
              bootstrapButtonCss: binding.BootstrapButtonCss,
              iconName: binding.IconName,
              datepickerOptions: {
                minDate: null, maxDate: null
              }
            };
            dpOptions.onChange = function (newValue) {
              //When the user selects a date with the date picker.
              var range = $(element).daterangepicker('getRange');
              ko.bindingHandlers.DateRangeValue.SetObs(binding, range);
              if (binding.AfterRangeChangedJSFunction) {
                binding.AfterRangeChangedJSFunction(range);
              }
            }
            dpOptions.onClose = binding.OnCloseJSFunction();

            //select text on focus
            Singular.OnFocus(element, function () {
              element.select();
            });
            //Create the jquery datepicker.
            $(element).daterangepicker(dpOptions);
            $(element).daterangepicker('setRange', { start: new Date(binding.StartDate()), end: new Date(binding.EndDate()) });
          }

        }
      }
    }
  },
  SetObs: function (binding, NewValue) {
    //Set the date / time while keeping the other half of the date / time component.
    var OldStart = binding.StartDate.peek();
    var OldEnd = binding.EndDate.peek();
    var IsTime = binding.Type == 'Time';

    var NewStart = new Date(NewValue.start);
    var NewEnd = new Date(NewValue.end);

    NewStart = NewStart instanceof Date ? (IsTime ? NewStart.format('HH:mm:ss') : NewStart.format('dd MMM yyyy')) : NewStart;
    NewEnd = NewEnd instanceof Date ? (IsTime ? NewEnd.format('HH:mm:ss') : NewEnd.format('dd MMM yyyy')) : NewEnd;
    if (NewStart) {
      NewStart = GetUpdatedDateTimeProperty(NewStart, OldStart, binding.Type);
    }
    if (NewEnd) {
      NewEnd = GetUpdatedDateTimeProperty(NewEnd, OldEnd, binding.Type);
    }
    //Only allow in range of min and max dates.
    if (NewStart) {
      NewStart = GetDateInRange(NewStart, binding.MaxDate ? ko.utils.unwrapObservable(binding.MaxDate) : null, binding.MinDate ? ko.utils.unwrapObservable(binding.MinDate) : null);
    }
    if (NewEnd) {
      NewEnd = GetDateInRange(NewEnd, binding.MaxDate ? ko.utils.unwrapObservable(binding.MaxDate) : null, binding.MinDate ? ko.utils.unwrapObservable(binding.MinDate) : null);
    }
    binding.StartDate(new Date(NewStart));
    binding.EndDate(new Date(NewEnd));

  },
  update: function (element, valueAccessor, allBindingsAccessor) {

    var ObsStart = ko.utils.unwrapObservable(valueAccessor().StartDate);
    var ObsEnd = ko.utils.unwrapObservable(valueAccessor().EndDate);
    if (ObsStart != null) {
      if (!(ObsStart instanceof Date)) {
        ObsStart = new Date(ObsStart);
      }
    }
    if (ObsEnd != null) {
      if (!(ObsEnd instanceof Date)) {
        ObsEnd = new Date(ObsEnd);
      }
    }

  }
};
//#endregion

//#region Slider
ko.bindingHandlers.Slider = {
  init: function (element, valueAccessor, allBindingsAccessor) {
    var options = allBindingsAccessor().sliderOptions || {};
    $(element).slider(options);
    ko.utils.registerEventHandler(element, "slidechange", function (event, ui) {
      var observable = valueAccessor();
      observable(ui.value);
    });
    ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
      $(element).slider("destroy");
    });
    ko.utils.registerEventHandler(element, "slide", function (event, ui) {
      var observable = valueAccessor();
      observable(ui.value);
    });
  },
  update: function (element, valueAccessor) {
    var value = ko.utils.unwrapObservable(valueAccessor());
    if (isNaN(value)) value = 0;
    $(element).slider("value", value);
  }
};
//#endregion

//#region placeolder
ko.bindingHandlers.Placeholder = {
  update: function (element, valueAccessor) {
    var Value = ko.utils.unwrapObservable(valueAccessor()) || '';

    $(element).attr('placeholder', Value);
  }
}
//#endregion

//#region DateAndTimeEditor
ko.bindingHandlers.DateAndTimeEditor = {
  init: function (element, valueAccessor, allBindingsAccessor) {
    //setup the control
    var binding = valueAccessor();
    var prop = ko.utils.unwrapObservable(binding);
    var currentVal = prop.Value.peek();

    var options = {
      displayFormat: (binding.dateAndTimeOptions.displayFormat),
      format: 'ddd DD MMM yyyy HH:mm',
      defaultDate: ((currentVal === undefined || currentVal === null) ? "" : currentVal),
      allowInputToggle: (binding.dateAndTimeOptions.allowInputToggle === "true"),
      viewMode: (binding.dateAndTimeOptions.viewMode),
      showClear: (binding.dateAndTimeOptions.showClear === "true"),
      calendarWeeks: (binding.dateAndTimeOptions.calendarWeeks === "true"),
      keepOpen: (binding.dateAndTimeOptions.keepOpen === "true"),
      sideBySide: (binding.dateAndTimeOptions.sideBySide === "true"),
      useCurrent: false,
      inline: (binding.dateAndTimeOptions.inline === "true")
    };
    //binding.dateAndTimeOptions.displayFormat = binding.dateAndTimeOptions.format
    element.actualValue = currentVal;
    $(element).datetimepicker(options);

    Singular.AddElementToProperty(valueAccessor().Value, element)

    SetupSValue(element, function () { return binding.Value }, allBindingsAccessor);

    //$(element).on('focus', (function () {
    //  this.select()
    //}))

    //handle change of datetimepicker
    $(element).on('dp.change', (function (obj) {
      this.actualValue = obj.date.toDate()
      valueAccessor().Value(this.actualValue)
      if (this.actualValue === undefined) {
        this.title = ""
        this.value = ""
        ko.bindingHandlers.DateAndTimeEditor.SetObs(valueAccessor(), null)
      } else {
        this.title = moment(this.actualValue).format(prop.dateAndTimeOptions.displayFormat);
        this.value = moment(this.actualValue).format(prop.dateAndTimeOptions.displayFormat);
        ko.bindingHandlers.DateAndTimeEditor.SetObs(valueAccessor, this.actualValue)
      }
    }))

    //handle destroy of control
    ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
      $(element).datetimepicker("destroy");
    });

  },
  SetObs: function (binding, NewValue) {
    //binding.Value(NewValue)
  },
  update: function (element, valueAccessor) {
    valueAccessor()
    var prop = ko.utils.unwrapObservable(valueAccessor());
    var updatedVal = prop.Value();
    if (updatedVal === undefined || updatedVal === null) {
      element.actualValue = null;
      element.title = "";
      element.value = "";
    }
    else {
      if (typeof updatedVal === "string") {
        updatedVal = new Date(updatedVal)
      }
      element.actualValue = updatedVal;
      element.title = moment(updatedVal).format(prop.dateAndTimeOptions.displayFormat);
      element.value = moment(updatedVal).format(prop.dateAndTimeOptions.displayFormat);
    }
  }
};
//#endregion

//#region Select2
ko.bindingHandlers.select2 = {
  init: function (element, valueAccessor, allBindingsAccessor) {
    //initial setup

    //setup the control
    var binding = valueAccessor();
    //console.log('init select2');

    //setup options with datasource
    var dataSource = [];
    if (binding.select2Options.source.indexOf("ClientData") >= 0) {
      var listName = binding.select2Options.source.split(".")[1];
      var tempDataSource = window.ClientData[listName];

      for (i = 0; i < tempDataSource.length - 1; i++) {
        var item = tempDataSource[i];
        item.IsSelected = true;
        //defaultSelection.push({ id: item[binding.select2Options.valueMember], text: item[binding.select2Options.displayMember] });
        dataSource.push({ id: item[binding.select2Options.valueMember], text: item[binding.select2Options.displayMember] });
      }

    } else if (binding.select2Options.filterMethod) {
      dataSource = [];
      dataSource = ko.utils.unwrapObservable(binding.select2Options.filterMethod);
    }

    var select2Options = {
      placeholder: binding.select2Options.placeholder,
      tags: (binding.select2Options.tags === "true"),
      multiple: (binding.select2Options.multiple === "true"),
      width: (binding.select2Options.width),
      data: dataSource,
      allowClear: (binding.select2Options.allowClear === "true")
    };

    //setup the control
    var theControl = $(element).select2(select2Options);

    //setup default value
    var initialValue = [];
    if (binding.select2Options.initialValueMethod) {
      initialValue = ko.utils.unwrapObservable(binding.select2Options.initialValueMethod);
    }
    this.defaultSet = true;
    valueAccessor().Value(initialValue);
    theControl.val(initialValue).trigger('change.select2');

    //singular stuff
    //Singular.AddElementToProperty(valueAccessor().Value, element)
    //SetupSValue(element, function () { return binding.Value }, allBindingsAccessor);

    //handle change of selections
    $(element).on('change.select2', (function (obj) {
      if (!this.defaultSet && typeof this.value === "string") {
        this.defaultSet = false;
        var actualValue = [];
        if (this.value.length > 0) {
          var allValues = this.value.split(",");
          allValues.forEach(function (val) {
            var converted = parseInt(val);
            if (!isNaN(converted)) {
              actualValue.push(converted);
            };
          });
        };
        valueAccessor().Value(actualValue);
      }
    }));

    ////handle destroy of control
    //ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
    //  $(element).datetimepicker("destroy");
    //})
  },
  update: function (element, businessObject) {
    //, valueAccessor, allBindings, viewModel
    if (!this.defaultSet) {
      var binding = valueAccessor();
      if (binding.select2Options.onBindingUpdatedMethod) {
        ko.utils.unwrapObservable(binding.select2Options.onBindingUpdatedMethod).call(this, element, businessObject);
      }
    } else {
      this.defaultSet = false;
    }
  }
};
//#endregion

ko.bindingHandlers.tinyMCE = {
  init: function (element, valueAccessor) {

    element.id = GetControlID(Singular.UnwrapFunction(valueAccessor));

    var prop = valueAccessor(),
      Settings = {
        selector: '#' + element.id,
        setup: function (ed) {
          ed.on('change', function (e) {
            prop(ed.getContent()); //set property value on editor change.
          });
        }
      };
    SetupTinyMCE(Settings);

    //set elements value.
    $(element).html(prop());

    //setup tinymce on element.
    tinyMCE.init(Settings);

    ko.utils['domNodeDisposal'].addDisposeCallback(element, function () { //garbage collection.
      tinyMCE.remove('#' + element.id);
    });

  },
  update: function (element, valueAccessor) {
    //update tinymce editor on property change.
    var tinymce = tinyMCE.get(element.id),
      value = valueAccessor()();

    if (tinymce && tinymce.getContent() !== value) {
      tinymce.setContent(value);
      tinymce.execCommand('keyup');
    }
  }
}

ko.bindingHandlers.tabs = {
  init: function (element, valueAccessor) {
    var jElem = $(element);
    if (!jElem.data().tabs) {
      jElem.tabs({
        beforeActivate: function (event, ui) {
          valueAccessor()(ui.newTab.index());
        }
      });
      valueAccessor().GetKey = function () {
        return $(jElem.find('> ul > li')[ko.utils.unwrapObservable(valueAccessor())]).attr('data-tab-key');
      }
    }
  }, update: function (element, valueAccessor) {
    var Index = ko.utils.unwrapObservable(valueAccessor());
    $(element).tabs("option", "active", Index);
  }
}

ko.bindingHandlers.readonly = {
  update: function (element, valueAccessor) {
    if (valueAccessor()) {
      $(element).attr("readonly", "readonly");
    } else {
      $(element).removeAttr("readonly");
    }
  }
};