Imports iTextSharp.tool.xml.html
Imports iTextSharp.tool.xml.css
Imports iTextSharp.tool.xml
Imports iTextSharp.tool.xml.pipeline.html
Imports iTextSharp.tool.xml.pipeline.end
Imports iTextSharp
Imports System.IO

Namespace Pdf

	Public Class PdfDocument
		Implements IDisposable

		Private Property mDocument As iTextSharp.text.Document

		Private Property MS As System.IO.MemoryStream

		Private Property Writer As iTextSharp.text.pdf.PdfWriter

		Private Property tagProcessors As DefaultTagProcessorFactory

		Private Property HPC As HtmlPipelineContext

		Private Property mHeader As Header

		Private Property mFooter As Footer

		Public ReadOnly Property Document As iTextSharp.text.Document
			Get
				Return mDocument
			End Get
		End Property

		Public ReadOnly Property HtmlPipelineContext As HtmlPipelineContext
			Get
				Return HPC
			End Get
		End Property

		Public ReadOnly Property PdfWriter As iTextSharp.text.pdf.PdfWriter
			Get
				Return Writer
			End Get
		End Property

		Public ReadOnly Property DocumentHeader As Header
			Get
				Return mHeader
			End Get
		End Property

		Public ReadOnly Property DocumentFooter As Footer
			Get
				Return mFooter
			End Get
		End Property

		Public Sub New()
			MS = New System.IO.MemoryStream()
			mDocument = New iTextSharp.text.Document(iTextSharp.text.PageSize.A4)
			Writer = iTextSharp.text.pdf.PdfWriter.GetInstance(Document, MS)


			tagProcessors = CType(Tags.GetHtmlTagProcessorFactory(), DefaultTagProcessorFactory)
			tagProcessors.RemoveProcessor(iTextSharp.tool.xml.html.HTML.Tag.IMG)
			tagProcessors.AddProcessor(iTextSharp.tool.xml.html.HTML.Tag.IMG, New CustomProcessors.CustomImageTagProcessor())

			tagProcessors.RemoveProcessor(iTextSharp.tool.xml.html.HTML.Tag.TABLE)
			tagProcessors.AddProcessor(iTextSharp.tool.xml.html.HTML.Tag.TABLE, New CustomProcessors.CustomTableTagProcessor())

			HPC = New HtmlPipelineContext(New CssAppliersImpl(New XMLWorkerFontProvider()))
			HPC.SetAcceptUnknown(True).AutoBookmark(True).SetTagFactory(tagProcessors)

			Writer.PageEvent = New CustomProcessors.PdfDocumentHeaderFooter()
			CType(Writer.PageEvent, CustomProcessors.PdfDocumentHeaderFooter).PdfDocument = Me


		End Sub

		Public Sub OpenDocument()
			SetDocumentMargins()
			Document.Open()
		End Sub

		Public Function CloseAndGetBytes() As Byte()
			SetDocumentMargins()
			Document.Close()
			Return MS.ToArray()
		End Function

		Public Sub NewPage()
			Document.NewPage()
		End Sub

		Public Sub AddHTML(Html As String, Optional CorrectCSSStyles As Boolean = True)

			Dim seg = New PdfDocumentSegment(Me, Html)

			If CorrectCSSStyles Then
				seg.CorrectCSSStyles()
			End If

			seg.WriteToDocument()

		End Sub

		Public Function AddSegment(Html As String) As PdfDocumentSegment
			Return New PdfDocumentSegment(Me, Html)
		End Function

		Private Sub SetDocumentMargins()

			Dim TopMargin As Single = Document.TopMargin
			Dim RightMargin As Single = Document.RightMargin
			Dim BottomMargin As Single = Document.BottomMargin
			Dim LeftMargin As Single = Document.LeftMargin

			If DocumentHeader IsNot Nothing Then
				Dim LeftHeight As Integer = 0
				Dim CenterHeight As Integer = 0
				Dim RightHeight As Integer = 0
				'Check Image
				If DocumentHeader.Image IsNot Nothing Then
					Select Case DocumentHeader.Image.Alignment
						Case Alignment.left
							LeftHeight += DocumentHeader.Image.GetHeight
						Case Alignment.center
							CenterHeight += DocumentHeader.Image.GetHeight
						Case Alignment.right
							RightHeight += DocumentHeader.Image.GetHeight
					End Select
				End If
				'Check Title
				If DocumentHeader.Title IsNot Nothing Then
					Select Case DocumentHeader.Title.Alignment
						Case Alignment.left
							LeftHeight += DocumentHeader.Title.GetHeight
						Case Alignment.center
							CenterHeight += DocumentHeader.Title.GetHeight
						Case Alignment.right
							RightHeight += DocumentHeader.Title.GetHeight
					End Select
				End If
				'Check SubTitle
				If DocumentHeader.SubTitle IsNot Nothing Then
					Select Case DocumentHeader.SubTitle.Alignment
						Case Alignment.left
							LeftHeight += DocumentHeader.SubTitle.GetHeight
						Case Alignment.center
							CenterHeight += DocumentHeader.SubTitle.GetHeight
						Case Alignment.right
							RightHeight += DocumentHeader.SubTitle.GetHeight
					End Select
				End If

				TopMargin += Math.Max(LeftHeight, Math.Max(CenterHeight, RightHeight))
			End If

			If DocumentFooter IsNot Nothing Then
				Dim LeftHeight As Integer = 0
				Dim RightHeight As Integer = 0
				'Check left Text and Image
				If DocumentFooter.LeftText IsNot Nothing Then
					LeftHeight += DocumentFooter.LeftText.GetHeight()
				End If
				If DocumentFooter.LeftImage IsNot Nothing Then
					LeftHeight += DocumentFooter.LeftImage.GetHeight()
				End If
				'Check Right Text and Image
				If DocumentFooter.RightText IsNot Nothing Then
					RightHeight += DocumentFooter.RightText.GetHeight()
				End If
				If DocumentFooter.RightImage IsNot Nothing Then
					RightHeight += DocumentFooter.RightImage.GetHeight()
				End If

				BottomMargin += Math.Max(LeftHeight, RightHeight)

			End If

			Document.SetMargins(LeftMargin, RightMargin, TopMargin, BottomMargin)

		End Sub

#Region " Header Setup "

		Public Sub SetupHeader(Title As String)
			mHeader = New Header(Title)
		End Sub

		Public Sub SetupHeader(Title As Text)
			mHeader = New Header(Title)
		End Sub

		Public Sub SetupHeader(Image As Image)
			mHeader = New Header(Image)
		End Sub

		Public Sub SetupHeader(Title As String, LineBelow As Boolean)
			mHeader = New Header(Title)
			mHeader.LineBelow = LineBelow
		End Sub

		Public Sub SetupHeader(Image As Image, LineBelow As Boolean)
			mHeader = New Header(Image)
			mHeader.LineBelow = LineBelow
		End Sub

		Public Sub SetupHeader(Title As String, TitleAlignment As Alignment)
			mHeader = New Header(New Text(Title) With {.Alignment = TitleAlignment})
		End Sub

		Public Sub SetupHeader(Title As String, SubTitle As String)
			mHeader = New Header(Title, SubTitle)
		End Sub

		Public Sub SetupHeader(Title As String, SubTitle As String, Image As Image)
			mHeader = New Header(New Text(Title), New Text(SubTitle), Image)
		End Sub

		Public Sub SetupHeader(Title As String, TitleAlignment As Alignment, LineBelow As Boolean)
			mHeader = New Header(New Text(Title) With {.Alignment = TitleAlignment})
			mHeader.LineBelow = LineBelow
		End Sub

		Public Sub SetupHeader(Title As String, SubTitle As String, LineBelow As Boolean)
			mHeader = New Header(Title, SubTitle)
			mHeader.LineBelow = LineBelow
		End Sub

		Public Sub SetupHeader(Title As String, SubTitle As String, TitleAlignment As Alignment, SubTitleAlignment As Alignment)
			mHeader = New Header(New Text(Title) With {.Alignment = TitleAlignment}, New Text(SubTitle) With {.Alignment = SubTitleAlignment})
		End Sub

		Public Sub SetupHeader(Title As String, SubTitle As String, Image As Image, LineBelow As Boolean)
			mHeader = New Header(New Text(Title), New Text(SubTitle), Image)
			mHeader.LineBelow = LineBelow
		End Sub

		Public Sub SetupHeader(Title As String, SubTitle As String, TitleAlignment As Alignment, SubTitleAlignment As Alignment, LineBelow As Boolean)
			mHeader = New Header(New Text(Title) With {.Alignment = TitleAlignment}, New Text(SubTitle) With {.Alignment = SubTitleAlignment})
			mHeader.LineBelow = LineBelow
		End Sub

		Public Sub SetupHeader(Title As String, SubTitle As String, Image As Image, TitleAlignment As Alignment, SubTitleAlignment As Alignment)
			mHeader = New Header(New Text(Title) With {.Alignment = TitleAlignment}, New Text(SubTitle) With {.Alignment = SubTitleAlignment}, Image)
		End Sub

		Public Sub SetupHeader(Title As String, SubTitle As String, Image As Image, TitleAlignment As Alignment, SubTitleAlignment As Alignment, LineBelow As Boolean)
			mHeader = New Header(New Text(Title) With {.Alignment = TitleAlignment}, New Text(SubTitle) With {.Alignment = SubTitleAlignment}, Image)
			mHeader.LineBelow = LineBelow
		End Sub

		Public Sub AddHeaderTitle(Title As Text)
			If mHeader Is Nothing Then
				SetupHeader(Title)
			Else
				mHeader.Title = Title
			End If
		End Sub

		Public Sub AddHeaderSubTitle(SubTitle As Text)
			If mHeader Is Nothing Then
				mHeader = New Header()
			End If
			mHeader.SubTitle = SubTitle
		End Sub

		Public Sub AddHeaderImage(Image As Image)
			If mHeader Is Nothing Then
				mHeader = New Header(Image)
			Else
				mHeader.Image = Image
			End If
		End Sub

#End Region

#Region " Footer Setup "

		Public Sub SetupFooter(LeftText As String, ShowPageNo As Boolean)
			mFooter = New Footer(LeftText)
			mFooter.ShowPageNumber = ShowPageNo
		End Sub

		Public Sub SetupFooter(LeftImage As Image, ShowPageNo As Boolean)
			mFooter = New Footer(LeftImage)
			mFooter.ShowPageNumber = ShowPageNo
		End Sub

		Public Sub SetupFooter(LeftText As String, RightText As String, ShowPageNo As Boolean)
			mFooter = New Footer(New Text(LeftText), New Text(RightText))
			mFooter.ShowPageNumber = ShowPageNo
		End Sub

		Public Sub SetupFooter(LeftText As String, RightImage As Image, ShowPageNo As Boolean)
			mFooter = New Footer(New Text(LeftText), RightImage)
			mFooter.ShowPageNumber = ShowPageNo
		End Sub

		Public Sub AddFooterLeft(LeftText As Text)
			If mFooter Is Nothing Then
				mFooter = New Footer(LeftText)
			Else
				mFooter.LeftText = LeftText
			End If
		End Sub

		Public Sub AddFooterLeft(LeftImage As Image)
			If mFooter Is Nothing Then
				mFooter = New Footer(LeftImage)
			Else
				mFooter.LeftImage = LeftImage
			End If
		End Sub

		Public Sub AddFooterRight(RightText As Text)
			If mFooter Is Nothing Then
				mFooter = New Footer()
			End If
			mFooter.RightText = RightText
		End Sub

		Public Sub AddFooterRight(RightImage As Image)
			If mFooter Is Nothing Then
				mFooter = New Footer()
			End If
			mFooter.RightImage = RightImage
		End Sub

		Public Sub AddFooterPageNumbers(ShowPageNumbers As Boolean)
			If mFooter Is Nothing Then
				mFooter = New Footer()
			End If
			mFooter.ShowPageNumber = ShowPageNumbers
		End Sub

#End Region

#Region "IDisposable Support"

		Private disposedValue As Boolean ' To detect redundant calls

		' IDisposable
		Protected Overridable Sub Dispose(disposing As Boolean)
			If Not Me.disposedValue Then
				If disposing Then
					' TODO: dispose managed state (managed objects).
					Document.Close()
					Writer.Close()
					MS.Close()
				End If
				' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
				' TODO: set large fields to null.
				mDocument = Nothing
				Writer = Nothing
				tagProcessors = Nothing
				HPC = Nothing
			End If
			Me.disposedValue = True
		End Sub

		' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
		'Protected Overrides Sub Finalize()
		'    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
		'    Dispose(False)
		'    MyBase.Finalize()
		'End Sub

		' This code added by Visual Basic to correctly implement the disposable pattern.
		Public Sub Dispose() Implements IDisposable.Dispose
			' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub
#End Region

	End Class

	Namespace CustomProcessors

		Public Class CustomImageTagProcessor
			Inherits iTextSharp.tool.xml.html.Image

			Public Overrides Function [End](ctx As iTextSharp.tool.xml.IWorkerContext, tag As iTextSharp.tool.xml.Tag, currentContent As IList(Of iTextSharp.text.IElement)) As IList(Of iTextSharp.text.IElement)

				Dim attributes As IDictionary(Of String, String) = tag.Attributes
				Dim src As String = ""
				If Not attributes.TryGetValue(HTML.Attribute.SRC, src) Then
					Return New List(Of iTextSharp.text.IElement)(1)
				End If
				If src Is Nothing OrElse src = "" Then
					Return New List(Of iTextSharp.text.IElement)(1)
				End If

				If src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase) Then
					'data:[<MIME-type>][;charset=<encoding>[;base64],<data>
					Dim base64Data = src.Substring(src.IndexOf(",") + 1)
					Dim imageData = Convert.FromBase64String(base64Data)
					Dim image = iTextSharp.text.Image.GetInstance(imageData)

					Dim list = New List(Of iTextSharp.text.IElement)
					Dim htmlPipelineContext = GetHtmlPipelineContext(ctx)
					list.Add(GetCssAppliers().Apply(New iTextSharp.text.Chunk(CType(GetCssAppliers().Apply(image, tag, htmlPipelineContext), iTextSharp.text.Image), 0, 0, True), tag, htmlPipelineContext))
					Return list
				Else
					Return MyBase.[End](ctx, tag, currentContent)
				End If

			End Function

		End Class

		Public Class CustomTableTagProcessor
			Inherits iTextSharp.tool.xml.html.table.Table

			Public Overrides Function [End](ctx As iTextSharp.tool.xml.IWorkerContext, tag As iTextSharp.tool.xml.Tag, currentContent As IList(Of iTextSharp.text.IElement)) As IList(Of iTextSharp.text.IElement)

				Return MyBase.[End](ctx, tag, currentContent)

			End Function

		End Class

		Public Class PdfDocumentHeaderFooter
			Inherits iTextSharp.text.pdf.PdfPageEventHelper

			Private Property ItemMargin As Integer = 10

			Public Property PdfDocument As PdfDocument

			Private Property PrintDateTime As DateTime

			Private Property cb As iTextSharp.text.pdf.PdfContentByte

			Private Property PageNumberTemplate As iTextSharp.text.pdf.PdfTemplate

			Private Property bf As text.pdf.BaseFont

			'Public Sub New(PdfDocument As PdfDocument)
			'	MyBase.New()
			'	Me.PdfDocument = PdfDocument
			'	PrintDateTime = Now

			'End Sub

			Public Overrides Sub OnOpenDocument(writer As iTextSharp.text.pdf.PdfWriter, document As iTextSharp.text.Document)
				MyBase.OnOpenDocument(writer, document)

				PrintDateTime = Now

				cb = writer.DirectContent()
				PageNumberTemplate = cb.CreateTemplate(50, 50)

				bf = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, False)

			End Sub

			Public Overrides Sub OnStartPage(writer As iTextSharp.text.pdf.PdfWriter, document As iTextSharp.text.Document)
				MyBase.OnStartPage(writer, document)

				If PdfDocument.DocumentHeader IsNot Nothing Then

					Dim cbHeader = writer.DirectContent()

					Dim pageSize As text.Rectangle = document.PageSize

					Dim leftMargin As Single = ItemMargin
					Dim centerMargin As Single = ItemMargin
					Dim rightMargin As Single = ItemMargin

					'Add Image
					If PdfDocument.DocumentHeader.Image IsNot Nothing Then
						Dim imgHead As text.Image = text.Image.GetInstance(PdfDocument.DocumentHeader.Image.Src)
						imgHead.Width = PdfDocument.DocumentHeader.Image.GetWidth()
						'Cant set height, might need to fix this
						'imgHead.Height = PdfDocument.DocumentHeader.Image.GetHeight()
						Dim imgTemplate As text.pdf.PdfTemplate = cbHeader.CreateTemplate(imgHead.Width, imgHead.Height)
						imgTemplate.AddImage(imgHead)
						Select Case PdfDocument.DocumentHeader.Image.Alignment
							Case Pdf.PdfDocument.Alignment.left
								cbHeader.AddTemplate(imgTemplate, 10, leftMargin)
								leftMargin += imgTemplate.Height + ItemMargin
							Case Pdf.PdfDocument.Alignment.center
								cbHeader.AddTemplate(imgTemplate, (pageSize.Width - imgTemplate.Width) / 2, pageSize.GetTop(centerMargin))
								centerMargin += imgTemplate.Height + ItemMargin
							Case Pdf.PdfDocument.Alignment.right
								cbHeader.AddTemplate(imgTemplate, (pageSize.Width - imgTemplate.Width) - 10, pageSize.GetTop(rightMargin))
								rightMargin += imgTemplate.Height + ItemMargin
						End Select
					End If

					'Title
					If PdfDocument.DocumentHeader.Title IsNot Nothing Then
						cbHeader.BeginText()
						cbHeader.SetFontAndSize(PdfDocument.DocumentHeader.Title.Font, PdfDocument.DocumentHeader.Title.FontSize)
						Select Case PdfDocument.DocumentHeader.Title.Alignment
							Case Pdf.PdfDocument.Alignment.left
								cbHeader.SetTextMatrix(10, pageSize.GetTop(leftMargin))
								leftMargin += PdfDocument.DocumentHeader.Title.GetHeight() + ItemMargin
							Case Pdf.PdfDocument.Alignment.center
								cbHeader.SetTextMatrix((pageSize.Width - PdfDocument.DocumentHeader.Title.GetWidth()) / 2, pageSize.GetTop(centerMargin))
								centerMargin += PdfDocument.DocumentHeader.Title.GetHeight() + ItemMargin
							Case Pdf.PdfDocument.Alignment.right
								cbHeader.SetTextMatrix((pageSize.Width - PdfDocument.DocumentHeader.Title.GetWidth()) - 10, pageSize.GetTop(rightMargin))
								rightMargin += PdfDocument.DocumentHeader.Title.GetHeight() + ItemMargin
						End Select
						cbHeader.ShowText(PdfDocument.DocumentHeader.Title.Text)
						cbHeader.EndText()
					End If

					'SubTitle
					If PdfDocument.DocumentHeader.SubTitle IsNot Nothing Then
						cbHeader.BeginText()
						cbHeader.SetFontAndSize(PdfDocument.DocumentHeader.SubTitle.Font, PdfDocument.DocumentHeader.SubTitle.FontSize)
						Select Case PdfDocument.DocumentHeader.SubTitle.Alignment
							Case Pdf.PdfDocument.Alignment.left
								cbHeader.SetTextMatrix(10, pageSize.GetTop(leftMargin))
								leftMargin += PdfDocument.DocumentHeader.SubTitle.GetHeight() + ItemMargin
							Case Pdf.PdfDocument.Alignment.center
								cbHeader.SetTextMatrix((pageSize.Width - PdfDocument.DocumentHeader.SubTitle.GetWidth()) / 2, pageSize.GetTop(centerMargin))
								centerMargin += PdfDocument.DocumentHeader.SubTitle.GetHeight() + ItemMargin
							Case Pdf.PdfDocument.Alignment.right
								cbHeader.SetTextMatrix((pageSize.Width - PdfDocument.DocumentHeader.SubTitle.GetWidth()) - 10, pageSize.GetTop(rightMargin))
								rightMargin += PdfDocument.DocumentHeader.SubTitle.GetHeight() + ItemMargin
						End Select
						cbHeader.ShowText(PdfDocument.DocumentHeader.SubTitle.Text)
						cbHeader.EndText()
					End If

					If PdfDocument.DocumentHeader.LineBelow Then

						Dim margin = Math.Max(leftMargin, Math.Max(centerMargin, rightMargin))

						cbHeader.MoveTo(pageSize.GetLeft(50), pageSize.GetTop(margin))
						cb.LineTo(pageSize.GetRight(50), pageSize.GetTop(margin))
						cb.Stroke()

					End If

				End If

			End Sub

			Public Overrides Sub OnEndPage(writer As iTextSharp.text.pdf.PdfWriter, document As iTextSharp.text.Document)
				MyBase.OnEndPage(writer, document)

				If PdfDocument.DocumentFooter IsNot Nothing Then

					Dim pageSize As text.Rectangle = document.PageSize

					If PdfDocument.DocumentFooter.ShowPageNumber Then

						Dim pn As String = "Page " & writer.PageNumber & " of "

						cb.BeginText()
						cb.SetFontAndSize(bf, 8)
						cb.SetTextMatrix((pageSize.Width - bf.GetWidthPoint(pn, 8) - 50) / 2, pageSize.GetBottom(10))
						cb.ShowText(pn)
						cb.EndText()
						cb.AddTemplate(PageNumberTemplate, ((pageSize.Width - bf.GetWidthPoint(pn, 8) - 50) / 2) + bf.GetWidthPoint(pn, 8), pageSize.GetBottom(10))

					End If




				End If

			End Sub

			Public Overrides Sub OnCloseDocument(writer As text.pdf.PdfWriter, document As text.Document)
				MyBase.OnCloseDocument(writer, document)

				PageNumberTemplate.BeginText()
				PageNumberTemplate.SetFontAndSize(bf, 8)
				PageNumberTemplate.SetTextMatrix(0, 0)
				PageNumberTemplate.ShowText((writer.PageNumber - 1).ToString)
				PageNumberTemplate.EndText()

			End Sub

		End Class

	End Namespace

End Namespace
