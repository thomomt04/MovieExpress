<%@ Page Title="System Settings" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="SystemSettings.aspx.cs" Inherits="METTWeb.Maintenance.SystemSettings" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/Maintenance/systemSettings.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			//h.HTML().Heading2("System Settings");
		}
	%>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<%
	using (var h = Helpers)
	{

			h.Control(new Singular.Web.SystemSettings.SystemSettingsControl(true));
		
	}
%>
  <script type="text/javascript">

		Singular.OnPageLoad(function () {
			$("#menuItem5").addClass("active");
			$("#menuItem5 > ul").addClass("in");

			$("#menuItem5ChildItem4").addClass("subActive");

		});

		$("div.DivToolbar > h2").css("display", "none");

		$("table").removeClass("Grid").addClass("table-responsive table table-striped table-bordered table-hover Grid SUI-RuleBorder");
	</script>
</asp:Content>
