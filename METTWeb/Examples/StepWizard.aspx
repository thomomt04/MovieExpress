<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StepWizard.aspx.cs" Inherits="MEWeb.Examples.StepWizard" %>

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

                    RowCol.Helpers.HTML().Heading2("Progress Wizard");
                    RowCol.Helpers.HTML("<p>The example below shows how to use [ForEach] to loop through a list of items and display them in a <span> HTML tag.</p>");

                    var ExampleRow = RowCol.Helpers.DivC("div");
                    {
                      var GroupItems = ExampleRow.Helpers.ForEach<MELib.Questionnaire.ROQuestionnaireGroup>(c => c.ROQuestionnaireGroupList);
                      {
                        var Item = GroupItems.Helpers.HTMLTag("span");
                        {
                          Item.Helpers.ReadOnlyFor(c => c.QuestionnaireGroup);
                        }
                      }
                    }

                    RowCol.Helpers.HTML().Heading2("Example");
                    RowCol.Helpers.HTML("<p>To build our progress wizard we can use additional CSS formatting and Javascript. The example below shows how the functionality of [ForEach] can be used to build a Progress Wizard / Bar with the help of JavaScript and CSS.</p>");
                    RowCol.Helpers.HTML("<p>Click on the items below to see the JavsScript being trigger. Use the JavaScript function to call a WebCallable Method in the ViewModel.</p>");
                    var WizardRow = RowCol.Helpers.DivC("row");
                    {
                      var WizardDiv = WizardRow.Helpers.DivC("wizard");
                      {
                        var WizzardInner = WizardDiv.Helpers.DivC("wizard-inner");
                        {
                          WizzardInner.Helpers.DivC("connecting-line");
                          var WizzardUL = WizzardInner.Helpers.HTMLTag("ul");
                          {
                            WizzardUL.AddClass("nav nav-tabs");
                            WizzardUL.Attributes.Add("role", "tablist");
                            WizzardUL.Attributes.Add("style", "border-bottom:none");
                            WizzardUL.Attributes.Add("id", "ul-tab-list");

                            var Groups = WizzardUL.Helpers.ForEach<MELib.Questionnaire.ROQuestionnaireGroup>(c => c.ROQuestionnaireGroupList);
                            {
                              var WizardListItem = Groups.Helpers.HTMLTag("li");
                              {
                                WizardListItem.Attributes.Add("role", "presentation");
                                WizardListItem.Attributes.Add("style", "background:none");
                                var WizardListItemHeader = WizardListItem.Helpers.HTMLTag("h4");
                                {
                                  WizardListItemHeader.AddClass("text-center");
                                  var CurrentGroup = WizardListItemHeader.Helpers.ReadOnlyFor(c => c.QuestionnaireGroup);
                                }
                                var WizardIcon = WizardListItem.Helpers.HTMLTag("a");
                                {
                                  WizardIcon.Attributes.Add("role", "tab");
                                  WizardIcon.AddBinding(Singular.Web.KnockoutBindingString.click, "ShowData($data)");
                                  var WizardIconSpan = WizardIcon.Helpers.SpanC("round-tab showSingle");
                                  {
                                    WizardIconSpan.AddBinding(Singular.Web.KnockoutBindingString.id, c => c.QuestionnaireGroupID);
                                    var WizardIconImage = WizardIconSpan.Helpers.HTMLTag("i");
                                    {
                                      WizardIconImage.AddBinding(Singular.Web.KnockoutBindingString.css, c => c.Icon);
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
          }
        }
      }
    }

  %>

  <script type="text/javascript">

    var ShowData = function (obj) {
      alert('You clicked on item ID [' + obj.QuestionnaireGroupID() + ']');
    }
  </script>

</asp:Content>
