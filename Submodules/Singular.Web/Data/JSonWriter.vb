Namespace Data

  Public Enum OutputType
    JSon = 1
    Javascript = 2
  End Enum

  Public Class JSonWriter

    Private mOutputType As OutputType = OutputType.JSon
    Public Property OutputMode As OutputType
      Get
        Return mOutputType
      End Get
      Set(value As OutputType)
        If mOutputType <> value Then
          mOutputType = value
          Quote = If(mOutputType = OutputType.JSon, """", "'")
        End If
      End Set
    End Property

    Private Quote As Char = """"
    Const Colon As Char = ":"
    Const Comma As Char = ","

    Public Class WriterContext
      Public FirstRow As Boolean = True
      Public FirstProperty As Boolean = True
      Public FirstValue As Boolean = True
    End Class

    Private mSB As Text.StringBuilder
    'Private mFirstClass As Boolean = True
    Private mCurrentContext As WriterContext
    Private mContextStack As Stack

    Public Sub New()
      mSB = New Text.StringBuilder(1024)
      mContextStack = New Stack
      mCurrentContext = New WriterContext
    End Sub

    Private Sub StartContext()
      mContextStack.Push(mCurrentContext)
      mCurrentContext = New WriterContext
    End Sub

    Private Sub EndContext()
      mCurrentContext = mContextStack.Pop
    End Sub

    Public Sub StartClass(PropertyName As String)

      WritePropertyName(PropertyName)

      mSB.Append("{") 'Start of Item
      StartContext()
    End Sub

    Public Sub Write(Value As String)
      mSB.Append(Value)
    End Sub

    Public Sub WriteProperty(PropertyName As String, Value As Object)
      WritePropertyName(PropertyName)
      WriteJSonValue(Value)
    End Sub

    Public Sub WritePropertyRaw(PropertyName As String, Value As Object)
      WritePropertyName(PropertyName)
      Write(Value)
    End Sub

    Public Sub WritePropertyName(PropertyName As String, Optional CheckPropertyName As Boolean = True)

      'Property Seperator
      If mCurrentContext.FirstProperty Then
        mCurrentContext.FirstProperty = False
      Else
        mSB.Append(Comma)
      End If

      If Not CheckPropertyName OrElse PropertyName <> "" Then
        If mOutputType = OutputType.JSon Then
          mSB.Append(Quote).Append(PropertyName).Append(Quote)
        Else
          mSB.Append(PropertyName)
        End If
        mSB.Append(Colon)
      End If

    End Sub

    Public Sub WriteJSonValue(Value As Object)
      WriteJSonValue(Value, mSB, Quote)
    End Sub

    Public Shared Function GetJSonValue(Value As Object, QuoteChar As Char) As String
      Dim sb As New Text.StringBuilder
      WriteJSonValue(Value, sb, QuoteChar)
      Return sb.ToString
    End Function

    Public Shared Sub WriteJSonValue(Value As Object, sb As Text.StringBuilder, QuoteChar As Char)

      If Value Is Nothing OrElse Value Is DBNull.Value Then
        '  Nothing
        sb.Append("null")
      ElseIf TypeOf Value Is String Then
        '  String
        sb.Append(QuoteChar)
        WriteEscapedJavaScriptString(sb, DirectCast(Value, String), If(QuoteChar = "'", SingleQuoteCharEscapeFlags, DoubleQuoteCharEscapeFlags))
        sb.Append(QuoteChar)

      ElseIf TypeOf Value Is Integer Then
        '  Integer
        If DirectCast(Value, Integer) < 0 Then
          sb.Append("-")
          Value = -Value
        End If
        sb.Append(CType(Value, UInteger).ToString(System.Globalization.CultureInfo.InvariantCulture))

      ElseIf TypeOf Value Is Long Then
        '  Integer
        If DirectCast(Value, Long) < 0 Then
          sb.Append("-")
          Value = -Value
        End If
        sb.Append(CType(Value, ULong).ToString(System.Globalization.CultureInfo.InvariantCulture))

      ElseIf TypeOf Value Is Decimal Then
        '  Decimal
        sb.Append(DirectCast(Value, Decimal).ToString(System.Globalization.CultureInfo.InvariantCulture))
      ElseIf TypeOf Value Is Boolean Then
        '  Boolean
        sb.Append(If(DirectCast(Value, Boolean), "true", "false"))
      ElseIf TypeOf Value Is Date Or TypeOf Value Is SmartDate Then
        '  Date

        If TypeOf Value Is SmartDate Then
          Value = DirectCast(Value, SmartDate).DBValue
        End If

        If TypeOf Value Is Date Then
          If Value = Date.MinValue Then
            sb.Append("null")
          Else
            sb.Append(QuoteChar)
            If CType(Value, Date) = CType(Value, Date).Date Then
              sb.Append(DirectCast(Value, Date).ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture))
            Else
              sb.Append(DirectCast(Value, Date).ToString("dd MMM yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
            End If
            sb.Append(QuoteChar)
          End If
        Else
          sb.Append("null")
        End If

      ElseIf TypeOf Value Is System.Drawing.Color Then
        '  Colour
        sb.Append(QuoteChar)
        Dim c As System.Drawing.Color = Value
        sb.Append(System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c.R, c.G, c.B)))
        sb.Append(QuoteChar)
      ElseIf Value.GetType.IsEnum Then
        sb.Append(Convert.ChangeType(Value, System.Enum.GetUnderlyingType(Value.GetType)))
      ElseIf TypeOf Value Is Array OrElse Singular.Reflection.TypeImplementsInterface(Value.GetType, GetType(IEnumerable)) Then
        Dim Serialiser As New Data.JS.StatelessJSSerialiser(Value)
        sb.Append(Serialiser.GetJSon())
      Else
        '  Other
        sb.Append(QuoteChar)
        sb.Append(Value.ToString)
        sb.Append(QuoteChar)
      End If

    End Sub

    Public Sub EndClass()
      mSB.Append("}")
      EndContext()
    End Sub

    Public Sub StartArray()
      StartArray("")
    End Sub

    Public Sub StartArray(PropertyName As String)
      WritePropertyName(PropertyName)
      mSB.Append("[")
      StartContext()
    End Sub

    Public Sub WriteArrayValue(Value As Object)
      If mCurrentContext.FirstValue Then
        mCurrentContext.FirstValue = False
      Else
        mSB.Append(",")
      End If
      WriteJSonValue(Value)
    End Sub

    Public Sub StartObject()
      If mCurrentContext.FirstRow Then
        mCurrentContext.FirstRow = False
      Else
        mSB.Append(Comma)
      End If
      mSB.Append("{")
      StartContext()
    End Sub

    Public Sub EndObject()
      mSB.Append("}")
      EndContext()
    End Sub

    Public Sub EndArray()
      mSB.Append("]")
      EndContext()
    End Sub

    Public Overrides Function ToString() As String

      Return mSB.ToString

    End Function

    Public Sub WriteObject(PropertyName As String, Obj As Object, Optional Context As String = "", Optional RenderGuids As Boolean? = Nothing)

      If Obj IsNot Nothing Then
        Dim Serialiser As New Data.JS.StatelessJSSerialiser(Obj)
        If RenderGuids IsNot Nothing Then
          Serialiser.RenderGuid = RenderGuids
        End If
        Serialiser.ContextList.AddContext(Context)
        Serialiser.RootPropertyName = PropertyName
        Serialiser.GetJSon(Me)
      End If

    End Sub

    Public Shared Function SerialiseObject(List As Object, Optional Context As String = "", Optional RenderGuids As Boolean? = Nothing,
                                           Optional Output As OutputType = OutputType.JSon) As String

      Dim jw As New Data.JSonWriter
      jw.OutputMode = Output
      jw.WriteObject("", List, Context, RenderGuids)
      Return jw.ToString

    End Function

    Private Shared SingleQuoteCharEscapeFlags(127) As Boolean
    Private Shared DoubleQuoteCharEscapeFlags(127) As Boolean

    Shared Sub New()

      Dim escapeChars As New List(Of Char) From {vbCr, vbLf, vbTab, vbFormFeed, vbBack, "\"}

      For Each ch As Char In escapeChars.Union({"'"})
        SingleQuoteCharEscapeFlags(Asc(ch)) = True
      Next

      For Each ch As Char In escapeChars.Union({""""})
        DoubleQuoteCharEscapeFlags(Asc(ch)) = True
      Next

    End Sub

    Private Shared Sub WriteEscapedJavaScriptString(sb As Text.StringBuilder, RawString As String, EscapeFlags As Boolean())


      If RawString IsNot Nothing AndAlso RawString.Length > 0 Then

        Dim HasTab As Boolean
        Dim HasCR As Boolean
        Dim HasLF As Boolean
        Dim HasFF As Boolean
        Dim HasBack As Boolean
        Dim HasSlash As Boolean
        Dim HasSingleQuote As Boolean
        Dim HasDoubleQuote As Boolean

        For i As Integer = 0 To RawString.Length - 1

          Dim c As Char = RawString(i)
          Dim ci As UInt32 = Convert.ToUInt32(c)

          If (ci < 128 AndAlso EscapeFlags(ci)) Then

            Select Case c
              Case vbTab
                HasTab = True

              Case vbLf
                HasLF = True

              Case vbCr
                HasCR = True

              Case vbFormFeed
                HasFF = True

              Case vbBack
                HasBack = True

              Case "\"
                HasSlash = True

              Case """"
                HasDoubleQuote = True

              Case "'"
                HasSingleQuote = True

            End Select

          End If

        Next

        If HasTab Then
          RawString = RawString.Replace(vbTab, "\t")
        End If
        If HasSlash Then
          RawString = RawString.Replace("\", "\\")
        End If
        If HasCR Then
          RawString = RawString.Replace(vbCr, "\r")
        End If
        If HasLF Then
          RawString = RawString.Replace(vbLf, "\n")
        End If
        If HasFF Then
          RawString = RawString.Replace(vbFormFeed, "\f")
        End If
        If HasDoubleQuote Then
          RawString = RawString.Replace("""", "\""")
        End If
        If HasSingleQuote Then
          RawString = RawString.Replace("'", "\'")
        End If
        If HasBack Then
          RawString = RawString.Replace(vbBack, "\b")
        End If
        sb.Append(RawString)
      End If

    End Sub


  End Class

  Public Class JSonReader

    Private mJSonString As String

    Public Sub New(JSonString As String)
      mJSonString = JSonString
    End Sub

    Public Function GetRawPropertyValue(PropertyName As String) As String
      'Quick and dirty...
      Dim StartPos As Integer = mJSonString.IndexOf("""" & PropertyName & """")
      If StartPos >= 0 Then
        Dim SearchText As String = mJSonString.Substring(StartPos + PropertyName.Length + 3).Trim
        Dim StartChar As String
        Dim EndChar As String
        Dim Level As Integer = 0
        Dim InString As Boolean = False
        If SearchText.StartsWith(":") Then
          SearchText = SearchText.Substring(1).Trim
        End If

        StartChar = SearchText(0)
        EndChar = If(StartChar = """", """", If(StartChar = "{", "}", If(StartChar = "[", "]", "")))

        For i As Integer = 1 To SearchText.Length - 1
          Dim Ch As Char = SearchText(i)

          If Ch = "\" Then
            i += 1
          Else
            If Ch = """" AndAlso Ch <> StartChar Then
              InString = Not InString
            End If
            If Not InString Then
              If Ch = StartChar Then
                Level += 1
              End If
              If Ch = EndChar Then
                If Level = 0 Then
                  Return SearchText.Substring(0, i + 1)
                End If
                Level -= 1
              End If
            End If
          End If
        Next

      End If

      Return ""

    End Function

  End Class

End Namespace
