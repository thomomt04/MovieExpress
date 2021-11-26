Imports System.Web.UI
Imports Singular.Web.Charting
Imports System.Linq
Imports System.Xml.Linq

Namespace CustomControls

  Public Class LineChartControl(Of ObjectType)
    Inherits Charting.ChartControl(Of Charting.LineChartSettings, ObjectType)

    Public Sub New(Name As String, Settings As Charting.LineChartSettings)
      MyBase.New(Name, Settings)
    End Sub

    Protected Overrides Sub RegisterGraphResources(Page As PageBase)

      For Each s In mChartSettings.mSeriesList
        If s.HighName IsNot Nothing Then
          Page.LateResources.Add(ChartJSFiles.JQOHLCRenderer.ScriptTag.ToString)
          Exit For
        End If
      Next

      If mChartSettings.XAxis.AxisRenderType = Charting.AxisRenderer.DateRenderer Then
        Page.LateResources.Add(ChartJSFiles.JQDateAxisRenderer.ScriptTag.ToString)
      End If

      If mChartSettings.XAxis.AxisRenderType = Charting.AxisRenderer.CategoryRenderer Then
        Page.LateResources.Add(ChartJSFiles.JQPlotcategoryAxisRenderer.ScriptTag.ToString)
      End If

      If mChartSettings.XAxis.TickRenderer = AxisLabelRenderer.CanvasAxisTickRenderer Then
        Page.LateResources.Add(ChartJSFiles.JQCanvasAxisTickRenderer.ScriptTag.ToString)
        Page.LateResources.Add(ChartJSFiles.JQCanvasTextRenderer.ScriptTag.ToString)
      End If

      Page.LateResources.Add(ChartJSFiles.JQPlothighlighter.ScriptTag.ToString)
      Page.LateResources.Add(ChartJSFiles.JQDateAxisRenderer.ScriptTag.ToString)

    End Sub

  End Class

End Namespace

Namespace Charting

  Public Class LineSeries
    Inherits SeriesBase

    ''' <summary>
    ''' The column / property name on the data source that this series will get its data from. 
    ''' </summary>
    Public Property XSourceName As String

    Public Property YSourceName As String

    ''' <summary>
    ''' Column name of the low value for this point. (Will cause this to be a histogram series with high / low values)
    ''' </summary>
    Public Property LowName As String

    ''' <summary>
    ''' Column name of the high value for this point. (Will cause this to be a histogram series with high / low values)
    ''' </summary>
    Public Property HighName As String

    Public Property LineWidth As Integer = -1

    Public Property MarkerOptions As New MarkerOptions

  End Class

  Public Enum MarkerStyle
    [Default] = 1
    diamond = 2
    circle = 3
    square = 4
    x = 5
    plus = 6
    dash = 7
    filledDiamond = 8
    filledCircle = 9
    filledSquare = 10
  End Enum

  Public Class MarkerOptions
    Public Property Size As Integer = -1
    Public Property Show As Boolean = True
    Public Property MarkerStyle As MarkerStyle = Charting.MarkerStyle.Default
  End Class

  Public Class LineChartSettings
    Inherits ChartSettings2Axis

    Public Property SeriesGroupName As String = ""

    Public Property ShowTooltip As Boolean = True
    Public Property TooltipShowX As Boolean = False
    Public Property TooltipShowY As Boolean = True
    Public Property TooltipFormatString As String = ""

    Friend mSeriesList As New List(Of LineSeries)
    Public Property SeriesColours As New List(Of System.Drawing.Color)

    Public Function AddSeries(SourceX As String, SourceY As String, ReadableName As String, Color As System.Drawing.Color) As LineSeries
      Dim ls As New LineSeries With {.XSourceName = SourceX, .YSourceName = SourceY, .ReadableName = ReadableName, .Colour = Singular.Web.Misc.GetHTMLColourCode(Color)}
      mSeriesList.Add(ls)
      Return ls

    End Function

    Protected Overrides Sub WriteJSonChartData(JSW As Data.JSonWriter, Data As System.Data.DataTable)

      If mSeriesList.Count = 0 Then
        JSW.StartArray()
        For Each row As DataRow In Data.Rows
          JSW.StartArray()
          JSW.WriteArrayValue(row(0))
          JSW.WriteArrayValue(row(1))
          JSW.EndArray()
        Next
        JSW.EndArray()

      Else

        If SeriesGroupName <> "" Then
          For Each s In mSeriesList
            JSW.StartArray()

            For Each row As DataRow In Data.Rows
              If s.ReadableName = row(SeriesGroupName) Then
                JSW.StartArray()
                JSW.WriteArrayValue(row(s.XSourceName))
                If s.HighName IsNot Nothing Then
                  JSW.WriteArrayValue(row(s.HighName))
                End If
                If s.LowName IsNot Nothing Then
                  JSW.WriteArrayValue(row(s.LowName))
                End If
                JSW.WriteArrayValue(row(s.YSourceName))
                JSW.EndArray()
              End If
            Next

            JSW.EndArray()
          Next
        Else
          For Each s In mSeriesList
            JSW.StartArray()

            For Each row As DataRow In Data.Rows
              JSW.StartArray()
              JSW.WriteArrayValue(row(s.XSourceName))
              If s.HighName IsNot Nothing Then
                JSW.WriteArrayValue(row(s.HighName))
              End If
              If s.LowName IsNot Nothing Then
                JSW.WriteArrayValue(row(s.LowName))
              End If
              JSW.WriteArrayValue(row(s.YSourceName))
              JSW.EndArray()
            Next

            JSW.EndArray()
          Next
        End If

      End If

    End Sub

    Protected Overrides Sub WriteSpecificSettings(jsw As Data.JSonWriter)

      'Series Options
      jsw.StartArray("series")
      For Each s As LineSeries In mSeriesList
        jsw.StartObject()
        If s.HighName IsNot Nothing Then
          jsw.WritePropertyRaw("renderer", "$.jqplot.OHLCRenderer")
        End If
        jsw.WriteProperty("label", s.ReadableName)
        jsw.WriteProperty("color", s.Colour)
        If s.LineWidth >= 0 Then
          jsw.WriteProperty("lineWidth", s.LineWidth)
        End If

        jsw.StartClass("markerOptions")
        With s.MarkerOptions
          jsw.WriteProperty("show", .Show)
          If .MarkerStyle <> MarkerStyle.Default Then
            jsw.WriteProperty("style", .MarkerStyle.ToString)
          End If
          If .Size <> -1 Then
            jsw.WriteProperty("size", .Size)
          End If
        End With
        jsw.EndClass()

        jsw.EndObject()
      Next
      jsw.EndArray()

      'Axes
      jsw.StartClass("axes")

      'x Axis
      XAxis.WriteSettings(jsw)

      'y Axis
      YAxis.WriteSettings(jsw)

      jsw.EndClass()

      If ShowTooltip Then
        jsw.StartClass("highlighter")
        jsw.WritePropertyRaw("show", "true")
        jsw.WritePropertyRaw("sizeAdjust", 7.5)
        If TooltipShowX AndAlso TooltipShowY Then
          jsw.WritePropertyRaw("tooltipAxes ", "'both'")
        Else
          If TooltipShowX Then
						jsw.WritePropertyRaw("tooltipAxes ", "'x'")
          Else
						jsw.WritePropertyRaw("tooltipAxes ", "'y'")
          End If
        End If
        If TooltipFormatString <> "" Then
          jsw.WritePropertyRaw("formatString", "'" & TooltipFormatString & "'")
        End If
        jsw.EndClass()
      End If


      If SeriesColours.Count > 0 Then
        jsw.StartArray("seriesColors")
        For Each col As System.Drawing.Color In SeriesColours
          jsw.WriteArrayValue(col)
        Next
        jsw.EndArray()
      End If

    End Sub

    Protected Overrides Function PreProcessData(Table As DataTable) As DataTable

      mSeriesList.Clear()
      If SeriesGroupName <> "" Then
        Dim newDT As New DataTable
        Dim XAxisColumn As String = Table.Columns(1).ColumnName
        Dim YAxisColumn As String = Table.Columns(2).ColumnName
        Dim SeriesGroups As IEnumerable(Of Object) = (From data In Table.Rows
                                                      Group By SeriesGroupName = data(SeriesGroupName) Into Group)
        For Each sr As Object In SeriesGroups
          Dim s As LineSeries = New LineSeries With {.XSourceName = XAxisColumn, .YSourceName = YAxisColumn, .ReadableName = sr.SeriesGroupName}
          mSeriesList.Add(s)
        Next
      End If

      If Me.XAxis.TicksDisplayMember <> "" And Me.XAxis.TicksValueMember <> "" Then
        Dim TicksDT As New DataTable
        Dim ValueMemberName As String = Table.Columns(Me.XAxis.TicksValueMember).ColumnName
        Dim DisplayMemberName As String = Table.Columns(Me.XAxis.TicksDisplayMember).ColumnName
        Dim Ticks As List(Of Tick) = (From data In Table.Rows
                                      Group By ValueMember = data(ValueMemberName), DisplayMember = data(DisplayMemberName) Into Group
                                      Select New Tick(ValueMember, DisplayMember)).ToList
        Me.XAxis.Ticks = Ticks
      End If

      Return MyBase.PreProcessData(Table)

    End Function

  End Class

  Public Class Tick

    Public Property ValueMember As String = ""
    Public Property DisplayMember As String = ""

    Public Sub New(ValueMember, DisplayMember)

      Me.ValueMember = ValueMember
      Me.DisplayMember = DisplayMember

    End Sub

  End Class

End Namespace


