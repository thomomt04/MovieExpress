Imports System.Web.HttpContext

Namespace CustomControls

  Public Class MegaMenu
    Inherits System.Web.UI.WebControls.WebControl

    Public Property SiteMapDatasourceID As String
    Public Property DivClass As String = "black"

    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)

      writer.WriteBeginTag("div")
      writer.WriteAttribute("class", DivClass)
      writer.Write(">")

      Dim smds As SiteMapDataSource = FindControl(SiteMapDatasourceID)
      RenderSubMenu(writer, CType(smds, Singular.Web.CustomControls.SiteMapDataSource).GetHierarchicalView.Select, True)

      writer.WriteEndTag("div")

      Dim sb As New System.Text.StringBuilder
      sb.AppendLine("$(document).ready(function ($) {")
      sb.AppendLine("  $('#" & "MegaMenu" & Me.ID & "').dcMegaMenu({")
      sb.AppendLine("    rowItems:  '4',")
      sb.AppendLine("    speed:  'fast',")
      sb.AppendLine("    effect:  'fade'")
      sb.AppendLine("  });")
      sb.AppendLine("});")

      System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.GetType, ID & "Startup", sb.ToString, True)

    End Sub

    Private Sub RenderSubMenu(writer As System.Web.UI.HtmlTextWriter, RootNode As SiteMapNodeCollection, First As Boolean)

      If First Then
        writer.WriteBeginTag("ul")
        writer.WriteAttribute("id", "MegaMenu" & Me.ID)
        writer.WriteAttribute("class", "mega-menu")
        writer.WriteAttribute("style", "list-style:none")
        writer.Write(">")
      Else
        writer.WriteFullBeginTag("ul")
      End If

      For Each node As SiteMapNode In RootNode

        'Render Item
        writer.WriteBeginTag("li")
        If node("Icon") IsNot Nothing Then
          writer.WriteAttribute("class", "ImageIcon")
        End If
        writer.Write(">")

        writer.WriteBeginTag("a")
        If node.Url = "" Then
          writer.WriteAttribute("href", "#")
        Else
          writer.WriteAttribute("href", node.Url)
        End If
        If node("target") IsNot Nothing Then
          writer.WriteAttribute("target", node("target"))
        End If
       
        writer.Write(">")

        If node("Icon") IsNot Nothing Then
          writer.WriteBeginTag("img")
          writer.WriteAttribute("src", Utils.URL_ToAbsolute(node("Icon")))
          writer.Write("/>")
        End If


        writer.Write(node.Title)
        writer.WriteEndTag("a")


        If node.ChildNodes.Count > 0 Then
          RenderSubMenu(writer, node.ChildNodes, False)
        End If

        writer.WriteEndTag("li")
        writer.WriteLine()

      Next

      writer.WriteEndTag("ul")
      writer.WriteLine()

    End Sub

  End Class

End Namespace


