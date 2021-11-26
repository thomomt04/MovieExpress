using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace METT.Service
{
	class Program : Singular.Service.ServiceBase
	{
		public const string SchedulerVersionNo = "1.0.0";

		protected override void AddServerPrograms()
		{
			Singular.Service.LogFile.LoggingEnabled = false;
			this.AddProgram(new SystemProgram());

			var EmailScheduledProgram = new EmailScheduledProgram();
			this.AddProgram(EmailScheduledProgram);
			
			var CheckQueries = new METT.Service.CustomServices.CheckQueryInitiatorProgram(5, "CheckQuery");
			this.AddProgram(CheckQueries);

			//// FOR DEBUG PURPOSES ONLY
			//EmailScheduledProgram.TestTimeUp();

			System.Globalization.CultureInfo newCI = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
			newCI.NumberFormat.CurrencyDecimalSeparator = ".";
			newCI.NumberFormat.NumberDecimalSeparator = ".";
			newCI.NumberFormat.PercentDecimalSeparator = ".";
			newCI.NumberFormat.NumberGroupSeparator = " ";

			System.Threading.Thread.CurrentThread.CurrentCulture = newCI;

			newCI = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentUICulture.Clone();
			newCI.NumberFormat.CurrencyDecimalSeparator = ".";
			newCI.NumberFormat.NumberDecimalSeparator = ".";
			newCI.NumberFormat.PercentDecimalSeparator = ".";
			newCI.NumberFormat.NumberGroupSeparator = " ";

			System.Threading.Thread.CurrentThread.CurrentUICulture = newCI;

		}

		//protected override void PreSetup()
		//{
		//	base.PreSetup();

		//	var smslist = Singular.SmsSending.SmsList.NewSmsList();
		//	var sms = new IFRS9Lib.BusineIFRS9bjects.Notification.SMSEncrypted();
		//	smslist.Add(sms);

		//}

		protected override string ServiceDescription
		{
			get { return "METT Service"; }
		}

		protected override string VersionNo
		{
			get { return SchedulerVersionNo; }
		}

		//protected override bool HideConnectionPassword()
		//{
		//	return true;
		//}

		//protected override void OnStart(string[] args)
		//{
		//	//System.Threading.Thread.Sleep(10000);
		//	base.OnStart(args);
		//}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
						{
								new Program()
						};
			ServiceBase.Run(ServicesToRun);

			//Program T = new Program();
			//T.OnStart(new string[] { });
		}
	}
}
