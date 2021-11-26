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

namespace METTWeb.TestPages
{
	public partial class Panels : METTPageBase<PanelsVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class PanelsVM : METTStatelessViewModel<PanelsVM>
	{

		public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }
		public PanelsVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();

			ROQuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();
		}

	}
}
