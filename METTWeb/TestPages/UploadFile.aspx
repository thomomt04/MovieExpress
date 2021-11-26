<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadFile.aspx.cs" Inherits="METTWeb.TestPages.UploadFile" MasterPageFile="~/Site.Master" %>

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
									cPanelTitle.Helpers.HTML().Heading5("Upload File Example");
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

												var DocumentPage = h.DivC("row");
												{
													var UploadDiv = DocumentPage.Helpers.DivC("");
													{
														var dropZoneDiv = UploadDiv.Helpers.Div();
														dropZoneDiv.Attributes.Add("id", "FileDropZone");
														dropZoneDiv.AddClass("FileDrop");
														dropZoneDiv.Style["cursor"] = "pointer";
														dropZoneDiv.Style["color"] = "#4a90e2";
														dropZoneDiv.AddBinding(Singular.Web.KnockoutBindingString.click, "BrowseFile()");
														var dz1 = dropZoneDiv.Helpers.Div();
														var dz2 = dropZoneDiv.Helpers.Div();
														var dz3 = dz2.Helpers.Span();
														//dz3.Attributes.Add("background-image", "url(../Images/icons/UploadImage.svg)");
														dz3.Style.Width = "25px";
														dz3.Style.Height = "20px";
														//dz3.Style.BackgroundImage = "url(../Images/icons/UploadImage.svg);";
														dz3.Style.Display = Singular.Web.Display.inlineblock;
														var dz4 = dz2.Helpers.Span("Drop files to upload, or ");
														dz4.Style.FontSize = "24";
														dz4.Style.Padding(bottom: "10px");
														var dz5 = dz2.Helpers.LinkFor(LinkTextString: "browse");
														dz5.Style.FontSize = "24";
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
	%>

	<script type="text/javascript">


		Singular.OnPageLoad(function () {

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
		}, true);

		var ImportingFile = false;

		var BrowseAndUpload = function (Doc, CallBack, FileTypes, QS) {
			var mustHide = true;
			Singular.ShowFileDialog(function (e) {
				mustHide = false;
				Singular.UploadFile(e.target.files[0], Doc, FileTypes, CallBack, QS);
			});
			if (mustHide) { Singular.HideLoadingBar(); }
		}

		function BrowseFile() {
			Singular.ShowLoadingBar();
			var ClientID = ViewModel.EditingClient().ClientID();
			BrowseAndUpload(ViewModel.TempDoc(), UploadResult, 'pdf', 'DocumentType=' + ClientID);
			console.log(Singular.FileProgress(ViewModel.TempDoc()));
		}

		function UploadFile(file) {
			Singular.ShowLoadingBar();
			ImportingFile = true;
			var ClientID = ViewModel.EditingClient().ClientID();
			Singular.UploadFile(file, ViewModel.TempDoc(), 'pdf', UploadResult, 'DocumentType=' + ClientID);
			Singular.FileProgress(ViewModel.TempDoc())
		}

		function UploadResult(args) {
			if (args.Success) {
				//Show message indicating saved successful
				var ClientDoc = new ClientDocumentObject();
				ClientDoc.DocumentFriendlyName(args.DocumentFriendlyName);
				ClientDoc.ClientID(ViewModel.EditingClient().ClientID());
				ClientDoc.DocumentID(args.DocumentID);
				ViewModel.ClientDocumentList.Add(ClientDoc);
				PDSHelpers.Notification("Document successfully uploaded", "center", "success", 5000)
				ImportingFile = false;
				Singular.HideLoadingBar();
			}
			else {
				PDSHelpers.Notification(args.Data, "center", "warning", 1000)
				ImportingFile = false;
				Singular.HideLoadingBar();
			}
		}

	</script>



</asp:Content>
