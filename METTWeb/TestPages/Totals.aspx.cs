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
	public partial class Totals : METTPageBase<TotalsVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class TotalsVM : METTStatelessViewModel<TotalsVM>
	{

		//public METTLib.Questionnaire.ROQuestionnaireStatisticsList ROQuestionnaireStatisticsList { get; set; }
		//public METTLib.Questionnaire.ROQuestionnaireStatistics FirstQuestionnaireStatistics { get; set; }

		public TotalsVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();

			//ROQuestionnaireStatisticsList = METTLib.Questionnaire.ROQuestionnaireStatisticsList.GetROQuestionnaireStatisticsList();
			//FirstQuestionnaireStatistics = ROQuestionnaireStatisticsList.FirstOrDefault();
		
		}

	}
}
