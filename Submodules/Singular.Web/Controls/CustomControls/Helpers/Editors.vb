Imports System.Reflection
Imports System.Dynamic
Imports System.Linq.Expressions
Imports Singular.Web.Controls.HelperControls
Imports System.ComponentModel.DataAnnotations
Imports Singular.DataAnnotations

Namespace CustomControls

#Region " Editor Base "

  ''' <summary>
  ''' Base class for input and select controls.
  ''' </summary>
  Public MustInherit Class EditorBase(Of ObjectType)
    Inherits Controls.HelperControls.HelperBase(Of ObjectType)

    'Used when not binding to a strongly typed object.
    Protected mBindProperty As String
    Protected mBindPath As String

    ''' <summary>
    ''' Causes the page to post back when the value is changed on the client.
    ''' </summary>
    Public Property AutoPostBack As Boolean = False

    Friend KeepInline As Boolean = False 'True if the label and editor must be kept inline, even when the screen is narrow.

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AddBindingAttributes()

      If PropertyInfo IsNot Nothing Then

        Tooltip = Singular.Reflection.GetDescription(PropertyInfo)

        If Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.AutoPostBack)(PropertyInfo) IsNot Nothing Then
          AutoPostBack = True
        End If

        Dim NoUpdateAttr = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NonUpdatable)(PropertyInfo)
        If NoUpdateAttr IsNot Nothing Then
          If NoUpdateAttr.AllowedRole = "" Then
            AddBinding(KnockoutBindingString.enable, "IsNew")
          Else

          End If
        End If

      End If

    End Sub

    ''' <summary>
    ''' Returns the correct editor for the specified property type.
    ''' </summary>
    Public Shared Function GetEditor(le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As EditorBase(Of ObjectType)
      Dim pi As MemberInfo = Singular.Reflection.GetMember(Of ObjectType)(le)
      Dim editor As EditorBase(Of ObjectType) = GetCorrectType(pi)
      editor.For(le)
      Return editor
    End Function

    Private Shared Function GetCorrectType(ByVal pi As MemberInfo) As EditorBase(Of ObjectType)

      Dim cmi = Singular.ReflectionCached.GetCachedMemberInfo(pi)

      If cmi.DropDownWebAttribute IsNot Nothing Then
        Return New DropDownEditor(Of ObjectType)
      ElseIf Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.FileInput)(pi) IsNot Nothing Then
        Return New FileEditor(Of ObjectType)
      ElseIf Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.RadioButtonList)(pi) IsNot Nothing Then
        Return New RadioButtonEditor(Of ObjectType)
      ElseIf Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.Slider)(pi) IsNot Nothing Then
        Return New SliderEditor(Of ObjectType)
      ElseIf Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateAndTimeField)(pi) IsNot Nothing Then
        Return New DateAndTimeEditor(Of ObjectType)
      ElseIf Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.Select2Field)(pi) IsNot Nothing Then
        Return New Select2(Of ObjectType)
      Else
        'Dim sPi As New Singular.Reflection.SMemberInfo(pi)
        Select Case cmi.MainDataType
          Case Reflection.SMemberInfo.MainType.Number
            Return New NumericEditor(Of ObjectType)
          Case Reflection.SMemberInfo.MainType.Date
            If Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.TimeField)(pi) Is Nothing Then
              Return New DateEditor(Of ObjectType)
            Else
              Return New TimeEditor(Of ObjectType)
            End If
          Case Reflection.SMemberInfo.MainType.Boolean
            Return New CheckBoxEditor(Of ObjectType)
          Case Else
            Return New TextEditor(Of ObjectType)
        End Select

      End If
    End Function

    Public Shared Function GetEditor(pi As PropertyInfo, BindingObject As Object) As EditorBase(Of ObjectType)
      Dim editor As EditorBase(Of ObjectType) = GetCorrectType(pi)
      editor.For(pi, BindingObject)
      Return editor
    End Function

    Public Shared Function GetEditor(pi As PropertyInfo) As EditorBase(Of ObjectType)
      Dim editor As EditorBase(Of ObjectType) = GetCorrectType(pi)
      editor.For(pi)
      Return editor
    End Function

    Friend Overrides ReadOnly Property ControlSettingType As ControlSettingType
      Get
        Return ControlSettingType.Editor
      End Get
    End Property

    Protected MustOverride Sub WriteTypeAttribute()
    Protected Overridable Sub WriteClassAttribute()
      WriteClass()
    End Sub
    Protected Overridable Sub WriteOtherAttributes()

    End Sub
    Protected Overridable Sub WriteStartTag()
      Writer.WriteBeginTag("input")
      If PropertyInfo IsNot Nothing AndAlso Not PropertyInfo.CanWrite Then
        MakeReadOnly()
      End If
    End Sub

    Protected Sub MakeReadOnly()
      Writer.WriteAttribute("readonly", "readonly")
      Writer.WriteAttribute("tabindex", "-1")
      AddClass("read-only")
    End Sub

    Protected Overridable Sub WriteEndTag()
      Writer.WriteFullCloseTag()
      Writer.WriteLine()
    End Sub

    Protected Sub AddValueBinding()

      If mBindingMode = LocationType.Client Then
        If mBindProperty IsNot Nothing Then
          Bindings.Add(KnockoutBindingString.SValue, GetContext(mBindPath, mBindProperty))
        Else
          Bindings.Add(KnockoutBindingString.SValue, GetForJS())
        End If

      Else
        Writer.WriteAttribute("value", GetValue(mFormatString))
      End If

    End Sub

    Protected MustOverride Sub AddBindingAttributes()

    Protected mFormatString As String = ""

    Public Property FormatString As String
      Get
        Return mFormatString
      End Get
      Set(value As String)
        mFormatString = value
        AddBindingAttributes()
      End Set
    End Property

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        WriteStartTag()

        WriteTypeAttribute()
        WriteClassAttribute()
        WriteOtherAttributes()
        WriteControlAttributes()

        Bindings.WriteKnockoutBindings()

        If mBindingMode = LocationType.Server Then

          Writer.WriteAttribute("id", ClientID)
          Writer.WriteAttribute("name", ClientID)

          'Check if there are any data errors.
          Dim deo As System.ComponentModel.IDataErrorInfo = TryCast(mBindingObject, System.ComponentModel.IDataErrorInfo)
          If deo IsNot Nothing AndAlso deo(PropertyInfo.Name) <> "" Then
            Style("border") = "1px red solid"
            Writer.WriteAttribute("title", deo(PropertyInfo.Name))
          End If

        End If

        WriteStyles()

        If Not String.IsNullOrEmpty(Tooltip) Then
          Writer.WriteAttribute("title", Tooltip)
        End If

        If AutoPostBack Then
          Dim apbA = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.AutoPostBack)(PropertyInfo)
          If apbA IsNot Nothing AndAlso apbA.CommandName <> "" Then
            Writer.WriteAttribute("data-AutoPost", apbA.CommandName)
          Else
            Writer.WriteAttribute("data-AutoPost", PropertyInfo.Name)
          End If

        End If

        WriteEndTag()

      End With

    End Sub

    'Protected Overrides Sub OnLoad(e As System.EventArgs)
    '  MyBase.OnLoad(e)

    '  'If the bindind is happening on the server, then set the underlying objects value on post back.
    '  If IsPostBack AndAlso mBindingMode = LocationType.Server AndAlso HasPostBackValue() Then
    '    Dim Value As Object = GetPostBackValue()
    '    If Not Singular.Misc.CompareSafeEmptyString(Value, mPreviousValue) Then
    '      PropertyInfo.SetValue(mBindingObject, Singular.Reflection.ConvertValueToType(PropertyInfo.PropertyType, GetPostBackValue), Nothing)
    '    End If
    '  End If
    'End Sub

    'Protected Overridable Function GetPostBackValue() As Object
    '  If PropertyInfo IsNot Nothing AndAlso mBindingObject IsNot Nothing Then
    '    Return Request.Form(ClientID)
    '  End If
    '  Return Nothing
    'End Function

    'Protected Overridable Function HasPostBackValue() As Boolean
    '  Return mHadRoundTrip AndAlso Request.Form.AllKeys.Contains(ClientID)
    'End Function

    'Protected mHadRoundTrip As Boolean = False
    'Protected mPreviousValue As Object

    'Protected Overrides Sub LoadViewState(savedState As Object)
    '  mHadRoundTrip = True
    '  Dim p As System.Web.UI.Pair = savedState
    '  If p.Second Then
    '    mPreviousValue = p.First
    '  Else
    '    mPreviousValue = Nothing
    '  End If
    'End Sub

    'Protected Overrides Function SaveViewState() As Object
    '  Dim Obj As Object = GetValue(mFormatString)
    '  Return New System.Web.UI.Pair(Obj, Obj IsNot Nothing)

    'End Function

  End Class

#End Region

  ''' <summary>
  ''' Creates a select element on the server. The options are created by the knockout library.
  ''' </summary>
  Public Class DropDownEditor(Of t)
    Inherits EditorBase(Of t)

    'Custom drop down - not a normal html select.
    Private IsCustom As Boolean = False
    Private ShowFindScreen As Boolean = False

    Private IsMultiSelect As Boolean = False
    Private IsAutoComplete As Boolean = False 'True if this is a find screen, and the display value is stored inline with the object.
    Private PrimarySearchField As PropertyInfo
    Private SearchFieldCount As Integer
    Private DDA As DropDownWeb
    Private AOTF As AddOnTheFly

    Private Function GetDropDownType() As DropDownWeb.SelectType
      If DDA.DropDownType = DropDownWeb.SelectType.Auto Then
        Return Singular.Web.Controls.DefaultDropDownType
      Else
        Return DDA.DropDownType
      End If
    End Function

    Public Sub New()

    End Sub

    Public Sub New(BindProperty As String, BindPath As String, DropDownOptions As DropDownWeb)
      DDA = DropDownOptions

      mBindProperty = BindProperty
      mBindPath = BindPath
    End Sub

    Protected Friend Overrides Sub Setup()

      If PropertyInfo IsNot Nothing Then
        DDA = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DropDownWeb)(PropertyInfo)
        AOTF = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.AddOnTheFly)(PropertyInfo)
        IsMultiSelect = GetType(IList).IsAssignableFrom(PropertyInfo.PropertyType)
        DDA.SetMultiSelect(IsMultiSelect)
      End If

      Dim Type = GetDropDownType()
      IsCustom = Type <> DropDownWeb.SelectType.NormalDropDown

      If IsCustom Then
        Style("cursor") = "pointer"

        If DDA.LookupMember <> "" OrElse Type = DropDownWeb.SelectType.AutoComplete OrElse Type = DropDownWeb.SelectType.FindScreen Then
          IsAutoComplete = True

          With DDA.GetPrimarySearchField
            PrimarySearchField = .Item1
            SearchFieldCount = .Item2
          End With
        End If

        ShowFindScreen = Type = DropDownWeb.SelectType.FindScreen
        'If there is only 1 search criteria in the find screen, which is the primary search field, then dont show the find screen.
        'The user will find records by typing into the combo. Showing the find screen button makes things confusing.
        If PrimarySearchField IsNot Nothing AndAlso SearchFieldCount <= 1 Then ShowFindScreen = False

      End If

      If Singular.Web.Controls.AddBootstrapClasses Then AddClass("form-control")

      If GetDropDownType() = Singular.DataAnnotations.DropDownWeb.SelectType.Combo Then
        AddClass("ComboBox")
      End If

      'If IsMultiSelect AndAlso GetDropDownType() <> DataAnnotations.DropDownWeb.SelectType.FindScreen Then
      '  Throw New Exception("Multiselect combos must have DropDownType = FindScreen")
      'End If

      MyBase.Setup()
    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      If IsCustom Then
        Writer.WriteAttribute("type", InputType.text.ToString)
      End If
    End Sub

    Protected Overrides Sub WriteStartTag()
      If IsCustom Then
        Writer.WriteBeginTag("input")

        If ShowFindScreen AndAlso (Not IsAutoComplete OrElse PrimarySearchField Is Nothing) Then
          MakeReadOnly()
        End If

        If DDA.DropDownType = DropDownWeb.SelectType.DropDownList Then
          MakeReadOnly()

        End If

      Else
        Writer.WriteBeginTag("select")
      End If

      If PropertyInfo IsNot Nothing AndAlso Not PropertyInfo.CanWrite Then
        Writer.WriteAttribute("disabled", "disabled")
      End If
    End Sub

    Protected Overrides Sub WriteEndTag()

      If IsCustom Then
        Writer.WriteFullCloseTag()
      Else
        Writer.WriteCloseTag(False)
        If GetDropDownType() = Singular.DataAnnotations.DropDownWeb.SelectType.Combo Then
          Writer.WriteEndTag("input")
        Else
          Writer.WriteEndTag("select")
        End If
      End If

    End Sub

    Protected Overrides Sub AddBindingAttributes()

      If DDA IsNot Nothing Then
        If DDA.ListType IsNot Nothing AndAlso Not IsAutoComplete Then
          Helpers.AddDropDownDataSource(DDA)
        End If

        If AOTF IsNot Nothing Then

          Dim ri = Helpers.Page.ModelNonGeneric.JSSerialiser.RootObjectInfo
          Dim om As Data.JS.ObjectInfo.ObjectMember = ri.CreateMember(AOTF.VMPropertyName, AOTF.NewType)

        End If

        If IsCustom Then

          AddComboBindings()

        Else

          AddHTMLSelectBindings()

        End If

        'unique name, value and display member
        If GetDropDownType() <> Singular.DataAnnotations.DropDownWeb.SelectType.Combo Then
          AddBinding(KnockoutBindingString.optionsValue, DDA.ValueMember.AddSingleQuotes)
          AddBinding(KnockoutBindingString.optionsText, DDA.DisplayMember.AddSingleQuotes)
          If DDA.TooltipMember <> "" Then
            AddBinding(KnockoutBindingString.itemTitle, DDA.TooltipMember.AddSingleQuotes)
          End If
        End If

      End If

    End Sub

    Private Sub AddComboBindings()

      Dim sb As New Text.StringBuilder

      'Selected Value
      If mBindProperty IsNot Nothing Then
        sb.Append("{ Value: " & GetContext(mBindPath, mBindProperty))
      Else
        sb.Append("{ Value: " & GetForJS())
      End If

      'Value / Display Member
      sb.Append(", ValueMember: " & DDA.ValueMember.AddSingleQuotes & ", Display: " & DDA.GetDisplayMember)

      'Grouped list
      If DDA.GroupChildListMember <> "" Then
        sb.Append(", ChildList: " & DDA.GroupChildListMember.AddSingleQuotes & ", GroupDisplay: " & DDA.GroupMember.AddSingleQuotes)
      End If

      'Prompt text
      If DDA.UnselectedText <> " " Then
        If DDA.UnselectedText.StartsWith("LocalText.") Then
          sb.Append(", Caption: " & Singular.Localisation.LocalText(DDA.UnselectedText.Substring(10)).AddSingleQuotes)
        Else
          sb.Append(", Caption: " & DDA.UnselectedText.AddSingleQuotes)
        End If
      End If

      'Display columns
      If DDA.DropDownColumns.Length > 0 Then
        Dim ChildType As Type = Nothing
        If DDA.ListType IsNot Nothing Then
          ChildType = Singular.Reflection.GetLastGenericType(DDA.ListType)
        End If
        sb.Append(", Columns: [")
        Dim First As Boolean = True
        For Each col As String In DDA.DropDownColumns

          If Not First Then
            sb.Append(",")
          End If
          First = False

          If ChildType IsNot Nothing Then
            Dim pi = Singular.Reflection.GetProperty(ChildType, col)
            If pi Is Nothing Then
              Throw New Exception("Dropdown column '" & col & "' not found")
            End If
            Dim pic = Singular.ReflectionCached.GetCachedMemberInfo(pi)
            Dim fs As String = ""

            Dim nf As Singular.DataAnnotations.NumberField = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(pi)

            If nf IsNot Nothing AndAlso nf.HasFormat Then
              fs = nf.GetFormatString(True)
            End If

            sb.Append("[" & pic.PropertyName.AddSingleQuotes & ", " & pic.DisplayName.AddSingleQuotes & ", " & pic.MainDataTypeShortString.AddSingleQuotes &
                      If(fs = "", "", ", " & fs) & "]")
          Else
            sb.Append("[" & col.AddSingleQuotes & ", " & Singular.Strings.Readable(col).AddSingleQuotes & ", 's']")
          End If

        Next
        sb.Append("]")
      End If

      'Source
      If IsAutoComplete Then

        sb.Append(", LookupMember: " & DDA.LookupMember.AddSingleQuotes & ", FetchType: " & Singular.ReflectionCached.GetCachedType(DDA.GetCriteriaClass).DotNetTypeName.AddSingleQuotes)
        If PrimarySearchField IsNot Nothing Then
          sb.Append(", SearchField: " & PrimarySearchField.Name.AddSingleQuotes)
        End If
        If DDA.PreFindJSFunction <> "" Then
          sb.Append(", PreFind: " & DDA.PreFindJSFunction)
        End If
        If DDA.BeforeFetchJS <> "" Then
          sb.Append(", PreFetchJS: " & DDA.BeforeFetchJS)
        End If
        If DDA.AfterFetchJS <> "" Then
          If DDA.AfterFetchJS.EndsWith(")") Then
            sb.Append(", AfterFetchJS: function(args) {" & DDA.AfterFetchJS & "}")
          Else
            sb.Append(", AfterFetchJS: " & DDA.AfterFetchJS)
          End If
        End If

      Else

        If DDA.AjaxFetch Then

          sb.Append(", AjaxName: " & DDA.Name.AddSingleQuotes & ", Filter: " & DDA.ThisFilterMember & ", FilterName: " & DDA.DataSourceFilterMember.AddSingleQuotes)
          If DDA.PreFindJSFunction <> "" Then
            sb.Append(", PreFind: " & DDA.PreFindJSFunction)
          End If
          If DDA.AfterFetchJS <> "" Then
            If DDA.AfterFetchJS.EndsWith(")") Then
              sb.Append(", AfterFetchJS: function(args) {" & DDA.AfterFetchJS & "}")
            Else
              sb.Append(", AfterFetchJS: " & DDA.AfterFetchJS)
            End If
          End If

        Else

          Dim DataSource = DDA.ClientName
          If DDA.FilterMethodName <> "" Then
            DataSource = DDA.FilterMethodName & "(" & DataSource & ", " & "$data" & ")"
          End If

          If DDA.DataSourceFilterMember <> "" Then
            sb.Append(", ListFilter: " & DDA.DataSourceFilterMember.AddSingleQuotes)
            If DDA.FilterConstant IsNot Nothing Then
              sb.Append(", Filter: " & System.Web.Helpers.Json.Encode(DDA.FilterConstant).Replace("""", "'"))
            Else
              sb.Append(", Filter: " & GetContext(DDA.ThisFilterMember))
            End If

          End If

          sb.Append(", Source: " & DataSource)
        End If

      End If

      sb.Append(", DDType: " & CInt(GetDropDownType()))

      If DDA.OnItemSelectJSFunction <> "" Then
        sb.Append(", OnItemSelect: " & DDA.OnItemSelectJSFunction)
      End If

      If ShowFindScreen Then
        sb.Append(", FindCriteria: ViewModel." & DDA.ListType.Name & DDA.GetCriteriaClass.Name)
      End If

      If IsMultiSelect Then
        sb.Append(", MultiSelect: true")
      End If

      'Unique
      If DDA.UniqueInList Then
        sb.Append(", Unique: true")
      End If

      'None selection
      If DDA.ComboDeselectText <> "" Then
        sb.Append(", ClearText: " & DDA.ComboDeselectText.AddSingleQuotes)
      End If

      'Old Ind
      If DDA.OldFilterProperties.Length > 0 Then
        sb.Append(", OldMembers: [" & GetFilterPropertiesString() & "]")
      End If

      'On Cell Create
      If DDA.OnCellCreateFunction <> "" Then
        sb.Append(", OnCellCreate: " & DDA.OnCellCreateFunction)
      End If

      'Add on the fly
      If AOTF IsNot Nothing Then
        sb.Append(", AOTF: { VMProperty: $root." & AOTF.VMPropertyName & ", Prompt: '" & AOTF.PromptText & "'" & If(AOTF.AfterAddFunctionName = "", "", ", AfterAddFunction: " & AOTF.AfterAddFunctionName) & " }")
      End If

      'Allow Not in List
      If DDA.AllowNotInList Then
        sb.Append(", AllowNotInList: true")
      End If

      If DDA.DropDownCssClass <> "" Then
        sb.Append(", DropDownCssClass: " & DDA.DropDownCssClass.AddSingleQuotes)
      End If

      sb.Append(", KeyDelay: " & DDA.KeyDelay)

      'Dim BindingString = "{ Value: " & GetForJS() & ", Source: " & DDA.ClientName & ", ValueMember: " & DDA.ValueMember.AddSingleQuotes & ", Display: " & DDA.GetDisplayMember


      AddBinding(KnockoutBindingString.SCombo, sb.ToString & "}")

    End Sub

    Private Sub AddHTMLSelectBindings()

      If mBindingMode = LocationType.Client Then
        If mBindProperty IsNot Nothing Then
          AddBinding(KnockoutBindingString.SValue, GetContext(mBindPath, mBindProperty))
        Else
          AddBinding(KnockoutBindingString.SValue, GetForJS)
        End If

      Else
        Dim ValueString As String = GetValueString("")
        If ValueString = "" Then
          ValueString = "undefined"
        End If
        AddBinding(KnockoutBindingString.value, ValueString)
      End If

      'Tell knockout to add an extra item as the blank selection.
      If DDA.AllowBlank Then
        AddBinding(KnockoutBindingString.optionsCaption, DDA.UnselectedText.AddSingleQuotes)
      End If

      If DDA.AjaxFetch Then
        AddBinding(KnockoutBindingString.DropDown, "{AjaxName: " & DDA.Name.AddSingleQuotes & ", Filter: " & DDA.ThisFilterMember & ", FilterName: " & DDA.DataSourceFilterMember.AddSingleQuotes & If(DDA.AfterFetchJS = "", "", ", AfterFetchJS: function() {" & DDA.AfterFetchJS & "}") & "}")
      Else
        AddBinding(KnockoutBindingString.DropDown, "{Source: " & GetSelectSourceJS() & "}")
      End If

      If DDA.GroupMember <> "" Then
        'If its a 2 level drop down
        AddBinding(KnockoutBindingString.optionsGroupList, DDA.GroupChildListMember.AddSingleQuotes)
        AddBinding(KnockoutBindingString.groupText, DDA.GroupMember.AddSingleQuotes)
      End If

    End Sub

    Private Function GetSelectSourceJS() As String
      Dim SourcePath As String = DDA.ClientName
      '*** Filter Unique ***
      If DDA.UniqueInList Then
        'MOVE to binding handler
        SourcePath = "Singular.FilterUnique(" & SourcePath & ", " & DDA.ValueMember.AddSingleQuotes & ", " & PropertyInfo.Name & ")"
      End If


      '*** Old / Inactive Filtering ***
      If DDA.OldFilterProperties.Length > 0 Then

        'MOVE to binding handler
        SourcePath = "Singular.FilterOld(" & SourcePath & ", " & PropertyInfo.Name & "(), " & DDA.ValueMember.AddSingleQuotes & ", " & DDA.GroupChildListMember.AddSingleQuotes & ", " & GetFilterPropertiesString() & ")"
      End If


      '*** Custom Filtering ***
      If DDA.ThisFilterMember <> "" OrElse DDA.FilterConstant IsNot Nothing OrElse DDA.FilterMethodName <> "" Then
        If mBindingMode = LocationType.Client Then

          If DDA.FilterMethodName <> "" Then
            SourcePath = DDA.FilterMethodName & "(" & SourcePath & ", " & "$data" & ")"
          Else
            If DDA.DataSourceFilterMember <> "" Then
              If DDA.FilterConstant IsNot Nothing OrElse DDA.ThisFilterMember = "" Then
                SourcePath = "Singular.FilterList(" & SourcePath & ", '" & DDA.DataSourceFilterMember & "', " & System.Web.Helpers.Json.Encode(DDA.FilterConstant).Replace("""", "'") & ")"
              Else
                SourcePath = "Singular.FilterList(" & SourcePath & ", '" & DDA.DataSourceFilterMember & "', " & GetContext(DDA.ThisFilterMember) & "())"
              End If
            End If
          End If

        Else
          Dim pi As PropertyInfo = Singular.Reflection.GetProperty(mBindingObject.GetType, DDA.ThisFilterMember)
          Dim Val As Object = pi.GetValue(mBindingObject, Nothing)
          If Val Is Nothing Then
            SourcePath = "[]"
          Else
            SourcePath = "Singular.FilterList(" & SourcePath & ", '" & DDA.DataSourceFilterMember & "', " & Val.ToString.AddSingleQuotes & ")"
          End If
        End If

      End If
      Return SourcePath
    End Function

    Private Function GetFilterPropertiesString() As String
      Dim FilterProperties As String = ""
      For Each p As String In DDA.OldFilterProperties
        Singular.Strings.Delimit(FilterProperties, p.AddSingleQuotes)
      Next

      If DDA.GroupChildListMember <> "" Then
        '2 levels
        If DDA.OldFilterProperties.Length <> 2 Then
          Throw New Exception("OldFilterProperties must have 2 items if this is a grouped options list.")
        End If

      End If
      Return FilterProperties
    End Function

    Protected Overrides Sub WriteOtherAttributes()

      If ShowFindScreen Then
        Dim VMCriteriaPropertyName As String = DDA.ListType.Name & DDA.GetCriteriaClass.Name

        If CType(Page, IPageBase).LateResources.CanAddFindDialog(VMCriteriaPropertyName) Then
          Dim dlg As New SearchDialog(Of t)
          dlg.ListType = DDA.ListType
          dlg.CriteriaType = DDA.GetCriteriaClass
          dlg.VMCriteriaPropertyName = VMCriteriaPropertyName
          dlg.MultiSelect = IsMultiSelect
          AddControl(dlg)
          dlg.DialogCaption = "Select " & Singular.Reflection.GetDisplayName(PropertyInfo)

          dlg.Render()
        End If

      End If

    End Sub
  End Class

  ''' <summary>
  ''' Create a an input type=text element.
  ''' </summary>
  Public Class TextEditor(Of t)
    Inherits EditorBase(Of t)

    Private mTF As Singular.DataAnnotations.TextField
    'Private UpdateImmediate As Boolean
    Public MultiLine As Boolean
    Private Password As Boolean = False

    Public Sub New()

    End Sub

    Public Sub New(BindProperty As String, BindPath As String, TextOptions As Singular.DataAnnotations.TextField)
      mTF = TextOptions
      If mTF IsNot Nothing Then
        MultiLine = mTF.MultiLine
      End If

      mBindProperty = BindProperty
      mBindPath = BindPath
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If Singular.Web.Controls.AddBootstrapClasses Then AddClass("form-control")
    End Sub

    Protected Overrides Sub WriteStartTag()
      If MultiLine Then
        Writer.WriteBeginTag("textarea")
        If PropertyInfo IsNot Nothing AndAlso Not PropertyInfo.CanWrite Then
          MakeReadOnly()
        End If
      Else
        MyBase.WriteStartTag()
      End If
    End Sub

    Protected Overrides Sub WriteEndTag()
      If MultiLine Then
        Writer.WriteCloseTag(False)
        Writer.WriteEndTag("textarea")
      Else
        MyBase.WriteEndTag()
      End If
    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      If Not MultiLine Then

        If Password Then
          Writer.WriteAttribute("type", InputType.password.ToString)
        Else
          Writer.WriteAttribute("type", InputType.text.ToString)
        End If

      End If
    End Sub

    Protected Overrides Sub WriteOtherAttributes()
      If PropertyInfo IsNot Nothing Then
        Dim sla = Singular.Reflection.GetAttribute(Of StringLengthAttribute)(PropertyInfo)
        If sla IsNot Nothing AndAlso sla.MaximumLength > 0 Then
          Writer.WriteAttribute("maxlength", sla.MaximumLength)
        End If
      End If

      If MultiLine Then
        If mTF IsNot Nothing Then
          Writer.WriteAttribute("rows", mTF.NoOfLines)
        Else
          Writer.WriteAttribute("rows", 0)
        End If
        'Writer.WriteAttribute("cols", "20")
      End If

      If Password Then
        Writer.WriteAttribute("autocomplete", "off")
      End If
    End Sub

    Protected Overrides Sub AddBindingAttributes()
      AddValueBinding()

      If PropertyInfo IsNot Nothing Then
        mTF = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.TextField)(PropertyInfo)
        If mTF IsNot Nothing Then
          MultiLine = mTF.MultiLine
        End If

        If Singular.Reflection.GetAttribute(Of System.ComponentModel.PasswordPropertyTextAttribute)(PropertyInfo) IsNot Nothing Then
          Password = True
        End If

        If mTF IsNot Nothing AndAlso Not mTF.UpdateOnLeave AndAlso mBindingMode = LocationType.Client Then
          AddBinding(KnockoutBindingString.valueUpdate, "'afterkeydown'")
        End If
      End If

    End Sub
  End Class

  ''' <summary>
  ''' Creates an input type=text element, but with a class name of numeric editor.
  ''' The singular.js library will force numeric input.
  ''' </summary>
  Public Class NumericEditor(Of t)
    Inherits EditorBase(Of t)

    Private mNf As Singular.DataAnnotations.NumberField

    Public Sub New()

    End Sub

    Public Sub New(BindProperty As String, BindPath As String, NumberOptions As Singular.DataAnnotations.NumberField)
      mNf = NumberOptions

      mBindProperty = BindProperty
      mBindPath = BindPath
    End Sub


    Protected Friend Overrides Sub Setup()

      If PropertyInfo IsNot Nothing Then
        mNf = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(PropertyInfo)
      End If

      MyBase.Setup()
    End Sub

    Protected Overrides Sub WriteClassAttribute()
      If Singular.Web.Controls.AddBootstrapClasses Then AddClass("form-control")
      WriteClass(Singular.Web.ApplicationSettings.DisplaySettings.NumericEditorClassName)
    End Sub

    Protected Overrides Sub AddBindingAttributes()

      If mBindProperty Is Nothing Then
        AddBinding(KnockoutBindingString.NValue, Singular.Web.Controls.BindingHelpers.GetNumberBinding(GetForJS, mNf))
      Else
        AddBinding(KnockoutBindingString.NValue, Singular.Web.Controls.BindingHelpers.GetNumberBinding(GetContext(mBindPath, mBindProperty), mNf))
      End If

      If PropertyInfo IsNot Nothing Then
        Dim TF = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.TextField)(PropertyInfo)
        If TF IsNot Nothing AndAlso Not TF.UpdateOnLeave Then
          AddBinding(KnockoutBindingString.valueUpdate, "'afterkeydown'")
        End If
      End If

    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      Writer.WriteAttribute("type", InputType.text.ToString)

      If Singular.Web.Controls.ShowMobileNumericKeyboard Then
        'This is used instead of type=number because it does not cause issues with number formatting.
        Writer.WriteAttribute("inputmode", "numeric")
        Writer.WriteAttribute("step", "any")
      End If

      If PropertyInfo IsNot Nothing Then
        Writer.WriteAttribute("data-type", PropertyInfo.PropertyType.Name(0))
      End If

    End Sub
  End Class

  ''' <summary>
  ''' Creates an input type=text element, but with a class name of date picker.
  ''' The singular.js library will convert this to a jquery date picker.
  ''' </summary>
  Public Class DateEditor(Of t)
    Inherits EditorBase(Of t)

    Private mDa As Singular.DataAnnotations.DateField

    Public Sub New()

    End Sub

    Public Sub New(BindProperty As String, BindPath As String, DateOptions As Singular.DataAnnotations.DateField)
      mDa = DateOptions
      mBindProperty = BindProperty
      mBindPath = BindPath
    End Sub

    Protected Friend Overrides Sub Setup()

      If PropertyInfo IsNot Nothing Then
        mDa = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateField)(PropertyInfo)
      End If

      MyBase.Setup()

    End Sub

    Protected Overrides Sub WriteStartTag()
      If mDa IsNot Nothing AndAlso mDa.AlwaysShow Then
        Writer.WriteBeginTag("div")
      Else
        MyBase.WriteStartTag()
      End If
    End Sub

    Protected Overrides Sub WriteEndTag()
      If mDa IsNot Nothing AndAlso mDa.AlwaysShow Then
        Writer.WriteCloseTag(False)
        Writer.WriteEndTag("div")
      Else
        MyBase.WriteEndTag()
      End If
    End Sub

    Protected Overrides Sub AddBindingAttributes()

      If mBindProperty Is Nothing Then
        AddBinding(KnockoutBindingString.DateValue, Singular.Web.Controls.BindingHelpers.GetDateBinding(GetForJS, PathToContext, mDa))
      Else
        'Value suplied as string.
        AddBinding(KnockoutBindingString.DateValue, Singular.Web.Controls.BindingHelpers.GetDateBinding(mBindProperty, mBindPath, mDa))
      End If

    End Sub

    Protected Overrides Sub WriteClassAttribute()
      If Singular.Web.Controls.AddBootstrapClasses Then AddClass("form-control")
      If mDa Is Nothing OrElse Not mDa.AlwaysShow Then
        WriteClass(Singular.Web.ApplicationSettings.DisplaySettings.DatePickerClassName)
      End If

    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      If mDa Is Nothing OrElse Not mDa.AlwaysShow Then
        Writer.WriteAttribute("type", InputType.text.ToString)
      End If
    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
    End Sub

  End Class

  Public Class TimeEditor(Of t)
    Inherits EditorBase(Of t)

    Public Sub New()
      KeepInline = True
    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      Writer.WriteAttribute("type", InputType.text.ToString)
    End Sub

    Protected Overrides Sub AddBindingAttributes()

      AddBinding(KnockoutBindingString.DateValue, "{ Value: " & GetForJS() & ", Type: 'Time'}")
      AddBinding(Singular.Web.KnockoutBindingString.id_sfx, "'time'")

    End Sub

  End Class

  ''' <summary>
  ''' Creates an input type=checkbox element
  ''' </summary>
  Public Class CheckBoxEditor(Of t)
    Inherits EditorBase(Of t)

    Private mIsThreeState As Boolean = False

    Public Sub New()
      KeepInline = True
    End Sub

    Public Sub New(BindProperty As String, BindPath As String)
      mBindProperty = BindProperty
      mBindPath = BindPath
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If Singular.Web.Controls.AddBootstrapClasses Then AddClass("form-check-input")

    End Sub

    Protected Overrides Sub WriteStartTag()
      Me.Style.Width = ""
      Writer.WriteBeginTag("input")
      If PropertyInfo IsNot Nothing AndAlso Not PropertyInfo.CanWrite Then
        Writer.WriteAttribute("disabled", "disabled")
      End If
    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      Writer.WriteAttribute("type", InputType.checkbox.ToString)
    End Sub

    Protected Overrides Sub AddBindingAttributes()

      If PropertyInfo IsNot Nothing AndAlso PropertyInfo.PropertyType Is GetType(Boolean?) Then
        mIsThreeState = True
      End If

      If mBindingMode = LocationType.Client Then
        Dim BindName As String
        If mBindProperty Is Nothing Then
          BindName = GetForJS()
        Else
          BindName = GetContext(mBindPath, mBindProperty)
        End If
        If mIsThreeState Then
          AddBinding(KnockoutBindingString.SChecked, "{ Value: " & BindName & ", ThreeState: true}")
        Else
          AddBinding(KnockoutBindingString.SChecked, BindName)
        End If


      Else
        If GetValue("") Then
          Writer.WriteAttribute("checked", "checked")
        End If
      End If

    End Sub

  End Class

  Public Class RadioButtonEditor(Of ObjectType)
    Inherits EditorBase(Of ObjectType)

    Private mRBA As Singular.DataAnnotations.RadioButtonList

    Private mLabel As HTMLTag(Of ObjectType)
    Private mInput As HTMLTag(Of ObjectType)
    Private mText As HTMLSnippet

    Public ReadOnly Property Label As HTMLTag(Of ObjectType)
      Get
        Return mLabel
      End Get
    End Property

    Public ReadOnly Property Input As HTMLTag(Of ObjectType)
      Get
        Return mInput
      End Get
    End Property

    Public Property JSBinding As String = ""
    Public Property Horizontal As Boolean = False

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      mRBA = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.RadioButtonList)(PropertyInfo)

      'Create the Controls
      mLabel = Helpers.HTMLTag("label")
      With mLabel

        mInput = .Helpers.HTMLTag("input", True)
        With mInput
          .Attributes("type") = InputType.radio.ToString
          '.Attributes("name") = PropertyInfo.Name
          .AddBinding(KnockoutBindingString.attr, "{ name: '" & PropertyInfo.Name & "' + $data.Guid() }")
          .AddBinding(KnockoutBindingString.checked, GetForJS)

        End With

        mText = .Helpers.HTML("")

      End With

    End Sub

    Protected Friend Overrides Sub Render()

      If JSBinding <> "" Then
        mInput.AddBinding(KnockoutBindingString.checked, JSBinding)
      End If

      If mRBA.DataSourceType.IsEnum Then

        For Each item As Singular.Reflection.EnumItem In Singular.Reflection.GetEnumArray(mRBA.DataSourceType)
          AddItem(item.Value, item.Description)
        Next

      Else
        Dim ddi = Singular.DataAnnotations.DropDownWeb.GetValueDisplayMember(mRBA.DataSourceType, "")
        Dim ChildType = Singular.Reflection.GetLastGenericType(mRBA.DataSourceType)
        Dim Val = ChildType.GetProperty(ddi.ValueMember, BindingFlags.Public Or BindingFlags.Instance)
        Dim Displ = ChildType.GetProperty(ddi.DisplayMember, BindingFlags.Public Or BindingFlags.Instance)

        Dim List = Singular.CommonData.GetList("", mRBA.DataSourceType)

        For Each item In List
          AddItem(Val.GetValue(item, Nothing), Displ.GetValue(item, Nothing))
        Next

      End If

      mHasRendered = True
    End Sub

    Private Sub AddItem(Value As Object, Text As String)
      mInput.Attributes("value") = Value
      mText.Content = Text
      mLabel.Render()
      If Not (Horizontal OrElse mRBA.Horizontal) Then
        Writer.Write("<br />")
      End If
    End Sub

    Protected Overrides Sub WriteTypeAttribute()

    End Sub

    Protected Overrides Sub AddBindingAttributes()

    End Sub
  End Class

  Public Class FileEditor(Of t)
    Inherits EditorBase(Of t)

    Public Property AllowedExtensions As New List(Of String)

    ''' <summary>
    ''' Adds 1 or more file extensions that are allowed to be uploaded.
    ''' Will allow any extension if this is left blank.
    ''' Don't include the "." part of the extension. 
    ''' </summary>
    ''' <param name="Extensions"></param>
    ''' <remarks></remarks>
    Public Sub AddAllowedExtensions(ParamArray Extensions As String())
      AllowedExtensions.AddRange(Extensions)
    End Sub

    Public Property ImagesOnly As Boolean = False

    Public Property OnChangeJS As String

    Protected Overrides Sub WriteStartTag()
      'overridden so that it is never read only.
      Writer.WriteBeginTag("input")

      If AllowedExtensions.Count > 0 Then
        Dim ExtString As String = ""
        For Each ext As String In AllowedExtensions
          Singular.Strings.Delimit(ExtString, ext.Replace(".", ""), ",")
        Next
        Writer.WriteAttribute("data-extAllowed", ExtString)
      End If

      If ImagesOnly Then
        Writer.WriteAttribute("accept", "image/*")
      End If

      If Not String.IsNullOrEmpty(OnChangeJS) Then
        Writer.WriteAttribute("data-SIgnore", "true")
        Writer.WriteAttribute("onchange", OnChangeJS)
      End If

    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      Writer.WriteAttribute("type", InputType.file.ToString)
    End Sub

    Protected Overrides Sub AddBindingAttributes()

      If mBindingMode = LocationType.Client Then
        AddBinding(KnockoutBindingString.id, "'DocMan_' + Guid()")
        AddBinding(KnockoutBindingString.name, "'DocMan_' + Guid()")
      End If
    End Sub
  End Class

  ''' <summary>
  ''' Creates div with the slider ko binding.
  ''' </summary>
  Public Class SliderEditor(Of t)
    Inherits EditorBase(Of t)

    Private mDa As Singular.DataAnnotations.Slider

    Public Sub New()

    End Sub

    Public Sub New(BindProperty As String, BindPath As String, SliderOptions As Singular.DataAnnotations.Slider)
      mDa = SliderOptions
      mBindProperty = BindProperty
      mBindPath = BindPath
    End Sub

    Protected Friend Overrides Sub Setup()

      If PropertyInfo IsNot Nothing Then
        mDa = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.Slider)(PropertyInfo)
      End If

      MyBase.Setup()

    End Sub

    Protected Overrides Sub WriteStartTag()
      Writer.WriteBeginTag("div")
    End Sub

    Protected Overrides Sub WriteEndTag()
      Writer.WriteCloseTag(False)
      Writer.WriteEndTag("div")
    End Sub

    Protected Overrides Sub AddBindingAttributes()

      If mBindProperty Is Nothing Then
        AddBinding(KnockoutBindingString.Slider, GetForJS() & ", sliderOptions: {min: " & mDa.Minimum & ", max: " & mDa.Maximum & ", range: false, step: " & mDa.StepLength & " }")
      Else
        'Value suplied as string.
        AddBinding(KnockoutBindingString.Slider, mBindProperty & ", sliderOptions: {min: " & mDa.Minimum & ", max: " & mDa.Maximum & ", range: false, step: " & mDa.StepLength & " }")
      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
    End Sub

    Protected Overrides Sub WriteTypeAttribute()

    End Sub
  End Class

  Public Class DateAndTimeEditor(Of t)
    Inherits EditorBase(Of t)

    Private mDa As Singular.DataAnnotations.DateAndTimeField

    Public Sub New()
      ' KeepInline = True
    End Sub

    Protected Friend Overrides Sub Setup()
      If PropertyInfo IsNot Nothing Then
        mDa = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateAndTimeField)(PropertyInfo)
      End If
      MyBase.Setup()
    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      Writer.WriteAttribute("type", InputType.text.ToString)
    End Sub

    Protected Overrides Sub AddBindingAttributes()

      AddBinding(KnockoutBindingString.DateAndTimeEditor, "{ Value: " & GetForJS() & ", Type: 'DateAndTime', dateAndTimeOptions: { displayFormat: '" & mDa.FormatString & "', initialDate: '" & mDa.InitialDateFunction & "', allowInputToggle: '" & mDa.AllowInputToggle.ToString.ToLower & "', viewMode: '" & mDa.ViewMode.ToString.ToLower & "', showClear: '" & mDa.ShowClear.ToString.ToLower & "', calendarWeeks: '" & mDa.ShowCalendarWeeks.ToString.ToLower & "', keepOpen: '" & mDa.KeepOpen.ToString.ToLower & "', sideBySide: '" & mDa.SideBySide.ToString.ToLower & "', inline: '" & mDa.Inline.ToString.ToLower & "' }}")

    End Sub

  End Class

  Public Class Select2(Of t)
    Inherits EditorBase(Of t)

    Private mDa As Singular.DataAnnotations.Select2Field

    Public Sub New()
      ' KeepInline = True
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If PropertyInfo IsNot Nothing Then
        mDa = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.Select2Field)(PropertyInfo)
      End If

    End Sub

    Protected Overrides Sub WriteTypeAttribute()
      'Writer.WriteAttribute("type", InputType.text.ToString)
    End Sub

    Protected Overrides Sub AddBindingAttributes()
      mDa = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.Select2Field)(PropertyInfo)

      Dim fName As String = ""
      If mDa.FilterMethod <> "" Then
        fName = mDa.FilterMethod & "(" & mDa.ClientName.ToString & ", " & "$data" & ")"
      End If

      Dim ivName As String = ""
      If mDa.InitialValueMethod <> "" Then
        ivName = mDa.InitialValueMethod & "(" & mDa.ClientName.ToString & ", " & "$data" & ")"
      End If

      'Dim obuName As String = ""
      'If mDa.OnBindingUpdatedMethod <> "" Then
      '  obuName = mDa.OnBindingUpdatedMethod & "(" & mDa.ClientName.ToString & ", " & "$data" & ")"
      'End If

      AddBinding(KnockoutBindingString.select2, "{ select2Options: { " &
                                                                   " placeholder: '" & mDa.Placeholder.ToString.ToLower & "'," &
                                                                   " tags: '" & mDa.Tags.ToString.ToLower & "'," &
                                                                   " multiple: '" & mDa.Multiple.ToString.ToLower & "'," &
                                                                   " width: '" & mDa.Width.ToString.ToLower & "'," &
                                                                   " allowClear: '" & mDa.AllowClear.ToString.ToLower & "'," &
                                                                   " source: '" & mDa.ClientName.ToString & "'," &
                                                                   " valueMember: '" & mDa.ValueMember.ToString & "'," &
                                                                   " displayMember: '" & mDa.DisplayMember.ToString & "'," &
                                                                   " filterMethod: " & fName & "," &
                                                                   " initialValueMethod: " & ivName & "," &
                                                                   " onBindingUpdatedMethod:" & mDa.OnBindingUpdatedMethod &
                                                                 " }, " &
                                                  "Value: " & GetForJS() &
                                                "}")
      '"'," &

    End Sub

  End Class

End Namespace

