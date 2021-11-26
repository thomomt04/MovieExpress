Imports System.Web
Imports System.Web.Services
Imports System.Text

Namespace WebServices

  Public Class HttpHandlerHelper


    Public Shared Sub AddGZipHeaders(context As System.Web.HttpContext)

      Try
        If HttpRuntime.UsingIntegratedPipeline Then
          Dim AcceptEncoding = context.Request.Headers("Accept-Encoding")
          If AcceptEncoding IsNot Nothing Then
            If AcceptEncoding.Contains("gzip") Then

              context.Response.Filter = New System.IO.Compression.GZipStream(context.Response.Filter, System.IO.Compression.CompressionMode.Compress)
              context.Response.Headers.Remove("Content-Encoding")
              context.Response.AppendHeader("Content-Encoding", "gzip")

            ElseIf AcceptEncoding.Contains("deflate") Then

              context.Response.Filter = New System.IO.Compression.DeflateStream(context.Response.Filter, System.IO.Compression.CompressionMode.Compress)
              context.Response.Headers.Remove("Content-Encoding")
              context.Response.AppendHeader("Content-Encoding", "deflate")

            End If

            ' Allow proxy servers to cache encoded and unencoded versions separately
            context.Response.AppendHeader("Vary", "Content-Encoding")
          End If
        End If
      Catch ex As Exception
        System.Web.HttpContext.Current.Response.StatusCode = 500
        context.Response.Write(Singular.Debug.RecurseExceptionMessage(ex))
      End Try

    End Sub

    Public Shared Sub HandleException(context As System.Web.HttpContext, ex As Exception, HandlerName As String, Optional AdditionalInfo As String = "")

      If Singular.Debug.IsCustomError(ex) Then

        Dim Result As New Singular.Web.Result(False, Singular.Debug.RecurseExceptionMessage(ex))
        Dim RestReponse As New RestResponse(Result)
        RestReponse.RenderResponse(context.Response, False)

      Else

        If ex.InnerException IsNot Nothing AndAlso TypeOf ex.InnerException Is HttpException Then
          ex = ex.InnerException
        End If

        If TypeOf ex Is HttpException Then
          context.Response.StatusCode = CType(ex, HttpException).GetHttpCode
        Else
          context.Response.StatusCode = 500
        End If

        context.Response.Write(Singular.Web.WebError.GetErrorText(ex, HandlerName, AdditionalInfo))

      End If

    End Sub

    Public Shared Sub PrepareRequest(context As System.Web.HttpContext)

      If context.Request.HttpMethod.ToUpper = "POST" Then
        Singular.Web.Security.ValidateCSRFToken(context.Request)
      End If

      Singular.Localisation.SetupRequest()

      AddGZipHeaders(context)

    End Sub

    Friend Shared Function GetArguments(context As HttpContext, ByRef ArgumentsObject As Object, ByRef ArgumentText As String) As Boolean

      Using Reader As New IO.StreamReader(context.Request.InputStream)
        'Read arguments from body of request. 
        ArgumentText = Reader.ReadToEnd()
      End Using

      If ArgumentText = "" Then
        'For GET methods, the above will return nothing, so see if there are any query string arguments.
        ArgumentsObject = New APIArgs(context.Request.QueryString)
      Else

        ArgumentsObject = System.Web.Helpers.Json.Decode(ArgumentText)
      End If

      If OnAjaxRequest IsNot Nothing Then
        Dim Args As New AjaxRequestArgs(ArgumentsObject)
        OnAjaxRequest.Invoke(Args)
        If Args.StopRequest Then
          context.Response.Write(Args.ReturnData)
          ArgumentText = Args.ReturnData
          Return False
        End If
      End If

      Return True

    End Function

  End Class

  Public MustInherit Class HttpHandlerBase
    Implements IHttpHandler

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
      Get
        Return False
      End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

      HttpHandlerHelper.PrepareRequest(context)
      Try
        Dim ReturnObj = Process(context, context.Items("RouteInfo"))
        If ReturnObj IsNot Nothing Then
          ReturnObj.RenderResponse(context.Response, False)
        End If
      Catch ex As Exception
        HttpHandlerHelper.HandleException(context, ex, Me.GetType.Name)
      End Try


    End Sub

    Protected MustOverride Function Process(context As HttpContext, RouteInfo As Routing.RouteValueDictionary) As RestResponse

  End Class

End Namespace


