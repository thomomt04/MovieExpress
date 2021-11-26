Imports MySql.Data.MySqlClient

<Serializable()>
Public Class MySqlSingularBusinessBase(Of C As MySqlSingularBusinessBase(Of C))
  Inherits SingularBusinessBase(Of C)

  Protected Overridable Overloads Sub DoInsertUpdateChild(ByVal cm As MySqlCommand)

    cm.Connection = CType(Csla.ApplicationContext.LocalContext("cn"), MySqlConnection)
    cm.Transaction = Csla.ApplicationContext.LocalContext("tr")

    If cm.Connection Is Nothing Then
      'Changed by Marlborough Dec 2012.
      'If the connection has not been set up, then the programmer is doing something like SingleObject.Save, but the update method is calling DoInsertUpdateChild.
      'If so, then just call DoInsertUpdateParent and save the hassle. 
      'Otherwise, SingularBusinessBase should decide for you, If IsChild DoInsertUpdateChild() Else DoInsertUpdateParent()
      DoInsertUpdateParent(cm, Settings.ConnectionString)

    Else
      'Normal Child Object
      CSLALib.ContextInfo.SetContextInfoOnConnection(Of C)(cm.Connection, cm.Transaction)
      InsertUpdate(cm)
    End If

  End Sub

  Protected Overridable Overloads Sub InsertUpdate(ByVal cm As MySqlCommand)

    Throw New Exception("InsertUpdate must be overridden in the inheriting class: " & Me.GetType.Name)

  End Sub

  Protected Friend Overrides Sub InsertUpdateGeneric()
    Using cm As New MySqlCommand
      DoInsertUpdateChild(cm)
    End Using
  End Sub

  Protected Overridable Overloads Sub DoDeleteChild(ByVal cm As MySqlCommand)

    cm.Connection = CType(Csla.ApplicationContext.LocalContext("cn"), MySqlConnection)
    cm.Transaction = Csla.ApplicationContext.LocalContext("tr")

    CSLALib.ContextInfo.SetContextInfoOnConnection(Of C)(cm.Connection, cm.Transaction)

    ' DeleteFromDB(cm)

    If cm.Connection Is Nothing Then

      DoDeleteParent(cm, Settings.ConnectionString)

    Else
      'Normal Child Object
      CSLALib.ContextInfo.SetContextInfoOnConnection(Of C)(cm.Connection, cm.Transaction)
      DeleteFromDB(cm)
    End If

  End Sub


  Protected Overridable Overloads Sub DoInsertUpdateParent(ByVal cm As MySqlCommand)

    DoInsertUpdateParent(cm, Settings.ConnectionString)

  End Sub

  Protected Overridable Overloads Sub DoInsertUpdateParent(ByVal cm As MySqlCommand, ByVal ConnectionString As String)

    Dim cn As MySqlConnection = New MySqlConnection(ConnectionString)
    Csla.ApplicationContext.LocalContext("cn") = cn
    cn.Open()
    Try
      Dim tr As MySqlTransaction = Nothing
      If MySqlBusinessObjects.UseTransactions Then
        tr = cn.BeginTransaction(IsolationLevel.ReadUncommitted)
        Csla.ApplicationContext.LocalContext("tr") = tr
      End If

      Try
        cm.Connection = cn
        cm.Transaction = tr

        CSLALib.ContextInfo.SetContextInfoOnConnection(Of C)(cn, tr)

        InsertUpdate(cm)

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
      Csla.ApplicationContext.LocalContext("tr") = Nothing
    End Try

  End Sub

  Protected Overridable Overloads Sub DoDeleteParent(ByVal cm As MySqlCommand)

    DoDeleteParent(cm, Settings.ConnectionString)

  End Sub

  Protected Overridable Overloads Sub DoDeleteParent(ByVal cm As MySqlCommand, ByVal ConnectionString As String)

    Dim cn As MySqlConnection = New MySqlConnection(ConnectionString)
    Csla.ApplicationContext.LocalContext("cn") = cn
    cn.Open()
    Try
      Dim tr As MySqlTransaction = Nothing
      If MySqlBusinessObjects.UseTransactions Then
        tr = cn.BeginTransaction(IsolationLevel.ReadCommitted)
        Csla.ApplicationContext.LocalContext("tr") = tr
      End If

      Try
        cm.Connection = cn
        cm.Transaction = tr

        CSLALib.ContextInfo.SetContextInfoOnConnection(Of C)(cn, tr)

        DeleteFromDB(cm)

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
      Csla.ApplicationContext.LocalContext("tr") = Nothing
    End Try

  End Sub

  Protected Overridable Overloads Sub DeleteFromDB(ByVal cm As MySqlCommand)

    Throw New Exception("DeleteFromDB must be overridden in the inheriting class: " & Me.GetType.Name)

  End Sub

End Class
