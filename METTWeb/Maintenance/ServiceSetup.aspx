<%@ Page Title="Service Setup" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
   CodeBehind ="ServiceSetup.aspx.cs" Inherits="METTWeb.Maintenance.ServiceSetup" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/organisationprofile.css" rel="stylesheet" />
	<link href="../Theme/Singular/METTCustomCss/Maintenance/maintenance.css" rel="stylesheet" />
	<link href="../Theme/Singular/METTCustomCss/Maintenance/servicesetup.css" rel="stylesheet" />

	<style>

	</style>
</asp:Content>
<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
		//	h.HTML().Heading2("Service Setup");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<%
  using (var h = Helpers)
  {
    h.Control(new Singular.Web.ServiceHelpers.ServiceSetup());
  }
%>
  <script type="text/javascript">
			Singular.OnPageLoad(function () {
				$("#menuItem5").addClass("active");
				$("#menuItem5 > ul").addClass("in");

				$("#menuItem5ChildItem5").addClass("subActive");

		});

		$("table").removeClass("Grid").addClass("table-responsive table table-striped table-bordered table-hover Grid SUI-RuleBorder");
		</script>

</asp:Content>