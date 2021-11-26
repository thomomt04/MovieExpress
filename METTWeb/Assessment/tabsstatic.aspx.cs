using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Csla;
using Singular.Web.Data;
using Singular.Web;

namespace METTWeb.Assessment
{
	public partial class tabsstatic : METTPageBase<tabsstaticVM>
	{

	}

	public class tabsstaticVM : METTStatelessViewModel<tabsstaticVM>
	{

		public tabsstaticVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();

		}
	}
}
