' Generated 22 Dec 2014 13:51 - Singular Systems Object Generator Version 2.1.661
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
  Public Class ReportParameterList
    Inherits SingularBusinessListBase(Of ReportParameterList, ReportParameter)

#Region "  Parent  "

    <NotUndoable()> Private mParent As Report
#End Region

#Region "  Business Methods  "

    Public Function GetItem(DynamicReportParameterID As Integer) As ReportParameter

      For Each child As ReportParameter In Me
        If child.DynamicReportParameterID = DynamicReportParameterID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Dynamic Report Parameters"

    End Function

#End Region

#Region "  Data Access  "

#Region "  Common  "

    Public Shared Function NewReportParameterList() As ReportParameterList

      Return New ReportParameterList()

    End Function

    Public Sub New()

      ' must have parameter-less constructor

    End Sub

#End Region

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As ReportParameter In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As ReportParameter In Me
          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace