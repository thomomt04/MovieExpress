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
  Public Class SecurityGroupListBase(Of T As SecurityGroupListBase(Of T, C), C As SecurityGroupBase(Of C))
    Inherits SingularBusinessListBase(Of T, C)
    Implements ISecurityGroupList

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

    Public Function GetItem(ByVal SecurityGroupID As Integer) As C

      For Each child As C In Me
        If child.SecurityGroupID = SecurityGroupID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Security Groups"

    End Function

    Public Function GetSecurityGroupRole(ByVal SecurityGroupRoleID As Integer) As SecurityGroupRole

      Dim obj As SecurityGroupRole = Nothing
      For Each parent As C In Me
        obj = parent.SecurityGroupRoleList.GetItem(SecurityGroupRoleID)
        If obj IsNot Nothing Then
          Return obj
        End If
      Next
      Return Nothing

    End Function

    Private Class Criteria
      Inherits Csla.CriteriaBase(Of Criteria)

      Public Values() As Object

    End Class

#If Silverlight = False Then
    Public Shared Function GetGeneric(ParamArray Values() As Object) As T
      Return DataPortal.Fetch(Of T)(New Criteria With {.Values = Values})
    End Function
#End If

#End Region

#Region " Data Access "

#Region " Silverlight "

#If SILVERLIGHT Then

#Else

#End Region

#Region " .Net Data Access "

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Dim sg = Activator.CreateInstance(Of C)()
        sg.Fetch(sdr)
        Me.Add(sg)
      End While
      Me.RaiseListChangedEvents = True

      LoadChildren(sdr)

    End Sub

    ' The warning below cannot be corrected because the base class is different for WPF and SL projects
    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      'Fetch the Security Group Objects.
      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getSecurityGroupList"
            AddParameters(cm, crit.Values)
            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Protected Overridable Sub AddParameters(cm As SqlCommand, Values() As Object)

    End Sub

    Protected Sub LoadChildren(sdr As SafeDataReader)

      Dim parent As C = Nothing
      If sdr.NextResult Then
        While sdr.Read
          If IsNothing(parent) OrElse parent.SecurityGroupID <> sdr.GetInt32(1) Then
            parent = Me.GetItem(sdr.GetInt32(1))
          End If
          parent.SecurityGroupRoleList.RaiseListChangedEvents = False
          parent.SecurityGroupRoleList.Add(SecurityGroupRole.GetSecurityGroupRole(sdr))
          parent.SecurityGroupRoleList.RaiseListChangedEvents = True
        End While
      End If

      For Each child As C In Me
        child.CheckRules()
        For Each SecurityGroupRole As SecurityGroupRole In child.SecurityGroupRoleList
          SecurityGroupRole.CheckRules()
        Next
      Next

    End Sub

    Friend Sub Update() Implements ISecurityGroupList.Update

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As C In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As C In Me
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

      UpdateTransactional(AddressOf Update)

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace