Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CorrespondanceTemplates

  <Serializable()> _
  Public Class CorrespondanceTemplateAttachmentList
    Inherits Singular.Documents.DocumentProviderListBase(Of CorrespondanceTemplateAttachmentList, CorrespondanceTemplateAttachment)

#Region " Parent "

    <NotUndoable()> Private mParent As CorrespondanceTemplate
#End Region

#Region " Business Methods "

    Public Function GetItem(CorrespondanceTemplateAttachmentID As Integer) As CorrespondanceTemplateAttachment

      For Each child As CorrespondanceTemplateAttachment In Me
        If child.CorrespondanceTemplateAttachmentID = CorrespondanceTemplateAttachmentID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Correspondance Template Attachments"

    End Function

#End Region

#Region " Data Access "

#Region " Common "

    Public Shared Function NewCorrespondanceTemplateAttachmentList() As CorrespondanceTemplateAttachmentList

      Return New CorrespondanceTemplateAttachmentList()

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

    Friend Overloads Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As CorrespondanceTemplateAttachment In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As CorrespondanceTemplateAttachment In Me
          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace