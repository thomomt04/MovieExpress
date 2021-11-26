Imports System.Reflection
Imports Singular.Dynamic
Imports System.Web.Helpers
Imports Singular.Web.Utilities

Namespace Web.Data.JS.ObjectInfo

  <Serializable()>
  Public Class ObjectMember
    Inherits Member

    Private mTypeName As String

    Public Property CreateNew As Boolean = False
    Public Property IsReadOnly As Boolean = False

    Public Sub New(ph As PropertyHelper, Obj As Object, Index As Integer, JSSerialiser As JSSerialiser, ti As Singular.ReflectionCached.TypeInfo)
      Dim Inst As Object = Setup(ph, Obj, Index, JSSerialiser, ti)

      If mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.DataSet Then
        'Dataset
        IsReadOnly = True
        mMemberList = New MemberList
        If Inst IsNot Nothing Then
          For Each table As DataTable In CType(Inst, DataSet).Tables

            If table.ParentRelations.Count = 0 Then
              Dim m = mMemberList.CreateMember(New PropertyHelper(table.TableName, mTypeAtRuntime, table.GetType), table, JSSerialiser)

            End If
          Next
        End If

        mTypeName = mJSonPropertyName & "_Dataset"
        JSSerialiser.TypeDefinitionList.AddTypeDefinition(mTypeAtRuntime, Me, mTypeName)
      Else
        'Normal
        Dim CustomResolveType = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ResolveType)(mTypeAtRuntime)
        If CustomResolveType IsNot Nothing Then
          mTypeAtRuntime = CustomResolveType.GetActualType(Inst, Obj, JSSerialiser)
        End If

        mTypeName = TypeDefinitionList.GetTypeName(mTypeAtRuntime)

        ComplexTypeSetup(mTypeAtRuntime, Inst)

        If ph.PropertyInfo IsNot Nothing Then
          Dim ObjAttribute = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ObjectProperty)(ph.PropertyInfo)
          If ObjAttribute IsNot Nothing AndAlso ObjAttribute.CreateNew Then
            CreateNew = True
          End If

        End If
        IsReadOnly = mPH.IsDynamicallyAdded

        If mPH.CachedPropertyInfo IsNot Nothing AndAlso mPH.CachedPropertyInfo.ClientOnly.HasValue AndAlso mPH.CachedPropertyInfo.ClientOnly Then
          IsReadOnly = True
        End If
      End If


    End Sub

    Friend Overrides Sub RenderModel(JW As Singular.Web.Utilities.JavaScriptWriter)

      If PreRenderModel(JW) Then

        'If this is the base object (the view model), then create the base instance.
        If GetType(JSSerialiser).IsAssignableFrom(mPH.OnType) Then
          JW.Write("self." & JSonPropertyName & " = new " & mTypeName & "();")
          JW.Write("self." & JSonPropertyName & ".SInfo.MarkOld()")
        Else
          'otherwise just render the property name as an observable.

          RenderModelProperty(JW, mPH, CreateNew, False, IsReadOnly, mTypeName)
        End If

      End If

    End Sub

    Friend Overrides Sub RenderSchema(jw As JSonWriter)
      If mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.DataSet Then
        'Hack... Datasets have their tables as members
        jw.StartClass(JSonPropertyName)
        For Each m As Member In MemberList
          m.RenderSchema(jw)
        Next
        jw.EndClass()

      Else
        RenderObjectSchema(JSonPropertyName, mMemberList, jw, mPH.CachedPropertyInfo.AutoGenerate)
      End If

    End Sub

    Friend Shared Sub RenderObjectSchema(Name As String, MemberList As MemberList, jw As JSonWriter, AutoGenerate As Boolean)
      jw.StartClass(Name)

      If Not AutoGenerate Then
        jw.WriteProperty("Hidden", True)
      Else
        jw.StartClass("Properties")

        Dim HasOrder = MemberList.Exists(Function(c) c.Order <> 0)
        Dim SortedList = If(HasOrder, MemberList.OrderBy(Function(c) c.Order), MemberList)

        For Each m As Member In SortedList
          m.RenderSchema(jw)
        Next
        jw.EndClass()
      End If

      jw.EndClass()
    End Sub

    Friend Shared Sub RenderModelProperty(jw As Singular.Web.Utilities.JavaScriptWriter,
                                          ph As PropertyHelper,
                                          CreateNew As Boolean,
                                          IsArray As Boolean,
                                          IsReadOnly As Boolean,
                                          TypeName As String, Optional ExludeClosing As Boolean = False)

      If IsReadOnly Then
        jw.Write("CreateTypedROProperty(self, '" & ph.Name & "', " & TypeName & ", " & IsArray.ToString.ToLower & ", " & CreateNew.ToString.ToLower & ")", False)
      Else
        If CreateNew Then
          jw.Write("CreateTypedProperty(self, '" & ph.Name & "', " & TypeName & ", " & IsArray.ToString.ToLower & ", true)", False)
        Else
          jw.Write("CreateTypedProperty(self, '" & ph.Name & "', " & TypeName & ", " & IsArray.ToString.ToLower & ")", False)
        End If
      End If

      If ph.IsProperty Then
        Dim nr = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NoBrokenRules)(ph.PropertyInfo)
        If nr IsNot Nothing Then
          'No Rules Attribute.
          jw.RawWriteLine("")
          jw.AddLevel()
          jw.Write(".NoRules()", False)
          jw.RemoveLevel()
        End If

        If Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.PrimaryProperty)(ph.PropertyInfo) IsNot Nothing Then
          jw.RawWriteLine("")
          jw.AddLevel()
          jw.Write(".IsPrimary()", False)
          jw.RemoveLevel()
        End If
      End If

      If Not ExludeClosing Then
        jw.RawWriteLine(";")
      End If

    End Sub

    Public Overrides Sub UpdateModel(Dynamic As System.Dynamic.DynamicObject, Model As Object)

      'Get the Value From the JSon Object
      Dim DObj As DynamicJsonObject = Nothing
      If Dynamic.TryGetMember(New MemberGetter(mPH.Name), DObj) Then

        Dim IsReference As Boolean = False
        If DObj IsNot Nothing Then
          DObj.TryGetMember(New MemberGetter("_Reference"), IsReference)
        End If

        Dim Guid As String = ""
        Dim Inst As Object = Nothing
        Dim ExistingObject As Object = Nothing

        If DObj IsNot Nothing Then
          'If the client object is set, then get the server object based on the GUID.
          If DObj.TryGetMember(New MemberGetter("Guid"), Guid) Then

            If IsReference Then
              Inst = mJSSerialiser.GetExistingObject(New Guid(Guid))
            Else

              If TypeOf mJSSerialiser Is StatelessJSSerialiser Then
                Inst = mPH.GetValue(Model)
              ElseIf Guid <> "" Then
                Dim obj = mJSSerialiser.GetExistingObject(New Guid(Guid))
                If obj IsNot Nothing Then
                  If mPH.GetValue(Model) IsNot Nothing Then
                    Inst = obj
                  Else
                    ExistingObject = obj
                  End If
                End If
              End If

            End If

          End If
        Else
          'If the client object is nothing, then get the server object from the property.
          Inst = mPH.GetValue(Model)
        End If

        If Inst Is Nothing AndAlso DObj IsNot Nothing AndAlso mPH.PropertyInfo.CanWrite Then
          'Create new object
          If ExistingObject IsNot Nothing Then
            Inst = ExistingObject
          Else
            Try
              Inst = Activator.CreateInstance(mTypeAtRuntime, True)
            Catch ex As Exception
              Throw New Exception("Cannot create instance of type '" & mTypeAtRuntime.Name & "' on property '" & mPH.Name & "'", ex)
            End Try

            If TypeOf Inst Is ISingularBase AndAlso Guid IsNot Nothing Then
              CType(Inst, ISingularBase).Guid = New Guid(Guid)
            End If
          End If

          'Add the object to the hashtable
          If DObj.TryGetMember(New MemberGetter("Guid"), Guid) AndAlso Guid IsNot Nothing Then
            mJSSerialiser.StoreObject(Inst, Nothing, Guid)
          End If
        End If

        If Inst IsNot Nothing AndAlso DObj Is Nothing AndAlso mPH.PropertyInfo.CanWrite Then
          'Delete the object.
          mPH.PropertyInfo.SetValue(Model, Nothing, Nothing)

        Else

          'Set the property.
          If mPH.PropertyInfo.CanWrite Then
            mPH.PropertyInfo.SetValue(Model, Inst, Nothing)
          End If

          'Update the objects properties.
          If Not IsReference Then
            UpdateObjectFromClientObject(mJSSerialiser, mMemberList, DObj, Inst)
          End If

        End If

      End If

    End Sub

    Friend Shared Sub UpdateObjectFromClientObject(JSSerialiser As JSSerialiser, MemberList As MemberList, ClientObject As System.Dynamic.DynamicObject, ServerObject As Object)

      If ServerObject IsNot Nothing AndAlso ClientObject IsNot Nothing Then

        Dim OldMode = JSSerialiser.CurrentPropertySetMode
        Dim IsBusinessBase As Boolean = GetType(Singular.ISingularBusinessBase).IsAssignableFrom(ServerObject.GetType)
        Dim IsSingularBase As Boolean = GetType(Singular.ISingularBase).IsAssignableFrom(ServerObject.GetType)
        Dim SServerObject As Singular.ISingularBusinessBase = Nothing
        If IsBusinessBase Then
          SServerObject = ServerObject
        End If

        If IsSingularBase AndAlso JSSerialiser.DefaultPropertySetMode = JS.JSSerialiser.SetMode.SetBackingField Then
          JSSerialiser.CurrentPropertySetMode = JS.JSSerialiser.SetMode.SetBackingField
        Else
          JSSerialiser.CurrentPropertySetMode = JS.JSSerialiser.SetMode.SetProperty
        End If

        'Get the server objects current value, and only update the property if it has changed since it was sent to the client.
        If JSSerialiser.CurrentPropertySetMode = JS.JSSerialiser.SetMode.SetProperty Then
          For Each m As Member In MemberList
            If TypeOf m Is SimpleMember Then
              CType(m, SimpleMember).SetTempPropertyValue(ServerObject)
            End If
          Next
        End If

        'Update the IsNew property of the object.
        Dim MemberNames = ClientObject.GetDynamicMemberNames()
        If MemberNames.Contains("IsNew") AndAlso IsBusinessBase Then
          If Not CType(ClientObject, Object).IsNew AndAlso SServerObject.IsNew Then
            Dim WasDirty As Boolean = SServerObject.IsSelfDirty
            SServerObject.MarkOld()

            If WasDirty Then
              SServerObject.MarkDirty()
            End If
          End If
        End If

        'Update the objects properties.
        If ClientObject IsNot Nothing Then
          For Each field As String In MemberNames

            Dim m As Member = MemberList.GetItem(field)
            If m IsNot Nothing AndAlso Not m.PropertyHelper.IsDynamicallyAdded Then
              m.UpdateModel(ClientObject, ServerObject)
            End If

          Next

        End If

        'Set the localisation data
        Dim LocData_EnglishStrings As DynamicJsonObject = Nothing
        If ClientObject.TryGetMember(New MemberGetter(Singular.Localisation.Data.JSONPropertyName), LocData_EnglishStrings) AndAlso LocData_EnglishStrings IsNot Nothing Then
          SServerObject.LocalisationDataValues = Localisation.Data.DataValueList.FromJSon(LocData_EnglishStrings)
        End If

        'Check if the object needs to be marked as clean.
        If MemberNames.Contains("IsDirty") AndAlso IsBusinessBase Then
          If CType(ClientObject, Object).IsDirty Then
            SServerObject.MarkDirty()
          Else
            SServerObject.MarkClean()
          End If

        End If

        JSSerialiser.CurrentPropertySetMode = OldMode

      End If



    End Sub

    Friend Overrides Sub RenderData(JW As JSonWriter, Model As Object)

      Dim Inst As Object = mPH.GetValue(Model)

      If CanRenderData(JW, Inst) Then

        If Inst IsNot Nothing Then
          If JSonPropertyName <> "" Then
            JW.StartClass(JSonPropertyName)
          Else
            JW.StartClass("")
          End If

          Dim RenderMembers As Boolean = True

          If JSSerialiser.RenderGuid Then
            JW.WritePropertyName("Guid")
            Dim soi As JSSerialiser.StoreObjectInfo = mJSSerialiser.StoreObject(Inst, Nothing)
            RenderMembers = Not soi.PreviouslyAdded
            JW.WriteJSonValue(soi.Guid)
          End If

          If RenderMembers Then

            Dim IsSingularObject As Boolean = GetType(ISingularBase).IsAssignableFrom(mTypeAtRuntime)
            If IsSingularObject AndAlso CType(Inst, ISingularBase).LocalisationDataValues IsNot Nothing Then
              CType(Inst, ISingularBase).LocalisationDataValues.WriteJSon(JW)
            End If

            Member.RenderMemberData(JW, Inst, mMemberList)
          Else
            JW.WriteProperty("_Reference", True)
          End If

          JW.EndClass()
        Else
          JW.WritePropertyName(mPH.Name)
          JW.WriteJSonValue(Nothing)
        End If

      End If

      'Don't do this for each item in the list, just once per property.
      mJSSerialiser.OnRenderData(Me, Inst)

    End Sub

    Protected Friend Overrides Sub RenderStaticTypeInfo(TypeName As String, jw As JavaScriptWriter)
      MyBase.RenderStaticTypeInfo(TypeName, jw)

      RenderDataLocalisationProperties(TypeName, ReflectionCached.GetCachedType(mTypeAtRuntime), jw)
    End Sub

    'Friend Overrides Sub UpdateItemTracker(Model As Object)

    '  If Model IsNot Nothing Then
    '    Dim Inst = mPH.GetValue(Model)

    '    mJSSerialiser.StoreObject(Inst, Nothing)

    '    For Each m As Member In mMemberList
    '      m.UpdateItemTracker(Inst)
    '    Next
    '  End If

    'End Sub

    'Friend Overrides Function Find(Model As Object, ObjectToFind As Object) As Member

    '  If Model IsNot Nothing Then
    '    Dim Inst = mPH.GetValue(Model)
    '    If Inst Is ObjectToFind Then
    '      Return Me
    '    End If

    '    For Each m As Member In mMemberList
    '      Dim Found = m.Find(Inst, ObjectToFind)
    '      If Found IsNot Nothing Then
    '        Return Found
    '      End If
    '    Next
    '  End If

    '  Return Nothing
    'End Function

  End Class

End Namespace