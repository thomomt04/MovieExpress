Imports System.Linq.Expressions

Namespace Defaultable

  Public MustInherit Class DefaultableObject(Of t)

    Private mParentObject As t
    Public Sub New(ParentObject As t)
      mParentObject = ParentObject
    End Sub

    ''' <summary>
    ''' Creates a defaultable property. Sets the value to the Root Value if this is the Root Object. Otherwise references the parent objects property.
    ''' </summary>
    ''' <typeparam name="p">Type of Property e.g String</typeparam>
    ''' <param name="le">Function(c) c.PropertyName</param>
    ''' <param name="RootValue">The Root Value of this property when this is the Root Object.</param>
    Protected Function CreateProperty(Of p)(le As System.Linq.Expressions.Expression(Of System.Func(Of t, Object)), RootValue As p) As DefaultableProperty(Of p)

      If mParentObject Is Nothing Then
        Return New DefaultableProperty(Of p)(Nothing) With {.Value = RootValue}
      Else
        Dim memberExpr As MemberExpression = CType(le.Body, UnaryExpression).Operand
        Dim pi As System.Reflection.PropertyInfo = memberExpr.Member
        Return New DefaultableProperty(Of p)(pi.GetValue(mParentObject, Nothing))
      End If

    End Function

  End Class

  Public Class DefaultableProperty(Of T)

    Private mParent As DefaultableProperty(Of T)
    Private mValue As T = Nothing
    Public Property IsDefault As Boolean = True

    Public Sub New(Parent As DefaultableProperty(Of T))
      mParent = Parent
    End Sub

    Public Property Value As T
      Get
        If IsDefault Then
          If mParent Is Nothing Then
            Return Nothing
          Else
            Return mParent.Value
          End If
        Else
          Return mValue
        End If
      End Get
      Set(value As T)
        If value Is Nothing Then
          IsDefault = True
          mValue = Nothing
        Else
          IsDefault = False
          mValue = value
        End If
      End Set
    End Property

    'Set
    Public Shared Widening Operator CType(ByVal value As T) As DefaultableProperty(Of T)
      Return New DefaultableProperty(Of T)(Nothing) With {.Value = value}
    End Operator

    'Get
    Public Shared Narrowing Operator CType(ByVal value As DefaultableProperty(Of T)) As T
      Return value.Value
    End Operator

  End Class

End Namespace
