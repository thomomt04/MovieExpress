﻿// Generated 26 Mar 2019 11:07 - Singular Systems Object Generator Version 2.2.694
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


namespace METTLib.RO
{
	[Serializable]
	public class ROQuestionnaireStatusList
	 : METTReadOnlyListBase<ROQuestionnaireStatusList, ROQuestionnaireStatus>
	{
		#region " Business Methods "

		public ROQuestionnaireStatus GetItem(int QuestionnaireStatusID)
		{
			foreach (ROQuestionnaireStatus child in this)
			{
				if (child.QuestionnaireStatusID == QuestionnaireStatusID)
				{
					return child;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return "Questionnaire Status";
		}

		#endregion

		#region " Data Access "

		[Serializable]
		public class Criteria
			: CriteriaBase<Criteria>
		{
			public Criteria()
			{
			}

		}

		public static ROQuestionnaireStatusList NewROQuestionnaireStatusList()
		{
			return new ROQuestionnaireStatusList();
		}

		public ROQuestionnaireStatusList()
		{
			// must have parameter-less constructor
		}

		public static ROQuestionnaireStatusList GetROQuestionnaireStatusList()
		{
			return DataPortal.Fetch<ROQuestionnaireStatusList>(new Criteria());
		}

		protected void Fetch(SafeDataReader sdr)
		{
			this.RaiseListChangedEvents = false;
			this.IsReadOnly = false;
			while (sdr.Read())
			{
				this.Add(ROQuestionnaireStatus.GetROQuestionnaireStatus(sdr));
			}
			this.IsReadOnly = true;
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
						cm.CommandText = "GetProcs.getROQuestionnaireStatusList";
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