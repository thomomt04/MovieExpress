Imports Csla.Core
Imports Csla
Imports System.Runtime.Serialization
Imports Csla.Serialization
Imports Singular.CommonData.Enums


Namespace Service.Scheduling

  Public Class ScheduleTimer
    Inherits SingularBusinessBase(Of ScheduleTimer)

    Public Delegate Sub TimeUpHandler()

    Public Delegate Sub ForcedToStopHandler()

    Public Shared delForcedToStopProperty As PropertyInfo(Of ForcedToStopHandler) = RegisterProperty(Of ForcedToStopHandler)(Function(c) c.delForcedToStop)

    Public Property delForcedToStop() As ForcedToStopHandler
      Get
        Return GetProperty(delForcedToStopProperty)
      End Get
      Set(value As ForcedToStopHandler)
        SetProperty(delForcedToStopProperty, value)
      End Set
    End Property

    Public Shared TimerProperty As PropertyInfo(Of System.Timers.Timer) = RegisterProperty(Of System.Timers.Timer)(Function(c) c.Timer, "Timer", CType(Nothing, Timers.Timer))

    Public Property Timer() As System.Timers.Timer
      Get
        Return GetProperty(TimerProperty)
      End Get
      Set(value As System.Timers.Timer)
        SetProperty(TimerProperty, value)
      End Set
    End Property

    Public Shared TimeUpHandleProperty As PropertyInfo(Of TimeUpHandler) = RegisterProperty(Of TimeUpHandler)(Function(c) c.TimeUpHandle)

    Public Property TimeUpHandle() As TimeUpHandler
      Get
        Return GetProperty(TimeUpHandleProperty)
      End Get
      Set(value As TimeUpHandler)
        SetProperty(TimeUpHandleProperty, value)
      End Set
    End Property

    Public Shared fsDebugProperty As PropertyInfo(Of System.IO.FileStream) = RegisterProperty(Of System.IO.FileStream)(Function(c) c.fsDebug)

    Public Property fsDebug() As System.IO.FileStream
      Get
        Return GetProperty(fsDebugProperty)
      End Get
      Set(value As System.IO.FileStream)
        SetProperty(fsDebugProperty, value)
      End Set
    End Property

    Public Shared NextRunDateProperty As PropertyInfo(Of DateTime) = RegisterProperty(Of DateTime)(Function(c) c.NextRunDate, "Next Run Date", DateTime.MinValue)

    Public Property NextRunDate As DateTime
      Get
        Return GetProperty(NextRunDateProperty)
      End Get
      Set(value As DateTime)
        SetProperty(NextRunDateProperty, value)
      End Set
    End Property

    Public Shared RunningProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Running)

    Public Property Running() As Boolean
      Get
        Return GetProperty(RunningProperty)
      End Get
      Set(value As Boolean)
        If Running <> value Then
          SetProperty(RunningProperty, value)
        End If
      End Set
    End Property

    Public Shared ScheduleProperty As PropertyInfo(Of Schedule) = RegisterProperty(Of Schedule)(Function(c) c.Schedule)

    Public Property Schedule() As Schedule
      Get
        Return GetProperty(ScheduleProperty)
      End Get
      Set(value As Schedule)
        If Schedule Is Nothing OrElse Schedule IsNot value Then
          If Running Then
            Throw New Exception("Cannot reset the schedule of the timer while timer is running")
          End If
          SetProperty(ScheduleProperty, value)
        End If
      End Set
    End Property

    Public Sub New(ByVal TheSchedule As Schedule, ByVal TimeUpHandler As TimeUpHandler, Optional ByVal ForcedToStopHandler As ForcedToStopHandler = Nothing)
      Schedule = TheSchedule
      TimeUpHandle = TimeUpHandler
      delForcedToStop = ForcedToStopHandler
      Timer = New Timers.Timer()
      If Timer Is Nothing Then
        Throw New Exception("Timer Is Nothing")
      End If
      Try
        AddHandler Timer.Elapsed, AddressOf Timer_Elapsed
      Catch ex As Exception
        Throw New Exception("Something wrong Here:" & ex.Message)
      End Try


    End Sub

    Private Sub SetTimerInterval()

      ' set the interval to half the time between now and the next run date
      Dim dInterval As Double = (DateDiff(DateInterval.Second, Now, NextRunDate) / 2) * 1000
      ' if the interval is less than a tenth of a second (100 ms) then just use a 100 ms
      If dInterval < 100 Then
        dInterval = 100
      ElseIf dInterval >= 1200000 Then
        dInterval = 600000
      End If
      Timer.Interval = dInterval


    End Sub

    Public Sub Start()

      If Schedule Is Nothing Then
        Throw New Exception("Schedule is Nothing")
      End If

      ' get the next scheduled date
      NextRunDate = Schedule.GetNextScheduled()
      ' set the timer Interval
      SetTimerInterval()
      ' start the timer
      Timer.Start()
      Running = True

    End Sub

    Public Sub [Stop]()

      ' get the next scheduled date
      NextRunDate = DateTime.MinValue
      ' set the timer Interval
      SetTimerInterval()
      ' start the timer
      Timer.Stop()
      Running = False

    End Sub

    'Public Function NextRunDates(ByVal NumDates As Integer) As DateTime()

    '  Dim RunDates(NumDates - 1) As DateTime
    '  RunDates(0) = mNextRunDate
    '  For i As Integer = 1 To NumDates - 1
    '    RunDates(i) = mSchedule.GetNextScheduled(RunDates(i - 1).AddMinutes(1))
    '  Next

    'End Function

    Public Function GetTimeToNextScheduledDate() As TimeSpan

      ' this function returns a date time value which tells the time to the next scheduled date
      Return NextRunDate.Subtract(Now)

    End Function

    Public Function GetTimeToNextScheduleDateAsString() As String

      Dim s As String
      Dim ts As TimeSpan = GetTimeToNextScheduledDate()

      If ts.Seconds < 0 Then
        s = ts.Minutes & " Minutes " & ts.Seconds & " Seconds "
      ElseIf ts.Days > 0 Then
        s = ts.Days & " Days " & ts.Hours & " Hours "
      ElseIf ts.Hours > 0 Then
        s = ts.Hours & " Hours " & ts.Minutes & " Minutes "
      ElseIf ts.Minutes > 0 Then
        s = ts.Minutes & " Minutes " & ts.Seconds & " Seconds "
      Else
        s = ts.Seconds & " Seconds "
      End If

      Return s

    End Function

    Private Sub Timer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)

      If NextRunDate.Ticks - Now.Ticks < 0 Then
        'Console.WriteLine("Time up!")
        ' DEBUGGING
        ' fsDebug = New System.IO.FileStream(System.Windows.Forms.Application.StartupPath & "\" & "Schedule_Debug.log", IO.FileMode.OpenOrCreate)
        ' Dim sw As System.IO.StreamWriter = Nothing
        Try
          ' move to the end of the stream
          'fsDebug.Position = Math.Max(0, fsDebug.Length - 1)
          ' indicate that time is up, together with the time
          'sw = New System.IO.StreamWriter(fsDebug)
          'sw.WriteLine(Now.ToLongTimeString & ": Time up.")



          ' less than a 10th of a second so reset the next date
          ' we will add one seccond to the date just to make sure 
          NextRunDate = Schedule.GetNextScheduled(NextRunDate.AddSeconds(1))

          ' sw.WriteLine("Next Run Date: " & mNextRunDate.ToString("dd-MMM-yy HH:mm:ss"))
          If NextRunDate = DateTime.MinValue Then
            ' invalid date, stop the timer
            Me.Stop()
            'sw.WriteLine(Now.ToLongTimeString & ": Timer stopped since next run date is minvalue!")
            ' and tell our creator that we have to stop
            If Not delForcedToStop Is Nothing Then
              delForcedToStop.Invoke()
            End If
          Else
            Try
              ' reset the schedule time
              SetTimerInterval()
              ' fire the event
              Try
                'sw.WriteLine(Now.ToLongTimeString & ": Invoking time up handler...")
                Timer.Stop()
                Try
                  TimeUpHandle.Invoke()
                Finally
                  If Me.Running Then
                    Timer.Start()
                  End If

                End Try
                'sw.WriteLine(Now.ToLongTimeString & ": Time up handler completed successfully")
                ' now the timeup handler might have put us past our next run date
                If NextRunDate < Now Then
                  ' yip, move the next run date
                  NextRunDate = Schedule.GetNextScheduled(Now.AddSeconds(1))
                End If
                ' sw.WriteLine()
              Catch ex As Exception
                ' there is not much we can do here so for now lets just show a message bos
                'sw.WriteLine(Now.ToLongTimeString & ": An error occured executing the Time up handler: " & Singular.Misc.Debug.RecurseExceptionMessage(ex))
                Throw ex
              End Try
            Catch ex As Exception

              If Not Me.Running Then
                Me.Start()
              End If
            End Try
          End If
        Catch ex As Exception
          ' sw.WriteLine(Now.ToLongTimeString & ": An error occured: " & Singular.Misc.Debug.RecurseExceptionMessage(ex))
          'sw.WriteLine()
        Finally
          'sw.Flush()
          'fsDebug.Close()
        End Try
      Else
        ' time is still a while ahead, just set a new interval
        SetTimerInterval()
        'Console.WriteLine("Timer: Interval changed to " & mTimer.Interval)
      End If

    End Sub

  End Class

End Namespace
