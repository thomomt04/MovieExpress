using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular;
using Singular.Web;
using System.ComponentModel.DataAnnotations;

namespace MEWeb.Maintenance
{
    public partial class AddProducts : MEPageBase<AddProductsVM>
    { }
    public class AddProductsVM : MEStatelessViewModel<AddProductsVM>
    {
        public MELib.Maintenance.ProductList ProductList { get; set; }

        // Filter Criteria
        public DateTime ReleaseFromDate { get; set; }
        public DateTime ReleaseToDate { get; set; }
        /// <summary>
        /// Gets or sets the Movie Genre ID
        /// </summary>
        
        public int? ProductID { get; set; }

        public AddProductsVM()
        {
        }
        protected override void Setup()
        {
            base.Setup();
            ProductList = MELib.Maintenance.ProductList.GetProductList();
        }

        [WebCallable]
        public Result SaveProduct(MELib.Maintenance.ProductList ProductList)
        {
            Result sr = new Result();
            if (ProductList.IsValid)
            {
                var SaveResult = ProductList.TrySave();
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
                return sr;
            }
            else
            {
                sr.ErrorText = ProductList.GetErrorsAsHTMLString();
                return sr;
            }
        }
    }
}