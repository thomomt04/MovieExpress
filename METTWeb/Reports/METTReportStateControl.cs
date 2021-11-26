using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Singular.Web;
using Singular.Web.Reporting;

namespace METTWeb.Reports
{
	public class METTReportStateControl : Singular.Web.Controls.HelperControls.HelperBase<ReportVM>
	{
		private METTReportCriteriaControl mReportCriteriaControl;

		public METTReportCriteriaControl ReportCriteriaControl
		{
			get
			{
				return mReportCriteriaControl;
			}
		}

		protected override void Setup()
		{
			base.Setup();			

			if (ViewModel.Report == null)
				Helpers.Control(new METTWeb.Reports.METTReportMenuControl());
			else
			{
				mReportCriteriaControl = new METTWeb.Reports.METTReportCriteriaControl();
				Helpers.Control(mReportCriteriaControl);
			}
		}

		protected override void Render()
		{
			base.Render();

			RenderChildren();
		}
	}
}
