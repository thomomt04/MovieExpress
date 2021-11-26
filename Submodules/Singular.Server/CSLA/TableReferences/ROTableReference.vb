Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CSLALib

  <Serializable()> _
  Public Class TableReference
    Inherits SingularReadOnlyBase(Of TableReference)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared TableNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.TableName, "TableName")
    ''' <Summary>
    ''' Gets the TableName value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="TableName", Description:="")> _
    Public ReadOnly Property TableName() As String
      Get
        Return GetProperty(TableNameProperty)
      End Get
    End Property

    Public Shared ColumnNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ColumnName, "Column Name")
    ''' <Summary>
    ''' Gets the Column Name value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Column Name", Description:="")> _
    Public ReadOnly Property ColumnName() As String
      Get
        Return GetProperty(ColumnNameProperty)
      End Get
    End Property

    Public Shared ConstraintNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ConstraintName, "Constraint Name")
    ''' <Summary>
    ''' Gets the Constraint Name value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Constraint Name", Description:="")> _
    Public ReadOnly Property ConstraintName() As String
      Get
        Return GetProperty(ConstraintNameProperty)
      End Get
    End Property

    Public Shared ConstraintDescriptionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ConstraintDescription, "Constraint Description")
    ''' <Summary>
    ''' Gets the Constraint Description value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Constraint Description", Description:="")> _
    Public ReadOnly Property ConstraintDescription() As String
      Get
        Return GetProperty(ConstraintDescriptionProperty)
      End Get
    End Property

    Public Shared NoOfReferencesProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.NoOfReferences, "No Of References")
    ''' <Summary>
    ''' Gets the No Of References value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="No Of References", Description:="")> _
    Public ReadOnly Property NoOfReferences() As Integer
      Get
        Return GetProperty(NoOfReferencesProperty)
      End Get
    End Property

#End Region

#Region " Methods "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(TableNameProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.ConstraintDescription.Trim <> "" Then
        Return Me.ConstraintDescription
      Else
        Return Singular.Strings.Readable(Me.TableName) & ": " & Singular.Strings.Pluralize(Me.NoOfReferences, "Record") & " referenced"
      End If

    End Function

#End Region

#End Region

#Region " Data Access & Factory Methods "

#Region " Silverlight "

#If SILVERLIGHT Then

Public Shared Function NewTableReference() As TableReference

Return New TableReference()

End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetTableReference(ByVal dr As SafeDataReader) As TableReference

      Dim r As New TableReference()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      With sdr
        LoadProperty(TableNameProperty, .GetString(0))
        LoadProperty(ColumnNameProperty, .GetString(1))
        LoadProperty(ConstraintNameProperty, .GetString(2))
        LoadProperty(ConstraintDescriptionProperty, .GetString(3))
        LoadProperty(NoOfReferencesProperty, .GetInt32(4))
      End With

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace