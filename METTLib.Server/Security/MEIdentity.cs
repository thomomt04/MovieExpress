using System;
using System.Data.SqlClient;
using Csla.Data;
using Singular;
using Singular.Security;
using Singular.Web.Security;

namespace MELib.Security
{
  /// <summary>
  /// ResetState Enumeration
  /// </summary>
  public enum ResetState
  {
    /// <summary>
    /// Normal State (No reset needed)
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Indicates that the password must be reset
    /// </summary>
    MustResetPassword = 1
  }

  /// <summary>
  /// The METTIdentity class
  /// </summary>
  [Serializable]
  public class MEIdentity: WebIdentity<MEIdentity>
  {
    #region Fields

    /// <summary>
    /// The First Name
    /// </summary>
    private string firstName;

    /// <summary>
    /// The Email Address
    /// </summary>
    private string emailAddress;

    /// <summary>
    /// Is this the identity's first time logging in?
    /// </summary>
    private bool firstTimeLogin;

    /// <summary>
    /// Password reset state
    /// </summary>
    private ResetState resetState;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the Email Address
    /// </summary>
    public string EmailAddress
    {
      get
      {
        return this.emailAddress;
      }
    }

    /// <summary>
    /// Gets the First Name
    /// </summary>
    public string FirstName
    {
      get
      {
        return this.firstName;
      }
    }

    /// <summary>
    /// Gets the First Time Login indicator
    /// </summary>
    public Boolean FirstTimeLogin
    {
      get
      {
        return this.firstTimeLogin;
      }
    }

    /// <summary>
    /// Gets the password Reset State
    /// </summary>
    public ResetState ResetState
    {
      get
      {
        return this.resetState;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Marks the user as not first time login. This should be called after the user has gone to any page / or whatever process needs to run for first time users.
    /// </summary>
    public void MarkNonFirstTimeLogin()
    {
      this.firstTimeLogin = false;

			// Call a cmd proc to update this in the database.

			CommandProc commandProc = new CommandProc("CmdProcs.cmdMarkNonFirstTimeLogin", new string[] { "@UserID"}, new object[] { this.UserID});
			commandProc.CommandType = System.Data.CommandType.StoredProcedure;
			commandProc = commandProc.Execute();

			//CommandProc mycommandProc = new CommandProc("[CmdProcs].[cmdSectionAccess]", ParameterArray, ValuesArray);
			//mycommandProc.Parameters.Add(new CommandProc.Parameter() { Name = "@HasAccess", Direction = System.Data.ParameterDirection.Output, SqlType = System.Data.SqlDbType.Bit });
			//mycommandProc.CommandType = System.Data.CommandType.StoredProcedure;
			//mycommandProc.FetchType = Singular.CommandProc.FetchTypes.DataRow;
			//mycommandProc = mycommandProc.Execute();

			//bPassCheck = Convert.ToBoolean(mycommandProc.Parameters[0].Value);

		}

    /// <summary>
    /// After changing a user's password, call this to clear the Reset State
    /// </summary>
    public void ChangedPassword()
    {
      this.resetState = ResetState.Normal;
    }

    /// <summary>
    /// Setup a SQL Command to do a login check
    /// </summary>
    /// <param name="command">A SQL Command instance</param>
    /// <param name="criteria">The login criteria</param>
    protected override void SetupSqlCommand(SqlCommand command, IdentityCriterea criteria)
    {
      base.SetupSqlCommand(command, criteria);

      command.CommandText = "CmdProcs.WebLogin";
      command.Parameters["@Password"].Value = MEWebSecurity.GetPasswordHash(command.Parameters["@Password"].Value.ToString());
    }

    /// <summary>
    /// Reads the extra METTIdentity class properties from the data reader
    /// </summary>
    /// <param name="safeDataReader">Data reader containing the additional properties</param>
    /// <param name="startIndex">Index to start reading from</param>
    protected override void ReadExtraProperties(SafeDataReader safeDataReader, ref int startIndex)
    {
      base.ReadExtraProperties(safeDataReader, ref startIndex);

      this.emailAddress = safeDataReader.GetString(startIndex);
      this.firstTimeLogin = safeDataReader.GetBoolean(startIndex + 1);
      // Implement these columns in the Stored Procedure if you need them
      this.resetState = (ResetState)safeDataReader.GetInt32(startIndex + 2);
      // this.firstName = safeDataReader.GetString(startIndex + 3);
    }

		public void RefreshRoles()
		{
			Singular.Security.IdentityCriterea identityCriterea = new Singular.Security.IdentityCriterea { Username = Singular.Settings.CurrentUser.UserName, RefreshingRoles = true };
			AuthenticateUser(identityCriterea);
		}

    public void MakeConsolAppUser(int userID, int authorisedUserID, string name)
    {
      LoadProperty(UserIDProperty, userID);
      LoadProperty(NameProperty, name);
      //mAuthorisedUserID = authorisedUserID;
    }
    #endregion
  }
}
