Namespace AuditTrails

  Public Class AuditTrailLookupControl
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of IAuditTrailLookupVM)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      CType(Page, IPageBase).LateResources.Add(Scripts.LibraryIncludes.AuditTrailsScript.ScriptTag.ToString)

      With Helpers.With(Of Singular.Web.AuditTrails.AuditTrailLookup)(Function(c) c.AuditTrailLookup)

        With .Helpers.DivC("at-container")

          'Header is not selected
          'Show List of tables and headers
          With .Helpers.Div
            .IsVisible = Function(c) c.SelectedHeader Is Nothing

            .Helpers.UseTemplate("TableList")
          End With

          'Header is selected
          'Show details of single record.
          With .Helpers.With(Of Singular.AuditTrails.ROAuditTrailHeader)(Function(c) c.SelectedHeader)

            .Helpers.HTMLTag("h4").AddBinding(Singular.Web.KnockoutBindingString.text, "ATHelper.GetCurrentDescription()")

            With .Helpers.DivC("at-buttons")
              .Helpers.Button("Back", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Small, Singular.Web.FontAwesomeIcon.back).ClickJS = "$parent.SelectedHeader(null);"

              With .Helpers.DivC("at-filters")
                .Helpers.Span("Highlight changes to ")
                .Helpers.EditorFor(Function(c) ViewModel.AuditTrailLookup.ColumnFilter)

                With .Helpers.Button("", Singular.Web.ButtonMainStyle.Info, Singular.Web.ButtonSize.Small, Singular.Web.FontAwesomeIcon.filter)
                  .ClickJS = "$parent.HideFilteredRows(!$parent.HideFilteredRows())"
                  .IsVisible = Function(c) Not String.IsNullOrEmpty(ViewModel.AuditTrailLookup.ColumnFilter)
                  .ButtonText.Text = Function(c) If(ViewModel.AuditTrailLookup.HideFilteredRows, "Show", "Hide") & " Filtered Rows"
                End With
              End With

            End With

            With .Helpers.TableReadOnlyFor(Of Singular.AuditTrails.ROAuditTrail)(Function(c) c.ROAuditTrailList, "")
              .ReadOnlyColumnsHaveSpan = False

              .FirstRow.AddReadOnlyColumn(Function(c) c.ChageDateTime, 150)
              .FirstRow.AddReadOnlyColumn(Function(c) c.Type, 250).CellBindings.Add(Singular.Web.KnockoutBindingString.text, "ATHelper.GetOperation($data)")
              .FirstRow.AddReadOnlyColumn(Function(c) c.UserName, 300)
              .FirstRow.Bindings.AddCSSClassBinding(Function(c) c.IsFiltered, "at-filtered")
              .FirstRow.IsVisible = Function(c) Not ViewModel.AuditTrailLookup.HideFilteredRows OrElse Not c.IsFiltered

              With .AddChildTableReadOnly(Of Singular.AuditTrails.ROAuditTrailDetail)(Function(c) c.ROAuditTrailDetailList, "")
                .Style.MarginI(10, 0, 15, 10)
                .ReadOnlyColumnsHaveSpan = False

                .FirstRow.AddReadOnlyColumn(Function(c) c.Column, 200)
                .FirstRow.AddReadOnlyColumn(Function(c) c.OldValue, 200).AddClass("at-detail-value")
                .FirstRow.AddReadOnlyColumn(Function(c) c.NewValue, 200).AddClass("at-detail-value")
              End With
              .FirstRow.ExpandButton.IsVisible = Function(c) c.ROAuditTrailDetailList.Count > 0

            End With

          End With

        End With

      End With





      With Helpers.Template(Of Singular.AuditTrails.IAuditTrailHeader)("TableList")

        With .Helpers.ForEachTemplate(Of Singular.AuditTrails.ROAuditTrailTable)(Function(c) c.ROAuditTrailTableList)

          With .Helpers.DivC("at-table-row")
            With .Helpers.Button("", Singular.Web.ButtonMainStyle.Default, Singular.Web.ButtonSize.ExtraSmall, Singular.Web.FontAwesomeIcon.None)
              .Style.Width = 22
              .ButtonText.Text = Function(c) If(c.IsExpanded, "-", "+")
              .Click = Function(c) c.IsExpanded = Not c.IsExpanded
            End With
            .Helpers.Span(Function(c) c.TableName)
          End With

          With .Helpers.Div
            .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.IsExpanded)

            With .Helpers.TableFor(Of Singular.AuditTrails.ROAuditTrailHeader)(Function(c) c.ROAuditTrailHeaderList, False, False)
              .ReadOnlyColumnsHaveSpan = False

              .FirstRow.AddClass("at-header")

              .FirstRow.AddBinding(Singular.Web.KnockoutBindingString.click, "ATHelper.SelectRow($data)")

              .FirstRow.AddReadOnlyColumn(Function(c) c.RowID, 100).Style.TextAlign = Singular.Web.TextAlign.left
              .FirstRow.AddReadOnlyColumn(Function(c) c.Description, 400)
              .FirstRow.AddReadOnlyColumn(Function(c) c.LastChangeDate, 150)
              .FirstRow.AddReadOnlyColumn(Function(c) c.LastChangedBy, 200)

              With .AddChildRow
                .Helpers.UseTemplate("TableList")
              End With
              With .FirstRow.ExpandButton
                .AddBinding(Singular.Web.KnockoutBindingString.visible, "ATHelper.ShowHeaderExpand($parent, $data)")
                .ClickJS = "ATHelper.HeaderExpand($parent, $data, e)"
                .ButtonText.Text = Function(c) If(c.FetchedChildren, If(c.IsExpanded, "-", "+"), "")
                With .Image
                  .IsVisible = Function(c) Not c.FetchedChildren
                  .Glyph = Singular.Web.FontAwesomeIcon.blank
                  .AddBinding(Singular.Web.KnockoutBindingString.css, "ATHelper.HeaderExpandClass($data)")
                End With

              End With


            End With

          End With

        End With

      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace

