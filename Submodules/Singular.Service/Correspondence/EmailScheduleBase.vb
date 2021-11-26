Public Class EmailScheduleBase
  Inherits ScheduleProgramBase

  Public Enum ProgressReportFrequencyType
    Only_When_Emails_Are_Sent = 1
    Always = 2
  End Enum


  Public Sub New(ByVal ServerProgramTypeID As Integer, ByVal MailCredential As Singular.Emails.SingularMailSettings.MailCredential)
    MyBase.New(ServerProgramTypeID, "Email Scheduler")

    Singular.Emails.MailSettings.DefaultCredential = MailCredential
  End Sub

  Public Sub New(ByVal ServerProgramTypeID As Integer, ByVal MailCredential As Singular.Emails.SingularMailSettings.MailCredential, ByVal EnableSsl As Boolean)
    MyBase.New(ServerProgramTypeID, "Email Scheduler")

    Singular.Emails.MailSettings.DefaultCredential = MailCredential
    Singular.Emails.MailSettings.EnableSsl = EnableSsl
  End Sub

  Public Sub New(ByVal CustomEmailServiceName As String, ByVal ServerProgramTypeID As Integer)
    MyBase.New(ServerProgramTypeID, CustomEmailServiceName)
  End Sub

  Protected Overridable Function EmailParameters() As List(Of SqlClient.SqlParameter)
    Return Nothing
  End Function

  Protected Overridable Function GetMailCredentials() As List(Of Singular.Emails.SingularMailSettings.MailCredential)
    Return Nothing
  End Function

  Protected Overridable Function DefaultMailCredential() As Singular.Emails.SingularMailSettings.MailCredential
    Return Singular.Emails.MailSettings.DefaultCredential
  End Function

  Protected Overridable Function ProgressReportFrequency() As ProgressReportFrequencyType
    Return ProgressReportFrequencyType.Always
  End Function

  Protected Overrides Function StartSchedule() As Boolean


    If DefaultMailCredential() Is Nothing OrElse DefaultMailCredential.FromServer = "" Then
      WriteProgress("DefaultMailCredential Must be provided.")
      Return False
    Else
      WriteProgress("Email Sender started.")
      Return True
    End If

  End Function

  Protected Overrides Function StopSchedule() As Boolean
    WriteProgress("Email Sender Stopped.")
    Return True
  End Function

  Private mCredentialList As List(Of Singular.Emails.SingularMailSettings.MailCredential)

  Protected Overrides Sub TimeUp()

    AddHandler Emails.SingularMail.AddEmailFooterImage, AddressOf AddEmailFooterImage

    If ProgressReportFrequency() = ProgressReportFrequencyType.Always Then WriteProgress("Email TimeUp started")

    If mCredentialList Is Nothing Then
      mCredentialList = New List(Of Singular.Emails.SingularMailSettings.MailCredential)
      mCredentialList.Add(DefaultMailCredential)
      Dim mc As List(Of Singular.Emails.SingularMailSettings.MailCredential) = GetMailCredentials()
      If Not IsNothing(mc) Then
        mCredentialList.AddRange(mc)
      End If
    End If

    Dim EmailList As Singular.Emails.EmailList

    Try
      EmailList = Singular.Emails.EmailList.GetEmailList(False)

      If EmailList.Count > 0 Then
        If ProgressReportFrequency() = ProgressReportFrequencyType.Always Then
          WriteProgress("Email Count:" & EmailList.Count.ToString)
        End If
        EmailList.SendEmails(mCredentialList)
        WriteProgress("Sent " & EmailList.SentCount & " Emails, " & EmailList.FailedCount & " Failed to Send")
        EmailList.Save()
      Else
        If ProgressReportFrequency() = ProgressReportFrequencyType.Always Then
          WriteProgress("No Emails to Send")
        End If
      End If

    Catch ex As Exception
      WriteProgress("Error Sending Emails: (" & ex.Source & ") " & ex.Message & " | " & ex.StackTrace)
    Finally
      RemoveHandler Emails.SingularMail.AddEmailFooterImage, AddressOf AddEmailFooterImage
    End Try

  End Sub

  Protected Overridable Sub AddEmailFooterImage(sender As Object, e As Emails.SingularMail.AddEmailFooterImageEventArgs)

  End Sub

End Class




