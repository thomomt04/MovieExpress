' Generated 09 Dec 2014 14:35 - Singular Systems Object Generator Version 2.1.661
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
  Public Class ROParameterList
    Inherits SingularReadOnlyListBase(Of ROParameterList, ROParameter)

#Region "  Business Methods  "

    Public Function Find(ParamName As String) As ROParameter
      For Each param As ROParameter In Me
        If param.ParameterName = ParamName Then
          Return param
        End If
      Next
      Return Nothing
    End Function

    Public Overrides Function ToString() As String

      Return "s"

    End Function

#End Region

#Region "  Data Access  "

#Region "  Common  "

    Public Shared Function NewROParameterList() As ROParameterList

      Return New ROParameterList()

    End Function

    Public Sub New()

      IsReadOnly = False
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

#End If

#End Region

#End Region

  End Class

End Namespace