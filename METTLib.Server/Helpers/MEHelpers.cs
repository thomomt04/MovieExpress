using Singular;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MELib.Helpers
{
	class MEHelpers
	{
		public static string DecryptStringDatabaseValue(object Value)
		{
			if (Value == null)
			{
				return "";
			}
			return Encryption.DecryptString(Convert.ToBase64String((byte[])Value));
		}


		public static string GetSingularDropdownValue(object obj,string propertyName)
		{
			var pi = obj.GetType().GetProperty(propertyName);
			var ddw = Singular.Reflection.GetAttribute<Singular.DataAnnotations.DropDownWeb>(pi);

			return ddw != null ? ddw.GetDisplayFromID(pi.GetValue(obj, null), obj).ToString() : "";
		}
	
	}
	public static class EnumExtensions
	{
		public static string Description(this Enum value)
		{
			var enumType = value.GetType();
			var field = enumType.GetField(value.ToString());
			var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute),
																								 false);
			return attributes.Length == 0
					? value.ToString()
					: ((DescriptionAttribute)attributes[0]).Description;
		}
	}
}
