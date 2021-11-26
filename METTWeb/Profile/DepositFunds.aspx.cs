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
using MELib.Accounts;

namespace MEWeb.Profile
{
  public partial class DepositFunds : MEPageBase<DepositFundsVM>
  {
  }
  public class DepositFundsVM : MEStatelessViewModel<DepositFundsVM>
  {
    public MELib.Accounts.AccountList DepositAccount { get; set; }
    public MELib.Accounts.Account Deposit { get; set; }
    
    public MELib.Transactions.TransactionList TransactionList { get; set; }
    public MELib.Transactions.Transaction Transactions { get; set; }
    public int? AccountID { get; set; }
    public int? TransactionID { get; set; }
   
    public decimal DepositAmount { get; set; }
    public DepositFundsVM()
    {

    }
    protected override void Setup()
    {
      base.Setup();
      DepositAccount = MELib.Accounts.AccountList.GetAccountList();
      Deposit = DepositAccount.FirstOrDefault();

     TransactionList= MELib.Transactions.TransactionList.GetTransactionList();
     DepositAmount = Deposit.Balance;
     }

        [WebCallable]
        public Result DepositFunds (AccountList Account)
        {
            Result sr = new Result();
            var newBalance = MELib.Accounts.AccountList.GetAccountList(Singular.Security.Security.CurrentIdentity.UserID).FirstOrDefault();
            newBalance.UserID = Singular.Security.Security.CurrentIdentity.UserID;

            MELib.Transactions.Transaction transaction = new MELib.Transactions.Transaction();
            
            Transactions temp = new Transactions();
            transaction.UserID = Settings.CurrentUser.UserID;

            decimal DepositAmount = Account.FirstOrDefault().Balance;
            string BalanceText = Account.FirstOrDefault().Balance.ToString();
            bool parseSuccess = decimal.TryParse(BalanceText, out DepositAmount);


            //When less or equal to 0
            if (Account.FirstOrDefault().Balance <= 0)
            {
                return new Singular.Web.Result() { ErrorText = "The amount should be greater than 0 ", Success = false };
             }
            else
            {
                newBalance.Balance += Account.FirstOrDefault().Balance;
                newBalance.TrySave(typeof(AccountList));

                transaction.Amount = Account.FirstOrDefault().Balance;
                transaction.TransactionTypeID = 2;
                transaction.Description = "Deposit";
                transaction.TrySave(typeof(MELib.Transactions.TransactionList));
                sr.Success = true;
                return new Singular.Web.Result() { Success = true };
            }   
        }
    }
}