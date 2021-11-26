Imports Csla


<Serializable()>
Public Class ChildObjectContainer
  Inherits SingularBusinessBase(Of ChildObjectContainer)

  Public Shared ChildObjectProperty As PropertyInfo(Of Object) = RegisterProperty(Of Object)(Function(c) c.ChildObject)

  Public ReadOnly Property ChildObject As Object
    Get
      Return GetProperty(ChildObjectProperty)
    End Get
  End Property

  Public Shared Function NewChildObjectContainer(ByVal ChildObject As Object) As ChildObjectContainer

    Dim coc As New ChildObjectContainer
    coc.SetProperty(ChildObjectProperty, ChildObject)
    Return coc

  End Function

  Private Sub New()

    MarkOld()

  End Sub

  Protected Overrides Sub DataPortal_Update()

    Dim IsNewProperty = Me.ChildObject.GetType.GetProperty("IsNew")

    If IsNewProperty IsNot Nothing AndAlso CBool(IsNewProperty.GetValue(Me.ChildObject, Nothing)) Then

      Dim InsertMethod = Me.ChildObject.GetType.GetMethod("Insert", System.Reflection.BindingFlags.NonPublic + System.Reflection.BindingFlags.Instance)

      InsertMethod.Invoke(Me.ChildObject, Nothing)
    Else
      Dim UpdateMethod = Me.ChildObject.GetType.GetMethod("Update", System.Reflection.BindingFlags.NonPublic + System.Reflection.BindingFlags.Instance)

      UpdateMethod.Invoke(Me.ChildObject, Nothing)
    End If

  End Sub

End Class
