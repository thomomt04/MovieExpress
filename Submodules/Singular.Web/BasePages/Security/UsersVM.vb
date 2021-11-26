Namespace Security

  <Serializable()>
  Public Class UsersVMSingular
    Inherits Singular.Web.Security.UsersVM(Of UsersVMSingular, Singular.Security.UserList, Singular.Security.User)

    Protected Overrides Function GetUserList() As Singular.Security.UserList
      Return Singular.Security.UserList.GetUserList
    End Function

  End Class

  <Serializable()>
  Public MustInherit Class UsersVM(Of VMType As UsersVM(Of VMType, UList, UObj), UList As Singular.Security.UserListBase(Of UList, UObj), UObj As Singular.Security.UserBase(Of UObj))
    Inherits ViewModel(Of VMType)

    Private mUserList As UList

    Public ReadOnly Property UserList As UList
      Get
        Return mUserList
      End Get
    End Property

    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property SecurityGroupList As Singular.Security.SecurityGroupList
      Get
        Return Singular.Security.SecurityGroupList.GetSecurityGroupList
      End Get
    End Property

    <ComponentModel.DisplayName("Name Filter"), Singular.DataAnnotations.TextField()>
    Public Property NameFilter As String

    Protected Overrides Sub Setup()
      MyBase.Setup()

      mUserList = GetUserList()
    End Sub

    Protected MustOverride Function GetUserList() As UList

    Protected Overrides Sub HandleCommand(Command As String, Argument As Singular.Web.CommandArgs)
      MyBase.HandleCommand(Command, Argument)

      Select Case Command
        Case "Save"
          For Each user As UObj In UserList
            If user.IsDirty Then
              Singular.Web.Security.ResetUser(user.GetOldUserName)
              Singular.Web.Security.ResetUser(user.LoginName)
            End If
          Next
          With TrySave(UserList)
            If .Success Then
              mUserList = .SavedObject
            End If
          End With

        Case "Undo"
          mUserList = GetUserList()
      End Select
    End Sub

  End Class

End Namespace

