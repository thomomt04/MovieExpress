<%@ Page Title="METT - Questionnaires" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Questionnaires.aspx.cs" Inherits="METTWeb.Maintenance.Questionnaires" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			//	h.HTML().Heading2("Questionnaires");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<%	using (var h = this.Helpers)
		{
			var MainHDiv = h.DivC("row p-h-xs");
			{
				var QuestionnaireGroupContainer = MainHDiv.Helpers.DivC("container");
				{
					var QuestionnaireGroupRow = QuestionnaireGroupContainer.Helpers.DivC("row");
					{
						var QuestionnaireRowCol = MainHDiv.Helpers.DivC("col-md-12  p-n-lr");
						{
							var QuestionnaireContent = QuestionnaireRowCol.Helpers.Div();
							{
								var TabContainer = QuestionnaireContent.Helpers.DivC("tabs-container");
								{
									var EntityTab = TabContainer.Helpers.TabControl();
									{
										EntityTab.Style.ClearBoth();
										EntityTab.AddClass("nav nav-tabs");

										var QuestionnaireTab = EntityTab.AddTab("Questionnaires");
										{
											var ResultsReports1 = QuestionnaireTab.Helpers.DivC("ibox float-e-margins paddingBottom");
											{
												var ResultsReportsTitleDiv = ResultsReports1.Helpers.DivC("ibox-title");
												{
													ResultsReportsTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
													ResultsReportsTitleDiv.Helpers.HTML().Heading5("New Questionnaire");
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
												var ResultsReportsContentDiv = ResultsReports1.Helpers.DivC("ibox-content");
												{
													var ResultsReportsRow3Div = ResultsReportsContentDiv.Helpers.DivC("row margin0");
													{

														var ScoreText = ResultsReportsRow3Div.Helpers.DivC("col-md-12  text-center pad-top-15");
														{
															ScoreText.Helpers.HTML("<h2 style='margin-top:0px'>Questionnaire</h2>");
															ScoreText.Helpers.HTML("<p>When creating a new questionnaire, the current published questionnaire will be duplicated and will be editable until marked as published by the user. To create a new questionnaire, click the <Create Questionnaire> link below. </p>");

															var SaveBtnAudit = ScoreText.Helpers.Button("Create Questionnaire", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
															{
																SaveBtnAudit.AddBinding(Singular.Web.KnockoutBindingString.click, "CreateQuestionnaireAnswerSet()");
																SaveBtnAudit.AddClass("btn btn-primary ");
															}
														}
													}
												}
											}
											var QuestionnaireDiv = QuestionnaireTab.Helpers.DivC("ibox float-e-margins paddingBottom");
											{
												var QuestionnaireTitleDiv = QuestionnaireDiv.Helpers.DivC("ibox-title");
												{
													QuestionnaireTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
													QuestionnaireTitleDiv.Helpers.HTML().Heading5("Questionnaires");
												}
												var QuestionnaireToolsDiv = QuestionnaireTitleDiv.Helpers.DivC("ibox-tools");
												{
													var aQuestionnaireToolsTag = QuestionnaireToolsDiv.Helpers.HTMLTag("a");
													aQuestionnaireToolsTag.AddClass("collapse-link");
													{
														var iQuestionnaireToolsTag = aQuestionnaireToolsTag.Helpers.HTMLTag("i");
														iQuestionnaireToolsTag.AddClass("fa fa-chevron-up");
													}
												}
												var QuestionnaireDivContentDiv = QuestionnaireDiv.Helpers.DivC("ibox-content");
												{
													var QuestionnaireDivContentDivRow = QuestionnaireDivContentDiv.Helpers.DivC("row");
													{
														var QuestionnaireDataDiv = QuestionnaireDivContentDivRow.Helpers.DivC("col-md-12 ");
														{
															var QuestionnaireItems = QuestionnaireDataDiv.Helpers.BootstrapTableFor<METTLib.Questionnaire.Questionnaire>((c) => c.QuestionnaireList, false, false, "");
															{
																var QuestionnaireTableFirstRow = QuestionnaireItems.FirstRow;
																{
																	var QuestionnaireItemNameCol = QuestionnaireTableFirstRow.AddColumn("Questionnaire Name");
																	{
																		QuestionnaireItemNameCol.Attributes.Add("style", "");
																		var QuestionnaireItemNameText = QuestionnaireItemNameCol.Helpers.Span(c => c.QuestionnaireName);
																	}
																	var QuestionnaireItemDescription = QuestionnaireTableFirstRow.AddReadOnlyColumn(c => c.QuestionnaireVersionNumber);
																	{
																		QuestionnaireItemDescription.HeaderText = "Version";
																	}
																	var StartDate = QuestionnaireTableFirstRow.AddReadOnlyColumn(c => c.StartDate);
																	{
																		StartDate.HeaderText = "Start Date";
																	}
																	var EndDate = QuestionnaireTableFirstRow.AddReadOnlyColumn(c => c.EndDate);
																	{
																		EndDate.HeaderText = "End Date";
																	}
																	var PublishedDate = QuestionnaireTableFirstRow.AddReadOnlyColumn(c => c.PublishDateTime);
																	{
																		EndDate.HeaderText = "Published Date";
																	}
																	var PublishedIndCol = QuestionnaireTableFirstRow.AddReadOnlyColumn(c => c.PublishInd);
																	var EditCol = QuestionnaireTableFirstRow.AddColumn("Action");
																	{
																		EditCol.Attributes.Add("style", "width:175px;");
																		var ManageBtn = EditCol.Helpers.Button("Manage", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																		{
																			ManageBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ManageQuestionnaire($data)");
																			ManageBtn.AddClass("btn btn-outline btn-info btn");
																		}
																		var DeleteBtn = EditCol.Helpers.Button("Delete", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
																		{
																			DeleteBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "DeleteQuestionnaire($data)");
																			DeleteBtn.AddClass("btn btn-outline btn-danger btn");
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
							}
						}
					}
				}
			}
		}
	%>

	<script type="text/javascript">
		Singular.OnPageLoad(function () {
			$("#menuItem5").addClass("active");
			$("#menuItem5 > ul").addClass("in");
			$("#menuItem5ChildItem1").addClass("subActive");
		});

		var ManageQuestionnaire = function (obj) {
			ViewModel.CallServerMethod('ManageQuestionnaire', { QuestionnaireID: obj.QuestionnaireID(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					window.location = result.Data;
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})
		}

		var CreateQuestionnaireAnswerSet = function (obj) {
			Singular.ShowLoadingBar;
			METTHelpers.QuestionDialogYesNo("Are you sure you would like to create a new questionnaire answer set?", 'center',
				function () { // Yes
					ViewModel.CallServerMethod('CreateQuestionnaireAnswerSet', { QuestionnaireID: -1, ShowLoadingBar: true }, function (result) {
						if (result.Success) {
							ViewModel.QuestionnaireList.Set(result.Data);
							METTHelpers.Notification("Questionnaire successfully created", 'center', 'success', 5000);
						}
						else {
							METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
						}
					});
				},
				function () { // No
				})
		}

		var DeleteQuestionnaire = function (obj) {
			METTHelpers.QuestionDialogYesNo("Are you sure you would like to remove this questionnaire from the system?", 'center',
				function () { // Yes 
					ViewModel.QuestionnaireList.RemoveNoCheck(obj);
					SaveQuestionnaire(obj);
				},
				function () { // No
				})
		}

		var SaveQuestionnaire = function (obj) {
			QuestionnaireObject.CallServerMethod('SaveQuestionnaireOverview', { QuestionnaireList: ViewModel.QuestionnaireList.Serialise(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					METTHelpers.Notification("Questionnaire set successfully removed", 'center', 'success', 5000);
					ViewModel.QuestionnaireList.Set(result.Data);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			});
		}

		


	</script>

</asp:Content>
