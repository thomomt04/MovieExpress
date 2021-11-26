Namespace Scripts

  Public Enum ScriptLocation
    Project = 1
    LibraryVirtualDir = 2 'Jquery, knockout etc
    LibraryEmbedded = 3
  End Enum

  Public Enum ScriptGroupType
    Knockout = 0
    JQuery = 1
    Singular = 2
    SGrid = 3
    GridReport = 4
  End Enum

  Public Module Scripts
    Public Property Settings As New ScriptSettings

    Private mAppScriptManager As ScriptManager
    Public ReadOnly Property AppScriptManager As ScriptManager
      Get
        If mAppScriptManager Is Nothing Then
          mAppScriptManager = New ScriptManager
        End If
        Return mAppScriptManager
      End Get
    End Property

    Public ReadOnly Property UseDebugScripts As Boolean
      Get
        Return Singular.Debug.InDebugMode OrElse Settings.AlwaysUseDebugScripts
      End Get
    End Property

    Public Function GetPath(Location As ScriptLocation, Optional SubPath As String = "") As String

      Dim BasePath As String = String.Empty

      If Location = ScriptLocation.LibraryVirtualDir Then
        BasePath = Settings.LibraryScriptsThirdPartyPath
      ElseIf Location = ScriptLocation.LibraryEmbedded Then
        BasePath = Settings.LibraryScriptsResPath
      Else
        BasePath = Settings.ScriptsPath
      End If

      Return BasePath & If(String.IsNullOrEmpty(SubPath), String.Empty, If(BasePath.EndsWith("/"), "", "/") & SubPath)
    End Function


    Private mLibraryScript As String = Nothing
    ''' <summary>
    ''' Renders the script tags to include all the .js files required by the singular library.
    ''' </summary>
    Public Function RenderLibraryScripts() As IHtmlString
      SyncLock mAppScriptManager
        If mLibraryScript Is Nothing Then
          mLibraryScript = AppScriptManager.RenderLibraryScripts()
        End If

        'Localised resources:
        If Singular.Localisation.LocalisationEnabled AndAlso Singular.Localisation.CurrentLanguageID <> 1 Then
          Return New HtmlString(mLibraryScript & ScriptFile.GetScriptTag(JavascriptLocaliser.LocalisedScriptURL(Singular.Localisation.CurrentCulture.TwoLetterISOLanguageName)).ToString)
        Else
          Return New HtmlString(mLibraryScript)
        End If
      End SyncLock
    End Function

    Public Function RenderGridScripts() As IHtmlString
      Return New HtmlString(RenderScriptGroup(ScriptGroupType.SGrid.ToString).ToString & vbCrLf & RenderScriptGroup(ScriptGroupType.GridReport.ToString).ToString)
    End Function

    Public Function RenderScriptGroup(GroupName As String) As IHtmlString

      Return New HtmlString(AppScriptManager.GetGroup(GroupName).RenderScriptTags)

    End Function

    Public Function RenderScriptFile(DebugName As String) As IHtmlString

      Return ScriptFile.GetScriptTag(Settings.ScriptAbsolutePath & "/" & DebugName)

    End Function

    Public Function RenderScriptFile(DebugName As String, ReleaseName As String) As IHtmlString

      If UseDebugScripts Then
        Return RenderScriptFile(DebugName)
      Else
        Return ScriptFile.GetScriptTag(Settings.ScriptAbsolutePath & "/" & ReleaseName)
      End If

    End Function

    Public Function RenderGoogleAnalyticsSetup(TrackingCode As String, URL As String) As IHtmlString

      If TrackingCode <> "" Then
        Return New HtmlString(vbCrLf & String.Format(My.Resources.GoogleAnalyticsInclude, TrackingCode, URL))
      Else
        Return New HtmlString("")
      End If

    End Function

  End Module

  Public Class ScriptSettings

    ''' <summary>
    ''' Project scripts
    ''' </summary>
    Public Property ScriptsPath As String = "~/Scripts"

    ''' <summary>
    ''' Library scripts when referenced from virtual directory
    ''' </summary>
    Public Property LibraryScriptsOldPath As String = "~/Singular/Javascript"

    Public Property LibraryScriptsThirdPartyPath As String = "~/Singular/Javascript"

    ''' <summary>
    ''' Library scripts when referenced from resource handler
    ''' </summary>
    Public Property LibraryScriptsResPath As String = "~/" & LibResourceHandler.HandlerURL & "/Javascript"

    Private mScriptAbsolutePath As String = ""
    Friend ReadOnly Property ScriptAbsolutePath As String
      Get
        If mScriptAbsolutePath = "" Then
          mScriptAbsolutePath = Utils.URL_ToAbsolute(ScriptsPath)
        End If
        Return mScriptAbsolutePath
      End Get
    End Property

    ''' <summary>
    ''' Tells the script manager to always make the browser load debug javascript files., even when the web site is not running in debug mode.
    ''' </summary>
    Public Property AlwaysUseDebugScripts As Boolean = False

    ''' <summary>
    ''' Tells the script manager to make the browser load javascript files from a content delivery network. Debug files will still be loaded locally when in debug mode.
    ''' </summary>
    Public Property UseCDN As Boolean = True

    Public Enum JQueryVersion
      JQ_1_7_2 = 1
      JQ_1_12_4 = 2
      JQ_2_2_4 = 3
    End Enum

    Public Enum JQueryUIVersion
      JQ_UI_1_9_0 = 1
      JQ_UI_1_12_1 = 2
    End Enum

    Public Property LibJQueryVersion As JQueryVersion = JQueryVersion.JQ_1_7_2

    Public Property LibJQueryUIVersion As JQueryUIVersion = JQueryUIVersion.JQ_UI_1_9_0

    ''' <summary>
    ''' Uses version 2.3.0 of knockout, including fixed security vulnerabilities.
    ''' </summary>
    ''' <returns></returns>
    Public Property UsePatchedKnockoutFiles As Boolean = False

    Public Property SupportECMA6 As Boolean = False

    ''' <summary>
    ''' Specifies an alternate location for Singular\Files if you dont have a Singular virtual directory.
    ''' </summary>
    ''' <param name="SingularPath">The physical file path, or path relative to your site root. e.g. ..\..\Singular.Web</param>
    ''' <param name="StylesUrl">Where are the non Singular styles found? e.g. ~/styles/font-awesome.css</param>
    ''' <param name="ScriptsUrl">Where are the non Singular scripts found? e.g. knockout / jquery. Leave blank if they are in your ~/scripts folder.</param>
    Public Sub ByPassSingularVirtualDir(Optional SingularPath As String = "..\..\Singular.Web", Optional StylesUrl As String = "~/Styles/", Optional ScriptsUrl As String = Nothing)

      _SingularPhysicalPath = IO.Path.Combine(Utils.Server_MapPath("~"), SingularPath.Replace("/", "\\"), "Files")
      CSSFile.LibThirdPartyURL = StylesUrl
      LibraryScriptsThirdPartyPath = If(String.IsNullOrEmpty(ScriptsUrl), ScriptsPath, ScriptsUrl)
    End Sub

    Private _SingularPhysicalPath As String
    Public ReadOnly Property SingularPhysicalPath As String
      Get
        If String.IsNullOrEmpty(_SingularPhysicalPath) Then
          _SingularPhysicalPath = Utils.Server_MapPath("~/Singular")
        End If
        Return _SingularPhysicalPath
      End Get
    End Property

  End Class

  Public Class LibraryIncludes
    Private Shared Property LibIncludeBasePath = GetPath(ScriptLocation.LibraryEmbedded, "Include")

    Public Shared Property AuditTrailsScript As New ScriptFile(LibIncludeBasePath, "AuditTrails.js") With {.VersionNo = ScriptsVersion}
    Public Shared Property CheckQueriesScript As New ScriptFile(LibIncludeBasePath, "CheckQueries.js") With {.VersionNo = ScriptsVersion}
    Public Shared Property ImageResizerScript As New ScriptFile(LibIncludeBasePath, "ImageResizer.js") With {.VersionNo = ScriptsVersion}
    Public Shared Property MaintenanceScript As New ScriptFile(LibIncludeBasePath, "Maintenance.js") With {.VersionNo = ScriptsVersion}
    Public Shared Property ScheduleScript As New ScriptFile(LibIncludeBasePath, "Schedule.js") With {.VersionNo = ScriptsVersion}
    Public Shared Property SecurityScript As New ScriptFile(LibIncludeBasePath, "Security.js") With {.VersionNo = ScriptsVersion}
  End Class

  Public Class ScriptManager

    Private mScriptGroupList As New Dictionary(Of String, ScriptGroup)

    ''' <summary>
    ''' Adds a container for a collection of files.
    ''' </summary>
    ''' <param name="GroupName">The unique name of the group.</param>
    ''' <param name="Path">The URL where all of the files in this group are stored. (Use ~ for the web site root)</param>
    Public Function AddScriptGroup(GroupName As String, Path As String) As ScriptGroup

      Dim sg As New ScriptGroup With {.GroupName = GroupName, .Path = Path}
      mScriptGroupList.Add(GroupName, sg)
      Return sg

    End Function

    Public Function AddVersionedScript(GroupName As String, DebugPath As String) As ScriptGroup
      Dim Path As String = IO.Path.GetDirectoryName(DebugPath)
      Dim FileName As String = IO.Path.GetFileName(DebugPath)
      If FileName.EndsWith(".js") Then
        FileName = FileName.Substring(0, FileName.Length - 3)
      End If

      Dim sg = AddScriptGroup(GroupName, Path)
      sg.EnableCombineAndMinify(sg.AddScript(FileName & ".js"))
      Return sg
    End Function

    Public Function GetGroup(GroupName As String) As ScriptGroup
      Return mScriptGroupList(GroupName)
    End Function

    Friend Function RenderLibraryScripts() As String
      Dim sb As New Text.StringBuilder
      For Each key As String In mScriptGroupList.Keys
        If Singular.Misc.In(key, ScriptGroupType.Knockout.ToString, ScriptGroupType.JQuery.ToString, ScriptGroupType.Singular.ToString) Then
          mScriptGroupList(key).RenderScriptTags(sb)
        End If
      Next
      If Settings.SupportECMA6 Then
        mScriptGroupList("ECMA6").RenderScriptTags(sb)
      End If
      Return sb.ToString
    End Function

    Public Sub GenerateVersionedFiles()
      For Each key As String In mScriptGroupList.Keys
        mScriptGroupList(key).GenerateCombinedScript()
      Next
    End Sub

    Public Sub New()
      CreateLibraryScriptGroups()
    End Sub

    Private Sub CreateLibraryScriptGroups()

      'JQuery
      With AddScriptGroup(ScriptGroupType.JQuery.ToString, Scripts.GetPath(ScriptLocation.LibraryVirtualDir, ScriptGroupType.JQuery.ToString))
        If Scripts.Settings.LibJQueryVersion = ScriptSettings.JQueryVersion.JQ_1_7_2 Then
          .AddScript("jquery-1.7.2.js", "jquery-1.7.2.min.js", "//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js")
        ElseIf Scripts.Settings.LibJQueryVersion = ScriptSettings.JQueryVersion.JQ_1_12_4 Then
          .AddScript("jquery-1.12.4.js", "jquery-1.12.4.min.js", "//ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js")
        Else
          .AddScript("jquery-2.2.4.js", "jquery-2.2.4.min.js", "//ajax.googleapis.com/ajax/libs/jquery/2.2.4/jquery.min.js")
        End If

        If Scripts.Settings.LibJQueryUIVersion = ScriptSettings.JQueryUIVersion.JQ_UI_1_9_0 Then
          .AddScript("jquery-ui-1.9.0.custom.js", "jquery-ui-1.9.0.custom.min.js", "//ajax.googleapis.com/ajax/libs/jqueryui/1.9.0/jquery-ui.min.js")
        Else
          .AddScript("jquery-ui-1.12.1.js", "jquery-ui-1.12.1.min.js", "//ajax.googleapis.com/ajax/libs/jqueryui/1.12.1/jquery-ui.min.js")
        End If
      End With

      'Knockout
      With AddScriptGroup(ScriptGroupType.Knockout.ToString, Scripts.GetPath(ScriptLocation.LibraryVirtualDir, ScriptGroupType.Knockout.ToString))
        If Settings.UsePatchedKnockoutFiles Then
          .AddScript("knockout-2.3.0.patched.js", "knockout-2.3.0.patched.min.js")
        Else
          .AddScript("knockout-2.3.0.debug.js", "knockout-2.3.0.js", "//cdnjs.cloudflare.com/ajax/libs/knockout/2.3.0/knockout-min.js")
        End If

      End With

      'Singular
      With AddScriptGroup(ScriptGroupType.Singular.ToString, Scripts.GetPath(ScriptLocation.LibraryEmbedded, ScriptGroupType.Singular.ToString))

        Dim MinifiedName As String = "Singular"

        .AddScript("Utils.js")
        .AddScript("Singular.Core.js")
        Dim Main = .AddScript("Singular.ASPNet.js")
        .AddScript("Singular.Controls.js")
        .AddScript("Singular.Data.js")
        .AddScript("Singular.Validation2.js")
        .AddScript("KO.Bindings.js")
        .AddScript("KO.Plugins.js")

        If Controls.UsesPagedGrid Then
          MinifiedName &= "Pg"
          .AddScript("Singular.Paging.js")
        End If
        If Controls.UsesComboDropDown Then
          MinifiedName &= "Cd"
          .AddScript("SCombo.js")
        End If

        Main.ReleaseScriptName = MinifiedName & ".min.js"
        .EnableCombineAndMinify(Main)
      End With

      'Grid and Grid reporting
      With AddScriptGroup(ScriptGroupType.SGrid.ToString, Scripts.GetPath(ScriptLocation.LibraryEmbedded, "SGrid"))
        .AddScript("Singular.Drawing.js")
        .EnableCombineAndMinify(.AddScript("Singular.Grid.js", "Singular.SGrid.min.js"))
      End With

      With AddScriptGroup(ScriptGroupType.GridReport.ToString, Scripts.GetPath(ScriptLocation.LibraryEmbedded, "SGrid"))
        .AddScript("Singular.GridChart.js")
        .EnableCombineAndMinify(.AddScript("Singular.GridReport.js", "Singular.GridReport.min.js"))
      End With


      'If the project has ECMA6 support
      If Scripts.Settings.SupportECMA6 Then
        With AddScriptGroup("ECMA6", Scripts.GetPath(ScriptLocation.LibraryEmbedded, "Singular"))
          .AddScript("Singular.ecma6.js")
        End With
      End If

    End Sub

  End Class



End Namespace

