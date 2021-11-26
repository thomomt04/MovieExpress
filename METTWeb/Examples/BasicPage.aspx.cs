using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;

namespace MEWeb.Examples
{
  public partial class BasicPage : MEPageBase<BasicPageVM>
  {
  }
  public class BasicPageVM : MEStatelessViewModel<BasicPageVM>
  {
    public BasicPageVM()
    {

    }
    protected override void Setup()
    {
      base.Setup();
    }
  }
}