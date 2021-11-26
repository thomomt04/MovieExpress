using System;
using Singular.CommonData;

namespace MELib
{
  public class CommonData : CommonDataBase<MELib.CommonData.MECachedLists>
  {
    [Serializable]
    public class MECachedLists : CommonDataBase<MECachedLists>.CachedLists
    {
      /// <summary>
      /// Gets cached ROUserList
      /// </summary>
      public MELib.RO.ROUserList ROUserList
      {
        get
        {
          return RegisterList<MELib.RO.ROUserList>(Misc.ContextType.Application, c => c.ROUserList, () => { return MELib.RO.ROUserList.GetROUserList(); });
        }
      }
      /// <summary>
      /// Gets cached ROMovieGenreList
      /// </summary>
      public RO.ROMovieGenreList ROMovieGenreList
      {
        get
        {
          return RegisterList<MELib.RO.ROMovieGenreList>(Misc.ContextType.Application, c => c.ROMovieGenreList, () => { return MELib.RO.ROMovieGenreList.GetROMovieGenreList(); });
        }
      }      
      
      /// <summary>
      /// Gets cached ROMovieGenreList
      /// </summary>
      public RO.ROProductCategoryList ROProductCategoryList
            {
        get
        {
          return RegisterList<MELib.RO.ROProductCategoryList>(Misc.ContextType.Application, c => c.ROProductCategoryList, () => { return MELib.RO.ROProductCategoryList.GetROProductCategoryList(); });
        }
      }

    public Maintenance.ProductCategoryList ProductCategoryList            {
         get
         {
           return RegisterList<MELib.Maintenance.ProductCategoryList>(Misc.ContextType.Application, c => c.ProductCategoryList, () => { return MELib.Maintenance.ProductCategoryList.GetProductCategoryList(); });
         }
       }


     public Maintenance.CategoryList CategoryList
      {
       get
         {
          return RegisterList<MELib.Maintenance.CategoryList>(Misc.ContextType.Application, c => c.CategoryList, () => { return MELib.Maintenance.CategoryList.GetCategoryList(); });
         }
     }

    //Adding cached ROProductList

           public RO.ROProductList ROProductList 
            {
                get
                {
                    return RegisterList<MELib.RO.ROProductList>(Misc.ContextType.Application, c => c.ROProductList, () => { return MELib.RO.ROProductList.GetROProductList(); });
                }
            }

            public  Basket.DeliveryList DeliveryList
            {
                get
                {
                    return RegisterList<MELib.Basket.DeliveryList>(Misc.ContextType.Application, c => c.DeliveryList, () => { return MELib.Basket.DeliveryList.GetDeliveryList(); });
                }
            }

            public RO.RODeliveryList RODeliveryList
            {
                get
                {
                    return RegisterList<MELib.RO.RODeliveryList>(Misc.ContextType.Application, c => c.RODeliveryList, () => { return MELib.RO.RODeliveryList.GetRODeliveryList(); });
                }
            }

            public Transactions.TransactionTypeList TransactionTypeList
            {
                get
                {
                    return RegisterList<MELib.Transactions.TransactionTypeList>(Misc.ContextType.Application, c => c.TransactionTypeList, () => { return MELib.Transactions.TransactionTypeList.GetTransactionTypeList(); });
                }
            }
        }
  }

  public class Enums
  {
		public enum AuditedInd
		{
			Yes = 1,
			No = 0
		}
    public enum DeletedInd
    {
      Yes = 1,
      No = 0
    }
  }
}
