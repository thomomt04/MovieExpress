''' <summary>
''' Provides methods for manipulating files
''' </summary>
''' <remarks></remarks>
Public Class Files

	''' <summary>
	''' Gets the readable size of a file form the no of bytes. E.g. 1048576 = 1MB, 4567 = 4.45KB
	''' </summary>
	''' <param name="SizeInBytes">Size of the file in bytes value</param>
	''' <returns>Returns a string value with a friendly file size description</returns>
	''' <remarks></remarks>
	Public Shared Function GetReadableSize(SizeInBytes As Int64) As String

		If SizeInBytes < 1024 Then
			Return SizeInBytes & "B"
		ElseIf SizeInBytes < 1048576 Then
			Return (SizeInBytes / 1024).ToString("#.##") & "KB"
		ElseIf SizeInBytes < 1048576 * 1024 Then
			Return (SizeInBytes / 1048576).ToString("#.##") & "MB"
		Else
			Return (SizeInBytes / 1073741824).ToString("#.##") & "GB"
		End If

	End Function

End Class
