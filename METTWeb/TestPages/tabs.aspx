<%@ Page Title="METT Assessment" Language="C#" AutoEventWireup="true" CodeBehind="tabs.aspx.cs" MasterPageFile="~/Site.Master" Inherits="METTWeb.TestPages.tabs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
<%--	<link href="../Theme/Singular/METTCustomCss/organisationprofile.css" rel="stylesheet" />--%>

	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />



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
			var MainHDiv = h.DivC("row pad-top-10");
			{
				var QuestionnaireGroupContainer = MainHDiv.Helpers.DivC("container");
				{
					var QuestionnaireGroupBreadcrumbRow = QuestionnaireGroupContainer.Helpers.DivC("row");
					{
						var QuestionnaireContentDiv = QuestionnaireGroupBreadcrumbRow.Helpers.DivC("col-md-12 p-n-lr");
						{

							var TestDiv = QuestionnaireContentDiv.Helpers.Div();
							{
								var QuestionnaireContentDivRow = TestDiv.Helpers.DivC("row");

								{

									var QuestionnaireContentDivRowCol = QuestionnaireContentDivRow.Helpers.DivC("col-md-12 p-n-lr");
									{

										var TabContainer = QuestionnaireContentDivRowCol.Helpers.DivC("tabs-container");
										{
											var EntityTab = TabContainer.Helpers.TabControl(c => ViewModel.SelectedTab);
											TabContainer.Attributes.Add("style", "padding-bottom: 50;");
											{
												EntityTab.Style.ClearBoth();
												EntityTab.AddClass("nav nav-tabs");


												#region Tab1
												var OverviewTab = EntityTab.AddTab("Tab One");
												{
													var OverviewRow = OverviewTab.Helpers.DivC("");
													{
														var OverviewText = OverviewRow.Helpers.DivC("row margin0");
														{
															var CardHeaderCol = OverviewText.Helpers.DivC("col-lg-12");
															{
																var CardHeaderColContainerBox = CardHeaderCol.Helpers.DivC("ibox float-e-margins paddingBottom");
																{
																	var CardHeaderColContainerContentBox = CardHeaderColContainerBox.Helpers.DivC("ibox-content");
																	{
																		var CardHeaderColContainerRowInner0 = CardHeaderColContainerContentBox.Helpers.DivC("row");
																		{
																			var CardHeaderColContainerRowInnerCol1 = CardHeaderColContainerRowInner0.Helpers.DivC("col-lg-12");
																			{
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																			}
																		}
																	}
																}
															}
														}


														var OverviewText2 = OverviewRow.Helpers.DivC("row margin0");
														{
															var CardHeaderCol = OverviewText2.Helpers.DivC("col-lg-12");
															{
																var CardHeaderColContainerBox = CardHeaderCol.Helpers.DivC("ibox float-e-margins paddingBottom");
																{
																	var CardHeaderColContainerContentBox = CardHeaderColContainerBox.Helpers.DivC("ibox-content");
																	{
																		var CardHeaderColContainerRowInner0 = CardHeaderColContainerContentBox.Helpers.DivC("row");
																		{
																			var CardHeaderColContainerRowInnerCol1 = CardHeaderColContainerRowInner0.Helpers.DivC("col-lg-12");
																			{
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																			}
																		}
																	}
																}
															}
														}
													}
												}
												#endregion

												#region Tab2
												var OverviewTab2 = EntityTab.AddTab("Tab Two");
												{
													var OverviewRow = OverviewTab2.Helpers.DivC("");
													{
														var OverviewText = OverviewRow.Helpers.DivC("row margin0");
														{
															var CardHeaderCol = OverviewRow.Helpers.DivC("col-lg-12 p-n-lr");
															{
																var CardHeaderColContainerBox = CardHeaderCol.Helpers.DivC("ibox float-e-margins paddingBottom");
																{
																	var CardHeaderColContainerContentBox = CardHeaderColContainerBox.Helpers.DivC("ibox-content");
																	{
																		var CardHeaderColContainerRowInner0 = CardHeaderColContainerContentBox.Helpers.DivC("row");
																		{
																			var CardHeaderColContainerRowInnerCol1 = CardHeaderColContainerRowInner0.Helpers.DivC("col-lg-12");
																			{
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				CardHeaderColContainerRowInnerCol1.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");

																			}
																		}
																	}
																}
															}
														}
													}
												}
												#endregion


												var ThreatsTab = EntityTab.AddTab("Threats");
												{
													var SelectedThreatsDiv = ThreatsTab.Helpers.DivC("ibox float-e-margins paddingBottom");
													{
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
															var SelectedThreatsDivContentDivRow = SelectedThreatsDivContentDiv.Helpers.DivC("row");
															{


																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");
																				SelectedThreatsDivContentDivRow.Helpers.HTML("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis ac ipsum a aliquam. Duis eget tortor in augue ornare pharetra vitae nec nisi. Aenean bibendum elit sit amet dolor consectetur iaculis. Ut aliquet aliquam molestie. Sed tristique metus dui, in congue lacus mattis congue. Suspendisse vulputate enim in quam luctus placerat. Vivamus placerat vestibulum magna, ac elementum diam imperdiet in. Nullam viverra eros id diam lobortis eleifend. Sed lacinia et purus ut aliquet. Quisque luctus magna ultricies mi iaculis ultricies. Duis fringilla, purus id consectetur congue, lorem ipsum placerat risus, vulputate dapibus nibh urna vitae justo.");

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
			ViewModel.SelectedTab(selectedTab);
		});

	</script>



</asp:Content>
