
Namespace Service

  Public Module ServiceModule

    Public Const ServiceUpdateQueueName As String = "ServiceUpdateQueue"

    ''' <summary>
    ''' Enable if changes to settings / commondata / service info from the main app must notify the service.
    ''' </summary>
    Public Property ServiceUpdateMode As ServiceUpdateOption = ServiceUpdateOption.NotEnabled

    ''' <summary>
    ''' True if the current application is a service. Will be set by Servicebase.
    ''' </summary>
    Public Property IsService As Boolean = False

    Public Property GetDefaultInfoFunction As Func(Of Integer, String, IServerProgramInfo)

    Public Enum ServiceUpdateMessageType
      ServiceInfoUpdated = 1
      SystemSettingsUpdated = 2
      CommonDataType = 3
      CommonDataName = 4
      CommonDataAll = 5
      ServiceMessage = 6
    End Enum

    Public Enum ServiceUpdateOption
      NotEnabled = 0
      ''' <summary>
      ''' Uses the ServiceUpdateService and ServiceUpdateQueue to watch for messages.
      ''' </summary>
      UseQueue = 1
      ''' <summary>
      ''' Uses the ServerProgramMessages table to poll for messages.
      ''' </summary>
      UseMessagesTable = 2
    End Enum

#If SILVERLIGHT Then

#Else

    Public Sub NotifyService(MessageType As ServiceUpdateMessageType, Message As String, Optional IgnoreEnableSetting As Boolean = False)
      If (ServiceUpdateMode <> ServiceUpdateOption.NotEnabled OrElse IgnoreEnableSetting) AndAlso Not IsService Then
        Singular.CommandProc.RunCommand("CmdProcs.cmdServerProgramMessage", {"@MessageType", "@Message"}, {MessageType, Message})
      End If
    End Sub

#End If

  End Module

End Namespace
