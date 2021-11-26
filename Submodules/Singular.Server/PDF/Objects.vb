Imports iTextSharp

Namespace Pdf

	Partial Public Class PdfDocument

		Public Class Header

			Public Property Title As Text = Nothing

			Public Property SubTitle As Text = Nothing

			Public Property Image As Image = Nothing

			Public Property LineBelow As Boolean = False

			Public Sub New()

			End Sub

			Public Sub New(Title As String)
				Me.Title = New Text(Title)
			End Sub

			Public Sub New(Title As Text)
				Me.Title = Title
			End Sub

			Public Sub New(Title As String, SubTitle As String)
				Me.Title = New Text(Title)
				Me.SubTitle = New Text(SubTitle)
			End Sub

			Public Sub New(Title As Text, SubTitle As Text)
				Me.Title = Title
				Me.SubTitle = SubTitle
			End Sub

			Public Sub New(Image As Image)
				Me.Image = Image
			End Sub

			Public Sub New(Image As Byte())
				Me.Image = New Image(Image)
			End Sub

			Public Sub New(Title As Text, SubTitle As Text, Image As Image)
				Me.Title = Title
				Me.SubTitle = SubTitle
				Me.Image = Image
			End Sub

		End Class

		Public Class Footer

			Public Property LeftText As Text = Nothing

			Public Property LeftImage As Image = Nothing

			Public Property RightText As Text = Nothing

			Public Property RightImage As Image = Nothing

			Public Property ShowPageNumber As Boolean = False

			Public Property LineAbove As Boolean = False

			Public Sub New()

			End Sub

			Public Sub New(LeftText As Text)
				Me.LeftText = LeftText
			End Sub

			Public Sub New(LeftText As String)
				Me.LeftText = New Text(LeftText)
			End Sub

			Public Sub New(LeftImage As Image)
				Me.LeftImage = LeftImage
			End Sub

			Public Sub New(LeftImage As Byte())
				Me.LeftImage = New Image(LeftImage)
			End Sub

			Public Sub New(LeftText As Text, RightText As Text)
				Me.LeftText = LeftText
				Me.RightText = RightText
			End Sub

			Public Sub New(LeftText As Text, RightImage As Image)
				Me.LeftText = LeftText
				Me.RightImage = RightImage
			End Sub

			Public Sub New(LeftImage As Image, RightText As Text)
				Me.LeftImage = LeftImage
				Me.RightText = RightText
			End Sub

			Public Sub New(LeftImage As Image, RightImage As Image)
				Me.LeftImage = LeftImage
				Me.RightImage = RightImage
			End Sub

		End Class

		Public Class Image

			Public Property Src As Byte()

			Public Property Width As Integer?

			Public Property Height As Integer?

			Public Property Alignment As Alignment

			Public Sub New()

			End Sub

			Public Sub New(Image As Byte())
				Using ms As New IO.MemoryStream(Image)
					Dim img As System.Drawing.Image = System.Drawing.Image.FromStream(ms)
					Me.Src = Image
					Me.Width = img.Width
					Me.Height = img.Height
				End Using
			End Sub

			Public Function GetWidth() As Integer
				If Width IsNot Nothing Then
					Return Width
				Else
					If Src IsNot Nothing Then
						Dim ret As Integer
						Using ms As New IO.MemoryStream(Src)
							Dim img As System.Drawing.Image = System.Drawing.Image.FromStream(ms)
							ret = img.Width
						End Using
						Return ret
					Else
						Return -1
					End If
				End If
			End Function

			Public Function GetHeight() As Integer
				If Height IsNot Nothing Then
					Return Height
				Else
					If Src IsNot Nothing Then
						Dim ret As Integer
						Using ms As New IO.MemoryStream(Src)
							Dim img As System.Drawing.Image = System.Drawing.Image.FromStream(ms)
							ret = img.Height
						End Using
						Return ret
					Else
						Return -1
					End If
				End If
			End Function

		End Class

		Public Class Text
			Public Property Text As String
			Public Property Font As iTextSharp.text.pdf.BaseFont = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, False)
			Public Property FontSize As Integer = 12
			Public Property Alignment As Alignment = PdfDocument.Alignment.left

			Public Sub New(Text As String)
				Me.Text = Text
			End Sub

			Public Sub New(Text As String, Font As iTextSharp.text.pdf.BaseFont)
				Me.Text = Text
				Me.Font = Font
			End Sub

			Public Sub New(Text As String, FontSize As Integer)
				Me.Text = Text
				Me.FontSize = FontSize
			End Sub

			Public Sub New(Text As String, Font As iTextSharp.text.pdf.BaseFont, FontSize As Integer)
				Me.Text = Text
				Me.Font = Font
				Me.FontSize = FontSize
			End Sub

			Public Sub New(Text As String, Alignment As Alignment)
				Me.Text = Text
				Me.Alignment = Alignment
			End Sub

			Public Sub New(Text As String, Font As iTextSharp.text.pdf.BaseFont, Alignment As Alignment)
				Me.Text = Text
				Me.Font = Font
				Me.Alignment = Alignment
			End Sub

			Public Sub New(Text As String, FontSize As Integer, Alignment As Alignment)
				Me.Text = Text
				Me.FontSize = FontSize
				Me.Alignment = Alignment
			End Sub

			Public Sub New(Text As String, Font As iTextSharp.text.pdf.BaseFont, FontSize As Integer, Alignment As Alignment)
				Me.Text = Text
				Me.Font = Font
				Me.FontSize = FontSize
				Me.Alignment = Alignment
			End Sub

			Public Function GetHeight() As Single
				Dim h = Font.GetAscentPoint(Text, FontSize) - Font.GetDescentPoint(Text, FontSize)
				Return h
			End Function

			Public Function GetWidth() As Single
				Dim w = Font.GetWidthPoint(Text, FontSize)
				Return w
			End Function

		End Class

		Public Enum Alignment
			left
			right
			center
		End Enum

	End Class

End Namespace

