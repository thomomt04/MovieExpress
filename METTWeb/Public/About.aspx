<%@ Page Title="METT - About Us" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
   CodeBehind ="About.aspx.cs" Inherits="MEWeb.Public.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/organisationprofile.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
		//	h.HTML().Heading2("About");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<%	using (var h = this.Helpers)
	{
		var MainHDiv = h.DivC("row p-h-xs");
		{
		}
		var rowDiv = MainHDiv.Helpers.DivC("row");
		{
			var gridDivMain = rowDiv.Helpers.DivC("col-lg-12 paddingTop15 ");
			{
				var cardDiv = gridDivMain.Helpers.DivC("ibox float-e-margins paddingBottom");
				{
					var cardTitleDiv = cardDiv.Helpers.DivC("ibox-title");
					{
						cardTitleDiv.Helpers.HTML("<i class='fa fa-user fa-lg fa-fw pull-left'></i>");
						cardTitleDiv.Helpers.HTML().Heading5("About");
					}
					var cardToolsDiv = cardTitleDiv.Helpers.DivC("ibox-tools");
					{
						var aToolsTag = cardToolsDiv.Helpers.HTMLTag("a");
						aToolsTag.AddClass("collapse-link");
						{
							var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
							iToolsTag.AddClass("fa fa-chevron-up");
						}
					}
					var cardContentDiv = cardDiv.Helpers.DivC("ibox-content");
					{
						cardContentDiv.Helpers.HTML("<h2>Popcorn</h2>");
						cardContentDiv.Helpers.HTML("Version 1.0.0");
					}
				}
			}
		}
	}
		%>
<script type="text/javascript">

	Singular.OnPageLoad(function () {
		$("#menuItem6").addClass("active");
	});
	
</script>

</asp:Content>


