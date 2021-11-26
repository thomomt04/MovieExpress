<%@ Page Title="Register" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Register.aspx.cs" Inherits="MEWeb.Public.Register" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<%
  using (var h = Helpers)
  {
    var toolbar = h.Toolbar();
    toolbar.Helpers.HTML().Heading2("Register");
    toolbar.Helpers.HTML("Enter your details below:");

    h.MessageHolder();

    var fieldSet = h.FieldSet("New User Details");
    fieldSet.Helpers.EditorRowFor(c => c.Title);
    fieldSet.Helpers.EditorRowFor(c => c.FirstName);
    fieldSet.Helpers.EditorRowFor(c => c.LastName);
    fieldSet.Helpers.EditorRowFor(c => c.EmailAddress);

    h.Button("Register", "Register").Validate = true;
  }
%>
</asp:Content>
