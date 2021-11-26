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
  Public Class SecurityGroupRole
    Inherits SingularBusinessBase(Of SecurityGroupRole)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared SecurityGroupRoleIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SecurityGroupRoleID, "Security Group Role", 0)
    ''' <Summary>
    ''' Gets the Security Group Role value
    ''' </Summary>
    <Display(Name:="Security Group Role", Description:=""), Key()> _
    Public ReadOnly Property SecurityGroupRoleID() As Integer
      Get
        Return GetProperty(SecurityGroupRoleIDProperty)
      End Get
    End Property

    Public Shared SecurityGroupIDProperty As PropertyInfo(Of Nullable(Of Integer)) = RegisterProperty(Of Nullable(Of Integer))(Function(c) c.SecurityGroupID, "Security Group", CType(Nothing, Nullable(Of Integer)))
    ''' <Summary>
    ''' Gets the Security Group value
    ''' </Summary>
    <Display(Name:="Security Group", Description:=""),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "SecurityGroup")> _
    Public Property SecurityGroupID() As Nullable(Of Integer)
      Get
        Return GetProperty(SecurityGroupIDProperty)
      End Get
      Friend Set(ByVal value As Nullable(Of Integer))
        SetProperty(SecurityGroupIDProperty, value)
      End Set
    End Property

    Public Shared SecurityRoleIDProperty As PropertyInfo(Of Nullable(Of Integer)) = RegisterProperty(Of Nullable(Of Integer))(Function(c) c.SecurityRoleID, "Security Role", CType(Nothing, Nullable(Of Integer)))
    ''' <Summary>
    ''' Gets and sets the Security Role value
    ''' </Summary>
    <Display(Name:="Security Role", Description:=""),
    Required(ErrorMessage:="Security Role required"),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "SecurityRole")> _
    Public Property SecurityRoleID() As Nullable(Of Integer)
      Get
        Return GetProperty(SecurityRoleIDProperty)
      End Get
      Set(ByVal Value As Nullable(Of Integer))
        SetProperty(SecurityRoleIDProperty, Value)
      End Set
    End Property

    Public Shared IsSelectedProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.IsSelected, "IsSelected", True)
    ''' <Summary>
    ''' Gets and sets the Security Role value
    ''' </Summary>
    Public Property IsSelected() As Boolean
      Get
        Return GetProperty(IsSelectedProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(IsSelectedProperty, Value)
      End Set
    End Property

#End Region

#Region " Methods "

    Shared Function NewSecurityGroupRole(ByVal FromSecurityGroupRole As SecurityGroupRole) As SecurityGroupRole

      Dim sgr As New SecurityGroupRole()
      sgr.SecurityRoleID = sgr.SecurityRoleID
      Return sgr

    End Function


    Shared Function NewSecurityGroupRole(ByVal FromROSecurityRole As ROSecurityRole) As SecurityGroupRole

      Dim sgr As New SecurityGroupRole()
      sgr.SecurityRoleID = FromROSecurityRole.SecurityRoleID
      Return sgr

    End Function

    Public Function GetParent() As ISecurityGroup

      Return CType(CType(Me.Parent, SecurityGroupRoleList).Parent, ISecurityGroup)

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SecurityGroupRoleIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.SecurityGroupID.ToString.Length = 0 Then
        If Me.IsNew Then
          Return "New Security Group Role"
        Else
          Return "Blank Security Group Role"
        End If
      Else
        Return Me.SecurityGroupID.ToString
      End If

    End Function

#End Region

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Sub New()

      MarkAsChild()
      BusinessRules.CheckRules()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewSecurityGroupRole() As SecurityGroupRole

      Return New SecurityGroupRole()

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetSecurityGroupRole(ByVal dr As SafeDataReader) As SecurityGroupRole

      Dim s As New SecurityGroupRole()
      s.Fetch(dr)
      Return s

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(SecurityGroupRoleIDProperty, .GetInt32(0))
          LoadProperty(SecurityGroupIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
          LoadProperty(SecurityRoleIDProperty, Singular.Misc.ZeroNothing(.GetInt32(2)))
        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insSecurityGroupRole"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updSecurityGroupRole"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      'Dont save it if its new and De-Selected or Old and Selected.
      If MyBase.IsDirty AndAlso Not (IsNew And Not IsSelected) AndAlso Not (IsSelected And Not IsNew) Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramSecurityGroupRoleID As SqlParameter = .Parameters.Add("@SecurityGroupRoleID", SqlDbType.Int)
          paramSecurityGroupRoleID.Value = GetProperty(SecurityGroupRoleIDProperty)
          If Me.IsNew Then
            paramSecurityGroupRoleID.Direction = ParameterDirection.Output
          End If

          If CType(Parent, SecurityGroupRoleList).Parent IsNot Nothing Then
            .Parameters.AddWithValue("@SecurityGroupID", CType(CType(Parent, SecurityGroupRoleList).Parent, ISecurityGroup).SecurityGroupID)
          Else
            .Parameters.AddWithValue("@SecurityGroupID", GetProperty(SecurityGroupIDProperty))
          End If
          .Parameters.AddWithValue("@SecurityRoleID", GetProperty(SecurityRoleIDProperty))
          .Parameters.AddWithValue("@SelectedInd", GetProperty(IsSelectedProperty))

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(SecurityGroupRoleIDProperty, paramSecurityGroupRoleID.Value)
          End If
          ' update child objects
          ' mChildList.Update()
          MarkOld()
        End With
      Else
        ' update child objects
        ' mChildList.Update()
      End If

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delSecurityGroupRole"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@SecurityGroupRoleID", GetProperty(SecurityGroupRoleIDProperty))
        DoDeleteChild(cm)
      End Using

    End Sub

    Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)

      If Me.IsNew Then Exit Sub

      With cm
        .ExecuteNonQuery()
      End With
      MarkNew()

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace