Namespace Documents

  Public Interface IDocument

    Property Document As Byte()
    Property DocumentName As String

    'ReadOnly Property IsDownloaded As Boolean
    'Sub Download(DownloadCompleted As Action(Of IDocument))

  End Interface

End Namespace