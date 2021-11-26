<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" 	CodeBehind="Users.aspx.cs" Inherits="MEWeb.Maintenance.Users" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<link href="../Theme/Singular/METTCustomCss/Maintenance/users.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageTitleContent" runat="server" ContentPlaceHolderID="PageTitleContent">
	<%
		using (var h = this.Helpers)
		{
		//	h.HTML().Heading2("Users");
		}
	%>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

	<%using (var h = this.Helpers)
        {

            var rowDiv = h.DivC("row p-h-xs");
            {
                rowDiv.ID = "Step0Modal";

                var gridDiv = rowDiv.Helpers.DivC("col-lg-3 paddingLeftRight0");
                {
                    var gridDiv1 = gridDiv.Helpers.Div();
                    {
                        gridDiv1.Helpers.HTML("<i class='fa fa-user-plus fa-2x fa-fw pull-left'></i>");
                        gridDiv1.Helpers.HTML("<h2 class='HeadingFontStyle'>Manage Users</h2>");
                        gridDiv1.Helpers.HTML("<p>Add new users to the admin system by selecting new user</p>");
                    }


                    if (Singular.Security.Security.HasAccess("Security.Manage Users"))
                    {
                        var newUserButton = gridDiv1.Helpers.Button("New User", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                        newUserButton.AddClass("btn-primary");
                        newUserButton.Style.Margin("10px");
                        newUserButton.AddBinding(Singular.Web.KnockoutBindingString.click, "EditUser()");
                    }

                }

                var gridDivMain = rowDiv.Helpers.DivC("col-lg-9 paddingLeftRight0");
                {
                    var cardDiv = gridDivMain.Helpers.DivC("ibox float-e-margins");
                    {
                        var cardTitleDiv = cardDiv.Helpers.DivC("ibox-title");
                        {
                            cardTitleDiv.Helpers.HTML("<i class='fa fa-user-plus fa-lg fa-fw pull-left'></i>");
                            cardTitleDiv.Helpers.HTML().Heading5("Manage Users");
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

                            var grid = cardContentDiv.Helpers.PagedGridFor<MELib.Security.ROUserPaged>(c => c.UserListPageManager, c => c.UserList, false, false);
                            {
                                grid.AddClass("table-responsive table table-striped table-bordered table-hover");
                                grid.Style.Margin("10px 0 15px 0;");

                                var firstGridRow = grid.FirstRow;
                                {
                                    firstGridRow.AddClass("table-responsive table table-bordered");
                                    firstGridRow.AddReadOnlyColumn(c => c.UserName, 200);
                                    firstGridRow.AddReadOnlyColumn(c => c.FirstName, 150);
                                    firstGridRow.AddReadOnlyColumn(c => c.LastName, 150);

                                    var editColumn = firstGridRow.AddColumn();
                                    editColumn.Style.Width = "150";
                                    editColumn.Style.TextAlign = Singular.Web.TextAlign.center;
                                    if (Singular.Security.Security.HasAccess("Security.Manage Users"))
                                    {
                                        var editButton = editColumn.Helpers.Button("Edit", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                        editButton.AddClass("btn-outline btn-info");
                                        editButton.AddBinding(Singular.Web.KnockoutBindingString.click, "EditUser($data)");
                                    
                                         var ManageColumn = firstGridRow.AddColumn();
                                        ManageColumn.Style.Width = "150";
                                        ManageColumn.Style.TextAlign = Singular.Web.TextAlign.center;
                                        var manageButton = ManageColumn.Helpers.Button("Manage", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                        manageButton.AddClass("btn-outline btn-info");
                                        manageButton.AddBinding(Singular.Web.KnockoutBindingString.click, "ManageUser($data)");

                                        var DeleteColumn = firstGridRow.AddColumn();
                                        DeleteColumn.Style.Width = "150";
                                        DeleteColumn.Style.TextAlign = Singular.Web.TextAlign.center;
                                        var deleteButton = DeleteColumn.Helpers.Button("", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.trash);
                                        deleteButton.AddClass("btn-danger btn ");
                                        deleteButton.AddBinding(Singular.Web.KnockoutBindingString.click, "DeleteUser($data)");
                                        deleteButton.Tooltip = "Delete user";
                                    }

                                    var SecurityColumn = firstGridRow.AddColumn();
                                    SecurityColumn.Style.Width = "150";
                                    SecurityColumn.Style.TextAlign = Singular.Web.TextAlign.center;
                                    if (Singular.Security.Security.HasAccess("Security.Reset Passwords"))
                                    {
                                        var resetButton = SecurityColumn.Helpers.Button("Reset Password", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                                        resetButton.AddClass("btn-outline btn-warning");
                                        resetButton.AddBinding(Singular.Web.KnockoutBindingString.click, "ResetPassword($data)");
                                        resetButton.Tooltip = "Reset Password";
                                    }
                                }
                            }
                        } // panel
                    }

                    var dialog = h.Dialog(
                        c => c.EditingUser != null,
                        c => ((c.EditingUser != null) && (c.EditingUser.IsNew)) ? "New User" : "Edit User",
                        "CancelEdit");
                    {
                        dialog.Style.Width = "600";

                        var dialogContent = dialog.Helpers.With<MELib.Security.User>(c => c.EditingUser);
                        {

                            var loginName = dialogContent.Helpers.DivC("row");
                            {
                                var loginNameLabel = loginName.Helpers.LabelFor(c => c.LoginName);
                                {
                                    loginNameLabel.AddBinding(Singular.Web.KnockoutBindingString.enable, c => c.IsNew);
                                    loginNameLabel.AddBinding(Singular.Web.KnockoutBindingString.visible, c => !c.IsNew);
                                    loginNameLabel.AddClass("control-label");
                                }

                                var loginNameEditor = loginName.Helpers.EditorFor(c => c.LoginName);
                                {
                                    loginNameEditor.AddBinding(Singular.Web.KnockoutBindingString.enable, c => c.IsNew);
                                    loginNameEditor.AddBinding(Singular.Web.KnockoutBindingString.visible, c => !c.IsNew);
                                    loginNameEditor.AddClass("form-control");
                                }
                            }

                            var userName = dialogContent.Helpers.DivC("row");
                            {
                                var userNameLabel = userName.Helpers.LabelFor(c => c.FirstName);
                                {
                                    userNameLabel.AddClass("control-label");
                                }
                                var userNameEditor = userName.Helpers.EditorFor(c => c.FirstName);
                                {
                                    userNameEditor.AddClass("form-control");
                                }
                            }

                            var Surname = dialogContent.Helpers.DivC("row");
                            {
                                var SurnameLabel = Surname.Helpers.LabelFor(c => c.Surname);
                                {
                                    SurnameLabel.AddClass("control-label");
                                }
                                var SurnameEditor = Surname.Helpers.EditorFor(c => c.Surname);
                                {
                                    SurnameEditor.AddClass("form-control");
                                }
                            }

                            //dialogContent.Helpers.EditorRowFor(c => c.Password); // Note: If you want to allow Passwords to be changed here, also follow instructions in the User object.

                            var emailAddress = dialogContent.Helpers.DivC("row m-b");
                            {
                                var emailAddressLabel = Surname.Helpers.LabelFor(c => c.EmailAddress);
                                {
                                    emailAddressLabel.AddClass("control-label");
                                }
                                var emailAddressEditor = Surname.Helpers.EditorFor(c => c.EmailAddress);
                                {
                                    emailAddressEditor.AddClass("form-control");
                                }
                            }

                            var table = dialogContent.Helpers.TableFor<Singular.Security.SecurityGroupUser>(c => c.SecurityGroupUserList, true, true);
                            table.AddClass("table-responsive table table-striped table-bordered table-hover");
                            table.AddNewButtonLocation = Singular.Web.Controls.Controls.TableAddButtonLocationType.InHeader;
                            table.FirstRow.AddColumn(c => c.SecurityGroupID, 380);
                        }

                        dialog.AddConfirmationButtons("Save", "SaveUser()", "Cancel");
                    }
                }
            }
        }// using
	%>

	<script type="text/javascript">


		Singular.OnPageLoad(function () {
			$("#menuItem5").addClass("active");
			$("#menuItem5 > ul").addClass("in");

			$("#menuItem5ChildItem2").addClass("subActive");

        });

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

        var ManageUser = function (ROUser) {
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

		var CancelEdit = function () {
			ViewModel.EditingUser.Clear();
		}

		var SaveUser = function () {

      if (ViewModel.EditingUser() != null) {
        ViewModel.EditingUser().LoginName(ViewModel.EditingUser().EmailAddress());
				var jsonUser = ViewModel.EditingUser.Serialise();

				ViewModel.CallServerMethod('SaveUser', { User: jsonUser, ShowLoadingBar: true }, function (result) {
					if (result.Success) {
						ViewModel.UserListPageManager().Refresh();
						ViewModel.EditingUser.Clear();

            METTHelpers.Notification("User has been saved successfully", 'center', 'success', 5000);
					} else {

            METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				});
			};
		}

		var DeleteUser = function (User) {

			ViewModel.CallServerMethod('CanDeleteUser', { UserID: User.UserID(), ShowLoadingBar: true }, function (result) {
				if (result.Success) {
					if (result.Data == true)
					{
						//user can be deleted, let's just show the standard delete confirmation message
						METTHelpers.QuestionDialogYesNo("Are you sure you want to delete the selected user?", 'center',
							function ()
							{ //yes, delete user
								ViewModel.CallServerMethod('DeleteUser', { UserID: User.UserID(), ShowLoadingBar: true }, function (result) {
									if (result.Success) {
										ViewModel.UserList.remove(User);
										METTHelpers.Notification("User has been deleted successfully.", 'center', 'success', 5000);
									}
									else {
										METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
									}
								});
							},
							function ()
							{
								//no, don't delete user
							})

					}
					else
					{
						//user either linked to PA or Organisation, we need end user confirmation
						METTHelpers.QuestionDialogYesNo("The user will be deleted from all associated organisations and relevant protected areas/sites. Are you sure you want to delete the selected user?", 'center',
							function ()
							{ //yes, delete user
								ViewModel.CallServerMethod('DeleteUser', { UserID: User.UserID(), RemoveAssociations: true, ShowLoadingBar: true }, function (result)
								{
									if (result.Success)
									{
										ViewModel.UserList.remove(User);
										METTHelpers.Notification("User and all associations has been deleted successfully.", 'center', 'success', 5000);
									}
									else
									{
												METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
									}
								});
							},
							function ()
							{
								//no, don't delete user
							})

					}
				}
				else
				{
					METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
				}

			});
		}

		var ResetPassword = function (User) {
			Singular.ShowMessageQuestion('Reset Password', 'Are you sure you want to reset the password for ' + User.UserName() + '?', function () {

				ViewModel.CallServerMethod('ResetPassword', { EmailAddress: User.EmailAddress(), ShowLoadingBar: true }, function (result) {
					if (result.Success) {
            METTHelpers.Notification("Users password has been reset successfully.", 'center', 'success', 5000);
					} else {
            METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
					}
				});

			});
		}

    </script>

</asp:Content>

