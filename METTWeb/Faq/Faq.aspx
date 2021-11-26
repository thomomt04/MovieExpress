<%@ Page Title="Faq" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Faq.aspx.cs" Inherits="METTWeb.Faq.Faq" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			h.HTML().Heading2("METT FAQ");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<%	using (var h = this.Helpers)
			{

				var MainHDiv = h.DivC("row");
				{
					var QuestionnaireGroupContainer = MainHDiv.Helpers.DivC("container");
					{
						var QuestionnaireGroupBreadcrumbRow = QuestionnaireGroupContainer.Helpers.DivC("row");
						{
							var MainDiv = MainHDiv.Helpers.DivC("col-md-12");
							{
								var FAQDiv = MainDiv.Helpers.Div();
								{
									var TabContainer = FAQDiv.Helpers.DivC("tabs-container");
									{
										var EntityTab = TabContainer.Helpers.TabControl();
										{
											EntityTab.Style.ClearBoth();
											EntityTab.AddClass("nav nav-tabs");

											var InstructionsTab = EntityTab.AddTab("Assessment Instructions");
											{
												InstructionsTab.Helpers.HTML("<h2>Assessment Instructions</h2>");
												InstructionsTab.Helpers.HTML("<p>1. Save the workbook with an appropriate name preferably the name of the site. In this way you will retain the original template for the next assessment.</p>");
												InstructionsTab.Helpers.HTML("<p>2. Enter the name of the site on the 'Cover' sheet.</p>");
												InstructionsTab.Helpers.HTML("<p>3. Read the 'Role' and 'Guidelines' sheets.</p>");
												InstructionsTab.Helpers.HTML("<p>4. Complete the 'Datasheet'.</p>");
												InstructionsTab.Helpers.HTML("<p>5. Complete the works sheets numbered 1-6 by selecting the most appropriately worded answer for each indicator from the 'drop down box'. If an indicator is not applicable, please provide adequate reasons in the 'Comments' column. As each sheet is filled in, click on the 'Submit' button. This will transfer the score to the 'Summary' sheet (7).</p>");
												InstructionsTab.Helpers.HTML("<p>6.  Make sure you fill in 'Comments and verification', 'Next steps' and list the 'Evidence produced' for each answer.</p>");
												InstructionsTab.Helpers.HTML("<p>7. After each sheet (Context, Planning, Inputs, Process, Outputs, Outcomes) has been completed, please check your ratings and once you are satisfied click 'Submit'. Ratings for this section will then be added to the 'Summary' sheet. It is important to do this after completing each sheet. If your entries are incomplete you will be prompted to complete the required entry.</p>");
												InstructionsTab.Helpers.HTML("<p>8. If you cannot complete the entire METT SA 3 assessment at one attempt you will need to save it. Apply the standard Excel 'Save As' function. Rename the workbook to the name of the Site and the date.  When you want to continue reopen the renamed workbook and complete the assessment.</p>");
												InstructionsTab.Helpers.HTML("<p>9. Complete the 'Verification' sheet</p>");
												InstructionsTab.Helpers.HTML("<p>10. After all sheets have been finalised, click the 'Click to complete assessment' button located on the 'Summary' Sheet. Be absolutely sure that you do not want to make any changes, because once you have clicked on the  'Click to complete assessment' button no editing will be allowed. A new workbook will be created, and using the standard Excel 'Save As' function rename this workbook to the name of the site assessed and the date. If Excel presents a dialog mentioing macro-enabled workbooks, click 'Yes' to save a macro-free workbook and continue saving the completed spreadsheet. Your completed METT SA 3 assessment is now finalised and can be emailed or printed.</p>");
												InstructionsTab.Helpers.HTML("<p>NOTE: If you did not save the original template as in Step 1, you can open the original completed workbook and by clicking on 'Click to clear previous inputs' below you will get a clean template to work with.</p>");
											}

											var ThreatsDetailsTab = EntityTab.AddTab("Role of the Assessment");
											{
												ThreatsDetailsTab.Helpers.HTML("<h2>The Role of the Assessment</h2>");
												ThreatsDetailsTab.Helpers.HTML("<p>The METT-SA 3a, is a rapid site level assessment tool which has been adapted for the South African context. It is based on the orginal tool developed by the World Bank and WWF. The system is based on the idea that good protected  area management follows a process that has six distinct stages or elements to apply the principles of adaptive management:</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>1 it begins with understanding the context (where are we now?) of existing values and threats,</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>2 progress through planning (where do we want to be?),</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>3 allocation of resources (inputs) (what do we need?),</p> ");
												ThreatsDetailsTab.Helpers.HTML("<p>4 as a result of management action (processes) (how do we go about it?), </p>");
												ThreatsDetailsTab.Helpers.HTML("<p>5 eventually produces products and services (outputs) (what were the results?),</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>6 that result in impacts (outcomes) (what did we achieve?).</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>This 2017 version has been compiled so that it can be applied to special nature reserves, national parks, provincial nature reserves, world heritage sites and marine protected areas / sites (including islands) and Ramsar sites. NOTE: Throughout the assessment the term 'site' has been used to cover all types of protected areas being assessed.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>When applying the METT-SA 3a it is important that the following be kept in mind:</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	The assessment is not a scorecard of the site manager's performance, but it is rather a reflection on the organisation's proficiency in area management.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	The METT-SA 3a, is intended to report on progress. Thus, the score is the baseline against which future assessments are made to see if there has been improvement in management effectiveness. It is, however, more important to track the ratings of individual indicators and the next steps required to improve management effectiveness for this particular element.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	It is site specific and the indices must thus not be used to compare between different sites or organisations.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	It is a useful tool to give indications of trends over time in the effective management of sites.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	It is not intended to replace more detailed assessments or reviews which may be used as part of adaptive management systems and it may even give an indication of where these more specific assessments are required.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	It has limitations in the quantitative measurement of outcomes and where possible the need for more objective assessments has been introduced.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	This version adjusts the total value where indicators are completed as not applicable.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	Often low ratings in some indicators can be a reflection on the organisation and are out of the control of the site manager. Thus under no circumstances should the performance of managers be measured against the results of the METT-SA 3a.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>▪	As this is an on site evaluation, it is important that all information required to verify ratings (e.g. status of declaration) is available on site. Where necessary the documentation produced must be recorded in the 'Evidence produced' column.</p>");
												ThreatsDetailsTab.Helpers.HTML("<p>Frequency of application. Tracking the trends in management effectiveness is a long term process and instant improvements are unlikely to be obtained.</p>");
											}

											var GeneralGuidelinesTab = EntityTab.AddTab("General Guidelines");
											{
												GeneralGuidelinesTab.Helpers.HTML("<h2>General Guidelines</h2>");
												GeneralGuidelinesTab.Helpers.HTML("<p>METT-SA 3a must be completed by a broadly composed team of site management staff with input from other relevant roleplayers in an open discussion. The involvement of project staff such as Working on Programmes e.g. Working  for Water and tourism operators is also highly recommended.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>The tracking tool has been designed to be easily answered by those managing the site. It is emphasised that best results are obtained if  the METT -SA 3a is completed in an open forum with active debate from all members on the ratings to be allocated without suppression of senior managers. Where possible, it is recommended to have the assistance of an external facilitator to allow for open debate and fostering the principle of peer review.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>It should not be a rushed process. Depending on the complexity of the site in question, at least a day should be set aside.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>When assessing a composite area made up of several protected areas / sites, it is advised to assess each entity separately and then calculate an average.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>All sections of the tracking tool should be completed. There are two sections:</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>1. Datasheet: This details key information on the site, its characteristics and values.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>2. Assessment Form:");
												GeneralGuidelinesTab.Helpers.HTML("<p>2.1 Indicators: The main part of the assessment form is a series of  indicators  (grouped into the six elements of protected area / site management) with a question.   A series of four alternative answers to the question (with rating ranging from 0-3) are provided against the question to help assessors make judgments as to the level of rating given.  0= No Management, 1=Inadequate Management, 2=Sound Management and 3= Best Practice. Choose the appropriate answer and  click on the 'Drop down box' and choose the correct rating. Allocation of a rating is inevitably an approximate process and there will be situations in which none of the four alternative answers precisely fits conditions in the site.  The evidence produced to verify the rating must be listed in the space provided.  It is however important that you choose the answer that is nearest and use the comments section to provide relevant comments. Although the automatic  system in this Excel version adjusts the rating for non applicable indicators, it is mandatory  to use the 'Comments' column to explain why it is not relevant. Where appropriate, there are binary indicators that only measure if a particular aspect is in place or not, and as such a rating of 1 or 0 is assigned with no degrees of variation (in the past these were referred to as supplementary indicators).</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>As the METT-SA 3a applies to several types of sites the indicators, questions and answers are broadly worded. Where necessary, explanatory notes are included on the interpretation of the indicator. These notes can be opened by hovering  the cursor over cells which have a red triangle in the upper right hand corner.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>2.2 Comments and Verification: A box next to each indicator allows for qualitative judgments to be justified by explaining why they were made (this could range from personal opinion, a reference document, monitoring results or external studies and assessments – the purpose being to give anyone reading the report an idea of why the assessment was made). In this section it is also suggest that respondents comment on the role/influence of WWF/ World Bank/C.A.P.E. or other externally funded projects, if appropriate. In some instances suggestions are made about what might be covered in the 'Comments' column. Comments are vital to ensure that when successive assessments are carried out, the assessors are able to understand the reason for the rating allocated. Also provide verification of your answer. e.g. for 1.1 Legal Status provide the official declaration details or the PA register listing in the 'Evidence produced' column.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>2.3 Next Steps: For each question respondents are asked to identify a long-term management need to further adaptive management at the site, if this is relevant. This is essential to identify actions needed and to identify potential projects for funding. To be effective these actions should be consolidated into a separate detailed 'Action or Interventions' document.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>2.4 Evidence Produced: For each indicator, respondents should document what methods of verification were used. This could be in the form of specific documents cited or physical checks that were done. Examples have been provided as 'Comments' in the cell where appropriate. A red triangle in the top right hand corner indicates where these have been added and by hovering the cursor over these cells the 'Comments' will appear. These should serve as a guideline and not as an exhaustive list of potential documents/methods used for verification.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>Total: The final total adjusted for non applicable items, is expressed as a percentage of those indicators applicable. As the METT-SA 3a  is a tool to assist in assessing progress in the specific protected area / site to which it has been applied, it is vital to remember that there  is no 'pass or fail'. The final total is a bench mark against which future evaluations will be made to see if there have been improvements. It is also vital that results are not compared with that of other areas. More important than the total are the ratings for individual indicators.  These give an indication of where priorities for remedial action should be set.</p>");
												GeneralGuidelinesTab.Helpers.HTML("<p>NOTE: If a rating has been selected and the submit button clicked, the ratings cannot be changed.</p>");
											}
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
			var urlParams = new URLSearchParams(window.location.search);
			console.log(urlParams.get('Tab')); 
			//TO DO: Set active tab, values passed from assessment page via querystring eg. Instructions, Role, Guidelines
			
		})
	</script>
</asp:Content>
