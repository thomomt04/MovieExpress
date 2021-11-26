Singular.GridChart = function (Element, GridInfo, DesignMode, Layout) {
  var self = {},
    ChartElement,
    DoChanges = true,
    ChartType,
    Grid = GridInfo.Grid;

  var ValueChanged = function () {
    if (DoChanges && CreateChart && self.IsValid()) CreateChart();
  }

  self.IsGrouped = Grid.Bands[0].GroupByInfo != null;
  self.DisplayColumns = [];

  self.ValueColumns = [];
  self.GetValueColumns = function (Series) {
    var List = !Series.Primary && Series.ValueColumn() ? [{ Key: '__Remove', Text: '(Remove Column)' }] : [{ Text: 'Select Column' }];
    return List.concat(self.ValueColumns);
  }

  self.GridInfo = GridInfo;
  self.IsDesigning = ko.observable(DesignMode);
  self.ChartTitle = ko.observable('');
  self.ChartType = ko.observable();
  self.DisplayColumn = ko.observable('');
  self.TopNo = ko.observable(20);
  self.SortMode = ko.observable('Original');
  self.ShowOther = ko.observable(true);
  self.SeriesList = ko.observableArray([]);
  self.ShowPercentages = ko.observable(true); //pie only.

  self.AddChartSeries = function (Primary) {
    var s = new Singular.GridChart.Series(self, ValueChanged, Primary);
    self.SeriesList.push(s);
    return s;
  }
  
  self.IsValid = function () { 
    if (!self.IsGrouped && !self.DisplayColumn()) return false;
    if (GetSelectedSeries().length == 0) return false;
    return true;
  };

  self.GetLayoutInfo = ko.computed(function () {
    var SaveObj = {
      ChartTitle: self.ChartTitle(),
      ChartType: self.ChartType(),
      DisplayColumn: self.DisplayColumn(),
      TopNo: self.TopNo(),
      SortMode: self.SortMode(),
      ShowOther: self.ShowOther(),
      ShowPercentages: self.ShowPercentages(),
      Series: []
    }
    GetSelectedSeries().Iterate(function (Item) {
      SaveObj.Series.push(Item.Key);
    });
    return SaveObj;
  }, this, { deferEvaluation: true });

  var Setup = function (AddHandlers) {
    
    if (self.IsGrouped) {
      Grid.Bands[0]._SummaryColumns.Iterate(function (Col) {
        if (Col.Type == 'n' && Col._SummariseType != 5) self.ValueColumns.push({ Key: Col.Key, Text: Col.HeaderText, Col: Col });
      });
      self.ValueColumns.push({ Key: 'Count', Text: 'Count' });
    } else {
      Grid.Bands[0].Columns.Iterate(function (Col) {
        self.DisplayColumns.push({ Key: Col.Key, Text: Col.HeaderText, Col: Col });
        if (Col.Type == 'n') self.ValueColumns.push({ Key: Col.Key, Text: Col.HeaderText, Col: Col });
      });
    }
       
    //if a layout was supplied
    if (Layout) {
      self.ChartTitle(Layout.ChartTitle);
      self.ChartType(Layout.ChartType);
      self.DisplayColumn(Layout.DisplayColumn);
      self.TopNo(Layout.TopNo);
      self.ShowOther(Layout.ShowOther);
      self.SortMode(Layout.SortMode);
      if (Layout.ShowPercentages === false) self.ShowPercentages(false);

      Layout.Series.Iterate(function (Item, i) {
        var s = self.AddChartSeries(i == 0);
        s.ValueColumn(Item);
      });
    } else {
      self.AddChartSeries(true).ValueColumn(self.ValueColumns[0].Key);
    }

    if (AddHandlers) {
      self.ChartTitle.subscribe(ValueChanged);
      self.ChartType.subscribe(ValueChanged);
      self.DisplayColumn.subscribe(ValueChanged);
      self.TopNo.subscribe(ValueChanged);
      self.ShowOther.subscribe(ValueChanged);
      self.SortMode.subscribe(ValueChanged);
      self.ShowPercentages.subscribe(ValueChanged);
    }
      
  }
  Setup(true);

  //make sure the previous selections are still applicable to the new data
  var CheckSchema = function () {
    
    var Reset = false;
    var IsGrouped = Grid.Bands[0].GroupByInfo != null;
    if (self.IsGrouped != IsGrouped) {
      self.IsGrouped = IsGrouped;
      Reset = true;
    } else {
      for (var i = 0; i < self.ValueColumns.length; i++) {
        var Replaced = false;
        for (var j = 0; j < Grid.Bands[0]._SummaryColumns.length; j++){
          var Col = Grid.Bands[0]._SummaryColumns[j];
          if ((Col.Type == 'n' && Col._SummariseType != 5 && Col.Key == self.ValueColumns[i].Key) || self.ValueColumns[i].Key == 'Count') {
            Replaced = true;
          }
        }
        if (!Replaced) {
          Reset = true;
          break;
        }
      }
      
    }

    if (Reset) {
      DoChanges = false;
      self.SeriesList([]);
      self.ValueColumns = [];
      self.DisplayColumn('');
      
      Setup(false);
      DoChanges = true;
     
    }
  }

  self.Show = function () {
    CheckSchema();
    Singular.ShowDialog(Element, { Custom: true, Animate: false });
    ValueChanged();
  }

  var CreateChart = function () {

    var ChartOptions;
    ChartElement = $(Element).find('#divChartContainer');

    if (self.ChartType.peek()) { //Chart type selected
      var SeriesList = GetRootData();
      if (SeriesList.length > 0) { //Display column, and series selected.
        switch (self.ChartType.peek()) {
          case 'Pie Chart':
            ChartOptions = CreatePieChart(SeriesList);
            break;
          case 'Column Chart':
            ChartOptions = CreateSeriesChart(SeriesList, 'column');
            break;
          case 'Bar Chart':
            ChartOptions = CreateSeriesChart(SeriesList, 'bar');
            break;
          case 'Line Chart':
            ChartOptions = CreateSeriesChart(SeriesList, 'line');
            break;
        }
      }
      
    }
    if (ChartOptions) {
      ChartElement.show();
      ChartElement.highcharts(ChartOptions);
    } else {
      ChartElement.hide();
    }
  }

  var CurrentData;
  var GetRootData = function () {
    var ChartData = []
    SeriesList = GetSelectedSeries();
        
    PopulateSeries(Grid.GetCurrentData(), SeriesList, Grid.Bands[0]);

    return SeriesList;
  }

  var GetSelectedSeries = function(){
    var SeriesList = [];
    
    //Get the series that have columns specified.
    self.SeriesList().Iterate(function (Item) {
      var Key = Item.ValueColumn();
      if (Key) {
        var Col = self.ValueColumns.Find('Key', Key);
        SeriesList.push({ Col: Col.Col, name: Col.Text, Key: Col.Key });
      }
    });
    return SeriesList;
  }

  var PopulateSeries = function (GridData, SeriesList, Band) {

    var OtherItem = {},
        HasOtherData = false,
        TopNo = self.TopNo.peek()

    if (SeriesList.length == 0) return;

    //Sort the data
    if (self.SortMode.peek() != 'Original Order') {
      var NewArray = [];
      GridData.Iterate(function (Item) { NewArray.push(Item); });
      GridData = NewArray;
      var av = self.SortMode.peek() == 'First Series Desc' ? -1 : 1, bv = av * -1,
          Key = SeriesList[0].Key;
      GridData.sort(function (a, b) {
        return a[Key] == b[Key] ? 0 : a[Key] > b[Key] ? av : bv;
      });
    }

    SeriesList.Iterate(function (Series) {
      Series.data = [];
      OtherItem[Series.Key] = 0;
    });

    //Populate the chart Data array
    for (var i = 0; i < GridData.length; i++) {
      var GridItem = GridData[i];
      if (!GridItem._SGrid.IsFiltered()) {
        
        if (SeriesList[0].data.length < TopNo) {
          //Normal Items
          var DisplayText = '';
          if (self.IsGrouped) {
            for (var j = 0; j < Band._GroupColumns.length; j++) {
              if (j > 0) DisplayText += ' - ';
              var Col = Band._GroupColumns[j];
              DisplayText += GridItem._SGrid.GetDisplayValue(Col);
            }
          } else {
            DisplayText = GridItem._SGrid.GetValue(Band.Columns.Find('Key', self.DisplayColumn()));
          }

          SeriesList.Iterate(function (Series) {
            Series.data.push({
              name: DisplayText, y: GridItem[Series.Key], ListItem: GridItem,
              drilldown: Band.GroupByInfo
            });
            //ChartItem[Series.Key] = GridItem[Series.Key];
          });

        } else {
          //Additional items
          SeriesList.Iterate(function (Series) {
            OtherItem[Series.Key] += GridItem[Series.Key];
          });
          HasOtherData = true;
        }

      }
    }
            
    SeriesList.Iterate(function (Series) {
      if (HasOtherData && self.ShowOther()) {
        //Add the 'other item'
        Series.data.push({ name: 'Other', y: OtherItem[Series.Key] });
      }
      Series.data.Col = Series.Col;
      Series.data.Band = Band;
      //delete other properties or the graph fucks out.
      delete Series.Key;
      delete Series.Col;
    });

  }

  var OnDrillDown = function (ChartBuilder, SeriesList, ListItem) {
    var NewSeriesList = [],
        ThisBand = SeriesList[0].data.Band,
        ChildBand = ThisBand.ChildBands[0];

    if (ChildBand.GroupByInfo) {
      for (var i = 0; i < SeriesList.length; i++) {
        var SumCol = SeriesList[i].data.Col;
        var OrigCol = SumCol ? SumCol.OrigCol : { Key: 'Count', Text: 'Count' };
        NewSeriesList.push({ Col: OrigCol, Key: OrigCol.Key, name: OrigCol.HeaderText });
      }
      PopulateSeries(ListItem[ChildBand.Key], NewSeriesList, ChildBand);
      BackData.push({ SeriesList: SeriesList, ChartBuilder: ChartBuilder });
      ChartElement.highcharts(ChartBuilder(NewSeriesList, ChartType));
    } else {
      Grid.ScrollToRow(ListItem);
    }
  }

  var BackData = ko.observableArray([]);
  self.CanGoBack = ko.computed(function () {
    return BackData().length > 0;
  });

  self.GoBack = function () {
    var LastChart = BackData.pop();
    ChartElement.highcharts(LastChart.ChartBuilder(LastChart.SeriesList, ChartType));
  }

  var CreatePieChart = function (SeriesList) {

    var CanDrill = Grid.Bands[0].ChildBands.length > 0 && Grid.Bands[0].ChildBands[0].GroupByInfo != undefined,
        Tooltip = self.ShowPercentages() ? '{point.percentage:.1f} %' : '{point.y:,.0f}'
        ChartOptions = {
          chart: {
            type: 'pie',
            events: {
              drilldown: function (e) {
                OnDrillDown(CreatePieChart, SeriesList, e.point.ListItem);
              }
            }
          },
          tooltip: {
            pointFormat: '{series.name}: <b>{point.y:,.0f}</b> - <i>{point.percentage:.1f}%</i>'
          },
          plotOptions: {
            pie: {
              dataLabels: {
                enabled: true,
                format: '{point.name}: ' + Tooltip,
                style: {
                  color: '#000'
                }
              },
              allowPointSelect: !CanDrill,
              showInLegend: false,
              animation: { duration: 300, easing: 'linear' }
            }
          }
        };
    SetCommonChartOptions(ChartOptions);

    ChartOptions.series = SeriesList;
    if (!CanDrill) {
      ChartOptions.series[0].data[0].sliced = true;
      ChartOptions.series[0].data[0].selected = true;
    }
   
    return ChartOptions;

  }
  
  var CreateSeriesChart = function (SeriesList, Type) {
    ChartType = Type;

    var ChartOptions = {
        chart: {
          type: Type,
          events: {
            drilldown: function (e) {
              OnDrillDown(CreateSeriesChart, SeriesList, e.point.ListItem);
            }
          }
        },
        tooltip: {
          shared: true,
          pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y:,.0f}</b><br/>'
        },
        xAxis: {
          labels: { rotation: Type == 'column' ? -45 : 0 },
          categories: []
        }
    };
    SetCommonChartOptions(ChartOptions)

    ChartOptions.series = SeriesList;

    return ChartOptions;
    
  }

  var SetCommonChartOptions = function (ChartOptions) {

    ChartOptions.title = { text: self.ChartTitle() };
    ChartOptions.drilldown = { activeDataLabelStyle: { textDecoration: 'none' } };
    ChartOptions.exporting = { enabled: false }; // exporting sends data over http (not SSL) to a highcharts server.
   
  }
  
  return self;
};
Singular.GridChart.CurrentChart = ko.observable(); //the currently displayed chart, can only be one.
Singular.GridChart.CloseChart = function () {
  $('#ChartPopup').hide();
}
Singular.GridChart.ChartTypes = ['Pie Chart', 'Column Chart', 'Bar Chart', 'Line Chart'];
Singular.GridChart.PieDrillMode = ['Drill down', 'Show donut'];
Singular.GridChart.SortMode = ['Original Order', 'First Series Desc', 'First Series Asc']
Singular.GridChart.Series = function (Parent, ValueChanged, Primary) {
  var self = this;
  this.Primary = Primary;
  this.ValueColumn = ko.observable();
  this.ValueColumn.subscribe(function(Value){
    if (Value == '__Remove'){
      Parent.SeriesList.remove(self);
    }
    ValueChanged();
  });
}
Singular.GridChart.RadialColors = function () {
  Highcharts.getOptions().colors = Highcharts.map(Highcharts.getOptions().colors, function (color) {
    return {
      radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
      stops: [
          [0, Highcharts.Color(color).brighten(-0.15).get('rgb')],
          [1, color]
      ]
    };
  });
}
Singular.GridChart.LoadGraphScripts = function (Success, FirstLoad) {
  if (!$.fn.highcharts) {
    $.getScript(ClientData.HighchartsScriptPath, function () {
      FirstLoad();
      Success();
    }).fail(function () {
      alert('Error loading graphs');
    });
  } else {
    Success();
  }
}

