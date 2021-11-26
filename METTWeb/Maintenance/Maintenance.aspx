<%@ Page Title="METT - Maintenance" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
  CodeBehind="Maintenance.aspx.cs" Inherits="MEWeb.Maintenance.Maintenance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <!-- Add page specific styles and JavaScript classes below -->
  <link href="../Theme/Singular/Custom/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/Custom/customstyles.css" rel="stylesheet" />
  <link href="../Theme/Singular/METTCustomCss/Maintenance/maintenance.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
  <%
    using (var h = this.Helpers)
    {
      //	h.HTML().Heading2("Maintenance");
    }
  %>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
  <%
    using (var h = this.Helpers)
    {
        h.Control(new Singular.Web.MaintenanceHelpers.MaintenanceStateControl());
    }
  %>

  <script type="text/javascript">
    Singular.OnPageLoad(function () {
      $("#menuItem7").addClass("active");
      $("#menuItem7 > ul").addClass("in");
      //$("#menuItem7ChildItem0").addClass("subActive");
    });
    $("table").removeClass("Grid").addClass("table-responsive table table-striped table-bordered table-hover Grid SUI-RuleBorder");
  </script>

</asp:Content>
