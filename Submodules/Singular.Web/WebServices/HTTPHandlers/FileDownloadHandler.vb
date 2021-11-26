Imports System.Web.Caching

Namespace WebServices

  Public MustInherit Class FileDownloadHandler
    Implements System.Web.IHttpHandler
    Implements System.Web.SessionState.IReadOnlySessionState

    Public Sub ProcessRequest(context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

      Try

        If context.Request.QueryString("DocumentHash") IsNot Nothing Then
          'Document Hash - get from database
          Dim Hash As String = context.Request.QueryString("DocumentHash")
          If Hash.Length = 32 OrElse Hash.Length = 64 Then
            Try
              Dim Doc As Singular.Documents.Document = Singular.Documents.Document.GetDocumentFromHash(Hash)
              SendFile(context.Response, Doc.DocumentName, Doc.Document, False, 180)
            Catch ex As Exception
              context.Response.StatusCode = 500
            End Try

          End If


        ElseIf context.Request.QueryString("TempGUID") IsNot Nothing Then
          'file guid - get from temp cache

          Dim Doc As Singular.Documents.IDocument = System.Web.HttpContext.Current.Cache.Remove(context.Request.QueryString("TempGUID"))
          SendFile(context.Response, Doc.DocumentName, Doc.Document, True, 180)

        ElseIf context.Request.HttpMethod.ToUpper = "POST" Then

          HandlePost(context)

        Else

          Dim VM As IViewModel = Nothing
          If context.Request.QueryString("PageGuid") IsNot Nothing Then
            VM = PageBase.GetModel(context.Request.QueryString("PageGuid"), context)
          End If

          If VM IsNot Nothing Then

            Select Case context.Request.QueryString("Type")

              Case "Document"
                SendDocument(context, VM, context.Request.QueryString("DocumentGuid"))
            End Select

          Else
            SendDocument(context)
          End If
        End If

      Catch ex As Exception

        context.Response.Write(Singular.Web.WebError.GetErrorText(ex, "DownloadHandler", context.Request.QueryString.ToString))

      End Try

    End Sub

    Private Sub HandlePost(Context As System.Web.HttpContext)

      Try
        Dim JSon As String = HttpUtility.UrlDecode(Context.Request.Form("post-data"))

        Dim args As New HandleCommandArguments(New BaseArgs With {
                                               .Type = Context.Request.Form("type"),
                                               .Args = System.Web.Helpers.Json.Decode(JSon),
                                               .CallMethod = Context.Request.Form("method-name")}, JSon)
        Dim Response = StatelessService.HandleCommand(args)
        Dim Document = Response.SuccessObject

        Dim Attachment As Boolean = args.JSonArgs.Inline Is Nothing

        Dim IDoc As Singular.Documents.IDocument = Document
        SendFile(Context.Response, IDoc.DocumentName, IDoc.Document, Attachment, , Context.Request.Form("fCookie"))

      Catch ex As Exception

        Dim ReturnText As String = Singular.Web.WebError.GetErrorText(ex, "[Post]DownloadFile")
        SendFile(Context.Response, "Error.txt", System.Text.Encoding.ASCII.GetBytes(ReturnText), True, , Context.Request.Form("fCookie"))

      End Try

    End Sub

    Protected Overridable Sub SendDocument(Context As System.Web.HttpContext)
      Dim IsAttachment As Boolean = Context.Request.QueryString("inline") Is Nothing

      Dim DocID As Integer? = Context.Request.QueryString("DocumentID")
      If Context.Request.QueryString("DocumentID") IsNot Nothing Then
        DocID = Context.Request.QueryString("DocumentID")
      ElseIf Context.Request.QueryString("ImageID") IsNot Nothing Then
        DocID = Context.Request.QueryString("ImageID")
        IsAttachment = False
      End If

      If DocID IsNot Nothing Then
        Dim Doc = GetDocument(Context, DocID)
        SendFile(Context.Response, Doc.DocumentName, Doc.Document, IsAttachment, If(IsAttachment, -1, 180))
      Else
        Context.Response.StatusCode = 500
      End If
    End Sub

    Protected Overridable Function GetDocument(Context As System.Web.HttpContext, DocumentID As Integer) As Singular.Documents.IDocument
      Return Singular.Documents.Document.GetDocument(DocumentID)
    End Function

    Private Sub SendDocument(Context As System.Web.HttpContext, Model As IViewModel, DocumentObjectGuid As String)

      Try
        Dim t As Data.FileManager.TempFile = Model.FileManager.GetFile(New Guid(DocumentObjectGuid))

        Dim IsAttachment As Boolean = Context.Request.QueryString("inline") Is Nothing

        SendFile(Context.Response, t.FileName, t.FileBytes, IsAttachment)
      Catch ex As Exception
        Throw New Exception("FileDownloader.SendDocument Failed.", ex)
      End Try

    End Sub

    'Private Sub SendCaptcha(Context As System.Web.HttpContext, Model As IViewModel)

    '  Dim c As Singular.Captcha = Model.GenerageCaptcha
    '  SendFile(Context.Response, "Captcha.jpg", c.GetJpg, False)

    'End Sub

    ''' <summary>
    ''' Sends a file to the browser.
    ''' </summary>
    ''' <param name="response">Page.Response or System.Web.HttpContext.Current.Response</param>
    ''' <param name="FileName">The name of the attachment. Must not be the full file path.</param>
    ''' <param name="FileBytes">The byte array of the file content.</param>
    ''' <param name="Attachment">True if this must act like a download, where the save dialog is shown. False if you are doing something like img.src=downloadhandler?imgid=123</param>
    Public Shared Sub SendFile(Response As System.Web.HttpResponse, FileName As String, FileBytes As Byte(), Attachment As Boolean, Optional CacheDays As Integer = -1, Optional CookieName As String = Nothing)

      If Response Is Nothing Then Throw New ArgumentException("Response is null")
      If FileName Is Nothing Then Throw New ArgumentException("FileName is null")
      If FileBytes Is Nothing Then Throw New ArgumentException("FileBytes is null")

      Dim IsCompressed As Boolean = Response.Filter IsNot Nothing AndAlso (TypeOf Response.Filter Is System.IO.Compression.GZipStream OrElse TypeOf Response.Filter Is System.IO.Compression.DeflateStream)

      If Not IsCompressed Then
        Response.ClearContent()
        Response.ClearHeaders()
      End If

      Try

        'This is so that Singular.js knows when the file has arrived, and can stop the loading bar.
        If Attachment Then
          Dim TickString As String = Now.Ticks.ToString
          Response.AppendCookie(New HttpCookie(If(CookieName, "FDToken"), TickString.Substring(TickString.Length - 8, 8)) With {.HttpOnly = False})
        End If

        Response.ContentType = GetMimeType(FileName)
        Response.AddHeader("content-disposition", If(Attachment, "attachment", "inline") & "; filename=" & FileName.Replace(" ", "").Replace(",", "").Replace(";", ""))
        If Not IsCompressed Then
          Response.AddHeader("Content-Length", FileBytes.Length)
        End If

      Catch ex As Exception
        Throw New Exception("Error setting headers")
      End Try

      If CacheDays > 0 Then
        Response.Cache.SetExpires(Now.AddDays(CacheDays))
        Response.Cache.SetCacheability(HttpCacheability.Public)
      End If

      Response.BinaryWrite(FileBytes)

      Try
        If Not IsCompressed Then
          Response.Flush()
          Response.SuppressContent = True
          HttpContext.Current.ApplicationInstance.CompleteRequest()
        End If
      Catch ex As Exception
        Throw New Exception("Error completing request")
      End Try


    End Sub

    ''' <summary>
    ''' Temporarily stores a file in the application cache. Used if you need to return the file guid in an ajax response, and then fetch the file directly afterwards.
    ''' </summary>
    ''' <param name="FileName">File name to be presented to the user.</param>
    ''' <param name="FileContents">File contents.</param>
    ''' <param name="StoreDuration">Amount of time in seconds to store the file. This should be as low as possible.</param>
    ''' <returns>Guid to be used to retrieve the file using Singular.DownloadTempFile(Guid)</returns>
    Public Shared Function SaveTempFile(FileName As String, FileContents As Byte(), Optional StoreDuration As Integer = 30, Optional Cache As System.Web.Caching.Cache = Nothing) As Guid

      Dim TempDoc As New Singular.Documents.Document(FileName, FileContents)
      Return SaveTempFile(TempDoc, StoreDuration, Cache)

    End Function

    ''' <summary>
    ''' Temporarily stores a file in the application cache. Used if you need to return the file guid in an ajax response, and then fetch the file directly afterwards.
    ''' </summary>
    ''' <param name="StoreDuration">Amount of time in seconds to store the file. This should be as low as possible.</param>
    ''' <returns>Guid to be used to retrieve the file using Singular.DownloadTempFile(Guid)</returns>
    Public Shared Function SaveTempFile(TempDoc As Singular.Documents.TemporaryDocument, Optional StoreDuration As Integer = 30, Optional Cache As System.Web.Caching.Cache = Nothing) As Guid
      Return SaveTempFile(TempDoc.Document, StoreDuration, Cache)
    End Function

    ''' <summary>
    ''' Temporarily stores a file in the application cache. Used if you need to return the file guid in an ajax response, and then fetch the file directly afterwards.
    ''' </summary>
    ''' <param name="StoreDuration">Amount of time in seconds to store the file. This should be as low as possible.</param>
    ''' <returns>Guid to be used to retrieve the file using Singular.DownloadTempFile(Guid)</returns>
    Public Shared Function SaveTempFile(TempDoc As Singular.Documents.IDocument, Optional StoreDuration As Integer = 30, Optional Cache As System.Web.Caching.Cache = Nothing) As Guid
      Dim CacheGuid = Guid.NewGuid
      Dim UseCache = If(Cache Is Nothing, System.Web.HttpContext.Current.Cache, Cache)
      UseCache.Add(CacheGuid.ToString, TempDoc, Nothing, Now.AddSeconds(StoreDuration), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
      Return CacheGuid
    End Function

    Public Shared Function GetMimeType(FileName As String) As String

      Select Case IO.Path.GetExtension(FileName.ToLower())
        Case ".txt"
          Return "text/plain"
        Case ".doc", ".docx"
          Return "application/ms-word"
          'Case ".xls", ".xlsx"
          '  Return "application/vnd.ms-excel"
        Case ".gif"
          Return "image/gif"
        Case ".jpg", ".jpeg"
          Return "image/jpeg"
        Case ".bmp"
          Return "image/bmp"
        Case ".png"
          Return "image/png"
        Case ".wav"
          Return "audio/wav"
        Case ".ppt"
          Return "application/mspowerpoint"
        Case ".pdf"
          Return "application/pdf"
        Case ".css"
          Return "text/css"
        Case ".js"
          Return "text/javascript"
        Case ".html"
          Return "text/html"
      End Select

      Return "application/octet-stream"

    End Function

    Public ReadOnly Property IsReusable As Boolean Implements System.Web.IHttpHandler.IsReusable
      Get
        Return True
      End Get
    End Property


  End Class

End Namespace

