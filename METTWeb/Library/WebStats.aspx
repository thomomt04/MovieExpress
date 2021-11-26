<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="WebStats.aspx.cs" Inherits="MEWeb.WebStats" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Web Stats</h2>

<%= new Singular.Web.CustomControls.WebStatsInfo().RenderAsString() %>

</asp:Content>
