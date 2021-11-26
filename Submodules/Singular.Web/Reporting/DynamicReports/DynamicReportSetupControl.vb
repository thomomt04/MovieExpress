Imports System.Reflection

Namespace Reporting

  Public Class DynamicReportSetupControl
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of DynamicReportSetupVM)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      Dim h = Helpers

      With h.Toolbar

        .Helpers.HTML.Heading2("Dynamic Report Setup")

        With .Helpers.Button(Singular.Web.DefinedButtonType.Save)
          .AddBinding(Singular.Web.KnockoutBindingString.click, "SaveAll()")
          .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.EditReport Is Nothing)
        End With
      End With

      h.MessageHolder()

      With h.If(Function(c) c.EditReport Is Nothing)
        'List of Groups

        With .Helpers.TableFor(Of Singular.Reporting.Dynamic.ReportGroup)(Function(c) c.ReportGroupList, True, True)
          .Style.Margin("0.5em", "0", "0", "0")
          .AddNewButton.Image.SrcDefined = Singular.Web.DefinedImageType.BlankPage
          .AddNewButton.ButtonText.HTML = "Add Group"

          .FirstRow.Bindings.AddDropBinding("Group", "OnDrop", "DragHover")

          .FirstRow.AddColumn(Function(c) c.GroupName, 250)

          .FirstRow.AddColumn(Function(c) c.SecurityRole, 200)
          .FirstRow.AddColumn(Function(c) c.SortOrder, 50)

          With .AddChildTable(Of Singular.Reporting.Dynamic.Report)(Function(c) c.ReportList, True, True)
            .Style.MarginAll("10px")
            .AddNewButton.AddBinding(Singular.Web.KnockoutBindingString.click, "AddReport($data)")
            .AddNewButton.Image.SrcDefined = Singular.Web.DefinedImageType.Print
            .AddNewButton.ButtonText.HTML = "Add Report"

            With .FirstRow.AddColumn()
              With .Helpers.Image
                .SrcDefined = DefinedImageType.BlankPage
                .Style("cursor") = "pointer"
                .Tooltip = "Move this row to another group by dragging this icon"
                .Bindings.AddDragBinding("Group", "Drag to a group name")
              End With
            End With

            .FirstRow.AddColumn(Function(c) c.DisplayName, 350)
            .FirstRow.AddColumn(Function(c) c.SortOrder, 50)
            With .FirstRow.AddColumn("")
              With .Helpers.Button("Edit")
                .AddClass("SmallButton EditButton")
                .Image.SrcDefined = Singular.Web.DefinedImageType.Edit
                .AddBinding(Singular.Web.KnockoutBindingString.click, "EditReport($data)")
              End With
            End With
          End With

        End With

      End With

      'Report being edited / created
      With h.With(Of Singular.Reporting.Dynamic.Report)(Function(c) c.EditReport)
        With .Helpers.Div
          .AddClass("EditReport")

          With .Helpers.If(Function(c) c.Stage = 0)
            'First stage, select data source.

            .Helpers.HTML.Heading2("Select a data source")

            With .Helpers.EditorFor(Function(c) c.StoredProcedureName)
              .Style.Width = 400
            End With

            With .Helpers.Div
              .Style.Margin("20px")

              With .Helpers.Button("Ok")
                .Image.SrcDefined = Singular.Web.DefinedImageType.Yes_GreenTick
                .AddBinding(Singular.Web.KnockoutBindingString.click, "SelectProc()")
              End With
              With .Helpers.Button("Cancel")
                .Image.SrcDefined = Singular.Web.DefinedImageType.No_RedCross
                .AddBinding(Singular.Web.KnockoutBindingString.click, "ViewModel.EditReport.Clear()")
              End With
            End With


          End With

          With .Helpers.If(Function(c) c.Stage = 1)
            'Second Stage

            With .Helpers.HTMLTag("h2")
              .AddBinding(Singular.Web.KnockoutBindingString.text, Function(c) If(c.IsNew, "New Report", "Edit Report"))
            End With

            .Helpers.ReadOnlyRowFor(Function(c) c.StoredProcedureName)

            .Helpers.EditorRowFor(Function(c) c.DisplayName)
            .Helpers.EditorRowFor(Function(c) c.Description).Editor.Style.Width = 300
            .Helpers.EditorRowFor(Function(c) c.SecurityRole)
            .Helpers.EditorRowFor(Function(c) c.SortOrder).Editor.Style.Width = 50

            With .Helpers.Div
              .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.ReportParameterList.Count > 0)

              With .Helpers.HTML
                .Heading3("Parameters")
                .Style.Margin("20px")
              End With

              With .Helpers.TableFor(Of Singular.Reporting.Dynamic.ReportParameter)(Function(c) c.ReportParameterList, False, False)

                .FirstRow.AddReadOnlyColumn(Function(c) c.ParameterName, 130)
                .FirstRow.AddColumn(Function(c) c.DisplayName, 130)
                .FirstRow.AddColumn(Function(c) c.RequiredInd, 50)
                .FirstRow.AddColumn(Function(c) c.DropDownSource, 100)

                .FirstRow.AddColumn(Function(c) c.Visible, 50)
                .FirstRow.AddColumn(Function(c) c.DefaultType, 100)
                .FirstRow.AddReadOnlyColumn(Function(c) c.ParameterDefaultValue, 100)
                With .FirstRow.AddColumn()
                  .Style.Width = 100
                  .HeaderText = "Default Value"

                  With .Helpers.EditorFor(Function(c) c.DefaultValue)
                    .Style.Width = 110
                    .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.DefaultType <> Singular.Reporting.Dynamic.ReportParameter.DefaultValueType.Special)
                  End With
                  With .Helpers.EditorFor(Function(c) c.DefinedDefaultValue)
                    .Style.Width = 110
                    .AddBinding(Singular.Web.KnockoutBindingString.visible, Function(c) c.DefaultType = Singular.Reporting.Dynamic.ReportParameter.DefaultValueType.Special)
                  End With

                End With

              End With
            End With

            With .Helpers.Div
              .Style.Margin("20px")

              With .Helpers.Button("Save")
                .Image.SrcDefined = Singular.Web.DefinedImageType.Disk
                .AddBinding(Singular.Web.KnockoutBindingString.click, "SaveReport()")
              End With
              With .Helpers.Button("Cancel")
                .Image.SrcDefined = Singular.Web.DefinedImageType.No_RedCross
                .AddBinding(Singular.Web.KnockoutBindingString.click, "ViewModel.EditReport.Clear()")
              End With
            End With

          End With

        End With
      End With

      With h.HTMLTag("script")
        .Attributes("type") = "text/javascript"

        Using Str = Assembly.GetAssembly(Me.GetType).GetManifestResourceStream("Singular.Web.DynamicReportSetup.js")
          Using Reader As New IO.StreamReader(Str)
            .HTML = Reader.ReadToEnd
          End Using
        End Using

      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace

