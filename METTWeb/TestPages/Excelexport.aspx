<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExcelExport.aspx.cs" Inherits="METTWeb.TestPages.ExcelExport" MasterPageFile="~/Site.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

	<link href="../Theme/Singular/css/assessment.css" rel="stylesheet" />
	<style type="text/css">
		/*Breadcrumbs*/
		span.round-tab i:active {
			color: #fff;
		}

		span.round-tab.active {
			background: #1ab394;
			border: 2px solid #ddd;
			color: #fff;
		}

			span.round-tab.active i {
				color: #fff;
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
			var MainRow = h.DivC("row");
			{

				var MainContainer = MainRow.Helpers.DivC(""); //"container"
				{
					var MainContainerRow = MainContainer.Helpers.DivC("row");
					{
						var MainContainerRowCol = MainContainerRow.Helpers.DivC("col-md-12");
						{

							var cPanel = MainContainerRowCol.Helpers.DivC("ibox float-e-margins");
							{
								var cPanelTitle = cPanel.Helpers.DivC("ibox-title");
								{
									cPanelTitle.Helpers.HTML("<i class='fa fa-coffee fa-lg fa-fw pull-left'></i>");
									cPanelTitle.Helpers.HTML().Heading5("Export to Excel Example");
								}
								var cPanelTools = cPanelTitle.Helpers.DivC("ibox-tools");
								{
									var cPanelToolsTag = cPanelTools.Helpers.HTMLTag("a");
									cPanelToolsTag.AddClass("collapse-link");
									{
										var cPanelToolsTagIcon = cPanelToolsTag.Helpers.HTMLTag("i");
										cPanelToolsTagIcon.AddClass("fa fa-chevron-up");
									}
								}
								var cPanelContent = cPanel.Helpers.DivC("ibox-content");
								{
									var cPanelMainContent = cPanelContent.Helpers.Div();
									{
										var MainContainerRowx = cPanelMainContent.Helpers.DivC("row");
										{
											var MainContainerRowColx = MainContainerRowx.Helpers.DivC("col-md-12");
											{

												var downloadBtn = MainContainerRowColx.Helpers.Button("Export to Excel");
												downloadBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "DownloadTemplate()");

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


		var DownloadTemplate = function () {

			//ViewModel.DownloadFile('Export', { ThreatsList: ViewModel.ThreatsList.Serialise(), ShowLoadingBar: true }, function () { });
			//METTLib.Questionnaire.QuestionnaireAnswerExportSetList

			ViewModel.CallServerMethod('Export', { QuestionnaireAnswerExportSetList: ViewModel.QuestionnaireAnswerExportSetList.Serialise(), ShowLoadingBar: true }, function (result) {
	

				if (result.Success) {
 
					Singular.DownloadFile(null, result.Data);
				}
				else {
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			})
		}

	</script>



</asp:Content>
