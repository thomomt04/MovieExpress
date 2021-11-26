<%@ Page Title="Error..." Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeBehind="ErrorPage.aspx.cs" Inherits="MEWeb.ErrorPage" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

		<link href="Theme/Singular/METTCustomCss/home.css" rel="stylesheet" />
	<link href="Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
	<link href="Theme/Singular/css/badges.css" rel="stylesheet" />
	<link href="Theme/Singular/css/assessment.css" rel="stylesheet" />
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
								var PageNotFoundText = PanelContainerRowCol.Helpers.DivC("");
								{
									PageNotFoundText.Helpers.HTML("<h2>Page Error</h2>");
									PageNotFoundText.Helpers.HTML("<p>An error has occured. Please try again or contact your system administrator.</p>");

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
