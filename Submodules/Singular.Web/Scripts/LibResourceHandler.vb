Namespace Scripts

  Public Class LibResourceHandler
    Implements System.Web.IHttpHandler

    Public Const HandlerURL As String = "LibResources"

    Private Shared _HasPhysicalFiles As Boolean?

    Private Shared _Resources As New Dictionary(Of String, Byte())

    Public Shared Property UseSafeFileNames As Boolean = True

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
      Get
        Return False
      End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

      Dim RelativePath As String = If(context.Request.QueryString("path"), "")

      If RelativePath.Contains("..") OrElse RelativePath.Contains(":") OrElse RelativePath.Contains("\\") Then
        Throw New HttpException(400, "Invalid path")
      End If

      Dim Type As String = If(context.Request.QueryString("type"), "")
      Dim FileName = context.Request.QueryString("file")

      If Not String.IsNullOrEmpty(FileName) Then

        'Add mime type, and make the browser cache the file
        context.Response.ContentType = Singular.Web.WebServices.FileDownloadHandler.GetMimeType(FileName)
        If Not Debugger.IsAttached Then
          context.Response.Cache.SetExpires(Now.AddDays(180))
          context.Response.Cache.SetCacheability(HttpCacheability.Public)
        End If

        If Type = "d" AndAlso Debugger.IsAttached Then
          'Debug file requested - try serve the file from disk, assuming the Singular.Web directory is in the correct place.

          If _HasPhysicalFiles Is Nothing Then
            _HasPhysicalFiles = IO.Directory.Exists(Scripts.Settings.SingularPhysicalPath)
          End If

          If _HasPhysicalFiles Then
            context.Response.WriteFile(IO.Path.Combine(Scripts.Settings.SingularPhysicalPath, RelativePath, FileName))
            Exit Sub
          End If

        End If

        If Type = "rc" Then
          'CSS Minified
          CSSFile.GetFile(IO.Path.Combine(RelativePath, FileName)).WriteContents(context.Response)
          Exit Sub
        End If

        If Type = "sg" Then
          'Script Group
          context.Response.BinaryWrite(Scripts.AppScriptManager.GetGroup(RelativePath).MinifiedBytes)
          Exit Sub
        End If

        If Type = "l" Then
          'Localisation strings
          context.Response.BinaryWrite(JavascriptLocaliser.GetLocalisedResourceStrings(RelativePath))
          Exit Sub
        End If

        'If none of the above match, then get the content from the assembly.
        context.Response.BinaryWrite(GetEmbeddedContent(FileName))

      End If

    End Sub

    Public Shared Function GetLibImageURL(ImageName As String) As String
      Return String.Format("{0}/img?file={1}", Utils.URL_ToAbsolute("~/" & HandlerURL), ImageName)
    End Function

    Friend Shared Function GetSubPath(RootPath As String, FullPath As String) As String
      Dim SubPath = FullPath.Substring(RootPath.Length + 1)
      If SubPath.Contains("/") Then
        Return SubPath.Substring(0, SubPath.LastIndexOf("/"))
      Else
        Return String.Empty
      End If
    End Function

    Friend Shared Function GetURL(FileName As String, Path As String, Type As String, VersionNo As String, Optional AddLevels As Boolean = False) As String

      Dim HandlerPath = "~/" & HandlerURL & "/"

      If AddLevels AndAlso Path.StartsWith("~/") Then
        HandlerPath &= Path.Substring(Path.IndexOf("/", 2) + 1)
      End If

      Dim SafeFileName = If(UseSafeFileNames, FileName.Replace(".", "_"), FileName)

      Return String.Format("{0}?type={1}&v={2}&path={3}&file={4}", Utils.URL_ToAbsolute(HandlerPath & SafeFileName),
                                                                                                 Type, VersionNo, Path, FileName)
    End Function

    Public Shared Function GetEmbeddedContent(FileName As String) As Byte()

      Dim Contents() As Byte = Nothing

      'Check if the file has been fetched from embedded resources.
      If Not _Resources.TryGetValue(FileName, Contents) Then
        SyncLock _Resources
          If Not _Resources.TryGetValue(FileName, Contents) Then
            Dim str = System.Reflection.Assembly.GetAssembly(GetType(LibResourceHandler)).GetManifestResourceStream("Singular.Web." & FileName)
            ReDim Contents(str.Length - 1)
            str.Read(Contents, 0, Contents.Length)
            _Resources.Add(FileName, Contents)
          End If
        End SyncLock
      End If

      Return Contents

    End Function

  End Class

End Namespace


