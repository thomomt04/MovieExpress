<%@ Page Title="Security" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
   CodeBehind ="Security.aspx.cs" Inherits="MEWeb.Maintenance.Security" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/Maintenance/security.css" rel="stylesheet" />

</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			//h.HTML().Heading2("Security");
		}
	%>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<%
  using (var h = Helpers)
  {
    h.Control(new Singular.Web.Security.SecurityControl());
  }
%>

  <script type="text/javascript">
		Singular.OnPageLoad(function () {
			$("#menuItem2").addClass("active");
			$("#menuItem2 > ul").addClass("in");

			$("#menuItem5ChildItem3").addClass("subActive");

		});

		$("table").removeClass("Grid").addClass("table-responsive table table-striped table-bordered table-hover Grid SUI-RuleBorder");
	</script>


</asp:Content>
