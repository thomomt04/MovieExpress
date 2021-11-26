Imports Csla
Imports Csla.Data
Imports Singular.DataAnnotations
Imports System.Linq

Namespace AuditTrails

  <Serializable()> _
  Public Class ROAuditTrailTable
    Inherits SingularReadOnlyBase(Of ROAuditTrailTable)

    Private mAuditTrailTableID As Integer
    Private mTableName As String = ""

    Private mROAuditTrailHeaderList As New ROAuditTrailHeaderList

    Public ReadOnly Property AuditTrailTableID() As Integer
      Get
        Return mAuditTrailTableID
      End Get
    End Property

    Public ReadOnly Property TableName() As String
      Get
        Return mTableName
      End Get
    End Property

    <ClientOnlyNoData, ExpandOptions(ExpandOptions.RenderChildrenModeType.OnExpand)>
    Public Property IsExpanded As Boolean

    Public Property HasChildren As Boolean

    Public ReadOnly Property ROAuditTrailHeaderList() As ROAuditTrailHeaderList
      Get
        Return mROAuditTrailHeaderList
      End Get
    End Property

#Region " Methods "

    Public Shared Function GetAuditTrailTableID(TableName As String) As Integer
      Return Singular.CommandProc.GetDataValue("GetProcs.getAuditTrailTableID", {"@TableName"}, {TableName})
    End Function

    Public Sub PopulateInsertedDetails()

      Dim RowsWithInsertOnly = mROAuditTrailHeaderList.SelectMany(Function(c) c.ROAuditTrailList).Where(Function(c) c.Type = 1)

      If RowsWithInsertOnly.Count > 0 Then

        Dim xml = (New System.Xml.Linq.XElement("IDs", RowsWithInsertOnly.Select(Function(i) New System.Xml.Linq.XElement("ID", i.RowID)))).ToString()
        Dim RowsIndex = RowsWithInsertOnly.ToDictionary(Function(c) c.RowID)

        Dim cProc As New Singular.CommandProc("GetProcs.getROAuditTrailInsertList", {"@AuditTrailTableID", "@XMLIds"}, {mAuditTrailTableID, xml})
        cProc.ExecuteReaderLocal(
          Sub(sdr)

            While sdr.Read

              Dim at As ROAuditTrail = Nothing
              If RowsIndex.TryGetValue(sdr.GetInt32(0), at) Then
                Dim atd = New ROAuditTrailDetail
                atd.Populate(sdr.GetString(1), sdr.GetValue(2), sdr.GetString(3))
                at.ROAuditTrailDetailList.Add(atd)
              End If

            End While

          End Sub)

      End If

    End Sub

    Public Sub SetTableName(ByVal Name As String)
      mTableName = Name
    End Sub

    Protected Overrides Function GetIdValue() As Object
      Return Me.mTableName
    End Function

#End Region

#Region " Factory Methods "

    Friend Shared Function NewROAuditTrailTable() As ROAuditTrailTable

      Return New ROAuditTrailTable

    End Function

    Friend Shared Function GetROAuditTrailTable(ByVal dr As SafeDataReader) As ROAuditTrailTable

      Return New ROAuditTrailTable(dr)

    End Function

    Private Sub New()


    End Sub

    Private Sub New(ByVal dr As SafeDataReader)

      Fetch(dr)

    End Sub

#End Region

#Region " Data Access "

    Protected Sub Fetch(ByRef dr As SafeDataReader)

      With dr
        mAuditTrailTableID = .GetInt32(0)
        mTableName = Singular.Strings.Readable(.GetString(1))
        HasChildren = .GetBoolean(4)
      End With

    End Sub

#End Region

  End Class


End Namespace