<%@ Page Title="Reports" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
   CodeBehind ="Reports.aspx.cs" Inherits="METTWeb.Maintenance.Reports" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/Maintenance/Reports.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			h.HTML().Heading2("Reports");
		}
	%>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<%
  using (var h = Helpers)
  {
    h.Control(new Singular.Web.Reporting.ReportStateControl());
  }
%>

  <script type="text/javascript">
		Singular.OnPageLoad(function () {
			$("#menuItem4").addClass("active");
			$("#menuItem4 > ul").addClass("in");

			$("#menuItem4ChildItem2").addClass("subActive");

		});

		$("table").removeClass("Grid").addClass("table-responsive table table-striped table-bordered table-hover Grid SUI-RuleBorder");
	</script>


</asp:Content>
