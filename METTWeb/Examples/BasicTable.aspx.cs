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

namespace MEWeb.Examples
{

  public partial class BasicTable : MEPageBase<BasicTableVM>
  {

  }
  public class BasicTableVM : MEStatelessViewModel<BasicTableVM>
  {
    public MELib.Movies.MovieList MovieList { get; set; }

    public BasicTableVM()
    {

    }

    protected override void Setup()
    {
      base.Setup();
      MovieList = MELib.Movies.MovieList.GetMovieList();
    }
  }
}