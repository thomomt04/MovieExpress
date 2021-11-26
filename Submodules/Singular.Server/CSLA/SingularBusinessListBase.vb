Imports Csla
Imports Csla.Core
Imports Csla.Properties
Imports Csla.Serialization
Imports System.Reflection
Imports Singular.ObjectErrors
Imports System.Xml.Linq

Public Delegate Sub AbortableAction(ByVal Target As Object, ByVal Context As AbortableActionContext)

Public Class AbortableActionContext

  Public Property Abort As Boolean = False

End Class

Public Class EditLevelContext
  Inherits AbortableActionContext

  Public Property EditLevel As Integer
  Public Property EditLevelMissmatch As String = ""

End Class

<Serializable()>
Public Class GenericList(Of ChildType As Singular.SingularBusinessBase(Of ChildType))
  Inherits Singular.SingularBusinessListBase(Of GenericList(Of ChildType), ChildType)


End Class

<Serializable()> _
Public Class SingularBusinessListBase(Of T As SingularBusinessListBase(Of T, C), C As SingularBusinessBase(Of C))
#If Silverlight Or WPF Then
  Inherits Csla.BusinessListBase(Of T, C)
#Else
  Inherits Csla.BusinessBindingListBase(Of T, C)
#End If
  Implements Singular.ISingularBusinessListBase
  Implements Singular.ISingularBusinessListBaseGeneric(Of T, C)

  Public Overridable Sub FlattenEditLevels() Implements ISavable.FlattenEditLevels

    Dim BaseEditLevel As Integer = Me.GetEditLevel()

    Dim Context As New EditLevelContext() With {.EditLevel = Me.GetEditLevel}
    RecurseObjectGraphAndPerformAction(AddressOf FlattenEditLevel, Context, True)

  End Sub

  Protected Overridable Sub FlattenEditLevel(ByVal Target As Object, ByVal Context As AbortableActionContext)

    Dim EditLevelContext As EditLevelContext = Context

    Dim mi = Target.GetType.GetMethod("GetEditLevel")

    Dim piDeletedList = Target.GetType.GetProperty("DeletedList", BindingFlags.NonPublic + BindingFlags.FlattenHierarchy + BindingFlags.Instance)

    If piDeletedList IsNot Nothing Then
      Dim DeletedList = piDeletedList.GetValue(Target, Nothing)
      For i = DeletedList.Count - 1 To 0 Step -1
        If DeletedList(i).IsNew Then
          DeletedList.RemoveAt(i)
        Else
          While DeletedList(i).GetEditLevel > EditLevelContext.EditLevel
            DeletedList(i).ApplyEdit()
          End While
          While DeletedList(i).GetEditLevel < EditLevelContext.EditLevel
            DeletedList(i).BeginEdit()
          End While
        End If
      Next
    End If


    If mi IsNot Nothing Then
      While CInt(mi.Invoke(Target, Nothing)) > EditLevelContext.EditLevel
        Target.ApplyEdit()
      End While

      While CInt(mi.Invoke(Target, Nothing)) < EditLevelContext.EditLevel
        Target.BeginEdit()
      End While
    End If

  End Sub

#If SILVERLIGHT Then
#Else
  Private mIndex As Hashtable
  Public Sub BuildIndexSafe(Of ReturnType)([Property] As System.Linq.Expressions.Expression(Of Func(Of C, ReturnType)))
    Dim mi = Singular.Reflection.GetMemberSpecific([Property])
    Dim cmi = Singular.ReflectionCached.GetCachedMemberInfo(mi)

    mIndex = New Hashtable
    Dim key As String = ""
    For Each Item As C In Me
      key = cmi.GetValueFast(Item)
      If Not mIndex.ContainsKey(key) Then
        mIndex.Add(key, Item)
      End If
    Next
  End Sub

  Public Function GetItemIndexed(Value As Object) As C
    Return mIndex(Value)
  End Function
#End If

  Public Function FindParent(Of Y)() As Y

    Return FindParent(Of Y)(Me)

  End Function

  Private Function FindParent(Of Y)(ByVal OfObject As Object) As Y

    Dim parent = OfObject.Parent

    If parent IsNot Nothing Then
      If TypeOf parent Is Y Then
        Return parent
      Else
        Return FindParent(Of Y)(parent)
      End If
    Else
      Return Nothing
    End If

  End Function

  Public Sub ClearNewItems() Implements ISingularBusinessListBase.ClearNewItems

    For i As Integer = Me.Count - 1 To 0 Step -1
      If Me(i).IsNew Then
        Me.RemoveAt(i)
      End If
    Next

  End Sub

  Public Function GetEditLevel() As Integer
    Return MyBase.EditLevel
  End Function

  Public Function GetDeletedList() As Object Implements ISingularBusinessListBase.GetDeletedList
    Return DeletedList
  End Function

#Region " Sort Helper "

#If SILVERLIGHT Then

  Public Function Sort(SortInfo As SingularDataGrid.SortInfo) As ISingularListBase Implements ISingularListBase.Sort

    Dim SortedList As SingularBusinessListBase(Of T, C) = Activator.CreateInstance(Me.GetType, Nothing)

    For Each Child In Me.OrderBy(Function(c) GetSortItem(c, SortInfo))
      SortedList.Add(Child)
    Next

    Return SortedList

  End Function

  Private mSortProperty As PropertyInfo

  Private Function GetSortItem(Child As C, SortInfo As SingularDataGrid.SortInfo) As Object

    If mSortProperty Is Nothing OrElse mSortProperty.Name <> SortInfo.SortDescription.PropertyName Then
      mSortProperty = Child.GetType.GetProperty(SortInfo.SortDescription.PropertyName)
    End If
    If mSortProperty IsNot Nothing Then
      Dim Value = mSortProperty.GetValue(Child, Nothing)

      If Value IsNot Nothing Then
        If SortInfo.ComboList IsNot Nothing Then

          Dim LookupProperty As PropertyInfo = Nothing
          Dim DisplayProperty As PropertyInfo = Nothing

          Dim LookupObject = Enumerable.FirstOrDefault(Of Object)(SortInfo.ComboList, Function(lo)

                                                                                        If LookupProperty Is Nothing Then
                                                                                          LookupProperty = lo.GetType.GetProperty(SortInfo.ComboSelectedValuePath)
                                                                                        End If


                                                                                        If LookupProperty IsNot Nothing Then
                                                                                          If Singular.Misc.CompareSafe(LookupProperty.GetValue(lo, Nothing), Value) Then
                                                                                            ' this is the one
                                                                                            Return True
                                                                                          End If
                                                                                        End If

                                                                                        Return False

                                                                                      End Function)

          If LookupObject IsNot Nothing Then
            If DisplayProperty Is Nothing Then
              DisplayProperty = LookupObject.GetType.GetProperty(SortInfo.ComboDisplayMemberPath)
            End If

            If DisplayProperty IsNot Nothing Then
              Return DisplayProperty.GetValue(LookupObject, Nothing)
            End If
          End If

        End If
      End If

      Return Value
    Else
      Return Nothing
    End If


  End Function

#End If

#End Region

  <System.ComponentModel.Browsable(False), System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
  Public Overloads ReadOnly Property IsDirty As Boolean Implements ISingularBusinessListBase.IsDirty
    Get
      Return MyBase.IsDirty
    End Get
  End Property


  Public Overridable Function GetErrorsAsBusinessNodeList() As Singular.ObjectErrors.BusinessObjectNodeList(Of T, C)

    Dim nodes As New Singular.ObjectErrors.BusinessObjectNodeList(Of T, C)(Me)
    Return nodes

  End Function

  Public Overridable Function GetErrorsAsHTMLString() As String Implements ISavable.ErrorsAsHTMLString

    Dim IndentLevel = 0
    Dim Errors As String = "<ul>" & vbCrLf
    For Each node As ObjectErrors.BusinessObjectNodeBase In Me.GetErrorsAsBusinessNodeList
      Errors &= "<li>" & vbCrLf & ObjectErrors.GetErrorsAsHTMLString(node, IndentLevel) & "</li>" & vbCrLf
    Next
    Errors &= "</ul>" & vbCrLf
    Return Errors

  End Function

  Public Overridable Function GetErrorsAsString() As String Implements ISavable.ErrorsAsString

    Dim Errors As String = ""
    Dim IndentLevel = 0
    For Each node As ObjectErrors.BusinessObjectNodeBase In Me.GetErrorsAsBusinessNodeList
      Errors &= ObjectErrors.GetErrorsAsString(node, IndentLevel)
    Next
    Return Errors

  End Function

  Public Function GetXmlIDs() As String Implements ISingularListBase.GetXmlIDs

    Return (New XElement("IDs", Me.[Select](Function(i) New XElement("ID", i.GetId)))).ToString()

  End Function

  Public Sub RecurseObjectGraphAndPerformAction(ByVal Action As AbortableAction, ByVal Context As AbortableActionContext, Optional ByVal PerformOnListObjects As Boolean = False)

#If Silverlight = False Then
    Dim Iterator As New CSLALib.ObjectIterator(PerformOnListObjects, True, False, Action)
    Iterator.Context = Context
    Iterator.RecurseObjectGraphAndPerformAction(Me)
#End If

  End Sub

  Public Function GetEditLevelMissmatch() As String

    Dim Context As New EditLevelContext() With {.EditLevel = Me.GetEditLevel}
    RecurseObjectGraphAndPerformAction(AddressOf CheckEditLevelMissmatch, Context, True)
    Return Context.EditLevelMissmatch

  End Function

  Private Sub CheckEditLevelMissmatch(ByVal Target As Object, ByVal Context As AbortableActionContext)

    Dim EditLevelContext As EditLevelContext = Context

    Dim mi = Target.GetType.GetMethod("GetEditLevel")

    If mi IsNot Nothing Then
      Dim ObjectEditLevel As Integer = CInt(mi.Invoke(Target, Nothing))
      If ObjectEditLevel <> EditLevelContext.EditLevel Then
        ' we have found the conflicting object
        Dim CurrentObject As Object = Target
        EditLevelContext.EditLevelMissmatch = "Missmatch Object: " & Target.ToString() & " - Edit Level: " & ObjectEditLevel & " - Expected Edit Level: " & EditLevelContext.EditLevel
        EditLevelContext.Abort = True
      End If
    End If

  End Sub

#If SILVERLIGHT Then

  'Private mOverridingEditLevel As Integer = -1

  'Protected Overloads Overrides ReadOnly Property EditLevel As Integer
  '  Get
  '    If mOverridingEditLevel = -1 Then
  '      Return MyBase.EditLevel
  '    Else
  '      Return mOverridingEditLevel
  '    End If
  '  End Get
  'End Property

  'Public Shared Function CreateNew(ByVal OfType As Type) As Object

  '  Dim DataSource As Object = Nothing

  '  Dim NewListMethodName As String = "New" & OfType.Name
  '  ' need to try fetch the list
  '  Dim mis() As System.Reflection.MethodInfo = OfType.GetMethods(BindingFlags.Static Or BindingFlags.Public)
  '  Try
  '    For Each mi As System.Reflection.MethodInfo In mis
  '      If mi.Name = NewListMethodName Then
  '        If Reflection.MethodMatchesParameters(mi, Nothing) Then
  '          DataSource = mi.Invoke(Nothing, Nothing)
  '          Exit For
  '        End If
  '      End If
  '    Next
  '    If DataSource Is Nothing Then
  '      Throw New Exception("No 'New' Factory Method found (" & NewListMethodName & ")")
  '    End If
  '  Catch ex As Exception
  '    Throw New Exception("Error Creating " & OfType.Name & ": " & ex.Message)
  '  End Try

  '  Return DataSource

  'End Function

  'Private mSavedHandler As System.EventHandler(Of Csla.Core.SavedEventArgs)

  'Public Overrides Sub BeginSave(ByVal handler As System.EventHandler(Of Csla.Core.SavedEventArgs), ByVal userState As Object)

  '  mSavedHandler = handler
  '  MyBase.BeginSave(AddressOf ListSaved, userState)

  'End Sub

  'Public Overrides Sub BeginSave(ByVal handler As System.EventHandler(Of Csla.Core.SavedEventArgs), ByVal userState As Object)

  '  Me.ApplyEdit()
  '  'mSavedHandler = handler
  '  'Me.SaveAll(AddressOf ListSaved, userState)

  '  Dim List As SingularBusinessListBase(Of T, C) = CreateNew(Me.GetType)
  '  For Each child As C In Me
  '    If child.IsDirty Then
  '      List.Add(child)
  '      child.SetParent(Me)
  '    End If
  '  Next
  '  mSavedHandler = handler
  '  List.SaveAll(AddressOf ListSaved, userState)

  'End Sub

  'Protected Overridable Sub SaveAll(ByVal handler As System.EventHandler(Of Csla.Core.SavedEventArgs), ByVal userState As Object)

  '  MyBase.BeginSave(handler, userState)

  'End Sub


  'Private Sub ListSaved(ByVal sender As Object, ByVal e As Csla.Core.SavedEventArgs)

  '  '    mOverridingEditLevel = -1
  '  '    Dim List As SingularBusinessListBase(Of T, C) = e.NewObject
  '  If mSavedHandler IsNot Nothing Then
  '    mSavedHandler.Invoke(sender, e)
  '  End If
  '  Me.BeginEdit()

  'End Sub


#Else

  Protected Overrides Sub RemoveItem(ByVal index As Integer)
    If Not Me(index).IsChild Then
      Me(index).MarkAsChild()
    End If
    MyBase.RemoveItem(index)
  End Sub

  Protected Overridable Function GetConnectionString() As String
    Return Settings.ConnectionString
  End Function

  Protected Overridable Sub UpdateTransactional(ByVal UpdateMethod As Action)

    UpdateTransactional(UpdateMethod, GetConnectionString)

    DODropDownUpdate()

  End Sub

  Protected Sub DODropDownUpdate()

    RaiseEvent UpdateDropDowns(Me, New EventArgs)

  End Sub

  Protected Overridable Sub UpdateTransactional(ByVal UpdateMethod As Action, ByVal ConnectionString As String)

    Dim cn As SqlClient.SqlConnection = New SqlClient.SqlConnection(ConnectionString)
    Csla.ApplicationContext.LocalContext("cn") = cn
    cn.Open()
    Try
      Dim tr As SqlClient.SqlTransaction = cn.BeginTransaction(IsolationLevel.ReadUncommitted, Left(Me.GetType.Name, 32))
      Csla.ApplicationContext.LocalContext("tr") = tr
      Try

        UpdateMethod.Invoke()

        tr.Commit()
      Catch ex As Exception
        tr.Rollback()
        Throw ex
      End Try
    Finally
      Csla.ApplicationContext.LocalContext("cn") = Nothing
      cn.Close()
      cn.Dispose()
      Csla.ApplicationContext.LocalContext("tr") = Nothing
    End Try

  End Sub

  Protected Friend Overridable Sub UpdateGeneric() Implements ISingularBusinessListBase.UpdateGeneric

    Me.RaiseListChangedEvents = False
    Try
      ' Loop through each deleted child object and call its Update() method
      For Each Child As C In DeletedList
        Child.DeleteSelfGeneric()
      Next

      ' Then clear the list of deleted objects because they are truly gone now.
      DeletedList.Clear()

      ' Loop through each non-deleted child object and call its Update() method
      For Each Child As C In Me
        Child.InsertUpdateGeneric()
      Next
    Finally
      Me.RaiseListChangedEvents = True
    End Try

  End Sub

  Protected Overrides Sub DataPortal_Update()

    UpdateTransactional(AddressOf UpdateGeneric)

  End Sub

  ''' <summary>
  ''' Performs a bulk update of records using XML.
  ''' </summary>
  ''' <param name="UpdateMethod">Update all records at once, or insert / update seperately. InsProc will be called if All is specified.</param>
  ''' <param name="BatchSizeKB">Whenever the xml size reaches this limit, the proc will be called.</param>
  ''' <param name="ProcSetup">Any custom params for the proc. By default Proc name is InsProcs.ins[ObjectName]Bulk, or UpdProcs.upd[ObjectName]Bulk</param>
  Public Sub BulkUpdate(UpdateMethod As BulkUpdateMethod, Optional BatchSizeKB As Integer = 2048, Optional ProcSetup As Action(Of Singular.CommandProc) = Nothing)

    ' How to write stored proc code:

    ' SELECT  [Rows].[Row].value('(@NoteTypeID)[1]', 'Int') As NoteTypeID,
    '         [Rows].[Row].value('(@CalcDate)[1]', 'Date') As CalcDate
    ' FROM @Data.nodes('//Rows/Row') AS [Rows]([Row]) 
    If Me.Count > 0 Then

      Dim cProc As New Singular.CommandProc(Me(0).GetSPPrefix() & Me(0).ProcNameSuffix() & "Bulk")
      cProc.Parameters.AddWithValue("@Data", Nothing).SqlType = SqlDbType.Xml
      If ProcSetup IsNot Nothing Then ProcSetup(cProc)
      Dim BatchName As String = Guid.NewGuid.ToString("n")

      Dim sb As Text.StringBuilder = Nothing
      Dim xmlW As System.Xml.XmlWriter = Nothing
      Dim xmlSettings As New System.Xml.XmlWriterSettings
      xmlSettings.Encoding = System.Text.Encoding.UTF8

      Dim RecordCount As Integer = 0

      Try
        Dim InitBuilder = Sub()
                            sb = New Text.StringBuilder

                            xmlW = System.Xml.XmlWriter.Create(sb, xmlSettings)
                            xmlW.WriteStartDocument()
                            xmlW.WriteStartElement("Rows")
                          End Sub

        Dim CommitBatch = Sub()
                            xmlW.WriteEndElement()
                            xmlW.WriteEndDocument()
                            xmlW.Flush()

                            Dim XmlParam As String = sb.ToString
                            cProc.Parameters("@Data").Value = XmlParam

                            cProc.ExecuteBatch(BatchName)

                            RecordCount = 0
                          End Sub

        InitBuilder()

        For Each item As C In Me
          If UpdateMethod = BulkUpdateMethod.AllRecords OrElse
            (UpdateMethod = BulkUpdateMethod.InsertOnly AndAlso item.IsNew) OrElse
            (UpdateMethod = BulkUpdateMethod.UpdateOnly AndAlso Not item.IsNew) Then

            xmlW.WriteStartElement("Row")
            RecordCount += 1

            Dim cmd As New SqlClient.SqlCommand
            item.SetupSaveCommand(cmd)

            For Each param As SqlClient.SqlParameter In cmd.Parameters
              If param.Direction <> ParameterDirection.Output Then
                If Not Singular.Misc.IsNullNothing(param.Value) Then xmlW.WriteStartAttribute(param.ParameterName.Replace("@", ""))
                If TypeOf param.Value Is Date Then
                  xmlW.WriteValue(CDate(param.Value).ToString("yyyyMMdd HH:mm:ss")) 'By default xml writer adds the timezone info. The default toString of Date doesn't.
                ElseIf Singular.Misc.IsNullNothing(param.Value) Then
                  'do nothing
                Else
                  xmlW.WriteValue(param.Value)
                End If

                If Not Singular.Misc.IsNullNothing(param.Value) Then xmlW.WriteEndAttribute()
              End If

            Next

            xmlW.WriteEndElement()
            xmlW.Flush()

            If sb.Length >= BatchSizeKB * 1024 Then
              CommitBatch()
              InitBuilder()
            End If

          End If
        Next

        CommitBatch()
        Singular.CommandProc.CommitBatch(BatchName)

        For Each it As C In Me
          it.MarkOld()
        Next

      Catch ex As Exception
        Singular.CommandProc.RollbackBatch(BatchName)
        Throw ex
      End Try

    End If

  End Sub

  Protected Sub SetArithAbortOn(cn As SqlClient.SqlConnection)

    Dim CmdArithAbort As New SqlClient.SqlCommand("SET ARITHABORT ON", cn)
    CmdArithAbort.CommandType = System.Data.CommandType.Text
    CmdArithAbort.ExecuteNonQuery()

  End Sub

  Public Shared Event UpdateDropDowns(ByVal sender As Object, ByVal e As EventArgs)

  Public Shared Function GetDropDownInfo(ByVal Context As String) As DropDownInfo

    Return New DropDownInfo(GetType(C))

  End Function

  Public Shared Function FetchList(ByVal Criteria As Object) As T

    Return Csla.DataPortal.Fetch(Of T)(Criteria)

  End Function

  Protected Overrides Sub DataPortal_OnDataPortalInvokeComplete(e As DataPortalEventArgs)
    MyBase.DataPortal_OnDataPortalInvokeComplete(e)

    If e.Operation = DataPortalOperations.Fetch Then Localisation.Data.ObjectVisitor.FetchData(Me)
    If e.Operation = DataPortalOperations.Update Then Localisation.Data.ObjectVisitor.SaveData(Me)
  End Sub

  ''' <summary>
  ''' Returns the first item in the list. If there is more than 1 item, and exception is thrown.
  ''' </summary>
  Public Function FirstAndOnly() As C
    If Me.Count > 1 Then
      Throw New Exception("List contains more than 1 item")
    ElseIf Me.Count = 1 Then
      Return Me(0)
    Else
      Return Nothing
    End If
  End Function

  ''' <summary>
  ''' Replaces the items in this list with items where the guids match. Useful if saving in a different list, and you need to merge the items back into the original list.
  ''' </summary>
  Public Sub ReplaceOrAdd(ByVal Item As ISingularBase)
    For i As Integer = 0 To Me.Count - 1
      If Me(i).Guid = Item.Guid Then
        Me(i) = Item
        Exit Sub
      End If
    Next
    Me.Add(Item)
  End Sub

  ''' <summary>
  ''' Replaces the items in this list with items where the guids match. Useful if saving in a different list, and you need to merge the items back into the original list.
  ''' </summary>
  Public Sub ReplaceOrAdd(ByVal List As ISingularBusinessListBase)
    For Each Item As ISingularBusinessBase In List
      ReplaceOrAdd(Item)
    Next
  End Sub

	Public Function GetExcelDocumentStream() As IO.Stream
		Dim ee As New Singular.Data.ExcelExporter
		ee.PopulateData(Me, True, True)
		Return ee.GetStream()
	End Function

	Public Function GetExcelDocumentStream(IncludeChildren As Boolean, Optional UseDropDownTextValue As Boolean = False) As IO.Stream
		Dim ee As New Singular.Data.ExcelExporter
		ee.PopulateData(Me, Not IncludeChildren, Not IncludeChildren, "", 0, False, UseDropDownTextValue)
		Return ee.GetStream()
	End Function

	Public Function GetDataset(IncludeChildren As Boolean) As DataSet
    Return Singular.CSLALib.GetDatasetFromBusinessListBase(Me, Not IncludeChildren, Not IncludeChildren)
  End Function

  Public Sub CheckRules()
    For Each child As C In Me
      child.CheckRules()
    Next
  End Sub

  Public Sub CheckAllRules()
    For Each child As C In Me
      child.CheckAllRules()
    Next
  End Sub

#End If

  Public Function TrySave() As SaveHelper Implements IParentSavable.TrySave

    Return New SaveHelper().Save(Me, IsValid, IsDirty)

  End Function


End Class
