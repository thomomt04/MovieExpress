Namespace Data

  <Serializable()>
  Public Class FileManager

    Private mObjectTracker As ServerObjectTracker

    Public Sub New(ObjectTracker As ServerObjectTracker)
      mObjectTracker = ObjectTracker
    End Sub

    Public Class TempFile
      Public Property FileBytes As Byte()
      Public Property FileName As String
    End Class

    Private mTempFiles As New Hashtable

    Public Sub AddFile(ObjectGuid As Guid, FileName As String, FileBytes As Byte())

      Dim Obj As ServerObjectTracker.ServerObjectInfo = mObjectTracker.GetObjectInfo(ObjectGuid)
      If Obj IsNot Nothing Then
        CType(Obj.ListItem, Singular.Documents.IDocumentProvider).SetDocument(FileBytes, IO.Path.GetFileName(FileName))
      Else
        mTempFiles(ObjectGuid) = New TempFile With {.FileName = FileName, .FileBytes = FileBytes}
      End If

    End Sub

    Public Function GetFile(ObjectGuid As Guid) As TempFile

      Dim Obj As ServerObjectTracker.ServerObjectInfo = mObjectTracker.GetObjectInfo(ObjectGuid)
      If Obj IsNot Nothing Then
        Dim doc As Singular.Documents.IDocumentProvider = Obj.ListItem
        If Not doc.IsDownloaded Then
          doc.GetDocument()
        End If
        Return New TempFile() With {.FileBytes = doc.Document.Document, .FileName = doc.Document.DocumentName}
      Else
        Return mTempFiles(ObjectGuid)
      End If

    End Function

    Public Sub PopulateNewObjects()

      For Each key As Guid In mTempFiles.Keys

        Dim Obj As ServerObjectTracker.ServerObjectInfo = mObjectTracker.GetObjectInfo(key)
        If Obj IsNot Nothing Then

          Dim tf As TempFile = mTempFiles(key)
          CType(Obj.ListItem, Singular.Documents.IDocumentProvider).SetDocument(tf.FileBytes, IO.Path.GetFileName(tf.FileName))

        End If

      Next

      'Clear the temp holder.
      mTempFiles = New Hashtable

    End Sub

  End Class

End Namespace


