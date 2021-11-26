Namespace Emails

  Public Class SingularMailSettings

    Public Class MailCredential

#If SILVERLIGHT Then
#Else
      Public Sub New()

      End Sub

      Public Sub New(loadFromConfig As Boolean)

        If loadFromConfig Then
          Me.FromAddress = System.Configuration.ConfigurationManager.AppSettings("FromEmailAddress")
          Me.FromAccount = Me.FromAddress
          Me.FromPassword = System.Configuration.ConfigurationManager.AppSettings("FromEmailAddressPassword")
          Me.FromServer = System.Configuration.ConfigurationManager.AppSettings("SMTPAddress")
        End If

      End Sub
#End If

      Private mFromAddress As String = ""
      Public Property FromAddress() As String
        Get
          Return mFromAddress
        End Get
        Set(ByVal value As String)
          mFromAddress = value
        End Set
      End Property

      Private mFromServer = ""
      Public Property FromServer() As String
        Get
          Return mFromServer
        End Get
        Set(ByVal value As String)
          mFromServer = value
        End Set
      End Property

      Private mFromAccount As String = ""
      Public Property FromAccount() As String
        Get
          Return mFromAccount
        End Get
        Set(ByVal value As String)
          If value Is Nothing Then
            mFromAccount = ""
          Else
            mFromAccount = value
          End If
        End Set
      End Property

      Private mFriendlyFrom As String = ""
      Public Property FriendlyFrom() As String
        Get
          Return mFriendlyFrom
        End Get
        Set(ByVal value As String)
          If value Is Nothing Then
            mFriendlyFrom = ""
          Else
            mFriendlyFrom = value
          End If
        End Set
      End Property

      Private mFromPassword As String = ""
      Public Property FromPassword() As String
        Get
          Return mFromPassword
        End Get
        Set(ByVal value As String)
          If value Is Nothing Then
            mFromPassword = ""
          Else
            mFromPassword = value
          End If
        End Set
      End Property

#If SILVERLIGHT Then
#Else

      ''' <summary>
      ''' Populates the settings from your projects settings class. Inherit from Singular.Correspondence.CorrespondenceSettingsBase, otherwise implement Singular.Correspondence.IEmailSettings
      ''' </summary>
      Public Sub FromProjectSettings(EmailSettings As Correspondence.IEmailSettings)
        mFriendlyFrom = EmailSettings.FriendlyFrom
        mFromServer = EmailSettings.EMailServer
        mFromAddress = EmailSettings.FromEMailAddress
        mFromAccount = EmailSettings.FromEMailAccount
        mFromPassword = EmailSettings.FromEmailPassword
      End Sub

#End If

    End Class

    Private mDefaultCredential As New MailCredential
    Private mEnableSsl As Boolean = False
    Private mUseHTML As Boolean = True
    Private mDefaultEmailBodyType As EmailBodyType = EmailBodyType.PlainTextAndSimpleHTML

    Public Enum EmailBodyType
      ''' <summary>
      ''' Sends plain text version of the email only. The email will look like it's in Notepad.
      ''' </summary>
      ''' <remarks></remarks>
      PlainTextOnly = 1
      ''' <summary>
      ''' Adds a Body tag around the plain text to change the font of the email body. Will use the text in the Body property.
      ''' </summary>
      ''' <remarks></remarks>
      PlainTextAndSimpleHTML = 3
      ''' <summary>
      ''' Uses the value in the HTMLBody Property as the HTML part of the Email.
      ''' </summary>
      ''' <remarks></remarks>
      PlainTextAndCustomHTML = 5
      ''' <summary>
      ''' Uses the value in the HTMLBody Property as the HTML part of the Email. Does not add Plain Text Version
      ''' </summary>
      ''' <remarks></remarks>
      CustomHTMLOnly = 4
      ''' <summary>
      ''' Sends calendar entry version of the email only.
      ''' </summary>
      ''' <remarks></remarks>
      CalendarEntry = 2

    End Enum

    Public Property DefaultCredential() As MailCredential
      Get
        Return mDefaultCredential
      End Get
      Set(ByVal value As MailCredential)
        mDefaultCredential = value
      End Set
    End Property

    Public Property EnableSsl() As Boolean
      Get
        Return mEnableSsl
      End Get
      Set(ByVal value As Boolean)
        mEnableSsl = value
      End Set
    End Property

    Public Property DefaultEmailBodyType As EmailBodyType
      Get
        Return mDefaultEmailBodyType
      End Get
      Set(ByVal value As EmailBodyType)
        mDefaultEmailBodyType = value
      End Set
    End Property

    ''' <summary>
    ''' If not empty, the library will check if the to email addresses are in this list. If not, the address will be changed to the first address in this list.
    ''' Seperate with ;
    ''' </summary>
    Public Property AllowedEmailAddresses As String
      Get
        Return mAllowedEmailAddresses
      End Get
      Set(value As String)
        mAllowedEmailAddresses = value

        mAllowedEmailAddressesHT = New Dictionary(Of String, Boolean)
        If Not String.IsNullOrEmpty(mAllowedEmailAddresses) Then
          Dim First As Boolean = True
          For Each address In mAllowedEmailAddresses.Split({";"}, StringSplitOptions.RemoveEmptyEntries)
            mAllowedEmailAddressesHT.Add(address.Trim.ToLower, True)
            If First Then
              First = False
              mOverrideAddress = address.Trim.ToLower
            End If
          Next
        End If
      End Set
    End Property

    Private mAllowedEmailAddresses As String
    Private mAllowedEmailAddressesHT As Dictionary(Of String, Boolean)
    Private mOverrideAddress As String

    Public Function GetSafeAddress(EmailAddress As String) As String
      If mAllowedEmailAddressesHT.ContainsKey(EmailAddress.Trim.ToLower) Then
        Return EmailAddress
      Else
        Return mOverrideAddress
      End If
    End Function

  End Class

End Namespace