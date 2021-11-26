using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METTReports.Reports
{
	public class METTReportBase : METTLib.Reports.METTDynamicReport
	{
		public METTReportBase(Singular.Reporting.Dynamic.Report ReportInfo) : base(ReportInfo)
		{
		}
		public METTReportBase(Singular.Reporting.Dynamic.DynamicReport Report, Singular.Reporting.Dynamic.DynamicReportCriteria Criteria) : base(Report, Criteria)
		{
		}
	}
	public abstract class SFReportBase<RC> : METTLib.Reports.METTReportBase<RC> where RC : Singular.Reporting.ReportCriteria
	{
	}
}
