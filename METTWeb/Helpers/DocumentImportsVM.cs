using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MEWeb.Helpers
{
  public class DocumentImportsVM : MEStatelessViewModel<DocumentImportsVM>
  {
		private static readonly string ASSESSMENTFILENAME = "Assessment File";

		public Singular.Documents.TemporaryDocumentNotRequired TempDoc { get; set; }

    protected override void Setup()
    {
      base.Setup();

      this.TempDoc = new Singular.Documents.TemporaryDocumentNotRequired();
    }

    public static Singular.Web.WebServices.FileUploadHandler.ResponseObject ImportExcel(byte[] FileStream, string Filename)
    {
      var StreamToRead = new MemoryStream(FileStream);
      var Result = new Singular.Web.WebServices.FileUploadHandler.ResponseObject();
      try
      {
        //Add Logic here to upload file
        MELib.Document.Excel ClientExcelFile = new MELib.Document.Excel(StreamToRead, Filename);

        if (ClientExcelFile.IsValidFile())
        {
					if (ClientExcelFile.GETDocumentNameSheet(0) == ASSESSMENTFILENAME)
					{						
						Result.Data = ClientExcelFile.SaveDataWithReturnPropValue("QuestionnaireAnswerSetID");
						Result.Success = true;
					}
					else
					{
						ClientExcelFile.SaveData();
						Result.Success = true;
					}
				}
        else
        {
          Result.Success = false;
          Result.Data = $"{ClientExcelFile.ValidationMessages[0].ErrorMessage}";
        }
      }
      catch (Exception e)
      {
        Result.Data = $"{e.Message}";
        Result.Success = false;
      }

      return Result;
    }

		public string ViewAssessment(int QuestionnaireAnswerSetId)
		{
			var url = "";
			url = $"../Survey/Survey.aspx?QuestionnaireAnswerSetId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(QuestionnaireAnswerSetId.ToString()))}&QuestionnaireAnswerStepId=FLZqAzYCjn9icNeLmtV%2bZQ%3d%3d";
			//1 - FLZqAzYCjn9icNeLmtV%2bZQ%3d%3d
			return url;
		}


	}
}