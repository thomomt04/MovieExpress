Namespace Data

  <Serializable()>
  Public Class ServerObjectTracker

    <Serializable()>
    Public Class ServerObjectInfo
      Private mListItem As Object
      Private mParentList As Object
      Private mParentProperty As System.Reflection.PropertyInfo

      Public Sub New(ListItem As Object, Parent As Object)

        mListItem = ListItem
        ParentList = Parent

      End Sub

      Public Property ParentList As Object
        Get
          Return mParentList
        End Get
        Set(value As Object)
          If value IsNot Nothing Then
            Dim pi = Singular.Reflection.GetProperty(mListItem.GetType, "Parent")
            If pi IsNot Nothing AndAlso pi.GetValue(mListItem, Nothing) Is value Then
              mParentProperty = pi
            Else
              mParentList = value
            End If
          End If
        End Set
      End Property

      Public Property ListItem As Object
        Get
          Return mListItem
        End Get
        Set(value As Object)
          mListItem = value
        End Set
      End Property

      Public Sub RemoveSelf()
        Dim tmpParent As IList = mParentList
        If tmpParent Is Nothing Then
          tmpParent = mParentProperty.GetValue(ListItem, Nothing)
        End If
        tmpParent.Remove(mListItem)
      End Sub

    End Class

    Private mHashTableObjects As Dictionary(Of Object, Guid)
    Private mHashTableGuids As Dictionary(Of Guid, ServerObjectInfo)

    Public Sub New()
      Reset()
    End Sub

    Public Function AddObject(Obj As Object, Parent As Object, Optional GuidString As String = "", Optional ByRef Existed As Boolean = False) As Guid
      If Obj IsNot Nothing Then

        'Get the ObjectID.
        'If its a Singular CSLA Object, then use the GUID, else use the HashCode
        Dim ObjectID As Object
        If TypeOf Obj Is ISingularBase Then
          ObjectID = CType(Obj, ISingularBase).Guid
        Else
          ObjectID = Obj.GetHashCode
        End If

        'Add / Update the Object linked to the ObjectID
        If mHashTableObjects.ContainsKey(ObjectID) Then
          Existed = True

          If TypeOf ObjectID Is Guid Then
            Dim soi As ServerObjectInfo = mHashTableGuids(ObjectID)
            If Not soi.ListItem Is Obj Then
              soi.ListItem = Obj
            End If
            If Parent IsNot Nothing AndAlso soi.ParentList Is Nothing Then
              soi.ParentList = Parent
            End If
          End If

          Return mHashTableObjects(ObjectID)
        Else
          Dim Guid As System.Guid
          If TypeOf ObjectID Is Guid Then
            Guid = ObjectID
          Else
            If GuidString = "" Then
              Guid = System.Guid.NewGuid
            Else
              Guid = New System.Guid(GuidString)
            End If
          End If

          mHashTableObjects(ObjectID) = Guid
          mHashTableGuids(Guid) = New ServerObjectInfo(Obj, Parent)
          Return Guid
        End If
      End If
    End Function

    Public Function GetObjectInfo(Guid As Guid) As ServerObjectInfo

      If mHashTableGuids.ContainsKey(Guid) Then
        Return mHashTableGuids(Guid)
      Else
        Return Nothing
      End If

    End Function

    Public Function GetObject(Guid As Object) As Object
      If TypeOf Guid Is String Then
        Guid = New Guid(DirectCast(Guid, String))
      End If
      Dim oi = GetObjectInfo(Guid)
      If oi Is Nothing Then
        Return Nothing
      Else
        Return oi.ListItem
      End If
    End Function

    Public Sub Reset()
      mHashTableObjects = New Dictionary(Of Object, Guid)
      mHashTableGuids = New Dictionary(Of Guid, ServerObjectInfo)
    End Sub

  End Class

End Namespace


