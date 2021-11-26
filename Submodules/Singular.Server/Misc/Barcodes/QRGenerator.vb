Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
Imports System.Drawing
#End If

Namespace Barcodes

	Public Enum BCFormat
		Code128 = 1
		QRCode = 2
	End Enum

	<Serializable()> _
	Public Class QRGenerator
		Inherits SingularBusinessBase(Of QRGenerator)

#Region " Properties and Methods "

#Region " Properties "

		Public Shared ImageProperty As PropertyInfo(Of Byte()) = RegisterProperty(Of Byte())(Function(c) c.Image, "Image")

		<Display(Name:="Image", Description:="")>
		Public Property Image() As Byte()
			Get
				Return GetProperty(ImageProperty)
			End Get
			Set(ByVal Value As Byte())
				SetProperty(ImageProperty, Value)
			End Set
		End Property

		Public Shared STProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ST, "")
		''' <Summary>
		''' Gets and sets the ST value
		''' </Summary>
		<Display(Name:="ST", Description:=""),
		Required(ErrorMessage:="ST required")>
		Public Property ST() As String
			Get
				Return GetProperty(STProperty)
			End Get
			Set(ByVal Value As String)
				SetProperty(STProperty, Value)
			End Set
		End Property

		Public Shared BarcodeFormatProperty As PropertyInfo(Of BCFormat) = RegisterProperty(Of BCFormat)(Function(c) c.BarcodeFormat, "Barcode Format", BCFormat.Code128)
		''' <Summary>
		''' Gets and sets the ST value
		''' </Summary>
		<Display(Name:="Barcode Format", Description:=""),
		Required(ErrorMessage:="Barcode Format required")>
		Public Property BarcodeFormat() As BCFormat
			Get
				Return GetProperty(BarcodeFormatProperty)
			End Get
			Set(ByVal Value As BCFormat)
				SetProperty(BarcodeFormatProperty, Value)
			End Set
		End Property



#If SILVERLIGHT Then
#Else
		Public Property BitmapImage As System.Drawing.Image
#End If

#End Region

#Region " Methods "

		Protected Overrides Function GetIdValue() As Object

			Return 1

		End Function

		Public Overrides Function ToString() As String

			Return ""

		End Function

#End Region

#End Region

#Region " Validation Rules "

		Protected Overrides Sub AddBusinessRules()

			MyBase.AddBusinessRules()

		End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

		Public Shared Function NewQRGenerator() As QRGenerator

			Return DataPortal.CreateChild(Of QRGenerator)()

		End Function

		Public Sub New()

			MarkAsChild()

		End Sub

#End Region

		<Serializable()>
		Public Class Criteria
			Inherits CriteriaBase(Of Criteria)

			Public Shared ImageProperty As PropertyInfo(Of Byte()) = RegisterProperty(Of Byte())(Function(c) c.Image, "Image")
			''' <Summary>
			''' Gets and sets the ImageList value
			''' </Summary>
			Public Property Image() As Byte()
				Get
					Return ReadProperty(ImageProperty)
				End Get
				Set(ByVal Value As Byte())
					LoadProperty(ImageProperty, Value)
				End Set
			End Property

			Public Shared STProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ST, "")
			''' <Summary>
			''' Gets and sets the STList value
			''' </Summary>
			Public Property ST() As String
				Get
					Return ReadProperty(STProperty)
				End Get
				Set(ByVal Value As String)
					LoadProperty(STProperty, Value)
				End Set
			End Property

			Public Shared BarcodeFormatProperty As PropertyInfo(Of BCFormat) = RegisterProperty(Of BCFormat)(Function(c) c.BarcodeFormat, "")
			''' <Summary>
			''' Gets and sets the STList value
			''' </Summary>
			Public Property BarcodeFormat() As BCFormat
				Get
					Return ReadProperty(BarcodeFormatProperty)
				End Get
				Set(ByVal Value As BCFormat)
					LoadProperty(BarcodeFormatProperty, Value)
				End Set
			End Property

			Public Sub New()

			End Sub

		End Class

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Sub BeginGenerateBarcode(CallBack As EventHandler(Of DataPortalResult(Of QRGenerator)))

      Dim dp As New DataPortal(Of QRGenerator)
      AddHandler dp.FetchCompleted, CallBack

      dp.BeginFetch(New Criteria() With {.Image = Me.Image, .ST = Me.ST, .BarcodeFormat = Me.BarcodeFormat})

    End Sub

#End Region

#Region " .NET Data Access "

#Else

		Public Sub Fetch()

			Me.GenerateBarcode(Me.Image, Me.ST, Me.BarcodeFormat)

		End Sub

		Friend Sub DeleteSelf()


		End Sub

		Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)


		End Sub

		Protected Sub DataPortal_Fetch(ByVal criteria As Criteria)

			Me.GenerateBarcode(criteria.Image, criteria.ST, criteria.BarcodeFormat)

		End Sub

		Private Sub GenerateBarcode(Image As Byte(), ST As String, BarcodeFormat As BCFormat)

			Dim writer As ZXing.BarcodeWriter = New ZXing.BarcodeWriter

			Select Case BarcodeFormat
				Case BCFormat.Code128
					writer.Format = ZXing.BarcodeFormat.CODE_128
					writer.Options.Width = 350
				Case BCFormat.QRCode
					writer.Format = ZXing.BarcodeFormat.QR_CODE
					writer.Options.Width = 100
					writer.Options.Height = 100
			End Select


			Dim i = writer.Write(ST)

			'i.Save("C:\Temp\Test.png", System.Drawing.Imaging.ImageFormat.Png)

			Dim imgStream As New System.IO.MemoryStream()
			i.Save(imgStream, System.Drawing.Imaging.ImageFormat.Png)
			Dim byteArray As Byte() = imgStream.ToArray()
			imgStream.Close()
			imgStream.Dispose()
			Me.Image = byteArray

		End Sub

		Public Function GenerateBarcode() As Byte()

			Dim writer As ZXing.BarcodeWriter = New ZXing.BarcodeWriter

			Select Case BarcodeFormat
				Case BCFormat.Code128
					writer.Format = ZXing.BarcodeFormat.CODE_128
					writer.Options.Width = 350
				Case BCFormat.QRCode
					writer.Format = ZXing.BarcodeFormat.QR_CODE
					writer.Options.Width = 100
					writer.Options.Height = 100
			End Select


			Dim i = writer.Write(ST)

			'i.Save("C:\Temp\Test.png", System.Drawing.Imaging.ImageFormat.Png)

			Dim imgStream As New System.IO.MemoryStream()
			i.Save(imgStream, System.Drawing.Imaging.ImageFormat.Png)
			Dim byteArray As Byte() = imgStream.ToArray()
			imgStream.Close()
			imgStream.Dispose()
			Return byteArray

		End Function

#End If

#End Region

#End Region

	End Class


End Namespace