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
using Singular;
using METTLib.RO;

namespace METTWeb.Maintenance
{
	public partial class QuestionnaireSetup : METTPageBase<QuestionnaireSetupVM>
	{
	}
	public class QuestionnaireSetupVM : METTStatelessViewModel<QuestionnaireSetupVM>
	{
		public int SelectedTab { get; set; }
		public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }
		public METTLib.Questionnaire.QuestionnaireList QuestionnairesList { get; set; }
		public METTLib.Questionnaire.Questionnaire EditQuestionnaire { get; set; }
		public METTLib.QuestionnaireSurvey.QuestionnaireQuestionList QuestionnaireQuestionList { get; set; }
		public int FirstQuestionID { get; set; }
		public int FirstDefaultQuestionGroupID { get; set; }
		public int paramQuestionnaireId { get; set; }
		public bool AddQuestionInd { get; set; }
		public bool IsViewingQuestionInd { get; set; }
		public METTLib.Maintenance.MAQuestionnaireQuestionList MAQuestionnaireQuestionList { get; set; }
		public METTLib.Maintenance.MAQuestionnaireQuestion EditingMAQuestionnaireQuestion { get; set; }
		public int MAFirstQuestionID { get; set; }

		public METTLib.Maintenance.QuestionnaireQuestionManagementSphereList QuestionnaireQuestionManagementSphereList { get; set; }

		public METTLib.Maintenance.QuestionnaireQuestionLegalDesignationList QuestionnaireQuestionLegalDesignationsList { get; set; }


		public QuestionnaireSetupVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();

			if (Page.Request.Params["QuestionnaireId"] != null)
			{
				paramQuestionnaireId = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["QuestionnaireId"]).Replace(" ", "+")));
			}
			else
			{
				paramQuestionnaireId = 0;
				Page.Response.Redirect("../Maintenance/Questionnaires.aspx");
			}

			//Set assessment view - Indicators, Questions and Answers 
			QuestionnairesList = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList(paramQuestionnaireId, null);
			EditQuestionnaire = QuestionnairesList.FirstOrDefault();

			//FirstQuestionID = 0;
			//Breadcrumb/ Wizard Control
			ROQuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();

			//Default to the first group ID
			FirstDefaultQuestionGroupID = 1;

			MAQuestionnaireQuestionList = METTLib.Maintenance.MAQuestionnaireQuestionList.NewMAQuestionnaireQuestionList(1, paramQuestionnaireId);
			//MAFirstQuestionID = MAQuestionnaireQuestionList.FirstOrDefault().QuestionnaireQuestionID;
			EditingMAQuestionnaireQuestion = MAQuestionnaireQuestionList.FirstOrDefault();

		}

		[WebCallable]
		public static Result GetQuestion(int QuestionnaireQuestionID)
		{
			Result sr = new Result();
			try
			{
				METTLib.Maintenance.MAQuestionnaireQuestion EditingMAQuestionnaireQuestion = METTLib.Maintenance.MAQuestionnaireQuestionList.GetMAQuestionnaireQuestionList(QuestionnaireQuestionID).FirstOrDefault();
				var QuestionnaireQuestionManagementSphereList = METTLib.Maintenance.QuestionnaireQuestionManagementSphereList.GetQuestionnaireQuestionManagementSphereList(QuestionnaireQuestionID);
				var QuestionnaireQuestionLegalDesignationsList = METTLib.Maintenance.QuestionnaireQuestionLegalDesignationList.GetQuestionnaireQuestionLegalDesignationList(QuestionnaireQuestionID);

				Tuple<METTLib.Maintenance.MAQuestionnaireQuestion, METTLib.Maintenance.QuestionnaireQuestionManagementSphereList, METTLib.Maintenance.QuestionnaireQuestionLegalDesignationList> ReturnTuple = new Tuple<METTLib.Maintenance.MAQuestionnaireQuestion, METTLib.Maintenance.QuestionnaireQuestionManagementSphereList, METTLib.Maintenance.QuestionnaireQuestionLegalDesignationList>(EditingMAQuestionnaireQuestion, QuestionnaireQuestionManagementSphereList, QuestionnaireQuestionLegalDesignationsList);

				//sr.Data = EditingMAQuestionnaireQuestion;
				sr.Data = ReturnTuple;
				sr.Success = true;
			}
			catch (Exception e)
			{
				sr.Data = e.InnerException;
				sr.Success = false;
			}
			return sr;
		}

		//[WebCallable]
		//public static Result GetQuestion(int QuestionnaireQuestionID)
		//{
		//	Result sr = new Result();
		//	try
		//	{
		//		METTLib.Maintenance.MAQuestionnaireQuestion EditingMAQuestionnaireQuestion = METTLib.Maintenance.MAQuestionnaireQuestionList.GetMAQuestionnaireQuestionList(QuestionnaireQuestionID).FirstOrDefault();
		//		var QuestionnaireQuestionManagementSphereList = METTLib.Maintenance.QuestionnaireQuestionManagementSphereList.GetQuestionnaireQuestionManagementSphereList(QuestionnaireQuestionID);
		//		var QuestionnaireQuestionLegalDesignationsList = METTLib.Maintenance.QuestionnaireQuestionLegalDesignationList.GetQuestionnaireQuestionLegalDesignationList(QuestionnaireQuestionID);
		//		Tuple<METTLib.Maintenance.MAQuestionnaireQuestion, METTLib.Maintenance.QuestionnaireQuestionManagementSphereList, METTLib.Maintenance.QuestionnaireQuestionLegalDesignationList> ReturnTuple = new Tuple<METTLib.Maintenance.MAQuestionnaireQuestion, METTLib.Maintenance.QuestionnaireQuestionManagementSphereList, METTLib.Maintenance.QuestionnaireQuestionLegalDesignationList>(EditingMAQuestionnaireQuestion, QuestionnaireQuestionManagementSphereList, QuestionnaireQuestionLegalDesignationsList);
		//		//sr.Data = EditingMAQuestionnaireQuestion;
		//		sr.Data = ReturnTuple;
		//		sr.Success = true;
		//	}
		//	catch (Exception e)
		//	{
		//		sr.Data = e.InnerException;
		//		sr.Success = false;
		//	}
		//	return sr;
		//}

		[WebCallable]
		public static Tuple<METTLib.Maintenance.MAQuestionnaireQuestionList, int> GetSurveyQuestionnaireGroupData(int QGroupID, int QuestionnaireID)
		{
			METTLib.Maintenance.MAQuestionnaireQuestionList MAQuestionnaireQuestionList = METTLib.Maintenance.MAQuestionnaireQuestionList.NewMAQuestionnaireQuestionList(QGroupID, QuestionnaireID);
			int MAFirstQuestionID = MAQuestionnaireQuestionList.FirstOrDefault().QuestionnaireQuestionID;

			//Show answers from user when loading group information
			//METTLib.Questionnaire.QuestionnaireGroupAnswerList.GetQuestionnaireGroupAnswerList(QuestionnaireID, QGroupID);

			return Tuple.Create<METTLib.Maintenance.MAQuestionnaireQuestionList, int>(MAQuestionnaireQuestionList, MAFirstQuestionID);
		}
	}
}



