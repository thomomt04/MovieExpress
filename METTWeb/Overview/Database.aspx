<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Library.aspx.cs" Inherits="MEWeb.Overview.Database" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <script type="text/javascript" src="../Scripts/JSLINQ.js"></script>
  <link href="../Theme/Singular/Custom/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/Custom/customstyles.css" rel="stylesheet" />
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
                    RowCol.Helpers.HTML().Heading2("Databases");
                    RowCol.Helpers.HTMLTag("p").HTML = "Singular follows a data directed approach to software development and a significant amount of time and emphasis is placed on correct and efficient database design at the onset of any project.";
                    RowCol.Helpers.HTMLTag("p").HTML = "Read more about the Singular Database Standards by downloading the Word document and watching the videos.";
                    var VisitURLBtn = RowCol.Helpers.Button("Download Singular Standards Document...", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.download);
                    {
                      VisitURLBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "VisitURL()");
                      VisitURLBtn.AddClass("btn btn-primary btn-outline");
                    }
                    var ViewVideoBtn = RowCol.Helpers.Button("Singular Standards Video...", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.desktop);
                    {
                      ViewVideoBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewVideo()");
                      ViewVideoBtn.AddClass("btn btn-primary btn-outline");
                    }
                    RowCol.Helpers.HTML("<p></br></p>");
                  }
                }
                var AnotherRow = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var RowColLeft = Row.Helpers.DivC("col-md-6");
                  {
                    RowColLeft.Helpers.HTML().Heading3("Exercise 2: Create a New Table");
                    RowColLeft.Helpers.HTMLTag("p").HTML = "Watch the video on how to create a new table, learn about the standards and add it to the database solution. ";
                    RowColLeft.Helpers.HTML("<p>In this video you will learn the following;</p>");
                    RowColLeft.Helpers.HTML("<ul>");
                    RowColLeft.Helpers.HTML("<li>How to create a table to the database and the relevant columns.</li>");
                    RowColLeft.Helpers.HTML("<li>Understand the standards put in place.</li>");
                    RowColLeft.Helpers.HTML("</ul>");
                   
                     var ExternalBtn = RowColLeft.Helpers.Button("Proceed to CSLA Objects", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.arrow_right);
                    {
                      ExternalBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "NextPage()");
                      ExternalBtn.AddClass("btn btn-primary btn-outline");
                    }
                  }
                  var RowColRight = Row.Helpers.DivC("col-md-6");
                  {
                     RowColRight.Helpers.HTML("<video width='320' height='240' controls><source src='../Media/01 Database/02 2021 Creating a table using Singular Design Standards.mp4' type='video/mp4'>");
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
    Singular.OnPageLoad(function () {
      $("#menuItem1").addClass('active');
      $("#menuItem1 > ul").addClass('in');
    });

    // Page specific functions, preferrably in a seperate JS file
    var ViewExample = function () {
      window.location = '../Examples/BasicPage.aspx';
    }

    var NextPage = function () {
      window.location = '../Overview/CSLA.aspx';
    }

    var VisitURL = function () {
      window.open('../Media/01 Database/SQL Database standards.docx', '_blank');
    }

     var ViewVideo = function () {
      window.open('../Media/01 Database/Singular Database Design.avi', '_blank');
    }
  </script>

</asp:Content>
