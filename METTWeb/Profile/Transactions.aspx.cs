using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;

namespace MEWeb.Profile
{
    public partial class Transactions : MEPageBase<TransactionsVM>
    {
    }
    public class TransactionsVM : MEStatelessViewModel<TransactionsVM>
    {
        public MELib.Transactions.TransactionList TransHistory { get; set; }
        public MELib.Transactions.TransactionTypeList TransactionTypeList { get; set; }

        [Singular.DataAnnotations.DropDownWeb(typeof(MELib.Transactions.TransactionTypeList), UnselectedText = "Select", ValueMember = "TransactionTypeID", DisplayMember = "TransactionTypeName")]
        [Display(Name = "Transaction Type")]
        public int? TransactionTypeID { get; set; }
        public TransactionsVM()
        {

        }
        protected override void Setup()
        {
            base.Setup();
            TransHistory = MELib.Transactions.TransactionList.GetTransactionList();
            TransactionTypeList = MELib.Transactions.TransactionTypeList.GetTransactionTypeList();
        }

        [WebCallable]
        public Result FilterTransactions(int TransactionTypeID)
        {
            Result sr = new Result();
            try
            {
                if (TransactionTypeID != 0)
                {
                    sr.Data = MELib.Transactions.TransactionList.GetTransactionList(TransactionTypeID).Where(x => x.TransactionTypeID == TransactionTypeID);
                    sr.Success = true;
                }
                else
                {
                    sr.Data = MELib.Transactions.TransactionList.GetTransactionList();
                    sr.Success = true;
                }
            }
            catch (Exception e)
            {
                WebError.LogError(e, "Page: Transactions.aspx | Method: FilterTransactions", $"(int TransactionTypeID, ({TransactionTypeID})");
                sr.Data = e.InnerException;
                sr.ErrorText = "Could not filter by transaction type.";
                sr.Success = false;
            }
            return sr;
        }
    }
}

