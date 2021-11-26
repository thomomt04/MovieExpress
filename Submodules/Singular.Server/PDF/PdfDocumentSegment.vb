Imports iTextSharp.tool.xml.html
Imports iTextSharp.tool.xml.css
Imports iTextSharp.tool.xml
Imports iTextSharp.tool.xml.pipeline.html
Imports iTextSharp.tool.xml.pipeline.end
Imports System.IO

Namespace Pdf

	Public Class PdfDocumentSegment

		Private Document As PdfDocument

		Private Property cssResolver As StyleAttrCSSResolver

		Private HTML As String

		Private WrittenInd As Boolean = False

		Public Sub New(Document As PdfDocument, Html As String)
			Me.HTML = Html
			Me.Document = Document

			'CSS Engine
			Dim cssFiles = New CssFilesImpl()
			cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS())

			cssResolver = New StyleAttrCSSResolver(cssFiles)

		End Sub

		Public Sub CorrectCSSStyles()
			Dim styles As New Dictionary(Of String, String)
			'Extract styles from 'style' tags
			CorrectCSS(HTML, styles)
			'Extract in line styles
			CorrectInlineCSS(HTML, styles)
			'Add Styles from tags and in line styles to CSS Engine
			For Each item In styles
				cssResolver.AddCss("." & item.Key & " {" & item.Value & " }", True)
			Next
		End Sub

		Public Sub AddClassToTag(Tag As String, ClassName As String, Style As String)
			'Add Style to CSS Engine
			cssResolver.AddCss("." & ClassName & " " & Style, True)
			'Add CSS Class to all tag instances
			HTML = AddClass(HTML, Tag, ClassName)
		End Sub

		Public Sub WriteToDocument()
			If WrittenInd Then
				Throw New Exception("Segment Already Written to Document")
			End If

			WrittenInd = True
			Dim htmlPipeline = New HtmlPipeline(Document.HtmlPipelineContext, New PdfWriterPipeline(Document.Document, Document.PdfWriter))
			Dim pipeline = New iTextSharp.tool.xml.pipeline.css.CssResolverPipeline(cssResolver, htmlPipeline)
			Dim worker = New XMLWorker(pipeline, True)
			Dim xmlparser = New iTextSharp.tool.xml.parser.XMLParser(True, worker, System.Text.Encoding.UTF8)
			xmlparser.Parse(New StringReader(HTML))
		End Sub

		Private Sub CorrectCSS(ByRef Html As String, ByRef Styles As Dictionary(Of String, String))
			Dim temphtml = Html.Replace("""", "'")
			While Html.Replace("""", "'").IndexOf("<style") >= 0
				'Need to check if src is set
				Dim style = Html.Substring(Html.IndexOf("<style"))
				style = style.Substring(0, style.IndexOf(">"))
				'Double check type
				If style.Replace("""", "'").Contains("type='text/css'") Then
					If style.Contains("src=") Then
						'src is set, need to get styles from uri to process
						CorrectURIStyles(Html, Styles)
					Else
						'CSS nested in tag
						CorrectLocalStyles(Html, Styles)
					End If

				End If
			End While
		End Sub

		Private Sub CorrectLocalStyles(ByRef Html As String, ByRef Styles As Dictionary(Of String, String))
			Dim temphtml = Html
			'Search HTML for any classes that are applied
			While temphtml.Replace("""", "'").IndexOf("class='") > -1
				Dim s = temphtml.Replace("""", "'").IndexOf("class='")
				'Get Class Name
				Dim temp = temphtml.Substring(s + 7)
				temphtml = temphtml.Substring(s + 7)
				temphtml = temphtml.Substring(temp.Replace("""", "'").IndexOf("'"))
				Dim classname = temp.Substring(0, temp.Replace("""", "'").IndexOf("'"))
				'If Class not yet saved
				If Not Styles.ContainsKey(classname) Then
					'Find Class CSS
					Dim ss = Html.IndexOf("." & classname & " {")
					If ss >= 0 Then
						Dim temps = Html.Substring(ss + 3 + classname.Length)
						temps = temps.Substring(0, temps.IndexOf(" }"))
						'html = html.Replace("class=""" & temp & """", "style=""" & temps & """")
						Html = Html.Replace("." & temp & " {" & temps & " }", "")
						'Add to CSS Engine
						Styles.Add(classname, temps)
					End If
				End If
			End While
			'Delete Style Tag
			Dim start = Html.Replace("""", "'").IndexOf("<style")
			If start >= 0 Then
				Dim en = Html.IndexOf("style>", start)
				Dim css = Html.Substring(start, en + 6 - start)
				Html = Html.Replace(css, "")
			End If
		End Sub

		Private Sub CorrectURIStyles(ByRef Html As String, ByRef Styles As Dictionary(Of String, String))
			'Style Source is URI
			'Get Style Tag
			Dim style = Html.Substring(Html.IndexOf("<style"))
			style = style.Substring(0, style.IndexOf(">") + 1)
			'Delete Style Tag from HTML
			If style.EndsWith("/>") Then
				Html = Html.Replace(style, "")
			Else
				Dim styleStart = Html.IndexOf(style)
				Dim styleEnd = Html.IndexOf("</style", styleStart + style.Length)
				styleEnd = Html.IndexOf(">", styleEnd)
				Html = Html.Replace(Html.Substring(styleStart, styleEnd - styleStart), "")
			End If
			style = style.Replace("""", "'")
			'Now have style tag
			Dim src = style.Substring(style.IndexOf("src='") + 5)
			src = src.Substring(0, src.IndexOf("'"))

			cssResolver.AddCssFile(src, True)

			'Dont need the below code because I didnt notice the above method

			''src is the url of the style sheet
			'Dim wr As System.Net.WebRequest = System.Net.WebRequest.Create(src)
			'Dim response = wr.GetResponse()
			'Dim responseStream As New IO.MemoryStream
			'response.GetResponseStream().CopyTo(responseStream)
			''Get CSS Text
			'Dim css = Text.Encoding.UTF8.GetString(responseStream.ToArray())

			''Loop through all styles
			'While css.IndexOf("{") >= 0
			'  'Get Class Name
			'  Dim styleStart = css.IndexOf("{")
			'  Dim className = css.Substring(0, styleStart).Trim()
			'  css = css.Substring(styleStart + 1)
			'  'Get CSS Style
			'  Dim styleEnd = css.IndexOf("}")
			'  style = css.Substring(0, styleEnd)
			'  css = css.Substring(styleEnd + 1)
			'  style = style.Replace("{", "").Replace("}", "").Trim()
			'  If className.StartsWith(".") Then
			'    'Custom Class Name, just add to Styles
			'    If Not Styles.ContainsKey(className.Substring(1)) Then
			'      Styles.Add(className.Substring(1), style)
			'    End If
			'  Else
			'    'Need to add to relevant Tags
			'    AddClassToTag(className, "SSCustomCSS" & className, style)
			'  End If

			'End While

		End Sub

		Private Sub CorrectInlineCSS(ByRef Html As String, ByRef Styles As Dictionary(Of String, String))
			Dim temphtml = Html
			'Loop through html to find any inline styles
			While temphtml.Replace("""", "'").IndexOf("style='") > -1
				Dim s = temphtml.Replace("""", "'").IndexOf("style='")
				Dim temp = temphtml.Substring(s + 7)
				temphtml = temphtml.Substring(s + 7)
				temphtml = temphtml.Substring(temp.Replace("""", "'").IndexOf("'"))

				Dim style As String = temp.Substring(0, temp.Replace("""", "'").IndexOf("'"))
				Dim classname As String

				Dim Item = Styles.FirstOrDefault(Function(c) c.Value = style)
				If Item.Key Is Nothing And Item.Value Is Nothing Then
					classname = "SSCustomClass" & StyleClassCount
					While Styles.ContainsKey(classname)
						StyleClassCount += 1
						classname = "SSCustomClass" & StyleClassCount
					End While
					Styles.Add(classname, style)
				Else
					classname = Item.Key
				End If

				'Need to fix this for if the tag already has a class name
				Html = Html.Replace("style=""" & style & """", "class=""" & classname & """")

			End While

		End Sub

		Private Property StyleClassCount As Integer = 0

		Private Function AddClass(html As String, Tag As String, ClassName As String) As String
			Dim ret As String = ""
			Dim temp = html
			While temp.IndexOf("<" & Tag) >= 0
				'Add Everything before to return string
				ret &= temp.Substring(0, temp.IndexOf("<" & Tag))
				'Cut out start
				temp = temp.Substring(temp.IndexOf("<" & Tag))
				'Find where tag ends
				Dim tagend = temp.IndexOf(">")
				'Seperate tag
				Dim currentTag = temp.Substring(0, tagend + 1)
				'Remove from html
				temp = temp.Substring(tagend + 1)

				If currentTag.IndexOf("class") > 0 Then
					'Already has class, add
					'Need to add to current class attribute
					Dim cls = currentTag.Substring(currentTag.IndexOf("class=""") + 7)
					ret &= currentTag.Substring(0, currentTag.IndexOf("class=""") + 7)
					ret &= cls.Substring(0, cls.IndexOf("""")) + " " + ClassName + cls.Substring(cls.IndexOf(""""))
				Else
					'Just add class attribute
					currentTag = currentTag.Replace("<" & Tag, "<" & Tag & " class=""" & ClassName & """")
					ret &= currentTag
				End If

			End While

			Return ret & temp

		End Function

	End Class

End Namespace
