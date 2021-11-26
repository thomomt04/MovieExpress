
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
	public partial class Wizard : METTPageBase<WizardVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class WizardVM : METTStatelessViewModel<WizardVM>
	{

		public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }
		public WizardVM()
		{

		}

		protected override void Setup()
		{
			base.Setup();

			ROQuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();

		}

	}
}
