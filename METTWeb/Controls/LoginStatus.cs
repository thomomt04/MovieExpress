using System.Web;
using System.Web.Security;
using Singular.Web;
using Singular.Web.Controls;
using Singular.Web.CustomControls;
using MELib.Security;

namespace MEWeb.CustomControls
{
  /// <summary>
  /// The LoginStatus control class
  /// </summary>
  public class LoginStatus : HelperControls.HelperBase<object>
  {
    /// <summary>
    /// Setup the login status control
    /// </summary>
    protected override void Setup()
    {
      base.Setup();

      HTMLDiv<object> container = Helpers.Div();
      container.Style.Display = Display.inlineblock;
      container.Style.MarginLeft("20px");

      string loginUrl = VirtualPathUtility.ToAbsolute(FormsAuthentication.LoginUrl);
      string defaultUrl = VirtualPathUtility.ToAbsolute(FormsAuthentication.DefaultUrl);

      // Logged in
      if (Singular.Security.Security.HasAuthenticatedUser)
      {
        MEIdentity identity = MEWebSecurity.CurrentIdentity();

        var loginStatus = container.Helpers.DivC("login-status");
        {
          loginStatus.Attributes["data-ContextMenu"] = "cmSecurity";

          // User label
          var userLabel = loginStatus.Helpers.HTMLTag("span", "Hello " + identity.FirstName);
          {
            userLabel.Style.Display = Display.inlineblock;
          }

          // Icon
          loginStatus.Helpers.Image().Glyph = Singular.Web.FontAwesomeIcon.user;

          // Context menu
          var contextMenu = loginStatus.Helpers.DivC("context-menu-ls");
          {
            contextMenu.Attributes["id"] = "cmSecurity";
            contextMenu.Style.TextAlign = Singular.Web.TextAlign.right;

            var contextMenuMain = contextMenu.Helpers.DivC("CM-Main");
            {
              var contextMenuMainHeader = contextMenuMain.Helpers.Div();
              {
                contextMenuMainHeader.AddClass("CM-Header");
                contextMenuMainHeader.Helpers.Div().Helpers.HTML(identity.UserNameReadable);
                contextMenuMainHeader.Helpers.Div().Helpers.HTML(identity.EmailAddress);
              }

              var contextMenuBody = contextMenuMain.Helpers.Div();
              {
                contextMenuBody.AddClass("Selectable");

								// Uncomment if you have an edit profile page
								var contextMenuEditProfile = contextMenuBody.Helpers.Div();
								{
									contextMenuEditProfile.Helpers.LinkFor(null, null, VirtualPathUtility.ToAbsolute("~/Users/UserProfile.aspx?UserID=" + HttpUtility.UrlEncode(Singular.Encryption.EncryptString(identity.UserID.ToString()))), "Edit Profile");
								}

								// Change password
								var contextMenuChangePassword = contextMenuBody.Helpers.Div();
                {
                  contextMenuChangePassword.Helpers.LinkFor(null, null, VirtualPathUtility.ToAbsolute("~/Account/ChangePassword.aspx"), "Change Password");
                }

                // Logout
                var contextMenuLogout = contextMenuBody.Helpers.Div();
                {
                  contextMenuLogout.Helpers.LinkFor(null, null, defaultUrl + "?SCmd=Logout", "Logout");
                }
              }
            }
          }
        }
      }
      else
      {
        // Logged out
        var loggedOutDiv = container.Helpers.Div();
        {
          loggedOutDiv.Style.FontSize = "14px";
          loggedOutDiv.Helpers.LinkFor(null, null, loginUrl, "Login").Style["text-decoration"] = "none";
          loggedOutDiv.Helpers.Image().Glyph = FontAwesomeIcon.user;
        }
      }
		}

    /// <summary>
    /// Render the login status control
    /// </summary>
		protected override void Render()
		{
			base.Render();

			RenderChildren();
		}
  }
}