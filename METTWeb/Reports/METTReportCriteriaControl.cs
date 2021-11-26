using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Singular;
using Singular.Extensions;
using Singular.Reporting.Dynamic;
using Singular.Web;

namespace METTWeb.Reports
{

	public class METTReportCriteriaControl : Singular.Web.Controls.HelperControls.HelperBase<ReportVM>
	{
		private Singular.Web.CustomControls.Toolbar<Singular.Web.Reporting.ReportVM> mToolbar;

		public Singular.Web.CustomControls.Toolbar<Singular.Web.Reporting.ReportVM> Toolbar
		{
			get
			{
				return mToolbar;
			}
		}

		protected override void Setup()
		{
			base.Setup();

			Helpers.MessageHolder();

			Singular.Reporting.IReport Report = (Singular.Reporting.IReport)Model;

			if (Report.CustomCriteriaControlType == null)
			{
				Singular.Reporting.Dynamic.ROParameterList ParameterList = Report.ReportCriteriaGeneric.ParameterList;
				if (ParameterList != null)
					SetupDynamicReportCriteria(ParameterList);
				else
					SetupStronglyTypedReportCriteria(ViewModel.Report.ReportName);
			}
			else
			{
				Helpers.Control(Activator.CreateInstance(Report.CustomCriteriaControlType).GetType());
			}

			Report.CustomButtons.Where(c => !c.AfterNormalButtons).ToList().ForEach(AddCustomButton);

			if (Report.CrystalReportType != null & Report.HideCrystalReport == false)
			{
				{
					var withBlock = Helpers.Button("View as PDF");
					// .AddBinding(KnockoutBindingString.click, "ViewPDF()")
					withBlock.PostBackType = PostBackType.Full;
					withBlock.Image.Src = "~/Singular/Images/IconPDF.png";
					withBlock.Validate = true;
					withBlock.ID = "PDF";
				}
				if (Report.ShowWordExport)
				{
					{
						var withBlock = Helpers.Button("Word", "View As Word");
						withBlock.AddClass("btn btn-primary");
						withBlock.PostBackType = PostBackType.Full;
						withBlock.Image.Src = "~/Singular/Images/IconWord.png";
						withBlock.Validate = true;
						Style.BackgroundColour = "transparent";
					}
				}
			}
			if (Report.AllowDataExport)
			{
				{
					var withBlock = Helpers.Button("Export Data");
					withBlock.AddClass("btn btn-primary btn-outline");
					withBlock.PostBackType = PostBackType.Full;
					withBlock.Image.Src = "~/Singular/Images/IconExcelData.png";
					withBlock.Validate = true;
				}
			}

			var gi = Report.GridInfo;
			if (gi != null)
			{
				{
					var withBlock = Helpers.Button("GridData", "View Data");
					withBlock.AddClass("btn btn-info btn-outline");
					withBlock.Image.Src = "~/Singular/Images/IconExcelData.png";
					withBlock.AddBinding(KnockoutBindingString.click, "Singular.GridReport.ShowOptions(e)");
				}
			}

			Report.CustomButtons.Where(c => c.AfterNormalButtons).ToList().ForEach(AddCustomButton);

			{
				var withBlock = Helpers.Div();
				withBlock.Style.ClearBoth();
				withBlock.Style.Height = "30px";
			}
		}

		private void AddCustomButton(Singular.Reporting.CustomButton eb)
		{
			{
				var withBlock = Helpers.Button(eb.ButtonText);
				if (eb.ImageURL != "")
				{
					if (eb.ImageURL.StartsWith("fa fa"))
					{
						// font-awesome icon
						withBlock.Image.Glyph = FontAwesomeIcon.blank;
						withBlock.Image.AddClass(eb.ImageURL);
					}
					else
						withBlock.Image.Src = eb.ImageURL;
				}
				withBlock.PostBackType = PostBackType.Full;
				withBlock.AddBinding(KnockoutBindingString.ButtonArgument, eb.ButtonID.ToString().AddSingleQuotes());
				withBlock.Validate = true;
			}
		}

		private void SetupStronglyTypedReportCriteria(string reportName = "")
		{
			{
				var withBlock = Helpers.With<Singular.Reporting.ReportCriteria>(c => c.Report.ReportCriteriaGeneric);
				var StartDatePI = Singular.Reflection.GetProperty(ViewModel.Report.ReportCriteriaGeneric.GetType(), "StartDate");
				var EndDatePI = Singular.Reflection.GetProperty(ViewModel.Report.ReportCriteriaGeneric.GetType(), "EndDate");
				var OwnerPI = Singular.Reflection.GetProperty(ViewModel.Report.ReportCriteriaGeneric.GetType(), "OwnerCounterpartId");

				if (StartDatePI != null || EndDatePI != null)
				{
					// Add Date container
					{
						var withBlock1 = withBlock.Helpers.FieldSet("Date Selection");
						{
							var withBlock2 = withBlock1.Helpers.DivC("ibox");
							{
								var withBlock3 = withBlock2.Helpers.DivC("row marginLeftRight0");
								{
									var withBlock4 = withBlock3.Helpers.DivC("col-md-12 paddingTopBottom");
									{
										var withBlock5 = withBlock4.Helpers.DivC("ibox-title paddingTitle");
										withBlock5.Helpers.HTML("<i Class='fa fa-folder fa-lg fa-fw pull-left'></i>");
										withBlock5.Helpers.HTML().Heading5(reportName);
										{
											var withBlock6 = withBlock5.Helpers.DivC("ibox-tools toolsTopNone4");
											{
												var withBlock7 = withBlock6.Helpers.HTMLTag("a");
												withBlock7.AddClass("collapse-link");
												{
													var withBlock8 = withBlock7.Helpers.HTMLTag("i");
													withBlock8.AddClass("fa fa-chevron-up");
												}
											}
										}
									}
									{
										var withBlock5 = withBlock4.Helpers.DivC("ibox-content");
										{
											var withBlock6 = withBlock5.Helpers.DivC("row");
											// Start Date
											if (StartDatePI != null)
											{
												{
													var withBlock7 = withBlock6.Helpers.Div();
													withBlock7.Style.FloatLeft();
													//withBlock7.Style("padding") = "5px";
													{
														var withBlock8 = withBlock7.Helpers.HTMLTag("span");
														//withBlock8.Style.Padding(null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, "5px");
														withBlock8.Style.Display = Display.block;
														withBlock8.AddBinding(KnockoutBindingString.text, "'Start Date: '+ dateFormat(StartDate(), 'dd MMM yyyy')");
													}

													withBlock7.Helpers.Control(Singular.Web.CustomControls.EditorBase<ReportVM>.GetEditor(StartDatePI));
												}
											}

											// End Date
											if (EndDatePI != null)
											{
												{
													var withBlock7 = withBlock6.Helpers.Div();
													withBlock7.Style.FloatLeft();
													//withBlock7.Style("padding") = "5px";
													{
														var withBlock8 = withBlock7.Helpers.HTMLTag("span");
														//withBlock8.Style.Padding(null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, "5px");
														withBlock8.Style.Display = Display.block;
														withBlock8.AddBinding(KnockoutBindingString.text, "'End Date: '+ dateFormat(EndDate(), 'dd MMM yyyy')");
													}
													withBlock7.Helpers.Control(Singular.Web.CustomControls.EditorBase<ReportVM>.GetEditor(EndDatePI));
												}
											}
										}
									}
								}
							}
						}
					}
				}

				Singular.Web.CustomControls.FieldSet<Singular.Reporting.ReportCriteria, Singular.Reporting.ReportCriteria> mCriteriaGroup = null;

				{
					var withBlock1 = withBlock.Helpers.DivC("ibox");
					{
						var withBlock2 = withBlock1.Helpers.DivC("row marginLeftRight0");
						{
							var withBlock3 = withBlock2.Helpers.DivC("col-md-12 paddingTopBottom");
							{
								var withBlock4 = withBlock3.Helpers.DivC("ibox-title paddingTitle");
								withBlock4.Helpers.HTML("<i class='fa fa-file fa-lg fa-fw pull-left'></i>");
								withBlock4.Helpers.HTML().Heading5("Criteria");
								{
									var withBlock5 = withBlock4.Helpers.DivC("ibox-tools toolsTopNone4");
									{
										var withBlock6 = withBlock5.Helpers.HTMLTag("a");
										withBlock6.AddClass("collapse-link");
										{
											var withBlock7 = withBlock6.Helpers.HTMLTag("i");
											withBlock7.AddClass("fa fa-chevron-up");
										}
									}
								}
							}
							{
								var withBlock4 = withBlock3.Helpers.DivC("ibox-content");
								{
									var withBlock5 = withBlock4.Helpers.DivC("row");
									ViewModel.Report.ReportCriteriaGeneric.GetType().ForEachBrowsableProperty(null, pi =>
									{
										if (pi != StartDatePI && pi != EndDatePI)
										{
											if (mCriteriaGroup == null)
												mCriteriaGroup = withBlock5.Helpers.FieldSet("Criteria");

											if (pi != OwnerPI)
											{
												{
													var withBlock6 = mCriteriaGroup.Helpers.DivC("col-md-4");
													{
														var withBlock7 = withBlock6.Helpers.DivC("");
														{
															var withBlock8 = withBlock7.Helpers.LabelFor(pi);
															withBlock8.AddClass("control-label");
														}
														{
															var withBlock8 = withBlock7.Helpers.EditorFor(pi);
															withBlock8.AddClass("form-control");
														}
													}
												}
											}
											else
											{
												var withBlock6 = mCriteriaGroup.Helpers.DivC("col-md-2");
												{
													var withBlock7 = withBlock6.Helpers.DivC("");
													{
														var withBlock8 = withBlock7.Helpers.LabelFor(pi);
														withBlock8.AddClass("control-label");
													}
													{
														var withBlock8 = withBlock7.Helpers.EditorFor(pi);
														withBlock8.AddClass("form-control comboTriggerWidth noMargin-Bottom");
													}
												}
											}
										}
									}, false, true, true);
								}
							}
						}
					}
				}
			}
		}

		private void SetupDynamicReportCriteria(Singular.Reporting.Dynamic.ROParameterList ParameterList)
		{
			{
				var withBlock = Helpers.With<Singular.Reporting.ReportCriteria>(c => c.Report.ReportCriteriaGeneric);
				ROParameter StartDateP = ParameterList.Find("StartDate");
				ROParameter EndDateP = ParameterList.Find("EndDate");


				if (StartDateP != null || EndDateP != null)
				{
					// Add Date container
					{
						var withBlock1 = withBlock.Helpers.FieldSet("Date Selection");
						{
							var withBlock2 = withBlock1.Helpers.DivC("ibox");
							{
								var withBlock3 = withBlock2.Helpers.DivC("row");
								{
									var withBlock4 = withBlock3.Helpers.DivC("col-md-12 paddingTopBottom");
									{
										var withBlock5 = withBlock4.Helpers.DivC("ibox-content");
										{
											var withBlock6 = withBlock5.Helpers.DivC("row");
											// Julzy We need to add java script here for an on click event for the date picker "date" heading to change color - it also needs to be font size 16px
											// Start Date
											if (StartDateP != null)
											{
												{
													var withBlock7 = withBlock6.Helpers.Div();
													withBlock7.Style.FloatLeft();
													//withBlock7.Style("padding") = "5px";
													{
														var withBlock8 = withBlock7.Helpers.HTMLTag("span");
													//	withBlock8.Style.Padding(null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, "5px");
														//withBlock8.Style("font-size") = "16px";
														withBlock8.Style.Display = Display.block;
														withBlock8.AddBinding(KnockoutBindingString.text, "'Start Date: '+ dateFormat(StartDate(), 'dd MMM yyyy')");
													}

													Singular.Web.CustomControls.DateEditor<object> de = new Singular.Web.CustomControls.DateEditor<object>("StartDate", "", new Singular.DataAnnotations.DateField() { AlwaysShow = true, AutoChange = Singular.DataAnnotations.AutoChangeType.StartOfMonth, MaxDateProperty = EndDateP != null ? "EndDate" : "" });
													withBlock7.Helpers.Control(de);
												}
											}

											// End Date
											if (EndDateP != null)
											{
												{
													var withBlock7 = withBlock6.Helpers.Div();
													withBlock7.Style.FloatLeft();
													//withBlock7.Style("padding") = "5px";
													{
														var withBlock8 = withBlock7.Helpers.HTMLTag("span");
													//	withBlock8.Style.Padding(null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, "5px");
														//withBlock8.Style.FontSize() = "16px";
														withBlock8.Style.Display = Display.block;
														withBlock8.AddBinding(KnockoutBindingString.text, "'End Date: '+ dateFormat(EndDate(), 'dd MMM yyyy')");
													}

													Singular.Web.CustomControls.DateEditor<object> de = new Singular.Web.CustomControls.DateEditor<object>("EndDate", "", new Singular.DataAnnotations.DateField() { AlwaysShow = true, AutoChange = Singular.DataAnnotations.AutoChangeType.EndOfMonth, MinDateProperty = StartDateP != null ? "StartDate" : "" });
													withBlock7.Helpers.Control(de);
												}
											}
										}
									}
								}
							}
						}
					}
				}

				Singular.Web.CustomControls.FieldSet<Singular.Reporting.ReportCriteria, Singular.Reporting.ReportCriteria> mCriteriaGroup = null/* TODO Change to default(_) if this is not a reference type */;

				{
					var withBlock1 = withBlock.Helpers.DivC("ibox");
					{
						var withBlock2 = withBlock1.Helpers.DivC("row");
						{
							var withBlock3 = withBlock2.Helpers.DivC("col-md-12 paddingTopBottom");
							{
								var withBlock4 = withBlock3.Helpers.DivC("ibox-content");
								{
									var withBlock5 = withBlock4.Helpers.DivC("row");
									foreach (ROParameter Param in ParameterList)
									{
										if (Param != StartDateP && Param != EndDateP && Param.Visible)
										{
											if (mCriteriaGroup == null)
												mCriteriaGroup = withBlock5.Helpers.FieldSet("Criteria");

											Singular.Web.CustomControls.EditorBase<object> Editor = null/* TODO Change to default(_) if this is not a reference type */;

											Singular.Reporting.Dynamic.DynamicDropDown ddd = null/* TODO Change to default(_) if this is not a reference type */;
											if (Param.DropDownSource != null)
												ddd = Singular.Reporting.Dynamic.Settings.DropDowns.IncludeDatabaseDropDowns().GetItem(Param.DropDownSource);

											if (ddd != null)
											{
												// Drop down

												var di = ddd.GetDynamicInfo();
												if (di.Data != null)
												{ ViewModel.ClientDataProvider.AddDataSource(di.DropDownInfo.ClientName, di.Data, false); }
												Editor = new Singular.Web.CustomControls.DropDownEditor<object>(Param.ParameterName, "", di.DropDownInfo);
											}
											else

												// Other
												switch (Param.ParamDataType)
												{
													case Singular.Reflection.SMemberInfo.MainType.String:
														{
															Editor = new Singular.Web.CustomControls.TextEditor<object>(Param.ParameterName, "", null);
															break;
														}

													case Singular.Reflection.SMemberInfo.MainType.Number:
														{
															Editor = new Singular.Web.CustomControls.NumericEditor<object>(Param.ParameterName, "", null);
															break;
														}

													case Singular.Reflection.SMemberInfo.MainType.Date:
														{
															Editor = new Singular.Web.CustomControls.DateEditor<object>(Param.ParameterName, "", null);
															break;
														}

													case Singular.Reflection.SMemberInfo.MainType.Boolean:
														{
															Editor = new Singular.Web.CustomControls.CheckBoxEditor<object>(Param.ParameterName, "");
															break;
														}
												}



											mCriteriaGroup.Helpers.Control(new Singular.Web.CustomControls.EditorRow<object>(Editor, new Singular.Web.CustomControls.FieldLabel<object>(Param.ParameterName, Param.DisplayName)));
											//Editor.Style.Width = 300;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		protected override void Render()
		{
			base.Render();

			RenderChildren();
		}
	}

	public abstract class CriteriaControlBase : Singular.Web.Controls.HelperControls.HelperBase<ReportVM>
	{
		protected override void Render()
		{
			base.Render();
			RenderChildren();
		}
	}

}
