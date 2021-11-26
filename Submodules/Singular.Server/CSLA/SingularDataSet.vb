Public Class SingularDataSet

#Region " Helper Functions "

  Public Shared Function ConvertDotNetDataSetToSingularDataSet(ByVal DN_DataSet As System.Data.DataSet) As SingularDataSet.DataSet

    Dim SNG_DS As New SingularDataSet.DataSet
    For Each dt As System.Data.DataTable In DN_DataSet.Tables
      Dim SNG_DT As SingularDataSet.DataTable = ConvertDotNetDataTableToSingularDataTable(dt)
      SNG_DS.Tables.Add(SNG_DT)
    Next
    Return SNG_DS

  End Function

  Public Shared Function ConvertDotNetDataTableToSingularDataTable(ByVal DN_DataTable As System.Data.DataTable) As SingularDataSet.DataTable

    Dim SNG_DT As New SingularDataSet.DataTable(DN_DataTable.TableName)
    For Each dc As System.Data.DataColumn In DN_DataTable.Columns
      Dim SNG_DC As New SingularDataSet.DataColumn(dc.DataType, dc.ColumnName)
      SNG_DT.Columns.Add(SNG_DC)
    Next
    For Each dr As System.Data.DataRow In DN_DataTable.Rows
      Dim SNG_DR As SingularDataSet.DataRow = ConvertDotNetDataRowToSingularDataRow(dr)
      SNG_DT.Rows.Add(SNG_DR)
    Next
    Return SNG_DT

  End Function

  Public Shared Function ConvertDotNetDataRowToSingularDataRow(ByVal DN_DataRow As System.Data.DataRow) As SingularDataSet.DataRow

    Dim SNG_DR As New SingularDataSet.DataRow
    For Each dc As System.Data.DataColumn In DN_DataRow.Table.Columns
      SNG_DR.Cells.Add(DN_DataRow(dc))
    Next
    Return SNG_DR

  End Function

  Public Shared Function ConvertSingularDataTableToDotNetDataTable(ByVal SNG_DataTable As SingularDataSet.DataTable) As System.Data.DataTable

    Dim dt As New System.Data.DataTable(SNG_DataTable.TableName)
    For Each SNG_DC As SingularDataSet.DataColumn In SNG_DataTable.Columns
      Dim dc As New System.Data.DataColumn(SNG_DC.ColumnName, SNG_DC.Type)
      dt.Columns.Add(dc)
    Next
    For Each SNG_DR As SingularDataSet.DataRow In SNG_DataTable.Rows
      Dim dr As System.Data.DataRow = dt.NewRow
      dt.Rows.Add(dr)
      For i As Integer = 0 To SNG_DR.Cells.Count - 1
        dr(i) = SNG_DR.Cells(i)
      Next
    Next
    Return dt

  End Function

  Public Shared Function ConvertSingularDataSetToDotNetDataSet(ByVal SNG_DataSet As SingularDataSet.DataSet) As System.Data.DataSet

    Dim ds As New System.Data.DataSet
    For Each SNG_DT As SingularDataSet.DataTable In SNG_DataSet.Tables
      Dim dt As System.Data.DataTable = ConvertSingularDataTableToDotNetDataTable(SNG_DT)
      ds.Tables.Add(dt)
    Next
    Return ds

  End Function

#End Region

#Region " DataSet Classes "

  <Serializable()> _
  Public Class DataSet

    Private mTables As New List(Of DataTable)
    Public ReadOnly Property Tables() As List(Of DataTable)
      Get
        Return mTables
      End Get
    End Property

    Public Function ConvertToADODataSet() As System.Data.DataSet
      Return Singular.SingularDataSet.ConvertSingularDataSetToDotNetDataSet(Me)
    End Function


  End Class

  <Serializable()> _
  Public Class DataTable

    Private mTableName As String
    Private mColumns As New List(Of DataColumn)
    Private mRows As New List(Of DataRow)
    Public Sub New()

    End Sub

    Public Sub New(ByVal TableName As String)
      mTableName = TableName
    End Sub

    Public ReadOnly Property TableName() As String
      Get
        Return mTableName
      End Get
    End Property

    Public ReadOnly Property Columns() As List(Of DataColumn)
      Get
        Return mColumns
      End Get
    End Property

    Public ReadOnly Property Rows() As List(Of DataRow)
      Get
        Return mRows
      End Get
    End Property

  End Class

  <Serializable()> _
  Public Class DataRow

    Private mCells As New List(Of Object)

    Public ReadOnly Property Cells() As List(Of Object)
      Get
        Return mCells
      End Get
    End Property

  End Class

  <Serializable()> _
  Public Class DataColumn

    Private mType As Type
    Private mColumnName As String

    Public Sub New(ByVal Type As Type, ByVal ColumnName As String)

      mType = Type
      mColumnName = ColumnName

    End Sub

    Public ReadOnly Property Type() As Type
      Get
        Return mType
      End Get
    End Property

    Public ReadOnly Property ColumnName() As String
      Get
        Return mColumnName
      End Get
    End Property

  End Class

#End Region

End Class