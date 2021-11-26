Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Emails

  <Serializable()> _
  Public Class EmailList
    Inherits SingularBusinessListBase(Of EmailList, Email)

#Region " Business Methods "

#If Silverlight = False Then

    Public Property OverrideConnectionString As String

    Protected Overrides Function GetConnectionString() As String
      If String.IsNullOrEmpty(OverrideConnectionString) Then
        Return MyBase.GetConnectionString()
      Else
        Return OverrideConnectionString
      End If
    End Function

#End If

    Private Function GetEmailAttachmentWithDocumentID(DocumentID As Integer) As EmailAttachment
      For Each child As Email In Me
        For Each att In child.EmailAttachmentList
          If att.DocumentID = DocumentID Then
            Return att
          End If
        Next
      Next
      Return Nothing
    End Function

    Public Function GetItem(EmailID As Integer) As Email

      For Each child As Email In Me
        If child.EmailID = EmailID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Emails"

    End Function

    Private mSentCount As Integer = 0
    Public ReadOnly Property SentCount() As Integer
      Get
        Return mSentCount
      End Get
    End Property

    Private mFailedCount As Integer = 0
    Public ReadOnly Property FailedCount() As Integer
      Get
        Return mFailedCount
      End Get
    End Property

#If SILVERLIGHT Then
#Else

    Public Sub SendEmails()
      SendEmails(New List(Of SingularMailSettings.MailCredential)({MailSettings.DefaultCredential}))
    End Sub

    Public Sub SendEmails(Credentials As List(Of SingularMailSettings.MailCredential))
      SendEmailsInternal(Credentials, False)
    End Sub

    Public Sub SendEmailsFast()
      SendEmailsFast(New List(Of SingularMailSettings.MailCredential)({MailSettings.DefaultCredential}))
    End Sub

    Public Sub SendEmailsFast(Credentials As List(Of SingularMailSettings.MailCredential))
      SyncLock Me

        Dim SendInvoker As Action(Of List(Of SingularMailSettings.MailCredential), Boolean) = AddressOf SendEmailsInternal
        SendInvoker.BeginInvoke(Credentials, True, Sub(Result As System.Runtime.Remoting.Messaging.AsyncResult)

                                                     Dim CalledMethod As Action(Of List(Of SingularMailSettings.MailCredential), Boolean) = Result.AsyncDelegate

                                                     Try
                                                       CalledMethod.EndInvoke(Result)
                                                     Catch ex As Exception

                                                     End Try

                                                   End Sub, Nothing)

        'Wait until they have all sent
        System.Threading.Monitor.Wait(Me, 300000) 'timeout of 5 minutes
      End SyncLock
    End Sub

    Private Sub SendEmailsInternal(Credentials As List(Of SingularMailSettings.MailCredential), Async As Boolean)
      mSentCount = 0
      mFailedCount = 0

      For Each em As Email In Me

        Dim c As SingularMailSettings.MailCredential = Nothing

        'Get the Correct Credential for this From Address.
        For Each cred As SingularMailSettings.MailCredential In Credentials
          If cred.FromAddress = em.FromEmailAddress Then
            c = cred
            Exit For
          End If
        Next
        'If there is no match, then use the default credential.
        If c Is Nothing Then
          c = Credentials(0)
        End If


        If [Async] Then
          em.Send(c, AddressOf EmailSendComplete)
        Else
          'Synchronous
          If em.Send(c) Then
            mSentCount += 1
          Else
            mFailedCount += 1
          End If
        End If
      Next

    End Sub

    Private Sub EmailSendComplete(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)

      Dim em As Email = CType(e.UserState, SingularMail).OriginalEmail
      If em.HandleSendComplete(e.Error) Then
        mSentCount += 1
      Else
        mFailedCount += 1
      End If

      If mSentCount + mFailedCount = Me.Count Then
        'Allow the SendEmailsFast method to continue once all emails are sent.
        SyncLock (Me)
          System.Threading.Monitor.Pulse(Me)
        End SyncLock

      End If

    End Sub

#End If



#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared EmailIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.EmailID, "EmailID", DirectCast(Nothing, Integer?))

      <Display(Name:="EmailID")> _
      Public Property EmailID() As Integer?
        Get
          Return ReadProperty(EmailIDProperty)
        End Get
        Set(ByVal value As Integer?)
          LoadProperty(EmailIDProperty, value)
        End Set
      End Property

      Public Shared SentIndProperty As PropertyInfo(Of Boolean?) = RegisterProperty(Of Boolean?)(Function(c) c.SentInd, "SentInd", DirectCast(Nothing, Boolean?))

      <Display(Name:="SentInd")> _
      Public Property SentInd() As Boolean?
        Get
          Return ReadProperty(SentIndProperty)
        End Get
        Set(ByVal value As Boolean?)
          LoadProperty(SentIndProperty, value)
        End Set
      End Property

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewEmailList() As EmailList

      Return New EmailList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

Public Shared Sub BeginGetEmailList(ByVal CallBack As EventHandler(Of DataPortalResult(Of EmailList)))

Dim dp As New DataPortal(Of EmailList)
AddHandler dp.FetchCompleted, CallBack
dp.BeginFetch(New Criteria)

End Sub

Public Sub New()

' require use of MobileFormatter

End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetEmailList() As EmailList

      Return DataPortal.Fetch(Of EmailList)(New Criteria)

    End Function

    Public Shared Function GetEmailList(SentInd As Boolean) As EmailList

      Return DataPortal.Fetch(Of EmailList)(New Criteria With {.SentInd = SentInd})

    End Function

    Public Shared Function GetEmailList(EmailID As Integer) As EmailList

      Return DataPortal.Fetch(Of EmailList)(New Criteria With {.EmailID = EmailID})

    End Function

    Protected Sub New()

      ' require use of factory methods

    End Sub

    Protected Overridable Function GetEmail(ByVal sdr As SafeDataReader) As Email
      Return Email.GetEmail(sdr)
    End Function

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(GetEmail(sdr))
      End While
      Me.RaiseListChangedEvents = True

      Dim parent As Email = Nothing
      If sdr.NextResult Then
        While sdr.Read
          If IsNothing(parent) OrElse parent.EmailID <> sdr.GetInt32(1) Then
            parent = Me.GetItem(sdr.GetInt32(1))
          End If
          parent.EmailAttachmentList.RaiseListChangedEvents = False
          parent.EmailAttachmentList.Add(EmailAttachment.GetEmailAttachment(sdr))
          parent.EmailAttachmentList.RaiseListChangedEvents = True
        End While
      End If

      Dim parent2 As EmailAttachment = Nothing
      If sdr.NextResult Then
        While sdr.Read
          If IsNothing(parent2) OrElse parent2.DocumentID <> sdr.GetInt32(0) Then
            parent2 = Me.GetEmailAttachmentWithDocumentID(sdr.GetInt32(0))
          End If
          Dim Document As Documents.Document = Documents.Document.GetDocument(sdr)
          parent2.LoadDoucment(Document)
        End While
      End If

      For Each child As Email In Me
        child.CheckRules()
        For Each EmailAttachment As EmailAttachment In child.EmailAttachmentList
          EmailAttachment.CheckRules()
        Next
      Next

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(GetConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getEmailList"

            cm.Parameters.AddWithValue("@EmailID", Singular.Misc.NothingDBNull(crit.EmailID))
            cm.Parameters.AddWithValue("@SentInd", Singular.Misc.NothingDBNull(crit.SentInd))

            AddAdditionalFetchParameters(cm, crit)

            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Protected Overridable Sub AddAdditionalFetchParameters(cm As SqlCommand, crit As Criteria)

    End Sub

    Public Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        'Loop through emails and send if neccessary
        For Each Child As Email In Me
          If Child.SendOnInsert AndAlso Child.SentDate Is Nothing Then
            Child.Send()
          End If
        Next

        ' Loop through each deleted child object and call its Update() method
        For Each Child As Email In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As Email In Me
          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

    Protected Overrides Sub DataPortal_Update()

      UpdateTransactional(AddressOf Update)

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace