Imports System.Reflection

Public Class DropDownInfo

  Public Sub New()

  End Sub

  Public Sub New(ObjectType As Type)

#If SILVERLIGHT Then
#Else
    'Check if the type has a key property
    Dim KeyProperty = Singular.ReflectionCached.GetCachedType(ObjectType).KeyProperty
    If KeyProperty IsNot Nothing Then
      ValueMember = KeyProperty.PropertyName
    End If
#End If

    For Each pi As PropertyInfo In ObjectType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
      If pi.Name.EndsWith("ID") AndAlso ValueMember = "" Then
        ValueMember = pi.Name
      End If
      If Singular.Reflection.CanSerialiseField(pi) AndAlso pi.PropertyType Is GetType(String) AndAlso DisplayMember = "" Then
        DisplayMember = pi.Name
      End If
    Next
  End Sub

  Public Property ValueMember As String = ""
  Public Property DisplayMember As String = ""

End Class

Public Class WindowsDropDownInfo
  Inherits DropDownInfo


End Class

#If SILVERLIGHT Then
#Else

Public Class WebDropDownInfo
  Inherits DropDownInfo

  Public Property GroupChildListMember As String = ""
  Public Property GroupMember As String = ""

End Class

#End If

