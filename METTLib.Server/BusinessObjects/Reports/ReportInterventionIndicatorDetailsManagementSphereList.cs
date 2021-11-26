using System;
using Csla;
using Csla.Serialization;
using Csla.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Singular;
using System.Data;
using System.Data.SqlClient;


namespace METTLib.Reports
{
	[Serializable]
	public class ReportInterventionIndicatorDetailsManagementSphereList
	 : METTBusinessListBase<ReportInterventionIndicatorDetailsManagementSphereList, ReportInterventionIndicatorDetailsManagementSphere>
	{
		#region " Business Methods "

		public ReportInterventionIndicatorDetailsManagementSphere GetItem(int QuestionnaireAnswerResultID)
		{
			foreach (ReportInterventionIndicatorDetailsManagementSphere child in this)
			{
				if (child.QuestionnaireAnswerResultID == QuestionnaireAnswerResultID)
				{
					return child;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return "Report Intervention Indicator Details Management Spheres";
		}

		#endregion

		#region " Data Access "

		[Serializable]
		public class Criteria
			: CriteriaBase<Criteria>
		{
			public int? QuestionnaireAnswerSetID = null;

			public int? ManagementSphereID = null;

			public Criteria()
			{
			}

			public Criteria(int? questionnaireAnswerSetID, int? managementSphereID)
			{
				this.QuestionnaireAnswerSetID = questionnaireAnswerSetID;
				this.ManagementSphereID = managementSphereID;
			}

		}

		public static ReportInterventionIndicatorDetailsManagementSphereList NewReportInterventionIndicatorDetailsManagementSphereList()
		{
			return new ReportInterventionIndicatorDetailsManagementSphereList();
		}

		public ReportInterventionIndicatorDetailsManagementSphereList()
		{
			// must have parameter-less constructor
		}

		public static ReportInterventionIndicatorDetailsManagementSphereList GetReportInterventionIndicatorDetailsManagementSphereList()
		{
			return DataPortal.Fetch<ReportInterventionIndicatorDetailsManagementSphereList>(new Criteria());
		}

		public static ReportInterventionIndicatorDetailsManagementSphereList GetReportInterventionIndicatorDetailsManagementSphereList(int? QuestionnaireAnswerSetID, int? ManagementSphereID)
		{
			return DataPortal.Fetch<ReportInterventionIndicatorDetailsManagementSphereList>(new Criteria(QuestionnaireAnswerSetID, ManagementSphereID));
		}

		protected void Fetch(SafeDataReader sdr)
		{
			this.RaiseListChangedEvents = false;
			while (sdr.Read())
			{
				this.Add(ReportInterventionIndicatorDetailsManagementSphere.GetReportInterventionIndicatorDetailsManagementSphere(sdr));
			}
			this.RaiseListChangedEvents = true;
		}

		protected override void DataPortal_Fetch(Object criteria)
		{
			Criteria crit = (Criteria)criteria;
			using (SqlConnection cn = new SqlConnection(Singular.Settings.ConnectionString))
			{
				cn.Open();
				try
				{
					using (SqlCommand cm = cn.CreateCommand())
					{
						cm.CommandType = CommandType.StoredProcedure;
						cm.CommandText = "GetProcs.getReportInterventionIndicatorDetailsManagementSphereList";

						cm.Parameters.AddWithValue("@QuestionnaireAnswerSetID", Singular.Misc.NothingDBNull(crit.QuestionnaireAnswerSetID));
						cm.Parameters.AddWithValue("@ManagementSphereID", Singular.Misc.NothingDBNull(crit.ManagementSphereID));
						using (SafeDataReader sdr = new SafeDataReader(cm.ExecuteReader()))
						{
							Fetch(sdr);
						}
					}
				}
				finally
				{
					cn.Close();
				}
			}
		}

		#endregion

	}

}