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

  public partial class BasicForms : MEPageBase<BasicFormsVM>
  {

  }
  public class BasicFormsVM : MEStatelessViewModel<BasicFormsVM>
  {
    public MELib.Movies.Movie EditMovie { get; set; }

    public BasicFormsVM()
    {
    }
    protected override void Setup()
    {
      base.Setup();
      EditMovie = MELib.Movies.MovieList.GetMovieList().FirstOrDefault();
    }

    [WebCallable]
    public static Result GetMovie(int MovieID)
    {
      Result sr = new Result();
      try
      {
        //MELib.Movies.Movie EditMovie = MELib.Movies.MovieList.GetMovieList(MovieID).FirstOrDefault();
        //sr.Data = EditMovie;
        sr.Success = true;
      }
      catch (Exception e)
      {
        sr.Data = e.InnerException;
        sr.Success = false;
      }
      return sr;
    }

    [WebCallable]
    public static Result DeleteMovie(int MovieID)
    {
      Result sr = new Result();
      try
      {
        //MELib.Movies.Movie MovieToRemove = MELib.Movies.MovieList.GetMovieList().FirstOrDefault(c => c.MovieID == MovieID);
        //MovieToRemove.IsActiveInd = false;
        //MovieToRemove.TrySave();
        // We cannot save object directly need to add it to a list...


        MELib.Movies.MovieList MovieList = MELib.Movies.MovieList.GetMovieList(MovieID);
        MovieList.ToList().ForEach(c => { c.IsActiveInd = false; });
        MovieList.TrySave();

     //   MovieToRemove.IsActiveInd = false;
     // MovieToRemove.TrySave();


        sr.Success = true;
      }
      catch (Exception e)
      {
        sr.Data = e.InnerException;
        sr.Success = false;
      }
      return sr;
    }
  }

}


