using System;
using System.Data.SqlClient;
using Singular;
using Singular.Emails;
using Singular.Security;
using Singular.Rules;
using Csla;
using Csla.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Csla.Core;
using Singular.Web;
using System.Configuration;
using System.Linq;

namespace MELib.Security
{
  [Serializable]
  public class User: UserBase<User>
  {
    #region Properties

    /// <summary>
    /// Gets the parameter name for UserName
    /// </summary>
    protected override string UserNameParamName
    {
      get
      {
        return "@UserName";
      }
    }

    /// <summary>
    /// Gets the parameter name for Surname
    /// </summary>
    protected override string SurnameParamName
    {
      get
      {
        return "@LastName";
      }
    }

    /// <summary>
    /// Gets the Can Update UserName flag
    /// (NOTE: If you want to make the username updateable, you will need to update the updUser Stored Proc to accept a Username parameter (Same ordering as insUser)
    /// </summary>
    //protected override bool CanUpdateUserName
    //{
    //  get
    //  {
    //    return false;
    //  }
    //}

		public override string FirstName {
			get => base.FirstName;
			set => base.FirstName = value; }

		public static PropertyInfo<string> ContactNumberProperty = RegisterProperty<string>(c => c.ContactNumber, "Contact Number", null);
		public string ContactNumber
		{
			get { return GetProperty(ContactNumberProperty); }
			set { SetProperty(ContactNumberProperty, value); }
		}

		public static PropertyInfo<string> JobDescriptionProperty = RegisterProperty<string>(c => c.JobDescription, "Job Description", string.Empty);
		/// <summary>
		/// Gets and sets the Job Description value
		/// </summary>
		[Display(Name = "Job Description", Description = "")]
		public string JobDescription
		{
			get { return GetProperty(JobDescriptionProperty); }
			set { SetProperty(JobDescriptionProperty, value); }
		}

		#endregion

		#region Members

		/// <summary>
		/// Adds rules to enforce Email Address entry and detect if a Username already exists
		/// </summary>
		protected override void AddCustomPasswordRule()
    {
      // Add custom password rule
      this.AddWebRule(EmailAddressProperty, u => u.EmailAddress == "", u => "Email address is required");
			this.AddWebRule(FirstNameProperty, u => u.FirstName == "", u => "First name is required");

			JavascriptRule<User> rule = AddWebRule(User.LoginNameProperty, u => u.LoginName == "", u => "");
      rule.AsyncBusyText = "Checking rule...";
      rule.ServerRuleFunction = (user) =>
      {
        CommandProc commandProc = new CommandProc("CmdProcs.cmdCheckUserExists", new string[] { "@UserID", "@UserName" }, new object[] { user.UserID, user.LoginName }, CommandProc.FetchTypes.DataRow);
        if (commandProc.Execute().DataRowValue != null)
        {
          return "There is already a user in the system with this email address, please use a different email account to proceed.";// "A user with this name already exists.";
        }
        else
        {
          return "";
        }
      };
    }

    // Instructions to allow passwords to be changed in Edit User dialog.
    // 1. Remove the InsertUpdate method which generates a random password
    // 2. Uncomment the first part of UpdProcs.updUser

    /// <summary>
    /// Inserts or updates a user
    /// </summary>
    /// <param name="command">A SQL Command instance</param>
    protected override void InsertUpdate(SqlCommand command)
    {
      Boolean sendEmail = false;
      String tempPassword = Singular.Misc.Password.CreateRandomEasyPassword(8);

      if (this.IsNew)
      {
        sendEmail = true;
        this.Password = tempPassword;
      }

			base.InsertUpdate(command);

      if (sendEmail)
      {

				var WebsiteURL = ConfigurationManager.AppSettings["WebsiteURL"].ToString();

				Singular.Emails.EMailBuilder.Create(this.EmailAddress, "Movie Express User Created")
          .AddHeading("Movie Express User Created")
          .AddParagraph("Please note, your user for Movie Express has been created.")
          .AddParagraph("User Name: " + this.LoginName + "<br/>Password: " + tempPassword)
          .AddParagraph("Please log in as soon as possible with the above temporary password. You will be required to change your password the first time you log in.")
					.AddParagraph("Website: " + WebsiteURL)
					.AddRegards()
          .Save();
      }
    }

    /// <summary>
    /// Adds the hashed password to the given SQL parameters list
    /// </summary>
    /// <param name="parameters">SQL Parameters list</param>
    protected override void AddExtraParameters(ref SqlParameterCollection parameters)
    {
      base.AddExtraParameters(ref parameters);

      var password = parameters["@Password"].Value;
      if (password != DBNull.Value) {
	        parameters["@Password"].Value = MEWebSecurity.GetPasswordHash(password.ToString());

			}

			var FirstName = parameters["@FirstName"].Value;
			if (FirstName != DBNull.Value)
			{
				parameters["@FirstName"].Value = Convert.FromBase64String(Encryption.EncryptString(FirstName.ToString()));
			}

			var Surname = parameters[SurnameParamName].Value;
			if (Surname != DBNull.Value)
			{
				parameters[SurnameParamName].Value = Convert.FromBase64String(Encryption.EncryptString(Surname.ToString()));
			}

			parameters.AddWithValue("ContactNumber", Singular.Misc.NothingDBNull(GetProperty(ContactNumberProperty)));
			parameters.AddWithValue("JobDescription", Singular.Misc.NothingDBNull(GetProperty(JobDescriptionProperty)));

		}

    /// <summary>
    /// Reset a user's password
    /// </summary>
    /// <param name="userName">The user's username</param>
    public static void ResetPassword(string Email)
    {
			var ParameterArray = new string[] { "@EmailAddress" };
			var ValuesArray = new string[] { Email };
			Singular.CommandProc cProc = new Singular.CommandProc("[CmdProcs].[cmdTempPassword]", ParameterArray, ValuesArray);
			cProc.CommandType = System.Data.CommandType.StoredProcedure;
			cProc.FetchType = Singular.CommandProc.FetchTypes.DataRow;
			cProc = cProc.Execute();
			if (!Singular.Misc.IsNullNothing(cProc.DataRow))
			{
				string newPassword = (string)(cProc.DataRow[0]);
				Singular.Emails.Email FPEmail = Singular.Emails.Email.NewEmail();
				String EmailSignature = MELib.Maintenance.SettingList.GetSettingList().FirstOrDefault().EmailSignatureText;
				var EmailSignatureURL = ConfigurationManager.AppSettings["EmailSignature"].ToString();
				String FirstName = MELib.RO.ROUserList.GetROUserListForForgotEmail(Email).FirstOrDefault().FirstName;
				if (ConfigurationManager.AppSettings["IsUATInd"].ToString() == "true")
				{
					FPEmail.Subject = "Reset Password - Management Effectiveness Tracking Tool";
				}
				else
				{
					FPEmail.Subject = "Reset Password - Management Effectiveness Tracking Tool";
				}
				FPEmail.ToEmailAddress = Email;
				FPEmail.FromEmailAddress = System.Configuration.ConfigurationManager.AppSettings["FromAddress"];
				FPEmail.FriendlyFrom = System.Configuration.ConfigurationManager.AppSettings["FriendlyFrom"];
				FPEmail.Body = "<div> <p> Hi [Name], </p> <p> A request to reset your password has been made. </p> <p> Please login to the METT website using your username and the following temporary password: </p> <span><b> [Password]</b> </span> <p> Regards, <br /> Management Effectiveness Tracking Tool</p> <br /><img src= " + EmailSignatureURL + " width='798' height='142'> <br />" + EmailSignature + "</div>";
				FPEmail.Body = FPEmail.Body.Replace("[Name]", FirstName);
				FPEmail.Body = FPEmail.Body.Replace("[Password]", newPassword);
				FPEmail.DateToSend = System.DateTime.Now;
				FPEmail.TrySave(typeof(Singular.Emails.EmailList));
			}
		}

		protected override void FetchExtraProperties(ref SafeDataReader sdr)
		{
			base.FetchExtraProperties(ref sdr);
			using (BypassPropertyChecks)
			{
				var i = 9;
				LoadProperty(ContactNumberProperty, sdr.GetString(i++));
				LoadProperty(JobDescriptionProperty, sdr.GetString(i++));
				LoadProperty(FirstNameProperty, Helpers.MEHelpers.DecryptStringDatabaseValue(sdr.GetValue(i++)));
				LoadProperty(SurnameProperty, Helpers.MEHelpers.DecryptStringDatabaseValue(sdr.GetValue(i++)));
			}

		}

		[WebCallable]
		public Result SaveUser(MELib.Security.User User)
		{
			Result sr = new Result();

			if (User.IsValid)
			{
				Singular.SaveHelper SavedUserSaveHelper = User.TrySave(typeof(MELib.Security.UserList));
				MELib.Security.User SavedUser = (MELib.Security.User)SavedUserSaveHelper.SavedObject;

				if (SavedUserSaveHelper.Success)
				{
					sr.Data = SavedUser;
					sr.Success = true;
					CommonData.Lists.Refresh("ROUserList");
				}

				return sr;
			}
			else
			{
				sr.ErrorText = User.GetErrorsAsHTMLString();
				sr.Success = false;
				return sr;
			}
		}
		#endregion
	}
}
