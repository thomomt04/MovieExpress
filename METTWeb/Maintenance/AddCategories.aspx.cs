using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Singular.Web;
using static MEWeb.Maintenance.AddCategories;

namespace MEWeb.Maintenance
{
    public partial class AddCategories : MEPageBase<AddCategoriesVM>
    {
      
        public class AddCategoriesVM : MEStatelessViewModel<AddCategoriesVM>
        {
            public MELib.Maintenance.ProductCategoryList ProductCategoryList { get; set; }
            public AddCategoriesVM()
            {
            }
            protected override void Setup()
            {
                base.Setup();
                ProductCategoryList = MELib.Maintenance.ProductCategoryList.GetProductCategoryList();
            }

            [WebCallable]
            public Result SaveProductCategoryList(MELib.Maintenance.ProductCategoryList ProductCategoryList)
            {
                Result sr = new Result();
                if (ProductCategoryList.IsValid)
                {
                    var SaveResult = ProductCategoryList.TrySave();
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
                    sr.ErrorText = ProductCategoryList.GetErrorsAsHTMLString();
                    return sr;
                }
            }
        }
    }

}
    


    
    