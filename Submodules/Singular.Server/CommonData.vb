Imports System.Reflection
Imports System.ComponentModel
Imports System.Collections

Namespace CommonData

  <Serializable>
  Public Class CachedData
    Public Data As Object
    Public JSon As String
    Public JSonHash As String
  End Class

  Public Class Enums

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Perform the <B>Description</B> operation on this object.
    ''' </summary>
    ''' <param name="value">The enumeration value for which the Description is required.</param>
    ''' <returns>
    ''' The <see cref="System.ComponentModel.DescriptionAttribute.Description">DescriptionAttribute.Description</see> 
    ''' associated with the specified <paramref name="value"/>.  If the specified <paramref name="value"/> has no 
    ''' <see cref="System.ComponentModel.DescriptionAttribute">DescriptionAttribute</see> attribute
    ''' then the <see cref="System.Enum.ToString">System.Enum.ToString</see> value is returned.
    ''' </returns>
    ''' -----------------------------------------------------------------------------
    Public Shared Function Description(ByVal value As Object) As String

      Dim Info As FieldInfo = value.GetType.GetField(value.ToString)

      ' If the value is not valid for the enumeration then return '(None)'.
      If Info Is Nothing Then Return "Enum Description Not Located."

      Dim Attributes As DescriptionAttribute() = DirectCast(Info.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

      If Attributes.Length > 0 Then
        Return Attributes(0).Description
      Else
        Return Singular.Strings.Readable(value.ToString.Replace("_", ""))
      End If

    End Function

    Public Shared Function GetEnumAttribute(Of T As Attribute)(ByVal value As Object) As T

      Dim Info As FieldInfo = value.GetType.GetField(value.ToString)

      If Info Is Nothing Then
        Throw New Exception(String.Format("{0} is not a valid enum", value.ToString()))
      End If

      Dim att() As T = Info.GetCustomAttributes(GetType(T), False)
      If att.Length > 0 Then
        Return att(0)
      Else
        Return Nothing
      End If

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Perform the <B>GetEnumList</B> operation on this object.
    ''' </summary>
    ''' <param name="categories"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------------
    Public Shared Function GetStronglyTypedEnumList(Of T)(Optional ByVal categories As String = "") As EnumList(Of T)

      Dim enumType = GetType(T)

      Dim InfoCollection As FieldInfo() = enumType.GetFields

      ' create the generic type
      Dim Values = Activator.CreateInstance(GetType(EnumList(Of )).MakeGenericType(enumType))

      Dim EnumItemType = GetType(EnumItem(Of )).MakeGenericType(enumType)

      Dim DescAttributes As DescriptionAttribute()
      Dim CatAttributes As CategoryAttribute()
      Dim Value As [Enum]

      For Each Info As FieldInfo In InfoCollection
        If Info.FieldType Is enumType Then
          ' Set the category to valid if no categories were specified.
          Dim CatValid As Boolean = (categories = String.Empty)

          ' Get the description custom attributes attributes.
          DescAttributes = DirectCast(Info.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

          ' Check if the category is valid.
          If Not CatValid Then
            ' Get the category custom attributes.
            CatAttributes = DirectCast(Info.GetCustomAttributes(GetType(CategoryAttribute), False), CategoryAttribute())

            ' Set the category to valid if the specified category matches.
            If CatAttributes.Length > 0 Then CatValid = (CatAttributes(0).Category.ToLower = categories.ToLower)
          End If

          ' Only add the item to the list if it doesn't already exist and it's valid for the categegory.
          Value = CType(Info.GetValue(Info), [Enum])
          If (Not Values.Contains(Value)) And CatValid Then
            If DescAttributes.Length > 0 Then
              Values.Add(Activator.CreateInstance(EnumItemType, {DescAttributes(0).Description, Info.GetValue(Info)}))
            Else
              Values.Add(Activator.CreateInstance(EnumItemType, {Info.Name, Info.GetValue(Info)}))
            End If
          End If
        End If
      Next

      Return Values

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Perform the <B>GetEnumList</B> operation on this object.
    ''' </summary>
    ''' <param name="enumType"></param>
    ''' <param name="categories"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------------
    Public Shared Function GetStronglyTypedEnumList(ByVal enumType As Type, Optional ByVal categories As String = "") As IList

      Dim InfoCollection As FieldInfo() = enumType.GetFields

      ' create the generic type
      Dim Values = Activator.CreateInstance(GetType(EnumList(Of )).MakeGenericType(enumType))

      Dim EnumItemType = GetType(EnumItem(Of )).MakeGenericType(enumType)

      Dim DescAttributes As DescriptionAttribute()
      Dim CatAttributes As CategoryAttribute()
      Dim Value As [Enum]

      For Each Info As FieldInfo In InfoCollection
        If Info.FieldType Is enumType Then
          ' Set the category to valid if no categories were specified.
          Dim CatValid As Boolean = (categories = String.Empty)

          ' Get the description custom attributes attributes.
          DescAttributes = DirectCast(Info.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

          ' Check if the category is valid.
          If Not CatValid Then
            ' Get the category custom attributes.
            CatAttributes = DirectCast(Info.GetCustomAttributes(GetType(CategoryAttribute), False), CategoryAttribute())

            ' Set the category to valid if the specified category matches.
            If CatAttributes.Length > 0 Then CatValid = (CatAttributes(0).Category.ToLower = categories.ToLower)
          End If

          ' Only add the item to the list if it doesn't already exist and it's valid for the categegory.
          Value = CType(Info.GetValue(Info), [Enum])
          If (Not Values.Contains(Value)) And CatValid Then
            If DescAttributes.Length > 0 Then
              Values.Add(Activator.CreateInstance(EnumItemType, {DescAttributes(0).Description, Info.GetValue(Info)}))
            Else
              Values.Add(Activator.CreateInstance(EnumItemType, {Info.Name, Info.GetValue(Info)}))
            End If
          End If
        End If
      Next

      Return Values

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Perform the <B>GetEnumList</B> operation on this object.
    ''' </summary>
    ''' <param name="enumType"></param>
    ''' <param name="categories"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------------
    Public Shared Function GetEnumList(ByVal enumType As Type, Optional ByVal categories As String = "") As EnumList

      Dim InfoCollection As FieldInfo() = enumType.GetFields
      Dim Values As New EnumList
      Dim DescAttributes As DescriptionAttribute()
      Dim CatAttributes As CategoryAttribute()
      Dim Value As [Enum]

      For Each Info As FieldInfo In InfoCollection
        If Info.FieldType Is enumType Then
          ' Set the category to valid if no categories were specified.
          Dim CatValid As Boolean = (categories = String.Empty)

          ' Get the description custom attributes attributes.
          DescAttributes = DirectCast(Info.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

          ' Check if the category is valid.
          If Not CatValid Then
            ' Get the category custom attributes.
            CatAttributes = DirectCast(Info.GetCustomAttributes(GetType(CategoryAttribute), False), CategoryAttribute())

            ' Set the category to valid if the specified category matches.
            If CatAttributes.Length > 0 Then CatValid = (CatAttributes(0).Category.ToLower = categories.ToLower)
          End If

          ' Only add the item to the list if it doesn't already exist and it's valid for the categegory.
          Value = CType(Info.GetValue(Info), [Enum])
          If (Not Values.Contains(Value)) And CatValid Then
            If DescAttributes.Length > 0 Then
              Values.Add(New EnumItem(DescAttributes(0).Description, CType(Info.GetValue(Info), Integer)))
            Else
              Values.Add(New EnumItem(Info.Name, CType(Info.GetValue(Info), Integer)))
            End If
          End If
        End If
      Next

      Return Values

    End Function

    ''' <summary>
    ''' Provides access to all <B>EnumItem</B> business logic.
    ''' </summary>
    Public Class EnumItem(Of T)

      ''' <summary>
      ''' Perform the <B>New</B> operation on this object.
      ''' </summary>
      ''' <param name="displayMember"></param>
      ''' <param name="valueMember"></param>
      Public Sub New(ByVal displayMember As String, ByVal valueMember As T)

        Me.mDisplayMember = displayMember
        Me.mValueMember = valueMember

      End Sub

      Private mDisplayMember As String

      ''' <summary>
      ''' Returns the <B>DisplayMember</B> attribute for this object.
      ''' </summary>
      ''' <value></value>
      Public ReadOnly Property DisplayMember() As String
        Get
          Return mDisplayMember
        End Get
      End Property

      Private mValueMember As T

      ''' <summary>
      ''' Returns the <B>ValueMember</B> attribute for this object.
      ''' </summary>
      ''' <value></value>
      ''' 
      <ComponentModel.DataAnnotations.Key()>
      Public ReadOnly Property ValueMember() As T
        Get
          Return mValueMember
        End Get
      End Property
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Singular
    ''' Class	 : EnumList
    ''' Name	 : Singular.EnumList
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Provides access to all <B>EnumList</B> business logic.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Class EnumList(Of T)
      Inherits List(Of EnumItem(Of T))

      Public Const DisplayMember As String = "DisplayMember"
      Public Const ValueMember As String = "ValueMember"

      Public Overloads Function Contains(ByVal Value As System.Enum) As Boolean

        For Each ei As EnumItem(Of T) In Me
          If ei.ValueMember.Equals(Value) Then
            Return True
          End If
        Next
        Return False

      End Function

#Region " Sorter "

      ''' -----------------------------------------------------------------------------
      ''' Project	 : Singular
      ''' Class	 : Sorter
      ''' Name	 : Singular.EnumList.Sorter
      ''' -----------------------------------------------------------------------------
      ''' <summary>
      ''' Provides access to all <B>Sorter</B> business logic.
      ''' </summary>
      ''' -----------------------------------------------------------------------------
      Public Class Sorter
        Implements IComparer

        ''' -----------------------------------------------------------------------------
        ''' Project	 : Singular
        ''' Type	 : Key
        ''' Name	 : Singular.EnumList.Sorter.Key
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Enumeration of <B>Key</B> values.
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Enum Key
          DisplayMember
          ValueMember
        End Enum

        Private mDescending As Boolean
        Private mKey As Key

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Perform the <B>Compare</B> operation on this object.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------------
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

          Dim xEnumItem As EnumItem
          Dim yEnumItem As EnumItem

          If mDescending Then
            ' Descending, so reverse the items.
            xEnumItem = DirectCast(y, EnumItem)
            yEnumItem = DirectCast(x, EnumItem)
          Else
            ' Ascending.
            xEnumItem = DirectCast(x, EnumItem)
            yEnumItem = DirectCast(y, EnumItem)
          End If

          Select Case mKey
            Case Key.DisplayMember
              Return xEnumItem.DisplayMember.ToLower.CompareTo(yEnumItem.DisplayMember.ToLower)

            Case Key.ValueMember
              Return xEnumItem.ValueMember.CompareTo(yEnumItem.ValueMember)

            Case Else
              Return 0
          End Select

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Perform the <B>New</B> operation on this object.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="descending"></param>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal value As Key, ByVal descending As Boolean)
          mKey = value
          mDescending = descending
        End Sub

      End Class

#End Region

    End Class

    ''' <summary>
    ''' Provides access to all <B>EnumItem</B> business logic.
    ''' </summary>
    Public Class EnumItem

      ''' <summary>
      ''' Perform the <B>New</B> operation on this object.
      ''' </summary>
      ''' <param name="displayMember"></param>
      ''' <param name="valueMember"></param>
      Public Sub New(ByVal displayMember As String, ByVal valueMember As Integer)

        Me.mDisplayMember = displayMember
        Me.mValueMember = valueMember

      End Sub

      Public Overrides Function ToString() As String
        Return Me.ValueMember.ToString() & ": " & Me.DisplayMember
      End Function

      Private mDisplayMember As String

      ''' <summary>
      ''' Returns the <B>DisplayMember</B> attribute for this object.
      ''' </summary>
      ''' <value></value>
      Public Property DisplayMember() As String
        Get
          Return mDisplayMember
        End Get
        Set(value As String)
          mDisplayMember = value
        End Set
      End Property

      Private mValueMember As Integer

      ''' <summary>
      ''' Returns the <B>ValueMember</B> attribute for this object.
      ''' </summary>
      ''' <value></value>
      Public ReadOnly Property ValueMember() As Integer
        Get
          Return mValueMember
        End Get
      End Property
    End Class



    ''' -----------------------------------------------------------------------------
    ''' Project	 : Singular
    ''' Class	 : EnumList
    ''' Name	 : Singular.EnumList
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Provides access to all <B>EnumList</B> business logic.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Class EnumList
      Inherits List(Of EnumItem)

      Public Const DisplayMember As String = "DisplayMember"
      Public Const ValueMember As String = "ValueMember"

      Public Overloads Function Contains(ByVal Value As System.Enum) As Boolean

        For Each ei As EnumItem In Me
          If ei.ValueMember.Equals(Value) Then
            Return True
          End If
        Next
        Return False

      End Function

#Region " Sorter "

      ''' -----------------------------------------------------------------------------
      ''' Project	 : Singular
      ''' Class	 : Sorter
      ''' Name	 : Singular.EnumList.Sorter
      ''' -----------------------------------------------------------------------------
      ''' <summary>
      ''' Provides access to all <B>Sorter</B> business logic.
      ''' </summary>
      ''' -----------------------------------------------------------------------------
      Public Class Sorter
        Implements IComparer

        ''' -----------------------------------------------------------------------------
        ''' Project	 : Singular
        ''' Type	 : Key
        ''' Name	 : Singular.EnumList.Sorter.Key
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Enumeration of <B>Key</B> values.
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Enum Key
          DisplayMember
          ValueMember
        End Enum

        Private mDescending As Boolean
        Private mKey As Key

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Perform the <B>Compare</B> operation on this object.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------------
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

          Dim xEnumItem As EnumItem
          Dim yEnumItem As EnumItem

          If mDescending Then
            ' Descending, so reverse the items.
            xEnumItem = DirectCast(y, EnumItem)
            yEnumItem = DirectCast(x, EnumItem)
          Else
            ' Ascending.
            xEnumItem = DirectCast(x, EnumItem)
            yEnumItem = DirectCast(y, EnumItem)
          End If

          Select Case mKey
            Case Key.DisplayMember
              Return xEnumItem.DisplayMember.ToLower.CompareTo(yEnumItem.DisplayMember.ToLower)

            Case Key.ValueMember
              Return xEnumItem.ValueMember.CompareTo(yEnumItem.ValueMember)

            Case Else
              Return 0
          End Select

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Perform the <B>New</B> operation on this object.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="descending"></param>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal value As Key, ByVal descending As Boolean)
          mKey = value
          mDescending = descending
        End Sub

      End Class

#End Region

    End Class

  End Class

  Public Module Misc

    Public Function GetListProperty(Of T)() As T

      ' see if we can find a property of the same type in common data
      If Lists IsNot Nothing Then
        Dim piCommonData = Lists.GetType.GetProperties(BindingFlags.Public + BindingFlags.Instance).FirstOrDefault(Function(picd) picd.PropertyType.Equals(GetType(T)))

        If piCommonData IsNot Nothing Then
          ' great, there is a CommonData property

          Return piCommonData.GetValue(Lists, Nothing)
        Else
          Throw New Exception("No CommonData property found for type " & GetType(T).Name)
        End If
      Else
        Throw New Exception("CommonData not initialised, set the Singular.CommonData.Lists property")
      End If

    End Function

    Public Const CommonDataSessionKey As String = "Singular.CommonData.SessionLists"

    Public Enum ContextType
      Application = 1
      Session = 2
      Temporary = 4
      Any = Application Or Session Or Temporary
      Localised = 8
      Application_Localised = Application Or Localised
      Session_Localised = Session Or Localised
    End Enum

    Public Enum ContextDisallowedModeType
      Exception = 1
      ReturnNull = 2
      PreviousValue = 3
    End Enum

    Public Property Lists As ICommonDataLists
    Public Property TempLists As ICommonDataLists

    Private GetListsMethod As MethodInfo = Nothing
    Private GetListPropertysMethod As MethodInfo = Nothing
    Private SetListPropertysMethod As MethodInfo = Nothing

    Public Function GetListProperty(ByVal OfType As Type) As Csla.Core.IPropertyInfo

      If Lists IsNot Nothing Then

        If GetListPropertysMethod Is Nothing Then
          GetListPropertysMethod = Lists.GetType.GetMethod("GetListProperty")
        End If

        If GetListPropertysMethod IsNot Nothing Then
          Return GetListPropertysMethod.Invoke(Lists, New Object() {OfType})
        End If
      End If

      Return Nothing

    End Function

    Public Sub SetListProperty(ByVal OfType As Type, ByVal Value As Object)

      If Lists IsNot Nothing Then

        If SetListPropertysMethod Is Nothing Then
          SetListPropertysMethod = Lists.GetType.GetMethod("SetListProperty")
        End If

        If SetListPropertysMethod IsNot Nothing Then
          SetListPropertysMethod.Invoke(Lists, New Object() {OfType, Value})
        End If
      End If

    End Sub

    Public Function GetList(ByVal OfType As Type) As Object
      Return Lists.GetList(OfType)
    End Function

    Public Function GetList(ByVal Name As String) As Object
      Return Lists.GetList(Name)
    End Function

    ''' <summary>
    ''' Gets a list based on the property name. If the name is blank, uses the type.
    ''' </summary>
    Public Function GetList(ByVal Name As String, ByVal Type As Type) As Object
      Dim List As Object = Nothing

      If Name <> "" Then
        List = GetList(Name)
      Else
        List = GetList(Type)
      End If

      Return List
    End Function

    Public Function GetTempList(ByVal Name As String, ByVal Type As Type) As Object

      If TempLists IsNot Nothing Then
        If Name <> "" Then
          Return TempLists.GetList(Name)
        Else
          Return TempLists.GetList(Type)
        End If
      Else
        Return Nothing
      End If

    End Function

    Public Sub Refresh(ByVal PropertyType As Type)
      Lists.Refresh(PropertyType)

      If TempLists IsNot Nothing Then
        TempLists.Refresh(PropertyType)
      End If
      If System.Web.HttpContext.Current IsNot Nothing AndAlso SessionLists IsNot Nothing Then
        SessionLists.Refresh(PropertyType)

      End If
    End Sub

#If SILVERLIGHT = False Then

    Public ReadOnly Property SessionLists As ICommonDataLists
      Get
        Return System.Web.HttpContext.Current.Session(CommonDataSessionKey)
      End Get
    End Property

    Public Function GetSessionList(ByVal OfType As Type) As Object
      Return SessionLists.GetList(OfType)
    End Function

    Public Function GetSessionList(ByVal Name As String) As Object
      Return SessionLists.GetList(Name)
    End Function

    Public Function GetSessionList(ByVal Name As String, ByVal OfType As Type) As Object
      Dim List As Object = Nothing
      If Not Singular.Misc.IsNullNothingOrEmpty(Name) Then
        List = GetSessionList(Name)
      End If

      If List Is Nothing Then
        List = GetSessionList(OfType)
      End If
      Return List
    End Function

    Public Sub SetContextDisallowedMode(ByVal Mode As ContextDisallowedModeType)
      If Lists IsNot Nothing Then
        Lists.SetContextDisallowedMode(Mode)
      End If
    End Sub

#End If

  End Module

  Public Class DataLoadedEventArgs

    Private mObject As Object

    Public ReadOnly Property [Object] As Object
      Get
        Return mObject
      End Get
    End Property

    Private mError As Exception

    Public ReadOnly Property [Error] As Exception
      Get
        Return mError
      End Get
    End Property

    Public Sub New(ByVal Data As Object, ByVal Exception As Exception)

      mObject = Data
      mError = Exception

    End Sub

    Public Sub New(ByVal Data As Object)

      mObject = Data

    End Sub

    Public Sub New(ByVal Exception As Exception)

      mError = Exception

    End Sub

  End Class


  Public Interface ICommonDataLists

    Sub Refresh(ByVal DataName As String)

    Sub Refresh(ByVal PropertyType As Type)
    Sub RefreshAll()

    Sub SetContextDisallowedMode(ByVal Mode As ContextDisallowedModeType)

    Function GetList(ByVal Name As String) As Object
    Function GetList(ByVal Type As Type) As Object
    Function GetPropertyName(ByVal ListType As Type) As String

    Function GetCachedData(Type As Type, Name As String) As CachedData

    Event PropertyRefreshed(ByVal PropertyName As String)

  End Interface

  Public Class CommonDataBase(Of T As CachedLists)

    Public Delegate Sub DataLoadedHandler(ByVal e As DataLoadedEventArgs)

    <Serializable()>
    Public Class CachedLists
      Inherits SingularReadOnlyBase(Of T)
      Implements INotifyPropertyChanged
      Implements ICommonDataLists

      Public Event PropertyRefreshed(ByVal PropertyName As String) Implements ICommonDataLists.PropertyRefreshed

#Region " Defined Lists "

#Region " SecurityGroupList "

#If SILVERLIGHT Then

      Public Shared SecurityGroupListProperty As Csla.PropertyInfo(Of Security.SecurityGroupList) = _
        RegisterProperty(Of Security.SecurityGroupList)(Function(c) c.SecurityGroupList)

      Public Property SecurityGroupList() As Security.SecurityGroupList
        Get
          If Not FieldManager.FieldExists(SecurityGroupListProperty) Then
            Security.SecurityGroupList.BeginGetSecurityGroupList( _
              Sub(o, e)
                If e.Error IsNot Nothing Then
                  Throw e.Error
                End If
                Me.SecurityGroupList = e.Object
                OnPropertyChanged(SecurityGroupListProperty)
              End Sub _
            )
          End If
          Return GetProperty(SecurityGroupListProperty)
        End Get
        Private Set(ByVal value As Security.SecurityGroupList)
          LoadProperty(SecurityGroupListProperty, value)
        End Set
      End Property

#End If

#End Region

#Region " HomeScreenList "

#If SILVERLIGHT Then

      Public Shared HomeScreenListProperty As Csla.PropertyInfo(Of CSLALib.HomeScreenList) = _
        RegisterProperty(Of CSLALib.HomeScreenList)(Function(c) c.HomeScreenList)

      Public Overridable Property HomeScreenList() As CSLALib.HomeScreenList
        Get
          Return GetProperty(HomeScreenListProperty)
        End Get
        Set(ByVal value As CSLALib.HomeScreenList)
          LoadProperty(HomeScreenListProperty, value)
        End Set
      End Property

#End If

#End Region

#End Region

#Region " System Settings "

      'for now use Singular.SystemSettings.GetSystemSetting

#End Region

      Friend Property MainContext As ContextType
      Private mLastCacheReset As Date = Now
      Private mCacheLifeTime As TimeSpan = CommonDataBase(Of T).DefaultLifeTime

      Friend Function ShouldReset() As Boolean
        If mCacheLifeTime.Ticks = 0 Then
          Return False
        Else
          Return mLastCacheReset.Add(mCacheLifeTime) < Now
        End If
      End Function

      Public Function GetListProperty(ByVal TypeToGet As Type) As Csla.Core.IPropertyInfo

        For Each pi In FieldManager.GetRegisteredProperties
          If pi.Type.Equals(TypeToGet) Then
            Return pi
          End If
        Next

        Return Nothing

      End Function

      Public Sub SetListProperty(ByVal OfType As Type, ByVal Value As Object)

        Dim PropertyToSet = GetListProperty(OfType)
        LoadProperty(PropertyToSet, Value)

      End Sub

      Public Function GetPropertyName(ByVal ListType As Type) As String Implements ICommonDataLists.GetPropertyName

#If SILVERLIGHT = False Then

        Dim PropInfo As PropertyInfo = Singular.Reflection.GetProperty(Me.GetType, ListType)
        If PropInfo IsNot Nothing Then
          Return PropInfo.Name
        End If
#End If

        Dim pi = GetListProperty(ListType)
        If pi IsNot Nothing Then
          Return pi.Name
        End If

        Return Nothing

      End Function

      ''' <summary>
      ''' Gets the First List that is the Specified Type.
      ''' </summary>
      Public Function GetList(ByVal TypeToGet As Type) As Object Implements ICommonDataLists.GetList

        Dim PropertyName As String = GetPropertyName(TypeToGet)
        If PropertyName IsNot Nothing Then
          Return GetList(PropertyName)
        End If

        Return Nothing
        '#If SILVERLIGHT = False Then

        '        Dim PropInfo As PropertyInfo = Singular.Reflection.GetProperty(Me.GetType, TypeToGet)
        '        If PropInfo IsNot Nothing Then
        '          Return PropInfo.GetValue(Me, Nothing)
        '        End If
        '#End If

        '        Dim pi = GetListProperty(TypeToGet)
        '        If pi IsNot Nothing Then
        '          Return GetProperty(pi)
        '        End If

        '        Return Nothing

      End Function

      ''' <summary>
      ''' Gets the First List that is the Specified Name
      ''' </summary>
      Public Function GetList(ByVal Name As String) As Object Implements ICommonDataLists.GetList

#If SILVERLIGHT Then
#Else
        Dim PropInfo As PropertyInfo = Singular.Reflection.GetProperty(Me.GetType, Name)
        If PropInfo IsNot Nothing Then
          Return PropInfo.GetValue(Me, Nothing)
        End If
#End If

        Dim pi = GetListProperty(FieldManager.GetRegisteredProperties.Where(Function(f) f.Name = Name).FirstOrDefault())
        If pi IsNot Nothing Then
          Return GetProperty(pi)
        End If

        Return Nothing

      End Function

      Private mCacheLock As New Object
      Private mLastCachedData As CachedData
      Public Function GetCachedData(Type As Type, Name As String) As CachedData Implements ICommonDataLists.GetCachedData
        If Name = "" Then
          Name = GetPropertyName(Type)
        End If
        SyncLock mCacheLock
          mLastCachedData = Nothing
          Dim Data As Object = GetList(Name)
          If mLastCachedData Is Nothing Then
            mLastCachedData = New CachedData With {.Data = Data}
          End If
          Return mLastCachedData
        End SyncLock
      End Function

#If SILVERLIGHT Then
#Else
      Private mContextDisallowedMode As ContextDisallowedModeType = ContextDisallowedModeType.Exception
      Private mLastContextMode As ContextDisallowedModeType = ContextDisallowedModeType.PreviousValue
      Public Sub SetContextDisallowedMode(ByVal Mode As ContextDisallowedModeType) Implements ICommonDataLists.SetContextDisallowedMode
        If Mode = ContextDisallowedModeType.PreviousValue Then
          If mLastContextMode = ContextDisallowedModeType.PreviousValue Then
            Throw New Exception("There was no Previous Context Mode")
          Else
            mContextDisallowedMode = mLastContextMode
          End If
        Else
          mLastContextMode = mContextDisallowedMode
          mContextDisallowedMode = Mode
        End If
      End Sub

      Private Class CachedObject
        Private Contexts As New Hashtable

        Public Function GetData(Context As String) As CachedData
          Return Contexts(Context)
        End Function

        Public Function SetData(Context As String, Data As Object) As CachedData
          Dim cd As CachedData = Contexts(Context)
          If cd Is Nothing Then
            cd = New CachedData
            Contexts(Context) = cd
          End If
          cd.Data = Data
          Return cd
        End Function
      End Class

      Private mCachedTypes As New Hashtable
      Private mCachedObjects As New Hashtable

      Private Function GetCachedObject(Name As String, Scope As ContextType) As CachedObject
        SyncLock mCacheLock
          If Scope = ContextType.Temporary Then
#If SILVERLIGHT = False Then

            Dim co As CachedObject = System.Web.HttpContext.Current.Cache.Get("CommonData" & Name)
            If co Is Nothing Then
              co = New CachedObject
              System.Web.HttpContext.Current.Cache.Add("CommonData" & Name, co, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 1, 0), System.Web.Caching.CacheItemPriority.Default, Nothing)
            End If
            Return co

#End If
          Else
            Dim co As CachedObject = mCachedObjects(Name)
            If co Is Nothing Then
              co = New CachedObject
              mCachedObjects(Name) = co
            End If
            Return co
          End If
        End SyncLock
      End Function

      Private Function GetCachedObjects(NameStartingWith As String) As List(Of KeyValuePair(Of String, CachedObject))
        SyncLock mCacheLock
          Dim cachedObjects = (From modKey As DictionaryEntry In mCachedObjects
                               Where modKey.Key.ToString.StartsWith(NameStartingWith)
                               Select New KeyValuePair(Of String, CachedObject)(modKey.Key, CType(modKey.Value, CachedObject))).ToList()
          Return cachedObjects
        End SyncLock
      End Function

      Public Function GetCachedObjectData(ByVal AllowedScope As ContextType,
                                          NameStartingWith As String,
                                          Optional Context As Object = Nothing) As List(Of KeyValuePair(Of String, Object))
        Dim cachedObjs = GetCachedObjects(NameStartingWith)
        Return cachedObjs.Select(Function(co)
                                   SyncLock co.Value
                                     Dim LookupContext = If(AllowedScope And ContextType.Localised = ContextType.Localised, Singular.Localisation.CurrentCulture.TwoLetterISOLanguageName, "en")
                                     If Context IsNot Nothing Then
                                       LookupContext &= "_" & Context.ToString
                                     End If
                                     Return New KeyValuePair(Of String, Object)(co.Key, co.Value.GetData(LookupContext).Data)
                                   End SyncLock
                                 End Function).ToList() '.Select(Function(co) Return Nothing)
      End Function

      Private Sub ClearCachedObject(Name As String)
        mCachedTypes.Remove(Name)
        mCachedObjects.Remove(Name)
#If SILVERLIGHT = False Then
        If System.Web.HttpContext.Current IsNot Nothing Then
          System.Web.HttpContext.Current.Cache.Remove("CommonData" & Name)
        End If
#End If
      End Sub

      Public Function RegisterList(Of ReturnType)(ByVal AllowedScope As ContextType,
                                                  ByVal le As System.Linq.Expressions.Expression(Of Func(Of T, ReturnType)),
                                                  ByVal CallBack As System.Func(Of Object),
                                                  Optional Context As Object = Nothing) As ReturnType

        Dim pi As System.Reflection.PropertyInfo = Singular.Reflection.GetMemberSpecific(Of T, ReturnType)(le)
        Dim Name As String = pi.Name

        Return RegisterList(Of ReturnType)(AllowedScope, Name, CallBack, Context)

      End Function

      Public Function RegisterList(Of ReturnType)(ByVal AllowedScope As ContextType,
                                                  ByVal Name As String,
                                                  ByVal CallBack As System.Func(Of Object),
                                                  Optional Context As Object = Nothing) As ReturnType

        If AllowedScope = (AllowedScope Or MainContext) Then

          'Get the cached object by name
          Dim co As CachedObject = GetCachedObject(Name, AllowedScope)

          SyncLock co
            'Get the data from the cached object, according to the current language and provided context.
            Dim LookupContext = If(AllowedScope And ContextType.Localised = ContextType.Localised, Singular.Localisation.CurrentCulture.TwoLetterISOLanguageName, "en")
            If Context IsNot Nothing Then
              LookupContext &= "_" & Context.ToString
            End If
            Dim CData As CachedData = co.GetData(LookupContext)
            Dim Data As Object
            If CData Is Nothing OrElse CData.Data Is Nothing Then
              Data = CallBack()
              mCachedTypes(Name) = Data.GetType
              CData = co.SetData(LookupContext, Data)
              OnPropertyChanged(Name)
            Else
              Data = CData.Data
            End If
            mLastCachedData = CData

            Return Data
          End SyncLock

        Else
          If mContextDisallowedMode = ContextDisallowedModeType.Exception Then
            Throw New Exception(Name & " is not allowed in the " & MainContext.ToString & " context.")
          Else
            Return Nothing
          End If
        End If

      End Function

      'Public Function RegisterList(ByVal AllowedScope As ContextType,
      '                                            ByVal le As System.Linq.Expressions.Expression(Of Func(Of T, Object)),
      '                                            ByVal CallBack As System.Func(Of Object),
      '                                            Optional Context As Object = Nothing) As Object
      '  Return RegisterList(Of Object)(AllowedScope, le, CallBack, Context)
      'End Function

      Public Sub Refresh(ByVal PropertyType As Type) Implements ICommonDataLists.Refresh

        'Properties with CSLA managed backing fields.
        For Each pi In FieldManager.GetRegisteredProperties
          If pi.Type = PropertyType Then
            LoadProperty(pi, Nothing)
            RaiseEvent PropertyRefreshed(pi.Name)
          End If
        Next

        'Properties with internal storage using RegisterList
        'This is done in two parts because the collection can't be modified in a loop.
        Dim Keys As New List(Of String)
        For Each key As String In mCachedTypes.Keys
          Keys.Add(key)
        Next

        For Each key As String In Keys
          If mCachedTypes(key) Is PropertyType Then
            ClearCachedObject(key)
            RaiseEvent PropertyRefreshed(key)
          End If
        Next

        If MainContext = ContextType.Application Then
          Singular.Service.NotifyService(Service.ServiceUpdateMessageType.CommonDataType, Singular.Reflection.GetTypeFullName(PropertyType))
        End If

      End Sub

      Public Sub RefreshAll() Implements ICommonDataLists.RefreshAll

        'Properties with CSLA managed backing fields.
        For Each pi In FieldManager.GetRegisteredProperties
          LoadProperty(pi, Nothing)
          RaiseEvent PropertyRefreshed(pi.Name)
        Next

        Dim Keys As New List(Of String)
        For Each key As String In mCachedTypes.Keys
          Keys.Add(key)
        Next

        For Each key As String In Keys
          ClearCachedObject(key)
          RaiseEvent PropertyRefreshed(key)
        Next

        If MainContext = ContextType.Application Then
          Singular.Service.NotifyService(Service.ServiceUpdateMessageType.CommonDataAll, "")
        End If

      End Sub

#End If

      Public Sub RefreshCachedStartingWith(DataNameStartsWith As String)

        Dim keysToRemove As New List(Of String)
        For Each ct As DictionaryEntry In mCachedTypes
          If ct.Key.ToString().StartsWith(DataNameStartsWith) Then
            keysToRemove.Add(ct.Key.ToString())
          End If
        Next

        For Each key In keysToRemove
          ClearCachedObject(key)
          RaiseEvent PropertyRefreshed(key)

          If MainContext = ContextType.Application Then
            Singular.Service.NotifyService(Service.ServiceUpdateMessageType.CommonDataName, key)
          End If
        Next

      End Sub

      Public Sub RefreshCached(DataName As String)
        If mCachedTypes.ContainsKey(DataName) Then
          ClearCachedObject(DataName)
          RaiseEvent PropertyRefreshed(DataName)
        End If

        If MainContext = ContextType.Application Then
          Singular.Service.NotifyService(Service.ServiceUpdateMessageType.CommonDataName, DataName)
        End If

      End Sub

      Public Sub Refresh(ByVal DataName As String) Implements ICommonDataLists.Refresh

        Dim ListsCleared As Boolean = False

        'Dim x = CObj(Me).ROEventTypeList
        For Each pi In FieldManager.GetRegisteredProperties
          If (pi.Name.EndsWith(DataName) Or DataName = "") Then
            LoadProperty(pi, Nothing)
            RaiseEvent PropertyRefreshed(pi.Name)
            ListsCleared = True
            If DataName <> "" Then
              Exit For
            End If
          End If
        Next

#If SILVERLIGHT = False Then

        RefreshCached(DataName)

#End If

        If Singular.Settings.CurrentPlatform = Singular.CommonDataPlatform.Windows Then
          'Moved here by Marlborough, there is no point doing this on the server, maybe its neccessary in silverlight?
          ' now to try re-fetch the property
          Dim pis() As PropertyInfo = Me.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.Public)
          For Each pi As PropertyInfo In pis
            If (pi.Name.EndsWith(DataName) Or DataName = "") Then
              Dim obj = pi.GetValue(Me, Nothing)
              Exit For
            End If
          Next
        Else

#If SILVERLIGHT Then
        'Moved here by Marlborough, there is no point doing this on the server, maybe its neccessary in silverlight?
          ' now to try re-fetch the property
        Dim pis() As PropertyInfo = Me.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.Public)
        For Each pi As PropertyInfo In pis
          If (pi.Name.EndsWith(DataName) Or DataName = "") Then
            Dim obj = pi.GetValue(Me, Nothing)
            Exit For
          End If
        Next
#End If

        End If

      End Sub

    End Class

#Region " Enums "

    Public Class Enums

#If SILVERLIGHT Then

      Public Shared Function EnumToString(ByVal value As [Enum]) As String

        Return Singular.Strings.Readable(value.ToString.Replace("_", ""))

      End Function

      ''' -----------------------------------------------------------------------------
      ''' <summary>
      ''' Perform the <B>AlternateDescription</B> operation on this object.
      ''' </summary>
      ''' <param name="value"></param>
      ''' <returns></returns>
      ''' -----------------------------------------------------------------------------
      Public Shared Function AlternateDescription(ByVal value As [Enum]) As String

        Dim Info As FieldInfo = value.GetType.GetField(value.ToString)

        ' If the value is not valid for the enumeration then return '(None)'.
        If Info Is Nothing Then Return "Enum Alternative Description Not Located."

        Dim Attributes As CategoryAttribute() = DirectCast(Info.GetCustomAttributes(GetType(CategoryAttribute), False), CategoryAttribute())

        If Attributes.Length > 0 Then
          Return Attributes(0).Category
        Else
          Return Singular.Strings.Readable(value.ToString.Replace("_", ""))
        End If

      End Function

      ' ''' <summary>
      ' ''' Returns a value list item of the given enumeration using the value and the description of the enum
      ' ''' </summary>
      ' ''' <param name="Value"></param>
      ' ''' <returns></returns>
      ' ''' <remarks></remarks>
      'Public Shared Function GetEnumValueListItem(ByVal Value As [Enum]) As Infragistics.Win.ValueListItem

      '  Return New Infragistics.Win.ValueListItem(Value, Description(Value))

      'End Function

      ''' -----------------------------------------------------------------------------
      ''' <summary>
      ''' Perform the <B>Value</B> operation on this object.
      ''' </summary>
      ''' <param name="enumType"></param>
      ''' <param name="description"></param>
      ''' <returns></returns>
      ''' -----------------------------------------------------------------------------
      Public Shared Function Value(ByVal enumType As Type, ByVal description As String) As Integer

        Dim InfoCollection As FieldInfo() = enumType.GetFields
        Dim DescAttributes As DescriptionAttribute()

        For Each Info As FieldInfo In InfoCollection
          If Info.FieldType Is enumType Then
            ' Get the description custom attributes.
            DescAttributes = DirectCast(Info.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

            If DescAttributes.Length > 0 Then
              ' Enum has a description attribute so check with the specified description.
              If DescAttributes(0).Description.Equals(description) Then
                ' Match found so return the enum as a generic integer.
                Return Convert.ToInt32(Info.GetValue(Info))
              End If

            Else
              ' Enum doesn't have a description attribute so check with the enum name.
              If Info.Name.Equals(description) Then
                ' Match found so return the enum as a generic integer.
                Return Convert.ToInt32(Info.GetValue(Info))
              End If
            End If
          End If
        Next
        Return 0

      End Function

      ' ''' -----------------------------------------------------------------------------
      ' ''' <summary>
      ' ''' Perform the <B>GetDescriptions</B> operation on this object.
      ' ''' </summary>
      ' ''' <param name="enumType"></param>
      ' ''' <param name="categories"></param>
      ' ''' <returns></returns>
      ' ''' -----------------------------------------------------------------------------
      'Public Shared Function GetDescriptions(ByVal enumType As Type, Optional ByVal categories As String = "") As DataTable

      '  Dim InfoCollection As FieldInfo() = enumType.GetFields
      '  Dim Values As New DataTable
      '  Dim DescAttributes As DescriptionAttribute()
      '  Dim CatAttributes As CategoryAttribute()
      '  Dim Key As Object

      '  Values.Columns.Add("ValueMember", GetType(Integer))
      '  Values.Columns.Add("DisplayMember", GetType(String))

      '  For Each Info As FieldInfo In InfoCollection
      '    If Info.FieldType Is enumType Then
      '      ' Set the category to valid if no categories were specified.
      '      Dim CatValid As Boolean = (categories = String.Empty)

      '      ' Get the description custom attributes.
      '      DescAttributes = DirectCast(Info.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

      '      ' Check if the category is valid.
      '      If Not CatValid Then
      '        ' Get the category custom attributes.
      '        CatAttributes = DirectCast(Info.GetCustomAttributes(GetType(CategoryAttribute), False), CategoryAttribute())

      '        ' Set the category to valid if the specified category matches.
      '        If CatAttributes.Length > 0 Then CatValid = (CatAttributes(0).Category.ToLower = categories.ToLower)
      '      End If

      '      ' Only add the item to the list if it doesn't already exist and it's valid for the categegory.
      '      Key = Info.GetValue(Info)
      '      Dim drw As DataRow = Values.NewRow
      '      If DescAttributes.Length > 0 Then
      '        drw("ValueMember") = CType(Info.GetValue(Info), Integer)
      '        drw("DisplayMember") = DescAttributes(0).Description
      '        Values.Rows.Add(drw)
      '      Else
      '        drw("ValueMember") = CType(Info.GetValue(Info), Integer)
      '        drw("DisplayMember") = Singular.Strings.Readable(Info.Name)
      '        Values.Rows.Add(drw)
      '      End If
      '    End If
      '  Next

      '  Return Values

      'End Function

      ''' -----------------------------------------------------------------------------
      ''' <summary>
      ''' Returns true if the given value is an enum type
      ''' </summary>
      ''' <param name="EnumType">Type of Enumeration</param>
      ''' <param name="Value">Value you want to test</param>
      ''' <returns></returns>
      ''' -----------------------------------------------------------------------------
      Public Shared Function IsEnum(ByVal EnumType As Type, ByVal Value As Object) As Boolean

        Dim InfoCollection As FieldInfo() = EnumType.GetFields

        For Each Info As FieldInfo In InfoCollection
          If Info.FieldType Is EnumType Then
            ' Only add the item to the list if it doesn't already exist and it's valid for the categegory.
            If Value = CType(Info.GetValue(Info), Integer) Then
              Return True
            End If
          End If
        Next

        Return False

      End Function



#End If



      '  Public Shared Function GetEnumDescriptionCombo(ByVal EnumType As Type, Optional ByVal ComboToLoad As UltraCombo = Nothing) As UltraCombo

      '    Dim EnmList As DataTable = CommonData.Enums.GetDescriptions(EnumType)
      '    If Not (ComboToLoad Is Nothing) Then
      '      ComboToLoad.DataSource = EnmList
      '      ComboToLoad.ValueMember = "ValueMember"
      '      ComboToLoad.DisplayMember = "DisplayMember"
      '      ComboToLoad.DisplayLayout.Bands(0).Columns("ValueMember").Hidden = True
      '      ComboToLoad.DisplayLayout.Bands(0).ColHeadersVisible = False
      '      Return ComboToLoad
      '    Else
      '      Dim udc As New UltraCombo()
      '      udc.DataSource = EnmList
      '      udc.ValueMember = "ValueMember"
      '      udc.DisplayMember = "DisplayMember"
      '      udc.DisplayLayout.Bands(0).Columns("ValueMember").Hidden = True
      '      udc.DisplayLayout.Bands(0).ColHeadersVisible = False
      '      Return udc
      '    End If

      '  End Function

      '  Public Shared Function GetEnumDescriptionDropDown(ByVal EnumType As Type) As UltraDropDown

      '    Dim udd As New UltraDropDown()
      '    Dim EnmList As DataTable = CommonData.Enums.GetDescriptions(EnumType)
      '    udd.DataSource = EnmList
      '    udd.ValueMember = "ValueMember"
      '    udd.DisplayMember = "DisplayMember"

      '    Singular.Controls.DropDowns.SetupCommonConfig(udd)

      '    udd.DisplayLayout.Bands(0).Columns("ValueMember").Hidden = True
      '    udd.DisplayLayout.Bands(0).ColHeadersVisible = False

      '    Return udd

      '  End Function

      'End Class

      Protected Shared mLists As CachedLists

      Public Shared Function GetCachedLists() As CachedLists

        Return mLists

      End Function

      Friend Shared Sub SetCachedLists(ByVal CachedLists As CachedLists)

        mLists = CachedLists

      End Sub


      Public Sub Refresh(ByVal DataName As String)



      End Sub


      'Public Shared Function GetEnumDropDown(ByVal EnumType As Type) As UltraDropDown

      '  Dim udd As New UltraDropDown()
      '  Dim tbl As New DataTable
      '  tbl.Columns.Add("Value", GetType(Integer))
      '  tbl.Columns.Add("Display", GetType(String))
      '  For Each enu As Object In [Enum].GetValues(EnumType)
      '    Dim drw As DataRow = tbl.NewRow
      '    drw("Value") = CInt(enu)
      '    drw("Display") = Strings.Readable(enu.ToString.Replace("_", ""))
      '    tbl.Rows.Add(drw)
      '  Next
      '  udd.DataSource = tbl
      '  udd.ValueMember = "Value"
      '  udd.DisplayMember = "Display"
      '  udd.DisplayLayout.Bands(0).Columns("Value").Hidden = True
      '  udd.DisplayLayout.Bands(0).ColHeadersVisible = False
      '  Return udd

      'End Function

      'Public Shared Function GetEnumComboBoxDropDown(ByVal EnumType As Type) As UltraCombo

      '  Dim ucb As New UltraCombo()
      '  Dim tbl As New DataTable
      '  tbl.Columns.Add("Value", GetType(Integer))
      '  tbl.Columns.Add("Display", GetType(String))
      '  For Each enu As Object In [Enum].GetValues(EnumType)
      '    Dim drw As DataRow = tbl.NewRow
      '    drw("Value") = CInt(enu)
      '    drw("Display") = Strings.Readable(enu.ToString.Replace("_", ""))
      '    tbl.Rows.Add(drw)
      '  Next
      '  ucb.DataSource = tbl
      '  ucb.ValueMember = "Value"
      '  ucb.DisplayMember = "Display"
      '  Return ucb

      'End Function


      'Public Class SystemSettings

      '  Protected Shared mSystemSettingList As Singular.SystemSettings.SystemSettingList

      '  Public Shared Function GetCustomSetting(ByVal SettingName As String) As Singular.SystemSettings.CustomSettings

      '    Dim ss As Singular.SystemSettings.SystemSetting = SystemSettingList.Find(SettingName)
      '    If ss Is Nothing Then
      '      Return Nothing
      '    Else
      '      Return ss.CustomSettings
      '    End If

      '  End Function

      '  Public Shared ReadOnly Property SystemSettingList() As Singular.SystemSettings.SystemSettingList
      '    Get
      '      If mSystemSettingList Is Nothing Then
      '        mSystemSettingList = Singular.SystemSettings.SystemSettingList.GetSystemSettingList
      '      End If
      '      Return mSystemSettingList
      '    End Get
      '  End Property

      '  Public Shared Sub ClearSettings()
      '    mSystemSettingList = Nothing
      '  End Sub

    End Class

#End Region

#Region " Application Scoped Lists "

    ''' <summary>
    ''' The time inverval before the commondata lists should be reset. Default is Zero, which means never.
    ''' </summary>
    Public Shared Property DefaultLifeTime As New TimeSpan(0)

    Protected Shared mLists As T

    Public Shared Function GetCachedLists() As T

      'Check if the Lists should be reset.
      If mLists Is Nothing OrElse mLists.ShouldReset Then
        Try
          mLists = Activator.CreateInstance(Of T)()
          mLists.MainContext = ContextType.Application
          Misc.Lists = mLists
        Catch ex As Exception
          Throw New Exception("Error creating CommonData cached lists object: " & GetType(T).Name, ex)
        End Try

      End If
      Return mLists

    End Function

    Public Shared ReadOnly Property Lists As T
      Get
        Return GetCachedLists()
      End Get
    End Property

#End Region

#Region " Session Scoped Lists "

#If SILVERLIGHT Then
#Else

    ''' <summary>
    ''' Returns the Lists specific to this session.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property SessionLists As T
      Get
        If System.Web.HttpContext.Current Is Nothing Then
          Throw New Exception("This is not a Web Application: Use Lists, not SessionLists")
        Else
          If System.Web.HttpContext.Current.Session(CommonDataSessionKey) Is Nothing Then
            'Throw New Exception("Session Lists have not been initialised. Call InitialiseSessionLists when each session starts.")
            InitialiseSessionLists()
          End If
          Return System.Web.HttpContext.Current.Session(CommonDataSessionKey)
        End If
      End Get
    End Property

    Public Shared Sub InitialiseSessionLists()
      Dim Lists As T = Activator.CreateInstance(Of T)()
      Lists.MainContext = ContextType.Session
      System.Web.HttpContext.Current.Session(CommonDataSessionKey) = Lists
    End Sub

#End If

#End Region

#Region " Temporary Application Scoped Lists "

    Private Shared mTempLists As T = Nothing
    Public Shared ReadOnly Property TempLists As T
      Get
        If mTempLists Is Nothing Then
          mTempLists = Activator.CreateInstance(Of T)()
          mTempLists.MainContext = ContextType.Temporary
          Misc.TempLists = mTempLists
        End If
        Return mTempLists
      End Get
    End Property

#End Region


    Friend Shared Sub SetCachedLists(ByVal CachedLists As T)

      mLists = CachedLists

    End Sub

    Public Shared Sub Refresh(ByVal DataName As String)

      Refresh(DataName, False)

    End Sub

    Public Shared Sub Refresh(ByVal DataName As String, ByVal ThrowExceptionIfNotExists As Boolean)

      If mLists Is Nothing Then Exit Sub

      mLists.Refresh(DataName)

      'Dim ListsCleared As Boolean = False
      'Dim fis() As FieldInfo = mLists.GetType.GetFields(BindingFlags.Instance Or BindingFlags.NonPublic)
      'For Each fi As FieldInfo In fis
      '  If (fi.Name.EndsWith(DataName) Or DataName = "") Then
      '    fi.SetValue(mLists, Nothing)
      '    ListsCleared = True
      '    Exit 
      '  End If
      'Next

      'Dim pis() As PropertyInfo = mLists.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.NonPublic)
      'For Each pi As PropertyInfo In pis
      '  If (pi.Name.EndsWith(DataName) Or DataName = "") Then
      '    Dim obj = pi.GetValue(mLists, Nothing)
      '    Exit For
      '  End If
      'Next


    End Sub

  End Class

  Public Class EnumsBase

    Public Shared Function GetEnumStrings(Of T)(ByVal SortDirection As ListSortDirection) As String()

      If SortDirection = ListSortDirection.Ascending Then
        Return (From field In GetEnumStrings(Of T)()
                Select field
                Order By field).ToArray()
      Else
        Return (From field In GetEnumStrings(Of T)()
                Select field
                Order By field Descending).ToArray()
      End If

    End Function

    Public Shared Function GetEnumStrings(Of T)() As String()
      Dim type = GetType(T)
      If Not type.IsEnum Then
        Throw New ArgumentException("Type '" + type.Name & "' is not an enum")
      End If

      Return (From field In type.GetFields(BindingFlags.[Public] Or BindingFlags.[Static])
              Where field.IsLiteral
              Select Enums.Description(DirectCast(field.GetValue(Nothing), T))).ToArray()

    End Function

    Public Shared Function GetEnumValues(Of T)(ByVal SortDirection As ListSortDirection) As T()

      If SortDirection = ListSortDirection.Ascending Then
        Return (From value In GetEnumValues(Of T)()
                Select value
                Order By value).ToArray()
      Else
        Return (From value In GetEnumValues(Of T)()
                Select value
                Order By value Descending).ToArray()
      End If

    End Function

    Public Shared Function GetEnumValues(Of T)() As T()
      Dim type = GetType(T)
      If Not type.IsEnum Then
        Throw New ArgumentException("Type '" + type.Name & "' is not an enum")
      End If

      Return (From field In type.GetFields(BindingFlags.[Public] Or BindingFlags.[Static])
              Where field.IsLiteral
              Select DirectCast(field.GetValue(Nothing), T)).ToArray()
    End Function

  End Class

End Namespace