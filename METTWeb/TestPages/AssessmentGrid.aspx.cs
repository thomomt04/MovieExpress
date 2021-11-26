using System;
using MELib.Home.RO;
using Singular.Web.Data;

namespace MEWeb.TestPages
{
	public partial class AssessmentGrid : MEPageBase<AssessmentGridVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class AssessmentGridVM : MEStatelessViewModel<AssessmentGridVM>
	{

		//public PagedDataManager<PageGridVM> ROProtectedAreaPagedListManager { get; set; }
		//public ROProtectedAreaPagedList ROProtectedAreaPagedList { get; set; }
		//public ROProtectedAreaPagedList.Criteria ROProtectedAreaPagedListCriteria { get; set; }

		public ROAssessmentPagedList ROAssessmentPagedList { get; set; }
		public ROAssessmentPagedList.Criteria ROAssessmentPagedListCriteria { get; set; }
		public PagedDataManager<AssessmentGridVM> ROAssessmentPagedListManager { get; set; }


		public bool IsViewingProtectedAreaInd { get; set; }

		public AssessmentGridVM()
		{

		}

		protected override void Setup()
		{
			base.Setup();

			// Protected Area Paged Grid
			//ROProtectedAreaPagedListManager = new PagedDataManager<PageGridVM>((c) => c.ROProtectedAreaPagedList, (c) => c.ROProtectedAreaPagedListCriteria, "ProtectedAreaName", 20, true);
			//ROProtectedAreaPagedListCriteria = new ROProtectedAreaPagedList.Criteria();
			//ROProtectedAreaPagedList = (ROProtectedAreaPagedList)this.ROProtectedAreaPagedListManager.GetInitialData();

			ROAssessmentPagedListManager = new PagedDataManager<AssessmentGridVM>((d) => d.ROAssessmentPagedList, (d) => d.ROAssessmentPagedListCriteria, "MEReportingName", 5, true);
			ROAssessmentPagedListCriteria = new ROAssessmentPagedList.Criteria();
			ROAssessmentPagedList = (ROAssessmentPagedList)this.ROAssessmentPagedListManager.GetInitialData();

		}

	}
}
