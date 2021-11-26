using System;
using Singular;
using Singular.Web.Security;

namespace MELib.Security
{
  /// <summary>
  /// METTWebSecurity class
  /// </summary>
  [Serializable]
  public class MEWebSecurity: WebSecurity<WebPrinciple<MEIdentity>, MEIdentity>
  {
    /// <summary>
    /// Generate a hash for a password
    /// </summary>
    /// <param name="plainTextPassword">The password to hash</param>
    /// <returns>The hashed password</returns>
    public static string GetPasswordHash(string plainTextPassword)
    {
      return Encryption.GetStringHash(plainTextPassword, Encryption.HashType.Sha256);
    }

    public static void SetupConsoleAppPrinciple(int userID, int authorisedUserID, string name)
    {
      MEIdentity ident = new MEIdentity();
      ident.MakeConsolAppUser(userID, authorisedUserID, name);

      Singular.Web.Security.WebPrinciple<MEIdentity> _ServicePrinciple = new Singular.Web.Security.WebPrinciple<MEIdentity>(ident);
      System.Threading.Thread.CurrentPrincipal = _ServicePrinciple;

      //Singular.Security.Security.SetGetIdentityDelegate(() =>
      //{
      //  return _ServicePrinciple;
      //});

    }
  }
}
