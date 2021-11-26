Namespace Controls

  Public Class HtmlTextWriter
    Inherits System.Web.UI.HtmlTextWriter

    Public Sub New()
      MyBase.New(New IO.StringWriter)
    End Sub

    Public Sub New(Writer As System.Web.UI.HtmlTextWriter)
      MyBase.New(Writer.InnerWriter)
    End Sub

    ''' <summary>
    ''' Writes >
    ''' </summary>
    Public Sub WriteCloseTag(AddLevelAfter As Boolean)
      InnerWriter.Write(TagRightChar)
      If AddLevelAfter Then
        AddLevel()
      End If
    End Sub

    ''' <summary>
    ''' Writes />
    ''' </summary>
    Public Sub WriteFullCloseTag()
      InnerWriter.Write(SelfClosingTagEnd)
    End Sub

    Public Overloads Sub WriteFullBeginTag(tagName As String, AddLevelAfter As Boolean)
      MyBase.WriteFullBeginTag(tagName)
      If AddLevelAfter Then
        AddLevel()
      End If
    End Sub

    Public Overrides Sub WriteEndTag(tagName As String)
      MyBase.WriteEndTag(tagName)
      WriteLine()
    End Sub

    Public Overloads Sub WriteEndTag(tagName As String, RemoveLevelBefore As Boolean)
      If RemoveLevelBefore Then
        Indent -= 1
      End If
      MyBase.WriteEndTag(tagName)
      WriteLine()
    End Sub

    Public Sub AddLevel()
      Indent += 1
      WriteLine()
    End Sub

    Public Sub RemoveLevel()
      Indent -= 1
      WriteLine()
    End Sub

    Public Overrides Sub WriteLine()
      MyBase.WriteLine()
    End Sub

    Public Overrides Function ToString() As String
      Return InnerWriter.ToString
    End Function

  End Class

End Namespace