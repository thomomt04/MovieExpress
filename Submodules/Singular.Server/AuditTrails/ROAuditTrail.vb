Imports System.Reflection
Imports System.Data.SqlClient
Imports Csla.Data
Imports System.ComponentModel
Imports Singular.DataAnnotations

Namespace AuditTrails

  <Serializable()> _
  Public Class ROAuditTrailList
    Inherits SingularReadOnlyListBase(Of ROAuditTrailList, ROAuditTrail)

  End Class

  <Serializable()> _
  Public Class ROAuditTrail
    Inherits SingularReadOnlyBase(Of ROAuditTrail)

#Region " AuditTrailTypes "

    Public Enum AuditTrailType
      None = 0
      Inserted = 1
      Updated = 2
      Deleted = 3
    End Enum

#End Region

    Private mAuditTrailID As Integer = 0
    Private mAuditTrailTableID As Integer = 0
    Private mRowID As Integer = 0
    Private mAuditTrailType As AuditTrailType
    Private mChageDateTime As Date
    Private mAuditTrailUserID As Integer = 0

#Region " Properties "

    <Browsable(False)>
    Public ReadOnly Property AuditTrailID() As Integer
      Get
        Return mAuditTrailID
      End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property AuditTrailTableID() As String
      Get
        Return mAuditTrailTableID
      End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property RowID() As Integer
      Get
        Return mRowID
      End Get
    End Property

    Public ReadOnly Property Type() As AuditTrailType
      Get
        Return mAuditTrailType
      End Get
    End Property

    <ComponentModel.DataAnnotations.Display(Name:="Change Date Time"), Singular.DataAnnotations.DateField(FormatString:="dd MMM yyyy HH:mm")>
    Public ReadOnly Property ChageDateTime() As Date
      Get
        Return mChageDateTime
      End Get
    End Property

    Public ReadOnly Property ATUserID() As Integer
      Get
        Return mAuditTrailUserID
      End Get
    End Property

    <ClientOnlyNoData>
    Public Property UserName As String

    <ClientOnlyNoData, ExpandOptions(ExpandOptions.RenderChildrenModeType.OnExpand)>
    Public Property IsExpanded As Boolean

    <ClientOnlyNoData>
    Public Property IsFiltered As Boolean

#End Region

#Region " Child Lists "

    Private mROAuditTrailDetailList As New ROAuditTrailDetailList

    Public ReadOnly Property ROAuditTrailDetailList() As ROAuditTrailDetailList
      Get
        Return mROAuditTrailDetailList
      End Get
    End Property

#End Region

#Region " Methods "

    Protected Overrides Function GetIdValue() As Object

      Return Me.mAuditTrailID

    End Function

    Public Function GetColumnValue(ColumnName As String, RemoveBrackets As Boolean) As String
      Dim Col = mROAuditTrailDetailList.Where(Function(c) c.Column = ColumnName).FirstOrDefault
      Dim Value = If(Col Is Nothing, "", If(String.IsNullOrEmpty(Col.NewValue), Col.OldValue, Col.NewValue))
      If RemoveBrackets AndAlso Value.Contains(" (") Then
        Return Value.Substring(0, Value.LastIndexOf(" ("))
      Else
        Return Value
      End If
    End Function

#End Region

#Region " Factory Methods "

    Public Shared Function GetROAuditTrail(ByVal sdr As SafeDataReader) As ROAuditTrail

      Return New ROAuditTrail(sdr)

    End Function

    Private Sub New(ByVal dr As SafeDataReader)

      Fetch(dr)

    End Sub

#End Region

#Region " Data Access "

    Protected Sub Fetch(ByRef dr As SafeDataReader)

      With dr
        mAuditTrailID = .GetInt32(0)
        mAuditTrailTableID = .GetInt32(1)
        mRowID = .GetInt32(2)
        mAuditTrailType = .GetByte(3)
        mChageDateTime = .GetDateTime(4)
        mAuditTrailUserID = .GetInt32(5)
      End With

    End Sub

#End Region

  End Class



End Namespace