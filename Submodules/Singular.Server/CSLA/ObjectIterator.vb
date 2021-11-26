Imports System.Reflection

Namespace CSLALib

  Public Class ObjectIterator

    Private _PerformOnListObjects As Boolean = False
    Private _CheckBackingFieldFirst As Boolean = False
    Private _IgnoreNoBrokenRules As Boolean = True
    Private _Action As AbortableAction

    Public Property Context As AbortableActionContext

    ''' <summary>
    ''' True if you want to skip certain types. The child object type is passed.
    ''' </summary>
    Public Property CanIterate As Func(Of Singular.ReflectionCached.TypeInfo, Boolean)

    Public Sub New(ByVal PerformOnListObjects As Boolean,
                          CheckBackingFieldFirst As Boolean,
                          IgnoreNoBrokenRules As Boolean,
                          Action As AbortableAction)

      _PerformOnListObjects = PerformOnListObjects
      _CheckBackingFieldFirst = CheckBackingFieldFirst
      _IgnoreNoBrokenRules = IgnoreNoBrokenRules
      _Action = Action

      Context = New Singular.AbortableActionContext

    End Sub

    Public Sub RecurseObjectGraphAndPerformAction(List As ISingularListBase)

      If _PerformOnListObjects Then
        _Action.Invoke(List, Context)
        If Context.Abort Then
          Exit Sub
        End If
      End If

      Dim TypeInfo = Singular.ReflectionCached.GetCachedType(Singular.ReflectionCached.GetCachedType(List.GetType).LastGenericType)

      If CanIterate Is Nothing OrElse CanIterate(TypeInfo) Then

        For Each Child As ISingularBase In List
          RecurseObjectGraphAndPerformAction(Child, TypeInfo)
          If Context.Abort Then
            Exit For
          End If
        Next

      End If

    End Sub

    Public Sub RecurseObjectGraphAndPerformAction(Obj As ISingularBase)
      RecurseObjectGraphAndPerformAction(Obj, Singular.ReflectionCached.GetCachedType(Obj.GetType))
    End Sub

    Public Sub RecurseObjectGraphAndPerformAction(Obj As ISingularBase,
                                                  TypeInfo As Singular.ReflectionCached.TypeInfo)

      _Action.Invoke(Obj, Context)

      If Context.Abort Then
        Exit Sub
      End If

      'Child Lists
      For Each ChildListProperty In TypeInfo.ChildLists

        If _IgnoreNoBrokenRules OrElse Not Attribute.IsDefined(ChildListProperty.MemberInfo, GetType(Singular.DataAnnotations.NoBrokenRules)) Then

          If Not _CheckBackingFieldFirst OrElse ChildListProperty.BackingField Is Nothing OrElse Obj.GetBackingFieldValue(ChildListProperty.BackingField) IsNot Nothing Then

            Dim List As ISingularListBase = ChildListProperty.GetValueFast(Obj)
            If List IsNot Nothing Then

              RecurseObjectGraphAndPerformAction(List)

              If Context.Abort Then
                Exit Sub
              End If

            End If
          End If
        End If
      Next

      'Single objects on this object
      For Each ChildObjectProperty In TypeInfo.ChildObjects

        If Not _CheckBackingFieldFirst OrElse ChildObjectProperty.BackingField Is Nothing OrElse Obj.GetBackingFieldValue(ChildObjectProperty.BackingField) IsNot Nothing Then

          Dim ChildObject As ISingularBase = ChildObjectProperty.GetValueFast(Obj)
          If ChildObject IsNot Nothing Then

            Dim ChildType = Singular.ReflectionCached.GetCachedType(DirectCast(ChildObjectProperty.MemberInfo, PropertyInfo).PropertyType)

            If CanIterate Is Nothing OrElse CanIterate(ChildType) Then
              RecurseObjectGraphAndPerformAction(ChildObject, ChildType)
            End If

            If Context.Abort Then
              Exit Sub
            End If

          End If
        End If
      Next

      '' now loop through each of the children
      'For Each pi In Obj.GetType.GetProperties(System.Reflection.BindingFlags.Instance + System.Reflection.BindingFlags.Public)

      '  If Not Attribute.IsDefined(pi, GetType(Singular.DataAnnotations.NoBrokenRules)) Then
      '    If GetType(ISingularListBase).IsAssignableFrom(pi.PropertyType) Then
      '      ' LIST CHILD

      '      Dim piChild = Obj.FieldManager.GetRegisteredProperties.FirstOrDefault(Function(c)
      '                                                                              Dim ip = TryCast(c, Csla.Core.IPropertyInfo)
      '                                                                              If ip IsNot Nothing AndAlso ip.Name = pi.Name Then
      '                                                                                Return True
      '                                                                              Else
      '                                                                                Return False
      '                                                                              End If
      '                                                                            End Function)
      '      If piChild Is Nothing OrElse Me.GetProperty(piChild) IsNot Nothing Then
      '        Dim list As ISingularListBase = pi.GetValue(Obj, Nothing)

      '        If list IsNot Nothing Then

      '          list.RecurseObjectGraphAndPerformAction(Action, Context, PerformOnListObjects)

      '          If Context.Abort Then
      '            Exit Sub
      '          End If

      '        End If
      '      End If

      '    ElseIf GetType(ISingularBase).IsAssignableFrom(pi.PropertyType) Then
      '      ' SINGLE OBJECT CHILD

      '      Dim piChild = Obj.FieldManager.GetRegisteredProperties.FirstOrDefault(Function(c)
      '                                                                              Dim ip = TryCast(c, IPropertyInfo)
      '                                                                              If ip IsNot Nothing AndAlso ip.Name = pi.Name Then
      '                                                                                Return True
      '                                                                              Else
      '                                                                                Return False
      '                                                                              End If
      '                                                                            End Function)
      '      If piChild Is Nothing OrElse Obj.GetProperty(piChild) IsNot Nothing Then
      '        Dim child As ISingularBase = pi.GetValue(Obj, Nothing)

      '        If child IsNot Nothing Then

      '          child.RecurseObjectGraphAndPerformAction(Action, Context, PerformOnListObjects)

      '          If Context.Abort Then
      '            Exit Sub
      '          End If

      '        End If
      '      End If

      '    End If
      '  End If
      'Next

    End Sub

  End Class

End Namespace

