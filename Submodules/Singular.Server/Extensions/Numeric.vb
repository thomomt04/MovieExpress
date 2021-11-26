Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module Numeric

    <Extension()>
    Public Function IsPrime(ByVal number As Integer) As Boolean
      If (number Mod 2) = 0 Then
        Return number = 2
      End If
      Dim sqrt As Integer = CInt(System.Math.Sqrt(number))
      Dim t As Integer = 3
      While t <= sqrt
        If number Mod t = 0 Then
          Return False
        End If
        t = t + 2
      End While
      Return number <> 1
    End Function

    <Extension()>
    Public Function IsPowerOf2(ByVal Number As Integer) As Boolean

      If Number <= 0 Then
        Return False
      End If

      Dim power As Integer = 0
      While System.Math.Pow(2, power) <= Number
        If System.Math.Pow(2, power) = Number Then
          Return True
        Else
          power += 1
        End If
      End While
      Return False

    End Function

    'These are for javascript rules where you need to ignore floating point precision errors.
    <Extension()>
    Public Function CompareGT(x As Decimal, y As Decimal) As Boolean
      Return x > y
    End Function

    <Extension()>
    Public Function CompareGTE(x As Decimal, y As Decimal) As Boolean
      Return x >= y
    End Function

    <Extension()>
    Public Function CompareLT(x As Decimal, y As Decimal) As Boolean
      Return x < y
    End Function

    <Extension()>
    Public Function CompareLTE(x As Decimal, y As Decimal) As Boolean
      Return x <= y
    End Function

    <Extension()>
    Public Function CompareE(x As Decimal, y As Decimal) As Boolean
      Return x = y
    End Function

    <Extension()>
    Public Function CompareNE(x As Decimal, y As Decimal) As Boolean
      Return x <> y
    End Function

  End Module

  Public Module Csla

    ''' <summary>
    ''' Will convert the collection into a BindingList collection
    ''' </summary>
    ''' <typeparam name="C"></typeparam>
    ''' <param name="list"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
#If SILVERLIGHT Then

#Else
    <System.Runtime.CompilerServices.Extension()> _
    Public Function ToBindingList(Of C)(list As IEnumerable(Of C)) As BindingList(Of C)

      Dim returnList As New BindingList(Of C)
      For Each item In list
        returnList.Add(item)
      Next
      Return returnList

    End Function
#End If


  End Module

End Namespace
