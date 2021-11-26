<%@ Page Title="Error..." Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
	CodeBehind="PageNotFound.aspx.cs" Inherits="METTWeb.PageNotFound" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

	<link href="Theme/Singular/METTCustomCss/home.css" rel="stylesheet" />
	<link href="Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
	<link href="Theme/Singular/css/badges.css" rel="stylesheet" />
	<link href="Theme/Singular/css/assessment.css" rel="stylesheet" />

	<%
		using (var h = this.Helpers)
		{
			//h.HTML().Heading2("Page Not Found");
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
						var PanelContainerRowCol = MainHDiv.Helpers.DivC("col-md-12 ");
						{
							var PageNotFoundText = PanelContainerRowCol.Helpers.DivC("");
							{
								PageNotFoundText.Helpers.HTML("<h2>Error 404 - Page Not Found</h2>");
								PageNotFoundText.Helpers.HTML("<p>The page you were looking for wasn't found.</p>");

							}

						}
					}
				}
			}
		}
	%>

	<%--	<script type="text/javascript">

		$(document).ready(function () {

			window.location = "Account/Home.aspx";

		});

	</script>--%>
</asp:Content>
