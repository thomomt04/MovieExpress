Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

<Serializable()> _
Public Class ServerProgramProgressList
  Inherits SingularBusinessListBase(Of ServerProgramProgressList, ServerProgramProgress)

#Region " Business Methods "

  Public Function GetItem(ServerProgramProgressID As Integer) As ServerProgramProgress

    For Each child As ServerProgramProgress In Me
      If child.ServerProgramProgressID = ServerProgramProgressID Then
        Return child
      End If
    Next
    Return Nothing

  End Function

  Public Overrides Function ToString() As String

    Return "Server Program Progres"

  End Function

  Public Function GetServerProgramProgressDetail(ByVal ServerProgramProgressDetailID As Integer) As ServerProgramProgressDetail

    Dim obj As ServerProgramProgressDetail = Nothing
    For Each parent As ServerProgramProgress In Me
      obj = parent.ServerProgramProgressDetailList.GetItem(ServerProgramProgressDetailID)
      If obj IsNot Nothing Then
        Return obj
      End If
    Next
    Return Nothing

  End Function

#End Region

#Region " Data Access "

  <Serializable()> _
  Public Class Criteria
    Inherits CriteriaBase(Of Criteria)

    Public Sub New()


    End Sub

  End Class

#Region " Common "

  Public Shared Function NewServerProgramProgressList() As ServerProgramProgressList

    Return New ServerProgramProgressList()

  End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

Public Shared Sub BeginGetServerProgramProgressList(ByVal CallBack As EventHandler(Of DataPortalResult(Of ServerProgramProgressList)))

Dim dp As New DataPortal(Of ServerProgramProgressList)
AddHandler dp.FetchCompleted, CallBack
dp.BeginFetch(New Criteria)

End Sub

Public Sub New()

' require use of MobileFormatter

End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

  Public Shared Function GetServerProgramProgressList() As ServerProgramProgressList

    Return DataPortal.Fetch(Of ServerProgramProgressList)(New Criteria)

  End Function

  Private Sub New()

    ' require use of factory methods

  End Sub

  Private Sub Fetch(ByVal sdr As SafeDataReader)

    Me.RaiseListChangedEvents = False
    While sdr.Read
      Me.Add(ServerProgramProgress.GetServerProgramProgress(sdr))
    End While
    Me.RaiseListChangedEvents = True

    Dim parent As ServerProgramProgress = Nothing
    If sdr.NextResult Then
      While sdr.Read
        If IsNothing(parent) OrElse parent.ServerProgramProgressID <> sdr.GetInt32(1) Then
          parent = Me.GetItem(sdr.GetInt32(1))
        End If
        parent.ServerProgramProgressDetailList.RaiseListChangedEvents = False
        parent.ServerProgramProgressDetailList.Add(ServerProgramProgressDetail.GetServerProgramProgressDetail(sdr))
        parent.ServerProgramProgressDetailList.RaiseListChangedEvents = True
      End While
    End If


    For Each child As ServerProgramProgress In Me
      child.CheckRules()
      For Each ServerProgramProgressDetail As ServerProgramProgressDetail In child.ServerProgramProgressDetailList
        ServerProgramProgressDetail.CheckRules()
      Next
    Next

  End Sub

  Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

    Dim crit As Criteria = criteria
    Using cn As New SqlConnection(Settings.ConnectionString)
      cn.Open()
      Try
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.StoredProcedure
          cm.CommandText = "GetProcs.getServerProgramProgressList"
          Using sdr As New SafeDataReader(cm.ExecuteReader)
            Fetch(sdr)
          End Using
        End Using
      Finally
        cn.Close()
      End Try
    End Using

  End Sub

  Friend Sub Update()

    Me.RaiseListChangedEvents = False
    Try
      ' Loop through each deleted child object and call its Update() method
      For Each Child As ServerProgramProgress In deletedList
        Child.DeleteSelf()
      Next

      ' Then clear the list of deleted objects because they are truly gone now.
      deletedList.Clear()

      ' Loop through each non-deleted child object and call its Update() method
      For Each Child As ServerProgramProgress In Me
        If Child.IsNew Then
          Child.Insert()
        Else
          Child.Update()
        End If
      Next
    Finally
      Me.RaiseListChangedEvents = True
    End Try

  End Sub

  Protected Overrides Sub DataPortal_Update()

    UpdateTransactional(AddressOf Update)

  End Sub

#End If

#End Region

#End Region

End Class
