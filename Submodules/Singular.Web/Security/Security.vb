Imports System.Web.HttpContext
Imports System.Web.Caching
Imports System.Security.Cryptography.SymmetricAlgorithm
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Security
Imports Singular.Security

Namespace Security

  Public Module Security

    Public Property EnableCSRFToken As Boolean = True

    ''' <summary>
    ''' True if the auth cookie must be decrypted on each request in order to extract the cookies user data.
    ''' </summary>
    Public Property DecryptCookie As Boolean = False

    Friend WebLoginMethod As Action(Of String, String, AuthType)

    Friend RestoreCachedUser As Func(Of String, HttpContext, IPrincipal, IPrincipal)

    Public Function GetPrinciple() As IPrincipal

      Dim HttpContext = Current

      If HttpContext Is Nothing Then
        'running under a background thread.
        If TypeOf System.Threading.Thread.CurrentPrincipal Is IPrincipal Then
          Return System.Threading.Thread.CurrentPrincipal
        Else
          Return Nothing
        End If
      Else
        'main thread.
        If TypeOf HttpContext.User Is IPrincipal Then
          Return HttpContext.User
        Else
          Return GetCachedUser(HttpContext)
        End If
      End If

    End Function

    Private Function GetCachedUser(HttpContext As HttpContext) As IPrincipal

      Try

        If HttpContext.Request.IsAuthenticated Then

          Dim UserName = HttpContext.User.Identity.Name.ToLower
          Dim CachedUser = GetUser(UserName)

          If RestoreCachedUser IsNot Nothing Then
            CachedUser = RestoreCachedUser(UserName, HttpContext, CachedUser)
          End If

          Return CachedUser

        Else
          Return Nothing
        End If
      Catch ex As Exception
        Return Nothing
      End Try

    End Function

    Public Property GetLogoutURLHandler As Func(Of String)
    Public Property BeforeLogout As Action(Of String)

    Public Sub Logout()

      If BeforeLogout IsNot Nothing Then BeforeLogout.Invoke(Current.User.Identity.Name)

      If Current.User IsNot Nothing Then
        ResetUser(Current.User.Identity.Name)
      End If

      FormsAuthentication.SignOut()
      If Current.Session IsNot Nothing Then
        ChangeSessionID()
      End If
      Current.User = Nothing
      Csla.ApplicationContext.User = New Csla.Security.UnauthenticatedPrincipal

      'Clear the auth cookie
      Dim AuthCookie As New HttpCookie(FormsAuthentication.FormsCookieName, "")
      AuthCookie.Expires = DateTime.Now.AddYears(-1)
      Current.Response.Cookies.Add(AuthCookie)

    End Sub

    Public Sub ChangeSessionID()
      System.Web.HttpContext.Current.Session.Clear()
      System.Web.HttpContext.Current.Session.Abandon()
      Dim sm As New System.Web.SessionState.SessionIDManager()
      sm.RemoveSessionID(System.Web.HttpContext.Current)
      sm.SaveSessionID(System.Web.HttpContext.Current, sm.CreateSessionID(System.Web.HttpContext.Current), False, False)
    End Sub

    Private Const UserTokenName As String = "UserToken"

    Friend Sub StoreUser(UserName As String, User As Singular.Security.IPrincipal)

      System.Web.HttpContext.Current.Cache.Add(UserTokenName & UserName.ToLower, User, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 20, 0), CacheItemPriority.Default, Nothing)

    End Sub

    Public Function GetUser(UserName As String) As Singular.Security.IPrincipal

      Return System.Web.HttpContext.Current.Cache.Get(UserTokenName & UserName.ToLower)

    End Function

    Public Sub ResetAllUsers()
      For Each entry As DictionaryEntry In System.Web.HttpContext.Current.Cache
        If entry.Key.StartsWith(UserTokenName) Then
          System.Web.HttpContext.Current.Cache.Remove(entry.Key)
        End If
      Next
    End Sub

    Public Sub ResetUser(UserName As String)
      If UserName IsNot Nothing AndAlso GetUser(UserName) IsNot Nothing Then
        System.Web.HttpContext.Current.Cache.Remove(UserTokenName & UserName.ToLower)

      End If
    End Sub

    Public Function GetCSRFToken() As String

      Dim User = CType(CurrentIdentity, IWebIdentity)

      If Singular.Web.Data.JS.JSSerialiser.KeyEncodingType = Singular.Web.Data.JS.JSSerialiser.EncodingType.Hex Then
        Return Encryption.GetEncryptedTokenHex("CSRFToken" & User.GetCSRFTokenValue)
      Else
        Return Encryption.GetEncryptedToken("CSRFToken" & User.GetCSRFTokenValue)
      End If

    End Function

    Public Sub AddCSRFToken(Page As System.Web.UI.Page)
      If EnableCSRFToken AndAlso Page.Request.IsAuthenticated AndAlso Singular.Security.CurrentIdentity IsNot Nothing Then
        System.Web.UI.ScriptManager.RegisterHiddenField(Page, "hCSRF", GetCSRFToken)
      End If
    End Sub

    Public Sub ValidateCSRFToken(Request As System.Web.HttpRequest)

      If EnableCSRFToken AndAlso Request.IsAuthenticated AndAlso Singular.Security.CurrentIdentity IsNot Nothing Then

        If (HttpContext.Current.Items("HttpHeaderAuth") IsNot Nothing AndAlso HttpContext.Current.Items("HttpHeaderAuth") = True) OrElse
        CType(Singular.Security.CurrentIdentity, IWebIdentity).AuthType = AuthType.HTTPHeader Then

          'If the user was authenticated with a header, then there is no CSRF which is ok.
        Else

          Dim Token As String = Request.Form("hCSRF")
          If Token Is Nothing Then
            Token = Request.Headers("hCSRF")
          End If
          If Token Is Nothing Then
            Token = Request.QueryString("CSRFToken")
          End If
          'Decryption was breaking sometimes when the app pool reset.
          'The padding / block size was different, and the token couldnt be decrypted, even though the encrypted string was exactly the same.
          'So now we just compare the encrypted tokens

          'Decrypt token
          'If Token IsNot Nothing Then
          '  Try
          '    Token = DecryptToken(Token)
          '  Catch ex As Exception
          '    Throw New Exception("Cannot decrypt CSRF token: " & Token)
          '  End Try
          'End If

          If Token Is Nothing OrElse Token <> GetCSRFToken() Then
            Throw New Exception(String.Format("Invalid CSRF Token"))
          End If
        End If

      End If
    End Sub

    Public CreateCriteria As Func(Of String, IdentityCriterea) = Function(UserName)
                                                                   Return New IdentityCriterea(UserName, "", True)
                                                                 End Function

    Public Class TokenInfo
      Public Property UserName As String
      Public Property ExpiryDate As Date
    End Class

    Friend Function GetMessageFromToken(TokenInfo As TokenInfo) As String
      Return Singular.Encryption.GetEncryptedToken(TokenInfo.ExpiryDate.ToBinary.ToString("X") & "_" & TokenInfo.UserName.ToLower)
    End Function

    Public Function GetFormsAuthTicket() As FormsAuthenticationTicket
      Dim AuthCookie = System.Web.HttpContext.Current.Request.Cookies(FormsAuthentication.FormsCookieName)
      If AuthCookie IsNot Nothing Then
        Return FormsAuthentication.Decrypt(AuthCookie.Value)
      End If
      Return Nothing
    End Function

    Public Function GetTokenInfo(Token As String) As TokenInfo
      Dim ti As New TokenInfo
      Dim Message As String = Singular.Encryption.DecryptToken(Token)
      Dim FirstPos As Integer = Message.IndexOf("_")

      ti.UserName = Message.Substring(FirstPos + 1)
      Dim ExpiryDateString As String = Message.Substring(0, FirstPos)

      ti.ExpiryDate = Date.FromBinary(Long.Parse(ExpiryDateString, System.Globalization.NumberStyles.HexNumber))
      Return ti
    End Function

    Public Function GetSafeRedirectUrl(URL As String, DefaultURL As String) As String
      If Not String.IsNullOrEmpty(URL) AndAlso URL.Length > 2 AndAlso
                  ((URL(0) = "/" AndAlso (URL.Length = 1 OrElse (URL(1) <> "/" AndAlso URL(1) <> "\"))) OrElse
                   (URL.Length > 1 AndAlso URL(0) = "~" AndAlso URL(1) = "/")) Then
        Return URL
      Else
        Return DefaultURL
      End If
    End Function

    ''' <summary>
    ''' Checks that the url is local, then redirects. Otherwise redirects to root path.
    ''' </summary>
    Public Sub RedirectSafe(Url As String)
      System.Web.HttpContext.Current.Response.Redirect(GetSafeRedirectUrl(Url, "~/"))
    End Sub


  End Module

End Namespace

