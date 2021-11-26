using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;namespace MEWeb.Movies
{
    public partial class Movie : MEPageBase<MovieVM>
    {
    }
    public class MovieVM : MEStatelessViewModel<MovieVM>
    {
        public MELib.Movies.MovieList MovieList { get; set; }
        public MELib.Movies.UserMovieList UserMovieList { get; set; }
        public int MovieID { get; set; }
        public MovieVM()
        { }
        protected override void Setup()
        {
            base.Setup();
            MovieID = System.Convert.ToInt32(Page.Request.QueryString[0]);
            UserMovieList = MELib.Movies.UserMovieList.GetUserMovieList();
            MovieList = MELib.Movies.MovieList.GetMovieList(null, MovieID);
        }
        public Result RentNow(int MovieID)
        {
            Result sr = new Result();
            try
            {
                // ToDo Check User Balance
                decimal Price;
                var AccBalance = MELib.Accounts.AccountList.GetAccountList().Select(c => c.Balance).FirstOrDefault();
                Price = MELib.Movies.MovieList.GetMovieList().Where(c => c.MovieID == MovieID).Select(c => c.Price).FirstOrDefault();
                var NewBalance = AccBalance - Price;
                
                if (AccBalance >= Price)
                {
                    var newBalance = MELib.Accounts.AccountList.GetAccountList(Singular.Security.Security.CurrentIdentity.UserID).FirstOrDefault();
                    newBalance.UserID = Singular.Security.Security.CurrentIdentity.UserID;
                    newBalance.Balance = NewBalance;
                    newBalance.TrySave(typeof(MELib.Accounts.AccountList)); // ToDo Insert Data in Transctions
                    MELib.Transactions.Transaction Transaction = new MELib.Transactions.Transaction();
                    MELib.Transactions.TransactionList TransactionList = new MELib.Transactions.TransactionList(); 
                    Transaction.UserID = Singular.Security.Security.CurrentIdentity.UserID;
                    Transaction.TransactionTypeID = 5;
                    Transaction.Description = "Rent Movie";
                    Transaction.Amount = Price;
                    Transaction.IsActiveInd = true;
                    Transaction.TrySave(typeof(MELib.Transactions.TransactionList));
                    sr.Success = true;
                    return sr;
                }
                else
                {
                    sr.Success = false;
                    return sr;
                }
            }
            catch (Exception e)
            {
                sr.Data = e.InnerException;
                sr.Success = false;
                return sr;
            }
        }
    }
}

