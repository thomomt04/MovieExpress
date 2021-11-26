Imports System.Linq.Expressions

Public Interface ISingularPropertyInfo
  ReadOnly Property Name As String
  ReadOnly Property HasGetExpression As Boolean
  ReadOnly Property HasDefaultValueExpression As Boolean
  ReadOnly Property HasSetExpressions As Boolean
  ReadOnly Property SetExpressionList As List(Of SetExpressionInfo)
  Sub WriteDefaultValue(JW As Singular.Web.Utilities.JavaScriptWriter)
  Sub WriteGet(JW As Singular.Web.Utilities.JavaScriptWriter)
  Function GetParsedGetExpression() As Expression
  Function HasParsedGetExpression(Of T)() As Boolean
End Interface

Public Class SetExpressionInfo
  Public Property PropertyToSet As String
  Public Property SetExpressionJS As String = ""
  Public Property SetExpressionParsed As Linq.IExpressionParser
  Public Property BeforeChange As Boolean = False
  Public Property Delay As Integer = 0

  Public Sub WriteSet(JW As Singular.Web.Utilities.JavaScriptWriter)

    If SetExpressionJS <> "" Then
      JW.WriteBlock(SetExpressionJS)
    Else
      JW.Write("self." & PropertyToSet & "(" & SetExpressionParsed.ToString & ");")
    End If

  End Sub

  Public Sub SetProperty(Of ObjectType)(Instance As Object)

    Dim ValueToSet As Object = CType(SetExpressionParsed, Singular.Linq.ExpressionParser(Of ObjectType)).GetValue(Instance)
    Dim Prop = Singular.Reflection.GetProperty(GetType(ObjectType), PropertyToSet)
    Prop.SetValue(Instance, Singular.Reflection.ConvertValueToType(Prop.PropertyType, ValueToSet), Nothing)

  End Sub

End Class

Public Class SPropertyInfo(Of PropertyType, ObjectType)
  Inherits Csla.PropertyInfo(Of PropertyType)
  Implements ISingularPropertyInfo

  Public Sub New(Name As String, DefaultValue As PropertyType)
    'Friendly name is the same as the property name.
    'Everything seems to use the display annotation, so don't know why friendly name is even here..
    MyBase.New(Name, Name, DefaultValue)
  End Sub

  'Public Property CSLAPropertyInfo As Csla.PropertyInfo(Of PropertyType)

  Public Shadows ReadOnly Property Name As String Implements ISingularPropertyInfo.Name
    Get
      Return MyBase.Name
    End Get
  End Property

#Region " Get Expressions "

  Private mGetExpressionJS As String = ""
  Private mGetExpressionCallBack As Func(Of String)
  Private mGetExpressionParsed As Singular.Linq.JavascriptExpressionParser(Of ObjectType)
  Private mHasGetExpression As Boolean = False

  Public Function GetExpression(GetExpressionJS As String) As SPropertyInfo(Of PropertyType, ObjectType)
    mGetExpressionJS = GetExpressionJS
    mHasGetExpression = mGetExpressionJS <> ""
    Return Me
  End Function

  Public Function GetExpression(GetExpressionCallBack As Func(Of String)) As SPropertyInfo(Of PropertyType, ObjectType)
    mGetExpressionCallBack = GetExpressionCallBack
    mHasGetExpression = True
    Return Me
  End Function

  Public Function GetExpression(GetExpr As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As SPropertyInfo(Of PropertyType, ObjectType)
    mGetExpressionParsed = New Singular.Linq.JavascriptExpressionParser(Of ObjectType)(GetExpr, Linq.OutputModeType.GetProperty)
    mGetExpressionParsed.Parse()
    mHasGetExpression = True
    Return Me
  End Function

  Public Function GetExpression(GetExpr As System.Linq.Expressions.LambdaExpression) As SPropertyInfo(Of PropertyType, ObjectType)
    mGetExpressionParsed = New Singular.Linq.JavascriptExpressionParser(Of ObjectType)(GetExpr, Linq.OutputModeType.GetProperty)
    mGetExpressionParsed.Parse()
    mHasGetExpression = True
    Return Me
  End Function

  Public ReadOnly Property HasGetExpression As Boolean Implements ISingularPropertyInfo.HasGetExpression
    Get
      'mGetExpressionParsed IsNot Nothing OrElse mGetExpressionJS <> ""
      Return mHasGetExpression
    End Get
  End Property

  Public Function HasParsedGetExpression(Of T)() As Boolean Implements ISingularPropertyInfo.HasParsedGetExpression
    Return GetType(T) Is GetType(ObjectType) AndAlso mGetExpressionParsed IsNot Nothing
  End Function

  Public ReadOnly Property ParsedGetExpression As Singular.Linq.JavascriptExpressionParser(Of ObjectType)
    Get
      Return mGetExpressionParsed
    End Get
  End Property

  Public Sub WriteGetJS(JW As Singular.Web.Utilities.JavaScriptWriter) Implements ISingularPropertyInfo.WriteGet

    If mGetExpressionParsed IsNot Nothing Then
      JW.WriteBlock(mGetExpressionParsed.ToString())
    ElseIf mGetExpressionCallBack IsNot Nothing Then
      JW.WriteBlock(mGetExpressionCallBack.Invoke)
    ElseIf mGetExpressionJS <> "" Then
      JW.WriteBlock(mGetExpressionJS)
    End If

  End Sub

  Public Function GetParsedGetExpressionJS() As Expression Implements ISingularPropertyInfo.GetParsedGetExpression

    Return mGetExpressionParsed.Expression

  End Function

#End Region

#Region " Set Expressions "

  Private mSetExpressionList As New List(Of SetExpressionInfo)

  Public ReadOnly Property SetExpressionList As List(Of SetExpressionInfo) Implements ISingularPropertyInfo.SetExpressionList
    Get
      Return mSetExpressionList
    End Get
  End Property

  Public Function AddSetExpression(SetExpression As String, Optional BeforeChange As Boolean = False, Optional DelayInMs As Integer = 0) As SPropertyInfo(Of PropertyType, ObjectType)
    mSetExpressionList.Add(New SetExpressionInfo() With {.SetExpressionJS = SetExpression, .BeforeChange = BeforeChange, .Delay = DelayInMs})
    Return Me
  End Function

  Public Function AddSetExpression(PropertyToSet As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                   SetExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                                   Optional BeforeChange As Boolean = False, Optional Delay As Integer = 0) As SPropertyInfo(Of PropertyType, ObjectType)
    Dim sei As New SetExpressionInfo
    Dim sep As New Singular.Linq.JavascriptExpressionParser(Of ObjectType)(SetExpression, Linq.OutputModeType.GetProperty)
    sep.IncludeReturnStatement = False
    sep.Parse()
    sei.SetExpressionParsed = sep
    sei.BeforeChange = BeforeChange
    sei.Delay = Delay
    sei.PropertyToSet = Singular.Reflection.GetMember(PropertyToSet).Name
    mSetExpressionList.Add(sei)
    Return Me
  End Function

  Public ReadOnly Property HasSetExpressions As Boolean Implements ISingularPropertyInfo.HasSetExpressions
    Get
      Return mSetExpressionList.Count > 0
    End Get
  End Property

#End Region

#Region " Default Value Expressions "

  Private mDefaultValueExpressionJS As String = ""
  Private mDefaultValueExpressionParsed As Singular.Linq.JavascriptExpressionParser(Of ObjectType)

  Public Shadows Function DefaultValueExpression(DefaultValueJS As String) As SPropertyInfo(Of PropertyType, ObjectType)
    mDefaultValueExpressionJS = DefaultValueJS
    Return Me
  End Function

  Public Shadows Function DefaultValueExpression(DefaultValueExpr As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As SPropertyInfo(Of PropertyType, ObjectType)
    mDefaultValueExpressionParsed = New Singular.Linq.JavascriptExpressionParser(Of ObjectType)(DefaultValueExpr, Linq.OutputModeType.GetProperty)
    mDefaultValueExpressionParsed.Parse()
    Return Me
  End Function

  Public ReadOnly Property HasDefaultValueExpression As Boolean Implements ISingularPropertyInfo.HasDefaultValueExpression
    Get
      Return mDefaultValueExpressionParsed IsNot Nothing OrElse mDefaultValueExpressionJS <> ""
    End Get
  End Property

  Public Sub WriteDefaultValue(JW As Singular.Web.Utilities.JavaScriptWriter) Implements ISingularPropertyInfo.WriteDefaultValue
    If mDefaultValueExpressionJS <> "" Then
      JW.RawWrite(mDefaultValueExpressionJS)
    ElseIf mDefaultValueExpressionParsed IsNot Nothing Then
      JW.RawWrite(mDefaultValueExpressionParsed.ToString)
    End If
  End Sub

#End Region

End Class

Public Module PropertyHelpers

  Public Function RegisterSProperty(Of PropertyType, C)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)), DefaultValue As PropertyType) As SPropertyInfo(Of PropertyType, C)
    Return New SPropertyInfo(Of PropertyType, C)(Singular.Reflection.GetMember(TargetMember).Name, DefaultValue)
  End Function

  Public Function RegisterSProperty(Of PropertyType, C)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object))) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType, C)(TargetMember, Nothing)
  End Function

End Module

Namespace Web

  Public Class JSonCache
    Public JSon As String
    Public Hash As String
    Public RetrievedFrom As DataAnnotations.DropDownWeb.SourceType
  End Class

  Public Enum HttpRestrictMethod
    Any
    Post
    [Get]
  End Enum

  ''' <summary>
  ''' Specifies that this class can be used directly from javascript. This is only for authenticated users unless otherwise specified.
  ''' </summary>
  ''' <remarks></remarks>
  Public MustInherit Class WebDataAllowed
    Inherits Attribute

    Public Property LoggedInOnly As Boolean
    Public Property Roles As String()
    Public Property RestrictMethod As HttpRestrictMethod = HttpRestrictMethod.Any

    Public Sub New()
      LoggedInOnly = True
    End Sub

    Public Sub New(LoggedInOnly As Boolean)
      Me.LoggedInOnly = LoggedInOnly
    End Sub

    Public Sub New(Roles As String())
      LoggedInOnly = True
      Me.Roles = Roles
    End Sub

    Public Function Allowed() As Boolean
      If LoggedInOnly Then
        If Singular.Security.HasAuthenticatedUser Then
          'Check that the user has at least 1 role.
          Dim HasRole As Boolean = False
          If Roles Is Nothing OrElse Roles.Length = 0 Then
            Return True
          Else
            For Each role As String In Roles
              If Singular.Security.HasAccess(role) Then
                Return True
              End If
            Next
            Return False
          End If
        Else
          'Not authenticated
          Return False
        End If
      Else
        Return True
      End If
    End Function

  End Class

  Public Enum DataType
    JSON = 1
    Binary = 2
  End Enum

  <AttributeUsage(AttributeTargets.Method)>
  Public Class WebCallable
    Inherits WebDataAllowed

    Public Property DataType As DataType = DataType.JSON

  End Class

  <AttributeUsage(AttributeTargets.Class)>
  Public Class WebFetchable
    Inherits WebDataAllowed

  End Class

  <AttributeUsage(AttributeTargets.Class)>
  Public Class WebSavable
    Inherits WebDataAllowed

  End Class

  Public Interface IBindingRoot

  End Interface

End Namespace

Namespace Web.Utilities

  ''' <summary>
  ''' Allows a custom JavaScript function to be attached to the business object.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()>
  Public Class JavascriptMethod

    Protected mMethodName As String
    Public ReadOnly Property MethodName As String
      Get
        Return mMethodName
      End Get
    End Property

    Protected mMethodBody As String = ""
    Public ReadOnly Property MethodBody As String
      Get
        Return mMethodBody
      End Get
    End Property

    Public Sub New(ByVal MethodName As String, ByVal MethodBody As String)

      mMethodName = MethodName
      mMethodBody = MethodBody

    End Sub

  End Class

  Public Class SingularWebInterface

    Public Shared Function GetCacheableJSon(Data As Object, Context As String) As String
      Return GetCacheableJSonCallBack(Data, Context, Web.Data.OutputType.JSon)
    End Function

    Public Shared GetCacheableJSonCallBack As Func(Of Object, String, Data.OutputType, String)

  End Class


End Namespace
