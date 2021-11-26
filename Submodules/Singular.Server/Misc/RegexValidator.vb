Namespace RegexValidation
	Public Class RegexValidator

		Public Enum ValidationTypes
			InternationalMobile
			Email
			IPAddress
			Trust
			CompanyRegistation
			AlphaChars
			Numeric
			BitCoinAddress
			EthereumAddress
		End Enum

		Private ValidInternationalMobileRegString As String = "(^\+[1-9]{1}[0-9]{3,14}$)*[0-9{17}]"
		Private ValidEmailRegexString As String = "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
		Private ValidIPAddressRegexString As String = "\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b"
		Private ValidTrustRegexString As String = "^IT[0-9]{5}[/][0-9]{4}$"
		Private ValidCompanyRegNotRegexString As String = "^IT[0-9]{5}[/][0-9]{4}$"
		Private ValidAlphaCharRegexString As String = "^[a-zA-Z]+$"
		Private ValidNumericRegexString As String = "^[0-9]*$"
		Private ValidBitcoinRegexSting As String = "^[13][a-km-zA-HJ-NP-Z1-9]{25,34}$"
		Private ValidEthereumRegexSting As String = "^0x?[0-9a-f]{40}$"

		Private _validationType As ValidationTypes
		Private isCustomRegularExpression As Boolean = False
		Private regularExpressionString As String = ""


		Public Sub New(validationType As ValidationTypes)
			_validationType = validationType
		End Sub

		Public Sub New(regularExpression As String)
			isCustomRegularExpression = True
			regularExpressionString = regularExpression
		End Sub


		Public Function Validate(value As String) As Boolean
			Dim regularExpression As New System.Text.RegularExpressions.Regex(getRegularExpressionString())

			Return regularExpression.IsMatch(value)
		End Function

		Public Function Validate(value As String, options As System.Text.RegularExpressions.RegexOptions) As Boolean
			Dim regularExpression As New System.Text.RegularExpressions.Regex(getRegularExpressionString(), options)

			Return regularExpression.IsMatch(value)
		End Function

		Private Function getRegularExpressionString() As String

			If isCustomRegularExpression Then
				Return regularExpressionString
			End If

			Select Case _validationType
				Case ValidationTypes.InternationalMobile
					Return ValidInternationalMobileRegString
				Case ValidationTypes.Email
					Return ValidEmailRegexString
				Case ValidationTypes.IPAddress
					Return ValidIPAddressRegexString
				Case ValidationTypes.Trust
					Return ValidTrustRegexString
				Case ValidationTypes.CompanyRegistation
					Return ValidCompanyRegNotRegexString
				Case ValidationTypes.AlphaChars
					Return ValidAlphaCharRegexString
				Case ValidationTypes.Numeric
					Return ValidNumericRegexString
				Case ValidationTypes.BitCoinAddress
					Return ValidBitcoinRegexSting
				Case ValidationTypes.EthereumAddress
					Return ValidEthereumRegexSting
				Case Else
					Exit Select
			End Select

			Return ""
		End Function
	End Class
End Namespace