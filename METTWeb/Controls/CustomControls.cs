using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MEWeb.CustomControls
{

	public class MainMenu : System.Web.UI.WebControls.WebControl
	
	{

		#region Properties

		public string SiteMapDatasourceID { get; set; }

		#endregion

		#region Methods 

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			dynamic Script = "";

			Script += " var Links = document.getElementsByClassName(\"SingularLink\");";
			Script += " for (i = 0; i < Links.length; i++) {";
			Script += " if (Links[i].href.toString() == window.location.toString()) {";
			Script += " Links[i].className = \"SelectedItem\";";
			Script += " }";
			Script += " }";

			Page.ClientScript.RegisterStartupScript(this.GetType(), "ManagementMenu", Script, true);

			System.Web.UI.WebControls.SiteMapDataSource DS = (System.Web.UI.WebControls.SiteMapDataSource)FindControl(SiteMapDatasourceID);
			SiteMapNode RootNode = DS.Provider.RootNode;

			//writer.WriteLine("<div id ='wrapper' class='active' >");

			//writer.WriteLine("<div id='sidebar-wrapper'>");
			writer.WriteLine("<div class='sidebar-nav nav nav-stacked' id='sidebar'>");
			//writer.WriteLine("<img class='NavImgHeader' src='../Images/menu_header.jpg'>");

			writer.WriteLine("<div id='MainMenu'>");
			//writer.WriteLine("<div class='list-group panel'>");

			foreach (SiteMapNode node in RootNode.ChildNodes)
			{
				if (node.Roles.Count == 0)
				{
					writer.Write(GetNodeString(node));
				}
				else if (Singular.Security.Security.HasAccess(node.Roles[0].ToString()))
				{
					writer.Write(GetNodeString(node));
				}
			}

			//writer.Write("</ul>")

			//writer.WriteLine("</div>");
			//writer.WriteLine("</div>");
			//writer.WriteLine("</div>");
			writer.WriteLine("</div>");
			writer.WriteLine("</div>");
			writer.WriteLine("");


		}

		private string GetNodeString(SiteMapNode node)
		{

			System.Text.StringBuilder SB = new System.Text.StringBuilder();

			if (node.HasChildNodes)
			{
				SB.Append("<a ");
				//SB.Append("href='" + node.Url + "#" + node.Title.ToLower() + "'");
				SB.Append("id='" + node["id"] + "' ");
				if (!(string.IsNullOrWhiteSpace(node["onclick"])))
				{
					SB.Append("onclick='" + node["onclick"] + "' ");
				}
				if (!(string.IsNullOrWhiteSpace(node["class"])))
				{
					SB.Append("class='" + node["class"] + "' ");
				}
				SB.Append("data-toggle = 'collapse'");
				SB.Append("style = 'height:auto'");
				SB.Append("data-parent='#MainMenu' ");
			}
			else
			{
				if ((node.Title.Contains("Menu-Close")))
				{
					SB.Append("<a href='#' ");
					SB.Append("id='menu-toggle' ");
					SB.Append("onclick='doTheToggle()' ");
				}
				else
				{
					SB.Append("<a href='" + node.Url + "' ");
				}
				SB.Append("class='list-group-item' ");
				SB.Append("data-parent='#MainMenu' ");
			}

			SB.Append("id='" + node["id"] + "' > ");

			SB.Append("<em>" + node.Title + "</em>");

			if ((node.Title.Contains("Menu")))
			{
				SB.Append("<span id ='main_icon' class='" + node["glyphicon"] + "' >");

			}
			else
			{
				SB.Append("<span class='" + node["glyphicon"] + "' >");
			}


			SB.Append("</span>");

			if (node.HasChildNodes)
			{
				SB.Append("<i class='fa fa-caret-down'></i>");
				SB.Append("</a>");
				SB.Append("<div class='submenu panel-collapse collapse' id='" + node.Title.ToLower() + "'>");
			}
			else
			{
				SB.Append("</a>");
			}



			if (node.HasChildNodes)
			{
				foreach (SiteMapNode SubNode in node.ChildNodes)
				{
					if (SubNode.Roles.Count == 0)
					{
						SB.Append(GetNodeStringChild(SubNode));
					}
					else if (Singular.Security.Security.HasAccess(SubNode.Roles[0].ToString()))
					{
						SB.Append(GetNodeStringChild(SubNode));
					}
				}
			}
			if (node.HasChildNodes)
			{
				SB.Append("</div>");
			}

			return SB.ToString();

		}

		private string GetNodeStringChild(SiteMapNode node)
		{
			System.Text.StringBuilder SB = new System.Text.StringBuilder();

			if (node.HasChildNodes)
			{
				SB.Append("<a");

				SB.Append(" onclick='");
				SB.Append(" if (!this.parentElement.children) { return }");
				SB.Append(" if (this.parentElement.children.length <= 1) { return }");
				SB.Append(" if (this.parentElement.children[1].clientHeight > 0) {");
				SB.Append("  this.parentElement.children[1].style.display = 'none';");
				SB.Append(" } else {");
				SB.Append("  this.parentElement.children[1].style.display = 'block';");
				SB.Append(" };' ");
				SB.Append(" >");

			}
			else
			{
				SB.Append("<a  href='" + node.Url + "' ");
				SB.Append("id='" + node["id"] + "' ");

				if (!(string.IsNullOrWhiteSpace(node["class"])))
				{
					SB.Append("class='" + node["class"] + "' ");
				}

				if (!(string.IsNullOrWhiteSpace(node["onclick"])))
				{
					SB.Append("onclick='" + node["onclick"] + "' ");
				}

				if (!(string.IsNullOrWhiteSpace(node["data-toggle"])))
				{
					SB.Append("data-toggle='" + node["data-toggle"] + "' ");
				}

				if (!(string.IsNullOrWhiteSpace(node["parent"]) | node["parent"] == "Client"))
				{
					SB.Append("data-parent='" + node.Url + "' ");
				}

				SB.Append(">");
			}

			SB.Append("<em>");
			SB.Append(node.Title);
			SB.Append("</em>");

			if (node["glyphicon"] != null)
			{
				SB.Append("<span class='" + node["glyphicon"] + "' ></span>");
			}
			else
			{
				if (node["font-awesome"] != null)
				{
					SB.Append("<span class='" + node["font-awesome"] + "' ></span>");
				}
			}

			if (node.HasChildNodes)
			{
				SB.Append("<span class='' ></span>");
			}

			SB.Append("</a>");

			if (node.HasChildNodes)
			{
				foreach (SiteMapNode SubNode in node.ChildNodes)
				{
					if (SubNode.Roles.Count == 0)
					{
						SB.Append(GetNodeStringChild(SubNode));

					}
					else if (Singular.Security.Security.HasAccess(SubNode.Roles[0].ToString()))
					{
						SB.Append(GetNodeStringChild(SubNode));
					}
				}
			}

			return SB.ToString();

		}

		protected virtual Boolean HasAccess(string PagePath)
		{
			return Singular.Web.CustomControls.SiteMapDataSource.HasAccess(PagePath);
		}

		#endregion

	}

}