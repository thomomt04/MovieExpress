#If SILVERLIGHT Then
Imports System.Runtime.InteropServices.Automation
Imports C1.Silverlight.Excel
#End If
Imports System.IO

Public Class FileImporting

#If SILVERLIGHT Then

  Public Class Row

    Public Property Cells As List(Of Cell)

  End Class

  Public Class Cell

    Public Property Value As Object

    Public Sub New(value As Object)
      Me.Value = value
    End Sub

  End Class

  Public Shared Function ImportExcel(fileStream As Stream) As List(Of Row)

    Dim Ret As List(Of Row) = New List(Of Row)
    Dim row As Row

    Dim file As C1XLBook = New C1XLBook
    file.Load(fileStream)

    Dim Sheet As XLSheet = file.Sheets.Item(file.Sheets.FirstIndex)

    For i = 0 To Sheet.Rows.Count - 1
      row = New Row
      For j = 0 To Sheet.Columns.Count - 1
        row.Cells.Add(New Cell(Sheet.GetCell(i, j).Value))
      Next
      Ret.Add(row)
    Next

    Return Ret

  End Function

#Else



  <Serializable()> _
  Public Class FixedLengthField

    Private mField As String
    Private mLength As Integer

    Public ReadOnly Property Field() As String
      Get
        Return mField
      End Get
    End Property

    Public ReadOnly Property Length() As Integer
      Get
        Return mLength
      End Get
    End Property

    Public Shared Function NewFixedLengthField(ByVal Field As String, ByVal Length As Integer) As FixedLengthField

      Return New FixedLengthField(Field, Length)

    End Function

    Private Sub New(ByVal Field As String, ByVal Length As Integer)

      mField = Field
      mLength = Length

    End Sub

  End Class

  <Serializable()> _
  Public Class FixedLengthFieldList
    Inherits List(Of FixedLengthField)

    Public Overloads Sub AddCore(ByVal obj As FixedLengthField)
      Me.Add(obj)
    End Sub

    Public Function GetField(ByVal FromLine As String, ByVal FieldName As String, Optional ByVal AutoTrim As Boolean = True, Optional ByVal AddCarriageReturnIfNotBlank As Boolean = False) As String

      Dim index As Integer = 0
      Dim flf As FixedLengthField = Nothing
      Dim sReturn As String = ""
      For Each flf In Me
        If flf.Field.ToLower = FieldName.ToLower Then
          ' we have our field
          Exit For
        Else
          index += flf.Length
        End If
      Next
      If index >= FromLine.Length Then
        Return ""
      Else
        If index + flf.Length >= FromLine.Length Then
          sReturn = FromLine.Substring(index)
        Else
          sReturn = FromLine.Substring(index, flf.Length)
        End If
      End If

      If AutoTrim Then
        sReturn = Trim(sReturn)
      End If

      If AddCarriageReturnIfNotBlank And sReturn <> "" Then
        sReturn = sReturn & vbCrLf
      End If

      Return sReturn

    End Function

    Public Shared Function NewFixedLengthFieldList() As FixedLengthFieldList

      Return New FixedLengthFieldList

    End Function

    Private Sub New()

    End Sub

  End Class

#End If


End Class