Imports System.Reflection
Imports Singular.Dynamic
Imports System.Web.Helpers

Namespace Web.Data.JS.ObjectInfo

  <Serializable()>
  Public Class ArrayMember
    Inherits Member

    Private mChildType As Type
    Private mTableName As String
    Private mColumns As DataColumnCollection
    Private mChildRelations As DataRelationCollection
    Private mIsByteArray As Boolean = False
    Private mClientOnlyProperty As Boolean = False

    Public Sub New(ph As PropertyHelper, Obj As Object, Index As Integer, JSSerialiser As JSSerialiser, ti As Singular.ReflectionCached.TypeInfo)
      Dim Inst As Object = Setup(ph, Obj, Index, JSSerialiser, ti)

      If mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.Array Then
        mChildType = ti.LastGenericType
        mIsByteArray = ti.Type Is GetType(Byte())
      ElseIf mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.DataTable Then
        Dim Table As DataTable = Inst
        mTableName = Table.TableName
        mColumns = Table.Columns
        mChildRelations = Table.ChildRelations
        mPH.WritesData = True
        'Need to add a list of child tables here, and not in WriteDataTable. Currently dataset schemas wont include child tables. 
      End If

      If ph.PropertyInfo IsNot Nothing Then
        mSetExpr = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.SetExpression)(ph.PropertyInfo)

        Dim co = mPH.CachedPropertyInfo.ClientOnly
        If co IsNot Nothing AndAlso co.Value Then
          mClientOnlyProperty = True
        End If
      End If

    End Sub

    Friend Overrides Sub RenderModel(JW As Singular.Web.Utilities.JavaScriptWriter)

      If PreRenderModel(JW) Then

        If mIsByteArray Then
          JW.Write("CreateProperty(self, '" & mPH.Name & "')")
        Else
          If mSetExpr IsNot Nothing Then
            ObjectMember.RenderModelProperty(JW, mPH, False, True, mClientOnlyProperty, "null", True)
            RenderPropertySet(JW)
          Else
            ObjectMember.RenderModelProperty(JW, mPH, False, True, mClientOnlyProperty, "null", False)
          End If
        End If

        'JW.Write("self." & mPH.Name & " = ko.observableArray([]);")
      End If

    End Sub

    Friend Overrides Sub RenderSchema(jw As JSonWriter)
      If mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.DataTable Then

        RenderSchema(jw, JSonPropertyName, mColumns, mChildRelations)

      End If
    End Sub

    Private Overloads Sub RenderSchema(jw As JSonWriter, PropertyName As String, Columns As DataColumnCollection, ChildRelations As DataRelationCollection)

      jw.StartClass(PropertyName)
      jw.StartClass("Properties")

      For Each col As DataColumn In Columns
        jw.StartClass(col.ColumnName)

        If Not Singular.Data.DataTables.AutoGenerateColumn(col) Then
          jw.WriteProperty("Hidden", True)
        Else

          Dim smi As New Singular.Reflection.SMemberInfo(col.DataType)
          'Type
          If smi.DataTypeMain <> Reflection.SMemberInfo.MainType.String Then
            jw.WriteProperty("Type", smi.DataTypeMain.ToString().ToLower()(0))
          End If

          'Name
          If col.Caption <> col.ColumnName Then
            jw.WriteProperty("Display", col.Caption)
          End If

          'Format
          Dim FormatString As String = Singular.Data.DataTables.GetExtProperty(col, Singular.Data.DataTables.ExtendedProperties.FormatString.ToString, "")
          If FormatString <> "" Then
            jw.WriteProperty("Format", FormatString)
          End If

          Dim Description = Singular.Data.DataTables.GetExtProperty(col, Singular.Data.DataTables.ExtendedProperties.Description, "")
          If Description <> "" Then
            jw.WriteProperty("Description", Description)
          End If
        End If

        jw.EndClass()
      Next

      For Each rel As DataRelation In ChildRelations
        RenderSchema(jw, rel.RelationName, rel.ChildTable.Columns, rel.ChildTable.ChildRelations)
      Next

      jw.EndClass()
      jw.EndClass()

    End Sub

    Public Overrides Sub UpdateModel(Dynamic As System.Dynamic.DynamicObject, Model As Object)

      Dim ClientArray As Object = Nothing

      If Model IsNot Nothing AndAlso Dynamic.TryGetMember(New MemberGetter(mPH.Name), ClientArray) AndAlso ClientArray IsNot Nothing Then

        If mIsByteArray Then
          'Convert the base64 String back to an array.
          mPH.PropertyInfo.SetValue(Model, Convert.FromBase64String(ClientArray), Nothing)
        Else

          Dim Collection As Object = mPH.PropertyInfo.GetValue(Model, Nothing)
          If Collection Is Nothing AndAlso mPH.PropertyInfo.CanWrite Then
            'If the list property is writable, then set the property to a new list.
            Collection = Activator.CreateInstance(mTypeAtRuntime)
            mPH.PropertyInfo.SetValue(Model, Collection, Nothing)
          End If

          If Collection IsNot Nothing Then

            If mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.Array Then

              Dim List As IList = Collection
              List.Clear()
              For Each DItem As Object In ClientArray
                List.Add(Convert.ChangeType(DItem, mChildType))
              Next

            ElseIf mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.Dictionary Then

              Dim Dict As IDictionary = Collection
              Dict.Clear()
              For Each DItem As Object In ClientArray
                Dict.Add(DItem.Key, DItem.Value)
              Next

            End If
          End If

        End If

      End If

    End Sub

    Friend Overrides Sub RenderData(JW As JSonWriter, Model As Object)

      Dim List As Object = Nothing
      If mPH.PropertyInfo IsNot Nothing Then
        List = mPH.PropertyInfo.GetValue(Model, Nothing)
      End If

      If CanRenderData(JW, List) Then

        If mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.Array Then

          If mIsByteArray Then
            'Convert to base64.
            If List Is Nothing Then
              JW.WriteProperty(JSonPropertyName, "")
            Else
              JW.WriteProperty(JSonPropertyName, Convert.ToBase64String(List))
            End If


          Else

            'Normal array.
            If JSonPropertyName <> "" Then
              JW.StartArray(JSonPropertyName)
            Else
              JW.StartArray()
            End If


            If List IsNot Nothing Then
              For Each item As Object In List
                JW.WriteArrayValue(item)
              Next
            End If

            JW.EndArray()
          End If

        ElseIf mTypeInfo.SerialisedType = ReflectionCached.SerialiseType.Dictionary Then

          JW.StartArray(JSonPropertyName)
          For Each Item In List
            JW.StartObject()
            JW.WriteProperty("Key", Item.Key)
            JW.WriteProperty("Value", Item.Value)
            JW.EndObject()
          Next
          JW.EndArray()

        Else
          'Table

          Dim Table As DataTable
          If TypeOf Model Is DataSet Then
            Table = Model.Tables(mTableName)
          Else
            Table = List
          End If
          WriteDataTable(JW, Table, If(mJSonPropertyName = "", "", Table.TableName), Table.Rows)

        End If



      End If

    End Sub

    Private Sub WriteDataTable(JW As JSonWriter, Table As DataTable, TableName As String, Rows As IEnumerable)

      JW.StartArray(TableName)

      For Each row As DataRow In Rows
        JW.StartObject()

        For Each col As DataColumn In Table.Columns
          JW.WriteProperty(col.ColumnName, row(col))
        Next

        For Each child As DataRelation In Table.ChildRelations
          WriteDataTable(JW, child.ChildTable, child.RelationName, row.GetChildRows(child))
        Next

        JW.EndObject()
      Next

      JW.EndArray()

    End Sub

    Private Sub RenderPropertySet(JW As Singular.Web.Utilities.JavaScriptWriter)

      If mSetExpr IsNot Nothing Then

        JW.RawWriteLine("")
        JW.AddLevel()
        JW.Write(".OnValueChanged(false, function(args) {")
        JW.AddLevel()

        mSetExpr.Write(JW)

        JW.RemoveLevel()
        JW.Write("})", False)
        JW.RemoveLevel()
        JW.RawWriteLine(";")

      End If

    End Sub

  End Class

End Namespace


