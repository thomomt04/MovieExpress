<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Movie.aspx.cs" Inherits="MEWeb.Movies.Movie" %>

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
                          var ContainerTab = PageTab.AddTab("Available Movies");
                          {
                              var RowContentDiv = ContainerTab.Helpers.DivC("row");
                              {
                                  #region Left Column / Data
                                  var LeftColRight = RowContentDiv.Helpers.DivC("col-md-2");
                                  {
                                  }
                                  #endregion

                                  #region Deposit Column / Filters
                                  var MiddleColRight = RowContentDiv.Helpers.DivC("col-md-8");
                                  {

                                      var AnotherCardDiv = MiddleColRight.Helpers.DivC("ibox float-e-margins paddingBottom");
                                      {
                                          var CardTitleDiv = AnotherCardDiv.Helpers.DivC("ibox-title");
                                          {
                                              CardTitleDiv.Helpers.HTML("<i class='ffa-lg fa-fw pull-left'></i>");
                                              CardTitleDiv.Helpers.HTML().Heading5("Transaction Purchase Confirmation");
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
                                          var ContentDiv = AnotherCardDiv.Helpers.DivC("ibox-content");
                                          {
                                              var MovieContentDiv = ContentDiv.Helpers.DivC("row");
                                              {
                                                  var MovieDiv = MovieContentDiv.Helpers.DivC("col-md-12 text-center");
                                                  {
                                                      var FormContent = MovieDiv.Helpers.ForEach<MELib.Movies.Movie>((c) => c.MovieList);
                                                      {
                                                          // Place holder
                                                          FormContent.Helpers.Span(c => c.MovieTitle).Style.FontSize="30px";
                                                          FormContent.Helpers.HTML("<br> </br>");
                                                          FormContent.Helpers.Span(c => c.MovieDescription);
                                                          MovieDiv.Helpers.HTML("<h3>Preview</h3>");
                                                          MovieDiv.Helpers.HTML("<p></p>");
                                                          var VideoContainer = MovieDiv.Helpers.HTMLTag("video controls");
                                                          {
                                                              VideoContainer.Helpers.HTML("<source src='../Media/Videos/Silver Fox Intro.mp4' type='video/mp4'>");
                                                          }

                                                          // Rent Movie
                                                          var RentMovieBtn = MovieDiv.Helpers.Button("Pay & Watch", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                          {
                                                              RentMovieBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "RentNow($data)");
                                                              RentMovieBtn.AddClass("btn btn-primary");
                                                          }
                                                      }
                                                  }

                                              }
                                          }
                                      }
                                      #endregion
                                      #region Right Column / Data
                                      var RowColRight = RowContentDiv.Helpers.DivC("col-md-2");
                                      {
                                      }
                                      #endregion
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
      $("#menuItem1").addClass('active');
      $("#menuItem1 > ul").addClass('in');
    });

      var RentNow = function (obj) {
          ViewModel.CallServerMethod('RentNow', { MovieID: obj.MovieID(), ShowLoadingBar: true }, function (result) {
              if (result.Success) {
                  MEHelpers.Notification("Movie paid successfully.", 'center', 'Success', 3000);
              }
              else {
                  MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
              }
          })
      };

  </script>
</asp:Content>
