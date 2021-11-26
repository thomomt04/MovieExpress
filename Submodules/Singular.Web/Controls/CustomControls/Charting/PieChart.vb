Imports Singular.Web.Charting

Namespace CustomControls

  Public Class PieChartControl(Of ObjectType)
    Inherits Charting.ChartControl(Of Charting.PieChartSettings, ObjectType)

    Public Sub New(Name As String, Settings As Charting.PieChartSettings)
      MyBase.New(Name, Settings)
    End Sub

    Protected Overrides Sub RegisterGraphResources(Page As PageBase)
      Page.LateResources.Add(ChartJSFiles.JQPlotPieRenderer.ScriptTag.ToString)
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

  Public Class PieChartSettings
    Inherits Charting.ChartSettings

    Public Property PieSliceMargin As Integer = 0
    Public Property PieStartAngle As Integer = -90
		Public Property SeriesColours As New List(Of System.Drawing.Color)
		Public Property PercentPrecision As Integer = 2
		Public Property LabelSuffix As String = "%"

    ''' <summary>
    ''' The number of pie slices to show before grouping everything into an 'other' pie slice.
    ''' </summary>
    Public Property TopRecords As Integer = 20

    Protected Overrides Function PreProcessData(Table As System.Data.DataTable) As System.Data.DataTable

      Dim NewTable = Table.Clone

      Dim i As Integer = 0
      Dim OtherTotal As Decimal = 0

      For Each row As DataRow In Table.Rows

        If i < TopRecords Then
          NewTable.ImportRow(row)
        Else
          OtherTotal += row(1)
        End If

        i += 1
      Next

      If OtherTotal <> 0 Then
        Dim OtherRow = NewTable.NewRow
        OtherRow(0) = "Other"
        OtherRow(1) = OtherTotal
        NewTable.Rows.Add(OtherRow)
      End If

      Return NewTable

    End Function

    Protected Overrides Sub WriteJSonChartData(JSW As Data.JSonWriter, Data As System.Data.DataTable)

      JSW.StartArray()
      For Each row As DataRow In Data.Rows

        JSW.StartArray()
        JSW.WriteArrayValue(row(0))
        JSW.WriteArrayValue(row(1))
        JSW.EndArray()

      Next
      JSW.EndArray()

    End Sub

    Protected Overrides Sub WriteSpecificSettings(jsw As Data.JSonWriter)

      jsw.StartClass("seriesDefaults")
      jsw.WritePropertyRaw("renderer", "$.jqplot.PieRenderer")

      jsw.StartClass("rendererOptions")
      jsw.WriteProperty("showDataLabels", True)
      If PieSliceMargin <> 0 Then
        jsw.WriteProperty("sliceMargin", PieSliceMargin)
      End If
      If PieStartAngle <> 0 Then
        jsw.WriteProperty("startAngle", PieStartAngle)
      End If

      If SeriesColours.Count > 0 Then
        jsw.StartArray("seriesColors")
        For Each col As System.Drawing.Color In SeriesColours
          jsw.WriteArrayValue(col)
        Next
        jsw.EndArray()
			End If

			jsw.WriteProperty("dataLabelFormatString", "%." & PercentPrecision & "f " & LabelSuffix)

      jsw.EndClass()

      jsw.EndClass()

    End Sub
  End Class

End Namespace

