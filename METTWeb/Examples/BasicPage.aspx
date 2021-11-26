<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BasicPage.aspx.cs" Inherits="MEWeb.Examples.BasicPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <!-- Add page specific styles and JavaScript classes below -->
  <link href="../Theme/Singular/Custom/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/Custom/customstyles.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
  <!-- Placeholder not used in this example -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <%
    using (var h = this.Helpers)
    {
      var MainContent = h.DivC("row pad-top-10");
      {
        var MainContainer = MainContent.Helpers.DivC("col-md-12 p-n-lr");
        {
          var PageContainer = MainContainer.Helpers.DivC("tabs-container");
          {
            var PageTab = PageContainer.Helpers.TabControl();
            {
              PageTab.Style.ClearBoth();
              PageTab.AddClass("nav nav-tabs");
              var TabHeading = PageTab.AddTab("Basic Page");
              {
                var MainRow = TabHeading.Helpers.DivC("row margin0");
                {
                  var MainCol = MainRow.Helpers.DivC("col-md-12");
                  {
                    MainCol.Helpers.HTML().Heading2("Basic Page");
                    MainCol.Helpers.HTML("<p>The Bootstrap Grid system is a powerful mobile-first flexbox grid to build layouts of all shapes and sizes thanks to a twelve column system.</p>");
                    MainCol.Helpers.HTML("<p>Pages are designed using the bootstrap layout system, see examples below.</p><br>");
                    MainCol.Helpers.HTML().Heading3("Example");
                  }
                }
                // Create another row with columns
                var FirstRow = TabHeading.Helpers.DivC("row margin0");
                {
                  var RowCol = FirstRow.Helpers.DivC("col-md-6");
                  {
                    RowCol.Helpers.HTML().Heading4("Column 1");
                    RowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                  var SecondRowCol = FirstRow.Helpers.DivC("col-md-6");
                  {
                    SecondRowCol.Helpers.HTML().Heading4("Column 2");
                    SecondRowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                }

                var Row = TabHeading.Helpers.DivC("row margin0");
                {
                  var RowCol = Row.Helpers.DivC("col-md-4");
                  {
                    RowCol.Helpers.HTML().Heading4("Column 1");
                    RowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                  var SecondRowCol = Row.Helpers.DivC("col-md-4");
                  {
                    SecondRowCol.Helpers.HTML().Heading4("Column 2");
                    SecondRowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                  var ThirdRowCol = Row.Helpers.DivC("col-md-4");
                  {
                    ThirdRowCol.Helpers.HTML().Heading4("Column 3");
                    ThirdRowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                }

                // Create another row with columns
                var AnotherRow = TabHeading.Helpers.DivC("row margin0");
                {
                  var RowCol = AnotherRow.Helpers.DivC("col-md-3");
                  {
                    RowCol.Helpers.HTML().Heading4("Column 1");
                    RowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                  var SecondRowCol = AnotherRow.Helpers.DivC("col-md-3");
                  {
                    SecondRowCol.Helpers.HTML().Heading4("Column 2");
                    SecondRowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                  var ThirdRowCol = AnotherRow.Helpers.DivC("col-md-3");
                  {
                    ThirdRowCol.Helpers.HTML().Heading4("Column 3");
                    ThirdRowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                  var FourthRowCol = AnotherRow.Helpers.DivC("col-md-3");
                  {
                    FourthRowCol.Helpers.HTML().Heading4("Column 4");
                    FourthRowCol.Helpers.HTML("<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>");
                  }
                }
              }
            }
          }
        }
      }
    }
  %>
  <script type="text/javascript">
    // Place page specific JavaScript here or in a JS file and include in the HeadContent section
    Singular.OnPageLoad(function () {
      $("#menuItem3").addClass('active');
      $("#menuItem3 > ul").addClass('in');
    });
  </script>
</asp:Content>
