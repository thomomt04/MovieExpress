<%@ Page Title="METT Assessment" Language="C#" AutoEventWireup="true" CodeBehind="Totals.aspx.cs" MasterPageFile="~/Site.Master" Inherits="METTWeb.TestPages.Totals" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<%--	<script src="../Theme/Inspinia/js/jquery-3.1.1.min.js"></script>


	<script src="../Theme/Inspinia/js/plugins/tipr/tipr.min.js"></script>
	<link href="../Theme/Inspinia/css/plugins/tipr/tipr.css" rel="stylesheet" />--%>


	<!-- <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script> -->

	<link rel="stylesheet" type="text/css" media="screen" href="../Theme/tinyTips.css" />
	<script type="text/javascript" src="../Theme/jquery.tinyTips.js"></script>

	<script type="text/javascript">
		$(document).ready(function () {
			$('span.tTip').tinyTips('light', 'title');
		});
	</script>
	<style>


		/* setup tooltips */
.tooltip {
  position: relative;
}
.tooltip:before,
.tooltip:after {
  display: block;
  opacity: 0;
  pointer-events: none;
  position: absolute;
}
.tooltip:after {
	border-right: 6px solid transparent;
	border-bottom: 6px solid rgba(0,0,0,.75); 
  border-left: 6px solid transparent;
  content: '';
  height: 0;
    top: 20px;
    left: 20px;
  width: 0;
}
.tooltip:before {
  background: rgba(0,0,0,.75);
  border-radius: 2px;
  color: #fff;
  content: attr(data-title);
  font-size: 14px;
  padding: 6px 10px;
    top: 26px;
  white-space: nowrap;
}

/* the animations */
/* fade */
.tooltip.fade:after,
.tooltip.fade:before {
  transform: translate3d(0,-10px,0);
  transition: all .15s ease-in-out;
}
.tooltip.fade:hover:after,
.tooltip.fade:hover:before {
  opacity: 1;
  transform: translate3d(0,0,0);
}

/* expand */
.tooltip.expand:before {
  transform: scale3d(.2,.2,1);
  transition: all .2s ease-in-out;
}
.tooltip.expand:after {
  transform: translate3d(0,6px,0);
  transition: all .1s ease-in-out;
}
.tooltip.expand:hover:before,
.tooltip.expand:hover:after {
  opacity: 1;
  transform: scale3d(1,1,1);
}
.tooltip.expand:hover:after {
  transition: all .2s .1s ease-in-out;
}

/* swing */
.tooltip.swing:before,
.tooltip.swing:after {
  transform: translate3d(0,30px,0) rotate3d(0,0,1,60deg);
  transform-origin: 0 0;
  transition: transform .15s ease-in-out, opacity .2s;
}
.tooltip.swing:after {
  transform: translate3d(0,60px,0);
  transition: transform .15s ease-in-out, opacity .2s;
}
.tooltip.swing:hover:before,
.tooltip.swing:hover:after {
  opacity: 1;
  transform: translate3d(0,0,0) rotate3d(1,1,1,0deg);
}

/* basic styling: has nothing to do with tooltips: */
h1 {
  padding-left: 50px;
}
ul {
  margin-bottom: 40px;
}
li {
  cursor: pointer; 
  display: inline-block; 
  padding: 0 10px;
}




.singulartooltip {
    position: absolute;
    display: inline-block;
}

.singulartooltip-icon{
  background: #0026ff;
  color: #fff;
  padding: 5px 5px;
  border-radius: 50%;
  font-size: 10px;
  font-weight: bold;
  cursor: pointer;
}

.singulartooltip .singulartooltiptext {
    visibility: hidden;
    width: 350px;
    background-color: black;
    color: #fff;
    text-align: left;
    border-radius: 6px;
    padding: 10px;

    /* Position the tooltip */
    position: absolute;
    z-index: 1;
    left: 20px;
    top: -40%;
}

.singulartooltip:hover .singulartooltiptext {
    visibility: visible;
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


									var ContentRow = ThreatsDivContentDiv.Helpers.DivC("row");
									{

										var Col1 = ContentRow.Helpers.DivC("col-md-3");
										{
											Col1.Helpers.HTML("test");
											
										}
										var Col2 = ContentRow.Helpers.DivC("col-md-3");
										{

											//var html = Col2.Helpers.HTML("<span class='tTip' href='#' title='<p><b>• Very High:</b> very widespread or pervasive in its scope, and affects the conservation values across all or most(71 - 100 %) of the protected area (site) or the range of the specific value.</p><p><b>• High:</b> widespread in its scope and affects conservation values across much(31 - 70 %) of the protected area (site).</p><p><b>• Medium:</b> localized in its scope, and affects conservation values over some(11 - 30 %) of the protected area (site) or the range of the specific value.</p><p><b>• Low:</b> very localized in its scope, and affects conservation values over a limited portion(1 - 10 %) of the protected area (site) or the range of the specific value.</p>'><p class='fa fa-info-circle infoIconBlue'></p></span>");

											////var someTag = Col2.Helpers.HTMLTag("span");
											////{
											////	someTag.AddClass("tTip");
											////	someTag.Attributes.Add.Title("");

											////}

											//var TooltipICON = Col2.Helpers.DivC("");
											//	{
											//		TooltipICON.Helpers.HTML("<div class='singulartooltip'><p class='tTip fa fa-info-circle infoIconBlue'></p><span class='singulartooltiptext'>");
											//		TooltipICON.Helpers.HTML("<p><b>• Very High:</b> very widespread or pervasive in its scope, and affects the conservation values across all or most(71 - 100 %) of the protected area (site) or the range of the specific value.</p>");
											//		TooltipICON.Helpers.HTML("<p><b>• High:</b> widespread in its scope and affects conservation values across much(31 - 70 %) of the protected area (site).</p>");
											//		TooltipICON.Helpers.HTML("<p><b>• Medium:</b> localized in its scope, and affects conservation values over some(11 - 30 %) of the protected area (site) or the range of the specific value.</p>");
											//		TooltipICON.Helpers.HTML("<p><b>• Low:</b> very localized in its scope, and affects conservation values over a limited portion(1 - 10 %) of the protected area (site) or the range of the specific value.</p>");
											//		TooltipICON.Helpers.HTML("</span</div>");

											//	}
											//Col2.Helpers.HTML("<br><br><br><br>");
										}

										//var Stats = ContentRow.Helpers.With<METTLib.Questionnaire.ROQuestionnaireStatistics>(c => c.FirstQuestionnaireStatistics);
										//{
										//	var WizardListItemHeader = Stats.Helpers.HTMLTag("h4");
										//	{
										//		WizardListItemHeader.AddClass("text-center");
										//		var CurrentGroup = WizardListItemHeader.Helpers.ReadOnlyFor(c => c.TotalAccepted);
										//	}
										//}

									}
								}
							}
						}
					}
				}
			}
		}
	%>

</asp:Content>
