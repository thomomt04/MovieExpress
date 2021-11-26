Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Service.Scheduling

  <Serializable()> _
  Public Class ROScheduleProgress
    Inherits SingularReadOnlyBase(Of ROScheduleProgress)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared ScheduleProgressIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ScheduleProgressID, "Schedule Progress", 0)
    ''' <Summary>
    ''' Gets the Schedule Progress value
    ''' </Summary>
    <Display(AutoGenerateField:=False),
   Singular.DataAnnotations.LocalisedDisplay(GetType(Localisation.SingularObservableResources), "ROScheduleProgress_ScheduleProgressID")>
    Public ReadOnly Property ScheduleProgressID() As Integer
      Get
        Return GetProperty(ScheduleProgressIDProperty)
      End Get
    End Property

    Public Shared ScheduleInfoIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.ScheduleInfoID, "Schedule Info", CType(Nothing, Integer?))
    ''' <Summary>
    ''' Gets the Schedule Info value
    ''' </Summary>
    <Display(AutoGenerateField:=False),
   Singular.DataAnnotations.LocalisedDisplay(GetType(Localisation.SingularObservableResources), "ROScheduleProgress_ScheduleInfoID")>
    Public ReadOnly Property ScheduleInfoID() As Integer?
      Get
        Return GetProperty(ScheduleInfoIDProperty)
      End Get
    End Property

    Public Shared ProgressProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Progress, "Progress", "")
    ''' <Summary>
    ''' Gets the Progress value
    ''' </Summary>
    <Display(Name:="Progress", Description:="Current status of the scheduler"),
   Singular.DataAnnotations.LocalisedDisplay(GetType(Localisation.SingularObservableResources), "ROScheduleProgress_Progress")>
    Public ReadOnly Property Progress() As String
      Get
        Return GetProperty(ProgressProperty)
      End Get
    End Property

    Public Shared CreatedDateProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.CreatedDate, "Created Date", New SmartDate(Now()))
    ''' <Summary>
    ''' Gets the Created Date value
    ''' </Summary>
    <Display(Name:="Created Date", Description:="Date the schedule process was executed"),
    Singular.DataAnnotations.LocalisedDisplay(GetType(Localisation.SingularObservableResources), "ROScheduleProgress_CreatedDate")>
    Public ReadOnly Property CreatedDate() As SmartDate
      Get
        Return GetProperty(CreatedDateProperty)
      End Get
    End Property

    Public Shared VersionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Version, "Version", "")
    ''' <Summary>
    ''' Gets the Version value
    ''' </Summary>
    <Display(Name:="Version", Description:="Scheduler version"),
   Singular.DataAnnotations.LocalisedDisplay(GetType(Localisation.SingularObservableResources), "ROScheduleProgress_Version")>
    Public ReadOnly Property Version() As String
      Get
        Return GetProperty(VersionProperty)
      End Get
    End Property

#End Region

#Region " Methods "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(ScheduleProgressIDProperty)

    End Function

    Public Overrides Function ToString() As String

      Return Me.Progress

    End Function

#End Region

#End Region

#Region " Data Access & Factory Methods "

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetROScheduleProgress(ByVal dr As SafeDataReader) As ROScheduleProgress

      Dim r As New ROScheduleProgress()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      With sdr
        LoadProperty(ScheduleProgressIDProperty, .GetInt32(0))
        LoadProperty(ScheduleInfoIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
        LoadProperty(ProgressProperty, .GetString(2))
        LoadProperty(CreatedDateProperty, .GetSmartDate(3))
        LoadProperty(VersionProperty, .GetString(4))
      End With

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace