using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METTLib.Reports
{
	public class METTDynamicReport : Singular.Reporting.Dynamic.DynamicReport
	{

		public static Dictionary<string, Type> EncryptedFields = new Dictionary<string, Type> 
		{
			{
				"AssessedBy",
				typeof(string)
			},
			{
				"ReviewedBy",
				typeof(string)
			},
			{
				"AuditedBy",
				typeof(string)
			},
			{
			 "FullName",
			 typeof(string)
			},
      {
       "ContactPerson",
       typeof(string)
      }
    };

		protected virtual bool HasEncryptedValues
		{
			get { return true; }
		}

		protected virtual Dictionary<string, Type> EncryptedFieldNames
		{
			get { return EncryptedFields; }
		}

		public METTDynamicReport(Singular.Reporting.Dynamic.Report ReportInfo) : base(ReportInfo)
		{
		}

		public METTDynamicReport(Singular.Reporting.Dynamic.DynamicReport Report, Singular.Reporting.Dynamic.DynamicReportCriteria Criteria) : base(Report, Criteria)
		{
		}

		protected override void ModifyDataSet(DataSet DataSet)
		{
			base.ModifyDataSet(DataSet);

			if (HasEncryptedValues && EncryptedFieldNames.Count > 0)
			{
				DataSet = DecryptData(DataSet);
			}

		}

		public static DataSet DecryptDataSet(DataSet ds)
		{

			METTDynamicReport sf = new METTDynamicReport(new Singular.Reporting.Dynamic.Report());
			return sf.DecryptData(ds);

		}


		protected DataSet DecryptData(DataSet ds)
		{

			foreach (DataTable Table in ds.Tables)
			{
				int i = 0;
				// Each column As DataColumn In Table.Columns
				for (i = 0; i <= Table.Columns.Count - 1; i++)
				{
					dynamic column = Table.Columns[i];
					if (EncryptedFieldNames.ContainsKey(column.ColumnName))
					{
						Type type = EncryptedFieldNames[column.ColumnName];
						Table.Columns.Add("New_Decrypted_" + column.ColumnName, type);
						Table.Columns[Table.Columns.Count - 1].SetOrdinal(i + 1);
						foreach (DataRow row in Table.Rows)
						{
							//Decrypt Value
							if (object.ReferenceEquals(row[i], DBNull.Value))
							{
								row[i + 1] = row[i];
							}
							else
							{
								//row[i + 1] = METTLib.Helpers.METTHelpers.DecryptStringDatabaseValue(row[i]);
								//THIS IS ONLY TEMPORARY: at the moment the encryoted fields have UserID stored in them, therefore we would like to display the User's fullname, I'm adding this logic here as I get errors when try to concatenate FirstName and LastName due to them having a data type of varbinary
								if(int.TryParse(row[i].ToString(), out int output) == false)
								{
									row[i + 1] = METTLib.Helpers.METTHelpers.DecryptStringDatabaseValue(row[i]);
								}
								else
								{						
									row[i + 1] = METTLib.CommonData.Lists.ROUserList.FirstOrDefault(c => c.UserID == int.Parse(row[i].ToString()))?.FullName;
								}
							}
						}
						//Once All Updated, change column type
						Table.Columns.RemoveAt(i);
						Table.Columns[i].ColumnName = Table.Columns[i].ColumnName.Replace("New_Decrypted_", "");
					}
					//Check for GUID and convert to string
					if (object.ReferenceEquals(column.DataType, typeof(Guid)))
					{
						Table.Columns.Add("New_Guid_" + column.ColumnName, typeof(string));
						Table.Columns[Table.Columns.Count - 1].SetOrdinal(i + 1);
						foreach (DataRow row in Table.Rows)
						{
							//Decrypt Value
							if (object.ReferenceEquals(row[i], DBNull.Value))
							{
								row[i + 1] = row[i];
							}
							else
							{
								row[i + 1] = row[i].ToString();
							}
						}
						//Once All Updated, change column type
						Table.Columns.RemoveAt(i);
						Table.Columns[i].ColumnName = Table.Columns[i].ColumnName.Replace("New_Guid_", "");
					}
				}

			}
			return ds;
		}

	}

	public abstract class METTReportBase<RC> : Singular.Reporting.ReportBase<RC> where RC : Singular.Reporting.ReportCriteria
	{

		protected virtual bool HasEncryptedValues
		{
			get { return true; }
		}

		protected virtual Dictionary<string, Type> EncryptedFieldNames
		{
			get { return METTDynamicReport.EncryptedFields; }
		}

		protected override void ModifyDataSet(DataSet DataSet)
		{
			base.ModifyDataSet(DataSet);

			if (HasEncryptedValues && EncryptedFieldNames.Count > 0)
			{
				DataSet = DecryptData(DataSet);
			}

		}

		protected DataSet DecryptData(DataSet ds)
		{
			foreach (DataTable Table in ds.Tables)
			{
				int i = 0;
				// Each column As DataColumn In Table.Columns
				for (i = 0; i <= Table.Columns.Count - 1; i++)
				{
					dynamic column = Table.Columns[i];
					if (EncryptedFieldNames.ContainsKey(column.ColumnName))
					{
						Type type = EncryptedFieldNames[column.ColumnName];
						Table.Columns.Add("New_Decrypted_" + column.ColumnName, type);
						Table.Columns[Table.Columns.Count - 1].SetOrdinal(i + 1);
						foreach (DataRow row in Table.Rows)
						{
							//Decrypt Value
							if (object.ReferenceEquals(row[i], DBNull.Value))
							{
								row[i + 1] = row[i];
							}
							else
							{
								row[i + 1] = METTLib.Helpers.METTHelpers.DecryptStringDatabaseValue(row[i]);
							}
						}
						//Once All Updated, change column type
						Table.Columns.RemoveAt(i);
						Table.Columns[i].ColumnName = Table.Columns[i].ColumnName.Replace("New_Decrypted_", "");
					}
					//Check for GUID and convert to string
					if (object.ReferenceEquals(column.DataType, typeof(Guid)))
					{
						Table.Columns.Add("New_Guid_" + column.ColumnName, typeof(string));
						Table.Columns[Table.Columns.Count - 1].SetOrdinal(i + 1);
						foreach (DataRow row in Table.Rows)
						{
							//Decrypt Value
							if (object.ReferenceEquals(row[i], DBNull.Value))
							{
								row[i + 1] = row[i];
							}
							else
							{
								row[i + 1] = row[i].ToString();
							}
						}
						//Once All Updated, change column type
						Table.Columns.RemoveAt(i);
						Table.Columns[i].ColumnName = Table.Columns[i].ColumnName.Replace("New_Guid_", "");
					}
				}

			}
			return ds;
		}

	}
}
