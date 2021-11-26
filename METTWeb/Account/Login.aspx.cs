using System.Web;
using System.Web.UI;
using Singular.Web;
using Singular.Web.Security;
using MELib.Security;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System;
using System.Linq;

namespace MEWeb.Account
{
	public partial class Login : MEPageBase<LoginVM>
	{
	}

	public class LoginVM : MEStatelessViewModel<LoginVM>
	{
		/// <summary>
		/// The login details
		/// </summary>
		public LoginDetails LoginDetails { get; set; }
		public MELib.Security.User EditingUser { get; set; }

		/// <summary>
		/// The location to redirect to after login
		/// </summary>
		public string RedirectLocation { get; set; }
		public bool ShowForgotPasswordInd { get; set; }

		[Display(Name = "Enter your email address", Description = "")]
		public string ForgotEmail { get; set; }
		/// <summary>
		/// Setup the Login ViewModel
		/// </summary>
		protected override void Setup()
		{
			base.Setup();

			this.LoginDetails = new LoginDetails();
			this.ShowForgotPasswordInd = false;
			this.ValidationMode = ValidationMode.OnSubmit;
			this.ValidationDisplayMode = ValidationDisplayMode.Controls;
			this.RedirectLocation = VirtualPathUtility.ToAbsolute(Security.GetSafeRedirectUrl(Page.Request.QueryString["ReturnUrl"], "~/default.aspx"));
		}
		/// <summary>
		/// Check the login details
		/// </summary>
		/// <param name="loginDetails">Login details</param>
		/// <returns>True if the login details are valid</returns>
		[WebCallable(LoggedInOnly = false)]
		public Result Login(LoginDetails loginDetails)
		{
			Result ret = new Result();

			try
			{
				MEIdentity.Login(loginDetails);
				ret.Success = true;
				if (MEWebSecurity.CurrentIdentity().FirstTimeLogin)
				{
					ret.Data = "ChangePassword.aspx";
				}
			}
			catch
			{
				ret.ErrorText = "";
				ret.Success = false;
			}

			return ret;
		}

		[WebCallable(LoggedInOnly = false)]
		public Result ResetPassword(string Email)
		{
			Result ret = new Result();
			try
			{
				MELib.Security.User.ResetPassword(Email);
				ret.Success = true;
			}
			catch (Exception ex)
			{
				ret.Success = false;
				ret.ErrorText = ex.Message;
			}
			return ret;
		}

		[WebCallable(LoggedInOnly = false)]
		public Result Register(LoginDetails loginDetails)
		{
			Result ret = new Result();

			try
			{
				MEIdentity.Login(loginDetails);
				ret.Success = true;
				if (MEWebSecurity.CurrentIdentity().FirstTimeLogin)
				{
					ret.Data = "ChangePassword.aspx";
				}
			}
			catch
			{
				ret.ErrorText = "";
				ret.Success = false;
			}

			return ret;
		}

	}
}
