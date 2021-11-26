Imports System.Reflection
Imports Csla.Core
Imports System.Linq.Expressions
Imports Singular.Linq

Namespace Rules

  Public Enum RuleSeverity
    [Error] = 1
    Warning = 2
    Information = 3
  End Enum

  Public Class RuleHelpers

    Private Shared GetRulesForTypeAccessor As MethodInfo
    Private Shared CreatedInstances As New Hashtable

    Shared Sub New()
      'GetRulesForType is private in the csla BusinessRuleManager, so get it with reflection
      For Each mi In GetType(Csla.Rules.BusinessRuleManager).GetMethods(System.Reflection.BindingFlags.Static Or System.Reflection.BindingFlags.NonPublic)
        If mi.Name = "GetRulesForType" AndAlso mi.GetParameters.Length = 1 Then
          GetRulesForTypeAccessor = mi
        End If
      Next
    End Sub

    Public Shared Function GetRulesForType(Type As Type) As List(Of Csla.Rules.IBusinessRule)

      'An instance of the type must have been created for the rules to have been added.
      'Check if we have created an instance.
      If CreatedInstances(Type.FullName) Is Nothing Then
        'If Type.IsClass AndAlso Not Type.IsAbstract Then
        '  'Check if the type has a parameter-less constructor.
        '  Dim Constructors = Type.GetConstructors(System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.NonPublic)
        '  For Each Constructor In Constructors
        '    If Constructor.GetParameters.Length = 0 Then
        '      Dim Obj = Activator.CreateInstance(Type, True)
        '      Exit For
        '    End If
        '  Next
        'End If
        Singular.Reflection.TryCreateInstance(Type)
        CreatedInstances(Type.FullName) = True
      End If

      Dim brm As Csla.Rules.BusinessRuleManager = GetRulesForTypeAccessor.Invoke(Nothing, {Type})
      Return brm.Rules

    End Function

    Public Shared Function GetRulesForProperty(pi As PropertyInfo) As List(Of Csla.Rules.IBusinessRule)
      Return GetRulesForProperty(Of Csla.Rules.IBusinessRule)(pi)
    End Function

    Public Shared Function GetRulesForProperty(Of RuleType As Csla.Rules.IBusinessRule)(pi As PropertyInfo) As List(Of RuleType)

      Dim FilteredRules As New List(Of RuleType)
      Dim AllRules = GetRulesForType(pi.ReflectedType)

      For Each Rule As Csla.Rules.IBusinessRule In AllRules

        If Rule.PrimaryProperty.Name = pi.Name AndAlso (Singular.Reflection.TypeImplementsInterface(Rule.GetType, GetType(RuleType)) OrElse Singular.Reflection.TypeInheritsFromOrIsType(Rule.GetType, GetType(RuleType))) Then
          FilteredRules.Add(Rule)
        End If

      Next

      Return FilteredRules

    End Function

  End Class

  Public Class LinqRule(Of ObjectType)
    Inherits Csla.Rules.BusinessRule
    Implements ILinqRule

    Protected mRuleProperty As Csla.Core.IPropertyInfo
    Protected mRuleFailDelegate As System.Func(Of ObjectType, Object)
    Protected mErrorTextDelegate As System.Func(Of ObjectType, Object)
    Protected mTriggerProperties As New Hashtable
    Public Property Severity As RuleSeverity = RuleSeverity.Error Implements ILinqRule.Severity

    Public Sub New(RuleProperty As IPropertyInfo,
                   RuleFailExpression As Expression(Of System.Func(Of ObjectType, Object)),
                   ErrorExpression As Expression(Of System.Func(Of ObjectType, Object)),
                   Optional Severity As Singular.Rules.RuleSeverity = Rules.RuleSeverity.Error)

      MyBase.New(RuleProperty)
      Me.InputProperties = New List(Of IPropertyInfo) From {RuleProperty}
      mRuleProperty = RuleProperty
      Me.Severity = Severity
      If RuleFailExpression IsNot Nothing Then
        mRuleFailDelegate = RuleFailExpression.Compile
      End If
      If ErrorExpression IsNot Nothing Then
        mErrorTextDelegate = ErrorExpression.Compile
      End If

      SetUniqueRuleName()

      Dim parser = GetParser(RuleFailExpression, ErrorExpression)

      If parser IsNot Nothing Then
        parser.Parse()

        'Get all the referenced properties from the parsed expression, and add them as trigger properties.
        For Each mi As System.Reflection.MemberInfo In parser.FoundMembers
          For Each pi As IPropertyInfo In Singular.Reflection.GetRegisteredProperties(GetType(ObjectType))
            If pi.Name = mi.Name Then
              AddTriggerProperty(pi)
            End If
          Next
        Next
      End If

    End Sub

    Private Shared mRuleNameUniqueHelper As New Hashtable
    Private mUniqueRuleName As String = ""
    Public ReadOnly Property UniqueRuleName As String Implements ILinqRule.UniqueRuleName
      Get
        Return mUniqueRuleName
      End Get
    End Property

    Private Sub SetUniqueRuleName()
      Dim TypeRuleName = GetType(ObjectType).FullName + mRuleProperty.Name
      If mRuleNameUniqueHelper.ContainsKey(TypeRuleName) Then
        mRuleNameUniqueHelper(TypeRuleName) += 1
        mUniqueRuleName = mRuleProperty.Name & "Rule" & mRuleNameUniqueHelper(TypeRuleName)
      Else
        mRuleNameUniqueHelper(TypeRuleName) = 0
        mUniqueRuleName = mRuleProperty.Name & "Rule"
      End If
      MyBase.RuleUri.AddQueryParameter("UniqueName", mUniqueRuleName)
    End Sub

    Public Sub AddTriggerProperty(pi As IPropertyInfo)
      If Not mTriggerProperties.ContainsKey(pi.Name) Then
        mTriggerProperties(pi.Name) = pi
        If pi.Name <> mRuleProperty.Name Then
          RuleHelpers.GetRulesForType(GetType(ObjectType)).Add(New Csla.Rules.CommonRules.Dependency(pi, mRuleProperty))
        End If
      End If
    End Sub

    Public Sub AddTriggerProperties(ParamArray pis() As IPropertyInfo)
      For Each pi As IPropertyInfo In pis
        AddTriggerProperty(pi)
      Next
    End Sub

    Protected Overridable Function GetParser(RuleFailExpression As Expression(Of System.Func(Of ObjectType, Object)), ErrorExpression As Expression(Of System.Func(Of ObjectType, Object))) As Singular.Linq.ExpressionParser(Of ObjectType)
      Return New Singular.Linq.ExpressionParser(Of ObjectType)(RuleFailExpression)
    End Function

    Protected Overrides Sub Execute(context As Csla.Rules.RuleContext)

      Dim IsBroken As Boolean = mRuleFailDelegate.Invoke(context.Target)

      If IsBroken Then
        SetErrorText(context, mErrorTextDelegate.Invoke(context.Target))
      End If

    End Sub

    Protected Sub SetErrorText(context As Csla.Rules.RuleContext, ErrorText As String)
      Select Case Severity
        Case RuleSeverity.Error
          context.AddErrorResult(ErrorText)
        Case RuleSeverity.Warning
          context.AddWarningResult(ErrorText)
        Case RuleSeverity.Information
          context.AddInformationResult(ErrorText)
      End Select
    End Sub

  End Class

  Public Interface ILinqRule
    Inherits Csla.Rules.IBusinessRule

    ReadOnly Property UniqueRuleName As String

    Property Severity As RuleSeverity
  End Interface

  Public Interface IJavascriptRule
    Inherits ILinqRule

    Function CheckServerRule(target As Object) As String
    Function GetTriggerProperties(ThisProperty As String) As String
    Sub WriteRuleJavascript(jw As Singular.Web.Utilities.JavaScriptWriter)
    Sub Suspend(Value As Boolean)

  End Interface

  Public Class JavascriptRule(Of ObjectType)
    Inherits LinqRule(Of ObjectType)
    Implements IJavascriptRule

    Private RuleParser As Singular.Linq.JavascriptExpressionParser(Of ObjectType)
    Private ErrorParser As Singular.Linq.JavascriptExpressionParser(Of ObjectType)

    Private mServerRuleFunction As Func(Of ObjectType, String)
    ''' <summary>
    ''' Address of a function that will check the rule on the server. If JavascriptRuleFunctionName and JavascriptRuleCode are not set, then this will be called
    ''' asynchronously from the client as well.
    ''' </summary>
    Public Property ServerRuleFunction As Func(Of ObjectType, String)
      Get
        Return mServerRuleFunction
      End Get
      Set(value As Func(Of ObjectType, String))
        If value IsNot mServerRuleFunction Then
          mServerRuleFunction = value

          ' also add dependency rules for affected properties

        End If
      End Set
    End Property


    ''' <summary>
    ''' The text to show the user while the rule is being sent/validated from client to server and back.
    ''' If this is blank, the rule will only be checked on the next post back.
    ''' </summary>
    Public Property AsyncBusyText As String = ""

    ''' <summary>
    ''' Async Rules by default only run when a dependant property changes. Set this to true to run the rule on load / object set etc.
    ''' </summary>
    Public Property RunAsyncOnLoad As Boolean = False

    ''' <summary>
    ''' If true, the server call will only be made if the object is dirty.
    ''' </summary>
    Public Property AsyncDirtyOnly As Boolean = False

    ''' <summary>
    ''' Name of a JavaScript function that must be called for Client Side validation. Must have Parameters {Value, Rule, CtlError}
    ''' </summary>
    Public Property JavascriptRuleFunctionName As String

    ''' <summary>
    ''' Actual JavaScript code that will be copied into the JavaScript object. Will be passed Parameters {Value, Rule, CtlError}
    ''' </summary>
    Public Property JavascriptRuleCode As String

    ''' <summary>
    ''' True if the rule must only be executed on the client. Rule will never break on the server.
    ''' </summary>
    Public Property ClientOnly As Boolean = False

    ''' <summary>
    ''' Creates a Rule that will be checked on the client, and the server.
    ''' </summary>
    Public Sub New(RuleProperty As IPropertyInfo,
                  RuleFailExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                  ErrorExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                  Optional Severity As Singular.Rules.RuleSeverity = Rules.RuleSeverity.Error)
      MyBase.New(RuleProperty, RuleFailExpression, ErrorExpression, Severity)

    End Sub

    Public Sub New(RuleProperty As IPropertyInfo, Optional Severity As Singular.Rules.RuleSeverity = Rules.RuleSeverity.Error)
      MyBase.New(RuleProperty, Nothing, Nothing, Severity)
    End Sub

    Protected Overrides Function GetParser(RuleFailExpression As Expression(Of System.Func(Of ObjectType, Object)), ErrorExpression As Expression(Of System.Func(Of ObjectType, Object))) As Linq.ExpressionParser(Of ObjectType)

      If RuleFailExpression IsNot Nothing Then

        RuleParser = New JavascriptExpressionParser(Of ObjectType)(RuleFailExpression, OutputModeType.GetProperty)
        RuleParser.IncludeReturnStatement = False

        ErrorParser = New JavascriptExpressionParser(Of ObjectType)(ErrorExpression, OutputModeType.GetProperty)
        ErrorParser.IncludeReturnStatement = False
        ErrorParser.Parse()

        Return RuleParser

      End If
      Return Nothing
    End Function

    Public Sub WriteRuleJavascript(jw As Singular.Web.Utilities.JavaScriptWriter) Implements IJavascriptRule.WriteRuleJavascript

      Dim IsInlineJSFunction As Boolean = RuleParser Is Nothing AndAlso JavascriptRuleFunctionName <> ""
      Dim HasCustomRule As Boolean = (ServerRuleFunction IsNot Nothing AndAlso ASyncBusyText <> "") OrElse JavascriptRuleCode <> "" OrElse JavascriptRuleFunctionName <> ""

      Dim affProp As String = ""
      If AffectedProperties IsNot Nothing AndAlso AffectedProperties.Where(Function(c) c.Name <> PrimaryProperty.Name).Count > 0 Then
        For Each prop In AffectedProperties.Where(Function(c) c.Name <> PrimaryProperty.Name)
          affProp &= "self." & prop.Name & ", "
        Next
        affProp = ", {}, [" & affProp.Substring(0, affProp.Length - 2) & "]"
      End If


      If IsInlineJSFunction Then
        'Simplest type where we have an address of a function.
        jw.Write("AddRule(self." & PrimaryProperty.Name & ",  " & JavascriptRuleFunctionName & affProp & ");")

      Else

        jw.Write("AddRule(self." & PrimaryProperty.Name & ", function(Value, Rule, CtlError){")
        jw.AddLevel()

        'Check the simple rule first (if there is one).
        If RuleParser IsNot Nothing Then

          jw.Write("if(" & RuleParser.ToString & "){")
          jw.AddLevel()
          If Severity = RuleSeverity.Error Then
            jw.Write("CtlError.AddError(" & ErrorParser.ToString & ");")
          ElseIf Severity = RuleSeverity.Warning Then
            jw.Write("CtlError.AddWarning(" & ErrorParser.ToString & ");")
          Else
            jw.Write("CtlError.AddInfo(" & ErrorParser.ToString & ");")
          End If
          jw.RemoveLevel()
          If Not HasCustomRule Then
            jw.Write("}")
          End If

        End If
        'Check the complex rule if the simple rule passes.
        If HasCustomRule Then

          If RuleParser IsNot Nothing Then
            jw.Write("} else {")
          End If

          If JavascriptRuleFunctionName <> "" Then
            jw.Write(JavascriptRuleFunctionName & "(Value, Rule, CtlError)")
          ElseIf JavascriptRuleCode <> "" Then
            jw.AddLevel()
            jw.Write(JavascriptRuleCode)
            jw.RemoveLevel()
          ElseIf ServerRuleFunction IsNot Nothing AndAlso ASyncBusyText <> "" Then
            WriteAsyncRuleCheck(jw)
          End If

          If RuleParser IsNot Nothing Then
            jw.Write("}")
          End If

        End If

        jw.RemoveLevel()
        jw.Write("}" & affProp & ");")

      End If

    End Sub

    Private Sub WriteAsyncRuleCheck(jw As Singular.Web.Utilities.JavaScriptWriter)
      'Get the Properties to Send.
      Dim PropertiesToSend As New Hashtable
      For Each sp As String In mTriggerProperties.Keys
        PropertiesToSend(sp) = "self." & sp & "()"
      Next
      PropertiesToSend(mRuleProperty.Name) = "self." & mRuleProperty.Name & "()"

      'Convert array to string
      Dim PropertiesToSendString As String = ""
      For Each key As String In PropertiesToSend.Keys
        Singular.Strings.Delimit(PropertiesToSendString, key & ":" & PropertiesToSend(key))
      Next

      jw.Write("CtlError.CheckRuleASync('" & ASyncBusyText & "', '" & UniqueRuleName & "', { " & PropertiesToSendString & " }" &
               ", " & RunAsyncOnLoad.ToString.ToLower & ", " & AsyncDirtyOnly.ToString.ToLower & ");")
    End Sub

    Protected CurrentContext As Csla.Rules.RuleContext

    Protected Overrides Sub Execute(context As Csla.Rules.RuleContext)
      If Not ClientOnly Then
        If Not mRuleSuspended Then
          Dim IsSimpleRuleBroken As Boolean = False
          'Check the simple rule expression if there is one
          If mRuleFailDelegate IsNot Nothing Then
            IsSimpleRuleBroken = mRuleFailDelegate.Invoke(context.Target)
          End If

          If IsSimpleRuleBroken Then
            'If its broken, add the error result.
            SetErrorText(context, mErrorTextDelegate.Invoke(context.Target))
          ElseIf ServerRuleFunction IsNot Nothing Then
            'Only check the complicated rule if the simple rule passes.

            If String.IsNullOrEmpty(AsyncBusyText) OrElse Not AsyncDirtyOnly OrElse CType(context.Target, Singular.ISingularBusinessBase).IsSelfDirty Then

              CurrentContext = context
              Dim Err = CheckServerRule(context.Target)
              If Err.Length > 0 Then
                SetErrorText(context, Err)
              End If

            End If
          End If
        End If
      End If
    End Sub

    Public Function CheckServerRule(target As Object) As String Implements IJavascriptRule.CheckServerRule
      Return ServerRuleFunction(target)
    End Function

    Public Function GetTriggerProperties(ThisProperty As String) As String Implements IJavascriptRule.GetTriggerProperties
      Dim Triggers As String = ThisProperty
      For Each tp As String In mTriggerProperties.Keys
        If Not ThisProperty.EndsWith(tp) Then
          Singular.Strings.Delimit(Triggers, tp)
        End If
      Next
      Return Triggers
    End Function

    Private mRuleSuspended As Boolean = False
    Public Sub Suspend(Value As Boolean) Implements IJavascriptRule.Suspend
      mRuleSuspended = Value
    End Sub
  End Class

  Public Class ASyncRuleResult

    Public Sub New(ErrorText As String, Optional Severity As Singular.Rules.RuleSeverity = RuleSeverity.Error)
      Me.Error = ErrorText
      Me.Severity = Severity
    End Sub

    Public Property Severity As Singular.Rules.RuleSeverity
    Public Property [Error] As String

  End Class

End Namespace


