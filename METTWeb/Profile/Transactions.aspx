<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Transactions.aspx.cs" Inherits="MEWeb.Profile.Transactions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <!-- Add page specific styles and JavaScript classes below -->
  <link href="../Theme/Singular/Custom/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/Custom/customstyles.css" rel="stylesheet" />

<style>
.column 
{
    text-align:right;
}
.column1 
{
    text-align:left;

}
</style>
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
                          var ContainerTab = PageTab.AddTab("Transaction History");
                          {
                              var RowContentDiv = ContainerTab.Helpers.DivC("row");
                              {

                                  #region Left Column / Data
                                  var LeftColRight = RowContentDiv.Helpers.DivC("col-md-9");
                                  {

                                      var AnotherCardDiv = LeftColRight.Helpers.DivC("ibox float-e-margins paddingBottom");
                                      {
                                          var CardTitleDiv = AnotherCardDiv.Helpers.DivC("ibox-title");
                                          {
                                              CardTitleDiv.Helpers.HTML("<i class='ffa-lg fa-fw pull-left'></i>");
                                              CardTitleDiv.Helpers.HTML().Heading5("Transactions");
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
                                      }
                                      var ProductsDiv = LeftColRight.Helpers.BootstrapTableFor<MELib.Transactions.Transaction>((c) => c.TransHistory, false, false, "");
                                      {
                                          var FirstRow = ProductsDiv.FirstRow;
                                          {
                                              var DateTitle = FirstRow.AddColumn("Created Date");
                                              {
                                                  var DateTitleText = DateTitle.Helpers.Span(c => c.CreatedDate);
                                                  DateTitle.Style.Width = "250px";
                                              }
                                              var DescriptionTitle = FirstRow.AddColumn("Description");
                                              {
                                                  var PDescriptionTitleText = DescriptionTitle.Helpers.Span(c => c.Description);
                                                  DescriptionTitle.Style.Width = "250px";
                                              }
                                              var AmountDescription = FirstRow.AddColumn("Total Amount");
                                              {
                                                  var AmountDescriptionText = AmountDescription.Helpers.Span(c => "R" + c.Amount);
                                                  AmountDescription.AddClass("column");
                                              }
                                          }
                                      }
                                  }
                                  #endregion

                                  #region Right Column / Filters
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
                                              var RowContentDiv1 = ContentDiv.Helpers.DivC("row");
                                              {
                                                 
                                                  var MovieGenreContentDiv = RowContentDiv1.Helpers.DivC("col-md-12");
                                                  {

                                                      MovieGenreContentDiv.Helpers.LabelFor(c => ViewModel.TransactionTypeID);
                                                      var ReleaseFromDateEditor = MovieGenreContentDiv.Helpers.EditorFor(c => ViewModel.TransactionTypeID);
                                                      ReleaseFromDateEditor.AddClass("form-control marginBottom20 ");

                                                      var FilterBtn = MovieGenreContentDiv.Helpers.Button("Apply Filter", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                      {
                                                          FilterBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "FilterTransactions($data)");
                                                          FilterBtn.AddClass("btn btn-primary btn-outline");
                                                      }
                                                      var ResetBtn = MovieGenreContentDiv.Helpers.Button("Reset", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                      {
                                                          ResetBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "FilterReset($data)");
                                                          ResetBtn.AddClass("btn btn-primary btn-outline");
                                                      }
                                                  }
                                              }
                                          }
                                      }
                                  }
                              }
                              #endregion
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
      var FilterTransactions = function (obj) {
          ViewModel.CallServerMethod('FilterTransactions', { TransactionTypeID: obj.TransactionTypeID(), ResetInd: 0, ShowLoadingBar: true }, function (result) {
              if (result.Success) {
                  ViewModel.TransHistory.Set(result.Data);
                  MEHelpers.Notification("Transactions filtered successfully.", 'center', 'info', 1000);                  
              }
              else {
                  MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
              }
          })
      };

  </script>
</asp:Content>
