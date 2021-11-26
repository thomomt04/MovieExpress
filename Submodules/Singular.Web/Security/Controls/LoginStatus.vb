Namespace CustomControls

  Public Class LoginStatus
    Inherits Controls.CustomWebControl

    Public Property ChangePasswordUrl As String = ""
    Public Property LogoutUrl As String = "~/default.aspx"
    Public Property LoggedInAsText As String = "Logged in as "
    Public Property ChangePasswordText As String = "Change Password"
    Public Property LogInText As String = "Log In"
    Public Property LogoutText As String = "Logout"
    Public Property LoginUrl As String = System.Web.Security.FormsAuthentication.LoginUrl
    Public Property LockOpenImage As String = "~/Singular/Images/LockOpen.png"
    Public Property LockClosedImage As String = "~/Singular/Images/LockClosed.png"
    Public Property LockEditImage As String = "~/Singular/Images/LockEdit.png"
    Public Property LockLogoutImage As String = "~/Singular/Images/LockBreak.png"

    ''' <summary>
    ''' The Name of a Javascript function to call when the login button is clicked.
    ''' </summary>
    Public Property LoginJSFunction As String = ""

    Public Class MenuItem
      Public Property Text As String
      Public Property Link As String
      Public Property ImageURL As String
      Public Property IsSpacer As Boolean
    End Class

    Public Class MenuItemList
      Inherits List(Of MenuItem)

      Public Sub AddMenuItem(Text As String, Link As String, ImageURL As String)
        Dim mi As New MenuItem With {.Text = Text, .Link = Link, .ImageURL = ImageURL}
        Add(mi)
      End Sub

      Public Sub AddSpacer()
        Dim mi As New MenuItem With {.IsSpacer = True}
        Add(mi)
      End Sub

    End Class

    Public Event OnAddDropDownItems(sender As Object, e As MenuItemList)

    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)
      MyBase.Render(writer)

      If Singular.Security.HasAuthenticatedUser Then

        writer.Write("<div style='display:block;' data-ContextMenu='cmSecurity'>")
        writer.Write("<span  style='display:inline-block; vertical-align:middle;'>" & LoggedInAsText & CType(Singular.Security.CurrentIdentity, Security.IWebIdentity).LoginLabelHTML())
        writer.Write("</span><img src='" & Utils.URL_ToAbsolute(LockClosedImage) & "' style='vertical-align:middle;' class='LockBreak' />")
        writer.Write("</div>")

        writer.Write("<div id='cmSecurity' class='ContextMenu' style='text-align:right'>")
        Dim mil As New MenuItemList
        RaiseEvent OnAddDropDownItems(Me, mil)

        For Each mi As MenuItem In mil
          If mi.IsSpacer Then
            writer.Write("<div class=""spacer""><hr /></div>")
          Else
            writer.Write("<div onclick=""window.location.href = '" & Utils.URL_ToAbsolute(mi.Link) & "'"">" & mi.Text & "<img src='" & Utils.URL_ToAbsolute(mi.ImageURL) & "' style='vertical-align:middle;' class='LockEdit'  /></div>")
          End If
        Next

        'Change Password
        If ChangePasswordUrl <> "" Then
          writer.Write("<div onclick=""window.location.href = '" & Utils.URL_ToAbsolute(ChangePasswordUrl) & "'"">" & ChangePasswordText & "<img src='" & Utils.URL_ToAbsolute(LockEditImage) & "' style='vertical-align:middle;' class='LockEdit'  /></div>")
        End If

        'Logout
        If LogoutUrl.EndsWith("?SCmd=Logout") Then
          LogoutUrl = LogoutUrl.Substring(0, LogoutUrl.Length - "?SCmd=Logout".Length)
        End If
        If LogoutUrl.StartsWith("~") Then
          LogoutUrl = Utils.URL_ToAbsolute(LogoutUrl)
        End If
        writer.Write("<div onclick=""window.location.href = '" & LogoutUrl & "?SCmd=Logout'"">" & LogoutText & "<img src='" & Utils.URL_ToAbsolute(LockLogoutImage) & "' style='vertical-align:middle;'  class='LockBreak' /></div>")
        writer.Write("</div>")

      Else

        If LoginJSFunction = "" Then
          writer.Write("<a href='" & LoginUrl & "'>" & LogInText & "</a>")
        Else
          writer.Write("<a href='#' onclick='" & LoginJSFunction & "'>" & LogInText & "</a>")
        End If

        writer.Write("<img src='" & Utils.URL_ToAbsolute(LockOpenImage) & "' style='vertical-align:middle;' class='LockOpen' />")
      End If

    End Sub

  End Class

End Namespace


