<%@ Page Title="Reports" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Reports.aspx.cs" Inherits="MEWeb.Reports.Reports" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<%--	<link href="../Theme/Singular/METTCustomCss/customstyles.css" rel="stylesheet" />--%>
	<link href="../Theme/Singular/METTCustomCss/Maintenance/reports.css" rel="stylesheet" />
	<link href="../Theme/Singular/METTCustomCss/organisationprofile.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			//h.HTML().Heading2("Reports");
			var MainHDiv = h.DivC("row p-h-xs");
			{
			}
		}
	%>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<div class="panel">
		<% 
			using (var h = Helpers)
			{
				//h.Control(new METTWeb.Reports.METTReportStateControl());
				h.Control(new Singular.Web.Reporting.ReportStateControl());
			}

		%>
	</div>
	<script type="text/javascript">

		Singular.OnPageLoad(function () {
			$("#menuItem4").addClass("active");
			$("#menuItem4 > ul").addClass("in");

			//$("#ReportsMenu").addClass("active");
			var listOfLinks = $('.ReportSection > div > ul > li').addClass("PaddingLi");

			for (var k = 0; k <= listOfLinks.length - 1; k++) {

				switch (k) {

					case 0:
						listOfLinks[k].innerHTML = '<i class="fa fa-bar-chart MarginRight5"></i>' + listOfLinks[k].innerHTML;
						break;

					case 1:
						listOfLinks[k].innerHTML = '<i class="fa fa-bar-chart MarginRight5"></i>' + listOfLinks[k].innerHTML;
						break;

					case 2:
						listOfLinks[k].innerHTML = '<i class="fa fa-bar-chart MarginRight5"></i>' + listOfLinks[k].innerHTML;
						break;

					case 3:
						listOfLinks[k].innerHTML = '<i class="fa fa-bar-chart MarginRight5"></i>' + listOfLinks[k].innerHTML;
						break;

					case 4:
						listOfLinks[k].innerHTML = '<i class="fa fa-bar-chart MarginRight5"></i>' + listOfLinks[k].innerHTML;
						break;

					case 5:
						listOfLinks[k].innerHTML = '<i class="fa fa-bar-chart MarginRight5"></i>' + listOfLinks[k].innerHTML;
						break;

					default:
						listOfLinks[k].innerHTML = '<i class="fa fa-bar-chart MarginRight5"></i>' + listOfLinks[k].innerHTML;
						break;
				}
			}
			$(".NumericEditor").val("");
		});

		$(".ReportSection").on("mouseenter", function () {
			$(this).children('.Heading').animate({ backgroundColor: "" }, { duration: 100, queue: false });
		});

		$(".ReportSection").on("mouseleave", function () {
			$(this).children('.Heading').animate({ backgroundColor: "" }, { duration: 100, queue: false });
		});
	</script>


</asp:Content>
