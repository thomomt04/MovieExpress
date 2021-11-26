using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MELib;


namespace MEWeb.Examples
{

  public partial class FileUpload : MEPageBase<FileUploadVM>
  {
    protected void Page_Load(object sender, EventArgs e)
    {
    }
  }
  public class FileUploadVM : MEStatelessViewModel<FileUploadVM>
  {

    public FileUploadVM()
    {

    }

    protected override void Setup()
    {
      base.Setup();
    }
  }
}


