Public Class CSSFile

  Private mFullPath As String
  Private mVersionNo As String = ""
  Private mStyleTag As String
  Private mMinifiedBytes As Byte()

  Private mProcessFunction As Action(Of System.Text.StringBuilder)
  Private mProcessedByes As Byte()
  Private mLastModifiedDate As Date = Date.MinValue

  Friend Shared Property LibThirdPartyURL As String = "~/Singular/CSS/"

  Public ReadOnly Property StyleTag As IHtmlString
    Get
      Return New HtmlString(mStyleTag)
    End Get
  End Property

  Private Shared _Files As New Dictionary(Of String, CSSFile)

  Public Shared Function RenderLibraryStyles() As IHtmlString
    Return RenderStyleTag("~/" & Scripts.LibResourceHandler.HandlerURL & "/CSS/Singular.css")
  End Function

  Public Shared Function IncludeSGridStyles() As IHtmlString
    Return RenderStyleTag("~/" & Scripts.LibResourceHandler.HandlerURL & "/CSS/SGrid.css")
  End Function

  Public Shared Property FontAwesomeVersion As String = "4.7.0"

  Public Shared Function IncludeFontAwesome() As IHtmlString

    If Debugger.IsAttached OrElse Not Singular.Web.Scripts.Settings.UseCDN Then
      Return RenderStyleTag(LibThirdPartyURL & "font-awesome.css")
    Else
      Return New HtmlString("<link href=""//maxcdn.bootstrapcdn.com/font-awesome/" & FontAwesomeVersion & "/css/font-awesome.min.css"" rel=""stylesheet"">")
    End If

  End Function

  ''' <summary>
  ''' Renders a style tag and changes the file name to match the version no.
  ''' </summary>
  Public Shared Function RenderStyleTag(RelativePath As String) As IHtmlString
    Return GetFile(RelativePath).StyleTag
  End Function

  Public Shared Function GetFile(RelativePath As String) As CSSFile

    Dim File As CSSFile = Nothing

    If Not _Files.TryGetValue(RelativePath, File) Then
      SyncLock _Files
        If Not _Files.TryGetValue(RelativePath, File) Then
          File = New CSSFile(RelativePath)
          _Files.Add(RelativePath, File)
        End If
      End SyncLock
    End If

    Return File

  End Function

  ''' <summary>
  ''' Call this from global.asax to add and process a style file.
  ''' </summary>
  Public Shared Sub AddFile(Path As String, ProcessFile As Action(Of System.Text.StringBuilder))
    Dim File As New CSSFile(Path, ProcessFile)
    _Files.Add(Path, File)
  End Sub

  Private Sub New(RelativePath As String, Optional ProcessFunction As Action(Of System.Text.StringBuilder) = Nothing)

    mProcessFunction = ProcessFunction

    If Not RelativePath.StartsWith("~/") Then Throw New ArgumentException("Path must start with '~/'")

    If RelativePath.Contains("..") OrElse RelativePath.Contains(":") OrElse RelativePath.Contains("\\") Then
      Throw New HttpException(400, "Invalid path")
    End If

    Dim FullURL As String = Utils.URL_ToAbsolute(RelativePath)
    Dim LibPath = "~/" & Scripts.LibResourceHandler.HandlerURL

    If RelativePath.StartsWith("~/Singular") Then
      mFullPath = RelativePath.Replace("~/Singular", Scripts.Settings.SingularPhysicalPath).Replace("/", "\")
    Else
      mFullPath = Utils.Server_MapPath(RelativePath)
    End If

    Dim IsLibPath As Boolean = False

    'Get the contents of the file.
    Dim FileContent As String

    If RelativePath.StartsWith(LibPath) Then
      Dim Content = Scripts.LibResourceHandler.GetEmbeddedContent(IO.Path.GetFileName(RelativePath))
      Using ms As New IO.MemoryStream(Content)
        Using sr As New IO.StreamReader(ms)
          FileContent = sr.ReadToEnd
        End Using
      End Using
      IsLibPath = True
    Else
      FileContent = IO.File.ReadAllText(mFullPath)
    End If

    If ProcessFunction IsNot Nothing Then
      FileContent = ProcessFile(FileContent)
    End If

    'Check if the file has a version no
    If FileContent.StartsWith("/* Version ", StringComparison.InvariantCultureIgnoreCase) Then
      Dim EndVersionIndex = FileContent.IndexOf("*/")
      mVersionNo = FileContent.Substring(11, EndVersionIndex - 11).Replace("*/", "").Trim
    End If

    If mVersionNo <> "" Then
      'If its versioned, minify it, and serve it from the lib resource handler.

      Dim jsm As New Microsoft.Ajax.Utilities.Minifier
      Dim MinifiedText = jsm.MinifyStyleSheet(FileContent)
      mMinifiedBytes = System.Text.Encoding.UTF8.GetBytes(MinifiedText)


      Dim Path = RelativePath.Substring(0, RelativePath.LastIndexOf("/") + 1)
      Dim File = IO.Path.GetFileName(RelativePath)
      Dim Type = "rc"

      If Debugger.IsAttached AndAlso IsLibPath Then
        Type = "d"
        Path = Scripts.LibResourceHandler.GetSubPath(LibPath, Path)
      End If
      FullURL = Scripts.LibResourceHandler.GetURL(File, Path, Type, mVersionNo, True)

    End If

    mStyleTag = "<link href=""" & FullURL & """ rel=""stylesheet"" type=""text/css"" />" & vbCrLf

  End Sub

  Private Function ProcessFile(Content As String) As String
    Dim sb As New System.Text.StringBuilder(Content)
    mProcessFunction(sb)
    Dim FileContent = sb.ToString()
    mProcessedByes = System.Text.Encoding.UTF8.GetBytes(FileContent)
    Return FileContent
  End Function

  Friend Sub WriteContents(Response As System.Web.HttpResponse)

    Response.BinaryWrite(GetProcessedBytes)

  End Sub

  Public Function GetProcessedBytes() As Byte()

    If Debugger.IsAttached Then
      If mProcessedByes IsNot Nothing Then
        'Check if the file modified date has changed.
        Dim fi As New IO.FileInfo(mFullPath)
        If fi.LastWriteTime > mLastModifiedDate Then
          mLastModifiedDate = fi.LastWriteTime
          ProcessFile(IO.File.ReadAllText(mFullPath))
        End If
        Return mProcessedByes
      Else
        Return IO.File.ReadAllBytes(mFullPath)
      End If

    Else
      Return mMinifiedBytes
    End If

  End Function

End Class
