Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Namespace CorrespondanceTemplates

  Public Class CorrespondanceDataSource
    Inherits Singular.SingularReadOnlyBase(Of CorrespondanceDataSource)

    Public Shared DataSourceNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DataSourceName, "Data Source Name", "")
    ''' <Summary>
    ''' Gets the Security Group value
    ''' </Summary>
    <Display(Name:="Data Source Name", Description:="")> _
    Public ReadOnly Property DataSourceName() As String
      Get
        Return GetProperty(DataSourceNameProperty)
      End Get
    End Property

    Public Shared DataSourceProperty As PropertyInfo(Of Object) = RegisterProperty(Of Object)(Function(c) c.DataSource, "DataSource")

    Public ReadOnly Property DataSource() As Object
      Get
        Return GetProperty(DataSourceProperty)
      End Get
    End Property

#If SILVERLIGHT Then

    Public Shared FieldListProperty As PropertyInfo(Of List(Of Singular.Reflection.PropertyItem)) = RegisterProperty(Of List(Of Singular.Reflection.PropertyItem))(Function(c) c.FieldList, "FieldList")

    Public ReadOnly Property FieldList As List(Of Singular.Reflection.PropertyItem)
      Get
        Return GetProperty(FieldListProperty)
      End Get
    End Property

    Public Shared EmailAddressColumnProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.EmailAddressColumn, "EmailAddressColumn")

    Public ReadOnly Property EmailAddressColumn As String
      Get
        Return GetProperty(EmailAddressColumnProperty)
      End Get
    End Property

    Public Shared CellNoColumnProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.CellNoColumn, "CellNoColumn")

    Public ReadOnly Property CellNoColumn As String
      Get
        Return GetProperty(CellNoColumnProperty)
      End Get
    End Property

    Public Sub New(DataSourceName As String, DataSource As Object, EmailAddressColumn As String, CellNoColumn As String)
      LoadProperty(DataSourceNameProperty, DataSourceName)
      LoadProperty(DataSourceProperty, DataSource)
      LoadProperty(EmailAddressColumnProperty, EmailAddressColumn)
      LoadProperty(CellNoColumnProperty, CellNoColumn)

      If DataSource IsNot Nothing AndAlso DataSource.Count > 0 Then
        LoadProperty(FieldListProperty, Singular.Reflection.GetPropertiesList(DataSource(0).GetType, False, False))
      End If

      'MarkAsChild()

    End Sub

    Private mCellNoPI As System.Reflection.PropertyInfo

    Public Function GetCellNo(Obj As Object) As String
      If mCellNoPI Is Nothing Then
        mCellNoPI = Obj.GetType.GetProperty(CellNoColumn, System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.Public)
      End If
      Return mCellNoPI.GetValue(Obj, Nothing)
    End Function

    Private mEmailAddressPI As System.Reflection.PropertyInfo

    Public Function GetEmailAddress(Obj As Object) As String
      If mEmailAddressPI Is Nothing Then
        mEmailAddressPI = Obj.GetType.GetProperty(EmailAddressColumn, System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.Public)
      End If
      Return mEmailAddressPI.GetValue(Obj, Nothing)
    End Function

#End If



  End Class
End Namespace