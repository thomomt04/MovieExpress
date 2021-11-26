<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PagedTables.aspx.cs" Inherits="MEWeb.Tables.tables" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

  <script type="text/javascript" src="../Scripts/JSLINQ.js"></script>
  <link href="../Theme/Singular/METTCustomCss/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
  <link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
  <link href="../Theme/Singular/css/assessment.css" rel="stylesheet" />
  <script type="text/javascript" src="../Scripts/accesscheck.js"></script>
  <style>

    img {
      display: block;
        max-width: 100px;
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
              var HomeContainerTab = AssessmentsTab.AddTab("Paged Tables");
              {
                var Row = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var RowCol = Row.Helpers.DivC("col-md-12");
                  {
                    RowCol.Helpers.HTML().Heading2("Paged Tables");
                    RowCol.Helpers.HTMLTag("p").HTML = "Implement a paged table  .";

                    var UserList = RowCol.Helpers.BootstrapTableFor<MELib.Movies.Movie>((c) => c.MovieList, false, false, "");
                    {
                      var UserListFirstRow = UserList.FirstRow;
                      {
                        var UserNameCol = UserListFirstRow.AddColumn("Movie Title");
                        {
                          var UserNameColText = UserNameCol.Helpers.Span(c => c.MovieTitle);
                        }
                        var Actions = UserListFirstRow.AddColumn("Actions");
                        {
                          // Add Buttons
                          var btnView = Actions.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                          {
                            btnView.AddClass("btn btn-primary btn-outline");
                            btnView.AddBinding(Singular.Web.KnockoutBindingString.click, "GenerateInterventionRpt($data)");
                          }
                          var btnDelete = Actions.Helpers.Button("Delete", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                          {
                            btnDelete.AddClass("btn btn-primary btn-outline");
                            btnDelete.AddBinding(Singular.Web.KnockoutBindingString.click, "GenerateInterventionRpt($data)");
                          }
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

                    RowCol.Helpers.HTML().Heading3("Adding Data Annotations");
                    RowCol.Helpers.HTMLTag("p").HTML = "In this basic example of a BootstrapTable we are making use of [Singular.DataAnnotations] to display the Movie Category name and not the Category Id.";
                    RowCol.Helpers.HTMLTag("blockquote").HTML = "<pre><code>This is a basic example of a Singular BootstrapTable.</code></pre>";

                    RowCol.Helpers.HTML().Heading3("Extending the GET Method to accept criteria");
                    RowCol.Helpers.HTMLTag("p").HTML = "In some cases you want to pass criteria and only return the results based on the criteria. We will use the above example and build a new MELib.Movies.MovieList.GetMovieList() to accept criteria.";

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
