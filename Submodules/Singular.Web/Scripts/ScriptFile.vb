Imports System.Text

Namespace Scripts

  Public Class ScriptFile

    Public Property Path As String
    Public Property DebugScriptName As String
    Public Property ReleaseScriptName As String
    Public Property CDNPath As String
    Friend Property VersionNo As String = ""

    Public Sub New()

    End Sub

    Public Sub New(Path As String, DebugScriptName As String)
      Me.Path = Path
      Me.DebugScriptName = DebugScriptName
      Me.ReleaseScriptName = DebugScriptName
    End Sub

    Public Sub New(Path As String, DebugScriptName As String, ReleaseScriptName As String)
      Me.Path = Path
      Me.DebugScriptName = DebugScriptName
      Me.ReleaseScriptName = ReleaseScriptName
    End Sub

    Private ReadOnly Property DebugOnly As Boolean
      Get
        Return ReleaseScriptName = "" AndAlso CDNPath = ""
      End Get
    End Property

    ''' <summary>
    ''' Returns the Debug / release / CDN path depending on the debugger being attached, settings etc.
    ''' </summary>
    Public ReadOnly Property ScriptTag As IHtmlString
      Get
        Return GetScriptTag(ScriptPath)
      End Get
    End Property

    Private _ScriptPath As String
    Private _HasVirtualFolder As Boolean?

    Private ReadOnly Property HasVirtualFolder As Boolean
      Get
        If _HasVirtualFolder Is Nothing Then
          Try
            Dim PhysicalPath = Utils.Server_MapPath("~/Singular")
            _HasVirtualFolder = IO.Directory.Exists(PhysicalPath)
          Catch ex As Exception
            _HasVirtualFolder = False
          End Try
        End If
        Return _HasVirtualFolder
      End Get
    End Property



    Private ReadOnly Property ScriptPath As String
      Get
        If String.IsNullOrEmpty(_ScriptPath) Then

          Dim Type As String = ""
          Dim IsDebug As Boolean = False

          If UseDebugScripts OrElse DebugOnly Then
            _ScriptPath = Utils.URL_ToAbsolute(Path) & "/" & DebugScriptName
            IsDebug = True
            Type = "d"
          Else
            If CDNPath = "" OrElse Not Settings.UseCDN Then
              _ScriptPath = Utils.URL_ToAbsolute(Path) & "/" & ReleaseScriptName
            Else
              _ScriptPath = CDNPath
            End If

          End If

          Dim LibPath = Utils.URL_ToAbsolute("~/" & LibResourceHandler.HandlerURL)

          If _ScriptPath.StartsWith(LibPath) Then
            'This is a library script

            If HasVirtualFolder AndAlso IsDebug Then
              'If the virtual directory for Singular exists, use it (debug only).
              _ScriptPath = _ScriptPath.Replace(LibPath, Utils.URL_ToAbsolute("~/Singular"))
            Else
              'Otherwise serve from the resource handler.
              Dim SubPath = LibResourceHandler.GetSubPath(LibPath, _ScriptPath)
              Dim File = IO.Path.GetFileName(_ScriptPath)
              If String.IsNullOrEmpty(VersionNo) Then VersionNo = Singular.Web.ScriptsVersion
              _ScriptPath = LibResourceHandler.GetURL(File, SubPath, Type, VersionNo)
            End If

          End If
        End If
        Return _ScriptPath

      End Get
    End Property

    Friend Sub RenderScriptTag(Path As String, sb As StringBuilder)

      Me.Path = Path
      sb.Append(ScriptTag)

    End Sub

    Public Shared Function GetScriptTag(FilePath As String) As IHtmlString
      If FilePath.StartsWith("~") Then
        FilePath = Utils.URL_ToAbsolute(FilePath)
      End If
      Return New HtmlString(vbCrLf & "<script src=""" & FilePath & """ type=""text/javascript"" ></script>")
    End Function

    Private _FileBytes As Byte()
    Friend Function ReadContents(Path As String) As Byte()

      If _FileBytes Is Nothing Then
        If Path.StartsWith("~/" & LibResourceHandler.HandlerURL) Then
          _FileBytes = LibResourceHandler.GetEmbeddedContent(DebugScriptName)
        Else
          _FileBytes = IO.File.ReadAllBytes(IO.Path.Combine(Utils.Server_MapPath(Path), DebugScriptName))
        End If
      End If

      Return _FileBytes

    End Function

    Friend Sub ReleaseContents()
      _FileBytes = Nothing
    End Sub

    Friend Sub FindVersionNo(Path As String)

      Dim Index As Integer = 0

      Using ms As New IO.MemoryStream(ReadContents(Path))
        Using sr As New IO.StreamReader(ms)

          While Not sr.EndOfStream AndAlso Index < 5

            Dim Line = sr.ReadLine

            'Check the first 5 lines for the version no
            If Line.StartsWith("// Version ", StringComparison.InvariantCultureIgnoreCase) Then
              VersionNo = Line.Substring(11)
              Exit Sub
            ElseIf Line.StartsWith("//Version ", StringComparison.InvariantCultureIgnoreCase) Then
              VersionNo = Line.Substring(10)
              Exit Sub
            End If

            Index += 1

          End While

        End Using
      End Using

    End Sub

  End Class


End Namespace


