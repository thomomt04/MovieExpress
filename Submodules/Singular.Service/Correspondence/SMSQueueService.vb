Public MustInherit Class SMSQueueService
  Inherits CorrespondenceQueueBase(Of Singular.Correspondence.ISmsSettings)

  Public Sub New(ServiceID As Integer, QueueName As String)
    MyBase.New(ServiceID, QueueName, "SMS Sender")
  End Sub

  Protected Overrides Sub SetupSender(Settings As Singular.Correspondence.ISmsSettings)
    Singular.SmsSending.SmsSender.SetSettings(Settings)
  End Sub

  Protected Overrides Function HasSettings(Settings As Correspondence.ISmsSettings) As Boolean
    Return Not String.IsNullOrEmpty(Settings.SmsUserName) AndAlso Not String.IsNullOrEmpty(Settings.SmsPassword)
  End Function

  ''' <summary>
  ''' If the sms list contains this amount of items or more, it will be refetched once all the smses have sent.
  ''' Use this if your get sms proc has a SELECT TOP x clause. The Threshold should be the same or less than the TOP value.
  ''' </summary>
  Protected Overridable ReadOnly Property ResendThreshold() As Integer
    Get
      Return Integer.MaxValue
    End Get
  End Property

  Protected Overrides Sub SendCorrespondence()

    Dim SMSList As Singular.SmsSending.SmsList = Nothing

    Do

      Try
        SMSList = Singular.SmsSending.SmsList.GetUnsentSmsList

        Try
          SMSList.Send()

          Try
            SMSList.Save()
          Catch ex As Exception
            'If the sms list fails to save, it means there is no way of marking that the email is sent.
            'If the service is not stopped, the same smses will continue to be sent over and over.
            _QR.Stop()
            WriteProgress("Error saving sms list", ex)
            Exit Do
          End Try

        Catch ex As Exception
          WriteProgress("Error sending smses", ex)
        End Try

      Catch ex As Exception
        WriteProgress("Error fetching sms list", ex)
      End Try

    Loop While SMSList.GetRecipientCount >= ResendThreshold

  End Sub

End Class
