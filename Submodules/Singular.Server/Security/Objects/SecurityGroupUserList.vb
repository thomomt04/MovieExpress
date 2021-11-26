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
  Public Class SecurityGroupUserList
    Inherits SingularBusinessListBase(Of SecurityGroupUserList, SecurityGroupUser)

#Region " Parent "

    <NotUndoable()> Private mParent As User
#End Region

#Region " Business Methods "

    Public Function GetItem(SecurityGroupUserID As Integer) As SecurityGroupUser

      For Each child As SecurityGroupUser In Me
        If child.SecurityGroupUserID = SecurityGroupUserID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Security Group Users"

    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared UserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.UserID)

      Public Property UserID() As Integer
        Get
          Return ReadProperty(UserIDProperty)
        End Get
        Set(value As Integer)
          LoadProperty(UserIDProperty, value)
        End Set
      End Property

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewSecurityGroupUserList() As SecurityGroupUserList

      Return New SecurityGroupUserList()

    End Function

    Public Shared Sub BeginGetSecurityGroupUserList(UserID As Integer, ByVal CallBack As EventHandler(Of DataPortalResult(Of SecurityGroupUserList)))

      Dim dp As New DataPortal(Of SecurityGroupUserList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria With {.UserID = UserID})

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Private Sub New()

      ' require use of factory methods

    End Sub

    Public Shared Function GetSecurityGroupUserList(ByVal UserID As Integer) As SecurityGroupUserList

      Return DataPortal.Fetch(Of SecurityGroupUserList)(New Criteria With {.UserID = UserID})

    End Function

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(SecurityGroupUser.GetSecurityGroupUser(sdr))
      End While
      Me.RaiseListChangedEvents = True

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getSecurityGroupUserList"

            cm.Parameters.AddWithValue("@SecurityGroupID", crit.UserID)

            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Public Shared Property UseSelectedField As Boolean = False

    Public Sub Update()

      Me.RaiseListChangedEvents = False

      If UseSelectedField Then
        ' remove any un-selected
        For i As Integer = Me.Count - 1 To 0 Step -1
          If Not Me(i).Selected Then
            Me.RemoveAt(i)
          End If
        Next
      End If

      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As SecurityGroupUser In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As SecurityGroupUser In Me
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

#End If

#End Region

#End Region

  End Class


End Namespace