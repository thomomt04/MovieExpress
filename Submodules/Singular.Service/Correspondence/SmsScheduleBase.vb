Public Class SmsScheduleBase
  Inherits ScheduleProgramBase

  Public Sub New(ByVal DatabaseID As Integer)
    MyBase.New(DatabaseID, "SMS Scheduler")
  End Sub

  Public Sub New(ByVal DatabaseID As Integer, ByVal ClickATellCredentials As Singular.SmsSending.ClickatellSettings)
    MyBase.New(DatabaseID, "SMS Scheduler")

    Singular.SmsSending.SmsSender.Settings = ClickATellCredentials
  End Sub

  Protected Overrides Function StartSchedule() As Boolean

    If AppID = 0 AndAlso SMSProvider = SmsSending.SMSProviderType.Clickatell Then
      WriteProgress("Incorrect Settings.")
      Return False
    Else

      SmsSending.SmsSender.SMSProvider = SMSProvider

      Select Case SMSProvider
        Case SmsSending.SMSProviderType.Clickatell
          Dim Settings As SmsSending.ClickatellSettings = SmsSending.SmsSender.Settings
          Settings.ApiID = AppID
          Settings.UserName = UserName
          Settings.Password = Password
          Settings.From = [From]
          Settings.MO = MO

        Case SmsSending.SMSProviderType.SMSWarehouse
          Dim Settings As SmsSending.SMSWarehouseSettings = SmsSending.SmsSender.Settings
          Settings.UserName = UserName
          Settings.Password = Password
          Settings.SenderID = SenderID

        Case SmsSending.SMSProviderType.Vodacom
          Dim Settings As SmsSending.VodacomSettings = SmsSending.SmsSender.Settings
          Settings.UserName = UserName
          Settings.Password = Password

        Case SmsSending.SMSProviderType.CellFind
          SmsSending.SmsSender.Settings.UserName = UserName
          SmsSending.SmsSender.Settings.Password = Password

      End Select


      WriteProgress("SMS Sender started.")
      Return True
    End If

  End Function

  Protected Overrides Function StopSchedule() As Boolean
    WriteProgress("SMS Sender stopped.")
    Return True
  End Function

  Protected Overrides Sub TimeUp()

    Dim SMSList As SmsSending.SmsList

    Try
      SMSList = SmsSending.SmsList.GetUnsentSmsList

      If SMSList.Count > 0 Then
        For Each sms As SmsSending.Sms In SMSList
          sms.Send()
        Next

        SMSList.Save()
        Dim fCount As Integer = SMSList.FailedCount
        Dim count As Integer = SMSList.Count - fCount
        If fCount > 0 Then
          WriteProgress(count & " Smses Sent. " & fCount & " Smses failed to Send.")
        Else
          WriteProgress(count & " Smses Sent.")
        End If
      Else
        WriteProgress("No Smses to Send")
      End If

    Catch ex As Exception
      WriteProgress("Error Sending Smses: " & ex.Message)
    End Try

  End Sub

  Protected Overridable ReadOnly Property AppID As Integer
    Get
      Return 0 'Singular.SmsSending.SmsSender.Settings.AppID
    End Get
  End Property

  Protected Overridable ReadOnly Property UserName As String
    Get
      Return Singular.SmsSending.SmsSender.Settings.UserName
    End Get
  End Property

  Protected Overridable ReadOnly Property Password As String
    Get
      Return Singular.SmsSending.SmsSender.Settings.Password
    End Get
  End Property

  Public Overridable ReadOnly Property SMSProvider As Singular.SmsSending.SMSProviderType
    Get
      Return SmsSending.SMSProviderType.Clickatell
    End Get
  End Property

  ''' <summary>
  ''' Required Only for SMS Warehouse.
  ''' </summary>
  Protected Overridable ReadOnly Property SenderID As String
    Get
      Return "SS"
    End Get
  End Property

  Protected Overridable ReadOnly Property MO As Integer
    Get
      Return 1
    End Get
  End Property

  Protected Overridable ReadOnly Property From As String
    Get
      Return ""
    End Get
  End Property

End Class
