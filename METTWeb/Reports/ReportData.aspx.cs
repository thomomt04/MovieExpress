using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace METTWeb.Reports
{
	public partial class ReportData : Singular.Web.PageBase<ReportDataVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		
		}
		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if(this.ViewModel.GridInfo != null && this.ViewModel.GridInfo.ExportFileName == "AverageScoreTrendsAnalysis")
			{
				this.ViewModel.GridInfo.Options.DefaultSummaryType = Singular.Web.CustomControls.SGrid.SummaryType.Average;
			}
		}
	}

	public class ReportDataVM : Singular.Web.Reporting.GridReportVM
	{

	}

}


