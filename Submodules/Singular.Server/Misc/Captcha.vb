Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Text

<Serializable()>
Public Class Captcha

  Private mText As String = ""
  Private mSecret As String
  Private mImage As Byte()

  Public ReadOnly Property Secret As String
    Get
      Return mSecret
    End Get
  End Property

  Public ReadOnly Property Image As Byte()
    Get
      Return mImage
    End Get
  End Property

  Public Shared Property CaptchaExpiryPeriod As New TimeSpan(0, 30, 0)

  <Singular.Web.WebCallable(LoggedInOnly:=False)>
  Public Shared Function NewCaptcha(Optional CaptchaLength As Integer = 6, Optional Width As Integer = 250, Optional Height As Integer = 40) As Captcha
    Dim c As New Captcha
    c.mText = GenerateRandomString(CaptchaLength)
    'Generate the image of the random text.
    Using bmp = GenerateImage(c.mText, Width, Height, "Arial")
      Using ms As New IO.MemoryStream
        bmp.Save(ms, ImageFormat.Jpeg)
        c.mImage = ms.ToArray
      End Using
    End Using

    c.mSecret = Singular.Encryption.GetEncryptedToken(Now.Add(CaptchaExpiryPeriod), c.mText)

    Return c
  End Function

  Public Function IsTextCorrect(UserEnteredText As String) As Boolean
    Return UserEnteredText.ToLower = mText.ToLower
  End Function

  Public Shared Function IsTextCorrect(Obj As ICaptcha) As Boolean
    Return IsTextCorrect(Obj.CaptchaText, Obj.CaptchaSecret)
  End Function

  Public Shared Function IsTextCorrect(CaptchaText As String, CaptchaSecret As String, Optional ByRef HasExpired As Boolean = False) As Boolean
    Try
      If CaptchaSecret = "" Then
        Return False
      Else
        Dim PlainText As String = ""
        If Singular.Encryption.TryDecryptToken(Now, CaptchaSecret, PlainText) Then
          HasExpired = False
          Return PlainText.ToLower = CaptchaText.ToLower
        Else
          HasExpired = True
          Return False
        End If
      End If
    Catch ex As Exception
      Return False
    End Try
  End Function

  Private Shared Function GenerateImage(text As String, width As Integer, height As Integer, fontFamily As String) As Bitmap
    Dim random As New Random(DateTime.Now.Millisecond)

    ' Create a new 32-bit bitmap image.
    Dim bitmap As New Bitmap(width, height, PixelFormat.Format32bppArgb)

    ' Create a graphics object for drawing.
    Dim g As Graphics = Graphics.FromImage(bitmap)
    g.SmoothingMode = SmoothingMode.AntiAlias
    Dim rect As New Rectangle(0, 0, width, height)

    'Make a random colour.
    Dim ColBase As Integer = 120
    Dim ColRandom As Integer = 70
    Dim col As Color = Color.FromArgb(random.Next(ColRandom) + ColBase, random.Next(ColRandom) + ColBase, random.Next(ColRandom) + ColBase)

    ' Fill in the background.
    Dim hatchBrush As New HatchBrush(HatchStyle.Wave, col, Color.FromArgb(random.Next(10) + 245, random.Next(10) + 245, random.Next(10) + 245))
    g.FillRectangle(hatchBrush, rect)

    ' Set up the text font.
    Dim size As SizeF
    Dim fontSize As Single = rect.Height + 1
    Dim font As Font
    Dim format As New StringFormat()
    format.Alignment = StringAlignment.Center
    format.LineAlignment = StringAlignment.Center

    ' Adjust the font size until the text fits within the image.
    Do
      fontSize -= 1
      font = New Font(fontFamily, fontSize, FontStyle.Bold)
      size = g.MeasureString(text, font, New SizeF(width, height), format)
    Loop While size.Width > rect.Width

    ' Create a path using the text and warp it randomly.
    For i As Integer = 0 To text.Length - 1
      Dim path As New GraphicsPath()
      path.AddString(text(i), font.FontFamily, CInt(font.Style), CSng(font.Size * Math.Min(1.1, random.NextDouble + 0.6)),
                     New Rectangle(i * (width / text.Length), 0, width / text.Length, height), format)
      Dim v As Single = 6.0F
      Dim v2 As Single = text.Length * 1.7
      Dim points As PointF() = {New PointF(random.[Next](rect.Width) / v2, random.[Next](rect.Height) / v),
                                New PointF(rect.Width - random.[Next](rect.Width) / v2, random.[Next](rect.Height) / v),
                                New PointF(random.[Next](rect.Width) / v2, rect.Height - random.[Next](rect.Height) / v),
                                New PointF(rect.Width - random.[Next](rect.Width) / v2, rect.Height - random.[Next](rect.Height) / v)}
      Dim matrix As New Matrix()
      matrix.Translate(0.0F, 0.0F)
      path.Warp(points, rect, matrix, WarpMode.Perspective, 0.0F)

      ' Draw the text.
      hatchBrush = New HatchBrush(HatchStyle.DashedUpwardDiagonal, Color.Black, Color.FromArgb(random.Next(180), random.Next(180), random.Next(180)))
      g.FillPath(hatchBrush, path)
    Next

    ' Add some random noise.
    Dim m As Integer = Math.Max(rect.Width, rect.Height)
    For i As Integer = 0 To CInt(Math.Truncate(rect.Width * rect.Height / 30.0F)) - 1
      Dim x As Integer = random.[Next](rect.Width)
      Dim y As Integer = random.[Next](rect.Height)
      Dim w As Integer = random.[Next](m \ 50)
      Dim h As Integer = random.[Next](m \ 50)
      If i Mod 10 = 0 Then
        hatchBrush = New HatchBrush(HatchStyle.DashedUpwardDiagonal, Color.Red, Color.FromArgb(random.Next(240), random.Next(240), random.Next(240)))
      End If

      g.FillEllipse(hatchBrush, x, y, w, h)
    Next

    ' Clean up.
    font.Dispose()
    hatchBrush.Dispose()
    g.Dispose()

    Return bitmap
  End Function

  Private Shared Function GenerateRandomString(size As Integer) As String
    Dim builder As New StringBuilder
    Dim random As New Random(DateTime.Now.Millisecond - 8)
    Dim ch As Char
    For i As Integer = 0 To size - 1
      ch = Convert.ToChar(Convert.ToInt32(Math.Floor(25 * random.NextDouble() + 65)))
      If ch = "I" Then
        ch = "Z"
      End If
      builder.Append(ch)
    Next
    Return builder.ToString()
  End Function

End Class

Public Interface ICaptcha

  <System.ComponentModel.DataAnnotations.Display(Name:="Enter the text that you see in the image."), Singular.DataAnnotations.LocalisedDisplay("CapthaText")>
  Property CaptchaText As String

  Property CaptchaSecret As String

End Interface


