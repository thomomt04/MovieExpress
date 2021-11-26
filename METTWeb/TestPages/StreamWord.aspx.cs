
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
using Infragistics.Documents.Word;
using System.Drawing;
using System.IO;
using System.Data;

namespace METTWeb.TestPages
{
	public partial class StreamWord : METTPageBase<StreamWordVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class StreamWordVM : METTStatelessViewModel<StreamWordVM>
	{

		public METTLib.Reports.ReportInterventionManagementSphereList ReportInterventionManagementSphereList { get; set; }

		public METTLib.Reports.ROManagementSphereList ROManagementSphereList { get; set; }
		public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }

		public StreamWordVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();

		}

		[WebCallable]
		public void ExportNow(int QuestionnaireAnswerSetId)
		{
			QuestionnaireAnswerSetId = QuestionnaireAnswerSetId + 1;

		}


		[WebCallable]
		public static Singular.Web.Result ExportToWord()
		{
			var fileTimeStamp = DateTime.Now.ToString("ddMMyy-hhmmss");
			var fileName = "METT-ProtectedAreaName-" + fileTimeStamp + ".doc";
			Singular.Documents.TemporaryDocument tempDoc = new Singular.Documents.TemporaryDocument();
			tempDoc.SetDocument(CreateWord().ToArray(), fileName);
			return new Singular.Web.Result(true) { Data = Singular.Web.WebServices.FileDownloadHandler.SaveTempFile(tempDoc) };

		}

		[WebCallable]
		private static MemoryStream CreateWord()
		{
			//	private static MemoryStream CreateExcel(METTLib.Questionnaire.QuestionnaireAnswerExportSetList QuestionnaireAnswerExportSetList)
			//public void CreateWord()
			//{
			//	//Create a new instance of the WordDocumentWriter class
			MemoryStream m = new MemoryStream();


			WordDocumentWriter docWriter = WordDocumentWriter.Create(m);

			docWriter.Unit = UnitOfMeasurement.Point;

			docWriter.DocumentProperties.Title = "METT - Intervention Report";
			docWriter.DocumentProperties.Author = string.Format("Singular Systems", "");
			docWriter.StartDocument();

			Infragistics.Documents.Word.Font fontHeading = docWriter.CreateFont();
			fontHeading.Name = "Arial";
			fontHeading.Size = 22;
			fontHeading.Bold = true;

			Infragistics.Documents.Word.Font fontSubHeading = docWriter.CreateFont();
			fontSubHeading.Name = "Arial";
			fontSubHeading.Size = 16;
			fontSubHeading.Bold = true;

			Infragistics.Documents.Word.Font fontNormal = docWriter.CreateFont();
			fontNormal.Name = "Arial";
			fontNormal.Size = 9;
			fontNormal.Bold = false;

			Infragistics.Documents.Word.Font fontBold = docWriter.CreateFont();
			fontBold.Name = "Arial";
			fontBold.Size = 9;
			fontBold.Bold = true;

			Infragistics.Documents.Word.Font fontBoldLarge = docWriter.CreateFont();
			fontBoldLarge.Name = "Arial";
			fontBoldLarge.Size = 11;
			fontBoldLarge.Bold = true;

			Infragistics.Documents.Word.Font fontSmall = docWriter.CreateFont();
			fontSmall.Name = "Arial";
			fontSmall.Size = 8;
			fontSmall.Bold = false;

			docWriter.StartParagraph();
			docWriter.AddTextRun("Intervention Report", fontHeading);
			docWriter.EndParagraph();

			docWriter.StartParagraph();
			docWriter.AddTextRun("", fontHeading);
			docWriter.EndParagraph();

			//Specify the default parts for header and footer
			SectionHeaderFooterParts parts = SectionHeaderFooterParts.HeaderAllPages | SectionHeaderFooterParts.FooterAllPages;
			SectionHeaderFooterWriterSet writerSet = docWriter.AddSectionHeaderFooter(parts);
			//Set text for Header
			writerSet.HeaderWriterAllPages.Open();
			writerSet.HeaderWriterAllPages.StartParagraph();
			writerSet.HeaderWriterAllPages.AddTextRun("Management Effectiveness Tracking Tool-South Africa (METT) - Intervention Report [Protected Area]", fontSmall);
			writerSet.HeaderWriterAllPages.EndParagraph();
			writerSet.HeaderWriterAllPages.Close();
			// Set text for Footer
			writerSet.FooterWriterAllPages.Open();
			writerSet.FooterWriterAllPages.StartParagraph();
			writerSet.FooterWriterAllPages.AddTextRun("[This report was generated on " + DateTime.Now.ToString("dd/MM/yyyy") + "]", fontSmall);
			writerSet.FooterWriterAllPages.EndParagraph();
			writerSet.FooterWriterAllPages.Close();

			// MANAGEMENT SPHERES SUMMARY SECTION 
			// **********************************
			docWriter.StartParagraph();
			docWriter.AddTextRun("Management Sphere Summary", fontSubHeading);
			docWriter.EndParagraph();

			docWriter.StartParagraph();
			docWriter.AddTextRun("", fontHeading);
			docWriter.EndParagraph();

			TableBorderProperties borderProps = docWriter.CreateTableBorderProperties();
			borderProps.Color = Color.Black;
			borderProps.Style = TableBorderStyle.Double;
			TableProperties tableProps = docWriter.CreateTableProperties();
			tableProps.Layout = TableLayout.Fixed;
			tableProps.PreferredWidthAsPercentage = 100;
			tableProps.Alignment = ParagraphAlignment.Center;
			tableProps.BorderProperties.Color = borderProps.Color;
			tableProps.BorderProperties.Style = borderProps.Style;
			TableRowProperties rowProps = docWriter.CreateTableRowProperties();
			rowProps.IsHeaderRow = true;
			TableCellProperties cellProps = docWriter.CreateTableCellProperties();

			var ROManagementSphereList = METTLib.Reports.ROManagementSphereList.GetROManagementSphereList();

			foreach (var sphereitem in ROManagementSphereList)
			{
				cellProps.BackColor = Color.DarkGray;
				cellProps.TextDirection = TableCellTextDirection.LeftToRightTopToBottom;

				docWriter.StartTable(1, tableProps);
				docWriter.StartTableRow(rowProps);
				docWriter.StartTableCell(cellProps);

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun(sphereitem.ManagementSphereID + ". " + sphereitem.ManagementSphere, fontBoldLarge);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun(sphereitem.ManagementSphereContent, fontNormal);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.EndTableCell();
				docWriter.EndTableRow();
				docWriter.EndTable();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.StartTable(3, tableProps);
				docWriter.StartTableRow(rowProps);

				cellProps.PreferredWidthAsPercentage = 70;
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Indicators");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				cellProps.PreferredWidthAsPercentage = 15;
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Value");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				cellProps.PreferredWidthAsPercentage = 15;
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Rating (as %)");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				docWriter.EndTableRow();

				cellProps.Reset();
				cellProps.BackColor = Color.White;

				docWriter.EndTable();

				docWriter.StartTable(3, tableProps);

				int dTotalValue = 0;
				int dTotalRating = 0;
				decimal dTotalPerc = 0;

				var ReportInterventionManagementSphereList = METTLib.Reports.ReportInterventionManagementSphereList.GetReportInterventionManagementSphereList(284, sphereitem.ManagementSphereID);
				foreach (var item in ReportInterventionManagementSphereList)
				{
					dTotalValue = dTotalValue + item.MaxValue;
					dTotalRating = dTotalRating + item.AnswerRating;

					docWriter.StartTableRow();

					docWriter.StartTableCell(cellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(item.IndicatorDetailName, fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					docWriter.StartTableCell(cellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(Convert.ToString(item.MaxValue), fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					if (item.AnswerRating == 0)
					{
						cellProps.BackColor = Color.Red;
					}
					if (item.AnswerRating == 1)
					{
						cellProps.BackColor = Color.Yellow;
					}
					if (item.AnswerRating == 2)
					{
						cellProps.BackColor = Color.Yellow;
					}
					if (item.AnswerRating == 3)
					{
						cellProps.BackColor = Color.LightGreen;
					}

					docWriter.StartTableCell(cellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(Convert.ToString(item.AnswerRating), fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					cellProps.Reset();
					cellProps.BackColor = Color.White;

					docWriter.EndTableRow();
				}

				docWriter.StartTableRow();

				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Total", fontBold);
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun(Convert.ToString(dTotalValue), fontBold);
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				if (dTotalRating > 0)
				{
					if (dTotalValue > 0)
					{
						dTotalPerc = Math.Round((Convert.ToDecimal(dTotalRating) / Convert.ToDecimal(dTotalValue)) * 100, 2);
					}
					else
					{
						dTotalPerc = 0;
					}
				}
				else
				{
					dTotalPerc = 0;
				}

				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun(Convert.ToString(dTotalPerc), fontBold);
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				docWriter.EndTableRow();

				docWriter.EndTable();
				docWriter.StartParagraph();
				docWriter.AddTextRun("", fontHeading);
				docWriter.EndParagraph();
				docWriter.StartParagraph();
				docWriter.AddTextRun("", fontHeading);
				docWriter.EndParagraph();
			}

			docWriter.StartParagraph();
			docWriter.AddTextRun("", fontHeading);
			docWriter.EndParagraph();

			docWriter.StartParagraph();
			docWriter.AddTextRun("");
			docWriter.EndParagraph();

			// NEXT STEPS REPORT SECTION 
			// *************************

			docWriter.StartParagraph();
			docWriter.AddTextRun("Next Steps Summary", fontSubHeading);
			docWriter.EndParagraph();

			docWriter.StartParagraph();
			docWriter.AddTextRun("", fontHeading);
			docWriter.EndParagraph();

			TableBorderProperties NextStepsborderProps = docWriter.CreateTableBorderProperties();
			NextStepsborderProps.Color = Color.Black;
			NextStepsborderProps.Style = TableBorderStyle.Double;

			TableProperties NextStepstableProps = docWriter.CreateTableProperties();
			NextStepstableProps.Layout = TableLayout.Fixed;
			NextStepstableProps.PreferredWidthAsPercentage = 100;
			NextStepstableProps.Alignment = ParagraphAlignment.Left;
			NextStepstableProps.BorderProperties.Color = borderProps.Color;
			NextStepstableProps.BorderProperties.Style = borderProps.Style;
			TableRowProperties NextStepsrowProps = docWriter.CreateTableRowProperties();
			rowProps.IsHeaderRow = true;
			TableCellProperties NextStepscellProps = docWriter.CreateTableCellProperties();
			NextStepscellProps.BackColor = Color.DarkGray;
			NextStepscellProps.TextDirection = TableCellTextDirection.LeftToRightTopToBottom;

			var ReportInterventionNextStepsGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();
			foreach (var groupItem in ReportInterventionNextStepsGroupList)
			{
				docWriter.StartParagraph();
				docWriter.AddTextRun(groupItem.QuestionnaireGroup, fontBoldLarge);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.StartTable(3, NextStepstableProps);
				//HEADER ROW
				NextStepscellProps.BackColor = Color.DarkGray;
				docWriter.StartTableRow(NextStepsrowProps);
				NextStepscellProps.PreferredWidthAsPercentage = 15;
				docWriter.StartTableCell(NextStepscellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Indicator #");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				NextStepscellProps.PreferredWidthAsPercentage = 40;
				docWriter.StartTableCell(NextStepscellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Indicator");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				NextStepscellProps.PreferredWidthAsPercentage = 45;
				docWriter.StartTableCell(NextStepscellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Next Steps");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();

				NextStepscellProps.BackColor = Color.White;
				var iNo = 1;

				var ReportInterventionNextStepsList = METTLib.Reports.ReportInterventionNextStepList.GetReportInterventionNextStepList(284, groupItem.QuestionnaireGroupID);
				foreach (var item in ReportInterventionNextStepsList)
				{
					docWriter.StartTableRow();

					NextStepscellProps.PreferredWidthAsPercentage = 15;
					docWriter.StartTableCell(NextStepscellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(Convert.ToString(iNo), fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					NextStepscellProps.PreferredWidthAsPercentage = 40;
					docWriter.StartTableCell(NextStepscellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(item.IndicatorDetailName, fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					NextStepscellProps.PreferredWidthAsPercentage = 45;
					docWriter.StartTableCell(NextStepscellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(item.NextSteps, fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					docWriter.EndTableRow();
					iNo++;
				}
				docWriter.EndTable();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
			}

			docWriter.StartParagraph();
			docWriter.AddTextRun("");
			docWriter.EndParagraph();

			docWriter.EndDocument();


			docWriter.Close();


			m.Position = 0;

			//Document doc = new Document();
			//m.Position = 0;
			//doc.Save(m);

			return m;
		}

	}
}

