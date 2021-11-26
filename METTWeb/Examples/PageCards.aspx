<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PageCards.aspx.cs" Inherits="MEWeb.Examples.PageCards" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <!-- Add page specific styles and JavaScript classes below -->
  <link href="../Theme/Singular/Custom/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/Custom/customstyles.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

  <%
    using (var h = this.Helpers)
    {

      var MainHDiv = h.DivC("row pad-top-10");
      {
        var PanelContainer = MainHDiv.Helpers.DivC("col-md-12 p-n-lr");
        {
          var HomeContainer = PanelContainer.Helpers.DivC("tabs-container");
          {
            var AssessmentsTab = HomeContainer.Helpers.TabControl();
            {
              AssessmentsTab.Style.ClearBoth();
              AssessmentsTab.AddClass("nav nav-tabs");
              var HomeContainerTab = AssessmentsTab.AddTab("Page Cards");
              {
                var MainRow = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var MainCol = MainRow.Helpers.DivC("col-md-12");
                  {
                    MainCol.Helpers.HTML().Heading2("Cards");
                    MainCol.Helpers.HTML("<p>Add cards/iboxs to a page using the bootstrap layout system, see examples below.</p><br>");
                    MainCol.Helpers.HTML().Heading3("Example");
                  }
                }


                var Row = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var RowColLeft = Row.Helpers.DivC("col-md-6");
                  {
                    // iBox / / Card 1
                    var CardDiv = RowColLeft.Helpers.DivC("ibox float-e-margins paddingBottom");
                    {
                      var CardTitleDiv = CardDiv.Helpers.DivC("ibox-title");
                      {
                        CardTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
                        CardTitleDiv.Helpers.HTML().Heading5("Card 1");
                      }
                      var CardTitleToolsDiv = CardTitleDiv.Helpers.DivC("ibox-tools");
                      {
                        var aToolsTag = CardTitleToolsDiv.Helpers.HTMLTag("a");
                        aToolsTag.AddClass("collapse-link");
                        {
                          var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
                          iToolsTag.AddClass("fa fa-chevron-up");
                        }
                      }
                      var ContentDiv = CardDiv.Helpers.DivC("ibox-content");
                      {
                        var RowContentDiv = ContentDiv.Helpers.DivC("row");
                        {
                          var LeftColContentDiv = RowContentDiv.Helpers.DivC("col-md-12");
                          {
                            // Place holder
                              LeftColContentDiv.Helpers.HTMLTag("p").HTML = "{ Place content here } ";

                          }

                        }
                      }
                    }
                  }

                   var RowColRight = Row.Helpers.DivC("col-md-6");
                  {
                    // iBox / / Card 1
                    var CardDiv = RowColRight.Helpers.DivC("ibox float-e-margins paddingBottom");
                    {
                      var CardTitleDiv = CardDiv.Helpers.DivC("ibox-title");
                      {
                        CardTitleDiv.Helpers.HTML("<i class='fa fa-table fa-lg fa-fw pull-left'></i>");
                        CardTitleDiv.Helpers.HTML().Heading5("Card 2");
                      }
                      var CardTitleToolsDiv = CardTitleDiv.Helpers.DivC("ibox-tools");
                      {
                        var aToolsTag = CardTitleToolsDiv.Helpers.HTMLTag("a");
                        aToolsTag.AddClass("collapse-link");
                        {
                          var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
                          iToolsTag.AddClass("fa fa-chevron-up");
                        }
                      }
                      var ContentDiv = CardDiv.Helpers.DivC("ibox-content");
                      {
                        var RowContentDiv = ContentDiv.Helpers.DivC("row");
                        {
                          var RightColContentDiv = RowContentDiv.Helpers.DivC("col-md-12");
                          {
                            // Place holder
                            RightColContentDiv.Helpers.HTMLTag("p").HTML = "{ Place content here } ";
                          }

                        }
                      }
                    }
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
