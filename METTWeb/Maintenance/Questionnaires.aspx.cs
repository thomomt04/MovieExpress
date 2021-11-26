using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Csla;
using Singular.Web.Data;
using Singular.Web;
using METTLib.Maintenance;
using Singular.Web.MaintenanceHelpers;

namespace METTWeb.Maintenance
{
	public partial class Questionnaires : METTPageBase<QuestionnairesVM>
	{
	}
	public class QuestionnairesVM : METTStatelessViewModel<QuestionnairesVM>
	{
		public METTLib.Questionnaire.QuestionnaireList QuestionnaireList { get; set; }
		public METTLib.Questionnaire.QuestionnaireList EditingQuestionnaire { get; set; }
		public bool IsViewingQuestionnaireInd { get; set; }
		public METTLib.QuestionnaireSurvey.QuestionnaireQuestionList QuestionnaireQuestionList { get; set; }

		public QuestionnairesVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();
			this.QuestionnaireList = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList();
		}

		[Singular.Web.WebCallable(LoggedInOnly = true)]
		public string ManageQuestionnaire(int QuestionnaireId)
		{
			var url = $"../Maintenance/QuestionnaireSetup.aspx?QuestionnaireId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(QuestionnaireId.ToString()))}";
			return url;
		}

		[WebCallable]
		public static Result CreateQuestionnaireAnswerSet(int QuestionnaireID)
		{
			Result results = new Singular.Web.Result();

			//Get published questionnaire first to create duplicate
			var pQuestionnaireID = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList().First(c => c.PublishInd == true && c.IsDeleted == false).QuestionnaireID;
			if (pQuestionnaireID != 0)
			{
				//Get values to duplicate
				var pQuestionnaireName = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList().First(c => c.PublishInd == true && c.IsDeleted == false).QuestionnaireName;
				var pQuestionnaireVersionNumber = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList().First(c => c.PublishInd == true && c.IsDeleted == false).QuestionnaireVersionNumber;
				var pStartDate = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList().First(c => c.PublishInd == true && c.IsDeleted == false).StartDate;
				var pEndDate = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList().First(c => c.PublishInd == true && c.IsDeleted == false).EndDate;
				var pPublishDateTime = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList().First(c => c.PublishInd == true && c.IsDeleted == false).PublishDateTime;

				var newQuestionnaireList = METTLib.Questionnaire.QuestionnaireList.NewQuestionnaireList();
				//Create empty questionnaire object to duplicate
				var newQuestionnaire = new METTLib.Questionnaire.Questionnaire
				{
					//Set new object values 
					QuestionnaireName = "Copy of [ " + pQuestionnaireName + " - " + pQuestionnaireVersionNumber + " ]",
					QuestionnaireVersionNumber = "",
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddYears(1),
					PublishDateTime = DateTime.Now,
					PublishInd = false
				};
				//Save new questionnaire (1)
				if (newQuestionnaire.IsValid)
				{
					Singular.SaveHelper SaveNewQuestionnaireSaveHelper = newQuestionnaire.TrySave(typeof(METTLib.Questionnaire.QuestionnaireList));
					METTLib.Questionnaire.Questionnaire SaveNewQuestionnaire = (METTLib.Questionnaire.Questionnaire)SaveNewQuestionnaireSaveHelper.SavedObject;
					//SaveNewQuestionnaire.QuestionnaireID
					if (SaveNewQuestionnaireSaveHelper.Success)
					{
						//Update and return list object
						results.Data = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList();
						results.Success = true;
					}
				}
			}
			return results;
		}
	}

}




