Public Interface ISingularBusinessListBase
  Inherits ISingularListBase
  Inherits IParentSavable

  Sub ClearNewItems()
  Function GetDeletedList() As Object

  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
  ReadOnly Property IsDirty As Boolean

#If Silverlight = False Then

  Sub UpdateGeneric()

#End If


End Interface

Public Interface ISingularBusinessListBaseGeneric(Of L As ISingularBusinessListBaseGeneric(Of L, C),
                                                    C As ISingularBusinessBase)
  Inherits ISingularListBase

End Interface
