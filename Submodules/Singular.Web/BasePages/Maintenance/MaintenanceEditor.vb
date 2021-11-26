Imports System.Reflection

Namespace MaintenanceHelpers

  Public Class MaintenanceEditor
		Inherits Controls.HelperControls.HelperBase(Of MaintenanceVM)

		Private Property mOnRenderButton As Action(Of Controls.HelperControls.HelperAccessors(Of MaintenanceVM), MaintenancePage) = Nothing

    Public Sub New(VM As MaintenanceVM)
      Me.MaintenancePage = VM.CurrentMaintenancePage
			mVM = VM
		End Sub

		Public Sub New(VM As MaintenanceVM, OnRenderButton As Action(Of Controls.HelperControls.HelperAccessors(Of MaintenanceVM), MaintenancePage))
			Me.MaintenancePage = VM.CurrentMaintenancePage
			mVM = VM
			mOnRenderButton = OnRenderButton
		End Sub

    Private mVM As MaintenanceVM
    Public Property MaintenancePage As MaintenancePage

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers

        Dim VM As MaintenanceVM = Model

        With .Toolbar()
          .Helpers.HTML.Heading3("Edit " & MaintenancePage.DisplayName)

          If VM.CurrentList Is Nothing AndAlso VM.CurrentObject Is Nothing Then

            .Helpers.HTML("<p>Please add a new record, or click find to edit an existing record.</p>")
            .Helpers.FindScreen(Of Object)("Find", "Find", False, MaintenancePage.ListType)
            If MaintenancePage.AllowAdd Then
              .Helpers.Button(DefinedButtonType.New)
            End If
          Else
            If VM.CanEdit Then
              With .Helpers.Button(DefinedButtonType.Save)
                If VM.IsStateless Then
                  .AddBinding(KnockoutBindingString.click, "Save()")
                End If
              End With
              With .Helpers.Button(DefinedButtonType.Undo)
                .AddBinding(KnockoutBindingString.visible, Function(c) c.IsDirty)
                .ButtonText.Text = Function(c) If(c.IsDirty, "Undo", "Back")
                If VM.IsStateless Then
                  .AddBinding(KnockoutBindingString.click, "Undo()")
                End If
              End With
              If MaintenancePage.AllowExport Then
                .Helpers.Button(DefinedButtonType.Export).PostBackType = PostBackType.Full
              End If

              If MaintenancePage.AllowRemove AndAlso MaintenancePage.EditMode = MaintenanceHelpers.MaintenancePage.EditModeType.Form Then
                With .Helpers.Button("Delete", "Delete")
                  .Image.SrcDefined = DefinedImageType.TrashCan
                  .PromptText = "Are you sure you want to Delete this Record?"
                End With
              End If

              For Each btn In MaintenancePage.ButtonList
                .Helpers.Control(btn)
              Next

            End If
          End If
          If Model.CurrentObject IsNot Nothing Then
            With .Helpers.Button("Back", "Back")
              .Image.SrcDefined = DefinedImageType.Left
              .Image.Glyph = FontAwesomeIcon.back
            End With
          End If
					If mOnRenderButton IsNot Nothing Then
						mOnRenderButton.Invoke(.Helpers, MaintenancePage)
					End If
        End With

        .MessageHolder()

        If MaintenancePage.InfoText <> "" Then
          .HTML(MaintenancePage.InfoText)
        End If

        If MaintenancePage.EditMode = MaintenanceHelpers.MaintenancePage.EditModeType.Grid Then
          AddTable(Helpers)
        ElseIf MaintenancePage.EditMode = MaintenanceHelpers.MaintenancePage.EditModeType.Form Then
          AddForm(Helpers)
        Else
          AddCustomControl(Helpers)
        End If

        If VM.IsStateless Then
          CType(Page, IPageBase).LateResources.Add(Scripts.LibraryIncludes.MaintenanceScript.ScriptTag.ToString)
        End If

      End With

    End Sub



		Protected Friend Sub AddTable(Helpers As Controls.HelperControls.HelperAccessors(Of MaintenanceVM))

			Dim ct = Helpers.ChangeType(Of Object)()

			Dim tbl = ct.Helpers.TableFor(Of Object)(Singular.Reflection.GetMember(Of MaintenanceVM)(Function(c) c.CurrentList),
																							 Singular.DataAnnotations.AllowAddRemoveEdit.AllowsAdd(MaintenancePage.ListType, MaintenancePage.AllowAdd),
																							 Singular.DataAnnotations.AllowAddRemoveEdit.AllowsRemove(MaintenancePage.ListType, MaintenancePage.AllowRemove))
			tbl.OverrideChildType = Singular.Reflection.GetLastGenericType(CType(Model, MaintenanceVM).CurrentList.GetType)
			tbl.AutoGenerateColumns()
			If MaintenancePage.RemovePromptText <> "" Then
				tbl.RemoveButton.PromptText = MaintenancePage.RemovePromptText
			End If
			'If Not tbl.HasChildTables Then
			'  tbl.AddClass("Sortable")
			'End If

		End Sub

		Protected Friend Sub AddForm(Helpers As Controls.HelperControls.HelperAccessors(Of MaintenanceVM))

			If Model.CurrentObject IsNot Nothing Then

				Dim cl As New Singular.UIContextList
				cl.AddContext(Model.GetType.Name)

				With Helpers.Div
					.AddBinding(KnockoutBindingString.with, "CurrentList()[0]")

					Model.CurrentObject.GetType.ForEachBrowsableProperty(cl, Sub(pi)
																																		 If Singular.Reflection.TypeImplementsInterface(pi.PropertyType, GetType(Singular.ISingularBusinessListBase)) Then
																																			 'Child List
																																			 'Get Child Object type
																																			 Dim ChildType As System.Type = pi.PropertyType.BaseType.GetGenericArguments(1)
																																			 'Create Table
																																			 Dim tbl = .Helpers.TableFor(Of Object)(pi, MaintenancePage.AllowAdd, MaintenancePage.AllowRemove)
																																			 tbl.OverrideChildType = ChildType
																																			 tbl.AutoGenerateColumns()
																																		 Else
																																			 .Helpers.EditorRowFor(pi).Editor.Style.Width = 250
																																		 End If
																																	 End Sub, , , True)

				End With

			End If

		End Sub

		Protected Friend Sub NoContentSetup()
			MyBase.Setup()
		End Sub

		Protected Friend Sub AddCustomControl(Helpers As Controls.HelperControls.HelperAccessors(Of MaintenanceVM))
			Dim Control = Activator.CreateInstance(MaintenancePage.CustomControlType)
			Helpers.Control(Control)
		End Sub

		Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace

