Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace SmsSending

  <Serializable()> _
  Public Class Sms
    Inherits SingularBusinessBase(Of Sms)
    Implements Correspondence.IExtraInfoContainer

#Region " Properties and Methods "

#Region " Properties "

    Public Shared SmsIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SmsID, "Sms", 0)
    ''' <Summary>
    ''' Gets the Sms value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Sms", Description:="")> _
    Public ReadOnly Property SmsID() As Integer
      Get
        Return GetProperty(SmsIDProperty)
      End Get
    End Property

    Public Shared MessageProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Message, "Message", "")
    ''' <Summary>
    ''' Gets and sets the Message value
    ''' </Summary>
    '''StringLength(459, ErrorMessage:="Message cannot be more than 459 characters")
    <Display(Name:="Message", Description:="Message text of sms")> _
    Public Property Message() As String
      Get
        Return GetProperty(MessageProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(MessageProperty, Value)
      End Set
    End Property

    Public Shared CreatedDateProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.CreatedDate, "Created Date", New SmartDate(Now()))
    ''' <Summary>
    ''' Gets the Created Date value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Created Date", Description:="Date at which sms was created")> _
    Public ReadOnly Property CreatedDate() As SmartDate
      Get
        Return GetProperty(CreatedDateProperty)
      End Get
    End Property

    Public Shared DateToSendProperty As PropertyInfo(Of DateTime?) = RegisterProperty(Of DateTime?)(Function(c) c.DateToSend, "Date To Send")
    ''' <Summary>
    ''' Gets and sets the Date To Send value
    ''' </Summary>
    <Display(Name:="Date To Send", Description:="Date to send the email (will be 5 minutes after this date)")> _
    Public Property DateToSend() As DateTime?
      Get
        Return GetProperty(DateToSendProperty)
      End Get
      Set(ByVal Value As DateTime?)
        SetProperty(DateToSendProperty, Value)
      End Set
    End Property

    Public Shared IgnoreProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Ignore, "Ignore", False)
    ''' <Summary>
    ''' Gets and sets the Ignore value
    ''' </Summary>
    <Display(Name:="Ignore", Description:="Tick indicates that this email will be ignored"),
    Required(ErrorMessage:="Ignore required")> _
    Public Property Ignore() As Boolean
      Get
        Return GetProperty(IgnoreProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(IgnoreProperty, Value)
      End Set
    End Property

    ''' <summary>
    ''' Used to store extra information about this sms. E.g. to link it to another record
    ''' </summary>
    Public Property ExtraInfo As Correspondence.ExtraInfo Implements Correspondence.IExtraInfoContainer.ExtraInfo

#End Region

#Region " Child Lists "

    Public Shared SmsRecipientListProperty As PropertyInfo(Of SmsRecipientList) = RegisterProperty(Of SmsRecipientList)(Function(c) c.SmsRecipientList, "Sms Recipient List")

		<Display(AutoGenerateField:=False), Browsable(True)> _
		Public ReadOnly Property SmsRecipientList() As SmsRecipientList
			Get
				If Not FieldManager.FieldExists(SmsRecipientListProperty) Then
					LoadProperty(SmsRecipientListProperty, SmsSending.SmsRecipientList.NewSmsRecipientList())
				End If
				Return GetProperty(SmsRecipientListProperty)
			End Get
		End Property

    Public Shared SmsChildProperty As PropertyInfo(Of ISmsChild) = RegisterProperty(Of ISmsChild)(Function(c) c.SmsChild)

    <Display(AutoGenerateField:=False)> _
    Public Property SmsChild() As ISmsChild
      Get
        Return GetProperty(SmsChildProperty)
      End Get
      Set(value As ISmsChild)
        SetProperty(SmsChildProperty, value)
      End Set
    End Property

#End Region

#Region " Methods "

#If Silverlight = False Then

    Public Shared Sub SendImmediate(SmsID As Integer)

      Dim sl As SmsList = SmsList.GetSmsList(SmsID)
      If sl.Count = 1 Then
        sl(0).SendAndSave()
      End If

    End Sub

    ''' <summary>
    ''' Sends the email and saves it asyncronously. If the send fails, it will be saved with the not sent error.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SendAndSave()

      Send(, Sub()

               Try
                 Dim sl As SmsList = SmsList.NewSmsList
                 sl.Add(Me)
                 sl.Save()
               Catch ex As Exception
               End Try

             End Sub)

    End Sub

#End If

    Public Sub Send(Optional ByVal From As String = "", Optional OnComplete As Action = Nothing)
#If Silverlight = False Then

      Dim BatchID As String = ""

      If Me.SmsRecipientList.Count > 1 AndAlso SmsSender.SMSProvider = SMSProviderType.Clickatell Then
        'Start a batch because you can send the message once, and then just specify each number to send to after that.
        BatchID = ClickatellSender.StartBatch(Message)

      End If

      'This method will check if the Batch ID is filled in.
      For Each smsr As SmsRecipient In Me.SmsRecipientList
        smsr.Send([From], BatchID, OnComplete)
      Next

#End If
    End Sub

    ''' <summary>
    ''' Sends to all the recipients at once, this way you will not be able to tell which numbers fail.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SendAll()
#If SILVERLIGHT Then
#Else
      If SmsSender.SendSms(GetNumbers, Message).Sent Then
        For Each sr As SmsRecipient In Me.SmsRecipientList
          sr.SentDate = Now
        Next
      End If
#End If

    End Sub

    Public Sub AddRecipient(CellNo As String, Optional RecipientName As String = "")
      Dim smsr As SmsRecipient = SmsRecipient.NewSmsRecipient()
      smsr.CellNo = CellNo
      smsr.RecipientName = RecipientName
      SmsRecipientList.Add(smsr)
    End Sub

    Public Function GetRecipentsAndNumbersAsString() As String

      Dim sReturn As String = ""
      For Each Recipient As String In Me.GetRecipentsAndNumbers
        If sReturn <> "" Then
          sReturn &= "; "
        End If

        sReturn &= Recipient
      Next
      Return sReturn

    End Function

    Public Function GetRecipentsAndNumbers() As String()

      Dim sReturn(SmsRecipientList.Count - 1) As String

      For i As Integer = 0 To sReturn.Length - 1
        sReturn(i) = SmsRecipientList(i).RecipientName & ": " & SmsRecipientList(i).CellNo
      Next

      Return sReturn

    End Function

    Public Function GetNumbers() As String()

      Dim sReturn(SmsRecipientList.Count - 1) As String

      For i As Integer = 0 To sReturn.Length - 1
        sReturn(i) = SmsRecipientList(i).CellNo
      Next

      Return sReturn

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SmsIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.Message.Length = 0 Then
        If Me.IsNew Then
          Return "New Smse"
        Else
          Return "Blank Smse"
        End If
      Else
        Return Me.Message
      End If

    End Function

    Protected Overrides ReadOnly Property TableReferencesToIgnore() As String()
      Get
        Return New String() {"SmsRecipients"}
      End Get
    End Property

#End Region

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    ''' <summary>
    ''' Override if your project has an inherited SMS class.
    ''' </summary>
    Public Shared CreateSMS As Func(Of Sms) = Function()
                                                Return DataPortal.CreateChild(Of Sms)()
                                              End Function

    Public Shared Function NewSms() As Sms

      Return CreateSMS()

    End Function

    Public Sub New()

      MarkAsChild()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetSms(ByVal dr As SafeDataReader) As Sms

      Dim s = CreateSMS()
      s.Fetch(dr)
      Return s

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        Populate(sdr)
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Protected Overridable Sub Populate(sdr As SafeDataReader)
      With sdr
        LoadProperty(SmsIDProperty, .GetInt32(0))
        LoadProperty(MessageProperty, .GetString(1))
        LoadProperty(CreatedDateProperty, .GetSmartDate(2))
        LoadProperty(DateToSendProperty, .GetDateTime(3))
        LoadProperty(IgnoreProperty, .GetBoolean(4))
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

          Dim paramSmsID As SqlParameter = .Parameters.Add("@SmsID", SqlDbType.Int)
          paramSmsID.Value = GetProperty(SmsIDProperty)
          If Me.IsNew Then
            cm.CommandText = "InsProcs.insSms"
            paramSmsID.Direction = ParameterDirection.Output
          Else
            cm.CommandText = "UpdProcs.updSms"
          End If

          AddParams(cm)

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(SmsIDProperty, paramSmsID.Value)
          End If

          ' update child objects
          Me.SmsRecipientList.Update()

          If Me.SmsChild IsNot Nothing Then
            Me.SmsChild.SmsID = Me.SmsID

            If Me.SmsChild.IsNew Then
              CObj(Me.SmsChild).Insert()
            Else
              CObj(Me.SmsChild).Update()
            End If
          End If

          MarkOld()
        End With
      Else
        ' update child objects
        Me.SmsRecipientList.Update()

        If Me.SmsChild IsNot Nothing Then
          Me.SmsChild.SmsID = Me.SmsID

          If Me.SmsChild.IsNew Then
            CObj(Me.SmsChild).Insert()
          Else
            CObj(Me.SmsChild).Update()
          End If
        End If
      End If

    End Sub

    Protected Overridable Sub AddParams(cm As SqlCommand)

      cm.Parameters.AddWithValue("@Message", GetProperty(MessageProperty))
      cm.Parameters.AddWithValue("@DateToSend", GetProperty(DateToSendProperty))
      cm.Parameters.AddWithValue("@Ignore", GetProperty(IgnoreProperty))
      If ExtraInfo IsNot Nothing Then ExtraInfo.BeforeUpdate(cm)

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delSms"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@SmsID", GetProperty(SmsIDProperty))
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

#End If

#End Region

#End Region

  End Class


End Namespace