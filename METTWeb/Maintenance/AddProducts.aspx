<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddProducts.aspx.cs" 
    Inherits="MEWeb.Maintenance.AddProducts" %>
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
                            var HomeContainerTab = AssessmentsTab.AddTab("Manage Category");
                            {
                                var Row = HomeContainerTab.Helpers.DivC("row margin0");
                                {
                                    var RowCol = Row.Helpers.DivC("col-md-12");
                                    {
                                        var ProductList = RowCol.Helpers.TableFor<MELib.Maintenance.Product>((c) => c.ProductList, true, true);
                                        {
                                            ProductList.AddClass("table table-striped table-bordered table-hover");
                                            var ProductRow = ProductList.FirstRow;
                                            {
                                             var ProductCategoryID = ProductRow.AddColumn(c => c.ProductCategoryID);
                                                {
                                                    ProductCategoryID.Style.Width = "300px";
                                                }
                                                var ProductName = ProductRow.AddColumn(c => c.ProductName);
                                                {
                                                    ProductName.Style.Width = "300px";
                                                }
                                             var ProductDescription = ProductRow.AddColumn(c => c.ProductDescription);
                                                {
                                                    ProductDescription.Style.Width = "300px";
                                                } 
                                             var SellingPrice = ProductRow.AddColumn(c => c.SellingPrice);
                                                {
                                                    SellingPrice.Style.Width = "300px";
                                                }
                                             var Quantity = ProductRow.AddColumn(c => c.Quantity);
                                                {
                                                    Quantity.Style.Width = "300px";
                                                }
                                                var isActiveInd = ProductRow.AddColumn(c => c.IsActiveInd);
                                                {
                                                    isActiveInd.Style.Width = "300px";
                                                }
                                       }
                                        }

                                        var SaveList = RowCol.Helpers.DivC("col-md-12 text-right");
                                        {
                                            var SaveBtn = SaveList.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                            {
                                                SaveBtn.AddClass("btn-primary btn btn btn-primary");
                                                SaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveProduct($data)");
                                            }
                                        }

                                    }
                                }
                                var BottomRow = HomeContainerTab.Helpers.DivC("row margin0");
                                {
 
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

      var SaveProduct = function (obj) {
          ViewModel.CallServerMethod('SaveProduct', { ProductList: ViewModel.ProductList.Serialise(), ShowLoadingBar: true }, function (result) {
        if (result.Success) {
          MEHelpers.Notification("Successfully Saved", 'center', 'success', 3000);
        }
        else {
          MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      });
    }

  </script>
</asp:Content>
