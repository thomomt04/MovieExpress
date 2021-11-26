Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Emails

  <Serializable()> _
  Public Class EmailAttachmentList
    Inherits Singular.Documents.DocumentProviderListBase(Of EmailAttachmentList, EmailAttachment)

#Region " Parent "

    <NotUndoable()> Private mParent As Email
#End Region

#Region " Business Methods "

    Public Function GetAllAttachments() As System.IO.Stream()

      If Me.Count = 0 Then Return Nothing

      Dim stream(Me.Count - 1) As System.IO.MemoryStream
      For i As Integer = 0 To Me.Count - 1
        stream(i) = New System.IO.MemoryStream(Me(i).AttachmentData)
      Next
      Return stream

    End Function

    Public Function GetAllFileNames() As String()

      If Me.Count = 0 Then Return Nothing

      Dim FileNames(Me.Count - 1) As String
      For i As Integer = 0 To Me.Count - 1
        FileNames(i) = Me(i).AttachmentName
      Next
      Return FileNames

    End Function

    Public Function GetItem(EmailAttachmentID As Integer) As EmailAttachment

      For Each child As EmailAttachment In Me
        If child.EmailAttachmentID = EmailAttachmentID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Email Attachments"

    End Function

#End Region

#Region " Data Access "

#Region " Common "

    Public Shared Function NewEmailAttachmentList() As EmailAttachmentList

      Return New EmailAttachmentList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

Public Sub New()

' require use of MobileFormatter

End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(EmailAttachment.GetEmailAttachment(sdr))
      End While
      Me.RaiseListChangedEvents = True

    End Sub

    'Friend Sub Update()

    '  Me.RaiseListChangedEvents = False
    '  Try
    '    ' Loop through each deleted child object and call its Update() method
    '    For Each Child As EmailAttachment In deletedList
    '      Child.DeleteSelf()
    '    Next

    '    ' Then clear the list of deleted objects because they are truly gone now.
    '    deletedList.Clear()

    '    ' Loop through each non-deleted child object and call its Update() method
    '    For Each Child As EmailAttachment In Me
    '      If child.IsNew Then
    '        child.Insert()
    '      Else
    '        child.Update()
    '      End If
    '    Next
    '  Finally
    '    Me.RaiseListChangedEvents = True
    '  End Try

    'End Sub

#End If

#End Region

#End Region

  End Class


End Namespace