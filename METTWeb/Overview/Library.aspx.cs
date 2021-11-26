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

namespace MEWeb.Overview
{
  public partial class SingularLibrary : MEPageBase<SingularLibraryVM>
  {
    protected void Page_Load(object sender, EventArgs e)
    {
    }
  }
  public class SingularLibraryVM : MEStatelessViewModel<SingularLibraryVM>
  {
    //  public MELib.Documents.DocumentList DocumentList { get; set; }

    public SingularLibraryVM()
    {

    }

    protected override void Setup()
    {
      base.Setup();

      //   DocumentList = MELib.Documents.DocumentList.GetDocumentList();

    }
  }
}

