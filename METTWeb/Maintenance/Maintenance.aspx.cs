using Singular.Web.MaintenanceHelpers;

namespace MEWeb.Maintenance
{
  /// <summary>
  /// The Maintenance custom page class
  /// </summary>
  public partial class Maintenance : MEPageBase<MaintenanceVM>
  {
  }

  /// <summary>
  /// The MaintenanceVM ViewModel class
  /// </summary>
  public class MaintenanceVM: StatelessMaintenanceVM
  {
    /// <summary>
    /// Setup the ViewModel
    /// </summary>
    protected override void Setup()
    {
      base.Setup();

      // Add Maintenance pages here.
      MainSection mainSection = AddMainSection("General");
      mainSection.AddMaintenancePage<MELib.Maintenance.MovieGenreList>("Movie Genres");
            // Add more lists here for maintaining, e.g. Status List, Years or lookup tables used in the project
           // mainSection.AddMaintenancePage<MELib.Maintenance.ProductList>("Products");
         //  mainSection.AddMaintenancePage<MELib.Maintenance.CategoryList>("Categories");
        }
	}
}
