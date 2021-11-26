'Imports System.Linq.Expressions
'Imports System.Reflection

'Namespace Extensions

'  Module LinqExtensions

'    <System.Runtime.CompilerServices.Extension()> _
'    Public Function OrderBy(Of T)(source As IQueryable(Of T), [property] As String) As IOrderedQueryable(Of T)
'      Return ApplyOrder(Of T)(source, [property], "OrderBy")
'    End Function

'    <System.Runtime.CompilerServices.Extension()> _
'    Public Function OrderByDescending(Of T)(source As IQueryable(Of T), [property] As String) As IOrderedQueryable(Of T)
'      Return ApplyOrder(Of T)(source, [property], "OrderByDescending")
'    End Function

'    <System.Runtime.CompilerServices.Extension()> _
'    Public Function ThenBy(Of T)(source As IOrderedQueryable(Of T), [property] As String) As IOrderedQueryable(Of T)
'      Return ApplyOrder(Of T)(source, [property], "ThenBy")
'    End Function

'    <System.Runtime.CompilerServices.Extension()> _
'    Public Function ThenByDescending(Of T)(source As IOrderedQueryable(Of T), [property] As String) As IOrderedQueryable(Of T)
'      Return ApplyOrder(Of T)(source, [property], "ThenByDescending")
'    End Function

'    Private Function ApplyOrder(Of T)(source As IQueryable(Of T), [property] As String, methodName As String) As IOrderedQueryable(Of T)
'      Dim props As String() = [property].Split("."c)
'      Dim type As Type = GetType(T)
'      Dim arg As ParameterExpression = Expression.Parameter(type, "x")
'      Dim expr As Expression = arg
'      For Each prop As String In props
'        ' use reflection (not ComponentModel) to mirror LINQ
'        Dim pi As PropertyInfo = type.GetProperty(prop)
'        expr = Expression.[Property](expr, pi)
'        type = pi.PropertyType
'      Next
'      Dim delegateType As Type = GetType(Func(Of ,)).MakeGenericType(GetType(T), type)
'      Dim lambda As LambdaExpression = Expression.Lambda(delegateType, expr, arg)

'      Dim result As Object = GetType(Queryable).GetMethods().[Single](Function(method) method.Name = methodName AndAlso method.IsGenericMethodDefinition AndAlso method.GetGenericArguments().Length = 2 AndAlso method.GetParameters().Length = 2).MakeGenericMethod(GetType(T), type).Invoke(Nothing, New Object() {source, lambda})
'      Return DirectCast(result, IOrderedQueryable(Of T))
'    End Function


'  End Module

'End Namespace