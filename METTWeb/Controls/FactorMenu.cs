using Singular.Web.MaintenanceHelpers;
using Singular.Web.Controls;

namespace MEWeb.Controls
{
	public class FactorMenu : MaintenanceMenu
	{
		protected override void Setup()
		{
			base.NoContentSetup();

			MaintenanceVM VM = (MaintenanceVM)base.Model;

			using (var h = this.Helpers)
			{
				var mainSectionDiv = h.Div();
				{
					foreach (var MS in VM.MainSectionList)
					{
						var rowDiv = mainSectionDiv.Helpers.DivC("row");
						{
							var gridDiv = rowDiv.Helpers.DivC("col-lg-12");
							{
								//var cardDiv = gridDiv.Helpers.DivC("ibox float-e-margins");
								//{
									//var cardTitleDiv = cardDiv.Helpers.DivC("ibox-title");
									//{
									//	cardTitleDiv.Helpers.HTML().Heading5(MS.Heading);
									//}
									//var cardToolsDiv = cardTitleDiv.Helpers.DivC("ibox-tools");
									//{
									//	var aToolsTag = cardToolsDiv.Helpers.HTMLTag("a");
									//	aToolsTag.AddClass("collapse-link");
									//	{
									//		var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
									//		iToolsTag.AddClass("fa fa-chevron-up");
									//	}
									//}
									var cardContentDiv = gridDiv.Helpers.DivC("ibox-content");
									{
										var rowDiv2 = cardContentDiv.Helpers.Div();
										{
											cardContentDiv.Helpers.HTML("<table class='table table-bordered table-responsive'><thead><tr><th>Factor Tables</th><th>View</th></tr></thead><tbody>");
											foreach (SubSection SS in MS.SubSectionList)
											{
												foreach (MaintenancePage mp in SS.MaintenancePageList)
												{
													cardContentDiv.Helpers.HTML("<tr><td>");
													cardContentDiv.Helpers.HTML(mp.DisplayName);
													cardContentDiv.Helpers.HTML("</td><td>");
													var link = cardContentDiv.Helpers.LinkFor(null, null, string.IsNullOrEmpty(mp.URL) ? $"?Type={mp.Hash}" : mp.URL, "View & Populate");
													link.AddClass("btn btn-sm btn-primary pull-right m-t-n-xs");
													//string url = string.IsNullOrEmpty(mp.URL) ? $"?Type={mp.Hash}" : mp.URL;
													//cardContentDiv.Helpers.HTML($"<button class='btn btn-sm btn-primary pull-right m-t-n-xs' onclick='{url}'><strong>View & Populate</strong></button>");
													cardContentDiv.Helpers.HTML("</td></tr>");
													
												}
											}
											cardContentDiv.Helpers.HTML("</tbody></table>");
										}
									}
								//}
							}
						}
					}
				}
			}

		}
	}
}