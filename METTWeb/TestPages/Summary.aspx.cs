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
	public partial class Summary : METTPageBase<SummaryVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class SummaryVM : METTStatelessViewModel<SummaryVM>
	{

		public METTLib.Questionnaire.QuestionnaireGroupAnswerList QuestionnaireGroupAnswerList { get; set; }
		public METTLib.Questionnaire.QuestionnaireGroupAnswer FirstQuestionnaireGroupAnswer { get; set; }

		public METTLib.RO.ROThreatMatrixList ScopeROThreatMatrixList { get; set; }
		public METTLib.RO.ROThreatMatrixList MagnitudeROThreatMatrixList { get; set; }

		public SummaryVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();

			ScopeROThreatMatrixList = METTLib.RO.ROThreatMatrixList.GetROThreatMatrixList(1);
			MagnitudeROThreatMatrixList = METTLib.RO.ROThreatMatrixList.GetROThreatMatrixList(3);

			QuestionnaireGroupAnswerList = METTLib.Questionnaire.QuestionnaireGroupAnswerList.GetQuestionnaireGroupAnswerList(280, 1);
			FirstQuestionnaireGroupAnswer = QuestionnaireGroupAnswerList.FirstOrDefault();

		}

	}
}
