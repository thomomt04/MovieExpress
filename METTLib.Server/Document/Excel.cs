using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Infragistics.Documents.Excel;

namespace METTLib.Document
{
	public class Excel : Document
	{
		private List<Type> CSLAObjectTypeList;
		private List<DataTable> ExcelDataTableList;
		private List<ExcelImportDocumentHeaderList> ExcelImportDocumentHeadersList;
		private Infragistics.Documents.Excel.Workbook ExcelWorkbook;
		public List<dynamic> CSLAListObjectList { get; private set; }
		private bool useStream;
		private System.IO.Stream _documentStream;

		public Excel(string documentLocation) : base(documentLocation)
		{
			useStream = false;
			ExcelWorkbook = Infragistics.Documents.Excel.Workbook.Load(documentLocation);
			ExcelDataTableList = new List<DataTable>();
			CSLAObjectTypeList = new List<Type>();
			ExcelImportDocumentHeadersList = new List<ExcelImportDocumentHeaderList>();
			CSLAListObjectList = new List<dynamic>();
		}

		public Excel(System.IO.Stream documentStream, string fileName) : base(documentStream, fileName)
		{
			useStream = true;
			_documentStream = documentStream;
			ExcelWorkbook = Infragistics.Documents.Excel.Workbook.Load(documentStream);
			ExcelDataTableList = new List<DataTable>();
			CSLAObjectTypeList = new List<Type>();
			ExcelImportDocumentHeadersList = new List<ExcelImportDocumentHeaderList>();
			CSLAListObjectList = new List<dynamic>();
		}

		private bool HasValidData()
		{
			for (int i = 0; i < CSLAObjectTypeList.Count; i++)
			{
				dynamic CSLAListObject = Activator.CreateInstance(CSLAObjectTypeList[i]);

				foreach (DataRow row in ExcelDataTableList[i].Rows)
				{
					Type ChildType = Singular.Reflection.GetLastGenericType(CSLAObjectTypeList[i]);
					dynamic CSLAObject = Activator.CreateInstance(ChildType);
					foreach (var column in ExcelImportDocumentHeadersList[i])
					{
						if (!column.NotImportableInd)
						{
							string ColumnName = column.ExcelHeader;
							if (row[ColumnName].ToString().Replace("N/A", "").Trim().Length > 0 && row[ColumnName].ToString().ToUpper() != "NULL")
							{
								string PropertyName = column.CSLAProperty;
								if (PropertyName.Length > 0)
								{
									PropertyInfo Property = ChildType.GetProperty(PropertyName);
									if (Property != null)
									{
										try
										{
											if (Property.PropertyType.ToString().Contains("System.DateTime"))
											{
												var dataTableValue = ConvertExcelIntToDate((int)Convert.ToDecimal(row[ColumnName].ToString().Replace("N/A", "").Trim()));
												Property.SetValue(CSLAObject, dataTableValue);
											}
											else
											{
												Property.SetValue(CSLAObject, Singular.Reflection.ConvertValueToType(Property.PropertyType, row[ColumnName].ToString().Replace("N/A", "").Trim()));
											}
										}
										catch (Exception ex)
										{

											addValidationMessage($"Import failed. Error on Sheet ({ExcelDataTableList[i]}) for column ({ColumnName}) row ({ExcelDataTableList[i].Rows.IndexOf(row) + 2}) value ({row[ColumnName].ToString()}). Error occured: ({ex.Message})", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.File);
											return false;
										}
									}

								}
								else
								{
									addValidationMessage($"Import failed. Required field ({row[ColumnName].ToString()})", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.File);


								}

							}



						}
					}

					if (!CSLAObject.IsValid)
					{
						foreach (Csla.Rules.BrokenRule rule in CSLAObject.BrokenRulesCollection)
						{
							addValidationMessage(rule.Description, rule.Severity, DocumentEnums.MessageRelation.Data);
						}
						return false;
					}
					CSLAListObject.Add(CSLAObject);
				}

				if (CSLAListObjectList.Count > 0)
				{
					bool TypeIsInList = false;
					foreach (var obj in CSLAListObjectList)
					{
						if (obj.GetType() == CSLAListObject.GetType())
						{
							obj.AddRange(CSLAListObject);
							TypeIsInList = true;
							break;
						}
					}
					if (!TypeIsInList)
					{
						CSLAListObjectList.Add(CSLAListObject);
					}
				}
				else
				{
					CSLAListObjectList.Add(CSLAListObject);
				}
			}
			return true;
		}

		private static bool IsBetween(int item, int start, int end)
		{
			return Comparer<int>.Default.Compare(item, start) >= 0
					&& Comparer<int>.Default.Compare(item, end) <= 0;
		}

		public string GETDocumentNameSheet(int sheetIndex)
		{
			string ObjectName = string.Empty;
			bool containsAllHeader = false;
			ExcelImportDocumentList DocumentList = ExcelImportDocumentList.GetExcelImportDocumentList();
			var sheet = ExcelWorkbook.Worksheets[sheetIndex];

			DataTable ExcelDataTable = getDataTable(sheet);
			foreach (var excelDoc in DocumentList)
			{
				if (excelDoc.ExcelImportDocumentHeaderList.Count == ExcelDataTable.Columns.Count)
				{
					foreach (var excelHeader in excelDoc.ExcelImportDocumentHeaderList)
					{
						if (!ExcelDataTable.Columns.Contains(excelHeader.ExcelHeader))
						{
							break;
						}
						containsAllHeader = true;
					}
					if (!containsAllHeader) { continue; }
					ObjectName = excelDoc.DocumentName;
					break;
				}
			}
			return ObjectName;
		}

		private bool HasValidLayOut()
		{

			ExcelImportDocumentList DocumentList = ExcelImportDocumentList.GetExcelImportDocumentList();
			bool ContainsAllHeader = false;

			foreach (var sheet in ExcelWorkbook.Worksheets)
			{
				ContainsAllHeader = false;
				DataTable ExcelDataTable = getDataTable(sheet);

				ExcelDataTableList.Add(ExcelDataTable);

				foreach (var excelDoc in DocumentList)
				{
					if (excelDoc.ExcelImportDocumentHeaderList.Count == ExcelDataTable.Columns.Count)
					{
						foreach (var excelHeader in excelDoc.ExcelImportDocumentHeaderList)
						{
							if (!ExcelDataTable.Columns.Contains(excelHeader.ExcelHeader))
							{
								break;
							}
							ContainsAllHeader = true;
						}
						if (!ContainsAllHeader) { continue; }
						try
						{
							ExcelImportDocumentHeadersList.Add(excelDoc.ExcelImportDocumentHeaderList);
							CSLAObjectTypeList.Add(Type.GetType(excelDoc.CSLAObject));
						}
						catch (TypeLoadException e)
						{
							addValidationMessage($"{e.GetType().Name}: Unable to load type {excelDoc.CSLAObject}", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.File);
						}
						break;
					}
				}

				if (ContainsAllHeader == false)
				{

					//Add Logic here to see if there is a way to chech closes macth
					foreach (var excelDoc in DocumentList)
					{
						List<string> InvalidColumnList = new List<string>();
						List<string> ExpectedColumnList = new List<string>();
						int headercountdiff = excelDoc.ExcelImportDocumentHeaderList.Count - ExcelDataTable.Columns.Count;

						if (IsBetween(headercountdiff, -2, 2))
						{
							foreach (var excelHeader in excelDoc.ExcelImportDocumentHeaderList)
							{
								if (!ExcelDataTable.Columns.Contains(excelHeader.ExcelHeader))
								{
									ExpectedColumnList.Add(excelHeader.ExcelHeader);
								}
							}

							foreach (DataColumn dc in ExcelDataTable.Columns)
							{
								if (excelDoc.ExcelImportDocumentHeaderList.FirstOrDefault(x => x.ExcelHeader.ToLower() == dc.ColumnName.ToLower()) == null)
								{
									InvalidColumnList.Add(dc.ColumnName);
								}
							}

							if (ExpectedColumnList.Count <= 3)
							{
								StringBuilder layoutDiff = new StringBuilder();

								if (InvalidColumnList.Count > 0)
								{
									layoutDiff.Append($"Invalid Columns({ string.Join(",", InvalidColumnList)})");
								}

								if (ExpectedColumnList.Count > 0)
								{
									layoutDiff.Append($"Expected Columns({ string.Join(",", ExpectedColumnList)})");
								}

								addValidationMessage($"Invalid file format for {ExcelDataTable.TableName} closes supported file is {excelDoc.DocumentName}. Layout Differences : {layoutDiff.ToString()}", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.File);
								break;
							}
						}
					}
					break;
				}
			}
			return ContainsAllHeader;
		}

		private DataTable getDataTable(Worksheet sheet)
		{
			DataTable ExcelDataTable;
			if (useStream == true)
			{
				ExcelDataTable = Singular.Data.Excel.ImportExcelToDataTableUsingInfragistics(_documentStream, true, sheet.Name);
			}
			else
			{
				ExcelDataTable = Singular.Data.Excel.ImportExcelToDataTableUsingInfragistics(DocumentLocation, true, sheet.Name);
			}

			return ExcelDataTable;
		}

		public override void SaveData()
		{
			for (int i = 0; i < CSLAListObjectList.Count; i++)
			{
				var cslSaveObject = CSLAListObjectList[i].Save();
			}


		}

		public string SaveDataWithReturnPropValue(string PropName)
		{
			string PropValue = string.Empty;
			for (int i = 0; i < CSLAListObjectList.Count; i++)
			{
				Type ChildType = Singular.Reflection.GetLastGenericType(CSLAObjectTypeList[i]);
				PropertyInfo Property = ChildType.GetProperty(PropName);
				var cslSaveObject = CSLAListObjectList[i].Save();

				if (Property != null)
				{
					PropValue = Property.GetValue(cslSaveObject[0]).ToString();
				}
			}

			return PropValue;
		}

		public override bool IsValidFile()
		{
			if (DocumentType != DocumentEnums.DocumentTypes.Excel)
			{
				addValidationMessage("Document is not a valid Excel File", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.File);
				return false;
			}

			if (!HasValidLayOut())
			{
				addValidationMessage("There is no object that support the file provided, please provide correctly formated file.", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.File);
				return false;
			}

			//if (HasAlreadyFileBeenUploaded())
			//{
			//	addValidationMessage("File has already been upload, please delete existing data before uploading", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.Data);
			//	return false;
			//}

			if (!HasValidData())
			{
				addValidationMessage("Invalid Data", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.Data);
				return false;
			}


			return true;
		}

		//private bool HasAlreadyFileBeenUploaded()
		//{
		//	//try
		//	//{
		//	//	//RORawClientBalanceDateList roRawClientBalanceDateList = RORawClientBalanceDateList.GetRORawClientBalanceDateList();

		//	//	DateTime excelBalanceDate = Singular.Misc.Dates.DateMonthEnd(ConvertExcelIntToDate((int)Convert.ToDecimal(ExcelDataTableList[1].Rows[1]["Balance Date"])));
		//	//	int excelLoanManagementSystemID = Convert.ToInt32(ExcelDataTableList[1].Rows[1]["LoanManagementSystemID"]);

		//	//	if (roRawClientBalanceDateList.Where(x => x.BalanceDate == excelBalanceDate && x.LoanManagementSystemID == excelLoanManagementSystemID).Count() > 0)
		//	//	{
		//	//		return true;
		//	//	}
		//	//}
		//	//catch (Exception ex)
		//	//{
		//	//	addValidationMessage($"Error checking duplicate file import;  Error : {ex.Message}", Csla.Rules.RuleSeverity.Error, DocumentEnums.MessageRelation.File);
		//	//	return true;
		//	//}		

		//	//return false;			
		//}

		private DateTime ConvertExcelIntToDate(int ExcelNumber)
		{
			DateTime StartDate = DateTime.Parse("1899/12/31");
			return StartDate.AddDays(ExcelNumber - 1);
		}

	}
}
