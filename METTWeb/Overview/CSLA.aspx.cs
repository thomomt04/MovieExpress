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

namespace MEWeb.CSLAOverview
{
  public partial class CSLAOverview : MEPageBase<CSLAOverviewVM>
  {
    protected void Page_Load(object sender, EventArgs e)
    {
    }
  }
  public class CSLAOverviewVM : MEStatelessViewModel<CSLAOverviewVM>
  {
    //  public MELib.Documents.DocumentList DocumentList { get; set; }

    public CSLAOverviewVM()
    {

    }

    protected override void Setup()
    {
      base.Setup();

      //   DocumentList = MELib.Documents.DocumentList.GetDocumentList();

    }
  }
}

