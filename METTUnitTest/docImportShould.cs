using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;


namespace METTUnitTest
{

	[TestClass]
	public class docImportShould
	{
		[ClassInitialize]
	public static void docImportClassInit (TestContext context)
		{
			Singular.Settings.SetConnectionString(ConfigurationManager.AppSettings["ConnectionString"]);
		}

		[TestMethod]
		public void shouldImportSurvey()
		{
			//Arrange
			string workingFile = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}/TestCaseFiles/test.xls";
			METTLib.Document.Excel SurveyFile = new METTLib.Document.Excel(workingFile);
			//Act
			bool isValidFile = SurveyFile.IsValidFile();
			if (isValidFile)
			{
				SurveyFile.SaveData();
			}
			//Assert
			Assert.IsTrue(isValidFile);
		}


	}
}
