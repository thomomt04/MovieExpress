Imports System.Reflection

Namespace Web.Data.JS.ObjectInfo

  <Serializable()>
  Public Class MemberList
    Inherits List(Of Member)

    Public Shared Function GetMember(ph As PropertyHelper, obj As Object, Count As Integer, JSSerialiser As JSSerialiser, ConvertEnumToList As Boolean) As Member

      Dim TypeAtRuntime As Type = ph.GetReturnType(obj)
      Dim ReturnValue As Object = Nothing

      'if the return type is object, then try get the actual type.
      If TypeAtRuntime Is GetType(Object) AndAlso obj IsNot Nothing Then
        ReturnValue = ph.GetValue(obj)

        'If TypeOf ReturnValue Is Type Then
        '  If CType(ReturnValue, Type).IsEnum AndAlso ConvertEnumToList Then
        '    Return New ComplexListMember(ph, obj, Count, JSSerialiser, Singular.ReflectionCached.GetCachedType(ReturnValue))
        '  End If
        '  Return Nothing
        'End If

        If ReturnValue IsNot Nothing Then
          TypeAtRuntime = ReturnValue.GetType
        End If

      End If

      Dim ti As Singular.ReflectionCached.TypeInfo = Singular.ReflectionCached.GetCachedType(TypeAtRuntime)
      Dim SerialiseType = ti.SerialisedType

      If ti.ServerOnly Then
        Return Nothing
      End If
      If SerialiseType = ReflectionCached.SerialiseType.Enumeration Then
        SerialiseType = If(ConvertEnumToList, ReflectionCached.SerialiseType.TypedList, ReflectionCached.SerialiseType.Simple)
      End If

      Select Case SerialiseType
        Case ReflectionCached.SerialiseType.Simple
          Return New SimpleMember(ph, obj, Count, JSSerialiser, ti)
        Case ReflectionCached.SerialiseType.Array, ReflectionCached.SerialiseType.DataTable, ReflectionCached.SerialiseType.Dictionary
          Return New ArrayMember(ph, obj, Count, JSSerialiser, ti)
        Case ReflectionCached.SerialiseType.TypedList
          Return New ComplexListMember(ph, obj, Count, JSSerialiser, ti)
        Case Else
          Return New ObjectMember(ph, obj, Count, JSSerialiser, ti)
      End Select

    End Function

    Public Function CreateMember(ph As PropertyHelper, Obj As Object, JSSerialiser As JSSerialiser) As Member
      Dim NewMember As Member = GetMember(ph, Obj, Count, JSSerialiser, False)
      If NewMember IsNot Nothing Then
        Me.Add(NewMember)
      End If
      Return NewMember
    End Function

    Public Function GetItem(Name As String) As Member
      Return Me.Where(Function(c) c.PropertyHelper.Name = Name).FirstOrDefault
    End Function

    Public Function GetMemberNames() As String()
      Return (From m As Member In Me Select m.PropertyHelper.Name).ToArray
    End Function

    Public Function HasMember(Name As String) As Boolean
      Return Me.Where(Function(c) c.PropertyHelper.Name = Name).Count > 0
    End Function

    Public Function GetKey() As Member
      Return Me.Where(Function(c) c.PropertyHelper.CachedPropertyInfo.IsKey).FirstOrDefault
    End Function

  End Class

End Namespace
