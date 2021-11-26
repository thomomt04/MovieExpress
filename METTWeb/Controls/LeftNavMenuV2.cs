using Singular.Web.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MEWeb.CustomControls
{
	public class LeftNavMenuV2
	{
		public string Setup(SiteMapDataSource smds)
		{
			string html = "";

			SiteMapNodeCollection smc = smds.GetAllNodes();
			SiteMapNodeCollection smcFiltered = new SiteMapNodeCollection();

			//filter the nodes list
			foreach (SiteMapNode smn in smc)
			{
				if(smn.Roles.Count > 0)
				{
					//check if the node has children, if so then we need to include it
					if (smn.HasChildNodes)
					{
						var parentNodeAdded = false;
						SiteMapNodeCollection childNodeList = new SiteMapNodeCollection();
						foreach (SiteMapNode smnL2 in smn.ChildNodes)
						{
							for (int i = 0; i < smnL2.Roles.Count; i++)
							{
								if (MELib.Security.MEWebSecurity.CurrentPrinciple().IsInRole(smnL2.Roles[i].ToString()))
								{
									childNodeList.Add(smnL2);
								}
							}
		
						}
						if(childNodeList.Count > 0)
						{
							smn.ChildNodes.Clear();
							smn.ChildNodes.AddRange(childNodeList);
							if (parentNodeAdded == false)
							{
								smcFiltered.Add(smn);
								parentNodeAdded = true;
							}
						}
						
					}
					else
					{
						//loop through the roles and find out if the logged in user has access
						for (int i = 0; i < smn.Roles.Count; i++)
						{
							if (MELib.Security.MEWebSecurity.CurrentPrinciple().IsInRole(smn.Roles[i].ToString()))
							{
								smcFiltered.Add(smn);
							} 
						}
				
					}
				
				}
				else{
					smcFiltered.Add(smn);
				}
			}

			smc = smcFiltered;

				int nodeIndex = 0;

			foreach (SiteMapNode smn in smc)
      {
        //Cadds active class to to top level:
        html += "<li id=menuItem" + nodeIndex + ">";
				//Check to see if the node is a dropdown rather than a link:
				if (smn.Url.Replace(" ", "") != "")
				{
					html += "<a href = '" + smn.Url + "' >";
				}
				else
				{
					html += "<a>";
			
				}

				html += "<i class = '" + smn.Description + "'></i>";
				html += "<span class='nav-label'>" + smn.Title + "</span>";
				//html += "</a>";

				if (smn.HasChildNodes)
				{
					html += "<span class='fa arrow'></span>";
					html += "</a>";
					html += "<ul class='nav nav-second-level collapse'>";

          int childNodeIndex = 0;
					foreach (SiteMapNode smnL2 in smn.ChildNodes)
					{
						html += "<li id=menuItem" + nodeIndex + "ChildItem" + childNodeIndex + ">";
						html += "<a href = '" + smnL2.Url + "' >";
						html += "<i class = '" + smnL2.Description + "'></i>";
						html += smnL2.Title;
						html += "</a>";
						html += "</li>";
            childNodeIndex++;
					}

					html += "</ul>";
				}


				html += "</li>";

        nodeIndex++;
      }

			return html;

		}
	}
}