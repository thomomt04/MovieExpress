Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Singular.DataAnnotations

Namespace Documents

  <Serializable()>
  Public MustInherit Class TemporaryDocumentBase(Of T As TemporaryDocumentBase(Of T))
    Inherits DocumentProviderBase(Of T)

    Public Sub New()

    End Sub

    Public Sub New(DocumentName As String, Document As Byte())
      SetDocument(Document, DocumentName)
    End Sub

    Public Shared Function NewTemporaryDocument(DocumentID As Integer, DocumentName As String) As T
      Dim td = Activator.CreateInstance(Of T)()
      td.SetDocument(DocumentID, DocumentName)
      Return td
    End Function

    <Key>
    Public Overrides Property DocumentID As Integer?
      Get
        Return MyBase.DocumentID
      End Get
      Set(value As Integer?)
        MyBase.DocumentID = value
      End Set
    End Property

    Public Shadows Property DocumentName As String
      Get
        Return MyBase.DocumentName
      End Get
      Set(value As String)
        MyBase.SetDocumentName(value)
      End Set
    End Property

    Public Function SaveToDatabase() As T
      Document = Document.Save
      DocumentID = Document.DocumentID
      Return Me
    End Function

    Protected Overrides Sub CallSaveDocument()
      Throw New NotSupportedException("Save not allowed on TemporaryDocument")
    End Sub

    Public Overrides Sub DeleteSelf()
      Throw New NotSupportedException("Delete not allowed on TemporaryDocument")
    End Sub

    Public Overrides Sub Insert()
      Throw New NotSupportedException("Insert not allowed on TemporaryDocument")
    End Sub

    Public Overrides Sub Update()
      Throw New NotSupportedException("Update not allowed on TemporaryDocument")
    End Sub

  End Class

  ''' <summary>
  ''' Allows temporary storage of documents in memory, for use with UI elements that use DocumentProviderBase.
  ''' </summary>
  <Serializable(), ProtectedKeySalt("TDocument")>
  Public Class TemporaryDocument
    Inherits TemporaryDocumentBase(Of TemporaryDocument)

    Public Sub New()

    End Sub

    Public Sub New(DocumentName As String, Document As Byte())
      MyBase.New(DocumentName, Document)
    End Sub

  End Class

  ''' <summary>
  ''' Allows temporary storage of documents in memory, for use with UI elements that use DocumentProviderBase.
  ''' </summary>
  <Serializable(), ProtectedKeySalt("TDocument")>
  Public Class TemporaryDocumentNotRequired
    Inherits TemporaryDocumentBase(Of TemporaryDocumentNotRequired)

    Public Sub New()

    End Sub

    Public Sub New(DocumentName As String, Document As Byte())
      MyBase.New(DocumentName, Document)
    End Sub

    Protected Overrides ReadOnly Property DocumentRequired As Boolean
      Get
        Return False
      End Get
    End Property

  End Class

  <Serializable>
  Public Class TemporaryDocumentUnprotected
    Inherits TemporaryDocumentBase(Of TemporaryDocumentUnprotected)

    Public Sub New()

    End Sub

    Public Sub New(DocumentName As String, Document As Byte())
      MyBase.New(DocumentName, Document)
    End Sub

    <Key, Singular.DataAnnotations.UnProtectedKey>
    Public Overrides Property DocumentID As Integer?
      Get
        Return MyBase.DocumentID
      End Get
      Set(value As Integer?)
        MyBase.DocumentID = value
      End Set
    End Property
  End Class

End Namespace


