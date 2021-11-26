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
  Public Class CorrespondanceTemplateAttachment
    Inherits Singular.Documents.DocumentProviderBase(Of CorrespondanceTemplateAttachment)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared CorrespondanceTemplateAttachmentIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.CorrespondanceTemplateAttachmentID, "Correspondance Template Attachment", 0)
    ''' <Summary>
    ''' Gets the Correspondance Template Attachment value
    ''' </Summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property CorrespondanceTemplateAttachmentID() As Integer
      Get
        Return GetProperty(CorrespondanceTemplateAttachmentIDProperty)
      End Get
    End Property

    Public Shared CorrespondanceTemplateIDProperty As PropertyInfo(Of Nullable(Of Integer)) = RegisterProperty(Of Nullable(Of Integer))(Function(c) c.CorrespondanceTemplateID, "Correspondance Template", CType(Nothing, Nullable(Of Integer)))
    ''' <Summary>
    ''' Gets the Correspondance Template value
    ''' </Summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property CorrespondanceTemplateID() As Integer?
      Get
        Return GetProperty(CorrespondanceTemplateIDProperty)
      End Get
    End Property

#End Region

#Region " Methods "

    Public Function GetParent() As CorrespondanceTemplate

      Return CType(CType(Me.Parent, CorrespondanceTemplateAttachmentList).Parent, CorrespondanceTemplate)

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(CorrespondanceTemplateAttachmentIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.CorrespondanceTemplateID.ToString.Length = 0 Then
        If Me.IsNew Then
          Return String.Format("New {0}", "Correspondance Template Attachment")
        Else
          Return String.Format("Blank {0}", "Correspondance Template Attachment")
        End If
      Else
        Return Me.CorrespondanceTemplateID.ToString
      End If

    End Function

#End Region

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Shared Function NewCorrespondanceTemplateAttachment() As CorrespondanceTemplateAttachment

      Return DataPortal.CreateChild(Of CorrespondanceTemplateAttachment)()

    End Function

    Public Sub New()

      MarkAsChild()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetCorrespondanceTemplateAttachment(ByVal dr As SafeDataReader) As CorrespondanceTemplateAttachment

      Dim c As New CorrespondanceTemplateAttachment()
      c.Fetch(dr)
      Return c

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(CorrespondanceTemplateAttachmentIDProperty, .GetInt32(0))
          LoadProperty(CorrespondanceTemplateIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
          LoadProperty(DocumentIDProperty, Singular.Misc.ZeroNothing(.GetInt32(2)))
        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Public Overrides Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insCorrespondanceTemplateAttachment"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Public Overrides Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updCorrespondanceTemplateAttachment"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      CallSaveDocument()
      If Me.IsSelfDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramCorrespondanceTemplateAttachmentID As SqlParameter = .Parameters.Add("@CorrespondanceTemplateAttachmentID", SqlDbType.Int)
          paramCorrespondanceTemplateAttachmentID.Value = GetProperty(CorrespondanceTemplateAttachmentIDProperty)
          If Me.IsNew Then
            paramCorrespondanceTemplateAttachmentID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@CorrespondanceTemplateID", Me.GetParent.CorrespondanceTemplateID)
          .Parameters.AddWithValue("@DocumentID", GetProperty(DocumentIDProperty))

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(CorrespondanceTemplateAttachmentIDProperty, paramCorrespondanceTemplateAttachmentID.Value)
          End If
          ' update child objects
          ' mChildList.Update()
          MarkOld()
        End With
      Else
      End If

    End Sub

    Protected Overrides Sub CallSaveDocument()
      SaveDocument()
    End Sub

    Public Overrides Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delCorrespondanceTemplateAttachment"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@CorrespondanceTemplateAttachmentID", GetProperty(CorrespondanceTemplateAttachmentIDProperty))
        DoDeleteChild(cm)
      End Using

    End Sub

    Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)

      If Me.IsNew Then Exit Sub

      With cm
        .ExecuteNonQuery()
      End With
      MarkNew()

    End Sub

#End If

#End Region

#End Region
  End Class


End Namespace