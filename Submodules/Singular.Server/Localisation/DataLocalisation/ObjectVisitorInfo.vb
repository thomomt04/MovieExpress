Imports System.Linq

Namespace Localisation.Data

  Public Class ObjectTypeInfo

    Public Property ObjectType As Type

    Private _TableName As String
    Private _LDAttr As LocaliseDataAttribute
    Private _KeyProperty As Singular.ReflectionCached.CachedMemberInfo
    Private _IsEditable As Boolean

    Private _ObjectList As New Dictionary(Of Integer, ObjectInfo)

    Public Sub New(ObjectType As Type, LDA As LocaliseDataAttribute, Instance As ISingularBase)
      Me.ObjectType = ObjectType

      _LDAttr = LDA
      _TableName = Instance.GetTableName
      _KeyProperty = Singular.ReflectionCached.GetCachedType(ObjectType).KeyProperty
      _IsEditable = GetType(ISingularBusinessBase).IsAssignableFrom(ObjectType)

    End Sub

    Public Sub AddObject(Obj As ISingularBase)

      Dim ID As Integer = _KeyProperty.GetValueFast(Obj)

      _ObjectList.Add(ID, New ObjectInfo(ID, Obj, _IsEditable))

    End Sub

    Public Sub FetchData(LanguageID As Integer, VariantID As Integer?)

      'TODO: Pass in a list of column names, in case a column is removed from the list of localised columns

      Dim XmlIds = New XElement("IDs", _ObjectList.Select(Function(i) New XElement("ID", i.Key))).ToString()

      Dim cProc As New Singular.CommandProc("GetProcs.getLocalisationDataValueList",
                                            {"@TableName", "@XMLIds", "@LanguageID", "@VariantID", "@VariantFirst"},
                                            {_TableName, XmlIds, LanguageID, VariantID, LanguageVariantPriority = LanguageVariant.Variant})

      cProc.ExecuteReaderLocal(AddressOf ReadData)

    End Sub

    Private Sub ReadData(sdr As Csla.Data.SafeDataReader)

      Dim LastObject As ObjectInfo = Nothing
      Dim LastColumnName As String = String.Empty

      For Each obj In _ObjectList.Values
        obj.PreRead()
      Next

      While sdr.Read

        'Find the object if its not the same ID as the last record.
        If LastObject Is Nothing OrElse LastObject.ID <> sdr.GetInt32(1) Then
          LastObject = _ObjectList(sdr.GetInt32(1))
          LastColumnName = String.Empty
        End If

        'Only set the column value once, the order from the database will put the first value per column as the most important.
        If LastColumnName <> sdr.GetString(2) Then
          LastColumnName = sdr.GetString(2)

          LastObject.AddColumnValue(sdr.GetInt32(0), LastColumnName, sdr.GetString(3))
        End If

      End While

    End Sub

    Public Sub SaveData(LanguageID As Integer, VariantID As Integer?)

      Dim DataValueCount As Integer = 0

      Dim xml = Singular.Xml.CreateXML(
        Sub(xmlH)

          For Each obj In _ObjectList.Values
            If obj.Obj.LocalisationDataValues IsNot Nothing Then
              DataValueCount += obj.Obj.LocalisationDataValues.WriteXML(obj.ID, obj.Obj, xmlH)
            End If
          Next

        End Sub)

      If DataValueCount > 0 Then

        Singular.CommandProc.RunCommand("UpdProcs.updLocalisationDataValueList",
                                        {"@TableName", "@PKColumnName", "@LanguageID", "@VariantID", "@Data"},
                                        {_TableName, _KeyProperty.PropertyName, LanguageID, Singular.Misc.ZeroDBNull(VariantID), xml})

      End If

    End Sub

  End Class

  Public Class ObjectInfo

    Public Property ID As Integer
    Public Property Obj As ISingularBase
    Public Property IsEditable As Boolean

    Public Sub New(ID As Integer, Obj As ISingularBase, IsEditable As Boolean)
      Me.ID = ID
      Me.Obj = Obj
      Me.IsEditable = IsEditable
    End Sub

    Public Sub PreRead()
      If Obj.LocalisationDataValues Is Nothing Then Obj.LocalisationDataValues = New DataValueList
    End Sub

    Public Sub AddColumnValue(LocalisationDataValueID As Integer, FieldName As String, Value As String)

      Obj.LocalisationDataValues.AddDataValue(Obj, LocalisationDataValueID, FieldName, Value)

      If Not IsEditable Then
        'Readonly objects dont need the english version after the localised version has been substituted
        Obj.LocalisationDataValues = Nothing
      End If

    End Sub

  End Class

End Namespace