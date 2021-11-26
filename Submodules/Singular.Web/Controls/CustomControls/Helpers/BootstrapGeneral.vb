Imports System.Reflection
Imports System.Dynamic
Imports System.Linq.Expressions
Imports Singular.Web.Controls.HelperControls
Imports System.ComponentModel.DataAnnotations
Imports Singular.Web.Controls
Imports System.ComponentModel

Namespace CustomControls

  Public Class BootstrapToolbar(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mValidationDiv As HTMLDiv(Of ObjectType)
    Private mInformationInd As Boolean
    Private mWarningsInd As Boolean
    Private mLeftAlignInd As Boolean

    Public Sub New(InformationInd As Boolean, WarningsInd As Boolean, LeftAlignInd As Boolean)
      mInformationInd = InformationInd
      mWarningsInd = WarningsInd
      mLeftAlignInd = LeftAlignInd
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AddClass("DivToolbar")

      Writer.WriteBeginTag("ul") '<ul class="nav nav-pills nav-stacked">
      Writer.WriteAttribute("class", "nav nav-pills " & If(mLeftAlignInd, "pull-left", "pull-right"))
      Writer.Write(">")

      If mInformationInd Then
        AddPill(Writer, "Info", "PillInfo")
      End If

      If mWarningsInd Then
        AddPill(Writer, "Warnings", "PillWarnings")
      End If

      AddPill(Writer, "Errors", "PillErrors")

      Writer.WriteEndTag("ul")

      mValidationDiv = Helpers.Div
      mValidationDiv.AddClass("ValidationPopup")
      mValidationDiv.AddClass("Msg-Validation")
      mValidationDiv.Attributes("data-validation-summary") = "2"

    End Sub

    Private Sub AddPill(Writer As HtmlTextWriter, PillText As String, PillID As String)

      Writer.WriteBeginTag("li")
      Writer.Write(">")
      Writer.WriteBeginTag("a")
      Writer.WriteAttribute("href", "#")
      Writer.WriteAttribute("id", PillID)
      Writer.Write(">")
      Writer.WriteBeginTag("span")
      Writer.WriteAttribute("class", "badge pull-right")
      Writer.Write(">")
      Writer.Write("0")
      Writer.WriteEndTag("span")
      Writer.Write(PillText)
      Writer.WriteEndTag("a")
      Writer.WriteEndTag("li")

      'Writer.WriteAttribute("class", "btn btn-info")
      'Writer.WriteAttribute("class", "panel-info")

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      WriteFullStartTag("div", TagType.IndentChildren)

      RenderChildren()

      Writer.WriteEndTag("div", True)

    End Sub

  End Class

  Public Class BootstrapEditorRow(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Protected mLabel As FieldLabel(Of ObjectType)
    Protected mEditor As EditorBase(Of ObjectType)
    Protected mRowContainer As HTMLDiv(Of ObjectType)
    Protected mEditorContainer As HTMLDiv(Of ObjectType)

    Private mRowContainerClasses = "" 'col-lg-9
    Private mEditorContainerClasses = "" 'col-lg-5
    Private mLabelClasses = "" 'pull-left
    Private mLabelAttributes = "" 'style = "text-align: left; width:100px;"
    Private mEditorClasses = "" 'pull-left
    Private mEditorAttributes = "" 'style = "text-align: left; width:100px;"

    Protected Friend Overrides Function IsBindingAllowed(BindingType As KnockoutBindingString) As Boolean
      If Singular.Misc.In(BindingType, KnockoutBindingString.enable, KnockoutBindingString.disable) Then
        Throw New Exception("enabled and disabled binding must be specified on the editor, otherwise it won't work in firefox or chrome.")
      Else
        Return True
      End If
    End Function

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If mLinqExpression Is Nothing Then
        mLabel = Helpers.LabelFor(PropertyInfo)
      Else
        mLabel = Helpers.LabelFor(mLinqExpression)
      End If
      mLabel.AddClass("control-label")
      mLabel.AddClass(mLabelClasses)
      mLabel.Attributes("style") = mLabelAttributes

      If mLinqExpression Is Nothing Then
        mEditor = Helpers.EditorFor(PropertyInfo)
      Else
        mEditor = Helpers.EditorFor(mLinqExpression)
      End If
      mEditor.AddClass("form-control")
      mEditor.AddClass(mEditorClasses)
      mEditor.Attributes("style") = mEditorAttributes

      mRowContainer = Helpers.Div
      mEditorContainer = mRowContainer.Helpers.Div

      mEditorContainer.Controls.Add(mEditor)
      mEditorContainer.AddClass(mEditorContainerClasses)

      mRowContainer.AddClass("form-group")
      mRowContainer.AddClass(mRowContainerClasses)
      mRowContainer.Controls.Add(mLabel)
      mRowContainer.Controls.Add(mEditorContainer)

    End Sub

    Public Sub New(Optional RowContainerClasses As String = "", _
                   Optional EditorContainerClasses As String = "", _
                   Optional LabelClasses As String = "", _
                   Optional LabelAttributes As String = "", _
                   Optional EditorClasses As String = "", _
                   Optional EditorAttributes As String = "")

      mRowContainerClasses = RowContainerClasses
      mEditorContainerClasses = EditorContainerClasses
      mLabelClasses = LabelClasses
      mLabelAttributes = LabelAttributes
      mEditorClasses = EditorClasses
      mEditorAttributes = EditorAttributes

    End Sub

    Public ReadOnly Property Label() As FieldLabel(Of ObjectType)
      Get
        Return mLabel
      End Get
    End Property

    Public ReadOnly Property Editor() As EditorBase(Of ObjectType)
      Get
        Return mEditor
      End Get
    End Property

    Public ReadOnly Property RowContainer() As HTMLDiv(Of ObjectType)
      Get
        Return mRowContainer
      End Get
    End Property

    Public ReadOnly Property EditorContainer() As HTMLDiv(Of ObjectType)
      Get
        Return mEditorContainer
      End Get
    End Property

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        RenderChildren()

      End With

    End Sub

  End Class

  Public Class BootstrapDialog(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mModalDiv As HTMLDiv(Of ObjectType)
    Private mModalDialogDiv As HTMLDiv(Of ObjectType)
    Private mModalContentDiv As HTMLDiv(Of ObjectType)
    Private mModalHeaderDiv As HTMLDiv(Of ObjectType)
    Private mModalBodyDiv As HTMLDiv(Of ObjectType)
    Private mModalFooterDiv As HTMLDiv(Of ObjectType)
    Private mModalDismissButton As HTMLTag(Of ObjectType)
    Private mModalHeading As HTMLTag(Of ObjectType)
    Private mTitle As String = ""
    Private mID As String = ""

    Public ReadOnly Property Modal As HTMLDiv(Of ObjectType)
      Get
        Return mModalDiv
      End Get
    End Property

    Public ReadOnly Property Header As HTMLDiv(Of ObjectType)
      Get
        Return mModalHeaderDiv
      End Get
    End Property

    Public ReadOnly Property Heading As HTMLTag(Of ObjectType)
      Get
        Return mModalHeading
      End Get
    End Property

    Public ReadOnly Property Body As HTMLDiv(Of ObjectType)
      Get
        Return mModalBodyDiv
      End Get
    End Property

    Public ReadOnly Property ModalDialogDiv As HTMLDiv(Of ObjectType)
      Get
        Return mModalDialogDiv
      End Get
    End Property

    Public ReadOnly Property Footer As HTMLDiv(Of ObjectType)
      Get
        Return mModalFooterDiv
      End Get
    End Property

    Public ReadOnly Property DismissButton As HTMLTag(Of ObjectType)
      Get
        Return mModalDismissButton
      End Get
    End Property

    Public Sub New(ID As String, Title As String)

      'Title As String
      mID = ID
      mTitle = Title

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      mModalDiv = Helpers.Div

      'outermost
      With mModalDiv
        mModalDiv.AddClass("modal fade")
        mModalDiv.Attributes("tabindex") = "-1"
        mModalDiv.Attributes("role") = "dialog"
        mModalDiv.Attributes("aria-hidden") = "true"
        If mID <> "" Then
          mModalDiv.Attributes("id") = mID
        End If

        '
        mModalDialogDiv = mModalDiv.Helpers.Div
        With mModalDialogDiv
          mModalDialogDiv.AddClass("modal-dialog")

          mModalContentDiv = mModalDialogDiv.Helpers.Div
          With mModalContentDiv
            mModalContentDiv.AddClass("modal-content")

            'Header-----------------------------------------
            mModalHeaderDiv = mModalContentDiv.Helpers.Div
            With mModalHeaderDiv
              mModalHeaderDiv.AddClass("modal-header")

              mModalDismissButton = mModalHeaderDiv.Helpers.HTMLTag("button")
              With mModalDismissButton
                .Helpers.HTML("&times;")
                .AddClass("close")
                .Attributes("type") = "button"
                .Attributes("data-dismiss") = "modal"
                .Attributes("aria-hidden") = "true"
              End With

              mModalHeading = mModalHeaderDiv.Helpers.HTMLTag("H4")
              With mModalHeading
                .AddClass("modal-title")
                .Helpers.HTML(mTitle)
              End With

            End With

            'Body-------------------------------------------
            mModalBodyDiv = mModalContentDiv.Helpers.Div
            With mModalBodyDiv
              mModalBodyDiv.AddClass("modal-body")

            End With

            'Footer----------------------------------------
            mModalFooterDiv = mModalContentDiv.Helpers.Div
            With mModalFooterDiv
              mModalFooterDiv.AddClass("modal-footer")

            End With

          End With

        End With

      End With

    End Sub

    Protected Friend Overrides Sub Render()

      MyBase.Render()
      RenderChildren()

    End Sub

  End Class

  'Public Class BootstrapTable(Of ObjectType)
  '  Inherits HelperBase(Of ObjectType)

  '  Private mContainerDiv As HTMLDiv(Of ObjectType)
  '  Private mTable As HTMLTag(Of ObjectType)
  '  Private mTableHeader As HTMLTag(Of ObjectType)
  '  Private mTableBody As HTMLTag(Of ObjectType)
  '  Private mTableFooter As HTMLTag(Of ObjectType)
  '  Private mColumnNames() As String
  '  Private mHeaderClasses As String = ""

  '  Public ReadOnly Property Container As HTMLDiv(Of ObjectType)
  '    Get
  '      Return mContainerDiv
  '    End Get
  '  End Property

  '  Public ReadOnly Property Table As HTMLTag(Of ObjectType)
  '    Get
  '      Return mTable
  '    End Get
  '  End Property

  '  Public ReadOnly Property TableHeader As HTMLTag(Of ObjectType)
  '    Get
  '      Return mTableHeader
  '    End Get
  '  End Property

  '  Public ReadOnly Property TableBody As HTMLTag(Of ObjectType)
  '    Get
  '      Return mTableBody
  '    End Get
  '  End Property

  '  Public ReadOnly Property TableFooter As HTMLTag(Of ObjectType)
  '    Get
  '      Return mTableFooter
  '    End Get
  '  End Property

  '  Public Sub New(ColumnNames As String(), HeaderClasses As String)

  '    mColumnNames = ColumnNames
  '    mHeaderClasses = HeaderClasses

  '  End Sub

  '  Protected Friend Overrides Sub Setup()
  '    MyBase.Setup()

  '    mContainerDiv = Helpers.Div
  '    mContainerDiv.AddClass("table-responsive")

  '    With mContainerDiv
  '      mTable = .Helpers.HTMLTag("table")
  '      .AddClass("table table-striped table-hover")

  '      With mTable
  '        mTableHeader = .Helpers.HTMLTag("thead")

  '        With mTableHeader
  '          With .Helpers.HTMLTag("tr")
  '            mColumnNames.ToList.ForEach(Sub(ColumnHeader)

  '                                          With .Helpers.HTMLTag("th")
  '                                            .Helpers.HTML(ColumnHeader)
  '                                            .AddClass(mHeaderClasses)
  '                                            '.Style.TextAlign = Singular.Web.TextAlign.center
  '                                          End With

  '                                        End Sub)
  '          End With
  '        End With

  '        mTableBody = .Helpers.HTMLTag("tbody")
  '        mTableFooter = .Helpers.HTMLTag("tfoot")

  '      End With

  '    End With

  '  End Sub

  '  Protected Friend Overrides Sub Render()

  '    MyBase.Render()
  '    RenderChildren()

  '  End Sub

  'End Class

  Public Class BootstrapBadge(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    'Private mLink As HTMLTag(Of ObjectType)
    Private mBadge As HTMLTag(Of ObjectType)
    'Private mHref As String = ""
    Private mBadgeName As String = ""
    Private mIconName As String = ""
    Private mHTMLText As String = ""
    Private mGlyphIcon As HTMLTag(Of ObjectType)
    'Private mCssClasses As String = ""
    'Private mClickBinding As String = ""

    'Public ReadOnly Property Link As HTMLTag(Of ObjectType)
    '  Get
    '    Return mLink
    '  End Get
    'End Property

    Public ReadOnly Property Badge As HTMLTag(Of ObjectType)
      Get
        Return mBadge
      End Get
    End Property

    Public ReadOnly Property GlyphIcon As HTMLTag(Of ObjectType)
      Get
        Return mGlyphIcon
      End Get
    End Property

    Public Sub New(BadgeName As String, IconName As String, HTMLText As String)

      'Href As String, 
      ', Optional CssClasses As String = "", Optional OnClickJSFunction As String = ""
      'mCssClasses = CssClasses
      'mHref = Href
      mBadgeName = BadgeName
      mIconName = IconName
      mHTMLText = HTMLText
      'mClickBinding = OnClickJSFunction

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      'With mContainerDiv
      ' mLink = Helpers.HTMLTag("a")

      'With mLink
      '.AddClass(mCssClasses)
      '.Attributes("href") = mHref
      mBadge = Helpers.HTMLTag("span")

      ''Add the click binding if it is specified
      'If mClickBinding.Trim.Length > 0 Then
      '  .AddBinding(KnockoutBindingString.click, mClickBinding)
      'End If

      With mBadge
        If mBadgeName.Trim.Length <> 0 Then
          .AddClass("badge " & mBadgeName)
        End If
        If mIconName.Trim.Length <> 0 Then
          mGlyphIcon = .Helpers.HTMLTag("span")
          With mGlyphIcon
            .AddClass("glyphicon " & mIconName)
          End With
        End If
        .Helpers.HTML(mHTMLText)
      End With

      'End With

      'End With

    End Sub

    Protected Friend Overrides Sub Render()

      MyBase.Render()
      RenderChildren()

    End Sub

  End Class

  Public Class BootstrapMenu
    Inherits System.Web.UI.WebControls.WebControl

    Public Property SiteMapDatasourceID As String
    'Public Property DivClass As String = "navbar"

    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)

      'NAVBAR--------------------------------------------------------------------------
      writer.WriteBeginTag("nav")
      writer.WriteAttribute("class", "navbar navbar-inverse") ' navbar-fixed-top
      writer.WriteAttribute("role", "navigation")
      writer.Write(">")

      'HEADER--------------------------------------------------------------------------
      writer.WriteBeginTag("div")
      writer.WriteAttribute("class", "navbar-header")
      writer.Write(">")

      'button inside header
      writer.WriteBeginTag("button")
      writer.WriteAttribute("type", "button")
      writer.WriteAttribute("class", "navbar-toggle")
      writer.WriteAttribute("data-toggle", "collapse")
      writer.WriteAttribute("data-target", "navbar-ex1-collapse")
      writer.Write(">")

      'spans inside button
      writer.WriteBeginTag("span")
      writer.WriteAttribute("class", "sr-only")
      writer.Write(">")
      writer.Write("Toggle navigation")
      writer.WriteEndTag("span")

      writer.WriteBeginTag("span")
      writer.WriteAttribute("class", "icon-bar")
      writer.Write(">")
      writer.WriteEndTag("span")

      writer.WriteBeginTag("span")
      writer.WriteAttribute("class", "icon-bar")
      writer.Write(">")
      writer.WriteEndTag("span")

      writer.WriteBeginTag("span")
      writer.WriteAttribute("class", "icon-bar")
      writer.Write(">")
      writer.WriteEndTag("span")

      writer.WriteBeginTag("span")
      writer.WriteAttribute("class", "navbar-toggle")
      writer.Write(">")
      writer.WriteEndTag("span")

      writer.WriteEndTag("button")

      writer.WriteBeginTag("a")
      writer.WriteAttribute("class", "navbar-brand")
      writer.WriteAttribute("href", "#")
      writer.Write(">")
      writer.Write("SOBERMS")
      writer.WriteEndTag("a")

      'END HEADER--------------------------------------------------------------------------
      writer.WriteEndTag("div")

      'MENU--------------------------------------------------------------------------------
      writer.WriteBeginTag("div")
      writer.WriteAttribute("class", "collapse navbar-collapse navbar-ex1-collapse")
      writer.Write(">")


      Dim allLinks As SiteMapDataSource = FindControl(SiteMapDatasourceID)
      'LEFT LINKS--------------------------------------------------------------------------
      writer.WriteBeginTag("ul")
      writer.WriteAttribute("class", "nav navbar-nav pull-left")
      writer.Write(">")
      RenderSubMenu(writer, CType(allLinks, Singular.Web.CustomControls.SiteMapDataSource).GetHierarchicalView.Select, True, "Left")
      writer.WriteEndTag("ul")

      'RIGHT LINKS------------------------------------------------------------------------
      writer.WriteBeginTag("ul")
      writer.WriteAttribute("class", "nav navbar-nav pull-right")
      writer.Write(">")
      RenderSubMenu(writer, CType(allLinks, Singular.Web.CustomControls.SiteMapDataSource).GetHierarchicalView.Select, True, "Right")
      writer.WriteEndTag("ul")

      'END MENU---------------------------------------------------------------------------
      writer.WriteEndTag("div")

      'END NAVBAR-------------------------------------------------------------------------
      writer.WriteEndTag("nav")

    End Sub

    Private Sub CreateListItem(writer As System.Web.UI.HtmlTextWriter, Node As SiteMapNode)

      writer.WriteBeginTag("li")
      If Node.ChildNodes.Count > 0 Then
        PopulateAsDropDown(writer, Node)
      Else
        PopulateAsSingleItem(writer, Node)
      End If

      writer.WriteEndTag("li")
      writer.WriteLine()

    End Sub

    Private Sub PopulateAsDropDown(writer As System.Web.UI.HtmlTextWriter, Node As SiteMapNode)

      'close begin tag
      writer.WriteAttribute("class", "dropdown")
      writer.Write(">")

      'clickable link
      writer.WriteBeginTag("a")
      writer.WriteAttribute("href", "#")
      writer.WriteAttribute("class", "dropdown-toggle")
      writer.WriteAttribute("data-toggle", "dropdown")
      writer.Write(">")
      writer.Write(Node.Title)

      'the little down arrow thing
      writer.WriteBeginTag("b")
      writer.WriteAttribute("class", "caret")
      writer.Write(">")
      writer.WriteEndTag("b")

      'close clickable link
      writer.WriteEndTag("a")

      'children
      writer.WriteBeginTag("ul")
      writer.WriteAttribute("class", "dropdown-menu")
      writer.Write(">")
      RenderSubMenu(writer, Node.ChildNodes, False, Nothing)
      writer.WriteEndTag("ul")

    End Sub

    Private Sub PopulateAsSingleItem(writer As System.Web.UI.HtmlTextWriter, Node As SiteMapNode)

      writer.Write(">")
      writer.WriteBeginTag("a")
      If Node.Url = "" Then
        writer.WriteAttribute("href", "#")
      Else
        writer.WriteAttribute("href", Node.Url)
      End If
      writer.Write(">")
      'writer.WriteBeginTag("span")
      'writer.WriteAttribute("class", "glyphicon glyphicon-search")
      'writer.Write(">")
      'writer.WriteEndTag("span")
      writer.WriteBeginTag("span")
      writer.Write(">")
      writer.Write(Node.Title)
      writer.WriteEndTag("span")
      writer.WriteEndTag("a")

    End Sub

    Private Sub RenderSubMenu(writer As System.Web.UI.HtmlTextWriter, RootNode As SiteMapNodeCollection, First As Boolean, CurrentSide As String)

      For Each node As SiteMapNode In RootNode
        If CurrentSide Is Nothing OrElse CurrentSide = GetSide(node) Then
          CreateListItem(writer, node)
          writer.WriteLine()
        End If
      Next
      writer.WriteLine()

    End Sub

    Private Function GetSide(Node As SiteMapNode) As String

      'check attributes for horizonal align
      Dim pi As System.Reflection.PropertyInfo = Node.GetType.GetProperty("Attributes", System.Reflection.BindingFlags.Instance + System.Reflection.BindingFlags.NonPublic)
      Dim prop As System.Collections.Specialized.NameValueCollection = Nothing
      If pi IsNot Nothing Then
        prop = pi.GetValue(Node, Nothing)
      End If

      If prop.Item("Left") = "true" Then
        Return "Left"
      Else
        Return "Right"
      End If

      ''if horizontal align is specified
      'If prop IsNot Nothing AndAlso prop.Item("horizontal-align") IsNot Nothing Then
      '  writer.WriteAttribute("class", prop.Item("horizontal-align"))
      'End If

    End Function

  End Class

  Public Class BootstrapPreviousNext(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    '<ul class="pager">
    '  <li><a href="#">Previous</a></li>
    '  <li><a href="#">Next</a></li>
    '</ul>

    Private mUL As HTMLTag(Of ObjectType)
    Private mPreviousButtonLI As HTMLTag(Of ObjectType)
    Private mPreviousButton As Link(Of ObjectType)
    Private mNextButtonLI As HTMLTag(Of ObjectType)
    Private mNextButton As Link(Of ObjectType)
    Private mCenterContentLI As HTMLTag(Of ObjectType)
    Private mCenterContent As HTMLDiv(Of ObjectType)

    Private mPreviousButtonText As String = "Previous"
    Private mPreviousButtonIcon As String = "&larr;"
    Private mNextButtonText As String = "Next"
    Private mNextButtonIcon As String = "&rarr;"

    Public ReadOnly Property UL As HTMLTag(Of ObjectType)
      Get
        Return mUL
      End Get
    End Property

    Public ReadOnly Property PreviousButton As Link(Of ObjectType)
      Get
        Return mPreviousButton
      End Get
    End Property

    Public ReadOnly Property NextButton As Link(Of ObjectType)
      Get
        Return mNextButton
      End Get
    End Property

    Public ReadOnly Property CenterContent As HTMLDiv(Of ObjectType)
      Get
        Return mCenterContent
      End Get
    End Property

    Public Sub New(Optional PreviousButtonText As String = "", Optional NextButtonText As String = "", Optional PreviousButtonIcon As String = "", Optional NextButtonIcon As String = "")

      If PreviousButtonText.Trim.Length <> 0 Then
        mPreviousButtonText = PreviousButtonText
      End If

      If NextButtonText.Trim.Length <> 0 Then
        mNextButtonText = NextButtonText
      End If

      If PreviousButtonIcon.Trim.Length <> 0 Then
        mPreviousButtonIcon = PreviousButtonIcon
      End If

      If NextButtonIcon.Trim.Length <> 0 Then
        mNextButtonIcon = NextButtonIcon
      End If

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()


      mUL = Helpers.HTMLTag("ul")
      mUL.AddClass("pager")

      With mUL
        mPreviousButtonLI = .Helpers.HTMLTag("li")
        mPreviousButtonLI.AddClass("previous")
        mCenterContentLI = .Helpers.HTMLTag("li")
        mNextButtonLI = .Helpers.HTMLTag("li")
        mNextButtonLI.AddClass("next")
      End With

      With mPreviousButtonLI
        mPreviousButton = .Helpers.LinkFor(, , "#", mPreviousButtonIcon & " " & mPreviousButtonText)
      End With

      With mCenterContentLI
        mCenterContent = .Helpers.Div()
      End With

      With mNextButtonLI
        mNextButton = .Helpers.LinkFor(, , "#", mNextButtonText & " " & mNextButtonIcon)
      End With


      'With mPreviousButton

      'End With

      'With mCenterContent

      'End With

      'With mNextButton

      'End With


      'mLink = Helpers.HTMLTag("a")

      'With mLink
      '  .AddClass(mClasses)
      '  .Attributes("href") = mHref
      '  mBadge = .Helpers.HTMLTag("span")
      '  If mClickBinding.Trim.Length > 0 Then
      '    .AddBinding(KnockoutBindingString.click, mClickBinding)
      '  End If

      '  With mBadge
      '    If mBadgeName.Trim.Length <> 0 Then
      '      .AddClass("badge " & mBadgeName)
      '    End If
      '    If mIconName.Trim.Length <> 0 Then
      '      .AddClass("glyphicon " & mIconName)
      '    End If
      '    .Helpers.HTML(mHTMLText)
      '  End With

      'End With

    End Sub

    Protected Friend Overrides Sub Render()

      MyBase.Render()
      RenderChildren()

    End Sub

  End Class

  Public Class BootstrapButton(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    ''' <summary>
    ''' Indicates what will happen when the button is clicked on the browser.
    ''' If PostBackType = Ajax, then set the Argument, Validate and LoadingText properties
    ''' If PostBackType = Full, then set the Argument and Validate properties
    ''' </summary>
    Public Property mPostBackType As PostBackType = DefaultButtonPostBackType

    Public Property Button As HTMLTag(Of ObjectType)
    Public Property GlyphContainer As BootstrapGlyphIcon(Of ObjectType)
    Public Property ButtonText As HTMLTag(Of ObjectType)
    Private mButtonGroupInd As Boolean = False
    Public Property ButtonGroup As HTMLDiv(Of ObjectType)

    Private mIconName As String = ""
    Private mID As String = ""
    Private mText As String = ""
    Private mCssClass As String = ""
    Private mTooltipToggle As String = "tooltip"
    Private mTooltipPlacement As String = "right"
    Private mTooltipText As String = ""
    Private mClickBinding As String = ""

    Public Property HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    Public Property ButtonTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    Public Property Target As LinkTargetType = LinkTargetType.NotSet

    Public Sub AddPopover(Optional TitleExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                          Optional ContentExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                          Optional Placement As String = "top",
                          Optional Title As String = "",
                          Optional Content As String = "")
      Button.AddClass("Popovers")
      If TitleExpression IsNot Nothing Then
        Button.AddBinding(KnockoutBindingString.attr, TitleExpression)
      Else
        Button.AddBinding(KnockoutBindingString.attr, Title)
      End If
      If ContentExpression IsNot Nothing Then
        Button.AddBinding(KnockoutBindingString.attr, ContentExpression)
      Else
        Button.AddBinding(KnockoutBindingString.attr, Content)
      End If
      Button.Attributes("data-container") = "body"
      Button.Attributes("data-toggle") = "popover"
      Button.Attributes("data-placement") = Placement
    End Sub

    'Public Sub AddPopover(Optional HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
    '                                Optional LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
    '                                Optional HrefString As String = "",
    '                                Optional LinkTextString As String = "",
    '                                Optional Target As Singular.Web.LinkTargetType = LinkTargetType.NotSet,
    '                                Optional Glyphicon As String = "") 'As HTMLTag(Of ObjectType)

    '  With NavItemsContainer
    '    With .Helpers.HTMLTag("li")
    '      With .Helpers.LinkFor(HRefExpression, LinkTextExpression, HrefString, "", Target)
    '        With .Helpers.HTMLTag("span")
    '          .AddClass("glyphicon " & Glyphicon)
    '        End With
    '        .Helpers.HTML(LinkTextString)
    '      End With
    '    End With
    '  End With

    'End Sub

    Public Sub New(Optional ID As String = "",
                   Optional Text As String = "",
                   Optional CssClass As String = "",
                   Optional IconName As String = "",
                   Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                   Optional ButtonGroupInd As Boolean = True,
                   Optional TooltipPlacement As String = "right",
                   Optional TooltipText As String = "",
                   Optional ClickBinding As String = "")
      'data-toggle="tooltip" data-placement="right" title="Tooltip on right"

      mID = ID
      mText = Text
      mCssClass = CssClass
      mIconName = IconName
      mPostBackType = PostBackType
      mButtonGroupInd = ButtonGroupInd
      mTooltipPlacement = TooltipPlacement
      mTooltipText = TooltipText
      mClickBinding = ClickBinding

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If mButtonGroupInd Then
        ButtonGroup = Helpers.DivC("btn-group")
        With ButtonGroup
          Button = .Helpers.HTMLTag("button")
        End With
      Else
        Button = Helpers.HTMLTag("button")
      End If
      Button.AddClass("btn")

      If mID <> "" Then
        Button.Attributes("id") = mID
      End If

      If mTooltipText <> "" Then
        Button.Attributes("data-toggle") = mTooltipToggle
        Button.Attributes("data-placement") = mTooltipPlacement
        Button.Attributes("title") = mTooltipText
      End If

      If mCssClass <> "" Then
        Button.AddClass(mCssClass)
      End If

      If Not Bindings.ContainsKey(Singular.Web.KnockoutBindingString.click) Then
        If mPostBackType = PostBackType.Ajax Then
          Button.AddBinding(KnockoutBindingString.click, "Singular.ButtonClickAjax(arguments[1])")
        End If
      End If

      If mID <> "" And mPostBackType = PostBackType.Full Then
        If Not Bindings.ContainsKey(Singular.Web.KnockoutBindingString.click) Then
          Button.AddBinding(KnockoutBindingString.click, "Singular.ButtonClickPost(arguments[1])")
        End If
        Button.Attributes.Add("type", "submit")
      ElseIf mID = "" And (mPostBackType = PostBackType.None Or mPostBackType = PostBackType.Ajax) And mClickBinding <> "" Then
        Button.Attributes.Add("type", "button")
        Button.AddBinding(KnockoutBindingString.click, mClickBinding)
      Else
        Button.Attributes.Add("type", "button")
      End If

      'If Validate Then
      '  Attributes.Add("data-validate", "PreventPost")
      'End If

      'If PromptText <> "" Then
      '  Attributes.Add("data-prompt", PromptText)
      'End If

      'If LoadingText <> "" Then
      '  Attributes.Add("data-loadText", LoadingText)
      'End If

      With Button
        If mIconName <> "" Then
          .Helpers.BootstrapGlyphIcon(mIconName)
        End If
        ButtonText = .Helpers.HTMLTag("span")
        ButtonText.AddClass("button-text-span")
        If mText <> "" Then
          ButtonText.Helpers.HTML(mText)
          'ElseIf ButtonTextExpression IsNot Nothing Then
          '  mButtonText.AddBinding(KnockoutBindingString.html, ButtonTextExpression)
        End If
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapGlyphIcon(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mGlyphContainer As HTMLTag(Of ObjectType)
    Private mIconName As String = ""

    Public ReadOnly Property GlyphContainer As HTMLTag(Of ObjectType)
      Get
        Return mGlyphContainer
      End Get
    End Property

    Public ReadOnly Property IconName As String
      Get
        Return mIconName
      End Get
    End Property

    Public Sub New(IconName As String)

      mIconName = IconName

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      mGlyphContainer = Helpers.HTMLTag("span")
      mGlyphContainer.AddClass("glyphicon-span ")
      mGlyphContainer.AddClass("glyphicon " & mIconName)

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapThumbnail(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mThumbnailContainer As HTMLDiv(Of ObjectType)
    Private mThumbnailImage As HTMLTag(Of ObjectType)
    Private mThumbnailCaption As HTMLDiv(Of ObjectType)

    Private mThumbnailHeader As HTMLDiv(Of ObjectType)
    Private mThumbnailBody As HTMLDiv(Of ObjectType)
    Private mThumbnailFooter As HTMLDiv(Of ObjectType)

    Private mImagePath As String = ""
    Private mCssClasses As String = ""
    Private mAlternateImagePath As String = ""

    Public ReadOnly Property ThumbnailContainer As HTMLDiv(Of ObjectType)
      Get
        Return mThumbnailContainer
      End Get
    End Property

    Public ReadOnly Property ThumbnailImage As HTMLTag(Of ObjectType)
      Get
        Return mThumbnailImage
      End Get
    End Property

    Public ReadOnly Property ThumbnailCaption As HTMLDiv(Of ObjectType)
      Get
        Return mThumbnailCaption
      End Get
    End Property

    Public ReadOnly Property ThumbnailHeader As HTMLDiv(Of ObjectType)
      Get
        Return mThumbnailHeader
      End Get
    End Property

    Public ReadOnly Property ThumbnailBody As HTMLDiv(Of ObjectType)
      Get
        Return mThumbnailBody
      End Get
    End Property

    Public ReadOnly Property ThumbnailFooter As HTMLDiv(Of ObjectType)
      Get
        Return mThumbnailFooter
      End Get
    End Property

    Public Sub New(ImagePath As String, AlternateImagePath As String, CssClasses As String)

      mImagePath = ImagePath
      mCssClasses = CssClasses
      mAlternateImagePath = AlternateImagePath

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      mThumbnailContainer = Helpers.DivC("thumbnail" & " " & mCssClasses)

      With mThumbnailContainer

        If Not (mImagePath.Trim.Length = 0 And mAlternateImagePath.Trim.Length = 0) Then
          mThumbnailImage = .Helpers.HTMLTag("img")
          mThumbnailImage.Attributes("data-src") = If(mImagePath = "", "holder.js/300x200", mImagePath) 'mImagePath 
          mThumbnailImage.Attributes("alt") = mAlternateImagePath '"..."
        End If

        mThumbnailCaption = .Helpers.DivC("caption")

        With mThumbnailCaption

          mThumbnailHeader = .Helpers.DivC("thumbnail-header")
          mThumbnailBody = .Helpers.DivC("thumbnail-body")
          mThumbnailFooter = .Helpers.DivC("thumbnail-footer")

          'mThumbnailLabel = .Helpers.HTMLTag("h3")
          'If LabelTextExpression IsNot Nothing Then
          '  With mThumbnailLabel
          '    .AddBinding(KnockoutBindingString.html, LabelTextExpression)
          '  End With
          'Else
          '  mThumbnailLabel.Helpers.HTML(mLabelText)
          'End If

          'Other(Content)
          'mThumbnailContent = .Helpers.DivC("thumbnail-content")

        End With

      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapLabel(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    'Private mContainerDiv As HTMLDiv(Of ObjectType)
    Private mLink As HTMLTag(Of ObjectType)
    Private mBadge As HTMLTag(Of ObjectType)
    Private mHref As String = ""
    Private mBadgeName As String = ""
    'Private mContainerClasses As String = ""
    Private mIconName As String = ""
    Private mHTMLText As String = ""
    Private mCssClasses As String = ""
    Private mClickBinding As String = ""

    Public ReadOnly Property Link As HTMLTag(Of ObjectType)
      Get
        Return mLink
      End Get
    End Property

    Public ReadOnly Property Badge As HTMLTag(Of ObjectType)
      Get
        Return mBadge
      End Get
    End Property

    Public Sub New(Href As String, BadgeName As String, IconName As String, Optional HTMLText As String = "", Optional CssClasses As String = "", Optional OnClickJSFunction As String = "")

      'ContainerClasses As String,
      'mContainerClasses = ContainerClasses
      mCssClasses = CssClasses
      mHref = Href
      mBadgeName = BadgeName
      mIconName = IconName
      mHTMLText = HTMLText
      mClickBinding = OnClickJSFunction

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      'mContainerDiv = Helpers.Div
      'mContainerDiv.AddClass(mContainerClasses)

      'With mContainerDiv
      mLink = Helpers.HTMLTag("a")

      With mLink
        .AddClass(mCssClasses)
        .Attributes("href") = mHref
        mBadge = .Helpers.HTMLTag("span")

        'Add the click binding if it is specified
        If mClickBinding.Trim.Length > 0 Then
          .AddBinding(KnockoutBindingString.click, mClickBinding)
        End If

        With mBadge
          If mBadgeName.Trim.Length <> 0 Then
            .AddClass("badge " & mBadgeName)
          End If
          If mIconName.Trim.Length <> 0 Then
            .AddClass("glyphicon " & mIconName)
          End If
          .Helpers.HTML(mHTMLText)
        End With

      End With

      'End With

    End Sub

    Protected Friend Overrides Sub Render()

      MyBase.Render()
      RenderChildren()

    End Sub

  End Class

  Public Class BootstrapNav(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mContainerDiv As HTMLDiv(Of ObjectType)
    Private mContainerClasses As String = ""
    Private mHeadingText As String = ""
    Private mNavItemsContainer As HTMLTag(Of ObjectType)

    Public ReadOnly Property NavItemsContainer As HTMLTag(Of ObjectType)
      Get
        Return mNavItemsContainer
      End Get
    End Property

    Public Sub AddMenuItemLink(Optional HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                    Optional LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                    Optional HrefString As String = "",
                                    Optional LinkTextString As String = "",
                                    Optional Target As Singular.Web.LinkTargetType = LinkTargetType.NotSet,
                                    Optional Glyphicon As String = "") 'As HTMLTag(Of ObjectType)

      With NavItemsContainer
        With .Helpers.HTMLTag("li")
          With .Helpers.LinkFor(HRefExpression, LinkTextExpression, HrefString, "", Target)
            With .Helpers.HTMLTag("span")
              .AddClass("glyphicon " & Glyphicon)
            End With
            .Helpers.HTML(LinkTextString)
          End With
        End With
      End With

    End Sub

    Public Sub AddMenuItemHeader(Optional HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                      Optional LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                      Optional HrefString As String = "",
                                      Optional LinkTextString As String = "",
                                      Optional Target As Singular.Web.LinkTargetType = LinkTargetType.NotSet,
                                      Optional CssClass As String = "") 'As HTMLTag(Of ObjectType)

      With NavItemsContainer
        With .Helpers.HTMLTag("li")
          .AddClass("disabled")
          With .Helpers.LinkFor(HRefExpression, LinkTextExpression, HrefString, "", Target)
            If CssClass <> "" Then
              .AddClass(CssClass)
            End If
            .Helpers.HTML(LinkTextString)
          End With
        End With
      End With

    End Sub

    Public Sub New(ContainerClasses As String, HeadingText As String)

      mContainerClasses = ContainerClasses
      mHeadingText = HeadingText

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      mContainerDiv = Helpers.Div
      mContainerDiv.AddClass(mContainerClasses)

      With mContainerDiv

        'Add the Heading
        If mHeadingText <> "" Then
          .Helpers.HTML.Heading1("Quick Nav")
        End If

        'Create the unordered list which will contain the menu items
        mNavItemsContainer = .Helpers.HTMLTag("ul")
        With mNavItemsContainer
          .AddClass("nav nav-pills nav-stacked")
        End With

      End With

      ''With mContainerDiv
      'mLink = Helpers.HTMLTag("a")

      'With mLink
      '  .AddClass(mCssClasses)
      '  .Attributes("href") = mHref
      '  mBadge = .Helpers.HTMLTag("span")

      '  'Add the click binding if it is specified
      '  If mClickBinding.Trim.Length > 0 Then
      '    .AddBinding(KnockoutBindingString.click, mClickBinding)
      '  End If

      '  With mBadge
      '    If mBadgeName.Trim.Length <> 0 Then
      '      .AddClass("badge " & mBadgeName)
      '    End If
      '    If mIconName.Trim.Length <> 0 Then
      '      .AddClass("glyphicon " & mIconName)
      '    End If
      '    .Helpers.HTML(mHTMLText)
      '  End With

      'End With

      ''End With

    End Sub

    Protected Friend Overrides Sub Render()

      MyBase.Render()
      RenderChildren()

    End Sub

  End Class

  Public Class BootstrapLink(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property Href As String = ""
    Public Property LinkText As String = ""
    Public Property HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    Public Property LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    Public Property Target As LinkTargetType = LinkTargetType.NotSet
    Public Property GlyphiconName As String = ""
    Public Property Link As HTMLTag(Of ObjectType)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      Link = Helpers.HTMLTag("a")

      If HRefExpression IsNot Nothing Then
        Link.AddBinding(KnockoutBindingString.href, HRefExpression)
      End If

      If LinkTextExpression IsNot Nothing Then
        Link.AddBinding(KnockoutBindingString.html, LinkTextExpression)
      End If

      If Href <> "" Then
        If Href.StartsWith("~") Then
          Href = Utils.URL_ToAbsolute(Href)
        End If
        Link.Attributes("href") = Href
      End If

      If Target <> LinkTargetType.NotSet Then
        Link.Attributes("target") = Target.ToString
      End If

      If GlyphiconName <> "" Then
        With Link
          .Helpers.BootstrapGlyphIcon(GlyphiconName)
          .Helpers.HTML(LinkText)
        End With
      Else
        Link.Helpers.HTML(LinkText)
      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      'WriteFullStartTag("a", TagType.Normal)
      'Writer.Write(LinkText)
      RenderChildren()
      'Writer.WriteEndTag("a")
    End Sub

  End Class

  Public Class BootstrapButtonDropDown(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property BtnGroup As HTMLDiv(Of ObjectType)
    Public Property BtnAction As HTMLTag(Of ObjectType)
    Public Property GlyphIcon As BootstrapGlyphIcon(Of ObjectType)
    Public Property ActionButtonText As HTMLTag(Of ObjectType)
    Public Property BtnDropDownToggle As HTMLTag(Of ObjectType)
    Public Property DropDownMenu As HTMLTag(Of ObjectType)
    Public Property ActionButtonColorClass As String = ""
    Public Property DropDownButtonColorClass As String = ""
    Public Property ActionText As String = ""
    Public Property ActionTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    Public Property GlyphIconName As String = ""
    Public Property PostBackType As PostBackType = Singular.Web.PostBackType.None
    Public Property ActionButtonClickBinding As String = ""
    Public Property ButtonID As String = ""

    Public Class BootstrapDropDownAction
      Inherits HelperBase(Of ObjectType)

      Public Property ActionListItem As HTMLTag(Of ObjectType)
      Public Property ActionLink As HTMLTag(Of ObjectType)
      Public Property FontAwesomeIcon As String
      Public Property ActionText As String
      Public Property ClickBinding As String
      Public Property CanClickBinding As String

      Public Shadows ReadOnly Property Parent As BootstrapButtonDropDown(Of ObjectType)
        Get
          Return MyBase.Parent
        End Get
      End Property

      Public Sub New(ActionText As String, Optional FontAwesomeIcon As String = "", Optional ClickBinding As String = "", Optional CanClickBinding As String = "")
        Me.ActionText = ActionText
        Me.FontAwesomeIcon = FontAwesomeIcon
        Me.ClickBinding = ClickBinding
        Me.CanClickBinding = CanClickBinding
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        With Parent.DropDownMenu
          ActionListItem = .Helpers.HTMLTag("li")
          With ActionListItem
            ActionLink = .Helpers.HTMLTag("a")
            With ActionLink
              If CanClickBinding <> "" Then
                .AddBinding(KnockoutBindingString.enable, CanClickBinding)
              End If
              If ClickBinding <> "" Then
                .AddBinding(KnockoutBindingString.click, ClickBinding)
              End If
              If FontAwesomeIcon <> "" Then
                With .Helpers.HTMLTag("i")
                  .AddClass("fa " & FontAwesomeIcon)
                End With
              End If
              With .Helpers.HTMLTag("span")
                .Helpers.HTML(ActionText)
              End With
            End With
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Function AddAction(ActionText As String, Optional FontAwesomeIcon As String = "", Optional ClickBinding As String = "", Optional CanClickBinding As String = "") As BootstrapDropDownAction

      Dim t As New BootstrapDropDownAction(ActionText, FontAwesomeIcon, ClickBinding, CanClickBinding)
      Return AddControl(t)

    End Function

    Public Sub New(Optional GlyphIconName As String = "",
                   Optional ActionButtonColorClass As String = "btn-default",
                   Optional DropDownButtonColorClass As String = "btn-default",
                   Optional ActionTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                   Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                   Optional ActionButtonClickBinding As String = "",
                   Optional ButtonID As String = "")
      Me.GlyphIconName = GlyphIconName
      Me.ActionButtonColorClass = ActionButtonColorClass
      Me.DropDownButtonColorClass = DropDownButtonColorClass
      Me.ActionTextExpression = ActionTextExpression
      Me.PostBackType = PostBackType
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      BtnGroup = Helpers.DivC("btn-group")

      With BtnGroup
        'Action Button-----------------------------
        BtnAction = .Helpers.HTMLTag("button")
        With BtnAction
          .Attributes("class") = "btn " & ActionButtonColorClass
          If GlyphIconName <> "" Then
            GlyphIcon = .Helpers.BootstrapGlyphIcon(GlyphIconName)
          End If
          ActionButtonText = .Helpers.HTMLTag("span")
          If ActionTextExpression IsNot Nothing Then
            With ActionButtonText
              .AddBinding(KnockoutBindingString.text, ActionTextExpression)
            End With
          End If
        End With

        If Not Bindings.ContainsKey(Singular.Web.KnockoutBindingString.click) Then
          If PostBackType = PostBackType.Ajax Then
            BtnAction.AddBinding(KnockoutBindingString.click, "Singular.ButtonClickAjax(arguments[1])")
          End If
        End If

        If ButtonID <> "" And PostBackType = PostBackType.Full Then
          If Not Bindings.ContainsKey(Singular.Web.KnockoutBindingString.click) Then
            BtnAction.AddBinding(KnockoutBindingString.click, "Singular.ButtonClickPost(arguments[1])")
          End If
          BtnAction.Attributes.Add("type", "submit")
        ElseIf ButtonID = "" And (PostBackType = PostBackType.None Or PostBackType = PostBackType.Ajax) And ActionButtonClickBinding <> "" Then
          BtnAction.Attributes.Add("type", "button")
          BtnDropDownToggle.AddBinding(KnockoutBindingString.click, ActionButtonClickBinding)
        Else
          BtnAction.Attributes.Add("type", "button")
        End If

        'Dropdown Button--------------------------
        BtnDropDownToggle = .Helpers.HTMLTag("button")
        With BtnDropDownToggle
          .Attributes("class") = "btn " & DropDownButtonColorClass & " dropdown-toggle"
          .Attributes("data-toggle") = "dropdown"

          With .Helpers.HTMLTag("span")
            .AddClass("caret")
          End With

          With .Helpers.HTMLTag("span")
            .AddClass("sr-only")
            .Helpers.HTML("Toggle Dropdown")
          End With

        End With
        'Menu Actions
        DropDownMenu = .Helpers.HTMLTag("ul")
        With DropDownMenu
          .AddClass("dropdown-menu")
          .Attributes("role") = "menu"
        End With
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapStateButton(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    'Properties
    Private mID As String = ""
    Public Property ButtonGroupInd As Boolean = False
    Public Property OnClickFunction As String = ""
    Public Property ButtonCssFunction As String = ""
    Public Property ButtonInnerHtmlFunction As String = ""

    'Controls
    Public Property Button As HTMLTag(Of ObjectType)
    Public Property GlyphContainer As BootstrapGlyphIcon(Of ObjectType)
    Public Property ButtonGroup As HTMLDiv(Of ObjectType)
    Public Property ButtonText As HTMLTag(Of ObjectType)

    Public Sub New(ID As String,
                   OnClickFunction As String,
                   ButtonCssFunction As String,
                   ButtonInnerHtmlFunction As String,
                   ButtonGroupInd As Boolean)

      mID = ID
      Me.ButtonGroupInd = ButtonGroupInd
      Me.OnClickFunction = OnClickFunction
      Me.ButtonCssFunction = ButtonCssFunction
      Me.ButtonInnerHtmlFunction = ButtonInnerHtmlFunction

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If ButtonGroupInd Then
        ButtonGroup = Helpers.DivC("btn-group")
        With ButtonGroup
          Button = .Helpers.HTMLTag("button")
        End With
      Else
        Button = Helpers.HTMLTag("button")
      End If
      Button.AddClass("btn")
      If mID <> "" Then
        Button.Attributes("id") = mID
      End If
      Button.Attributes.Add("type", "button")
      Button.AddBinding(KnockoutBindingString.click, OnClickFunction)
      Button.AddBinding(KnockoutBindingString.css, ButtonCssFunction)
      Button.AddBinding(KnockoutBindingString.html, ButtonInnerHtmlFunction)

      With Button
        .AddClass("StateButton")
        '.Helpers.BootstrapGlyphIcon("").AddClass("StateButtonIcon")
        '.Helpers.HTMLTag("span").AddClass("StateButtonText")
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapInputGroup(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    '<div class="input-group">
    '  <input type="text" class="form-control">
    '  <span class="input-group-addon">.00</span>
    '</div>

    'Properties
    Private mPropertyEditorForExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing
    Public Property PreInput As Boolean = False
    Private mPreInputEditorForExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing
    Public Property PostInput As Boolean = False
    Private mPostInputEditorForExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing

    'Controls
    Public Property InputGroup As HTMLDiv(Of ObjectType)
    Public Property PreInputAddOn As HTMLTag(Of ObjectType)
    Public Property PreInputEditor As EditorBase(Of ObjectType)
    Public Property PrimaryEditor As EditorBase(Of ObjectType)
    Public Property PostInputAddOn As HTMLTag(Of ObjectType)
    Public Property PostInputEditor As EditorBase(Of ObjectType)

    Public Sub New(Optional PropertyExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                   Optional PreInput As Boolean = False,
                   Optional PreInputExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                   Optional PostInput As Boolean = False,
                   Optional PostInputExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing)

      Me.mPropertyEditorForExpression = PropertyExpression
      Me.PreInput = PreInput
      Me.mPreInputEditorForExpression = PreInputExpression
      Me.PostInput = PostInput
      Me.mPostInputEditorForExpression = PostInputExpression

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      InputGroup = Helpers.Div

      With InputGroup
        .AddClass("input-group")
        If PreInput Then
          PreInputAddOn = .Helpers.HTMLTag("span")
          PreInputAddOn.AddClass("input-group-addon")
          If mPreInputEditorForExpression IsNot Nothing Then
            With PreInputAddOn
              PreInputEditor = .Helpers.EditorFor(mPreInputEditorForExpression)
            End With
          End If
        End If
        'add input here
        If mPropertyEditorForExpression IsNot Nothing Then
          PrimaryEditor = .Helpers.EditorFor(mPropertyEditorForExpression)
          With PrimaryEditor
            .AddClass("form-control")
          End With
        End If
        If PostInput Then
          PostInputAddOn = .Helpers.HTMLTag("span")
          PostInputAddOn.AddClass("input-group-addon")
          If mPostInputEditorForExpression IsNot Nothing Then
            With PostInputAddOn
              PostInputEditor = .Helpers.EditorFor(mPostInputEditorForExpression)
            End With
          End If
        End If
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapPanel(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    'Properties
    Public Property Panel As HTMLDiv(Of ObjectType)
    Public Property PanelHeading As HTMLDiv(Of ObjectType)
    Public Property PanelBody As HTMLDiv(Of ObjectType)
    'Public Property PanelFooter As HTMLDiv(Of ObjectType)
    Public Property PanelColorClass As String = ""
    Public Property CollapseLink As HTMLTag(Of ObjectType)
    Public Property ExpandedContainer As HTMLDiv(Of ObjectType)

    Private mAccordion As Boolean = False
    Private mAccordionID As String = ""
    Private mPanelGroupID As String = ""

    Public Sub New(Optional PanelColorClass As String = "panel-primary", Optional Accordion As Boolean = False, Optional AccordionID As String = "", Optional PanelGroupID As String = "")
      Me.PanelColorClass = PanelColorClass
      mAccordion = Accordion
      mAccordionID = AccordionID
      mPanelGroupID = PanelGroupID
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()
      '<a data-toggle="collapse" data-parent="#accordion" href="#collapseOne">
      '   Collapsible Group Item #1
      '</a>
      Panel = Helpers.DivC("panel " & PanelColorClass)
      With Panel
        PanelHeading = .Helpers.DivC("panel-heading")
        If mAccordion Then
          CollapseLink = PanelHeading.Helpers.HTMLTag("a")
          CollapseLink.Attributes("data-toggle") = "collapse"
          CollapseLink.Attributes("data-parent") = "#" + mPanelGroupID
          CollapseLink.Attributes("href") = "#" + mAccordionID
        End If
        If mAccordion Then
          ExpandedContainer = .Helpers.DivC("panel-collapse collapse in")
          .Attributes("id") = mAccordionID
        Else
          PanelBody = .Helpers.DivC("panel-body")
        End If
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapPageHeader(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    'With .Helpers.DivC("page-header")
    '      With .Helpers.HTMLTag("h1")
    '        .Helpers.HTML("Monthly HR Invoicing")
    '        .Helpers.HTML("<small>enter search criteria</small>")
    '      End With
    '    End With

    Public Property PageHeaderDiv As HTMLDiv(Of ObjectType)
    Public Property HeaderTag As HTMLTag(Of ObjectType)
    Public Property HeadingSize As Integer = 1
    Public Property HeadingText As String = ""
    Public Property SubText As String = ""
    Public Property HeadingTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing

    Public Sub New(Optional HeadingSize As Integer = 2, Optional HeadingText As String = "Heading", Optional SubText As String = "",
                   Optional HeadingTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing)

      Me.HeadingSize = HeadingSize
      Me.HeadingText = HeadingText
      Me.HeadingTextExpression = HeadingTextExpression
      Me.SubText = SubText

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      PageHeaderDiv = Helpers.DivC("page-header")
      With PageHeaderDiv
        HeaderTag = .Helpers.HTMLTag("h" & HeadingSize)
        With HeaderTag
          If HeadingTextExpression Is Nothing Then
            .Helpers.HTML(HeadingText & " <small>" & SubText & "</small>")
          Else
            .AddBinding(KnockoutBindingString.html, HeadingTextExpression)
          End If
        End With
      End With

    End Sub

    Protected Friend Overrides Sub Render()

      MyBase.Render()
      RenderChildren()

    End Sub

  End Class

  Public Class BootstrapUnorderedList(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property UnorderedList As HTMLTag(Of ObjectType)
    Private mID As String = ""
    Private mCssClass As String = ""

    Public Function AddItem(Optional HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                            Optional LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                            Optional HrefString As String = "",
                            Optional LinkTextString As String = "",
                            Optional Target As Singular.Web.LinkTargetType = LinkTargetType.NotSet,
                            Optional Glyphicon As String = "",
                            Optional LinkClickBinding As String = "") As HTMLTag(Of ObjectType)

      Dim NewItem As HTMLTag(Of ObjectType)

      With UnorderedList
        NewItem = .Helpers.HTMLTag("li")
        With NewItem
          With .Helpers.BootstrapLinkFor(HRefExpression, LinkTextExpression, HrefString, "", Target, "")
            With .Link
              If LinkClickBinding <> "" Then
                .AddBinding(KnockoutBindingString.click, LinkClickBinding)
              End If
              With .Helpers.HTMLTag("i")
                .AddClass(Glyphicon)
              End With
              With .Helpers.HTMLTag("span")
                .Helpers.HTML(LinkTextString)
              End With
            End With
          End With
        End With
      End With

      Return NewItem

    End Function

    Public Function AddSubMenu(Optional Icon As String = "",
                               Optional Text As String = "") As BootstrapUnorderedList(Of ObjectType)

      '          <li>
      '              <a class="dropdown-toggle" href="#">
      '                  <i class="icon-edit"></i>
      '                  <span>Forms</span>
      '                  <i class="icon-chevron-down"></i>
      '              </a>
      '              <ul class="submenu">
      '                  <li><a href="form-showcase.html">Form showcase</a></li>
      '                  <li><a href="form-wizard.html">Form wizard</a></li>
      '              </ul>
      '          </li>
      Dim ListItem As HTMLTag(Of ObjectType)
      Dim NewItem As BootstrapUnorderedList(Of ObjectType)
      Dim Toggle As HTMLTag(Of ObjectType)

      With UnorderedList
        ListItem = .Helpers.HTMLTag("li")
        With ListItem
          Toggle = .Helpers.HTMLTag("a")
          With Toggle
            .AddClass("dropdown-toggle")
            .Attributes("href") = "#"
            '.Helpers.BootstrapGlyphIcon(Icon)
            With .Helpers.HTMLTag("i")
              .AddClass(Icon)
            End With
            With .Helpers.HTMLTag("span")
              .Helpers.HTML(Text)
            End With
            '.Helpers.BootstrapGlyphIcon("glyphicon-chevron-down")
            With .Helpers.HTMLTag("i")
              .AddClass("icon-chevron-down")
            End With
          End With
          NewItem = .Helpers.BootstrapUnorderedList("", "submenu")
        End With
      End With

      Return NewItem

    End Function

    Public Sub New(ID As String, CssClass As String)
      mID = ID
      mCssClass = CssClass
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      UnorderedList = Helpers.HTMLTag("ul")
      If mCssClass <> "" Then
        UnorderedList.AddClass(mCssClass)
      End If
      If mID <> "" Then
        UnorderedList.Attributes("id") = mID
      End If

    End Sub

    Protected Friend Overrides Sub Render()

      MyBase.Render()
      RenderChildren()

    End Sub

  End Class

  ''' <summary>
  ''' Creates a table / grid and creates a row for each item in the provided list.
  ''' </summary>
  Public Class BootstrapTable(Of ObjectType, ChildControlObjectType)
    Inherits HelperBase(Of ObjectType, ChildControlObjectType)

    Public Class TableCaption
      Inherits HelperBase(Of ObjectType)

      Protected Friend Overrides Sub Render()
        MyBase.Render()

        WriteFullStartTag("caption", TagType.Normal)
        RenderChildren()
        Writer.WriteEndTag("caption")
      End Sub

    End Class

    Public Class TableColumn
      Inherits HelperBase(Of ChildControlObjectType)

      Public Property HeaderText As String
      Public Property ColSpan As Integer = 1
      Public Property AutoFormat As Boolean = True
      Public Property AllowSort As Boolean = True

      Private mEditor As EditorBase(Of ChildControlObjectType)
      Private mFieldDisplay As FieldDisplay(Of ChildControlObjectType)
      Friend ReadOnlyColumn As Boolean = False
      Friend HardCodedContent As String = ""

      Private mHeaderStyle As CSSStyle
      Public ReadOnly Property HeaderStyle As CSSStyle
        Get
          If mHeaderStyle Is Nothing Then
            mHeaderStyle = New CSSStyle
          End If
          Return mHeaderStyle
        End Get
      End Property


      Public ReadOnly Property Editor As EditorBase(Of ChildControlObjectType)
        Get
          Return mEditor
        End Get
      End Property

      Public ReadOnly Property FieldDisplay As FieldDisplay(Of ChildControlObjectType)
        Get
          Return mFieldDisplay
        End Get
      End Property

      Private mCellBindings As New KnockoutBindingManager(Of ChildControlObjectType)(Me, Nothing)
      Public ReadOnly Property CellBindings As KnockoutBindingManager(Of ChildControlObjectType)
        Get
          Return mCellBindings
        End Get
      End Property

      Private mHeaderBindings As New KnockoutBindingManager(Of ChildControlObjectType)(Me, Nothing)
      Public ReadOnly Property HeaderBindings As KnockoutBindingManager(Of ChildControlObjectType)
        Get
          Return mHeaderBindings
        End Get
      End Property

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        If PropertyInfo IsNot Nothing Then
          If ReadOnlyColumn Then

            Dim FormatString As String = ""
            Dim MainType As Reflection.SMemberInfo.MainType



            Dim smi As New Singular.Reflection.SMemberInfo(PropertyInfo)
            MainType = smi.DataTypeMain
            If AutoFormat AndAlso PropertyInfo IsNot Nothing Then
              Select Case MainType
                Case Reflection.SMemberInfo.MainType.Number
                  Style.TextAlign = TextAlign.right
                  If smi.IsInteger Then
                    FormatString = "#,##0;(#,##0)"
                  Else
                    FormatString = "#,##0.00;(#,##0.00)"
                  End If
                Case Reflection.SMemberInfo.MainType.Boolean
                  Style.TextAlign = TextAlign.center
                Case Reflection.SMemberInfo.MainType.Date
                  Style.TextAlign = TextAlign.center
                  FormatString = "dd MMM yyyy"
              End Select
            End If


            Dim TableParent As BootstrapTable(Of ObjectType, ChildControlObjectType) = CType(Parent, TableRow).Parent

            If TableParent.ReadOnlyColumnsHaveSpan OrElse smi.DataTypeMain = Reflection.SMemberInfo.MainType.Boolean Then

              mFieldDisplay = New Singular.Web.CustomControls.FieldDisplay(Of ChildControlObjectType)
              mFieldDisplay.DataType = MainType
              If mLinqExpression IsNot Nothing Then
                mFieldDisplay.For(mLinqExpression)
              Else
                mFieldDisplay.For(PropertyInfo)
              End If

              mFieldDisplay.TagType = FieldTagType.span
              'If TableParent.ServerBindObject Is Nothing Then
              '  TargetStyle = mFieldDisplay.Style
              'End If
              If FormatString <> "" Then
                mFieldDisplay.FormatString = FormatString
              End If

              AddControl(mFieldDisplay)

            Else

              Select Case MainType
                Case Reflection.SMemberInfo.MainType.Number
                  CellBindings.Add(KnockoutBindingString.NValue, Singular.Web.Controls.BindingHelpers.GetNumberBinding(GetForJS, Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(PropertyInfo), FormatString))
                Case Reflection.SMemberInfo.MainType.Date
                  CellBindings.Add(KnockoutBindingString.DateValue, Singular.Web.Controls.BindingHelpers.GetDateBinding(GetForJS, PathToContext, Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateField)(PropertyInfo), FormatString))
                Case Else
                  CellBindings.Add(KnockoutBindingString.html, GetForJS)
              End Select

            End If

          Else
            'Editable
            If mLinqExpression IsNot Nothing Then
              mEditor = Singular.Web.CustomControls.EditorBase(Of ChildControlObjectType).GetEditor(mLinqExpression)
            Else
              mEditor = Singular.Web.CustomControls.EditorBase(Of ChildControlObjectType).GetEditor(PropertyInfo)
            End If

            If TypeOf mEditor Is CheckBoxEditor(Of ChildControlObjectType) Then
              Style.TextAlign = TextAlign.center
            End If

            mEditor.Settings.Width = "100%"
            'mEditor.TableCellMode()

            mEditor.Tooltip = Singular.Reflection.GetDisplayName(PropertyInfo)
            AddControl(mEditor)
          End If


          If HeaderText = "" Then
            HeaderText = Singular.Reflection.GetDisplayName(PropertyInfo)
          End If
        End If

      End Sub

      Friend Sub RenderHeader()
        MyBase.Render()

        If ColSpan > 1 Then
          Attributes("colspan") = ColSpan
        End If

        Dim TableParent As BootstrapTable(Of ObjectType, ChildControlObjectType) = CType(Parent, TableRow).Parent

        If PropertyInfo IsNot Nothing AndAlso TableParent.ServerBindObject Is Nothing AndAlso AllowSort Then
          Attributes("data-Property") = PropertyInfo.Name
        End If

        Dim TempStyle As CSSStyle = Nothing
        If mHeaderStyle IsNot Nothing Then
          TempStyle = Style
          Style = mHeaderStyle
        End If

        WriteFullStartTag("th", TagType.Normal, HeaderBindings)

        If TempStyle IsNot Nothing Then
          Style = TempStyle
        End If

        Writer.WriteEncodedText(HeaderText)

        Writer.WriteEndTag("th")

      End Sub

      Public Property OnRenderTD As Action(Of TableColumn, Object)

      Friend Sub RenderContent()
        MyBase.Render()

        If ColSpan > 1 Then
          Attributes("colspan") = ColSpan
        End If

        If OnRenderTD IsNot Nothing Then
          OnRenderTD.Invoke(Me, ServerBindObject)
        End If

        WriteFullStartTag("td", TagType.Normal, CellBindings)
        If HardCodedContent <> "" Then
          Writer.Write(HardCodedContent)
        End If

        For Each ctl As Controls.CustomWebControl In Me.Controls
          ctl.ServerBindObject = ServerBindObject
          ctl.Render()
        Next

        Writer.WriteEndTag("td")
        ServerBindObject = Nothing
      End Sub

    End Class

    Public MustInherit Class TableRow
      Inherits HelperBase(Of ChildControlObjectType)

      Protected mRowButtons As New List(Of Button(Of ChildControlObjectType))


      'Friend mIsBodyRow As Boolean = True

      Public Property IncludeInHeader As Boolean = True

      Public Property AutoFormat As Boolean = True

      Friend Function AddButton(ButtonText As String) As Button(Of ChildControlObjectType)
        Dim mNewButton = Helpers.Button(ButtonText)
        mNewButton.Attributes("tabindex") = "-1"
        mRowButtons.Add(mNewButton)
        Return mNewButton
      End Function

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

      End Sub

#Region " Add Column Functions "

      Protected Sub AddAutoClass(col As TableColumn, IsReadOnly As Boolean)
        Dim tbl = CType(Parent, BootstrapTable(Of ObjectType, ChildControlObjectType))
        If IsReadOnly Then
          If tbl.ReadOnlyCellClass <> "" Then
            col.AddClass(tbl.ReadOnlyCellClass)
          End If
        Else
          If tbl.EditableCellClass <> "" Then
            col.AddClass(tbl.EditableCellClass)
          End If
          If tbl.EditorClass <> "" AndAlso col.Editor IsNot Nothing Then
            col.Editor.AddClass(tbl.EditorClass)
          End If
        End If
      End Sub

      Protected Sub SetWidthFromAttribute(col As TableColumn)

        'Check if it has a column width attribute
        Dim cw = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ColumnWidth)(col.PropertyInfo)
        If cw IsNot Nothing AndAlso cw.DefaultWidth <> 0 Then
          col.Style.Width = cw.DefaultWidth
        End If

      End Sub

#End Region

      Friend Function GetDataCellCount() As Integer
        Dim CellCount As Integer = 0
        For Each ctl As Controls.CustomWebControl In Controls
          If TypeOf ctl Is TableColumn Then
            CellCount += CType(ctl, TableColumn).ColSpan
          End If
        Next
        If mRowButtons.Count > 0 Then
          CellCount += 1
        End If
        Return CellCount
      End Function

      Protected Overridable Sub RenderPreCells(ByVal Index As Integer, ByVal RowCount As Integer, ByVal TableHasChildBands As Boolean)

      End Sub

      Friend Sub RenderContent(ByVal Index As Integer, ByVal RowCount As Integer, ByVal TableHasChildBands As Boolean)
        MyBase.Render()

        Writer.WriteBeginTag("tr")
        WriteClass()
        Bindings.WriteKnockoutBindings()
        WriteStyles()
        Writer.WriteCloseTag(True)

        RenderPreCells(Index, RowCount, TableHasChildBands)

        'Normal Cells
        For Each col As HelperBase In Controls
          If TypeOf col Is TableColumn Then
            CType(col, TableColumn).ServerBindObject = ServerBindObject
            CType(col, TableColumn).RenderContent()
          End If
        Next

        'Remove Button
        If Index = 0 AndAlso (mRowButtons.Count > 0) Then
          Writer.WriteBeginTag("td")
          If TableHasChildBands Then
            RowCount -= 1
          End If
          Writer.WriteAttribute("class", "RButtons")
          If RowCount > 1 Then
            Writer.WriteAttribute("rowspan", RowCount)
          End If
          Writer.WriteCloseTag(False)

          'RemoveButton.Render()
          For Each btn In mRowButtons
            btn.Render()
          Next

          Writer.WriteEndTag("td")
        End If

        Writer.WriteEndTag("tr", True)
        ServerBindObject = Nothing
      End Sub

    End Class

    Public Class DataRowTemplate
      Inherits TableRow

      Private mExpandButton As Button(Of ChildControlObjectType)
      Friend mRemoveButton As Button(Of ChildControlObjectType)
      Friend mIsChildBandRow As Boolean = False

      Friend Sub AddExpandButton(ExpandedProperty As String)
        mExpandButton = Helpers.Button("")
        mExpandButton.Style("width") = Singular.Web.Controls.ExpandButtonWidth & "px"
        mExpandButton.AddBinding(KnockoutBindingString.click, "$data." & ExpandedProperty & "(!$data." & ExpandedProperty & "())")
        mExpandButton.ButtonText.AddBinding(KnockoutBindingString.html, "($data." & ExpandedProperty & "() ? '-' : '+')")
        mExpandButton.AddBinding(KnockoutBindingString.title, "($data." & ExpandedProperty & "() ? 'Hide Child Rows.' : 'Expand Child Rows.')")

        mExpandButton.ButtonStyle = ButtonMainStyle.Default
        mExpandButton.ButtonSize = ButtonSize.ExtraSmall
      End Sub

      Public ReadOnly Property ExpandButton As Button(Of ChildControlObjectType)
        Get
          Return mExpandButton
        End Get
      End Property

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        If CType(Parent, BootstrapTable(Of ObjectType, ChildControlObjectType)).AllowRemove Then
          mRemoveButton = AddButton("")
          mRemoveButton.Image.SrcDefined = DefinedImageType.TrashCan
          mRemoveButton.Attributes("title") = "Delete this record."

          mRemoveButton.ButtonSize = ButtonSize.ExtraSmall
          mRemoveButton.ButtonStyle = ButtonMainStyle.Danger
          mRemoveButton.AddRemoveBinding()
        End If
      End Sub

      ''' <summary>
      ''' Adds a column with nothing in it.
      ''' </summary>
      Public Function AddColumn(Optional HeaderText As String = "") As TableColumn
        Dim Col = AddColumn(CType(Nothing, PropertyInfo), HeaderText)
        AddAutoClass(Col, True)
        Return Col
      End Function

      ''' <summary>
      ''' Adds an editable Column. 
      ''' </summary>
      ''' <param name="le">The property to bind the column to.</param>
      Public Function AddColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), Optional HeaderText As String = "") As TableColumn

        Dim Col As New TableColumn With {.HeaderText = HeaderText}
        Col.For(le)
        SetWidthFromAttribute(Col)

        AddControl(Col)
        AddAutoClass(Col, False)
        Return Col

      End Function

      ''' <summary>
      ''' Adds an editable Column. 
      ''' </summary>
      Public Function AddColumn(pi As PropertyInfo, Optional HeaderText As String = "") As TableColumn

        Dim Col As New TableColumn With {.HeaderText = HeaderText}
        If pi IsNot Nothing Then
          Col.For(pi)
          SetWidthFromAttribute(Col)
        End If

        AddControl(Col)
        AddAutoClass(Col, False)
        Return Col

      End Function

      ''' <summary>
      ''' Adds an editable Column. 
      ''' </summary>
      ''' <param name="le">The property to bind the column to.</param>
      Public Function AddColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ColumnWidth As Integer, Optional HeaderText As String = "") As TableColumn

        Dim col = AddColumn(le, HeaderText)
        col.Style.Width = ColumnWidth & "px"
        Return col

      End Function

      ''' <summary>
      ''' Adds a Column that displays the property but doesn't allow it to be edited.
      ''' </summary>
      ''' <param name="le">The Property to Display</param>
      Public Function AddReadOnlyColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), Optional HeaderText As String = "") As TableColumn

        Dim Col As New TableColumn With {.HeaderText = HeaderText}
        Col.ReadOnlyColumn = True
        Col.AutoFormat = AutoFormat
        Col.For(le)
        SetWidthFromAttribute(Col)
        AddAutoClass(Col, True)
        Return AddControl(Col)

      End Function

      ''' <summary>
      ''' Adds a Column that displays the property but doesn't allow it to be edited.
      ''' </summary>
      Public Function AddReadOnlyColumn(pi As PropertyInfo, Optional HeaderText As String = "") As TableColumn

        Dim Col As New TableColumn With {.HeaderText = HeaderText}
        Col.ReadOnlyColumn = True
        Col.AutoFormat = AutoFormat
        If pi IsNot Nothing Then
          Col.For(pi)
          SetWidthFromAttribute(Col)
        End If
        AddAutoClass(Col, True)
        Return AddControl(Col)

      End Function

      ''' <summary>
      ''' Adds a Column that displays the property but doesn't allow it to be edited.
      ''' </summary>
      ''' <param name="le">The Property to Display</param>
      Public Function AddReadOnlyColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ColumnWidth As Integer, Optional HeaderText As String = "") As TableColumn

        Dim Col = AddReadOnlyColumn(le, HeaderText)
        Col.Style.Width = ColumnWidth & "px"
        Return Col

      End Function

      Friend Sub RenderHeader(Index As Integer, RowCount As Integer)
        MyBase.Render()

        If IncludeInHeader Then
          Writer.WriteFullBeginTag("tr")

          'Expand Button Heading
          If mExpandButton IsNot Nothing Then
            Writer.WriteBeginTag("th")
            Writer.WriteAttribute("class", "LButtons")
            Writer.WriteAttribute("rowspan", RowCount)
            'Writer.WriteAttribute("width", "30px")
            Writer.WriteCloseTag(False)
            Writer.WriteEndTag("th")
          End If

          'Normal Cells
          For Each col As HelperBase In Controls
            If TypeOf col Is TableColumn Then
              CType(col, TableColumn).RenderHeader()
            End If
          Next

          'Remove Button Heading
          If Index = 0 AndAlso (mRowButtons.Count > 0) Then
            Writer.WriteBeginTag("th")
            'Writer.WriteAttribute("class", "RButtons")
            If RowCount > 1 Then
              Writer.WriteAttribute("rowspan", RowCount)
            End If
            Writer.WriteCloseTag(False)
            Writer.WriteEndTag("th")
          End If

          Writer.WriteEndTag("tr")
        End If

      End Sub

      Protected Overrides Sub RenderPreCells(ByVal Index As Integer, ByVal RowCount As Integer, ByVal TableHasChildBands As Boolean)

        'Expand Button
        If mExpandButton IsNot Nothing Then
          Writer.WriteBeginTag("td")
          Writer.WriteAttribute("class", "LButtons")
          If RowCount > 2 AndAlso Index = 0 Then
            Writer.WriteAttribute("rowspan", RowCount - 1)
          End If
          'Writer.WriteAttribute("width", "30px")
          Writer.WriteCloseTag(False)
          mExpandButton.Render()
          Writer.WriteEndTag("td")
        End If
        If mIsChildBandRow Then
          Writer.WriteFullBeginTag("td")
          Writer.WriteEndTag("td")
        End If

      End Sub

    End Class

    Public Class TableFooterRow
      Inherits TableRow

      Public Function AddColumn(Optional CellContent As String = "") As TableColumn

        Dim Col As New TableColumn
        Col.HardCodedContent = CellContent
        AddControl(Col)

        Return Col

      End Function

      ''' <summary>
      ''' Adds a footer column that displays the sum of all the items in the list. 
      ''' </summary>
      Public Function AddSumColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object))) As TableColumn

        Return AddSummaryColumn("Sum", le)

      End Function

      ''' <summary>
      ''' Adds a footer column that displays the average of all the items in the list. 
      ''' </summary>
      Public Function AddAverageColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object))) As TableColumn

        Return AddSummaryColumn("Avg", le)

      End Function

      ''' <summary>
      ''' Adds a footer column that displays the average of all the items in the list. Only items with a non zero value are included.
      ''' </summary>
      Public Function AddAverageNZColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object))) As TableColumn

        Return AddSummaryColumn("AvgNZ", le)

      End Function

      Private Function AddSummaryColumn(Type As String, le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object))) As TableColumn

        Dim Col = AddColumn()
        Dim Binding As String = CType(Parent, BootstrapTable(Of ObjectType, ChildControlObjectType)).GetForJS
        Dim Pi As PropertyInfo = Singular.Reflection.GetMember(le)
        Binding &= "()." & Type & "('" & Pi.Name & "')"

        Col.Style.TextAlign = TextAlign.right
        Col.CellBindings.Add(KnockoutBindingString.NValue,
                             Singular.Web.Controls.BindingHelpers.GetNumberBinding(Binding,
                                                                                   Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(Pi),
                                                                                   ""))
        Return Col


      End Function

    End Class

    'Private Variables
    Private mAddNewButton As Button(Of ObjectType)
    Private mRowCount As Integer = 1
    Private mFirstRow As DataRowTemplate
    Private mFooterRows As New List(Of TableRow)
    Private mChildBandCell As TableColumn
    Private mChildBandRow As DataRowTemplate
    Private mTableBodyClass As String = ""

    Public Property EditableCellClass As String = ""
    Public Property EditorClass As String = ""
    Public Property ReadOnlyCellClass As String = ""

    Public Property BodyHeightMin As Integer = 0
    Public Property BodyHeightMax As Integer = 0

    Public Property ReadOnlyColumnsHaveSpan As Boolean = True

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If PathToContext() <> "" Then
        AddBinding(KnockoutBindingString.with, PathToContext)
      End If
      BindingManager.HasBindingContext = True

      mFirstRow = New DataRowTemplate
      If ServerBindObject IsNot Nothing Then
        AllowRemove = False
      End If
      AddControl(mFirstRow)

      If AllowAddNew Then
        mAddNewButton = AddControl(New Button(Of ObjectType) With {.Text = "Add", .ButtonID = ""})
        mAddNewButton.Bindings.AddAddBinding(PropertyInfo)
      End If

      'AddClass("Grid")
      AddClass("table " & IIf(Striped, "table-striped", "") & " table-bordered table-hover table-condensed")
      AddClass("SUI-RuleBorder") 'Make broken rule controls appear with red border, rather than with an icon.

    End Sub

    'Public Properties
    Public Property AllowAddNew As Boolean = True
    Public Property AllowRemove As Boolean = True
    Public Property ShowHeader As Boolean = True
    Public Property AllowClientSideSorting As Boolean = True
    Public Property DataSourceString As String = ""
    Public Property IfExpression As String = ""
    Public Property Striped As Boolean = True


    Public ReadOnly Property FirstRow As DataRowTemplate
      Get
        Return mFirstRow
      End Get
    End Property

    Public ReadOnly Property FooterRow As TableFooterRow
      Get
        If mFooterRows.Count = 0 Then
          Dim NewRow As New TableFooterRow
          mFooterRows.Add(NewRow)
          AddControl(NewRow)
        End If
        Return mFooterRows(0)
      End Get
    End Property

    Public Function AddFooterRow() As TableFooterRow

      If mFooterRows.Count = 0 Then
        Return FooterRow
      Else
        Dim NewRow As New TableFooterRow
        AddControl(NewRow)
        mFooterRows.Add(NewRow)
        Return NewRow
      End If
    End Function

    Public ReadOnly Property ParentCell As HelperBase
      Get
        If TypeOf Parent Is HelperBase Then
          Return Parent
        Else
          Return Nothing
        End If
      End Get
    End Property

    Public ReadOnly Property AddNewButton As Button(Of ObjectType)
      Get
        Return mAddNewButton
      End Get
    End Property

    Public ReadOnly Property RemoveButton As Button(Of ChildControlObjectType)
      Get
        Return FirstRow.mRemoveButton
      End Get
    End Property

    Public ReadOnly Property ChildBandRow As DataRowTemplate
      Get
        Return mChildBandRow
      End Get
    End Property

    Public ReadOnly Property ChildBandCell As TableColumn
      Get
        Return mChildBandCell
      End Get
    End Property

    Public Property TableBodyClass As String
      Get
        Return mTableBodyClass
      End Get
      Set(value As String)
        mTableBodyClass = value
      End Set
    End Property

    ''' <summary>
    ''' Adds a button to the data rows last cell. E.g. if you want to add a select button.
    ''' </summary>
    ''' <param name="ButtonText">The text of the button.</param>
    Public Function AddButton(ByVal ButtonText As String) As Button(Of ChildControlObjectType)
      Return FirstRow.AddButton(ButtonText)
    End Function

    ''' <summary>
    ''' Adds a row to each data row. E.g. If you want each item in a list to have 2 rows in the table.
    ''' </summary>
    Public Function AddRow() As DataRowTemplate
      If mChildBandCell IsNot Nothing Then
        Throw New Exception("Normal Rows cannot be added after Child Bands.")
      Else
        Dim row As New DataRowTemplate
        mRowCount += 1
        Return AddControl(row)
      End If
    End Function

    Private mCaption As TableCaption
    Public Function AddCaption(Optional CaptionText As String = "") As TableCaption
      mCaption = AddControl(New TableCaption())
      mCaption.Helpers.HTML(CaptionText)
      Return mCaption
    End Function

    Public Function AddChildTable(Of ChildTableObjectType)(pi As PropertyInfo, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean, ChildIFExpression As String) As BootstrapTable(Of ChildControlObjectType, ChildTableObjectType)

      'Remember what position the child band row container is in.
      If mChildBandCell Is Nothing Then

        'Add a new row for the child band
        mChildBandRow = AddRow()
        mChildBandRow.IncludeInHeader = False

        'Child Band only has one cell that spans the whole table.
        mChildBandCell = ChildBandRow.AddColumn
        mChildBandCell.ColSpan = FirstRow.GetDataCellCount

        'Check if the object has an expanded property
        Dim piExpand As PropertyInfo = Singular.Reflection.GetProperty(OverrideChildType, "Expanded")
        If piExpand Is Nothing Then
          piExpand = Singular.Reflection.GetProperty(OverrideChildType, "IsExpanded")
        End If

        If piExpand IsNot Nothing Then
          mFirstRow.AddExpandButton(piExpand.Name)
          Dim eo = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ExpandOptions)(piExpand)
					ChildBandRow.AddBinding(If(eo Is Nothing OrElse eo.RenderChildrenMode = Singular.DataAnnotations.ExpandOptions.RenderChildrenModeType.OnParentRender, KnockoutBindingString.visible, KnockoutBindingString.if), "$data." & piExpand.Name)
          ChildBandRow.mIsChildBandRow = True
        End If

      End If

      Return mChildBandCell.Helpers.BootstrapTableFor(Of ChildTableObjectType)(pi, AllowAddNew, AllowRemove, ChildIFExpression)

    End Function

    Public Function AddChildTable(Of ChildTableObjectType)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean, ChildIFExpression As String) As BootstrapTable(Of ChildControlObjectType, ChildTableObjectType)

      Return AddChildTable(Of ChildTableObjectType)(Singular.Reflection.GetMember(Of ChildControlObjectType)(le), AllowAddNew, AllowRemove, ChildIFExpression)

    End Function

    Public ReadOnly Property HasChildTables As Boolean
      Get
        Return mChildBandCell IsNot Nothing
      End Get
    End Property

    ''' <summary>
    ''' Adds a column for each browsable property in the object. List properties are added as child tables.
    ''' </summary>
    Public Sub AutoGenerateColumns()

      'Temp Child Lists
      Dim TempChildLists As New List(Of System.Reflection.PropertyInfo)

      For Each pi As System.Reflection.PropertyInfo In OverrideChildType.GetProperties(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
        If Singular.Reflection.AutoGenerateField(pi) AndAlso Not Singular.Misc.In(pi.Name, "Expanded", "IsExpanded") Then

          'Check Return Type
          If Singular.Reflection.TypeImplementsInterface(pi.PropertyType, GetType(ISingularListBase)) Then
            'For Lists, add a child table.
            TempChildLists.Add(pi)
          Else

            If pi.CanWrite Then
              FirstRow.AddColumn(pi)
            Else
              FirstRow.AddReadOnlyColumn(pi)
            End If

          End If

        End If
      Next

      'Add Child Lists
      For Each pi As System.Reflection.PropertyInfo In TempChildLists
        Dim childAllowAddNew As Boolean = AllowAddNew
        Dim childAllowRemove As Boolean = AllowRemove
        If Singular.Reflection.TypeImplementsInterface(pi.PropertyType, GetType(Singular.ISingularReadOnlyListBase)) Then
          childAllowAddNew = False
          childAllowRemove = False
        End If
        Dim ChildTbl = AddChildTable(Of Object)(pi, childAllowAddNew, childAllowRemove, "")
        ChildTbl.OverrideChildType = Singular.Reflection.GetLastGenericType(pi.PropertyType)
        ChildTbl.AutoGenerateColumns()
      Next

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        If BodyHeightMin > 0 OrElse BodyHeightMax > 0 Then
          Attributes.Add("data-ScrollHeight", BodyHeightMin & "|" & BodyHeightMax)
        End If

        'Table
        WriteFullStartTag("table", TagType.IndentChildren)

        'Caption
        If mCaption IsNot Nothing Then
          mCaption.Render()
        End If

        'Header
        If ShowHeader Then
          .WriteFullBeginTag("thead")
          'Header Rows
          For i As Integer = 0 To Me.Controls.Count - 1
            If TypeOf Controls(i) Is DataRowTemplate Then
              CType(Controls(i), DataRowTemplate).RenderHeader(i, mRowCount)
            End If
          Next

          .WriteEndTag("thead")
        End If

        'Start the Table Body
        .WriteBeginTag("tbody")
        .WriteAttribute("class", mTableBodyClass)
        '.WriteAttribute("data-RowsPerItem", mRowCount)

        If ServerBindObject Is Nothing Then
          RenderClientBoundRows(Writer)
        Else
          RenderServerBoundRows(Writer)
        End If

        If IfExpression <> "" Then
          Writer.Write("<!-- /ko -->")
        End If

        .WriteEndTag("tbody", True)

        'Footer section
        If mFooterRows IsNot Nothing OrElse AllowAddNew Then
          .WriteFullBeginTag("tfoot", True)
        End If

        'Summary row
        If mFooterRows.Count > 0 Then
          For Each fr As TableFooterRow In mFooterRows
            fr.RenderContent(0, 0, mChildBandCell IsNot Nothing)
          Next

        End If

        'Add New Button
        If AllowAddNew Then

          .WriteBeginTag("tr")
          '.WriteAttribute("class", "AddNewRow")
          .WriteCloseTag(True)
          .WriteBeginTag("td")
          .WriteAttribute("colspan", FirstRow.GetDataCellCount + If(FirstRow.ExpandButton IsNot Nothing, 1, 0))
          .WriteCloseTag(False)

          mAddNewButton.Render()

          .WriteEndTag("td")
          .WriteEndTag("tr", True)
        End If

        If mFooterRows.Count > 0 OrElse AllowAddNew Then
          .WriteEndTag("tfoot", True)
        End If

        .WriteEndTag("table", True)

      End With

    End Sub

    Public Property ApplyAlternateRowStyle As Boolean = False

    Private Sub RenderClientBoundRows(ByVal Writer As HtmlTextWriter)

      Dim BindingString As String = ""
      If DataSourceString = "" Then
        DataSourceString = PropertyInfo.Name
        If AllowClientSideSorting Then
          BindingString = "SFilter: " & PropertyInfo.Name & ", "
        End If
      End If

      If ApplyAlternateRowStyle Then
        Writer.WriteAttribute("data-bind", BindingString & "template: {foreach: Singular.ProcessList(" & DataSourceString & "), afterRender: function(c, o){ Singular.FormatTableRow(c, o); Singular.AfterTemplateRender(c, o) }}")
      Else
        Writer.WriteAttribute("data-bind", BindingString & "template: {foreach: Singular.ProcessList(" & DataSourceString & "), afterRender: function(c, o){ Singular.AfterTemplateRender(c, o) }}")
      End If

      Writer.WriteCloseTag(True)

      If IfExpression <> "" Then
        Writer.Write("<!-- ko if: " & IfExpression & " -->")
      End If

      'Data Rows.
      For i As Integer = 0 To Me.Controls.Count - 1
        If TypeOf Controls(i) Is DataRowTemplate Then
          CType(Controls(i), TableRow).RenderContent(i, mRowCount, mChildBandCell IsNot Nothing)
        End If
      Next

    End Sub

    Private Sub RenderServerBoundRows(ByVal Writer As HtmlTextWriter)
      Writer.WriteCloseTag(True)

      For Each row As Object In ServerBindObject

        For i As Integer = 0 To Me.Controls.Count - 1
          If TypeOf Controls(i) Is DataRowTemplate Then
            CType(Controls(i), TableRow).ServerBindObject = row
            CType(Controls(i), TableRow).RenderContent(i, mRowCount, mChildBandCell IsNot Nothing)
          End If
        Next

      Next
    End Sub

  End Class

  Public Class BootstrapProgressBar(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property Percentage As Decimal = 0.0
    Public Property ActionText As String = ""

    Public Sub New(Percentage As Decimal, ActionText As String)
      Me.Percentage = Percentage
      Me.ActionText = ActionText
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers.DivC("progress progress-striped active")
        With .Helpers.DivC("progress-bar")
          .Attributes("role") = ""
          .Attributes("aria-valuenow") = Percentage
          .Attributes("aria-valuemin") = "0"
          .Attributes("aria-valuemax") = "100"
          .Style.Width = Percentage.ToString & "%"
          With .Helpers.HTMLTag("span")
            .AddClass("action-text sr-only")
            .Helpers.HTML(ActionText)
          End With
        End With
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapPanelGroup(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    'Properties
    Public Property PanelGroup As HTMLDiv(Of ObjectType)
    Private mPanelGroupID As String = "PanelGroup"

    Public Function AddPanel(PanelColorClass As String, AccordionID As String) As BootstrapPanel(Of ObjectType)

      Dim t As New BootstrapPanel(Of ObjectType)(PanelColorClass, True, AccordionID, mPanelGroupID)
      Return AddControl(t)

    End Function

    Public Sub New(ID As String)
      mPanelGroupID = ID
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      PanelGroup = Helpers.DivC("panel-group")
      PanelGroup.Attributes("id") = mPanelGroupID

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapTabControl(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    'Properties
    Public Property ParentDiv As HTMLDiv(Of ObjectType)
    Public Property TabHeaderContainer As HTMLTag(Of ObjectType)
    Public Property TabContentContainer As HTMLDiv(Of ObjectType)
    Private mTabCssClass = "nav-justified"
    Private mTabRole As String = "tablist"
    Private mParentDivCssClass As String = ""

    Public Function AddTab(TabName As String, IconName As String, Active As Boolean, TabClickFunction As String, Optional TabNameDisplay As String = "") As BootstrapTab

      Dim t As New BootstrapTab(TabName, IconName, TabHeaderContainer, TabContentContainer, Active, TabClickFunction, TabNameDisplay)
      Return AddControl(t)

    End Function

    Public Sub New(Optional ParentDivCssClass As String = "", Optional ByVal TabCssClass As String = "nav-justified", Optional ByVal TabRole As String = "tablist")
      mTabCssClass = TabCssClass
      mTabRole = TabRole
      mParentDivCssClass = ParentDivCssClass
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      ParentDiv = Helpers.DivC(mParentDivCssClass)
      TabHeaderContainer = ParentDiv.Helpers.HTMLTag("ul")
      TabHeaderContainer.AddClass("nav nav-tabs" & " " & mTabCssClass)
      TabHeaderContainer.Attributes("role") = mTabRole
      TabContentContainer = ParentDiv.Helpers.DivC("tab-content")

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

    Public Class BootstrapTab
      Inherits HelperBase(Of ObjectType)

      Public Property TabName As String
      Public Property IconName As String
      Public Property TabHeader As HTMLTag(Of ObjectType)
      Public Property TabPane As HTMLDiv(Of ObjectType)
      Public Property TabHeaderContainer As HTMLTag(Of ObjectType)
      Public Property TabContentContainer As HTMLDiv(Of ObjectType)
      Private mActive As Boolean = False
      Private mTabClickFunction As String = ""
      Private mTabNameDisplay As String = ""

      Public Shadows ReadOnly Property Parent As BootstrapTabControl(Of ObjectType)
        Get
          Return MyBase.Parent
        End Get
      End Property

      'Private mHeaderBindings As New KnockoutBindingManager(Of ObjectType)(Me)
      'Public ReadOnly Property HeaderBindings As KnockoutBindingManager(Of ObjectType)
      '  Get
      '    Return mHeaderBindings
      '  End Get
      'End Property

      'Private mHeaderStyle As New CSSStyle
      'Public ReadOnly Property HeaderStyle As CSSStyle
      '  Get
      '    Return mHeaderStyle
      '  End Get
      'End Property

      'Friend Sub RenderHeader()
      '  'Writer.WriteFullBeginTag("li")

      '  'Writer.WriteBeginTag("a")
      '  'Writer.WriteAttribute("href", "#" & TabName.Replace(" ", ""))
      '  'mHeaderBindings.WriteKnockoutBindings()
      '  'WriteStyles(mHeaderStyle)

      '  'Writer.WriteCloseTag(False)
      '  'Writer.Write(TabName)
      '  'Writer.WriteEndTag("a")

      '  'Writer.WriteEndTag("li")
      'End Sub

      Public Sub New(TabName As String, IconName As String, TabHeaderContainer As HTMLTag(Of ObjectType), TabContentContainer As HTMLDiv(Of ObjectType), Active As Boolean, TabClickFunction As String, TabNameDisplay As String)
        Me.TabName = TabName
        Me.IconName = IconName
        Me.TabHeaderContainer = TabHeaderContainer
        Me.TabContentContainer = TabContentContainer
        mActive = Active
        mTabClickFunction = TabClickFunction
        mTabNameDisplay = TabNameDisplay
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        TabHeader = TabHeaderContainer.Helpers.HTMLTag("li")
        With TabHeader
          With .Helpers.HTMLTag("a")
            .Attributes("href") = "#" & Me.TabName.Replace(" ", "")
            .Attributes("role") = "tab"
            .Attributes("data-toggle") = "tab"
            If mTabClickFunction <> "" Then
              .AddBinding(KnockoutBindingString.click, mTabClickFunction)
            End If
            If Me.IconName <> "" Then
              With .Helpers.HTMLTag("i")
                .AddClass(Me.IconName)
              End With
            End If
            With .Helpers.HTMLTag("span")
              If mTabNameDisplay = "" Then
                .Helpers.HTML(Me.TabName)
              Else
                .Helpers.HTML(mTabNameDisplay)
              End If
            End With
          End With
        End With

        TabPane = TabContentContainer.Helpers.DivC("tab-pane")
        TabPane.Attributes("id") = Me.TabName.Replace(" ", "")
        If mActive Then
          TabPane.AddClass(" active")
        End If

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

  End Class

  Public Class BootstrapROStateButton(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    'Properties
    Private mCheckedButtonColor As String = ""
    Private mUnCheckedButtonColor As String = ""
    Private mCheckedIcon As String = ""
    Private mCheckedText As String = ""
    Private mUnCheckedIcon As String = ""
    Private mUnCheckedText As String = ""
    Private mProp As System.Reflection.MemberInfo = Nothing
    Private mPropValue As String = ""

    'Controls
    Public Property Button As HTMLTag(Of ObjectType)

    Public Sub New(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                   ByVal CheckedButtonColor As String, ByVal CheckedIcon As String, CheckedText As String,
                   ByVal UnCheckedButtonColor As String, UnCheckedIcon As String, UnCheckedText As String)

      mProp = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
      mPropValue = mProp.Name & "()"
      mCheckedButtonColor = CheckedButtonColor
      mUnCheckedButtonColor = UnCheckedButtonColor
      mCheckedIcon = CheckedIcon
      mCheckedText = CheckedText
      mUnCheckedIcon = UnCheckedIcon
      mUnCheckedText = UnCheckedText

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      Button = Helpers.HTMLTag("button")
      'Button.AddClass("btn")
      Button.Attributes.Add("type", "button")
      Button.AddBinding(KnockoutBindingString.css, "$data." & mPropValue & " ? 'btn btn-xs " & mCheckedButtonColor & "' : 'btn btn-xs " & mUnCheckedButtonColor & "'")
      Button.AddBinding(KnockoutBindingString.html, "$data." & mPropValue & " ? ""<span class='glyphicon " & mCheckedIcon & "> " & mCheckedText & "' : ""<span class='glyphicon " & mUnCheckedIcon & "> " & mUnCheckedText & "'")

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

  Public Class BootstrapSpanButton(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    ''' <summary>
    ''' Indicates what will happen when the button is clicked on the browser.
    ''' If PostBackType = Ajax, then set the Argument, Validate and LoadingText properties
    ''' If PostBackType = Full, then set the Argument and Validate properties
    ''' </summary>
    Public Property mPostBackType As PostBackType = DefaultButtonPostBackType

    Public Property Button As HTMLTag(Of ObjectType)
    Public Property GlyphContainer As BootstrapGlyphIcon(Of ObjectType)
    Public Property ButtonText As HTMLTag(Of ObjectType)
    Private mButtonGroupInd As Boolean = False
    Public Property ButtonGroup As HTMLDiv(Of ObjectType)

    Private mIconName As String = ""
    Private mID As String = ""
    Private mText As String = ""
    Private mCssClass As String = ""
    Private mTooltipToggle As String = "tooltip"
    Private mTooltipPlacement As String = "right"
    Private mTooltipText As String = ""
    Private mClickBinding As String = ""

    Public Property HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    Public Property ButtonTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    Public Property Target As LinkTargetType = LinkTargetType.NotSet

    'Public Sub AddPopover(Optional TitleExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
    '                      Optional ContentExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
    '                      Optional Placement As String = "top",
    '                      Optional Title As String = "",
    '                      Optional Content As String = "")
    '  Button.AddClass("Popovers")
    '  If TitleExpression IsNot Nothing Then
    '    Button.AddBinding(KnockoutBindingString.attr, TitleExpression)
    '  Else
    '    Button.AddBinding(KnockoutBindingString.attr, Title)
    '  End If
    '  If ContentExpression IsNot Nothing Then
    '    Button.AddBinding(KnockoutBindingString.attr, ContentExpression)
    '  Else
    '    Button.AddBinding(KnockoutBindingString.attr, Content)
    '  End If
    '  Button.Attributes("data-container") = "body"
    '  Button.Attributes("data-toggle") = "popover"
    '  Button.Attributes("data-placement") = Placement
    'End Sub

    'Public Sub AddPopover(Optional HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
    '                                Optional LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
    '                                Optional HrefString As String = "",
    '                                Optional LinkTextString As String = "",
    '                                Optional Target As Singular.Web.LinkTargetType = LinkTargetType.NotSet,
    '                                Optional Glyphicon As String = "") 'As HTMLTag(Of ObjectType)

    '  With NavItemsContainer
    '    With .Helpers.HTMLTag("li")
    '      With .Helpers.LinkFor(HRefExpression, LinkTextExpression, HrefString, "", Target)
    '        With .Helpers.HTMLTag("span")
    '          .AddClass("glyphicon " & Glyphicon)
    '        End With
    '        .Helpers.HTML(LinkTextString)
    '      End With
    '    End With
    '  End With

    'End Sub

    Public Sub New(Optional ID As String = "",
                   Optional Text As String = "",
                   Optional CssClass As String = "",
                   Optional IconName As String = "",
                   Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                   Optional ButtonGroupInd As Boolean = True,
                   Optional TooltipPlacement As String = "right",
                   Optional TooltipText As String = "",
                   Optional ClickBinding As String = "")
      'data-toggle="tooltip" data-placement="right" title="Tooltip on right"

      mID = ID
      mText = Text
      mCssClass = CssClass
      mIconName = IconName
      mPostBackType = PostBackType
      mButtonGroupInd = ButtonGroupInd
      mTooltipPlacement = TooltipPlacement
      mTooltipText = TooltipText
      mClickBinding = ClickBinding

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      'If mButtonGroupInd Then
      '  ButtonGroup = Helpers.DivC("btn-group")
      '  With ButtonGroup
      '    Button = .Helpers.HTMLTag("button")
      '  End With
      'Else
      '  Button = Helpers.HTMLTag("button")
      'End If
      'Button.AddClass("btn")

      'If mID <> "" Then
      '  Button.Attributes("id") = mID
      'End If

      'If mTooltipText <> "" Then
      '  Button.Attributes("data-toggle") = mTooltipToggle
      '  Button.Attributes("data-placement") = mTooltipPlacement
      '  Button.Attributes("title") = mTooltipText
      'End If

      'If mCssClass <> "" Then
      '  Button.AddClass(mCssClass)
      'End If

      'If Not Bindings.ContainsKey(Singular.Web.KnockoutBindingString.click) Then
      '  If mPostBackType = PostBackType.Ajax Then
      '    Button.AddBinding(KnockoutBindingString.click, "Singular.ButtonClickAjax(arguments[1])")
      '  End If
      'End If

      'If mID <> "" And mPostBackType = PostBackType.Full Then
      '  If Not Bindings.ContainsKey(Singular.Web.KnockoutBindingString.click) Then
      '    Button.AddBinding(KnockoutBindingString.click, "Singular.ButtonClickPost(arguments[1])")
      '  End If
      '  Button.Attributes.Add("type", "submit")
      'ElseIf mID = "" And (mPostBackType = PostBackType.None Or mPostBackType = PostBackType.Ajax) And mClickBinding <> "" Then
      '  Button.Attributes.Add("type", "button")
      '  Button.AddBinding(KnockoutBindingString.click, mClickBinding)
      'Else
      '  Button.Attributes.Add("type", "button")
      'End If

      'With Button
      '  If mIconName <> "" Then
      '    .Helpers.BootstrapGlyphIcon(mIconName)
      '  End If
      '  ButtonText = .Helpers.HTMLTag("span")
      '  ButtonText.AddClass("button-text-span")
      '  If mText <> "" Then
      '    ButtonText.Helpers.HTML(mText)
      '    'ElseIf ButtonTextExpression IsNot Nothing Then
      '    '  mButtonText.AddBinding(KnockoutBindingString.html, ButtonTextExpression)
      '  End If
      'End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

End Namespace
