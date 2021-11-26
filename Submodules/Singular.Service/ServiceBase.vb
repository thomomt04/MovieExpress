

Public MustInherit Class ServiceBase
  Inherits System.ServiceProcess.ServiceBase

#Region " Properties "

  Protected mServerPrograms As New List(Of ServerProgramBase)
  Private mMainProgram As ServerProgramBase

  Protected mEventLog As EventLog
  Protected mConnectionAttempts As Integer = 1
  Protected mConnectionRetryPeriod As New TimeSpan(0, 1, 0)
  Private mConnectionSuccess As Boolean = False
  Private mProgramsStarted As Boolean = False

  Private mMultiInstanceHandler As MultiInstanceHandler
  Private mServiceUpdater As ServiceUpdater

  Protected ReadOnly Property ConnectionSuccess As Boolean
    Get
      Return mConnectionSuccess
    End Get
  End Property

  Public ReadOnly Property MultiInstanceHandler As MultiInstanceHandler
    Get
      Return mMultiInstanceHandler
    End Get
  End Property

  Public ReadOnly Property ServiceUpdater As ServiceUpdater
    Get
      Return mServiceUpdater
    End Get
  End Property

  Public ReadOnly Property ServerPrograms As List(Of ServerProgramBase)
    Get
      Return mServerPrograms
    End Get
  End Property

#End Region

#Region " Methods "

  Protected Sub AddProgram(ByVal Program As ServerProgramBase)

    LogFile.WriteLogEntry("Add Program " & Program.Name & " : " & Program.ServerProgramType.ServerProgramType)

    Try
      Program.SetVersion(VersionNo)

      mServerPrograms.Add(Program)

    Catch ex As Exception
      mEventLog.WriteEntry("Error addind " & Program.Name & " server program:" & vbCrLf & Singular.Debug.RecurseExceptionMessage(ex))
      If Me.ConnectionSuccess Then
        Program.WriteProgress("Error starting " & Program.Name & " server program:" & vbCrLf & Singular.Debug.RecurseExceptionMessage(ex), ServerProgramBase.ProgressType.Failure)
      End If
    End Try

  End Sub

  Public Sub WriteProgress(ByVal Text As String)
    If Me.ConnectionSuccess Then
      mServerPrograms(0).WriteProgress(Text, ServerProgramBase.ProgressType.Success)
    Else
      mEventLog.WriteEntry(Text)
    End If
  End Sub

#End Region

#Region " Startup "

  Protected Overrides Sub OnStart(ByVal args() As String)
    Try

      Singular.Service.IsService = True

      mMultiInstanceHandler = New MultiInstanceHandler(Me)
      mServiceUpdater = New ServiceUpdater(Me)

      If Not System.Diagnostics.EventLog.SourceExists(Me.EventSource) Then
        System.Diagnostics.EventLog.CreateEventSource(Me.EventSource, "Application")
      End If

      mEventLog = New EventLog("EventLog", ".", Me.EventSource)
      mEventLog.Log = "Application"


      mEventLog.WriteEntry("Attempting to start Service...")

      PreSetup()

      mEventLog.WriteEntry("Settings Path: " & Settings.GetSettingsPath())

      Dim ConnectionString As String = ""
      Try
        ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings(Environment.MachineName).ConnectionString
      Catch
        Try
          ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("Main").ConnectionString
        Catch

        End Try
      End Try

      If ConnectionString = "" Then
        Singular.Settings.SetConnectionStringFromSettings("Connection")
      Else
        Singular.Settings.SetConnectionString(ConnectionString)
        mEventLog.WriteEntry("Settings section found in app.config")
      End If

      If HideConnectionPassword() AndAlso Settings.ConnectionString <> "" Then
        Dim parser As New System.Data.SqlClient.SqlConnectionStringBuilder(Settings.ConnectionString)
        mEventLog.WriteEntry(String.Format("Connection: Server: {0}, Database {1}, Integrated? {2}, UserName: {3}, Password Length: {4}",
                                           parser.DataSource, parser.InitialCatalog, parser.IntegratedSecurity.ToString, parser.UserID, parser.Password.Length))
      Else
        mEventLog.WriteEntry("Connection String: " & Settings.ConnectionString)
      End If


      If RequiresDBConnection() Then
        ConnectToDatabase(True)
      Else
        OnDatabaseConnected()
      End If

    Catch ex As Exception
      Throw ex
    End Try

  End Sub

  Private Sub ConnectToDatabase(Initial As Boolean)

    Dim ex As New Exception()
    Dim parser As New System.Data.SqlClient.SqlConnectionStringBuilder(Settings.ConnectionString)

    If Singular.Settings.TestConnection(parser.DataSource, parser.InitialCatalog, parser.IntegratedSecurity, parser.UserID, parser.Password, ex, 4) = Settings.ConnectionTestResults.Success Then

      mConnectionSuccess = True
      OnDatabaseConnected()

    Else

      mEventLog.WriteEntry("Database connection failed: " & Singular.Debug.RecurseExceptionMessage(ex))
      mConnectionAttempts -= 1
      If mConnectionAttempts = 0 Then

        If Initial Then
          'Called from startup, throw exception so that the user can see the service has failed to start.
          Throw New Exception("Database connection failed")
        Else
          'Called from timer, the service has already started, although it is not doing anything usefull. Stop it so anyone looking at services will see there is a problem.
          [Stop]()
        End If
      Else
        mEventLog.WriteEntry("Will attempt to connect again at " & Now.Add(mConnectionRetryPeriod).ToString("HH:mm:ss"))
        Dim tmr As New System.Threading.Timer(Sub(t)
                                                ConnectToDatabase(False)
                                              End Sub, Nothing, mConnectionRetryPeriod, New TimeSpan(0, 0, 0, 0, -1))
      End If

    End If

  End Sub

#End Region

#Region " Ready "

  Private Sub OnDatabaseConnected()

    Try
      OtherSetup()

      mEventLog.WriteEntry("Connected to database, adding programs")

      AddServerPrograms()

      mMainProgram = mServerPrograms(0)

      mEventLog.WriteEntry("Added " & mServerPrograms.Count & " server program(s)")

      If ConnectionSuccess Then
        'Write entry into database and start programs and schedule timers
        mMainProgram.WriteProgress(String.Format("Service started on {0} - Version {1}", Environment.MachineName, VersionNo), ServerProgramBase.ProgressType.Success)
      End If

      If RequiresDBConnection() Then
        'Check if this is the first service to connect to the DB
        mMultiInstanceHandler.Start(mMainProgram, AddressOf StartServerPrograms)
      Else
        StartServerPrograms()
      End If

    Catch ex As Exception
      mEventLog.WriteEntry("Error starting service: " & ex.Message)
      Throw ex
    End Try

  End Sub

  Private Sub StartServerPrograms()

    Dim StartTimes As String = String.Empty

    For Each prog As ServerProgramBase In mServerPrograms
      Try
        Dim StartTime = Now
        prog.Start()
        StartTimes &= prog.Name & ": " & (Now - StartTime).TotalMilliseconds.ToString("#,##0") & vbCrLf
      Catch ex As Exception
        If Me.ConnectionSuccess Then
          LogFile.WriteLogEntry("Program  " & prog.Name & " : " & prog.ServerProgramType.ServerProgramType)
          prog.WriteProgress("Cant start " & prog.Name & " Server Program: " & Singular.Debug.RecurseExceptionMessage(ex), ServerProgramBase.ProgressType.Success)
        Else
          mEventLog.WriteEntry("Cant start " & prog.Name & " Scheduled Program: " & Singular.Debug.RecurseExceptionMessage(ex))
        End If
      End Try
    Next

    mProgramsStarted = True
    mEventLog.WriteEntry("Server Programs Started, start times: " & vbCrLf & StartTimes)

    mServiceUpdater.Initialise()

  End Sub

#End Region

#Region " Overridable Properties "

  Protected MustOverride ReadOnly Property VersionNo() As String
  Protected MustOverride ReadOnly Property ServiceDescription() As String
  Protected MustOverride Sub AddServerPrograms()

  ''' <summary>
  ''' Called before any settings path / connection setup is done.
  ''' </summary>
  Protected Overridable Sub PreSetup()

  End Sub

  ''' <summary>
  ''' Called when a database connection has been established.
  ''' </summary>
  Protected Overridable Sub OtherSetup()

  End Sub

  Public Overridable ReadOnly Property ServiceType() As String
    Get
      Return ".Service"
    End Get
  End Property

  Public Overridable ReadOnly Property EventSource() As String
    Get
      Return Me.ServiceDescription & Me.ServiceType
    End Get
  End Property

  Protected Overridable Function RequiresDBConnection() As Boolean
    Return True
  End Function

  Protected Overridable Function HideConnectionPassword() As Boolean
    Return False
  End Function

#End Region

#Region " Stop "

  Protected Overrides Sub OnStop()

    If mProgramsStarted Then
      For Each prog As ServerProgramBase In mServerPrograms
        Try
          If Me.ConnectionSuccess Then
            prog.WriteProgress("Stopping " & prog.Name, ServerProgramBase.ProgressType.Success)
          End If
          prog.Stop()
          mEventLog.WriteEntry(prog.Name & " Stopped")
        Catch ex As Exception
          mEventLog.WriteEntry("Error stopping " & prog.Name & ":" & ex.Message)
          If Me.ConnectionSuccess Then
            prog.WriteProgress("Exception Stopping " & prog.Name & " Service: " & ex.Message, ServerProgramBase.ProgressType.Failure)
          End If
        End Try
      Next
    End If

    MultiInstanceHandler.Stop()
    mServiceUpdater.Stop()

    If ConnectionSuccess Then mMainProgram.WriteProgress("Service Stopped")
    mEventLog.WriteEntry("Service Stopped")
  End Sub

#End Region

End Class

