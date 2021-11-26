using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular;
using Singular.Web;
using MELib.Basket;
using MELib.Accounts;
using System.ComponentModel.DataAnnotations;

namespace MEWeb.MyCart
{
    public partial class MyCart : MEPageBase<MyCartVM>
    {
    }
    public class MyCartVM : MEStatelessViewModel<MyCartVM>
    {

        public UserCartList UserCartList { get; set; }
        public MELib.Basket.DeliveryList DeliveryList { get; set; }
        public MELib.Product.ProductList ProductList { get; set; }
        public MELib.Accounts.AccountList AccountList { get; set; }
        public MELib.Maintenance.ProductList productList{ get; set; }
       public MELib.Basket.ShoppingCartList ShoppingCartList { get; set; }
        public MELib.Transactions.TransactionList TransactionList { get; set; }
        public MELib.Transactions.Transaction Transaction { get; set; }
       
        public decimal TotalSum { get; set; }
        public decimal UserBalance { get; set; }

        int? ProductID { get; set; }

        public int? UserCartID { get; set; }
        public int UserID { get; set; }
      
        public  int? TransactionID { get; set; }
        /// <summary>
        /// Gets or sets the Movie Genre ID
        /// </summary>
        [Singular.DataAnnotations.DropDownWeb(typeof(MELib.Basket.DeliveryList), UnselectedText = "Select", ValueMember = "DeliveryID", DisplayMember = "DeliveryType")]
        [Display(Name = "Delivery Options")]
        public int? DeliveryID { get; set; }

        public MyCartVM()
        {
        }

        protected override void Setup()
        {
            base.Setup();
            UserID = Singular.Security.Security.CurrentIdentity.UserID;

            UserCartList = UserCartList.GetUserCartList(UserCartID, UserID);
            DeliveryList = MELib.Basket.DeliveryList.GetDeliveryList();
            ProductList = MELib.Product.ProductList.GetProductList();
            AccountList = MELib.Accounts.AccountList.GetAccountList();
           TotalSum = UserCartList.GetUserCartList().Sum(x => x.SubTotal);
            ShoppingCartList = MELib.Basket.ShoppingCartList.GetShoppingCartList();

            TransactionList = MELib.Transactions.TransactionList.GetTransactionList();

        }

        //Updating the item
        [WebCallable]
        public Result UpdateItem(int UserCartID, MELib.Basket.UserCartList UserCartList)
        {
            Result sr = new Result();

            if (UserCartList.IsValid) 
            {
                foreach(var item in UserCartList)
                {
                    item.SubTotal = (decimal)(item.SellingPrice * item.ItemsCount);
                }

                var SaveResult = UserCartList.TrySave();
                if(SaveResult.Success)
                {
                    sr.Data = UserCartList;
                    sr.Success = true;
                }
                else
                {
                    sr.ErrorText = SaveResult.ErrorText;
                    sr.Success = false;
                }
                return sr;
            }
            else 
            {
                sr.ErrorText = UserCartList.GetErrorsAsHTMLString();
                return sr;
            }
             sr.Success = true;
            return sr;
        }

        //Saving the cart
        [WebCallable]
        public Result SaveCart(int UserCartID, MELib.Basket.UserCartList UserCartList)
        {
            Result sr = new Result();
            try
            {
                if (UserCartList.IsValid)
                {
                    var SaveResult = UserCartList.TrySave();
                    if(SaveResult.Success)
                    {
                        sr.Data = SaveResult.SavedObject;
                        sr.Success = true;
                    }
                    else
                    {
                        sr.ErrorText = SaveResult.ErrorText;
                        sr.Success = false;
                    }
                    return sr;
                }
                else
                {
                    sr.ErrorText = UserCartList.GetErrorsAsHTMLString();
                    return sr;
                }
            }

            catch (Exception ex)
            {
                //Return error message
                sr.Data = ex.InnerException;
                sr.Success = true;
            }
            return sr;
        }

        //Removing item from cart
        [WebCallable]
        public Result RemoveFromCart(int UserCartID)
        {
            //Removing items from Cart
            Result sr = new Result();
            try
            {
                if(UserCartID != null)
                {
                    UserCartList = UserCartList.GetUserCartList(UserCartID);
                    var temp = UserCartList.Single(x => x.UserCartID == UserCartID);
                    UserCartList.Remove(temp); //Not doing soft delete here basically removing the record on database
                    UserCartList.TrySave();
                    sr.Success = true;
                }
            }

            catch (Exception ex)
            {
                //Return error message
                sr.Data = ex.InnerException;
                sr.Success = true;
            }
            return sr;
        }

        //Checkout items from Cart
        [WebCallable]
        public Result Checkout(decimal TotalSum, UserCartList UserCartList, int DeliveryID)
        {
            Result sr = new Result();
            try
            { 
                UserBalance = MELib.Accounts.AccountList.GetAccountList().Select(x => x.Balance).FirstOrDefault();
                TotalSum = MELib.Basket.UserCartList.GetUserCartList().Sum(c => c.SubTotal);
                var BalanceCheckOut = UserBalance - TotalSum;
                MELib.Transactions.Transaction transaction = new MELib.Transactions.Transaction();

                if (UserBalance < TotalSum)
                {
                    //error message
                    return new Singular.Web.Result() { ErrorText = "Insuffient funds! Please make a deposit. ", Success = false };
                }
                else
                {
                    sr.ErrorText = "Balance greater";
                    AccountList = AccountList.GetAccountList();
                    AccountList.ToList().ForEach(f => f.Balance = BalanceCheckOut);
                    AccountList.TrySave();

                    UserCartList = UserCartList.GetUserCartList();
                    UserCartList.ToList().ForEach(c => { c.isActiveInd = false; });
                  //  UserCartList.ToList().ForEach(c => { c.DeliveryID = 2; });
                    UserCartList.TrySave();
                }

                MELib.Orders.Order Order = new MELib.Orders.Order();
                MELib.Orders.OrderList OrderList = new MELib.Orders.OrderList();

                Order.UserID = Singular.Security.Security.CurrentIdentity.UserID;
                Order.UserCartID = UserCartList.Select(c => c.UserCartID).FirstOrDefault();
                Order.DeliveryID = DeliveryID;
                decimal total;
                if (DeliveryID == 5)
                {
                    Order.TotalAmount = TotalSum + 60;
                    total = 60;
                }
                else
                {
                    Order.TotalAmount = TotalSum;
                    total = 0;

                }
                Order.IsActiveInd = true;
                OrderList.Add(Order);
                OrderList.TrySave();

                var getOrderList = MELib.Orders.OrderList.GetOrderList();

                transaction.UserID = Settings.CurrentUser.UserID;
                transaction.Amount = TotalSum + total;
                transaction.TransactionTypeID = 1;
                transaction.IsActiveInd = true;
                transaction.Description = "Purchase";

                // transaction.OrderID = Order.OrderID;
                transaction.OrderID = getOrderList.Select(c => c.OrderID).LastOrDefault();

                var transactionResult = transaction.TrySave(typeof(MELib.Transactions.TransactionList));

                if (transactionResult.Success)
                {
                    sr.Success = true;
                }

                return new Singular.Web.Result() { Success = true };
            }

            catch (Exception ex)
            {
                //Return error message
                sr.Data = ex.InnerException;
                sr.Success = true;
            }
            return sr;
        }
        
    }
}