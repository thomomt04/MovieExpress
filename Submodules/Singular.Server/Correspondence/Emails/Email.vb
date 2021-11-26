Imports Csla
Imports Csla.Data
Imports Csla.Serialization
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Emails

  <Serializable()>
  Public Class Email
    Inherits SingularBusinessBase(Of Email)
    Implements Correspondence.IExtraInfoContainer

#Region " Properties and Methods "

    <NonSerialized()>
    Private mSMail As SingularMail

#Region " Properties "

    Public Shared EmailIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.EmailID, "Email", 0)
    ''' <Summary>
    ''' Gets the Email value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Email", Description:="System generated unique ID"), Key>
    Public ReadOnly Property EmailID() As Integer
      Get
        Return GetProperty(EmailIDProperty)
      End Get
    End Property

    Public Shared ToEmailAddressProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ToEmailAddress, "To Email Address", "")
    ''' <Summary>
    ''' Gets and sets the To Email Address value
    ''' </Summary>
    <Display(Name:="To Email Address", Description:="Email Address to which email should be sent"),
    StringLength(1000, ErrorMessage:="To Email Address cannot be more than 1000 characters")>
    Public Property ToEmailAddress() As String
      Get
        Return GetProperty(ToEmailAddressProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(ToEmailAddressProperty, Value)
      End Set
    End Property

    Public Shared FromEmailAddressProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.FromEmailAddress, "From Email Address", "")
    ''' <Summary>
    ''' Gets and sets the From Email Address value
    ''' </Summary>
    <Display(Name:="From Email Address", Description:="Email address of the sender"),
    StringLength(100, ErrorMessage:="From Email Address cannot be more than 100 characters")>
    Public Property FromEmailAddress() As String
      Get
        Return GetProperty(FromEmailAddressProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(FromEmailAddressProperty, Value)
      End Set
    End Property

    Public Shared FriendlyFromProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.FriendlyFrom, "Friendly From", "")
    ''' <Summary>
    ''' Gets and sets the Friendly From value
    ''' </Summary>
    <Display(Name:="Friendly From", Description:="Friendly name of sender"),
    StringLength(50, ErrorMessage:="Friendly From cannot be more than 50 characters")>
    Public Property FriendlyFrom() As String
      Get
        Return GetProperty(FriendlyFromProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(FriendlyFromProperty, Value)
      End Set
    End Property

    Public Shared SubjectProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Subject, "Subject", "")
    ''' <Summary>
    ''' Gets and sets the Subject value
    ''' </Summary>
    <Display(Name:="Subject", Description:="Subject of Email"),
    StringLength(255, ErrorMessage:="Subject cannot be more than 255 characters")>
    Public Property Subject() As String
      Get
        Return GetProperty(SubjectProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(SubjectProperty, Value)
      End Set
    End Property

    Public Shared BodyProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Body, "Body", "")
    ''' <Summary>
    ''' Gets and sets the Body value
    ''' </Summary>
    <Display(Name:="Body", Description:="Body of email")>
    Public Property Body() As String
      Get
        Return GetProperty(BodyProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(BodyProperty, Value)
      End Set
    End Property

    Public Shared CCEmailAddressesProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.CCEmailAddresses, "CC Email Addresses", "")
    ''' <Summary>
    ''' Gets and sets the CC Email Addresses value
    ''' </Summary>
    <Display(Name:="CC Email Addresses", Description:="Any other Users that email should be sent to")>
    Public Property CCEmailAddresses() As String
      Get
        Return GetProperty(CCEmailAddressesProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(CCEmailAddressesProperty, Value)
      End Set
    End Property

    Public Shared CreatedByProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.CreatedBy, "Created By", "")
    ''' <Summary>
    ''' Gets the Created By value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Created By", Description:="Person who initiated creation of this email")>
    Public ReadOnly Property CreatedBy() As String
      Get
        Return GetProperty(CreatedByProperty)
      End Get
    End Property

    Public Shared CreatedDateProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.CreatedDate, "Created Date", New SmartDate(Now()))
    ''' <Summary>
    ''' Gets the Created Date value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Created Date", Description:="Date on which email was created")>
    Public ReadOnly Property CreatedDate() As SmartDate
      Get
        Return GetProperty(CreatedDateProperty)
      End Get
    End Property

    Public Shared DateToSendProperty As PropertyInfo(Of DateTime?) = RegisterProperty(Of DateTime?)(Function(c) c.DateToSend, "Date To Send")
    ''' <Summary>
    ''' Gets and sets the Date To Send value
    ''' </Summary>
    <Display(Name:="Date To Send", Description:="Date to send the email (will be 5 minutes after this date)")>
    Public Property DateToSend() As DateTime?
      Get
        Return GetProperty(DateToSendProperty)
      End Get
      Set(ByVal Value As DateTime?)
        SetProperty(DateToSendProperty, Value)
      End Set
    End Property

    Public Shared SentDateProperty As PropertyInfo(Of DateTime?) = RegisterProperty(Of DateTime?)(Function(c) c.SentDate, "Sent Date")
    ''' <Summary>
    ''' Gets and sets the Sent Date value
    ''' </Summary>
    <Display(Name:="Sent Date", Description:="Date on which email was sent")>
    Public Property SentDate() As DateTime?
      Get
        Return GetProperty(SentDateProperty)
      End Get
      Set(ByVal Value As DateTime?)
        SetProperty(SentDateProperty, Value)
      End Set
    End Property

    Public Shared NotSentErrorProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.NotSentError, "Not Sent Error", "")
    ''' <Summary>
    ''' Gets and sets the Not Sent Error value
    ''' </Summary>
    <Display(Name:="Not Sent Error", Description:="Any errors encountered during sending"),
    StringLength(1024, ErrorMessage:="Not Sent Error cannot be more than 1024 characters")>
    Public Property NotSentError() As String
      Get
        Return GetProperty(NotSentErrorProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(NotSentErrorProperty, Value)
      End Set
    End Property

    Public Shared IgnoreProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Ignore, "Ignore", False)
    ''' <Summary>
    ''' Gets and sets the Ignore value
    ''' </Summary>
    <Display(Name:="Ignore", Description:="Tick indicates that this email will be ignored"),
    Required(ErrorMessage:="Ignore required")>
    Public Property Ignore() As Boolean
      Get
        Return GetProperty(IgnoreProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(IgnoreProperty, Value)
      End Set
    End Property

    Public Shared SendOnInsertProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.SendOnInsert, "Send On Insert", False)
    ''' <Summary>
    ''' Gets and sets the SendOnInsert value
    ''' </Summary>
    <Display(Name:="Send On Insert", Description:="Tick indicates that this email will be Sent When it is Saved"),
    Required(ErrorMessage:="Ignore required")>
    Public Property SendOnInsert() As Boolean
      Get
        Return GetProperty(SendOnInsertProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(SendOnInsertProperty, Value)
      End Set
    End Property

    ''' <summary>
    ''' Used to store extra information about this email. E.g. to link it to another record
    ''' </summary>
    Public Property ExtraInfo As Correspondence.ExtraInfo Implements Correspondence.IExtraInfoContainer.ExtraInfo

#End Region

#Region " Methods "

#If SILVERLIGHT Then
#Else

    Public Shared Sub SendImmediate(EmailID As Integer)

      Dim el As EmailList = EmailList.GetEmailList(EmailID)
      If el.Count = 1 Then
        el(0).SendAndSave()
      End If

    End Sub

    ''' <summary>
    ''' Sends the email and saves it asyncronously. If the send fails, it will be saved with the not sent error.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function SendAndSave(Optional SendAsync As Boolean = True) As Email

      Dim savedEmailList As EmailList = Nothing
      Dim Method = New Action(Sub()
                                Send()
                                Dim el As EmailList = EmailList.NewEmailList
                                el.Add(Me)
                                savedEmailList = el.Save()
                              End Sub)

      If SendAsync Then
        Method.BeginInvoke(Sub(r)
                             'Do nothing
                             Try
                               Method.EndInvoke(r)
                             Catch ex As Exception

                             End Try
                           End Sub, Nothing)

        Return Nothing
      Else
        Method()

        Return savedEmailList(0)
      End If


    End Function

    ''' <summary>
    ''' Sends the email and saves it asyncronously. If the send fails, it will be saved with the not sent error.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SendAndSave(MailCred As SingularMailSettings.MailCredential, Optional SendAsync As Boolean = True, Optional OnComplete As Action(Of Emails.Email, Exception) = Nothing)

      Dim Method = New Action(Sub()
                                Try
                                  Send(MailCred)
                                  Dim el As EmailList = EmailList.NewEmailList
                                  el.Add(Me)
                                  el.Save()
                                  If OnComplete IsNot Nothing Then
                                    OnComplete(Me, Nothing)
                                  End If
                                Catch ex As Exception
                                  If OnComplete IsNot Nothing Then
                                    OnComplete(Me, ex)
                                  End If
                                End Try
                              End Sub)

      If SendAsync Then
        Method.BeginInvoke(Sub(r)
                             'Do nothing
                             Try
                               Method.EndInvoke(r)
                             Catch ex As Exception

                             End Try
                           End Sub, Nothing)
      Else
        Method()
      End If


    End Sub

    Public Shared Property SendEmailHandler As Func(Of Email, System.Net.Mail.SendCompletedEventHandler, Boolean)

    Public Function Send(Optional Cred As SingularMailSettings.MailCredential = Nothing, Optional OnComplete As System.Net.Mail.SendCompletedEventHandler = Nothing) As Boolean

      Try
        If SendEmailHandler IsNot Nothing Then
          SendEmailHandler.Invoke(Me, OnComplete)
          SentDate = Now
          Return True
        Else
          mSMail = New SingularMail(Me, Cred)
          mSMail.SendMail(OnComplete)

          If OnComplete Is Nothing Then
            Return HandleSendComplete(Nothing)
          Else
            Return True
          End If
        End If


      Catch ex As Exception
        Return HandleSendComplete(ex)
      End Try

    End Function

    Friend Function HandleSendComplete(ex As Exception)
      If ex Is Nothing Then
        SentDate = Now
      Else
        NotSentError = Singular.Debug.RecurseExceptionMessage(ex)
        If mSMail IsNot Nothing Then
          NotSentError += vbCrLf & "Settings: " & mSMail.FromEmailAddress & ", " & mSMail.SMTPAddress & ", " & mSMail.AccountUserName
        End If
      End If
      If mSMail IsNot Nothing Then
        mSMail.DisposeMessage()
      End If
      Return ex Is Nothing
    End Function

#End If

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(EmailIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.ToEmailAddress.Length = 0 Then
        If Me.IsNew Then
          Return "New Email"
        Else
          Return "Blank Email"
        End If
      Else
        Return Me.ToEmailAddress
      End If

    End Function

#End Region

#End Region

#Region " Child Lists "

    Public Shared EmailAttachmentListProperty As PropertyInfo(Of EmailAttachmentList) = RegisterProperty(Of EmailAttachmentList)(Function(c) c.EmailAttachmentList, "Email Attachment List")

    <Display(AutoGenerateField:=False), Browsable(True)>
    Public ReadOnly Property EmailAttachmentList() As EmailAttachmentList
      Get
        If Not FieldManager.FieldExists(EmailAttachmentListProperty) Then
          LoadProperty(EmailAttachmentListProperty, Emails.EmailAttachmentList.NewEmailAttachmentList())
        End If
        Return GetProperty(EmailAttachmentListProperty)
      End Get
    End Property

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Shared Function NewEmail(ToAddress As String, Subject As String, Body As String) As Email

      Dim em = NewEmail()
      em.ToEmailAddress = ToAddress
      em.Subject = Subject
      em.Body = Body
      Return em

    End Function

    ''' <summary>
    ''' Override if your project has an inherited Email class.
    ''' </summary>
    Public Shared CreateEmail As Func(Of Email) = Function()
                                                    Return DataPortal.CreateChild(Of Email)()
                                                  End Function

    Public Shared Function NewEmail() As Email

      Return CreateEmail()

    End Function

    Public Sub New()

      'MarkAsChild()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetEmail(ByVal dr As SafeDataReader) As Email

      Dim e As Email = CreateEmail()
      e.Fetch(dr)
      Return e

    End Function

    Public Overridable Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        Populate(sdr)
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Protected Overridable Sub Populate(ByRef sdr As SafeDataReader)
      With sdr
        LoadProperty(EmailIDProperty, .GetInt32(0))
        LoadProperty(ToEmailAddressProperty, .GetString(1))
        LoadProperty(FromEmailAddressProperty, .GetString(2))
        LoadProperty(FriendlyFromProperty, .GetString(3))
        LoadProperty(SubjectProperty, .GetString(4))
        LoadProperty(BodyProperty, .GetString(5))
        LoadProperty(CCEmailAddressesProperty, .GetString(6))
        LoadProperty(CreatedByProperty, .GetString(7))
        LoadProperty(CreatedDateProperty, .GetSmartDate(8))
        LoadProperty(DateToSendProperty, .GetValue(9))
        LoadProperty(SentDateProperty, .GetValue(10))
        LoadProperty(NotSentErrorProperty, .GetString(11))
        LoadProperty(IgnoreProperty, .GetBoolean(12))
      End With
    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      If Me.IsSelfDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          If IsNew Then
            cm.CommandText = InsertProcedureName
          Else
            cm.CommandText = UpdateProcedureName
          End If
          AddParams(cm)
          If ExtraInfo IsNot Nothing Then ExtraInfo.BeforeUpdate(cm)

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(EmailIDProperty, .Parameters("@EmailID").Value)
          End If
          ' update child objects
          Me.EmailAttachmentList.Update()
          MarkOld()
        End With
      Else
        ' update child objects
        Me.EmailAttachmentList.Update()
      End If

    End Sub

    Protected Overridable Sub AddParams(ByVal cm As SqlCommand)
      With cm
        Dim paramEmailID As SqlParameter = .Parameters.Add("@EmailID", SqlDbType.Int)
        paramEmailID.Value = GetProperty(EmailIDProperty)
        If Me.IsNew Then
          paramEmailID.Direction = ParameterDirection.Output
        End If
        .Parameters.AddWithValue("@ToEmailAddress", GetProperty(ToEmailAddressProperty))
        .Parameters.AddWithValue("@FromEmailAddress", GetProperty(FromEmailAddressProperty))
        .Parameters.AddWithValue("@FriendlyFrom", GetProperty(FriendlyFromProperty))
        .Parameters.AddWithValue("@Subject", GetProperty(SubjectProperty))
        .Parameters.AddWithValue("@Body", GetProperty(BodyProperty))
        .Parameters.AddWithValue("@CCEmailAddresses", GetProperty(CCEmailAddressesProperty))
        .Parameters.AddWithValue("@CreatedBy", GetProperty(CreatedByProperty))
        .Parameters.AddWithValue("@DateToSend", (New SmartDate(GetProperty(DateToSendProperty))).DBValue)
        .Parameters.AddWithValue("@SentDate", (New SmartDate(GetProperty(SentDateProperty))).DBValue)
        .Parameters.AddWithValue("@NotSentError", GetProperty(NotSentErrorProperty))
        .Parameters.AddWithValue("@Ignore", GetProperty(IgnoreProperty))
        AddExtraInfo(cm)
      End With
    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = DeleteProcedureName
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@EmailID", GetProperty(EmailIDProperty))
        DoDeleteChild(cm)
      End Using

    End Sub

    Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)

      If Me.IsNew Then Exit Sub

      With cm
        .ExecuteNonQuery()
      End With
      MarkNew()

    End Sub

    Public Shared InsertProcedureName As String = "InsProcs.insEmail"
    Public Shared UpdateProcedureName As String = "UpdProcs.updEmail"
    Public Shared DeleteProcedureName As String = "DelProcs.delEmail"

    Protected Overridable Sub AddExtraInfo(ByVal cm As SqlCommand)

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace