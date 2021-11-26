<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MultiSelect.aspx.cs" Inherits="MEWeb.Examples.MultiSelect" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

  <script type="text/javascript" src="../Scripts/JSLINQ.js"></script>
  <link href="../Theme/Singular/METTCustomCss/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
  <link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
  <link href="../Theme/Singular/css/assessment.css" rel="stylesheet" />
  <script type="text/javascript" src="../Scripts/accesscheck.js"></script>

  <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.13.1/css/bootstrap-select.min.css" rel="stylesheet" />

  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js"></script>
  <!--<script type="text/javascript" src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js"></script>-->
  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.13.1/js/bootstrap-select.min.js"></script>


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
              var HomeContainerTab = AssessmentsTab.AddTab("Bootstrap Multi Select Examples");
              {
                var Row = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var RowCol = Row.Helpers.DivC("col-md-12");
                  {
                    RowCol.Helpers.HTML().Heading2("Basic Form");
                    RowCol.Helpers.HTMLTag("p").HTML = "This ia an example of how to create/edit an object and save it to the object list and database. ";

                    var CardDiv = RowCol.Helpers.DivC("ibox float-e-margins paddingBottom");
                    {
                      var CardTitleDiv = CardDiv.Helpers.DivC("ibox-title");
                      {
                        CardTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
                        CardTitleDiv.Helpers.HTML().Heading5("Basic Multiple Select Example");
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
                          var LeftColContentDiv = RowContentDiv.Helpers.DivC("col-md-2");
                          {
                            // Place holder
                          }
                          var MiddleColContentDiv = RowContentDiv.Helpers.DivC("col-md-8");
                          {
                            //var multiSelect = MiddleColContentDiv;
                            //{
                            //  //multiSelect.Helpers.HTML("<label>Multiple Select</label>");
                            //  multiSelect.Helpers.HTML("<select class='selectpicker' multiple>");

                            //  var optionsList = multiSelect.Helpers.ForEach<MELib.Movies.Movie>(c => c.MovieList);
                            //  {
                            //    // var optionsListItem = optionsList.Helpers.HTML("<option data-bind=\"text: $data.MovieTitle()\"></option>");
                            //    var optionsListItem = optionsList.Helpers.HTMLTag("<option>");
                            //    optionsListItem.Helpers.HTML("<span data-bind=\"text: $data.MovieTitle()\"></span");

                            //  }
                            //  multiSelect.Helpers.HTML("</select>");
                            //}
                            //BootstrapTableFor<MELib.Movies.Movie>((c) => c.MovieList, false, false, "");

                            var optionlist3 = MiddleColContentDiv.Helpers.HTMLTag("select");
                            {
                              optionlist3.AddClass("selectpicker");
                              var optionsList = optionlist3.Helpers.ForEach<MELib.Movies.Movie>(c => c.MovieList);
                              {
                                var tag = optionsList.Helpers.HTMLTag("option data-bind=\"text: $data.MovieTitle()\">");
                                {

                                }

                              }

                            }




                          }
                          var RightColContentDiv = RowContentDiv.Helpers.DivC("col-md-2");
                          {
                            // Place holder
                          }
                        }
                      }
                    }

                    var AnotherCardDiv = RowCol.Helpers.DivC("ibox float-e-margins paddingBottom");
                    {
                      var CardTitleDiv = AnotherCardDiv.Helpers.DivC("ibox-title");
                      {
                        CardTitleDiv.Helpers.HTML("<i class='fa fa-cog fa-lg fa-fw pull-left'></i>");
                        CardTitleDiv.Helpers.HTML().Heading5("Place Title Here");
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
                        var RowContentDiv = ContentDiv.Helpers.DivC("row");
                        {
                          var LeftColContentDiv = RowContentDiv.Helpers.DivC("col-md-4");
                          {
                            // Place holder
                          }
                          var MiddleColContentDiv = RowContentDiv.Helpers.DivC("col-md-4");
                          {
                            MiddleColContentDiv.Helpers.HTML("[ This is an example without the actual form, place your content here ]");
                          }
                          var RightColContentDiv = RowContentDiv.Helpers.DivC("col-md-4");
                          {
                            // Place holder
                          }
                        }
                        // Place Content Here

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
    Singular.OnPageLoad(function () {
      $("#menuItem4").addClass("active");
      $("#menuItem4 > ul").addClass("in");
    });


    var UpdateMovie = function (obj) {
      alert('Place your update movie code here');
    };

    var AddMovie = function (obj) {
      alert('Place your add movie code here');
    };


    var DeleteMovie = function (obj) {
      MEHelpers.QuestionDialogYesNo("Are you sure you would like to delete this item?", 'center',
        function () { // Yes 
          ViewModel.CallServerMethod('DeleteMovie', { MovieID: obj.MovieID(), ShowLoadingBar: true }, function (result) {
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

</script>
</asp:Content>
