Imports Csla
#If SILVERLIGHT Then
#Else
Imports System.Drawing
#End If
''' <summary>
''' Provides methods for manipulating or comparing objects 
''' </summary>
''' <remarks></remarks>
Public Class Misc
  ''' <summary>
  ''' Method to compare objects type and value
  ''' </summary>
  ''' <param name="obj1">First object that is used in comparing</param>
  ''' <param name="obj2">Second object that is used in comparing</param>
  ''' <returns>Boolean value true if object are the same type and value, otherwise false.</returns>
  ''' <remarks>If an exception happens then false will be returned</remarks>
  Public Shared Function CompareSafe(ByVal obj1 As Object, ByVal obj2 As Object) As Boolean

    If IsDBNull(obj1) Or IsNothing(obj1) Then
      Return IsDBNull(obj2) Or IsNothing(obj2)
    Else
      If IsDBNull(obj2) Or IsNothing(obj2) Then
        Return False
      Else
        Try
          If TypeOf obj1 Is DateTime And TypeOf obj2 Is DateTime Then
            Return DateDiff(DateInterval.Second, CDate(obj1), CDate(obj2)) = 0
          Else
            Return obj1.Equals(obj2)
          End If
        Catch ex As Exception
          ' if that caused an error then just return false
          Return False
        End Try
      End If
    End If

  End Function
  ''' <summary>
  ''' Method to compare objects type and value if object is empty it will convert the object to empty string.
  ''' </summary>
  ''' <param name="obj1">First object that is used in comparing</param>
  ''' <param name="obj2">Second object that is used in comparing</param>
  ''' <returns>Boolean value true if object are the same type and value, otherwise false.</returns>
  ''' <remarks>If an exception happens then false will be returned</remarks>
  Public Shared Function CompareSafeEmptyString(ByVal obj1 As Object, ByVal obj2 As Object) As Boolean

    Return CompareSafe(IsNull(obj1, ""), IsNull(obj2, ""))

  End Function
  ''' <summary>
  ''' Method to compare objects type and value
  ''' </summary>
  ''' <param name="obj1">First object that is used in comparing</param>
  ''' <param name="obj2">Second object that is used in comparing</param>
  ''' <param name="IgnoreCase">Pass true to ignore case</param>
  ''' <returns>Boolean value true if object are the same type and value, otherwise false.</returns>
  ''' <remarks>If an exception happens then false will be returned</remarks>
  Public Shared Function CompareSafe(ByVal obj1 As Object, ByVal obj2 As Object, ByVal IgnoreCase As Boolean) As Boolean

    If IsDBNull(obj1) Then
      Return IsDBNull(obj2)
    Else
      If IsDBNull(obj2) Then
        Return False
      Else
        Try
          If TypeOf obj1 Is String AndAlso TypeOf obj2 Is String AndAlso IgnoreCase Then
            Return CType(obj1, String).ToLower = CType(obj2, String).ToLower
          Else
            Return obj1 = obj2
          End If

        Catch ex As Exception
          ' if that caused an error then just return false
          Return False
        End Try
      End If
    End If

  End Function
  ''' <summary>
  ''' Method to compare objects type and value with Operator
  ''' </summary>
  ''' <param name="obj1">First object that is used in comparing</param>
  ''' <param name="obj2">Second object that is used in comparing</param>
  ''' <param name="Operator">Operate that is used to compare in string format e.g =, >, >= </param>
  ''' <returns>Boolean value true if the comapre if the operator passed is true.</returns>
  ''' <remarks>If an exception happens then false will be returned</remarks>
  Public Shared Function CompareSafe(ByVal obj1 As Object, ByVal obj2 As Object, ByVal [Operator] As String) As Boolean

    If IsDBNull(obj1) Or IsNothing(obj1) Then
      Return IsDBNull(obj2) Or IsNothing(obj2)
    Else
      If IsDBNull(obj2) Or IsNothing(obj2) Then
        Return False
      Else
        Select Case [Operator]
          Case ">"
            Return obj1 > obj2
          Case "<"
            Return obj1 < obj2
          Case ">="
            Return obj1 >= obj2
          Case "<="
            Return obj1 <= obj2
          Case "<>"
            Return obj1 <> obj2
          Case "="
            Return obj1 = obj2
        End Select
      End If
    End If
    Return False

  End Function
  ''' <summary>
  ''' Method to replace the value if is null
  ''' </summary>
  ''' <typeparam name="RType">Object typethat is passed thought in Value parameter</typeparam>
  ''' <param name="Value">Object value that null is being check against</param>
  ''' <param name="ReplaceValue">Replace value if object part in Value parameter is null</param>
  ''' <returns>Value pass in Value parameter unless it is null the it will return the value passed in the ReplaceValue parameter </returns>
  ''' <remarks></remarks>
  Public Shared Function IsNullT(Of RType)(ByVal Value As Object, ByVal ReplaceValue As RType) As RType
    Return If(Value Is Nothing OrElse Value Is DBNull.Value OrElse (TypeOf Value Is String AndAlso Value = ""), ReplaceValue, Value)
  End Function
  ''' <summary>
  ''' Method to replace the value if is null
  ''' </summary>
  ''' <param name="Value">Object value that null is being check against</param>
  ''' <param name="ReplaceValue">Replace value if object part in Value parameter is null</param>
  ''' <returns>Value pass in Value parameter unless it is null the it will return the value passed in the ReplaceValue parameter </returns>
  ''' <remarks></remarks>
  Public Shared Function IsNull(ByVal Value As Object, ByVal ReplaceValue As Object) As Object

    Return IsNullT(Of Object)(Value, ReplaceValue)

  End Function
  ''' <summary>
  ''' Method to check if object value is null or Zero
  ''' </summary>
  ''' <param name="Value">Object value to check against</param>
  ''' <param name="ZeroIncluded">Booleab value to include Zero to null check</param>
  ''' <returns>Boolean value true if object is null otherwise false is returned</returns>
  ''' <remarks>If ZeroIncluded is true must pass numeric object in Value parameter</remarks>
  Public Shared Function IsNullNothing(ByVal Value As Object, ByVal ZeroIncluded As Boolean) As Boolean

    If ZeroIncluded Then
      If (Value Is Nothing OrElse IsDBNull(Value)) OrElse Value = 0 Then
        Return True
      Else
        Return False
      End If
    Else
      If (Value Is Nothing OrElse IsDBNull(Value)) Then
        Return True
      Else
        Return False
      End If
    End If

  End Function
  ''' <summary>
  ''' Method to check if object value is null
  ''' </summary>
  ''' <param name="Value">Object value to check against</param>
  ''' <returns>Boolean value true if object is null or empty string otherwise false is returned</returns>
  ''' <remarks></remarks>
  Public Shared Function IsNullNothing(ByVal Value As Object) As Boolean

    If Value Is Nothing OrElse IsDBNull(Value) Then
      Return True
    Else
      Return False
    End If

  End Function
  ''' <summary>
  ''' Method to check if object value is null or empty
  ''' </summary>
  ''' <param name="Value">Object value to check against</param>
  ''' <returns>Boolean value true if object is null otherwise false is returned</returns>
  ''' <remarks></remarks>
  Public Shared Function IsNullNothingOrEmpty(ByVal Value As Object) As Boolean

    If Value Is Nothing OrElse Value Is DBNull.Value Then
      Return True
    ElseIf TypeOf Value Is String AndAlso CStr(Value).Trim = "" Then
      Return True
    Else
      Return False
    End If

  End Function
  ''' <summary>
  ''' Method to return DBNull.Value if object is null
  ''' </summary>
  ''' <param name="value">Object value to check against</param>
  ''' <returns>The value parameter unless it is null then DBNull.Value is returned</returns>
  ''' <remarks></remarks>
  Public Shared Function NothingDBNull(ByVal value As Object) As Object
    If value Is Nothing Then
      Return DBNull.Value
    ElseIf TypeOf value Is String AndAlso String.Empty.Equals(value) Then
      Return DBNull.Value
    ElseIf TypeOf value Is Date AndAlso value = Date.MinValue Then
      Return DBNull.Value
    Else
      Return value
    End If
  End Function
  ''' <summary>
  ''' Method to return DBNull.Value if object is Zero
  ''' </summary>
  ''' <param name="Value">Object value to check against</param>
  ''' <returns>The value parameter unless it is 0 then DBNull.Value is returned</returns>
  ''' <remarks></remarks>
  Public Shared Function ZeroDBNull(ByVal Value As Object) As Object

    If Value = 0 Then
      Return DBNull.Value
    Else
      Return Value
    End If
    'Return IIf(Value = 0, DBNull.Value, Value)

  End Function
  ''' <summary>
  '''  Method to return DBNull.Value if object is Zero or Nothing
  ''' </summary>
  ''' <param name="Value">Object value to check against</param>
  ''' <returns>The value parameter unless it is 0 or Nothing then DBNull.Value is returned</returns>
  ''' <remarks></remarks>
  Public Shared Function ZeroNothingDBNull(ByVal Value As Object) As Object

    If Value Is Nothing OrElse Value = 0 Then
      Return DBNull.Value
    Else
      Return Value
    End If
    'Return IIf(Value = 0, DBNull.Value, Value)

  End Function

  ''' <summary>
  ''' This function will return nothing if the value is 0
  ''' </summary>
  ''' <param name="Value">Object value to check against</param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function ZeroNothing(ByVal Value As Object) As Object

    If Value = 0 Then
      Return Nothing
    Else
      Return Value
    End If
    'Return IIf(Value = 0, DBNull.Value, Value)

  End Function
  ''' <summary>
  ''' Method to return DBNull.Value if value parmeter equels NullValue parmeter
  ''' </summary>
  ''' <param name="Value">Object value to check against</param>
  ''' <param name="NullValue">Object that will result in DBNull.Value return</param>
  ''' <returns>The value parameter Value = NullValue then DBNull.Value is returned</returns>
  ''' <remarks></remarks>
  Public Shared Function ValueDBNull(ByVal Value As Object, ByVal NullValue As Object) As Object
    If Value = NullValue Then
      Return DBNull.Value
    Else
      Return Value
    End If
  End Function

  Public Shared Function SmartDateEqualsValue(ByVal SmartDate As Csla.SmartDate, ByVal Value As Object) As Boolean

    If IsNothing(Value) OrElse IsDBNull(Value) OrElse (TypeOf Value Is String AndAlso Value = "") Then
      If SmartDate.IsEmpty Then
        ' they are the same
        Return True
      Else
        Return False
      End If
    Else
      ' Value has a value
      If SmartDate.IsEmpty Then
        Return Value = DateTime.MinValue
      Else
        Dim oValue As DateTime = DateTime.MinValue
        If TypeOf Value Is String Then
          oValue = DateTime.Parse(Value)
        ElseIf TypeOf Value Is Nullable(Of DateTime) Then
          oValue = Value
        Else
          oValue = Value
        End If

        Return oValue.Equals(SmartDate.Date)
      End If
    End If

  End Function

  ''' <summary>
  ''' If you ever wished the SQL 'in' clause existed in .net? Then you are in luck! (almost).
  ''' This function will do a similar comparison, with slightly different syntax.
  ''' E.G: Instead of WHERE MainGroupID In (18, 20, 25, 26) use this: If Singular.Misc.In(MainGroupID, 18, 20, 25, 26)
  ''' </summary>
  ''' <param name="CompareObj">See Method Summary</param>
  ''' <param name="CompareObjects">See Method Summary</param>
  ''' <returns></returns>
  ''' <remarks>B Marlborough 26 Aug 09</remarks>
  Public Shared Function [In](ByVal CompareObj As Object, ByVal ParamArray CompareObjects As Object()) As Boolean

    For Each obj As Object In CompareObjects
      If CompareSafe(obj, CompareObj) Then
        Return True
      End If
    Next

    Return False

  End Function

  ''' <summary>
  ''' See Singular.Misc.In 
  ''' This version is faster, but doesn't handle nulls.
  ''' </summary>
  ''' <param name="CompareObj">See Method Summary</param>
  ''' <param name="CompareObjects">See Method Summary</param>
  ''' <returns></returns>
  ''' <remarks>B Marlborough 26 Aug 09</remarks>
  Public Shared Function [InSafe](ByVal CompareObj As String, ByVal ParamArray CompareObjects As String()) As Boolean

    For Each obj As String In CompareObjects
      If CompareObj = obj Then
        Return True
      End If
    Next

    Return False

  End Function

  Public Shared Function TimeSpanToString(ByVal ts As TimeSpan) As String

    Dim IsNegative As Boolean = ts.Ticks < 0
    If IsNegative Then ts = -ts
    Dim str As String = CInt(Math.Floor(ts.TotalMinutes / 60)).ToString("#00") & ":" & (ts.TotalMinutes Mod 60).ToString("00")
    Return IIf(IsNegative, "-", "") & str

  End Function

  ''' <summary>
  ''' Returns the effective annual interest rate.
  ''' </summary>
  ''' <param name="Rate">The monthly / quarterly / etc rate</param>
  ''' <param name="Period">Periods per year. e.g. 12 for monthly interest.</param>
  Public Shared Function Effect(Rate As Decimal, Period As Integer) As Decimal
    Return (1 + Rate / Period) ^ Period - 1
  End Function

#Region " Booleans "
  ''' <summary>
  ''' Calls that contians methods for boolean objects
  ''' </summary>
  Public Class Booleans
    ''' <summary>
    ''' Method to convert boolean value to string value yes/no
    ''' </summary>
    ''' <param name="Value">Boolean value to convert</param>
    ''' <returns>yes/no string value values yes when true else no</returns>
    ''' <remarks></remarks>
    Public Shared Function ConvertBooleanToYesNo(ByVal Value As Boolean) As String
      If Value Then
        Return "yes"
      Else
        Return "no"
      End If
    End Function

  End Class

#End Region

#Region " Dates "

  Public Class Dates

    Public Shared Function DateMonthStart(ByVal ReferenceDate As Date) As Date

      Return New Date(ReferenceDate.Year, ReferenceDate.Month, 1)

    End Function

    Public Shared Function DateMonthEnd(ByVal ReferenceDate As Date, Optional ByVal ToLastSecond As Boolean = False) As Date

      If ToLastSecond Then
        Return New Date(ReferenceDate.Year, ReferenceDate.Month, DateTime.DaysInMonth(ReferenceDate.Year, ReferenceDate.Month)).AddDays(1).AddMilliseconds(-1)
      End If
      Return New Date(ReferenceDate.Year, ReferenceDate.Month, DateTime.DaysInMonth(ReferenceDate.Year, ReferenceDate.Month))

    End Function

  End Class

#End Region

#Region " Colors "

  Public Class Colors

    ''' <summary>
    ''' Returns the best font colour for a specific background colour
    ''' </summary>
    ''' <param name="BackColor">The background colour the text will be displayed on</param>
    ''' <returns>Color.White or Color.Black to be used as the font colour</returns>
    ''' <remarks></remarks>
    Public Shared Function GetTextColorForBackground(BackColor As Color) As Color
#If SILVERLIGHT Then
			Dim WhiteDiff As Integer = CInt(Math.Max(BackColor.R, System.Windows.Media.Colors.White.R) - Math.Min(BackColor.R, System.Windows.Media.Colors.White.R)) + _
				CInt(Math.Max(BackColor.G, System.Windows.Media.Colors.White.G) - Math.Min(BackColor.G, System.Windows.Media.Colors.White.G)) + _
				CInt(Math.Max(BackColor.B, System.Windows.Media.Colors.White.B) - Math.Min(BackColor.B, System.Windows.Media.Colors.White.B))

			Dim BlackDiff As Integer = CInt(Math.Max(BackColor.R, System.Windows.Media.Colors.Black.R) - Math.Min(BackColor.R, System.Windows.Media.Colors.Black.R)) + _
				CInt(Math.Max(BackColor.G, System.Windows.Media.Colors.Black.G) - Math.Min(BackColor.G, System.Windows.Media.Colors.Black.G)) + _
				CInt(Math.Max(BackColor.B, System.Windows.Media.Colors.Black.B) - Math.Min(BackColor.B, System.Windows.Media.Colors.Black.B))

			If WhiteDiff > BlackDiff Then
				Return System.Windows.Media.Colors.White
			Else
				Return System.Windows.Media.Colors.Black
			End If
#Else
      Dim WhiteDiff As Integer = CInt(Math.Max(BackColor.R, Color.White.R) - Math.Min(BackColor.R, Color.White.R)) +
        CInt(Math.Max(BackColor.G, Color.White.G) - Math.Min(BackColor.G, Color.White.G)) +
        CInt(Math.Max(BackColor.B, Color.White.B) - Math.Min(BackColor.B, Color.White.B))

      Dim BlackDiff As Integer = CInt(Math.Max(BackColor.R, Color.Black.R) - Math.Min(BackColor.R, Color.Black.R)) +
        CInt(Math.Max(BackColor.G, Color.Black.G) - Math.Min(BackColor.G, Color.Black.G)) +
        CInt(Math.Max(BackColor.B, Color.Black.B) - Math.Min(BackColor.B, Color.Black.B))

      If WhiteDiff > BlackDiff Then
        Return Color.White
      Else
        Return Color.Black
      End If
#End If


    End Function
  End Class

#End Region

#If SILVERLIGHT Then

#Else

#Region " Passwords "

  Public Class Password

    ''' <summary>
    ''' Gets a random ascii character value between 33 and 126.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetRandomCharInt() As Integer

      Dim arr(0) As Byte
      Dim rndCrypt As New System.Security.Cryptography.RNGCryptoServiceProvider
      rndCrypt.GetBytes(arr)
      Return (arr(0) Mod 93 + 33)

    End Function



    ''' <summary>
    ''' Creates a random password, based on the parameters specified.
    ''' </summary>
    ''' <param name="PasswordLength">How many characters must be generated.</param>
    ''' <param name="IncludeSpecialCharacters">Are special characters allowed? E.g: !#$%^</param>
    ''' <param name="IncludeNumericCharacters">Are numeric characters allowed? E.g: 1234567890</param>
    ''' <param name="IncludeAlphabeticalCapsCharacters">Are capital letters allowed?</param>
    ''' <param name="IncludeAlphabeticalLowercaseCharacters">Are lower case letters allowed?</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateRandomPassword(Optional ByVal PasswordLength As Integer = 6,
                                                Optional ByVal IncludeSpecialCharacters As Boolean = True,
                                                Optional ByVal IncludeNumericCharacters As Boolean = True,
                                                Optional ByVal IncludeAlphabeticalCapsCharacters As Boolean = True,
                                                Optional ByVal IncludeAlphabeticalLowercaseCharacters As Boolean = True) As String

      Dim pwd As String = ""
      'Dim rnd As New Random(Now.Ticks Mod Integer.MaxValue)
      'Special Chars - ASCII: 33 - 47 and 58 - 64 and 91 - 96 and 123 - 126
      'Alphanumeric Chars - ASCII: 48 - 57
      'Alphabetical Caps Chars - ASCII: 65 - 90
      'Alphabetical Lowercase Chars - ASCII: 97 - 122
      While pwd.Length <= PasswordLength
        Dim rndChar As Integer = GetRandomCharInt()
        'Dim rndChar As Integer = rnd.Next(93) + 33
        If (IncludeSpecialCharacters And ((rndChar >= 33 And rndChar <= 47) _
                                                   Or (rndChar >= 58 And rndChar <= 64) _
                                                   Or (rndChar >= 91 And rndChar <= 96) _
                                                   Or (rndChar >= 123 And rndChar <= 126))) _
            Or (IncludeNumericCharacters And rndChar >= 48 And rndChar <= 57) _
            Or (IncludeAlphabeticalCapsCharacters And rndChar >= 65 And rndChar <= 90 And rndChar <> 79 And rndChar <> 76) _
            Or (IncludeAlphabeticalLowercaseCharacters And rndChar >= 97 And rndChar <= 122 And rndChar <> 108 And rndChar <> 111) Then
          pwd &= Char.ConvertFromUtf32(rndChar)

        End If
      End While

      Return pwd

    End Function

    ''' <summary>
    ''' Creates a random password with easy to read letters and no ambiguous characters like {0 O} or {l I 1} etc. 
    ''' </summary>
    Public Shared Function CreateRandomEasyPassword(Optional ByVal PasswordLength As Integer = 6, Optional AllowedChars As String = "ABCDEFGHJKMPQRSTUVWXYZ23456789") As String

      Dim Bytes(PasswordLength - 1) As Byte
      Dim rndCrypt As New System.Security.Cryptography.RNGCryptoServiceProvider
      rndCrypt.GetBytes(Bytes)

      Dim rand As New Random
      Dim pwd As String = ""

      For i As Integer = 0 To Bytes.Length - 1
        pwd &= AllowedChars((Bytes(i) / 256.0) * (AllowedChars.Length - 1))
      Next

      Return pwd

    End Function

    ''' <summary>
    ''' Provides functions that help check if a password complies to specified password rules. Such as minimum length, character / numeric requirements etc.
    ''' </summary>
    ''' <remarks>Passwords</remarks>
    Public Class PasswordChecker

      Private mWhatsWrong As String = ""
      Private mMinLength As Integer = 0
      Private mLowerCase As Integer = 0
      Private mUpperCase As Integer = 0
      Private mNumbers As Integer = 0
      Private mSpecialChars As Integer = 0
      Private mNumRequired As Integer = 0

      ''' <summary>
      ''' Creates a simple password checker that only checks minimum length.
      ''' </summary>
      ''' <param name="MinLength">minimum length of the password</param>
      ''' <remarks></remarks>
      Public Sub New(ByVal MinLength As Integer)
        mMinLength = MinLength
      End Sub

      Public Sub New(ByVal MinLength As Integer, ByVal LowerCase As Integer, ByVal UpperCase As Integer, ByVal Numbers As Integer, ByVal SpecialChars As Integer)
        mMinLength = MinLength
        mLowerCase = LowerCase
        mUpperCase = UpperCase
        mNumbers = Numbers
        mSpecialChars = SpecialChars
      End Sub

      Public Sub New(ByVal MinLength As Integer, ByVal LowerCase As Boolean, ByVal UpperCase As Boolean, ByVal Numbers As Boolean, ByVal SpecialChars As Boolean, ByVal NumRequired As Integer)
        mMinLength = MinLength
        If LowerCase Then
          mLowerCase = 1
        End If
        If UpperCase Then
          mUpperCase = 1
        End If
        If Numbers Then
          mNumbers = 1
        End If
        If SpecialChars Then
          mSpecialChars = 1
        End If
        mNumRequired = NumRequired
      End Sub

      Public ReadOnly Property ErrorMessage() As String
        Get
          Return mWhatsWrong
        End Get
      End Property

      ''' <summary>
      ''' Checks that the specified password complies to the rules specified. 
      ''' </summary>
      ''' <param name="Password"></param>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public Function CheckPassword(ByVal Password As String) As Boolean
        ', Optional NumbersMustBeTogether As Boolean = True

        If Password.Length < mMinLength Then
          mWhatsWrong &= "Must be at least " & mMinLength & " characters." & vbCrLf
        End If

        Dim rLC As New System.Text.RegularExpressions.Regex("^(.*[a-z]){" & mLowerCase & "}.*$")
        Dim rUC As New System.Text.RegularExpressions.Regex("^(.*[A-Z]){" & mUpperCase & "}.*$")
        'Dim rN As New System.Text.RegularExpressions.Regex("(?=(?:[^0-9]*[0-9]){" & mNumbers & "})")
        Dim rN As New System.Text.RegularExpressions.Regex("(.*\d){" & mNumbers & "}.*$")
        'Dim rN As New System.Text.RegularExpressions.Regex("^(?=.*\d{" & mNumbers & "})")
        'Dim rX As New System.Text.RegularExpressions.Regex("(?=(.*\d){" & mNumbers & "})")

        'Const Specialchars As String = "~`!@#$%\^&\*()-_=+<>,\.\?\\\{\}\[\]:;'"""
        Const Specialchars As String = "~`!@#$%^&*()+=<>?.,"

        Dim rSC As New System.Text.RegularExpressions.Regex("^(.*[" & Specialchars & "]){" & mSpecialChars & "}.*$")

        Dim HasLower As Boolean = False
        Dim HasUpper As Boolean = False
        Dim HasNumber As Boolean = False
        Dim HasSpecial As Boolean = False

        Dim Count As Integer = 0

        If rLC.IsMatch(Password) And mLowerCase > 0 Then
          HasLower = True
          Count += 1
        End If
        If rUC.IsMatch(Password) And mUpperCase > 0 Then
          HasUpper = True
          Count += 1
        End If
        If rN.IsMatch(Password) And mNumbers > 0 Then
          HasNumber = True
          Count += 1
        End If
        'If NumbersMustBeTogether Then
        '  'This is forcing for x number of numerics directly after each other
        '  If rN.IsMatch(Password) And mNumbers > 0 Then
        '    HasNumber = True
        '    Count += 1
        '  End If
        'Else
        '  'This is checking for x number of numerics, dont need to be directly after each other
        '  If Not HasNumber Then
        '    If rX.IsMatch(Password) And mNumbers > 0 Then
        '      HasNumber = True
        '      Count += 1
        '    End If
        '  End If
        'End If
        If rSC.IsMatch(Password) And mSpecialChars > 0 Then
          HasSpecial = True
          Count += 1
        End If

        If mNumRequired > 0 Then
          If Count < mNumRequired Then

            mWhatsWrong = "Password requires any " & mNumRequired & " of the following: "
            If mLowerCase > 0 Then
              mWhatsWrong &= "Lower Case Characters, "
            End If
            If mUpperCase > 0 Then
              mWhatsWrong &= "Upper Case Characters, "
            End If
            If mNumbers > 0 Then
              mWhatsWrong &= "Numbers, "
            End If
            If mSpecialChars > 0 Then
              mWhatsWrong &= "Special Characters, "
            End If
            If mWhatsWrong.EndsWith(", ") Then
              mWhatsWrong = mWhatsWrong.Substring(0, mWhatsWrong.Length - 2)
            End If
          End If

        Else
          If Not HasLower AndAlso mLowerCase > 0 Then
            mWhatsWrong &= "Password requires " & Singular.Strings.Pluralize(mLowerCase, "lower case character") & "." & vbCrLf
          End If
          If Not HasUpper AndAlso mUpperCase > 0 Then
            mWhatsWrong &= "Password requires " & Singular.Strings.Pluralize(mUpperCase, "upper case character") & "." & vbCrLf
          End If
          If Not HasNumber AndAlso mNumbers > 0 Then
            mWhatsWrong &= " Password requires " & Singular.Strings.Pluralize(mNumbers, "numeric character") & "." & vbCrLf
          End If
          If Not HasSpecial AndAlso mSpecialChars > 0 Then
            mWhatsWrong &= "Password requires " & Singular.Strings.Pluralize(mSpecialChars, "special character") & "." & vbCrLf
          End If
        End If

        Return mWhatsWrong = ""

      End Function

      Public Function CheckPassword(ByVal UserName As String, ByVal Password As String) As Boolean

        CheckPassword(Password)

        If Password.ToLower.Contains(UserName.ToLower) Then
          mWhatsWrong &= vbCrLf & "Password cannot contain user name."
        End If

        Return mWhatsWrong = ""

      End Function

    End Class

    '''' <summary>
    '''' Checks a password for passing generic requirements of a password policy
    '''' </summary>
    '''' <param name="Password">The new password to match against</param>
    '''' <param name="MinLength">The minimum length of the password</param>
    '''' <param name="LowerCaseChars">Number of lower case characters. 0 = Doesnt have to have any.</param>
    '''' <param name="UpperCaseChars">Number of upper case characters. 0 = Doesnt have to have any.</param>
    '''' <param name="Numbers">Number of numeric characters. 0 = Doesnt have to have any.</param>
    '''' <returns>String saying whats wrong with the password. Empty string if it passes.</returns>
    '''' <remarks></remarks>
    'Public Shared Function CheckPassword(ByVal Password As String, ByVal MinLength As Integer, ByVal LowerCaseChars As Integer, ByVal UpperCaseChars As Integer, ByVal Numbers As Integer) As String

    '  Dim WhatsWrong As String = ""

    '  If Password.Length < MinLength Then
    '    WhatsWrong &= "Must be at least " & MinLength & " characters." & vbCrLf
    '  End If

    '  Dim rLC As New System.Text.RegularExpressions.Regex("^(?=.*[a-z]{" & LowerCaseChars & "})")
    '  Dim rUC As New System.Text.RegularExpressions.Regex("^(?=.*[A-Z]{" & UpperCaseChars & "})")
    '  Dim rN As New System.Text.RegularExpressions.Regex("^(?=.*\d{" & Numbers & "})")

    '  If Not rLC.IsMatch(Password) Then
    '    WhatsWrong &= "Requires " & Singular.Strings.Pluralize(LowerCaseChars, "lower case character") & "." & vbCrLf
    '  End If
    '  If Not rUC.IsMatch(Password) Then
    '    WhatsWrong &= "Requires " & Singular.Strings.Pluralize(UpperCaseChars, "upper case character") & "." & vbCrLf
    '  End If
    '  If Not rN.IsMatch(Password) Then
    '    WhatsWrong &= "Requires " & Singular.Strings.Pluralize(Numbers, "numeric character") & "." & vbCrLf
    '  End If

    '  If WhatsWrong <> "" Then
    '    Return "Password " & WhatsWrong
    '  Else
    '    Return ""
    '  End If
    'End Function

  End Class

#End Region

#Region " Versioning "

  Public Class Versioning

    Public Enum VersionResult
      Same = 1
      Older = 2
      Newer = 3
    End Enum

    Public Shared Function IsLatestVersion(TestVersion As String, LatestVersion As String) As VersionResult

      Dim TestVersionParts As String() = TestVersion.Split(".")
      Dim LatestVersionParts As String() = LatestVersion.Split(".")

      If TestVersionParts.Length <> LatestVersionParts.Length Then
        Return VersionResult.Older
      Else
        'Loop through all the vesion numbers starting at the left
        For i As Integer = 0 To TestVersionParts.Length - 1

          If CInt(TestVersionParts(i)) < CInt(LatestVersionParts(i)) Then

            Return VersionResult.Older

          ElseIf CInt(TestVersionParts(i)) > CInt(LatestVersionParts(i)) Then

            Return VersionResult.Newer

          End If

        Next

        Return VersionResult.Same

      End If

    End Function

  End Class

#End Region

#End If

#Region " Times "

  Public Enum TimeStringSingleNumberType
    Hours
    Minutes
    Seconds
  End Enum

  Public Enum TimeFormats
    LongTime
    ShortTime
    TimeSpan
    Custom
  End Enum

  Public Shared Function DateTimeEqualsTimeValue(ByVal TimeDateTime As DateTime, ByVal Value As Object, ByVal SingleNumberType As TimeStringSingleNumberType) As Boolean

    If Value Is Nothing OrElse IsDBNull(Value) OrElse (TypeOf Value Is String AndAlso Value = "") Then
      If TimeDateTime = DateTime.MinValue Then
        ' they are the same
        Return True
      Else
        Return False
      End If
    Else
      ' Value has a value
      If TimeDateTime <> DateTime.MinValue Then
        Return CStr(Value).Replace(":", "").Replace("0", "") = ""
      Else
        Dim oValue As TimeSpan
        If TypeOf Value Is String Then
          oValue = GetTimeSpanFromString(CStr(Value), SingleNumberType)
        ElseIf TypeOf Value Is Nullable(Of TimeSpan) Then
          oValue = Value
        ElseIf TypeOf Value Is DateTime Then
          oValue = CDate(Value).TimeOfDay
        Else
          oValue = Value
        End If

        Return oValue.Equals(TimeDateTime.TimeOfDay)
      End If
    End If

  End Function

  Public Shared Function GetTimeSpanFromString(ByVal Value As String, ByVal SingleNumberType As TimeStringSingleNumberType) As TimeSpan

    Dim IsNegative As Boolean = False
    If Value.StartsWith("-") Then
      IsNegative = True
      Value = Value.Substring(1)
    End If
    Dim ReturnValue As TimeSpan = Nothing
    If IsDate(Value) Then
      Dim MyDate As DateTime = DateTime.Parse(Value)
      ReturnValue = MyDate.TimeOfDay
    ElseIf Value.Contains(":") Then
      ' has time components in string
      If TimeSpan.TryParse(Value, ReturnValue) Then
        Return ReturnValue
      End If
    ElseIf IsNumeric(Value) Then
      If CStr(Value).Length > 2 Then
        While CStr(Value).Length Mod 2 <> 0
          Value = CStr(Value) + "0"
        End While

        Dim Hours, Minutes, Seconds As Integer
        For i As Integer = 0 To 2
          Dim UnitNumber As String = Left(Value, 2)
          Value = Right(Value, Len(Value) - 2)
          Select Case i
            Case 0
              Hours = CInt(UnitNumber)
            Case 1
              Minutes = CInt(UnitNumber)
            Case 2
              Seconds = CInt(UnitNumber)
          End Select
          If Value = "" Then
            Exit For
          End If
        Next
        ReturnValue = New TimeSpan(Hours, Minutes, Seconds)
      Else
        Select Case SingleNumberType
          Case TimeStringSingleNumberType.Hours
            ReturnValue = New TimeSpan(CInt(Value), 0, 0)
          Case TimeStringSingleNumberType.Minutes
            ReturnValue = New TimeSpan(0, CInt(Value), 0)
          Case TimeStringSingleNumberType.Seconds
            ReturnValue = New TimeSpan(0, 0, CInt(Value))
        End Select
      End If

    End If
    If IsNegative Then
      ReturnValue = -ReturnValue
    End If
    Return ReturnValue

  End Function

#End Region

#Region " IDNumber / Company / Trust Nos / Sars Tax Nos "

  ''' <summary>
  ''' Contains methods to help with extracting gender and birth date data from a South African ID Number, as well as validating an ID Number.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class IDNumber

    ''' <summary>
    ''' Calculates the control digit (The 13th number) of the ID Number, for comparison with the actual 13th number, to check if the ID No is valid.
    ''' </summary>
    ''' <param name="IdNo"></param>
    ''' <returns></returns>
    ''' <remarks>ID No ID Number</remarks>
    Public Shared Function GetControlDigit(ByVal IdNo As String) As Integer

      ' In accordance with the legislation, 
      'the control figure which is the 13th digit of all identity numbers which have 08 and 09 is calculated as follows using ID Number 800101 5009 087 as an example:

      'Add all the digits in the odd positions (excluding last digit).
      '  8 + 0 + 0 + 5 + 0 + 0 = 13...................[1] 
      'Move the even positions into a field and multiply the number by 2.
      '11098: x(2 = 22196)
      'Add the digits of the result in b).
      '  2 + 2 + 1 + 9 + 6 = 20.........................[2] 
      'Add the answer in [2] to the answer in [1].
      '  13 + 20 = 33 
      'Subtract the second digit (i.e. 3) from 10.  The number must tally with the last number in the ID Number. If the result is 2 digits, the last digit is used to compare against the last number in the ID Number.  If the answer differs, the ID number is invalid. 

      Dim a As Integer = 0

      Try

        For i As Integer = 0 To 5
          a += CInt(IdNo.Substring(i * 2, 1))
        Next

        Dim b As Integer = 0
        For i As Integer = 0 To 5
          b = b * 10 + CInt(IdNo.Substring(2 * i + 1, 1))
        Next
        b *= 2
        Dim c As Integer = 0
        Do
          c += b Mod 10
          b = Int(b / 10)
        Loop Until b <= 0
        c += a
        Dim d As Integer = 0
        d = 10 - (c Mod 10)
        If (d = 10) Then d = 0
        Return d

      Catch ex As Exception
        Return -1
      End Try

    End Function

    ''' <summary>
    ''' Will attempt to set the given BirthDate to the birth date from to the ID Number
    ''' </summary>
    ''' <param name="BirthDate"></param>
    ''' <param name="IDNumber"></param>
    ''' <returns>True if the set was successful and value changed, False if unsuccessful</returns>
    ''' <remarks>Changed Jan 2011 by Marlborough to not duplicate code from GetBirthDateFromIDNumber()</remarks>
    Public Shared Function TrySetBirthDateFromIDNumber(ByRef BirthDate As SmartDate, ByVal IDNumber As String) As Boolean

      Dim CalcDate As SmartDate = GetBirthDateFromIDNumber(IDNumber)

      If CalcDate.IsEmpty Then
        Return False
      Else
        BirthDate = CalcDate
        Return True
      End If

    End Function

    ''' <summary>
    ''' Will attempt to set the Gender from the given ID Number
    ''' </summary>
    ''' <param name="Gender"></param>
    ''' <param name="IDNumber"></param>
    ''' <param name="Male"></param>
    ''' <param name="Female"></param>
    ''' <returns>True if the set was successful and value changed, False if unsuccessful</returns>
    ''' <remarks></remarks>
    Public Shared Function TrySetGenderFromIDNumber(ByRef Gender As Object, ByVal IDNumber As String, Optional ByVal Male As Object = "M", Optional ByVal Female As Object = "F") As Boolean

      Dim CalcGender As Object = GetGenderFromIDNumber(IDNumber, Male, Female)

      If CalcGender Is Nothing Then
        Return False
      Else
        If Not Singular.Misc.CompareSafe(Gender, CalcGender) Then
          Gender = CalcGender
        End If
        Return True
      End If

    End Function

    ''' <summary>
    ''' Will try and return a new SmartDate containing the birthdate of the ID number
    ''' If this is not possible an empty SmartDate will be returned
    ''' </summary>
    ''' <param name="IDNumber"></param>
    ''' <returns></returns>
    ''' <remarks>Changed Jan 2011 by Marlborough</remarks>
    Public Shared Function GetBirthDateFromIDNumber(ByVal IDNumber As String, Optional ByVal MinimumAgeInyears As Integer = 0) As SmartDate

      Dim IDNo As String = IDNumber.Replace(" ", "")
      If IDNo.Length = 13 Then

        Try
          Dim Day As Integer = IDNo.Substring(4, 2)
          Dim Month As Integer = IDNo.Substring(2, 2)
          Dim Year As Integer = Now.Year.ToString.Substring(0, 2) & IDNo.Substring(0, 2)

          Dim tmpDate As New Date(Year, Month, Day)

          If tmpDate > Now.Date.AddYears(-MinimumAgeInyears) Then
            tmpDate = tmpDate.AddYears(-100)
          End If

          Return New SmartDate(tmpDate)
        Catch ex As Exception
          Return New SmartDate()
        End Try

      End If
      Return New SmartDate()

    End Function

    ''' <summary>
    ''' Gets the Gender from the passed in ID Number.
    ''' </summary>
    ''' <param name="IDNumber">The ID Number to extract the gender from.</param>
    ''' <param name="Male">How does your business object store Male?</param>
    ''' <param name="Female">How does your business object store Female?</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetGenderFromIDNumber(ByVal IDNumber As String, Optional ByVal Male As Object = "M", Optional ByVal Female As Object = "F") As Object

      Dim IDNo As String = IDNumber.Replace(" ", "")
      If IDNo.Length = 13 AndAlso Char.IsDigit(IDNo.Chars(6)) Then
        If CInt(CStr(IDNo(6))) < 5 Then
          Return Female
        Else
          Return Male
        End If
      Else
        Return Nothing
      End If

    End Function

    ''' <summary>
    ''' Checks if an ID Number is valid by checking the length, if there are alphabet characters, if the birth date is valid, and if the check digit is correct.
    ''' </summary>
    ''' <param name="IDNumber">The ID Number to check</param>
    ''' <param name="Errors">Any errors with the ID Number will be written to this string.</param>
    ''' <param name="NonSouthAfrican">Are you checking non South African ID Numbers? This will affect the validation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ValidIDNumber(ByVal IDNumber As String, ByRef Errors As String, Optional ByVal NonSouthAfrican As Boolean = False) As Boolean

      Singular.Formatting.FormatIDNumber(IDNumber, Errors, NonSouthAfrican)

      Return Errors = ""

    End Function

    ''' <summary>
    ''' Checks to see whether a given Birth Date corresponds to a given ID Number.
    ''' </summary>
    ''' <param name="IDNumber">The ID Number to compare.</param>
    ''' <param name="BirthDate">The Birth Date to compare.</param>
    ''' <returns>True or false depending on the comparison result.</returns>
    ''' <author>Mark van Dyk</author>
    ''' <created>2011-01-18</created>
    ''' <remarks>Changed 19 Jan 2011 by Marlborough</remarks>
    Public Shared Function IDNoMatchesBirthDate(ByVal IDNumber As String, ByVal BirthDate As Object) As Boolean

      If Singular.Misc.IsNullNothing(BirthDate, False) Then
        Return False
      Else

        Dim CalcDate As SmartDate = GetBirthDateFromIDNumber(IDNumber)
        If CalcDate.IsEmpty Then
          Return False
        Else
          Dim dBirthDate As Date
          If TypeOf BirthDate Is SmartDate Then
            dBirthDate = CType(BirthDate, SmartDate).Date.Date
          Else
            dBirthDate = CType(BirthDate, DateTime).Date
          End If
          Return dBirthDate.Month = CalcDate.Date.Month AndAlso dBirthDate.Day = CalcDate.Date.Day AndAlso dBirthDate.Year Mod 100 = CalcDate.Date.Year Mod 100
        End If

      End If

    End Function

  End Class

  ''' <summary>
  ''' Contains methods for validating Company Registration Numbers and Trust Numbers.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class CompanyTrustNos

    Public Shared Function IsTrustNoValid(ByVal TrustNo As String, ByRef Errors As String) As Boolean

      Dim regTrust As New System.Text.RegularExpressions.Regex("^IT[0-9]{5}[/][0-9]{4}$")

      Errors = ""

      If TrustNo = "" Then
        Errors = "Trust Number Required."
        Return False
      Else
        If Not regTrust.IsMatch(TrustNo) Then
          Errors = "Trust Number needs to be in the format ITxxxxx/yyyy."
          Return False
        End If
        Dim Year As Integer = CInt(TrustNo.Substring(8, 4))
        If Year < 1900 Or Year > Now.Year Then
          Errors = "Trust Number is invalid."
          Return False
        End If
      End If

      Return True

    End Function

    Public Shared Function IsCompanyRegNoValid(ByVal RegistrationNo As String, ByRef Errors As String, Optional MinYear As Integer = 1900) As Boolean

      Dim regCompany As New System.Text.RegularExpressions.Regex("^[0-9]{4}[/][0-9]{6}[/][0-9]{2}$")

      Errors = ""

      If RegistrationNo = "" Then
        Errors = "Registration No Required."
        Return False
      Else
        If Not regCompany.IsMatch(RegistrationNo) Then
          Errors = "Registration Number needs to be in the format yyyy/xxxxxx/xx"
          Return False
        End If
        Dim Year As Integer = CInt(RegistrationNo.Substring(0, 4))
        If Year < 1900 Or Year > Now.Year Then
          Errors = "Registration Number is invalid."
          Return False
        End If
        Dim suffix As String = RegistrationNo.Substring(12, 2)
        If Not [In](suffix, "23", "07", "06", "08", "09", "10", "11", "20", "21", "22", "24", "25", "26") Then
          Errors = "Registration Number is invalid."
          Return False
        End If
      End If

      Return True

    End Function

  End Class

  Public Class TaxRefNo

    ''' <summary>
    ''' Checks if a SARS Tax Ref No is valid using the modulus 10 check.
    ''' </summary>
    ''' <param name="TaxRefNo">The Ref No to be checked.</param>
    ''' <param name="StripGroupingCharacters">If this function must ignore grouing characters such as / and -.</param>
    ''' <param name="Errors">Description of what is wrong.</param>
    Public Shared Function IsTaxRefNoValid(ByVal TaxRefNo As String, ByVal StripGroupingCharacters As Boolean, ByRef Errors As String) As Boolean

      Dim TaxRefNoClean As String = TaxRefNo
      If StripGroupingCharacters Then
        TaxRefNoClean = TaxRefNoClean.Replace("-", "").Replace("/", "")
      End If

      If TaxRefNoClean.Length <> 10 Then
        Errors = "Tax Ref No must contain 10 numbers."
        Return False
      End If

      If Not IsNumeric(TaxRefNoClean) Then
        Errors = "Tax Ref No cannot contain letters."
        Return False
      End If

      Dim odd As Integer = 0
      Dim even As Integer = 0
      Dim RefNoArray(10) As Char

      RefNoArray = TaxRefNoClean.ToCharArray()

      'Reverse the array
      Array.Reverse(RefNoArray, 0, 10)

      'Multiply every second number by two and get the sum. 
      'Get the sum of the rest of the numbers.
      For i As Integer = 0 To 9

        If (i Mod 2 = 0) Then
          odd += (Convert.ToInt32(RefNoArray.GetValue(i)) - 48)
        Else

          Dim temp As Integer = (Convert.ToInt32(RefNoArray(i)) - 48) * 2
          'if the value is greater than 9, substract 9 from the value
          If (temp > 9) Then
            temp = temp - 9
          End If
          even += temp
        End If
      Next
      If ((odd + even) Mod 10 = 0) Then
        Return True
      Else
        Errors = "Invalid Check Digit on Tax Ref No."
        Return False
      End If


    End Function

  End Class

#End Region

#Region " IP Address "

  Public Class IPAddress

    Public Shared ReadOnly Property ValidIPAddressRegexString As String
      Get
        Return "\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b"
      End Get
    End Property

  End Class

#End Region

#Region " Emails "

  ' ******************************************************************************
  ' *******  Functions Moved To Singular.Emails **********************************
  ' ******************************************************************************

  'Public Class Email

  '  Public Shared ReadOnly Property ValidEmailRegexString As String
  '    Get
  '      Return "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
  '    End Get
  '  End Property

  '  Public Shared Function ValidEmailAddress(ByVal EmailAddress As String) As Boolean
  '    Dim reg As New System.Text.RegularExpressions.Regex(ValidEmailRegexString, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
  '    Return reg.IsMatch(EmailAddress)
  '  End Function

  '  'Public Shared Sub SendMail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, ByVal FriendlyFrom As String, ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, ByVal SMTPAddress As String)

  '  '  SendMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, New System.IO.Stream() {}, Nothing, , FriendlyFrom)

  '  'End Sub

  '  'Public Shared Sub SendMail(ByVal FromEmailAddress As String, ByVal FriendlyFrom As String, ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, ByVal SMTPAddress As String, ByVal AccountUserName As String, ByVal AccountPassword As String)

  '  '  SendMail(FromEmailAddress, AccountPassword, ToEmailAddress, Subject, Body, SMTPAddress, New System.IO.Stream() {}, Nothing, AccountUserName, FriendlyFrom)

  '  'End Sub

  '  'Public Shared Sub SendMail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, ByVal FriendlyFrom As String, ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, ByVal SMTPAddress As String, ByVal FilesToAttach As String())

  '  '  SendMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, FilesToAttach, FriendlyFrom)

  '  'End Sub

  '  'Public Shared Sub SendMail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, ByVal FriendlyFrom As String, ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, ByVal SMTPAddress As String, ByVal Files() As System.IO.Stream, ByVal FileNames() As String)

  '  '  SendMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, Files, FileNames, , FriendlyFrom)

  '  'End Sub

  '  'Public Shared Sub SendMail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, ByVal FriendlyFrom As String, ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, ByVal SMTPAddress As String, ByVal File As System.IO.Stream, ByVal FileName As String)

  '  '  SendMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, New System.IO.Stream() {File}, New String() {FileName}, , FriendlyFrom)

  '  'End Sub

  '  'Public Shared Sub SendMail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, ByVal SMTPAddress As String, ByVal FilesToAttach As String(), Optional ByVal FriendlyFrom As String = "")

  '  '  Dim Files(FilesToAttach.Length) As System.IO.Stream
  '  '  Dim FileNames(FilesToAttach.Length) As String
  '  '  If Not FilesToAttach Is Nothing Then
  '  '    For i As Integer = 0 To FilesToAttach.Length - 1
  '  '      Files(i) = New System.IO.MemoryStream(System.IO.File.ReadAllBytes(FilesToAttach(i)))
  '  '      FileNames(i) = System.IO.Path.GetFileName(FilesToAttach(i))
  '  '    Next

  '  '  End If

  '  '  SendMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, Files, FileNames, , FriendlyFrom)

  '  'End Sub

  '  'Public Shared Sub SendMail(ByVal FromEmailAddress As String, ByVal FromEmailAddressPassword As String, _
  '  '                            ByVal ToEmailAddress As String, ByVal Subject As String, ByVal Body As String, _
  '  '                            ByVal SMTPAddress As String, ByVal Files() As System.IO.Stream, _
  '  '                            ByVal FileNames() As String, Optional ByVal AccountUserName As String = "", Optional ByVal FriendlyFrom As String = "")

  '  '  Dim sm As New Singular.Emails.SingularMail(FromEmailAddress, FromEmailAddressPassword, ToEmailAddress, Subject, Body, SMTPAddress, Files, FileNames, AccountUserName, FriendlyFrom)
  '  '  sm.SendMail()

  '  'End Sub

  'End Class

#End Region

#Region " Comparers "
#If Silverlight = False Then


  Public Class Comparer
    Implements IComparer

    Private mComparer As Func(Of Object, Object, Integer)

    Public Sub New(Comparer As Func(Of Object, Object, Integer))
      mComparer = Comparer
    End Sub

    Function Compare(x As Object, y As Object) As Integer _
       Implements IComparer.Compare
      Return mComparer.Invoke(y, x)
    End Function

  End Class

  Public Class Comparer(Of T)
    Implements IComparer

    Private mComparer As Func(Of T, T, Integer)

    Public Sub New(Comparer As Func(Of T, T, Integer))
      mComparer = Comparer
    End Sub

    Function Compare(x As Object, y As Object) As Integer _
       Implements IComparer.Compare
      Return mComparer.Invoke(y, x)
    End Function

  End Class

#End If
#End Region

#Region " Logging "

  Public Class LogEntry

    Public Enum LogEntryType
      Undefined = 0
      Success = 1
      Warning = 2
      Failure = 3
    End Enum

    <Singular.DataAnnotations.DateField(FormatString:="dd MMM yyyy HH:mm:ss")>
    Public Property CreatedDate As Date
    Public Property EntryText As String
    Public Property EntryType As LogEntryType

    Public Sub New(EntryText As String, Optional EntryType As LogEntryType = LogEntryType.Undefined)
      Me.CreatedDate = Now
      Me.EntryText = EntryText
      Me.EntryType = EntryType
    End Sub

  End Class

#End Region

  Public Class Numbers

    Private Shared EncodeChars As String = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"

    ''' <summary>
    ''' Converts from base 10 to some other base. Max base is 62. Encode characters start with numbers then upper case, then lower case.
    ''' </summary>
    ''' <param name="Value">The number to encode</param>
    ''' <param name="Base">The base to encode to (max 62)</param>
    Public Shared Function BaseXEncode(Value As Long, Base As Byte) As String

      If Value < 0 Then Throw New ArgumentException("Value must be positive")
      If Base < 2 OrElse Base > EncodeChars.Length Then Throw New ArgumentException("Base must be between 2 and " & EncodeChars.Length)

      Dim Encoded = String.Empty

      Do
        Encoded = EncodeChars(Math.Floor(Value Mod Base)) & Encoded

        Value = Math.Floor(Value / Base)
      Loop While Value <> 0

      Return Encoded

    End Function

    ''' <summary>
    ''' Converts from base x to base 10. Encoded string must have been generated using EncodeChars using the same base.
    ''' </summary>
    ''' <param name="Value">The encoded string in base x.</param>
    ''' <param name="Base">The base the string is encoded in.</param>
    Public Shared Function BaseXDecode(Value As String, Base As Byte) As Long

      If Base < 2 OrElse Base > EncodeChars.Length Then Throw New ArgumentException("Base must be between 2 and " & EncodeChars.Length)
      If Value.Any(Function(c) Not EncodeChars.Contains(c)) Then Throw New ArgumentException("Invalid Value string")

      Dim Decoded = 0

      For i As Integer = 0 To Value.Length - 1
        Decoded += EncodeChars.IndexOf(Value(i)) * (Base ^ (Value.Length - i - 1))
      Next

      Return Decoded

    End Function

  End Class

End Class

Public Interface IProgress
  Property CurrentStep As Integer
  Property MaxSteps As Integer
  Property CurrentStatus As String
  Property Errors As String

  Sub Increment()
End Interface

Public Interface IUpdatableProgress
  Inherits IProgress

  Sub Update(message As String, Optional CurrentStep As Integer = -1)

End Interface

Public Enum MessageType
  Information = 1
  [Error] = 2
  Validation = 3
  Success = 4
  Warning = 5
End Enum

Public Class Message
  Public Property MessageType As MessageType
  Public Property MessageTitle As String
  Public Property Message As String

  Public Sub New(MessageType As MessageType, MessageTitle As String, Message As String)
    Me.MessageType = MessageType
    Me.MessageTitle = MessageTitle
    Me.Message = Message
  End Sub

  Public Sub New()

  End Sub

  ''' <summary>
  ''' Time in milliseconds to wait until the message fades out. 0 or less means dont fade.
  ''' </summary>
  Public Property FadeAfter As Integer = 0
End Class




Public Class ExpiringLazy(Of T)

  Private _Lock As New Object
  Private _Value As T
  Private _HasValue As Boolean = False
  Private _LastCreatedTime As Date
  Private _ValueFactory As Func(Of T)
  Private _ExpiryPeriod As TimeSpan

  Public Sub New(ValueFactory As Func(Of T), ExpiryPeriod As TimeSpan)
    _ValueFactory = ValueFactory
    _ExpiryPeriod = ExpiryPeriod
  End Sub

  Public ReadOnly Property Value As T
    Get

      SyncLock _Lock

        If _HasValue Then
          If _LastCreatedTime.Add(_ExpiryPeriod) < Now Then
            _HasValue = False
          End If
        End If

        If Not _HasValue Then
          _Value = _ValueFactory()
          _HasValue = True
          _LastCreatedTime = Now
        End If

        Return _Value

      End SyncLock

    End Get
  End Property

End Class