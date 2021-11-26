Imports System.Web.HttpContext

Namespace CustomControls

  Public Class LoggedInNameControl
    Inherits Controls.CustomWebControl

    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)
      MyBase.Render(writer)

      If Not Singular.Security.HasAuthenticatedUser Then
        writer.Write("Not Logged In")
      Else
        writer.Write(CType(Singular.Security.CurrentIdentity, Security.IWebIdentity).LoginLabelHTML())

        'writer.WriteBeginTag("a")
        'writer.WriteAttribute("href", "#")
        'writer.WriteAttribute("data-toggle", "dropdown")
        'writer.WriteAttribute("class", "btndropdown")
        'writer.Write(">")

        'writer.WriteBeginTag("span")
        'writer.WriteAttribute("class", "caret")
        'writer.Write("/>")

        'writer.WriteEndTag("a")
      End If

    End Sub

  End Class

End Namespace