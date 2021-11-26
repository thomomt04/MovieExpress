using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Csla;
using Singular.Web.Data;
using Singular.Web;
using METTLib.Maintenance;
using Singular.Web.MaintenanceHelpers;


namespace METTWeb.Maintenance
{
	public partial class Threats : METTPageBase<ThreatsVM>
	{
	}
	public class ThreatsVM : METTStatelessViewModel<ThreatsVM>
	{
		public ThreatsVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();

		}

	}
}

