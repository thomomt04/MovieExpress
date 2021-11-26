Imports Singular.Extensions

Public Class ObjectErrors

  Public Shared ReadOnly Property GetListIsValid(ByVal ListObject As Object) As Boolean
    Get
      If ListObject Is Nothing Then
        Return True
      End If
      Dim pi = ListObject.GetType.GetProperty("IsValid")
      If pi IsNot Nothing Then
        Return pi.GetValue(ListObject, Nothing)
      Else
        Return True
      End If
    End Get
  End Property

  Public Shared Function GetErrorsAsString(ByVal Node As BusinessObjectNodeBase, ByVal IndentLevel As Integer) As String

    Dim Errors As String = Node.Description & vbCrLf
    IndentLevel += 1

    Dim tabs As String = ""
    For t As Integer = 0 To IndentLevel - 1 Step +1
      tabs = tabs + vbTab
    Next

    If Node.Children IsNot Nothing Then
      For Each ChildNode As ObjectErrors.BusinessObjectNodeBase In Node.Children
        Errors &= tabs + GetErrorsAsString(ChildNode, IndentLevel)
      Next
    End If
    IndentLevel -= 1
    Return Errors

  End Function


  Public Shared Function GetErrorsAsHTMLString(ByVal Node As BusinessObjectNodeBase, ByVal IndentLevel As Integer) As String
    Dim Errors As String = ""

    If IndentLevel = 0 Then
      Errors = "<b>" & Node.Description & "</b>" & vbCrLf
    Else
      Errors = "<span class='liImg ImgValidation'>" & Node.Description & "</span>" & vbCrLf
    End If

    IndentLevel += 1

    If Node.Children IsNot Nothing Then
      Errors &= "<ol>" & vbCrLf
      For Each ChildNode As ObjectErrors.BusinessObjectNodeBase In Node.Children
        Errors &= "<li class='liVal'>" & vbCrLf & GetErrorsAsHTMLString(ChildNode, IndentLevel) & "</li>" & vbCrLf
      Next
      Errors &= "</ol>" & vbCrLf
    End If
    IndentLevel -= 1
    Return Errors

  End Function

  'Public Class BusinessObjectNodeDataSource
  '  Inherits BaseViewModel

  '  Private mBusinessObjectNode As BusinessObjectNodeList

  '  Public Property BusinessObjectNodes() As BusinessObjectNodeList
  '    Get
  '      Return mBusinessObjectNode
  '    End Get
  '    Set(ByVal value As BusinessObjectNodeList)
  '      If mBusinessObjectNode Is Nothing OrElse Not mBusinessObjectNode.Equals(value) Then
  '        mBusinessObjectNode = value
  '        Me.OnPropertyChanged("BusinessObjectNodes")
  '      End If
  '    End Set
  '  End Property
  'End Class

  Public Class BusinessObjectNodeList(Of T As SingularBusinessListBase(Of T, C), C As SingularBusinessBase(Of C))
    Inherits BusinessObjectNodeListBase

    Public Sub New(ByVal List As T)

      For Each child As C In List
        If Not child.IsValid Then
          Me.Add(New BusinessObjectNode(child))
        End If
      Next

    End Sub

  End Class

  Public Class BusinessObjectRuleList
    Inherits BusinessObjectNodeListBase

    Public Sub New(ByVal FromBusinessObject As Csla.Core.BusinessBase)

      For Each br As Csla.Rules.BrokenRule In FromBusinessObject.BrokenRulesCollection
        If Not Me.ContainsDescription(br.Description) Then
          Me.Add(New BusinessObjectRuleNode(FromBusinessObject, br))
        End If
      Next

    End Sub

  End Class

  Public Class BusinessObjectNodeListBase
    Inherits List(Of BusinessObjectNodeBase)

    Public Function ContainsDescription(ByVal Description As String) As Boolean

      For Each child In Me
        If child.Description = Description Then
          Return True
        End If
      Next
      Return False

    End Function

  End Class

  Public Class BusinessObjectChildNodeList
    Inherits BusinessObjectNodeBase

    Public Overrides ReadOnly Property ImageUri As Uri
      Get
        Return New Uri("/Singular.Silverlight;component/Images/SingularLogoSmall.png", UriKind.Relative)
      End Get
    End Property

    Public Sub New(ByVal ChildListName As String, ByVal ChildList As System.Collections.IEnumerable)

      Me.Description = ChildListName

      mChildren = New BusinessObjectNodeListBase
      For Each child As Csla.Core.BusinessBase In ChildList
        If Not child.IsValid Then
          mChildren.Add(New BusinessObjectNode(child))
        End If
      Next

    End Sub

  End Class

  Public Class BusinessObjectNodeBase
    Inherits Singular.SingularReadOnlyBase(Of BusinessObjectNodeBase)

    Public Shared IsExpandedProperty As Csla.PropertyInfo(Of Boolean) = RegisterProperty(Function(c) c.IsExpanded, "Expanded", True)

    Public Property IsExpanded() As Boolean
      Get
        Return GetProperty(IsExpandedProperty)
      End Get
      Set(ByVal value As Boolean)
        LoadProperty(IsExpandedProperty, value)
      End Set
    End Property

    Public Overridable ReadOnly Property ImageUri As Uri
      Get
        Return New Uri("/Singular.Silverlight;component/Images/SingularLogoSmall.png", UriKind.Relative)
      End Get
    End Property

    Protected mDescription As String

    Public Property Description() As String
      Get
        Return mDescription
      End Get
      Set(ByVal value As String)
        If mDescription <> value Then
          mDescription = value
          '  Me.OnPropertyChanged("ObjectName")
        End If
      End Set
    End Property

    Protected mChildren As List(Of BusinessObjectNodeBase)

    Public Property Children() As List(Of BusinessObjectNodeBase)
      Get
        Return mChildren
      End Get
      Set(ByVal value As List(Of BusinessObjectNodeBase))
        If mChildren Is Nothing OrElse Not mChildren.Equals(value) Then
          mChildren = value
          '   Me.OnPropertyChanged("Children")
        End If
      End Set
    End Property

  End Class


  Public Class BusinessObjectNode
    Inherits BusinessObjectNodeBase

    Private mBusinessObject As Csla.Core.BusinessBase

    Public ReadOnly Property BusinessObject As Csla.Core.BusinessBase
      Get
        Return mBusinessObject
      End Get
    End Property



    Public Sub New(ByVal BusinessObject As Csla.Core.BusinessBase)

      mBusinessObject = BusinessObject
      Me.Description = mBusinessObject.ToString

      mChildren = New BusinessObjectRuleList(mBusinessObject)

      ' now loop through each of the children
      For Each pi In mBusinessObject.GetType.GetProperties(System.Reflection.BindingFlags.Instance + System.Reflection.BindingFlags.Public)
        If Not Attribute.IsDefined(pi, GetType(Singular.DataAnnotations.NoBrokenRules)) Then
          If Singular.Reflection.IsDerivedFromGenericType(pi.PropertyType, GetType(SingularBusinessListBase(Of ,))) Then
            ' LIST CHILD
            Dim list As Object = pi.GetValue(mBusinessObject, Nothing)
            If Not GetListIsValid(list) Then
              For Each child As Csla.Core.BusinessBase In list
                If Not child.IsValid Then
                  mChildren.Add(New BusinessObjectNode(child))
                End If
              Next
              'mChildren.Add(New BusinessObjectChildNodeList(Singular.CSLALib.GetDisplayNameFromProperty(pi), list))
            End If

          ElseIf Singular.Reflection.IsDerivedFromGenericType(pi.PropertyType, GetType(SingularBusinessBase(Of ))) Then
            ' SINGLE OBJECT CHILD
            Dim child As Csla.Core.BusinessBase = pi.GetValue(mBusinessObject, Nothing)
            If child IsNot Nothing AndAlso Not child.IsValid Then
              mChildren.Add(New BusinessObjectNode(child))
            End If

          End If
        End If
      Next

    End Sub

    Private mRuleDescription As String

    Public Sub New(ByVal RuleDescription As String)

      mRuleDescription = RuleDescription

    End Sub

  End Class

  Public Class BusinessObjectRuleNode
    Inherits BusinessObjectNodeBase

    Public Overrides ReadOnly Property ImageUri As Uri
      Get
        Return New Uri("/Singular.Silverlight;component/Images/ExclamationMark16.png", UriKind.Relative)
      End Get
    End Property

    Private mBusinessRule As Csla.Rules.BrokenRule

    Public ReadOnly Property BusinessRule As Csla.Rules.BrokenRule
      Get
        Return mBusinessRule
      End Get
    End Property

    Private mBusinessObject As Csla.Core.BusinessBase

    Public ReadOnly Property BusinessObject As Csla.Core.BusinessBase
      Get
        Return mBusinessObject
      End Get
    End Property

    Public Sub New(ByVal BusinessObject As Csla.Core.BusinessBase, ByVal BusinessRule As Csla.Rules.BrokenRule)

      mBusinessRule = BusinessRule
      mBusinessObject = BusinessObject
      mDescription = BusinessRule.Description

    End Sub

  End Class

End Class
