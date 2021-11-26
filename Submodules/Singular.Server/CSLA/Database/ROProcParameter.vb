Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CSLALib.Database

  <Serializable()> _
  Public Class ROProcParameter
    Inherits SingularReadOnlyBase(Of ROProcParameter)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared SchemaProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Schema, "ID")
    ''' <Summary>
    ''' Gets the ID value
    ''' </Summary>
    <Display(Name:="ID", Description:="")> _
    Public ReadOnly Property Schema() As String
      Get
        Return GetProperty(SchemaProperty)
      End Get
    End Property

    Public Shared ProcNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ProcName, "Proc Name")
    ''' <Summary>
    ''' Gets the Proc Name value
    ''' </Summary>
    <Display(Name:="Proc Name", Description:="")> _
    Public ReadOnly Property ProcName() As String
      Get
        Return GetProperty(ProcNameProperty)
      End Get
    End Property

    Public Shared ParameterNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ParameterName, "Parameter Name")
    ''' <Summary>
    ''' Gets the Parameter Name value
    ''' </Summary>
    <Display(Name:="Parameter Name", Description:="")> _
    Public ReadOnly Property ParameterName() As String
      Get
        Return GetProperty(ParameterNameProperty)
      End Get
    End Property

    Public Shared SqlDataTypeProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SqlDataType, "Data Type")
    ''' <Summary>
    ''' Gets the Data Type value
    ''' </Summary>
    <Display(Name:="Data Type", Description:="")> _
    Public ReadOnly Property SqlDataType() As String
      Get
        Return GetProperty(SqlDataTypeProperty)
      End Get
    End Property

    Public Shared DotNetDataTypeProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DotNetDataType, "Data Type")
    ''' <Summary>
    ''' Gets the Data Type value
    ''' </Summary>
    <Display(Name:="Data Type", Description:="")> _
    Public ReadOnly Property DotNetDataType() As String
      Get
        Return GetProperty(DotNetDataTypeProperty)
      End Get
    End Property

    Public Shared DefaultIndProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.DefaultInd, "Data Type")
    ''' <Summary>
    ''' Gets the Data Type value
    ''' </Summary>
    <Display(Name:="Data Type", Description:="")> _
    Public ReadOnly Property DefaultInd() As Boolean
      Get
        Return GetProperty(DefaultIndProperty)
      End Get
    End Property

    Public Shared DefaultValueProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DefaultValue, "Data Type")
    ''' <Summary>
    ''' Gets the Data Type value
    ''' </Summary>
    <Display(Name:="Data Type", Description:="")> _
    Public ReadOnly Property DefaultValue() As String
      Get
        Return GetProperty(DefaultValueProperty)
      End Get
    End Property

#End Region

#Region " Methods "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SchemaProperty)

    End Function

    Public Overrides Function ToString() As String

      Return GetProperty(SchemaProperty)

    End Function

#End Region

#End Region

#Region " Data Access & Factory Methods "

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewROProcParameter() As ROProcParameter

      Return New ROProcParameter()

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetROProcParameter(ByVal dr As SafeDataReader) As ROProcParameter

      Dim r As New ROProcParameter()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      If sdr.FieldCount = 6 Then
        With sdr
          LoadProperty(SchemaProperty, .GetString(0))
          LoadProperty(ProcNameProperty, .GetString(1))
          LoadProperty(ParameterNameProperty, .GetString(2))
          LoadProperty(SqlDataTypeProperty, .GetString(3))
          LoadProperty(DefaultIndProperty, .GetBoolean(4))
          LoadProperty(DefaultValueProperty, .GetString(5))
        End With

      ElseIf sdr.FieldCount = 5 Then

        Dim FullProcName As String = sdr.GetString(0)
        LoadProperty(SchemaProperty, FullProcName.Substring(0, FullProcName.IndexOf(".") - 1))
        LoadProperty(ProcNameProperty, FullProcName.Replace(Me.Schema + ".", ""))

        LoadProperty(ParameterNameProperty, sdr.GetString(1))
        LoadProperty(SqlDataTypeProperty, sdr.GetString(2))
        LoadProperty(DefaultIndProperty, sdr.GetBoolean(3))
        LoadProperty(DefaultValueProperty, sdr.GetString(4))
      End If

      LoadProperty(DotNetDataTypeProperty, Singular.Data.Sql.GetDotNetTypeFromSqlType(Me.SqlDataType))

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace