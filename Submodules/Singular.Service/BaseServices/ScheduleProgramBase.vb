Imports Singular.Service.Scheduling
Imports System.Runtime.InteropServices
Imports System.Threading

Public MustInherit Class ScheduleProgramBase
  Inherits ServerProgramBase

  'Private _ScheduleLock As New Object
  Private _Schedule As Scheduling.Schedule
  Private _ServerProgramTypeID As Integer
  Private _Timer As Timer
  Private _DueTime As Date
  Private _IsBusyRunning As Boolean = False

  Public ReadOnly Property Schedule As Scheduling.Schedule
    Get
      Return _Schedule
    End Get
  End Property

  Public Sub New(Schedule As Schedule, ByVal Name As String)
    MyBase.New(Name)

    _Schedule = Schedule

  End Sub

  Public Sub New(ByVal ServerProgramTypeID As Integer, ByVal Name As String)

    MyBase.New(Name)
    _ServerProgramTypeID = ServerProgramTypeID

    'Get the schedule from the database
    _Schedule = GetSchedule()

  End Sub

  Protected Overridable Function GetSchedule() As Schedule
    Return Me.ServerProgramType.Info
  End Function

  Public Overrides ReadOnly Property ServerProgramTypeID As Integer
    Get
      Return _ServerProgramTypeID
    End Get
  End Property

  Public Overrides Sub Start()

    If _Schedule Is Nothing Then
      Throw New Exception("Schedule has not been set up for " & Name)
    End If

    If StartSchedule() Then
      Try
        StartTimer()
        ScheduleStarted()
      Catch ex As Exception
        If LogFile.LoggingEnabled Then
          LogFile.WriteLogEntry("Error Starting Timer: " & ex.Message)
        End If
      End Try

    End If

  End Sub

  Public Overrides Sub [Stop]()
    If _Timer IsNot Nothing Then _Timer.Dispose()
  End Sub

  Public Sub RunNow()
    If Not _IsBusyRunning Then
      TimeUp()
    End If
  End Sub

  Public ReadOnly Property Active() As Boolean
    Get
      Return Me.ServerProgramType.ActiveInd
    End Get
  End Property

  Protected MustOverride Sub TimeUp()

  Protected MustOverride Function StartSchedule() As Boolean

  Protected MustOverride Function StopSchedule() As Boolean

  Public Sub TestTimeUp()
    TimeUp()
  End Sub

  Public Sub TestStartSchedule()
    StartSchedule()
  End Sub

  Private Sub MemoryCleanup()

    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1)

  End Sub

  Protected Overridable Sub ScheduleStarted()

  End Sub

  <DllImport("kernel32")> _
  Public Shared Function SetProcessWorkingSetSize(ByVal instr As IntPtr, ByVal Minsize As Integer, ByVal MaxSize As Integer) As Integer
  End Function

  Protected Friend Overrides Sub ServiceInfoChanged(NewInfo As ServerProgramType)
    MyBase.ServiceInfoChanged(NewInfo)

    _Schedule = Me.GetSchedule()

    If Not _IsBusyRunning Then
      StartTimer()
    End If

    If ServerProgramType.ActiveInd Then
      WriteProgress("Schedule reset, next runtime: " & Schedule.GetNextScheduled().ToString("dd MMM yyyy HH:mm"))
    End If

  End Sub

  Private Sub StartTimer()

    Try

      If Schedule.Duration.IsValidDate(Now) Then

        _DueTime = _Schedule.GetNextScheduled

        Dim Timeout = _DueTime - Now
        If Timeout < TimeSpan.Zero Then
          Timeout = TimeSpan.Zero
        End If

        If _Timer IsNot Nothing Then _Timer.Dispose()
        _Timer = New Timer(AddressOf TimeElapsed, Nothing, Timeout, New TimeSpan(0, 0, 0, 0, System.Threading.Timeout.Infinite))

      End If

    Catch ex As Exception
      WriteProgress("Error starting timer: ", ex)
    End Try

  End Sub

  Private Sub TimeElapsed()

    Try
      _IsBusyRunning = True

      While Now < _DueTime
        Dim WaitTimeMs = (_DueTime - Now).TotalMilliseconds

        If WaitTimeMs > 0 Then Thread.Sleep(WaitTimeMs)
      End While


      If Service.ServiceUpdateMode = ServiceUpdateOption.NotEnabled Then

        Dim TempSchedule = mServerProgramType

        'Every time the schedule runs, we refetch it from the database in case the user marked it as inactive, or its schedule was updated.
        If RefetchServiceInfo() Then

          _Schedule = Me.GetSchedule()
        Else
          'Set the service info back to what it was.
          mServerProgramType = TempSchedule
        End If

      End If

      If ServerProgramType.ActiveInd Then

        'Call user code
        TimeUp()
        MemoryCleanup()

      End If

    Catch ex As Exception
      WriteProgress("Error in schedule: " & Singular.Debug.RecurseExceptionMessage(ex))
    Finally

      StartTimer()
      _IsBusyRunning = False

    End Try

  End Sub

End Class
