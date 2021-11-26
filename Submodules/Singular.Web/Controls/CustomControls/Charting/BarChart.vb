Imports System.Web.UI
Imports Singular.Web.Charting

Namespace CustomControls

  Public Class BarChartControl(Of ObjectType)
    Inherits Charting.ChartControl(Of Charting.BarChartSettings, ObjectType)

    Public Sub New(Name As String, Settings As Charting.BarChartSettings)
      MyBase.New(Name, Settings)
    End Sub

    Protected Overrides Sub RegisterGraphResources(Page As PageBase)
      Page.LateResources.Add(ChartJSFiles.JQPlotcategoryAxisRenderer.ScriptTag.ToString)
      Page.LateResources.Add(ChartJSFiles.JQPlotBarRenderer.ScriptTag.ToString)
    End Sub

    Protected Overrides Sub AddGraphContainerAttributes(Div As HTMLDiv(Of ObjectType))

      Div.Attributes("data-graph-tooltipID") = Name & "Tooltip"

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers.Div
        .Attributes("id") = Name & "Tooltip"
        .AddClass("ChartTooltip")
      End With

    End Sub

  End Class


End Namespace

Namespace Charting

  Public Class BarSeries
    Inherits SeriesBase

    ''' <summary>
    ''' The column / property name on the data source that this series will get its data from. 
    ''' </summary>
    Public Property SourceName As String

  End Class

  Public Class BarChartSettings
    Inherits ChartSettings2Axis

#Region " Settings "

    Public Sub New()
      XAxis.AxisRenderType = AxisRenderer.CategoryRenderer
    End Sub

    Public Property SeriesList As New List(Of BarSeries)

    Public Function AddSeries(SourceName As String, ReadableName As String, Color As System.Drawing.Color) As BarSeries
      Dim gs As New BarSeries With {.SourceName = SourceName, .ReadableName = ReadableName, .Colour = Singular.Web.Misc.GetHTMLColourCode(Color)}
      SeriesList.Add(gs)
      Return gs
    End Function

    Public Function AddSeries(SourceName As String, ReadableName As String, Color As String) As BarSeries
      Dim gs As New BarSeries With {.SourceName = SourceName, .ReadableName = ReadableName, .Colour = Color}
      SeriesList.Add(gs)
      Return gs
    End Function

#End Region

#Region " Functions "

    Protected Overrides Sub WriteRootJSonData(JSW As Data.JSonWriter, Data As DataTable)

      'Write the X Axis Values
      JSW.StartArray("Axis")
      For Each row As DataRow In Data.Rows
        Dim Val = row(XAxis.SourceName)
        If XAxis.FormatString <> "" Then
          Val = Format(Val, XAxis.FormatString)
        End If
        JSW.WriteArrayValue(Val)
      Next
      JSW.EndArray()

    End Sub

    Protected Overrides Sub WriteJSonChartData(JSW As Data.JSonWriter, Data As DataTable)

      For Each s As BarSeries In SeriesList

        JSW.StartArray()
        For Each row As DataRow In Data.Rows
          JSW.WriteArrayValue(row(s.SourceName))
        Next
        JSW.EndArray()

      Next

    End Sub

    Protected Overrides Sub WriteSpecificSettings(jsw As Data.JSonWriter)

      jsw.StartClass("seriesDefaults")
      jsw.WritePropertyRaw("renderer", "$.jqplot.BarRenderer")
      jsw.WritePropertyRaw("rendererOptions", "{ fillToZero: true }")
      jsw.EndClass()

      'Series Options
      jsw.StartArray("series")
      For Each s As BarSeries In SeriesList
        jsw.StartObject()
        jsw.WriteProperty("label", s.ReadableName)
        jsw.WriteProperty("color", s.Colour)
        jsw.EndObject()
      Next
      jsw.EndArray()

      'Axes
      jsw.StartClass("axes")

      'x Axis
      XAxis.WriteSettings(jsw, Sub()
                                 jsw.WritePropertyRaw("ticks", "GData.Axis")
                               End Sub)

      'y Axis
      YAxis.WriteSettings(jsw)

      jsw.EndClass()

    End Sub

#End Region



  End Class

End Namespace


