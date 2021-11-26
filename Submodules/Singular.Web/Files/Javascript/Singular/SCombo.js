ko.bindingHandlers.SCombo = {
  init: function (element, valueAccessor, allBindingsAccessor) {
    //console.log(element, valueAccessor, allBindingsAccessor)
    var Combo = new Singular.Combobox(element, valueAccessor());
    Combo.CreateButtons();

    SetupSValue(element, function () { return valueAccessor().Value }, allBindingsAccessor);

  },
  update: function (element, valueAccessor, allBindingsAccessor) {
    var Combo = Singular.GetSValue(element, 'Combo');
    Combo.UpdateBindings(valueAccessor());
    Combo.SetDisplayText(true);
  }
};

Singular.ComboButton = false;

Singular.Combobox = function (TextInput, KOBindings) {
  var self = this;

  self.TextInput = TextInput;
  Singular.SetSValue(TextInput, 'Combo', self);

  //Ensure the drop down part is created.
  Singular.Combobox.CreateDropDownDiv();

  var Bindings;
  var JInput = $(TextInput);
  var DropDown = Singular.Combobox.DropDownDiv;
  var JDropDown = $(DropDown.Div);
  var ShouldFilter = false,
    FilteredItems = [],
    SelectedItem,
    SelectedIndex,
    SelectedIds,
    OldSelectedID,
    SortCol = '',
    SortAsc = true,
    Columns,
    InputHeight,
    InputWidth,
    SelectorWidth = 0,
    ExtraColumnInfo = false,
    SourceData,
    AjaxFilterValue,
    AddNewItemRow,
    NoneItemRow,
    TypeToSearchRow,
    Caption,
    DropDownType;

  //Input On Focus
  var ClickFocused = false,
    MouseDown = false;
  TextInput.onmousedown = function () {
    MouseDown = true;
    if (self.ComboHasFocus()) {
      OnFocus();
    }
    //else {
    //      TextInput.focus();
    //    }
  }

  //#region Setup 

  var FindButton;
  var ShowFindScreen = function () {

    if (!TextInput.disabled) {
      var SelectedValue = ko.utils.unwrapObservable(Bindings.Value);

      var OpenDialog = function () {
        Singular.ShowFindDialog(ko.dataFor(self.TextInput), KOBindings.FindCriteria, null, false, KOBindings.PreFind, function (ID, Obj, dlg) {

          //is the display value for the id in a lookup list, or inline with the current record.
          if (!Bindings.MultiSelect) {
            if (Bindings.LookupMember) {
              SourceData = [Obj];
            } else {
              //lookup:
              var Item = SourceData.Find(Bindings.ValueMember, ID);
              if (!Item) {
                SourceData.push(Obj);
              }
            }
          }
          self.SelectItem({ __ID: ID, __Object: Obj });
          dlg.dialog('close');

        }, Bindings.MultiSelect, Bindings.PreFetchJS);
      }

      if (!SelectedValue || SelectedValue.length == 0) {
        OpenDialog();
      } else {
        Singular.ContextMenu.Show([{ ID: 1, Text: 'Find a different record' }, { ID: 2, Text: 'Clear current item' }], FindButton.offset().left, FindButton.offset().top, function (Item) {
          if (Item.ID == 1) {
            OpenDialog();
          } else {
            self.SelectItem({ __ID: null });
          }
          Singular.ContextMenu.Hide();
        });
      }
    }

  }

  self.CreateButtons = function () {

    //if there is a combo icon next to the combo.
    if (Singular.ComboButton || (KOBindings.FindCriteria && Bindings.FindButton !== false)) {

      //setTimeout(function () {
      var Width, Parent = JInput.parent()[0];
      var PercentWidth;

      if (Singular.UIStyle && Singular.UIStyle == 2) {
        if (JInput[0].className.indexOf("form-control") >= 0) {
          Width = "100%"
        } else {
          //do nothing
        }
      } else {
        if (TextInput.style.width == '100%' && Parent && Parent.style.width && Parent.style.width.indexOf('%') < 0) {
          //100% text box in a parent with fixed width
          Width = parseInt(Parent.style.width);
        } else if (TextInput.style.width.indexOf('%') > 0) {
          //x% text box
          Width = JInput.outerWidth();
          PercentWidth = TextInput.style.width;
        } else {
          Width = parseInt(TextInput.offsetWidth == 0 ? JInput.width() : TextInput.offsetWidth);
        }
      }

      JInput.wrap('<div class="Trigger ComboTrigger" ' + (GetIEVersion() > 9 ? 'style="display:inline-flex"' : '') + '></div>');
      var btnDef = '<button type="button" tabindex="-1"><span></span></button>';

      var BtnWidth = 0;

      if (KOBindings.FindCriteria) {
        var fb = FindButton = $(btnDef);
        fb.addClass('Find');
        fb.insertAfter(self.TextInput);
        fb.click(ShowFindScreen);
        BtnWidth += Math.max(fb.outerWidth(true), 20); //if the input is hidden, e.g. in a hidden tab, then none of the dimensions are correct.
      }
      if (Singular.ComboButton) {
        var db = $(btnDef);
        db.addClass('Cbo');
        db.insertAfter(TextInput);
        db.click(function () { self.TextInput.focus(); });
        if (KOBindings.FindCriteria) {
          db.css('border-radius', 0);
        }
        BtnWidth += Math.max(db.outerWidth(true), 20);

      }

      self.SetSelectorWidth(BtnWidth);
      if (GetIEVersion() > 8 && PercentWidth) {
        JInput.css('width', 'calc(100% - ' + BtnWidth + 'px)');
        JInput.parent().css('width', PercentWidth);
      } else {
        JInput.css('width', Width - BtnWidth);
      }

      //}, 0);
    }
  }

  //#endregion

  var OnFocus = function () {

    ////fix for weird issue when using stacked modals - inputs will not be focused when clicked in, causing really long loop
    //if (!self.ComboHasFocus()) {
    //  TextInput.focus();
    //};

    //if this is in an opening dialog, dont auto open the combo.
    if (JInput.parents('[data-dlg]').attr('data-dlg') == 'opening') {
      ClearPrompt();
      return;
    }

    //Check if the drop down div is visible, if it is, then the user has clicked from the drop down into the text input.
    if (JInput.attr('readonly') && DropDownType != 5) {
      if (KOBindings.FindCriteria) ShowFindScreen();
    } else {
      if (DropDown.Div.style.display == 'none' || DropDown.CurrentCombo != self) {

        ClickFocused = MouseDown;
        MouseDown = false;

        self.FilterSource();

        AfterFocus();

        ClearPrompt();
        ShowDropDown(true)

      }
    }
  }
  TextInput.onfocus = OnFocus;

  var AfterFocus = function () {
    DropDown.CurrentCombo = self;
    DropDown.AllowMulti = Bindings.MultiSelect;

    InputHeight = JInput.outerHeight();

    var bcr = TextInput.getBoundingClientRect();
    if (bcr && bcr.width) {
      //more accurate than below.
      InputWidth = bcr.width;
    } else {
      InputWidth = JInput.outerWidth();
    }


  }

  //Input On De-Focus
  TextInput.onblur = function () {

    //If the user clicked off the input (and not onto the drop down)
    if (!self.ComboHasFocus() && DropDown.CurrentCombo == self) {
      self.OnLostFocus();
    }
  };

  //Input key down
  self.OnKeyDown = function (e) {

    if (JInput.attr('readonly')) return;

    if (!$(DropDown.Div).is(":visible")) {

      BuildDropDownItems();
      AfterFocus();
      CalcLayout(true);
      JDropDown.show();//Show the drop down if hidden.
      HighlightSelected();

    }
    if (e.keyCode == 38 || e.keyCode == 40) { //Up or Down

      var MinIndex = (AddNewItemRow ? -1 : 0);

      if (SelectedIndex == undefined) {
        SelectedIndex = e.keyCode == 38 ? FilteredItems.length - 1 : MinIndex;
      } else {
        if (e.keyCode == 38) { //up

          SelectedIndex = SelectedIndex == MinIndex ? FilteredItems.length - 1 : SelectedIndex - 1;
        }
        if (e.keyCode == 40) { //down
          SelectedIndex = SelectedIndex == FilteredItems.length - 1 ? MinIndex : SelectedIndex + 1;
        }
      }
      HighlightSelected();
      e.preventDefault();

    } else if (e.keyCode == 9 || e.keyCode == 13) { //TAB or ENTER

      if (TextInput.value == '' && Bindings.Value()) {
        self.SelectItem({ __ID: null });

      } else if (SelectedItem) {
        //if the user selected an item with the arrow keys, then set the data value to this items id.
        self.SelectItem(SelectedItem[0]);

      } else {
        //if the user didnt select anything, then reset the display text.
        if (Bindings.AllowNotInList) CreateItemFromTextValue();
        self.SetDisplayText()
      }

      JDropDown.hide(); //.fadeOut(150);

      if (e.keyCode == 13) { //prevent enter from submitting the page.
        e.preventDefault();
      }

    } else if (e.keyCode == 27) { //Escape

      self.SetDisplayText();
      ClearPrompt();
      if (TextInput.value == '') JDropDown.hide();

    } else if (e.keyCode == 37 || e.keyCode == 39) {
      //left and right
    } else if (e.keyCode == 16 || e.keyCode == 17) {
      //shift, ctrl
    } else {
      //normal key, filter the list.

      if (Bindings.SearchField) {

        DelayedFetch();

      } else {
        ShouldFilter = true;

        setTimeout(function () {
          BuildDropDownItems();
          CalcLayout();
        }, 0);
      }

    }
  }
  JInput.keydown(self.OnKeyDown);

  var KeyDelay,
    IsSearching = false;
  //Fetch items asyncronously filtered on the text in the textbox.
  var DelayedFetch = function () {
    if (KeyDelay) clearTimeout(KeyDelay);
    KeyDelay = setTimeout(FetchItems, 500);
  }
  var FetchItems = function (SearchValue) {

    IsSearching = true;
    //if (Singular.UIStyle == 2) {
    //  //JInput.attr('readonly', 'readonly');
    //  //JInput.attr('placeholder', 'searching');
    //  JInput[0].style.opacity = '0.5';
    //  JDropDown.hide();//hide the dropdown
    //}
    BuildDropDownItems();
    var Args = {};
    Args[Bindings.SearchField] = SearchValue != null ? SearchValue : TextInput.value;

    var pfArgs = { Data: Args, Object: ko.dataFor(TextInput) };
    if (Bindings.PreFetchJS) Bindings.PreFetchJS(pfArgs);

    var FetchType = KOBindings.FetchType;
    if (!FetchType) FetchType = KOBindings.FindCriteria.ChildType.Type;

    Singular.GetDataStateless(FetchType, pfArgs.Data, function (result) {
      if (result.Success) {
        ObjectSchema.Apply(result.TypeName, result.Data);
        SourceData = result.Data;

        if (SourceData.Properties) {
          if (pfArgs.DataTransform) pfArgs.DataTransform(SourceData);
          //Hide columns
          Columns.Iterate(function (Column) {
            var ci = SourceData.Properties.Find('Name', Column[0]);
            if (ci && ci.Hidden !== undefined) Column.Hidden = ci.Hidden;
          });
        }

        IsSearching = false;
        ShouldFilter = true;
        BuildDropDownItems();

        CalcLayout(true);
        HighlightSelected();
        if (Bindings.AfterFetchJS) Bindings.AfterFetchJS({ Data: SourceData, Property: Bindings.Value, Object: ko.dataFor(TextInput) });
      } else {
        alert('Error on drop down: ' + result.ErrorText);
      }
    });
  }

  self.GetSelectedInfo = function (external) {
    var SelectedValue = ko.utils.unwrapObservable(Bindings.Value);

    if (Bindings.MultiSelect) {
      if (SelectedValue.length == 0) {
        return null;
      } else if (SelectedValue.length == 1) {
        SelectedValue = SelectedValue[0];
      } else {
        var Item = {};
        Item[Bindings.Display] = SelectedValue.length + ' Items selected';
        return { Item: Item };
      }
    }

    if ((SelectedValue != null && SelectedValue !== '') || (Bindings.LookupMember)) {

      var Source, Item, Parent;

      Source = SourceData ? SourceData : [];

      if (Bindings.ChildList) {
        for (var i = 0; i < Source.length && !Item; i++) {
          for (var j = 0; j < Source[i][Bindings.ChildList].length; j++) {
            if (Source[i][Bindings.ChildList][j][Bindings.ValueMember] == SelectedValue) {
              Item = Source[i][Bindings.ChildList][j];
              Parent = Source[i];
              break;
            }
          }
        }
      } else {
        Item = Source.Find(Bindings.ValueMember, SelectedValue);
      }
      if (Bindings.LookupMember && !Item) {
        var LookupText = ko.utils.unwrapObservable(Bindings.Value.AttachedTo[Bindings.LookupMember]);

        if (typeof Bindings.Display == 'function') {
          return { LookupText: LookupText };
        } else {
          Item = {};
          Item[Bindings.Display] = (LookupText == undefined ? '' : LookupText);
        }
      }

      if (Item) return { Item: Item, Parent: Parent };

    }
    return null;
  }
  self.SetSelectorWidth = function (NewWidth) {
    SelectorWidth = NewWidth;
  }
  self.SetDisplayText = function (external) {
    if (Bindings.Value.PropertyName == Bindings.LookupMember && !external) return;

    var SelectedInfo = self.GetSelectedInfo(external);
    if (SelectedInfo) {

      TextInput.value = self.GetDisplayText(SelectedInfo);

      Bindings.Value.DropDownText = TextInput.value;

      JInput.removeClass('Blank');

    } else {

      //No selected item
      TextInput.value = Caption;
      Bindings.Value.DropDownText = '';

      JInput.addClass('Blank');

    }
  };

  function CreateItemFromTextValue() {

    if (TextInput.value && TextInput.value != Caption) {

      var Exiting = SourceData.FindText(Bindings.ValueMember, TextInput.value);

      if (Exiting) {
        Bindings.Value(Exiting[Bindings.ValueMember]);
      } else {
        //Add this to the datasource
        var NewItem = {};
        NewItem[Bindings.Display] = TextInput.value;
        if (Bindings.ListFilter) {
          NewItem[Bindings.ListFilter] = ko.utils.unwrapObservable(Bindings.Filter);
        }
        Bindings.Source.push(NewItem);
        Bindings.Value(TextInput.value);
        if (Bindings.Source.Refresh) Bindings.Source.Refresh();
      }
    }

  }

  self.GetDisplayText = function (SelectedInfo) {
    if (SelectedInfo.Parent) {
      //Child List
      return SelectedInfo.Item[Bindings.Display] + ' (' + SelectedInfo.Parent[Bindings.GroupDisplay] + ')';
    } else {
      //No Child list
      if (SelectedInfo.LookupText !== undefined) {
        return SelectedInfo.LookupText;
      } else if (typeof Bindings.Display == 'function') {
        return Bindings.Display(SelectedInfo.Item);
      } else {
        if (SelectedInfo.Item) {
          return ko.utils.unwrapObservable(SelectedInfo.Item[Bindings.Display]);
        } else { return "" }
      }
    }
  }

  function ClearPrompt() {
    if (self.GetSelectedInfo() == null) {
      TextInput.value = '';
      JInput.removeClass('Blank');
    } else if (DropDownType != 5) {
      setTimeout(function () { JInput.select() }, 0);//Select all text;
    }
  };

  function ShowDropDown(ShowDiv) {
    ShouldFilter = false;

    if (ShowDiv && Bindings.SearchField && Bindings.PreFind) {
      var Crit = Bindings.FindCriteria ? Bindings.FindCriteria() : null;
      var Args = { Control: TextInput, Data: ko.dataFor(TextInput), AutoPopulate: false, SearchValue: null, Items: SourceData, Object: ko.dataFor(TextInput), Criteria: Crit };
      Bindings.PreFind(Args);
      if (Args.AutoPopulate) FetchItems(Args.SearchValue);
    }
    BuildDropDownItems();

    //if (Singular.UIStyle && Singular.UIStyle == 2) {
    //  //bootstrap specific
    //  JDropDown[0].classList.add('animated')
    //  JDropDown[0].classList.add('fadeIn')
    //  JDropDown[0].classList.add('faster')
    //  CalcLayout(true)
    //}
    //else {
    //Position the drop down
    CalcLayout(true);
    if (ShowDiv) JDropDown.show();
    //}

  }

  function CalcLayout(CheckPosition) {

    //Decide whether to put it above or below
    JDropDown.css({ height: 'auto', display: 'block', width: '' });
    var ActualHeight = JDropDown.outerHeight();
    var MaxHeight = parseInt(JDropDown.css('max-height'));
    //Get the amount of space above, and below the editor.
    var SpaceAbove = JInput.offset().top - $(window).scrollTop();
    var SpaceBelow = $(window).height() - (JInput.offset().top + InputHeight) + $(window).scrollTop();
    SpaceAbove = SpaceAbove > MaxHeight ? MaxHeight : SpaceAbove;
    SpaceBelow = SpaceBelow > MaxHeight ? MaxHeight : SpaceBelow;
    var Width = JDropDown.width();

    if (CheckPosition) {
      DropDown.IsAbove = (SpaceAbove > SpaceBelow && SpaceBelow < ActualHeight);
    }

    var Top, Height;
    if (DropDown.IsAbove) {
      Height = SpaceAbove < ActualHeight ? SpaceAbove : 'auto';
      Top = JInput.offset().top - (Height == 'auto' ? ActualHeight : Height);
    } else {
      Height = SpaceBelow < ActualHeight ? SpaceBelow : 'auto';
      Top = JInput.offset().top + InputHeight;
    }
    JDropDown.css({ 'height': Height, top: Top, 'overflow-y': 'auto', minWidth: InputWidth + SelectorWidth });
    //JDropDown.css({ left: JInput.offset().left });
    var dropDownLeft = document.body.clientWidth - JDropDown.width();
    JDropDown.css({ left: dropDownLeft < JInput.offset().left ? dropDownLeft - 50 : JInput.offset().left });

    var customDropDownClass = (KOBindings.DropDownCssClass ? KOBindings.DropDownCssClass : "")
    if (customDropDownClass != "") {
      JDropDown.addClass(customDropDownClass);
    }

    setTimeout(HighlightSelected, 0);

  };

  self.UpdateBindings = function (KOBindings) {
    if (KOBindings) {
      Bindings = KOBindings;
    }
    if (Bindings.Columns) {
      ExtraColumnInfo = Bindings.Columns.length > 1;
      Columns = Bindings.Columns;
    } else {
      ExtraColumnInfo = false;
      Columns = [[Bindings.Display, Bindings.Display, 's']];
    }
    if (Bindings.LookupMember === '') {
      Bindings.LookupMember = '__Lookup_' + Bindings.ValueMember;
      KOBindings.Value.AttachedTo[Bindings.LookupMember] = ko.observable('');
    }
    if (Bindings.LookupMember) ko.utils.unwrapObservable(KOBindings.Value.AttachedTo[Bindings.LookupMember]);

    if (Bindings.Source) {
      if (Bindings.Source.Version) Bindings.Source.Version();
      SourceData = ko.utils.unwrapObservable(Bindings.Source);
      if (Bindings.ListFilter) {
        var FilterValue = ko.utils.unwrapObservable(Bindings.Filter);
        SourceData = Singular.FilterList(SourceData, Bindings.ListFilter, FilterValue);
      }
    } else if (!Bindings.LookupMember) {
      //Handle ajax fetch
      if (Bindings.Filter && Bindings.Filter()) {  //Changed by Emile. Must Ask Marlborough to check
        if (!AjaxFilterValue || Bindings.Filter() != AjaxFilterValue) {
          AjaxFilterValue = Bindings.Filter();

          //Loading image
          var img;
          if (JInput.offset().top != 0) {
            img = document.createElement('img');
            img.src = Singular.RootPath + '/Singular/Images/LoadingSmall.gif';
            $(img).css({ position: 'fixed', top: JInput.offset().top + 2, left: JInput.offset().left + 4 });
            JInput.parent()[0].appendChild(img);
          }
          //Fetch data
          SourceData = [];
          var Args = { Context: Bindings.AjaxName };
          Args[Bindings.FilterName] = AjaxFilterValue;
          if (Bindings.PreFetchJS) { Bindings.PreFetchJS(Args, ko.dataFor(TextInput)) }
          Singular.AJAXCall('GetData', Args, function (Data) {

            SourceData = JSON.parse(Data);
            BuildDropDownItems();
            HighlightSelected();
            if (img) {
              JInput.parent()[0].removeChild(img);
            }
            self.SetDisplayText();

            if (Bindings.AfterFetchJS) Bindings.AfterFetchJS({ Data: SourceData, Property: Bindings.Value, Object: ko.dataFor(TextInput) });

          });
        }

      } else {
        AjaxFilterValue = null;
        SourceData = [];
      }
    }

    if (Bindings.AOTF) {
      Bindings.AOTF.FromControl = TextInput;
      Bindings.AOTF.AfterAdd = function (SavedObject) {
        if (Bindings.AOTF.AfterAddFunction) {
          Bindings.AOTF.AfterAddFunction(SavedObject);
          self.UpdateBindings();
        } else {
          Bindings.Source.push(SavedObject);
        }
        self.SelectItem({ __ID: SavedObject[Bindings.ValueMember], __Object: SavedObject });
      }
    }

    if (Bindings.Caption) {
      Caption = ko.utils.unwrapObservable(Bindings.Caption);
    } else {
      Caption = LocalStrings['Combo_Choose'];
    }

    if (Bindings.MultiSelect) {
      SelectedIds = Bindings.Value.peek();
    }

    DropDownType = Bindings.DDType;
  };

  self.FilterSource = function () {
    if (Bindings.Unique) {
      SourceData = Singular.FilterUnique(SourceData, Bindings.ValueMember, Bindings.Value);
    }
    if (Bindings.OldMembers) {
      SourceData = Singular.FilterOld(SourceData, Bindings.Value(), Bindings.ValueMember, Bindings.ChildList, Bindings.OldMembers[0], Bindings.OldMembers.length == 2 ? Bindings.OldMembers[1] : null);
    }
  };

  self.SelectItem = function (Item) {
    if (Item) {
      if (Item.AOTF) {
        Singular.AddOnTheFly(Bindings.AOTF);
        JDropDown.hide();
      } else {
        var Val = Item.__ID;
        if (Bindings.MultiSelect) {

          if (!Val) {
            Bindings.Value([]);
          } else if (!(Val instanceof Array)) {
            if ($(Item).hasClass('Selected')) {
              Bindings.Value.push(Val);
            } else {
              Bindings.Value.remove(Val);
            }
          } else {
            Bindings.Value(Val);
          }

        } else {
          if (Bindings.LookupMember) Bindings.Value.AttachedTo[Bindings.LookupMember](self.GetDisplayText({ Item: Item.__Object }))
          Bindings.Value(Val);
          if (Bindings.OnItemSelect) Bindings.OnItemSelect(Item.__Object, ko.dataFor(TextInput));
        }

      }
    }

    if (SelectedItem) SelectedItem.removeClass('Selected');
    SelectedItem = null;

  };

  self.GetBindings = function () {
    return Bindings;
  }

  self.OnLostFocus = function () {

    //Stop timer that fetches data
    if (KeyDelay) clearTimeout(KeyDelay);

    //Hide the drop down
    if (!Singular.Combobox.DropDownDiv.HasFocus()) {
      JDropDown.hide();
    }

    //Cancel any changes.
    if (TextInput.value == '' && Bindings.Value()) {
      self.SelectItem({ __ID: null });
    } else {
      self.SetDisplayText();
    }

    //clear any custom classes that might have been added
    JDropDown.attr("class", "ComboDropDown");

  }

  self.ComboHasFocus = function () {
    if (document.activeElement) {
      if (document.activeElement == TextInput || Singular.Combobox.DropDownDiv.HasFocus()) {
        return true;
      }
    }
    return false;
  }

  function BuildDropDownItems() {

    var Container = document.createDocumentFragment(),
      tbl, tbody, div;


    if (Bindings.ChildList || !ExtraColumnInfo || Bindings.AOTF || Bindings.SearchField || Bindings.ClearText) {
      //Create a container div
      div = document.createElement('div');
      Container.appendChild(div);
    }
    if (Bindings.MultiSelect) {
      SelectedIds = Bindings.Value.peek();
    }

    if (!Bindings.ChildList && ExtraColumnInfo) {
      //Create a table
      tbl = document.createElement('table');
      if (Singular.UIStyle && Singular.UIStyle == 2) {
        tbl.classList.add("table")
        tbl.classList.add("table-condensed")
      }
      tbl.style.width = '100%';
      var thead = document.createElement('thead');
      tbl.appendChild(thead);
      var thr = document.createElement('tr');
      thead.appendChild(thr);
      for (var i = 0; i < Columns.length; i++) {
        if (!Columns[i].Hidden) {
          var th = Singular.CreateTableHeader(thr, { Display: Columns[i][1] });
          Singular.SetSValue(th, 'ColIndex', i);
          //Handle click on the table header.
          $(th).click(function () {
            var Index = Singular.GetSValue(this, 'ColIndex');
            if (SortCol == Columns[Index][0]) {
              SortAsc = !SortAsc;
            } else {
              SortCol = Columns[Index][0];
              SortAsc = true;
            }
            BuildDropDownItems();
            JInput.focus();

          });
        }
      }
      tbody = document.createElement('tbody');
      tbl.appendChild(tbody);
      Container.appendChild(tbl);

      if (div) div.style.marginBottom = '2px';
    }
    FilteredItems = [];

    if (SortCol) {
      SourceData = Singular.MergeSort(SourceData, function (Item) {
        return ko.utils.peekObservable(Item[SortCol]);
      }, SortAsc ? 1 : 0);
    }

    var InputValueLC = TextInput.value.toLowerCase();

    //Preserve the selected item
    if (!Bindings.AllowNotInList) {
      OldSelectedID = ko.utils.peekObservable(Bindings.Value);
      if (OldSelectedID == null && SelectedItem != null) {
        OldSelectedID = SelectedItem.__ID;
      }
    }
    SelectedIndex = undefined;
    SelectedItem = undefined;

    function CanAddItem(DataItem) {
      for (var i = 0; i < Columns.length; i++) {
        if (!Columns[i].Hidden) {
          var Value = ko.utils.peekObservable(DataItem[Columns[i][0]]);
          if (Value.toString().toLowerCase().indexOf(InputValueLC) >= 0) {
            return true;
          }
          if (InputValueLC == ko.utils.peekObservable(DataItem[Bindings.ValueMember])) {
            return true;
          }
        }
      }
      return false;
    }

    var SuppressNoItemsMessage = false;

    if (Bindings.SearchField) {
      if (!TypeToSearchRow) {
        TypeToSearchRow = document.createElement('div');
        TypeToSearchRow.classList.add('type-to-search-row')
      }
      TypeToSearchRow.innerHTML = IsSearching ? 'Searching...' : 'Start typing to search...';
      SuppressNoItemsMessage = TextInput.value == '' || !SourceData;
      div.appendChild(TypeToSearchRow);
    }

    //Create the '(None)' Item
    if (Bindings.ClearText) {
      if (!NoneItemRow) {
        NoneItemRow = document.createElement('div');
        NoneItemRow.__NoValue = true;
      }
      NoneItemRow.innerHTML = Bindings.ClearText;
      div.appendChild(NoneItemRow);
    }

    //Create the Add New Item
    if (Bindings.AOTF) {
      if (!AddNewItemRow) {
        AddNewItemRow = document.createElement('div');
        AddNewItemRow.AOTF = true;
      }
      AddNewItemRow.innerHTML = '(' + Bindings.AOTF.Prompt + ')';
      SuppressNoItemsMessage = true;
      div.appendChild(AddNewItemRow);
    }

    //Go through each item in the list, and add the items that match the text filter.
    if (SourceData) {
      for (var i = 0; i < SourceData.length; i++) {
        var DataItem = SourceData[i];

        //Add all items if input value is blank
        var AddItem = TextInput.value == '' || !ShouldFilter || Bindings.SearchField;

        if (Bindings.ChildList) {
          var ParentItem = DataItem;
          for (var j = 0; j < DataItem[Bindings.ChildList].length; j++) {
            var ChildItem = DataItem[Bindings.ChildList][j];
            if (AddItem || CanAddItem(ChildItem)) {
              if (ParentItem) {
                CreateGroupItem(div, ParentItem);
                ParentItem = null;
              }
              $(CreateItem(div, ChildItem)).addClass('ComboChild');
            }
          }
        } else {
          if (AddItem || CanAddItem(DataItem)) {
            if (ExtraColumnInfo) {
              CreateRow(tbody, DataItem);
            } else {
              CreateItem(div, DataItem)
            }

          }
        }

      }
    }

    //select only item if there wasnt a previous selection
    if ((OldSelectedID == null || OldSelectedID == '') && FilteredItems.length == 1 && TextInput.value != '' && !Bindings.AllowNotInList) {
      SelectedIndex = 0;
      SelectedItem = $(FilteredItems[0]);
    }

    if (FilteredItems.length == 0 && !SuppressNoItemsMessage) {
      var Blank = document.createElement('div');
      Blank.innerHTML = 'No items found';
      if (Bindings.AllowNotInList && TextInput.value != '') Blank.innerHTML += '<span class="combo-add-prompt">Press tab to create a new item.</span>'
      Blank.className = 'ComboGroup';
      Container = Blank;

      //Container.appendChild(Blank);

    }

    //Container.childNodes[0].style.minWidth = InputWidth + 'px';
    DropDown.Div.innerHTML = '';
    //if (DropDown.Container) {
    //  DropDown.Div.replaceChild(Container, DropDown.Container);
    //} else {
    DropDown.Div.appendChild(Container);
    //}
    //DropDown.Container = DropDown.Div.childNodes[0];

  };

  function CreateRow(Container, DataItem) {

    var row = document.createElement('tr');
    Container.appendChild(row);
    //row.className = 'ComboSelectable';

    for (var i = 0; i < Columns.length; i++) {
      if (!Columns[i].Hidden) {
        var HighlightedText = ko.utils.peekObservable(DataItem[Columns[i][0]]).toString();
        if (TextInput.value != '' && ShouldFilter) {
          var escaped = TextInput.value.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");

          var re = new RegExp('(' + escaped + ')', 'gi');
          HighlightedText = HighlightedText.replace(re, '<strong>$1</strong>');
        }
        if (Columns[i].length > 3) {
          var td = Singular.CreateTableCell(row, HighlightedText, { Type: Columns[i][2], Format: Columns[i][3] });
        } else {
          var td = Singular.CreateTableCell(row, HighlightedText, { Type: Columns[i][2] });
        }
        if (Bindings.OnCellCreate) {
          Bindings.OnCellCreate(td, i, Columns[i], DataItem);
        }
      }
    }

    SetItemID(row, DataItem);
    return row;
  }

  function CreateItem(Container, DataItem) {
    //Create the row.
    var Item = document.createElement('div');
    Item.className = "DropDownItem";
    Container.appendChild(Item);

    if (TextInput.value == '' || !ShouldFilter) {
      Item.innerHTML = ko.utils.peekObservable(DataItem[Columns[0][0]]);
    } else {
      var re = new RegExp('(' + TextInput.value + ')', 'gi');
      Item.innerHTML = ko.utils.peekObservable(DataItem[Columns[0][0]]).replace(re, '<strong>$1</strong>');
    }
    if (Bindings.OnCellCreate) {
      Bindings.OnCellCreate(Item, 0, Columns[0], DataItem);
    }
    SetItemID(Item, DataItem);

    return Item;
  }

  function CreateGroupItem(Container, DataItem) {
    var Item = document.createElement('div');
    Item.className = 'ComboGroup';
    Item.innerHTML = ko.utils.peekObservable(DataItem[Bindings.GroupDisplay]);
    Container.appendChild(Item);
  }

  function SetItemID(Item, DataItem) {
    Item.__ID = ko.utils.peekObservable(DataItem[Bindings.ValueMember]);
    Item.__Object = DataItem;
    if (Item.__ID == OldSelectedID) {
      SelectedIndex = FilteredItems.length;
      SelectedItem = $(Item);
    }
    FilteredItems.push(Item);
    if (SelectedIds && SelectedIds.indexOf(Item.__ID) >= 0) {
      $(Item).addClass('Selected');
    }
  }

  function HighlightSelected() {

    if (SelectedItem) {
      SelectedItem.removeClass('Selected');
      SelectedItem = null;
    }
    if (SelectedIndex < 0) {
      SelectedItem = $(AddNewItemRow);
    }
    if (SelectedIndex >= 0) {
      SelectedItem = $(FilteredItems[SelectedIndex]);
    }
    if (SelectedItem && SelectedItem[0]) {
      SelectedItem.addClass('Selected');

      if (SelectedItem.offset().top < JDropDown.offset().top) {

        DropDown.Div.scrollTop -= (JDropDown.offset().top - SelectedItem.offset().top) + parseInt(JDropDown.css('padding-top'));
      } else if ((SelectedItem.offset().top + SelectedItem.outerHeight()) - JDropDown.offset().top > JDropDown.height()) {

        DropDown.Div.scrollTop += ((SelectedItem.offset().top + SelectedItem.outerHeight()) - JDropDown.offset().top) - JDropDown.height();
      }
    }

  };

  self.UpdateBindings(KOBindings);

};

Singular.Combobox.CreateDropDownDiv = function () {

  if (!Singular.Combobox.DropDownDiv) {

    var InClick = false;
    var DropDiv = document.createElement('div');
    var JDropDiv = $(DropDiv);
    JDropDiv.addClass('ComboDropDown');
    document.body.appendChild(DropDiv);

    //Drop Down Div Lost Focus
    DropDiv.onblur = function () {

      if (!Singular.Combobox.DropDownDiv.CurrentCombo.ComboHasFocus()) {
        Singular.Combobox.DropDownDiv.CurrentCombo.OnLostFocus();
      }
    };

    var SelectItem = function (Item, AlwaysHide) {
      var cbo = Singular.Combobox.DropDownDiv;
      cbo.CurrentCombo.SelectItem(Item);
      cbo.ShoudHide = AlwaysHide || !cbo.AllowMulti;
    }

    JDropDiv.mousedown(function (e) {
      Singular.Combobox.DropDownDiv.ShoudHide = false;
      InClick = true;
      var element = e.target;
      do {
        if (element.__ID != undefined || element.AOTF) {
          $(element).toggleClass('Selected');
          SelectItem(element);
          return;
        } else if (element.__NoValue) {
          SelectItem({ __ID: null }, true);
          return;
        }
        element = element.parentNode;
      } while (element != undefined && !$(element).hasClass('ComboDropDown'));
      $(Singular.Combobox.DropDownDiv.CurrentCombo.TextInput).focus();
    });

    JDropDiv.mouseup(function (e) {
      InClick = false;

      if (Singular.Combobox.DropDownDiv.ShoudHide) { //only hide id the user clicked an item, not if they clicked a header etc.
        var ti = Singular.Combobox.DropDownDiv.CurrentCombo.TextInput;
        var NextControl = Singular.GetNextEditor(ti);
        var Cbo;
        if (NextControl) Cbo = Singular.GetSValue(NextControl, 'Combo');
        if (!NextControl || (Cbo && Cbo.GetSelectedInfo()) || !Singular.Combobox.AutoFocus) {
          //Next control is a combo / drop down with a selected item, dont focus it.
          ti.focus();
          setTimeout(function () { JDropDiv.hide(); }, 0);
        } else {
          JDropDiv.hide();
          Singular.FocusNextControl(ti);

        }

      }
    });

    $(document).mousedown(function (e) {
      if (DropDiv.offsetParent && Singular.Combobox.DropDownDiv.CurrentCombo) { //is visible
        //check if the popup must be hidden.
        if (!(elementIsParent(DropDiv, e.target) || elementIsParent(Singular.Combobox.DropDownDiv.CurrentCombo.TextInput, e.target))) {
          JDropDiv.hide();
        }
      }
    });

    Singular.Combobox.DropDownDiv = {
      Div: DropDiv, HasFocus: function () {
        return InClick || document.activeElement == DropDiv || $(DropDiv).find(':focus').length > 0;
      }
    };

    if (window.addEventListener) {
      window.addEventListener('scroll', function (e) {
        var dd = Singular.Combobox.DropDownDiv.Div;
        if (e.target != dd && dd.style.display != '') $(dd).hide();
      }, true);
    }

  }
};

Singular.Combobox.AutoFocus = true;

Singular.AddOnTheFly = function (Args) {

  if (!Args.VMProperty.AOTFManager) {
    Args.VMProperty.AOTFManager = new AOTFManager(Args);
  }
  Args.VMProperty.AOTFManager.BeginAdd();

}

var AOTFManager = function (AOTFArgs) {
  var self = this;
  var Private = { AddStack: [] };
  var PopupDiv = $('#' + AOTFArgs.VMProperty.PropertyName)[0];
  if (!PopupDiv) throw 'Error: add on the fly not found for ' + AOTFArgs.VMProperty.PropertyName;

  self.IsBusy = ko.observable(false);

  self.BeginAdd = function () {

    var Value = AOTFArgs.VMProperty.peek();

    if (!Value) {
      //if this is the first object of this type being added.
      Singular.ShowDialog(PopupDiv, { title: AOTFArgs.Prompt, modal: true, close: ClearCurrent, Class: 'Editable' });
      AOTFArgs.VMProperty.Set();
    } else {
      //if there was another object being added, then remember it, and set the add context to a new object.
      Private.AddStack.push(Value);
      AOTFArgs.VMProperty.Set();
    }
    setTimeout(function () {
      $(PopupDiv).find('input:first').focus();
    }, 0);


  }
  self.SaveCurrent = function () {
    //Check if its valid
    if (AOTFArgs.VMProperty.peek().IsValid()) {
      self.IsBusy(true);
      Singular.SaveDataStateless(AOTFArgs.VMProperty.peek(), function (args) {
        self.IsBusy(false);
        if (args.Success) {
          self.Close();
          AOTFArgs.AfterAdd(args.SavedObject);
        } else {
          Singular.ShowMessage(AOTFArgs.Prompt, '<strong>Error saving:</strong><p>' + args.ErrorText + '</p>');
        }
      });

    }

  }
  self.Close = function () {
    PopupDiv.DlgClose();
    if (AOTFArgs.FromControl) {
      AOTFArgs.FromControl.focus();
    }
  }

  var ClearCurrent = function () {
    if (Private.AddStack.length > 0) {
      //if there was an object being edited before this, then put it back in the VM property.
      AOTFArgs.VMProperty.Set(Private.AddStack.pop());
      //Show the dialog again.
      Singular.ShowDialog(PopupDiv);
    } else {
      //otherwise close the dialog.
      AOTFArgs.VMProperty.Clear();
    }
  }
}