Namespace WebServices

  'Stateless upload
  Public MustInherit Class FileUploadHandler
    Implements System.Web.IHttpHandler
    Implements System.Web.SessionState.IReadOnlySessionState

    Public Class ResponseObject
      Public Property Success As Boolean = False
      Public Property Response As String = ""
      Public Property DocumentID As Integer
      Public Property KeyValue As String
      Public Property Data As Object
      Public ReadOnly Property ErrorText As String
        Get
          Return If(Success, "", Response)
        End Get
      End Property
    End Class

    Public Shared Property MaxRequestSizeKB As Integer = -1

    Public Overridable Function CreateSaveDocument(context As System.Web.HttpContext) As Singular.Documents.Document
      Return New Singular.Documents.Document()
    End Function

    Public Overridable Function SaveDocument(context As System.Web.HttpContext, FullPath As String, FileBytes As Byte(), DocumentGuid As Guid) As ResponseObject

      Dim FileName As String = IO.Path.GetFileName(FullPath)

      If context.Request.QueryString("ImgScaleParams") IsNot Nothing Then
        'Called from the image resizer control.
        Dim Params = context.Request.QueryString("ImgScaleParams").Split(",")
        FileBytes = GetScaledImage(FileBytes, Params(0), Params(1))
      End If

      Dim tDoc = CreateSaveDocument(context)
      tDoc.DocumentName = FileName
      tDoc.Document = FileBytes
      tDoc = Insert(context, tDoc)
      Return New ResponseObject With {.DocumentID = tDoc.DocumentID,
                                      .KeyValue = Singular.Web.Data.JS.ObjectInfo.SimpleMember.EncryptKey(tDoc.DocumentID, GetType(Singular.Documents.TemporaryDocument)),
                                      .Success = True,
                                      .Response = FileName}

    End Function

    Protected Overridable Function Insert(context As System.Web.HttpContext, Doc As Singular.Documents.Document) As Singular.Documents.Document

      Return Doc.Save()

    End Function

    Protected Overridable Function GetScaledImage(ImageBytes As Byte(), Width As Integer, Height As Integer) As Byte()
      Dim Resizer As New Singular.Imaging.Resizer(500, 500, Singular.Imaging.Resizer.ImageResizeMode.KeepOriginalAspect)
      Resizer.FitMode = Imaging.Resizer.ImageFitMode.Cover
      Return Resizer.GetResizedImage(ImageBytes)
    End Function

    Public Sub ProcessRequest(context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

      ProcessFileUpload(context, AddressOf SaveDocument)

    End Sub

    Friend Shared Sub ProcessFileUpload(Context As System.Web.HttpContext, SaveHandler As Func(Of System.Web.HttpContext, String, Byte(), Guid, ResponseObject))

      Dim ro As New ResponseObject

      Dim FileName As String = "?"
      Dim FileBytes As Byte() = Nothing
      Dim HasFile As Boolean = False
      Dim DataType As String = Context.Request.QueryString("Type")

      Try

        If Context.Request.Headers("X_FILENAME") IsNot Nothing Then
          'This was called using an ajax request.

          FileName = Context.Request.Headers("X_FILENAME")
          'the request body is the file.
          ReDim FileBytes(Context.Request.InputStream.Length - 1)
          Context.Request.InputStream.Read(FileBytes, 0, Context.Request.InputStream.Length)
          HasFile = True

        ElseIf Not String.IsNullOrEmpty(DataType) AndAlso DataType = "Base64" Then

          'Request contains base64 data, not binary.
          Context.Response.ContentType = "application/json"
          Context.Response.ContentEncoding = System.Text.Encoding.UTF8

          FileName = Context.Request.QueryString("FileName")
          Using sr As New IO.StreamReader(Context.Request.InputStream)
            Dim Base64Text = sr.ReadToEnd
            FileBytes = Convert.FromBase64String(Base64Text)
            HasFile = True
          End Using

        Else
          'This was called using an html form.
          FileName = Context.Request.QueryString("FileName")
          If Context.Request.Files IsNot Nothing And Context.Request.Files.Count > 0 Then
            Dim file = Context.Request.Files(0)

            'Read the stream into a byte array
            ReDim FileBytes(file.ContentLength - 1)
            file.InputStream.Read(FileBytes, 0, file.ContentLength)

            HasFile = True

          End If

        End If

        FileName = HttpUtility.UrlDecode(FileName)

        If HasFile Then

          If SaveHandler IsNot Nothing Then
            'Stateless
            ro = SaveHandler(Context, FileName, FileBytes, New Guid(CStr(Context.Request.QueryString("DocumentGuid"))))
            If String.IsNullOrEmpty(ro.Response) Then
              ro.Response = IO.Path.GetFileName(FileName)
            End If

          Else

            'Let the view model handle the file.
            Dim model As IViewModel = PageBase.GetModel(Context.Request.QueryString("PageGuid"), Context)
            If model Is Nothing Then
              'Session expired.
              ro.Response = "You have waited too long, please reload the page and try again."
            Else
              model.FileManager.AddFile(New Guid(CStr(Context.Request.QueryString("DocumentGuid"))), FileName, FileBytes)

              ro.Response = IO.Path.GetFileName(FileName)
              ro.Success = True
            End If

          End If

        Else
          ro.Response = "File did not make it to the server."
        End If

      Catch ex As Exception

        ro.Response = "Upload of file '" & FileName & "' failed."

        'Check why the upload failed.
        If ex.Message = "Maximum request length exceeded." Then

          'Request limit exceeded.
          If MaxRequestSizeKB = -1 Then
            ro.Response &= "The selected file was too large (" & Singular.Files.GetReadableSize(Context.Request.ContentLength) & ")."
          Else
            ro.Response &= "The max allowed size is " & Singular.Files.GetReadableSize(MaxRequestSizeKB * 1024) & ". The selected file was " & Singular.Files.GetReadableSize(Context.Request.ContentLength) & "."
          End If
        Else
          ro.Response &= "An unknown error occured."
        End If

        If Singular.Web.WebError.SupportsWebError Then Singular.Web.WebError.LogError(ex, "UploadFile")

      End Try

      Context.Response.Write(Singular.Web.Data.JSonWriter.SerialiseObject(ro))

    End Sub

    <Singular.Web.WebCallable>
    Public Overridable Function SaveBase64Data(FileName As String, Data As String) As ResponseObject
      Return SaveDocument(System.Web.HttpContext.Current, FileName, Convert.FromBase64String(Data), Nothing)
    End Function

    Public ReadOnly Property IsReusable As Boolean Implements System.Web.IHttpHandler.IsReusable
      Get
        Return True
      End Get
    End Property

  End Class

  'Session upload
  Public Class SessionFileUploadHandler
    Implements System.Web.IHttpHandler
    Implements System.Web.SessionState.IRequiresSessionState

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
      Get
        Return True
      End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
      FileUploadHandler.ProcessFileUpload(context, Nothing)
    End Sub
  End Class

End Namespace

