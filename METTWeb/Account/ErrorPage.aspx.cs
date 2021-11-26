using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web;
using static MEWeb.ErrorPage;

namespace MEWeb
{
	public partial class ErrorPage : MEPageBase<ErrorPageVM>
	{
		public class ErrorPageVM : MEStatelessViewModel<ErrorPageVM>
		{
		}
	}
}

