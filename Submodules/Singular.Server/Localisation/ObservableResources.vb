Imports System.ComponentModel

Namespace Localisation

  Public Class ObservableResources(Of T)
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Private Shared MyResources As T

    Public ReadOnly Property LocalisationResources() As T
      Get
        Return MyResources
      End Get
    End Property

    Public Sub New(resources As T)

      MyResources = resources

      ObservableResourcesList.Add(Me)

    End Sub

    Private Shared ObservableResourcesList As New List(Of ObservableResources(Of T))

      Public Shared Sub UpdateAllBindings()

      For Each ors In ObservableResourcesList
        ors.UpdateBindings()
      Next

    End Sub

    Public Overridable Sub UpdateBindings()

      RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("LocalisationResources"))

    End Sub

  End Class

  Public Class SingularObservableResources
    Inherits ObservableResources(Of My.Resources.localstring)

    Private Shared DefaultSingularObservableResources As New SingularObservableResources

    Public Shared ReadOnly Property DefaultResources As SingularObservableResources
      Get
        Return DefaultSingularObservableResources
      End Get
    End Property

    Public Sub New()

      MyBase.New(New My.Resources.localstring)

    End Sub

  End Class

End Namespace