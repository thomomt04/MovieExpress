
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;

namespace MEWeb.Examples
{
  public partial class KnockoutBindings : MEPageBase<KnockoutBindingsVM>
  {
  }
  public class KnockoutBindingsVM : MEStatelessViewModel<KnockoutBindingsVM>
  {
    public KnockoutBindingsVM()
    {

    }
    protected override void Setup()
    {
      base.Setup();
    }
  }
}