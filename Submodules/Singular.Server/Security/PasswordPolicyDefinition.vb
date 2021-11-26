Namespace Security

  Public Class PasswordPolicyDefinition

    Private mEncryptPassword As Boolean = False
    ''' <summary>
    ''' True if passwords are enrypted and not hashed.
    ''' </summary>
    <Obsolete("Encrypted Passwords should no longer be used.")>
    Public Property EncryptPassword As Boolean
      Get
        Return mEncryptPassword
      End Get
      Set(value As Boolean)
        mEncryptPassword = value
      End Set
    End Property

#If SILVERLIGHT Then
#Else
    Public Function GetHashedPassword(PlainText As String) As String
      If mEncryptPassword Then
        Return Singular.Encryption.EncryptString(PlainText)
      Else
        Return Singular.Encryption.GetStringHash(PlainText, Encryption.HashType.Sha256, True)
      End If
    End Function
#End If

    Public Property EnforcePasswordPolicy As Boolean = True

    Public Property PasswordExpires As Boolean = False

    Public Property PasswordExpiryDays As Integer = 30

    Public Property MaxPasswordLength As Integer = 50

    Public Property MinPasswordLength As Integer = 6

    Public Property NoOfSpecialCharacters As Integer = 1

    Public Property NoOfUpperCaseLetters As Integer = 1

    Public Property NoOfLowerCaseLetters As Integer = 1

    Public Property NoOfNumericCharacters As Integer = 1

    Public Property AllowConsecutiveCharacters As Boolean = False

    <Obsolete("Please use corrected spelled SpecialCharacters version instead")>
    Public Property SpecialCharactes As String
      Get
        Return Me.SpecialCharacters
      End Get
      Set(value As String)
        Me.SpecialCharacters = value
      End Set
    End Property

    Public Property SpecialCharacters As String = "!@#$%^&*"

    Public Property BannedCharacters As String = "_+-."

    Public Property WarningPasswordExpiresDays As Integer = 7

    Public Property MonthsPasswordNeedsToBeUnique As Integer = 24

    Public Function ValidatePassword(ByVal Password As String, ByRef Message As String, Optional LineBreaksAsNewLine As Boolean = False) As Boolean

      Message = ""

      'Minimum Length
      If Password.Trim.Length < Me.MinPasswordLength Then
        Message += Singular.Localisation.Localisation.LocalText("Password has to be at least {0} character(s) long.", Me.MinPasswordLength.ToString) + If(LineBreaksAsNewLine, "<br />", Environment.NewLine)
      End If

      'Maximum Length
      If Password.Trim.Length > Me.MaxPasswordLength Then
        Message += Singular.Localisation.Localisation.LocalText("Password cannot be more than {0} character(s) long.", Me.MaxPasswordLength.ToString) + If(LineBreaksAsNewLine, "<br />", Environment.NewLine)
      End If

      'Special Characters
      If Me.NoOfSpecialCharacters > 0 Then
        Dim count As Integer = 0
        Dim UsedSpecialCharacter As List(Of Char) = (From cPas In Password
                                                     Join cSpecial In Me.SpecialCharacters On cPas Equals cSpecial
                                                     Select cPas).ToList()

        'For Each ch As Char In Password
        '  For Each sp As Char In Me.SpecialCharactes
        '    If ch = sp AndAlso Not UsedSpecialCharacter.Contains(ch) Then
        '      UsedSpecialCharacter.Add(ch)
        '      count = count + 1
        '    End If
        '  Next
        'Next
        If UsedSpecialCharacter.Count < Me.NoOfSpecialCharacters Then
          Message += Singular.Localisation.Localisation.LocalText("Password requires at least {0} special character(s).", Me.NoOfSpecialCharacters.ToString) + If(LineBreaksAsNewLine, "<br />", Environment.NewLine)
        End If
      End If

      'Banned Characters
      Dim foundBannedChar As Boolean = False
      Dim bannedPasswordCharacters = (From cPas In Password
                                      Join cBanned In Me.BannedCharacters On cPas Equals cBanned
                                      Select cPas).ToList()
      For Each ch In bannedPasswordCharacters
        foundBannedChar = True
        Message += Singular.Localisation.Localisation.LocalText("Password contains banned characters ({0}).", Me.BannedCharacters) + If(LineBreaksAsNewLine, "<br />", Environment.NewLine)
        Exit For
      Next


      'Upper Case Letters
      If Me.NoOfUpperCaseLetters > 0 Then
        Dim count As Integer = 0
        For Each ch As Char In Password
          If Char.IsUpper(ch) Then
            count = count + 1
          End If
        Next
        If count < Me.NoOfUpperCaseLetters Then
          Message += Singular.Localisation.Localisation.LocalText("Password requires at least {0} upper case character(s).", Me.NoOfUpperCaseLetters.ToString) + If(LineBreaksAsNewLine, "<br />", Environment.NewLine)
        End If
      End If

      'Lower Case Letters
      If Me.NoOfLowerCaseLetters > 0 Then
        Dim count As Integer = 0
        For Each ch As Char In Password
          If Char.IsLower(ch) Then
            count = count + 1
          End If
        Next
        If count < Me.NoOfLowerCaseLetters Then
          Message += Singular.Localisation.Localisation.LocalText("Password requires at least {0} lower case character(s).", Me.NoOfLowerCaseLetters.ToString) + If(LineBreaksAsNewLine, "<br />", Environment.NewLine)
        End If
      End If

      'Consecutive Characters
      Dim foundConsecutiveChars As Boolean = False
      If Not Me.AllowConsecutiveCharacters Then
        If Password.Length > 1 Then
          For index As Integer = 0 To Password.Length - 2 Step +1
            If Not foundConsecutiveChars Then
              If Password(index) = Password(index + 1) Then
                foundConsecutiveChars = True
                Message += Singular.Localisation.Localisation.LocalText("Password cannot contain duplicate consecutive characters.") + If(LineBreaksAsNewLine, "<br />", Environment.NewLine)
              End If
            End If
          Next
        End If
      End If

      'Numeric Characters
      If Me.NoOfNumericCharacters > 0 Then
        Dim count As Integer = 0
        For Each ch As Char In Password
          If Char.IsNumber(ch) Then
            count = count + 1
          End If
        Next
        If count < Me.NoOfNumericCharacters Then
          Message += Singular.Localisation.Localisation.LocalText("Password requires at least {0} numeric character(s).", Me.NoOfNumericCharacters.ToString) + If(LineBreaksAsNewLine, "<br />", Environment.NewLine)
        End If
      End If

      If Message <> "" Then
        Return False
      Else
        Return True
      End If

    End Function

    Public Function CheckPasswordHasExpired(ByVal PasswordChangedDate As Date, ByRef DaysExpired As Integer) As Boolean

      DaysExpired = (PasswordChangedDate.AddDays(Me.PasswordExpiryDays).Date - Now.Date).Days

      'Password Expiry
      If Me.PasswordExpires Then
        If PasswordChangedDate.AddDays(Me.PasswordExpiryDays).Date <= Now.Date Then
          Return True
        End If
      End If

      Return False

    End Function

  End Class

End Namespace