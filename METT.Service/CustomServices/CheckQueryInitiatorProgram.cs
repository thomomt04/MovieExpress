using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Configuration;

namespace METT.Service.CustomServices
{
	public class CheckQueryInitiatorProgram : Singular.Service.ScheduleProgramBase
	{


		public enum ServiceStatuses
		{
			Running = 1,
			Stopping = 2,
			Stopped = 3
		}


		public CheckQueryInitiatorProgram(int ServerProgramTypeID, string Name) : base(ServerProgramTypeID, Name)
		{
			// Add any initialization after the InitializeComponent() call.
			//Singular.Settings.SetConnectionString(ConfigurationManager.AppSettings["ConnectionString"]);
		}

		protected override bool StartSchedule()
		{
			//Do any checks when service starts
			WriteProgress("Check Query Initiator Program Started.");
			return true;
		}

		protected override bool StopSchedule()
		{
			WriteProgress("Check Query Initiator Program Stopped.");
			// Add code here to perform any tear-down necessary to stop your service.
			WriteProgress("METTService has stopped");
			return true;
		}

		protected override void TimeUp()
		{
			try
			{
				RunScheduled();
			}
			catch (Exception ex)
			{
				WriteProgressDetail("Error: " + ex.Message, ProgressType.Warning);
			}
		}

		public enum CheckQueryStatus
		{
			[Description("Pending")]
			Pending = 1,
			[Description("Passed")]
			Passed = 2,
			[Description("Failed")]
			Failed = 3
		}


		public static void RunScheduled()
		{
			string FailedNames = "";
			bool ChecksFailed = false;
			bool StopTrading = false;

			dynamic ScheduledCheckQueriesList = Singular.CheckQueries.CheckQueryList.GetCheckQueriesList();

			//IFRS9Lib.Maintenance.SaxoSetting EmailList = IFRS9Lib.Maintenance.SaxoSettingList.GetSaxoSettingList().FirstAndOnly();

			foreach (Singular.CheckQueries.CheckQuery cq in ScheduledCheckQueriesList)
			{
				if (cq.Severity <= 3)
				{
					cq.Run();

					if (cq.Status == Singular.CheckQueries.CheckQueryStatus.Failed)
					{
						ChecksFailed = true;

						FailedNames += cq.FailDescription;

						if (cq.Severity == 1)
						{
							//Serious issue: Stop the trading engine
							StopTrading = true;
						}

					}
				}
			}

			if (ChecksFailed)
			{
				Singular.Emails.Email e = Singular.Emails.Email.NewEmail();
				e.ToEmailAddress = "mettAlerts@singular.co.za";
				e.FromEmailAddress = "mettAlerts@singular.co.za";

				if (StopTrading)
				{
					//WriteProgress("Major Problem with Check Queries... Attempting to Stop Trading Engine.")
					e.Subject = "!!! MAJOR CHECK QUERY PROBLEM !!!";
					e.Body = $"A Check Query with a severity of 'Major Problem' has failed for scheme Please review and close the market if necessary! {FailedNames}";

				}
				else
				{
					//Just sent an email asking the user to run the check queries.
					e.Subject = " Check Queries ";
					e.Body = $"A Check Query with a severity of 'Error' or higher has failed for scheme on lease run the check queries for more info. {FailedNames}";
				}

				Singular.Emails.EmailList el = Singular.Emails.EmailList.NewEmailList();
				el.Add(e);
				el.Save();
			}

		}

	}
}
