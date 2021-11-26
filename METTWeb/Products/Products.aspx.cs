using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MELib.Basket;
using MELib.Cart;
using MELib.Maintenance;
using Singular.Web;


namespace MEWeb.Products
{
    public partial class Products : PageBase<ProductsVM>
    {
    }
    public class ProductsVM : StatelessViewModel<ProductsVM>
    {
        public MELib.Maintenance.ProductList ProductList { get; set; }

        // Filter Criteria
        public DateTime ReleaseFromDate { get; set; }
        public DateTime ReleaseToDate { get; set; }
        /// <summary>
        /// Gets or sets the Movie Genre ID
        /// </summary>
        [Singular.DataAnnotations.DropDownWeb(typeof(MELib.RO.ROProductCategoryList), UnselectedText = "Select", ValueMember = "ProductCategoryID", DisplayMember = "ProductCategoryName")]
        [Display(Name = "Category")]
        public int? ProductCategoryID { get; set; }
        public bool IsActiveInd { get; set; }
        public decimal TotalSum { get; set; }

        public bool ProductDialog { get; set; }

        public ProductsVM()
        {

        }
        protected override void Setup()
        {
            base.Setup();
            ProductList = MELib.Maintenance.ProductList.GetProductList();
            ProductList = MELib.Maintenance.ProductList.GetProductList(true);
            TotalSum = UserCartList.GetUserCartList().Sum(x => x.SubTotal);

            ProductDialog = false;
        }

        [WebCallable]
        public Result FilterProducts(int ProductCategoryID)
        {
            Result sr = new Result();
            try
            {
                if (ProductCategoryID != 0)
                {
                    sr.Data = MELib.Product.ProductList.GetProductList(ProductCategoryID, 0).Where(x => x.ProductCategoryID == ProductCategoryID);
                    sr.Success = true;
                }
                else
                {

                    sr.Data = MELib.Maintenance.ProductList.GetProductList();
                    sr.Success = true;
                }

            }
            catch (Exception e)
            {
                WebError.LogError(e, "Page: Products.aspx | Method: FilterProducts", $"(int ProductCategoryID, ({ProductCategoryID})");
                sr.Data = e.InnerException;
                sr.ErrorText = "Could not filter products by category. Try again";
                sr.Success = false;
            }
            return sr;
        }
        //Method for Adding to Cart
        //from products page to cart page
        [WebCallable]
        public Result AddToCart(int ProductID, ProductList productList)
        {
            Result sr = new Result();
            var Item = MELib.Product.ProductList.GetProductList(0, ProductID).FirstOrDefault();
            var ProductPrice = Item.SellingPrice;
            var Quantity = Item.Quantity;
            var InputQuantity = productList.GetItem(ProductID).InputQuantity;
            try
            {
                if (ProductID != 0)
                {                                    
                    if (InputQuantity > Quantity || InputQuantity < 1)
                    {
                        return new Singular.Web.Result() { ErrorText = "Input quantity  is 0 or greater than that in stock", Success = false };
                    }
                    else
                    {
                        UserCart cart = new UserCart();
                        UserCartList tempStorage = new UserCartList();

                        cart.ProductID = (int)ProductID;
                        cart.SubTotal = ProductPrice * InputQuantity;
                        cart.isActiveInd = true;
                        cart.ItemsCount = InputQuantity;
                        cart.UserID = Singular.Security.Security.CurrentIdentity.UserID;
                        tempStorage.Add(cart);

                        var SaveResult = tempStorage.TrySave(); 

                        var UpdateProduct = MELib.Maintenance.ProductList.GetProductList().GetItem(ProductID);
                        UpdateProduct.Quantity -= InputQuantity;
                        UpdateProduct.TrySave(typeof(MELib.Maintenance.ProductList));


                        if (SaveResult.Success)
                        {
                            sr.Data = SaveResult.SavedObject;
                            sr.Success = true;
                        }
                        else
                        {
                            sr.ErrorText = SaveResult.ErrorText;
                            sr.Success = false;
                        }
                       
                    }
                }
                else
                {
                    sr.ErrorText = "Something went wrong.Please try again!";
                    sr.Success = false;
                }
            }
            catch (Exception ex)
            {
                //Return error message
                WebError.LogError(ex, "Page: Products.aspx | Method: FilterProducts", $"int ProductID, ({ProductID})");
                sr.Data = ex.InnerException;
                sr.ErrorText = "Something ent wrong.Please try again!";
                sr.Success = false;
            }
            return sr;
        }

    }
}