using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MELib;


namespace MEWeb.Examples
{

  public partial class StepWizard : MEPageBase<StepWizardVM>
  {
    protected void Page_Load(object sender, EventArgs e)
    {
    }
  }
  public class StepWizardVM : MEStatelessViewModel<StepWizardVM>
  {

    public int FirstQuestionID { get; set; }

    public StepWizardVM()
    {

    }

    protected override void Setup()
    {
      base.Setup();


    }



  }
}


