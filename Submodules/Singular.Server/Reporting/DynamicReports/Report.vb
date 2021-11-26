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
  Public Class Report
    Inherits SingularBusinessBase(Of Report)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared DynamicReportIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DynamicReportID, "Dynamic Report", 0)
    ''' <summary>
    ''' Gets the Dynamic Report value
    ''' </summary>
    <Display(AutoGenerateField:=False), Key>
    Public ReadOnly Property DynamicReportID() As Integer
      Get
        Return GetProperty(DynamicReportIDProperty)
      End Get
    End Property

    Public Shared DynamicReportGroupIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.DynamicReportGroupID, "Dynamic Report Group", Nothing)
    ''' <summary>
    ''' Gets the Dynamic Report Group value
    ''' </summary>
    Public ReadOnly Property DynamicReportGroupID() As Integer?
      Get
        Return GetProperty(DynamicReportGroupIDProperty)
      End Get
    End Property

    Public Shared DisplayNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DisplayName, "Display Name", "")
    ''' <summary>
    ''' Gets and sets the Display Name value
    ''' </summary>
    <Display(Name:="Display Name", Description:=""),
    StringLength(50, ErrorMessage:="Display Name cannot be more than 50 characters")>
    Public Property DisplayName() As String
      Get
        Return GetProperty(DisplayNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(DisplayNameProperty, Value)
      End Set
    End Property

    Public Shared DescriptionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Description, "Description", "")
    ''' <summary>
    ''' Gets and sets the Description value
    ''' </summary>
    <Display(Name:="Description", Description:="Description of the report."),
    StringLength(200, ErrorMessage:="Description cannot be more than 200 characters"), Singular.DataAnnotations.TextField(True, , True, NoOfLines:=2)>
    Public Property Description() As String
      Get
        Return GetProperty(DescriptionProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(DescriptionProperty, Value)
      End Set
    End Property

    Public Shared StoredProcedureNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.StoredProcedureName, "Stored Procedure Name", "")
    ''' <summary>
    ''' Gets and sets the Stored Procedure Name value
    ''' </summary>
    <Display(Name:="Source Name", Description:="The stored procedure name of the report that must be run including schema name. Blank if Query Text is used."),
    StringLength(200, ErrorMessage:="Stored Procedure Name cannot be more than 200 characters"), Required(ErrorMessage:="Stored Procedure Name is required"),
    Singular.DataAnnotations.DropDownWeb(GetType(ROReportSourceList), DropDownType:=Singular.DataAnnotations.DropDownWeb.SelectType.FindScreen,
      LookupMember:="StoredProcedureName", ValueMember:="SourceName", DisplayMember:="SourceName")>
    Public Property StoredProcedureName() As String
      Get
        Return GetProperty(StoredProcedureNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(StoredProcedureNameProperty, Value)
      End Set
    End Property

    Public Shared QueryTextProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.QueryText, "Query Text", "")
    ''' <summary>
    ''' Gets and sets the Query Text value
    ''' </summary>
    <Display(Name:="Query Text", Description:="The query to run for the report data (if stored proc name is blank).")>
    Public Property QueryText() As String
      Get
        Return GetProperty(QueryTextProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(QueryTextProperty, Value)
      End Set
    End Property

    Public Shared SortOrderProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SortOrder, "Sort Order", 0)
    ''' <summary>
    ''' Gets and sets the Sort Order value
    ''' </summary>
    <Display(Name:="Sort Order", Description:="The order the reports will appear in the group.")>
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

    Public Shared AutoGeneratedIndProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.AutoGeneratedInd, "Auto Generated", False)
    ''' <summary>
    ''' Gets the Auto Generated value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property AutoGeneratedInd() As Boolean
      Get
        Return GetProperty(AutoGeneratedIndProperty)
      End Get
    End Property

    <DefaultValue(0), Singular.DataAnnotations.ClientOnly>
    Public Property Stage As Integer = 1

    Private mGroupName As String 'User to allow saving without parent.

#End Region

#Region "  Child Lists  "

    Public Shared ReportParameterListProperty As PropertyInfo(Of ReportParameterList) = RegisterProperty(Of ReportParameterList)(Function(c) c.ReportParameterList, "Report Parameter List")

    Public ReadOnly Property ReportParameterList() As ReportParameterList
      Get
        If GetProperty(ReportParameterListProperty) Is Nothing Then
          LoadProperty(ReportParameterListProperty, Reporting.Dynamic.ReportParameterList.NewReportParameterList())
        End If
        Return GetProperty(ReportParameterListProperty)
      End Get
    End Property

#End Region

#Region "  Methods  "

    Public Sub SetGroupInfo(GroupID As Integer, GroupName As String)
      LoadProperty(DynamicReportGroupIDProperty, GroupID)
      mGroupName = GroupName
    End Sub

    Friend Function GetROParameterList() As ROParameterList
      Dim ParamList As ROParameterList = ROParameterList.NewROParameterList
      For Each Param As ReportParameter In ReportParameterList
        ParamList.Add(ROParameter.NewROParameter(Param))
      Next
      Return ParamList
    End Function

    Friend Shared Function GetGroupName(ProcName As String) As String
      Dim Parts As String() = ProcName.Split({"."}, StringSplitOptions.RemoveEmptyEntries)
      If Parts.Length = 2 Then
        Return Singular.Strings.Readable(Parts(0))
      Else
        Return "Uncategorised"
      End If
    End Function

    Public Function GetParent() As ReportGroup

      Return CType(CType(Me.Parent, ReportList).Parent, ReportGroup)

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(DynamicReportIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.DisplayName.Length = 0 Then
        If Me.IsNew Then
          Return String.Format("New {0}", "Report")
        Else
          Return String.Format("Blank {0}", "Report")
        End If
      Else
        Return Me.DisplayName
      End If

    End Function

    Protected Overrides ReadOnly Property TableReferencesToIgnore() As String()
      Get
        Return New String() {"DynamicReportParameters"}
      End Get
    End Property

    Protected Overrides ReadOnly Property TableName As String
      Get
        Return "DynamicReports"
      End Get
    End Property

#End Region

#End Region

#Region "  Validation Rules  "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

      AddWebRule(DisplayNameProperty, Function(c) c.Stage = 1 AndAlso c.DisplayName = "", Function(c) "Display Name is required")

    End Sub

    Public Overrides Sub CanDelete(CallBack As EventHandler(Of CanDeleteArgs))

      If DynamicReportID < 0 Then
        CallBack(Me, New CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CantDelete, "This report is auto generated."))
      Else
        MyBase.CanDelete(CallBack)
      End If

    End Sub

#End Region

#Region "  Data Access & Factory Methods  "

#Region "  Common  "

    Public Shared Function NewReport() As Report

      Return DataPortal.CreateChild(Of Report)()

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

    Friend Shared Function GetReport(dr As SafeDataReader) As Report

      Dim r As New Report()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      Using BypassPropertyChecks

        With sdr
          LoadProperty(DynamicReportIDProperty, .GetInt32(0))
          LoadProperty(DynamicReportGroupIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))

          Dim mDisplayName As String = .GetString(2)
          Dim mDescription As String = .GetString(3)
          Dim mStoredProcedureParts As String() = .GetString(4).Split(".")
          LoadProperty(QueryTextProperty, .GetString(5))
          LoadProperty(SortOrderProperty, .GetInt32(6))
          LoadProperty(SecurityRoleProperty, .GetString(7))
          LoadProperty(AutoGeneratedIndProperty, .GetBoolean(8))

          If AutoGeneratedInd Then
            'Display Name needs to be fixed.
            If mDisplayName.StartsWith("rpt") Then
              mDisplayName = mDisplayName.Substring(3)
            End If
            mDisplayName = Singular.Strings.Readable(mDisplayName)
            mDescription = mDisplayName
          End If

          LoadProperty(DisplayNameProperty, mDisplayName)
          LoadProperty(DescriptionProperty, mDescription)

          'Add brackets to stored proc name
          Dim mStoredProcName As String = ""
          For i As Integer = 0 To mStoredProcedureParts.Length - 1
            mStoredProcName &= If(i > 0, ".", "") & "[" & mStoredProcedureParts(i) & "]"
          Next
          LoadProperty(StoredProcedureNameProperty, mStoredProcName)

        End With

      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insDynamicReport"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Friend Sub Update()

      If DynamicReportID <= 0 Then
        MarkNew()
        Insert()
      Else

        ' if we're not dirty then don't update the database
        Using cm As SqlCommand = New SqlCommand
          cm.CommandText = "UpdProcs.updDynamicReport"

          DoInsertUpdateChild(cm)
        End Using

      End If

    End Sub

    Protected Overrides Sub InsertUpdate(cm As SqlCommand)

      If Me.IsSelfDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramDynamicReportID As SqlParameter = .Parameters.Add("@DynamicReportID", SqlDbType.Int)
          paramDynamicReportID.Value = GetProperty(DynamicReportIDProperty)
          If Me.IsNew Then
            paramDynamicReportID.Direction = ParameterDirection.Output
          End If
          If mGroupName Is Nothing Then
            .Parameters.AddWithValue("@DynamicReportGroupID", Me.GetParent().DynamicReportGroupID)
          Else
            .Parameters.AddWithValue("@DynamicReportGroupID", DynamicReportGroupID)
          End If
          .Parameters("@DynamicReportGroupID").Direction = ParameterDirection.InputOutput
          .Parameters.AddWithValue("@GroupName", mGroupName)

          .Parameters.AddWithValue("@DisplayName", GetProperty(DisplayNameProperty))
          .Parameters.AddWithValue("@Description", GetProperty(DescriptionProperty))
          .Parameters.AddWithValue("@StoredProcedureName", GetProperty(StoredProcedureNameProperty).Replace("[", "").Replace("]", ""))
          .Parameters.AddWithValue("@QueryText", GetProperty(QueryTextProperty))
          .Parameters.AddWithValue("@SortOrder", GetProperty(SortOrderProperty))

          If SecurityRole <> "" AndAlso Not SecurityRole.Contains(".") Then
            SecurityRole = "Dynamic Reports." & SecurityRole
          End If
          .Parameters.AddWithValue("@SecurityRole", GetProperty(SecurityRoleProperty))

          .ExecuteNonQuery()

          If Me.IsNew Then
            LoadProperty(DynamicReportIDProperty, paramDynamicReportID.Value)
            LoadProperty(DynamicReportGroupIDProperty, .Parameters("@DynamicReportGroupID").Value)
          End If
          ' update child objects
          If GetProperty(ReportParameterListProperty) IsNot Nothing Then
            Me.ReportParameterList.Update()
          End If
          MarkOld()
        End With
      Else
        ' update child objects
        If GetProperty(ReportParameterListProperty) IsNot Nothing Then
          Me.ReportParameterList.Update()
        End If
      End If

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delDynamicReport"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@DynamicReportID", GetProperty(DynamicReportIDProperty))
        DoDeleteChild(cm)
      End Using

    End Sub

    Protected Overrides Sub DeleteFromDB(cm As SqlCommand)

      If Me.IsNew Then Exit Sub

      cm.ExecuteNonQuery()
      MarkNew()

      Singular.Reporting.ProjectReportHierarchy.ResetDynamicReports()

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace