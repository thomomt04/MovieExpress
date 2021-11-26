"use strict";

$(document).ready(function () {
  Singular.Graphing.SetupGraphs();
});

Singular.Graphing = (function () {

  var Public = {};

  var Charts = {};

  Public.SetGraphData = function (ChartName, Data) {

    if (Charts[ChartName] && Charts[ChartName].Chart) {
      //the graph exists and has been set up, so redraw it with the new data.
      Charts[ChartName].Data = Data;

      SetupChart(ChartName);

    } else {
      //This is a new graph, just set the data.
      Charts[ChartName] = { Data: Data, Settings: {} };
    }


  };

  Public.SetupGraphs = function () {

    $('[data-graph-settings]').each(function () {

      //Setup the Graph and store a reference to it.
      SetupChart(this.id);

    });

  };

  var SetupChart = function (ChartID) {

    //Graph Data
    var GData = Charts[ChartID].Data;

    //Graph Settings
    var SettingsString = $('#' + ChartID).attr('data-graph-settings');
    var Settings;
    eval('Settings = ' + SettingsString);
    Charts[ChartID].Settings = Settings;

    if ((GData.Axis && GData.Axis.length == 0) || (GData.Data.length ==1 && GData.Data[0].length ==0)) {
      //If there is no x axis data, then hide the chart container.
      $('#' + ChartID).parent().hide();
    } else {
      //If there is data, then show the parent container.
      $('#' + ChartID).parent().show();
      //Plot the chart, set the chart variable
      var OldChart = Charts[ChartID].Chart;
      Charts[ChartID].Chart = $.jqplot(ChartID, GData.Data, Settings);

      if (OldChart) {
        OldChart.destroy();
        Charts[ChartID].Chart.redraw(true);
      } else {
        //First time the chart is being set up.
        //if (Settings.seriesDefaults.renderer == $.jqplot.BarRenderer) {
        SetupBarChartTooltip(ChartID, Settings);
        //}
      }

    }

  }

  var SetupBarChartTooltip = function (ChartID, Settings) {

    var ChartContainer = $('#' + ChartID);
    var Tooltip = $('#' + ChartContainer.attr('data-graph-tooltipID'));

    ChartContainer.bind('jqplotDataHighlight', function (ev, seriesIndex, pointIndex, data) {

      var Chart = Charts[ChartID].Chart;

      if (Chart.__LastIndex == pointIndex && Chart.__LastSeries == seriesIndex) {
        return;
      } else {
        Chart.__LastIndex = pointIndex;
        Chart.__LastSeries = seriesIndex;
      }

      var prefix;
      if (Settings.ToolTipPrefix) {
        prefix = Settings.ToolTipPrefix + ' ';
      } else {
        prefix = '';
      }

      if (Settings.seriesDefaults.renderer == $.jqplot.BarRenderer) {

        //Bar
        var top = ChartContainer.offset().top + Chart.axes.yaxis.u2p(data[1]);
        var left = ChartContainer.offset().left + Chart.axes.xaxis.u2p(data[0]) + Chart.series[seriesIndex]._barNudge;
        Tooltip.html(prefix + parseFloat(data[1]).formatMoney(0, ' '));

      } else {

        //Pie
        var s = Chart.series[seriesIndex];
        var fact = (s._radius) * 0.8 + s.sliceMargin + s.dataLabelNudge;
        var avg = (s._sliceAngles[pointIndex][0] + s._sliceAngles[pointIndex][1]) / 2
        var left = s._center[0] + Math.cos(avg) * fact + ChartContainer.offset().left;
        var top = s._center[1] + Math.sin(avg) * fact + ChartContainer.offset().top;

        Tooltip.html(data[0] + '<br/>' + prefix + parseFloat(data[1]).formatMoney(0, ' '));

      }

      var cssObj = {
        'position': 'absolute',
        'font-weight': 'bold',
        'left': left + 'px', //usually needs more offset here
        'top': (top - 25) + 'px'
      };
      Tooltip.css(cssObj);
      Tooltip.show();
    });

    ChartContainer.bind('jqplotDataUnhighlight', function (ev) {
      var Chart = Charts[ChartID].Chart;
      Tooltip.hide();
      Chart.__LastIndex = -1;
      Chart.__LastSeries = -1;
    });
  };


  return Public;
})();