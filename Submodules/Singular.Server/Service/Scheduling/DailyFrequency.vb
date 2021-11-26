Imports Csla.Serialization
Imports Csla
Imports Singular.CommonData.Enums


Namespace Service.Scheduling

#Region " Enumerations "

  Public Enum DailyFrequencyType
    <System.ComponentModel.DataAnnotations.Display(Order:=0)>
    Once = 0
    <System.ComponentModel.DataAnnotations.Display(Order:=1)>
    Every = 1
  End Enum

  Public Enum TimeMeasure
    Hour = 1
    Minute = 2
    Second = 3
  End Enum

#End Region

#Region " Daily Frequency Base Class "

  Public Interface IDailyFrequency

    Function GetTimeOfDay(ByVal AtTime As DateTime) As DateTime
    Function GetStartTime() As DateTime

  End Interface

  <Serializable()> _
  Public MustInherit Class DailyFrequency(Of T As DailyFrequency(Of T))
    Inherits SingularBusinessBase(Of T)
    Implements IDailyFrequency

    Public Shared TypeProperty As PropertyInfo(Of DailyFrequencyType) = RegisterProperty(Of DailyFrequencyType)(Function(c) c.Type)

    Public Property Type() As DailyFrequencyType
      Get
        Return GetProperty(TypeProperty)
      End Get
      Set(value As DailyFrequencyType)
        SetProperty(TypeProperty, value)
      End Set
    End Property

    Public Overrides Function ToString() As String
      Return ""
    End Function


    Protected Sub New()

    End Sub

    Public MustOverride Function GetTimeOfDay(ByVal AtTime As DateTime) As DateTime Implements IDailyFrequency.GetTimeOfDay
    Public MustOverride Function GetStartTime() As DateTime Implements IDailyFrequency.GetStartTime

  End Class

#End Region

  <Serializable()> _
  Public Class DailyFrequencyOnce
    Inherits DailyFrequency(Of DailyFrequencyOnce)


    Public Shared AtTimeProperty As PropertyInfo(Of DateTime) = RegisterProperty(Of DateTime)(Function(c) c.AtTime, "AtTime", Now.Date)

    <Singular.DataAnnotations.TimeField()>
    Public Property AtTime() As DateTime
      Get
        Return GetProperty(AtTimeProperty)
      End Get
      Set(value As DateTime)
        SetProperty(AtTimeProperty, value)
      End Set
    End Property

    Public Sub New(ByVal AtTime As DateTime)
      MyBase.New()
      Me.AtTime = AtTime
      Me.Type = DailyFrequencyType.Once
    End Sub

    Public Sub New()
      MyBase.New()

    End Sub

    Public Overrides Function ToString() As String

      Return "at " & Me.AtTime.ToString("HH:mm")

    End Function

    Public Overloads Overrides Function GetTimeOfDay(ByVal AtTime As DateTime) As Date

      ' add the time to the current date
      Dim dtReturn As DateTime = AtTime.Date.AddTicks(Me.AtTime.TimeOfDay.Ticks)
      ' if the result date is after the AtTime then add a day
      If dtReturn < AtTime Then
        Return dtReturn.AddDays(1)
      Else
        Return dtReturn
      End If

    End Function

    Public Overrides Function GetStartTime() As Date
      Return GetProperty(AtTimeProperty)
    End Function

  End Class

  <Serializable()> _
  Public Class DailyFrequencyEvery
    Inherits DailyFrequency(Of DailyFrequencyEvery)

    Public Shared UnitProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Unit, "Unit", 1)

    Public Property Unit() As Integer
      Get
        Return GetProperty(UnitProperty)
      End Get
      Set(value As Integer)
        SetProperty(UnitProperty, value)
      End Set
    End Property

    Public Shared TimeMeasureProperty As PropertyInfo(Of TimeMeasure) = RegisterProperty(Of TimeMeasure)(Function(c) c.TimeMeasure, "TimeMeasure", TimeMeasure.Hour)

    <Singular.DataAnnotations.DropDownWeb(GetType(TimeMeasure))>
    Public Property TimeMeasure() As TimeMeasure
      Get
        Return GetProperty(TimeMeasureProperty)
      End Get
      Set(value As TimeMeasure)
        SetProperty(TimeMeasureProperty, value)
      End Set
    End Property

    Public Shared StartTimeProperty As PropertyInfo(Of DateTime) = RegisterProperty(Of DateTime)(Function(c) c.StartTime)

    <Singular.DataAnnotations.TimeField()>
    Public Property StartTime() As DateTime
      Get
        Return GetProperty(StartTimeProperty)
      End Get
      Set(value As DateTime)
        SetProperty(StartTimeProperty, value)
      End Set
    End Property

    Public Shared EndTimeProperty As PropertyInfo(Of DateTime) = RegisterProperty(Of DateTime)(Function(c) c.EndTime)

    <Singular.DataAnnotations.TimeField()>
    Public Property EndTime() As DateTime
      Get
        Return GetProperty(EndTimeProperty)
      End Get
      Set(value As DateTime)
        SetProperty(EndTimeProperty, value)
      End Set
    End Property

    Public Shared EndTimeNextDateProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.EndTimeNextDate, "EndTimeNextDate", False)

    Public Property EndTimeNextDate() As Boolean
      Get
        Return GetProperty(EndTimeNextDateProperty)
      End Get
      Set(value As Boolean)
        SetProperty(EndTimeNextDateProperty, value)
      End Set
    End Property

    Public Overrides Function ToString() As String

      Return "every " & Me.Unit & " " & CommonData.Enums.Description(Me.TimeMeasure) & IIf(Me.Unit > 1, "s", "") & " from " & Me.StartTime.ToString("HH:mm") & " to " & Me.EndTime.ToString("HH:mm")

    End Function

    Public Sub New(ByVal Unit As Integer, ByVal TimeMeasure As TimeMeasure, ByVal StartTime As DateTime, ByVal EndTime As DateTime)

      MyBase.New()
      Me.Type = DailyFrequencyType.Every
      Me.Unit = Unit
      Me.TimeMeasure = TimeMeasure
      Me.StartTime = StartTime
      Me.EndTime = EndTime
      If Me.StartTime.TimeOfDay > Me.EndTime.TimeOfDay Then
        Me.EndTimeNextDate = True
      End If

    End Sub

    Public Sub New()
      MyBase.New()

    End Sub

    Public Overloads Overrides Function GetTimeOfDay(ByVal AtDateAndTime As DateTime) As Date

      Dim startTicks As Long = Me.StartTime.TimeOfDay.Ticks
      Dim endTicks As Long = Me.EndTime.TimeOfDay.Ticks
      If Me.EndTimeNextDate Then
        endTicks += System.TimeSpan.TicksPerDay
      End If

      Dim dtReturnTicks As Long = AtDateAndTime.TimeOfDay.Ticks

      If AtDateAndTime.TimeOfDay.Ticks < startTicks And AtDateAndTime.TimeOfDay.Ticks > Me.EndTime.TimeOfDay.Ticks And Me.EndTimeNextDate Or _
          AtDateAndTime.TimeOfDay.Ticks < startTicks And AtDateAndTime.TimeOfDay.Ticks < endTicks And Not Me.EndTimeNextDate Then
        ' time is before start time so return at time with start time component
        dtReturnTicks = Me.StartTime.TimeOfDay.Ticks
        Dim dtReturn As Date = Now.Date
        dtReturn = dtReturn.AddTicks(dtReturnTicks)
        Return dtReturn
      ElseIf AtDateAndTime.TimeOfDay.Ticks > startTicks And AtDateAndTime.TimeOfDay.Ticks > endTicks And Not Me.EndTimeNextDate Then
        ' time is after both start and end times, so the next time is the start time for the next day
        dtReturnTicks = Me.StartTime.TimeOfDay.Ticks + TimeSpan.TicksPerDay
        Dim dtReturn As Date = Now.Date
        dtReturn = dtReturn.AddTicks(dtReturnTicks)
        Return dtReturn
      Else
        ' start time has already passed so lets start at start time and move units forward until 
        ' we are past at time and in the limit of start and end time

        dtReturnTicks = Me.StartTime.TimeOfDay.Ticks
        Dim DurationTicks As Long = endTicks - startTicks
        Dim DurationNowTicks As Long = AtDateAndTime.TimeOfDay.Ticks - startTicks
        If DurationNowTicks < 0 Then
          DurationNowTicks += TimeSpan.TicksPerDay
        End If
        'While (dtReturnTicks < AtDateAndTime.TimeOfDay.Ticks And dtReturnTicks <= endTicks) 'Or dtReturnTicks = AtDateAndTime.TimeOfDay.Ticks
        While (dtReturnTicks - startTicks < DurationTicks And dtReturnTicks - startTicks < DurationNowTicks)
          If Me.TimeMeasure = Scheduling.TimeMeasure.Hour Then
            dtReturnTicks += Unit * TimeSpan.TicksPerHour
          ElseIf Me.TimeMeasure = Scheduling.TimeMeasure.Minute Then
            dtReturnTicks += Unit * TimeSpan.TicksPerMinute
          ElseIf Me.TimeMeasure = Scheduling.TimeMeasure.Second Then
            dtReturnTicks += Unit * TimeSpan.TicksPerSecond
          End If
          Dim x As New Date(dtReturnTicks)
          If dtReturnTicks > endTicks Then
            ' we have gone past the end time, so add a day to it and set the time to the start time
            dtReturnTicks = startTicks + TimeSpan.TicksPerDay
          End If
        End While
        ' now we should have the right time
        Dim dtReturn As Date = Nothing
        If AtDateAndTime.TimeOfDay.Ticks < startTicks And AtDateAndTime.TimeOfDay.Ticks < endTicks Then
          'we have rolled over to the next day so set the date to the day the schedule started and add the ticks
          dtReturn = AtDateAndTime.Date.AddDays(-1)
        Else
          dtReturn = AtDateAndTime.Date
        End If
        dtReturn = dtReturn.AddTicks(dtReturnTicks)
        Return dtReturn
      End If

    End Function

    Public Overrides Function GetStartTime() As Date
      Return GetProperty(StartTimeProperty)
    End Function

  End Class

End Namespace