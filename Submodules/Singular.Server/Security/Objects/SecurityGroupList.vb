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
  Public Class SecurityGroupList
    Inherits SecurityGroupListBase(Of SecurityGroupList, SecurityGroup)

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewSecurityGroupList() As SecurityGroupList

      Return New SecurityGroupList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Sub BeginGetSecurityGroupList(ByVal CallBack As EventHandler(Of DataPortalResult(Of SecurityGroupList)))

      Dim dp As New DataPortal(Of SecurityGroupList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria)

    End Sub

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetSecurityGroupList() As SecurityGroupList

      Return DataPortal.Fetch(Of SecurityGroupList)(New Criteria)

    End Function

    Public Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(SecurityGroup.GetSecurityGroup(sdr))
      End While
      Me.RaiseListChangedEvents = True

      MyBase.LoadChildren(sdr)

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
            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace