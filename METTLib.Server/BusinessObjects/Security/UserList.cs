using System;
using System.Data.SqlClient;
using Csla;
using Singular.Security;

namespace MELib.Security
{
  /// <summary>
  /// The UserList class
  /// </summary>
  [Serializable]
  public class UserList: UserListBase<UserList, User>
  {
    /// <summary>
    /// Add additional User parameters to the Sql Command
    /// </summary>
    /// <param name="command">SQL Command instance</param>
    /// <param name="criteria">Base user criteria</param>
    protected override void AddProcParameters(SqlCommand command, BaseCriteria criteria)
    {
      base.AddProcParameters(command, criteria);
      command.Parameters.AddWithValue("@UserID", criteria.UserID);
    }

    /// <summary>
    /// Get a user list
    /// </summary>
    /// <param name="userId">A user ID</param>
    /// <returns>A UserList instance</returns>
    public static UserList GetUserList(int userId)
    {
      return DataPortal.Fetch<UserList>(new BaseCriteria()
      {
        UserID = userId
      });
    }
  }
}
