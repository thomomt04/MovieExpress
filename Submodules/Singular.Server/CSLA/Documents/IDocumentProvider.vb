Namespace Documents

  Public Interface IDocumentProviderBasic
    Inherits ISingularBusinessBase

    Property DocumentID As Integer?
#If SILVERLIGHT Then
    ReadOnly Property DocumentName As String
#Else
    <Singular.DataAnnotations.FileInput()> ReadOnly Property DocumentName As String
    Property ExistsOnServer As Integer
    ReadOnly Property UploadPercent As Decimal
#End If

  End Interface

  Public Interface IDocumentProvider
    Inherits IDocumentProviderBasic

    Property Document As Document
    Sub SetDocument(Document As Byte(), DocumentName As String, Optional MarkOld As Boolean = False)
    Sub GetDocument()
    ReadOnly Property IsDownloaded As Boolean
    Property ButtonEnabled As Boolean
    Property ButtonText As String
    Property OverridesModifiedBy As Integer?

  End Interface

End Namespace
