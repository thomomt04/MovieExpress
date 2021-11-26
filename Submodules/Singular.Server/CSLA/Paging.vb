Imports System.ComponentModel
Imports Singular.DataAnnotations

Namespace Paging

  Public Interface IPagedList
    ReadOnly Property TotalRecords As Integer
  End Interface

  Public Interface IPageCriteria
    Property PageSize As Integer
    Property SortColumn As String
    Property SortAsc As Boolean
  End Interface

  Public Class PageCriteria(Of CriteriaType As PageCriteria(Of CriteriaType))
    Inherits SingularCriteriaBase(Of CriteriaType)
    Implements IPageCriteria

    Public Sub New()

    End Sub

    <Singular.DataAnnotations.ClientOnlyNoData> Public Property PageNo As Integer = 1
    <Singular.DataAnnotations.ClientOnlyNoData> Public Property PageSize As Integer Implements IPageCriteria.PageSize
    <Singular.DataAnnotations.ClientOnlyNoData> Public Property SortColumn As String Implements IPageCriteria.SortColumn
    <Singular.DataAnnotations.ClientOnlyNoData> Public Property SortAsc As Boolean Implements IPageCriteria.SortAsc '= True

    Public Sub AddParameters(cm As SqlClient.SqlCommand)
      cm.Parameters.AddWithValue("@PageNo", PageNo)
      cm.Parameters.AddWithValue("@PageSize", PageSize)
      cm.Parameters.AddWithValue("@SortColumn", SortColumn)
      cm.Parameters.AddWithValue("@SortAsc", SortAsc)
    End Sub

  End Class

  Public Class PageCriteria
    Inherits PageCriteria(Of PageCriteria)

    Public Sub New()

    End Sub

  End Class

  'Public Interface IEditablePagedList
  '  ReadOnly Property TotalRecords As Integer
  'End Interface

  'Public Interface IEditablePageCriteria
  '  Property PageSize As Integer
  '  Property SortColumn As String
  'End Interface

  'Public Class EditablePageCriteria(Of CriteriaType As EditablePageCriteria(Of CriteriaType))
  '  Inherits SingularCriteriaBase(Of CriteriaType)
  '  Implements IEditablePageCriteria

  '  Public Sub New()

  '  End Sub

  '  Public Property PageNo As Integer = 1
  '  Public Property PageSize As Integer Implements IEditablePageCriteria.PageSize
  '  Public Property SortColumn As String Implements IEditablePageCriteria.SortColumn
  '  Public Property SortAsc As Boolean = True

  '  Public Sub AddParameters(cm As SqlClient.SqlCommand)
  '    cm.Parameters.AddWithValue("@PageNo", PageNo)
  '    cm.Parameters.AddWithValue("@PageSize", PageSize)
  '    cm.Parameters.AddWithValue("@SortColumn", SortColumn)
  '    cm.Parameters.AddWithValue("@SortAsc", SortAsc)
  '  End Sub

  'End Class

  'Public Class EditablePageCriteria
  '  Inherits PageCriteria(Of PageCriteria)

  '  Public Sub New()

  '  End Sub

  'End Class

End Namespace
