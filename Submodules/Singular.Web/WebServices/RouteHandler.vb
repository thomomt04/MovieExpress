Imports System.Web.Routing
Imports System.Reflection

Namespace WebServices

  Public MustInherit Class RoutingSettings

    ''' <summary>
    ''' The url from the base url for api calls. Eg. http://website/Api/GetUserList URLFolder would be 'Api'
    ''' </summary>
    Public Property URLFolder As String = "Api"

    ''' <summary>
    ''' If a subfolder will be including before the method name.
    ''' E.g. http://website/Api/GetUserList has no subfolder.
    ''' http://website/Api/Security/GetUserList has a subfolder called security. The GetUserList method will be in a class called Security.
    ''' </summary>
    Public Property HasSubFolder As Boolean = False

    ''' <summary>
    ''' When HasSubFolder is true, and you want common methods in the root Api folder, specify the default class here.
    ''' E.g. http://website/Api/GetUserList will map to {ApiNameSpace}.{DefaultClassName}.GetUserList
    ''' </summary>
    Public Property DefaultClassName As String = ""

    Public Property IncludeGuidsOnGET As Boolean = False

    Public Property IncludeGuidsOnPOST As Boolean = False

  End Class

  Public Class TypeRouteHandler
    Implements IRouteHandler

    Private mSettings As RoutingSettings
    Private mMethods As New Hashtable

    Private Sub New(Settings As RoutingSettings)
      mSettings = Settings

      'Get the type
      Dim SettingsType As Type = mSettings.GetType
      For Each apiType As Type In SettingsType.Assembly.GetTypes()
        If apiType.Namespace = SettingsType.Namespace Then

          For Each mi As MethodInfo In apiType.GetMethods(BindingFlags.Public Or BindingFlags.Static Or BindingFlags.Instance)
            If Settings.HasSubFolder Then
              mMethods(apiType.Name.ToLower & "." & mi.Name.ToLower) = mi
            End If
            If Not Settings.HasSubFolder OrElse Settings.DefaultClassName <> "" Then
              mMethods(mi.Name.ToLower) = mi
            End If
          Next

        End If
      Next
    End Sub

    Public Shared Sub RegisterRoutes(Settings As RoutingSettings)

      Dim RHInstance As New TypeRouteHandler(Settings)

      If Settings.HasSubFolder Then
        RouteTable.Routes.Add(New Route(Settings.URLFolder & "/{Class}/{Method}", RHInstance))
      End If
      If Not Settings.HasSubFolder OrElse Settings.DefaultClassName <> "" Then
        RouteTable.Routes.Add(New Route(Settings.URLFolder & "/{Method}", RHInstance))
      End If

    End Sub

    Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler

      requestContext.HttpContext.Items("RouteSettings") = mSettings

      Dim Key As String = ""
      If requestContext.RouteData.Values("Class") IsNot Nothing Then
        Key = requestContext.RouteData.Values("Class") & "."
      ElseIf mSettings.HasSubFolder AndAlso mSettings.DefaultClassName <> "" Then
        Key = mSettings.DefaultClassName & "."
      End If
      Key &= requestContext.RouteData.Values("Method")

      Dim mi As MethodInfo = mMethods(Key.ToLower)
      If mi IsNot Nothing Then
        requestContext.HttpContext.Items("MethodInfo") = mi
      End If

      Return New Singular.Web.WebServices.StatelessHandler

    End Function
  End Class

  Public Class StaticRouteHandler(Of HandlerType As IHttpHandler)
    Implements IRouteHandler

    Private mConstructor As Func(Of HandlerType)

    Public Sub New()
      mConstructor = Expressions.Expression.Lambda(Of Func(Of HandlerType))(Expressions.Expression.[New](GetType(HandlerType))).Compile()
    End Sub

    Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler

      requestContext.HttpContext.Items("RouteInfo") = requestContext.RouteData.Values

      Return mConstructor()
    End Function
  End Class

  Friend Class LibRouteInfo
    Public Property Method As String
    Public Property MainArg As String
  End Class

  Public Class LibraryRouteHandler
    Implements IRouteHandler

    Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler

      Dim lri As New LibRouteInfo
      lri.Method = requestContext.RouteData.Values("Method")
      lri.MainArg = requestContext.RouteData.Values("MainArg")
      requestContext.HttpContext.Items("LibRouteInfo") = lri

      Return New Singular.Web.WebServices.StatelessHandler

    End Function

  End Class

End Namespace


