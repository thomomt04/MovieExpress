using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Csla;
using Singular.Web.Data;
using Singular.Web;
using METTLib.Organisation;
using METTLib.ProtectedArea;

namespace METTWeb.Faq
{
	public partial class Faq : METTPageBase<FaqVM>
	{
	}

	public class FaqVM : METTStatelessViewModel<FaqVM>
	{
		public FaqVM()
		{
		}
		protected override void Setup()
		{
			base.Setup();

		}

	}
}
