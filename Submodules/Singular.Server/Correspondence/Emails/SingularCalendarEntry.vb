Imports System.Net

#If SILVERLIGHT Then
#Else
Imports System.Net.Mail


Namespace Emails

  Public Class SingularCalendarEntry
    Inherits SingularMail

    Dim typeCalendar As New System.Net.Mime.ContentType("text/calendar")
    Dim strBodyCalendar As New Text.StringBuilder()

    Public Sub New(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String,
                                ByVal ToEmailAddress As String,
                                Subject As String, Summary As String, Location As String, OrganizerName As String,
                                OrganizerEmail As String,
                                ByVal SMTPAddress As String, Optional ByVal AccountUserName As String = "", Optional ByVal FriendlyFrom As String = "")

      MyBase.New(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, "", SMTPAddress, Nothing, Nothing, AccountUserName, FriendlyFrom)

      mEmailBodyType = SingularMailSettings.EmailBodyType.CalendarEntry

      'Create the Body in VCALENDAR format
      Dim strCalDateFormat As String = "yyyyMMddTHHmmssZ"

      'Dim strBodyCalendar As New Text.StringBuilder()
      strBodyCalendar.AppendLine("BEGIN:VCALENDAR")
      strBodyCalendar.AppendLine("PRODID:-//Microsoft Corporation//Outlook 10.0 MIMEDIR//EN")
      strBodyCalendar.AppendLine("VERSION:2.0")
      strBodyCalendar.AppendLine("METHOD:REQUEST")
      strBodyCalendar.AppendLine("BEGIN:VEVENT")

      strBodyCalendar.AppendFormat("ORGANIZER:MAILTO:{0}", "Kyle Hill" & vbCr & vbLf, OrganizerName)
      strBodyCalendar.AppendFormat("DTSTART:{0}" & vbCr & vbLf, Date.Now.AddDays(1).ToUniversalTime().ToString(strCalDateFormat))
      strBodyCalendar.AppendFormat("DTEND:{0}" & vbCr & vbLf, Date.Now.AddDays(1).ToUniversalTime().ToString(strCalDateFormat))
      strBodyCalendar.AppendFormat("LOCATION:{0}" & vbCr & vbLf, "Location")
      strBodyCalendar.AppendFormat("TRANSP:OPAQUE" & vbCr & vbLf)
      strBodyCalendar.AppendFormat("SEQUENCE:0" & vbCr & vbLf)
      strBodyCalendar.AppendFormat("UID:{0}" & vbCr & vbLf, Guid.NewGuid().ToString)
      strBodyCalendar.AppendFormat("DTSTAMP:{0}" & vbCr & vbLf, DateTime.Now.ToUniversalTime().ToString(strCalDateFormat))
      strBodyCalendar.AppendFormat("DESCRIPTION:{0}" & vbCr & vbLf, Summary)
      strBodyCalendar.AppendFormat("SUMMARY:{0}" & vbCr & vbLf, Subject)

      strBodyCalendar.AppendLine(String.Format("ATTENDEE;CN""{0}"";RSVP=TRUE:mailto:{0}", "Kyle Hill", "khill@singular.co.za"))

      strBodyCalendar.AppendLine("PRIORITY:5")
      strBodyCalendar.AppendLine("X-MICROSOFT-CDO-IMPORTANCE:1")
      strBodyCalendar.AppendLine("CLASS:PUBLIC")
      strBodyCalendar.AppendLine("BEGIN:VALARM")
      strBodyCalendar.AppendLine("TRIGGER:-PT15M")
      strBodyCalendar.AppendLine("ACTION:DISPLAY")
      strBodyCalendar.AppendLine("DESCRIPTION:Reminder")
      strBodyCalendar.AppendLine("END:VALARM")
      strBodyCalendar.AppendLine("END:VEVENT")
      strBodyCalendar.AppendLine("END:VCALENDAR")

    End Sub

    Public Overrides Sub SendMail(Optional OnComplete As System.Net.Mail.SendCompletedEventHandler = Nothing)

      Dim EmailAddresses() As String = ToEmailAddress.Split(";")

      Using msg As New System.Net.Mail.MailMessage(FromEmailAddress, EmailAddresses(0), Subject, Body)
        msg.Subject = Subject

        If mEmailBodyType = SingularMailSettings.EmailBodyType.CalendarEntry Then

          '  Add parameters to the calendar header
          typeCalendar.Parameters.Add("Content-class", "urn:content-classes:calendarmessage")
          typeCalendar.Parameters.Add("Content-Type", "text/calendar")
          typeCalendar.Parameters.Add("method", "REQUEST")
          typeCalendar.Parameters.Add("name", "meeting.ics")
          typeCalendar.Parameters.Add("Content-Transfer-Encoding", "7-bit")

          msg.AlternateViews.Add((System.Net.Mail.AlternateView.CreateAlternateViewFromString(strBodyCalendar.ToString, typeCalendar)))

        End If

        If EmailAddresses.Length > 1 Then
          For i As Integer = 1 To EmailAddresses.Length - 1
            msg.CC.Add(New System.Net.Mail.MailAddress(EmailAddresses(i)))
          Next
        End If

        'Now uses proper From Display / Friendly From. Changed by Marlborough 23 June 2010. 
        If FriendlyFrom <> "" Then
          Dim FromAddress As New System.Net.Mail.MailAddress(FromEmailAddress, FriendlyFrom)
          msg.From = FromAddress
        End If

        Dim smtp As System.Net.Mail.SmtpClient
        Dim theCredential As System.Net.NetworkCredential

        'Changed my Marlborough 28 July 09
        'If the smtp address is like this: "server.domain.com, 587", then extract the 587 and use it as the port
        If SMTPAddress.Contains(",") Then
          smtp = New System.Net.Mail.SmtpClient(SMTPAddress.Substring(0, SMTPAddress.IndexOf(",")))
          smtp.Port = SMTPAddress.Substring(SMTPAddress.IndexOf(",") + 1).Trim
        Else
          smtp = New System.Net.Mail.SmtpClient(SMTPAddress)
        End If

        'Changed by Marlborough 07 April 2010
        'Sometimes, the UserName of the credential is not the same as the email account..
        If AccountUserName = "" Then
          'Changed my Marlborough 28 July 09
          'This wasnt working. Once the from email address has reached here, it has the friendly from as well in this format: "Bob <bob@gmail.com>"
          'the credential wont work like this, and must only contain the "bob@gmail.com" part.
          If FromEmailAddress.Contains("<") Then
            Dim StrippedFrom As String = FromEmailAddress.Substring(FromEmailAddress.IndexOf("<") + 1)
            StrippedFrom = StrippedFrom.Substring(0, StrippedFrom.LastIndexOf(">")).Trim
            theCredential = New System.Net.NetworkCredential(StrippedFrom, FromEmailAddressPassword)
          Else
            theCredential = New System.Net.NetworkCredential(FromEmailAddress, FromEmailAddressPassword)
          End If
        Else
          theCredential = New System.Net.NetworkCredential(AccountUserName, FromEmailAddressPassword)
        End If

        smtp.EnableSsl = Me.EnableSsl

        'Only use the credential if the password is not blank
        If FromEmailAddressPassword.Length > 0 Then
          smtp.UseDefaultCredentials = False
          smtp.Credentials = theCredential
        Else
          smtp.UseDefaultCredentials = True
        End If
        smtp.Send(msg)

      End Using

    End Sub

  End Class

End Namespace

#End If
