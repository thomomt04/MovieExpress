Imports Csla
Imports System.IO
Imports Csla.Serialization

Public Class SingularDownloader

  Public Property DownloadPath As String = ""
  Public Property SavePath As String = ""
  Public Property ChunkSize As Integer = 250000
  Private Property FileSize As Integer = -1
  Private Property FileStream As FileStream = Nothing
  Private Property RestartDownload As Boolean = False
  Private Property OverwriteExisting As Boolean = False
  Private Property OriginalCreatedDate As Date

  Public Sub New(ByVal DownloadPath As String, ByVal SavePath As String, Optional OverwriteExisting As Boolean = False)

    'This code should use the following line:
    'System.Web.HttpContext.Current.Server.UrlDecode

    Me.OverwriteExisting = OverwriteExisting

    If DownloadPath.StartsWith("\\") Then
      Me.DownloadPath = "\" & DownloadPath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    Else
      Me.DownloadPath = DownloadPath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    End If
    If SavePath.StartsWith("\\") Then
      Me.SavePath = "\" & SavePath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    Else
      Me.SavePath = SavePath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    End If
    Me.SavePath = Me.SavePath & ".part"
  End Sub

  Public Sub GetFile()
    GetChunk(Me.DownloadPath, 0)
  End Sub

  Public Sub Restart()
    RestartDownload = True
    Try
      IO.File.Delete(Me.SavePath)
    Catch ex As Exception
      Throw ex
    End Try
    GetChunk(Me.DownloadPath, 0)
  End Sub

  Private Sub GetChunk(ByVal DownloadPath As String, ByVal ChunkNumber As Integer)
    Dim cmdDownload As cmdDownloadFileChunk
    Try
      cmdDownload = cmdDownloadFileChunk.NewcmdDownloadFileChunk(DownloadPath, ChunkSize, ChunkNumber)
    Catch ex As Exception
      Throw ex
    End Try
    cmdDownload.BeginExecute(AddressOf GetCheckCallBack)
  End Sub

  Private Sub GetCheckCallBack(sender As Object, e As Csla.DataPortalResult(Of cmdDownloadFileChunk))

    If e.Error IsNot Nothing Then
      Dim retry As Boolean = False
      RaiseEvent DownloadFileError(Me, New DownloadFileErrorEventArgs(e.Error.Message, retry, SavePath.Substring(0, SavePath.LastIndexOf("."))))
      DownloadCompleted(True, retry)
      Exit Sub
    ElseIf Not String.IsNullOrEmpty(e.Object.ErrorString) Then
      DownloadCompleted(True, False, e.Object.ErrorString)
      Exit Sub
    End If
    If CType(e.Object, cmdDownloadFileChunk).FileSize = 0 Then
      Dim retry As Boolean = False
      RaiseEvent DownloadFileError(Me, New DownloadFileErrorEventArgs("0 byte file size!", retry, SavePath.Substring(0, SavePath.LastIndexOf("."))))
      DownloadCompleted(True, retry, "0 byte file size!")
      Exit Sub
    End If
    Dim ChunkNumber As Integer = e.Object.ChunkNumber
    If RestartDownload AndAlso ChunkNumber > 0 Then
      Exit Sub
    Else
      RestartDownload = False
    End If
    If FileSize = -1 Then
      FileSize = CType(e.Object, cmdDownloadFileChunk).FileSize
    End If
    If FileSize <> CType(e.Object, cmdDownloadFileChunk).FileSize Then
      Dim retry As Boolean = False
      RaiseEvent DownloadFileError(Me, New DownloadFileErrorEventArgs("File Size Mismatch", retry, SavePath.Substring(0, SavePath.LastIndexOf("."))))
      DownloadCompleted(True, retry)
      Exit Sub
    End If
    Dim SaveDirectoryPath = SavePath.Substring(0, SavePath.LastIndexOf("\"))
    If FileStream Is Nothing Then
      FileStream = GetFileStream(SaveDirectoryPath)
    End If
    Dim str = WriteFileChunk(FileStream, CType(e.Object, cmdDownloadFileChunk).Chunk, ChunkNumber)
    If str <> "" Then
      FileStream.Close()
      FileStream = Nothing
      Dim Retry As Boolean = False
      RaiseEvent DownloadFileError(Me, New DownloadFileErrorEventArgs(str, Retry, SavePath.Substring(0, SavePath.LastIndexOf("."))))
      DownloadCompleted(True, Retry)
      Exit Sub
    End If
    Dim dp As Double = DownloadedPercent()
    Dim crea As ChunkRecievedEventArgs = New ChunkRecievedEventArgs(dp, False)
    RaiseEvent ChunkRecieved(Me, crea)
    If Not crea.Cancel Then
      If dp = 100 Then
        Try
          FileStream.Close()
          FileStream = File.Open(SavePath, FileMode.Open)
          If FileStream.Length <> FileSize Then
            'Error with file
            FileStream.Close()
            FileStream = Nothing
            Dim Retry As Boolean = False
            RaiseEvent DownloadFileError(Me, New DownloadFileErrorEventArgs("File Size Mismatch", Retry, SavePath))
            DownloadCompleted(True, Retry)
            Exit Sub
          Else
            FileStream.Close()
            FileStream = Nothing
            DownloadCompleted(False, False)
          End If
        Catch ex As Exception
          Throw ex
        End Try
      Else
        FileStream.Close()
        FileStream = Nothing
        GetChunk(DownloadPath, ChunkNumber + 1)
      End If
    Else
      FileStream.Close()
      FileStream = Nothing
      File.Delete(SavePath)
      RaiseEvent DownloadCanceled(Me, New DownloadCanceledEventArgs)
    End If
  End Sub

  Private Function WriteFileChunk(ByRef FileStream As FileStream, ByVal Chunk As Byte(), ByVal ChunkNumber As Integer) As String
    If FileStream.Length = ChunkNumber * ChunkSize Then
      Try
        FileStream.Write(Chunk, 0, Chunk.Length)
      Catch ex As Exception
        Throw ex
      End Try
      Return ""
    Else
      Return "Error: Mismatch of file part size and recieved chunks"
    End If
  End Function

  Private Function GetFileStream(SaveDirectoryPath As String) As FileStream
    If Not IO.Directory.Exists(SaveDirectoryPath) Then
      Try
        IO.Directory.CreateDirectory(SaveDirectoryPath)
      Catch ex As Exception
        Throw ex
      End Try
    End If
    If Not File.Exists(SavePath) Then
      Try
        FileStream = File.Create(SavePath)
        FileStream.Close()
      Catch ex As Exception
        Throw ex
      End Try
    End If
    Try
      Return File.Open(SavePath, FileMode.Append)
    Catch ex As Exception
      Throw ex
    End Try
  End Function

  Private Sub DownloadCompleted(ErrorInd As Boolean, Retry As Boolean, Optional ErrorString As String = "")
    If Not ErrorInd Then
      Try
        Dim ActualPath As String = SavePath.Substring(0, SavePath.LastIndexOf("."))
        If IO.File.Exists(ActualPath) Then
          If Me.OverwriteExisting Then
            IO.File.Delete(ActualPath)
          Else
            Throw New Exception("File Already Exists")
          End If
        End If
        File.Move(SavePath, ActualPath)
      Catch ex As Exception
        Throw ex
      End Try
    Else
      If IO.File.Exists(SavePath) Then
        Try
          File.Delete(SavePath)
        Catch ex As Exception
          Throw ex
        End Try
      End If
    End If

    If Retry Then
      GetChunk(DownloadPath, 0)
    Else
      RaiseEvent DownloadComplete(Me, New DownloadCompletedEventArgs() With {.Error = ErrorString})
    End If
  End Sub

  Private Function DownloadedPercent() As Double
    Dim d As Double = If(FileStream Is Nothing, 0, FileStream.Length / FileSize) * 100
    Return d
  End Function

#Region " Events and Custon Event Args "

	Public Event ChunkRecieved(sender As Object, ByRef e As ChunkRecievedEventArgs)

  Public Event DownloadComplete(sender As Object, e As DownloadCompletedEventArgs)

  Public Event DownloadCanceled(sender As Object, e As DownloadCanceledEventArgs)

  Public Event DownloadFileError(sender As Object, e As DownloadFileErrorEventArgs)

  Public Class ChunkRecievedEventArgs
    Inherits EventArgs

    Public Property PercentageComplete As Integer
    Public Property Cancel As Boolean

		Public Sub New(PercentageComplete As Integer, Cancel As Boolean)
			Me.PercentageComplete = PercentageComplete
			Me.Cancel = Cancel
		End Sub

  End Class

  Public Class DownloadFileErrorEventArgs
    Inherits EventArgs

    Public Property FileError As String
    Public Property ReTryFileDownload As Boolean = False
    Public Property FileName As String

    Public Sub New(FileError As String, ByRef ReTry As Boolean, FileName As String)
      Me.FileError = FileError
      Me.ReTryFileDownload = ReTry
      Me.FileName = FileName
    End Sub

  End Class

  Public Class DownloadCompletedEventArgs
    Inherits EventArgs

    Public Property [Error] As String = ""

  End Class

  Public Class DownloadCanceledEventArgs
    Inherits EventArgs

  End Class

#End Region

End Class


<Serializable()>
Public Class cmdDownloadFileChunk
  Inherits Singular.CommandBase(Of cmdDownloadFileChunk)

  Public Shared PathProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Path)

  Public Property Path() As String
    Get
      Return ReadProperty(PathProperty)
    End Get
    Set(value As String)
      LoadProperty(PathProperty, value)
    End Set
  End Property

  Public Shared ChunkNumberProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ChunkNumber)

  Public Property ChunkNumber() As Integer
    Get
      Return ReadProperty(ChunkNumberProperty)
    End Get
    Set(value As Integer)
      LoadProperty(ChunkNumberProperty, value)
    End Set
  End Property

  Public Shared ChunkSizeProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ChunkSize)

  Public Property ChunkSize() As Integer
    Get
      Return ReadProperty(ChunkSizeProperty)
    End Get
    Set(value As Integer)
      LoadProperty(ChunkSizeProperty, value)
    End Set
  End Property

  Public Shared FileSizeProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.FileSize)

  Public Property FileSize() As Integer
    Get
      Return ReadProperty(FileSizeProperty)
    End Get
    Set(value As Integer)
      LoadProperty(FileSizeProperty, value)
    End Set
  End Property

  Public Shared ChunkProperty As PropertyInfo(Of Byte()) = RegisterProperty(Of Byte())(Function(c) c.Chunk)

  Public Property Chunk() As Byte()
    Get
      Return ReadProperty(ChunkProperty)
    End Get
    Set(value As Byte())
      LoadProperty(ChunkProperty, value)
    End Set
  End Property

  Public Shared ResultStringProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ResultString)

  Public Property ResultString() As String
    Get
      Return ReadProperty(ResultStringProperty)
    End Get
    Set(value As String)
      LoadProperty(ResultStringProperty, value)
    End Set
  End Property

  Public Shared ErrorStringProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ErrorString)

  Public Property ErrorString() As String
    Get
      Return ReadProperty(ErrorStringProperty)
    End Get
    Set(value As String)
      LoadProperty(ErrorStringProperty, value)
    End Set
  End Property

  Public Shared Function NewcmdDownloadFileChunk(ByVal Path As String, ByVal ChunkSize As Integer, ByVal ChunkNumber As Integer) As cmdDownloadFileChunk

    Return New cmdDownloadFileChunk() With {.Path = Path, .ChunkSize = ChunkSize, .ChunkNumber = ChunkNumber}

  End Function

  Public Sub New()

  End Sub

#If SILVERLIGHT Then
#Else

  Protected Overrides Sub DataPortal_Execute()
    Try
      ResultString = ""
      Dim CompletePath As String
      CompletePath = Path.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")

      If File.Exists(CompletePath) Then

        'If ChunkNumber = 0 OrElse Not IO.File.Exists(CompletePath & ".part") Then
        '  IO.File.Copy(CompletePath, CompletePath & ".part")
        'End If
        'CompletePath = CompletePath & ".part"

        ResultString = ResultString & "File Exists" & vbCrLf
        Using fs As FileStream = File.Open(CompletePath, FileMode.Open)
          ResultString = ResultString & "Reading File" & vbCrLf
          FileSize = fs.Length
          fs.Seek(CType(ChunkSize, Long) * CType(ChunkNumber, Long), SeekOrigin.Begin)
          ResultString = ResultString & "Offset: " & fs.Position & vbCrLf
          Dim tempBuffer As Byte() = New Byte(ChunkSize - 1) {}
          Dim bytesRead As Integer = fs.Read(tempBuffer, 0, ChunkSize)
          ResultString = ResultString & "Bytes Read: " & bytesRead & vbCrLf
          If bytesRead <> ChunkSize Then
            Dim TrimmedBuffer As Byte() = New Byte(bytesRead - 1) {}
            Array.Copy(tempBuffer, TrimmedBuffer, bytesRead)
            ResultString = ResultString & "Loading Trimmed Bytes" & vbCrLf
            Chunk = TrimmedBuffer
            'IO.File.Delete(CompletePath)
          Else
            ResultString = ResultString & "Loading Bytes" & vbCrLf
            Chunk = tempBuffer
          End If
        End Using
      Else
        ErrorString = "File " & CompletePath & " does not exist"
        'Throw New Exception("File " & CompletePath & " does not exist")
      End If
    Catch e As Exception
      Throw e
    End Try

  End Sub

#End If

End Class
