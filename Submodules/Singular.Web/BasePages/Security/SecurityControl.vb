Namespace Security

  Public MustInherit Class SecurityControl(Of SGType)
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of SecurityVM)

    Public ButtonConainer As Controls.HelperControls.HelperBase

    Public Property GroupsTable As Singular.Web.CustomControls.Table(Of SecurityVM, SGType)
    Public Property RolesTable As Singular.Web.CustomControls.Table(Of SecurityVM, Singular.Security.ROSecurityRoleHeader)
    Public Property EditRolesButton As Singular.Web.CustomControls.Button(Of SGType)

    Protected Friend Sub SetupEmptyView()
      MyBase.Setup()
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      Dim VM As Singular.Web.IViewModel = Model
      Dim IsStateless As Boolean = VM.IsStateless
      Dim CustomContainer As Boolean = ButtonConainer IsNot Nothing

      'Toolbar with save and undo.
      If ButtonConainer Is Nothing Then
        Dim Toolbar = Helpers.Toolbar
        With Toolbar

          .Helpers.HTML.Heading2("Security Groups and Roles")

          If Not IsStateless Then
            With .Helpers.Div
              .Style.Margin("0px", "0px", "10px", "0px")
              .AddClass("HeaderUnderline")
            End With
          End If

        End With
        ButtonConainer = Toolbar

      End If
      'With Helpers.Toolbar

      Dim btnSave = Helpers.Button(Singular.Web.DefinedButtonType.Save)
      With btnSave
        If IsStateless Then .AddBinding(KnockoutBindingString.click, "Save()")
        .PostBackType = PostBackType.Ajax
      End With

      Dim btnUndo = Helpers.Button(Singular.Web.DefinedButtonType.Undo)
      With btnUndo
        .AddBinding(KnockoutBindingString.visible, Function(c) c.CurrentGroup IsNot Nothing)
        If IsStateless Then .AddBinding(KnockoutBindingString.click, "Undo()")
        .PostBackType = PostBackType.Ajax

      End With

      ButtonConainer.Helpers.Control(btnSave)
      ButtonConainer.Helpers.Control(btnUndo)

      'End With

      System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.GetType, "SelectAll", My.Resources.ControlScripts.SecurityControlSelectAll, True)

      'Message container for messages, and validation summary.
      If Not CustomContainer Then Helpers.MessageHolder()

      'Security Groups.
      With Helpers.Div
        .AddBinding(KnockoutBindingString.visible, Function(c) c.CurrentGroup Is Nothing)

        With .Helpers.FieldSet("Edit / Add Security Groups").Helpers

          GroupsTable = .TableFor(Of SGType)(Function(c) c.SecurityGroupList, True, True)
          With GroupsTable
            .AddClass("Grid")
            AddColumns(GroupsTable)

            'Delete button, only when not editing roles.
            'If Model.CurrentGroup Is Nothing Then

            EditRolesButton = .AddButton("Edit Roles")
            With EditRolesButton
              .ButtonID = "EditRoles"
              .AddBinding(KnockoutBindingString.enable, "IsValid")

              If IsStateless Then
                .AddBinding(KnockoutBindingString.click, "EditRoles($data)")
              Else
                .PostBackType = PostBackType.Ajax
                .AddBinding(Singular.Web.KnockoutBindingString.ButtonArgument, "$data.Guid()")
              End If

              .ButtonSize = ButtonSize.Small
              .ButtonStyle = ButtonMainStyle.Default
              .Image.Glyph = FontAwesomeIcon.edit
            End With

            '.FirstRow.mRemoveButton.AddBinding(KnockoutBindingString.visible, "$root.CurrentGroup() == null")
            'End If


          End With

        End With

      End With

      'Right Div for assigned Security Roles.
      With Helpers.If(Function(c) c.CurrentGroup IsNot Nothing)

        With .Helpers.Div

          With .Helpers.FieldSet("")
            .Helpers.HTMLTag("legend").AddBinding(KnockoutBindingString.text, Function(c) "Select Roles for group '" & c.CurrentGroup.SecurityGroup & "'")

            With .Helpers.Div()
              .Style.MarginI(, , 10)
              .Helpers.Button("Select All", ButtonMainStyle.Primary, ButtonSize.ExtraSmall, FontAwesomeIcon.thumbsUp).AddBinding(KnockoutBindingString.click, "SelectAll(true)")
              .Helpers.Button("Select None", ButtonMainStyle.Info, ButtonSize.ExtraSmall, FontAwesomeIcon.thumbsDown).AddBinding(KnockoutBindingString.click, "SelectAll(false)")

            End With

            'Security Role Header Table
            RolesTable = .Helpers.TableFor(Of Singular.Security.ROSecurityRoleHeader)(Function(c) c.ROSecurityRoleHeaderList, False, False)

            With RolesTable
              If IsStateless Then
                .DataSourceString = "GetSecurityRoles(CurrentGroup())"
              End If

              .AddClass("Grid GridMultiBand")
              .FirstRow.AddReadOnlyColumn(Function(c) c.SectionName, "Description").FieldDisplay.TagType = Singular.Web.FieldTagType.strong

              'Security Roles
              With .AddChildTable(Of Singular.Security.ROSecurityRole)(Function(c) c.ROSecurityRoleList, False, False)
                .ShowHeader = False
                .ParentCell.AddClass("ChildBand1")

                '1 custom column with the checkbox, image and label all in one.
                With .FirstRow.AddColumn("Role")
                  '.Settings.Width = "300px"
                  'Image
                  Dim ImageURL = Scripts.LibResourceHandler.GetLibImageURL("")

                  .Helpers.Image().AddBinding(Singular.Web.KnockoutBindingString.src, "'" & ImageURL & "' + (SelectedInd() ? 'IconAuth.png' : 'IconError.png')")
                  'Check Box
                  .Helpers.EditorFor(Function(c) c.SelectedInd)
                  'Label with Security Role as the text, Description as the tooltip, and targeting to the checkbox when clicked.
                  With .Helpers.ReadOnlyFor(Function(c) c.SecurityRole, Singular.Web.FieldTagType.label)
                    .AddBinding(Singular.Web.KnockoutBindingString.for, Function(c) c.SelectedInd)
                    .AddBinding(Singular.Web.KnockoutBindingString.title, Function(c) c.Description)
                  End With

                End With

                With .FirstRow.AddColumn("")
                  .Style("padding") = "0 10px"
                  With .Helpers.Span(Function(c) c.Description)
                    .Style.FontWeight = FontWeight.light
                  End With
                End With

              End With

            End With
          End With
        End With

      End With

      If IsStateless Then
        CType(Page, IPageBase).LateResources.Add(Scripts.LibraryIncludes.SecurityScript.ScriptTag.ToString)
      End If

    End Sub

    Protected MustOverride Sub AddColumns(tbl As CustomControls.Table(Of SecurityVM, SGType))

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()

    End Sub

  End Class

  Public Class SecurityControl
    Inherits SecurityControl(Of Singular.Security.SecurityGroup)

    Protected Overrides Sub AddColumns(tbl As CustomControls.Table(Of SecurityVM, Singular.Security.SecurityGroup))
      tbl.FirstRow.AddColumn(Function(c) c.SecurityGroup, 200)
      tbl.FirstRow.AddColumn(Function(c) c.Description, 350)
    End Sub
  End Class

End Namespace