Namespace Misc

  Public Class BroswerInfo

    Public Shared ReadOnly Property BrowserInfo As System.Web.HttpBrowserCapabilities
      Get
        Return System.Web.HttpContext.Current.Request.Browser
      End Get
    End Property

    Public Enum BrowserType
      IE = 1
      Firefox = 2
      Chrome = 3
      Opera = 4
      Safari = 5
      Other = 999
    End Enum

    Public Shared ReadOnly Property Browser As BrowserType
      Get
        If BrowserInfo.Browser.ToLower = "ie" OrElse BrowserInfo.Browser.ToLower.Contains("explorer") Then
          Return BrowserType.IE
        ElseIf BrowserInfo.Browser.ToLower = "firefox" Then
          Return BrowserType.Firefox
        ElseIf BrowserInfo.Browser.ToLower.Contains("safari") Then
          Return BrowserType.Safari
        ElseIf BrowserInfo.Browser.ToLower.Contains("chrome") Then
          Return BrowserType.Chrome
        ElseIf BrowserInfo.Browser.ToLower.Contains("opera") Then
          Return BrowserType.Opera
        End If
        Return BrowserType.Other
      End Get
    End Property

    Public Shared ReadOnly Property BroswerInfoString As String
      Get
        Return Browser.ToString & " " & BrowserInfo.MajorVersion
      End Get
    End Property

    Public Shared ReadOnly Property IsPhone As Boolean
      Get
        Try
          Dim ua = System.Web.HttpContext.Current.Request.UserAgent.ToLower
          Return ua.Contains("mobile") OrElse ua.Contains("iphone") OrElse ua.Contains("blackberry")
        Catch ex As Exception
          Return False
        End Try
      End Get
    End Property

    ''' <summary>
    ''' True if the browser has native support for JSON.parse and JSON.stringify
    ''' </summary>
    Public Shared ReadOnly Property SupportsJSon As Boolean
      Get
        Select Case Browser
          Case BrowserType.IE
            Return BrowserInfo.MajorVersion >= 8
          Case BrowserType.Firefox
            Return BrowserInfo.MajorVersion >= 4
          Case BrowserType.Chrome
            Return BrowserInfo.MajorVersion >= 4
          Case BrowserType.Opera
            Return BrowserInfo.MajorVersion >= 11
          Case BrowserType.Safari
            Return BrowserInfo.MajorVersion >= 4
        End Select
        Return False
      End Get
    End Property

    Public Shared ReadOnly Property SupportsLocalStorage As Boolean
      Get
        If IsPhone Then
          'Dont do local storage on phones.
          Return False
        End If
        Select Case Browser
          Case BrowserType.IE
            Return BrowserInfo.MajorVersion >= 8
          Case BrowserType.Firefox
            Return BrowserInfo.MajorVersion >= 4
          Case BrowserType.Chrome
            Return BrowserInfo.MajorVersion >= 4
          Case BrowserType.Opera
            Return BrowserInfo.MajorVersion >= 11
          Case BrowserType.Safari
            Return BrowserInfo.MajorVersion >= 4
        End Select
        Return False
      End Get
    End Property

    Public Shared ReadOnly Property SupportsFileAPI As Boolean
      Get
        If Browser = BrowserType.IE AndAlso BrowserInfo.MajorVersion <= 9 Then
          Return False
        Else
          Return True
        End If
      End Get
    End Property

  End Class

End Namespace


