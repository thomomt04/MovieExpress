Imports System.Text

Namespace Web.Utilities

  Public Class JavaScriptWriter

    Private mLevel As Integer = 0
    Private mSB As StringBuilder

    Public Sub New()
      mSB = New StringBuilder
      mIndentStrings(0) = ""
    End Sub

    Public Function IsEmpty() As Boolean
      Return mSB.Length = 0
    End Function

    Public Sub WriteStartClass(Name As String, ParamArray Parameters As String())
      Write("var " & Name & " = function (", False)

      Dim First As Boolean = True
      If Parameters IsNot Nothing Then
        For Each param As String In Parameters
          If First Then
            First = False
          Else
            mSB.Append(",")
          End If
          mSB.Append(param)
        Next
      End If

      Write(") {")
      AddLevel()
      'Write("var self = this;")
      'mSB.AppendLine()
    End Sub

    Public Sub WriteEndClass(Optional ExtraLine As Boolean = True)
      RemoveLevel()
      Write("};")
      If ExtraLine Then
        mSB.AppendLine()
      End If
    End Sub

    Public Sub WriteStartFunction(Name As String, ParamArray Parameters As String())
      Write("self." & Name & " = function (", False)

      Dim First As Boolean = True
      If Parameters IsNot Nothing Then
        For Each param As String In Parameters
          If First Then
            First = False
          Else
            mSB.Append(",")
          End If
          mSB.Append(param)
        Next
      End If

      mSB.AppendLine(") {")
      AddLevel()
    End Sub

    Public Sub WriteEndFunction()
      WriteEndClass()
    End Sub

    Public Sub AddLevel()
      mLevel += 1
    End Sub

    Public Sub RemoveLevel()
      mLevel -= 1
    End Sub

    Private Shared mIndentStrings As New Hashtable
    Public Sub Write(Text As String, Optional NewLine As Boolean = True)

      If mIndentStrings(mLevel) Is Nothing Then
        mIndentStrings(mLevel) = Singular.Strings.Repeat(" ", mLevel * 2)
      End If

      If NewLine Then
        mSB.AppendLine(mIndentStrings(mLevel) & Text)
      Else
        mSB.Append(mIndentStrings(mLevel) & Text)
      End If
    End Sub

    Public Sub WriteBlock(ByVal Text As String)

      For Each line As String In Text.Split(vbCrLf)
        Write(line.Replace(vbLf, ""))
      Next

    End Sub

    Public Sub RawWrite(Text As String)
      mSB.Append(Text)
    End Sub

    Public Sub WriteJSEncoded(Text As String)
      Singular.Web.Data.JSonWriter.WriteJSonValue(Text, mSB, "'")
    End Sub

    Public Sub RawWriteLine(Text As String)
      mSB.AppendLine(Text)
    End Sub

    Public Sub Clear()
      mSB.Clear()
    End Sub

    Public Overrides Function ToString() As String
      Return mSB.ToString
    End Function

    Public Shared Function EscapeJSONCharacters(NormalString As String) As String

      NormalString = NormalString.Replace(vbTab, "\t")
      NormalString = NormalString.Replace("\", "\\")
      NormalString = NormalString.Replace(vbCr, "\r")
      NormalString = NormalString.Replace(vbLf, "\n")
      NormalString = NormalString.Replace(vbFormFeed, "\f")
      NormalString = NormalString.Replace("""", "\""")
      NormalString = NormalString.Replace("'", "\'")
      NormalString = NormalString.Replace(vbBack, "\b")
      Return NormalString

    End Function

  End Class

End Namespace


