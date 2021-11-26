Public Class SingularCriteriaBase(Of C As SingularCriteriaBase(Of C))
  Inherits Csla.CriteriaBase(Of C)

  ''' <summary>
  ''' Forces the type to be initialised, and have all of its csla property infos created.
  ''' </summary>
  Protected Shared Function InitialisationDummy() As Boolean
    Return True
  End Function

  Shared Sub New()
    Csla.Core.FieldManager.FieldDataManager.ForceStaticFieldInit(GetType(C))
  End Sub

  Public Shared Function RegisterSProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)), DefaultValue As PropertyType) As SPropertyInfo(Of PropertyType, C)
    Dim spi = Singular.RegisterSProperty(Of PropertyType, C)(TargetMember, DefaultValue)
    Return RegisterProperty(spi)
  End Function

  Public Shared Function RegisterSProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object))) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember, Nothing)
  End Function

  Public Shared Function RegisterReadOnlyProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                                                                  GetExpression As System.Linq.Expressions.Expression(Of System.Func(Of C, Object))) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember).GetExpression(GetExpression)
  End Function

  Public Shared Function RegisterReadOnlyProperty(Of PropertyType)(ByVal TargetMember As System.Linq.Expressions.Expression(Of System.Func(Of C, Object)),
                                                                 JSGetExpression As String) As SPropertyInfo(Of PropertyType, C)
    Return RegisterSProperty(Of PropertyType)(TargetMember).GetExpression(JSGetExpression)
  End Function

  Protected Overloads Function ReadProperty(Of PropertyType)(SProperty As Csla.PropertyInfo(Of PropertyType)) As PropertyType
    If TypeOf SProperty Is ISingularPropertyInfo AndAlso CType(SProperty, SPropertyInfo(Of PropertyType, C)).ParsedGetExpression IsNot Nothing Then
      Return CType(SProperty, SPropertyInfo(Of PropertyType, C)).ParsedGetExpression.GetValue(Me)
    Else
      Return MyBase.ReadProperty(SProperty)
    End If
  End Function

  Protected Overloads Function LoadProperty(Of PropertyType)(SProperty As Csla.PropertyInfo(Of PropertyType), Value As PropertyType) As PropertyType

    MyBase.LoadProperty(SProperty, Value)

    If TypeOf SProperty Is ISingularPropertyInfo Then
      'Check if this property has AutoSet properties.
      For Each se In CType(SProperty, SPropertyInfo(Of PropertyType, C)).SetExpressionList
        If se.SetExpressionParsed IsNot Nothing Then
          se.SetProperty(Of C)(Me)
        End If
      Next
    End If
  End Function

End Class
