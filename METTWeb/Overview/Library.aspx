<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Library.aspx.cs" Inherits="MEWeb.Overview.SingularLibrary" %>
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
                    RowCol.Helpers.HTML().Heading2("Singular Library and Working with CSLA Objects");
                    RowCol.Helpers.HTMLTag("p").HTML = "Once you have created your table and CSLA object as per the last exercise, your next step is to display the information on a page.";
                    //var VisitURLBtn = RowCol.Helpers.Button("Download Singular Standards Document...", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.download);
                    //{
                    //  VisitURLBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "VisitURL()");
                    //  VisitURLBtn.AddClass("btn btn-primary btn-outline");
                    //}
                    RowCol.Helpers.HTML("<p></br></p>");

                  }
                }
                var AnotherRow = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var LeftCol = AnotherRow.Helpers.DivC("col-md-6");
                  {
                    LeftCol.Helpers.HTML().Heading3("Exercise 4: Create a Basic Table");
                    LeftCol.Helpers.HTMLTag("p").HTML = "Watch the video on how to create a new CSLA class and learn about the key features. ";
                    LeftCol.Helpers.HTML("<p>In this video you will learn the following;</p>");
                    LeftCol.Helpers.HTML("<ul>");
                    LeftCol.Helpers.HTML("<li>Setting up CSLA for the first time.</li>");
                    LeftCol.Helpers.HTML("<li>How to create a new CSLA object in the Library Project.</li>");
                    LeftCol.Helpers.HTML("<li>Importance of your table design/Singular Standards.</li>");
                    LeftCol.Helpers.HTML("<li>Review the Stored Procedures created.</li>");
                    LeftCol.Helpers.HTML("</ul>");


                    //var TipContent = LeftCol.Helpers.DivC("row bg-tip");
                    //{
                    //  var TipIcon = TipContent.Helpers.DivC("col-md-2");
                    //  {
                    //    TipIcon.Helpers.HTML("<img src='../Images/ReadMore.png' width='75px'>");
                    //  }
                    //  var TipText = TipContent.Helpers.DivC("col-md-10");
                    //  {
                    //    TipText.Helpers.HTML("<p>CSLA Objects, Properties and Stored Procedures.</p>");
                    //    TipText.Helpers.HTML("<p>Take note of normal objects and read only objects. Some properties might not be needed and these can be removed, depending on your project requirements.</p>");
                    //    TipText.Helpers.HTML("<p>In some cases you might want to use a temporary table to generate your objects based on a SQL Statement.</p>");
                    //  }
                    //}
                    LeftCol.Helpers.HTML("<p></br></p>");

                    var ExternalBtn = LeftCol.Helpers.Button("Proceed to Basic Examples", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.arrow_right);
                    {
                      ExternalBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "NextPage()");
                      ExternalBtn.AddClass("btn btn-primary btn-outline");
                    }
                  }
                  var RightCol = AnotherRow.Helpers.DivC("col-md-6");
                  {
                    RightCol.Helpers.HTML("<video width='320' height='240' controls><source src='../Media/04 Singular Library/04 2021 Binding CSLA Object to ViewModel and creating a Boostrap Table to view the data on the View.mp4' type='video/mp4'>");
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
      // Placeholder
    });
    
    var NextPage = function () {
      window.location = '../Overview/Overview.aspx';
    }
    
    var VisitURL = function () {
      window.open('https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm','_blank');
    }
  </script>

</asp:Content>
