Imports System.Text.RegularExpressions

Public Class Strings

  ''' <summary>
  ''' This function will remove markup tags from text
  ''' </summary>
  ''' <param name="HTML"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function StripTags(ByVal HTML As String) As String
    ' Removes tags from passed HTML
    'Dim objRegEx As _
    '    System.Text.RegularExpressions.Regex
    Return System.Text.RegularExpressions.Regex.Replace(HTML, "<[^>]*>", "").Replace("&edsp;", "").ToUpper()

  End Function

  ''' <summary>
  ''' Adds th, st, nd etc to a number. Eg. 1st 2nd 3rd
  ''' </summary>
  Public Shared Function AddOrdinal(ToNumber As Integer) As String

    If ToNumber <= 0 Then
      Return ToNumber.ToString()
    End If

    Select Case (ToNumber Mod 100)
      Case 11, 12, 13
        Return ToNumber.ToString() & "th"
    End Select

    Select Case (ToNumber Mod 10)
      Case 1
        Return ToNumber.ToString() & "st"
      Case 2
        Return ToNumber.ToString() & "nd"
      Case 3
        Return ToNumber.ToString() & "rd"
      Case Else
        Return ToNumber.ToString() & "th"
    End Select

  End Function

  ''' <summary>
  ''' Limits the number of lines of the given Text to Lines (can end with an ellipses '...')
  ''' </summary>
  ''' <param name="Text"></param>
  ''' <param name="Lines"></param>
  ''' <param name="LastLineAsEllipses"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function LimitLines(ByVal Text As String, ByVal Lines As Integer, Optional ByVal LastLineAsEllipses As Boolean = True) As String

    If LastLineAsEllipses Then
      Lines -= 1
    End If

    While Count(vbCrLf, Text) > Lines
      Text = Text.Substring(0, Text.LastIndexOf(vbCrLf))
    End While

    If LastLineAsEllipses Then
      Text &= vbCrLf & "..."
    End If

    Return Text

  End Function

  ''' <summary>
  ''' Indents the given Text by the number of Tabs
  ''' </summary>
  ''' <param name="Text"></param>
  ''' <param name="Tabs"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function Indent(ByVal Text As String, ByVal Tabs As Integer) As String

    Dim sTabs As String = Repeat(vbTab, Tabs)
    Return sTabs & Text.Replace(vbCrLf, vbCrLf & sTabs)

  End Function

  Public Shared Function Repeat(ByVal Text As String, ByVal Number As Integer) As String

    Dim sReturn As String = ""
    For i As Integer = 0 To Number
      sReturn &= Text
    Next
    Return sReturn

  End Function

  Public Shared Function Count(ByVal Word As String, ByVal InSentence As String) As Integer

    Dim iCount As Integer = 0
    While InSentence.IndexOf(Word) <> -1
      InSentence = InSentence.Substring(InSentence.IndexOf(Word) + Word.Length)
      iCount += 1
    End While
    Return iCount

  End Function

  Public Shared Function TabAsSpaces(ByVal wordLength As Integer) As String
    Dim s As String = ""
    For i As Integer = 0 To 9 - wordLength
      s &= " "
    Next
    Return s
  End Function

  Public Shared Function Split(ByVal Sentence As String, ByVal SplitWord As String) As String()

    Dim sReturn(Count(SplitWord, Sentence)) As String
    For i As Integer = 0 To sReturn.Length - 1
      If Sentence.IndexOf(SplitWord) <> -1 Then
        sReturn(i) = Sentence.Substring(0, Sentence.IndexOf(SplitWord))
      Else
        sReturn(i) = Sentence
      End If
      Sentence = Sentence.Substring(Sentence.IndexOf(SplitWord) + SplitWord.Length)
    Next
    Return sReturn

  End Function

  Public Shared Function SplitOnNewLine(Sentence As String) As String()
    Return Sentence.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
  End Function

  ''' <summary>
  ''' Concatenates a list of string, seperating the words with a seperator. Blank words are ignored.
  ''' </summary>
  Public Shared Function Concat(Seperator As String, ParamArray Words As String()) As String

    Dim FinalString As String = String.Empty
    For Each Word As String In Words
      If Not String.IsNullOrEmpty(Word) AndAlso Not Word = Seperator Then
        FinalString &= If(FinalString = String.Empty, String.Empty, Seperator) & Word
      End If
    Next
    Return FinalString

  End Function

  ''' <summary>
  ''' Joins the specified strings together using the supplied seperator.
  ''' </summary>
  ''' <param name="Seperator">How to seperate the strings. E.G, a space or a new line after each string.</param>
  ''' <param name="IncludeBlank">True if blank strings must be added to the joined string.</param>
  ''' <param name="SeperateBlank">True if the seperator must be added after blank strings.</param>
  ''' <param name="Strings">The strings to join together.</param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function Join(ByVal Seperator As String, ByVal IncludeBlank As Boolean, ByVal SeperateBlank As Boolean, ByVal ParamArray Strings() As Object) As String

    Dim Str As String = String.Empty

    If Strings IsNot Nothing Then
      For i As Integer = 0 To Strings.Length - 1
        If i <> 0 AndAlso (IsNothingString(Strings(i)) <> "" OrElse SeperateBlank) Then
          Str &= Seperator
        End If
        If IsNothingString(Strings(i)) <> "" OrElse IncludeBlank Then
          Str &= Strings(i)
        End If
      Next
    End If
   
    Return Str

  End Function

  Public Shared Function RemoveEmptyStrings(ByVal Strings() As String) As String()

    Dim sReturn() As String = Strings
    Dim iNumEmptyStrings As Integer = 0
    For Each s As String In Strings
      If s = "" Then
        ' valid string
        iNumEmptyStrings += 1
      End If
    Next

    If iNumEmptyStrings > 0 Then
      ReDim sReturn(Strings.Length - 1 - iNumEmptyStrings)
      Dim i As Integer = 0
      For Each s As String In Strings
        If s <> "" Then
          ' valid string
          sReturn(i) = s
          i += 1
        End If
      Next
    End If

    Return sReturn

  End Function

  Public Shared Function GetPreNoun(ByVal WordAfter As String) As String
    Select Case WordAfter(0).ToString.ToLower
      Case "a", "e", "i", "o", "u"
        Return "an"
      Case "h"
        If WordAfter(1).ToString.ToLower = "o" Then
          Return "an"
        End If
    End Select

    Return "a"
  End Function

  Public Shared Property PluralizeOverride As PluralizeFunction

  Public Delegate Function PluralizeFunction(ByVal WordToPluralizeWithoutS As String, ByVal WordsEndingWithY_ToEndWithIES As Boolean) As String

  Public Shared Function Pluralize(ByVal WordToPluralizeWithoutS As String, Optional ByVal WordsEndingWithY_ToEndWithIES As Boolean = True) As String

    Static InOverride As Boolean = False
    If Not InOverride AndAlso PluralizeOverride IsNot Nothing Then
      InOverride = True
      Try
        Return PluralizeOverride.Invoke(WordToPluralizeWithoutS, WordsEndingWithY_ToEndWithIES)
      Finally
        InOverride = False
      End Try
    End If

    If WordToPluralizeWithoutS.ToLower = "day" Then
      Return WordToPluralizeWithoutS & "s"
    ElseIf WordToPluralizeWithoutS.ToLower.EndsWith("y") AndAlso WordsEndingWithY_ToEndWithIES Then
      Return WordToPluralizeWithoutS.Substring(0, WordToPluralizeWithoutS.Length - 1) & "ies"
    ElseIf WordToPluralizeWithoutS.ToLower.EndsWith("s") Then
      Return WordToPluralizeWithoutS
    ElseIf WordToPluralizeWithoutS.ToLower.EndsWith("ch") Then
      Return WordToPluralizeWithoutS & "es"
    ElseIf WordToPluralizeWithoutS.ToLower.EndsWith("ess") Then
      Return WordToPluralizeWithoutS & "es"
    Else
      Return WordToPluralizeWithoutS & "s"
    End If

  End Function


  ''' <summary>
  ''' This function will make a number together with a noun sound more correct
  ''' Eg. No Days, 1 Day, 2 Days
  ''' </summary>
  ''' <param name="Number">The number concerned</param>
  ''' <param name="WordToPluralizeWithoutS">The word (without the s)</param>
  ''' <param name="CapitaliseNo">Indicate whether No should be capitalised</param>
  ''' <param name="NoNumberWhen1">Indicates whether the number should be left out if the number is 1</param>
  ''' <param name="NoNumberEver">Indicates whether the number should be left out</param>
  ''' <param name="NumberFormat">How the number will be formatted</param>
  ''' <param name="WordsEndingWithY_ToEndWithIES">To end with ies instead of ys (e.g. 2 Berries, 2 Bays)</param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function Pluralize(ByVal Number As Integer, ByVal WordToPluralizeWithoutS As String, _
      Optional ByVal CapitaliseNo As Boolean = True, Optional ByVal NoNumberWhen1 As Boolean = False, Optional ByVal NoNumberEver As Boolean = False, _
      Optional ByVal NumberFormat As String = "#,##0", Optional ByVal WordsEndingWithY_ToEndWithIES As Boolean = True, Optional UseZero As Boolean = False) As String

    Dim ZeroPhrase As String = "No "
    If UseZero Then
      If CapitaliseNo Then
        ZeroPhrase = "Zero "
      Else
        ZeroPhrase = "zero "
      End If
    Else
      If CapitaliseNo Then
        ZeroPhrase = "No "
      Else
        ZeroPhrase = "no "
      End If
    End If


    If Number = 0 Then
      Return ZeroPhrase & Pluralize(WordToPluralizeWithoutS, WordsEndingWithY_ToEndWithIES)
    ElseIf Number = 1 Then
      If NoNumberWhen1 Or NoNumberEver Then
        Return WordToPluralizeWithoutS
      Else
        If NoNumberEver Then
          Return WordToPluralizeWithoutS
        Else
          Return "1 " & WordToPluralizeWithoutS
        End If
      End If

    Else
      If NoNumberEver Then
        Return Pluralize(WordToPluralizeWithoutS)
      Else
        Return Number.ToString(NumberFormat) & " " & Pluralize(WordToPluralizeWithoutS, WordsEndingWithY_ToEndWithIES)
      End If
    End If

  End Function

  'Public Shared Function Readable(ByVal sName As String) As String

  '  'This new version can handle cases like IDNumber, and will convert it to ID Number, instead of I D Number
  '  'OR UserID -> User ID and not User I D
  '  Dim caps As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_"
  '  For i As Integer = 1 To sName.Length - 2
  '    If caps.Contains(sName(i).ToString) AndAlso Not (caps.Contains(sName(i + 1).ToString) AndAlso caps.Contains(sName(i - 1).ToString)) Then
  '      sName = sName.Insert(i, " ")
  '      i += 1
  '    End If
  '  Next
  '  If sName.EndsWith("ID") Then
  '    sName = sName.Substring(0, sName.Length - 2)
  '  End If
  '  Return sName.Trim

  'End Function

  Public Shared Function Readable(ByVal Name As String) As String

    ' Look for instances of a lower case character followed by an upper case character.
    ' e.g 'ConditionCode' has 'nC' so replace with 'n C' -> 'Condition Code'. 
    Name = Regex.Replace(Name, "[a-z][A-Z]", AddressOf AddSpace)

    ' Look for an instance of two upper case characters followed by a lower case character.
    ' e.g. 'SOPCode' has 'PCo' so replace with 'P Co' -> 'SOP Code'.
    Name = Regex.Replace(Name, "[A-Z]{2,2}[a-z]", AddressOf AddSpace)

    ' Look for an instance of one character followed by a numeric character.
    ' e.g. 'Code1' has 'e1' so replace with 'e 1' -> 'Code 1'.
    Name = Regex.Replace(Name, "[a-zA-Z]\d", AddressOf AddSpace)

    ' Look for an instance of a number followed by a more than 1 letter
    ' e.g. 'Code1' has 'e1' so replace with 'e 1' -> 'Code 1'.
    Name = Regex.Replace(Name, "\d[a-zA-Z]{2,2}", AddressOf AddSpace)

    Return Name

  End Function

#If SILVERLIGHT Then

#Else

  ''' <summary>
  ''' Converts a string to Title Case:
  ''' hello -> Hello, HELLO -> Hello, HelLO -> Hello, two words -> Two Words
  ''' </summary>
  ''' <param name="str">The String to convert</param>
  ''' <returns></returns>
  ''' <remarks>Marlborough March 2010</remarks>
  Public Shared Function ToTitleCase(ByVal str As String) As String
    Dim cultureInfo As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture
    Return cultureInfo.TextInfo.ToTitleCase(str.ToLower)
  End Function

  Public Shared Function ToUTF8(FromSql As String) As String
    Return System.Text.Encoding.UTF8.GetString(System.Text.Encoding.ASCII.GetBytes(FromSql))
  End Function

#End If

  Private Shared Function AddSpace(ByVal Match As Match) As String

    With New System.Text.StringBuilder
      Dim Chars As Char() = Match.Value.ToCharArray
      For Index As Integer = 0 To Chars.GetUpperBound(0)
        .Append(Chars(Index))
        If Index = 0 Then .Append(" "c)
      Next
      Return .ToString
    End With

  End Function

  Public Shared Function PadLeft(ByVal Input As String, ByVal PadChar As String, ByVal Length As Integer) As String

    Input = Right(Input, Length)
    If Input.Length < Length Then
      Return New String(PadChar, Length - Input.Length) & Input
    Else
      Return Input
    End If

  End Function

  Public Shared Function PadRight(ByVal Input As String, ByVal PadChar As String, ByVal Length As Integer) As String

    Input = Left(Input, Length)
    If Input.Length < Length Then
      Return Input & New String(PadChar, Length - Input.Length)
    Else
      Return Input
    End If

  End Function

  ''' <summary>
  ''' Pads left if PadChar is "0", otherwise Pads to the right
  ''' </summary>
  Public Shared Function Pad(ByVal Input As String, ByVal PadChar As String, ByVal Length As Integer) As String

    If PadChar = "0" Then
      Return PadLeft(Input, PadChar, Length)
    Else
      Return PadRight(Input, PadChar, Length)
    End If

  End Function

  Public Shared Function FormatForUltraLinkLabel(ByVal Name As String) As String

    ' Look for any character that is NOT(^) Alphanumeric character or (). : & -
    ' Replace it with an empty string. This character % or & will break link label.
    Name = Regex.Replace(Name, "[^a-z A-Z 0-9 ( ) . : -]", AddressOf AddEmptyString)

    Return Name


  End Function

  ''' <summary>
  ''' Returns true if the String 1 is contained in String 2, or if String 2 is contained in String 1 (Trimmed and Ignoring case)
  ''' </summary>
  ''' <param name="String1"></param>
  ''' <param name="String2"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function Similar(ByVal String1 As String, ByVal String2 As String) As Boolean

    String1 = String1.Trim()
    String2 = String2.Trim()

    If String1.Length > String2.Length Then
      Return String1.ToLower.Contains(String2.ToLower)
    Else
      Return String2.ToLower.Contains(String1.ToLower)
    End If

  End Function

  Private Shared Function AddEmptyString(ByVal Match As Match) As String


    Return String.Empty

  End Function

  ''' <summary>
  ''' Useful when you need to make a list of strings and separate them with a delimiter.
  ''' </summary>
  ''' <param name="Text">The text you will end up with</param>
  ''' <param name="AppendString">The current item to append.</param>
  ''' <param name="Delimiter">The text to go between each item.</param>
  Public Shared Sub Delimit(ByRef Text As String, AppendString As String, Optional Delimiter As String = ", ")
    If Text = "" Then
      Text = AppendString
    Else
      Text &= Delimiter & AppendString
    End If
  End Sub

  Public Shared Function RemoveCommaAndSpace(ByRef Text As String, Optional ByVal ReplacePreviousCommaWithAnd As Boolean = False) As String

    If Text.EndsWith(", ") Then
      Text = Text.TrimEnd(", ".ToCharArray)
    ElseIf Text.EndsWith(",") Then
      Text = Text.TrimEnd(",")
    End If

    If ReplacePreviousCommaWithAnd AndAlso Text.IndexOf(",") <> -1 Then
      Text = String.Format("{0} and{1}", Text.Substring(0, Text.LastIndexOf(",")),
                                         Text.Substring(Text.LastIndexOf(",") + 1))
    End If

    Return Text

  End Function

  Public Shared Function RemoveCommaAndSpaceGet(ByVal Text As String, Optional ByVal ReplacePreviousCommaWithAnd As Boolean = False) As String

    If Text.EndsWith(", ") Then
      Text = Text.TrimEnd(", ".ToCharArray)
    ElseIf Text.EndsWith(",") Then
      Text = Text.TrimEnd(",")
    End If

    If ReplacePreviousCommaWithAnd AndAlso Text.IndexOf(",") <> -1 Then
      Text = String.Format("{0} and{1}", Text.Substring(0, Text.LastIndexOf(",")),
                                         Text.Substring(Text.LastIndexOf(",") + 1))
    End If

    Return Text

  End Function

  Public Shared Function IsNothingString(ByVal Obj As Object, Optional ByVal ReplaceString As String = "") As String

    If Obj Is Nothing OrElse Obj Is DBNull.Value Then
      Return ReplaceString
    Else
      Return CStr(Obj)
    End If

  End Function

  ''' <summary>
  ''' Converts and empty string, nothing or dbnull to dbnull, otherwise returns the string
  ''' </summary>
  ''' <param name="Text"></param>
  ''' <returns></returns>
  ''' <remarks>Andrew C changed 'Text As String' to 'Text as Object'. Function would break if we passed DBNull.Value in.
  ''' 24 Nov 2010 - Marlborough changed it so that if a non text datatype is passed in it will work the same as before Andrews change.</remarks>
  Public Shared Function MakeEmptyDBNull(ByVal Text As Object) As Object
    If Text Is Nothing OrElse Text Is DBNull.Value Then
      Return DBNull.Value
    ElseIf CStr(Text) = "" Or CStr(Text) = String.Empty Then
      Return DBNull.Value
    Else
      Return Text
    End If
  End Function

  Public Shared Function MakeEmptyNothing(ByVal Text As String) As Object
    If Text = "" Or Text = String.Empty Then
      Return Nothing
    Else
      Return Text
    End If
  End Function

  Public Shared Function StringIn(ByVal [String] As String, ByVal InStrings() As String)

    For Each s As String In InStrings
      If [String] = s Then
        Return True
      End If
    Next
    Return False

  End Function

  Public Shared Function GetLastLine(ByVal Text As String) As String
    If Text.IndexOf(vbCrLf) = -1 Then
      Return ""
    End If
    Return Text.Substring(Text.LastIndexOf(vbCrLf) + 2)
  End Function

  Public Shared Function RemoveLastLine(ByVal Text As String) As String
    If Text.IndexOf(vbCrLf) = -1 Then
      Return Text
    End If
    Return Text.Substring(0, Text.LastIndexOf(vbCrLf))
  End Function

#If SILVERLIGHT Then

#Else

  Public Shared Function IncrementString(ByVal text As String) As String
    'Adds a number onto the end of a string
    'e.g  Hello   -> Hello1
    '     Hello55 -> Hello56

    If Not IsNumeric(text(text.Length - 1)) Then
      text &= "1"
    Else

      Dim LastNumberIndex As Integer = text.Length - 1
      While IsNumeric(text(LastNumberIndex))
        LastNumberIndex -= 1
      End While

      Dim tmpCount As Integer = text.Substring(LastNumberIndex + 1)
      tmpCount += 1

      text = text.Substring(0, LastNumberIndex + 1)
      text &= tmpCount

    End If

    Return text

  End Function

#End If

  ''' <summary>
  ''' Like SQL Left(), use if you want to make sure a string is up to a certain length
  ''' </summary>
  Public Shared Function Left(Str As String, Length As Integer) As String
    If Str.Length > Length Then
      Return Str.Substring(0, Length)
    Else
      Return Str
    End If
  End Function

  ''' <summary>
  ''' Like SQL Right(), use if you want to make sure a string is up to a certain length
  ''' </summary>
  Public Shared Function Right(Str As String, Length As Integer) As String
    If Str.Length > Length Then
      Return Str.Substring(Str.Length - Length, Length)
    Else
      Return Str
    End If
  End Function

  Public Shared Function IsSpaceOrTabOrEnter(ByVal ch As Char) As Boolean
    Return ch = " " OrElse ch = vbCr OrElse ch = vbLf OrElse ch = vbTab
  End Function


End Class

