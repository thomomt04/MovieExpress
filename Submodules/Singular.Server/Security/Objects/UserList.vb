Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Security

  <Serializable()> _
  Public Class UserList
    Inherits UserListBase(Of UserList, User)

    Public Function GetSecurityGroupUser(ByVal SecurityGroupUserID As Integer) As SecurityGroupUser

      Dim obj As SecurityGroupUser = Nothing
      For Each parent As User In Me
        obj = parent.SecurityGroupUserList.GetItem(SecurityGroupUserID)
        If obj IsNot Nothing Then
          Return obj
        End If
      Next
      Return Nothing

    End Function

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits UserListBase(Of UserList, User).BaseCriteria

      Public Sub New()

      End Sub

    End Class

#Region " Common "

    Public Shared Function NewUserList() As UserList

      Return New UserList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Sub BeginGetUserList(UserID As Integer, ByVal CallBack As EventHandler(Of DataPortalResult(Of UserList)))

      Dim dp As New DataPortal(Of UserList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria() With {.UserID = UserID})

    End Sub


    Public Shared Sub BeginGetUserList(ByVal CallBack As EventHandler(Of DataPortalResult(Of UserList)))

      Dim dp As New DataPortal(Of UserList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria())

    End Sub

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetUserList(UserID As Integer) As UserList

      Return DataPortal.Fetch(Of UserList)(New Criteria With {.UserID = UserID})

    End Function

    Public Shared Function GetUserList() As UserList

      Return DataPortal.Fetch(Of UserList)(New Criteria)

    End Function

    Public Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(User.GetUser(sdr))
      End While
      Me.RaiseListChangedEvents = True

      MyBase.LoadChildren(sdr)

      For Each child As User In Me
        child.CheckRules()
        For Each SecurityGroupUser As SecurityGroupUser In child.SecurityGroupUserList
          SecurityGroupUser.CheckRules()
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
            cm.CommandText = "GetProcs.getUserList"

            CSLALib.ContextInfo.SetContextInfoOnConnection(Of User)(cn, Nothing)

            If crit.UserID <> 0 Then
              cm.Parameters.AddWithValue("@UserID", crit.UserID)
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