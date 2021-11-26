Imports Singular.Security

Namespace Security

  <Serializable()>
  Public MustInherit Class SecurityVM(Of SGLType As Singular.ISingularBusinessListBase)
    Inherits ViewModel(Of SecurityVM(Of SGLType))

    Private mSecurityGroupList As SGLType
    Private mROSecurityRoleList As Singular.Security.ROSecurityRoleHeaderList

    Protected Overrides Sub Setup()
      MyBase.Setup()

      mSecurityGroupList = GetSecurityGroupList()
      mROSecurityRoleList = GetROSecurityRoleHeaderList()
    End Sub

    Protected MustOverride Function GetSecurityGroupList() As SGLType

    Protected Overridable Function GetROSecurityRoleHeaderList() As ROSecurityRoleHeaderList
      Return ROSecurityRoleHeaderList.GetROSecurityRoleHeaderList
    End Function

    Public ReadOnly Property SecurityGroupList As SGLType
      Get
        Return mSecurityGroupList
      End Get
    End Property

    <Singular.DataAnnotations.ClientOnly(False)>
    Public ReadOnly Property ROSecurityRoleHeaderList As Singular.Security.ROSecurityRoleHeaderList
      Get
        Return mROSecurityRoleList
      End Get
    End Property

    Public Property CurrentGroup As Singular.Security.SecurityGroup

    Protected Overrides Sub HandleCommand(Command As String, args As Singular.Web.CommandArgs)
      MyBase.HandleCommand(Command, args)

      Select Case Command

        Case "EditRoles"
          EditRole(args.ClientArgs.ButtonArgument)

        Case "Undo"
          AddMessage(Singular.Web.MessageType.Information, "Undo", "Changes to " & CurrentGroup.SecurityGroup & " roles have been un-done.")
          CurrentGroup = Nothing
          'mROSecurityRoleList = Nothing

        Case "Save"
          SaveSecurityGroup()

      End Select
    End Sub

    Private Sub EditRole(Argument As String)

      CurrentGroup = FindObject(Argument)

      CurrentGroup.PopulateSecurityRoleList(ROSecurityRoleHeaderList)

    End Sub

    Private Sub SaveSecurityGroup()

      If CurrentGroup IsNot Nothing Then
        CurrentGroup.UpdateFromSecurityRoleList(ROSecurityRoleHeaderList)
      End If
      Dim WasDirty As Boolean = mSecurityGroupList.IsDirty
      With TrySave(mSecurityGroupList)
        If .Success Then
          mSecurityGroupList = .SavedObject
        End If
      End With
      CurrentGroup = Nothing
      If WasDirty Then
        Security.ResetAllUsers()
      End If

    End Sub

  End Class

  Public Class SecurityVM
    Inherits SecurityVM(Of SecurityGroupList)

    Protected Overrides Function GetSecurityGroupList() As SecurityGroupList
      Return Singular.Security.SecurityGroupList.GetSecurityGroupList
    End Function

  End Class

  Public MustInherit Class StatelessSecurityVM(Of SGLType As Singular.ISingularBusinessListBase)
    Inherits SecurityVM(Of SGLType)
    Implements IStatelessViewModel

    Public Shared Property EditRoles_SecurityRole = "Security.Edit Roles"

    Public Shared Function SaveGroups(GroupList As SGLType) As Singular.Web.SaveResult

      Dim WasDirty = GroupList.IsDirty

      If Singular.Security.HasAccess(EditRoles_SecurityRole) Then
        Dim Result As New Singular.Web.SaveResult(GroupList.TrySave)

        If WasDirty Then
          Security.ResetAllUsers()
        End If

        Return Result

      Else
        Throw New Exception("Access denied")
      End If
    End Function

  End Class

  Public Class StatelessSecurityVM
    Inherits StatelessSecurityVM(Of SecurityGroupList)

    Protected Overrides Function GetSecurityGroupList() As SecurityGroupList
      Return Singular.Security.SecurityGroupList.GetSecurityGroupList
    End Function

  End Class

End Namespace


