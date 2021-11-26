Imports System.Reflection

Namespace Web.Data.JS.ObjectInfo

  Public Class PropertyHelper

    Private mMi As MemberInfo
    Private mName As String
    Private mOnType As Type
    Private mReturnType As Type = Nothing
    Friend IsProperty As Boolean = False

    ''' <summary>
    ''' If this property writes JSON data.
    ''' </summary>
    Friend WritesData As Boolean = False
    Friend IsDynamicallyAdded As Boolean = False
    Private HasExtraPI As Boolean = False

    Private mIsMethod As Boolean = False
    Private mCachedPropertyInfo As Singular.ReflectionCached.CachedMemberInfo

    Public Sub New(Name As String, OnType As Type, ReturnType As Type)
      mName = Name
      mOnType = OnType
      mReturnType = ReturnType
      IsDynamicallyAdded = True
    End Sub

    Public Sub New(mi As MemberInfo, Name As String, OnType As Type, Optional ReturnType As Type = Nothing)
      mMi = mi
      mName = Name
      mOnType = OnType
      mReturnType = ReturnType

      If mMi IsNot Nothing Then
        Dim GetMethod As MethodInfo
        If TypeOf mMi Is PropertyInfo Then
          IsProperty = True
          GetMethod = CType(mMi, PropertyInfo).GetGetMethod(True)
        Else
          mIsMethod = True
          GetMethod = mMi
        End If

        mCachedPropertyInfo = Singular.ReflectionCached.GetCachedMemberInfo(mi)
        HasExtraPI = mCachedPropertyInfo IsNot Nothing

      End If

      WritesData = (IsProperty Or mMi Is Nothing) AndAlso Not mCachedPropertyInfo.ClientOnlyNoData

    End Sub

    Public ReadOnly Property IsMethod As Boolean
      Get
        Return mIsMethod
      End Get
    End Property

    Public ReadOnly Property PropertyInfo As PropertyInfo
      Get
        Return mMi
      End Get
    End Property

    Public ReadOnly Property CachedPropertyInfo As Singular.ReflectionCached.CachedMemberInfo
      Get
        Return mCachedPropertyInfo
      End Get
    End Property

    Public ReadOnly Property OnType As Type
      Get
        Return mOnType
      End Get
    End Property

    Public ReadOnly Property Name As String
      Get
        Return mName
      End Get
    End Property

    Public Function GetValue(Instance As Object) As Object

      If IsDynamicallyAdded Then
        Return Nothing
      End If

      Try
        If HasExtraPI Then
          Return mCachedPropertyInfo.GetValueFast(Instance)
        ElseIf mMi IsNot Nothing Then
          If IsProperty Then
            Return PropertyInfo.GetValue(Instance, Nothing)
          Else
            Return DirectCast(mMi, MethodInfo).Invoke(Instance, Nothing)
          End If
        Else

          Dim Obj As Object = Nothing
          CType(Instance, System.Dynamic.DynamicObject).TryGetMember(New Singular.Dynamic.MemberGetter(mName), Obj)
          Return Obj
        End If
      Catch ex As Exception
        If mOnType <> Instance.GetType Then
          Throw New Exception("Error getting value on property " & mName & ", expected type " & mOnType.Name & ", given type " & Instance.GetType.Name, ex)
        Else
          Throw New Exception("Error getting value on property " & mName, ex)
        End If

      End Try


    End Function

    Public Function GetReturnType(Instance As Object) As Type

      If mReturnType IsNot Nothing Then
        Return mReturnType
      End If

      If mMi IsNot Nothing Then
        If IsProperty Then
          Return CType(mMi, PropertyInfo).PropertyType
        ElseIf mIsMethod Then
          Return CType(mMi, MethodInfo).ReturnType
        End If
      Else
        Return GetValue(Instance).GetType
      End If
      Return Nothing

    End Function

    Public Function IsValueType() As Boolean
      If mReturnType IsNot Nothing Then
        Return mReturnType.IsValueType
      Else
        If IsProperty Then
          Return CType(mMi, PropertyInfo).PropertyType.IsValueType
        ElseIf mIsMethod Then
          Return CType(mMi, MethodInfo).ReturnType.IsValueType
        End If
      End If
      Return False
    End Function

  End Class

End Namespace

