//using METTLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METTReports
{
	public class METTWebReportHierarchy : Singular.Reporting.ReportHierarchy
	{



		protected override void SetupHeirarchy()
		{
			AddDymanicReports = true;
			AlwaysFetchDynamicReports = true;

			//var mainSection = MainSection("Management Reports1");

			//mainSection.Report(new METTReports.Reports.ManagmentSphereReport());

			//bool DebugMode = CommonData.Lists.ROSettingList.FirstOrDefault() != null ? CommonData.Lists.ROSettingList.FirstOrDefault().DebugMode : false;
			
			//var _with1 = MainSection("ILPC Reports");

			//_with1.Report(new METTReports.Reports.ECLProvision.ECLProvisionReport());
			//_with1.Report(new METTReports.Reports.ProvisionSummary.ProvisionSummaryReport());
			//_with1.Report(new METTReports.Reports.ExceptionRpt.ExceptionReport());
			//_with1.Report(new Reports.YearOnYearMETTAnalysisReport());

			//if (DebugMode)
			//{
			//	_with1.Report(new METTReports.Reports.ProvisionFactor.ProvisionFactorReport());
			
			//}

		}
	}
}
