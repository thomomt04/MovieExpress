Imports Singular.CommonData.Enums
Imports Csla
Imports Csla.Serialization

Namespace Service.Scheduling

#Region " Enumerations "

  Public Enum OccursType
    <System.ComponentModel.DataAnnotations.Display(Order:=0)>
    Daily = 0
    <System.ComponentModel.DataAnnotations.Display(Order:=1)>
    Weekly = 1
    <System.ComponentModel.DataAnnotations.Display(Order:=2)>
    Monthly = 2
  End Enum

  Public Enum DaysOfWeek
    <System.ComponentModel.DescriptionAttribute("Monday")>
    Monday = 0
    <System.ComponentModel.DescriptionAttribute("Tuesday")>
    Tuesday = 1
    <System.ComponentModel.DescriptionAttribute("Wednesday")>
    Wednesday = 2
    <System.ComponentModel.DescriptionAttribute("Thursday")>
    Thursday = 3
    <System.ComponentModel.DescriptionAttribute("Friday")>
    Friday = 4
    <System.ComponentModel.DescriptionAttribute("Saturday")>
    Saturday = 5
    <System.ComponentModel.DescriptionAttribute("Sunday")>
    Sunday = 6
    <System.ComponentModel.DescriptionAttribute("Day")>
    Day = 7
    <System.ComponentModel.DescriptionAttribute("Weekday")>
    Weekday = 8
    <System.ComponentModel.DescriptionAttribute("WeekendDay")>
    WeekendDay = 9
  End Enum

  Public Enum DayNumeration
    <System.ComponentModel.DescriptionAttribute("1st")>
      First = 1
    <System.ComponentModel.DescriptionAttribute("2nd")>
      Second = 2
    <System.ComponentModel.DescriptionAttribute("3rd")>
      Third = 3
    <System.ComponentModel.DescriptionAttribute("4th")>
      Fourth = 4
    <System.ComponentModel.DescriptionAttribute("Last")>
      Last = -1
  End Enum

  Public Enum OccursMonthlyType
    Day = 1
    The = 0
  End Enum


#End Region

#Region " Occurs Base Class "

  Public Interface IOccurs

    Function GetNextOccuranceDay(ByVal FromDate As DateTime, ByVal DailyFrequency As IDailyFrequency) As DateTime

  End Interface

  <Serializable()> _
  Public MustInherit Class Occurs(Of T As Occurs(Of T))
    Inherits SingularBusinessBase(Of T)
    Implements IOccurs


    Public Shared TypeProperty As PropertyInfo(Of OccursType) = RegisterProperty(Of OccursType)(Function(c) c.Type)

    Public Property Type() As OccursType
      Get
        Return GetProperty(TypeProperty)
      End Get
      Set(value As OccursType)
        SetProperty(TypeProperty, value)
      End Set
    End Property

    Public Overrides Function ToString() As String
      Return ""
    End Function

    Public MustOverride Function GetNextOccuranceDay(ByVal FromDate As DateTime, ByVal DailyFrequency As IDailyFrequency) As DateTime Implements IOccurs.GetNextOccuranceDay

  End Class

#End Region

#Region " Daily "

  <Serializable()> _
  Public Class OccursDaily
    Inherits Occurs(Of OccursDaily)


    Public Shared DayIntervalProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DayInterval, "Day Interval", 1)

    Public Property DayInterval() As Integer
      Get
        Return GetProperty(DayIntervalProperty)
      End Get
      Set(value As Integer)
        SetProperty(DayIntervalProperty, value)
      End Set
    End Property


    Public Sub New(ByVal DayInterval As Integer)
      Me.Type = OccursType.Daily
      Me.DayInterval = DayInterval
    End Sub

    Public Sub New()

    End Sub

    Public Overrides Function GetNextOccuranceDay(ByVal FromDate As Date, ByVal DailyFrequency As IDailyFrequency) As Date

      ' find the next day after this day that we can occur, all we must do is add the day interval
      Return DailyFrequency.GetTimeOfDay(FromDate.AddDays(Me.DayInterval - 1))

    End Function

    Public Overrides Function ToString() As String

      Return "every" & IIf(Me.DayInterval = 1, "", " " & Me.DayInterval) & " Day" & IIf(Me.DayInterval > 1, "s", "")

    End Function

  End Class

#End Region

#Region " Weekly "

  <Serializable()> _
  Public Class OccursWeekly
    Inherits Occurs(Of OccursWeekly)


    Public Shared WeekIntervalProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.WeekInterval, "WeekInterval", 1)

    Public Property WeekInterval() As Integer
      Get
        Return GetProperty(WeekIntervalProperty)
      End Get
      Set(value As Integer)
        SetProperty(WeekIntervalProperty, value)
      End Set
    End Property

    Public Shared MondayProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Monday, "Monday", False)

    Public Property Monday() As Boolean
      Get
        Return GetProperty(MondayProperty)
      End Get
      Set(value As Boolean)
        SetProperty(MondayProperty, value)
      End Set
    End Property

    Public Shared TuesdayProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Tuesday, "Tuesday", False)

    Public Property Tuesday() As Boolean
      Get
        Return GetProperty(TuesdayProperty)
      End Get
      Set(value As Boolean)
        SetProperty(TuesdayProperty, value)
      End Set
    End Property

    Public Shared WednesdayProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Wednesday, "Wednesday", False)

    Public Property Wednesday() As Boolean
      Get
        Return GetProperty(WednesdayProperty)
      End Get
      Set(value As Boolean)
        SetProperty(WednesdayProperty, value)
      End Set
    End Property

    Public Shared ThursdayProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Thursday, "Thursday", False)

    Public Property Thursday() As Boolean
      Get
        Return GetProperty(ThursdayProperty)
      End Get
      Set(value As Boolean)
        SetProperty(ThursdayProperty, value)
      End Set
    End Property

    Public Shared FridayProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Friday, "Friday", False)

    Public Property Friday() As Boolean
      Get
        Return GetProperty(FridayProperty)
      End Get
      Set(value As Boolean)
        SetProperty(FridayProperty, value)
      End Set
    End Property

    Public Shared SaturdayProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Saturday, "Saturday", False)

    Public Property Saturday() As Boolean
      Get
        Return GetProperty(SaturdayProperty)
      End Get
      Set(value As Boolean)
        SetProperty(SaturdayProperty, value)
      End Set
    End Property

    Public Shared SundayProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Sunday, "Sunday", False)

    Public Property Sunday() As Boolean
      Get
        Return GetProperty(SundayProperty)
      End Get
      Set(value As Boolean)
        SetProperty(SundayProperty, value)
      End Set
    End Property

    Public Overrides Function ToString() As String

      Dim sReturn As String
      Dim bOneAdded As Boolean = False
      sReturn = "every" & IIf(Me.WeekInterval = 1, "", " " & Me.WeekInterval) & " Week" & IIf(Me.WeekInterval > 1, "s", "") & " on "

      For Each day As DayOfWeek In New DayOfWeek() {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday}
        If IsValidDay(day) Then
          If bOneAdded Then
            sReturn &= ", "
          End If
          sReturn &= day.ToString
          bOneAdded = True
        End If
      Next

      Return sReturn

    End Function

    Public Sub New(ByVal WeekInterval As Integer, ByVal Monday As Boolean, _
                                  ByVal Tuesday As Boolean, _
                                  ByVal Wednesday As Boolean, _
                                  ByVal Thursday As Boolean, _
                                  ByVal Friday As Boolean, _
                                  ByVal Saturday As Boolean, _
                                  ByVal Sunday As Boolean)
      Me.Type = OccursType.Weekly
      Me.WeekInterval = WeekInterval
      Me.Monday = Monday
      Me.Tuesday = Tuesday
      Me.Wednesday = Wednesday
      Me.Thursday = Thursday
      Me.Friday = Friday
      Me.Saturday = Saturday
      Me.Sunday = Sunday
    End Sub

    Public Sub New()

    End Sub

    Public Function IsValidDay(ByVal DayOfWeek As DayOfWeek) As Boolean

      Select Case DayOfWeek
        Case DayOfWeek.Monday
          Return Me.Monday
        Case DayOfWeek.Tuesday
          Return Me.Tuesday
        Case DayOfWeek.Wednesday
          Return Me.Wednesday
        Case DayOfWeek.Thursday
          Return Me.Thursday
        Case DayOfWeek.Friday
          Return Me.Friday
        Case DayOfWeek.Saturday
          Return Me.Saturday
        Case DayOfWeek.Sunday
          Return Me.Sunday
        Case Else
          Return False
      End Select

    End Function

    Public Overrides Function GetNextOccuranceDay(ByVal FromDate As Date, ByVal DailyFrequency As IDailyFrequency) As Date

      Dim dtReturn As DateTime = DailyFrequency.GetTimeOfDay(FromDate.AddDays((WeekInterval - 1) * 7))

      While Not IsValidDay(dtReturn.DayOfWeek)
        dtReturn = dtReturn.AddDays(1)
      End While

      If dtReturn.Date <> FromDate.Date Then
        'if the next run date is after today, set the start time to the start time of the daily schedule.
        Dim StartTime As Date = DailyFrequency.GetStartTime
        dtReturn = New Date(dtReturn.Year, dtReturn.Month, dtReturn.Day, StartTime.Hour, StartTime.Minute, StartTime.Second)
      End If

      Return dtReturn

    End Function

  End Class

#End Region

#Region " Monthly "

#Region " Monthly Base Class "

  Public Interface IOccursMonthly
    Inherits IOccurs

  End Interface

  <Serializable()> _
  Public MustInherit Class OccursMonthly(Of T As OccursMonthly(Of T))
    Inherits Occurs(Of T)
    Implements IOccursMonthly


    Public Shared MonthIntervalProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.MonthInterval, "Month Interval", 1)

    Public Property MonthInterval() As Integer
      Get
        Return GetProperty(MonthIntervalProperty)
      End Get
      Set(value As Integer)
        SetProperty(MonthIntervalProperty, value)
      End Set
    End Property

    Public Shared MonthlyTypeProperty As PropertyInfo(Of OccursMonthlyType) = RegisterProperty(Of OccursMonthlyType)(Function(c) c.MonthlyType)

    Public Property MonthlyType() As OccursMonthlyType
      Get
        Return GetProperty(MonthlyTypeProperty)
      End Get
      Set(value As OccursMonthlyType)
        SetProperty(MonthlyTypeProperty, value)
      End Set
    End Property

    Protected Sub New(ByVal MonthInterval As Integer)
      Me.Type = OccursType.Monthly
      Me.MonthInterval = MonthInterval
    End Sub

  End Class

#End Region

  <Serializable()> _
  Public Class OccursMonthlyDay
    Inherits OccursMonthly(Of OccursMonthlyDay)

    Public Shared DayProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Day, "Day", 1)

    <System.ComponentModel.DataAnnotations.RangeAttribute(1, 31, ErrorMessage:="Must be in Range 1 - 31")>
    Public Property Day() As Integer
      Get
        Return GetProperty(DayProperty)
      End Get
      Set(value As Integer)
        SetProperty(DayProperty, value)
      End Set
    End Property

    Public Overrides Function ToString() As String

      Return "on day " & Me.Day & " of " & IIf(Me.MonthInterval > 1, "every " & Me.MonthInterval, "each") & " Month" & IIf(Me.MonthInterval > 1, "s", "")

    End Function

    Public Sub New(ByVal MonthInterval As Integer, ByVal Day As Integer)
      MyBase.New(MonthInterval)
      Me.MonthlyType = OccursMonthlyType.Day
      Me.Day = Day
    End Sub

    Public Sub New()

      MyBase.New(1)

    End Sub

    Public Overrides Function GetNextOccuranceDay(ByVal FromDate As Date, ByVal DailyFrequency As IDailyFrequency) As Date

      Dim dtReturn As DateTime = DailyFrequency.GetTimeOfDay(FromDate.AddMonths(Me.MonthInterval - 1))
      ' check if this date is actually the number of months ahead
      While dtReturn < FromDate.AddMonths(Me.MonthInterval - 1)
        ' nope, its not right, so add another month
        dtReturn = dtReturn.AddMonths(1)
      End While
      ' now just set the right day
      If Me.Day > DateTime.DaysInMonth(dtReturn.Year, dtReturn.Month) Then
        ' make last day of month
        dtReturn = dtReturn.AddDays(DateTime.DaysInMonth(dtReturn.Year, dtReturn.Month) - dtReturn.Day)
      ElseIf Me.Day > dtReturn.Day Then
        dtReturn = dtReturn.AddDays(Me.Day - dtReturn.Day)
      ElseIf Me.Day < dtReturn.Day Then
        ' the day we want is less than the day of the date that we have, so move forwards until the day is the same
        While Me.Day <> dtReturn.Day
          dtReturn = dtReturn.AddDays(1)
        End While
      End If
      Return dtReturn

    End Function

  End Class

  <Serializable()> _
  Public Class OccursMonthlyThe
    Inherits OccursMonthly(Of OccursMonthlyThe)


    Public Shared TheDayProperty As PropertyInfo(Of DayNumeration) = RegisterProperty(Of DayNumeration)(Function(c) c.TheDay, "TheDay", DayNumeration.First)

    <Singular.DataAnnotations.DropDownWeb(GetType(DayNumeration))>
    Public Property TheDay() As DayNumeration
      Get
        Return GetProperty(TheDayProperty)
      End Get
      Set(value As DayNumeration)
        SetProperty(TheDayProperty, value)
      End Set
    End Property


    Public Shared TheDayOfWeekProperty As PropertyInfo(Of DaysOfWeek) = RegisterProperty(Of DaysOfWeek)(Function(c) c.TheDayOfWeek, "TheDayOfWeek", DaysOfWeek.Monday)

    <Singular.DataAnnotations.DropDownWeb(GetType(DaysOfWeek))>
    Public Property TheDayOfWeek() As DaysOfWeek
      Get
        Return GetProperty(TheDayOfWeekProperty)
      End Get
      Set(value As DaysOfWeek)
        SetProperty(TheDayOfWeekProperty, value)
      End Set
    End Property

    Public Overrides Function ToString() As String

      Dim sReturn As String = "on the "
      sReturn &= CommonData.Enums.Description(Me.TheDay) & " "
      sReturn &= CommonData.Enums.Description(Me.TheDayOfWeek) & " of " & IIf(Me.MonthInterval > 1, "every " & Me.MonthInterval, "each") & " Month" & IIf(Me.MonthInterval > 1, "s", "")
      Return sReturn

    End Function


    Public Sub New(ByVal MonthInterval As Integer, ByVal TheDay As Integer, ByVal TheDayOfWeek As DaysOfWeek)
      MyBase.New(MonthInterval)
      Me.MonthlyType = OccursMonthlyType.The
      Me.TheDay = TheDay
      Me.TheDayOfWeek = TheDayOfWeek
    End Sub

    Public Sub New()

      MyBase.New(1)

    End Sub

    Private Function IsDay(ByVal TheDate As DateTime) As Boolean

      If CommonData.Enums.Description(Me.TheDayOfWeek) = "Day" Then
        Return True
      ElseIf CommonData.Enums.Description(Me.TheDayOfWeek) = "Weekend Day" Then
        If TheDate.DayOfWeek = DayOfWeek.Saturday Or TheDate.DayOfWeek = DayOfWeek.Sunday Then
          Return True
        Else
          Return False
        End If
      ElseIf CommonData.Enums.Description(Me.TheDayOfWeek) = "Weekday" Then
        If TheDate.DayOfWeek = DayOfWeek.Saturday Or TheDate.DayOfWeek = DayOfWeek.Sunday Then
          Return False
        Else
          Return True
        End If
      Else
        ' must be one of the week days
        If TheDate.DayOfWeek.ToString() = CommonData.Enums.Description(Me.TheDayOfWeek) Then
          Return True
        End If
      End If
      Return False

    End Function

    Public Overrides Function GetNextOccuranceDay(ByVal FromDate As Date, ByVal DailyFrequency As IDailyFrequency) As Date

      Dim dtReturn As DateTime = DailyFrequency.GetTimeOfDay(FromDate.AddMonths(Me.MonthInterval - 1))
      ' now we have a date in the right month
      If Me.TheDay = DayNumeration.Last Then
        ' last day
        dtReturn = DailyFrequency.GetTimeOfDay(dtReturn.AddDays(DateTime.DaysInMonth(dtReturn.Year, dtReturn.Month) - dtReturn.Day))
        ' now work backward till we find the right day
        While Not IsDay(dtReturn) Or dtReturn < FromDate
          dtReturn = DailyFrequency.GetTimeOfDay(DailyFrequency.GetTimeOfDay(dtReturn.AddDays(-1)))
          ' but if the new date is less than the propesed months forward then move it forward another month
          If Math.Abs(DateDiff(DateInterval.Month, FromDate, dtReturn)) < Me.MonthInterval - 1 Or dtReturn < FromDate Then
            dtReturn = dtReturn.AddMonths(1)
          End If
        End While
        Return dtReturn
      Else
        Dim iCount As Integer = 1
        ' set the date to the first day of the month
        dtReturn = DailyFrequency.GetTimeOfDay(dtReturn.AddDays(-(dtReturn.Day - 1)))
        ' now work forward counting the correct days until we have a match
        While Not IsDay(dtReturn) Or iCount <= Me.TheDay Or dtReturn < FromDate
          dtReturn = DailyFrequency.GetTimeOfDay(dtReturn.AddDays(1))
          If IsDay(dtReturn) Then
            iCount += 1
          End If
          If iCount > Me.TheDay + 1 Then
            ' we are over the correct day in this month, make it next month
            dtReturn = DailyFrequency.GetTimeOfDay(dtReturn.AddMonths(1).AddDays(-(dtReturn.Day - 1)))
            iCount = 1
          End If

          If dtReturn.Date.Equals(FromDate.Date) Then
            ' the dates are the same so make sure the time goes after todays time
            dtReturn = DailyFrequency.GetTimeOfDay(dtReturn.Date.AddTicks(FromDate.TimeOfDay.Ticks))
          End If
        End While
        Return dtReturn
      End If

    End Function

  End Class

#End Region

End Namespace
