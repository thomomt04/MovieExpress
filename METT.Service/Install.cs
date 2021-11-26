using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;

namespace MMELA.Service
{
  [RunInstaller(true)]
  public class ProjectInstaller : System.Configuration.Install.Installer
  {
    ServiceInstaller serviceInstaller;
    ServiceProcessInstaller processInstaller;
    const string SchedulerServiceName = "METT.Service";


    public ProjectInstaller()
      : base()// Setup Base Class
    {

		
			//Instantiate Installers
			processInstaller = new ServiceProcessInstaller();
      serviceInstaller = new ServiceInstaller();

      //Configure Installers
      //processInstaller.Account = ServiceAccount.User;
			this.processInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			//processInstaller.Username = "Singular\CGunther"
			//processInstaller.Password = ""
			//Singular.SystemSettings.GetSystemSetting(Of CSLib.SystemSettings.CorrespondenceSettings).GetDecryptedOutgoingMailPassword.ToString

			serviceInstaller.StartType = ServiceStartMode.Manual;
      serviceInstaller.ServiceName = SchedulerServiceName;
      //serviceInstaller.ServicesDependedOn = New String() {"MSSQL$SQL2012"}

      //Add them to the list of installers to install
      Installers.Add(serviceInstaller);
      Installers.Add(processInstaller);
    }


		private static string serviceName()
		{
			dynamic configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetAssembly(typeof(ProjectInstaller)).Location);
			dynamic settings = configFile.AppSettings.Settings;

			if ((settings("ServiceName") == null))
			{
				return "METT.Service";
			}
			else
			{
				return settings("ServiceName").Value;
			}
		}
	}
}
