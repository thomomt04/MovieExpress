// Generated 31 Mar 2015 14:42 - Singular Systems Object Generator Version 2.1.661
// ToDo: Was manually converted. Generate new base with updated object generator.
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Data;
using Singular.Paging;
using Singular.Web;
using Singular;
using System.Linq;
#if SILVERLIGHT == false
using System.Data.SqlClient;
#endif

namespace MELib.Security
{
  /// <summary>
  /// The ROUserPagedList class
  /// </summary>
  [Serializable]
  public class ROUserPagedList: MEReadOnlyListBase<ROUserPagedList, ROUserPaged>, IPagedList
  {
    private int totalRecords = 0;

    public int TotalRecords
    {
      get
      {
        return this.totalRecords;
      }
    }

    #region "Data Access"

    [Serializable, WebFetchable]
    public class Criteria : PageCriteria<Criteria>
    {
      public static PropertyInfo<string> UserNameProperty = PageCriteria<Criteria>.RegisterSProperty<string>((c) => c.UserName, string.Empty);

      [Singular.DataAnnotations.TextField, Display(Name="Filter User Name")]
      public string UserName { get; set; }

			public static PropertyInfo<String> FirstNameProperty = RegisterSProperty<String>((c) => c.FirstName, "").AddSetExpression("ViewModel.UserListPageManager().Refresh();", false, 300);

			[Display(Name = "First Name", Description = "")]
			public String FirstName { get; set; }

			public static PropertyInfo<String> LastNameProperty = RegisterSProperty<String>((c) => c.LastName, "").AddSetExpression("ViewModel.UserListPageManager().Refresh();", false, 300);

			[Display(Name = "Last Name", Description = "")]
			public String LastName { get; set; }

			public static PropertyInfo<String> EmailProperty = RegisterSProperty<String>((c) => c.EmailAddress, "").AddSetExpression("ViewModel.UserListPageManager().Refresh();", false, 300);

			[Display(Name = "Email Address", Description = "")]
			public String EmailAddress { get; set; }

			public String Filters { get; set; }

			public Criteria()
      {
        UserName = string.Empty;
				FirstName = "";
				LastName = "";
				EmailAddress = "";
				Filters = string.Empty;

			}
		}

    public void New()
    {
    }

    public void Fetch(SafeDataReader safeDataReader)
    {
      safeDataReader.Read();
      this.totalRecords = safeDataReader.GetInt32(0);
      safeDataReader.NextResult();

      this.RaiseListChangedEvents = false;
      this.IsReadOnly = false;
      while (safeDataReader.Read())
      {
        this.Add(ROUserPaged.GetROUserPaged(safeDataReader));
      }
      this.IsReadOnly = true;
      this.RaiseListChangedEvents = true;
    }

    protected override void DataPortal_Fetch(object criteriaObject)
    {
      Criteria criteria = (Criteria)criteriaObject;
      using (SqlConnection connection = new SqlConnection(Singular.Settings.ConnectionString))
      {
        connection.Open();
        try
        {
          using (SqlCommand command = connection.CreateCommand())
          {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "GetProcs.getROUserPagedList";
						if ((criteria.FirstName != null && criteria.FirstName != "") || (criteria.LastName != null && criteria.LastName != ""))
						{
							MELib.RO.ROUserList rOUserList = RO.ROUserList.GetROUserList();
							System.Collections.Generic.List<RO.ROUser> listrOUserList = null;

							//filter the list
							if (criteria.FirstName.Length != 0 && criteria.LastName.Length != 0)
							{
								listrOUserList = rOUserList.Where<RO.ROUser>(c => c.FirstName.ToUpper().Contains(criteria.FirstName.ToUpper()) == true && c.LastName.ToUpper().Contains(criteria.LastName.ToUpper()) == true).ToList();
							}
							if (criteria.FirstName.Length != 0 && criteria.LastName.Length == 0)
							{
								listrOUserList = rOUserList.Where<RO.ROUser>(c => c.FirstName.ToUpper().Contains(criteria.FirstName.ToUpper()) == true).ToList();
							}
							if (criteria.FirstName.Length == 0 && criteria.LastName.Length != 0)
							{
								listrOUserList = rOUserList.Where<RO.ROUser>(c => c.LastName.ToUpper().Contains(criteria.LastName.ToUpper()) == true).ToList();
							}

							criteria.Filters = listrOUserList.GetXmlIDs<RO.ROUser>();

						}
						criteria.AddParameters(command);
            command.Parameters.AddWithValue("@UserName", criteria.UserName);
						command.Parameters.AddWithValue("@EmailAddress", Singular.Misc.NothingDBNull(criteria.EmailAddress));
						command.Parameters.AddWithValue("@Filters", Singular.Misc.NothingDBNull(criteria.Filters));

						using (SafeDataReader safeDataReader = new SafeDataReader(command.ExecuteReader()))
            {
              this.Fetch(safeDataReader);
            }
          }
        }
        finally
        {
          connection.Close();
        }
      }
    }

    #endregion
  }
}
