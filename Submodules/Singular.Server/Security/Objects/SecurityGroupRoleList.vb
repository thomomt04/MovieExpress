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
  Public Class SecurityGroupRoleList
    Inherits SingularBusinessListBase(Of SecurityGroupRoleList, SecurityGroupRole)

#Region " Parent "

    <NotUndoable()> Private mParent As SecurityGroup
#End Region

#Region " Business Methods "

#If SILVERLIGHT Then
#Else
    Public Overloads Sub BeginEdit()

      If Settings.CurrentPlatform = CommonDataPlatform.Windows Then
        Me.FlattenEditLevels()
      End If

      MyBase.BeginEdit()

    End Sub

    Public Overloads Sub CancelEdit()

      If Settings.CurrentPlatform = CommonDataPlatform.Windows Then
        Me.FlattenEditLevels()
      End If

      MyBase.CancelEdit()

    End Sub

    Public Overloads Sub ApplyEdit()

      If Settings.CurrentPlatform = CommonDataPlatform.Windows Then
        Me.FlattenEditLevels()
      End If

      MyBase.ApplyEdit()

    End Sub
#End If

    Public Function Find(ByVal SecurityRoleID As Integer) As SecurityGroupRole
      For Each child As SecurityGroupRole In Me
        If child.SecurityRoleID = SecurityRoleID Then
          Return child
        End If
      Next
      Return Nothing
    End Function

    Public Function GetItem(ByVal SecurityGroupRoleID As Integer) As SecurityGroupRole

      For Each child As SecurityGroupRole In Me
        If child.SecurityGroupRoleID = SecurityGroupRoleID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Security Group Roles"

    End Function

#End Region

#Region " Data Access "

#Region " Common "

    Public Shared Function NewSecurityGroupRoleList() As SecurityGroupRoleList

      Return New SecurityGroupRoleList()

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

    Public Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(SecurityGroupRole.GetSecurityGroupRole(sdr))
      End While
      Me.RaiseListChangedEvents = True

    End Sub

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As SecurityGroupRole In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As SecurityGroupRole In Me
          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

    Protected Overrides Sub DataPortal_Update()

      MyBase.UpdateTransactional(AddressOf Update)

    End Sub

#End If

#End Region

#End Region

    
  End Class


End Namespace