using System.Web;
using System.Web.Security;
using Singular.Web;
using Singular.Web.Controls;
using Singular.Web.CustomControls;
using MELib.Security;
using System.Web.UI;

namespace MEWeb.CustomControls
{
    public class BreadcrumbV2
    {
        public string Setup(SiteMapDataSource smds, string CurrentWebPage)
        {
            SiteMapNode node = smds.Provider.FindSiteMapNodeFromKey(CurrentWebPage);

            if (node == null)
            {
                return "<li></li>";
            }

            string html = "";

            //Loop through Parent Nodes:
            if (node.ParentNode != null)
            {
                if (node.ParentNode.ParentNode != null)
                {
                    html += "<li><a href = '" + node.ParentNode.ParentNode.Url + "'>" + node.ParentNode.ParentNode.Title + "</a></li>";
                }
                html += "<li><a href = '" + node.ParentNode.Url + "'>" + node.ParentNode.Title + "</a></li>";
            }

            //Define current page node:
            html += "<li><strong>" + node.Title + "</strong></li>";

            //Loop through Child Nodes:
            if (node.HasChildNodes)
            {
                foreach (SiteMapNode smn in node.ChildNodes)
                {
                    html += "<li><a href = '" + smn.Url + "'>" + smn.Title + "</a></li>";
                }
            }
            
            return html;

    }

    }
}