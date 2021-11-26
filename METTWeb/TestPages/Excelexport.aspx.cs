
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
using System.IO;
using System.Text;
using Infragistics.Documents.Excel;

namespace METTWeb.TestPages
{
	public partial class ExcelExport : METTPageBase<ExcelExportVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class ExcelExportVM : METTStatelessViewModel<ExcelExportVM>
	{
		public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }
		public METTLib.ThreatCategories.ThreatCategoryList ThreatsList { get; set; }
		public METTLib.Questionnaire.QuestionnaireAnswerExportSetList QuestionnaireAnswerExportSetList { get; set; }
		public ExcelExportVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();
			ROQuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();
			QuestionnaireAnswerExportSetList = METTLib.Questionnaire.QuestionnaireAnswerExportSetList.GetQuestionnaireAnswerExportSetList();
			ThreatsList = METTLib.ThreatCategories.ThreatCategoryList.GetThreatCategoryList();
		}

		[WebCallable]
		public static Singular.Web.Result Export(METTLib.Questionnaire.QuestionnaireAnswerExportSetList QuestionnaireAnswerExportSetList, int QuestionnaireAnswerSetId)
		{
			//return new Singular.Documents.Document($"Provision{ClientName}_{ContractNumber}.xlsx", CreateExcel(ROGroups).ToArray());
			//return new Singular.Documents.Document($"Mett.xls", CreateExcel(ThreatsList).ToArray());
			//_QID_PAID_QASID
			//return new Singular.Web.Result(true) { Data = Singular.Web.WebServices.FileDownloadHandler.SaveTempFile($"METT_Q{QuestionnaireID}_PA{ProtectedAreaID}_QAS{QuestionnaireAnswerSetID}.xls", CreateExcel(ThreatsList).ToArray()) };
			//return new Singular.Web.Result(true) { Data = Singular.Web.WebServices.FileDownloadHandler.SaveTempFile($"METT_Q1_PA1_QAS1.xls", CreateExcel(ThreatsList).ToArray()) };

			//SAVE
			//var fileTimeStamp = DateTime.Now.ToString("ddMMyy-hhmmss");
			//File.WriteAllBytes(@"C:\Clients\METT\mett\METTWeb\Temp\METT-ProtectedAreaName-" + fileTimeStamp + ".xls", CreateExcel(QuestionnaireAnswerExportSetList).ToArray());

			//DOWNLOAD
			//return new Singular.Web.Result(true) { Data = Singular.Web.WebServices.FileDownloadHandler.SaveTempFile(tempDoc) };
			//	Singular.Web.WebServices.FileDownloadHandler.SendFile(System.Web.HttpContext.Current.Response, "Mett.pdf", new System.IO.MemoryStream(File.ReadAllBytes(@"C:\Clients\METT\mett\METTWeb\Temp\test.pdf")).ToArray(), false);
			//	Singular.Web.WebServices.FileDownloadHandler.SendFile(System.Web.HttpContext.Current.Response, "Mett.pdf", new MemoryStream(Encoding.UTF8.GetBytes(value ?? "")).ToArray(),true);
			Singular.Documents.TemporaryDocument tempDoc = new Singular.Documents.TemporaryDocument();

			tempDoc.SetDocument(CreateExcel(QuestionnaireAnswerExportSetList).ToArray(), "Mett.xls");
			//tempdoc = Singular.Web.WebServices.FileDownloadHandler.SendFile(System.Web.HttpContext.Current.Response, "Mett.xls", CreateExcel(QuestionnaireAnswerExportSetList).ToArray(), false);


			return new Singular.Web.Result(true) { Data = Singular.Web.WebServices.FileDownloadHandler.SaveTempFile(tempDoc) }; 
		}

		private static MemoryStream CreateExcel(METTLib.Questionnaire.QuestionnaireAnswerExportSetList QuestionnaireAnswerExportSetList)
		{
			Singular.Data.ExcelExporter ExcelDoc = new Singular.Data.ExcelExporter(Infragistics.Documents.Excel.WorkbookFormat.Excel97To2003);
			ExcelDoc.FormatAsTable = false;

			var QuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();

			foreach (var item in QuestionnaireGroupList)
			{
				//Create sheets for each Questionnaire Group
				var vSheet = ExcelDoc.WorkBook.Worksheets.Add(item.QuestionnaireGroup);

				var QAESList = METTLib.Questionnaire.QuestionnaireAnswerExportSetList.GetQuestionnaireAnswerExportSetList(item.QuestionnaireGroupID,0,0,0);
				ExcelDoc.PopulateData(QAESList, vSheet, false, false, 0, false);

				//Column renaming and alignment
				vSheet.Rows[0].Cells[1].Value = "No";
				vSheet.Columns[1].CellFormat.Alignment = Infragistics.Documents.Excel.HorizontalCellAlignment.Center;
				vSheet.Rows[0].Cells[2].Value = "Indicator";
				vSheet.Rows[0].Cells[7].Value = "Answer Option 1";
				vSheet.Rows[0].Cells[8].Value = "Answer Option 2";
				vSheet.Rows[0].Cells[9].Value = "Answer Option 3";
				vSheet.Rows[0].Cells[10].Value = "Answer Option 4";
				vSheet.Rows[0].Cells[11].Value = "Answer Option 5";
				vSheet.Rows[0].Cells[12].Value = "Your Answer";
				vSheet.Columns[12].CellFormat.Alignment = Infragistics.Documents.Excel.HorizontalCellAlignment.Center;
				vSheet.Rows[0].Cells[14].Value = "Next Steps";
				//ID Columns - Hide Columns
				vSheet.Columns[0].Width = 0;
				vSheet.Columns[3].Width = 0;
				vSheet.Columns[5].Width = 0;
				vSheet.Columns[6].Width = 0;
				vSheet.Columns[11].Width = 0;
				//Indicator Column Resizing
				vSheet.Columns[2].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
				vSheet.Columns[2].Width = 12500;
				//Question Column Resizing
				vSheet.Columns[4].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
				vSheet.Columns[4].Width = 15000;
				//Answer Columns Resizing
				vSheet.Columns[7].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
				vSheet.Columns[7].Width = 10500;
				vSheet.Columns[8].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
				vSheet.Columns[8].Width = 10500;
				vSheet.Columns[9].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
				vSheet.Columns[9].Width = 10500;
				vSheet.Columns[10].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
				vSheet.Columns[10].Width = 10500;
				vSheet.Columns[11].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
				vSheet.Columns[11].Width = 10500;
				//Frozen Column Headings
				vSheet.DisplayOptions.PanesAreFrozen = true;
				vSheet.DisplayOptions.FrozenPaneSettings.FrozenRows = 1;
				vSheet.Rows[0].CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(System.Drawing.Color.SeaGreen);
				//Change Row Height
				var iTotalRows = vSheet.Rows.Count();
				var headingColour = System.Drawing.ColorTranslator.FromHtml("#1ab394");
				vSheet.Rows[0].Height = 750;
				for (int i = 1; i < iTotalRows; i++)
				{
					if (i % 2 == 0)
					{
						vSheet.Rows[i].Height = 1550;
					}
					else
					{
						vSheet.Rows[i].Height = 1550;
						vSheet.Rows[i].CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(System.Drawing.Color.LightGray); 
					}
				}
			}
			//Save to File
			MemoryStream m = new MemoryStream();
			ExcelDoc.WorkBook.Save(m);
			return m;
		}

	}
}



//private static MemoryStream CreateExcel(METTLib.ThreatCategories.ThreatCategoryList ThreatsList)
//{
//	Singular.Data.ExcelExporter ExcelDoc = new Singular.Data.ExcelExporter(Infragistics.Documents.Excel.WorkbookFormat.Excel97To2003);
//	ExcelDoc.FormatAsTable = false;

//	var QuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();

//	//METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList

//	//var SheetsExport = ROQuestionnaireGroupList.

//	foreach (var item in QuestionnaireGroupList)
//	{
//		//Create sheets for each questionnaire group
//		var vSheet = ExcelDoc.WorkBook.Worksheets.Add(item.QuestionnaireGroup);
//		ExcelDoc.PopulateData(ThreatsList, vSheet, false, false, 0, false);
//	}

//	//var vSheet1 = ExcelDoc.WorkBook.Worksheets.Add("Sheet 1");
//	//ExcelDoc.PopulateData(ThreatsList, vSheet1, false, false, 0, false);

//	//var vSheet2 = ExcelDoc.WorkBook.Worksheets.Add("Sheet 2");
//	//ExcelDoc.PopulateData(ThreatsList, vSheet2, false, false, 0, false);

//	MemoryStream m = new MemoryStream();
//	ExcelDoc.WorkBook.Save(m);

//	return m;
//}

////METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList
//var SheetsExport = new METTLib.Questionnaire.ROQuestionnaireGroupList();
//int counter = 0;
//foreach ()
//{

//	counter++;
//}

//// WizzardUL.Helpers.ForEach<METTLib.Questionnaire.ROQuestionnaireGroup>(c => c.ROQuestionnaireGroupList);

