<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Overview.aspx.cs" Inherits="MEWeb.Overview.Overview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <script type="text/javascript" src="../Scripts/JSLINQ.js"></script>
  <link href="../Theme/Singular/Custom/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/Custom/customstyles.css" rel="stylesheet" />

  <style>
    .bg-tip {
      background: #F3F3F3;
      padding: 20px;
      border: 2px solid #F3F3F3;
      border-radius: 5px;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
  <%
    using (var h = this.Helpers)
    {
      //h.HTML().Heading2("Home");
    }
  %>
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
              var HomeContainerTab = AssessmentsTab.AddTab("Overview");
              {
                var Row = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var RowCol = Row.Helpers.DivC("col-md-12");
                  {
                    RowCol.Helpers.HTML().Heading2("General Overview");
                    RowCol.Helpers.HTMLTag("p").HTML = "This solution uses the Model-View-ViewModel (MVVM) structural design pattern. Model-View-ViewModel (MVVM) separates objects into three distinct groups:</p>";
                    RowCol.Helpers.HTML("<ul><li><b>Models</b> hold application data. They’re usually structs or simple classes.</li>");
                    RowCol.Helpers.HTML("<li><b>Views</b> display visual elements and controls on the screen. They’re typically subclasses of UIView.</li>");
                    RowCol.Helpers.HTML("<li><b>View models</b> transform model information into values that can be displayed on a view. They're usually classes, so they can be passed around as references.</li></ul>");
                    RowCol.Helpers.HTMLTag("p").HTML = "Follow the link below to read more about MVVM and/or continue to the next page...";
                    var VisitURLBtn = RowCol.Helpers.Button("Read More...", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.desktop);
                    {
                      VisitURLBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "VisitURL()");
                      VisitURLBtn.AddClass("btn btn-primary btn-outline");
                    }
                    RowCol.Helpers.HTML("<p></br></p>");
                  }
                  var PageRowColLeft = Row.Helpers.DivC("col-md-6");
                  {
                    PageRowColLeft.Helpers.HTML().Heading3("Exercise 1: Create a New Page");
                    PageRowColLeft.Helpers.HTMLTag("p").HTML = "Watch the video on how to create a basic MVVM Aspx.net page, learn about the page structure and add it to the Sitemap.";
                    PageRowColLeft.Helpers.HTML("<p>In this video you will learn the following;</p>");
                    PageRowColLeft.Helpers.HTML("<ul>");
                    PageRowColLeft.Helpers.HTML("<li>How to create a blank Aspx.net Page from a MasterPage.</li>");
                    PageRowColLeft.Helpers.HTML("<li>Create a Page, ViewModel and add content to the View/Page.</li>");
                    PageRowColLeft.Helpers.HTML("<li>Understand the layout/structure of the page using Bootstrap's Grid system (Rows/Cols).</li>");
                    PageRowColLeft.Helpers.HTML("<li>Add a button and Knockout binding to call a custom JavaScript function.</li>");
                    PageRowColLeft.Helpers.HTML("<li>Add the newly create page to the project's Sitemap file.</li>");
                    PageRowColLeft.Helpers.HTML("</ul>");

                    var TipContent = PageRowColLeft.Helpers.DivC("row bg-tip");
                    {
                      var TipIcon = TipContent.Helpers.DivC("col-md-2");
                      {
                        TipIcon.Helpers.HTML("<img src='../Images/ReadMore.png' width='75px'>");
                      }
                      var TipText = TipContent.Helpers.DivC("col-md-10");
                      {
                        TipText.Helpers.HTML("<p>Read more on Bootstrap's Grid system and Knockout JS below.</p>");
                        TipText.Helpers.HTML("<p><a href='https://getbootstrap.com/docs/4.4/layout/grid/' target='_blank'>https://getbootstrap.com/docs/4.4/layout/grid/</a></p>");
                        TipText.Helpers.HTML("<p><a href='https://knockoutjs.com/index.html' target='_blank'>https://knockoutjs.com/index.html</a></p>");
                      }
                    }
                    PageRowColLeft.Helpers.HTML("<p></br></p>");

                    var NextBtn = PageRowColLeft.Helpers.Button("View Example 'Basic Page'", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.search);
                    {
                      NextBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewExample()");
                      NextBtn.AddClass("btn btn-primary btn-outline");
                    }
                     var ExternalBtn = PageRowColLeft.Helpers.Button("Proceed to Databases", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.arrow_right);
                    {
                      ExternalBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "NextPage()");
                      ExternalBtn.AddClass("btn btn-primary btn-outline");
                    }

                    PageRowColLeft.Helpers.HTML("<p></br></p>");
                  }
                  var PageRowColRight = Row.Helpers.DivC("col-md-6");
                  {
                    PageRowColRight.Helpers.HTML("<video width='320' height='240' controls><source src='../Videos/01 2021 Creating a basic Aspx Page from a MasterPage in Visual Studio.mp4' type='video/mp4'>");

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

    // On page load
    Singular.OnPageLoad(function () {
      $("#menuItem1").addClass("active");
      $("#menuItem1 > ul").addClass("in");
    });

    // Page specific functions, preferrably in a seperate JS file
    var ViewExample = function () {
      window.location = '../Examples/BasicPage.aspx';
    }

    var NextPage = function () {
      window.location = '../Overview/Database.aspx';

    }

    var VisitURL = function () {
      window.open('https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm', '_blank');
    }
  </script>

</asp:Content>
