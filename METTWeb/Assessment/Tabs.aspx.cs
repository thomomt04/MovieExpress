
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static METTWeb.Organisation.OrganisationProfile;
using Csla;
using Singular.Web.Data;
using Singular.Web;
using METTLib.Organisation;
using METTLib.ProtectedArea;

namespace METTWeb.Assessment
{
	public partial class Tabs : METTPageBase<TabsVM>
	{

	}

	public class TabsVM : METTStatelessViewModel<TabsVM>
	{

		public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }

		public METTLib.QuestionnaireSurvey.QuestionnaireQuestionList QuestionnaireQuestionList { get; set; }


		public int FirstQuestionID { get; set; }

		public TabsVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();
			FirstQuestionID = 0;

			ROQuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();

			QuestionnaireQuestionList = METTLib.QuestionnaireSurvey.QuestionnaireQuestionList.NewQuestionnaireQuestionList(1, 280);

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
