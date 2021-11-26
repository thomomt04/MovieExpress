Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Singular

#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Service.Scheduling

  <Serializable()> _
  Public Class ROScheduleProgressList
    Inherits SingularReadOnlyListBase(Of ROScheduleProgressList, ROScheduleProgress)

#Region " Business Methods "

    Public Function GetItem(ScheduleProgressID As Integer) As ROScheduleProgress

      For Each child As ROScheduleProgress In Me
        If child.ScheduleProgressID = ScheduleProgressID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return My.Resources.localstring.ROScheduleProgressList_ToString

    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared ScheduleInfoIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ScheduleInfoID)

      Public Property ScheduleInfoID() As Integer
        Get
          Return ReadProperty(ScheduleInfoIDProperty)
        End Get
        Set(ByVal Value As Integer)
          LoadProperty(ScheduleInfoIDProperty, Value)
        End Set
      End Property

      Public Property ToDate As Date?

      Public Shared Function NewCriteria() As Criteria

        Return New Criteria

      End Function

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewROScheduleProgressList() As ROScheduleProgressList

      Return New ROScheduleProgressList()

    End Function

    Public Shared Sub BeginGetROScheduleProgressList(ByVal CallBack As EventHandler(Of DataPortalResult(Of ROScheduleProgressList)))

      Dim dp As New DataPortal(Of ROScheduleProgressList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria)

    End Sub

    Public Shared Sub BeginGetROScheduleProgressList(ScheduleInfoID As Integer, ByVal CallBack As EventHandler(Of DataPortalResult(Of ROScheduleProgressList)))

      Dim dp As New DataPortal(Of ROScheduleProgressList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria() With {.ScheduleInfoID = ScheduleInfoID})

    End Sub


    Public Sub New()

      ' must have parameterless constructor

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetROScheduleProgressList(ScheduleID As Integer, ToDate As Date) As ROScheduleProgressList

      Return DataPortal.Fetch(Of ROScheduleProgressList)(New Criteria With {.ScheduleInfoID = ScheduleID, .ToDate = ToDate})

    End Function

    Public Shared Function GetROScheduleProgressList() As ROScheduleProgressList

      Return DataPortal.Fetch(Of ROScheduleProgressList)(New Criteria)

    End Function

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Me.Add(ROScheduleProgress.GetROScheduleProgress(sdr))
      End While
      Me.IsReadOnly = True
      Me.RaiseListChangedEvents = True

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getROScheduleProgressList"
            cm.Parameters.AddWithValue("@ScheduleInfoID", Singular.Strings.MakeEmptyDBNull(crit.ScheduleInfoID))
            If crit.ToDate IsNot Nothing Then
              cm.Parameters.AddWithValue("@ToDate", crit.ToDate)
            End If

            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub



#End If

#End Region

#End Region

  End Class


End Namespace