Public Class EmptyViewModel(Of Type As Singular.Web.ViewModel(Of Type))
  Inherits ViewModel(Of Type)

End Class

Public Class EmptyViewModel
  Inherits ViewModel(Of EmptyViewModel)

  Protected Overrides Sub SetCacheability()

  End Sub

End Class
