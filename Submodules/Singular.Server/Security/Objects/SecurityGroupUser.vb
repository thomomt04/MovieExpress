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
  Public Class SecurityGroupUser
    Inherits SingularBusinessBase(Of SecurityGroupUser)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared SecurityGroupUserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SecurityGroupUserID, "Security Group User", 0)
    ''' <Summary>
    ''' Gets the Security Group User value
    ''' </Summary>
    <Display(Name:="Security Group User", Description:="", AutoGenerateField:=False), Key()> _
    Public ReadOnly Property SecurityGroupUserID() As Integer
      Get
        Return GetProperty(SecurityGroupUserIDProperty)
      End Get
    End Property

    Public Shared SecurityGroupIDProperty As PropertyInfo(Of Nullable(Of Integer)) = RegisterProperty(Of Nullable(Of Integer))(Function(c) c.SecurityGroupID, "Security Group", CType(Nothing, Nullable(Of Integer)))
    ''' <Summary>
    ''' Gets and sets the Security Group value
    ''' </Summary>
    <Display(Name:="Security Group", Description:=""),
    Required(ErrorMessage:="Security Group required"),
    Singular.DataAnnotations.DropDownField(GetType(SecurityGroupList), DataAnnotations.BindingContext.BindingLocationType.ParentContentControlViewModel, "SecurityGroupList"),
    Singular.DataAnnotations.DropDownWeb(GetType(SecurityGroupList), UniqueInList:=True, DropDownColumns:={"SecurityGroup", "Description"}, FilterMethodName:="Singular.FilterSecurityGroupList", Source:=Singular.DataAnnotations.DropDownWeb.SourceType.All + DataAnnotations.DropDownWeb.SourceType.IgnoreMissing),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "SecurityGroup")> _
    Public Property SecurityGroupID() As Nullable(Of Integer)
      Get
        Return GetProperty(SecurityGroupIDProperty)
      End Get
      Set(ByVal Value As Nullable(Of Integer))
        SetProperty(SecurityGroupIDProperty, Value)
      End Set
    End Property

    Public Shared UserIDProperty As PropertyInfo(Of Nullable(Of Integer)) = RegisterProperty(Of Nullable(Of Integer))(Function(c) c.UserID, "User", CType(Nothing, Nullable(Of Integer)))
    ''' <Summary>
    ''' Gets the User value
    ''' </Summary>
    <Display(Name:="User", Description:="", AutoGenerateField:=False)>
    Public ReadOnly Property UserID() As Nullable(Of Integer)
      Get
        Return GetProperty(UserIDProperty)
      End Get
    End Property

    <Display(AutoGenerateField:=False), Browsable(True)>
    Public Property Selected As Boolean


#End Region

#Region " Methods "

    Public Function GetParent() As IUser

      Return CType(CType(Me.Parent, SecurityGroupUserList).Parent, IUser)

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SecurityGroupUserIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.SecurityGroupID Is Nothing Then
        If Me.IsNew Then
          Return "New Security Group User"
        Else
          Return "Blank Security Group User"
        End If
      Else
#If SILVERLIGHT Then
        Return Me.SecurityGroupUserID.ToString
#Else
        If Singular.Settings.CurrentPlatform = CommonDataPlatform.Windows Then
          Return CommonData.GetListProperty(Of Singular.Security.SecurityModel).SecurityGroupList.GetItem(Me.SecurityGroupID).ToString
        Else
          Return "Security Group User"
        End If
#End If
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

    Public Shared Function NewSecurityGroupUser() As SecurityGroupUser

      Return DataPortal.CreateChild(Of SecurityGroupUser)()

    End Function

    Public Sub New()

      MarkAsChild()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then



#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetSecurityGroupUser(ByVal dr As SafeDataReader) As SecurityGroupUser

      Dim s As New SecurityGroupUser()
      s.Fetch(dr)
      Return s

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(SecurityGroupUserIDProperty, .GetInt32(0))
          LoadProperty(SecurityGroupIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
          LoadProperty(UserIDProperty, Singular.Misc.ZeroNothing(.GetInt32(2)))
        End With
      End Using

      If SecurityGroupUserList.UseSelectedField Then
        Me.Selected = True
      End If

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insSecurityGroupUser"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updSecurityGroupUser"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      If MyBase.IsDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramSecurityGroupUserID As SqlParameter = .Parameters.Add("@SecurityGroupUserID", SqlDbType.Int)
          paramSecurityGroupUserID.Value = GetProperty(SecurityGroupUserIDProperty)
          If Me.IsNew Then
            paramSecurityGroupUserID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@SecurityGroupID", GetProperty(SecurityGroupIDProperty))
          .Parameters.AddWithValue("@UserID", Me.GetParent.UserID)

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(SecurityGroupUserIDProperty, paramSecurityGroupUserID.Value)
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
        cm.CommandText = "DelProcs.delSecurityGroupUser"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@SecurityGroupUserID", GetProperty(SecurityGroupUserIDProperty))
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