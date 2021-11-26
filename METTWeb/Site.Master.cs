using Singular.Web;
using System;
using System.Web.UI.WebControls;


namespace MEWeb
{
	public partial class Site : MasterBase
	{

		protected override void OnLoad(EventArgs e)
		{
			if (!IsPostBack)
			{
				//breadcrumbControl.InnerHtml = new METTWeb.CustomControls.BreadcrumbV2().Setup(SiteMapMain, Page.Request.Path);

				navMenuControl.InnerHtml = new MEWeb.CustomControls.LeftNavMenuV2().Setup(SiteMapMain);
			}

		}
}
}

