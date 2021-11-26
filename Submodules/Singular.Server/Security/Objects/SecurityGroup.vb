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

Namespace Security

  <Serializable()> _
  Public Class SecurityGroup
    Inherits SecurityGroupBase(Of SecurityGroup)

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Sub New()

      MarkAsChild()
      BusinessRules.CheckRules()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewSecurityGroup() As SecurityGroup

      Return New SecurityGroup()

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetSecurityGroup(ByVal dr As SafeDataReader) As SecurityGroup

      Dim s As New SecurityGroup()
      s.Fetch(dr)
      Return s

    End Function

    Public Sub SaveSecurityGroupRoleList()

      Dim SavedSecurityGroupRoleList = Me.SecurityGroupRoleList.Save()
      LoadProperty(SecurityGroupRoleListProperty, SavedSecurityGroupRoleList)

    End Sub

#End If

#End Region

#End Region


  End Class


End Namespace