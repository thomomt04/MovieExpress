using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web.Data;
using METTLib.Security;
using Singular.Security;
using Singular.Web;
using METTLib.ProtectedArea;
using METTLib.Organisation;

namespace METTWeb.Users
{
	public partial class UserProfile : METTPageBase<UserProfileVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}

	public class UserProfileVM : METTStatelessViewModel<UserProfileVM>
	{

		public METTLib.Security.User Editinguser { get; set; }
		public SecurityGroupUserList SecurityGroupUserList { get; set; }

		public SecurityGroupProtectedAreaUserList TempSecurityGroupProtectedAreaUserList { get; set; }
		//public ROSecurityGroupProtectedAreaUserList ROSecurityGroupProtectedAreaUserList { get; set; }
		public OrganisationProtectedAreaUser OrganisationProtectedAreaUser { get; set; }
		public SecurityGroupProtectedAreaUserList SecurityGroupProtectedAreaUserList { get; set; }
		public int ProtectedAreaID { get; set; }
		public bool IsProtectedAreaUserInd { get; set; }

		public SecurityGroupOrganisationUserList SecurityGroupOrganisationUserList { get; set; }
		//public ROSecurityGroupOrganisationUserList ROSecurityGroupOrganisationUserList { get; set; }
		public OrganisationUser OrganisationUser { get; set; }
		public int OrganisationID { get; set; }
		public bool IsOrganisationUsersInd { get; set; }

		//public SecurityGroup TempSecurityGroup { get; set; }

		public ROSecurityGroupList ROSecurityGroupList { get; set; }

		public bool CanGoBack { get; set; }


		/// <summary>
		/// UsersVM constructor
		/// </summary>
		public UserProfileVM()
		{
	
		}

		protected override void Setup()
		{
			base.Setup();

			CanGoBack = false;

			ROSecurityGroupList = ROSecurityGroupList.GetROSecurityGroupList(true);

			//need to find UserID from query string params
			int UserID = 0;
			int OrganisationUserID = 0;
			int ProtectedAreaUserID = 0;

			#region " Edit User\Add New Protected Area user\ Add New OrganisationUser"

			if ((Page != null) && (Page.Request.Params["UserID"] != null))
			{
				UserID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["UserID"]).Replace(" ", "+")));
				Editinguser = UserID != 0 ? METTLib.Security.UserList.GetUserList(UserID).First() : null;

				if (Editinguser != null)
				{
					//let's check if the page redirection was from protected areas
					if ((Page.Request.Params["OrganisationProtectedAreaID"] != null))
					{
						IsProtectedAreaUserInd = true;
						//check if the user already exists before creating a new one to avoid duplicates
						ProtectedAreaID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["ProtectedAreaID"]).Replace(" ", "+")));
						var OrganisationProtectedAreaID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["OrganisationProtectedAreaID"]).Replace(" ", "+")));
						var ProtectedAreaUser = OrganisationProtectedAreaUserList.GetOrganisationProtectedAreaUserList().FirstOrDefault(c => c.OrganisationIDProtectedAreaID == OrganisationProtectedAreaID && c.UserID == UserID);
						ProtectedAreaUserID = ProtectedAreaUser != null ? OrganisationProtectedAreaUserList.GetOrganisationProtectedAreaUserList().FirstOrDefault(c => c.OrganisationIDProtectedAreaID == OrganisationProtectedAreaID && c.UserID == UserID).OrganisationProtectedAreaUserID : 0;

						if(ProtectedAreaUserID != 0)
						{
							OrganisationProtectedAreaUser = OrganisationProtectedAreaUserList.GetOrganisationProtectedAreaUserList(ProtectedAreaUserID).FirstOrDefault();
						}

						if (OrganisationProtectedAreaUser != null)
						{
							SecurityGroupProtectedAreaUserList = SecurityGroupProtectedAreaUserList.GetSecurityGroupProtectedAreaUserList(UserID, ProtectedAreaID);
							//mark as Active those roles that exists
							if ((SecurityGroupProtectedAreaUserList != null) && (SecurityGroupProtectedAreaUserList.Count > 0))
							{
								foreach (var item in ROSecurityGroupList)
								{
									if (SecurityGroupProtectedAreaUserList.Any(c => c.SecurityGroupID == item.SecurityGroupID && c.IsActiveInd == true))
									{
										item.IsActiveInd = true;
									}
								}
							}
						}
						else
						{
							OrganisationProtectedAreaUser = METTLib.ProtectedArea.OrganisationProtectedAreaUser.NewOrganisationProtectedAreaUser();
							OrganisationProtectedAreaUser.OrganisationIDProtectedAreaID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["OrganisationProtectedAreaID"]).Replace(" ", "+")));
							OrganisationProtectedAreaUser.UserID = UserID;

							SecurityGroupProtectedAreaUserList = SecurityGroupProtectedAreaUserList.NewSecurityGroupProtectedAreaUserList();
						}

					}

					//let's check if the page redirection was from organisations
					else if ((Page.Request.Params["OrganisationID"] != null))
					{
						IsOrganisationUsersInd = true;
						//check if the user already exists before creating a new one to avoid duplicates
						OrganisationID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["OrganisationID"]).Replace(" ", "+")));
						OrganisationUser = OrganisationUserList.GetOrganisationUserList().FirstOrDefault(c => c.OrganisationID == OrganisationID && c.UserID == UserID);

						if (OrganisationUser == null)
						{
							OrganisationUser = OrganisationUser.NewOrganisationUser();
							OrganisationUser.OrganisationID = OrganisationID;
							OrganisationUser.UserID = UserID;

							SecurityGroupOrganisationUserList = SecurityGroupOrganisationUserList.NewSecurityGroupOrganisationUserList();
						}
						else
						{
							SecurityGroupOrganisationUserList = SecurityGroupOrganisationUserList.GetSecurityGroupOrganisationUserList(UserID, OrganisationID);
							//mark as Active the roles that exists
							if ((SecurityGroupProtectedAreaUserList != null) && (SecurityGroupProtectedAreaUserList.Count > 0))
							{
								foreach (var item in ROSecurityGroupList)
								{
									if (SecurityGroupProtectedAreaUserList.Any(c => c.SecurityGroupID == item.SecurityGroupID && c.IsActiveInd == true))
									{
										item.IsActiveInd = true;
									}
								}
							}

						}

					}

					else
					{
						//editing User
						SecurityGroupUserList = SecurityGroupUserList.GetSecurityGroupUserList(UserID);

						if ((SecurityGroupUserList != null) && (SecurityGroupUserList.Count > 0))
						{
							foreach (var item in ROSecurityGroupList)
							{
								if (SecurityGroupUserList.Any(c => c.SecurityGroupID == item.SecurityGroupID))
								{
									item.IsActiveInd = true;
								}
							}
						}

					}
				}
		

			}

			#endregion


			#region " Edit Organisation User - From Organisation Profile "
			if ((Page != null) && (Page.Request.Params["OrganisationUserID"] != null))
			{
				OrganisationUserID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["OrganisationUserID"]).Replace(" ", "+")));
				OrganisationID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["OrganisationID"]).Replace(" ", "+")));

				OrganisationUser = OrganisationUserList.GetOrganisationUserList().FirstOrDefault(c=> c.OrganisationUserID == OrganisationUserID);

				if (OrganisationUser != null)
				{
					UserID = OrganisationUser.UserID;
					Editinguser = UserID != 0 ? METTLib.Security.UserList.GetUserList(UserID).FirstOrDefault() : null;
					IsOrganisationUsersInd = true;

					SecurityGroupOrganisationUserList = SecurityGroupOrganisationUserList.GetSecurityGroupOrganisationUserList(UserID, OrganisationID);

					//mark as Active those roles that exists
					if ((SecurityGroupOrganisationUserList != null) && (SecurityGroupOrganisationUserList.Count > 0))
					{
						foreach (var item in ROSecurityGroupList)
						{
							if (SecurityGroupOrganisationUserList.Any(c => c.SecurityGroupID == item.SecurityGroupID && c.IsActiveInd == true))
							{
								item.IsActiveInd = true;
							}
						}
					}

				}

			}
			
			#endregion


			#region " Edit Organisation Protected Area User - From Protected Area Profile"

			if ((Page != null) && (Page.Request.Params["OrganisationProtectedAreaUserID"] != null))
			{
				ProtectedAreaUserID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["OrganisationProtectedAreaUserID"]).Replace(" ", "+")));
				ProtectedAreaID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["ProtectedAreaID"]).Replace(" ", "+")));

				OrganisationProtectedAreaUser = OrganisationProtectedAreaUserList.GetOrganisationProtectedAreaUserList(ProtectedAreaUserID).FirstOrDefault();
				
				if (OrganisationProtectedAreaUser != null)
				{
					UserID = OrganisationProtectedAreaUser.UserID;

					Editinguser = UserID != 0 ? METTLib.Security.UserList.GetUserList(UserID).FirstOrDefault() : null;
					IsProtectedAreaUserInd = true;

					//ROSecurityGroupProtectedAreaUserList = ROSecurityGroupProtectedAreaUserList.GetROSecurityGroupProtectedAreaUserList(UserID,ProtectedAreaID);

					SecurityGroupProtectedAreaUserList = SecurityGroupProtectedAreaUserList.GetSecurityGroupProtectedAreaUserList(UserID, ProtectedAreaID);

					//mark as Active those roles that exists
					if ((SecurityGroupProtectedAreaUserList != null) && (SecurityGroupProtectedAreaUserList.Count > 0))
					{
						foreach (var item in ROSecurityGroupList)
						{
							if (SecurityGroupProtectedAreaUserList.Any(c=> c.SecurityGroupID == item.SecurityGroupID && c.IsActiveInd == true))
							{
								item.IsActiveInd = true;
							}
						}
					}
				}


			}

			#endregion

			if (Editinguser == null)
			{
				//new user
				//Editinguser = METTLib.Security.UserList.GetUserList.new
			}

		}

		/// <summary>
		/// Get the user with the given Id
		/// </summary>
		/// <param name="userId">The User Id</param>
		/// <returns>A User instance</returns>
		[WebCallable]
		public static METTLib.Security.User GetUser(int userId)
		{
			return METTLib.Security.UserList.GetUserList(userId).First();
		}

		[WebCallable]
		public Result SaveProtectedAreaSecurityRoles(METTLib.ProtectedArea.SecurityGroupProtectedAreaUserList TempSecurityGroupProtectedAreaUserList)
		{
			Result result= new Result() ;

			//result.Data = "";

			return result;
		}

		[WebCallable]
		public Result AddSecurityGroup(int SecurityGroupID, SecurityGroupProtectedAreaUserList SecurityGroupProtectedAreaUserList, SecurityGroupOrganisationUserList SecurityGroupOrganisationUserList, int UserID, int ProtectedAreaID, int OrganisationID)
		{
			Result result = new Result();

			if (ProtectedAreaID != 0)
			{
				if (SecurityGroupProtectedAreaUserList.Any(c=> c.SecurityGroupID == SecurityGroupID))
				{
					//remove it from the list
					SecurityGroupProtectedAreaUserList.First(c => c.SecurityGroupID == SecurityGroupID).IsActiveInd = true;
				}
				else
				{
					//add to list
					SecurityGroupProtectedAreaUser SecurityGroupProtectedAreaUser = new SecurityGroupProtectedAreaUser();
					SecurityGroupProtectedAreaUser.SecurityGroupID = SecurityGroupID;
					SecurityGroupProtectedAreaUser.UserID = UserID;
					SecurityGroupProtectedAreaUser.ProtectedAreaID = ProtectedAreaID;

					SecurityGroupProtectedAreaUserList.Add(SecurityGroupProtectedAreaUser);
				}
			}

			if (OrganisationID != 0)
			{
				if (SecurityGroupOrganisationUserList.Any(c => c.SecurityGroupID == SecurityGroupID))
				{
					//remove it from the list
					SecurityGroupOrganisationUserList.First(c=> c.SecurityGroupID == SecurityGroupID).IsActiveInd = true;
				}
				else
				{
					//add to list
					SecurityGroupOrganisationUser SecurityGroupOrganisationUser = new SecurityGroupOrganisationUser();
					SecurityGroupOrganisationUser.SecurityGroupID = SecurityGroupID;
					SecurityGroupOrganisationUser.UserID = UserID;
					SecurityGroupOrganisationUser.OrganisationID = OrganisationID;

					SecurityGroupOrganisationUserList.Add(SecurityGroupOrganisationUser);
				}
			}

			result.Data = Tuple.Create(SecurityGroupProtectedAreaUserList, SecurityGroupOrganisationUserList);
			result.Success = true;

			return result;
		}

		[WebCallable]
		public Result RemoveSecurityGroup(int SecurityGroupID, SecurityGroupProtectedAreaUserList SecurityGroupProtectedAreaUserList, SecurityGroupOrganisationUserList SecurityGroupOrganisationUserList)
		{
			Result result = new Result();

			if (SecurityGroupProtectedAreaUserList != null && SecurityGroupProtectedAreaUserList.Count> 0)
			{
				if (SecurityGroupProtectedAreaUserList.Any(c => c.SecurityGroupID == SecurityGroupID))
				{
					//remove it from the list by making it inactive
					SecurityGroupProtectedAreaUserList.First(c => c.SecurityGroupID == SecurityGroupID).IsActiveInd = false;
				}
			}

			if (SecurityGroupOrganisationUserList != null && SecurityGroupOrganisationUserList.Count > 0)
			{
				if (SecurityGroupOrganisationUserList.Any(c => c.SecurityGroupID == SecurityGroupID))
				{
					//remove it from the list by making it inactive
					SecurityGroupOrganisationUserList.First(c => c.SecurityGroupID == SecurityGroupID).IsActiveInd = false;
				}
			}

			result.Data = Tuple.Create(SecurityGroupProtectedAreaUserList, SecurityGroupOrganisationUserList);
			result.Success = true;

			return result;
		}

		[WebCallable]
		public static string EncodeUrl(string urlParam, string urlString)
		{
			var url = urlString + HttpUtility.UrlEncode(Singular.Encryption.EncryptString(urlParam));

			return url;
		}

	}

}