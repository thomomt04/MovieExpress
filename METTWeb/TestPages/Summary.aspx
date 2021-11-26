<%@ Page Title="METT Assessment" Language="C#" AutoEventWireup="true" CodeBehind="Summary.aspx.cs" MasterPageFile="~/Site.Master" Inherits="METTWeb.TestPages.Summary" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/assessment.css" rel="stylesheet" />
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
				var QuestionnaireGroupContainer = MainHDiv.Helpers.DivC("container");
				{
					var QuestionnaireGroupBreadcrumbRow = QuestionnaireGroupContainer.Helpers.DivC("row");
					{
						var MainDiv = MainHDiv.Helpers.DivC("col-md-12");
						{
							var AvailableThreatsDiv = MainDiv.Helpers.DivC("ibox float-e-margins paddingBottom");
							{
								var AvailableThreatsTitleDiv = AvailableThreatsDiv.Helpers.DivC("ibox-title");
								{
									AvailableThreatsTitleDiv.Helpers.HTML("<i class='fa fa-book fa-lg fa-fw pull-left'></i>");
									AvailableThreatsTitleDiv.Helpers.HTML().Heading5("Available Threats");
								}
								var AvailableThreatsToolsDiv = AvailableThreatsTitleDiv.Helpers.DivC("ibox-tools");
								{
									var aAvailableThreatsToolsTag = AvailableThreatsToolsDiv.Helpers.HTMLTag("a");
									aAvailableThreatsToolsTag.AddClass("collapse-link");
									{
										var iAvailableThreatsToolsTag = aAvailableThreatsToolsTag.Helpers.HTMLTag("i");
										iAvailableThreatsToolsTag.AddClass("fa fa-chevron-up");
									}
								}
								var ThreatsDivContentDiv = AvailableThreatsDiv.Helpers.DivC("ibox-content");
								{


									var ContentRow = ThreatsDivContentDiv.Helpers.DivC("row");
									{

										//var ContentInfo = ContentRow.Helpers.DivC("col-md-12");
										//{
										//	var QuestionnaireAnswers = ContentInfo.Helpers.ForEach<METTLib.Questionnaire.QuestionnaireGroupAnswer>(c => c.QuestionnaireGroupAnswerList);
										//	{
										//		var QuestionnaireAnswerChoice = QuestionnaireAnswers.Helpers.DivC("col-md-12  pad-top-5");
										//		{
										//			QuestionnaireAnswerChoice.Helpers.BootstrapEditorRowFor(c => c.QuestionnaireQuestionAnswerOptionID);
										//		}
										//		var QuestionnaireAnswersComments = QuestionnaireAnswers.Helpers.DivC("col-md-12  pad-top-5");
										//		{
										//			QuestionnaireAnswersComments.Helpers.BootstrapEditorRowFor(c => c.Comments);
										//		}
										//		var QuestionnaireAnswersNextSteps = QuestionnaireAnswers.Helpers.DivC("col-md-6  pad-top-5");
										//		{
										//			QuestionnaireAnswersNextSteps.Helpers.BootstrapEditorRowFor(c => c.NextSteps);
										//		}
										//		var QuestionnaireAnswersEvidence = QuestionnaireAnswers.Helpers.DivC("col-md-6  pad-top-5");
										//		{
										//			QuestionnaireAnswersEvidence.Helpers.BootstrapEditorRowFor(c => c.Evidence);
										//		}
										//	}
										//}

										#region ScopeMatrix
										var ScopeMatrix = ContentRow.Helpers.DivC("col-md-6");
										{
											var ThreatRatingMatrix = ScopeMatrix.Helpers.BootstrapTableFor<METTLib.RO.ROThreatMatrix>((c => c.ScopeROThreatMatrixList), false, false, "", true);

											var ThreatFirstRow = ThreatRatingMatrix.FirstRow;
											{


												var ThreatRatingItemName = ThreatFirstRow.AddReadOnlyColumn(c => c.RatingName);
												{
													ThreatRatingItemName.HeaderText = "";
													ThreatRatingItemName.Attributes.Add("style", "width:150px;");

												}
												var ThreatRatingItemVeryHigh = ThreatFirstRow.AddColumn("Very High");
												{
													ThreatRatingItemVeryHigh.Attributes.Add("style", "width:100px;text-align: center;");
													var ThreatRatingItemVeryHighInd = ThreatRatingItemVeryHigh.Helpers.DivC("b-r-xl");
													ThreatRatingItemVeryHighInd.Attributes.Add("style", "padding:5px;");
													var ThreatRatingItemVeryHighText = ThreatRatingItemVeryHighInd.Helpers.Span(c => c.VeryHighName);
													ThreatRatingItemVeryHighInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-info': $data.VeryHighName() == 'Low', 'bg-primary': $data.VeryHighName() == 'Medium', 'bg-warning': $data.VeryHighName() == 'High',  'bg-danger': $data.VeryHighName() == 'Very High'}");
												}

												var ThreatRatingItemHigh = ThreatFirstRow.AddColumn("High");
												{
													ThreatRatingItemHigh.Attributes.Add("style", "width:100px;text-align: center;");
													var ThreatRatingItemHighInd = ThreatRatingItemHigh.Helpers.DivC("b-r-xl");
													ThreatRatingItemHighInd.Attributes.Add("style", "padding:5px;");
													var ThreatRatingItemHighText = ThreatRatingItemHighInd.Helpers.Span(c => c.HighName);
													ThreatRatingItemHighInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-info': $data.HighName() == 'Low', 'bg-primary': $data.HighName() == 'Medium', 'bg-warning': $data.HighName() == 'High',  'bg-danger': $data.HighName() == 'Very High'}");
												}

												var ThreatRatingItemMedium = ThreatFirstRow.AddColumn("Medium");
												{
													ThreatRatingItemMedium.Attributes.Add("style", "width:100px;text-align: center;");
													var ThreatRatingItemMediumInd = ThreatRatingItemMedium.Helpers.DivC("b-r-xl");
													ThreatRatingItemMediumInd.Attributes.Add("style", "padding:5px;");
													var ThreatRatingItemMediumText = ThreatRatingItemMediumInd.Helpers.Span(c => c.MediumName);
													ThreatRatingItemMediumInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-info': $data.MediumName() == 'Low', 'bg-primary': $data.MediumName() == 'Medium', 'bg-warning': $data.MediumName() == 'High',  'bg-danger': $data.MediumName() == 'Very High'}");
												}

												var ThreatRatingItemLow = ThreatFirstRow.AddColumn("Low");
												{
													ThreatRatingItemLow.Attributes.Add("style", "width:100px;text-align: center;");
													var ThreatRatingItemLowInd = ThreatRatingItemLow.Helpers.DivC("b-r-xl");
													ThreatRatingItemLowInd.Attributes.Add("style", "padding:5px;");
													var ThreatRatingItemLowText = ThreatRatingItemLowInd.Helpers.Span(c => c.LowName);
													ThreatRatingItemLowInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-info': $data.LowName() == 'Low', 'bg-primary': $data.LowName() == 'Medium', 'bg-warning': $data.LowName() == 'High',  'bg-danger': $data.LowName() == 'Very High'}");
												}
											}
										}
										#endregion

										#region MagnitudeMatrix
										var MagnitudeMatrix = ContentRow.Helpers.DivC("col-md-6");
										{
											var ThreatRatingMatrix = MagnitudeMatrix.Helpers.BootstrapTableFor<METTLib.RO.ROThreatMatrix>((c => c.MagnitudeROThreatMatrixList), false, false, "", true);

											var ThreatFirstRow = ThreatRatingMatrix.FirstRow;
											{


												var ThreatRatingItemName = ThreatFirstRow.AddReadOnlyColumn(c => c.RatingName);
												{
													ThreatRatingItemName.HeaderText = "";
													ThreatRatingItemName.Attributes.Add("style", "width:150px;");

												}
												var ThreatRatingItemVeryHigh = ThreatFirstRow.AddColumn("Very High");
												{
													ThreatRatingItemVeryHigh.Attributes.Add("style", "width:100px;text-align: center;");
													var ThreatRatingItemVeryHighInd = ThreatRatingItemVeryHigh.Helpers.DivC("b-r-xl");
													ThreatRatingItemVeryHighInd.Attributes.Add("style", "padding:5px;");
													var ThreatRatingItemVeryHighText = ThreatRatingItemVeryHighInd.Helpers.Span(c => c.VeryHighName);
													ThreatRatingItemVeryHighInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-info': $data.VeryHighName() == 'Low', 'bg-primary': $data.VeryHighName() == 'Medium', 'bg-warning': $data.VeryHighName() == 'High',  'bg-danger': $data.VeryHighName() == 'Very High'}");
												}

												var ThreatRatingItemHigh = ThreatFirstRow.AddColumn("High");
												{
													ThreatRatingItemHigh.Attributes.Add("style", "width:100px;text-align: center;");
													var ThreatRatingItemHighInd = ThreatRatingItemHigh.Helpers.DivC("b-r-xl");
													ThreatRatingItemHighInd.Attributes.Add("style", "padding:5px;");
													var ThreatRatingItemHighText = ThreatRatingItemHighInd.Helpers.Span(c => c.HighName);
													ThreatRatingItemHighInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-info': $data.HighName() == 'Low', 'bg-primary': $data.HighName() == 'Medium', 'bg-warning': $data.HighName() == 'High',  'bg-danger': $data.HighName() == 'Very High'}");
												}

												var ThreatRatingItemMedium = ThreatFirstRow.AddColumn("Medium");
												{
													ThreatRatingItemMedium.Attributes.Add("style", "width:100px;text-align: center;");
													var ThreatRatingItemMediumInd = ThreatRatingItemMedium.Helpers.DivC("b-r-xl");
													ThreatRatingItemMediumInd.Attributes.Add("style", "padding:5px;");
													var ThreatRatingItemMediumText = ThreatRatingItemMediumInd.Helpers.Span(c => c.MediumName);
													ThreatRatingItemMediumInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-info': $data.MediumName() == 'Low', 'bg-primary': $data.MediumName() == 'Medium', 'bg-warning': $data.MediumName() == 'High',  'bg-danger': $data.MediumName() == 'Very High'}");
												}

												var ThreatRatingItemLow = ThreatFirstRow.AddColumn("Low");
												{
													ThreatRatingItemLow.Attributes.Add("style", "width:100px;text-align: center;");
													var ThreatRatingItemLowInd = ThreatRatingItemLow.Helpers.DivC("b-r-xl");
													ThreatRatingItemLowInd.Attributes.Add("style", "padding:5px;");
													var ThreatRatingItemLowText = ThreatRatingItemLowInd.Helpers.Span(c => c.LowName);
													ThreatRatingItemLowInd.AddBinding(Singular.Web.KnockoutBindingString.css, "{'bg-info': $data.LowName() == 'Low', 'bg-primary': $data.LowName() == 'Medium', 'bg-warning': $data.LowName() == 'High',  'bg-danger': $data.LowName() == 'Very High'}");
												}
											}
										}
										#endregion

									}
								}
							}
						}
					}
				}
			}
		}
	%>


	<script type="text/javascript">
		Singular.OnPageLoad(function () {
		});

	</script>
</asp:Content>
