using Microsoft.VisualStudio.TestTools.UnitTesting;
using METTLib.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace METTLib.Document.Tests
{
	[TestClass()]
	public class ExcelShould
	{
		[TestMethod()]
		//[Ignore("This is a integrated test that need to run manually")]
		public void SaveData_For_AssessmentImports()
		{
			//Arrange
			Singular.Settings.SetConnectionString(ConfigurationManager.AppSettings["ConnectionString"]);
			string filePath = @"C:\Clients\METT\mett\METTUnitTest\TestCaseFiles\METT-Assessment-Template-261118-045557.xlsx";
			System.IO.Stream stream = System.IO.File.Open(filePath, System.IO.FileMode.Open);
			METTLib.Document.Excel excel = new Excel(stream, "METT-Assessment-Template.xls");

			// Act
			if(excel.IsValidFile())
			{
				excel.SaveData();
			}
			//Assert
			Assert.Fail();
		}
	}
}