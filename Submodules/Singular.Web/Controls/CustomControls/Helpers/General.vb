Imports System.Reflection
Imports System.Dynamic
Imports System.Linq.Expressions
Imports Singular.Web.Controls.HelperControls
Imports System.ComponentModel.DataAnnotations
Imports Singular.Web.Controls

Namespace CustomControls

  ''' <summary>
  ''' Label which describes what an editor is for.
  ''' </summary>
  Public Class FieldLabel(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mBindProperty As String
    Public Property LabelText As String = ""

    Public Sub New()

    End Sub

    Public Sub New(BindProperty As String, LabelText As String)
      Me.LabelText = LabelText
      mBindProperty = BindProperty
    End Sub

    Friend Overrides ReadOnly Property ControlSettingType As ControlSettingType
      Get
        Return ControlSettingType.Label
      End Get
    End Property

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If PropertyInfo IsNot Nothing Then
        'Add 'LabelFor' Binding to link this label to its Editor
        AddBinding(KnockoutBindingString.UID, GetForJS)
      ElseIf mBindProperty IsNot Nothing Then
        AddBinding(KnockoutBindingString.UID, mBindProperty)
      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        WriteFullStartTag("label", TagType.Normal)

        If LabelText <> "" Then
          .Write(LabelText)
        ElseIf PropertyInfo IsNot Nothing Then
          'Use the Display Name of the Property as the Label Name.
          .Write(Singular.Reflection.GetDisplayName(PropertyInfo))
        End If


        .WriteEndTag("label")

      End With

    End Sub

  End Class

  ''' <summary>
  ''' Displays the value of a property as a non editable label.
  ''' </summary>
  Public Class FieldDisplay(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property TagType As Singular.Web.FieldTagType = FieldTagType.span
    Public Property DisplayText As String = ""

    Friend Overrides ReadOnly Property ControlSettingType As ControlSettingType
      Get
        Return ControlSettingType.Display
      End Get
    End Property

    Protected mFormatString As String = ""

    Public Property FormatString As String
      Get
        Return mFormatString
      End Get
      Set(value As String)
        mFormatString = value
      End Set
    End Property

    Public Property DataType As Reflection.SMemberInfo.MainType = 0

    Private mTFA As Singular.DataAnnotations.TextField
    Private mDDA As Singular.DataAnnotations.DropDownWeb
    Private mNf As Singular.DataAnnotations.NumberField
    Private mDf As Singular.DataAnnotations.DateField

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If PropertyInfo IsNot Nothing Then

        If DataType = 0 Then
          Dim smi As New Singular.Reflection.SMemberInfo(PropertyInfo)
          DataType = smi.DataTypeMain
        End If

        mTFA = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.TextField)(PropertyInfo)

        mNf = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(PropertyInfo)
        If mNf Is Nothing Then
          mDf = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateField)(PropertyInfo)
          If mDf Is Nothing Then
            mDDA = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DropDownWeb)(PropertyInfo)
          End If
        End If



        If mTFA IsNot Nothing AndAlso mTFA.MultiLine Then
          AddClass("ro-multi-line")
          AddBinding(KnockoutBindingString.text, GetForJS)
        ElseIf mDDA IsNot Nothing Then

          If mDDA.LookupMember <> "" Then
            'If there is no source, then use the lookup column as a normal text value binding
            AddBinding(KnockoutBindingString.text, mDDA.LookupMember)
          Else
            'Else add a ValueLookup binding.
            AddBinding(KnockoutBindingString.ValueLookup, GetForJS)
            AddBinding(KnockoutBindingString.LookupList, mDDA.ClientName)
            AddBinding(KnockoutBindingString.optionsValue, mDDA.ValueMember.AddSingleQuotes)
            AddBinding(KnockoutBindingString.optionsText, mDDA.DisplayMember.AddSingleQuotes)
          End If

        Else

          Dim Binding As String = ""
          If DataType = Reflection.SMemberInfo.MainType.Date AndAlso Not GetForJS.Contains("Format") Then
            AddBinding(KnockoutBindingString.DateValue, Singular.Web.Controls.BindingHelpers.GetDateBinding(GetForJS, PathToContext, mDf, mFormatString))
          ElseIf DataType = Reflection.SMemberInfo.MainType.Number AndAlso Not GetForJS.ToLower.Contains("format") Then
            AddBinding(KnockoutBindingString.NValue, Singular.Web.Controls.BindingHelpers.GetNumberBinding(GetForJS, mNf, mFormatString))
          Else
            AddBinding(KnockoutBindingString.text, GetForJS)
          End If

        End If

      End If

    End Sub

    Private Sub RenderTextDisplay()
      WriteFullStartTag(TagType.ToString, HelperControls.TagType.Normal)
      If Controls.Count > 0 Then
        RenderChildren()
      End If
      Writer.Write(DisplayText)
      Writer.WriteEndTag(TagType.ToString)
    End Sub

    Private Sub RenderBooleanDisplay()
      Dim chk As New CheckBoxEditor(Of ObjectType)
      If mLinqExpression IsNot Nothing Then
        chk.For(mLinqExpression)
      Else
        chk.For(PropertyInfo)
      End If
      chk.Setup()
      chk.Attributes("disabled") = "disabled"
      chk.WriterOverride = Writer
      chk.Render()
    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        If ServerBindObject Is Nothing Then
          'Binding on the client.

          If PropertyInfo IsNot Nothing AndAlso DisplayText = "" Then

            If DataType = Reflection.SMemberInfo.MainType.Boolean Then
              RenderBooleanDisplay()
            Else
              RenderTextDisplay()
            End If

          Else
            RenderTextDisplay()
          End If

        Else
          'Binding on the server.
          If TagType = FieldTagType.strong Then
            .WriteFullBeginTag("strong", False)
          End If

          Dim Value As Object = PropertyInfo.GetValue(ServerBindObject, Nothing)
          If Value IsNot Nothing Then
            If FormatString <> "" Then

              Value = Format(Value, FormatString)

            End If
            If Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.HtmlProperty)(PropertyInfo) IsNot Nothing Then
              .Write(Value.ToString)
            Else
              .WriteEncodedText(Value.ToString)
            End If

          End If


          If TagType = FieldTagType.strong Then
            .WriteEndTag("strong", False)
          End If

        End If

      End With
      ServerBindObject = Nothing
    End Sub

  End Class

  ''' <summary>
  ''' Allows any asp control to be added to the helper control tree.
  ''' </summary>
  ''' <remarks></remarks>
  <System.Web.UI.ParseChildren(False), System.Web.UI.PersistChildren(True)> _
  Public Class HTMLSnippet
    Inherits HelperBase

    Private mInternalWriter As HtmlTextWriter

    Public Sub New()
      Me.Visible = False
      mInternalWriter = New HtmlTextWriter

    End Sub


#Region " Defined HTML "

    Public Sub HorizontalRule()
      mInternalWriter.WriteBeginTag("hr")
      mInternalWriter.WriteFullCloseTag()
    End Sub

    Public Sub NewLine()
      mInternalWriter.WriteBeginTag("br")
      mInternalWriter.WriteFullCloseTag()
    End Sub

    Public Sub Gap()
      mInternalWriter.WriteBeginTag("p")
      mInternalWriter.WriteFullCloseTag()
    End Sub

    Public Sub Heading1(HeadingText As String)
      mInternalWriter.WriteFullBeginTag("h1", False)
      mInternalWriter.Write(HeadingText)
      mInternalWriter.WriteEndTag("h1", False)
    End Sub

    Public Sub Heading2(HeadingText As String)
      mInternalWriter.WriteFullBeginTag("h2", False)
      mInternalWriter.Write(HeadingText)
      mInternalWriter.WriteEndTag("h2", False)
    End Sub

    Public Sub Heading3(HeadingText As String)
      mInternalWriter.WriteFullBeginTag("h3", False)
      mInternalWriter.Write(HeadingText)
      mInternalWriter.WriteEndTag("h3", False)
    End Sub

    Public Sub Heading4(HeadingText As String)
      mInternalWriter.WriteFullBeginTag("h4", False)
      mInternalWriter.Write(HeadingText)
      mInternalWriter.WriteEndTag("h4", False)
    End Sub

    Public Sub Heading5(HeadingText As String)
      mInternalWriter.WriteFullBeginTag("h5", False)
      mInternalWriter.Write(HeadingText)
      mInternalWriter.WriteEndTag("h5", False)
    End Sub

#End Region

    <System.Web.UI.PersistenceMode(UI.PersistenceMode.InnerProperty)> _
    Public Property Content As String = ""

    Protected Overrides Sub AddParsedSubObject(obj As Object)

      AddControl(obj)

    End Sub

    ''' <summary>
    ''' Loads the content in the Editable HTML content table / file.
    ''' </summary>
    Public Sub LoadContent(ContentName As String)
      Content = ContentEditing.GetContent(ContentName).HTMLContent
    End Sub

    Public Shadows Sub AddControl(Control As System.Web.UI.Control)
      If TypeOf Control Is HelperBase Then
        CType(Control, HelperBase).HelperAccessorsParent = Me.Helpers
        If Not CType(Control, HelperBase).IsSetup Then
          CType(Control, HelperBase).Setup()
        End If
        'mInternalWriter.Write(CType(Control, HelperBase).GetHTMLString)
      Else
        'CType(Control, System.Web.UI.Control).Visible = True - This doesnt work.
        CType(Control, System.Web.UI.Control).RenderControl(mInternalWriter)
        CType(Control, System.Web.UI.Control).Visible = False
      End If
    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      Writer.Write(mInternalWriter.ToString)
      Writer.WriteLine(Content.Trim)
    End Sub

  End Class

  Public Class StarRating(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property MaxPoints As Integer = 5
    Public Property MaxPointsJS As String = ""
    Public Property ToolTipDataSourceName As String
    Public Property ToolTipDataSource As String = "{}"

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AddBinding(KnockoutBindingString.StarRating, "{ MaxPoints: " & If(MaxPointsJS = "", MaxPoints, MaxPointsJS) & ", Value: " & GetForJS() & ", ToolTipDataSourceName: '" & ToolTipDataSourceName & "', ToolTipDataSource: " & ToolTipDataSource & "}")

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      WriteFullStartTag("div", TagType.Normal)
      Writer.WriteEndTag("div")
    End Sub
  End Class

  Public Class HTMLTag(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property Tag As String
    Public Property SelfClosing As Boolean = False

    Private mHTML As HTMLSnippet

    Public Property HTML As String
      Get
        If mHTML Is Nothing Then
          mHTML = Helpers.HTML
        End If
        Return mHTML.Content
      End Get
      Set(value As String)
        If mHTML Is Nothing Then
          mHTML = Helpers.HTML
        End If
        mHTML.Content = value
      End Set
    End Property


    Protected Friend Overrides Sub Render()
      MyBase.Render()

      WriteFullStartTag(Tag, If(SelfClosing, TagType.SelfClosing, TagType.Normal))

      RenderChildren()

      If Not SelfClosing Then
        Writer.WriteEndTag(Tag)
      End If
    End Sub

  End Class

  Public Class HTMLDiv(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Function ScrollableDiv(ByVal Height As String) As HTMLDiv(Of ObjectType)
      Me.Style("height") = Height
      Me.Style("overflow") = "scroll"
      Return Me
    End Function

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        WriteFullStartTag("div", TagType.IndentChildren)

        For Each child As Singular.Web.Controls.CustomWebControl In Controls
          child.Render()
        Next

        .WriteEndTag("div", True)
      End With

    End Sub

  End Class

  Public Class HTMLSpan(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer
        WriteFullStartTag("span", TagType.IndentChildren)
        For Each child As Singular.Web.Controls.CustomWebControl In Controls
          child.Render()
        Next
        .WriteEndTag("span", True)
      End With

    End Sub

  End Class

  Public Class Image(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    ''' <summary>
    ''' Alternate Text of the Image.
    ''' </summary>
    Public Property Alt As String = ""

    ''' <summary>
    ''' The path of the image. Can use ~ as the root of the site. Leave blank if using knockout src binding.
    ''' </summary>
    Public Property Src As String = ""

    Private mSrcDefined As DefinedImageType = DefinedImageType.None
    Public Property SrcDefined As DefinedImageType
      Get
        Return mSrcDefined
      End Get
      Set(value As DefinedImageType)
        If mSrcDefined <> value Then
          mSrcDefined = value
        End If
      End Set
    End Property

    Private mGlyph As FontAwesomeIcon = FontAwesomeIcon.None

    Public Property Glyph As FontAwesomeIcon
      Get
        Return mGlyph
      End Get
      Set(value As FontAwesomeIcon)
        If AllowFontAwesomeGlyphs Then
          mGlyph = value
        End If
      End Set
    End Property

    Public ReadOnly Property HasSrc As Boolean
      Get
        Return Src <> "" OrElse mSrcDefined <> DefinedImageType.None OrElse mGlyph <> FontAwesomeIcon.None
      End Get
    End Property

    ''' <summary>
    ''' Sets the image to be pending/  authorised / rejected based on the value of the property. Uses default singular images.
    ''' </summary>
    Public Sub AuthorisationIcon(AuthPropertyName As String)
      AddBinding(KnockoutBindingString.src, "Singular.GetAuthImage(" & AuthPropertyName & ")")
      AddBinding(KnockoutBindingString.title, "Singular.GetAuthTT(" & AuthPropertyName & ")")
    End Sub

    ''' <summary>
    ''' Sets the Image to get as a document in the documents table. The property is the property on this object that contains the hash.
    ''' </summary>
    Public Sub HashSrc(PropertyOrPath As String, Optional MissingPath As String = "")

      If MissingPath.StartsWith("~") Then
        MissingPath = Utils.URL_ToAbsolute(MissingPath)
      End If

      AddBinding(Singular.Web.KnockoutBindingString.src,
                                 "(" & PropertyOrPath & " ? '" & Utils.URL_ToAbsolute("~/Library/FileDownloader.ashx") & "?DocumentHash=' + " & PropertyOrPath & " : '" & MissingPath & "')")
      AddBinding(KnockoutBindingString.visible, PropertyOrPath)
    End Sub

    Public Sub FromDocumentsTable(DocumentIDPropertyName As String)
      AddBinding(Singular.Web.KnockoutBindingString.src, "Singular.GetImage(" & DocumentIDPropertyName & ")")
    End Sub

    Public Sub FromDocumentsTable(DocumentIDPropertyName As System.Linq.Expressions.Expression(Of Func(Of ObjectType, Object)))
      FromDocumentsTable(CType(BindingManager, KnockoutBindingManager(Of ObjectType)).CreateBindingExpression(KnockoutBindingString.src, DocumentIDPropertyName))
    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        If mGlyph <> FontAwesomeIcon.None OrElse SrcDefined <> DefinedImageType.None Then

          If mGlyph <> FontAwesomeIcon.None Then
            AddClass("fa")
            AddClass("fa-" & Singular.Reflection.GetEnumDisplayName(mGlyph))

          ElseIf SrcDefined <> DefinedImageType.None Then
            AddClass("ImgIcon")
            Style("background-position") = -(SrcDefined Mod 4) * 16 & "px " & -Math.Floor(SrcDefined / 4) * 16 & "px"

          End If
          WriteFullStartTag("span", TagType.Normal)
          .WriteEndTag("span")

        Else
          Style("vertical-align") = "middle"
          If Src.Contains("~") Then
            Attributes.Add("src", Utils.URL_ToAbsolute(Src))
          ElseIf Src.Length > 0 Then
            Attributes.Add("src", Src)
          End If

          If Alt <> "" Then
            Attributes.Add("alt", Alt)
          End If

          WriteFullStartTag("img", TagType.SelfClosing)

        End If


      End With

    End Sub

  End Class

  ''' <summary>
  ''' Renders an HTML Button with the specified Text.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class Button(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mButtonType As DefinedButtonType = DefinedButtonType.General

    ''' <summary>
    ''' The Text that will be displayed to the user.
    ''' </summary>
    Public Shadows Property Text As String = "Submit"

    ''' <summary>
    ''' The Buttons ID and Name which will be returned when it is clicked. Leave this blank if you don't want the button to cause post back.
    ''' </summary>
    Public Property ButtonID As String = "Submit"

    ''' <summary>
    ''' True if the page must not post back if Rules are broken on the client.
    ''' </summary>
    Public Property Validate As Boolean = False

    Public Property Image As New Image(Of ObjectType)

    Public Property ButtonText As HTMLTag(Of ObjectType)

    Public Property ImageRightAligned As Boolean = False

    ''' <summary>
    ''' If true, will mark this as a submit button, which will be pressed when the user pressed enter
    ''' </summary>
    Public Property ClickOnEnterKey As Boolean = False

    ''' <summary>
    ''' Text to warn the user they are about to do something. E.g. PromptText = "Are you sure?"
    ''' </summary>
    Public Property PromptText As String = ""

    ''' <summary>
    ''' Indicates what will happen when the button is clicked on the browser.
    ''' If PostBackType = Ajax, then set the Argument, Validate and LoadingText properties
    ''' If PostBackType = Full, then set the Argument and Validate properties
    ''' </summary>
    Public Property PostBackType As PostBackType = DefaultButtonPostBackType

    ''' <summary>
    ''' When PostBackType is set to Ajax, this is the text that will apear while the async post back is in progress.
    ''' </summary>
    Public Property LoadingText As String = ""

    ''' <summary>
    ''' The Predefined Type of Button. 
    ''' </summary>
    Public Property ButtonType As DefinedButtonType
      Get
        Return mButtonType
      End Get
      Set(value As DefinedButtonType)
        If mButtonType <> value Then
          mButtonType = value
          If mButtonType <> DefinedButtonType.General Then
            Select Case ButtonType
              Case DefinedButtonType.Save
                If Singular.Web.Controls.DefaultButtonStyle = Singular.Web.ButtonStyle.Bootstrap Then
                  Image.Glyph = FontAwesomeIcon.save
                Else
                  Image.SrcDefined = DefinedImageType.Disk
                End If
                Validate = True
                ButtonStyle = ButtonMainStyle.Success
              Case DefinedButtonType.Find
                If Singular.Web.Controls.DefaultButtonStyle = Singular.Web.ButtonStyle.Bootstrap Then
                  Image.Glyph = FontAwesomeIcon.search
                Else
                  Image.SrcDefined = DefinedImageType.Find
                End If
              Case DefinedButtonType.Undo
                If Singular.Web.Controls.DefaultButtonStyle = Singular.Web.ButtonStyle.Bootstrap Then
                  Image.Glyph = FontAwesomeIcon.undo
                Else
                  Image.SrcDefined = DefinedImageType.Undo
                End If
              Case DefinedButtonType.New
                If Singular.Web.Controls.DefaultButtonStyle = Singular.Web.ButtonStyle.Bootstrap Then
                  Image.Glyph = FontAwesomeIcon.file
                Else
                  Image.SrcDefined = DefinedImageType.BlankPage
                End If
              Case DefinedButtonType.Export
                If Singular.Web.Controls.DefaultButtonStyle = Singular.Web.ButtonStyle.Bootstrap Then
                  Image.Glyph = FontAwesomeIcon.grid9
                Else
                  Image.SrcDefined = DefinedImageType.OpenFile
                End If
              Case DefinedButtonType.Cancel
                If Singular.Web.Controls.DefaultButtonStyle = Singular.Web.ButtonStyle.Bootstrap Then
                  Image.Glyph = FontAwesomeIcon.close
                Else
                  Image.SrcDefined = DefinedImageType.No_RedCross
                End If
                ButtonStyle = ButtonMainStyle.Danger
            End Select
          End If
        End If
      End Set
    End Property

    Public Property ButtonStyle As ButtonMainStyle = ButtonMainStyle.Primary
    Public Property ButtonSize As ButtonSize = DefaultButtonSize
    Public Property RenderMode As ButtonStyle = DefaultButtonStyle

    ''' <summary>
    ''' Adds a binding that will add a new object to the specified list when the control is clicked.
    ''' </summary>
    Public Sub AddAddBinding(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
      Bindings.AddAddBinding(ListProperty)
    End Sub

    Public Sub AddClearBinding(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
      Bindings.AddClearBinding(Singular.Reflection.GetMember(Of ObjectType)(ListProperty))
    End Sub

    ''' <summary>
    ''' Adds a binding that will remove the current item from its parent list.
    ''' </summary>
    Public Sub AddRemoveBinding()
      Dim h As IBindingControl = GetParentWithContext()
      If h Is Nothing OrElse h.PropertyInfo Is Nothing Then
        Throw New Exception("Cannot add a remove binding to a control that has no binding context parent.")
      End If
      AddBinding(KnockoutBindingString.click, "$parent." & GetParentWithContext.PropertyInfo.Name & ".Remove($data, arguments[1])")
    End Sub

    Public WriteOnly Property Click As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Set(value As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
        Bindings.Add(KnockoutBindingString.click, value)
      End Set
    End Property

    Private _ClickJS As String
    Public WriteOnly Property ClickJS As String
      Set(value As String)
        _ClickJS = value
        'Bindings.Add(KnockoutBindingString.click, value)
      End Set
    End Property

    Public Sub MakeStateButton(CheckedState As String, UncheckedState As String, ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), Optional ByVal SelectedCssClass As String = "", Optional ByVal UnSelectedCssClass As String = "", Optional ByVal IconName As String = "")

      Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
      Dim PropValue As String = Prop.Name & "()"
      ButtonText.AddBinding(KnockoutBindingString.html, PropValue & " ? '" & CheckedState & "' : '" & UncheckedState & "'")
      Me.AddBinding(KnockoutBindingString.click, Prop.Name & "(!" & Prop.Name & "())")
      Me.AddBinding(KnockoutBindingString.css, PropValue & " ? '" & SelectedCssClass & "' : '" & UnSelectedCssClass & "'")

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AddControl(Image)
      ButtonText = Helpers.HTMLTag("span", Text)

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      If Not String.IsNullOrEmpty(_ClickJS) Then AddBinding(KnockoutBindingString.click, _ClickJS)

      If RenderMode = Singular.Web.ButtonStyle.JQuery Then
        AddClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")

        If ButtonSize = Singular.Web.ButtonSize.ExtraSmall Then
          AddClass("SmallButton")
        End If
      Else
        AddClass("btn")

        If ButtonStyle <> ButtonMainStyle.NoStyle Then
          AddClass("btn-" & ButtonStyle.ToString.ToLower)
        End If

        Select Case ButtonSize
          Case Singular.Web.ButtonSize.ExtraSmall
            AddClass("btn-xs")
          Case Singular.Web.ButtonSize.Small
            AddClass("btn-sm")
          Case Singular.Web.ButtonSize.Large
            AddClass("btn-lg")
          Case Singular.Web.ButtonSize.Tiny
            AddClass("btn-tiny")
        End Select

      End If

      If ImageRightAligned Then
        AddClass("ImgRight")
      End If

      If Not Bindings.ContainsKey(Singular.Web.KnockoutBindingString.click) Then
        If PostBackType = PostBackType.Ajax Then
          AddBinding(KnockoutBindingString.click, "Singular.ButtonClickAjax(arguments[1])")
        End If
      End If


      Dim TagType As TagType
      If ButtonType = DefinedButtonType.General Then
        TagType = HelperControls.TagType.Normal
      Else
        TagType = HelperControls.TagType.IndentChildren
      End If

      If ButtonID <> "" Then
        Attributes.Add("id", ButtonID)
        Attributes.Add("name", ButtonID)
      End If

      If (ButtonID <> "" And PostBackType = PostBackType.Full) OrElse ClickOnEnterKey Then
        Attributes.Add("type", "submit")
      Else
        Attributes.Add("type", "button")
      End If

      If ButtonID <> "" AndAlso PostBackType = PostBackType.Full AndAlso Not Bindings.ContainsKey(Singular.Web.KnockoutBindingString.click) Then
        AddBinding(KnockoutBindingString.click, "Singular.ButtonClickPost(arguments[1])")
      End If

      If Validate Then
        Attributes.Add("data-validate", "PreventPost")
      End If

      If PromptText <> "" Then
        Attributes.Add("data-prompt", PromptText)
      End If

      If LoadingText <> "" Then
        Attributes.Add("data-loadText", LoadingText)
      End If

      WriteFullStartTag("button", TagType)

      If RenderMode = Singular.Web.ButtonStyle.JQuery Then
        Writer.Write("<span class=""ui-button-text"">")
      End If

      If (Image.HasSrc OrElse Image.Bindings.Count > 0) AndAlso Not ImageRightAligned Then
        Image.Render()
      End If

      'Writer.Write(" " & Text)
      ButtonText.Render()

      'If ButtonType <> DefinedButtonType.General Then
      '  Writer.WriteLine()
      'End If

      If (Image.HasSrc) AndAlso ImageRightAligned Then
        Writer.Write(" ")
        Image.Render()
      End If

      If RenderMode = Singular.Web.ButtonStyle.JQuery Then
        Writer.Write("</span>")
      End If

      Writer.WriteEndTag("button", ButtonType <> DefinedButtonType.General)

    End Sub

  End Class

  ''' <summary>
  ''' Renders JQuery Tabs.
  ''' </summary>
  Public Class TabControl(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property TabHolderName As String
    Public Property TabIndexProperty As System.Linq.Expressions.Expression(Of Func(Of ObjectType, Object))
    Public Property SelectedTabIndex As Integer = 0

    Private AddIndex As Integer = 0

    ''' <summary>
    ''' Add a tab to the tab container.
    ''' </summary>
    ''' <param name="TabText">The text of the tab</param>
    ''' <param name="TabKey">The key if you are using SelectedTabIndex. Not required.</param>
    Public Function AddTab(TabText As String, Optional TabKey As String = "") As Tab

      Dim t As New Tab With {.TabName = TabText, .TabIndex = AddIndex, .TabKey = TabKey}
      AddControl(t)
      AddIndex += 1
      Return t

    End Function

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If TabIndexProperty IsNot Nothing Then
        mLinqExpression = TabIndexProperty
        AddBinding(KnockoutBindingString.tabs, TabIndexProperty)
      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      'AddClass("TabControl")
      If TabIndexProperty Is Nothing Then
        Attributes("data-TabControl") = SelectedTabIndex
      End If

      WriteFullStartTag("div", TagType.IndentChildren)


      Writer.WriteFullBeginTag("ul", True)

      'Tab Headers
      For Each t As Tab In Controls
        t.RenderHeader()
      Next

      Writer.WriteEndTag("ul", True)

      'Tabs
      Dim Index As Integer = 0
      For Each t As Tab In Controls
        t.Render()
        Index += 1
      Next

      Writer.WriteEndTag("div", True)

    End Sub

    ''' <summary>
    ''' Renders a single tab within the Tab holder.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Tab
      Inherits HelperBase(Of ObjectType)

      Public Property TabName As String
      ''' <summary>
      ''' If the tab container is bound to a selected tab index property, the value of TabKey will be set in the sub property 'TabKey'
      ''' </summary>
      Public Property TabKey As String
      Friend TabIndex As Integer

      Public Shadows ReadOnly Property Parent As TabControl(Of ObjectType)
        Get
          Return MyBase.Parent
        End Get
      End Property

      Private mHeaderBindings As New KnockoutBindingManager(Of ObjectType)(Me, Nothing)
      Public ReadOnly Property HeaderBindings As KnockoutBindingManager(Of ObjectType)
        Get
          Return mHeaderBindings
        End Get
      End Property

      Private mHeaderStyle As New CSSStyle
      Public ReadOnly Property HeaderStyle As CSSStyle
        Get
          Return mHeaderStyle
        End Get
      End Property

      Private mLIStyle As New CSSStyle
      Public ReadOnly Property LI_Style As CSSStyle
        Get
          Return mLIStyle
        End Get
      End Property

      Public Function LateBind() As Singular.Web.Controls.HelperControls.HelperAccessors(Of ObjectType)
        Return Helpers.If(Parent.GetForJS & "() == " & TabIndex).Helpers
      End Function

      Friend Sub RenderHeader()
        Writer.WriteBeginTag("li")
        WriteStyles(mLIStyle)
        If Not String.IsNullOrEmpty(Me.Tooltip) Then
          Writer.WriteAttribute("title", Me.Tooltip)
        End If
        ' Writer.WriteAttribute("
        Writer.WriteAttribute("data-tab-key", If(String.IsNullOrEmpty(TabKey), TabIndex, TabKey))
        Writer.WriteCloseTag(False)

        Writer.WriteBeginTag("a")
        Writer.WriteAttribute("href", "#" & TabName.Replace(" ", ""))
        mHeaderBindings.WriteKnockoutBindings()
        WriteStyles(mHeaderStyle)

        Writer.WriteCloseTag(False)
        Writer.Write(TabName)
        Writer.WriteEndTag("a")

        Writer.WriteEndTag("li")
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()

        Writer.WriteBeginTag("div")
        Writer.WriteAttribute("id", TabName.Replace(" ", ""))
        Bindings.WriteKnockoutBindings()
        Writer.WriteCloseTag(True)

        For Each ctl As HelperBase In Controls
          ctl.Render()
        Next

        Writer.WriteEndTag("div", True)

      End Sub

    End Class

  End Class

  Public Class Link
    Public Shared Function PopupWindowLink(Link As String, LinkText As String, Optional Width As Integer = 400, Optional Height As Integer = 300) As String
      Return "<a href='javascript: void(0)' onclick=""window.open('" & Link & "', 'mywin', 'width=" & Width & ", height=" & Height & ", status=no, resizable=yes, menubar=no, scrollbars=yes')"" >" & LinkText & "</a>"
    End Function

    Public Shared Function NormalLink(Link As String, LinkText As String, Optional NewWindow As Boolean = False) As String
      If Link.StartsWith("~") Then
        Link = Utils.URL_ToAbsolute(Link)
      End If
      If NewWindow Then
        Return "<a href='" & Link & "', target='_blank'>" & LinkText & "</a>"
      Else
        Return "<a href='" & Link & "'>" & LinkText & "</a>"
      End If
    End Function
  End Class

  Public Class Link(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    ''' <summary>
    ''' if you want to call a js function, specify it with the brackets. e.g. OnLinkClick()
    ''' </summary>
    Public Property Href As String = ""
    Public Property LinkText As String = ""

    Public Property HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    Public Property LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))

    Public Property Target As LinkTargetType = LinkTargetType.NotSet

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If HRefExpression IsNot Nothing Then
        AddBinding(KnockoutBindingString.href, HRefExpression)
      End If
      If LinkTextExpression IsNot Nothing Then
        AddBinding(KnockoutBindingString.text, LinkTextExpression)
      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      If Not String.IsNullOrEmpty(Href) Then
        If Href.Contains("(") AndAlso Href.Contains(")") AndAlso Not Href.Contains("?") Then
          AddBinding(KnockoutBindingString.click, Href & "; return false")
          Href = "#"
        End If

        If Href.StartsWith("~") Then
          Href = Utils.URL_ToAbsolute(Href)
        End If
        Attributes("href") = Href
      End If

      If Target <> LinkTargetType.NotSet Then
        Attributes("target") = Target.ToString
      End If

      WriteFullStartTag("a", TagType.Normal)
      Writer.Write(LinkText)

      RenderChildren()

      Writer.WriteEndTag("a")

    End Sub

  End Class

  Public Class Toolbar(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mValidationDiv As HTMLDiv(Of ObjectType)
    Public Property AddValidation As Boolean

    Public Sub New(AddValidation As Boolean)
      Me.AddValidation = AddValidation
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AddClass("DivToolbar")

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      If AddValidation Then
        mValidationDiv = Helpers.Div
        mValidationDiv.AddClass("ValidationPopup")
        mValidationDiv.AddClass("Msg-Validation")
        mValidationDiv.Attributes("data-validation-summary") = "2"
      End If

      WriteFullStartTag("div", TagType.IndentChildren)

      RenderChildren()

      Writer.WriteEndTag("div", True)

    End Sub


  End Class

  Public Class NoContainer(Of ObjectType)
    Inherits NoContainer(Of ObjectType, ObjectType)

  End Class

  Public Class NoContainer(Of ObjectType, ChildType)
    Inherits HelperBase(Of ObjectType, ChildType)

    Public Property BindingType As KnockoutBindingString
    Public Property JSFunction As String = ""
    Public Property Condition As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))


    Public Sub New()
      mRequiresProperty = False
    End Sub

    Public Sub MakeTemplate(TemplateID As String, Optional DataContext As String = "")
      DataContext = If(DataContext = "", GetForJS(), DataContext)
      Dim Binding = "{ name: '" & TemplateID & "', 'if': " & DataContext & ", data: " & DataContext & " }"
      BindingType = KnockoutBindingString.template
      AddBinding(KnockoutBindingString.template, Binding)
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If JSFunction <> "" Then
        AddBinding(BindingType, JSFunction)
      Else
        AddBinding(BindingType, GetForJS)
      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        Dim BindString As String
        If Condition Is Nothing Then
          BindString = Bindings.GetBinding(BindingType)
        Else
          BindString = "{ 'if': " & Singular.Web.Controls.KnockoutBindingManager(Of ObjectType).CreateBindingExpression(Singular.Web.KnockoutBindingString.if, Condition, False) & ", data: " & GetForJS() & ", afterRender: Singular.AfterTemplateRender }"
        End If

        .Write("<!-- ko ")
        .Write("template: " & BindString)
        .Write(" -->")
        .AddLevel()
        RenderChildren()
        .RemoveLevel()
        .WriteLine("<!-- /ko -->")

      End With

    End Sub

  End Class

  Public Class ImageWithPopup(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property Title As String
    Public Property InnerText As String

    Private mImage As Image(Of ObjectType)
    Private mInnerDiv As HTMLDiv(Of ObjectType)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers.Div
        .AddClass("Popup")
        .Attributes("data-popup") = Title

        mImage = .Helpers.Image(, DefinedImageType.Help)
        mImage.AddClass("PopupImage")
        mImage.Tooltip = "What is this?"

        mInnerDiv = .Helpers.Div
        mInnerDiv.Helpers.HTML(InnerText)
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

  Public Class Dialog(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property PopupCondition As String
    Public Property Title As String
    Public Property OnCloseFunctionName As String
    Public Property AllowResize As Boolean = True
    Public Property WidthBinding As String = ""
    Public Property OnOpenFunctionName As String

    Public Function AddConfirmationButtons(YesText As String, YesJS As String, NoText As String) As ConfirmationButtons(Of ObjectType)
      Dim cb As New ConfirmationButtons(Of ObjectType)(YesText, YesJS, NoText, OnCloseFunctionName & "()")
      Helpers.Control(cb)
      Return cb
    End Function

    Public Function AddConfirmationButtons(Of Type)(Helpers As HelperAccessors(Of Type), YesText As String, YesJS As String, NoText As String) As ConfirmationButtons(Of Type)
      Dim cb As New ConfirmationButtons(Of Type)(YesText, YesJS, NoText, OnCloseFunctionName & "()")
      Helpers.Control(cb)
      Return cb
    End Function

    ''' <summary>
    ''' Adds a hidden element which will get focus when the dialog is opened. Use this if you don't want the first actual control to be focused.
    ''' </summary>
    Public Sub AddHiddenFocusElement()
      Helpers.HTMLTag("input").AddClass("ui-helper-hidden-accessible")
    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      AddClass("Dialog")
      Bindings.AddDialogBinding(PopupCondition, Title, OnCloseFunctionName, AllowResize, WidthBinding, OnOpenFunctionName)

      WriteFullStartTag("div", TagType.IndentChildren)

      RenderChildren()

      Writer.WriteEndTag("div", True)

    End Sub

  End Class

  Public Class ConfirmationButtons(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property YesButton As Button(Of ObjectType)
    Public Property NoButton As Button(Of ObjectType)

    Private mYesText As String
    Private mYesOnClick As String
    Private mCancelText As String
    Private mCancelOnClick As String

    Public Sub New(YesText As String, YesOnClick As String, CancelText As String, CancelOnClick As String)
      mYesText = YesText
      mYesOnClick = YesOnClick
      mCancelText = CancelText
      mCancelOnClick = CancelOnClick
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If mYesOnClick <> "" Then
        YesButton = Helpers.Button(DefinedButtonType.Save, mYesText)
        YesButton.AddBinding(KnockoutBindingString.click, mYesOnClick)
        YesButton.ButtonSize = ButtonSize.Small
      End If
      If mCancelText <> "" Then
        NoButton = Helpers.Button(DefinedButtonType.Cancel, mCancelText)
        NoButton.AddBinding(KnockoutBindingString.click, mCancelOnClick)
        NoButton.ButtonSize = ButtonSize.Small
      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      AddClass("ConfButtons")
      WriteFullStartTag("div", TagType.IndentChildren)
      RenderChildren()
      Writer.WriteEndTag("div", True)

    End Sub

  End Class


  Public Class NullControl(Of ObjectType, ChildType)
    Inherits HelperBase(Of ChildType)

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()

    End Sub

  End Class

  Public Class AjaxControlLoader
    Inherits HelperBase

    Public Class CacheInfo
      Public Property HashCode As String
      Public Property Cache As CachedHelper(Of Object)
    End Class

    Private mContainer As ContainerType

    Public Shared KeyLookup As New Dictionary(Of String, CacheInfo)
    Public Shared RenderedControls As New Hashtable

    Public Sub New(ControlType As Type, Optional Container As ContainerType = ContainerType.Div, Optional ID As String = "", Optional Helpers As HelperAccessors = Nothing, Optional ExtraKeyInfo As Func(Of String) = Nothing)

      If Container = ContainerType.Script Then
        Attributes.Add("type", "text/html")
      End If
      If ID <> "" Then
        Attributes.Add("id", ID)
      End If
      mContainer = Container

      Dim Key As String = ControlType.FullName & "__" & Singular.Localisation.CurrentCulture.TwoLetterISOLanguageName

      If ExtraKeyInfo IsNot Nothing Then Key &= "__" & ExtraKeyInfo()

      CalculateHash(Key, Helpers, ControlType)

    End Sub

    Private Sub CalculateHash(Key As String, Helpers As HelperAccessors, ControlType As Type)

      Dim CacheInfo As CacheInfo = Nothing

      'Get the hash code for this control type.
      If Not KeyLookup.TryGetValue(Key, CacheInfo) Then

        CacheInfo = New CacheInfo
        If Helpers IsNot Nothing Then
          CacheInfo.Cache = New CachedHelper(Of Object)(Helpers.IPage)
          Helpers.SetCached(CacheInfo.Cache)
        End If

        Dim Control As HelperBase = Activator.CreateInstance(ControlType, True)
        If Not Control.IsSetup Then
          If Helpers IsNot Nothing Then
            Control.SetupInternal(Helpers)
          Else
            Control.Setup()
          End If
        End If

        Dim ControlHtml As String = Control.GetHTMLString
        CacheInfo.HashCode = Singular.Encryption.GetStringHash(ControlHtml, Encryption.HashType.MD5, False, Encryption.ASCIIEncoding.ASCII)
        SyncLock KeyLookup
          KeyLookup(Key) = CacheInfo
          RenderedControls(CacheInfo.HashCode) = ControlHtml
        End SyncLock
      End If

      If CacheInfo.Cache IsNot Nothing Then
        CacheInfo.Cache.SetupPage(Helpers.IPage)
      End If

      Attributes.Add("data-AsyncContent", CacheInfo.HashCode)

    End Sub

    Public Shared Function GetHTML(Hash As String) As String
      Return RenderedControls(Hash)
    End Function

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      WriteFullStartTag(mContainer.ToString.ToLower, TagType.Normal)
      Writer.WriteEndTag(mContainer.ToString.ToLower)
    End Sub

  End Class

  Public Class ContextMenu
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of Object)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers.Div
        .AddClass("S-CtxMenu")
        .Bindings.Add(Singular.Web.KnockoutBindingString.style, "{ left: Left() + 'px', top: Top() + 'px' }")

        'Left color strip 
        .Helpers.Div.AddClass("LeftStrip")

        With .Helpers.Div
          .Attributes("data-bind") = "foreach: Items"

          'if its a menu item
          With .Helpers.Div
            .AddClass("S-MenuItem")
            .Attributes("data-bind") = "visible: $data.Text, css: ($data.Selectable == undefined || $data.Selectable) ? 'Selectable' : '', event: { mouseenter: function (d, e) { $parent.ItemHover($data, e); }, click: function (i, e) { $parent.ItemClick($data, e) } }"

            'Icon
            With .Helpers.HTMLTag("span")
              '.AddClass("Icon")
              .Attributes("data-bind") = "css: Singular.CMIconClass($data), style: Singular.CMIconStyle($data)"
            End With

            'Text / template
            With .Helpers.Div
              .AddBinding(KnockoutBindingString.style, "{ 'margin-right': $parent.HasChildren() ? '16px' : '' }")
              .Style.Display = Display.inlineblock

              'Text
              With .Helpers.Div
                .Attributes("data-bind") = "'if': $data.TemplateID == undefined"
                With .Helpers.HTMLTag("span")
                  .Style.Display = Display.inlineblock
                  .Style("margin") = "2px 0"
                  .AddBinding(KnockoutBindingString.html, "Text")
                End With
              End With

              'Template
              With .Helpers.Div
                .Attributes("data-bind") = "template: { 'if': $data.TemplateID, name: $data.TemplateID }"
              End With
            End With

            'Right arrow
            With .Helpers.HTMLTag("span")
              .AddClass("fa fa-angle-double-right")
              .Style("margin") = "0"
              .Style("position") = "absolute"
              .Style("top") = "3px"
              .Style("right") = "0"
              .Style("font-size") = "14px"
              .Attributes("data-bind") = "visible: $data.Items != undefined"
            End With
          End With

          'if its a seperator
          With .Helpers.Div
            .AddClass("S-MenuBreak")
            .Attributes("data-bind") = "visible: $data.Break"
          End With
        End With

      End With

      'Sub menu template
      With Helpers.Div
        .Attributes("data-bind") = "template: {'if': $data.SubMenu, name: 'SMenuTemplate', data: $data.SubMenu, afterRender: $data.FadeIn }"
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()

    End Sub

  End Class

  Public Class Template(Of ObjectType, ChildControlObjectType)
    Inherits HelperBase(Of ObjectType, ChildControlObjectType)

    Public Property IDString As String = "template"

    Public Sub New(IDString As String)
      Me.IDString = IDString
      Attributes.Add("type", "text/html")
      Attributes.Add("charset", "utf-8")
      Attributes.Add("id", IDString)
    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        WriteFullStartTag("script", TagType.IndentChildren)

        RenderChildren()

        .WriteEndTag("script", True)

      End With


    End Sub

  End Class

  Public Class DateRange(Of ObjectType)
    Inherits HTMLTag(Of ObjectType)

    Public Property StartDateFunction As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing
    Public Property EndDateFunction As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing
    Public Property StartDateJS As String = ""
    Public Property EndDateJS As String = ""
    Public Property InputTag As HTMLTag(Of ObjectType)
    Private Property ApplyOnMenuSelect As Boolean = False
    Private Property OnCloseJSFunction As String = ""
    Private Property AfterRangeChangedJSFunction As String = ""
    Private Property Bootstrap As Boolean = False
    Private Property BootstrapButtonCss As String = ""
    Private Property IconName As String = ""
    Private Property InitialText As String = "Select Date Range..."

    Public Sub New(StartDateExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                   EndDateExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                   Optional ApplyOnMenuSelect As Boolean = True,
                   Optional OnCloseJSFunction As String = "",
                   Optional AfterRangeChangedJSFunction As String = "",
                   Optional InitialText As String = "Select Date Range...",
                   Optional Bootstrap As Boolean = False,
                   Optional BootstrapButtonCss As String = "",
                   Optional IconName As String = "")
      Me.StartDateFunction = StartDateExpression
      Me.EndDateFunction = EndDateExpression
      Me.ApplyOnMenuSelect = ApplyOnMenuSelect
      Me.OnCloseJSFunction = OnCloseJSFunction
      Me.AfterRangeChangedJSFunction = AfterRangeChangedJSFunction
      Me.Bootstrap = Bootstrap
      Me.BootstrapButtonCss = BootstrapButtonCss
      Me.IconName = IconName
      Me.InitialText = InitialText
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()
      If StartDateFunction IsNot Nothing Then
        StartDateJS = GetJS(StartDateFunction)
      End If
      If EndDateFunction IsNot Nothing Then
        EndDateJS = GetJS(EndDateFunction)
      End If
      InputTag = Helpers.HTMLTag("div")
      'InputTag.Attributes("type") = "button"
      InputTag.AddBinding(KnockoutBindingString.DateRangeValue,
                          "{ " & _
                            "StartDate: " & StartDateJS & ", EndDate: " & EndDateJS & _
                            IIf(Me.OnCloseJSFunction <> "", ", OnCloseJSFunction:" & Me.OnCloseJSFunction, "OnCloseJSFunction: null") & _
                            IIf(Me.AfterRangeChangedJSFunction <> "", ", AfterRangeChangedJSFunction:" & Me.AfterRangeChangedJSFunction, "AfterRangeChangedJSFunction: null") & _
                            ", Bootstrap: " & Me.Bootstrap.ToString.ToLower & _
                            ", BootstrapButtonCss: '" & Me.BootstrapButtonCss & "'" & _
                            ", IconName: '" & Me.IconName & "'" & _
                            ", InitialText: '" & Me.InitialText & "'" & _
                          "}")
    End Sub

    Private Function GetJS(Func As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As String
      Dim Output = Singular.Linq.JavascriptExpressionParser(Of ObjectType).GetCachedOutput(Func, Linq.OutputModeType.KnockoutBinding, True)
      Return Output.JavascriptString
    End Function

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      'Writer.WriteBeginTag("input")
      'Writer.WriteAttribute("value", "Select Date Range...")
      'Writer.WriteEndTag("input")
    End Sub
  End Class

  Public Class Slider(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property MinValue As Decimal = 0
    Public Property MaxValue As Decimal = 10
    Public Property StepLength As Decimal = 1

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AddBinding(KnockoutBindingString.Slider, GetForJS() & ", sliderOptions: {min: " & MinValue & ", max: " & MaxValue & ", range: false, step: " & StepLength & "}")

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      WriteFullStartTag("div", TagType.Normal)
      Writer.WriteEndTag("div")
    End Sub
  End Class

  Public Class LoadingOverlay(Of ObjectType)
    Inherits HTMLDiv(Of ObjectType)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AddClass("LoadingOverlay")
      Style.Position = Position.absolute

      With Helpers.Div
        .AddClass("Outer")

        With .Helpers.Div
          .AddClass("Inner")

          .Helpers.HTMLTag("span", "Loading...").AddClass("Loading")
        End With
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

    End Sub

  End Class

End Namespace

