Imports Csla

Public Class Formatting

#Region " Phone number formatting "

  Public Class PhoneNumberFormatting

    Public DefaultCountryCode As String = "27" '  South Africa
    Public DefaultAreaCode As String = "011" ' Johannesburg
    Public RemoveCountryCodeIfDefault As Boolean = True

    Public Sub New(Optional ByVal DefaultCountryCode As String = "27", _
                  Optional ByVal DefaultAreaCode As String = "011", _
                  Optional ByVal RemoveCountryCodeIfDefault As Boolean = True)

      Me.DefaultCountryCode = DefaultCountryCode
      Me.DefaultAreaCode = DefaultAreaCode
      Me.RemoveCountryCodeIfDefault = RemoveCountryCodeIfDefault

    End Sub

  End Class

  Private Shared mPhoneNumberFormatting As PhoneNumberFormatting = New PhoneNumberFormatting

  Public Shared Sub SetPhoneNumberFormatting(ByVal PhoneNumberFormatting As PhoneNumberFormatting)

    mPhoneNumberFormatting = PhoneNumberFormatting

  End Sub

  Public Shared Function FormatPhoneNumber(ByVal PhoneNumber As String, ByRef PhoneNumberErrors As String, Optional ByVal IsCellNumber As Boolean = False) As String

    If IsNothing(PhoneNumber) Then Return ""

    PhoneNumberErrors = ""

    ' first get rid of any crap in the number, will reformat at the end
    Dim NewNumber As String = PhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "")

    If NewNumber <> "" Then
      If Not IsNumeric(NewNumber) Then
        PhoneNumberErrors = "Phone number cannot contain non numeric characters"
      ElseIf NewNumber.Length < 9 Then
        ' number is not even close!
        PhoneNumberErrors = "Phone number too short"
      ElseIf NewNumber.Length > 11 Then
        ' phone number is too long
        PhoneNumberErrors = "Phone number too long"
      ElseIf NewNumber.StartsWith("27") AndAlso NewNumber.Length = 11 Then
        'valid number
        NewNumber = NewNumber.Insert(2, "0")
      ElseIf NewNumber.StartsWith("0") AndAlso NewNumber.Length = 10 Then
        'valid number
      ElseIf Not NewNumber.StartsWith("0") AndAlso NewNumber.Length = 9 Then
        'probably just forgot the 0 or +27
        NewNumber = NewNumber.Insert(0, "0")
      Else
        PhoneNumberErrors = "Incomplete phone number"
      End If

      If PhoneNumberErrors = "" Then
        ' format the number from right to left
        Dim NewNumberChars() As Char = NewNumber.ToCharArray
        NewNumber = ""
        Dim indexFromRight As Integer = 0
        Dim bClosingBracketWithoutOpening As Boolean = False
        Dim bCountryCodePresent As Boolean = False
        For i As Integer = NewNumberChars.Length - 1 To 0 Step -1
          If indexFromRight = 4 Then
            NewNumber = "-" & NewNumber
          ElseIf indexFromRight = 7 Then
            NewNumber = ") " & NewNumber
            bClosingBracketWithoutOpening = True
          ElseIf indexFromRight = 10 Then
            NewNumber = " (" & NewNumber
            bClosingBracketWithoutOpening = False
          ElseIf indexFromRight > 10 Then
            bCountryCodePresent = True
          End If
          NewNumber = NewNumberChars(i) & NewNumber
          indexFromRight += 1
        Next
        If bClosingBracketWithoutOpening Then
          NewNumber = "(" & NewNumber
        End If
        If bCountryCodePresent Then
          NewNumber = "+" & NewNumber
        End If

      End If

    End If

    If mPhoneNumberFormatting.RemoveCountryCodeIfDefault Then
      If NewNumber.StartsWith("+" & mPhoneNumberFormatting.DefaultCountryCode) Then
        NewNumber = NewNumber.Substring(("+" & mPhoneNumberFormatting.DefaultCountryCode).Length)
      End If
    End If

    Return NewNumber

  End Function


#End Region

#Region " ID Number Formatting "

  Public Shared Function FormatIDNumber(ByVal IDNumber As String, ByRef IDNumberErrors As String, Optional ByVal NonSouthAfrican As Boolean = False) As String

    Dim NewNumber As String = ""

    If Not NonSouthAfrican Then
      NewNumber = IDNumber.Replace(" ", "")
      If NewNumber <> "" Then
        If Not IsNumeric(NewNumber) Then
          IDNumberErrors = "ID number cannot contain non numeric characters"
        ElseIf NewNumber.Length < 13 Then
          ' number is too short
          IDNumberErrors = "ID number too short"
        ElseIf NewNumber.Length > 13 Then
          ' number is too long
          IDNumberErrors = "ID number too long"
        Else
          Try
            Dim sdDate As SmartDate = Misc.IDNumber.GetBirthDateFromIDNumber(NewNumber)
            If sdDate.IsEmpty Then
              IDNumberErrors = "Invalid Date part of ID Number"
            ElseIf sdDate > Now Then
              ' how can somebody be born in the future
              IDNumberErrors = "Birth date of ID Number is a future date"
            End If
          Catch ex As Exception
            IDNumberErrors = "Invalid ID Number"
          End Try

          Dim cd As Integer = Singular.Misc.IDNumber.GetControlDigit(NewNumber)
          If cd.ToString <> NewNumber(12) Then
            IDNumberErrors = "Incorrect Check Digit on ID Number."
          End If
        End If

        ' now format it accordingly
        If IDNumberErrors = "" Then

          NewNumber = NewNumber.Insert(6, " ")
          If NewNumber.Length > 11 Then
            NewNumber = NewNumber.Insert(11, " ")
          End If
        End If
      End If
    End If

    Return NewNumber

  End Function

#End Region

#Region " General Formatting "

  Public Enum DataType
    [Other] = 0
    [String] = 1
    [Date] = 2
    [Number] = 3
  End Enum

  Public Enum NumberFormatType
    NotSet = 0
    NoFormatting = 1
    NoDecimals = 2
    Decimals = 3
    PercentNoDecimals = 4
    Percent = 5
    CurrencyNoDecimals = 6
    CurrencyDecimals = 7
  End Enum

  Public Class FormatContainer

    Private Const DecimalSeperatorTempChar As Char = "^"
    Private _CurrencySymbol As String = "R"

    Public Number_NoFormatting As String = "#"
    Public Number_NoDecimals As String = "#,##0;(#,##0)"
    Public Number_Decimals As String = "#,##0.00;(#,##0.00)"
    Public Number_PercentNoDecimals As String = "#,##0 %;(#,##0) %"
    Public Number_Percent As String = "#,##0.00 %;(#,##0.00) %"
    Public Number_CurrencyNoDecimals As String = "R #,##0;R (#,##0)"
    Public Number_CurrencyDecimals As String = "R #,##0.00;R (#,##0.00)"

    Public Date_OnlyDate As String = "dd MMM yyyy"
    Public Date_MonthYear As String = "MMM yyyy"
    Public Date_MonthYearFull As String = "MMMM yyyy"
    Public Date_DateAndTime As String = "dd MMM yyyy - HH:mm:ss"
    Public Date_OnlyTime As String = "HH:mm:ss"

    Public Function GetNumberFormatString(CommonNumberFormat As NumberFormatType) As String
      Select Case CommonNumberFormat
        Case NumberFormatType.NoFormatting
          Return Number_NoFormatting
        Case NumberFormatType.NoDecimals
          Return Number_NoDecimals
        Case NumberFormatType.Decimals
          Return Number_Decimals
        Case NumberFormatType.PercentNoDecimals
          Return Number_PercentNoDecimals
        Case NumberFormatType.Percent
          Return Number_Percent
        Case NumberFormatType.CurrencyNoDecimals
          Return Number_CurrencyNoDecimals
        Case NumberFormatType.CurrencyDecimals
          Return Number_CurrencyDecimals
      End Select
      Return "#"
    End Function

    Public Sub ReplaceCurrencySymbol(NewSymbol As String)
      Number_CurrencyNoDecimals = Number_CurrencyNoDecimals.Replace(_CurrencySymbol, NewSymbol)
      Number_CurrencyDecimals = Number_CurrencyDecimals.Replace(_CurrencySymbol, NewSymbol)
      _CurrencySymbol = NewSymbol
    End Sub

    Public Sub ReplaceThousandsAndDecimalCharacter(NewThousandsCharacter As String, NewDecimalCharacter As String)
      Number_NoDecimals = Number_NoDecimals.Replace(",", NewThousandsCharacter)
      Number_PercentNoDecimals = Number_PercentNoDecimals.Replace(",", NewThousandsCharacter)
      Number_CurrencyNoDecimals = Number_CurrencyNoDecimals.Replace(",", NewThousandsCharacter)

      'First replace the decimal seperator to something that wont likely be used by thousands.
      Number_Decimals = Number_Decimals.Replace(".", DecimalSeperatorTempChar)
      Number_Percent = Number_Percent.Replace(".", DecimalSeperatorTempChar)
      Number_CurrencyDecimals = Number_CurrencyDecimals.Replace(".", DecimalSeperatorTempChar)

      'Replace thousands
      Number_Decimals = Number_Decimals.Replace(",", NewThousandsCharacter)
      Number_Percent = Number_Percent.Replace(",", NewThousandsCharacter)
      Number_CurrencyDecimals = Number_CurrencyDecimals.Replace(",", NewThousandsCharacter)

      'Replace decimals
      Number_Decimals = Number_Decimals.Replace(DecimalSeperatorTempChar, NewDecimalCharacter)
      Number_Percent = Number_Percent.Replace(DecimalSeperatorTempChar, NewDecimalCharacter)
      Number_CurrencyDecimals = Number_CurrencyDecimals.Replace(DecimalSeperatorTempChar, NewDecimalCharacter)
    End Sub

    Public ReadOnly Property CurrencySymbol As String
      Get
        Return _CurrencySymbol
      End Get
    End Property

    Public Function GetJavascript() As String
      Dim sb As New Text.StringBuilder
      sb.AppendLine("NFormats = {}")
      sb.AppendLine("NFormats.NoFormatting = '" & Number_NoFormatting & "';")
      sb.AppendLine("NFormats.NoDecimals = '" & Number_NoDecimals & "';")
      sb.AppendLine("NFormats.Decimals = '" & Number_Decimals & "';")
      sb.AppendLine("NFormats.Percent = '" & Number_Percent & "';")
      sb.AppendLine("NFormats.PercentNoDecimals = '" & Number_PercentNoDecimals & "';")
      sb.AppendLine("NFormats.CurrencyDecimals = '" & Number_CurrencyDecimals & "';")
      sb.AppendLine("NFormats.CurrencyNoDecimals = '" & Number_CurrencyNoDecimals & "';")
      Return sb.ToString
    End Function

  End Class

  Public Shared DefaultFormats As FormatContainer = New FormatContainer
  Private Shared FormatStringProvider As Func(Of FormatContainer) = Function()
                                                                      Return DefaultFormats
                                                                    End Function

  Public Shared Sub SetFormatStringProvider(Provider As Func(Of FormatContainer))
    FormatStringProvider = Provider
  End Sub

  Public Shared Function GetCurrentFormats() As FormatContainer
    Return FormatStringProvider()
  End Function

  Public Class SingularFormat
    Private mDataType As DataType
    Private mFormatName As String
    Private mFormatString As String
    Private mCustomFormat As Boolean

    Public Sub New(ByVal DataType As DataType, ByVal formatName As String, ByVal formatString As String, Optional ByVal customFormat As Boolean = False)
      mDataType = DataType
      mFormatName = formatName
      mFormatString = formatString
      mCustomFormat = customFormat
    End Sub

    Public ReadOnly Property DataType() As DataType
      Get
        Return mDataType
      End Get
    End Property

    Public ReadOnly Property FormatName() As String
      Get
        Return mFormatName
      End Get
    End Property

    Public ReadOnly Property FormatString() As String
      Get
        Return mFormatString
      End Get
    End Property
  End Class

  Public Class SingularFormatList

    Private mFormatList As List(Of SingularFormat)

    Public Sub New(Optional ByVal AddCustomFormats As Boolean = False)
      mFormatList = New List(Of SingularFormat)

      If AddCustomFormats Then
        mFormatList.Add(New SingularFormat(DataType.String, "Don't change", "", True))
        mFormatList.Add(New SingularFormat(DataType.String, "Lower Case", "l", True))
        mFormatList.Add(New SingularFormat(DataType.String, "Upper Case", "u", True))
        mFormatList.Add(New SingularFormat(DataType.String, "Title Case", "s", True))
      End If

      mFormatList.Add(New SingularFormat(DataType.Date, "Only Date", GetCurrentFormats.Date_OnlyDate))
      mFormatList.Add(New SingularFormat(DataType.Date, "Date And Time", GetCurrentFormats.Date_DateAndTime))
      mFormatList.Add(New SingularFormat(DataType.Date, "Only Time", GetCurrentFormats.Date_OnlyTime))

      mFormatList.Add(New SingularFormat(DataType.Number, "No Decimals", GetCurrentFormats.Number_NoDecimals))
      mFormatList.Add(New SingularFormat(DataType.Number, "Show Decimals", GetCurrentFormats.Number_Decimals))
      mFormatList.Add(New SingularFormat(DataType.Number, "Percent With No Decimals", GetCurrentFormats.Number_PercentNoDecimals))
      mFormatList.Add(New SingularFormat(DataType.Number, "Percent With Decimals", GetCurrentFormats.Number_Percent))
    End Sub

    Public Function GetFormats() As List(Of SingularFormat)
      Return mFormatList
    End Function

    Public Function GetFormats(ByVal Type As DataType) As List(Of SingularFormat)
      Dim fList As New List(Of SingularFormat)
      For Each sf As SingularFormat In mFormatList
        If sf.DataType = Type Then
          fList.Add(sf)
        End If
      Next
      Return fList
    End Function

    Public Function GetFormats(ByVal Type As Type) As List(Of SingularFormat)
      Return GetFormats(GetSimpleTypeFromType(Type))
    End Function

    Public Shared Function GetSimpleTypeFromType(ByVal type As Type) As DataType
      If type Is GetType(String) Then
        Return DataType.String
      ElseIf type Is GetType(Boolean) Then
        Return DataType.Other
      ElseIf type Is GetType(DateTime) Or type Is GetType(Date) Or type Is GetType(Csla.SmartDate) Or type Is GetType(Nullable(Of Date)) Then
        Return DataType.Date
      ElseIf type Is GetType(Integer) Or type Is GetType(Decimal) Or type Is GetType(Double) Or type Is GetType(Byte) Or type Is GetType(Int16) Or type Is GetType(Int32) Or type Is GetType(Int64) Then
        Return DataType.Number
      Else
        Return DataType.Other
      End If

    End Function

    Public Shared Function GetDefaultFormat(ByVal sfl As List(Of SingularFormat)) As SingularFormat
      If sfl IsNot Nothing AndAlso sfl.Count > 0 Then
        Return sfl(0)
      Else
        Return New SingularFormat(DataType.Other, "None", "", False)
      End If
    End Function


  End Class

#End Region

#Region " Name Formatting "

  Public Class NameFormatting

    Public Enum NameFormatting
      TitleFirstNameLastName = 0
      TitleLastNameCommaFirstName = 1
      TitleInitialsLastName = 2
      TitleLastNameCommaInitials = 3
      FirstNameLastName = 4
      LastNameCommaFirstName = 5
      InitialsLastName = 6
      LastNameCommaInitials = 7
      LastNameCommaTitleFirstName = 8
    End Enum

    '  <Obsolete("Use Singular Version Instead")> _
    Public Shared Function FormatNameString(ByVal Title As String, ByVal FirstName As String, ByVal LastName As String, ByVal Formatting As NameFormatting, Optional ByVal Initials As String = "") As String

      If Initials = "" Then
        Initials = GetInitials(FirstName)
      End If
      If Title = "." Then
        Title = ""
      End If
      Select Case Formatting
        Case NameFormatting.TitleFirstNameLastName
          Return Title & IIf(Title = "", "", IIf(Title.EndsWith("."), " ", ". ")) & FirstName & " " & LastName
        Case NameFormatting.TitleLastNameCommaFirstName
          Return Title & IIf(Title = "", "", IIf(Title.EndsWith("."), " ", ". ")) & LastName & ", " & FirstName
        Case NameFormatting.TitleInitialsLastName
          Return Title & IIf(Title = "", "", IIf(Title.EndsWith("."), " ", ". ")) & Initials & " " & LastName
        Case NameFormatting.TitleLastNameCommaInitials
          Return Title & IIf(Title = "", "", IIf(Title.EndsWith("."), " ", ". ")) & LastName & ", " & Initials
        Case NameFormatting.FirstNameLastName
          Return FirstName & " " & LastName
        Case NameFormatting.LastNameCommaFirstName
          Return LastName & ", " & FirstName
        Case NameFormatting.InitialsLastName
          Return Initials & " " & LastName
        Case NameFormatting.LastNameCommaInitials
          Return LastName & ", " & Initials
        Case NameFormatting.LastNameCommaTitleFirstName
          Return LastName & ", " & IIf(Title = "", "", Title & IIf(Title.EndsWith("."), "", ".")) & " " & FirstName
      End Select

      Return ""

    End Function

    Public Shared Function GetInitials(ByVal FirstName As String) As String

      Dim sReturn As String = ""
      For Each Name As String In FirstName.Split(" ")
        If Name <> "" Then
          If sReturn <> "" Then
            sReturn &= " "
          End If
          sReturn &= Name.ToUpper.Substring(0, 1) & "."
        End If
      Next
      Return sReturn

    End Function

  End Class


#End Region

End Class
