<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportData.aspx.cs" Inherits="METTWeb.Reports.ReportData" %>

<%@ Import Namespace="Singular.Web" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>Report Data</title>

  <%= Singular.Web.Scripts.Scripts.RenderLibraryScripts()%>
  <%= Singular.Web.CSSFile.RenderLibraryStyles()%>
  <%= ViewModel.IncludeGridReportResources()%>
</head>
<body>
  <form id="Form1" runat="server">

    <%
			using (var h = Helpers)
			{
        var GridReport = new Singular.Web.CustomControls.SGrid.GridReportContainer();
				h.Control(GridReport);
			}
			%>

    <asp:ScriptManager ID="SCMMain" runat="server" />
    <singular:PageModelRenderer ID="pmrMain" runat="server" />
        
  </form>
</body>

  <script type="text/javascript">

  	//var ViewChart = function () {
  	//  Singular.Web.SGrid.SGridInfo.FetchData();
  	//  Singular.Web.SGrid.SGridInfo.ShowChart();
  	//}

  </script>

</html>
