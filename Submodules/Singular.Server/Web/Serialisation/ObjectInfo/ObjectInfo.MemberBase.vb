Imports System.Reflection

Namespace Web.Data.JS.ObjectInfo

  <Serializable()>
  Public MustInherit Class Member
    Protected mPH As PropertyHelper
    Protected mIndex As Integer 'The position of this Object's Property / Column in the Parent
    Protected mMemberList As New MemberList 'The Properties in this Object
    Protected mTypeAtRuntime As Type 'The property type might be object, but it returns a String at run time.
    Protected mPropertyInfo As Csla.Core.IPropertyInfo
    Protected mHasExtraPropertyInfo As Boolean = False
    Protected mJSSerialiser As JSSerialiser
    Protected mJSMethodList As New List(Of Singular.Web.Utilities.JavascriptMethod)
    Protected mCanRender As Boolean = False
    Protected mCalculatedCanRender As Boolean = False
    Protected mTypeInfo As Singular.ReflectionCached.TypeInfo
    Protected mTypeRendererInstance As Data.JS.ITypeRenderer
    Protected mSetExpr As Singular.DataAnnotations.SetExpression

    Friend Property Order As Integer

    Protected mJSonPropertyName As String = ""
    Public Property JSonPropertyName As String
      Get
        Return mJSonPropertyName
      End Get
      Set(value As String)
        mJSonPropertyName = value
        mRenderPropertyName = mJSonPropertyName <> ""
      End Set
    End Property
    Protected mRenderPropertyName As Boolean = True

    Protected Function Setup(ph As PropertyHelper, Obj As Object, Index As Integer, JSSerialiser As JSSerialiser, ti As Singular.ReflectionCached.TypeInfo) As Object

      mIndex = Index
      mJSSerialiser = JSSerialiser
      mPH = ph
      mTypeInfo = ti

      Dim Inst As Object = Nothing

      If mPH.IsValueType Then
        mTypeAtRuntime = mPH.GetReturnType(Obj)
      ElseIf mPH.IsProperty AndAlso (mPH.CachedPropertyInfo Is Nothing OrElse Not mPH.CachedPropertyInfo.IsNullableType) Then

        If Obj IsNot Nothing AndAlso mPH.OnType Is Obj.GetType Then
          Inst = mPH.GetValue(Obj)
          If Inst IsNot Nothing AndAlso Inst IsNot DBNull.Value Then
            If TypeOf Inst Is Type Then
              mTypeAtRuntime = Inst
            Else
              mTypeAtRuntime = Inst.GetType
            End If
          End If
        End If
      End If

      If mTypeAtRuntime Is Nothing Then
        mTypeAtRuntime = mPH.GetReturnType(Obj)
      End If

      If mTypeAtRuntime Is GetType(DataTable) AndAlso TypeOf Obj Is DataTable Then
        Inst = Obj
      End If

      mTypeRendererInstance = Data.JS.TypeRendererHelper.GetTypeRendererInstance(mTypeAtRuntime)

      'See if it has a csla property info
      mPropertyInfo = If(ph.CachedPropertyInfo IsNot Nothing, ph.CachedPropertyInfo.BackingField, Nothing)
      mHasExtraPropertyInfo = TypeOf mPropertyInfo Is Singular.ISingularPropertyInfo

      If mPH.CachedPropertyInfo IsNot Nothing Then
        Order = ph.CachedPropertyInfo.Order
      End If

      JSonPropertyName = mPH.Name

      Return Inst
    End Function

    Protected Sub ComplexTypeSetup(RunTimeType As Type, ExampleInstance As Object)

      'Check if this type renders itself

      If mPH.PropertyInfo IsNot Nothing AndAlso Singular.Reflection.GetAttribute(Of DataAnnotations.RawDataOnly)(mPH.PropertyInfo) IsNot Nothing Then
        mTypeRendererInstance = New ClasslessRenderer
      End If

      Dim TypeRenderingMode As RenderTypeOption = If(mTypeRendererInstance Is Nothing, RenderTypeOption.RenderThisTypeAndChildTypes, mTypeRendererInstance.TypeRenderingMode)

      If TypeRenderingMode <> RenderTypeOption.RenderThisTypeAndChildTypes Then
        JSSerialiser.TypeDefinitionList.SupressRendering()
      End If

      'Add the type to the list of types to be rendered
      If JSSerialiser.TypeDefinitionList.AddTypeDefinition(RunTimeType, Me) Then
        If TypeRenderingMode = RenderTypeOption.RenderChildTypesOnly Then
          JSSerialiser.TypeDefinitionList.RestoreRendering()
        End If
        FindMembers(RunTimeType, ExampleInstance, True, mMemberList)
      Else
        'this type is already added, its properties have already been found
        mMemberList = JSSerialiser.TypeDefinitionList.GetItem(RunTimeType).Member.MemberList
      End If

      If TypeRenderingMode = RenderTypeOption.RenderNoTypes Then
        JSSerialiser.TypeDefinitionList.RestoreRendering()
      End If

    End Sub

    Protected Sub FindMembers(ObjectType As Type, CurrentInstance As Object, IncludeMethods As Boolean, MemberList As MemberList)

      'Dim lMemberList = MemberList

      If Singular.Reflection.TypeInheritsFromOrIsType(ObjectType, GetType(System.Dynamic.DynamicObject)) Then
        'For a dynamic object, get the member names.

        For Each Member As String In CType(CurrentInstance, System.Dynamic.DynamicObject).GetDynamicMemberNames
          Dim ph As New PropertyHelper(Nothing, Member, ObjectType)
          MemberList.CreateMember(ph, CurrentInstance, mJSSerialiser)
        Next

      Else
        Dim ContextList As UIContextList
        If mJSSerialiser IsNot Nothing Then
          ContextList = mJSSerialiser.ContextList
        Else
          ContextList = New UIContextList
        End If
        ObjectType.ForEachBrowsableProperty(ContextList, Sub(pi)

                                                           MemberList.CreateMember(New PropertyHelper(pi, pi.Name, ObjectType), CurrentInstance, mJSSerialiser)

                                                         End Sub, True, mJSSerialiser.SortProperties, , mJSSerialiser.IncludeIsDirty)

      End If

      If IncludeMethods Then

        'Include ToString (Won't be included above cause its a Method not a property).
        Dim miToString = ObjectType.GetMethod("ToString", BindingFlags.Public Or BindingFlags.Instance)
        If miToString IsNot Nothing Then
          MemberList.CreateMember(New PropertyHelper(miToString, miToString.Name, ObjectType), CurrentInstance, mJSSerialiser)
        End If

        'Look for Javascript Methods
        Dim fis = ObjectType.GetFields(BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy)
        For Each fi As FieldInfo In fis
          If Singular.Reflection.TypeInheritsFromOrIsType(fi.FieldType, GetType(Singular.Web.Utilities.JavascriptMethod)) Then
            Dim JSMethod As Singular.Web.Utilities.JavascriptMethod = fi.GetValue(Nothing)
            mJSMethodList.Add(JSMethod)
          End If
        Next

      End If

    End Sub

    Public Function CreateMember(Name As String, ReturnType As Type) As Member
      Return CreateMember(Name, mTypeAtRuntime, ReturnType, JSSerialiser)
    End Function

    Public Function CreateMember(Name As String, OnType As Type, ReturnType As Type, JSSerialiser As JSSerialiser) As Member
      If mMemberList.HasMember(Name) Then
        Return Nothing
      Else
        Return MemberList.CreateMember(New PropertyHelper(Name, OnType, ReturnType), Nothing, JSSerialiser)
      End If
    End Function

    Friend Sub RenderRules(jw As Singular.Web.Utilities.JavaScriptWriter)

      Dim IsOldCSLAObject As Boolean = Not Singular.Reflection.TypeImplementsInterface(RuleType, GetType(Csla.Core.IBusinessObject))

      'Normal CSLA Rules including data annotations
      If Not IsOldCSLAObject Then
        For Each Rule In Singular.Rules.RuleHelpers.GetRulesForType(RuleType)
          Dim Member = mMemberList.GetItem(Rule.PrimaryProperty.Name)
          If Member IsNot Nothing Then
            If TypeOf Rule Is Singular.Rules.IJavascriptRule Then
              CType(Rule, Singular.Rules.IJavascriptRule).WriteRuleJavascript(jw)
            ElseIf TypeOf Rule Is Csla.Rules.CommonRules.DataAnnotation Then

              Dim CSLARule As Csla.Rules.CommonRules.DataAnnotation = Rule
              Dim PropHelper = Member.mPH
              Dim DRule As New JSRule(CSLARule.Attribute, CSLARule.PrimaryProperty.Name, If(PropHelper Is Nothing, Nothing, PropHelper.CachedPropertyInfo))
              If DRule.Rule IsNot Nothing Then
                jw.Write("AddRule(self." & Rule.PrimaryProperty.Name & ", '" & DRule.Rule & "', " & DRule.GetRuleArgsJS & ");")
              End If

            End If
          End If
        Next
      End If

      For Each m As Member In mMemberList
        If m.PropertyHelper.IsProperty Then

          'JSRule DataAnnotations
          Dim JSR = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.JSRule)(m.PropertyHelper.PropertyInfo)
          If JSR IsNot Nothing AndAlso JSR.HasLogic Then

            If JSR.FunctionName <> "" Then
              jw.Write("AddRule(self." & m.PropertyHelper.Name & ", " & JSR.FunctionName & ");")
            Else
              jw.Write("AddRule(self." & m.PropertyHelper.Name & ", function(Value, Rule, CtlError){")
              jw.AddLevel()
              jw.Write(JSR.JavascriptCode)
              jw.RemoveLevel()
              jw.Write("});")
            End If
          End If

          'Data annotation
          If IsOldCSLAObject Then

            For Each DARule As Attribute In m.PropertyHelper.PropertyInfo.GetCustomAttributes(GetType(System.ComponentModel.DataAnnotations.ValidationAttribute), False)
              Dim DRule As New JSRule(DARule, m.PropertyHelper.PropertyInfo.Name, Nothing)
              If DRule.Rule IsNot Nothing Then
                jw.Write("AddRule(self." & m.PropertyHelper.PropertyInfo.Name & ", '" & DRule.Rule & "', " & DRule.GetRuleArgsJS & ");")
              End If
            Next

          End If

        End If
      Next

    End Sub

    Protected Friend Sub RenderModelMembers(JW As Singular.Web.Utilities.JavaScriptWriter)

      For Each m As Member In mMemberList
        'Render property / function
        If m.mTypeRendererInstance IsNot Nothing Then
          m.mTypeRendererInstance.RenderModel(JW, m)
        Else
          If m.CanRender Then
            m.RenderModel(JW)
          End If
        End If
      Next

    End Sub

    Protected Friend Sub RenderJSMethods(ByVal JW As Singular.Web.Utilities.JavaScriptWriter)

      For Each jsm As Singular.Web.Utilities.JavascriptMethod In mJSMethodList

        JW.WriteStartFunction(jsm.MethodName)
        JW.WriteBlock(jsm.MethodBody)
        JW.WriteEndFunction()

      Next

    End Sub

    Protected Friend Overridable Sub RenderAdditionalTypeInfo(jw As Singular.Web.Utilities.JavaScriptWriter)

    End Sub

    Protected Friend Overridable Sub RenderStaticTypeInfo(TypeName As String, jw As Singular.Web.Utilities.JavaScriptWriter)

    End Sub

    Protected Sub RenderDataLocalisationProperties(TypeName As String, TypeInfo As ReflectionCached.TypeInfo, JW As Singular.Web.Utilities.JavaScriptWriter)

      'Data localisation
      If Localisation.Data.CanPerformDataLocalisation(TypeInfo) Then

        Dim Properties = Localisation.Data.GetDataLocalisedProperties(TypeInfo)
        If Properties.Count > 0 Then

          JW.Write(TypeName & ".LocalisedProperties = {")
          JW.AddLevel()
          Dim First As Boolean = True
          For i As Integer = 0 To Properties.Count - 1
            JW.Write(Properties(i).Name & ": true" & If(i = Properties.Count - 1, String.Empty, ", "))
          Next
          JW.RemoveLevel()
          JW.Write("}")

        End If

      End If

    End Sub

    Protected Shared Sub RenderMemberData(JW As JSonWriter, Instance As Object, MemberList As MemberList)

      For Each m As Member In MemberList
        'If m.mTypeRendererInstance IsNot Nothing AndAlso m.mTypeRendererInstance.RendersData Then
        '  m.mTypeRendererInstance.RenderData(m.PropertyHelper.GetValue(Instance), JW, m)
        'Else
        If m.CanRender Then
          m.RenderData(JW, Instance)
        End If
        'End If
      Next

    End Sub

    'If its conditionally browsable.
    Private Function CanRender() As Boolean
      If Not mCalculatedCanRender Then
        If mPH.CachedPropertyInfo IsNot Nothing Then
          mCanRender = mPH.CachedPropertyInfo.BrowsableAllowedContext Is Nothing OrElse
            (mJSSerialiser IsNot Nothing AndAlso mJSSerialiser.ContextList.HasContext(mPH.CachedPropertyInfo.BrowsableAllowedContext))

          If mPH.CachedPropertyInfo.InitialDataOnly AndAlso Not mJSSerialiser.IsInitial Then
            mCanRender = False
          End If
        Else
          mCanRender = True
        End If
        mCalculatedCanRender = True
      End If
      Return mCanRender
    End Function

    Protected ReadOnly Property SPropertyInfo As ISingularPropertyInfo
      Get
        If TypeOf mPropertyInfo Is ISingularPropertyInfo Then
          Return mPropertyInfo
        Else
          Return Nothing
        End If
      End Get
    End Property

    Public ReadOnly Property JSSerialiser As JSSerialiser
      Get
        Return mJSSerialiser
      End Get
    End Property

    Public ReadOnly Property PropertyHelper As PropertyHelper
      Get
        Return mPH
      End Get
    End Property

    Public ReadOnly Property MemberList As MemberList
      Get
        Return mMemberList
      End Get
    End Property

    Public ReadOnly Property TypeAtRunTime As Type
      Get
        Return mTypeAtRuntime
      End Get
    End Property

    Protected Overridable ReadOnly Property RuleType As Type
      Get
        Return mTypeAtRuntime
      End Get
    End Property

    ''' <summary>
    ''' Checks if the property has its own get expression, and renders it. Returns true if the caller must carry on with rendering.
    ''' </summary>
    Protected Function PreRenderModel(JW As Singular.Web.Utilities.JavaScriptWriter) As Boolean

      If Not mHasExtraPropertyInfo AndAlso Not mPH.IsProperty AndAlso Not mPH.IsDynamicallyAdded Then
        Return False
      End If

      If mHasExtraPropertyInfo AndAlso SPropertyInfo.HasGetExpression Then
        'ReadOnly Computed properties.
        JW.Write("self." & mPH.Name & " = ko.computed({read: function() {")
        JW.AddLevel()
        SPropertyInfo.WriteGet(JW)
        JW.RemoveLevel()

        If SPropertyInfo.HasSetExpressions Then
          JW.Write("}, write: function(value) {")
          JW.AddLevel()
          For Each setExp In SPropertyInfo.SetExpressionList
            setExp.WriteSet(JW)
          Next
          JW.RemoveLevel()
        Else
        End If

        JW.Write("}, deferEvaluation: true});")

        If mSetExpr IsNot Nothing Then
          JW.Write("self." & mPH.Name & ".OnReturnValueChanged(function(){ " & mSetExpr.JavascriptCode & " });")
        End If

        Return False
      End If

      Return True

    End Function

    Friend MustOverride Sub RenderModel(JW As Singular.Web.Utilities.JavaScriptWriter)
    Friend MustOverride Sub RenderSchema(JS As JSonWriter)

    Protected Function CanRenderData(JW As JSonWriter, Instance As Object) As Boolean

      If mTypeRendererInstance IsNot Nothing AndAlso mTypeRendererInstance.RendersData Then
        Dim BaseRender As Boolean = False
        mTypeRendererInstance.RenderData(Instance, JW, Me, BaseRender)
        Return BaseRender

      ElseIf (mHasExtraPropertyInfo AndAlso SPropertyInfo.HasGetExpression) OrElse Not mPH.WritesData Then
        Return False

      End If

      Return True
    End Function

    Friend MustOverride Sub RenderData(JW As JSonWriter, Model As Object)
    Public MustOverride Sub UpdateModel(Dynamic As System.Dynamic.DynamicObject, Model As Object)

    'Friend Overridable Sub UpdateItemTracker(Model As Object)

    'End Sub

    'Friend Overridable Function Find(Model As Object, ObjectToFind As Object) As Member
    '  Return Nothing
    'End Function

  End Class

End Namespace