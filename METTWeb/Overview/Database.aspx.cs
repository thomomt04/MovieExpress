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
  public partial class Database : MEPageBase<DatabaseVM>
  {
    protected void Page_Load(object sender, EventArgs e)
    {
    }
  }
  public class DatabaseVM : MEStatelessViewModel<DatabaseVM>
  {
    //  public MELib.Documents.DocumentList DocumentList { get; set; }

    public DatabaseVM()
    {

    }

    protected override void Setup()
    {
      base.Setup();

      //   DocumentList = MELib.Documents.DocumentList.GetDocumentList();

    }
  }
}

