Imports System.Text
Imports System.Web.UI
Imports System.Reflection
Imports System.Linq.Expressions
Imports Singular.Web.CustomControls

Namespace Controls

  Partial Public Class HelperControls

    Public Class HelperAccessors

      Private mPage As PageBase
      Private mIPage As Data.IViewModelPage

      Protected mParentControl As HelperBase
      Protected mControlsToRender As New List(Of HelperBase)
      Private mCachedHelper As ICachedHelper

      Public Sub New(Page As Data.IViewModelPage, ParentControl As HelperBase)
        mIPage = Page
        If TypeOf Page Is PageBase Then
          mPage = Page
        End If
        mParentControl = ParentControl

        If mParentControl IsNot Nothing AndAlso mParentControl.HelperAccessorsParent IsNot Nothing Then
          mCachedHelper = mParentControl.HelperAccessorsParent.mCachedHelper
          mContextList = mParentControl.HelperAccessorsParent.mContextList

          If mIPage Is Nothing Then
            mIPage = mParentControl.HelperAccessorsParent.mIPage
          End If
        End If

      End Sub

      Friend Sub SetCached(CachedHelper As ICachedHelper)
        mCachedHelper = CachedHelper
      End Sub

      Friend Sub SetPage(Page As PageBase)
        mPage = Page
        mIPage = Page
      End Sub

      Protected Friend Overridable ReadOnly Property Page As PageBase
        Get
          Return mPage
        End Get
      End Property

      Protected Friend Overridable ReadOnly Property IPage As IPageBase
        Get
          Return mIPage
        End Get
      End Property

#Region " Settings "

      Private mControlSettings As New List(Of ControlSettings)
      Private Function CreateSettings(Type As ControlSettingType, ExactType As Type) As ControlSettings
        For Each cs As ControlSettings In mControlSettings
          If cs.TargetType = Type Then
            If Type = ControlSettingType.Exact Then
              If cs.TargetTypeExact Is ExactType Then
                Return cs
              End If
            Else
              Return cs
            End If
          End If
        Next
        Dim csNew As New ControlSettings()
        csNew.TargetType = Type
        csNew.TargetTypeExact = ExactType
        mControlSettings.Add(csNew)
        Return csNew
      End Function

      Friend Sub PopulateSettings(Control As HelperBase)

        If mParentControl IsNot Nothing AndAlso mParentControl.HelperAccessorsParent IsNot Nothing Then
          mParentControl.HelperAccessorsParent.PopulateSettings(Control)
        End If

        For Each s As ControlSettings In mControlSettings

          Dim isMatch As Boolean = False
          If s.TargetType = ControlSettingType.All Then
            isMatch = True
          ElseIf s.TargetType = ControlSettingType.Exact AndAlso s.TargetTypeExact Is Control.GetType Then
            isMatch = True
          ElseIf s.TargetType = Control.ControlSettingType Then
            isMatch = True
          End If

          If isMatch Then
            s.PopulateSettings(Control.Settings)
          End If

        Next

      End Sub

      Public Function CreateSettings(Type As ControlSettingType) As ControlSettings
        If Type = ControlSettingType.Exact Then
          Throw New ArgumentException("Pass through only the exact type when using ControlSettingType.Exact")
        Else
          Return CreateSettings(Type, Nothing)
        End If
      End Function

      Public Function CreateSettings(Type As Type) As ControlSettings
        Return CreateSettings(ControlSettingType.Exact, Type)
      End Function

#End Region

      Protected mContextList As Singular.UIContextList
      Public ReadOnly Property ContextList As Singular.UIContextList
        Get
          If mCachedHelper IsNot Nothing Then
            Return mCachedHelper.ContextList
          Else
            Return mIPage.ModelNonGeneric.JSSerialiser.ContextList
          End If
        End Get
      End Property

      Public Sub AddSchemaType(Type As Type)
        If mCachedHelper IsNot Nothing Then
          mCachedHelper.AddSchemaType(Type)
        Else
          mIPage.ModelNonGeneric.SchemaList.AddType(Type)
        End If
      End Sub

      Public Sub AddDropDownDataSource(DropDown As Singular.DataAnnotations.DropDownWeb)
        If mCachedHelper IsNot Nothing Then
          mCachedHelper.AddDropDownDataSource(DropDown)
        ElseIf mIPage IsNot Nothing Then
          mIPage.ModelNonGeneric.ClientDataProvider.AddDropDownDataSource(DropDown)
        End If

      End Sub

      Protected Friend Function SetupControl(Control As HelperBase) As HelperBase

        'Dim Handler = System.Web.HttpContext.Current.CurrentHandler
        'If TypeOf Handler Is PageBase Then
        '  CType(Handler, PageBase).Timer.AddTime("Control Pre Setup " & Control.GetType.Name)
        'End If

        Control.Page = Page
        'Control.HelperAccessorsParent = Me
        If mParentControl IsNot Nothing Then
          mParentControl.Controls.Add(Control)
        End If
        Control.SetupInternal(Me)
        mControlsToRender.Add(Control)

        'If TypeOf Handler Is PageBase Then
        '  CType(Handler, PageBase).Timer.AddTime("Control Setup " & Control.GetType.Name)
        'End If

        Return Control
      End Function

      Public Function HTML(HtmlString As String) As HTMLSnippet
        Dim hs As New HTMLSnippet
        hs.Content = HtmlString
        Return SetupControl(hs)
      End Function

      Public Function HTML() As HTMLSnippet
        Dim hs As New HTMLSnippet
        Return SetupControl(hs)
      End Function

      ''' <summary>
      ''' Adds a control 
      ''' </summary>
      ''' <param name="WebControl"></param>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public Function Control(WebControl As Control) As Control

        If WebControl Is GetType(HTMLSnippet) OrElse WebControl.Parent Is Nothing Then
          'If the control is not part of a control collection, then just add it to the controls to render.
          Return SetupControl(WebControl)

        ElseIf TypeOf WebControl Is HelperBase Then
          CType(WebControl, HelperBase).HelperAccessorsParent = Me
          If Not CType(WebControl, HelperBase).IsSetup Then
            CType(WebControl, HelperBase).Setup()
          End If
          Me.mParentControl.Controls.Add(WebControl)
          Return WebControl
        Else

          Dim s As New HTMLSnippet
          s.AddControl(WebControl)
          Return SetupControl(s)
        End If
      End Function

    End Class

    Public Class HelperAccessors(Of ObjectType)
      Inherits HelperAccessors
      Implements IDisposable

#Region "IDisposable Support"

      'Dispose is called at the end of the Using block on our page.
      'This is where all the controls added to the base helpers are rendered.
      Protected Overridable Sub Dispose(disposing As Boolean)

        RenderToPage(If(_Writer Is Nothing, Page.Response.Output, _Writer))

      End Sub

      'Public Sub ASyncRender(UpdateHelper As HTMLSnippet)
      '  For Each ctl As HelperBase In mControlsToRender
      '    If Not ctl.HasRendered Then
      '      UpdateHelper.AddControl(ctl)
      '    End If
      '  Next
      'End Sub

      Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
      End Sub
#End Region

      Public Sub New(Page As Data.IViewModelPage, ParentControl As Singular.Web.Controls.HelperControls.HelperBase)
        MyBase.New(Page, ParentControl)
      End Sub

      Private _Writer As IO.TextWriter

      Public Sub New(Writer As IO.TextWriter)
        MyBase.New(Nothing, Nothing)
        _Writer = Writer
      End Sub

      Friend Sub RenderToPage(Writer As IO.TextWriter)

        'If Page IsNot Nothing OrElse _Writer IsNot Nothing Then

        'If the First Control is a Toolbar, then add all the other controls after it into a containing div.
        'This div will be hidden until the UI has been created after all the data for the model has been fetched.
        'If mControlsToRender.Count > 0 AndAlso TypeOf mControlsToRender(0) Is Toolbar(Of ObjectType) AndAlso
        '    Not CType(mControlsToRender(0), HelperBase).HasRendered Then

        '  'Toolbar outside ui container
        '  Writer.Write(mControlsToRender(0).GetHTMLString)
        '  'UI container
        '  Writer.Write("<div class=""UIContainer"">")
        '  For i As Integer = 1 To mControlsToRender.Count - 1
        '    Writer.Write(mControlsToRender(i).GetHTMLString)
        '  Next
        '  Writer.Write("</div>")

        'Else
        'Write the controls HTML to the page.
        For Each ctl As HelperBase In mControlsToRender
          If Not ctl.HasRendered Then
            Writer.Write(ctl.GetHTMLString)
          End If
        Next
        'End If

        'End If

      End Sub

      Protected Friend Overloads Function SetupControl(Control As HelperBase(Of ObjectType), le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As HelperBase
        Control.For(le)
        MyBase.SetupControl(Control)
        Return Control
      End Function

      Protected Friend Overloads Function SetupControl(Control As HelperBase(Of ObjectType), pi As PropertyInfo) As HelperBase
        Control.For(pi)
        MyBase.SetupControl(Control)
        Return Control
      End Function

      Public Overloads Function Control(WebControl As Control, le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As Control
        Return SetupControl(WebControl, le)
      End Function

      ''' <summary>
      ''' Loads this control asyncronously, and only if the html content has changed.
      ''' The control will not have any state.
      ''' </summary>
      Public Overloads Function Control(ControlType As Type, Optional Container As Singular.Web.ContainerType = ContainerType.Div, Optional ID As String = "", Optional ExtraKeyInfo As Func(Of String) = Nothing) As AjaxControlLoader
        Return SetupControl(New Singular.Web.CustomControls.AjaxControlLoader(ControlType, Container, ID, Me, ExtraKeyInfo))
      End Function

      ''' <summary>
      ''' Loads this control asyncronously, and only if the html content has changed.
      ''' The control will not have any state.
      ''' </summary>
      Public Overloads Function Control(Of ControlType)(Optional Container As Singular.Web.ContainerType = ContainerType.Div, Optional ID As String = "", Optional ExtraKeyInfo As Func(Of String) = Nothing) As AjaxControlLoader
        Return Control(GetType(ControlType), Container, ID, ExtraKeyInfo)
      End Function

      Public Function HTMLTag(TagName As String, Optional SelfClosing As Boolean = False) As HTMLTag(Of ObjectType)
        Return SetupControl(New HTMLTag(Of ObjectType) With {.Tag = TagName, .SelfClosing = SelfClosing})
      End Function

      Public Function HTMLTag(TagName As String, InnerHTML As String) As HTMLTag(Of ObjectType)
        Dim Tag As HTMLTag(Of ObjectType) = SetupControl(New HTMLTag(Of ObjectType) With {.Tag = TagName, .SelfClosing = False})
        Tag.HTML = InnerHTML
        Return Tag
      End Function

      ''' <summary>
      ''' Creates a find button, that when clicked, opens a find dialog with criteria based on the Lists Criteria object.
      ''' When the user selects a record, the page is posted back with the selected id.
      ''' </summary>
      ''' <typeparam name="ListType">The list type of the records to find.</typeparam>
      ''' <param name="ButtonText">The text to show on the find button.</param>
      ''' <param name="DialogCaption">The text to show on the title of the find dialog.</param>
      ''' <param name="AsyncFetch">True if only the model is re-fetched using ajax. False if the whole page is posted back.</param>
      ''' <param name="ListTypeOverride">If the listType is an object, then use this to specify the actual type.</param>
      ''' <param name="AutoPopulate">If true, the list will be fetched using the default criteria the first time the user clicks find.</param>
      ''' <param name="MultiSelect">True if multiselect.</param>
      ''' <param name="BeforeOpenJSFunction">Called before the find screen is opened.</param>
      ''' <param name="OnRowSelectJSFunction">Called when a row is selected in the results grid.</param>
      ''' <param name="PreSearchJSFunction">Called when the user clicks the search button after entering criteria</param>
      Public Function FindScreen(Of ListType)(ButtonText As String, DialogCaption As String,
                                              Optional AsyncFetch As Boolean = False, Optional ListTypeOverride As Type = Nothing,
                                              Optional AutoPopulate As Boolean = False, Optional MultiSelect As Boolean = False,
                                              Optional BeforeOpenJSFunction As String = "",
                                              Optional OnRowSelectJSFunction As String = "",
                                              Optional PreSearchJSFunction As String = "") As FindScreen(Of ListType, ObjectType)
        Dim fs As New FindScreen(Of ListType, ObjectType)
        fs.ButtonText = ButtonText
        fs.IsAsync = AsyncFetch
        fs.ListTypeOverride = ListTypeOverride
        fs.AutoPopulate = AutoPopulate
        fs.MultiSelect = MultiSelect
        fs.PreFindJSFunction = BeforeOpenJSFunction
        fs.OnRowSelectJSFunction = OnRowSelectJSFunction
        fs.PreSearchJSFunction = PreSearchJSFunction
        SetupControl(fs)
        fs.Dialog.DialogCaption = DialogCaption
        Return fs
      End Function

      ''' <summary>
      ''' Adds a search dialog for a criteria object. Add this if you want to trigger a find screen from javascript
      ''' </summary>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public Function SearchDialog(Of CriteriaType)(Caption As String) As SearchDialog(Of CriteriaType)
        Return SearchDialog(Of CriteriaType)(Caption, "")
      End Function

      ''' <summary>
      ''' Adds a search dialog for a criteria object. Add this if you want to trigger a find screen from javascript
      ''' </summary>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public Function SearchDialog(Of CriteriaType)(Caption As String, VMProperty As System.Linq.Expressions.Expression(Of Func(Of ObjectType, Object))) As SearchDialog(Of CriteriaType)
        Return SearchDialog(Of CriteriaType)(Caption, Singular.Reflection.GetMember(VMProperty).Name)
      End Function

      ''' <summary>
      ''' Adds a search dialog for a criteria object. Add this if you want to trigger a find screen from javascript
      ''' </summary>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public Function SearchDialog(Of CriteriaType)(Caption As String, VMProperty As String) As SearchDialog(Of CriteriaType)
        Dim sd = New SearchDialog(Of CriteriaType)(VMProperty)
        sd = SetupControl(sd)
        sd.DialogCaption = Caption
        Return sd
      End Function

      Public Function MessageHolder(Optional HolderName As String = MessageInfo.MainHolderName) As MessageHolderControl(Of ObjectType)
        Return SetupControl(New MessageHolderControl(Of ObjectType) With {.Name = HolderName})
      End Function

      Public Function MessageHolder(HolderName As String, MessageType As MessageType, Title As String, Message As String) As MessageHolderControl(Of ObjectType)
        Dim mhc As New MessageHolderControl(Of ObjectType)
        mhc.Name = HolderName
        SetupControl(mhc)
        mhc.AddMessage(MessageType, MessageType, Title, Message)
        Return mhc
      End Function

      Public Function Message(MessageType As MessageType, Title As String, MessageText As String) As SimpleMessage(Of ObjectType)
        Return SetupControl(New SimpleMessage(Of ObjectType)(MessageType, Title, MessageText))
      End Function

      Public Function Message(SMessage As Singular.Message) As SimpleMessage(Of ObjectType)
        Return SetupControl(New SimpleMessage(Of ObjectType)(SMessage.MessageType, SMessage.MessageTitle, SMessage.Message))
      End Function

      Public Function Button(Text As String) As Button(Of ObjectType)
        Return SetupControl(New Button(Of ObjectType) With {.Text = Text, .ButtonID = ""})
      End Function

      Public Function Button(ID As String, Text As String) As Button(Of ObjectType)
        Return SetupControl(New Button(Of ObjectType) With {.Text = Text, .ButtonID = ID})
      End Function

      Public Function Button(ButtonType As DefinedButtonType, ID As String, Text As String) As Button(Of ObjectType)
        Return SetupControl(New Button(Of ObjectType) With {.Text = Text, .ButtonID = ID, .ButtonType = ButtonType})
      End Function

      Public Function Button(ButtonType As DefinedButtonType, Text As String) As Button(Of ObjectType)
        Return SetupControl(New Button(Of ObjectType) With {.Text = Text, .ButtonID = ButtonType.ToString, .ButtonType = ButtonType})
      End Function

      Public Function Button(ButtonType As DefinedButtonType) As Button(Of ObjectType)
        Return SetupControl(New Button(Of ObjectType) With {.Text = ButtonType.ToString, .ButtonID = ButtonType.ToString, .ButtonType = ButtonType})
      End Function

      Public Function Button(Text As String, Style As ButtonMainStyle, Size As ButtonSize, Glyph As FontAwesomeIcon) As Button(Of ObjectType)
        Dim Btn = New Button(Of ObjectType) With {.Text = Text,
                                                  .ButtonID = "",
                                                  .ButtonStyle = Style,
                                                  .ButtonSize = Size
                                                  }
        Btn.RenderMode = Singular.Web.ButtonStyle.Bootstrap
        SetupControl(Btn)
        Btn.Image.Glyph = Glyph
        Return Btn
      End Function

      Public Function LabelFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As FieldLabel(Of ObjectType)
        Return SetupControl(New FieldLabel(Of ObjectType)(), le)
      End Function

      Public Function LabelFor(pi As PropertyInfo) As FieldLabel(Of ObjectType)
        Return SetupControl(New FieldLabel(Of ObjectType)(), pi)
      End Function

      Public Function ReadOnlyFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), Optional TagType As Singular.Web.FieldTagType = FieldTagType.span) As FieldDisplay(Of ObjectType)
        Return SetupControl(New FieldDisplay(Of ObjectType) With {.TagType = TagType}, le)
      End Function

      Public Function ReadOnlyFor(pi As PropertyInfo, Optional TagType As Singular.Web.FieldTagType = FieldTagType.span) As FieldDisplay(Of ObjectType)
        Return SetupControl(New FieldDisplay(Of ObjectType) With {.TagType = TagType}, pi)
      End Function

      Public Function ReadOnlyFor(Optional TagType As Singular.Web.FieldTagType = FieldTagType.span) As FieldDisplay(Of ObjectType)
        Return SetupControl(New FieldDisplay(Of ObjectType) With {.TagType = TagType})
      End Function

      Public Function TextBoxFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As TextEditor(Of ObjectType)
        Return SetupControl(New TextEditor(Of ObjectType), le)
      End Function

      Public Function EditorFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As EditorBase(Of ObjectType)
        Return SetupControl(EditorBase(Of ObjectType).GetEditor(le))
      End Function

      Public Function EditorFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), Placeholder As String) As EditorBase(Of ObjectType)
        Dim control = SetupControl(EditorBase(Of ObjectType).GetEditor(le))
        control.Attributes("placeholder") = Placeholder
        Return control
      End Function

      Public Function TimeEditorFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As TimeEditor(Of ObjectType)
        Return SetupControl(New TimeEditor(Of ObjectType), le)
      End Function

      Public Function EditorFor(pi As PropertyInfo) As EditorBase(Of ObjectType)
        Return SetupControl(EditorBase(Of ObjectType).GetEditor(pi))
      End Function

      Public Function EditorRowFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), Optional LabelOnRight As Boolean = False) As EditorRow(Of ObjectType)
        Return SetupControl(New EditorRow(Of ObjectType)(LabelOnRight), le)
      End Function

      Public Function EditorRowFor(pi As PropertyInfo) As EditorRow(Of ObjectType)
        Return SetupControl(New EditorRow(Of ObjectType), pi)
      End Function

      Public Function EditorRowFor(Editor As EditorBase(Of ObjectType), LabelText As String, Optional EditorBinding As String = "") As EditorRow(Of ObjectType)
        Dim er As New EditorRow(Of ObjectType)(Editor)
        SetupControl(er)
        If EditorBinding <> "" Then
          Editor.AddBinding(KnockoutBindingString.value, EditorBinding)
        End If
        er.Label.LabelText = LabelText
        Return er
      End Function

      Public Function ReadOnlyRowFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As ReadOnlyRow(Of ObjectType)
        Return SetupControl(New ReadOnlyRow(Of ObjectType), le)
      End Function

      Public Function ReadOnlyRowFor(pi As PropertyInfo) As ReadOnlyRow(Of ObjectType)
        Return SetupControl(New ReadOnlyRow(Of ObjectType), pi)
      End Function

      Public Function ReadOnlyRow(LabelText As String, DisplayExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As ReadOnlyRow(Of ObjectType)
        Dim ror As New ReadOnlyRow(Of ObjectType)
        SetupControl(ror)
        ror.Label.LabelText = LabelText
        ror.Display.AddBinding(KnockoutBindingString.text, DisplayExpression)
        Return ror
      End Function

      Public Function ReadOnlyRow(LabelText As String, DisplayText As String) As ReadOnlyRow(Of ObjectType)
        Dim ror As New ReadOnlyRow(Of ObjectType)
        SetupControl(ror)
        ror.Label.LabelText = LabelText
        ror.Display.DisplayText = DisplayText
        Return ror
      End Function

      Public Function FieldSet(Title As String) As FieldSet(Of ObjectType, ObjectType)
        Dim fs As FieldSet(Of ObjectType, ObjectType) = SetupControl(New FieldSet(Of ObjectType, ObjectType))
        If Title <> "" Then
          fs.Legend.Helpers.HTML(Title)
        End If
        Return fs
      End Function

      Public Function FieldSetFor(Of ChildControlsObjectType)(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As FieldSet(Of ObjectType, ChildControlsObjectType)
        Return SetupControl(New FieldSet(Of ObjectType, ChildControlsObjectType), le)
      End Function

      Public Function FieldSetFor(Of ChildControlsObjectType)(Title As String, le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As FieldSet(Of ObjectType, ChildControlsObjectType)

        Dim fs As FieldSet(Of ObjectType, ChildControlsObjectType) = FieldSetFor(Of ChildControlsObjectType)(le)
        If Title <> "" Then
          fs.Legend.Helpers.HTML(Title)
        End If
        Return fs
      End Function

      'Public Function DataPathFor(Of ChildControlsObjectType)(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As DataBindingPath(Of ObjectType, ChildControlsObjectType)
      '  Return SetupControl(New DataBindingPath(Of ObjectType, ChildControlsObjectType), le)
      'End Function

      ''' <summary>
      ''' Creates an editable table that will be bound on the client.
      ''' </summary>
      ''' <typeparam name="ChildControlsObjectType">The type of the lists items.</typeparam>
      ''' <param name="le">The property of a list to bind to.</param>
      ''' <param name="AllowAddNew">Allow the user to add items.</param>
      ''' <param name="AllowRemove">Allow the user to remove items.</param>
      Public Function TableFor(Of ChildControlsObjectType)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean) As Table(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New Table(Of ObjectType, ChildControlsObjectType)
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        SetupControl(koTbl, le)
        Return koTbl

      End Function

      Public Function TableReadOnlyFor(Of ChildControlsObjectType)(ByVal pi As PropertyInfo,
                                                                   NoDataHTML As String) As Table(Of ObjectType, ChildControlsObjectType)

        Dim ListName As String = pi.Name

        Dim Container = Div()
        Container.Helpers.HTML(NoDataHTML)
        Container.AddBinding(KnockoutBindingString.visible, ListName & "().length == 0")
        Container.AddClass("TableNoData")

        Dim Table = TableFor(Of ChildControlsObjectType)(pi, False, False)
        Table.AddBinding(KnockoutBindingString.visible, ListName & "().length > 0")
        Table.NoDataContainer = Container

        Return Table

      End Function

      Public Function TableReadOnlyFor(Of ChildControlsObjectType)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                                   NoDataHTML As String) As Table(Of ObjectType, ChildControlsObjectType)

        Return TableReadOnlyFor(Of ChildControlsObjectType)(Singular.Reflection.GetMember(le), NoDataHTML)

      End Function

      Public Function TableFor(Of ChildControlsObjectType)(ByVal pi As PropertyInfo, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean) As Table(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New Table(Of ObjectType, ChildControlsObjectType)
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        SetupControl(koTbl, pi)
        Return koTbl

      End Function

      Public Function TableFor(Of ChildControlsObjectType)(DatasourceJS As String, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean) As Table(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New Table(Of ObjectType, ChildControlsObjectType)
        koTbl.DataSourceString = DatasourceJS
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        SetupControl(koTbl)
        Return koTbl

      End Function

      Public Function PagedGridFor(Of ChildControlsObjectType)(
            ByVal PagingManager As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
            ByVal TableList As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
            ByVal AllowAddNew As Boolean,
            ByVal AllowRemove As Boolean,
            Optional PageControlsType As PageControlsType? = Nothing) As PagedGrid(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New PagedGrid(Of ObjectType, ChildControlsObjectType)
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        koTbl.PagingManager = PagingManager
        koTbl.PageControlsType = If(PageControlsType, Singular.Web.Controls.DefaultPageControlsType)
        SetupControl(koTbl, TableList)
        Return koTbl

      End Function

      Public Function PagedForEach(Of ChildControlsObjectType)(ByVal PagingManagerExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                               ByVal ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                               Optional PageControlsType As PageControlsType? = Nothing) As PagedForEach(Of ObjectType, ChildControlsObjectType)

        Dim pFE As New PagedForEach(Of ObjectType, ChildControlsObjectType)(PagingManagerExpression)
        pFE.PageControlsType = If(PageControlsType, Singular.Web.Controls.DefaultPageControlsType)
        SetupControl(pFE, ListProperty)
        Return pFE

      End Function

      Public Function CanvasGrid(ByVal GridInfoProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                 Width As String, Height As String) As Singular.Web.CustomControls.SGrid.GridContainer(Of ObjectType)

        Dim gc As New Singular.Web.CustomControls.SGrid.GridContainer(Of ObjectType)(GridInfoProperty)
        SetupControl(gc)
        gc.Grid.Style.Height = Height
        gc.Style.Width = Width
        Return gc

      End Function

      ''' <summary>
      ''' Creates a static table that is bound on the server.
      ''' </summary>
      ''' <typeparam name="ChildControlsObjectType"></typeparam>
      ''' <param name="List"></param>
      Public Function Table(Of ChildControlsObjectType)(ByVal List As Object) As Table(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New Table(Of ObjectType, ChildControlsObjectType)
        koTbl.ServerBindObject = List
        koTbl.AllowAddNew = False
        koTbl.AllowRemove = False
        SetupControl(koTbl)


        Return koTbl

      End Function

      Public Function Table() As Table(Of ObjectType, ObjectType)

        Dim tbl As New Table(Of ObjectType, ObjectType)
        tbl.AllowAddNew = False
        tbl.AllowRemove = False
        Return SetupControl(tbl)

      End Function

      Public Function ForEachTemplate(Of ChildControlsObjectType)(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As ForEach(Of ObjectType, ChildControlsObjectType)

        Dim fe As New ForEach(Of ObjectType, ChildControlsObjectType)
        Return SetupControl(fe, le)

      End Function

      Public Function ForEachTemplate(Of ChildControlsObjectType)(JSExpression As String) As ForEach(Of ObjectType, ChildControlsObjectType)

        Dim fe As New ForEach(Of ObjectType, ChildControlsObjectType)
        fe.JSExpression = JSExpression
        Return SetupControl(fe)

      End Function

      Public Function TabControl() As TabControl(Of ObjectType)
        Return SetupControl(New TabControl(Of ObjectType))
      End Function

      Public Function TabControl(TabIndexProperty As System.Linq.Expressions.Expression(Of Func(Of ObjectType, Object))) As TabControl(Of ObjectType)
        Return SetupControl(New TabControl(Of ObjectType) With {.TabIndexProperty = TabIndexProperty})
      End Function

      ''' <summary>
      ''' Creates an anchor tag.
      ''' </summary>
      ''' <param name="HRefExpression">Dynamic href</param>
      ''' <param name="LinkTextExpression">Dynamic inner text.</param>
      ''' <param name="HrefString">The static location to redirect to. If you want to call a js function, specify it with the brackets. e.g. OnLinkClick()</param>
      ''' <param name="LinkTextString">Static inner text.</param>
      ''' <param name="Target">If a new window or tab must be opened.</param>
      Public Function LinkFor(Optional HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                              Optional LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                              Optional HrefString As String = "",
                              Optional LinkTextString As String = "",
                              Optional Target As Singular.Web.LinkTargetType = LinkTargetType.NotSet) As Link(Of ObjectType)
        Dim Link As New Link(Of ObjectType)
        Link.HRefExpression = HRefExpression
        Link.LinkTextExpression = LinkTextExpression
        Link.Href = HrefString
        Link.LinkText = LinkTextString
        Link.Target = Target
        Return SetupControl(Link)

      End Function

      Public Function DocumentLinkFor(DocumentIDPropertyName As System.Linq.Expressions.Expression(Of Func(Of ObjectType, Object)),
                                      DocumentNamePropertyName As System.Linq.Expressions.Expression(Of Func(Of ObjectType, Object)), Optional QueryString As String = "") As Link(Of ObjectType)

        Dim Link = LinkFor(, DocumentNamePropertyName)
        Dim PropertyName = KnockoutBindingManager(Of ObjectType).CreateBindingExpression(KnockoutBindingString.src, DocumentIDPropertyName, False)
        Link.AddBinding(KnockoutBindingString.href, "Singular.DownloadPath(" & PropertyName & ")" & If(String.IsNullOrEmpty(QueryString), "", " + '&" & QueryString + "'"))
        Return Link

      End Function

      Public Function Div() As HTMLDiv(Of ObjectType)
        Dim d As New HTMLDiv(Of ObjectType)
        Return SetupControl(d)
      End Function

      ''' <summary>
      ''' Adds a div with display: inline-block
      ''' </summary>
      Public Function DivInline() As HTMLDiv(Of ObjectType)
        Dim d As New HTMLDiv(Of ObjectType)
        SetupControl(d)
        d.Style.Display = Display.inlineblock
        Return d
      End Function

      Public Function Span(Optional InnerText As String = "") As HTMLTag(Of ObjectType)
        Return HTMLTag("span", InnerText)
      End Function

      Public Function Span(TextBinding As Expression(Of Func(Of ObjectType, Object))) As HTMLTag(Of ObjectType)
        Dim s = HTMLTag("span")
        s.AddBinding(KnockoutBindingString.text, TextBinding)
        Return s
      End Function

      ''' <summary>
      ''' Allows you specify the css classes in the constrcutor 
      ''' </summary>
      ''' <param name="CssClasses"></param>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public Function SpanC(CssClasses As String) As HTMLSpan(Of ObjectType)
        Dim d As New HTMLSpan(Of ObjectType)
        d.AddClass(CssClasses)
        Return SetupControl(d)
      End Function

      Public Function PopupDialog(Title As String, InnerText As String) As ImageWithPopup(Of ObjectType)
        Dim pd As New ImageWithPopup(Of ObjectType) With {.Title = Title, .InnerText = InnerText}
        Return SetupControl(pd)
      End Function

      Public Function Dialog(PopupCondition As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                             Title As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                             Optional OnCloseFunctionName As String = "") As Dialog(Of ObjectType)

        Return Dialog(KnockoutBindingManager(Of ObjectType).CreateBindingExpression(KnockoutBindingString.visible, PopupCondition, False),
                      KnockoutBindingManager(Of ObjectType).CreateBindingExpression(KnockoutBindingString.text, Title, False),
                      OnCloseFunctionName)

      End Function

      Public Function Dialog(PopupConditionJS As String, Title As String, Optional OnCloseFunctionName As String = "") As Dialog(Of ObjectType)

        Dim dlg As New Dialog(Of ObjectType)
        dlg.PopupCondition = PopupConditionJS
        dlg.Title = Title
        dlg.OnCloseFunctionName = OnCloseFunctionName
        Return SetupControl(dlg)

      End Function

      ''' <summary>
      ''' Creates a toolbar to put buttons etc in. Also contains a popup validation control if specified.
      ''' </summary>
      ''' <param name="AddValidation">True will add a popup validation control.</param>
      Public Function Toolbar(Optional AddValidation As Boolean = True) As Toolbar(Of ObjectType)
        Dim tb As New Toolbar(Of ObjectType)(AddValidation)
        Return SetupControl(tb)
      End Function

      Public Function Image(Optional Src As String = "",
                            Optional SrcDefined As DefinedImageType = DefinedImageType.None,
                            Optional SrcExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing) As Image(Of ObjectType)
        Dim img As New Image(Of ObjectType)
        img.Src = Src
        img.SrcDefined = SrcDefined
        SetupControl(img)
        If SrcExpression IsNot Nothing Then
          img.AddBinding(KnockoutBindingString.src, SrcExpression)
        End If
        Return img
      End Function

      Public Function DocumentManager() As DocumentManager(Of ObjectType)
        Dim dm As New DocumentManager(Of ObjectType)
        Return SetupControl(dm)
      End Function

      Public Function DocumentManager(Of DocumentProviderType)(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As DocumentManager(Of DocumentProviderType)
        Dim c As New NoContainer(Of ObjectType, DocumentProviderType)
        c.BindingType = KnockoutBindingString.with
        SetupControl(c, le)
        Return c.Helpers.DocumentManager()
      End Function

      Public Function DocumentManager(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As DocumentManager(Of Singular.Documents.IDocumentProvider)
        Dim c As New NoContainer(Of ObjectType, Singular.Documents.IDocumentProvider)
        c.BindingType = KnockoutBindingString.with
        SetupControl(c, le)
        Return c.Helpers.DocumentManager()
      End Function

      Public Function DocumentManager(pi As PropertyInfo) As DocumentManager(Of Singular.Documents.IDocumentProvider)
        Dim c As New NoContainer(Of ObjectType, Singular.Documents.IDocumentProvider)
        c.BindingType = KnockoutBindingString.with
        SetupControl(c, pi)
        Return c.Helpers.DocumentManager()
      End Function

      Public Function DocumentDownloader() As DocumentDownloader(Of ObjectType)
        Dim dm As New DocumentDownloader(Of ObjectType)
        Return SetupControl(dm)
      End Function

      Public Function DocumentDownloader(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As DocumentDownloader(Of Singular.Documents.IDocumentProvider)
        Dim c As New NoContainer(Of ObjectType, Singular.Documents.IDocumentProvider)
        c.BindingType = KnockoutBindingString.with
        SetupControl(c, le)
        Return c.Helpers.DocumentDownloader()
      End Function

      Public Function CaptchaEntry() As CaptchaEntry(Of ObjectType)
        Dim c As New CaptchaEntry(Of ObjectType)
        Return SetupControl(c)
      End Function

      Public Function [If](le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As NoContainer(Of ObjectType)
        Dim c As New NoContainer(Of ObjectType)
        c.BindingType = KnockoutBindingString.if
        Return SetupControl(c, le)
      End Function

      Public Function [If](JSCondition As String) As NoContainer(Of ObjectType)
        Dim c As New NoContainer(Of ObjectType)
        c.BindingType = KnockoutBindingString.if
        c.JSFunction = JSCondition
        Return SetupControl(c)
      End Function

      Public Function [With](Of ChildType)(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As NoContainer(Of ObjectType, ChildType)
        Dim c As New NoContainer(Of ObjectType, ChildType)
        c.BindingType = KnockoutBindingString.with
        Return SetupControl(c, le)
      End Function

      Public Function [With](Of ChildType)(Data As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                           Condition As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As NoContainer(Of ObjectType, ChildType)
        Dim c As New NoContainer(Of ObjectType, ChildType)
        c.BindingType = KnockoutBindingString.with
        c.Condition = Condition
        Return SetupControl(c, Data)
      End Function

      Public Function [With](Of ChildType)(pi As PropertyInfo) As NoContainer(Of ObjectType, ChildType)
        Dim c As New NoContainer(Of ObjectType, ChildType)
        c.BindingType = KnockoutBindingString.with
        Return SetupControl(c, pi)
      End Function

      Public Function [With](Of ChildType)(JSBinding As String) As NoContainer(Of ObjectType, ChildType)
        Dim c As New NoContainer(Of ObjectType, ChildType)
        c.BindingType = KnockoutBindingString.with
        c.JSFunction = JSBinding
        Return SetupControl(c)
      End Function

      Public Function [ForEach](le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As NoContainer(Of ObjectType)
        Dim c As New NoContainer(Of ObjectType)
        c.BindingType = KnockoutBindingString.foreach
        Return SetupControl(c, le)
      End Function

      Public Function [ForEach](Of ChildType)(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As NoContainer(Of ObjectType, ChildType)
        Dim c As New NoContainer(Of ObjectType, ChildType)
        c.BindingType = KnockoutBindingString.foreach
        Return SetupControl(c, le)
      End Function

      Public Function [ForEach](JSFunction As String) As NoContainer(Of ObjectType)
        Dim c As New NoContainer(Of ObjectType)
        c.BindingType = KnockoutBindingString.foreach
        c.JSFunction = JSFunction
        Return SetupControl(c)
      End Function

      Public Function [ForEach](Of ChildType)(JSFunction As String) As NoContainer(Of ObjectType, ChildType)
        Dim c As New NoContainer(Of ObjectType, ChildType)
        c.BindingType = KnockoutBindingString.foreach
        c.JSFunction = JSFunction
        Return SetupControl(c)
      End Function

      Public Function ChangeType(Of ChangeTypeTo)() As NullControl(Of ObjectType, ChangeTypeTo)
        Dim c As New NullControl(Of ObjectType, ChangeTypeTo)
        Return SetupControl(c)
      End Function

      Public Function StarRatingControl(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As StarRating(Of ObjectType)
        Dim c As New CustomControls.StarRating(Of ObjectType)
        Return SetupControl(c, le)
      End Function

      Public Function StarRatingControl(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), MaxPoints As Integer) As StarRating(Of ObjectType)
        Dim c As New CustomControls.StarRating(Of ObjectType) With {.MaxPoints = MaxPoints}
        Return SetupControl(c, le)
      End Function

      Public Function StarRatingControl(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), MaxPoints As Integer, ToolTipDataSourceName As String) As StarRating(Of ObjectType)
        Dim c As New CustomControls.StarRating(Of ObjectType) With {.MaxPoints = MaxPoints, .ToolTipDataSourceName = ToolTipDataSourceName}
        Return SetupControl(c, le)
      End Function

      Public Function StarRatingControl(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), MaxPointsJS As String, ToolTipDataSource As String) As StarRating(Of ObjectType)
        Dim c As New CustomControls.StarRating(Of ObjectType) With {.MaxPointsJS = MaxPointsJS, .ToolTipDataSource = ToolTipDataSource}
        Return SetupControl(c, le)
      End Function

      Public Function AddOnTheFly(Of ChildType)(Callback As Action(Of HelperAccessors(Of ChildType)), Optional ShowCancel As Boolean = True) As AddOnTheFly(Of ObjectType, ChildType)
        Dim aotf As New AddOnTheFly(Of ObjectType, ChildType)(Callback, ShowCancel)
        Return SetupControl(aotf)
      End Function

      Public Function Template(Of ChildType)(IDString As String) As Template(Of ObjectType, ChildType)
        Dim t As New Template(Of ObjectType, ChildType)(IDString)
        Return SetupControl(t)
      End Function

      Public Function UseTemplate(DataContext As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), TemplateID As String) As NoContainer(Of ObjectType)
        Dim nc As New NoContainer(Of ObjectType)
        SetupControl(nc, DataContext)
        nc.MakeTemplate(TemplateID)
        Return nc
      End Function

      Public Function UseTemplate(TemplateID As String) As NoContainer(Of ObjectType)
        Return UseTemplate("$data", TemplateID)
      End Function

      Public Function UseTemplate(DataContext As String, TemplateID As String) As NoContainer(Of ObjectType)
        Dim nc As New NoContainer(Of ObjectType)
        SetupControl(nc)
        nc.MakeTemplate(TemplateID, DataContext)
        Return nc
      End Function

      Public Function AddInlineLoadingBar(Condition As String)

        If mParentControl IsNot Nothing Then
          mParentControl.Style.Position = Singular.Web.Position.relative
        End If

        Dim LoadingBar As New Singular.Web.CustomControls.LoadingOverlay(Of ObjectType)
        SetupControl(LoadingBar)
        LoadingBar.Bindings.AddVisibilityBinding(Condition, Singular.Web.VisibleFadeType.None, Singular.Web.VisibleFadeType.None, "Singular.InlineLoadingBarShown")
        Return LoadingBar

      End Function

      Public Function Slider(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As Slider(Of ObjectType)
        Dim control As New Slider(Of ObjectType)
        Return SetupControl(control, le)
      End Function

      Public Function Slider(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), MinValue As Decimal, MaxValue As Decimal, StepLength As Decimal) As Slider(Of ObjectType)
        Dim control As New Slider(Of ObjectType) With {.MinValue = MinValue, .MaxValue = MaxValue, .StepLength = StepLength}
        Return SetupControl(control, le)
      End Function

      ''' <summary>
      ''' Adds an image resize dialog control to the page. Must be used with an ImageChooser control. ImageResizer.js must also be included in your page.
      ''' </summary>
      Public Sub AddImageResizeDialog()
        Control(Of ImageResizeDialog)()
      End Sub

      ''' <summary>
      ''' Adds a button which allows the user to select, and resize an image. Must be used with an ImageResizeDialog
      ''' </summary>
      ''' <param name="RequiredWidth">Required width in pixels of the saved image</param>
      ''' <param name="RequiredHeight">Required height in pixels of the saved image</param>
      ''' <param name="Scale">Scale that the dialog must display. Use this to reduce the size of the dialog if a large image size is required.</param>
      ''' <param name="BackColor">If blank, the background will be transparent, and the image will be saved according to its original dimensions if smaller than the required size.
      ''' If a background color is specified, the image will be bordered with this color, and will be the required diminesions.
      ''' Use this is the image needs to be displayed on a crystal report.</param>
      Public Function ImageChooser(DocumentIDProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                   RequiredWidth As Integer, RequiredHeight As Integer, Scale As Decimal, Optional PromptText As String = "Choose Image",
                                   Optional BackColor As String = "") As ImageChooser(Of ObjectType)
        Dim ic As New ImageChooser(Of ObjectType) With {.RequiredWidth = RequiredWidth,
                                           .RequiredHeight = RequiredHeight,
                                           .Scale = Scale,
                                           .PromptText = PromptText,
                                            .BackColor = BackColor}
        Return SetupControl(ic, DocumentIDProperty)
      End Function

      ''' <summary>
      ''' For use with ImageChooser
      ''' </summary>
      Public Function DocumentImage(ImageID As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As ImageViewer(Of ObjectType)
        Dim iv As New ImageViewer(Of ObjectType)
        Return SetupControl(iv, ImageID)
      End Function

#Region " Bootstrap "

      Public Function BootstrapToolbar(Optional InformationInd As Boolean = False, Optional WarningsInd As Boolean = False, Optional LeftAlignInd As Boolean = False) As BootstrapToolbar(Of ObjectType)
        Dim tb As New BootstrapToolbar(Of ObjectType)(InformationInd, WarningsInd, LeftAlignInd)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapEditorRowFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                             Optional RowContainerClasses As String = "",
                                             Optional EditorContainerClasses As String = "",
                                             Optional LabelClasses As String = "",
                                             Optional LabelAttributes As String = "",
                                             Optional EditorClasses As String = "",
                                             Optional EditorAttributes As String = "") As BootstrapEditorRow(Of ObjectType)
        Return SetupControl(New BootstrapEditorRow(Of ObjectType)(RowContainerClasses, EditorContainerClasses, LabelClasses, LabelAttributes, EditorClasses, EditorAttributes), le)
      End Function

      Public Function BootstrapDialog(ID As String, Title As String) As BootstrapDialog(Of ObjectType)
        Dim tb As New BootstrapDialog(Of ObjectType)(ID, Title)
        Return SetupControl(tb)
      End Function

      'Public Function BootstrapTable(ColumnNames As String(), ColumnClasses As String) As BootstrapTable(Of ObjectType)
      '  Dim tb As New BootstrapTable(Of ObjectType)(ColumnNames, ColumnClasses)
      '  Return SetupControl(tb)
      'End Function

      Public Function BootstrapBadge(BadgeName As String, IconName As String, HTMLText As String) As BootstrapBadge(Of ObjectType)
        '= "", Optional CssClasses As String = "", Optional OnClickJSFunction As String = ""Href As String, 
        Dim tb As New BootstrapBadge(Of ObjectType)(BadgeName, IconName, HTMLText)
        ', CssClasses, OnClickJSFunction
        Return SetupControl(tb)
      End Function

      Public Function BootstrapPreviousNext(Optional PreviousButtonText As String = "", Optional NextButtonText As String = "", Optional PreviousButtonIcon As String = "", Optional NextButtonIcon As String = "") As BootstrapPreviousNext(Of ObjectType)
        Dim tb As New BootstrapPreviousNext(Of ObjectType)(PreviousButtonText, NextButtonText, PreviousButtonIcon, NextButtonIcon)
        Return SetupControl(tb)
      End Function

      ''' <summary>
      ''' Allows you specify the css classes in the constrcutor 
      ''' </summary>
      ''' <param name="CssClasses"></param>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public Function DivC(CssClasses As String) As HTMLDiv(Of ObjectType)
        Dim d As New HTMLDiv(Of ObjectType)
        d.AddClass(CssClasses)
        Return SetupControl(d)
      End Function

      Public Function ColContainer(MaxColumns As Integer, ParamArray controls() As HelperBase(Of ObjectType)) As HTMLDiv(Of ObjectType)
        Dim colClass As String = "col" & Math.Min(MaxColumns, controls.Length)
        Dim Container = DivC("container")
        For Each ctl In controls
          Container.Helpers.DivC(colClass).Helpers.Control(ctl)
        Next
        Return Container
      End Function

      Public Function BootstrapGlyphIcon(IconName As String) As BootstrapGlyphIcon(Of ObjectType)
        Dim tb As New BootstrapGlyphIcon(Of ObjectType)(IconName)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapButton(Optional ID As String = "",
                                       Optional Text As String = "",
                                       Optional CssClass As String = "",
                                       Optional IconName As String = "",
                                       Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                                       Optional ButtonGroupInd As Boolean = True,
                                       Optional TooltipPlacement As String = "right",
                                       Optional TooltipText As String = "",
                                       Optional ClickBinding As String = "") As BootstrapButton(Of ObjectType)
        Dim tb As New BootstrapButton(Of ObjectType)(ID, Text, CssClass, IconName, PostBackType, ButtonGroupInd, TooltipPlacement, TooltipText, ClickBinding)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapThumbnail(ImagePath As String,
                                         AlternateImagePath As String,
                                         CssClasses As String) As BootstrapThumbnail(Of ObjectType)

        Dim tb As New BootstrapThumbnail(Of ObjectType)(ImagePath, AlternateImagePath, CssClasses)
        Return SetupControl(tb)

      End Function

      Public Function BootstrapLinkFor(Optional HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                              Optional LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                              Optional HrefString As String = "",
                              Optional LinkTextString As String = "",
                              Optional Target As Singular.Web.LinkTargetType = LinkTargetType.NotSet,
                              Optional GlyphiconName As String = "") As BootstrapLink(Of ObjectType)



        Dim Link As New BootstrapLink(Of ObjectType)
        Link.HRefExpression = HRefExpression
        Link.LinkTextExpression = LinkTextExpression
        Link.Href = HrefString
        Link.LinkText = LinkTextString
        Link.Target = Target
        Link.GlyphiconName = GlyphiconName

        Return SetupControl(Link)

      End Function

      Public Function BootstrapButtonDropDown(Optional GlyphIconName As String = "",
                                               Optional ActionButtonColorClass As String = "btn-default",
                                               Optional DropDownButtonColorClass As String = "btn-default",
                                               Optional ActionTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                               Optional le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                               Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                                               Optional ActionButtonClickBinding As String = "",
                                               Optional ButtonID As String = "") As BootstrapButtonDropDown(Of ObjectType)

        Dim BBD As New BootstrapButtonDropDown(Of ObjectType)(GlyphIconName, ActionButtonColorClass, DropDownButtonColorClass, ActionTextExpression, PostBackType, ActionButtonClickBinding, ButtonID)
        Return SetupControl(BBD, le)

      End Function

      Public Function BootstrapStateButton(ID As String,
                                           OnClickFunction As String,
                                           CssFunction As String,
                                           HtmlFunction As String,
                                           ButtonGroupInd As Boolean) As BootstrapStateButton(Of ObjectType)
        Dim tb As New BootstrapStateButton(Of ObjectType)(ID, OnClickFunction, CssFunction, HtmlFunction, ButtonGroupInd)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapInputGroup(Optional PropertyExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                           Optional PreInput As Boolean = False,
                                           Optional PreInputExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                           Optional PostInput As Boolean = False,
                                           Optional PostInputExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing) As BootstrapInputGroup(Of ObjectType)

        Dim tb As New BootstrapInputGroup(Of ObjectType)(PropertyExpression, PreInput, PreInputExpression, PostInput, PostInputExpression)
        Return SetupControl(tb)

      End Function

      Public Function BootstrapPanel(Optional PanelColorClass As String = "panel-primary") As BootstrapPanel(Of ObjectType)
        Dim tb As New BootstrapPanel(Of ObjectType)(PanelColorClass)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapPageHeader(Optional HeadingSize As Integer = 2, Optional HeadingText As String = "Heading", Optional SubText As String = "",
                                          Optional HeadingTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing) As BootstrapPageHeader(Of ObjectType)
        Dim tb As New BootstrapPageHeader(Of ObjectType)(HeadingSize, HeadingText, SubText, HeadingTextExpression)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapDivColumn(Optional SmallSize As Integer = 2, Optional MediumSize As Integer = 2, Optional LargeSize As Integer = 2, Optional ExtraSmallSize As Integer = 12) As HTMLDiv(Of ObjectType)
        'col-sm-12 col-md-2 col-lg-2
        Dim d As New HTMLDiv(Of ObjectType)
        If ExtraSmallSize > 0 Then
          d.AddClass("col-xs-" & ExtraSmallSize.ToString)
        End If
        If SmallSize > 0 Then
          d.AddClass("col-sm-" & SmallSize.ToString)
        End If
        If MediumSize > 0 Then
          d.AddClass("col-md-" & MediumSize.ToString)
        End If
        If LargeSize > 0 Then
          d.AddClass("col-lg-" & LargeSize.ToString)
        End If
        Return SetupControl(d)
      End Function

      Public Function BootstrapUnorderedList(ID As String, CssClass As String) As BootstrapUnorderedList(Of ObjectType)
        Dim tb As New BootstrapUnorderedList(Of ObjectType)(ID, CssClass)
        Return SetupControl(tb)
      End Function

      'Public Function BootstrapTableFor(Of ChildControlsObjectType)(DatasourceJS As String, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean) As BootstrapTable(Of ObjectType, ChildControlsObjectType)

      '  Dim koTbl As New BootstrapTable(Of ObjectType, ChildControlsObjectType)
      '  koTbl.DataSourceString = DatasourceJS
      '  koTbl.AllowAddNew = AllowAddNew
      '  koTbl.AllowRemove = AllowRemove
      '  SetupControl(koTbl)
      '  Return koTbl

      'End Function

      Public Function BootstrapTableFor(Of ChildControlsObjectType)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                                    ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                                    IFExpression As String, Optional Striped As Boolean = True) As BootstrapTable(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New BootstrapTable(Of ObjectType, ChildControlsObjectType)
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        koTbl.IfExpression = IFExpression
        koTbl.Striped = Striped
        SetupControl(koTbl, le)
        Return koTbl

      End Function

      Public Function BootstrapTableFor(Of ChildControlsObjectType)(ByVal pi As PropertyInfo, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean, IFExpression As String, Optional Striped As Boolean = True) As BootstrapTable(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New BootstrapTable(Of ObjectType, ChildControlsObjectType)
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        koTbl.IfExpression = IFExpression
        koTbl.Striped = Striped
        SetupControl(koTbl, pi)
        Return koTbl

      End Function

      Public Function BootstrapTableFor(Of ChildControlsObjectType)(DatasourceJS As String, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean, IFExpression As String, Optional Striped As Boolean = True) As BootstrapTable(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New BootstrapTable(Of ObjectType, ChildControlsObjectType)
        koTbl.DataSourceString = DatasourceJS
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        koTbl.Striped = Striped
        SetupControl(koTbl)
        Return koTbl

      End Function

      Public Function BootstrapTableFor(Of ChildControlsObjectType)(ByVal pi As PropertyInfo, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean, Optional Striped As Boolean = True) As BootstrapTable(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New BootstrapTable(Of ObjectType, ChildControlsObjectType)
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        koTbl.Striped = Striped
        SetupControl(koTbl, pi)
        Return koTbl

      End Function

      Public Function BootstrapTableFor(Of ChildControlsObjectType)(DatasourceJS As String, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean, Optional Striped As Boolean = True) As BootstrapTable(Of ObjectType, ChildControlsObjectType)

        Dim koTbl As New BootstrapTable(Of ObjectType, ChildControlsObjectType)
        koTbl.DataSourceString = DatasourceJS
        koTbl.AllowAddNew = AllowAddNew
        koTbl.AllowRemove = AllowRemove
        koTbl.Striped = Striped
        SetupControl(koTbl)
        Return koTbl

      End Function

      Public Function BootstrapProgressBar(Percentage As Decimal, ActionText As String) As BootstrapProgressBar(Of ObjectType)
        Dim tb As New BootstrapProgressBar(Of ObjectType)(Percentage, ActionText)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapPanelGroup(ID As String) As BootstrapPanelGroup(Of ObjectType)
        Dim tb As New BootstrapPanelGroup(Of ObjectType)(ID)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapTabControl(Optional ParentDivCssClass As String = "", Optional ByVal TabCssClass As String = "nav-justified", Optional ByVal TabRole As String = "tablist") As BootstrapTabControl(Of ObjectType)
        Dim tb As New BootstrapTabControl(Of ObjectType)(ParentDivCssClass, TabCssClass, TabRole)
        Return SetupControl(tb)
      End Function

      Public Function BootstrapROStateButton(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                             ByVal CheckedButtonColor As String, ByVal CheckedIcon As String, CheckedText As String,
                                             ByVal UnCheckedButtonColor As String, UnCheckedIcon As String, UnCheckedText As String) As BootstrapROStateButton(Of ObjectType)
        Dim tb As New BootstrapROStateButton(Of ObjectType)(ListProperty,
                                                            CheckedButtonColor, CheckedIcon, CheckedText,
                                                            UnCheckedButtonColor, UnCheckedIcon, UnCheckedText)
        Return SetupControl(tb)
      End Function



      Private mBootstrapAccessors As New BootstrapAccessors(Me)
      Public ReadOnly Property Bootstrap As BootstrapAccessors
        Get
          Return mBootstrapAccessors
        End Get
      End Property

      Public Class BootstrapAccessors

        Private mParent As HelperAccessors(Of ObjectType)
        Public Sub New(Parent As HelperAccessors(Of ObjectType))
          mParent = Parent
        End Sub

        Public Function Panel(Style As BootstrapEnums.Style, HeadingText As String, Optional CustomStyleClass As String = "") As CustomControls.Bootstrap.Panel(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.Panel(Of ObjectType)(Style, HeadingText, CustomStyleClass)
          Return mParent.SetupControl(tb)
        End Function

        Public Function Column(Optional ExtraSmallSize As Integer = 12, Optional SmallSize As Integer = 12, Optional MediumSize As Integer = 12,
                               Optional LargeSize As Integer = 12, Optional ExtraLargeSize As Integer = 0) As HTMLDiv(Of ObjectType)
          Dim d As New HTMLDiv(Of ObjectType)
          If ExtraSmallSize > 0 Then
            d.AddClass("col-xs-" & ExtraSmallSize.ToString)
          End If
          If SmallSize > 0 Then
            d.AddClass("col-sm-" & SmallSize.ToString)
          End If
          If MediumSize > 0 Then
            d.AddClass("col-md-" & MediumSize.ToString)
          End If
          If LargeSize > 0 Then
            d.AddClass("col-lg-" & LargeSize.ToString)
          End If
          If ExtraLargeSize > 0 Then
            d.AddClass("col-xl-" & ExtraLargeSize.ToString)
          End If
          Return mParent.SetupControl(d)
        End Function

        Public Function Row() As HTMLDiv(Of ObjectType)
          Dim d As New HTMLDiv(Of ObjectType)
          d.AddClass("row")
          Return mParent.SetupControl(d)
        End Function

        Public Function FontAwesomeIcon(IconName As String,
                                        Optional IconSize As String = "") As CustomControls.Bootstrap.FontAwesomeIcon(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.FontAwesomeIcon(Of ObjectType)(IconName, IconSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function Button(Optional ID As String = "",
                               Optional Text As String = "",
                               Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                               Optional CustomStyleClass As String = "",
                               Optional ButtonSize As BootstrapEnums.ButtonSize = BootstrapEnums.ButtonSize.ExtraSmall,
                               Optional CustomSizeClass As String = "",
                               Optional IconName As String = "",
                               Optional IconSize As String = "",
                               Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                               Optional ClickBinding As String = "",
                               Optional TextBeforeIcon As Boolean = False) As CustomControls.Bootstrap.Button(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.Button(Of ObjectType)(ID, Text, ButtonStyle, CustomStyleClass, ButtonSize, CustomSizeClass, IconName, IconSize, PostBackType, ClickBinding, TextBeforeIcon)
          Return mParent.SetupControl(tb)
        End Function

        Public Function FormGroup(Optional FormGroupSize As BootstrapEnums.FormGroupSize = BootstrapEnums.FormGroupSize.Small,
                                  Optional CustomSizeClass As String = "") As HTMLDiv(Of ObjectType)
          Dim tb As New HTMLDiv(Of ObjectType)
          tb.AddClass("form-group")
          If FormGroupSize = BootstrapEnums.FormGroupSize.Small Then
            tb.AddClass("form-group-sm")
          ElseIf FormGroupSize = BootstrapEnums.FormGroupSize.Large Then
            tb.AddClass("form-group-lg")
          ElseIf FormGroupSize = BootstrapEnums.FormGroupSize.Custom Then
            tb.AddClass(CustomSizeClass)
          ElseIf FormGroupSize = BootstrapEnums.FormGroupSize.ExtraSmall Then
            tb.AddClass("form-group-xs")
          End If
          Return mParent.SetupControl(tb)
        End Function

        Public Function Well(WellSize As BootstrapEnums.WellSize, Optional CustomSizeClass As String = "") As HTMLDiv(Of ObjectType)
          Dim tb As New HTMLDiv(Of ObjectType)
          tb.AddClass("well ")
          If WellSize = BootstrapEnums.WellSize.Default Then
            'do nothing
          ElseIf WellSize = BootstrapEnums.WellSize.Small Then
            tb.AddClass("well-sm")
          ElseIf WellSize = BootstrapEnums.WellSize.Large Then
            tb.AddClass("well-lg")
          ElseIf WellSize = BootstrapEnums.WellSize.Custom Then
            tb.AddClass(CustomSizeClass)
          End If
          Return mParent.SetupControl(tb)
        End Function

        Public Function PageHeader(HeaderText As String, SubText As String) As HTMLDiv(Of ObjectType)
          Dim tb As New HTMLDiv(Of ObjectType)
          tb.AddClass("page-header")
          Dim hdr As HTMLTag(Of ObjectType) = tb.Helpers.HTMLTag("h1")
          hdr.Helpers.HTML(HeaderText.ToString & " <small>" & SubText.ToString & "</small>")
          Return mParent.SetupControl(tb)
        End Function

        Public Function TabControl(Optional ParentDivCssClass As String = "", Optional ByVal TabCssClass As String = "nav-justified",
                                   Optional ByVal TabRole As String = "tablist",
                                   Optional TabAlignment As BootstrapEnums.TabAlignment = BootstrapEnums.TabAlignment.Top) As CustomControls.Bootstrap.TabControl(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.TabControl(Of ObjectType)(ParentDivCssClass, TabCssClass, TabRole, TabAlignment)
          Return mParent.SetupControl(tb)
        End Function

        Public Function Dialog(ID As String, Title As String,
                               Optional ExcludeHeader As Boolean = False,
                               Optional ModalSizeClass As String = "",
                               Optional HeaderColor As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                               Optional CustomHeaderClass As String = "",
                               Optional IconName As String = "",
                               Optional IconSize As String = "fa-2x",
                               Optional HideFooter As Boolean = False) As CustomControls.Bootstrap.Dialog(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.Dialog(Of ObjectType)(ID, Title, ExcludeHeader, ModalSizeClass, HeaderColor, CustomHeaderClass,
                                                                       IconName, IconSize, HideFooter)
          Return mParent.SetupControl(tb)
        End Function

        Public Function StateButton(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                     Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                                     Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                                     Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-minus",
                                     Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.StateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.StateButton(Of ObjectType)(ListProperty,
                                                                            TrueText, FalseText,
                                                                            TrueCss, FalseCss,
                                                                            TrueIconName, FalseIconName, ButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function StateButton(ListProperty As String,
                                    Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                                    Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                                    Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-minus",
                                    Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.StateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.StateButton(Of ObjectType)(ListProperty,
                                                                            TrueText, FalseText,
                                                                            TrueCss, FalseCss,
                                                                            TrueIconName, FalseIconName, ButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function StateButtonNew(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                     Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                                     Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                                     Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-minus",
                                     Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.StateButtonNew(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.StateButtonNew(Of ObjectType)(ListProperty,
                                                                            TrueText, FalseText,
                                                                            TrueCss, FalseCss,
                                                                            TrueIconName, FalseIconName, ButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function StateButtonNew(ListProperty As String,
                                    Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                                    Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                                    Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-minus",
                                    Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.StateButtonNew(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.StateButtonNew(Of ObjectType)(ListProperty,
                                                                            TrueText, FalseText,
                                                                            TrueCss, FalseCss,
                                                                            TrueIconName, FalseIconName, ButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function StateDiv(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                             Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                             Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                             Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-times",
                             Optional ButtonSize As String = "btn-xs",
                             Optional IsRaw As Boolean = False) As CustomControls.Bootstrap.StateDiv(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.StateDiv(Of ObjectType)(ListProperty,
                                                                            TrueText, FalseText,
                                                                            TrueCss, FalseCss,
                                                                            TrueIconName, FalseIconName, ButtonSize, IsRaw)
          Return mParent.SetupControl(tb)
        End Function

        Public Function StateDiv(ListProperty As String,
                                    Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                                    Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                                    Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-times",
                                    Optional ButtonSize As String = "btn-xs",
                                    Optional IsRaw As Boolean = False) As CustomControls.Bootstrap.StateDiv(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.StateDiv(Of ObjectType)(ListProperty,
                                                                            TrueText, FalseText,
                                                                            TrueCss, FalseCss,
                                                                            TrueIconName, FalseIconName, ButtonSize, IsRaw)
          Return mParent.SetupControl(tb)
        End Function

        Public Function ROStateButton(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                     Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                                     Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                                     Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-times",
                                     Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.ROStateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.ROStateButton(Of ObjectType)(ListProperty,
                                                                            TrueText, FalseText,
                                                                            TrueCss, FalseCss,
                                                                            TrueIconName, FalseIconName, ButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function Image(Optional PhotoPathExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                              Optional PhotoPath As String = "",
                              Optional CssClasses As String = "") As CustomControls.Bootstrap.Image(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.Image(Of ObjectType)(PhotoPathExpression, PhotoPath, CssClasses)
          Return mParent.SetupControl(tb)
        End Function

        Public Function SelectButton(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                     OnSelectFunction As String) As CustomControls.Bootstrap.StateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.SelectButton(Of ObjectType)(ListProperty, OnSelectFunction)
          Return mParent.SetupControl(tb)
        End Function

        Public Function SelectButton(ListProperty As String,
                                     OnSelectFunction As String) As CustomControls.Bootstrap.StateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.SelectButton(Of ObjectType)(ListProperty, OnSelectFunction)
          Return mParent.SetupControl(tb)
        End Function

        Public Function DateTimeEditor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                       Optional EditorWidth As Integer? = Nothing,
                                       Optional DateEditorWidth As Integer? = Nothing,
                                       Optional TimeEditorWidth As Integer? = Nothing)

          Dim tb As New CustomControls.Bootstrap.DateTimeEditor(Of ObjectType)(le, EditorWidth, DateEditorWidth, TimeEditorWidth)
          Return mParent.SetupControl(tb)

        End Function

        Public Function ProgressBar(Percentage As Decimal, ProgressText As String) As CustomControls.Bootstrap.ProgressBar(Of ObjectType)

          Dim tb As New CustomControls.Bootstrap.ProgressBar(Of ObjectType)(Percentage, ProgressText)
          Return mParent.SetupControl(tb)

        End Function

        Public Function ProgressBar(Optional ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                    Optional ProgressText As String = "",
                                    Optional Striped As Boolean = False, Optional Active As Boolean = False,
                                    Optional CustomCssBinding As String = "") As CustomControls.Bootstrap.ProgressBar(Of ObjectType)

          Dim tb As New CustomControls.Bootstrap.ProgressBar(Of ObjectType)(ListProperty, ProgressText, Striped, Active, CustomCssBinding)
          Return mParent.SetupControl(tb)

        End Function

        Public Function Label(Optional LabelText As String = "",
                              Optional LabelStyle As BootstrapEnums.Style = BootstrapEnums.Style.Custom,
                              Optional CustomLabelClass As String = "",
                              Optional IconName As String = "",
                              Optional IconSize As String = "",
                              Optional LabelTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing) As CustomControls.Bootstrap.Label(Of ObjectType)

          Dim tb As New CustomControls.Bootstrap.Label(Of ObjectType)(LabelText, LabelStyle, CustomLabelClass, IconName, IconSize, LabelTextExpression)
          Return mParent.SetupControl(tb)

        End Function

        Public Function InputGroup(Optional InputGroupSize As BootstrapEnums.InputGroupSize = BootstrapEnums.InputGroupSize.Small,
                                   Optional CustomSizeClass As String = "") As HTMLDiv(Of ObjectType)
          Dim tb As New HTMLDiv(Of ObjectType)
          tb.AddClass("input-group")
          If InputGroupSize = BootstrapEnums.InputGroupSize.Small Then
            tb.AddClass("input-group-sm")
          ElseIf InputGroupSize = BootstrapEnums.InputGroupSize.Large Then
            tb.AddClass("input-group-lg")
          ElseIf InputGroupSize = BootstrapEnums.InputGroupSize.Custom Then
            tb.AddClass(CustomSizeClass)
          ElseIf InputGroupSize = BootstrapEnums.InputGroupSize.ExtraSmall Then
            tb.AddClass("input-group-xs")
          End If
          Return mParent.SetupControl(tb)
        End Function

        Public Function InputGroupAddOnText(AddOnText As String) As CustomControls.Bootstrap.InputGroupAddOnText(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.InputGroupAddOnText(Of ObjectType)(AddOnText)
          Return mParent.SetupControl(tb)
        End Function

        Public Function InputGroupAddOnButton(Optional ButtonID As String = "",
                                               Optional ButtonText As String = "",
                                               Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                                               Optional CustomStyleClass As String = "",
                                               Optional ButtonSize As BootstrapEnums.ButtonSize = BootstrapEnums.ButtonSize.ExtraSmall,
                                               Optional CustomSizeClass As String = "",
                                               Optional IconName As String = "",
                                               Optional IconSize As String = "",
                                               Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                                               Optional ClickBinding As String = "") As CustomControls.Bootstrap.InputGroupAddOnButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.InputGroupAddOnButton(Of ObjectType)(ButtonID, ButtonText, ButtonStyle,
                                                                                      CustomStyleClass, ButtonSize, CustomSizeClass,
                                                                                      IconName, IconSize, PostBackType, ClickBinding)
          Return mParent.SetupControl(tb)
        End Function

        Public Function InputGroupAddOnStateButton(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                   Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                                                   Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                                                   Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-times",
                                                   Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.InputGroupAddOnStateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.InputGroupAddOnStateButton(Of ObjectType)(ListProperty,
                                                                                           TrueText, FalseText,
                                                                                           TrueCss, FalseCss,
                                                                                           TrueIconName, FalseIconName, ButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function InputGroupAddOnStateButton(ListProperty As String,
                                                  Optional TrueText As String = "Yes", Optional FalseText As String = "No",
                                                  Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default",
                                                  Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-times",
                                                  Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.InputGroupAddOnStateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.InputGroupAddOnStateButton(Of ObjectType)(ListProperty,
                                                                                           TrueText, FalseText,
                                                                                           TrueCss, FalseCss,
                                                                                           TrueIconName, FalseIconName, ButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        ''' <summary>
        ''' Creates an editable table that will be bound on the client.
        ''' </summary>
        ''' <typeparam name="ChildControlsObjectType">The type of the lists items.</typeparam>
        ''' <param name="le">The property of a list to bind to.</param>
        ''' <param name="AllowAddNew">Allow the user to add items.</param>
        ''' <param name="AllowRemove">Allow the user to remove items.</param>
        Public Function TableFor(Of ChildControlsObjectType)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                             ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                             Optional Striped As Boolean = True,
                                                             Optional Bordered As Boolean = True,
                                                             Optional Hover As Boolean = True,
                                                             Optional Condensed As Boolean = True,
                                                             Optional Responsive As Boolean = True,
                                                             Optional ListNameForSorting As String = "",
                                                             Optional AfterItemAddedFunctionName As String = "",
                                                             Optional AfterItemRemovedFunctionName As String = "") As Bootstrap.Table(Of ObjectType, ChildControlsObjectType)

          Dim koTbl As New Bootstrap.Table(Of ObjectType, ChildControlsObjectType)
          koTbl.AllowAddNew = AllowAddNew
          koTbl.AllowRemove = AllowRemove
          koTbl.Striped = Striped
          koTbl.Bordered = Bordered
          koTbl.Hover = Hover
          koTbl.Condensed = Condensed
          koTbl.Responsive = Responsive
          koTbl.ListSortName = ListNameForSorting
          koTbl.AfterItemAddedFunctionName = AfterItemAddedFunctionName
          koTbl.AfterItemRemovedFunctionName = AfterItemRemovedFunctionName
          mParent.SetupControl(koTbl, le)
          Return koTbl

        End Function

        Public Function TableFor(Of ChildControlsObjectType)(ByVal pi As PropertyInfo,
                                                             ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                             Optional Striped As Boolean = True,
                                                             Optional Bordered As Boolean = True,
                                                             Optional Hover As Boolean = True,
                                                             Optional Condensed As Boolean = True,
                                                             Optional Responsive As Boolean = True,
                                                             Optional ListNameForSorting As String = "",
                                                             Optional AfterItemAddedFunctionName As String = "",
                                                             Optional AfterItemRemovedFunctionName As String = "") As Bootstrap.Table(Of ObjectType, ChildControlsObjectType)

          Dim koTbl As New Bootstrap.Table(Of ObjectType, ChildControlsObjectType)
          koTbl.AllowAddNew = AllowAddNew
          koTbl.AllowRemove = AllowRemove
          koTbl.Striped = Striped
          koTbl.Bordered = Bordered
          koTbl.Hover = Hover
          koTbl.Condensed = Condensed
          koTbl.Responsive = Responsive
          koTbl.ListSortName = ListNameForSorting
          koTbl.AfterItemAddedFunctionName = AfterItemAddedFunctionName
          koTbl.AfterItemRemovedFunctionName = AfterItemRemovedFunctionName
          mParent.SetupControl(koTbl, pi)
          Return koTbl

        End Function

        Public Function TableFor(Of ChildControlsObjectType)(DatasourceJS As String,
                                                             ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                             Optional Striped As Boolean = True,
                                                             Optional Bordered As Boolean = True,
                                                             Optional Hover As Boolean = True,
                                                             Optional Condensed As Boolean = True,
                                                             Optional Responsive As Boolean = True,
                                                             Optional ListNameForSorting As String = "",
                                                             Optional AfterItemAddedFunctionName As String = "",
                                                             Optional AfterItemRemovedFunctionName As String = "") As Bootstrap.Table(Of ObjectType, ChildControlsObjectType)

          Dim koTbl As New Bootstrap.Table(Of ObjectType, ChildControlsObjectType)
          koTbl.DataSourceString = DatasourceJS
          koTbl.AllowAddNew = AllowAddNew
          koTbl.AllowRemove = AllowRemove
          koTbl.Striped = Striped
          koTbl.Bordered = Bordered
          koTbl.Hover = Hover
          koTbl.Condensed = Condensed
          koTbl.Responsive = Responsive
          koTbl.ListSortName = ListNameForSorting
          koTbl.AfterItemAddedFunctionName = AfterItemAddedFunctionName
          koTbl.AfterItemRemovedFunctionName = AfterItemRemovedFunctionName
          mParent.SetupControl(koTbl)
          Return koTbl

        End Function

        Public Function ContainerFluid() As CustomControls.HTMLDiv(Of ObjectType)
          Dim tb As New CustomControls.HTMLDiv(Of ObjectType)
          tb.AddClass("container-fluid")
          Return mParent.SetupControl(tb)
        End Function

        Public Function Navbar(NavbarStyle As BootstrapEnums.NavbarStyle,
                               NavbarID As String,
                               IncludeBrand As Boolean,
                               ExcludeToggle As Boolean,
                               Optional BrandName As String = "") As CustomControls.Bootstrap.Navbar(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.Navbar(Of ObjectType)(NavbarStyle, NavbarID, IncludeBrand, ExcludeToggle, BrandName)
          Return mParent.SetupControl(tb)
        End Function

        Public Function UnorderedList(CssClass As String) As CustomControls.Bootstrap.UnorderedList(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.UnorderedList(Of ObjectType)(CssClass)
          Return mParent.SetupControl(tb)
        End Function

        Public Function ListItem(CssClass As String) As CustomControls.Bootstrap.ListItem(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.ListItem(Of ObjectType)(CssClass)
          Return mParent.SetupControl(tb)
        End Function

        Public Function ButtonDropDown(Optional IconName As String = "",
                                       Optional ActionButtonColorClass As String = "btn-default",
                                       Optional DropDownButtonColorClass As String = "btn-default",
                                       Optional ActionTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                       Optional le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                       Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                                       Optional ActionButtonClickBinding As String = "",
                                       Optional ButtonID As String = "") As CustomControls.Bootstrap.ButtonDropDown(Of ObjectType)

          Dim BBD As New CustomControls.Bootstrap.ButtonDropDown(Of ObjectType)(IconName, ActionButtonColorClass, DropDownButtonColorClass, ActionTextExpression, PostBackType, ActionButtonClickBinding, ButtonID)
          Return mParent.SetupControl(BBD, le)

        End Function

        Public Function Anchor(Optional HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                               Optional LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                               Optional HrefString As String = "",
                               Optional LinkTextString As String = "",
                               Optional Target As Singular.Web.LinkTargetType = LinkTargetType.NotSet,
                               Optional IconName As String = "",
                               Optional IconSize As String = "") As CustomControls.Bootstrap.Anchor(Of ObjectType)
          Dim Link As New CustomControls.Bootstrap.Anchor(Of ObjectType)
          Link.HRefExpression = HRefExpression
          Link.LinkTextExpression = LinkTextExpression
          Link.Href = HrefString
          Link.LinkText = LinkTextString
          Link.Target = Target
          Link.IconName = IconName
          Link.IconSize = IconSize
          Return mParent.SetupControl(Link)
        End Function

        Public Function Pager() As CustomControls.Bootstrap.Pager(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.Pager(Of ObjectType)()
          Return mParent.SetupControl(tb)
        End Function

        Public Function PagerListItem(Optional CssClass As String = "",
                                      Optional AnchorHRef As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                      Optional AnchorLinkText As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                      Optional AnchorHrefString As String = "",
                                      Optional AnchorLinkTextString As String = "",
                                      Optional AnchorTarget As Singular.Web.LinkTargetType = LinkTargetType.NotSet) As CustomControls.Bootstrap.PagerListItem(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.PagerListItem(Of ObjectType)(CssClass, AnchorHRef, AnchorLinkText, AnchorHrefString, AnchorLinkTextString, AnchorTarget)
          Return mParent.SetupControl(tb)
        End Function

        Public Function Nav(Optional ID As String = "", Optional NavCssClasses As String = "") As CustomControls.Bootstrap.Nav(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.Nav(Of ObjectType)(ID, NavCssClasses)
          Return mParent.SetupControl(tb)
        End Function

        Public Function PagedGridFor(Of ChildControlsObjectType)(ByVal PagingManager As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                                 ByVal TableList As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                                 ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                                 Optional Striped As Boolean = True,
                                                                 Optional Bordered As Boolean = True,
                                                                 Optional Hover As Boolean = True,
                                                                 Optional Condensed As Boolean = True,
                                                                 Optional Responsive As Boolean = True,
                                                                 Optional NavID As String = "",
                                                                 Optional PagerPosition As BootstrapEnums.PagerPosition = BootstrapEnums.PagerPosition.Bottom,
                                                                 Optional OnRowSelect As String = "",
                                                                 Optional HidePager As Boolean = False) As Bootstrap.PagedGrid(Of ObjectType, ChildControlsObjectType)

          Dim koTbl As New Bootstrap.PagedGrid(Of ObjectType, ChildControlsObjectType)
          koTbl.AllowAddNew = AllowAddNew
          koTbl.AllowRemove = AllowRemove
          koTbl.PagingManager = PagingManager
          koTbl.Striped = Striped
          koTbl.Bordered = Bordered
          koTbl.Hover = Hover
          koTbl.Condensed = Condensed
          koTbl.Responsive = Responsive
          koTbl.NavID = NavID
          koTbl.PagerPosition = PagerPosition
          koTbl.OnRowSelect = OnRowSelect
          koTbl.HidePager = HidePager
          mParent.SetupControl(koTbl, TableList)
          Return koTbl

        End Function

        Public Function PagedGridFor(Of ChildControlsObjectType)(ByVal PagingManagerPropertyJS As String,
                                                                 ByVal TableListPropertyJS As String,
                                                                 ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                                 Optional Striped As Boolean = True,
                                                                 Optional Bordered As Boolean = True,
                                                                 Optional Hover As Boolean = True,
                                                                 Optional Condensed As Boolean = True,
                                                                 Optional Responsive As Boolean = True,
                                                                 Optional NavID As String = "",
                                                                 Optional PagerPosition As BootstrapEnums.PagerPosition = BootstrapEnums.PagerPosition.Bottom,
                                                                 Optional OnRowSelect As String = "",
                                                                 Optional HidePager As Boolean = False,
                                                                 Optional IgnoreRoot As Boolean = False,
                                                                 Optional ContainerCssClass As String = "") As Bootstrap.PagedGrid(Of ObjectType, ChildControlsObjectType)

          Dim koTbl As New Bootstrap.PagedGrid(Of ObjectType, ChildControlsObjectType)
          koTbl.AllowAddNew = AllowAddNew
          koTbl.AllowRemove = AllowRemove
          'koTbl.PagingManager = PagingManager
          koTbl.PagingManagerJSProperty = PagingManagerPropertyJS
          koTbl.Striped = Striped
          koTbl.Bordered = Bordered
          koTbl.Hover = Hover
          koTbl.Condensed = Condensed
          koTbl.Responsive = Responsive
          koTbl.NavID = NavID
          koTbl.PagerPosition = PagerPosition
          koTbl.OnRowSelect = OnRowSelect
          koTbl.HidePager = HidePager
          koTbl.DataSourceString = TableListPropertyJS
          koTbl.IgnoreRoot = IgnoreRoot
          koTbl.ContainerCssClass = ContainerCssClass
          mParent.SetupControl(koTbl)
          Return koTbl

        End Function

        'Public Function EditablePagedGridFor(Of ChildControlsObjectType)(ByVal EditablePagingManager As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
        '                                                                ByVal TableList As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
        '                                                                ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
        '                                                                Optional Striped As Boolean = True,
        '                                                                Optional Bordered As Boolean = True,
        '                                                                Optional Hover As Boolean = True,
        '                                                                Optional Condensed As Boolean = True,
        '                                                                Optional Responsive As Boolean = True,
        '                                                                Optional NavID As String = "",
        '                                                                Optional PagerPosition As BootstrapEnums.PagerPosition = BootstrapEnums.PagerPosition.Bottom) As Bootstrap.EditablePagedGrid(Of ObjectType, ChildControlsObjectType)

        '  Dim koTbl As New Bootstrap.EditablePagedGrid(Of ObjectType, ChildControlsObjectType)
        '  koTbl.AllowAddNew = AllowAddNew
        '  koTbl.AllowRemove = AllowRemove
        '  koTbl.EditablePagingManager = EditablePagingManager
        '  koTbl.Striped = Striped
        '  koTbl.Bordered = Bordered
        '  koTbl.Hover = Hover
        '  koTbl.Condensed = Condensed
        '  koTbl.Responsive = Responsive
        '  koTbl.NavID = NavID
        '  koTbl.PagerPosition = PagerPosition
        '  mParent.SetupControl(koTbl, TableList)
        '  Return koTbl

        'End Function

        Public Function FlatDreamAlert(ErrorText As String, AlertColor As BootstrapEnums.FlatDreamAlertColor,
                                       Optional IconName As String = "fa-times-circle", Optional AlertTitleText As String = "",
                                       Optional Bordered As Boolean = False,
                                       Optional White As Boolean = True) As CustomControls.Bootstrap.FlatDreamAlert(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.FlatDreamAlert(Of ObjectType)(ErrorText, AlertColor, IconName, AlertTitleText, Bordered, White)
          Return mParent.SetupControl(tb)
        End Function

        Public Function CustomErrorBox(ErrorText As String, AlertColor As BootstrapEnums.FlatDreamAlertColor,
                                       Optional IconName As String = "fa-times-circle", Optional AlertTitleText As String = "",
                                       Optional Bordered As Boolean = False,
                                       Optional White As Boolean = True) As CustomControls.Bootstrap.CustomErrorBox(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.CustomErrorBox(Of ObjectType)(ErrorText, AlertColor, IconName, AlertTitleText, Bordered, White)
          Return mParent.SetupControl(tb)
        End Function

        Public Function StaticDetailTile(IconName As String,
                                           Optional HeaderText As String = "",
                                           Optional HeaderTextBinding As String = "",
                                           Optional HeaderSubText As String = "",
                                           Optional HeaderSubTextBinding As String = "",
                                           Optional Detail As Boolean = True,
                                           Optional Clean As Boolean = True,
                                           Optional FDTileColor As BootstrapEnums.FDTileColor = BootstrapEnums.FDTileColor.Purple,
                                           Optional ViewDetailsClickBinding As String = "") As CustomControls.Bootstrap.StaticDetailTile(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.StaticDetailTile(Of ObjectType)(IconName,
                                                                                   HeaderText, HeaderTextBinding,
                                                                                   HeaderSubText, HeaderSubTextBinding,
                                                                                   Detail, Clean, FDTileColor,
                                                                                   ViewDetailsClickBinding)
          Return mParent.SetupControl(tb)
        End Function

        Public Function FDTableBlock(Optional HeaderText As String = "") As CustomControls.Bootstrap.FDTableBlock(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.FDTableBlock(Of ObjectType)(HeaderText)
          Return mParent.SetupControl(tb)
        End Function

        Public Function FlatBlock(Optional HeaderText As String = "",
                                  Optional HasContentAbove As Boolean = False,
                                  Optional ExcludeHeader As Boolean = False,
                                  Optional DarkBox As Boolean = False,
                                  Optional ClassOverride As String = "",
                                  Optional IncludeActionsDiv As Boolean = False) As CustomControls.Bootstrap.FlatBlock(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.FlatBlock(Of ObjectType)(HeaderText, HasContentAbove, ExcludeHeader, DarkBox, ClassOverride, IncludeActionsDiv)
          Return mParent.SetupControl(tb)
        End Function

        Public Function InputGroupCombo(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                         Optional EditorPlaceholderText As String = "",
                                         Optional ButtonText As String = "",
                                         Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                                         Optional IconName As String = "fa-sort-down",
                                         Optional InputSize As BootstrapEnums.InputSize = BootstrapEnums.InputSize.Small,
                                         Optional CustomInputSize As String = "",
                                         Optional InputGroupSize As BootstrapEnums.InputGroupSize = BootstrapEnums.InputGroupSize.Small,
                                         Optional CustomInputGroupSize As String = "",
                                         Optional AddOnButtonSize As BootstrapEnums.ButtonSize = BootstrapEnums.ButtonSize.ExtraSmall,
                                         Optional CustomAddOnButtonSize As String = "") As CustomControls.Bootstrap.ComboFor(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.ComboFor(Of ObjectType)(le, EditorPlaceholderText, ButtonText, ButtonStyle, IconName,
                                                                         InputSize, CustomInputSize, InputGroupSize, CustomInputGroupSize,
                                                                         AddOnButtonSize, CustomAddOnButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function ReadOnlyComboFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                         OnClickBinding As String,
                                         Optional EditorPlaceholderText As String = "",
                                         Optional ButtonText As String = "",
                                         Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                                         Optional IconName As String = "fa-sort-down",
                                         Optional UseInputControl As Boolean = False,
                                         Optional ClearFunction As String = "",
                                         Optional InputGroupWidthPercentage As String = "",
                                         Optional InputGroupSize As BootstrapEnums.InputGroupSize = BootstrapEnums.InputGroupSize.Small,
                                         Optional InputSize As BootstrapEnums.InputSize = BootstrapEnums.InputSize.Small,
                                         Optional IncludeClearButton As Boolean = True) As CustomControls.Bootstrap.ReadOnlyComboFor(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.ReadOnlyComboFor(Of ObjectType)(le, OnClickBinding, EditorPlaceholderText, ButtonText, ButtonStyle,
                                                                                 IconName, UseInputControl, ClearFunction, InputGroupWidthPercentage,
                                                                                 InputGroupSize, InputSize, IncludeClearButton)
          Return mParent.SetupControl(tb)
        End Function

        Public Function FormControlFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                       Optional Size As BootstrapEnums.InputSize = BootstrapEnums.InputSize.Small,
                                       Optional CustomSize As String = "",
                                       Optional PlaceHolderText As String = "") As CustomControls.Bootstrap.FormControlFor(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.FormControlFor(Of ObjectType)(le, Size, CustomSize, PlaceHolderText)
          Return mParent.SetupControl(tb)
        End Function

        'Public Function FormControlFor(PropertyJS As String,
        '                               Size As BootstrapEnums.InputSize,
        '                               Optional CustomSize As String = "",
        '                               Optional PlaceHolderText As String = "") As CustomControls.Bootstrap.FormControlFor(Of ObjectType)
        '  Dim tb As New CustomControls.Bootstrap.FormControlFor(Of ObjectType)(PropertyJS, Size, CustomSize, PlaceHolderText)
        '  Return mParent.SetupControl(tb)
        'End Function

        Public Function ReadOnlyFormControlFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                               Size As BootstrapEnums.InputSize,
                                               Optional CustomSize As String = "",
                                               Optional PlaceholderText As String = "") As CustomControls.Bootstrap.ReadOnlyFormControlFor(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.ReadOnlyFormControlFor(Of ObjectType)(le, Size, CustomSize, PlaceholderText)
          Return mParent.SetupControl(tb)
        End Function

        Public Function ReadOnlyFindFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                         OnClickBinding As String,
                                         Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                                         Optional IconName As String = "fa-search",
                                         Optional PlaceholderText As String = "") As CustomControls.Bootstrap.ReadOnlyFindFor(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.ReadOnlyFindFor(Of ObjectType)(le, OnClickBinding, ButtonStyle, IconName, PlaceholderText)
          Return mParent.SetupControl(tb)
        End Function

        Public Function WizardControl(WizardID As String, OnPrevClick As String, OnNextClick As String) As CustomControls.Bootstrap.WizardControl(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.WizardControl(Of ObjectType)(WizardID, OnPrevClick, OnNextClick)
          Return mParent.SetupControl(tb)
        End Function

        Public Function HorizontalForm(Optional DashedBorder As Boolean = False) As CustomControls.HTMLDiv(Of ObjectType)
          Dim tb As New CustomControls.HTMLDiv(Of ObjectType)
          tb.AddClass("form-horizontal")
          If DashedBorder Then
            tb.AddClass("group-border-dashed")
          End If
          Return mParent.SetupControl(tb)
        End Function

        Public Function LabelDisplay(LabelText As String) As CustomControls.Bootstrap.LabelDisplay(Of ObjectType)
          Dim ctrl As New CustomControls.Bootstrap.LabelDisplay(Of ObjectType)(LabelText)
          Return mParent.SetupControl(ctrl)
        End Function

        Public Function TimeEditorFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                       Size As BootstrapEnums.InputSize,
                                       Optional CustomSize As String = "",
                                       Optional PlaceholderText As String = "") As TimeEditor(Of ObjectType)
          Dim tc As TimeEditor(Of ObjectType) = mParent.SetupControl(New TimeEditor(Of ObjectType), le)
          Dim isize As String = Singular.Web.CustomControls.Bootstrap.GetInputSizeClass(Size, CustomSize)
          tc.AddClass("form-control " & isize)
          If PlaceholderText <> "" Then
            tc.Attributes("placeholder") = PlaceholderText
          End If
          Return tc
        End Function

        Public Function ButtonGroup() As HTMLDiv(Of ObjectType)
          'col-sm-12 col-md-2 col-lg-2
          Dim d As New HTMLDiv(Of ObjectType)
          d.AddClass("btn-group")
          d.Attributes("role") = "group"
          Return mParent.SetupControl(d)
        End Function

        Public Function DateRangeEditor(StartDateExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                        EndDateExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                        Optional ApplyOnMenuSelect As Boolean = True,
                                        Optional OnCloseJSFunction As String = "",
                                        Optional AfterRangeChangedJSFunction As String = "",
                                        Optional InitialText As String = "Select Date Range...",
                                        Optional BootstrapButtonCss As String = "",
                                        Optional IconName As String = "") As DateRange(Of ObjectType)
          Dim d As New DateRange(Of ObjectType)(StartDateExpression, EndDateExpression, ApplyOnMenuSelect,
                                                OnCloseJSFunction, AfterRangeChangedJSFunction,
                                                InitialText,
                                                True, BootstrapButtonCss, IconName)
          Return mParent.SetupControl(d)
        End Function

        Public Function LabelFor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As FieldLabel(Of ObjectType)
          Dim d As New FieldLabel(Of ObjectType)()
          d.AddClass("control-label")
          Return mParent.SetupControl(d, le)
        End Function

        Public Function Div(Optional ID As String = "", Optional CssClasses As String = "") As HTMLDiv(Of ObjectType)
          Dim d As New HTMLDiv(Of ObjectType)
          If ID <> "" Then
            d.Attributes("id") = ID
          End If
          d.AddClass(CssClasses)
          Return mParent.SetupControl(d)
        End Function

        Public Function PullLeft() As HTMLDiv(Of ObjectType)
          Dim d As New HTMLDiv(Of ObjectType)
          d.AddClass("pull-left")
          Return mParent.SetupControl(d)
        End Function

        Public Function PullRight() As HTMLDiv(Of ObjectType)
          Dim d As New HTMLDiv(Of ObjectType)
          d.AddClass("pull-right")
          Return mParent.SetupControl(d)
        End Function

        'Public Function DateAndTimeEditor(DateTimeExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As Bootstrap.DateAndTimeEditor(Of ObjectType)
        '  Dim d As New Bootstrap.DateAndTimeEditor(Of ObjectType)(DateTimeExpression)
        '  Return mParent.SetupControl(d)
        'End Function

        Public Function Select2(PropertyExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As Bootstrap.Select2(Of ObjectType)
          Dim d As New Bootstrap.Select2(Of ObjectType)(PropertyExpression)
          Return mParent.SetupControl(d)
        End Function

        Public Function TriStateButton(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                       Optional TrueText As String = "Yes", Optional FalseText As String = "No", Optional NullText As String = "Neither",
                                       Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default", Optional NullCss As String = "btn-info",
                                       Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-minus", Optional NullIconName As String = "fa-question",
                                       Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.TriStateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.TriStateButton(Of ObjectType)(ListProperty,
                                                                                TrueText, FalseText, NullText,
                                                                                TrueCss, FalseCss, NullCss,
                                                                                TrueIconName, FalseIconName, NullIconName,
                                                                                ButtonSize)
          Return mParent.SetupControl(tb)
        End Function

        Public Function TriStateButton(ListProperty As String,
                                       Optional TrueText As String = "Yes", Optional FalseText As String = "No", Optional NullText As String = "Neither",
                                       Optional TrueCss As String = "btn-primary", Optional FalseCss As String = "btn-default", Optional NullCss As String = "btn-info",
                                       Optional TrueIconName As String = "fa-check-square-o", Optional FalseIconName As String = "fa-minus", Optional NullIconName As String = "fa-question",
                                       Optional ButtonSize As String = "btn-xs") As CustomControls.Bootstrap.TriStateButton(Of ObjectType)
          Dim tb As New CustomControls.Bootstrap.TriStateButton(Of ObjectType)(ListProperty,
                                                                                TrueText, FalseText, NullText,
                                                                                TrueCss, FalseCss, NullCss,
                                                                                TrueIconName, FalseIconName, NullIconName,
                                                                                ButtonSize)
          Return mParent.SetupControl(tb)
        End Function


      End Class

#End Region

#Region " Graphing "

      Private mChartingAccessors As New ChartingAccessors(Me)
      Public ReadOnly Property Charting As ChartingAccessors
        Get
          Return mChartingAccessors
        End Get
      End Property

      Public Class ChartingAccessors

        Private mParent As HelperAccessors
        Public Sub New(Parent As HelperAccessors)
          mParent = Parent
        End Sub

        Public Function BarChart(Name As String, GraphSettings As Charting.BarChartSettings, Width As Integer, Height As Integer) As BarChartControl(Of ObjectType)
          Dim bgc As New BarChartControl(Of ObjectType)(Name, GraphSettings) With {.Width = Width, .Height = Height}
          Return mParent.SetupControl(bgc)
        End Function

        Public Function PieChart(Name As String, GraphSettings As Charting.PieChartSettings, Width As Integer, Height As Integer) As PieChartControl(Of ObjectType)
          Dim bgc As New PieChartControl(Of ObjectType)(Name, GraphSettings) With {.Width = Width, .Height = Height}
          Return mParent.SetupControl(bgc)
        End Function

        Public Function BarChart(Name As String, GraphSettings As Charting.LineChartSettings, Width As Integer, Height As Integer) As LineChartControl(Of ObjectType)
          Dim bgc As New LineChartControl(Of ObjectType)(Name, GraphSettings) With {.Width = Width, .Height = Height}
          Return mParent.SetupControl(bgc)
        End Function

        Public Function LineChart(Name As String, GraphSettings As Charting.LineChartSettings, Width As Integer, Height As Integer) As LineChartControl(Of ObjectType)
          Dim bgc As New LineChartControl(Of ObjectType)(Name, GraphSettings) With {.Width = Width, .Height = Height}
          Return mParent.SetupControl(bgc)
        End Function

      End Class

#End Region

    End Class

  End Class

End Namespace


