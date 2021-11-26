using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;

namespace MEWeb.Profile
{
  public partial class Profile : MEPageBase<ProfileVM>
  {
  }
  public class ProfileVM : MEStatelessViewModel<ProfileVM>
  {
    public ProfileVM()
    {

    }
    protected override void Setup()
    {
      base.Setup();
    }
  }
}

