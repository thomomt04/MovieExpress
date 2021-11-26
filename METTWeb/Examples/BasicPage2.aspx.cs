using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;

namespace MEWeb.Examples
{
    public partial class BasicPage2 : MEPageBase<BasicPageVM>
    { 
    }

    public class BasicPage2VM : MEStatelessViewModel<BasicPageVM>
    {
        public BasicPage2VM()
        { 
        
        }

        protected override void Setup()
        {
            base.Setup();
        }

    }
}

/*protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
*/