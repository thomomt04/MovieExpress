using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;

namespace MEWeb.Movies
{
  public partial class LatestReleases : MEPageBase<LatestReleasesVM>
  {
  }
  public class LatestReleasesVM : MEStatelessViewModel<LatestReleasesVM>
  {
    public MELib.Movies.MovieList MovieList { get; set; }

    // Filter Criteria
    public String MovieTitle { get; set; }
    public DateTime ReleaseFromDate { get; set; }
    public DateTime ReleaseToDate { get; set; }

    /// <summary>
    /// Gets or sets the Movie Genre ID
    /// </summary>
    [Singular.DataAnnotations.DropDownWeb(typeof(MELib.RO.ROMovieGenreList), UnselectedText = "Select", ValueMember = "MovieGenreID", DisplayMember = "Genre")]
    [Display(Name = "Genre")]
    public int? MovieGenreID { get; set; }

    public LatestReleasesVM()
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
      var url = $"../Movies/Movie.aspx?MovieId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(MovieID.ToString()))}";
      return url;
    }

    [WebCallable]
    public Result FilterMovies(int MovieGenreID, int ResetInd)
    {
      Result sr = new Result();
      try
      {
        if (ResetInd == 0)
        {
          MELib.Movies.MovieList MovieList = MELib.Movies.MovieList.GetMovieList(MovieGenreID);
          sr.Data = MovieList;
        }
        else
        {
          MELib.Movies.MovieList MovieList = MELib.Movies.MovieList.GetMovieList();
          sr.Data = MovieList;
        }
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