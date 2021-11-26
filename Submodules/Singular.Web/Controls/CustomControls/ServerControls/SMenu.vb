Namespace CustomControls

  Public Class SMenu
    Inherits System.Web.UI.WebControls.WebControl

    Public Property SiteMapDatasourceID As String
    Public Property TopLevelClass As String = "s-menu"

    Protected Overrides Sub Render(writer As UI.HtmlTextWriter)

      writer.WriteBeginTag("div")
      writer.WriteAttribute("class", TopLevelClass)
      writer.Write(">")

      Dim smds As SiteMapDataSource = FindControl(SiteMapDatasourceID)
      RenderSubMenu(writer, CType(smds, Singular.Web.CustomControls.SiteMapDataSource).GetHierarchicalView.Select, Nothing, 1)

      writer.WriteEndTag("div")

      writer.WriteBeginTag("div")
      writer.WriteAttribute("style", "clear:both")
      writer.Write(">")
      writer.WriteEndTag("div")

    End Sub

    Private Sub RenderSubMenu(writer As System.Web.UI.HtmlTextWriter, RootNode As SiteMapNodeCollection, DataBinding As String, Level As Integer)

      'if this is level 2, and there is no level 3, then add a fake level
      Dim AddedFake As Boolean = False
      Dim HasChildren As Boolean = False
      If Level = 2 Then
        If RootNode IsNot Nothing Then
          For Each node As SiteMapNode In RootNode
            If node.ChildNodes.Count > 0 Then
              HasChildren = True
              Exit For
            End If
          Next
        End If
      
        If Not HasChildren Then
          AddedFake = True
          writer.Write("<ul class=""level2 fake""><li class=""has-child"">")
          Level += 1
        End If
      End If


      writer.WriteBeginTag("ul")
      writer.WriteAttribute("class", "level" & Level)
      If RootNode Is Nothing Then
        writer.WriteAttribute("data-bind", "foreach: " & DataBinding)
      End If
      writer.Write(">")

      If RootNode IsNot Nothing Then

        For Each node As SiteMapNode In RootNode
          WriteNode(writer, node, Nothing, Level)
        Next

      Else
        'Data-bound
        WriteNode(writer, Nothing, DataBinding, Level)

      End If

      writer.WriteEndTag("ul")
      writer.WriteLine()

      If AddedFake Then
        Level -= 1
        writer.Write("</li></ul>")
      End If

    End Sub

    Private Sub WriteNode(writer As System.Web.UI.HtmlTextWriter, Node As SiteMapNode, DataBinding As String, Level As Integer)

      Dim css As String = ""

      'Render Item
      writer.WriteBeginTag("li")
      If Node IsNot Nothing AndAlso Node.ChildNodes.Count > 0 Then
        css &= "has-child "
      Else
        css &= "no-child "
      End If

      If Node IsNot Nothing AndAlso Node("Class") IsNot Nothing Then
        css &= Node("Class") & " "
      End If

      writer.WriteAttribute("class", css)
      writer.Write(">")

      PreLink(writer, Node, Level)

      writer.WriteBeginTag("a")
      If Node IsNot Nothing Then
        If Node.Url = "" Then
          'writer.WriteAttribute("href", "#")
        Else
          writer.WriteAttribute("href", Node.Url)
        End If
        If Node("target") IsNot Nothing Then
          writer.WriteAttribute("target", Node("target"))
        End If
      Else

        writer.WriteAttribute("data-bind", "attr: { 'href': url }, text: title")

      End If


      writer.Write(">")
      If Node IsNot Nothing Then writer.Write(Node.Title)
      writer.WriteEndTag("a")

      If Node IsNot Nothing Then
        If Node.ChildNodes.Count > 0 Then
          RenderSubMenu(writer, Node.ChildNodes, Nothing, Level + 1)
        End If
        'Data bound children
        If Not String.IsNullOrEmpty(Node("data-bind")) Then
          RenderSubMenu(writer, Nothing, Node("data-bind"), Level + 1)
        End If
      End If

      writer.WriteEndTag("li")
      writer.WriteLine()

    End Sub


    Protected Overridable Sub PreLink(writer As System.Web.UI.HtmlTextWriter, Node As SiteMapNode, Level As Integer)
      If Level = 3 Then
        writer.WriteBeginTag("i")
        writer.WriteAttribute("class", "fa fa-arrow-right")
        writer.Write(">")
        writer.WriteEndTag("i")
      End If

    End Sub

  End Class

End Namespace


