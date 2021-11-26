using Singular.Web;

namespace MEWeb
{
  /// <summary>
  /// The MEPageBase class
  /// </summary>
  /// <typeparam name="VM"></typeparam>
  public class MEPageBase<VM>: PageBase<VM> where VM: IViewModel
  {
    protected override void OnInit(System.EventArgs e)
    {
      base.OnInit(e);

      if (MELib.Security.MEWebSecurity.HasAuthenticatedUser())
      {
        if (MELib.Security.MEWebSecurity.CurrentIdentity().ResetState == MELib.Security.ResetState.MustResetPassword)
        {
          Singular.Web.Misc.NavigationHelper.RedirectAndRemember("~/Account/ChangePassword.aspx?WasReset=true");
        }
      }
    }
  }
}
