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
  Public Class ROProc
    Inherits SingularReadOnlyBase(Of ROProc)

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

#End Region

#Region " Child Lists "

    Public Shared ROProcParameterListProperty As PropertyInfo(Of ROProcParameterList) = RegisterProperty(Of ROProcParameterList)(Function(c) c.ROProcParameterList, "RO Reporting Proc Parameter List")

    Public ReadOnly Property ROProcParameterList() As ROProcParameterList
      Get
        If Not FieldManager.FieldExists(ROProcParameterListProperty) Then
          LoadProperty(ROProcParameterListProperty, ROProcParameterList.NewROProcParameterList())
        End If
        Return GetProperty(ROProcParameterListProperty)
      End Get
    End Property

#End Region

#Region " Methods "

    Public Function GetFullProcedureName() As String

      Return "[" & Me.Schema & "]" & "." & "[" & Me.ProcName & "]"

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SchemaProperty)

    End Function

    Public Overrides Function ToString() As String

      Return GetProperty(SchemaProperty)

    End Function

#End Region

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Sub New()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewROProc() As ROProc

      Return New ROProc()

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetROProc(ByVal dr As SafeDataReader) As ROProc

      Dim r As New ROProc()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      With sdr
        LoadProperty(SchemaProperty, .GetString(0))
        LoadProperty(ProcNameProperty, .GetString(1))
      End With

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace