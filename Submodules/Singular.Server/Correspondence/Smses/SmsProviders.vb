Namespace SmsSending

  Public Class ClickatellSender

    Private Shared mSessionID As String = ""
    Private Shared mWebService As PushServerWS

    Private Shared ReadOnly Property Settings As ClickatellSettings
      Get
        Return SmsSender.Settings
      End Get
    End Property

    Private Shared Function InitialiseMessage(Message As String) As Integer
      If mWebService Is Nothing Then
        mWebService = New PushServerWS
      End If

      If Message.Length > 160 Then
        ' we need to concatenate
        If Message.Length / 153 > 3 Then
          Throw New Exception("A message cannot be longer than " & (153 * 3) & " Characters")
        Else
          Return (Message.Length \ 153) + 1
        End If
      End If

      Return 1
    End Function

    Private Shared mLastActivity As DateTime = Date.MinValue
    Private Shared ReadOnly Property SessionID() As String
      Get
        If mSessionID = "" OrElse DateDiff(DateInterval.Minute, mLastActivity, Now) > Settings.SessionTimeMinutes Then
          mSessionID = mWebService.auth(Settings.ApiID, Settings.UserName, Settings.Password)
          mSessionID = mSessionID.Substring(mSessionID.IndexOf(":") + 1).Trim
          mLastActivity = Now
        End If
        Return mSessionID
      End Get
    End Property

    ''' <summary>
    ''' Sends and SMS to a single number. Settings must be set before calling this method.
    ''' </summary>
    ''' <param name="ToNumber"></param>
    ''' <param name="Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SendSms(ByVal ToNumber As String, ByVal Message As String) As SmsResult

      Return SendSms(New String() {ToNumber}, Message)

    End Function

    ''' <summary>
    ''' Sends an SMS to the specified numbers. Settings must be set before calling this method.
    ''' </summary>
    ''' <param name="ToNumbers"></param>
    ''' <param name="Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SendSms(ByVal ToNumbers() As String, ByVal Message As String) As SmsResult

      Dim Concat As Integer = InitialiseMessage(Message)

      Dim Result As String() = mWebService.sendmsg(SessionID, Nothing, Nothing, Nothing, ToNumbers, Settings.From, Message, Concat, 1, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)

      If Result.Length > 0 AndAlso Result(0).StartsWith("ID") Then
        Return New SmsResult(True, "")
      Else
        Return New SmsResult(False, If(Result.Length = 0, "Unknown", Result(0)))
      End If

    End Function

    ''' <summary>
    ''' Starts a batch and returns a batch ID to be used with the QuickSend Message.
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function StartBatch(Message As String) As String

      Dim Concat As Integer = InitialiseMessage(Message)

      Dim Result As String() = mWebService.startbatch(SessionID, Nothing, Nothing, Nothing, Settings.From, Concat, Message, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)

      If Result.Length > 0 AndAlso Result(0).StartsWith("ID") Then
        Return Result(0).Substring(Result(0).IndexOf(":") + 1).Trim
      Else
        Return ""
      End If

    End Function

    ''' <summary>
    ''' Sends an already defined SMS to a number. The Message must be defined by calling StartBatch.
    ''' </summary>
    ''' <param name="BatchID"></param>
    ''' <param name="ToNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function QuickSend(BatchID As String, ToNumber As String) As SmsResult

      Dim Result As String() = mWebService.quicksend(SessionID, Nothing, Nothing, Nothing, BatchID, New String() {ToNumber})

      If Result.Length > 0 AndAlso Result(0).StartsWith("ID") Then
        Return New SmsResult(True, "")
      Else
        Return New SmsResult(False, If(Result.Length = 0, "Unknown", Result(0)))
      End If

    End Function

    ''' <summary>
    ''' Sends and SMS to a single number. Settings must be set before calling this method.
    ''' </summary>
    ''' <param name="ToNumber"></param>
    ''' <param name="Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SendSmsWithMessageID(ByVal ToNumber As String, ByVal Message As String, MessageID As String) As SmsResult

      Dim Concat As Integer = InitialiseMessage(Message)
      Dim toNums As String() = New String() {ToNumber}
      Dim Result As String() = mWebService.sendmsg(SessionID, Settings.ApiID, Settings.UserName, Settings.Password, toNums, Settings.From, Message, Concat, Nothing, Settings.CallbackStatusDetailLevel, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, MessageID, Nothing, Nothing, Nothing, Nothing, Nothing)
      If Result.Length > 0 AndAlso Result(0).StartsWith("ID") Then
        Return New SmsResult(True, "")
      Else
        Return New SmsResult(False, If(Result.Length = 0, "Unknown", Result(0)))
      End If

    End Function

    ' ''' <summary>
    ' ''' Gets the last result returned from clickatell.
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared ReadOnly Property LastResult As String
    '  Get
    '    If mReturn.Length = 0 Then
    '      Return "Unknown"
    '    Else
    '      Return mReturn(0)
    '    End If
    '  End Get
    'End Property

  End Class

  Public Class SMSWarehouseSender

    Public Shared Function SendSms(ToNumbers As String(), Message As String) As SmsResult

      Dim Settings As SMSWarehouseSettings = SmsSender.Settings
      Try

        Dim Type As String = "1"
        Dim ESM As String = "0"

        If Message.Length > 160 Then
          Type = "5"
          ESM = "64"
        End If

        Dim NumberString As String = ""
        Dim First As Boolean = True
        For Each number As String In ToNumbers
          If Not First Then
            NumberString &= ","
          End If
          First = False
          NumberString &= number
        Next

        Dim req As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(Settings.SendURL & _
                                                                                "user=" & Settings.UserName & _
                                                                                "&pass=" & Settings.Password & _
                                                                                "&sid=" & Settings.SenderID & _
                                                                                "&mno=" & NumberString & _
                                                                                "&text=" & Message & _
                                                                                "&type=" & Type & _
                                                                                "&esm=" & ESM & _
                                                                                "&dcs=0")
        req.Method = "POST"

        Dim wr As System.Net.HttpWebResponse = req.GetResponse()

        Dim Result As String
        Using sr As New IO.StreamReader(wr.GetResponseStream)
          Result = sr.ReadToEnd

          If Result.StartsWith("response id", StringComparison.InvariantCultureIgnoreCase) Then
            Return New SmsResult(True, "")
          Else
            Return New SmsResult(False, Result)
          End If
        End Using

      Catch ex As Exception
        Return New SmsResult(False, Singular.Debug.RecurseExceptionMessage(ex))
      End Try

    End Function

  End Class

  Public Class VodacomSender

    Public Shared Function SendSms(ToNumber As String, Message As String) As SmsResult

      Dim Settings As VodacomSettings = SmsSender.Settings
      Dim xmlString As String = ""
      Try

        Dim EMS As String = "0"
        Message = ReplaceEscapeCharacters(Message)

        If Message.Length > 160 Then
          EMS = "1"
        End If

        Dim req As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(Settings.SendURL & _
                                                                                "username=" & Settings.UserName & _
                                                                                "&password=" & Settings.Password & _
                                                                                "&number=" & ToNumber & _
                                                                                "&message=" & Message & _
                                                                                If(EMS = "1", "&ems=" & EMS, ""))
        req.Method = "GET"

        Dim wr As System.Net.HttpWebResponse = req.GetResponse()

        Using sr As New IO.StreamReader(wr.GetResponseStream)
          Dim xml As System.Xml.XmlDocument = New System.Xml.XmlDocument
          xmlString = sr.ReadToEnd()
          xml.LoadXml(xmlString)
          'Get action from string

          Dim result As Integer = xml.ChildNodes(1).ChildNodes(0).Attributes("result").Value
          If result = 0 Then
            Dim errorCode As VodacomSmsCodes = xml.ChildNodes(1).ChildNodes(0).Attributes("error").Value
            Return New SmsResult(False, errorCode.ToString())
          Else
            Return New SmsResult(True, "")
          End If
        End Using

      Catch ex As Exception
        Return New SmsResult(False, Singular.Debug.RecurseExceptionMessage(ex) & ", " & xmlString)
      End Try

    End Function

    Public Shared Function ReplaceEscapeCharacters(ByVal Message As String) As String
      Dim ret As String = Message.Replace("%", "%25").Replace(" ", "%20").Replace("""", "%22").Replace("-", "%2D").Replace(".", "%2E").Replace("<", "%3C").Replace(">", "%3E")
      ret = ret.Replace("\", "%5C").Replace("^", "%5E").Replace("_", "%5F").Replace("`", "%60").Replace("{", "%7B").Replace("|", "%7C").Replace("}", "%7D").Replace("~", "%7E")
      Return ret ' System.Web.HttpContext.Current.Server.UrlEncode(Message)
    End Function

    Private Enum VodacomSmsCodes
      BadAuthentication = 150
      NoReadSecurity = 151
      NoSendSecurity = 152
      NoSendTooMany = 153
      NoSendBanned = 154
      NoSendDuplicate = 155
      NoSendRoute = 156
    End Enum

  End Class

  Public Class CellFindInterface

    Public Enum MessageStatus
      Received = 1
      Submitted = 2
      Queued = 3
      Sent = 4
      Failed = 5
      Retry = 6
      ErrorMessage = 7
      Delivered = 8
      Expired = 9
      Exclusion = 11
      InvalidNumber = 12
      CallLimitReached = 13
      Duplicate = 14
    End Enum

    Private Shared mService As CellFind.ServiceSoapClient
    Private Shared mSessionID As String

    Public Shared Function SendSms(ByVal ToNumber As String, ByVal Message As String) As SmsResult

      Initialise()

      Dim SessionString As String = SessionID
      If IsNumeric(SessionString) Then
        Dim MessageID = mService.SendSMSMessageSingle(SessionString, ToNumber, Message)
        Return New SmsResult(MessageID > 0, If(MessageID > 0, "", "Message Not Sent"), MessageID)
      Else
        'Credentials are incorrect.
        Return New SmsResult(False, SessionString)
      End If

    End Function

    Public Shared Sub Initialise()
      If mService Is Nothing Then

        Dim b As New ServiceModel.BasicHttpBinding(ServiceModel.BasicHttpSecurityMode.Transport)
        b.Name = "ServiceSoap"
        mService = New CellFind.ServiceSoapClient(b, New ServiceModel.EndpointAddress(New Uri("https://www.cellportal.co.za/gatewaywebservice/Service.asmx")))
      End If
    End Sub

    Public Shared Function GetSmsStatus(MessageID As Integer)
      Initialise()
      ''
      Dim ds = mService.GetSentSMSMessageDetailByID(SessionID, MessageID)

      If ds.Tables(0).Rows.Count > 0 Then
        Dim MessageStatusID As MessageStatus = ds.Tables(0).Rows(0)("Status_ID")
        If MessageStatusID = MessageStatus.Delivered Then
          Return MessageStatusID.ToString() & " on " & ds.Tables(0).Rows(0)("Delivery_Time")
        ElseIf MessageStatusID = MessageStatus.InvalidNumber Then
          Return "Invalid Number " & ds.Tables(0).Rows(0)("Recipient_No")
        ElseIf MessageStatusID = MessageStatus.Sent OrElse MessageStatusID = MessageStatus.Submitted Then
          Return "Message Sent awaiting delivery"
        ElseIf MessageStatusID = MessageStatus.Queued Then
          Return "Awaiting Response for Delivery"
        Else
          Return "Failed to send"
        End If
      End If
      Return "Reponse Unavailable"
    End Function

    Private Shared mLastActivity As DateTime = Date.MinValue
    Private Shared ReadOnly Property SessionID() As String
      Get
        If mSessionID = "" OrElse DateDiff(DateInterval.Minute, mLastActivity, Now) > 14 Then
          mSessionID = mService.LogIn(SmsSender.Settings.UserName, SmsSender.Settings.Password)
          mLastActivity = Now
        End If
        Return mSessionID
      End Get
    End Property

  End Class

  Public Class BulkSMSSender

    Public Shared Function SendSms(ToNumbers As String(), Message As String) As SmsResult

      Dim Settings As BasicSettings = SmsSender.Settings
      Try

       
        Dim NumberString As String = ""

        For Each number As String In ToNumbers
          'default to South Africa
          If number.StartsWith("0") Then number = "27" & number.Substring(1)
          'remove +
          number = number.Replace("+", "")

          NumberString &= If(NumberString = "", "", ",") & number

        Next


        Dim req As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("http://bulksms.2way.co.za/eapi/submission/send_sms/2/2.0?" & _
                                                                                "username=" & Settings.UserName & _
                                                                                "&password=" & Settings.Password & _
                                                                                "&message=" & System.Web.HttpUtility.UrlEncode(Message) & _
                                                                                "&msisdn=" & NumberString &
                                                                                "&allow_concat_text_sms=1")

        req.Method = "POST"

        Dim wr As System.Net.HttpWebResponse = req.GetResponse()

        Dim Result As String
        Using sr As New IO.StreamReader(wr.GetResponseStream)
          Result = sr.ReadToEnd

          Dim ResultInfo = Result.Split("|")

          If Singular.Misc.InSafe(ResultInfo(0), "0", "1") Then
            Return New SmsResult(True, "")
          Else
            Return New SmsResult(False, ResultInfo(1))
          End If
        End Using

      Catch ex As Exception
        Return New SmsResult(False, Singular.Debug.RecurseExceptionMessage(ex))
      End Try

    End Function

  End Class

End Namespace

