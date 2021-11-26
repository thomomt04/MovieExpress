Namespace DevicePush

  Public Class DeviceMessageSender

    Private Shared SendLock As New Object

    Public Enum DeviceOS
      Android = 1
      iPhone = 2
      WinPhone = 3
    End Enum

    Private mCGMMessageList As New Hashtable
    Private mAppleSender As New AppleSender
    Private mErrors As String = ""

    ''' <summary>
    ''' Sends Messages to the device, and sets the sent date of the object. Does not save the object.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SendMessages(dnl As DeviceNotificationList)

      'Split into Android / iPhone etc.
      For Each dn As DeviceNotification In dnl

        Select Case CType(dn.DeviceOS, DeviceOS)
          Case DeviceOS.Android
            Dim Msg As GCMSender = mCGMMessageList(dn.Message)
            If Msg Is Nothing Then
              Msg = New GCMSender
              mCGMMessageList(dn.Message) = Msg
            End If
            Msg.AddSendID(dn)

          Case DeviceOS.iPhone
            mAppleSender.QueueMessage(dn)

          Case DeviceOS.WinPhone


        End Select

      Next

      'Send the messages
      'Google
      Try
        For Each s As Sender In mCGMMessageList.Values
          s.Send()
        Next
      Catch ex As Exception
        mErrors &= "Error sending notifications to gcm: " & Singular.Debug.RecurseExceptionMessage(ex) & vbCrLf
      End Try

      'Apple
      Try
        If AppleSender.CertificatePath <> "" AndAlso IO.File.Exists(AppleSender.CertificatePath) Then
          mAppleSender.SendQueuedMessages()
        End If
      Catch ex As Exception
        mErrors &= "Error sending notifications to Apple: " & Singular.Debug.RecurseExceptionMessage(ex) & vbCrLf
      End Try

    End Sub

    ''' <summary>
    ''' Fetches the notification list, sends the notifications, and saves them.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub BeginSendMessages(Optional CompleteCallBack As Action(Of String) = Nothing)

      Dim t As New System.Threading.Tasks.Task(
        Sub()

          SyncLock SendLock

            Dim dnl As Singular.DevicePush.DeviceNotificationList = Singular.DevicePush.DeviceNotificationList.GetDeviceNotificationList
            Dim dms As New Singular.DevicePush.DeviceMessageSender
            dms.SendMessages(dnl)

            dnl.Save()

          End SyncLock

        End Sub)

      t.Start()

    End Sub

  End Class

End Namespace



