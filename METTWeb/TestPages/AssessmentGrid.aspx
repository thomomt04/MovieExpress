<%@ Page Title="Grid Example" Language="C#" AutoEventWireup="true" CodeBehind="AssessmentGrid.aspx.cs" MasterPageFile="~/Site.Master" Inherits="MEWeb.TestPages.AssessmentGrid" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/assessment.css" rel="stylesheet" />


	<script src="../Theme/Inspinia/js/plugins/jsKnob/jquery.knob.js" async=""></script>
	<script src="../Theme/Inspinia/js/plugins/jsKnob/angular-knob.js" async=""></script>

	<style>
		.qprogress {
			position: relative;
			margin: 4px;
			float: left;
			text-align: center;
			height: 40px;
		}

		.qbarOverflow { /* Wraps the rotating .bar */
			position: relative;
			overflow: hidden; /* Comment this line to understand the trick */
			width: 90px;
			height: 45px; /* Half circle (overflow) */
			margin-bottom: -14px; /* bring the numbers up */
		}

		.qbar {
			position: absolute;
			top: 0;
			left: 0;
			width: 90px;
			height: 90px; /* full circle! */
			border-radius: 50%;
			box-sizing: border-box;
			border: 8px solid #eee; /* half gray, */
			border-bottom-color: #1ab394; /* half azure */
			border-right-color: #1ab394;
		}
	</style>
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			h.HTML().Heading2("");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

	<%	
		using (var h = this.Helpers)
		{
			var MainHDiv = h.DivC("row");
			{
				var PanelContainer = MainHDiv.Helpers.DivC("container");
				{



					var PanelContainerRow0 = PanelContainer.Helpers.DivC("row");
					{
						var PanelContainerRowCol1 = MainHDiv.Helpers.DivC("col-md-3");
						{
							var PanelContainerBox = PanelContainerRowCol1.Helpers.DivC("ibox float-e-margins paddingBottom");
							{
								var PanelContainerContentBox = PanelContainerBox.Helpers.DivC("ibox-content");
								{
									var PanelContainerRowInner0 = PanelContainerContentBox.Helpers.DivC("row");
									{
										var PanelContainerRowInnerCol1 = PanelContainerRowInner0.Helpers.DivC("col-md-8");
										{
											PanelContainerRowInnerCol1.Helpers.HTML("<h2>Total Assessments</h2>");
											PanelContainerRowInnerCol1.Helpers.HTML("<p>Total assessments created.</p>");
										}
										var PanelContainerRowInnerCol2 = PanelContainerRowInner0.Helpers.DivC("col-md-4");
										{
											PanelContainerRowInnerCol2.Helpers.HTML("<span style='font-size:42px;font-weight:500'>32</span>");
										}
									}

								}

							}
						}

						var PanelContainerRowCol2 = MainHDiv.Helpers.DivC("col-md-3");
						{

							var PanelContainerBox = PanelContainerRowCol2.Helpers.DivC("ibox float-e-margins paddingBottom");
							{
								var PanelContainerContentBox = PanelContainerBox.Helpers.DivC("ibox-content");
								{
									var PanelContainerRowInner0 = PanelContainerContentBox.Helpers.DivC("row");
									{
										var PanelContainerRowInnerCol1 = PanelContainerRowInner0.Helpers.DivC("col-md-8");
										{
											PanelContainerRowInnerCol1.Helpers.HTML("<h2>To be Audited </h2>");
											PanelContainerRowInnerCol1.Helpers.HTML("<p>Total assessments for the month.</p>");
										}
										var PanelContainerRowInnerCol2 = PanelContainerRowInner0.Helpers.DivC("col-md-4");
										{
											PanelContainerRowInnerCol2.Helpers.HTML("<span style='font-size:42px;font-weight:500'>03</span>");
										}
									}
								}
							}

						}
						var PanelContainerRowCol3 = MainHDiv.Helpers.DivC("col-md-3");
						{

							var PanelContainerBox = PanelContainerRowCol3.Helpers.DivC("ibox float-e-margins paddingBottom");
							{
									var PanelContainerContentBox = PanelContainerBox.Helpers.DivC("ibox-content");
								{
									var PanelContainerRowInner0 = PanelContainerContentBox.Helpers.DivC("row");
									{
										var PanelContainerRowInnerCol1 = PanelContainerRowInner0.Helpers.DivC("col-md-8");
										{
											PanelContainerRowInnerCol1.Helpers.HTML("<h2>In Progress</h2>");
											PanelContainerRowInnerCol1.Helpers.HTML("<p>Assessments in progress.</p>");
										}
										var PanelContainerRowInnerCol2 = PanelContainerRowInner0.Helpers.DivC("col-md-4");
										{
											PanelContainerRowInnerCol2.Helpers.HTML("<span style='font-size:42px;font-weight:500'>01</span>");
										}
									}
								}
							}

						}
						var PanelContainerRowCol4 = MainHDiv.Helpers.DivC("col-md-3");
						{

							var PanelContainerBox = PanelContainerRowCol4.Helpers.DivC("ibox float-e-margins paddingBottom");
							{
								var PanelContainerContentBox = PanelContainerBox.Helpers.DivC("ibox-content");
								{
										var PanelContainerRowInner0 = PanelContainerContentBox.Helpers.DivC("row");
									{
										var PanelContainerRowInnerCol1 = PanelContainerRowInner0.Helpers.DivC("col-md-8");
										{
											PanelContainerRowInnerCol1.Helpers.HTML("<h2>Completed</h2>");
											PanelContainerRowInnerCol1.Helpers.HTML("<p>Total completed  by <b>##username##</b>.</p>");
										}
										var PanelContainerRowInnerCol2 = PanelContainerRowInner0.Helpers.DivC("col-md-4");
										{
											PanelContainerRowInnerCol2.Helpers.HTML("<span style='font-size:42px;font-weight:500'>01</span>");
										}
									}
								}
							}

						}
					}











					var PanelContainerRow = PanelContainer.Helpers.DivC("row");
					{
						var PanelContainerRowCol = MainHDiv.Helpers.DivC("col-md-12");
						{
							var PanelContainerBox = PanelContainerRowCol.Helpers.DivC("ibox float-e-margins paddingBottom");
							{
								var PanelContainerBoxTitle = PanelContainerBox.Helpers.DivC("ibox-title");
								{
									PanelContainerBoxTitle.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
									PanelContainerBoxTitle.Helpers.HTML().Heading5("Paged Grid Example");
								}
								var PanelContainerBoxTools = PanelContainerBoxTitle.Helpers.DivC("ibox-tools");
								{
									var PanelContainerBoxToolsTag = PanelContainerBoxTools.Helpers.HTMLTag("a");
									PanelContainerBoxToolsTag.AddClass("collapse-link");
									{
										var iPanelContainerBoxToolsTag = PanelContainerBoxToolsTag.Helpers.HTMLTag("i");
										iPanelContainerBoxToolsTag.AddClass("fa fa-chevron-up");
									}
								}
								var PanelContainerContentBox = PanelContainerBox.Helpers.DivC("ibox-content");
								{
									//var ContentRow = PanelContainerContentBox.Helpers.DivC("row");
									//{


									var FilterRow = PanelContainerContentBox.Helpers.DivC("row filterFrame");
									{
										var FilterAssessmentDateCol = FilterRow.Helpers.DivC("col-md-3");
										{
											FilterAssessmentDateCol.Helpers.LabelFor(c => c.ROAssessmentPagedListCriteria.AssessmentDate);
											FilterAssessmentDateCol.AddClass("control-label");

											var FilterAssessmentDateEditor = FilterAssessmentDateCol.Helpers.EditorFor(c => c.ROAssessmentPagedListCriteria.AssessmentDate);
											FilterAssessmentDateEditor.AddClass("form-control marginBottom20 filterBox");
										}

										var FilterNameCol = FilterRow.Helpers.DivC("col-md-3");
										{
											FilterNameCol.Helpers.LabelFor(c => c.ROAssessmentPagedListCriteria.AssessmentStepID);
											FilterNameCol.AddClass("control-label");

											var FilterNameEditor = FilterNameCol.Helpers.EditorFor(c => c.ROAssessmentPagedListCriteria.AssessmentStepID);
											FilterNameEditor.AddClass("form-control marginBottom20 filterBox");
										}


										var ButtonsCol = FilterRow.Helpers.DivC("col-md-6 paddingTop24");
										{

											ButtonsCol.Style.FloatRight();

											var ClearFilterCol = ButtonsCol.Helpers.Div();
											{
												var ClearBtn = ClearFilterCol.Helpers.Button("Clear", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.close);
												{
													ClearBtn.Style.FloatRight();
													ClearBtn.AddClass("btn-primary btn btn-outline");
													ClearBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ClearAssessmentFilters()");
												}
											}

											var NewCol = ButtonsCol.Helpers.Div();
											{
												var NewAssessmentBtn = NewCol.Helpers.Button("New Assessment", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.newspaper_o);
												{
													NewAssessmentBtn.Style.FloatRight();
													NewAssessmentBtn.AddClass("btn-primary btn btn btn-primary m-r");
													NewAssessmentBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "CreateNewAssessment()");

												}
											}


										}
									}

									var AssessmentDiv = PanelContainerContentBox.Helpers.DivC("row");
									{

										var AssessmentDivCol = AssessmentDiv.Helpers.DivC("col-lg-12");
										{

											var AssessmentPagedList = AssessmentDivCol.Helpers.PagedGridFor<METTLib.Home.ROAssessmentPaged>(c => c.ROAssessmentPagedListManager, c => c.ROAssessmentPagedList, false, false);
											{
												AssessmentPagedList.AddClass("table-responsive table table-bordered");

												var AssessmentFirstRow = AssessmentPagedList.FirstRow;
												{
													var METTReportingName = AssessmentFirstRow.AddReadOnlyColumn(c => c.METTReportingName);
													var AssessmentStep = AssessmentFirstRow.AddReadOnlyColumn(c => c.AssessmentStep);








													var AuditedBy = AssessmentFirstRow.AddReadOnlyColumn(c => c.AuditedBy);
													{
														AuditedBy.Style.TextAlign = Singular.Web.TextAlign.left;
													}

													var CreatedDate = AssessmentFirstRow.AddReadOnlyColumn(c => c.CreatedDateTime);
													{
														CreatedDate.Style.TextAlign = Singular.Web.TextAlign.left;
													}
													var CreatedBy = AssessmentFirstRow.AddReadOnlyColumn(c => c.CreatedBy);
													{
														CreatedBy.Style.TextAlign = Singular.Web.TextAlign.left;
													}

													var AcceptedInd = AssessmentFirstRow.AddColumn("Accepted");
													{
														AcceptedInd.Attributes.Add("style", "width:100px;text-align: center;");
														var AcceptedIndInd = AcceptedInd.Helpers.DivC("b-r-xl");
														var AcceptedIndText = AcceptedIndInd.Helpers.Span(c => c.AcceptedInd == true ? "Yes" : "No");
														AcceptedIndInd.Attributes.Add("style", "padding:5px;");
														AcceptedIndInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-primary': $data.AcceptedInd() == true, 'bg-danger': $data.AcceptedInd() == false}");

													}



													//	var ProgressIcon = AssessmentFirstRow.AddColumn("Progress");
													//{
													//	ProgressIcon.Attributes.Add("style", "width:150px;");
													//	ProgressIcon.Helpers.HTML("<div class='qprogress'><div class='qbarOverflow'><div class='qbar'></div></div><span>70</span>%</div>");
													//}

													var ViewCol = AssessmentFirstRow.AddColumn();
													{
														ViewCol.Style.Width = "150px";
														ViewCol.HeaderText = "View";

														var viewBtn = ViewCol.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
														{
															viewBtn.AddClass("btn btn-outline btn-primary");
															viewBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewAssessment($data)");
														}
													}
												}
											}
										}
									}

								}
							}

							//var ProtectedAreaDiv = TableRow.Helpers.DivC("row col-lg-12");
							//{

							//	var ProtectedAreaPagedList = ProtectedAreaDiv.Helpers.PagedGridFor<METTLib.ProtectedArea.ROProtectedAreaPaged>(c => c.ROProtectedAreaPagedListManager, c => c.ROProtectedAreaPagedList, false, false);
							//	{
							//		ProtectedAreaPagedList.AddClass("table-responsive table table-bordered");

							//		var ProtectedAreaFirstRow = ProtectedAreaPagedList.FirstRow;
							//		{
							//			var ProtectedAreaName = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.METTReportingName);
							//			var OfficialName = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.OfficialName);
							//			{
							//				OfficialName.Style.TextAlign = Singular.Web.TextAlign.left;
							//			}
							//			var OrganisationName = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.OrganisationName);
							//			{
							//				OfficialName.Style.TextAlign = Singular.Web.TextAlign.left;
							//			}
							//			var Region = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.Region);
							//			{
							//				Region.Style.TextAlign = Singular.Web.TextAlign.left;
							//			}
							//			var Province = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.Province);
							//			{
							//				Province.Style.TextAlign = Singular.Web.TextAlign.left;
							//			}

							//			var ViewCol = ProtectedAreaFirstRow.AddColumn();
							//			{
							//				ViewCol.Style.Width = "150px";
							//				ViewCol.HeaderText = "View";

							//				var viewBtn = ViewCol.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
							//				{
							//					viewBtn.AddClass("btn btn-outline btn-primary");
							//					viewBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewProtectedArea($data)");
							//				}
							//			}
							//		}
							//	}
							//}

						}

					}

				}
			}
		}
	%>


	<script type="text/javascript">
		Singular.OnPageLoad(function () {

		});

		var ViewAssessment = function (obj) {
			Singular.ShowLoadingBar;
			ViewModel.CallServerMethod('ManageAssessment', { QuestionnaireAnswerSetID: obj.QuestionnaireAnswerSetID(), AssessmentStepID: obj.AssessmentStepId(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					window.location = result.Data;
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})
		}

	</script>


</asp:Content>
