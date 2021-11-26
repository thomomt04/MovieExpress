Imports System.Web.Security
Imports System.Web.HttpContext
Imports Singular.Security

Namespace Security

  <Serializable()>
  Public Class WebSecurity(Of p As WebPrinciple(Of i), i As WebIdentity(Of i))
    Inherits MembershipProvider

    Public Overrides Function ValidateUser(ByVal username As String, ByVal password As String) As Boolean

      Try
        LoginError = Nothing

        If WebPrinciple(Of i).Login(username, password, AuthType.FormsCookie) Then
          Return True
        Else
          Return False
        End If

      Catch ex As Exception
        If Singular.Debug.IsCustomError(ex) Then
          LoginError = Singular.Debug.RecurseExceptionMessage(ex)
        Else
          Throw ex
        End If
        Return False
      End Try

    End Function

    Public Shared Property LoginError As String
      Get
        Return System.Web.HttpContext.Current.Items("LoginError")
      End Get
      Set(value As String)
        System.Web.HttpContext.Current.Items("LoginError") = value
      End Set
    End Property

    Public Shared Function HasAuthenticatedUser() As Boolean
      Return Current.Request.IsAuthenticated AndAlso CurrentPrinciple() IsNot Nothing AndAlso CurrentIdentity.CustomAuthCriteria
    End Function

    Public Shared Function CurrentIdentity() As i

      Dim principle = CurrentPrinciple()
      If principle IsNot Nothing Then
        Return principle.Identity
      Else
        Return Nothing
      End If

    End Function

    Public Shared Function CurrentPrinciple() As p

      If Current IsNot Nothing AndAlso Not Current.Request.IsAuthenticated() Then
        Return Nothing
      Else
        Dim principle As p = GetPrinciple()
        If principle Is Nothing Then

          'This method should not be called to check if a user is logged in, as it will redirect to the login page.
          If Current IsNot Nothing Then
            FormsAuthentication.SignOut()
            FormsAuthentication.RedirectToLoginPage()
          End If

          Return Nothing
        Else
          Return principle
        End If
      End If

    End Function

    Public Shared Function GetLoginError() As String
      Dim LoginError As String = System.Web.HttpContext.Current.Items("LoginError")
      If LoginError Is Nothing Then
        Return "Incorrect user name and or password."
      Else
        Return LoginError
      End If
    End Function

    Private Shared LoginRestoreLock As New Object

    Friend Shared Function RestoreCachedUser(UserName As String, HttpContext As HttpContext, CachedUser As IPrincipal) As IPrincipal

      Dim Ticket As FormsAuthenticationTicket = Nothing
      Dim AuthType As AuthType

      If IsServerFarm OrElse DecryptCookie Then
        Ticket = GetFormsAuthTicket()
      ElseIf CachedUser IsNot Nothing Then
        'Shortcut to bypass lock.
        Return CachedUser
      End If

      SyncLock LoginRestoreLock

        If CachedUser IsNot Nothing AndAlso CachedUser.GetIdentity(Of i).SetData(Ticket) Then
          'Cookie version is later than cached version, refetch from database
          CachedUser = Nothing
        End If

        If CachedUser Is Nothing Then
          Try

            If HttpContext.User IsNot Nothing AndAlso TypeOf HttpContext.User Is System.Security.Principal.WindowsPrincipal Then
              AuthType = Singular.Web.Security.AuthType.WindowsAuth
            Else
              AuthType = Singular.Web.Security.AuthType.FormsCookie
            End If

            'Fetch from database, (also stores in cache).
            CachedUser = Singular.Web.Security.WebPrinciple(Of i).Refetch(UserName, AuthType, Ticket)
            If CachedUser IsNot Nothing Then
              CachedUser.GetIdentity(Of i).SetData(Ticket)
            End If

          Catch ex As Exception
            Singular.Web.Security.Logout()
          End Try

          'Set the thread and HTTPContext user to the projects specific principle type.
          HttpContext.User = CachedUser
          System.Threading.Thread.CurrentPrincipal = CachedUser
        End If

      End SyncLock

      Return CachedUser

    End Function

    Public Shared Sub Setup()

      'Callback from Singular.Server into Singular.Web
      SetGetIdentityDelegate(AddressOf GetPrinciple)

      Singular.Web.Security.RestoreCachedUser = AddressOf RestoreCachedUser

    End Sub

#Region " Not Implemented Members "

    Public Overrides Property ApplicationName() As String
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
      Set(ByVal value As String)
        Throw New Exception("The method or operation is not implemented.")
      End Set
    End Property

    Public Overrides Function ChangePassword(ByVal username As String, ByVal oldPassword As String, ByVal newPassword As String) As Boolean

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function ChangePasswordQuestionAndAnswer(ByVal username As String, ByVal password As String, ByVal newPasswordQuestion As String, ByVal newPasswordAnswer As String) As Boolean

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function CreateUser(ByVal username As String, ByVal password As String, ByVal email As String, ByVal passwordQuestion As String, ByVal passwordAnswer As String, ByVal isApproved As Boolean, ByVal providerUserKey As Object, ByRef status As System.Web.Security.MembershipCreateStatus) As System.Web.Security.MembershipUser

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function DeleteUser(ByVal username As String, ByVal deleteAllRelatedData As Boolean) As Boolean

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides ReadOnly Property EnablePasswordReset() As Boolean
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides ReadOnly Property EnablePasswordRetrieval() As Boolean
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides Function FindUsersByEmail(ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Security.MembershipUserCollection

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function FindUsersByName(ByVal usernameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Security.MembershipUserCollection

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function GetAllUsers(ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Security.MembershipUserCollection

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function GetNumberOfUsersOnline() As Integer

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function GetPassword(ByVal username As String, ByVal answer As String) As String

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overloads Overrides Function GetUser(ByVal providerUserKey As Object, ByVal userIsOnline As Boolean) As System.Web.Security.MembershipUser

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overloads Overrides Function GetUser(ByVal username As String, ByVal userIsOnline As Boolean) As System.Web.Security.MembershipUser

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function GetUserNameByEmail(ByVal email As String) As String

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides ReadOnly Property MaxInvalidPasswordAttempts() As Integer
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides ReadOnly Property MinRequiredNonAlphanumericCharacters() As Integer
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides ReadOnly Property MinRequiredPasswordLength() As Integer
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides ReadOnly Property PasswordAttemptWindow() As Integer
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides ReadOnly Property PasswordFormat() As System.Web.Security.MembershipPasswordFormat
      Get
        Return New System.Web.Security.MembershipPasswordFormat
        'Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides ReadOnly Property PasswordStrengthRegularExpression() As String
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides ReadOnly Property RequiresQuestionAndAnswer() As Boolean
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides ReadOnly Property RequiresUniqueEmail() As Boolean
      Get
        Throw New Exception("The method or operation is not implemented.")
      End Get
    End Property

    Public Overrides Function ResetPassword(ByVal username As String, ByVal answer As String) As String

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Function UnlockUser(ByVal userName As String) As Boolean

      Throw New Exception("The method or operation is not implemented.")

    End Function

    Public Overrides Sub UpdateUser(ByVal user As System.Web.Security.MembershipUser)

      Throw New Exception("The method or operation is not implemented.")

    End Sub

#End Region

  End Class

  Public Class WebSecurity(Of i As WebIdentity(Of i))
    Inherits WebSecurity(Of Singular.Web.Security.WebPrinciple(Of i), i)

  End Class

End Namespace

