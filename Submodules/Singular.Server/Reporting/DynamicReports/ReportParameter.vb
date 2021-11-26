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
  Public Class ReportParameter
    Inherits SingularBusinessBase(Of ReportParameter)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared DynamicReportParameterIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DynamicReportParameterID, "Dynamic Report Parameter", 0)
    ''' <summary>
    ''' Gets the Dynamic Report Parameter value
    ''' </summary>
    <Display(AutoGenerateField:=False), Key>
    Public ReadOnly Property DynamicReportParameterID() As Integer
      Get
        Return GetProperty(DynamicReportParameterIDProperty)
      End Get
    End Property

    Public Shared DynamicReportIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.DynamicReportID, "Dynamic Report", Nothing)
    ''' <summary>
    ''' Gets the Dynamic Report value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property DynamicReportID() As Integer?
      Get
        Return GetProperty(DynamicReportIDProperty)
      End Get
    End Property

    Public Shared ParameterNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ParameterName, "Parameter Name", "")
    ''' <summary>
    ''' Gets and sets the Parameter Name value
    ''' </summary>
    <Display(Name:="Parameter Name", Description:="The name of the parameter as it is in the stored procedure."),
    StringLength(50, ErrorMessage:="Parameter Name cannot be more than 50 characters")>
    Public Property ParameterName() As String
      Get
        Return GetProperty(ParameterNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(ParameterNameProperty, Value)
      End Set
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

    Public Shared DropDownSourceProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DropDownSource, "Drop Down Source", "")
    ''' <summary>
    ''' Gets and sets the Drop Down Source value
    ''' </summary>
    <Display(Name:="Drop Down Source", Description:=""),
    StringLength(100, ErrorMessage:="Drop Down Source cannot be more than 100 characters"),
    Singular.DataAnnotations.DropDownWeb("ClientData.DropDownList", ValueMember:="Name", DisplayMember:="Name")>
    Public Property DropDownSource() As String
      Get
        Return GetProperty(DropDownSourceProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(DropDownSourceProperty, Value)
      End Set
    End Property

    Public Shared RequiredIndProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.RequiredInd, "Required", False)
    ''' <summary>
    ''' Gets and sets the Required value
    ''' </summary>
    <Display(Name:="Required?", Description:="")>
    Public Property RequiredInd() As Boolean
      Get
        Return GetProperty(RequiredIndProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(RequiredIndProperty, Value)
      End Set
    End Property

    Public Shared DataTypeProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DataType, "Data Type", "")
    ''' <summary>
    ''' Gets and sets the Data Type value
    ''' </summary>
    <Display(Name:="Data Type", Description:="Main Data Type of the parameter. Must be Int, VarChar, DateTime, Bit"),
    StringLength(50, ErrorMessage:="Data Type cannot be more than 50 characters")>
    Public Property DataType() As String
      Get
        Return GetProperty(DataTypeProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(DataTypeProperty, Value)
      End Set
    End Property

    Public Shared VisibleProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.Visible, "Visible?", False)
    ''' <summary>
    ''' Gets and sets the Required value
    ''' </summary>
    <Display(Name:="Visible?", Description:="Will this parameter be shown on the report criteria?")>
    Public Property Visible() As Boolean
      Get
        Return GetProperty(VisibleProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(VisibleProperty, Value)
      End Set
    End Property

    Public Shared DefaultValueProperty As PropertyInfo(Of String) = RegisterSProperty(Of String)(Function(c) c.DefaultValue, "") _
                                                                    .AddSetExpression(Function(c) c.DefaultType, Function(c) If(c.DefaultValue = "", DefaultValueType.StoredProc, DefaultValueType.Specify))
    ''' <summary>
    ''' Gets and sets the Required value
    ''' </summary>
    <Display(Name:="Default Value", Description:="")>
    Public Property DefaultValue() As String
      Get
        Return GetProperty(DefaultValueProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(DefaultValueProperty, Value)
      End Set
    End Property

    Public Shared DefinedDefaultValueProperty As PropertyInfo(Of SpecialDefaultValue) = RegisterProperty(Of SpecialDefaultValue)(Function(c) c.DefinedDefaultValue, "Default Value", SpecialDefaultValue.None)
    ''' <summary>
    ''' Gets and sets the Required value
    ''' </summary>
    <Display(Name:="Default Value", Description:=""),
    Singular.DataAnnotations.DropDownWeb("GetDefinedDefaults($data)", ValueMember:="Val", DisplayMember:="Text")>
    Public Property DefinedDefaultValue() As SpecialDefaultValue
      Get
        Return GetProperty(DefinedDefaultValueProperty)
      End Get
      Set(ByVal Value As SpecialDefaultValue)
        SetProperty(DefinedDefaultValueProperty, Value)
      End Set
    End Property

    Public Enum DefaultValueType
      StoredProc = 1
      Specify = 2
      Special = 3
    End Enum

    Public Enum SpecialDefaultValue
      None = 0
      CurrentUser = 100
      Today = 200
      Yesterday = 201
      ThisMonthStart = 202
      ThisMonthEnd = 203
      LastMonthStart = 204
      LastMonthEnd = 205
    End Enum

    Public Shared DefaultTypeProperty As PropertyInfo(Of DefaultValueType) = RegisterProperty(Of DefaultValueType)(Function(c) c.DefaultType, "Default Type", DefaultValueType.StoredProc)
    ''' <summary>
    ''' Gets and sets the Required value
    ''' </summary>
    <Display(Name:="Default Type", Description:=""), Singular.DataAnnotations.DropDownWeb(GetType(DefaultValueType))>
    Public Property DefaultType() As DefaultValueType
      Get
        Return GetProperty(DefaultTypeProperty)
      End Get
      Set(ByVal Value As DefaultValueType)
        SetProperty(DefaultTypeProperty, Value)
      End Set
    End Property

    Private mParameterDefaultValue As Object
    <Display(Name:="DB Default")>
    Public ReadOnly Property ParameterDefaultValue As Object
      Get
        Return mParameterDefaultValue
      End Get
    End Property

    Private mMainDataType As Singular.Reflection.SMemberInfo.MainType
    Public ReadOnly Property ParamDataType As String
      Get
        Return mMainDataType.ToString.ToLower()(0)
      End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property MainDataType As Singular.Reflection.SMemberInfo.MainType
      Get
        Return mMainDataType
      End Get
    End Property

#End Region

#Region "  Methods  "

    Public Function GetDefaultValue() As Object

      If DefaultType = DefaultValueType.StoredProc Then

        Return mParameterDefaultValue

      ElseIf DefaultType = DefaultValueType.Specify Then

        Return DefaultValue

      Else

        Select Case DefinedDefaultValue
          Case SpecialDefaultValue.CurrentUser
            If Singular.Security.CurrentIdentity IsNot Nothing Then
              Return Singular.Security.CurrentIdentity.UserID
            Else
              Return Nothing
            End If

          Case SpecialDefaultValue.Today
            Return Now.Date
          Case SpecialDefaultValue.Yesterday
            Return Now.Date.AddDays(-1)
          Case SpecialDefaultValue.ThisMonthStart
            Return Singular.Dates.DateMonthStart(Now)
          Case SpecialDefaultValue.ThisMonthEnd
            Return Singular.Dates.DateMonthEnd(Now)
          Case SpecialDefaultValue.LastMonthStart
            Return Singular.Dates.DateMonthStart(Now.AddMonths(-1))
          Case SpecialDefaultValue.LastMonthEnd
            Return Singular.Dates.DateWeekEnd(Now.AddMonths(-1))
        End Select

      End If

      Return Nothing

    End Function

    Public Function GetParent() As Report

      Return CType(CType(Me.Parent, ReportParameterList).Parent, Report)

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(DynamicReportParameterIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.DisplayName.Length = 0 Then
        If Me.IsNew Then
          Return String.Format("New {0}", "Report Parameter")
        Else
          Return String.Format("Blank {0}", "Report Parameter")
        End If
      Else
        Return Me.DisplayName
      End If

    End Function

#End Region

#End Region

#Region "  Validation Rules  "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

      AddWebRule(VisibleProperty, Function(c) c.RequiredInd AndAlso Not c.Visible, Function(c) "Parameter cannot be hidden and required.")
      AddWebRule(DefaultTypeProperty, Function(c) c.DefaultType = DefaultValueType.Specify AndAlso Singular.Misc.IsNullNothing(c.DefaultValue), Function(c) "Default value required for this default type.")
      AddWebRule(DefaultTypeProperty, Function(c) c.DefaultType = DefaultValueType.Special AndAlso Singular.Misc.IsNullNothing(c.DefinedDefaultValue), Function(c) "Default value required for this default type.")

    End Sub

#End Region

#Region "  Data Access & Factory Methods  "

#Region "  Common  "

    Public Shared Function NewReportParameter() As ReportParameter

      Return DataPortal.CreateChild(Of ReportParameter)()

    End Function

    Public Sub New()

      MarkAsChild()

    End Sub

#End Region

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Friend Shared Function GetReportParameter(dr As SafeDataReader, AutoGenereated As Boolean) As ReportParameter

      Dim r As New ReportParameter()
      r.Fetch(dr, AutoGenereated)
      Return r

    End Function

    Protected Sub Fetch(sdr As SafeDataReader, AutoGenereated As Boolean)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(DynamicReportParameterIDProperty, .GetInt32(0))
          LoadProperty(DynamicReportIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))

          ParameterName = .GetString(2).Trim.Replace("@", "")
          LoadProperty(DisplayNameProperty, If(AutoGenereated, Singular.Strings.Readable(.GetString(3).Trim), .GetString(3).Trim))
          LoadProperty(DropDownSourceProperty, .GetString(4))
          LoadProperty(RequiredIndProperty, .GetBoolean(5))
          LoadProperty(DataTypeProperty, .GetString(6))

          mParameterDefaultValue = .GetString(7).Trim

          LoadProperty(VisibleProperty, .GetBoolean(8))
          LoadProperty(DefaultTypeProperty, .GetInt32(10))

          If DefaultType = DefaultValueType.Special Then
            LoadProperty(DefinedDefaultValueProperty, .GetValue(9))
          Else
            LoadProperty(DefaultValueProperty, .GetValue(9))
          End If

          'If auto generated
          If DynamicReportParameterID = 0 Then

            If ParameterName = "StartDate" Then
              DefaultType = DefaultValueType.Special
              DefinedDefaultValue = SpecialDefaultValue.ThisMonthStart
            End If

            If ParameterName = "EndDate" Then
              DefaultType = DefaultValueType.Special
              DefinedDefaultValue = SpecialDefaultValue.ThisMonthEnd
            End If

            If ParameterName = "UserID" Then
              DefaultType = DefaultValueType.Special
              DefinedDefaultValue = SpecialDefaultValue.CurrentUser
              Visible = False
            End If

            Dim ddd = Settings.DropDowns.GetDefault(ParameterName)
            If ddd IsNot Nothing Then
              DropDownSource = ddd.Name
              If DisplayName.EndsWith(" ID") Then
                DisplayName = DisplayName.Substring(0, DisplayName.Length - 3)
              End If
            End If

          End If

          Select Case .GetString(6)
            Case "date", "datetime"
              mMainDataType = Reflection.SMemberInfo.MainType.Date
            Case "varchar", "nvarchar"
              mMainDataType = Reflection.SMemberInfo.MainType.String
            Case "int", "float", "money"
              mMainDataType = Reflection.SMemberInfo.MainType.Number
            Case "bit"
              mMainDataType = Reflection.SMemberInfo.MainType.Boolean
            Case Else
              mMainDataType = Reflection.SMemberInfo.MainType.String
          End Select

        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insDynamicReportParameter"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Friend Sub Update()

      If DynamicReportParameterID <= 0 Then
        MarkNew()
        Insert()
      Else

        Using cm As SqlCommand = New SqlCommand
          cm.CommandText = "UpdProcs.updDynamicReportParameter"

          DoInsertUpdateChild(cm)
        End Using
      End If

    End Sub

    Protected Overrides Sub InsertUpdate(cm As SqlCommand)

      If Me.IsSelfDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramDynamicReportParameterID As SqlParameter = .Parameters.Add("@DynamicReportParameterID", SqlDbType.Int)
          paramDynamicReportParameterID.Value = GetProperty(DynamicReportParameterIDProperty)
          If Me.IsNew Then
            paramDynamicReportParameterID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@DynamicReportID", GetParent.DynamicReportID)
          .Parameters.AddWithValue("@ParameterName", GetProperty(ParameterNameProperty))
          .Parameters.AddWithValue("@DisplayName", GetProperty(DisplayNameProperty))
          .Parameters.AddWithValue("@DropDownSource", GetProperty(DropDownSourceProperty))
          .Parameters.AddWithValue("@RequiredInd", GetProperty(RequiredIndProperty))
          .Parameters.AddWithValue("@DataType", GetProperty(DataTypeProperty))
          .Parameters.AddWithValue("@Visible", GetProperty(VisibleProperty))
          If DefaultType = DefaultValueType.Special Then
            .Parameters.AddWithValue("@DefaultValue", DefinedDefaultValue)
          Else
            .Parameters.AddWithValue("@DefaultValue", DefaultValue)
          End If

          .Parameters.AddWithValue("@DefaultType", GetProperty(DefaultTypeProperty))

          .ExecuteNonQuery()

          If Me.IsNew Then
            LoadProperty(DynamicReportParameterIDProperty, paramDynamicReportParameterID.Value)
          End If
          ' update child objects
          ' mChildList.Update()
          MarkOld()
        End With
      Else
      End If

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delDynamicReportParameter"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@DynamicReportParameterID", GetProperty(DynamicReportParameterIDProperty))
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