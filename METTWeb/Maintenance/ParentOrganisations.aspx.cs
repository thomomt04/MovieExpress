using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using METTLib.Organisation;
using Singular.Web;
using System.Text;

namespace METTWeb.Maintenance
{
	public partial class ParentOrganisations : METTPageBase<ParentOrganisationVM>
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}

	public class ParentOrganisationVM : METTStatelessViewModel<ParentOrganisationVM>
	{
		public OrganisationList OrganisationList { get; set; }

		protected override void Setup()
		{
			base.Setup();
			OrganisationList = OrganisationList.GetOrganisationList();
		}

		[WebCallable]
		public static Result SaveParentOrganisations(OrganisationList OrganisationList)
		{
			Result results = new Result();
			OrganisationRelationshipList organisationRelationships = OrganisationRelationshipList.GetOrganisationRelationshipList();
			OrganisationList OrganisationToSaveList = OrganisationList.NewOrganisationList();
			StringBuilder linkedOrganisations= new StringBuilder();

			try
			{
				//only deal with organisations that have changes
				foreach (var organisation in OrganisationList.Where(c => c.IsDirty))
				{
					//check if the organisation has been linked
					if(organisation.ParentInd == false && organisationRelationships.Any(c => c.OrganisationId_Parent == organisation.OrganisationID))
					{
						//make a list of those organisations so that we may inform the user
						linkedOrganisations.AppendLine(organisation.OrganisationName + ";<br>");
					}
					else
					{
						OrganisationToSaveList.Add(organisation);
					}
					
				
				}

				//only save the list when none of the modified organisations
				if (OrganisationToSaveList.IsValid)
				{
					OrganisationToSaveList.Save();

					results.Success = true;
					if (linkedOrganisations.Length > 0)
					{
						results.Data = "The following organisations were not saved: " + linkedOrganisations + " as they are parents to other organisations.";
					}
					else
					{
						results.Data = "Parent Organisations setup was successful.";
					}
					
				}
				else
				{
					results.Success = false;
					results.ErrorText= OrganisationToSaveList.GetErrorsAsHTMLString();
					return results;
				}

			}
			catch (Exception ex)
			{
				results.Success = false;
				results.ErrorText = ex.Message;
			}
			

			return results;
		}


	}
}