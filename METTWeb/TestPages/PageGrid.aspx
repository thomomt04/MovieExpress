<%@ Page Title="METT Assessment PageGrid Example" Language="C#" AutoEventWireup="true" CodeBehind="PageGrid.aspx.cs" MasterPageFile="~/Site.Master" Inherits="METTWeb.TestPages.PageGrid" %>

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
								cardTitleDiv.Helpers.HTML().Heading5("Protected Area List");
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
									//Protected Area Paged grid and filter



									var FilterRow = TableRow.Helpers.DivC("row filterFrame");
									{
										var FilterNameCol = FilterRow.Helpers.DivC("col-md-2");
										{
											FilterNameCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.METTReportingName);
											FilterNameCol.AddClass("control-label");

											var FilterNameEditor = FilterNameCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.METTReportingName);
											FilterNameEditor.AddClass("form-control marginBottom20 filterBox");
										}

										var FilterOfficialNameCol = FilterRow.Helpers.DivC("col-md-2");
										{
											FilterOfficialNameCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.OfficialName);
											FilterOfficialNameCol.AddClass("control-label");

											var FilterOfficialNameEditor = FilterOfficialNameCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.OfficialName);
											FilterOfficialNameEditor.AddClass("form-control marginBottom20 filterBox");
										}

										var FilterRelatedOrganisationCol = FilterRow.Helpers.DivC("col-md-2");
										{
											FilterRelatedOrganisationCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.RelatedOrganisationID);
											FilterRelatedOrganisationCol.AddClass("control-label");

											var FilterRelatedOrgEditor = FilterRelatedOrganisationCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.RelatedOrganisationID);
											FilterRelatedOrgEditor.AddClass("form-control marginBottom20 filterBox");
										}

										var FilterRegionCol = FilterRow.Helpers.DivC("col-md-2");
										{
											FilterRegionCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.RegionID);
											FilterRegionCol.AddClass("control-label");

											var FilterRegionEditor = FilterRegionCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.RegionID);
											FilterRegionEditor.AddClass("form-control marginBottom20 filterBox");
										}

										var FilterProvinceCol = FilterRow.Helpers.DivC("col-md-2");
										{
											FilterProvinceCol.Helpers.LabelFor(c => c.ROProtectedAreaPagedListCriteria.ProvinceID);
											FilterProvinceCol.AddClass("control-label");

											var FilterProvinceEditor = FilterProvinceCol.Helpers.EditorFor(c => c.ROProtectedAreaPagedListCriteria.ProvinceID);
											FilterProvinceEditor.AddClass("form-control marginBottom20 filterBox");
										}

										var ButtonsCol = FilterRow.Helpers.DivC("col-md-2 buttonColStyling paddingTop24");
										{

											ButtonsCol.Style.FloatRight();

											//var SearchFilterCol = ButtonsCol.Helpers.Div();// C("col-md-3");
											//{
											//  var SearchBtn = SearchFilterCol.Helpers.Button("Search", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.search);
											//  {
											//    SearchBtn.Style.FloatRight();
											//    SearchBtn.AddClass("btn-primary btn btn btn-primary");
											//    SearchBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "Search()");
											//  }
											//}

											var ClearFilterCol = ButtonsCol.Helpers.Div();
											{
												var ClearBtn = ClearFilterCol.Helpers.Button("Clear", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
												{
													ClearBtn.Style.FloatRight();
													ClearBtn.AddClass("btn-primary btn btn-outline");
													ClearBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ClearFilters()");
												}
											}

											var NewCol = ButtonsCol.Helpers.Div();
											{
												var NewProtectedAreaBtn = NewCol.Helpers.Button("New Protected Area", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
												{
													NewProtectedAreaBtn.Style.FloatRight();
													NewProtectedAreaBtn.AddClass("btn-primary btn btn btn-primary marginLeft2 m-r");
													NewProtectedAreaBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewProtectedArea()");

												}
											}
										}
									}

									var ProtectedAreaDiv = TableRow.Helpers.DivC("row col-lg-12");
									{

										var ProtectedAreaPagedList = ProtectedAreaDiv.Helpers.PagedGridFor<METTLib.ProtectedArea.ROProtectedAreaPaged>(c => c.ROProtectedAreaPagedListManager, c => c.ROProtectedAreaPagedList, false, false);
										{
											ProtectedAreaPagedList.AddClass("table-responsive table table-bordered");

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
													ViewCol.Style.Width = "150px";
													ViewCol.HeaderText = "View";

													var viewBtn = ViewCol.Helpers.Button("View", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
													{
														viewBtn.AddClass("btn btn-outline btn-primary");
														viewBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ViewProtectedArea($data)");
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
	%>


	<script type="text/javascript">
		Singular.OnPageLoad(function () {

		});


	</script>


</asp:Content>
