' Generated 01 Mar 2013 10:33 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Collections.Generic
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace Localisation.Objects

  Public Class KeyData

    Public Property DefaultValue As String

    Public Property LanguageList As New Dictionary(Of String, String)

  End Class

  <Serializable()>
  Public Class LocalisationKeys
    Inherits Singular.SingularReadOnlyBase(Of LocalisationKeys)

#Region "  Business Methods  "

    Private _LanguageList As New Dictionary(Of Integer, Language)
    Private _LanguageIDList As New Dictionary(Of String, Integer)

    Public ReadOnly Property SupportedLanguages As Dictionary(Of Integer, Language)
      Get
        Return _LanguageList
      End Get
    End Property

    Private _KeyDictionary As New Dictionary(Of String, KeyData)
    Private _DefaultLanguageCode As String = Nothing

    Public Shared Function GetCompositeKey(LanguageCode As String, VariantID As Integer) As String
      Return LanguageCode & "_" & VariantID
    End Function

    Public Function GetValue(Language As String, VariantID As Integer?, Key As String, ReturnKeyIfMissing As Boolean) As String

      Dim KeyData As KeyData = Nothing

      If _KeyDictionary.TryGetValue(Key.ToLower, KeyData) Then

        VariantID = If(VariantID, DefaultVariantID)

        If Language <> _DefaultLanguageCode OrElse VariantID <> DefaultVariantID Then

          Dim Value As String = Nothing

          'Get language variant
          If KeyData.LanguageList.TryGetValue(GetCompositeKey(Language, VariantID), Value) Then Return Value

          If Language <> _DefaultLanguageCode Then

            If LanguageVariantPriority = LanguageVariant.Variant Then
              'English variant is more important than language if there is no variant for the current language.

              If KeyData.LanguageList.TryGetValue(GetCompositeKey(_DefaultLanguageCode, VariantID), Value) Then Return Value
              If KeyData.LanguageList.TryGetValue(GetCompositeKey(Language, DefaultVariantID), Value) Then Return Value

            Else
              'Language default is more important than variant if there is no variant for the current language.

              If KeyData.LanguageList.TryGetValue(GetCompositeKey(Language, DefaultVariantID), Value) Then Return Value
              If KeyData.LanguageList.TryGetValue(GetCompositeKey(_DefaultLanguageCode, VariantID), Value) Then Return Value

            End If

          End If
        End If

        'English default
        Return KeyData.DefaultValue

      Else
        'Key doesnt exist.
        Return If(ReturnKeyIfMissing, Key, Nothing)
      End If

    End Function

    Public Function GetLanguageID(CultureCode As String) As Integer
      Return _LanguageIDList(CultureCode)
    End Function

    Public Overrides Function ToString() As String

      Return "Localisation Keys"

    End Function

#End Region

#Region "  Data Access  "

    <Serializable()>
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Property ConnectionString As String

      Public Sub New()

      End Sub

    End Class

#Region "  Common  "

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

    Public Shared Function GetLocalisationKeys(Optional ConnectionString As String = Nothing) As LocalisationKeys

      Return DataPortal.Fetch(Of LocalisationKeys)(New Criteria() With {.ConnectionString = ConnectionString})

    End Function

    Private Sub Fetch(sdr As SafeDataReader)

      'Ket index used for lookup later in this method.
      Dim KeyIndex As New Dictionary(Of Integer, KeyData)

      While sdr.Read
        Dim Key = sdr.GetString(1).ToLower
        Dim KeyData As New KeyData With {.DefaultValue = sdr.GetString(2)}

        _KeyDictionary.Add(Key, KeyData)
        KeyIndex.Add(sdr.GetInt32(0), KeyData)
      End While

      'Get Languages
      If sdr.NextResult() Then
        While sdr.Read
          Dim Language As New Language() With {.CultureCode = sdr.GetString(1), .Language = sdr.GetString(2)}
          _LanguageList.Add(sdr.GetInt32(0), Language)
          _LanguageIDList.Add(Language.CultureCode, sdr.GetInt32(0))

          If _DefaultLanguageCode Is Nothing Then _DefaultLanguageCode = Language.CultureCode
        End While
      End If


      'Create the localised versions
      If sdr.NextResult() Then

        Dim LastLanguageID As Integer = -1
        Dim CurrentLanguageCode As String = Nothing

        While sdr.Read

          Dim VariantID As Integer = DefaultVariantID
          If LocalisationVariantsEnabled Then VariantID = If(sdr.GetValue(4), DefaultVariantID)

          If sdr.GetInt32(2) <> LastLanguageID Then
            LastLanguageID = sdr.GetInt32(2)
            CurrentLanguageCode = _LanguageList(LastLanguageID).CultureCode
          End If

          Dim LanguageKey = GetCompositeKey(CurrentLanguageCode, VariantID)

          KeyIndex(sdr.GetInt32(1)).LanguageList.Add(LanguageKey, sdr.GetString(3))

        End While
      End If

    End Sub

    Protected Sub DataPortal_Fetch(criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(If(String.IsNullOrEmpty(crit.ConnectionString), Settings.ConnectionString, crit.ConnectionString))
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getROLocalisationKeyList"
            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace