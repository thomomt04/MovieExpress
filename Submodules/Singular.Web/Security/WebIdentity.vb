Imports Singular.Security
Imports Csla
Imports Csla.Serialization
Imports System.Web.Security
Imports System.ComponentModel.DataAnnotations
Imports Singular.DataAnnotations

Namespace Security

  <Serializable()>
  Public Class WebIdentity(Of i As WebIdentity(Of i))
    Inherits Singular.Security.IdentityBase(Of i)
    Implements IWebIdentity

    ''' <summary>
    ''' The HTML to be used by the LoggedInName control.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Overridable ReadOnly Property LoginLabelHTML As String Implements IWebIdentity.LoginLabelHTML
      Get
        Return "<strong>" & System.Web.HttpUtility.HtmlEncode(UserNameReadable) & "</strong>"
      End Get
    End Property

    Friend Property AuthType As AuthType Implements IWebIdentity.AuthType

    Protected Overrides Sub SetupSqlCommand(cmd As System.Data.SqlClient.SqlCommand, Criteria As IdentityCriterea)
      cmd.CommandText = "GetProcs.WebLogin"
      If cmd.Parameters("@RefreshingRoles") Is Nothing Then
        cmd.Parameters.AddWithValue("@RefreshingRoles", CType(Criteria, IdentityCriterea).RefreshingRoles)
      Else
        cmd.Parameters("@RefreshingRoles").Value = CType(Criteria, IdentityCriterea).RefreshingRoles
      End If
    End Sub

    Public Function GetAuthToken(ExpiryDate As Date) As String Implements IWebIdentity.GetAuthToken
      Return Singular.Web.Security.GetMessageFromToken(New Singular.Web.Security.TokenInfo() With {.ExpiryDate = ExpiryDate, .UserName = GetAuthTokenUsername()})
    End Function

    Protected Overridable Function GetAuthTokenUsername() As String
      Return Me.Name
    End Function

    Protected Overridable Function GetCSRFTokenValue() As String Implements IWebIdentity.GetCSRFTokenValue
      Return Name.ToLower.Trim
    End Function

    ''' <summary>
    ''' Additional check to determine if the user is authenticated or not.
    ''' </summary>
    Public Overridable ReadOnly Property CustomAuthCriteria As Boolean
      Get
        Return True
      End Get
    End Property

#Region " Cookie "

    Private mPersistCookie As Boolean
    Private mVersion As Integer = 0
    Private mVersionChanged As Boolean = False

    Protected Friend Overridable Function SetData(Ticket As FormsAuthenticationTicket) As Boolean
      mVersionChanged = False
      If Ticket IsNot Nothing AndAlso mVersion <> Ticket.Version Then
        mVersion = Ticket.Version
        Return True
      End If
      Return False
    End Function

    ''' <summary>
    ''' Marks this identity object as old, so other webservers will refetch it.
    ''' </summary>
    Protected Sub Invalidate()
      If Not mVersionChanged Then
        Dim Ticket = GetFormsAuthTicket()
        SetData(Ticket)
        mVersion += 1
        If Ticket IsNot Nothing Then
          System.Web.HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName)
          System.Web.HttpContext.Current.Response.Cookies.Add(GetCookie(Ticket.IsPersistent))
        End If
        mVersionChanged = True
      End If
    End Sub

    Private Function GetCookie(Remember As Boolean) As System.Web.HttpCookie

      Return GetAuthCookie(Name, Remember, mVersion, GetCookieUserData)

    End Function

    Protected Overridable Function GetCookieUserData() As String
      Return UserID
    End Function

    Public Shared Function GetAuthCookie(UserName As String, Remember As Boolean, Optional Version As Integer = 0, Optional UserData As String = "") As System.Web.HttpCookie

      Dim Ticket As New FormsAuthenticationTicket(Version, UserName, Now, Now.Add(FormsAuthentication.Timeout), Remember, UserData)
      Dim AuthCookie As New System.Web.HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(Ticket))
      AuthCookie.HttpOnly = True
      If Remember Then AuthCookie.Expires = Ticket.Expiration
      Return AuthCookie

    End Function

    ''' <summary>
    ''' Sets a new user name, and re-generates the auth cookie.
    ''' </summary>
    Public Sub Rename(NewUserName As String)
      LoadProperty(NameProperty, NewUserName)
      Invalidate()
    End Sub

#End Region

    Protected Overridable Sub OnLogin(Result As LoginResult)

    End Sub

    ''' <summary>
    ''' Checks the user credentials, and creates the authentication cookie
    ''' </summary>
    ''' <param name="LoginInfo">Login Information</param>
    ''' <param name="OnError">An optional function to provide custom error text</param>
    Public Shared Function Login(LoginInfo As LoginDetails, Optional OnError As Action(Of Singular.Web.Result) = Nothing) As LoginResult

      Dim Result = Login(New Singular.Security.IdentityCriterea With {.Username = LoginInfo.UserName, .Password = LoginInfo.Password},
                         LoginInfo.RememberMe, OnError, LoginInfo.RedirectLocation)

      Return Result

    End Function

    Public Shared Function Login(Criteria As Singular.Security.IdentityCriterea, RememberMe As Boolean,
                                 Optional OnError As Action(Of Singular.Web.Result) = Nothing,
                                 Optional RedirectLocation As String = Nothing) As LoginResult

      Dim Result As LoginResult
      Dim Context = System.Web.HttpContext.Current

      Dim Ident = WebPrinciple(Of i).DoLogin(Criteria, Singular.Web.Security.AuthType.FormsCookie)

      If Ident IsNot Nothing AndAlso Ident.IsAuthenticated Then

        'Set the auth cookie
        Dim cookie = Ident.GetCookie(RememberMe)
        Context.Response.Cookies.Add(cookie)

        Result = New LoginResult With {.Success = True}

        'Redirect if redicrect location was supplied.
        If Not String.IsNullOrEmpty(RedirectLocation) Then
          Result.RedirectLocation = Singular.Web.Security.GetSafeRedirectUrl(RedirectLocation, String.Empty)
        End If

        'Custom Login Code
        Ident.OnLogin(Result)

      Else
        If Context.Items("LoginError") IsNot Nothing Then
          Result = New LoginResult With {.Success = False, .ErrorText = System.Web.HttpContext.Current.Items("LoginError")}
        Else
          Result = New LoginResult With {.Success = False, .ErrorText = "Incorrect User Name or Password"}
        End If

        'Custom Error Code
        If OnError IsNot Nothing Then
          OnError(Result)
        End If

      End If

      Return Result

    End Function

  End Class

  <Serializable()>
  Public Class WebIdentityGeneric
    Inherits WebIdentity(Of WebIdentityGeneric)
  End Class

End Namespace