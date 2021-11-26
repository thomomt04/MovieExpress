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
  Public Class EmailAttachment
    Inherits Singular.Documents.DocumentProviderBase(Of EmailAttachment)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared EmailAttachmentIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.EmailAttachmentID, "Email Attachment", 0)
    ''' <Summary>
    ''' Gets the Email Attachment value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Email Attachment", Description:="System generated unique ID")> _
    Public ReadOnly Property EmailAttachmentID() As Integer
      Get
        Return GetProperty(EmailAttachmentIDProperty)
      End Get
    End Property

    Public Shared EmailIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.EmailID, "Email", CType(Nothing, Integer?))
    ''' <Summary>
    ''' Gets the Email value
    ''' </Summary>
    <Display(Name:="Email", Description:="Email to which this attachment belongs")> _
    Public ReadOnly Property EmailID() As Integer?
      Get
        Return GetProperty(EmailIDProperty)
      End Get
    End Property

    Public Shared AttachmentNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.AttachmentName, "Attachment Name", "")
    ''' <Summary>
    ''' Gets and sets the Attachment Name value
    ''' </Summary>
    <Display(Name:="Attachment Name", Description:="Name of the Attachment"),
    StringLength(255, ErrorMessage:="Attachment Name cannot be more than 255 characters")> _
    Public Property AttachmentName() As String
      Get
        Return GetProperty(AttachmentNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(AttachmentNameProperty, value)
      End Set
    End Property

    Public Shared AddressOfAttachmentProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.AddressOfAttachment, "Address Of Attachment", "")
    ''' <Summary>
    ''' Gets and sets the Address Of Attachment value
    ''' </Summary>
    <Display(Name:="Address Of Attachment", Description:="Address of attachment"),
    StringLength(512, ErrorMessage:="Address Of Attachment cannot be more than 512 characters")> _
    Public Property AddressOfAttachment() As String
      Get
        Return GetProperty(AddressOfAttachmentProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(AddressOfAttachmentProperty, value)
      End Set
    End Property

    Public Shared AttachmentDataProperty As PropertyInfo(Of Byte()) = RegisterProperty(Of Byte())(Function(c) c.AttachmentData, "AttachmentData")
    <Display(AutoGenerateField:=False)> _
    Public Property AttachmentData() As Byte()
      Get
        Return GetProperty(AttachmentDataProperty)
      End Get
      Set(ByVal Value As Byte())
        SetProperty(AttachmentDataProperty, Value)
      End Set
    End Property

    <Display(Name:="Document", Description:="Document", AutoGenerateField:=True),
    Singular.DataAnnotations.DocumentField(GetType(Singular.Documents.Document), "DocumentName"), Browsable(True)> _
    Public Overrides Property DocumentID As Integer?
      Get
				If GetProperty(DocumentIDProperty) Is Nothing AndAlso GetProperty(DocumentProperty) IsNot Nothing Then
					Return GetProperty(DocumentProperty).DocumentID
				End If
        Return GetProperty(DocumentIDProperty)
      End Get
      Set(value As Integer?)
        SetProperty(DocumentIDProperty, value)
      End Set
    End Property

    'Public Shared AttachmentDocumentIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.AttachmentDocumentID, "Attachment Document ID", 0)
    ' ''' <Summary>
    ' ''' Gets and sets the Attachment Name value
    ' ''' </Summary>
    '<Display(Name:="Attachment Document ID", Description:="Document of the Attachment")> _
    'Public Property AttachmentDocumentID() As Integer
    '  Get
    '    Return GetProperty(AttachmentDocumentIDProperty)
    '  End Get
    '  Set(ByVal Value As Integer)
    '    SetProperty(AttachmentDocumentIDProperty, Value)
    '  End Set
    'End Property

#End Region

#Region " Methods "

    Public Function GetParent() As Email

      Return CType(CType(Me.Parent, EmailAttachmentList).Parent, Email)

    End Function

    Public Sub LoadDoucment(Document As Documents.Document)
      LoadProperty(DocumentProperty, Document)
    End Sub

    Protected Overrides ReadOnly Property DocumentRequired As Boolean
      Get
        Return False
      End Get
    End Property

#If SILVERLIGHT Then
#Else
    Public Sub SetAttachmentDataFromFile(ByVal FileName As String)

      Dim bmp As System.Drawing.Bitmap = System.Drawing.Bitmap.FromFile(FileName)
      SetProperty(AttachmentDataProperty, bmp)

    End Sub
#End If

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(EmailAttachmentIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.AttachmentName.Length = 0 Then
        If Me.IsNew Then
          Return "New Email Attachment"
        Else
          Return "Blank Email Attachment"
        End If
      Else
        Return Me.AttachmentName
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

    Public Shared Function NewEmailAttachment(Document As Singular.Documents.Document) As EmailAttachment
      Dim obj = DataPortal.CreateChild(Of EmailAttachment)()
      While Document.GetEditLevel > 0
        Document.ApplyEdit()
      End While
      obj.Document = Document
      Return obj
    End Function

    Public Shared Function NewEmailAttachment(AttachmentName As String, AttachmentData() As Byte) As EmailAttachment
      Dim obj = DataPortal.CreateChild(Of EmailAttachment)()
      obj.AttachmentData = AttachmentData
      obj.AttachmentName = AttachmentName
      Return obj
    End Function

    Public Shared Function NewEmailAttachment() As EmailAttachment

      Return DataPortal.CreateChild(Of EmailAttachment)()

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

    Friend Shared Function GetEmailAttachment(ByVal dr As SafeDataReader) As EmailAttachment

      Dim e As New EmailAttachment()
      e.Fetch(dr)
      Return e

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(EmailAttachmentIDProperty, .GetInt32(0))
          LoadProperty(EmailIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
          LoadProperty(AttachmentNameProperty, .GetString(2))
          LoadProperty(AddressOfAttachmentProperty, .GetString(3))
          LoadProperty(AttachmentDataProperty, .GetValue(4))
          If sdr.FieldCount > 5 Then
            LoadProperty(DocumentIDProperty, .GetInt32(5))
            LoadProperty(DocumentNameProperty, AttachmentName)
          End If
          
        End With
      End Using

      MarkAsChild()
      MarkOld()

      BusinessRules.CheckRules()

    End Sub

    Public Overrides Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insEmailAttachment"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Public Overrides Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updEmailAttachment"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      CallSaveDocument()
      If Me.IsSelfDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramEmailAttachmentID As SqlParameter = .Parameters.Add("@EmailAttachmentID", SqlDbType.Int)
          paramEmailAttachmentID.Value = GetProperty(EmailAttachmentIDProperty)
          If Me.IsNew Then
            paramEmailAttachmentID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@EmailID", Me.GetParent.EmailID)
          .Parameters.AddWithValue("@AttachmentName", GetProperty(AttachmentNameProperty))
          .Parameters.AddWithValue("@AddressOfAttachment", GetProperty(AddressOfAttachmentProperty))
          Dim mAttachmentData = GetProperty(AttachmentDataProperty)
          If mAttachmentData Is Nothing OrElse mAttachmentData.Length = 0 Then
            .Parameters.AddWithValue("@AttachmentData", DBNull.Value).SqlDbType = SqlDbType.Image
          Else
            '.Parameters.AddWithValue("@AttachmentData", mAttachmentData).SqlDbType = SqlDbType.Image
            .Parameters.AddWithValue("@AttachmentData", mAttachmentData).SqlDbType = SqlDbType.VarBinary
          End If
          .Parameters.AddWithValue("@AttachmentDocumentID", Singular.Misc.ZeroNothingDBNull(Me.DocumentID))

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(EmailAttachmentIDProperty, paramEmailAttachmentID.Value)
          End If
          ' update child objects
          ' mChildList.Update()
          MarkOld()
        End With
      Else
        ' update child objects
        ' mChildList.Update()
      End If

    End Sub

    Protected Overrides Sub CallSaveDocument()
      SaveDocument()
    End Sub

    Public Overrides Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delEmailAttachment"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@EmailAttachmentID", GetProperty(EmailAttachmentIDProperty))
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