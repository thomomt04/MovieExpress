Namespace Localisation.Data

  Public Class ObjectVisitor

#Region " Factory Methods "

    Public Shared Sub FetchData(List As ISingularListBase)

      Dim Instance As New ObjectVisitor(List)
      Instance.PopulateObjects()

    End Sub

    Public Shared Sub FetchData(Obj As ISingularBase)

      Dim Instance As New ObjectVisitor(Obj)
      Instance.PopulateObjects()

    End Sub

    Public Shared Sub SaveData(List As ISingularBusinessListBase)

      Dim Instance As New ObjectVisitor(List)
      Instance.SaveData()

    End Sub

    Public Shared Sub SaveData(Obj As ISingularBusinessBase)

      Dim Instance As New ObjectVisitor(Obj)
      Instance.SaveData()

    End Sub

#End Region

    Private _Iterator As CSLALib.ObjectIterator
    Private _LanguageID As Integer
    Private _VariantID As Integer?

    Private Sub New(List As ISingularListBase)

      Dim TypeInfo = Singular.ReflectionCached.GetCachedType(Singular.Reflection.GetLastGenericType(List.GetType))

      If Setup(TypeInfo) Then
        _Iterator.RecurseObjectGraphAndPerformAction(List)
      End If

    End Sub

    Private Sub New(Obj As ISingularBase)

      Dim TypeInfo = Singular.ReflectionCached.GetCachedType(Obj.GetType)

      If Setup(TypeInfo) Then
        _Iterator.RecurseObjectGraphAndPerformAction(Obj, TypeInfo)
      End If

    End Sub

    Private Function Setup(ChildType As Singular.ReflectionCached.TypeInfo) As Boolean

      If CanPerformDataLocalisation(ChildType) Then

        _LanguageID = Localisation.CurrentLanguageID
        _VariantID = Localisation.CurrentVariant

        _Iterator = New CSLALib.ObjectIterator(False, False, True, AddressOf RecordObject)
        _Iterator.CanIterate = AddressOf TypeSupportsDataLocalisation

        Return True
      End If

      Return False

    End Function

    Private _LastType As ObjectTypeInfo
    Private _ObjectTypes As New Dictionary(Of Type, ObjectTypeInfo)

    Private Sub RecordObject(Obj As Object, Context As Singular.AbortableActionContext)

      Dim ObjectType = Obj.GetType

      Dim LD As LocaliseDataAttribute = Attribute.GetCustomAttribute(ObjectType, GetType(LocaliseDataAttribute))

      If LD IsNot Nothing AndAlso LD.LocalisationLevel <> DataLocalisationLevel.ChildrenOnly Then

        'Get the object type info for this object.
        If _LastType Is Nothing OrElse _LastType.ObjectType <> ObjectType Then
          If Not _ObjectTypes.TryGetValue(ObjectType, _LastType) Then
            _LastType = New ObjectTypeInfo(ObjectType, LD, Obj)
            _ObjectTypes.Add(ObjectType, _LastType)
          End If
        End If

        'Add each object
        _LastType.AddObject(Obj)

      End If

    End Sub

    Public Sub PopulateObjects()

      For Each Type In _ObjectTypes.Values
        Type.FetchData(_LanguageID, _VariantID)
      Next

    End Sub

    Public Sub SaveData()

      For Each Type In _ObjectTypes.Values
        Type.SaveData(_LanguageID, _VariantID)
      Next

    End Sub

  End Class

End Namespace

