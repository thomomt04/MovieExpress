Imports System.Reflection
Imports System.ComponentModel.DataAnnotations
Imports Singular.Extensions

Namespace DataAnnotations


#Region " Normal "

  ''' <summary>
  ''' Incidates that this property must be serialised, regardless of browsable / autogenerate.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class MustSerialise
    Inherits Attribute
  End Class

  ''' <summary>
  ''' Indicates that this Key is not encrypted, and can be changed client side.
  ''' Property must still have a Key attribute assigned.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class UnProtectedKey
    Inherits Attribute

  End Class

  <AttributeUsage(AttributeTargets.Class)>
  Public Class ProtectedKeySalt
    Inherits Attribute

    Public Property Salt As String

    Public Sub New(Salt As String)
      Me.Salt = Salt
    End Sub
  End Class

  <AttributeUsage(AttributeTargets.Property)>
  Public Class ParentID
    Inherits Attribute

  End Class

  ''' <summary>
  ''' Indicates that this property cannot be updated unless the object is new.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class NonUpdatable
    Inherits Attribute

    Private mAllowedRole As String

    Public ReadOnly Property AllowedRole As String
      Get
        Return mAllowedRole
      End Get
    End Property

    Public Sub New()

    End Sub

    Public Sub New(AllowedRole As String)
      mAllowedRole = AllowedRole
    End Sub

  End Class

  Public Enum CommonDataInvokeType
    Method
    [Property]
  End Enum

  Public Class DisplayLayoutInfo
    Inherits Attribute

    Public Property DisplayLayoutName As String

    Public Sub New(DisplayLayoutName As String)

      Me.DisplayLayoutName = DisplayLayoutName

    End Sub

  End Class

#If SILVERLIGHT Then
#Else

  Public Class Intellisense
    Inherits Attribute

    Private mCommonDataInvokeType As CommonDataInvokeType

    Private mProperty As PropertyInfo
    Private mMethod As MethodInfo

    Private mStringPropertyToUse As String

    Private mParameter As Object = Nothing

    Public Function GetIntellisenseList() As List(Of String)

      Dim List As IEnumerable = Nothing
      Select Case mCommonDataInvokeType
        Case CommonDataInvokeType.Method
          If mParameter Is Nothing Then
            List = mMethod.Invoke(CommonData.Lists, Nothing)
          Else
            List = mMethod.Invoke(CommonData.Lists, {mParameter})
          End If
        Case CommonDataInvokeType.Property
          List = mProperty.GetValue(CommonData.Lists, Nothing)
      End Select

      If List IsNot Nothing Then
        If List.GetType.Equals(GetType(List(Of String))) Then
          Return List
        Else
          If CObj(List).Count > 0 Then
            Dim pi = List(0).GetType.GetProperty(mStringPropertyToUse)
            If pi IsNot Nothing Then
              Return (From obj In List
                      Select CStr(pi.GetValue(obj, Nothing))).Distinct.OrderBy(Function(s) s).ToList
            End If
          End If
        End If
      End If

      Return Nothing

    End Function

    Public Sub New(ByVal CommonDataInvokeType As CommonDataInvokeType, ByVal MethodOrProperty As String, ByVal StringPropertyToUse As String, ByVal Parameter As Object)

      mCommonDataInvokeType = CommonDataInvokeType
      mParameter = Parameter
      mStringPropertyToUse = StringPropertyToUse
      Select Case CommonDataInvokeType
        Case DataAnnotations.CommonDataInvokeType.Method
          mMethod = CommonData.Lists.GetType.GetMethod(MethodOrProperty)
          If mMethod Is Nothing Then
            Throw New Exception(String.Format("Method '{0}' not found in CommonData.Lists", MethodOrProperty))
          End If
        Case DataAnnotations.CommonDataInvokeType.Property
          mProperty = CommonData.Lists.GetType.GetProperty(MethodOrProperty)
          If mProperty Is Nothing Then
            Throw New Exception(String.Format("Property '{0}' not found in CommonData.Lists", MethodOrProperty))
          End If
      End Select

    End Sub

    Public Sub New(ByVal CommonDataInvokeType As CommonDataInvokeType, ByVal MethodOrProperty As String, ByVal StringPropertyToUse As String)

      mCommonDataInvokeType = CommonDataInvokeType
      mStringPropertyToUse = StringPropertyToUse
      Select Case CommonDataInvokeType
        Case DataAnnotations.CommonDataInvokeType.Method
          mMethod = CommonData.Lists.GetType.GetMethod(MethodOrProperty)
          If mMethod Is Nothing Then
            Throw New Exception(String.Format("Method '{0}' not found in CommonData.Lists", MethodOrProperty))
          End If
        Case DataAnnotations.CommonDataInvokeType.Property
          mProperty = CommonData.Lists.GetType.GetProperty(MethodOrProperty)
          If mProperty Is Nothing Then
            Throw New Exception(String.Format("Property '{0}' not found in CommonData.Lists", MethodOrProperty))
          End If
      End Select

    End Sub

  End Class
#End If

  Public Class DropDownColumn
    Inherits Attribute

    Public Property ShowColumn As Boolean = True

    Public Property IsDisplayMember As Boolean = False

    Public Property IsValueMember As Boolean = False

  End Class

  Public Class ColumnWidth
    Inherits Attribute

    Public MinWidth As Integer = 0
    Public MaxWidth As Integer = 0
    Public DefaultWidth As Integer = 0

    Public Property NewLineAfter As Boolean = False
    Public Property ColSpan As Integer = 1
    Public Property Bold As Boolean

    Public Sub New(Optional ByVal MinWidth As Integer = 0, Optional ByVal MaxWidth As Integer = 0, Optional ByVal DefaultWidth As Integer = 0)

      Me.MinWidth = MinWidth
      Me.MaxWidth = MaxWidth
      Me.DefaultWidth = DefaultWidth

    End Sub

  End Class

  Public Class Column
    Inherits ColumnWidth

    Public Sub New(DefaultWidth As Integer)
      Me.DefaultWidth = DefaultWidth
    End Sub
  End Class

  Public MustInherit Class FilterConditionBase

    Public MustOverride Function MatchesFilter(ByVal DataItem As Object) As Boolean

  End Class

  Public Class MultiFilterCondition
    Inherits FilterConditionBase

    Public FilterConditions As New List(Of FilterConditionBase)

    Public Overrides Function MatchesFilter(ByVal DataItem As Object) As Boolean

      Dim Result As Boolean = True

      For Each fc In FilterConditions
        Result = Result And fc.MatchesFilter(DataItem)
      Next

      Return Result

    End Function

    Public Sub New(ByVal FilterConditions As List(Of FilterConditionBase))

      Me.FilterConditions = FilterConditions

    End Sub

  End Class

  Public Class FilterCondition
    Inherits FilterConditionBase

    Public Property FilterField As String

    Public Property FilterValue As Object

    Public Property FilterType As DirectFilterType

    Public Property UniqueAmongstSiblings As Boolean = False

    Public Property SiblingValues As List(Of Object)

    Private mFilterProperty As PropertyInfo = Nothing

    Public Overrides Function MatchesFilter(ByVal DataItem As Object) As Boolean

      If FilterValue Is Nothing AndAlso Not Me.UniqueAmongstSiblings Then
        Return True
      Else
        If Me.FilterField IsNot Nothing Then
          If mFilterProperty Is Nothing OrElse mFilterProperty.Name <> Me.FilterField Then
            mFilterProperty = DataItem.GetType.GetProperty(Me.FilterField, BindingFlags.Instance + BindingFlags.Public)
          End If

          If mFilterProperty IsNot Nothing Then
            If Me.UniqueAmongstSiblings AndAlso Me.SiblingValues IsNot Nothing Then
              ' need to ensure that the value is not in our sibling values
              Dim value As Object = mFilterProperty.GetValue(DataItem, Nothing)
              Return Me.SiblingValues.FirstOrDefault(Function(sv) Singular.Misc.CompareSafe(value, sv)) Is Nothing
            ElseIf Me.FilterValue IsNot Nothing Then
              Select Case Me.FilterType
                Case DirectFilterType.Equals
                  Return Singular.Misc.CompareSafe(mFilterProperty.GetValue(DataItem, Nothing), Me.FilterValue)
                Case DirectFilterType.NotEqual
                  Return Not Singular.Misc.CompareSafe(mFilterProperty.GetValue(DataItem, Nothing), Me.FilterValue)
                Case DirectFilterType.Contains
                  Return CStr(mFilterProperty.GetValue(DataItem, Nothing)).Contains(Me.FilterValue)
                Case DirectFilterType.GreaterThan
                  Return Singular.Misc.CompareSafe(mFilterProperty.GetValue(DataItem, Nothing), Me.FilterValue, ">")
                Case DirectFilterType.LessThan
                  Return Singular.Misc.CompareSafe(mFilterProperty.GetValue(DataItem, Nothing), Me.FilterValue, "<")
                Case DirectFilterType.GreaterThanOrEqualTo
                  Return Singular.Misc.CompareSafe(mFilterProperty.GetValue(DataItem, Nothing), Me.FilterValue, ">=")
                Case DirectFilterType.LessThanOrEqualTo
                  Return Singular.Misc.CompareSafe(mFilterProperty.GetValue(DataItem, Nothing), Me.FilterValue, "<=")
              End Select
            Else
              Return True
            End If
          Else
            Return True
          End If
        End If
        Return True
      End If

    End Function

    Public Sub New(ByVal FilterField As String, ByVal FilterValue As Object, ByVal FilterType As DirectFilterType)

      Me.FilterField = FilterField
      Me.FilterValue = FilterValue
      Me.FilterType = FilterType

    End Sub

    Public Sub New(ByVal UniqueAmongstSiblings As Boolean, ByVal FilterField As String, ByVal SiblingValues As List(Of Object))

      Me.FilterField = FilterField
      Me.UniqueAmongstSiblings = UniqueAmongstSiblings
      Me.SiblingValues = SiblingValues

    End Sub

  End Class

  'Public MustInherit Class BusinessRuleAttribute
  '  Inherits System.ComponentModel.DataAnnotations.ValidationAttribute

  'End Class
#If SILVERLIGHT Then

  Public Class PasswordField
    Inherits Attribute

  End Class

#End If

  Public Class TimeField
    Inherits Attribute

    Public Property SingleNumberType As Singular.Misc.TimeStringSingleNumberType
    Public Property TimeFormat As Singular.Misc.TimeFormats
    Public Property CustomTimeFormat As String

    Public Sub New(ByVal SingleNumberType As Singular.Misc.TimeStringSingleNumberType, ByVal TimeFormat As Singular.Misc.TimeFormats)

      Me.SingleNumberType = SingleNumberType
      Me.TimeFormat = TimeFormat

    End Sub

    Public Sub New(ByVal SingleNumberType As Singular.Misc.TimeStringSingleNumberType, ByVal CustomTimeFormat As String)

      Me.SingleNumberType = SingleNumberType
      Me.TimeFormat = Misc.TimeFormats.Custom
      Me.CustomTimeFormat = CustomTimeFormat

    End Sub

    Public Sub New()
      Me.SingleNumberType = Misc.TimeStringSingleNumberType.Hours
      Me.TimeFormat = Misc.TimeFormats.ShortTime
    End Sub

  End Class

  Public Class FilterConditionContext
    Inherits Attribute

    Public Enum DependantFieldLocationType
      WithinObject
      WithinParent
    End Enum

    Public Property NotMatchFilterRuleDescription As String

    Public Property DependantField As String

    Public Property FilterField As String

    Public Property ValueMemberField As String

    Public Property DirectFilterValue As Object

    Public Property DirectFilterType As DirectFilterType = DataAnnotations.DirectFilterType.Equals

    Public Property UniqueAmongstSiblings As Boolean = False
    Public Property UniqueAmongstSiblingFilterField As String
    Public Property SiblingsToIgnore As Object()

    Public Property DependantFieldLocation As DependantFieldLocationType = DependantFieldLocationType.WithinObject

    Public Property UseGetDropDownFilterPredicate As Boolean = False
    Public Property GetDropDownFilterPredicateParameter As String = ""

    Public Function GetTargetObject(ByVal DataItem As Object) As Object

      Dim TargetObject = DataItem
      If TargetObject IsNot Nothing Then
        If Me.DependantFieldLocation = DataAnnotations.FilterConditionContext.DependantFieldLocationType.WithinObject Then
          Return TargetObject
        Else
          If TargetObject.Parent IsNot Nothing AndAlso TargetObject.Parent.Parent IsNot Nothing Then
            Return TargetObject.Parent.Parent
          End If
        End If
      End If
      Return Nothing

    End Function

    Public Function GetDependantProperty(ByVal TargetObject As Object) As PropertyInfo

      Return TargetObject.GetType.GetProperty(Me.DependantField, BindingFlags.Instance + BindingFlags.Public)

    End Function

    Public Function GetFilterCondition(ByVal DataItem As Object) As Singular.DataAnnotations.FilterConditionBase

      Dim FilterConditions As New List(Of FilterConditionBase)

      If Me.UniqueAmongstSiblings Then
        ' need to get the sibling values
        Dim List = DataItem.Parent
        Dim ValueList As New List(Of Object)
        Dim pi As PropertyInfo = Nothing
        If List.Count > 0 Then
          pi = List(0).GetType.GetProperty(Me.ValueMemberField)
          If pi IsNot Nothing Then
            For Each obj As Object In List
              If obj IsNot DataItem AndAlso Not ExistsInIgnoreList(pi.GetValue(obj, Nothing)) Then
                ValueList.Add(pi.GetValue(obj, Nothing))
              End If
            Next
          End If
        End If

        FilterConditions.Add(New Singular.DataAnnotations.FilterCondition(True, Me.UniqueAmongstSiblingFilterField, ValueList))

      End If

      If Me.DirectFilterValue IsNot Nothing Then
        FilterConditions.Add(New Singular.DataAnnotations.FilterCondition(Me.FilterField, Me.DirectFilterValue, Me.DirectFilterType))
      ElseIf Not String.IsNullOrEmpty(Me.DependantField) Then
        Dim TargetObject = Me.GetTargetObject(DataItem)

        If TargetObject IsNot Nothing Then
          Dim DependantProperty = Me.GetDependantProperty(TargetObject)

          If DependantProperty IsNot Nothing Then
            Dim FilterValue As Object = DependantProperty.GetValue(TargetObject, Nothing)

            FilterConditions.Add(New Singular.DataAnnotations.FilterCondition(Me.FilterField, FilterValue, Me.DirectFilterType))
          End If
        End If
      End If

      If FilterConditions.Count = 0 Then
        Return Nothing
      ElseIf FilterConditions.Count = 1 Then
        Return FilterConditions(0)
      Else
        Return New MultiFilterCondition(FilterConditions)
      End If

    End Function

    Private Function ExistsInIgnoreList(obj As Object) As Boolean
      If Me.SiblingsToIgnore IsNot Nothing Then
        Return Me.SiblingsToIgnore.FirstOrDefault(Function(c) Singular.Misc.CompareSafe(obj, SiblingsToIgnore)) IsNot Nothing
      End If
      Return False
    End Function

    Public Sub New(ByVal UseGetDropDownFilterPredicate As Boolean, ByVal GetDropDownFilterPredicateParameter As String)

      Me.UseGetDropDownFilterPredicate = UseGetDropDownFilterPredicate
      Me.GetDropDownFilterPredicateParameter = GetDropDownFilterPredicateParameter

    End Sub

    Public Sub New(ByVal DependantField As String, ByVal FilterField As String)

      Me.DependantField = DependantField
      Me.FilterField = FilterField

    End Sub

    Public Sub New(ByVal FilterField As String, ByVal DirectFilterValue As Object, ByVal DirectFilterType As DirectFilterType)

      Me.FilterField = FilterField
      Me.DirectFilterValue = DirectFilterValue
      Me.DirectFilterType = DirectFilterType

    End Sub

    Public Sub New(ByVal FilterField As String, ByVal DirectFilterValue As Object, ByVal DirectFilterType As DirectFilterType, ByVal UniqueAmongstSibling As Boolean, ByVal ValueMemberField As String, ByVal UniqueAmongstSiblingFilterField As String)

      Me.FilterField = FilterField
      Me.DirectFilterValue = DirectFilterValue
      Me.DirectFilterType = DirectFilterType
      Me.UniqueAmongstSiblings = UniqueAmongstSibling
      Me.UniqueAmongstSiblingFilterField = UniqueAmongstSiblingFilterField
      Me.ValueMemberField = ValueMemberField

    End Sub

    Public Sub New(ByVal DependantField As String, ByVal FilterField As String, ByVal NotMatchFilterRuleDescription As String)

      Me.DependantField = DependantField
      Me.FilterField = FilterField
      Me.NotMatchFilterRuleDescription = NotMatchFilterRuleDescription

    End Sub

    Public Sub New(ByVal DependantFieldLocation As DependantFieldLocationType, ByVal DependantField As String, ByVal FilterField As String)

      Me.DependantFieldLocation = DependantFieldLocation
      Me.DependantField = DependantField
      Me.FilterField = FilterField

    End Sub

    Public Sub New(ByVal DependantFieldLocation As DependantFieldLocationType, ByVal DependantField As String, ByVal FilterField As String, ByVal NotMatchFilterRuleDescription As String)

      Me.DependantFieldLocation = DependantFieldLocation
      Me.DependantField = DependantField
      Me.FilterField = FilterField
      Me.NotMatchFilterRuleDescription = NotMatchFilterRuleDescription

    End Sub

    Public Sub New(ByVal UniqueAmongstSiblings As Boolean, ByVal ValueMemberField As String, ByVal UniqueAmongstSiblingFilterField As String)

      Me.UniqueAmongstSiblings = UniqueAmongstSiblings
      Me.UniqueAmongstSiblingFilterField = UniqueAmongstSiblingFilterField
      Me.ValueMemberField = ValueMemberField

    End Sub

    Public Sub New(ByVal UniqueAmongstSiblings As Boolean, ByVal ValueMemberField As String, ByVal UniqueAmongstSiblingFilterField As String, SiblingsToIgnore As Object())

      Me.UniqueAmongstSiblings = UniqueAmongstSiblings
      Me.UniqueAmongstSiblingFilterField = UniqueAmongstSiblingFilterField
      Me.ValueMemberField = ValueMemberField
      Me.SiblingsToIgnore = SiblingsToIgnore

    End Sub

  End Class

  Public Class BindingContext

    Public Enum BindingLocationType
      ParentContentControlViewModel
      '  ParentViewModel
      ParentList
      ParentUserControl
    End Enum

    Public Property BindingLocation As BindingLocationType

    Public Property PropertyToBind As String

  End Class

  Public Enum DirectFilterType
    Equals
    NotEqual
    Contains
    GreaterThan
    LessThan
    GreaterThanOrEqualTo
    LessThanOrEqualTo
  End Enum

  Public Class DropDownField
    Inherits System.ComponentModel.DataAnnotations.ValidationAttribute

    Public Property BindingContext As BindingContext

    Private mValueMember As String = ""

    Public ReadOnly Property ValueMember As String
      Get
        Return mValueMember
      End Get
    End Property

    Private mSourceType As Type

    ' Private mSourceTypeName As String = ""

    Public Property SourceType As Type
      Get
        Return mSourceType
      End Get
      Set(ByVal value As Type)
        mSourceType = value
      End Set
    End Property

    'Public ReadOnly Property SourceTypeName As String
    '  Get
    '    Return mSourceTypeName
    '  End Get
    'End Property

    Public Property FilterContext As FilterConditionContext

    Public Property DropDownWidth As Integer = -1

    Public Sub New(ByVal SourceType As Type)

      mSourceType = SourceType

    End Sub


    Public Sub New(ByVal SourceType As Type, DropDownWidth As Integer)

      mSourceType = SourceType
      Me.DropDownWidth = DropDownWidth

    End Sub

    'Public Sub New(ByVal SourceTypeName As String)

    '  mSourceTypeName = SourceTypeName

    'End Sub

    Public Sub New(ByVal SourceType As Type, ByVal UseGetDropDownFilterPredicate As Boolean, ByVal GetDropDownFilterPredicateParameter As String)

      mSourceType = SourceType
      Me.FilterContext = New FilterConditionContext(UseGetDropDownFilterPredicate, GetDropDownFilterPredicateParameter)

    End Sub


    ''' <summary>
    ''' Creates a non dependant filter for an exact value
    ''' </summary>
    ''' <param name="SourceType"></param>
    ''' <param name="UniqueAmongstSiblings">Indicates whether the value should be unique in the siblings</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal SourceType As Type, ByVal UniqueAmongstSiblings As Boolean, ByVal ValueMember As String, ByVal UniqueAmongstSiblingFilterField As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(UniqueAmongstSiblings, ValueMember, UniqueAmongstSiblingFilterField)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal UniqueAmongstSiblings As Boolean, ByVal ValueMember As String, ByVal UniqueAmongstSiblingFilterField As String, ByVal SiblingsToIgnore As Object())

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(UniqueAmongstSiblings, ValueMember, UniqueAmongstSiblingFilterField, SiblingsToIgnore)

    End Sub

    ''' <summary>
    ''' Creates a non dependant filter for an exact value
    ''' </summary>
    ''' <param name="SourceType"></param>
    ''' <param name="ValueMember"></param>
    ''' <param name="FilterField"></param>
    ''' <param name="DirectFilterValue">NB: CType this to Object to ensure correct contructer is called</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal SourceType As Type, ByVal ValueMember As String, ByVal FilterField As String, ByVal DirectFilterValue As Object)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(FilterField, DirectFilterValue, DirectFilterType.Equals)

    End Sub

    ''' <summary>
    ''' Creates a non dependant filter for an exact value
    ''' </summary>
    ''' <param name="SourceType"></param>
    ''' <param name="ValueMember"></param>
    ''' <param name="FilterField"></param>
    ''' <param name="DirectFilterValue">NB: CType this to Object to ensure correct contructer is called</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal SourceType As Type, ByVal ValueMember As String, ByVal FilterField As String, ByVal DirectFilterValue As Object, ByVal UniqueAmongstSibling As Boolean, ByVal UniqueAmongstSiblingFilterField As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(FilterField, DirectFilterValue, DirectFilterType.Equals, UniqueAmongstSibling, ValueMember, UniqueAmongstSiblingFilterField)

    End Sub

    ''' <summary>
    ''' Creates a non dependant filter for an exact value
    ''' </summary>
    ''' <param name="SourceType"></param>
    ''' <param name="ValueMember"></param>
    ''' <param name="FilterField"></param>
    ''' <param name="DirectFilterValue">NB: CType this to Object to ensure correct contructer is called</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal SourceType As Type, ByVal ValueMember As String, ByVal FilterField As String, ByVal DirectFilterValue As Object, ByVal DirectFilterType As DirectFilterType)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(FilterField, DirectFilterValue, DirectFilterType)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal ValueMember As String, ByVal DependantField As String, ByVal FilterField As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(DependantField, FilterField)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal ValueMember As String, ByVal DependantFieldLocation As FilterConditionContext.DependantFieldLocationType, ByVal DependantField As String, ByVal FilterField As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(DependantFieldLocation, DependantField, FilterField)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal ValueMember As String, ByVal DependantField As String, ByVal FilterField As String, ByVal InvalidValueDescription As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(DependantField, FilterField, InvalidValueDescription)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal ValueMember As String, ByVal DependantFieldLocation As FilterConditionContext.DependantFieldLocationType, ByVal DependantField As String, ByVal FilterField As String, ByVal InvalidValueDescription As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(DependantFieldLocation, DependantField, FilterField, InvalidValueDescription)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal BindingContextLocation As BindingContext.BindingLocationType, ByVal BindingContextProperty As String)

      mSourceType = SourceType
      Me.BindingContext = New BindingContext() With {.BindingLocation = BindingContextLocation,
                                                     .PropertyToBind = BindingContextProperty}

    End Sub


    Public Sub New(ByVal SourceType As Type, ByVal BindingContextLocation As BindingContext.BindingLocationType, ByVal BindingContextProperty As String, ByVal UniqueAmongstSiblings As Boolean, ByVal ValueMember As String, ByVal FilterField As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.BindingContext = New BindingContext() With {.BindingLocation = BindingContextLocation,
                                                     .PropertyToBind = BindingContextProperty}
      Me.FilterContext = New FilterConditionContext(UniqueAmongstSiblings, ValueMember, FilterField)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal BindingContextLocation As BindingContext.BindingLocationType, ByVal BindingContextProperty As String, ByVal ValueMember As String, ByVal DependantField As String, ByVal FilterField As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.BindingContext = New BindingContext() With {.BindingLocation = BindingContextLocation,
                                                     .PropertyToBind = BindingContextProperty}
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(DependantField, FilterField)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal BindingContextLocation As BindingContext.BindingLocationType, ByVal BindingContextProperty As String, ByVal ValueMember As String, ByVal DependantFieldLocation As FilterConditionContext.DependantFieldLocationType, ByVal DependantField As String, ByVal FilterField As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.BindingContext = New BindingContext() With {.BindingLocation = BindingContextLocation,
                                                     .PropertyToBind = BindingContextProperty}
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(DependantFieldLocation, DependantField, FilterField)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal BindingContextLocation As BindingContext.BindingLocationType, ByVal BindingContextProperty As String, ByVal ValueMember As String, ByVal DependantField As String, ByVal FilterField As String, ByVal InvalidValueDescription As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.BindingContext = New BindingContext() With {.BindingLocation = BindingContextLocation,
                                                     .PropertyToBind = BindingContextProperty}
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(DependantField, FilterField, InvalidValueDescription)

    End Sub

    Public Sub New(ByVal SourceType As Type, ByVal BindingContextLocation As BindingContext.BindingLocationType, ByVal BindingContextProperty As String, ByVal ValueMember As String, ByVal DependantFieldLocation As FilterConditionContext.DependantFieldLocationType, ByVal DependantField As String, ByVal FilterField As String, ByVal InvalidValueDescription As String)

      mSourceType = SourceType
      mValueMember = ValueMember
      Me.BindingContext = New BindingContext() With {.BindingLocation = BindingContextLocation,
                                                     .PropertyToBind = BindingContextProperty}
      Me.FilterContext = New Singular.DataAnnotations.FilterConditionContext(DependantFieldLocation, DependantField, FilterField, InvalidValueDescription)

    End Sub

    Protected Overrides Function IsValid(ByVal value As Object, ByVal validationContext As System.ComponentModel.DataAnnotations.ValidationContext) As System.ComponentModel.DataAnnotations.ValidationResult

#If SILVERLIGHT Then
      If Me.FilterContext Is Nothing OrElse Me.FilterContext.UniqueAmongstSiblings Then
        Return Nothing
      Else
        If validationContext.ObjectInstance IsNot Nothing AndAlso CommonData.Lists IsNot Nothing AndAlso value IsNot Nothing Then
          Dim SourceList = CommonData.GetList(Me.SourceType)

          If SourceList IsNot Nothing Then
            Dim ie As System.Collections.IEnumerable = TryCast(SourceList, System.Collections.IEnumerable)
            If ie IsNot Nothing Then
              Dim FilterCondition = Me.FilterContext.GetFilterCondition(validationContext.ObjectInstance)
              'Emile Added this.
              'It seems like if there is already a value in the dropdown, there will be an error here. Will ask webber to look at it when he gets back
              If FilterCondition IsNot Nothing Then
                Dim FilteredList = (From item In ie
                                    Select item
                                    Where FilterCondition.MatchesFilter(item) And MatchesValue(item, value))


                If FilteredList.Count = 0 Then
                  Dim RuleDescription As String = Me.FilterContext.NotMatchFilterRuleDescription
                  If String.IsNullOrEmpty(RuleDescription) Then
                    RuleDescription = "Invalid Value"
                  End If
                  Return New System.ComponentModel.DataAnnotations.ValidationResult(RuleDescription)
                End If

              End If
            End If
          End If
        End If
        'Return New System.ComponentModel.DataAnnotations.ValidationResult
      End If

#End If

      Return Nothing

    End Function

    Public Function MatchesValue(ByVal item As Object, ByVal Value As Object) As Boolean

      Dim pi = item.GetType.GetProperty(Me.ValueMember)

      If pi IsNot Nothing Then
        Return Singular.Misc.CompareSafe(pi.GetValue(item, Nothing), Value)
      Else
        Return True
      End If

    End Function

  End Class

  Public Enum RestrictDateType
    Min = 1
    Max = 2
  End Enum

  Public Enum AutoChangeType
    None = 0
    StartOfMonth = 1
    EndOfMonth = 2
  End Enum

  Public Class DateField
    Inherits Attribute

    Public Property MinDateProperty As String
    Public Property MaxDateProperty As String

    ''' <summary>
    ''' Display the Date picker inline, so that the user doesn't have to click the input control to get it to pop up.
    ''' </summary>
    Public Property AlwaysShow As Boolean = False

    ''' <summary>
    ''' When the user scrolls between months, must the selected date change to a date in that month.
    ''' </summary>
    Public Property AutoChange As AutoChangeType = AutoChangeType.None

    ''' <summary>
    ''' The number of years to show before / after. e.g: "-20:+0" = 20 years before selected date, "-20:c" = 20 years before current date
    ''' </summary>
    Public Property YearRange As String = ""

    Public Property FormatString As String = ""

    ''' <summary>
    ''' A function / property that will return the initial date to select when there is no date selected. Defaults to today.
    ''' </summary>
    Public Property InitialDateFunction As String
  End Class

  Public Class DocumentField
    Inherits Attribute

    Public Property DisplayNameField As String
    Public Property DocumentClassType As Type

    Public Sub New(ByVal DocumentClassType As Type, ByVal DisplayNameField As String)

      Me.DocumentClassType = DocumentClassType
      Me.DisplayNameField = DisplayNameField

    End Sub

  End Class

  Public Class ColorField
    Inherits Attribute

    Public Sub New()

    End Sub

  End Class

  Public Class ProgressField
    Inherits Attribute

    Public Property VisibleProperty As String
    Public Property StatusTextProperty As String

    Public Sub New(Optional ByVal VisibleProperty As String = "", Optional ByVal StatusTextProperty As String = "")
      Me.VisibleProperty = VisibleProperty
      Me.StatusTextProperty = StatusTextProperty
    End Sub

  End Class

  Public Class FileSizeField
    Inherits Attribute

    Public Sub New()

    End Sub

  End Class

  Public Class RichTextField
    Inherits Attribute

    Public Sub New()

    End Sub

  End Class

  <AttributeUsage(AttributeTargets.Property)>
  Public Class TextField
    Inherits Attribute

    Public Property UpdateOnLeave As Boolean = False

    Public Property WordWrap As Boolean

    Public Property MultiLine As Boolean
    Public Property NoOfLines As Integer = 3 'DO NOT CHANGE THIS, change it on YOUR property.

    Public Property WinFormsUpdateOnTextChange As Boolean = False

    Public Sub New(Optional ByVal MultiLine As Boolean = False, Optional ByVal WordWrap As Boolean = False, Optional ByVal UpdateOnLeave As Boolean = False, Optional ByVal NoOfLines As Integer = 3, Optional WinFormsUpdateOnTextChange As Boolean = False)

      Me.MultiLine = MultiLine
      Me.WordWrap = WordWrap
      Me.UpdateOnLeave = UpdateOnLeave
      Me.NoOfLines = NoOfLines
      Me.WinFormsUpdateOnTextChange = WinFormsUpdateOnTextChange

    End Sub

  End Class

  ''' <summary>
  ''' Update bound property value on each keypress, instead of when focus is lost.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class UpdateOnKeyPress
    Inherits TextField

    Public Sub New()
      UpdateOnLeave = False
    End Sub

  End Class

  ''' <summary>
  ''' Change a string property to render as a multiline editor.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class MultiLine
    Inherits TextField

    Public Sub New()
      MyBase.New(True, False, True)
    End Sub

    Public Sub New(NoOfLines As Integer)
      MyBase.New(True, False, True, NoOfLines)
    End Sub

  End Class

  Public Class Alignment
    Inherits Attribute

    'NOTE: Make sure Singular.Web.Enums.TextAlign matches this enum
    Public Enum HorizontalAlign
      Left = 1
      Center = 2
      Right = 3
      Justify = 4
    End Enum

    Public Sub New(ByVal Align As HorizontalAlign)
      Me.Align = Align
    End Sub

    Public Property Align As HorizontalAlign

  End Class

  Public Class NumberField
    Inherits Attribute

    Public Property FormatString As String = ""
    Public Property FormatStringPropertyName As String = ""
    Public Property CommonNumberFormat As Singular.Formatting.NumberFormatType = Formatting.NumberFormatType.NotSet
    Public Property ShownButtons As Boolean = False
    Public Property UpdateOnLeave As Boolean = False
    Public Shared Property ResolveFormatType As Boolean = True

    ''' <summary>
    ''' The name of the proprerty that contains a currency symbol for use in this numbers format string. Use $parent, or ViewModel. for other locations.
    ''' </summary>
    Public Property CurrencySymbolProperty As String

    ''' <summary>
    ''' The parameters to pass to the numberformat function.
    ''' 0, '' will format with no decimals, and no thousands seperator.
    ''' 2, ',' will format with 2 decimals and comma thousands seperator.
    ''' </summary>
    Public Property WebFormatParameters As String = ""

    Public Sub New(CommonNumberFormat As Singular.Formatting.NumberFormatType)
      Me.CommonNumberFormat = CommonNumberFormat
    End Sub

    Public Sub New(ByVal FormatString As String, Optional ByVal ShownButtons As Boolean = False)
      Me.FormatString = FormatString
      Me.ShownButtons = ShownButtons
    End Sub

    Public Sub New(ByVal FormatString As String, ByVal ShownButtons As Boolean, ByVal UpdateOnLeave As Boolean)
      Me.FormatString = FormatString
      Me.ShownButtons = ShownButtons
      Me.UpdateOnLeave = UpdateOnLeave
    End Sub

    Public ReadOnly Property HasFormat As Boolean
      Get
        Return FormatString <> "" OrElse FormatStringPropertyName <> "" OrElse CommonNumberFormat <> Formatting.NumberFormatType.NotSet
      End Get
    End Property

    Public Function GetFormatString(Optional AddQuotes As Boolean = False) As String

      Dim FmtString As String

      If CommonNumberFormat <> Formatting.NumberFormatType.NotSet Then
        If Singular.DataAnnotations.NumberField.ResolveFormatType Then
          FmtString = Singular.Formatting.GetCurrentFormats.GetNumberFormatString(CommonNumberFormat)
        Else
          Return "NFormats." & CommonNumberFormat.ToString
        End If
      Else
        If FormatStringPropertyName = "" Then
          FmtString = FormatString
        Else
          Return FormatStringPropertyName
        End If
      End If

      If AddQuotes Then
        FmtString = FmtString.AddSingleQuotes
      End If

      If Not String.IsNullOrEmpty(CurrencySymbolProperty) Then
        Return FmtString.Replace(Singular.Formatting.GetCurrentFormats.CurrencySymbol, "{0}")
      Else
        Return FmtString
      End If

    End Function

  End Class

  Public Class EmailField
    Inherits ValidationAttribute

    Protected Overrides Function IsValid(ByVal value As Object, ByVal validationContext As ValidationContext) As ValidationResult
      Dim str As String = value
      If str = "" Then
        Return ValidationResult.Success
      Else
        If Singular.Emails.ValidEmailAddress(value) Then
          Return ValidationResult.Success
        Else
          Return New ValidationResult("Invalid Email Address")
        End If
      End If
    End Function

  End Class

#If SILVERLIGHT Then
#Else

  Public Class RoundedNumber
    Inherits ValidationAttribute

    Public Property DecimalPlaces As Integer = 2

    Protected Overrides Function IsValid(ByVal value As Object, ByVal validationContext As ValidationContext) As ValidationResult

      If Math.Round(CType(value, Decimal), DecimalPlaces, MidpointRounding.AwayFromZero) <> value Then
        Return New ValidationResult(String.Format("Number can only have {0} decimal places.", DecimalPlaces))
      Else
        Return ValidationResult.Success
      End If

    End Function

  End Class
#End If


  Public Class SearchField
    Inherits Attribute

    Public Property ObjectListType As Type
    Public Property ObjectType As Type
    Public Property ObjectListCriteriaType As Type
    Public Property ValueMemberPath As String
    Public Property DisplayMemberPath As String
    Public Property ObjectMemberPath As String
    Public Property PreserveSearchContext As Boolean
    Public Property WindowTitle As String

    Public Sub New(ByVal ObjectListType As Type, ByVal ObjectType As Type, ByVal ObjectListCriteriaType As Type, ByVal ValueMemberPath As String, ByVal DisplayMemberPath As String, ByVal ObjectMemberPath As String,
                   Optional ByVal PreserveSearchContext As Boolean = False, Optional ByVal WindowTitle As String = "")
      Me.ObjectListType = ObjectListType
      Me.ObjectType = ObjectType
      Me.ObjectListCriteriaType = ObjectListCriteriaType
      Me.ValueMemberPath = ValueMemberPath
      Me.DisplayMemberPath = DisplayMemberPath
      Me.ObjectMemberPath = ObjectMemberPath
      Me.PreserveSearchContext = PreserveSearchContext
      Me.WindowTitle = WindowTitle
    End Sub

  End Class

  ''' <summary>
  ''' Marks a field to indicate that the value should not 
  ''' be recursed for determining all broken rules
  ''' </summary>
  <AttributeUsage(AttributeTargets.Property)>
  Public NotInheritable Class NoBrokenRules
    Inherits Attribute

    Public Sub New()
    End Sub

  End Class

  ''' <summary>
  ''' Marks a property as a primary VM property.
  ''' Can be applied to multiple properties. These properties will be the only properties checked for IsDirty / IsValid / BrokenRules.
  ''' </summary>
  ''' <remarks></remarks>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class PrimaryProperty
    Inherits Attribute

  End Class

  Public Class ImageField
    Inherits Attribute

    Public ColumnFileByteData As String
    Public Sub New(ByVal ColumnFileByteData As String)

      Me.ColumnFileByteData = ColumnFileByteData

    End Sub

  End Class

  <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Field)>
  Public Class LocalisedDisplay
    Inherits Attribute

    Private mObservableResourceType As Type
    Private mResourceName As String

    Public ReadOnly Property ResourceName As String
      Get
        Return mResourceName
      End Get
    End Property

    Public Property DescriptionKey As String

    Private Shared ObservableResourceList As New List(Of Object)

    Public Function GetObservableResource() As Object

      Dim obs = ObservableResourceList.FirstOrDefault(Function(o) o.GetType.Equals(mObservableResourceType))

      If obs Is Nothing Then
        obs = Activator.CreateInstance(mObservableResourceType)
        ObservableResourceList.Add(obs)
      End If

      Return obs

    End Function

    Public Sub New(ByVal ObservableResourceType As Type, ByVal ResourceName As String)

      mObservableResourceType = ObservableResourceType
      mResourceName = ResourceName

    End Sub

#If SILVERLIGHT = False Then

    Public Sub New(ByVal ResourceName As String)

      mResourceName = ResourceName

    End Sub

    Public Sub New(ByVal NameKey As String, DescriptionKey As String)

      mResourceName = NameKey
      Me.DescriptionKey = DescriptionKey

    End Sub

#End If

  End Class

  ''' <summary>
  ''' Marks a property in a criteria object as the primary field. Enables search when typing in a field without showing the full find screen criteria.
  ''' </summary>
  ''' <remarks></remarks>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class PrimarySearchField
    Inherits Attribute

  End Class

#End Region

#Region " Web Specific "

#If SILVERLIGHT Then

  Public Class DropDownWeb
    Inherits Attribute

    Public Enum SelectType
      Auto = 0
      NormalDropDown = 1
      FindScreen = 2
      Combo = 3
    End Enum

    Public Enum SourceType
      None = 0
      CommonData = 1
      SessionData = 2
      ViewModel = 4
      TempData = 8
      Fetch = 16
      All = 31
      IgnoreMissing = 32
    End Enum

    Public Property ValueMember As String
    Public Property DisplayMember As String
    Public Property DropDownColumns As String() = {}

    Public Property DropDownType As SelectType
    Public Property AutoSetProperties As Object
    Public Property AutoSetFromProperties As Object

    Public Property Source As SourceType
    Public Property ThisFilterMember As String = ""
    Public Property DataSourceFilterMember As String = ""
    Public Property UniqueInList As Boolean = False
    Public Property FilterMethodName As String = ""

    Public Sub New(ListType As Type)

    End Sub

    Public Sub New(ListType As Type, Context As String)

    End Sub

    Public Sub New(ClientSource As String)

    End Sub

  End Class

#Else

  <Serializable(), AttributeUsage(AttributeTargets.Property)>
  Public Class DropDownWeb
    Inherits Attribute

    <Flags()> _
    Public Enum SourceType
      ''' <summary>
      ''' Doesnt Look Anywhere, Used when passing in an enum for example.
      ''' </summary>
      None = 0
      ''' <summary>
      ''' Searches Commondata.Lists
      ''' </summary>
      CommonData = 1
      ''' <summary>
      ''' Searches Commondata.SessionLists
      ''' </summary>
      SessionData = 2
      ''' <summary>
      ''' Searches ViewModel of the current Page.
      ''' </summary>
      ViewModel = 4
      ''' <summary>
      ''' Searches Commondata.TempLists
      ''' </summary>
      TempData = 8
      ''' <summary>
      ''' Fetches the list from the database
      ''' </summary>
      Fetch = 16
      ''' <summary>
      ''' Searches All Locations.
      ''' </summary>
      All = 31

      IgnoreMissing = 32
    End Enum

    Public Enum SelectType
      Auto = 0
      NormalDropDown = 1
      FindScreen = 2
      Combo = 3
      ''' <summary>
      ''' Same as find screen, but without the find button.
      ''' </summary>
      AutoComplete = 4
      ''' <summary>
      ''' Normal combo, but the input is readonly.
      ''' </summary>
      DropDownList = 5
    End Enum

    Public Sub New(ClientSource As String)
      mClientDatasourceName = ClientSource
      mName = ClientSource
    End Sub

    Private mListChildType As Type
    Private mValueMemberPI As PropertyInfo = Nothing
    Private mHasCheckedValueMember As Boolean = False

    Public Sub New(ListType As Type)
      Setup(ListType, "")
    End Sub

    Public Sub New(ListType As Type, Context As String)
      Setup(ListType, Context)
    End Sub

    Private Sub Setup(ListOrCriteriaType As Type, Context As String)
      ListType = ListOrCriteriaType

      If Singular.Reflection.IsDerivedFromGenericType(ListType, GetType(Csla.CriteriaBase(Of ))) Then
        'Criteria type was given
        mCriteriaClass = ListType
        ListType = mCriteriaClass.DeclaringType

      End If

      mListChildType = Singular.ReflectionCached.GetCachedType(ListType).LastGenericType

      'Special Case For Enumerations.
      If ListType.IsEnum Then
        Me.Source = SourceType.None
        Me.ValueMember = "Val"
        Me.DisplayMember = "Text"
      Else

        'Guess the value and display member.
        If Singular.Reflection.TypeImplementsInterface(ListType, GetType(Singular.ISingularListBase)) Then

          Dim ddi = GetValueDisplayMember(ListType, Context)
          If ddi IsNot Nothing Then

            DisplayMember = ddi.DisplayMember
            ValueMember = ddi.ValueMember

            If TypeOf ddi Is Singular.WebDropDownInfo Then
              GroupMember = CType(ddi, Singular.WebDropDownInfo).GroupMember
              GroupChildListMember = CType(ddi, Singular.WebDropDownInfo).GroupChildListMember
            End If

          End If

        End If

      End If

    End Sub

    Public Shared Function GetValueDisplayMember(ListType As Type, Context As String) As DropDownInfo
      Dim mi As MethodInfo = ListType.GetMethod("GetDropDownInfo", BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy)
      If mi IsNot Nothing Then
        Return mi.Invoke(Nothing, New Object() {Context})
      End If
      Return Nothing
    End Function

#Region " Properties "

    Public Property ListType As Type
    Public Property Source As SourceType = SourceType.All

    Private mValueMember As String
    Public Property ValueMember As String
      Get
        Return If(mValueMember Is Nothing, "Val", mValueMember)
      End Get
      Set(value As String)
        mValueMember = value
      End Set
    End Property

    Private mDisplayMember As String
    Public Property DisplayMember As String
      Get
        Return If(mDisplayMember Is Nothing, "Val", mDisplayMember)
      End Get
      Set(value As String)
        mDisplayMember = value
      End Set
    End Property

    ''' <summary>
    ''' For find type drop downs, where there could be thousands of items to lookup, the display value can be stored in the object as well as the lookup id.
    ''' </summary>
    Public Property LookupMember As String = ""

    ''' <summary>
    ''' The names of the properties to appear as columns in the drop down. By default this is the Display Member. 
    ''' If specified, the display member will still be displayed when the combo is not dropped down.
    ''' </summary>
    Public Property DropDownColumns As String() = {}

    Private _DisplayFunction As String

    ''' <summary>
    ''' Allows a calculated display member. E.g. set the value to "function(Item){ return Item.FirstName + ' ' + Item.LastName }"
    ''' You can also use Item.FirstName + ' ' + Item.LastName directly in this property.
    ''' </summary>
    Public Property DisplayFunction As String
      Get
        Return _DisplayFunction
      End Get
      Set(value As String)
        If value.Contains(" ") AndAlso Not value.StartsWith("function") Then 'String to call a function will not have a space
          _DisplayFunction = "function(Item){ return Item ? (" & value & ") : ''; }"
        Else
          _DisplayFunction = value
        End If
      End Set
    End Property

    Public Property GroupMember As String
    Public Property GroupChildListMember As String
    Public Property ThisFilterMember As String = ""
    Public Property FilterConstant As Object
    Public Property FilterMethodName As String = ""
    Public Property AutoSetProperties As String() = {}
    Public Property AutoSetFromProperties As String() = {}
    Public Property UniqueInList As Boolean = False
    Public Property AjaxFetch As Boolean = False
    Public Property DropDownCssClass As String = ""

    ''' <summary>
    ''' The name of a javascript function to call when each row / cell is created.
    ''' </summary>
    Public Property OnCellCreateFunction As String = ""

    ''' <summary>
    ''' Javascipt code to run after Ajax data has been fetched.
    ''' </summary>
    Public Property AfterFetchJS As String = ""

    ''' <summary>
    ''' Javascript function name to run as the user clicks a find drop down, before the find screen is shown. Function(Args: { AutoPopulate, Object, Criteria })
    ''' </summary>
    Public Property PreFindJSFunction As String = ""

    ''' <summary>
    ''' Javascipt code to run before Ajax data is fetched. E.g. as the user clicks search, or when they type in an autocomplete combo. Function (Args.Data, BoundObject)
    ''' </summary>
    Public Property BeforeFetchJS As String

    ''' <summary>
    ''' Javascript function to run when a user selects an item in a drop down or find screen. Function(SelectedItem, BoundObject)
    ''' </summary>
    Public Property OnItemSelectJSFunction As String = ""

    ''' <summary>
    ''' The first item in the list to prompt the user to select something. E.g. Select Item...
    ''' </summary>
    Public Property UnselectedText As String = " "
    Public Property AllowBlank As Boolean = True
    Public Property ComboDeselectText As String = ""

    ''' <summary>
    ''' timeout to use when handling key presses
    ''' </summary>
    Public Property KeyDelay As Integer = 500


    ''' <summary>
    ''' If the list has items that should be filtered out because they are old or inactive, specify the property that marks the item as old.
    ''' Specify 2 items if this is a grouped options list.
    ''' </summary>
    Public Property OldFilterProperties As String() = {}

    Private mDataSourceFilterMember As String = ""

    Public Property DataSourceFilterMember As String
      Get
        If mDataSourceFilterMember = "" Then
          Return ThisFilterMember
        Else
          Return mDataSourceFilterMember
        End If
      End Get
      Set(value As String)
        mDataSourceFilterMember = value
      End Set
    End Property

    Private mClientDatasourceName As String = ""
    Private mName As String = ""
    ''' <summary>
    ''' Name to be used in Client Data Provider, if Blank, the list type name will be used.
    ''' </summary>
    Public Property Name As String
      Get
        If mName = "" Then
          Return ListType.Name
        Else
          Return mName
        End If
      End Get
      Set(value As String)
        mName = value
      End Set
    End Property

    Public Function GetPropertyName() As String
      Return mName
    End Function

    Public ReadOnly Property ClientName As String
      Get
        If mClientDatasourceName = "" Then
          Return "ClientData." & Name
        Else
          If mClientDatasourceName.EndsWith(")") Then
            Return mClientDatasourceName
          Else
            If mClientDatasourceName.Contains("ClientData") Then
              Return mClientDatasourceName
            Else
              Return mClientDatasourceName & "()"
            End If

          End If
        End If
      End Get
    End Property

    Private mCriteriaClass As Type
    Private mSearchedForCriteriaType As Boolean = False
    Public Function GetCriteriaClass(Optional SetDefault As Boolean = True) As Type
      If ListType IsNot Nothing AndAlso mCriteriaClass Is Nothing AndAlso SetDefault Then
        If Not mSearchedForCriteriaType Then
          mSearchedForCriteriaType = True
          mCriteriaClass = ListType.GetNestedType("Criteria", BindingFlags.Public Or BindingFlags.NonPublic)
        End If
      End If
      Return mCriteriaClass
    End Function

    Private mPrimarySearchField As PropertyInfo
    Private mSearchFieldCount As Integer = 0
    Private mLookedForPrimarySearchField As Boolean
    Public Function GetPrimarySearchField() As Tuple(Of PropertyInfo, Integer)
      If mPrimarySearchField Is Nothing AndAlso Not mLookedForPrimarySearchField Then
        mLookedForPrimarySearchField = True

        Dim Crit = GetCriteriaClass()
        If Crit IsNot Nothing Then
          For Each pi As PropertyInfo In Crit.GetProperties(BindingFlags.Instance Or BindingFlags.Public)
            If Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.PrimarySearchField)(pi) IsNot Nothing Then
              mPrimarySearchField = pi
            End If
            If Singular.ReflectionCached.AutoGenerateField(pi) Then
              mSearchFieldCount += 1
            End If
          Next
        End If

      End If
      Return Tuple.Create(mPrimarySearchField, mSearchFieldCount)
    End Function

    Public Property DropDownType As SelectType = SelectType.Auto

    Public Property TooltipMember As String
    Public Property AllowNotInList As Boolean

    Private mIsMultiSelect As Boolean = False
    Public Sub SetMultiSelect(Value As Boolean)
      mIsMultiSelect = Value
    End Sub

    Public Function AddsCustomData() As Boolean
      'For find screens, the data added is the list of items with ids in the page data
      Return DropDownType = SelectType.FindScreen AndAlso Not mIsMultiSelect
    End Function

#End Region

    Public Function AddAutoSetToModel() As Boolean
      Return AutoSetProperties.Length > 0 AndAlso LookupMember = ""
    End Function

    Public Function GetAutoSetFromProperty(ByVal Index As Integer)
      If AutoSetFromProperties.Length = 0 Then
        Return AutoSetProperties(Index)
      Else
        Return AutoSetFromProperties(Index)
      End If
    End Function

    Public Function GetDisplayMember() As String
      If DisplayFunction IsNot Nothing Then
        Return DisplayFunction
      Else
        Return DisplayMember.AddSingleQuotes
      End If
    End Function

    Public Function GetValueMemberPI() As PropertyInfo
      'check that the Value Member of the Lookup List is browsable.
      If mValueMemberPI Is Nothing Then
        If mValueMember IsNot Nothing Then
          mValueMemberPI = Singular.Reflection.GetProperty(mListChildType, mValueMember)
        End If
        If GroupChildListMember = "" Then
          If mValueMemberPI Is Nothing Then
            Throw New Exception("Value Member '" & mValueMember & "' Not Found on Type '" & mListChildType.Name & "'.")
          ElseIf Not Singular.ReflectionCached.SerlialiseField(mValueMemberPI, True) Then
            Throw New Exception("Value Member '" & mValueMember & "' is marked as non browsable on Type '" & mListChildType.Name & "'.")
          End If
        End If
      End If
      Return mValueMemberPI
    End Function

    Public Class DataSourceInfo
      Public Data As Object
      Public RetreivedFrom As SourceType

      Public Sub New(Data As Object, RetrievedFrom As SourceType)
        Me.Data = Data
        Me.RetreivedFrom = RetrievedFrom
      End Sub
    End Class

    Public Shared Function GetDataSource(ListType As Type, PropertyName As String, Source As SourceType, ViewModel As Object) As DataSourceInfo

      Dim CriteriaType As Type = Nothing
      If ListType IsNot Nothing Then
        Singular.Reflection.ResolveCriteriaType(ListType, ListType, CriteriaType)
      End If

      'For Enum types, just return the type.
      If ListType IsNot Nothing AndAlso ListType.IsEnum Then
        Return New DataSourceInfo(ListType, SourceType.None)
      Else

        'Set this so it doesn't throw an exception.
        Singular.CommonData.SetContextDisallowedMode(CommonData.ContextDisallowedModeType.ReturnNull)
        Dim ReturnList As Object = Nothing

        If Source = (Source Or SourceType.CommonData) Then
          'Application
          ReturnList = Singular.CommonData.GetList(PropertyName, ListType)
          If ReturnList IsNot Nothing Then
            Return New DataSourceInfo(ReturnList, SourceType.CommonData)
          End If
        End If

        If Source = (Source Or SourceType.SessionData) Then
          'Session
          If Singular.CommonData.SessionLists IsNot Nothing Then
            ReturnList = Singular.CommonData.GetSessionList(PropertyName, ListType)
            If ReturnList IsNot Nothing Then
              Return New DataSourceInfo(ReturnList, SourceType.SessionData)
            End If
          End If
        End If

        If Source = (Source Or SourceType.TempData) Then
          'Temp Lists
          ReturnList = Singular.CommonData.GetTempList(PropertyName, ListType)
          If ReturnList IsNot Nothing Then
            Return New DataSourceInfo(ReturnList, SourceType.TempData)
          End If
        End If

        Singular.CommonData.SetContextDisallowedMode(CommonData.ContextDisallowedModeType.PreviousValue)

        If ViewModel IsNot Nothing AndAlso Source = (Source Or SourceType.ViewModel) Then

          'Look for a property that matches the drop down list type.
          Dim pi = Singular.Reflection.GetProperty(ViewModel.GetType, ListType)
          If pi IsNot Nothing Then
            ReturnList = pi.GetValue(ViewModel, Nothing)
            If ReturnList IsNot Nothing Then
              Return New DataSourceInfo(ReturnList, SourceType.ViewModel)
            End If
          End If

        End If

        If Source = SourceType.Fetch Then

          'Dim mi As MethodInfo = ListType.GetMethod("Get" & ListType.Name, BindingFlags.Public Or BindingFlags.Static)
          'If mi Is Nothing OrElse Singular.Reflection.MethodHasParameters(mi) Then
          '  Throw New Exception("Parameterless Get" & ListType.Name & " not found on type " & ListType.Name)
          'Else
          '  ReturnList = Singular.Reflection.InvokeMethod(mi, Nothing)
          'End If
          ReturnList = Singular.Reflection.FetchList(If(CriteriaType Is Nothing, ListType, CriteriaType), Nothing)
          If ReturnList IsNot Nothing Then
            Return New DataSourceInfo(ReturnList, SourceType.Fetch)
          End If

        End If

        If ReturnList Is Nothing AndAlso Source <> SourceType.None AndAlso Not Source = (Source Or SourceType.IgnoreMissing) AndAlso Singular.Debug.InDebugMode Then
          Throw New Exception("Can't find data for drop down of type: '" & ListType.Name & "'. Did you forget to add it to commondata?")
        End If

      End If '''''''''''''''''''''''''''''''''''''''''''''''
      Return Nothing
    End Function

    Public Function GetDisplayFromID(ID As Object, Optional Obj As Object = Nothing)

      If LookupMember = "" Then

        If ID Is Nothing OrElse ID Is DBNull.Value Then
          Return UnselectedText
        Else
          If ListType.IsEnum Then
            Return Singular.Reflection.GetEnumDisplayName(ID)
          End If
          Dim sourceInfo = GetDataSource(GetCriteriaClass, mName, Source, Nothing)
          If sourceInfo IsNot Nothing Then
            Dim List As IList = sourceInfo.Data
            If List IsNot Nothing Then
              Dim gtMi = List.GetType.GetMethod("GetItem", BindingFlags.Public Or BindingFlags.Instance)
              If gtMi IsNot Nothing Then
                Try
                  Dim LookupObject = gtMi.Invoke(List, {ID})
                  Dim pi = LookupObject.GetType.GetProperty(mDisplayMember, BindingFlags.Public Or BindingFlags.Instance)
                  Return pi.GetValue(LookupObject, Nothing)
                Catch ex As Exception
                End Try
              End If
            Else
              Return ""
            End If
          End If
        End If

      Else

        If Obj IsNot Nothing Then
          Return CallByName(Obj, LookupMember, CallType.Get)
        End If

      End If

      Return ""
    End Function

    Public ReadOnly Property FindContextKey As String
      Get
        Return ListType.Name & "Select"
      End Get
    End Property

  End Class

  Public Class AddOnTheFly
    Inherits Attribute

    '         ((((c,               ,7))))
    '        (((((((              ))))))))
    '         (((((((            ))))))))
    '          ((((((@@@@@@@@@@@))))))))
    '           @@@@@@@@@@@@@@@@)))))))
    '        @@@@@@@@@@@@@@@@@@))))))@@@@
    '       @@/,:::,\/,:::,\@@@@@@@@@@@@@@
    '       @@|:::::||:::::|@@@@@@@@@@@@@@@
    '       @@\':::'/\':::'/@@@@@@@@@@@@@@
    '        @@@@@@@@@@@@@@@@@@@@@@@@@@@
    '          @@@@@@@@@@@@@@@@@@@@@@\
    '             /    \        (     \
    '            (      )        \     \
    '             \    /          \

    Public ReadOnly Property NewType As Type
      Get
        Return mNewType
      End Get
    End Property

    Public ReadOnly Property PromptText As String
      Get
        Return mPromptText
      End Get
    End Property

    Public ReadOnly Property VMPropertyName As String
      Get
        Return mVMPropertyName
      End Get
    End Property

    Public ReadOnly Property AfterAddFunctionName As String
      Get
        Return mAfterAddFunctionName
      End Get
    End Property

    Private mNewType As Type
    Private mPromptText As String
    Private mVMPropertyName As String
    Private mAfterAddFunctionName As String

    Public Sub New(NewType As Type, PromptText As String, Optional AfterAddFunctionName As String = "")
      mNewType = NewType
      mPromptText = PromptText
      mVMPropertyName = "AOTF_" & mNewType.FullName.Replace(".", "")
      mAfterAddFunctionName = AfterAddFunctionName
    End Sub

  End Class

  ''' <summary>
  ''' Specifies that this property must be displayed as a list of radio buttons from the supplied enumeration.
  ''' </summary>
  Public Class RadioButtonList
    Inherits Attribute

    Public Property DataSourceType As Type

    Public Property Horizontal As Boolean

    ''' <summary>
    ''' Editor for this property will be rendered as a radio button list. 
    ''' </summary>
    ''' <param name="DataSourceType">Type can be an enum, or a list type of a list in commondata.</param>
    Public Sub New(DataSourceType As Type)
      Me.DataSourceType = DataSourceType
    End Sub

  End Class

  Public Class AutoPostBack
    Inherits Attribute

    Public Property CommandName As String


  End Class

  Public Class FileInput
    Inherits Attribute
  End Class


  Public Class ExpandOptions
    Inherits Attribute

    Public Enum RenderChildrenModeType
      ''' <summary>
      ''' Renders the child elements when the parent element is rendered. Slower initial rendering, faster expand.
      ''' </summary>
      OnParentRender = 1
      ''' <summary>
      ''' Renders the child elements when they are expanded. Faster initial rendering, slower expand.
      ''' </summary>
      ''' <remarks></remarks>
      OnExpand = 2

    End Enum

    Public Property RenderChildrenMode As RenderChildrenModeType = RenderChildrenModeType.OnParentRender

    Public Sub New(RenderChildrenMode As RenderChildrenModeType)
      Me.RenderChildrenMode = RenderChildrenMode
    End Sub

  End Class

  ''' <summary>
  ''' When applied on a property, indicates that this property must not be html encoded. e.g. dont change " to $ quot;
  ''' </summary>
  Public Class HtmlProperty
    Inherits Attribute
  End Class

  Public Class BrowsableConditional
    Inherits Attribute

    Public Property AllowedContext As String

    Public Sub New(AllowedContext As String)
      Me.AllowedContext = AllowedContext
    End Sub

  End Class

  ''' <summary>
  ''' NOTE: Please use the AddBusinessRules() function in your business object. This is only here for old CSLA objects.
  ''' Defines a custom JavaScript validation rule to be run when this property changes.
  ''' Rule function must have these parameters: function (Value, Rule, CtlError)
  ''' </summary>
  ''' <remarks></remarks>
  Public Class JSRule
    Inherits Attribute

    ''' <summary>
    ''' Custom Rule Name, defaults to the property name
    ''' </summary>
    Public Property RuleName As String = ""

    ''' <summary>
    ''' Name of a JavaScript function which contains the rule logic.
    ''' </summary>
    Public Property FunctionName As String = ""

    ''' <summary>
    ''' Actual JavaScript snippet which contains the rule logic.
    ''' </summary>
    Public Property JavascriptCode As String = ""

    ''' <summary>
    ''' Other Properties that should trigger this rule. Can use $parent or $root
    ''' </summary>
    Public Property OtherTriggerProperties As String() = {}

    Public ReadOnly Property HasLogic As Boolean
      Get
        Return JavascriptCode <> "" OrElse FunctionName <> ""
      End Get
    End Property

    Public Function GetRuleName(pi As PropertyInfo) As String
      If RuleName = "" Then
        Return pi.Name & "DARule"
      Else
        Return RuleName
      End If
    End Function

    Public Function GetTriggerProperties(ThisProperty As String) As String
      Dim Triggers As String = ThisProperty
      For Each tp As String In OtherTriggerProperties
        Singular.Strings.Delimit(Triggers, tp)
      Next
      Return Triggers
    End Function

  End Class

  Public Enum RequiredCondition
    Visible = 1
    Enabled = 2
  End Enum

  Public Class RequiredIf
    Inherits System.ComponentModel.DataAnnotations.RequiredAttribute

    Public Property Condition As RequiredCondition = RequiredCondition.Visible Or RequiredCondition.Enabled

    ''' <summary>
    ''' JavaScript function that returns true or false.
    ''' </summary>
    Public Property ConditionLogicJS As String = ""

    Public Function GetRuleArgument() As String
      If ConditionLogicJS = "" Then
        Return CInt(Condition)
      Else
        Return "function(){ return " & ConditionLogicJS & "}"
      End If
    End Function

    Public Overrides Function IsValid(value As Object) As Boolean
      'The logic cannot be executed on the server, so this will only validate on the client
      Return True
    End Function

    Public Sub New(ConditionLogicJS As String)
      Me.ConditionLogicJS = ConditionLogicJS
    End Sub

    Public Sub New()

    End Sub

  End Class

  ''' <summary>
  ''' When attached to a Class, any properties returning this class type will not be serialised into JavaScript.
  ''' </summary>
  ''' <remarks></remarks>
  <AttributeUsage(AttributeTargets.Class)>
  Public Class ServerOnly
    Inherits Attribute
  End Class

  ''' <summary>
  ''' Specifies if a list can add / remove / update. Put this attribute on your List class.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Class)>
  Public Class AllowAddRemoveEdit
    Inherits Attribute

    Public Property AllowAdd As Boolean?
    Public Property AllowRemove As Boolean?
    Public Property AllowEdit As Boolean?

    Public Sub New(AllowAdd As Boolean, AllowRemove As Boolean, AllowEdit As Boolean)
      Me.AllowAdd = AllowAdd
      Me.AllowRemove = AllowRemove
      Me.AllowEdit = AllowEdit
    End Sub

    Public Shared Function AllowsAdd(ListType As Type, OriginalValue As Boolean) As Boolean
      Dim atr = Singular.Reflection.GetAttribute(Of AllowAddRemoveEdit)(ListType)
      If atr IsNot Nothing AndAlso atr.AllowAdd.HasValue Then
        Return atr.AllowAdd
      Else
        Return OriginalValue
      End If
    End Function

    Public Shared Function AllowsRemove(ListType As Type, OriginalValue As Boolean) As Boolean
      Dim atr = Singular.Reflection.GetAttribute(Of AllowAddRemoveEdit)(ListType)
      If atr IsNot Nothing AndAlso atr.AllowRemove.HasValue Then
        Return atr.AllowRemove
      Else
        Return OriginalValue
      End If
    End Function

  End Class

  ''' <summary>
  ''' Specifies the javascript code to run client side when this property is changed.
  ''' Use self to refer to current object.
  ''' </summary>
  ''' <remarks></remarks>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class SetExpression
    Inherits Attribute

    Public Property JavascriptCode As String
    Public Property BeforeChange As Boolean = False
    Public Property DelayMS As Integer = 0

    Public Sub New(JavascriptCode As String, Optional BeforeChange As Boolean = False, Optional DelayMS As Integer = 0)
      Me.JavascriptCode = JavascriptCode
      Me.BeforeChange = BeforeChange
      Me.DelayMS = DelayMS
    End Sub

    Public Sub Write(jw As Singular.Web.Utilities.JavaScriptWriter)
      jw.Write(Me.JavascriptCode)
    End Sub

  End Class

  <AttributeUsage(AttributeTargets.Property)>
  Public Class SetExpressionBeforeChange
    Inherits Attribute

    Public Property JavascriptCode As String

    Public Sub New(JavascriptCode As String)
      Me.JavascriptCode = JavascriptCode
    End Sub

    Public Sub Write(jw As Singular.Web.Utilities.JavaScriptWriter)
      jw.Write(Me.JavascriptCode)
    End Sub

  End Class

  ''' <summary>
  ''' Specifies that this property should be converted into javascript as a statement, rather than serialising the return value of the property.
  ''' If Javascript Statement is left blank, the property get method will be decompiled into javascript.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Method), Obsolete("Decompilation does not work properly with the VS2017 compiler.")>
  Public Class ComputedProperty
    Inherits Attribute

    Public Property JavascriptStatement As String

    Public Property SupportsJS As Boolean = True
    Public Property SupportsExcel As Boolean = False

  End Class

  ''' <summary>
  ''' Specifies that this list contains items that are different types. Used when you have different objects inheriting from the same base type.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Class)>
  Public Class VariableContents
    Inherits Attribute
  End Class

  ''' <summary>
  ''' If an object can be inherited, this attribute can be placed on the base object, to get the actual type in a certain context.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Class Or AttributeTargets.Interface)>
  Public Class ResolveType
    Inherits Attribute

    Private mResolverType As Type
    Private Shared mInstances As New Hashtable
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ResolverType">ResolverType must be a class that inherits from Singular.DataAnnotations.TypeResolver</param>
    ''' <remarks></remarks>
    Public Sub New(ResolverType As Type)
      mResolverType = ResolverType
    End Sub

    Public Function GetActualType(Instance As Object, Parent As Object, Serialiser As Web.Data.JS.JSSerialiser) As Type
      Dim rt = mInstances(mResolverType.FullName)
      If rt Is Nothing Then
        rt = Activator.CreateInstance(mResolverType, True)
        mInstances(mResolverType.FullName) = rt
      End If
      Return CType(rt, TypeResolver).GetActualType(Instance, Parent, Serialiser)
    End Function

  End Class

  Public MustInherit Class TypeResolver

    Public MustOverride Function GetActualType(Instance As Object, Parent As Object, Serialiser As Web.Data.JS.JSSerialiser) As Type

  End Class

  ''' <summary>
  ''' Indicates that this is a client only property.
  ''' This property will not be serialised back to the server.
  ''' This property will not trigger a save in Immediate Save mode.
  ''' </summary>
  ''' <remarks></remarks>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class ClientOnly
    Inherits Attribute

    Public Enum DataOptionType
      ''' <summary>
      ''' Data will be sent to the client on initial load and postbacks.
      ''' </summary>
      SendDataToClient = 1
      ''' <summary>
      ''' Data will never be sent to the client.
      ''' </summary>
      DontSendData = 2
      ''' <summary>
      ''' Data will only be sent to the client on the inital load of the object, not on postbacks
      ''' </summary>
      InitalDataOnly = 3
    End Enum

    Public Property Value As Boolean = True

    ''' <summary>
    ''' When true, data will be sent to the client, but not back to the server.
    ''' When false, this property will not be included in any JSON serialisation. 
    ''' </summary>
    Public Property DataOption As DataOptionType = DataOptionType.SendDataToClient

    Public Sub New(Value As Boolean)
      Me.Value = Value
    End Sub

    Public Sub New(DataOption As DataOptionType)
      Me.DataOption = DataOption
    End Sub

    Public Sub New()

    End Sub
  End Class

  ''' <summary>
  ''' Indicates that this is a client only property.
  ''' This property will not be serialised into JSON, only included as a property in the Model.
  ''' </summary>
  Public Class ClientOnlyNoData
    Inherits ClientOnly

    Public Sub New()
      MyBase.New(DataOptionType.DontSendData)
    End Sub
  End Class

  Public Class InitialDataOnly
    Inherits ClientOnly

    Public Sub New()
      MyBase.New(DataOptionType.InitalDataOnly)
    End Sub
  End Class

  ''' <summary>
  ''' If a property has this attribute, it will not be included in the JS Class, but will still be rendered as json.
  ''' Usually this would be used for supporting data in a VM, like with ClientDataProvider.AddDatasource, but where you want a strongly typed property.
  ''' </summary>
  Public Class RawDataOnly
    Inherits InitialDataOnly

  End Class

  ''' <summary>
  ''' Put this on a property if you need it to be writable, and dont want it to make the object dirty. E.g. on an IsSelected or IsExpanded property.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class AlwaysClean
    Inherits Attribute

  End Class

  <Serializable()>
  Public Class Slider
    Inherits Attribute

    Public Property Minimum As Decimal = 0
    Public Property Maximum As Decimal = 10
    Public Property StepLength As Decimal = 1

    Public Sub New(Minimum As Double, Maximum As Double, StepLength As Double)
      Me.Minimum = Minimum
      Me.Maximum = Maximum
      Me.StepLength = StepLength
    End Sub

  End Class

  Public Class DateAndTimeField
    Inherits Attribute

    'allowInputToggle: true, showClear: true, keepOpen: true

    'Public Property MinDateProperty As String
    'Public Property MaxDateProperty As String

    ''' <summary>
    ''' Display the Date picker inline, so that the user doesn't have to click the input control to get it to pop up.
    ''' </summary>
    Public Property AlwaysShow As Boolean = False

    ' ''' <summary>
    ' ''' The number of years to show before / after. e.g: "-20:+0" = 20 years before selected date, "-20:c" = 20 years before current date
    ' ''' </summary>
    'Public Property YearRange As String = ""

    ''' <summary>
    ''' The javascript date format to use, default equates to something like: "Mon 01 Jan 2017 13:30"
    ''' </summary>
    Public Property FormatString As String = "ddd DD MMM YYYY HH:mm"

    ''' <summary>
    ''' Will cause the date picker to stay open after selecting a date if no time components are being used.
    ''' </summary>
    Public Property KeepOpen As Boolean = True

    ''' <summary>
    ''' Show the "Clear" button in the icon toolbar.
    ''' Clicking the "Clear" button will set the calendar to null.
    ''' </summary>
    Public Property ShowClear As Boolean = True

    ''' <summary>
    ''' If true, the picker will show on textbox focus and icon click when used in a button group
    ''' </summary>
    Public Property AllowInputToggle As Boolean = True

    ''' <summary>
    ''' Shows the date picker and time picker side by side when using the time and date together.
    ''' </summary>
    Public Property SideBySide As Boolean = True

    ''' <summary>
    ''' Shows the week of the year to the left of first day of the week.
    ''' </summary>
    Public Property ShowCalendarWeeks As Boolean = False

    ''' <summary>
    ''' The default view to display when the picker is shown. Accepts: 'decades','years','months','days'
    ''' </summary>
    Public Property ViewMode As String = "days"

    ''' <summary>
    ''' A function / property that will return the initial date to select when there is no date selected. Defaults to today.
    ''' </summary>
    Public Property InitialDateFunction As String

    ''' <summary>
    ''' Shows the datepicker as an inline calendar.
    ''' </summary>
    Public Property Inline As Boolean = False

  End Class

  Public Class Select2Field
    Inherits Attribute

    <Flags()>
    Public Enum SourceType
      ''' <summary>
      ''' Doesnt Look Anywhere, Used when passing in an enum for example.
      ''' </summary>
      None = 0
      ''' <summary>
      ''' Searches Commondata.Lists
      ''' </summary>
      CommonData = 1
      '''' <summary>
      '''' Searches Commondata.SessionLists
      '''' </summary>
      'SessionData = 2
      ''' <summary>
      ''' Searches ViewModel of the current Page.
      ''' </summary>
      ViewModel = 4
      '''' <summary>
      '''' Searches Commondata.TempLists
      '''' </summary>
      'TempData = 8
      '''' <summary>
      '''' Fetches the list from the database
      '''' </summary>
      'Fetch = 16
      ''' <summary>
      ''' Searches All Locations.
      ''' </summary>
      All = 31

      IgnoreMissing = 32
    End Enum

#Region " Properties "

    ''' <summary>
    ''' 
    ''' </summary>
    Public Property Tags As Boolean = True

    ''' <summary>
    ''' 
    ''' </summary>
    Public Property Placeholder As String = "Select Items"

    ''' <summary>
    ''' 
    ''' </summary>
    Public Property Multiple As Boolean = True

    ''' <summary>
    ''' 
    ''' </summary>
    Public Property AllowClear As Boolean = True

    ''' <summary>
    ''' 
    ''' </summary>
    Public Property Width As String = "100%"

    Public Property ListType As Type
    Public Property Source As SourceType = SourceType.All

    Public Property FilterMethod As String = ""

    Public Property InitialValueMethod As String = ""

    Private mValueMember As String
    Public Property ValueMember As String
      Get
        Return If(mValueMember Is Nothing, "Val", mValueMember)
      End Get
      Set(value As String)
        mValueMember = value
      End Set
    End Property

    Private mDisplayMember As String
    Public Property DisplayMember As String
      Get
        Return If(mDisplayMember Is Nothing, "Val", mDisplayMember)
      End Get
      Set(value As String)
        mDisplayMember = value
      End Set
    End Property

    Private mListName As String = ""
    ''' <summary>
    ''' Name to be used in Client Data Provider, if Blank, the list type name will be used.
    ''' </summary>
    Public Property ListName As String
      Get
        If mListName = "" Then
          Return ListType.Name
        Else
          Return mListName
        End If
      End Get
      Set(value As String)
        mListName = value
      End Set
    End Property

    Private mClientDatasourceName As String = ""
    Public ReadOnly Property ClientName As String
      Get
        If mClientDatasourceName = "" Then
          Return "ClientData." & ListName
        Else
          If mClientDatasourceName.EndsWith(")") Then
            Return mClientDatasourceName
          Else
            If mClientDatasourceName.Contains("ClientData") Then
              Return mClientDatasourceName
            Else
              Return mClientDatasourceName & "()"
            End If

          End If
        End If
      End Get
    End Property
    Public Property OnBindingUpdatedMethod As String = ""

#End Region

  End Class

#End If


  Public Class ObjectProperty
    Inherits Attribute

    Public Property CreateNew As Boolean

    Public Sub New(CreateNew As Boolean)
      Me.CreateNew = CreateNew
    End Sub
  End Class

#End Region

End Namespace
