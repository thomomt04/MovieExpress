Imports System.Reflection
Imports System.Linq.Expressions
Imports System.Text
Imports Singular.Web.CustomControls
Imports System.Drawing

Namespace Controls

  Partial Public Class HelperControls

    Public Enum TagType
      Normal = 1
      IndentChildren = 2
      SelfClosing = 3
    End Enum

    Public Class ControlSettings

      Private Class SettingAttribute
        Inherits Attribute

        Public Property IgnoreValue As Object = Nothing

        Public Sub New(IgnoreValue As Object)
          Me.IgnoreValue = IgnoreValue
        End Sub

        Public Sub New()

        End Sub

      End Class

      <SettingAttribute()> Public Property Width As String
      <SettingAttribute()> Public Property Height As String
      <SettingAttribute()> Public Property Colour As Color?
      <SettingAttribute(UI.WebControls.HorizontalAlign.NotSet)> Public Property TextAlign As System.Web.UI.WebControls.HorizontalAlign = UI.WebControls.HorizontalAlign.NotSet
      <SettingAttribute(UI.WebControls.HorizontalAlign.NotSet)> Public Property VerticalAlign As System.Web.UI.WebControls.VerticalAlign = UI.WebControls.VerticalAlign.NotSet
      <SettingAttribute()> Public Property CSSClass As String

      Friend Property TargetType As ControlSettingType = ControlSettingType.All
      Friend Property TargetTypeExact As Type

      Public Sub PopulateSettings(Settings As ControlSettings)

        For Each pi As PropertyInfo In Me.GetType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)

          Dim sa As SettingAttribute = Singular.Reflection.GetAttribute(Of SettingAttribute)(pi)
          If sa IsNot Nothing Then

            Dim OwnValue As Object = pi.GetValue(Me, Nothing)
            If OwnValue IsNot Nothing AndAlso OwnValue <> sa.IgnoreValue Then
              pi.SetValue(Settings, OwnValue, Nothing)
            End If

          End If

        Next

      End Sub

    End Class

    Public MustInherit Class HelperBase
      Inherits CustomWebControl

      ''' <summary>
      ''' True if this control doesnt have custom rendering, and is only renders through child controls.
      ''' Setting this to true allows you to omit the render method.
      ''' </summary>
      Protected Property OnlyChildControls As Boolean = False

      Public Overrides Property ID As String
        Get
          Return Attributes("id")
        End Get
        Set(value As String)
          Attributes("id") = value
        End Set
      End Property

      Friend Property HelperAccessorsParent As HelperAccessors

      Protected mSettings As New ControlSettings
      Public Overridable ReadOnly Property Settings As ControlSettings
        Get
          Return mSettings
        End Get
      End Property

      Friend Overridable ReadOnly Property ControlSettingType As ControlSettingType
        Get
          Return ControlSettingType.All
        End Get
      End Property

      Protected ReadOnly Property Model As Object
        Get
          If Page IsNot Nothing Then
            Return CType(Page, PageBase).ModelNonGeneric
          Else
            Return Helpers.IPage.ModelNonGeneric
          End If
        End Get
      End Property

      Protected Function AddControl(Control As HelperBase) As HelperBase
        Control.HelperAccessorsParent = HelperAccessorsParent
        Me.Controls.Add(Control)
        Control.Setup()
        Return Control
      End Function

      Protected Sub WriteStyles(Optional StyleClass As CSSStyle = Nothing)
        If StyleClass Is Nothing Then
          StyleClass = Style
        End If
        'Write these settings as styles.
        If Settings.Width IsNot Nothing AndAlso StyleClass("width") Is Nothing Then
          StyleClass("width") = Settings.Width
        End If
        If Settings.Height IsNot Nothing AndAlso StyleClass("height") Is Nothing Then
          StyleClass("height") = Settings.Height
        End If
        If Settings.Colour IsNot Nothing Then
          StyleClass("color") = System.Drawing.ColorTranslator.ToHtml(Settings.Colour.Value)
        End If
        If Settings.TextAlign <> UI.WebControls.HorizontalAlign.NotSet Then
          StyleClass("text-align") = Settings.TextAlign.ToString.ToLower
        End If
        If Settings.VerticalAlign <> UI.WebControls.VerticalAlign.NotSet Then
          StyleClass("vertical-align") = Settings.VerticalAlign.ToString.ToLower
        End If

        If StyleClass.Count > 0 Then
          Writer.Write(" style=""")
          'Write each style thats been set in the Styles Collection.
          For Each key As String In StyleClass.Keys
            If StyleClass(key) <> "" Then
              Writer.Write(key & ": " & StyleClass(key) & ";")
            End If
          Next

          Writer.Write("""")
        End If
      End Sub

      Protected Sub WriteClass(Optional SingleClassName As String = "")

        If SingleClassName <> "" Then
          AddClass(SingleClassName)
        End If
        If Settings.CSSClass IsNot Nothing AndAlso Not mCSSClasses.ContainsKey(Settings.CSSClass) Then
          AddClass(Settings.CSSClass)
        End If

        Dim Classes As String = ""
        For Each cssclass As String In mCSSClasses.Keys
          If mCSSClasses(cssclass) Then
            Classes &= cssclass & " "
          End If
        Next

        If Classes <> "" Then
          Writer.WriteAttribute("class", Classes)
        End If
      End Sub

      Private mCSSClasses As New Hashtable

      Public Function GetCSSClasses() As Hashtable
        Return mCSSClasses
      End Function

      Public Function AddClass(ClassName As String) As HelperBase
        mCSSClasses(ClassName) = True
        Return Me
      End Function

      Public Sub RemoveClass(ClassName As String)
        mCSSClasses(ClassName) = False
      End Sub

      ''' <summary>
      ''' Sets the animation type to use when rendering this control inside a template.
      ''' e.g. in an if statement, you want the inner controls to fade in. Set this property on the control directly inside the if.
      ''' </summary>
      Public WriteOnly Property TemplateRenderAnimation As VisibleFadeType
        Set(value As VisibleFadeType)
          Attributes("data-tmplAnimate") = value
        End Set
      End Property

      Private mControlIsSetup As Boolean = False
      Protected Friend Overridable Sub Setup()
        mControlIsSetup = True
        If mHelpers Is Nothing Then
          CreateHelpers()
        End If
        If HelperAccessorsParent IsNot Nothing Then
          HelperAccessorsParent.PopulateSettings(Me)
        End If
      End Sub

      Protected Friend Sub SetupInternal(ParentHelperAccessors As Singular.Web.Controls.HelperControls.HelperAccessors)
        HelperAccessorsParent = ParentHelperAccessors
        CreateHelpers()
        Setup()
      End Sub

      Protected Overridable Sub CreateHelpers()
        mHelpers = New Singular.Web.Controls.HelperControls.HelperAccessors(Page, Me)
      End Sub

      Public ReadOnly Property IsSetup As Boolean
        Get
          Return mControlIsSetup
        End Get
      End Property

      Protected mHelpers As Singular.Web.Controls.HelperControls.HelperAccessors
      Public ReadOnly Property Helpers As Singular.Web.Controls.HelperControls.HelperAccessors
        Get
          Return mHelpers
        End Get
      End Property

      Protected Overridable Sub WriteFullStartTag(TagName As String, TagType As TagType, Optional AddOtherAttributes As Action = Nothing)

        Writer.WriteBeginTag(TagName)

        WriteControlAttributes()
        WriteClass()
        WriteStyles()

        If AddOtherAttributes IsNot Nothing Then
          AddOtherAttributes.Invoke()
        End If

        If TagType = TagType.SelfClosing Then
          Writer.WriteFullCloseTag()
        Else
          Writer.WriteCloseTag(TagType = HelperControls.TagType.IndentChildren)
        End If

      End Sub

      Protected Friend Overridable Sub TransferPropertiesTo(ToControl As HelperBase)

        'Copy Attributes.
        For Each att As String In Me.Attributes.Keys
          ToControl.Attributes(att) = Attributes(att)
        Next

        'Copy Class
        For Each c As String In mCSSClasses.Keys
          If mCSSClasses(c) Then
            ToControl.AddClass(c)
          End If
        Next

        'Copy Styles
        For Each key As String In Style.Keys
          ToControl.Style(key) = Style(key)
        Next

      End Sub

      Public Sub ASyncRender(UpdateHelper As HTMLSnippet)
        UpdateHelper.AddControl(Me)
      End Sub

      Public Overrides Sub Dispose()
        If Not HasRendered Then
          'If System.Web.UI.ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
          '  Throw New Exception("Helper controls in an Update Panel must call ASyncRender")
          'Else
          System.Web.HttpContext.Current.Response.Write(GetHTMLString)
          'End If


          MyBase.Dispose()
        End If
      End Sub

      Protected Friend Overrides Sub Render()
        If Not mControlIsSetup Then
          Setup()
        End If
        MyBase.Render()

        If OnlyChildControls Then
          RenderChildren()
        End If
      End Sub

    End Class

    Public MustInherit Class HelperBase(Of ObjectType)
      Inherits HelperBase
      Implements IBindingControl

      Protected mRequiresProperty As Boolean = True 'Requires that the linq expression is a property, not a full expression.

      Private mPropertyInfo As PropertyInfo
      Private mPathToContext As String = ""
      Private mLinqExpressionJS As String = ""

      Protected mLinqExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
      Protected mBindingMode As LocationType = LocationType.Client
      Protected mBindingObject As Object

      Public Shadows ReadOnly Property Helpers As Singular.Web.Controls.HelperControls.HelperAccessors(Of ObjectType)
        Get
          Return mHelpers
        End Get
      End Property

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        If mKnockoutBindings Is Nothing Then
          If Parent IsNot Nothing AndAlso GetType(IBindingControl).IsAssignableFrom(Parent.GetType) Then
            mKnockoutBindings = New KnockoutBindingManager(Of ObjectType)(Me, CType(Parent, IBindingControl).BindingManager)
          Else
            mKnockoutBindings = New KnockoutBindingManager(Of ObjectType)(Me, Nothing)
          End If
        End If

      End Sub

      Protected Overrides Sub CreateHelpers()
        mHelpers = New Singular.Web.Controls.HelperControls.HelperAccessors(Of ObjectType)(Page, Me)
      End Sub

      Public ReadOnly Property PropertyInfo As PropertyInfo Implements IBindingControl.PropertyInfo
        Get
          EnsurePropertyInfo()
          Return mPropertyInfo
        End Get
      End Property

      Public Function GetForJS() As String
        If mLinqExpression Is Nothing Then
          If mPropertyInfo IsNot Nothing Then
            Return mPropertyInfo.Name
          Else
            Return ""
          End If
        Else
          EnsurePropertyInfo()
          Return mLinqExpressionJS
        End If
      End Function

      Protected Function PathToContext() As String
        EnsurePropertyInfo()
        Return mPathToContext
      End Function

      Protected Friend Function GetContext(PropertyName As String)
        EnsurePropertyInfo()
        Return GetContext(mPathToContext, PropertyName)
      End Function

      Protected Friend Function GetContext(PathToContext As String, PropertyName As String)
        If PathToContext = "" Then
          Return PropertyName
        Else
          Return PathToContext & "." & PropertyName
        End If
      End Function

      Public ReadOnly Property Self As HelperBase(Of ObjectType)
        Get
          Return Me
        End Get
      End Property

      Public Sub [For](le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
        mLinqExpression = le
      End Sub

      Public Sub [For](pi As PropertyInfo)
        mPropertyInfo = pi
      End Sub

      Public Sub [For](pi As PropertyInfo, Obj As Object)
        mPropertyInfo = pi
        mBindingObject = Obj
        mBindingMode = LocationType.Server
        Setup()
      End Sub

      Private Sub EnsurePropertyInfo()

        If mPropertyInfo Is Nothing AndAlso mLinqExpression IsNot Nothing Then

          Dim Output = Singular.Linq.JavascriptExpressionParser(Of ObjectType).GetCachedOutput(mLinqExpression, Linq.OutputModeType.KnockoutBinding, True)

          mLinqExpressionJS = Output.JavascriptString
          mPropertyInfo = Output.MemberInfo
          mPathToContext = Output.DataPath


          If mPropertyInfo Is Nothing AndAlso mRequiresProperty Then
            Throw New Exception("Can't add helper control, unsupported linq expression: " & mLinqExpressionJS)
          End If
        End If

      End Sub

      Protected Friend Overrides Sub TransferPropertiesTo(ToControl As HelperBase)
        MyBase.TransferPropertiesTo(ToControl)

        If TypeOf ToControl Is IBindingControl AndAlso mKnockoutBindings IsNot Nothing Then
          For Each bindingtype As Singular.Web.KnockoutBindingString In Bindings.Keys
            CType(ToControl, IBindingControl).AddBinding(bindingtype, Bindings(bindingtype))
          Next
        End If

      End Sub

#Region " Binding "

      Private mKnockoutBindings As KnockoutBindingManager(Of ObjectType)

      Public ReadOnly Property Bindings As KnockoutBindingManager(Of ObjectType)
        Get
          If mKnockoutBindings Is Nothing Then
            'This should not ever get hit if the control is setup properly. Setup should be called before any bindings are added, as this will create the binding manager with the proper parent etc.
            If Debugger.IsAttached Then
              Throw New Exception("Call Marlborough to fix this.")
            Else
              mKnockoutBindings = New KnockoutBindingManager(Of ObjectType)(Me, Nothing)
            End If

          End If
          Return mKnockoutBindings
        End Get
      End Property

      Protected ReadOnly Property BindingManager As IBindingManager Implements IBindingControl.BindingManager
        Get
          Return Bindings
        End Get
      End Property

      Protected Friend Overridable Function IsBindingAllowed(ByVal BindingType As KnockoutBindingString) As Boolean
        Return True
      End Function

      Public Overloads Sub AddBinding(ByVal BindingType As KnockoutBindingString, ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
        Bindings.Add(BindingType, le)
      End Sub

      Public Overloads Sub AddBinding(ByVal BindingType As KnockoutBindingString, ByVal Binding As String) Implements IBindingControl.AddBinding
        Bindings.Add(BindingType, Binding)
      End Sub

      Public WriteOnly Property Enable As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
        Set(value As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
          Bindings.Add(KnockoutBindingString.enable, value)
        End Set
      End Property

      Public WriteOnly Property EnableJS As String
        Set(value As String)
          Bindings.Add(KnockoutBindingString.enable, value)
        End Set
      End Property

      Public WriteOnly Property IsVisible As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
        Set(value As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
          Bindings.Add(KnockoutBindingString.visible, value)
        End Set
      End Property

      Public WriteOnly Property Text As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))
        Set(value As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
          Bindings.Add(KnockoutBindingString.text, value)
        End Set
      End Property

#End Region

      Protected Function GetValue(FormatString As String) As Object
        If mBindingObject Is Nothing OrElse mPropertyInfo Is Nothing Then
          Return Nothing
        Else
          Dim Val As Object = mPropertyInfo.GetValue(mBindingObject, Nothing)
          If FormatString = "" Then
            Return Val
          Else
            Return Format(Val, FormatString)
          End If
        End If
      End Function

      Protected Function GetValueString(FormatString As String) As String
        Dim val As Object = GetValue(FormatString)
        If val Is Nothing OrElse val Is DBNull.Value Then
          Return ""
        Else
          Return val.ToString
        End If
      End Function

      Protected Friend Function GetParentWithContext() As IBindingControl Implements IBindingControl.GetParentWithContext

        If Me.Parent IsNot Nothing AndAlso TypeOf Me.Parent Is IBindingControl Then
          If CType(Me.Parent, IBindingControl).BindingManager.HasBindingContext Then
            Return Me.Parent
          Else
            Return CType(Me.Parent, IBindingControl).GetParentWithContext
          End If
        Else
          Return Nothing
        End If

      End Function

      Protected Overrides Sub WriteFullStartTag(TagName As String, TagType As TagType, Optional AddOtherAttributes As Action = Nothing)

        WriteFullStartTag(TagName, TagType, mKnockoutBindings, AddOtherAttributes)

      End Sub

      Protected Overloads Sub WriteFullStartTag(TagName As String, TagType As TagType, BindingManager As KnockoutBindingManager(Of ObjectType), Optional AddOtherAttributes As Action = Nothing)

        MyBase.WriteFullStartTag(TagName, TagType, Sub()

                                                     If BindingManager IsNot Nothing Then
                                                       BindingManager.WriteKnockoutBindings()
                                                     End If

                                                     If AddOtherAttributes IsNot Nothing Then
                                                       AddOtherAttributes.Invoke()
                                                     End If

                                                   End Sub)

      End Sub

      Public Shared ReadOnly Property ViewModel As ObjectType
        Get
          Throw New Exception("Only used for Binding, call 'Model' on an instance of helper instead.")
        End Get
      End Property

      ''' <summary>
      ''' Fake c# with statement.
      ''' </summary>
      Public Function [With](act As Action(Of HelperBase(Of ObjectType))) As HelperBase(Of ObjectType)
        act(Me)
        Return Me
      End Function

      ''' <summary>
      ''' Fake c# with statement.
      ''' </summary>
      Public Overridable Function WithH(act As Action(Of HelperAccessors(Of ObjectType))) As HelperBase(Of ObjectType)
        act(Me.Helpers)
        Return Me
      End Function

    End Class

    ''' <summary>
    ''' Helperbase for controls that contain child controls which need a sub binding context.
    ''' </summary>
    ''' <typeparam name="ObjectType">The object type that this control binds to.</typeparam>
    ''' <typeparam name="ChildControlObjectType">The object type that the child controls bind to.</typeparam>
    Public MustInherit Class HelperBase(Of ObjectType, ChildControlObjectType)
      Inherits HelperBase(Of ObjectType)

      Private mOverrideChildType As Type = Nothing
      ''' <summary>
      ''' Allows the proper child type to be specified in cases where you have to pass object as the generic type.
      ''' </summary>
      Public Property OverrideChildType As Type
        Get
          If mOverrideChildType IsNot Nothing Then
            Return mOverrideChildType
          Else
            Return GetType(ChildControlObjectType)
          End If
        End Get
        Set(value As Type)
          mOverrideChildType = value
        End Set
      End Property

      'Private Shadows mHelpers As Singular.Web.Controls.HelperControls.HelperAccessors(Of ChildControlObjectType)
      Public Shadows ReadOnly Property Helpers As Singular.Web.Controls.HelperControls.HelperAccessors(Of ChildControlObjectType)
        Get
          Return mHelpers
        End Get
      End Property

      Protected Overrides Sub CreateHelpers()
        mHelpers = New Singular.Web.Controls.HelperControls.HelperAccessors(Of ChildControlObjectType)(Page, Me)
      End Sub

      ''' <summary>
      ''' Fake c# with statement.
      ''' </summary>
      Public Overloads Function WithH(act As Action(Of HelperAccessors(Of ChildControlObjectType))) As HelperBase(Of ObjectType, ChildControlObjectType)
        act(Me.Helpers)
        Return Me
      End Function

    End Class

  End Class

End Namespace

