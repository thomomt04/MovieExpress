Namespace SmsSending

  Public Enum SMSProviderType
    Clickatell = 1
    SMSWarehouse = 2
    Vodacom = 3
    CellFind = 4
    BulkSMS = 5
  End Enum

  Public Enum ClickatellCallbackStatusDetailLevel
    NoStatuses = 0
    IntermediateStatuses = 1
    FinalStatuses = 2
    IntermediateAndFinalStatuses = 3
    ErrorStatuses = 4
    IntermediateAndErrorStatuses = 5
    FinalAndErrorStatuses = 6
    IntermediateFinalAndErrorStatuses = 7
  End Enum

  Public Class SmsResult
    Public Property Sent As Boolean
    Public Property ErrorMessage As String
    Public Property MessageID As String

    Public Sub New(Sent As Boolean, ErrorMessage As String, Optional MessageID As String = "")
      Me.Sent = Sent
      Me.ErrorMessage = ErrorMessage
      Me.MessageID = MessageID
    End Sub
  End Class

  Public MustInherit Class SMSSettings

    Protected mUserName As String = ""
    Protected mPassword As String = ""

    Public Property UserName() As String
      Get
        Return mUserName
      End Get
      Set(ByVal value As String)
        mUserName = value
      End Set
    End Property

    Public Property Password() As String
      Get
        Return mPassword
      End Get
      Set(ByVal value As String)
        mPassword = value
      End Set
    End Property

  End Class

  Public Class ClickatellSettings
    Inherits SMSSettings

    Private mMO As Integer = 0
    Private mFrom As String = ""
    Private mApiID As Integer = 0
    Private mCallbackURL As String = ""
    Private mCallbackStatusDetailLevel As ClickatellCallbackStatusDetailLevel = ClickatellCallbackStatusDetailLevel.NoStatuses

    Public Property ApiID() As Integer
      Get
        Return mApiID
      End Get
      Set(ByVal value As Integer)
        mApiID = value
      End Set
    End Property

    Public Property MO() As Integer
      Get
        Return mMO
      End Get
      Set(ByVal value As Integer)
        mMO = value
      End Set
    End Property

    Public Property From() As String
      Get
        Return mFrom
      End Get
      Set(ByVal value As String)
        mFrom = value
      End Set
    End Property

    Private mSessionTimeMinutes As Integer = 14
    Public Property SessionTimeMinutes() As Integer
      Get
        Return mSessionTimeMinutes
      End Get
      Set(ByVal value As Integer)
        mSessionTimeMinutes = value
      End Set
    End Property

    Public Property CallbackStatusDetailLevel() As ClickatellCallbackStatusDetailLevel
      Get
        Return mCallbackStatusDetailLevel
      End Get
      Set(ByVal value As ClickatellCallbackStatusDetailLevel)
        mCallbackStatusDetailLevel = value
      End Set
    End Property

  End Class

  Public Class SMSWarehouseSettings
    Inherits SMSSettings

    Private mSendURL As String = "http://212.7.192.140:7800/websms/websms?"
    Private mSenderID As String = ""

    Public Property SendURL() As String
      Get
        Return mSendURL
      End Get
      Set(ByVal value As String)
        mSendURL = value
      End Set
    End Property

    Public Property SenderID() As String
      Get
        Return mSenderID
      End Get
      Set(ByVal value As String)
        mSenderID = value
      End Set
    End Property

  End Class

  Public Class VodacomSettings
    Inherits SMSSettings

    Private mSendURL As String = "https://www.xml2sms.gsm.co.za/send?"

    Public Property SendURL() As String
      Get
        Return mSendURL
      End Get
      Set(ByVal value As String)
        mSendURL = value
      End Set
    End Property

  End Class

  Public Class BasicSettings
    Inherits SMSSettings

  End Class

  <Serializable()> _
  Public Class SmsSender

    Private Shared mSMSProvider As SMSProviderType = SMSProviderType.Clickatell
    Private Shared mSettings As SMSSettings = New ClickatellSettings

    Public Shared Property Settings() As SMSSettings
      Get
        Return mSettings
      End Get
      Set(value As SMSSettings)
        If Not mSettings.Equals(value) Then
          mSettings = value
        End If
      End Set
    End Property

    Public Shared Property SMSProvider As SMSProviderType
      Get
        Return mSMSProvider
      End Get
      Set(value As SMSProviderType)
        If value <> mSMSProvider Then
          mSMSProvider = value
          Select Case mSMSProvider
            Case SMSProviderType.Clickatell
              mSettings = New ClickatellSettings
            Case SMSProviderType.SMSWarehouse
              mSettings = New SMSWarehouseSettings
            Case SMSProviderType.Vodacom
              mSettings = New VodacomSettings
            Case SMSProviderType.CellFind, SMSProviderType.BulkSMS
              mSettings = New BasicSettings

          End Select
        End If
      End Set
    End Property

    Public Shared Sub SetSettings(Settings As Singular.Correspondence.ISmsSettings)
      SMSProvider = Settings.SmsProvider
      mSettings.UserName = Settings.SmsUserName
      mSettings.Password = Settings.SmsPassword
      If Settings.SmsProvider = SMSProviderType.Clickatell Then
        CType(mSettings, ClickatellSettings).ApiID = Settings.SmsAPIKey
      End If
    End Sub

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


      If Not String.IsNullOrEmpty(AllowedNumbers) Then
        'Replace addresses
        For i As Integer = 0 To ToNumbers.Length - 1
          ToNumbers(i) = GetSafeNumber(ToNumbers(i))
        Next

      End If


      Select Case SMSProvider
        Case SMSProviderType.Clickatell
          Return ClickatellSender.SendSms(ToNumbers, Message)
        Case SMSProviderType.SMSWarehouse
          Return SMSWarehouseSender.SendSms(ToNumbers, Message)
        Case SMSProviderType.Vodacom
          If ToNumbers.Length > 1 Then
            Throw New Exception("Vodacom does not support sending messages to multiple numbers.")
          End If
          Return VodacomSender.SendSms(ToNumbers(0), Message)
        Case SMSProviderType.CellFind
          If ToNumbers.Length > 1 Then
            Throw New Exception("CellFind does not support sending messages to multiple numbers.")
          End If
          Return CellFindInterface.SendSms(ToNumbers(0), Message)
        Case SMSProviderType.BulkSMS
          Return BulkSMSSender.SendSms(ToNumbers, Message)
        Case Else
          Throw New Exception("Unknown Sms Service Provider")
      End Select

    End Function

    ''' <summary>
    ''' Sends and SMS to a single number. Settings must be set before calling this method.
    ''' </summary>
    ''' <param name="ToNumber"></param>
    ''' <param name="Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SendSmsWithMessageID(ByVal ToNumber As String, ByVal Message As String, RecipientID As Integer) As SmsResult

      Select Case SMSProvider
        Case SMSProviderType.Clickatell
          Return ClickatellSender.SendSmsWithMessageID(ToNumber, Message, RecipientID)
        Case Else
          Throw New Exception("Unknown Sms Service Provider")
      End Select

      'Return SendSmsSober(New String() {ToNumber}, Message)

    End Function

#Region " Allowed Numbers "

    ''' <summary>
    ''' If not empty, the library will check if the cell no is in this list. If not, the numer will be changed to the first address in this list.
    ''' Seperate with ;
    ''' </summary>
    Public Shared Property AllowedNumbers As String
      Get
        Return mAllowedNumbers
      End Get
      Set(value As String)
        mAllowedNumbers = value

        mAllowedNumbersHT = New Dictionary(Of String, Boolean)
        If Not String.IsNullOrEmpty(mAllowedNumbers) Then
          Dim First As Boolean = True
          For Each address In mAllowedNumbers.Split({";"}, StringSplitOptions.RemoveEmptyEntries)
            mAllowedNumbersHT.Add(address.Trim.ToLower, True)
            If First Then
              First = False
              mOverrideNumber = address.Trim.ToLower
            End If
          Next
        End If
      End Set
    End Property

    Private Shared mAllowedNumbers As String
    Private Shared mAllowedNumbersHT As Dictionary(Of String, Boolean)
    Private Shared mOverrideNumber As String

    Public Shared Function GetSafeNumber(Number As String) As String
      If mAllowedNumbersHT.ContainsKey(Number.Trim.ToLower) Then
        Return Number
      Else
        Return mOverrideNumber
      End If
    End Function

#End Region

  End Class

End Namespace
