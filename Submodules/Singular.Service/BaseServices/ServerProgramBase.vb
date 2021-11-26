

Public MustInherit Class ServerProgramBase

  Protected mVersion As String = "Not Set"
  Protected mName As String

  Public Sub New(ByVal Name As String)

    mName = Name
    LogFile.WriteLogEntry("Schedule Name : " & Name)

  End Sub

  Public MustOverride ReadOnly Property ServerProgramTypeID() As Integer

  Protected mServerProgramType As ServerProgramType

  Public ReadOnly Property ServerProgramType() As ServerProgramType
    Get
      If mServerProgramType Is Nothing Then
        RefetchServiceInfo()
      End If

      Return mServerProgramType
    End Get
  End Property

  ''' <summary>
  ''' Refetches the ServerProgramType object from the database. Returns false if the fetch fails.
  ''' </summary>
  Protected Function RefetchServiceInfo() As Boolean
    Try
      mServerProgramType = ServerProgramTypeList.GetServerProgramTypeList(New ServerProgramTypeList.Criteria() With {.ServiceProgramTypeID = Me.ServerProgramTypeID})(0)
      LogFile.WriteLogEntry("Getting Server Program Type : " & mServerProgramType.ServerProgramType)
      Return True
    Catch ex As Exception

      WriteProgress("Error re-fetching service info", ex)

      LogFile.WriteLogEntry("Error Getting Server Program Type with ID " & Me.ServerProgramTypeID & ": " & Singular.Debug.RecurseExceptionMessage(ex))
      mServerProgramType = Singular.Service.ServerProgramType.NewServerProgramType()
      mServerProgramType.ServerProgramType = "Unknown"
      Return False
    End Try
  End Function

  Public Overridable ReadOnly Property Version() As String
    Get
      Return mVersion
    End Get
  End Property

  Public ReadOnly Property Name() As String
    Get
      Return mName
    End Get
  End Property

  Public Sub SetVersion(ByVal Version As String)
    mVersion = Version
  End Sub

  Public MustOverride Sub Start()

  Public MustOverride Sub [Stop]()

#Region " Logging "

  Public Enum ProgressType
    Undefined = 0
    Success = 1
    Warning = 2
    Failure = 3
  End Enum

  Private mLastProgress As ServerProgramProgress = Nothing

  Public ReadOnly Property LastProgress() As ServerProgramProgress
    Get
      Return mLastProgress
    End Get
  End Property

  Public Sub WriteProgress(Progress As String, ex As Exception)
    If ex Is Nothing Then
      WriteProgress(Progress)
    Else
      WriteProgress(Progress & ": " & Singular.Debug.RecurseExceptionMessage(ex))
    End If
  End Sub

  Public Sub WriteProgress(Progress As String)
    Me.WriteProgress(Progress, ProgressType.Undefined)
  End Sub

  Public Overridable Function WriteProgress(ByVal Progress As String, ByVal ProgressType As ProgressType) As Boolean

    Try
      ' LogFile.WriteLogEntry("Connection String: " & Settings.ConnectionString)
      mLastProgress = ServerProgramProgress.NewServerProgramProgress(Me.ServerProgramTypeID, Progress, Me.Version, ProgressType)
      If mLastProgress.IsValid Then
        mLastProgress.Save()
      Else
        If Singular.Debug.InDebugMode Then
          Throw New Exception("Cannot save progress: " & mLastProgress.GetErrorsAsString())
        End If
      End If
      LogFile.WriteLogEntry("Progress: " & Progress)

      Return True
    Catch ex As Exception
      SqlConnection.ClearAllPools()

      LogFile.WriteLogEntry("Error updating progress: " & Singular.Debug.RecurseExceptionMessage(ex) & vbCrLf & "(Progress: " & Progress & ")")
      Return False
      If Singular.Debug.InDebugMode Then
        Throw New Exception("Error updating progress: " & Singular.Debug.RecurseExceptionMessage(ex))
      End If
    End Try

  End Function

  Public Sub UpdateProgress(ByVal Progress As String, ByVal ProgressType As ProgressType)

    Try
      mLastProgress.Progress = Progress
      mLastProgress.ProgressTypeID = ProgressType
      If mLastProgress.IsValid Then
        mLastProgress.Save()
      Else
        If Singular.Debug.InDebugMode Then
          Throw New Exception("Cannot save progress: " & mLastProgress.GetErrorsAsString())
        End If
      End If
      LogFile.WriteLogEntry("Progress: " & Progress)
    Catch ex As Exception
      SqlConnection.ClearAllPools()

      LogFile.WriteLogEntry("Error updating progress: " & Singular.Debug.RecurseExceptionMessage(ex) & vbCrLf & "Progress: " & Progress)
      If Singular.Debug.InDebugMode Then
        Throw New Exception("Error updating progress: " & Singular.Debug.RecurseExceptionMessage(ex))
      End If
    End Try

  End Sub

  Public Overridable Sub WriteProgressDetail(ByVal ProgressDetail As String, ByVal ProgressType As ProgressType)

    If mLastProgress Is Nothing Then
      WriteProgress(ProgressDetail, ProgressType)
    Else
      Try
        mLastProgress.AddProgressDetail(ProgressDetail, ProgressType)
        If mLastProgress.IsValid Then
          mLastProgress.Save()
        Else
          If Singular.Debug.InDebugMode Then
            Throw New Exception("Cannot save progress: " & mLastProgress.GetErrorsAsString())
          End If
        End If
      Catch ex As Exception

        If Singular.Debug.InDebugMode Then
          Throw New Exception("Error updating progress: " & Singular.Debug.RecurseExceptionMessage(ex))
        End If
      End Try
    End If

    LogFile.WriteLogEntry("Progress Detail: " & ProgressDetail)

  End Sub

#End Region

  Protected Friend Overridable Sub SystemSettingsChanged(SettingsType As Type, Instance As Singular.SystemSettings.ISettingsSection)

  End Sub

  Protected Friend Overridable Sub ServiceInfoChanged(NewInfo As Singular.Service.ServerProgramType)

    If NewInfo.ActiveInd <> mServerProgramType.ActiveInd Then
      WriteProgress("Program now marked " & If(NewInfo.ActiveInd, "active", "inactive"))
    End If

    mServerProgramType = NewInfo

    WriteProgress("Service info updated")

  End Sub

#Region " Job Manager "

  Private _JobManager As JobManager

  Protected Overridable ReadOnly Property JobManager As JobManager
    Get
      If _JobManager Is Nothing Then
        _JobManager = New JobManager(Me)
      End If
      Return _JobManager
    End Get
  End Property

#End Region

End Class

