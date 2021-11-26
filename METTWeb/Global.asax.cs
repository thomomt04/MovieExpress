using System;
using Singular.Web;
using MELib;
using MELib.Security;

namespace MEWeb
{
	public class Global : ApplicationSettings<MEIdentity>
	{
		/// <summary>
		/// Setup the Application
		/// </summary>
		protected override void ApplicationSetup()
		{
			WebError.SupportsWebError = true;

			// Set the Common Data Reset Time
			CommonData.DefaultLifeTime = new TimeSpan(1, 0, 0);

			// Initialise Common Data Application Scope Lists
			CommonData.GetCachedLists();

			Singular.Web.Controls.Controls.DefaultButtonStyle = Singular.Web.ButtonStyle.Bootstrap;
			Singular.Web.Controls.Controls.DefaultButtonPostBackType = Singular.Web.PostBackType.Ajax;
			Singular.Web.Controls.Controls.DefaultDropDownType = Singular.DataAnnotations.DropDownWeb.SelectType.NormalDropDown;
			Singular.Web.Controls.Controls.UsesPagedGrid = true;
			Singular.Web.Scripts.Scripts.Settings.UseCDN = false;

			Singular.Documents.Settings.PassUserIDToGetProc = true;
			Singular.Documents.Settings.DocumentHashesEnabled = true;

			Singular.Emails.EMailBuilder.RegardsText = "Regards<br/>MEWeb";

			Singular.SystemSettings.General.RegisterSettingsClass<MELib.CorrespondenceSettings>();
			Singular.Web.Scripts.Scripts.Settings.LibJQueryVersion = Singular.Web.Scripts.ScriptSettings.JQueryVersion.JQ_1_12_4;
			Singular.Web.Scripts.Scripts.Settings.LibJQueryUIVersion = Singular.Web.Scripts.ScriptSettings.JQueryUIVersion.JQ_UI_1_12_1;
			Singular.Web.Controls.Controls.DefaultPageControlsType = Singular.Web.CustomControls.PageControlsType.ButtonsOnly;

     // Singular.Reporting.ReportFunctions.ProjectReportHierarchy = new METTReports.METTWebReportHierarchy();

      Singular.Reporting.Dynamic.Settings.DynamicReportsAutoSchema = "QuickReports";//,RptProcs";

			Singular.Reporting.Dynamic.Settings.DropDowns.DatabaseSchema = "DropDowns";

		//	Singular.Reporting.Dynamic.Settings.DynamicReportOverrideClass = typeof(METTReports.Reports.METTReportBase);

		}



		/// <summary>
		/// Handles the start session event
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The event arguments</param>
		public override void Session_Start(object sender, EventArgs e)
		{
			base.Session_Start(sender, e);

			// Initialise Common Data Session Scope Lists
			CommonData.InitialiseSessionLists();
		}
	}
}
