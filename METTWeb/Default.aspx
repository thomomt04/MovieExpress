<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeBehind="Default.aspx.cs" Inherits="MEWeb.Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
  
	<script type="text/javascript">

		$(document).ready(function () {

			window.location = "Account/Home.aspx";

		});

	</script>

</asp:Content>
