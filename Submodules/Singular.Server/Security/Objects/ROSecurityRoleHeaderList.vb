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
  Public Class ROSecurityRoleHeaderList
    Inherits SingularReadOnlyListBase(Of ROSecurityRoleHeaderList, ROSecurityRoleHeader)

#Region " Business Methods "

#If SILVERLIGHT Then

    Public Function AddItem(Section As String, Role As String, Description As String) As ROSecurityRole

      Dim sr = GetItem(Section, Role)
      If sr Is Nothing Then
        Dim Header As ROSecurityRoleHeader = Nothing
        For Each child In Me
          If child.SectionName = Section Then
            Header = child
            Exit For
          End If
        Next

        If Header Is Nothing Then
          Header = ROSecurityRoleHeader.NewROSecurityRoleHeader(Section)
        End If

        sr = ROSecurityRole.CreateSecurityRole(Section, Role, Description)

        Header.ROSecurityRoleList.Add(sr)
      End If
      Return sr

    End Function


#End If

    
    Public Function GetItem(Section As String, Role As String) As ROSecurityRole

      For Each child As ROSecurityRoleHeader In Me
        If child.SectionName = Section Then
          For Each sr In child.ROSecurityRoleList
            If sr.SecurityRole = Role Then
              Return sr
            End If
          Next
        End If
      Next
      Return Nothing

    End Function

    Public Function GetItem(SectionName As String) As ROSecurityRoleHeader

      For Each child As ROSecurityRoleHeader In Me
        If child.SectionName = SectionName Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "s"

    End Function

    Public Function GetROSecurityRole(ByVal SecurityRoleID As Integer) As ROSecurityRole

      Dim obj As ROSecurityRole = Nothing
      For Each parent As ROSecurityRoleHeader In Me
        obj = parent.ROSecurityRoleList.GetItem(SecurityRoleID)
        If obj IsNot Nothing Then
          Return obj
        End If
      Next
      Return Nothing

    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewROSecurityRoleHeaderList() As ROSecurityRoleHeaderList

      Return New ROSecurityRoleHeaderList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then


    Public Shared Sub BeginGetROSecurityRoleHeaderList(ByVal CallBack As EventHandler(Of DataPortalResult(Of ROSecurityRoleHeaderList)))

      Dim dp As New DataPortal(Of ROSecurityRoleHeaderList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria)

    End Sub

    Public Shared Function NewROSecurityRoleHeaderList(OriginalList As ROSecurityRoleHeaderList, SecurityGroup As ISecurityGroup) As ROSecurityRoleHeaderList

      Return New ROSecurityRoleHeaderList(OriginalList, SecurityGroup)

    End Function

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

    Public Sub New(OriginalList As ROSecurityRoleHeaderList, SecurityGroup As ISecurityGroup)

      For Each srh As ROSecurityRoleHeader In OriginalList

        Dim Newsrh As ROSecurityRoleHeader = ROSecurityRoleHeader.NewROSecurityRoleHeader(srh.SectionName)
        Me.IsReadOnly = False
        Me.AllowNew = True
        Me.AllowEdit = True
        Me.Add(Newsrh)

        For Each sr As ROSecurityRole In srh.ROSecurityRoleList

          Newsrh.ROSecurityRoleList.Add2(ROSecurityRole.NewROSecurityRole(sr.SecurityRoleID, sr.SectionName, sr.SecurityRole, sr.Description, SecurityGroup))

        Next

      Next

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetROSecurityRoleHeaderList() As ROSecurityRoleHeaderList

      Return DataPortal.Fetch(Of ROSecurityRoleHeaderList)(New Criteria)

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Me.Add(ROSecurityRoleHeader.GetROSecurityRoleHeader(sdr))
      End While
      Me.IsReadOnly = True
      Me.RaiseListChangedEvents = True

      If sdr.NextResult Then

        Dim Parent As ROSecurityRoleHeader = Nothing

        While sdr.Read

          If Parent Is Nothing OrElse Parent.SectionName <> sdr.GetString(1) Then
            Parent = GetItem(sdr.GetString(1))
          End If
          'Parent.ROSecurityRoleList.AllowAdd()
          Parent.ROSecurityRoleList.Add(ROSecurityRole.GetROSecurityRole(sdr))
          'Parent.ROSecurityRoleList.DisallowAdd()

        End While

      End If

    End Sub

		Public Shared Property GetProcedureName As String = "[GetProcs].[getROSecurityRoleList]"
		Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
						cm.CommandText = GetProcedureName
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