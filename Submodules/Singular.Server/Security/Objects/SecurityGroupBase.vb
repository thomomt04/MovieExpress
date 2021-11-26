Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Singular.Localisation
Imports Csla.Core

#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Security

  <Serializable()> _
  Public MustInherit Class SecurityGroupBase(Of T As SecurityGroupBase(Of T))
    Inherits SingularBusinessBase(Of T)
    Implements ISecurityGroup

#Region " Properties and Methods "

#Region " Properties "

    Public Shared SecurityGroupIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SecurityGroupID, "Security Group", 0)
    ''' <Summary>
    ''' Gets the Security Group value
    ''' </Summary>
    <Display(Name:="Security Group", Description:="", AutoGenerateField:=False), Browsable(True), Key()> _
    Public ReadOnly Property SecurityGroupID() As Integer Implements ISecurityGroup.SecurityGroupID
      Get
        Return GetProperty(SecurityGroupIDProperty)
      End Get
    End Property

    Public Shared SecurityGroupProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SecurityGroup, "Security Group", "")
    ''' <Summary>
    ''' Gets and sets the Security Group value
    ''' </Summary>
    <Display(Description:="The Name of the Group.", Name:="Security Group"),
    Required(ErrorMessage:="Security Group required"),
    StringLength(50, ErrorMessage:="Security Group cannot be more than 50 characters"),
     Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "SecurityGroup")> _
    Public Property SecurityGroup() As String
      Get
        Return GetProperty(SecurityGroupProperty)
      End Get
      Set(ByVal Value As String)
        If Me.Description = "" OrElse Me.Description = Me.SecurityGroup Then
          Me.Description = Value
        End If
        SetProperty(SecurityGroupProperty, Value)
      End Set
    End Property

    Public Shared DescriptionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Description, "Description", "")
    ''' <Summary>
    ''' Gets and sets the Description value
    ''' </Summary>
    <Display(Name:="Description", Description:="A Short Description of the Group."),
    Required(ErrorMessage:="Description required"),
    StringLength(255, ErrorMessage:="Description cannot be more than 255 characters"),
    DataAnnotations.TextField(WordWrap:=True),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "Description")> _
    Public Property Description() As String
      Get
        Return GetProperty(DescriptionProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(DescriptionProperty, Value)
      End Set
    End Property

#End Region

#Region " Child Lists "

    Public Shared SecurityGroupRoleListProperty As PropertyInfo(Of SecurityGroupRoleList) = RegisterProperty(Of SecurityGroupRoleList)(Function(c) c.SecurityGroupRoleList, "Assigned Roles")

    <Display(AutoGenerateField:=False), Browsable(True)> _
    Public ReadOnly Property SecurityGroupRoleList() As SecurityGroupRoleList Implements ISecurityGroup.SecurityGroupRoleList
      Get
        If Not FieldManager.FieldExists(SecurityGroupRoleListProperty) Then
          LoadProperty(SecurityGroupRoleListProperty, SecurityGroupRoleList.NewSecurityGroupRoleList())
        End If
        Return GetProperty(SecurityGroupRoleListProperty)
      End Get
    End Property

#If SILVERLIGHT Then

    <NonSerialized()> Private mROSecurityRoleList As ROSecurityRoleHeaderList
    <Display(AutoGenerateField:=False)> _
    Public ReadOnly Property ROSecurityRoleHeaderList As ROSecurityRoleHeaderList Implements ISecurityGroup.ROSecurityRoleHeaderList
      Get
        If mROSecurityRoleList Is Nothing Then
          mROSecurityRoleList = ROSecurityRoleHeaderList.NewROSecurityRoleHeaderList(CType(CType(Parent, Object).Parent, ISecurityModel).ROSecurityRoleHeaderList, Me)
        End If
        Return mROSecurityRoleList
      End Get
    End Property

    Private Sub SecurityGroup_ChildChanged(sender As Object, e As EventArgs)

    End Sub

    Protected Overrides Sub OnChildChanged(e As Csla.Core.ChildChangedEventArgs)
      MyBase.OnChildChanged(e)

      'If Not mChangingRole Then
      '  mROSecurityRoleList = Nothing
      'End If
    End Sub

#End If



#End Region

#Region " Methods "

    Public Overrides Sub CanDelete(CallBack As System.EventHandler(Of CanDeleteArgs))

      If Me.SecurityGroup = "Administrator" OrElse Me.SecurityGroup = "Administrators" Then
        CallBack.Invoke(Me, New CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CantDelete, String.Format("{0} group cannot be deleted", Me.SecurityGroup)))
      Else
        MyBase.CanDelete(CallBack)
      End If

    End Sub


    Public Overrides Function CanEditField(FieldName As String, Args As Singular.CanEditFieldArgs) As Boolean

      If Me.SecurityGroup = "Administrator" OrElse Me.SecurityGroup = "Administrators" Then
        Args.Reason = "Cannot update, " & String.Format("{0} group cannot be deleted", Me.SecurityGroup)
        Return False
      End If
      Return MyBase.CanEditField(FieldName, Args)
    End Function
     

    Public Function GrantRole(ByVal Role As ROSecurityRole) As SecurityGroupRole

      Dim sgr As SecurityGroupRole = SecurityGroupRole.NewSecurityGroupRole(Role)
      sgr.SecurityGroupID = Me.SecurityGroupID
      Me.SecurityGroupRoleList.Add(sgr)
      Return sgr

    End Function

    Function CloneSecurityGroup() As Singular.Security.SecurityGroup

      Dim sg As New SecurityGroup
      For Each utsr In Me.SecurityGroupRoleList
        Dim cloneUTSR As SecurityGroupRole = SecurityGroupRole.NewSecurityGroupRole(utsr)
        sg.SecurityGroupRoleList.Add(cloneUTSR)
      Next
      sg.SecurityGroup = Me.SecurityGroup & " - Copy"
      Return sg

    End Function

    Public Sub SetTempSecurityGroupID(ByVal SecurityGroupID As Integer)

      Me.SetProperty(SecurityGroupIDProperty, SecurityGroupID)

    End Sub

#If SILVERLIGHT Then
#Else

    Public Sub PopulateSecurityRoleList(ByRef ROSecurityRoleHeaderList As Singular.Security.ROSecurityRoleHeaderList)

      For Each srh As Singular.Security.ROSecurityRoleHeader In ROSecurityRoleHeaderList
        For Each sr As Singular.Security.ROSecurityRole In srh.ROSecurityRoleList

          sr.SelectedInd = False
          Dim sgr = SecurityGroupRoleList.Find(sr.SecurityRoleID)
          If sgr IsNot Nothing AndAlso sgr.IsSelected Then
            sr.SelectedInd = True
          End If

        Next
      Next

    End Sub

    Public Sub UpdateFromSecurityRoleList(ByRef ROSecurityRoleHeaderList As Singular.Security.ROSecurityRoleHeaderList)

      For Each srh As Singular.Security.ROSecurityRoleHeader In ROSecurityRoleHeaderList
        For Each sr As Singular.Security.ROSecurityRole In srh.ROSecurityRoleList

          Dim sgr = SecurityGroupRoleList.Find(sr.SecurityRoleID)
          If sgr Is Nothing Then
            If sr.SelectedInd Then
              Dim sgrNew = New Singular.Security.SecurityGroupRole
              sgrNew.SecurityRoleID = sr.SecurityRoleID
              sgrNew.IsSelected = True
              SecurityGroupRoleList.Add(sgrNew)
            End If
          Else
            sgr.IsSelected = sr.SelectedInd
          End If

        Next
      Next

    End Sub

    Public Shared ToStringField As Csla.PropertyInfo(Of String) = RegisterReadOnlyProperty(Of String)(Function(c) c.ToString, Function(c) c.SecurityGroup)

#End If

    Private mChangingRole As Boolean = False

    Public Sub SetChangingRole(ByVal Value As Boolean) Implements ISecurityGroup.SetChangingRole

      mChangingRole = Value

    End Sub

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SecurityGroupIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.SecurityGroup.Length = 0 Then
        If Me.IsNew Then
          Return "New Security Group"
        Else
          Return "Blank Security Group"
        End If
      Else
        Return Me.SecurityGroup
      End If

    End Function

    Protected Overrides ReadOnly Property TableReferencesToIgnore() As String()
      Get
        Return New String() {"SecurityGroupRoles"}
      End Get
    End Property

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

      ' MarkAsChild()
      BusinessRules.CheckRules()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then


#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Protected Friend Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(SecurityGroupIDProperty, .GetInt32(0))
          LoadProperty(SecurityGroupProperty, .GetString(1))
          LoadProperty(DescriptionProperty, .GetString(2))
        End With

        FetchExtraFields(sdr, 3)
      End Using

      '  MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Protected Overridable Sub FetchExtraFields(sdr As SafeDataReader, StartIndex As Integer)

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insSecurityGroup"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updSecurityGroup"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      If MyBase.IsDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramSecurityGroupID As SqlParameter = .Parameters.Add("@SecurityGroupID", SqlDbType.Int)
          paramSecurityGroupID.Value = GetProperty(SecurityGroupIDProperty)
          If Me.IsNew Then
            paramSecurityGroupID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@SecurityGroup", GetProperty(SecurityGroupProperty))
          .Parameters.AddWithValue("@Description", GetProperty(DescriptionProperty))

          AddParameters(cm)

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(SecurityGroupIDProperty, paramSecurityGroupID.Value)
          End If
          ' update child objects
          UpdateChildren()
          MarkOld()
        End With
      Else
        ' update child objects
        UpdateChildren()
      End If

    End Sub

    Protected Overridable Sub AddParameters(cm As SqlCommand)

    End Sub

    Protected Overridable Sub UpdateChildren()

      Me.SecurityGroupRoleList.Update()

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delSecurityGroup"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@SecurityGroupID", GetProperty(SecurityGroupIDProperty))
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