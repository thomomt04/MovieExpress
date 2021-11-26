

Namespace Data

  Public Class MySQL

    Public Shared Function ConnectionString(ByVal ServerName As String, ByVal DBName As String, ByVal Username As String, ByVal Password As String, Optional ByVal Port As String = "3306") As String

      Return String.Format("server={0};user={1};database={2};password={3};port={4};", ServerName, Username, DBName, Password, Port)

    End Function

  End Class

End Namespace
