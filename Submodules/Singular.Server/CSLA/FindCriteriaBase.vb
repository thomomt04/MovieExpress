Imports Csla.Serialization
Imports System.ComponentModel.DataAnnotations

<Serializable()> _
Public MustInherit Class FindCriteriaBase(Of F As FindCriteriaBase(Of F))
  Inherits Csla.CriteriaBase(Of F)

  <Display(AutoGenerateField:=False)>
  Public MustOverride Property FindText As String

End Class
