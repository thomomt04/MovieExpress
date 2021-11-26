' Generated 23 Dec 2014 09:05 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace Reporting.Dynamic

  <Serializable()> _
  Public Class ROReportSource
    Inherits SingularReadOnlyBase(Of ROReportSource)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared SourceTypeProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SourceType, "SourceType")
    ''' <summary>
    ''' Gets the SourceType value
    ''' </summary>
    <Display(Name:="SourceType", Description:="")>
    Public ReadOnly Property SourceType() As String
      Get
        Return GetProperty(SourceTypeProperty)
      End Get
    End Property

    Public Shared SourceNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SourceName, "ID")
    ''' <summary>
    ''' Gets the ID value
    ''' </summary>
    <Key>
    Public ReadOnly Property SourceName() As String
      Get
        Return GetProperty(SourceNameProperty)
      End Get
    End Property

#End Region

#Region "  Methods  "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SourceNameProperty)

    End Function

    Public Overrides Function ToString() As String

      Return Me.SourceName

    End Function

#End Region

#End Region

#Region "  Data Access & Factory Methods  "

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Friend Shared Function GetROReportSource(dr As SafeDataReader) As ROReportSource

      Dim r As New ROReportSource()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      With sdr
        LoadProperty(SourceTypeProperty, .GetString(0))
        LoadProperty(SourceNameProperty, .GetString(1))
      End With

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace