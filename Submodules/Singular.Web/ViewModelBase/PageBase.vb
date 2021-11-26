Imports System.Web.UI
Imports System.Web.Caching
Imports System.Reflection
Imports System.Web.UI.WebControls
Imports System.Linq.Expressions

Public Interface IPageBase
  Inherits Data.IViewModelPage

  ReadOnly Property LateResources As LateResourceHelper

End Interface

Public MustInherit Class PageBase
  Inherits Page
  Implements IPageBase

  <Serializable()>
  Friend Class PageInfo
    Public Property PageTypeName As String
    Public Property PageCreatedDate As Date = Now
    Public Property PageGuid As Guid
  End Class

  Protected mMaintainState As Boolean = True

  Public ReadOnly Property MaintainsState As Boolean
    Get
      Return mMaintainState
    End Get
  End Property

#Region " View State "

  Protected Overrides Sub SavePageStateToPersistenceMedium(viewState As Object)

    If EnableViewState Then
      If ApplicationSettings.Settings.StoreViewStateInSession Then
        Dim VSStr As String = "VS_" & Session.SessionID & "_" & DateTime.Now.Ticks.ToString()
        Cache.Add(VSStr, viewState, Nothing, DateTime.Now.AddMinutes(Session.Timeout), TimeSpan.Zero, CacheItemPriority.Default, Nothing)
        ScriptManager.RegisterHiddenField(Me, "ViewStateKey", VSStr)
      Else
        MyBase.SavePageStateToPersistenceMedium(viewState)
      End If
    End If

  End Sub

  Protected Overrides Function LoadPageStateFromPersistenceMedium() As Object

    If EnableViewState Then
      If ApplicationSettings.Settings.StoreViewStateInSession Then
        Dim VSStr As String = Request.Form("ViewStateKey")
        If VSStr Is Nothing Then
          Return Nothing
        End If
        If Not VSStr.StartsWith("VS_") Then
          Throw New Exception("Invalid viewstate key:" + VSStr)
        End If
        Return Cache(VSStr)
      Else
        Return MyBase.LoadPageStateFromPersistenceMedium
      End If
    Else
      Return Nothing
    End If

  End Function

#End Region

#Region " Local Session "

  Protected mPageGuid As Guid
  Public ReadOnly Property PageGuid As Guid
    Get
      Return mPageGuid
    End Get
  End Property

  Protected Function GetPageGuid() As Guid

    Dim Queue As New Hashtable

    'Get all the pages of this type in storage.
    For Each key As String In Session.Keys
      If key.StartsWith("PageState.PageInfo") Then
        Dim pi As PageInfo = Session(key)
        If pi.PageTypeName = Me.GetType.Name & PageInstanceKey Then
          'Get the page guid.
          Queue.Add(pi.PageCreatedDate, pi.PageGuid)
        End If
      End If
    Next

    If Queue.Keys.Count >= NoOfInstancedAllowed Then
      'remove all session key that end with this page guid.
      Dim PageGuid As String = Queue(Queue.Keys(0)).ToString
      For i As Integer = Session.Count - 1 To 0 Step -1
        If Session.Keys(i).EndsWith(PageGuid) Then
          Session.RemoveAt(i)
        End If
      Next
    End If

    Return Guid.NewGuid

  End Function

  Protected Overridable ReadOnly Property NoOfInstancedAllowed As Integer
    Get
      Return 1
    End Get
  End Property

  Protected Overridable ReadOnly Property PageInstanceKey As String
    Get
      Return ""
    End Get
  End Property

  Private mLocalState As New Hashtable
  Protected Property PageSession(Key As String) As Object
    Get
      If mMaintainState Then
        Return System.Web.HttpContext.Current.Session("PageState." & Key & "." & mPageGuid.ToString)
      Else
        Return mLocalState(Key)
      End If
    End Get
    Set(value As Object)
      If mMaintainState Then
        System.Web.HttpContext.Current.Session("PageState." & Key & "." & mPageGuid.ToString) = value
      Else
        mLocalState(Key) = value
      End If
    End Set
  End Property

  Public Shared Function GetModel(PageGuid As String, Context As HttpContext) As IViewModel
    Return Context.Session("PageState.ViewModel." & PageGuid)
  End Function

#End Region

#Region " Localisation "

  Protected Overrides Sub InitializeCulture()
    MyBase.InitializeCulture()

    'Change Language
    If Request.QueryString("SCmd") = "ChangeLanguage" AndAlso Request.QueryString("Language") IsNot Nothing Then
      CustomControls.LanguageSelector.SetCulture(Singular.Localisation.CreateLanguageCulture(Request.QueryString("Language")))
    End If

    Singular.Localisation.SetupRequest()
  End Sub

  ''' <summary>
  ''' Gets the localised text for the given key. Returns the text in the default language if the localised version is missing.
  ''' </summary>
  Public Shared Function LocalText(ResourceName As String) As String
    Return Singular.Localisation.LocalText(ResourceName)
  End Function

  ''' <summary>
  ''' Gets the localised text for the given key and parameters. Returns the text in the default language if the localised version is missing.
  ''' Requires that the localised text has the parameters in {0}, {1} format.
  ''' </summary>
  Public Shared Function LocalText(ResourceName As String, ParamArray Params() As Object) As String
    If Params.Length = 0 Then
      Return Singular.Localisation.LocalText(ResourceName)
    Else
      Return Singular.Localisation.LocalText(ResourceName, Params)
    End If

  End Function

#End Region

  Public Class ProcessTimer
    Private TotalTicks As Long

    Private stopwatch As New Stopwatch


    Private mSW As New IO.StringWriter

    Public Sub New()
      stopwatch.Start()
    End Sub

    Public Sub AddTime(Name As String)

      TotalTicks += stopwatch.ElapsedTicks

      mSW.Write(Singular.Strings.PadRight(Name, " ", 30) &
                (stopwatch.ElapsedTicks / 1000).ToString("00000") & " - " &
                (TotalTicks / 1000).ToString("00000") & vbCrLf)
      stopwatch.Reset()
      stopwatch.Start()
    End Sub

    Public Overrides Function ToString() As String
      Return mSW.ToString
    End Function
  End Class

  Protected mTimer As New ProcessTimer
  Public ReadOnly Property Timer As ProcessTimer
    Get
      Return mTimer
    End Get
  End Property

  Public Overloads ReadOnly Property Master As MasterBase
    Get
      Return MyBase.Master

    End Get
  End Property

  Public ReadOnly Property ModelNonGeneric As IViewModel Implements Data.IViewModelPage.ModelNonGeneric
    Get
      Return PageSession("ViewModel")
    End Get
  End Property

  Friend Sub ResetViewModel()
    If ModelNonGeneric Is Nothing Then
      Throw New Exception("View Model has not been initialised yet.")
    Else
      Dim t As Type = ModelNonGeneric.GetType
      PageSession("ViewModel") = Activator.CreateInstance(t)
      If Not ModelNonGeneric.SetPage(Me, True) Then
        HandleNoAccess()
      End If
    End If
  End Sub

  Protected Overridable Sub HandleNoAccess()
    Throw New UnauthorizedAccessException("User does not have sufficient access to this page.")
  End Sub

  Private _LateResources As New LateResourceHelper

	Public ReadOnly Property LateResources As LateResourceHelper Implements IPageBase.LateResources
		Get
			Return _LateResources
		End Get
	End Property

End Class

Public MustInherit Class PageBase(Of VM As IViewModel)
	Inherits PageBase(Of VM, Controls.HelperControls.HelperAccessors(Of VM))

End Class

Public MustInherit Class PageBase(Of VM As IViewModel, HelpersType As Controls.HelperControls.HelperAccessors(Of VM))
	Inherits PageBase

#Region " View Model "

	'Shouldn't have called this model... That's why the ViewModel property below is the same as this.
	<Obsolete("Please use the ViewModel property instead")>
	Public ReadOnly Property Model As VM
		Get
			Return PageSession("ViewModel")
		End Get
	End Property

	Public ReadOnly Property ViewModel As VM
		Get
			Return PageSession("ViewModel")
		End Get
	End Property

#End Region

	Public Sub New()
		mMaintainState = Not GetType(Singular.Web.IStatelessViewModel).IsAssignableFrom(GetType(VM))
		EnableViewState = mMaintainState
	End Sub

	Private mInitialLoad As Boolean
	Private mExpiredSession As Boolean
  Public ReadOnly Property ExpiredSession As Boolean
    Get
      Return mExpiredSession
    End Get
  End Property


  Protected Overrides Sub OnPreInit(e As System.EventArgs)
		MyBase.OnPreInit(e)

		Me.Title = GetPageTitle()

		'Logout
		If Request.QueryString("SCmd") = "Logout" AndAlso Singular.Security.CurrentIdentity IsNot Nothing Then
      Singular.Web.Security.Logout()
      Dim logoutUrl As String = ""
      If (Singular.Web.Security.GetLogoutURLHandler IsNot Nothing) Then logoutUrl = Singular.Web.Security.GetLogoutURLHandler.Invoke()
      If (String.IsNullOrEmpty(logoutUrl)) Then logoutUrl = Request.CurrentExecutionFilePath
      Response.Redirect(logoutUrl, True)
    End If

      'Check the site map for this pages name
      'If the page is in the site map, check that if it has any roles, the user also has the roles.
      If Not HasAccess() Then
			If Singular.Security.HasAuthenticatedUser Then
				HandleNoAccess()
			Else
				System.Web.Security.FormsAuthentication.RedirectToLoginPage()
			End If
		End If

	End Sub

	Protected Overridable Function HasAccess() As Boolean
		Return Singular.Web.CustomControls.SiteMapDataSource.HasAccess(Me.AppRelativeVirtualPath)
	End Function

	Protected Overrides Sub OnInit(e As System.EventArgs)
		MyBase.OnInit(e)

		If IsPostBack Then
			If Request.Form("hPageGuid") IsNot Nothing Then
				Try
					mPageGuid = New Guid(Request.Form("hPageGuid"))
				Catch ex As Exception
					Throw New Exception("invalid GUID: " & Request.Form("hPageGuid"))
				End Try

			End If
		End If

		'Anti cross site request forgery
		If Request.HttpMethod.ToLower = "post" Then
			Singular.Web.Security.ValidateCSRFToken(Page.Request)
		End If

		'Specifies that this is the first time the page is being loaded
		'   either because it is the first time the user has loaded the page
		'   or because the session has expired, and the user has clicked a button.
		Dim NoSessionInfo As Boolean = PageSession("PageInfo") Is Nothing AndAlso MaintainsState
		mInitialLoad = Not IsPostBack OrElse NoSessionInfo
		mExpiredSession = IsPostBack AndAlso NoSessionInfo

		If mInitialLoad Then

			If mMaintainState Then
				'Clear the page session.
				mPageGuid = GetPageGuid()
				PageSession("PageInfo") = New PageInfo With {.PageTypeName = Me.GetType.Name & PageInstanceKey, .PageGuid = mPageGuid}
			End If

			PreSetup()
		End If

		'Create the ViewModel
		Dim HasAccess As Boolean
		If PageSession("ViewModel") Is Nothing Then
			PageSession("ViewModel") = Activator.CreateInstance(GetType(VM))
			HasAccess = ViewModel.SetPage(Me, True)
		Else
			HasAccess = ViewModel.SetPage(Me, False)
		End If

		If Not HasAccess Then
			HandleNoAccess()
		Else

			mTimer.AddTime("Model Created")

			'Call the page setup.
			If mInitialLoad Then
				Setup()
				'Check if there is a command query string
				If Request.QueryString("Command") IsNot Nothing Then

					Dim Arg As String = ""
					If Request.QueryString("Arg") IsNot Nothing Then
						Arg = Request.QueryString("Arg")
					End If
					Dim args As New CommandArgs(Arg, False, Not mExpiredSession, True, ViewModel)
					ViewModel.HandleCommandInternal(Request.QueryString("Command"), args)
				End If

			Else
				'Clear page cycle view model variables
				Me.ModelNonGeneric.ClearPageCycleVariables()

			End If

			ModelNonGeneric.SetCacheability()

		End If

	End Sub

	Protected Overrides Sub OnLoadComplete(e As System.EventArgs)
		MyBase.OnLoadComplete(e)

		'Handle page postbacks
		If mExpiredSession Then
			'Tell the view model that the user tried to do something, but the session has expired.
			HandleFullCommand("SessionExpired", GetPostBackTarget.First)
		ElseIf Not mInitialLoad OrElse (Request.HttpMethod.ToLower = "post" AndAlso Request.Form("hModelData") IsNot Nothing) Then
			HandlePostBack()
		End If

	End Sub

	Private Sub HandlePostBack()

		Dim ModelExpired As Boolean = False

		'Get the Client Side Model Data, and update the Server Side View Model.
		If Request.Form("hModelData") IsNot Nothing AndAlso Request.Form("hModelData") <> "" Then

			ModelExpired = Not ViewModel.HandlePostBack(Request.Form("hModelData"))

		End If

		Dim p As Pair = GetPostBackTarget()

		If ModelExpired Then
			HandleFullCommand("ModelExpired", p.First)
		Else
			'If p.First <> "System.Command" Then
			HandleFullCommand(p.First, p.Second)
			'End If
		End If

	End Sub

	Private Sub HandleFullCommand(Command As String, Argument As String)

		Dim args As New CommandArgs(Argument, False, Not mExpiredSession, True, ViewModel)
		If ViewModel IsNot Nothing Then
			ViewModel.HandleCommandInternal(Command, args)
		End If

	End Sub

	''' <summary>
	''' Gets the html control that caused the post back.
	''' </summary>
	Private Function GetPostBackTarget() As Pair

		If Request.Form.GetValues("submit") IsNot Nothing AndAlso Request.Form.GetValues("submit").Count > 0 Then
			Return New Pair("Submit", Nothing)
		End If

		Dim CtlEvent As String = Request.Form("__EVENTTARGET")
		If CtlEvent IsNot Nothing AndAlso CtlEvent <> String.Empty Then
			Return New Pair(CtlEvent, Request.Form("__EVENTARGUMENT"))
		ElseIf Request.Form.Keys.Count > 0 Then
			Dim LastKey As String = Request.Form.Keys(Request.Form.Keys.Count - 1)
			Dim ctl As Control = Page.FindControl(LastKey)
			If ctl IsNot Nothing AndAlso TypeOf ctl Is Button Then
				Return New Pair(ctl.ID, Nothing)
			Else
				Return New Pair(LastKey, Nothing)
			End If
		End If
		Return New Pair("", "")
	End Function

	Public Overloads Function FindControl(ControlType As Type) As Control
		Return Singular.Web.Controls.FindControl(Me, ControlType)
	End Function

	''' <summary>
	''' Gets called before the ViewModel is created.
	''' </summary>
	Protected Overridable Sub PreSetup()

	End Sub

	''' <summary>
	''' Gets called after the View Model is created, and only on an initial page load (not post backs)
	''' </summary>
	Protected Overridable Sub Setup()

	End Sub

	Protected Overridable Function GetPageTitle() As String
		Dim Title As String = ""
		Try
			Title = Me.Title
		Catch ex As Exception

		End Try
		If Title = "" Then
			Dim NewTitle As String = Me.GetType.Name
			If NewTitle.ToLower = "default" Then
				Return "Home"
			Else
				If Me.GetType.BaseType IsNot Nothing Then
					Return Singular.Strings.Readable(Me.GetType.BaseType.Name)
				Else
					Return ""
				End If
			End If
		Else
			Return Title
		End If
	End Function

	Private mHelpers As New Singular.Web.Controls.HelperControls.HelperAccessors(Of VM)(Me, Nothing)
	Protected Overridable ReadOnly Property Helpers As HelpersType
		Get
			Return mHelpers
		End Get
	End Property

#Region " Cached Helpers "

	Private Shared GeneratedHTML As New Hashtable

	Protected Sub CreateUI(Key As String, CreateHelpers As Action(Of CachedHelper(Of VM)))

		Key = Me.GetType.FullName & "__" & Key & "__" & Singular.Localisation.CurrentCulture.TwoLetterISOLanguageName

		Dim ch As CachedHelper(Of VM) = Nothing

		SyncLock GeneratedHTML
			ch = GeneratedHTML(Key)
			If ch Is Nothing Then
				ch = New CachedHelper(Of VM)(Me, CreateHelpers)
				GeneratedHTML(Key) = ch
			End If
		End SyncLock

		Response.Write(ch.OutputHTML)

		ch.SetupPage(Me)

	End Sub

	Protected Sub CreateUI(CreateHelpers As Action(Of CachedHelper(Of VM)))

		CreateUI("Default", CreateHelpers)

	End Sub

	Protected Sub CreateTemplate(Of ChildType)(TemplateName As String, HelperCallBack As Action(Of Singular.Web.CustomControls.Template(Of VM, ChildType)))

		CreateUI("Template_" & TemplateName,
						 Sub(ch)
							 HelperCallBack(ch.Helpers.Template(Of ChildType)(TemplateName))
						 End Sub)

	End Sub

	Protected Sub CreateTemplate(TemplateName As String, HelperCallBack As Action(Of Singular.Web.CustomControls.Template(Of VM, VM)))
		CreateTemplate(Of VM)(TemplateName, HelperCallBack)
	End Sub

#End Region

End Class

Public Interface ICachedHelper
	ReadOnly Property ContextList As Singular.UIContextList
	Sub AddDropDownDataSource(DropDown As Singular.DataAnnotations.DropDownWeb)
	Sub AddSchemaType(Type As Type)
End Interface

Public Class CachedHelper(Of RootType)
	Implements ICachedHelper

  Private mPage As Data.IViewModelPage
  Private mOutputHTML As String
	Private mContextList As New Singular.UIContextList
	Private mRequestedDatasources As New Hashtable
	Private mSchemasToAdd As New Hashtable
	Private mHelpers As Singular.Web.Controls.HelperControls.HelperAccessors(Of RootType)

  Public Sub New(Page As Data.IViewModelPage, Callback As Action(Of CachedHelper(Of RootType)))
    mPage = Page
    mHelpers = New Singular.Web.Controls.HelperControls.HelperAccessors(Of RootType)(mPage, Nothing)
    mHelpers.SetCached(Me)
    Callback(Me)

    Dim writer As New System.IO.StringWriter
    mHelpers.RenderToPage(writer)
    mOutputHTML = writer.ToString
    mHelpers = Nothing
  End Sub

  Public Sub New(Page As Data.IViewModelPage)
    mPage = Page
  End Sub

  Public Sub SetupPage(Page As Data.IViewModelPage)
		'Add context keys
		For Each ContextKey As String In mContextList.Keys
			Page.ModelNonGeneric.JSSerialiser.ContextList.AddContext(ContextKey)
		Next

		'Add datasources add by drop downs.
		For Each DSKey As String In mRequestedDatasources.Keys
			Page.ModelNonGeneric.ClientDataProvider.AddDropDownDataSource(mRequestedDatasources(DSKey))
		Next

    'Add schema types
    For Each DSKey In mSchemasToAdd.Keys
      Page.ModelNonGeneric.SchemaList.AddType(DSKey)
    Next
  End Sub

	Public ReadOnly Property Helpers As Singular.Web.Controls.HelperControls.HelperAccessors(Of RootType)
		Get
			Return mHelpers
		End Get
	End Property

	Public ReadOnly Property OutputHTML As String
		Get
			Return mOutputHTML
		End Get
	End Property

	Friend ReadOnly Property ContextList As Singular.UIContextList Implements ICachedHelper.ContextList
		Get
			Return mContextList
		End Get
	End Property

	Friend Sub AddDropDownDataSource(DropDown As Singular.DataAnnotations.DropDownWeb) Implements ICachedHelper.AddDropDownDataSource
		mRequestedDatasources(DropDown.Name) = DropDown
	End Sub

	Friend Sub AddSchemaType(Type As Type) Implements ICachedHelper.AddSchemaType
		mSchemasToAdd(Type) = True
	End Sub

End Class

Public Class LateResourceHelper

  Private Property LateResources As New System.Collections.Generic.Dictionary(Of String, Boolean)
  Public Sub Add(IncludeString As String)
    If Not LateResources.ContainsKey(IncludeString) Then
      LateResources.Add(IncludeString, True)
    End If

  End Sub

  Public Sub AddLateScript(Source As String)
    Add("<script type=""text/javascript"" src=""" & Utils.URL_ToAbsolute(Source) & """></script>")
  End Sub

  Public Sub WriteLateResources(writer As HtmlTextWriter)
    For Each lr As String In LateResources.Keys
      writer.Write(lr)
    Next
  End Sub

  Protected mEndPageHTML As New Singular.Web.Controls.HtmlTextWriter
  Friend ReadOnly Property EndPageHTML As Singular.Web.Controls.HtmlTextWriter
    Get
      Return mEndPageHTML
    End Get
  End Property

  Friend mCriteriaClassList As New Hashtable

  Friend Function CanAddFindDialog(CriteriaName As String) As Boolean
    If mCriteriaClassList.ContainsKey(CriteriaName) Then
      Return False
    Else
      mCriteriaClassList(CriteriaName) = True
      Return True
    End If
  End Function

End Class