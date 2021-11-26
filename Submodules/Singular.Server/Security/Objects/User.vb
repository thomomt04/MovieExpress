Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Singular.Localisation
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If
Imports Csla.Core

Namespace Security

  <Serializable()> _
  Public Class User
    Inherits UserBase(Of User)

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Sub New()

      MarkAsChild()
      BusinessRules.CheckRules()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewUser() As User

      Return New User()
      ' DataPortal.Create(Of User)(Csla.DataPortal.ProxyModes.LocalOnly)

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetUser(UserID As Integer) As User

      Dim list As UserList = UserList.GetUserList(UserID)
      If list.Count = 1 Then
        Return list(0)
      Else
        Return Nothing
      End If

    End Function

    Friend Shared Function GetUser(ByVal dr As SafeDataReader) As User

      Dim u As New User()
      u.Fetch(dr)
      Return u

    End Function

#End If

#End Region

#End Region

  End Class


End Namespace
