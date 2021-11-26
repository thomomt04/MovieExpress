
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Errorlog

  <Serializable()> _
  Public Class ErrorLog
    Inherits SingularBusinessBase(Of ErrorLog)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared ErrorLogIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ErrorLogID, "Error Log", 0)
    ''' <Summary>
    ''' Gets the Log value
    ''' </Summary>
    <Display(Name:="Error Log", Description:="")> _
    Public ReadOnly Property ErrorLogID() As Integer
      Get
        Return GetProperty(ErrorLogIDProperty)
      End Get
    End Property

    Public Shared ErrorDescriptionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ErrorDescription, "Error Description", "")
    ''' <Summary>
    ''' Gets and sets the Log Location value
    ''' </Summary>
    <Display(Name:="Error Description", Description:=""),
    StringLength(2000, ErrorMessage:="Error Description cannot be more than 100 characters")> _
    Public Property ErrorDescription() As String
      Get
        Return GetProperty(ErrorDescriptionProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(ErrorDescriptionProperty, Value)
      End Set
    End Property

    Public Shared StackTraceProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.StackTrace, "Stack Trace", "")
    ''' <Summary>
    ''' Gets and sets the Exception Detail value
    ''' </Summary>
    <Display(Name:="Stack Trace", Description:="")> _
    Public Property StackTrace() As String
      Get
        Return GetProperty(StackTraceProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(StackTraceProperty, Value)
      End Set
    End Property

    Public Shared ProgramVersionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ProgramVersion, "Version No", "")
    ''' <Summary>
    ''' Gets and sets the Version No value
    ''' </Summary>
    <Display(Name:="Program Version No", Description:=""),
    StringLength(20, ErrorMessage:="Version No cannot be more than 10 characters")> _
    Public Property ProgramVersion() As String
      Get
        Return GetProperty(ProgramVersionProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(ProgramVersionProperty, Value)
      End Set
    End Property

    Public Shared CreatedDateTimeProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.CreatedDateTime, "Created Date Time", New SmartDate(Now()))
    ''' <Summary>
    ''' Gets the Created Date Time value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Created Date Time", Description:="")> _
    Public ReadOnly Property CreatedDateTime() As SmartDate
      Get
        Return GetProperty(CreatedDateTimeProperty)
      End Get
    End Property

    Public Shared CreatedByProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.CreatedBy, "Created By", CInt(Csla.ApplicationContext.ClientContext("UserID")))
    ''' <Summary>
    ''' Gets the Created By value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Created By", Description:="")> _
    Public ReadOnly Property CreatedBy() As Integer
      Get
        Return GetProperty(CreatedByProperty)
      End Get
    End Property

    Private mIsEmpty As Boolean = False
    <Browsable(False)> _
    Public ReadOnly Property IsEmpty() As Boolean
      Get
        Return mIsEmpty
      End Get
    End Property

#End Region

#Region " Methods "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(ErrorLogIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If GetProperty(ErrorDescriptionProperty).Length = 0 Then
        If Me.IsNew Then
          Return "New Error Log"
        Else
          Return "Error Log"
        End If
      Else
        Return GetProperty(ErrorDescriptionProperty)
      End If

    End Function

#End Region

#End Region

#Region " Validation Rules "

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "


    Public Sub New()

      ' BusinessRules.CheckRules()

    End Sub

    <Serializable()> _
    Private Class Criteria

      Public ErrorLogID As Integer

      Public Sub New(ByVal ErrorLogID As Integer)

        Me.ErrorLogID = ErrorLogID

      End Sub

    End Class

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewErrorLog() As Errorlog

      Return DataPortal.CreateChild(Of Errorlog)()

    End Function

    Public Shared Sub BeginGetErrorLog(ByVal ErrorLogID As Integer, ByVal CallBack As EventHandler(Of DataPortalResult(Of ErrorLog)))

      Dim dp As New DataPortal(Of ErrorLog)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New SingleCriteria(Of ErrorLog, Integer)(ErrorLogID))

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetErrorLog(ByVal dr As SafeDataReader) As ErrorLog

      Dim l As New ErrorLog()
      l.Fetch(dr)
      Return l

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(ErrorLogIDProperty, .GetInt32(0))
          LoadProperty(ErrorDescriptionProperty, .GetString(1))
          LoadProperty(StackTraceProperty, .GetString(2))
          LoadProperty(ProgramVersionProperty, .GetString(3))
          LoadProperty(CreatedDateTimeProperty, .GetSmartDate(4))
          LoadProperty(CreatedByProperty, .GetInt32(5))
        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Protected Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getErrorLog"
            cm.Parameters.AddWithValue("@ErrorLogID", crit.ErrorLogID)

            Using sdr As New SafeDataReader(cm.ExecuteReader)
              If sdr.Read() Then
                Fetch(sdr)
              End If
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insErrorLog"

        DoInsertUpdateParent(cm)

      End Using

    End Sub

    Protected Overrides Sub DataPortal_Insert()

      Me.Insert()

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updErrorLog"

        DoInsertUpdateParent(cm)

      End Using

    End Sub

    Protected Overrides Sub DataPortal_Update()

      Me.Update()

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      If MyBase.IsDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramErrorLogID As SqlParameter = .Parameters.Add("@ErrorLogID", SqlDbType.Int)
          paramErrorLogID.Value = GetProperty(ErrorLogIDProperty)
          If Me.IsNew Then
            paramErrorLogID.Direction = ParameterDirection.Output
          End If
   
          .Parameters.AddWithValue("@ErrorDescription", GetProperty(ErrorDescriptionProperty))
          .Parameters.AddWithValue("@StackTrace", GetProperty(ErrorDescriptionProperty))
          .Parameters.AddWithValue("@ProgramVersion", GetProperty(ErrorDescriptionProperty))
          .Parameters.AddWithValue("@CreatedBy", GetProperty(CreatedByProperty))

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(ErrorLogIDProperty, paramErrorLogID.Value)
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
        cm.CommandText = "DelProcs.delErrorLog"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@ErrorLogID", GetProperty(ErrorLogIDProperty))
        DoDeleteParent(cm)
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