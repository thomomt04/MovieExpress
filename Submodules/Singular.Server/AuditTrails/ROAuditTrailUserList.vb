Imports Csla
Imports Csla.Data

Namespace AuditTrails

  Public Class ROAuditTrailUserList
    Inherits List(Of ROAuditTrailUser)

    Private Index As New Dictionary(Of Integer, ROAuditTrailUser)

    Public Overloads Sub Add(atu As ROAuditTrailUser)
      MyBase.Add(atu)
      Index.Add(atu.AuditTrailUserID, atu)
    End Sub

    Public Sub New(sdr As SafeDataReader)
      While sdr.Read
        Dim atu As New ROAuditTrailUser(sdr)
        Me.Add(atu)
      End While
    End Sub

    Public Sub Merge(NewUserList As ROAuditTrailUserList)
      For Each atu In NewUserList
        If Not Index.ContainsKey(atu.AuditTrailUserID) Then
          Add(atu)
        End If
      Next
    End Sub

    Public Function GetIndexed(AuditTrailUserID As Integer) As ROAuditTrailUser
      Dim Item As ROAuditTrailUser = Nothing
      Index.TryGetValue(AuditTrailUserID, Item)
      Return Item
    End Function

  End Class

  Public Class ROAuditTrailUser

    Public Property AuditTrailUserID As Integer
    Public Property UserID As Integer?
    Public Property UserName As String

    Public Sub New(sdr As SafeDataReader)

      AuditTrailUserID = sdr.GetInt32(0)
      UserID = sdr.GetValue(1)

      Dim DomainName As String = sdr.GetString(3)
      UserName = sdr.GetString(4)

      If String.IsNullOrEmpty(UserName) Then
        UserName = DomainName
      End If

    End Sub

  End Class

End Namespace