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
using METTLib.Organisation;
using METTLib.ProtectedArea;


namespace METTWeb.TestPages
{
	public partial class dlWizard : METTPageBase<dlWizardVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class dlWizardVM : METTStatelessViewModel<dlWizardVM>
	{

		public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }

		public METTLib.QuestionnaireSurvey.QuestionnaireQuestionList QuestionnaireQuestionList { get; set; }

		public int FirstQuestionID { get; set; }

		public dlWizardVM()
		{

		}

		protected override void Setup()
		{
			base.Setup();

			QuestionnaireQuestionList = METTLib.QuestionnaireSurvey.QuestionnaireQuestionList.NewQuestionnaireQuestionList(1, 280);

			ROQuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();
			FirstQuestionID = QuestionnaireQuestionList.FirstOrDefault().QuestionnaireQuestionID;

		}

		[WebCallable]
		public static Tuple<METTLib.QuestionnaireSurvey.QuestionnaireQuestionList, int> GetSurveyQuestionnaireGroupData(int QGroupID, int questionnaireAnswerSetID)
		{
			//Get Object data
			METTLib.QuestionnaireSurvey.QuestionnaireQuestionList QuestionnaireQuestionList = METTLib.QuestionnaireSurvey.QuestionnaireQuestionList.NewQuestionnaireQuestionList(QGroupID, questionnaireAnswerSetID);

			int FirstQuestionID = QuestionnaireQuestionList.FirstOrDefault().QuestionnaireQuestionID;

			return Tuple.Create<METTLib.QuestionnaireSurvey.QuestionnaireQuestionList, int>(QuestionnaireQuestionList, FirstQuestionID);
		}


	}
}
