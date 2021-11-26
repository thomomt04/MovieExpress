Namespace Security

  Public Interface ISecurityGroup

    ReadOnly Property SecurityGroupID As Integer

    ReadOnly Property SecurityGroupRoleList() As SecurityGroupRoleList

    Sub SetChangingRole(Value As Boolean)

#If SILVERLIGHT Then
    ReadOnly Property ROSecurityRoleHeaderList As ROSecurityRoleHeaderList

#End If

  End Interface

End Namespace