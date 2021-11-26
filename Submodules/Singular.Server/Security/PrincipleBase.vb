Imports Csla
Imports Csla.Serialization

Namespace Security

  <Serializable()> _
  Public Class PrincipalBase(Of i As IdentityBase(Of i))
    Inherits Csla.Security.CslaPrincipal
    Implements IPrincipal

    Public Sub New()

    End Sub

    Public Function GetIdentity(Of T As IIdentity)() As T Implements IPrincipal.GetIdentity

      Return Me.Identity

    End Function

    Public Overrides Function IsInRole(role As String) As Boolean

      Return Me.GetIdentity.IsInRole(role)

    End Function

    Public Function GetIdentity() As i
      Return Me.Identity
    End Function

    Public Sub New(ByVal identity As i)
      MyBase.New(identity)
    End Sub

    Public Shared Sub Logout()
      Csla.ApplicationContext.User = New Csla.Security.UnauthenticatedPrincipal()
    End Sub

#If SILVERLIGHT Then

#Else

    ''' <summary>
    ''' Tries to authenticate the user with the supplied password
    ''' </summary>
    ''' <remarks>Password must be encrypted</remarks>
    Public Shared Function Login(ByVal UserName As String, ByVal Password As String) As Boolean

      Dim identity = DataPortal.Fetch(Of i)(New Singular.Security.IdentityCriterea(UserName, Password))

      If identity.IsAuthenticated Then
        Dim principal As New PrincipalBase(Of i)(identity)
        Csla.ApplicationContext.User = principal
      End If
      Return identity.IsAuthenticated

    End Function

#End If

  End Class

End Namespace
