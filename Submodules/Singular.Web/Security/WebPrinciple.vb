Imports Singular.Security
Imports Csla
Imports Csla.Serialization
Imports System.Web.Security
Imports System.ComponentModel.DataAnnotations
Imports Singular.DataAnnotations

Namespace Security

  Public Enum AuthType
    FormsCookie = 1
    WindowsAuth = 2
    HTTPHeader = 3
  End Enum

  <Serializable()>
  Public Class WebPrinciple(Of i As WebIdentity(Of i))
    Inherits Singular.Security.PrincipalBase(Of i)

    Public Sub New(ByVal identity As i)
      MyBase.New(identity)
    End Sub

    ''' <summary>
    ''' Refetches the User From the Database. This is called when the User Session has expired, but the Authentication Cookie is still active.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function Refetch(UserName As String, AuthType As AuthType, Optional AuthTicket As FormsAuthenticationTicket = Nothing) As Singular.Security.IPrincipal

      Dim Criteria = CreateCriteria(UserName)
      If AuthTicket IsNot Nothing Then
        Criteria.AuthTicketUserData = AuthTicket.UserData
      End If

      Dim identity = DataPortal.Fetch(Of i)(Criteria)

      If identity.IsAuthenticated OrElse AuthType = Singular.Web.Security.AuthType.WindowsAuth Then
        'Windows authentication will have authenticated this user on the domain,
        'but the user is not in the app database. Return a user that is not authenticated.
        Dim principal As New Singular.Web.Security.WebPrinciple(Of i)(identity)
        Csla.ApplicationContext.User = principal
        ResetUser(identity.Name.ToLower)
        StoreUser(identity.Name.ToLower, principal)
        identity.AuthType = AuthType
        Return principal

      End If
      Return Nothing
    End Function

    Public Shared Shadows Function Login(ByVal UserName As String, ByVal Password As String, AuthType As AuthType) As Boolean

      Return Login(New IdentityCriterea(UserName, Password, False), AuthType)

    End Function

    Public Shared Shadows Function Login(Criteria As IdentityCriterea, AuthType As AuthType) As Boolean

      Dim ident = DoLogin(Criteria, AuthType)

      Return If(ident Is Nothing, False, ident.IsAuthenticated)

    End Function

    Public Shared Function DoLogin(Criteria As IdentityCriterea, AuthType As AuthType) As i

      Try

        Dim identity = DataPortal.Fetch(Of i)(Criteria)

        If identity.IsAuthenticated Then
          identity.AuthType = AuthType
          Dim principal As New Singular.Web.Security.WebPrinciple(Of i)(identity)
          Csla.ApplicationContext.User = principal
          System.Web.HttpContext.Current.User = principal
          ResetUser(identity.Name.ToLower)
          StoreUser(identity.Name.ToLower, principal)
        End If

        Return identity

      Catch ex As Exception
        If Singular.Debug.IsCustomError(ex) Then
          System.Web.HttpContext.Current.Items("LoginError") = Singular.Debug.RecurseExceptionMessage(ex)
          Return Nothing
        Else
          Throw ex
        End If

      End Try

    End Function

    Private Shared LoginRestoreLock As New Object

    Public Shared Sub InitialiseRequest(Context As System.Web.HttpContext)

      Dim UserName As String = Nothing
      Dim Ticket As FormsAuthenticationTicket = Nothing
      Dim AuthType As AuthType

      Dim MobileAuthToken As String = Nothing
      If Context.Request.Headers("AuthToken") IsNot Nothing Then
        MobileAuthToken = Context.Request.Headers("AuthToken")
      End If

      If String.IsNullOrEmpty(MobileAuthToken) Then
        'Forms Authentication / Windows Authentication

        If IsServerFarm OrElse DecryptCookie Then
          'decrypt the cookie to get the version
          Ticket = GetFormsAuthTicket()
        End If

        If Context.Request.IsAuthenticated Then
          UserName = Context.User.Identity.Name.ToLower()
        End If

        'Check if this is a windows auth principle
        If Context.User IsNot Nothing AndAlso TypeOf Context.User Is System.Security.Principal.WindowsPrincipal Then
          AuthType = Singular.Web.Security.AuthType.WindowsAuth
        Else
          AuthType = Singular.Web.Security.AuthType.FormsCookie
        End If

      Else
        'Mobile
        Dim ti As Security.TokenInfo = Singular.Web.Security.GetTokenInfo(MobileAuthToken)
        If ti.ExpiryDate > Now Then
          UserName = ti.UserName
          AuthType = Singular.Web.Security.AuthType.HTTPHeader
          Context.Items("HttpHeaderAuth") = True
        End If

      End If

      If Not String.IsNullOrEmpty(UserName) Then

        SyncLock LoginRestoreLock

          'Get the cached user.
          Dim CachedUser As IPrincipal = GetUser(UserName)

          'If the cached users version is different to the cookie, reset it.
          If CachedUser IsNot Nothing AndAlso CachedUser.GetIdentity(Of i).SetData(Ticket) Then
            CachedUser = Nothing
          End If

          If CachedUser Is Nothing Then
            'Set the cached user.
            Try
              CachedUser = Refetch(UserName, AuthType, Ticket)
              If CachedUser IsNot Nothing Then
                CachedUser.GetIdentity(Of i).SetData(Ticket)
              End If

            Catch ex As Exception
              Singular.Web.Security.Logout()
            End Try

          End If
          'Set the thread and HTTPContext user to the projects specific principle type.
          Context.User = CachedUser
          System.Threading.Thread.CurrentPrincipal = CachedUser

        End SyncLock
      End If

    End Sub

  End Class

  Public Class LoginDetails

    <Required, LocalisedDisplay("Username"),
    Singular.DataAnnotations.TextField(, , False)>
    Public Property UserName As String

    <Required, LocalisedDisplay("Password"), System.ComponentModel.PasswordPropertyText(),
    Singular.DataAnnotations.TextField(, , False)>
    Public Property Password As String

    <Display(Name:="Stay logged in"), LocalisedDisplay("RememberMe")>
    Public Property RememberMe As Boolean = False

    Public Property RedirectLocation As String
  End Class

  Public Class LoginResult
    Inherits Singular.Web.Result

    Public Property RedirectLocation As String

  End Class

End Namespace
