Namespace WebServices

  Public Class StatelessHandler
    Implements System.Web.IHttpHandler
    Implements System.Web.SessionState.IReadOnlySessionState

    Public Shared Property DefaultTypeAssemblyName As String = ""
    Public Const StatelessHandlerPath As String = "Library/StatelessHandler"

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
      Get
        Return False
      End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

      Dim ArgumentText As String = ""

      Try

        HttpHandlerHelper.PrepareRequest(context)

        Dim ArgumentsObject As Object = Nothing
        If HttpHandlerHelper.GetArguments(context, ArgumentsObject, ArgumentText) Then

          If context.Items("RouteSettings") IsNot Nothing Then
            ProcessApiRoute(context, ArgumentsObject, ArgumentText)
          ElseIf context.Items("LibRouteInfo") IsNot Nothing Then
            ProcessLibRoute(context, ArgumentsObject)
          ElseIf context.Request.HttpMethod.ToUpper = "POST" Then
            ProcessPost(context, ArgumentsObject, ArgumentText)
          Else
            'Blank get
            DefaultResponse(context)
          End If

        End If

      Catch ex As Exception
        HttpHandlerHelper.HandleException(context, ex, "StatelessHandler", ArgumentText)
      End Try

    End Sub

    Private Sub ProcessPost(context As HttpContext, ArgumentsObject As Object, ArgumentText As String)

      context.Response.ContentType = "application/json"
      context.Response.ContentEncoding = System.Text.Encoding.UTF8

      If ArgumentsObject IsNot Nothing Then

        Dim Method As String = ArgumentsObject.Method

        'Old methods that require readonly session
        If Method.StartsWith("VM_") Then
          DataHandler.ProcessSessionRequest(context, ArgumentsObject)
        Else

          'Stateless methods
          Select Case Method
            Case "GetData"
              context.Response.Write(StatelessService.GetData(New GetDataArguments(ArgumentsObject)))
            Case "SaveData"
              context.Response.Write(StatelessService.SaveData(New PrepareSaveArguments(ArgumentsObject)))
            Case "Command"
              StatelessService.HandleCommand(New HandleCommandArguments(ArgumentsObject, ArgumentText)).RenderResponse(context.Response, True)
            Case "Delete"
              context.Response.Write(StatelessService.DeleteData(New DeleteArgs(ArgumentsObject)))
            Case "Login"
              context.Response.Write(StatelessService.Login(ArgumentsObject))
            Case "ProgressPoll"
              Dim ap = Singular.Web.AjaxProgress.GetInstance(ArgumentsObject.ProgressGuid)
              ap.WriteHTTPResponse(True, context)

          End Select

        End If

      Else
        context.Response.Write("{ ""Message"": ""No arguments specified, at minimum ?Type=xxxx query string must be provided."" }")
      End If

    End Sub

    Private Sub ProcessApiRoute(context As HttpContext, ArgumentsObject As Object, ArgumentText As String)

      Dim RSettings As RoutingSettings = context.Items("RouteSettings")
      Dim Args = New HandleCommandArguments(context.Items("MethodInfo"), ArgumentsObject, ArgumentText)

      StatelessService.HandleCommand(Args).RenderResponse(context.Response, If(context.Request.HttpMethod.ToUpper = "POST", RSettings.IncludeGuidsOnPOST, RSettings.IncludeGuidsOnGET))
    End Sub

    Private Sub ProcessLibRoute(context As HttpContext, ArgumentsObject As Object)

      Dim lri As LibRouteInfo = context.Items("LibRouteInfo")

      If lri.Method = "GetHTML" Then
        Dim controlText As String = CustomControls.AjaxControlLoader.GetHTML(lri.MainArg)
        If Not String.IsNullOrEmpty(controlText) Then
          controlText = controlText.Trim()
          context.Response.ContentType = "text/html"
          context.Response.Cache.SetExpires(Now.AddDays(180))
          context.Response.Cache.SetCacheability(HttpCacheability.Public)
          context.Response.Cache.SetLastModified(New Date(2000, 1, 1))
          context.Response.Write(controlText)
        Else
          context.Response.StatusCode = System.Net.HttpStatusCode.NotFound
        End If
      End If

    End Sub

    Private Sub DefaultResponse(context As HttpContext)
      context.Response.ContentType = "text/plain"
      context.Response.ContentEncoding = System.Text.Encoding.UTF8

      context.Response.Write("Singular Stateless Handler" & vbCrLf)
      context.Response.Write("Invalid or no arguments supplied")
      context.Response.StatusCode = 400
    End Sub

  End Class

End Namespace

