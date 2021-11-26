Imports System.Reflection
Imports Singular.Dynamic
Imports System.Web.Helpers

Namespace Web.Data.JS.ObjectInfo

  <Serializable()>
  Public Class ComplexListMember
    Inherits Member

    Private mChildObjectName As String
    Private mClientOnlyProperty As Boolean = False
    Private mChildType As Type
    Private mIsReadOnly As Boolean
    Private mIsPagedList As Boolean
    Private mHasVariableItemTypes As Boolean = False


    Public Sub New(ph As PropertyHelper, Obj As Object, Index As Integer, JSSerialiser As JSSerialiser, ti As Singular.ReflectionCached.TypeInfo)
      Dim Inst As Object = Setup(ph, Obj, Index, JSSerialiser, ti)

      mChildType = ti.LastGenericType
      mIsReadOnly = ti.ReadOnlyList OrElse mTypeAtRuntime.IsEnum
      mIsPagedList = ti.IsPagedList
      If Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.VariableContents)(ti.Type) IsNot Nothing Then
        mHasVariableItemTypes = True
      End If

      If (mChildType Is Nothing OrElse mChildType Is GetType(Object)) AndAlso Not mTypeAtRuntime.IsEnum Then
        'If the child type is not known, then just look at the first item in the list.
        Try
          If Inst.Count > 0 Then
            mChildType = Inst(0).GetType
          End If
        Catch ex As Exception
          mChildType = Nothing
        End Try
      End If

      If mChildType IsNot Nothing Then

        Dim CustomResolveType = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ResolveType)(mChildType)
        If CustomResolveType IsNot Nothing Then
          mChildType = CustomResolveType.GetActualType(Inst, Obj, JSSerialiser)
        End If

        If mChildType.GetInterfaces.Contains(GetType(IEnumerable)) Then
          Throw New Exception("List of Lists is not supported: Child Type is " & mChildType.Name)
        End If

        Dim co = mPH.CachedPropertyInfo.ClientOnly
        If co IsNot Nothing Then
          If co.Value Then
            mClientOnlyProperty = True
          End If
        Else
          If mIsReadOnly Then
            mClientOnlyProperty = True
          End If
        End If

        Dim FirstObject As Object = Nothing
        Try
          If Inst IsNot Nothing AndAlso Inst.Count > 0 Then
            FirstObject = Inst(0)
          End If
        Catch ex As Exception

        End Try

        Dim IsNewType As Boolean = True
        mChildObjectName = TypeDefinitionList.GetTypeName(mChildType)

        ComplexTypeSetup(mChildType, FirstObject)

      End If

    End Sub

    Protected Overrides ReadOnly Property RuleType As System.Type
      Get
        Return mChildType
      End Get
    End Property

    Friend Overrides Sub RenderModel(JW As Singular.Web.Utilities.JavaScriptWriter)

      If PreRenderModel(JW) Then

        ObjectMember.RenderModelProperty(JW, mPH, False, True, mClientOnlyProperty, mChildObjectName)

      End If

    End Sub

    Friend Overrides Sub RenderSchema(jw As JSonWriter)
      ObjectMember.RenderObjectSchema(JSonPropertyName, mMemberList, jw, mPH.CachedPropertyInfo.AutoGenerate)
    End Sub

    Public Overrides Sub UpdateModel(Dynamic As System.Dynamic.DynamicObject, Model As Object)

      'Get the Value From the JSon Object
      Dim ClientObject As System.Dynamic.DynamicObject = Nothing
      Dim ClientArray As DynamicJsonArray = Nothing

      'Get the Added / Changed Items
      If Model IsNot Nothing AndAlso Dynamic.TryGetMember(New MemberGetter(mPH.Name), ClientObject) AndAlso ClientObject IsNot Nothing Then
        Dim List As Object = mPH.PropertyInfo.GetValue(Model, Nothing)

        'Check if the object has a deleted list
        If TypeOf ClientObject Is DynamicJsonArray Then
          ClientArray = ClientObject
        Else
          If List Is Nothing Then
            List = Activator.CreateInstance(mPH.PropertyInfo.PropertyType)
            mPH.PropertyInfo.SetValue(Model, List, Nothing)
          End If

          Dim DeletedArray As DynamicJsonArray = Nothing
          ClientObject.TryGetMember(New MemberGetter("Items"), ClientArray)
          If ClientObject.TryGetMember(New MemberGetter("Deleted"), DeletedArray) Then

            For Each DeletedInfo As Object In DeletedArray
              Dim ObjToDelete As Object = mJSSerialiser.GetExistingObject(New Guid(CStr(DeletedInfo.Guid)))
              If ObjToDelete IsNot Nothing Then
                List.Remove(ObjToDelete)
              Else
                'Add and remove.

                ObjToDelete = CreateObject(mJSSerialiser, List, DeletedInfo.Guid)
                If DeletedInfo.ID IsNot Nothing Then
                  Dim KeyProperty As SimpleMember = mMemberList.GetKey
                  KeyProperty.DecryptKey(DeletedInfo.ID)
                  CType(ObjToDelete, ISingularBusinessBase).SetKey(DeletedInfo.ID)
                  'check if this is a full object or not
                  Dim IsFullObject As Boolean = False
                  DeletedInfo.TryGetMember(New MemberGetter("_IsFullObject"), IsFullObject)
                  If IsFullObject Then
                    ObjectMember.UpdateObjectFromClientObject(mJSSerialiser, mMemberList, DeletedInfo, ObjToDelete)
                  End If
                End If
                CType(ObjToDelete, ISingularBusinessBase).MarkOld()
                List.Remove(ObjToDelete)
              End If

            Next

          End If
        End If


        For Each DItem As DynamicJsonObject In ClientArray

          If List Is Nothing Then
            List = Activator.CreateInstance(mPH.PropertyInfo.PropertyType)
            mPH.PropertyInfo.SetValue(Model, List, Nothing)
          End If

          'Get the Index GUID
          Dim Guid As String = ""

          If DItem.TryGetMember(New MemberGetter("Guid"), Guid) Then

            'Get the Server Object
            Dim Obj As Object = Nothing
            If Guid IsNot Nothing Then
              Obj = mJSSerialiser.GetExistingObject(New Guid(Guid))
            End If

            Dim IsReference As Boolean
            DItem.TryGetMember(New MemberGetter("_Reference"), IsReference)

            If Obj Is Nothing Then
              'Create the Object

              Obj = CreateObject(mJSSerialiser, List, Guid)

            Else
              'the object might have existed somewhere other than in this list. 
              Dim Found As Boolean = False
              For Each item In List
                If item Is Obj Then
                  Found = True
                  Exit For
                End If
              Next
              'it wasn't in this list, so add it.
              If Not Found Then
                List.Add(Obj)
              End If
            End If

            If Obj IsNot Nothing AndAlso Not IsReference Then
              'Update the server object with the client objects properties.
              ObjectMember.UpdateObjectFromClientObject(mJSSerialiser, mMemberList, DItem, Obj)
            End If

          End If
        Next
      End If

    End Sub

    Friend Shared Function CreateObject(Serialiser As JSSerialiser, ContainerInstance As Object, ObjectGuid As String)

      'If ObjectID IsNot Nothing AndAlso JSSerialiser.ProtectKeyProperties Then
      '  If Not SimpleMember.DecryptKey(ObjectID, Singular.Reflection.GetLastGenericType(ContainerInstance.GetType), ObjectID) Then
      '    Throw New Exception("Invalid object key")
      '  End If
      'End If

      Dim AddNewMI As MethodInfo = ContainerInstance.GetType.GetMethod("AddNew", BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
      Dim NewObject As Object
      'Dim CSLAList As Csla.Core.IObservableBindingList = TryCast(ContainerInstance, Csla.Core.IObservableBindingList)
      If AddNewMI IsNot Nothing Then
        'If its a CSLA object, then make the list add the object to itself.
        NewObject = AddNewMI.Invoke(ContainerInstance, Nothing)
      Else
        'Otherwise create the object and add it to the list.
        Dim ChildType As Type = Singular.Reflection.GetLastGenericType(ContainerInstance.GetType)
        NewObject = Activator.CreateInstance(ChildType)
        CType(ContainerInstance, IList).Add(NewObject)
      End If
      'Add the object to the hashtable
      If TypeOf NewObject Is ISingularBusinessBase AndAlso ObjectGuid IsNot Nothing Then
        CType(NewObject, ISingularBase).Guid = New Guid(ObjectGuid)
      End If

      Serialiser.StoreObject(NewObject, ContainerInstance)

      Return NewObject

    End Function

    Friend Overrides Sub RenderData(JW As JSonWriter, Model As Object)

      Dim List = If(Model Is Nothing, Nothing, mPH.GetValue(Model))

      If CanRenderData(JW, List) Then


        Dim DeletedList As IList = Nothing
        Dim RenderDeletedList As Boolean = False
        If mJSSerialiser.RenderDeletedList AndAlso GetType(Singular.ISingularBusinessListBase).IsAssignableFrom(List.GetType) Then
          DeletedList = CType(List, Singular.ISingularBusinessListBase).GetDeletedList
          RenderDeletedList = DeletedList.Count > 0
        End If


        If RenderDeletedList Then
          'Need to render this list as an object with deleted list and item list.
          JW.StartClass(JSonPropertyName)
          JW.StartArray("Deleted")
          RenderListItems(JW, CType(List, Singular.ISingularBusinessListBase).GetDeletedList)
          JW.EndArray()
          JW.StartArray("Items")

        Else

          If JSonPropertyName <> "" Then
            JW.StartArray(JSonPropertyName)
          Else
            JW.StartArray()
          End If

        End If

        If List IsNot Nothing Then

          If mTypeAtRuntime.IsEnum Then

            Dim values As Array = System.Enum.GetValues(mTypeAtRuntime)
            For Each item As Object In values
              JW.StartObject()
              JW.WritePropertyName("Val")
              JW.WriteJSonValue(CInt(item))

              JW.WritePropertyName("Text")
              JW.WriteJSonValue(Singular.Reflection.GetEnumDisplayName(item))

              JW.EndObject()
            Next

          Else

            RenderListItems(JW, List)

          End If

        End If

        'Don't do this for each item in the list, just once per property.
        mJSSerialiser.OnRenderData(Me, List)
        'If TypeOf mJSSerialiser Is SessionVMJSSerialiser Then
        '  For Each m As Member In mMemberList
        '    If TypeOf m Is SimpleMember Then
        '      CType(m, SimpleMember).RenderDataStatic(List, mJSSerialiser)
        '    End If
        '  Next
        'End If

        JW.EndArray()
        If RenderDeletedList Then
          JW.EndClass()
        End If

      End If

    End Sub

    Private Sub RenderListItems(JW As JSonWriter, List As Object)

      Dim FirstRow As Boolean = True
      Dim IsSingularObject As Boolean = GetType(ISingularBase).IsAssignableFrom(mChildType)

      For Each Item As Object In List
        JW.StartObject()

        Dim lMemberList As MemberList = mMemberList
        If mHasVariableItemTypes Then
          Dim TypeInfo = Singular.ReflectionCached.GetCachedType(Item.GetType)
          lMemberList = TypeInfo.AdditionalInfo("JSMemberList")
          If lMemberList Is Nothing Then
            lMemberList = New MemberList
            FindMembers(Item.GetType, Item, False, lMemberList)
            TypeInfo.AdditionalInfo("JSMemberList") = lMemberList
          End If
        End If

        Dim RenderMembers As Boolean = True

        If JSSerialiser.RenderGuid Then
          JW.WritePropertyName("Guid")
          Dim soi As JSSerialiser.StoreObjectInfo = mJSSerialiser.StoreObject(Item, List)
          RenderMembers = Not soi.PreviouslyAdded
          JW.WriteJSonValue(soi.Guid)
        End If

        If RenderMembers Then
          Member.RenderMemberData(JW, Item, lMemberList)
        Else
          JW.WriteProperty("_Reference", True)
        End If

        If FirstRow Then
          FirstRow = False
          'Info row gets created for json array data that isnt going to be put into a view model.
          'E.g on the find screen.
          'Info array is added onto the first row of the list to give each columns display name and datatype.
          'Write the Display Names for each property.
          'If mJSSerialiser.RenderInfoArray Then
          ' RenderInfoArray(JW)
          'End If

          If mIsPagedList Then
            JW.WriteProperty("__TotalRows", CType(List, Singular.Paging.IPagedList).TotalRecords)
          End If

        End If

        If IsSingularObject AndAlso CType(Item, ISingularBase).LocalisationDataValues IsNot Nothing Then
          CType(Item, ISingularBase).LocalisationDataValues.WriteJSon(JW)
        End If

        JW.EndObject()
      Next

    End Sub

    Protected Friend Overrides Sub RenderStaticTypeInfo(TypeName As String, jw As Utilities.JavaScriptWriter)
      MyBase.RenderStaticTypeInfo(TypeName, jw)

      RenderDataLocalisationProperties(TypeName, ReflectionCached.GetCachedType(mChildType), jw)
    End Sub

  End Class

End Namespace
