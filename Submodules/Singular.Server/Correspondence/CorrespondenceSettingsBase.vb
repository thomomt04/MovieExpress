Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Namespace Correspondence

  Public Interface IEmailSettings
    Property FriendlyFrom As String
    Property EMailServer As String
    Property FromEMailAccount As String
    Property FromEMailAddress As String
    Property FromEmailPassword As String
  End Interface

  Public Interface ISmsSettings
    Property SmsProvider As Singular.SmsSending.SMSProviderType
    Property SmsAPIKey As String
    Property SmsUserName As String
    Property SmsPassword As String
  End Interface

  <Serializable>
  Public Class CorrespondenceSettingsBase
    Inherits Singular.SystemSettings.SettingsSection(Of CorrespondenceSettingsBase)
    Implements IEmailSettings, ISmsSettings

    Public Overrides ReadOnly Property Description As String
      Get
        Return "Settings for Emails and Smses"
      End Get
    End Property

    Public Overrides ReadOnly Property Name As String
      Get
        Return "Correspondence"
      End Get
    End Property

#Region " Email Settings "

    ''' <summary>
    ''' FriendlyFrom will only be used if the email doesnt have its own friendly from.
    ''' </summary>
    <Display(Description:="The recipient will see the email from: FriendlyFrom <FromAddress>."), CategoryAttribute("Email Settings")>
    Public Property FriendlyFrom As String = "" Implements IEmailSettings.FriendlyFrom

    <Display(Description:="SMTP Mail server for email sending"), CategoryAttribute("Email Settings")>
    Public Property EMailServer As String = "" Implements IEmailSettings.EMailServer

    <Display(Description:="Email Address of the account from which to send emails."), CategoryAttribute("Email Settings")>
    Public Property FromEMailAddress As String = "" Implements IEmailSettings.FromEMailAddress

    <Display(Description:="Account Name from which to send emails."), CategoryAttribute("Email Settings")>
    Public Property FromEMailAccount As String = "" Implements IEmailSettings.FromEMailAccount

    <Display(Description:="Password of the account from which to send emails."), CategoryAttribute("Email Settings")>
    Public Property FromEmailPassword As String = "" Implements IEmailSettings.FromEmailPassword

#End Region

#Region " Sms Settings "

    <Display(Description:="Sms gateway to use."), CategoryAttribute("Sms Settings"),
     Singular.DataAnnotations.DropDownWeb(GetType(Singular.SmsSending.SMSProviderType))>
    Public Property SmsProvider As Singular.SmsSending.SMSProviderType Implements ISmsSettings.SmsProvider

    <Display(Description:="API Key (Clickatell Only)"), CategoryAttribute("Sms Settings")>
    Public Property SmsAPIKey As String = "" Implements ISmsSettings.SmsAPIKey

    <Display(Description:="User Name of the account from which to send smses."), CategoryAttribute("Sms Settings")>
    Public Property SmsUserName As String = "" Implements ISmsSettings.SmsUserName

    <Display(Description:="Password of the account from which to send smses."), CategoryAttribute("Sms Settings")>
    Public Property SmsPassword As String = "" Implements ISmsSettings.SmsPassword

#End Region

  End Class

End Namespace


