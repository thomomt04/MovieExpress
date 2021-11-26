Namespace Emails

  Public Module MailSettingsModule

    Private mMailSettings As SingularMailSettings = Nothing

    Public Property MailSettings() As SingularMailSettings
      Get
        If mMailSettings Is Nothing Then
          mMailSettings = New SingularMailSettings()
        End If
        Return mMailSettings
      End Get
      Set(ByVal value As SingularMailSettings)
        mMailSettings = value
      End Set
    End Property

    Private mAllowExportToOutlook As Boolean = True

    Public Property AllowExportToOutlook() As Boolean
      Get
        Return mAllowExportToOutlook
      End Get
      Set(ByVal value As Boolean)
        mAllowExportToOutlook = value
      End Set
    End Property

    Public Property TreatCCFieldAsBCC As Boolean = False

    Public ReadOnly Property ValidEmailRegexString As String
      Get
        Return "[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?"
      End Get
    End Property

		Public Function ValidEmailAddress(ByVal EmailAddress As String) As Boolean
			Dim Trimmed As String = EmailAddress.Trim()
			Dim reg As New System.Text.RegularExpressions.Regex(ValidEmailRegexString, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
			For Each match As System.Text.RegularExpressions.Match In reg.Matches(Trimmed)
				If match.Value = Trimmed Then
					Return True
				End If
			Next
			Return False
			'Return reg.IsMatch(EmailAddress)
		End Function

    Public Property HTMLWrapperStart As String = "<html><body><span style=""font-family: Calibri, Arial; font-size: 11pt;"">"
    Public Property HTMLWrapperEnd As String = "</span></body></html>"

  End Module

End Namespace
