using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Singular.Web;

namespace MEWeb.Movies
{
  public partial class Movies : MEPageBase<MoviesVM>
  {
  }
  public class MoviesVM : MEStatelessViewModel<MoviesVM>
  {
    public MELib.Movies.MovieList MovieList { get; set; }
       
        public MELib.Movies.UserMovieList UserMovieList { get; set; }

        public int MovieID { get; set; }

        // Filter Criteria
        public DateTime ReleaseFromDate { get; set; }
    public DateTime ReleaseToDate { get; set; }

    /// <summary>
    /// Gets or sets the Movie Genre ID
    /// </summary>
    [Singular.DataAnnotations.DropDownWeb(typeof(MELib.RO.ROMovieGenreList), UnselectedText = "Select", ValueMember = "MovieGenreID", DisplayMember = "Genre")]
    [Display(Name = "Genre")]
    public int? MovieGenreID { get; set; }

    public MoviesVM()
    {

    }
    protected override void Setup()
    {
      base.Setup();
           
            MovieList = MELib.Movies.MovieList.GetMovieList();

        }

    [WebCallable(LoggedInOnly = true)]
    public string RentMovie(int MovieID)
    {
    
      var url = VirtualPathUtility.ToAbsolute("~/Movies/Movie.aspx?MovieID=" + HttpUtility.UrlEncode((MovieID.ToString())));
            return url;
    }

    [WebCallable]
    public static Result WatchMovie(int MovieID)
    {
      Result sr = new Result();
      try
      {

        // ToDo Check User Balance
        // ToDo Insert Data in Transctions

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
    public Result FilterMovies(int MovieGenreID)
    {
      Result sr = new Result();
      try
      {
        sr.Data = MELib.Movies.MovieList.GetMovieList(MovieGenreID);
        sr.Success = true;
      }
      catch (Exception e)
      {
        WebError.LogError(e, "Page: LatestReleases.aspx | Method: FilterMovies", $"(int MovieGenreID, ({MovieGenreID})");
        sr.Data = e.InnerException;
        sr.ErrorText = "Could not filter movies by category.";
        sr.Success = false;
      }
      return sr;
    }

  }
}

