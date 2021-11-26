// Version 1.0.33
ko.bindingHandlers.SGrid = {
  init: function (element, valueAccessor, allBindings) {
   
    var GridInfo = Singular.UnwrapFunction(valueAccessor());
    GridInfo.TargetElement = element;
    GridInfo.Grid = null;
    if (GridInfo.GetOptions().FetchDataOnLoad) GridInfo.FetchData();
  
  }
}

var SGridInfo = function (Parent) {
  SetupKOObject(this, Parent, function (self) {

    var _Options;

    function LayoutChanged(NewValue) {
      if (self.Grid) {
        self.Grid.LoadLayout(self.CurrentLayout(), self.Schema);
        Singular.GridChart.CurrentChart(null);
      }
    }

    self.TargetElement = null;
    self.FriendlyName = ko.observable();
    self.UniqueKey = ko.observable();
    self.ClassType = ko.observable();
    self.SelectedLayoutName = ko.observable();
    self.SelectedLayoutName.subscribe(LayoutChanged);
    self.HasLoaded = ko.observable(false);
    self.GetDataParams = ko.observable();
    self.LayoutList = ko.observableArray([]);

    self.ShowHeader = ko.observable();
    self.AllowDataExport = ko.observable();
    self.AllowPrint = ko.observable();
    self.AllowGraph = ko.observable();
    self.AllowSaveLayout = ko.observable();

    self.Options = ko.observable();
    self.AllowGroupBy = ko.observable();
    self.AllowEdit = ko.observable();
    self.OnCellEditFunction = ko.observable();
    self.AfterCellEditFunction = ko.observable();

    self.Exporting = ko.observable(false);
    self.ExportText = ko.observable('Export');
    self.PrintText = ko.observable('Print');
    self.ShowLayoutSelect = ko.computed(function () {
      return self.LayoutList().length > 1;
    });
        
    self.GetOptions = function () {
      if (!_Options) {
        _Options = self.Options();
        for (var prop in _Options) {
          if (prop.indexOf('Function') > 0 && _Options[prop]) {
            _Options[prop.replace('Function', '')] = eval(_Options[prop]);
          }
        }
      }
      return _Options;
    }

    self.CurrentLayout = function () {
      var Layout = self.CurrentLayoutInfo();
      return Layout ? Layout.LayoutInfo : '';
    }
    self.CurrentLayoutInfo = function () {
      return self.LayoutList().Find('LayoutName', self.SelectedLayoutName());
    }

    var GetArguments = function () {
      var Params = self.GetDataParams();
      if (_Options.GetArguments) Params = _Options.GetArguments(Params)
      return { GetDataParams: Params, ObjectType: self.ClassType() };
    }
  
    self.FetchData = function (OnComplete) {

      var SetStatus = function (Heading, Status) {
        clearInterval(_LoadTimer);
        self.LoadingText(Heading);
        self.LoadingStatus(Status);
      }

      Singular.CallServerMethod(Singular.SGrid.ApiPath, 'GetData', GetArguments(),
        function (result) {
          if (result.Success) {

            self.Data = result.Data.Data;
            self.Schema = result.Data.Schema;
            self.LayoutList(result.Data.LayoutList);

            SetStatus('Loaded', 'Please wait for the data to display.');
            setTimeout(function () {
              self.HasLoaded(true);
              
              var opt = self.GetOptions();

              if (opt.DataTransform) opt.DataTransform(self.Data, self.Schema, opt);
              if (!self.Grid) {

                //Set width
                if (self.TargetElement.width == 0) self.TargetElement.width = $(self.TargetElement).parent().width();
                if (self.TargetElement.width == 0) return; //cant setup grid if it doesnt have a width.

                if (opt.AutoHeightFunction) opt.AutoHeight = opt.AutoHeight();
                if (!opt.AutoHeight && !$(self.TargetElement).height()) {
                  //make grid full screen
                  opt.AutoHeight = $(self.TargetElement).parents('.ReportContainer').find('.SGrid-Toolbar').outerHeight();
                }

                var Grid = new Singular.SGrid(self.TargetElement, opt);
                if (opt.StyleGrid) opt.StyleGrid(Grid);
                self.Grid = Grid;
              }
              self.Grid.SetData(self.Data, opt);
              self.Grid.LoadLayout(self.CurrentLayout(), self.Schema || {});


              if (OnComplete) OnComplete();
            }, 50);

          } else {
            SetStatus('Error', 'An error occured retrieving the data for this report: ' + result.ErrorText);
          }

        });

    }

    self.Export = function (ExportType) {

      self.Exporting(true);
      var Text = ExportType == 1 ? self.ExportText : self.PrintText;
      var OriginalText = Text();
      Text('Exporting...');

      var OnProgress = function (pi) {
        Text(pi.CurrentStatus);
      }

      Singular.CallServerMethod(Singular.SGrid.ApiPath, 'Export' + (ExportType == 1 ? 'Excel' : 'PDF'),
        { OnProgress: OnProgress, ReportData: GetArguments(), Layout: self.Grid.SaveLayout(true, true) }, function (result) {

          function Complete() {
            self.Exporting(false);
            Text(OriginalText);
          }

          if (result.Success) {
            Singular.OnDownloadComplete(Complete);
            Text('Downloading...');
            window.location = Singular.RootPath + '/Library/FileDownloader.ashx?TempGUID=' + result.Data;
          } else {
            Complete();
            alert('Error exporting data: ' + result.ErrorText);
          }

        });
    }

    self.LoadingText = ko.observable('Loading data');
    self.LoadingStatus = ko.observable('Please wait for the data to load.');
    var _LoadTimer = setInterval(function () {
      var LoadText = self.LoadingText() + '.';
      if (LoadText.length > 17) LoadText = 'Loading data';
      self.LoadingText(LoadText);
    }, 400);

    self.ShowChart = function () {

      Singular.GridChart.LoadGraphScripts(function () {

        var Chart = Singular.GridChart.CurrentChart.peek();
        if (!Chart) {
          var ChartLayout = self.CurrentLayoutInfo().ChildList[0];
          if (ChartLayout) ChartLayout = JSON.parse(ChartLayout.LayoutInfo);
          Chart = new Singular.GridChart(document.getElementById('ChartPopup'), self, true, ChartLayout);
          Singular.GridChart.CurrentChart(Chart);

        }

        Chart.Show();

      }, Singular.GridChart.RadialColors);
    }
    self.SaveChart = function () {
      var Chart = Singular.GridChart.CurrentChart();
      var GridLayout = self.CurrentLayoutInfo();

      //First, save the grids layout
      Singular.CallServerMethod(Singular.SGrid.ApiPath, 'SaveLayout',
        { UniqueKey: self.UniqueKey(), LayoutName: GridLayout.LayoutName, LayoutInfo: JSON.stringify(self.Grid.SaveLayout(true)) }, function (result) {

          if (result.Success) {
            //Grid layout saved, now save chart layout
            var ChartLayout = GridLayout.ChildList[0]; //Only allow 1 graph per grid
            if (!ChartLayout) {
              ChartLayout = { LayoutName: 'Default Chart', ParentLayout: GridLayout.LayoutName };
              GridLayout.ChildList.push(ChartLayout);
            }
            ChartLayout.LayoutObj = Chart.GetLayoutInfo();
            ChartLayout.LayoutInfo = JSON.stringify(ChartLayout.LayoutObj);
            Singular.CallServerMethod(Singular.SGrid.ApiPath, 'SaveLayout',
              { UniqueKey: self.UniqueKey(), LayoutName: ChartLayout.LayoutName, LayoutInfo: ChartLayout.LayoutInfo, ParentLayout: GridLayout.LayoutName },
              function (result) {
                if (result.Success) {
                  alert('Chart Saved Successfully');
                }
              });

          }
        });
    }

  });
}

Singular.SGrid = function (Canvas, SetupArgs) {
  SetupArgs = SetupArgs || {};
  
  var self = this,
    Context = Canvas.getContext('2d'),
    Parent,
    MainRect,
    VScroll = new ScrollBar(),
    VScrollX,
    HScroll = new ScrollBar(true, 100),
    HScrollY,
    TooWide = false,
    Width = -1,
    _RootData = { Root: [] },
    _MultiBand = false,//true if there is a child list with at least 1 item in it.
    _GridList = [], //list of grids being drawn, will contain the main grid, and any expanded visible bands.
    RowExpanders = [],
    RowIndexes = [],
    GridRect,
    GroupByRect,
    CHAR_SEPERATOR = String.fromCharCode(29),
    BandID = 0,
    SuspendLayout = false,
    PositionOffset = {},
    _AutoHeight = SetupArgs.AutoHeight,
    NoData = false,
    _TouchedInCanvas = false,
    _Opt = {}, //Options
    _Editor;

  self.UniqueKey = SetupArgs.UniqueKey;

  //Constants
  var COL_FILTER_WIDTH = 15,
      COL_SORT_WIDTH = 12,
      GROUP_BY_HEIGHT = 32,
      GROUP_HEADER_PADDING = 5,
      GROUP_HEADER_GAP = 20,
      GROUPBOX_PADDING = 3,
      GRID_GAP = 5,
      NUMBERWIDTH = 120,
      DATEWIDTH = 120,
      STRINGWIDTH = 150,
      FONT_NAME = 'Calibri';

  var ElementType = {
    Column: 1,
    Row: 2,
    RowExpansion: 3,
    GroupBy: 4,
    Cell: 5,
    GridSelector: 6,
    RowSelector: 7
  }

  //#region Utils 

  function GetBandID() {
    return ++BandID;
  }

  var defaultable = function (Name, Parent, Value, AfterSet) {
    var self = this;
    this.__Values = this.__Values || {};
    this.__Values[Name] = Value;
    this.__DefParent = Parent;
    Object.defineProperty(this, Name, {
      get: function () {
        var Val = this.__Values[Name];
        if (Val === undefined) {
          return this.__DefParent[Name];
        } else {
          return Val;
        }
      },
      set: function (NewValue) {
        if (NewValue != this.__Values[Name]) {
          this.__Values[Name] = NewValue;
          if (AfterSet) AfterSet(NewValue);
        }
      }
    });

  }

  if (!Object.Defaultable) {
    Object.defineProperty(Object.prototype, 'Defaultable', {
      value: defaultable, enumerable: false
    });
  }

  var AddProperty = function (Object, PropertyName, DefaultValue, OnChanged) {
    Object[PropertyName] = ko.observable(DefaultValue);
    if (OnChanged) {
      Object[PropertyName].subscribe(OnChanged);
    }
  }
  
  //#endregion 
	
  //#region Group By

  var GroupByInfo = function (DataBand, Text) {
    this.DataBand = DataBand;
    this.IncludeCount = true;
    this.Text = Text;
    this.SizedText = Text;
    this.Rect = null;
		
  }

  GroupByInfo.InsertColumn = function (Column, ElemHover) {
    
    var Groups = [],
 	      Index = ElemHover.Index;
    if (self.Bands[Index].GroupByInfo && self.Bands[Index].GroupByInfo.DataBand == Column.Band) {
      Groups.push(self.Bands[Index]);
      self.Bands[Index].RemoveGrouping(Groups);
    }
    TempBands = Groups;
    Groups.insert(0, Column.CreateGroupByBand(Groups[0]));
    TempBands = [];

    Column.Band.CreateGroupings(Groups);

  };
  GroupByInfo.HeaderText = '';
  GroupByInfo.Refresh = function () {

    GroupByInfo.HeaderText = 'Drag a column here to group its data';
    var Left;

    for (var i = 0; i < self.Bands.length; i++) {
      var band = self.Bands[i];
      if (band.GroupByInfo) {

        if (Left == undefined) {
          GroupByInfo.HeaderText = 'Grouped columns';
          Left = Context.measureText(GroupByInfo.HeaderText).width + 20;
        }

        band.GroupByInfo.SizedText = DrawUtils.GetSizedText(Context, band.GroupByInfo.Text, 150);
        var Width = Context.measureText(band.GroupByInfo.Text).width +
            GROUP_HEADER_PADDING * 2 + /* Padding */
            (band.AllowColumnChooser ? (5 + 16) : 0) /* Gap and Options */;

        band.GroupByInfo.Rect = new Rectangle(Left, GROUPBOX_PADDING, Width, GROUP_BY_HEIGHT - (GROUPBOX_PADDING*2));
        Left += Width + GROUP_HEADER_GAP;
      }
    }
  }
  GroupByInfo.Draw = function () {

    //Main group by area
    if (GroupByRect) {
      Context.fillStyle = self.DefaultStyles.GroupByStyle.BackColor;
      GroupByRect.Fill(Context);
      Context.font = '13px ' + FONT_NAME;
      Context.textAlign = 'left';

      //Calculate group by column rectangles.
      if (CalcBounds) GroupByInfo.Refresh();

      //Group by promt text
      Context.textBaseline = 'middle';
      Context.fillStyle = self.DefaultStyles.GroupByStyle.ForeColor;
      Context.fillText(GroupByInfo.HeaderText, 10, GROUP_BY_HEIGHT / 2);

      var MoveToElement;
      if (ElemClicked && ElemHover && ElemHover.Type == ElementType.GroupBy && ElemHover.IsMove) MoveToElement = ElemHover;

      function DrawMoveTo(X) {
        X = Math.floor(X - ColMoveImage.width / 2);
        Context.drawImage(ColMoveImage, X, GROUPBOX_PADDING);
        Context.drawImage(ColMoveImageF, X, GROUP_BY_HEIGHT - GROUPBOX_PADDING - ColMoveImageF.height);
      }

      var gbi;
      for (var i = 0; i < self.Bands.length; i++) {
        var band = self.Bands[i];
        if (band.GroupByInfo) {
          //Column Rectangle
          Context.CreateShadow(1, 1, 2, 'rgba(0,0,0,0.3)', true);
          gbi = band.GroupByInfo;
          Context.fillStyle = '#333';
          gbi.Rect.MakeRounded(Context, 2).fill();

          //Text
          Context.CreateShadow(1, 1, 1, 'rgba(255,255,255,0.2)');
          Context.font = '13px ' + FONT_NAME;
          Context.fillStyle = '#fff';
          Context.fillText(gbi.Text, gbi.Rect.X + GROUP_HEADER_PADDING, gbi.Rect.Y + (gbi.Rect.Height / 2));
          Context.restore();

          //Options icon
          if (band.AllowColumnChooser) {
            Context.font = '14px FontAwesome';
            if (ElemHover && ElemHover.Type == ElementType.GroupBy && !ElemHover.Dragging && ElemHover.Band == band) {
              Context.fillStyle = '#eee';
            } else {
              Context.fillStyle = '#555';
            }
            Context.fillText('\uf013', gbi.Rect.Right() - 13 - GROUP_HEADER_PADDING, gbi.Rect.Y + (gbi.Rect.Height / 2));
          }

          //Move to icon
          if (MoveToElement && MoveToElement.Index <= i) {
            if (MoveToElement.MoveForward) {
              DrawMoveTo(gbi.Rect.Right() + GROUP_HEADER_GAP / 2);
            } else {
              DrawMoveTo(gbi.Rect.X - (i == 0 ? ColMoveImage.width : GROUP_HEADER_GAP) / 2);
            }

            MoveToElement = null;
          }
        }

      }
      if (MoveToElement && gbi) DrawMoveTo(gbi.Rect.Right() + GROUP_HEADER_GAP / 2);
    }

  }

  //#endregion

  //#region Bands

  self.Bands = [];

  var Band = function (Parent, Key) {
    var thisBand = this;
    this.ParentBand = Parent;
    this.Key = Key;
    this.BandID = GetBandID();
    this.ChildBands = [];
    this.Level = Parent == null ? 0 : Parent.Level + 1;
    this.MaxRowIndex = 0;
    this.GroupByInfo = null; //will be set if this is a group by band.
    this.Columns = [];
    this.SGrid = self;
    this.ShowInExport = true;
    this.ShowInReport = true;
    this.AllowColumnChooser = true;

    this._GroupColumns = [];
    this._SummaryColumns = [];
		
    this.HeaderStyle = new CellStyle(self.DefaultStyles.HeaderStyle);
    this.CellStyle = new CellStyle(self.DefaultStyles.CellStyle);

    var index = NextColorIndex();
    this.RowColor = self.DefaultStyles.RowColors[index];
    this.RowAltColor = self.DefaultStyles.RowAltColors[index];
    this.RowSelectGradient = [];
    this.FooterSelectGradient = [];
    this.FrozenColumn = -1;

    this.RowBorderColor = self.DefaultStyles.RowBorderColors[index];

    this._ColHeaderLines = 1;
    this.SetColHeaderLines = function (num) {
      CalcBounds = true;
      thisBand._ColHeaderLines = Math.max(1, num);
      thisBand.HeaderStyle.Height = GetHeaderHeight(num);
    }

    function GetHeaderHeight(lines) {
      return (thisBand.HeaderStyle.PaddingV * 2) + (thisBand.HeaderStyle.FontSize * lines) + (lines - 1);
    }

    this.GetFooterHeight = function () {
      return GetHeaderHeight(1);
    }

    this.AddColumn = function(Col){
      this.Columns.push(Col);
      Col.Band = this;
      if (Col._SummariseType) {
        this._SummaryColumns.push(Col);
      } else {
        this._GroupColumns.push(Col);
      }
    }

    this.FindColumns = function (Data, ColSchema) {
      
      //merge the info from the data schema, and the first row.
      var ColList = {};
      if (Data[0]) {
        for (name in Data[0]) {
          ColList[name] = { Val: Data[0][name] };
        }
      }
      if (ColSchema) {
        for (name in ColSchema) {
          if (ColList[name]) {
            ColList[name].Schema = ColSchema[name];
          } else {
            ColList[name] = { Schema: ColSchema[name] };
          }
        }
      }
                      
      for (name in ColList) {
        if (name.indexOf('_') != 0) {

          var value = ColList[name].Val,
              ColSc = ColList[name].Schema;

          if (value instanceof Array || (ColSc && ColSc.Properties)) {
            //Child bands handled later.
          } else if (!(value instanceof Date) && value instanceof Object) {
            //ignore single objects.
          } else {

            var col = this.Columns.Find('Key', name);
            if (!col && !(ColSc && !!ColSc.Hidden)) {
              col = new Column(this, name);
              col.SetDefaults(value, ColSc);
              this.Columns.push(col);
            }
          }

        }
      }
      this.InitRowInfo(Data);

      //Find child bands
      for (name in ColList) {
        var value = ColList[name].Val,
            ColSc = ColList[name].Schema;
        if (value instanceof Array || (ColSc && ColSc.Properties)) {
          //Create a new band
          var band = self.Bands.Find('Key', name);
          if (!band && !(ColSc && !!ColSc.Hidden)) {
            band = new Band(this, name);
            self.Bands.push(band);
            this.ChildBands.push(band);
            band.FindColumns(value, ColSc ? ColSc.Properties : null);
          }

        }
      }
    }
    
    this.GetX = function () {
      return _MultiBand ? ((this.Level + 1) * self.DefaultStyles.RowSelectorWidth) : 0
    }

    this.Rows = [];

    this.IterateRows = function (CallBack, VisibleOnly) {
      if (VisibleOnly) {

        for (var i = 0; i < _GridList.length; i++) {
          var grid = _GridList[i];
          if (grid.Band == thisBand) {
            //var list = grid.ParentRow[Key];
            for (j = grid.MinRowIndex; j <= grid.MaxRowIndex; j++) {
              var Row = RowIndexes[j].Row;
              if (Row) CallBack(Row);
            }
          }
        }

      } else {

        var Iterator = function (row) {
          var list = row[Key];
          for (var i = 0; list && i < list.length; i++) {
            if (CallBack(list[i], row)) break;
          }
        }

        if (thisBand.ParentBand == null) {
          Iterator(_RootData);
        } else {
          thisBand.ParentBand.IterateRows(Iterator, false);
        }
        //for (var i = 0; i < this.Rows.length; i++) {
        //  var Row = this.Rows[i];
        //  CallBack(Row.Row, Row.Parent, Row);
        //}
      }


    }

    this.FindMaxRowIndex = function () {
      for (var i = RowIndexes.length - 1; i >= 0; i--) {
        if (RowIndexes[i].Band == thisBand) {
          this.MaxRowIndex = i;
          return;
        }
      }
    }

    this.CreateGrouping = function (NewBand, SkipSummaries, UserActioned) {
       
      var GroupKey = NewBand.Key,
			    ParentBand = this.ParentBand,
		      GroupInfo = NewBand.GroupByInfo;

      var LastParentRow, ChildList, Value, GroupColSingle;
      if (NewBand._GroupColumns.length == 1) {
        GroupColSingle = NewBand._GroupColumns[0];
      }
      if (UserActioned) {
        NewBand._GroupColumns.Iterate(function (col) {
          col.OrigCol.Visible = false;
        });
      }

      //Group the data.
      this.IterateRows(function (row, parentRow, container) {

        //Get the group value
        if (GroupColSingle) {
          Value = GroupColSingle.GetTransformedValue(row);
        } else {
          Value = '';
          for (var i = 0; i < NewBand._GroupColumns.length; i++) {
            var GroupCol = NewBand._GroupColumns[i];
            Value += GroupCol.GetTransformedValue(row).toString() + CHAR_SEPERATOR;
          }
        }

        if (LastParentRow == null || LastParentRow != parentRow) {
          if (LastParentRow) {
            //clean up the parent row we've just finished with.
            delete LastParentRow[GroupKey].HashTable;
            delete LastParentRow[thisBand.Key];
          }
          ChildList = [];
          ChildList.HashTable = {};
          ChildList.VisibleCount = 0;
          parentRow[GroupKey] = ChildList;
          LastParentRow = parentRow;
        }
        var groupedRow = ChildList.HashTable[Value];
        if (!groupedRow) {
          //Create the row for this group by value.
          groupedRow = { Count: 0 };
          SetRowInfo(groupedRow);

          //Set the grouped by field(s) values
          for (var i = 0; i < NewBand._GroupColumns.length; i++) {
            var GroupCol = NewBand._GroupColumns[i];
            groupedRow._SGrid.SetValue(GroupCol, GroupCol._.LastTransformedValue);
            if (GroupCol._DataTransform && GroupCol._DataTransform.SetRowValues) {
              GroupCol._DataTransform.SetRowValues(groupedRow, GroupCol);
            }
          }

          //Set the child list to this bands list.
          groupedRow[thisBand.Key] = [];

          ChildList.push(groupedRow);
          //NewBand.Rows.push({ Row: groupedRow, Parent: LastParentRow });
          ChildList.HashTable[Value] = groupedRow;
        }
        groupedRow[thisBand.Key].push(row);
        //container.Parent = groupedRow;
        //row._SGrid.Parent = groupedRow;

      });
      //remove the original ungrouped data from the parent row.
      if (LastParentRow) {
        delete LastParentRow[GroupKey].HashTable;
        delete LastParentRow[thisBand.Key];
      } else {
        return false; //Cant group if there are no rows.
      }
      
      //Insert the new band level before the current band.
      var thisBandIndex = -1;
      NewBand.Level = thisBand.Level;
      for (var i = 0; i < self.Bands.length; i++) {
        if (self.Bands[i] == thisBand) {
          thisBandIndex = i;
          break;
        }
      }
      self.Bands.insert(thisBandIndex, NewBand);

      if (thisBand.ParentBand) {
        //replace child band with the new band.
        for (var i = 0; i < thisBand.ParentBand.ChildBands.length; i++) {
          if (thisBand.ParentBand.ChildBands[i] == thisBand) {
            thisBand.ParentBand.ChildBands[i] = NewBand;
          }
        }
      }
      NewBand.ChildBands = [thisBand];
      NewBand.ParentBand = thisBand.ParentBand;
      thisBand.ParentBand = NewBand;
      thisBand.ResetTotals();

      Band.RefreshLevels();
      
      if (!SkipSummaries) NewBand.RefilterGrouping();
      if (!SuspendLayout && _Opt.AfterLayout) _Opt.AfterLayout(self, NewBand, true);

      return true;
    }

    this.CreateGroupings = function (Groups) {
      for (var i = 0; i < Groups.length ; i++) {
        this.CreateGrouping(Groups[i], i < Groups.length-1, true);
      }

      FlattenAllRows();
		 
    }

    this.RemoveGrouping = function (RemovedChildren) {

      var ChildBand = thisBand.ChildBands[0];
      //If there are any groups below this one for the same data, remove them to be added later.
      if (ChildBand && ChildBand.GroupByInfo && ChildBand.GroupByInfo.DataBand == this.GroupByInfo.DataBand) {
        RemovedChildren.push(ChildBand);
        ChildBand.RemoveGrouping(RemovedChildren);
      }

      this._GroupColumns.Iterate(function(col){
        col.OrigCol.Visible = true;
      });
     
      var LastParentRow;
      var ChildKey = thisBand.ChildBands[0].Key;

      thisBand.IterateRows(function (row, parentRow, container) {
        //move the grouped rows of this group to the parent.
        if (parentRow[ChildKey] == undefined) {
          parentRow[ChildKey] = [];
        }
        var ChildItems = row[ChildKey];
        for (var i = 0; i < ChildItems.length; i++) {
          parentRow[ChildKey].push(ChildItems[i]);
          //ChildItems[i]._SGrid.Parent = parentRow;
        }

        if (LastParentRow && LastParentRow != parentRow) {
          delete LastParentRow[thisBand.Key];
        }
        LastParentRow = parentRow;
      });
      if (LastParentRow) delete LastParentRow[thisBand.Key];

      //find the band index
      var thisBandIndex = -1;
      for (var i = 0; i < self.Bands.length; i++) {
        if (self.Bands[i] == thisBand) {
          thisBandIndex = i;
          break;
        }
      }
      //connect this bands parent to this bands child.
      if (thisBand.ParentBand != null) {
        var pcb = thisBand.ParentBand.ChildBands;
        for (var i = 0; i < pcb.length; i++) {
          if (pcb[i] == thisBand) {
            pcb[i] = thisBand.ChildBands[0];
            break;
          }
        }
      }
      thisBand.ChildBands[0].ParentBand = thisBand.ParentBand;
      self.Bands.splice(thisBandIndex, 1);

      //if the parent band is a grouping of this band, remove it.
      if (thisBand.ParentBand && thisBand.ParentBand.GroupByInfo && thisBand.ParentBand.GroupByInfo.DataBand == thisBand) {
        thisBand.ParentBand.RemoveGrouping([]);
        thisBand.ParentBand = thisBand.ParentBand.ParentBand;
      }

      thisBand.ChildBands[0].ResetTotals();
      Band.RefreshLevels();
      if (thisBand.ParentBand) thisBand.ParentBand.RefilterGrouping();
      if (!SuspendLayout && _Opt.AfterLayout) _Opt.AfterLayout(self, thisBand, false);
    }

    this.ShowGroupSettings = function (Grid) {

      var gb = new Singular.SGrid.GroupBy(this);
      if (_Opt.SortColumnChooser) gb.SortColumns();
      Singular.SGrid.CurrentGroupBy(gb);
      var Dlg = $('[data-sgrid="GroupByInfo"]');

      var position = $(Canvas).offset();
      var Left = position.left, Top = position.top, height = Dlg.outerHeight(true), winHeight = $(window).height();
      if (Grid) {
        Left += Grid.X - HScroll.Value() - 1;
        Top += Grid.Y;
      } else if (this.GroupByInfo) {
        Left += this.GroupByInfo.Rect.X;
        Top += this.GroupByInfo.Rect.Y;
      }
      Left = Math.max(2, Left);
      
      if (height + Top > winHeight) {
        //if the dlg is going below the bottom of the screen, move it up.
        Top = Math.max(winHeight - height, 2);
      }
      if (height + Top > winHeight) {
        //if its still too big, make the srollable area smaller
        var scroller = Dlg.find(this.GroupByInfo ? '#Scroller' : '.tblColumns tbody');
        var ScollerHeight = (winHeight - height) + scroller.outerHeight() - Top;
        if (ScollerHeight < 150){
          Top -= (150 - ScollerHeight);
          ScollerHeight = 150;
        }
        scroller.css('max-height', ScollerHeight);
     
      }

      Dlg.css({ left: Left, top: Top });
    }

    this.RefreshGrouping = function (gi, ReGroup) {
           
      var newBand = this,
          SameBand = true,
          SummariesChanged = false;
      if (gi) {
        if (gi._GroupsChanged) {
          SameBand = false;

          newBand = new Band(this.ParentBand, this.Key);
          newBand.GroupByInfo = this.GroupByInfo;
          newBand.GroupByInfo.Text = gi.Name();

          //Group by columns
          gi.GroupColumns().Iterate(function (col) {
            var GroupCol = Column.CreateGroupColumn(newBand, thisBand.GroupByInfo.DataBand.Columns.Find('Key', col.Key()), col.GroupTypeID());
            GroupCol._DataTransformParam = col.GroupParam();
          });
          
        } else {
          //Remove summaries where the Key / Type has changed.
          newBand._SummaryColumns.IterateR(function (Col, i) {
            if (Col._Remove) {
              newBand.Columns.RemoveItem(Col);
              newBand._SummaryColumns.splice(i, 1);
              SummariesChanged = true;
            }
          });
        }
        
        //Add / refresh the Summary columns
        gi.SummaryColumns().Iterate(function (sc) {
          if (sc.Key.peek()){

            var SCol = SameBand ? sc.GridColumn : null;
            if (SCol){
              SCol._SummaryFilter = sc.Filter.peek();
            } else {
              var st = sc.SummaryType.peek(), sumCol = thisBand.ChildBands[0].Columns.Find('Key', sc.Key.peek());
              if (sc.ConditionType() == 2) {
                var XCol = thisBand.ChildBands[0].Columns.Find('Key', sc.FilterColumn());
                Column.CreateCrossTabColumn(st, newBand, sumCol, XCol);
                SummariesChanged = true;
              } else {
                SCol = Column.CreateSummaryColumn(st, newBand, sumCol, sc.Filter.peek());
                SummariesChanged = true;
              }
              
            }
          }
        });

        //Other band settings.
        newBand.ShowInExport = gi.ShowInExport();
        newBand.ShowInReport = gi.ShowInReport();

      }

      //Regroup / recalc summaries.
      if ((gi && gi._GroupsChanged) || ReGroup) {
        var ReAdd = [];
        this.RemoveGrouping(ReAdd);
        ReAdd.insert(0, newBand);
        thisBand.GroupByInfo.DataBand.CreateGroupings(ReAdd);
      } else if (SummariesChanged) {
        this.RefilterGrouping();
        FlattenAllRows();
      }

      CheckHScroll();
    }

    this.RefilterGrouping = function (ReSort, ri /*only recalc single row*/) {

      var thisBand = this;
      if (thisBand.GroupByInfo && thisBand._SummaryColumns.length > 0) {
        var cb = thisBand.ChildBands[0],
            SameBand = thisBand._SummaryColumns[0].OrigCol.Band == cb;

        //Re-link summary columns to the new child band.
        for (var i = thisBand._SummaryColumns.length - 1; i >= 0; i--) {
          var SCol = thisBand._SummaryColumns[i], Replaced = false;
           
            for (var j = 0; j < cb.Columns.length; j++) {
              var ChildCol = cb.Columns[j];

              if (SCol._SummariseType == 5 || (SameBand && SCol.OrigCol.Key == ChildCol.Key) || (!SameBand && SCol._DataKey == (cb.GroupByInfo ? ChildCol._DataKey : ChildCol.Key))) {
                //same key, check the sum type
                if (SCol._SummariseType == 5 || SameBand || SCol._SummariseType == undefined || ChildCol._SummariseType == undefined || SCol._SummariseType == ChildCol._SummariseType) {
                  Replaced = true;
                  SCol.OrigCol = ChildCol;
                  //filter
                  var sf = SCol._SummaryFilter;
                  if (sf && !cb.Columns.Find('Key', sf.Column.Key)) {
                    //SCol._SummaryFilter = null;
                    Replaced = false;
                  }
                  break;
                }
              }
            }
          
          if (!Replaced) {
            SCol.Remove();
          }
        }

        //Refilter and re total.
        var SummariseRow = function (row) {
          var ChildRows = row[cb.Key];
          row.Count = 0;

          //Iterate child rows
          for (var j = 0; j < ChildRows.length; j++) {
            var ChildRow = ChildRows[j], ChildFiltered = ChildRow._SGrid.IsFiltered();

            if (!ChildFiltered) {
              for (var i = 0; i < thisBand._SummaryColumns.length; i++) {
                var SCol = thisBand._SummaryColumns[i], sf = SCol._SummaryFilter;
                //Conditional Filter.
                if (sf) {
                  var cValue = sf.Column.GetFilterValue(ChildRow);
                  if (!sf.MatchFunction(cValue, sf.FValue)) continue;
                }
                SCol.Calc.Add(SCol._CalcObj, ChildRow._SGrid.GetValue(SCol.OrigCol));
              }
              row.Count += 1;
            }
          }

          ChildRows.VisibleCount = row.Count;
          row._SGrid.ChildrenFiltered = ChildRows.VisibleCount == 0;
        }
        var RowCalc = function (row) {
          row._SGrid.SelfFiltered = false;
          row._SGrid.Filters = {};

          //Initialise Summaries
          for (var i = 0; i < thisBand._SummaryColumns.length; i++) {
            var SCol = thisBand._SummaryColumns[i];
            SCol._CalcObj = {};
            SCol.Calc.Init(SCol._CalcObj);
          }

          SummariseRow(row);

          //Set final summary values.
          for (var i = 0; i < thisBand._SummaryColumns.length; i++) {
            var SCol = thisBand._SummaryColumns[i];
            row._SGrid.SetValue(SCol, SCol.Calc.Final(SCol._CalcObj));
          }
        }

        if (ri) {
          RowCalc(ri.Row);
        } else {
          this.IterateRows(RowCalc);
        }
        
        //reapply this bands filters after summing
        if (!ri) {
          //for (var i = 0; i < thisBand.Columns.length; i++) {
          //  if (thisBand.Columns[i].Filters.length > 0) {
          //    thisBand.Columns[i].ApplyFilters(undefined, true);
          //  }
          //  if (ReSort && thisBand.Columns[i].SortDirection != undefined) {
          //    thisBand.Columns[i].Sort(true);
          //  }
          //}
          thisBand.ReSortAndFilter(ReSort);
        }

        thisBand.ResetTotals();
        
        if (thisBand.ParentBand) thisBand.ParentBand.RefilterGrouping(undefined, ri ? ri.ParentRi : null);
      }
      
    }

    this.ReSortAndFilter = function (ReSort) {
      for (var i = 0; i < this.Columns.length; i++) {
        if (this.Columns[i].Filters.length > 0) {
          this.Columns[i].ApplyFilters(undefined, true);
        }
        if (ReSort && this.Columns[i].SortDirection != undefined) {
          this.Columns[i].Sort(true);
        }
      }
    }

    this.InitRowInfo = function (resetRowInfo) {
      var thisBand = this;

      thisBand.IterateRows(function (row, parent) {
        //make sure there is an info object on each row.
        SetRowInfo(row, resetRowInfo);
      });

      //check if any of the columns need a type conversion on the cell value.
      for (var j = 0; j < thisBand.Columns.length; j++) {
        var col = thisBand.Columns[j];
        if (col.Type == 'd') {
          thisBand.IterateRows(function (row, parent) {
            var cellValue = row[col.Key]; //for performance
            row._SGrid.SetCachedValues(col, cellValue);
          });
        }
      }

    }

    this.ResetTotals = function (Redraw) {
     
      if (thisBand.ParentBand) {
        thisBand.ParentBand.IterateRows(function (row) {
          row._SGrid.Totals = null;
        });
      } else{
        _RootData._SGrid.Totals = null;
      }
      if (Redraw) CheckDraw();

    };

    this.SaveLayout = function (lObj, AddFilters, ForExport) {
           
      lObj.ID = thisBand.BandID;
      //Save the column layout.
      var Columns = [], ColKeys = [], OrderChanged = false;
      thisBand.Columns.Iterate(function (col, i) {
        if ((!col._.CrossTabLink && !ForExport) || (ForExport && !col._.CrossTabColumn)) {
          var scol = {};
          col._SaveLayout(scol, AddFilters);
          if (Object.keys(scol).length > 1) {
            Columns.push(scol);
          }
          ColKeys.push(col.Key);
          if (col._.OrigIndex != i) OrderChanged = true;
        }
      });
     
      if (Columns.length > 0) lObj.Columns = Columns;
      //Save the order of all columns if 1 or more orders have changed.
      if (OrderChanged) {
        lObj.ColOrder = ColKeys;
      }
      //Styles
      lObj.Colors = [thisBand.RowColor, thisBand.RowAltColor, thisBand.RowBorderColor];
      if (!thisBand.ShowInExport) lObj.NoExport = true;
      if (!thisBand.ShowInReport) lObj.NoPrint = true;
      if (thisBand.FrozenColumn > -1) lObj.FrozenColumn = thisBand.FrozenColumn;
      if (thisBand._ColHeaderLines > 1) lObj.HeaderLines = thisBand._ColHeaderLines;

      if (thisBand.GroupByInfo) {
        //group by info
        lObj.DataBandID = thisBand.GroupByInfo.DataBand.BandID;
        lObj.GroupHeaderText = thisBand.GroupByInfo.Text;
      } else {
        lObj.Key = thisBand.Key;

        //child bands
        for (var i = 0; i < thisBand.ChildBands.length; i++) {
          var cObj = {};
          thisBand.ChildBands[i].SaveLayout(cObj, AddFilters, ForExport);
          if (cObj.Columns || cObj.Children || cObj.ColOrder) {
            lObj.Children = lObj.Children || [];
            lObj.Children.push(cObj);
          }
        }
      }
      
      //parent band if its a Group by band
      if (thisBand.ParentBand && thisBand.ParentBand.GroupByInfo){
        lObj.GroupBy = {};
        thisBand.ParentBand.SaveLayout(lObj.GroupBy, AddFilters, ForExport);
      }
    };

    this.LoadLayout = function (lObj, DataBand, Type /* 1=Group, 2=Summary */) {

      thisBand.BandID = lObj.ID === undefined ? GetBandID() : lObj.ID;
      BandID = Math.max(BandID, this.BandID);

      //Load column info
      if (lObj.Columns) {
        for (var i = 0; i < lObj.Columns.length; i++) {
          var col = lObj.Columns[i];
          if (!DataBand) {
            var fcol = thisBand.Columns.Find('Key', col.Key);
            if (fcol) fcol._LoadLayout(col);
          } else {
            //add the group / summary column.
            var dcol = DataBand.Columns.Find('Key', col.OrigKey), newCol;
            if (dcol) {
              if (col.SummaryType && Type == 2) {
                //Summary Col
                var Filter = null;
                if (col.FKey) {
                  var FilterCol = DataBand.Columns.Find('Key', col.FKey);
                  if (FilterCol.Type == 'd') {
                    col.FValue = new Date(col.FValue);
                  }
                  Filter = new Singular.SGrid.FilterInfo(FilterCol, col.FType, col.FValue);
                }
                if (col.CTKey) {
                  //crosstab
                  var CTCol = DataBand.Columns.Find('Key', col.CTKey);
                  newCol = Column.CreateCrossTabColumn(col.SummaryType, thisBand, dcol, CTCol);
                } else {
                  //normal
                  newCol = Column.CreateSummaryColumn(col.SummaryType, thisBand, dcol ? dcol : DataBand.Columns[0], Filter);
                }

                newCol._LoadLayout(col, true);
              } else if (!col.SummaryType && Type == 1) {
                //Group col
                newCol = Column.CreateGroupColumn(thisBand, dcol);
                newCol._LoadLayout(col, true);
              }
            }
          }
        }
      }
      //column order
      if (lObj.ColOrder && Type != 1) {
        for (var i = 0; i < lObj.ColOrder.length; i++) {
          var fcol = thisBand.Columns.Find('Key', lObj.ColOrder[i]);
          if (fcol) fcol._.Order = i;
        }
        thisBand.Columns.sort(function (a, b) {
          return a._.Order == b._.Order ? 0 : (a._.Order > b._.Order ? 1 : -1);
        });
      }

      //child bands
      if (lObj.Children) {
        for (var i = 0; i < lObj.Children.length; i++) {
          var fChild = thisBand.ChildBands.Find('Key', lObj.Children[i].Key);
          if (fChild) fChild.LoadLayout(lObj.Children[i]);
        }
      }

      //Styles
      if (lObj.NoExport) thisBand.ShowInExport = false;
      if (lObj.NoPrint) thisBand.ShowInReport = false;
      if (lObj.FrozenColumn) thisBand.FrozenColumn = lObj.FrozenColumn;
      if (lObj.HeaderLines) thisBand.SetColHeaderLines(lObj.HeaderLines);

      //group by
      var gb = lObj.GroupBy,
          GroupList = [],
          NextGB;
      //get all the group by parents that have this as the data band.
      while (gb && !DataBand) {
        if (gb.DataBandID == lObj.ID) {
          GroupList.insert(0, gb);
        }
        gb = gb.GroupBy;
      }
      
      //create the group by bands.
      for (var i = 0; i < GroupList.length; i++) {
        var band = new Band(thisBand.ParentBand, '_G_' + thisBand.Key);
        band.GroupByInfo = new GroupByInfo(thisBand, GroupList[i].GroupHeaderText);

        band.LoadLayout(GroupList[i], thisBand, 1);
        GroupList[i].Band = band;
        GroupList[i].Success = thisBand.CreateGrouping(band, true, false);
      }
      //now go through the list in reverse to create the summaries.
      for (var i = GroupList.length - 1; i >= 0; i--) {
        if (GroupList[i].Success) {
          GroupList[i].Band.LoadLayout(GroupList[i], GroupList[i].Band.ChildBands[0], 2);
          GroupList[i].Band.RefilterGrouping(true);

          if (GroupList[i].GroupBy && GroupList[i].GroupBy.DataBandID == GroupList[i].Band.BandID) {
            //group by of group by
            var gb = GroupList[i].GroupBy, dband = GroupList[i].Band;
            var band = new Band(dband.ParentBand, '_G_' + dband.Key);
            band.GroupByInfo = new GroupByInfo(dband, gb.GroupHeaderText);
            band.LoadLayout(gb, dband, 1);
            dband.CreateGrouping(band, true, false);
            band.LoadLayout(gb, dband, 2);
            band.RefilterGrouping(true);
          }
        }
      }

    };

    this.GetRowGradient = function (y, height) {
      var grad = Context.createLinearGradient(0, y, 0, y + height);
      grad.addColorStop(0, thisBand.RowSelectGradient[0]);
      grad.addColorStop(0.49, thisBand.RowSelectGradient[1]);
      grad.addColorStop(0.5, thisBand.RowSelectGradient[2]);
      grad.addColorStop(1, thisBand.RowSelectGradient[3]);
      return grad;
    }

    this.GetFooterGradient = function (y, height) {
      var grad = Context.createLinearGradient(0, y, 0, y + height);
      grad.addColorStop(0, thisBand.FooterSelectGradient[0]);
      grad.addColorStop(0.49, thisBand.FooterSelectGradient[1]);
      grad.addColorStop(0.5, thisBand.FooterSelectGradient[2]);
      grad.addColorStop(1, thisBand.FooterSelectGradient[3]);
      return grad;
    }

    function Setup() {

      var c = DrawUtils.HexToHSL(thisBand.RowColor);
      c.s -= (c.s * 0.3);
      c.l += (1 - c.l) * 0.3;
      var lOrig = c.l;
      for (var i = 0; i < 4; i++) {
        thisBand.RowSelectGradient.push(c.GetCSSColor());
        c.l -= 0.05;
      }
      c.s = 0.2;
      c.l = lOrig * 1.05;
      for (var i = 0; i < 4; i++) {
        thisBand.FooterSelectGradient.push(c.GetCSSColor());
        c.l -= 0.05;
      }
    }
    Setup();
  }
  Band.RefreshLevels = function(){
    for (var i = 0; i < self.Bands.length; i++) {
      var band = self.Bands[i];
      band.Level = band.ParentBand ? band.ParentBand.Level + 1 : 0;
    }
  }
  
  var Grid = function (StartIndex, y, PrevGrid) {
    var RowInfo = RowIndexes[_DrawIndex];
    this.Band = RowInfo.Band;
    this.X = RowInfo.Band.GetX();
    this.Y = y;
    this.ColX = this.X + self.DefaultStyles.RowSelectorWidth;
    this.MinRowIndex = StartIndex;
    this.MaxRowIndex = 0;
    this.PrevGrid = PrevGrid;
    this.RowHeight = 0;
    this.ParentRow = RowInfo.Parent;    

    while (this.PrevGrid && this.Band.Level <= this.PrevGrid.Band.Level) {
      this.PrevGrid = this.PrevGrid.PrevGrid;
    }
             
    if (typeof (this.Band.HeaderStyle.BackColor) == 'string') {
      this.HeaderBackColor = this.Band.HeaderStyle.BackColor;
    } else {
      this.HeaderBackColor = Context.createLinearGradient(0, this.Y, 0, this.Y + this.Band.HeaderStyle.Height);
      var hbc = this.HeaderBackColor;
      this.Band.HeaderStyle.BackColor.Iterate(function (stop) {
        hbc.addColorStop(stop[0], stop[1]);
      });
    }
       
    this.GetTotal = function (Col) {
      var ChildRows = this.ParentRow[this.Band.Key];

      if (!this.ParentRow._SGrid.Totals) this.ParentRow._SGrid.Totals = {};
      var Totals = this.ParentRow._SGrid.Totals;
      
      var thisBand = this.Band;
      if (!Totals[this.Band.Key]) {
        Totals[this.Band.Key] = {};

        for (var i = 0; i < thisBand.Columns.length; i++) {
          var col = thisBand.Columns[i];
          if (col.SummaryType) {

            var calc = Singular.SGrid.SummaryTypes.Items[col.SummaryType].Calc;
            var calcObj = {}
            calc.Init(calcObj);

            for (var j = 0; j < ChildRows.length; j++) {
              if (!ChildRows[j]._SGrid.IsFiltered()) {
                calc.Add(calcObj, ChildRows[j][col.Key]);
              }
            }

            Totals[this.Band.Key][col.Key] = calc.Final(calcObj);
          }
        }
      }
      return Totals[this.Band.Key][Col.Key];
    }

  }

  //#endregion

  //#region Styling / Settings
  
  // Styles
  function CellStyle(Parent, PadH, PadV, FontS, BackColor, ForeColor, BorderColor) {
    var self = this;
    
    self.Height = 0;
    function CalcHeight() {
      self.Height = self.PaddingV * 2 + self.FontSize;
    }
    self.Defaultable('PaddingH', Parent, PadH);
    self.Defaultable('PaddingV', Parent, PadV, CalcHeight);
    self.Defaultable('FontSize', Parent, FontS, CalcHeight);
    self.Defaultable('BackColor', Parent, BackColor);
    self.Defaultable('ForeColor', Parent, ForeColor);
    self.Defaultable('BorderColor', Parent, BorderColor);
    
    CalcHeight();
  }

  self.DefaultStyles = {
    HeaderStyle: new CellStyle(null, 3, 4, 13, [[0, '#797979'], [0.49, '#4e4e4e'], [0.5, '#333333'], [1, '#0a0a0a']], '#fff', null),
    CellStyle: new CellStyle(null, 3, 4, 13, null, '#000', null),
    GroupByStyle: new CellStyle(null, 3, 4, 13, '#666', '#fff', null),
    HoverOverlayColor: 'rgba(255,255,255,0.15)',
    HeaderIconColor: '#aaa',
    HeaderIconActiveColor: '#fff',
    RowSelectorWidth: 16,
    RowColors: ['#DDF2F7', '#DCF8DD', '#F8F9D9', '#F7EBD7', '#F7DBD7'],
    RowAltColors: ['#DCEBF6', '#E4FAE5', '#F9FBE3', '#FAF2E0', '#FAE4E0'],
    RowBorderColors: ['#B6E4EE', '#B4F0B7', '#EDEFAB', '#F0D9B4', '#F0BAB4']
  }

  var TempBands = [];
  var NextColorIndex = function () {
    //count how many times each color has been used, return the least used color
    for (var i = 0; i < self.DefaultStyles.RowColors.length; i++) {
      var Color = self.DefaultStyles.RowColors[i];
      var UsedCount = 0;
      self.Bands.Iterate(function (Band) {
        if (Band.RowColor == Color) UsedCount += 1;
      });
      TempBands.Iterate(function (Band) {
        if (Band.RowColor == Color) UsedCount += 1;
      });
      if (UsedCount == 0) return i;
    }
 
    return 0;
  }

  //#endregion

  //#region Public Methods

  self.SetData = function (Data, Opt) {
    _RootData.Root = Data;
    SetRowInfo(_RootData);
    _RootData._SGrid.Expanded = true;
    if (!Data || !Data.length) NoData = true;

    //General Settings
    _Opt = Opt || {};
   
    if (_Opt.AllowGroupBy === undefined) _Opt.AllowGroupBy = true;
    if (_Opt.AllowCopy === undefined) _Opt.AllowCopy = true;


  }

  self.LoadLayout = function (Layout, Schema) {

    //first, all the groups need to be removed, and the grid needs to be in its default state.
    SuspendLayout = true;
    var remove = [];
    for (var i = self.Bands.length-1; i >= 0; i--) {
      var band = self.Bands[i];
      if (band.GroupByInfo) {
        band.RemoveGrouping([]);
      }
    }
    self.Bands = [];
    self.Bands.push(new Band(null, 'Root'));
    
    self.Bands[0].FindColumns(_RootData.Root, Schema ? Schema.Properties : null);

    if (Layout) {
      var LObj;
      if (typeof (Layout) == "string") {
        if (Layout.indexOf('"') == -1) {
          //replace | with "
          Layout = Layout.replace(/\|\|/g, String.fromCharCode(29)).replace(/\|/g, '"');
          Layout = Layout.replace(new RegExp(String.fromCharCode(29), 'g'), '|');
        }
        LObj = JSON.parse(Layout);
      } else {
        LObj = Layout;
      }
      self.Bands[0].LoadLayout(LObj.RootBand);
    }
    _MultiBand = self.Bands.length > 1;
    SuspendLayout = false;
    _DoneLayout = false;

    if (_Opt.AfterLayout) _Opt.AfterLayout(self, null, true);

    FlattenAllRows();
    CheckHScroll();
    self.BeginDraw();
  }

  self.SaveLayout = function (AddFilters, ForExport) {
    //AddFilters if the column filters must be included.
    //ForExport if dynamic columns such as crosstabbed columns must be converted to fixed value columns.
    for (var i = 0; i < self.Bands.length; i++) {
      if (!self.Bands[i].GroupByInfo) {
        var lobj = {};
        self.Bands[i].SaveLayout(lobj, AddFilters, ForExport);
        return { RootBand: lobj };
      }
    }

  }

  self.BeginDraw = function () {
    CheckDraw();
  }

  self.GetCurrentData = function () {
    return _RootData[self.Bands[0].Key];
  }

  self.ScrollToRow = function (Row) {
    for (var i = 0; i < RowIndexes.length; i++) {
      var ri = RowIndexes[i];
      if (ri.Row == Row) {
        ExpandRow({Index: i});
        //ri.Row._SGrid.Expanded = true;
        VScroll.SetValues(i);
        return;
      }
    }
  }

  self.AddData = function (NewData) {
    var Current = self.GetCurrentData();
    Current.push.apply(Current, NewData);

    self.Bands[0].InitRowInfo();
    self.Bands.Iterate(function (band) {
      band.ReSortAndFilter(true);
    });
    
    FlattenAllRows();
    CheckDraw();
  }

  //#endregion

  //#region Columns

  var ColDefaults = [
      { Type: 's', FormatString: '', TextAlign: 'left', Width: STRINGWIDTH },
      { Type: 'n', FormatString: '#,##0.00;(#,##0.00)', TextAlign: 'right', Width: NUMBERWIDTH },
      { Type: 'd', FormatString: 'dd MMM yyyy', TextAlign: 'center', Width: DATEWIDTH },
      { Type: 'b', FormatString: '', TextAlign: 'center', Width: NUMBERWIDTH }];

  function Column(band, Key) {

    this._ = { Defaults: { HeaderText: Key } }; //private variables.
    var pvt = this._;
    var thisCol = this;

    this.Key = Key;
    this.HeaderText = Key;
    this.ToolTip = null;
    this.Width = STRINGWIDTH;
    this.Visible = true;
    this.SortDirection = null; // 0=Asc, 1=Desc
    this.Band = band;
    this.Type = 's';
    this.OriginalType = 's';
    this.FormatString = '';
    this.TextAlign = 'left';
    this.Guid = Singular.NewGuid();
    this.Filters = [];
    this.FilterOperator = 1;
    this.HeaderColor = null;
    this.RowColor = null;
    this.HeaderStyle = new CellStyle(band.HeaderStyle);
    this.CellStyle = new CellStyle(band.CellStyle);
    this.CellAltStyle = new CellStyle(this.CellStyle);
    
    pvt.IsID = false;
    pvt.OrigIndex = band.Columns.length;
    pvt.Order = 999; //Order of the column. This is set in loadlayout, new columns must be at end, hence 999.

    //Group by
    this._DataTransformID = 1;
    this._DataTransform = null;
    this._DataTransformParam = null;
    //Summaries
    this._SummariseType = null;//Sum / Avg etc
    this._SummaryFilter = null;
    pvt.CrossTabColumn = null;
    pvt.CrossTabLink = null;//This column will be a normal filter summary linked to a cross tab.
    //Totals
    this.SummaryType = null;
    
    //Formula Column
    this._Formula = null;

    pvt.LastTransformedValue = null;

    //uif this columns data is transformed, e.g. strip off the time. or if its summarised, e.g. sum.
    this.SetGroupInfo = function(tid, st){
      this._DataTransformID = parseInt(tid);
      this._SummariseType = st;
      if (st) {
         
        if (this._SummaryFilter) {
         this.Key = Singular.NewGuid();
        } else {
          this.Key = st == Singular.SGrid.SummaryTypes.Count ? 'Count' : this.Key + '_S' + st;
        }
      } else {
        this.Key = this.Key + '_G' + tid;
        this._DataTransform = Singular.SGrid.GroupTypes.Find('ColType', this.Type).Types.Find('ID', tid);
        if (this._DataTransform.FormatString !== undefined) this.FormatString = this._DataTransform.FormatString;
        if (this._DataTransform.NewType) {
          this.Type = this._DataTransform.NewType;
        }
      }
    }

    //Observable Properties
    this.SetVisible = function(value){
      this.Visible = value;
      CheckHScroll();
    };

    //Drawing vars
    pvt.ShowFilter = false;
    pvt.ShowSort = false;
    pvt.OldShowFilter = false,
	  pvt.OldShowSort = false;
    pvt.ShortenedText = undefined; //Header text that fits in the headers width.
    pvt.WrappedHeaderLines = undefined; //Header text lines if bands col header lines is > 1

    //#region  Methods 

    this._RightPadding = function () {
      return (this.Band.HeaderStyle.PaddingH) + (pvt.ShowFilter || _Opt.AlwaysShowHeaderFilters ? COL_FILTER_WIDTH : 0) + (pvt.ShowSort || _Opt.AlwaysShowHeaderFilters ? COL_SORT_WIDTH : 0);
    }

    pvt.TextXPos = function (Offset, row) {
      var PadH = thisCol.Band.CellStyle.PaddingH;
      var MaxWidth = row ? (thisCol.Width - PadH) : (thisCol.Width - thisCol._RightPadding());
      var TextX = thisCol.TextAlign == 'left' ? Offset + PadH : thisCol.TextAlign == 'right' ? Offset + MaxWidth : Offset + (MaxWidth + PadH) / 2;
      if (pvt.BracketNegatives) {
        //Take bracket width into account.
        if (!row || row[thisCol.Key] >= 0) {
          TextX -= (pvt.BracketWidth - 1);
        } else {
          TextX += 1;
        }
      }
      return TextX;
    };

    this.ResetCachedText = function (VisibleOnly, InclFormat) {
      //when resizing the column, the shortened text needs to be recomputed.
      var col = this;
      this.Band.IterateRows(function (row, i) {
        row._SGrid.TextCache[col.Key] = undefined;
        if (InclFormat) {
          row._SGrid.FormattedCache[col.Key] = undefined;
        }
      }, VisibleOnly);
      pvt.ShortenedText = undefined;
      pvt.WrappedHeaderLines = undefined;
    }

    this.CellTextSized = function (row) {
      //make sure the text fits in the width of the column.
      //cache the shortened value, unless the text doesnt need to be shortened, then cache empty string.
      var CachedValue,
          CellValue,
          MaxWidth = this.Width;

      if (row) {
        CachedValue = row._SGrid.TextCache[this.Key];
        if (!CachedValue) {
          CellValue = row._SGrid.GetDisplayValue(this);
        }
        MaxWidth -= this.Band.CellStyle.PaddingH * 2
      
      } else {
        if (pvt.ShowFilter != pvt.OldShowFilter || pvt.ShowSort != pvt.OldShowSort) {
          pvt.ShortenedText = undefined;
          pvt.WrappedHeaderLines = undefined;
          pvt.OldShowFilter = pvt.ShowFilter;
          pvt.OldShowSort = pvt.ShowSort;
        }

        CellValue = this.HeaderText;
        MaxWidth -= this.Band.HeaderStyle.PaddingH;
        
         if (this.Band._ColHeaderLines > 1) {
           var FirstLineOffset = this.TextAlign == 'left' ? this._RightPadding() : 0; //only left align columns can have full width text from the second line.
           if (!pvt.WrappedHeaderLines) pvt.WrappedHeaderLines = DrawUtils.WrapText(Context, CellValue, MaxWidth - (this._RightPadding() - FirstLineOffset), FirstLineOffset);
          CachedValue = pvt.WrappedHeaderLines;
        } else {
          CachedValue = pvt.ShortenedText;
        }

        MaxWidth -= this._RightPadding();
      
      }
      if (pvt.BracketNegatives) {
        //Take bracket width into account.
        if (!row || row[thisCol.Key] >= 0) {
          MaxWidth -= (pvt.BracketWidth - 1);
        } else {
          MaxWidth += 1;
        }
      }

      if (CachedValue == undefined) {
        CachedValue = DrawUtils.GetSizedText(Context, CellValue, MaxWidth, this.Type == 'n' && row);
        if (CachedValue == CellValue) CachedValue = '';
        if (row) {
          row._SGrid.TextCache[this.Key] = CachedValue;
        } else {
          pvt.ShortenedText = CachedValue;
        }

      }
      return CachedValue == '' ? CellValue : CachedValue;
    }

    this.Sort = function (AlreadySet) {

      if (!AlreadySet) {
        //reset the sort indicator on all the columns except this one.
        for (var i = 0; i < this.Band.Columns.length; i++) {
          if (this.Band.Columns[i] != this) this.Band.Columns[i].SortDirection = null;
        }
        //reverse / initialise the direction.
        this.SortDirection = (this.SortDirection == null ? (this.Type == 'n' ? 1 : 0) : 1 - this.SortDirection);
      }

      var av = this.SortDirection == 0 ? 1 : -1,
          bv = av * -1,
          key = this.Key;

      //Sort comparitor.
      var SortFunction = function (data) {

        if (this.Type == 'd') {
          //use converted value.
          data.sort(function (a, b) {
            return a._SGrid.SortCache[key] == b._SGrid.SortCache[key] ? 0 : (a._SGrid.SortCache[key] > b._SGrid.SortCache[key] ? av : bv);
          });
        } else {
          //user original data value
          data.sort(function (a, b) {
            return a[key] == b[key] ? 0 : (a[key] > b[key] ? av : bv);
          });
        }
      }

      //Sort this list in every parent object.
      var data;
      if (this.Band.ParentBand == null) {
        SortFunction.call(this, _RootData[band.Key]);
      } else {
        var col = this;
        band.ParentBand.IterateRows(function (row) {
          SortFunction.call(col, row[band.Key]);
        });
      }

      //reset the flattened row list.
      FlattenAllRows();
    }

    this.SetDefaults = function (Value, Schema) {

      if (Schema) {
        if (Schema.Display) {
          this.HeaderText = Schema.Display;
          pvt.Defaults.HeaderText = Schema.Display;
        }
        if (Schema.Description) this.ToolTip = Schema.Description;
        if (Schema.Type) {
          this.Type = Schema.Type
          if (this.Type == 'd' && !(Value instanceof Date)) {
            this.OriginalType = 's';
          } else {
            this.OriginalType = this.Type;
          }
        }
      } else {
        //Predict based on Value.

        //Number
        if (typeof Value == 'number') {
          this.Type = 'n';
          this.OriginalType = 'n';
        }
        if (typeof Value == 'boolean') {
          this.Type = 'b';
          this.OriginalType = 'b';
        }
        //Check if its a date, or can parse to a date.
        if (this.Type == 's') {
          if (Value instanceof Date) {
            this.Type = 'd';
            this.OriginalType = 'd';
          } else if (Value) {
            //if its a string that looks like a date.
            //remove all numbers and see if there is just a month name left.
            var ds = Value.replace(/[0-9]/g, '').trim();
            if (dateFormat.i18n.monthNames.indexOf(ds) >= 0 && !isNaN(new Date(Value))) {
              this.Type = 'd';
            }

          }
        }
      }
      if (this.Type == 'n') {
        pvt.IsID = this.Key.indexOf('ID') == this.Key.length - 2;
        this.SummaryType = pvt.IsID ? null : _Opt.DefaultSummaryType;
      }
      var cd = ColDefaults.Find('Type', this.Type);
     
      pvt.Defaults.FormatString = cd.FormatString;
      pvt.Defaults.TextAlign = cd.TextAlign;
      pvt.Defaults.Width = cd.Width;
      this.FormatString = cd.FormatString;
      this.TextAlign = cd.TextAlign;
      this.Width = cd.Width;

      if (Schema && Schema.Format) {
        this.FormatString = Schema.Format;
        pvt.Defaults.FormatString = Schema.Format;
      }
    }

    this.CopyTo = function (ToCol) {
      ToCol.Type = this.Type;
      ToCol.OriginalType = this.OriginalType;
      ToCol.HeaderText = this.HeaderText;
      ToCol.FormatString = this.FormatString;
      ToCol.Width = this.Width;
      ToCol.TextAlign = this.TextAlign;
      ToCol._.Defaults = this._.Defaults;
      ToCol.SummaryType = this.SummaryType;
    }

    this.CreateGroupByBand = function (ChildBand) {

      var ColList = ChildBand ? ChildBand._SummaryColumns : this.Band.Columns;

      //Create the new band.
      var newBand = new Band(this.Band.ParentBand, '_G_' + this.Band.Key);
      newBand.GroupByInfo = new GroupByInfo(this.Band, this.HeaderText);
      
      //Group by column
      var GroupCol = Column.CreateGroupColumn(newBand, this)
            
      //Count
      Column.CreateSummaryColumn(Singular.SGrid.SummaryTypes.Count, newBand, ColList[0]);

      //Create a summed column for each numeric column 
      for (var i = 0; i < ColList.length; i++) {
        var col = ColList[i];
        if (col.Visible && col != this && col.Type == 'n' && !col._.IsID) {
          Column.CreateSummaryColumn(_Opt.DefaultSummaryType, newBand, col);
        }
      }

      return newBand;
    }

    this.GetTransformedValue = function (row) {
      pvt.LastTransformedValue = this.OrigCol.Type == this.OrigCol.OriginalType ? row[this.OrigCol.Key] : row._SGrid.ValueCache[this.OrigCol.Key];
      if (this._DataTransformID > 1) {
        pvt.LastTransformedValue = this._DataTransform.GetValue(pvt.LastTransformedValue, this._DataTransformParam);
      }
      return pvt.LastTransformedValue;
    }
      
    this.ShowColumnInfo = function (columnElement) {

      Singular.SGrid.CurrentColumnInfo(new Singular.SGrid.ColumnInfo(this, columnElement.Index));
      Singular.SGrid.CurrentColumnInfo().Filters(this.Filters);
      
      var dlg = $('[data-sgrid="ColInfo"]'), canv = $(Canvas);
      var x = Math.max(columnElement.Right - HScroll.Value() - dlg.outerWidth(), 0),
          y = columnElement.Grid.Y + columnElement.Column.Band.HeaderStyle.Height;

      var position = $(Canvas).offset();
      y = position.top + y + 1;
      if (y + dlg.outerHeight() > $(window).height()) y = $(window).height() - dlg.outerHeight();

      dlg.css({ left: position.left + x + 'px', top: y + 'px' });

    }
      
    this.GetFilterValue = function (Row) {
      if (this.Type == 'd') {
        return Row._SGrid.SortCache[this.Key];
      } else {
        if (this.Type == 's') {
          var Value = Row[this.Key];
          return Value == null ? '' : Value.toLowerCase();
        } else {
          return Row._SGrid.GetValue(this);
        }
      }
    }

    this.ApplyFilters = function (fi, SuspendLayout) {

      if (fi) {
        //get new filters
        this.Filters = fi.Filters();
        this.FilterOperator = fi.FilterOperator();
      }
      
      //filter each row.
      var MatchCount = 0, FilterCount = thisCol.Filters.length, Value;

      if (_Opt.OnFilter) {
        var HasFilters = false;
        self.Bands.Iterate(function (band) { band.Columns.Iterate(function (col) { if (col.Filters.length > 0) HasFilters = true }) });
        _Opt.OnFilter(HasFilters, thisCol);
      }
            
      //TODO, check if filters have changed since last filter, if not, then skip this.
      this.Band.IterateRows(function (row, parentRow) {
        if (FilterCount > 0) {
          MatchCount = 0;
          Value = thisCol.GetFilterValue(row);

          for (var i = 0; i < FilterCount; i++) {
            var Filter = thisCol.Filters[i];
            if (Filter.MatchFunction(Value, Filter.FValue)) MatchCount += 1;
          }

          if (thisCol.FilterOperator == 1 ? MatchCount < FilterCount : MatchCount == 0) {
            row._SGrid.Filters[thisCol.Key] = true;
          } else {
            delete row._SGrid.Filters[thisCol.Key];
          }
          row._SGrid.SelfFiltered = Object.keys(row._SGrid.Filters).length > 0;
        } else {
          delete row._SGrid.Filters[thisCol.Key];
          row._SGrid.SelfFiltered = Object.keys(row._SGrid.Filters).length > 0;
        }
      });

      if (!SuspendLayout) {
        this.Band.ResetTotals();
        if (this.Band.ParentBand) this.Band.ParentBand.RefilterGrouping();

        FlattenAllRows();
        Draw();
      }
      
    }

    this.Remove = function(Process) {
      this._Removing = false;
      
      if (this._DataTransform) {
        //Change the grouping
        this.Band._GroupColumns.RemoveItem(this);
        this.Band.Columns.RemoveItem(this);
        if (Process) {
          if (this.Band._GroupColumns.length > 0) {
            //there is still a group column, so refresh grouping
            this.Band.RefreshGrouping(null, true);
          } else {
            //no more group columns, so remove band.
            var ReAdd = [];
            this.Band.RemoveGrouping(ReAdd);
            this.Band.GroupByInfo.DataBand.CreateGroupings(ReAdd);
          }
        }
      } else if (this._SummariseType) {
        this.Band._SummaryColumns.RemoveItem(this);
        this.Band.Columns.RemoveItem(this);
        if (Process) {
          this.Band.RefilterGrouping();
        }
      } else {
        this.SetVisible(false);
      }
      CheckHScroll();

    }

    this._SaveLayout = function (obj, AddFilters) {
      obj.Key = this.Key
      if (pvt.Defaults.HeaderText != this.HeaderText || this.Band.GroupByInfo) obj.HeaderText = this.HeaderText;
      if (pvt.Defaults.FormatString != this.FormatString) obj.FormatString = this.FormatString;
      if (pvt.Defaults.TextAlign != this.TextAlign) obj.TextAlign = this.TextAlign;
      if (pvt.Defaults.Width != this.Width) obj.Width = this.Width;
      if (this.SortDirection != null) obj.SortDirection = this.SortDirection;
      if (this.Visible === false) obj.Visible = this.Visible;
      if (this.Band.GroupByInfo) {
        if (this._SummariseType) {
          obj.SummaryType = this._SummariseType;
          if (this._SummaryFilter) {
            obj.FKey = this._SummaryFilter.Column.Key;
            obj.FType = this._SummaryFilter.FilterType();
            obj.FValue = this._SummaryFilter.FilterValue();

          } else if (this._.CrossTabColumn) {
            obj.CTKey = this._.CrossTabColumn.Key;
          }
        } else if (this._DataTransformID) {
          obj.TransformID = this._DataTransformID;
          obj.TransformParam = this._DataTransformParam;
        }
      } else {
        if (this.SummaryType && this.Type == 'n') {
          if (pvt.IsID || this.SummaryType != Singular.SGrid.SummaryTypes.Sum) obj.SummaryType = this.SummaryType;
        }
      }
      if (this.OrigCol) {
        obj.OrigKey = this.OrigCol.Key;
      }
      if (AddFilters && this.Filters.length > 0) {
        obj.FilterOperator = this.FilterOperator;
        obj.Filters = this.Filters.map(function(item){
          return { Type: item.FilterType.peek(), Value: item.FValue };
        });
      }

    }
    this._LoadLayout = function (obj, NoSort) {

      if (obj.TransformID) {
        this.SetGroupInfo(obj.TransformID);
        this._DataTransformParam = obj.TransformParam;
      }
      if (obj.SummaryType) this.SummaryType = obj.SummaryType;

      for (var prop in obj) {
        if (this.hasOwnProperty(prop)) {
          this[prop] = obj[prop];
        }
      }
      if (!NoSort && this.SortDirection != undefined) {
        this.Sort(true);
      }
      var NewFilters = [];
      this.Filters.forEach(function (val) {
        NewFilters.push(new Singular.SGrid.FilterInfo(thisCol, val.Type, val.Value));        
      });
      this.Filters = NewFilters;
      if (this.Filters.length > 0) this.ApplyFilters(null, true);
    }

    //#endregion
  }
  Column.CreateSummaryColumn = function (sumType, band, origCol, filter) {
    var newCol = new Column(band, origCol ? origCol.Key : '');
    newCol.OrigCol = origCol;

    var st = Singular.SGrid.SummaryTypes.Items[sumType];

    if (sumType != Singular.SGrid.SummaryTypes.Count) {
      origCol.CopyTo(newCol);

      var DataCol = origCol;
      while (DataCol.OrigCol) DataCol = DataCol.OrigCol;
      newCol.Key = DataCol.Key; //prevent Amount_S1_S1
      if (st.Type == 'n') {
        //distinct count must not inherit the original cols type.
        newCol.Type = st.Type;
        newCol.TextAlign = 'right';
        newCol.Width = NUMBERWIDTH;
      }
      
      newCol.OriginalType = newCol.Type;
      newCol._DataKey = DataCol.Key;
      if (origCol.HeaderText.indexOf(st.Text) == 0) {
        //prevent sum of sum of
        newCol.HeaderText = origCol.HeaderText;
      } else {
        newCol.HeaderText = st.Text + ' ' + origCol.HeaderText;
      }
      
    } else {
      //Count
      newCol.Key = 'Count';
      newCol.SetDefaults(0);
      newCol.FormatString = "#,##0";
      newCol.Width = 70;
      newCol.HeaderText = 'Count';
      
    }
    newCol._SummaryFilter = filter;
    newCol.Calc = st.Calc;
    newCol.SetGroupInfo(1, sumType);
	  
    band.AddColumn(newCol);
    return newCol;
  }
  Column.CreateGroupColumn = function (newBand, origCol, transform) {
    var GroupCol = new Column(newBand, origCol.Key);
    origCol.CopyTo(GroupCol);
    var DataCol = origCol;
    while (DataCol.OrigCol) DataCol = DataCol.OrigCol;
    GroupCol._DataKey = DataCol.Key;
    GroupCol.OriginalType = origCol.Type; //Group cols must always have the correct type
    GroupCol.SetGroupInfo(transform ? transform : 1);//Value transform.
    GroupCol.OrigCol = origCol;
    newBand.AddColumn(GroupCol);
    return GroupCol;
  }
  Column.CreateCrossTabColumn = function (sumType, band, origCol, ctCol) {

    var AddedValues = {},
        AddedCount = 0;

    //Create the master cross tab column;
    var XCol = new Column.CreateSummaryColumn(sumType, band, origCol);
    XCol._.CrossTabColumn = ctCol;
    XCol.Visible = false;

    band.ChildBands[0].IterateRows(function (row) {
      var Value = row._SGrid.GetValue(ctCol);
      if (AddedCount < 15 && !AddedValues[Value]) {
        AddedCount += 1;
        AddedValues[Value] = true;
        var SCol = Column.CreateSummaryColumn(Singular.SGrid.SummaryTypes.Sum, band, origCol, new Singular.SGrid.FilterInfo(ctCol, 1, Value));
        SCol.HeaderText = row._SGrid.GetDisplayValue(ctCol) + ' ' + origCol.HeaderText;
        SCol._.CrossTabLink = XCol;
      }
    });
    return XCol;
  }

  //#endregion

  //#region Rows
  
  var RowInfo = function (Row) {
    this.TextCache = {};
    this.ValueCache = {};
    this.SortCache = {};
    this.FormattedCache = {};
    this.Filters = {};
    this.SelfFiltered = false;
    this.ChildrenFiltered = false;
    this.Row = Row;
  }
  RowInfo.prototype.GetValue = function (Column) {
    if (Column.Type == Column.OriginalType) {
      return this.Row[Column.Key];
    } else {
      return this.ValueCache[Column.Key];
    }
  }
  RowInfo.prototype.GetDisplayValue = function (Column) {
  
    var CellValue = this.GetValue(Column);
    if (CellValue == null) CellValue = '';

    if (Column.FormatString != '') {

      var FormattedValue = this.FormattedCache[Column.Key];
      if (FormattedValue == undefined && CellValue != undefined) {
        if (CellValue === '') {
          FormattedValue = '';
        } else if (Column.Type == 'n') {
          FormattedValue = CellValue.formatDotNet(Column.FormatString);
        } else if (Column.Type == 'd') {
          FormattedValue = CellValue.format(Column.FormatString);
        }
        this.FormattedCache[Column.Key] = FormattedValue;
      }
      CellValue = FormattedValue;
    }
    return CellValue;
  };
  RowInfo.prototype.SetValue = function (Column, CellValue) {
    this.Row[Column.Key] = CellValue;
    this.FormattedCache[Column.Key] = undefined;
    this.SetCachedValues(Column, CellValue);
  }
  RowInfo.prototype.SetCachedValues = function (Column, CellValue) {
    if (Column.Type == 'd') {
      if (Column.OriginalType == 's') {
        if (CellValue != null) CellValue = new Date(CellValue);
        this.ValueCache[Column.Key] = CellValue;
      }
      this.SortCache[Column.Key] = CellValue ? CellValue.getTime() : 0;   
    }
  }
  RowInfo.prototype.IsFiltered = function () {
    return this.SelfFiltered || this.ChildrenFiltered;
  }

  var SetRowInfo = function (row, reset) {
    if (!row._SGrid) row._SGrid = new RowInfo(row);
    if (reset) {
      row._SGrid.TextCache = {}; //Cached shortened text if the text is too large to fit the column.
      row._SGrid.ValueCache = {}; //Cached date value where the json data was converted from string to date.
      row._SGrid.SortCache = {}; //Cached sort value where the sort value is different from the display value.
      row._SGrid.FormattedCache = {}; //Cached formatted string
      row._SGrid.BackColor = undefined;
    }
  }

  function FlattenAllRows() {
    if (!SuspendLayout) {
      RowIndexes = [];
      FlattenRows({ Row: _RootData, Band: { ChildBands: [self.Bands[0]] } }, RowIndexes);
      FindMaxRowIndexes();
      CalcBounds = true;
    }
  }

  function FindMaxRowIndexes() {
    for (var i = 0; i < self.Bands.length; i++) {
      self.Bands[i].FindMaxRowIndex();
    }
    FindScrollHeight();
  }

  function FindScrollHeight() {
    //Find the last rox index where the rest of the rows will fill the grid height.
    var LastIndex = RowIndexes.length - 1,
        LastBand,
        Height = GridRect.Height - 21 - (TooWide ? HScroll.Thickness() : 0),/*Fix - Header height of top row*/
        i = LastIndex;;
    do {
      if (LastBand && LastBand != RowIndexes[i].Band) {
        Height -= (LastBand.HeaderStyle.Height + GRID_GAP);
      }
      LastBand = RowIndexes[i].Band;
      if (RowIndexes[i].LastChild) {
        Height -= 3; //Totals row gap.
      }
      LastIndex -= 1;
      Height -= LastBand.CellStyle.Height;
      i--;
    } while (i >= 0 && Height - LastBand.CellStyle.Height - 5 >= 0);

    VScroll.SetValues(VScroll.Value(), LastIndex + VScroll.LargeChange());
  }

  function FlattenRows(ri, FlattenedList) {
    //Create the RowIndexes array. 
    var Band = ri.Band, Row = ri.Row;

    for (var i = 0; i < Band.ChildBands.length; i++) {
      var cb = Band.ChildBands[i],
			    list = Row[cb.Key];
      for (var j = 0; j < list.length; j++) {
        if (!list[j]._SGrid.IsFiltered()) {
          var NewRi = { Row: list[j], Band: cb, Parent: Row, ParentRi: ri }
          FlattenedList.push(NewRi);

          if (list[j]._SGrid.Expanded) {
            FlattenRows(NewRi, FlattenedList);
          }
        }
      }

      //Fake Footer Row
      FlattenedList.push({ Row: null, Band: cb, LastChild: true, Parent: Row });
    }
    
  }

  function ExpandRow(Expander) {
    var ri = RowIndexes[Expander.Index];

    ri.Row._SGrid.Expanded = !ri.Row._SGrid.Expanded
    if (ri.Row._SGrid.Expanded) {
      //Put the child and grandchild rows in a flat array
      var NewRows = [];
      FlattenRows(ri, NewRows);
      RowIndexes.insert(Expander.Index + 1, NewRows);
    } else {
      //check how many rows are currently expanded.
      var expCount = 0;
      for (var i = Expander.Index+1; i < RowIndexes.length; i++) {
        if (RowIndexes[i].Band == ri.Band) break;
        expCount += 1;
      }
      RowIndexes.splice(Expander.Index + 1, expCount);
    }
    if (_CellSelection && _CellSelection.End.RowIndex > Expander.Index) ResetCellSelection();

    FindMaxRowIndexes();
    CalcBounds = true;
    CheckDraw();
  }

  //#endregion

  //#region Editing

  var _CellEditInfo,
      _DoCellEditLayout = false;

  var NextCell = function (Backwards) {
    var NextCol = function (Columns, RefCol, Backwards) {
      var Found = RefCol == null, NewCol;
      var Func = Backwards ? Columns.IterateR : Columns.Iterate;
      Func.call(Columns, function (Col, i) {
        if (RefCol == Col) {
          Found = true;
        } else if (Found && !NewCol) {
          if (Col.Visible && CellEditAllowed(Col)) NewCol = Col;
        }
      });
      return NewCol;
    }
    //Get the next column
    var NewCol = NextCol(_CellEditInfo.Column.Band.Columns, _CellEditInfo.Column, Backwards);
    if (NewCol) {
      _CellEditInfo.Column = NewCol;
    } else {
      //Go to next row.
      var ri;
      do {
        _CellEditInfo.RowIndex += Backwards ? -1 : 1;
        ri = RowIndexes[_CellEditInfo.RowIndex];
        _CellEditInfo.Row = ri ? ri.Row : null;
      } while (ri && (!_CellEditInfo.Row || _CellEditInfo.Row._SGrid.IsFiltered()));
      if (ri) _CellEditInfo.Column = NextCol(ri.Band.Columns, null, Backwards);
    }
  }

  var EditorValueChanged = function (e) {
    var cmi = _CellEditInfo;
    cmi.Band = cmi.Grid.Band;

    cmi.Value = e.target.value;
        
    if (cmi.Column.Type == 'n') cmi.Value = parseFloat(cmi.Value);
    if (cmi.Column.Type == 'd') cmi.Value = Date.parse(cmi.Value);
    if (cmi.Column.Type == 'b') cmi.Value = cmi.Value == 'true';
    cmi.Row[cmi.Column.Key] = cmi.Value;

    if (_Opt.AfterCellEdit) {
      _Opt.AfterCellEdit(cmi);
    }
    
    self.ResetRow(cmi);
  }

  var EditorKeyDown = function (e) {
    if (e.keyCode == 9) {
      //tab
      if (_Editor._OldValue != _Editor.value) EditorValueChanged(e);
      NextCell(e.shiftKey); 
      if (HandleCellEdit()) {
        _DoCellEditLayout = true;
      } 
      CheckDraw();

      e.preventDefault();
      return false;
    }
  }

  var CellEditAllowed = function (Col) {
    var cmi = _CellEditInfo,
        Args = { Row: cmi.Row, Column: Col ? Col : cmi.Column, Value: cmi.Row._SGrid.GetValue(cmi.Column), Cancel: false };
    if (_Opt.OnCellEdit) {
      _Opt.OnCellEdit(Args);
    }
    return !Args.Cancel;
  }

  var HandleCellEdit = function (SetPosition) {

    if (!_CellEditInfo.Row){
      _Editor.style.display = 'none';
      return false;
    }

    var cmi = _CellEditInfo;
    if (!CellEditAllowed()) {
      _CellEditInfo = null;
      if (_Editor) _Editor.style.display = 'none';
      return false;
    } else {
      if (!_Editor) {
        _Editor = document.createElement('input');
        _Editor.setAttribute('type', 'text');
        _Editor.classList.add('sg-CellEdit');
        _Editor.style.transition = 'none';
        Canvas.parentElement.appendChild(_Editor);
        _Editor.addEventListener('change', EditorValueChanged);
        _Editor.addEventListener('keydown', EditorKeyDown);
      }
      var Value = cmi.Row._SGrid.GetValue(cmi.Column);
      _Editor.value = Value;
      _Editor._OldValue = _Editor.value;
      if (cmi.Column.Type == 'd') {
        $(_Editor).datepicker();
        $(_Editor).datepicker('setDate', Value);
      } else {
        $(_Editor).datepicker('destroy');
      }
      if (SetPosition) PositionCellEditor(cmi.Column._.Left, cmi.RowTop, cmi.Grid);
      return true;
    }

  }
  
  var PositionCellEditor = function (x, y, grid) {
    _DoCellEditLayout = false;
    if (y < 0 || y + grid.RowHeight + 1 > (TooWide ? HScrollY : GridRect.Bottom())) {
      _Editor.style.display = 'none';
    } else if (x + _CellEditInfo.Column.Width + 1 > VScrollX) {
      _Editor.style.display = 'none';
    } else {
      var st = _Editor.style;
      st.left = x - 1 + 'px';
      st.top = y - 1 + 'px';
      st.height = grid.RowHeight + 1 + 'px';
      st.width = _CellEditInfo.Column.Width + 1 + 'px';
      st.textAlign = _CellEditInfo.Column.TextAlign;
      st.boxShadow = 'inset 0 0px 5px ' + grid.Band.RowBorderColor;
      st.display = '';

      var c = DrawUtils.HexToHSL(grid.Band.RowBorderColor);
      c.s = c.s * 0.5;
      c.l = c.l * 0.5;
      _Editor.style.borderColor = c.GetCSSColor();
      setTimeout(function () { _Editor.focus(); }, 0);
      _Editor.select();
    }
  }

  self.ResetRow = function (RowInfo, ResetChildren) {
    SetRowInfo(RowInfo.Row, true);

    RowInfo.Band.Columns.Iterate(function (Item) {
      if (Item.Type == 'd') RowInfo.Row._SGrid.SetCachedValues(Item, RowInfo.Row[Item.Key]);
    });

    if (ResetChildren) {
      RowInfo.Band.ChildBands.Iterate(function (Band) {
        Band.InitRowInfo(true);
        Band.ResetTotals();
      });
    }
    
    if (RowInfo.Row._SGrid.Expanded) {
      ExpandRow({ Index: RowInfo.RowIndex });
      ExpandRow({ Index: RowInfo.RowIndex });
    }
    
    //Clear totals, and re-summarise
    if (RowInfo.Band.ParentBand && RowInfo.RowIndex) RowInfo.Band.ParentBand.RefilterGrouping(false, RowIndexes[RowInfo.RowIndex].ParentRi);

    RowInfo.Band.ResetTotals();
    CheckDraw();
  }

  //#endregion

  //#region Cell Selection / Copying

  self.GetSelectedRows = function (SingleOnly) {
    var Rows = [];
    if (_CellSelection) {
      for (var i = _CellSelection.Start.RowIndex; i <= _CellSelection.End.RowIndex; i++) {
        if (RowIndexes[i].Row && RowIndexes[i].Band == _CellSelection.Grid.Band) {
          RowIndexes[i].RowIndex = i;
          Rows.push(RowIndexes[i]);
        }
      }
    } else if (_CellEditInfo) {
      return [RowIndexes[_CellEditInfo.RowIndex]];
    }
    if (SingleOnly && Rows.length > 0) {
      SelectRowUpDown(Rows[0], 0);//select row if only cell selected.
      return Rows;
    }
    return Rows;
   
  }
  self.GetSelectedRow = function () {
    return self.GetSelectedRows(true)[0];
  }

  var SelectRowUpDown = function(RowInfo, Delta){
    var idx = RowInfo.RowIndex + Delta;
    while (true) {
      if (Delta > 0 && idx >= RowIndexes.length) idx = 0;
      if (Delta < 0 && idx < 0) idx = RowIndexes.length - 1;
      if (RowIndexes[idx].Row && RowIndexes[idx].Band == RowInfo.Band) break;
      idx += Delta;
    }
    _CellSelection = {
      Grid: { Band: RowInfo.Band },
      Start: { RowIndex: idx, ColIndex: 0 },
      End: { RowIndex: idx, ColIndex: 999 }
    };
    self.ReDraw();
    return self.GetSelectedRows()[0];
  };

  self.SelectNextRow = function(RowInfo){
    return SelectRowUpDown(RowInfo, 1);
  }
  self.SelectPrevRow = function (RowInfo) {
    return SelectRowUpDown(RowInfo, -1);
  }
  
  var ResetCellSelection = function () {
    _CellSelection = null;
    _CellSelectStart = null;
    _CellSelectEnd = null;
  }
  var FixCellSelection = function (Type, IsStart) {
    if (_CellSelectStart) {
      _CellSelection = {
        Grid: _CellSelectStart.Grid,
        Start: { RowIndex: Math.min(_CellSelectStart.RowIndex, _CellSelectEnd.RowIndex) },
        End: { RowIndex: Math.max(_CellSelectStart.RowIndex, _CellSelectEnd.RowIndex) }
      };
     
      if (Type == ElementType.RowSelector || _CellSelectStart.RowsOnly) {
        _CellSelection.Start.ColIndex = 0;
        _CellSelection.End.ColIndex = 999;
      } else {
        _CellSelection.Start.ColIndex = Math.min(_CellSelectStart.ColIndex, _CellSelectEnd.ColIndex);
        _CellSelection.End.ColIndex = Math.max(_CellSelectStart.ColIndex, _CellSelectEnd.ColIndex);
      }
    }
    
  }
  var InCellSelection = function (Cell) {
    return _CellSelection && Cell && Cell.Grid && Cell.Grid.Band == _CellSelection.Grid.Band &&
      Cell.RowIndex >= _CellSelection.Start.RowIndex && Cell.RowIndex <= _CellSelection.End.RowIndex &&
      (Cell.ColIndex == undefined || (Cell.ColIndex >= _CellSelection.Start.ColIndex && Cell.ColIndex <= _CellSelection.End.ColIndex))
  }
  var GetCopyText = function () {
    var CopyText = '',
        Columns = _CellSelection.Grid.Band.Columns,
        FirstCol = true;

    //Headers
    if (_CellSelection.Start.ColIndex != _CellSelection.End.ColIndex) {
      for (var j = 0; j < Columns.length; j++) {
        var Col = Columns[j];
        if (Col.Visible && j >= _CellSelection.Start.ColIndex && j <= _CellSelection.End.ColIndex) {
          CopyText += (FirstCol ? '' : '\t') + Col.HeaderText;
          FirstCol = false;
        }
      }
      CopyText += '\n';
    }

    for (var i = _CellSelection.Start.RowIndex; i <= _CellSelection.End.RowIndex; i++) {
      if (RowIndexes[i].Row && RowIndexes[i].Band == _CellSelection.Grid.Band) {
        FirstCol = true;
        CopyText += (i == _CellSelection.Start.RowIndex ? '' : '\n');
        for (var j = 0; j < Columns.length; j++) {
          var Col = Columns[j];
          if (Col.Visible && j >= _CellSelection.Start.ColIndex && j <= _CellSelection.End.ColIndex) {
            var Value = Col.Type == 'd' ? RowIndexes[i].Row._SGrid.GetDisplayValue(Col) : RowIndexes[i].Row._SGrid.GetValue(Col); //Excel cant parse a js date, so export the formatted value.
            CopyText += (FirstCol ? '' : '\t') + (Value !== null ? Value.toString() : '');
            FirstCol = false;
          }
        }
      }
    }
    return CopyText;
  }
  var CellCopy = function (FromMenu) {
    if (window.clipboardData) {
      //only IE supports this.
      clipboardData.setData('text', GetCopyText());
    } else if (!FromMenu) {
      //hack for other browsers.
      var txt = document.createElement('textarea');
      txt.style.position = 'absolute';
      txt.style.left = '-1000px';
      txt.style.top = '100px';
      document.body.appendChild(txt);
      txt.value = GetCopyText();
      txt.focus();
      txt.select();
      setTimeout(function () {
        document.body.removeChild(txt);
      }, 100)
    } else {
      alert('Your browser does not allow copying to the clipboard. Please select the cells to copy, and press ctrl + c.');
    }
  }

  //#endregion

  //#region Context Menu

  //Shows the context menu for this column.
  var ShowMenu = function (elem, x, y) {

    var Items = [],
        Col = elem.Column;

    if (elem.Type == ElementType.Column || elem.Type == ElementType.Cell) {

      if (elem.Type == ElementType.Column) {
        var isFrozen = elem.Index == Col.Band.FrozenColumn;
        Items.push({
          Text: 'Hide Column', Click: function () {
            Col.SetVisible(false);
          }
        });
        if (TooWide) {
          Items.push({
            Text: isFrozen ? 'Un-Freeze' : 'Freeze' + ' Column', Click: function () {
              Col.Band.FrozenColumn = isFrozen ? -1 : elem.Index;
              CheckDraw();
            }
          });
        }
      }

      //Remove current filters.
      for (var i = 0; i < Col.Filters.length; i++) {
        var TypeName = Singular.SGrid.FilterTypes.FindValue('ID', Col.Filters[i].FilterType.peek(), 'Text').Value;
        Items.push({
          Text: 'Remove Filter: ' + TypeName + (Col.Type == 'b' ? '' : (" <b>'" + Col.Filters[i].DisplayValue() + "'</b>")),
          _Filter: Col.Filters[i],
          Click: function (Item) {
            Col.Filters.splice(Col.Filters.indexOf(Item._Filter), 1);
            Col.ApplyFilters();
          }
        });
      }

      if (elem.Type == ElementType.Cell) {

        //Add filters
        Items.push({ Text: 'Add Filter', Items: Singular.SGrid.ColumnInfo.GetFiltersForType(Col.Type), Icon: [2, 2] });

        //Cross tab / pivot
        if (Col.Band.ParentBand && Col.Band.ParentBand.GroupByInfo) {
          var CTItem = { Text: 'Cross Tab on', Icon: 'fa fa-random', Items: [] };
          Col.Band.Columns.Iterate(function (CTCol) {
            if (CTCol.Type == 'n' && CTCol != Col) CTItem.Items.push({ Key: CTCol.Key, Text: CTCol.HeaderText });
          })
          if (CTItem.Items.length) Items.push(CTItem)
        }

        Singular.SGrid.CurrentColumnInfo.AddingFilter = new Singular.SGrid.FilterInfo(Col, null, elem.Row._SGrid.GetValue(Col));

      }

    }
    
    if (elem.Type == ElementType.Cell || elem.Type == ElementType.RowSelector) {
      
      //Expand / Collapse
      var ExpandCollapseAll = function (Expand) {
        elem.Grid.Band.IterateRows(function (row) {
          if (row._SGrid.Expanded !== undefined && row._SGrid.Expanded !== null) {
            row._SGrid.Expanded = Expand;
          }
        });
        FlattenAllRows();
        CheckDraw();
      };

      if (elem.Grid.Band.ChildBands.length > 0) {
        Items.push({ Text: 'Expand All', Icon: [1, 0], Click: function () { ExpandCollapseAll(true); } });
        Items.push({ Text: 'Collapse All', Icon: [0, 0], Click: function () { ExpandCollapseAll(false); } });
      }

      if (_CellSelection && _Opt.AllowCopy) {
        //Copy
        var OneRow = _CellSelection.Start.RowIndex == _CellSelection.End.RowIndex,
            OneCol = _CellSelection.Start.ColIndex == _CellSelection.End.ColIndex;

        Items.push({
          Text: 'Copy ' + (elem.Type == ElementType.Cell ? ((OneRow && OneCol) ? 'cell' : 'cells') : (OneRow ? 'row' : 'rows')), Icon: 'fa fa-copy', Click: function () {
            CellCopy(true);
          }
        });
      }

    }

    if (_Opt.ContextMenu) _Opt.ContextMenu(elem, Items);
    
    if (Items.length > 0) {
      Singular.ContextMenu.Show(Items, x, y, function (Item, Args) {

        if (Args[1].target.nodeName != 'INPUT' && (Args.length > 2 || Item.Click || Item.ID || Item.Key)) {
          Singular.ContextMenu.Hide();
          if (Item.Click) {
            Item.Click(Item);
          } else if (Item.ID) {
            //Filter
            Singular.SGrid.CurrentColumnInfo.AddingFilter.FilterType(Item.ID);
            Col.Filters.push(Singular.SGrid.CurrentColumnInfo.AddingFilter);
            Col.ApplyFilters();
          } else if (Item.Key) {
            //Cross tab
            Column.CreateCrossTabColumn(Singular.SGrid.SummaryTypes.Sum, Col.Band.ParentBand, Col.Band.Columns.Find('Key', Item.Key), Col);
            Col.Band.ParentBand.RefilterGrouping(false);
            CheckHScroll();
          }
        }
      });
    }

  }


  //#endregion

  //#region Utils

  //Adds a sub control offset to the mouse event.
  var CtlMouse = function (e, x, y) {
    e.ctlX = e.ctlXRoot - x;
    e.ctlY = e.ctlYRoot - y;
  }

  //#endregion

  //#region Mouse / Keyboard

  //#region Events

  var _MouseDown = false,
      _RightMouseDown = false,
      _MovedThreshold = false,
      _MouseDownTime,
      _CellSelectStart,
      _CellSelectEnd,
      _CellSelection,
      _LastYPos,
      _FlickScrollerTimer,
      _FlickVelocity = 0;

  var _OnMouseDown = function (e) {
    //_TouchedInCanvas = true; //for testing.
    if (_FlickScrollerTimer) {
      clearInterval(_FlickScrollerTimer);
      _FlickScrollerTimer = null;
    }
    DrawUtils.FixEvent(e, PositionOffset);

    if (e.button == 0 || e.touches) {

      if (e.ctlX >= VScrollX) {
        //Vertical Scroll
        CtlMouse(e, VScrollX, 0);
        VScroll.MouseDown(e);
        IsScrolling = true;
        StartAnimating();
      } else if (e.ctlY >= HScrollY) {
        CtlMouse(e, 0, HScrollY);
        HScroll.MouseDown(e);
        IsScrolling = true;
        StartAnimating();
      } else {
        if (_Editor) {
          _Editor.blur();
        _Editor.style.display = 'none';
        }
        var cmi = GetElement(e);
        if (cmi) {
          if (cmi.Type == ElementType.Column || cmi.Type == ElementType.GroupBy) {

            //remember that user clicked on a column.
            ElemClicked = cmi;
            StartAnimating();
          } else if (cmi.Type == ElementType.GridSelector) {
            ElemClicked = cmi;
          } else if (cmi.Type == ElementType.RowExpansion) {

            //Expand immediately.
            ExpandRow(cmi.Expander);
          } else if (cmi.Type == ElementType.Cell || cmi.Type == ElementType.RowSelector) {

            if (_TouchedInCanvas) {
              //Start finger scrolling
              ElemClicked = cmi;
              ElemClicked.ScrollPos = VScroll.Value();
              StartAnimating();
            } else {
              //Mouse
              var IsEditing = false;
              if (cmi.Type == ElementType.Cell && _Opt.AllowEdit) {
                //Editing
                _CellEditInfo = cmi;
                IsEditing = HandleCellEdit(true);

              }
              if (IsEditing) {
                ResetCellSelection();
              } else {
                //Cell selection
                ElemClicked = cmi;
                if (_CellSelection && e.shiftKey) {
                  _CellSelectEnd = cmi;
                } else {
                  _CellSelectStart = _CellSelectEnd = cmi;
                  _CellSelectStart.RowsOnly = cmi.Type == ElementType.RowSelector;
                }
                FixCellSelection(cmi.Type, true);
                StartAnimating();
              }
            }
          }

        }
      }
      _MouseDown = true;
      _MouseDownTime = Date.now();
    
    } else if (e.button == 2) {
      if (_Editor) _Editor.style.display = 'none';
      _RightMouseDown = true;
      var cmi = GetElement(e);
      if (cmi && !InCellSelection(cmi)) {
        if (cmi.Type == ElementType.RowSelector || cmi.Type == ElementType.Cell) {
          _CellSelectStart = _CellSelectEnd = cmi;
          FixCellSelection(cmi.Type);
        } else {
          ResetCellSelection();
        }
        
        CheckDraw();
      }
    }

    _MovedThreshold = false;

    e.preventDefault();

  }

  var _OnMouseMove = function(e){

    DrawUtils.FixEvent(e, PositionOffset);
    _LastYPos = e.ctlY;

    if ((e.srcElement != Canvas && !_MouseDown) || _FlickScrollerTimer) return;

    //Get the element the user is hovering over.
    ElemHover = GetElement(e);
    var Cursor = 'default';

    //check if the mouse has moved more than x pixels.
    if (!_MovedThreshold && ElemClicked && (Math.abs(ElemClicked.X - e.ctlX) > 4 || Math.abs(ElemClicked.Y - e.ctlY) > 4)) _MovedThreshold = true;

    //Hovering.
    if (!_MouseDown) {

      if (ElemHover && (ElemHover.Type == ElementType.Column || ElemHover.Type == ElementType.GroupBy)) {

        //StartAnimating();
        if (ElemHover.IsEdge) {
          //over resize area.
          Cursor = 'col-resize';
          //return;
        }
      } else {
        //IsAnimating = false;
      }
    } else if ((e.button == 0 || e.touches) && ElemClicked) {

      //Dragging.
      if (ElemClicked.Type == ElementType.Column) {
        //Column was initially clicked

        if (ElemClicked.IsEdge) {

          //Resizing the column
          ElemHover = null;
          var Column = ElemClicked.Column;
          Cursor = 'col-resize';
          Column.Width = Math.max(ElemClicked.OrigWidth + (e.ctlX - ElemClicked.X), 20);
          Column.ResetCachedText(true);

        } else if (_MovedThreshold) {
          //Moving the column

          if (e.target != Canvas) {

            //outside the bounds of the canvas
            if (ElemClicked.Column.Band.AllowColumnChooser) {
              Cursor = 'no-drop';
              ElemClicked.Column._Removing = true;
            }
          } else {
            ElemClicked.Column._Removing = false;
            if (ElemHover && ElemHover.Type == ElementType.GroupBy) {

              ElemHover.IsMove = true;
              Cursor = 'copy';

            } else {

              //dragging a column somewhere else in the grid, treat this as moving.
              ElemHover = GetElement(e, ElemClicked.Column.Band);
              if (ElemHover) {
                Cursor = 'move';
                //Get the column at the mouse x point, for the same band as the click column.
                if (ElemHover.Column != ElemClicked.Column || ElemClicked.IsMove) {
                  ElemHover.IsMove = true;
                  ElemClicked.IsMove = true;
                }
              }

            }

          }
        }
      } else if (_MovedThreshold && ElemClicked.Type == ElementType.GroupBy && ElemClicked.Band) {
        //Group by column was clicked.
        if (ElemHover && ElemHover.Type == ElementType.GroupBy) {
          Cursor = 'move';
          //if (ElemClicked.Band != ElemHover.Band) {
          //  ElemHover.IsMove = true;
          //}
        } else {
          Cursor = 'no-drop';
          ElemClicked.Remove = true;
        }
      } else if (ElemHover && _CellSelectStart && (ElemHover.Type == ElementType.Cell || ElemHover.Type == ElementType.RowSelector)) {
        
        _CellSelectEnd = ElemHover;
        FixCellSelection(ElemHover.Type);
      } else if (_TouchedInCanvas && _MovedThreshold && ElemClicked.Type == ElementType.Cell) {
        //Finger dragging
        var StartPos = ElemClicked.ScrollPos;
        var MovedAmount = e.ctlY - ElemClicked.Y;
        VScroll.SetValues(StartPos - (MovedAmount / ElemClicked.Column.Band.CellStyle.Height));
          

      }

    }

    //if (Cursor != Canvas.style.cursor) {
    //setTimeout(function () {
    Canvas.style.cursor = Cursor;
    //}, 0);

    //}

    if ((LastElemHover || ElemHover) && (!ElemHover || !LastElemHover || LastElemHover.Type != ElemHover.Type || LastElemHover.UniqueRef != ElemHover.UniqueRef)) {
      LastElemHover = ElemHover;
      LastElementChangeTime = new Date().getTime();
      SetTooltip('');
      CheckDraw();
    }

    CtlMouse(e, VScrollX, 0);
    VScroll.MouseMove(e);
    CtlMouse(e, 0, HScrollY);
    HScroll.MouseMove(e);
  }

  var _OnMouseUp = function (e) {

    DrawUtils.FixEvent(e, PositionOffset);

    IsScrolling = false;
    
    if (_MouseDown) {
      //reset variables
      _RightMouseDown = false;
      _MouseDown = false;

      CtlMouse(e, VScrollX, 0);
      VScroll.MouseUp(e);

      CtlMouse(e, 0, HScrollY);
      HScroll.MouseUp(e);

      Canvas.style.cursor = 'default';

      IsAnimating = false;

      if (ElemClicked) {
        if (ElemClicked.Type == ElementType.Column) {
          if (ElemClicked.IsEdge) {

            //finished resizing column.
            ElemClicked.Column.ResetCachedText(false);
            CalcBounds = true;
            CheckHScroll();

          } else if (_MovedThreshold) {

            if (ElemClicked.Column._Removing) {
              ElemClicked.Column.Remove(true);
              
            } else if (ElemHover) {
              //moved the mouse more than a few pixels
              if (ElemHover.Type == ElementType.GroupBy) {

                //dragged column to group by.
                GroupByInfo.InsertColumn(ElemClicked.Column, ElemHover);

              } else if (ElemHover.IsMove) {

                //Move column
                var columns = ElemClicked.Grid.Band.Columns;
                columns.splice(ElemHover.Index, 0, columns.splice(ElemClicked.Index, 1)[0]);

              }
            }

          } else if (ElemClicked.IsFilter || (_TouchedInCanvas && Date.now() - _MouseDownTime > 750)) {
            ElemClicked.Column.ShowColumnInfo(ElemClicked);
          } else {

            //Sort
            ElemClicked.Column.Sort();
            CalcBounds = true;
          }
        } else if (ElemClicked.Type == ElementType.GroupBy && ElemClicked.Band) {

          //Group by column
          if (ElemClicked.Remove || ElemHover.IsMove) {
            var ReAdd = [],
                DataBand = ElemClicked.Band.GroupByInfo.DataBand;

            if (ElemHover && ElemHover.IsMove) {
              var NewIndex, RemoveBand;

              if (ElemClicked.GroupIndex < ElemHover.GroupIndex) {
                //moved forward
                ElemClicked.Band.RemoveGrouping(ReAdd);
                ReAdd.insert(ElemHover.GroupIndex, ElemClicked.Band);

              } else {
                //moved back
                ElemHover.Band.RemoveGrouping(ReAdd);
                ReAdd.insert(0, ElemHover.Band);
                ReAdd.splice(ElemClicked.GroupIndex - ElemHover.GroupIndex, 1);
                ReAdd.insert(0, ElemClicked.Band);
              }


            } else {
              ElemClicked.Band.RemoveGrouping(ReAdd);
            }

            DataBand.CreateGroupings(ReAdd);

          } else if (ElemClicked.Settings) {
            ElemClicked.Band.ShowGroupSettings();
          }

        } else if (ElemClicked.Type == ElementType.GridSelector) {
          ElemClicked.Grid.Band.ShowGroupSettings(ElemClicked.Grid);
        } else if (_TouchedInCanvas && _MovedThreshold && ElemClicked.Type == ElementType.Cell) {
          var ms = Date.now() - FlickScrollParams[0].ms;
          var moved = _LastYPos - FlickScrollParams[0].Y;
          var NewVelocity = (moved / ms);
          if (NewVelocity < 0 == _FlickVelocity < 0) {
            _FlickVelocity += NewVelocity;
          } else {
            _FlickVelocity = NewVelocity;
          }
          
          var Position = VScroll.Value();
          if (Math.abs(_FlickVelocity) > 0.5) {
            _FlickScrollerTimer = setInterval(function () {
              Position -= _FlickVelocity;
              VScroll.SetValues(Position);

            }, 50);
          }
        }
        ElemClicked = null;
        ElemHover = null;
      }
      Draw();
    } else if (_RightMouseDown) {
      _RightMouseDown = false;
      _MouseDown = false;

      //right click (this must be done on mouseup, otherwise you cant cancel the default event)
      var elem = GetElement(e);
      if (elem) {
        //if (elem.Type == ElementType.Column || elem.Type == ElementType.Cell) {
          
          if (ShowMenu(elem, e.pageX, e.pageY)) {
            e.preventDefault();
            return false;
          }
          
        //}
      }

    }

    if (!_FlickScrollerTimer) _FlickVelocity = 0;

  }

  var _OnKeyDown = function (e) {
    if (e.keyCode == 67 && e.ctrlKey && _CellSelection && _Opt.AllowCopy) {
      CellCopy(false);
    }
    if (e.keyCode == 27) {
      ResetCellSelection();
      CheckDraw();
    }
    
  }

  $(document).ready(function () {

    document.addEventListener("contextmenu", function (e) {
      DrawUtils.FixEvent(e);
      if (e.target == Canvas) {
        e.preventDefault();
      }
    });
    
    Canvas.addEventListener("touchstart", function (e) {
      _TouchedInCanvas = true;
      e.preventDefault();
      _OnMouseDown(e);
    });
    document.addEventListener("touchmove", function (e) {
      if (_TouchedInCanvas) {
        e.preventDefault();
        _OnMouseMove(e);
      }
    });
    document.addEventListener("touchend", function (e) {
      if (_TouchedInCanvas) {
        e.preventDefault();
        _OnMouseUp(e);
      }
      _TouchedInCanvas = false;
    });

    Canvas.addEventListener("mousedown", _OnMouseDown);
    document.addEventListener("mousemove", _OnMouseMove);
    document.addEventListener("mouseup", _OnMouseUp);
    document.addEventListener("keydown", _OnKeyDown)
           
  });

  Canvas.addEventListener('mousewheel', function (e) {
    if ((e.wheelDelta < 0 && VScroll.Value() < VScroll.Maximum())
      || e.wheelDelta > 0 && VScroll.Value() > 0) {

      e.preventDefault();
      VScroll.SetValues(VScroll.Value() - Math.ceil(e.wheelDelta / 30));
    }

    return false;
  });

  //Hovering
  var LastElementChangeTime = null;
  var GeneralTimer = window.setInterval(function () {
    if (ElemHover && LastElementChangeTime != null && LastElementChangeTime < new Date().getTime() - 500) {
      LastElementChangeTime = null;
      if (ElemHover.Type == ElementType.Cell) {
        var row = ElemHover.Row;
        if (row._SGrid.TextCache[ElemHover.Column.Key]) {
          SetTooltip(row[ElemHover.Column.Key]);
          //Set Postion
          $('.SGrid-Tooltip').css({ left: ElemHover.Left + PositionOffset.left - 1 - HScroll.Value(), top: ElemHover.RowTop + PositionOffset.top });
        }
      } else if (ElemHover.Type == ElementType.Column) {
        if (ElemHover.Column.ToolTip) {
          SetTooltip(ElemHover.Column.ToolTip);
          $('.SGrid-Tooltip').css({ left: ElemHover.Left + PositionOffset.left - 1 - HScroll.Value(), top: ElemHover.Y + PositionOffset.top });
        }
      }
    }
  }, 500);
  var _Tooltip = '';
  var SetTooltip = function (value) {
    if (_Tooltip != value) {
      _Tooltip = value;
      Singular.SGrid.Tooltip(_Tooltip);
    }
  }

  //#endregion

  //#region Helpers

  var ElemClicked,
      ElemHover,
      LastElemHover;

  var GetElement = function (e, TargetBand) {

    function AddCommonProperties(Type, UniqueRef, Element) {
      Element.Type = Type;
      Element.X = e.ctlX;
      Element.Y = e.ctlY;
      Element.UniqueRef = UniqueRef;
      return Element;
    }

    //Group by
    if (GroupByRect && GroupByRect.Contains(e.ctlX, e.ctlY)) {

      var info = { Index: 0, GroupIndex: 0 },
          IsCol = (ElemClicked && ElemClicked.Column != undefined),
          IsDragging = ElemClicked && (ElemClicked.Column || ElemClicked.Band),
          FromLevel = -1,
          FromBand,
          LastDataBand = null;

      if (IsDragging) {
        FromBand = IsCol ? ElemClicked.Column.Band : ElemClicked.Band.GroupByInfo.DataBand;
        FromLevel = FromBand.Level;
      }

      //find out which group by its over.
      for (var i = 0; i < self.Bands.length; i++) {
        var band = self.Bands[i];
        info.Band = band;
        info.Index = i;

        if (band.GroupByInfo || IsCol) {
          if (band.GroupByInfo && LastDataBand == band.GroupByInfo.DataBand) {
            info.GroupIndex += 1;
          } else {
            info.GroupIndex = 0;
          }

          if (IsDragging) {
            info.Dragging = true;

            var ToBand = band.GroupByInfo ? band.GroupByInfo.DataBand : band,
                ToLevel = ToBand.Level;

            if (!IsCol && ElemClicked.Index < i) {
              info.MoveForward = true;
            }

            if ((IsCol && FromLevel < ToLevel) || (FromBand == ToBand && (!band.GroupByInfo || e.ctlX < band.GroupByInfo.Rect.Right() + GROUP_HEADER_GAP))) {
              info.IsMove = ElemClicked.Index != i;
              return AddCommonProperties(ElementType.GroupBy, info.Index, info);
            }

          } else if (!ElemClicked) {

            //hovering
            if (band.GroupByInfo && band.GroupByInfo.Rect.Contains(e.ctlX, e.ctlY)) {
              if (e.ctlX > band.GroupByInfo.Rect.Right() - 24) {
                info.Settings = true;
              }
              return AddCommonProperties(ElementType.GroupBy, info.Index, info);
            }
          }
          LastDataBand = band.GroupByInfo ? band.GroupByInfo.DataBand : null;
        }
			  
      }

      //if dragged past the last element
      if (IsDragging && IsCol) {
        if (info.Band && (FromLevel > ToLevel || e.ctlX > info.Band.GroupByInfo.Rect.Right())) {
          info.Index += 1;
        }
        return AddCommonProperties(ElementType.GroupBy, info.Index, info);
      } else {
        info.Band = null;
        info.IsMove = false;
        return AddCommonProperties(ElementType.GroupBy, info.Index, info);
      }

    }
       
    //Expansion indicators.
    for (var i = 0; i < RowExpanders.length; i++) {
      var rexp = RowExpanders[i];
      if (e.ctlX >= rexp.X - 2 && e.ctlX <= rexp.X + 11 && e.ctlY >= rexp.Y - 2 && e.ctlY <= rexp.Y + 11) {
        return AddCommonProperties(ElementType.RowExpansion, 0, { Expander: rexp });
      }
    }

    //Columns
    for (var i = 0; i < _GridList.length; i++) {
      var grid = _GridList[i], ColLeft = grid.ColX, ColRight = grid.ColX;

      if ((TargetBand && grid.Band == TargetBand) || (!TargetBand && e.ctlY > grid.Y && e.ctlY < grid.GridBottom)) {
        //Header area

        if (e.ctlX >= grid.X && e.ctlX < grid.ColX) {
          if (e.ctlY < grid.Y + grid.Band.HeaderStyle.Height) {
            //Grid selector.
            if (grid.Band.AllowColumnChooser) {
              return AddCommonProperties(ElementType.GridSelector, grid, { Grid: grid });
            } else {
              return null;
            }
          } else {
            //Row selector.
            var RowIndex = Math.floor((e.ctlY - grid.Y - grid.Band.HeaderStyle.Height) / grid.RowHeight) + grid.MinRowIndex;
            return AddCommonProperties(ElementType.RowSelector, 'Row_' + RowIndex, { Grid: grid, RowIndex: RowIndex, Row: RowIndexes[RowIndex].Row });
          }
        }

        for (var j = 0; j < grid.Band.Columns.length && ColLeft < VScrollX + grid.xOffset; j++) {
          var col = grid.Band.Columns[j];
          if (col.Visible) {
            ColLeft = col._.Left;
            ColRight = ColLeft + col.Width;

            if (e.ctlX >= ColLeft && e.ctlX < ColRight + (_MouseDown ? 0 : 5)) {
              var type,
                  props = {
                    Column: col,
                    Index: j,
                    Grid: grid,
                    Left: ColLeft,
                    Right: ColRight
                  };

              var UniqueRef;
              if (e.ctlY <= grid.Y + grid.Band.HeaderStyle.Height || TargetBand && grid.Band == TargetBand) {
                //Column
                type = ElementType.Column;
                props.OrigWidth = col.Width;
                props.IsEdge = e.ctlX > ColRight - 3 && e.ctlX < ColRight + 5;
                props.IsMove = false;
                props.IsFilter = e.ctlX >= ColRight - COL_FILTER_WIDTH && e.ctlX <= ColRight - 3;
                UniqueRef = col.Key + (props.IsFilter ? '_F' : '') + (props.IsEdge ? '_E' : '');

              } else if (e.ctlY < grid.RowBottom) {
                //Cell
                type = ElementType.Cell;
                var RowIndex = Math.floor((e.ctlY - grid.Y - grid.Band.HeaderStyle.Height) / grid.RowHeight);
                props.RowTop = RowIndex * grid.RowHeight + grid.Y + grid.Band.HeaderStyle.Height;
                props.Row = RowIndexes[grid.MinRowIndex + RowIndex].Row;
                props.RowIndex = grid.MinRowIndex + RowIndex;
                props.ColIndex = j;
                props.Grid = grid;
                UniqueRef = col.Key + RowIndex;
              }

              return AddCommonProperties(type, UniqueRef, props);
            }

            ColLeft = ColRight;
          }
        }
      } 
    }
   

    return null;
  }

  //#endregion

  //#endregion

  //#region Layout

  window.addEventListener("resize", function (e) {
    //this should be changed to animate.
    CalcBounds = true;
    CalcLayout();
    FindScrollHeight();
    CheckDraw();
  });

  var _DoneLayout = false;
  var CalcLayout = function () {

    var Style = window.getComputedStyle(Canvas.parentElement);
    var VMargin = parseInt(Style.marginTop) + parseInt(Style.marginBottom);

		  _DoneLayout = true;
		  Canvas.width = Parent.width();//* window.devicePixelRatio;

		  if (_AutoHeight !== undefined) {
		    Canvas.height = (window.innerHeight - VMargin - _AutoHeight);//* window.devicePixelRatio;
		    Parent.height(Canvas.height);
		  } else {
		    Canvas.height = $(Canvas).height();
		  }

		  MainRect = new Rectangle(0, 0, Canvas.width, Canvas.height);

		  VScrollX = Canvas.width - VScroll.Thickness();
			TooWide = MaxHeaderWidth >= VScrollX - 1;
      VScroll.Resize(Canvas.height - (TooWide ? (VScroll.Thickness() - 1) : 1));
			
			var ScrollWidth = Canvas.width - VScroll.Thickness() - 1;
			HScroll.Resize(ScrollWidth);
			HScroll.SetLargeChange(ScrollWidth);
			HScrollY = Canvas.height - HScroll.Thickness();

			var MainGridY = 0;
			if (_Opt.AllowGroupBy) {
			  MainGridY = GROUP_BY_HEIGHT + 2;
			  GroupByRect = new Rectangle(0, 0, Canvas.width - VScroll.Thickness() - 2, GROUP_BY_HEIGHT);
			} else {
			  GroupByRect = null;
			}
			GridRect = new Rectangle(0, MainGridY, Canvas.width - VScroll.Thickness(), Canvas.height - MainGridY);

			PositionOffset = $(Canvas).offset();
		//}
  };

  var MaxHeaderWidth = 0;
  var CheckHScroll = function () {
        
    MaxHeaderWidth = 0;
    for (var i = 0; i < self.Bands.length; i++) {
      var Band = self.Bands[i], HeaderWidth = self.DefaultStyles.RowSelectorWidth + Band.GetX();
      for (var j = 0; j < Band.Columns.length; j++) {
        if (Band.Columns[j].Visible) HeaderWidth += Band.Columns[j].Width;
      }
      MaxHeaderWidth = Math.max(MaxHeaderWidth, HeaderWidth);
    }
        
    HScroll.SetValues(undefined, MaxHeaderWidth);
   
    CalcLayout();
    FindScrollHeight();
    CheckDraw();
  
  }

  //#endregion

  //#region Drawing

  var IsAnimating = false,
      IsDrawing = false,
      DrawOnce = false,
      IsScrolling = false,
      FlickScrollParams = [];

  var CheckDraw = function () {
    if (!IsAnimating && !IsDrawing) {
      Draw();
    }
    if (!IsAnimating && IsDrawing) {
      DrawOnce = true;
    }
  }
  var StartAnimating = function () {
    if (!IsAnimating) {
      IsAnimating = true;
      Draw();
    }
  }

  var CalcBounds = true;

  self.ReDraw = function () {
    CheckDraw();
  }

  var Draw = function () {
    DrawOnce = false;
    IsDrawing = true;

    Context.clearRect(0, 0, MainRect.Width, MainRect.Height);

    //Draw the root band.
    if (self.Bands.length > 0) {

      _YPos = GridRect.Y, _DrawIndex = VScroll.Value(), _DrawStartMS = Date.now();
      if (ElemClicked && _TouchedInCanvas) {
        if (FlickScrollParams.length > 2) FlickScrollParams.shift();
        FlickScrollParams.push({ ms: _DrawStartMS, Y: _LastYPos });
      }
      RowExpanders = [];

      GroupByInfo.Draw();
     
      if (CalcBounds) {
        _GridList = [];
        RowExpanders = [];
        _MultiBand = self.Bands.length > 1;

        while (_YPos < MainRect.Height && _DrawIndex < RowIndexes.length) {
          var grid = new Grid(_DrawIndex, _YPos, grid);
          _GridList.push(grid);
          DrawGrid(grid);

        }

        CalcBounds = false;
      } else {
        for (var i = 0; i < _GridList.length; i++) {
          DrawGrid(_GridList[i]);
        }

      }

      //if the editor wasnt rendered yet, it is off screen and needs to be hidden.
      if (_DoCellEditLayout && _CellEditInfo && _Editor) {
        PositionCellEditor(-1, -1, null);
      }

      //Draw the row expanders.
      for (var i = 0; i < RowExpanders.length; i++) {
        var ri = RowExpanders[i],
			      Img = RowIndexes[ri.Index].Row._SGrid.Expanded ? ContractImage : ExpandImage;

        if (ri.X + 16 > 0) {
          Context.drawImage(Img, ri.X - 0.5, ri.Y - 0.5);
        }
      }

    }

    //Draw scrollbar
    Context.clearRect(VScrollX - 1, 0, VScroll.Thickness() + 1, MainRect.Height)
    Context.drawImage(VScroll.GetCanvas(), VScrollX, 0);
    if (TooWide) {
      Context.clearRect(0, HScrollY - 1, MainRect.Width, HScroll.Thickness() + 1)
      Context.drawImage(HScroll.GetCanvas(), 0, HScrollY);
    }

    //If the user is doing something like resizing, moving, scrolling etc, then keep drawing.
    if (IsAnimating || DrawOnce) {
      window.requestAnimationFrame(Draw);
    } else {
      IsDrawing = false;
    }
  };
  var _YPos, _DrawIndex, _DrawStartMS;

  var DrawGrid = function (Grid) {

    var Band = Grid.Band,
        RowSelectorWidth = self.DefaultStyles.RowSelectorWidth,
        HeaderWidth = RowSelectorWidth,
        ScreenRight = VScrollX + HScroll.Value(),
        xOffset = HScroll.Value();

    //Calculate the header width
    for (var j = 0; j < Band.Columns.length /*&& HeaderWidth < ScreenRight*/; j++) {
      if (Band.Columns[j].Visible) HeaderWidth += Band.Columns[j].Width;
    }

    var gRight = Grid.X + HeaderWidth + VScroll.Thickness() + 2;
    if (gRight - xOffset < MainRect.Width) {
      xOffset = Math.max(0, gRight - MainRect.Width);
    }
    Grid.xOffset = xOffset;
    //Alternate h.scrolling
    //xOffset *= (Grid.X + HeaderWidth - MainRect.Width + 19) / (MaxHeaderWidth - MainRect.Width + 19);

    var PadV = Band.HeaderStyle.PaddingV,
        PadH = Band.HeaderStyle.PaddingH,
        PadV2 = PadV * 2,
        PadH2 = PadH * 2,
        FontSize = Band.HeaderStyle.FontSize,
        x = Grid.X,
        HeaderEnd,
        Top = _YPos;

    Context.textBaseline = 'bottom';
    Context.font = Band.CellStyle.FontSize + 'px ' + FONT_NAME;

    var ColLeft = x,
        MoveColX = undefined,
        HeaderWidth = RowSelectorWidth,
        BracketWidth = Context.measureText(')').width,
        LeftMost = x + RowSelectorWidth,
        IsClipping = false;

    //#region Header

    for (var j = 0; j < Band.Columns.length && HeaderWidth < ScreenRight; j++) {
      if (Band.Columns[j].Visible) HeaderWidth += Band.Columns[j].Width;
    }

    HeaderWidth -= xOffset;

    Context.fillStyle = Grid.HeaderBackColor;
    Context.fillRect(x - 1, _YPos, HeaderWidth, Band.HeaderStyle.Height);

    //prepare col border style
    Context.lineWidth = 1;
    Context.strokeStyle = '#888';
    Context.beginPath();

    if (self.DefaultStyles.HeaderStyle.BorderColor) {
      //custom header colour, must draw horizontal lines
      Context.strokeStyle = self.DefaultStyles.HeaderStyle.BorderColor;

      //top
      Context.moveTo(ColLeft - 0.5, _YPos - 0.5);
      Context.lineTo(ColLeft + HeaderWidth, _YPos - 0.5);

      //bottom
      Context.moveTo(ColLeft - 0.5, _YPos + Band.HeaderStyle.Height - 0.5);
      Context.lineTo(ColLeft + HeaderWidth, _YPos + Band.HeaderStyle.Height - 0.5);

      //left
      Context.moveTo(ColLeft + 0.5, _YPos);
      Context.lineTo(ColLeft + 0.5, _YPos + Band.HeaderStyle.Height);
    }

    //Row selector header:
    if (Band.AllowColumnChooser) {
      Context.font = '12px FontAwesome';
      Context.textAlign = 'center';
      Context.fillStyle = self.DefaultStyles.HeaderIconColor;
      Context.fillText('\uf1b2', x + RowSelectorWidth / 2, _YPos + 16);
    }
   
    Context.moveTo(ColLeft + RowSelectorWidth - 0.5, _YPos);
    Context.lineTo(ColLeft + RowSelectorWidth - 0.5, _YPos + Band.HeaderStyle.Height);
    if (ElemHover && ElemHover.Type == ElementType.GridSelector && ElemHover.Grid == Grid) {
      Context.fillStyle = self.DefaultStyles.HoverOverlayColor;
      Context.fillRect(x - 0.5, _YPos, RowSelectorWidth, Band.HeaderStyle.Height);
    }

    ColLeft = x + RowSelectorWidth;
            
    //Col headers
    Context.font = FontSize + 'px ' + FONT_NAME;
    for (var j = 0; j < Band.Columns.length && ColLeft < ScreenRight; j++) {
      var Col = Band.Columns[j],
		      Hovering = !_TouchedInCanvas && ElemHover && ElemHover.Type == ElementType.Column && ElemHover.Column == Col && !ElemHover.IsEdge && !ElemHover.IsMove;
      
      if (j == Band.FrozenColumn + 1 && xOffset > 0) {
        LeftMost = ColLeft;
        ColLeft -= xOffset;

        //clip from the leftmost position
        Context.stroke();
        Context.save();
        Context.beginPath();
        Context.rect(LeftMost, _YPos, HeaderWidth, _YPos + Band.HeaderStyle.Height);
        Context.clip();
        IsClipping = true;
        Context.beginPath();
      }

      if (Col.Visible) {
        Col._.OffScreen = j > Band.FrozenColumn && ColLeft + Col.Width < LeftMost;

        if (!Col._.OffScreen) {
          Col._.ShowFilter = Col.Filters.length > 0 || !!Hovering;
          Col._.ShowSort = Col.SortDirection != null || !!Hovering;
          Col._.BracketNegatives = Col.Type == 'n' && Col.TextAlign == 'right' && Col.FormatString.indexOf(')') > 0;
          Col._.BracketWidth = BracketWidth;
          Col._.Left = ColLeft;

          if (Col.HeaderStyle.__Values.BackColor) {
            //Custom header colour
            Context.fillStyle = Col.HeaderStyle.__Values.BackColor;
            Context.fillRect(ColLeft - 1, _YPos, Col.Width, Band.HeaderStyle.Height+1);
          }

        	//Draw header text          
          Context.fillStyle = Col.HeaderStyle.ForeColor;
          Context.textAlign = Col.TextAlign;
          if (Band._ColHeaderLines > 1) {
            var TextLines = Col.CellTextSized();
            var lY = _YPos + FontSize + PadV;
            for (var l = 0; l < TextLines.length && l < Band._ColHeaderLines; l++) {
              Context.fillText(TextLines[l], Col._.TextXPos(ColLeft), lY);
              lY += FontSize + 1;
            }
          } else {
            Context.fillText(Col.CellTextSized(), Col._.TextXPos(ColLeft), _YPos + FontSize + PadV + 1);
          }
         	

          if (ElemClicked && ElemHover && ElemHover.Type == ElementType.Column && ElemHover.Column == Col && ElemClicked.IsMove) {
            //if this is the column the mouse is over while moving, remember the position for drawing below.
            MoveColX = (ColLeft - 1) + (ElemHover.Index < ElemClicked.Index ? 0 : (Col.Width / (ElemHover.Index == ElemClicked.Index ? 2 : 1)));
          }

          //Removing column
          if (Col._Removing) {
            Context.fillStyle = 'rgba(255,255,255,0.5)';
            Context.fillRect(ColLeft - 1, _YPos, Col.Width, Band.HeaderStyle.Height);
          }

          ColLeft += Col.Width;

          //right border
          if (j < Band.Columns.length) {
            Context.moveTo(ColLeft - 0.5, _YPos);
            Context.lineTo(ColLeft - 0.5, _YPos + Band.HeaderStyle.Height);
          }
          Context.textAlign = 'left';

        	//Sort / filter / hover indicator 
          if (Col._.ShowFilter || _Opt.AlwaysShowHeaderFilters) {

          	Context.font = '12px FontAwesome';
          	Context.fillStyle = Col.Filters.length == 0 ? self.DefaultStyles.HeaderIconColor : self.DefaultStyles.HeaderIconActiveColor;
          	Context.fillText(Col.Filters.length == 0 ? '\uf013' : '\uf0b0', ColLeft - COL_FILTER_WIDTH + 2, _YPos + 16);

          	//overlay fill
          	if (Hovering && ElemHover.IsFilter) {
          		Context.fillStyle = self.DefaultStyles.HoverOverlayColor;
          		Context.fillRect(ColLeft - COL_FILTER_WIDTH, _YPos, COL_FILTER_WIDTH, Band.HeaderStyle.Height);
          	}

          	Context.font = FontSize + 'px ' + FONT_NAME; //reset text back to normal.

          }
          if (Col._.ShowSort || _Opt.AlwaysShowHeaderFilters) {
            //sort indicator.
            var Offset = (Col._.ShowFilter || _Opt.AlwaysShowHeaderFilters ? COL_FILTER_WIDTH : 0) + COL_SORT_WIDTH;

            Context.font = '12px FontAwesome';
            if (Col.SortDirection == 0 || Hovering || _Opt.AlwaysShowHeaderFilters) {
              Context.fillStyle = Col.SortDirection == 0 ? self.DefaultStyles.HeaderIconActiveColor : self.DefaultStyles.HeaderIconColor;
              Context.fillText('\uf0de', ColLeft - Offset + 2, _YPos + 17);
            }
            if (Col.SortDirection == 1 || Hovering || _Opt.AlwaysShowHeaderFilters) {
              Context.fillStyle = Col.SortDirection == 1 ? self.DefaultStyles.HeaderIconActiveColor : self.DefaultStyles.HeaderIconColor;
              Context.fillText('\uf0dd', ColLeft - Offset + 2, _YPos + 17);
            }
            Context.font = FontSize + 'px ' + FONT_NAME; //reset text back to normal.

          }
        } else {
          ColLeft += Col.Width;
        }

      }

    }
    //Draw all the queued lines .
    Context.stroke();

    _YPos += Band.HeaderStyle.Height;
    HeaderEnd = _YPos;

    if (IsClipping) {
      Context.restore();
      IsClipping = false;
    }

    //Move to indicator
    if (MoveColX != undefined) {
      Context.drawImage(ColMoveImage, (MoveColX - ColMoveImage.width / 2) + 0.5, Top);
      Context.drawImage(ColMoveImageF, (MoveColX - ColMoveImage.width / 2) + 0.5, Top + Band.HeaderStyle.Height - ColMoveImage.height);
    }

    //#endregion

    //#region Rows

    function DrawHConnector(y) {
      Context.beginPath();
      Context.strokeStyle = '#bbb';
      Context.save();
      Context.setLineDash([2, 2]);
      Context.moveTo(x - 10.5, y);
      Context.lineTo(x, y);
      Context.stroke();
      Context.restore();
    }

    function DrawFreezeLine(t, b, opacity) {
      if (Grid.xOffset > 0) {
        Context.save();
        Context.lineWidth = 1;
        Context.shadowColor = '#000';
        Context.shadowBlur = 2;
        Context.shadowOffsetX = 1;

        Context.strokeStyle = 'rgba(0,0,0,' + opacity + ')';
        Context.beginPath();
        Context.moveTo(LeftMost - 0.5, t);
        Context.lineTo(LeftMost - 0.5, b);
        Context.stroke();
        Context.restore();
      }
    }

    PadV = Band.CellStyle.PaddingV;
    PadH = Band.CellStyle.PaddingH;
    PadV2 = PadV * 2;
    PadH2 = PadH * 2;
    FontSize = Band.CellStyle.FontSize;
    var RowHeight = Band.CellStyle.Height,
        HalfRowHeight = Math.floor(RowHeight / 2) + 0.5,
        SelectedRect = { t: -1, l: -1, r: 0, b: 0 };

    Context.lineWidth = 1;

    Context.font = FontSize + 'px ' + FONT_NAME;

    Grid.RowTop = _YPos + HalfRowHeight;
    Grid.RowHeight = RowHeight;

    var Rows = [];
    for (_DrawIndex; _YPos < MainRect.Height && _DrawIndex < RowIndexes.length && RowIndexes[_DrawIndex].Band == Grid.Band; _DrawIndex++) {
      var row = RowIndexes[_DrawIndex].Row;
      if (row) {
        Rows.push({ row: row, _YPos: _YPos, DrawIndex: _DrawIndex });

        var backColour;
        if (_DrawIndex % 2 == 1) {
          backColour = Band.RowColor;
        } else {
          backColour = Band.RowAltColor;
        }

        //Conditional formatting
        if (_Opt.RowColor) {
          if (row._SGrid.BackColor === undefined) {
            //only call this if the cached value is null
            row._SGrid.SelectorColor = null; //so the user knows this property exists.
            row._SGrid.BackColor = _Opt.RowColor(row, _DrawIndex % 2 == 1, backColour);
          }
          backColour = row._SGrid.BackColor;
        }

        //fill row selectors
        if (x + RowSelectorWidth > 0) {
          Context.fillStyle = row._SGrid.SelectorColor ? row._SGrid.SelectorColor : Band.GetRowGradient(_YPos, RowHeight);
          Context.fillRect(x - 1, _YPos, RowSelectorWidth, RowHeight);
        }

        //Fill cell background
        Context.fillStyle = backColour;
        Context.fillRect(x + RowSelectorWidth, _YPos, HeaderWidth - RowSelectorWidth - 1, RowHeight);

        //expansion indicator
        if (_MultiBand) {
          if (row._SGrid.Expanded === undefined) {
            row._SGrid.Expanded = null;
            //check if any of the child bands have data.
            for (var j = 0; j < Grid.Band.ChildBands.length; j++) {
              var ChildList = row[Grid.Band.ChildBands[j].Key];
              if (ChildList && ChildList.length > 0) {
                row._SGrid.Expanded = false;
                break;
              }
            }
          }

          if (row._SGrid.Expanded != null) {
            var ExpandBlock = { X: x - 14.5, Y: Math.floor(_YPos + (RowHeight - 9) / 2) + 0.5, Index: _DrawIndex };
            RowExpanders.push(ExpandBlock);
          }

          //Horizontal Dotted lines
          DrawHConnector(_YPos + HalfRowHeight);
        }

        _YPos += RowHeight;
        Grid.MaxRowIndex = _DrawIndex;

        //horizontal row lines.
        Context.strokeStyle = Band.RowBorderColor;
        Context.beginPath();
        Context.moveTo(x + RowSelectorWidth, _YPos - 0.5);
        Context.lineTo(HeaderWidth + x - 1, _YPos - 0.5);
        Context.stroke();
               
      }
    }

    //vertical cell lines.
   
    if (Rows.length) {
      Context.beginPath();
      Context.moveTo(x + RowSelectorWidth - 0.5, HeaderEnd);
      Context.lineTo(x + RowSelectorWidth - 0.5, _YPos);

      for (var j = 0; j < Band.Columns.length; j++) {
        if (Band.Columns[j]._.Left && Band.Columns[j]._.Left >= ScreenRight) break;
        var Col = Band.Columns[j];
        if (Col.Visible) {
          if (!Col._.OffScreen) {
            Context.moveTo(Col._.Left + Col.Width - 0.5, HeaderEnd);
            Context.lineTo(Col._.Left + Col.Width - 0.5, _YPos);
          }
                    
          //Removing column
          if (Col._Removing) {
            Context.fillStyle = 'rgba(255,255,255,0.5)';
            Context.fillRect(Col._.Left - 1, HeaderEnd, Col.Width, _YPos - HeaderEnd);
          }

        }
      }
      
      Context.stroke();

      //Create offscreen image for scroll clipping
      if (xOffset > 0) {
        if (!Grid.cTextContext) {
          Grid.cTextBuffer = document.createElement('canvas');
          Grid.cTextBuffer.width = MainRect.Width;
          Grid.cTextBuffer.height = Rows[Rows.length - 1]._YPos + RowHeight - Rows[0]._YPos + 1;
          Grid.cTextContext = Grid.cTextBuffer.getContext('2d');
          Grid.cTextContext.font = Context.font;
          Grid.cTextContext.textBaseline = 'bottom';
        }
        var tCtx = Grid.cTextContext;
        tCtx.clearRect(0, 0, Grid.cTextBuffer.width, Grid.cTextBuffer.height);
      }
      
      var tY = 0, yOffset = 0;
      _YPos = HeaderEnd;

      //Cell text
      for (var i = 0; i < Rows.length; i++) {
        var ri = Rows[i], row = ri.row;

        ColLeft = LeftMost = x + RowSelectorWidth;
        //draw to main canvas by default
        tCtx = Context;
        yOffset = HeaderEnd;

        //if (!IsScrolling || Date.now() - _DrawStartMS < 20) { //stop drawing text if the fps drops below 20 while animating / scrolling.

        //cell values.
        for (var j = 0; j < Band.Columns.length && ColLeft < ScreenRight; j++) {
          var Col = Band.Columns[j];

          if (j == Band.FrozenColumn + 1 && xOffset > 0) {
            //draw to offscreen buffer
            LeftMost = ColLeft;
            ColLeft = -xOffset;
            tCtx = Grid.cTextContext;
            yOffset = 0;
          }

          if (Col.Visible) {

            if (!Col._.OffScreen) {
              tCtx.textAlign = Col.TextAlign;
              var TextX = Col._.TextXPos(ColLeft, row);
              var CellStyle = (i % 2 == 1) ? Col.CellStyle : Col.CellAltStyle;
              var TextColor = CellStyle.ForeColor;

              if (Col.CellStyle.__Values.BackColor && !Col._Removing) {
                //Custom column colour
                tCtx.fillStyle = CellStyle.BackColor;
                tCtx.fillRect(ColLeft, tY + yOffset, Col.Width, Band.CellStyle.Height);
              }

              tCtx.fillStyle = TextColor;

              if (InCellSelection({ Grid: Grid, RowIndex: ri.DrawIndex, ColIndex: j })) {
                //Cell is selected
                if (SelectedRect.t == -1) {
                  SelectedRect.l = Math.max(LeftMost, Col._.Left + 1);
                  SelectedRect.t = _YPos;
                }
                SelectedRect.r = Col._.Left + Col.Width;
                SelectedRect.b = _YPos + RowHeight;

                tCtx.fillStyle = 'rgba(128, 128, 128, 0.2)';
                tCtx.fillRect(ColLeft, tY + yOffset - 1, Col.Width, Band.CellStyle.Height);
                tCtx.fillStyle = TextColor;
              }


              if (Col.Type == 'b') {
                //boolean gets a tick, not text
                tCtx.font = '12px FontAwesome';
                tCtx.fillStyle = row[Col.Key] ? '#000' : 'rgba(0,0,0,0.5)';
                tCtx.fillText(row[Col.Key] ? '\uf00c' : '\uf00d', TextX, tY + yOffset + FontSize + PadV);
                //reset to normal
                tCtx.font = FontSize + 'px ' + FONT_NAME;
                tCtx.fillStyle = TextColor;
              } else {
                tCtx.fillText(Col.CellTextSized(row), TextX, tY + yOffset + FontSize + PadV);
              }

              if (_DoCellEditLayout && _CellEditInfo) {
                if (_CellEditInfo.Row == ri.row && _CellEditInfo.Column == Col) PositionCellEditor(Col._.Left, _YPos, Grid);
              }

            }

            ColLeft += Col.Width;
          }
        }
        tY += RowHeight;
        _YPos += RowHeight;
        //}
      }

      if (xOffset > 0) {
        Context.drawImage(Grid.cTextBuffer, LeftMost, HeaderEnd);
      }

      //Selected area border
      if (SelectedRect.t != -1) {
        Context.lineWidth = 2;
        Context.strokeStyle = '#000';
        Context.beginPath();

        Context.moveTo(SelectedRect.l, SelectedRect.t);//t
        if (_CellSelection.Start.RowIndex > Grid.MinRowIndex) {
          Context.lineTo(SelectedRect.r, SelectedRect.t);
        } else {
          Context.moveTo(SelectedRect.r, SelectedRect.t);
        }
        Context.lineTo(SelectedRect.r, SelectedRect.b);//r
        if (_CellSelection.End.RowIndex <= Grid.MaxRowIndex) {   //b
          Context.lineTo(SelectedRect.l, SelectedRect.b);
        } else {
          Context.moveTo(SelectedRect.l, SelectedRect.b);
        }
        Context.lineTo(SelectedRect.l, SelectedRect.t - 1);//l

        Context.stroke();
        Context.lineWidth = 1;
      }

      //Frozen row line
      DrawFreezeLine(Top, _YPos, 0.3);

    }
    
    Grid.RowBottom = _YPos;

    //#endregion

    //#region Footer

    if (_DrawIndex > 0 && RowIndexes[_DrawIndex - 1].LastChild) {

      //if (Band.Level > 0) {
        ColLeft = x + RowSelectorWidth;
        _YPos += 3;

        Context.fillStyle = Band.GetFooterGradient(_YPos, RowHeight);
        Context.fillRect(x - 1, _YPos - 1, HeaderWidth, RowHeight + 1);

        //Totals icon
        Context.font = '11px FontAwesome';
        Context.textAlign = 'center';
        Context.fillStyle = '#444';
        Context.fillText('\uf080', x + RowSelectorWidth / 2, _YPos + 16);

        Context.font = FontSize + 'px ' + FONT_NAME;
        Context.fillStyle = '#222';

        Context.strokeStyle = '#ddd';
        Context.beginPath();
       
        var FooterHeight = Band.GetFooterHeight();

        Context.moveTo(x + RowSelectorWidth - 0.5, _YPos);
        Context.lineTo(x + RowSelectorWidth - 0.5, _YPos + FooterHeight);

        for (var j = 0; j < Band.Columns.length && (!Band.Columns[j].Visible || Band.Columns[j]._.Left < ScreenRight) ; j++) {
          var Col = Band.Columns[j];

          if (j == Band.FrozenColumn + 1 && xOffset > 0) {
            //clip from the leftmost position
            Context.stroke();
            Context.save();
            Context.beginPath();
            Context.rect(ColLeft, _YPos - 1, HeaderWidth, _YPos + FooterHeight);
            Context.clip();
            IsClipping = true;
            Context.beginPath();

            ColLeft -= xOffset;
          }

          if (Col.Visible) {

            if (!Col._.OffScreen) {
              //Draw header text 
              if (Col.SummaryType) {
                Context.textAlign = Col.TextAlign;

                var Total = Grid.GetTotal(Col);
                var DummyRow = {};
                DummyRow[Col.Key] = Total;
                if (Total != null) Context.fillText(Total.formatDotNet(Col.FormatString), Col._.TextXPos(ColLeft, DummyRow), _YPos + FontSize + PadV);
              }

              //border
              if (j < Band.Columns.length) {
                Context.moveTo(ColLeft + Col.Width - 0.5, _YPos);
                Context.lineTo(ColLeft + Col.Width - 0.5, _YPos + FooterHeight);
              }
            }
            ColLeft += Col.Width;
          }

        }

        if (IsClipping) {
          Context.stroke();
          Context.restore();
          IsClipping = false;
          Context.beginPath();
        }

        //horizontal lines
        Context.moveTo(x + RowSelectorWidth - 1, _YPos - 0.5);
        Context.lineTo(HeaderWidth + x, _YPos - 0.5);
        Context.moveTo(x + RowSelectorWidth, _YPos + RowHeight - 0.5);
        Context.lineTo(HeaderWidth + x, _YPos + RowHeight - 0.5);
        
        //Draw all the queued lines .
        Context.stroke();

        //Frozen row line
        DrawFreezeLine(_YPos, _YPos + RowHeight, 0.1);
        

        _YPos += RowHeight;
        DrawHConnector(_YPos - HalfRowHeight);
      //}
      Grid.MaxRowIndex = _DrawIndex -1;
    }

    //#endregion

    Grid.LastRowConnector = _YPos - HalfRowHeight;
    Grid.GridBottom = _YPos;

    //add some space before the next grid.
    _YPos += GRID_GAP;

    DrawConnectingLines(Grid, Grid.Band);

  }

  var DrawConnectingLines = function (Grid, Band) {

    //lines connecting the different grid levels
    if (_MultiBand) {
      var PrevGrid = Grid ? Grid.PrevGrid : null,
          MoreRowsBand = Grid ? (!RowIndexes[Grid.MaxRowIndex].LastChild) : (_DrawIndex < Band.MaxRowIndex + 1),
          MoreRows = _DrawIndex < RowIndexes.length,
          NextLevel = MoreRows ? RowIndexes[_DrawIndex].Band.Level : -1,
          FinishedDrawing = _YPos >= MainRect.Height || !MoreRows;

      if (RowIndexes.length == 1 && !RowIndexes[0].Row) return;

      if (FinishedDrawing || NextLevel < Band.Level) {
        //the next level is a parent of this one, draw the connectors.

        if (Grid || (MoreRowsBand && FinishedDrawing)) {

          var Top;
          if (PrevGrid) {
            Top = PrevGrid.GridBottom;
          } else {
            Top = VScroll.Value() == 0 ? _GridList[0].RowTop : GridRect.Y;
          }

          var X = Band.GetX()/* - HScroll.Value()*/;
          if (X > 0) {
            Context.strokeStyle = '#bbb';
            Context.save();
            Context.setLineDash([2, 2]);

            Context.beginPath();
            Context.moveTo(X - 10.5, Top);
            Context.lineTo(X - 10.5, (MoreRowsBand && FinishedDrawing) ? GridRect.Bottom() : Grid.LastRowConnector);
            Context.stroke();

            Context.restore();
          }
        }

        if (Band.Level > 0 && (FinishedDrawing || NextLevel < Band.Level - 1)) {
          DrawConnectingLines(PrevGrid, Band.ParentBand);
        }

      }
    }
  }

  //#endregion
    
  var Setup = function () {

    $(Canvas).wrap('<div style="position: relative; overflow: hidden"></div>');
    Canvas.parentElement.style.margin = Canvas.style.margin;
    Parent = $(Canvas.parentElement);
    Canvas.style.margin = '0';
   
		VScroll.ValueChanged = function () {
		  CalcBounds = true;
		  _DoCellEditLayout = true;
			CheckDraw();
		};
		HScroll.ValueChanged = function () {
		  _DoCellEditLayout = true;
		  CheckDraw();
		};

		HScroll.SetValues(0, 20);
		VScroll.SetValues(0, 0);

		CalcLayout();
		Draw();
	}

  var ColMoveImage = new Image(),
      ColMoveImageF,
      ExpandImage = new Image(),
      ContractImage = new Image();
	ColMoveImage.src = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAsAAAAGCAYAAAAVMmT4AAAALHRFWHRDcmVhdGlvbiBUaW1lAFR1ZSAyNiBBdWcgMjAxNCAxNToyNjo1MCArMDIwMBtOCj4AAAAHdElNRQfeCBoNOwmmgu8KAAAACXBIWXMAAB7CAAAewgFu0HU+AAAABGdBTUEAALGPC/xhBQAAAD1JREFUeNpj/A8El1lZGQgB3d+/GRhBDEIawApBACaASwNMIYjNiCyBrgFZIVYA0nCJhQVE/SfoEZgGbOIAMPEpzri3IyUAAAAASUVORK5CYII=";
	ExpandImage.src = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAYAAADgkQYQAAAALHRFWHRDcmVhdGlvbiBUaW1lAFdlZCAxMCBTZXAgMjAxNCAxMDo1NjoxNSArMDIwMMRo0v0AAAAHdElNRQfeCQoJEhjiBIbgAAAACXBIWXMAAB7BAAAewQHDaVRTAAAABGdBTUEAALGPC/xhBQAAAF5JREFUeNpjNDY2loqPj3/KgAMsXLhQmgWkIDc3lwEPeMqELuISMQlDFRMDEYAFRLx795YhLGsphmmrpkUjFCELgBTD2Cgmffv2HUUQnY/hpgU9QZhuAoUDyJv4wgkAh0Ah4jCYEWoAAAAASUVORK5CYII=";
	ContractImage.src = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAYAAADgkQYQAAAALHRFWHRDcmVhdGlvbiBUaW1lAFdlZCAxMCBTZXAgMjAxNCAxMDo1NjoxNSArMDIwMMRo0v0AAAAHdElNRQfeCQoJFAXXWE2/AAAACXBIWXMAAB7BAAAewQHDaVRTAAAABGdBTUEAALGPC/xhBQAAAEtJREFUeNpjNDY2loqPj3/KgAMsXLhQmgWkIDc3lwEPeMrEQAQgShELiHj37i1DWNZSDMlV06IRipAFcJr07dt3KrgJFA4gb+ILJwDWnRgl3SQoaQAAAABJRU5ErkJggg==";

	ColMoveImage.onload = function () {
	  ColMoveImageF = DrawUtils.FlipImage(ColMoveImage, false);
	}

	Setup();
  
	return self;
};
Singular.SGrid.ApiPath = 'Singular.Web.CustomControls.SGrid.GridInterface, Singular.Web';

//#region Knockout Objects

//#region Band Properties

Singular.SGrid.GroupBy = function (Band) {
  var self = this;
  this.Columns = [];
  this.ChildColumns = [];
  this.IsGroupBand = Band.GroupByInfo != null;
  this.Name = ko.observable(Band.GroupByInfo ? Band.GroupByInfo.Text : '');
  this.GroupColumns = ko.observableArray([]);
  this.SummaryColumns = ko.observableArray([]);
  this.ShowInExport = ko.observable(Band.ShowInExport);
  this.ShowInReport = ko.observable(Band.ShowInReport);
  this.GroupColsExpanded = ko.observable(true);
  this.SummaryColsExpanded = ko.observable(true);
  this.ColHeaderLines = ko.observable(Band._ColHeaderLines);
  this._GroupsChanged = false;

  this.AddColumn = function () {
    this.GroupColumns.push(new Singular.SGrid.GroupColumn(self, null, 1));
    this._GroupsChanged = true;
  }
  this.RemoveColumn = function (Col) {
    this.GroupColumns.remove(Col);
    this._GroupsChanged = true;
  }
  this.AddSummary = function () {
    this.SummaryColumns.push(new Singular.SGrid.SummaryColumn(self, Band.ChildBands[0]));
  }
  this.RemoveSummary = function (Col) {
    Col.Unlink();
    this.SummaryColumns.remove(Col);
  }

  this.RefreshBand = function () {
    Band.SetColHeaderLines(self.ColHeaderLines());
    Band.RefreshGrouping(this);
    Singular.SGrid.CurrentGroupBy(null);
  }

  //Setup
  if (Band.GroupByInfo) {
    this.Columns = Band.GroupByInfo.DataBand.Columns;
    this.ChildColumns = Band.ChildBands[0].Columns;

    //Current group by columns
    for (var i = 0; i < Band._GroupColumns.length; i++) {
      var col = Band._GroupColumns[i];
      var gCol = new Singular.SGrid.GroupColumn(this, col.OrigCol.Key, col._DataTransformID, col._DataTransformParam);
      this.GroupColumns.push(gCol);
    }
    //Current Summary columns
    for (var i = 0 ; i < Band._SummaryColumns.length; i++) {
      var col = Band._SummaryColumns[i];

      this.SummaryColumns.push(new Singular.SGrid.SummaryColumn(self, Band.ChildBands[0], col, i));
    }
  } else {
    //Normal, non group by band.
    for (var i = 0; i < Band.Columns.length; i++) {
      this.SummaryColumns.push(new Singular.SGrid.SummaryColumn(self, Band, Band.Columns[i], i));
    }
    
  }
  var IsSorted = false;
  this.SortColumns = function () {
    IsSorted = !IsSorted;
    var fn = IsSorted ? 
      function (a, b) { return a.GridColumn.HeaderText > b.GridColumn.HeaderText ? 1 : -1; } :
      function (a, b) { return a.Index > b.Index ? 1 : -1; }
    this.SummaryColumns.sort(fn);
  }
}

Singular.SGrid.GroupColumn = function (Parent, Key, GroupTypeID, TransformParam) {
  var self = this;
  this.Key = ko.observable(Key);
  this.Key.subscribe(function () {
    self.GroupTypeID(1);
    Parent._GroupsChanged = true;
  });
  this.GroupTypeID = ko.observable(GroupTypeID)
  this.GroupTypeID.subscribe(function () {
    self.GroupParam(self.SelectedOption().ParamValue);
    Parent._GroupsChanged = true;
  });
  this.GroupParam = ko.observable(TransformParam);
  this.GroupParam.subscribe(function () {
    Parent._GroupsChanged = true;
  });
  this.Guid = Singular.NewGuid();

  this.GetOptions = ko.computed(function () {
    if (self.Key()) {
      var col = Parent.Columns.Find('Key', self.Key());
      return Singular.SGrid.GroupTypes.Find('ColType', col.Type).Types;
    } else {
      return [];
    }
  }, this);
  this.SelectedOption = function () {
    return self.GetOptions().Find('ID', self.GroupTypeID());
  };
}
Singular.SGrid.SummaryColumn = function (Parent, SummaryBand, GridColumn, Index) {
  var self = this;
  self.Index = Index
  self.Expanded = ko.observable(false);
  self.GridColumn = GridColumn;
  GridColumn = GridColumn || {};
  GridColumn._Remove = false;

  var LastSType = GridColumn._SummariseType;

  self.SummaryType = ko.observable(LastSType);
  self.SummaryType.subscribe(function (value) {
    if (value == 5) self.Key(SummaryBand.Columns[0].Key);
    if (LastSType == 5) self.Key(null);
    LastSType = value;
    self.Unlink();
  })
  //Key of the Child Column to Summarise
  self.Key = ko.observable(GridColumn.OrigCol ? GridColumn.OrigCol.Key : null);
  self.Key.subscribe(function (Key) {
    self.Unlink()
  });
   
  //Filtering / Cross tab
  self.FilterType = ko.observable();
  self.FilterColumn = ko.observable();
  self.Filter = ko.observable();
  self.ConditionType = ko.observable();//Filter / Cross tab

  if (GridColumn._ && GridColumn._.CrossTabColumn) {
    self.ConditionType(2);
    self.FilterColumn(GridColumn._.CrossTabColumn.Key);
  }

  var LastFilterValue = self.FilterColumn();
  self.FilterColumn.subscribe(function (value) {
    if (value && self.ConditionType() == 1) {
      self.Filter(new Singular.SGrid.FilterInfo(SummaryBand.Columns.Find('Key', value)));
    } else {
      self.Filter(null);
    }
    if (self.ConditionType() == 2) {
      self.Unlink();
    }
    if (!value && LastFilterValue) self.ConditionType(null);
    LastFilterValue = value;
  });
  
  self.Visible = ko.observable(self.GridColumn ? GridColumn.Visible : true);
  self.Visible.subscribe(function (value) {
    if (self.GridColumn) {
      self.GridColumn.SetVisible(value);
    }
  });
  self.Hidden = ko.observable(GridColumn._ ? GridColumn._.CrossTabLink : false);
    
  self.Total = ko.observable();
  self.Average = ko.observable();
  SummaryTypeChanged();

  self.Total.subscribe(function (value) {
    SetSummaryType(value ? Singular.SGrid.SummaryTypes.Sum : null);
  });
    
  self.Average.subscribe(function (value) {
    SetSummaryType(value ? Singular.SGrid.SummaryTypes.Average : null);
  });

  var summaryIsChanging = false;

  function SetSummaryType(SummaryType) {
    if (!summaryIsChanging) {
      summaryIsChanging = true;
      GridColumn.SummaryType = SummaryType;
      SummaryTypeChanged();
      GridColumn.Band.ResetTotals(true);
      summaryIsChanging = false;
    }
  }

  function SummaryTypeChanged() {
    self.Total(GridColumn ? GridColumn.SummaryType == Singular.SGrid.SummaryTypes.Sum : false);
    self.Average(GridColumn ? GridColumn.SummaryType == Singular.SGrid.SummaryTypes.Average : false);
  }

  //Setup
  if (GridColumn._SummaryFilter) {
    self.ConditionType(1);
    var sf = GridColumn._SummaryFilter;
    self.FilterColumn(sf.Column.Key);
    self.Filter().FilterType(sf.FilterType());
    self.Filter().FilterValue(sf.FilterValue());
  }
 

  self.Convert = function () {
    //if (Type == 1) {
    //  self.Filter(new Singular.SGrid.FilterInfo(SummaryBand.Columns.Find('Key', value))
    //} else if (Type == 2) {

    //}
  }

  self.Unlink = function () {
    if (self.GridColumn) {
      self.GridColumn._Remove = true;
      
      //The cross tab column has changed, remove all the summaries linked to it.
      self.GridColumn.Band.Columns.Iterate(function (Col) {
        if (Col._.CrossTabLink == self.GridColumn) Col._Remove = true;
      });
      self.GridColumn = null;
    }
  }
   
  self.GetTypes = function () {
    var fList = [];
    for (var i = 1; i < Singular.SGrid.SummaryTypes.Items.length; i++) {
      /*if (i != 5)*/ fList.push({ ID: i, Text: Singular.SGrid.SummaryTypes.Items[i].Text })
    }
    return fList;
  }
  self.GetAllowedColumns = function () {
    var fList = [], st = self.SummaryType();
    for (var i = 0; i < SummaryBand.Columns.length && st; i++) {
      var col = SummaryBand.Columns[i];
      if ((st != 1 && st != 2) || col.Type == 'n') fList.push(col);
    }
    return fList;
  }
  self.GetFilterTypes = ko.computed(function () {
    var OrigCol = SummaryBand.Columns.Find('Key', self.FilterColumn());
    return Singular.SGrid.ColumnInfo.GetFiltersForType(OrigCol ? OrigCol.Type : 's');
  });

  //check if it should be expanded or not.
  if (sf || self.Visible.peek() == false) self.Expanded(true);
}

Singular.SGrid.GroupTypes = [
  {
    ColType: 's',
    Types: [
      {
        ID: 1, Desc: 'Value'
      },
      {
        ID: 2, Desc: 'First x characters',
        GetValue: function (Value, NoOfChars) {
          if (Value) {
            NoOfChars = NoOfChars > Value.length ? Value.length : NoOfChars;
            return Value.substring(0, NoOfChars);
          } else {
            return Value;
          }
        }, ParamValue: 1, ParamPrompt: 'No of characters'
      },
      {
        ID: 3, Desc: 'First word',
        GetValue: function (Value) {
          var index = Value.indexOf(' ');
          return index == -1 ? Value : Value.substring(0, index);
        }
      }
    ]
  },
  {
    ColType: 'd',
    Types: [
      {
        ID: 1, Desc: 'Date and Time'
      },
      {
        ID: 2, Desc: 'Date only',
        GetValue: function (Value) {
          Value.setHours(0, 0, 0, 0);
          return Value;
        }, FormatString: 'dd MMM yyyy'
      },
      {
        ID: 3, Desc: 'Month',
        GetValue: function (Value) {
          Value.setDate(1);
          Value.setHours(0, 0, 0, 0);
          return Value;
        }, FormatString: 'MMM yyyy'
      },
      {
        ID: 4, Desc: 'Year',
        GetValue: function (Value) {
          Value.setMonth(0);
          Value.setDate(1);
          Value.setHours(0, 0, 0, 0);
          return Value;
        }, FormatString: 'yyyy'
      }
    ]
  },
  {
    ColType: 'n',
    Types: [
      {
        ID: 1, Desc: 'Value'
      },
      {
        ID: 2, Desc: 'Band',
        GetValue: function (Value, Range) {
          return Math.floor(Value / Range) * Range;
        },
        ParamValue: 1000, ParamPrompt: 'Range', NewType: 's', FormatString: '',
        SetRowValues: function (row, col) {
          var Value = row[col.Key];
          row._SGrid.ValueCache[col.Key] = Value.formatDotNet('#,##0;(#,##0)') + ' - ' + (Value + parseInt(col._DataTransformParam)).formatDotNet('#,##0;(#,##0)');
          row._SGrid.SortCache[col.Key] = Value;
     
        }
      }
    ]
  },
  {
    ColType: 'b',
    Types: [
      {
        ID: 1, Desc: 'Value'
      }
    ]
  }
];
Singular.SGrid.SummaryTypes = {
  Sum: 1,
  Average: 2,
  Min: 3,
  Max: 4,
  Count: 5,
  First: 6,
  Last: 7,
  DistinctCount: 8,
  Items: [{},
    {
      Text: 'Sum of', ColName: 'Sum', Type: 'n', Calc: { Init: function (co) { co.Total = 0; }, Add: function (co, value) { co.Total += value; }, Final: function (co) { return co.Total } }
    },
    {
      Text: 'Avg of', ColName: 'Avg', Type: 'n', Calc: { Init: function (co) { co.Total = 0; co.Count = 0; }, Add: function (co, value) { co.Total += value; co.Count += 1; }, Final: function (co) { return co.Count == 0 ? 0 : co.Total / co.Count; }}
    },
    {
      Text: 'Minimum', ColName: 'Min', Calc: { Init: function (co) { }, Add: function (co, value) { if (co.Min == undefined || value < co.Min) co.Min = value; }, Final: function (co) { return co.Min } }
    },
    {
      Text: 'Maximum', ColName: 'Max', Calc: { Init: function (co) { }, Add: function (co, value) { if (co.Max == undefined || value > co.Max) co.Max = value; }, Final: function (co) { return co.Max } }
    },
    {
      Text: 'Count', ColName: '', Type: 'n', Calc: { Init: function (co) { co.Count = 0; }, Add: function (co, value) { co.Count += 1; }, Final: function (co) { return co.Count; } }
    },
    {
      Text: 'First', ColName: 'First', Calc: { Init: function (co) { }, Add: function (co, value) { if (co.First == undefined) co.First = value }, Final: function (co) { return co.First } }
    },
    {
      Text: 'Last', ColName: 'Last', Calc: { Init: function (co) { }, Add: function (co, value) { co.Last = value; }, Final: function (co) { return co.Last } }
    },
    {
      Text: 'Count of', ColName: 'DCount', Type: 'n', Calc: { Init: function (co) { co.Keys = {}; co.DCount = 0; }, Add: function (co, value) { if (!co.Keys[value]) { co.Keys[value] = true; co.DCount += 1; }; }, Final: function (co) { return co.DCount; } }
    }
  ]
}

Singular.SGrid.CurrentGroupBy = ko.observable(null);
Singular.SGrid.Tooltip = ko.observable('');

//#endregion

//#region Column Properties

Singular.SGrid.ColumnInfo = function (Col, ColIndex) {
  var self = this;
  
  self._FiltersChanged = false;
  //Name
  self.Name = ko.observable(Col.HeaderText);
  self.Name.subscribe(function (value) {
    Col.HeaderText = value;
    Col.Band.SGrid.ReDraw();
  });
  //Text Align
  self.TextAlign = ko.observable(Col.TextAlign);
  self.TextAlign.subscribe(function (value) {
    Col.TextAlign = value;
    Col.Band.SGrid.ReDraw();
  });
  //Format
  self.FormatString = ko.observable(Col.FormatString);
  self.FormatString.subscribe(function (value) {
    Col.FormatString = value;
    Col.ResetCachedText(false, true);
    Col.Band.SGrid.ReDraw();
  });
  //Frozen
  self.Frozen = ko.observable(Col.Band.FrozenColumn == ColIndex);
  self.Frozen.subscribe(function (value) {
    Col.Band.FrozenColumn = value ? ColIndex : -1;
    Col.Band.SGrid.ReDraw();
  });

  self.Filters = ko.observableArray([]);
  self.Filters.subscribe(function () {
    self._FiltersChanged = true;
  });

  self.FilterOperator = ko.observable(Col.FilterOperator);
  self.FilterOperator.subscribe(function () {
    self._FiltersChanged = true;
  });
  self.CanFormat = Col.Type == 'n' || Col.Type == 'd';
  self.Type = Col.Type;
  
  self.AddFilter = function (e) {
    var Target = $(e.target);
    var Filters = [{ Text: '<b>Choose Filter Type:</b>', Selectable: false}].concat(self.GetFilterTypes());
    Filters.push({ Break: true });
    Filters.push({ Text: 'Cancel' });

    Singular.SGrid.CurrentColumnInfo.AddingFilter = new Singular.SGrid.FilterInfo(Col, null, '');

    Singular.ContextMenu.Show(Filters, Target.offset().left + Target.outerWidth() - 200, Target.offset().top + Target.outerHeight(), function (Item, Args) {
      if (Args.length > 2 || !Item.ID || Args[1].target.nodeName != 'INPUT') {
        Singular.ContextMenu.Hide();
        if (Item.ID) {
          self._FiltersChanged = true;
          var NewFilter = Singular.SGrid.CurrentColumnInfo.AddingFilter;
          NewFilter.FilterType(Item.ID);
          self.Filters.push(NewFilter);
          //self.Filters.push(new Singular.SGrid.FilterInfo(Item.ID, Singular.SGrid.CurrentColumnInfo.AddingFilter.Binding.Value()));
        }
      }
      
    });
    Singular.ContextMenu.Current().Left(Target.offset().left + Target.outerWidth() - $('.SGrid-Menu').outerWidth());
  }
  self.AddedFilter = function (elems) {
    $(elems[1]).children('input').focus();
  }
  self.RemoveFilter = function (Filter) {
    self.Filters.remove(Filter);
    self._FiltersChanged = true;
  }
  self.RemoveAll = function () {
    self.Filters([]);
    self._FiltersChanged = true;
  }
  self.GetFilterTypes = ko.computed(function () {
    return Singular.SGrid.ColumnInfo.GetFiltersForType(Col.Type);
  });
  self.RefreshColumn = function () {
    if (self._FiltersChanged) Col.ApplyFilters(self);
    Singular.SGrid.CurrentColumnInfo(null);
  }
}
Singular.SGrid.ColumnInfo.GetFiltersForType = function(Type){
  var fList;
  if (Type == 's') {
    fList = [Singular.SGrid.FilterTypes[0].Filters[0]].concat(Singular.SGrid.FilterTypes[1].Filters);
    fList.insert(4, Singular.SGrid.FilterTypes[0].Filters[1]);
    fList[0].Icon = [5, 1];
    fList[4].Icon = [1, 2];
  } else if (Type == 'd' || Type == 'n') {
    fList = Singular.SGrid.FilterTypes[0].Filters.concat(Singular.SGrid.FilterTypes[2].Filters);
    fList[0].Icon = [3, 0];
    fList[1].Icon = [0, 1];
  } else if (Type == 'b') {
    fList = Singular.SGrid.FilterTypes[3].Filters;
  }
  return fList;
}

Singular.SGrid.FilterInfo = function (Column, FilterType, Value) {
  var self = this;
  self.Guid = Singular.NewGuid();
  self.Column = Column;
  if (Column.Type == 'd' && typeof Value == 'number') Value = new Date(Value);

  var DisplayValue;

  var OnChange = function () {
    //Set the matching function
    if (self.FilterType.peek()) self.MatchFunction = Singular.SGrid.FilterTypes.FindValue('ID', self.FilterType.peek()).Object.Match;
    //Set the processed value
    self.FValue = self.FilterValue.peek();
    DisplayValue = self.FValue == null ? '(blank)' : self.FValue;
    if (Column.Type == 's') {
      self.FValue = self.FValue == null ? '' : self.FValue.toLowerCase();
    } else if (Column.Type == 'd' && self.FValue) {
      self.FValue = self.FValue.getTime();
      DisplayValue = DisplayValue.format(Column.FormatString);
    } else if (Column.Type == 'n' && self.FValue) {
      FormattedValue = DisplayValue.formatDotNet(Column.FormatString);
    }

  }

  self.FilterType = ko.observable(FilterType);
  self.FilterType.subscribe(OnChange);
  self.FilterValue = ko.observable(Value);
  self.FilterValue.subscribe(OnChange);
  self.FValue = null;
  self.MatchFunction = null;

  self.Binding = function () {
    return { Type: Column.Type, Value: self.FilterValue, Format: Column.FormatString };
  }
  self.DisplayValue = function () {
    return DisplayValue;
  }
  OnChange();
}

Singular.SGrid.FilterTypes = [
  {
    ColType: '*',
    Filters: [{
      ID: 1, Text: 'Equals', Icon: [3, 0], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value == FilterValue;
      }},
      {
        ID: 2, Text: 'Does not Equal', Icon: [0, 1], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value != FilterValue;
      } }]
  },
  {
    ColType: 's',
    Filters: [ {
      ID: 100, Text: 'Starts with', Icon: [3, 1], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value.indexOf(FilterValue) == 0;
      }
    }, {
      ID: 101, Text: 'Contains', Icon: [4, 1], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value.indexOf(FilterValue) >= 0;
      }
    }, {
      ID: 102, Text: 'Ends with', Icon: [2, 1], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value.indexOf(FilterValue, Value.length - FilterValue.length) !== -1;
      }
    }, {
      ID: 103, Text: "Doesn't start with", Icon: [6, 1], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value.indexOf(FilterValue) !== 0;
      }
    }, {
      ID: 104, Text: "Doesn't contain", Icon: [0, 2], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value.indexOf(FilterValue) == -1;
      }
    }, {
      ID: 105, Text: "Doesn't end with", Icon: [7, 1], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value.indexOf(FilterValue, Value.length - FilterValue.length) == -1;
      }
    }]
  },
  {
    ColType: 'dn',
    Filters: [{
      ID: 200, Text: 'Less than', Icon: [6, 0], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value < FilterValue;
      }
    }, {
      ID: 201, Text: 'Less than or equal', Icon: [1, 1], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value <= FilterValue;
      }
    }, {
      ID: 202, Text: 'Greater than', Icon: [4, 0], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value > FilterValue;
      }
    }, {
      ID: 203, Text: 'Greater or equal', Icon: [5, 0], TemplateID: 'SGrid-FilterItem', Match: function (Value, FilterValue) {
        return Value >= FilterValue;
      }
    }]
  },
  {
    ColType: 'b',
    Filters: [{
      ID: 300, Text: 'True', Icon: 'fa fa-check', Match: function (Value, FilterValue) {
        return Value;
      }
    }, {
      ID: 301, Text: 'False', Icon: 'fa fa-times', Match: function (Value, FilterValue) {
        return !Value;
      }
    }]
  }

];
Singular.SGrid.FilterOperators = [{ ID: 1, Text: 'all of the above' }, { ID: 2, Text: 'any of the above' }];

Singular.SGrid.CurrentColumnInfo = ko.observable(null);

//#endregion

//#region Save layout

Singular.SGrid.GridLayoutInfo = ko.observable(null);

Singular.SGrid.ShowSaveLayout = function (GridInfo) {

  var IncludeFilters = ko.observable(false),
     ObsLayout = ko.observable(''),
     LayoutName = GridInfo.SelectedLayoutName();
  IncludeFilters.subscribe(GetLayout);

  function GetLayout(IncludeFilters) {
    var Layout = JSON.stringify(GridInfo.Grid.SaveLayout(IncludeFilters));
    GridInfo.Layout = Layout;
    if (Singular.DebugMode) {
      //for developers, replace " with | so it can be copy pasted into code.
      Layout = Layout.replace(/\|/g, "||");
      Layout = Layout.replace(/"/g, "|");
    }
    ObsLayout(Layout);
  }
  GetLayout(false); 
    
  var gli = {};
  gli.SaveType = ko.observable(1); //0=New, 1=Overwrite, 2=Delete
  gli.LayoutName = ko.observable(LayoutName);
  gli.NewLayoutName = ko.observable('');
  gli.IncludeFilters = IncludeFilters;
  gli.VisibleToOthers = ko.observable(false);
  gli.Layout = ObsLayout;
  gli.GridInfo = GridInfo;
  gli.CanDelete = ko.computed(function () {
    var userLayout = GridInfo.LayoutList().Find('LayoutName', this.LayoutName());
    return this.SaveType() == 1 && userLayout && userLayout.Saved && !userLayout.CreatedByOtherUser;
  }, gli, { deferEvaluation: true });

  Singular.SGrid.GridLayoutInfo(gli);

  Singular.ShowDialog($('#SaveLayoutOptions')[0], { Custom: true });

}

Singular.SGrid.DoSaveLayout = function () {
  //Make sure everything is filled in.
  var gli = Singular.SGrid.GridLayoutInfo(),
      lList = gli.GridInfo.LayoutList(),
      Name = '';

  if (gli.SaveType() == 0) {
    Name = gli.NewLayoutName().trim();
    if (Name == '') {
      alert('Please select a layout name.');
      return;
    }
    
    for (var i = 0; i < lList.length; i++) {
      if (Name.toLowerCase() == lList[i].LayoutName.toLowerCase()) {
        alert('Please select a name that has not been used.');
        return;
      }
    }
  } else if (gli.SaveType() == 1) {
    Name = gli.LayoutName();
  }
  //Save to database
  Singular.CallServerMethod(Singular.SGrid.ApiPath, 'SaveLayout',
    { UniqueKey: gli.GridInfo.UniqueKey(), LayoutName: Name, LayoutInfo: gli.GridInfo.Layout, VisibleToOthers: gli.VisibleToOthers() },
    function (result) {
      if (result.Success) {
        
        //Update the client data.
        if (gli.SaveType() == 0) {
          gli.GridInfo.LayoutList.push({ LayoutName: Name, LayoutInfo: gli.GridInfo.Layout, Saved: true, ChildList: [] });
        } else if (gli.SaveType() == 1) {
          var layout = lList.Find('LayoutName', Name);
          layout.LayoutInfo = gli.GridInfo.Layout;
          layout.Saved = true;
        }
        gli.GridInfo.SelectedLayoutName(Name);

        //Close the dialog.
        Singular.SGrid.GridLayoutInfo(null);
        $('#SaveLayoutOptions').hide();
      } else {
        alert('Error: ' + result.ErrorText);
      }
    })
}

Singular.SGrid.DeleteLayout = function () {

  var gli = Singular.SGrid.GridLayoutInfo();

  Singular.CallServerMethod(Singular.SGrid.ApiPath, 'SaveLayout', { UniqueKey: gli.GridInfo.UniqueKey(), LayoutName: gli.LayoutName() }, function (data) {
    var layoutList = gli.GridInfo.LayoutList,
        userLayout = layoutList().Find('LayoutName', gli.LayoutName());
    layoutList.remove(userLayout);

    //Select first layout
    gli.GridInfo.SelectedLayoutName(layoutList()[0].LayoutName);

    //Close the dialog.
    Singular.SGrid.GridLayoutInfo(null);
    $('#SaveLayoutOptions').hide();

  }, true);

}

Singular.SGrid.CancelSaveLayout = function() {
  Singular.SGrid.GridLayoutInfo(null);
  $('#SaveLayoutOptions').hide();
}

var GridLayoutInfo = ko.observable();

//#endregion

//#endregion