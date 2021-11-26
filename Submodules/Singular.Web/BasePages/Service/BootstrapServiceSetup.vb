Namespace ServiceHelpers

  Public Class BootstrapServiceSetup
    Inherits Controls.HelperControls.HelperBase(Of ServiceVM)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      CType(Page, IPageBase).LateResources.AddLateScript("~/Singular/Javascript/Include/Schedule.js")

      With Helpers.Bootstrap.Row
        With .Helpers.Toolbar()
          .Helpers.MessageHolder()
        End With
      End With

      With Helpers.Bootstrap.Row
        With Helpers.Bootstrap.FlatBlock("Server Programs", True)
          With .AboveContentTag
            .Helpers.Button(Singular.Web.DefinedButtonType.Save)
            .Helpers.Button(Singular.Web.DefinedButtonType.Undo)
          End With
          With .ContentTag
            With .Helpers.Bootstrap.TableFor(Of Singular.Service.ServerProgramType)("$root.ServerProgramTypeList()", False, False, False, False, True, True, False)
              With .FirstRow
                .AddReadOnlyColumn(Function(c) c.ServerProgramTypeID).HeaderText = "Server Program Number"
                .AddReadOnlyColumn(Function(c) c.ServerProgramType)
                .AddReadOnlyColumn(Function(c) c.InfoString)
                With .AddColumn("Active?")
                  With .Helpers.Bootstrap.StateButton(Function(c) c.ActiveInd, "Active", "Inactive",
                                                      "btn-success", "btn-default", ,
                                                      "fa-minus", "btn-sm")
                  End With
                End With
                With .AddColumn()
                  With .Helpers.Bootstrap.Button(, "Edit", , ,
                                                 BootstrapEnums.ButtonSize.Small, , "fa-edit", ,
                                                 PostBackType.None, "EditSchedule($data)")
                  End With
                  'With .Helpers.Button("Edit")
                  '  .AddBinding(Singular.Web.KnockoutBindingString.click, "ViewModel.ServerProgramType($data)")
                  'End With
                End With
              End With
            End With
          End With
        End With
      End With

      ''Menu
      'With Helpers.With(Of Singular.Service.ServerProgramTypeList)(Function(c) c.ServerProgramTypeList)
      '  With .Helpers.Toolbar()
      '    .Helpers.HTML.Heading2("Server Programs")
      '    .Helpers.Button(Singular.Web.DefinedButtonType.Save)
      '    .Helpers.Button(Singular.Web.DefinedButtonType.Undo)
      '  End With

      '  With .Helpers.TableFor(Of Singular.Service.ServerProgramType)("$root.ServerProgramTypeList()", False, False)
      '    '.AddNewButton.Visible = False
      '    '.RemoveButton.Visible = False
      '    .FirstRow.AddReadOnlyColumn(Function(c) c.ServerProgramTypeID).HeaderText = "Server Program Number"
      '    .FirstRow.AddColumn(Function(c) c.ServerProgramType)
      '    .FirstRow.AddReadOnlyColumn(Function(c) c.InfoString)
      '    .FirstRow.AddColumn(Function(c) c.ActiveInd)
      '    With .FirstRow.AddColumn()
      '      With .Helpers.Button("Edit")
      '        .AddBinding(Singular.Web.KnockoutBindingString.click, "$root.ServerProgramType($data)")
      '      End With
      '    End With
      '  End With

      'End With

      'Schedule info
      With Helpers.Div
        .Attributes("id") = "EditSchedule"
        '.Bindings.AddDialogBinding(Function(d) d.ServerProgramType IsNot Nothing, Function(c) "Edit Schedule")
        With .Helpers.With(Of Singular.Service.ServerProgramType)(Function(c) c.ServerProgramType)
          With .Helpers.With(Of Singular.Service.Scheduling.Schedule)(Function(c) c.Info)
            'Occurs Type
            With .Helpers.Div
              .Style.FloatLeft()
              .Style.Padding(, "50px", , )
              With .Helpers.Div
                .Style("font-weight") = "700"
                .Helpers.HTML("Occurs")
                .Style.Padding(, , "10px", )
              End With
              .Helpers.EditorFor(Function(c) c.OccursType)
            End With
            'With .Helpers.Bootstrap.Column(12, 12, 3, 3)
            '  With .Helpers.EditorFor(Function(c) c.OccursType)
            '  End With
            'End With
            'Occurs Daily
            With .Helpers.Div()
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.OccursType = 0)
              With .Helpers.Div
                .Helpers.HTML("Daily")
                .Style.Padding(, , "10px", )
                .Style("font-weight") = "700"
              End With
              With .Helpers.Div
                .Style.Padding(, , , "150px")
                .Helpers.HTML("Every")
                .Helpers.EditorFor(Function(c) c.OccursDaily.DayInterval).Style.Width = "30px"
                .Helpers.HTML("Day(s)")
              End With
            End With
            'Occurs Weekly
            With .Helpers.Div()
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.OccursType = 1)
              With .Helpers.Div
                .Helpers.HTML("Weekly")
                .Style.Padding(, , "10px", )
                .Style("font-weight") = "700"
              End With
              With .Helpers.Div
                .Style.Padding(, , , "150px")
                .Helpers.HTML("Every")
                .Helpers.EditorFor(Function(c) c.OccursDaily.DayInterval).Style.Width = "30px"
                .Helpers.HTML("Week(s) on:")
                With .Helpers.Div()
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Monday)
                    .Style.FloatLeft()
                    .Style.Padding(, , , "20px")
                    .Label.Style.Width = "40px"
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Tuesday)
                    .Style.FloatLeft()
                    .Style.Padding(, , , "20px")
                    .Label.Style.Width = "40px"
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Wednesday)
                    .Style.FloatLeft()
                    .Style.Padding(, , , "20px")
                    .Label.Style.Width = "60px"
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Thursday)
                    .Style.FloatLeft()
                    .Style.Padding(, , , "20px")
                    .Label.Style.Width = "50px"
                  End With

                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Friday)
                    .Style.FloatLeft()
                    .Style.Padding(, , , "20px")
                    .Label.Style.Width = "35px"
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Saturday)
                    .Style.FloatLeft()
                    .Style.Padding(, , , "20px")
                    .Label.Style.Width = "45px"
                  End With
                  With .Helpers.EditorRowFor(Function(c) c.OccursWeekly.Sunday)
                    .Style.FloatLeft()
                    .Style.Padding(, , , "20px")
                    .Label.Style.Width = "40px"
                  End With
                End With
              End With
            End With
            'Occurs Monthly
            With .Helpers.Div()
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.OccursType = 2)
              With .Helpers.Div
                .Helpers.HTML("Monthly")
                .Style.Padding(, , "10px", )
                .Style("font-weight") = "700"
              End With
              'Occurs Monthly type
              With .Helpers.Div
                .Style.FloatLeft()
                .Style.Padding(, , , "30px")
                .Helpers.EditorFor(Function(c) c.OccursMonthlyType)
              End With
              With .Helpers.Div
                .Style.FloatLeft()
                .Style.Padding(, , , "10px")
                With .Helpers.Div
                  .AddBinding(Singular.Web.KnockoutBindingString.enableChildren, Function(c) c.OccursMonthlyType = 1)
                  .Helpers.EditorFor(Function(c) c.OccursMonthlyDay.Day).Style.Width = "30px"
                  .Helpers.HTML("of Every")
                  .Helpers.EditorFor(Function(c) c.OccursMonthlyDay.MonthInterval).Style.Width = "30px"
                  .Helpers.HTML("Month(s)")
                End With
                With .Helpers.Div
                  .AddBinding(Singular.Web.KnockoutBindingString.enableChildren, Function(c) c.OccursMonthlyType = 0)
                  .Helpers.EditorFor(Function(c) c.OccursMonthlyThe.TheDay).Style.Width = "40px"
                  .Helpers.EditorFor(Function(c) c.OccursMonthlyThe.TheDayOfWeek).Style.Width = "100px"
                  .Helpers.HTML("of Every")
                  .Helpers.EditorFor(Function(c) c.OccursMonthlyThe.MonthInterval).Style.Width = "30px"
                  .Helpers.HTML("Month(s)")
                End With
              End With
              .Helpers.Div.Style.ClearBoth()
            End With
            .Helpers.Div.Style.ClearBoth()
            .Helpers.HTML.NewLine()
            'Daily Frequency
            With .Helpers.Div()
              With .Helpers.Div
                .Style("font-weight") = "700"
                .Style.Padding(, , "10px", )
                .Helpers.HTML("Daily Frequency")
              End With
              'Occurs Monthly type
              With .Helpers.Div
                .Style.FloatLeft()
                .Helpers.EditorFor(Function(c) c.DailyFrequencyType)
              End With
              With .Helpers.Div
                .Style.FloatLeft()
                .Style.Padding(, , , "10px")
                With .Helpers.Div
                  .AddBinding(Singular.Web.KnockoutBindingString.enableChildren, Function(c) c.DailyFrequencyType = 0)
                  '.Helpers.HTML("Once at: ")
                  .Helpers.EditorFor(Function(c) c.DailyFrequencyOnce.AtTime).Style.Width = "50px"
                End With
                With .Helpers.Div
                  .AddBinding(Singular.Web.KnockoutBindingString.enableChildren, Function(c) c.DailyFrequencyType = 1)
                  '.Helpers.HTML("Occurs Every: ")
                  With .Helpers.EditorFor(Function(c) c.DailyFrequencyEvery.Unit)
                    .Style.Width = "30px"
                    .Style.FloatLeft()
                  End With
                  With .Helpers.EditorFor(Function(c) c.DailyFrequencyEvery.TimeMeasure)
                    .Style.Width = "80px"
                    .Style.FloatLeft()
                  End With
                  With .Helpers.Div
                    .Style.FloatLeft()
                    .Style.Margin("-5px", , , "10px")
                    With .Helpers.EditorRowFor(Function(c) c.DailyFrequencyEvery.StartTime)
                      .Label.LabelText = "Starting At"
                      .Editor.Style.Width = "50px"
                    End With
                    With .Helpers.EditorRowFor(Function(c) c.DailyFrequencyEvery.EndTime)
                      .Label.LabelText = "Ending At"
                      .Editor.Style.Width = "50px"
                    End With
                  End With
                End With
              End With
              .Helpers.Div.Style.ClearBoth()
            End With
            .Helpers.Div.Style.ClearBoth()
            .Helpers.HTML.NewLine()
            'Duration
            With .Helpers.Div
              With .Helpers.Div
                .Helpers.HTML("Duration")
                .Style("font-weight") = "700"
                .Style.Padding(, , "10px", )
              End With
              With .Helpers.Div
                .Style.FloatLeft()
                With .Helpers.EditorRowFor(Function(c) c.Duration.StartDate)
                  .Label.Style.Width = "70px"
                  .Editor.Style.Width = "100px"
                  .Style.FloatLeft()
                End With
                With .Helpers.Div
                  .Style.FloatLeft()
                  With .Helpers.EditorFor(Function(c) c.Duration.HasEndDate)

                    .Style.FloatLeft()
                    .Style.Width = "100px"
                  End With
                End With
                With .Helpers.EditorFor(Function(c) c.Duration.EndDate)
                  .Style.FloatLeft()
                  .Style.Width = "100px"
                  .AddBinding(Singular.Web.KnockoutBindingString.enable, "$data.Duration().HasEndDate() == 1") ' Function(c) c.Duration.HasEndDate)
                End With
              End With
            End With
          End With
          .Helpers.Div.Style.ClearBoth()
        End With
        With .Helpers.Button("OK")
          .AddBinding(Singular.Web.KnockoutBindingString.click, "$root.ServerProgramType(null)")
        End With
        With .Helpers.Button("Cancel")
          .AddBinding(Singular.Web.KnockoutBindingString.click, "$root.ServerProgramType(null)")
        End With
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace
