Imports System.Drawing
Imports System.Drawing.Imaging

Public Class Imaging

  Public Class Resizer

    Public Enum ImageResizeMode
      ''' <summary>
      ''' The aspect ratio of the original image is preserved.
      ''' </summary>
      KeepOriginalAspect = 1
      ''' <summary>
      ''' The new image will have the aspect ratio of the required size. The image will be centered with borders of color 'BorderColor' will be added. If the border color is transparent, the image will be converted to png.
      ''' </summary>
      AddBorders = 2
      'CenterCrop = 3
    End Enum

    Public Enum ImageFitMode
      ''' <summary>
      ''' The image will be contained within the borders of the required size area. Both of the dimensions will be less than the required size.
      ''' </summary>
      Contain = 1
      ''' <summary>
      ''' The image will fit into the borders of the required size area with no gaps. One of the dimensions may be more than the required size.
      ''' </summary>
      Cover = 2
    End Enum

    Private mRequiredSize As Size
    Private mRequiredAspect As Decimal
    Private mOutputFormat As ImageFormat
    Private Shared mEncoderJpg As ImageCodecInfo = ImageCodecInfo.GetImageEncoders.First(Function(f) f.MimeType = "image/jpeg")
    Private Shared mEncoderHQ As New EncoderParameters(1) With {.Param = {New EncoderParameter(Encoder.Quality, 90)}}

    Public Property ResizeMode As ImageResizeMode = ImageResizeMode.KeepOriginalAspect
    Public Property FitMode As ImageFitMode = ImageFitMode.Contain
    Public Property BorderColor As Color = Color.Transparent

    Public ReadOnly Property OutputFormat As ImageFormat
      Get
        Return mOutputFormat
      End Get
    End Property

    ''' <summary>
    ''' File extension to use for the resized image. Includes the "."
    ''' </summary>
    Public ReadOnly Property FileExtension As String
      Get
        Return ImageCodecInfo.GetImageEncoders().FirstOrDefault(Function(c) c.FormatID = mOutputFormat.Guid).FilenameExtension.Split({";"}, StringSplitOptions.RemoveEmptyEntries).First().Trim("*").ToLower
      End Get
    End Property

    Public Sub New(Width As Integer, Height As Integer, Optional ResizeMode As ImageResizeMode = ImageResizeMode.KeepOriginalAspect)
      SetRequiredDimensions(Width, Height)
      Me.ResizeMode = ResizeMode
    End Sub

    Public Sub SetRequiredDimensions(Width As Integer, Height As Integer)
      mRequiredSize = New Size(Width, Height)
      mRequiredAspect = Width / Height
    End Sub

    Public Function GetResizedImage(ImageBytes As Byte()) As Byte()

      Dim OldImage = Image.FromStream(New IO.MemoryStream(ImageBytes))
      Using NewImage = GetResizedImage(OldImage)

        If NewImage Is OldImage Then
          Return ImageBytes
        Else

          Using ms As New IO.MemoryStream

            If mOutputFormat Is ImageFormat.Jpeg Then
              NewImage.Save(ms, mEncoderJpg, mEncoderHQ)
            Else
              NewImage.Save(ms, mOutputFormat)
            End If

            Return ms.ToArray

          End Using

        End If

      End Using

    End Function

    Public Function GetResizedImage(Image As Bitmap) As Bitmap

      mOutputFormat = Image.RawFormat
      Dim ImageAspect As Decimal = Image.Width / Image.Height
      Dim IsBigger As Boolean = False
      Dim NewSize As Size
      Dim ContainerSize As Size
      Dim MaxHeight = Math.Min(Image.Height, mRequiredSize.Height)
      Dim MaxWidth = Math.Min(Image.Width, mRequiredSize.Width)

      If ImageAspect < mRequiredAspect Then
        'Narrower

        IsBigger = Image.Height > mRequiredSize.Height
        If FitMode = ImageFitMode.Cover AndAlso MaxHeight * ImageAspect < mRequiredSize.Width Then
          NewSize = New Size(mRequiredSize.Width, mRequiredSize.Width / ImageAspect)
        Else
          NewSize = New Size(MaxHeight * ImageAspect, MaxHeight)
        End If

        ContainerSize = New Size(MaxHeight * mRequiredAspect, MaxHeight)

      Else
        'Wider

        IsBigger = Image.Width > mRequiredSize.Width
        If FitMode = ImageFitMode.Cover AndAlso MaxWidth / ImageAspect < mRequiredSize.Height Then
          NewSize = New Size(mRequiredSize.Height * ImageAspect, mRequiredSize.Height)
        Else
          NewSize = New Size(MaxWidth, MaxWidth / ImageAspect)
        End If

        ContainerSize = New Size(MaxWidth, MaxWidth / mRequiredAspect)

      End If

      If Not IsBigger AndAlso ResizeMode = ImageResizeMode.KeepOriginalAspect Then
        'if the image is smaller than the required dimensions, and doesnt need borders, return original.
        Return Image
      End If
      If ContainerSize.Equals(Image.Size) Then
        'if the aspect ratio results in an image of the same size, return original.
        Return Image
      End If

      Dim NewImage As Bitmap
      If ResizeMode = ImageResizeMode.KeepOriginalAspect Then
        'Create a new image with the smaller size, .NET will resize the image. Keep the original image format.
        NewImage = New Bitmap(Image, NewSize)

      Else
        'Create a new image with the smaller size with the background color equal to the border color. Draw the new image in the center.

        NewImage = New Bitmap(ContainerSize.Width, ContainerSize.Height)
        Using gr = Graphics.FromImage(NewImage)
          gr.Clear(BorderColor)
          gr.DrawImage(Image,
                       New Rectangle((ContainerSize.Width - NewSize.Width) / 2, (ContainerSize.Height - NewSize.Height) / 2, NewSize.Width, NewSize.Height),
                       New Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel)
        End Using
        mOutputFormat = Image.RawFormat
        If BorderColor.A < Byte.MaxValue Then
          'if the border color has transparency, it has to be saved as png.
          mOutputFormat = ImageFormat.Png
        End If
      End If

      Return NewImage

    End Function

  End Class

End Class
