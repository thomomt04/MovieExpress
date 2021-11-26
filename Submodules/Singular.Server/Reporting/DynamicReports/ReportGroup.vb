' Generated 22 Dec 2014 13:51 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace Reporting.Dynamic

  <Serializable()> _
  Public Class ReportGroup
    Inherits SingularBusinessBase(Of ReportGroup)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared DynamicReportGroupIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DynamicReportGroupID, "Dynamic Report Group", 0)
    ''' <summary>
    ''' Gets the Dynamic Report Group value
    ''' </summary>
    <Display(AutoGenerateField:=False), Key()>
    Public ReadOnly Property DynamicReportGroupID() As Integer
      Get
        Return GetProperty(DynamicReportGroupIDProperty)
      End Get
    End Property

    Public Shared GroupNameProperty As PropertyInfo(Of String) = RegisterSProperty(Of String)(Function(c) c.GroupName, "") _
                                                                 .AddSetExpression("GroupRenamed(self)")

    ''' <summary>
    ''' Gets and sets the Group Name value
    ''' </summary>
    <Display(Name:="Group Name", Description:="Name of group to show in reports menu."), Required(ErrorMessage:="Group Name is required"),
    StringLength(50, ErrorMessage:="Group Name cannot be more than 50 characters")>
    Public Property GroupName() As String
      Get
        Return GetProperty(GroupNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(GroupNameProperty, Value)
      End Set
    End Property

    Public Shared SortOrderProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SortOrder, "Sort Order", 0)
    ''' <summary>
    ''' Gets and sets the Sort Order value
    ''' </summary>
    <Display(Name:="Sort Order", Description:="The order the groups will appear."),
    Required(ErrorMessage:="Sort Order required")>
    Public Property SortOrder() As Integer
      Get
        Return GetProperty(SortOrderProperty)
      End Get
      Set(ByVal Value As Integer)
        SetProperty(SortOrderProperty, Value)
      End Set
    End Property

    Public Shared SecurityRoleProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SecurityRole, "Security Role", "")
    ''' <summary>
    ''' Gets and sets the Security Role value
    ''' </summary>
    <Display(Name:="Security Role", Description:="Only users with this role can see this report."),
    Singular.DataAnnotations.DropDownWeb("ClientData.SecurityRoleList", ValueMember:="SecurityRole", DisplayMember:="SecurityRole", AllowNotInList:=True)>
    Public Property SecurityRole() As String
      Get
        Return GetProperty(SecurityRoleProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(SecurityRoleProperty, Value)
      End Set
    End Property

    <DefaultValue(True)>
    Public Property IsExpanded As Boolean

    Public Property Auto As Boolean

#End Region

#Region "  Child Lists  "

    Public Shared ReportListProperty As PropertyInfo(Of ReportList) = RegisterProperty(Of ReportList)(Function(c) c.ReportList, "Report List")

    Public ReadOnly Property ReportList() As ReportList
      Get
        If GetProperty(ReportListProperty) Is Nothing Then
          LoadProperty(ReportListProperty, Reporting.Dynamic.ReportList.NewReportList())
        End If
        Return GetProperty(ReportListProperty)
      End Get
    End Property

#End Region

#Region "  Methods  "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(DynamicReportGroupIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.GroupName.Length = 0 Then
        If Me.IsNew Then
          Return String.Format("New {0}", "Report Group")
        Else
          Return String.Format("Blank {0}", "Report Group")
        End If
      Else
        Return Me.GroupName
      End If

    End Function

    Protected Overrides ReadOnly Property TableName As String
      Get
        Return "DynamicReportGroups"
      End Get
    End Property

#End Region

#End Region

#Region "  Validation Rules  "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region "  Data Access & Factory Methods  "

#Region "  Common  "

    Friend Shared Function NewReportGroup(GroupName As String) As ReportGroup
      Dim grp As New ReportGroup
      grp.LoadProperty(GroupNameProperty, GroupName)
      Return grp
    End Function

    Public Shared Function NewReportGroup() As ReportGroup

      Return DataPortal.CreateChild(Of ReportGroup)()

    End Function

    Public Sub New()

      'MarkAsChild()

    End Sub

#End Region

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Friend Shared Function GetReportGroup(dr As SafeDataReader) As ReportGroup

      Dim r As New ReportGroup()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(DynamicReportGroupIDProperty, .GetInt32(0))
          LoadProperty(GroupNameProperty, .GetString(1))
          LoadProperty(SortOrderProperty, .GetInt32(2))
          LoadProperty(SecurityRoleProperty, .GetString(3))
        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insDynamicReportGroup"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updDynamicReportGroup"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(cm As SqlCommand)

      If Me.IsSelfDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramDynamicReportGroupID As SqlParameter = .Parameters.Add("@DynamicReportGroupID", SqlDbType.Int)
          paramDynamicReportGroupID.Value = GetProperty(DynamicReportGroupIDProperty)
          If Me.IsNew Then
            paramDynamicReportGroupID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@GroupName", GetProperty(GroupNameProperty))
          .Parameters.AddWithValue("@SortOrder", GetProperty(SortOrderProperty))

          If SecurityRole <> "" AndAlso Not SecurityRole.Contains(".") Then
            SecurityRole = "Dynamic Reports." & SecurityRole
          End If
          .Parameters.AddWithValue("@SecurityRole", GetProperty(SecurityRoleProperty))

          .ExecuteNonQuery()

          If Me.IsNew Then
            LoadProperty(DynamicReportGroupIDProperty, paramDynamicReportGroupID.Value)
          End If
          ' update child objects
          If GetProperty(ReportListProperty) IsNot Nothing Then
            Me.ReportList.Update()
          End If
          MarkOld()
        End With
      Else
        ' update child objects
        If GetProperty(ReportListProperty) IsNot Nothing Then
          Me.ReportList.Update()
        End If
      End If

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delDynamicReportGroup"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@DynamicReportGroupID", GetProperty(DynamicReportGroupIDProperty))
        DoDeleteChild(cm)
      End Using

    End Sub

    Protected Overrides Sub DeleteFromDB(cm As SqlCommand)

      If Me.IsNew Then Exit Sub

      cm.ExecuteNonQuery()
      MarkNew()

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace