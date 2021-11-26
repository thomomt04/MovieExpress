
Namespace CSLALib

  <Serializable()> _
  Public Class SaveContainer
    Inherits SingularBusinessBase(Of SaveContainer)

    Public Property ChildList As List(Of Object)

    Public Overrides ReadOnly Property IsDirty As Boolean
      Get
        Return ChildList.Any(Function(c) c.IsDirty)
      End Get
    End Property

    Public Overrides ReadOnly Property IsValid As Boolean
      Get
        Return ChildList.All(Function(c) c.IsValid)
      End Get
    End Property

    Protected Overrides Sub InsertUpdate(cm As SqlClient.SqlCommand)

      For Each child In ChildList
        Dim InsertUpdateGenericMethod = child.GetType.GetMethod("InsertUpdateGeneric", System.Reflection.BindingFlags.Instance + System.Reflection.BindingFlags.NonPublic)
        InsertUpdateGenericMethod.Invoke(child, Nothing)
      Next

    End Sub

    Public Sub New(ParamArray ChildObjects() As Object)

      Me.ChildList = ChildObjects.ToList()

    End Sub

  End Class

End Namespace
