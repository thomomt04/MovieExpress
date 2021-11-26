using Singular.Web;
using System;
using static MEWeb.Default;

namespace MEWeb
{
	/// <summary>
	/// The Default page class
	/// </summary>
	public partial class Default : MEPageBase<DefaultVM>
	{
		public class DefaultVM : MEStatelessViewModel<DefaultVM>
		{
		}
	}
}

