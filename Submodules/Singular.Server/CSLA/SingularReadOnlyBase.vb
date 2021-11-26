Imports Csla
Imports Csla.Serialization
Imports Csla.Core

<Serializable()>
Public Class SingularReadOnlyBase(Of C As SingularReadOnlyBase(Of C))
  Inherits Csla.ReadOnlyBase(Of C)
  Implements ISingularReadOnlyBase

  Protected Function GetSmartDateProperty(ByVal PropertyInfo As PropertyInfo(Of SmartDate)) As Nullable(Of DateTime)

    Dim value = GetProperty(PropertyInfo)
    If value.IsEmpty Then
      Return Nothing
    Else
      Return value.Date
    End If

  End Function

  Public Function GetId() As Object
    Return Me.GetIdValue()
  End Function

  Protected Overrides Function GetIdValue() As Object Implements ISingularBase.GetIdValue

    Dim pi = GetIDProperty()
    If pi Is Nothing Then
      Return MyBase.GetIdValue
    Else
      Return GetProperty(pi)
    End If

  End Function

  Public Overrides Function Equals(obj As Object) As Boolean

    Dim ib = TryCast(obj, ISingularBase)
    If ib IsNot Nothing AndAlso ib.GetIdValue IsNot Nothing Then
      If ib.GetIdValue.Equals(0) Then
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

  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
  Public Overloads ReadOnly Property FieldManager As Csla.Core.FieldManager.FieldDataManager Implements ISingularBase.FieldManager
    Get
      Return MyBase.FieldManager
    End Get
  End Property

#If Silverlight = False Then
  Friend Function GetBackingFieldValue(PropertyInfo As Csla.Core.IPropertyInfo) As Object Implements ISingularBase.GetBackingFieldValue
    Return GetProperty(PropertyInfo)
  End Function
#End If

  Public Overridable Function GetTableName() As String Implements ISingularBase.GetTableName
    Return CSLALib.GetTableName(Me.GetType.Name)
  End Function

#If Silverlight = False Then

  Public Shared GuidProperty As Csla.PropertyInfo(Of Guid) = RegisterProperty(Of Guid)(Function(f) f.Guid)
  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False),
  System.ComponentModel.Browsable(False)>
  Public Overridable Property Guid As Guid Implements ISingularReadOnlyBase.Guid
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

#Region " Conditional Formatting "

  Public Overridable Function GetFieldStyle(FieldName As String) As FieldStyle Implements ISingularBase.GetFieldStyle

    Return Nothing

  End Function

#End Region

#Region " Singular Properties "

#If Silverlight = False Then

  ''' <summary>
  ''' Forces the type to be initialised, and have all of its csla property infos created.
  ''' </summary>
  Protected Shared Function InitialisationDummy() As Boolean
    Return True
  End Function

  Shared Sub New()
    Csla.Core.FieldManager.FieldDataManager.ForceStaticFieldInit(GetType(C))
  End Sub

  Public Shared Function RegisterSProperty(Of PropertyType, ObjectType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)), DefaultValue As PropertyType) As SPropertyInfo(Of PropertyType, ObjectType)
    Dim spi = Singular.RegisterSProperty(Of PropertyType, ObjectType)(TargetMember, DefaultValue)

    Return RegisterProperty(GetType(ObjectType), spi)

  End Function

  Public Shared Function RegisterSProperty(Of PropertyType, ObjectType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))) As SPropertyInfo(Of PropertyType, ObjectType)

    Return RegisterSProperty(Of PropertyType, ObjectType)(TargetMember, Nothing)

  End Function

  Public Shared Function RegisterSProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)), DefaultValue As PropertyType) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType, C)(TargetMember, DefaultValue)
  End Function

  Public Shared Function RegisterSProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object))) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember, Nothing)
  End Function

  Public Shared Function RegisterReadOnlyProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                                                                  GetExpression As System.Linq.Expressions.Expression(Of System.Func(Of C, Object))) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember).GetExpression(GetExpression)

  End Function

  Public Shared Function RegisterReadOnlyProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                                                               ByVal JSGetExpression As String) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember).GetExpression(JSGetExpression)
  End Function

  Protected Overloads Function GetProperty(Of PropertyType)(SProperty As Csla.PropertyInfo(Of PropertyType)) As PropertyType
    If TypeOf SProperty Is ISingularPropertyInfo AndAlso CType(SProperty, ISingularPropertyInfo).HasParsedGetExpression(Of C)() Then
      Return CType(SProperty, SPropertyInfo(Of PropertyType, C)).ParsedGetExpression.GetValue(Me)
    Else
      Return MyBase.GetProperty(SProperty)
    End If
  End Function

  Friend Sub SetBackingFieldValue(PropertyInfo As Csla.Core.IPropertyInfo, Value As Object, IsLoad As Boolean) Implements ISingularBase.SetBackingFieldValue
    LoadProperty(PropertyInfo, Value)
  End Sub

#End If

#End Region

#If SILVERLIGHT Then
#Else

  Public Function GetDataset(IncludeChildren As Boolean) As DataSet
    Return Singular.CSLALib.GetDatasetFromBusinessBase(Me, Not IncludeChildren, Not IncludeChildren)
  End Function

  Protected Overrides Sub DataPortal_OnDataPortalInvokeComplete(e As DataPortalEventArgs)
    MyBase.DataPortal_OnDataPortalInvokeComplete(e)

    If e.Operation = DataPortalOperations.Fetch Then Localisation.Data.ObjectVisitor.FetchData(Me)
  End Sub

  Friend Property LocalisationDataValues As Localisation.Data.DataValueList Implements ISingularBase.LocalisationDataValues

#End If

End Class
