Imports System.Net.Sockets
Imports System.Net
Imports System.Text

Namespace Networking

  Public Class MessageSender

    Private Shared Function ConnectSocket(ByVal Server As String, ByVal Port As Integer) As Socket

      Dim S As Socket = Nothing
      Dim hostEntry As IPHostEntry = Nothing

      ' Get host related information.
      hostEntry = Dns.GetHostEntry(Server)

      For Each address As IPAddress In hostEntry.AddressList
        Dim ipe As New IPEndPoint(address, Port)
        Dim tempSocket As New Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
        tempSocket.Connect(ipe)

        If tempSocket.Connected Then
          S = tempSocket
          Exit For
        Else
          Continue For
        End If
      Next

      Return S

    End Function


    Public Shared Function SendMessage(ByVal IPAddress As String, ByVal PortNumber As Integer, ByVal Message As Object, ByVal ResponseHandler As ResponseHandler) As Object

      Dim tcpClient As New System.Net.Sockets.TcpClient()
      tcpClient.Connect(IPAddress, PortNumber)
      Dim networkStream As NetworkStream = tcpClient.GetStream()

      If networkStream.CanWrite Then
        ' Do a simple write.
        Dim ms As New System.IO.MemoryStream()
        Dim f As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        f.Serialize(ms, Message)
        Dim sendBytes As Byte() = ms.ToArray()
        networkStream.Write(sendBytes, 0, sendBytes.Length)

        If ResponseHandler IsNot Nothing Then
          If networkStream.CanRead Then

            Dim bwt As New System.ComponentModel.BackgroundWorker()
            AddHandler bwt.DoWork, AddressOf WaitForResponse
            bwt.RunWorkerAsync(New WaitForResponseArgs(tcpClient, networkStream, ResponseHandler))
          Else
            tcpClient.Close()
            Throw New Exception("Cannot not read data from Server " & IPAddress & ": " & PortNumber)
          End If
        End If
      Else
        tcpClient.Close()
        Throw New Exception("Cannot not write data to Server " & IPAddress & ": " & PortNumber)
      End If

      Return Nothing

    End Function

    Public Shared Function SendMessage(ByVal IPAddress As String, ByVal PortNumber As Integer, ByVal Message As Object) As Object

      Dim tcpClient As New System.Net.Sockets.TcpClient()
      tcpClient.Connect(IPAddress, PortNumber)
      Dim networkStream As NetworkStream = tcpClient.GetStream()

      Dim ReturnMessage As Object = Nothing

      If networkStream.CanWrite Then
        ' Do a simple write.
        Dim ms As New System.IO.MemoryStream()
        Dim f As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        f.Serialize(ms, Message)
        Dim sendBytes As Byte() = ms.ToArray()
        networkStream.Write(sendBytes, 0, sendBytes.Length)

        If networkStream.CanRead Then

          ' WaitForResponseHandler
          ' Read the NetworkStream into a byte buffer.
          Dim bytes(tcpClient.ReceiveBufferSize) As Byte
          networkStream.Read(bytes, 0, CInt(tcpClient.ReceiveBufferSize))
          tcpClient.Close()

          ms = New IO.MemoryStream(CType(bytes, Array))
          f = New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
          ms.Position = 0
          ReturnMessage = f.Deserialize(ms)
        Else
          tcpClient.Close()
          Throw New Exception("Cannot not read data from Server " & IPAddress & ": " & PortNumber)
        End If

        tcpClient.Close()
      Else
        tcpClient.Close()
        Throw New Exception("Cannot not write data to Server " & IPAddress & ": " & PortNumber)
      End If

      Return ReturnMessage

    End Function

    Public Delegate Sub ResponseHandler(ByVal Response As ResponseArgs)

    <Serializable()> _
    Public Class ResponseArgs

      Public Exception As Exception
      Public Message As Object

      Public Sub New(ByVal Message As Object)

        Me.Message = Message

      End Sub

      Public Sub New(ByVal Exception As Exception)

        Me.Exception = Exception

      End Sub

    End Class

    <Serializable()> _
    Private Class WaitForResponseArgs

      Public TcpClient As TcpClient
      Public NetworkStream As NetworkStream
      Public ResponseHandler As ResponseHandler

      Public Sub New(ByVal TcpClient As TcpClient, ByVal NetworkStream As NetworkStream, ByVal ResponseHandler As ResponseHandler)

        Me.TcpClient = TcpClient
        Me.NetworkStream = NetworkStream
        Me.ResponseHandler = ResponseHandler

      End Sub

    End Class

    Private Shared Sub WaitForResponse(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)

      Dim Args As WaitForResponseArgs = e.Argument

      ' WaitForResponseHandler
      ' Read the NetworkStream into a byte buffer.
      Dim bytes(Args.TcpClient.ReceiveBufferSize) As Byte
      Args.NetworkStream.Read(bytes, 0, CInt(Args.TcpClient.ReceiveBufferSize))
      Args.TcpClient.Close()

      Dim ms As New IO.MemoryStream(CType(bytes, Array))
      Dim f As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
      Try
        ' Dim obj As Object = f.Deserialize(ms)
        Dim s As String = GetStringFromMemoryStream(ms)
        Args.ResponseHandler.Invoke(New ResponseArgs(s))
      Catch ex As Exception
        Args.ResponseHandler.Invoke(New ResponseArgs(New Exception("Error deserialising stream", ex)))
      End Try

      Args.TcpClient.Close()

    End Sub

    Public Shared Function GetStringFromMemoryStream(ByVal m As IO.MemoryStream) As String
      If m Is Nothing OrElse m.Length = 0 Then Return ""

      m.Flush()
      m.Position = 0
      Dim sr As IO.StreamReader = New IO.StreamReader(m)
      Dim s As String = sr.ReadToEnd()
      Return s

    End Function



    'public static MemoryStream GetMemoryStreamFromString(string s)
    '{
    '  if (s == null || s.Length == 0)
    '    return null;

    '  MemoryStream m = new MemoryStream();
    '  StreamWriter sw = new StreamWriter(m);
    '  sw.Write(s);
    '  sw.Flush();

    '  return m;
    '}

  End Class

End Namespace
