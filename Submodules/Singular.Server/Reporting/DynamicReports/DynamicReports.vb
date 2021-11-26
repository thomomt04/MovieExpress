Namespace Reporting.Dynamic

  Public Class Settings

    Public Shared Property DynamicReportsAutoSchema As String = "QuickReports"

    Public Shared Property DynamicReportsAllowedSchemas As New List(Of String) From {"RptProcs"}

    Public Shared Property DropDowns As New DynamicDropDownList

		Public Shared Property DynamicReportDatabaseConnectionString As String = ""

		'Added to support project level encryption
		Public Shared Property DynamicReportOverrideClass As Type = Nothing

  End Class

  Public Class DynamicReportCriteria
    Inherits ReportCriteria
    Implements Web.Data.JS.ITypeRenderer

    Private mParameterList As ROParameterList

    <System.ComponentModel.Browsable(True)>
    Public Overrides ReadOnly Property ParameterList As ROParameterList
      Get
        If ReportParent Is Nothing Then
          If mParameterList Is Nothing Then
            mParameterList = ROParameterList.NewROParameterList
          End If
          Return mParameterList
        Else
          Return CType(ReportParent, DynamicReport).ROParameterList
        End If
      End Get
    End Property

    Protected Friend Overrides Function AddParameters(CProc As CommandProc) As DataTable

      Dim InfoTable = New DataTable
      InfoTable.TableName = "Information"
      InfoTable.ExtendedProperties.Add(Data.DataTables.ExtendedProperties.ReportCriteria.ToString, True)

      'Add the parameters to the command.
      For Each param As ROParameter In ParameterList

        CProc.Parameters.AddWithValue("@" & param.ParameterName, param.GetUserValue)

        'Check if the property has a drop down attribute on it.
        'Dim ddaWeb = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DropDownWeb)(pi)
        Dim VisibleColumn As DataColumn = AddColumnToInfoTable(InfoTable, param.ParameterName, GetType(String), param.GetUserValue)
        'If ddaWeb IsNot Nothing Then
        '  VisibleColumn.ExtendedProperties(Singular.Data.DataTables.AutoGenerateName) = False
        '  VisibleColumn = AddColumnToInfoTable(InfoTable, pi.Name & "_Text", GetType(String), ddaWeb.GetDisplayFromID(ParamValue))
        'End If
        VisibleColumn.Caption = param.DisplayName

      Next

      ProcessInfoTable(InfoTable)

      If InfoTable.Columns.Count > 0 Then
        InfoTable.Rows.Add(InfoTable.NewRow())
      End If

      Return InfoTable

    End Function

    Public Sub RenderData(Inst As Object, JW As Web.Data.JSonWriter, Member As Web.Data.JS.ObjectInfo.Member, ByRef UseDefaultRenderer As Boolean) Implements Web.Data.JS.ITypeRenderer.RenderData

    End Sub

    Public Sub RenderModel(JW As Web.Utilities.JavaScriptWriter, Member As Web.Data.JS.ObjectInfo.Member) Implements Web.Data.JS.ITypeRenderer.RenderModel
      JW.Write("CreateTypedProperty(self, '" & Member.JSonPropertyName & "', DynamicCriteriaObject, false);")
    End Sub

    Public ReadOnly Property TypeRenderingMode As Web.Data.JS.RenderTypeOption Implements Web.Data.JS.ITypeRenderer.TypeRenderingMode
      Get
        Return Web.Data.JS.RenderTypeOption.RenderChildTypesOnly
      End Get
    End Property

    Public ReadOnly Property RendersData As Boolean Implements Web.Data.JS.ITypeRenderer.RendersData
      Get
        Return False
      End Get
    End Property

  End Class

  Public Class DynamicReport
    Inherits ReportBase(Of DynamicReportCriteria)

    Private mReportInfo As Report
    Private mROParameterList As ROParameterList

    Public Sub New(ReportInfo As Report)
      SetupDynamic(ReportInfo)
    End Sub

    Public Sub New(Report As DynamicReport, Criteria As DynamicReportCriteria)
      SetupDynamic(Report.mReportInfo)

      If Criteria IsNot Nothing Then
        'New Criteria is passed in on the grid report, update the default values with the values from the new criteria.
        For Each Param As ROParameter In Criteria.ParameterList
          Dim DefaultParam As ROParameter = mROParameterList.Find(Param.ParameterName)
          DefaultParam.SelectedValue = Param.SelectedValue
        Next
      End If
    End Sub

    Private Sub SetupDynamic(ReportInfo As Report)
      mReportInfo = ReportInfo
      mROParameterList = mReportInfo.GetROParameterList
    End Sub

    Public Overrides ReadOnly Property ReportName As String
      Get
        Return mReportInfo.DisplayName
      End Get
    End Property

    Public Overrides ReadOnly Property UniqueKey As String
      Get
        Return mReportInfo.DynamicReportID
      End Get
    End Property

    'Friend ReadOnly Property ReportInfo As Report
    '  Get
    '    Return mReportInfo
    '  End Get
    'End Property

    Friend ReadOnly Property ROParameterList As ROParameterList
      Get
        Return mROParameterList
      End Get
    End Property

    Public Overrides ReadOnly Property GridInfo As GridInfo
      Get
        Dim GI As New GridInfo(Me)
        GI.UniqueKey = "RptDynamic_" & mReportInfo.StoredProcedureName
        Return GI
      End Get
    End Property

    Protected Overrides Sub SetupCommandProc(cmd As CommandProc)
      If Settings.DynamicReportDatabaseConnectionString <> "" Then
        cmd.ConnectionString = Settings.DynamicReportDatabaseConnectionString
      End If
      cmd.CommandText = mReportInfo.StoredProcedureName
      cmd.CommandTimeout = 0 'This should be stored in the DynamicReport table.
    End Sub
  End Class

End Namespace
