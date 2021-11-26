Imports System.Reflection
Imports System.Reflection.Emit

Public Class ReflectionCached

  Private Shared TypeInfoList As New Dictionary(Of Type, TypeInfo)
  Private Shared Property MemberList As New Dictionary(Of MemberInfo, CachedMemberInfo)

  Public Enum SerialiseType
    Simple = 1
    TypedList = 2
    Array = 3
    ComplexObject = 4
    DataSet = 5
    DataTable = 6
    Enumeration = 7
    Dictionary = 8
  End Enum

  Public Class TypeInfo

    Public Property Type As Type
    Public Property ReadableName As String

    ''' <summary>
    ''' If this class has a serveronly attribute on it.
    ''' </summary>
    Public Property ServerOnly As Boolean = False

    ''' <summary>
    ''' If this type is a readonly collection
    ''' </summary>
    Public Property ReadOnlyList As Boolean = False

    ''' <summary>
    ''' If this type inherits from SingularBusinessBase. E.g. it can save data.
    ''' </summary>
    Public Property IsBusinessObject As Boolean = False

    Public Property IsPagedList As Boolean = False
    ''' <summary>
    ''' How many generic arguments this type has.
    ''' </summary>
    Public Property GenericArgumentCount As Integer = 0
    ''' <summary>
    ''' The last generic type, usefull to determine the type of items in a list.
    ''' </summary>
    Public Property LastGenericType As Type
    ''' <summary>
    ''' True if this is a collection of items that can be iterated.
    ''' </summary>
    Public Property IsEnumerable As Boolean
    ''' <summary>
    ''' The general type of data object this is.
    ''' </summary>
    Public Property SerialisedType As SerialiseType = SerialiseType.Simple
    Public Property DotNetTypeName As String
    Public Property KeyProperty As CachedMemberInfo
    Public Property ParentProperty As CachedMemberInfo

    ''' <summary>
    ''' The primary key of this type will be encrypted using this name as a salt. This is set to the type name by default.
    ''' </summary>
    Public Property ProtectedKeySalt As String

    Public Property RuleInstanceCreated As Boolean

    Private Property MemberNameList As New Dictionary(Of String, List(Of CachedMemberInfo))

    Private _ChildObjects As New List(Of CachedMemberInfo)
    Public ReadOnly Property ChildObjects As List(Of CachedMemberInfo)
      Get
        Return _ChildObjects
      End Get
    End Property

    Private _ChildLists As New List(Of CachedMemberInfo)
    Public ReadOnly Property ChildLists As List(Of CachedMemberInfo)
      Get
        Return _ChildLists
      End Get
    End Property

    ''' <summary>
    ''' Any info other parts of the library need to store about a type.
    ''' </summary>
    Public Property AdditionalInfo As New Hashtable

    Public Sub New(Type As Type)
      Me.Type = Type

      InitialiseType()

      GetAdditionalInfo()

      GetSerialisedType()

      If SerialisedType = SerialiseType.ComplexObject OrElse SerialisedType = SerialiseType.TypedList Then
        FindProperties()

        FindBackingFields()

        IsBusinessObject = GetType(Singular.ISingularBusinessBase).IsAssignableFrom(Type)

        For Each spiList In MemberNameList.Values
          For Each spi In spiList
            spi.Setup()
          Next

        Next
      End If

    End Sub

    Private Sub GetSerialisedType()

      If Type.IsEnum Then

        SerialisedType = SerialiseType.Enumeration

      ElseIf Type Is GetType(String) OrElse Type Is GetType(Csla.SmartDate) OrElse Type Is GetType(Date) OrElse Type Is GetType(Object) OrElse Type Is GetType(DBNull) OrElse
         Not (Type.IsClass OrElse Type.IsInterface) OrElse Type.BaseType Is GetType(System.ValueType) Then

        SerialisedType = SerialiseType.Simple

      ElseIf Type Is GetType(DataSet) Then
        SerialisedType = SerialiseType.DataSet
      ElseIf Type Is GetType(DataTable) Then
        SerialisedType = SerialiseType.DataTable
      ElseIf GetType(IDictionary).IsAssignableFrom(Type) Then
        SerialisedType = SerialiseType.Dictionary
      Else

        If IsEnumerable Then
          If LastGenericType Is Nothing OrElse ReflectionCached.GetCachedType(LastGenericType).SerialisedType = SerialiseType.Simple Then
            SerialisedType = SerialiseType.Array
          Else
            SerialisedType = SerialiseType.TypedList
          End If
        Else
          SerialisedType = SerialiseType.ComplexObject
        End If
      End If

    End Sub

    Private Sub GetAdditionalInfo()

      ReadableName = Singular.Strings.Readable(Type.Name)
      If ReadableName.StartsWith("RO ") Then
        ReadableName = ReadableName.Substring(3)
      End If

      DotNetTypeName = Singular.Reflection.GetTypeFullName(Type)
      'MD5HashCode = Singular.Encryption.GetStringHash(Type.FullName, Encryption.HashType.MD5)

      If Attribute.IsDefined(Type, GetType(Singular.DataAnnotations.ServerOnly)) Then
        ServerOnly = True
      End If

      Dim KeyAttr As Singular.DataAnnotations.ProtectedKeySalt = Type.GetCustomAttributes(GetType(Singular.DataAnnotations.ProtectedKeySalt), True).FirstOrDefault
      If KeyAttr IsNot Nothing Then
        ProtectedKeySalt = KeyAttr.Salt
      Else
        ProtectedKeySalt = Type.Name
      End If


      IsEnumerable = Type.GetInterfaces.Contains(GetType(IEnumerable))

      IsPagedList = Type.GetInterfaces.Contains(GetType(Singular.Paging.IPagedList))

      GenericArgumentCount = Singular.Reflection.GetGenericArgumentCount(Type)
      If GenericArgumentCount > 0 Then
        LastGenericType = Singular.Reflection.GetGenericArgumentType(Type, GenericArgumentCount - 1)

        If IsEnumerable AndAlso Singular.Reflection.TypeImplementsInterface(LastGenericType, GetType(Singular.ISingularReadOnlyBase)) Then
          ReadOnlyList = True
        End If
      End If

    End Sub

    Private Sub FindProperties()

      For Each mi As MemberInfo In Type.GetMembers(BindingFlags.Public Or BindingFlags.Instance)
        If TypeOf mi Is PropertyInfo OrElse (TypeOf mi Is MethodInfo AndAlso DirectCast(mi, MethodInfo).GetParameters.Length = 0 AndAlso Not (mi.Name.StartsWith("get_") OrElse mi.Name.StartsWith("set_"))) Then

          Try
            Dim spi As CachedMemberInfo
            If MemberList.ContainsKey(mi) Then
              spi = MemberList(mi)
            Else
              spi = New CachedMemberInfo(mi, Me)
              MemberList.Add(mi, spi)
            End If
            If spi.IsKey Then
              If KeyProperty IsNot Nothing Then
                Throw New Exception("Error on type: " & Type.Name & ". Only one property can have the Key attribute")
              Else
                KeyProperty = spi
              End If
            End If
            If spi.IsParentID Then
              If ParentProperty IsNot Nothing Then
                Throw New Exception("Error on type: " & Type.Name & ". Only one property can have the ParentID attribute")
              Else
                ParentProperty = spi
              End If
            End If

            If spi.IsProperty Then
              Dim pi = DirectCast(mi, PropertyInfo)

              If GetType(ISingularBase).IsAssignableFrom(pi.PropertyType) Then _ChildObjects.Add(spi)
              If GetType(ISingularListBase).IsAssignableFrom(pi.PropertyType) Then _ChildLists.Add(spi)
            End If


            'Add to the global member list.
            If Not MemberNameList.ContainsKey(mi.Name) Then
              MemberNameList.Add(mi.Name, New List(Of CachedMemberInfo) From {spi})
            Else
              MemberNameList(mi.Name).Add(spi)
            End If

          Catch ex As Exception
            Throw New Exception("Error adding member: " & mi.Name & ": " & ex.Message)
          End Try

        End If
      Next

    End Sub

    Private Sub FindBackingFields()

      Dim Found As Boolean = False

      'Types using the new csla objects will return all backing fields using this method.
      For Each pi As Csla.Core.IPropertyInfo In Singular.Reflection.GetRegisteredProperties(Me.Type)
        If MemberNameList.ContainsKey(pi.Name) Then
          For Each spi As CachedMemberInfo In MemberNameList(pi.Name)
            spi.SetBackingField(pi)
          Next
        End If

        Found = True
      Next

      'If nothing was returned, then call this static method on the type to get its SProperties.
      If Not Found Then
        Dim mi As MethodInfo = Type.GetMethod("GetRegisteredProperties", BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy)
        If mi IsNot Nothing Then
          Dim Properties As Hashtable = mi.Invoke(Nothing, Nothing)
          For Each key As String In Properties.Keys
            For Each spi As CachedMemberInfo In MemberNameList(key)
              spi.SetBackingField(Properties(key))
            Next
          Next
        End If
      End If

    End Sub

    Private Sub InitialiseType()

      'See if the type has a static initialisation method.
      Dim DummyMi As MethodInfo = Type.GetMethod("InitialisationDummy", BindingFlags.Static Or BindingFlags.NonPublic Or BindingFlags.FlattenHierarchy)
      If DummyMi IsNot Nothing Then
        DummyMi.Invoke(Nothing, Nothing)
      Else

        'If not, then see if there is a parameterless constructor and try create an instance of the type.
        Singular.Reflection.TryCreateInstance(Type)
      End If

    End Sub

    Private _TableName As String
    Public Function GetObjectTableName() As String
      If String.IsNullOrEmpty(_TableName) Then
        Dim Instance As Singular.ISingularBusinessBase = Activator.CreateInstance(Me.Type)
        _TableName = Instance.TableName
      End If
      Return _TableName
    End Function

  End Class

  Public Class CachedMemberInfo

    ''' <summary>
    ''' The Type this property belongs to, not the property return type
    ''' </summary>
    Public Property OnType As TypeInfo

    ''' <summary>
    ''' The System.Reflection.MemberInfo this is based on.
    ''' </summary>
    Public Property MemberInfo As MemberInfo

    ''' <summary>
    ''' True if this is a property, false if its a method.
    ''' </summary>
    Public Property IsProperty As Boolean

    ''' <summary>
    ''' The CSLA.PropertyInfo linked to this property.
    ''' </summary>
    Public Property BackingField As Csla.Core.IPropertyInfo

    ''' <summary>
    ''' Property name defined in code.
    ''' </summary>
    Public Property PropertyName As String

    ''' <summary>
    ''' Display name returned by Singular.Reflection.DisplayName
    ''' </summary>
    Public Property DisplayName As String

    ''' <summary>
    ''' Default value declared in a DefaultValue attribute, or in CSLA registerproperty method.
    ''' </summary>
    Public Property DefaultValue As Object

    ''' <summary>
    ''' True if this property defines Javascript code for its get accessor.
    ''' </summary>
    Public Property HasJSGetExpression As Boolean = False

    Public Property ClientOnly As Boolean? = Nothing
    Public Property ClientOnlyNoData As Boolean = False
    Public Property InitialDataOnly As Boolean = False
    'Public Property VMReadOnly As Boolean = False

    Public Property AutoGenerate As Boolean
    Public Property Browsable As Boolean
    Public Property BrowsableAllowedContext As String
    Public Property IsNullableType As Boolean
    Public Property Order As Integer

    Public Property MainDataType As Singular.Reflection.SMemberInfo.MainType
    Public Property MainDataTypeShortString As String = "s"

    Public Property DropDownWebAttribute As Singular.DataAnnotations.DropDownWeb
    Public IsKey As Boolean = False
    Public IsParentID As Boolean = False
    Private ProtectKey As Boolean = True

    Private mGetMethod As MethodInfo
    Private mComiledGetMethod As Func(Of Object, Object)
    Private mHasFastGet As Boolean = False

    Public Sub New(mi As MemberInfo, ti As TypeInfo)

      MemberInfo = mi
      IsProperty = TypeOf mi Is PropertyInfo
      PropertyName = MemberInfo.Name
      OnType = ti

      GetGetMethod()

      If IsProperty Then
        DisplayName = Singular.Reflection.GetDisplayName(MemberInfo)
        Dim co = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ClientOnly)(MemberInfo)
        If co IsNot Nothing Then
          ClientOnly = co.Value
          ClientOnlyNoData = co.DataOption = DataAnnotations.ClientOnly.DataOptionType.DontSendData
        End If
        Dim id = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.InitialDataOnly)(MemberInfo)
        If id IsNot Nothing Then
          InitialDataOnly = True
        End If
        'VMReadOnly = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.VMReadOnly)(MemberInfo) IsNot Nothing

        IsNullableType = Nullable.GetUnderlyingType(DirectCast(MemberInfo, PropertyInfo).PropertyType) IsNot Nothing

        AutoGenerate = Singular.Reflection.AutoGenerateField(MemberInfo)
        Browsable = Singular.Reflection.CanSerialiseField(MemberInfo)

        Dim bca As Singular.DataAnnotations.BrowsableConditional = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.BrowsableConditional)(MemberInfo)
        If bca IsNot Nothing Then
          BrowsableAllowedContext = bca.AllowedContext
        End If

        'Dim da = Singular.Reflection.GetAttribute(Of System.ComponentModel.DataAnnotations.DisplayAttribute)(MemberInfo)
        'AutoGenerateField = da Is Nothing OrElse da.GetAutoGenerateField Is Nothing OrElse da.GetAutoGenerateField
      End If

      For Each attr As Attribute In MemberInfo.GetCustomAttributes(True)
        If TypeOf attr Is Singular.DataAnnotations.DropDownWeb Then
          DropDownWebAttribute = attr
        End If
        If TypeOf attr Is System.ComponentModel.DataAnnotations.KeyAttribute Then
          'IDPropertyAttribute = attr
          IsKey = True
        End If

        If TypeOf attr Is System.ComponentModel.DataAnnotations.DisplayAttribute Then Order = If(CType(attr, System.ComponentModel.DataAnnotations.DisplayAttribute).GetOrder, 0)

        If TypeOf attr Is Singular.DataAnnotations.ParentID Then IsParentID = True

        If TypeOf attr Is Singular.DataAnnotations.UnProtectedKey Then ProtectKey = False

        If TypeOf attr Is Singular.DataAnnotations.ComputedProperty Then CreateBackingField(attr)

      Next


      GetMainDataType()
      GetCompiledGetMethod()

    End Sub

    Private Sub GetGetMethod()

      If TypeOf MemberInfo Is PropertyInfo Then
        If CType(MemberInfo, PropertyInfo).CanRead Then
          mGetMethod = CType(MemberInfo, PropertyInfo).GetGetMethod(True)
        End If
      Else
        mGetMethod = MemberInfo
      End If

    End Sub

    Private Sub GetCompiledGetMethod()

      If mGetMethod IsNot Nothing AndAlso Not mGetMethod.ReturnType Is GetType(System.Void) AndAlso Not mGetMethod.IsGenericMethod Then

        Dim DMethod As New DynamicMethod("Get" & PropertyName, GetType(Object), {GetType(Object)}, True)
        Dim ILGenerator As ILGenerator = DMethod.GetILGenerator

        'Push instance
        ILGenerator.Emit(OpCodes.Ldarg_0)
        If MemberInfo.DeclaringType.IsValueType Then
          ILGenerator.Emit(OpCodes.Unbox, MemberInfo.DeclaringType)
        Else
          ILGenerator.Emit(OpCodes.Castclass, MemberInfo.DeclaringType)
        End If

        'Call method
        If (mGetMethod.IsFinal OrElse Not mGetMethod.IsVirtual) Then
          ILGenerator.Emit(OpCodes.Call, mGetMethod)
        Else
          ILGenerator.Emit(OpCodes.Callvirt, mGetMethod)
        End If

        'Return
        If (mGetMethod.ReturnType.IsValueType()) Then
          ILGenerator.Emit(OpCodes.Box, mGetMethod.ReturnType)
        Else
          ILGenerator.Emit(OpCodes.Castclass, mGetMethod.ReturnType)
        End If
        ILGenerator.Emit(OpCodes.Ret)

        mComiledGetMethod = DMethod.CreateDelegate(GetType(Func(Of Object, Object)))
        mHasFastGet = True

      End If

    End Sub

    Private Sub GetMainDataType()

      Dim s As New Singular.Reflection.SMemberInfo(MemberInfo)
      MainDataType = s.DataTypeMain
      Select Case MainDataType
        Case Reflection.SMemberInfo.MainType.Number
          MainDataTypeShortString = "n"
        Case Reflection.SMemberInfo.MainType.Boolean
          MainDataTypeShortString = "b"
        Case Reflection.SMemberInfo.MainType.Date
          MainDataTypeShortString = "d"
      End Select
    End Sub

    Private Sub CreateBackingField(attr As Singular.DataAnnotations.ComputedProperty)

      Dim spi As New Singular.SPropertyInfo(Of Object, Object)(PropertyName, DefaultValue)
      If String.IsNullOrEmpty(attr.JavascriptStatement) Then

        Dim decom As New DelegateDecompiler.MethodBodyDecompiler(mGetMethod)
        Dim expr = decom.Decompile
        spi.GetExpression(expr)

      Else
        spi.GetExpression(attr.JavascriptStatement)
      End If
      SetBackingField(spi)

    End Sub

    Friend Sub SetBackingField(BackingField As Csla.Core.IPropertyInfo)
      If BackingField IsNot Nothing Then
        Me.BackingField = BackingField

        Me.HasJSGetExpression = TypeOf BackingField Is Singular.ISingularPropertyInfo AndAlso DirectCast(BackingField, Singular.ISingularPropertyInfo).HasGetExpression
      End If
    End Sub

    Friend Sub Setup()
      If IsProperty Then
        GetDefaultValue()
      End If
    End Sub

    Private Sub GetDefaultValue()

      Dim dfA As System.ComponentModel.DefaultValueAttribute = Singular.Reflection.GetAttribute(Of System.ComponentModel.DefaultValueAttribute)(MemberInfo)
      If dfA IsNot Nothing Then
        DefaultValue = dfA.Value

      ElseIf BackingField IsNot Nothing AndAlso BackingField.DefaultValue IsNot Nothing Then

        DefaultValue = BackingField.DefaultValue

      End If

    End Sub

    Public Function GetValueFast(Instance As Object) As Object

      If mHasFastGet Then
        Return mComiledGetMethod(Instance)
      Else
        If IsProperty Then
          Return DirectCast(MemberInfo, PropertyInfo).GetValue(Instance, Nothing)
        Else
          Return DirectCast(MemberInfo, MethodInfo).Invoke(Instance, Nothing)
        End If
      End If
    End Function

    Public Function IsProtectedKey() As Boolean
      Return IsKey AndAlso ProtectKey
    End Function

  End Class

  Private Shared LockObject As New Object

  Public Shared Function GetCachedType(Type As Type) As TypeInfo

    SyncLock (LockObject)

      Dim TI As TypeInfo
      If TypeInfoList.ContainsKey(Type) Then
        TI = TypeInfoList(Type)
      Else
        SyncLock (LockObject)
          If TypeInfoList.ContainsKey(Type) Then
            TI = TypeInfoList(Type)
          Else
            TI = New TypeInfo(Type)
            TypeInfoList(Type) = TI
          End If
        End SyncLock

      End If
      Return TI

    End SyncLock

  End Function

  Public Shared Function GetCachedType(TypeName As String) As TypeInfo
    Return GetCachedType(Type.GetType(TypeName))
  End Function

  Public Shared Function GetCachedMemberInfo(mi As System.Reflection.MemberInfo) As CachedMemberInfo

    If MemberList.ContainsKey(mi) Then
      Return MemberList(mi)
    End If
    'Dim Temp = MemberList.Where(Function(c) c.Key.DeclaringType = mi.DeclaringType).ToList
    Dim TI As TypeInfo = GetCachedType(mi.ReflectedType)

    If MemberList.ContainsKey(mi) Then
      Return MemberList(mi)
    End If

    Return Nothing

  End Function

  Public Shared Function GetDisplayName(ByVal PropertyInfo As PropertyInfo) As String
    Return GetCachedMemberInfo(PropertyInfo).DisplayName
  End Function

  Public Shared Function AutoGenerateField(PropertyInfo As PropertyInfo) As Boolean

    Dim cmi = GetCachedMemberInfo(PropertyInfo)
    Return cmi.AutoGenerate

  End Function

  Public Shared Function SerlialiseField(PropertyInfo As PropertyInfo, Optional IncludeIDProperty As Boolean = False, Optional IncludeIsDirty As Boolean = False)
    Dim cmi = GetCachedMemberInfo(PropertyInfo)

    'Include ID property
    If IncludeIDProperty AndAlso cmi.IsKey Then
      Return True
    End If

    'Special cases.
    If cmi.PropertyName = "IsNew" OrElse (IncludeIsDirty AndAlso cmi.PropertyName = "IsSelfDirty") Then
      Return True
    End If

    'Normal
    If cmi.HasJSGetExpression Then
      Return True
    Else
      Return cmi.Browsable
    End If
  End Function

  Public Shared Function ServerOnlyType(type As Type) As Boolean
    Return GetCachedType(type).ServerOnly
  End Function

End Class
