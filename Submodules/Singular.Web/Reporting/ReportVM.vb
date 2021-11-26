Namespace Reporting

  <Serializable()>
  Public Class ReportVM
    Inherits ViewModel(Of ReportVM)

    Private mReport As Singular.Reporting.IReport

    Public ReadOnly Property Report As Singular.Reporting.IReport
      Get
        Return mReport
      End Get
    End Property

    Protected Overrides Sub Setup()
      MyBase.Setup()

      If Page.Request.QueryString("Type") IsNot Nothing Then
        Dim ReportType As Type = Singular.Reporting.GetReportType(Page.Request.QueryString("Type"))
        'Check if access denied.

        Dim Report As Singular.Reporting.IReport = Activator.CreateInstance(ReportType)
        If GetType(Singular.Reporting.IStandAloneReport).IsAssignableFrom(ReportType) AndAlso CType(Report, Singular.Reporting.IStandAloneReport).HasAccess Then
          mReport = Report
        End If

        If mReport Is Nothing AndAlso Singular.Reporting.ProjectReportHierarchy.GetReport(ReportType) IsNot Nothing Then
          mReport = Activator.CreateInstance(ReportType)
        End If

        If mReport IsNot Nothing Then
          mReport.ReportCriteriaGeneric.SetCriteriaValues(Page.Request.QueryString)
        End If

      ElseIf Page.Request.QueryString("Key") IsNot Nothing Then
        Dim Report As Singular.Reporting.IReport = Singular.Reporting.ProjectReportHierarchy.GetReport(Page.Request.QueryString("Key"))
        If Report IsNot Nothing Then
          Dim DRType As Type = GetType(Singular.Reporting.Dynamic.DynamicReport)
          If Singular.Reporting.Dynamic.Settings.DynamicReportOverrideClass IsNot Nothing Then
            DRType = Singular.Reporting.Dynamic.Settings.DynamicReportOverrideClass
          End If
          mReport = Activator.CreateInstance(DRType, Report, Nothing)
        End If
      End If

      If mReport IsNot Nothing AndAlso mReport.GridInfo IsNot Nothing Then
        'Add the available grid layouts
        ClientDataProvider.AddDataSource("GridLayoutList", Reporting.ROGridUserInfoList.GetROGridUserInfoList(mReport.GridInfo, True), False)
        'Add the grid reporting js file
        Page.LateResources.Add(Singular.Web.Scripts.RenderScriptGroup(Singular.Web.Scripts.ScriptGroupType.GridReport.ToString).ToString)

      End If

      ValidationMode = Singular.Web.ValidationMode.OnLoad

    End Sub

    Protected Overrides Sub HandleCommand(Command As String, CommandArgs As Singular.Web.CommandArgs)
      MyBase.HandleCommand(Command, CommandArgs)

      Select Case Command

        Case "PDF"
          SendReportToBrowser(Singular.Reporting.ReportDocumentType.PDF)

        Case "Word"
          SendReportToBrowser(Singular.Reporting.ReportDocumentType.Word)

        Case "Data"
          SendReportToBrowser(Singular.Reporting.ReportDocumentType.ExcelData)
         
        Case "Custom"
          Dim Result = Report.ExecCustomButton(New Guid(CStr(CommandArgs.ClientArgs)))
          If TypeOf Result Is Singular.Documents.TemporaryDocument Then
            SendFile(CType(Result, Singular.Documents.TemporaryDocument))
          ElseIf TypeOf Result Is Singular.Documents.Document Then
            SendFile(CType(Result, Singular.Documents.Document))
          Else
            Dim msg As Singular.Message = Result
            AddMessage(msg.MessageType, msg.MessageTitle, msg.Message).FadeAfter = msg.FadeAfter
          End If


      End Select

      Report.ResetData()

    End Sub

    Public Sub SendReportToBrowser(OutputType As Singular.Reporting.ReportDocumentType)

      Dim rfi = Report.GetDocumentFile(OutputType)
      If rfi.FileStream IsNot Nothing Then
        SendFile(rfi.FileName, rfi.FileBytes, True)
      Else
        AddMessage(MessageType.Information, "No Data", "No data to display").FadeAfter = 300
      End If
    End Sub

  End Class

End Namespace


