Imports Csla
Imports Csla.Serialization
Imports System.Threading

Namespace Service.Scheduling

  <Serializable()> _
  Public Class Schedule
    Inherits ServerProgramInfoBase(Of Schedule)

#Region " Properties "

    Public Function Occurs() As IOccurs
      Select Case Me.OccursType
        Case 0
          Return Me.OccursDaily
        Case 1
          Return Me.OccursWeekly
        Case 2
          Select Case Me.OccursMonthlyType
            Case Scheduling.OccursMonthlyType.Day
              Return Me.OccursMonthlyDay
            Case Scheduling.OccursMonthlyType.The
              Return Me.OccursMonthlyThe
          End Select
      End Select
      Return Nothing
    End Function

    Public Shared OccursMonthlyDayProperty As PropertyInfo(Of OccursMonthlyDay) = RegisterProperty(Of OccursMonthlyDay)(Function(c) c.OccursMonthlyDay)

    <Singular.DataAnnotations.ObjectProperty(True)>
    Public Property OccursMonthlyDay() As OccursMonthlyDay
      Get
        Return GetProperty(OccursMonthlyDayProperty)
      End Get
      Set(value As OccursMonthlyDay)
        SetProperty(OccursMonthlyDayProperty, value)
      End Set
    End Property

    Public Shared OccursMonthlyTheProperty As PropertyInfo(Of OccursMonthlyThe) = RegisterProperty(Of OccursMonthlyThe)(Function(c) c.OccursMonthlyThe)

    <Singular.DataAnnotations.ObjectProperty(True)>
    Public Property OccursMonthlyThe() As OccursMonthlyThe
      Get
        Return GetProperty(OccursMonthlyTheProperty)
      End Get
      Set(value As OccursMonthlyThe)
        SetProperty(OccursMonthlyTheProperty, value)
      End Set
    End Property

    Public Shared OccursMonthlyTypeProperty As PropertyInfo(Of Scheduling.OccursMonthlyType) = RegisterProperty(Of Scheduling.OccursMonthlyType)(Function(c) c.OccursMonthlyType, "OccursMonthlyType", Scheduling.OccursMonthlyType.Day)
#If SILVERLIGHT Then
    Public Property OccursMonthlyType() As Scheduling.OccursMonthlyType
      Get
        Return GetProperty(OccursMonthlyTypeProperty)
      End Get
      Set(value As Scheduling.OccursMonthlyType)
        If value <> Me.OccursMonthlyType Then
          SetProperty(OccursMonthlyTypeProperty, value)
          OnPropertyChanged("Occurs")
        End If
      End Set
    End Property
#Else
    <Singular.DataAnnotations.RadioButtonList(GetType(OccursMonthlyType))>
    Public Property OccursMonthlyType() As Scheduling.OccursMonthlyType
      Get
        Return GetProperty(OccursMonthlyTypeProperty)
      End Get
      Set(value As Scheduling.OccursMonthlyType)
        If value <> Me.OccursMonthlyType Then
          SetProperty(OccursMonthlyTypeProperty, value)
          OnPropertyChanged("Occurs")
        End If
      End Set
    End Property
#End If


    Public Shared OccursTypeProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.OccursType, "Occurs Type", 0)
#If SILVERLIGHT Then
    Public Property OccursType As Integer
      Get
        Return GetProperty(OccursTypeProperty)
      End Get
      Set(value As Integer)
        If value <> Me.OccursType Then
          SetProperty(OccursTypeProperty, value)
          OnPropertyChanged("Occurs")
        End If
      End Set
    End Property
#Else
    <Singular.DataAnnotations.RadioButtonList(GetType(OccursType))>
    Public Property OccursType As Integer
      Get
        Return GetProperty(OccursTypeProperty)
      End Get
      Set(value As Integer)
        If value <> Me.OccursType Then
          SetProperty(OccursTypeProperty, value)
          OnPropertyChanged("Occurs")
        End If
      End Set
    End Property
#End If


    Public Shared OccursDailyProperty As PropertyInfo(Of OccursDaily) = RegisterProperty(Of OccursDaily)(Function(c) c.OccursDaily)

    <Singular.DataAnnotations.ObjectProperty(True)>
    Public Property OccursDaily() As OccursDaily
      Get
        Return GetProperty(OccursDailyProperty)
      End Get
      Set(value As OccursDaily)
        SetProperty(OccursDailyProperty, value)
      End Set
    End Property

    Public Shared OccursWeeklyProperty As PropertyInfo(Of OccursWeekly) = RegisterProperty(Of OccursWeekly)(Function(c) c.OccursWeekly)

    <Singular.DataAnnotations.ObjectProperty(True)>
    Public Property OccursWeekly() As OccursWeekly
      Get
        Return GetProperty(OccursWeeklyProperty)
      End Get
      Set(value As OccursWeekly)
        SetProperty(OccursWeeklyProperty, value)
      End Set
    End Property

    Public Function DailyFrequency() As IDailyFrequency
      Select Case CType(Me.DailyFrequencyType, Scheduling.DailyFrequencyType)
        Case Scheduling.DailyFrequencyType.Every
          Return Me.DailyFrequencyEvery
        Case Scheduling.DailyFrequencyType.Once
          Return Me.DailyFrequencyOnce
        Case Else
          Return Nothing
      End Select
    End Function

    Public Shared DailyFrequencyTypeProperty As PropertyInfo(Of Scheduling.DailyFrequencyType) = RegisterProperty(Of Scheduling.DailyFrequencyType)(Function(c) c.DailyFrequencyType, "Daily Frequency Type", Scheduling.DailyFrequencyType.Once)
#If SILVERLIGHT Then

    Public Property DailyFrequencyType As Scheduling.DailyFrequencyType
      Get
        Return GetProperty(DailyFrequencyTypeProperty)
      End Get
      Set(value As Scheduling.DailyFrequencyType)
        If value <> Me.DailyFrequencyType Then
          SetProperty(DailyFrequencyTypeProperty, value)
          OnPropertyChanged("DailyFrequency")
        End If
      End Set
    End Property
#Else
    <Singular.DataAnnotations.RadioButtonList(GetType(Scheduling.DailyFrequencyType))>
    Public Property DailyFrequencyType As Scheduling.DailyFrequencyType
      Get
        Return GetProperty(DailyFrequencyTypeProperty)
      End Get
      Set(value As Scheduling.DailyFrequencyType)
        If value <> Me.DailyFrequencyType Then
          SetProperty(DailyFrequencyTypeProperty, value)
          OnPropertyChanged("DailyFrequency")
        End If
      End Set
    End Property
#End If

    Public Shared DailyFrequencyOnceProperty As PropertyInfo(Of DailyFrequencyOnce) = RegisterProperty(Of DailyFrequencyOnce)(Function(c) c.DailyFrequencyOnce)

    <Singular.DataAnnotations.ObjectProperty(True)>
    Public Property DailyFrequencyOnce() As DailyFrequencyOnce
      Get
        Return GetProperty(DailyFrequencyOnceProperty)
      End Get
      Set(value As DailyFrequencyOnce)
        SetProperty(DailyFrequencyOnceProperty, value)
        'If value IsNot Nothing Then
        '  Me.DailyFrequencyType = DailyFrequencyType.Once
        'End If
      End Set
    End Property

    Public Shared DailyFrequencyEveryProperty As PropertyInfo(Of DailyFrequencyEvery) = RegisterProperty(Of DailyFrequencyEvery)(Function(c) c.DailyFrequencyEvery)

    <Singular.DataAnnotations.ObjectProperty(True)>
    Public Property DailyFrequencyEvery() As DailyFrequencyEvery
      Get
        Return GetProperty(DailyFrequencyEveryProperty)
      End Get
      Set(value As DailyFrequencyEvery)
        SetProperty(DailyFrequencyEveryProperty, value)
        'If value IsNot Nothing Then
        '  Me.DailyFrequencyType = DailyFrequencyType.Every
        'End If
      End Set
    End Property

    Public Shared DurationProperty As PropertyInfo(Of Duration) = RegisterProperty(Of Duration)(Function(c) c.Duration)

    <Singular.DataAnnotations.ObjectProperty(True)>
    Public Property Duration() As Duration
      Get
        Return GetProperty(DurationProperty)
      End Get
      Set(value As Duration)
        SetProperty(DurationProperty, value)
      End Set
    End Property

    <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
    Public Overrides ReadOnly Property Description() As String
      Get
        If TypeOf Me.Occurs Is OccursDaily OrElse TypeOf Me.Occurs Is OccursWeekly Then
          Return "Occurs " & Me.Occurs.ToString & " " & Me.DailyFrequency.ToString & ". " & Me.Duration.ToString
        Else
          Return "Occurs " & Me.Occurs.ToString & ". " & Me.Duration.ToString
        End If
      End Get
    End Property

    <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
    Public ReadOnly Property DescriptionShort() As String
      Get
        Return "Occurs " & Me.Occurs.ToString & " " & Me.DailyFrequency.ToString
      End Get
    End Property

#End Region

#Region " Methods "

    Protected Overrides Sub OnPropertyChanged(propertyInfo As Csla.Core.IPropertyInfo)
      MyBase.OnPropertyChanged(propertyInfo)
      MyBase.OnPropertyChanged("Description")
    End Sub

    Protected Overrides Sub OnChildChanged(e As Csla.Core.ChildChangedEventArgs)
      MyBase.OnChildChanged(e)
      MyBase.OnPropertyChanged("Description")
    End Sub

    Public Overloads Sub MarkOld()

      MyBase.MarkOld()

      If Duration IsNot Nothing Then Duration.MarkOld()
      If OccursMonthlyDay IsNot Nothing Then OccursMonthlyDay.MarkOld()
      If OccursMonthlyThe IsNot Nothing Then OccursMonthlyThe.MarkOld()
      If OccursWeekly IsNot Nothing Then OccursWeekly.MarkOld()
      If OccursDaily IsNot Nothing Then OccursDaily.MarkOld()
      If DailyFrequencyEvery IsNot Nothing Then DailyFrequencyEvery.MarkOld()
      If DailyFrequencyOnce IsNot Nothing Then DailyFrequencyOnce.MarkOld()

    End Sub

#End Region

    Public Function GetNextScheduled(ByVal FromDate As DateTime) As DateTime
      ' this function will find the next possible date and time (after the given date for this event to happen)

      Dim dtDate As DateTime
      dtDate = Me.Occurs.GetNextOccuranceDay(FromDate, Me.DailyFrequency)
      If Me.Duration.IsValidDate(dtDate) Then
        Return dtDate
      Else
        Return DateTime.MinValue
      End If

    End Function

    Public Function GetNextScheduled() As DateTime

      ' this function will find the next possible date and time (after the given date for this event to happen)
      Return GetNextScheduled(Now)

    End Function

    Public Sub New()

      Me.OccursType = Scheduling.OccursType.Daily

      Me.DailyFrequencyOnce = New DailyFrequencyOnce()
      Me.DailyFrequencyEvery = New DailyFrequencyEvery()


      Me.Duration = New Duration()

      Me.OccursMonthlyDay = New OccursMonthlyDay()
      Me.OccursMonthlyThe = New OccursMonthlyThe()
      Me.OccursWeekly = New OccursWeekly()
      Me.OccursDaily = New OccursDaily()


    End Sub

  End Class

End Namespace