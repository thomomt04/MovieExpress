<%@ Page Title="Assessment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Assessment.aspx.cs" Inherits="METTWeb.Assessment.Assessment" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			h.HTML().Heading2("METT Assessment");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<%	using (var h = this.Helpers)
		{

			var MainHDiv = h.DivC("row");
			{
				var QuestionnaireGroupContainer = MainHDiv.Helpers.DivC("container");
				{
					var QuestionnaireGroupBreadcrumbRow = QuestionnaireGroupContainer.Helpers.DivC("row");
					{
						var BreadCrumbDiv = MainHDiv.Helpers.DivC("col-md-12");
						{
							#region Breadcrumb
							var WizardRow = BreadCrumbDiv.Helpers.DivC("row");
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
												//For Each handling each questionnaire group and creating a button for it
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
						}
					}

				}
				#region AssessmentSurvey
				{
					var QuestionnaireGroupContext = h.DivC("row");
					{

						var TabContainers = QuestionnaireGroupContext.Helpers.DivC("ibox-content ibox-contentOrganisationUser padding0");
						{
							{
								var TabContainer = TabContainers.Helpers.DivC("tabs-container");
								{
									var TabContentLeft = TabContainer.Helpers.DivC("tabs-left userTabs");
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
											var TabPaneBody = TabPane.Helpers.DivC("panel-body");
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
													}
												}

												var TabQuestions = TabPaneBody.Helpers.DivC("");
												{
													var QuestionnaireQuestionAnswerGroup = TabQuestions.Helpers.ForEach<METTLib.QuestionnaireSurvey.QuestionnaireQuestionType>(c => c.QuestionnaireQuestionTypeList);
													{
														QuestionnaireQuestionAnswerGroup.Helpers.HTML("<div class='col-md-12 pad-top-5'>");

														var QuestionAnswerBtn = QuestionnaireQuestionAnswerGroup.Helpers.Button("", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);

														QuestionAnswerBtn.AddClass("fa ");
														QuestionAnswerBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "UpdateQACheck($data)");
														//QuestionAnswerBtn.AddBinding(Singular.Web.KnockoutBindingString.css, "{'fa-dot-circle-o': $data.IsSelectedInd() == true, 'fa-circle-o': $data.AnswerOption() != 'Some but not all properties managed as part of the site have been declared.'}"); // , 'fa fa-circle-o': obj.IsSelectedInd() == false} 
														QuestionAnswerBtn.AddBinding(Singular.Web.KnockoutBindingString.css, "{'fa-dot-circle-o btn-bg-color-green':UpdateQA($data), 'fa-circle-o': !UpdateQA($data)}"); // , 'fa fa-circle-o': obj.IsSelectedInd() == false} 

														//var QuestionAnswersColRight = QuestionAnswersRow.Helpers.DivC("col-md-8"); + c.IsSelectedInd AnswerOption
														var QuestionAnswerText = QuestionnaireQuestionAnswerGroup.Helpers.ReadOnlyFor(c => c.AnswerOption);

														QuestionnaireQuestionAnswerGroup.Helpers.HTML("</div>");
													}
												}

												var TabComments = TabPaneBody.Helpers.DivC("");
												{
													TabComments.Helpers.HTML("<div class='col-md-12 pad-top-5'>");
													TabComments.Helpers.HTML("<h4>Comments</h4>");
													TabComments.Helpers.HTML("<textarea placeholder='Write comments here...' style='width: 100%;'></textarea>");
													TabComments.Helpers.HTML("</div>");
												}

												var TabnextSteps = TabPaneBody.Helpers.DivC("");
												{
													TabnextSteps.Helpers.HTML("<div class='col-md-6 pad-top-5'>");
													TabnextSteps.Helpers.HTML("<h4>Next Steps</h4>");
													TabnextSteps.Helpers.HTML("<textarea placeholder='Write next steps here...' style='width: 100%;'></textarea>");
													TabnextSteps.Helpers.HTML("</div>");
												}

												var TabEvidence = TabPaneBody.Helpers.DivC("");
												{
													TabEvidence.Helpers.HTML("<div class='col-md-6 pad-top-5'>");
													TabEvidence.Helpers.HTML("<h4>Evidence</h4>");
													TabEvidence.Helpers.HTML("<textarea placeholder='Write evidence here...' style='width: 100%;'></textarea>");
													TabEvidence.Helpers.HTML("</div>");
												}

												var TabOptions = TabPaneBody.Helpers.DivC("");
												{
													var BtnPreviousCol = TabOptions.Helpers.DivC("col-md-12  pad-top-15");
													var BtnPrev = BtnPreviousCol.Helpers.Button("Previous", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.back);
													{
														BtnPrev.AddClass("btn-primary btn btn btn-outline");
														BtnPrev.AddBinding(Singular.Web.KnockoutBindingString.click, "QuestionStepBack($data)");
													}
													var BtnSave = BtnPreviousCol.Helpers.Button("Save and Next", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.forward);
													{
														BtnSave.AddClass("btn-primary btn btn btn-primary pad-left-15");
														BtnSave.AddBinding(Singular.Web.KnockoutBindingString.click, "QuestionStepNext($data)");
													}
												}
											}

											//Adjust spacing at bottom of page
											var SpacerRow = TabContainers.Helpers.DivC("row pad-top-20");
											{
											}

										}
									}
								}
							}
						}
					}
				}
				#endregion
			}
		}
	%>

	<script type="text/javascript">

		Singular.OnPageLoad(function () {
			$("#menuItem3").addClass("active");
			$("#menuItem3 > ul").addClass("in");

			$("#menuItem3ChildItem2").addClass("subActive");


			METTHelpers.horizontalTabControl("userTabs");
			METTHelpers.SideMenuActive(METTHelpers.SplitURLVars(window.location.href)[0]);

		})

		function QuestionStepBack(obj) {
			//Indicators Items 
			var $list = $("#navigation li");
			var idx = $("#navigation .active")
				.removeClass("li active")
				.prev()
				.index();
			if (idx == 0) idx = 0;
			$list.eq(idx).addClass("active");
			console.log(idx);

			//Tab Panel Items
			var $listtp = $("#tab-content div");
			var prevElem = $("#tab-content .active").prev();
			var tpidx = $("#tab-content .active")
				.removeClass("active")
				.next()
				.index();
			if (tpidx == -1) tpidx = 0;
			prevElem.addClass("active");

		}

		function QuestionStepNext(obj) {
			//Indicators Items 
			var $list = $("#navigation li");
			var idx = $("#navigation .active")
				.removeClass("active")
				.next()
				.index();
			if (idx == -1) idx = 0;
			$list.eq(idx).addClass("active");
			//console.log(idx);
			
			//Tab Panel Items
			var $listtp = $("#tab-content div");
			var nextElem = $("#tab-content .active").next();
			var tpidx = $("#tab-content .active")
				.removeClass("active")
				.next()
				.index();
			if (tpidx == -1) tpidx = 0;
			nextElem.addClass("active");

		}

		var UpdateQA = function (obj) {
			return obj.IsSelectedInd();
		}

		var UpdateQACheck = function (obj) {
			obj.IsSelectedInd(!obj.IsSelectedInd());
			//loop thru set inactive
		}

		var ShowQuestionnaireGroupData = function (obj) {
			ViewModel.CallServerMethod('GetSurveyQuestionnaireGroupData', { QGroupID: obj.QuestionnaireGroupID() }, function (result) {
				if (result.Success) {
					ViewModel.QuestionnaireQuestionList.Set(result.Data.Item1)
					ViewModel.FirstQuestionID(result.Data.Item2)
					METTHelpers.horizontalTabControl("userTabs");
				}
				else {
					//show error
				}
			})
		}

	</script>

</asp:Content>



