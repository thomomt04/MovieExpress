<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BasicForms.aspx.cs" Inherits="MEWeb.Examples.BasicForms" %>

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
                          var HomeContainerTab = AssessmentsTab.AddTab("Basic Examples");
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
                                              CardTitleDiv.Helpers.HTML().Heading5("Basic Form Example");
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
                                                      var FormContent = MiddleColContentDiv.Helpers.With<MELib.Movies.Movie>(c => c.EditMovie);
                                                      {
                                                          //var MovieTitle = FormContent.Helpers.DivC("col-md-12");
                                                          //{
                                                          //  MovieTitle.Helpers.BootstrapEditorRowFor(c => c.MovieTitle);
                                                          //}

                                                          var MovieDescription = FormContent.Helpers.DivC("col-md-12");
                                                          {
                                                              MovieDescription.Helpers.BootstrapEditorRowFor(c => c.MovieDescription);
                                                          }
                                                          var MovieGenre = FormContent.Helpers.DivC("col-md-12");
                                                          {
                                                              MovieGenre.Helpers.BootstrapEditorRowFor(c => c.MovieGenreID);
                                                          }
                                                          var ActionsDiv = FormContent.Helpers.DivC("col-md-12");
                                                          {

                                                              var SaveBtn = ActionsDiv.Helpers.Button("Save", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                              {
                                                                  SaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "UpdateMovie($data)");
                                                                  SaveBtn.AddClass("btn btn-primary");
                                                              }

                                                              //var Addtn = ActionsDiv.Helpers.Button("Add", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.none);
                                                              //{
                                                              //  Addtn.AddBinding(Singular.Web.KnockoutBindingString.click, "AddMovie()");
                                                              //  Addtn.AddClass("btn btn-primary");
                                                              //}

                                                              var DeleteBtn = ActionsDiv.Helpers.Button("Delete", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                              {
                                                                  DeleteBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "DeleteMovie($data)");
                                                                  DeleteBtn.AddClass("btn btn-primary");
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
