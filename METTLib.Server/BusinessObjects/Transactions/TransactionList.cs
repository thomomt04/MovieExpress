﻿// Generated 17 Nov 2021 12:11 - Singular Systems Object Generator Version 2.2.694
//<auto-generated/>
using System;
using Csla;
using Csla.Serialization;
using Csla.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Singular;
using System.Data;
using System.Data.SqlClient;


namespace MELib.Transactions
{
    [Serializable]
    public class TransactionList
     : MEBusinessListBase<TransactionList, Transaction>
    {
        #region " Business Methods "

        public Transaction GetItem(int TransactionID)
        {
            foreach (Transaction child in this)
            {
                if (child.TransactionID == TransactionID)
                {
                    return child;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return "Transactions";
        }

        #endregion

        #region " Data Access "

        [Serializable]
        public class Criteria
          : CriteriaBase<Criteria>
        {
           
            public Criteria()
            {
            }

            public int? TransactionTypeID { get; set; }

        }

        public static TransactionList NewTransactionList()
        {
            return new TransactionList();
        }

        public TransactionList()
        {
            // must have parameter-less constructor
        }

        public static TransactionList GetTransactionList()
        {
            return DataPortal.Fetch<TransactionList>(new Criteria());
        }

        public static TransactionList GetTransactionList(int? TransactionTypeID)
        {
            return DataPortal.Fetch<TransactionList>(new Criteria() { TransactionTypeID = TransactionTypeID });
        }

        protected void Fetch(SafeDataReader sdr)
        {
            this.RaiseListChangedEvents = false;
            while (sdr.Read())
            {
                this.Add(Transaction.GetTransaction(sdr));
            }
            this.RaiseListChangedEvents = true;
        }

        protected override void DataPortal_Fetch(Object criteria)
        {
            Criteria crit = (Criteria)criteria;
            using (SqlConnection cn = new SqlConnection(Singular.Settings.ConnectionString))
            {
                cn.Open();
                try
                {
                    using (SqlCommand cm = cn.CreateCommand())
                    {
                        cm.CommandType = CommandType.StoredProcedure;
                        cm.CommandText = "GetProcs.getTransactionList";
                        cm.Parameters.AddWithValue("@UserID", Singular.Security.Security.CurrentIdentity.UserID);
                        cm.Parameters.AddWithValue("@TransactionTypeID", Singular.Misc.NothingDBNull(crit.TransactionTypeID));
                        using (SafeDataReader sdr = new SafeDataReader(cm.ExecuteReader()))
                        {
                            Fetch(sdr);
                        }
                    }
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        #endregion

    }

}