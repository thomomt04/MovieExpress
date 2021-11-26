<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DepositFunds.aspx.cs" Inherits="MEWeb.Profile.DepositFunds" %>

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
          var MainHDiv = h.DivC("row pad-top-10");
          {
              var PanelContainer = MainHDiv.Helpers.DivC("col-md-12 center p-n-lr");
              {
                  var HomeContainer = PanelContainer.Helpers.DivC("tabs-container");
                  {
                      var AssessmentsTab = HomeContainer.Helpers.TabControl();
                      {
                          AssessmentsTab.Style.ClearBoth();
                          AssessmentsTab.AddClass("nav nav-tabs");
                          var HomeContainerTab = AssessmentsTab.AddTab("Manage Transactions");
                          {
                              var Row = HomeContainerTab.Helpers.DivC("row margin0");
                              {
                                  var RowCol = Row.Helpers.DivC("col-md-4-center");
                                  {
                                      var CardDiv = RowCol.Helpers.DivC("ibox float-e-margins paddingBottom");
                                      {
                                          var CardTitleDiv = CardDiv.Helpers.DivC("ibox-title");
                                          {
                                              CardTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
                                              CardTitleDiv.Helpers.HTML().Heading5("Deposit Funds");
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
                                                  var LeftColContentDiv = RowContentDiv.Helpers.DivC("col-md-4");
                                                  {
                                                      // Place holder
                                                  }
                                                  var MiddleColContentDiv = RowContentDiv.Helpers.DivC("col-md-4");
                                                  {
                                                      var FormContent = MiddleColContentDiv.Helpers.TableFor<MELib.Accounts.Account>(c => c.DepositAccount, false, false);
                                                      {
                                                          var BalanceDiv = RowContentDiv.Helpers.DivC("col-md-12");
                                                          {
                                                              var BalanceDescription = BalanceDiv.Helpers.DivC("col-md-12");
                                                              {
                                                                  var test = FormContent.FirstRow.AddColumn(c => c.Balance);
                                                                
                                                              }


                                                              var SaveBtn = BalanceDiv.Helpers.Button("Deposit", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                              {
                                                                  SaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "DepositFunds($data)");
                                                                  SaveBtn.AddClass("btn btn-primary");
                                                              }
                                                          }
                                                      }
                                                  }
                                                  var RightColContentDiv = RowContentDiv.Helpers.DivC("col-md-4");
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
    // Place page specific JavaScript here or in a JS file and include in the HeadContent section
    Singular.OnPageLoad(function () {
      $("#menuItem1").addClass('active');
      $("#menuItem1 > ul").addClass('in');
    });

      var DepositFunds = function (data) {
         
          ViewModel.CallServerMethod('DepositFunds', { Account: data.DepositAccount.Serialise(), ShowLoadingBar: true }, function (result) {
              if (result.Success) {
                  window.location = "../Account/Home.aspx";
                  Singular.AddMessage(3, 'Save', 'Saved Successfully.').Fade(2000);
                  
                  alert('Deposit successful!');
              }
              else {
                  Singular.AddMessage(1, 'Error', result.ErrorText).Fade(2000);
                  alert('Error! Incorrect Input');
                  
              }
          });
      }
  </script>
</asp:Content>
