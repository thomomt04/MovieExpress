
Public MustInherit Class CorrespondenceQueueBase(Of SettingsType)
  Inherits QueueServiceBase

  Private mServiceID As Integer

  Public Sub New(ServiceID As Integer, QueueName As String, ServiceName As String)
    MyBase.New(QueueName, ServiceName)

    mServiceID = ServiceID

  End Sub

  Public Overrides ReadOnly Property ServerProgramTypeID As Integer
    Get
      Return mServiceID
    End Get
  End Property

  Public Overrides Sub Start()

    If SetSettings() Then
      WriteProgress(Name & " Started.")
      MyBase.Start()
    Else
      WriteProgress(Name & " cannot start due to missing settings.")
    End If

  End Sub

  Protected Overrides Sub RunEvent(message As String)

    If _HasSettings Then
      SendCorrespondence()
    End If

  End Sub

  Private _HasSettings As Boolean

  Protected Function SetSettings() As Boolean
    Dim Settings = GetSettings()
    If Not HasSettings(Settings) Then
      _HasSettings = False

    Else

      SetupSender(Settings)
      _HasSettings = True
    End If

    Return _HasSettings
  End Function

  Protected MustOverride Function HasSettings(Settings As SettingsType) As Boolean
  Protected MustOverride Function GetSettings() As SettingsType
  Protected MustOverride Sub SetupSender(Settings As SettingsType)
  Protected MustOverride Sub SendCorrespondence()

End Class
