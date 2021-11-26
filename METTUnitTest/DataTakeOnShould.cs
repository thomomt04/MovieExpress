using System;
using System.Configuration;
using System.IO;
using METTLib.DataTakeOn;
using METTLib.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Singular.Web.Security;

namespace METTUnitTest
{
  [TestClass]
  public class DataTakeOnShould
  {
    [TestMethod]
    public void ImportData()
    {
      Singular.Settings.SetConnectionString(ConfigurationManager.AppSettings["ConnectionString"]);

      //WebPrinciple<METTIdentity>.Refetch("DataTakeOn_User", Singular.Web.Security.AuthType.WindowsAuth);
      //WebPrinciple<METTIdentity>.Login("DataTakeOn_User", "KK6FQP48", Singular.Web.Security.AuthType.HTTPHeader);

      METTLib.Security.METTWebSecurity.SetupConsoleAppPrinciple(1, 1, "SuperU");
      
      //Arrange 
      DataTakeOn dataTakeOn = new DataTakeOn(@"C:\Clients\METT\DATA");

      //Act
      dataTakeOn.ImportData();
     

      //Assert
      if(dataTakeOn.InvalidProtectedArearList.Count > 0)
      {
        TextWriter tw = new StreamWriter("ProtectedAreas.csv");

        foreach (var s in dataTakeOn.InvalidProtectedArearList)
          tw.WriteLine($"{s.FileName};{s.ProtectedArea}");

        tw.Close();
      }

      if (dataTakeOn.FailedFiles.Count > 0)
      {
        TextWriter tw = new StreamWriter("FailedFiles.csv");

        foreach (string s in dataTakeOn.FailedFiles)
          tw.WriteLine(s);
        tw.Close();
      }

      Assert.IsTrue(dataTakeOn.InvalidProtectedArearList.Count == 0);
      Assert.IsTrue(dataTakeOn.FailedFiles.Count == 0);
    }
  }
}
