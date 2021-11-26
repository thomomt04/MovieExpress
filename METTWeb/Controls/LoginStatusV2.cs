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
    public class LoginStatusV2 : HelperControls.HelperBase<object>
    {
        /// <summary>
        /// Setup the login status control
        /// </summary>
        protected override void Setup()
        {
            base.Setup();

            HTMLTag<object> listItem = Helpers.HTMLTag("li");
            listItem.AddClass("dropdown");

            string loginUrl = VirtualPathUtility.ToAbsolute(FormsAuthentication.LoginUrl);
            string defaultUrl = VirtualPathUtility.ToAbsolute(FormsAuthentication.DefaultUrl);

            // Logged in
            if (Singular.Security.Security.HasAuthenticatedUser)
            {
				MEIdentity identity = MEWebSecurity.CurrentIdentity();

                var aTagUserName = listItem.Helpers.HTMLTag("a");
                aTagUserName.AddClass("dropdown-toggle count-info");
                aTagUserName.Attributes["data-toggle"] = "dropdown";
                {
                    aTagUserName.Helpers.HTML(MELib.CommonData.Lists.ROUserList.GetItem(identity.UserID).FullName);
                    var iTagUserName = aTagUserName.Helpers.HTMLTag("i");
                    iTagUserName.AddClass("fa fa-angle-down fa-lg");
                }

                var ulTagDropDown = listItem.Helpers.HTMLTag("ul");
                ulTagDropDown.AddClass("dropdown-menu animated fadeInRight");
                {
										var liTagEditProfile = ulTagDropDown.Helpers.HTMLTag("li");
										{
											var aTagEditProfile = liTagEditProfile.Helpers.HTMLTag("a");
											aTagEditProfile.Attributes["href"] = VirtualPathUtility.ToAbsolute("~/Users/UserProfile.aspx?UserID=" + HttpUtility.UrlEncode(Singular.Encryption.EncryptString(identity.UserID.ToString())));
											{
												var iTagEditProfile = aTagEditProfile.Helpers.HTMLTag("i");
												iTagEditProfile.AddClass("fa fa-user pad_5_right");
											}
											aTagEditProfile.Helpers.HTML("Edit Profile");
										}

										var liDivider = ulTagDropDown.Helpers.HTMLTag("li");
										liDivider.AddClass("divider");

										var liTagChangePassword = ulTagDropDown.Helpers.HTMLTag("li");
                    {
                        var aTagChangePassword = liTagChangePassword.Helpers.HTMLTag("a");
                        aTagChangePassword.Attributes["href"] = VirtualPathUtility.ToAbsolute("~/Account/ChangePassword.aspx");
                        {
                            var iTagChangePassword = aTagChangePassword.Helpers.HTMLTag("i");
                            iTagChangePassword.AddClass("fa fa-lock pad_5_right");
                        }
                        aTagChangePassword.Helpers.HTML("Change Password");
                    }

                    var liDivider1 = ulTagDropDown.Helpers.HTMLTag("li");
                    liDivider1.AddClass("divider");

                    var liTagLogout = ulTagDropDown.Helpers.HTMLTag("li");
                    {
                        var aTagLogout = liTagLogout.Helpers.HTMLTag("a");
                        aTagLogout.Attributes["href"] = defaultUrl + "?SCmd=Logout";
                        {
                            var iTagLogout = aTagLogout.Helpers.HTMLTag("i");
                            iTagLogout.AddClass("fa fa-sign-out pad_5_right");
                        }
                        aTagLogout.Helpers.HTML("Log Out");
                    }
                }
            }
            else
            {
                // Logged out
                var aLogin = listItem.Helpers.HTMLTag("a");
                {
                    var iTagUserName = aLogin.Helpers.HTMLTag("i");
                    iTagUserName.AddClass("fa fa-sign-in fa-lg");
                    aLogin.Attributes["href"] = loginUrl;
                    aLogin.Helpers.HTML("Log In");
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