

Namespace CheckQueries

  Public Class CodeCheckResult

    Public Property CheckName As String

    Public Property FailedData As DataTable

    Public ReadOnly Property HasFailures As Boolean
      Get
        Return FailedData IsNot Nothing AndAlso FailedData.Rows.Count > 0
      End Get
    End Property

    Public Sub New(CheckName As String)

      Me.CheckName = CheckName

    End Sub

  End Class

End Namespace