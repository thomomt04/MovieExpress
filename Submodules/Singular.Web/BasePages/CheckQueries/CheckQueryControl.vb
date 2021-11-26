Namespace CheckQueryHelpers

  Public Class CheckQueryControl
    Inherits Controls.HelperControls.HelperBase(Of CheckQueryVM)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With CType(Page, IPageBase).LateResources
        .Add(Scripts.LibraryIncludes.CheckQueriesScript.ScriptTag.ToString)
        .Add(Singular.Web.Scripts.RenderScriptGroup(Singular.Web.Scripts.ScriptGroupType.SGrid.ToString).ToString)
        .Add(Singular.Web.CSSFile.IncludeSGridStyles.ToString)
      End With


      With Helpers.Toolbar
        .Helpers.HTML.Heading2("Check Queries")

        With .Helpers.Button("RunAll", "Run All")
          .AddBinding(Singular.Web.KnockoutBindingString.click, "RunAllCheckQueries()")
          .PostBackType = Singular.Web.PostBackType.Ajax
          .Image.Glyph = Singular.Web.FontAwesomeIcon.fastforward
          .AddBinding(Singular.Web.KnockoutBindingString.enable, Function(c) Not c.IsLoading)
        End With
        .Helpers.HTMLTag("span").AddBinding(Singular.Web.KnockoutBindingString.text, Function(c) c.Status)
      End With

      Helpers.MessageHolder()

      With Helpers.Div
        With .Helpers.TableFor(Of Singular.CheckQueries.CheckQuery)(Function(c) c.CheckQueryList, False, False)

          .FirstRow.AddReadOnlyColumn(Function(c) c.Name, 300)
          .FirstRow.AddReadOnlyColumn(Function(c) c.Description, 400)

          'Status Image.
          With .FirstRow.AddColumn("")
            With .Helpers.Image(SrcDefined:=Singular.Web.DefinedImageType.BlankPage)
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.Status = CheckQueries.CheckQueryStatus.Pending)
            End With
            With .Helpers.Image(SrcDefined:=Singular.Web.DefinedImageType.Yes_GreenTick)
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.Status = CheckQueries.CheckQueryStatus.Passed)
            End With
            With .Helpers.Image(SrcDefined:=Singular.Web.DefinedImageType.No_RedCross)
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.Status = CheckQueries.CheckQueryStatus.Failed)
            End With
            With .Helpers.Image(SrcDefined:=Singular.Web.DefinedImageType.ValidationError)
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.Status = CheckQueries.CheckQueryStatus.Exception)
            End With
          End With

          'Buttons
          With .FirstRow.AddColumn()
            With .Helpers.Button("Run", "Run")
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.Status = CheckQueries.CheckQueryStatus.Pending)
              .AddBinding(Singular.Web.KnockoutBindingString.click, "RunCheckQuery($data)")
              .AddBinding(Singular.Web.KnockoutBindingString.ButtonArgument, Function(c) c.Description)
              .PostBackType = Singular.Web.PostBackType.None
              .ButtonSize = Singular.Web.ButtonSize.Small
              .Image.Glyph = Singular.Web.FontAwesomeIcon.play
              .ButtonStyle = Singular.Web.ButtonMainStyle.Info
            End With

            With .Helpers.Div
              .IsVisible = Function(c) c.Status = CheckQueries.CheckQueryStatus.Failed

              With .Helpers.Button("Download", "Export")
                .Image.SrcDefined = Singular.Web.DefinedImageType.Print
                .Image.Glyph = FontAwesomeIcon.download
                .PostBackType = Singular.Web.PostBackType.Full
                .AddBinding(Singular.Web.KnockoutBindingString.ButtonArgument, Function(c) c.Description)
                .ButtonSize = Singular.Web.ButtonSize.ExtraSmall
              End With

              With .Helpers.Button("View", ButtonMainStyle.Info, ButtonSize.ExtraSmall, FontAwesomeIcon.grid9)
                .ClickJS = "ViewCQ($data)"
              End With
            End With
            
          End With
        End With
      End With

      With Helpers.Dialog(Function(c) Not String.IsNullOrEmpty(c.CurrentCQ), Function(c) "Results for " & c.CurrentCQ, "CloseCQView")
        .Style.Width = 800

        Dim gc As New Singular.Web.CustomControls.SGrid.GridContainer(Of CheckQueryVM)(Function(c) c.CQResult)
        .Helpers.Control(gc)
        gc.Grid.Style.Height = 500
        gc.Style.Width = 780

      End With

      Helpers.Control(New Singular.Web.CustomControls.SGrid.SGridHelpers)

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace