Imports System.Reflection
Imports System.Linq.Expressions
Imports System.Text
Imports Singular.Web.CustomControls
Imports System.Drawing
Imports Singular.Web.Controls.HelperControls

Namespace Controls

#Region " Interfaces "

  Public Interface IBindingControl

    Function GetParentWithContext() As IBindingControl
    Sub AddBinding(BindingType As KnockoutBindingString, Binding As String)
    ReadOnly Property PropertyInfo As PropertyInfo
    ReadOnly Property BindingManager As IBindingManager

  End Interface

  Public Interface IBindingManager
    Property HasBindingContext As Boolean
    Property JSonBindings As Boolean
  End Interface

#End Region

  Public Class KnockoutBindingManager(Of ObjectType)
    Inherits Hashtable
    Implements IBindingManager

#Region " Properties "

    Private mCustomBindings As New Hashtable

    Private Property HasBindingContext As Boolean Implements IBindingManager.HasBindingContext

    Private mParent As HelperBase(Of ObjectType)
    Public Sub New(Parent As HelperBase(Of ObjectType), ParentBindingManager As IBindingManager)
      mParent = Parent
      If ParentBindingManager IsNot Nothing Then
        JSonBindings = ParentBindingManager.JSonBindings
      End If
    End Sub

    Public Property SortBindings As Boolean = False
    ''' <summary>
    ''' if true, brackets wont be added after property names. This is for when you are binding to a plain JSON object, not a KO object.
    ''' </summary>
    Public Property JSonBindings As Boolean = False Implements IBindingManager.JSonBindings

#End Region

#Region " Add Bindings "

    Public Sub AddVisibilityBinding(VisibleCondition As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                    FadeInType As VisibleFadeType, FadeOutType As VisibleFadeType, Optional HandlerFunction As String = "")
      AddVisibilityBinding(CreateBindingExpression(KnockoutBindingString.visibleA, VisibleCondition), FadeInType, FadeOutType, HandlerFunction)
    End Sub

    Public Sub AddVisibilityBinding(VisibleCondition As String, FadeInType As VisibleFadeType, FadeOutType As VisibleFadeType, Optional HandlerFunction As String = "")
      Add(KnockoutBindingString.visibleA, "{ In: " & FadeInType & ", Out: " & FadeOutType & ", Condition: " & VisibleCondition &
          If(HandlerFunction <> "", ", Handler: " & HandlerFunction, "") & " }")
    End Sub

    Public Sub AddDialogBinding(VisibleCondition As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                Title As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), Optional BeforeCloseFunctionName As String = "")
      AddDialogBinding(CreateBindingExpression(KnockoutBindingString.visible, VisibleCondition),
                       CreateBindingExpression(KnockoutBindingString.text, Title),
                       BeforeCloseFunctionName)
    End Sub

    Public Sub AddDialogBinding(VisibleConditionJS As String, Title As String, BeforeCloseFunctionName As String,
                                Optional AllowResize As Boolean = True, Optional WidthBinding As String = "", Optional OnOpenFunctionName As String = "")

      Add(KnockoutBindingString.dialog, BindingHelpers.GetDialogBinding(VisibleConditionJS, Title, BeforeCloseFunctionName, AllowResize, WidthBinding, OnOpenFunctionName))

    End Sub


    ''' <summary>
    ''' Adds a binding that will add a new object to the specified list when the control is clicked.
    ''' </summary>
    Public Sub AddAddBinding(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
      AddAddBinding(Singular.Reflection.GetMember(Of ObjectType)(ListProperty))
    End Sub

    Public Sub AddAddBinding(ListProperty As System.Reflection.PropertyInfo, Optional Options As String = "{}")
      Add(KnockoutBindingString.click, "Singular.AddAndFocus($data." & ListProperty.Name & "," & Options & ")")
    End Sub

    Public Sub AddAddBinding(ListPropertyName As String, Optional Options As String = "{}")
      Add(KnockoutBindingString.click, "Singular.AddAndFocus($data." & ListPropertyName & "," & Options & ")")
    End Sub

    Public Sub AddClearBinding(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
      AddClearBinding(Singular.Reflection.GetMember(Of ObjectType)(ListProperty))
    End Sub

    Public Sub AddClearBinding(ListProperty As System.Reflection.PropertyInfo)
      Add(KnockoutBindingString.click, ListProperty.Name & ".Clear()")
    End Sub

    Public Sub AddCSSClassBinding(ConditionProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), CSSClassIfTrue As String)
      AddCSSClassBinding(ConditionProperty, CSSClassIfTrue, "")
    End Sub

    Public Sub AddCSSClassBinding(ConditionProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), CSSClassIfTrue As String, CSSClassIfFalse As String)
      Add(KnockoutBindingString.css, "ko.utils.unwrapObservable(" & CreateBindingExpression(KnockoutBindingString.css, ConditionProperty) & ") ? '" & CSSClassIfTrue & "': '" & CSSClassIfFalse & "'")
    End Sub

    Public Sub AddBackgroundImage(ImageIDProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))

      Add(KnockoutBindingString.style, "{ 'background-image': Singular.GetImage(" & Singular.Reflection.GetMember(Of ObjectType)(ImageIDProperty).Name & ", true) }")

    End Sub

    Public Sub AddNumericBinding(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))

      Dim pi = Singular.Reflection.GetMember(ListProperty)
      Dim nf = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(pi)
      Add(KnockoutBindingString.NValue, BindingHelpers.GetNumberBinding(CreateBindingExpression(KnockoutBindingString.value, ListProperty), nf))

    End Sub

    Public Sub AddNumericBinding(Value As String, FormatString As String)

      Add(KnockoutBindingString.NValue, BindingHelpers.GetNumberBinding(Value, Nothing, FormatString))

    End Sub

    Public Sub AddDateBinding(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))

      Dim pi = Singular.Reflection.GetMember(ListProperty)
      Dim df = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateField)(pi)
      Dim expr = Singular.Linq.JavascriptExpressionParser(Of ObjectType).GetCachedOutput(ListProperty,
                                                                                        Linq.OutputModeType.KnockoutBinding, True,
                                                                                        Linq.EqualsActionType.Compare)
      Add(KnockoutBindingString.NValue, BindingHelpers.GetDateBinding(expr.JavascriptString, expr.DataPath, df))

    End Sub

    ''' <summary>
    ''' Adds a drag binding which allows the element to be dragged to another element.
    ''' </summary>
    ''' <param name="Scope">The drop target must have the same scope as this</param>
    ''' <param name="DragStartText">The text to display when this element is not over the drop target.</param>
    Public Sub AddDragBinding(Scope As String, DragStartText As String, Optional OnDragStartFunctionName As String = "")
      Add(Singular.Web.KnockoutBindingString.Drag, "{ Scope: '" & Scope & "', DragText: '" & DragStartText & "'" &
          If(OnDragStartFunctionName = "", "", ", OnStart: " & OnDragStartFunctionName) & " }")
    End Sub

    ''' <summary>
    ''' Adds a drop binding which allows an element with a drag binding to be dropped.
    ''' </summary>
    ''' <param name="Scope">The drag target must share the same scope as this</param>
    ''' <param name="OnDropFunctionName">function that must be called when a valid drop occurs</param>
    ''' <param name="OnHoverFunctionName">function that must be called when a drop element hovers over this. function(ev, ui, from, to)</param>
    Public Sub AddDropBinding(Scope As String, OnDropFunctionName As String, Optional OnHoverFunctionName As String = "")
      Add(Singular.Web.KnockoutBindingString.Drop, "{ Scope: '" & Scope & "', OnDrop: " & OnDropFunctionName &
          If(OnHoverFunctionName = "", "", ", OnHover: " & OnHoverFunctionName) & " }")
    End Sub

    ''' <summary>
    ''' Adds a for each binding, to render any child controls for each item in the supplied list.
    ''' </summary>
    ''' <param name="AfterRenderFunctionName">Allows custom UI code to run after an item is rendered. Function can take arguments: (elements, object)</param>
    ''' <remarks></remarks>
    Public Sub AddForEachBinding(ListProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), Optional AfterRenderFunctionName As String = "", Optional TemplateID As String = "")

      AddForEachBinding(CreateBindingExpression(KnockoutBindingString.foreach, ListProperty), AfterRenderFunctionName, TemplateID)

    End Sub

    ''' <summary>
    ''' Adds a for each binding, to render any child controls for each item in the supplied list.
    ''' </summary>
    ''' <param name="AfterRenderFunctionName">Allows custom UI code to run after an item is rendered. Function can take arguments: (elements, object)</param>
    ''' <remarks></remarks>
    Public Sub AddForEachBinding(List As String, Optional AfterRenderFunctionName As String = "", Optional TemplateID As String = "")

      Dim AfterRender As String = ""
      Dim Binding As String = "{ 'foreach': " & List & ", afterAdd: Singular.AfterTemplateRender"

      If AfterRenderFunctionName <> "" Then
        Binding &= ", afterRender: " & AfterRenderFunctionName
      End If

      If TemplateID <> "" Then
        Binding &= ", name: " & TemplateID
      End If

      Binding &= " }"

      Me(KnockoutBindingString.template) = Binding

    End Sub

    Public Sub AddIfBinding(Condition As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), AfterRenderFunctionName As String)
      AddIfBinding(CreateBindingExpression(KnockoutBindingString.if, Condition), AfterRenderFunctionName)
    End Sub

    Public Sub AddIfBinding(Condition As String, AfterRenderFunctionName As String)
      Me(KnockoutBindingString.template) = "{ 'if': " & Condition & ", afterRender: " & AfterRenderFunctionName & " }"
    End Sub

    Public Sub AddWithBinding(Condition As String, AfterRenderFunctionName As String)
      Me(KnockoutBindingString.template) = "{ 'if': " & Condition & ", data: " & Condition & ", afterRender: " & AfterRenderFunctionName & " }"
    End Sub

    Public Overloads Sub Add(ByVal BindingType As KnockoutBindingString, ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))

      Add(BindingType, CreateBindingExpression(BindingType, le))

    End Sub

    Public Sub AddCustom(CustomBinding As String, ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
      mCustomBindings(CustomBinding) = CreateBindingExpression(KnockoutBindingString.text, le)
    End Sub

    Public Sub AddCustom(CustomBinding As String, Binding As String)
      mCustomBindings(CustomBinding) = Binding
    End Sub

    'Public Overloads Sub Add(ByVal BindingType As KnockoutBindingString, jse As Singular.Linq.JavascriptExpressionParser(Of ObjectType))
    '  Try
    '    If mParent.IsBindingAllowed(BindingType) Then

    '      Add(BindingType, jse.ToString)

    '    End If
    '  Catch ex As Singular.Linq.JavascriptExpressionParser(Of ObjectType).InvalidPropertyException
    '    Throw New Exception("Cannot add " & BindingType.ToString & " binding because property " & ex.PropertyName & " is non browsable.")
    '  End Try

    'End Sub

    Public Overloads Sub Add(ByVal BindingType As KnockoutBindingString, ByVal Binding As String)

      If mParent.IsBindingAllowed(BindingType) Then

        'Special cases.
        Select Case BindingType
          Case KnockoutBindingString.for
            Me(BindingType) = "'" & Binding & "_' + " & mParent.GetContext("Guid()")
          Case KnockoutBindingString.click
            Me(BindingType) = "function(_obj, e) {" & Binding & "}"

            'Change the bindings for with and if, so that after they are rendered, the controls inside them are converted to jquery ui controls.
          Case KnockoutBindingString.with
            Me(KnockoutBindingString.template) = BindingHelpers.GetWithTemplateBinding(Binding)
          Case KnockoutBindingString.if
            Me(KnockoutBindingString.template) = BindingHelpers.GetIfTemplateBinding(Binding)
          Case KnockoutBindingString.foreach
            mParent.Bindings.AddForEachBinding(Binding)

          Case Else
            Me(BindingType) = Binding
        End Select

        If BindingType = KnockoutBindingString.with Or BindingType = KnockoutBindingString.foreach Then
          HasBindingContext = True
        End If

      End If

    End Sub

#End Region

#Region " Methods "

    Friend Function CreateBindingExpression(ByVal BindingType As KnockoutBindingString,
                                                        ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As String
      Return CreateBindingExpression(BindingType, le, JSonBindings)
    End Function

    Public Shared Function CreateBindingExpression(ByVal BindingType As KnockoutBindingString,
                                                        ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                                        JSonBindings As Boolean) As String

      Try
        Return Singular.Linq.JavascriptExpressionParser(Of ObjectType).GetCachedOutput(
          le,
          Linq.OutputModeType.KnockoutBinding,
          True,
          If(BindingType = KnockoutBindingString.click, Linq.EqualsActionType.SetValue, Linq.EqualsActionType.Compare),
          If(JSonBindings, Linq.PropertyAccessType.PlainJSon, Linq.PropertyAccessType.AutoDetect)) _
        .JavascriptString

      Catch ex As Singular.Linq.JavascriptExpressionParser(Of ObjectType).InvalidPropertyException
        Throw New Exception("Cannot add " & BindingType.ToString & " binding because property " & ex.PropertyName & " is non browsable.")
      End Try

    End Function

    Public Function GetBinding(ByVal BindingType As KnockoutBindingString) As String
      If BindingType = KnockoutBindingString.with Or BindingType = KnockoutBindingString.if Or BindingType = KnockoutBindingString.foreach Then
        Return Me(KnockoutBindingString.template)
      Else
        Return Me(BindingType)
      End If
    End Function

    Protected Friend Sub WriteKnockoutBindings()

      If Count > 0 OrElse mCustomBindings.Count > 0 Then
        Dim Bindings As String = ""
        Dim AttrBindings As String = ""

        Dim Keys As ICollection = Me.Keys

        If SortBindings AndAlso Count > 1 Then
          Dim SortedList As New SortedDictionary(Of Integer, String)
          For Each bindingtype As Singular.Web.KnockoutBindingString In Keys
            SortedList.Add(CInt(bindingtype), Me(bindingtype))
          Next
          Keys = SortedList.Keys
        End If

        For Each bindingtype As Singular.Web.KnockoutBindingString In Keys
          If Me(bindingtype) <> "" Then
            'Check if its an attribute binding.
            If CInt(bindingtype) >= 100 Then
              Singular.Strings.Delimit(AttrBindings, "'" & bindingtype.ToString & "': " & Me(bindingtype))
            Else
              Singular.Strings.Delimit(Bindings, bindingtype.ToString & ": " & Me(bindingtype))
            End If
          End If
        Next

        'Add attribute bindings to normal bindings.
        If AttrBindings <> "" Then
          AttrBindings = "attr: {" & AttrBindings & "}"
          If Bindings <> "" Then
            AttrBindings &= ", " & Bindings
          End If
          Bindings = AttrBindings
        End If


        If mCustomBindings.Count > 0 Then
          For Each cb As String In mCustomBindings.Keys
            If Bindings.Length <> 0 Then
              Bindings &= ", "
            End If
            Bindings &= cb & ": " & mCustomBindings(cb)
          Next
        End If

        mParent.Writer.WriteAttribute("data-bind", Bindings)
      End If

    End Sub

#End Region

  End Class

#Region " Binding Helpers "

  Public Class BindingHelpers

    Friend Shared Function GetNumberBinding(Value As String, nf As Singular.DataAnnotations.NumberField, Optional FormatString As String = "") As String

      Dim BindString = "{Value: " & Value
      If nf IsNot Nothing AndAlso nf.HasFormat Then
        BindString &= ", Format: " & nf.GetFormatString(True)

      ElseIf FormatString <> "" Then
        BindString &= ", Format: '" & FormatString & "'"
      End If

      If nf IsNot Nothing AndAlso Not String.IsNullOrEmpty(nf.CurrencySymbolProperty) Then
        Dim CurrencySymbol = nf.CurrencySymbolProperty
        If Not CurrencySymbol.StartsWith("$") AndAlso Not CurrencySymbol.StartsWith("ViewModel.") Then
          CurrencySymbol = "$data." & CurrencySymbol
        End If
        BindString &= ", CSymbol: " & CurrencySymbol
      End If

      Return BindString & "}"

    End Function

    Friend Shared Function GetDateBinding(Value As String, BindPath As String, da As Singular.DataAnnotations.DateField, Optional FormatString As String = "") As String

      Dim BindString = "{ Value: " & Value & ", Type: 'Date'"
      If BindPath <> "" Then
        BindPath &= "."
      End If
      If da IsNot Nothing AndAlso da.FormatString <> "" Then
        FormatString = da.FormatString
      End If
      If da IsNot Nothing Then
        If da.AutoChange <> Singular.DataAnnotations.AutoChangeType.None Then
          BindString &= ", AutoChange: " & da.AutoChange
        End If
        If da.YearRange <> "" Then
          BindString &= ", YearRange: '" & da.YearRange & "'"
        End If
        If FormatString <> "" Then
          BindString &= ", Format: '" & FormatString & "'"
        End If

        If da.MaxDateProperty <> "" Then
          BindString &= ", MaxDate: " & BindPath & da.MaxDateProperty
        End If
        If da.MinDateProperty <> "" Then
          BindString &= ", MinDate: " & BindPath & da.MinDateProperty
        End If
        If da.InitialDateFunction <> "" Then
          BindString &= ", Initial: " & da.InitialDateFunction
        End If
      End If

      'If the format string doesnt contain day, make it a month picker.
      If FormatString <> "" AndAlso Not FormatString.Contains("d") Then

        BindString &= ", MonthOnly: true"

      End If

      Return BindString & "}"

    End Function

    Friend Shared Function GetDialogBinding(VisibleConditionJS As String, TitleBinding As String, BeforeCloseFunctionName As String, AllowResize As Boolean,
                                            WidthBinding As String, OnOpenFunctionName As String) As String
      Return "{ Show: " & VisibleConditionJS &
        ", Title: " & TitleBinding &
        If(BeforeCloseFunctionName = "", "", ", beforeClose: " & BeforeCloseFunctionName) &
        If(AllowResize, "", ", Resizable: false") &
        If(WidthBinding = "", "", ", Width: " & WidthBinding) &
        If(OnOpenFunctionName = "", "", ", onOpen: " & OnOpenFunctionName) & "}"
    End Function

    Friend Shared Function GetIfTemplateBinding(Condition As String) As String
      Return "{ 'if': " & Condition & ", afterRender: Singular.AfterTemplateRender }"
    End Function

    Friend Shared Function GetWithTemplateBinding(ConditionAndData As String) As String
      Return "{ 'if': " & ConditionAndData & ", data: " & ConditionAndData & ", afterRender: Singular.AfterTemplateRender }"
    End Function

  End Class

#End Region

End Namespace
