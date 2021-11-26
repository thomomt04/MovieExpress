Imports Singular.Reporting.Dynamic

Namespace Reporting

  Public Class GridReportVM
    Inherits StatelessViewModel(Of GridReportVM)

    Public Property GridInfo As Singular.Reporting.GridInfo

#Region " Setup "

    Public ReadOnly Property AllowOtherUsersLayout As Boolean
      Get
        Return Singular.Security.HasAccess("Reports.Manage Layouts")
      End Get
    End Property

    Protected Friend Overrides Function HandlePostBack(PostData As String) As Boolean
      'This is called when the grid report is run from the reports screen.
      'It consructs the load parameters json object and does a POST to this page, with the json object in the body of the POST.

      GridInfo = Singular.Web.SGridInfo.GetGridInfo(System.Web.Helpers.Json.Decode(PostData))
      Return False

    End Function

    Protected Overrides Sub Setup()
      MyBase.Setup()

      'This is called when the grid report screen is loaded with a GET and the load parameters are in the query string.
      If Page.Request.QueryString("Type") IsNot Nothing Then

        Dim ReportType = Singular.Reporting.GetReportType(Page.Request.QueryString("Type"))

        If Singular.Reporting.ProjectReportHierarchy.GetReport(ReportType) IsNot Nothing Then
          Dim Report As Singular.Reporting.IReport = Activator.CreateInstance(ReportType)
          GridInfo = Report.GridInfo
          Report.ReportCriteriaGeneric.SetCriteriaValues(Page.Request.QueryString)

        End If

      End If

      ClientDataProvider.AddStringVariable("HighchartsScriptPath",
                                           VirtualPathUtility.ToAbsolute(Singular.Web.Scripts.GetPath(Scripts.ScriptLocation.LibraryVirtualDir, "SGrid/highcharts-custom.js")))

    End Sub

    Public Function IncludeGridReportResources() As String

      Dim ResourceTags As New Text.StringBuilder()
      ResourceTags.AppendLine(Singular.Web.Scripts.RenderScriptGroup(Singular.Web.Scripts.ScriptGroupType.GridReport.ToString).ToString)
      ResourceTags.AppendLine(Singular.Web.Scripts.RenderScriptGroup(Singular.Web.Scripts.ScriptGroupType.SGrid.ToString).ToString)
      ResourceTags.AppendLine(Singular.Web.CSSFile.IncludeFontAwesome().ToString)
      ResourceTags.AppendLine(CSSFile.IncludeSGridStyles.ToString)
      Return ResourceTags.ToString

    End Function

#End Region

  End Class


End Namespace


