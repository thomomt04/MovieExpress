using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Csla;
using Singular.Web;

namespace MEWeb.Public
{
  /// <summary>
  /// The RegisterVM ViewModel class
  /// </summary>
  public class RegisterVM: MEStatelessViewModel<RegisterVM>
  {
    /// <summary>
    /// The Title Type enumeration
    /// </summary>
    public enum TitleType
    {
      Mr = 1,
      Mrs = 2,
      Miss = 3
    }

    #region Properties

    /// <summary>
    /// The User's Title PropertyInfo
    /// </summary>
    public static PropertyInfo<TitleType> mTitle = RegisterProperty<TitleType>(c => c.Title, "Title", TitleType.Mr);

    /// <summary>
    /// The User's First Name PropertyInfo
    /// </summary>
    public static PropertyInfo<string> mFirstName = RegisterProperty<string>(c => c.FirstName, "First Name", "");

    /// <summary>
    /// The User's Last Name PropertyInfo
    /// </summary>
    public static PropertyInfo<string> mLastName = RegisterProperty<string>(c => c.LastName, "Last Name", "");

    /// <summary>
    /// The User's Email Address PropertyInfo
    /// </summary>
    public static PropertyInfo<string> mEmailAddress = RegisterProperty<string>(c => c.EmailAddress, "EmailAddress", "");

    /// <summary>
    /// The User's Title
    /// </summary>
    [DisplayName("Title:"), Required(ErrorMessage = "Title is Required"), Singular.DataAnnotations.DropDownWeb(typeof(TitleType))]
    public TitleType Title { get; set; }

    /// <summary>
    /// The User's First Name
    /// </summary>
    [DisplayName("First Name:"), Required(ErrorMessage = "First Name is Required")]
    public string FirstName { get; set; }

    /// <summary>
    /// The User's Last Name
    /// </summary>
    [DisplayName("Last Name:"), Required(ErrorMessage ="Last Name is Required")]
    public string LastName { get; set; }

    /// <summary>
    /// The User's Email Address
    /// </summary>
    [DisplayName("Email Address:"), Required(ErrorMessage = "Email Address is Required")]
    public string EmailAddress { get; set; }


    #endregion

    #region Methods

    /// <summary>
    /// ToString PropertyInfo
    /// </summary>
    public static PropertyInfo<string> ToStringMI = RegisterReadOnlyProperty<string>(c => c.ToString(), f => f.FirstName == "" ? "New User" : f.FirstName);

    /// <summary>
    /// Converts the object to a string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return this.GetProperty(ToStringMI);
    }

    #endregion

    /// <summary>
    /// Handle a registration command
    /// </summary>
    /// <param name="Command">The command</param>
    /// <param name="CommandArgs">The command arguments</param>
    protected override void HandleCommand(string Command, Singular.Web.CommandArgs CommandArgs)
    {
      // Implement Register Logic here.
      AddMessage(MessageType.Warning, "Register", "Registration not yet supported.");
    }
  }
}
