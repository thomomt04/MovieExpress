Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If
Imports Singular.DataAnnotations

Namespace Service


  <Serializable()> _
  Public Class ServerProgramType
    Inherits SingularBusinessBase(Of ServerProgramType)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared ServerProgramTypeIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ServerProgramTypeID, "Server Program Type", 0)
    ''' <Summary>
    ''' Gets the Server Program Type value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Server Program Type", Description:=""), Key()> _
    Public ReadOnly Property ServerProgramTypeID() As Integer
      Get
        Return GetProperty(ServerProgramTypeIDProperty)
      End Get
    End Property

    Public Shared ServerProgramTypeProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ServerProgramType, "Server Program Type", "")
    ''' <Summary>
    ''' Gets and sets the Server Program Type value
    ''' </Summary>
    <Display(Name:="Program Name", Description:=""),
    StringLength(50, ErrorMessage:="Server Program Type cannot be more than 50 characters")> _
    Public Property ServerProgramType() As String
      Get
        Return GetProperty(ServerProgramTypeProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(ServerProgramTypeProperty, Value)
      End Set
    End Property

    Public Shared InfoProperty As PropertyInfo(Of IServerProgramInfo) = RegisterProperty(Of IServerProgramInfo)(Function(c) c.Info)
#If SILVERLIGHT Then
    <Display(AutoGenerateField:=False)>
    Public Property Info() As IServerProgramInfo
      Get
        Return GetProperty(InfoProperty)
      End Get
      Set(value As IServerProgramInfo)
        If value IsNot Nothing Then
          While value.GetEditLevel < Me.GetEditLevel
            CType(value, Object).BeginEdit()
          End While
        End If

        SetProperty(InfoProperty, value)
      End Set
    End Property
#Else

    Public Property Info() As IServerProgramInfo
      Get
        Return GetProperty(InfoProperty)
      End Get
      Set(value As IServerProgramInfo)
        If value IsNot Nothing Then
          While value.GetEditLevel < Me.GetEditLevel
            CType(value, Object).BeginEdit()
          End While
        End If

        SetProperty(InfoProperty, value)
      End Set
    End Property

    Public Shared InfoStringProperty As PropertyInfo(Of String) = RegisterReadOnlyProperty(Of String)(Function(c) c.InfoString,
                                                                                                      "return ServiceHelper.getScheduleInfo(self.Info(), self.ScheduleTypeID());")

    <Display(Name:="Schedule"), Singular.DataAnnotations.TextField(True, True), Singular.DataAnnotations.ClientOnlyNoData>
    Public ReadOnly Property InfoString As String
      Get
        Return GetProperty(InfoStringProperty)
      End Get
    End Property

    <InitialDataOnly, RawDataOnly>
    Public Property ServiceMenuItems As New List(Of ServiceMenuInfo)

#End If

    Public Shared ActiveIndProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.ActiveInd, "Active", False)
    ''' <Summary>
    ''' Gets and sets the Active value
    ''' </Summary>
    <Display(Name:="Active?", Description:=""),
    Required(ErrorMessage:="Active required")> _
    Public Property ActiveInd() As Boolean
      Get
        Return GetProperty(ActiveIndProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(ActiveIndProperty, Value)
      End Set
    End Property

    Public Property ScheduleTypeID As Integer

    Public Enum ScheduleType
      Scheduled = 1
      AlwaysRunning = 2
    End Enum

#End Region

#Region " Methods "

#If SILVERLIGHT Then

    Protected Overrides Sub OnDeserialized(context As System.Runtime.Serialization.StreamingContext)
      MyBase.OnDeserialized(context)

      If Me.Info Is Nothing Then
        ' it is null, so create a new one
        If Singular.Service.GetDefaultInfoFunction IsNot Nothing Then
          Me.Info = Singular.Service.GetDefaultInfoFunction.Invoke(Me.ServerProgramTypeID, Me.ServerProgramType)
        End If
      End If

    End Sub

#End If

    'Public Sub SetInfoFromFile(ByVal FileName As String)

    '  Dim bmp As System.Drawing.Bitmap = System.Drawing.Bitmap.FromFile(FileName)
    '  SetProperty(InfoProperty, bmp)

    'End Sub

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(ServerProgramTypeIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.ServerProgramType.Length = 0 Then
        If Me.IsNew Then
          Return "New Server Program Type"
        Else
          Return "Blank Server Program Type"
        End If
      Else
        Return Me.ServerProgramType
      End If

    End Function

#End Region

#End Region

#Region " Child Lists "

    '#If SILVERLIGHT Then

    '    Public Shared ROScheduleProgressListProperty As PropertyInfo(Of ROScheduleProgressList) = RegisterProperty(Of ROScheduleProgressList)(Function(c) c.ROScheduleProgressList, "Schedule Progress List")

    '    <Display(AutoGenerateField:=False)> _
    '    Public Property ROScheduleProgressList() As ROScheduleProgressList
    '      Get
    '        If GetProperty(ROScheduleProgressListProperty) Is Nothing Then
    '          LoadProperty(ROScheduleProgressListProperty, Scheduling.ROScheduleProgressList.NewROScheduleProgressList())
    '          MarkBusy()

    '          ROScheduleProgressList.BeginGetROScheduleProgressList(Me.ScheduleInfoID, Sub(o, e)
    '                                                                                     If e.Error IsNot Nothing Then
    '                                                                                       Throw e.Error
    '                                                                                     End If
    '                                                                                     Me.ROScheduleProgressList = e.Object
    '                                                                                     OnPropertyChanged(ROScheduleProgressListProperty)
    '                                                                                     MarkIdle()
    '                                                                                     OnPropertyChanged("IsBusy")
    '                                                                                   End Sub)
    '        End If
    '        Return GetProperty(ROScheduleProgressListProperty)
    '      End Get
    '      Private Set(ByVal value As ROScheduleProgressList)
    '        LoadProperty(ROScheduleProgressListProperty, value)
    '      End Set
    '    End Property

    '#End If



#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Shared Function NewServerProgramType() As ServerProgramType

      Return DataPortal.CreateChild(Of ServerProgramType)()

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

    Friend Shared Function GetServerProgramType(ByVal dr As SafeDataReader) As ServerProgramType

      Dim s As New ServerProgramType()
      s.Fetch(dr)
      Return s

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(ServerProgramTypeIDProperty, .GetInt32(0))
          LoadProperty(ServerProgramTypeProperty, .GetString(1))
          LoadProperty(InfoProperty, Singular.Compression.CompressionUtility.DeserialiseObject(.GetValue(2)))
          If Info IsNot Nothing Then
            'Create a new guid in case this data was copied from another record in the table.
            Dim oi As New Singular.CSLALib.ObjectIterator(False, False, False, Sub(obj As ISingularBase, ctx As AbortableActionContext)
                                                                                 obj.Guid = System.Guid.NewGuid
                                                                               End Sub)
            oi.RecurseObjectGraphAndPerformAction(Info)
          End If
          LoadProperty(ActiveIndProperty, .GetBoolean(3))

          If sdr.FieldCount > 4 Then
            ScheduleTypeID = .GetInt32(4)
          Else
            ScheduleTypeID = ScheduleType.Scheduled
          End If
        End With
      End Using

      MarkAsChild()
      MarkOld()
      If Info IsNot Nothing Then Info.MarkOld()
      '   BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insServerProgramType"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updServerProgramType"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      If Me.IsSelfDirty OrElse (Me.Info IsNot Nothing AndAlso Me.Info.IsDirty) Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramServerProgramTypeID As SqlParameter = .Parameters.Add("@ServerProgramTypeID", SqlDbType.Int)
          paramServerProgramTypeID.Value = GetProperty(ServerProgramTypeIDProperty)
          If Me.IsNew Then
            paramServerProgramTypeID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@ServerProgramType", GetProperty(ServerProgramTypeProperty))

          If Info IsNot Nothing Then
            .Parameters.AddWithValue("@Info", Singular.Compression.CompressionUtility.SerialiseObject(Info))
          End If

          .Parameters.AddWithValue("@ActiveInd", GetProperty(ActiveIndProperty))

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(ServerProgramTypeIDProperty, paramServerProgramTypeID.Value)
          End If
          ' update child objects
          ' mChildList.Update()
          MarkOld()
          If Info IsNot Nothing Then Me.Info.MarkOld()
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
        cm.CommandText = "DelProcs.delServerProgramType"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@ServerProgramTypeID", GetProperty(ServerProgramTypeIDProperty))
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