Public Class WebError

  ''' <summary>
  ''' Does this database have a proc called [CmdProcs].[cmdInsWebError] 
  ''' </summary>
  Public Shared Property SupportsWebError As Boolean = False

  ''' <summary>
  ''' Set to true if you always want detailed errors to be shown to the user. Useful for test websites.
  ''' </summary>
  Public Shared Property ShowDetailedErrors As Boolean = False

  ''' <summary>
  ''' Path of the applications error page.
  ''' </summary>
  Public Shared Property ErrorPageLocation As String = "~/ErrorPage.aspx"

  ''' <summary>
  ''' True if LogEvent must not log.
  ''' </summary>
  ''' <returns></returns>
  Public Shared Property SuppressLog As Boolean = False

  Public Shared Function LogError(Exception As Exception) As Integer

    Dim Page As String = "Unknown"

    If HttpContext.Current IsNot Nothing Then
      Page = HttpContext.Current.Request.Path
    End If

    Return LogError(Exception, Page)

  End Function

  Public Shared Function LogError(Exception As Exception, PageOrMethod As String, Optional OtherData As String = "") As Integer

    Dim StackTrace As String = Singular.Debug.GetCleanStackTrace(Singular.Debug.GetBaseException(Exception))
    Dim ErrorText As String = ""
    Dim BrowserInfo As String = "Unknown"

    'These happen often, so shorten the message that gets stored.
    If Exception.StackTrace IsNot Nothing Then
      If Exception.StackTrace.Trim.StartsWith("Object Reference", Global.System.StringComparison.InvariantCultureIgnoreCase) Then
        ErrorText = "Null Reference"
      ElseIf Exception.StackTrace.Trim.StartsWith("at System.Web.UI.ViewStateException.ThrowError(") Then
        ErrorText = "View State Error"
      End If
    End If
    If String.IsNullOrEmpty(ErrorText) Then
      ErrorText = Singular.Debug.RecurseExceptionMessage(Exception)
    End If

    If HttpContext.Current IsNot Nothing Then

      Try
        BrowserInfo = HttpContext.Current.Request.Browser.Browser & HttpContext.Current.Request.Browser.MajorVersion & "; " & HttpContext.Current.Request.Browser.Platform
      Catch ex As Exception
      End Try

    End If

    Return LogError(PageOrMethod, BrowserInfo, StackTrace, ErrorText, OtherData)

  End Function

  Public Shared ReadOnly Property MustShowDetailedErrors As Boolean
    Get
      Return Debugger.IsAttached OrElse ShowDetailedErrors
    End Get
  End Property

  Public Shared Function LogError(Page As String, BrowserInfo As String, StackTrace As String, ErrorText As String, OtherData As String) As Integer

    Try

      Dim UserID As Integer?
      If Singular.Security.CurrentIdentity IsNot Nothing Then
        UserID = Singular.Security.CurrentIdentity.UserID
      End If

      Dim cProc As New Singular.CommandProc("[CmdProcs].[cmdInsWebError]")
      cProc.Parameters.AddWithValue("@UserID", UserID)
      cProc.Parameters.AddWithValue("@Page", Page)
      cProc.Parameters.AddWithValue("@Browser", BrowserInfo)
      cProc.Parameters.AddWithValue("@StackTrace", StackTrace)
      cProc.Parameters.AddWithValue("@Error", ErrorText)
      cProc.Parameters.AddWithValue("@OtherData", OtherData)
      cProc.FetchType = CommandProc.FetchTypes.DataRow
      cProc = cProc.Execute()

      Return cProc.DataRowValue

    Catch ex As Exception
      Return -1
    End Try

  End Function

  ''' <summary>
  ''' Gets error text to be returned to the client. If in debug mode, or the error is a customerror, the error text is returned. Else the error ID from the WebErrors table
  ''' </summary>
  Public Shared Function GetErrorText(ex As Exception, PageOrMethodOrHandler As String, Optional AdditionalInfo As String = "") As String

    If Singular.Debug.IsCustomError(ex) Then
      Return Singular.Debug.RecurseExceptionMessage(ex)
    ElseIf MustShowDetailedErrors Then
      Return ex.ToString
    Else
      If Singular.Web.WebError.SupportsWebError Then
        Dim ErrorID As Integer = LogError(ex, PageOrMethodOrHandler, AdditionalInfo)
        Return "Service Error. Error ID: " & ErrorID
      Else
        Return "An unknown error occurred."
      End If
    End If

  End Function

  ''' <summary>
  ''' Redirects to the default error page.
  ''' </summary>
  Public Shared Sub RedirectToErrorPage(ErrorLocation As String, ErrorText As String)
    System.Web.HttpContext.Current.Session("AppErrorLocation") = ErrorLocation
    System.Web.HttpContext.Current.Session("AppErrorID") = 0
    System.Web.HttpContext.Current.Session("AppErrorText") = ErrorText
    System.Web.HttpContext.Current.Response.Redirect(ErrorPageLocation)
  End Sub

  ''' <summary>
  ''' Logs any unhandled exceptions to the database, and directs the user to the error page. Your database must have the weberrors table and cmd proc.
  ''' </summary>
  Public Shared Sub HandleApplicationError(Optional ErrorPagePath As String = "", Optional OtherData As String = "", Optional NoConnectionPage As String = "")

    If ErrorPagePath = "" Then
      ErrorPagePath = ErrorPageLocation
    End If

    Dim Request As System.Web.HttpRequest = Nothing
    Try
      Request = System.Web.HttpContext.Current.Request
    Catch ex As Exception
    End Try

    If Request IsNot Nothing Then

      Dim IsHTTPHandler As Boolean = Request.AppRelativeCurrentExecutionFilePath.EndsWith(".ashx")
      Dim ErrorPageName As String = ErrorPagePath.Substring(ErrorPagePath.LastIndexOf("/") + 1)

      Dim Current = System.Web.HttpContext.Current

      If Not Current.Request.AppRelativeCurrentExecutionFilePath.EndsWith(ErrorPageName) Then

        Dim LastError As Exception = System.Web.HttpContext.Current.Server.GetLastError

        If LastError IsNot Nothing Then

          Dim ErrorID As Integer = -1

          Try
            Dim Err As String = Singular.Debug.RecurseExceptionMessage(LastError)
            'Shorten common errors.
            If LastError.StackTrace.Trim.StartsWith("Object Reference", Global.System.StringComparison.InvariantCultureIgnoreCase) Then
              Err = "Null Reference"
            End If

            If LastError.StackTrace.Trim.StartsWith("at System.Web.UI.ViewStateException.ThrowError(") Then
              Err = "View State Error"
            End If
            Dim ClientInfo As String = ""
            Try
              'Get the Browser / Client Info
              ClientInfo &= HttpContext.Current.Request.Browser.Browser & HttpContext.Current.Request.Browser.MajorVersion & "; "
              ClientInfo &= HttpContext.Current.Request.Browser.Platform

            Catch ex As Exception

            End Try

            ErrorID = LogError(Request.Path,
                                ClientInfo,
                                Singular.Debug.GetCleanStackTrace(LastError),
                                Err, OtherData)


          Catch ex As Exception

          End Try

          If Current.Session IsNot Nothing Then
            Current.Session("AppErrorLocation") = Current.Request.Path
            Current.Session("AppErrorID") = ErrorID
            Current.Session("AppErrorText") = ""
            Current.Session("AppError") = LastError
          End If

          If Current.Handler Is Nothing OrElse TypeOf System.Web.HttpContext.Current.Handler Is System.Web.UI.Page Then
            'Error came from normal page
            If Not MustShowDetailedErrors Then
              Dim ErrorCode As Integer = -1
              If LastError IsNot Nothing AndAlso TypeOf LastError Is System.Web.HttpException Then
                ErrorCode = CType(LastError, System.Web.HttpException).GetHttpCode
              End If

              Dim QS As String = ""
              If ErrorCode = 404 Then
                QS = "?code=" & ErrorCode
              ElseIf ErrorID > 0 Then
                QS = "?id=" & ErrorID
              End If

              If LastError IsNot Nothing AndAlso TypeOf LastError.InnerException Is UnauthorizedAccessException Then
                If QS <> "" Then
                  QS &= "&"
                End If
                QS &= "type=UnauthorizedAccessException"
              End If

              Current.Response.Redirect(ErrorPagePath & QS)
            End If

          ElseIf IsHTTPHandler Then
            'Error came from http handler
            Current.Response.Clear()
            If MustShowDetailedErrors Then
              Current.Response.Write(Singular.Debug.RecurseExceptionMessage(LastError))
            Else
              Current.Response.Write("An unhandled error occured: " & ErrorID)
            End If
            If TypeOf LastError Is HttpException Then
              Current.Response.StatusCode = CType(LastError, HttpException).GetHttpCode()
            Else
              Current.Response.StatusCode = 500
            End If

            Current.Response.Flush()
            Current.Response.Close()
            Current.Response.End()

          End If

        End If

      Else
        'Error on error page, probably a connection issue
        If Not String.IsNullOrEmpty(NoConnectionPage) Then

          Dim NoConnectionPageName As String = NoConnectionPage.Substring(NoConnectionPage.LastIndexOf("/") + 1)

          If Not Current.Request.AppRelativeCurrentExecutionFilePath.EndsWith(NoConnectionPageName) Then
            Current.Response.Redirect(NoConnectionPage)
          End If

        End If
      End If

    End If

  End Sub

  Public Shared Sub LogEvent(Section As String, Info As String)

    If SuppressLog = False Then

      Dim cProc As New Singular.CommandProc("[CmdProcs].[cmdInsWebLog]")
      cProc.Parameters.AddWithValue("@Section", Section)
      cProc.Parameters.AddWithValue("@Info", Info)

      'cProc.FetchType = CommandProc.FetchTypes.DataRow
      cProc = cProc.Execute()

    End If

  End Sub

End Class
