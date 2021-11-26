Public Class LogFile

  Public Shared Property LoggingEnabled As Boolean = True

  Private Shared mLogFileName As String = ""

  Public Shared ReadOnly Property LogFileName() As String
    Get
      If mLogFileName = "" Then
        mLogFileName = IO.Path.Combine(Settings.GetSettingsPath, "LogFile.txt")
      End If
      Return mLogFileName
    End Get
  End Property

  Public Shared LogFileCreated As Boolean = False

  Private Shared Sub CreateLogFile()

    Dim Directory As String = IO.Path.GetDirectoryName(LogFileName)
    If Not IO.Directory.Exists(Directory) Then
      IO.Directory.CreateDirectory(Directory)
    End If
    LogFileCreated = True

  End Sub

  Public Shared Sub WriteLogEntry(ByVal LogText As String)

    If LoggingEnabled Then
      If Not LogFileCreated Then
        CreateLogFile()
      End If
      Using fs As New IO.StreamWriter(LogFileName, True)
        fs.WriteLine("> " & Now.ToString("yyyyMMdd HH:mm:ss") & ":- " & LogText)
      End Using
    End If

  End Sub

End Class