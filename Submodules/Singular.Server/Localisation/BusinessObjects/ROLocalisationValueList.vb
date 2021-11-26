' Generated 01 Mar 2013 10:33 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace Localisation.Objects

  <Serializable()> _
  Public Class ROLocalisationValueList
    Inherits SingularReadOnlyListBase(Of ROLocalisationValueList, ROLocalisationValue)

#Region "  Parent  "

    <NotUndoable()> Private mParent As ROLocalisationKey
#End Region

#Region "  Business Methods  "

    Public Function GetItem(LocalisationValueID As Integer) As ROLocalisationValue

      For Each child As ROLocalisationValue In Me
        If child.LocalisationValueID = LocalisationValueID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Localisation Values"

    End Function

#End Region

#Region "  Data Access  "

#Region "  Common  "

    Public Shared Function NewROLocalisationValueList() As ROLocalisationValueList

      Return New ROLocalisationValueList()

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

#End If

#End Region

#End Region

  End Class

End Namespace