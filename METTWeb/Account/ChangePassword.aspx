<%@ Page Title="Change Password" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
	CodeBehind="ChangePassword.aspx.cs" Inherits="MEWeb.Account.ChangePassword" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
	<link href="../Theme/Singular/METTCustomCss/home.css" rel="stylesheet" />
	<link href="../Theme/Singular/METTCustomCss/assessment.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

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
							var HomeContainerTab = AssessmentsTab.AddTab("Change Password");
							{
								var StatisticsRow = HomeContainerTab.Helpers.DivC("row");
								{
									var CardColumnOne = StatisticsRow.Helpers.DivC("col-md-12");
									{
										CardColumnOne.Helpers.HTML().Heading2("Change Password");
										CardColumnOne.Helpers.HTML("Please complete the required fields below to change your old password to a new password.");

										h.MessageHolder();

										var EditPasswordDiv = CardColumnOne.Helpers.DivC("row");
										{
											{
												var PasswordContent = EditPasswordDiv.Helpers.With<ChangePasswordVM.ChangeDetails>((c) => c.Details);
												{
													var OldPassword = PasswordContent.Helpers.DivC("col-md-4 paddingTop24");
													{
														OldPassword.Helpers.HTML("<p>Old Password</p>");
														OldPassword.Helpers.EditorFor(c => c.OldPassword);
													}
													var NewPassword = PasswordContent.Helpers.DivC("col-md-4 paddingTop24");
													{
														NewPassword.Helpers.HTML("<p>New Password</p>");
														NewPassword.Helpers.EditorFor(c => c.NewPassword);
													}
													var NewPasswordConfirm = PasswordContent.Helpers.DivC("col-md-4 paddingTop24");
													{
														NewPasswordConfirm.Helpers.HTML("<p>Confirm New Password</p>");
														NewPasswordConfirm.Helpers.EditorFor(c => c.ConfirmPassword);
													}
													var OkButton = PasswordContent.Helpers.DivC("col-md-12 paddingTop24");
													{
														var button = OkButton.Helpers.Button("Ok", Singular.Web.ButtonMainStyle.Success, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.tick);
														{
															button.AddBinding(Singular.Web.KnockoutBindingString.click, "ChangePassword()");
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
	%>

	<script type="text/javascript">

		function ChangePassword() {
			Singular.Validation.IfValid(ViewModel.Details(), function () {
				ViewModel.CallServerMethod('ChangePassword', { Details: ViewModel.Details.Serialise() }, function (data) {
					Singular.AddMessage(data.Item1 - 1, 'Change Password', data.Item2).Fade(2000);
					if (data.Item1 == 4) {
					window.location = "Home.aspx";
					}
					
				}, true);
			});
		}
	</script>

</asp:Content>
