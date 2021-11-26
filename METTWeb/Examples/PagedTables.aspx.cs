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

namespace MEWeb.Tables
{
  public partial class tables : MEPageBase<TablesVM>
  {
    protected void Page_Load(object sender, EventArgs e)
    {
    }
  }
  public class TablesVM : MEStatelessViewModel<TablesVM>
  {
    public MELib.Movies.MovieList MovieList { get; set; }

    public TablesVM()
    {

    }

    protected override void Setup()
    {
      base.Setup();
      MovieList = MELib.Movies.MovieList.GetMovieList();
    }
  }
}




