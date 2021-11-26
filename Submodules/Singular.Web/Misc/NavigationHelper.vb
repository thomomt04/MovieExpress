Imports System.Web.HttpContext

Namespace Misc

  Public Class NavigationHelper

    Public Class RedirectOptions
      Public RedirectedFrom As String
      Public RedirectedTo As String
    End Class

    ''' <summary>
    ''' Redirects the user to a page, and remembers the current page. When the user has done what they need to on the new page, call RedirectBack() to come back to this page.
    ''' </summary>
    ''' <param name="RedirectTo"></param>
    ''' <remarks></remarks>
    Public Shared Sub RedirectAndRemember(ByVal RedirectTo As String)
      'Don't redirect to the same page that the user is on.
      If Not Current.Request.AppRelativeCurrentExecutionFilePath.EndsWith(Singular.Web.Misc.GetPageFromURL(RedirectTo)) Then

        Dim ro As RedirectOptions = Current.Session("NavigationHelper.RedirectPageFrom")
        If ro Is Nothing Then
          ro = New RedirectOptions
          ro.RedirectedFrom = Current.Request.AppRelativeCurrentExecutionFilePath
        End If
        ro.RedirectedTo = RedirectTo

        Current.Session("NavigationHelper.RedirectPageFrom") = ro
        Current.Response.Redirect(RedirectTo, True)
      End If
    End Sub

    ''' <summary>
    ''' Checks if the page you are calling this from, was redirected from another page.
    ''' If so, then redirect back to the original page.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub RedirectBack(Optional DefaultPage As String = "")
      If Current.Session("NavigationHelper.RedirectPageFrom") IsNot Nothing Then
        Dim RO As RedirectOptions = Current.Session("NavigationHelper.RedirectPageFrom")

        If WasRedirected() Then
          Current.Session("NavigationHelper.RedirectPageFrom") = Nothing
          Current.Response.Redirect(RO.RedirectedFrom)
          Exit Sub
        End If
      End If

      If DefaultPage <> "" Then
        Current.Response.Redirect(DefaultPage)
      End If

    End Sub

    ''' <summary>
    ''' Checks if the user was redirected to the current page.
    ''' </summary>
    Public Shared Function WasRedirected() As Boolean
      If Current.Session("NavigationHelper.RedirectPageFrom") IsNot Nothing Then
        Return Current.Request.AppRelativeCurrentExecutionFilePath.EndsWith(Singular.Web.Misc.GetPageFromURL(CType(Current.Session("NavigationHelper.RedirectPageFrom"), RedirectOptions).RedirectedTo))
      Else
        Return False
      End If
    End Function

    Public Shared Function RedirectPath() As String
      If WasRedirected() Then
        Dim RO As RedirectOptions = Current.Session("NavigationHelper.RedirectPageFrom")
        If RO.RedirectedFrom.StartsWith("~") Then
          Return Utils.URL_ToAbsolute(RO.RedirectedFrom)
        Else
          Return RO.RedirectedFrom
        End If

      Else
        Return ""
      End If
    End Function

  End Class

End Namespace


