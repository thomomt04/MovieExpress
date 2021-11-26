Imports Csla
Imports Csla.Data

Namespace AuditTrails

  <Serializable()> _
  Public Class ROAuditTrailTableList
    Inherits SingularReadOnlyListBase(Of ROAuditTrailTableList, ROAuditTrailTable)

    Public Property AuditTrailUserList As ROAuditTrailUserList

    Public Sub FetchChildren(HeaderList As ROAuditTrailTableList, Optional ByVal Levels As Integer = 1, Optional ConnectionString As String = Nothing)
      For Each t As ROAuditTrailTable In Me
        For Each h As ROAuditTrailHeader In t.ROAuditTrailHeaderList
          h.FetchChildren(HeaderList, Levels, ConnectionString)
        Next
      Next
    End Sub

#Region " Factory Methods "

    Public Shared Function GetROAuditTrailTableList(TableID As Integer, KeyValue As Integer) As ROAuditTrailTableList
      Return DataPortal.Fetch(Of ROAuditTrailTableList)(New Criteria() With {.AuditTrailTableID = TableID, .RowID = KeyValue})
    End Function

    Public Shared Function GetROAuditTrailTableList(ByVal AuditTrailTableID As Integer?, ByVal RowID As Integer?, ByVal ParentTableID As Integer?, ByVal ParentID As Integer?,
                                                    ByVal StartDate As Date?, ByVal EndDate As Date?, ByVal UserID As Integer?,
                                                    Optional ConnectionString As String = Nothing) As ROAuditTrailTableList

      Return DataPortal.Fetch(Of ROAuditTrailTableList)(New Criteria() With {.AuditTrailTableID = AuditTrailTableID, .RowID = RowID, .ParentTableID = ParentTableID, .ParentID = ParentID,
                                                                              .StartDate = StartDate, .EndDate = EndDate, .UserID = UserID, .ConnectionString = ConnectionString})

    End Function

    Public Shared Function GetROAuditTrailTableList(Criteria As Criteria) As ROAuditTrailTableList
      Return DataPortal.Fetch(Of ROAuditTrailTableList)(Criteria)
    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria

      Public Property ConnectionString As String = Nothing

      Public Property AuditTrailTableID As Integer?
      Public Property RowID As Integer?
      Public Property ParentTableID As Integer?
      Public Property ParentID As Integer?
      Public Property StartDate As Date?
      Public Property EndDate As Date?
      Public Property UserID As Integer?

    End Class

    Private Sub Fetch(ByVal sdr As SafeDataReader, crit As Criteria)

      Dim HeaderIndex As New Dictionary(Of String, ROAuditTrailHeader)
      Dim AuditTrailIndex As New Dictionary(Of Integer, ROAuditTrail)

      'Users
      AuditTrailUserList = New ROAuditTrailUserList(sdr)

      'Tables
      sdr.NextResult()
      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Me.Add(ROAuditTrailTable.GetROAuditTrailTable(sdr))
      End While

      BuildIndex(Function(c) c.AuditTrailTableID)

      'Headers (Audit Trails Grouped by Key Value)
      If sdr.NextResult Then
        Dim Table As ROAuditTrailTable = Nothing
        While sdr.Read
          If Table Is Nothing OrElse Table.AuditTrailTableID <> sdr.GetInt32(0) Then
            Table = Me.GetItemIndexed(sdr.GetInt32(0))
          End If

          Dim TrailHeader As ROAuditTrailHeader = ROAuditTrailHeader.GetROAuditTrailHeader(sdr)
          Table.ROAuditTrailHeaderList.Add(TrailHeader)
          HeaderIndex.Add(String.Join("|", TrailHeader.AuditTrailTableID, TrailHeader.RowID), TrailHeader)
        End While
      End If

      'AuditTrails
      If sdr.NextResult Then
        Dim Header As ROAuditTrailHeader = Nothing
        Dim First As Boolean = False

        While sdr.Read
          'Index not needed, because the results are ordered from the DB
          If Header Is Nothing OrElse Header.AuditTrailTableID <> sdr.GetInt32(1) OrElse Header.RowID <> sdr.GetInt32(2) Then
            Header = HeaderIndex(String.Join("|", sdr.GetInt32(1), sdr.GetInt32(2)))
            First = True
          End If

          Dim AT = ROAuditTrail.GetROAuditTrail(sdr)
          Header.ROAuditTrailList.Add(AT)
          If AT.AuditTrailID > 0 Then AuditTrailIndex.Add(AT.AuditTrailID, AT)

          If First Then
            First = False
            Header.Type = AT.Type
          End If

        End While
      End If

      'AuditTrail Details
      If sdr.NextResult Then
        Dim AuditTrail As ROAuditTrail = Nothing
        While sdr.Read
          If AuditTrail Is Nothing OrElse AuditTrail.AuditTrailID <> sdr.GetInt32(0) Then
            AuditTrail = AuditTrailIndex(sdr.GetInt32(0))
          End If
          AuditTrail.ROAuditTrailDetailList.Add(ROAuditTrailDetail.GetROAuditTrailDetail(sdr))
        End While
      End If

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlClient.SqlConnection(If(crit.ConnectionString, Settings.ConnectionString))
        cn.Open()
        Using cm As SqlClient.SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.StoredProcedure
          cm.CommandText = "[GetProcs].[getROAuditTrailHeaderList]"
          cm.Parameters.AddWithValue("@AuditTrailTableID", crit.AuditTrailTableID)
          cm.Parameters.AddWithValue("@RowID", crit.RowID)
          cm.Parameters.AddWithValue("@ParentTableID", crit.ParentTableID)
          cm.Parameters.AddWithValue("@ParentID", crit.ParentID)
          cm.Parameters.AddWithValue("@StartDate", If(crit.StartDate, New Date(1900, 1, 1)))
          cm.Parameters.AddWithValue("@EndDate", If(crit.EndDate, New Date(2999, 1, 1)))
          cm.Parameters.AddWithValue("@UserID", crit.UserID)
          Using sdr As New SafeDataReader(cm.ExecuteReader)
            Fetch(sdr, crit)
          End Using
        End Using
      End Using

    End Sub

#End Region

  End Class


End Namespace