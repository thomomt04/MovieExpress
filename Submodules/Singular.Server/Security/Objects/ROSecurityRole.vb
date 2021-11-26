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
  Public Class ROSecurityRole
    Inherits SingularReadOnlyBase(Of ROSecurityRole)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared SecurityRoleIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SecurityRoleID, "ID")
    ''' <Summary>
    ''' Gets the ID value
    ''' </Summary>
    <Display(Name:="ID", Description:="")> _
    Public ReadOnly Property SecurityRoleID() As Nullable(Of Integer)
      Get
        Return GetProperty(SecurityRoleIDProperty)
      End Get
    End Property

    Public Shared SectionNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SectionName, "Section Name")
    ''' <Summary>
    ''' Gets the Section Name value
    ''' </Summary>
    <Display(Name:="Section Name", Description:=""),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "SectionName"), Browsable(False)> _
    Public ReadOnly Property SectionName() As String
      Get
        Return GetProperty(SectionNameProperty)
      End Get
    End Property

    Public Shared SecurityRoleProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SecurityRole, "Security Role")
    ''' <Summary>
    ''' Gets the Security Role value
    ''' </Summary>
    <Display(Name:="Security Role", Description:=""),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "SecurityRole")> _
    Public ReadOnly Property SecurityRole() As String
      Get
        Return GetProperty(SecurityRoleProperty)
      End Get
    End Property

    Public Shared DescriptionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Description, "Description")
    ''' <Summary>
    ''' Gets the Description value
    ''' </Summary>
    <Display(Name:="Description", Description:=""),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "Description")> _
    Public ReadOnly Property Description() As String
      Get
        Return GetProperty(DescriptionProperty)
      End Get
    End Property

    Private mSelectedInd As Boolean

    Public Property SelectedInd As Boolean
      Get
        Return mSelectedInd
      End Get
      Set(value As Boolean)
        mSelectedInd = value

#If SILVERLIGHT Then

        mSecurityGroup.SetChangingRole(True)

        Dim gr As SecurityGroupRole = mSecurityGroup.SecurityGroupRoleList.Find(SecurityRoleID)

        If mSelectedInd Then
          'Selecting the item

          If gr Is Nothing Then
            'Add an item to the list.
            gr = SecurityGroupRole.NewSecurityGroupRole
            gr.SecurityRoleID = SecurityRoleID
            mSecurityGroup.SecurityGroupRoleList.Add(gr)

          ElseIf Not gr.IsSelected Then
            'If its been deselected after loading from the database, then just select it again.
            gr.IsSelected = True
          End If

        Else
          'Un-Selecting the item

          If gr IsNot Nothing AndAlso gr.IsSelected Then
            'Don't delete the item, just mark it as unselected.
            gr.IsSelected = False
          End If

        End If

        mSecurityGroup.SetChangingRole(False)

#End If
       

        OnPropertyChanged("SelectedInd")
        OnPropertyChanged("Image")
      End Set
    End Property

#If SILVERLIGHT Then

    Public ReadOnly Property Image As Media.Imaging.BitmapImage
      Get
        If SelectedInd Then
          Return Images.LockAccess
        Else
          Return Images.LockNoAccess
        End If
      End Get
    End Property

#End If



#End Region

#Region " Methods "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SecurityRoleIDProperty)

    End Function

    Public Overrides Function ToString() As String

      Return Me.SectionName

    End Function

#End Region

#End Region

#Region " Data Access & Factory Methods "

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewROSecurityRole() As ROSecurityRole

      Return New ROSecurityRole()

    End Function

    Private Sub BeginInsert()

      Dim cmd = CmdInsSecurityRole.NewCmdInsSecurityRole(Me.SectionName, Me.SecurityRole, Me.Description)
      cmd.BeginExecute(Sub(o, e)
                         If e.Error IsNot Nothing Then
                           Throw e.Error
                         End If

                         LoadProperty(SecurityRoleIDProperty, e.Object.SecurityRoleID)
                       End Sub)

    End Sub

    Public Shared Function CreateSecurityRole(Section As String, Role As String, Description As String) As ROSecurityRole

      Dim nsr = NewROSecurityRole(0, Section, Role, Description, Nothing)
      nsr.BeginInsert()
      Return nsr

    End Function

    Private mSecurityGroup As ISecurityGroup

    Friend Shared Function NewROSecurityRole(SecurityRoleID As Integer, SectionName As String, Role As String, Description As String, SecurityGroup As ISecurityGroup) As ROSecurityRole

      Dim newObj As New ROSecurityRole()
      newObj.LoadProperty(SecurityRoleIDProperty, SecurityRoleID)
      newObj.LoadProperty(SectionNameProperty, SectionName)
      newObj.LoadProperty(SecurityRoleProperty, Role)
      newObj.LoadProperty(DescriptionProperty, Description)
      newObj.mSecurityGroup = SecurityGroup

      If SecurityGroup IsNot Nothing Then
        Dim gr As SecurityGroupRole = newObj.mSecurityGroup.SecurityGroupRoleList.Find(SecurityRoleID)
        If gr IsNot Nothing AndAlso gr.IsSelected Then
          newObj.mSelectedInd = True
        Else
          newObj.mSelectedInd = False
        End If
      End If

      Return newObj

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetROSecurityRole(ByVal dr As SafeDataReader) As ROSecurityRole

      Dim r As New ROSecurityRole()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      With sdr
        LoadProperty(SecurityRoleIDProperty, .GetInt32(0))
        LoadProperty(SectionNameProperty, .GetString(1))
        LoadProperty(SecurityRoleProperty, .GetString(2))
        LoadProperty(DescriptionProperty, .GetString(3))
      End With

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace