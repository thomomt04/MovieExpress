<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BasicTable.aspx.cs" Inherits="MEWeb.Examples.BasicTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <script type="text/javascript" src="../Scripts/JSLINQ.js"></script>
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
              var HomeContainerTab = AssessmentsTab.AddTab("Basic Table");
              {
                var Row = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var RowCol = Row.Helpers.DivC("col-md-12");
                  {
                    RowCol.Helpers.HTML().Heading2("Basic Table");
                    RowCol.Helpers.HTMLTag("p").HTML = "Create a basic table by generating your classes using the CSLA Extension.";
                    RowCol.Helpers.HTML("You can alter the stored procedure to retrieve the required information (e.g. Genre Name/Description instead of the foreign key/id).<br><br>");
                    RowCol.Helpers.HTML().Heading3("Example");

                    var MovieList = RowCol.Helpers.BootstrapTableFor<MELib.Movies.Movie>((c) => c.MovieList, false, false, "");
                    {
                      var MovieListRow = MovieList.FirstRow;
                      {
                        var MovieTitle = MovieListRow.AddColumn("Title");
                        {
                          var MovieTitleText = MovieTitle.Helpers.Span(c => c.MovieTitle);
                        }
                        var MovieGenreID = MovieListRow.AddColumn("Genre");
                        {
                          var MovieGenreText = MovieGenreID.Helpers.Span(c => c.MovieGenreID);
                        }
                        var MovieDescription = MovieListRow.AddColumn("Description");
                        {
                          var MovieDescriptionText = MovieDescription.Helpers.Span(c => c.MovieDescription);
                        }
                      }
                    }
                  }
                }
                var Row2 = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var RowCol = Row.Helpers.DivC("col-md-12");
                  {
                    RowCol.Helpers.HTML().Heading2("Overview");
                    RowCol.Helpers.HTMLTag("p").HTML = "In this basic example of a BootstrapTable we using a business object [MovieList] to display the data from the [Movies] table.";
                    RowCol.Helpers.HTMLTag("p").HTML = " In the page's ViewModel we load data from the movies table into a list object.";
                    RowCol.Helpers.HTMLTag("blockquote").HTML = "<pre><code>MovieList = MELib.Movies.MovieList.GetMovieList();</code></pre>";
                    RowCol.Helpers.HTMLTag("p").HTML = "From the page (View) we bind to the object using BoostrapTable in this case.";

                    RowCol.Helpers.HTMLTag("blockquote").HTML = "<pre><code>var UserList = RowCol.Helpers.BootstrapTableFor<MELib.Movies.Movie>((c) => c.MovieList, false, false, &quot;&quot;);</code></pre>";
                    RowCol.Helpers.HTMLTag("p").HTML = "To add a column use the code snippet below to reference the column.";
                    RowCol.Helpers.HTMLTag("blockquote").HTML = "<pre><code>var UserNameCol = UserListFirstRow.AddColumn(&quot;Movie Title&quot;)<br>{<br>  var UserNameColText = UserNameCol.Helpers.Span(c => c.MovieTitle);<br>}</code></pre>";
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
      $("#menuItem5").addClass("active");
      $("#menuItem5 > ul").addClass("in");
    });
  </script>
</asp:Content>
