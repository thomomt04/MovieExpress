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
  Public Class SecurityModel
    Inherits SecurityModelBase(Of SecurityModel, 
                                   SecurityGroupList, 
                                   SecurityGroup)

    Protected Overrides Function CreateNewSecurityGroupList() As ISecurityGroupList

      Return Singular.Security.SecurityGroupList.NewSecurityGroupList

    End Function

#If SILVERLIGHT Then

#Else

    Protected Overrides Function GetSecurityGroupList() As ISecurityGroupList

      Return Singular.Security.SecurityGroupList.GetSecurityGroupList

    End Function

#End If



    '#Region " Properties and Methods "

    '#Region " Properties "



    '#End Region

    '#Region " Child Lists "

    '    Public Shared SecurityGroupListProperty As PropertyInfo(Of SecurityGroupList) = RegisterProperty(Of SecurityGroupList)(Function(c) c.SecurityGroupList, "SecurityGroupList")
    '    Public ReadOnly Property SecurityGroupList() As SecurityGroupList
    '      Get
    '        If Not FieldManager.FieldExists(SecurityGroupListProperty) Then
    '          LoadProperty(SecurityGroupListProperty, SecurityGroupList.NewSecurityGroupList)
    '        End If
    '        Return GetProperty(SecurityGroupListProperty)
    '      End Get
    '    End Property

    '    Public Shared UserListProperty As PropertyInfo(Of UserList) = RegisterProperty(Of UserList)(Function(c) c.UserList, "UserList")
    '    Public ReadOnly Property UserList() As UserList
    '      Get
    '        If Not FieldManager.FieldExists(UserListProperty) Then
    '          LoadProperty(UserListProperty, UserList.NewUserList)
    '        End If
    '        Return GetProperty(UserListProperty)
    '      End Get
    '    End Property

    '    Public Shared ROSecurityRoleHeaderListProperty As PropertyInfo(Of ROSecurityRoleHeaderList) = RegisterProperty(Of ROSecurityRoleHeaderList)(Function(c) c.ROSecurityRoleHeaderList, "ROSecurityRoleList")
    '    Public ReadOnly Property ROSecurityRoleHeaderList() As ROSecurityRoleHeaderList
    '      Get
    '        If Not FieldManager.FieldExists(ROSecurityRoleHeaderListProperty) Then
    '          LoadProperty(ROSecurityRoleHeaderListProperty, ROSecurityRoleList.NewROSecurityRoleList)
    '        End If
    '        Return GetProperty(ROSecurityRoleHeaderListProperty)
    '      End Get
    '    End Property


    '#End Region

    '#Region " Methods "

    '    Public Overrides Function ToString() As String
    '      Return "Security"
    '    End Function

    '#End Region

    '#End Region

    '#Region " Validation Rules "

    '    Protected Overrides Sub AddBusinessRules()

    '      MyBase.AddBusinessRules()

    '    End Sub

    '#End Region

    '#Region " Data Access & Factory Methods "

    '#Region " Common "

    '    Public Sub New()

    '#If SILVERLIGHT Then
    '#Else
    '      LoadProperty(SecurityGroupListProperty, SecurityGroupList.GetSecurityGroupList)
    '      LoadProperty(UserListProperty, UserList.GetUserList)
    '      LoadProperty(ROSecurityRoleHeaderListProperty, ROSecurityRoleHeaderList.GetROSecurityRoleHeaderList)

    '      MarkOld()

    '#End If

    '      MarkAsChild()
    '      BusinessRules.CheckRules()

    '    End Sub

    '#End Region

    '#Region " Silverlight "

    '#If SILVERLIGHT Then

    '    Public Shared Function NewSecurityModel() As SecurityModel

    '      Return New SecurityModel()

    '    End Function

    '#End Region

    '#Region " .NET Data Access "

    '#Else

    '#End Region

    '#Region " .Net Data Access "

    '    Friend Sub Update()

    '      ' if we're not dirty then don't update the database
    '      Using cm As SqlCommand = New SqlCommand
    '        cm.CommandText = "UpdProcs.updSecurityModel"

    '        DoInsertUpdateChild(cm)

    '      End Using

    '    End Sub

    '    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

    '      SecurityGroupList.Update()
    '      UserList.Update()
    '      MarkOld()

    '    End Sub


    '#End If

    '#End Region

    '#End Region


  End Class


End Namespace