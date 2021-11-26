using Singular;
using Singular.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METTReports.Reports
{
	public class QuestionnaireAnswerSetCriteria : METTReportCriteria.ProtectedAreaCriteria
	{
		public int questionnaireAnswerSetID { get; set; }

	}

	public class ManagmentSphereReport : ReportBase<QuestionnaireAnswerSetCriteria>
	{

		protected override void SetupCommandProc(CommandProc cmd)
		{
			cmd.CommandText = "RptProcs.rptManagementSphereSummary";
		}

		public override string ReportName
		{
			get
			{
				return "Management Sphere Report";
			}
		}
	}
}



				