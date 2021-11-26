using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MELib;


namespace MEWeb.Examples
{

  public partial class PageCards : MEPageBase<PageCardsVM>
  {

  }
  public class PageCardsVM : MEStatelessViewModel<PageCardsVM>
  {

    public PageCardsVM()
    {

    }

    protected override void Setup()
    {
      base.Setup();
    }
  }
}


