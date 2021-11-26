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
	public class ReportInterventionIndicatorDetailsManagementSphere
	 : METTBusinessBase<ReportInterventionIndicatorDetailsManagementSphere>
	{
		#region " Properties and Methods "

		#region " Properties "

		public static PropertyInfo<int> QuestionnaireAnswerResultIDProperty = RegisterProperty<int>(c => c.QuestionnaireAnswerResultID, "ID", 0);
		/// <summary>
		/// Gets the ID value
		/// </summary>
		[Display(AutoGenerateField = false), Key]
		public int QuestionnaireAnswerResultID
		{
			get { return GetProperty(QuestionnaireAnswerResultIDProperty); }
		}

		public static PropertyInfo<int> ManagementSphereIDProperty = RegisterProperty<int>(c => c.ManagementSphereID, "Management Sphere", 0);
		/// <summary>
		/// Gets and sets the Management Sphere value
		/// </summary>
		[Display(Name = "Management Sphere", Description = ""),
		Required(ErrorMessage = "Management Sphere required")]
		public int ManagementSphereID
		{
			get { return GetProperty(ManagementSphereIDProperty); }
			set { SetProperty(ManagementSphereIDProperty, value); }
		}

		public static PropertyInfo<String> ManagementSphereProperty = RegisterProperty<String>(c => c.ManagementSphere, "Management Sphere", "");
		/// <summary>
		/// Gets and sets the Management Sphere value
		/// </summary>
		[Display(Name = "Management Sphere", Description = ""),
		StringLength(500, ErrorMessage = "Management Sphere cannot be more than 500 characters")]
		public String ManagementSphere
		{
			get { return GetProperty(ManagementSphereProperty); }
			set { SetProperty(ManagementSphereProperty, value); }
		}

		public static PropertyInfo<String> ManagementSphereContentProperty = RegisterProperty<String>(c => c.ManagementSphereContent, "Management Sphere Content", "");
		/// <summary>
		/// Gets and sets the Management Sphere Content value
		/// </summary>
		[Display(Name = "Management Sphere Content", Description = "")]
		public String ManagementSphereContent
		{
			get { return GetProperty(ManagementSphereContentProperty); }
			set { SetProperty(ManagementSphereContentProperty, value); }
		}

		public static PropertyInfo<String> IndicatorDetailNameProperty = RegisterProperty<String>(c => c.IndicatorDetailName, "Indicator Detail Name", "");
		/// <summary>
		/// Gets and sets the Indicator Detail Name value
		/// </summary>
		[Display(Name = "Indicator Detail Name", Description = ""),
		StringLength(500, ErrorMessage = "Indicator Detail Name cannot be more than 500 characters")]
		public String IndicatorDetailName
		{
			get { return GetProperty(IndicatorDetailNameProperty); }
			set { SetProperty(IndicatorDetailNameProperty, value); }
		}

		public static PropertyInfo<String> NextStepsProperty = RegisterProperty<String>(c => c.NextSteps, "NextSteps", "");
		/// <summary>
		/// Gets and sets the NextSteps value
		/// </summary>
		[Display(Name = "NextSteps", Description = ""),
		StringLength(500, ErrorMessage = "NextSteps")]
		public String NextSteps
		{
			get { return GetProperty(NextStepsProperty); }
			set { SetProperty(NextStepsProperty, value); }
		}

		public static PropertyInfo<String> CommentsProperty = RegisterProperty<String>(c => c.Comments, "Comments", "");
		/// <summary>
		/// Gets and sets the Comments value
		/// </summary>
		[Display(Name = "Comments", Description = ""),
		StringLength(500, ErrorMessage = "Comments")]
		public String Comments
		{
			get { return GetProperty(CommentsProperty); }
			set { SetProperty(CommentsProperty, value); }
		}

		public static PropertyInfo<String> EvidenceProperty = RegisterProperty<String>(c => c.Evidence, "Evidence", "");
		/// <summary>
		/// Gets and sets the Evidence value
		/// </summary>
		[Display(Name = "Evidence", Description = ""),
		StringLength(500, ErrorMessage = "Evidence")]
		public String Evidence
		{
			get { return GetProperty(EvidenceProperty); }
			set { SetProperty(EvidenceProperty, value); }
		}

		//public static PropertyInfo<int> MaxValueProperty = RegisterProperty<int>(c => c.MaxValue, "Max Value", 0);
		///// <summary>
		///// Gets and sets the Max Value value
		///// </summary>
		//[Display(Name = "Max Value", Description = ""),
		//Required(ErrorMessage = "Max Value required")]
		//public int MaxValue
		//{
		//	get { return GetProperty(MaxValueProperty); }
		//	set { SetProperty(MaxValueProperty, value); }
		//}

		//public static PropertyInfo<int> AnswerRatingProperty = RegisterProperty<int>(c => c.AnswerRating, "Answer Rating", 0);
		///// <summary>
		///// Gets and sets the Answer Rating value
		///// </summary>
		//[Display(Name = "Answer Rating", Description = ""),
		//Required(ErrorMessage = "Answer Rating required")]
		//public int AnswerRating
		//{
		//	get { return GetProperty(AnswerRatingProperty); }
		//	set { SetProperty(AnswerRatingProperty, value); }
		//}

		#endregion

		#region " Methods "

		protected override object GetIdValue()
		{
			return GetProperty(QuestionnaireAnswerResultIDProperty);
		}

		public override string ToString()
		{
			if (this.ManagementSphere.Length == 0)
			{
				if (this.IsNew)
				{
					return String.Format("New {0}", "Report Intervention Management Sphere");
				}
				else
				{
					return String.Format("Blank {0}", "Report Intervention Management Sphere");
				}
			}
			else
			{
				return this.ManagementSphere;
			}
		}

		#endregion

		#endregion

		#region " Validation Rules "

		protected override void AddBusinessRules()
		{
			base.AddBusinessRules();
		}

		#endregion

		#region " Data Access & Factory Methods "

		protected override void OnCreate()
		{
			// This is called when a new object is created
			// Set any variables here, not in the constructor or NewReportInterventionManagementSphere() method.
		}

		public static ReportInterventionIndicatorDetailsManagementSphere NewReportInterventionIndicatorDetailsManagementSphere()
		{
			return DataPortal.CreateChild<ReportInterventionIndicatorDetailsManagementSphere>();
		}

		public ReportInterventionIndicatorDetailsManagementSphere()
		{
			MarkAsChild();
		}

		internal static ReportInterventionIndicatorDetailsManagementSphere GetReportInterventionIndicatorDetailsManagementSphere(SafeDataReader dr)
		{
			var r = new ReportInterventionIndicatorDetailsManagementSphere();
			r.Fetch(dr);
			return r;
		}

		protected void Fetch(SafeDataReader sdr)
		{
			using (BypassPropertyChecks)
			{
				int i = 0;
				LoadProperty(QuestionnaireAnswerResultIDProperty, sdr.GetInt32(i++));
				LoadProperty(ManagementSphereIDProperty, sdr.GetInt32(i++));
				LoadProperty(ManagementSphereProperty, sdr.GetString(i++));
				LoadProperty(ManagementSphereContentProperty, sdr.GetString(i++));
				LoadProperty(IndicatorDetailNameProperty, sdr.GetString(i++));
				LoadProperty(CommentsProperty, sdr.GetString(i++));
				LoadProperty(NextStepsProperty, sdr.GetString(i++));
				LoadProperty(EvidenceProperty, sdr.GetString(i++));
			}

			MarkAsChild();
			MarkOld();
			BusinessRules.CheckRules();
		}

		protected override Action<SqlCommand> SetupSaveCommand(SqlCommand cm)
		{
			AddPrimaryKeyParam(cm, QuestionnaireAnswerResultIDProperty);

			cm.Parameters.AddWithValue("@ManagementSphereID", GetProperty(ManagementSphereIDProperty));
			cm.Parameters.AddWithValue("@ManagementSphere", GetProperty(ManagementSphereProperty));
			cm.Parameters.AddWithValue("@ManagementSphereContent", GetProperty(ManagementSphereContentProperty));
			cm.Parameters.AddWithValue("@IndicatorDetailName", GetProperty(IndicatorDetailNameProperty));
			cm.Parameters.AddWithValue("@Comments", GetProperty(CommentsProperty));
			cm.Parameters.AddWithValue("@NextSteps", GetProperty(NextStepsProperty));
			cm.Parameters.AddWithValue("@Evidence", GetProperty(EvidenceProperty));

			return (scm) =>
			{
				// Post Save
				if (this.IsNew)
				{
					LoadProperty(QuestionnaireAnswerResultIDProperty, scm.Parameters["@QuestionnaireAnswerResultID"].Value);
				}
			};
		}

		protected override void SaveChildren()
		{
			// No Children
		}

		//protected override void SetupDeleteCommand(SqlCommand cm)
		//{
		//	cm.Parameters.AddWithValue("@QuestionnaireAnswerResultID", GetProperty(QuestionnaireAnswerResultIDProperty));
		//}

		#endregion

	}

}