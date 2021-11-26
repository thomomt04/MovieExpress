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
  Public Class ROLocalisationKey
    Inherits SingularReadOnlyBase(Of ROLocalisationKey)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared LocalisationKeyIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.LocalisationKeyID, "Localisation Key", 0)
    ''' <summary>
    ''' Gets the Localisation Key value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property LocalisationKeyID() As Integer
      Get
        Return GetProperty(LocalisationKeyIDProperty)
      End Get
    End Property

    Public Shared KeyProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Key, "Key", "")
    ''' <summary>
    ''' Gets the Key value
    ''' </summary>
    <Display(Name:="Key", Description:="The key used to reference this word / phrase.")>
    Public ReadOnly Property Key() As String
      Get
        Return GetProperty(KeyProperty)
      End Get
    End Property

    Public Shared DefaultValueProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DefaultValue, "Default Value", "")
    ''' <summary>
    ''' Gets the Default Value value
    ''' </summary>
    <Display(Name:="Default Value", Description:="The Word / Phrase in the default language of the system.")>
    Public ReadOnly Property DefaultValue() As String
      Get
        Return GetProperty(DefaultValueProperty)
      End Get
    End Property

#End Region

#Region "  Child Lists  "

    Public Shared ROLocalisationValueListProperty As PropertyInfo(Of ROLocalisationValueList) = RegisterProperty(Of ROLocalisationValueList)(Function(c) c.ROLocalisationValueList, "RO Localisation Value List")

    <Display(AutoGenerateField:=False)> _
    Public ReadOnly Property ROLocalisationValueList() As ROLocalisationValueList
      Get
        If GetProperty(ROLocalisationValueListProperty) Is Nothing Then
          LoadProperty(ROLocalisationValueListProperty, ROLocalisationValueList.NewROLocalisationValueList())
        End If
        Return GetProperty(ROLocalisationValueListProperty)
      End Get
    End Property

#End Region

#Region "  Methods  "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(LocalisationKeyIDProperty)

    End Function

    Public Overrides Function ToString() As String

      Return Me.DefaultValue

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

    Friend Shared Function GetROLocalisationKey(dr As SafeDataReader) As ROLocalisationKey

      Dim r As New ROLocalisationKey()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      With sdr
        LoadProperty(LocalisationKeyIDProperty, .GetInt32(0))
        LoadProperty(KeyProperty, .GetString(1).ToLower)
        LoadProperty(DefaultValueProperty, .GetString(2))
      End With

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace