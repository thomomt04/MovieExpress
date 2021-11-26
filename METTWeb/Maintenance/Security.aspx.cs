using Singular.Web.Security;
using System;

namespace MEWeb.Maintenance
{
    /// <summary>
    /// The Security page class
    /// </summary>
    public partial class Security : MEPageBase<SecurityVM>
    {
        //internal static string GetSafeRedirectUrl(string v1, string v2)
        //{
        //    throw new NotImplementedException();
        //}
    }

    /// <summary>
    /// The SecurityVM ViewModel class
    /// </summary>
    public class SecurityVM: StatelessSecurityVM
  {
  }
}
