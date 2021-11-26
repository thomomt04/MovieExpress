
Imports System.Collections
Imports System.ComponentModel


Public Class PropertySorter
  Inherits ExpandableObjectConverter

#Region "Methods"
  Public Overrides Function GetPropertiesSupported(ByVal context As ITypeDescriptorContext) As Boolean
    Return True
  End Function

  Public Overrides Function GetProperties(ByVal context As ITypeDescriptorContext, ByVal value As Object, ByVal attributes As Attribute()) As PropertyDescriptorCollection
    '
    ' This override returns a list of properties in order
    '
    Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(value, attributes)
    Dim orderedProperties As New ArrayList()
    For Each pd As PropertyDescriptor In pdc
      Dim attribute As Attribute = pd.Attributes(GetType(PropertyOrderAttribute))
      If attribute IsNot Nothing Then
        '
        ' If the attribute is found, then create an pair object to hold it
        '
        Dim poa As PropertyOrderAttribute = DirectCast(attribute, PropertyOrderAttribute)
        orderedProperties.Add(New PropertyOrderPair(pd.Name, poa.Order))
      Else
        '
        ' If no order attribute is specifed then given it an order of 0
        '
        orderedProperties.Add(New PropertyOrderPair(pd.Name, 0))
      End If
    Next
    '
    ' Perform the actual order using the value PropertyOrderPair classes
    ' implementation of IComparable to sort
    '
    orderedProperties.Sort()
    '
    ' Build a string list of the ordered names
    '
    Dim propertyNames As New ArrayList()
    For Each pop As PropertyOrderPair In orderedProperties
      propertyNames.Add(pop.Name)
    Next
    '
    ' Pass in the ordered list for the PropertyDescriptorCollection to sort by
    '
    Return pdc.Sort(DirectCast(propertyNames.ToArray(GetType(String)), String()))
  End Function
#End Region
End Class

#Region "Helper Class - PropertyOrderAttribute"

<AttributeUsage(AttributeTargets.[Property])> _
Public Class PropertyOrderAttribute
  Inherits Attribute
  '
  ' Simple attribute to allow the order of a property to be specified
  '
  Private _order As Integer
  Public Sub New(ByVal order As Integer)
    _order = order
  End Sub

  Public ReadOnly Property Order() As Integer
    Get
      Return _order
    End Get
  End Property
End Class
#End Region

#Region "Helper Class - PropertyOrderPair"

Public Class PropertyOrderPair
  Implements IComparable
  Private _order As Integer
  Private _name As String
  Public ReadOnly Property Name() As String
    Get
      Return _name
    End Get
  End Property

  Public Sub New(ByVal name As String, ByVal order As Integer)
    _order = order
    _name = name
  End Sub

  Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
    '
    ' Sort the pair objects by ordering by order value
    ' Equal values get the same rank
    '
    Dim otherOrder As Integer = DirectCast(obj, PropertyOrderPair)._order
    If otherOrder = _order Then
      '
      ' If order not specified, sort by name
      '
      Dim otherName As String = DirectCast(obj, PropertyOrderPair)._name
      Return String.Compare(_name, otherName)
    ElseIf otherOrder > _order Then
      Return -1
    End If
    Return 1
  End Function
End Class

#End Region