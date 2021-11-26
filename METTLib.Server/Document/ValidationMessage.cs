using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MELib.Document
{
	public class ValidationMessage
	{
		public string ErrorMessage { get; set; }
		public Csla.Rules.RuleSeverity MessageSeverity { get; set; }

		public DocumentEnums.MessageRelation MessageRelation { get; set; }
	}
}
