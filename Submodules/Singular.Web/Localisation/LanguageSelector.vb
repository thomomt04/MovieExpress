Imports System.Web.HttpContext
Imports System.Globalization

Namespace CustomControls

  Public Class LanguageSelector
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of Object)

    Const PostBackKey As String = "ChangeLanguage"

    Public Shared ReadOnly Property UserCulture() As CultureInfo
      Get
        If Current.Session("Singular.Web.UserCulture") Is Nothing Then
          If Current.Request.Cookies("Language") IsNot Nothing Then
            Dim CookieValue = Current.Request.Cookies("Language").Value
            Dim Language = Singular.Localisation.SupportedLanguages.Values.Where(Function(c) c.CultureCode = CookieValue).FirstOrDefault
            If Language IsNot Nothing Then
              Current.Session("Singular.Web.UserCulture") = Singular.Localisation.CreateLanguageCulture(CookieValue)
            Else
              Current.Session("Singular.Web.UserCulture") = System.Threading.Thread.CurrentThread.CurrentCulture
            End If

          Else
            Current.Session("Singular.Web.UserCulture") = System.Threading.Thread.CurrentThread.CurrentCulture
          End If
        End If
        Return Current.Session("Singular.Web.UserCulture")
      End Get
    End Property

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      If Visible Then
        With Writer

          Attributes.Add("value", Singular.Localisation.CurrentCulture.TwoLetterISOLanguageName)
          'Attributes.Add("onchange", "Singular.DoPostBack('" & PostBackKey & "', this.value)")
          Attributes.Add("onchange", "window.location='?SCmd=ChangeLanguage&Language=' + this.value")

          WriteFullStartTag("select", Singular.Web.Controls.HelperControls.TagType.IndentChildren)

          For Each language In Singular.Localisation.SupportedLanguages

            .WriteBeginTag("option")
            .WriteAttribute("value", language.Value.CultureCode)
            If language.Value.CultureCode = UserCulture.TwoLetterISOLanguageName Then
              .WriteAttribute("selected", "selected")
            End If
            .WriteCloseTag(False)

            .Write(language.Value.Language)

            .WriteEndTag("option")

          Next

          .WriteEndTag("select")

        End With
      End If
      
    End Sub

    Public Shared Sub SetCulture(Culture As CultureInfo)
      Current.Session("Singular.Web.UserCulture") = Culture

      Dim Cookie = Current.Response.Cookies("Language")
      If Cookie Is Nothing Then
        Cookie = New HttpCookie("Language")
        Current.Response.Cookies.Add(Cookie)
      End If
      Cookie.Value = Culture.TwoLetterISOLanguageName
      Cookie.Expires = Now.AddMonths(12)
    End Sub

  End Class

End Namespace


