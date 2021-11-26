Imports Csla
Imports Csla.Serialization

Namespace Security

  <Serializable()>
  Public Class PrincipalGetter
    Inherits SingularReadOnlyBase(Of PrincipalGetter)

    Public Shared Sub BeginLogin(callback As EventHandler(Of DataPortalResult(Of PrincipalGetter)))
      Dim dp = New DataPortal(Of PrincipalGetter)()
      AddHandler dp.FetchCompleted, Sub(o, e)

                                      If e.[Object] IsNot Nothing Then
                                        Csla.ApplicationContext.User = e.[Object].User
                                      End If

                                      callback.Invoke(o, e)

                                    End Sub

      dp.BeginFetch()
    End Sub

    Public Shared UserProperty As PropertyInfo(Of IPrincipal) = RegisterProperty(Of IPrincipal)(Function(c) c.User)
    Public Property User() As IPrincipal
      Get
        Return ReadProperty(UserProperty)
      End Get
      Private Set(value As IPrincipal)
        LoadProperty(UserProperty, value)
      End Set
    End Property


    Public Shared ErrorProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Error)

    Public Property [Error]() As String
      Get
        Return GetProperty(ErrorProperty)
      End Get
      Private Set(value As String)
        LoadProperty(ErrorProperty, value)
      End Set
    End Property



#If SILVERLIGHT Then

#Else

    Private Sub DataPortal_Fetch()

      Dim Info As String = ""
      Try
        Dim p As Singular.Security.IPrincipal = Nothing

        If Not System.Web.HttpContext.Current.Request.IsAuthenticated Then
          'Try get user from cookie user name.

          Dim AuthCookie As System.Web.HttpCookie = System.Web.HttpContext.Current.Request.Cookies(".ASPXAUTH")
          If AuthCookie IsNot Nothing Then
            Dim UserName As String = System.Web.Security.FormsAuthentication.Decrypt(AuthCookie.Value).Name
            p = System.Web.HttpContext.Current.Cache.Get("UserToken" & UserName)
          End If
        End If

        If p Is Nothing Then
          Try
            p = Singular.Security.CurrentPrincipal()
          Catch ex As Exception
            Throw New Exception("Error retrieving Principal", ex)
          End Try
        End If

        If p Is Nothing Then
          Info = "Principal is Nothing"
        Else
          Info = "Principal: " & p.GetType.Name

          Dim i As New Singular.Security.Identity
          i.FromIIdentity(p.Identity)

          Me.User = New Singular.Security.Principal(i)
        End If


      Catch ex As Exception
        Me.Error = Info & " - " & Singular.Debug.RecurseExceptionMessage(ex)
      End Try

    End Sub

#End If
  End Class
End Namespace
