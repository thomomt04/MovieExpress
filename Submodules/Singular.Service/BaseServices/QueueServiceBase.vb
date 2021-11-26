Public MustInherit Class QueueServiceBase
  Inherits Singular.Service.ServerProgramBase

  Private _QueueName As String
  Protected _QR As Singular.Data.SQLQueueReader

  Public Sub New(QueueName As String, ServiceName As String)
    MyBase.New(ServiceName)
    _QueueName = QueueName
  End Sub

  Public Overrides Sub Start()

    If ServerProgramType.ActiveInd Then
      Begin()
    End If

  End Sub

  Public Overrides Sub [Stop]()

    If _QR IsNot Nothing Then
      _QR.Stop()
    End If

  End Sub

  Protected Overridable Sub SetupReader(QueueReader As Singular.Data.SQLQueueReader)

  End Sub

  Private Sub Begin()

    'Run at startup
    RunEvent("")

    WriteProgress("Creating Queue Reader")

    'Begin listening on queue.
    _QR = New Singular.Data.SQLQueueReader(_QueueName)
    SetupReader(_QR)

    Try

      WriteProgress("Starting Queue Reader")

      _QR.BeginWaitForMessage(
             AddressOf RunEvent,
             Sub(ex)
               WriteProgress("Error in " & _QueueName & " queue", ex)
             End Sub)

    Catch ex As Exception
      WriteProgress("Error Starting Queue Reader", ex)
    End Try

  End Sub

  Protected MustOverride Sub RunEvent(message As String)

End Class
