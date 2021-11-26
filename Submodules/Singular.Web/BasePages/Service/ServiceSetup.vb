Namespace ServiceHelpers

  Public Class ServiceSetup
    Inherits Controls.HelperControls.HelperBase(Of ServiceVM)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      CType(Page, IPageBase).LateResources.Add(Scripts.LibraryIncludes.ScheduleScript.ScriptTag.ToString)

      'Main Edit Section
      With Helpers.Div
        .IsVisible = Function(c) c.LastLogProgram Is Nothing AndAlso c.CurrentUIProgram Is Nothing

        With .Helpers.Toolbar()
          .Helpers.HTML.Heading2("Service Setup")

          With .Helpers.Button("Save", ButtonMainStyle.Success, ButtonSize.Normal, FontAwesomeIcon.save)

            .ClickJS = "ServiceHelper.SaveScheduleList()"
          End With

        End With

        .Helpers.MessageHolder()

        With .Helpers.Div

          With .Helpers.TableFor(Of Singular.Service.ServerProgramType)(Function(c) c.ServerProgramTypeList, False, False)

            With .FirstRow.AddReadOnlyColumn(Function(c) c.ServerProgramTypeID, 40)
              .HeaderText = "ID"
              .Style.TextAlign = TextAlign.left
            End With
            .FirstRow.AddReadOnlyColumn(Function(c) c.ServerProgramType, 200)
            .FirstRow.AddReadOnlyColumn(Function(c) c.InfoString, 300)
            .FirstRow.AddColumn(Function(c) c.ActiveInd, 50)

            With .FirstRow.AddColumn()
              With .Helpers.Button("Edit", ButtonMainStyle.Warning, ButtonSize.ExtraSmall, FontAwesomeIcon.edit)
                .IsVisible = Function(c) c.ScheduleTypeID = Singular.Service.ServerProgramType.ScheduleType.Scheduled
                .ClickJS = "$root.ServerProgramType($data)"
              End With

              With .Helpers.Button("", ButtonMainStyle.Info, ButtonSize.ExtraSmall, FontAwesomeIcon.barChart)
                .Tooltip = "View Log"
                .ClickJS = "ServiceHelper.GetScheduleProgress($data)"
              End With

              With .Helpers.Button("", ButtonMainStyle.Default, ButtonSize.ExtraSmall, FontAwesomeIcon.cogs)
                .Tooltip = "Options"
                .IsVisible = Function(c) c.ServiceMenuItems.Count > 0
                .ClickJS = "ServiceHelper.ShowCustomMenu(e, $data)"
              End With

            End With
          End With
        End With

      End With
      
      


      'Log
      With Helpers.Div
        .AddBinding(KnockoutBindingString.if, Function(c) c.LastLogProgram IsNot Nothing)

        With .Helpers.Div
          With .Helpers.Button("Back", ButtonMainStyle.Default, ButtonSize.Small, FontAwesomeIcon.back)
            .ClickJS = "ViewModel.LastLogProgram(null)"
            .Style.MarginRight("15px")
          End With

          With .Helpers.HTMLTag("h4")
            .Style.Display = Display.inline
            .Text = Function(c) "Log entries for " & c.LastLogProgram.ServerProgramType
          End With

        End With

        'Export / Fetch previous
        With .Helpers.Div
          .Style.MarginVH(15, 0)

          With .Helpers.EditorRowFor(Function(c) c.LogToDate)
            .Style.Display = Display.inline
          End With

          With .Helpers.Button("Export Log", ButtonMainStyle.Info, ButtonSize.Small, FontAwesomeIcon.grid9)
            .IsVisible = Function(c) c.ServerProgressList.Count > 0
            .ClickJS = "ServiceHelper.ExportData()"
          End With

          With .Helpers.Button("Refresh", ButtonMainStyle.Primary, ButtonSize.Small, FontAwesomeIcon.refresh)
            .IsVisible = Function(c) c.ServerProgressList.Count > 0
            .ClickJS = "ServiceHelper.GetScheduleProgress()"
          End With

        End With

        'Progress list
        With .Helpers.TableReadOnlyFor(Of Singular.Service.Scheduling.ROScheduleProgress)(Function(c) c.ServerProgressList, "")
          .NoDataContainer.AddBinding(KnockoutBindingString.text, Function(c) "No log entries for " & c.LastLogProgram.ServerProgramType)

          .FirstRow.AddReadOnlyColumn(Function(c) c.CreatedDate.ToString("dd MMM yyyy - HH:mm:ss"), 150)
          .FirstRow.AddReadOnlyColumn(Function(c) c.Progress, 350)
        End With
      End With

      With Helpers.Div
        .AddBinding(KnockoutBindingString.if, Function(c) c.CurrentUIProgram IsNot Nothing)

        With .Helpers.Div
          .Style.Margin(, , "15px")

          With .Helpers.Button("Back", ButtonMainStyle.Default, ButtonSize.Small, FontAwesomeIcon.back)
            .ClickJS = "ServiceHelper.CloseCustomUI()"
            .Style.MarginRight("15px")
          End With

          With .Helpers.HTMLTag("h3")
            .Style.Display = Display.inline
            .AddBinding(KnockoutBindingString.text, "ViewModel.CurrentUIProgram().ServerProgramType() + ' - ' + ServiceHelper.UIText")
          End With

        End With

        For Each sui In Model.ServiceUIList

          With Helpers.With(Of Object)(sui.BindPropertyName)
            .Helpers.Control(sui.Control)
          End With

        Next

      End With


      'Schedule info
      With Helpers.Div
        .Bindings.AddDialogBinding(Function(c) c.ServerProgramType IsNot Nothing, Function(c) "Edit Schedule")

        With .Helpers.With(Of Singular.Service.ServerProgramType)(Function(c) c.ServerProgramType)

          .Helpers.Control(New ScheduleSetup(Of Singular.Service.ServerProgramType)(Function(c) c.Info))
          
        End With

        With .Helpers.Button("OK", ButtonMainStyle.Primary, ButtonSize.Normal, FontAwesomeIcon.tickFilled)
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
