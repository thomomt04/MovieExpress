<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Movies.aspx.cs" Inherits="MEWeb.Movies.Movies" %>

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
                          var ContainerTab = PageTab.AddTab("Testing");
                          {
                              var RowContentDiv = ContainerTab.Helpers.DivC("row");
                              {
                                  var ColContentDiv = RowContentDiv.Helpers.DivC("col-md-9");
                                  {
                                      var MoviesDiv = ColContentDiv.Helpers.BootstrapTableFor<MELib.Movies.Movie>((c) => c.MovieList, false, false, ""); //  <table>
                                      {

                                          var FirstRow = MoviesDiv.FirstRow; //<tr>
                                          {
                                              var MovieTitle = FirstRow.AddColumn("Title");
                                              {
                                                  var MovieTitleText = MovieTitle.Helpers.Span(c => c.MovieTitle);
                                                  MovieTitle.Style.Width = "250px";
                                              }
                                              var MovieDescription = FirstRow.AddColumn("Description");
                                              {
                                                  var MovieDescriptionText = MovieDescription.Helpers.Span(c => c.MovieDescription);
                                              }
                                              var MovieAction = FirstRow.AddColumn("");
                                              {
                                                  // Watch Movie
                                                  var WatchBtn = MovieAction.Helpers.Button("Rent Now", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                  {
                                                      WatchBtn.AddBinding(Singular.Web.KnockoutBindingString.text, c => "Rent Now @ R " + c.Price);
                                                      WatchBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "RentMovie($data)");
                                                      WatchBtn.AddClass("btn btn-primary btn-outline");
                                                  }
                                              }
                                          } 
                                      } 
                                  }
                                  var RowColRight = RowContentDiv.Helpers.DivC("col-md-3");
                                  {

                                      var AnotherCardDiv = RowColRight.Helpers.DivC("ibox float-e-margins paddingBottom");
                                      {
                                          var CardTitleDiv = AnotherCardDiv.Helpers.DivC("ibox-title");
                                          {
                                              CardTitleDiv.Helpers.HTML("<i class='ffa-lg fa-fw pull-left'></i>");
                                              CardTitleDiv.Helpers.HTML().Heading5("Filter Criteria");
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
                                              var RightRowContentDiv = ContentDiv.Helpers.DivC("row");
                                              {
                                                  var RightColContentDiv = RightRowContentDiv.Helpers.DivC("col-md-12");
                                                  {
                                                      RightColContentDiv.Helpers.LabelFor(c => ViewModel.MovieGenreID);
                                                      var ReleaseFromDateEditor = RightColContentDiv.Helpers.EditorFor(c => ViewModel.MovieGenreID);
                                                      ReleaseFromDateEditor.AddClass("form-control marginBottom20 ");

                                                      var FilterBtn = RightColContentDiv.Helpers.Button("Apply Filter", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                      {
                                                          FilterBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "FilterMovies($data)");
                                                          FilterBtn.AddClass("btn btn-primary btn-outline");
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
      }
  %>
  <script type="text/javascript">
    // Place page specific JavaScript here or in a JS file and include in the HeadContent section
    Singular.OnPageLoad(function () {
      $("#menuItem1").addClass('active');
      $("#menuItem1 > ul").addClass('in');
    });

    var RentMovie = function (obj) {
      ViewModel.CallServerMethod('RentMovie', { MovieID: obj.MovieID(), ShowLoadingBar: true }, function (result) {
        if (result.Success) {
          window.location = result.Data;
        }
        else {
          MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      })
    }

    var WatchMovie = function (obj) {

      MEHelpers.QuestionDialogYesNo("Are you sure you would like to watch this movie?", 'center',
        function () { // Yes
          ViewModel.CallServerMethod('WatchMovie', { MovieID: obj.MovieID(), ShowLoadingBar: true }, function (result) {
            if (result.Success) {
              MEHelpers.Notification("Item deleted successfully.", 'center', 'success', 5000);
            }
            else {
              MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
            }
          })
        },
        function () { // No
        })
    };

    var FilterMovies = function (obj) {
      ViewModel.CallServerMethod('FilterMovies', { MovieGenreID: obj.MovieGenreID(), ShowLoadingBar: true }, function (result) {
        if (result.Success) {
          MEHelpers.Notification("Movies filtered successfully.", 'center', 'info', 1000);
          ViewModel.MovieList.Set(result.Data);
        }
        else {
          MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      })
    };

  </script>
</asp:Content>
