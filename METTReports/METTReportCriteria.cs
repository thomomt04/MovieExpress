using System;
using System.Data;

namespace METTReports
{
	public class METTReportCriteria 
	{
		public METTReportCriteria()
		{
			
		}

		public class ProtectedAreaCriteria : Singular.Reporting.ReportCriteria
		{			
			public int protectedAreaID { get; set; }
		}

	}
}
