Imports Singular.Reporting.Dynamic

Namespace Reporting

  Public Class DynamicReportSetupVM
    Inherits Singular.Web.StatelessViewModel(Of DynamicReportSetupVM)

    Public Property ReportGroupList As ReportGroupList

    <Singular.DataAnnotations.ClientOnly>
    Public Property EditReport As Report

    Protected Overrides Sub Setup()
      MyBase.Setup()

      DeleteMode = Singular.Web.SaveMode.Immediate

      ReportGroupList = ReportGroupList.GetReportGroupList(False)

      ClientDataProvider.AddDataSource("DropDownList", Singular.Reporting.Dynamic.Settings.DropDowns.FetchDatabaseDropDowns, False)
      ClientDataProvider.AddDataSource("SecurityRoleList", New Singular.Web.Security.FlatRoleList, False)
      ClientDataProvider.AddDataSource("SpecialDefaultValue", Singular.CommonData.Enums.GetEnumList(GetType(ReportParameter.SpecialDefaultValue)), False)
      ClientDataProvider.AddDataSource("DefaultValueType", Singular.CommonData.Enums.GetEnumList(GetType(ReportParameter.DefaultValueType)), False)
      ClientDataProvider.AddDataSource(Of ReportParameter.SpecialDefaultValue)(Singular.DataAnnotations.DropDownWeb.SourceType.None)
    End Sub

    Public Shared Function SaveReport(Report As Singular.Reporting.Dynamic.Report, GroupID As Integer, GroupName As String) As Singular.Web.Result

      Return New Singular.Web.Result(
        Function()

          Dim RptList As ReportList = ReportList.NewReportList
          Report.SetGroupInfo(GroupID, GroupName)
          RptList.Add(Report)
          RptList.CheckAllRules()
          RptList = RptList.Save()
          Singular.Reporting.ProjectReportHierarchy.ResetDynamicReports()
          Singular.Reporting.Dynamic.Settings.DropDowns.ResetDatabaseDropDowns()

          Return RptList(0)

        End Function)

    End Function

    Public Shared Function SaveGroups(GroupList As ReportGroupList) As Singular.Web.Result

      Return New Singular.Web.Result(
        Function()

          Dim SavedList = GroupList.Save
          Singular.Reporting.ProjectReportHierarchy.ResetDynamicReports()
          Singular.Reporting.Dynamic.Settings.DropDowns.ResetDatabaseDropDowns()
          Return SavedList

        End Function)

    End Function

  End Class

End Namespace


