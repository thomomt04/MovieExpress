﻿// Generated 05 Dec 2018 07:42 - Singular Systems Object Generator Version 2.2.694
//<auto-generated/>
using System;
using Csla;
using Csla.Serialization;
using Csla.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Singular;
using System.Data;
using System.Data.SqlClient;


namespace METTLib.Maintenance
{
	[Serializable]
	public class QuestionnaireQuestionManagementSphereList
	 : METTBusinessListBase<QuestionnaireQuestionManagementSphereList, QuestionnaireQuestionManagementSphere>
	{
		#region " Business Methods "

		public QuestionnaireQuestionManagementSphere GetItem(int QuestionnaireQuestionManagementSphereID)
		{
			foreach (QuestionnaireQuestionManagementSphere child in this)
			{
				if (child.QuestionnaireQuestionManagementSphereID == QuestionnaireQuestionManagementSphereID)
				{
					return child;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return "Questionnaire Question Management Spheres";
		}

		#endregion

		#region " Data Access "

		[Serializable]
		public class Criteria
			: CriteriaBase<Criteria>
		{

			public int? QuestionnaireQuestionID { get; set; }

			public Criteria( int? questionnaireQuestionID)
			{
				QuestionnaireQuestionID = questionnaireQuestionID;
			}

			public Criteria()
			{
			}

		}

		public static QuestionnaireQuestionManagementSphereList NewQuestionnaireQuestionManagementSphereList()
		{
			return new QuestionnaireQuestionManagementSphereList();
		}
	
		public QuestionnaireQuestionManagementSphereList()
		{
			// must have parameter-less constructor
		}

		public static QuestionnaireQuestionManagementSphereList GetQuestionnaireQuestionManagementSphereList(int? QuestionnaireQuestionID)
		{
			return DataPortal.Fetch<QuestionnaireQuestionManagementSphereList>(new Criteria(QuestionnaireQuestionID));
		}



		public static QuestionnaireQuestionManagementSphereList GetQuestionnaireQuestionManagementSphereList()
		{
			return DataPortal.Fetch<QuestionnaireQuestionManagementSphereList>(new Criteria());
		}

		protected void Fetch(SafeDataReader sdr)
		{
			this.RaiseListChangedEvents = false;
			while (sdr.Read())
			{
				this.Add(QuestionnaireQuestionManagementSphere.GetQuestionnaireQuestionManagementSphere(sdr));
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
						cm.CommandText = "GetProcs.getQuestionnaireQuestionManagementSphereList";

						cm.Parameters.AddWithValue("@QuestionnaireQuestionID", Singular.Misc.NothingDBNull(crit.QuestionnaireQuestionID));

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