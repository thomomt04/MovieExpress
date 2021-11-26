Imports Csla
Imports Csla.Core
Imports Csla.Properties
Imports Csla.Serialization
Imports System.Reflection
Imports Singular.ObjectErrors
Imports Singular.DataAnnotations
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

#Region " UI Context, CanDeleteArgs, CanArgs "

#Region " CanDeleteArgs "

Public Class CanDeleteArgs
  Inherits UIContextArgs

  Public Enum CanDeleteResultType
    CanDelete
    CantDelete
    Warning
  End Enum

  Public Property CanDeleteResult As CanDeleteResultType

  Public Property CustomError As Boolean

  Public Property Detail As String = ""

  Public Sub New(ByVal CanDeleteResult As CanDeleteResultType, ByVal Detail As String, Optional CustomError As Boolean = False)

    Me.CanDeleteResult = CanDeleteResult
    Me.Detail = Detail
    Me.CustomError = CustomError

  End Sub

End Class

#End Region

#Region " CanArgs "

Public Class CanEditFieldArgs

  Public Property Reason As String = ""
  Public Property DisableEditButtons As Boolean = True
  Public Property EditorButtonsToLeaveEnabled() As String = Nothing

  Public Sub New()

  End Sub

End Class

#If Silverlight = False Then

<Serializable()>
Public Class UIContextList
  Inherits Hashtable

  Public Sub AddContext(Name As String)
    Me(Name) = True
  End Sub

  Public Function HasContext(Context As String) As Boolean
    Return ContainsKey(Context) AndAlso Me(Context)
  End Function

End Class

Public Class PropertyList(Of ChildType)
  Inherits List(Of PropertyInfo)

  Public Sub AddProperty(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildType, Object)))
    Dim mi As MemberInfo = Singular.Reflection.GetMember(Of ChildType)(le)
    If mi.MemberType = MemberTypes.Property Then
      Add(mi)
    End If
  End Sub

End Class

#End If

Public Class UIContext

  Public Property Name As String

End Class

Public Class CanEditArgs

  Public Property Context As UIContext

  Public Property CantEditReason As String = ""

  Public Property UserNotified As Boolean = False

End Class

#End Region

Public Class UIContextArgs
  Inherits EventArgs



End Class

#End Region


#Region " SingularBusinessBase"

<Serializable()> _
Public Class SingularBusinessBase(Of C As SingularBusinessBase(Of C))
  Inherits BusinessBase(Of C)
  Implements ISingularBusinessBase

  'Friend Function GetDescriptionProperties(ByVal Context As UIContext) As List(Of IPropertyInfo)

  '  For Each pi As IPropertyInfo In FieldManager.GetRegisteredProperties
  '    If pi.Type.Equals(GetType(String)) Then
  '      Return New List(Of IPropertyInfo) From {pi}
  '    End If
  '  Next
  '  Return Nothing

  'End Function

#Region " Edit Levels "

  Public Sub FlattenEditLevels() Implements ISavable.FlattenEditLevels

    Dim BaseEditLevel As Integer = Me.GetEditLevel()

    Dim Context As New EditLevelContext() With {.EditLevel = Me.GetEditLevel}
    RecurseObjectGraphAndPerformAction(AddressOf FlattenEditLevel, Context, True)

  End Sub

  Private Sub FlattenEditLevel(ByVal Target As Object, ByVal Context As AbortableActionContext)

    Dim EditLevelContext As EditLevelContext = Context

    Dim mi = Target.GetType.GetMethod("GetEditLevel")

    If mi IsNot Nothing Then
      While CInt(mi.Invoke(Target, Nothing)) > EditLevelContext.EditLevel
        Target.ApplyEdit()
      End While

      While CInt(mi.Invoke(Target, Nothing)) < EditLevelContext.EditLevel
        Target.BeginEdit()
      End While
    End If

  End Sub

  Public Function GetEditLevel() As Integer Implements ISingularBusinessBase.GetEditLevel
    Return MyBase.EditLevel
  End Function

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


#End Region

#Region " Integrity Checks: Can Delete "

  Protected Overridable ReadOnly Property TableName() As String Implements ISingularBusinessBase.TableName
    Get
      Return CSLALib.GetTableName(Me.GetType.Name)
    End Get
  End Property

  Public Function GetTableName() As String Implements ISingularBase.GetTableName
    Return TableName
  End Function

  Protected Overridable ReadOnly Property TableReferencesToIgnore() As String()
    Get
      If FieldManager.GetChildren.Count > 0 Then
        'If there are children, add them as references to ignore (as they should have cascade deletes)
        Dim IgnoreReferences(FieldManager.GetChildren.Count - 1) As String
        Dim i As Integer = 0
        For Each child In FieldManager.GetChildren
          IgnoreReferences(i) = CSLALib.GetTableName(child.GetType.Name)
          i += 1
        Next
        Return IgnoreReferences
      Else
        Return New String() {}
      End If
    End Get
  End Property

  <NotUndoable(), NonSerialized()> Protected mTableReferenceList As CSLALib.TableReferenceList
  Private mFirstTimeTableReferenceList As Boolean = True

  Protected Overridable Function TableReferences(Optional ByVal Fetch As Boolean = True) As CSLALib.TableReferenceList

    ' If IsNothing(mTableReferenceList) OrElse mJustGivenReasons Then
    If Fetch OrElse mFirstTimeTableReferenceList OrElse mTableReferenceList Is Nothing Then
#If SILVERLIGHT Then
      Me.MarkBusy()
      CSLALib.TableReferenceList.BeginGetTableReferenceList(Me.TableName, Me.GetIdValue, Sub(o, e)
                                                                                           If e.Error IsNot Nothing Then
                                                                                             Throw e.Error
                                                                                           End If

                                                                                           mTableReferenceList = e.Object

                                                                                           TableReferencesLoaded()
                                                                                         End Sub)
#Else
      mTableReferenceList = CSLALib.TableReferenceList.GetTableReferenceList(Me.TableName, Me.GetIdValue)
      TableReferencesLoaded()

#End If
      mFirstTimeTableReferenceList = False
    End If

    Return mTableReferenceList

  End Function

  Protected Overridable Sub TableReferencesLoaded()

    Me.MarkIdle()
    If mTableReferenceList.HasReferences(Me.TableReferencesToIgnore) Then
      mCanDeleteCallBack.Invoke(Me, New CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CantDelete, mTableReferenceList.GetReferenceListAsString(Me.TableReferencesToIgnore, 2)))
    Else
      mCanDeleteCallBack.Invoke(Me, New CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CanDelete, ""))
    End If

    mCanDeleteCallBack = Nothing

  End Sub

#If SILVERLIGHT Then
  Protected mCanDeleteCallBack As EventHandler(Of CanDeleteArgs)
#Else
  <NonSerialized()> Protected mCanDeleteCallBack As EventHandler(Of CanDeleteArgs)
#End If


  Public Overridable Sub CanDelete(ByVal CallBack As EventHandler(Of CanDeleteArgs))

    If Me.IsNew Then
      CallBack.Invoke(Me, New CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CanDelete, ""))
    Else
      mCanDeleteCallBack = CallBack
#If SILVERLIGHT Then
        Me.TableReferences() ' this will call the callback
#Else
      If Singular.Settings.CheckTableReferences Then
        Me.TableReferences() ' this will call the callback
      Else
        CallBack.Invoke(Me, New CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CanDelete, ""))
        mCanDeleteCallBack = Nothing
      End If
#End If
    End If

  End Sub

#If Silverlight = False Then

  Public Overridable Function CanDelete() As CanDeleteArgs Implements ISingularBusinessBase.CanDelete

    Dim cda As CanDeleteArgs = Nothing
    CanDelete(Sub(o, e)
                cda = e
              End Sub)
    Return cda

  End Function

#End If

  ' ''' <summary>
  ' ''' Returns can delete args non asynchronously.
  ' ''' </summary>
  'Public Function CanDelete(Optional TabIndentCount As Integer = 0) As Singular.CanDeleteArgs

  '  If TableReferences(False).HasReferences(Me.TableReferencesToIgnore) Then
  '    Return New CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CantDelete, TableReferences(False).GetReferenceListAsString(Me.TableReferencesToIgnore, TabIndentCount))
  '  Else
  '    Return New CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CanDelete, "")
  '  End If

  'End Function

#End Region

#Region " Authorisation Checks: Cans "

#Region " Cans "

  Public Overridable Function CanEditField(ByVal FieldName As String, ByVal Args As Singular.CanEditFieldArgs) As Boolean Implements ISingularBusinessBase.CanEditField

    Return True

  End Function

#End Region

#End Region

#Region " Helper Functions "

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

  Protected Function GetSmartDateProperty(ByVal PropertyInfo As PropertyInfo(Of SmartDate)) As Nullable(Of DateTime)

    Dim value = GetProperty(PropertyInfo)
    If value.IsEmpty Then
      Return Nothing
    Else
      Return value.Date
    End If

  End Function

  Protected Function GetFilename(ByVal ID As Integer) As String

    Return String.Format(Me.GetType.Name & "-{0}.xml", ID.ToString)

  End Function

  Public Overloads Sub SetParent(ByVal Parent As Csla.Core.IParent)

    MyBase.SetParent(Parent)

  End Sub

  Public Sub CheckRules() Implements ISingularBusinessBase.CheckRules

    BusinessRules.CheckRules()

  End Sub

  Public Sub RecurseObjectGraphAndPerformAction(ByVal Action As AbortableAction, ByVal Context As AbortableActionContext, Optional ByVal PerformOnListObjects As Boolean = False)

#If Silverlight = False Then
    If Context Is Nothing Then Context = New AbortableActionContext
    Dim Iterator As New CSLALib.ObjectIterator(PerformOnListObjects, True, False, Action)
    Iterator.Context = Context
    Iterator.RecurseObjectGraphAndPerformAction(Me, Singular.ReflectionCached.GetCachedType(Me.GetType))
#End If

  End Sub

  ''' <summary>
  ''' Returns the provided text if it is not blank, Otherwise returns New Object / Blank Object.
  ''' </summary>
  Public Function ToStringHelper(ByVal PopulatedText As String, ByVal ObjectReadableName As String) As String
    If PopulatedText.Trim = "" Then
      If IsNew Then
        Return String.Format("New {0}", ObjectReadableName)
      Else
        Return String.Format("Blank {0}", ObjectReadableName)
      End If
    Else
      Return PopulatedText
    End If

  End Function

#End Region

#Region " Drop Down Help "

  Public Overridable Function GetDropDownFilterPredicate(ByVal Parameter As String) As Func(Of Object, Boolean)

    Throw New Exception("Override the GetDropDownFilterPredicate in the " & Me.GetType.Name & " class")

  End Function

#End Region

#Region " Rules "

  Protected Overrides Sub AddBusinessRules()
    MyBase.AddBusinessRules()

    For Each prop In Me.GetType.GetProperties()
      Dim attList = prop.GetCustomAttributes(GetType(Singular.DataAnnotations.DropDownField), True)
      For Each att In attList
        Dim ddf As DataAnnotations.DropDownField = TryCast(att, DataAnnotations.DropDownField)
        If ddf.FilterContext IsNot Nothing AndAlso ddf.FilterContext.DirectFilterValue Is Nothing AndAlso ddf.FilterContext.DependantField IsNot Nothing Then
          ' add dependancy between objects
          Dim FieldName As String = prop.Name
          Dim Prop1 As IPropertyInfo = FieldManager.GetRegisteredProperties.First(Function(c) c.Name = ddf.FilterContext.DependantField)
          Dim Prop2 As IPropertyInfo = FieldManager.GetRegisteredProperties.First(Function(c) c.Name = FieldName)
          BusinessRules.AddRule(New Csla.Rules.CommonRules.Dependency(Prop1, Prop2))
        End If
      Next
    Next

  End Sub

#If Silverlight = False Then

  Protected Function AddBusinessRule(Of R As Csla.Rules.IBusinessRule)(ByVal Rule As R) As R
    BusinessRules.AddRule(Rule)
    Return Rule
  End Function

  Protected Function AddWebRule(ByVal RuleProperty As IPropertyInfo,
                  ByVal RuleFailExpression As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                  ByVal ErrorExpression As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                  Optional Severity As Singular.Rules.RuleSeverity = Rules.RuleSeverity.Error) As Singular.Rules.JavascriptRule(Of C)

    Return AddBusinessRule(New Singular.Rules.JavascriptRule(Of C)(RuleProperty, RuleFailExpression, ErrorExpression, Severity))

  End Function

  Protected Function AddWebRule(ByVal RuleProperty As IPropertyInfo, Optional Severity As Singular.Rules.RuleSeverity = Rules.RuleSeverity.Error) As Singular.Rules.JavascriptRule(Of C)

    Return AddBusinessRule(New Singular.Rules.JavascriptRule(Of C)(RuleProperty, Severity))

  End Function

  ''' <summary>
  ''' Checks that the items in a child list of this object are unique.
  ''' </summary>
  ''' <typeparam name="ChildType">The single object type of the child list</typeparam>
  ''' <param name="RuleProperty">The property to add the rule to</param>
  ''' <param name="ErrorDescription">Description if the rule is broken</param>
  ''' <param name="List">The list to check for uniqueness</param>
  ''' <param name="Properties">The properties that must be unique</param>
  Protected Sub AddDuplicateRule(Of ChildType)(RuleProperty As Csla.Core.IPropertyInfo,
                                             ErrorDescription As String,
                                             List As Expressions.Expression(Of Func(Of C, IList)),
                                             ParamArray Properties() As Expressions.Expression(Of Func(Of ChildType, Object)))

    With AddWebRule(RuleProperty, Singular.Rules.RuleSeverity.Error)

      Dim CProperties(Properties.Length - 1) As Func(Of ChildType, Object)
      Dim PropNames As String = ""

      'Get the compiled get functions and property names for the unique properties.
      For i As Integer = 0 To Properties.Length - 1
        CProperties(i) = Properties(i).Compile
        PropNames = PropNames & If(PropNames = "", "", ",") & "'" & Singular.Reflection.GetMemberSpecific(Properties(i)).Name & "'"
      Next

      .ServerRuleFunction =
        Function(Parent)

          'Dictionary to hold keys
          Dim UIndex As New Dictionary(Of String, Integer)

          For Each Item As ChildType In List.Compile()(Parent)
            'for each item, create a key by concatenating the property values together
            Dim Key As String = ""
            For i As Integer = 0 To CProperties.Length - 1
              Dim Value = CProperties(i)(Item)
              Key &= If(Value Is Nothing, "", Value.ToString.ToLower) & Chr(30)
            Next
            'check if the key already exists in the dictionary, else add it.
            Dim Count As Integer = 0
            If UIndex.TryGetValue(Key, Count) Then
              Return ErrorDescription
            Else
              UIndex.Add(Key, 1)
            End If
          Next

          Return ""
        End Function

      'Same code as above exists in Singular.Validation.js
      .JavascriptRuleCode = "Singular.Validation.Rules.CheckDuplicate(Value, { List: self." & Singular.Reflection.GetMemberSpecific(List).Name & "(), Properties: [" &
        PropNames & "], Desc: '" & ErrorDescription & "'}, CtlError)"

    End With

  End Sub

#End If

  ''' <summary>
  ''' Checks the rules in this object, and all child objects. Returns true if Valid
  ''' </summary>
  Public Function CheckAllRules() As Boolean

    Me.CheckRules()

    For Each p In Me.GetType.GetProperties()
      If Singular.Reflection.TypeImplementsInterface(p.PropertyType, GetType(ISingularBusinessBase)) OrElse Singular.Reflection.TypeImplementsInterface(p.PropertyType, GetType(ISingularBusinessListBase)) Then
        Dim child = p.GetValue(Me, Nothing)
        If child IsNot Nothing Then
          child.CheckAllRules()
        End If
      End If
    Next

    Return IsValid

  End Function

  ''' <summary>
  ''' If this returns true, your object will be allowed to save, as all errors will be treated as warnings.
  ''' To change this value dynamically, create a backing field using RegisterReadOnlyProperty.
  ''' </summary>
#If SILVERLIGHT Then
  <Browsable(False)>
  Public Overridable ReadOnly Property ErrorsAsWarnings As Boolean
    Get
      Return False
    End Get
  End Property
#Else
  <Browsable(False), SetExpression("Singular.Validation.CheckRules(self)")>
  Public Overridable ReadOnly Property ErrorsAsWarnings As Boolean
    Get
      Return False
    End Get
  End Property
#End If

  Public Overrides ReadOnly Property IsSelfValid As Boolean
    Get
      Return MyBase.IsSelfValid OrElse ErrorsAsWarnings
    End Get
  End Property

#End Region

#Region " Errors "

  Public Overridable Function GetErrorsAsBusinessNode() As Singular.ObjectErrors.BusinessObjectNode

    Dim node As New Singular.ObjectErrors.BusinessObjectNode(Me)
    Return node

  End Function

  Public Overridable Function GetErrorsAsString() As String Implements ISavable.ErrorsAsString

    Return ObjectErrors.GetErrorsAsString(GetErrorsAsBusinessNode, 0)

  End Function

  Public Overridable Function GetErrorsAsHTMLString() As String Implements ISavable.ErrorsAsHTMLString

    Dim IndentLevel = 0
    Dim Errors As String = "<ul>" & vbCrLf
    Errors &= "<li>" & ObjectErrors.GetErrorsAsHTMLString(GetErrorsAsBusinessNode, IndentLevel) & "</li>" & vbCrLf
    Errors &= "</ul>" & vbCrLf
    Return Errors

  End Function


#End Region

#Region " Get ID "

  Public Function GetId() As Object
    Return Me.GetIdValue()
  End Function

  Protected Overrides Function GetIdValue() As Object Implements ISingularBusinessBase.GetIdValue

    Dim pi = GetIDProperty()
    If pi Is Nothing Then
      Return MyBase.GetIdValue
    Else
      Return GetProperty(pi)
    End If

  End Function

  Public Overrides Function Equals(ByVal obj As Object) As Boolean

    Dim ib = TryCast(obj, ISingularBase)
    If ib Is Nothing Then
      Return False
    End If
    Dim ID As Object = ib.GetIdValue
    If ib IsNot Nothing AndAlso ID IsNot Nothing Then
      If TypeOf ID Is Integer AndAlso ID = 0 Then
        Return MyBase.Equals(obj)
      Else
        Return Singular.Misc.CompareSafe(Me.GetIdValue, ib.GetIdValue)
      End If
    Else
      Return MyBase.Equals(obj)
    End If

  End Function

  Public Overridable Function GetIDProperty() As IPropertyInfo

    For Each pi In FieldManager.GetRegisteredProperties
      If pi.Type.Equals(GetType(Integer)) AndAlso pi.Name.EndsWith("ID") AndAlso GetType(C).Name.EndsWith(pi.Name.Substring(0, pi.Name.Length - 2)) Then
        Return pi
      End If
    Next
    Return Nothing

  End Function

#End Region

#Region " Data Access "

#If SILVERLIGHT Then

#Else

  ''' <summary>
  ''' Will try save this object on its own, by creating a new list of this objects parent type, and adding the object to the list.
  ''' </summary>
  ''' <remarks>B Marlborough.</remarks>
  Public Function SaveDirectly() As Object

    Dim OldParent As Object = Me.Parent

    If OldParent Is Nothing Then
      'If this doesn't have a parent, then save it directly and hope its not marked as child.
      Return Me.Save()
    Else
      'Otherwise create a fake parent list.
      'this is a possible cause for edit level mismatch errors
      Dim ParentType As Type = Me.Parent.GetType
      Dim FakeList As Object = Activator.CreateInstance(ParentType, True)
      FakeList.Add(Me)
      FakeList = FakeList.Save
      Return FakeList(0)
    End If

  End Function

  Protected Overridable Sub DoInsertUpdateChild(ByVal cm As SqlClient.SqlCommand, Optional Callback As Action = Nothing)

    Dim Connection As SqlClient.SqlConnection = Csla.ApplicationContext.LocalContext("cn")
    Dim Transaction As SqlClient.SqlTransaction = Csla.ApplicationContext.LocalContext("tr")

    If cm IsNot Nothing Then
      cm.Connection = Connection
      cm.Transaction = Transaction
    End If

    If Connection Is Nothing Then
      'Changed by Marlborough Dec 2012.
      'If the connection has not been set up, then the programmer is doing something like SingleObject.Save, but the update method is calling DoInsertUpdateChild.
      'If so, then just call DoInsertUpdateParent and save the hassle. 
      'Otherwise, SingularBusinessBase should decide for you, If IsChild DoInsertUpdateChild() Else DoInsertUpdateParent()
      DoInsertUpdateParent(cm, GetConnectionString, Callback)

    Else
      'Normal Child Object
      BeforeInsertUpdate(Connection, Transaction)

      If Callback Is Nothing Then
        InsertUpdate(cm)
      Else
        Callback()
      End If

    End If

  End Sub

  Protected Overridable Sub DoDeleteChild(ByVal cm As SqlClient.SqlCommand)

    cm.Connection = CType(Csla.ApplicationContext.LocalContext("cn"), SqlClient.SqlConnection)
    cm.Transaction = Csla.ApplicationContext.LocalContext("tr")

    If cm.Connection Is Nothing Then

      DoDeleteParent(cm)

    Else

      CSLALib.ContextInfo.SetContextInfoOnConnection(Of C)(cm.Connection, cm.Transaction)
      DeleteFromDB(cm)

    End If

  End Sub

  Public Overridable Function GetConnectionString() As String
    Return Settings.ConnectionString
  End Function

  ''' <summary>
  ''' The isolation level to use when starting a transaction in DoInsertUpdateParent
  ''' </summary>
  Protected Overridable Function TransactionIsolationLevel() As IsolationLevel
    Return IsolationLevel.ReadUncommitted
  End Function

  Protected Overridable Sub DoInsertUpdateParent(ByVal cm As SqlClient.SqlCommand)

    DoInsertUpdateParent(cm, GetConnectionString)

  End Sub

  Protected Overridable Sub DoInsertUpdateParent(ByVal cm As SqlClient.SqlCommand, ByVal ConnectionString As String)
    DoInsertUpdateParent(cm, ConnectionString, Nothing)
  End Sub

  Protected Overridable Sub DoInsertUpdateParent(ByVal cm As SqlClient.SqlCommand, ByVal ConnectionString As String, CallBack As Action)

    Dim cn As SqlClient.SqlConnection = New SqlClient.SqlConnection(ConnectionString)
    Csla.ApplicationContext.LocalContext("cn") = cn
    Try
      cn.Open()
      Dim tr As SqlClient.SqlTransaction = cn.BeginTransaction(TransactionIsolationLevel, Left(Me.GetType.Name, 32))
      Csla.ApplicationContext.LocalContext("tr") = tr
      Try
        If cm IsNot Nothing Then
          cm.Connection = cn
          cm.Transaction = tr
        End If

        BeforeInsertUpdate(cn, tr)

        If cm IsNot Nothing Then
          InsertUpdate(cm)
        Else
          CallBack()
        End If

        tr.Commit()
      Catch ex As Exception
        tr.Rollback()
        Throw ex
      End Try
    Finally
      Csla.ApplicationContext.LocalContext("cn") = Nothing
      If cn.State = ConnectionState.Open Then
        cn.Close()
      End If
      Csla.ApplicationContext.LocalContext("tr") = Nothing
    End Try

  End Sub

  Protected Overridable Sub DoDeleteParent(ByVal cm As SqlClient.SqlCommand)

    DoDeleteParent(cm, GetConnectionString)

  End Sub

  Protected Overridable Sub DoDeleteParent(ByVal cm As SqlClient.SqlCommand, ByVal ConnectionString As String)

    Dim cn As SqlClient.SqlConnection = New SqlClient.SqlConnection(ConnectionString)
    Csla.ApplicationContext.LocalContext("cn") = cn
    Try
      cn.Open()
      Dim tr As SqlClient.SqlTransaction = cn.BeginTransaction(IsolationLevel.ReadCommitted, Left(Me.GetType.Name, 32))
      Csla.ApplicationContext.LocalContext("tr") = tr
      Try
        cm.Connection = cn
        cm.Transaction = tr

        CSLALib.ContextInfo.SetContextInfoOnConnection(Of C)(cn, tr)

        DeleteFromDB(cm)

        tr.Commit()
      Catch ex As Exception
        tr.Rollback()
        Throw ex
      End Try
    Finally
      Csla.ApplicationContext.LocalContext("cn") = Nothing
      If cn.State = ConnectionState.Open Then
        cn.Close()
      End If
      Csla.ApplicationContext.LocalContext("tr") = Nothing
    End Try

  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()

    Dim pi = Me.GetType.GetMethod("DeleteSelf", BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
    If pi Is Nothing Then
      DeleteSelfGeneric()
      'Throw New Exception("Business object must implement DeleteSelf method")
    Else
      pi.Invoke(Me, Nothing)
    End If

  End Sub

  ''' <summary>
  ''' Loads properties with data from the data reader. A list of properties can be specified without indexing each property.
  ''' </summary>
  Protected Function LoadMultipleProperties(Sdr As Csla.Data.SafeDataReader, StartIndex As Integer, ParamArray Properties() As Csla.Core.IPropertyInfo) As Integer

    Dim i As Integer
    For i = 0 To Properties.Length - 1
      LoadProperty(Properties(i), Sdr.GetValue(i + StartIndex))
    Next
    Return i

  End Function

  Private Sub BeforeInsertUpdate(Connection As SqlClient.SqlConnection, Transaction As SqlClient.SqlTransaction)

    If IsSelfDirty AndAlso LocalisationDataValues IsNot Nothing Then
      'Object will currently have localised text in its string properties.
      'We need to get it back to the default language, and store the localised property values to save afterwards.
      LocalisationDataValues.SwapValues(Me)
    End If

    CSLALib.ContextInfo.SetContextInfoOnConnection(Of C)(Connection, Transaction)

  End Sub

#Region " Generic Insert / Update / Delete "

  Protected Friend Overridable Function GetSPPrefix() As String
    Return If(IsNew, "InsProcs.ins", "UpdProcs.upd")
  End Function

  Protected Friend Overridable Function GetSPDeletePrefix() As String
    Return "DelProcs.del"
  End Function

  Protected Friend Overridable Function ProcNameSuffix() As String
    Return Me.GetType.Name
  End Function

  Protected Function AddPrimaryKeyParam(cm As SqlClient.SqlCommand, KeyProperty As Csla.PropertyInfo(Of Integer)) As SqlClient.SqlParameter
    If IsNew Then
      Dim param = cm.Parameters.Add("@" & KeyProperty.Name, SqlDbType.Int)
      param.Direction = ParameterDirection.Output
      Return param
    Else
      Return cm.Parameters.AddWithValue("@" & KeyProperty.Name, GetProperty(KeyProperty))
    End If
  End Function

  Protected Function AddPrimaryKeyParam(cm As SqlClient.SqlCommand, KeyProperty As Csla.PropertyInfo(Of Guid)) As SqlClient.SqlParameter
    If IsNew Then
      Dim param = cm.Parameters.Add("@" & KeyProperty.Name, SqlDbType.UniqueIdentifier)
      param.Direction = ParameterDirection.Output
      Return param
    Else
      Return cm.Parameters.AddWithValue("@" & KeyProperty.Name, GetProperty(KeyProperty))
    End If
  End Function

  Protected Function AddPrimaryKeyParam(cm As SqlClient.SqlCommand, KeyProperty As Csla.PropertyInfo(Of Long)) As SqlClient.SqlParameter
    If IsNew Then
      Dim param = cm.Parameters.Add("@" & KeyProperty.Name, SqlDbType.BigInt)
      param.Direction = ParameterDirection.Output
      Return param
    Else
      Return cm.Parameters.AddWithValue("@" & KeyProperty.Name, GetProperty(KeyProperty))
    End If
  End Function

  Protected Overrides Sub DataPortal_Insert()
    InsertUpdateGeneric()
  End Sub

  Protected Overrides Sub DataPortal_Update()
    InsertUpdateGeneric()
  End Sub

  Protected Friend Overridable Sub InsertUpdateGeneric() Implements ISingularBusinessBase.InsertUpdateGeneric
    Using cm As New SqlClient.SqlCommand
      DoInsertUpdateChild(cm)
    End Using
  End Sub

  Protected Overridable Sub DoUnitOfWork(Callback As action)

    DoInsertUpdateChild(Nothing, Callback)

  End Sub

  ''' <summary>
  ''' Checks if the child list is not nothing, then calls UpdateGeneric on the child list.
  ''' </summary>
  Protected Sub UpdateChild(ChildList As ISingularBusinessListBase)
    If ChildList IsNot Nothing Then
      ChildList.UpdateGeneric()
    End If
  End Sub

  Protected Sub UpdateChild(ChildObject As ISingularBusinessBase)
    If ChildObject IsNot Nothing Then
      ChildObject.InsertUpdateGeneric()
    End If
  End Sub

  Public Enum UpdateType
    None = 0
    ChildrenOnly = 1
    All = 2
  End Enum

  Protected Overridable Function CanUpdate() As UpdateType
    If IsSelfDirty Then
      Return UpdateType.All
    ElseIf IsDirty Then
      Return UpdateType.ChildrenOnly
    Else
      Return UpdateType.None
    End If
  End Function

  Protected Property DuplicateKeyDescription As String = Nothing

  Protected Friend Overridable Function SetupSaveCommand(cm As SqlClient.SqlCommand) As Action(Of SqlClient.SqlCommand)
    Throw New Exception("Business object must override SetupSaveCommand")
  End Function

  Protected Overridable Sub InsertUpdate(ByVal cm As SqlClient.SqlCommand)

    Dim uo = CanUpdate()

    If uo = UpdateType.All Then
      cm.CommandType = CommandType.StoredProcedure
      cm.CommandText = GetSPPrefix() & ProcNameSuffix()
      Dim PostSaveMethod = SetupSaveCommand(cm)

      Try
        cm.ExecuteNonQuery()
      Catch ex As SqlClient.SqlException
        If Not String.IsNullOrEmpty(DuplicateKeyDescription) AndAlso ex.Number = 2601 Then
          Throw New Exception("CustomError: " & DuplicateKeyDescription)
        End If
        Throw ex
      End Try

      If PostSaveMethod IsNot Nothing Then
        PostSaveMethod(cm)
      End If
      SaveChildren()
      MarkOld()
    ElseIf uo = UpdateType.ChildrenOnly Then
      SaveChildren()
    End If

  End Sub

  Protected Overridable Sub SaveChildren()

  End Sub

  Public Overridable Sub DeleteSelfGeneric()
    If Not IsNew Then
      Using cm As New SqlClient.SqlCommand
        cm.CommandText = GetSPDeletePrefix() & ProcNameSuffix()
        cm.CommandType = CommandType.StoredProcedure
        SetupDeleteCommand(cm)
        DoDeleteChild(cm)
      End Using
    End If
  End Sub

  Protected Overridable Sub SetupDeleteCommand(cm As SqlClient.SqlCommand)
    Throw New Exception("Business object must override SetupDeleteCommand")
  End Sub

  Protected Overridable Sub DeleteFromDB(ByVal cm As SqlClient.SqlCommand)

    If Me.IsNew Then Exit Sub
    cm.ExecuteNonQuery()
    MarkNew()

  End Sub

#End Region

  Protected Overrides Sub DataPortal_OnDataPortalInvokeComplete(e As DataPortalEventArgs)
    MyBase.DataPortal_OnDataPortalInvokeComplete(e)

    If e.Operation = DataPortalOperations.Fetch Then Localisation.Data.ObjectVisitor.FetchData(Me)
    If e.Operation = DataPortalOperations.Update Then Localisation.Data.ObjectVisitor.SaveData(Me)

  End Sub

#End If

#End Region

#Region " Create Object "

#If SILVERLIGHT Then
  Public NotOverridable Overrides Sub Child_Create()
#Else
  Protected NotOverridable Overrides Sub Child_Create()
#End If

    MyBase.Child_Create()

    'Set Created by to the current user name / user id
    Dim CreatedBy = FieldManager.GetRegisteredProperties.FirstOrDefault(Function(f) f.Name = "CreatedBy")
    If CreatedBy IsNot Nothing Then
      If Singular.Reflection.GetGenericArgumentType(CreatedBy.GetType, 0).Equals(GetType(String)) Then
        If Singular.Security.CurrentIdentity IsNot Nothing Then
          Me.LoadProperty(CreatedBy, Singular.Security.CurrentIdentity.Name)
        Else
          Me.LoadProperty(CreatedBy, CInt(Csla.ApplicationContext.ClientContext("UserID")).ToString)
        End If
      Else
        If Singular.Security.CurrentIdentity IsNot Nothing Then
          Me.LoadProperty(CreatedBy, Singular.Security.CurrentIdentity.UserID)
        Else
          Me.LoadProperty(CreatedBy, CInt(Csla.ApplicationContext.ClientContext("UserID")))
        End If
      End If
    End If

    'Set Created Date to now
    Dim CreatedDate = FieldManager.GetRegisteredProperties.FirstOrDefault(Function(f) f.Name.StartsWith("CreatedDate"))
    If CreatedDate IsNot Nothing Then
      If CreatedDate.Type Is GetType(SmartDate) Then
        Me.LoadProperty(CreatedDate, New SmartDate(Now))
      Else
        Me.LoadProperty(CreatedDate, Now)
      End If
    End If

    'Set Modified by to the current user name / user id
    Dim ModifiedBy = FieldManager.GetRegisteredProperties.FirstOrDefault(Function(f) f.Name = "ModifiedBy")
    If ModifiedBy IsNot Nothing Then
      If Singular.Reflection.GetGenericArgumentType(ModifiedBy.GetType, 0).Equals(GetType(String)) Then
        If Singular.Security.CurrentIdentity IsNot Nothing Then
          Me.LoadProperty(ModifiedBy, Singular.Security.CurrentIdentity.Name)
        Else
          Me.LoadProperty(ModifiedBy, CInt(Csla.ApplicationContext.ClientContext("UserID")).ToString)
        End If
      Else
        If Singular.Security.CurrentIdentity IsNot Nothing Then
          Me.LoadProperty(ModifiedBy, Singular.Security.CurrentIdentity.UserID)
        Else
          Me.LoadProperty(ModifiedBy, CInt(Csla.ApplicationContext.ClientContext("UserID")))
        End If
      End If
    End If

    'Set Modified Date to now
    Dim ModifiedDate = FieldManager.GetRegisteredProperties.FirstOrDefault(Function(f) f.Name.StartsWith("ModifiedDate"))
    If ModifiedDate IsNot Nothing Then
      If ModifiedDate.Type Is GetType(SmartDate) Then
        Me.LoadProperty(ModifiedDate, New SmartDate(Now))
      Else
        Me.LoadProperty(ModifiedDate, Now)
      End If
    End If

    ' LoadProperty(GuidProperty, System.Guid.NewGuid)

    'Allow the object to set properties.
    OnCreate()

  End Sub

  Protected Overridable Sub OnCreate()

  End Sub

#End Region

#Region " Overrides and Overloads "

  <Display(AutoGenerateField:=False), ComponentModel.DefaultValue(True)>
  Public Overloads ReadOnly Property IsNew As Boolean Implements ISingularBusinessBase.IsNew
    Get
      Return MyBase.IsNew
    End Get
  End Property

  <Display(AutoGenerateField:=False), System.ComponentModel.Browsable(False)>
  Public Overloads ReadOnly Property IsChild As Boolean Implements ISingularBusinessBase.IsChild
    Get
      Return MyBase.IsChild
    End Get
  End Property

  <Display(AutoGenerateField:=False), System.ComponentModel.Browsable(False)>
  Public Overloads ReadOnly Property IsSelfDirty As Boolean Implements ISingularBusinessBase.IsSelfDirty
    Get
      Return MyBase.IsSelfDirty
    End Get
  End Property

  <Display(AutoGenerateField:=False), System.ComponentModel.Browsable(False)>
  Public Overrides ReadOnly Property IsValid As Boolean Implements ISingularBusinessBase.IsValid
    Get
      Return MyBase.IsValid
    End Get
  End Property

  Public Overloads Sub MarkAsChild()
    MyBase.MarkAsChild()
  End Sub

  Public Overloads Sub MarkOld() Implements ISingularBusinessBase.MarkOld
    MyBase.MarkOld()
  End Sub

  Public Overloads Sub MarkDirty() Implements ISingularBusinessBase.MarkDirty
    MyBase.MarkDirty()
  End Sub

  Public Overloads Sub MarkClean() Implements ISingularBusinessBase.MarkClean
    MyBase.MarkClean()
  End Sub

  ''' <summary>
  ''' Used for Singular Web library. In javascript, will return if the object was created on the client. On server, will return the same value as IsNew.
  ''' </summary>
  <Display(AutoGenerateField:=False),
  System.ComponentModel.Browsable(False)>
  Public ReadOnly Property IsClientNew As Boolean
    Get
      Return IsNew
    End Get
  End Property

  <Display(AutoGenerateField:=False),
  System.ComponentModel.Browsable(False)>
  Public Overloads ReadOnly Property FieldManager As Csla.Core.FieldManager.FieldDataManager Implements ISingularBusinessBase.FieldManager
    Get
      Return MyBase.FieldManager
    End Get
  End Property

  <Display(AutoGenerateField:=False),
  System.ComponentModel.Browsable(False)>
  Public Overloads ReadOnly Property BusinessRules As Csla.Rules.BusinessRules
    Get
      Return MyBase.BusinessRules
    End Get
  End Property

#End Region

#Region " Conditional Formatting "

  Public Overridable Function GetFieldStyle(ByVal FieldName As String) As FieldStyle Implements ISingularBusinessBase.GetFieldStyle

    Return Nothing

  End Function

#End Region

#If SILVERLIGHT Then
#Else
  Public Shared GuidProperty As Csla.PropertyInfo(Of Guid) = RegisterProperty(Of Guid)(Function(f) f.Guid)
  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False),
  System.ComponentModel.Browsable(False)>
  Public Overridable Property Guid As Guid Implements ISingularBusinessBase.Guid
    Get
      If Not FieldManager.FieldExists(GuidProperty) Then
        LoadProperty(GuidProperty, Guid.NewGuid)
      End If
      Return GetProperty(GuidProperty)
    End Get
    Set(ByVal value As Guid)
      LoadProperty(GuidProperty, value)
    End Set
  End Property

#End If

#Region " Singular Properties "

#If SILVERLIGHT Then
#Else

  ''' <summary>
  ''' Forces the type to be initialised, and have all of its csla property infos created.
  ''' </summary>
  Protected Shared Function InitialisationDummy() As Boolean
    Return True
  End Function

  Shared Sub New()
    Csla.Core.FieldManager.FieldDataManager.ForceStaticFieldInit(GetType(C))
  End Sub

  Public Shared Function RegisterSProperty(Of PropertyType, ObjectType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), ByVal DefaultValue As PropertyType) As SPropertyInfo(Of PropertyType, ObjectType)
    Dim spi = Singular.RegisterSProperty(Of PropertyType, ObjectType)(TargetMember, DefaultValue)
    Return RegisterProperty(GetType(ObjectType), spi)
  End Function

  Public Shared Function RegisterSProperty(Of PropertyType, ObjectType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As SPropertyInfo(Of PropertyType, ObjectType)
    Return RegisterSProperty(Of PropertyType, ObjectType)(TargetMember, Nothing)
  End Function

  Public Shared Function RegisterSProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)), ByVal DefaultValue As PropertyType) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType, C)(TargetMember, DefaultValue)
  End Function

  Public Shared Function RegisterSProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object))) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType, C)(TargetMember, Nothing)
  End Function

  Public Shared Function RegisterReadOnlyProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                                                                  ByVal GetExpression As System.Linq.Expressions.Expression(Of System.Func(Of C, Object))) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember).GetExpression(GetExpression)
  End Function

  Public Shared Function RegisterReadOnlyProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                                                                 ByVal JSGetExpression As String) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember).GetExpression(JSGetExpression)
  End Function

  Public Shared Function RegisterReadOnlyProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                                                                ByVal GetCallBackJS As Func(Of String)) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember).GetExpression(GetCallBackJS)
  End Function

  Protected Overloads Function GetProperty(Of PropertyType)(ByVal SProperty As Csla.PropertyInfo(Of PropertyType)) As PropertyType

    If TypeOf SProperty Is ISingularPropertyInfo AndAlso CType(SProperty, ISingularPropertyInfo).HasParsedGetExpression(Of C)() Then
      Return CType(SProperty, SPropertyInfo(Of PropertyType, C)).ParsedGetExpression.GetValue(Me)
    Else
      Return MyBase.GetProperty(SProperty)
    End If

  End Function

  Protected Overloads Function SetProperty(Of PropertyType)(ByVal SProperty As Csla.PropertyInfo(Of PropertyType), ByVal Value As PropertyType) As PropertyType

    MyBase.SetProperty(SProperty, Value)

    If TypeOf SProperty Is ISingularPropertyInfo Then
      'Check if this property has AutoSet properties.
      For Each se In CType(SProperty, SPropertyInfo(Of PropertyType, C)).SetExpressionList
        If se.SetExpressionParsed IsNot Nothing Then
          se.SetProperty(Of C)(Me)
        End If
      Next
    End If
  End Function

  Friend Sub SetBackingFieldValue(ByVal PropertyInfo As Csla.Core.IPropertyInfo, ByVal Value As Object, IsLoad As Boolean) Implements ISingularBase.SetBackingFieldValue
    If IsLoad Then
      LoadProperty(PropertyInfo, Value)
    Else
      SetProperty(PropertyInfo, Value)
    End If
  End Sub

  Friend Function GetBackingFieldValue(PropertyInfo As Csla.Core.IPropertyInfo) As Object Implements ISingularBase.GetBackingFieldValue
    Return GetProperty(PropertyInfo)
  End Function

  Friend Sub SetKey(ByVal Value As Object) Implements ISingularBusinessBase.SetKey

    Dim Key = Singular.ReflectionCached.GetCachedType(Me.GetType).KeyProperty
    If Key Is Nothing Then
      Throw New Exception("Key attribute is not set on primary key in " & Me.GetType.Name)
    End If
    SetProperty(Key.BackingField, Value)

  End Sub

#End If

#End Region

#Region " Dynamic Storage "

#If Silverlight = 0 Then

  Private mDynamicStorage As Singular.Dynamic.DynamicStorage(Of C)

  ''' <summary>
  ''' Allows dynamic declaration of properties / variables. e.g. Storage.SomeValue = "x"
  ''' </summary>
  <System.ComponentModel.Browsable(False)> _
  Public ReadOnly Property StorageDynamic As Object
    Get
      Return Storage
    End Get
  End Property

  <System.ComponentModel.Browsable(False)> _
  Public ReadOnly Property Storage As Singular.Dynamic.DynamicStorage(Of C)
    Get
      If mDynamicStorage Is Nothing Then
        mDynamicStorage = New Singular.Dynamic.DynamicStorage(Of C)
      End If
      Return mDynamicStorage
    End Get
  End Property

#End If

#End Region

  Public Function TrySave(ByVal ContainerListType As Type) As SaveHelper Implements IChildSavable.TrySave
    Me.CheckAllRules()
    If IsChild Then
      Return New SaveHelper().Save(Me, ContainerListType, IsValid, IsDirty)
    Else
      Return New SaveHelper().Save(Me, IsValid, IsDirty)
    End If

  End Function

  Public Function TrySave() As SaveHelper
    Me.CheckAllRules()
    If IsChild Then
      Throw New Exception("Cannot try save child objects directly, need to pass in a ContainerListType")
    Else
      Return New SaveHelper().Save(Me, IsValid, IsDirty)
    End If

  End Function

#If SILVERLIGHT Then
#Else

  Public Function GetDataset(IncludeChildren As Boolean) As DataSet
    Return Singular.CSLALib.GetDatasetFromBusinessBase(Me, Not IncludeChildren, Not IncludeChildren)
  End Function

  Friend Property LocalisationDataValues As Localisation.Data.DataValueList Implements ISingularBase.LocalisationDataValues

#End If

End Class

#End Region
