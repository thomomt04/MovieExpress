Imports Csla
Imports Csla.Serialization

Namespace Security

  <Serializable()> _
  Public Class Principal
    Inherits PrincipalBase(Of Identity)

    Public Sub New()
      MyBase.New()
    End Sub

    Public Sub New(ByVal identity As Identity)
      MyBase.New(identity)
    End Sub

    Public Class PrincipleLoginCompletedEventArgs
      Public Property ErrorMessage As Exception
      Public Property Authenticated As Boolean
    End Class

    Public Delegate Sub PrincipleLoginCompletedCallBack(ByVal sender As Object, ByVal e As PrincipleLoginCompletedEventArgs)

    'Public Shared Sub LoginFromAsp(ByVal callback As PrincipleLoginCompletedCallBack)

    '  Dim identity As New DataPortal(Of Identity)
    '  AddHandler identity.FetchCompleted,
    '    Sub(s, e)

    '      If (e.Error Is Nothing AndAlso e.Object.IsAuthenticated) Then
    '        'Create a principle containing the Authenticated Identity.
    '        Csla.ApplicationContext.User = New Principal(e.Object)
    '        callback.Invoke(Nothing, New PrincipleLoginCompletedEventArgs() With {.Authenticated = True})
    '      Else
    '        'Set the user to an unauthenticated Principle and Identity.
    '        Csla.ApplicationContext.User = New Csla.Security.UnauthenticatedPrincipal()
    '        callback.Invoke(Nothing, New PrincipleLoginCompletedEventArgs() With {.Authenticated = False, .ErrorMessage = e.Error})
    '      End If

    '    End Sub

    '  identity.BeginFetch(New EmptyUsernameCriteria())


    'End Sub

    Public Overloads Shared Sub Login(ByVal UserName As String, ByVal Password As String, ByVal callback As PrincipleLoginCompletedCallBack)

      Dim identity As New DataPortal(Of Identity)
      AddHandler identity.FetchCompleted,
        Sub(s, e)

          If (e.Error Is Nothing AndAlso e.Object.IsAuthenticated) Then
            'Create a principle containing the Authenticated Identity.
            Csla.ApplicationContext.User = New Principal(e.Object)
            callback.Invoke(Nothing, New PrincipleLoginCompletedEventArgs() With {.Authenticated = True})
          Else
            'Set the user to an unauthenticated Principle and Identity.
            Csla.ApplicationContext.User = New Csla.Security.UnauthenticatedPrincipal()
            callback.Invoke(Nothing, New PrincipleLoginCompletedEventArgs() With {.Authenticated = False, .ErrorMessage = e.Error})
          End If

        End Sub

      identity.BeginFetch(New IdentityCriterea(UserName, Password))

    End Sub

    Public Overloads Shared Sub Login(ByVal UserName As String, ByVal Password As String, ByVal RefreshingRoles As Boolean?, ByVal callback As PrincipleLoginCompletedCallBack)

      Dim identity As New DataPortal(Of Identity)
      AddHandler identity.FetchCompleted,
        Sub(s, e)

          If (e.Error Is Nothing AndAlso e.Object.IsAuthenticated) Then
            'Create a principle containing the Authenticated Identity.
            Csla.ApplicationContext.User = New Principal(e.Object)
            callback.Invoke(Nothing, New PrincipleLoginCompletedEventArgs() With {.Authenticated = True})
          Else
            'Set the user to an unauthenticated Principle and Identity.
            Csla.ApplicationContext.User = New Csla.Security.UnauthenticatedPrincipal()
            callback.Invoke(Nothing, New PrincipleLoginCompletedEventArgs() With {.Authenticated = False, .ErrorMessage = e.Error})
          End If

        End Sub

      identity.BeginFetch(New IdentityCriterea(UserName, Password, RefreshingRoles))

    End Sub

  End Class

End Namespace
