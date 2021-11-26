Public Class JobManager

  Private _Parent As ServerProgramBase

  Public Sub New(Parent As ServerProgramBase)
    _Parent = Parent
  End Sub

  Public Class RunInfo

    ''' <summary>
    ''' The date and time the job was meant to run.
    ''' </summary>
    Public Property JobDate As Date

    ''' <summary>
    ''' The date and time the job actually completed (database time).
    ''' </summary>
    Public Property CompleteDate As Date

    Public Sub New(JobDate As Date, CompleteDate As Date)
      Me.JobDate = JobDate
      Me.CompleteDate = CompleteDate
    End Sub
  End Class

  ''' <summary>
  ''' Gets the last run time of a job. 
  ''' </summary>
  Public Function GetLastRunTime(Type As Integer) As RunInfo

    With Singular.CommandProc.GetDataValues(Of Date, Date)("GetProcs.getLastScheduledJobTime",
                                                             {"@ServerProgramID", "@JobType"},
                                                             {_Parent.ServerProgramTypeID, Type})
      Return New RunInfo(.Item1, .Item2)
    End With

  End Function

  Public Sub MarkJobComplete(Type As Integer, JobDate As Date)

    Singular.CommandProc.RunCommand("InsProcs.insServerProgramJob", {"@ServerProgramID", "@JobDate", "@JobType"}, {_Parent.ServerProgramTypeID, JobDate, Type})

  End Sub

  ''' <summary>
  ''' Runs the job today. If the job has run today, it wont run again.
  ''' </summary>
  ''' <param name="SubType">Sub Type for this server program. Make an enum and pass in the enum value.</param>
  ''' <param name="DoWork">Callback if the job hasnt been run.</param>
  Public Sub RunOncePerDay(SubType As Integer, DoWork As Action)

    RunOncePerDay(SubType,
                  Function()
                    DoWork()
                    Return True
                  End Function)

  End Sub

  ''' <summary>
  ''' Runs the job today. If the job has run today, it wont run again.
  ''' </summary>
  ''' <param name="SubType">Sub Type for this server program. Make an enum and pass in the enum value.</param>
  ''' <param name="DoWork">Callback if the job hasnt been run. Must return true if the job completed successfully.</param>
  Public Sub RunOncePerDay(SubType As Integer, DoWork As Func(Of Boolean))

    If GetLastRunTime(SubType).JobDate.Date < Now.Date Then

      If DoWork() Then
        MarkJobComplete(SubType, Now.Date)
      End If

    End If

  End Sub



  Public Sub RunOncePerInterval(SubType As Integer, Interval As TimeSpan, DoWork As Action)

    If Now - GetLastRunTime(SubType).JobDate >= Interval Then

      DoWork()

      MarkJobComplete(SubType, Now)

    End If

  End Sub

  ''' <summary>
  ''' Runs the job for every day since the last day it ran. Wont run more than once per day.
  ''' </summary>
  ''' <param name="subType">Sub Type for this server program. Make an enum and pass in the enum value.</param>
  ''' <param name="DoWork">Callback if the job hasnt been run. Run date is passed in</param>
  ''' <param name="MaxNoOfDays">If this is the first time the job ever runs, how far back must it run from?</param>
  ''' <remarks></remarks>
  Public Sub RunUntilToday(subType As Integer, DoWork As Action(Of Date), Optional MaxNoOfDays As Integer? = Nothing)

    Dim RunDate = GetLastRunTime(subType).JobDate

    If RunDate = Date.MinValue AndAlso MaxNoOfDays IsNot Nothing Then
      RunDate = Now.Date.AddDays(-MaxNoOfDays)
    End If

    While RunDate < Now.Date
      RunDate = RunDate.AddDays(1)

      DoWork(RunDate)

      MarkJobComplete(subType, RunDate)

    End While

  End Sub

  ''' <summary>
  ''' Runs once a month. If the job has run this month, it wont run again. Doesn't restrict to the 1st of the month.
  ''' </summary>
  ''' <param name="SubType">Sub Type for this server program. Make an enum and pass in the enum value.</param>
  ''' <param name="DoWork">Callback if the job hasnt been run.</param>
  Public Sub RunOncePerMonth(SubType As Integer, DoWork As Action)

    If Singular.Dates.DateMonthStart(GetLastRunTime(SubType).JobDate) < Singular.Dates.DateMonthStart(Now) Then

      DoWork()

      MarkJobComplete(SubType, Singular.Dates.DateMonthStart(Now))

    End If

  End Sub

End Class
