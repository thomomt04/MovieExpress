<%@ Page Title="METT - Protected Areas / Sites" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProtectedAreaProfile.aspx.cs" Inherits="METTWeb.ProtectedArea.ProtectedAreaProfile" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
  <link href="../Theme/Singular/METTCustomCss/protectedareaprofile.css" rel="stylesheet" />
  <script type="text/javascript" src="../Scripts/JSLINQ.js"></script>
  <script type="text/javascript" src="../Scripts/accesscheck.js"></script>

  <style>
    .SortIcon {
      background-image: url('../Images/IconSort.png') !important;
    }

    .BtnPadding {
      margin-top: 22px;
    }
  </style>

</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
  <%
    using (var h = this.Helpers)
    {
      //h.HTML().Heading2("Protected Areas");
    }
  %>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
  <%	using (var h = this.Helpers)
    {
      var MainHDiv = h.DivC("row p-h-xs");
      {
      }
      var rowDiv = MainHDiv.Helpers.DivC("row");
      {
        var gridDivMain = rowDiv.Helpers.DivC("col-lg-12 paddingTop15 ");
        {
          var cardTableDiv = gridDivMain.Helpers.DivC("ibox float-e-margins paddingBottom");
          {
            cardTableDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingProtectedAreaInd == false);
            var cardTitleDiv = cardTableDiv.Helpers.DivC("ibox-title");
            {
              cardTitleDiv.Helpers.HTML("<i class='fa fa-user fa-lg fa-fw pull-left'></i>");
              cardTitleDiv.Helpers.HTML().Heading5("Protected Areas / Sites");
            }
            var cardToolsDiv = cardTitleDiv.Helpers.DivC("ibox-tools");
            {
              var aToolsTag = cardToolsDiv.Helpers.HTMLTag("a");
              aToolsTag.AddClass("collapse-link");
              {
                var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
                iToolsTag.AddClass("fa fa-chevron-up");
              }
            }
            var cardContentDiv = cardTableDiv.Helpers.DivC("ibox-content");
            {
              var TableRow = cardContentDiv.Helpers.DivC("row m-n");
              {
                TableRow.AddBinding(Singular.Web.KnockoutBindingString.visible, c => c.IsViewingProtectedAreaInd == false);
                // Protected area paged grid and filter
                var FilterButtonRow = TableRow.Helpers.DivC("row filterFrame m-n");
                {
                  var FilterNameCol = FilterButtonRow.Helpers.DivC("col-md-4 p-l-n");
                  {
                    FilterNameCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.METTReportingName);
                    FilterNameCol.AddClass("control-label");
                    var FilterNameEditor = FilterNameCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.METTReportingName);
                    FilterNameEditor.AddClass("form-control marginBottom20 filterBox");
                  }
                  var FilterOfficialNameCol = FilterButtonRow.Helpers.DivC("col-md-4 p-l-n");
                  {
                    FilterOfficialNameCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.OfficialName);
                    FilterOfficialNameCol.AddClass("control-label");
                    var FilterOfficialNameEditor = FilterOfficialNameCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.OfficialName);
                    FilterOfficialNameEditor.AddClass("form-control marginBottom20 filterBox");
                  }
                  var ClearFilterCol = FilterButtonRow.Helpers.DivC("col-md-4 p-l-n paddingTop24");
                  {

                    var SearchBtn = ClearFilterCol.Helpers.Button("Search", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                    {
                      SearchBtn.Style.FloatLeft();
                      SearchBtn.AddClass("btn-primary btn btn btn-outline filterButtonWidths");
                      SearchBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewModel.ROProtectedAreaPagedListManager().Refresh();");
                    }

                    var ClearBtn = ClearFilterCol.Helpers.Button("Clear", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                    {
                      ClearBtn.Style.FloatLeft();
                      ClearBtn.AddClass("btn-default btn m-l-4 filterButtonWidths ");
                      ClearBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ClearFilters()");
                    }
                    //var ButtonsCol = FilterRow.Helpers.DivC("col-md-1 paddingTop24 p-w-lg");
                    //{
                    var NewCol = ClearFilterCol.Helpers.Div();
                    {
                      var NewProtectedAreaBtn = NewCol.Helpers.Button("New", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                      {
                        NewProtectedAreaBtn.AddClass("btn-primary btn pull-right filterButtonWidths");
                        NewProtectedAreaBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewProtectedArea()");
                        NewProtectedAreaBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageNewProtectedAreaSiteBtnInd == true);
                        //}
                      }
                    }
                  }

                  var FilterRow = TableRow.Helpers.DivC("row filterFrame m-n");
                  {
                    var FilterRegionCol = FilterRow.Helpers.DivC("col-md-4 p-l-n ");
                    {
                      FilterRegionCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.RegionID);
                      FilterRegionCol.AddClass("control-label");
                      var FilterRegionEditor = FilterRegionCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.RegionID);
                      FilterRegionEditor.AddClass("form-control marginBottom20 filterBox");
                    }
                    var FilterProvinceCol = FilterRow.Helpers.DivC("col-md-4 p-l-n");
                    {
                      FilterProvinceCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.ProvinceID);
                      FilterProvinceCol.AddClass("control-label");
                      var FilterProvinceEditor = FilterProvinceCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.ProvinceID);
                      FilterProvinceEditor.AddClass("form-control marginBottom20 filterBox");
                    }
                    var FilterRelatedOrganisationCol = FilterRow.Helpers.DivC("col-md-4 p-l-n");
                    {
                      FilterRelatedOrganisationCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.RelatedOrganisationID);
                      FilterRelatedOrganisationCol.AddClass("control-label");
                      var FilterRelatedOrgEditor = FilterRelatedOrganisationCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.RelatedOrganisationID);
                      FilterRelatedOrgEditor.AddClass("form-control marginBottom20 filterBox");
                    }


                  }
                }
                var ProtectedAreaDiv = TableRow.Helpers.DivC("row col-lg-12 p-r-n");
                {
                  var ProtectedAreaPagedList = ProtectedAreaDiv.Helpers.PagedGridFor<METTLib.ProtectedArea.ROProtectedAreaPaged>(c => c.ROProtectedAreaPagedListManager, c => c.ROProtectedAreaPagedList, false, false);
                  {
                    ProtectedAreaPagedList.AddClass("table-responsive table-striped table table-bordered");
                    var ProtectedAreaFirstRow = ProtectedAreaPagedList.FirstRow;
                    {
                      var ProtectedAreaName = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.METTReportingName);
                      var OfficialName = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.OfficialName);
                      {
                        OfficialName.Style.TextAlign = Singular.Web.TextAlign.left;
                      }
                      var OrganisationName = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.OrganisationName);
                      {
                        OfficialName.Style.TextAlign = Singular.Web.TextAlign.left;
                      }
                      var Region = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.Region);
                      {
                        Region.Style.TextAlign = Singular.Web.TextAlign.left;
                      }
                      var Province = ProtectedAreaFirstRow.AddReadOnlyColumn(c => c.Province);
                      {
                        Province.Style.TextAlign = Singular.Web.TextAlign.left;
                      }
                      var ViewCol = ProtectedAreaFirstRow.AddColumn();
                      {
                        ViewCol.Style.Width = "80px";
                        ViewCol.HeaderText = "View";
                        var viewBtn = ViewCol.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                        {
                          viewBtn.AddClass("btn btn-outline btn-primary");
                          viewBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewProtectedArea($data)");
                          viewBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewProtectedAreaSiteBtnInd == true);
                        }
                      }
                    }
                  }
                }
              }
            }
          }

          #region ProtectedAreas            
          var ProtectedAreaDetailsDiv = gridDivMain.Helpers.Div();
          {
            ProtectedAreaDetailsDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingProtectedAreaInd == true);
            var TabContainer = ProtectedAreaDetailsDiv.Helpers.DivC("tabs-container");
            {
              var EntityTab = TabContainer.Helpers.TabControl();
              {
                EntityTab.Style.ClearBoth();
                EntityTab.AddClass("nav nav-tabs");
                var ProtectedAreaDetailsTab = EntityTab.AddTab("Protected Area / Site Details");
                {
                  var ProtectedAreaContent = ProtectedAreaDetailsTab.Helpers.With<METTLib.ProtectedArea.ProtectedArea>(c => c.EditingProtectedArea);
                  {

                    var ProtectedAreaMETTName = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      ProtectedAreaMETTName.Helpers.BootstrapEditorRowFor(c => c.METTReportingName);
                    }
                    var OfficialName = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      OfficialName.Helpers.BootstrapEditorRowFor(c => c.OfficialName);
                    }
                    var Area = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      Area.Helpers.BootstrapEditorRowFor(c => c.Area);
                    }
                    var OrganisationDiv = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      var ManagementOrganisationID = OrganisationDiv.Helpers.DivC("col-md-10 Paddingleft0 ClearBoth");
                      {
                        ManagementOrganisationID.Helpers.BootstrapEditorRowFor(c => c.OrganisationID);
                      }

                      var ViewBtn = OrganisationDiv.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                      {
                        ViewBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewOrganisation()");
                        ViewBtn.AddClass("btn-primary btn BtnPadding");
                        ViewBtn.Style.FloatRight();
                        ViewBtn.AddBinding(Singular.Web.KnockoutBindingString.enable, c => (ViewModel.EditingProtectedArea != null && !ViewModel.EditingProtectedArea.IsNew) && c.OrganisationID != 0);
                      }
                    }
                    var LegalDesignationID = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      LegalDesignationID.Helpers.BootstrapEditorRowFor(c => c.LegalDesignationID);
                    }
                    var AddressLine1 = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      AddressLine1.Helpers.BootstrapEditorRowFor(c => c.AddressLine1);
                    }
                    var AddressLine2 = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      AddressLine2.Helpers.BootstrapEditorRowFor(c => c.AddressLine2);
                    }
                    var AddressCity = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      AddressCity.Helpers.BootstrapEditorRowFor(c => c.AddressCity);
                    }
                    var AddressProvinceID = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      AddressProvinceID.Helpers.BootstrapEditorRowFor(c => c.AddressProvinceID);
                    }
                    var AddressCountryID = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      AddressCountryID.Helpers.BootstrapEditorRowFor(c => c.AddressCountryID);
                    }
                    var HeadOfficeContactNumber = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      HeadOfficeContactNumber.Helpers.BootstrapEditorRowFor(c => c.HeadOfficeContactNumber);
                    }
                    var OrganisationRegionID = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      OrganisationRegionID.Helpers.BootstrapEditorRowFor(c => c.OrganisationRegionID);
                    }
                    var ContactPersonID = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      ContactPersonID.Helpers.BootstrapEditorRowFor(c => c.ContactPersonID);
                    }
                    var UnOfficial = ProtectedAreaContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
                    {
                      UnOfficial.Helpers.LabelFor(c => c.IsUnofficialProtectedAreaInd);
                      UnOfficial.AddClass("control-label");
                      var UnOfficialEditor = UnOfficial.Helpers.EditorFor(c => c.IsUnofficialProtectedAreaInd);
                      UnOfficialEditor.AddClass("checkbox");
                    }
                  }
                }

                var AssessmentTab = EntityTab.AddTab("Assessment");
                {
                  var AssessmentrowDiv = AssessmentTab.Helpers.DivC("row");
                  {
                    var AssessmentDivMain = AssessmentrowDiv.Helpers.DivC("col-lg-12 paddingTop15 p-r-md");
                    {
                      var cardAssessmentContentDiv = AssessmentDivMain.Helpers.DivC("ibox float-e-margins paddingBottom");
                      {
                        var cardAssessmentTitleDiv = cardAssessmentContentDiv.Helpers.DivC("ibox-title");
                        {
                          cardAssessmentTitleDiv.Helpers.HTML("<i class='fa fa-file fa-lg fa-fw pull-left'></i>");
                          cardAssessmentTitleDiv.Helpers.HTML().Heading5("Assessment");
                        }
                        var cardToolsDiv = cardAssessmentTitleDiv.Helpers.DivC("ibox-tools");
                        {
                          var aToolsTag = cardToolsDiv.Helpers.HTMLTag("a");
                          aToolsTag.AddClass("collapse-link");
                          {
                            var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
                            iToolsTag.AddClass("fa fa-chevron-up");
                          }
                        }
                        var cardContentDiv = cardAssessmentContentDiv.Helpers.DivC("ibox-content");
                        {
                          var TableRowMessage = cardContentDiv.Helpers.DivC("row assessmentMessageBox text-center");
                          {
                            TableRowMessage.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingProtectedAreaInd == true && c.EditingProtectedArea.IsNew);
                            TableRowMessage.Helpers.Span("<div><i class='fa fa-info-circle infoIconStyle'></i></div><i class='assessmentMessage'>Please save the Protected Area in order to use this functionality.</i><br><br>");
                          }

                          var TableRow = cardContentDiv.Helpers.DivC("row m-n");
                          {
                            TableRow.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingProtectedAreaInd == true);

                            var FilterRow = TableRow.Helpers.DivC("row filterFrame marginBottom20");
                            {
                              FilterRow.AddBinding(Singular.Web.KnockoutBindingString.visible, c => c.EditingProtectedArea != null && !c.EditingProtectedArea.IsNew);

                              var ButtonsCol = FilterRow.Helpers.DivC("col-md-6 p-w-lg");
                              {

                                ButtonsCol.Style.FloatRight();

                                var NewCol = ButtonsCol.Helpers.Div();
                                {
                                  var NewAssessmentBtn = NewCol.Helpers.Button("New Assessment", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                  {
                                    NewAssessmentBtn.Style.FloatRight();
                                    NewAssessmentBtn.AddClass("btn-primary btn btn btn-primary m-r");
                                    NewAssessmentBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "CreateNewAssessment()");
                                    NewAssessmentBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageNewAssessmentProtectedAreaSiteBtnInd == true);
                                  }
                                }

                                var ExportTemplateCol = ButtonsCol.Helpers.Div();
                                {
                                  var ExportTemplateBtn = ExportTemplateCol.Helpers.Button("Export Template", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                  {
                                    ExportTemplateBtn.Style.FloatRight();
                                    ExportTemplateBtn.AddClass("btn-primary btn btn-outline btn-primary m-r");
                                    ExportTemplateBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ExportAssessment()");
                                    ExportTemplateBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageExportAssessmentProtectedAreaSiteBtnInd == true);
                                  }
                                }

                                var ImportTemplateCol = ButtonsCol.Helpers.Div();
                                {
                                  var ImportTemplateBtn = ImportTemplateCol.Helpers.Button("Import Template", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                  {
                                    ImportTemplateBtn.Style.FloatRight();
                                    ImportTemplateBtn.AddClass("btn-primary btn btn btn-info m-r");
                                    ImportTemplateBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ImportAssessment()");
                                    ImportTemplateBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageImportAssessmentProtectedAreaSiteBtnInd == true);
                                  }
                                }
                              }
                            }

                            var AssessmentDiv = TableRow.Helpers.DivC("row col-lg-12 p-r-n");
                            {
                              AssessmentDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => c.EditingProtectedArea != null && !c.EditingProtectedArea.IsNew);

                              var AssessmentPagedList = AssessmentDiv.Helpers.PagedGridFor<METTLib.ProtectedArea.ROProtectedAreaAssessmentPaged>(c => c.ROProtectedAreaAssessmentPagedListManager, c => c.ROProtectedAreaAssessmentPagedList, false, false);
                              {
                                AssessmentPagedList.AddClass("table-responsive table-striped table table-bordered");

                                var AssessmentFirstRow = AssessmentPagedList.FirstRow;
                                {
                                  var METTReportingName = AssessmentFirstRow.AddReadOnlyColumn(c => c.METTReportingName);
                                  var OfficialName = AssessmentFirstRow.AddReadOnlyColumn(c => c.OfficialName);
                                  var OrganisationName = AssessmentFirstRow.AddReadOnlyColumn(c => c.OrganisationName);
                                  var CreatedBy = AssessmentFirstRow.AddReadOnlyColumn(c => c.CreatedBy);
                                  {
                                    CreatedBy.Style.TextAlign = Singular.Web.TextAlign.left;
                                  }

                                  var AssessmentDate = AssessmentFirstRow.AddReadOnlyColumn(c => c.AssessmentDate);
                                  {
                                    AssessmentDate.Style.TextAlign = Singular.Web.TextAlign.left;
                                  }

                                  var ViewCol = AssessmentFirstRow.AddColumn();
                                  {
                                    ViewCol.Style.Width = "150px";
                                    ViewCol.HeaderText = "View";

                                    var viewBtn = ViewCol.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                    {
                                      viewBtn.AddClass("btn btn-outline btn-primary");
                                      viewBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewAssessment($data)");
                                      viewBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewAssessmentProtectedAreaSiteBtnInd == true);
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
          #endregion

          #region ProtectedAreaTabs
          var SpacerRow = ProtectedAreaDetailsDiv.Helpers.DivC("row pad-top-20");
          {
          }
          var ProtectedAreaTabDiv = gridDivMain.Helpers.DivC("row margin0");
          var ProtectedAreaTabContainer = ProtectedAreaTabDiv.Helpers.DivC("tabs-container");
          {
            ProtectedAreaTabContainer.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingProtectedAreaInd == true);
            var TabContentLeft = ProtectedAreaTabContainer.Helpers.DivC("tabs-left userTabs");
            {
              var TabList = TabContentLeft.Helpers.HTMLTag("ul");
              {
                TabList.AddClass("nav nav-tabs");
                TabList.Attributes.Add("id", "navigation");

                var UserTabItem = TabList.Helpers.HTMLTag("li");
                {
                  UserTabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabsUsers");
                  var TabItemLink = UserTabItem.Helpers.HTMLTag("a");
                  {
                    TabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => "Associated Users");
                    TabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-Users");
                    TabItemLink.Attributes.Add("data-toggle", "tab");
                  }
                }

                var BiodiversityTabItem = TabList.Helpers.HTMLTag("li");
                {
                  BiodiversityTabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabsBiodiversity");
                  var BiodiversityTabItemLink = BiodiversityTabItem.Helpers.HTMLTag("a");
                  {
                    BiodiversityTabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => "Primary Biodiversity And Cultural Attributes");
                    BiodiversityTabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-Biodiversity");
                    BiodiversityTabItemLink.Attributes.Add("data-toggle", "tab");
                  }
                }

                var ObjectivesTabItem = TabList.Helpers.HTMLTag("li");
                {
                  ObjectivesTabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabsObjectives");
                  var ObjectivesTabItemLink = ObjectivesTabItem.Helpers.HTMLTag("a");
                  {
                    ObjectivesTabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => "High Level Site Objectives");
                    ObjectivesTabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-Objectives");
                    ObjectivesTabItemLink.Attributes.Add("data-toggle", "tab");
                  }
                }

                var BiomesTabItem = TabList.Helpers.HTMLTag("li");
                {
                  BiomesTabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabsBiomes");
                  var BiomesTabItemLink = BiomesTabItem.Helpers.HTMLTag("a");
                  {
                    BiomesTabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => "National Biomes");
                    BiomesTabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-Biomes");
                    BiomesTabItemLink.Attributes.Add("data-toggle", "tab");
                  }
                }

                var OtherDesignationsTabItem = TabList.Helpers.HTMLTag("li");
                {
                  OtherDesignationsTabItem.AddBinding(Singular.Web.KnockoutBindingString.id, c => "userTabsDesignations");
                  var OtherDesignationsTabItemLink = OtherDesignationsTabItem.Helpers.HTMLTag("a");
                  {
                    OtherDesignationsTabItemLink.Helpers.Span().AddBinding(Singular.Web.KnockoutBindingString.text, c => "Other Designations");

                    OtherDesignationsTabItemLink.AddBinding(Singular.Web.KnockoutBindingString.href, c => "#tab-Designations");
                    OtherDesignationsTabItemLink.Attributes.Add("data-toggle", "tab");
                  }
                }
              }

              var TabItemsContents = TabContentLeft.Helpers.DivC("tab-content");
              {
                TabItemsContents.Attributes.Add("id", "tab-content");
              }

              #region Associated Users Contents
              var TabPane = TabItemsContents.Helpers.DivC("tab-pane");
              TabPane.AddBinding(Singular.Web.KnockoutBindingString.id, c => "tab-Users");
              TabPane.AddBinding(Singular.Web.KnockoutBindingString.css, "{'active': ViewModel.ActiveTab() == 'Users'}");
              var TabPaneBody = TabPane.Helpers.DivC("panel-body panelRaiseShadow");
              {
                var TabPaneTitle = TabPaneBody.Helpers.DivC("ibox-title ibox-titleContainer");
                {
                  TabPaneTitle.Helpers.HTML("<i class='fa fa-users fa-lg fa-fw pull-left'></i>");
                  TabPaneTitle.Helpers.HTML().Heading5("Associated Users");
                }
                var TabUsers = TabPaneBody.Helpers.DivC("");
                {
                  var UserDetailsTable = TabUsers.Helpers.BootstrapTableFor<METTLib.ProtectedArea.ROOrganisationProtectedAreaUser>(c => c.ROOrganisationProtectedAreaUserList, false, false, "");
                  {
                    var UserTableFirstRow = UserDetailsTable.FirstRow;
                    {
                      var FirstName = UserTableFirstRow.AddReadOnlyColumn(c => c.FirstName);
                      {
                        FirstName.HeaderText = "First Name";
                      }
                      var LastName = UserTableFirstRow.AddReadOnlyColumn(c => c.LastName);
                      {
                        LastName.HeaderText = "Last Name";
                      }
                      var EmailAddress = UserTableFirstRow.AddReadOnlyColumn(c => c.EmailAddress);
                      {
                        EmailAddress.HeaderText = "Email Address";
                      }

                      var EditCol = UserTableFirstRow.AddColumn("View");
                      {
                        var EditBtn = EditCol.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                        {
                          EditBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "EditProtectedAreaUser($data)");
                          EditBtn.AddClass("btn btn-outline btn-info btn");
                          EditBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageViewUserProtectedAreaSiteBtnInd == true);
                        }
                        var RemoveBtn = EditCol.Helpers.Button("Remove", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                        {
                          RemoveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "RemoveProtectedAreaUser($data)");
                          RemoveBtn.AddClass("btn btn-outline btn-danger btn");
                          RemoveBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.PageRemoveUserProtectedAreaSiteBtnInd == true);
                        }
                      }
                    }
                  }
                  // Add button to create new protected area users
                  var AddUserRow = TabUsers.Helpers.DivC("row");
                  {
                    var AdduserRowCol = AddUserRow.Helpers.DivC("col-md-12 pad-top-15");
                    AddUserRow.Style.FloatRight();
                    var AddNewUserBtn = AdduserRowCol.Helpers.Button("Add New User", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                    {
                      AddNewUserBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "EditUser()");
                      AddNewUserBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => (c.EditingProtectedArea != null && !c.EditingProtectedArea.IsNew) && ViewModel.PageAddNewUserProtectedAreaSiteBtnInd == true);
                      AddNewUserBtn.AddClass("btn-primary btn");
                      AddNewUserBtn.Attributes.Add("data-toggle", "modal");
                      AddNewUserBtn.Attributes.Add("data-target", "#modalNewUserAdd");
                    }
                    var AddExistingUserBtn = AdduserRowCol.Helpers.Button("Add Existing User", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                    {
                      AddExistingUserBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "AddUser(true)");
                      AddExistingUserBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => (c.EditingProtectedArea != null && !c.EditingProtectedArea.IsNew) && ViewModel.PageAddExistingUserProtectedAreaSiteBtnInd == true);
                      AddExistingUserBtn.AddClass("btn btn-primary btn-outline");
                      AddExistingUserBtn.Attributes.Add("data-toggle", "modal");
                      AddExistingUserBtn.Attributes.Add("data-target", "#modalExistingUserAdd");
                    }
                  }

                  var UserModalDiv = MainHDiv.Helpers.DivC("modal fade");
                  {

                    UserModalDiv.Attributes.Add("id", "modalExistingUserAdd");
                    UserModalDiv.Attributes.Add("role", "dialog");
                    UserModalDiv.Attributes.Add("aria-hidden", "true");
                    UserModalDiv.Attributes.Add("tabindex", "-1");
                    UserModalDiv.Attributes.Add("style", "display: none;");
                    var UserModalDivDialog = UserModalDiv.Helpers.DivC("modalExistingUserAdd");
                    {
                      UserModalDivDialog.AddClass("modal-dialog");
                      var UserModalDivDialogContent = UserModalDiv.Helpers.DivC("modalExistingUserAdd");
                      {
                        UserModalDivDialogContent.AddClass("modal-content animated fadeIn");

                        var UserModalContentHeader = UserModalDivDialogContent.Helpers.DivC("modal-header");
                        {
                          UserModalContentHeader.Helpers.HTML().Heading2("Add Existing User");
                        }
                        var UserModalContentBody = UserModalDivDialogContent.Helpers.DivC("modal-body");
                        {
                          var FilterRow = UserModalDivDialogContent.Helpers.DivC("row filterFrame");
                          {
                            var FilterNameCol = FilterRow.Helpers.DivC("col-md-3");
                            {
                              FilterNameCol.Helpers.LabelFor(c => c.UserCriteria.FirstName);
                              FilterNameCol.AddClass("control-label");
                              var FilterNameEditor = FilterNameCol.Helpers.EditorFor(c => c.UserCriteria.FirstName);
                              FilterNameEditor.AddClass("form-control marginBottom20 filterBox");
                            }
                            var FilterSurnameCol = FilterRow.Helpers.DivC("col-md-3");
                            {
                              FilterSurnameCol.Helpers.LabelFor(c => c.UserCriteria.LastName);
                              FilterSurnameCol.AddClass("control-label");
                              var FilterSurnameEditor = FilterSurnameCol.Helpers.EditorFor(c => c.UserCriteria.LastName);
                              FilterSurnameEditor.AddClass("form-control marginBottom20 filterBox");
                            }
                            var FilterEmailAddress = FilterRow.Helpers.DivC("col-md-3");
                            {
                              FilterEmailAddress.Helpers.LabelFor(c => c.UserCriteria.EmailAddress);
                              FilterEmailAddress.AddClass("control-label");
                              var FilterEmailEditor = FilterEmailAddress.Helpers.EditorFor(c => c.UserCriteria.EmailAddress);
                              FilterEmailEditor.AddClass("form-control marginBottom20 filterBox");
                            }
                            var ClearFilterCol = FilterRow.Helpers.DivC("col-md-3 paddingTop24");
                            {
                              var ClearBtn = ClearFilterCol.Helpers.Button("Clear", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                              {
                                ClearBtn.Style.FloatRight();
                                ClearBtn.AddClass("btn-primary btn btn btn-primary");
                                ClearBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ClearUserFilters()");
                              }
                            }
                          }
                          var UserModalContentBodyDiv = UserModalDivDialogContent.Helpers.DivC("row");
                          {
                            var UserModalContentBodyDivCol = UserModalContentBodyDiv.Helpers.DivC("col-md-12 ExistingUserTablePadding30");
                            {

                              var grid = UserModalContentBodyDivCol.Helpers.PagedGridFor<METTLib.Security.ROUserPaged>(c => c.UserListPageManager, c => c.UserList, false, false);
                              {
                                grid.AddClass("table-responsive table table-striped table-bordered table-hover");
                                var firstGridRow = grid.FirstRow;
                                {
                                  firstGridRow.AddClass("table-responsive table table-bordered");
                                  firstGridRow.AddReadOnlyColumn(c => c.FirstName, 150);
                                  firstGridRow.AddReadOnlyColumn(c => c.LastName, 150);
                                  var editColumn = firstGridRow.AddColumn();
                                  editColumn.Style.Width = "150";
                                  editColumn.Style.TextAlign = Singular.Web.TextAlign.center;
                                  var editButton = editColumn.Helpers.Button("Select", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                  editButton.AddClass("btn-outline btn-info");
                                  editButton.AddBinding(Singular.Web.KnockoutBindingString.click, "EditUser($data)");
                                }
                              }
                            }
                          }
                        }
                      }
                      var UserModalContentFooter = UserModalDivDialogContent.Helpers.DivC("modal-footer");
                      {
                        var UserCloseBtn = UserModalContentFooter.Helpers.Button("Close", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                        {
                          UserCloseBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "");
                          UserCloseBtn.AddClass("btn btn-white");
                          UserCloseBtn.Attributes.Add("data-dismiss", "modal");
                        }
                      }
                    }
                  }
                  var NewUserModalDiv = MainHDiv.Helpers.DivC("modal fade");
                  {

                    NewUserModalDiv.Attributes.Add("id", "modalNewUserAdd");
                    NewUserModalDiv.Attributes.Add("role", "dialog");
                    NewUserModalDiv.Attributes.Add("aria-hidden", "true");
                    NewUserModalDiv.Attributes.Add("tabindex", "-2");
                    NewUserModalDiv.Attributes.Add("style", "display: none;");
                    var NewUserModalDivDialog = NewUserModalDiv.Helpers.DivC("modalNewUserAdd");
                    {
                      NewUserModalDivDialog.AddClass("modal-dialog");
                      var NewUserModalDivDialogContent = NewUserModalDiv.Helpers.DivC("modalNewUserAdd");
                      {
                        NewUserModalDivDialogContent.AddClass("modal-content animated fadeIn");
                        var UserModalContentHeader = NewUserModalDivDialogContent.Helpers.DivC("modal-header");
                        {
                          UserModalContentHeader.Helpers.HTML().Heading2("Add New User");
                        }
                        var UserModalContentBody = NewUserModalDivDialogContent.Helpers.DivC("modal-body");
                        {
                          var UserModalContentBodyDiv = UserModalContentBody.Helpers.DivC("row margin0");
                          {
                            var grid = NewUserModalDivDialogContent.Helpers.With<METTLib.Security.User>(c => c.EditingUser);
                            {
                              var userName = grid.Helpers.DivC("col-md-12");
                              {
                                userName.Helpers.BootstrapEditorRowFor(c => c.FirstName);
                              }
                              var Surname = grid.Helpers.DivC("col-md-12");
                              {
                                Surname.Helpers.BootstrapEditorRowFor(c => c.Surname);
                              }
                              var emailAddress = grid.Helpers.DivC("col-md-12");
                              {
                                emailAddress.Helpers.BootstrapEditorRowFor(c => c.EmailAddress);
                              }
                              var ContactNumber = grid.Helpers.DivC("col-md-12");
                              {
                                ContactNumber.Helpers.BootstrapEditorRowFor(c => c.ContactNumber);
                              }
                              var JobDescription = grid.Helpers.DivC("col-md-12");
                              {
                                JobDescription.Helpers.BootstrapEditorRowFor(c => c.JobDescription);
                              }
                              var SecurityTable = grid.Helpers.TableFor<METTLib.ProtectedArea.SecurityGroupProtectedAreaUser>(c => ViewModel.SecurityGroupProtectedAreaUserList, true, true);
                              SecurityTable.AddClass("table-responsive table table-striped table-bordered table-hover");
                              SecurityTable.AddNewButtonLocation = Singular.Web.Controls.Controls.TableAddButtonLocationType.InHeader;
                              SecurityTable.FirstRow.AddColumn(c => c.SecurityGroupID, 380);
                            }
                          }
                        }
                      }
                      var UserModalContentFooter = NewUserModalDivDialogContent.Helpers.DivC("modal-footer");
                      {
                        var UserCloseBtn = UserModalContentFooter.Helpers.Button("Close", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                        {
                          UserCloseBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "");
                          UserCloseBtn.AddClass("btn btn-white");
                          UserCloseBtn.Attributes.Add("data-dismiss", "modal");
                        }
                        var UserSaveBtn = UserModalContentFooter.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                        {
                          UserSaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveUser($data)");
                          UserSaveBtn.AddClass("btn btn-primary");
                        }
                      }
                    }
                  }
                }
              }
              #endregion

              #region Biodiversity
              var TabPane1 = TabItemsContents.Helpers.DivC("tab-pane");
              TabPane1.AddBinding(Singular.Web.KnockoutBindingString.id, c => "tab-Biodiversity");
              var TabPaneBody1 = TabPane1.Helpers.DivC("panel-body panelRaiseShadow");
              {
                var TabPaneTitle = TabPaneBody1.Helpers.DivC("ibox-title ibox-titleContainer");
                {
                  TabPaneTitle.Helpers.HTML("<i class='fa fa-paw fa-lg fa-fw pull-left'></i>");
                  TabPaneTitle.Helpers.HTML().Heading5("Primary Biodiversity & Cultural attributes");
                }
                var TabBiodiversity = TabPaneBody1.Helpers.DivC("");
                {
                  TabBiodiversity.Helpers.HTML().Heading3("List primary biodiversity and cultural attributes (values) as listed in the management plan, which underline the purpose of the protection of the site.");
                  var BiodiversityTable = TabBiodiversity.Helpers.TableFor<METTLib.ProtectedArea.ProtectedAreaPrimaryAttribute>(c => c.EditingProtectedArea.ProtectedAreaPrimaryAttributeList, ViewModel.PageNewBiodiversityProtectedAreaSiteBtnInd, ViewModel.PageRemoveBiodiversityProtectedAreaSiteBtnInd);
                  {
                    var Firstrow = BiodiversityTable.FirstRow;
                    {
                      var Name = Firstrow.AddColumn(c => c.PrimaryAttribute);

                    }
                  }
                }
                var SaveAttributeRow = TabPaneBody1.Helpers.DivC("row");
                {
                  var SaveAttributeRowCol = SaveAttributeRow.Helpers.DivC("col-md-12 pad-top-15");
                  SaveAttributeRow.Style.FloatRight();

                  var SaveAttributeBtn = SaveAttributeRowCol.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                  {
                    SaveAttributeBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveProtectedAreaAttributes()");
                    SaveAttributeBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => (c.EditingProtectedArea != null && !c.EditingProtectedArea.IsNew) && ViewModel.PageSaveBiodiversityProtectedAreaSiteBtnInd == true);
                    SaveAttributeBtn.AddClass("btn-primary btn");
                  }
                }

              }
              #endregion

              #region Objectives
              var TabPane2 = TabItemsContents.Helpers.DivC("tab-pane");
              TabPane2.AddBinding(Singular.Web.KnockoutBindingString.id, c => "tab-Objectives");
              var TabPaneBody2 = TabPane2.Helpers.DivC("panel-body panelRaiseShadow");
              {
                var TabPaneTitle = TabPaneBody2.Helpers.DivC("ibox-title ibox-titleContainer");
                {
                  TabPaneTitle.Helpers.HTML("<i class='fa fa-paw fa-lg fa-fw pull-left'></i>");
                  TabPaneTitle.Helpers.HTML().Heading5("High Level Site Objectives");
                }
                var TabObjectives = TabPaneBody2.Helpers.DivC("");
                {
                  TabObjectives.Helpers.HTML().Heading3("List primary high level objectives (values) as listed in the management plan, which underline the purpose of the protection of the site.");
                  var ObjectivesTable = TabObjectives.Helpers.TableFor<METTLib.ProtectedArea.ProtectedAreaHighLevelObjective>(c => c.EditingProtectedArea.ProtectedAreaHighLevelObjectiveList, ViewModel.PageNewObjectivesProtectedAreaSiteBtnInd, ViewModel.PageRemoveObjectivesProtectedAreaSiteBtnInd);
                  {
                    var Firstrow = ObjectivesTable.FirstRow;
                    {
                      var Name = Firstrow.AddColumn(c => c.HighLevelObjective);

                    }
                  }
                }
                var SaveObjectiveRow = TabPaneBody2.Helpers.DivC("row");
                {
                  var SaveObjectiveRowCol = SaveObjectiveRow.Helpers.DivC("col-md-12 pad-top-15");
                  SaveObjectiveRow.Style.FloatRight();

                  var SaveObjectiveBtn = SaveObjectiveRowCol.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                  {
                    SaveObjectiveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveObjectives()");
                    SaveObjectiveBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => (c.EditingProtectedArea != null && !c.EditingProtectedArea.IsNew) && ViewModel.PageSaveObjectivesProtectedAreaSiteBtnInd == true);
                    SaveObjectiveBtn.AddClass("btn-primary btn");
                  }
                }
              }

              #endregion

              #region Biomes

              var TabPane3 = TabItemsContents.Helpers.DivC("tab-pane");
              TabPane3.AddBinding(Singular.Web.KnockoutBindingString.id, c => "tab-Biomes");

              var TabPaneBody3 = TabPane3.Helpers.DivC("panel-body panelRaiseShadow");
              {
                var TabPaneTitle = TabPaneBody3.Helpers.DivC("ibox-title ibox-titleContainer");
                {
                  TabPaneTitle.Helpers.HTML("<i class='fa fa-paw fa-lg fa-fw pull-left'></i>");
                  TabPaneTitle.Helpers.HTML().Heading5("National Biomes");
                }

                var TabBiomes = TabPaneBody3.Helpers.DivC("");
                {
                  var BiomesTable = TabBiomes.Helpers.TableFor<METTLib.ProtectedArea.ProtectedAreaNationalBiome>(c => c.EditingProtectedArea.ProtectedAreaNationalBiomeList, true, ViewModel.PageRemoveNationalBiomesProtectedAreaSiteBtnInd);
                  BiomesTable.AddClass("table-responsive table table-striped table-bordered table-hover");
                  BiomesTable.AddNewButtonLocation = Singular.Web.Controls.Controls.TableAddButtonLocationType.InHeader;
                  BiomesTable.FirstRow.AddColumn(c => c.NationalBiomeID, 380);
                }
              }
              #endregion

              #region Designations
              var TabPane4 = TabItemsContents.Helpers.DivC("tab-pane");
              TabPane4.AddBinding(Singular.Web.KnockoutBindingString.id, c => "tab-Designations");

              var TabPaneBody4 = TabPane4.Helpers.DivC("panel-body panelRaiseShadow");
              {
                var TabPaneTitle = TabPaneBody4.Helpers.DivC("ibox-title ibox-titleContainer");
                {
                  TabPaneTitle.Helpers.HTML("<i class='fa fa-paw fa-lg fa-fw pull-left'></i>");
                  TabPaneTitle.Helpers.HTML().Heading5("Other Designations");
                }

                var TabDesignations = TabPaneBody4.Helpers.DivC("");
                {
                  var DesignationsTable = TabDesignations.Helpers.TableFor<METTLib.ProtectedArea.ProtectedAreaDesignation>(c => c.EditingProtectedArea.ProtectedAreaDesignationList, true, true);
                  DesignationsTable.AddClass("table-responsive table table-striped table-bordered table-hover");
                  DesignationsTable.AddNewButtonLocation = Singular.Web.Controls.Controls.TableAddButtonLocationType.InHeader;
                  DesignationsTable.FirstRow.AddColumn(c => c.DesignationID, 380);
                }
              }
              #endregion
            }
          }
          #endregion
          var SpacerRowEnd = gridDivMain.Helpers.DivC("row pad-top-20");
          {
          }
          var ButtonsRow = gridDivMain.Helpers.DivC("row pad-top-20 m-n");
          {
            var BackBtn = ButtonsRow.Helpers.Button("Back");
            {
              BackBtn.AddClass("btn btn-default pull-left marginBottomLeftButton ");
              BackBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.IsViewingProtectedAreaInd == true);
              BackBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "Back($data)");
            }
            var SaveProtectedAreaBtn = ButtonsRow.Helpers.Button("Save");
            {
              SaveProtectedAreaBtn.AddClass("btn btn-primary pull-right marginBottomRightButton ");
              SaveProtectedAreaBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => (ViewModel.IsViewingProtectedAreaInd == true) && (ViewModel.PageSaveProtectedAreaSiteBtnInd == true));
              SaveProtectedAreaBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveProtectedArea($data)");
            }
          }

          var EndSpacerRow = gridDivMain.Helpers.DivC("row margin0");
          {
            var SpacerCols = EndSpacerRow.Helpers.DivC("col-md-12");
            {
              SpacerCols.Helpers.HTML("<br><br>");
            }
          }

        }
      }
    }

  %>

  <script type="text/javascript">
    Singular.OnPageLoad(function () {
      $("#menuItem2").addClass("active");
      METTHelpers.horizontalTabControl("userTabs");
      let urlParam = METTHelpers.getUrlParameter('ProtectedAreaID')
      if (urlParam != null) {
        ViewModel.CallServerMethod('DecodeUrlParam', { urlParam: urlParam }, function (result) {
          if (result.Success) {
            ViewProtectedArea(null, result.Data);
          }
          else {
            METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
          }
        });
      }
    })

    // Import assessment against protected area exported
    var ImportAssessment = function (obj) {
      ViewModel.CallServerMethod('Import', { ProtectedAreaID: ViewModel.EditingProtectedArea().ProtectedAreaID(), ShowLoadingBar: true }, function (result) {
        if (result.Success) {
          window.location = result.Data;
        }
        else {
          METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      })
    }

    // Export published questionnaire template for offline assessment
    var ExportAssessment = function (obj) {
      ViewModel.CallServerMethod('Export', { QuestionnaireAnswerExportSetList: ViewModel.QuestionnaireAnswerExportSetList.Serialise(), ProtectedAreaID: ViewModel.EditingProtectedArea().ProtectedAreaID(), OrganisationID: ViewModel.EditingProtectedArea().OrganisationID(), LegalDesignationID: ViewModel.EditingProtectedArea().LegalDesignationID(), ShowLoadingBar: true }, function (result) {
        if (result.Success) {
          Singular.DownloadFile(null, result.Data);
        }
        else {
          METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      })
    }

    var ViewProtectedArea = function (obj, urlParam) {
      if (obj == null) {
        protectedAreaID = urlParam;
      }
      else {
        protectedAreaID = obj.ProtectedAreaID();
      }
      if (protectedAreaID == null || protectedAreaID == 0) {
        ViewModel.EditingProtectedArea.Set();
        ViewModel.IsViewingProtectedAreaInd(true);
        $("#userTabsUsers").addClass("active");

        let urlParam = METTHelpers.getUrlParameter('OrganisationID')
        if (urlParam != null) {
          ViewModel.CallServerMethod('SetOrganisation', { ProtectedArea: ViewModel.EditingProtectedArea.Serialise(), urlParam: urlParam }, function (result) {
            if (result.Success) {
              ViewModel.EditingProtectedArea.Set(result.Data);
            }
            else {
              METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
            }
          });
        }
      }
      else {
        ProtectedAreaObject.CallServerMethod('GetProtectedArea', { ProtectedAreaID: protectedAreaID, ShowLoadingBar: true }, function (result) {
          if (result.Success) {
            $("#userTabsUsers").addClass("active");
            ViewModel.ROOrganisationProtectedAreaUserList.Set(result.Data.Item2);
            ViewModel.EditingProtectedArea.Set(result.Data.Item1);
            ViewModel.OrganisationProtectedAreaID(result.Data.Item3);

            // Mark selected national biomes
            for (var i = 0; i < ViewModel.NationalBiomes().length; i++) {
              for (var j = 0; j < ViewModel.EditingProtectedArea().ProtectedAreaNationalBiomeList().length; j++) {
                if (ViewModel.NationalBiomes()[i].NationalBiomeID() == ViewModel.EditingProtectedArea().ProtectedAreaNationalBiomeList()[j].NationalBiomeID() && ViewModel.EditingProtectedArea().ProtectedAreaNationalBiomeList()[j].IsActiveInd()) {
                  ViewModel.NationalBiomes()[i].IncludedInd(true);
                }
              }
            }

            ViewModel.ROProtectedAreaAssessmentPagedListCriteria().ProtectedAreaID(result.Data.Item1.ProtectedAreaID);
            ViewModel.ROProtectedAreaAssessmentPagedListManager().Refresh();

            ViewModel.IsViewingProtectedAreaInd(true);

          } else {
            METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
          }
        });
      }
    }

    var EditProtectedAreaUser = function (obj) {
      Singular.ShowLoadingBar;
      ViewModel.CallServerMethod('EditProtectedAreaUser', { UserID: obj.OrganisationProtectedAreaUserID(), ProtectedAreaID: ViewModel.EditingProtectedArea().ProtectedAreaID(), ShowLoadingBar: true }, function (result) {
        if (result.Success) {
          window.location = result.Data;
        }
        else {
          METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      });
    }

    var RemoveProtectedAreaUser = function (obj) {
      Singular.ShowLoadingBar;
      if (obj != null) {
        METTHelpers.QuestionDialogYesNo("Are you sure you would like to remove this user from this protected area / site?", 'center',
          function () { // Yes
            ViewModel.CallServerMethod('RemoveProtectedAreaUser', { OrganisationProtectedAreaUserID: obj.OrganisationProtectedAreaUserID(), UserID: obj.UserID(), ProtectedAreaID: ViewModel.EditingProtectedArea().ProtectedAreaID(), ShowLoadingBar: true }, function (result) {
              if (result.Success) {
                ViewModel.ROOrganisationProtectedAreaUserList.Set(result.Data);
                METTHelpers.Notification("User has been removed", 'center', 'success', 5000);
              }
              else {

                METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
              }
            });
          },
          function () { // No
          })
      }
    }

    var SaveProtectedArea = function (obj) {
      ProtectedAreaObject.CallServerMethod('SaveProtectedArea', { ProtectedArea: obj.EditingProtectedArea.Serialise(), ShowLoadingBar: true }, function (result) {
        if (result.Success) {
          METTHelpers.Notification("Protected Area / Site Successfully Saved", 'center', 'success', 5000);
          ViewModel.EditingProtectedArea.Set(result.Data.Item1);
          ViewModel.OrganisationProtectedAreaID(result.Data.Item2);
          ClearAssessmentFilters();
        }
        else {
          METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      });
    }

    var ClearFilters = function () {
      ViewModel.ROProtectedAreaPagedListCriteria().METTReportingName(null);
      ViewModel.ROProtectedAreaPagedListCriteria().OfficialName(null);
      ViewModel.ROProtectedAreaPagedListCriteria().RelatedOrganisationID(null);
      ViewModel.ROProtectedAreaPagedListCriteria().RegionID(null);
      ViewModel.ROProtectedAreaPagedListCriteria().ProvinceID(null);
      ViewModel.ROProtectedAreaPagedListManager().Refresh();
    }

    var ClearAssessmentFilters = function () {
      ViewModel.ROProtectedAreaAssessmentPagedListCriteria().AssessmentStepID(null);
      ViewModel.ROProtectedAreaAssessmentPagedListCriteria().AssessmentDate(null);
      ViewModel.ROProtectedAreaAssessmentPagedListManager().Refresh();
    }

    var AddUser = function (obj) {
      Singular.ShowLoadingBar;
    }

    var EditUser = function (ROUser) {
      if (ROUser) {
        // Edit
        ViewModel.CallServerMethod('GetUser', { UserID: ROUser.UserID(), OrganisationProtectedAreaID: ViewModel.OrganisationProtectedAreaID(), ProtectedAreaID: ViewModel.EditingProtectedArea().ProtectedAreaID(), ShowLoadingBar: true }, function (result) {
          if (result.Success) {
            ViewModel.EditingUser.Set(result.Data.Item1);
            window.location = result.Data.Item2;
          }
        });
      } else {
        // New
        ViewModel.EditingUser.Set();
        ViewModel.EditingUser().Password('12345678'); // This will be set to random on server side
      }
    }

    var SaveUser = function () {
      if (ViewModel.EditingUser() != null) {
        ViewModel.EditingUser().LoginName(ViewModel.EditingUser().EmailAddress());
        var jsonUser = ViewModel.EditingUser.Serialise();
        ViewModel.CallServerMethod('SaveUser', { User: jsonUser, ProtectedAreaID: ViewModel.EditingProtectedArea().ProtectedAreaID(), SecurityGroupProtectedAreaUserList: ViewModel.SecurityGroupProtectedAreaUserList.Serialise(), ShowLoadingBar: true }, function (result) {
          if (result.Success) {
            ViewModel.ROOrganisationProtectedAreaUserList(result.Data.Item1);
            ViewModel.SecurityGroupProtectedAreaUserList(result.Data.Item2);
            METTHelpers.Notification("Protected Area / Site User Successfully Saved", 'center', 'success', 5000);
            $('#modalNewUserAdd').modal('hide');
          }
          else {
            METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
          }
        });
      }
    }

    var ViewAssessment = function (obj) {
      Singular.ShowLoadingBar;
      ViewModel.CallServerMethod('ViewAssessment', { QuestionnaireAnswerSetId: obj.QuestionnaireAnswerSetID(), AssessmentStepId: obj.AssessmentStepID(), ProtectedAreaID: obj.ProtectedAreaID(), ShowLoadingBar: true }, function (result) {
        if (result.Success) {
          window.location = result.Data;
        }
        else {
          METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      });
    }

    var CreateNewAssessment = function () {
      METTHelpers.QuestionDialogYesNo("Are all the details for this protected area / site correct and up to date?", 'center',
        function () {
          // allow user to select the year they wish to create the assessment for
          METTHelpers.QuestionDialogAssessmentYear("Please select the year under review, ie; the year that this assessment is related to. Note that this can be different to the year you complete the assessment.", 'center',
            function () {// previous year
              CreateNewOrCopyOfAssessment(ViewModel.PreviousYear())
            },
            function () { // current year
              CreateNewOrCopyOfAssessment(ViewModel.CurrentYear())
            })

        },
        function () { }
      )
    }

    function CreateNewOrCopyOfAssessment(assessmentYear) {
      METTHelpers.QuestionDialogAssessment("Do you want to start a blank assessment or copy of a previous assessment (if available)?", 'center',
        function () { // Blank Assessment
          Singular.ShowLoadingBar;
          ViewModel.CallServerMethod('CreateNewAssessment', { ProtectedArea: ViewModel.EditingProtectedArea.Serialise(), AssessmentYear: assessmentYear, ShowLoadingBar: true }, function (result) {
            if (result.Success) {
              window.location = result.Data;
            }
            else {
              METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
            }
          });
        },
        function () { // Copy Assessment
          Singular.ShowLoadingBar;

          ViewModel.CallServerMethod('CreateDuplicateAssessment', { ProtectedArea: ViewModel.EditingProtectedArea.Serialise(), CreatedBy: ViewModel.EditingProtectedArea.Serialise(), AssessmentYear: assessmentYear, ShowLoadingBar: true }, function (result) {
            if (result.Success) {
              window.location = result.Data;
            }
            else {
              METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
            }
          });
        });
    }

    var AddProtectedAreaNationalBiome = function (obj) {
      // Check if it already exists in the list
      ViewModel.CallServerMethod('AddProtectedAreaNationalBiome', { ProtectedArea: ViewModel.EditingProtectedArea.Serialise(), NationalBiomeID: obj.NationalBiomeID() }, function (result) {
        if (result.Success) {
          ViewModel.EditingProtectedArea.Set(result.Data);
          // Mark selected national biomes
          for (var i = 0; i < ViewModel.NationalBiomes().length; i++) {
            for (var j = 0; j < ViewModel.EditingProtectedArea().ProtectedAreaNationalBiomeList().length; j++) {
              if (ViewModel.NationalBiomes()[i].NationalBiomeID() == ViewModel.EditingProtectedArea().ProtectedAreaNationalBiomeList()[j].NationalBiomeID() && ViewModel.EditingProtectedArea().ProtectedAreaNationalBiomeList()[j].IsActiveInd()) {
                ViewModel.NationalBiomes()[i].IncludedInd(true);
              }
            }
          }
        }
        else {
          METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      });
    }

    var RemoveProtectedAreaNationalBiome = function (obj) {
      // ViewModel.EditingProtectedArea().ProtectedAreaNationalBiomeList().Remove(obj);
      ViewModel.CallServerMethod('RemoveProtectedAreaNationalBiome', { ProtectedArea: ViewModel.EditingProtectedArea.Serialise(), NationalBiomeID: obj.NationalBiomeID() }, function (result) {
        if (result.Success) {
          ViewModel.EditingProtectedArea.Set(result.Data);
          // ViewModel.NationalBiomes.Set(result.Data.Item2);
          // Mark selected national biomes
          for (var i = 0; i < ViewModel.NationalBiomes().length; i++) {
            for (var j = 0; j < ViewModel.EditingProtectedArea().ProtectedAreaNationalBiomeList().length; j++) {
              if (ViewModel.NationalBiomes()[i].NationalBiomeID() == result.Data.ProtectedAreaNationalBiomeList[j].NationalBiomeID && result.Data.ProtectedAreaNationalBiomeList[j].IsActiveInd) {
                ViewModel.NationalBiomes()[i].IncludedInd(true);
              }
              else {
                ViewModel.NationalBiomes()[i].IncludedInd(false);
              }
            }
          }
        }
        else {
          METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
        }
      });
    }

    var Back = function (obj) {
      // Need to check if it came from an organisation or not
      let urlParam = METTHelpers.getUrlParameter('OrganisationID');
      let urlAssessmentParam = METTHelpers.getUrlParameter('QuestionnaireAnswerSetID');
      if (ViewModel.EditingProtectedArea().IsDirty()) {
        METTHelpers.QuestionDialogYesNo("You have not saved your changes. Are you sure you would like to exit the screen?", 'center',
          function () { // Yes
            if (urlParam == null && urlAssessmentParam == null) {
              window.location = "ProtectedAreaProfile.aspx";
            }
            if (urlParam != null) {
              window.location = "../Organisation/OrganisationProfile.aspx?OrganisationID=" + urlParam;
            }
            if (urlAssessmentParam != null) {
              window.location = "../Survey/Survey.aspx?QuestionnaireAnswerSetID=" + urlAssessmentParam;
            }
          },
          function () { // No 
          })
      }
      else {
        if (urlParam == null && urlAssessmentParam == null) {
          window.location = "ProtectedAreaProfile.aspx";
        }
        if (urlParam != null) {
          window.location = "../Organisation/OrganisationProfile.aspx?OrganisationID=" + urlParam;
        }
        if (urlAssessmentParam != null) {
          window.location = "../Survey/Survey.aspx?QuestionnaireAnswerSetID=" + urlAssessmentParam;
        }
      }
    }

    var SaveProtectedAreaAttributes = function () {

      if (ViewModel.EditingProtectedArea().ProtectedAreaPrimaryAttributeList() != null) {
        ViewModel.CallServerMethod('SaveProtectedAreaAttributes', { ProtectedArea: ViewModel.EditingProtectedArea.Serialise() }, function (result) {
          if (result.Success) {
            ViewModel.EditingProtectedArea.Set(result.Data);
            METTHelpers.Notification("Primary Attributes Successfully Saved", 'center', 'success', 5000);
          }
          else {
            METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
          }
        })
      }
    }

    var SaveObjectives = function () {

      if (ViewModel.EditingProtectedArea().ProtectedAreaHighLevelObjectiveList() != null) {
        ViewModel.CallServerMethod('SaveObjectives', { ProtectedArea: ViewModel.EditingProtectedArea.Serialise() }, function (result) {
          if (result.Success) {
            ViewModel.EditingProtectedArea.Set(result.Data);
            METTHelpers.Notification("Highlevel Objectives Successfully Saved", 'center', 'success', 5000);
          }
          else {
            METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
          }
        })
      }
    }

    var ViewOrganisation = function () {
      if (ViewModel.EditingProtectedArea() != null && ViewModel.EditingProtectedArea().OrganisationID() != null) {
        ViewModel.CallServerMethod('ViewOrganisation', { OrganisationID: ViewModel.EditingProtectedArea().OrganisationID(), ProtectedAreaID: ViewModel.EditingProtectedArea().ProtectedAreaID() }, function (result) {
          if (result.Success) {
            window.location = result.Data;
          }
          else {
            METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
          }
        })
      }
    }

    var ClearUserFilters = function () {
      ViewModel.UserCriteria().FirstName(null);
      ViewModel.UserCriteria().LastName(null);
      ViewModel.UserCriteria().EmailAddress(null);
      ViewModel.UserListPageManager().Refresh();
    }
  </script>
</asp:Content>
