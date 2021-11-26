Namespace MaintenanceHelpers

  Public Class MaintenanceMenu
    Inherits Controls.HelperControls.HelperBase(Of MaintenanceVM)

		Protected Friend Overrides Sub Setup()
			MyBase.Setup()

			For Each MS As MainSection In CType(Model, MaintenanceVM).MainSectionList

				'Column
				With Helpers.Div
					.AddClass("MainSection")

					'Heading
					With .Helpers.Div()
						.AddClass("Heading")
						If MS.ImagePath <> "" Then
							.Helpers.Image(MS.ImagePath).Style("margin-right") = "5px"
						End If
						.Helpers.HTML(MS.Heading)
					End With

					'Body
					With .Helpers.Div()

						Dim TopLevelUL = .Helpers.HTMLTag("ul")
						With TopLevelUL

							'Sub Sections
							For Each SS As SubSection In MS.SubSectionList

								Dim SubUL As Singular.Web.CustomControls.HTMLTag(Of MaintenanceVM) = TopLevelUL

								If SS.Heading <> "" Then
									With .Helpers.HTMLTag("li")
										'If the sub section has a heading, then create another sub un-ordered list
										.Helpers.HTML(SS.Heading)
										SubUL = .Helpers.HTMLTag("ul")
									End With
								End If

								For Each mp As MaintenancePage In SS.MaintenancePageList

									With SubUL.Helpers.HTMLTag("li")

										.Helpers.LinkFor(, , If(String.IsNullOrEmpty(mp.URL), "?Type=" & mp.Hash, mp.URL), mp.DisplayName) '.AddBinding(KnockoutBindingString.click, "Singular.DoPostBack('Load', '" & mp.Hash & "')")

									End With

								Next


							Next

						End With

					End With



				End With

			Next

			Helpers.Div.Style.ClearBoth()

		End Sub

		Protected Friend Sub NoContentSetup()
			MyBase.Setup()

		End Sub

		Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace
