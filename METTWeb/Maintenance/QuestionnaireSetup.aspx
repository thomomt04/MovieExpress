<%@ Page Language="C#" Title="Questionnaires" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuestionnaireSetup.aspx.cs" Inherits="METTWeb.Maintenance.QuestionnaireSetup" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/assessment.css" rel="stylesheet" />
	<link href="../Theme/Singular/METTCustomCss/customstyles.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			h.HTML().Heading2("Questionnaires");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<%	
		using (var h = this.Helpers)
		{
			var MainHDiv = h.DivC("row");
			{
				var QuestionnaireGroupContainer = MainHDiv.Helpers.DivC("container");
				{
					var QuestionnaireGroupBreadcrumbRow = QuestionnaireGroupContainer.Helpers.DivC("row");
					{
						var MainDiv = MainHDiv.Helpers.DivC("col-md-12  p-n-lr");
						{
							var EditingQuestionnaireDiv = MainDiv.Helpers.Div();
							{
								var TabContainer = EditingQuestionnaireDiv.Helpers.DivC("tabs-container");
								{
									var EntityTab = TabContainer.Helpers.TabControl();
									{
										EntityTab.Style.ClearBoth();
										EntityTab.AddClass("nav nav-tabs");

										var ThreatsTab = EntityTab.AddTab("Overview");
										{
											var QuestionnaireDiv = ThreatsTab.Helpers.DivC("ibox float-e-margins paddingBottom");
											{
												var SelectedThreatsTitleDiv = QuestionnaireDiv.Helpers.DivC("ibox-title");
												{
													SelectedThreatsTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
													SelectedThreatsTitleDiv.Helpers.HTML().Heading5("Overview Information");
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
												var QuestionnaireDivContentDiv = QuestionnaireDiv.Helpers.DivC("ibox-content");
												{
													var QuestionnaireDivContentDivRow = QuestionnaireDivContentDiv.Helpers.DivC("row");
													{
														var QuestionnaireContent = QuestionnaireDivContentDivRow.Helpers.With<METTLib.Questionnaire.Questionnaire>(c => c.EditQuestionnaire);
														{
															var QuestionnaireNameCol = QuestionnaireContent.Helpers.DivC("col-md-9");
															{
																QuestionnaireNameCol.Helpers.BootstrapEditorRowFor(c => c.QuestionnaireName);
															}
															var QuestionnaireVersionCol = QuestionnaireContent.Helpers.DivC("col-md-3");
															{
																QuestionnaireVersionCol.Helpers.BootstrapEditorRowFor(c => c.QuestionnaireVersionNumber);
															}
															var QuestionnaireStartDateCol = QuestionnaireContent.Helpers.DivC("col-md-3");
															{
																QuestionnaireStartDateCol.Helpers.BootstrapEditorRowFor(c => c.StartDate);
															}
															var QuestionnaireEndDateCol = QuestionnaireContent.Helpers.DivC("col-md-3");
															{
																QuestionnaireEndDateCol.Helpers.BootstrapEditorRowFor(c => c.EndDate);
															}
															var QuestionnairePublishedDateCol = QuestionnaireContent.Helpers.DivC("col-md-3");
															{
																QuestionnairePublishedDateCol.Helpers.BootstrapEditorRowFor(c => c.PublishDateTime);
															}
															var QuestionnairePublishedInd = QuestionnaireContent.Helpers.DivC("col-md-3");
															{
																var PublishedCheck = QuestionnairePublishedInd.Helpers.DivC("");
																{
																	PublishedCheck.Helpers.HTML("<b>Published</b>");
																	PublishedCheck.Helpers.HTML("<div class='checkbox'><label style='font-size: 2em'>");
																	var PublishedInd = PublishedCheck.Helpers.EditorFor(c => c.PublishInd);
																	PublishedInd.Attributes.Add("id", "PublishInd");
																	PublishedCheck.Helpers.HTML("<span class='cr'><i class='cr-icon fa fa-check'></i></span></label></div>");
																}
															}
															var InfoRow = QuestionnaireContent.Helpers.DivC("col-md-12");
															{
																InfoRow.Helpers.HTML("<p><i><b>Important Note:</b><br>Only one questionnaire set can be active / published at any given time. The <b>published</b> questionnaire set will be used when starting a new assessment.</i></p>");
															}
															var SaveRow = QuestionnaireContent.Helpers.DivC("col-md-12");
															{
																var BtnBack = SaveRow.Helpers.Button("Back", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																{
																	BtnBack.AddClass("btn-primary btn btn btn-outline");
																	BtnBack.AddBinding(Singular.Web.KnockoutBindingString.click, "window.history.back();");
																}
																var ThreatSaveBtn = SaveRow.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																{
																	ThreatSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireOverview($data)");
																	ThreatSaveBtn.Attributes.Add("id", "Publish");
																	ThreatSaveBtn.AddClass("btn btn-primary");
																}
															}
														}
													}
												}
											}
										}

										var ContextTab = EntityTab.AddTab("Questionnaire");
										{
											#region AssessmentBreadcrumb

											var AssessmentRow = ContextTab.Helpers.DivC("row");
											{
												var AssessmentNoAccess = AssessmentRow.Helpers.DivC("col-md-12");
												{
													var OverviewQuestionnaireAnswerSet = AssessmentNoAccess.Helpers.With<METTLib.Questionnaire.Questionnaire>(c => c.EditQuestionnaire);
													OverviewQuestionnaireAnswerSet.Helpers.HTML("<h2 class='headingFontColorWeight' data-bind=\"text: $data.QuestionnaireName() \"></h2>");
												}
											}

											var WizardRow = ContextTab.Helpers.DivC("row");
											{
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
																			WizardIconSpan.AddBinding(Singular.Web.KnockoutBindingString.id, c => c.QuestionnaireGroupID);
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

											var AssessmentDiv = ContextTab.Helpers.DivC("row margin0");

											var AssessmentTabContainer = AssessmentDiv.Helpers.DivC("tabs-container");
											{
												var TabContentLeft = AssessmentTabContainer.Helpers.DivC("tabs-left userTabs");
												{
													var TabList = TabContentLeft.Helpers.HTMLTag("ul");
													{
														TabList.AddClass("nav nav-tabs");
														TabList.Attributes.Add("id", "navigation");
														var TabItems = TabList.Helpers.ForEach<METTLib.Maintenance.MAQuestionnaireQuestion>(c => c.MAQuestionnaireQuestionList);
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
													var TabItemContent = TabItemsContents.Helpers.ForEach<METTLib.Maintenance.MAQuestionnaireQuestion>(c => c.MAQuestionnaireQuestionList);
													{
														var TabPane = TabItemContent.Helpers.DivC("tab-pane");
														TabPane.AddBinding(Singular.Web.KnockoutBindingString.id, c => "tab-" + c.QuestionnaireQuestionID);
														TabPane.AddBinding(Singular.Web.KnockoutBindingString.css, "{'active': $data.QuestionnaireQuestionID() == ViewModel.FirstQuestionID()}");
														var TabPaneBody = TabPane.Helpers.DivC("panel-body panelRaiseShadow");
														{
															var TabPaneTitle = TabPaneBody.Helpers.DivC("ibox-title ibox-titleContainer");
															{																
																var Indicator = TabPaneTitle.Helpers.HTMLTag("h3");
																Indicator.Helpers.BootstrapEditorRowFor(c => c.IndicatorDetailName);
															}
															var TabIboxRow = TabPaneBody.Helpers.DivC("row margin0");
															{
																var TabIboxCol = TabIboxRow.Helpers.DivC("col-md-12");
																{
																	var QuestionnaireQuestionHeading = TabIboxCol.Helpers.HTMLTag("h3");
																	{
																		QuestionnaireQuestionHeading.Helpers.HTML("Question");
																	}
																}
															}
															var TabIboxCol2 = TabIboxRow.Helpers.DivC("col-md-12");
															{
																var QuestionnaireQuestionHeading = TabIboxCol2.Helpers.HTMLTag("h3");
																{
																	QuestionnaireQuestionHeading.AddClass("headingFontColorWeight");
																	var QuestionHeading = QuestionnaireQuestionHeading.Helpers.EditorFor(c => c.Question);
																	QuestionHeading.Attributes.Add("style", "width: 100%;padding:5px;");
																}
															}
															var TabIboxCol2T = TabIboxRow.Helpers.DivC("col-md-10");
															{
																var QuestionnaireQuestionHeading = TabIboxCol2T.Helpers.HTMLTag("h4");
																{
																	QuestionnaireQuestionHeading.Helpers.HTML("Question Tooltip");
																}
																var QuestionnaireQuestionHeadingToolTip = TabIboxCol2T.Helpers.DivC("");
																{
																	var QuestionHeadingToolTip = QuestionnaireQuestionHeadingToolTip.Helpers.EditorFor(c => c.QuestionTooltip);
																	QuestionHeadingToolTip.Attributes.Add("style", "width: 100%;padding:5px;");
																}
															}
															var TabIboxCol2SO = TabIboxRow.Helpers.DivC("col-md-2");
															{
																var QuestionnaireQuestionHeadingSortOrder = TabIboxCol2SO.Helpers.HTMLTag("h4");
																{
																	QuestionnaireQuestionHeadingSortOrder.Helpers.HTML("Sort Order");
																}
																var QuestionnaireQuestionSortOrder = TabIboxCol2SO.Helpers.DivC("");
																{
																	var QuestionHeadingSortOrder = QuestionnaireQuestionSortOrder.Helpers.EditorFor(c => c.SortOrder);
																	QuestionHeadingSortOrder.Attributes.Add("style", "width: 100%;padding:5px;");
																}
															}

															var TabIboxColEvidence = TabIboxRow.Helpers.DivC("col-md-12");
															{
																var QuestionnaireQuestionEvidenceHeading = TabIboxColEvidence.Helpers.HTMLTag("h4");
																{
																	QuestionnaireQuestionEvidenceHeading.Helpers.HTML("Evidence Tooltip");
																	QuestionnaireQuestionEvidenceHeading.Attributes.Add("style", "padding-top:10px;");
																}
																var QuestionnaireEvidenceHeadingToolTip = TabIboxColEvidence.Helpers.DivC("");
																{
																	var EvidenceHeadingToolTip = QuestionnaireEvidenceHeadingToolTip.Helpers.EditorFor(c => c.EvidenceTooltip);
																	EvidenceHeadingToolTip.Attributes.Add("style", "width: 100%;padding:5px;");
																}
															}

															var TabIboxCol3 = TabIboxRow.Helpers.DivC("col-md-6  pad-top-15");
															{
																var BtnEdit = TabIboxCol3.Helpers.Button("Edit Question", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																{
																	BtnEdit.AddClass("btn-primary btn btn btn-outline");
																	BtnEdit.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewQuestion($data)");
																	BtnEdit.Attributes.Add("data-toggle", "modal");
																	BtnEdit.Attributes.Add("data-target", "#modalQuestionEdit");
																}
																var BtnSave1 = TabIboxCol3.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																{
																	BtnSave1.AddClass("btn-primary btn btn btn-primary");
																	BtnSave1.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireQuestion($data)");
																}
															}
															var TabIboxCol4 = TabIboxRow.Helpers.DivC("col-md-6 text-right  pad-top-15");
															{
																var BtnNew = TabIboxCol4.Helpers.Button("New Question", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																{
																	BtnNew.AddClass("btn-primary btn btn btn-outline");
																	BtnNew.Attributes.Add("data-toggle", "modal");
																	BtnNew.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewQuestion($data)");
																	BtnNew.Attributes.Add("data-toggle", "modal");
																	BtnNew.Attributes.Add("data-target", "#modalQuestionEdit");
																}
															}
															var AnswerRow = TabIboxRow.Helpers.DivC("row margin0");
															{
																//var QTextHeading = AnswerRow.Helpers.DivC("col-md-6 pad-top-15");
																//{
																//	QTextHeading.Helpers.HTML("<h4>Answer Option(s)</h4>");
																//}
																//var qValueHeadingTooltip = AnswerRow.Helpers.DivC("col-md-4 pad-top-15");
																//{
																//	qValueHeadingTooltip.Helpers.HTML("<h4>Tooltip</h4>");
																//}
																//var qValueHeadingSortOrder = AnswerRow.Helpers.DivC("col-md-1 pad-top-15");
																//{
																//	qValueHeadingSortOrder.Helpers.HTML("<h4>Sort Order</h4>");
																//}
																//var qValueHeading = AnswerRow.Helpers.DivC("col-md-1 pad-top-15");
																//{
																//	qValueHeading.Helpers.HTML("<h4>Rating</h4>");
																//}

																#region Questionnaire Question Answer Options
																var QuestionnaireQuestionAnswerGroup = AnswerRow.Helpers.DivC("col-md-12  pad-top-15");
																{
																	//var QuestionAnswerGroupHeadingText = QuestionnaireQuestionAnswerGroup.Helpers.HTML("<div style='font-size:16px;' class='headerColor'/>Question Answer Options</div>");
																}
																var QuestionLegalItems = QuestionnaireQuestionAnswerGroup.Helpers.TableFor<METTLib.Maintenance.MAQuestionnaireQuestionType>((c) => c.MAQuestionnaireQuestionTypeList, true, true);
																{
																	QuestionLegalItems.AddClass("table table-striped table-bordered table-hover");
																	var QuestionLegalItemsTableFirstRow = QuestionLegalItems.FirstRow;
																	{
																		var LegalDesignationIdCol = QuestionLegalItemsTableFirstRow.AddColumn(c => c.AnswerOption);
																		{
																			//LegalDesignationIdCol.Editor.AddBinding(Singular.Web.KnockoutBindingString.enable, c => c.IsNew);
																		}
																		var LegalDesignationIdCol2 = QuestionLegalItemsTableFirstRow.AddColumn(c => c.QuestionnaireQuestionAnswerTooltip);
																		{
																			//LegalDesignationIdCol2.Editor.AddBinding(Singular.Web.KnockoutBindingString.enable, c => c.IsNew);
																		}
																		var LegalDesignationIdCol3 = QuestionLegalItemsTableFirstRow.AddColumn(c => c.AnswerRating);
																		{
																			//LegalDesignationIdCol3.Editor.AddBinding(Singular.Web.KnockoutBindingString.enable, c => c.IsNew);
																			LegalDesignationIdCol3.Style.Width = "125px";
																		}
																		var LegalDesignationIdCol4 = QuestionLegalItemsTableFirstRow.AddColumn(c => c.SortOrder);
																		{
																			//LegalDesignationIdCol4.Editor.AddBinding(Singular.Web.KnockoutBindingString.enable, c => c.IsNew);
																			LegalDesignationIdCol4.Style.Width = "100px";
																		}

																	}
																}
																var SaveAnswers = AnswerRow.Helpers.DivC("col-md-12 text-right");
																{
																	var BtnSaveQuestionAnswer = SaveAnswers.Helpers.Button("Save Answer Options", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																	{
																		BtnSaveQuestionAnswer.AddClass("btn-primary btn btn btn-primary pad-left-15");
																		BtnSaveQuestionAnswer.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireQuestion($data)");
																	}
																}
																#endregion

																//var QuestionnaireQuestionAnswerGroup = AnswerRow.Helpers.ForEach<METTLib.Maintenance.MAQuestionnaireQuestionType>(c => c.MAQuestionnaireQuestionTypeList);
																//{
																//	var QTextHeading2 = QuestionnaireQuestionAnswerGroup.Helpers.DivC("col-md-6 pad-top-5");
																//	{
																//		var Answer = QTextHeading2.Helpers.EditorFor(c => c.AnswerOption);
																//		Answer.Attributes.Add("style", "width:100%; padding:5px;");
																//	}
																//	var QTextHeading3 = QuestionnaireQuestionAnswerGroup.Helpers.DivC("col-md-4 pad-top-5");
																//	{
																//		var Tooltip = QTextHeading3.Helpers.EditorFor(c => c.QuestionnaireQuestionAnswerTooltip);
																//		Tooltip.Attributes.Add("style", "width:100%; padding:5px;");
																//	}
																//	var QTextHeading4 = QuestionnaireQuestionAnswerGroup.Helpers.DivC("col-md-1 pad-top-5");
																//	{
																//		var AnswerSortOrder = QTextHeading4.Helpers.EditorFor(c => c.SortOrder);
																//		AnswerSortOrder.Attributes.Add("style", "width:100%; padding:5px;");
																//	}

																//	var QTextHeading5 = QuestionnaireQuestionAnswerGroup.Helpers.DivC("col-md-1 pad-top-5");
																//	{
																//		var AnswerValue = QTextHeading5.Helpers.EditorFor(c => c.AnswerRating);
																//		AnswerValue.Attributes.Add("style", "width:30px; padding:5px;");

																//		var ManageBtn = QTextHeading5.Helpers.Button("", Singular.Web.ButtonMainStyle.Danger, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.trash_o);
																//		{
																//			ManageBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "DeleteQuestionAnswerOption($data)");
																//			ManageBtn.AddClass("btn-primary btn btn btn-primary");
																//		}
																//	}
																//}

																//var QTextSave = AnswerRow.Helpers.DivC("col-md-6 pad-top-15");
																//{
																//	var BtnSaveQuestionAnswer = QTextSave.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																//	{
																//		BtnSaveQuestionAnswer.AddClass("btn-primary btn btn btn-primary pad-left-15");
																//		BtnSaveQuestionAnswer.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireQuestion($data)");
																//	}
																//}
																//var QTextAdd = AnswerRow.Helpers.DivC("col-md-6 pad-top-15 text-right");
																//{
																//	var BtnNewQuestionAnswer = QTextAdd.Helpers.Button("New Answer", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																//	{
																//		BtnNewQuestionAnswer.AddClass("btn-primary btn btn btn-outline");
																//		BtnNewQuestionAnswer.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewQuestionAnswerOption($data)");
																//		BtnNewQuestionAnswer.Attributes.Add("data-toggle", "modal");
																//		BtnNewQuestionAnswer.Attributes.Add("data-target", "#modalQuestionAnswerEdit");
																//	}
																//}
															}
														}
													}
												}
											}

											// Modal for new/edit Questionnaire Questions
											var QuestionModalDiv = MainHDiv.Helpers.DivC("modal fade");
											{

												QuestionModalDiv.Attributes.Add("id", "modalQuestionEdit");
												QuestionModalDiv.Attributes.Add("role", "dialog");
												QuestionModalDiv.Attributes.Add("aria-hidden", "true");
												QuestionModalDiv.Attributes.Add("tabindex", "-1");
												QuestionModalDiv.Attributes.Add("style", "display: none;");
												var QuestionModalDivDialog = QuestionModalDiv.Helpers.DivC("modalQuestionEdit");
												{
													QuestionModalDivDialog.AddClass("modal-dialog");
													var QuestionModalDivDialogContent = QuestionModalDiv.Helpers.DivC("modalQuestionEdit");
													{
														QuestionModalDivDialogContent.AddClass("modal-content animated fadeIn");
														{
															var QuestionModalContentHeader = QuestionModalDivDialogContent.Helpers.DivC("modal-header ");
															{
																var QuestionModalContent = QuestionModalContentHeader.Helpers.With<METTLib.Maintenance.MAQuestionnaireQuestion>(c => c.EditingMAQuestionnaireQuestion);
																{
																	var QuestionHeaderHeading = QuestionModalContent.Helpers.HTML("<h2 class='modal-title headerColor'/>Question</h2>");
																	var QuestionHeaderSmall = QuestionModalContent.Helpers.HTML("<span style='font-size:16px;' data-bind=\"text: $data.Question()\"/></span>");
																}
															}
															var QuestionModalContentBody = QuestionModalDivDialogContent.Helpers.DivC("modal-body m");
															{
																var QuestionModalContentBodyDiv = QuestionModalContentBody.Helpers.DivC("row margin0");
																{
																	var QuestionModalContent = QuestionModalContentBodyDiv.Helpers.With<METTLib.Maintenance.MAQuestionnaireQuestion>(c => c.EditingMAQuestionnaireQuestion);
																	{
																		var QuestionHeading = QuestionModalContent.Helpers.DivC("col-md-12 p-n-lr");
																		{
																			var QuestionLegalHeadingText = QuestionHeading.Helpers.HTML("<div style='font-size:16px;' class='headerColor'/>Question Information</div>");
																		}
																		var Question = QuestionModalContent.Helpers.DivC("col-md-12 p-n-lr");
																		{
																			Question.Helpers.BootstrapEditorRowFor(c => c.Question);
																		}
																		var QuestionTooltip = QuestionModalContent.Helpers.DivC("col-md-9 p-n-lr");
																		{
																			QuestionTooltip.Helpers.BootstrapEditorRowFor(c => c.QuestionTooltip);
																		}
																		var QuestionSpacer = QuestionModalContent.Helpers.DivC("col-md-1 p-n-lr");
																		{
																		}
																		var QuestionSortOrder = QuestionModalContent.Helpers.DivC("col-md-2 p-n-lr");
																		{
																			QuestionSortOrder.Helpers.BootstrapEditorRowFor(c => c.SortOrder);
																		}

																		//Legal Designations and Management Spheres
																		#region Legal Designations Management Spheres
																		var QuestionLegalHeading = QuestionModalContent.Helpers.DivC("col-md-12 p-n-lr");
																		{
																			var QuestionLegalHeadingText = QuestionLegalHeading.Helpers.HTML("<div style='font-size:16px;' class='headerColor'/>Question Legal Designations & Management Spheres</div>");
																		}
																		var QuestionLegalContent = QuestionModalContent.Helpers.DivC("col-md-12 p-n-lr");
																		{
																			var QuestionLegalItems = QuestionModalContentBodyDiv.Helpers.TableFor<METTLib.Maintenance.QuestionnaireQuestionLegalDesignation>((c) => c.EditingMAQuestionnaireQuestion.QuestionnaireQuestionLegalDesignationList, true, true);
																			{
																				QuestionLegalItems.AddClass("table table-striped table-bordered table-hover");
																				var QuestionLegalItemsTableFirstRow = QuestionLegalItems.FirstRow;
																				{
																					var LegalDesignationIdCol = QuestionLegalItemsTableFirstRow.AddColumn(c => c.LegalDesignationID);
																					{
																						LegalDesignationIdCol.Editor.AddBinding(Singular.Web.KnockoutBindingString.enable, c => c.IsNew);
																					}
																				}
																			}
																		}

																		var QuestionSpheresContent = QuestionModalContent.Helpers.DivC("col-md-12 p-n-lr pad-top-15");
																		{
																			var QuestionSphereItems = QuestionModalContentBodyDiv.Helpers.TableFor<METTLib.Maintenance.QuestionnaireQuestionManagementSphere>((c) => c.EditingMAQuestionnaireQuestion.QuestionnaireQuestionManagementSphereList, true, true);
																			{

																				QuestionSphereItems.AddClass("table table-striped table-bordered table-hover");

																				var QuestionSphereItemsTableFirstRow = QuestionSphereItems.FirstRow;
																				{

																					var ManagementSphereIdCol = QuestionSphereItemsTableFirstRow.AddColumn(c => c.ManagementSphereID);
																					{
																						ManagementSphereIdCol.Editor.AddBinding(Singular.Web.KnockoutBindingString.enable, c => c.IsNew);
																					}
																				}
																			}
																		}
																		#endregion

																	}
																}
															}
														}
														var QuestionModalContentFooter = QuestionModalDivDialogContent.Helpers.DivC("modal-footer");
														{
															var QuestionCloseBtn = QuestionModalContentFooter.Helpers.Button("Close", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
															{
																QuestionCloseBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "");
																QuestionCloseBtn.AddClass("btn btn-white");
																QuestionCloseBtn.Attributes.Add("data-dismiss", "modal");
															}
															var QuestionSaveBtn = QuestionModalContentFooter.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
															{
																QuestionSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveQuestionnaireQuestion($data)");
																QuestionSaveBtn.AddClass("btn btn-primary");
															}
														}
													}
												}
											}

											// Modal for new/edit Questionnaire Question Answer Options
											//var QuestionAnswerModalDiv = MainHDiv.Helpers.DivC("container");
											//{
											//	QuestionAnswerModalDiv.AddClass("modal inmodal");
											//	QuestionAnswerModalDiv.Attributes.Add("id", "modalQuestionAnswerEdit");
											//	QuestionAnswerModalDiv.Attributes.Add("role", "dialog");
											//	QuestionAnswerModalDiv.Attributes.Add("aria-hidden", "true");
											//	QuestionAnswerModalDiv.Attributes.Add("tabindex", "-2");
											//	QuestionAnswerModalDiv.Attributes.Add("style", "display: none;");
											//	var QuestionAnswerModalDivDialog = QuestionAnswerModalDiv.Helpers.DivC("modalQuestionAnswerEdit");
											//	{
											//		QuestionAnswerModalDiv.AddClass("modal-dialog");
											//		var QuestionAnswerModalDivDialogContent = QuestionAnswerModalDiv.Helpers.DivC("modalQuestionAnswerEdit");
											//		{
											//			QuestionAnswerModalDivDialogContent.AddClass("modal-content animated fadeIn");
											//			{
											//				var QuestionAnswerModalContentHeader = QuestionAnswerModalDivDialogContent.Helpers.DivC("modal-header ");
											//				{
											//					var QuestionAnswerHeaderHeading = QuestionAnswerModalContentHeader.Helpers.HTML("<h2 class='modal-title headerColor'/>Answer Option</h2>");
											//					var QuestionAnswerHeaderSmall = QuestionAnswerModalContentHeader.Helpers.HTML("<span style='font-size:16px;' />##Question Answer Option Placeholder##</span>");
											//				}
											//				var QuestionAnswerModalContentBody = QuestionAnswerModalDivDialogContent.Helpers.DivC("modal-body m");
											//				{
											//					var QuestionAnswerModalContentBodyDiv = QuestionAnswerModalContentBody.Helpers.DivC("row margin0");
											//					{
											//						var QuestionAnswerHeading = QuestionAnswerModalContentBodyDiv.Helpers.HTML("<div style='font-size:16px;' class='headerColor'/>Answer Information</div>");
											//					}
											//				}
											//			}
											//			var QuestionAnswerModalContentFooter = QuestionAnswerModalDivDialogContent.Helpers.DivC("modal-footer");
											//			{
											//				var QuestionAnswerCloseBtn = QuestionAnswerModalContentFooter.Helpers.Button("Close", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
											//				{
											//					QuestionAnswerCloseBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "");
											//					QuestionAnswerCloseBtn.AddClass("btn btn-white");
											//					QuestionAnswerCloseBtn.Attributes.Add("data-dismiss", "modal");
											//				}
											//				var QuestionAnswerSaveBtn = QuestionAnswerModalContentFooter.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
											//				{
											//					QuestionAnswerSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "");
											//					QuestionAnswerSaveBtn.AddClass("btn btn-primary");
											//				}
											//			}
											//		}
											//	}
											//}
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

	<script type="text/javascript">

		Singular.OnPageLoad(function () {
			Singular.ShowLoadingBar();
			$("[id^=userTabs]").removeClass("active");
			$($("[id^=userTabs]")[0]).addClass("active");
			$("[id^=tab-]").removeClass("active");
			$($("[id^=tab-]")[1]).addClass("active");
			$("[id^='PublishInd']").siblings(".ImgIcon").remove();
			var stepID = 0;
			$("#ul-tab-list li:eq(" + (stepID) + ")").addClass("active").show();
			Singular.HideLoadingBar();
		});

		//Assessment Tab Scripts
		var ShowQuestionnaireGroupData = function (obj) {
			$('ul.nav li').on('click', function () {
				$(this).parent().find('li.active').removeClass('active');
				$(this).addClass('active');
			});

			ViewModel.CallServerMethod('GetSurveyQuestionnaireGroupData', { QGroupID: obj.QuestionnaireGroupID(), QuestionnaireID: ViewModel.paramQuestionnaireId() }, function (result) {
				if (result.Success) {
					ViewModel.MAQuestionnaireQuestionList.Set(result.Data.Item1)
					ViewModel.MAFirstQuestionID(result.Data.Item2)
					METTHelpers.horizontalTabControl("userTabs");
					$("[id^=userTabs]").removeClass("active");
					$($("[id^=userTabs]")[0]).addClass("active");

					$("[id^=tab-]").removeClass("active");
					$($("[id^=tab-]")[1]).addClass("active");
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})
		}

		//Save Questionnaire Overview
		var SaveQuestionnaireOverview = function (obj) {
			QuestionnaireObject.CallServerMethod('SaveQuestionnaireOverview', { QuestionnaireList: ViewModel.QuestionnairesList.Serialise(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					METTHelpers.Notification("Questionnaire Information Successfully Saved", 'center', 'success', 3000);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			});
		}

		//Save Questionnaire Question
		var SaveQuestionnaireQuestion = function (obj) {
			MAQuestionnaireQuestionObject.CallServerMethod('SaveQuestionnaireQuestion', {
				//MAQuestionnaireQuestionList: ViewModel.MAQuestionnaireQuestionList.Serialise(),	ShowLoadingBar: true }, function (result) {
				//MAQuestionnaireQuestion: ViewModel.EditingMAQuestionnaireQuestion.Serialise(), ShowLoadingBar: true
				MAQuestionnaireQuestionList: ViewModel.MAQuestionnaireQuestionList.Serialise(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					METTHelpers.Notification("Questionnaire Question Successfully Saved", 'center', 'success', 3000);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			});
		}

		//Edit Questionnaire Question 
		var ViewQuestion = function (obj) {
			//	ViewModel.AddQuestionInd = false;
			//	ViewModel.Edit.Set(obj);
			//	ViewModel.IsViewingQuestionInd(true);
			ViewModel.CallServerMethod('GetQuestion', { QuestionnaireQuestionID: obj.QuestionnaireQuestionID() }, function (result) {
				if (result.Success) {
					ViewModel.EditingMAQuestionnaireQuestion.Set(result.Data.Item1);
					//	ViewModel.QuestionnaireQuestionManagementSphereList.Set(result.Data.Item2);
					//	ViewModel.QuestionnaireQuestionLegalDesignationsList.Set(result.Data.Item3);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			});
		}

		//Delete Questionnaire Question Answer Option
		//var DeleteQuestionAnswerOption = function (obj) {
		//	METTHelpers.QuestionDialogYesNo("Are you sure you would like to delete this answer option?", 'center',
		//		function () { // Yes 
		//			// Code below giving error when trying to remove object and then call save function
		//			//ViewModel.MAQuestionnaireQuestionList.RemoveNoCheck(obj);
		//			//SaveQuestionnaireQuestion(obj);
		//			MAQuestionnaireQuestionObject.CallServerMethod('DeleteQuestionnaireQuestionAnswerOption', { QuestionnaireQuestionAnswerOptionID: obj.QuestionnaireQuestionAnswerOptionID(), ShowLoadingBar: true }, function (result) {
		//				if (result.Success) {
		//					METTHelpers.Notification("Questionnaire Question Answer Option Successfully Removed", 'center', 'success', 3000);
		//				}
		//				else {
		//					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
		//				}
		//			});
		//		},
		//		function () { // No
		//		})
		//}

		//Edit Questionnaire Question Answer Option
		//var ViewQuestionAnswerOption = function (obj) {

		//}

	</script>

</asp:Content>
