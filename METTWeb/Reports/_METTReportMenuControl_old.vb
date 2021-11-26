Imports Singular.Web

Public Class METTReportMenuControl_old
	Inherits Controls.HelperControls.HelperBase(Of ReportVM)

	Protected Overrides Sub Setup()
		MyBase.Setup()
		With Helpers.DivC("")

		End With
		With Helpers.DivC("ibox paddingTop15")
			With .Helpers.DivC("row marginLeftRight0")
				For Each MS As Singular.Reporting.MainSection In Singular.Reporting.ProjectReportHierarchy.GetMainSections
					If MS.IsAllowed() Then

						'Column

						'With .Helpers.DivC("col-md-12")
						With .Helpers.DivC("col-md-6")

							With .Helpers.Div
								.AddClass("ibox")

								'Heading
								With .Helpers.DivC("ibox-title")
									'With .Helpers.HTMLTag("i")
									'	.AddClass("fa fa-university")
									'End With
									With .Helpers.DivC("ibox-tools")
										With .Helpers.HTMLTag("a")
											.AddClass("collapse-link")
											With .Helpers.HTMLTag("i")
												.AddClass("fa fa-chevron-up")
											End With
										End With
									End With
									If MS.ImagePath <> "" Then
										.Helpers.Image(MS.ImagePath).Style("margin-right") = "5px"
									End If
									.Helpers.HTML(MS.Heading)


								End With

								'Body
								With .Helpers.DivC("ibox-content reportsIboxContentHeight")

									Dim TopLevelUL = .Helpers.HTMLTag("ul")
									With TopLevelUL

										'Sub Sections
										For Each SS As Singular.Reporting.SubSection In MS.SubSectionList
											If SS.IsAllowed Then

												Dim ReportUL As Singular.Web.CustomControls.HTMLTag(Of ReportVM) = TopLevelUL

												If SS.Heading <> "" Then
													With .Helpers.HTMLTag("li")
														'If the sub section has a heading, then create another sub un-ordered list
														.Helpers.HTML(SS.Heading)
														ReportUL = .Helpers.HTMLTag("ul")
													End With
												End If

												For Each R As Singular.Reporting.Report In SS.ReportList
													If R.IsAllowed Then

														With ReportUL.Helpers.HTMLTag("li")
															.Attributes("title") = HttpUtility.HtmlEncode(R.Report.Description)

															'Add a link to the report.

															If R.Report.ReportURL = "" Then

																If R.Report.UniqueKey = "" Then
																	'Make a hash of the link so the user can't see the Type Name of the report.
																	Dim LinkHash As String = Singular.Reporting.GetHash(R.Report.GetType)
																	.Helpers.LinkFor(, , "?Type=" & Server.UrlEncode(LinkHash), R.Report.ReportName)
																Else
																	.Helpers.LinkFor(, , "?Key=" & Server.UrlEncode(R.Report.UniqueKey), R.Report.ReportName)
																End If

															Else
																Dim Url As String = R.Report.ReportURL
																.Helpers.LinkFor(, , If(Url.StartsWith("~"), Utils.URL_ToAbsolute(Url), Url), R.Report.ReportName, R.Report.LinkTargetType)
															End If


														End With

													End If
												Next

											End If
										Next

									End With
								End With
							End With
						End With
						'End With


					End If
				Next
			End With
		End With


		With Helpers.Div
			.Style.ClearBoth()
			.Style("height") = "30px"
		End With

	End Sub

	Protected Overrides Sub Render()
		MyBase.Render()

		RenderChildren()
	End Sub


End Class
