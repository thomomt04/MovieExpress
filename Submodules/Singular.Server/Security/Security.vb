Namespace Security

  Public Module Security

    Public Property PasswordPolicy As PasswordPolicyDefinition = New PasswordPolicyDefinition

    Public Function CurrentPrincipal() As Singular.Security.IPrincipal

      If delGetPrincipal IsNot Nothing Then
        Try
          Dim p As Singular.Security.IPrincipal = delGetPrincipal.Invoke()
          If p IsNot Nothing Then
            Return p
          End If
        Catch ex As Exception
          Throw New Exception("Error using delegate to retrieve principal", ex)
        End Try
      End If

      Try
        If TypeOf Csla.ApplicationContext.User Is IPrincipal Then
          Return Csla.ApplicationContext.User
        Else
          Return Nothing
        End If
      Catch ex As Exception
        Throw New Exception("Error retrieving Csla.ApplicationContext.User", ex)
      End Try


    End Function

    Public ReadOnly Property CurrentIdentity() As Singular.Security.IIdentity
      Get
        Dim p = CurrentPrincipal()
        If p Is Nothing Then
          Return Nothing
        Else
          Return p.Identity
        End If
      End Get
    End Property

    Public ReadOnly Property HasAuthenticatedUser As Boolean
      Get
        If CurrentPrincipal() Is Nothing Then
          Return False
        Else
          Return Csla.ApplicationContext.User.Identity.IsAuthenticated
        End If
      End Get
    End Property

    Public ReadOnly Property AdministratorUserGroupID As Integer
      Get
        Return 1
      End Get
    End Property

    Public Function HasAccess(Role As String) As Boolean
      'Check that there is an authenticated user.
      If Not HasAuthenticatedUser Then
        Return False
      Else

        'If the user is an administrator, the user has all roles.
        Return CurrentIdentity.IsAdministrator OrElse
          CurrentPrincipal.IsInRole(Role) 'Otherwise check if the user has the role.
      End If
    End Function

    Public Function HasAccess(SectionName As String, SecurityRole As String, SecurityType As String) As Boolean

      Return HasAccess(SectionName & "." & SecurityRole & "." & SecurityType)

    End Function

    Public Function HasAccess(SectionName As String, SecurityRole As String) As Boolean

      Return HasAccess(SectionName & "." & SecurityRole)

    End Function

    Public Delegate Function GetPrincipal() As Singular.Security.IPrincipal
    Private delGetPrincipal As GetPrincipal
    Public Sub SetGetIdentityDelegate(GetPrincipalDelegate As GetPrincipal)
      delGetPrincipal = GetPrincipalDelegate
    End Sub

    Public Enum SecurityRole
      None = 0
    End Enum

  End Module

End Namespace





