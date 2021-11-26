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
  Public Class ROLocalisationValue
    Inherits SingularReadOnlyBase(Of ROLocalisationValue)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared LocalisationValueIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.LocalisationValueID, "Localisation Value", 0)
    ''' <summary>
    ''' Gets the Localisation Value value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property LocalisationValueID() As Integer
      Get
        Return GetProperty(LocalisationValueIDProperty)
      End Get
    End Property

    Public Shared LocalisationKeyIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.LocalisationKeyID, "Localisation Key", Nothing)
    ''' <summary>
    ''' Gets the Localisation Key value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property LocalisationKeyID() As Integer?
      Get
        Return GetProperty(LocalisationKeyIDProperty)
      End Get
    End Property

    Public Shared LanguageIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.LanguageID, "Language", Nothing)
    ''' <summary>
    ''' Gets the Language value
    ''' </summary>
    <Display(Name:="Language", Description:="")>
    Public ReadOnly Property LanguageID() As Integer?
      Get
        Return GetProperty(LanguageIDProperty)
      End Get
    End Property

    Public Shared ValueProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Value, "Value", "")
    ''' <summary>
    ''' Gets the Value value
    ''' </summary>
    <Display(Name:="Value", Description:="")>
    Public ReadOnly Property Value() As String
      Get
        Return GetProperty(ValueProperty)
      End Get
    End Property

#End Region

#Region "  Methods  "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(LocalisationValueIDProperty)

    End Function

    Public Overrides Function ToString() As String

      Return Me.Value

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

    Friend Shared Function GetROLocalisationValue(dr As SafeDataReader) As ROLocalisationValue

      Dim r As New ROLocalisationValue()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      With sdr
        LoadProperty(LocalisationValueIDProperty, .GetInt32(0))
        LoadProperty(LocalisationKeyIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
        LoadProperty(LanguageIDProperty, Singular.Misc.ZeroNothing(.GetInt32(2)))
        LoadProperty(ValueProperty, .GetString(3))
      End With

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace