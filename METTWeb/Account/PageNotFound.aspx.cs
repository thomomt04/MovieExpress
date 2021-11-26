using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;
using static METTWeb.PageNotFound;

namespace METTWeb
{
	public partial class PageNotFound : METTPageBase<PageNotFoundVM>
	{
		public class PageNotFoundVM : METTStatelessViewModel<PageNotFoundVM>
		{
		}
	}
}

