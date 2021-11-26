<%@ Page Title="METT Assessment Wizard Example" Language="C#" AutoEventWireup="true" CodeBehind="Wizard.aspx.cs" MasterPageFile="~/Site.Master" Inherits="METTWeb.TestPages.Wizard" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			h.HTML().Heading2("");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

	<%	
		using (var h = this.Helpers)
		{
			var MainHDiv = h.DivC("row");
			{
				var PanelContainer = MainHDiv.Helpers.DivC("container");
				{
					var PanelContainerRow = PanelContainer.Helpers.DivC("row");
					{
						var PanelContainerRowCol = MainHDiv.Helpers.DivC("col-md-12");
						{
							var PanelContainerBox = PanelContainerRowCol.Helpers.DivC("ibox float-e-margins paddingBottom");
							{
								var PanelContainerBoxTitle = PanelContainerBox.Helpers.DivC("ibox-title");
								{
									PanelContainerBoxTitle.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
									PanelContainerBoxTitle.Helpers.HTML().Heading5("Wizard Example");
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
									var ContentRow = PanelContainerContentBox.Helpers.DivC("row");
									{

										#region Wizard
										var WizardDiv = ContentRow.Helpers.DivC("wizard");
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
																WizardIcon.AddBinding(Singular.Web.KnockoutBindingString.click, "wizardClick($data)");

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
										#endregion

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

		});

		var wizardClick = function (obj) {
			$('ul.nav li').on('click', function () {
				$(this).parent().find('li.active').removeClass('active');
				$(this).addClass('active');
			});
		}

	</script>


</asp:Content>
