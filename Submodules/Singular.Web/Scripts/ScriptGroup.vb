Imports System.Text

Namespace Scripts

  Public Class ScriptGroup
    Inherits List(Of ScriptFile)

    Private mCombineAndMinify As Boolean = False
    'Private mReleasePath As String = ""
    Private mMainScript As ScriptFile = Nothing
    Private mCheckVersion As String = ""
    Private _VersionNo As String = ""
    Private mMinifiedBytes As Byte()

    ''' <summary>
    ''' Name of the group used with RenderScriptGroup
    ''' </summary>
    Public Property GroupName As String

    ''' <summary>
    ''' The path where the files in this group are located.
    ''' </summary>
    Public Property Path As String

    ''' <summary>
    ''' All files in this group will be merged into one file and minified.
    ''' </summary>
    ''' <param name="MainScript">The script which contains the release name, and version no if versioning required.</param>
    ''' <param name="CheckVersion">The version the script file must equal. Useful to check in case the script files haven't been copied to the web server.</param>
    Public Sub EnableCombineAndMinify(MainScript As ScriptFile, Optional CheckVersion As String = "")

      mCombineAndMinify = True
      mMainScript = MainScript
      mCheckVersion = CheckVersion
    End Sub

    ''' <summary>
    ''' All files in this group will be merged into one file and minified.
    ''' </summary>
    ''' <param name="ServerVersion">The version of the scripts. This version will be added to the file name.</param>
    ''' <remarks></remarks>
    Public Sub EnableCombineAndMinify(ServerVersion As String)
      mCombineAndMinify = True
      _VersionNo = ServerVersion
    End Sub

    ''' <summary>
    ''' All files in this group will be merged into one file and minified.
    ''' The version no will be retrieved from the assembly version no. Note, * must be used in the assembly info file for auto version increment.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EnableCombineAndMinify()

      EnableCombineAndMinify(System.Reflection.Assembly.GetCallingAssembly.GetName.Version.ToString)

    End Sub

    ''' <summary>
    ''' Adds a script to this group.
    ''' </summary>
    ''' <param name="DebugScriptName">The name of the file with the readable source code.</param>
    ''' <param name="ReleaseScriptName">The name of the minified file.</param>
    ''' <param name="CDNPath">The complete URL of the file on a CDN</param>
    Public Function AddScript(DebugScriptName As String, ReleaseScriptName As String, CDNPath As String) As ScriptFile
      Dim Script As New ScriptFile With {.DebugScriptName = DebugScriptName, .ReleaseScriptName = ReleaseScriptName, .CDNPath = CDNPath}
      Add(Script)
      Return Script
    End Function

    ''' <summary>
    ''' Adds a script to this group.
    ''' </summary>
    ''' <param name="DebugScriptName">The name of the file with the readable source code.</param>
    ''' <param name="ReleaseScriptName">The name of the minified file.</param>
    Public Function AddScript(DebugScriptName As String, ReleaseScriptName As String) As ScriptFile
      Return AddScript(DebugScriptName, ReleaseScriptName, "")
    End Function

    ''' <summary>
    ''' Adds a script to this group.
    ''' </summary>
    ''' <param name="DebugScriptName">The name of the file with the readable source code.</param>
    Public Function AddScript(DebugScriptName As String) As ScriptFile
      Return AddScript(DebugScriptName, "", "")
    End Function

    Private mRenderedScriptTag As String = ""
    Friend Function RenderScriptTags(Optional sb As StringBuilder = Nothing) As String

      If String.IsNullOrEmpty(mRenderedScriptTag) Then
        If sb Is Nothing Then
          sb = New Text.StringBuilder
        End If
        If mCombineAndMinify AndAlso Not UseDebugScripts Then

          'Render the tag for the minified group.
          Dim LibPath = Utils.URL_ToAbsolute("~/" & LibResourceHandler.HandlerURL)
          Dim ReleaseName As String
          If mMainScript IsNot Nothing Then
            ReleaseName = mMainScript.ReleaseScriptName
          Else
            ReleaseName = GroupName & ".js"
          End If

          sb.Append(ScriptFile.GetScriptTag(LibResourceHandler.GetURL(GroupName & ".js", GroupName, "sg", _VersionNo)))

        Else

          'Render the individual scripts.
          For Each sf As ScriptFile In Me
            sf.RenderScriptTag(Path, sb)
          Next
        End If
        mRenderedScriptTag = sb.ToString
      Else
        If sb IsNot Nothing Then
          sb.Append(mRenderedScriptTag)
        End If
      End If
      Return mRenderedScriptTag

    End Function

    Friend Sub GenerateCombinedScript()

      If mCombineAndMinify Then

        'Check if the main script has a version no.
        If mMainScript IsNot Nothing AndAlso String.IsNullOrEmpty(_VersionNo) Then
          mMainScript.FindVersionNo(Path)

          If mCheckVersion <> "" AndAlso mCheckVersion <> mMainScript.VersionNo Then
            Throw New Exception("Script Group '" & GroupName & "' version mismatch: Client " & mMainScript.VersionNo & " / Server " & mCheckVersion)
            Exit Sub
          End If

          _VersionNo = mMainScript.VersionNo

        End If


        'Combine all the files into one.
        Using ms As New IO.MemoryStream

          Dim NewLine As Byte() = Encoding.ASCII.GetBytes(Environment.NewLine)

          For Each sf As ScriptFile In Me
            Dim Contents = sf.ReadContents(Path)
            ms.Write(Contents, 0, Contents.Length)
            ms.Write(NewLine, 0, NewLine.Length)
            sf.ReleaseContents()
          Next

          ms.Position = 0
          Using sr As New IO.StreamReader(ms)
            Dim jsm As New Microsoft.Ajax.Utilities.Minifier
            Dim MinText As String = jsm.MinifyJavaScript(sr.ReadToEnd())
            mMinifiedBytes = System.Text.Encoding.UTF8.GetBytes(MinText)
          End Using

        End Using

      End If

    End Sub

    Public ReadOnly Property MinifiedBytes As Byte()
      Get
        Return mMinifiedBytes
      End Get
    End Property

  End Class

End Namespace

