using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MELib.RO;

namespace MEWeb.Reports
{
	public partial class Reports : Singular.Web.PageBase<ReportVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}

	public class ReportVM : Singular.Web.Reporting.ReportVM
	{

		public bool PageViewInd { get; set; }
		//public ROSecurityOrganisationProtectedAreaGroupUserList ROSecurityOrganisationProtectedAreaGroupUserList { get; set; }

		enum SecurityRoles
		{
			Create,
			Access,
			View
		}

		public ReportVM()
		{

		}

		protected override void Setup()
		{
			base.Setup();

			// Security based on multiple groups and roles for a selected user
		//	ROSecurityOrganisationProtectedAreaGroupUserList = METTLib.RO.ROSecurityOrganisationProtectedAreaGroupUserList.GetROSecurityOrganisationProtectedAreaGroupUserList(Singular.Security.Security.CurrentIdentity.UserID, null, null);
		//	PageViewInd = Singular.Settings.CurrentUser.Roles.Contains("Reports.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());

		}
	}

}