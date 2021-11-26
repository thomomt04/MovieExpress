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

  <Serializable()>
  Public Class UserListBase(Of T As UserListBase(Of T, C), C As UserBase(Of C))
    Inherits SingularBusinessListBase(Of T, C)
    Implements IUserList

#Region " Business Methods "

#If SILVERLIGHT Then
#Else
    Public Overloads Sub BeginEdit()

      If Settings.CurrentPlatform = CommonDataPlatform.Windows Then
        Me.FlattenEditLevels()
      End If

      MyBase.BeginEdit()

    End Sub

    Public Overloads Sub CancelEdit()

      If Settings.CurrentPlatform = CommonDataPlatform.Windows Then
        Me.FlattenEditLevels()
      End If

      MyBase.CancelEdit()

    End Sub

    Public Overloads Sub ApplyEdit()

      If Settings.CurrentPlatform = CommonDataPlatform.Windows Then
        Me.FlattenEditLevels()
      End If

      MyBase.ApplyEdit()

    End Sub
#End If

    Public Function GetItem(ByVal UserID As Integer) As C

      For Each child As C In Me
        If child.UserID = UserID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Users"

    End Function

#End Region


#Region " Data Access "

#Region " Common "

    Public Class BaseCriteria
      Inherits Csla.CriteriaBase(Of BaseCriteria)

      Public Shared UserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(f) f.UserID, "UserID", 0)

      <Display(AutoGenerateField:=False)>
      Public Property UserID() As Integer?
        Get
          Return ReadProperty(UserIDProperty)
        End Get
        Set(ByVal value As Integer?)
          LoadProperty(UserIDProperty, value)
        End Set
      End Property

    End Class

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else



#End Region

#Region " .Net Data Access "



    Protected Overridable Function GetUser(sdr As SafeDataReader) As C
      Dim u As C = Activator.CreateInstance(Of C)()
      u.Fetch(sdr)
      Return u
    End Function

    Private Sub BaseFetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(GetUser(sdr))
      End While
      Me.RaiseListChangedEvents = True

      LoadChildren(sdr)

      For Each child As C In Me
        child.CheckRules()
        For Each SecurityGroupUser As SecurityGroupUser In child.SecurityGroupUserList
          SecurityGroupUser.CheckRules()
        Next
      Next

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      'Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = GetListProcName
            AddProcParameters(cm, criteria)

            Using sdr As New SafeDataReader(cm.ExecuteReader)
              BaseFetch(sdr)
            End Using

          End Using
        Finally
          cn.Close()
        End Try

        PostFetch(criteria)
      End Using

    End Sub

    Protected Overridable Sub PostFetch(ByVal criteria As BaseCriteria)

    End Sub

    Protected Overridable ReadOnly Property GetListProcName As String
      Get
        Return "GetProcs.getUserList"
      End Get
    End Property

    Protected Overridable Sub AddProcParameters(cm As SqlCommand, Criteria As BaseCriteria)

    End Sub

    Protected Overridable Sub LoadChildren(sdr As SafeDataReader)

      Dim parent As C = Nothing
      If sdr.NextResult Then
        While sdr.Read
          If IsNothing(parent) OrElse parent.UserID <> sdr.GetInt32(2) Then
            parent = Me.GetItem(sdr.GetInt32(2))
          End If
          parent.SecurityGroupUserList.RaiseListChangedEvents = False
          parent.SecurityGroupUserList.Add(SecurityGroupUser.GetSecurityGroupUser(sdr))
          parent.SecurityGroupUserList.RaiseListChangedEvents = True
        End While
      End If

    End Sub

    Protected Friend Sub Update() Implements IUserList.Update

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As C In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As C In Me
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

End Namespace
