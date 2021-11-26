Imports Singular.Security

Namespace Security

  Public Class FlatRoleList
    Inherits List(Of FlatRole)

    Public Sub New()

      Dim srhl As ROSecurityRoleHeaderList = ROSecurityRoleHeaderList.GetROSecurityRoleHeaderList

      For Each srh As ROSecurityRoleHeader In srhl
        For Each sr As ROSecurityRole In srh.ROSecurityRoleList

          Add(New FlatRole With {.SecurityRoleID = sr.SecurityRoleID, .SecurityRole = sr.SectionName & "." & sr.SecurityRole})
        Next
      Next

    End Sub

    Public Class FlatRole
      Public Property SecurityRoleID As Integer
      Public Property SecurityRole As String
    End Class


  End Class

 
End Namespace


