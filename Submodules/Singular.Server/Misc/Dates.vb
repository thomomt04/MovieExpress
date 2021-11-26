''' <summary>
''' Provides methods for manipulating dates
''' </summary>
''' <remarks></remarks>
Public Class Dates
''' <summary>
''' Method to calculates the difference in month between two dates
''' </summary>
''' <param name="Date1">Date value of the date the months will be calculated for</param>
''' <param name="Date2">Date value of the date for the calculation</param>
''' <returns>Integer with the difference in month value</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the MonthDifference Method
''' <code>
''' DateTime date1 = new DateTime(2016, 02, 24);
''' DateTime date2 = new DateTime(2017, 02, 27);
''' int monthDifference = Singular.Dates.MonthDifference(date1, date2);
''' </code>
''' </example>
	Public Shared Function MonthDifference(Date1 As Date, Date2 As Date) As Integer

		Dim MonthDiff As Integer = DateDiff(DateInterval.Month, Date1, Date2) - 1

		While Date1.AddMonths(MonthDiff + 1) < Date2
			MonthDiff += 1
		End While

		Return MonthDiff

	End Function
''' <summary>
'''  Method to calculates the difference in month between two dates
''' </summary>
''' <param name="Date1">Date value of the date the months will be calculated for</param>
''' <param name="Date2">Date value of the date for the calculation</param>
''' <returns>Decimal value with the difference in month value</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the MonthDifferencePrecise Method
''' <code>
''' DateTime date1 = new DateTime(2016, 02, 24);
''' DateTime date2 = new DateTime(2017, 02, 27);
''' decimal monthDifference = Singular.Dates.MonthDifferencePrecise(date1, date2);
''' </code>
'''</example>
	Public Shared Function MonthDifferencePrecise(Date1 As Date, Date2 As Date) As Decimal

		Dim MonthDiff As Decimal = DateDiff(DateInterval.Month, Date1, Date2) - 1

		While Date1.AddMonths(MonthDiff + 1) < Date2
			MonthDiff += 1
		End While

		Dim Days As Integer = 0
		While Date1.AddMonths(MonthDiff).AddDays(Days) < Date2
			Days += 1
		End While

		Return MonthDiff + (Days / (365.25 / 12))

	End Function

	Public Shared Function SmartDateEqualsValue(ByVal SmartDate As Csla.SmartDate, ByVal Value As Object) As Boolean

		If IsNothing(Value) OrElse IsDBNull(Value) OrElse (TypeOf Value Is String AndAlso Value = "") Then
			If SmartDate.IsEmpty Then
				' they are the same
				Return True
			Else
				Return False
			End If
		Else
			' Value has a value
			If SmartDate.IsEmpty Then
				Return Value = DateTime.MinValue
			Else
				Dim oValue As DateTime = DateTime.MinValue
				If TypeOf Value Is String Then
					oValue = DateTime.Parse(Value)
				ElseIf TypeOf Value Is Nullable(Of DateTime) Then
					oValue = Value
				Else
					oValue = Value
				End If

				Return oValue.Equals(SmartDate.Date)
			End If
		End If

	End Function

	Public Shared Function GetFormatString(ByVal OfTimeSpan As TimeSpan) As String

		If OfTimeSpan.TotalDays >= 365 Then
			Return "yyyy"
		ElseIf OfTimeSpan.TotalDays >= 25 Then
			Return "MM/yyyy"
		ElseIf OfTimeSpan.TotalHours >= 20 Then
			Return "dd/MM/yy"
		ElseIf OfTimeSpan.TotalHours >= 1 Then
			Return "HH:mm"
		ElseIf OfTimeSpan.TotalSeconds >= 5 Then
			Return "HH:mm"
		ElseIf OfTimeSpan.TotalSeconds >= 1 Then
			Return "HH:mm:ss"
		Else
			Return "HH:mm:ss.fff"
		End If

	End Function


	Public Shared Function GetDetailFormatString(ByVal OfTimeSpan As TimeSpan) As String

		If OfTimeSpan.TotalHours >= 20 Then
			Return "dd-MMM-yyyy"
		ElseIf OfTimeSpan.TotalHours >= 1 Then
			Return "dd-MMM HH:mm"
		ElseIf OfTimeSpan.TotalSeconds >= 5 Then
			Return "dd-MMM HH:mm"
		ElseIf OfTimeSpan.TotalSeconds >= 1 Then
			Return "dd-MMM HH:mm:ss"
		Else
			Return "dd-MMM HH:mm:ss.fff"
		End If

	End Function

	Public Shared Function GetFormatStringWithDay(ByVal OfTimeSpan As TimeSpan) As String

		If OfTimeSpan.TotalDays >= 365 Then
			Return "yyyy"
		ElseIf OfTimeSpan.TotalDays >= 25 Then
			Return "MM/yyyy"
		ElseIf OfTimeSpan.TotalHours >= 20 Then
			Return "dd/MM/yy"
		ElseIf OfTimeSpan.TotalHours >= 1 Then
			Return "ddd dd-MMM HH:mm"
		ElseIf OfTimeSpan.TotalSeconds >= 5 Then
			Return "ddd dd-MMM HH:mm"
		ElseIf OfTimeSpan.TotalSeconds >= 1 Then
			Return "ddd dd-MMM HH:mm:ss"
		Else
			Return "ddd dd-MMM HH:mm:ss.fff"
		End If

	End Function
	Public Shared Function FormatTimeSpan(ByVal TimeSpan As TimeSpan, Optional ByVal UseNewLines As Boolean = False, Optional ByVal StopAtSeconds As Boolean = True, Optional ByVal StopAtMinutes As Boolean = False, Optional ByVal StopAtHours As Boolean = False) As String

		Dim sReturn As String = ""
		If TimeSpan.Days <> 0 Then
			sReturn &= Strings.Pluralize(TimeSpan.Days, "Day")
			If UseNewLines Then
				sReturn &= vbCrLf
			Else
				sReturn &= ", "
			End If
		End If

		If TimeSpan.Hours <> 0 Then
			sReturn &= Strings.Pluralize(TimeSpan.Hours, "Hour")
			If UseNewLines Then
				sReturn &= vbCrLf
			ElseIf Not StopAtHours Then
				sReturn &= ", "
			End If
		End If

		If (TimeSpan.Minutes <> 0 AndAlso Not StopAtHours) OrElse String.IsNullOrEmpty(sReturn) Then
			sReturn &= Strings.Pluralize(TimeSpan.Minutes, "Min")
			If UseNewLines Then
				sReturn &= vbCrLf
			ElseIf Not StopAtHours AndAlso Not StopAtMinutes Then
				sReturn &= ", "
			End If
		End If



		If (Not StopAtSeconds AndAlso Not StopAtHours AndAlso Not StopAtMinutes) OrElse String.IsNullOrEmpty(sReturn) Then
			If TimeSpan.Seconds <> 0 Then
				sReturn &= Strings.Pluralize(TimeSpan.Seconds, "Sec")
				If UseNewLines Then
					sReturn &= vbCrLf
				Else
					sReturn &= ", "
				End If
			End If
			If TimeSpan.Milliseconds <> 0 Then
				sReturn &= (CDec(TimeSpan.Milliseconds) + ((TimeSpan.Ticks Mod TimeSpan.TicksPerMillisecond) / TimeSpan.TicksPerMillisecond)).ToString("#,##0.#") & " ms"
			End If
			'Else
			'  sReturn &= Strings.Pluralize(TimeSpan.Seconds, "Sec")
		End If

		If String.IsNullOrEmpty(sReturn.Trim) Then
			If Not StopAtSeconds AndAlso Not StopAtHours AndAlso Not StopAtMinutes Then
				sReturn = "Less than 1 ms"
			Else
				sReturn = ""
			End If
		End If

		Return sReturn

	End Function
''' <summary>
''' Method to calculated age for the passed in date of birth
''' </summary>
''' <param name="BirthDate">Date value for Date of birth</param>
''' <returns>Integer value with age</returns>
''' <remarks></remarks>
	Public Shared Function GetAgeFromDate(BirthDate As Date) As Integer

		Dim today = DateTime.Today
		Dim age = today.Year - BirthDate.Year
		If (BirthDate > today.AddYears(-age)) Then age -= 1
		Return age

	End Function
''' <summary>
''' Method to calculates the first date of the month
''' </summary>
''' <param name="ReferenceDate">Date value for the date you want the first date of the month</param>
''' <returns>Date value for first date of the month</returns>
''' <remarks></remarks>
	Public Shared Function DateMonthStart(ByVal ReferenceDate As Date) As Date

		Return New Date(ReferenceDate.Year, ReferenceDate.Month, 1)

	End Function
''' <summary>
''' Method to calculates the last date of the month
''' </summary>
''' <param name="ReferenceDate">Date value for the date you want the end date of the month </param>
''' <param name="ToLastSecond">Must alwayds be false as true will throw an exception: Throw New NotSupportedException("To Last Second Not Supported")</param>
''' <returns>Date value for the last date of the month</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the DateMonthEnd Method
''' <code>
''' DateTime DateParam = new DateTime(2017, 01, 15);
''' DateTime MonthEndDate = Singular.Dates.DateMonthEnd(DateParam);
''' </code>
'''</example>
	Public Shared Function DateMonthEnd(ByVal ReferenceDate As Date, Optional ByVal ToLastSecond As Boolean = False) As Date

		If ToLastSecond Then
			Throw New NotSupportedException("To Last Second Not Supported")
		End If
		Return New Date(ReferenceDate.Year, ReferenceDate.Month, DateTime.DaysInMonth(ReferenceDate.Year, ReferenceDate.Month))

	End Function

	' The only reason I have placed the SILVERLIGHT directive around this code is to minimize the size of the XAP file.  If functionality is neede in SL 
	' please just move it out of the Non SL code
#If SILVERLIGHT Then
#Else

	''' <summary>
	''' This function will give you an accurate decimal year difference between the 2 dates (taking into account leap years)
	''' </summary>
	''' <param name="Date1"></param>
	''' <param name="Date2"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function YearDiff(ByVal Date1 As DateTime, ByVal Date2 As DateTime) As Decimal

		Dim StartDate As DateTime, EndDate As DateTime
		If Date2 > Date1 Then
			StartDate = Date1
			EndDate = Date2
		Else
			StartDate = Date2
			EndDate = Date1
		End If

		Dim Diff As Decimal = 0

		While StartDate < EndDate
			If DateAdd(DateInterval.Year, 1, StartDate) > EndDate Then
				' startdate is less than a year from end date, add the difference
				Dim Days As Integer = DateDiff(DateInterval.Day, StartDate, EndDate) + 1
				' need to see if the portion of the year used includes a leap year
				Dim ExtraDays As Integer = 0
				If (StartDate.Month <= 2 And EndDate.Month > 2) Then
					' year includes february, so might have the extra day
					ExtraDays = DateTime.DaysInMonth(StartDate.Year, 2) - 28
				ElseIf (StartDate.Month > 2 And EndDate.Month > 2 And EndDate.Year > StartDate.Year) Then
					' year includes february, so might have the extra day
					ExtraDays = DateTime.DaysInMonth(EndDate.Year, 2) - 28
				End If

				Diff += Days / (365 + ExtraDays)
			Else
				' end date is more than a year away, so add 1
				Diff += 1
			End If
			EndDate = DateAdd(DateInterval.Year, -1, EndDate)
		End While
		If Date1 > Date2 Then
			Return -Diff
		Else
			Return Diff
		End If

	End Function
''' <summary>
''' Returns the South African Tax year
''' </summary>
''' <param name="OfDate">Date the tax year must be calucalted against</param>
''' <returns>Integer value of the Tax year</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the GetSATaxYear Method
''' <code>
''' DateTime DateParam = new DateTime(2016, 02, 24);
''' int taxyear = Singular.Dates.GetSATaxYear(DateParam);
''' </code>
''' </example>
	Public Shared Function GetSATaxYear(ByVal OfDate As DateTime) As Integer

		If OfDate.Month > 2 Then
			Return OfDate.Year + 1
		Else
			Return OfDate.Year
		End If

	End Function
''' <summary>
''' Converts the specified string representation of a date and time to its System.DateTime equivalent using the specified format and culture-specific format information. The format of the string representation must match the specified format exactly.
''' </summary>
''' <param name="DateString">A string that contains a date and time to convert.</param>
''' <param name="FormatString">A format specifier that defines the required format of DateString</param>
''' <returns>An object that is equivalent to the date and time contained in s, as specified by format and provider.</returns>
''' <remarks></remarks>
	Public Shared Function Parse(ByVal DateString As String, ByVal FormatString As String) As DateTime

		If DateString.Length = 0 Then
			Return DateTime.MinValue
		End If
		Return DateTime.ParseExact(DateString, FormatString, New Globalization.DateTimeFormatInfo)

	End Function
''' <summary>
''' Method to calculates the first day of the week that falls on a Monday or System.DayOfWeek if passed
''' </summary>
''' <param name="ReferenceDate">Date value to check the start of the week agains</param>
''' <param name="FirstDayOfWeek">Optional param to specify the Firstday of the week default is Monday</param>
''' <returns>Date value for the first day of week for passed in date</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the DateMonthEnd Method
''' <code>
''' DateTime DateParam = new DateTime(2017, 01, 15);
''' DateTime WeekStartDate = Singular.Dates.DateWeekStart(DateParam);
''' </code>
'''</example>
	Public Shared Function DateWeekStart(ByVal ReferenceDate As Date, Optional FirstDayOfWeek As System.DayOfWeek = DayOfWeek.Monday) As Date

		Return DateBackUntil(ReferenceDate, FirstDayOfWeek)

	End Function
''' <summary>
''' Method to calculates the date for the next day that falls on a Saturday
''' </summary>
''' <param name="ReferenceDate">Date value to check the end of the week against</param>
''' <param name="ToLastSecond">Default to false this param is not being used in the Method</param>
''' <returns>Date value for the next day that falls on a Saturday</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the DateWeekEnd Method
''' <code>
''' DateTime DateParam = new DateTime(2017, 01, 15);
''' DateTime WeekEndDate = Singular.Dates.DateWeekEnd(DateParam);
''' </code>
'''</example>
	Public Shared Function DateWeekEnd(ByVal ReferenceDate As Date, Optional ByVal ToLastSecond As Boolean = False) As Date

		Return DateForwardUntil(ReferenceDate, System.DayOfWeek.Saturday)

	End Function

	Private Shared Function DateBackUntil(ByVal ReferenceDate As Date, ByVal Day As System.DayOfWeek)

		If ReferenceDate.DayOfWeek = Day Then
			Return ReferenceDate
		Else
			Return DateBackUntil(ReferenceDate.AddDays(-1), Day)
		End If

	End Function

	Public Shared Function DateForwardUntil(ByVal ReferenceDate As Date, ByVal Day As System.DayOfWeek)

		If ReferenceDate.DayOfWeek = Day Then
			Return ReferenceDate
		Else
			Return DateForwardUntil(ReferenceDate.AddDays(1), Day)
		End If

	End Function
''' <summary>
''' Method to calculates the start date of the year
''' </summary>
''' <param name="ReferenceDate">Date value for the date you want the start of the year calculated against</param>
''' <returns>date value of start of the year</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the DateYearStart Method
''' <code>
''' DateTime DateParam = new DateTime(2017, 06, 22);
''' DateTime YearStartDate = Singular.Dates.DateYearStart(DateParam);
''' </code>
'''</example>
	Public Shared Function DateYearStart(ByVal ReferenceDate As Date) As Date

		Return New Date(ReferenceDate.Year, 1, 1)

	End Function
''' <summary>
''' Method to calculates the end date of the year
''' </summary>
''' <param name="ReferenceDate">Date value for the date you want the end of the year calculated against</param>
''' <param name="ToLastSecond">Must alwayds be false as true will throw an exception: Throw New NotSupportedException("To Last Second Not Supported")</param>
''' <returns>date value of end of the year</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the DateYearEnd Method
''' <code>
''' DateTime DateParam = new DateTime(2017, 06, 22);
''' DateTime YearEndDate = Singular.Dates.DateYearEnd(DateParam);
''' </code>
'''</example>
	Public Shared Function DateYearEnd(ByVal ReferenceDate As Date, Optional ByVal ToLastSecond As Boolean = False) As Date

		If ToLastSecond Then
			Throw New NotSupportedException("To Last Second Not Supported")
		End If
		Return New Date(ReferenceDate.Year, 12, 31)

	End Function

	''' <summary>
	''' This function will return the financial month of the DateToCheck based on the FinYearMonthStart
	''' </summary>
	''' <param name="FinYearMonthStart">Starting calendar month of the financial year</param>
	''' <param name="DateToCheck">The date of which the function must return the financial month</param>
	''' <returns>Integer value of the financial month corresponding to the calendar DateToCheck</returns>
	''' <remarks></remarks>
	Public Shared Function GetFinMonth(ByVal FinYearMonthStart As Integer, ByVal DateToCheck As DateTime) As Integer

		Dim FinYearStart As DateTime
		If FinYearMonthStart <= DateToCheck.Month Then
			' fin year started earlier in the same year
			FinYearStart = New DateTime(DateToCheck.Year, FinYearMonthStart, 1)
		Else
			' fin year starts later in the same year, therefore this date is in the following year
			FinYearStart = New DateTime(DateToCheck.Year - 1, FinYearMonthStart, 1)
		End If
		Dim FinMonth As Integer = 1
		While FinYearStart.Month <> DateToCheck.Month
			FinYearStart = FinYearStart.AddMonths(1)
			FinMonth += 1
		End While
		Return FinMonth

	End Function

	''' <summary>
	''' Returns the Financial year of the DateToCheck according to the FinYearMonthStart
	''' </summary>
	''' <param name="FinYearMonthStart">First Month of the Financial Year</param>
	''' <param name="DateToCheck">Date to check which financial year</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function GetFinYear(ByVal FinYearMonthStart As Integer, ByVal DateToCheck As DateTime) As Integer

		If FinYearMonthStart <= DateToCheck.Month Then
			' fin year started earlier in the same year
			Return DateToCheck.Year + 1
		Else
			' fin year starts later in the same year, therefore this date is in the following year
			Return DateToCheck.Year
		End If

	End Function

	''' <summary>
	''' This function will return the 1st date of the calendar month of the FinMonth and FinYear based on the FinYearMonthStart
	''' </summary>
	''' <param name="FinYearMonthStart">Starting calendar month of the financial year</param>
	''' <param name="FinYear">The current financial year</param>
	''' <param name="FinMonth">The fin month you want the calendar date for</param>
	''' <returns>Integer value of the financial month corresponding to the calendar DateToCheck</returns>
	''' <remarks></remarks>
	Public Shared Function GetCalMonth(ByVal FinYearMonthStart As Integer, ByVal FinYear As Integer, ByVal FinMonth As Integer) As DateTime

		If FinYearMonthStart = 1 Then
			Return New DateTime(FinYear, FinMonth, 1)
		Else
			Return New DateTime(FinYear, FinYearMonthStart + FinMonth + 1 Mod 12, 1)
		End If

	End Function
''' <summary>
''' Methord that calculates the date for the day of the week passed in either before or after a specified date.
''' </summary>
''' <param name="FromDate">Date used for calculation</param>
''' <param name="DayOfWeek">Day of the week date is calculated for</param>
''' <param name="Before">If Before = true it gets the date after specified date, when false gets the date before. Default to false.</param>
''' <returns>A datetime with the calucated day of the week</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the DateYearEnd Method
''' <code>
''' DateTime date1 = new DateTime(2017, 02, 23);
''' DayOfWeek day = DayOfWeek.Friday;
''' DateTime firstDay = Singular.Dates.GetFirstDay(date1, day);
''' </code>
'''</example>
	Public Shared Function GetFirstDay(ByVal FromDate As DateTime, ByVal DayOfWeek As DayOfWeek, Optional ByVal Before As Boolean = True) As DateTime

		While FromDate.DayOfWeek <> DayOfWeek
			FromDate = FromDate.AddDays(IIf(Before, 1, -1))
		End While
		Return FromDate

	End Function
''' <summary>
''' Methord that calculates the date for the day of the week passed in either before or after a specified date.
''' </summary>
''' <param name="Year">Year used for calculation</param>
''' <param name="Month">Month used for calculation</param>
''' <param name="Day">Day used for calculation</param>
''' <param name="DayOfWeek">Day of the week date is calculated for</param>
''' <param name="Before">If Before = true it gets the date after specified date, when false gets the date before. Default to false.</param>
''' <returns>A datetime with the calucated day of the week</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the DateYearEnd Method
''' <code>
''' DayOfWeek day = DayOfWeek.Friday;
''' DateTime firstDay = Singular.Dates.GetFirstDay(2017,02,23, day);
''' </code>
'''</example>

	Public Shared Function GetFirstDay(ByVal Year As Integer, ByVal Month As Integer, ByVal Day As Integer, ByVal DayOfWeek As DayOfWeek, Optional ByVal Before As Boolean = True) As DateTime

		Return GetFirstDay(New DateTime(Year, Month, Day), DayOfWeek, Before)

	End Function

	Public Shared Function WeekDaysIntToString(ByVal Days As Integer, Optional ByVal Abbreviate As Boolean = False, _
		Optional ByVal FirstDayOfweek As Microsoft.VisualBasic.FirstDayOfWeek = Microsoft.VisualBasic.FirstDayOfWeek.Monday) As String
		' Takes in integer of days and extracts them to descriptive string
		Dim sDays As String = Days
		Dim sResult As System.Text.StringBuilder = New System.Text.StringBuilder
		For i As Integer = 0 To sDays.Length - 1
			sResult.Append(WeekdayName(CInt(sDays.Substring(i, 1)), Abbreviate, FirstDayOfweek) & ", ")
		Next
		' Take out last comma
		sResult.Remove(sResult.ToString.Length - 2, 2)
		' Replace last comma with and
		If sDays.Length > 1 Then
			sResult.Replace(",", " And", sResult.ToString.LastIndexOf(","), 1)
		End If
		Return sResult.ToString
	End Function


	Public Class MonthOption

		Public Value As Integer
		Public Month As String

		Public Sub New(Value As Integer, Month As String)

			Me.Value = Value
			Me.Month = Month

		End Sub

	End Class

	''' <summary>
	''' Returns a value list of all the months in a year, starting at a specified month.
	''' Contains the Index of the Month, and the Name of the Month
	''' </summary>
	''' <param name="StartMonth">Start Month (January = 1)</param>
	Public Shared Function GetMonthValueList(ByVal StartMonth As Integer) As List(Of MonthOption)

		Dim vl As New List(Of MonthOption)

		For i As Integer = 0 To 11
			vl.Add(New MonthOption(((i + StartMonth - 1) Mod 12) + 1, New Date(2000, ((i + StartMonth - 1) Mod 12) + 1, 1).ToString("MMMM")))
		Next

		Return vl

	End Function

	''' <summary>
	''' Returns a value list of years.
	''' The value and display are the same
	''' </summary>
	''' <param name="StartYear">Which year to start at</param>
	''' <param name="NoOfYears">No Of Years (to include backwards)</param>
	Public Shared Function GetYearValueList(ByVal StartYear As Integer, ByVal NoOfYears As Integer) As List(Of Integer)
		Dim vl As New List(Of Integer)
		For i As Integer = StartYear To StartYear - NoOfYears Step -1
			vl.Add(i)
		Next
		Return vl
	End Function

	''' <summary>
	''' Returns an Integer list of years.
	''' The value and display are the same
	''' </summary>
	''' <param name="StartYear">Which year to start at</param>
	''' <param name="NoOfYears">No Of Years (to include backwards)</param>
	Public Shared Function GetYearList(ByVal StartYear As Integer, ByVal NoOfYears As Integer) As List(Of Integer)
		Dim vl As New List(Of Integer)
		For i As Integer = StartYear To StartYear - NoOfYears Step -1
			vl.Add(i)
		Next
		Return vl
	End Function

  ''' <summary>
  ''' If you have a month string. e.g 'March' and you want 3 returned.. Use this function
  ''' </summary>
  ''' <param name="Month">The Full Month Name. e.g. January, Not Jan</param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function GetMonthIndexFromString(ByVal Month As String) As Integer
    Dim d As Date = Date.Parse("1 " & Month & " 2008")
    Return d.Month
  End Function

  ''' <summary>
  ''' Method to caluculate the next working day
  ''' </summary>
  ''' <param name="ReferenceDate">Date the previous working day is calculated against</param>
  ''' <returns>Date value of the previous working day</returns>
  ''' <remarks></remarks>
  Public Shared Function GetNextWorkingDay(ByVal ReferenceDate As Date) As Date

    Dim dt As Date = DateAdd(DateInterval.Day, 1, ReferenceDate)
    If dt.DayOfWeek = DayOfWeek.Saturday OrElse dt.DayOfWeek = DayOfWeek.Sunday Then
      Return GetNextWorkingDay(dt)
    Else
      Return dt
    End If

  End Function

  ''' <summary>
  ''' Method to caluculate the previous working day
  ''' </summary>
  ''' <param name="ReferenceDate">Date the previous working day is calculated against</param>
  ''' <returns>Date value of the previous working day</returns>
  ''' <remarks></remarks>
  Public Shared Function GetPreviousWorkingDay(ByVal ReferenceDate As Date) As Date

		Dim dt As Date = DateAdd(DateInterval.Day, -1, ReferenceDate)
		If dt.DayOfWeek = DayOfWeek.Saturday OrElse dt.DayOfWeek = DayOfWeek.Sunday Then
			Return GetPreviousWorkingDay(dt)
		Else
			Return dt
		End If

	End Function
''' <summary>
''' Method to calculates the min date between two dates
''' </summary>
''' <param name="Date1">Date value of the first date for comparison</param>
''' <param name="Date2">Date value of the second date for comparison</param>
''' <returns>Object type with the min date</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the Min Method
''' <code>
''' DateTime date1 = new DateTime(2017, 02, 24);
''' DateTime date2 = new DateTime(2017, 02, 27);
''' object minDate = Singular.Dates.Min(date1, date2);
''' </code>
'''</example>
	Public Shared Function Min(ByVal Date1 As Date, ByVal Date2 As Date)
		If Date1 < Date2 Then
			Return Date1
		Else
			Return Date2
		End If
	End Function
''' <summary>
''' Method to calculates the max date between two dates
''' </summary>
''' <param name="Date1">Date value of the first date for comparison</param>
''' <param name="date2">Date value of the second date for comparison</param>
''' <returns>Object type value with the max date</returns>
''' <remarks></remarks>
''' <example>
''' This a C# sample that shows how to call the Max Method
''' <code>
''' DateTime date1 = new DateTime(2017, 02, 24);
''' DateTime date2 = new DateTime(2017, 02, 27);
''' object maxDate = Singular.Dates.Max(date1, date2);
''' </code>
'''</example>
	Public Shared Function Max(ByVal Date1 As Date, ByVal date2 As Date)
		If Date1 > date2 Then
			Return Date1
		Else
			Return date2
		End If
	End Function

	''' <summary>
	''' Used for javascript date comparisons.
	''' </summary>
	Public Shared Function SafeCompare(cb As Func(Of Boolean)) As Boolean
		Return cb()
	End Function


#End If

End Class
