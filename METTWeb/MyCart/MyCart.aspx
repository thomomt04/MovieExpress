<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind= "~/MyCart/MyCart.aspx.cs" 
    Inherits="MEWeb.MyCart.MyCart" %>

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
                            var ContainerTab = PageTab.AddTab("Cart");
                            {
                                var MainRow = ContainerTab.Helpers.DivC("row margin0");
                                {

                                }
                                var RowContentDiv = ContainerTab.Helpers.DivC("row");
                                {
                                    var ColContentDiv = RowContentDiv.Helpers.DivC("col-md-9");
                                    {
                                        ColContentDiv.Helpers.HTML("Total: R " + Math.Round(ViewModel.TotalSum, 2));
                                        var ProductsDiv = ColContentDiv.Helpers.TableFor<MELib.Basket.UserCart>((c) => c.UserCartList, false, false);
                                        {
                                            var FirstRow = ProductsDiv.FirstRow;
                                            {
                                                var ProductName = FirstRow.AddColumn("Product Name");
                                                {
                                                    var ProductTitleText = ProductName.Helpers.Span(c => c.ProductName);
                                                    ProductName.Style.Width = "250px";
                                                }
                                                var ItemsCount = FirstRow.AddColumn("Quantity");
                                                {
                                                    var ItemsCountText = ItemsCount.Helpers.EditorFor(c => c.ItemsCount);
                                                    ItemsCount.Style.Width = "250px";
                                                }
                                                var Price = FirstRow.AddColumn("Price");
                                                {
                                                    var ItemQuantityText = Price.Helpers.Span(c => "R" + c.SubTotal);
                                                    Price.Style.Width = "50px";
                                                }
                                                var ProductAction = FirstRow.AddColumn(" ");
                                                {
                                                    var UpdateItemBtn = ProductAction.Helpers.Button("Update", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                    {
                                                        UpdateItemBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "UpdateItem($data)");
                                                        UpdateItemBtn.AddClass("btn btn-primary btn-outline");
                                                    }
                                                    var RemoveFromCartBtn = ProductAction.Helpers.Button("Remove", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                    {
                                                        RemoveFromCartBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "RemoveFromCart($data)");
                                                        RemoveFromCartBtn.AddClass("btn btn-primary btn-outline");
                                                    }
                                                }

                                            }
                                        }
                                        ColContentDiv.Helpers.HTML(" <br> </br>");
                                         ColContentDiv.Helpers.HTML("Delivery fee is R60");
                                        ColContentDiv.Helpers.HTML(" <br> </br>");
                                        var AnotherCardDiv = ColContentDiv.Helpers.DivC("ibox float-e-margins paddingBottom");
                                        {
                                            
                                            var ContentDiv = AnotherCardDiv.Helpers.DivC("ibox-content");
                                            {
                                                var RowContentDiv1 = ContentDiv.Helpers.DivC("row");
                                                {
                                                    var ColContentDiv1 = RowContentDiv1.Helpers.DivC("col-md-12");
                                                    {
                                                        var MovieTitleContentDiv = RowContentDiv1.Helpers.DivC("col-md-12");
                                                        {
                                                            MovieTitleContentDiv.Helpers.LabelFor(c => ViewModel.DeliveryID);
                                                            var MovieTitleEditor = MovieTitleContentDiv.Helpers.EditorFor(c => ViewModel.DeliveryID);
                                                            MovieTitleEditor.AddClass("form-control marginBottom20 filterBox");
                                                           
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    RowContentDiv.Helpers.HTML(" <br> </br>");

                                    var MainRow2 = ContainerTab.Helpers.DivC("row margin0");
                                    {
                                        var MainCol = MainRow2.Helpers.DivC("col-md-12");
                                        {
                                            var CheckouttBtn = MainCol.Helpers.Button("Checkout", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                            {
                                                CheckouttBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "Checkout($data)");
                                                CheckouttBtn.AddClass("btn btn-primary btn-outline");
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
            $("#menuItem1").addClass('active');
            $("#menuItem1 > ul").addClass('in');
        });

        var SaveCart = function (obj) {
            ViewModel.CallServerMethod('SaveCart', { UserCartID: obj.UserCartID(), UserCartList: ViewModel.UserCartList.Serialise(), ShowLoadingBar: true }, function (result) {
                if (result.Success) {
                    MEHelpers.Notification("Successfully Saved.", 'center', 'Success', 3000);
                }
                else {
                    MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
                }
            })
        };

        var Checkout = function (obj) {
            ViewModel.CallServerMethod('Checkout', { TotalSum: obj.TotalSum(), UserCartList: ViewModel.UserCartList.Serialise(), DeliveryID: obj.DeliveryID(), ShowLoadingBar: true }, function (result) {
                if (result.Success) {
                    MEHelpers.Notification("Successfully purchased the products.", 'center', 'Success', 1000);
                    window.location = '../MyCart/MyCart.aspx';
                    location.reload();
                }
                else {
                    MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
                }
            });
        }

        var RemoveFromCart = function (obj) {
            ViewModel.CallServerMethod('RemoveFromCart', { UserCartID: obj.UserCartID(), ShowLoadingBar: true }, function (result) {
                if (result.Success) {
                    MEHelpers.Notification("Product removed from cart successfully.", 'center', 'info', 1000);
                    location.reload();
                }
                else {
                    MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
                }
            })
        };
        
        var UpdateItem = function (obj) {
            ViewModel.CallServerMethod('UpdateItem', { UserCartID: obj.UserCartID(), UserCartList: ViewModel.UserCartList.Serialise(), ShowLoadingBar: true }, function (result) {
                if (result.Success) {
                    MEHelpers.Notification("Item updated.", 'center', 'info', 3000);
                    location.reload();
                }
                else {
                    MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
                }
            })
        };

        var ConfirmOrder = function (User) {
            Singular.ShowMessageQuestion('Reset Password', 'Are you sure you want to reset the password for ' + User.UserName() + '?', function () {

                ViewModel.CallServerMethod('ResetPassword', { EmailAddress: User.EmailAddress(), ShowLoadingBar: true }, function (result) {
                    if (result.Success) {
                        METTHelpers.Notification("Users password has been reset successfully.", 'center', 'success', 5000);
                    } else {
                        METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
                    }
                });

            });
        }

    </script>
</asp:Content>
