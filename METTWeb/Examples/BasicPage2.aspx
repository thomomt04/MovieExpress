<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BasicPage2.aspx.cs" Inherits="MEWeb.Examples.BasicPage2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!--Add page specific styles and javascript classes below-->
    <link href="../Theme/Singular/Custom/home.css" rel="Stylesheet" />
    <link href="../Theme/Singular/Custom/customstyles.css" rel="Stylesheet" />  
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
<!--placeholder not used-->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <%
            //code goes here
            using (var h = this.Helpers)
            {
                var MainContent = h.DivC("row");
                {
                    var MainContainer = MainContent.Helpers.DivC("col-md-12");
                    {
                        var PageContainer = MainContainer.Helpers.DivC("tabs-container");
                        {
                            var PageTab = PageContainer.Helpers.TabControl();
                            {
                                PageTab.Style.ClearBoth();
                                PageTab.AddClass("nav nav-tabs");
                                var TabHeading = PageTab.AddTab("Basic Page 2");
                                {
                                    var MainRow = TabHeading.Helpers.DivC("row margin 0");
                                    {
                                        var MainCol = MainRow.Helpers.DivC("col-md-12");
                                        {
                                            MainCol.Helpers.HTML().Heading2("Bootstrap");
                                            MainCol.Helpers.HTML("The Bootstrap Grid system is a powerful..");
                                        }
                                        var MainColRight = MainRow.Helpers.DivC("col-md-6");
                                        {
                                            // MainCol.Helpers.HTMLTag("p") = "This is a test";
                                            //Add a button
                                            var MyFirstButton = MainColRight.Helpers.Button("Take me somewhere", Singular.Web.ButtonMainStyle.NoStyle, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.caretUp);
                                            {
                                                MyFirstButton.AddClass("btn btn-primary btn-outline");
                                                MyFirstButton.AddBinding(Singular.Web.KnockoutBindingString.click, "TakeMeAway()");
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

    <script type=""text/javascript>
        //place page specific javascript here
        Singular.OnPageLoad(function () {
            $("#menuItem2").addClass('active');
            $("#menuItem2 > ul").addClass('in');
        });
        var TakeMeAway = function () {
            window.open('/http://getbootstrap.com');
           // alert('Where to?');
        }
    </script>
</asp:Content>
