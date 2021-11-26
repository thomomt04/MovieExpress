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
  Public MustInherit Class DocumentProviderBase(Of C As DocumentProviderBase(Of C))
    Inherits SingularBusinessBase(Of C)
    Implements IDocumentProvider

    Public Event DocumentFetchCompleted(sender As Object, e As DocumentFetchCompletedArgs)
    Public Class DocumentFetchCompletedArgs
      Inherits EventArgs

    End Class


#Region " Properties and Methods "

#Region " Properties "

    Public Shared DocumentIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(f) f.DocumentID, "Document", CType(Nothing, Integer?))

#If SILVERLIGHT Then
    <Display(Name:="Document", Description:="Document", AutoGenerateFilter:=False),
    Singular.DataAnnotations.DocumentField(GetType(Singular.Documents.Document), "DocumentName")> _
    Public Overridable Property DocumentID As Integer? Implements Singular.Documents.IDocumentProvider.DocumentID
      Get
        Return GetProperty(DocumentIDProperty)
      End Get
      Set(ByVal value As Integer?)
        SetProperty(DocumentIDProperty, value)
      End Set
    End Property
#Else
    <Display(Name:="Document", Description:="Document", AutoGenerateFilter:=False, AutoGenerateField:=True),
    Singular.DataAnnotations.DocumentField(GetType(Singular.Documents.Document), "DocumentName"), Browsable(True)> _
    Public Overridable Property DocumentID As Integer? Implements Singular.Documents.IDocumentProvider.DocumentID
      Get
        Return GetProperty(DocumentIDProperty)
      End Get
      Set(ByVal value As Integer?)
        SetProperty(DocumentIDProperty, value)
      End Set
    End Property
#End If


    Public Shared DocumentNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(f) f.DocumentName, "Document Name", "")
		''' <Summary>
		''' Gets and sets the Document value
		''' </Summary>
#If SILVERLIGHT Then
    <Display(AutoGenerateField:=False, Name:="Document Name", Description:=""), Browsable(True)> _
    Public Overridable ReadOnly Property DocumentName() As String Implements Singular.Documents.IDocumentProvider.DocumentName
      Get
        If Document Is Nothing Then
          Return GetProperty(DocumentNameProperty)
        Else
          Return Document.DocumentName
        End If
      End Get
    End Property
#Else
		<Display(AutoGenerateField:=False, Name:="Document Name", Description:=""), Browsable(True)>
		Public Overridable ReadOnly Property DocumentName() As String Implements Singular.Documents.IDocumentProvider.DocumentName
			Get
				If Document Is Nothing Then
					Return GetProperty(DocumentNameProperty)
				Else
					Return Document.DocumentName
				End If
			End Get
		End Property
#End If


		Public Shared IsDownloadedProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(f) f.IsDownloaded, "Is Downloaded", False)
    ''' <Summary>
    ''' Gets IsDownloaded value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="IsDownloaded", Description:="")> _
    Public ReadOnly Property IsDownloaded() As Boolean Implements Singular.Documents.IDocumentProvider.IsDownloaded
      Get
        Return Document IsNot Nothing AndAlso Document.IsDownloaded
      End Get
    End Property

    Public Shared OverridesModifiedByProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(f) f.OverridesModifiedBy, "OverridesModifiedBy", CType(Nothing, Integer?))

    ''' <Summary>
    ''' Gets and sets the Document value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="OverridesModifiedBy", Description:="OverridesModifiedBy")> _
    Public Overridable Property OverridesModifiedBy As Integer? Implements IDocumentProvider.OverridesModifiedBy
      Get
        Return GetProperty(OverridesModifiedByProperty)
      End Get
      Set(value As Integer?)
        LoadProperty(OverridesModifiedByProperty, value)
      End Set
    End Property

#If SILVERLIGHT Then
#Else

    Public Shared ExistsOnServerProperty As Csla.PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(f) f.ExistsOnServer, "Exists On Server", -1)
		''' <summary>
		''' Indicates that the document has been transferred from the browser to the server.
		''' 0 = Its not on the server, 1 = It has always been on the server, 2 = It is busy transferring to the server, 3 = It has finished transferring from the client to the server.
		''' If the document was loaded from the database, it does exist (Not IsNew)
		''' If the document data is not null, it exists (IsDownloaded)
		''' Used by the Web Document uploader.
		''' </summary>
		<Display(AutoGenerateField:=False), Browsable(True)>
		Public Property ExistsOnServer As Integer Implements Documents.IDocumentProvider.ExistsOnServer
			Get
				Dim Value As Integer = GetProperty(ExistsOnServerProperty)
				If Value > 1 Then
					Return Value
				Else
					Return If(Not IsNew OrElse IsDownloaded, 1, 0)
				End If
			End Get
			Set(value As Integer)
				SetProperty(ExistsOnServerProperty, value)
			End Set
		End Property

		''' <summary>
		''' Removes the 'Document ready to be saved' message
		''' </summary>
		''' <remarks></remarks>
		Public Sub MarkSaved()
      ExistsOnServer = 1
    End Sub

    <Display(AutoGenerateField:=False), Browsable(True)>
    Public ReadOnly Property UploadPercent As Decimal Implements IDocumentProvider.UploadPercent
      Get
        Return If(Not IsNew OrElse IsDownloaded, 1, 0)
      End Get
    End Property

#End If

#End Region

#Region " Button Properties "

    Public Shared ButtonEnabledProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(f) f.ButtonEnabled, "ButtonEnabled", True)
    ''' <Summary>
    ''' Gets the Entity Document value
    ''' </Summary>
    <Display(AutogenerateField:=False)> _
    Public Property ButtonEnabled() As Boolean Implements Singular.Documents.IDocumentProvider.ButtonEnabled
      Get
        Return GetProperty(ButtonEnabledProperty)
      End Get
      Private Set(value As Boolean)
        LoadProperty(ButtonEnabledProperty, value)
        OnPropertyChanged(ButtonEnabledProperty)
      End Set
    End Property

    Public Shared ButtonTextProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(f) f.ButtonText, "ButtonText", "")
    ''' <Summary>
    ''' Gets the Entity Document value
    ''' </Summary>
    <Display(AutogenerateField:=False)> _
    Public Property ButtonText() As String Implements Singular.Documents.IDocumentProvider.ButtonText
      Get
        Return GetProperty(ButtonTextProperty)
      End Get
      Private Set(value As String)
        LoadProperty(ButtonTextProperty, value)
        OnPropertyChanged(ButtonTextProperty)
#If SILVERLIGHT Then
        OnPropertyChanged(SaveVisibleProperty)
        OnPropertyChanged(ButtonWidthProperty)
#End If

      End Set
    End Property
#If SILVERLIGHT Then

    Public Shared SaveVisibleProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(f) f.SaveVisible, "SaveVisible", False)
    ''' <Summary>
    ''' Gets the Entity Document value
    ''' </Summary>
    <Display(AutogenerateField:=False)> _
    Public ReadOnly Property SaveVisible() As Boolean
      Get

        Return IIf(GetProperty(ButtonTextProperty) = "Show" OrElse GetProperty(ButtonTextProperty) = "Export", True, False)

      End Get
    End Property

    Public Shared ButtonWidthProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(f) f.ButtonWidth, "ButtonWidth", 65)
    ''' <Summary>
    ''' Gets the Entity Document value
    ''' </Summary>
    <Display(AutogenerateField:=False)> _
    Public ReadOnly Property ButtonWidth() As Integer
      Get

        Return IIf(GetProperty(ButtonTextProperty) = "Show" OrElse GetProperty(ButtonTextProperty) = "Export", 40, 65)

      End Get
    End Property
#End If
#End Region

#Region " Methods "

#If SILVERLIGHT Then

    Protected Overrides Sub OnDeserialized(context As System.Runtime.Serialization.StreamingContext)
      MyBase.OnDeserialized(context)

      If Me.DocumentID IsNot Nothing AndAlso Me.DocumentID <> 0 AndAlso Not IsDownloaded Then
        Me.ButtonText = "Download"
      ElseIf Me.DocumentID IsNot Nothing AndAlso Me.DocumentID <> 0 AndAlso IsDownloaded AndAlso Documents.FileViewSupported(DocumentName) Then
        Me.ButtonText = "Show"
      ElseIf Me.DocumentID IsNot Nothing AndAlso Me.DocumentID <> 0 AndAlso IsDownloaded Then
        Me.ButtonText = "Download"
      Else
        Me.ButtonText = "-"
      End If

    End Sub

#Else

    Private mGetDocumentCallBack As System.Func(Of Byte())

    Public Sub SetDocumentName(DocumentName As String, GetDocumentCallBack As System.Func(Of Byte()))
      SetProperty(DocumentNameProperty, DocumentName)
      SetProperty(DocumentIDProperty, -1)
      MarkOld()
      mGetDocumentCallBack = GetDocumentCallBack
    End Sub

    Public Sub SetDocumentName(DocumentName As String)
      SetProperty(DocumentNameProperty, DocumentName)
    End Sub

    ''' <summary>
    ''' Sets the information required to retrieve the document from the database, and marks it old.
    ''' </summary>
    Public Sub SetDocument(DocumentID As Integer, DocumentName As String)
      SetProperty(DocumentIDProperty, DocumentID)
      SetProperty(DocumentNameProperty, DocumentName)
      'Me.Document.MarkOld()
      MarkOld()
    End Sub

#End If

    Public Sub SetDocument(Document As Byte(), DocumentName As String, Optional MarkOld As Boolean = False) Implements Singular.Documents.IDocumentProvider.SetDocument

      If Document Is Nothing Then
        Me.Document = Nothing
        SetProperty(DocumentNameProperty, "")
      Else
        Dim d As Singular.Documents.IDocument = Me.Document
        If d Is Nothing Then
          d = Documents.Document.NewDocument
        End If

        d.Document = Document
        d.DocumentName = DocumentName
        SetProperty(DocumentNameProperty, DocumentName)
        Me.Document = d
      End If

      If MarkOld Then
        Me.Document.MarkOld()
        Me.MarkOld()
      End If


    End Sub

    Public Sub ClearDocument()
      Me.Document.DocumentName = ""
      Me.Document.Document = {}
#If SILVERLIGHT Then
#Else
      Me.ExistsOnServer = 0
#End If
      SetProperty(DocumentNameProperty, "")
    End Sub

    Public Sub GetDocument() Implements Singular.Documents.IDocumentProvider.GetDocument

      Try
        If GetProperty(DocumentProperty) Is Nothing AndAlso Me.DocumentID IsNot Nothing AndAlso Me.DocumentID <> 0 Then
          ' we need to start loading it
          If Not Me.IsNew Then
            Me.MarkBusy()
#If SILVERLIGHT Then

            Me.ButtonEnabled = False
            Me.ButtonText = "Busy..."

            Document.BeginGetDocument(Me.DocumentID, Sub(o, e)
                                                       If e.Error IsNot Nothing Then
                                                         Throw e.Error
                                                       End If

                                                       Me.Document = e.Object
                                                       MarkIdle()
                                                       LoadProperty(DocumentNameProperty, e.Object.DocumentName)
                                                       OnPropertyChanged(DocumentNameProperty)
                                                       OnPropertyChanged(DocumentProperty)
                                                       RaiseEvent DocumentFetchCompleted(Me, New DocumentFetchCompletedArgs)
                                                     End Sub)
#Else
            If mGetDocumentCallBack IsNot Nothing Then
              SetDocument(mGetDocumentCallBack.Invoke(), DocumentName)
            Else
              Me.Document = Document.GetDocument(Me.DocumentID)
            End If

#End If
          End If

        End If
      Finally
        Me.MarkIdle()
      End Try


    End Sub

#If SILVERLIGHT Then
#Else

    Public Sub ResizeImage(Scale As Decimal)
      Dim NewImage As Byte() = GetImageThumbnail(Scale)
      If NewImage IsNot Nothing Then
        Me.SetDocument(NewImage, Me.DocumentName)
      End If
    End Sub

    Public Sub ResizeImage(Width As Integer, ScaleHeight As Boolean)
      Dim NewImage As Byte() = GetImageThumbnail(Width, ScaleHeight)
      If NewImage IsNot Nothing Then
        Me.SetDocument(NewImage, Me.DocumentName)
      End If
    End Sub

    Public Sub ResizeImage(ScaleWidth As Boolean, Height As Integer)
      Dim NewImage As Byte() = GetImageThumbnail(ScaleWidth, Height)
      If NewImage IsNot Nothing Then
        Me.SetDocument(NewImage, Me.DocumentName)
      End If
    End Sub

    Public Sub ResizeImage(Width As Integer, Height As Integer)
      Dim NewImage As Byte() = GetImageThumbnail(Width, Height)
      If NewImage IsNot Nothing Then
        Me.SetDocument(NewImage, Me.DocumentName)
      End If
    End Sub

    Public Function GetImageThumbnail(Scale As Decimal) As Byte()

      Dim extension As String = DocumentName.Substring(DocumentName.LastIndexOf(".") + 1).ToLower
      If {"jpeg", "jpg", "png", "bmp"}.Contains(extension) Then
        'It is an image
        Dim bmp As New Drawing.Bitmap(New IO.MemoryStream(Document.Document))

        Return GetImageThumbnail(CInt(bmp.Width * Scale), CInt(bmp.Height * Scale))

      End If

      Return Nothing
    End Function

    Public Function GetImageThumbnail(Width As Integer, ScaleHeight As Boolean) As Byte()

      Dim extension As String = DocumentName.Substring(DocumentName.LastIndexOf(".") + 1).ToLower
      If {"jpeg", "jpg", "png", "bmp"}.Contains(extension) Then
        'It is an image
        Dim bmp As New Drawing.Bitmap(New IO.MemoryStream(Document.Document))

        If ScaleHeight Then
          Dim Scale As Decimal = Width / bmp.Width
          Return GetImageThumbnail(CInt(bmp.Width * Scale), CInt(bmp.Height * Scale))
        Else
          Return GetImageThumbnail(CInt(Width), CInt(Width))
        End If

      End If

      Return Nothing
    End Function

    Public Function GetImageThumbnail(ScaleWidth As Boolean, Height As Integer) As Byte()

      Dim extension As String = DocumentName.Substring(DocumentName.LastIndexOf(".") + 1).ToLower
      If {"jpeg", "jpg", "png", "bmp"}.Contains(extension) Then
        'It is an image
        Dim bmp As New Drawing.Bitmap(New IO.MemoryStream(Document.Document))

        If ScaleWidth Then
          Dim Scale As Decimal = Height / bmp.Height
          Return GetImageThumbnail(CInt(bmp.Width * Scale), CInt(bmp.Height * Scale))
        Else
          Return GetImageThumbnail(CInt(Height), CInt(Height))
        End If

      End If

      Return Nothing
    End Function

    Public Function GetImageThumbnail(Width As Integer, Height As Integer) As Byte()

      Dim extension As String = DocumentName.Substring(DocumentName.LastIndexOf(".") + 1).ToLower
      If {"jpeg", "jpg", "png", "bmp"}.Contains(extension) Then
        'It is an image
        Dim format As Drawing.Imaging.ImageFormat = Nothing
        Select Case extension
          Case "jpeg", "jpg"
            format = Drawing.Imaging.ImageFormat.Jpeg
          Case "png"
            format = Drawing.Imaging.ImageFormat.Png
          Case "bmp"
            format = Drawing.Imaging.ImageFormat.Bmp
        End Select
        Dim bmp As New Drawing.Bitmap(New IO.MemoryStream(Document.Document))
        Dim thumb As New Drawing.Bitmap(bmp, Width, Height)

        Dim ms As New IO.MemoryStream()

        thumb.Save(ms, format)

        Return ms.ToArray()

      End If

      Return Nothing
    End Function

#End If

#End Region

#Region " Child Objects "

    Public Shared DocumentProperty As PropertyInfo(Of Singular.Documents.Document) = RegisterProperty(Of Document)(Function(f) f.Document, RelationshipTypes.Child)

    <Display(AutogenerateField:=False)> _
    Public Overridable Property Document As Singular.Documents.Document Implements Singular.Documents.IDocumentProvider.Document
      Get
        If FieldManager.FieldExists(DocumentProperty) Then
          Return GetProperty(DocumentProperty)
        Else
          Return Nothing
        End If
      End Get
      Protected Set(value As Singular.Documents.Document)
        SetProperty(DocumentProperty, value)
        If value Is Nothing Then
          Me.ButtonEnabled = False
          Me.ButtonText = "-"
          SetProperty(DocumentNameProperty, "")
        Else
          Me.ButtonEnabled = True
#If SILVERLIGHT Then
          If Me.Document.Document Is Nothing OrElse Me.Document.Document.Length = 0 Then
            Me.ButtonText = "-"
          ElseIf Documents.FileViewSupported(Me.DocumentName) Then
            Me.ButtonText = "Show"
          Else
            Me.ButtonText = "Export"
          End If
#Else
          Me.ButtonText = "Export"
#End If

          If value.GetEditLevel <> Me.GetEditLevel Then
            For i = value.GetEditLevel To Me.GetEditLevel - 1
              value.BeginEdit()
            Next
          End If
        End If
        PropertyHasChanged(DocumentNameProperty)
        'PropertyHasChanged(DocumentProperty)
      End Set
    End Property

#End Region

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()
      'BusinessRules.AddRule(New DocumentValid(DocumentProperty))
      BusinessRules.AddRule(New HasDocument(DocumentProperty, DocumentNameProperty))

#If Silverlight = False Then
      If DocumentRequired Then
        AddWebRule(DocumentNameProperty, Function(c) c.DocumentName = "", Function(c) "Please select a document").ClientOnly = True
      End If
#End If

    End Sub

    Protected Overridable ReadOnly Property DocumentRequired As Boolean
      Get
        Return True
      End Get
    End Property

    Private Class HasDocument
      Inherits Csla.Rules.BusinessRule

      Public Sub New(ByVal DocumentProperty As Csla.Core.IPropertyInfo, ByVal DocumentNameProperty As Csla.Core.IPropertyInfo)

        MyBase.New(DocumentProperty)
        Me.InputProperties = New List(Of Csla.Core.IPropertyInfo) From {PrimaryProperty, DocumentNameProperty}

      End Sub

      Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

        Dim Document As Singular.Documents.Document = context.InputPropertyValues(PrimaryProperty)
        Dim dp As C = context.Target
        If String.IsNullOrEmpty(dp.DocumentName) AndAlso dp.DocumentRequired Then
          context.AddErrorResult("Document is required")
        End If

      End Sub

    End Class

#End Region

#Region " Data Access & Factory Methods "

#If SILVERLIGHT Then

#Else

    ''' <summary>
    ''' Add this method and call 'SaveDocument' is it. Call this method from your InsertUpdate method
    ''' </summary>
    ''' <remarks></remarks>
    Protected MustOverride Sub CallSaveDocument()

    Protected Sub SaveDocument()

      If Document IsNot Nothing AndAlso Document.IsDirty Then
        Document.DirectSave()
        If Document.IsDeleted Then
          LoadProperty(DocumentIDProperty, Nothing)
          Document = Nothing
        Else
          LoadProperty(DocumentIDProperty, Document.DocumentID)
        End If
        AfterDocumentSaved(Document)
      End If

    End Sub

    Public Overridable Sub AfterDocumentSaved(Document As Document)

    End Sub

    Public MustOverride Sub Update()

    Public MustOverride Sub Insert()

    Public MustOverride Sub DeleteSelf()


#End If

#End Region

  End Class

End Namespace