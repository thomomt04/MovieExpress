
Imports Csla
Imports Singular.Web.Data
Imports System.ComponentModel.DataAnnotations

Public Interface IViewModel

  ReadOnly Property JSSerialiser As Data.JS.JSSerialiser
  ReadOnly Property ServerObjectTracker As Data.ServerObjectTracker
  ReadOnly Property ClientDataProvider As ClientDataProvider
  ReadOnly Property ClientLocalisation As Localisation.JSKeyManager
  ReadOnly Property FileManager As FileManager
  ReadOnly Property Page As Singular.Web.PageBase
  ReadOnly Property SchemaList As Data.JS.SchemaDefinitionList
  ReadOnly Property IsStateless As Boolean

  Property IsSendingFile As Boolean
  Property DeleteMode As SaveMode

  Function SetPage(Page As Singular.Web.PageBase, CallSetup As Boolean) As Boolean
  Sub ClearPageCycleVariables()
  Sub HandleCommandInternal(Command As String, CommandArgs As CommandArgs)
  Function HandlePostBack(PostData As String) As Boolean
  Sub SetCacheability()
  Sub WriteInitialiseVariables(JSW As Singular.Web.Utilities.JavaScriptWriter)

  Function GetData(Args As GetDataArgs) As Object
  Function CheckRulesAsync(Args As AsyncRuleArgs) As Singular.Rules.ASyncRuleResult
  'Function GenerageCaptcha() As Singular.Captcha
  Function UpdateAndGetObject(ClientArgs As Object) As UpdateInfo
  Function GetAjaxResponse(CArgs As CommandArgs) As String

End Interface


<Serializable()>
Public MustInherit Class ViewModel(Of ModelType As ViewModel(Of ModelType))
  Inherits Singular.SingularBusinessBase(Of ModelType)
  Implements IViewModel
  Implements ICaptcha

  Public Sub New()
    'mModelBinder = New Knockout.KnockoutModelBinder
    'mModelBinder.BuildObjectGraph(Me)
    MarkOld()

  End Sub

  Private mIsStateless As Boolean? = Nothing
  <System.ComponentModel.Browsable(False)>
  Public ReadOnly Property IsStateless As Boolean Implements IViewModel.IsStateless
    Get
      If mIsStateless Is Nothing Then
        mIsStateless = GetType(Singular.Web.IStatelessViewModel).IsAssignableFrom(Me.GetType)
      End If
      Return mIsStateless
    End Get
  End Property

  Protected Overridable Sub Setup()

  End Sub

  Protected Friend Overridable Sub PostSetup()

  End Sub

  <System.ComponentModel.Browsable(False)>
  Public Overrides ReadOnly Property IsDirty As Boolean
    Get
      Return False
    End Get
  End Property

  <NonSerialized> Private mCurrentCommandArgs As CommandArgs

  Protected Sub HandleCommandInternal(Command As String, CommandArgs As CommandArgs) Implements IViewModel.HandleCommandInternal

    IsSendingFile = False

    mCurrentCommandArgs = CommandArgs
    HandleCommand(Command, CommandArgs)
    mCurrentCommandArgs = Nothing

    If CommandArgs.TransferModelToClient AndAlso Not IsSendingFile Then
      'At this point, the server view model has been updated.
      'In HandleCommand above, the programmer might have re-fetched data etc.
      'If so, then we don't want references to all the old objects, so clear the ServerObjectTracker.
      ' * However, if the model is not going back to the client (i.e. if we are just sending a file), then don't reset,
      '   as we don't want the next post back to refer to objects that we have lost the reference to.
      ServerObjectTracker.Reset()
    End If
  End Sub

  Protected Overridable Sub HandleCommand(Command As String, CommandArgs As CommandArgs)
    Select Case Command
      Case "SessionExpired"
        AddMessage(Singular.Web.MessageType.Information, Command, "Your session expired, please re-enter the information.")
      Case "ModelExpired"
        AddMessage(Singular.Web.MessageType.Information, Command, "This page has expired, and has been re-loaded.")
    End Select
  End Sub

  Protected Friend Overridable Function HandlePostBack(PostData As String) As Boolean Implements IViewModel.HandlePostBack

    If JSSerialiser.Deserialise(PostData) Then
      FileManager.PopulateNewObjects()
      Return True
    Else
      Return False
    End If

  End Function

  Protected Overridable ReadOnly Property SecurityRole As String
    Get
      Return ""
    End Get
  End Property

  ''' <summary>
  ''' Ensures the user has the required role returned by the SecurityRole property.
  ''' If SecurityRole returns an empty string, this does nothing.
  ''' </summary>
  Protected Sub AssertAccess()
    If Not String.IsNullOrEmpty(SecurityRole) AndAlso Not Singular.Security.HasAccess(SecurityRole) Then
      Throw New HttpException(403, "Unauthorised")
    End If
  End Sub

  Public Shared Function HTMLEncode(UnsafeString As String) As String
    Return System.Web.HttpContext.Current.Server.HtmlEncode(UnsafeString)
  End Function

  Protected Friend Overridable Function GetData(Args As GetDataArgs) As Object Implements IViewModel.GetData

    Return ClientDataProvider.FetchList(Args)

  End Function

  Friend Overridable Function UpdateAndGetObject(ClientArgs As Object) As UpdateInfo Implements IViewModel.UpdateAndGetObject

    If IsStateless Then

      Dim TypeString As String = ClientArgs.ObjectType
      Dim Obj = Activator.CreateInstance(Type.GetType(TypeString))
      Dim Serialiser As New Singular.Web.Data.JS.StatelessJSSerialiser(Obj)
      Serialiser.Deserialise(ClientArgs.Object)
      Return New UpdateInfo() With {.UpdatedObject = Obj}

    Else

      Dim Serialiser As Singular.Web.Data.JS.SessionVMJSSerialiser = JSSerialiser
      Dim ObjectToUpdate As Object
      Dim ObjectGuid As Guid = New Guid(ClientArgs.Object.Guid.ToString)
      Dim mUpdate As Data.JS.ObjectInfo.Member = Nothing

      If ClientArgs.ContainerGuid Is Nothing Then
        'Deserialising the whole viewmodel
        Serialiser.Deserialise(ClientArgs.Object)
        ObjectToUpdate = Me
        mUpdate = Serialiser.RootObjectInfo
      Else

        'Find the object being saved.

        ObjectToUpdate = ServerObjectTracker.GetObject(ObjectGuid)
        'Find the parent the object belongs to.
        Dim Parent = ServerObjectTracker.GetObject(ClientArgs.ContainerGuid.ToString)

        'Find the MemberInfo that knows about the parent.
        If Parent IsNot Nothing Then
          mUpdate = Serialiser.Find(Parent)
        End If
        If mUpdate Is Nothing Then
          'the parent is also new, so just create a standalone object.

          Return New UpdateInfo With {.UpdatedObject = Singular.Web.Data.JS.StatelessJSSerialiser.DeserialiseObject(Type.GetType(CStr(ClientArgs.ObjectType)), ClientArgs.Object), .ObjectGuid = ObjectGuid}
        End If
        mUpdate = mUpdate.MemberList.Find(Function(c) c.PropertyHelper.Name = ClientArgs.ContainerProperty.ToString)

        'Tell the MemberInfo to update the model with the object being saved.
        Dim dObjContainer As New Singular.Dynamic.DynamicStorage
        SyncLock mUpdate
          If TypeOf mUpdate Is Data.JS.ObjectInfo.ComplexListMember Then
            dObjContainer.SetMember(ClientArgs.ContainerProperty.ToString, New System.Web.Helpers.DynamicJsonArray({ClientArgs.Object}))
          Else
            dObjContainer.SetMember(ClientArgs.ContainerProperty.ToString, ClientArgs.Object)
          End If
          mUpdate.UpdateModel(dObjContainer, Parent)
        End SyncLock

      End If

      FileManager.PopulateNewObjects()

      'If the object was created on the server, then find it.
      If ObjectToUpdate Is Nothing Then
        ObjectToUpdate = ServerObjectTracker.GetObject(ObjectGuid)
      End If

      Return New UpdateInfo With {.UpdatedObject = ObjectToUpdate, .ObjectGuid = ObjectGuid, .Container = mUpdate}

    End If

  End Function

  Friend Function GetAjaxResponse(CArgs As CommandArgs) As String Implements IViewModel.GetAjaxResponse

    If CArgs.TransferModelToClient Then
      ServerObjectTracker.Reset()
    End If

    Dim jw As New Data.JSonWriter
    jw.StartClass("")
    If CArgs.TransferModelToClient Then
      JSSerialiser.RootPropertyName = "ViewModel"
      JSSerialiser.GetJSon(jw)
    End If
    jw.WriteObject("MessageList", mMessageList)
    jw.WriteObject("ReturnData", CArgs.ReturnData)
    jw.EndClass()

    Return jw.ToString

  End Function

  Protected Friend Overridable Function CheckRulesAsync(Args As AsyncRuleArgs) As Singular.Rules.ASyncRuleResult Implements IViewModel.CheckRulesAsync

    For Each Rule In Singular.Rules.RuleHelpers.GetRulesForType(Args.ObjectType)
      If Singular.Reflection.TypeImplementsInterface(Rule.GetType, GetType(Singular.Rules.IJavascriptRule)) Then

        If CType(Rule, Singular.Rules.IJavascriptRule).UniqueRuleName = Args.RuleName Then

          CType(Rule, Singular.Rules.IJavascriptRule).Suspend(True)
          Dim CheckObject = UpdateAndGetObject(Args.ClientArgs)
          If TypeOf JSSerialiser Is JS.SessionVMJSSerialiser Then
            CType(JSSerialiser, JS.SessionVMJSSerialiser).UpdateItemTracker()
          End If

          CType(Rule, Singular.Rules.IJavascriptRule).Suspend(False)



          Dim Err As String = CType(Rule, Singular.Rules.IJavascriptRule).CheckServerRule(CheckObject.UpdatedObject)

          Return New Singular.Rules.ASyncRuleResult(Err, CType(Rule, Singular.Rules.IJavascriptRule).Severity)

        End If
      End If
    Next

    'Return "Rule '" & Args.RuleName & "' not implemented."
    Return New Singular.Rules.ASyncRuleResult("Rule " & Args.RuleName & " not implemented.", Rules.RuleSeverity.Error)

  End Function

  <System.ComponentModel.Browsable(False)>
  Protected Property ShowDetailErrorMessages As Boolean = False

  Protected Overridable Overloads Function TrySave(ObjectToSave As ISavable, Optional ShowSuccessMessages As Boolean = True, Optional ShowFailedMessages As Boolean = True) As SaveHelper

    If TypeOf ObjectToSave Is IChildSavable Then
      Return TrySave(ObjectToSave, Nothing, ShowSuccessMessages, ShowFailedMessages)
    Else
      Dim sh = New Singular.Web.SaveHelper(CType(ObjectToSave, IParentSavable).TrySave, ObjectToSave)
      AddSaveMessages(sh, ShowSuccessMessages, ShowFailedMessages)
      Return sh
    End If

  End Function

  Protected Overloads Function TrySave(Of ContainerListType)(ObjectToSave As IChildSavable, Optional ShowSuccessMessages As Boolean = True, Optional ShowFailedMessages As Boolean = True) As SaveHelper

    Return TrySave(ObjectToSave, GetType(ContainerListType), ShowSuccessMessages, ShowFailedMessages)

  End Function

  Protected Overridable Overloads Function TrySave(ObjectToSave As IChildSavable, ContainerListType As Type, Optional ShowSuccessMessages As Boolean = True, Optional ShowFailedMessages As Boolean = True) As SaveHelper

    Dim sh = New Singular.Web.SaveHelper(ObjectToSave.TrySave(ContainerListType), ObjectToSave)
    AddSaveMessages(sh, ShowSuccessMessages, ShowFailedMessages)
    Return sh

  End Function

  Protected Overridable Sub AddSaveMessages(sh As SaveHelper, ShowSuccessMessages As Boolean, ShowFailedMessages As Boolean)

    If sh.Success Then
      If ShowSuccessMessages Then
        AddMessage(sh.SaveMessageType, "Save", sh.SaveMessage)
      End If
    Else
      If ShowFailedMessages Then
        AddMessage(sh.SaveMessageType, "Save", sh.SaveMessage)
      End If
    End If

  End Sub

  <System.ComponentModel.Browsable(False)>
  Public Property IsSendingFile As Boolean = False Implements IViewModel.IsSendingFile

  ''' <summary>
  ''' Sends a file to the browser for the user to view or download.
  ''' </summary>
  ''' <param name="FileName">The name of the file without a path.</param>
  ''' <param name="FileBytes">The contents of the file.</param>
  ''' <param name="Attachment">True if the user must see the 'view/save as' prompt. False if the document should display in the page. E.g. a PDF document.</param>
  Protected Sub SendFile(FileName As String, FileBytes As Byte(), Optional Attachment As Boolean = True)
    If mCurrentCommandArgs IsNot Nothing AndAlso mCurrentCommandArgs.IsAjaxPostBack Then
      Throw New Exception("Send File cannot be called in an ajax post back. Use Postback type = Full.")
    End If
    IsSendingFile = True
    Singular.Web.WebServices.FileDownloadHandler.SendFile(System.Web.HttpContext.Current.Response, FileName, FileBytes, Attachment)

  End Sub

  ''' <summary>
  ''' Sends a file to the browser for the user to view or download.
  ''' </summary>
  ''' <param name="Attachment">True if the user must see the 'view/save as' prompt. False if the document should display in the page. E.g. a PDF document.</param>
  Protected Sub SendFile(Doc As Singular.Documents.TemporaryDocument, Optional Attachment As Boolean = True)
    SendFile(Doc.DocumentName, Doc.Document.Document, Attachment)
  End Sub

  ''' <summary>
  ''' Sends a file to the browser for the user to view or download.
  ''' </summary>
  ''' <param name="Attachment">True if the user must see the 'view/save as' prompt. False if the document should display in the page. E.g. a PDF document.</param>
  Protected Sub SendFile(Doc As Singular.Documents.IDocument, Optional Attachment As Boolean = True)
    SendFile(Doc.DocumentName, Doc.Document, Attachment)
  End Sub

  Protected Function GetObjectAsJSon(Obj As Object, Optional Context As String = "") As String
    'Dim jsw As New Singular.Web.Client.JSonWriter
    'Singular.Web.Data.JS.ObjectInfo.GetObjectAsJSon(Obj, , jsw, Context, , Me)
    'Return jsw.ToString

    Dim Serialiser As New Data.JS.SessionVMJSSerialiser(Me, Obj)
    Serialiser.ContextList.AddContext(Context)
    Return Serialiser.GetJSon

  End Function

  Protected Overridable Sub SetCacheability() Implements IViewModel.SetCacheability
    'Page.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache)
    Page.Response.Cache.SetNoStore()
  End Sub

  Protected Sub Reset()
    mPage.ResetViewModel()
  End Sub

  <NonSerialized()>
  Private mPage As Singular.Web.PageBase

  <System.ComponentModel.Browsable(False)> _
  Protected Friend ReadOnly Property Page As Singular.Web.PageBase Implements IViewModel.Page
    Get
      Return mPage
    End Get
  End Property

  Protected Function SetPage(Page As Singular.Web.PageBase, CallSetup As Boolean) As Boolean Implements IViewModel.SetPage
    mPage = Page

    Dim lSecurityRole = SecurityRole
    If lSecurityRole <> "" Then
      If Not Singular.Security.HasAccess(lSecurityRole) Then
        Return False
      End If
    End If

    If CallSetup Then
      Setup()
      PostSetup() 'Maintenance page needs this, hack because its used in too many places to fix properly.
    End If

    'Dim MB = ModelBinder() 'Create the model binder if it hasn't been created yet..
    Return True
  End Function

  Private mServerObjectTracker As ServerObjectTracker
  <System.ComponentModel.Browsable(False)> _
  Public ReadOnly Property ServerObjectTracker As ServerObjectTracker Implements IViewModel.ServerObjectTracker
    Get
      If mServerObjectTracker Is Nothing Then
        mServerObjectTracker = New ServerObjectTracker
      End If
      Return mServerObjectTracker
    End Get
  End Property

  Private mFileManager As FileManager
  <System.ComponentModel.Browsable(False)> _
  Public ReadOnly Property FileManager As FileManager Implements IViewModel.FileManager
    Get
      If mFileManager Is Nothing Then
        mFileManager = New FileManager(ServerObjectTracker)
      End If
      Return mFileManager
    End Get
  End Property

  <NonSerialized()> Private mClientDataProvider As Data.ClientDataProvider
  <System.ComponentModel.Browsable(False)> _
  Public ReadOnly Property ClientDataProvider As ClientDataProvider Implements IViewModel.ClientDataProvider
    Get
      If mClientDataProvider Is Nothing Then
        mClientDataProvider = New ClientDataProvider(Me)
      End If
      Return mClientDataProvider
    End Get
  End Property

  <NonSerialized()>
  Protected mSerialiser As Data.JS.JSSerialiser
  <System.ComponentModel.Browsable(False)> _
  Public Overridable ReadOnly Property JSSerialiser As Data.JS.JSSerialiser Implements IViewModel.JSSerialiser
    Get
      If mSerialiser Is Nothing Then
        If IsStateless Then
          mSerialiser = New Singular.Web.Data.JS.StatelessJSSerialiser(Me)
        Else
          mSerialiser = New JS.SessionVMJSSerialiser(Me, Me)
        End If
        mSerialiser.ContextList.AddContext(Me.GetType.Name)
        mSerialiser.BuildObjectGraph()
      End If
      Return mSerialiser
    End Get
  End Property

  <NonSerialized> Private mJSKeyManager As Localisation.JSKeyManager
  <System.ComponentModel.Browsable(False)>
  Public ReadOnly Property ClientLocalisation As Localisation.JSKeyManager Implements IViewModel.ClientLocalisation
    Get
      If mJSKeyManager Is Nothing Then
        mJSKeyManager = New Localisation.JSKeyManager
      End If
      Return mJSKeyManager
    End Get
  End Property

  <NonSerialized()> Private mSchemaList As New Data.JS.SchemaDefinitionList
  <System.ComponentModel.Browsable(False)>
  Public ReadOnly Property SchemaList As Data.JS.SchemaDefinitionList Implements IViewModel.SchemaList
    Get
      Return mSchemaList
    End Get
  End Property

  Protected Sub ResetSerialiser()
    mSerialiser = Nothing
  End Sub

  ''' <summary>
  ''' The time in milliseconds after which messages will fade.
  ''' </summary>
  <System.ComponentModel.Browsable(False)> Public Property MessageFadeTime As Integer = 0

  Public Function AddMessage(MessageType As MessageType, Title As String, Message As String) As Singular.Web.CustomControls.MessageInfo
    Return AddMessage(MessageType, Singular.Web.CustomControls.MessageInfo.MainHolderName, Title, Message)
  End Function

  Public Function AddMessage(MessageType As MessageType, HolderName As String, Title As String, Message As String) As Singular.Web.CustomControls.MessageInfo
    Return AddMessage(MessageType, HolderName, MessageType.ToString, Title, Message)
  End Function

  Public Function AddMessage(MessageType As MessageType, HolderName As String, MessageName As String, Title As String, Message As String) As Singular.Web.CustomControls.MessageInfo
    Return mMessageList.AddMessage(HolderName, MessageName, MessageType, Title, Message, MessageFadeTime)
  End Function

  Public Function AddMessage(Message As Singular.Web.Message) As Singular.Web.CustomControls.MessageInfo
    Return AddMessage(Message.MessageType, Message.MessageTitle, Message.Message)
  End Function

  <NonSerialized> Private mMessageList As New Singular.Web.CustomControls.MessageList
  Protected ReadOnly Property MessageList As Singular.Web.CustomControls.MessageList
    Get
      Return mMessageList
    End Get
  End Property

  <NonSerialized> Private mRoleList As New List(Of String)

  ''' <summary>
  ''' Adds a list of roles the user is in according to the role group. E.g. AddRoleGroup('Entity') will add Entity.Access and Entity.Edit
  ''' </summary>
  Protected Sub AddRoleGroup(RoleGroup As String)
    For Each role In Singular.Security.CurrentIdentity.Roles
      Dim Parts = role.Split(".")
      If Parts.Count > 1 AndAlso RoleGroup = Parts(0) Then
        mRoleList.Add(role)
      End If
    Next
  End Sub

  ''' <summary>
  ''' Adds a list of roles the user is in. This can be checked on client side using Singular.HasAccess(Role)
  ''' </summary>
  Protected Sub AddRoles(ParamArray Roles() As String)
    For Each role In Roles
      If Singular.Security.HasAccess(role) Then
        mRoleList.Add(role)
      End If
    Next
  End Sub

  Friend Sub ClearPageCycleVariables() Implements IViewModel.ClearPageCycleVariables
    IsSendingFile = False
    mMessageList = New Singular.Web.CustomControls.MessageList
    ResetSerialiser()
  End Sub

  Protected Function FindObject(Guid As String) As Object
    Return Me.ServerObjectTracker.GetObject(New Guid(Guid))
  End Function

  <System.ComponentModel.Browsable(False)> Public Property DeleteMode As SaveMode = Singular.Web.SaveMode.OnSaveClick Implements IViewModel.DeleteMode
  <System.ComponentModel.Browsable(False)> Public Property SaveMode As SaveMode = Singular.Web.SaveMode.OnSaveClick
  <System.ComponentModel.Browsable(False)> Public Property ValidationDisplayMode As ValidationDisplayMode = ValidationDisplayMode.Normal
  <System.ComponentModel.Browsable(False)> Public Property ValidationMode As ValidationMode = Singular.Web.ValidationMode.OnFirstChange
  <System.ComponentModel.Browsable(False)> Public Property DirtyWarning As String = ""
  <System.ComponentModel.Browsable(False)> Public Property CheckDirtyFunctionName As String = ""
  '<System.ComponentModel.Browsable(False)> Public Property DeserialiseMode As Singular.Web.DeserialiseMode = DeserialiseMode.Normal

  Protected Friend Overridable Sub WriteInitialiseVariables(JSW As Singular.Web.Utilities.JavaScriptWriter) Implements IViewModel.WriteInitialiseVariables
    JSW.Write("Singular.Init(" & Singular.Security.HasAuthenticatedUser.ToString.ToLower & ", " &
              SaveMode & ", " & DeleteMode & ", " & ValidationDisplayMode & ", " & ValidationMode & ");")

    'If DeserialiseMode = Singular.Web.DeserialiseMode.Stateless Then
    '  JSW.Write("Singular.DeserialiseMode = 2;")
    'End If
    If DirtyWarning <> "" Then
      JSW.Write("window.DirtyWarning = " & DirtyWarning.AddSingleQuotes & ";")
    End If
    If CheckDirtyFunctionName <> "" Then
      JSW.Write("Singular.CheckVMDirty = " & CheckDirtyFunctionName & ";")
    End If
    If Controls.DefaultButtonStyle = ButtonStyle.Bootstrap Then
      JSW.Write("Singular.BSButtons = true;")
    End If
    If Singular.Web.WebServices.FileUploadHandler.MaxRequestSizeKB > 0 Then
      JSW.Write("Singular.MaxRequestSizeKB = " & Singular.Web.WebServices.FileUploadHandler.MaxRequestSizeKB & ";")
    End If

    If mMessageList.Count > 0 Then
      JSW.Write("Singular.CreateMessages(" & Data.JSonWriter.SerialiseObject(mMessageList, , False, OutputType.Javascript) & ");")
    End If

    If mRoleList.Count > 0 Then
      JSW.Write("Singular.ClientRoles = " & Data.JSonWriter.SerialiseObject(mRoleList, , False, OutputType.Javascript) & ";")
    End If


  End Sub

#Region " Captcha "

  'Singular.DataAnnotations.RequiredIf(ErrorMessageResourceName:="EMCapthaRequired", Condition:=DataAnnotations.RequiredCondition.Visible)>
  Public Shared CaptchaProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.CaptchaText)
  <Singular.DataAnnotations.BrowsableConditional("HasCaptchaControl")>
  Public Property CaptchaText As String = "" Implements ICaptcha.CaptchaText

  <Singular.DataAnnotations.BrowsableConditional("HasCaptchaControl")>
  Public Property CaptchaSecret As String = "" Implements ICaptcha.CaptchaSecret

  Protected Function IsCaptchaValid() As Boolean

    IsCaptchaValid = Singular.Captcha.IsTextCorrect(Me)
    CaptchaText = ""

  End Function

#End Region

End Class


