<%@ Page Title="Panels Example" Language="C#" AutoEventWireup="true" CodeBehind="Panels.aspx.cs" Inherits="METTWeb.TestPages.Panels" MasterPageFile="~/Site.Master" %>

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
			var MainRow = h.DivC("row");
			{

				var MainContainer = MainRow.Helpers.DivC(""); 
				{
					var MainContainerRow = MainContainer.Helpers.DivC("row");
					{
						var MainContainerRowCol = MainContainerRow.Helpers.DivC("col-md-12");
						{

							var cPanel = MainContainerRowCol.Helpers.DivC("ibox float-e-margins");
							{
								var cPanelTitle = cPanel.Helpers.DivC("ibox-title");
								{
									cPanelTitle.Helpers.HTML("<i class='fa fa-coffee fa-lg fa-fw pull-left'></i>");
									cPanelTitle.Helpers.HTML().Heading5("Collapsable Panel Heading");
								}
								var cPanelTools = cPanelTitle.Helpers.DivC("ibox-tools");
								{
									var cPanelToolsTag = cPanelTools.Helpers.HTMLTag("a");
									cPanelToolsTag.AddClass("collapse-link");
									{
										var cPanelToolsTagIcon = cPanelToolsTag.Helpers.HTMLTag("i");
										cPanelToolsTagIcon.AddClass("fa fa-chevron-up");
									}
								}
								var cPanelContent = cPanel.Helpers.DivC("ibox-content");
								{
									var cPanelMainContent = cPanelContent.Helpers.Div();
									{
										var MainContainerRowx = cPanelMainContent.Helpers.DivC("row");
										{
											var MainContainerRowColx = MainContainerRowx.Helpers.DivC("col-md-12");
											{

												var cPanel2 = MainContainerRowColx.Helpers.DivC("ibox float-e-margins");
												{
													var cPanelTitle2 = cPanel2.Helpers.DivC("ibox-title");
													{
														cPanelTitle2.Helpers.HTML("<i class='fa fa-coffee fa-lg fa-fw pull-left'></i>");
														cPanelTitle2.Helpers.HTML().Heading5("Collapsable Panel Heading");
													}
													var cPanelTools2 = cPanelTitle2.Helpers.DivC("ibox-tools");
													{
														var cPanelToolsTag2 = cPanelTools2.Helpers.HTMLTag("a");
														cPanelToolsTag2.AddClass("collapse-link");
														{
															var cPanelToolsTagIcon2 = cPanelToolsTag2.Helpers.HTMLTag("i");
															cPanelToolsTagIcon2.AddClass("fa fa-chevron-up");
														}
													}
													var cPanelContent2 = cPanel2.Helpers.DivC("ibox-content");
													{
														var cPanelMainContent2 = cPanelContent2.Helpers.Div();
														{

															var MainContainerRowx2 = cPanelMainContent2.Helpers.DivC("row");
															{
																var MainContainerRowColx2 = MainContainerRowx.Helpers.DivC("col-md-12");
																{
																	//CONTENT GOES HERE
																}
															}
														}
													}
												}

												var cPanel3 = MainContainerRowColx.Helpers.DivC("ibox float-e-margins");
												{
													var cPanelTitle3 = cPanel3.Helpers.DivC("ibox-title");
													{
														cPanelTitle3.Helpers.HTML("<i class='fa fa-coffee fa-lg fa-fw pull-left'></i>");
														cPanelTitle3.Helpers.HTML().Heading5("Collapsable Panel Heading");
													}
													var cPanelTools3 = cPanelTitle3.Helpers.DivC("ibox-tools");
													{
														var cPanelToolsTag3 = cPanelTools3.Helpers.HTMLTag("a");
														cPanelToolsTag3.AddClass("collapse-link");
														{
															var cPanelToolsTagIcon3 = cPanelToolsTag3.Helpers.HTMLTag("i");
															cPanelToolsTagIcon3.AddClass("fa fa-chevron-up");
														}
													}
													var cPanelContent3 = cPanel3.Helpers.DivC("ibox-content");
													{
														var cPanelMainContent3 = cPanelContent3.Helpers.Div();
														{

															var MainContainerRowx3 = cPanelMainContent3.Helpers.DivC("row");
															{
																var MainContainerRowColx3 = MainContainerRowx.Helpers.DivC("col-md-12");
																{
																	//CONTENT GOES HERE
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

							var cPanel4 = MainContainerRowCol.Helpers.DivC("ibox float-e-margins");
							{
								var cPanelTitle4 = cPanel4.Helpers.DivC("ibox-title");
								{
									cPanelTitle4.Helpers.HTML("<i class='fa fa-coffee fa-lg fa-fw pull-left'></i>");
									cPanelTitle4.Helpers.HTML().Heading5("Collapsable Panel Heading");
								}
								var cPanelTools4 = cPanelTitle4.Helpers.DivC("ibox-tools");
								{
									var cPanelToolsTag4 = cPanelTools4.Helpers.HTMLTag("a");
									cPanelToolsTag4.AddClass("collapse-link");
									{
										var cPanelToolsTagIcon4 = cPanelToolsTag4.Helpers.HTMLTag("i");
										cPanelToolsTagIcon4.AddClass("fa fa-chevron-up");
									}
								}
								var cPanelContent4 = cPanel4.Helpers.DivC("ibox-content");
								{
									var cPanelMainContent4 = cPanelContent4.Helpers.Div();
									{

										var MainContainerRowx4 = cPanelMainContent4.Helpers.DivC("row");
										{
											var MainContainerRowColx4 = MainContainerRowx4.Helpers.DivC("col-md-12");
											{
												//CONTENT GOES HERE
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

		});

	</script>

</asp:Content>
