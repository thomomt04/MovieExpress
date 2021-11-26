<%@ Page Title="METT - Assessment Import" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Import.aspx.cs" Inherits="METTWeb.Survey.Import" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
	<link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
	<link href="../Theme/Inspinia/css/plugins/dropzone/basic.css" rel="stylesheet" />
	<link href="../Theme/Inspinia/css/plugins/dropzone/dropzone.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
			//h.HTML().Heading2("METT Assessment");
		}
	%>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<%	using (var h = this.Helpers)
		{
			var MainHDiv = h.DivC("row");
			{
				var TabContainer = MainHDiv.Helpers.DivC("tabs-container");
				{
					var EntityTab = TabContainer.Helpers.TabControl();
					TabContainer.Attributes.Add("style", "padding-bottom: 50;");
					{
						EntityTab.Style.ClearBoth();
						EntityTab.AddClass("nav nav-tabs");
						var OverviewTab = EntityTab.AddTab("Import");
						{
							var TabRow = OverviewTab.Helpers.DivC("row");
							{
								var Card12Col = TabRow.Helpers.DivC("col-md-12");
								{
									Card12Col.Helpers.HTML("<h2>Import Assessment</h2>");
									Card12Col.Helpers.HTML("<p>You are about to import an offline assessment. Please drag and drop your file in the area below or browse for your MS-Excel file below (Format .xlsx). Your offline assessment will be automatically imported against your protected area.</p>");
								}

								var Card1Col = TabRow.Helpers.DivC("col-md-6");
								{
									var Card1ColContainerBox = Card1Col.Helpers.DivC("ibox float-e-margins paddingBottom");
									{
										var Card1ColContainerContentBox = Card1ColContainerBox.Helpers.DivC("ibox-content");
										{
											var Card1ColContainerRowInner0 = Card1ColContainerContentBox.Helpers.DivC("row");
											{
												var Card1ColContainerRowInnerCol1 = Card1ColContainerRowInner0.Helpers.DivC("col-md-12");
												{
													var ProtectedAreaInfo = Card1ColContainerRowInnerCol1.Helpers.With<METTLib.ProtectedArea.ProtectedArea>(c => c.FirstProtectedArea);
													{
														ProtectedAreaInfo.Helpers.HTML("<h2 style='color:#1ab394'>Reporting Name</h2>");
														ProtectedAreaInfo.Helpers.HTML("<span class='text-center' data-bind=\"text: $data.METTReportingName()\"></span>");
													}
												}
											}
										}
									}
								}
								var Card2Col = TabRow.Helpers.DivC("col-md-6");
								{
									var Card1ColContainerBox = Card2Col.Helpers.DivC("ibox float-e-margins paddingBottom");
									{
										var Card1ColContainerContentBox = Card1ColContainerBox.Helpers.DivC("ibox-content");
										{
											var Card1ColContainerRowInner0 = Card1ColContainerContentBox.Helpers.DivC("row");
											{
												var Card1ColContainerRowInnerCol1 = Card1ColContainerRowInner0.Helpers.DivC("col-md-12");
												{
													var ProtectedAreaInfo2 = Card1ColContainerRowInnerCol1.Helpers.With<METTLib.ProtectedArea.ProtectedArea>(c => c.FirstProtectedArea);
													{
														ProtectedAreaInfo2.Helpers.HTML("<h2 style='color:#1ab394'>Official Name</h2>");
														ProtectedAreaInfo2.Helpers.HTML("<span class='text-center' data-bind=\"text: $data.OfficialName()\"></span>");
													}
												}
											}
										}
									}
								}


								var TabCol = TabRow.Helpers.DivC("col-md-12");
								{
									var documentDragDropRow = TabCol.Helpers.DivC("row");
									{
										documentDragDropRow.Attributes.Add("style", "padding:20px;");
										var documentDragDropDiv = documentDragDropRow.Helpers.DivC("col-md-12");
										{
											documentDragDropDiv.Attributes.Add("id", "FileDropZone");
											documentDragDropDiv.AddClass("dropZone text-center");
											documentDragDropDiv.Style["cursor"] = "pointer";
											documentDragDropDiv.Style["color"] = "#1ab394";
											documentDragDropDiv.AddBinding(Singular.Web.KnockoutBindingString.click, "BrowseFile('xlsx','')");
											var dz1 = documentDragDropDiv.Helpers.DivC("paddingTop40");
											var dz2 = documentDragDropDiv.Helpers.Div();
											var dz3 = dz2.Helpers.Span();

											dz3.Style.VerticalAlign = Singular.Web.VerticalAlign.middle;
											dz3.Style.Display = Singular.Web.Display.inlineblock;
											var dz4 = dz3.Helpers.Span("Drag and drop your file here, or alternatively");
											dz4.Style.FontSize = "18";

											dz4.AddClass("browseStyle");
											//	dz4.Style.Padding(bottom: "15px");
											var dz5 = dz2.Helpers.LinkFor(LinkTextString: "browse for a file to import.");
											dz5.Style.FontSize = "18";
											dz5.Attributes.Add("style", "font-size: 18px; display: inline-block;vertical-align: middle;");
										}
									}
								}
								var ButtonsRow = TabRow.Helpers.DivC("col-md-12");
								{
									var BackBtn = ButtonsRow.Helpers.Button("Back");
									{
										BackBtn.AddClass("btn btn-primary pull-left marginBottomLeftButton ");
										BackBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "Back($data)");
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
			Singular.ShowLoadingBar();
			$("#menuItem3").addClass("active");
			$("#menuItem3 > ul").addClass("in");
			Singular.HideLoadingBar();

			var progress = ko.observable(0)
			var container = $('#FileDropZone')[0];
			var dragcounter = 0;
			if (container) {
				container.ondragover = function (e) {
					e.preventDefault();
					e.dataTransfer.dropEffect = 'copy';
					return false;
				}
				container.ondrop = function (e) {
					//check for file drop;
					e.preventDefault();
					if (e.dataTransfer && e.dataTransfer.files && e.dataTransfer.files.length) {
						UploadFile(e.dataTransfer.files[0]);
					}
					$(container).removeClass("DragEnter");
					dragcounter = 0;
					console.log("drag drop")
					return false;
				}
				container.ondragenter = function (e) {
					dragcounter++;
					e.preventDefault();
					$(container).removeClass("DragLeave");
					$(container).addClass("DragEnter");
					console.log("drag enter")
					return false;
				}
				container.ondragleave = function (e) {
					e.preventDefault();
					dragcounter--
					if (dragcounter == 0) {
						$(container).removeClass("DragEnter");
						$(container).addClass("DragLeave");
					}

					console.log("drag leave")
					return false;
				}
				container.ondrag = function (e) {
					e.preventDefault();
					$(container).removeClass("DragLeave");
					$(container).addClass("DragEnter");
					console.log("drag")
					return false;
				}
			};
		})

		var ImportingFile = true;

		var Back = function (obj) {

			let urlParam = METTHelpers.getUrlParameter('ProtectedAreaId');
			if (urlParam != null) {
				if (obj != null) {
					ViewModel.CallServerMethod('ViewProtectedArea', { ProtectedAreaID: obj.FirstProtectedArea().ProtectedAreaID() }, function (result) {
						if (result.Success) {
							window.location = result.Data;
						}
						else {
							METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
						}
					})
				}
			}
			else {
				window.location = "../ProtectedArea/ProtectedAreaProfile.aspx"
			}
		}

		function BrowseFile() {
			//Singular.ShowLoadingBar();
			Singular.ShowFileDialog(function (e) {
				UploadFile(e.target.files[0]);
			})
			Singular.HideLoadingBar();
		}

		function UploadFile(file) {
			Singular.ShowLoadingBar();
			Singular.UploadFile(file, ViewModel.TempDoc(), 'xlsx', UploadResult, 'DataImport=True', '')
			Singular.FileProgress(ViewModel.TempDoc())

			Singular.HideLoadingBar();
		}

		function UploadResult(args) {
			if (args.Success) {

				//Show message indicating saved successful
				METTHelpers.Notification("Document " + ViewModel.TempDoc().DocumentName() + " Imported Succesfully", "topCenter")
				ImportingFile = false;
				Singular.HideLoadingBar();

				//Redirect to assessment after successful import
				let urlParamProtectedArea = METTHelpers.getUrlParameter('ProtectedAreaId');
				if (urlParamProtectedArea != null) {
					ViewModel.CallServerMethod('ViewAssessment', { QuestionnaireAnswerSetId: args.Data, AssessmentStepId: '1', ProtectedAreaID: urlParamProtectedArea, ShowLoadingBar: true }, function (result) {
						if (result.Success) {
							window.location = result.Data;
						}
						else {
							METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
						}
					});
				}

			}
			else {
				METTHelpers.ErrorNotificationOk("Error occured :" + args.Data, "topCenter")
				ImportingFile = false;
				Singular.HideLoadingBar();
			}
			Singular.HideLoadingBar();
		}
	</script>

</asp:Content>
