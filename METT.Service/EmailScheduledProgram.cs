using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Singular.Emails;

namespace METT.Service
{
	class EmailScheduledProgram : Singular.Service.EmailScheduleBase
	{
		public EmailScheduledProgram()
			: base("Email Sender", 2)
		{ }

		protected override Singular.Emails.SingularMailSettings.MailCredential DefaultMailCredential()
		{
			return new Singular.Emails.SingularMailSettings.MailCredential()
			{
				FromServer = ConfigurationManager.AppSettings["FromServer"],
				FriendlyFrom = ConfigurationManager.AppSettings["FriendlyFrom"],
				FromAccount = ConfigurationManager.AppSettings["FromAccount"],
				FromAddress = ConfigurationManager.AppSettings["FromAddress"],
				FromPassword = ConfigurationManager.AppSettings["FromPassword"]
			};
		}

		protected override void TimeUp()
		{
			Singular.Emails.SingularMail.AddEmailFooterImage += AddEmailFooterImage;
			try
			{
				EmailList EmailList = Singular.Emails.EmailList.GetEmailList(false);
				if (EmailList.Count > 0)
				{
					var mailList = new List<Singular.Emails.SingularMailSettings.MailCredential>();
					mailList.Add(DefaultMailCredential());
					EmailList.SendEmails(mailList);
					if (EmailList.FailedCount > 0)
					{
						WriteProgress("Sent " + EmailList.SentCount + " Emails, " + EmailList.FailedCount + " Failed to Send");
					}
					EmailList.Save();
				}
			}
			catch (Exception ex)
			{
				WriteProgress("Error Sending Emails: (" + ex.Source + ") " + ex.Message + " | " + ex.StackTrace);
				throw;
			}
			finally
			{
				Singular.Emails.SingularMail.AddEmailFooterImage -= AddEmailFooterImage;
			}
		}

		protected override void AddEmailFooterImage(object sender, SingularMail.AddEmailFooterImageEventArgs e)
		{
			base.AddEmailFooterImage(sender, e);
		}
	
}
}
