<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ParentOrganisations.aspx.cs" Inherits="METTWeb.Maintenance.ParentOrganisations" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
		<%	
			using (var h = this.Helpers)
			{
				var MainHDiv = h.DivC("row");
				{
					var gridDivMain = MainHDiv.Helpers.DivC("col-lg-12 paddingLeftRight0");
					{
						var cardDiv = gridDivMain.Helpers.DivC("ibox float-e-margins");
						{
							var cardTitleDiv = cardDiv.Helpers.DivC("ibox-title");
							{
								cardTitleDiv.Helpers.HTML("<i class='fa fa-user-plus fa-lg fa-fw pull-left'></i>");
								cardTitleDiv.Helpers.HTML().Heading5("Parent Organisation Setup");
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
							var cardContentDiv = cardDiv.Helpers.DivC("ibox-content");
							{

								var grid = cardContentDiv.Helpers.TableFor<METTLib.Organisation.Organisation>(c => c.OrganisationList, false, false);
								{
									grid.AddClass("table-responsive table table-striped table-bordered table-hover");
									grid.Style.Margin("10px 0 15px 0;");

									var firstGridRow = grid.FirstRow;
									{
										firstGridRow.AddClass("table-responsive table table-bordered");
										firstGridRow.AddReadOnlyColumn(c => c.OrganisationName);
										firstGridRow.AddColumn(c => c.ParentInd, 150);
									}
								}

								var SaveRow = cardContentDiv.Helpers.DivC("row");
								{
									//save button
									var SaveCol = SaveRow.Helpers.DivC("col-md-12");
									{
										var SaveBtn = SaveCol.Helpers.Button("Save", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
										{
											SaveBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveOrganisations($data)");
											SaveBtn.AddClass("btn btn-primary");
											SaveBtn.Style.FloatRight();
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
				$("#menuItem5").addClass("active");
				$("#menuItem5 > ul").addClass("in");

				$("#menuItem5ChildItem2").addClass("subActive");

			});

			var SaveOrganisations = function (obj) {
				if (ViewModel.OrganisationList != null)
				{
					ViewModel.CallServerMethod('SaveParentOrganisations', { OrganisationList: ViewModel.OrganisationList.Serialise(), ShowLoadingBar: true }, function (result) {
						if (result.Success)
						{

							METTHelpers.Notification(result.Data, 'center', 'success', 7000);
						}
						else
						{
							METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
						}
					})
				}
			}

		</script>
</asp:Content>
