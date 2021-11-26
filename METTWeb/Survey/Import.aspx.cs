using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singular.Web.Data;
using Singular.Web;

namespace METTWeb.Survey
{
	public partial class Import : METTPageBase<ImportVM>
	{

	}

	public class ImportVM : METTStatelessViewModel<ImportVM>
	{
		public METTLib.ProtectedArea.ProtectedAreaList ProtectedAreaList { get; set; }
		public METTLib.ProtectedArea.ProtectedArea FirstProtectedArea { get; set; }
		public Singular.Documents.TemporaryDocumentNotRequired TempDoc { get; set; }
		public int paramProtectedAreaId { get; set; }
		public ImportVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();
			this.TempDoc = new Singular.Documents.TemporaryDocumentNotRequired();
			// Get protected area ID for import 
			if (Page.Request.Params["ProtectedAreaId"] != null)
			{
				paramProtectedAreaId = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["ProtectedAreaId"]).Replace(" ", "+")));
				ProtectedAreaList = METTLib.ProtectedArea.ProtectedAreaList.GetProtectedAreaList(paramProtectedAreaId);
				FirstProtectedArea = ProtectedAreaList.FirstOrDefault();
			}
			else
			{
				paramProtectedAreaId = 1;
			}
		}

		[Singular.Web.WebCallable(LoggedInOnly = true)]
		public string ViewProtectedArea(int ProtectedAreaID)
		{
			var url = "";
			url = $"../ProtectedArea/ProtectedAreaProfile.aspx?ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedAreaID.ToString()))}";

			return url;
		}

		[Singular.Web.WebCallable(LoggedInOnly = true)]
		public string ViewAssessment(string QuestionnaireAnswerSetId, string AssessmentStepId, string ProtectedAreaID)
		{
			var url = $"../Survey/Survey.aspx?QuestionnaireAnswerSetId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(QuestionnaireAnswerSetId.ToString()))}&QuestionnaireAnswerStepId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(AssessmentStepId.ToString()))}&ProtectedAreaID={ProtectedAreaID.ToString()}";
			return url;
		}

	}
}


