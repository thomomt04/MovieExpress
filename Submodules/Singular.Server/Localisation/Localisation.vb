Imports System.Resources
Imports System.Reflection
Imports System.Globalization

Namespace Localisation

  Public Module Localisation

    Public Const DefaultVariantID = 0
    Friend Const VariantDictionaryKey = "Language_Variant_ID"

#Region " Settings / Setup "

    Public Class Language
      Public Property CultureCode As String
      Public Property Language As String
    End Class

    Public Enum SourceType
      ''' <summary>
      ''' Project doesnt use localisation
      ''' </summary>
      None
      ''' <summary>
      ''' Finds Localised Strings in project resource files defined by calling RegisterLocalisedResource
      ''' </summary>
      ProjectResourceFile
      ''' <summary>
      ''' Finds Localised Strings in the database.
      ''' </summary>
      Database
    End Enum

    Public Enum LanguageVariant
      [Variant]
      Language
    End Enum

    ''' <summary>
    ''' Setup Localisation for this project.
    ''' </summary>
    ''' <param name="Source">Location of the Localised Strings</param>
    ''' <param name="CurrentCultureCallback">Custom function that returns the culture for the current user.</param>
    ''' <param name="CurrentVariantCallback">Custom function that returns the language variant ID for the current user.</param>
    Public Sub Setup(Source As SourceType,
                     CurrentCultureCallback As Func(Of CultureInfo),
                     Optional CurrentVariantCallback As Func(Of Integer?) = Nothing,
                     Optional LanguageVariantPriority As LanguageVariant = LanguageVariant.Variant)

      _LocalisationStringsSource = Source
      _LanguageVariantPriority = LanguageVariantPriority
      _CurrentCultureCallback = CurrentCultureCallback
      _CurrentVariantCallback = CurrentVariantCallback

      If Source <> SourceType.None Then
        LocalisationEnabled = True
      End If
      If _CurrentVariantCallback IsNot Nothing Then
        LocalisationVariantsEnabled = True
      End If
    End Sub

    Public Property LocalisationEnabled As Boolean = False

    ''' <summary>
    ''' True if your database splits localisation values by language, and variant.
    ''' </summary>
    ''' <returns></returns>
    Public Property LocalisationVariantsEnabled As Boolean = False

    Private _LocalisationStringsSource As SourceType = SourceType.ProjectResourceFile
    ''' <summary>
    ''' Location of the Localised Strings
    ''' </summary>
    Public ReadOnly Property LocalisationStringsSource As SourceType
      Get
        Return _LocalisationStringsSource
      End Get
    End Property

    Private _LanguageVariantPriority As LanguageVariant = LanguageVariant.Variant

    Public ReadOnly Property LanguageVariantPriority As LanguageVariant
      Get
        Return _LanguageVariantPriority
      End Get
    End Property

    Private _CurrentCultureCallback As Func(Of CultureInfo)
    Private _CurrentVariantCallback As Func(Of Integer?)

    ''' <summary>
    ''' Returns the culture for the current user. 
    ''' </summary>
    Public ReadOnly Property CurrentCulture As CultureInfo
      Get
        Return System.Threading.Thread.CurrentThread.CurrentCulture
      End Get
    End Property

    Public ReadOnly Property CurrentVariant As Integer?
      Get
        If System.Web.HttpContext.Current Is Nothing Then
          Return Nothing
        Else
          Return System.Web.HttpContext.Current.Items(VariantDictionaryKey)
        End If
      End Get
    End Property

    ''' <summary>
    ''' Invokes the current culture delegate in case the application overrides the default localisation.
    ''' </summary>
    Public Sub SetupRequest()
      If _CurrentCultureCallback IsNot Nothing Then
        System.Threading.Thread.CurrentThread.CurrentCulture = _CurrentCultureCallback.Invoke
      End If
      System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture

      If _CurrentVariantCallback IsNot Nothing Then
        Dim VariantID = _CurrentVariantCallback.Invoke
        System.Web.HttpContext.Current.Items(VariantDictionaryKey) = If(VariantID, DefaultVariantID)
      End If

    End Sub

    ''' <summary>
    ''' Returns the database language id for the language of the current culture.
    ''' If localisation source is not Database, then returns 1.
    ''' </summary>
    Public ReadOnly Property CurrentLanguageID As Integer
      Get
        If LocalisationStringsSource = SourceType.Database Then
          If CurrentCulture.TwoLetterISOLanguageName = "iv" Then
            Return ROLocalisationKeyList.GetLanguageID("en")
          Else
            Return ROLocalisationKeyList.GetLanguageID(CurrentCulture.TwoLetterISOLanguageName)
          End If
        Else
          Return 1
        End If
      End Get
    End Property

#End Region

#Region " RESX Implementation "

    Private mLocalisedResourceList As New List(Of ResourceManager)

    ''' <summary>
    ''' Tells the localisation helper where your resx file with the localised strings is.
    ''' </summary>
    ''' <param name="AssemblyName">e.g. "XXLib" or "XXWin"</param>
    ''' <param name="ResourceName">The name of the resx file (without the .resx extension)</param>
    ''' <remarks></remarks>
    Public Sub RegisterLocalisedResource(AssemblyName As String, Optional ResourceName As String = "localstring")

      Dim Ass = Assembly.Load(AssemblyName)

      Dim resourceManager As New Resources.ResourceManager(AssemblyName & "." & ResourceName, Ass)
      mLocalisedResourceList.Add(resourceManager)

    End Sub

    Private Function GetTextFromResourcesFile(ResourceName As String, Culture As System.Globalization.CultureInfo, Optional ReturnKeyIfMissing As Boolean = False) As String

      Dim ResourceValue As String = ""
      'Look in all the registered resource files for the Key.
      For Each rm As ResourceManager In mLocalisedResourceList

        ResourceValue = rm.GetString(ResourceName, Culture)
        If ResourceValue IsNot Nothing Then
          Return ResourceValue
        End If

      Next

      'Look in the singular library resource file if nothing was found in the registered resources.
      Dim LValue = My.Resources.ResourceManager.GetString(ResourceName, Culture)

      Return If(LValue Is Nothing AndAlso ReturnKeyIfMissing, ResourceName, LValue)

    End Function

#End Region

#Region " Database Implementation "

    Public Property ConnectionString As String = Nothing

    Private _LockObject As New Object

    Private _LocalisationKeys As Objects.LocalisationKeys
    Private ReadOnly Property ROLocalisationKeyList As Objects.LocalisationKeys
      Get
        If _LocalisationKeys Is Nothing Then
          SyncLock _LockObject
            If _LocalisationKeys Is Nothing Then
              _LocalisationKeys = Objects.LocalisationKeys.GetLocalisationKeys(ConnectionString)
            End If
          End SyncLock
        End If
        Return _LocalisationKeys
      End Get
    End Property

    Public Sub Refresh()
      SyncLock _LockObject
        _LocalisationKeys = Nothing
      End SyncLock
    End Sub

    Private Function GetTextFromDatabaseResources(ResourceKey As String, Culture As System.Globalization.CultureInfo,
                                                  Optional VariantID As Integer? = Nothing, Optional ReturnKeyIfMissing As Boolean = True) As String

      Return ROLocalisationKeyList.GetValue(Culture.TwoLetterISOLanguageName, VariantID, ResourceKey, ReturnKeyIfMissing)

    End Function

    ''' <summary>
    ''' Returns the list of languages from the database.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>from [GetProcs].[getROLocalisationKeyList]</remarks>
    Public ReadOnly Property SupportedLanguages As Dictionary(Of Integer, Language)
      Get
        Return ROLocalisationKeyList.SupportedLanguages
      End Get
    End Property

#End Region

    ''' <summary>
    ''' Create a CultureInfo object form the supplied CultureCode. The number and date formats are left as default.
    ''' </summary>
    Public Function CreateLanguageCulture(CultureCode As String) As CultureInfo
      Dim ci As New CultureInfo(CultureCode)
      ci.NumberFormat = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat
      'ci.DateTimeFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat
      Return ci
    End Function

    ''' <summary>
    ''' Gets the localised text for the given key. Returns the text in the default language if the localised version is missing.
    ''' </summary>
    Public Function LocalText(ResourceName As String) As String
      Return LocalTextForCulture(ResourceName, CurrentVariant, CurrentCulture, True)
    End Function


    ''' <summary>
    ''' Gets the localised text for the given key. If they key is missing, returns null.
    ''' </summary>
    Public Function LocalTextDefaultOverride(ResourceName As String, OverridingDefaultValue As String) As String
      Dim localText = LocalTextForCulture(ResourceName, CurrentVariant, CurrentCulture, False)
      If localText Is Nothing Then
        Return OverridingDefaultValue
      Else
        Return localText.ToString()
      End If
    End Function

    ''' <summary>
    ''' Gets the localised text for the given key. If they key is missing, returns null.
    ''' </summary>
    Public Function LocalText_DontReplaceMissing(ResourceName As String) As String
      Return LocalTextForCulture(ResourceName, CurrentVariant, CurrentCulture, False)
    End Function

    ''' <summary>
    ''' JS encoded local text
    ''' </summary>
    Public Function LocalTextJS(ResourceName As String) As String
      Return LocalTextForCultureJS(ResourceName, CurrentVariant, CurrentCulture)
    End Function

    Public Function LocalTextForCulture(ResourceName As String, VariantID As Integer?, Culture As System.Globalization.CultureInfo, ReturnKeyIfMissing As Boolean)
      If LocalisationStringsSource = SourceType.ProjectResourceFile Then
        Return GetTextFromResourcesFile(ResourceName, Culture, ReturnKeyIfMissing)
      ElseIf LocalisationStringsSource = SourceType.Database Then
        Dim Value = GetTextFromDatabaseResources(ResourceName, Culture, VariantID, ReturnKeyIfMissing)
        If Value Is Nothing Then
          Return GetTextFromResourcesFile(ResourceName, Culture, ReturnKeyIfMissing)
        Else
          Return Value
        End If
      Else
        If ReturnKeyIfMissing Then
          Return ResourceName
        Else
          Return Nothing
        End If
      End If
    End Function

    Public Function LocalTextForCultureJS(ResourceName As String, VariantID As Integer?, Culture As System.Globalization.CultureInfo)
      Return System.Web.HttpUtility.JavaScriptStringEncode(LocalTextForCulture(ResourceName, VariantID, CurrentCulture, True))
    End Function

    ''' <summary>
    ''' Gets the localised text for the given key and parameters. Returns the text in the default language if the localised version is missing.
    ''' Requires that the localised text has the parameters in {0}, {1} format.
    ''' </summary>
    Public Function LocalText(ResourceName As String, ParamArray Params() As Object) As String
      Return String.Format(LocalText(ResourceName), Params)
    End Function

#Region " Misc "

    ''' <summary>
    ''' Sets the number format to use a period as the decimal seperator, and sets the date format to day month year.
    ''' 'Don't use this in web, rather call Singular.Localisation.Setup and return Singular.Localisation.GetSafeFormatSettings
    ''' </summary>
    Public Sub ForceSafeFormatSettings()

      System.Threading.Thread.CurrentThread.CurrentCulture = GetSafeFormatSettings()
      System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture
    End Sub

    Public Function GetSafeFormatSettings() As CultureInfo
      Dim culture As System.Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture.Clone()
      culture.NumberFormat.NumberDecimalSeparator = "."
      culture.NumberFormat.CurrencyDecimalSeparator = "."
      culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy"
      culture.DateTimeFormat.LongDatePattern = "dd/MM/yyyy"
      Return culture
    End Function

#End Region

  End Module

End Namespace


