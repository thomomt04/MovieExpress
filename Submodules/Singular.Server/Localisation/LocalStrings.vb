Public Module LocalStrings

#Region " Keys "

  Public Grid_AddNew As String = "Add"
  Public Grid_AddNew_Tooltip As String = "Add a new item to the list"
  Public Grid_Delete_Tooltip As String = "Delete this record."


#End Region

  ''' <summary>
  ''' When true, the string will act as a key for localisation.
  ''' </summary>
  Public Property EnableLookup As Boolean = False

  Public Function GetText(LibKey As String) As String
    If EnableLookup Then
      Return Localisation.LocalText(LibKey)
    Else
      Return LibKey
    End If
  End Function


End Module
