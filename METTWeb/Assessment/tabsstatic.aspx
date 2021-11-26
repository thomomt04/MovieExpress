<%@ Page Title="Assessment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="tabsstatic.aspx.cs" Inherits="METTWeb.Assessment.tabsstatic" %>

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
							var ProtectedAreaTabDiv = BreadCrumbDiv.Helpers.DivC("row margin0");

							var ProtectedAreaTabContainer = ProtectedAreaTabDiv.Helpers.DivC("tabs-container");
							{
								var TabContentLeft = ProtectedAreaTabContainer.Helpers.DivC("tabs-left userTabs");
								{
									var TabList = TabContentLeft.Helpers.HTMLTag("ul");
									{
										TabList.AddClass("nav nav-tabs");
										TabList.Attributes.Add("id", "navigation");

										var UserTabItem = TabList.Helpers.HTMLTag("li");
										{
											UserTabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabsUsers");
											UserTabItem.Attributes.Add("id", "userTabsUsers");
														UserTabItem.Attributes.Add("class", "active");
											var TabItemLink = UserTabItem.Helpers.HTMLTag("a");
											{
												TabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => "Associated Users");

												TabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-Users");
												TabItemLink.Attributes.Add("data-toggle", "tab");
											}
										}

										var BiodiversityTabItem = TabList.Helpers.HTMLTag("li");
										{
											BiodiversityTabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabsBiodiversity");
											BiodiversityTabItem.Attributes.Add("id", "userTabsBiodiversity");
											var BiodiversityTabItemLink = BiodiversityTabItem.Helpers.HTMLTag("a");
											{
												BiodiversityTabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => "Primary Biodiversity And Cultural Attributes");

												BiodiversityTabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-Biodiversity");
												BiodiversityTabItemLink.Attributes.Add("data-toggle", "tab");
											}
										}

										var ObjectivesTabItem = TabList.Helpers.HTMLTag("li");
										{
											ObjectivesTabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabsObjectives");
											ObjectivesTabItem.Attributes.Add("id", "userTabsObjectives");
											var ObjectivesTabItemLink = ObjectivesTabItem.Helpers.HTMLTag("a");
											{
												ObjectivesTabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => "High Level Site Objectives");

												ObjectivesTabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-Objectives");
												ObjectivesTabItemLink.Attributes.Add("data-toggle", "tab");
											}
										}
									}

									var TabItemsContents = TabContentLeft.Helpers.DivC("tab-content");
									{
										TabItemsContents.Attributes.Add("id", "tab-content");
									}
									var TabPane = TabItemsContents.Helpers.DivC("tab-pane");
									TabPane.AddBinding(Singular.Web.KnockoutBindingString.id, "tab-Users");
									TabPane.Attributes.Add("id", "tab-Users");
									TabPane.Attributes.Add("class", "tab-pane active");
									TabPane.AddBinding(Singular.Web.KnockoutBindingString.css, "{'active': ViewModel.ActiveTab() == 'Users'}");
									var TabPaneBody = TabPane.Helpers.DivC("panel-body panelRaiseShadow");
									{
										var TabPaneTitle = TabPaneBody.Helpers.DivC("ibox-title ibox-titleContainer");
										{
											TabPaneTitle.Helpers.HTML("<i class='fa fa-users fa-lg fa-fw pull-left'></i>");
											TabPaneTitle.Helpers.HTML().Heading5("Associated Users");
										}
										var TabUsers = TabPaneBody.Helpers.DivC("");
										{
												TabUsers.Helpers.HTML().Heading3("Content goes here 1 ");
										}
									}

									var TabPane1 = TabItemsContents.Helpers.DivC("tab-pane");
									TabPane1.AddBinding(Singular.Web.KnockoutBindingString.id, "tab-Biodiversity");
									TabPane1.Attributes.Add("id", "tab-Biodiversity");
									var TabPaneBody1 = TabPane1.Helpers.DivC("panel-body panelRaiseShadow");
									{
										var TabPaneTitle = TabPaneBody1.Helpers.DivC("ibox-title ibox-titleContainer");
										{
											TabPaneTitle.Helpers.HTML("<i class='fa fa-paw fa-lg fa-fw pull-left'></i>");
											TabPaneTitle.Helpers.HTML().Heading5("Heading 2");
										}
										var TabBiodiversity = TabPaneBody1.Helpers.DivC("");
										{
											TabBiodiversity.Helpers.HTML().Heading3("Content goes here 2");
										}
									}

									var TabPane2 = TabItemsContents.Helpers.DivC("tab-pane");
									TabPane2.AddBinding(Singular.Web.KnockoutBindingString.id, "tab-Objectives");
									TabPane2.Attributes.Add("id", "tab-Objectives");

									var TabPaneBody2 = TabPane2.Helpers.DivC("panel-body panelRaiseShadow");
									{
										var TabPaneTitle = TabPaneBody2.Helpers.DivC("ibox-title ibox-titleContainer");
										{
											TabPaneTitle.Helpers.HTML("<i class='fa fa-paw fa-lg fa-fw pull-left'></i>");
											TabPaneTitle.Helpers.HTML().Heading5("Heading 3");
										}
										var TabObjectives = TabPaneBody2.Helpers.DivC("");
										{
											TabObjectives.Helpers.HTML().Heading3("Content goes here 3");
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


			METTHelpers.horizontalTabControl("userTabs");
			METTHelpers.SideMenuActive(METTHelpers.SplitURLVars(window.location.href)[0]);

		})







	</script>

</asp:Content>
