Imports Singular
Imports Singular.Extensions
Imports Singular.Reporting.Dynamic
Imports Singular.Web

Public Class METTReportCriteriaControl_old
	Inherits Controls.HelperControls.HelperBase(Of ReportVM)

	Private mToolbar As Singular.Web.CustomControls.Toolbar(Of Singular.Web.Reporting.ReportVM)

	Public ReadOnly Property Toolbar As Singular.Web.CustomControls.Toolbar(Of Singular.Web.Reporting.ReportVM)
		Get
			Return mToolbar
		End Get
	End Property

	Protected Overrides Sub Setup()
		MyBase.Setup()

		Helpers.MessageHolder()

		Dim Report As Singular.Reporting.IReport = Model.Report

		If Report.CustomCriteriaControlType Is Nothing Then

			Dim ParameterList As Singular.Reporting.Dynamic.ROParameterList = Report.ReportCriteriaGeneric.ParameterList()
			If ParameterList IsNot Nothing Then
				SetupDynamicReportCriteria(ParameterList)
			Else
				SetupStronglyTypedReportCriteria(Model.Report.ReportName)
			End If

		Else

			Helpers.Control(Activator.CreateInstance(Report.CustomCriteriaControlType))

		End If

		Report.CustomButtons.Where(Function(c) Not c.AfterNormalButtons).ToList.ForEach(AddressOf AddCustomButton)

		If Report.CrystalReportType IsNot Nothing And Report.HideCrystalReport = False Then
			With Helpers.Button(Singular.Web.ButtonMainStyle.NoStyle, "PDF", "View as PDF")
				'.AddBinding(KnockoutBindingString.click, "ViewPDF()")
				.PostBackType = PostBackType.Full
				.Image.Src = "~/Singular/Images/IconPDF.png"
				.Validate = True
				.ID = "PDF"
			End With
			If Report.ShowWordExport Then
				With Helpers.Button("Word", "View As Word")
					.AddClass("btn btn-primary")
					.PostBackType = PostBackType.Full
					.Image.Src = "~/Singular/Images/IconWord.png"
					.Validate = True
					Style.BackgroundColour = "transparent"

				End With
			End If
		End If
		If Report.AllowDataExport Then
			With Helpers.Button(Singular.Web.ButtonMainStyle.NoStyle, "Data", "Export Data")
				.AddClass("btn btn-primary btn-outline")
				.PostBackType = PostBackType.Full
				.Image.Src = "~/Singular/Images/IconExcelData.png"
				.Validate = True
			End With
		End If

		Dim gi = Report.GridInfo
		If gi IsNot Nothing Then
			With Helpers.Button("GridData", "View Data")
				.AddClass("btn btn-info btn-outline")
				.Image.Src = "~/Singular/Images/IconExcelData.png"
				.AddBinding(KnockoutBindingString.click, "Singular.GridReport.ShowOptions(e)")
			End With
		End If

		Report.CustomButtons.Where(Function(c) c.AfterNormalButtons).ToList.ForEach(AddressOf AddCustomButton)

		With Helpers.Div
			.Style.ClearBoth()
			.Style("height") = "30px"
		End With

	End Sub

	Private Sub AddCustomButton(eb As Singular.Reporting.CustomButton)
		With Helpers.Button(Singular.Web.ButtonMainStyle.NoStyle, "Custom", eb.ButtonText)
			If eb.ImageURL <> "" Then
				If eb.ImageURL.StartsWith("fa fa") Then
					'font-awesome icon
					.Image.Glyph = FontAwesomeIcon.blank
					.Image.AddClass(eb.ImageURL)
				Else
					.Image.Src = eb.ImageURL
				End If

			End If
			.PostBackType = PostBackType.Full
			.AddBinding(KnockoutBindingString.ButtonArgument, eb.ButtonID.ToString.AddSingleQuotes)
			.Validate = True
		End With
	End Sub

	Private Sub SetupStronglyTypedReportCriteria(Optional reportName As String = "")

		With Helpers.With(Of Singular.Reporting.ReportCriteria)(Function(c) c.Report.ReportCriteriaGeneric)

			Dim StartDatePI = Singular.Reflection.GetProperty(Model.Report.ReportCriteriaGeneric.GetType, "StartDate")
			Dim EndDatePI = Singular.Reflection.GetProperty(Model.Report.ReportCriteriaGeneric.GetType, "EndDate")
			Dim OwnerPI = Singular.Reflection.GetProperty(Model.Report.ReportCriteriaGeneric.GetType, "OwnerCounterpartId")

			If StartDatePI IsNot Nothing OrElse EndDatePI IsNot Nothing Then
				'Add Date container
				With .Helpers.FieldSet("Date Selection")

					With .Helpers.DivC("ibox")
						With .Helpers.DivC("row marginLeftRight0")
							With .Helpers.DivC("col-md-12 paddingTopBottom")
								With .Helpers.DivC("ibox-title paddingTitle")
									.Helpers.HTML("<i Class='fa fa-folder fa-lg fa-fw pull-left'></i>")
									.Helpers.HTML.Heading5(reportName)
									With .Helpers.DivC("ibox-tools toolsTopNone4")
										With .Helpers.HTMLTag("a")
											.AddClass("collapse-link")
											With .Helpers.HTMLTag("i")
												.AddClass("fa fa-chevron-up")
											End With
										End With
									End With
								End With
								With .Helpers.DivC("ibox-content")
									With .Helpers.DivC("row")
										'Start Date
										If StartDatePI IsNot Nothing Then
											With .Helpers.Div
												.Style.FloatLeft()
												.Style("padding") = "5px"
												With .Helpers.HTMLTag("span")
													.Style.Padding(, , "5px")
													.Style.Display = Display.block
													.AddBinding(KnockoutBindingString.text, "'Start Date: '+ dateFormat(StartDate(), 'dd MMM yyyy')")
												End With

												.Helpers.Control(Singular.Web.CustomControls.EditorBase(Of ReportVM).GetEditor(StartDatePI))
											End With
										End If

										'End Date
										If EndDatePI IsNot Nothing Then
											With .Helpers.Div()
												.Style.FloatLeft()
												.Style("padding") = "5px"
												With .Helpers.HTMLTag("span")
													.Style.Padding(, , "5px")
													.Style.Display = Display.block
													.AddBinding(KnockoutBindingString.text, "'End Date: '+ dateFormat(EndDate(), 'dd MMM yyyy')")
												End With
												.Helpers.Control(Singular.Web.CustomControls.EditorBase(Of ReportVM).GetEditor(EndDatePI))
											End With
										End If
									End With
								End With
							End With
						End With
					End With
				End With

			End If

			Dim mCriteriaGroup As Singular.Web.CustomControls.FieldSet(Of Singular.Reporting.ReportCriteria, Singular.Reporting.ReportCriteria) = Nothing

			With .Helpers.DivC("ibox")
				With .Helpers.DivC("row marginLeftRight0")
					With .Helpers.DivC("col-md-12 paddingTopBottom")
						With .Helpers.DivC("ibox-title paddingTitle")
							.Helpers.HTML("<i class='fa fa-file fa-lg fa-fw pull-left'></i>")
							.Helpers.HTML().Heading5("Criteria")
							With .Helpers.DivC("ibox-tools toolsTopNone4")
								With .Helpers.HTMLTag("a")
									.AddClass("collapse-link")
									With .Helpers.HTMLTag("i")
										.AddClass("fa fa-chevron-up")
									End With
								End With
							End With
						End With
						With .Helpers.DivC("ibox-content")
							With .Helpers.DivC("row")

								Model.Report.ReportCriteriaGeneric.GetType.ForEachBrowsableProperty(Nothing,
										 Sub(pi)

											 If pi IsNot StartDatePI AndAlso pi IsNot EndDatePI Then

												 If mCriteriaGroup Is Nothing Then
													 mCriteriaGroup = .Helpers.FieldSet("Criteria")
												 End If

												 If pi IsNot OwnerPI Then
													 With mCriteriaGroup.Helpers.DivC("col-md-4")
														 With .Helpers.DivC("")
															 With .Helpers.LabelFor(pi)
																 .AddClass("control-label")

															 End With
															 With .Helpers.EditorFor(pi)
																 .AddClass("form-control")

															 End With
														 End With
													 End With
												 Else
													 With mCriteriaGroup.Helpers.DivC("col-md-2")
														 With .Helpers.DivC("")
															 With .Helpers.LabelFor(pi)
																 .AddClass("control-label")

															 End With
															 With .Helpers.EditorFor(pi)
																 .AddClass("form-control comboTriggerWidth noMargin-Bottom")

															 End With
															 'With .Helpers.DivC("col-md-8")
															 ' With .Helpers.SpanC("form-control marginTop23")
															 '	 .ID = "OwnerDisplay"
															 ' End With
															 'End With
														 End With

													 End With
												 End If

											 End If

										 End Sub, , True, True)

							End With
						End With
					End With
				End With
			End With
		End With

	End Sub

	Private Sub SetupDynamicReportCriteria(ParameterList As Singular.Reporting.Dynamic.ROParameterList)

		With Helpers.With(Of Singular.Reporting.ReportCriteria)(Function(c) c.Report.ReportCriteriaGeneric)

			Dim StartDateP As ROParameter = ParameterList.Find("StartDate")
			Dim EndDateP As ROParameter = ParameterList.Find("EndDate")


			If StartDateP IsNot Nothing OrElse EndDateP IsNot Nothing Then
				'Add Date container
				With .Helpers.FieldSet("Date Selection")

					With .Helpers.DivC("ibox")
						With .Helpers.DivC("row")
							With .Helpers.DivC("col-md-12 paddingTopBottom")
								With .Helpers.DivC("ibox-content")
									With .Helpers.DivC("row")
										' Julzy We need to add java script here for an on click event for the date picker "date" heading to change color - it also needs to be font size 16px
										'Start Date
										If StartDateP IsNot Nothing Then
											With .Helpers.Div
												.Style.FloatLeft()
												.Style("padding") = "5px"
												With .Helpers.HTMLTag("span")
													.Style.Padding(, , "5px")
													.Style("font-size") = "16px"
													.Style.Display = Display.block
													.AddBinding(KnockoutBindingString.text, "'Start Date: '+ dateFormat(StartDate(), 'dd MMM yyyy')")
												End With

												Dim de As New Singular.Web.CustomControls.DateEditor(Of Object)("StartDate", "",
																			New Singular.DataAnnotations.DateField() With {.AlwaysShow = True,
																																										 .AutoChange = Singular.DataAnnotations.AutoChangeType.StartOfMonth,
																																										 .MaxDateProperty = If(EndDateP IsNot Nothing, "EndDate", "")})
												.Helpers.Control(de)
											End With
										End If

										'End Date
										If EndDateP IsNot Nothing Then
											With .Helpers.Div()
												.Style.FloatLeft()
												.Style("padding") = "5px"
												With .Helpers.HTMLTag("span")
													.Style.Padding(, , "5px")
													.Style.FontSize() = "16px"
													.Style.Display = Display.block
													.AddBinding(KnockoutBindingString.text, "'End Date: '+ dateFormat(EndDate(), 'dd MMM yyyy')")
												End With

												Dim de As New Singular.Web.CustomControls.DateEditor(Of Object)("EndDate", "",
																			New Singular.DataAnnotations.DateField() With {.AlwaysShow = True,
																																										 .AutoChange = Singular.DataAnnotations.AutoChangeType.EndOfMonth,
																																										 .MinDateProperty = If(StartDateP IsNot Nothing, "StartDate", "")})
												.Helpers.Control(de)
											End With
										End If

									End With
								End With
							End With
						End With
					End With
				End With

			End If

			Dim mCriteriaGroup As Singular.Web.CustomControls.FieldSet(Of Singular.Reporting.ReportCriteria, Singular.Reporting.ReportCriteria) = Nothing

			With .Helpers.DivC("ibox")
				With .Helpers.DivC("row")
					With .Helpers.DivC("col-md-12 paddingTopBottom")
						With .Helpers.DivC("ibox-content")
							With .Helpers.DivC("row")

								For Each Param As ROParameter In ParameterList

									If Param IsNot StartDateP AndAlso Param IsNot EndDateP AndAlso Param.Visible Then

										If mCriteriaGroup Is Nothing Then
											mCriteriaGroup = .Helpers.FieldSet("Criteria")
										End If

										Dim Editor As Singular.Web.CustomControls.EditorBase(Of Object) = Nothing

										Dim ddd As Singular.Reporting.Dynamic.DynamicDropDown = Nothing
										If Param.DropDownSource IsNot Nothing Then
											ddd = Singular.Reporting.Dynamic.Settings.DropDowns.IncludeDatabaseDropDowns.GetItem(Param.DropDownSource)
										End If

										If ddd IsNot Nothing Then
											'Drop down

											Dim di = ddd.GetDynamicInfo
											If di.Data IsNot Nothing Then
												CType(Model, ReportVM).ClientDataProvider.AddDataSource(di.DropDownInfo.ClientName, di.Data, False)
											End If
											Editor = New Singular.Web.CustomControls.DropDownEditor(Of Object)(Param.ParameterName, "", di.DropDownInfo)
										Else

											'Other
											Select Case Param.ParamDataType
												Case Reflection.SMemberInfo.MainType.String
													Editor = New Singular.Web.CustomControls.TextEditor(Of Object)(Param.ParameterName, "", Nothing)
												Case Reflection.SMemberInfo.MainType.Number
													Editor = New Singular.Web.CustomControls.NumericEditor(Of Object)(Param.ParameterName, "", Nothing)
												Case Reflection.SMemberInfo.MainType.Date
													Editor = New Singular.Web.CustomControls.DateEditor(Of Object)(Param.ParameterName, "", Nothing)
												Case Reflection.SMemberInfo.MainType.Boolean
													Editor = New Singular.Web.CustomControls.CheckBoxEditor(Of Object)(Param.ParameterName, "")
											End Select
										End If



										mCriteriaGroup.Helpers.Control(New Singular.Web.CustomControls.EditorRow(Of Object)(Editor, New Singular.Web.CustomControls.FieldLabel(Of Object)(Param.ParameterName, Param.DisplayName)))
										Editor.Style.Width = 300

									End If

								Next
							End With
						End With
					End With
				End With
			End With
		End With

	End Sub

	Protected Overrides Sub Render()
		MyBase.Render()

		RenderChildren()
	End Sub

End Class

Public MustInherit Class CriteriaControlBase
	Inherits Controls.HelperControls.HelperBase(Of ReportVM)
	Protected Overrides Sub Render()
		MyBase.Render()
		RenderChildren()
	End Sub
End Class