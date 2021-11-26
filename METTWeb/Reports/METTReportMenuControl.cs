using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Singular.Web;

namespace METTWeb.Reports
{

	public class METTReportMenuControl : Singular.Web.Controls.HelperControls.HelperBase<ReportVM>
	{
		protected override void Setup()
		{
			base.Setup();
			{
				var withBlock = Helpers.DivC("");
			}
			{
				var withBlock = Helpers.DivC("ibox paddingTop15");
				{
					var withBlock1 = withBlock.Helpers.DivC("row marginLeftRight0");
					foreach (Singular.Reporting.MainSection MS in Singular.Reporting.ReportFunctions.ProjectReportHierarchy.GetMainSections())
					{
						if (MS.IsAllowed())
						{

							// Column

							// With .Helpers.DivC("col-md-12")
							{
								var withBlock2 = withBlock1.Helpers.DivC("col-md-6");
								{
									var withBlock3 = withBlock2.Helpers.Div();
									withBlock3.AddClass("ibox");

									// Heading
									{
										var withBlock4 = withBlock3.Helpers.DivC("ibox-title");
										// With .Helpers.HTMLTag("i")
										// .AddClass("fa fa-university")
										// End With
										{
											var withBlock5 = withBlock4.Helpers.DivC("ibox-tools");
											{
												var withBlock6 = withBlock5.Helpers.HTMLTag("a");
												withBlock6.AddClass("collapse-link");
												{
													var withBlock7 = withBlock6.Helpers.HTMLTag("i");
													withBlock7.AddClass("fa fa-chevron-up");
												}
											}
										}
										if (MS.ImagePath != "")
											//withBlock4.Helpers.Image(MS.ImagePath).Style("margin-right") = "5px";
										withBlock4.Helpers.HTML(MS.Heading);
									}

									// Body
									{
										var withBlock4 = withBlock3.Helpers.DivC("ibox-content reportsIboxContentHeight");
										var TopLevelUL = withBlock4.Helpers.HTMLTag("ul");
										{
											var withBlock5 = TopLevelUL;

											// Sub Sections
											foreach (Singular.Reporting.SubSection SS in MS.SubSectionList)
											{
												if (SS.IsAllowed())
												{
													Singular.Web.CustomControls.HTMLTag<ReportVM> ReportUL = TopLevelUL;

													if (SS.Heading != "")
													{
														{
															var withBlock6 = withBlock5.Helpers.HTMLTag("li");
															// If the sub section has a heading, then create another sub un-ordered list
															withBlock6.Helpers.HTML(SS.Heading);
															ReportUL = withBlock6.Helpers.HTMLTag("ul");
														}
													}

													foreach (Singular.Reporting.Report R in SS.ReportList)
													{
														if (R.IsAllowed())
														{
															{
																var withBlock6 = ReportUL.Helpers.HTMLTag("li");
																//withBlock6.Attributes("title") = HttpUtility.HtmlEncode(R.Report.Description);

																// Add a link to the report.

																if (R.Report.ReportURL == "")
																{
																	if (R.Report.UniqueKey == "")
																	{
																		// Make a hash of the link so the user can't see the Type Name of the report.
																		string LinkHash = Singular.Reporting.ReportFunctions.GetHash(R.Report.GetType());
																		withBlock6.Helpers.LinkFor(null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, "?Type=" + Server.UrlEncode(LinkHash), R.Report.ReportName);
																	}
																	else
																		withBlock6.Helpers.LinkFor(null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, "?Key=" + Server.UrlEncode(R.Report.UniqueKey), R.Report.ReportName);
																}
																else
																{
																	string Url = R.Report.ReportURL;
																	withBlock6.Helpers.LinkFor(null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, Url.StartsWith("~") ? Utils.URL_ToAbsolute(Url) : Url, R.Report.ReportName, (LinkTargetType)R.Report.LinkTargetType);
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}


			{
				var withBlock = Helpers.Div();
				withBlock.Style.ClearBoth();
				//withBlock.Style("height") = "30px";
			}
		}

		protected override void Render()
		{
			base.Render();

			RenderChildren();
		}
	}
}