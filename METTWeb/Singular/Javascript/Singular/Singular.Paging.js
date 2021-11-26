ko.bindingHandlers.PagedGrid = {
  init: function (element, valueAccessor, allBindingsAccessor) {

    var PageManager = ko.utils.unwrapObservable(valueAccessor());
    PageManager.EnsureParams();

    $(element).find('th').each(function () {

      var Header = $(this);

      var Property = Header.attr('data-Property');
      if (Property) {
        Header.css({ cursor: 'pointer' });
        this.__SelectedClass = ko.computed(function () {
          return (PageManager.SortColumn() == Property ? 'ColSelected' : '');
        })
        ko.applyBindingsToNode(this, { css: this.__SelectedClass });

        Header.click(function () {
          PageManager.Sort(Property);
          Singular.FormatSortableHeader(element, this, PageManager.SortAsc());
        });
      }


    });

    var Pager = $(element).find('.Pager');
    Pager.find('button, a.PagerButton').each(function (i) {
      $(this).click(function () {
        switch (i) {
          case 0:
            PageManager.First();
            break;
          case 1:
            PageManager.Prev();
            break;
          case 2:
            PageManager.Next();
            break;
          case 3:
            PageManager.Last();
            break;
        }
        return false;
      });
    });
  }
};

var PageInfoManager = function (Parent) {
  SetupKOObject(this, Parent, function (self) {

    var IsUpdating = false;
    self.PropertyName = ko.observable();
    self.PropertyName.subscribe(function (NewValue) {
      //Get the observable from the name.
      self.Property(self.GetParent()[NewValue]);
      self.EnsureParams();
    });
    self.TypeName = ko.observable('');
    self.Property = ko.observable();

    self.CriteriaProperty = ko.observable();

    self.SortColumn = ko.observable();
    self.SortAsc = ko.observable(true);

    self.ActualPageNo = ko.observable(1);

    CreateProperty(self, 'PageNo', 1).subscribe(function (NewValue) {
      if (!IsUpdating) {
        if (NewValue <= 0) {
          self.PageNo(1);
        } else if (NewValue > self.Pages.peek()) {
          self.PageNo(self.Pages.peek());
        } else {
          self.Refresh(NewValue);
        }
      }
    });

    self.TotalRecords = ko.observable(0);
    self.PageSize = ko.observable(1);
    self.Pages = ko.computed(function () {
      return self.TotalRecords() == 0 ? 1 : Math.ceil(self.TotalRecords() / self.PageSize());
    });
    self.PageDescription = ko.computed(function () {
      if (self.TotalRecords() == 0) {
        return 'Nothing to display';
      } else {
        return 'Displaying ' + self.Start() + ' - ' + self.End() + ' of ' + self.TotalRecords();
      }
    });
    self.Start = ko.computed(function () {
      return Math.max(((self.ActualPageNo() - 1) * self.PageSize()) + 1, 0);
    });
    self.End = ko.computed(function () {
      return Math.min(self.TotalRecords(), self.Start() + self.PageSize() - 1);
    });
    self.First = function () {
      var prevPageNo = self.PageNo();
      self.PageNo(1);
      self.afterPagerButtonClicked('First', prevPageNo, self.PageNo());
    }
    self.Last = function () {
      var prevPageNo = self.PageNo();
      self.PageNo(self.Pages());
      self.afterPagerButtonClicked('Last', prevPageNo, self.PageNo());
    }
    self.Next = function () {
      var prevPageNo = self.PageNo();
      if (self.PageNo.peek() < self.Pages.peek()) self.PageNo(self.PageNo.peek() + 1);
      self.afterPagerButtonClicked('Next', prevPageNo, self.PageNo());
    }
    self.Prev = function () {
      var prevPageNo = self.PageNo();
      if (self.PageNo.peek() > 1) self.PageNo(self.PageNo.peek() - 1);
      self.afterPagerButtonClicked('Prev', prevPageNo, self.PageNo());
    }
    self.IsLoading = ko.observable(false);

    self.Sort = function (SortColumn) {
      if (self.SortColumn.peek() == SortColumn) {
        self.SortAsc(!self.SortAsc.peek());
      } else {
        self.SortColumn(SortColumn);
        self.SortAsc(true);
      }
      self.Refresh(self.PageNo.peek(), self.SortColumn.peek(), self.SortAsc.peek());
    }
    self.EnsureParams = function () {
      if (self.Property.peek().peek().length > 0) {
        self.TotalRecords(self.Property.peek().TotalRows);
      } else {
        self.TotalRecords(0);
      }
    }

    self.Refresh = function (PageNo, SortColumn, SortAsc) {

      //Create the criteria
      var Criteria, KOCriteria;
      if (self.CriteriaProperty.peek()) {

        var Formatter = new KOFormatterObject();
        Formatter.IncludeClean = true;
        Formatter.IncludeCleanProperties = true;

        var KOCrit = self.GetParent()[self.CriteriaProperty.peek()].peek();
        if (!KOCrit) {
          KOCrit = self.GetParent()[self.CriteriaProperty.peek()].Set();
        }

        Criteria = Formatter.Serialise(KOCrit);

      } else {
        Criteria = {};
      }

      Criteria.SortColumn = SortColumn == undefined ? self.SortColumn.peek() : SortColumn;
      Criteria.SortAsc = SortAsc == undefined ? self.SortAsc.peek() : SortAsc;
      Criteria.PageNo = PageNo == undefined ? self.PageNo.peek() : PageNo;
      Criteria.PageSize = self.PageSize.peek()

      self.IsLoading(true);
      if (self.beforeRefresh) { self.beforeRefresh() }
      Singular.GetDataStateless(self.TypeName.peek(), Criteria, function (args) {

        if (args.Success) {
          self.Property()([]);

          self.Property.peek().Set(args.Data);
          self.EnsureParams();
          self.ActualPageNo(Math.min(self.PageNo.peek(), self.Pages()));

        } else {
          alert(args.ErrorText);
        }

        self.IsLoading(false);
        if (self.afterRefresh) { self.afterRefresh(args) }
      });

    };

    //Selected Items
    var options = {
    };
    self.SetOptions = function (NewOptions) {
      options = NewOptions;
    };
    CreateProperty(self, "SingleSelect", true);
    CreateProperty(self, "MultiSelect", false);
    CreateTypedProperty(self, 'SelectedItems', SelectedItemObject, true);

    self.onRowSelectedBase = function (event, obj, element, callback) {
      var Found = self.SelectedItems().Find("ID", obj.SInfo.KeyProperty());
      if (Found) {
        self.RemoveSelectedItem(Found);
      } else {
        if (self.SingleSelect()) {
          if (self.SelectedItems().length < 1) {
            var n = callback(obj, element, event);
            self.SelectedItems.Add(n);
            if (options.AfterRowSelected) { options.AfterRowSelected(obj) }
          } else if (self.SelectedItems().length >= 1) {
            self.SelectedItems([]);
            var n = callback(obj, element, event);
            self.SelectedItems.Add(n);
            if (options.AfterRowSelected) { options.AfterRowSelected(obj) }
          }
        } else if (self.MultiSelect()) {
          var n = callback(obj, element, event);
          self.SelectedItems.Add(n);
          if (options.AfterRowSelected) { options.AfterRowSelected(obj) }
        }
      }
      //cleanup
      Found = null;
    };
    self.RemoveSelectedItem = function (SelectedItem) {
      self.SelectedItems.RemoveSelectedItem(SelectedItem);
      if (options.AfterRowDeSelected) { options.AfterRowDeSelected(SelectedItem) }
    };
    self.beforeRefresh = null;
    self.afterRefresh = null;
    self.afterPagerButtonClicked = function (what, prevPageNo, newPageNo) {
      
    };

    self.GetCriteria = function () {
      return self.GetParent()[self.CriteriaProperty.peek()].peek();
    };

    var DELAY = 200,
      fetchTimeout = null;
    self.DelayedRefresh = function () {
      clearTimeout(fetchTimeout);// clear any running timeout
      fetchTimeout = setTimeout(function () {
        self.Refresh();
      }, DELAY); // create a new timeout
    };

    self.HasDoneInitialLoad = ko.observable(false);

  });
};

var SelectedItemObject = function (Parent) {
  SetupKOObject(this, Parent, function (self) {
    CreatePropertyN(self, 'ID', 0);
    CreateProperty(self, 'Description', '');
    CreateProperty(self, 'Object');
  });
};