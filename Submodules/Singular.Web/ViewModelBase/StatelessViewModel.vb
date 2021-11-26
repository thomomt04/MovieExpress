Public Interface IStatelessViewModel

End Interface


Public Class StatelessViewModel(Of ModelType As StatelessViewModel(Of ModelType))
  Inherits ViewModel(Of ModelType)
  Implements IStatelessViewModel

  Public Sub New()
    'DeserialiseMode = Singular.Web.DeserialiseMode.Stateless
  End Sub

End Class
