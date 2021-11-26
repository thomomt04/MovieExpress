Imports System.Linq.Expressions
Imports Singular.Service.Scheduling

Namespace ServiceHelpers

  Public Class ScheduleSetup(Of Type)
    Inherits Controls.HelperControls.HelperBase(Of Type)

    Private _ScheduleProperty As Expression(Of Func(Of Type, Object))

    Public Sub New(ScheduleProperty As Expression(Of Func(Of Type, Object)))
      _ScheduleProperty = ScheduleProperty
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      CType(Page, IPageBase).LateResources.Add(Scripts.LibraryIncludes.ScheduleScript.ScriptTag.ToString)

      OnlyChildControls = True


      With Helpers.With(Of Schedule)(_ScheduleProperty)

        'Occurs Type
        With .Helpers.Div
          .AddClass("sch-Group")
          .Style.PaddingAll("10px")

          With .Helpers.Div
            .Style.Display = Display.inlineblock
            .Style.Width = 150

            .Helpers.HTML.Heading3("Occurs")
            .Helpers.EditorFor(Function(c) c.OccursType)
          End With

          With .Helpers.Div
            .Style.Display = Display.inlineblock
            .Style.VerticalAlign = VerticalAlign.top

            'Occurs Daily
            With .Helpers.Div()
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.OccursType = 0)

              .Helpers.HTML.Heading3("Daily")
              With .Helpers.Div
                .Helpers.HTML("Every")
                .Helpers.EditorFor(Function(c) c.OccursDaily.DayInterval).Style.Width = "30px"
                .Helpers.HTML("Day(s)")
              End With
            End With

            'Occurs Weekly
            With .Helpers.Div()
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.OccursType = 1)

              .Helpers.HTML.Heading3("Weekly")
              With .Helpers.Div
                .Helpers.HTML("Every")
                .Helpers.EditorFor(Function(c) c.OccursDaily.DayInterval).Style.Width = "30px"
                .Helpers.HTML("Week(s) on:")

                With .Helpers.Div
                  .Style.MarginVH(10, 0)
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Monday)
                    .AddClass("sch-WeekLabel")
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Tuesday)
                    .AddClass("sch-WeekLabel")
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Wednesday)
                    .AddClass("sch-WeekLabel")
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Thursday)
                    .AddClass("sch-WeekLabel")
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Friday)
                    .AddClass("sch-WeekLabel")
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Saturday)
                    .AddClass("sch-WeekLabel")
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Sunday)
                    .AddClass("sch-WeekLabel")
                  End With
                End With
              End With
            End With

            'Occurs Monthly
            With .Helpers.Div()
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.OccursType = 2)

              .Helpers.HTML.Heading3("Monthly")

              'Occurs Monthly type
              With .Helpers.Div
                .Style.Display = Display.inlineblock
                .Style.MarginRight("10px")

                Dim omt As CustomControls.RadioButtonEditor(Of Schedule) = .Helpers.EditorFor(Function(c) c.OccursMonthlyType)
                omt.Input.Style.MarginI(5, 0, 15)
              End With

              With .Helpers.Div
                .Style.Display = Display.inlineblock
                .Style.VerticalAlign = VerticalAlign.top

                With .Helpers.Div
                  .AddBinding(Singular.Web.KnockoutBindingString.enableChildren, Function(c) c.OccursMonthlyType = OccursMonthlyType.Day)
                  .Helpers.EditorFor(Function(c) c.OccursMonthlyDay.Day).Style.Width = "30px"
                  .Helpers.HTML("of Every")
                  .Helpers.EditorFor(Function(c) c.OccursMonthlyDay.MonthInterval).Style.Width = "30px"
                  .Helpers.HTML("Month(s)")
                End With
                With .Helpers.Div
                  .Style.MarginVH(7, 0)
                  .AddBinding(Singular.Web.KnockoutBindingString.enableChildren, Function(c) c.OccursMonthlyType = OccursMonthlyType.The)
                  With .Helpers.EditorFor(Function(c) c.OccursMonthlyThe.TheDay)
                    .Style.Width = "40px"
                  End With
                  With .Helpers.EditorFor(Function(c) c.OccursMonthlyThe.TheDayOfWeek)
                    .Style.Width = "100px"
                  End With
                  .Helpers.HTML("of Every")
                  .Helpers.EditorFor(Function(c) c.OccursMonthlyThe.MonthInterval).Style.Width = "30px"
                  .Helpers.HTML("Month(s)")
                End With
              End With

            End With

          End With



        End With


        'Daily Frequency
        With .Helpers.Div()
          .AddClass("sch-Group")
          .Style.PaddingAll("10px")

          .Helpers.HTML.Heading3("Daily Frequency")

          'Occurs Monthly type
          With .Helpers.Div
            .Style.Display = Display.inlineblock
            .Style.Width = 150

            Dim dft As CustomControls.RadioButtonEditor(Of Schedule) = .Helpers.EditorFor(Function(c) c.DailyFrequencyType)
            dft.Input.Style.MarginI(5, 0, 15)

          End With

          With .Helpers.Div

            .Style.Display = Display.inlineblock
            .Style.VerticalAlign = VerticalAlign.top

            With .Helpers.Div
              .AddBinding(Singular.Web.KnockoutBindingString.enableChildren, Function(c) c.DailyFrequencyType = 0)

              'once
              .Helpers.EditorFor(Function(c) c.DailyFrequencyOnce.AtTime).Style.Width = "50px"
            End With

            With .Helpers.Div
              .AddBinding(Singular.Web.KnockoutBindingString.enableChildren, Function(c) c.DailyFrequencyType = 1)
              .Style.MarginVH(7, 0)

              'every x hours / mins etc
              With .Helpers.EditorFor(Function(c) c.DailyFrequencyEvery.Unit)
                .Style.Width = "30px"
              End With
              With .Helpers.EditorFor(Function(c) c.DailyFrequencyEvery.TimeMeasure)
                .Style.Width = "80px"
              End With

            End With

            With .Helpers.Div
              'from when to when
              With .Helpers.EditorRowFor(Function(c) c.DailyFrequencyEvery.StartTime)
                .Label.LabelText = "Starting At"
                .Label.Style.Width = "90px"
                .Editor.Style.Width = "50px"
              End With
              With .Helpers.EditorRowFor(Function(c) c.DailyFrequencyEvery.EndTime)
                .Label.LabelText = "Ending At"
                .Label.Style.Width = "90px"
                .Editor.Style.Width = "50px"
              End With
            End With
          End With

        End With

        'Duration
        With .Helpers.Div
          .AddClass("sch-Group")
          .Style.PaddingAll("10px")

          .Helpers.HTML.Heading3("Duration")

          With .Helpers.Div

            With .Helpers.EditorRowFor(Function(c) c.Duration.StartDate)
              .Label.Style.Width = "150px"
              .Label.Style.PaddingAll("0px")
              .Editor.Style.Width = "120px"
            End With

            With .Helpers.EditorRowFor(Function(c) c.Duration.HasEndDate)
              .Label.Style.Width = "150px"
            End With

            With .Helpers.EditorRowFor(Function(c) c.Duration.EndDate)
              .Style.Display = Display.inlineblock
              .Style.VerticalAlign = VerticalAlign.top

              .Label.Style.Width = "150px"
              .Editor.Style.Width = "120px"
              .IsVisible = Function(c) c.Duration.HasEndDate
            End With
          End With
        End With

      End With

    End Sub

  End Class

End Namespace

