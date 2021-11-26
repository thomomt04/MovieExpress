Imports Singular.AuditTrails

Namespace AuditTrails

  Public Interface IAuditTrailLookupVM
    Property AuditTrailLookup As AuditTrailLookup
  End Interface

  Public Class AuditTrailLookup
    Implements IAuditTrailHeader

    Private _AuditTrailTableList As ROAuditTrailTableList

    Public ReadOnly Property ROAuditTrailTableList As Singular.AuditTrails.ROAuditTrailTableList Implements IAuditTrailHeader.ROAuditTrailTableList
      Get
        Return _AuditTrailTableList
      End Get
    End Property

    Public Property SelectedHeader As ROAuditTrailHeader

    <Singular.DataAnnotations.DropDownWeb("ATHelper.GetColumnList()", UnselectedText:="(Show All)", ValueMember:="Column", DisplayMember:="Column", ComboDeselectText:="(Show All)", OnItemSelectJSFunction:="ATHelper.ApplyColumnFilter")>
    Public Property ColumnFilter As String = ""

    Public Property HideFilteredRows As Boolean = False

    Public Shared Function Create(AuditTrailTableList As ROAuditTrailTableList, ViewModel As IViewModel, Optional ChildLevelsToFetch As Integer = 0, Optional ConnectionString As String = Nothing)
      Dim atl As New AuditTrailLookup
      atl._AuditTrailTableList = AuditTrailTableList

      If ChildLevelsToFetch > 0 Then
        AuditTrailTableList.FetchChildren(AuditTrailTableList, ChildLevelsToFetch, ConnectionString)
      End If

      'If this is only for 1 record, and the record has no children, select the record.
      If atl._AuditTrailTableList.Count = 1 AndAlso atl._AuditTrailTableList(0).ROAuditTrailHeaderList.Count = 1 Then
        Dim SingleRecord = atl._AuditTrailTableList(0).ROAuditTrailHeaderList(0)
        If SingleRecord.ROAuditTrailTableList Is Nothing OrElse SingleRecord.ROAuditTrailTableList.Count = 0 Then atl.SelectedHeader = SingleRecord

      End If

      ViewModel.ClientDataProvider.AddDataSource("ATUserList", AuditTrailTableList.AuditTrailUserList, False)
      Return atl
    End Function

    Private Sub New()

    End Sub

  End Class

End Namespace