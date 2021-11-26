''' <summary>
''' Ensures only one instance of the service is running at a time.
''' </summary>
Public Class MultiInstanceHandler

  ''' <summary>
  ''' Enable multiple instance checking. This requires a command proc called CmdProcs.cmdCheckServerProgramLock found in the blank DB.
  ''' </summary>
  Public Property Enabled As Boolean = False

  ''' <summary>
  ''' The Instance name of this service. Defaults to MachineName.
  ''' </summary>
  Public Property InstanceName As String

  ''' <summary>
  ''' Use this if you have more than one set of services connecting to the database.
  ''' The multi instance checker will only allow one service to be active per group.
  ''' </summary>
  Public Property ServiceGroupID As Integer?

  ''' <summary>
  ''' Time in seconds before checking again if the active service is still running.
  ''' </summary>
  Public Property CheckTimeoutInSeconds As Integer = 300

  Private _ServiceBase As System.ServiceProcess.ServiceBase
  Private _MainProgram As ServerProgramBase
  Private _StartupMethod As Action
  Private _CheckTimer As System.Timers.Timer
  Private _IsActive As Boolean = False

  Friend Sub New(ServiceBase As System.ServiceProcess.ServiceBase)

    InstanceName = Environment.MachineName
    _ServiceBase = ServiceBase
  End Sub

  Friend Sub Start(MainProgram As ServerProgramBase, StartupMethod As Action)

    If Enabled Then

      If CheckTimeoutInSeconds < 60 Then
        Throw New Exception("Check Timeout cannot be less than 60 seconds")
      End If

      _MainProgram = MainProgram
      _StartupMethod = StartupMethod

      _CheckTimer = New System.Timers.Timer(CheckTimeoutInSeconds * 1000)
      AddHandler _CheckTimer.Elapsed, AddressOf CheckActiveService
      _CheckTimer.Start()
      _CheckTimer.AutoReset = True

      CheckActiveService()

      If Not _IsActive Then
        MainProgram.WriteProgress(String.Format("Service on {0} is not active, programs will only start when active service is stopped.", InstanceName))
      End If

    Else
      MainProgram.WriteProgress("MultiInstanceHandler is not enabled on this service.")
      StartupMethod()
    End If

  End Sub

  Private Sub CheckActiveService()

    Dim ParamNames As New List(Of String) From {"@InstanceName", "@Interval"}
    Dim ParamValues As New List(Of String) From {InstanceName, CheckTimeoutInSeconds}

    If ServiceGroupID IsNot Nothing Then
      ParamNames.Add("@ServiceGroupID")
      ParamValues.Add(ServiceGroupID)
    End If

    Dim ReturnVal = Singular.CommandProc.GetDataValue("CmdProcs.cmdCheckServerProgramLock", ParamNames.ToArray, ParamValues.ToArray)

    If ReturnVal IsNot Nothing AndAlso Not _IsActive Then
      'This service is now active
      _IsActive = True
      _MainProgram.WriteProgress(String.Format("Active service is on {0}", InstanceName))

      'The active service must report that it is active slightly faster than the inactive services are checking for activity.
      _CheckTimer.Interval = (CheckTimeoutInSeconds - 15) * 1000

      'Start the programs
      _StartupMethod()

    ElseIf ReturnVal Is Nothing AndAlso _IsActive Then
      'If this service was active, and now isnt. This should only happen if connection is lost to the DB and then restored later.

      Try
        _MainProgram.WriteProgress(String.Format("Service no longer active on {0}", InstanceName))
      Catch
      End Try

      _ServiceBase.Stop()

    End If

  End Sub

  Public Sub [Stop]()
    If _CheckTimer IsNot Nothing AndAlso _CheckTimer.Enabled Then _CheckTimer.Stop()
  End Sub


End Class
