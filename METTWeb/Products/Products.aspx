<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="MEWeb.Products.Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- Add page specific styles and JavaScript classes below -->
    <link href="../Theme/Singular/Custom/home.css" rel="stylesheet" />
    <link href="../Theme/Singular/Custom/customstyles.css" rel="stylesheet" />

<style>
.movie-border {
  border-radius: 5px;
  border: 2px solid #DEDEDE;
  padding: 5px;
  margin: 5px;
  width: 100px;
}
div.item {
  vertical-align: top;
  display: block;
  text-align: center;
  padding-bottom: 5px;
  width:5px;
}
.caption {
  display: block;
  padding-bottom: 5px;
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
                            var ContainerTab = PageTab.AddTab("Available Products");
                            {
                                var RowContentDiv = ContainerTab.Helpers.DivC("row");
                                {
                                    var ColContentDiv = RowContentDiv.Helpers.DivC("col-md-9");
                                    {
                                        ColContentDiv.Helpers.HTML("Total of selected: R " + Math.Round(ViewModel.TotalSum,2));
                                        var ProductsDiv = ColContentDiv.Helpers.BootstrapTableFor<MELib.Maintenance.Product>((c) => c.ProductList, false, false, "");
                                        {
                                            var FirstRow = ProductsDiv.FirstRow;
                                            {
                                                var ProductTitle = FirstRow.AddColumn("Product Name");
                                                {
                                                    var ProductTitleText = ProductTitle.Helpers.Span(c => c.ProductName);
                                                    ProductTitle.Style.Width = "250px";
                                                }
                                                var ProductDescription = FirstRow.AddColumn("Image");
                                                {
                                                    
                                                    var prodImg = ProductDescription.Helpers.Span("<img data-bind=\"attr:{src: $data.ProductImageURL()} \" class='movie-border'/>");
                                                    prodImg.Helpers.HTML("<div class='item'>");
                                                   }
                                                var QuantityText = ProductsDiv.FirstRow.AddColumn(c => c.InputQuantity);

                                                var ProductPrice = FirstRow.AddColumn("Price");
                                                {
                                                    var ProductPriceText = ProductPrice.Helpers.Span(c => "R" + c.SellingPrice);
                                                }
                                                var ProductAction = FirstRow.AddColumn("");
                                                {

                                                    var AddToCartBtn = ProductAction.Helpers.Button("Add To Cart", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                    {
                                                        AddToCartBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "AddToCart($data)");
                                                        AddToCartBtn.AddClass("btn btn-primary btn-outline");
                                                    }
                                                }
                                            }
                                        }
                                        var SaveBtn = ColContentDiv.Helpers.Button("Go To Cart", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                        {
                                            SaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "GoToCart($data)");
                                            SaveBtn.AddClass("btn btn-primary");
                                        }
                                        ColContentDiv.Helpers.HTML(" <br> </br>");
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
                                                        RightColContentDiv.Helpers.LabelFor(c => ViewModel.ProductCategoryID);
                                                        var ReleaseFromDateEditor = RightColContentDiv.Helpers.EditorFor(c => ViewModel.ProductCategoryID);
                                                        ReleaseFromDateEditor.AddClass("form-control marginBottom20 "); var FilterBtn = RightColContentDiv.Helpers.Button("Apply Filter", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                                        {
                                                            FilterBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "FilterProducts($data)");
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

        var AddToCart = function (obj) {
            ViewModel.CallServerMethod('AddToCart', { ProductID: obj.ProductID(), ProductList: ViewModel.ProductList.Serialise(), ShowLoadingBar: true }, function (result) {
                if (result.Success) {
                    MEHelpers.Notification("Product added to cart successfully.", 'center', 'info', 5000);
                    location.reload();
                }
                else {
                    MEHelpers.Notification("Input quantity  is 0 or greater than that in stock", 'center', 'warning', 5000);
                }
            })
        };

        var GoToCart = function () {
            window.location = '../MyCart/MyCart.aspx';
        }

        var FilterProducts = function (obj) {
            ViewModel.CallServerMethod('FilterProducts', { ProductCategoryID: obj.ProductCategoryID(), ShowLoadingBar: true }, function (result) {
                if (result.Success) {
                    MEHelpers.Notification("Products filtered successfully.", 'center', 'info', 1000); ViewModel.ProductList.Set(result.Data);
                }
                else {
                    MEHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
                }
            })
        }; </script>
</asp:Content>

