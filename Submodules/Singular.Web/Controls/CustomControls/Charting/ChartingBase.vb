Imports System.Web.UI
Imports Singular.Web.Scripts

Namespace Charting

  Public Module ChartingSettings

    Public Property JQPlotCSSFile As String = "~/Singular/CSS/jquery.jqplot.min.css"

  End Module

  Friend Class ChartJSFiles

    Private Shared BasePath As String = Scripts.GetPath(ScriptLocation.LibraryVirtualDir, "JQPlot")

    Public Shared Property JQPlotCore As New ScriptFile(BasePath, "jquery.jqplot.js", "jquery.jqplot.min.js")
    Public Shared Property SingularGraphing As New ScriptFile(Scripts.GetPath(ScriptLocation.LibraryEmbedded, "Singular"), "Singular.Graphing.js", "Singular.Graphing.js")

    Public Shared Property JQPlothighlighter As New ScriptFile(BasePath, "jqplot.highlighter.js", "jqplot.highlighter.min.js")
    Public Shared Property JQPlotBarRenderer As New ScriptFile(BasePath, "jqplot.barRenderer.js", "jqplot.barRenderer.min.js")
    Public Shared Property JQPlotcategoryAxisRenderer As New ScriptFile(BasePath, "jqplot.categoryAxisRenderer.js", "jqplot.categoryAxisRenderer.min.js")
    Public Shared Property JQPlotPieRenderer As New ScriptFile(BasePath, "jqplot.pieRenderer.js", "jqplot.pieRenderer.min.js")
    Public Shared Property JQDateAxisRenderer As New ScriptFile(BasePath, "jqplot.dateAxisRenderer.js", "jqplot.dateAxisRenderer.min.js")
    Public Shared Property JQOHLCRenderer As New ScriptFile(BasePath, "jqplot.ohlcRenderer.js", "jqplot.ohlcRenderer.min.js")
    Public Shared Property JQCanvasAxisTickRenderer As New ScriptFile(BasePath, "jqplot.canvasAxisTickRenderer.js", "jqplot.canvasAxisTickRenderer.min.js")
    Public Shared Property JQCanvasTextRenderer As New ScriptFile(BasePath, "jqplot.canvasTextRenderer.js", "jqplot.canvasTextRenderer.min.js")


  End Class

#Region " Base Chart Control "

  Public MustInherit Class ChartControl(Of SettingsType As ChartSettings, ObjectType)
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of ObjectType)

    Public Overridable Property Name As String
    Public Property Width As Integer = 800
    Public Property Height As Integer = 500

    Public Sub New(Name As String, Settings As SettingsType)
      Me.Name = Name
      mChartSettings = Settings
    End Sub

    Protected mChartSettings As SettingsType
    Public ReadOnly Property ChartSettings As SettingsType
      Get
        Return mChartSettings
      End Get
    End Property

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      RegisterJQPlotCoreResources()

      RegisterGraphResources(Page)


      Dim SetDataJS As String = "Singular.Graphing.SetGraphData('" & Name & "', " & ChartSettings.GetJsonData() & ");"
      System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.GetType, Name & "Startup", SetDataJS, True)

      Dim Div = Helpers.Div
      With Div
        .Style.Width = Width
        .Style("height") = Height & "px"

        .Attributes("id") = Name
        .Attributes("data-graph-settings") = ChartSettings.GetSettingsJSObject

        AddGraphContainerAttributes(Div)

      End With

    End Sub

    Protected Overridable Sub RegisterGraphResources(Page As Singular.Web.PageBase)

    End Sub

    Protected Overridable Sub AddGraphContainerAttributes(Div As Singular.Web.CustomControls.HTMLDiv(Of ObjectType))

    End Sub

    Private Sub RegisterJQPlotCoreResources()

      With CType(Page, IPageBase).LateResources
        .Add("<link rel=""stylesheet"" type=""text/css"" href=""" & Utils.URL_ToAbsolute(JQPlotCSSFile) & """ />")
        .Add("<!--[if lt IE 9]><script type=""text/javascript"" src=""" & Utils.URL_ToAbsolute("~/Singular/Javascript/Graphing/excanvas.min.js") & """></script><![endif]-->")
        .Add(ChartJSFiles.JQPlotCore.ScriptTag.ToString)
        .Add(ChartJSFiles.SingularGraphing.ScriptTag.ToString)
      End With


    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      WriteFullStartTag("div", Singular.Web.Controls.HelperControls.TagType.IndentChildren)

      RenderChildren()

      Writer.WriteEndTag("div", True)

    End Sub

  End Class

#End Region

#Region " Settings Classes "

  <Singular.DataAnnotations.ServerOnly()>
  Public MustInherit Class ChartSettings

#Region " Settings "

    Public Property ChartTitle As String = ""
    Public Property ChartName As String = ""
    Public Property ToolTipPrefix As String = "R"

    Private mLegend As New LegendSettings
    Public ReadOnly Property Legend As LegendSettings
      Get
        Return mLegend
      End Get
    End Property

    Public Property GridBackground As System.Drawing.Color = Drawing.Color.WhiteSmoke

    Public Property GetDataCallBack As Func(Of DataTable)

#End Region

#Region " Functions "

#Region " Data "

    ''' <summary>
    ''' Gets the JSon data needed by the grid, using the series and axis settings.
    ''' </summary>
    Public Function GetJsonData() As String

      Dim jsw As New Singular.Web.Data.JSonWriter
      jsw.StartObject()

      WriteInternal(jsw)

      jsw.EndObject()
      Return jsw.ToString

    End Function

    Private Sub WriteInternal(jsw As Singular.Web.Data.JSonWriter)

      Dim Data = PreProcessData(GetDataCallBack.Invoke)

      'Other data that can be used in settings.
      WriteRootJSonData(jsw, Data)

      'Data to be passed into the graph setup.
      jsw.StartArray("Data")
      WriteJSonChartData(jsw, Data)
      jsw.EndArray()

    End Sub

    Public Sub WriteJSonData(jsw As Singular.Web.Data.JSonWriter, Name As String)

      jsw.StartClass(Name)
      WriteInternal(jsw)
      jsw.EndClass()

    End Sub

    Protected Overridable Function PreProcessData(Table As DataTable) As DataTable
      Return Table
    End Function

    Protected Overridable Sub WriteRootJSonData(JSW As Singular.Web.Data.JSonWriter, Data As DataTable)

    End Sub
    Protected MustOverride Sub WriteJSonChartData(JSW As Singular.Web.Data.JSonWriter, Data As DataTable)

#End Region

#Region " Settings "

    Public Function GetSettingsJSObject() As String

      Dim jsw As New Singular.Web.Data.JSonWriter
      jsw.OutputMode = Singular.Web.Data.OutputType.Javascript
      jsw.StartClass("")

      If ChartTitle <> "" Then
        jsw.WriteProperty("title", ChartTitle)
      End If
      If ChartName <> "" Then
        jsw.WriteProperty("ChartName", ChartName)
      End If
      If ToolTipPrefix <> "" Then
        jsw.WriteProperty("ToolTipPrefix", ToolTipPrefix)
      End If

      'Grid Options
      jsw.StartClass("grid")
      jsw.WriteProperty("background", GridBackground)
      jsw.EndClass()

      'Legend Options
      jsw.StartClass("legend")
      jsw.WriteProperty("show", Legend.Show)
      If Legend.Show Then
        jsw.WriteProperty("location", Legend.Location.ToString)
        jsw.WriteProperty("placement", Legend.Placement.ToString)
      End If

      jsw.EndClass()

      WriteSpecificSettings(jsw)

      jsw.EndClass()

      Return jsw.ToString

    End Function

    Protected MustOverride Sub WriteSpecificSettings(jsw As Singular.Web.Data.JSonWriter)

#End Region

#End Region

  End Class

  Public MustInherit Class ChartSettings2Axis
    Inherits ChartSettings

    Protected mXAxis As New XAxisSettings
    Public ReadOnly Property XAxis As XAxisSettings
      Get
        Return mXAxis
      End Get
    End Property

    Protected mYAxis As New YAxisSettings
    Public ReadOnly Property YAxis As YAxisSettings
      Get
        Return mYAxis
      End Get
    End Property

  End Class

  Public Class SeriesBase

    Public Property ReadableName As String

    Public Property Colour As String

  End Class

  Public Class LegendSettings

    Public Property Show As Boolean = True

    Public Property Location As Position = Position.ne

    Public Property Placement As LegendPlacement = LegendPlacement.outsideGrid

  End Class

  Public MustInherit Class AxisSettings

    Public Property Show As Boolean = True

    Public MustOverride Sub WriteSettings(jsw As Data.JSonWriter, Optional Custom As Action = Nothing)

    Public Property TickRenderer As AxisLabelRenderer = AxisLabelRenderer.CanvasAxisTickRenderer

    Public Property TickOptions As New TickOptions

    Public Sub RenderTickOptions(jsw As Data.JSonWriter)

      If TickRenderer = AxisLabelRenderer.CanvasAxisTickRenderer Then
        'axis renderer to use
        jsw.StartClass("rendererOptions")
        jsw.WritePropertyRaw("tickRenderer", "$.jqplot.CanvasAxisTickRenderer")
        jsw.EndClass()
      End If

      If TickRenderer <> AxisLabelRenderer.None Then
        'axis ui settings
        jsw.StartClass("tickOptions")
        jsw.WriteProperty("fontFamily", TickOptions.FontFamily)
        jsw.WriteProperty("fontSize", TickOptions.FontSize)
        jsw.WriteProperty("angle", TickOptions.Angle)
        'jsw.WritePropertyRaw("ticks", TickOptions.GetTicks)
        jsw.EndClass()
      End If

    End Sub

  End Class

  Public Class TickOptions

    Public Property FontFamily As String = ""
    Public Property FontSize As String = ""
    Public Property Angle As String = ""

  End Class

  Public Enum AxisRenderer
    Normal = 0
    CategoryRenderer = 1
    DateRenderer = 2
  End Enum

  Public Enum AxisLabelRenderer
    None = 0
    CanvasAxisTickRenderer = 1
  End Enum

  Public Class XAxisSettings
    Inherits AxisSettings

    Public Property FormatString As String

    ''' <summary>
    ''' The column / property name on the data source that this series will get its data from. 
    ''' </summary>
    Public Property SourceName As String

    Public Property AxisRenderType As AxisRenderer = AxisRenderer.Normal

    Public Property TicksValueMember As String = ""
    Public Property TicksDisplayMember As String = ""
    Public Property Ticks As New List(Of Tick)

    Public Overrides Sub WriteSettings(jsw As Data.JSonWriter, Optional Custom As Action = Nothing)
      jsw.StartClass("xaxis")
      Select Case AxisRenderType
        Case AxisRenderer.CategoryRenderer
          jsw.WritePropertyRaw("renderer", "$.jqplot.CategoryAxisRenderer")
        Case AxisRenderer.DateRenderer
          jsw.WritePropertyRaw("renderer", "$.jqplot.DateAxisRenderer")
      End Select

      If Me.Ticks.Count > 0 Then
        jsw.StartArray("ticks")
        For Each s In Me.Ticks
          jsw.StartArray()
          jsw.WriteArrayValue(s.ValueMember)
          jsw.WriteArrayValue(s.DisplayMember)
          jsw.EndArray()
        Next
        jsw.EndArray()
      End If

      RenderTickOptions(jsw)

      If Custom IsNot Nothing Then
        Custom.Invoke()
      End If

      jsw.EndClass()
    End Sub

  End Class

  Public Class YAxisSettings
    Inherits AxisSettings

    Private mNumberFormat As New NumberFormat
    Public ReadOnly Property NumberFormat As NumberFormat
      Get
        Return mNumberFormat
      End Get
    End Property

    Public Property MaxValue As Integer = 0

    Public Overrides Sub WriteSettings(jsw As Data.JSonWriter, Optional Custom As Action = Nothing)
      jsw.StartClass("yaxis")
      jsw.WritePropertyRaw("tickOptions", "{ formatter: function(s, v){ return '" & NumberFormat.CurrencySymbol & "' + v.formatMoney(" & NumberFormat.Decimals & ", '" & NumberFormat.ThousandsSeperator & "'); } }")
      If MaxValue <> 0 Then
        jsw.WritePropertyRaw("max", MaxValue)
      End If
      jsw.EndClass()
    End Sub

  End Class

  Public Enum Position
    n
    ne
    e
    se
    s
    sw
    w
    nw
  End Enum

  Public Enum LegendPlacement
    ''' <summary>
    ''' Puts the legend in the grid area
    ''' </summary>
    insideGrid
    ''' <summary>
    ''' Resizes the Grid in order to fit the legend outside the grid, but in the graph canvas area.
    ''' </summary>
    outsideGrid
    ''' <summary>
    ''' Keeps the Grid the same size, and puts the legend outside the graph canvas area. May overlap other elements.
    ''' </summary>
    outside
  End Enum

  Public Class NumberFormat
    Public Property Decimals As Integer = 0
    Public Property ThousandsSeperator As String = " "
    Public Property CurrencySymbol As String = ""
  End Class

#End Region

End Namespace
