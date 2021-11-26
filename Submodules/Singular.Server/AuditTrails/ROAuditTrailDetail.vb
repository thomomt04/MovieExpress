Imports System.Reflection
Imports System.Data.SqlClient
Imports Csla.Data

Namespace AuditTrails

  <Serializable()> _
  Public Class ROAuditTrailDetailList
    Inherits SingularReadOnlyListBase(Of ROAuditTrailDetailList, ROAuditTrailDetail)

  End Class

  <Serializable()> _
  Public Class ROAuditTrailDetail
    Inherits SingularReadOnlyBase(Of ROAuditTrailDetail)

    Private mColumnName As String
    Private mOldValue As String
    Private mNewValue As String

#Region " Properties "

    Public ReadOnly Property Column() As String
      Get
        Return mColumnName
      End Get
    End Property

    Public ReadOnly Property OldValue() As String
      Get
        Return mOldValue
      End Get
    End Property

    Public ReadOnly Property NewValue() As String
      Get
        Return mNewValue
      End Get
    End Property

#End Region

#Region " Methods "

    Private Function GetStringValue(SqlVariantValue As Object) As String
      If Singular.Misc.IsNullNothing(SqlVariantValue) Then
        Return ""
      End If

      If TypeOf SqlVariantValue Is Date Then
        Return DirectCast(SqlVariantValue, Date).ToString("dd MMM yyyy - HH:mm:ss")
      ElseIf TypeOf SqlVariantValue Is Decimal Then
        Return DirectCast(SqlVariantValue, Decimal).ToString("#,##0.##########;(#,##0.##########)")
      Else
        Return SqlVariantValue.ToString
      End If
    End Function

    Public Sub Populate(ColumnName As String, NewValue As Object, NewLookupValue As String)
      mColumnName = ColumnName
      If String.IsNullOrEmpty(NewLookupValue) Then
        mNewValue = GetStringValue(NewValue)
      Else
        mNewValue = NewLookupValue
      End If

    End Sub

    Protected Overrides Function GetIdValue() As Object

      Return Me.mColumnName

    End Function

#End Region

#Region " Factory Methods "

    Public Shared Function GetROAuditTrailDetail(sdr As SafeDataReader) As ROAuditTrailDetail

      Return New ROAuditTrailDetail(sdr)

    End Function

    Public Sub New()

    End Sub

    Private Sub New(sdr As SafeDataReader)

      mColumnName = sdr.GetString(1)
      mOldValue = GetStringValue(sdr.GetValue(2))
      mNewValue = GetStringValue(sdr.GetValue(3))

    End Sub

#End Region

  End Class



End Namespace