Imports Csla
Imports Csla.Serialization
Imports System.ComponentModel.DataAnnotations

Namespace Security

  <Serializable()> _
  Public Class Identity
    Inherits IdentityBase(Of Identity)

    Public Sub New()

      MyBase.New()

    End Sub

#If SILVERLIGHT Then
#Else

    Protected Overrides Sub SetupSqlCommand(ByVal cmd As System.Data.SqlClient.SqlCommand, ByVal Criteria As IdentityCriterea)

      cmd.CommandText = "[GetProcs].[SLLogin]"

    End Sub

#End If

  End Class

End Namespace
