Imports System.Net.Sockets
Imports System.Text

Namespace Networking

  Public MustInherit Class PortMessageProcessorBase

    'Protected mVersion As String = "Not Set"
    'Protected mServerProgramTypeID As Integer

    Public Overridable Sub ReportProgress(ByVal WorkerThread As System.ComponentModel.BackgroundWorker, ByVal PercentageProgress As Integer, ByVal ListenerStateType As PortServiceBase.ListenerStateType)

      PortServiceBase.ReportProgress(WorkerThread, PercentageProgress, ListenerStateType)

    End Sub

    Public Overridable Sub ReportProgress(ByVal WorkerThread As System.ComponentModel.BackgroundWorker, ByVal PercentageProgress As Integer, ByVal ListenerStateType As PortServiceBase.ListenerStateType, ByVal ProgressString As String)

      PortServiceBase.ReportProgress(WorkerThread, PercentageProgress, ListenerStateType, ProgressString)

    End Sub

    Public MustOverride ReadOnly Property PortNumber() As Integer

    Public MustOverride ReadOnly Property IPAddress() As System.Net.IPAddress

    Public ReadOnly Property Address() As String
      Get
        Return Me.IPAddress.ToString & ": " & Me.PortNumber
      End Get
    End Property

    Public Sub SendReponse(ByVal TcpClient As TcpClient, ByVal ResponseMessage As Object)

      Dim ms As New System.IO.MemoryStream()
      Dim f As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
      f.Serialize(ms, ResponseMessage)
      'ms.Seek(0, System.IO.SeekOrigin.Begin) 'DALE

      SendReponse(TcpClient, ms.ToArray())

    End Sub

    Public Sub SendReponse(ByVal TcpClient As TcpClient, ByVal ResponseMessage As Byte())

      TcpClient.GetStream.Write(ResponseMessage, 0, ResponseMessage.Length)

    End Sub

    Public MustOverride Sub ProcessMessage(ByVal WorkerThread As System.ComponentModel.BackgroundWorker, ByVal TcpClient As TcpClient, ByVal Message As Byte())

  End Class

End Namespace
