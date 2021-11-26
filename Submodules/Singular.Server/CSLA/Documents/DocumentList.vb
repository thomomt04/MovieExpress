Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Documents

  <Serializable()> _
  Public Class DocumentList
    Inherits SingularBusinessListBase(Of DocumentList, Document)

#Region " Business Methods "

    Public Function CreateZipFile(ZipFileName As String) As Document
      Dim DocList As New List(Of IDocument)
      For Each doc In Me
        DocList.Add(doc)
      Next
      Return Document.CreateZipFile(ZipFileName, DocList)
    End Function

    Public Function GetItem(DocumentID As Integer) As Document

      For Each child As Document In Me
        If child.DocumentID = DocumentID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Documents"

    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared DocumentIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DocumentID, "Document ID")

      <Display(AutoGenerateField:=False)> _
      Public Property DocumentID() As Integer
        Get
          Return ReadProperty(DocumentIDProperty)
        End Get
        Set(ByVal value As Integer)
          LoadProperty(DocumentIDProperty, value)
        End Set
      End Property

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewDocumentList() As DocumentList

      Return New DocumentList()

    End Function

    Public Sub New()

      ' must have parameterless constructor

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetDocumentList() As DocumentList

      Return DataPortal.Fetch(Of DocumentList)(New Criteria)

    End Function

    Public Shared Function GetDocumentList(DocumentID As Integer) As DocumentList

      Return DataPortal.Fetch(Of DocumentList)(New Criteria With {.DocumentID = DocumentID})

    End Function

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(Document.GetDocument(sdr))
      End While
      Me.RaiseListChangedEvents = True

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Singular.Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getDocumentList"
            If Not Singular.Misc.IsNullNothingOrEmpty(crit.DocumentID) AndAlso crit.DocumentID <> 0 Then
              cm.Parameters.AddWithValue("@DocumentID", crit.DocumentID)
            End If
            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    'Friend Sub Update()

    '  Me.RaiseListChangedEvents = False
    '  Try
    '    ' Loop through each deleted child object and call its Update() method
    '    For Each Child As Document In DeletedList
    '      Child.DeleteSelf()
    '    Next

    '    ' Then clear the list of deleted objects because they are truly gone now.
    '    DeletedList.Clear()

    '    ' Loop through each non-deleted child object and call its Update() method
    '    For Each Child As Document In Me
    '      If Child.IsNew Then
    '        Child.Insert()
    '      Else
    '        Child.Update()
    '      End If
    '    Next
    '  Finally
    '    Me.RaiseListChangedEvents = True
    '  End Try

    'End Sub

    'Protected Overrides Sub DataPortal_Update()

    '  UpdateTransactional(AddressOf Update)

    'End Sub

#End If

#End Region

#End Region

  End Class


End Namespace
