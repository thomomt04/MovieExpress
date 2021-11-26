Imports System.Reflection

Imports System.ComponentModel.DataAnnotations
Imports Singular.DataAnnotations
#If SILVERLIGHT Then
Imports System.Windows.Data
#End If

Public Class Reflection

#If SILVERLIGHT Then

  Public Shared Function GetBusinessBasePropertiesList(Of C As SingularBusinessBase(Of C))(ExcludeRequiredFields As Boolean, ExcludeReadOnlyFields As Boolean) As List(Of PropertyItem)

    Return (From pi In GetType(C).GetProperties()
            Where (IncludeProperty(pi, ExcludeRequiredFields, ExcludeReadOnlyFields))
                                ).Select(Function(o) GetPropertyItem(o)).ToList

  End Function

  Public Shared Function GetReadOnlyBasePropertiesList(Of C As SingularReadOnlyBase(Of C))(ExcludeRequiredFields As Boolean, ExcludeReadOnlyFields As Boolean) As List(Of PropertyItem)

    Return (From pi In GetType(C).GetProperties()
            Where (IncludeProperty(pi, ExcludeRequiredFields, ExcludeReadOnlyFields))
                                ).Select(Function(o) GetPropertyItem(o)).ToList

  End Function

  Public Shared Function GetPropertiesList(OfType As Type, ExcludeRequiredFields As Boolean, ExcludeReadOnlyFields As Boolean) As List(Of PropertyItem)

    Return (From pi In OfType.GetProperties()
            Where (IncludeProperty(pi, ExcludeRequiredFields, ExcludeReadOnlyFields))
                                ).Select(Function(o) GetPropertyItem(o)).ToList

  End Function

  Public Class PropertyItem
    Inherits DependencyObject

    Public Property Name As String = ""
    Public Property DisplayName As String = ""
    Public Property Description As String = ""
    Public Property IsReadOnly As Boolean = False
    Public Property PropertyInfo As PropertyInfo

  End Class


  Public Shared Function GetPropertyItem(ByVal pi As PropertyInfo) As PropertyItem

    Dim pitem As New PropertyItem With {.Name = pi.Name, .DisplayName = pi.Name.Readable(), .IsReadOnly = Not pi.CanWrite, .PropertyInfo = pi}

    Dim da = Reflection.GetAttribute(Of System.ComponentModel.DataAnnotations.DisplayAttribute)(pi)

    If Not IncludeProperty(pi, da) Then
      Return Nothing
    End If

    If da IsNot Nothing Then
      pitem.DisplayName = da.Name
      pitem.Description = da.Description
    End If

    Return pitem

  End Function

  Private Shared Function IncludeProperty(ByVal pi As PropertyInfo, ExcludeRequiredFields As Boolean, ExcludeReadOnlyFields As Boolean) As Boolean

    Dim ra = Reflection.GetAttribute(Of System.ComponentModel.DataAnnotations.RequiredAttribute)(pi)

    If ra IsNot Nothing AndAlso ExcludeRequiredFields Then
      Return False
    Else
      If IncludeProperty(pi, Reflection.GetAttribute(Of System.ComponentModel.DataAnnotations.DisplayAttribute)(pi)) Then
        If ExcludeReadOnlyFields Then
          ' check if read only
          If pi.CanWrite Then
            If pi.GetSetMethod IsNot Nothing AndAlso pi.GetSetMethod.IsPublic Then
              Return True
            Else
              Return False
            End If
          Else
            Return False
          End If
        Else
          Return True
        End If
      Else
        Return False
      End If
    End If

  End Function

  Private Shared Function IncludeProperty(ByVal pi As PropertyInfo, ByVal da As DisplayAttribute) As Boolean

    If da IsNot Nothing Then
      If da.GetAutoGenerateField IsNot Nothing AndAlso Not da.GetAutoGenerateField Then
        Return False
      End If
    End If
    Return True

  End Function

#End If

  Public Shared Function GenerateField(ByVal PropertyInfo As PropertyInfo) As Boolean

    Dim da = GetDisplayAttribute(PropertyInfo)
    If da IsNot Nothing Then
      If da.GetAutoGenerateField IsNot Nothing Then
        Return da.GetAutoGenerateField
      End If
    End If

    Return PropertyInfo.CanRead

  End Function

  Public Shared Function GetFieldDescription(FieldName As String, DataItem As Object) As String

    If FieldName Is Nothing Then
      Return ""
    End If

    Dim pi = DataItem.GetType.GetProperty(FieldName)

    If pi IsNot Nothing Then
      Dim Description = GetFieldDescription(pi)

      If Description <> "" Then
        Return Description
      End If
    End If

    If Strings.Readable(DataItem.GetType.Name).ToLower = "table" Then
      Return Strings.Readable(FieldName)
    Else
      Return Strings.Readable(FieldName) & " of " & Strings.Readable(DataItem.GetType.Name)
    End If

  End Function

  Public Shared Function GetFieldDescription(ByVal pi As PropertyInfo) As String

    If pi IsNot Nothing Then
      If Attribute.IsDefined(pi, GetType(System.ComponentModel.DataAnnotations.DisplayAttribute)) Then
        Dim da As DisplayAttribute = Attribute.GetCustomAttribute(pi, GetType(System.ComponentModel.DataAnnotations.DisplayAttribute))

        If da IsNot Nothing Then
          If Not String.IsNullOrWhiteSpace(da.Description) Then
            ' we have a description
            Return da.Description
          End If
        End If
      End If
    End If

    Return ""

  End Function

  Public Shared Function GetOrder(ByVal PropertyInfo As PropertyInfo) As Integer

    Dim da = GetAttribute(Of DisplayAttribute)(PropertyInfo)
    If da IsNot Nothing AndAlso da.GetOrder IsNot Nothing Then
      Return da.GetOrder()
    Else
      Return 0
    End If

  End Function

  Public Shared Function GetDisplayName(ByVal FieldName As String, DataObject As Object) As String

    Dim pi = DataObject.GetType.GetProperty(FieldName, BindingFlags.Public + BindingFlags.Instance + BindingFlags.FlattenHierarchy)
    If pi IsNot Nothing Then
      Return GetDisplayName(pi)
    Else
      Return Strings.Readable(FieldName)
    End If

  End Function

  Public Shared Function GetDisplayName(ByVal PropertyInfo As PropertyInfo) As String

    Dim LocalisationKey As String = Nothing

#If SILVERLIGHT = False Then
    Dim lda = GetLocalisedDisplayAttribute(PropertyInfo)
    If lda IsNot Nothing Then
      LocalisationKey = lda.ResourceName
      Dim LocalText = Singular.Localisation.LocalText_DontReplaceMissing(LocalisationKey)
      If LocalText IsNot Nothing Then
        Return LocalText
      End If
    End If
#End If

    Dim da = GetAttribute(Of DisplayAttribute)(PropertyInfo)
    If da IsNot Nothing AndAlso da.Name IsNot Nothing AndAlso da.Name <> "" Then
      Return da.Name
    Else
#If SILVERLIGHT Then
      Return Strings.Readable(PropertyInfo.Name)
#Else
      Dim dna = GetAttribute(Of System.ComponentModel.DisplayNameAttribute)(PropertyInfo)
      If dna IsNot Nothing AndAlso dna.DisplayName IsNot Nothing AndAlso dna.DisplayName <> "" Then
        Return dna.DisplayName
      Else
        If LocalisationKey Is Nothing Then
          Return Strings.Readable(PropertyInfo.Name)
        Else
          Return LocalisationKey
        End If
      End If
#End If

    End If
  End Function

  ''' <summary>
  ''' Gets the description of the property from the properties attributes in the following order: LocalisedDisplayAttribute >  Display attribute
  ''' </summary>
  ''' <param name="PropertyInfo"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function GetDescription(ByVal PropertyInfo As PropertyInfo) As String

#If SILVERLIGHT Then
#Else
    Dim lda = GetLocalisedDisplayAttribute(PropertyInfo)
    If lda IsNot Nothing AndAlso Not String.IsNullOrEmpty(lda.DescriptionKey) Then

      Dim LocalText = Singular.Localisation.LocalText(lda.DescriptionKey)
      If LocalText IsNot Nothing Then
        Return LocalText
      End If
    End If
#End If

    Dim da = GetAttribute(Of DisplayAttribute)(PropertyInfo)
    If da IsNot Nothing Then
      Return da.Description
    End If

    Return String.Empty
  End Function

  ''' <summary>
  ''' Gets the Description of an Enum. First checks if there is a Description attribute on the enum item, otherwise returns the readable name.
  ''' </summary>
  Public Shared Function GetEnumDisplayName(ByVal EnumItem As Object) As String

    Dim ItemType As MemberInfo() = EnumItem.GetType.GetMember(EnumItem.ToString)
    If ItemType.Length = 1 Then

#If SILVERLIGHT = False Then
      Dim lda = GetAttribute(Of Singular.DataAnnotations.LocalisedDisplay)(ItemType(0))
      If lda IsNot Nothing Then
        Dim Desc = Singular.Localisation.LocalText(lda.ResourceName)
        If Desc IsNot Nothing Then
          Return Desc
        End If
      End If
#End If

      Dim da = GetAttribute(Of System.ComponentModel.DescriptionAttribute)(ItemType(0))
      If da IsNot Nothing Then
        Return da.Description
      End If

      Dim dat = GetAttribute(Of DisplayAttribute)(ItemType(0))
      If dat IsNot Nothing AndAlso dat.Name IsNot Nothing Then
        Return dat.Name
      End If

    End If

    Return Strings.Readable(EnumItem.ToString)

  End Function

  Public Class EnumItem
    Public Property Value As Object
    Public Property Display As String
    Public Property Description As String
    Friend Property SortIndex As Integer = 0
  End Class

  ''' <summary>
  ''' Returns an list of EnumItem with the values, display and descriptions (using the System.ComponentModel.Description attribute) of the enumeration.
  ''' Also sorts the list if the items have the Singular.DataAnnotations.Order attribute on them.
  ''' </summary>
  Public Shared Function GetEnumArray(ByVal EnumType As Type) As List(Of EnumItem)
    Dim ReturnList As New List(Of EnumItem)


    Dim values As Array = System.Enum.GetValues(EnumType)
    For Each item As Object In values

      Dim ei As New EnumItem
      ei.Value = item
      ei.Display = item.ToString
      ei.Description = Singular.Reflection.GetEnumDisplayName(item)

      Dim ItemType As MemberInfo() = item.GetType.GetMember(item.ToString)
      If ItemType.Length = 1 Then
        Dim da = GetAttribute(Of DisplayAttribute)(ItemType(0))
        If da IsNot Nothing AndAlso da.GetOrder IsNot Nothing Then
          ei.SortIndex = da.GetOrder
        End If
      End If

      ReturnList.Add(ei)

    Next

    ReturnList.Sort(Function(x, y)
                      Return x.SortIndex.CompareTo(y.SortIndex)
                    End Function)

    Return ReturnList
  End Function


  Public Shared Function GetDisplayAttribute(ByVal PropertyInfo As PropertyInfo) As DisplayAttribute

    Return GetAttribute(Of DisplayAttribute)(PropertyInfo)

  End Function

  ''' <summary>
  ''' Returns true if the Property Should be Displayed in the UI.
  ''' Checks the Display:AutoGenerateField, and Browsable Attributes. Differs from CanSerialiseField in that AutoGenerateField overrides browsable.
  ''' </summary>
  Public Shared Function AutoGenerateField(pi As PropertyInfo) As Boolean

    Dim da = Singular.Reflection.GetDisplayAttribute(pi)
    If da IsNot Nothing AndAlso da.GetAutoGenerateField IsNot Nothing AndAlso Not da.GetAutoGenerateField Then
      Return False
    End If

    'Browsable
    Dim ba = GetAttribute(Of System.ComponentModel.BrowsableAttribute)(pi)
    If ba IsNot Nothing Then
      Return ba.Browsable
    End If

    Return True

  End Function

  ''' <summary>
  ''' Returns true if the Property Should be serialised into other formats. Or e.g. if the property should be added to a generated javascript object.
  ''' Checks the Display:AutoGenerateField, and Browsable Attributes. Differs from AutoGenerateField in that browsable overrides AutoGenerateField.
  ''' </summary>
  Public Shared Function CanSerialiseField(pi As PropertyInfo) As Boolean

    If GetAttribute(Of Singular.DataAnnotations.MustSerialise)(pi) IsNot Nothing Then
      Return True
    End If

    'Browsable
    Dim ba = GetAttribute(Of System.ComponentModel.BrowsableAttribute)(pi)
    If ba IsNot Nothing Then
      'Browsable: True will override GetAutoGenerateField
      Return ba.Browsable
    End If

    Dim da = Singular.Reflection.GetDisplayAttribute(pi)
    If da IsNot Nothing AndAlso da.GetAutoGenerateField IsNot Nothing AndAlso Not da.GetAutoGenerateField Then
      Return False
    End If

    Return True

  End Function

  ''' <summary>
  ''' Tries to get the Default Value of a Property by:
  ''' a) Looking at the DefaultValueAttribute if there is one.
  ''' b) Looking at the CLSA.PropertyInfo Field attached to the Property.
  ''' c) Creating an Instance of the Object and Getting the initial Value, if CreateInstance is True.
  ''' </summary>
  ''' <remarks>B.Marlborough Apr 2012</remarks>
  Public Shared Function GetDefaultValue(pi As PropertyInfo, Optional CreateInstance As Boolean = False, Optional ByRef Instance As Object = Nothing) As Object

    Dim dfA As System.ComponentModel.DefaultValueAttribute = GetAttribute(Of System.ComponentModel.DefaultValueAttribute)(pi)
    If dfA IsNot Nothing Then
      Return dfA.Value
    End If

#If SILVERLIGHT = False Then

    Dim cpi As Csla.Core.IPropertyInfo = Singular.ReflectionCached.GetCachedMemberInfo(pi).BackingField
    If cpi IsNot Nothing AndAlso cpi.DefaultValue IsNot Nothing Then
      Return cpi.DefaultValue
    End If

#End If

    If CreateInstance Or Instance IsNot Nothing Then
      If Instance Is Nothing Then
        Instance = Activator.CreateInstance(pi.DeclaringType)
      End If
      Return pi.GetValue(Instance, Nothing)
    End If

    Return Nothing

  End Function

  Public Shared Function GetCategory(pi As PropertyInfo) As String

    Dim c = Singular.Reflection.GetAttribute(Of System.ComponentModel.CategoryAttribute)(pi)
    If c IsNot Nothing Then
      Return c.Category
    End If

    Dim da = Singular.Reflection.GetDisplayAttribute(pi)
    If da IsNot Nothing Then
      If String.IsNullOrEmpty(da.GroupName) Then
        Return da.GroupName
      End If
    End If

    Return ""

  End Function


  Public Shared Function GetLocalisedDisplayAttribute(ByVal PropertyInfo As PropertyInfo) As LocalisedDisplay

    Return GetAttribute(Of LocalisedDisplay)(PropertyInfo)

  End Function

  Public Shared Function IsDerivedFromGenericType(ByVal givenType As Type, ByVal genericType As Type) As Boolean

    If givenType.IsGenericType Then
      If givenType.GetGenericTypeDefinition.Equals(genericType) Then
        Return True
      End If
    End If

    Dim baseType As Type = givenType.BaseType
    If baseType Is Nothing Then
      Return False
    End If

    If baseType.IsGenericType Then
      If baseType.GetGenericTypeDefinition.Equals(genericType) Then
        Return True
      End If
    End If
    Return IsDerivedFromGenericType(baseType, genericType)

  End Function

#If SILVERLIGHT Then
#Else
  Public Shared Function GetListTypesObjectType(ListType As Type) As Type

    If Singular.Reflection.IsDerivedFromGenericType(ListType, GetType(System.ComponentModel.BindingList(Of ))) OrElse
        Singular.Reflection.IsDerivedFromGenericType(ListType, GetType(Collections.ObjectModel.ObservableCollection(Of ))) OrElse
        Singular.Reflection.IsDerivedFromGenericType(ListType, GetType(List(Of ))) Then

      If Singular.Reflection.GetGenericArgumentCount(ListType) = 2 Then
        ' we are dealing with a normal CSLA list
        Return Singular.Reflection.GetGenericArgumentType(ListType, 1)
      ElseIf Singular.Reflection.GetGenericArgumentCount(ListType) = 1 Then
        If Singular.Reflection.IsDerivedFromGenericType(ListType.BaseType, GetType(List(Of ))) Then
          Return Singular.Reflection.GetGenericArgumentType(ListType.BaseType, 0)
        Else
          Return Singular.Reflection.GetGenericArgumentType(ListType, 0)
        End If
      End If
    End If

    Return Nothing

  End Function
#End If

  Public Shared Function GetGenericBaseType(ByVal OfType As Type) As Type

    If OfType.GetGenericArguments.Length > 0 Then
      Return OfType
    Else
      If OfType.BaseType IsNot Nothing Then
        Return GetGenericBaseType(OfType.BaseType)
      Else
        Return Nothing
      End If
    End If
    Return Nothing

  End Function

#If Silverlight = False Then
  Private Shared mPropertyAttributeList As New Hashtable
#End If
  Public Class AttributeInfo
    Public Attribute As Attribute = Nothing
  End Class

  Public Shared Function GetAttribute(Of A As Attribute)(ByVal ClassType As Type) As A

    Dim att = CType(ClassType.GetCustomAttributes(GetType(A), True).FirstOrDefault, A)
    Return att

  End Function

  Public Shared Function GetAttribute(Of A As Attribute)(ByVal MemberInfo As MemberInfo) As A

#If Silverlight Then
    If Attribute.IsDefined(MemberInfo, GetType(A)) Then
      Return Attribute.GetCustomAttribute(MemberInfo, GetType(A))
    Else
      Return Nothing
    End If
#Else
    Dim key As String = MemberInfo.DeclaringType.FullName & MemberInfo.Name & GetType(A).FullName

    SyncLock mPropertyAttributeList
      If mPropertyAttributeList(key) Is Nothing Then

        Dim ai As New AttributeInfo
        mPropertyAttributeList(key) = ai

        If Attribute.IsDefined(MemberInfo, GetType(A)) Then
          'Try
          Dim attributes = Attribute.GetCustomAttributes(MemberInfo, GetType(A))
          If attributes.Count = 1 Then
            ai.Attribute = attributes(0)
          Else
            ai.Attribute = attributes.FirstOrDefault(Function(c) c.GetType.Equals(GetType(A)))
          End If

          'Catch ex As Exception
          '  ai.Attribute = GetAttributes(Of A)()
          'End Try

        End If
        Return ai.Attribute
      Else
        Return DirectCast(mPropertyAttributeList(key), AttributeInfo).Attribute
      End If
    End SyncLock
    
#End If
  End Function


  'Public Shared Function GetCustomAttributes(Of TAttribute As Attribute)(provider As ICustomAttributeProvider, Optional inherit As Boolean = False) As IEnumerable(Of TAttribute)
  '  Return provider.GetCustomAttributes(GetType(TAttribute), inherit).Cast(Of TAttribute)()
  'End Function

  Public Shared Function GetAttributes(Of A As Attribute)(ByVal Type As Type) As A()

    Return Type.GetCustomAttributes(GetType(A), True)

  End Function

  Public Shared Function GetGenericArgumentType(ByVal FromGenericType As Type, ByVal ArgumentIndex As Integer) As Type

    If FromGenericType.GetGenericArguments.Count = 0 Then
      Return GetGenericArgumentType(GetGenericBaseType(FromGenericType), ArgumentIndex)
    Else
      Return FromGenericType.GetGenericArguments()(ArgumentIndex)
    End If

  End Function

  ''' <summary>
  ''' Returns the no of generic arguments of the first generic type. Zero if the type, and none of its parents are not generic.
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function GetGenericArgumentCount(ByVal Type As Type) As Integer

    If Type.GetGenericArguments.Count = 0 Then
      If Type.BaseType Is Nothing Then
        Return 0
      Else
        Return GetGenericArgumentCount(Type.BaseType)
      End If
    Else
      Return Type.GetGenericArguments.Count
    End If

  End Function

  ''' <summary>
  ''' If the Type is generic, this will return the last generic type. E.g. if you want to know a 'List(of User)' contains objects of type 'User'.
  ''' </summary>
  Public Shared Function GetLastGenericType(ByVal Type As Type) As Type
    Dim Count As Integer = GetGenericArgumentCount(Type)
    If Count = 0 Then
      Return Type
    Else
      Return GetGenericArgumentType(Type, Count - 1)
    End If
  End Function

  Private Shared Function IsGenericEnumerable(ByVal t As Type) As Boolean
    Dim genArgs = t.GetGenericArguments()
    If genArgs.Length = 1 AndAlso GetType(IEnumerable(Of )).MakeGenericType(genArgs).IsAssignableFrom(t) Then
      Return True
    Else
      Return t.BaseType IsNot Nothing AndAlso IsGenericEnumerable(t.BaseType)
    End If
  End Function

  Public Shared Function IsBrowsable(ByVal PropertyInfo As PropertyInfo) As Boolean?

    Dim ba = GetAttribute(Of System.ComponentModel.BrowsableAttribute)(PropertyInfo)
    If ba Is Nothing Then
      Return Nothing
    Else
      Return ba.Browsable
    End If


  End Function

  Public Shared Function AutoGenerate(ByVal PropertyInfo As PropertyInfo) As Boolean?

    Dim da = GetAttribute(Of System.ComponentModel.DataAnnotations.DisplayAttribute)(PropertyInfo)
    If da Is Nothing Then
      Return Nothing
    Else
      Return da.GetAutoGenerateField
    End If

  End Function

  Public Shared Function ShouldDisplayField(ByVal PropertyInfo As PropertyInfo) As Boolean

    Dim Browsable = IsBrowsable(PropertyInfo)
    Dim AutoG = AutoGenerate(PropertyInfo)

    If Browsable Is Nothing AndAlso AutoG Is Nothing Then
      Return True
    ElseIf Browsable IsNot Nothing AndAlso AutoG Is Nothing Then
      Return Browsable
    ElseIf AutoG IsNot Nothing AndAlso Browsable Is Nothing Then
      Return AutoG
    Else
      ' both are not nothing
      Return Browsable OrElse AutoG
    End If

  End Function

  Public Shared Sub CopyCommonProperties(ByVal FromObject As Object, ByVal ToObject As Object, Optional ThrowExceptionsOnSet As Boolean = True)

    For Each PropertyMapping In (From pi1 In FromObject.GetType.GetProperties(BindingFlags.Instance + BindingFlags.Public)
                                 Join pi2 In ToObject.GetType.GetProperties(BindingFlags.Instance + BindingFlags.Public) On pi1.Name Equals pi2.Name
                                 Where pi2.CanWrite
                                 Select New With {
                                   .FromPi = pi1,
                                   .ToPi = pi2
                                 })

      Try
        Dim Value = PropertyMapping.FromPi.GetValue(FromObject, Nothing)
        PropertyMapping.ToPi.SetValue(ToObject, Value, Nothing)
      Catch ex As Exception
        If ThrowExceptionsOnSet Then
          Throw New Exception(String.Format("Error setting property '{0}' on object '{1}'", PropertyMapping.ToPi.Name, ToObject.GetType.Name), ex)
        End If
      End Try

    Next

  End Sub

  Public Shared Function TypeInheritsFromOrIsType(ByVal Type As Type, ByVal InheritsFromType As Type) As Boolean

    If Type Is Nothing Then Return False

    If Type.Equals(InheritsFromType) Then
      Return True
    ElseIf IsNothing(Type.BaseType) Then
      Return False
    Else
      Return TypeInheritsFromOrIsType(Type.BaseType, InheritsFromType)
    End If

  End Function

  Public Shared Function TypeImplementsInterface(ByVal type As Type, ByVal InterfaceType As Type) As Boolean

    If type.GetInterfaces.Contains(InterfaceType) Then
      Return True
    Else
      Return False
    End If

  End Function
#If SILVERLIGHT Then
#Else
  ''' <summary>
  ''' Gets the type name and assembly name without the culture and version info.
  ''' </summary>
  Public Shared Function GetTypeFullName(Type As Type) As String
    Return Type.FullName & ", " & Type.Assembly.GetName.Name
  End Function

#End If

#If SILVERLIGHT Then

	'Public Shared Function GetBindingExpressionLeafItem(be As BindingExpression) As Object


	'  Dim pi As PropertyInfo = be.GetType.GetProperty("LeafItem", BindingFlags.NonPublic + BindingFlags.Instance)
	'  If pi IsNot Nothing Then
	'    Return pi.GetValue(be, Nothing)
	'  Else
	'    Return Nothing
	'  End If

	'End Function

#End If


  Public Shared Function MethodMatchesParameters(ByVal Method As MethodInfo, ByVal Parameters() As Object, Optional ByVal MatchTypesToo As Boolean = False)

    If IsNothing(Method.GetParameters()) OrElse Method.GetParameters.Length = 0 Then
      If IsNothing(Parameters) Then
        Return True
      Else
        Return False
      End If
    Else
      If IsNothing(Parameters) Then
        Return Method.GetParameters.Length = 0
      Else
        If Method.GetParameters.Length = Parameters.Length Then
          If MatchTypesToo Then
            ' need to match the types
            For i As Integer = 0 To Method.GetParameters.Length - 1
              If Not Method.GetParameters(i).ParameterType Is Parameters(i).GetType And Method.GetParameters(i).ParameterType IsNot GetType(Object) And Parameters(i).GetType IsNot GetType(Object) Then
                Return False
              End If
            Next
            Return True
          Else
            Return True
          End If
        End If
      End If
    End If
    Return False

  End Function

  ''' <summary>
  ''' Returns true if the method has REQUIRED parameters. e.g. GetList(optional ID as Integer = 0) will return false.
  ''' </summary>
  ''' <param name="Method"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function MethodHasParameters(ByVal Method As MethodInfo) As Boolean
    For Each pi As ParameterInfo In Method.GetParameters
      If Not pi.IsOptional Then
        Return True
      End If
    Next
    Return False
  End Function

  ''' <summary>
  ''' Invokes a method, and will fill in optional parameters with the default values.
  ''' </summary>
  Public Shared Function InvokeMethod(ByVal Method As MethodInfo, ByVal Obj As Object) As Object
    Dim pis As ParameterInfo() = Method.GetParameters
    Dim Params(pis.Length - 1) As Object
    For i As Integer = 0 To Params.Length - 1
      If Not pis(i).IsOptional Then
        Throw New Exception("Singular.Reflection.InvokeMethod can only be called on a method with no, or only optional parameters.")
      End If
      Params(i) = pis(i).DefaultValue
    Next
    Return Method.Invoke(Obj, Params)
  End Function

  ''' <summary>
  ''' Gets the Property Info from the type that the list contains.
  ''' </summary>
  Public Shared Function GetPropertyFromList(ByVal FromListType As Type, ByVal Name As String) As PropertyInfo
    Dim GenericArguments As Integer = Singular.Reflection.GetGenericArgumentCount(FromListType)
    If GenericArguments > 0 Then
      Dim ChildType As Type = Singular.Reflection.GetGenericArgumentType(FromListType, GenericArguments - 1)
      Return ChildType.GetProperty(Name, BindingFlags.Public Or BindingFlags.Instance)
    End If
    Return Nothing
  End Function

  ''' <summary>
  ''' Gets the Property Info using the Public or Instance Binding.
  ''' </summary>
  Public Shared Function GetProperty(ByVal FromType As Type, ByVal Name As String) As PropertyInfo
    Return FromType.GetProperty(Name, BindingFlags.Public Or BindingFlags.Instance)
  End Function

  ''' <summary>
  ''' Gets the first Property Info where the property type matches the Given Type.
  ''' </summary>
  Public Shared Function GetProperty(ByVal FromType As Type, ByVal PropertyType As Type) As PropertyInfo
    For Each pi In FromType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
      If pi.PropertyType Is PropertyType Then
        Return pi
      End If
    Next
    Return Nothing
  End Function

  ''' <summary>
  ''' Gets a value from a property. If the value is null, an object of the property type is created, and the property is set with this value.
  ''' </summary>
  Public Shared Function GetPropertyValueWithSet(Instance As Object, PropertyName As String) As Object

    Dim pi = Instance.GetType.GetProperty(PropertyName, System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
    Dim Value = pi.GetValue(Instance, Nothing)
    If Value Is Nothing Then
      Value = Activator.CreateInstance(pi.PropertyType)
      pi.SetValue(Instance, Value, Nothing)
    End If
    Return Value

  End Function

#If SILVERLIGHT = False Then

  Private Shared mGetRegisteredPropertiesAccessor As MethodInfo
  Private Shared mHasMetaData As Boolean = True
  ''' <summary>
  ''' Returns all the Csla.IPropertyInfo's that were registered on this type.
  ''' </summary>
  Public Shared Function GetRegisteredProperties(ByVal FromType As Type) As List(Of Csla.Core.IPropertyInfo)

    Csla.Core.FieldManager.FieldDataManager.ForceStaticFieldInit(FromType)

    If mHasMetaData AndAlso mGetRegisteredPropertiesAccessor Is Nothing Then
      Dim t = Type.GetType("Csla.Core.FieldManager.PropertyInfoManager, Csla")
      If t Is Nothing Then
        mHasMetaData = False
      Else
        mGetRegisteredPropertiesAccessor = t.GetMethod("GetRegisteredProperties", BindingFlags.Public Or BindingFlags.Static)
      End If
    End If

    If mHasMetaData Then
      Return mGetRegisteredPropertiesAccessor.Invoke(Nothing, {FromType})
    Else
      Return New List(Of Csla.Core.IPropertyInfo)
    End If

  End Function

  ''' <summary>
  ''' Tries to create an instance of a type using the parameterless constructor.
  ''' Returns nothing if there is no parameterless constructor, or an error occured.
  ''' </summary>
  Public Shared Function TryCreateInstance(ByVal Type As Type) As Object

    Try
      If Type.IsClass AndAlso Not Type.IsAbstract Then
        'Check if the type has a parameter-less constructor.
        Dim Constructors = Type.GetConstructors(System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.NonPublic)
        For Each Constructor In Constructors
          If Constructor.GetParameters.Length = 0 Then
            Return Activator.CreateInstance(Type, True)
          End If
        Next
      End If
    Catch ex As Exception

    End Try

    Return Nothing

  End Function

  Public Shared Property UseOldFetchListMethod As Boolean = False

  ''' <summary>
  ''' Fetches the csla list with an empty criteria object. Note: Calls DataPortal.fetch(of type), not List.GetList
  ''' </summary>
  Public Shared Function FetchList(ByVal ListOrCriteriaType As Type, ByVal Criteria As Object) As Object

    Dim ListType As Type = Nothing
    Dim CriteriaType As Type = Nothing

    ResolveCriteriaType(ListOrCriteriaType, ListType, CriteriaType)

    Dim FetchListMi As MethodInfo

    If UseOldFetchListMethod Then
      FetchListMi = ListType.GetMethod("FetchList", BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy)
    Else
      Dim FetchMethod = GetType(Csla.DataPortal).GetMethod("Fetch", {GetType(Object)})
      FetchListMi = FetchMethod.MakeGenericMethod(ListType)
    End If

    If Criteria Is Nothing Then
      Criteria = Activator.CreateInstance(CriteriaType)
    End If

    If FetchListMi IsNot Nothing Then
      Return FetchListMi.Invoke(Nothing, {Criteria})
    End If
    Return Nothing

  End Function

  Public Shared Sub ResolveCriteriaType(ByVal ListOrCriteriaType As Type, ByRef ListType As Type, ByRef CriteriaType As Type)
    If ListOrCriteriaType IsNot Nothing Then
      If Singular.Reflection.IsDerivedFromGenericType(ListOrCriteriaType, GetType(Csla.CriteriaBase(Of ))) OrElse ListOrCriteriaType.Name.EndsWith("Criteria") Then
        ListType = ListOrCriteriaType.DeclaringType
        CriteriaType = ListOrCriteriaType
      Else
        ListType = ListOrCriteriaType
        CriteriaType = ListOrCriteriaType.GetNestedType("Criteria", BindingFlags.Public Or BindingFlags.NonPublic)
      End If
    End If
  End Sub

  ''' <summary>
  ''' Gets the Property Info from a Linq Expression.
  ''' </summary>
  Public Shared Function GetMember(Of t)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of t, Object))) As MemberInfo
    Return GetMemberSpecific(Of t, Object)(le)
  End Function

  Public Shared Function GetMemberSpecific(Of t, [return])(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of t, [return]))) As MemberInfo

    Return GetMember(le)

  End Function

  Private Shared Function GetMember(le As Expressions.LambdaExpression) As MemberInfo

    If le Is Nothing Then
      Return Nothing
    ElseIf TypeOf le.Body Is Expressions.MemberExpression Then
      Return CType(le.Body, Expressions.MemberExpression).Member
    ElseIf TypeOf le.Body Is Expressions.MethodCallExpression Then
      Dim mcExpr = CType(le.Body, Expressions.MethodCallExpression)
      Return mcExpr.Method
    ElseIf TypeOf le.Body Is Expressions.LambdaExpression Then
      Return GetMember(le.Body)
    Else

      Dim uExpr = CType(le.Body, Expressions.UnaryExpression)
      If TypeOf uExpr.Operand Is Expressions.MethodCallExpression Then
        Return CType(uExpr.Operand, Expressions.MethodCallExpression).Method
      ElseIf TypeOf uExpr.Operand Is Expressions.MemberExpression Then
        Return CType(uExpr.Operand, Expressions.MemberExpression).Member
      End If
    End If
    Return Nothing

  End Function

  Public Shared Function ConvertValueToType(ByVal Type As Type, ByVal ValueToConvert As Object) As Object

    Dim IsNullable As Boolean = Type.GetGenericArguments.Count = 1 AndAlso Type.Name.StartsWith("nullable", StringComparison.InvariantCultureIgnoreCase)

    If ValueToConvert Is Nothing OrElse ValueToConvert Is DBNull.Value OrElse (TypeOf ValueToConvert Is String AndAlso ValueToConvert = "") Then
      'If the value is nothing, then convert it to dbnull or zero depending on the property type.
      If Type Is GetType(Integer) OrElse Type Is GetType(Decimal) Then
        Return Convert.ChangeType(0, Type)
      ElseIf IsNullable Then
        Return Nothing
      ElseIf Type Is GetType(String) Then
        Return ""
      ElseIf Type Is GetType(Date) Then
        Return Date.MinValue
      ElseIf Type Is GetType(Boolean) Then
        Return False
      ElseIf Type.IsEnum Then
        Return 0
      ElseIf Type Is GetType(Guid) Then
        Return Guid.NewGuid()
      Else
        Return DBNull.Value
      End If
    End If

    If IsNullable Then
      'Emile Added this in as he needed to use a Nullable Guid in an object and it was breaking here.
      'P.S. Marlborough please check
      Type = GetGenericArgumentType(Type, 0)
      If Type Is GetType(Guid) Then
        If ValueToConvert Is Nothing Then Return ValueToConvert
        Return New Guid(CStr(ValueToConvert))
      ElseIf Type.IsEnum Then
        Return CInt(ValueToConvert)
      Else
        Return Convert.ChangeType(ValueToConvert, Type)
      End If
    ElseIf Type.IsEnum Then
      Return CInt(ValueToConvert) 'System.Enum.ToObject(Type, ValueToConvert)
    ElseIf Type Is GetType(Guid) Then
      Return New Guid(CStr(ValueToConvert))
    Else
      Return Convert.ChangeType(ValueToConvert, Type)
    End If

  End Function

  Public Class SMemberInfo

    Private mMemberInfo As MemberInfo
    Private mDataType As Type
    Private mIsInteger As Boolean

    Public Sub New(ByVal Mi As MemberInfo)
      mMemberInfo = Mi
    End Sub

    Public Sub New(ByVal DataType As Type)
      mDataType = DataType
    End Sub

    Public Enum MainType
      [String] = 1
      Number = 2
      [Date] = 3
      [Boolean] = 4
      Other = 5
    End Enum

    Private mDataTypeMain As MainType = -1
    Public ReadOnly Property DataTypeMain As MainType
      Get
        If mDataTypeMain = -1 Then

          Dim TypeToCheck As Type
          If mMemberInfo Is Nothing Then
            TypeToCheck = mDataType
          Else
            If mMemberInfo.MemberType = MemberTypes.Property Then
              TypeToCheck = CType(mMemberInfo, PropertyInfo).PropertyType
            ElseIf mMemberInfo.MemberType = MemberTypes.Method Then
              TypeToCheck = CType(mMemberInfo, MethodInfo).ReturnType
            Else
              TypeToCheck = mMemberInfo.ReflectedType
            End If
          End If

          Dim IsNullable As Boolean = TypeToCheck.GetGenericArguments.Count = 1 AndAlso TypeToCheck.Name.StartsWith("nullable", StringComparison.InvariantCultureIgnoreCase)

          If IsNullable Then
            TypeToCheck = GetGenericArgumentType(TypeToCheck, 0)
          End If

          If TypeToCheck Is GetType(String) Then
            mDataTypeMain = MainType.String
          ElseIf TypeToCheck Is GetType(Csla.SmartDate) Then
            mDataTypeMain = MainType.Date
          ElseIf TypeToCheck Is GetType(Date) Then
            mDataTypeMain = MainType.Date
          ElseIf TypeToCheck Is GetType(TimeSpan) Then
            mDataTypeMain = MainType.Date
          ElseIf TypeToCheck Is GetType(Byte) OrElse TypeToCheck Is GetType(Integer) OrElse TypeToCheck Is GetType(Long) Then
            mIsInteger = True
            mDataTypeMain = MainType.Number
          ElseIf TypeToCheck Is GetType(Decimal) OrElse TypeToCheck Is GetType(Double) Then
            mDataTypeMain = MainType.Number
          ElseIf TypeToCheck Is GetType(Boolean) Then
            mDataTypeMain = MainType.Boolean
          Else
            mDataTypeMain = MainType.Other
          End If

        End If
        Return mDataTypeMain
      End Get
    End Property

    Public ReadOnly Property MemberName As String
      Get
        Return mMemberInfo.Name
      End Get
    End Property

    Public ReadOnly Property IsInteger As Integer
      Get
        Return mIsInteger
      End Get
    End Property

  End Class

#End If

End Class

