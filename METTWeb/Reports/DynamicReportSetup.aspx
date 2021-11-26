<%@ Page Title="" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="DynamicReportSetup.aspx.cs" Inherits="METTWeb.Reports.DynamicReportSetup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 <link href="../Styles/CustomOverrides.css" rel="stylesheet" type="text/css" />

 <style type="text/css">
   .EditReport {
     margin-bottom: 20px;
   }

     .EditReport div.row label {
       width: 150px;
     }

     .EditReport input[type=text] {
       width: 300px;
     }


   .GridAltRow:hover {
     background: lightgray;
   }


   .GridRowFIG:hover {
     background: white;
   }

 </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
   <%
     using (var h = Helpers)
     {
       h.Control(new Singular.Web.Reporting.DynamicReportSetupControl());
     }
			%>
  
</asp:Content>