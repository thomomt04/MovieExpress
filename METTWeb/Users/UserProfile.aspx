<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserProfile.aspx.cs" Inherits="METTWeb.Users.UserProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
	<link href="../Theme/Singular/METTCustomCss/Maintenance/userProfile.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
	<%
		using (var h = this.Helpers)
		{
			//	h.HTML().Heading2("User Profile");
		}
	%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
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

						#region "Edit User"

						var cardTitleDiv = cardTableDiv.Helpers.DivC("ibox-title");
						{
							//cardTitleDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => c.Editinguser != null);
							cardTitleDiv.Helpers.HTML("<i class='fa fa-user fa-lg fa-fw pull-left'></i>");
							cardTitleDiv.Helpers.HTML().Heading5("User Details");
						}
						var cardToolsDiv = cardTitleDiv.Helpers.DivC("ibox-tools");
						{
							//cardToolsDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => c.Editinguser != null);
							var aToolsTag = cardToolsDiv.Helpers.HTMLTag("a");
							aToolsTag.AddClass("collapse-link");
							{
								var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
								iToolsTag.AddClass("fa fa-chevron-up");
							}
						}
						var cardContentDiv = cardTableDiv.Helpers.DivC("ibox-content");
						{
							//cardContentDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => c.Editinguser != null);
							var EditingRow = cardContentDiv.Helpers.DivC("row m-n");
							{

								var EditingUserDiv = EditingRow.Helpers.Div();
								{
									var UserContent = EditingUserDiv.Helpers.With<METTLib.Security.User>(c => c.Editinguser);
									{
										var FirstName = UserContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
										{
											FirstName.Helpers.BootstrapEditorRowFor(c => c.FirstName);
										}
										var LastName = UserContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
										{
											LastName.Helpers.BootstrapEditorRowFor(c => c.Surname);
										}
										var EmailAddress = UserContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
										{
											var EmailAddressrow = EmailAddress.Helpers.DivC("form-group");
											{
												var EmailLabel = EmailAddressrow.Helpers.HTMLTag("label", "Email Address / Username");

												var Emailtext = EmailAddressrow.Helpers.DivC("");
												{
													Emailtext.Helpers.EditorFor(c => c.EmailAddress);
												}

											}
										}
										var ContactNumber = UserContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
										{
											ContactNumber.Helpers.BootstrapEditorRowFor(c => c.ContactNumber);
										}
										var JobDescription = UserContent.Helpers.DivC("col-md-4 Paddingleft0 ClearBoth");
										{
											JobDescription.Helpers.BootstrapEditorRowFor(c => c.JobDescription);
										}
									}

								}

							}
						}
						#endregion

					}

					#region "SecurityRoles"
					var ProtectedAreaSecurityRolesDiv = gridDivMain.Helpers.DivC("ibox float-e-margins paddingBottom");
					{
						ProtectedAreaSecurityRolesDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ((ViewModel.Editinguser != null) && (ViewModel.IsProtectedAreaUserInd == true)));

						var SecurityRolesTitleDiv = ProtectedAreaSecurityRolesDiv.Helpers.DivC("ibox-title");
						{
							SecurityRolesTitleDiv.Helpers.HTML("<i class='fa fa-lock fa-lg fa-fw pull-left'></i>");
							SecurityRolesTitleDiv.Helpers.HTML().Heading5("Security Roles");
						}
						var cardToolsDiv = SecurityRolesTitleDiv.Helpers.DivC("ibox-tools");
						{
							var aToolsTag = cardToolsDiv.Helpers.HTMLTag("a");
							aToolsTag.AddClass("collapse-link");
							{
								var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
								iToolsTag.AddClass("fa fa-chevron-up");
							}
						}
						var rolesCardContentDiv = ProtectedAreaSecurityRolesDiv.Helpers.DivC("ibox-content");
						{
							var EditingSecurityRoleRow = rolesCardContentDiv.Helpers.DivC("row m-n");
							{

								var EditSecurityRoleDiv = EditingSecurityRoleRow.Helpers.Div();
								{
									var SecurityTable = EditSecurityRoleDiv.Helpers.TableFor<METTLib.ProtectedArea.SecurityGroupProtectedAreaUser>(c => c.SecurityGroupProtectedAreaUserList, true, true);
									SecurityTable.AddClass("table-responsive table table-striped table-bordered table-hover");
									SecurityTable.AddNewButtonLocation = Singular.Web.Controls.Controls.TableAddButtonLocationType.InHeader;
									SecurityTable.FirstRow.AddColumn(c => c.SecurityGroupID, 380);
								}
							}
						}

					}

					var OrganisationSecurityRolesDiv = gridDivMain.Helpers.DivC("ibox float-e-margins paddingBottom");
					{
						OrganisationSecurityRolesDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ((ViewModel.Editinguser != null) && (ViewModel.IsOrganisationUsersInd == true)));

						var SecurityRolesTitleDiv = OrganisationSecurityRolesDiv.Helpers.DivC("ibox-title");
						{
							SecurityRolesTitleDiv.Helpers.HTML("<i class='fa fa-lock fa-lg fa-fw pull-left'></i>");
							SecurityRolesTitleDiv.Helpers.HTML().Heading5("Security Roles");
						}
						var cardToolsDiv = SecurityRolesTitleDiv.Helpers.DivC("ibox-tools");
						{
							var aToolsTag = cardToolsDiv.Helpers.HTMLTag("a");
							aToolsTag.AddClass("collapse-link");
							{
								var iToolsTag = aToolsTag.Helpers.HTMLTag("i");
								iToolsTag.AddClass("fa fa-chevron-up");
							}
						}
						var rolesCardContentDiv = OrganisationSecurityRolesDiv.Helpers.DivC("ibox-content");
						{
							var EditingSecurityRoleRow = rolesCardContentDiv.Helpers.DivC("row m-n");
							{

								var EditSecurityRoleDiv = EditingSecurityRoleRow.Helpers.Div();
								{
									var SecurityTable = EditSecurityRoleDiv.Helpers.TableFor<METTLib.Organisation.SecurityGroupOrganisationUser>(c => c.SecurityGroupOrganisationUserList, true, true);
									SecurityTable.AddClass("table-responsive table table-striped table-bordered table-hover");
									SecurityTable.AddNewButtonLocation = Singular.Web.Controls.Controls.TableAddButtonLocationType.InHeader;
									SecurityTable.FirstRow.AddColumn(c => c.SecurityGroupID, 380);
								}
							}
						}

					}
					#endregion

					var ButtonsRow = gridDivMain.Helpers.DivC("row m-r-none m-l-none m-b");
					{
						var BackBtn = ButtonsRow.Helpers.Button("Back");
						{
							BackBtn.AddClass("btn-default pull-left marginBottomRightButton ");
							BackBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.Editinguser != null);
							BackBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "Back()");
						}

						var SaveProtectedAreaUserBtn = ButtonsRow.Helpers.Button("Save");
						{
							SaveProtectedAreaUserBtn.AddClass("btn btn-primary pull-right marginBottomRightButton ");
							SaveProtectedAreaUserBtn.AddBinding(Singular.Web.KnockoutBindingString.visible, c => ViewModel.Editinguser != null);
							SaveProtectedAreaUserBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "SaveUser()");
						}
					}
				}
			}

		}

	%>

	<script type="text/javascript">
		Singular.OnPageLoad(function () {

			METTHelpers.horizontalTabControl("userTabs");
			let urlProtectedAreaParam = METTHelpers.getUrlParameter('ProtectedAreaID')
			let urlOrganisationParam = METTHelpers.getUrlParameter('OrganisationID')

			if (urlProtectedAreaParam != null) {
				$("#menuItem1").addClass("active");
			}
			else if (urlOrganisationParam != null) {
				$("#menuItem2").addClass("active");
			}
			else {
				$("#menuItem0").addClass("active");
			}
			$("[id^=EmailAddress]").addClass("form-control");
		})


		//add/update OrganisationProtectedAreaUser and add/update SecurityGroupProtectedAreaUser
		var SaveUser = function (obj) {
			if (ViewModel.Editinguser != null) {

				//save user object
				UserObject.CallServerMethod('SaveUser', { User: ViewModel.Editinguser.Serialise(), ShowLoadingBar: true }, function (result) {
					if (result.Success) {
						ViewModel.Editinguser.Set(result.Data);

						if ((METTHelpers.getUrlParameter('OrganisationProtectedAreaID') != null) || (METTHelpers.getUrlParameter('OrganisationProtectedAreaUserID') != null)) {
							OrganisationProtectedAreaUserObject.CallServerMethod('SaveOrganisationProtectedAreaUser', { OrganisationProtectedAreaUser: ViewModel.OrganisationProtectedAreaUser.Serialise(), SecurityGroupProtectedAreaUserList: ViewModel.SecurityGroupProtectedAreaUserList.Serialise(), ProtectedAreaID: ViewModel.ProtectedAreaID(), ShowLoadingBar: true }, function (result) {
								if (result.Success) {
									ViewModel.OrganisationProtectedAreaUser.Set(result.Data.Item1);
									METTHelpers.Notification("Organisation Protected Area / Site User Successfully Saved", 'center', 'success', 5000);

									//ADDED SILVERFOX 5/4/19
									ViewModel.SecurityGroupProtectedAreaUserList.IsDirty = false;
									ViewModel.CanGoBack(true);
								}
								else {
									METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
								}
							});
						}

						//else if it's organisation then we should save an OrganisationUser
						if ((METTHelpers.getUrlParameter('OrganisationUserID') != null) || (METTHelpers.getUrlParameter('OrganisationID') != null)) {
							OrganisationUserObject.CallServerMethod('SaveOrganisationUser', { OrganisationUser: ViewModel.OrganisationUser.Serialise(), SecurityGroupOrganisationUserList: ViewModel.SecurityGroupOrganisationUserList.Serialise(), ShowLoadingBar: true }, function (result) {
								if (result.Success) {
									ViewModel.OrganisationUser.Set(result.Data.Item1);
									METTHelpers.Notification("Organisation User Successfully Saved", 'center', 'success', 5000);

									//ADDED SILVERFOX 24/4/19
									ViewModel.SecurityGroupOrganisationUserList.IsDirty = false;
									ViewModel.CanGoBack(true);
								}
								else {
									METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
								}
							});
						}
					}
					else {
						METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				});

			}
		}

		var EditUser = function (ROUser) {

			if (ROUser) {
				//Edit
				ViewModel.CallServerMethod('GetUser', { UserID: ROUser.UserID(), ShowLoadingBar: true }, function (result) {
					if (result.Success) {
						ViewModel.EditingUser.Set(result.Data);
					}
				});

			} else {
				//New
				ViewModel.EditingUser.Set();
				ViewModel.EditingUser().Password('12345678');//This will be set to random on server side.
			}

		}

		var Back = function () {

			// Protected Area Page
			// OK ViewModel.SecurityGroupProtectedAreaUserList.IsDirty || ViewModel.SecurityGroupOrganisationUserList.IsDirty()

			//Organisations Page
			// OK ViewModel.SecurityGroupProtectedAreaUserList.IsDirty() || ViewModel.SecurityGroupOrganisationUserList.IsDirty

			//ADDED SILVERFOX 5/4/19
			//if (ViewModel.Editinguser().IsDirty() || ViewModel.SecurityGroupProtectedAreaUserList.IsDirty() || ViewModel.SecurityGroupOrganisationUserList.IsDirty() || ViewModel.OrganisationUser().IsDirty() || ViewModel.OrganisationProtectedAreaUser().IsDirty())
			//if (ViewModel.Editinguser().IsDirty() || ViewModel.SecurityGroupProtectedAreaUserList.IsDirty || ViewModel.SecurityGroupOrganisationUserList.IsDirty() || (ViewModel.OrganisationUser() !== null && ViewModel.OrganisationUser().IsDirty()) || (ViewModel.OrganisationProtectedAreaUser() !== null && ViewModel.OrganisationProtectedAreaUser().IsDirty())) {
			if (ViewModel.CanGoBack() !== true)
			{
				METTHelpers.QuestionDialogYesNo("You have not saved your changes. Are you sure you would like to exit the screen?", 'center',
					function () { //yes exit the screen and go to calling page

						if ((METTHelpers.getUrlParameter('OrganisationProtectedAreaID') != null) || (METTHelpers.getUrlParameter('OrganisationProtectedAreaUserID') != null)) //protected area page
						{
							var url = "../ProtectedArea/ProtectedAreaProfile.aspx?ProtectedAreaID=";
							ViewModel.CallServerMethod('EncodeUrl', { urlParam: ViewModel.ProtectedAreaID(), urlString: url }, function (result) {
								if (result.Success) {
									window.location = result.Data;
								}
								else {
									METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
								}
							})

						}
						else if ((METTHelpers.getUrlParameter('OrganisationUserID') != null) || (METTHelpers.getUrlParameter('OrganisationID') != null)) //organisation page
						{
							var url = "../Organisation/OrganisationProfile.aspx?OrganisationID=";
							ViewModel.CallServerMethod('EncodeUrl', { urlParam: ViewModel.OrganisationID(), urlString: url }, function (result) {
								if (result.Success) {
									window.location = result.Data;
								}
								else {
									METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
								}
							})
						}
						else {
							window.location = "../Account/Home.aspx";
						}
					},
					function () {//no, remain on the screen  
						//Do nothing
					})
			}
			else {
				if ((METTHelpers.getUrlParameter('OrganisationProtectedAreaID') != null) || (METTHelpers.getUrlParameter('OrganisationProtectedAreaUserID') != null)) {
					var url = "../ProtectedArea/ProtectedAreaProfile.aspx?ProtectedAreaID=";
					ViewModel.CallServerMethod('EncodeUrl', { urlParam: ViewModel.ProtectedAreaID(), urlString: url }, function (result) {
						if (result.Success) {
							window.location = result.Data;
						}
						else {
							METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
						}
					})
					//window.location = "../ProtectedArea/ProtectedAreaProfile.aspx?ProtectedAreaID=" + ViewModel.ProtectedAreaID();
				}
				else if ((METTHelpers.getUrlParameter('OrganisationUserID') != null) || (METTHelpers.getUrlParameter('OrganisationID') != null)) {
					// window.location = "../Organisation/OrganisationProfile.aspx?OrganisationID=" + ViewModel.OrganisationID();
					var url = "../Organisation/OrganisationProfile.aspx?OrganisationID=";
					ViewModel.CallServerMethod('EncodeUrl', { urlParam: ViewModel.OrganisationID(), urlString: url }, function (result) {
						if (result.Success) {
							window.location = result.Data;
						}
						else {
							METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
						}
					})
				}
				else {
					window.location = "../Account/Home.aspx";
				}
			}
		}

	</script>
</asp:Content>


