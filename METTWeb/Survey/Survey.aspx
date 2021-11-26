<%@ Page Title="METT - Assessments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Survey.aspx.cs" Inherits="METTWeb.Survey.Survey" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/assessment.css" rel="stylesheet" />
	<link href="../Theme/Singular/METTCustomCss/customstyles.css" rel="stylesheet" />
	<link href="../Theme/Singular/METTCustomCss/survey.css" rel="stylesheet" />
	<link rel="stylesheet" type="text/css" media="screen" href="../Theme/tinyTips.css" />
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<%	
		using (var h = this.Helpers)
		{
			var MainHDiv = h.DivC("row pad-top-10");
			{
				var QuestionnaireGroupContainer = MainHDiv.Helpers.DivC("container");
				{
					var QuestionnaireGroupBreadcrumbRow = QuestionnaireGroupContainer.Helpers.DivC("row");
					{
						var MainDiv = MainHDiv.Helpers.DivC("col-md-12 p-n-lr");
						{
							var TestDiv = MainDiv.Helpers.Div();
							{
								var QuestionnaireContentDivRow = TestDiv.Helpers.DivC("row");
								{
									var QuestionnaireMessageDiv = QuestionnaireContentDivRow.Helpers.DivC("col-md-12");
									{
										QuestionnaireMessageDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingQuestionnaireMessageInd == true);
										var PanelContainerBox = QuestionnaireMessageDiv.Helpers.DivC("ibox float-e-margins paddingBottom");
										{
											var PanelContainerBoxTitle = PanelContainerBox.Helpers.DivC("ibox-title");
											{
												PanelContainerBoxTitle.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
												PanelContainerBoxTitle.Helpers.HTML().Heading5("Assessments");
											}
											var PanelContainerBoxTools = PanelContainerBoxTitle.Helpers.DivC("ibox-tools");
											{
												var PanelContainerBoxToolsTag = PanelContainerBoxTools.Helpers.HTMLTag("a");
												PanelContainerBoxToolsTag.AddClass("collapse-link");
												{
													var iPanelContainerBoxToolsTag = PanelContainerBoxToolsTag.Helpers.HTMLTag("i");
													iPanelContainerBoxToolsTag.AddClass("fa fa-chevron-up");
												}
											}
											var PanelContainerContentBox = PanelContainerBox.Helpers.DivC("ibox-content");
											{
												var FilterRow = PanelContainerContentBox.Helpers.DivC("row filterFrame");
												{

													var FilterAssessmentStatusSearchBoxCol = FilterRow.Helpers.DivC("col-md-4 p-r-n");
													{
														FilterAssessmentStatusSearchBoxCol.Helpers.LabelFor(c => c.ROAssessmentPagedListCriteria.METTReportingName);
														FilterAssessmentStatusSearchBoxCol.AddClass("control-label");
														var FilterAssessmentStatusEditor = FilterAssessmentStatusSearchBoxCol.Helpers.EditorFor(c => c.ROAssessmentPagedListCriteria.METTReportingName);
														FilterAssessmentStatusEditor.AddClass("form-control marginBottom20 filterBox");
													}

											
													var FilterYearCol = FilterRow.Helpers.DivC("col-md-1");
													{
														FilterYearCol.Helpers.LabelFor(c => c.ROAssessmentPagedListCriteria.QuestionnaireYear);
														FilterYearCol.AddClass("control-label");

														var FilterYearEditor = FilterYearCol.Helpers.EditorFor(c => c.ROAssessmentPagedListCriteria.QuestionnaireYear);
														FilterYearEditor.AddClass("form-control marginBottom20 filterBox");
													}

													var FilterProvinceCol = FilterRow.Helpers.DivC("col-md-2 p-l-n");
													{
														FilterProvinceCol.Helpers.LabelFor(c => c.ROAssessmentPagedListCriteria.QuestionnaireStatusID);
														FilterProvinceCol.AddClass("control-label");

														var FilterProvinceEditor = FilterProvinceCol.Helpers.EditorFor(c => c.ROAssessmentPagedListCriteria.QuestionnaireStatusID);
														FilterProvinceEditor.AddClass("form-control marginBottom20 filterBox");
													}

													var AuditedCol = FilterRow.Helpers.DivC("col-md-2 p-l-n");
													{
														AuditedCol.Helpers.LabelFor(c => c.ROAssessmentPagedListCriteria.AuditedInd);
														AuditedCol.AddClass("control-label");

														var AuditedEditor = AuditedCol.Helpers.EditorFor(c => c.ROAssessmentPagedListCriteria.AuditedInd);
														AuditedEditor.AddClass("form-control marginBottom20 filterBox");
													}

													var SearchButtonCol = FilterRow.Helpers.DivC("col-md-3 p-l-n p-t-m");
													{
														var SearchBtn = SearchButtonCol.Helpers.Button("Search", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
														{
															SearchBtn.Style.FloatRight();
															SearchBtn.AddClass("btn-primary btn pull-left filterButtonSurveyWidth");
															SearchBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewModel.ROAssessmentPagedListManager().Refresh();");
														}
                                                        var ClearBtn = SearchButtonCol.Helpers.Button("Clear", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
															{
																ClearBtn.Style.FloatRight();
																ClearBtn.AddClass("btn-default btn filterButtonSurveyWidth");
																ClearBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ClearAssessmentFilters()");
															}
													}

												}

												var AssessmentDiv = PanelContainerContentBox.Helpers.DivC("row");
												{
													var AssessmentDivCol = AssessmentDiv.Helpers.DivC("col-lg-12");
													{
														var AssessmentPagedList = AssessmentDivCol.Helpers.PagedGridFor<METTLib.Home.ROAssessmentPaged>(c => c.ROAssessmentPagedListManager, c => c.ROAssessmentPagedList, false, false);
														{
															AssessmentPagedList.AddClass("table-responsive table-striped table table-bordered");
															var AssessmentFirstRow = AssessmentPagedList.FirstRow;
															{
																var METTReportingNameCol = AssessmentFirstRow.AddReadOnlyColumn(c => c.METTReportingName);
																var OfficialName = AssessmentFirstRow.AddReadOnlyColumn(c => c.OfficialName);
																var OrganisationName = AssessmentFirstRow.AddReadOnlyColumn(c => c.OrganisationName);
																var CreatedBy = AssessmentFirstRow.AddReadOnlyColumn(c => c.CreatedBy);
																{
																	CreatedBy.Style.TextAlign = Singular.Web.TextAlign.left;
																}

																var AssessmentDate = AssessmentFirstRow.AddReadOnlyColumn(c => c.AssessmentDate);
																{
																	AssessmentDate.Style.TextAlign = Singular.Web.TextAlign.left;
																}

																var AssessmentStatus = AssessmentFirstRow.AddReadOnlyColumn(c => c.QuestionnaireStatus);
																{
																	AssessmentStatus.Style.TextAlign = Singular.Web.TextAlign.left;
																}

																var AuditedInd = AssessmentFirstRow.AddColumn("Audited");
																{
																	AuditedInd.Attributes.Add("style", "width:7px; text-align: center;");
																	var AuditedIndInd = AuditedInd.Helpers.DivC("b-r-xl");
																	var AuditedIndText = AuditedIndInd.Helpers.Span(c => c.AuditedInd == true ? "Yes" : "No");
																	AuditedIndInd.Attributes.Add("style", "padding:5px; max-width:50px;");
																	AuditedIndInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-primary': $data.AuditedInd() == true, 'bg-danger': $data.AuditedInd() == false}");
																}
																if (Singular.Settings.CurrentUser.Roles.Contains("Organisations.Access"))
																{
																	var ViewCol = AssessmentFirstRow.AddColumn();
																	{
																		ViewCol.Style.Width = "150px";
																		ViewCol.HeaderText = "View";
																		var viewBtn = ViewCol.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																		{
																			viewBtn.AddClass("btn btn-outline btn-primary");
																			viewBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewAssessment($data)");
																			viewBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewAssessmentBtnInd == true);
																		}
																		var deleteBtn = ViewCol.Helpers.Button("Delete", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																		{
																			deleteBtn.AddClass("btn btn-outline btn-danger btn");
																			deleteBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "DeleteAssessment($data)");
																			deleteBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageDeleteAssessmentBtnInd == true && c.AssessmentDate == null);
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
										var QuestionnaireContentDiv = QuestionnaireContentDivRow.Helpers.DivC("col-md-12");
										{
											QuestionnaireContentDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingQuestionnaireContentInd == true);
											var TabContainer = QuestionnaireContentDiv.Helpers.DivC("tabs-container");
											{
												TabContainer.Attributes.Add("id", "page-tabs");
												var EntityTab = TabContainer.Helpers.TabControl(c => ViewModel.SelectedTab);

												TabContainer.Attributes.Add("style", "padding-bottom: 50;");
												{
													EntityTab.Style.ClearBoth();
													EntityTab.AddClass("nav nav-tabs");


													// Overview Tab
													#region OverviewTab
													var OverviewTab = EntityTab.AddTab("Overview");
													{
														#region OverviewDetails
														var OverviewRow = OverviewTab.Helpers.DivC("");
														{
															var OverviewQuestionnaireAnswerSet = OverviewRow.Helpers.With<METTLib.Questionnaire.QuestionnaireAnswerSet>(c => c.FirstQuestionnaireAnswerSet);
															{
																OverviewRow.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingSurveyInd == true);
																var OverviewText = OverviewQuestionnaireAnswerSet.Helpers.DivC("row margin0");
																{
																	var CardHeaderCol = OverviewText.Helpers.DivC("col-md-12 p-n-lr");
																	{
																		CardHeaderCol.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewOverviewTabInd == true);
																		var CardHeaderColContainerBox = CardHeaderCol.Helpers.DivC("ibox float-e-margins paddingBottom");
																		{
																			var CardHeaderColContainerContentBox = CardHeaderColContainerBox.Helpers.DivC("ibox-content");
																			{
																				var CardHeaderColContainerRowInner0 = CardHeaderColContainerContentBox.Helpers.DivC("row");
																				{
																					var CardHeaderColContainerRowInnerCol1 = CardHeaderColContainerRowInner0.Helpers.DivC("col-md-12");
																					{
																						CardHeaderColContainerRowInnerCol1.Helpers.HTML("<h2 style='color:#1ab394'>Management Effectiveness Tracking Tool-South Africa</h2>");
																						CardHeaderColContainerRowInnerCol1.Helpers.HTML("<h3 data-bind=\"text: $data.QuestionnaireNameVersion() \"></h3>");
																						CardHeaderColContainerRowInnerCol1.Helpers.HTML("<p>A site level self-evaluation tool for reporting progress in management effectiveness.</p>");
																						CardHeaderColContainerRowInnerCol1.Helpers.HTML("<p class='paddingBottom'>The third South African adaptation of the World Bank/ WWF Management Effectiveness Tracking Tool.</p>");
																					}
																					var OverviewOptions = CardHeaderColContainerRowInnerCol1.Helpers.DivC("");
																					{
																						var FAQBtnsDiv = OverviewOptions.Helpers.DivC("pad-top-15");
																						var ViewRoleBtn = FAQBtnsDiv.Helpers.Button("View Role of Assessment", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																						{
																							ViewRoleBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewRoleFAQ()");
																							ViewRoleBtn.AddClass("btn btn-primary btn-outline");
																						}
																						var ViewInstructionsBtn = FAQBtnsDiv.Helpers.Button("View Assessment Instructions", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																						{
																							ViewInstructionsBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewInstructionsFAQ()");
																							ViewInstructionsBtn.AddClass("btn btn-primary btn-outline");
																						}
																					}
																				}
																			}
																		}
																	}
																	var Card1Col = OverviewText.Helpers.DivC("col-md-4 p-l-n");
																	{
																		Card1Col.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewOverviewTabInd == true);
																		var Card1ColContainerBox = Card1Col.Helpers.DivC("ibox float-e-margins paddingBottom overviewSmallBoxHeight");
																		{
																			var Card1ColContainerContentBox = Card1ColContainerBox.Helpers.DivC("ibox-content");
																			{
																				var Card1ColContainerRowInner0 = Card1ColContainerContentBox.Helpers.DivC("row");
																				{
																					var Card1ColContainerRowInnerCol1 = Card1ColContainerRowInner0.Helpers.DivC("col-md-12");
																					{
																						Card1ColContainerRowInnerCol1.Helpers.HTML("<h2 style='color:#1ab394'>Reporting Name</h2>");

																						var ViewPABtn = Card1ColContainerRowInnerCol1.Helpers.Button("", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																						{
																							ViewPABtn.AddBinding(Singular.Web.KnockoutBindingString.text, c => c.METTReportingName);
																							ViewPABtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewProtectedArea($data)");
																							ViewPABtn.AddClass("btnHyperlink");
																						}
																					}
																				}
																			}
																		}
																	}
																	var Card2Col = OverviewText.Helpers.DivC("col-md-4 p-l-n");
																	{
																		Card2Col.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewOverviewTabInd == true);
																		var Card2ColContainerBox = Card2Col.Helpers.DivC("ibox float-e-margins paddingBottom overviewSmallBoxHeight");
																		{
																			var Card2ColContainerContentBox = Card2ColContainerBox.Helpers.DivC("ibox-content");
																			{
																				var Card2ColContainerRowInner0 = Card2ColContainerContentBox.Helpers.DivC("row");
																				{
																					var Card2ColContainerRowInnerCol1 = Card2ColContainerRowInner0.Helpers.DivC("col-md-12");
																					{
																						Card2ColContainerRowInnerCol1.Helpers.HTML("<h2 style='color:#1ab394'>Organisation</h2>");

																						var ViewOrgBtn = Card2ColContainerRowInnerCol1.Helpers.Button("", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																						{
																							ViewOrgBtn.AddBinding(Singular.Web.KnockoutBindingString.text, c => c.OrganisationName);
																							ViewOrgBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewOrganisation($data)");
																							ViewOrgBtn.AddClass("btnHyperlink");
																						}
																					}
																				}
																			}
																		}
																	}
																	var Card3Col = OverviewText.Helpers.DivC("col-md-4 p-n-lr");
																	{
																		Card3Col.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewOverviewTabInd == true);
																		var Card3ColContainerBox = Card3Col.Helpers.DivC("ibox float-e-margins paddingBottom overviewSmallBoxHeight");
																		{
																			var Card3ColContainerContentBox = Card3ColContainerBox.Helpers.DivC("ibox-content");
																			{
																				var Card3ColContainerRowInner0 = Card3ColContainerContentBox.Helpers.DivC("row");
																				{
																					var Card3ColContainerRowInnerCol1 = Card3ColContainerRowInner0.Helpers.DivC("col-md-12");
																					{
																						Card3ColContainerRowInnerCol1.Helpers.HTML("<h2 style='color:#1ab394'>Official Name</h2>");
																						Card3ColContainerRowInnerCol1.Helpers.HTML("<span class='text-center' data-bind=\"text: '&nbsp;' + $data.OfficialName()\">&nbsp; </span>");
																					}
																				}
																			}
																		}
																	}
																	var Card4Col = OverviewText.Helpers.DivC("col-md-6 p-l-n");
																	{
																		Card4Col.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewOverviewTabInd == true);
																		var Card4ColContainerBox = Card4Col.Helpers.DivC("ibox float-e-margins paddingBottom");
																		{
																			var Card4ColContainerContentBox = Card4ColContainerBox.Helpers.DivC("ibox-content");
																			{
																				var Card4ColContainerRowInner0 = Card4ColContainerContentBox.Helpers.DivC("row");
																				{
																					var Card4ColContainerRowInnerCol1 = Card4ColContainerRowInner0.Helpers.DivC("col-md-12 ");
																					{
																						Card4ColContainerRowInnerCol1.Helpers.HTML("<h2 style='color:#1ab394'>Legal Designations</h2>");
																						Card4ColContainerRowInnerCol1.Helpers.HTML("<span class='text-center' data-bind=\"text: $data.LegalDesignation()\">&nbsp;</span>");
																					}
																				}
																			}
																		}
																	}

																	var Card5Col = OverviewText.Helpers.DivC("col-md-6 p-n-lr");
																	{
																		Card5Col.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewOverviewTabInd == true);
																		var Card5ColContainerBox = Card5Col.Helpers.DivC("ibox float-e-margins paddingBottom");
																		{
																			var Card5ColContainerContentBox = Card5ColContainerBox.Helpers.DivC("ibox-content");
																			{
																				var Card5ColContainerRowInner0 = Card5ColContainerContentBox.Helpers.DivC("row");
																				{
																					var Card5ColContainerRowInnerCol1 = Card5ColContainerRowInner0.Helpers.DivC("col-md-12 ");
																					{
																						Card5ColContainerRowInnerCol1.Helpers.HTML("<h2 style='color:#1ab394'>Area</h2>");
																						Card5ColContainerRowInnerCol1.Helpers.HTML("<span class='text-center' data-bind=\"text: $data.AreaWithHectares()\">&nbsp;</span>");
																					}
																				}
																			}
																		}
																	}


																}
															}
														}
														var FAQPaddingDiv = OverviewTab.Helpers.DivC("row");
														{
															FAQPaddingDiv.Helpers.DivC("col-md-12");
														}
														#endregion
														var Card1 = OverviewRow.Helpers.DivC("col-md-12  p-n-lr");
														{
															Card1.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewOverviewTabInd == true);
															var Card5ColContainerBox = Card1.Helpers.DivC("ibox float-e-margins paddingBottom");
															{
																var Card5ColContainerContentBox = Card5ColContainerBox.Helpers.DivC("ibox-content");
																{
																	var Card5ColContainerRowInner0 = Card5ColContainerContentBox.Helpers.DivC("row");
																	{
																		var Card5ColContainerRowInnerCol1 = Card5ColContainerRowInner0.Helpers.DivC("col-md-12");
																		{
																			Card5ColContainerRowInnerCol1.Helpers.HTML("<h2 style='color:#1ab394'>Biomes</h2>");
																			var Biomes = Card5ColContainerRowInnerCol1.Helpers.ForEach<METTLib.Questionnaire.RO.ROQuestionnaireAnswerSetNationalBiomeList>(c => c.ROQuestionnaireAnswerSetNationalBiomeList);
																			{
																				Biomes.Helpers.HTML("<span data-bind=\"text: $data.NationalBiome()\"></span>,");
																			}
																		}
																	}
																}
															}

															var OverviewNoAccess = OverviewRow.Helpers.DivC("col-md-12  p-n-lr");
															{
																OverviewNoAccess.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewOverviewTabInd == false);
																var OverviewNoAccessColContainerBox = OverviewNoAccess.Helpers.DivC("ibox float-e-margins paddingBottom");
																{
																	var OverviewNoAccessContentBox = OverviewNoAccessColContainerBox.Helpers.DivC("ibox-content");
																	{
																		var OverviewNoAccessContentBoxRow = OverviewNoAccessContentBox.Helpers.DivC("row");
																		{
																			var OverviewNoAccessContentBoxRowCol = OverviewNoAccessContentBoxRow.Helpers.DivC("col-md-12 ");
																			{
																				OverviewNoAccessContentBoxRowCol.Helpers.HTML("<h2 style='color:#1ab394;'>Assessment Overview</h2>");
																				OverviewNoAccessContentBoxRowCol.Helpers.HTML("<span>You do not have access. Please confirm your security role/group settings.</span>");
																			}
																		}
																	}
																}
															}
														}

														var ButtonsRow = OverviewRow.Helpers.DivC("col-md-12 p-n-lr");
														{
															var BackBtn = ButtonsRow.Helpers.Button("Back");
															{
																BackBtn.AddClass("btn btn-default pull-left marginBottomLeftButton ");
																BackBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "Back($data)");
															}
														}

														#region FAQAssessmentInstructions
														var FAQInstructionsDiv = OverviewTab.Helpers.DivC("row margin0");
														{
															FAQInstructionsDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingFAQInstructionsInd == true);
															FAQInstructionsDiv.Helpers.DivC("col-md-12");
															var FAQInstructionsDivContent = FAQInstructionsDiv.Helpers.With<METTLib.Questionnaire.QuestionnaireGuidelineList>(c => c.FirstFAQAssessmentInstructions);
															FAQInstructionsDivContent.Helpers.HTML("<h2 data-bind=\"text: $data.QuestionnaireGuidelineHeading()\"></h2>");
															FAQInstructionsDivContent.Helpers.HTML("<div data-bind=\"html: $data.QuestionnaireGuidelineContent()\"></div>");
															var ViewCloseBtn = FAQInstructionsDiv.Helpers.Button("Close", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
															{
																ViewCloseBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "CloseFAQ($data)");
																ViewCloseBtn.AddClass("btn btn btn-primary btn");
															}
														}
														#endregion
														#region FAQRoleofAssessment
														var FAQRoleDiv = OverviewTab.Helpers.DivC("row margin0");
														{
															FAQRoleDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingFAQRoleInd == true);
															FAQRoleDiv.Helpers.DivC("col-md-12");
															var FAQRoleDivContent = FAQRoleDiv.Helpers.With<METTLib.Questionnaire.QuestionnaireGuidelineList>(c => c.FirstFAQRoleofAssessment);
															FAQRoleDivContent.Helpers.HTML("<h2 data-bind=\"text: $data.QuestionnaireGuidelineHeading()\"></h2>");
															FAQRoleDivContent.Helpers.HTML("<div data-bind=\"html: $data.QuestionnaireGuidelineContent()\"></div>");
															var ViewCloseBtn = FAQRoleDiv.Helpers.Button("Close", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
															{
																ViewCloseBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "CloseFAQ($data)");
																ViewCloseBtn.AddClass("btn btn btn-primary btn");
															}
														}
														#endregion
													}
													#endregion

													// Threats Tab
													#region ThreatsTab
													var ThreatsTab = EntityTab.AddTab("Threats");
													{
														#region ThreatsSelected
														var SelectedThreatsDiv = ThreatsTab.Helpers.DivC("ibox float-e-margins paddingBottom");
														{
															SelectedThreatsDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewThreatsTabInd == true);
															var SelectedThreatsTitleDiv = SelectedThreatsDiv.Helpers.DivC("ibox-title");
															{
																SelectedThreatsTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
																SelectedThreatsTitleDiv.Helpers.HTML().Heading5("Threats");
															}
															var SelectedThreatsToolsDiv = SelectedThreatsTitleDiv.Helpers.DivC("ibox-tools");
															{
																var aSelectedThreatsToolsTag = SelectedThreatsToolsDiv.Helpers.HTMLTag("a");
																aSelectedThreatsToolsTag.AddClass("collapse-link");
																{
																	var iSelectedThreatsToolsTag = aSelectedThreatsToolsTag.Helpers.HTMLTag("i");
																	iSelectedThreatsToolsTag.AddClass("fa fa-chevron-up");
																}
															}
															var SelectedThreatsDivContentDiv = SelectedThreatsDiv.Helpers.DivC("ibox-content");
															{
																var SelectedThreatsDivContentDivRow = SelectedThreatsDivContentDiv.Helpers.DivC("row assessmentMessageBox");
																{
																	var ThreatMessageDiv = SelectedThreatsDivContentDivRow.Helpers.DivC("col-md-12 text-center pad-top-15");
																	{
																		ThreatMessageDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingThreatsMessageInd == true);
																		ThreatMessageDiv.Helpers.Span("<div><i class='fa fa-info-circle infoIconStyle'></i></div><i class='assessmentMessage'>To add threats to this assessment's protected area / site, please choose from the available threats list below.</i><br><br>");
																	}
																	var ThreatDataDiv = SelectedThreatsDivContentDivRow.Helpers.DivC("col-md-12");
																	{
																		ThreatDataDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingThreatsTableInd == true);
																		var ProtectedAreaThreatsItems = ThreatDataDiv.Helpers.BootstrapTableFor<METTLib.Threats.ThreatAnswer>((c) => c.ThreatAnswersList, false, false, "");
																		{
																			var ProtectedAreaThreatsItemsTableFirstRow = ProtectedAreaThreatsItems.FirstRow;
																			{
																				var ProtectedAreaThreatsItemNameCol = ProtectedAreaThreatsItemsTableFirstRow.AddColumn("Previously Defined Threats");
																				{
																					ProtectedAreaThreatsItemNameCol.Attributes.Add("style", "");
																					var ProtectedAreaThreatsItemNameText = ProtectedAreaThreatsItemNameCol.Helpers.Span(c => c.ThreatCategoryName + " | " + c.ThreatSubCategoryName);

																					var TooltipICON = ProtectedAreaThreatsItemNameCol.Helpers.HTMLTag("div");
																					{
																						TooltipICON.AddClass("tTip fa fa-info-circle infoIconBlue");
																						TooltipICON.AddBinding(Singular.Web.KnockoutBindingString.title, c => c.ThreatTooltip);
																					}

																					ProtectedAreaThreatsItemNameCol.Helpers.HTMLTag("br");
																					ProtectedAreaThreatsItemNameCol.Helpers.Span(c => c.RemovedInd == true ? "THREAT NO LONGER ACTIVE" : "");
																				}

																				var ProtectedAreaThreatsItemScopeCol = ProtectedAreaThreatsItemsTableFirstRow.AddColumn("Scope");
																				{
																					ProtectedAreaThreatsItemScopeCol.Attributes.Add("style", "width:100px;text-align: center; ");
																					var ProtectedAreaThreatsItemScopeInd = ProtectedAreaThreatsItemScopeCol.Helpers.DivC("b-r-xl");
																					ProtectedAreaThreatsItemScopeInd.Attributes.Add("style", "padding:5px;");
																					var ProtectedAreaThreatsItemScopeText = ProtectedAreaThreatsItemScopeInd.Helpers.Span(c => c.ScopeRatingName);
																					ProtectedAreaThreatsItemScopeInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{" +
																						"'bg-info': ($data.RemovedInd() == 0 && $data.ScopeRatingName() == 'Low'), " +
																						"'bg-primary': ($data.RemovedInd() == 0 && $data.ScopeRatingName() == 'Medium'), " +
																						"'bg-warning': ($data.RemovedInd() == 0 && $data.ScopeRatingName() == 'High'), " +
																						"'bg-danger': ($data.RemovedInd() == 0 && $data.ScopeRatingName() == 'Very High')	}");
																				}
																				var ProtectedAreaThreatsItemSeverityCol = ProtectedAreaThreatsItemsTableFirstRow.AddColumn("Severity");
																				{
																					ProtectedAreaThreatsItemSeverityCol.Attributes.Add("style", "width:100px;text-align: center;");
																					var ProtectedAreaThreatsItemSeverityInd = ProtectedAreaThreatsItemSeverityCol.Helpers.DivC("b-r-xl");
																					ProtectedAreaThreatsItemSeverityInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-muted': $data.RemovedInd() == 1}");
																					ProtectedAreaThreatsItemSeverityInd.Attributes.Add("style", "padding:5px;");
																					var ProtectedAreaThreatsItemSeverityText = ProtectedAreaThreatsItemSeverityInd.Helpers.Span(c => c.SeverityRatingName);
																					ProtectedAreaThreatsItemSeverityInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{" +
																						"'bg-info': ($data.RemovedInd() == 0 && $data.SeverityRatingName() == 'Low'), " +
																						"'bg-primary': ($data.RemovedInd() == 0 && $data.SeverityRatingName() == 'Medium'), " +
																						"'bg-warning': ($data.RemovedInd() == 0 && $data.SeverityRatingName() == 'High'), " +
																						"'bg-danger': ($data.RemovedInd() == 0 && $data.SeverityRatingName() == 'Very High')	}");
																				}
																				var ProtectedAreaThreatsItemIrreversibilityCol = ProtectedAreaThreatsItemsTableFirstRow.AddColumn("Irreversibility");
																				{
																					ProtectedAreaThreatsItemIrreversibilityCol.Attributes.Add("style", "width:100px;text-align: center;");
																					var ProtectedAreaThreatsItemIrreversibilityInd = ProtectedAreaThreatsItemIrreversibilityCol.Helpers.DivC("b-r-xl");
																					ProtectedAreaThreatsItemIrreversibilityInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-muted': $data.RemovedInd() == 1}");
																					ProtectedAreaThreatsItemIrreversibilityInd.Attributes.Add("style", "padding:5px;");
																					var ProtectedAreaThreatsItemIrreversibilityText = ProtectedAreaThreatsItemIrreversibilityInd.Helpers.Span(c => c.IrreversibilityRatingName);
																					ProtectedAreaThreatsItemIrreversibilityInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{" +
																						"'bg-info': ($data.RemovedInd() == 0 && $data.IrreversibilityRatingName() == 'Low'), " +
																						"'bg-primary': ($data.RemovedInd() == 0 && $data.IrreversibilityRatingName() == 'Medium'), " +
																						"'bg-warning': ($data.RemovedInd() == 0 && $data.IrreversibilityRatingName() == 'High'), " +
																						"'bg-danger': ($data.RemovedInd() == 0 && $data.IrreversibilityRatingName() == 'Very High')	}");
																				}
																				var ProtectedAreaThreatsItemMagnitudeCol = ProtectedAreaThreatsItemsTableFirstRow.AddColumn("Magnitude");
																				{
																					ProtectedAreaThreatsItemMagnitudeCol.Attributes.Add("style", "width:100px;text-align: center;");
																					var ProtectedAreaThreatsItemMagnitudeInd = ProtectedAreaThreatsItemMagnitudeCol.Helpers.DivC("b-r-xl");
																					ProtectedAreaThreatsItemMagnitudeInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-muted': $data.RemovedInd() == 1}");
																					ProtectedAreaThreatsItemMagnitudeInd.Attributes.Add("style", "padding:5px;");
																					var ProtectedAreaThreatsItemMagnitudeText = ProtectedAreaThreatsItemMagnitudeInd.Helpers.Span(c => c.MagnitudeRatingName);
																					ProtectedAreaThreatsItemMagnitudeInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{" +
																						"'bg-info': ($data.RemovedInd() == 0 && $data.MagnitudeRatingName() == 'Low'), " +
																						"'bg-primary': ($data.RemovedInd() == 0 && $data.MagnitudeRatingName() == 'Medium'), " +
																						"'bg-warning': ($data.RemovedInd() == 0 && $data.MagnitudeRatingName() == 'High'), " +
																						"'bg-danger': ($data.RemovedInd() == 0 && $data.MagnitudeRatingName() == 'Very High')	}");
																				}
																				var ProtectedAreaThreatsItemRatingCol = ProtectedAreaThreatsItemsTableFirstRow.AddColumn("Rating");
																				{
																					ProtectedAreaThreatsItemRatingCol.Attributes.Add("style", "width:100px;text-align: center;");
																					var ProtectedAreaThreatsItemRatingInd = ProtectedAreaThreatsItemRatingCol.Helpers.DivC("b-r-xl");
																					ProtectedAreaThreatsItemRatingInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-muted': $data.RemovedInd() == 1}");
																					ProtectedAreaThreatsItemRatingInd.Attributes.Add("style", "padding:5px;");
																					var ProtectedAreaThreatsItemRatingText = ProtectedAreaThreatsItemRatingInd.Helpers.Span(c => c.RatingRatingName);
																					ProtectedAreaThreatsItemRatingInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{" +
																						"'bg-info': ($data.RemovedInd() == 0 && $data.RatingRatingName() == 'Low'), " +
																						"'bg-primary': ($data.RemovedInd() == 0 && $data.RatingRatingName() == 'Medium'), " +
																						"'bg-warning': ($data.RemovedInd() == 0 && $data.RatingRatingName() == 'High'), " +
																						"'bg-danger': ($data.RemovedInd() == 0 && $data.RatingRatingName() == 'Very High')	}");
																				}
																				var ProtectedAreaThreatsItemDescription = ProtectedAreaThreatsItemsTableFirstRow.AddReadOnlyColumn(c => c.ThreatComments);
																				{
																					ProtectedAreaThreatsItemDescription.HeaderText = "Comments";
																				}

																				var InactiveCol = ProtectedAreaThreatsItemsTableFirstRow.AddColumn("Inactive");
																				{
																					InactiveCol.Helpers.HTML("<div class='checkbox'><label style='font-size: 2em'>");
																					var InactiveColInd = InactiveCol.Helpers.EditorFor(c => c.RemovedInd);

																					InactiveColInd.Attributes.Add("id", "RemovedCheckbox");
																					InactiveCol.Helpers.HTML("<span class='crSmall'><i class='cr-icon fa fa-check'></i></span></label></div>");
																					InactiveColInd.AddBinding(Singular.Web.KnockoutBindingString.valueUpdate, "UpdateThreatAnswer($data)");
																				}

																				var EditCol = ProtectedAreaThreatsItemsTableFirstRow.AddColumn("Action");
																				{
																					EditCol.Attributes.Add("style", "width:175px;");
																					var EditBtn = EditCol.Helpers.Button("Edit", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																					{
																						EditBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewThreatAnswer($data)");
																						EditBtn.AddClass("btn btn-outline btn-info btn");
																						EditBtn.Attributes.Add("data-toggle", "modal");
																						EditBtn.Attributes.Add("data-target", "#modalThreatEdit");
																						EditBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageEditThreatAssessmentBtnInd == true);
																					}

																					var DeleteBtn = EditCol.Helpers.Button("Delete", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																					{
																						DeleteBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "RemoveThreatAnswer($data)");
																						DeleteBtn.AddClass("btn btn-outline btn-danger btn");
																						DeleteBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageRemoveThreatAssessmentBtnInd == true);
																					}

																				}
																			}
																		}
																	}
																}
															}
															var ThreatModalDiv = MainHDiv.Helpers.DivC("container");
															{
																ThreatModalDiv.AddClass("modal inmodal");
																ThreatModalDiv.Attributes.Add("id", "modalThreatEdit");
																ThreatModalDiv.Attributes.Add("role", "dialog");
																ThreatModalDiv.Attributes.Add("aria-hidden", "true");
																ThreatModalDiv.Attributes.Add("tabindex", "-1");
																ThreatModalDiv.Attributes.Add("style", "display: none;");
																var ThreatModalDivDialog = ThreatModalDiv.Helpers.DivC("modalThreatEdit");
																{
																	ThreatModalDivDialog.AddClass("modal-dialog");
																	var ThreatModalDivDialogContent = ThreatModalDiv.Helpers.DivC("modalThreatEdit");
																	{
																		ThreatModalDivDialogContent.AddClass("modal-content animated fadeIn");
																		var ThreatModalContentHeading = ThreatModalDivDialogContent.Helpers.With<METTLib.Threats.ThreatAnswer>(c => c.EditingThreatAnswer);
																		{
																			var ThreatModalContentHeader = ThreatModalContentHeading.Helpers.DivC("modal-header ");
																			{
																				var ThreatHeaderHeading = ThreatModalContentHeader.Helpers.HTML("<h2 class='modal-title headerColor' data-bind=\"text: $data.ThreatCategoryName()\" /></h2>");
																				var ThreatHeaderSmall = ThreatModalContentHeader.Helpers.HTML("<span style='font-size:16px;' data-bind=\"text: $data.ThreatSubCategoryName()\" /></span>");
																			}
																			var ThreatModalContentBody = ThreatModalDivDialogContent.Helpers.DivC("modal-body m");
																			{


																				var ThreatModalContentBodyDiv = ThreatModalContentBody.Helpers.DivC("row margin0");
																				{
																					var ThreatModalContent = ThreatModalContentBodyDiv.Helpers.With<METTLib.Threats.ThreatAnswer>(c => c.EditingThreatAnswer);
																					{
																						var ThreatAnswerCategory = ThreatModalContent.Helpers.DivC("col-md-12");
																						{
																							ThreatAnswerCategory.Helpers.BootstrapEditorRowFor(c => c.ThreatCategoryID);
																						}
																						var ThreatAnswerSubCategory = ThreatModalContent.Helpers.DivC("col-md-12");
																						{
																							ThreatAnswerSubCategory.Helpers.BootstrapEditorRowFor(c => c.ThreatSubCategoryID);
																						}
																						var ThreatAnswerScope = ThreatModalContent.Helpers.DivC("col-md-4");
																						{
																							var html = ThreatAnswerScope.Helpers.HTML("<span class='text-center' href='#' title='• Very High: very widespread or pervasive in its scope, and affects the conservation values across all or most(71 - 100 %) of the protected area (site) or the range of the specific value.&#10;• High: widespread in its scope and affects conservation values across much(31 - 70 %) of the protected area (site).&#10;• Medium: localized in its scope, and affects conservation values over some(11 - 30 %) of the protected area (site) or the range of the specific value. &#10;• Low: very localized in its scope, and affects conservation values over a limited portion(1 - 10 %) of the protected area (site) or the range of the specific value.'><p class='fa fa-info-circle infoIconBlue'></p></span>");
																							ThreatAnswerScope.Helpers.BootstrapEditorRowFor(c => c.ScopeRatingID);
																						}
																						var ThreatAnswerSeverity = ThreatModalContent.Helpers.DivC("col-md-4");
																						{
																							var html = ThreatAnswerSeverity.Helpers.HTML("<span class='text-center' href='#' title='•	Very High: The threat is likely to destroy or eliminate the conservation values 71 - 100 % of the protected area (site) or the range of a specific value.&#10;•	High: The threat is likely to seriously degrade conservation values over 31 - 70 % of the protected area (site) or the range of a specific value.&#10;•	Medium: The threat is likely to moderately degrade the conservation values over 11 - 30 % of the protected area (site) or the range of a specific value.&#10;•	Low: The threat is likely to only slightly impair the conservation values over 1 - 10 % of the protected area (site) or the range of a specific value.'><p class='fa fa-info-circle infoIconBlue'></p></span>");
																							ThreatAnswerSeverity.Helpers.BootstrapEditorRowFor(c => c.SeverityRatingID);
																						}
																						var ThreatAnswerIrreversibility = ThreatModalContent.Helpers.DivC("col-md-4");
																						{
																							var html = ThreatAnswerIrreversibility.Helpers.HTML("<span class='text-center' href='#' title='•	Very High: The effects of the threat cannot be reversed and it is very unlikely the values could be restored, and/or it would take more than 100 years to achieve this (e.g., wetlands converted to a shopping centre).&#10;•	High: technically reversible, but not practically affordable (e.g., wetland converted to agriculture) and it would take 21-100 years to achieve.&#10;•	Medium: reversible with a reasonable commitment of resources (e.g., ditching & draining of wetland) and achievable in 6-20 years.&#10;•	Low: easily reversible at relatively low cost (e.g., off-road vehicles trespassing in wetland) within 0-5 years.'><p class='fa fa-info-circle infoIconBlue'></p></span>");
																							ThreatAnswerIrreversibility.Helpers.BootstrapEditorRowFor(c => c.IrreversibilityRatingID);
																						}

																						// Scope + Severity = Threat Magnitude
																						var ThreatAnswerMagnitude = ThreatModalContent.Helpers.DivC("col-md-6");
																						{
																							var ThreatWellMagnitude = ThreatAnswerMagnitude.Helpers.DivC("well");
																							{
																								ThreatWellMagnitude.Attributes.Add("id", "ThreatWellMagnitude");
																								ThreatWellMagnitude.Helpers.BootstrapEditorRowFor(c => c.MagnitudeRatingID);
																							}
																							ThreatWellMagnitude.Helpers.HTML("<i>Severity + Scope = Magnitude Rating</i>");

																						}
																						// Magnitude + Irreversibility = Threat Rating
																						var ThreatAnswerRating = ThreatModalContent.Helpers.DivC("col-md-6");
																						{
																							var ThreatWellRating = ThreatAnswerRating.Helpers.DivC("well");
																							{
																								ThreatWellRating.Attributes.Add("id", "ThreatWellRating");
																								ThreatWellRating.Helpers.BootstrapEditorRowFor(c => c.RatingRatingID);
																							}
																							ThreatWellRating.Helpers.HTML("<i>Magnitude + Irreversibility = Overall Rating</i>");
																						}
																						var ThreatAnswerComments = ThreatModalContent.Helpers.DivC("col-md-11");
																						{
																							ThreatAnswerComments.Helpers.BootstrapEditorRowFor(c => c.ThreatComments);
																						}

																						var ThreatInactive = ThreatModalContent.Helpers.DivC("col-md-1");
																						{
																							var InactiveCheck = ThreatInactive.Helpers.DivC("");
																							{
																								InactiveCheck.Helpers.HTML("<b>Inactive</b>");
																								InactiveCheck.Helpers.HTML("<div class='checkbox'><label style='font-size: 2em'>");
																								var InactiveCheckInd = InactiveCheck.Helpers.EditorFor(c => c.RemovedInd);
																								InactiveCheckInd.Attributes.Add("id", "AcceptedCheckbox");
																								InactiveCheck.Helpers.HTML("<span class='cr'><i class='cr-icon fa fa-check'></i></span></label></div>");
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																	var ThreatModalContentFooter = ThreatModalDivDialogContent.Helpers.DivC("modal-footer");
																	{
																		var ThreatCloseBtn = ThreatModalContentFooter.Helpers.Button("Close", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																		{
																			ThreatCloseBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "RefreshThreatList()");
																			ThreatCloseBtn.AddClass("btn btn-white");
																			ThreatCloseBtn.Attributes.Add("data-dismiss", "modal");
																		}
																		var ThreatSaveBtn = ThreatModalContentFooter.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																		{
																			ThreatSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveThreatAnswer($data)");
																			ThreatSaveBtn.AddClass("btn btn-primary");
																		}
																	}
																}
															}
														}
													}
													#endregion

													#region ThreatsAvailable
													var AvailableThreatsDiv = ThreatsTab.Helpers.DivC("ibox float-e-margins paddingBottom");
													{
														AvailableThreatsDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewThreatsTabInd == true);
														var AvailableThreatsTitleDiv = AvailableThreatsDiv.Helpers.DivC("ibox-title");
														{
															AvailableThreatsTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
															AvailableThreatsTitleDiv.Helpers.HTML().Heading5("Available Threats");
														}
														var AvailableThreatsToolsDiv = AvailableThreatsTitleDiv.Helpers.DivC("ibox-tools");
														{
															var aAvailableThreatsToolsTag = AvailableThreatsToolsDiv.Helpers.HTMLTag("a");
															aAvailableThreatsToolsTag.AddClass("collapse-link");
															{
																var iAvailableThreatsToolsTag = aAvailableThreatsToolsTag.Helpers.HTMLTag("i");
																iAvailableThreatsToolsTag.AddClass("fa fa-chevron-up");
															}
														}
														var ThreatsDivContentDiv = AvailableThreatsDiv.Helpers.DivC("ibox-content");
														{
															var ThreatsDivPagedDiv = ThreatsDivContentDiv.Helpers.Div();
															{
																var ThreatsDescription = ThreatsDivPagedDiv.Helpers.HTMLTag("p");
																ThreatsDescription.Helpers.HTML("Threats are human activities or processes that have impacted, are impacting, or may impact the status of the protected area being assessed (e.g., water pollution upstream entering and impacting on the protected area). Threats can be past (historical, unlikely to return or historical likely to return), ongoing, and/or likely to occur in the future. Natural phenomena are also regarded as threats in some situations. Threats can be in or outside of the protected area. The Zone of Influence should be used as a guideline for the area within which threats are identified and evaluated. However, this is an iterative process and thus the threats assessment should also guide the delineation of the Zone of Influence.");

																var Threats = ThreatsDivPagedDiv.Helpers.BootstrapTableFor<METTLib.ThreatCategories.ThreatCategory>((c) => c.ThreatsList, false, false, "", true);

																var ThreatsTableFirstRow = Threats.FirstRow;
																{
																	var ThreatName = ThreatsTableFirstRow.AddReadOnlyColumn(c => c.ThreatCategoryName);
																	{
																		ThreatName.HeaderText = "Threat Category";
																	}
																}
																var ThreatsItems = Threats.AddChildTable<METTLib.ThreatSubCategories.ThreatSubCategory>((c) => c.ThreatSubCategoryList, false, false, "");
																{
																	var ThreatsItemsTableFirstRow = ThreatsItems.FirstRow;
																	{
																		var ThreatsItemName = ThreatsItemsTableFirstRow.AddReadOnlyColumn(c => c.ThreatSubCategoryName);
																		{
																			ThreatsItemName.HeaderText = "Threat";
																		}
																		var ThreatsItemDescription = ThreatsItemsTableFirstRow.AddReadOnlyColumn(c => c.ThreatSubCategoryDescription);
																		{
																			ThreatsItemDescription.HeaderText = "Description";
																		}
																		var EditCol = ThreatsItemsTableFirstRow.AddColumn("Action");
																		{
																			var AddBtn = EditCol.Helpers.Button("Add", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																			{
																				AddBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "AddThreat($data)");
																				AddBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageNewThreatAssessmentBtnInd == true);
																				AddBtn.AddClass("btn btn-outline btn-info btn");
																			}
																		}
																	}
																}
															}
														}
														#endregion

														var ThreatMatrixDiv = ThreatsTab.Helpers.DivC("ibox float-e-margins paddingBottom");
														{
															ThreatMatrixDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewThreatsTabInd == true);
															var ThreatMatrixTitleDiv = ThreatMatrixDiv.Helpers.DivC("ibox-title");
															{
																ThreatMatrixTitleDiv.Helpers.HTML("<i class='fa fa-cog fa-lg fa-fw pull-left'></i>");
																ThreatMatrixTitleDiv.Helpers.HTML().Heading5("Threat Matrix");
															}
															var ThreatMatrixToolsDiv = ThreatMatrixTitleDiv.Helpers.DivC("ibox-tools");
															{
																var aThreatMatrixToolsTag = ThreatMatrixToolsDiv.Helpers.HTMLTag("a");
																aThreatMatrixToolsTag.AddClass("collapse-link");
																{
																	var iAvailableThreatsToolsTag = aThreatMatrixToolsTag.Helpers.HTMLTag("i");
																	iAvailableThreatsToolsTag.AddClass("fa fa-chevron-up");
																}
															}
															var ThreatMatrixDivContentDiv = ThreatMatrixDiv.Helpers.DivC("ibox-content");
															{
																var ThreatsDivPagedDiv = ThreatMatrixDivContentDiv.Helpers.Div();
																var MatrixExplanatoryRow = ThreatsDivPagedDiv.Helpers.DivC("row");
																{
																	var MatrixExplanatoryCol = MatrixExplanatoryRow.Helpers.DivC("col-md-12");
																	{
																		MatrixExplanatoryCol.Helpers.HTML("<p>The Threat Matrix tables below are for informational purposes only. These illustrate how the Magnitude and Overall ratings are calculated based on the pre-saved Severity, Scope and Magnitude ratings captured against each threat category.</p>");
																	}

																}

																var ThreatMatrixContentDivRow = ThreatsDivPagedDiv.Helpers.DivC("row");
																{
																	#region ScopeMatrix
																	var ScopeMatrix = ThreatMatrixContentDivRow.Helpers.DivC("col-md-6");
																	{
																		ScopeMatrix.Helpers.HTML().Heading3("Magnitude ");
																		ScopeMatrix.Helpers.HTML("<p><i>Severity + Scope = Magnitude Rating</i></p>");
																		ScopeMatrix.Helpers.HTML("<img src='../images/ss.jpg' alt='Magnitude Rating' width='100%'>");
																	}
																	#endregion

																	#region MagnitudeMatrix
																	var MagnitudeMatrix = ThreatMatrixContentDivRow.Helpers.DivC("col-md-6");
																	{
																		MagnitudeMatrix.Helpers.HTML().Heading3("Overall Rating");
																		MagnitudeMatrix.Helpers.HTML("<p><i>Magnitude + Irreversibility = Overall Rating</i></p>");
																		MagnitudeMatrix.Helpers.HTML("<img src='../images/mi.jpg' alt='Overall Rating' width='100%'>");
																	}
																	#endregion
																}

																var OverviewNoAccessRow = ThreatsTab.Helpers.DivC("row margin0");
																{
																	OverviewNoAccessRow.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewThreatsTabInd == false);
																	var OverviewNoAccess = OverviewNoAccessRow.Helpers.DivC("col-md-12  p-n-lr");
																	{
																		var OverviewNoAccessColContainerBox = OverviewNoAccess.Helpers.DivC("ibox float-e-margins paddingBottom");
																		{
																			var OverviewNoAccessContentBox = OverviewNoAccessColContainerBox.Helpers.DivC("ibox-content");
																			{
																				var OverviewNoAccessContentBoxRow = OverviewNoAccessContentBox.Helpers.DivC("row");
																				{
																					var OverviewNoAccessContentBoxRowCol = OverviewNoAccessContentBoxRow.Helpers.DivC("col-md-12 ");
																					{
																						OverviewNoAccessContentBoxRowCol.Helpers.HTML("<h2 style='color:#1ab394;'>Assessment Threats</h2>");
																						OverviewNoAccessContentBoxRowCol.Helpers.HTML("<span>You do not have access. Please confirm your security role/group settings.</span>");
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
													#endregion

													// Assessment Tab
													#region AssessmentTab
													var AssessmentTab = EntityTab.AddTab("Assessment");
													{
														var AssessmentRow = AssessmentTab.Helpers.DivC("row");
														{
															var AssessmentNoAccess = AssessmentRow.Helpers.DivC("col-md-12");
															{
																var OverviewQuestionnaireAnswerSet = AssessmentNoAccess.Helpers.With<METTLib.Questionnaire.QuestionnaireAnswerSet>(c => c.FirstQuestionnaireAnswerSet);
																OverviewQuestionnaireAnswerSet.Helpers.HTML("<h2 class='headingFontColorWeight' data-bind=\"text: $data.METTReportingName()\"></h2>");
															}
														}
														#region AssessmentBreadcrumb
														var WizardRow = AssessmentTab.Helpers.DivC("row");
														{
															WizardRow.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewAssessmentTabInd == true);
															var WizardDiv = WizardRow.Helpers.DivC("wizard");
															{
																var WizzardInner = WizardDiv.Helpers.DivC("wizard-inner");
																{
																	WizzardInner.Helpers.DivC("connecting-line");
																	var WizzardUL = WizzardInner.Helpers.HTMLTag("ul");
																	{
																		WizzardUL.AddClass("nav nav-tabs");
																		WizzardUL.Attributes.Add("role", "tablist");
																		WizzardUL.Attributes.Add("style", "border-bottom:none");
																		WizzardUL.Attributes.Add("id", "ul-tab-list");
																		var QuestionaireGroups = WizzardUL.Helpers.ForEach<METTLib.Questionnaire.ROQuestionnaireGroup>(c => c.ROQuestionnaireGroupList);
																		{
																			var WizardListItem = QuestionaireGroups.Helpers.HTMLTag("li");
																			{
																				WizardListItem.Attributes.Add("role", "presentation");
																				WizardListItem.Attributes.Add("style", "background:none");
																				var WizardListItemHeader = WizardListItem.Helpers.HTMLTag("h4");
																				{
																					WizardListItemHeader.AddClass("text-center");
																					var CurrentGroup = WizardListItemHeader.Helpers.ReadOnlyFor(c => c.QuestionnaireGroup);
																				}
																				var WizardIcon = WizardListItem.Helpers.HTMLTag("a");
																				{
																					WizardIcon.Attributes.Add("role", "tab");
																					WizardIcon.AddBinding(Singular.Web.KnockoutBindingString.click, "ShowQuestionnaireGroupData($data)");
																					var WizardIconSpan = WizardIcon.Helpers.SpanC("round-tab showSingle");
																					{
																						WizardIconSpan.AddBinding(Singular.Web.KnockoutBindingString.id, c => c.QuestionnaireGroupID); // c => "tab-" + c.QuestionnaireGroupID);
																						var WizardIconImage = WizardIconSpan.Helpers.HTMLTag("i");
																						{
																							WizardIconImage.AddBinding(Singular.Web.KnockoutBindingString.css, c => c.Icon);
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
														#endregion

														var AssessmentNoAccessRow = AssessmentTab.Helpers.DivC("row  margin0");
														{
															var AssessmentNoAccess = AssessmentNoAccessRow.Helpers.DivC("col-md-12  p-n-lr");
															{
																AssessmentNoAccess.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewAssessmentTabInd == false);
																var OverviewNoAccessColContainerBox = AssessmentNoAccess.Helpers.DivC("ibox float-e-margins paddingBottom");
																{
																	var OverviewNoAccessContentBox = OverviewNoAccessColContainerBox.Helpers.DivC("ibox-content");
																	{
																		var OverviewNoAccessContentBoxRow = OverviewNoAccessContentBox.Helpers.DivC("row");
																		{
																			var OverviewNoAccessContentBoxRowCol = OverviewNoAccessContentBoxRow.Helpers.DivC("col-md-12 ");
																			{
																				OverviewNoAccessContentBoxRowCol.Helpers.HTML("<h2 style='color:#1ab394;'>Assessment</h2>");
																				OverviewNoAccessContentBoxRowCol.Helpers.HTML("<span>You do not have access. Please confirm your security role/group settings.</span>");
																			}
																		}
																	}
																}
															}
														}

														#region AssessmentSurvey
														var AssessmentDiv = AssessmentTab.Helpers.DivC("row margin0");

														AssessmentDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewAssessmentTabInd == true);

														var AssessmentTabContainer = AssessmentDiv.Helpers.DivC("tabs-container");
														{
															var TabContentLeft = AssessmentTabContainer.Helpers.DivC("tabs-left userTabs");
															{
																var TabList = TabContentLeft.Helpers.HTMLTag("ul");
																{
																	TabList.AddClass("nav nav-tabs");
																	TabList.Attributes.Add("id", "navigation");
																	var TabItems = TabList.Helpers.ForEach<METTLib.QuestionnaireSurvey.QuestionnaireQuestion>(c => c.QuestionnaireQuestionList);
																	{
																		var TabItem = TabItems.Helpers.HTMLTag("li");
																		{
																			TabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabs" + c.IndicatorDetailID);
																			TabItem.AddBinding(Singular.Web.KnockoutBindingString.css, "{'active ': $data.QuestionnaireQuestionID() == ViewModel.FirstQuestionID()}");
																			var TabItemLink = TabItem.Helpers.HTMLTag("a");
																			{
																				TabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => c.IndicatorDetailName);
																				TabItemLink.AddClass("ellipsis");
																				TabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-" + c.QuestionnaireQuestionID);
																				TabItemLink.Attributes.Add("data-toggle", "tab");
																			}
																		}
																	}
																}
																var TabItemsContents = TabContentLeft.Helpers.DivC("tab-content");
																{
																	TabItemsContents.Attributes.Add("id", "tab-content");
																}
																var TabItemContent = TabItemsContents.Helpers.ForEach<METTLib.QuestionnaireSurvey.QuestionnaireQuestion>(c => c.QuestionnaireQuestionList);
																{
																	var TabPane = TabItemContent.Helpers.DivC("tab-pane");

																	TabPane.AddBinding(Singular.Web.KnockoutBindingString.id, c => "tab-" + c.QuestionnaireQuestionID);
																	TabPane.AddBinding(Singular.Web.KnockoutBindingString.css, "{'active': $data.QuestionnaireQuestionID() == ViewModel.FirstQuestionID()}");
																	var TabPaneBody = TabPane.Helpers.DivC("panel-body panelRaiseShadow");
																	{
																		var TabPaneTitle = TabPaneBody.Helpers.DivC("ibox-title ibox-titleContainer");
																		{
																			TabPaneTitle.Helpers.HTML("<i class='fa fa-folder fa-lg fa-fw pull-left'></i>");
																			TabPaneTitle.Helpers.HTML("<h5 data-bind=\"text: $data.IndicatorDetailName()\" /></h5>");
																		}
																		var TabIboxContent = TabPaneBody.Helpers.DivC("col-md-12");
																		{
																			var QuestionnaireQuestionHeading = TabPaneBody.Helpers.HTMLTag("h3");
																			{
																				QuestionnaireQuestionHeading.AddClass("headingFontColorWeight");
																				var QuestionHeading = QuestionnaireQuestionHeading.Helpers.ReadOnlyFor(c => "Question: " + c.Question);

																				var TooltipICON = QuestionnaireQuestionHeading.Helpers.HTMLTag("div");
																				{
																					TooltipICON.AddClass("tTip fa fa-info-circle infoIconBlue");
																					TooltipICON.AddBinding(Singular.Web.KnockoutBindingString.title, c => c.QuestionTooltip);
																				}
																			}
																		}
																		#region QuestionnaireQuestionAnswers
																		var TabQuestions = TabPaneBody.Helpers.DivC("");
																		{
																			var QuestionnaireQuestionAnswerGroup = TabQuestions.Helpers.ForEach<METTLib.QuestionnaireSurvey.QuestionnaireQuestionType>(c => c.QuestionnaireQuestionTypeList);
																			{
																				QuestionnaireQuestionAnswerGroup.Helpers.HTML("<div class='col-md-12 pad-top-5'>");
																				var QRadio = QuestionnaireQuestionAnswerGroup.Helpers.DivC("col-md-2");
																				{
																					QRadio.Attributes.Add("style", "padding: 0px;");
																					var QuestionAnswerBtnAssessor = QRadio.Helpers.Button("", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																					QuestionAnswerBtnAssessor.AddClass("fa ");
																					QuestionAnswerBtnAssessor.AddBinding(Singular.Web.KnockoutBindingString.click, "UpdateQACheckAssessor($data)");
																					QuestionAnswerBtnAssessor.AddBinding(Singular.Web.KnockoutBindingString.css, "{'fa-dot-circle-o m-r-sm btn-bg-color-green':$data.IsSelectedInd(), 'fa-circle-o m-r-sm': !$data.IsSelectedInd()}"); // , 'fa fa-circle-o': obj.IsSelectedInd() == false} 
																																																																																																																						 //QuestionAnswerBtnAssessor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.AcceptedInd == true);
																					QuestionAnswerBtnAssessor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAssessorControlsDisabledInd);
																					var QuestionAnswerBtnReviewer = QRadio.Helpers.Button("", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																					QuestionAnswerBtnReviewer.AddClass("fa ");
																					QuestionAnswerBtnReviewer.AddBinding(Singular.Web.KnockoutBindingString.click, "UpdateQACheckReviewer($data)");
																					QuestionAnswerBtnReviewer.AddBinding(Singular.Web.KnockoutBindingString.css, "{'fa-dot-circle-o m-r-sm btn-bg-color-teal':$data.IsSelectedIndAuditor(), 'fa-circle-o m-r-sm': !$data.IsSelectedIndAuditor()}"); // , 'fa fa-circle-o': obj.IsSelectedInd() == false} 
																					QuestionAnswerBtnReviewer.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.AuditedInd == true);
																					QuestionAnswerBtnReviewer.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAuditorControlsDisabledInd);
																				}
																				var QText = QuestionnaireQuestionAnswerGroup.Helpers.DivC("col-md-10");
																				{
																					QText.Attributes.Add("style", "padding: 0px;");
																					QText.Helpers.ReadOnlyFor(c => c.AnswerOption);
																					var AnsweOptionTooltipIcon = QText.Helpers.HTMLTag("div");
																					{
																						//AnsweOptionTooltipIcon.AddClass("tTip fa fa-info-circle infoIconBlue");
																						AnsweOptionTooltipIcon.AddBinding(Singular.Web.KnockoutBindingString.title, c => c.QuestionnaireQuestionAnswerTooltip);
																						AnsweOptionTooltipIcon.AddBinding(Singular.Web.KnockoutBindingString.css, "{'tTip fa fa-info-circle infoIconBlue': $data.QuestionnaireQuestionAnswerTooltip() != '' }");
																					}
																				}
																				QuestionnaireQuestionAnswerGroup.Helpers.HTML("</div>");
																			}

																			var AssessorRow = TabPaneBody.Helpers.DivC("row margin0");
																			{
																				var TabComments = AssessorRow.Helpers.DivC("col-md-4 pad-top-15");
																				{
																					var TabComment = TabComments.Helpers.BootstrapEditorRowFor(c => c.Comments);
																					TabComments.Helpers.HTML("<div class='txtCommentsInfo' style='display:block; width:100%; min-height: 35px;'>Justify your selection and/or comment on the current situation. Also make a note of assumptions made.</div>");
																					TabComment.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.AcceptedInd == true);
																					TabComment.Editor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAssessorControlsDisabledInd);

																					var AuditorCommentDiv = TabComments.Helpers.DivC(" pad-top-15");
																					{
																						var TabAuditorComment = AuditorCommentDiv.Helpers.BootstrapEditorRowFor(c => c.CommentsAuditor);
																						AuditorCommentDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.AuditedInd == true);
																						TabAuditorComment.Editor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAuditorControlsDisabledInd);
																					}
																				}

																				var TabNextSteps = AssessorRow.Helpers.DivC("col-md-4 pad-top-15");
																				{
																					var NextStepsTextBox = TabNextSteps.Helpers.BootstrapEditorRowFor(c => c.NextSteps);
																					TabNextSteps.Helpers.HTML("<div class='txtCommentsInfo' style='display:block; width:100%; min-height: 35px;'>Identify actions needed to improve your management effectiveness rating.</div>");
																					NextStepsTextBox.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.AcceptedInd == true);
																					NextStepsTextBox.Editor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAssessorControlsDisabledInd);

																					var AuditorNextStepDiv = TabNextSteps.Helpers.DivC(" pad-top-15");
																					{
																						var TabAuditorNextSteps = AuditorNextStepDiv.Helpers.BootstrapEditorRowFor(c => c.NextStepsAuditor);
																						AuditorNextStepDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.AuditedInd == true);
																						TabAuditorNextSteps.Editor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAuditorControlsDisabledInd);
																					}
																				}

																				var TabEvidence = AssessorRow.Helpers.DivC("col-md-4 pad-top-15");
																				{
																					var TabEvidenceTextBox = TabEvidence.Helpers.BootstrapEditorRowFor(c => c.Evidence);
																					TabEvidence.Helpers.HTML("<span class='txtCommentsInfo' style='width:100%; min-height: 35px;'>List the evidence that you have used to verify your current rating.</span>");
																					TabEvidenceTextBox.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.AcceptedInd == true);
																					TabEvidenceTextBox.Editor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAssessorControlsDisabledInd);

																					var TooltipICON = TabEvidenceTextBox.Helpers.HTMLTag("span");
																					{
																						//TooltipICON.AddClass("tTip fa fa-info-circle infoIconBlue");
																						TooltipICON.AddBinding(Singular.Web.KnockoutBindingString.title, c => c.EvidenceTooltip);
																						//TooltipICON.AddBinding(Singular.Web.KnockoutBindingString.css,"{'tTip fa fa-info-circle infoIconBlue': ($data.EvidenceTooltip() <> '' || $data.EvidenceTooltip() <> ' ')}");
																						TooltipICON.AddBinding(Singular.Web.KnockoutBindingString.css,"{'tTip fa fa-info-circle infoIconBlue': ($data.EvidenceTooltip() !== '')}");
																					}

																					var AuditorEvidenceDiv = TabEvidence.Helpers.DivC(" pad-top-15");
																					{
																						var TabEvidenceAuditorTextBox = AuditorEvidenceDiv.Helpers.BootstrapEditorRowFor(c => c.EvidenceAuditor);
																						AuditorEvidenceDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.AuditedInd == true);
																						TabEvidenceAuditorTextBox.Editor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAuditorControlsDisabledInd);
																					}
																				}

																				var TabOptions = TabComments.Helpers.DivC("row pad-top-15");
																				{
																					var TabOptionsCol = TabOptions.Helpers.DivC("col-md-12 pad-left-15 pad-top-15");
																					{
																						var BtnPrev = TabOptionsCol.Helpers.Button("Previous", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																						{
																							BtnPrev.AddClass("btn-primary btn btn btn-outline");
																							BtnPrev.AddBinding(Singular.Web.KnockoutBindingString.click, "QuestionStepBack($data)");
																						}
																						var BtnSave_Assessor = TabOptionsCol.Helpers.Button("Save and Next", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																						{
																							BtnSave_Assessor.AddClass("btn-primary btn btn btn-primary pad-left-15");
																							BtnSave_Assessor.AddBinding(Singular.Web.KnockoutBindingString.click, "QuestionStepNext($data)");
																							BtnSave_Assessor.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageSaveAndNextAssessmentAssessmentBtnInd == true && ViewModel.PageAssessorControlsDisabledInd == false);
																						}
																						var BtnSave_Auditor = TabOptionsCol.Helpers.Button("Save and Next", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																						{
																							BtnSave_Auditor.AddClass("btn-primary btn btn btn-primary pad-left-15");
																							BtnSave_Auditor.AddBinding(Singular.Web.KnockoutBindingString.click, "QuestionStepNext($data)");
																							BtnSave_Auditor.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageSaveAndNextAssessmentAuditBtnInd == true);
																						}
																					}
																				}
																			}

																			var AuditorRow = TabPaneBody.Helpers.DivC("row margin0");
																			{

																			}
																		}
																		#endregion
																	}
																	var SpacerRow = AssessmentTab.Helpers.DivC("row pad-top-20");
																	{
																	}
																}
															}
														}
														#endregion
													}
													#endregion

													// Summary Tab
													#region SummaryTab
													var SummaryTab = EntityTab.AddTab("Summary");
													{
														var AssessmentNoAccessRow = SummaryTab.Helpers.DivC("row  margin0");
														{
															var AssessmentNoAccess = AssessmentNoAccessRow.Helpers.DivC("col-md-12  p-n-lr");
															{
																AssessmentNoAccess.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewSummaryTabInd == false);
																var OverviewNoAccessColContainerBox = AssessmentNoAccess.Helpers.DivC("ibox float-e-margins paddingBottom");
																{
																	var OverviewNoAccessContentBox = OverviewNoAccessColContainerBox.Helpers.DivC("ibox-content");
																	{
																		var OverviewNoAccessContentBoxRow = OverviewNoAccessContentBox.Helpers.DivC("row");
																		{
																			var OverviewNoAccessContentBoxRowCol = OverviewNoAccessContentBoxRow.Helpers.DivC("col-md-12 ");
																			{
																				OverviewNoAccessContentBoxRowCol.Helpers.HTML("<h2 style='color:#1ab394;'>Assessment Summary</h2>");
																				OverviewNoAccessContentBoxRowCol.Helpers.HTML("<span>You do not have access. Please confirm your security role/group settings.</span>");
																			}
																		}
																	}
																}
															}
														}

														#region SummaryOverview
														var SummaryOverview = SummaryTab.Helpers.DivC("ibox float-e-margins paddingBottom");
														{
															SummaryOverview.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewSummaryTabInd == true);
															var SummaryOverviewTitleDiv = SummaryOverview.Helpers.DivC("ibox-title");
															{
																SummaryOverviewTitleDiv.Helpers.HTML("<i class='fa fa-coffee fa-lg fa-fw pull-left'></i>");
																SummaryOverviewTitleDiv.Helpers.HTML().Heading5("Summary");
															}
															var SummaryOverviewToolsDiv = SummaryOverviewTitleDiv.Helpers.DivC("ibox-tools");
															{
																var aSummaryOverviewToolsTag = SummaryOverviewToolsDiv.Helpers.HTMLTag("a");
																aSummaryOverviewToolsTag.AddClass("collapse-link");
																{
																	var iSummaryOverviewToolsTag = aSummaryOverviewToolsTag.Helpers.HTMLTag("i");
																	iSummaryOverviewToolsTag.AddClass("fa fa-chevron-up");
																}
															}
															var SummaryOverviewContentDiv = SummaryOverview.Helpers.DivC("ibox-content");
															{
																var AssessmentSummaryDivPagedDiv = SummaryOverviewContentDiv.Helpers.Div();
																{
																	var AssessmentSummary = AssessmentSummaryDivPagedDiv.Helpers.BootstrapTableFor<METTLib.RO.ROQuestionnaireGroupAnswerResult>((c) => c.ROQuestionnaireGroupAnswerResultList, false, false, "", true);

																	var AssessmentSummaryTableFirstRow = AssessmentSummary.FirstRow;
																	{
																		var AssessmentSummaryName = AssessmentSummaryTableFirstRow.AddReadOnlyColumn(c => c.QuestionnaireGroup);
																		{
																			AssessmentSummaryName.HeaderText = "Adaptive Management Categories";
																		}
																		var AssessmentSummaryAnswers = AssessmentSummaryTableFirstRow.AddColumn("Incomplete Indicators");
																		{
																			AssessmentSummaryAnswers.Attributes.Add("style", "width:50px;text-align: center;");
																			var ThreatRatingItemVeryHighInd = AssessmentSummaryAnswers.Helpers.DivC("badge ");
																			ThreatRatingItemVeryHighInd.Attributes.Add("style", "padding:5px;");
																			var ThreatRatingItemVeryHighText = ThreatRatingItemVeryHighInd.Helpers.Span(c => c.GroupTotalAnswers);
																			ThreatRatingItemVeryHighInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'badge-primary': $data.GroupTotalAnswers() == 0, 'badge-warning': $data.GroupTotalAnswers() >= 1}");
																		}
																	}
																	var AssessmentSummaryItems = AssessmentSummary.AddChildTable<METTLib.RO.ROQuestionnaireAnswerResult>((c) => c.ROQuestionnaireAnswerResultList, false, false, "");
																	{
																		AssessmentSummaryItems.ID = "summary";
																		var AssessmentSummaryItemsTableFirstRow = AssessmentSummaryItems.FirstRow;
																		{

																			var AssessmentSummaryItemName = AssessmentSummaryItemsTableFirstRow.AddColumn("Description");
																			{

																				var AssessmentSummaryItemNameText = AssessmentSummaryItemName.Helpers.HTMLTag("a");
																				AssessmentSummaryItemNameText.AddClass("btnHyperlink btn");
																				AssessmentSummaryItemNameText.Helpers.Span(c => c.IndicatorDetailName);
																				AssessmentSummaryItemNameText.AddBinding(Singular.Web.KnockoutBindingString.click, "ShowIndicator($data)");

																			}

																			var AssessmentSummaryComments = AssessmentSummaryItemsTableFirstRow.AddColumn("Comments");
																			{
																				var AssessmentSummaryCommentsText = AssessmentSummaryComments.Helpers.Span(c => c.Comments);
																				var AssessmentSummaryCommentsTextDiv = AssessmentSummaryComments.Helpers.DivC("");
																				AssessmentSummaryCommentsTextDiv.AddBinding(Singular.Web.KnockoutBindingString.css, "{'tdRedBackground': $data.IsCommentsValid() == 0}");
																			}

																			var AssessmentSummaryNextSteps = AssessmentSummaryItemsTableFirstRow.AddColumn("Next Steps");
																			{
																				var AssessmentSummaryNextStepsText = AssessmentSummaryNextSteps.Helpers.Span(c => c.NextSteps);
																				var AssessmentSummaryNextStepsTextDiv = AssessmentSummaryNextSteps.Helpers.DivC("");
																				AssessmentSummaryNextStepsTextDiv.AddBinding(Singular.Web.KnockoutBindingString.css, "{'tdRedBackground': $data.IsNextStepsValid() == 0}");
																			}

																			var AssessmentSummaryEvidence = AssessmentSummaryItemsTableFirstRow.AddColumn("Evidence");
																			{
																				var AssessmentSummaryEvidenceText = AssessmentSummaryEvidence.Helpers.Span(c => c.Evidence);
																				var AssessmentSummaryEvidenceTextDiv = AssessmentSummaryEvidence.Helpers.DivC("");
																				AssessmentSummaryEvidenceTextDiv.AddBinding(Singular.Web.KnockoutBindingString.css, "{'tdRedBackground': $data.IsEvidenceValid() == 0}");
																			}

																			var AssessmentSummaryItemDescription = AssessmentSummaryItemsTableFirstRow.AddReadOnlyColumn(c => c.MaxRating);
																			{
																				AssessmentSummaryItemDescription.Attributes.Add("style", "width:75px;");
																				AssessmentSummaryItemDescription.HeaderText = "Value";
																			}
																			var AssessmentSummaryItemDescription2 = AssessmentSummaryItemsTableFirstRow.AddColumn("");
																			{
																				AssessmentSummaryItemDescription2.Attributes.Add("style", "width:75px;");
																				AssessmentSummaryItemDescription2.HeaderText = "Rating";
																				var AuditedIndText = AssessmentSummaryItemDescription2.Helpers.Span(c => c.AnswerRating == -1 ? "N/A" : c.AnswerRating == -99 ? " " : c.AnswerRating.ToString("c0"));
																			}
																		}
																	}
																}
															}
														}
														#endregion

														#region SummarySignOff
														var SummarySignOff = SummaryTab.Helpers.DivC("ibox float-e-margins paddingBottom");
														{
															SummarySignOff.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewSummaryTabInd == true);
															var SummarySignOffTitleDiv = SummarySignOff.Helpers.DivC("ibox-title");
															{
																SummarySignOffTitleDiv.Helpers.HTML("<i class='fa fa-sign-out fa-lg fa-fw pull-left'></i>");
																SummarySignOffTitleDiv.Helpers.HTML().Heading5("Designations & Acceptance");
															}
															var SummarySignOffToolsDiv = SummarySignOffTitleDiv.Helpers.DivC("ibox-tools");
															{
																var aSummarySignOffToolsTag = SummarySignOffToolsDiv.Helpers.HTMLTag("a");
																aSummarySignOffToolsTag.AddClass("collapse-link");
																{
																	var iSummarySignOffToolsTag = aSummarySignOffToolsTag.Helpers.HTMLTag("i");
																	iSummarySignOffToolsTag.AddClass("fa fa-chevron-up");
																}
															}
															var SummarySignOffContentDiv = SummarySignOff.Helpers.DivC("ibox-content");
															{
																var SummarySignOffRowDiv = SummarySignOffContentDiv.Helpers.DivC("row margin0");
																{
																	var AssessmentSummaryDivPagedDiv3 = SummarySignOffContentDiv.Helpers.Div();
																	{
																		var SummarySignOffText = SummarySignOffRowDiv.Helpers.DivC("col-md-12 text-center pad-top-15");
																		{
																			SummarySignOffText.Helpers.HTML("<h3>Designations</h3>");
																			SummarySignOffText.Helpers.HTML("<p>Before submitting your assessment, please complete the listed designations below.</p>");
																		}
																	}
																}
																var SummaryUsersRowDiv = SummarySignOffContentDiv.Helpers.DivC("row margin0");
																{
																	var SummaryDiv = SummaryUsersRowDiv.Helpers.With<METTLib.Questionnaire.QuestionnaireAnswerSet>(c => c.FirstQuestionnaireAnswerSet);
																	{
																		var LeadAssessor = SummaryDiv.Helpers.DivC("col-md-3 text-center pad-top-15");
																		{
																			var LeadCircle = LeadAssessor.Helpers.HTML("<div class='circlecenter'><div class='circlecontainerusergreen circlecenter'><span class='fas fa-user-graduate fa-lg fa-fw' style='font-size:38px;'></span></div></div>");
																			LeadAssessor.Helpers.BootstrapEditorRowFor(c => c.LeadAssessor);
																			LeadAssessor.AddBinding(Singular.Web.KnockoutBindingString.disable, c => ViewModel.PageAssessorControlsDisabledInd != true);

																		}
																		var SiteManager = SummaryDiv.Helpers.DivC("col-md-3 text-center pad-top-15");
																		{
																			var SiteCircle = SiteManager.Helpers.HTML("<div class='circlecenter'><div class='circlecontaineruser circlecenter'><span class='fas fa-user-tie fa-lg fa-fw' style='font-size:38px;'></span></div></div>");
																			SiteManager.Helpers.BootstrapEditorRowFor(c => c.SiteManager);
																		}
																		var DistrictManager = SummaryDiv.Helpers.DivC("col-md-3 text-center pad-top-15");
																		{
																			var DistrictCircle = DistrictManager.Helpers.HTML("<div class='circlecenter'><div class='circlecontaineruser circlecenter'><span class='fas fa-user-shield fa-lg fa-fw' style='font-size:38px;'></span></div></div>");
																			DistrictManager.Helpers.BootstrapEditorRowFor(c => c.Ecologist);
																		}
																		var Reviewer = SummaryDiv.Helpers.DivC("col-md-3 text-center pad-top-15");
																		{
																			var ReviewerCircle = Reviewer.Helpers.HTML("<div class='circlecenter'><div class='circlecontaineruserteal circlecenter'><span class='fas fa-user-edit fa-lg fa-fw' style='font-size:38px;'></span></div></div>");
																			Reviewer.Helpers.BootstrapEditorRowFor(c => c.AuditorFullName);
																			Reviewer.AddBinding(Singular.Web.KnockoutBindingString.visible, c => c.AuditedInd == true);
																		}
																		var SubmitCheckboxCol = SummaryDiv.Helpers.DivC("col-md-6 text-center pad-top-15");
																		{
																			var SubmitHeading = SubmitCheckboxCol.Helpers.HTML("<h3>Acceptance</h3><p>I hereby declare that the assessment information provided is true and correct.</p>");
																			var AcceptCheck = SubmitCheckboxCol.Helpers.DivC("");
																			{
																				AcceptCheck.Helpers.HTML("<div class='checkbox'><label style='font-size: 2em'>");
																				var AcceptAssessmentInd = AcceptCheck.Helpers.EditorFor(c => c.AcceptedInd);
																				AcceptAssessmentInd.Attributes.Add("id", "AcceptedCheckbox");
																				AcceptCheck.Helpers.HTML("<span class='cr'><i class='cr-icon fa fa-check'></i></span></label></div>");
																			}
																		}
																	}
																}
																var SummarySubmitRowDiv = SummarySignOffContentDiv.Helpers.DivC("row margin0");
																{
																	var SubmitText = SummarySubmitRowDiv.Helpers.DivC("col-md-12 text-center pad-top-15");
																	{
																		var ThreatSaveBtn = SubmitText.Helpers.Button("Submit for review", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																		{
																			ThreatSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireAnswerSet($data)");
																			ThreatSaveBtn.Attributes.Add("id", "submitAssessment");
																			ThreatSaveBtn.AddClass("btn btn-primary");
																			ThreatSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageSubmitSummaryAssessmentBtnInd == true);
																			ThreatSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.enable, c => ViewModel.ShowAssessmentResultsPageInd == true && ViewModel.FirstQuestionnaireAnswerSet.AcceptedInd == true);
																			//ThreatSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.text, c => c.FirstQuestionnaireAnswerSet.SubmitButtonText);
																		}
																	}
																}
															}
														}
														#endregion
													}
													#endregion

													// Results Tab
													#region ResultsTab
													var ResultsTab = EntityTab.AddTab("Results");
													{
														var ResultsNoAccessRow = ResultsTab.Helpers.DivC("row  margin0");
														{
															var AssessmentNoAccess = ResultsNoAccessRow.Helpers.DivC("col-md-12  p-n-lr");
															{
																AssessmentNoAccess.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewResultsTabInd == false);
																var OverviewNoAccessColContainerBox = AssessmentNoAccess.Helpers.DivC("ibox float-e-margins paddingBottom");
																{
																	var OverviewNoAccessContentBox = OverviewNoAccessColContainerBox.Helpers.DivC("ibox-content");
																	{
																		var OverviewNoAccessContentBoxRow = OverviewNoAccessContentBox.Helpers.DivC("row");
																		{
																			var OverviewNoAccessContentBoxRowCol = OverviewNoAccessContentBoxRow.Helpers.DivC("col-md-12 ");
																			{
																				OverviewNoAccessContentBoxRowCol.Helpers.HTML("<h2 style='color:#1ab394;'>Assessment Results</h2>");
																				OverviewNoAccessContentBoxRowCol.Helpers.HTML("<span>You do not have access. Please confirm your security role/group settings.</span>");
																			}
																		}
																	}
																}
															}
														}

														#region ResultsOverview
														var ResultsOverview = ResultsTab.Helpers.DivC("ibox float-e-margins paddingBottom");
														{
															ResultsOverview.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewResultsTabInd == true);
															var ResultsOverviewTitleDiv = ResultsOverview.Helpers.DivC("ibox-title");
															{
																ResultsOverviewTitleDiv.Helpers.HTML("<i class='fa fa-coffee fa-lg fa-fw pull-left'></i>");
																ResultsOverviewTitleDiv.Helpers.HTML().Heading5("Assessment Results");
															}
															var ResultsOverviewToolsDiv = ResultsOverviewTitleDiv.Helpers.DivC("ibox-tools");
															{
																var aResultsOverviewToolsTag = ResultsOverviewToolsDiv.Helpers.HTMLTag("a");
																aResultsOverviewToolsTag.AddClass("collapse-link");
																{
																	var iResultsOverviewToolsTag = aResultsOverviewToolsTag.Helpers.HTMLTag("i");
																	iResultsOverviewToolsTag.AddClass("fa fa-chevron-up");
																}
															}
															var ResultsOverviewContentDiv = ResultsOverview.Helpers.DivC("ibox-content");
															{
																var NoResultsMessageRow = ResultsOverviewContentDiv.Helpers.DivC("row assessmentMessageBox");
																NoResultsMessageRow.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.ShowAssessmentResultsPageInd == false);
																{
																	var NoResultsMessageRowDiv = NoResultsMessageRow.Helpers.DivC("col-md-12 text-center pad-top-15");
																	{
																		NoResultsMessageRowDiv.Helpers.Span("<div><i class='fa fa-info-circle infoIconStyle'></i></div><i class='assessmentMessage'>Please answer all questions and submit the assessment in order to view your assessment results.</i><br><br>");
																	}
																}
																var TotalsOverviewRowDiv = ResultsOverviewContentDiv.Helpers.DivC("row margin0");
																TotalsOverviewRowDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.ShowAssessmentResultsPageInd == true);
																{
																	var Col1 = TotalsOverviewRowDiv.Helpers.DivC("col-md-6  text-center pad-top-15");
																	{
																		var Col1Row = Col1.Helpers.DivC("row");
																		{
																			var ScoreIndexes = Col1Row.Helpers.With<METTLib.RO.ROQuestionnaireAnswerScoreAssessor>(c => c.FirstROQuestionnaireAnswerScoreAssessor);
																			{
																				var ScoreText = ScoreIndexes.Helpers.DivC("col-md-6  text-center pad-top-15");
																				{
																					var ScoreTextHeading = ScoreText.Helpers.HTMLTag("h3");
																					ScoreTextHeading.Helpers.HTML("Total Index");

																					var ScoreTextValue = ScoreText.Helpers.HTMLTag("div");
																					ScoreTextValue.AddClass("indextotal");

																					var ScoreTextValueHeading = ScoreTextValue.Helpers.HTMLTag("h1");
																					//ScoreTextValueHeading.AddClass("counter");
																					//ScoreTextValueHeading.Bindings.Add(Singular.Web.KnockoutBindingString.text, "$data.TotalIndexAssessorPct()");
																					//ScoreTextValueHeading.Bindings.Add(Singular.Web.KnockoutBindingString.text, ViewModel.AssessorScoreIndex.ToString());
																					ScoreTextValueHeading.Helpers.ReadOnlyFor(c => ViewModel.AssessorScoreIndex);

																					var ScoreTextValueHeadingPer = ScoreTextValue.Helpers.HTMLTag("div");
																					ScoreTextValueHeadingPer.AddClass("ScorePercentage");
																					ScoreTextValueHeadingPer.Helpers.HTML("%");

																				}
																			}
																			var ResultsScore = Col1Row.Helpers.With<METTLib.Questionnaire.QuestionnaireAnswerSet>(c => c.FirstQuestionnaireAnswerSet);
																			{
																				var AnswerSet1 = ResultsScore.Helpers.DivC("col-md-6 text-center pad-top-15");
																				{
																					AnswerSet1.Helpers.BootstrapEditorRowFor(c => c.LeadAssessor);
																					AnswerSet1.Helpers.BootstrapEditorRowFor(c => c.ReviewerFullName);
																					AnswerSet1.Helpers.BootstrapEditorRowFor(c => c.AssessmentDate);
																				}
																			}
																		}
																		var Col1Row2 = Col1.Helpers.DivC("row");
																		{
																			// Col1Row2.Helpers.HTML("<small style='pad-top-15;color:#C8C8C8;'>This assessment is not a measure of the site manager's performance, but it is rather a reflection on the organisation's proficiency in site management.The end result is in fact an index and not a score, it gives an indication of where improvements have been made from the previous assessment and where further improvements are required. Thus calculation of regional, organistional and averages should be used with extreme caution.</small>");
																		}
																	}
																	var Col2 = TotalsOverviewRowDiv.Helpers.DivC("col-md-6 pad-top-15");
																	{
																		var Col2Row = Col2.Helpers.DivC("row margin0");
																		{
																			Col2Row.Helpers.HTML("<h3>Intervention Report</h3>");
																			Col2Row.Helpers.HTML("<p>The downloadable (MS-Word format) intervention report details all findings per indicator ratings and next steps grouped into management spheres.</p>");
																			var InterventionReportBtn = Col2.Helpers.Button("Generate Report", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																			{
																				InterventionReportBtn.AddClass("btn btn-primary btn-outline");
																				InterventionReportBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "GenerateInterventionRpt($data)");
																				InterventionReportBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageGenerateReportResultsAssessmentBtnInd == true);
																			}
																		}
																		var Col2Row2 = Col2.Helpers.DivC("row margin0");
																		{
																			// Col2Row2.Helpers.HTML("<p><br><small style='pad-top-15;color:#C8C8C8;'>This assessment is not a measure of the site manager's performance, but it is rather a reflection on the organisation's proficiency in site management.The end result is in fact an index and not a score, it gives an indication of where improvements have been made from the previous assessment and where further improvements are required. Thus calculation of regional, organistional and averages should be used with extreme caution.</small></p>");
																		}
																	}
																}

																//TEST PUSH
																var AuditRow = ResultsOverviewContentDiv.Helpers.DivC("row");
																AuditRow.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.ShowAssessmentResultsPageInd == true);
																{
																	var ReviewOverviewUsersRowDivText = AuditRow.Helpers.DivC("col-md-12 text-center pad-top-15");
																	{
																		var SaveBtn = ReviewOverviewUsersRowDivText.Helpers.Button("", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																		{
																			SaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireAnswerSet($data, true)");
																			SaveBtn.AddClass("btn btn-primary ");
																			SaveBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageSaveAssessmentResultsAssessmentBtnInd == true);
																			SaveBtn.AddBinding(Singular.Web.KnockoutBindingString.text, c => ViewModel.FirstQuestionnaireAnswerSet.SubmitAssessmentButtonText);
																		}
																	}
																}
															}
														}
														#endregion

														#region ResultsOverview
														var ResultsOverview2 = ResultsTab.Helpers.DivC("ibox float-e-margins paddingBottom");
														//ResultsOverview2.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.ShowAssessmentResultsPageInd == true);
														ResultsOverview2.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.CanViewRequestAuditSection == true);
														{
															var ResultsOverviewTitleDiv2 = ResultsOverview2.Helpers.DivC("ibox-title");
															{
																ResultsOverviewTitleDiv2.Helpers.HTML("<i class='fa fa-sign-out fa-lg fa-fw pull-left'></i>");
																ResultsOverviewTitleDiv2.Helpers.HTML().Heading5("Audit Request");
															}
															var ResultsOverviewToolsDiv2 = ResultsOverviewTitleDiv2.Helpers.DivC("ibox-tools");
															{
																var aResultsOverviewToolsTag2 = ResultsOverviewToolsDiv2.Helpers.HTMLTag("a");
																aResultsOverviewToolsTag2.AddClass("collapse-link");
																{
																	var iResultsOverviewToolsTag2 = aResultsOverviewToolsTag2.Helpers.HTMLTag("i");
																	iResultsOverviewToolsTag2.AddClass("fa fa-chevron-up");
																}
															}
															var ResultsOverviewContentDiv2 = ResultsOverview2.Helpers.DivC("ibox-content");
															{
																var TotalsOverviewRowDiv2 = ResultsOverviewContentDiv2.Helpers.DivC("row margin0");
																{
																	var Col = TotalsOverviewRowDiv2.Helpers.DivC("col-md-12");
																	{
																		var Cols = Col.Helpers.DivC("row margin0");
																		{
																			var AuditCheck0 = Cols.Helpers.DivC("col-md-12");
																			{
																				AuditCheck0.Helpers.HTML("<h3>Request Audit</h3>");
																			}
																			var AuditCheck2 = Cols.Helpers.DivC("col-md-1 pad-top-15");
																			{
																				var SummaryDiv = AuditCheck2.Helpers.With<METTLib.Questionnaire.QuestionnaireAnswerSet>(c => c.FirstQuestionnaireAnswerSet);
																				{
																					var AuditCheckBox = SummaryDiv.Helpers.DivC("");
																					AuditCheckBox.Helpers.HTML("<div class='checkbox'><label style='font-size: 2em'>");
																					var AuditChecktInd = AuditCheckBox.Helpers.EditorFor(c => c.AuditedInd);
																					AuditChecktInd.Attributes.Add("id", "AuditCheckbox");
																					AuditCheckBox.Helpers.HTML("<span class='cr'><i class='cr-icon fa fa-check'></i></span></label></div>");
																				}
																			}
																			var AuditCheck = Cols.Helpers.DivC("col-md-11 pad-top-15");
																			{
																				AuditCheck.Helpers.HTML("<h3>New Assessment Audit Request</h3>");
																				AuditCheck.Helpers.HTML("<p> This option is to request an audit to be carried out on this assessment by a designated METT-SA Auditor. Audit comments will be noted against the original assessment with suggested ratings shown however the original score submitted will not be changed.</p><p><b>Thus calculation of regional, organisational and averages should be used with extreme caution.</b></p>");
																				var RequestAuditBtn = AuditCheck.Helpers.Button("Request Audit", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																				{
																					RequestAuditBtn.AddClass("btn btn-primary ");
																					RequestAuditBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireAnswerSet($data)");
																					RequestAuditBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageRequestAuditResultsAssessmentBtnInd == true);
																					RequestAuditBtn.AddBinding(Singular.Web.KnockoutBindingString.enable, c => ViewModel.FirstQuestionnaireAnswerSet.AuditedInd == true);
																					//RequestAuditBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageAuditorControlsDisabledInd != true);
																				}
																			}
																		}
																	}
																}
															}
														}
														#endregion

														#region ResultsReports
														var ResultsReports = ResultsTab.Helpers.DivC("ibox float-e-margins paddingBottom");
														ResultsReports.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.CanViewAuditResultsSection == true);
														{
															var ResultsReportsTitleDiv = ResultsReports.Helpers.DivC("ibox-title");
															{
																ResultsReportsTitleDiv.Helpers.HTML("<i class='fa fa-coffee fa-lg fa-fw pull-left'></i>");
																ResultsReportsTitleDiv.Helpers.HTML().Heading5("Audit Results");
															}
															var ResultsReportsToolsDiv = ResultsReportsTitleDiv.Helpers.DivC("ibox-tools");
															{
																var aResultsReportsToolsTag = ResultsReportsToolsDiv.Helpers.HTMLTag("a");
																aResultsReportsToolsTag.AddClass("collapse-link");
																{
																	var iResultsReportsToolsTag = aResultsReportsToolsTag.Helpers.HTMLTag("i");
																	iResultsReportsToolsTag.AddClass("fa fa-chevron-up");
																}
															}
															var ResultsReportsContentDiv = ResultsReports.Helpers.DivC("ibox-content");
															{
																var ResultsReportsRow3Div = ResultsReportsContentDiv.Helpers.DivC("row margin0");
																{
																	var Col1 = ResultsReportsRow3Div.Helpers.DivC("col-md-6  text-center pad-top-15");
																	{
																		var Col1Row = Col1.Helpers.DivC("row");
																		{
																			var ScoreIndexes = Col1Row.Helpers.With<METTLib.RO.ROQuestionnaireAnswerScoreAuditor>(c => c.FirstROQuestionnaireAnswerScoreAuditor);
																			{
																				var ScoreText = ScoreIndexes.Helpers.DivC("col-md-6  text-center pad-top-15");
																				{
																					var ScoreTextHeading = ScoreText.Helpers.HTMLTag("h3");
																					ScoreTextHeading.Helpers.HTML("Audit Index");

																					var ScoreTextValue = ScoreText.Helpers.HTMLTag("div");
																					ScoreTextValue.AddClass("indextotal");

																					var ScoreTextValueHeading = ScoreTextValue.Helpers.HTMLTag("h1");
																					//ScoreTextValueHeading.AddClass("counter");
																					//ScoreTextValueHeading.Bindings.Add(Singular.Web.KnockoutBindingString.text, "$data.TotalIndexAuditorPct()");
																					ScoreTextValueHeading.Helpers.ReadOnlyFor(c => ViewModel.AuditorScoreIndex);

																					var ScoreTextValueHeadingPer = ScoreTextValue.Helpers.HTMLTag("div");
																					ScoreTextValueHeadingPer.AddClass("ScorePercentage");
																					ScoreTextValueHeadingPer.Helpers.HTML("%");

																				}
																			}
																			var ResultsScore = Col1Row.Helpers.With<METTLib.Questionnaire.QuestionnaireAnswerSet>(c => c.FirstQuestionnaireAnswerSet);
																			{
																				var AnswerSet1 = ResultsScore.Helpers.DivC("col-md-6 text-center pad-top-15");
																				{
																					AnswerSet1.Helpers.BootstrapEditorRowFor(c => c.AuditorFullName);
																					AnswerSet1.Helpers.BootstrapEditorRowFor(c => c.AuditDate);
																				}
																			}
																		}
																	}
																	var Col2 = ResultsReportsRow3Div.Helpers.DivC("col-md-6 pad-top-15");
																	{
																		var Col2Row = Col2.Helpers.DivC("row margin0");
																		{
																			var ResultsScore = Col2Row.Helpers.With<METTLib.Questionnaire.QuestionnaireAnswerSet>(c => c.FirstQuestionnaireAnswerSet);
																			{
																				ResultsScore.Helpers.BootstrapEditorRowFor(c => c.AuditComments);
																			}
																			var SaveBtnAudit = Col2Row.Helpers.Button("Submit Audit", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																			{
																				SaveBtnAudit.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireAnswerSet($data, null, true)");
																				SaveBtnAudit.AddClass("btn btn-primary");
																				SaveBtnAudit.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageSubmitAuditResultsAssessmentBtnInd == true);
																			}
																			var AuditReportBtn = Col2Row.Helpers.Button("View Audit Report", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																			{
																				AuditReportBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewAuditReport($data)");
																				AuditReportBtn.AddClass("btn btn-primary btn-outline");
																			}

																		}
																	}
																}
															}
														}
														#endregion
													}
													#endregion
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	%>

    <script type="text/javascript" src="../Theme/jquery.tinyTips.js"></script>
	<script type="text/javascript" src="../Scripts/JSLINQ.js"></script>
	<script type="text/javascript" src="../Scripts/accesscheck.js"></script>
	<script type="text/javascript">

		$(document).ready(function () {
			$('span.tTip').tinyTips('light', 'title');
        });

		Singular.OnPageLoad(function () {
			Singular.ShowLoadingBar();
			$("#menuItem3").addClass("active");
			$("#menuItem3 > ul").addClass("in");
			// Using jQuery to remove strange Img Icon being placed over tick boxes
			$("[id^='AcceptedInd']").siblings(".ImgIcon").remove();
			$("[id^='ActiveAssessmentYearInd']").siblings(".ImgIcon").remove();
			$("[id^='AuditedInd']").siblings(".ImgIcon").remove();
			var selectedTab = getParameterByName('Tab');
			ViewModel.SelectedTab(selectedTab);
			// Wizard Set Last Step Id as Active
			var stepID = ViewModel.paramAssessmentStepId() - 1;
			$("#ul-tab-list li:eq(" + (stepID) + ")").addClass("active").show();
			Singular.HideLoadingBar();
			$("[id^=MagnitudeRatingID]").attr("disabled", "disabled");
			$("[id^=RatingRatingID]").attr("disabled", "disabled");
			$("#summary tr td").addClass("tdPosition");
		});
		function getParameterByName(name, url) {
			if (!url) url = window.location.href;
			name = name.replace(/[\[\]]/g, '\\$&');
			var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
				results = regex.exec(url);
			if (!results) return null;
			if (!results[2]) return 0;
			return decodeURIComponent(results[2].replace(/\+/g, ' '));
		}

		var OnTabChanged = function () {
			if (ViewModel.SelectedTab() == 3) {
				// Refresh Summary Information
			};
			if (ViewModel.SelectedTab() == 4) {
				// Animate Index Score Numbers
				$('.counter').each(function () {
					$(this).prop('Counter', 0).animate({
						Counter: $(this).text()
					}, {
							duration: 1000,
							easing: 'swing',
							step: function (now) {
								$(this).text(Math.ceil(now));
							}
						});
				});

				ViewModel.CallServerMethod('UpdateAssessorIndex', { QuestionnaireAnswerSetID: ViewModel.paramQuestionnaireAnswerSetId() }, function (result) {
					if (result.Success) {
						//ViewModel.ROQuestionnaireAnswerScoreAssessor.Set(result.Data);
						ViewModel.AssessorScoreIndex(result.Data);
					}
					else {
						//	METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				})

				ViewModel.CallServerMethod('UpdateAuditorIndex', { QuestionnaireAnswerSetID: ViewModel.paramQuestionnaireAnswerSetId() }, function (result) {
					if (result.Success) {
						ViewModel.AuditorScoreIndex(result.Data);
					}
					else {
						//	METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				})

			};
		};

		// Overview Tab Scripts
		var ViewFAQ = function (obj) {
			ViewModel.IsViewingSurveyInd(false);
			ViewModel.IsViewingFAQInstructionsInd(false);
			ViewModel.IsViewingFAQRoleInd(true);
			ViewModel.IsViewingFAQGuidelinesInd(false);
		};

		var ViewInstructionsFAQ = function (obj) {
			ViewModel.IsViewingSurveyInd(false);
			ViewModel.IsViewingFAQInstructionsInd(true);
			ViewModel.IsViewingFAQRoleInd(false);
			ViewModel.IsViewingFAQGuidelinesInd(false);
		};

		var ViewRoleFAQ = function (obj) {
			ViewModel.IsViewingSurveyInd(false);
			ViewModel.IsViewingFAQInstructionsInd(false);
			ViewModel.IsViewingFAQRoleInd(true);
			ViewModel.IsViewingFAQGuidelinesInd(false);
		};

		var CloseFAQ = function (obj) {
			ViewModel.IsViewingSurveyInd(true);
			ViewModel.IsViewingFAQInstructionsInd(false);
			ViewModel.IsViewingFAQRoleInd(false);
			ViewModel.IsViewingFAQGuidelinesInd(false);
		};

		// QuestionnaireAnswerSet ID to populate Overview, Summary, Results Tab
		var ViewQuestionnaireAnswerSet = function (obj) {
			Singular.ShowLoadingBar();
			ViewModel.CallServerMethod('GetQuestionnaireAnswerSet', { QuestionnaireAnswerSetID: obj.QuestionnaireAnswerSetID(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					ViewModel.EditQuestionnaireAnswerSet.Set(result.Data);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			}
			);
			Singular.HideLoadingBar();
		}

		function ShowIndicator(obj) {
			Singular.ShowLoadingBar();
			// 1/3 Change to Assessment Tab
			$('.tabs-container #ui-id-3').click();

			// 2/3 Show selected group indicators
			var qGroupId = obj.QuestionnaireGroupID();
			$('#ul-tab-list #' + qGroupId).click();

			// 3/3 Set specific indicator to active
			var qIndicatorDetailId = obj.IndicatorDetailID();
			$('#navigation #userTabs' + qIndicatorDetailId).click();

			Singular.HideLoadingBar();
		}

		// Threats Tab Scripts
		var UpdateThreatAnswer = function (obj) {
			if (obj.IsDirty()) {
				ThreatAnswerObject.CallServerMethod('SaveThreatAnswer', { ThreatAnswerList: ViewModel.ThreatAnswersList.Serialise(), ShowLoadingBar: false }, function (result) {
					if (result.Success) {
						METTHelpers.Notification("Threat Answer Successfully Saved", 'center', 'success', 5000);
					}
					else {
						METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				});
			}
		}

		var ViewThreatAnswer = function (obj) {
			ViewModel.AddThreatInd = false;
			ViewModel.EditingThreatAnswer.Set(obj);
			ViewModel.IsViewingThreatAnswerInd(true);
			ViewModel.CallServerMethod('GetThreatAnswer', { ThreatAnswerID: obj.ThreatAnswerID(), QuestionnaireAnswerSetID: obj.QuestionnaireAnswerSetID() }, function (result) {
				if (result.Success) {
					ViewModel.EditingThreatAnswer.Set(result.Data);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			});
			$("[id^=MagnitudeRatingID]").attr("disabled", "disabled");
			$("[id^=RatingRatingID]").attr("disabled", "disabled");
		}

		var SaveThreatAnswer = function (obj) {
			ThreatAnswerObject.CallServerMethod('SaveThreatAnswer', { ThreatAnswerList: ViewModel.ThreatAnswersList.Serialise(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					METTHelpers.Notification("Threat Answer Successfully Saved", 'center', 'success', 5000);
					ViewModel.ThreatAnswersList.Set(result.Data);
					$('#modalThreatEdit').modal('hide');
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			});
		}

		var RefreshThreatList = function (obj) {
			if (ViewModel.AddThreatInd == true) {
				var indexofLastItem = ViewModel.ThreatAnswersList().length - 1;
				var lastItem = ViewModel.ThreatAnswersList()[indexofLastItem];
				ViewModel.ThreatAnswersList.RemoveNoCheck(lastItem);
			}
		}

		// Calculate Magnitude Rating by comparing Scope and Severity to the threat matrix
		// Calculate Overall Rating by comparing Magnitude and Irreversibility to the threat matrix
		var GetMatrixRating = function (obj) {
			if (obj != null && (obj.ScopeRatingID() != null && obj.SeverityRatingID() != null)) {
				ViewModel.CallServerMethod('GetThreatMatrixRating', { EditingThreatAnswer: obj.Serialise() }, function (result) {
					if (result.Success) {
						ViewModel.EditingThreatAnswer.Set(result.Data);
					}
					else {
						METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				})
			}
		}

		var AddThreat = function (obj) {
			ViewModel.AddThreatInd = true;
			var foundDuplicate = false;
			// Create new threat object
			var newObj = new ThreatAnswerObject();
			newObj.QuestionnaireAnswerSetID(ViewModel.paramQuestionnaireAnswerSetId()); // Questionnaire we are adding a threat to
			newObj.ThreatCategoryID(obj.ThreatCategoryID());
			newObj.ThreatSubCategoryID(obj.ThreatSubCategoryID());

			ViewModel.ThreatAnswersList.Add(newObj);
			ViewModel.EditingThreatAnswer.Set(newObj);
			ViewModel.IsViewingThreatAnswerInd(true);
			ViewModel.IsViewingThreatsTableInd(true);
			ViewModel.IsViewingThreatsMessageInd(false);

			$('#modalThreatEdit').modal('show');
			$("[id^=MagnitudeRatingID]").attr("disabled", "disabled");
			$("[id^=RatingRatingID]").attr("disabled", "disabled");
		}

		var RemoveThreatAnswer = function (obj) {
			METTHelpers.QuestionDialogYesNo("Are you sure you would like to delete this threat?", 'center',
				function () { // Yes 
					ViewModel.ThreatAnswersList.RemoveNoCheck(obj);
					SaveThreatAnswer(obj);
				},
				function () { // No
				})
		}

		// Assessment Tab Scripts
		var ShowQuestionnaireGroupData = function (obj) {
			$('ul.nav li').on('click', function () {
				$(this).parent().find('li.active').removeClass('active');
				$(this).addClass('active');
			});
			Singular.ShowLoadingBar();
			ViewModel.CallServerMethod('GetSurveyQuestionnaireGroupData', { QGroupID: obj.QuestionnaireGroupID(), QuestionnaireAnswerSetID: ViewModel.paramQuestionnaireAnswerSetId() }, function (result) {
				if (result.Success) {
					ViewModel.QuestionnaireQuestionList.Set(result.Data.Item1)
					ViewModel.FirstQuestionID(result.Data.Item2)
					METTHelpers.horizontalTabControl("userTabs");
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}

			})
			Singular.HideLoadingBar();
		}

		var UpdateQAAssessor = function (obj) {
			return obj.IsSelectedInd();
		}

		var UpdateQACheckAssessor = function (obj) {
			for (var i = 0; i < obj.GetParent().QuestionnaireQuestionTypeList().length; i++) {
				if (obj.GetParent().QuestionnaireQuestionTypeList()[i].QuestionnaireQuestionAnswerOptionID() != obj.QuestionnaireQuestionAnswerOptionID()) {
					obj.GetParent().QuestionnaireQuestionTypeList()[i].IsSelectedInd(false);
				} else {
					obj.IsSelectedInd(!obj.IsSelectedInd());
				}
			}
		}

		var UpdateQAReviewer = function (obj) {
			return obj.IsSelectedInd();
		}

		var UpdateQACheckReviewer = function (obj) {
			for (var i = 0; i < obj.GetParent().QuestionnaireQuestionTypeList().length; i++) {
				if (obj.GetParent().QuestionnaireQuestionTypeList()[i].QuestionnaireQuestionAnswerOptionID() != obj.QuestionnaireQuestionAnswerOptionID()) {
					obj.GetParent().QuestionnaireQuestionTypeList()[i].IsSelectedIndAuditor(false);
				} else { obj.IsSelectedIndAuditor(!obj.IsSelectedIndAuditor()); }
			}
		}

		function QuestionStepBack(obj) {
			// Indicators Items 
			var $list = $("#navigation li");
			var idx = $("#navigation .active")
				.removeClass("li active")
				.prev()
				.index();
			if (idx == -1) {
				idx = 0;
				$list.eq(idx).addClass("active");
				return;
			}
			$list.eq(idx).addClass("active");

			// Tab Content Items
			var $listtp = $("#tab-content div");
			var prevElem = $("#tab-content .active").prev();
			var tpidx = $("#tab-content .active")
				.removeClass("active")
				.next()
				.index();
			if (tpidx == -1) tpidx = 0;
			prevElem.addClass("active");
		}

		// Save Answer and Comments on Button Next Step 	
		function QuestionStepNext(obj) {
			Singular.ShowLoadingBar();
			// Save Question Answer
			var QAobj = new QuestionnaireAnswerObject();
			QAobj.QuestionnaireAnswerSetID(ViewModel.paramQuestionnaireAnswerSetId());
			var QQAOID = 0;
			var QQAAOID = 0;
			var selectedAnswerOption = 0;
			var selectedAnswerOptionAuditor = 0;

			var QQAOIDRating = 0;

			for (var i = 0; i < ViewModel.QuestionnaireQuestionList().length; i++) {
				// Save Part 1 
				if (ViewModel.QuestionnaireQuestionList()[i].QuestionID() == obj.QuestionID()) {
					QAobj.QuestionnaireQuestionTypeID(ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList()[0].QuestionnaireQuestionTypeID());
					QAobj.Comments(ViewModel.QuestionnaireQuestionList()[i].Comments());
					QAobj.NextSteps(ViewModel.QuestionnaireQuestionList()[i].NextSteps());
					QAobj.Evidence(ViewModel.QuestionnaireQuestionList()[i].Evidence());
					QAobj.CommentsAuditor(ViewModel.QuestionnaireQuestionList()[i].CommentsAuditor());
					QAobj.NextStepsAuditor(ViewModel.QuestionnaireQuestionList()[i].NextStepsAuditor());
					QAobj.EvidenceAuditor(ViewModel.QuestionnaireQuestionList()[i].EvidenceAuditor());
					//QAobj.AuditedBy(1);
				}
				// Save Part 2 
				for (var k = 0; k < ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList().length; k++) {
					if (
						(ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList()[k].QuestionnaireQuestionTypeID() == QAobj.QuestionnaireQuestionTypeID()) && ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList()[k].IsSelectedInd()) {
						QQAOID = ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList()[k].QuestionnaireQuestionAnswerOptionID();

						QQAOIDRating = ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList()[k].AnswerRating();

						selectedAnswerOption = k;

					}
				}
				// Auditor Section
				for (var j = 0; j < ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList().length; j++) {
					if (
						(ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList()[j].QuestionnaireQuestionTypeID() == QAobj.QuestionnaireQuestionTypeID()) && ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList()[j].IsSelectedIndAuditor()) {
						QQAAOID = ViewModel.QuestionnaireQuestionList()[i].QuestionnaireQuestionTypeList()[j].QuestionnaireQuestionAnswerOptionID();
						selectedAnswerOptionAuditor = j;
					}
				}

			}

			// Validation
			if (QQAOID == 0) {
				METTHelpers.Notification("Please select an answer option...", 'center', 'warning', 5000);
				Singular.HideLoadingBar();
				return false;
			}

			// Validation 
			if (obj.Comments().length < 5) {
				METTHelpers.Notification("Selected answer requires valid Comments (more than 5 characters...)", 'center', 'warning', 5000);
				Singular.HideLoadingBar();
				return false;
			}

			switch (selectedAnswerOption) {
				case 0:
					if (obj.NextSteps().length < 5 && QQAOIDRating != -1) {
						METTHelpers.Notification("Selected answer requires valid Next Steps (more than 5 characters...)", 'center', 'warning', 5000);
						Singular.HideLoadingBar();
						return false;
					}
					break;
				case 1:
					if (obj.NextSteps().length < 5 && QQAOIDRating != -1) {
						METTHelpers.Notification("Selected answer requires valid Next Steps (more than 5 characters...)", 'center', 'warning', 5000);
						Singular.HideLoadingBar();
						return false;
					}
					if (obj.Evidence().length < 5 && QQAOIDRating != -1) {
						METTHelpers.Notification("Selected answer requires valid  Evidence (more than 5 characters...)", 'center', 'warning', 5000);
						Singular.HideLoadingBar();
						return false;
					}
					break;
				case 2:
					if (obj.NextSteps().length < 5 && QQAOIDRating != -1) {
						METTHelpers.Notification("Selected answer requires valid Next Steps (more than 5 characters...)", 'center', 'warning', 5000);
						Singular.HideLoadingBar();
						return false;
					}
					if (obj.Evidence().length < 5 && QQAOIDRating != -1) {
						METTHelpers.Notification("Selected answer requires valid  Evidence (more than 5 characters...)", 'center', 'warning', 5000);
						Singular.HideLoadingBar();
						return false;
					}
					break;
				case 3:
					if (obj.Evidence().length < 5 && QQAOIDRating != -1) {
						METTHelpers.Notification("Selected answer requires valid  Evidence (more than 5 characters...)", 'center', 'warning', 5000);
						Singular.HideLoadingBar();
						return false;
					}
					break;
			}

			Singular.HideLoadingBar();

			QAobj.QuestionnaireQuestionAnswerOptionID(QQAOID);
			QAobj.QuestionnaireQuestionAnswerOptionIDAuditor(QQAAOID);
			ViewModel.CallServerMethod('SaveQuestionAnswer', { QA: QAobj.Serialise(), QuestionnaireAnswerSetID: ViewModel.paramQuestionnaireAnswerSetId(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					METTHelpers.Notification("Assessment Answer Saved", 'center', 'success', 1000);
					// Indicators Items
					var $list = $("#navigation li");
					var idx = $("#navigation .active")
						.removeClass("active")
						.next()
						.index();
					if (idx == -1) {
						idx = 0;
					}

					$list.eq(idx).addClass("active");
					// Tab Content Items
					var $listtp = $("#tab-content div");
					var nextElem = $("#tab-content .active").next();
					var tpidx = $("#tab-content .active")
						.removeClass("active")
						.next()
						.index();
					if (tpidx == -1) tpidx = 0;
					nextElem.addClass("active");

					ViewModel.ROQuestionnaireGroupAnswerResultList.Set(result.Data.Item2);
					ViewModel.ROQuestionnaireAnswerScoreList.Set(result.Data.Item3);
					ViewModel.ShowAssessmentResultsPageInd(result.Data.Item4);
					Singular.HideLoadingBar();
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					Singular.HideLoadingBar();
				}
				$("#summary tr td").addClass("tdPosition");
			})


			ViewModel.CallServerMethod('UpdateAssessorIndex', { QuestionnaireAnswerSetID: ViewModel.paramQuestionnaireAnswerSetId() }, function (result) {
				if (result.Success) {
					//ViewModel.ROQuestionnaireAnswerScoreAssessor.Set(result.Data);
					ViewModel.AssessorScoreIndex(result.Data);
				}
				else {
					//	METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})

			ViewModel.CallServerMethod('UpdateAuditorIndex', { QuestionnaireAnswerSetID: ViewModel.paramQuestionnaireAnswerSetId() }, function (result) {
				if (result.Success) {
					ViewModel.AuditorScoreIndex(result.Data);
				}
				else {
					//	METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})
		}

		// Summary Tab Scripts

		// Results Tab Scripts
		var SaveQuestionnaireAnswerSet = function (obj, submitToDEA, submitAudit) {
			let savedByReviewer = false;
			if (submitToDEA != null) {
				savedByReviewer = true;
			}

			let savedByAuditor = false;
			if (submitAudit != null) {
				savedByAuditor = true;
			}

			QuestionnaireAnswerSetObject.CallServerMethod('SaveQuestionnaireAnswerSet', { QuestionnaireAnswerSetList: ViewModel.QuestionnaireAnswerSetList.Serialise(), SubmitToDEA: savedByReviewer, SubmitAudit: savedByAuditor, ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					METTHelpers.Notification("Assessment Successfully Saved", 'center', 'success', 5000);
					//redirect to Overview tab, this will refresh the page and hide any buttons that needs to be hidden
					let QuestionnaireAnswerSetId = METTHelpers.getUrlParameter('QuestionnaireAnswerSetId');
					let QuestionnaireAnswerStepId = METTHelpers.getUrlParameter('QuestionnaireAnswerStepId');
					let url = "../Survey/Survey.aspx?QuestionnaireAnswerSetId=" + QuestionnaireAnswerSetId + "&QuestionnaireAnswerStepId=" + QuestionnaireAnswerStepId;
					window.location = url;
					// METT-141 
					// ViewModel.QuestionnaireAnswerSetList.Set(result.Data);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			});
		}

		var ViewAssessment = function (obj) {
			ViewModel.CallServerMethod('ManageAssessment', { QuestionnaireAnswerSetID: obj.QuestionnaireAnswerSetID(), AssessmentStepID: obj.AssessmentStepID(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					window.location = result.Data;
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})
		}

		// METT-12
		var DeleteAssessment = function (obj) {
			METTHelpers.QuestionDialogYesNo("Are you sure you would like to delete this assessment?", 'center',
				function () { // Yes 
					ViewModel.CallServerMethod('DeleteAssessment', { QuestionnaireAnswerSetID: obj.QuestionnaireAnswerSetID(), ShowLoadingBar: true }, function (result) {
						if (result.Success) {

							METTHelpers.Notification("Assessment deleted successfully.", 'center', 'success', 5000);
							//refresh the assessment list
							ViewModel.ROAssessmentPagedListManager().Refresh();

						}
						else {
							METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
						}
					})
				},
				function () { // No
				})
		}

		var GenerateInterventionRpt = function (obj) {
			ViewModel.CallServerMethod('InterventionRpt', { QuestionnaireAnswerSetID: ViewModel.paramQuestionnaireAnswerSetId(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					Singular.DownloadFile(null, result.Data);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})
		}

		var ClearAssessmentFilters = function () {
			ViewModel.ROAssessmentPagedListCriteria().METTReportingName(null);
			ViewModel.ROAssessmentPagedListManager().Refresh();
		}

		var ViewOrganisation = function (obj) {
			if (obj != null) {
				ViewModel.CallServerMethod('ViewOrganisation', { OrganisationID: obj.OrganisationID(), QuestionnaireAnswerSetID: obj.QuestionnaireAnswerSetID() }, function (result) {
					if (result.Success) {
						window.location = result.Data;
					}
					else {
						METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				})
			}

		}

		var ViewProtectedArea = function (obj) {
			if (obj != null) {
				ViewModel.CallServerMethod('ViewProtectedArea', { ProtectedAreaID: obj.ProtectedAreaID(), QuestionnaireAnswerSetID: obj.QuestionnaireAnswerSetID() }, function (result) {
					if (result.Success) {
						window.location = result.Data;
					}
					else {
						METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				})
			}

		}

		var Back = function (obj) {
			let urlParam = METTHelpers.getUrlParameter('ProtectedAreaID');
			if (urlParam != null) {
				if (obj != null) {
					ViewModel.CallServerMethod('ViewProtectedArea', { ProtectedAreaID: obj.FirstQuestionnaireAnswerSet().ProtectedAreaID() }, function (result) {
						if (result.Success) {
							window.location = result.Data;
						}
						else {
							METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
						}
					})
				}
			}
			else {
				window.location = "Survey.aspx?Tab=0";
			}
		}

		var ViewAuditReport = function (obj) {
			ViewModel.CallServerMethod('GenerateAuditReport', { QuestionnaireAnswerSetID: ViewModel.paramQuestionnaireAnswerSetId(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					Singular.DownloadFile(null, result.Data);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})
		}

		$(document).keypress(function (e) {
			if (e.keyCode === 13) {
				e.preventDefault();
				return false;
			}
		});

	</script>
</asp:Content>

