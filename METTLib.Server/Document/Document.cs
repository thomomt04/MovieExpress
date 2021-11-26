using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MELib.Document
{
	abstract public class Document
	{

		public Document(string documentLocation)
		{
			this.ValidationMessages = new List<ValidationMessage>();
			this.DocumentName = System.IO.Path.GetFileName(documentLocation);
			this.DocumentLocation = documentLocation;
			setDocumentType();
		}

		public Document(System.IO.Stream documentstream, string Filename)
		{
			this.ValidationMessages = new List<ValidationMessage>();
			this.DocumentName = Filename;
			this.DocumentLocation = Filename;
			setDocumentType();
		}

		private void setDocumentType()
		{
			switch (DocumentExtension)
			{
				case "xlsx":
				case "xls":
					DocumentType = DocumentEnums.DocumentTypes.Excel;
					break;
				case "doc":
				case "docx":
					DocumentType = DocumentEnums.DocumentTypes.Word;
					break;
				case "xml":
					DocumentType = DocumentEnums.DocumentTypes.XML;
					break;
				case "json":
					DocumentType = DocumentEnums.DocumentTypes.JSon;
					break;
				case "txt":
					DocumentType = DocumentEnums.DocumentTypes.PlainText;
					break;
				case "fix":
					DocumentType = DocumentEnums.DocumentTypes.FIX;
					break;
				default:
					DocumentType = DocumentEnums.DocumentTypes.Other;
					break;
			}
		}

		public string DocumentName { get; }
		public DocumentEnums.DocumentTypes DocumentType { get; private set; }

		public string DocumentLocation { get; }

		public string DocumentExtension
		{
			get
			{
				return System.IO.Path.GetExtension(DocumentLocation).Replace(".", "").ToLower();
			}
		}

		public List<ValidationMessage> ValidationMessages { get; set; }

		public abstract bool IsValidFile();

		public abstract void SaveData();
		
		public virtual void addValidationMessage(string ErrorMessage, Csla.Rules.RuleSeverity MessageSeverity, DocumentEnums.MessageRelation MessageRelation)
		{
			ValidationMessage validationMessage = new ValidationMessage();

			validationMessage.ErrorMessage = ErrorMessage;
			validationMessage.MessageSeverity = MessageSeverity;
			validationMessage.MessageRelation = MessageRelation;

			ValidationMessages.Add(validationMessage);
		}

		public string ToString()
		{
			return DocumentName;
		}

	}

}
