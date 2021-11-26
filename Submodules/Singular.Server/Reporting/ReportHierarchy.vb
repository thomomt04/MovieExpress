Imports Singular.Reporting.Dynamic

Namespace Reporting

  ''' <summary>
  ''' Allows the Report Menu structure to be created.
  ''' </summary>
  Public MustInherit Class ReportHierarchy

    Public Sub New()
      SetupHeirarchy()
    End Sub

    Protected MustOverride Sub SetupHeirarchy()

    Private mMainSectionList As New List(Of MainSection)

    Protected Function MainSection(Heading As String) As MainSection
      Dim ms As New MainSection
      ms.Heading = Heading
      mMainSectionList.Add(ms)
      Return ms
    End Function

    Private mDynamicSectionList As New List(Of MainSection)
    Private mFetchedDynamicSections As Boolean = False
    Public Property AddDymanicReports As Boolean = False

    ''' <summary>
    ''' By default, dynamic reports that don't have meta data are added to a group with the same name as the schema of the stored proc.
    ''' You can override the name for uncategorised reports with this property.
    ''' </summary>
    Public Property DefaultGroupName As String = Nothing

    ''' <summary>
    ''' Always get the list of dynamic reports from the database. Otherwise the reports must be refreshed by going to the dynamic report setup screen.
    ''' </summary>
    Public Property AlwaysFetchDynamicReports As Boolean = False

    Public Sub ResetDynamicReports()
      mFetchedDynamicSections = False
    End Sub

    Public Function GetMainSections() As List(Of MainSection)
      If AddDymanicReports AndAlso (Not mFetchedDynamicSections OrElse AlwaysFetchDynamicReports) Then
        SyncLock mDynamicSectionList
          mDynamicSectionList = New List(Of MainSection)
					mFetchedDynamicSections = True

					Dim DRType As Type = GetType(DynamicReport)
					If Dynamic.Settings.DynamicReportOverrideClass IsNot Nothing Then
						DRType = Dynamic.Settings.DynamicReportOverrideClass
					End If

					Dim GroupList As ReportGroupList = ReportGroupList.GetReportGroupList(True)
					For Each group As ReportGroup In GroupList
						Dim ms As New MainSection With {.Heading = group.GroupName}
						ms.SecurityRole = group.SecurityRole
						mDynamicSectionList.Add(ms)

						For Each Report As Dynamic.Report In group.ReportList
							ms.Report(Activator.CreateInstance(DRType, Report), Report.SecurityRole)
						Next
					Next
				End SyncLock
      End If

      Return mMainSectionList.Union(mDynamicSectionList).ToList
    End Function

    Public Function GetReport(ReportType As Type) As IReport
      Return GetReport(ReportType, Nothing)
    End Function

    Public Function GetReport(ReportKey As String) As IReport
      Return GetReport(Nothing, ReportKey)
    End Function

    Private Function GetReport(ReportType As Type, ReportKey As String) As IReport

      For Each ms As MainSection In GetMainSections()
        If ms.IsAllowed Then
          For Each ss As SubSection In ms.SubSectionList
            If ss.IsAllowed Then
              For Each rpt As Report In ss.ReportList
                If rpt.IsMatch(ReportType, ReportKey) AndAlso rpt.IsAllowed Then
                  Return rpt.Report
                End If
              Next
            End If
          Next
        End If
      Next

      Return Nothing
    End Function

  End Class

#Region " Container Objects "

  Public Class MainSection

    Public Property Heading As String
    Public Property ImagePath As String
    Public Property SecurityRole As String = ""

    Private mSubSectionList As New List(Of SubSection)

    Public ReadOnly Property SubSectionList As List(Of SubSection)
      Get
        Return mSubSectionList
      End Get
    End Property

    Public Function SubSection(Heading As String) As SubSection
      Dim ss As New SubSection
      ss.Heading = Heading
      mSubSectionList.Add(ss)
      Return ss
    End Function

    Public Function Report(IReport As IReport, Optional SecurityRole As String = "") As Report
      With SubSection("")
        Return .Report(IReport, SecurityRole)
      End With
    End Function

    ''' <summary>
    ''' Checks if the user has access to this section, and if the sub sections have at least 1 item.
    ''' </summary>
    Public Function IsAllowed() As Boolean
      If SecurityRole = "" OrElse Singular.Security.HasAccess(SecurityRole) Then
        'if 1 section is allowed, then show the section.
        For Each ss As SubSection In mSubSectionList
          If ss.IsAllowed Then
            Return True
          End If
        Next
        Return False
      Else
        Return False
      End If
    End Function

  End Class

  Public Class SubSection

    Public Property Heading As String
    Public Property SecurityRole As String = ""

    Private mReportList As New List(Of Report)

    Public ReadOnly Property ReportList As List(Of Report)
      Get
        Return mReportList
      End Get
    End Property

    Public Function Report(IReport As IReport, Optional SecurityRole As String = "") As Report
      Dim r As New Report
      r.Report = IReport
      r.SecurityRole = SecurityRole
      mReportList.Add(r)
      Return r
    End Function

    Public Function IsAllowed() As Boolean
      If SecurityRole = "" OrElse Singular.Security.HasAccess(SecurityRole) Then
        'if 1 report is allowed, then show the section.
        For Each rpt As Report In mReportList
          If rpt.IsAllowed Then
            Return True
          End If
        Next
        Return False
      Else
        Return False
      End If
    End Function

  End Class

  Public Class Report

    Public Property Report As IReport
    Public Property EndsSection As Boolean = False
		Public Property SecurityRole As String = ""

    Public Function IsAllowed() As Boolean
      Return SecurityRole = "" OrElse Singular.Security.HasAccess(SecurityRole)
    End Function

    Friend Function IsMatch(ReportType As Type, ReportKey As String)
      If ReportType IsNot Nothing Then
        Return Report.GetType Is ReportType
      Else
        Return Report.UniqueKey = ReportKey
      End If
    End Function

  End Class

#End Region

End Namespace

