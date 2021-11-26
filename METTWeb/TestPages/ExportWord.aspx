<%@ Page Title="Export To Word" Language="C#" AutoEventWireup="true" CodeBehind="ExportWord.aspx.cs" MasterPageFile="~/Site.Master" Inherits="METTWeb.TestPages.ExportWord" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
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
									AvailableThreatsTitleDiv.Helpers.HTML().Heading5("Totals");
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

									var viewBtn = ThreatsDivContentDiv.Helpers.Button("Export To Word", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
									{
										viewBtn.AddClass("btn btn-outline btn-primary");
										viewBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "exportWord()");
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
		Singular.OnPageLoad(function ()
		{

		});

		var exportWord = function(obj) {

			alert('Start');
			ViewModel.CallServerMethod('ExportNow', { QuestionnaireAnswerSetID: 284, ShowLoadingBar: true }, function(result) {
				alert('result');
				if (result.Success)
				{
					alert('Success');
					//Singular.DownloadFile(null, result.Data);
				}
				else {
					alert('Failed');
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}
			});
		}

	</script>

</asp:Content>


