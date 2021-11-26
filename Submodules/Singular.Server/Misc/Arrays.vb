''' <summary>
''' Provides methods for searching, and manipulating arrays
''' </summary>
''' <remarks></remarks>
Public Class Arrays

''' <summary>
''' Transforming any array object to an Csla.Core.MobileList object
''' </summary>
''' <typeparam name="T">Type of your Array</typeparam>
''' <param name="FromArray">Accepts an array object</param>
''' <returns>Csla.Core.MobileList(Of T)</returns>
''' <remarks></remarks>
	Public Shared Function GetMobileList(Of T)(ByVal FromArray() As T) As Csla.Core.MobileList(Of T)

		Dim ml As New Csla.Core.MobileList(Of T)
		For Each item In FromArray
			ml.Add(item)
		Next
		Return ml

	End Function
''' <summary>
''' Returns a boolean value indicating whether a specified item exist within this array. 
''' </summary>
''' <param name="Array">Accepts an array object</param>
''' <param name="Value">The item to seek</param>
''' <returns>Boolean</returns>
''' <remarks></remarks>
	Public Shared Function ArrayContains(ByVal Array() As Object, ByVal Value As Object) As Boolean

		For i As Integer = 0 To Array.Length - 1
			If Singular.Misc.CompareSafe(Array(i), Value) Then
				Return True
			End If
		Next
		Return False

	End Function
''' <summary>
''' Returns a boolean value indicating whether a specified item exist within this array. 
''' </summary>
''' <typeparam name="T">Type of your Array</typeparam>
''' <param name="Array">Accepts an generic array object</param>
''' <param name="Value">The item to seek</param>
''' <returns>Boolean</returns>
''' <remarks></remarks>
	Public Shared Function ArrayContainsGeneric(Of T)(ByVal Array() As T, ByVal Value As Object) As Boolean

		For i As Integer = 0 To Array.Length - 1
			If Singular.Misc.CompareSafe(Array(i), Value) Then
				Return True
			End If
		Next
		Return False

	End Function

End Class
