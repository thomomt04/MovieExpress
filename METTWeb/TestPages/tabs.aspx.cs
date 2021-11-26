using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Csla;
using Singular.Web.Data;
using Singular.Web;
using METTLib.Questionnaire;
using METTLib.RO;

namespace METTWeb.TestPages
{
	public partial class tabs : METTPageBase<tabsVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class tabsVM : METTStatelessViewModel<tabsVM>
	{
		public int SelectedTab { get; set; }

		public tabsVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();
			SelectedTab = 0;

		}

	}
}
