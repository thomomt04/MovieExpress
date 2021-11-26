Imports System.Reflection
Imports Singular.Web.Data
Imports System.Web.Helpers
Imports Singular.Dynamic

Namespace Localisation.Data

  <Serializable>
  Public Class DataValueList
    Inherits List(Of DataValue)

    Private _DidSwap As Boolean = False

    Public Shared Function FromJSon(JSonObject As DynamicJsonObject) As DataValueList

      Dim dvl As New DataValueList

      For Each PropertyName As String In JSonObject.GetDynamicMemberNames

        Dim JSonInfo As Object = Nothing
        If JSonObject.TryGetMember(New MemberGetter(PropertyName), JSonInfo) Then

          Dim dv As New DataValue With {.ID = JSonInfo.ID, .PropertyName = PropertyName, .PropertyValue = JSonInfo.Val}
          dvl.Add(dv)

        End If

      Next

      Return dvl

    End Function

    Public Sub AddDataValue(Obj As ISingularBase, DataValueID As Integer, PropertyName As String, PropertyValue As String)

      Dim dv As New DataValue() With {.ID = DataValueID, .PropertyName = PropertyName, .PropertyValue = PropertyValue}
      dv.SwapValues(Obj)
      Add(dv)

    End Sub

    Public Sub SwapValues(Obj As ISingularBase)
      _DidSwap = True

      For Each DataValue In Me
        DataValue.SwapValues(Obj)
      Next

    End Sub

    Public Function WriteXML(ID As Integer, Obj As ISingularBase, xmlHelper As Singular.Xml.XMLHelper) As Integer
      If _DidSwap Then

        For Each DataValue In Me
          xmlHelper.AddRow(
         {"RecordID", "PropertyName", "Value"},
         {ID, DataValue.PropertyName, DataValue.PropertyValue})
        Next

        SwapValues(Obj)

        Return Me.Count
      Else
        Return 0
      End If
    End Function

    Public Sub WriteJSon(jsw As JSonWriter)

      If Me.Count > 0 Then
        jsw.StartClass("__LocalisedData")
        For Each DataValue In Me

          jsw.StartClass(DataValue.PropertyName)
          jsw.WriteProperty("ID", DataValue.ID)
          jsw.WriteProperty("Val", DataValue.PropertyValue)
          jsw.EndClass()

        Next
        jsw.EndClass()
      End If

    End Sub

  End Class

  <Serializable>
  Public Class DataValue

    Public Property ID As Integer
    Public Property PropertyName As String
    Public Property PropertyValue As String

    <NonSerialized> Private _PropertyInfo As ReflectionCached.CachedMemberInfo

    Friend Sub SwapValues(Obj As ISingularBase)

      If _PropertyInfo Is Nothing Then
        Dim PropertyInfo = Obj.GetType.GetProperty(PropertyName, BindingFlags.Public Or BindingFlags.Instance)
        _PropertyInfo = ReflectionCached.GetCachedMemberInfo(PropertyInfo)
      End If

      If _PropertyInfo.BackingField IsNot Nothing Then

        Dim ObjectValue As String = Obj.GetBackingFieldValue(_PropertyInfo.BackingField)

        'IsDefault = ObjectValue = PropertyValue

        'Swap
        Obj.SetBackingFieldValue(_PropertyInfo.BackingField, PropertyValue, True)
        PropertyValue = ObjectValue

      End If

    End Sub

  End Class

End Namespace


