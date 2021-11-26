Imports System.Net.Sockets

Namespace Networking

  Public MustInherit Class PortServiceBase
    Inherits Networking.NetworkServiceBase

    Private mPortListenerType As Type
    Protected mPortListenerTypeConstructorParams As Object()

    Private mPortListenerThread As System.Threading.Thread

    Public Overrides Sub Start()

      StartPortListenerThread()

    End Sub

    Public Overrides Sub [Stop]()

      mPortListenerThread.Abort()

    End Sub

    Private mPortMessageProcessor As PortMessageProcessorBase

    Private Sub StartPortListenerThread()

      Try
        If mPortListenerTypeConstructorParams Is Nothing Then
          WriteProgress(Singular.Strings.Readable(mPortListenerType.Name) & " Starting without parameters", ProgressType.Success)
          mPortMessageProcessor = Activator.CreateInstance(mPortListenerType)
        Else
          WriteProgress(Singular.Strings.Readable(mPortListenerType.Name) & " Starting with parameters", ProgressType.Success)
          mPortMessageProcessor = Activator.CreateInstance(mPortListenerType, mPortListenerTypeConstructorParams)
        End If
        Try
          WriteProgress(Singular.Strings.Readable(mPortListenerType.Name) & " Created - Address: " & mPortMessageProcessor.IPAddress.ToString() & ": " & mPortMessageProcessor.PortNumber & " ...", ProgressType.Success)
        Catch ex As Exception

        End Try
        mPortListenerThread = New System.Threading.Thread(AddressOf ListenForClients)
        mPortListenerThread.Start()
      Catch ex As Exception
        Me.WriteProgress("Error starting Port Listener " & mPortListenerType.Name & ": " & ex.Message, ProgressType.Failure)
      End Try

    End Sub

    Public Sub ListenForClients()

      Dim TcpListener As TcpListener = Nothing
      Try
        TcpListener = New TcpListener(Net.IPAddress.Any, mPortMessageProcessor.PortNumber)
        'TcpListener = New TcpListener(Net.IPAddress.Any, 19551)
        TcpListener.Start()
      Catch ex As Exception
        Me.WriteProgress("Error creating TCP Listener.  " & Singular.Debug.RecurseExceptionMessage(ex), ProgressType.Failure)
        Exit Sub
      End Try

      Try

        While (True)
          'Accept the pending client connection and return a TcpClient initialized for communication. 
          Me.WriteProgress("Waiting to accept message on " & Singular.Strings.Readable(mPortListenerType.Name) & "...", ProgressType.Success)
          Dim TcpClient As TcpClient = TcpListener.AcceptTcpClient()

          Try
            Dim bwt As New System.ComponentModel.BackgroundWorker()
            bwt.WorkerReportsProgress = True
            AddHandler bwt.ProgressChanged, AddressOf PortListenerProgress
            AddHandler bwt.DoWork, AddressOf ProcessMessageBase
            bwt.RunWorkerAsync(TcpClient)
          Catch ex As Exception
            Me.WriteProgress("Error starting message processing: " & Singular.Debug.RecurseExceptionMessage(ex), ProgressType.Failure)
          End Try
        End While

      Finally
        TcpListener.Stop()
      End Try

    End Sub

    Friend Sub ProcessMessageBase(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)

      Dim TcpClient As TcpClient = e.Argument

      Try
        ' Get the stream
        Dim NetworkStream As NetworkStream = TcpClient.GetStream()

        Dim bytes(TcpClient.ReceiveBufferSize) As Byte

        Try
          NetworkStream.Read(bytes, 0, CInt(TcpClient.ReceiveBufferSize))
          Me.WriteProgress("Message Recieved on " & Singular.Strings.Readable(mPortListenerType.Name) & "...", ProgressType.Success)
        Catch ex As Exception
          ReportProgress(sender, 1, ListenerStateType.Fault, "Error reading network stream: " & ex.Message)
          Exit Sub
        End Try

        Try
          mPortMessageProcessor.ProcessMessage(sender, TcpClient, bytes)
          Me.WriteProgress(Singular.Strings.Readable(mPortListenerType.Name) & " Message Processed Successfuly", ProgressType.Success)
        Catch ex As Exception
          Me.WriteProgress(Singular.Strings.Readable(mPortListenerType.Name) & " Error Processing Message: " & Singular.Debug.RecurseExceptionMessage(ex), ProgressType.Failure)
        End Try
      Catch ex As Exception
        Me.WriteProgress(Singular.Strings.Readable(mPortListenerType.Name) & " Error Processing Message: " & Singular.Debug.RecurseExceptionMessage(ex), ProgressType.Failure)
      Finally
        TcpClient.Close()
      End Try

    End Sub

    Public Enum ListenerStateType
      Listening
      Initialising
      MessageRecieved
      ProcessingMessage
      CompletedSuccess
      Fault
    End Enum

    <Serializable()> _
    Public Class ListenerState

      Private mListenerStateType As ListenerStateType = ListenerStateType.Listening

      Public ReadOnly Property Progress() As String
        Get
          If mProgressString = "" Then
            Return Singular.CommonData.Enums.Description(mListenerStateType)
          Else
            Return Singular.CommonData.Enums.Description(mListenerStateType) & " - " & mProgressString
          End If
        End Get
      End Property

      Public ReadOnly Property ListenerStateType() As ListenerStateType
        Get
          Return mListenerStateType
        End Get
      End Property

      Public Sub New(ByVal ListenerStateType As ListenerStateType)

        mListenerStateType = ListenerStateType

      End Sub

      Private mProgressString As String = ""

      Public Sub New(ByVal ListenerStateType As ListenerStateType, ByVal ProgressString As String)

        mListenerStateType = ListenerStateType
        mProgressString = ProgressString

      End Sub


    End Class

    Public Shared Sub ReportProgress(ByVal WorkerThread As System.ComponentModel.BackgroundWorker, ByVal PercentageProgress As Integer, ByVal ListenerStateType As ListenerStateType)

      WorkerThread.ReportProgress(PercentageProgress, New ListenerState(ListenerStateType))

    End Sub

    Public Shared Sub ReportProgress(ByVal WorkerThread As System.ComponentModel.BackgroundWorker, ByVal PercentageProgress As Integer, ByVal ListenerStateType As ListenerStateType, ByVal ProgressString As String)

      WorkerThread.ReportProgress(PercentageProgress, New ListenerState(ListenerStateType, ProgressString))

    End Sub

    Private Sub PortListenerProgress(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs)

      Dim ListenerState As ListenerState = e.UserState
      Me.WriteProgress(ListenerState.Progress, ProgressType.Success)

    End Sub

    Private Sub PortServerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)

      Me.WriteProgress(Singular.Strings.Readable(mPortListenerType.Name) & " Completed...", ProgressType.Success)

    End Sub

    Public Sub New(ByVal Name As String, ByVal MessageProcessorType As Type, ByVal PortListenerTypeConstructorParams As Object())

      MyBase.New(Name)
      mPortListenerType = MessageProcessorType
      mPortListenerTypeConstructorParams = PortListenerTypeConstructorParams

    End Sub

  End Class

End Namespace
