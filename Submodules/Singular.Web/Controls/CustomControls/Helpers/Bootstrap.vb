Imports System.Reflection
Imports System.Dynamic
Imports System.Linq.Expressions
Imports Singular.Web.Controls.HelperControls
Imports System.ComponentModel.DataAnnotations
Imports Singular.Web.Controls
Imports System.ComponentModel

Namespace CustomControls

  Public Class Bootstrap

    Private Shared Function GetPanelClass(Style As BootstrapEnums.Style) As String

      Select Case Style
        Case BootstrapEnums.Style.DefaultStyle
          Return "panel-default"
        Case BootstrapEnums.Style.Primary
          Return "panel-primary"
        Case BootstrapEnums.Style.Success
          Return "panel-success"
        Case BootstrapEnums.Style.Info
          Return "panel-info"
        Case BootstrapEnums.Style.Warning
          Return "panel-warning"
        Case BootstrapEnums.Style.Danger
          Return "panel-danger"
      End Select

      '.Custom
      Return ""

    End Function

    Private Shared Function GetButtonClass(Style As BootstrapEnums.Style, Optional CustomStyle As String = "") As String

      Select Case Style
        Case BootstrapEnums.Style.DefaultStyle
          Return "btn-default"
        Case BootstrapEnums.Style.Primary
          Return "btn-primary"
        Case BootstrapEnums.Style.Success
          Return "btn-success"
        Case BootstrapEnums.Style.Info
          Return "btn-info"
        Case BootstrapEnums.Style.Warning
          Return "btn-warning"
        Case BootstrapEnums.Style.Danger
          Return "btn-danger"
        Case BootstrapEnums.Style.Custom
          Return CustomStyle
      End Select

      Return ""

    End Function

    Private Shared Function GetButtonSizeClass(Style As BootstrapEnums.ButtonSize, Optional CustomSize As String = "") As String

      Select Case Style
        Case BootstrapEnums.ButtonSize.ExtraSmall
          Return "btn-xs"
        Case BootstrapEnums.ButtonSize.Small
          Return "btn-sm"
        Case BootstrapEnums.ButtonSize.Medium
          Return "btn-md"
        Case BootstrapEnums.ButtonSize.Large
          Return "btn-lg"
        Case BootstrapEnums.ButtonSize.Custom
          Return CustomSize
      End Select

      Return ""

    End Function

    Private Shared Function GetLabelClass(Style As BootstrapEnums.Style, Optional CustomStyle As String = "") As String

      Select Case Style
        Case BootstrapEnums.Style.DefaultStyle
          Return "label-default"
        Case BootstrapEnums.Style.Primary
          Return "label-primary"
        Case BootstrapEnums.Style.Success
          Return "label-success"
        Case BootstrapEnums.Style.Info
          Return "label-info"
        Case BootstrapEnums.Style.Warning
          Return "label-warning"
        Case BootstrapEnums.Style.Danger
          Return "label-danger"
        Case BootstrapEnums.Style.Custom
          Return CustomStyle
      End Select

      Return ""

    End Function

    Private Shared Function GetFDTileColorClass(Style As BootstrapEnums.FDTileColor, Optional CustomStyle As String = "") As String

      Select Case Style
        Case BootstrapEnums.FDTileColor.Green
          Return "tile-green"
        Case BootstrapEnums.FDTileColor.Lemon
          Return "tile-lemon"
        Case BootstrapEnums.FDTileColor.Red
          Return "tile-red"
        Case BootstrapEnums.FDTileColor.Blue
          Return "tile-blue"
        Case BootstrapEnums.FDTileColor.Orange
          Return "tile-orange"
        Case BootstrapEnums.FDTileColor.Prusia
          Return "tile-prusia"
        Case BootstrapEnums.FDTileColor.Concrete
          Return "tile-concrete"
        Case BootstrapEnums.FDTileColor.Purple
          Return "tile-purple"
      End Select

      '.Custom
      Return ""

    End Function

    Public Shared Function GetInputSizeClass(Size As BootstrapEnums.InputSize, Optional CustomSize As String = "") As String

      Select Case Size
        Case BootstrapEnums.InputSize.Small
          Return "input-sm"
        Case BootstrapEnums.InputSize.Large
          Return "input-lg"
        Case BootstrapEnums.InputSize.ExtraSmall
          Return "input-xs"
        Case BootstrapEnums.InputSize.Custom
          Return CustomSize
      End Select

      Return ""

    End Function

    Private Shared Function GetFlatDreamAlertColorClass(Style As BootstrapEnums.FlatDreamAlertColor, Optional CustomStyle As String = "") As String

      Select Case Style
        Case BootstrapEnums.FlatDreamAlertColor.Primary
          Return "alert-green"
        Case BootstrapEnums.FlatDreamAlertColor.Success
          Return "alert-success"
        Case BootstrapEnums.FlatDreamAlertColor.Info
          Return "alert-info"
        Case BootstrapEnums.FlatDreamAlertColor.Warning
          Return "alert-warning"
        Case BootstrapEnums.FlatDreamAlertColor.Danger
          Return "alert-danger"
        Case BootstrapEnums.FlatDreamAlertColor.Custom
          Return CustomStyle
      End Select

      Return ""

    End Function

    Private Shared Function GetDialogHeaderClass(Style As BootstrapEnums.Style) As String

      Select Case Style
        Case BootstrapEnums.Style.DefaultStyle
          Return "modal-default"
        Case BootstrapEnums.Style.Primary
          Return "modal-primary"
        Case BootstrapEnums.Style.Success
          Return "modal-success"
        Case BootstrapEnums.Style.Info
          Return "modal-info"
        Case BootstrapEnums.Style.Warning
          Return "modal-warning"
        Case BootstrapEnums.Style.Danger
          Return "modal-danger"
      End Select

      '.Custom
      Return ""

    End Function

    Public Class UnorderedList(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListTag As HTMLTag(Of ObjectType)
      Public Property CssClass As String = ""

      Public Function AddListItem(CssClass As String) As CustomControls.Bootstrap.ListItem(Of ObjectType)

        Return ListTag.Helpers.Bootstrap.ListItem(CssClass)
        'Return AddControl(t)

      End Function

      Public Function AddButtonDropDown(Optional ListItemCssClass As String = "",
                                        Optional IconName As String = "",
                                        Optional ActionButtonColorClass As String = "btn-default",
                                        Optional DropDownButtonColorClass As String = "btn-default",
                                        Optional ActionTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                        Optional le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                        Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                                        Optional ActionButtonClickBinding As String = "",
                                        Optional ButtonID As String = "") As CustomControls.Bootstrap.ButtonDropDown(Of ObjectType)

        Dim li As ListItem(Of ObjectType) = ListTag.Helpers.Bootstrap.ListItem("button dropdown" & ListItemCssClass)
        Dim Btn As ButtonDropDown(Of ObjectType) = li.Helpers.Bootstrap.ButtonDropDown(IconName, ActionButtonColorClass, DropDownButtonColorClass, ActionTextExpression, le, PostBackType, ActionButtonClickBinding, ButtonID)

        Return Btn

      End Function

      Public Sub New(CssClass As String)
        Me.CssClass = CssClass
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        ListTag = Helpers.HTMLTag("ul")
        ListTag.AddClass(CssClass)
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class ListItem(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ItemTag As HTMLTag(Of ObjectType)
      Public Property CssClass As String = ""

      Public Sub New(CssClass As String)
        Me.CssClass = CssClass
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        ItemTag = Helpers.HTMLTag("li")
        ItemTag.AddClass(CssClass)
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class PagerListItem(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListItem As ListItem(Of ObjectType)
      Public Property Anchor As Anchor(Of ObjectType)

      Public Property CssClass As String = ""
      Public Property AnchorHRef As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing
      Public Property AnchorLinkText As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing
      Public Property AnchorHrefString As String = ""
      Public Property AnchorLinkTextString As String = ""
      Public Property AnchorTarget As Singular.Web.LinkTargetType = LinkTargetType.NotSet

      Public Sub New(CssClass As String,
                     Optional AnchorHRef As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                     Optional AnchorLinkText As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                     Optional AnchorHrefString As String = "",
                     Optional AnchorLinkTextString As String = "",
                     Optional AnchorTarget As Singular.Web.LinkTargetType = LinkTargetType.NotSet)
        Me.CssClass = CssClass
        Me.AnchorHRef = AnchorHRef
        Me.AnchorLinkText = AnchorLinkText
        Me.AnchorHrefString = AnchorHrefString
        Me.AnchorLinkTextString = AnchorLinkTextString
        Me.AnchorTarget = AnchorTarget
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        ListItem = Helpers.Bootstrap.ListItem(CssClass)
        Anchor = ListItem.ItemTag.Helpers.Bootstrap.Anchor(AnchorHRef, AnchorLinkText, AnchorHrefString, AnchorLinkTextString, AnchorTarget)
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class Anchor(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property AnchorTag As HTMLTag(Of ObjectType)
      Public Property Href As String = ""
      Public Property LinkText As String = ""
      Public Property HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property LinkTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property Target As LinkTargetType = LinkTargetType.NotSet
      Public Property IconName As String = ""
      Public Property IconSize As String = ""
      Public Property IconTag As FontAwesomeIcon(Of ObjectType)

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        AnchorTag = Helpers.HTMLTag("a")

        If HRefExpression IsNot Nothing Then
          AnchorTag.AddBinding(KnockoutBindingString.href, HRefExpression)
        ElseIf Href <> "" Then
          If Href.StartsWith("~") Then
            Href = Utils.URL_ToAbsolute(Href)
          End If
          AnchorTag.Attributes("href") = Href
        End If

        If Target <> LinkTargetType.NotSet Then
          AnchorTag.Attributes("target") = Target.ToString
        End If

        With AnchorTag
          If IconName <> "" Then
            IconTag = .Helpers.Bootstrap.FontAwesomeIcon(IconName, IconSize)
          End If
          If LinkTextExpression IsNot Nothing Then
            .AddBinding(KnockoutBindingString.text, LinkTextExpression)
          ElseIf LinkText <> "" Then
            .Helpers.HTML(LinkText)
          End If
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

#Region " Buttons "

    Public Class Button(Of ObjectType)
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
      Public Property ButtonGroup As HTMLDiv(Of ObjectType)

      Public Property mIconName As String = ""
      Public Property mID As String = ""
      Public Property mText As String = ""
      Public Property mCustomStyleClass As String = ""
      Public Property mCustomSizeClass As String = ""
      Public Property mClickBinding As String = ""
      Public Property mIconSize As String = ""
      Public Property mButtonStyle As BootstrapEnums.Style
      Public Property mButtonSize As BootstrapEnums.ButtonSize
      Public Property Icon As FontAwesomeIcon(Of ObjectType)

      Public Property HRefExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ButtonTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property Target As LinkTargetType = LinkTargetType.NotSet
      Public Property mTextBeforeIcon As Boolean = False

      Public Sub AddRemoveBinding(Optional AfterItemRemoved As String = "")
        Dim h As IBindingControl = GetParentWithContext()
        If h Is Nothing OrElse h.PropertyInfo Is Nothing Then
          Throw New Exception("Cannot add a remove binding to a control that has no binding context parent.")
        End If
        Dim Options As String = "{}"
        If AfterItemRemoved <> "" Then
          Options = "{afterItemRemoved: " & AfterItemRemoved & "}"
        End If
        Button.AddBinding(KnockoutBindingString.click, "$parent." & GetParentWithContext.PropertyInfo.Name & ".Remove($data, arguments[1]," & Options & ")")
      End Sub

      Public Sub AddRemoveBindingByString(ParentListName As String, Optional AfterItemRemoved As String = "")
        'Dim h As IBindingControl = GetParentWithContext()
        'If h Is Nothing OrElse h.PropertyInfo Is Nothing Then
        '  Throw New Exception("Cannot add a remove binding to a control that has no binding context parent.")
        'End If
        Dim Options As String = "{}"
        If AfterItemRemoved <> "" Then
          Options = "{afterItemRemoved: " & AfterItemRemoved & "}"
        End If
        Button.AddBinding(KnockoutBindingString.click, "$parent." & ParentListName & ".Remove($data, arguments[1]," & Options & ")")
      End Sub

      Public Sub New(Optional ID As String = "",
                     Optional Text As String = "",
                     Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                     Optional CustomStyleClass As String = "",
                     Optional ButtonSize As BootstrapEnums.ButtonSize = BootstrapEnums.ButtonSize.ExtraSmall,
                     Optional CustomSizeClass As String = "",
                     Optional IconName As String = "",
                     Optional IconSize As String = "",
                     Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                     Optional ClickBinding As String = "",
                     Optional TextBeforeIcon As Boolean = False)

        mID = ID
        mText = Text
        mButtonStyle = ButtonStyle
        mCustomStyleClass = CustomStyleClass
        mButtonSize = ButtonSize
        mCustomSizeClass = CustomSizeClass
        mIconName = IconName
        mIconSize = IconSize
        mPostBackType = PostBackType
        mClickBinding = ClickBinding
        mTextBeforeIcon = TextBeforeIcon

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Button = Helpers.HTMLTag("button")
        Button.AddClass("btn")

        If mID <> "" Then
          Button.Attributes("id") = mID
        End If

        Button.AddClass(GetButtonClass(mButtonStyle, mCustomStyleClass))
        Button.AddClass(GetButtonSizeClass(mButtonSize, mCustomSizeClass))
        Button.Attributes("data-original-button-class") = GetButtonClass(mButtonStyle, mCustomStyleClass)

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

        With Button
          If mTextBeforeIcon Then
            ButtonText = .Helpers.HTMLTag("span")
            ButtonText.AddClass("button-text-span")
            If mText <> "" Then
              ButtonText.Helpers.HTML(mText)
            End If
            If mIconName <> "" Then
              Icon = .Helpers.Bootstrap.FontAwesomeIcon(mIconName, mIconSize)
            End If
          Else
            If mIconName <> "" Then
              Icon = .Helpers.Bootstrap.FontAwesomeIcon(mIconName, mIconSize)
            End If
            ButtonText = .Helpers.HTMLTag("span")
            ButtonText.AddClass("button-text-span")
            If mText <> "" Then
              ButtonText.Helpers.HTML(mText)
            End If
          End If
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class StateButton(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ListJS As String = ""
      Public Property TrueText As String = ""
      Public Property FalseText As String = ""
      Public Property TrueCss As String = ""
      Public Property FalseCss As String = ""
      Public Property TrueIconName As String = ""
      Public Property FalseIconName As String = ""
      Public Property ButtonSize As String = "btn-xs"

      'Controls
      Public Property Button As HTMLTag(Of ObjectType)
      Public Property GlyphContainer As BootstrapGlyphIcon(Of ObjectType)
      Public Property ButtonGroup As HTMLDiv(Of ObjectType)
      Public Property ButtonText As HTMLTag(Of ObjectType)

      Public Sub New(ListPropertyName As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String)

        Me.ListProperty = ListPropertyName
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Public Sub New(ListPropertyJS As String,
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String)

        Me.ListJS = ListPropertyJS
        Me.ListProperty = Nothing
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Button = Helpers.HTMLTag("button")
        Button.Attributes.Add("type", "button")
        Dim StandardCss As String = "btn"
        Dim PropName As String = ""
        Dim PropValue As String = ""
        If ListProperty IsNot Nothing Then
          Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
          PropName = Prop.Name
        Else
          PropName = ListJS
        End If
        PropValue = PropName & "()"

        If TrueIconName <> "" Then
          TrueIconName = "<i class=""fa " & TrueIconName & """></i>"
        End If

        If FalseIconName <> "" Then
          FalseIconName = "<i class=""fa " & FalseIconName & """></i>"
        End If

        Dim ClickString As String = HttpUtility.HtmlEncode(PropName & "(!" & PropName & "())")
        Dim CssString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & StandardCss & " " & ButtonSize & " " & TrueCss & "' : '" & StandardCss & " " & ButtonSize & " " & FalseCss & "'" & ")")
        Dim HtmlString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & TrueIconName & " " & TrueText & "' : '" & FalseIconName & " " & FalseText & "'" & ")")

        With Button
          .AddClass("state-button")
          .AddBinding(KnockoutBindingString.click, ClickString)
          .AddBinding(KnockoutBindingString.css, CssString)
          ButtonText = .Helpers.HTMLTag("span")
          With ButtonText
            .AddBinding(KnockoutBindingString.html, HtmlString)
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class StateButtonNew(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ListJS As String = ""
      Public Property TrueText As String = ""
      Public Property FalseText As String = ""
      Public Property TrueCss As String = ""
      Public Property FalseCss As String = ""
      Public Property TrueIconName As String = ""
      Public Property FalseIconName As String = ""
      Public Property ButtonSize As String = "btn-xs"

      'Controls
      Public Property Button As HTMLTag(Of ObjectType)
      Public Property ButtonText As HTMLTag(Of ObjectType)
      Public Property IconTag As HTMLTag(Of ObjectType)

      Public Sub New(ListPropertyName As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String)

        Me.ListProperty = ListPropertyName
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Public Sub New(ListPropertyJS As String,
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String)

        Me.ListJS = ListPropertyJS
        Me.ListProperty = Nothing
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Button = Helpers.HTMLTag("button")
        Button.Attributes.Add("type", "button")
        Dim StandardCss As String = "btn"
        Dim PropName As String = ""
        Dim PropValue As String = ""
        If ListProperty IsNot Nothing Then
          Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
          PropName = Prop.Name
        Else
          PropName = ListJS
        End If
        PropValue = PropName & "()"

        Dim ClickString As String = HttpUtility.HtmlEncode(PropName & "(!" & PropName & "())")
        Dim CssString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & StandardCss & " " & ButtonSize & " " & TrueCss & "' : '" & StandardCss & " " & ButtonSize & " " & FalseCss & "'" & ")")
        Dim IconString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & "fa " & TrueIconName & "' : '" & "fa " & FalseIconName & "'" & ")")
        Dim TextString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & TrueText & "' : '" & FalseText & "'" & ")")

        With Button
          .AddClass("state-button")
          .AddBinding(KnockoutBindingString.click, ClickString)
          .AddBinding(KnockoutBindingString.css, CssString)
          IconTag = .Helpers.HTMLTag("i")
          ButtonText = .Helpers.HTMLTag("span")
          With IconTag
            .AddBinding(KnockoutBindingString.css, IconString)
          End With
          With ButtonText
            .AddBinding(KnockoutBindingString.html, TextString)
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class TriStateButton(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ListJS As String = ""

      Public Property TrueText As String = ""
      Public Property FalseText As String = ""
      Public Property NullText As String = ""

      Public Property TrueCss As String = ""
      Public Property FalseCss As String = ""
      Public Property NullCss As String = ""

      Public Property TrueIconName As String = ""
      Public Property FalseIconName As String = ""
      Public Property NullIconName As String = ""

      Public Property ButtonSize As String = "btn-xs"

      'Controls
      Public Property Button As HTMLTag(Of ObjectType)
      Public Property ButtonText As HTMLTag(Of ObjectType)
      Public Property IconTag As HTMLTag(Of ObjectType)

      Public Sub New(ListPropertyName As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     TrueText As String, FalseText As String, NullText As String,
                     TrueCss As String, FalseCss As String, NullCss As String,
                     TrueIconName As String, FalseIconName As String, NullIconName As String,
                     ButtonSize As String)

        Me.ListProperty = ListPropertyName
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.NullText = NullText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.NullCss = NullCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.NullIconName = NullIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Public Sub New(ListPropertyJS As String,
                     TrueText As String, FalseText As String, NullText As String,
                     TrueCss As String, FalseCss As String, NullCss As String,
                     TrueIconName As String, FalseIconName As String, NullIconName As String,
                     ButtonSize As String)

        Me.ListJS = ListPropertyJS
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.NullText = NullText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.NullCss = NullCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.NullIconName = NullIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Button = Helpers.HTMLTag("button")
        Button.Attributes.Add("type", "button")
        Dim StandardCss As String = "btn"
        Dim PropName As String = ""
        Dim PropValue As String = ""
        If ListProperty IsNot Nothing Then
          Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
          PropName = Prop.Name
        Else
          PropName = ListJS
        End If
        PropValue = PropName & "()"

        Dim ClickString As String = HttpUtility.HtmlEncode("(" & PropName & "() === true ? " & PropName & "(false) : " & _
                                                           "(" & PropName & "() === false ? " & PropName & "(null) : " & _
                                                           "((" & PropName & "() === null || " & PropName & "() === undefined) ? " & PropName & "(true) : " & PropName & "(" & PropName & "())" & ")))")
        Dim CssString As String = HttpUtility.HtmlEncode("(" & PropName & "() === true ? " & "'" & StandardCss & " " & ButtonSize & " " & TrueCss & "' : " & _
                                                         "(" & PropName & "() === false ? " & "'" & StandardCss & " " & ButtonSize & " " & FalseCss & "' : " & _
                                                         "((" & PropName & "() === null || " & PropName & "() === undefined) ? " & "'" & StandardCss & " " & ButtonSize & " " & NullCss & "' : '')))")
        Dim IconString As String = HttpUtility.HtmlEncode("(" & PropName & "() === true ? " & "' fa " & TrueIconName & "' : " & _
                                                          "(" & PropName & "() === false ? " & "' fa " & FalseIconName & "' : " & _
                                                          "((" & PropName & "() === null || " & PropName & "() === undefined) ? " & "' fa " & NullIconName & "' : '')))")
        Dim TextString As String = HttpUtility.HtmlEncode("(" & PropName & "() === true ? " & "'" & TrueText & "' : " & _
                                                          "(" & PropName & "() === false ? " & "'" & FalseText & "' : " & _
                                                          "((" & PropName & "() === null || " & PropName & "() === undefined) ? " & "'" & NullText & "' : '')))")

        With Button
          .AddClass("state-button")
          .AddBinding(KnockoutBindingString.click, ClickString)
          .AddBinding(KnockoutBindingString.css, CssString)
          IconTag = .Helpers.HTMLTag("i")
          ButtonText = .Helpers.HTMLTag("span")
          With IconTag
            .AddBinding(KnockoutBindingString.css, IconString)
          End With
          With ButtonText
            .AddBinding(KnockoutBindingString.html, TextString)
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class StateDiv(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ListJS As String = ""
      Public Property TrueText As String = ""
      Public Property FalseText As String = ""
      Public Property TrueCss As String = ""
      Public Property FalseCss As String = ""
      Public Property TrueIconName As String = ""
      Public Property FalseIconName As String = ""
      Public Property ButtonSize As String = "btn-xs"
      Public Property IsRaw As Boolean = False

      'Controls
      Public Property Button As HTMLTag(Of ObjectType)
      Public Property ButtonIcon As HTMLTag(Of ObjectType)
      Public Property ButtonText As HTMLTag(Of ObjectType)

      Public Sub New(ListPropertyName As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String,
                     IsRaw As Boolean)

        Me.ListProperty = ListPropertyName
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize
        Me.IsRaw = IsRaw
      End Sub

      Public Sub New(ListPropertyJS As String,
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String,
                     IsRaw As Boolean)

        Me.ListJS = ListPropertyJS
        Me.ListProperty = Nothing
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize
        Me.IsRaw = IsRaw

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Button = Helpers.HTMLTag("div")
        'Button.Attributes.Add("type", "button")
        Dim StandardCss As String = "btn"
        Dim PropName As String = ""
        Dim PropValue As String = ""
        If ListProperty IsNot Nothing Then
          Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
          PropName = Prop.Name
        Else
          PropName = ListJS
        End If
        If Me.IsRaw Then
          PropValue = PropName
        Else
          PropValue = PropName & "()"
        End If

        If TrueIconName <> "" Then
          TrueIconName = "<i class=""fa " & TrueIconName & """></i>"
        End If

        If FalseIconName <> "" Then
          FalseIconName = "<i class=""fa " & FalseIconName & """></i>"
        End If

        Dim ClickString As String = ""
        Dim CssString As String = ""
        Dim IconHtml As String = ""
        Dim TextHtml As String = ""

        If IsRaw Then
          ClickString = HttpUtility.HtmlEncode(PropName & " = !" & PropName)
          CssString = HttpUtility.HtmlEncode("(" & PropName & " ? '" & StandardCss & " " & ButtonSize & " " & TrueCss & "' : '" & StandardCss & " " & ButtonSize & " " & FalseCss & "'" & ")")
          IconHtml = HttpUtility.HtmlEncode("(" & PropName & " ? '" & TrueIconName & "' : '" & FalseIconName & "'" & ")")
          TextHtml = HttpUtility.HtmlEncode("(" & PropName & " ? '" & TrueText & "' : '" & FalseText & "'" & ")")
        Else
          ClickString = HttpUtility.HtmlEncode(PropName & "(!" & PropName & "())")
          CssString = HttpUtility.HtmlEncode("(" & PropName & "() ? '" & StandardCss & " " & ButtonSize & " " & TrueCss & "' : '" & StandardCss & " " & ButtonSize & " " & FalseCss & "'" & ")")
          IconHtml = HttpUtility.HtmlEncode("(" & PropName & "() ? '" & TrueIconName & "' : '" & FalseIconName & "'" & ")")
          TextHtml = HttpUtility.HtmlEncode("(" & PropName & "() ? '" & TrueText & "' : '" & FalseText & "'" & ")")
        End If

        With Button
          .AddClass("state-div hide-overflow")
          .AddBinding(KnockoutBindingString.click, ClickString)
          .AddBinding(KnockoutBindingString.css, CssString)
          ButtonIcon = .Helpers.HTMLTag("span")
          With ButtonIcon
            .AddBinding(KnockoutBindingString.html, IconHtml)
          End With
          ButtonText = .Helpers.HTMLTag("span")
          With ButtonText
            .AddBinding(KnockoutBindingString.html, TextHtml)
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class ROStateButton(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ListJS As String = ""
      Public Property TrueText As String = ""
      Public Property FalseText As String = ""
      Public Property TrueCss As String = ""
      Public Property FalseCss As String = ""
      Public Property TrueIconName As String = ""
      Public Property FalseIconName As String = ""
      Public Property ButtonSize As String = "btn-xs"

      'Controls
      Public Property Button As HTMLTag(Of ObjectType)
      Public Property GlyphContainer As BootstrapGlyphIcon(Of ObjectType)
      Public Property ButtonGroup As HTMLDiv(Of ObjectType)
      Public Property ButtonText As HTMLTag(Of ObjectType)

      Public Sub New(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String)

        Me.ListProperty = ListProperty
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Public Sub New(ListProperty As String,
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String)

        Me.ListJS = ListProperty
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Button = Helpers.HTMLTag("span")
        'Button.Attributes.Add("type", "button")
        Dim StandardCss As String = "btn"
        Dim PropName As String = ""
        Dim PropValue As String = ""
        If ListProperty IsNot Nothing Then
          Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
          PropName = Prop.Name
        Else
          PropName = ListJS
        End If
        PropValue = PropName & "()"

        If TrueIconName <> "" Then
          TrueIconName = "<i class=""fa " & TrueIconName & """></i>"
        End If

        If FalseIconName <> "" Then
          FalseIconName = "<i class=""fa " & FalseIconName & """></i>"
        End If

        'Dim ClickString As String = HttpUtility.HtmlEncode(PropName & "(!" & PropName & "())")
        Dim CssString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & StandardCss & " " & ButtonSize & " " & TrueCss & "' : '" & StandardCss & " " & ButtonSize & " " & FalseCss & "'" & ")")
        Dim HtmlString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & TrueIconName & " " & TrueText & "' : '" & FalseIconName & " " & FalseText & "'" & ")")

        With Button
          .AddClass("state-button")
          '.AddBinding(KnockoutBindingString.click, ClickString)
          .AddBinding(KnockoutBindingString.css, CssString)
          ButtonText = .Helpers.HTMLTag("span")
          With ButtonText
            .AddBinding(KnockoutBindingString.html, HtmlString)
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class AsyncAddButtonBeta(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      'Public Property ListJS As String = ""
      'Public Property TrueText As String = ""
      'Public Property FalseText As String = ""
      'Public Property TrueCss As String = ""
      'Public Property FalseCss As String = ""
      'Public Property TrueIconName As String = ""
      'Public Property FalseIconName As String = ""

      Public Property StateID As Integer = 1

      Public Property NotSelectedText As String = ""
      Public Property NotSelectedCss As String = ""
      Public Property NotSelectedIconName As String = ""

      Public Property BusyText As String = ""
      Public Property BusyCss As String = ""
      Public Property BusyIconName As String = ""

      Public Property AddedText As String = ""
      Public Property AddedCss As String = ""
      Public Property AddedIconName As String = ""

      'Controls
      Public Property Button As HTMLTag(Of ObjectType)
      Public Property GlyphContainer As BootstrapGlyphIcon(Of ObjectType)
      Public Property ButtonGroup As HTMLDiv(Of ObjectType)
      Public Property ButtonText As HTMLTag(Of ObjectType)

      Public Sub New(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     NotSelectedText As String, NotSelectedCss As String, NotSelectedIconName As String,
                     BusyText As String, BusyCss As String, BusyIconName As String,
                     AddedText As String, AddedCss As String, AddedIconName As String)

        Me.ListProperty = ListProperty

        Me.NotSelectedText = ""
        Me.NotSelectedCss = ""
        Me.NotSelectedIconName = ""

        Me.BusyText = ""
        Me.BusyCss = ""
        Me.BusyIconName = ""

        Me.AddedText = ""
        Me.AddedCss = ""
        Me.AddedIconName = ""

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Button = Helpers.HTMLTag("button")
        Button.Attributes.Add("type", "button")
        Dim StandardCss As String = "btn btn-xs"
        Dim PropName As String = ""
        Dim PropValue As String = ""
        If ListProperty IsNot Nothing Then
          Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
          PropName = Prop.Name
        End If
        PropValue = PropName & "()"

        If NotSelectedIconName <> "" Then
          NotSelectedIconName = "<i class=""fa " & NotSelectedIconName & """></i>"
        End If

        If BusyIconName <> "" Then
          BusyIconName = "<i class=""fa " & BusyIconName & """></i>"
        End If

        If AddedIconName <> "" Then
          AddedIconName = "<i class=""fa " & AddedIconName & """></i>"
        End If

        Dim ClickString As String = HttpUtility.HtmlEncode(PropName & "(" & PropName & "() + 1)")

        Dim CssSwitchStart As String = "function($data){switch ($data." & PropName & "()) {"
        Dim CssCase1 As String = "case 1: return '" & StandardCss & NotSelectedCss & "';break;"
        Dim CssCase2 As String = "case 2: return '" & StandardCss & BusyCss & "';break;"
        Dim CssCase3 As String = "case 3: return '" & StandardCss & AddedCss & "';break;"
        Dim CssSwitchEnd As String = "}}"
        Dim FinalCss As String = CssSwitchStart & CssCase1 & CssCase2 & CssCase3 & CssSwitchEnd

        Dim HtmlSwitchStart As String = "function($data){switch ($data." & PropName & "()) {"
        Dim HtmlCase1 As String = "case 1: return '" & NotSelectedIconName & NotSelectedText & "';break;"
        Dim HtmlCase2 As String = "case 2: return '" & BusyIconName & BusyText & "';break;"
        Dim HtmlCase3 As String = "case 3: return '" & AddedIconName & AddedText & "';break;"
        Dim HtmlSwitchEnd As String = "}}"
        Dim FinalHtml As String = HtmlSwitchStart & HtmlCase1 & HtmlCase2 & HtmlCase3 & HtmlSwitchEnd

        Dim CssString As String = HttpUtility.HtmlEncode(FinalCss)
        Dim HtmlString As String = HttpUtility.HtmlEncode(FinalHtml)

        With Button
          .AddClass("state-button")
          .AddBinding(KnockoutBindingString.click, ClickString)
          .AddBinding(KnockoutBindingString.css, CssString)
          ButtonText = .Helpers.HTMLTag("span")
          With ButtonText
            .AddBinding(KnockoutBindingString.html, HtmlString)
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class SelectButton(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ListJS As String = ""
      Public Property TrueText As String = "Selected"
      Public Property FalseText As String = "Select"
      Public Property TrueCss As String = "btn-primary"
      Public Property FalseCss As String = "btn-default"
      Public Property TrueIconName As String = "fa-check-square-o"
      Public Property FalseIconName As String = "fa-times"
      Public Property OnSelectFunction As String = ""

      'Controls
      Public Property Button As HTMLTag(Of ObjectType)
      Public Property GlyphContainer As BootstrapGlyphIcon(Of ObjectType)
      Public Property ButtonGroup As HTMLDiv(Of ObjectType)
      Public Property ButtonText As HTMLTag(Of ObjectType)

      Public Sub New(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     OnSelectFunction As String)

        Me.ListProperty = ListProperty
        Me.OnSelectFunction = OnSelectFunction

      End Sub

      Public Sub New(ListProperty As String,
                     OnSelectFunction As String)

        Me.ListJS = ListProperty
        Me.OnSelectFunction = OnSelectFunction

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Button = Helpers.HTMLTag("button")
        Button.Attributes.Add("type", "button")
        Dim StandardCss As String = "btn btn-xs"
        Dim PropName As String = ""
        Dim PropValue As String = ""
        If ListProperty IsNot Nothing Then
          Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(ListProperty)
          PropName = Prop.Name
        Else
          PropName = ListJS
        End If
        PropValue = PropName & "()"

        If TrueIconName <> "" Then
          TrueIconName = "<i class=""fa " & TrueIconName & """></i>"
        End If

        If FalseIconName <> "" Then
          FalseIconName = "<i class=""fa " & FalseIconName & """></i>"
        End If

        Dim ClickString As String = HttpUtility.HtmlEncode(PropName & "(!" & PropName & "())")
        Dim CssString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & StandardCss & " " & TrueCss & "' : '" & StandardCss & " " & FalseCss & "'" & ")")
        Dim HtmlString As String = HttpUtility.HtmlEncode("(" & PropName & "()" & " ? '" & TrueIconName & " " & TrueText & "' : '" & FalseIconName & " " & FalseText & "'" & ")")

        With Button
          .AddClass("state-button")
          .AddBinding(KnockoutBindingString.click, ClickString)
          .AddBinding(KnockoutBindingString.css, CssString)
          ButtonText = .Helpers.HTMLTag("span")
          With ButtonText
            .AddBinding(KnockoutBindingString.html, HtmlString)
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class ButtonDropDown(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property BtnGroup As HTMLDiv(Of ObjectType)
      Public Property BtnAction As HTMLTag(Of ObjectType)
      Public Property Icon As FontAwesomeIcon(Of ObjectType)
      Public Property ActionButtonText As HTMLTag(Of ObjectType)
      Public Property BtnDropDownToggle As HTMLTag(Of ObjectType)
      Public Property DropDownMenu As HTMLTag(Of ObjectType)
      Public Property ActionButtonColorClass As String = ""
      Public Property DropDownButtonColorClass As String = ""
      Public Property ActionText As String = ""
      Public Property ActionTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property IconName As String = ""
      Public Property PostBackType As PostBackType = Singular.Web.PostBackType.None
      Public Property ActionButtonClickBinding As String = ""
      Public Property ButtonID As String = ""

      Public Class DropDownAction
        Inherits HelperBase(Of ObjectType)

        Public Property ActionListItem As HTMLTag(Of ObjectType)
        Public Property ActionLink As HTMLTag(Of ObjectType)
        Public Property IconName As String
        Public Property ActionText As String
        Public Property ClickBinding As String
        Public Property CanClickBinding As String

        Public Shadows ReadOnly Property Parent As Bootstrap.ButtonDropDown(Of ObjectType)
          Get
            Return MyBase.Parent
          End Get
        End Property

        Public Sub New(ActionText As String, Optional IconName As String = "", Optional ClickBinding As String = "", Optional CanClickBinding As String = "")
          Me.ActionText = ActionText
          Me.IconName = IconName
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
                If IconName <> "" Then
                  With .Helpers.HTMLTag("i")
                    .AddClass("fa " & IconName)
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

      Public Function AddAction(ActionText As String, Optional FontAwesomeIcon As String = "", Optional ClickBinding As String = "", Optional CanClickBinding As String = "") As DropDownAction

        Dim t As New DropDownAction(ActionText, FontAwesomeIcon, ClickBinding, CanClickBinding)
        Return AddControl(t)

      End Function

      Public Sub New(Optional IconName As String = "",
                     Optional ActionButtonColorClass As String = "btn-default",
                     Optional DropDownButtonColorClass As String = "btn-default",
                     Optional ActionTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                     Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                     Optional ActionButtonClickBinding As String = "",
                     Optional ButtonID As String = "")
        Me.IconName = IconName
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
            If IconName <> "" Then
              Icon = .Helpers.Bootstrap.FontAwesomeIcon(IconName)
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

#End Region

#Region " Alerts "

    Public Class ProgressBar(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property Percentage As Decimal? = Nothing
      Public Property PercentageProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ActionText As String = ""
      Public Property Striped As Boolean = False
      Public Property Active As Boolean = False
      Public Property CustomCssBinding As String = ""
      Public Property ProgressBarTag As HTMLDiv(Of ObjectType)
      Public Property ProgressBarTextTag As HTMLTag(Of ObjectType)

      Public Sub New(Percentage As Decimal, ActionText As String)
        Me.Percentage = Percentage
        Me.ActionText = ActionText
      End Sub

      Public Sub New(Optional PercentageProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                     Optional ActionText As String = "",
                     Optional Striped As Boolean = False, Optional Active As Boolean = False,
                     Optional CustomCssBinding As String = "")
        Me.PercentageProperty = PercentageProperty
        Me.ActionText = ActionText
        Me.Striped = Striped
        Me.Active = Active
        Me.CustomCssBinding = CustomCssBinding
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Dim PropName As String = ""
        Dim PropValue As String = ""
        Dim PropPerc As String = ""
        If PercentageProperty IsNot Nothing Then
          Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(PercentageProperty)
          PropName = Prop.Name
          PropValue = PropName & "()"
        ElseIf Percentage IsNot Nothing Then
          PropValue = Percentage
        Else

        End If
        If PropValue <> "" Then
          PropPerc = PropValue & ".toString() + '%'"
        End If

        With Helpers.DivC("progress")
          If CustomCssBinding <> "" Then
            .AddBinding(KnockoutBindingString.css, CustomCssBinding)
          End If
          If Active Then
            .AddClass("active")
          End If
          If Striped Then
            .AddClass("progress-striped")
          End If
          ProgressBarTag = .Helpers.DivC("progress-bar")
          With ProgressBarTag
            .Attributes("role") = ""
            If PropValue <> "" Then
              .AddBinding(KnockoutBindingString.attr, "{ 'aria-valuenow':" & PropValue & "}")
              .Attributes("aria-valuemin") = "0"
              .Attributes("aria-valuemax") = "100"
            End If
            If PropPerc <> "" Then
              .AddBinding(KnockoutBindingString.style, "{ width: " & HttpUtility.HtmlEncode(PropPerc) & "}")
            End If
            ProgressBarTextTag = .Helpers.HTMLTag("span")
            With ProgressBarTextTag
              .AddClass("action-text")
              If ActionText <> "" Then
                .Helpers.HTML(ActionText)
              End If
            End With
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class FlatDreamAlert(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property ErrorText As String = ""
      Public Property AlertTag As HTMLDiv(Of ObjectType)
      Public Property CloseButtonTag As HTMLTag(Of ObjectType)
      Public Property IconTag As HTMLTag(Of ObjectType)
      Public Property ErrorTextContainerTag As HTMLTag(Of ObjectType)
      Public Property AlertTitleTag As HTMLTag(Of ObjectType)
      Public Property ErrorTextTag As HTMLTag(Of ObjectType)
      Public Property AlertColor As BootstrapEnums.FlatDreamAlertColor
      Public Property IconName As String
      Public Property AlertTitleText As String
      Public Property Bordered As Boolean = False
      Public Property White As Boolean = True

      Public Sub New(ErrorText As String, AlertColor As BootstrapEnums.FlatDreamAlertColor,
                     IconName As String, AlertTitleText As String,
                     Bordered As Boolean,
                     White As Boolean)
        Me.ErrorText = ErrorText
        Me.AlertColor = AlertColor
        Me.IconName = IconName
        Me.AlertTitleText = AlertTitleText
        Me.Bordered = Bordered
        Me.White = White
      End Sub

      Private Function BorderedClass() As String
        If White Then
          If Bordered Then
            Return "alert-white-alt"
          Else
            Return "alert-white"
          End If
        End If
        Return ""
      End Function

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        AlertTag = Helpers.DivC("alert " & GetFlatDreamAlertColorClass(AlertColor) & " " & BorderedClass() & " rounded")
        With AlertTag
          CloseButtonTag = .Helpers.HTMLTag("button")
          With CloseButtonTag
            .Attributes("type") = "button"
            .Attributes("data-dismiss") = "alert"
            .Attributes("aria-hidden") = "true"
            .AddClass("close")
          End With
          IconTag = .Helpers.HTMLTag("span")
          With IconTag
            .AddClass("icon")
            With .Helpers.HTMLTag("i")
              .AddClass("fa " & IconName)
            End With
          End With
          ErrorTextContainerTag = .Helpers.HTMLTag("span")
          With ErrorTextContainerTag
            With .Helpers.HTMLTag("span")
              AlertTitleTag = .Helpers.HTMLTag("strong")
              With AlertTitleTag
                .Helpers.HTML(AlertTitleText)
              End With
            End With
            ErrorTextTag = .Helpers.HTMLTag("span")
            With ErrorTextTag
              .Helpers.HTML(ErrorText)
            End With
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class CustomErrorBox(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property ErrorText As String = ""
      Public Property AlertTag As HTMLDiv(Of ObjectType)
      Public Property CloseButtonTag As HTMLTag(Of ObjectType)
      Public Property IconTag As HTMLTag(Of ObjectType)
      Public Property ErrorTextContainerTag As HTMLTag(Of ObjectType)
      Public Property AlertTitleTag As HTMLTag(Of ObjectType)
      Public Property ErrorTextTag As HTMLTag(Of ObjectType)
      Public Property AlertColor As BootstrapEnums.FlatDreamAlertColor
      Public Property IconName As String
      Public Property AlertTitleText As String
      Public Property Bordered As Boolean = False
      Public Property White As Boolean = True

      Public Sub New(ErrorText As String, AlertColor As BootstrapEnums.FlatDreamAlertColor,
                     IconName As String, AlertTitleText As String,
                     Bordered As Boolean,
                     White As Boolean)
        Me.ErrorText = ErrorText
        Me.AlertColor = AlertColor
        Me.IconName = IconName
        Me.AlertTitleText = AlertTitleText
        Me.Bordered = Bordered
        Me.White = White
      End Sub

      Private Function BorderedClass() As String
        If White Then
          If Bordered Then
            Return "alert-white-alt"
          Else
            Return "alert-white"
          End If
        End If
        Return ""
      End Function

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        AlertTag = Helpers.DivC("alert " & GetFlatDreamAlertColorClass(AlertColor) & " " & BorderedClass() & " rounded")
        With AlertTag
          IconTag = .Helpers.HTMLTag("span")
          With IconTag
            .AddClass("icon")
            With .Helpers.HTMLTag("i")
              .AddClass("fa " & IconName)
            End With
          End With
          ErrorTextContainerTag = .Helpers.HTMLTag("span")
          With ErrorTextContainerTag
            With .Helpers.HTMLTag("span")
              AlertTitleTag = .Helpers.HTMLTag("strong")
              With AlertTitleTag
                .Helpers.HTML(AlertTitleText)
              End With
            End With
            ErrorTextTag = .Helpers.HTMLTag("span")
            With ErrorTextTag
              .Helpers.HTML(ErrorText)
            End With
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    '<div class="alert alert-danger alert-white rounded">
    '				<button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
    '				<div class="icon"><i class="fa fa-times-circle"></i></div>
    '				<strong>Error!</strong> The server is not responding, try again later.
    '			 </div>

#End Region

    Public Class Panel(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property Panel As HTMLDiv(Of ObjectType)
      Public Property PanelHeading As HTMLDiv(Of ObjectType)
      Public Property PanelBody As HTMLDiv(Of ObjectType)
      Public Property PanelFooter As HTMLDiv(Of ObjectType)
      Public Property PanelStyle As BootstrapEnums.Style = BootstrapEnums.Style.Primary
      Public Property CustomStyleClass As String = ""
      Public Property HeadingText As String = ""

      Public Sub New(PanelStyle As BootstrapEnums.Style, HeadingText As String, Optional CustomStyleClass As String = "")
        Me.PanelStyle = PanelStyle
        Me.CustomStyleClass = CustomStyleClass
        Me.HeadingText = HeadingText
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        If Me.PanelStyle <> BootstrapEnums.Style.Custom Then
          Panel = Helpers.DivC("panel " & GetPanelClass(Me.PanelStyle))
        Else
          Panel = Helpers.DivC("panel " & Me.CustomStyleClass)
        End If

        With Panel
          PanelHeading = .Helpers.DivC("panel-heading")
          PanelBody = .Helpers.DivC("panel-body")
          PanelFooter = .Helpers.DivC("panel-footer")
        End With

        With PanelHeading
          If HeadingText <> "" Then
            .Helpers.HTML(HeadingText)
          End If
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class FontAwesomeIcon(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Private mIconContainer As HTMLTag(Of ObjectType)
      'Private mIconName As String = ""
      Private mIconSize As String = ""

      Public Property IconContainer As HTMLTag(Of ObjectType)
      '  Get
      '    Return mIconContainer
      '  End Get
      'End Property
      'ReadOnly

      Public Property IconName As String = ""
      '  Get
      '    Return mIconName
      '  End Get
      '  Set(value As String)
      '    mIconName
      '  End Set
      'End Property

      Public ReadOnly Property IconSize As String
        Get
          Return mIconSize
        End Get
      End Property

      Public Sub New(IconName As String, IconSize As String)

        Me.IconName = IconName
        mIconSize = IconSize

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        IconContainer = Helpers.HTMLTag("i")
        IconContainer.AddClass("fa " & IconName & " " & mIconSize)

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class TabControl(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property ParentDiv As HTMLDiv(Of ObjectType)
      Public Property TabHeaderContainer As HTMLTag(Of ObjectType)
      Public Property TabContentContainer As HTMLDiv(Of ObjectType)
      Private mTabCssClass = "nav-justified"
      Private mTabRole As String = "tablist"
      Private mParentDivCssClass As String = ""
      Private mTabAlignment As BootstrapEnums.TabAlignment = BootstrapEnums.TabAlignment.Top

      Public Function AddTab(TabName As String, IconName As String,
                             Optional TabClickFunction As String = "", Optional TabNameDisplay As String = "",
                             Optional IconSize As String = "",
                             Optional FirstTab As Boolean = False) As Tab

        Dim t As New Tab(TabName, IconName, TabHeaderContainer, TabContentContainer, TabClickFunction, TabNameDisplay, IconSize, FirstTab)
        Return AddControl(t)

      End Function

      Public Sub New(Optional ParentDivCssClass As String = "", Optional ByVal TabCssClass As String = "nav-justified", Optional ByVal TabRole As String = "tablist",
                     Optional TabAlignment As BootstrapEnums.TabAlignment = BootstrapEnums.TabAlignment.Top)
        mTabCssClass = TabCssClass
        mTabRole = TabRole
        mParentDivCssClass = ParentDivCssClass
        mTabAlignment = TabAlignment
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        ParentDiv = Helpers.DivC(mParentDivCssClass)
        If mTabAlignment = BootstrapEnums.TabAlignment.Top Then
          'Do nothing
        ElseIf mTabAlignment = BootstrapEnums.TabAlignment.Left Then
          ParentDiv.AddClass("tab-left")
          mTabCssClass = ""
        ElseIf mTabAlignment = BootstrapEnums.TabAlignment.Right Then
          ParentDiv.AddClass("tab-right")
          mTabCssClass = ""
        End If
        TabHeaderContainer = ParentDiv.Helpers.HTMLTag("ul")
        TabHeaderContainer.AddClass("nav nav-tabs" & " " & mTabCssClass)
        TabHeaderContainer.Attributes("role") = mTabRole
        TabContentContainer = ParentDiv.Helpers.DivC("tab-content")

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

      Public Class Tab
        Inherits HelperBase(Of ObjectType)

        Public Property TabName As String
        Public Property IconName As String
        Public Property IconSize As String
        Public Property TabHeader As HTMLTag(Of ObjectType)
        Public Property TabLink As HTMLTag(Of ObjectType)
        Public Property TabPane As HTMLDiv(Of ObjectType)
        Public Property TabHeaderContainer As HTMLTag(Of ObjectType)
        Public Property TabContentContainer As HTMLDiv(Of ObjectType)
        Private mTabClickFunction As String = ""
        Private mTabNameDisplay As String = ""
        Private mFirstTab As Boolean = False

        Public Shadows ReadOnly Property Parent As BootstrapTabControl(Of ObjectType)
          Get
            Return MyBase.Parent
          End Get
        End Property

        Public Sub New(TabName As String, IconName As String,
                       TabHeaderContainer As HTMLTag(Of ObjectType), TabContentContainer As HTMLDiv(Of ObjectType),
                       Optional TabClickFunction As String = "", Optional TabNameDisplay As String = "",
                       Optional IconSize As String = "", Optional FirstTab As Boolean = False)
          Me.TabName = TabName
          Me.IconName = IconName
          Me.TabHeaderContainer = TabHeaderContainer
          Me.TabContentContainer = TabContentContainer
          Me.IconSize = IconSize
          mTabClickFunction = TabClickFunction
          mTabNameDisplay = TabNameDisplay
          mFirstTab = FirstTab
        End Sub

        Protected Friend Overrides Sub Setup()
          MyBase.Setup()

          TabHeader = TabHeaderContainer.Helpers.HTMLTag("li")
          With TabHeader
            TabLink = .Helpers.HTMLTag("a")
            With TabLink
              .Attributes("href") = "#" & Me.TabName.Replace(" ", "")
              .Attributes("role") = "tab"
              .Attributes("data-toggle") = "tab"
              .AddClass(Me.TabName.Replace(" ", ""))
              If mTabClickFunction <> "" Then
                .AddBinding(KnockoutBindingString.click, mTabClickFunction)
              End If
              If Me.IconName <> "" Then
                .Helpers.Bootstrap.FontAwesomeIcon(Me.IconName, Me.IconSize)
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
          TabPane.AddClass("fade")
          If mFirstTab Then
            TabPane.AddClass("in")
          End If
          TabPane.Attributes("id") = Me.TabName.Replace(" ", "")
          'If mActive Then
          '  TabPane.AddClass(" active")
          'End If

        End Sub

        Protected Friend Overrides Sub Render()
          MyBase.Render()
          RenderChildren()
        End Sub

      End Class

    End Class

    Public Class Dialog(Of ObjectType)
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
      Private mExcludeHeader As Boolean = False
      Private mModalSizeClass As String = ""
      Private mHeaderColor As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle
      Private mStyleCssClass As String = ""
      Private mCustomHeaderStyleClass As String = ""
      Private mIconName As String = ""
      Private mIconSize As String = ""
      Private mHideFooter As Boolean = False

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
      Public ReadOnly Property ContentDiv As HTMLDiv(Of ObjectType)
        Get
          Return mModalContentDiv
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

      Public Sub New(ID As String, Title As String,
                     Optional ExcludeHeader As Boolean = False,
                     Optional ModalSizeClass As String = "",
                     Optional HeaderColor As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                     Optional CustomHeaderClass As String = "",
                     Optional IconName As String = "",
                     Optional IconSize As String = "fa-2x",
                     Optional HideFooter As Boolean = False)

        'Title As String
        mID = ID
        mTitle = Title
        mExcludeHeader = ExcludeHeader
        mModalSizeClass = ModalSizeClass
        mHeaderColor = HeaderColor
        mStyleCssClass = GetDialogHeaderClass(mHeaderColor)
        mCustomHeaderStyleClass = CustomHeaderClass
        mIconName = IconName
        mIconSize = IconSize
        mHideFooter = HideFooter

        If mCustomHeaderStyleClass <> "" Then
          mStyleCssClass = mCustomHeaderStyleClass
        End If

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
            If mModalSizeClass <> "" Then
              mModalDialogDiv.AddClass(mModalSizeClass)
            End If

            mModalContentDiv = mModalDialogDiv.Helpers.Div
            With mModalContentDiv
              mModalContentDiv.AddClass("modal-content")

              'Header-----------------------------------------
              If Not mExcludeHeader Then
                mModalHeaderDiv = mModalContentDiv.Helpers.Div
                With mModalHeaderDiv
                  mModalHeaderDiv.AddClass("modal-header")
                  mModalHeaderDiv.AddClass(mStyleCssClass)
                  If mIconName <> "" Then
                    .Helpers.Bootstrap.FontAwesomeIcon(mIconName, mIconSize)
                  End If
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
              End If

              'Body-------------------------------------------
              mModalBodyDiv = mModalContentDiv.Helpers.Div
              With mModalBodyDiv
                mModalBodyDiv.AddClass("modal-body")

              End With

              If Not mHideFooter Then
                'Footer----------------------------------------
                mModalFooterDiv = mModalContentDiv.Helpers.Div
                With mModalFooterDiv
                  mModalFooterDiv.AddClass("modal-footer")

                End With
              End If

            End With

          End With

        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub


    End Class

    Public Class DateTimeEditor(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Private Property mLe As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Private Property mDateTimeEditor As HTMLDiv(Of ObjectType)
      Private Property mEditorWidth As Integer? = Nothing
      Private Property mDateEditorWidth As Integer? = Nothing
      Private Property mTimeEditorWidth As Integer? = Nothing

      Public Sub New(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     Optional EditorWidth As Integer? = Nothing,
                     Optional DateEditorWidth As Integer? = Nothing,
                     Optional TimeEditorWidth As Integer? = Nothing)

        Me.mLe = le
        Me.mEditorWidth = EditorWidth
        Me.mDateEditorWidth = DateEditorWidth
        Me.mTimeEditorWidth = TimeEditorWidth

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        mDateTimeEditor = Helpers.Div

        With mDateTimeEditor
          If mEditorWidth IsNot Nothing Then
            .Style.Width = mEditorWidth
          End If
          With .Helpers.EditorFor(mLe)
            If mDateEditorWidth IsNot Nothing Then
              .Style.Width = mDateEditorWidth
            End If
          End With
          With .Helpers.TimeEditorFor(mLe)
            If mTimeEditorWidth IsNot Nothing Then
              .Style.Width = mTimeEditorWidth
            End If
          End With
        End With
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()

        RenderChildren()
      End Sub

    End Class

    Public Class Label(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Private mContainerDiv As HTMLDiv(Of ObjectType)
      Public Property LabelTag As HTMLTag(Of ObjectType)
      Public Property LabelStyle As BootstrapEnums.Style = BootstrapEnums.Style.Custom
      Public Property LabelText As String = ""
      Public Property CustomLabelClass As String = ""
      Public Property IconName As String = ""
      Public Property IconSize As String = ""
      Public Property LabelTextContainer As HTMLTag(Of ObjectType)
      Public Property LabelTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))

      Private mClickBinding As String = ""

      Public Sub New(Optional LabelText As String = "",
                     Optional LabelStyle As BootstrapEnums.Style = BootstrapEnums.Style.Custom,
                     Optional CustomLabelClass As String = "",
                     Optional IconName As String = "",
                     Optional IconSize As String = "",
                     Optional LabelTextExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing)

        Me.LabelText = LabelText
        Me.LabelStyle = LabelStyle
        Me.CustomLabelClass = CustomLabelClass
        Me.IconName = IconName
        Me.IconSize = IconSize
        Me.LabelTextExpression = LabelTextExpression

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        LabelTag = Helpers.HTMLTag("span")
        LabelTag.AddClass("label")
        LabelTag.AddClass(GetLabelClass(LabelStyle, CustomLabelClass))

        If IconName <> "" Then
          With LabelTag
            .Helpers.Bootstrap.FontAwesomeIcon(IconName, IconSize)
            LabelTextContainer = .Helpers.HTMLTag("span")
            If LabelTextExpression IsNot Nothing Then
              LabelTextContainer.AddBinding(KnockoutBindingString.html, LabelTextExpression)
            ElseIf LabelText <> "" Then
              With LabelTextContainer
                .Helpers.HTML(LabelText)
              End With
            End If
          End With
        Else
          With LabelTag
            LabelTextContainer = .Helpers.HTMLTag("span")
            If LabelTextExpression IsNot Nothing Then
              LabelTextContainer.AddBinding(KnockoutBindingString.html, LabelTextExpression)
            ElseIf LabelText <> "" Then
              With LabelTextContainer
                .Helpers.HTML(LabelText)
              End With
            End If
            'With LabelTextContainer
            '  .Helpers.HTML(LabelText)
            'End With
          End With
        End If

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class LabelDisplay(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Private mContainerDiv As HTMLDiv(Of ObjectType)
      Public Property LabelTag As HTMLTag(Of ObjectType)
      Public Property LabelText As String = ""

      Private mClickBinding As String = ""

      Public Sub New(Optional LabelText As String = "")

        Me.LabelText = LabelText

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        LabelTag = Helpers.HTMLTag("label")
        LabelTag.Helpers.HTML(LabelText)

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class InputGroupAddOnText(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Private mAddOn As HTMLTag(Of ObjectType)
      Private mAddOnText As String = ""

      Public ReadOnly Property AddOn As HTMLTag(Of ObjectType)
        Get
          Return mAddOn
        End Get
      End Property

      Public Sub New(AddOnText As String)

        mAddOnText = AddOnText

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        mAddOn = Helpers.HTMLTag("span")
        mAddOn.AddClass("input-group-btn")

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class InputGroupAddOnButton(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Private mAddOn As HTMLTag(Of ObjectType)
      Private mButton As Button(Of ObjectType)
      Private mButtonID As String = ""
      Private mButtonText As String = ""
      Private mButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle
      Private mCustomStyleClass As String = ""
      Private mButtonSize As BootstrapEnums.ButtonSize = BootstrapEnums.ButtonSize.ExtraSmall
      Private mCustomSizeClass As String = ""
      Private mIconName As String = ""
      Private mIconSize As String = ""
      Private mPostBackType As PostBackType = Singular.Web.PostBackType.None
      Private mClickBinding As String = ""

      Public ReadOnly Property Button As Button(Of ObjectType)
        Get
          Return mButton
        End Get
      End Property

      Public ReadOnly Property AddOn As HTMLTag(Of ObjectType)
        Get
          Return mAddOn
        End Get
      End Property

      Public Sub New(Optional ButtonID As String = "",
                     Optional ButtonText As String = "",
                     Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                     Optional CustomStyleClass As String = "",
                     Optional ButtonSize As BootstrapEnums.ButtonSize = BootstrapEnums.ButtonSize.ExtraSmall,
                     Optional CustomSizeClass As String = "",
                     Optional IconName As String = "",
                     Optional IconSize As String = "",
                     Optional PostBackType As PostBackType = Singular.Web.PostBackType.None,
                     Optional ClickBinding As String = "")

        mButtonID = ButtonID
        mButtonText = ButtonText
        mButtonStyle = ButtonStyle
        mCustomStyleClass = CustomStyleClass
        mButtonSize = ButtonSize
        mCustomSizeClass = CustomSizeClass
        mIconName = IconName
        mIconSize = IconSize
        mPostBackType = PostBackType
        mClickBinding = ClickBinding

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        mAddOn = Helpers.HTMLTag("span")
        mAddOn.AddClass("input-group-btn")
        With mAddOn
          mButton = .Helpers.Bootstrap.Button(mButtonID, mButtonText, mButtonStyle, mCustomStyleClass,
                                              mButtonSize, mCustomSizeClass, mIconName, mIconSize, mPostBackType, mClickBinding)
        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class InputGroupAddOnStateButton(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Private mStateButton As StateButton(Of ObjectType)
      Private mAddOn As HTMLTag(Of ObjectType)

      Public Property ListPropertyExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property ListJS As String = ""
      Public Property TrueText As String = ""
      Public Property FalseText As String = ""
      Public Property TrueCss As String = ""
      Public Property FalseCss As String = ""
      Public Property TrueIconName As String = ""
      Public Property FalseIconName As String = ""
      Public Property ButtonSize As String = "btn-xs"

      Public ReadOnly Property StateButton As StateButton(Of ObjectType)
        Get
          Return mStateButton
        End Get
      End Property

      Public ReadOnly Property AddOn As HTMLTag(Of ObjectType)
        Get
          Return mAddOn
        End Get
      End Property

      Public Sub New(ListPropertyExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String)

        Me.ListPropertyExpression = ListPropertyExpression
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Public Sub New(ListPropertyString As String,
                     TrueText As String,
                     FalseText As String,
                     TrueCss As String,
                     FalseCss As String,
                     TrueIconName As String,
                     FalseIconName As String,
                     ButtonSize As String)

        Me.ListJS = ListPropertyString
        Me.TrueText = TrueText
        Me.FalseText = FalseText
        Me.TrueCss = TrueCss
        Me.FalseCss = FalseCss
        Me.TrueIconName = TrueIconName
        Me.FalseIconName = FalseIconName
        Me.ButtonSize = ButtonSize

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        mAddOn = Helpers.HTMLTag("span")
        mAddOn.AddClass("input-group-btn")
        With mAddOn
          If ListPropertyExpression IsNot Nothing Then
            mStateButton = .Helpers.Bootstrap.StateButton(ListPropertyExpression, TrueText, FalseText, TrueCss, FalseCss, TrueIconName, FalseIconName, ButtonSize)
          Else
            mStateButton = .Helpers.Bootstrap.StateButton(ListJS, TrueText, FalseText, TrueCss, FalseCss, TrueIconName, FalseIconName, ButtonSize)
          End If
        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class ComboFor(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property FormControl As Bootstrap.FormControlFor(Of ObjectType)
      Public Property InputGroup As HTMLDiv(Of ObjectType)
      Public Property AddOnButton As InputGroupAddOnButton(Of ObjectType)

      Public Property EditorPlaceholderText As String = ""
      Public Property ButtonText As String = ""
      Public Property ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle
      Public Property IconName As String = "fa-sort-down"

      Public Property InputSize As BootstrapEnums.InputSize = BootstrapEnums.InputSize.Small
      Public Property CustomInputSize As String = ""

      Public Property InputGroupSize As BootstrapEnums.InputGroupSize = BootstrapEnums.InputGroupSize.Small
      Public Property CustomInputGroupSize As String = ""

      Public Property AddOnButtonSize As BootstrapEnums.ButtonSize = BootstrapEnums.ButtonSize.ExtraSmall
      Public Property CustomAddOnButtonSize As String = ""

      Public Sub New(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     Optional EditorPlaceholderText As String = "",
                     Optional ButtonText As String = "",
                     Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                     Optional IconName As String = "fa-sort-down",
                     Optional InputSize As BootstrapEnums.InputSize = BootstrapEnums.InputSize.Small,
                     Optional CustomInputSize As String = "",
                     Optional InputGroupSize As BootstrapEnums.InputGroupSize = BootstrapEnums.InputGroupSize.Small,
                     Optional CustomInputGroupSize As String = "",
                     Optional AddOnButtonSize As BootstrapEnums.ButtonSize = BootstrapEnums.ButtonSize.ExtraSmall,
                     Optional CustomAddOnButtonSize As String = "")

        Me.le = le
        Me.EditorPlaceholderText = EditorPlaceholderText
        Me.ButtonText = ButtonText
        Me.ButtonStyle = ButtonStyle
        Me.IconName = IconName
        Me.InputGroupSize = InputGroupSize
        Me.CustomInputGroupSize = CustomInputGroupSize
        Me.InputSize = InputSize
        Me.CustomInputSize = CustomInputSize
        Me.AddOnButtonSize = AddOnButtonSize
        Me.CustomAddOnButtonSize = CustomAddOnButtonSize

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        InputGroup = Helpers.Bootstrap.InputGroup(Me.InputGroupSize, Me.CustomInputGroupSize)
        With InputGroup
          FormControl = .Helpers.Bootstrap.FormControlFor(Me.le, Me.InputSize, Me.CustomInputSize)
          'With FormControl
          If Me.EditorPlaceholderText <> "" Then
            Me.FormControl.Editor.Attributes("placeholder") = EditorPlaceholderText
          End If
          'End With
          AddOnButton = .Helpers.Bootstrap.InputGroupAddOnButton(, ButtonText, ButtonStyle, , Me.AddOnButtonSize, Me.CustomAddOnButtonSize, IconName, , PostBackType.None, )
        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class ReadOnlyComboFor(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property Editor As FieldDisplay(Of ObjectType)
      Public Property Input As EditorBase(Of ObjectType)
      Public Property InputGroup As HTMLDiv(Of ObjectType)
      Public Property AddOnButton As InputGroupAddOnButton(Of ObjectType)
      Public Property ClearButton As InputGroupAddOnButton(Of ObjectType)

      Public Property EditorPlaceholderText As String = ""
      Public Property ButtonText As String = ""
      Public Property ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle
      Public Property IconName As String = "fa-sort-down"
      Private mOnClickBinding As String = ""
      Public Property UseInputControl As Boolean = False
      Public Property ClearFunction As String = ""
      Public Property InputGroupWidthPercentage As String = ""
      Public Property InputGroupSize As BootstrapEnums.InputGroupSize = BootstrapEnums.InputGroupSize.Small
      Public Property InputSize As BootstrapEnums.InputSize = BootstrapEnums.InputSize.Small
      Public Property IncludeClearButton As Boolean = False

      Public Sub New(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
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
                     Optional IncludeClearButton As Boolean = True)

        Me.le = le
        Me.EditorPlaceholderText = EditorPlaceholderText
        Me.ButtonText = ButtonText
        Me.ButtonStyle = ButtonStyle
        Me.IconName = IconName
        mOnClickBinding = OnClickBinding
        Me.UseInputControl = UseInputControl
        Me.ClearFunction = ClearFunction
        Me.InputGroupWidthPercentage = InputGroupWidthPercentage
        Me.InputGroupSize = InputGroupSize
        Me.InputSize = InputSize
        Me.IncludeClearButton = IncludeClearButton

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Dim InputSizeClass As String = GetInputSizeClass(Me.InputSize)
        InputGroup = Helpers.Bootstrap.InputGroup(Me.InputGroupSize)
        With InputGroup
          If InputGroupWidthPercentage <> "" Then
            InputGroup.Style.Width = InputGroupWidthPercentage & "%"
          End If
          If UseInputControl Then
            Input = .Helpers.EditorFor(Me.le)
            With Input
              .AddClass("form-control " & InputSizeClass)
              .Attributes("readonly") = "readonly"
              If EditorPlaceholderText <> "" Then
                .Attributes("placeholder") = EditorPlaceholderText
              End If
              If mOnClickBinding <> "" Then
                Input.AddBinding(KnockoutBindingString.click, mOnClickBinding)
              End If
            End With
          Else
            Editor = .Helpers.ReadOnlyFor(Me.le)
            With Editor
              .AddClass("form-control " & InputSizeClass)
              .Attributes("readonly") = "readonly"
              If EditorPlaceholderText <> "" Then
                .Attributes("placeholder") = EditorPlaceholderText
              End If
              If mOnClickBinding <> "" Then
                Editor.AddBinding(KnockoutBindingString.click, mOnClickBinding)
              End If
            End With
          End If
          AddOnButton = .Helpers.Bootstrap.InputGroupAddOnButton(, "", ButtonStyle, , BootstrapEnums.ButtonSize.ExtraSmall, , IconName, , PostBackType.None, mOnClickBinding)

          If Me.IncludeClearButton Then
            If ClearFunction = "" Then
              Dim PropName As String = ""
              Dim Prop As System.Reflection.MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(Me.le)
              PropName = Prop.Name
              ClearFunction = PropName & "(null)"
            End If
            ClearButton = .Helpers.Bootstrap.InputGroupAddOnButton(, "", ButtonStyle, , BootstrapEnums.ButtonSize.ExtraSmall, , "fa-eraser", , PostBackType.None, ClearFunction)
          End If

        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class ReadOnlyFindFor(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property Editor As FieldDisplay(Of ObjectType)
      Public Property InputGroup As HTMLDiv(Of ObjectType)
      Public Property AddOnButton As InputGroupAddOnButton(Of ObjectType)

      Public Property ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle
      Public Property IconName As String = "fa-search"
      Public Property PlaceholderText As String = ""
      Private mOnClickBinding As String = ""

      Public Sub New(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     OnClickBinding As String,
                     Optional ButtonStyle As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle,
                     Optional IconName As String = "fa-search",
                     Optional PlaceholderText As String = "")

        Me.le = le
        Me.ButtonStyle = ButtonStyle
        Me.IconName = IconName
        Me.PlaceholderText = PlaceholderText
        mOnClickBinding = OnClickBinding

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        InputGroup = Helpers.Bootstrap.InputGroup(BootstrapEnums.InputGroupSize.Small)
        With InputGroup
          Editor = .Helpers.ReadOnlyFor(Me.le)
          With Editor
            .AddClass("form-control input-sm")
            .Attributes("disabled") = "disabled"
            If Me.PlaceholderText <> "" Then
              Editor.Attributes("placeholder") = Me.PlaceholderText
            End If
            If mOnClickBinding <> "" Then
              Editor.AddBinding(KnockoutBindingString.click, mOnClickBinding)
            End If
          End With
          AddOnButton = .Helpers.Bootstrap.InputGroupAddOnButton(, "", ButtonStyle, , BootstrapEnums.ButtonSize.ExtraSmall, , IconName, , PostBackType.None, mOnClickBinding)
        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class FormControlFor(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      'Public Property PropertyJS As String = ""
      Public Property Editor As EditorBase(Of ObjectType)
      Public Property Size As BootstrapEnums.InputSize = BootstrapEnums.InputSize.Small
      Public Property CustomSize As String = ""
      Public Property PlaceHolderText As String = ""

      Private mPagingManagerProperty As PropertyInfo
      Private mPageManagerPropertyString As String
      Public Property PagingManagerJSProperty As String = ""

      Public Sub New(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     Size As BootstrapEnums.InputSize,
                     Optional CustomSize As String = "",
                     Optional PlaceHolderText As String = "")
        Me.le = le
        Me.Size = Size
        Me.CustomSize = CustomSize
        Me.PlaceHolderText = PlaceHolderText
      End Sub

      'Public Sub New(PropertyJS As String,
      '               Size As BootstrapEnums.InputSize,
      '               Optional CustomSize As String = "",
      '               Optional PlaceHolderText As String = "")
      '  Me.PropertyJS = PropertyJS
      '  Me.Size = Size
      '  Me.CustomSize = CustomSize
      '  Me.PlaceHolderText = PlaceHolderText
      'End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        'If PropertyJS <> "" Then
        '  With Helpers.HTMLTag("input")
        '    .AddBinding(KnockoutBindingString.value, PropertyJS & "()")
        '    .AddClass("form-control")
        '    .AddClass(GetInputSizeClass(Me.Size, Me.CustomSize))
        '    If PlaceHolderText <> "" Then
        '      .Attributes("placeholder") = PlaceHolderText
        '    End If
        '  End With
        'Else
        Editor = Helpers.EditorFor(Me.le)
        With Editor
          .AddClass("form-control")
          .AddClass(GetInputSizeClass(Me.Size, Me.CustomSize))
          If PlaceHolderText <> "" Then
            .Attributes("placeholder") = PlaceHolderText
          End If
        End With
        'End If

        'End If
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class ReadOnlyFormControlFor(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property FieldDisplay As FieldDisplay(Of ObjectType)
      Public Property Size As BootstrapEnums.InputSize = BootstrapEnums.InputSize.Small
      Public Property CustomSize As String = ""
      Public Property PlaceholderText As String = ""

      Public Sub New(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                     Size As BootstrapEnums.InputSize, Optional CustomSize As String = "",
                     Optional PlaceholderText As String = "")
        Me.le = le
        Me.Size = Size
        Me.CustomSize = CustomSize
        Me.PlaceholderText = PlaceholderText
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        FieldDisplay = Helpers.ReadOnlyFor(Me.le, FieldTagType.span)
        With FieldDisplay
          .Attributes("readonly") = "readonly"
          .AddClass("form-control")
          .AddClass(GetInputSizeClass(Me.Size, Me.CustomSize))
          .Attributes("placeholder") = PlaceholderText
        End With
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    'Public Class DateAndTimeEditor(Of ObjectType)
    '  Inherits HelperBase(Of ObjectType)

    '  Private Property mLe As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
    '  Private Property mDateTimeEditor As HTMLDiv(Of ObjectType)

    '  Public Sub New(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))

    '    Me.mLe = le

    '  End Sub

    '  Protected Friend Overrides Sub Setup()
    '    MyBase.Setup()
    '    mDateTimeEditor = Helpers.Div
    '    With mDateTimeEditor
    '    End With
    '  End Sub

    '  Protected Friend Overrides Sub Render()
    '    MyBase.Render()
    '    RenderChildren()
    '  End Sub

    'End Class

    Public Class Select2(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Private Property mLe As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Private Property mSelect2Editor As HTMLTag(Of ObjectType)

      Public Sub New(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))

        Me.mLe = le

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        mSelect2Editor = Helpers.HTMLTag("select")
        With mSelect2Editor
        End With
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class Image(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Private mContainerDiv As HTMLDiv(Of ObjectType)
      Public Property ImageTag As HTMLTag(Of ObjectType)
      Public Property PhotoPath As String = ""
      Public Property PhotoPathExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Public Property CssClasses As String = ""

      Private mClickBinding As String = ""

      Public Sub New(Optional PhotoPathExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                     Optional PhotoPath As String = "",
                     Optional CssClasses As String = "")

        Me.PhotoPath = PhotoPath
        Me.PhotoPathExpression = PhotoPathExpression
        Me.CssClasses = CssClasses

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        ImageTag = Helpers.HTMLTag("img")
        ImageTag.AddClass(CssClasses)
        If PhotoPathExpression IsNot Nothing Then
          With ImageTag
            .AddBinding(KnockoutBindingString.src, PhotoPathExpression)
          End With
        Else
          ImageTag.Attributes("src") = PhotoPath
        End If

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class CustomPopover(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Private mPopoverContainer As HTMLDiv(Of ObjectType)
      Private mArrow As HTMLDiv(Of ObjectType)
      Private mPopoverTitle As HTMLTag(Of ObjectType)
      Private mPopoverContent As HTMLDiv(Of ObjectType)
      Private mTitle As String = ""
      Private mPopoverID As String = ""

      '<div class="popover fade right in" role="tooltip" id="popover68930" style="top: 26px; left: 232.109px; display: block;">
      '     <div class="arrow" style="top: 50%;"></div>
      '     <h3 class="popover-title">Popover title</h3>
      '     <div class="popover-content">And here's some amazing content. It's very engaging. Right?</div>
      '</div>

      Public ReadOnly Property PopoverContainer As HTMLDiv(Of ObjectType)
        Get
          Return mPopoverContainer
        End Get
      End Property

      Public ReadOnly Property Arrow As HTMLDiv(Of ObjectType)
        Get
          Return mArrow
        End Get
      End Property

      Public ReadOnly Property PopoverTitle As HTMLTag(Of ObjectType)
        Get
          Return mPopoverTitle
        End Get
      End Property

      Public ReadOnly Property PopoverContent As HTMLDiv(Of ObjectType)
        Get
          Return mPopoverContent
        End Get
      End Property

      Public Sub New(ID As String, Title As String,
                     Optional ExcludeHeader As Boolean = False,
                     Optional ModalSizeClass As String = "",
                     Optional HeaderColor As BootstrapEnums.Style = BootstrapEnums.Style.DefaultStyle)

        'Title As String
        mPopoverID = ID
        mTitle = Title
        'mExcludeHeader = ExcludeHeader
        'mModalSizeClass = ModalSizeClass
        'mHeaderColor = HeaderColor
        'mStyleCssClass = GetDialogHeaderClass(mHeaderColor)

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        mPopoverContainer = Helpers.Div

        'outermost
        With mPopoverContainer
          .AddClass("popover fade right in")
          .Attributes("tabindex") = "-1"
          .Attributes("role") = "tooltip"
          .Attributes("aria-hidden") = "true"
          If mPopoverID <> "" Then
            mPopoverContainer.Attributes("id") = mPopoverID
          End If

          mArrow = mPopoverContainer.Helpers.DivC("arrow")
          mPopoverTitle = mPopoverContainer.Helpers.HTMLTag("h3")
          mPopoverTitle.AddClass("popover-title")
          mPopoverTitle.AddBinding(KnockoutBindingString.html, mTitle)
          mPopoverContent = mPopoverContainer.Helpers.DivC("popover-content")

        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub


    End Class

    Public Class WizardControl(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property WizardRow As HTMLDiv(Of ObjectType)
      Public Property FuelUxRow As HTMLDiv(Of ObjectType)
      Public Property WizardBlock As HTMLDiv(Of ObjectType)
      Public Property WizardStepHeadings As HTMLDiv(Of ObjectType)
      Public Property WizardStepList As HTMLTag(Of ObjectType)
      Public Property WizardActions As HTMLDiv(Of ObjectType)
      Public Property PrevStepButton As HTMLTag(Of ObjectType)
      Public Property NextStepButton As HTMLTag(Of ObjectType)
      Public Property WizardStepsContent As HTMLDiv(Of ObjectType)
      Public Property WizardID As String
      Public Property OnPrevClick As String = ""
      Public Property OnNextClick As String = ""

      Public Function AddStep(StepName As String,
                              Optional StepDisplayName As String = "",
                              Optional StepIconName As String = "",
                              Optional StepIconSize As String = "",
                              Optional StepClickFunction As String = "") As WizardStep

        Dim t As New WizardStep(StepName, WizardStepList, WizardStepsContent, StepIconName, StepClickFunction, StepDisplayName, StepIconSize)
        Return AddControl(t)

      End Function

      Public Sub New(WizardID As String, OnPrevClick As String, OnNextClick As String)
        Me.WizardID = WizardID
        Me.OnPrevClick = OnPrevClick
        Me.OnNextClick = OnNextClick
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        'With .Helpers.DivC("row wizard-row")
        '  '.Style.Margin("20")
        '  With .Helpers.DivC("col-md-12 fuelux")
        '    With .Helpers.DivC("block-wizard")
        '      With .Helpers.DivC("wizard-steps wizard wizard-ux")
        '        .Attributes("id") = "ImportWizard"
        '        With .Helpers.HTMLTag("ul")
        '          .AddClass("steps")
        '          With .Helpers.HTMLTag("li")
        '            .Attributes("data-target") = "#step1"
        '            .AddClass("active")
        '            .Helpers.HTML("Step 1<span class='chevron'></span>")
        '          End With
        '          With .Helpers.HTMLTag("li")
        '            .Attributes("data-target") = "#step2"
        '            .Helpers.HTML("Step 2<span class='chevron'></span>")
        '          End With
        '          With .Helpers.HTMLTag("li")
        '            .Attributes("data-target") = "#step3"
        '            .Helpers.HTML("Step 3<span class='chevron'></span>")
        '          End With
        '        End With
        '        With .Helpers.DivC("actions")
        '          With .Helpers.HTMLTag("button")
        '            .AddClass("btn btn-xs btn-prev btn-primary")
        '            .Helpers.HTML(" <i class='fa fa-arrow-left'></i>Prev")
        '          End With
        '          With .Helpers.HTMLTag("button")
        '            .AddClass("btn btn-xs btn-prev btn-primary")
        '            .Helpers.HTML("Next<i class='fa fa-arrow-right'></i>")
        '          End With
        '        End With
        '      End With
        '      With .Helpers.DivC("step-content")
        '        With .Helpers.DivC("step-pane active")
        '          .Attributes("id") = "step1"

        '        End With
        '        With .Helpers.DivC("step-pane")
        '          .Attributes("id") = "step2"

        '        End With
        '        With .Helpers.DivC("step-pane")
        '          .Attributes("id") = "step3"
        '        End With
        '      End With
        '    End With
        '  End With
        'End With

        WizardRow = Helpers.DivC("row wizard-row")
        With WizardRow
          FuelUxRow = .Helpers.DivC("fuelux")
          With FuelUxRow
            WizardBlock = .Helpers.DivC("block-wizard")
            With WizardBlock
              WizardStepHeadings = .Helpers.DivC("wizard-steps wizard wizard-ux")
              WizardStepHeadings.Attributes("id") = WizardID
              With WizardStepHeadings
                WizardStepList = .Helpers.HTMLTag("ul")
                With WizardStepList
                  .AddClass("steps")
                  .Style.MarginAll("0")
                End With
                WizardActions = .Helpers.DivC("actions")
                With WizardActions
                  PrevStepButton = .Helpers.HTMLTag("button")
                  With PrevStepButton
                    .Attributes("type") = "button"
                    .AddClass("btn btn-xs btn-prev btn-primary")
                    With .Helpers.HTMLTag("span")
                      .Helpers.Bootstrap.FontAwesomeIcon("fa-arrow-left")
                    End With
                    With .Helpers.HTMLTag("span")
                      .Helpers.HTML("Prev")
                    End With
                    .AddBinding(KnockoutBindingString.click, OnPrevClick)
                  End With
                  NextStepButton = .Helpers.HTMLTag("button")
                  With NextStepButton()
                    .Attributes("type") = "button"
                    .Attributes("data-last") = "Finish"
                    .AddClass("btn btn-xs btn-next btn-primary")
                    With .Helpers.HTMLTag("span")
                      .Helpers.HTML("Next")
                    End With
                    With .Helpers.HTMLTag("span")
                      .Helpers.Bootstrap.FontAwesomeIcon("fa-arrow-right")
                    End With
                    .AddBinding(KnockoutBindingString.click, OnNextClick)
                  End With
                End With
              End With
              WizardStepsContent = .Helpers.DivC("step-content")
              With WizardStepsContent

              End With
            End With
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

      Public Class WizardStep
        Inherits HelperBase(Of ObjectType)

        Public Property StepName As String
        Public Property IconName As String
        Public Property IconSize As String
        Public Property StepHeader As HTMLTag(Of ObjectType)
        Public Property StepPane As HTMLDiv(Of ObjectType)
        Public Property WizardStepList As HTMLTag(Of ObjectType)
        Public Property WizardStepsContent As HTMLDiv(Of ObjectType)
        Public Property StepClickFunction As String = ""
        Public Property StepDisplayName As String = ""

        Public Shadows ReadOnly Property Parent As WizardControl(Of ObjectType)
          Get
            Return MyBase.Parent
          End Get
        End Property

        Public Sub New(StepName As String,
                       WizardStepList As HTMLTag(Of ObjectType),
                       WizardStepsContent As HTMLDiv(Of ObjectType),
                       Optional IconName As String = "",
                       Optional StepClickFunction As String = "",
                       Optional StepDisplayName As String = "",
                       Optional IconSize As String = "")
          Me.StepName = StepName
          Me.IconName = IconName
          Me.WizardStepList = WizardStepList
          Me.WizardStepsContent = WizardStepsContent
          Me.IconSize = IconSize
          Me.StepClickFunction = StepClickFunction
          Me.StepDisplayName = StepDisplayName
        End Sub

        Protected Friend Overrides Sub Setup()
          MyBase.Setup()

          StepHeader = WizardStepList.Helpers.HTMLTag("li")
          With StepHeader
            .Attributes("data-target") = "#" & Me.StepName.Replace(" ", "")
            With .Helpers.HTMLTag("span")
              If StepDisplayName = "" Then
                .Helpers.HTML(Me.StepName)
              Else
                .Helpers.HTML(StepDisplayName)
              End If
            End With
            With .Helpers.HTMLTag("span")
              .AddClass("chevron")
            End With
          End With
          StepPane = WizardStepsContent.Helpers.DivC("step-pane")
          StepPane.Attributes("id") = Me.StepName.Replace(" ", "")

        End Sub

        Protected Friend Overrides Sub Render()
          MyBase.Render()
          RenderChildren()
        End Sub

      End Class

    End Class

#Region " Tables "

    ''' <summary>
    ''' Creates a table / grid and creates a row for each item in the provided list.
    ''' </summary>
    Public Class Table(Of ObjectType, ChildControlObjectType)
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
        Public Property HeaderCellAlign As String = ""

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


              Dim TableParent As Bootstrap.Table(Of ObjectType, ChildControlObjectType) = CType(Parent, TableRow).Parent

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

          Dim TableParent As Bootstrap.Table(Of ObjectType, ChildControlObjectType) = CType(Parent, TableRow).Parent

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

        Protected mRowButtons As New List(Of CustomControls.Bootstrap.Button(Of ChildControlObjectType))


        'Friend mIsBodyRow As Boolean = True

        Public Property IncludeInHeader As Boolean = True

        Public Property AutoFormat As Boolean = True

        Friend Function AddButton(ButtonText As String) As CustomControls.Bootstrap.Button(Of ChildControlObjectType)
          Dim mNewButton = Helpers.Bootstrap.Button(, ButtonText, , , , , , , , )
          'Helpers.Button(ButtonText)
          mNewButton.Attributes("tabindex") = "-1"
          mRowButtons.Add(mNewButton)
          Return mNewButton
        End Function

        Protected Friend Overrides Sub Setup()
          MyBase.Setup()

        End Sub

#Region " Add Column Functions "

        Protected Sub AddAutoClass(col As TableColumn, IsReadOnly As Boolean)
          Dim tbl = CType(Parent, Bootstrap.Table(Of ObjectType, ChildControlObjectType))
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

          'Add Row Select Binding if there is one
          Dim OnRowSelect As String = CType(Me.Parent, Table(Of ObjectType, ChildControlObjectType)).OnRowSelectJS
          If OnRowSelect <> "" Then
            Bindings.Add(KnockoutBindingString.click, OnRowSelect)
          End If

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

        Private mExpandButton As CustomControls.Button(Of ChildControlObjectType)
        Friend mRemoveButton As CustomControls.Bootstrap.Button(Of ChildControlObjectType)
        Friend mIsChildBandRow As Boolean = False
        Public Property ExandVisibleBinding As String = ""
        Public Property ExpandButtonColumnWidth As String = ""
        Private mListSortName As String = ""

        Friend Sub AddExpandButton(ExpandedProperty As String)
          mExpandButton = Helpers.Button("")
          mExpandButton.Style("width") = Singular.Web.Controls.ExpandButtonWidth & "px"
          mExpandButton.AddBinding(KnockoutBindingString.click, "$data." & ExpandedProperty & "(!$data." & ExpandedProperty & "())")
          mExpandButton.ButtonText.AddBinding(KnockoutBindingString.html, "($data." & ExpandedProperty & "() ? '-' : '+')")
          mExpandButton.AddBinding(KnockoutBindingString.title, "($data." & ExpandedProperty & "() ? 'Hide Child Rows.' : 'Expand Child Rows.')")
          If ExandVisibleBinding <> "" Then
            mExpandButton.AddBinding(KnockoutBindingString.visible, ExandVisibleBinding)
          End If
          mExpandButton.ButtonStyle = ButtonMainStyle.Default
          mExpandButton.ButtonSize = ButtonSize.ExtraSmall
        End Sub

        Public ReadOnly Property ExpandButton As CustomControls.Button(Of ChildControlObjectType)
          Get
            Return mExpandButton
          End Get
        End Property

        Public Sub New(Optional ListSortName As String = "")
          mListSortName = ListSortName
        End Sub

        Protected Friend Overrides Sub Setup()
          MyBase.Setup()

          Dim table As Bootstrap.Table(Of ObjectType, ChildControlObjectType) = CType(Parent, Bootstrap.Table(Of ObjectType, ChildControlObjectType))
          If table.AllowRemove Then
            mRemoveButton = Helpers.Bootstrap.Button(, , BootstrapEnums.Style.Danger, , BootstrapEnums.ButtonSize.ExtraSmall, , "fa-trash", , PostBackType.None, )
            mRemoveButton.Attributes("tabindex") = "-1"
            mRowButtons.Add(mRemoveButton)
            If PropertyInfo IsNot Nothing Then
              If table.AfterItemRemovedFunctionName <> "" Then
                mRemoveButton.AddRemoveBinding(table.AfterItemRemovedFunctionName)
              Else
                mRemoveButton.AddRemoveBinding()
              End If
            ElseIf mListSortName <> "" Then
              If table.AfterItemRemovedFunctionName <> "" Then
                mRemoveButton.AddRemoveBindingByString(mListSortName, table.AfterItemRemovedFunctionName)
              Else
                mRemoveButton.AddRemoveBindingByString(mListSortName)
              End If
              'Else
              '  Throw New Exception("PropertyInfo or ListSortName must be provided when enabling AllowAddNew")
            End If
          End If
        End Sub

        ''' <summary>
        ''' Adds a column with nothing in it.
        ''' </summary>
        Public Function AddColumn(Optional HeaderText As String = "",
                                  Optional xsSize As Integer? = Nothing, Optional smSize As Integer? = Nothing,
                                  Optional mdSize As Integer? = Nothing, Optional lgSize As Integer? = Nothing,
                                  Optional ColumnAlign As String = "") As TableColumn
          Dim Col = AddColumn(CType(Nothing, PropertyInfo), HeaderText)
          AddAutoClass(Col, True)
          If xsSize IsNot Nothing Then
            Col.AddClass("col-xs-" & xsSize.Value.ToString)
          End If
          If smSize IsNot Nothing Then
            Col.AddClass("col-sm-" & smSize.Value.ToString)
          End If
          If mdSize IsNot Nothing Then
            Col.AddClass("col-md-" & mdSize.Value.ToString)
          End If
          If lgSize IsNot Nothing Then
            Col.AddClass("col-lg-" & lgSize.Value.ToString)
          End If
          If ColumnAlign <> "" Then
            Col.Attributes("align") = ColumnAlign
          End If
          Return Col
        End Function

        ''' <summary>
        ''' Adds an editable Column. 
        ''' </summary>
        ''' <param name="le">The property to bind the column to.</param>
        Public Function AddColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), Optional HeaderText As String = "",
                                  Optional xsSize As Integer? = Nothing, Optional smSize As Integer? = Nothing,
                                  Optional mdSize As Integer? = Nothing, Optional lgSize As Integer? = Nothing,
                                  Optional ColumnAlign As String = "") As TableColumn

          Dim Col As New TableColumn With {.HeaderText = HeaderText}
          If xsSize IsNot Nothing Then
            Col.AddClass("col-xs-" & xsSize.Value.ToString)
          End If
          If smSize IsNot Nothing Then
            Col.AddClass("col-sm-" & smSize.Value.ToString)
          End If
          If mdSize IsNot Nothing Then
            Col.AddClass("col-md-" & mdSize.Value.ToString)
          End If
          If lgSize IsNot Nothing Then
            Col.AddClass("col-lg-" & lgSize.Value.ToString)
          End If
          Col.For(le)
          SetWidthFromAttribute(Col)

          AddControl(Col)
          AddAutoClass(Col, False)
          If ColumnAlign <> "" Then
            Col.Attributes("align") = ColumnAlign
          End If
          Return Col

        End Function

        ''' <summary>
        ''' Adds an editable Column. 
        ''' </summary>
        Public Function AddColumn(pi As PropertyInfo, Optional HeaderText As String = "",
                                  Optional ColumnAlign As String = "") As TableColumn

          Dim Col As New TableColumn With {.HeaderText = HeaderText}
          If pi IsNot Nothing Then
            Col.For(pi)
            SetWidthFromAttribute(Col)
          End If

          AddControl(Col)
          AddAutoClass(Col, False)
          If ColumnAlign <> "" Then
            Col.Attributes("align") = ColumnAlign
          End If
          Return Col

        End Function

        ''' <summary>
        ''' Adds an editable Column. 
        ''' </summary>
        ''' <param name="le">The property to bind the column to.</param>
        Public Function AddColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ColumnWidth As Integer,
                                  Optional HeaderText As String = "",
                                  Optional ColumnAlign As String = "") As TableColumn

          Dim col = AddColumn(le, HeaderText)
          If ColumnWidth > 0 Then
            col.Style.Width = ColumnWidth & "px"
            col.Editor.Style.Width = ColumnWidth & "px"
          End If
          If ColumnAlign <> "" Then
            col.Attributes("align") = ColumnAlign
            col.Editor.Style.TextAlign = ColumnAlign
          End If
          Return col

        End Function

        ''' <summary>
        ''' Adds a Column that displays the property but doesn't allow it to be edited.
        ''' </summary>
        ''' <param name="le">The Property to Display</param>
        Public Function AddReadOnlyColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), Optional HeaderText As String = "",
                                          Optional xsSize As Integer? = Nothing, Optional smSize As Integer? = Nothing,
                                          Optional mdSize As Integer? = Nothing, Optional lgSize As Integer? = Nothing,
                                          Optional ColumnAlign As String = "") As TableColumn

          Dim Col As New TableColumn With {.HeaderText = HeaderText}
          Col.ReadOnlyColumn = True
          Col.AutoFormat = AutoFormat
          Col.For(le)
          SetWidthFromAttribute(Col)
          If xsSize IsNot Nothing Then
            Col.AddClass("col-xs-" & xsSize.Value.ToString)
          End If
          If smSize IsNot Nothing Then
            Col.AddClass("col-sm-" & smSize.Value.ToString)
          End If
          If mdSize IsNot Nothing Then
            Col.AddClass("col-md-" & mdSize.Value.ToString)
          End If
          If lgSize IsNot Nothing Then
            Col.AddClass("col-lg-" & lgSize.Value.ToString)
          End If
          AddAutoClass(Col, True)
          If ColumnAlign <> "" Then
            Col.Attributes("align") = ColumnAlign
          End If
          Return AddControl(Col)

        End Function

        ''' <summary>
        ''' Adds a Column that displays the property but doesn't allow it to be edited.
        ''' </summary>
        Public Function AddReadOnlyColumn(pi As PropertyInfo, Optional HeaderText As String = "",
                                          Optional ColumnAlign As String = "") As TableColumn

          Dim Col As New TableColumn With {.HeaderText = HeaderText}
          Col.ReadOnlyColumn = True
          Col.AutoFormat = AutoFormat
          If pi IsNot Nothing Then
            Col.For(pi)
            SetWidthFromAttribute(Col)
          End If
          AddAutoClass(Col, True)
          If ColumnAlign <> "" Then
            Col.Attributes("align") = ColumnAlign
          End If
          Return AddControl(Col)

        End Function

        ''' <summary>
        ''' Adds a Column that displays the property but doesn't allow it to be edited.
        ''' </summary>
        ''' <param name="le">The Property to Display</param>
        Public Function AddReadOnlyColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ColumnWidth As Integer,
                                          Optional HeaderText As String = "",
                                          Optional ColumnAlign As String = "") As TableColumn

          Dim Col = AddReadOnlyColumn(le, HeaderText)
          If ColumnWidth > 0 Then
            Col.Style.Width = ColumnWidth & "px"
            Col.FieldDisplay.Style.Width = ColumnWidth & "px"
          End If
          If ColumnAlign <> "" Then
            Col.Attributes("align") = ColumnAlign
            'Col.FieldDisplay.Style.TextAlign = ColumnAlign
          End If
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
              If ExpandButtonColumnWidth <> "" Then
                Writer.WriteAttribute("style", "width: " & ExpandButtonColumnWidth)
              End If
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
            If ExpandButtonColumnWidth <> "" Then
              Writer.WriteAttribute("style", "width: " & ExpandButtonColumnWidth)
            End If
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
          Dim Binding As String = CType(Parent, Bootstrap.Table(Of ObjectType, ChildControlObjectType)).GetForJS
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
      Private mAddNewButton As CustomControls.Bootstrap.Button(Of ObjectType)
      Private mRowCount As Integer = 1
      Private mFirstRow As DataRowTemplate
      Private mFooterRows As New List(Of TableRow)
      Private mChildBandCell As TableColumn
      Private mChildBandRow As DataRowTemplate
      Private mTableBodyClass As String = ""
      Private mCustomCell As TableColumn

      Public Property EditableCellClass As String = ""
      Public Property EditorClass As String = ""
      Public Property ReadOnlyCellClass As String = ""

      Public Property OnRowSelectJS As String = ""

      Public Property BodyHeightMin As Integer = 0
      Public Property BodyHeightMax As Integer = 0

      Public Property ReadOnlyColumnsHaveSpan As Boolean = True

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        If PathToContext() <> "" Then
          AddBinding(KnockoutBindingString.with, PathToContext)
        End If
        BindingManager.HasBindingContext = True

        mFirstRow = New DataRowTemplate(ListSortName)
        If ServerBindObject IsNot Nothing Then
          AllowRemove = False
        End If
        AddControl(mFirstRow)
        'CustomControls.Button(Of ObjectType) With {.Text = "Add", .ButtonID = ""}
        If AllowAddNew Then
          mAddNewButton = AddControl(New Bootstrap.Button(Of ObjectType)(, "Add", BootstrapEnums.Style.Primary, , BootstrapEnums.ButtonSize.ExtraSmall, , "fa-plus-circle", , PostBackType.None, ))
          Dim OptionsString As String = IIf(AfterItemAddedFunctionName.Trim.Length = 0, "{}", "{ afterItemAdded: " & AfterItemAddedFunctionName & " }")
          If PropertyInfo IsNot Nothing Then
            mAddNewButton.Button.Bindings.AddAddBinding(PropertyInfo, OptionsString)
          ElseIf ListSortName <> "" Then
            mAddNewButton.Button.Bindings.AddAddBinding(ListSortName, OptionsString)
          Else
            Throw New Exception("PropertyInfo or ListSortName must be provided when enabling AllowAddNew")
          End If
        End If

        'AddClass("Grid")
        AddClass("table")
        If Striped Then
          AddClass("table-striped")
        End If
        If Bordered Then
          AddClass("table-bordered")
        End If
        If Hover Then
          AddClass("table-hover")
        End If
        If Condensed Then
          AddClass("table-condensed")
        End If

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
      Public Property Bordered As Boolean = True
      Public Property Hover As Boolean = True
      Public Property Condensed As Boolean = True
      Public Property Responsive As Boolean = True
      Public Property ListSortName As String = ""
      Public Property AfterItemAddedFunctionName As String = ""
      Public Property AfterItemRemovedFunctionName As String = ""

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

      Public ReadOnly Property AddNewButton As CustomControls.Bootstrap.Button(Of ObjectType)
        Get
          Return mAddNewButton
        End Get
      End Property

      Public ReadOnly Property RemoveButton As CustomControls.Bootstrap.Button(Of ChildControlObjectType)
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

      Public ReadOnly Property CustomCell As TableColumn
        Get
          Return mCustomCell
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
      Public Function AddButton(ByVal ButtonText As String) As CustomControls.Bootstrap.Button(Of ChildControlObjectType)
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

      Public Function AddCustomRow(Optional RowCssClass As String = "") As DataRowTemplate
        If mCustomCell IsNot Nothing Then
          Throw New Exception("Normal Rows cannot be added after Child Bands.")
        Else
          Dim row As New DataRowTemplate
          If RowCssClass <> "" Then
            row.AddClass(RowCssClass)
          End If
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

      Public Function AddChildTable(Of ChildTableObjectType)(pi As PropertyInfo,
                                                             ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                             Optional Striped As Boolean = True,
                                                             Optional Bordered As Boolean = True,
                                                             Optional Hover As Boolean = True,
                                                             Optional Condensed As Boolean = True,
                                                             Optional Responsive As Boolean = True,
                                                             Optional ChildRowCssClass As String = "table-row-child",
                                                             Optional ListNameForSorting As String = "",
                                                             Optional AfterItemAddedFunctionName As String = "",
                                                             Optional AfterItemRemovedFunctionName As String = "") As Bootstrap.Table(Of ChildControlObjectType, ChildTableObjectType)

        'Remember what position the child band row container is in.
        If mChildBandCell Is Nothing Then

          'Add a new row for the child band
          mChildBandRow = AddRow()
          mChildBandRow.IncludeInHeader = False
          If ChildRowCssClass <> "" Then
            mChildBandRow.AddClass(ChildRowCssClass)
          End If

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

        Return mChildBandCell.Helpers.Bootstrap.TableFor(Of ChildTableObjectType)(pi, AllowAddNew, AllowRemove, Striped, Bordered, Hover, Condensed, Responsive, ListNameForSorting, AfterItemAddedFunctionName, AfterItemRemovedFunctionName)

      End Function

      Public Function AddChildTableByDataSourceString(Of ChildTableObjectType)(DataSourceString As String,
                                                                               ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                                               Optional Striped As Boolean = True,
                                                                               Optional Bordered As Boolean = True,
                                                                               Optional Hover As Boolean = True,
                                                                               Optional Condensed As Boolean = True,
                                                                               Optional Responsive As Boolean = True,
                                                                               Optional ChildRowCssClass As String = "table-row-child",
                                                                               Optional ListNameForSorting As String = "",
                                                                               Optional AfterItemAddedFunctionName As String = "",
                                                                               Optional AfterItemRemovedFunctionName As String = "") As Bootstrap.Table(Of ChildControlObjectType, ChildTableObjectType)

        'Remember what position the child band row container is in.
        If mChildBandCell Is Nothing Then

          'Add a new row for the child band
          mChildBandRow = AddRow()
          mChildBandRow.IncludeInHeader = False
          If ChildRowCssClass <> "" Then
            mChildBandRow.AddClass(ChildRowCssClass)
          End If

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

        Return mChildBandCell.Helpers.Bootstrap.TableFor(Of ChildTableObjectType)(DataSourceString, AllowAddNew, AllowRemove, Striped, Bordered, Hover, Condensed, Responsive, ListNameForSorting, AfterItemAddedFunctionName, AfterItemRemovedFunctionName)

      End Function

      Public Function AddChildTable(Of ChildTableObjectType)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)),
                                                             ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                             Optional Striped As Boolean = True,
                                                             Optional Bordered As Boolean = True,
                                                             Optional Hover As Boolean = True,
                                                             Optional Condensed As Boolean = True,
                                                             Optional Responsive As Boolean = True,
                                                             Optional ChildRowCssClass As String = "table-row-child",
                                                             Optional ListNameForSorting As String = "",
                                                             Optional AfterItemAddedFunctionName As String = "",
                                                             Optional AfterItemRemovedFunctionName As String = "") As Bootstrap.Table(Of ChildControlObjectType, ChildTableObjectType)

        Return AddChildTable(Of ChildTableObjectType)(Singular.Reflection.GetMember(Of ChildControlObjectType)(le), AllowAddNew, AllowRemove, Striped, Bordered, Hover, Condensed, Responsive, ChildRowCssClass, ListNameForSorting, AfterItemAddedFunctionName, AfterItemRemovedFunctionName)

      End Function

      Public Function AddChildTable(Of ChildTableObjectType)(ByVal DataSourceString As String,
                                                             ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean,
                                                             Optional Striped As Boolean = True,
                                                             Optional Bordered As Boolean = True,
                                                             Optional Hover As Boolean = True,
                                                             Optional Condensed As Boolean = True,
                                                             Optional Responsive As Boolean = True,
                                                             Optional ChildRowCssClass As String = "table-row-child",
                                                             Optional ListNameForSorting As String = "",
                                                             Optional AfterItemAddedFunctionName As String = "",
                                                             Optional AfterItemRemovedFunctionName As String = "") As Bootstrap.Table(Of ChildControlObjectType, ChildTableObjectType)

        Return AddChildTableByDataSourceString(Of ChildTableObjectType)(DataSourceString, AllowAddNew, AllowRemove, Striped, Bordered, Hover, Condensed, Responsive, ChildRowCssClass, ListNameForSorting, AfterItemAddedFunctionName, AfterItemRemovedFunctionName)

      End Function

      Public Function AddChildRow(Of ChildTableObjectType)(Optional RowCssClass As String = "") As TableColumn

        If mChildBandCell Is Nothing Then

          'Add a new row for the child band
          mChildBandRow = AddCustomRow(RowCssClass)
          mChildBandRow.IncludeInHeader = False

          'Child Band only has one cell that spans the whole table.
          mChildBandCell = ChildBandRow.AddColumn
          mChildBandCell.ColSpan = FirstRow.GetDataCellCount
          'mChildBandCell.AddClass("Grid-ChildCell")

          'Check if the object has an expanded property
          Dim piExpand As PropertyInfo = Singular.Reflection.GetProperty(OverrideChildType, "Expanded")
          If piExpand Is Nothing Then
            piExpand = Singular.Reflection.GetProperty(OverrideChildType, "IsExpanded")
          End If

          If piExpand IsNot Nothing Then
            mFirstRow.AddExpandButton(piExpand.Name)
            Dim eo = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ExpandOptions)(piExpand)
            If eo Is Nothing OrElse eo.RenderChildrenMode = Singular.DataAnnotations.ExpandOptions.RenderChildrenModeType.OnExpand Then
              ChildBandRow.AddBinding(KnockoutBindingString.if, "$data." & piExpand.Name)
              'If AnimateExpansion Then
              mChildBandCell.TemplateRenderAnimation = VisibleFadeType.SlideUpDown
              'End If
            Else
              'If AnimateExpansion Then
              ChildBandRow.Bindings.AddVisibilityBinding("$data." & piExpand.Name, VisibleFadeType.SlideUpDown, VisibleFadeType.SlideUpDown)
              'Else
              '  ChildBandRow.AddBinding(KnockoutBindingString.visible, "$data." & piExpand.Name)
              'End If
            End If

            ChildBandRow.mIsChildBandRow = True
          End If

        End If

        Return mChildBandCell

        '(pi As PropertyInfo)
        'Remember what position the child band row container is in.
        'If mCustomCell Is Nothing Then

        '  'Add a new row for the child band
        '  mChildBandRow = AddCustomRow(RowCssClass)
        '  mChildBandRow.IncludeInHeader = False

        '  'Child Band only has one cell that spans the whole table.
        '  mCustomCell = ChildBandRow.AddColumn
        '  mCustomCell.ColSpan = FirstRow.GetDataCellCount

        '  'Check if the object has an expanded property
        '  Dim piExpand As PropertyInfo = Singular.Reflection.GetProperty(OverrideChildType, "Expanded")
        '  If piExpand Is Nothing Then
        '    piExpand = Singular.Reflection.GetProperty(OverrideChildType, "IsExpanded")
        '  End If

        '  If piExpand IsNot Nothing Then
        '    mFirstRow.AddExpandButton(piExpand.Name)
        '    Dim eo = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ExpandOptions)(piExpand)
        '    ChildBandRow.AddBinding(If(eo Is Nothing OrElse eo.RenderChildrenMode = Singular.DataAnnotations.ExpandOptions.RenderChildrenModeType.OnParentRender, KnockoutBindingString.visible, KnockoutBindingString.if), "$data." & piExpand.Name)
        '    ChildBandRow.mIsChildBandRow = True
        '  End If

        'End If

        'Return mCustomCell


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
          Dim ChildTbl = AddChildTable(Of Object)(pi, childAllowAddNew, childAllowRemove)
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
          'If AllowClientSideSorting Then
          '  BindingString = "SFilter: " & PropertyInfo.Name & ", "
          'End If
        End If

        Dim ListName As String = ""

        If PropertyInfo IsNot Nothing Then
          ListName = PropertyInfo.Name
        ElseIf ListSortName <> "" Then
          ListName = ListSortName
        End If

        If AllowClientSideSorting AndAlso ListName <> "" Then
          BindingString = "SFilter: " & ListName & ", template: {foreach: Singular.ProcessList(" & DataSourceString & ", " & ListName & "), "
        Else
          BindingString = "template: {foreach: " & DataSourceString & ", "
        End If

        'If ApplyAlternateRowStyle Then
        '  Writer.WriteAttribute("data-bind", BindingString & "template: {foreach: Singular.ProcessList(" & DataSourceString & "), afterRender: function(c, o){ Singular.FormatTableRow(c, o); Singular.AfterTemplateRender(c, o) }}")
        'Else
        '  Writer.WriteAttribute("data-bind", BindingString & "template: {foreach: Singular.ProcessList(" & DataSourceString & "), afterRender: function(c, o){ Singular.AfterTemplateRender(c, o) }}")
        'End If
        If ApplyAlternateRowStyle Then
          BindingString &= "afterRender: function(c, o){ Singular.FormatTableRow(c, o); Singular.AfterTemplateRender(c, o) }}"
        Else
          BindingString &= "afterRender: function(c, o){ Singular.AfterTemplateRender(c, o) }}"
        End If

        Writer.WriteAttribute("data-bind", BindingString)
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

    Public Class PagedGrid(Of ObjectType, ChildControlObjectType)
      Inherits Table(Of ObjectType, ChildControlObjectType)

      Public Property PagingManager As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))

      Private mPagingManagerProperty As PropertyInfo
      Private mPageManagerPropertyString As String
      Public Property PagingManagerJSProperty As String = ""
      Public Property PagerPosition As BootstrapEnums.PagerPosition = BootstrapEnums.PagerPosition.Bottom
      Public Property PageNavigation As Bootstrap.Nav(Of ChildControlObjectType)
      Public Property Pager As Bootstrap.Pager(Of ChildControlObjectType)
      Public Property LoadingDiv As HTMLDiv(Of ChildControlObjectType)
      Public Property NavID As String = ""
      Public Property ResultsDiv As HTMLTag(Of ChildControlObjectType)
      Public Property TextBox As HTMLTag(Of ChildControlObjectType)
      Public Property OnRowSelect As String = ""
      Public Property HidePager As Boolean = False
      Public Property HideDescription As Boolean = False
      Public Property IgnoreRoot As Boolean = False
      Public Property ContainerCssClass As String = ""

      ' Public Property ButtonClass As String = "TBButton"
      Public Property FirstPageItem As PagerListItem(Of ChildControlObjectType)
      Public Property PrevPageItem As PagerListItem(Of ChildControlObjectType)
      Public Property NextPageItem As PagerListItem(Of ChildControlObjectType)
      Public Property LastPageItem As PagerListItem(Of ChildControlObjectType)
      Public Property CurrentPageItem As ListItem(Of ChildControlObjectType)
      Public Property PageCountItem As ListItem(Of ChildControlObjectType)

      Public Sub New()

      End Sub

      Public Sub New(PagingManagerJSProperty As String)
        Me.PagingManagerJSProperty = PagingManagerJSProperty
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        AllowClientSideSorting = True

        If PagingManagerJSProperty = "" Then
          mPagingManagerProperty = Singular.Reflection.GetMember(PagingManager)
          mPageManagerPropertyString = GetContext(mPagingManagerProperty.Name)
        Else
          mPageManagerPropertyString = PagingManagerJSProperty
        End If

        '"$root." & 
        If Not IgnoreRoot Then
          If Not mPageManagerPropertyString.StartsWith("$root.") Then
            mPageManagerPropertyString = "$root." & mPageManagerPropertyString
          End If
        End If

        If OnRowSelect <> "" Then
          Me.OnRowSelectJS = mPageManagerPropertyString & "().onRowSelectedBase(e, $data, $element," & OnRowSelect & ");"
        End If

        If Not HidePager Then
          PageNavigation = Helpers.Bootstrap.Nav
          With PageNavigation.NavTag
            .AddClass("Pager")
            If Me.NavID <> "" Then
              .Attributes("id") = Me.NavID
            End If
            With .Helpers.DivC("pull-right")
              Pager = .Helpers.Bootstrap.Pager
              With Pager
                FirstPageItem = .AddPagerItem(, , , "#", "First", Singular.Web.LinkTargetType.NotSet)
                With FirstPageItem
                  With .Anchor.AnchorTag
                    .AddClass("PagerButton btn-xs")
                    '.Helpers.Bootstrap.FontAwesomeIcon("fa-angle-double-left")
                  End With
                End With
                PrevPageItem = .AddPagerItem(, , , "#", "Previous", Singular.Web.LinkTargetType.NotSet)
                With PrevPageItem
                  With .Anchor.AnchorTag
                    .AddClass("PagerButton btn-xs")
                    '.Helpers.Bootstrap.FontAwesomeIcon("fa-angle-left")
                  End With
                End With
                CurrentPageItem = .AddListItem()
                With CurrentPageItem.ItemTag
                  With .Helpers.HTMLTag("span", "Page ")
                    .AddClass("page-number-label")
                  End With
                  TextBox = .Helpers.HTMLTag("input")
                  With TextBox
                    .Style.Width = 30
                    .Attributes("type") = "text"
                    .AddBinding(KnockoutBindingString.NValue, mPageManagerPropertyString & "().PageNo")
                    .AddClass("page-number-value")
                  End With
                  With .Helpers.HTMLTag("span")
                    .AddBinding(KnockoutBindingString.text, "' of ' + " & mPageManagerPropertyString & "().Pages()")
                    .AddClass("page-count-label")
                  End With
                End With
                NextPageItem = .AddPagerItem(, , , "#", "Next", Singular.Web.LinkTargetType.NotSet)
                With NextPageItem
                  With .Anchor.AnchorTag
                    .AddClass("PagerButton btn-xs")
                    '.Helpers.Bootstrap.FontAwesomeIcon("fa-angle-right")
                  End With
                End With
                LastPageItem = .AddPagerItem(, , , "#", "Last", Singular.Web.LinkTargetType.NotSet)
                With LastPageItem
                  With .Anchor.AnchorTag
                    .AddClass("PagerButton btn-xs")
                    '.Helpers.Bootstrap.FontAwesomeIcon("fa-angle-double-right")
                  End With
                End With
              End With
            End With
          End With
          If Not HideDescription Then
            With Helpers.HTMLTag("div")
              .AddBinding(KnockoutBindingString.text, mPageManagerPropertyString & "().PageDescription()")
              .AddClass("page-manager-description")
            End With
          End If
        End If

        LoadingDiv = Helpers.Div
        With LoadingDiv
          .AddClass("loading")
          .Helpers.Bootstrap.FontAwesomeIcon("fa-refresh").IconContainer.AddClass("fa-spin")
        End With
        LoadingDiv.Bindings.AddVisibilityBinding(mPageManagerPropertyString & "().IsLoading", VisibleFadeType.Fade, VisibleFadeType.Fade)

      End Sub

      Protected Friend Overrides Sub Render()

        Dim css As String = "class='"
        If ContainerCssClass <> "" Then
          css += ContainerCssClass
        End If
        If Responsive Then
          css += " table-responsive"
        End If
        css += "'"

        'Render the containing div
        Writer.Write("<div " & css & " data-bind=""PagedGrid: " & mPageManagerPropertyString & """>")

        'If Responsive Then
        '  Writer.Write("<div class='table-responsive'>")
        'End If

        'Render the paging toolbar
        If Not HidePager AndAlso Not PagerPosition = BootstrapEnums.PagerPosition.None AndAlso PagerPosition = BootstrapEnums.PagerPosition.Top Then
          PageNavigation.Render()
        End If

        'Render the table
        MyBase.Render()

        'If Responsive Then
        '  Writer.Write("</div>")
        'End If

        'Render the paging toolbar
        If Not HidePager AndAlso Not PagerPosition = BootstrapEnums.PagerPosition.None AndAlso PagerPosition = BootstrapEnums.PagerPosition.Bottom Then
          PageNavigation.Render()
        End If

        'If Not HidePager Then
        '  ResultsDiv.Render()
        'End If
        LoadingDiv.Render()

        Writer.Write("</div>")

      End Sub

    End Class

    'Public Class EditablePagedGrid(Of ObjectType, ChildControlObjectType)
    '  Inherits Table(Of ObjectType, ChildControlObjectType)

    '  Public Property EditablePagingManager As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))

    '  Private mPagingManagerProperty As PropertyInfo
    '  Public Property PagerPosition As BootstrapEnums.PagerPosition = BootstrapEnums.PagerPosition.Bottom
    '  Public Property PageNavigation As Bootstrap.Nav(Of ChildControlObjectType)
    '  Public Property Pager As Bootstrap.Pager(Of ChildControlObjectType)
    '  Public Property LoadingDiv As HTMLDiv(Of ChildControlObjectType)
    '  Public Property NavID As String = ""
    '  Public Property ResultsDiv As HTMLTag(Of ChildControlObjectType)
    '  Public Property TextBox As HTMLTag(Of ChildControlObjectType)


    '  ' Public Property ButtonClass As String = "TBButton"
    '  Public Property FirstPageItem As PagerListItem(Of ChildControlObjectType)
    '  Public Property PrevPageItem As PagerListItem(Of ChildControlObjectType)
    '  Public Property NextPageItem As PagerListItem(Of ChildControlObjectType)
    '  Public Property LastPageItem As PagerListItem(Of ChildControlObjectType)
    '  Public Property CurrentPageItem As ListItem(Of ChildControlObjectType)
    '  Public Property PageCountItem As ListItem(Of ChildControlObjectType)

    '  Protected Friend Overrides Sub Setup()
    '    MyBase.Setup()

    '    AllowClientSideSorting = False

    '    mPagingManagerProperty = Singular.Reflection.GetMember(EditablePagingManager)

    '    PageNavigation = Helpers.Bootstrap.Nav
    '    With PageNavigation.NavTag
    '      .AddClass("Pager")
    '      If Me.NavID <> "" Then
    '        .Attributes("id") = Me.NavID
    '      End If
    '      Pager = .Helpers.Bootstrap.Pager
    '      With Pager
    '        FirstPageItem = .AddPagerItem(, , , "#", "First", Singular.Web.LinkTargetType.NotSet)
    '        With FirstPageItem
    '          With .Anchor.AnchorTag
    '            .AddClass("PagerButton")
    '            '.Helpers.Bootstrap.FontAwesomeIcon("fa-angle-double-left")
    '          End With
    '        End With
    '        PrevPageItem = .AddPagerItem(, , , "#", "Previous", Singular.Web.LinkTargetType.NotSet)
    '        With PrevPageItem
    '          With .Anchor.AnchorTag
    '            .AddClass("PagerButton")
    '            '.Helpers.Bootstrap.FontAwesomeIcon("fa-angle-left")
    '          End With
    '        End With
    '        CurrentPageItem = .AddListItem()
    '        With CurrentPageItem
    '          .Helpers.HTMLTag("span", "Page ")
    '          TextBox = .Helpers.HTMLTag("input")
    '          With TextBox
    '            .Style.Width = 30
    '            .Attributes("type") = "text"
    '            .AddBinding(KnockoutBindingString.NValue, mPagingManagerProperty.Name & "().PageNo")
    '          End With
    '          With .Helpers.HTMLTag("span")
    '            .AddBinding(KnockoutBindingString.text, "' of ' + " & mPagingManagerProperty.Name & "().Pages()")
    '          End With
    '        End With
    '        NextPageItem = .AddPagerItem(, , , "#", "Next", Singular.Web.LinkTargetType.NotSet)
    '        With NextPageItem
    '          With .Anchor.AnchorTag
    '            .AddClass("PagerButton")
    '            '.Helpers.Bootstrap.FontAwesomeIcon("fa-angle-right")
    '          End With
    '        End With
    '        LastPageItem = .AddPagerItem(, , , "#", "Last", Singular.Web.LinkTargetType.NotSet)
    '        With LastPageItem
    '          With .Anchor.AnchorTag
    '            .AddClass("PagerButton")
    '            '.Helpers.Bootstrap.FontAwesomeIcon("fa-angle-double-right")
    '          End With
    '        End With
    '      End With
    '    End With

    '    ResultsDiv = Helpers.HTMLTag("p")
    '    ResultsDiv.AddBinding(KnockoutBindingString.text, mPagingManagerProperty.Name & "().PageDescription()")

    '    LoadingDiv = Helpers.Div
    '    With LoadingDiv
    '      .AddClass("loading")
    '      .Helpers.Bootstrap.FontAwesomeIcon("fa-refresh").IconContainer.AddClass("fa-spin")
    '    End With
    '    LoadingDiv.Bindings.AddVisibilityBinding(mPagingManagerProperty.Name & "().IsLoading", VisibleFadeType.Fade, VisibleFadeType.Fade)

    '  End Sub

    '  Protected Friend Overrides Sub Render()

    '    'Render the containing div
    '    Writer.Write("<div data-bind=""EditablePagedGrid: " & mPagingManagerProperty.Name & """>")

    '    'Render the paging toolbar
    '    If PagerPosition = BootstrapEnums.PagerPosition.Top Then
    '      PageNavigation.Render()
    '    End If

    '    'Render the table
    '    MyBase.Render()

    '    'Render the paging toolbar
    '    If PagerPosition = BootstrapEnums.PagerPosition.Bottom Then
    '      PageNavigation.Render()
    '    End If

    '    ResultsDiv.Render()
    '    LoadingDiv.Render()

    '    Writer.Write("</div>")

    '  End Sub

    'End Class

#End Region

#Region " Navigation "

    Public Class Nav(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Private mID As String = ""
      Private mNavCssClasses As String = ""
      Public Property NavTag As HTMLTag(Of ObjectType)

      Public Sub New(Optional ID As String = "", Optional NavCssClasses As String = "")
        mID = ID
        mNavCssClasses = NavCssClasses
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        NavTag = Helpers.HTMLTag("nav")
        If mID <> "" Then
          NavTag.Attributes("id") = mID
        End If
        If mNavCssClasses <> "" Then
          NavTag.AddClass(mNavCssClasses)
        End If
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class Pager(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property PagerListTag As UnorderedList(Of ObjectType)

      Public Sub New()

      End Sub

      Public Function AddPagerItem(Optional CssClass As String = "",
                                   Optional AnchorHRef As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                   Optional AnchorLinkText As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)) = Nothing,
                                   Optional AnchorHrefString As String = "",
                                   Optional AnchorLinkTextString As String = "",
                                   Optional AnchorTarget As Singular.Web.LinkTargetType = LinkTargetType.NotSet) As PagerListItem(Of ObjectType)

        Dim Item As PagerListItem(Of ObjectType) = PagerListTag.ListTag.Helpers.Bootstrap.PagerListItem(CssClass, AnchorHRef, AnchorLinkText, AnchorHrefString, AnchorLinkTextString, AnchorTarget)
        Return Item

      End Function

      Public Function AddListItem(Optional CssClass As String = "") As ListItem(Of ObjectType)

        Dim Item As ListItem(Of ObjectType) = PagerListTag.ListTag.Helpers.Bootstrap.ListItem(CssClass)
        Return Item

      End Function

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        PagerListTag = Helpers.Bootstrap.UnorderedList("pager")
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class Navbar(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property Navbar As HTMLTag(Of ObjectType)
      Public Property ContainerFluid As HTMLDiv(Of ObjectType)
      Public Property NavbarHeader As HTMLDiv(Of ObjectType)
      Public Property NavbarBrand As HTMLTag(Of ObjectType)
      Public Property NavbarHeaderButton As HTMLTag(Of ObjectType)
      Public Property NavbarCollapse As HTMLDiv(Of ObjectType)
      'Public Property NavbarCollapseLeft As UnorderedList(Of ObjectType)
      'Public Property NavbarForm As HTMLDiv(Of ObjectType)
      'Public Property NavbarCollapseRight As UnorderedList(Of ObjectType)

      Public Property NavbarStyle As BootstrapEnums.NavbarStyle = BootstrapEnums.NavbarStyle.Default
      Public Property IncludeBrand As Boolean = True
      Public Property ExcludeToggle As Boolean = False
      Public Property NavbarID As String = ""
      Public Property BrandName As String = ""

      Public Function AddList(Side As String) As UnorderedList(Of ObjectType)

        Dim CssClass As String = "nav navbar-nav"
        If Side = "Left" Then
          CssClass &= " navbar-left"
        ElseIf Side = "Right" Then
          CssClass &= " navbar-right"
        End If
        Return NavbarCollapse.Helpers.Bootstrap.UnorderedList(CssClass)

      End Function

      Public Function AddForm(Side As String) As HTMLDiv(Of ObjectType)

        Dim CssClass As String = "navbar-form"
        If Side = "Left" Then
          CssClass &= " navbar-left"
        ElseIf Side = "Right" Then
          CssClass &= " navbar-right"
        End If
        Return NavbarCollapse.Helpers.DivC(CssClass)

      End Function

      Public Sub New(NavbarStyle As BootstrapEnums.NavbarStyle,
                     NavbarID As String,
                     IncludeBrand As Boolean,
                     ExcludeToggle As Boolean,
                     Optional BrandName As String = "")
        Me.NavbarID = NavbarID
        Me.IncludeBrand = IncludeBrand
        Me.ExcludeToggle = ExcludeToggle
        Me.BrandName = BrandName
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Navbar = Helpers.HTMLTag("nav")
        With Navbar
          .AddClass("navbar")
          If NavbarStyle = BootstrapEnums.NavbarStyle.Default Then
            .AddClass("navbar-default")
          ElseIf NavbarStyle = BootstrapEnums.NavbarStyle.Inverted Then
            .AddClass("navbar-inverse")
          End If
          ContainerFluid = .Helpers.Bootstrap.ContainerFluid
          With ContainerFluid
            'Header------------------------------------------------------
            If Not ExcludeToggle Then
              NavbarHeader = .Helpers.DivC("navbar-header")
              If IncludeBrand Then
                With NavbarHeader
                  NavbarHeaderButton = .Helpers.HTMLTag("button")
                  With NavbarHeaderButton
                    .AddClass("navbar-toggle collapsed")
                    .Attributes("type") = "button"
                    .Attributes("data-toggle") = "button"
                    .Attributes("data-target") = "#" & NavbarID
                    With .Helpers.HTMLTag("span")
                      .AddClass("sr-only")
                      .Helpers.HTML("Toggle Navigation")
                    End With
                    .Helpers.HTMLTag("span").AddClass("icon-bar")
                    .Helpers.HTMLTag("span").AddClass("icon-bar")
                    .Helpers.HTMLTag("span").AddClass("icon-bar")
                  End With
                  If IncludeBrand Then
                    NavbarBrand = .Helpers.HTMLTag("a")
                    With NavbarBrand
                      .AddClass("navbar-brand")
                      .Attributes("href") = "#"
                      .Helpers.HTML(BrandName)
                    End With
                    '<a class="navbar-brand" href="#">Brand</a>
                  End If
                End With
              End If
            End If
            'Collapse------------------------------------------------------
            NavbarCollapse = .Helpers.DivC("collapse navbar-collapse")
            With NavbarCollapse
              .Attributes("id") = NavbarID
              'NavbarCollapseLeft = .Helpers.Bootstrap.UnorderedList("nav navbar-nav navbar-left")
              'NavbarForm = .Helpers.DivC("navbar-form")
              'NavbarCollapseRight = .Helpers.Bootstrap.UnorderedList("nav navbar-nav navbar-right")
            End With
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class ContainerFluid(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      Public Property Container As HTMLDiv(Of ObjectType)

      Public Sub New()
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()
        Container = Helpers.DivC("container-fluid")
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

#End Region

    'Public Class TableList(Of ObjectType, ChildControlObjectType)
    '  Inherits HelperBase(Of ObjectType, ChildControlObjectType) '

    '  Public Class ListItemRowTemplate
    '    Inherits ListItem(Of ObjectType)

    '    'Private mExpandButton As CustomControls.Button(Of ChildControlObjectType)
    '    'Friend mRemoveButton As CustomControls.Bootstrap.Button(Of ChildControlObjectType)
    '    'Friend mIsChildBandRow As Boolean = False
    '    'Public Property ExandVisibleBinding As String = ""

    '    'Friend Sub AddExpandButton(ExpandedProperty As String)
    '    '  mExpandButton = Helpers.Button("")
    '    '  mExpandButton.Style("width") = Singular.Web.Controls.ExpandButtonWidth & "px"
    '    '  mExpandButton.AddBinding(KnockoutBindingString.click, "$data." & ExpandedProperty & "(!$data." & ExpandedProperty & "())")
    '    '  mExpandButton.ButtonText.AddBinding(KnockoutBindingString.html, "($data." & ExpandedProperty & "() ? '-' : '+')")
    '    '  mExpandButton.AddBinding(KnockoutBindingString.title, "($data." & ExpandedProperty & "() ? 'Hide Child Rows.' : 'Expand Child Rows.')")
    '    '  If ExandVisibleBinding <> "" Then
    '    '    mExpandButton.AddBinding(KnockoutBindingString.visible, ExandVisibleBinding)
    '    '  End If
    '    '  mExpandButton.ButtonStyle = ButtonMainStyle.Default
    '    '  mExpandButton.ButtonSize = ButtonSize.ExtraSmall
    '    'End Sub

    '    'Public ReadOnly Property ExpandButton As CustomControls.Button(Of ChildControlObjectType)
    '    '  Get
    '    '    Return mExpandButton
    '    '  End Get
    '    'End Property

    '    Protected Friend Overrides Sub Setup()
    '      MyBase.Setup()

    '      'If CType(Parent, Bootstrap.Table(Of ObjectType, ChildControlObjectType)).AllowRemove Then
    '      '  mRemoveButton = Helpers.Bootstrap.Button(, , BootstrapEnums.Style.Danger, , BootstrapEnums.ButtonSize.ExtraSmall, , "fa-trash", , PostBackType.None, )
    '      '  mRemoveButton.Attributes("tabindex") = "-1"
    '      '  mRowButtons.Add(mRemoveButton)
    '      '  mRemoveButton.AddRemoveBinding()
    '      'End If

    '    End Sub

    '    Public Sub New()
    '      MyBase.New("")

    '    End Sub

    '    ' ''' <summary>
    '    ' ''' Adds a column with nothing in it.
    '    ' ''' </summary>
    '    'Public Function AddColumn(Optional HeaderText As String = "",
    '    '                          Optional xsSize As Integer? = Nothing, Optional smSize As Integer? = Nothing,
    '    '                          Optional mdSize As Integer? = Nothing, Optional lgSize As Integer? = Nothing,
    '    '                          Optional ColumnAlign As String = "") As TableColumn
    '    '  Dim Col = AddColumn(CType(Nothing, PropertyInfo), HeaderText)
    '    '  AddAutoClass(Col, True)
    '    '  If xsSize IsNot Nothing Then
    '    '    Col.AddClass("col-xs-" & xsSize.Value.ToString)
    '    '  End If
    '    '  If smSize IsNot Nothing Then
    '    '    Col.AddClass("col-sm-" & smSize.Value.ToString)
    '    '  End If
    '    '  If mdSize IsNot Nothing Then
    '    '    Col.AddClass("col-md-" & mdSize.Value.ToString)
    '    '  End If
    '    '  If lgSize IsNot Nothing Then
    '    '    Col.AddClass("col-lg-" & lgSize.Value.ToString)
    '    '  End If
    '    '  If ColumnAlign <> "" Then
    '    '    Col.Attributes("align") = ColumnAlign
    '    '  End If
    '    '  Return Col
    '    'End Function

    '    ' ''' <summary>
    '    ' ''' Adds an editable Column. 
    '    ' ''' </summary>
    '    ' ''' <param name="le">The property to bind the column to.</param>
    '    'Public Function AddColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), Optional HeaderText As String = "",
    '    '                          Optional xsSize As Integer? = Nothing, Optional smSize As Integer? = Nothing,
    '    '                          Optional mdSize As Integer? = Nothing, Optional lgSize As Integer? = Nothing,
    '    '                          Optional ColumnAlign As String = "") As TableColumn

    '    '  Dim Col As New TableColumn With {.HeaderText = HeaderText}
    '    '  If xsSize IsNot Nothing Then
    '    '    Col.AddClass("col-xs-" & xsSize.Value.ToString)
    '    '  End If
    '    '  If smSize IsNot Nothing Then
    '    '    Col.AddClass("col-sm-" & smSize.Value.ToString)
    '    '  End If
    '    '  If mdSize IsNot Nothing Then
    '    '    Col.AddClass("col-md-" & mdSize.Value.ToString)
    '    '  End If
    '    '  If lgSize IsNot Nothing Then
    '    '    Col.AddClass("col-lg-" & lgSize.Value.ToString)
    '    '  End If
    '    '  Col.For(le)
    '    '  SetWidthFromAttribute(Col)

    '    '  AddControl(Col)
    '    '  AddAutoClass(Col, False)
    '    '  If ColumnAlign <> "" Then
    '    '    Col.Attributes("align") = ColumnAlign
    '    '  End If
    '    '  Return Col

    '    'End Function

    '    ' ''' <summary>
    '    ' ''' Adds an editable Column. 
    '    ' ''' </summary>
    '    'Public Function AddColumn(pi As PropertyInfo, Optional HeaderText As String = "",
    '    '                          Optional ColumnAlign As String = "") As TableColumn

    '    '  Dim Col As New TableColumn With {.HeaderText = HeaderText}
    '    '  If pi IsNot Nothing Then
    '    '    Col.For(pi)
    '    '    SetWidthFromAttribute(Col)
    '    '  End If

    '    '  AddControl(Col)
    '    '  AddAutoClass(Col, False)
    '    '  If ColumnAlign <> "" Then
    '    '    Col.Attributes("align") = ColumnAlign
    '    '  End If
    '    '  Return Col

    '    'End Function

    '    ' ''' <summary>
    '    ' ''' Adds an editable Column. 
    '    ' ''' </summary>
    '    ' ''' <param name="le">The property to bind the column to.</param>
    '    'Public Function AddColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ColumnWidth As Integer,
    '    '                          Optional HeaderText As String = "",
    '    '                          Optional ColumnAlign As String = "") As TableColumn

    '    '  Dim col = AddColumn(le, HeaderText)
    '    '  If ColumnWidth > 0 Then
    '    '    col.Style.Width = ColumnWidth & "px"
    '    '  End If
    '    '  If ColumnAlign <> "" Then
    '    '    col.Attributes("align") = ColumnAlign
    '    '  End If
    '    '  Return col

    '    'End Function

    '    ' ''' <summary>
    '    ' ''' Adds a Column that displays the property but doesn't allow it to be edited.
    '    ' ''' </summary>
    '    ' ''' <param name="le">The Property to Display</param>
    '    'Public Function AddReadOnlyColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), Optional HeaderText As String = "",
    '    '                                  Optional xsSize As Integer? = Nothing, Optional smSize As Integer? = Nothing,
    '    '                                  Optional mdSize As Integer? = Nothing, Optional lgSize As Integer? = Nothing,
    '    '                                  Optional ColumnAlign As String = "") As TableColumn

    '    '  Dim Col As New TableColumn With {.HeaderText = HeaderText}
    '    '  Col.ReadOnlyColumn = True
    '    '  Col.AutoFormat = AutoFormat
    '    '  Col.For(le)
    '    '  SetWidthFromAttribute(Col)
    '    '  If xsSize IsNot Nothing Then
    '    '    Col.AddClass("col-xs-" & xsSize.Value.ToString)
    '    '  End If
    '    '  If smSize IsNot Nothing Then
    '    '    Col.AddClass("col-sm-" & smSize.Value.ToString)
    '    '  End If
    '    '  If mdSize IsNot Nothing Then
    '    '    Col.AddClass("col-md-" & mdSize.Value.ToString)
    '    '  End If
    '    '  If lgSize IsNot Nothing Then
    '    '    Col.AddClass("col-lg-" & lgSize.Value.ToString)
    '    '  End If
    '    '  AddAutoClass(Col, True)
    '    '  If ColumnAlign <> "" Then
    '    '    Col.Attributes("align") = ColumnAlign
    '    '  End If
    '    '  Return AddControl(Col)

    '    'End Function

    '    ' ''' <summary>
    '    ' ''' Adds a Column that displays the property but doesn't allow it to be edited.
    '    ' ''' </summary>
    '    'Public Function AddReadOnlyColumn(pi As PropertyInfo, Optional HeaderText As String = "",
    '    '                                  Optional ColumnAlign As String = "") As TableColumn

    '    '  Dim Col As New TableColumn With {.HeaderText = HeaderText}
    '    '  Col.ReadOnlyColumn = True
    '    '  Col.AutoFormat = AutoFormat
    '    '  If pi IsNot Nothing Then
    '    '    Col.For(pi)
    '    '    SetWidthFromAttribute(Col)
    '    '  End If
    '    '  AddAutoClass(Col, True)
    '    '  If ColumnAlign <> "" Then
    '    '    Col.Attributes("align") = ColumnAlign
    '    '  End If
    '    '  Return AddControl(Col)

    '    'End Function

    '    ' ''' <summary>
    '    ' ''' Adds a Column that displays the property but doesn't allow it to be edited.
    '    ' ''' </summary>
    '    ' ''' <param name="le">The Property to Display</param>
    '    'Public Function AddReadOnlyColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ColumnWidth As Integer,
    '    '                                  Optional HeaderText As String = "",
    '    '                                  Optional ColumnAlign As String = "") As TableColumn

    '    '  Dim Col = AddReadOnlyColumn(le, HeaderText)
    '    '  If ColumnWidth > 0 Then
    '    '    Col.Style.Width = ColumnWidth & "px"
    '    '  End If
    '    '  If ColumnAlign <> "" Then
    '    '    Col.Attributes("align") = ColumnAlign
    '    '  End If
    '    '  Return Col

    '    'End Function

    '    'Friend Sub RenderHeader(Index As Integer, RowCount As Integer)
    '    '  MyBase.Render()

    '    '  If IncludeInHeader Then
    '    '    Writer.WriteFullBeginTag("tr")

    '    '    'Expand Button Heading
    '    '    If mExpandButton IsNot Nothing Then
    '    '      Writer.WriteBeginTag("th")
    '    '      Writer.WriteAttribute("class", "LButtons")
    '    '      Writer.WriteAttribute("rowspan", RowCount)
    '    '      'Writer.WriteAttribute("width", "30px")
    '    '      Writer.WriteCloseTag(False)
    '    '      Writer.WriteEndTag("th")
    '    '    End If

    '    '    'Normal Cells
    '    '    For Each col As HelperBase In Controls
    '    '      If TypeOf col Is TableColumn Then
    '    '        CType(col, TableColumn).RenderHeader()
    '    '      End If
    '    '    Next

    '    '    'Remove Button Heading
    '    '    If Index = 0 AndAlso (mRowButtons.Count > 0) Then
    '    '      Writer.WriteBeginTag("th")
    '    '      'Writer.WriteAttribute("class", "RButtons")
    '    '      If RowCount > 1 Then
    '    '        Writer.WriteAttribute("rowspan", RowCount)
    '    '      End If
    '    '      Writer.WriteCloseTag(False)
    '    '      Writer.WriteEndTag("th")
    '    '    End If

    '    '    Writer.WriteEndTag("tr")
    '    '  End If

    '    'End Sub

    '    'Protected Overrides Sub RenderPreCells(ByVal Index As Integer, ByVal RowCount As Integer, ByVal TableHasChildBands As Boolean)

    '    '  'Expand Button
    '    '  If mExpandButton IsNot Nothing Then
    '    '    Writer.WriteBeginTag("td")
    '    '    Writer.WriteAttribute("class", "LButtons")
    '    '    If RowCount > 2 AndAlso Index = 0 Then
    '    '      Writer.WriteAttribute("rowspan", RowCount - 1)
    '    '    End If
    '    '    'Writer.WriteAttribute("width", "30px")
    '    '    Writer.WriteCloseTag(False)
    '    '    mExpandButton.Render()
    '    '    Writer.WriteEndTag("td")
    '    '  End If
    '    '  If mIsChildBandRow Then
    '    '    Writer.WriteFullBeginTag("td")
    '    '    Writer.WriteEndTag("td")
    '    '  End If

    '    'End Sub

    '  End Class

    '  Protected Friend Overrides Sub Setup()
    '    MyBase.Setup()

    '  End Sub

    'End Class

    'With .Helpers.HTMLTag("ul")
    '                    .AddClass("nav nav-pills nav-stacked contacts synergy-events-header")
    ''.AddClass("nav nav-pills nav-stacked contacts synergy-events-header")
    '                    With .Helpers.HTMLTag("li")
    ''.AddClass("online")
    '                        With .Helpers.HTMLTag("a")
    '                            With .Helpers.Div
    '                                .Style.Width = "100%"
    '                                .Style.FloatLeft()
    '                                With .Helpers.HTMLTag("span")
    '                                    .AddClass("col-sm-1 white-cell left-aligned")
    '                                    .Helpers.HTML("Channel")
    '                                End With
    '                                With .Helpers.HTMLTag("span")
    '                                    .AddClass("col-sm-1 white-cell")
    '                                    .Helpers.HTML("Date")
    '                                End With
    '                                With .Helpers.HTMLTag("span")
    '                                    .AddClass("col-sm-1 white-cell")
    '                                    .Helpers.HTML("Start")
    '                                End With
    '                                With .Helpers.HTMLTag("span")
    '                                    .AddClass("col-sm-1 white-cell")
    '                                    .Helpers.HTML("End")
    '                                End With
    '                                With .Helpers.HTMLTag("span")
    '                                    .AddClass("col-sm-5 white-cell left-aligned")
    '                                    .Helpers.HTML("Genre (Series)")
    '                                End With
    '                                With .Helpers.HTMLTag("span")
    '                                    .AddClass("col-sm-3 white-cell left-aligned")
    '                                    .Helpers.HTML("Title")
    '                                End With
    ''With .Helpers.HTMLTag("span")
    ''  .AddClass("col-sm-2 genre-series left-aligned")
    ''  .AddBinding(KnockoutBindingString.html, "$data.GenRefNumber()")
    ''End With
    '                            End With
    '                        End With
    '                    End With
    '                End With
    '                With .Helpers.HTMLTag("ul")
    '                    .AddClass("nav nav-pills nav-stacked contacts synergy-events")
    '                    .Attributes("id") = mTableID
    '                    With .Helpers.ForEach(Of ROSynergyEvent)(Function(d) d.ROSynergyEventList)
    '                        With .Helpers.HTMLTag("li")
    '                            With .Helpers.HTMLTag("a")
    '                                .Attributes("href") = "#"
    '                                With .Helpers.HTMLTag("div")
    '                                    .Style.Width = "100%"
    '                                    .Style.FloatLeft()
    '                                    With .Helpers.HTMLTag("span")
    '                                        .AddClass("col-sm-1 left-aligned")
    '                                        With .Helpers.HTMLTag("span")
    '                                            .AddBinding(KnockoutBindingString.html, "ROSynergyEventBO.ChannelShortNameHtml($data)")
    '                                            .AddBinding(KnockoutBindingString.css, "ROSynergyEventBO.ChannelShortNameCss($data)")
    '                                        End With
    '                                    End With
    '                                    With .Helpers.HTMLTag("span")
    '                                        .AddClass("col-sm-1")
    '                                        .AddBinding(KnockoutBindingString.html, "$data.ScheduleDateString()")
    '                                    End With
    '                                    With .Helpers.HTMLTag("span")
    '                                        .AddClass("col-sm-1")
    '                                        .AddBinding(KnockoutBindingString.html, "$data.ScheduleStartString()")
    '                                    End With
    '                                    With .Helpers.HTMLTag("span")
    '                                        .AddClass("col-sm-1")
    '                                        .AddBinding(KnockoutBindingString.html, "$data.ScheduleEndString()")
    '                                    End With
    '                                    With .Helpers.HTMLTag("span")
    '                                        .AddClass("col-sm-5 genre-series left-aligned")
    '                                        .AddBinding(KnockoutBindingString.html, "$data.GenreSeries()")
    '                                        .AddBinding(KnockoutBindingString.attr, "{ title: $data.GenreSeries() }")
    '                                    End With
    '                                    With .Helpers.HTMLTag("span")
    '                                        .AddClass("col-sm-3 genre-series left-aligned")
    '                                        .AddBinding(KnockoutBindingString.html, "$data.Title()")
    '                                        .AddBinding(KnockoutBindingString.attr, "{ title: $data.Title() }")
    '                                    End With
    '                                End With
    '                            End With
    '                        End With
    '                    End With
    '                End With

#Region " Flat Dream - Custom Controls "

    Public Class StaticDetailTile(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property TileTag As HTMLDiv(Of ObjectType)
      Public Property Detail As Boolean = True
      Public Property Clean As Boolean = True
      Public Property FDTileColor As BootstrapEnums.FDTileColor = BootstrapEnums.FDTileColor.Purple
      Public Property ContentTag As HTMLDiv(Of ObjectType)
      Public Property HeaderTag As HTMLTag(Of ObjectType)
      Public Property HeaderSubTextTag As HTMLTag(Of ObjectType)
      Public Property IconTag As HTMLDiv(Of ObjectType)
      Public Property IconName As String = ""
      Public Property AnchorTag As Anchor(Of ObjectType)
      Public Property ViewDetailIcon As FontAwesomeIcon(Of ObjectType)
      Public Property HeaderText As String = ""
      Public Property HeaderTextBinding As String = ""
      Public Property HeaderSubText As String = ""
      Public Property HeaderSubTextBinding As String = ""
      Public Property ViewDetailsClickBinding As String = ""
      Public Property LoadingDiv As HTMLDiv(Of ObjectType)

      Public Sub New(IconName As String,
                     Optional HeaderText As String = "",
                     Optional HeaderTextBinding As String = "",
                     Optional HeaderSubText As String = "",
                     Optional HeaderSubTextBinding As String = "",
                     Optional Detail As Boolean = True,
                     Optional Clean As Boolean = True,
                     Optional FDTileColor As BootstrapEnums.FDTileColor = BootstrapEnums.FDTileColor.Purple,
                     Optional ViewDetailsClickBinding As String = "")
        Me.IconName = IconName
        Me.HeaderText = HeaderText
        Me.HeaderTextBinding = HeaderTextBinding
        Me.HeaderSubText = HeaderSubText
        Me.HeaderSubTextBinding = HeaderSubTextBinding
        Me.Detail = Detail
        Me.Clean = Clean
        Me.FDTileColor = FDTileColor
        Me.ViewDetailsClickBinding = ViewDetailsClickBinding
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        TileTag = Helpers.DivC("fd-tile")
        If Detail Then
          TileTag.AddClass("detail")
        End If
        If Clean Then
          TileTag.AddClass("clean")
        End If
        'Color Class
        TileTag.AddClass(GetFDTileColorClass(FDTileColor))
        With TileTag
          ContentTag = .Helpers.DivC("content")
          With ContentTag
            HeaderTag = .Helpers.HTMLTag("h1")
            HeaderTag.AddClass("text-left")
            With HeaderTag
              If HeaderTextBinding <> "" Then
                With .Helpers.HTMLTag("span")
                  .AddBinding(KnockoutBindingString.html, HeaderTextBinding)
                End With
              Else
                With .Helpers.HTMLTag("span")
                  .Helpers.HTML(HeaderText)
                End With
              End If
            End With
            With HeaderTag
              HeaderSubTextTag = .Helpers.HTMLTag("p")
              With HeaderSubTextTag
                If HeaderSubTextBinding <> "" Then
                  .AddBinding(KnockoutBindingString.html, HeaderSubTextBinding)
                Else
                  .Helpers.HTML(HeaderSubText)
                End If
              End With
            End With
          End With
          IconTag = .Helpers.DivC("icon")
          With IconTag
            .Helpers.Bootstrap.FontAwesomeIcon(IconName)
          End With
          AnchorTag = .Helpers.Bootstrap.Anchor(, , "#", "View Details", LinkTargetType.NotSet, , )
          With AnchorTag.AnchorTag
            .AddClass("details")
            With .Helpers.HTMLTag("span")
              ViewDetailIcon = .Helpers.Bootstrap.FontAwesomeIcon("fa-arrow-circle-right")
              ViewDetailIcon.IconContainer.AddClass("pull-right")
            End With
            If ViewDetailsClickBinding <> "" Then
              .AddBinding(KnockoutBindingString.click, ViewDetailsClickBinding)
            End If
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

    Public Class FDTableBlock(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Private mContainerDiv As HTMLDiv(Of ObjectType)
      Public Property FlatBlockTag As HTMLDiv(Of ObjectType)
      Public Property HeaderContainer As HTMLDiv(Of ObjectType)
      Public Property HeaderTag As HTMLTag(Of ObjectType)
      Public Property HeaderText As String = ""
      Public Property ContentTag As HTMLDiv(Of ObjectType)
      Public Property AboveTableContentTag As HTMLDiv(Of ObjectType)
      Public Property TableResponsiveTag As HTMLDiv(Of ObjectType)

      Public Sub New(Optional HeaderText As String = "")

        Me.HeaderText = HeaderText

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        FlatBlockTag = Helpers.DivC("block-flat")
        With FlatBlockTag
          HeaderContainer = .Helpers.DivC("header")
          With HeaderContainer
            HeaderTag = .Helpers.HTMLTag("h3")
            With HeaderTag
              .Helpers.HTML(HeaderText)
            End With
          End With
          ContentTag = .Helpers.DivC("content")
          With ContentTag
            AboveTableContentTag = .Helpers.Div
            TableResponsiveTag = .Helpers.DivC("table-responsive")
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class FlatBlock(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Private mContainerDiv As HTMLDiv(Of ObjectType)
      Public Property FlatBlockTag As HTMLDiv(Of ObjectType)
      Public Property HeaderContainer As HTMLDiv(Of ObjectType)
      Public Property HeaderTag As HTMLTag(Of ObjectType)
      Public Property ContentTag As HTMLDiv(Of ObjectType)
      Public Property AboveContentTag As HTMLDiv(Of ObjectType)
      Public Property ActionsDiv As HTMLDiv(Of ObjectType)

      Public Property HeaderText As String = ""
      Public Property HasContentAbove As Boolean = False
      Public Property ExcludeHeader As Boolean = False
      Public Property DarkBox As Boolean = False
      Public Property ClassOverride As String = ""
      Public Property IncludeActionsDiv As Boolean = False

      Public Sub New(Optional HeaderText As String = "",
                     Optional HasContentAbove As Boolean = False,
                     Optional ExcludeHeader As Boolean = False,
                     Optional DarkBox As Boolean = False,
                     Optional ClassOverride As String = "",
                     Optional IncludeActionsDiv As Boolean = False)

        Me.HeaderText = HeaderText
        Me.HasContentAbove = HasContentAbove
        Me.ExcludeHeader = ExcludeHeader
        Me.DarkBox = DarkBox
        Me.ClassOverride = ClassOverride
        Me.IncludeActionsDiv = IncludeActionsDiv

      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        If Me.ClassOverride <> "" Then
          FlatBlockTag = Helpers.DivC(Me.ClassOverride)
        Else
          FlatBlockTag = Helpers.DivC("block-flat")
        End If
        If DarkBox Then
          FlatBlockTag.AddClass("dark-box")
        End If
        With FlatBlockTag
          If Not ExcludeHeader Then
            HeaderContainer = .Helpers.DivC("header")
            With HeaderContainer
              If IncludeActionsDiv Then
                ActionsDiv = HeaderContainer.Helpers.DivC("actions")
              End If
              HeaderTag = HeaderContainer.Helpers.HTMLTag("h3")
              With HeaderTag
                .Helpers.HTML(HeaderText)
              End With
            End With
          End If
          ContentTag = .Helpers.DivC("content")
          With ContentTag
            If HasContentAbove Then
              AboveContentTag = .Helpers.Div
            End If
          End With
        End With

      End Sub

      Protected Friend Overrides Sub Render()

        MyBase.Render()
        RenderChildren()

      End Sub

    End Class

    Public Class Portlet(Of ObjectType)
      Inherits HelperBase(Of ObjectType)

      'Properties
      Public Property BlockTag As HTMLDiv(Of ObjectType)
      Public Property HeaderContainerTag As HTMLDiv(Of ObjectType)
      Public Property HeaderActionsTag As HTMLDiv(Of ObjectType)
      Public Property HeaderTag As HTMLTag(Of ObjectType)
      Public Property ContentTag As HTMLDiv(Of ObjectType)

      Public Property PortletColor As BootstrapEnums.PortletColor = BootstrapEnums.PortletColor.Primary
      Public Property FullTileColor As Boolean = True
      Public Property HeaderText As String = ""
      Public Property HeaderTextBinding As String = ""
      Public Property HeaderSubText As String = ""
      Public Property HeaderSubTextBinding As String = ""

      Public Sub New(IconName As String,
                     Optional HeaderText As String = "",
                     Optional HeaderTextBinding As String = "",
                     Optional HeaderSubText As String = "",
                     Optional HeaderSubTextBinding As String = "",
                     Optional Detail As Boolean = True,
                     Optional Clean As Boolean = True,
                     Optional FDTileColor As BootstrapEnums.FDTileColor = BootstrapEnums.FDTileColor.Purple,
                     Optional ViewDetailsClickBinding As String = "")
        Me.HeaderText = HeaderText
        Me.HeaderTextBinding = HeaderTextBinding
        Me.HeaderSubText = HeaderSubText
        Me.HeaderSubTextBinding = HeaderSubTextBinding
      End Sub

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        BlockTag = Helpers.Div
        If FullTileColor Then
          BlockTag.AddClass("block-flat")
          'BlockTag.AddClass(GetPortletFullTileColor())
        Else
          BlockTag.AddClass("block block-color")
          'BlockTag.AddClass(GetPortletHeaderColor())
        End If

        With BlockTag
          HeaderContainerTag = .Helpers.DivC("header")
          With HeaderContainerTag
            HeaderActionsTag = .Helpers.DivC("actions")
            With HeaderActionsTag

            End With
            HeaderTag = .Helpers.HTMLTag("h3")
            With HeaderTag
              If HeaderTextBinding <> "" Then
                .AddBinding(KnockoutBindingString.html, HeaderTextBinding)
              Else
                .Helpers.HTML(HeaderText)
              End If
            End With
          End With
          ContentTag = .Helpers.DivC("content")
        End With

      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()
        RenderChildren()
      End Sub

    End Class

#End Region

  End Class

End Namespace


