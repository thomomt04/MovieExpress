Imports System.ComponentModel

Namespace Web

  Namespace CustomControls.SGrid

    Public Enum SummaryType
      Sum = 1
      Average = 2
      Min = 3
      Max = 4
      Count = 5
      First = 6
      Last = 7
      DistinctCount = 8
    End Enum

  End Namespace

  Public MustInherit Class SGridInfo
    Implements Web.Data.JS.ITypeRenderer

    Public Sub New(UniqueName As String, Optional DefaultLayout As String = "")
      Me.UniqueKey = UniqueName
      AddLayout("Default", DefaultLayout)
    End Sub

    Private Sub New()

    End Sub

    Public Property UniqueKey As String

    Public Overridable Property AllowPrint As Boolean
    Public Overridable Property AllowDataExport As Boolean
    Public Property AllowGraph As Boolean = True
    Public Property AllowSaveLayout As Boolean = True
    Public Property ShowHeader As Boolean = True

    Public MustOverride ReadOnly Property FriendlyName As String
    <Browsable(False)>
    Public Overridable ReadOnly Property ExportFileName As String
      Get
        Return FriendlyName
      End Get
    End Property

#Region " Layouts "

    Private mLayouts As New List(Of LayoutInfo)
    <Browsable(False)>
    Public ReadOnly Property LayoutList As List(Of LayoutInfo)
      Get
        Return mLayouts
      End Get
    End Property

    Public Class LayoutInfo
      Public Property LayoutName As String
      Public Property Layout As String
    End Class

    Public Property SelectedLayoutName As String = "Default"

    Public Function AddLayout(LayoutName As String, Layout As String) As SGridInfo
      mLayouts.Add(New LayoutInfo With {.LayoutName = LayoutName, .Layout = Layout})
      Return Me
    End Function

#End Region

#Region " Lib Methods "

    Public Overridable ReadOnly Property GetDataParams As Object
      Get
        Return Nothing
      End Get
    End Property

    Protected Friend Overridable Function GetInstance(Arguments As Object) As SGridInfo
      Return Me
    End Function

    Public Shared Function GetGridInfo(Arguments As Object) As SGridInfo

      Dim ObjectTypeString As String = Arguments.ObjectType
      Dim ObjectType As Type = Type.GetType(ObjectTypeString)

      Dim gi As SGridInfo = Activator.CreateInstance(ObjectType, True)
      gi = gi.GetInstance(Arguments.GetDataParams)
      If Arguments.LayoutName IsNot Nothing Then
        gi.SelectedLayoutName = Arguments.LayoutName
      End If
      Return gi

    End Function

    Public MustOverride Function GetData(Arguments As Object) As DataSet

#End Region

#Region " Type Info "

    Public ReadOnly Property ClassType As String
      Get
        Return Singular.ReflectionCached.GetCachedType(Me.GetType).DotNetTypeName
      End Get
    End Property

    Public Sub RenderData(Instance As Object, JW As Web.Data.JSonWriter, Member As Web.Data.JS.ObjectInfo.Member, ByRef UseDefaultRenderer As Boolean) Implements Web.Data.JS.ITypeRenderer.RenderData

    End Sub

    Public Sub RenderModel(JW As Web.Utilities.JavaScriptWriter, Member As Web.Data.JS.ObjectInfo.Member) Implements Web.Data.JS.ITypeRenderer.RenderModel
      JW.Write("CreateTypedROProperty(self, '" & Member.JSonPropertyName & "', SGridInfo, false);")
    End Sub

    Public ReadOnly Property RendersData As Boolean Implements Web.Data.JS.ITypeRenderer.RendersData
      Get
        Return False
      End Get
    End Property

    Public ReadOnly Property TypeRenderingMode As Web.Data.JS.RenderTypeOption Implements Web.Data.JS.ITypeRenderer.TypeRenderingMode
      Get
        Return Web.Data.JS.RenderTypeOption.RenderNoTypes
      End Get
    End Property

#End Region

#Region " Options "

    Public Property Options As New GridOptions

    Public Class GridOptions
      Public Property AllowGroupBy As Boolean = True
      Public Property AllowEdit As Boolean = False
      Public Property OnCellEditFunction As String = ""
      Public Property AfterCellEditFunction As String = ""
      Public Property OnFilterFunction As String = ""
      Public Property AlwaysShowHeaderFilters As Boolean = False
      Public Property SortColumnChooser As Boolean = False
      Public Property DefaultSummaryType As CustomControls.SGrid.SummaryType = CustomControls.SGrid.SummaryType.Sum

      ''' <summary>
      ''' JS function that will return a color to conditionally color rows. return null for default. Fn(RowInfo, IsAlt, OrigColor)
      ''' </summary>
      Public Property RowColorFunction As String = ""

      ''' <summary>
      ''' JS Function to modify the data. Fn(Data, Schema, Options)
      ''' </summary>
      Public Property DataTransformFunction As String = ""

      ''' <summary>
      ''' JS function that will return the window height less x height in order for the page to fill the screen
      ''' </summary>
      Public Property AutoHeightFunction As String = ""

      ''' <summary>
      ''' JS Function to set default grid styles
      ''' </summary>
      Public Property StyleGridFunction As String = ""

      ''' <summary>
      ''' JS Function to allow context menu items to be created. Fn(ClickedElement, MenuItemsArray)
      ''' </summary>
      Public Property ContextMenuFunction As String = ""

      ''' <summary>
      ''' Set this to false if you don't want the data to be fetched on page load, and will call Property().FetchData manually.
      ''' </summary>
      Public Property FetchDataOnLoad As Boolean = True

      ''' <summary>
      ''' Function that will return an object with arguments to pass to the server.
      ''' </summary>
      Public Property GetArgumentsFunction As String = ""

      ''' <summary>
      ''' Called when the grid has created its bands initially, and whenever a group is created / removed. Fn(Grid, ChangedBand, IsAdding)
      ''' </summary>
      Public Property AfterLayoutFunction As String = ""

    End Class

#End Region

  End Class

End Namespace

Namespace Reporting

  Public Class GridInfo
    Inherits Web.SGridInfo

    Public Class DataParams
      Public Property ReportType As String
      Public Property ReportID As String
      Public Property Criteria As ReportCriteria
    End Class

    Protected mReport As IReport
    Private mDataParams As DataParams

    Public Sub New(Report As IReport, Optional DefaultLayout As String = "")
      MyBase.New("rpt" & Report.ReportName, DefaultLayout)
      mReport = Report
    End Sub

    Private Sub New()
      MyBase.New("")
    End Sub

    <System.ComponentModel.Browsable(False)>
    Public ReadOnly Property Report As IReport
      Get
        Return mReport
      End Get
    End Property

    Private mAllowPrint As Boolean? = Nothing
    Private mAllowDataExport As Boolean? = Nothing

    Public Overrides Property AllowPrint As Boolean
      Get
        If mAllowPrint Is Nothing Then
          Return mReport.AllowDataExport
        Else
          Return mAllowPrint
        End If
      End Get
      Set(value As Boolean)
        mAllowPrint = value
      End Set
    End Property

    Public Overrides Property AllowDataExport As Boolean
      Get
        Return mReport.AllowDataExport
      End Get
      Set(value As Boolean)

      End Set
    End Property

    ''' <summary>
    ''' Data transform function. Use this if you want to remove tables from the dataset or modify data / schema.
    ''' </summary>
    Public GetDataTransform As Func(Of DataSet, Object, DataSet)

    Public Overrides ReadOnly Property GetDataParams As Object
      Get
        If mDataParams Is Nothing Then
          mDataParams = New DataParams With {.ReportType = Singular.ReflectionCached.GetCachedType(mReport.GetType).DotNetTypeName, .Criteria = mReport.ReportCriteriaGeneric}
        End If
        Return mDataParams
      End Get
    End Property

    Protected Friend Overrides Function GetInstance(Arguments As Object) As Web.SGridInfo

      Dim ReportType As String = Arguments.ReportType
      Dim ReportID As String = Arguments.ReportID

      Dim Report As Singular.Reporting.IReport = Nothing
      Dim Criteria As Singular.Reporting.ReportCriteria = Nothing
      Dim CreateDynamicReport As Boolean = False

      If Singular.Misc.IsNullNothingOrEmpty(ReportID) Then
        'Check if has access
        If Singular.Reporting.ProjectReportHierarchy.GetReport(Type.GetType(ReportType)) IsNot Nothing Then
          Report = Activator.CreateInstance(Type.GetType(ReportType))
          Criteria = Report.ReportCriteriaGeneric
        End If
      Else
        Criteria = New Dynamic.DynamicReportCriteria
        CreateDynamicReport = True
      End If

      If Criteria IsNot Nothing Then

        If Arguments.Criteria IsNot Nothing Then
          Dim d As New Singular.Web.Data.JS.StatelessJSSerialiser(Criteria)
          d.Deserialise(Arguments.Criteria)
        End If

        If CreateDynamicReport Then
          Report = Activator.CreateInstance(Type.GetType(ReportType), Singular.Reporting.ProjectReportHierarchy.GetReport(ReportID), Criteria)
          'Report = New Dynamic.DynamicReport(Singular.Reporting.ProjectReportHierarchy.GetReport(ReportID), Criteria)
        End If

        Dim gi = Report.GridInfo
        gi.mDataParams = New DataParams With {.ReportType = ReportType, .ReportID = ReportID, .Criteria = Report.ReportCriteriaGeneric}
        Return gi

      End If
      Return Nothing

    End Function

    Public Overrides Function GetData(Arguments As Object) As DataSet
      Dim Data = mReport.GetData
      If GetDataTransform IsNot Nothing Then
        Return GetDataTransform(Data, Arguments)
      Else
        Return Data
      End If
    End Function

    Public Overrides ReadOnly Property FriendlyName As String
      Get
        Return mReport.ReportName
      End Get
    End Property

    Public Overrides ReadOnly Property ExportFileName As String
      Get
        Return mReport.GetExportFileName
      End Get
    End Property
  End Class

End Namespace