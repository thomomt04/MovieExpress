Public Module Web

  Public Const ScriptsVersion = "1.2.214"

  ''' <summary>
  ''' True indicates that the web application is hosted accross multiple servers.
  ''' </summary>
  Public Property IsServerFarm As Boolean = False

End Module

Public Class Utils

  Public Shared URL_ToAbsolute As Func(Of String, String) = AddressOf VirtualPathUtility.ToAbsolute

  Public Shared Server_MapPath As Func(Of String, String) = Function(RelativePath)
                                                              Return HttpContext.Current.Server.MapPath(RelativePath)
                                                            End Function


End Class