Imports Csla.Serialization

Namespace Emails

  Public Class EMailBuilder

    Public Shared Property RegardsText As String = "Regards"

    Public Shared Function Create(ToAddress As String, Subject As String) As EMailBuilder
      Return New EMailBuilder(ToAddress, Subject)
    End Function

    Protected mMail As Email

    Public Sub New(ToAddress As String, Subject As String)
      mMail = Email.NewEmail(ToAddress, Subject, "")
    End Sub

    Public Function AddHeading(Heading As String) As EMailBuilder
      mMail.Body &= "<span style=""font-weight: 600; font-size: 1.1em"">" & Heading & "</span>"
      Return Me
    End Function

    Public Function StartParagraph() As EMailBuilder
      mMail.Body &= "<p>"
      Return Me
    End Function

    Public Function EndParagraph() As EMailBuilder
      mMail.Body &= "</p>"
      Return Me
    End Function

    Private mListStack As New Stack(Of Boolean)

    Public Function StartList(Numbered As Boolean) As EMailBuilder
      mListStack.Push(Numbered)
      mMail.Body &= If(Numbered, "<ol>", "<ul>")
      Return Me
    End Function

    Public Function AddListItem(Text As String) As EMailBuilder
      If mListStack.Count = 0 Then Throw New Exception("StartList has not been called.")
      mMail.Body &= "<li>" & Text & "</li>"
      Return Me
    End Function

    Public Function EndList() As EMailBuilder
      Dim Numbered = mListStack.Pop
      mMail.Body &= If(Numbered, "</ol>", "</ul>")
      Return Me
    End Function

    Public Function Add(RawHTML As String) As EMailBuilder
      mMail.Body &= RawHTML
      Return Me
    End Function

    Public Function AddLine(RawHTML As String) As EMailBuilder
      mMail.Body &= RawHTML & vbCrLf
      Return Me
    End Function

    Public Function AddParagraph(Text As String) As EMailBuilder
      mMail.Body &= "<p>" & Text & "</p>"
      Return Me
    End Function

    Public Function AddParagraph(Text As String, ParamArray Params As Object()) As EMailBuilder
      Return AddParagraph(String.Format(Text, Params))
    End Function

    Public Function AddRegards() As EMailBuilder
      mMail.Body &= "<p>" & RegardsText & "</p>"
      Return Me
    End Function

    ''' <summary>
    ''' WARNING! This Only attaches the documentID, not the document contents. Use AddAttachmentData if you want the later.
    ''' </summary>
    Public Function AddAttachment(Doc As Singular.Documents.IDocument) As EMailBuilder
      mMail.EmailAttachmentList.Add(EmailAttachment.NewEmailAttachment(Doc))
      Return Me
    End Function

    Public Function AddAttachmentData(Doc As Singular.Documents.IDocument) As EMailBuilder
      mMail.EmailAttachmentList.Add(EmailAttachment.NewEmailAttachment(Doc.DocumentName, Doc.Document))
      Return Me
    End Function

    Public Function AddAttachment(AttachmentName As String, AttachmentData() As Byte) As EMailBuilder
      mMail.EmailAttachmentList.Add(EmailAttachment.NewEmailAttachment(AttachmentName, AttachmentData))
      Return Me
    End Function

    Public Function Save() As Singular.SaveHelper
      Return mMail.TrySave(GetType(EmailList))
    End Function

    ''' <summary>
    ''' Split into multiple emails if there are more than <paramref name="BatchSize" /> recipients. Note: All recipients are added as CC
    ''' </summary>
    ''' <param name="BatchSize">Max no of recipients per mail.</param>
    Public Function Split(BatchSize As Integer) As EmailList

      Dim Addresses = mMail.ToEmailAddress.Split(";")
      Dim EmailList = Singular.Emails.EmailList.NewEmailList

      Dim CurrentMail As Singular.Emails.Email = Nothing

      For i As Integer = 0 To Addresses.Length - 1
        If i Mod BatchSize = 0 Then
          CurrentMail = Singular.Emails.Email.NewEmail("", mMail.Subject, mMail.Body)

          For Each att In mMail.EmailAttachmentList
            CurrentMail.EmailAttachmentList.Add(att)
          Next

          CurrentMail.CCEmailAddresses = ""
          EmailList.Add(CurrentMail)
        End If
        CurrentMail.CCEmailAddresses &= Addresses(i).Trim & "; "
      Next

      Return EmailList

    End Function

    Public ReadOnly Property MailObject As Email
      Get
        Return mMail
      End Get
    End Property

#If SILVERLIGHT Then
#Else

    Public Overridable Function SendAndSave(SendAsync As Boolean) As Email
      Return mMail.SendAndSave(SendAsync)
    End Function

#End If

  End Class

  <Serializable()> _
  Public Class SingularMail

    Public Shared Property ReplaceBase64Images As Boolean = False

    Public Class SingularMailAttachmentList
      Inherits List(Of SingularMailAttachment)

      Public Sub AddFromMailObject(ByVal al As EmailAttachmentList)
        For Each att As EmailAttachment In al
          Add(New SingularMailAttachment(New IO.MemoryStream(att.AttachmentData), att.AttachmentName))
        Next
      End Sub
    End Class

    Public Class SingularMailAttachment
      Private mFileStream As IO.Stream
      Private mFileName As String
      Private mFullPath As String

      Public Sub New(ByVal FileStream As IO.Stream, ByVal FileName As String)
        mFileStream = FileStream
        mFileName = FileName
      End Sub

      Public Sub New(ByVal FullPath As String)
        mFullPath = FullPath
        mFileName = IO.Path.GetFileName(FullPath)
      End Sub

      Public ReadOnly Property FileName() As String
        Get
          Return mFileName
        End Get
      End Property

      Public ReadOnly Property FileStream() As IO.Stream
        Get
          Return mFileStream
        End Get
      End Property

      Public Property FullPath() As String
        Get
          Return mFullPath
        End Get
        Set(value As String)
          mFullPath = value
        End Set
      End Property

    End Class

    Private mFromEmailAddress As String
    Private mFromEmailAddressPassword As String
    Private mToEmailAddress As String
    Private mSubject As String
    Private mBody As String
    Private mSMTPAddress As String
    Private mAccountUserName As String = ""
    Private mFriendlyFrom As String = ""
    Private mEnableSsl As Boolean = False
    Private mAttachmentList As New SingularMailAttachmentList
    Private mHTMLBody As String = ""
    Protected mEmailBodyType As Singular.Emails.SingularMailSettings.EmailBodyType = MailSettings.DefaultEmailBodyType

    Friend OriginalEmail As Email
#If SILVERLIGHT = False Then
    Private mMailObject As System.Net.Mail.MailMessage = Nothing
#End If

    Public Property FromEmailAddress() As String
      Get
        Return mFromEmailAddress
      End Get
      Set(ByVal value As String)
        mFromEmailAddress = value
      End Set
    End Property

    Public Property FromEmailAddressPassword() As String
      Get
        Return mFromEmailAddressPassword
      End Get
      Set(ByVal value As String)
        mFromEmailAddressPassword = value
      End Set
    End Property

    Public Property ToEmailAddress() As String
      Get
        Return mToEmailAddress
      End Get
      Set(ByVal value As String)
        mToEmailAddress = value
      End Set
    End Property

    Public Property CCAddresses As String
    Public Property BCCAddresses As String

    Public Property Subject() As String
      Get
        Return mSubject
      End Get
      Set(ByVal value As String)
        mSubject = value
      End Set
    End Property

    Public Property Body() As String
      Get
        Return mBody
      End Get
      Set(ByVal value As String)
        mBody = value
      End Set
    End Property

    Public Property HTMLBody As String
      Get
        If mEmailBodyType = SingularMailSettings.EmailBodyType.PlainTextAndCustomHTML Then
          Return mHTMLBody
        Else

          If Body.Contains("<!--Formatted-->") Then
            'if this comment exists in the body, it means the email is already HTML formatted.
            Return HTMLWrapperStart & Body & HTMLWrapperEnd
          ElseIf Body.Contains("<!--IgnoreLibrary-->") Then
            'if this comment exists in the body, it means the entire email is already HTML formatted and does not need the wrappers.
            Return Body
          Else
            Return HTMLWrapperStart & Body.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />") & HTMLWrapperEnd
          End If

        End If
      End Get
      Set(ByVal value As String)
        mHTMLBody = value
        If value <> "" Then
          mEmailBodyType = SingularMailSettings.EmailBodyType.PlainTextAndCustomHTML
        End If
      End Set
    End Property

    Public Property SMTPAddress() As String
      Get
        Return mSMTPAddress
      End Get
      Set(ByVal value As String)
        mSMTPAddress = value
      End Set
    End Property

    Public Property AccountUserName() As String
      Get
        Return mAccountUserName
      End Get
      Set(ByVal value As String)
        mAccountUserName = value
      End Set
    End Property

    Public Property FriendlyFrom() As String
      Get
        Return mFriendlyFrom
      End Get
      Set(ByVal value As String)
        mFriendlyFrom = value
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

    ''' <summary>
    ''' When the to address contains more than 1 email address, split by ;, the first is added to the to address, the rest to cc, or bcc?
    ''' </summary>
    Public Property UseBCC As Boolean = False

    Public Property EmailBodyType() As Emails.SingularMailSettings.EmailBodyType
      Get
        Return mEmailBodyType
      End Get
      Set(ByVal value As Emails.SingularMailSettings.EmailBodyType)
        mEmailBodyType = value
      End Set
    End Property

    Public ReadOnly Property Attachments() As SingularMailAttachmentList
      Get
        Return mAttachmentList
      End Get
    End Property

    Public Sub New(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, _
                                ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                                ByVal SMTPAddress As String, ByVal Files() As System.IO.Stream, _
                                ByVal FileNames() As String, Optional ByVal AccountUserName As String = "", Optional ByVal FriendlyFrom As String = "")

      Me.FromEmailAddress = FromEmailAddress
      Me.FromEmailAddressPassword = FromEmailAddressPassword
      Me.ToEmailAddress = ToEmailAddress
      Me.Subject = Subject
      Me.Body = Body
      Me.SMTPAddress = SMTPAddress
      AddAttachments(Files, FileNames)
      Me.AccountUserName = AccountUserName
      Me.FriendlyFrom = FriendlyFrom
      Me.EnableSsl = MailSettings.EnableSsl

    End Sub

    Public Sub New(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, _
                                ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                                ByVal SMTPAddress As String)

      Me.FromEmailAddress = FromEmailAddress
      Me.FromEmailAddressPassword = FromEmailAddressPassword
      Me.ToEmailAddress = ToEmailAddress
      Me.Subject = Subject
      Me.Body = Body
      Me.SMTPAddress = SMTPAddress
      Me.EnableSsl = MailSettings.EnableSsl

    End Sub

    Public Sub New(ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                   ByVal Files() As System.IO.Stream, _
                  ByVal FileNames() As String)

      Me.FromEmailAddress = MailSettings.DefaultCredential.FromAddress
      Me.FromEmailAddressPassword = MailSettings.DefaultCredential.FromPassword
      Me.ToEmailAddress = ToEmailAddress
      Me.Subject = Subject
      Me.Body = Body
      Me.SMTPAddress = MailSettings.DefaultCredential.FromServer
      AddAttachments(Files, FileNames)
      Me.FriendlyFrom = MailSettings.DefaultCredential.FriendlyFrom
      Me.EnableSsl = MailSettings.EnableSsl

    End Sub

    Public Sub New(ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String)

      Me.FromEmailAddress = MailSettings.DefaultCredential.FromAddress
      Me.FromEmailAddressPassword = MailSettings.DefaultCredential.FromPassword
      Me.ToEmailAddress = ToEmailAddress
      Me.Subject = Subject
      Me.Body = Body
      Me.SMTPAddress = MailSettings.DefaultCredential.FromServer
      Me.FriendlyFrom = MailSettings.DefaultCredential.FriendlyFrom
      Me.EnableSsl = MailSettings.EnableSsl

    End Sub

    Public Sub New(ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
               ByVal File As System.IO.Stream, _
              ByVal FileName As String)

      Me.FromEmailAddress = MailSettings.DefaultCredential.FromAddress
      Me.FromEmailAddressPassword = MailSettings.DefaultCredential.FromPassword
      Me.ToEmailAddress = ToEmailAddress
      Me.Subject = Subject
      Me.Body = Body
      Me.SMTPAddress = MailSettings.DefaultCredential.FromServer
      AddAttachments(New System.IO.Stream() {File}, New String() {FileName})
      Me.FriendlyFrom = MailSettings.DefaultCredential.FriendlyFrom
      Me.EnableSsl = MailSettings.EnableSsl

    End Sub

    Public Sub New(Email As Email, Optional Credentials As SingularMailSettings.MailCredential = Nothing)

      If Credentials Is Nothing Then
        Credentials = MailSettings.DefaultCredential
      End If

      Me.FromEmailAddress = Credentials.FromAddress
      Me.FromEmailAddressPassword = Credentials.FromPassword
      Me.ToEmailAddress = Email.ToEmailAddress

      If TreatCCFieldAsBCC Then
        Me.BCCAddresses = Email.CCEmailAddresses
      Else
        Me.CCAddresses = Email.CCEmailAddresses
      End If

      Me.Subject = Email.Subject
      Me.Body = Email.Body
      Me.SMTPAddress = Credentials.FromServer
      For Each ea As EmailAttachment In Email.EmailAttachmentList
        AddAttachments(New System.IO.Stream() {New IO.MemoryStream(ea.AttachmentData)}, New String() {ea.AttachmentName})
      Next

      Me.AccountUserName = Credentials.FromAccount
            Me.FriendlyFrom = Email.FriendlyFrom
            If Me.FriendlyFrom = "" Then
                Me.FriendlyFrom = Credentials.FriendlyFrom
            End If

      Me.EnableSsl = MailSettings.EnableSsl
      OriginalEmail = Email

    End Sub

    Private Sub AddAttachments(ByVal Files() As System.IO.Stream, ByVal FileNames() As String)
      If Files IsNot Nothing AndAlso FileNames IsNot Nothing Then
        If Files.Length <> FileNames.Length Then
          Throw New ArgumentException("Different no of file names to file streams.")
        End If

        For i As Integer = 0 To Files.Length - 1

          mAttachmentList.Add(New SingularMailAttachment(Files(i), FileNames(i)))

        Next
      End If
    End Sub
#If SILVERLIGHT Then
#Else
    Public Shared Event AddEmailFooterImage(sender As Object, e As AddEmailFooterImageEventArgs)
    Public Class AddEmailFooterImageEventArgs
      Inherits EventArgs

      Public mail As System.Net.Mail.MailMessage
      Public EmailBuilt As Boolean = False
      Public Body As String
      Public Sub New(Body As String, mail As System.Net.Mail.MailMessage)
        Me.Body = Body
        Me.mail = mail
      End Sub

    End Class

    Public Shared Sub ConvertHTMLImagesToContendIDs(ByRef EmailMessage As System.Net.Mail.MailMessage)

      ' Replaces Base64 encoded images with ContentID objects to prevent them from being blocked.

      Dim Base64ImageTagPng As String = "src=""data:image/png;base64,"
      Dim Base64ImageTagJpeg As String = "src=""data:image/jpeg;base64,"
      Dim Base64ImageTagGif As String = "src=""data:image/gif;base64,"
      Dim Base64ImageTag As String
      Dim EmailImages As New List(Of String)()

      Dim tmpEmailBody As String = "", FinalEmailBody As String = ""
      Dim start As Int64 = -1, checkStart As Int64 = -1, StrEnd As Int64 = 0
      Dim Counter As Integer = 0

      'Make sure we have an email Body.
      If IsNothing(EmailMessage) OrElse IsNothing(EmailMessage.Body) Then
        Exit Sub
      Else
        tmpEmailBody = EmailMessage.Body
      End If


      While tmpEmailBody.Length > 0
        start = -1
        StrEnd = 0
        checkStart = -1

        'Check which image type appears first
        start = tmpEmailBody.IndexOf(Base64ImageTagPng)
        Base64ImageTag = Base64ImageTagPng

        checkStart = tmpEmailBody.IndexOf(Base64ImageTagJpeg)
        If (checkStart < start Or start = -1) AndAlso checkStart <> -1 Then
          start = checkStart
          Base64ImageTag = Base64ImageTagJpeg
        End If
        checkStart = tmpEmailBody.IndexOf(Base64ImageTagGif)
        If (checkStart < start Or start = -1) AndAlso checkStart <> -1 Then
          start = checkStart
          Base64ImageTag = Base64ImageTagGif
        End If

        If start = -1 Then
          ' If there are no more images, set the body to the remaining text and exit while.
          FinalEmailBody += tmpEmailBody
          Exit While
        End If

        FinalEmailBody += Left(tmpEmailBody, start + 5) + "cid:image" + Counter.ToString + """/>"
        tmpEmailBody = tmpEmailBody.Substring(start + Base64ImageTag.Length)
        StrEnd = tmpEmailBody.IndexOf("""", Base64ImageTag.Length)
        EmailImages.Add(tmpEmailBody.Substring(0, StrEnd))
        StrEnd = tmpEmailBody.IndexOf("/>")
        tmpEmailBody = tmpEmailBody.Substring(StrEnd + 2)

        Counter += 1

      End While

      If Counter = 0 Then Exit Sub

      Dim altView As System.Net.Mail.AlternateView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(FinalEmailBody, Nothing, System.Net.Mime.MediaTypeNames.Text.Html)
      Counter = 0
      For Each EI In EmailImages

        If IsNothing(EI) OrElse EI = "" Then
          Exit For
        End If

        Dim newImage As Byte() = Convert.FromBase64String(EI)
        Dim stream As System.IO.MemoryStream = New System.IO.MemoryStream(newImage)

        Dim yourPictureRes As New System.Net.Mail.LinkedResource(stream, System.Net.Mime.MediaTypeNames.Image.Jpeg)

        yourPictureRes.ContentId = "image" + Counter.ToString
        altView.LinkedResources.Add(yourPictureRes)
        Counter += 1
      Next

      EmailMessage.AlternateViews.Add(altView)
      EmailMessage.Body = ""

    End Sub

    Public Overridable Sub SendMail(Optional OnComplete As System.Net.Mail.SendCompletedEventHandler = Nothing)

      Dim EmailAddresses() As String = GetSafeEmailAddresses(ToEmailAddress)
      Dim CCAddressList() As String = GetSafeEmailAddresses(CCAddresses)
      Dim BCCAddressList() As String = GetSafeEmailAddresses(BCCAddresses)

      Try

        mMailObject = New System.Net.Mail.MailMessage()
        If EmailAddresses.Length > 0 Then
          mMailObject.To.Add(EmailAddresses(0))
        End If
        mMailObject.From = New Net.Mail.MailAddress(FromEmailAddress)
        mMailObject.Subject = Subject
        mMailObject.Body = Body

        ' Replaces Base64 encoded images with ContentID objects to prevent them from being blocked.
        If ReplaceBase64Images = True Then
          ConvertHTMLImagesToContendIDs(mMailObject)
        End If

        'Else
        If Not String.IsNullOrEmpty(mMailObject.Body) Then
          If mEmailBodyType = SingularMailSettings.EmailBodyType.PlainTextOnly Then
            mMailObject.Body = Body
          ElseIf mEmailBodyType = SingularMailSettings.EmailBodyType.CalendarEntry Then
            mMailObject.AlternateViews.Add((System.Net.Mail.AlternateView.CreateAlternateViewFromString(Body, Nothing, "text/calendar")))
          Else
            If mEmailBodyType <> SingularMailSettings.EmailBodyType.CustomHTMLOnly Then
              mMailObject.AlternateViews.Add(System.Net.Mail.AlternateView.CreateAlternateViewFromString(Body, Nothing, "text/plain"))
            End If
            Dim args As New AddEmailFooterImageEventArgs(Body, mMailObject)
            RaiseEvent AddEmailFooterImage(Me, args)
            If Not args.EmailBuilt Then
              mMailObject.AlternateViews.Add(System.Net.Mail.AlternateView.CreateAlternateViewFromString(HTMLBody, Nothing, "text/html"))
            End If
          End If
          'End If
        End If

        If EmailAddresses.Length > 1 Then
          For i As Integer = 1 To EmailAddresses.Length - 1
            If UseBCC Then
              mMailObject.Bcc.Add(New System.Net.Mail.MailAddress(EmailAddresses(i)))
            Else
              mMailObject.CC.Add(New System.Net.Mail.MailAddress(EmailAddresses(i)))
            End If
          Next
        End If

        For Each addr In CCAddressList
          mMailObject.CC.Add(New System.Net.Mail.MailAddress(addr))
        Next
        For Each addr In BCCAddressList
          mMailObject.Bcc.Add(New System.Net.Mail.MailAddress(addr))
        Next

        'Now uses proper From Display / Friendly From. Changed by Marlborough 23 June 2010. 
        If FriendlyFrom <> "" Then
          Dim FromAddress As New System.Net.Mail.MailAddress(FromEmailAddress, FriendlyFrom)
          mMailObject.From = FromAddress
        End If

        For Each att As SingularMailAttachment In mAttachmentList
          mMailObject.Attachments.Add(New System.Net.Mail.Attachment(att.FileStream, att.FileName))
        Next

        Dim smtp As System.Net.Mail.SmtpClient
        Dim theCredential As System.Net.NetworkCredential

        'Changed my Marlborough 28 July 09
        'If the smtp address is like this: "server.domain.com, 587", then extract the 587 and use it as the port
        If SMTPAddress.Contains(",") Then
          smtp = New System.Net.Mail.SmtpClient(SMTPAddress.Substring(0, SMTPAddress.IndexOf(",")))
          Dim port = SMTPAddress.Substring(SMTPAddress.IndexOf(",") + 1).Trim
          smtp.Port = port

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

        If smtp.Port = 587 AndAlso Not smtp.EnableSsl Then
          ' enable SSL
          smtp.EnableSsl = True
        End If

        'Only use the credential if the password is not blank
        If FromEmailAddressPassword.Length > 0 Then
          smtp.UseDefaultCredentials = False
          smtp.Credentials = theCredential
        Else
          smtp.UseDefaultCredentials = True
        End If
        If OnComplete IsNot Nothing Then
          AddHandler smtp.SendCompleted, OnComplete
          smtp.SendAsync(mMailObject, Me)
        Else
          smtp.Send(mMailObject)
        End If


      Finally

        If OnComplete Is Nothing AndAlso mMailObject IsNot Nothing Then
          mMailObject.Dispose()
          mMailObject = Nothing
        End If

      End Try

    End Sub

    Friend Sub DisposeMessage()
      If mMailObject IsNot Nothing Then
        mMailObject.Dispose()
      End If
    End Sub

    Private Shared Function GetSafeEmailAddresses(ToAddress As String) As String()
      If String.IsNullOrEmpty(ToAddress) Then
        Return {}
      Else
        Dim ToAddresses = ToAddress.Replace(" ", "").Split({";"}, StringSplitOptions.RemoveEmptyEntries)

        If Not String.IsNullOrEmpty(MailSettings.AllowedEmailAddresses) Then
          'Replace addresses
          For i As Integer = 0 To ToAddresses.Length - 1
            ToAddresses(i) = MailSettings.GetSafeAddress(ToAddresses(i))
          Next

        End If

        Return ToAddresses
      End If
    End Function

    Public Function EmbeddedImages(image As Drawing.Image, ByRef ContentId As String) As System.Net.Mail.AlternateView

      'then we create the Html part
      'to embed images, we need to use the prefix 'cid' in the img src value
      HTMLBody = "<img alt="""" hspace=0 src=""cid:TradewaySignature"" align=baseline border = 0 > """

      'create the AlternateView for embedded image

      Dim io As New IO.MemoryStream
      image.Save(io, System.Drawing.Imaging.ImageFormat.Jpeg)
      Dim imageView As New System.Net.Mail.AlternateView(io, System.Net.Mime.MediaTypeNames.Image.Jpeg)
      imageView.ContentId = ContentId
      imageView.TransferEncoding = System.Net.Mime.TransferEncoding.Base64

      Return imageView

    End Function ' End EmbedImages

    Public Shared Sub QuickSendEmail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, _
                                ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                                ByVal SMTPAddress As String)
      Dim em As SingularMail = New SingularMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress)
      em.SendMail()
    End Sub

    Public Shared Sub QuickSendEmail(ByVal FromEmailAddress As String, ByVal FriendlyFrom As String, ByVal FromEmailAddressPassword As String, _
                                ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                                ByVal SMTPAddress As String)
      Dim em As SingularMail = New SingularMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress)
      em.FriendlyFrom = FriendlyFrom
      em.SendMail()
    End Sub

    Public Shared Sub QuickSendEmail(ByVal FromEmailAddress As String, FriendlyFrom As String, ByVal FromEmailAddressPassword As String, _
                                ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                                ByVal SMTPAddress As String, ByVal Document As IO.Stream, ByVal DocumentName As String)
      Dim em As SingularMail = New SingularMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, {Document}, {DocumentName})
      em.FriendlyFrom = FriendlyFrom
      em.SendMail()
    End Sub

    Public Shared Sub QuickSendEmail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, _
                                ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                                ByVal SMTPAddress As String, ByVal Document As IO.Stream, ByVal DocumentName As String)
      Dim em As SingularMail = New SingularMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, {Document}, {DocumentName})
      em.SendMail()
    End Sub

    Public Shared Sub QuickSendEmail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, _
                                ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                                ByVal SMTPAddress As String, ByVal Document() As IO.Stream, ByVal DocumentName() As String)
      Dim em As SingularMail = New SingularMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, Document, DocumentName)
      em.SendMail()
    End Sub

    Public Shared Sub QuickSendEmail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, _
                               ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
                               ByVal SMTPAddress As String, ByVal FilesToAttach() As String)

      Dim Files(FilesToAttach.Length) As System.IO.Stream
      Dim FileNames(FilesToAttach.Length) As String
      If Not FilesToAttach Is Nothing Then
        For i As Integer = 0 To FilesToAttach.Length - 1
          Files(i) = New System.IO.MemoryStream(System.IO.File.ReadAllBytes(FilesToAttach(i)))
          FileNames(i) = System.IO.Path.GetFileName(FilesToAttach(i))
        Next
      End If
      QuickSendEmail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, Files, FileNames)

    End Sub

#End If

  End Class


End Namespace
