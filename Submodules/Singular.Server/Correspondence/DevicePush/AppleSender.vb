Imports System.Net.Security
Imports System.Net.Sockets
Imports System.Security.Cryptography.X509Certificates
Imports System.Text

Namespace DevicePush

  Public MustInherit Class Sender
    Public MustOverride Sub Send()
  End Class

  Public Class GCMSender
    Inherits Sender

    Public Shared Property GoogleCloudApiKey As String = ""

    Private Class GCMResponse
      Public Property multicast_id As String
      Public Property success As Integer
      Public Property failure As Integer
      Public Property canonical_ids As Integer
      Public Property results As New List(Of GCMResult)

      Public Class GCMResult
        Public Property message_id As String
        Public Property [error] As String = ""
      End Class
    End Class

    Private mDNList As New List(Of DeviceNotification)

    Public Sub AddSendID(dn As DeviceNotification)
      mDNList.Add(dn)
    End Sub

    Public Overrides Sub Send()

      Try

        'Build the json message
        Dim JSON As String = "{ ""registration_ids"": ["
        Dim First As Boolean = True
        For Each dn As DeviceNotification In mDNList
          If First Then
            First = False
          Else
            JSON &= ","
          End If
          JSON &= """" & dn.SendID & """"
        Next
        JSON &= "], ""data"": { ""message"": """ & mDNList(0).Message & """ }}"

        'Create the web request.
        Dim req = System.Net.WebRequest.Create("https://android.googleapis.com/gcm/send")
        req.Method = "POST"
        req.Headers.Add("Authorization", "key=" & GoogleCloudApiKey)
        req.ContentType = "application/json"

        Dim Bytes() = System.Text.Encoding.UTF8.GetBytes(JSON)
        req.ContentLength = Bytes.Length
        Using ReqStream = req.GetRequestStream
          ReqStream.Write(Bytes, 0, Bytes.Length)
        End Using

        Using Response = req.GetResponse
          Using RespStream = Response.GetResponseStream
            Dim ResponseText = New IO.StreamReader(RespStream).ReadToEnd

            Dim ResponseObj = System.Web.Helpers.Json.Decode(Of GCMResponse)(ResponseText)
            For i As Integer = 0 To mDNList.Count - 1

              If ResponseObj.results.Count > i Then
                mDNList(i).SetResult(ResponseObj.results(i).error)
              Else
                mDNList(i).SetResult("unknown")
              End If

            Next

            '{"multicast_id":8084468262130779308,"success":1,"failure":1,"canonical_ids":0,"results":[{"message_id":"0:1394440810158046%a03d212cf9fd7ecd"},{"error":"InvalidRegistration"}]}

          End Using
        End Using

      Catch ex As Exception
        For Each dn As DeviceNotification In mDNList
          dn.SetResult("Exception: " & Singular.Debug.RecurseExceptionMessage(ex))
        Next
      End Try

    End Sub

  End Class

  Public Class AppleSender

    Public Shared Property CertificatePath_Development As String = ""
    Public Shared Property CertificatePath_Production As String = ""
    Public Shared Property CertificatePassword As String = ""
    Public Shared Property AlwaysUseTest As Boolean = False

    Private mCertificate As X509Certificate2
    Private mCertificates As X509CertificateCollection
    Private mServerURL As String
    Private mFeedbackURL As String

    Private mConnectionOpen As Boolean = False
    Private mAPNSClient As TcpClient
    Private mAPNSStream As SslStream

    Private mMessageList As New MessageList

    Private Class MessageList
      Inherits List(Of Message)

      'Public Pending As Boolean = False

      Public Sub SetResponse(SequenceID As Integer, [Error] As String)

        For i As Integer = 0 To Me.Count - 1

          Dim msg = Me(i)
          If [Error] = "" OrElse msg.SequenceID < SequenceID Then
            'Success
            msg.DN.SetResult("")

          ElseIf msg.SequenceID = SequenceID Then
            msg.DN.SetResult([Error])

          Else
            msg.Pending = True
          End If

        Next

      End Sub

      Public Function HasPending() As Boolean
        For Each msg As Message In Me
          If msg.Pending Then
            Return True
          End If
        Next
        Return False
      End Function

    End Class

    Private Class Message
      Public DN As DeviceNotification
      Public SequenceID As Integer
      Public Pending As Boolean = True

      Public Sub New(DN As DeviceNotification, SequenceID As Integer)
        Me.DN = DN
        Me.SequenceID = SequenceID
      End Sub

      Public Function GetPayload() As Byte()

        'convert Device token to hex value.
        Dim deviceToken(31) As Byte
        For i As Integer = 0 To 31
          deviceToken(i) = Byte.Parse(DN.SendID.Substring(i * 2, 2), Globalization.NumberStyles.HexNumber)
        Next

        Dim ms As New IO.MemoryStream()

        'Command
        ms.WriteByte(1)

        ms.Write(BitConverter.GetBytes(SequenceID), 0, 4)

        'ExpiryDate
        Dim epoch As Integer = (DateTime.UtcNow.AddMinutes(300) - New DateTime(1970, 1, 1)).TotalSeconds
        Dim timeStamp = BitConverter.GetBytes(epoch)
        ms.Write(timeStamp, 0, timeStamp.Length)

        Dim tokenLength = BitConverter.GetBytes(CType(32, Int16))
        Array.Reverse(tokenLength)
        'device token length
        ms.Write(tokenLength, 0, 2)

        'Token
        ms.Write(deviceToken, 0, 32)

        'String length
        Dim apnMessage As String = "{ ""aps"": { ""alert"": """ & Singular.Web.Utilities.JavaScriptWriter.EscapeJSONCharacters(DN.Message) & """ } }"
        Dim apnMessageLength = BitConverter.GetBytes(CType(apnMessage.Length, Int16))
        Array.Reverse(apnMessageLength)
        ms.Write(apnMessageLength, 0, 2)

        'Write the message
        ms.Write(Encoding.ASCII.GetBytes(apnMessage), 0, apnMessage.Length)
        Return ms.ToArray()

      End Function

    End Class

    Public Sub QueueMessage(dn As DeviceNotification)

      mMessageList.Add(New Message(dn, mMessageList.Count))

    End Sub

    Private Shared mLastFeedBackCheck As Date = Date.MinValue

    Public Sub SendQueuedMessages()

      'Check for old device ids.
      If mLastFeedBackCheck.Date <> Now.Date Then
        mLastFeedBackCheck = Now
        ReadFeedBack(mMessageList)
      End If

      'Send pending messages.
      For i As Integer = 1 To 5
        If Not mMessageList.HasPending Then
          Exit Sub
        End If

        SendMessages()
      Next

    End Sub

    Private Sub SendMessages()

      If Connect(Service.PushNotifications) Then

        For Each msg As Message In mMessageList

          If msg.Pending AndAlso mAPNSClient IsNot Nothing AndAlso mAPNSClient.Connected Then

            If msg.DN.SendID.Length = 64 Then
              mAPNSStream.Write(msg.GetPayload)
            Else
              msg.DN.SetResult("Invalid SendID Length")
            End If
            msg.Pending = False

          End If

        Next
        mMessageList.SetResponse(mMessageList.Count, "")

        System.Threading.Thread.Sleep(3000)

        Disconnect()

      End If

    End Sub

    Private Shared ReadOnly Property IsTest As Boolean
      Get
        Return Debugger.IsAttached OrElse AlwaysUseTest
      End Get
    End Property

    Public Shared ReadOnly Property CertificatePath As String
      Get
        Return If(IsTest, CertificatePath_Development, CertificatePath_Production)
      End Get
    End Property

    Private Sub LoadCertificate()
      If mCertificate Is Nothing Then
        If CertificatePassword = "" Then
          mCertificate = New X509Certificate2(CertificatePath)
        Else
          mCertificate = New X509Certificate2(CertificatePath, CertificatePassword)
        End If

        mCertificates = New X509CertificateCollection({mCertificate})
        mServerURL = If(IsTest, "gateway.sandbox.push.apple.com", "gateway.push.apple.com")
        mFeedbackURL = If(IsTest, "feedback.sandbox.push.apple.com", "feedback.push.apple.com")
      End If
    End Sub

    Private Enum Service
      PushNotifications
      FeedBackService
    End Enum

    Private Function Connect(Service As Service) As Boolean

      LoadCertificate()

      Try
        mAPNSClient = New TcpClient
        mAPNSClient.Connect(If(Service = AppleSender.Service.PushNotifications, mServerURL, mFeedbackURL),
                            If(Service = AppleSender.Service.PushNotifications, 2195, 2196))

        mAPNSStream = New SslStream(mAPNSClient.GetStream, False, Function()
                                                                    Return True
                                                                  End Function,
                                                                  Function()
                                                                    Return mCertificate
                                                                  End Function)
        mAPNSStream.AuthenticateAsClient(mServerURL, mCertificates, System.Security.Authentication.SslProtocols.Tls, False)
        If mAPNSStream.IsMutuallyAuthenticated AndAlso mAPNSStream.CanWrite Then
          mConnectionOpen = True

          If Service = AppleSender.Service.PushNotifications Then
            Dim Response(5) As Byte
            mAPNSStream.BeginRead(Response, 0, 6, AddressOf ReadResponse, Response)
          End If
          Return True

        Else
          MarkAllMessagesFailed("Invalid SSL Certificate")
        End If

      Catch ex As Exception
        MarkAllMessagesFailed("Connection Error: " & ex.Message)
      End Try

      Return False
    End Function

    Private Sub MarkAllMessagesFailed(Message As String)
      For Each msg As Message In mMessageList
        msg.DN.SetResult(Message)
        msg.Pending = False
      Next
      Disconnect()
    End Sub

    Public Enum Status
      NoError = 0
      ProcessingError = 1
      Missingdevicetoken = 2
      Missingtopic = 3
      Missingpayload = 4
      Invalidtokensize = 5
      Invalidtopicsize = 6
      Invalidpayloadsize = 7
      Invalidtoken = 8
      Shutdown = 9
      unknown = 10
    End Enum

    Private Sub ReadResponse(ar As IAsyncResult)

      If mConnectionOpen Then

        Dim Result As Byte() = ar.AsyncState
        Dim SequenceID As Integer
        mAPNSStream.ReadTimeout = 100
        If mAPNSStream.CanRead Then

          Dim Status As Int16 = Convert.ToInt16(Result(1))
          Dim PayloadBytes(3) As Byte
          Array.Copy(Result, 2, PayloadBytes, 0, 4)
          SequenceID = BitConverter.ToInt32(PayloadBytes, 0)

          mMessageList.SetResponse(SequenceID, [Enum].GetName(GetType(Status), Status))

        End If
      End If

    End Sub

    Private Sub Disconnect()
      Try
        If mAPNSClient IsNot Nothing Then
          mConnectionOpen = False
          mAPNSClient.Close()
          If mAPNSStream IsNot Nothing Then
            mAPNSStream.Close()
            mAPNSStream.Dispose()
            mAPNSStream = Nothing
          End If
          mAPNSClient = Nothing

        End If
      Catch ex As Exception

      End Try
    End Sub

    Private Sub ReadFeedBack(MessageList As MessageList)

      If Connect(Service.FeedBackService) Then

        'Set up
        Dim buffer(37) As Byte
        Dim recd As Integer = 0
        'Dim minTimestamp = DateTime.Now.AddYears(-1)

        'Get the first feedback
        recd = mAPNSStream.Read(buffer, 0, buffer.Length)

        Dim OldDeviceIDs As New List(Of String)

        'Continue while we have results and are not disposing
        While recd > 0

          'Get our seconds since 1970 ?
          Dim Seconds(3) As Byte
          Dim DeviceToken(31) As Byte

          Array.Copy(buffer, 0, Seconds, 0, 4)

          'Check endianness
          If BitConverter.IsLittleEndian Then
            Array.Reverse(Seconds)
          End If

          Dim tSeconds = BitConverter.ToInt32(Seconds, 0)

          'Add seconds since 1970 to that date, in UTC and then get it locally
          'fb.Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(tSeconds).ToLocalTime();


          'Now copy out the device token
          Array.Copy(buffer, 6, DeviceToken, 0, 32)

          Dim Token As String = BitConverter.ToString(DeviceToken).Trim()
          If Token.Length = 64 Then
            OldDeviceIDs.Add(Token)
          End If

          'Clear our array to reuse it
          Array.Clear(buffer, 0, buffer.Length)

          'Read the next feedback
          recd = mAPNSStream.Read(buffer, 0, buffer.Length)
        End While

        Disconnect()

        Dim dnl As DeviceNotificationList = DeviceNotificationList.NewDeviceNotificationList

        For Each token As String In OldDeviceIDs
          Dim Found As Boolean = False
          For Each msg As Message In MessageList
            If msg.DN.SendID = token Then
              msg.DN.SetResult("InvalidRegistration") 'same as androids 'invalid device' result.
              msg.Pending = False
              Found = True
            End If
          Next
          If Not Found Then
            Dim dn = dnl.AddNew
            dn.SendID = token
            dn.SetResult("InvalidRegistration")
          End If
        Next

        dnl.Save()

      End If

    End Sub

  End Class

End Namespace


