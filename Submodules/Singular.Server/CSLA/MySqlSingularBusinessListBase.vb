
<Serializable()> _
Public Class MySqlSingularBusinessListBase(Of T As MySqlSingularBusinessListBase(Of T, C), C As MySqlSingularBusinessBase(Of C))
  Inherits SingularBusinessListBase(Of T, C)

  Protected Overrides Sub UpdateTransactional(ByVal UpdateMethod As Action, ByVal ConnectionString As String)

    Dim cn As MySql.Data.MySqlClient.MySqlConnection = New MySql.Data.MySqlClient.MySqlConnection(ConnectionString)
    Csla.ApplicationContext.LocalContext("cn") = cn
    cn.Open()
    Try
      Dim tr As MySql.Data.MySqlClient.MySqlTransaction = Nothing
      If MySqlBusinessObjects.UseTransactions Then
        tr = cn.BeginTransaction(IsolationLevel.ReadUncommitted)
        Csla.ApplicationContext.LocalContext("tr") = tr
      End If
      Try

        UpdateMethod.Invoke()

        If MySqlBusinessObjects.UseTransactions Then
          tr.Commit()
        End If
      Catch ex As Exception
        If MySqlBusinessObjects.UseTransactions Then
          tr.Rollback()
        End If
        Throw ex
      End Try
    Finally
      Csla.ApplicationContext.LocalContext("cn") = Nothing
      cn.Close()
      cn.Dispose()
      Csla.ApplicationContext.LocalContext("tr") = Nothing
    End Try

  End Sub


End Class


Public Class MySqlBusinessObjects

  Public Shared Property UseTransactions As Boolean = True

End Class