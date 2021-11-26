using METTLib.Organisation;
using METTLib.ProtectedArea;
using METTLib.RO;
using METTLib.Security;
using Singular;
using Singular.Localisation;
using Singular.Security;
using Singular.Web;
using Singular.Web.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace METTWeb.ProtectedArea
{
  public partial class ProtectedAreaProfile : METTPageBase<ProtectedAreaProfileVM>
  {

  }

  public class ProtectedAreaProfileVM : METTStatelessViewModel<ProtectedAreaProfileVM>
  {
    public PagedDataManager<ProtectedAreaProfileVM> ROProtectedAreaPagedListManager { get; set; }
    public ROProtectedAreaPagedList ROProtectedAreaPagedList { get; set; }
    public ROProtectedAreaPagedList.Criteria ROProtectedAreaPagedListCriteria { get; set; }
    public ROOrganisationProtectedAreaUserList ROOrganisationProtectedAreaUserList { get; set; }

    public ROProtectedAreaAssessmentPagedList ROProtectedAreaAssessmentPagedList { get; set; }
    public ROProtectedAreaAssessmentPagedList.Criteria ROProtectedAreaAssessmentPagedListCriteria { get; set; }
    public PagedDataManager<ProtectedAreaProfileVM> ROProtectedAreaAssessmentPagedListManager { get; set; }

    public METTLib.ProtectedArea.ProtectedArea EditingProtectedArea { get; set; }
    public METTLib.ProtectedArea.ProtectedAreaList ProtectedAreas { get; set; }
    public METTLib.Security.User EditingUser { get; set; }
    public bool IsViewingProtectedAreaInd { get; set; }

    public int OrganisationProtectedAreaID { get; set; }

    public METTLib.RO.RONationalbiomeList NationalBiomes { get; set; }
    public ProtectedAreaNationalBiomeList ProtectedAreaNationalBiomes { get; set; }

    public METTLib.Questionnaire.ROQuestionnaireGroupList ROQuestionnaireGroupList { get; set; }
    public METTLib.ThreatCategories.ThreatCategoryList ThreatsList { get; set; }
    public METTLib.Questionnaire.QuestionnaireAnswerExportSetList QuestionnaireAnswerExportSetList { get; set; }

    public bool PageViewInd { get; set; }
    public bool PageNewProtectedAreaSiteBtnInd { get; set; }
    public bool PageViewProtectedAreaSiteBtnInd { get; set; }
    public bool PageSaveProtectedAreaSiteBtnInd { get; set; }

    public bool PageAddNewUserProtectedAreaSiteBtnInd { get; set; }
    public bool PageAddExistingUserProtectedAreaSiteBtnInd { get; set; }
    public bool PageRemoveUserProtectedAreaSiteBtnInd { get; set; }
    public bool PageViewUserProtectedAreaSiteBtnInd { get; set; }

    public bool PageNewAssessmentProtectedAreaSiteBtnInd { get; set; }
    public bool PageImportAssessmentProtectedAreaSiteBtnInd { get; set; }

    public bool PageExportAssessmentProtectedAreaSiteBtnInd { get; set; }
    public bool PageViewAssessmentProtectedAreaSiteBtnInd { get; set; }

    public bool PageViewBiodiversityProtectedAreaSiteBtnInd { get; set; }
    public bool PageRemoveBiodiversityProtectedAreaSiteBtnInd { get; set; }
    public bool PageNewBiodiversityProtectedAreaSiteBtnInd { get; set; }
    public bool PageSaveBiodiversityProtectedAreaSiteBtnInd { get; set; }

    public bool PageViewObjectivesProtectedAreaSiteBtnInd { get; set; }
    public bool PageRemoveObjectivesProtectedAreaSiteBtnInd { get; set; }
    public bool PageNewObjectivesProtectedAreaSiteBtnInd { get; set; }
    public bool PageSaveObjectivesProtectedAreaSiteBtnInd { get; set; }

    public bool PageViewNationalBiomesProtectedAreaSiteBtnInd { get; set; }
    public bool PageRemoveNationalBiomesProtectedAreaSiteBtnInd { get; set; }
    public bool PageNewNationalBiomesProtectedAreaSiteBtnInd { get; set; }

    public METTLib.RO.ROOrganisationRegionList ROOrganisationRegions { get; set; }
    public METTLib.Organisation.ROOrganisationList ROOrganisationList { get; set; }
    public SecurityGroupProtectedAreaUserList SecurityGroupProtectedAreaUserList { get; set; }
    public OrganisationProtectedAreaUser OrganisationProtectedAreaUser { get; set; }
    public string CurrentYear { get => DateTime.Now.Year.ToString(); }
    public string PreviousYear { get => (DateTime.Now.Year - 1).ToString(); } 

    enum SecurityRoles
    {
      Create,
      Access,
      View
    }

    public enum ExcelColumns
    {
      OrganisationId = 0, // Hidden column
      ProtectedAreaId = 1, // Hidden column
      LegalDesignationId = 2, // Hidden column
      QuestionnaireId = 3, // Hidden column
      QuestionnaireGroupId = 4, // Hidden column
      No = 5,
      Indicator = 6,
      QuestionId = 7,
      Question = 8,
      QuestionnaireQuestionTypeIdProperty = 9, // Hidden column
      SortOrder = 10, // Hidden column
      QuestionnaireAnswerOptionIdCombo = 11,  // Hidden column
      AnswerOption1 = 12,
      AnswerOption2 = 13,
      AnswerOption3 = 14,
      AnswerOption4 = 15,
      AnswerOption5 = 16,
      YourAnswerOption = 17,
      Comments = 18,
      NextSteps = 19,
      Evidence = 20
    }

    public enum ExcelColumnSizes
    {
      Hidden = 0,
      Indicator = 12500,
      Question = 15000,
      AnswerOption = 10500,
      Comments = 10500,
      NextSteps = 10500,
      Evidence = 10500
    }


    // Security Group Roles Check
    public ROSecurityOrganisationProtectedAreaGroupUserList ROSecurityOrganisationProtectedAreaGroupUserList { get; set; }

    public string ActiveTab { get; set; }

    /// <summary>
    /// User List Page Manager 
    /// </summary>
    public PagedDataManager<ProtectedAreaProfileVM> UserListPageManager { get; set; }

    /// <summary>
    /// User Criteria 
    /// </summary>
    public ROUserPagedList.Criteria UserCriteria { get; set; }

    /// <summary>
    /// User List
    /// </summary>
    public ROUserPagedList UserList { get; set; }

    public ProtectedAreaProfileVM()
    {
      this.UserListPageManager = new PagedDataManager<ProtectedAreaProfileVM>((c) => c.UserList, (c) => c.UserCriteria, "UserName", 10);
      this.UserCriteria = new ROUserPagedList.Criteria();
      this.UserList = new ROUserPagedList();
    }
    protected override void Setup()
    {
      base.Setup();

      // Security based on multiple groups and roles for a selected user
      ROSecurityOrganisationProtectedAreaGroupUserList = METTLib.RO.ROSecurityOrganisationProtectedAreaGroupUserList.GetROSecurityOrganisationProtectedAreaGroupUserList(Singular.Security.Security.CurrentIdentity.UserID, null, null);

      PageViewInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageNewProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites.Create") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageViewProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites.View") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageSaveProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites.Save") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageAddNewUserProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Users.Create") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageAddExistingUserProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Users.Link") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageRemoveUserProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Users.Remove") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageViewUserProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Users.Access") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());

      PageNewAssessmentProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Assessments.New Assessment") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.View.ToString() || c.SecurityRole == SecurityRoles.View.ToString());

      PageImportAssessmentProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Assessments.Import Template") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());

      PageExportAssessmentProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Assessments.Export Template") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageViewAssessmentProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Assessments.View") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());

      PageRemoveBiodiversityProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Biodiversity.Remove") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageNewBiodiversityProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Biodiversity.Create") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageSaveBiodiversityProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites Biodiversity.Save") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());

      PageRemoveObjectivesProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites High Level Site Objectives.Remove") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageNewObjectivesProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites High Level Site Objectives.Create") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageSaveObjectivesProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites High Level Site Objectives.Save") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());

      PageRemoveNationalBiomesProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites National Biomes.Remove") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());
      PageNewNationalBiomesProtectedAreaSiteBtnInd = Singular.Settings.CurrentUser.Roles.Contains("Protected Areas / Sites National Biomes.Create") && ROSecurityOrganisationProtectedAreaGroupUserList.Any((c) => c.SecurityRole == SecurityRoles.Access.ToString() || c.SecurityRole == SecurityRoles.View.ToString());

      // Protected Area Paged Grid
      ROProtectedAreaPagedListManager = new PagedDataManager<ProtectedAreaProfileVM>((c) => c.ROProtectedAreaPagedList, (c) => c.ROProtectedAreaPagedListCriteria, "ProtectedAreaName", 20, true);
      ROProtectedAreaPagedListCriteria = new ROProtectedAreaPagedList.Criteria();
      ROProtectedAreaPagedList = (ROProtectedAreaPagedList)this.ROProtectedAreaPagedListManager.GetInitialData();

      ROProtectedAreaAssessmentPagedListManager = new PagedDataManager<ProtectedAreaProfileVM>((d) => d.ROProtectedAreaAssessmentPagedList, (d) => d.ROProtectedAreaAssessmentPagedListCriteria, "METTReportingName", 10, true);
      ROProtectedAreaAssessmentPagedListCriteria = new ROProtectedAreaAssessmentPagedList.Criteria();
      ROProtectedAreaAssessmentPagedList = (ROProtectedAreaAssessmentPagedList)this.ROProtectedAreaAssessmentPagedListManager.GetInitialData();

      NationalBiomes = METTLib.RO.RONationalbiomeList.GetRONationalbiomeList();

      this.IsViewingProtectedAreaInd = false;

      this.UserList = (ROUserPagedList)UserListPageManager.GetInitialData();

      ActiveTab = "Users";

      ROQuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();
      QuestionnaireAnswerExportSetList = METTLib.Questionnaire.QuestionnaireAnswerExportSetList.GetQuestionnaireAnswerExportSetList();
      ThreatsList = METTLib.ThreatCategories.ThreatCategoryList.GetThreatCategoryList();

      ROOrganisationList = ROOrganisationList.GetROOrganisationList();
      ROOrganisationRegions = METTLib.RO.ROOrganisationRegionList.GetROOrganisationRegionList();

      // SecurityGroupProtectedAreaUserList = SecurityGroupProtectedAreaUserList.GetSecurityGroupProtectedAreaUserList();
      SecurityGroupProtectedAreaUserList = SecurityGroupProtectedAreaUserList.NewSecurityGroupProtectedAreaUserList();
    }


    [Singular.Web.WebCallable(LoggedInOnly = true)]
    public string EditProtectedAreaUser(int UserID, int ProtectedAreaID)
    {
      var url = $"../Users/UserProfile.aspx?OrganisationProtectedAreaUserID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(UserID.ToString()))}&ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedAreaID.ToString()))}";
      return url;
    }

    /// <summary>
    /// Get the user with the given Id
    /// </summary>
    /// <param name="userId">The User Id</param>
    /// <returns>A User instance</returns>
    [WebCallable]
    public Singular.Web.Result GetUser(int UserID, int OrganisationProtectedAreaID, int ProtectedAreaID)
    {
      Singular.Web.Result results = new Singular.Web.Result();
      var url = "";
      var User = METTLib.Security.UserList.GetUserList(UserID).First();

      if (User != null)
      {
        url = $"../Users/UserProfile.aspx?UserID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(User.UserID.ToString()))}&OrganisationProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(OrganisationProtectedAreaID.ToString()))}&ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedAreaID.ToString()))}";
        results.Success = true;
        results.Data = Tuple.Create(User, url);
      }
      else
      {
        results.ErrorText = "Failed to retrieve user details";
      }

      return results;
    }


    [WebCallable]
    public string ViewAssessment(int QuestionnaireAnswerSetId, int AssessmentStepId, int ProtectedAreaID)
    {
      var url = $"../Survey/Survey.aspx?QuestionnaireAnswerSetId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(QuestionnaireAnswerSetId.ToString()))}&QuestionnaireAnswerStepId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(AssessmentStepId.ToString()))}&ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedAreaID.ToString()))}";
      return url;
    }

    [WebCallable]
    public string Import(int ProtectedAreaId)
    {
      var url = $"../Survey/Import.aspx?ProtectedAreaId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedAreaId.ToString()))}";
      return url;
    }


    [WebCallable]
    public Result CreateDuplicateAssessment(METTLib.ProtectedArea.ProtectedArea ProtectedArea, string AssessmentYear)
    {
      var url = "";
      Result sr = new Result();
      System.Text.StringBuilder requiredRecordString = ValidateProtectedAreaChildLists(ProtectedArea);
      string errorText = "Unable to create a copy of a previous assessment. Please continue by creating a blank assessment.";

      ROProtectedAreaAssessmentPagedListManager = new PagedDataManager<ProtectedAreaProfileVM>((d) => d.ROProtectedAreaAssessmentPagedList, (d) => d.ROProtectedAreaAssessmentPagedListCriteria, "METTReportingName", 20, true);
      ROProtectedAreaAssessmentPagedListCriteria = new ROProtectedAreaAssessmentPagedList.Criteria();
      ROProtectedAreaAssessmentPagedListCriteria.ProtectedAreaID = ProtectedArea.ProtectedAreaID;
      ROProtectedAreaAssessmentPagedList = (ROProtectedAreaAssessmentPagedList)this.ROProtectedAreaAssessmentPagedListManager.GetInitialData(ROProtectedAreaAssessmentPagedListCriteria);

      if (ROProtectedAreaAssessmentPagedList.TotalRecords > 0)
      {

        // Published Active Questionnaire ID vs Previous Assessment Questionnaire ID
        var QuestionnaireID = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList().First(c => c.PublishInd == true && c.IsDeleted == false).QuestionnaireID;

        if (requiredRecordString.ToString() == "")
        {

          if (QuestionnaireID != 0)
          {
            try
            {
              if (ProtectedArea.ProtectedAreaID != 0)
              {

                // ReviewedById Required - requested 4/4/19
                // maybe we need to check if there is at least one completed assessment for this site/protected area, a duplicate is only to be created when this check passes
                int completedAssessmentCount = (int) METTLib.Questionnaire.QuestionnaireAnswerSetList.GetQuestionnaireAnswerSetList()?.Where(c => c.AcceptedInd == true && c.AssessmentDate != null && c.ReviewedBy != null && c.ProtectedAreaID == ProtectedArea.ProtectedAreaID)?.Count();

                if (completedAssessmentCount > 0)
                {
                  int QuestionnaireAnswwerSetId = METTLib.Assessments.Assessments.CreateDuplicateAssessment(ProtectedArea.ProtectedAreaID, Settings.CurrentUser.UserID, AssessmentYear);

                  if (QuestionnaireAnswwerSetId != 0)
                  {
                     url = $"../Survey/Survey.aspx?QuestionnaireAnswerSetId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(Convert.ToString(QuestionnaireAnswwerSetId)))}&QuestionnaireAnswerStepId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString("1"))}&ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedArea.ProtectedAreaID.ToString()))}";
                      sr.Data = url;
                      sr.Success = true;
                  }
                  else
                  {
                    sr.Success = false;
                    sr.ErrorText = errorText;
                  }
 
                }
                else
                {
                  sr.Success = false;
                  sr.ErrorText = errorText;
                }
              }

            }
            catch (Exception e)
            {

              sr.Success = false;
              sr.ErrorText = errorText;
              WebError.LogError(e, "ProtectedAreaProfile.aspx - CreateDuplicateAssessment");
            }

          }
          else
          {
            sr.Success = false;
            sr.ErrorText = errorText;
          }

        }
        else
        {
          sr.Success = false;
          sr.ErrorText = $"At least one record needs to be added for: <br> {requiredRecordString.ToString()}";
        }
      }
      else
      {
        sr.Success = false;
        sr.ErrorText = errorText;
      }

      return sr;
    }


    [WebCallable]
    public Result CreateNewAssessment(METTLib.ProtectedArea.ProtectedArea ProtectedArea, string AssessmentYear)
    {
      var url = "";
      Result sr = new Result();
      System.Text.StringBuilder requiredRecordString = ValidateProtectedAreaChildLists(ProtectedArea);

      // Save questionnaire
      var QuestionnaireID = METTLib.Questionnaire.QuestionnaireList.GetQuestionnaireList().First(c => c.PublishInd == true && c.IsDeleted == false).QuestionnaireID;

      if (requiredRecordString.ToString() == "")
      {
        if (QuestionnaireID != 0)
        {
          var NewQuestionnaireAnswerSetList = METTLib.Questionnaire.QuestionnaireAnswerSetList.NewQuestionnaireAnswerSetList();
          var NewQuestionnaireAnswerSet = new METTLib.Questionnaire.QuestionnaireAnswerSet
          {
            QuestionnaireID = QuestionnaireID,
            ProtectedAreaID = ProtectedArea.ProtectedAreaID,
            AssessmentStepID = 1,
            LegalDesignationID = ProtectedArea.LegalDesignationID,
            OrganisationID = ProtectedArea.OrganisationID,
            Area = ProtectedArea.Area,
            LeadAssessorID = METTWebSecurity.CurrentIdentity().UserID,
            LanguageID = Localisation.CurrentLanguageID,
            IsUnofficialInd = ProtectedArea.IsUnofficialProtectedAreaInd,
            AssessmentYear = AssessmentYear,
            QuestionnaireStatusID = 1
          };
          if (ProtectedArea.ProtectedAreaNationalBiomeList != null && ProtectedArea.ProtectedAreaNationalBiomeList.Count > 0)
          {
            foreach (var biome in ProtectedArea.ProtectedAreaNationalBiomeList)
            {
              METTLib.Questionnaire.QuestionnaireAnswerSetNationalBiome questionnaireAnswerSetNationalBiome = METTLib.Questionnaire.QuestionnaireAnswerSetNationalBiome.NewQuestionnaireanswersetnationalbiome();
              questionnaireAnswerSetNationalBiome.NationalBiomeID = biome.NationalBiomeID;
              NewQuestionnaireAnswerSet.QuestionnaireAnswerSetNationalBiomeList.Add(questionnaireAnswerSetNationalBiome);
            }
          }

          if (ProtectedArea.LegalDesignationID > 0)
          {
            METTLib.Questionnaire.QuestionnaireAnswerSetLegalDesignation questionnaireAnswerSetLegalDesignation = METTLib.Questionnaire.QuestionnaireAnswerSetLegalDesignation.NewQuestionnaireanswersetlegaldesignation();
            questionnaireAnswerSetLegalDesignation.LegalDesignationID = ProtectedArea.LegalDesignationID;
            NewQuestionnaireAnswerSet.QuestionnaireAnswerSetLegalDesignationList.Add(questionnaireAnswerSetLegalDesignation);
          }

          METTLib.Questionnaire.QuestionnaireAnswerSetProtectedArea questionnaireAnswerSetProtectedArea = new METTLib.Questionnaire.QuestionnaireAnswerSetProtectedArea
          {
            ProtectedAreaID = ProtectedArea.ProtectedAreaID,
            CommonName = ProtectedArea.METTReportingName,
            Area = ProtectedArea.Area,
            ManagementOrganisationID = ProtectedArea.OrganisationID,
            LegalDesignationID = ProtectedArea.LegalDesignationID,
            AddressLine1 = ProtectedArea.AddressLine1,
            AddressLine2 = ProtectedArea.AddressLine2,
            AddressCity = ProtectedArea.AddressCity,
            AddressProvinceID = ProtectedArea.AddressProvinceID,
            AddressCountryID = ProtectedArea.AddressCountryID,
            HeadOfficeContactNumber = ProtectedArea.HeadOfficeContactNumber,
            OrganisationRegionID = ProtectedArea.OrganisationRegionID,
            ContactPersonID = ProtectedArea.ContactPersonID

          };

          NewQuestionnaireAnswerSet.QuestionnaireAnswerSetProtectedAreaList.Add(questionnaireAnswerSetProtectedArea);

          if (NewQuestionnaireAnswerSet.IsValid)
          {
            Singular.SaveHelper SavedNewQuestionnaireAnswerSetSaveHelper = NewQuestionnaireAnswerSet.TrySave(typeof(METTLib.Questionnaire.QuestionnaireAnswerSetList));
            METTLib.Questionnaire.QuestionnaireAnswerSet SavedNewQuestionnaireAnswerSet = (METTLib.Questionnaire.QuestionnaireAnswerSet)SavedNewQuestionnaireAnswerSetSaveHelper.SavedObject;

            if (SavedNewQuestionnaireAnswerSetSaveHelper.Success)
            {
              METTLib.Questionnaire.QuestionnaireAnswerScore questionnaireAnswerScore = new METTLib.Questionnaire.QuestionnaireAnswerScore();
              questionnaireAnswerScore.QuestionnaireAnswerSetID = SavedNewQuestionnaireAnswerSet.QuestionnaireAnswerSetID;

              if (questionnaireAnswerScore.TrySave(typeof(METTLib.Questionnaire.QuestionnaireAnswerScoreList)).Success)
              {
                url = $"../Survey/Survey.aspx?QuestionnaireAnswerSetId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(SavedNewQuestionnaireAnswerSet.QuestionnaireAnswerSetID.ToString()))}&QuestionnaireAnswerStepId={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(SavedNewQuestionnaireAnswerSet.AssessmentStepID.ToString()))}&ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedArea.ProtectedAreaID.ToString()))}";
                sr.Data = url;
                sr.Success = true;
              }
              else
              {
                sr.ErrorText = questionnaireAnswerScore.GetErrorsAsHTMLString();
                sr.Success = false;
              }
            }
            else
            {
              sr.ErrorText = SavedNewQuestionnaireAnswerSetSaveHelper.ErrorText;
              sr.Success = false;
            }
          }
          else
          {
            sr.ErrorText = ProtectedArea.GetErrorsAsHTMLString();
            sr.Success = false;
          }
        }
        else
        {
          //no published questionnaire
          sr.Success = false;
          sr.ErrorText = "There is no published questionnaire.";
        }
      }
      else
      {
        sr.Success = false;
        sr.ErrorText = "At least one record needs to be added for: <br>" + requiredRecordString.ToString();
      }

      return sr;
    }

    [WebCallable]
    public METTLib.ProtectedArea.ProtectedArea AddProtectedAreaNationalBiome(METTLib.ProtectedArea.ProtectedArea ProtectedArea, int NationalBiomeID)
    {
      METTLib.ProtectedArea.ProtectedAreaNationalBiome protectedAreaNationalBiome;
      protectedAreaNationalBiome = METTLib.ProtectedArea.ProtectedAreaNationalBiome.NewProtectedAreaNationalBiome();
      protectedAreaNationalBiome.ProtectedAreaID = ProtectedArea.ProtectedAreaID;
      protectedAreaNationalBiome.NationalBiomeID = NationalBiomeID;

      METTLib.ProtectedArea.ProtectedAreaNationalBiome ExistingProtectedAreaNationalBiome;
      ExistingProtectedAreaNationalBiome = ProtectedArea.ProtectedAreaNationalBiomeList.Count > 0 ? ProtectedArea.ProtectedAreaNationalBiomeList.FirstOrDefault(c => c.ProtectedAreaID == ProtectedArea.ProtectedAreaID && c.NationalBiomeID == NationalBiomeID) : null;

      if (ExistingProtectedAreaNationalBiome == null)
      {
        ProtectedArea.ProtectedAreaNationalBiomeList.Add(protectedAreaNationalBiome);
      }
      else
      {
        //check is it is active
        if (ExistingProtectedAreaNationalBiome.IsActiveInd == false)
        {
          ProtectedArea.ProtectedAreaNationalBiomeList.GetItem(ExistingProtectedAreaNationalBiome.ProtectedAreaNationalBiomeID).IsActiveInd = true;
        }
      }

      return ProtectedArea;
    }

    [WebCallable]
    public METTLib.ProtectedArea.ProtectedArea RemoveProtectedAreaNationalBiome(METTLib.ProtectedArea.ProtectedArea ProtectedArea, int NationalBiomeID)
    {
      METTLib.ProtectedArea.ProtectedAreaNationalBiome ItemToRemove = ProtectedArea.ProtectedAreaNationalBiomeList.FirstOrDefault(c => c.NationalBiomeID == NationalBiomeID);
      if (ItemToRemove != null)
      {
        ProtectedArea.ProtectedAreaNationalBiomeList.GetItem(ItemToRemove.ProtectedAreaNationalBiomeID).IsActiveInd = false;
      }

      return ProtectedArea;
    }

    [WebCallable]
    public static Result CheckUserExists(METTLib.Security.User user)
    {
      Result results = new Singular.Web.Result();
      //before saving user, let's check if there are any
      if (METTLib.RO.ROUserList.GetROUserList().Any(c => c.EmailAddress == user.EmailAddress))
      {
        //user already exists
        var ExistingUser = METTLib.RO.ROUserList.GetROUserList()?.FirstOrDefault(c => c.EmailAddress == user.EmailAddress);
        results.ErrorText = $"User with the same email address already exists. User Details: {ExistingUser.FirstName} {ExistingUser.LastName}";
      }
      else
      {
        results.Success = true;
      }

      return results;
    }

    /// <summary>
    /// Save changes to a user
    /// </summary>
    /// <param name="user">A user instance</param>
    /// <returns>The save result</returns>
    [WebCallable]
    public static Result SaveUser(METTLib.Security.User user, int ProtectedAreaID, SecurityGroupProtectedAreaUserList SecurityGroupProtectedAreaUserList)
    {
      Result results = new Singular.Web.Result();

      //at least one security role should be selected from the list or else the user will not appear in the users grid for this protected area
      if (SecurityGroupProtectedAreaUserList.Count == 0)
      {
        //exit and return an error notifying user to select a security group
        results.Success = false;
        results.ErrorText = "Please add at least one security group for this site user.";
        return results;
      }

      try
      {

        //1: save system user
        //add a default security group of General User
        SecurityGroupUser securityGroupUser = SecurityGroupUser.NewSecurityGroupUser();
        securityGroupUser.SecurityGroupID = ROSecurityGroupList.GetROSecurityGroupList(true).FirstOrDefault(c => c.SecurityGroup == "General User")?.SecurityGroupID;
        user.SecurityGroupUserList.Add(securityGroupUser);

        user.Password = "12345678";
        if (user.IsValid)
        {
          Result Saveresults = user.SaveUser(user);
          METTLib.Security.User SavedUser = Saveresults.Success ? (METTLib.Security.User)Saveresults.Data : null;

          if (SavedUser != null)
          {
            var userID = SavedUser.UserID;
            //2: save protected area user and roles
            //let's get the link between organisation and protected area
            var OrganisationID = ProtectedAreaList.GetProtectedAreaList(ProtectedAreaID).FirstOrDefault(c => c.ProtectedAreaID == ProtectedAreaID)?.OrganisationID;

            if (OrganisationID != 0)
            {
              var OrganisationProtectedArea = OrganisationProtectedAreaList.GetOrganisationProtectedAreaList().FirstOrDefault(c => c.ProtectedAreaID == ProtectedAreaID && c.OrganisationID == OrganisationID);
              METTLib.ProtectedArea.OrganisationProtectedAreaUser OrganisationProtectedAreaUser = METTLib.ProtectedArea.OrganisationProtectedAreaUser.NewOrganisationProtectedAreaUser();
              OrganisationProtectedAreaUser.OrganisationIDProtectedAreaID = OrganisationProtectedArea.OrganisationProtectedAreaID;
              OrganisationProtectedAreaUser.UserID = userID;
              SecurityGroupProtectedAreaUserList.ToList().ForEach(c => { c.ProtectedAreaID = ProtectedAreaID; c.UserID = userID; });
              OrganisationProtectedAreaUser.SaveOrganisationProtectedAreaUser(OrganisationProtectedAreaUser, SecurityGroupProtectedAreaUserList, ProtectedAreaID);
            }
            else
            {
              //no link between protected area and organisation
              throw new Exception("This protected area does not have a managing organisation.");
            }

            results.Data = Tuple.Create(ROOrganisationProtectedAreaUserList.GetROOrganisationProtectedAreaUserListByProtectedArea(ProtectedAreaID), SecurityGroupProtectedAreaUserList.NewSecurityGroupProtectedAreaUserList());
            results.Success = true;
          }
          else
          {
            throw new Exception(user.GetErrorsAsHTMLString());
          }

        }
        else
        {
           throw new Exception(user.GetErrorsAsHTMLString());
        }
      }
      catch (Exception ex)
      {
        results.Success = false;
        results.ErrorText = ex.Message;
        WebError.LogError(ex, "ProtectedAreaProfile.cs - SaveUser");
      }

      return results;
    }

    [WebCallable]
    public static int DecodeUrlParam(string urlParam)
    {
      return Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(urlParam).Replace(" ", "+")));
    }

    [WebCallable]
    public static object SetOrganisation(METTLib.ProtectedArea.ProtectedArea ProtectedArea, string urlParam)
    {
      var organisationID = Convert.ToInt32(Singular.Encryption.DecryptString(System.Web.HttpUtility.UrlDecode(urlParam).Replace(" ", "+")));
      ProtectedArea.OrganisationID = organisationID;
      return ProtectedArea;
    }

    [WebCallable]
    public static Singular.Web.Result Export(METTLib.Questionnaire.QuestionnaireAnswerExportSetList QuestionnaireAnswerExportSetList, int ProtectedAreaID, int OrganisationID, int LegalDesignationID)
    {
      METTLib.ProtectedArea.ProtectedArea ProtectedArea = ProtectedAreaList.GetProtectedAreaList(ProtectedAreaID)?.FirstOrDefault();
      System.Text.StringBuilder requiredRecordString = null;

      if (ProtectedArea != null)
      {
        requiredRecordString = ValidateProtectedAreaChildLists(ProtectedArea);
      }

      if (requiredRecordString == null || requiredRecordString.ToString() == "")
      {
        var fileTimeStamp = DateTime.Now.ToString("ddMMyy-hhmmss");
        string fileName = $"METT-Assessment-Template-{fileTimeStamp}.xls";
        Singular.Documents.TemporaryDocument tempDoc = new Singular.Documents.TemporaryDocument();
        tempDoc.SetDocument(CreateExcel(QuestionnaireAnswerExportSetList, ProtectedAreaID, OrganisationID, LegalDesignationID).ToArray(), fileName);
        return new Singular.Web.Result(true) { Data = Singular.Web.WebServices.FileDownloadHandler.SaveTempFile(tempDoc) };
      }
      else
      {
        return new Singular.Web.Result(false, $"At least one record needs to be added for: <br> {requiredRecordString.ToString()}");
      }

    }

    public static StringBuilder ValidateProtectedAreaChildLists(METTLib.ProtectedArea.ProtectedArea ProtectedArea)
    {
      StringBuilder requiredRecordString = new System.Text.StringBuilder();

      //at least one record in National Biomes, Attributes and Objectives should be added
      if (ProtectedArea.ProtectedAreaNationalBiomeList == null || ProtectedArea.ProtectedAreaNationalBiomeList.Count == 0)
      {
        // Ask user to add a record
        requiredRecordString.AppendLine("National Biomes <br>");
      }
      if (ProtectedArea.ProtectedAreaPrimaryAttributeList == null || ProtectedArea.ProtectedAreaPrimaryAttributeList.Count == 0)
      {
        // Ask user to add a record
        requiredRecordString.AppendLine("Primary Biodiversity And Cultural Attributes <br>");
      }
      if (ProtectedArea.ProtectedAreaHighLevelObjectiveList == null || ProtectedArea.ProtectedAreaHighLevelObjectiveList.Count == 0)
      {
        // Ask user to add a record
        requiredRecordString.AppendLine("High Level Site Objectives <br>");
      }
      // Ask user to completed Area Managed
      if (ProtectedArea.Area == 0)
      {
        requiredRecordString.AppendLine("Protected Area Area Managed (hectares)<br>");
      }

      return requiredRecordString;
    }

    [WebCallable]
    private static MemoryStream CreateExcel(METTLib.Questionnaire.QuestionnaireAnswerExportSetList QuestionnaireAnswerExportSetList, int ProtectedAreaID, int OrganisationID, int LegalDesignationID)
    {
      Singular.Data.ExcelExporter ExcelDoc = new Singular.Data.ExcelExporter(Infragistics.Documents.Excel.WorkbookFormat.Excel2007);
      ExcelDoc.FormatAsTable = false;
      var QuestionnaireGroupList = METTLib.Questionnaire.ROQuestionnaireGroupList.GetROQuestionnaireGroupList();
      // Create sheets for each Questionnaire Group
      foreach (var item in QuestionnaireGroupList)
      {
        var vSheet = ExcelDoc.WorkBook.Worksheets.Add(item.QuestionnaireGroup);
        var QAESList = METTLib.Questionnaire.QuestionnaireAnswerExportSetList.GetQuestionnaireAnswerExportSetList(item.QuestionnaireGroupID, ProtectedAreaID, OrganisationID, LegalDesignationID);
        ExcelDoc.PopulateData(QAESList, vSheet, false, false, 0, false);

        // Column headings renaming, alignment and hiding ID columns
        vSheet.Columns[(int)ExcelColumns.OrganisationId].Width = (int)ExcelColumnSizes.Hidden;
        vSheet.Columns[(int)ExcelColumns.ProtectedAreaId].Width = (int)ExcelColumnSizes.Hidden;
        vSheet.Columns[(int)ExcelColumns.LegalDesignationId].Width = (int)ExcelColumnSizes.Hidden;
        vSheet.Columns[(int)ExcelColumns.QuestionnaireId].Width = (int)ExcelColumnSizes.Hidden;
        vSheet.Columns[(int)ExcelColumns.QuestionnaireGroupId].Width = (int)ExcelColumnSizes.Hidden;

        vSheet.Rows[0].Cells[(int)ExcelColumns.No].Value = "No";
        vSheet.Rows[0].Cells[(int)ExcelColumns.Indicator].Value = "Indicator";
        vSheet.Columns[(int)ExcelColumns.Indicator].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.Indicator].Width = (int)ExcelColumnSizes.Indicator;

        vSheet.Columns[(int)ExcelColumns.QuestionId].Width = (int)ExcelColumnSizes.Hidden;

        vSheet.Rows[0].Cells[(int)ExcelColumns.Question].Value = "Question";
        vSheet.Columns[(int)ExcelColumns.Question].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.Question].Width = (int)ExcelColumnSizes.Question;

        // Hide ID columns
        vSheet.Columns[(int)ExcelColumns.QuestionnaireQuestionTypeIdProperty].Width = (int)ExcelColumnSizes.Hidden;
        vSheet.Columns[(int)ExcelColumns.SortOrder].Width = (int)ExcelColumnSizes.Hidden;
        vSheet.Columns[(int)ExcelColumns.QuestionnaireAnswerOptionIdCombo].Width = (int)ExcelColumnSizes.Hidden;

        vSheet.Rows[0].Cells[(int)ExcelColumns.AnswerOption1].Value = "Answer Option 1";
        vSheet.Rows[0].Cells[(int)ExcelColumns.AnswerOption2].Value = "Answer Option 2";
        vSheet.Rows[0].Cells[(int)ExcelColumns.AnswerOption3].Value = "Answer Option 3";
        vSheet.Rows[0].Cells[(int)ExcelColumns.AnswerOption4].Value = "Answer Option 4";
        vSheet.Rows[0].Cells[(int)ExcelColumns.AnswerOption5].Value = "Answer Option 5";

        // AnswerOption columns resizing
        vSheet.Columns[(int)ExcelColumns.AnswerOption1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.AnswerOption1].Width = (int)ExcelColumnSizes.AnswerOption;
        vSheet.Columns[(int)ExcelColumns.AnswerOption2].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.AnswerOption2].Width = (int)ExcelColumnSizes.AnswerOption;
        vSheet.Columns[(int)ExcelColumns.AnswerOption3].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.AnswerOption3].Width = (int)ExcelColumnSizes.AnswerOption;
        vSheet.Columns[(int)ExcelColumns.AnswerOption4].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.AnswerOption4].Width = (int)ExcelColumnSizes.AnswerOption;
        vSheet.Columns[(int)ExcelColumns.AnswerOption5].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.AnswerOption5].Width = (int)ExcelColumnSizes.AnswerOption;

        vSheet.Rows[0].Cells[(int)ExcelColumns.YourAnswerOption].Value = "Your Answer Option";
        vSheet.Columns[(int)ExcelColumns.YourAnswerOption].CellFormat.Alignment = Infragistics.Documents.Excel.HorizontalCellAlignment.Center;

        vSheet.Rows[0].Cells[(int)ExcelColumns.Comments].Value = "Comments";
        vSheet.Rows[0].Cells[(int)ExcelColumns.NextSteps].Value = "Next Steps";
        vSheet.Rows[0].Cells[(int)ExcelColumns.Evidence].Value = "Evidence";

        vSheet.Columns[(int)ExcelColumns.Comments].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.Comments].Width = (int)ExcelColumnSizes.Comments;
        vSheet.Columns[(int)ExcelColumns.NextSteps].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.NextSteps].Width = (int)ExcelColumnSizes.NextSteps;
        vSheet.Columns[(int)ExcelColumns.Evidence].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        vSheet.Columns[(int)ExcelColumns.Evidence].Width = (int)ExcelColumnSizes.Evidence;

        // Frozen headings
        vSheet.DisplayOptions.PanesAreFrozen = true;
        vSheet.DisplayOptions.FrozenPaneSettings.FrozenRows = 1;
        vSheet.Rows[0].CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(System.Drawing.Color.SeaGreen);

        // Change row height
        var iTotalRows = vSheet.Rows.Count();
        var headingColour = System.Drawing.ColorTranslator.FromHtml("#1ab394");
        vSheet.Rows[0].Height = 750;
        for (int i = 1; i < iTotalRows; i++)
        {
          if (i % 2 == 0)
          {
            vSheet.Rows[i].Height = 1550;
          }
          else
          {
            vSheet.Rows[i].Height = 1550;
            vSheet.Rows[i].CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(System.Drawing.Color.LightGray);
          }
        }
      }
      // Save excel file
      MemoryStream m = new MemoryStream();
      ExcelDoc.WorkBook.Save(m);

      return m;
    }

    [WebCallable]
    public static Result RemoveProtectedAreaUser(int OrganisationProtectedAreaUserID, int UserID, int ProtectedAreaID)
    {
      Result results = new Singular.Web.Result();
      OrganisationProtectedAreaUser OrganisationProtectedAreaUser = OrganisationProtectedAreaUserList.GetOrganisationProtectedAreaUserList(OrganisationProtectedAreaUserID).FirstOrDefault();
      // Get security roles
      SecurityGroupProtectedAreaUserList securityGroupProtectedAreaUsers = SecurityGroupProtectedAreaUserList.GetSecurityGroupProtectedAreaUserList(UserID, ProtectedAreaID);
      foreach (var securityGroup in securityGroupProtectedAreaUsers)
      {
        securityGroup.IsActiveInd = false;
      }

      if (OrganisationProtectedAreaUser != null)
      {
        OrganisationProtectedAreaUser.IsActiveInd = false;
        OrganisationProtectedAreaUser.SaveOrganisationProtectedAreaUser(OrganisationProtectedAreaUser, securityGroupProtectedAreaUsers, ProtectedAreaID);
        results.Data = ROOrganisationProtectedAreaUserList.GetROOrganisationProtectedAreaUserListByProtectedArea(ProtectedAreaID);
        results.Success = true;
      }
      else
      {
        results.ErrorText = "Error occured while trying to remove protected area / site user.";
        results.Success = false;
      }
      return results;
    }

    [WebCallable]
    public static Result SaveProtectedAreaAttributes(METTLib.ProtectedArea.ProtectedArea ProtectedArea)
    {
      Result results = new Singular.Web.Result();

      try
      {
        PrimaryAttribute primaryAttribute = PrimaryAttribute.NewPrimaryAttribute();
        PrimaryAttributeList PrimaryAttributes = PrimaryAttributeList.NewPrimaryAttributeList();

        foreach (var item in ProtectedArea.ProtectedAreaPrimaryAttributeList.Where(c => c.PrimaryAttributeID == null))
        {
          primaryAttribute.PrimaryAttributeName = item.PrimaryAttribute;
          PrimaryAttributes.Add(primaryAttribute);
          //save this item and get it's primary key ID
          if (PrimaryAttributes.IsValid)
          {
            Singular.SaveHelper SavedPrimaryAttributeSaveHelper = PrimaryAttributes.TrySave();
            PrimaryAttributeList SavedPrimaryAttribute = (PrimaryAttributeList)SavedPrimaryAttributeSaveHelper.SavedObject;

            if (SavedPrimaryAttributeSaveHelper.Success)
            {
              item.PrimaryAttributeID = SavedPrimaryAttribute.FirstOrDefault().PrimaryAttributeID;
              item.ProtectedAreaID = ProtectedArea.ProtectedAreaID;
            }

          }
        }

        ProtectedArea.ProtectedAreaPrimaryAttributeList.Save();
        results.Data = METTLib.ProtectedArea.ProtectedAreaList.GetProtectedAreaList(ProtectedArea.ProtectedAreaID)?.FirstOrDefault();
        results.Success = true;
      }
      catch (Exception ex)
      {
        results.Success = false;
        results.ErrorText = ex.Message;
      }
      return results;
    }

    [WebCallable]
    public static Result SaveObjectives(METTLib.ProtectedArea.ProtectedArea ProtectedArea, ProtectedAreaHighLevelObjectiveList ObjectiveList)
    {
      Result results = new Singular.Web.Result();

      try
      {
        //save High Level objectives
        HighLevelObjective highLevelObjective = HighLevelObjective.NewHighLevelObjective();
        HighLevelObjectiveList highLevelObjectives = HighLevelObjectiveList.NewHighLevelObjectiveList();

        foreach (var item in ProtectedArea.ProtectedAreaHighLevelObjectiveList.Where(c => c.HighLevelObjectiveID == null))
        {
          highLevelObjective.HighLevelObjectiveName = item.HighLevelObjective;
          highLevelObjectives.Add(highLevelObjective);
          //save this item and get it's primary key ID
          if (highLevelObjectives.IsValid)
          {
            Singular.SaveHelper SavedHighLevelObjectiveSaveHelper = highLevelObjectives.TrySave();
            HighLevelObjectiveList SavedHighLevelObjective = (HighLevelObjectiveList)SavedHighLevelObjectiveSaveHelper.SavedObject;

            if (SavedHighLevelObjectiveSaveHelper.Success)
            {
              item.HighLevelObjectiveID = SavedHighLevelObjective.FirstOrDefault().HighLevelObjectiveID;
              item.ProtectedAreaID = ProtectedArea.ProtectedAreaID;
            }

          }
        }

        ProtectedArea.ProtectedAreaHighLevelObjectiveList.TrySave();
        results.Data = METTLib.ProtectedArea.ProtectedAreaList.GetProtectedAreaList(ProtectedArea.ProtectedAreaID)?.FirstOrDefault();
        results.Success = true;
      }
      catch (Exception ex)
      {
        results.Success = false;
        results.ErrorText = ex.Message;
      }
      return results;
    }

    [Singular.Web.WebCallable(LoggedInOnly = true)]
    public string ViewOrganisation(int OrganisationID, int ProtectedAreaID)
    {
      var url = $"../Organisation/OrganisationProfile.aspx?OrganisationID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(OrganisationID.ToString()))}&ProtectedAreaID={HttpUtility.UrlEncode(Singular.Encryption.EncryptString(ProtectedAreaID.ToString()))}";
      return url;
    }
  }
}

