Imports System.Dynamic

Namespace Dynamic

  Public Module Dynamic
    Public Function GetValue(DynamicObj As DynamicObject, PropertyName As String, DefaultValue As Object) As Object
      Dim ActualValue As Object = Nothing
      If DynamicObj.GetDynamicMemberNames.Contains(PropertyName) Then
        If DynamicObj.TryGetMember(New MemberGetter(PropertyName), ActualValue) Then
          Return ActualValue
        Else
          Return DefaultValue
        End If
      Else
        Return DefaultValue
      End If
    End Function
  End Module

  <Serializable()>
  Public Class DynamicStorage
    Inherits System.Dynamic.DynamicObject
    Implements Runtime.Serialization.ISerializable



#Region " Serialisation "

    Public Sub New()

    End Sub

    Public Sub New(info As Runtime.Serialization.SerializationInfo, context As Runtime.Serialization.StreamingContext)
      For Each se As System.Runtime.Serialization.SerializationEntry In info
        Dictionary.Add(se.Name, se.Value)
      Next
    End Sub

    Public Overridable Sub GetObjectData(info As Runtime.Serialization.SerializationInfo, context As Runtime.Serialization.StreamingContext) Implements Runtime.Serialization.ISerializable.GetObjectData
      For Each kvp As KeyValuePair(Of String, Object) In Dictionary
        info.AddValue(kvp.Key, kvp.Value)
      Next
    End Sub

    Public Overrides Function GetDynamicMemberNames() As System.Collections.Generic.IEnumerable(Of String)
      Return Dictionary.Keys
    End Function

#End Region

    Protected Dictionary As New Dictionary(Of String, Object)

    Public Overrides Function TryGetMember(ByVal binder As System.Dynamic.GetMemberBinder, ByRef result As Object) As Boolean
      'If the dictionary key is missing, just return nothing.
      If Dictionary.ContainsKey(binder.Name) Then
        result = Dictionary(binder.Name)
      Else
        result = Nothing
      End If
      Return True
    End Function

    Public Overrides Function TrySetMember(ByVal binder As System.Dynamic.SetMemberBinder, ByVal value As Object) As Boolean
      Dictionary(binder.Name) = value
      Return True
    End Function

    Public Sub SetMember(MemberName As String, Value As Object)
      TrySetMember(New MemberSetter(MemberName), Value)
    End Sub

  End Class

  <Serializable()>
  Public Class DynamicStorage(Of t)
    Inherits DynamicStorage

    Public Sub New()
      MyBase.New()
    End Sub

    Public Sub New(info As Runtime.Serialization.SerializationInfo, context As Runtime.Serialization.StreamingContext)
      MyBase.New(info, context)
    End Sub

    ''' <summary>
    ''' This will check if there is data for the supplied property. If not, the code in the callback funcion will be called.
    ''' </summary>
    ''' <param name="Property">Property this is being used in.</param>
    ''' <param name="CallBack">Code that will be called the first time this property is called..</param>
    Public Function LateBind(ByVal [Property] As System.Linq.Expressions.Expression(Of Func(Of t, Object)), ByVal CallBack As System.Func(Of Object))
      Dim pi As System.Reflection.PropertyInfo = Singular.Reflection.GetMember([Property])

      If Not Dictionary.ContainsKey(pi.Name) Then
        Dictionary(pi.Name) = CallBack()
      End If
      Return Dictionary(pi.Name)

    End Function

    ''' <summary>
    ''' Clears the specified property so that it is refetched the next time it is called.
    ''' </summary>
    Public Sub ResetProperty(ByVal [Property] As System.Linq.Expressions.Expression(Of Func(Of t, Object)))
      Dim pi As System.Reflection.PropertyInfo = Singular.Reflection.GetMember([Property])
      If Dictionary.ContainsKey(pi.Name) Then
        Dictionary.Remove(pi.Name)
      End If
    End Sub

  End Class

  Public Class MemberGetter
    Inherits System.Dynamic.GetMemberBinder

    Public Sub New(Name As String)
      MyBase.New(Name, True)
    End Sub

    Public Overloads Overrides Function FallbackGetMember(target As System.Dynamic.DynamicMetaObject, errorSuggestion As System.Dynamic.DynamicMetaObject) As System.Dynamic.DynamicMetaObject
      Return Nothing
    End Function

    Public Shared Function GetValue(Obj As Object, PropertyName As String) As Object
      Dim ReturnValue As Object = Nothing
      Obj.TryGetMember(New MemberGetter(PropertyName), ReturnValue)
      Return ReturnValue
    End Function
  End Class

  Public Class MemberSetter
    Inherits System.Dynamic.SetMemberBinder

    Public Sub New(Name As String)
      MyBase.New(Name, True)
    End Sub

    Public Overloads Overrides Function FallbackSetMember(target As System.Dynamic.DynamicMetaObject, value As System.Dynamic.DynamicMetaObject, errorSuggestion As System.Dynamic.DynamicMetaObject) As System.Dynamic.DynamicMetaObject
      Return Nothing
    End Function
  End Class

End Namespace
