using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MELib.Document
{
	public class DocumentEnums
	{
		public enum DocumentTypes
		{
			Excel,
			Word,
			XML,
			JSon,
			PlainText,
			FIX, //FIX Protocal File
			Other
		}

		public enum MessageRelation
		{
			File,
			Data
		}
	}
}
