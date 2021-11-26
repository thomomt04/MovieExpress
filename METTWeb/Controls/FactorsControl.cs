using Singular.Web.MaintenanceHelpers;
using Singular.Web.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MEWeb.Controls
{
	public class FactorsControl : MaintenanceStateControl
	{

		protected override void Setup()
		{
			base.SetupNoMenu();

			MaintenanceVM VM = (MaintenanceVM)base.Model;

			if (VM.CurrentMaintenancePage == null)
			{
				Helpers.Control(new FactorMenu());
			}
			else
			{
				Helpers.Control(new FactorEditor(VM, base.OnRenderButton) { MaintenancePage = VM.CurrentMaintenancePage });
			}
		}
	}
}