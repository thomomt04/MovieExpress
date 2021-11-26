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
using METTLib.Questionnaire;
using Singular;
using System.IO;


namespace METTWeb.TestPages
{
	public partial class UploadFile : METTPageBase<UploadFileVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class UploadFileVM : METTStatelessViewModel<UploadFileVM>
	{

		public UploadFileVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();
		}
	}
}


