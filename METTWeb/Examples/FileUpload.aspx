<%@ Page Title="Popcorn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FileUpload.aspx.cs" Inherits="MEWeb.Examples.FileUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

  <script type="text/javascript" src="../Scripts/JSLINQ.js"></script>
  <link href="../Theme/Singular/METTCustomCss/home.css" rel="stylesheet" />
  <link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
  <link href="../Theme/Singular/css/badges.css" rel="stylesheet" />
  <link href="../Theme/Singular/css/assessment.css" rel="stylesheet" />
  <script type="text/javascript" src="../Scripts/accesscheck.js"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

  <%
    using (var h = this.Helpers)
    {

      var MainHDiv = h.DivC("row pad-top-10");
      {
        var PanelContainer = MainHDiv.Helpers.DivC("col-md-12 p-n-lr");
        {
          var HomeContainer = PanelContainer.Helpers.DivC("tabs-container");
          {
            var AssessmentsTab = HomeContainer.Helpers.TabControl();
            {
              AssessmentsTab.Style.ClearBoth();
              AssessmentsTab.AddClass("nav nav-tabs");
              var HomeContainerTab = AssessmentsTab.AddTab("Other Examples");
              {
                var Row = HomeContainerTab.Helpers.DivC("row margin0");
                {
                  var RowCol = Row.Helpers.DivC("col-md-12");
                  {


                     var ColContent = RowCol.Helpers.DivC("content");
                    ColContent.Helpers.HTML().Heading2("File Upload");
                    ColContent.Helpers.HTML("<p>Not required to developed as this has been done before and can be used as is with a few tweaks. Examine the JavaScript and FileUpload.ashx to understand how this works.</p>");

                      



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
				//PDSHelpers.Notification(args.Data, "center", "warning", 1000)
				ImportingFile = false;
				Singular.HideLoadingBar();
			}
		}

	</script>
</asp:Content>
