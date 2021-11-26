Namespace Extensions

  Public Module Strings

    <System.Runtime.CompilerServices.Extension()> _
    Public Function Readable(value As String) As String

      Return Singular.Strings.Readable(value)

    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function IsNumeric(ByVal value As String) As Boolean

      Dim Result As Object = Nothing
      Return Integer.TryParse(value, Result) OrElse Decimal.TryParse(value, Result)

    End Function

    ''' <summary>
    ''' Adds single quotes to the start and end of a string.
    ''' </summary>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function AddSingleQuotes(ByVal value As String) As String

      Return "'" & value & "'"

    End Function

    ''' <summary>
    ''' If the string is empty, returns the replacement value.
    ''' </summary>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function IfEmpty(ByVal value As String, ReplaceValue As String) As String

      Return If(value Is Nothing OrElse value = "", ReplaceValue, value)

    End Function

    Public Enum NewLineHandlingType
      Break = 1
      PointList = 2
      NumberedList = 3
    End Enum

		''' <summary>
		''' Tries to convert plain text to HTML by replacing line feeds with br /> etc.
		''' </summary>
		<System.Runtime.CompilerServices.Extension()>
		Public Function ToHTML(Value As String, Optional NewLineHandling As NewLineHandlingType = NewLineHandlingType.Break) As String

			If NewLineHandling = NewLineHandlingType.Break Then
				Return Value.Replace(vbCrLf, "<br />")
			Else

				Dim NewString As String = ""

				For Each line As String In Value.Replace(vbTab, "").Split(vbCrLf)
					Singular.Strings.Delimit(NewString, line, "</li><li>")
				Next
				If NewLineHandling = NewLineHandlingType.PointList Then
					NewString = "<ul><li>" & NewString & "</li></ul>"
				ElseIf NewLineHandling = NewLineHandlingType.NumberedList Then
					NewString = "<ol><li>" & NewString & "</li></ol>"
				End If

				Return NewString

			End If

		End Function

		''' <summary>
		''' Checks if 'self' is contained in the values array
		''' </summary>
		<System.Runtime.CompilerServices.Extension()>
		Public Function InArray(Of T)(self As T, ParamArray values() As T) As Boolean

			Return values.Contains(self)

		End Function

		''' <summary>
		''' Removes the beginning of the 'Source' string if it starts with the 'StartsWith' string
		''' </summary>
		<System.Runtime.CompilerServices.Extension()>
		Public Function RemoveStartsWith(Source As String, StartsWith As String) As String

			If String.IsNullOrEmpty(Source) Then Return Source

			If Not Source.StartsWith(StartsWith) Then Return Source

			Return Source.Substring(StartsWith.Length)

		End Function

	End Module

End Namespace
