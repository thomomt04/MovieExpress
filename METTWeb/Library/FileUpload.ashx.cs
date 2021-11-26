using MEWeb;
using System;

namespace MEWeb.Library
{
  /// <summary>
  /// The FileUpload handler web handler class
  /// </summary>
  public class FileUpload : Singular.Web.WebServices.FileUploadHandler
  {
    public class DPFileUploadResponse : Singular.Web.WebServices.FileUploadHandler.ResponseObject
    {
      public string DocumentName { get; set; }
      public string DocumentFriendlyName { get; set; }
      public string DocumentKey { get; set; }
      public bool DocumentBlobStorageInd { get; set; }
      public bool DocumentBlobStoragePublicInd { get; set; }
      public bool DocumentImageInd { get; set; }
      public bool DocumentAlreadyExistsInd { get; set; }
    }

    public string mDocumentID { get; set; }
    public int DocumentID { get { return int.Parse(mDocumentID); } }
    public string mDocumentData { get; set; }

    public int DocumentTypeID { get; set; }

    //public override Singular.Web.WebServices.FileUploadHandler.ResponseObject SaveDocument(System.Web.HttpContext context, string FullPath, byte[] FileBytes, Guid DocumentGuid)
    //{
    //  //return DocumentImportsVM.ImportExcel(FileBytes, FullPath);
    //}
  }
}
