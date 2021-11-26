Namespace Misc

  Public Module Misc

    ''' <summary>
    ''' Removes any query string from a url.
    ''' </summary>
    ''' <param name="URL"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPageFromURL(ByVal URL As String) As String

      Dim PageName As String = ""

      If URL.Contains("?") Then
        PageName = URL.Substring(0, URL.IndexOf("?"))
      Else
        PageName = URL
      End If

      If PageName.Contains("/") Then
        Return PageName.Substring(PageName.LastIndexOf("/") + 1)
      Else
        Return PageName
      End If


    End Function

    Public Function GetHTMLColourCode(Colour As System.Drawing.Color) As String

      Return System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(Colour.R, Colour.G, Colour.B))

    End Function

    ''' <summary>
    ''' Returns false if this is the first time the page is being requested. True if the page is being posted back, e.g. the user clicked a button.
    ''' This is an alternative to Page.IsPostBack if you don't have a reference to a Page control.
    ''' </summary>
    Public ReadOnly Property IsPostBack As Boolean
      Get
        Return System.Web.HttpContext.Current.Request.RequestType = "POST"
      End Get
    End Property

  End Module

End Namespace

