using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Csla;
using Singular.Web.Data;
using Singular.Web;
using METTLib.Organisation;
using METTLib.ProtectedArea;
using METTLib.Questionnaire;
using Singular;
using METTLib.RO;
using METTLib.Home;
using Infragistics.Documents.Word;
using System.Drawing;
using System.IO;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using METTLib.Helpers;

namespace METTWeb.Survey
{
	public partial class Survey : METTPageBase<SurveyVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}
	}

	public class SurveyVM : METTStatelessViewModel<SurveyVM>
	{


		public decimal AssessorScoreIndex { get; set; }
		public decimal AuditorScoreIndex { get; set; }

		public bool AddThreatInd { get; set; }
		public bool AuditedInd { get; set; }
		public bool AcceptedInd { get; set; }
		public bool IsUserAssessorInd { get; set; }
		public bool IsUserAuditorInd { get; set; }
		public bool IsViewingQuestionnaireMessageInd { get; set; }
		public bool IsViewingQuestionnaireContentInd { get; set; }
		public bool IsViewingFAQInstructionsInd { get; set; }
		public bool IsViewingFAQRoleInd { get; set; }
		public bool IsViewingFAQGuidelinesInd { get; set; }
		public bool IsViewingSurveyInd { get; set; }
		public bool IsViewingThreatAnswerInd { get; set; }
		public bool IsViewingThreatsTableInd { get; set; }
		public bool IsViewingThreatsMessageInd { get; set; }
		public bool ShowAssessmentResultsPageInd { get; set; }
		public bool PageViewInd { get; set; }
		public bool PageNewThreatAssessmentBtnInd { get; set; }
		public bool PageEditThreatAssessmentBtnInd { get; set; }
		public bool PageRemoveThreatAssessmentBtnInd { get; set; }
		public bool PageSaveAndNextAssessmentAssessmentBtnInd { get; set; }
		public bool PageSaveAndNextAssessmentAuditBtnInd { get; set; }
		public bool PageSubmitSummaryAssessmentBtnInd { get; set; }
		public bool PageSaveAssessmentResultsAssessmentBtnInd { get; set; }
		public bool PageRequestAuditResultsAssessmentBtnInd { get; set; }
		public bool PageGenerateReportResultsAssessmentBtnInd { get; set; }

		public bool PageViewOverviewTabInd { get; set; }
		public bool PageViewThreatsTabInd { get; set; }
		public bool PageViewAssessmentTabInd { get; set; }
		public bool PageViewSummaryTabInd { get; set; }
		public bool PageViewResultsTabInd { get; set; }

		public bool PageViewAssessmentBtnInd { get; set; }
		public bool PageDeleteAssessmentBtnInd { get; set; }

		public bool PageAssessmentControlsDisabledInd { get; set; }
		public bool PageAssessorControlsDisabledInd { get; set; }
		public bool PageAuditorControlsDisabledInd { get; set; }

		public bool PageSubmitAuditResultsAssessmentBtnInd { get; set; }

		[Singular.DataAnnotations.SetExpression("OnTabChanged()")]
		public int SelectedTab { get; set; }
		public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }
		public METTLib.Questionnaire.QuestionnaireList QuestionnairesList { get; set; }
		public METTLib.QuestionnaireSurvey.QuestionnaireQuestionList QuestionnaireQuestionList { get; set; }
		public int FirstQuestionID { get; set; }
		public METTLib.Questionnaire.QuestionnaireAnswerScoreList QuestionnaireAnswerScoreList { get; set; }
		public METTLib.Questionnaire.QuestionnaireAnswerScore FirstQuestionnaireAnswerScoreList { get; set; }
		public METTLib.Questionnaire.QuestionnaireAnswerSetList QuestionnaireAnswerSetList { get; set; }
		public METTLib.Questionnaire.QuestionnaireAnswerSet FirstQuestionnaireAnswerSet { get; set; }
		public METTLib.Questionnaire.QuestionnaireAnswerList QuestionnaireAnswerList { get; set; }
		public METTLib.Questionnaire.QuestionnaireAnswer FirstQuestionnaireAnswer { get; set; }
		public METTLib.Questionnaire.QuestionnaireGroupAnswerList QuestionnaireGroupAnswerList { get; set; }
		public METTLib.Questionnaire.QuestionnaireGroupAnswer FirstQuestionnaireGroupAnswer { get; set; }
		public METTLib.Threats.ThreatAnswerList ThreatAnswersList { get; set; }
		public METTLib.Threats.ThreatAnswer FirstThreatAnswers { get; set; }
		public METTLib.ThreatCategories.ThreatCategoryList ThreatsList { get; set; }
		public METTLib.Threats.ThreatAnswer EditThreatAnswer { get; set; }
		public METTLib.Threats.ThreatAnswer EditingThreatAnswer { get; set; }
		public METTLib.Threats.ThreatAnswer DeleteThreatAnswer { get; set; }
		public METTLib.RO.ROQuestionnaireGroupAnswerResultList ROQuestionnaireGroupAnswerResultList { get; set; }
		public METTLib.RO.ROQuestionnaireGroupAnswerResult FirstQuestionnaireGroupAnswerResult { get; set; }
		public METTLib.RO.ROQuestionnaireAnswerScoreList ROQuestionnaireAnswerScoreList { get; set; }
		public METTLib.RO.ROQuestionnaireAnswerScore FirstQuestionnaireAnswerScore { get; set; }

		public METTLib.RO.ROQuestionnaireAnswerScoreAssessorList ROQuestionnaireAnswerScoreAssessorList { get; set; }
		public METTLib.RO.ROQuestionnaireAnswerScoreAssessor FirstROQuestionnaireAnswerScoreAssessor { get; set; }
		public METTLib.RO.ROQuestionnaireAnswerScoreAuditorList ROQuestionnaireAnswerScoreAuditorList { get; set; }
		public METTLib.RO.ROQuestionnaireAnswerScoreAuditor FirstROQuestionnaireAnswerScoreAuditor { get; set; }


		public METTLib.RO.ROThreatMatrixList ScopeROThreatMatrixList { get; set; }
		public METTLib.RO.ROThreatMatrixList MagnitudeROThreatMatrixList { get; set; }
		public int paramQuestionnaireAnswerSetId { get; set; }
		public int paramAssessmentStepId { get; set; }
		public ROAssessmentPagedList ROAssessmentPagedList { get; set; }
		public ROAssessmentPagedList.Criteria ROAssessmentPagedListCriteria { get; set; }
		public PagedDataManager<SurveyVM> ROAssessmentPagedListManager { get; set; }
		public METTLib.Questionnaire.RO.ROQuestionnaireAnswerSetNationalBiomeList ROQuestionnaireAnswerSetNationalBiomeList { get; set; }
		public METTLib.Questionnaire.RO.ROQuestionnaireAnswerSetNationalBiome FirstROQuestionnaireAnswerSetNationalBiomeList { get; set; }
		public METTLib.Questionnaire.QuestionnaireGuidelineList FAQAssessmentInstructions { get; set; }
		public METTLib.Questionnaire.QuestionnaireGuideline FirstFAQAssessmentInstructions { get; set; }
		public METTLib.Questionnaire.QuestionnaireGuidelineList FAQRoleofAssessment { get; set; }
		public METTLib.Questionnaire.QuestionnaireGuideline FirstFAQRoleofAssessment { get; set; }
		public METTLib.Questionnaire.QuestionnaireGuidelineList FAQGuidelines { get; set; }
		public METTLib.Questionnaire.QuestionnaireGuideline FirstFAQGuidelines { get; set; }
		public METTLib.Reports.ReportInterventionManagementSphereList ReportInterventionManagementSphereList { get; set; }
		public METTLib.Reports.ROManagementSphereList ROManagementSphereList { get; set; }
		public Boolean CanViewAuditResultsSection { get; set; }
		public Boolean CanViewRequestAuditSection { get; set; }

		enum SecurityRoles
		{
			Create,
			Access,
			View
		}
		enum SectionNames
		{
			[Description("Web Application")]
			WebApplication,
			[Description("Reporting")]
			Reporting,
			[Description("Assessment")]
			Assessment,
			[Description("Assessment Overview")]
			AssessmentOverview,
			[Description("Assessment Threats")]
			AssessmentThreats,
			[Description("Assessment Summary")]
			AssessmentSummary,
			[Description("Assessment Results")]
			AssessmentResults,
			[Description("Assessment Questionnaire")]
			AssessmentQuestionnaire
		}

		enum SectionRoles
		{
			[Description("Edit Assessor")]
			EditAssessor,
			[Description("Edit Auditor")]
			EditAuditor,
			[Description("Submit Audit")]
			SubmitAudit
		}

		public ROSecurityOrganisationProtectedAreaGroupUserList ROSecurityOrganisationProtectedAreaGroupUserList { get; set; }

		public int ThreatMagnitudeRatingID { get; set; }

		public SurveyVM()
		{
		}

		protected override void Setup()
		{
			base.Setup();

			// Security based on multiple groups and roles for a selected user
			ROSecurityOrganisationProtectedAreaGroupUserList = METTLib.RO.ROSecurityOrganisationProtectedAreaGroupUserList.GetROSecurityOrganisationProtectedAreaGroupUserList(Singular.Security.Security.CurrentIdentity.UserID, null, null);
			// Groups / Roles functionality on various tabs
			PageViewInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
			PageNewThreatAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Threats.Create") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
			PageEditThreatAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Threats.Edit") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
			PageRemoveThreatAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Threats.Remove") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
			PageSaveAndNextAssessmentAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Questionnaire.Edit") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
			PageSubmitSummaryAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Summary.Submit") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SectionName == SectionNames.AssessmentSummary.Description() && (c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString()));
			PageSaveAssessmentResultsAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Results.Save") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SectionName == SectionNames.AssessmentResults.Description() && (c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString()));
			PageRequestAuditResultsAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Results.Request") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SectionName == SectionNames.AssessmentResults.Description() && (c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString()));
			PageGenerateReportResultsAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Results.Generate") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
			PageSubmitAuditResultsAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Results.Submit Audit") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SectionNames.AssessmentResults.Description() || c.SecurityRole == SectionRoles.SubmitAudit.Description());
			// Submit Audit
			// Groups / Roles functionality on assessment page grid
			PageViewAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment.View") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
			PageDeleteAssessmentBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment.Remove") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
			PageViewOverviewTabInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Overview.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SectionName == SectionNames.AssessmentOverview.Description() && (c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString()));
			PageViewThreatsTabInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Threats.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SectionName == SectionNames.AssessmentThreats.Description() && (c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString()));
			PageViewAssessmentTabInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Questionnaire.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SectionName == SectionNames.AssessmentQuestionnaire.Description() && (c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString()));
			PageViewSummaryTabInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Summary.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SectionName == SectionNames.AssessmentSummary.Description() && (c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString()));
			PageViewResultsTabInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Results.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SectionName == SectionNames.AssessmentResults.Description() && (c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString()));
			PageAssessmentControlsDisabledInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Questionnaire.Edit") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SectionNames.AssessmentQuestionnaire.Description() || c.SecurityRole == SecurityRoles.View.ToString());
			PageAssessorControlsDisabledInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Questionnaire.Edit Assessor") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SectionNames.AssessmentQuestionnaire.Description() || c.SecurityRole == SectionRoles.EditAssessor.Description());
			PageAuditorControlsDisabledInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Questionnaire.Edit Auditor") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SectionNames.AssessmentQuestionnaire.Description() || c.SecurityRole == SectionRoles.EditAuditor.Description());
			PageSaveAndNextAssessmentAuditBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Assessment Questionnaire.Edit Auditor") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SectionNames.AssessmentQuestionnaire.Description() || c.SecurityRole == SectionRoles.EditAuditor.Description());
			// General
			this.AddThreatInd = false;
			this.AuditedInd = false; // If AuditedInd is set to true a user is allowed to add audit answers and comments, next steps and evidence
			this.AcceptedInd = false; // If AcceptedInd is set to true a user is not allowed to edit answers and comments, next steps and evidence
			this.ShowAssessmentResultsPageInd = false;

			FAQAssessmentInstructions = METTLib.Questionnaire.QuestionnaireGuidelineList.GetQuestionnaireGuidelineList(1);
			FirstFAQAssessmentInstructions = FAQAssessmentInstructions.FirstOrDefault();

			FAQRoleofAssessment = METTLib.Questionnaire.QuestionnaireGuidelineList.GetQuestionnaireGuidelineList(2);
			FirstFAQRoleofAssessment = FAQRoleofAssessment.FirstOrDefault();

			FAQGuidelines = METTLib.Questionnaire.QuestionnaireGuidelineList.GetQuestionnaireGuidelineList(3);
			FirstFAQGuidelines = FAQGuidelines.FirstOrDefault();

			ROAssessmentPagedListManager = new PagedDataManager<SurveyVM>((d) => d.ROAssessmentPagedList, (d) => d.ROAssessmentPagedListCriteria, "METTReportingName", 10, true);
			ROAssessmentPagedListCriteria = new ROAssessmentPagedList.Criteria();
			ROAssessmentPagedList = (ROAssessmentPagedList)this.ROAssessmentPagedListManager.GetInitialData();

			// Get questionnaire information from Protected Areas Page/Object populate assessment information
			if (Page.Request.Params["QuestionnaireAnswerSetId"] != null)
			{
				paramQuestionnaireAnswerSetId = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["QuestionnaireAnswerSetId"]).Replace(" ", "+")));
			}
			else
			{
				paramQuestionnaireAnswerSetId = 0;
			}
			QuestionnaireAnswerSetList = METTLib.Questionnaire.QuestionnaireAnswerSetList.GetQuestionnaireAnswerSetList(paramQuestionnaireAnswerSetId);
			FirstQuestionnaireAnswerSet = QuestionnaireAnswerSetList.FirstOrDefault();

			//METT-267
			//we know that the logged in user has auditor rights
			if (FirstQuestionnaireAnswerSet != null && PageSubmitAuditResultsAssessmentBtnInd)
			{
				//catering for the unusual cases whereby the assessment has already been audited even though an audit was not requested
				if (FirstQuestionnaireAnswerSet.AuditedInd == false || FirstQuestionnaireAnswerSet.AuditDate != null)
				{
					PageSubmitAuditResultsAssessmentBtnInd = false;
				}
				//an audit has been requested and the assessment is already audited therefore user shouldn't be allowed to save audit again
				else if (FirstQuestionnaireAnswerSet.AuditedInd == true && FirstQuestionnaireAnswerSet.AuditDate != null)
				{
					PageSubmitAuditResultsAssessmentBtnInd = false;
				}
			}

			////logged in user has auditor rights to edit the assessment(this controls the disabling of the answer options and Save And Next button for an Auditor)
			if (FirstQuestionnaireAnswerSet != null && PageAuditorControlsDisabledInd && FirstQuestionnaireAnswerSet.CanAuditAssessment)
			{
				//can edit because user has the audit edit role
				PageAuditorControlsDisabledInd = false;

				//check if the assessment has been audited, if so lock the assessment from being audited
				if (FirstQuestionnaireAnswerSet.AuditDate != null)
				{
					PageAuditorControlsDisabledInd = true; //can't edit as the assessment has already been audited
				}

				//Request Audit section in Results tab will only be visible to the Auditor when the assessment is completed
				if(PageViewResultsTabInd && FirstQuestionnaireAnswerSet.AcceptedInd && FirstQuestionnaireAnswerSet.AssessmentDate != null && FirstQuestionnaireAnswerSet.CanAuditAssessment)
				{
					CanViewRequestAuditSection = true;
				}
				//Audit Results section in Results tab will only be visible to the Auditor when an audit has been requested on the assessment
				if (PageViewResultsTabInd && FirstQuestionnaireAnswerSet.AcceptedInd && FirstQuestionnaireAnswerSet.AuditedInd && FirstQuestionnaireAnswerSet.CanAuditAssessment)
				{
					CanViewAuditResultsSection = true;
				}
			}
			else
			{
				//cannot edit because user does not have the auditor role
				PageAuditorControlsDisabledInd = true;
				//if the user doesn't have an audit role on the assessment, they can only see the sections once the assessment has been audited
				if(FirstQuestionnaireAnswerSet != null)
				{ 
					if (PageViewResultsTabInd && FirstQuestionnaireAnswerSet.AcceptedInd && FirstQuestionnaireAnswerSet.AuditedInd && FirstQuestionnaireAnswerSet.AuditDate != null)
					{
						CanViewRequestAuditSection = true;
						CanViewAuditResultsSection = true;
					}
				}
			}

			//logged in as an auditor as only auditors have access to this role
			//Request Audit button visibility indicator
			if (FirstQuestionnaireAnswerSet != null && PageRequestAuditResultsAssessmentBtnInd)
			{
				//cannot request an audit on an assessment that has not been accepted, is already audited or requested for audit
				if (FirstQuestionnaireAnswerSet.AcceptedInd == false || FirstQuestionnaireAnswerSet.AuditedInd || FirstQuestionnaireAnswerSet.AuditDate != null || FirstQuestionnaireAnswerSet.CanAuditAssessment == false)
				{
					PageRequestAuditResultsAssessmentBtnInd = false;
				}
			}
		

			//Save AND Next button for Auditor
			if (FirstQuestionnaireAnswerSet != null && PageSaveAndNextAssessmentAuditBtnInd)
			{
				//must be visible when a request for audit has been submitted(AuditInd=true) and the assessment has not been audited(AuditDate=null) and the user is an auditor for that assessment
				if (FirstQuestionnaireAnswerSet.AuditedInd == false || FirstQuestionnaireAnswerSet.AuditDate != null || FirstQuestionnaireAnswerSet.CanAuditAssessment == false)
				{
					PageSaveAndNextAssessmentAuditBtnInd = false;
				}
			}


			// METT-144 Saving changes to an Assessment AFTER the Assessment Date (Completed Date) has been set must not be allowed.
			PageSaveAndNextAssessmentAssessmentBtnInd = FirstQuestionnaireAnswerSet != null ? FirstQuestionnaireAnswerSet.AssessmentDate == null ? true : false : false;


			// Last step completed when saving question answer
			if (Page.Request.Params["QuestionnaireAnswerStepId"] != null)
			{
				paramAssessmentStepId = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(Page.Request.Params["QuestionnaireAnswerStepId"]).Replace(" ", "+")));
				// Show auditor comments and answer options only once assessment has been marked as accepted.
				if (FirstQuestionnaireAnswerSet.AuditedInd == true)
				{
					AuditedInd = true;
				}
				if (FirstQuestionnaireAnswerSet.AcceptedInd == true)
				{
					AcceptedInd = true;
				}
			}
			else
			{
				paramAssessmentStepId = 1;
			}

			this.IsViewingQuestionnaireMessageInd = true; // Show assessment list
			this.IsViewingQuestionnaireContentInd = false; // Show assessment detail

			// Overview Tab
			this.IsViewingSurveyInd = true; // Show assessment detail
			this.IsViewingFAQInstructionsInd = false; // Show faq, role and guideline content
			this.IsViewingFAQRoleInd = false;
			this.IsViewingFAQGuidelinesInd = false;

			// var questionnaireAnswerSet = QuestionnaireAnswerSetList.Count;
			if (paramQuestionnaireAnswerSetId != 0)
			{
				this.IsViewingQuestionnaireMessageInd = false;
				this.IsViewingQuestionnaireContentInd = true;
				this.ShowAssessmentResultsPageInd = ShowAssessmentResultsPage(paramQuestionnaireAnswerSetId);
			}
			else
			{
				this.IsViewingQuestionnaireMessageInd = true;
				this.IsViewingQuestionnaireContentInd = false;
			};

			// Threats Tab
			this.IsViewingThreatAnswerInd = false;
			this.IsViewingThreatsTableInd = false;
			this.IsViewingThreatsMessageInd = true;

			ThreatMagnitudeRatingID = 0;

			this.ThreatsList = METTLib.ThreatCategories.ThreatCategoryList.GetThreatCategoryList();

			this.ThreatAnswersList = METTLib.Threats.ThreatAnswerList.GetThreatAnswerList(0, paramQuestionnaireAnswerSetId);
			FirstThreatAnswers = ThreatAnswersList.FirstOrDefault();

			var threatAnswers = ThreatAnswersList.Count;
			if (threatAnswers != 0)
			{
				IsViewingThreatsTableInd = true;
				IsViewingThreatsMessageInd = false;
			}
			else
			{
				IsViewingThreatsTableInd = false;
				IsViewingThreatsMessageInd = true;
			};

			// Matrix View - No longer needed replaced with images
			//ScopeROThreatMatrixList = METTLib.RO.ROThreatMatrixList.GetROThreatMatrixList(1);
			//MagnitudeROThreatMatrixList = METTLib.RO.ROThreatMatrixList.GetROThreatMatrixList(3);

			// Assessment Tab
			// Set assessment view - Indicators, Questions and Answers 
			this.QuestionnairesList = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList();
			FirstQuestionID = 0;
			ROQuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();

			if (paramQuestionnaireAnswerSetId != 0)
			{

				QuestionnaireQuestionList = METTLib.QuestionnaireSurvey.QuestionnaireQuestionList.NewQuestionnaireQuestionList(paramAssessmentStepId, paramQuestionnaireAnswerSetId);
				// getQuestionnaireQuestionsList returns zero due to legal designation not added in QuestionnaireAnswerSetLegalDesignations for a particular assessment
				if (QuestionnaireQuestionList.Count != 0)
				{
					FirstQuestionID = QuestionnaireQuestionList.FirstOrDefault().QuestionnaireQuestionID;
					QuestionnaireAnswerList = METTLib.Questionnaire.QuestionnaireAnswerList.GetQuestionnaireAnswerList(paramQuestionnaireAnswerSetId, 0);
				}

				// Biomes
				ROQuestionnaireAnswerSetNationalBiomeList = METTLib.Questionnaire.RO.ROQuestionnaireAnswerSetNationalBiomeList.GetROQuestionnaireAnswerSetNationalBiomeList(paramQuestionnaireAnswerSetId);
				FirstROQuestionnaireAnswerSetNationalBiomeList = ROQuestionnaireAnswerSetNationalBiomeList.FirstOrDefault();

				// Set user answers
				QuestionnaireGroupAnswerList = METTLib.Questionnaire.QuestionnaireGroupAnswerList.GetQuestionnaireGroupAnswerList(paramQuestionnaireAnswerSetId, 0);
				FirstQuestionnaireGroupAnswer = QuestionnaireGroupAnswerList.FirstOrDefault();

				// Summary Tab
				ROQuestionnaireGroupAnswerResultList = METTLib.RO.ROQuestionnaireGroupAnswerResultList.GetROQuestionnaireGroupAnswerResultList(paramQuestionnaireAnswerSetId);
				FirstQuestionnaireGroupAnswerResult = ROQuestionnaireGroupAnswerResultList.FirstOrDefault();

				// Results Tab
				ROQuestionnaireAnswerScoreList = METTLib.RO.ROQuestionnaireAnswerScoreList.GetROQuestionnaireAnswerScoreList(paramQuestionnaireAnswerSetId);
				FirstQuestionnaireAnswerScore = ROQuestionnaireAnswerScoreList.FirstOrDefault();

				ROQuestionnaireAnswerScoreAssessorList = METTLib.RO.ROQuestionnaireAnswerScoreAssessorList.GetROQuestionnaireAnswerScoreAssessorList(paramQuestionnaireAnswerSetId);
				FirstROQuestionnaireAnswerScoreAssessor = ROQuestionnaireAnswerScoreAssessorList.FirstOrDefault();

				UpdateAssessorIndex(paramQuestionnaireAnswerSetId);

				ROQuestionnaireAnswerScoreAuditorList = METTLib.RO.ROQuestionnaireAnswerScoreAuditorList.GetROQuestionnaireAnswerScoreAuditorList(paramQuestionnaireAnswerSetId);
				FirstROQuestionnaireAnswerScoreAuditor = ROQuestionnaireAnswerScoreAuditorList.FirstOrDefault();

			}
			// If an assessment is marked as accepted hide saving regardless of security groups and roles
			if (Page.Request.Params["QuestionnaireAnswerStepId"] != null)
			{
				if (FirstQuestionnaireAnswerSet.AcceptedInd == true)
				{
					PageSubmitSummaryAssessmentBtnInd = false;
					//only a reviewer is allowed to edit the questions after the assessment has been accepted/submitted by an assessor,
					//and the assessment is not yet submitted to DEA(assessment date is the indicator for that)
					if ((ROSecurityOrganisationProtectedAreaGroupUserList.Any(c => c.SecurityGroup == "Reviewer")) && FirstQuestionnaireAnswerSet.AssessmentDate == null)
					{
						PageSaveAndNextAssessmentAssessmentBtnInd = true;
						PageSaveAssessmentResultsAssessmentBtnInd = true;
						PageAssessorControlsDisabledInd = false;//can edit
					}
					else
					{
						PageSaveAndNextAssessmentAssessmentBtnInd = false;
						PageSaveAssessmentResultsAssessmentBtnInd = false;
						PageAssessorControlsDisabledInd = true;//can't edit
					}

				}
				else
				{
					//check if the user is a reviewer and lock edit as this assessment is not yet submitted for review
					if (ROSecurityOrganisationProtectedAreaGroupUserList.Any(c => c.SecurityGroup == "Reviewer"))
					{
						PageSaveAndNextAssessmentAssessmentBtnInd = false;
						PageSaveAssessmentResultsAssessmentBtnInd = false;
						PageAssessorControlsDisabledInd = true;//can't edit
					}
					else
					{
						//check if we have assessor access
						if (PageAssessorControlsDisabledInd == true)
						{
							PageAssessorControlsDisabledInd = false;//can still make changes as the assessment is not accepted
						}
						else
						{
							PageAssessorControlsDisabledInd = true;//cannot edit as we do not have assessor permissions
						}
					}

					if (ROSecurityOrganisationProtectedAreaGroupUserList.Any(c => c.SecurityGroup == "Assessor"))
					{
						PageSaveAndNextAssessmentAssessmentBtnInd = true;
						PageSaveAssessmentResultsAssessmentBtnInd = false;
						PageAssessorControlsDisabledInd = false;//can't edit
					}


				}
			}

			//who can see the 
		}

		[WebCallable]
		public static Result GetThreatMatrixRating(METTLib.Threats.ThreatAnswer EditingThreatAnswer)
		{
			Result sr = new Result();
			try
			{
				if (EditingThreatAnswer.SeverityRatingID != null && EditingThreatAnswer.ScopeRatingID != null)
				{
					var magnitudeRatingID = METTLib.CommonData.Lists.ROThreatMatrixScoreList?.FirstOrDefault(c => c.ThreatRatingItemNameOne == METTLib.Enums.ThreatRatingItems.Severity.ToString() && c.ThreatRatingItemNameTwo == METTLib.Enums.ThreatRatingItems.Scope.ToString() && c.ItemOneRatingID == EditingThreatAnswer.SeverityRatingID && c.ItemTwoRatingID == EditingThreatAnswer.ScopeRatingID)?.ScoreID;

					if (magnitudeRatingID != null)
					{
						EditingThreatAnswer.MagnitudeRatingID = magnitudeRatingID;
						sr.Success = true;
						sr.Data = EditingThreatAnswer;

					}
					else
					{
						sr.Success = false;
						sr.ErrorText = $"There is no threat rating matrix setup for {EditingThreatAnswer.SeverityRatingName} {METTLib.Enums.ThreatRatingItems.Severity} & {EditingThreatAnswer.ScopeRatingName}{METTLib.Enums.ThreatRatingItems.Scope}.";
						return sr;
					}
				}

				if (EditingThreatAnswer.MagnitudeRatingID != null && EditingThreatAnswer.IrreversibilityRatingID != null)
				{
					var overallRatingID = METTLib.CommonData.Lists.ROThreatMatrixScoreList?.FirstOrDefault(c => c.ThreatRatingItemNameOne == METTLib.Enums.ThreatRatingItems.Magnitude.ToString() && c.ThreatRatingItemNameTwo == METTLib.Enums.ThreatRatingItems.Irreversibility.ToString() && c.ItemOneRatingID == EditingThreatAnswer.MagnitudeRatingID && c.ItemTwoRatingID == EditingThreatAnswer.IrreversibilityRatingID)?.ScoreID;

					if (overallRatingID != null)
					{
						EditingThreatAnswer.RatingRatingID = overallRatingID;
						sr.Success = true;
						sr.Data = EditingThreatAnswer;
					}
					else
					{
						sr.Success = false;
						sr.ErrorText = $"There is no threat rating matrix setup for {EditingThreatAnswer.MagnitudeRatingName} {METTLib.Enums.ThreatRatingItems.Magnitude} & {EditingThreatAnswer.IrreversibilityRatingName} {METTLib.Enums.ThreatRatingItems.Irreversibility}.";
						return sr;
					}

				}

			}
			catch (Exception e)
			{
				sr.Data = e.InnerException;
				sr.Success = false;
			}
			return sr;
		}

		[WebCallable]
		public static Result GetThreatAnswer(int ThreatAnswerID, int QuestionnaireAnswerSetId)
		{
			Result sr = new Result();
			try
			{
				METTLib.Threats.ThreatAnswer EditThreatAnswer = METTLib.Threats.ThreatAnswerList.GetThreatAnswerList(ThreatAnswerID, QuestionnaireAnswerSetId).FirstOrDefault();

				sr.Data = EditThreatAnswer;
				sr.Success = true;
			}
			catch (Exception e)
			{
				sr.Data = e.InnerException;
				sr.Success = false;
			}
			return sr;
		}

		[WebCallable]
		public static Result GetQuestionnaireAnswerSet(int QuestionnaireAnswerSetID)
		{
			Result sr = new Result();
			try
			{
				METTLib.Questionnaire.QuestionnaireAnswerSet EditQuestionnaireAnswerSet = METTLib.Questionnaire.QuestionnaireAnswerSetList.GetQuestionnaireAnswerSetList(QuestionnaireAnswerSetID).FirstOrDefault();
				sr.Data = EditQuestionnaireAnswerSet;
				sr.Success = true;
			}
			catch (Exception e)
			{
				sr.Data = e.InnerException;
				sr.Success = false;
			}
			return sr;
		}

		//[WebCallable]
		//public static Tuple<METTLib.QuestionnaireSurvey.QuestionnaireQuestionList, int> GetSurveyQuestionnaireGroupData(int QGroupID, int QuestionnaireAnswerSetID)
		//{
		//	METTLib.QuestionnaireSurvey.QuestionnaireQuestionList QuestionnaireQuestionList = METTLib.QuestionnaireSurvey.QuestionnaireQuestionList.NewQuestionnaireQuestionList(QGroupID, QuestionnaireAnswerSetID);
		//	int FirstQuestionID = QuestionnaireQuestionList.FirstOrDefault().QuestionnaireQuestionID;
		//	// Show answers from user when loading group information
		//	METTLib.Questionnaire.QuestionnaireGroupAnswerList.GetQuestionnaireGroupAnswerList(QuestionnaireAnswerSetID, QGroupID);
		//	SaveProcessStepID(QuestionnaireAnswerSetID, QGroupID);
		//	return Tuple.Create<METTLib.QuestionnaireSurvey.QuestionnaireQuestionList, int>(QuestionnaireQuestionList, FirstQuestionID);
		//}

		[WebCallable]
		public Singular.Web.Result GetSurveyQuestionnaireGroupData(int QGroupID, int QuestionnaireAnswerSetID)
		{
			Result sr = new Result();
			try
			{
				METTLib.QuestionnaireSurvey.QuestionnaireQuestionList QuestionnaireQuestionList = METTLib.QuestionnaireSurvey.QuestionnaireQuestionList.NewQuestionnaireQuestionList(QGroupID, QuestionnaireAnswerSetID);
				int FirstQuestionID = QuestionnaireQuestionList.FirstOrDefault().QuestionnaireQuestionID;
				// Show answers from user when loading group information
				METTLib.Questionnaire.QuestionnaireGroupAnswerList.GetQuestionnaireGroupAnswerList(QuestionnaireAnswerSetID, QGroupID);
				SaveProcessStepID(QuestionnaireAnswerSetID, QGroupID);
				sr.Success = true;
				sr.Data = Tuple.Create<METTLib.QuestionnaireSurvey.QuestionnaireQuestionList, int>(QuestionnaireQuestionList, FirstQuestionID);
				return sr;
			}
			catch (Exception ex)
			{
				WebError.LogError(ex, "Survey.aspx", $"(int QGroupID, int QuestionnaireAnswerSetID) ({QGroupID},{QuestionnaireAnswerSetID})");
				sr.Success = false;
				sr.Data = null;
				sr.ErrorText = "Could not retrieve assessment information due to missing legal designation information for the protected area's assessment.";
				return sr;
			}
		}

		[WebCallable]
		public static Singular.SaveHelper SaveProcessStepID(int QuestionnaireAnswerSetID, int QGroupID)
		{
			// Saves last group user was viewing
			QuestionnaireAnswerSet QAS = QuestionnaireAnswerSetList.GetQuestionnaireAnswerSetList().FirstOrDefault(c => c.QuestionnaireAnswerSetID == QuestionnaireAnswerSetID);
			Singular.SaveHelper SH = new Singular.SaveHelper();
			try
			{
				if (QuestionnaireAnswerSetID != 0)
				{
					QAS.AssessmentStepID = QGroupID;

					SH = QAS.TrySave(typeof(QuestionnaireAnswerSetList));
				}
				else
				{

				}
			}
			catch (Exception e)
			{
			}
			return SH;
		}

		[WebCallable]
		public string ManageAssessment(int QuestionnaireAnswerSetId, int AssessmentStepId)
		{
			var url = $"../Survey/Survey.aspx?QuestionnaireAnswerSetId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(QuestionnaireAnswerSetId.ToString()))}&QuestionnaireAnswerStepId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(AssessmentStepId.ToString()))}";
			return url;
		}


		[WebCallable]
		public static Singular.Web.Result UpdateAssessorIndex(int QuestionnaireAnswerSetId)
		{
			Result sr = new Result();
			try
			{

				ROQuestionnaireAnswerScoreAssessorList QASAL = METTLib.RO.ROQuestionnaireAnswerScoreAssessorList.GetROQuestionnaireAnswerScoreAssessorList(QuestionnaireAnswerSetId);

				sr.Data = QASAL.FirstOrDefault().TotalIndexAssessorPct;
				sr.Success = true;
			}
			catch (Exception e)
			{
				sr.Data = e.InnerException;
				sr.Success = false;
			}
			return sr;

		}

		[WebCallable]
		public static Singular.Web.Result UpdateAuditorIndex(int QuestionnaireAnswerSetId)
		{
			Result sr = new Result();
			try
			{

				ROQuestionnaireAnswerScoreAuditorList QASAL = METTLib.RO.ROQuestionnaireAnswerScoreAuditorList.GetROQuestionnaireAnswerScoreAuditorList(QuestionnaireAnswerSetId);

				sr.Data = QASAL.FirstOrDefault().TotalIndexAuditorPct;
				sr.Success = true;
			}
			catch (Exception e)
			{
				sr.Data = e.InnerException;
				sr.Success = false;
			}
			return sr;

		}



		[WebCallable]
		public static Tuple<Singular.SaveHelper, METTLib.RO.ROQuestionnaireGroupAnswerResultList, METTLib.RO.ROQuestionnaireAnswerScoreList, Boolean> SaveQuestionAnswer(QuestionnaireAnswer QA, int QuestionnaireAnswerSetID)
		{
			QuestionnaireAnswerOnly QAO = QuestionnaireAnswerOnlyList.GetQuestionnaireAnswerOnlyList(QA.QuestionnaireQuestionTypeID, QuestionnaireAnswerSetID)?.FirstOrDefault();
			Singular.SaveHelper SH = new Singular.SaveHelper();
			if (QAO != null)
			{
				QAO.QuestionnaireAnswerSetID = QA.QuestionnaireAnswerSetID;
				QAO.QuestionnaireQuestionTypeID = QA.QuestionnaireQuestionTypeID;
				// Assessor
				QAO.QuestionnaireQuestionAnswerOptionID = QA.QuestionnaireQuestionAnswerOptionID;
				QAO.Comments = QA.Comments;
				QAO.NextSteps = QA.NextSteps;
				QAO.Evidence = QA.Evidence;
				// Auditor
				QAO.QuestionnaireQuestionAnswerOptionIDAuditor = QA.QuestionnaireQuestionAnswerOptionIDAuditor;
				QAO.CommentsAuditor = QA.CommentsAuditor;
				QAO.NextStepsAuditor = QA.NextStepsAuditor;
				QAO.EvidenceAuditor = QA.EvidenceAuditor;
				SH = QAO.TrySave(typeof(QuestionnaireAnswerOnlyList));

			}
			else
			{
				SH = QA.TrySave(typeof(QuestionnaireAnswerList));
			}

			var TotalQuestionsOutstanding = METTLib.RO.ROQuestionnaireGroupAnswerResultList.GetROQuestionnaireGroupAnswerResultList(QuestionnaireAnswerSetID);

			return Tuple.Create(SH, METTLib.RO.ROQuestionnaireGroupAnswerResultList.GetROQuestionnaireGroupAnswerResultList(QuestionnaireAnswerSetID), METTLib.RO.ROQuestionnaireAnswerScoreList.GetROQuestionnaireAnswerScoreList(QuestionnaireAnswerSetID), !TotalQuestionsOutstanding.Any(c => c.GroupTotalAnswers > 0));
		}

		[WebCallable]
		public static Singular.Web.Result InterventionRpt(int QuestionnaireAnswerSetId)
		{
			Result sr = new Result();
			try
			{
				var fileTimeStamp = DateTime.Now.ToString("ddMMyy-hhmmss");
				var ProtectedAreaInformation = METTLib.Questionnaire.QuestionnaireAnswerSetList.GetQuestionnaireAnswerSetList(QuestionnaireAnswerSetId)?.FirstOrDefault();

				string ProtectedAreaName = ProtectedAreaInformation?.METTReportingName ?? string.Empty;
				string ReportFileName = $"METT-InterventionReport-{ProtectedAreaName}-{fileTimeStamp}.doc";

				Singular.Documents.TemporaryDocument tempDoc = new Singular.Documents.TemporaryDocument();
				tempDoc.SetDocument(ExportInterventionReportToMSWord(QuestionnaireAnswerSetId).ToArray(), ReportFileName);
				sr.Success = true;
				return new Singular.Web.Result(true) { Data = Singular.Web.WebServices.FileDownloadHandler.SaveTempFile(tempDoc) };
			}
			catch (Exception e)
			{
				sr.Data = e.InnerException;
				sr.Success = false;
			}
			return sr;
		}

		[WebCallable]
		public Boolean ShowAssessmentResultsPage(int QuestionnaireAnswerSetId)
		{
			// Check if all questions have been answered and show results section on assessment if none outstanding
			var TotalQuestionsOutstanding = METTLib.RO.ROQuestionnaireGroupAnswerResultList.GetROQuestionnaireGroupAnswerResultList(QuestionnaireAnswerSetId);
			int TotalAnswersNotAnswered = TotalQuestionsOutstanding.Sum(item => item.GroupTotalAnswers.Value);
			if (TotalAnswersNotAnswered == 0)
			{
				ShowAssessmentResultsPageInd = true;
			}
			return ShowAssessmentResultsPageInd;
		}

		[WebCallable]
		private static MemoryStream ExportInterventionReportToMSWord(int QuestionnaireAnswerSetId)
		{
			MemoryStream m = new MemoryStream();
			WordDocumentWriter docWriter = WordDocumentWriter.Create(m);

			docWriter.Unit = UnitOfMeasurement.Point;
			docWriter.DocumentProperties.Title = "METT - Intervention Report";
			docWriter.DocumentProperties.Author = string.Format("Singular Systems", "");
			docWriter.StartDocument();

			Infragistics.Documents.Word.Font fontHeading = docWriter.CreateFont();
			fontHeading.Name = "Arial";
			fontHeading.Size = 22;
			fontHeading.Bold = true;
			Infragistics.Documents.Word.Font fontSubHeading = docWriter.CreateFont();
			fontSubHeading.Name = "Arial";
			fontSubHeading.Size = 16;
			fontSubHeading.Bold = true;
			Infragistics.Documents.Word.Font fontNormal = docWriter.CreateFont();
			fontNormal.Name = "Arial";
			fontNormal.Size = 9;
			fontNormal.Bold = false;
			Infragistics.Documents.Word.Font fontBold = docWriter.CreateFont();
			fontBold.Name = "Arial";
			fontBold.Size = 9;
			fontBold.Bold = true;
			Infragistics.Documents.Word.Font fontBoldLarge = docWriter.CreateFont();
			fontBoldLarge.Name = "Arial";
			fontBoldLarge.Size = 11;
			fontBoldLarge.Bold = true;
			Infragistics.Documents.Word.Font fontSmall = docWriter.CreateFont();
			fontSmall.Name = "Arial";
			fontSmall.Size = 8;
			fontSmall.Bold = false;

			docWriter.StartParagraph();
			docWriter.AddTextRun("Intervention Report", fontHeading);
			docWriter.EndParagraph();

			docWriter.StartParagraph();
			docWriter.AddTextRun("", fontHeading);
			docWriter.EndParagraph();

			SectionHeaderFooterParts parts = SectionHeaderFooterParts.HeaderAllPages | SectionHeaderFooterParts.FooterAllPages;
			SectionHeaderFooterWriterSet writerSet = docWriter.AddSectionHeaderFooter(parts);

			var ReportInterventionOverview = METTLib.Questionnaire.QuestionnaireAnswerSetList.GetQuestionnaireAnswerSetList(QuestionnaireAnswerSetId);
			foreach (var item in ReportInterventionOverview)
			{
				writerSet.HeaderWriterAllPages.Open();
				writerSet.HeaderWriterAllPages.StartParagraph();
				writerSet.HeaderWriterAllPages.AddTextRun("Management Effectiveness Tracking Tool-South Africa (METT) - Intervention Report [" + item.METTReportingName + "]", fontSmall);
				writerSet.HeaderWriterAllPages.EndParagraph();
				writerSet.HeaderWriterAllPages.Close();
				writerSet.FooterWriterAllPages.Open();
				writerSet.FooterWriterAllPages.StartParagraph();
				writerSet.FooterWriterAllPages.AddTextRun("[This report was generated on " + DateTime.Now.ToString("dd/MM/yyyy") + "]", fontSmall);
				writerSet.FooterWriterAllPages.EndParagraph();
				writerSet.FooterWriterAllPages.Close();
				docWriter.StartParagraph();
				docWriter.AddTextRun(item.METTReportingName, fontSubHeading);
				docWriter.EndParagraph();
				docWriter.StartParagraph();
				docWriter.AddTextRun(item.OfficialName, fontBoldLarge);
				docWriter.EndParagraph();
				docWriter.StartParagraph();
				docWriter.AddTextRun("Legal Designation " + item.LegalDesignation, fontBoldLarge);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("Biomes ", fontBold);
				var ROQuestionnaireAnswerSetNationalBiomeList = METTLib.Questionnaire.RO.ROQuestionnaireAnswerSetNationalBiomeList.GetROQuestionnaireAnswerSetNationalBiomeList(QuestionnaireAnswerSetId);
				foreach (var itemBiomes in ROQuestionnaireAnswerSetNationalBiomeList)
				{
					docWriter.AddTextRun(itemBiomes.NationalBiome + ", ", fontBold);
				}
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("(Created on " + item.CreatedDateTime + " and last Modified on " + item.ModifiedDateTime + ")", fontSmall);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				// Executive Summary
				docWriter.StartParagraph();
				docWriter.AddTextRun("Executive Summary", fontSubHeading);
				docWriter.EndParagraph();
				docWriter.StartParagraph();
				docWriter.AddTextRun("[Please add your own details regarding the overall executive summary regarding the assessment of the protected area / site. This should explain the context of the protected area/site of its current state of management effectiveness.]", fontNormal);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				TableBorderProperties borderProps1 = docWriter.CreateTableBorderProperties();
				borderProps1.Color = Color.Black;
				borderProps1.Style = TableBorderStyle.Double;
				TableProperties tableProps1 = docWriter.CreateTableProperties();
				tableProps1.Layout = TableLayout.Fixed;
				tableProps1.PreferredWidthAsPercentage = 100;
				tableProps1.Alignment = ParagraphAlignment.Center;
				tableProps1.BorderProperties.Color = borderProps1.Color;
				tableProps1.BorderProperties.Style = borderProps1.Style;
				TableRowProperties rowProps1 = docWriter.CreateTableRowProperties();
				rowProps1.IsHeaderRow = true;
				TableCellProperties cellProps1 = docWriter.CreateTableCellProperties();


				cellProps1.BackColor = Color.DarkGray;
				cellProps1.TextDirection = TableCellTextDirection.LeftToRightTopToBottom;

				docWriter.StartTable(1, tableProps1);
				docWriter.StartTableRow(rowProps1);
				docWriter.StartTableCell(cellProps1);

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				var ManagementRating = METTLib.RO.ROQuestionnaireAnswerScoreAssessorList.GetROQuestionnaireAnswerScoreAssessorList(QuestionnaireAnswerSetId);
				foreach (var itemMR in ManagementRating)
				{

					// Total for precentages
					var ROManagementSphereListTotal = METTLib.Reports.ROManagementSphereList.GetROManagementSphereList();

					int dSphereTotalValue = 0;
					int dSphereTotalRating = 0;
					decimal dSphereTotalPerc = 0;

					int dSphereGrandTotalValue = 0;
					int dSphereGrandTotalRating = 0;
					decimal dSphereGrandTotalPerc = 0;

					foreach (var sphereitemTotal in ROManagementSphereListTotal)
					{
						var ReportInterventionManagementSphereListTotal = METTLib.Reports.ReportInterventionManagementSphereList.GetReportInterventionManagementSphereList(QuestionnaireAnswerSetId, sphereitemTotal.ManagementSphereID);
						foreach (var itemTotal in ReportInterventionManagementSphereListTotal)
						{
							dSphereTotalValue += itemTotal.MaxValue;
							dSphereTotalRating += itemTotal.AnswerRating;
						}
						dSphereGrandTotalValue += dSphereTotalValue;
						dSphereGrandTotalRating += dSphereTotalRating;

						dSphereTotalValue = 0;
						dSphereTotalRating = 0;
					}
					// Total for precentages

					var AssessorScore = METTLib.RO.ROQuestionnaireAnswerScoreAssessorList.GetROQuestionnaireAnswerScoreAssessorList(QuestionnaireAnswerSetId);

					dSphereGrandTotalPerc = Math.Round((Convert.ToDecimal(dSphereGrandTotalRating) / Convert.ToDecimal(dSphereGrandTotalValue)) * 100, 2);

					foreach (var ScoreItem in AssessorScore)
					{
						docWriter.StartParagraph();
						docWriter.AddTextRun("Overall Management Effectiveness Rating: " + ScoreItem.TotalIndexAssessorPct.ToString("#") + "%", fontBoldLarge);
						docWriter.EndParagraph();
					}

				}

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.EndTableCell();
				docWriter.EndTableRow();
				docWriter.EndTable();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("Priority Interventions", fontSubHeading);
				docWriter.EndParagraph();
				docWriter.StartParagraph();
				docWriter.AddTextRun("[Please add details regarding the priority interventions to be focused on over the upcoming period to improve management effectiveness of the protected area/site. This should detail who will undertake these actions and a delivery timeframe.]", fontNormal);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				TableBorderProperties borderProps0 = docWriter.CreateTableBorderProperties();
				borderProps0.Color = Color.Black;
				borderProps0.Style = TableBorderStyle.Double;
				TableProperties tableProps0 = docWriter.CreateTableProperties();
				tableProps0.Layout = TableLayout.Fixed;
				tableProps0.PreferredWidthAsPercentage = 100;
				tableProps0.Alignment = ParagraphAlignment.Center;
				tableProps0.BorderProperties.Color = borderProps0.Color;
				tableProps0.BorderProperties.Style = borderProps0.Style;
				TableRowProperties rowProps0 = docWriter.CreateTableRowProperties();
				rowProps0.IsHeaderRow = true;
				TableCellProperties cellProps0 = docWriter.CreateTableCellProperties();

				cellProps0.BackColor = Color.DarkGray;
				cellProps0.TextDirection = TableCellTextDirection.LeftToRightTopToBottom;

				docWriter.StartTable(3, tableProps0);
				docWriter.StartTableRow(rowProps0);

				cellProps0.PreferredWidthAsPercentage = 60;
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Intervention Required");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				cellProps0.PreferredWidthAsPercentage = 15;
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Who");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				cellProps0.PreferredWidthAsPercentage = 15;
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Timeframe");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();

				cellProps0.Reset();
				cellProps0.BackColor = Color.White;

				docWriter.StartTableRow();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();

				docWriter.StartTableRow();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();

				docWriter.StartTableRow();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();

				docWriter.StartTableRow();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();

				docWriter.StartTableRow();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.StartTableCell(cellProps0);
				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				docWriter.EndTableRow();
				docWriter.EndTable();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				// Management Sphere Summary Section
				docWriter.StartParagraph();
				docWriter.AddTextRun("Management Sphere Summary", fontSubHeading);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();
			}
			TableBorderProperties borderProps = docWriter.CreateTableBorderProperties();
			borderProps.Color = Color.Black;
			borderProps.Style = TableBorderStyle.Double;
			TableProperties tableProps = docWriter.CreateTableProperties();
			tableProps.Layout = TableLayout.Fixed;
			tableProps.PreferredWidthAsPercentage = 100;
			tableProps.Alignment = ParagraphAlignment.Center;
			tableProps.BorderProperties.Color = borderProps.Color;
			tableProps.BorderProperties.Style = borderProps.Style;
			TableRowProperties rowProps = docWriter.CreateTableRowProperties();
			rowProps.IsHeaderRow = true;
			TableCellProperties cellProps = docWriter.CreateTableCellProperties();






			var ROManagementSphereList = METTLib.Reports.ROManagementSphereList.GetROManagementSphereList();

			foreach (var sphereitem in ROManagementSphereList)
			{
				cellProps.BackColor = Color.DarkGray;
				cellProps.TextDirection = TableCellTextDirection.LeftToRightTopToBottom;

				docWriter.StartTable(1, tableProps);
				docWriter.StartTableRow(rowProps);
				docWriter.StartTableCell(cellProps);

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun(sphereitem.ManagementSphereID + ". " + sphereitem.ManagementSphere, fontBoldLarge);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun(sphereitem.ManagementSphereContent, fontNormal);
				docWriter.EndParagraph();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.EndTableCell();
				docWriter.EndTableRow();
				docWriter.EndTable();

				docWriter.StartParagraph();
				docWriter.AddTextRun("");
				docWriter.EndParagraph();

				docWriter.StartTable(3, tableProps);
				docWriter.StartTableRow(rowProps);

				cellProps.PreferredWidthAsPercentage = 30;
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Indicators");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				cellProps.PreferredWidthAsPercentage = 20;
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Comments");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				cellProps.PreferredWidthAsPercentage = 20;
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Evidence");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				cellProps.PreferredWidthAsPercentage = 20;
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Next Steps");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				cellProps.PreferredWidthAsPercentage = 10;
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Rating (as %)");
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();

				cellProps.Reset();
				cellProps.BackColor = Color.White;

				docWriter.EndTable();

				docWriter.StartTable(3, tableProps);

				int dTotalValue = 0;
				int dTotalRating = 0;
				decimal dTotalPerc = 0;

				var ReportInterventionManagementSphereList = METTLib.Reports.ReportInterventionManagementSphereList.GetReportInterventionManagementSphereList(QuestionnaireAnswerSetId, sphereitem.ManagementSphereID);
				foreach (var item in ReportInterventionManagementSphereList)
				{

					if (item.AnswerRating != -1)
					{
						dTotalValue = dTotalValue + item.MaxValue;
						dTotalRating = dTotalRating + item.AnswerRating;
					}

					docWriter.StartTableRow();
					docWriter.StartTableCell(cellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(item.IndicatorDetailName, fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					docWriter.StartTableCell(cellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(item.NextSteps, fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					docWriter.StartTableCell(cellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(item.Comments, fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();

					docWriter.StartTableCell(cellProps);
					docWriter.StartParagraph();
					docWriter.AddTextRun(Convert.ToString(item.Evidence), fontNormal);
					docWriter.EndParagraph();
					docWriter.EndTableCell();


					if (item.AnswerRating == 0)
					{
						cellProps.BackColor = Color.Red;
					}
					if (item.AnswerRating == 1)
					{
						cellProps.BackColor = Color.Yellow;
					}
					if (item.AnswerRating == 2)
					{
						cellProps.BackColor = Color.Yellow;
					}
					if (item.AnswerRating == 3)
					{
						cellProps.BackColor = Color.LightGreen;
					}

					docWriter.StartTableCell(cellProps);
					docWriter.StartParagraph();

					if (item.AnswerRating != -1)
					{
						docWriter.AddTextRun(Convert.ToString(item.AnswerRating), fontNormal);
					}
					else
					{
						docWriter.AddTextRun(Convert.ToString("n/a"), fontNormal);
					}

					docWriter.EndParagraph();
					docWriter.EndTableCell();
					cellProps.Reset();
					cellProps.BackColor = Color.White;

					docWriter.EndTableRow();



				}
				docWriter.StartTableRow();

				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("", fontBold);
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("", fontBold);
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("", fontBold);
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun("Total");
				docWriter.EndParagraph();
				docWriter.EndTableCell();

				if (dTotalRating > 0)
				{
					if (dTotalValue > 0)
					{
						//dTotalPerc = Math.Round((Convert.ToDecimal(dTotalRating) / Convert.ToDecimal(dTotalValue)) * 100, 2);
						dTotalPerc = Math.Round((Convert.ToDecimal(dTotalRating) / Convert.ToDecimal(dTotalValue)) * 100, 2);

					}
					else
					{
						dTotalPerc = 0;
					}
				}
				else
				{
					dTotalPerc = 0;
				}
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun(Convert.ToString(dTotalPerc), fontBold);
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();
				docWriter.EndTable();

				docWriter.StartParagraph();
				docWriter.EndParagraph();


				TableBorderProperties borderProps110 = docWriter.CreateTableBorderProperties();
				borderProps110.Color = Color.Black;
				borderProps110.Style = TableBorderStyle.Double;
				TableProperties tableProps110 = docWriter.CreateTableProperties();
				tableProps110.Layout = TableLayout.Fixed;
				tableProps110.PreferredWidthAsPercentage = 100;
				tableProps110.Alignment = ParagraphAlignment.Center;
				tableProps110.BorderProperties.Color = borderProps110.Color;
				tableProps110.BorderProperties.Style = borderProps110.Style;
				TableRowProperties rowProps110 = docWriter.CreateTableRowProperties();
				rowProps110.IsHeaderRow = true;
				TableCellProperties cellProps110 = docWriter.CreateTableCellProperties();


				cellProps110.BackColor = Color.DarkGray;
				cellProps110.TextDirection = TableCellTextDirection.LeftToRightTopToBottom;

				docWriter.StartTable(3, tableProps110);

				docWriter.StartTableRow();
				docWriter.StartTableCell(cellProps);
				docWriter.StartParagraph();
				docWriter.AddTextRun($"{sphereitem.ManagementSphere} Summary:", fontBold);
				docWriter.EndParagraph();
				docWriter.StartParagraph();
				docWriter.AddTextRun("[Here you can summarise your contextual comments regarding the above management sphere ratings if appropriate.]", fontNormal);
				docWriter.EndParagraph();
				docWriter.EndTableCell();
				docWriter.EndTableRow();

				docWriter.EndTable();

				docWriter.StartParagraph();
				docWriter.AddTextRun("", fontHeading);
				docWriter.EndParagraph();
				docWriter.StartParagraph();
				docWriter.AddTextRun("", fontHeading);
				docWriter.EndParagraph();
			}

			//docWriter.StartParagraph();
			//docWriter.AddTextRun("Indicator Details per Management Sphere", fontSubHeading);
			//docWriter.EndParagraph();
			//docWriter.StartParagraph();
			//docWriter.AddTextRun("[Please add details regarding the priority interventions to be focused on over the upcoming period]", fontNormal);
			//docWriter.EndParagraph();

			//docWriter.StartParagraph();
			//docWriter.AddTextRun("", fontHeading);
			//docWriter.EndParagraph();

			//foreach (var sphereitem in ROManagementSphereList)
			//{
			//	cellProps.BackColor = Color.DarkGray;
			//	cellProps.TextDirection = TableCellTextDirection.LeftToRightTopToBottom;

			//	docWriter.StartTable(1, tableProps);
			//	docWriter.StartTableRow(rowProps);
			//	docWriter.StartTableCell(cellProps);

			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun("");
			//	docWriter.EndParagraph();

			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun(sphereitem.ManagementSphereID + ". " + sphereitem.ManagementSphere, fontBoldLarge);
			//	docWriter.EndParagraph();

			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun(sphereitem.ManagementSphereContent, fontNormal);
			//	docWriter.EndParagraph();

			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun("");
			//	docWriter.EndParagraph();

			//	docWriter.EndTableCell();
			//	docWriter.EndTableRow();
			//	docWriter.EndTable();

			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun("");
			//	docWriter.EndParagraph();

			//	docWriter.StartTable(3, tableProps);
			//	docWriter.StartTableRow(rowProps);

			//	cellProps.PreferredWidthAsPercentage = 40;
			//	docWriter.StartTableCell(cellProps);
			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun("Indicators");
			//	docWriter.EndParagraph();
			//	docWriter.EndTableCell();

			//	cellProps.PreferredWidthAsPercentage = 20;
			//	docWriter.StartTableCell(cellProps);
			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun("Evidence");
			//	docWriter.EndParagraph();
			//	docWriter.EndTableCell();

			//	cellProps.PreferredWidthAsPercentage = 20;
			//	docWriter.StartTableCell(cellProps);
			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun("Next Steps");
			//	docWriter.EndParagraph();
			//	docWriter.EndTableCell();

			//	cellProps.PreferredWidthAsPercentage = 20;
			//	docWriter.StartTableCell(cellProps);
			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun("Comments");
			//	docWriter.EndParagraph();
			//	docWriter.EndTableCell();
			//	docWriter.EndTableRow();

			//	var ReportInterventionIndicatorDetailsManagementSphereList = METTLib.Reports.ReportInterventionIndicatorDetailsManagementSphereList.GetReportInterventionIndicatorDetailsManagementSphereList(QuestionnaireAnswerSetId, sphereitem.ManagementSphereID);
			//	foreach (var item in ReportInterventionIndicatorDetailsManagementSphereList)
			//	{
			//		cellProps.BackColor = Color.White;
			//		docWriter.StartTableRow();
			//		docWriter.StartTableCell(cellProps);
			//		docWriter.StartParagraph();
			//		docWriter.AddTextRun(item.IndicatorDetailName, fontNormal);
			//		docWriter.EndParagraph();
			//		docWriter.EndTableCell();

			//		docWriter.StartTableCell(cellProps);
			//		docWriter.StartParagraph();
			//		docWriter.AddTextRun(Convert.ToString(item.Evidence), fontNormal);
			//		docWriter.EndParagraph();
			//		docWriter.EndTableCell();

			//		docWriter.StartTableCell(cellProps);
			//		docWriter.StartParagraph();
			//		docWriter.AddTextRun(Convert.ToString(item.Comments), fontNormal);
			//		docWriter.EndParagraph();
			//		docWriter.EndTableCell();

			//		docWriter.StartTableCell(cellProps);
			//		docWriter.StartParagraph();
			//		docWriter.AddTextRun(Convert.ToString(item.NextSteps), fontNormal);
			//		docWriter.EndParagraph();
			//		docWriter.EndTableCell();




			//		cellProps.Reset();
			//		cellProps.BackColor = Color.White;
			//		docWriter.EndTableRow();
			//	}
			//	cellProps.Reset();
			//	cellProps.BackColor = Color.White;
			//	docWriter.EndTable();
			//	docWriter.StartParagraph();
			//	docWriter.AddTextRun("", fontHeading);
			//	docWriter.EndParagraph();
			//}



			//TableBorderProperties borderProps19 = docWriter.CreateTableBorderProperties();
			//borderProps19.Color = Color.Black;
			//borderProps19.Style = TableBorderStyle.Double;
			//TableProperties tableProps19 = docWriter.CreateTableProperties();
			//tableProps19.Layout = TableLayout.Fixed;
			//tableProps19.PreferredWidthAsPercentage = 100;
			//tableProps19.Alignment = ParagraphAlignment.Center;
			//tableProps19.BorderProperties.Color = borderProps19.Color;
			//tableProps19.BorderProperties.Style = borderProps19.Style;
			//TableRowProperties rowProps19 = docWriter.CreateTableRowProperties();
			//rowProps19.IsHeaderRow = true;
			//TableCellProperties cellProps19 = docWriter.CreateTableCellProperties();


			//cellProps19.BackColor = Color.DarkGray;
			//cellProps19.TextDirection = TableCellTextDirection.LeftToRightTopToBottom;

			//docWriter.StartTable(1, tableProps19);
			//docWriter.StartTableRow(rowProps19);
			//docWriter.StartTableCell(cellProps19);

			//docWriter.StartParagraph();
			//docWriter.AddTextRun("");
			//docWriter.EndParagraph();


			//docWriter.EndTableCell();
			//docWriter.EndTableRow();

			//docWriter.EndTable();


			docWriter.StartParagraph();
			docWriter.AddTextRun("", fontHeading);
			docWriter.EndParagraph();


			docWriter.StartParagraph();
			docWriter.AddTextRun("");
			docWriter.EndParagraph();
			docWriter.EndDocument();
			docWriter.Close();
			m.Position = 0;
			return m;
		}

		[WebCallable]
		public static Result DeleteAssessment(int QuestionnaireAnswerSetID)
		{
			Result results = new Singular.Web.Result();

			try
			{
				QuestionnaireAnswerSetList QuestionnaireAnswerSets = QuestionnaireAnswerSetList.GetQuestionnaireAnswerSetList(QuestionnaireAnswerSetID);

				if (QuestionnaireAnswerSets != null && QuestionnaireAnswerSets.Count > 0)
				{
					QuestionnaireAnswerSets.ToList().ForEach(c => { c.DeletedInd = true; });
					QuestionnaireAnswerSets.Save();

					results.Success = true;
				}
				else
				{
					results.Success = false;
					results.ErrorText = $"No assessment with QuestionnaireAnswerSetID of {QuestionnaireAnswerSetID} was found in the database.";
				}

			}
			catch (Exception ex)
			{
				results.Success = false;
				results.ErrorText = ex.Message;
			}

			return results;
		}

		[Singular.Web.WebCallable(LoggedInOnly = true)]
		public string ViewOrganisation(int OrganisationID, int QuestionnaireAnswerSetID)
		{
			var url = $"../Organisation/OrganisationProfile.aspx?OrganisationID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(OrganisationID.ToString()))}&QuestionnaireAnswerSetID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(QuestionnaireAnswerSetID.ToString()))}";
			return url;
		}

		[Singular.Web.WebCallable(LoggedInOnly = true)]
		public string ViewProtectedArea(int ProtectedAreaID, int QuestionnaireAnswerSetID)
		{
			var url = "";
			if (QuestionnaireAnswerSetID > 0)
			{
				//protected area hyperlink click
				url = $"../ProtectedArea/ProtectedAreaProfile.aspx?ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedAreaID.ToString()))}&QuestionnaireAnswerSetID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(QuestionnaireAnswerSetID.ToString()))}";
			}
			else
			{
				//back button click
				url = $"../ProtectedArea/ProtectedAreaProfile.aspx?ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedAreaID.ToString()))}";
			}

			return url;
		}

		public static Singular.Web.Result GenerateAuditReport(int QuestionnaireAnswerSetID)
		{
			Result sr = new Result();
			try
			{
				var fileTimeStamp = DateTime.Now.ToString("ddMMyy-hhmmss");
				var ProtectedAreaInformation = METTLib.Questionnaire.QuestionnaireAnswerSetList.GetQuestionnaireAnswerSetList(QuestionnaireAnswerSetID)?.FirstOrDefault();

				string ProtectedAreaName = ProtectedAreaInformation?.METTReportingName ?? string.Empty;
				string ReportFileName = $"METT-AuditResultsFor-{ProtectedAreaName}-{fileTimeStamp}.xls";

				Singular.Documents.TemporaryDocument tempDoc = new Singular.Documents.TemporaryDocument();
				tempDoc.SetDocument(CreateExcel(QuestionnaireAnswerSetID).ToArray(), ReportFileName);

				sr.Success = true;
				return new Singular.Web.Result(true) { Data = Singular.Web.WebServices.FileDownloadHandler.SaveTempFile(tempDoc) };
			}
			catch (Exception e)
			{
				sr.Data = e.InnerException;
				sr.Success = false;
			}
			return sr;
		}

		[WebCallable]
		private static MemoryStream CreateExcel(int QuestionnaireAnswerSetID)
		{
			Singular.Data.ExcelExporter ExcelDoc = new Singular.Data.ExcelExporter(Infragistics.Documents.Excel.WorkbookFormat.Excel2007)
			{
				FormatAsTable = false,
				ShowTotalsRow = false,
			};

			var AuditResultsSheet = ExcelDoc.WorkBook.Worksheets.Add("Audit Results");
			METTLib.Reports.ROQuestionnaireAnswerSetAuditResultList AuditReportDataSource = METTLib.Reports.ROQuestionnaireAnswerSetAuditResultList.GetROQuestionnaireAnswerSetAuditResultList(QuestionnaireAnswerSetID);

			ExcelDoc.PopulateData(AuditReportDataSource, AuditResultsSheet, false, false, 0, false, false, true);

			//header settings
			AuditResultsSheet.Rows[0].CellFormat.Fill = Infragistics.Documents.Excel.CellFill.NoColor;
			AuditResultsSheet.Rows[0].CellFormat.Alignment = Infragistics.Documents.Excel.HorizontalCellAlignment.Center;
			AuditResultsSheet.Rows[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
			AuditResultsSheet.Rows[0].CellFormat.Font.ColorInfo = Infragistics.Documents.Excel.WorkbookColorInfo.Automatic;
			AuditResultsSheet.Rows[0].CellFormat.TopBorderStyle = Infragistics.Documents.Excel.CellBorderLineStyle.Thick;
			
			//set column widths
			AuditResultsSheet.Columns[0].Width = 12700;
		
			//make the comments columns to be the same width and wrap text
			DataTable dt = AuditReportDataSource.GetDataset(false)?.Tables[0];

			if (dt != null)
			{
				var columnCounter = dt.Columns.Count;
				for (int columnIndex = 0; columnIndex < columnCounter; columnIndex++)
				{
					if(dt.Columns[columnIndex].ColumnName.ToLower().Contains("comments") || dt.Columns[columnIndex].ColumnName.ToLower().Contains("evidence") || dt.Columns[columnIndex].ColumnName.ToLower().Contains("steps"))
					{
						AuditResultsSheet.Columns[columnIndex - 1].Width = 10500;
						AuditResultsSheet.Columns[columnIndex - 1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
					}
					//format the Value, Original Rating and Audit Rating columns
					if (dt.Columns[columnIndex].ColumnName.ToLower().Contains("rating") || dt.Columns[columnIndex].ColumnName.ToLower().Contains("value"))
					{
						AuditResultsSheet.Columns[columnIndex - 1].Width = 3000;
						AuditResultsSheet.Columns[columnIndex - 1].CellFormat.Alignment = Infragistics.Documents.Excel.HorizontalCellAlignment.Center;
						AuditResultsSheet.Columns[columnIndex - 1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;		
					}
				}
			}
			
			//need to find a way to make the Total rows to be bold and draw a line below
			var i = 0;
			bool firstBorderLineDrawn = false;
			foreach (var item in AuditReportDataSource)
			{
				if (item.IndicatorDetailName == "Total")
				{	
					AuditResultsSheet.Rows[i + 1].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
					
					for (int iterator = 0; iterator < dt.Columns.Count - 1; iterator++)
					{
						AuditResultsSheet.Rows[i + 1].Cells[iterator].CellFormat.BottomBorderStyle = Infragistics.Documents.Excel.CellBorderLineStyle.Thick;
						//puting this here so that the border under the headings is only drawn until the last column and not beyond
						if (firstBorderLineDrawn == false)
						{
							AuditResultsSheet.Rows[0].Cells[iterator].CellFormat.BottomBorderStyle = Infragistics.Documents.Excel.CellBorderLineStyle.Thick;		
						}
					}
					firstBorderLineDrawn = true;
				}
				//draw right border on the last column
				AuditResultsSheet.Rows[i].Cells[dt.Columns.Count - 2].CellFormat.RightBorderStyle = Infragistics.Documents.Excel.CellBorderLineStyle.Thick;
				
				i++;
			}
			//draw the border for the last row as well
			AuditResultsSheet.Rows[i].Cells[dt.Columns.Count - 2].CellFormat.RightBorderStyle = Infragistics.Documents.Excel.CellBorderLineStyle.Thick;

			// Save excel file
			MemoryStream m = new MemoryStream();
			ExcelDoc.WorkBook.Save(m);
			return m;
		}

	}
}

