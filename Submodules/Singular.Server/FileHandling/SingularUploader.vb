Imports Csla
Imports System.IO
Imports Csla.Serialization
Imports Singular

Public Class SingularUploader

  'Public Property Path As String
  'Public Property ChunkNumber As Integer
  Public Property ChunkSize As Integer = 250000
  Public Property CancelIND As Boolean = False
  Private Property FileSize As Integer = -1
  'Private Property Chunk As Byte()
  Private Property ResultString As String = ""
  Private Property ErrorString As String = ""
  Private Property SourcePath As String = ""
  Private Property Source As IO.FileInfo
  Private Property DestinationPath As String = ""
  Private Property FileStream As FileStream = Nothing
  Private Property RestartUpload As Boolean = False


  Public Sub New(ByVal SourcePath As String, ByVal DestinationPath As String)

    'This code should use the following line:
    'System.Web.HttpContext.Current.Server.UrlDecode

    If SourcePath.StartsWith("\\") Then
      Me.SourcePath = "\" & SourcePath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    Else
      Me.SourcePath = SourcePath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    End If
    Me.Source = New IO.FileInfo(Me.SourcePath)
    If DestinationPath.StartsWith("\\") Then
      Me.DestinationPath = "\" & DestinationPath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    Else
      Me.DestinationPath = DestinationPath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    End If

  End Sub

  Public Sub New(ByVal Source As IO.FileInfo, ByVal DestinationPath As String)
    Me.Source = Source

    If DestinationPath.StartsWith("\\") Then
      Me.DestinationPath = "\" & DestinationPath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    Else
      Me.DestinationPath = DestinationPath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
    End If
  End Sub

  Public Sub SendFile()
    SendChunk(Me.DestinationPath, Me.Source, 0)
  End Sub

  'Public Sub Restart()
  '  RestartUpload = True
  '  SendChunk(Me.DestinationPath, Me.SourcePath, 0)
  'End Sub

  Public Sub Cancel()
    RaiseEvent UploadCanceled(Me, New UploadCanceledEventArgs)
  End Sub

  Private Sub SendChunk(ByVal DestinationPath As String, ByVal Source As IO.FileInfo, ByVal ChunkNumber As Integer)
    Dim cmdUpload As cmdUploadFileChunk
    Dim Chunk As Byte()
    Try
      Dim CompletePath As String
      CompletePath = SourcePath.Replace("%20", " ").Replace("%5C", "\").Replace("\\", "\")
      If Source.Exists Then
        'NB The change made below is so that we can read 'read only' files.
        ' Using fs As FileStream = File.Open(CompletePath, FileMode.Open)
        Using fs As FileStream = Source.OpenRead ' File.OpenRead(CompletePath)
          FileSize = fs.Length
          fs.Seek(CType(ChunkSize, Long) * CType(ChunkNumber, Long), SeekOrigin.Begin)
          Dim tempBuffer As Byte() = New Byte(ChunkSize - 1) {}
          Dim bytesRead As Integer = fs.Read(tempBuffer, 0, ChunkSize)
          If bytesRead <> ChunkSize Then
            Dim TrimmedBuffer As Byte() = New Byte(bytesRead - 1) {}
            Array.Copy(tempBuffer, TrimmedBuffer, bytesRead)
            Chunk = TrimmedBuffer
          Else
            Chunk = tempBuffer
          End If
        End Using
        GC.Collect()
      Else
        Dim retry As Boolean = False
        RaiseEvent UploadFileError(Me, New UploadFileErrorEventArgs("File " & CompletePath & " does not exist", retry, DestinationPath.Substring(0, DestinationPath.LastIndexOf("."))))
        UploadCompleted(True, retry)
        Exit Sub

        'Throw New Exception("File " & CompletePath & " does not exist")
      End If
      cmdUpload = cmdUploadFileChunk.NewcmdUploadFileChunk(DestinationPath, FileSize, Chunk, ChunkNumber)

    Catch ex As Exception
      Throw ex
    End Try
    If cmdUpload IsNot Nothing Then
      cmdUpload.BeginExecute(Sub(o, e)
                               If e.Error IsNot Nothing Then
                                 Dim retry As Boolean = False
                                 RaiseEvent UploadFileError(Me, New UploadFileErrorEventArgs(e.Error.Message, retry, DestinationPath.Substring(0, DestinationPath.LastIndexOf("."))))
                                 UploadCompleted(True, retry)
                                 Exit Sub
                               End If
                               If e.Object.ErrorString <> "" Then
                                 Dim retry As Boolean = False
                                 RaiseEvent UploadFileError(Me, New UploadFileErrorEventArgs(e.Object.ErrorString, retry, DestinationPath.Substring(0, DestinationPath.LastIndexOf("."))))
                                 UploadCompleted(True, retry, e.Object.ErrorString)
                               ElseIf CancelIND = True Then
                                 Dim retry As Boolean = False
                                 RaiseEvent UploadCanceled(Me, New UploadCanceledEventArgs)
                                 UploadCompleted(True, retry, "Process Cancelled")
                               ElseIf Not e.Object.CompleteInd Then
                                 RaiseEvent ProgressChanged(e.Object.UploadPercentage)
                                 SendChunk(DestinationPath, Source, ChunkNumber + 1)
                               Else
                                 RaiseEvent ProgressChanged(e.Object.UploadPercentage)
                                 UploadCompleted(False, False)
                               End If
                             End Sub)
    End If
  End Sub

  Private Sub ChunkSent(sender As Object, e As DataPortalResult(Of Singular.SingularUploader))

  End Sub

  Public Event ProgressChanged(Percentage As Double)

  Private Sub UploadCompleted(ErrorInd As Boolean, Retry As Boolean, Optional ErrorString As String = "")

    If Retry Then
      SendChunk(DestinationPath, Source, 0)
    Else
      RaiseEvent UploadComplete(Me, New UploadCompletedEventArgs() With {.Error = ErrorString})
    End If
  End Sub

#Region " Events and Custon Event Args "

  Public Event ChunkReceived(sender As Object, ByRef e As ChunkReceivedEventArgs)

  Public Event UploadComplete(sender As Object, e As UploadCompletedEventArgs)

  Public Event UploadCanceled(sender As Object, e As UploadCanceledEventArgs)

  Public Event UploadFileError(sender As Object, e As UploadFileErrorEventArgs)

  Public Class ChunkReceivedEventArgs
    Inherits EventArgs

    Public Property PercentageComplete As Integer
    Public Property Cancel As Boolean

    Public Sub New(PercentageComplete As Integer, ByRef Cancel As Boolean)
      Me.PercentageComplete = PercentageComplete
      Me.Cancel = Cancel
    End Sub

  End Class

  Public Class UploadCompletedEventArgs
    Inherits EventArgs

    Public Property [Error] As String = ""

  End Class

  Public Class UploadCanceledEventArgs
    Inherits EventArgs

  End Class

  Public Class UploadFileErrorEventArgs
    Inherits EventArgs

    Public Property FileError As String
    Public Property ReTryFileUpload As Boolean = False
    Public Property FileName As String

    Public Sub New(FileError As String, ByRef ReTry As Boolean, FileName As String)
      Me.FileError = FileError
      Me.ReTryFileUpload = ReTry
      Me.FileName = FileName
    End Sub

  End Class

#End Region

  Public Sub RemoveFile()
    Dim cmd As cmdRemoveFile = cmdRemoveFile.NewcmdRemoveFile(DestinationPath)
    cmd.BeginExecute(Sub(o, e)

                     End Sub)
  End Sub

End Class


<Serializable()>
Public Class cmdUploadFileChunk
  Inherits Singular.CommandBase(Of cmdUploadFileChunk)

  Public Shared SavePathProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SavePath)

  Public Property SavePath() As String
    Get
      Return ReadProperty(SavePathProperty)
    End Get
    Set(value As String)
      LoadProperty(SavePathProperty, value)
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

  Public Shared FileSizeProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.FileSize)

  Public Property FileSize() As Integer
    Get
      Return ReadProperty(FileSizeProperty)
    End Get
    Set(value As Integer)
      LoadProperty(FileSizeProperty, value)
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

  Public Shared ChunkProperty As PropertyInfo(Of Byte()) = RegisterProperty(Of Byte())(Function(c) c.Chunk)

  Public Property Chunk() As Byte()
    Get
      Return ReadProperty(ChunkProperty)
    End Get
    Set(value As Byte())
      LoadProperty(ChunkProperty, value)
    End Set
  End Property

  Public Shared UploadPercentageProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.UploadPercentage)

  Public Property UploadPercentage() As Integer
    Get
      Return ReadProperty(UploadPercentageProperty)
    End Get
    Set(value As Integer)
      LoadProperty(UploadPercentageProperty, value)
    End Set
  End Property

  Public Shared CompleteIndProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.CompleteInd)

  Public Property CompleteInd() As Boolean
    Get
      Return ReadProperty(CompleteIndProperty)
    End Get
    Set(value As Boolean)
      LoadProperty(CompleteIndProperty, value)
    End Set
  End Property

  Public Shared Function NewcmdUploadFileChunk(ByVal SavePath As String, ByVal FileSize As Integer, ByVal Chunk As Byte(), ByVal ChunkNumber As Integer) As cmdUploadFileChunk

    Return New cmdUploadFileChunk() With {.SavePath = SavePath, .FileSize = FileSize, .Chunk = Chunk, .ChunkNumber = ChunkNumber}

  End Function

  Public Sub New()

  End Sub


#If SILVERLIGHT Then
#Else

  Private Sub VerifyFile(SavePath As String)
    Dim SaveDirectoryPath = SavePath.Substring(0, SavePath.LastIndexOf("\"))
    If Not IO.Directory.Exists(SaveDirectoryPath) Then
      Try
        IO.Directory.CreateDirectory(SaveDirectoryPath)
      Catch ex As Exception
        Throw ex
        ErrorString = ex.ToString
      End Try
    End If
    If Not File.Exists(SavePath) Then
      Try
        Dim FileStream = File.Create(SavePath)
        FileStream.Close()
      Catch ex As Exception
        Throw ex
        ErrorString = ex.ToString
      End Try
    End If
  End Sub

  Protected Overrides Sub DataPortal_Execute()
    Try

      If ChunkNumber = 0 AndAlso IO.File.Exists(SavePath) Then
        IO.File.Delete(SavePath)
      End If
      'Dim SaveDirectoryPath = SavePath.Substring(0, SavePath.LastIndexOf("\"))
      VerifyFile(SavePath)
      If ErrorString = "" Then
        Using FileStream As IO.FileStream = IO.File.Open(SavePath, FileMode.Append)
          FileStream.Write(Chunk, 0, Chunk.Length)
          Dim dp As Integer = (FileStream.Length / If(FileSize = 0, 1, FileSize)) * 100
          UploadPercentage = dp
          CompleteInd = FileStream.Length = FileSize
          FileStream.Close()
        End Using
      End If

    Catch e As Exception
      ErrorString &= " " & e.Message
    End Try
  GC.Collect()
  End Sub

#End If

End Class

<Serializable()>
Public Class cmdRemoveFile
  Inherits Singular.CommandBase(Of cmdRemoveFile)

  Public Shared FilePathProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.FilePath)

  Public Property FilePath() As String
    Get
      Return ReadProperty(FilePathProperty)
    End Get
    Set(value As String)
      LoadProperty(FilePathProperty, value)
    End Set
  End Property

  Public Shared Function NewcmdRemoveFile(ByVal FilePath As String) As cmdRemoveFile

    Return New cmdRemoveFile() With {.FilePath = FilePath}

  End Function

  Private Sub New()

  End Sub


#If SILVERLIGHT Then
#Else

  Protected Overrides Sub DataPortal_Execute()
    Try

      If IO.File.Exists(FilePath) Then
        IO.File.Delete(FilePath)
      End If

    Catch e As Exception
    End Try

  End Sub

#End If

End Class
