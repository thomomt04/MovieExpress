
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
using METTLib.ProtectedArea;

namespace METTWeb.TestPages
{
	public partial class PageGrid : METTPageBase<PageGridVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
	public class PageGridVM : METTStatelessViewModel<PageGridVM>
	{

		public PagedDataManager<PageGridVM> ROProtectedAreaPagedListManager { get; set; }
		public ROProtectedAreaPagedList ROProtectedAreaPagedList { get; set; }
		public ROProtectedAreaPagedList.Criteria ROProtectedAreaPagedListCriteria { get; set; }

		public bool IsViewingProtectedAreaInd { get; set; }

		public PageGridVM()
		{

		}

		protected override void Setup()
		{
			base.Setup();

			// Protected Area Paged Grid
			ROProtectedAreaPagedListManager = new PagedDataManager<PageGridVM>((c) => c.ROProtectedAreaPagedList, (c) => c.ROProtectedAreaPagedListCriteria, "ProtectedAreaName", 20, true);
			ROProtectedAreaPagedListCriteria = new ROProtectedAreaPagedList.Criteria();
			ROProtectedAreaPagedList = (ROProtectedAreaPagedList)this.ROProtectedAreaPagedListManager.GetInitialData();


		}

	}
}
