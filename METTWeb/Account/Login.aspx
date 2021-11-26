<%@ Page Title="Log In" Language="C#" MasterPageFile="~/SiteLoggedOut.Master" AutoEventWireup="false" CodeBehind="Login.aspx.cs" Inherits="MEWeb.Account.Login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent1">
  <link href="../Theme/Singular/Custom/loggedout.css" rel="stylesheet" />
  <link href="../Theme/Singular/css/loginandlockscreen.css" rel="stylesheet" />
 <!--<link href="../Theme/Singular/METTCustomCss/Maintenance/users.css" rel="stylesheet" />-->
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent1">
  <%
      using (var h = this.Helpers)
      {
          var toolbar = h.Toolbar();
          {
              toolbar.Helpers.HTML().Heading2("Log In");
              toolbar.AddBinding(Singular.Web.KnockoutBindingString.visible, c => !c.ShowForgotPasswordInd);

              var dialog = h.Dialog(
                        c => c.EditingUser != null,
                        c => ((c.EditingUser != null) && (c.EditingUser.IsNew)) ? "Register New User" : "Edit User",
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

                      var Surname = dialogContent.Helpers.DivC("row m-b");
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

                      var ContactNumber = dialogContent.Helpers.DivC("row m-b");
                      {
                          var ContactNumberLabel = Surname.Helpers.LabelFor(c => c.ContactNumber);
                          {
                              ContactNumberLabel.AddClass("control-label");
                          }
                          var ContactNumberLabelEditor = Surname.Helpers.EditorFor(c => c.ContactNumber);
                          {
                              ContactNumberLabelEditor.AddClass("form-control");
                          }
                      }
                      var Password = dialogContent.Helpers.DivC("row m-b");
                      {
                          var PasswordLabel = Surname.Helpers.LabelFor(c => c.Password);
                          {
                              PasswordLabel.AddClass("control-label");
                          }
                          var PasswordLabelEditor = Surname.Helpers.EditorFor(c => c.Password);
                          {
                              PasswordLabelEditor.AddClass("form-control");
                          }
                      }

                      var ConfirmPassword = dialogContent.Helpers.DivC("row m-b");
                      {
                          var ConfirmPasswordLabel = Surname.Helpers.LabelFor(c => c.Password);
                          {
                              ConfirmPasswordLabel.AddClass("control-label");
                          }
                          var ConfirmPasswordLabelEditor = Surname.Helpers.EditorFor(c => c.Password);
                          {
                              ConfirmPasswordLabelEditor.AddClass("form-control");
                          }
                      }
                  }

                  dialog.AddConfirmationButtons("Register", "SaveUser()", "Cancel");
              }
          }
          h.MessageHolder();
          var LoginDiv = h.Div();
          {
              LoginDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => !c.ShowForgotPasswordInd);

              var fieldSet = LoginDiv.Helpers.FieldSetFor<Singular.Web.Security.LoginDetails>("Account Information", c => c.LoginDetails);
              {
                  fieldSet.AddClass("StackedEditors SUI-RuleBorder");
                  fieldSet.Style["max-width"] = "420px";
                  var EmailAddressrow = fieldSet.Helpers.DivC("row");
                  {
                      var EmailLabel = EmailAddressrow.Helpers.HTMLTag("label", "Email Address");
                      EmailAddressrow.Helpers.EditorFor(c => c.UserName);
                  }
                  fieldSet.Helpers.EditorRowFor(c => c.Password);
                  var row = fieldSet.Helpers.DivC("row");
                  {
                  }
              }
             
              var Loginbutton = LoginDiv.Helpers.Button("Login", "Log In");
              {
                  Loginbutton.AddBinding(Singular.Web.KnockoutBindingString.click, "Login()");
                  Loginbutton.ClickOnEnterKey = true;
              }
              var ForgotPasswordBtn = LoginDiv.Helpers.Button("", "Forgot Password");
              {
                  ForgotPasswordBtn.AddBinding(Singular.Web.KnockoutBindingString.click, "ShowFP()");
              }
              
          }
          var ForgotPasswordDiv = h.Div();
          {
              ForgotPasswordDiv.AddBinding(Singular.Web.KnockoutBindingString.visible, c => c.ShowForgotPasswordInd);
              var FPtoolbar = ForgotPasswordDiv.Helpers.Toolbar();
              {
                  FPtoolbar.Helpers.HTML().Heading2("Forgot Password");
              }
              var FPDetailsDiv = ForgotPasswordDiv.Helpers.DivC("m-t-md");
              FPDetailsDiv.Helpers.BootstrapEditorRowFor(c => c.ForgotEmail);
              var FPButtons = ForgotPasswordDiv.Helpers.DivC("m-t-md");
              {
                  var FPback = FPButtons.Helpers.Button("Back", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                  {
                      FPback.AddBinding(Singular.Web.KnockoutBindingString.click, "ShowFP()");
                      FPback.AddClass("MarginRight1");
                  }
                  var FPReset = FPButtons.Helpers.Button("Reset Password", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.None);
                  {
                      FPReset.AddBinding(Singular.Web.KnockoutBindingString.click, "FPReset()");
                  }
              }



          }
      }
  %>

  <script type="text/javascript">
    function Login() {
      Singular.Validation.IfValid(ViewModel, function () {
        Singular.ShowLoadingBar();
        ViewModel.CallServerMethod('Login', { LoginDetails: ViewModel.LoginDetails.Serialise() }, function (result) {
          if (result.Success) {
            window.location = result.Data ? result.Data : ViewModel.RedirectLocation();
            Singular.HideLoadingBar();
          } else {
            ViewModel.LoginDetails().Password('');
            Singular.AddMessage(1, 'Login', result.ErrorText).Fade(2000);
            Singular.HideLoadingBar();
          }
        });
      });
    }

    var ShowFP = function () {
      ViewModel.ShowForgotPasswordInd(!ViewModel.ShowForgotPasswordInd());
    };

    var FPReset = function () {
      METTHelpers.QuestionDialogYesNo(("Are you sure you would like to reset your password?"), 'center',
        function () {
          Singular.ShowLoadingBar();
          ViewModel.CallServerMethod('ResetPassword', { Email: ViewModel.ForgotEmail() }, function (result) {
            Singular.HideLoadingBar();
            if (result.Success) {
              METTHelpers.Notification('Please check your email for reset instructions', 'center', 'success', 5000);
              ViewModel.ShowForgotPasswordInd(false);
              ViewModel.ForgotEmail("");
            } else {
              ViewModel.ShowForgotPasswordInd(false);
              ViewModel.ForgotEmail("");
            }
          })
        },
        function () {
          ViewModel.ShowForgotPasswordInd(false);
          ViewModel.ForgotEmail("");
        });
      };

      var RegisterUser = function (ROUser) {

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
                  if (result.Data == true) {
                      //user can be deleted, let's just show the standard delete confirmation message
                      METTHelpers.QuestionDialogYesNo("Are you sure you want to delete the selected user?", 'center',
                          function () { //yes, delete user
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
                          function () {
                              //no, don't delete user
                          })

                  }
                  else {
                      //user either linked to PA or Organisation, we need end user confirmation
                      METTHelpers.QuestionDialogYesNo("The user will be deleted from all associated organisations and relevant protected areas/sites. Are you sure you want to delete the selected user?", 'center',
                          function () { //yes, delete user
                              ViewModel.CallServerMethod('DeleteUser', { UserID: User.UserID(), RemoveAssociations: true, ShowLoadingBar: true }, function (result) {
                                  if (result.Success) {
                                      ViewModel.UserList.remove(User);
                                      METTHelpers.Notification("User and all associations has been deleted successfully.", 'center', 'success', 5000);
                                  }
                                  else {
                                      METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
                                  }
                              });
                          },
                          function () {
                              //no, don't delete user
                          })

                  }
              }
              else {
                  METTHelpers.Notification(result.ErrorText, 'center', 'warning', 5000);
              }

          });
      }

  </script>
</asp:Content>
