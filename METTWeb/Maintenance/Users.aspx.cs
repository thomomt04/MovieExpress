using System.ComponentModel;
using System.Linq;
using Singular.Security;
using Singular.Web;
using Singular.Web.Data;
using MELib.Security;
using System;

namespace MEWeb.Maintenance
{
  /// <summary>
  /// The Users page class
  /// </summary>
  public partial class Users : MEPageBase<UsersVM>
  {
  }

  /// <summary>
  /// The UsersVM ViewModel class
  /// </summary>
  public class UsersVM : MEStatelessViewModel<UsersVM>
  {
    /// <summary>
    /// User List Page Manager
    /// </summary>
    public PagedDataManager<UsersVM> UserListPageManager { get; set; }

    /// <summary>
    /// User Criteria
    /// </summary>
    public ROUserPagedList.Criteria UserCriteria { get; set; }

    /// <summary>
    /// User List
    /// </summary>
    public ROUserPagedList UserList { get; set; }

    /// <summary>
    /// The Editing User
    /// </summary>
    public MELib.Security.User EditingUser { get; set; }

    /// <summary>
    /// UsersVM constructor
    /// </summary>
    public UsersVM()
    {
      this.UserListPageManager = new PagedDataManager<UsersVM>((c) => c.UserList, (c) => c.UserCriteria, "UserName", 20);
      this.UserCriteria = new ROUserPagedList.Criteria();
      this.UserList = new ROUserPagedList();
    }

    /// <summary>
    /// Setup the Users ViewModel
    /// </summary>
    protected override void Setup()
    {
      base.Setup();

      this.ValidationDisplayMode = ValidationDisplayMode.Controls | ValidationDisplayMode.SubmitMessage;

      this.UserList = (ROUserPagedList)UserListPageManager.GetInitialData();
    }

    /// <summary>
    /// Gets the Security Group List (For drop down)
    /// </summary>
    [Browsable(false)]
    public SecurityGroupList SecurityGroupList
    {
      get
      {
				var ROSecurityRoles = MELib.Security.ROSecurityGroupList.GetROSecurityGroupList(true);

				var SecurityRoles = SecurityGroupList.GetSecurityGroupList();
				var clonedList = SecurityRoles.Clone();

				foreach (var item in SecurityRoles)
				{
					if((!ROSecurityRoles.Any(c => c.SecurityGroupID == item.SecurityGroupID)) || (item.SecurityGroup == "Administrator"))
					{
						clonedList.Remove(item);
					}
				}

				SecurityRoles = clonedList;

				return SecurityRoles;
      }
    }

    /// <summary>
    /// Get the user with the given Id
    /// </summary>
    /// <param name="userId">The User Id</param>
    /// <returns>A User instance</returns>
    [WebCallable]
    public static MELib.Security.User GetUser(int userId)
    {
			return MELib.Security.UserList.GetUserList(userId).First();
    }

    [WebCallable]
    public Result ManageUser()
    {
            Result sr = new Result();
            return sr;

        }
        /// <summary>
        /// Save changes to a user
        /// </summary>
        /// <param name="user">A user instance</param>
        /// <returns>The save result</returns>
        [WebCallable(Roles = new string[] { "Security.Manage Users" })]
    public static Result SaveUser(MELib.Security.User user)
    {
			if (user.SecurityGroupUserList.Count == 0)
			{
				//add a default security group of General User
				SecurityGroupUser securityGroupUser = SecurityGroupUser.NewSecurityGroupUser();
				securityGroupUser.SecurityGroupID = ROSecurityGroupList.GetROSecurityGroupList(true).FirstOrDefault(c => c.SecurityGroup ==	"General User")?.SecurityGroupID;
				user.SecurityGroupUserList.Add(securityGroupUser);
			}

			user.LoginName = user.EmailAddress;

			Result results = new Singular.Web.Result();
			Result Saveresults = user.SaveUser(user);
			MELib.Security.User SavedUser = (MELib.Security.User)Saveresults.Data;

			if(SavedUser != null)
			{
				results.Success = true;
				results.Data = SavedUser;
			}
			else
			{
				results.Success = false;
				results.ErrorText = Saveresults.ErrorText;
			}
			return results;
    }

    /// <summary>
    /// Delete the user with the given Id
    /// </summary>
    /// <param name="userId">The User Id</param>
    /// <returns>The delete result</returns>
    [WebCallable(Roles = new string[] { "Security.Manage Users" })]
    public static Result CanDeleteUser(int userId, Boolean RemoveAssociations)
    {
      Result results = new Singular.Web.Result();
			try
			{

				//get the user object and soft delete the user
				MELib.Security.UserList userList = MELib.Security.UserList.GetUserList(userId);
				userList.RemoveAt(0);
				userList.Save();

				results.Success = true;
			}
			catch (Exception ex)
			{
				results.ErrorText = ex.Message;
				results.Success = false;
			}
			return results;
    }

    /// <summary>
    /// Reset a user's password
    /// </summary>
    /// <param name="userName">The user's username</param>
    [WebCallable(Roles = new string[] { "Security.Reset Passwords" })]
    public static void ResetPassword(string EmailAddress)
    {
      MELib.Security.User.ResetPassword(EmailAddress);
    }
	}
}
