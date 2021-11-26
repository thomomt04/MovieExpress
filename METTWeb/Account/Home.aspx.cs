using System;
using Singular.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web.Data;
using MELib.RO;
using MELib.Security;
using Singular;
using MEWeb;

namespace MEWeb.Account
{
    public partial class Home : MEPageBase<HomeVM>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
    public class HomeVM : MEStatelessViewModel<HomeVM>
    {
        // Declare your page variables/properties here
        public bool FoundUserMoviesInd { get; set; }
        public MELib.Movies.UserMovieList UserMovieList { get; set; }

        public MELib.Accounts.AccountList UserAccount1List { get; set; }
       public MELib.Accounts.Account UserAccount1 { get; set; }
       public MELib.RO.ROUserList UserAccountList { get; set; }
        public MELib.RO.ROUser UserAccount { get; set; }
        public int UserID { get; set; }

        public HomeVM()
        {

        }

        protected override void Setup()
        {
            base.Setup();

            UserID = Singular.Security.Security.CurrentIdentity.UserID;
            // On page load initiate/set your data/variables and or properties here
            // Should pass in criteria for the specific user that is viewing the page, however using current identity
            UserMovieList = MELib.Movies.UserMovieList.GetUserMovieList();

            UserAccount1List = MELib.Accounts.AccountList.GetAccountList(UserID);
            UserAccountList = MELib.RO.ROUserList.GetROUserList(UserID);

            UserAccount1 = UserAccount1List.FirstOrDefault();
            UserAccount = UserAccountList.GetItem(UserID);
            if (UserMovieList.Count() > 0)
            {
                FoundUserMoviesInd = true;
            }
            else
            {
                FoundUserMoviesInd = false;
            }
             
            UserID = Singular.Security.Security.CurrentIdentity.UserID;

        }

        // Place your page's WebCallable methods here

        // Example WebCallable Method called GetSomeData layout/structure

        /// <summary>
        /// This is a very basic example of a WebCallable method
        /// </summary>
        /// <param name="SomeReferenceID"></param>
        /// <returns></returns>
        [Singular.Web.WebCallable(LoggedInOnly = true)]
        public static Singular.Web.Result GetSomeData(int SomeReferenceID)
        {
            Result sr = new Result();
            try
            {
                // Perform some action here and return the result
               
                sr.Success = true;
            }
            catch (Exception e)
            {
                sr.Data = e.InnerException;
                sr.Success = false;
            }
            return sr;
        }
    }
}


