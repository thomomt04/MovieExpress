﻿// Generated 31 Oct 2018 10:35 - Singular Systems Object Generator Version 2.2.693
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
	public class ROOrganisationType
	 : METTReadOnlyBase<ROOrganisationType>
	{
		#region " Properties and Methods "

		#region " Properties "

		public static PropertyInfo<int> OrganisationTypeIDProperty = RegisterProperty<int>(c => c.OrganisationTypeID, "ID", 0);
		/// <summary>
		/// Gets the ID value
		/// </summary>
		[Display(AutoGenerateField = false), Key, DisplayName("ID")]
		public int OrganisationTypeID
		{
			get { return GetProperty(OrganisationTypeIDProperty); }
		}

		public static PropertyInfo<String> OrganisationTypeProperty = RegisterProperty<String>(c => c.OrganisationType, "Organisationtype", "");
		/// <summary>
		/// Gets the Organisationtype value
		/// </summary>
		[Display(Name = "Organisationtype", Description = ""), DisplayName("Organisationtype")]
		public String OrganisationType
		{
			get { return GetProperty(OrganisationTypeProperty); }
		}

		#endregion

		#region " Methods "

		protected override object GetIdValue()
		{
			return GetProperty(OrganisationTypeIDProperty);
		}

		public override string ToString()
		{
			return this.OrganisationType;
		}

		#endregion

		#endregion

		#region " Data Access & Factory Methods "

		internal static ROOrganisationType GetROOrganisationType(SafeDataReader dr)
		{
			var r = new ROOrganisationType();
			r.Fetch(dr);
			return r;
		}

		protected void Fetch(SafeDataReader sdr)
		{
			int i = 0;
			LoadProperty(OrganisationTypeIDProperty, sdr.GetInt32(i++));
			LoadProperty(OrganisationTypeProperty, sdr.GetString(i++));
		}

		#endregion

	}

}