Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Security

  <Serializable()> _
  Public Class ROSecurityRoleList
    Inherits SingularReadOnlyListBase(Of ROSecurityRoleList, ROSecurityRole)

#Region " Parent "

    <NotUndoable()> Private mParent As ROSecurityRoleHeader
#End Region

#Region " Business Methods "

    Public Sub Add2(ROSecurityRole As ROSecurityRole)

      IsReadOnly = False
      AllowNew = True
      AllowEdit = True

      Me.Add(ROSecurityRole)

    End Sub

    Public Sub AllowAdd()
      IsReadOnly = False
    End Sub

    Public Sub DisallowAdd()
      IsReadOnly = True
    End Sub

    Public Function GetItem(SecurityRoleID As Integer) As ROSecurityRole

      For Each child As ROSecurityRole In Me
        If child.SecurityRoleID = SecurityRoleID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "s"

    End Function

#End Region

#Region " Data Access "

#Region " Common "

    Public Shared Function NewROSecurityRoleList() As ROSecurityRoleList

      Return New ROSecurityRoleList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Me.Add(ROSecurityRole.GetROSecurityRole(sdr))
      End While
      Me.IsReadOnly = True
      Me.RaiseListChangedEvents = True

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace