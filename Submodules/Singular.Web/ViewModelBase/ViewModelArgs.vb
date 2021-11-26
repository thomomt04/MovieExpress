

Public Class GetDataArgs

  Private mContext As String
  Private mClientArgs As System.Dynamic.DynamicObject
  Private mCriteriaType As Type

  Public ReadOnly Property Context As String
    Get
      Return mContext
    End Get
  End Property

  Public ReadOnly Property ClientArgs As Object
    Get
      Return mClientArgs
    End Get
  End Property

  Public ReadOnly Property CriteriaType As Type
    Get
      Return mCriteriaType
    End Get
  End Property

  Public Sub New(ClientArgs As System.Dynamic.DynamicObject)
    mClientArgs = ClientArgs
    mContext = CType(mClientArgs, Object).Context
    If ClientArgs.GetDynamicMemberNames.Contains("CriteriaTypeCode") Then
      mCriteriaType = Type.GetType(CType(mClientArgs, Object).CriteriaTypeCode) 'Singular.Web.Data.JS.TypeDefinition.GetTypeFromCode(CType(mClientArgs, Object).CriteriaTypeCode)
    End If

    If mContext Is Nothing Then
      mContext = ""
    End If
  End Sub

End Class

Public Class AsyncRuleArgs

  Public Property ObjectType As Type
  Public Property RuleName As String
  Public Property ClientArgs As Object

End Class

Public Class CommandArgs

  Private mIsAjaxPostBack As Boolean
  Private mModelHasBeenUpdated As Boolean
  Private mClientArgs As Object
  Private mReturnDataDynamic As New Singular.Dynamic.DynamicStorage
  Private mReturnDataStatic As Object
  Private mViewModel As IViewModel

  ''' <summary>
  ''' Indicates that the Model has been updated and is in sync with the model on the client.
  ''' </summary>
  Public ReadOnly Property ModelHasBeenUpdated As Boolean
    Get
      Return mModelHasBeenUpdated
    End Get
  End Property

  ''' <summary>
  ''' Indicates that this is an AJAX request, and only data will be sent back, the page will not be re-rendered.
  ''' </summary>
  Public ReadOnly Property IsAjaxPostBack As Boolean
    Get
      Return mIsAjaxPostBack
    End Get
  End Property

  ''' <summary>
  ''' Indicates that the Model must be sent back and updated on the client.
  ''' </summary>
  Public Property TransferModelToClient As Boolean

  ''' <summary>
  ''' The arguments from the browser.
  ''' </summary>
  Public ReadOnly Property ClientArgs As Object
    Get
      Return mClientArgs
    End Get
  End Property

  Friend ReadOnly Property ViewModel As IViewModel
    Get
      Return mViewModel
    End Get
  End Property

  ''' <summary>
  ''' Data to be returned to the browser. Either set this property to a new object, or set properties on this, e.g. ReturnData.RandomProperty = "1234"
  ''' </summary>
  Public Property ReturnData As Object
    Get
      If mReturnDataDynamic Is Nothing Then
        Return mReturnDataStatic
      Else
        Return mReturnDataDynamic
      End If
    End Get
    Set(value As Object)
      mReturnDataStatic = value
      mReturnDataDynamic = Nothing
    End Set
  End Property

  Public Sub New(ClientArgs As Object, IsAjax As Boolean, ModelUpdated As Boolean, TransferModel As Boolean, ViewModel As IViewModel)

    If TypeOf ClientArgs Is System.Dynamic.DynamicObject AndAlso CType(ClientArgs, System.Dynamic.DynamicObject).GetDynamicMemberNames.Contains("TextArgument") Then
      mClientArgs = ClientArgs.TextArgument
    Else
      mClientArgs = ClientArgs
    End If

    mIsAjaxPostBack = IsAjax
    mModelHasBeenUpdated = ModelUpdated
    TransferModelToClient = TransferModel
    mViewModel = ViewModel
  End Sub

  Private mLongActionInstance As AjaxProgress
  Friend ReadOnly Property LongActionInstance As AjaxProgress
    Get
      Return mLongActionInstance
    End Get
  End Property

  Public Sub StartLongAction(CurrentStatus As String, WorkerMethod As Action(Of AjaxProgress))
    mLongActionInstance = New AjaxProgress(CurrentStatus, WorkerMethod,
                                           Sub(rv, context)
                                             context.Response.ContentType = "text/html"
                                             context.Response.Write(ViewModel.GetAjaxResponse(Me))
                                           End Sub)
  End Sub

End Class

Public Class AjaxProgress
  Implements Singular.IUpdatableProgress

  Private Const CachePrefix As String = "AjaxProgress"
  Private Const UpdateLimitMin As Long = 500
  Private Const UpdateLimitMax As Long = 20000

  Private mWorkerGuid As Guid
  Private mCompleteHandler As Action(Of Object, HttpContext)
  Private mReturnValue As Object

  Private mCurrentStep As Integer = 0
  Private mMaxSteps As Integer = 0
  Private mCurrentStatus As String = "Loading..."
  Private mIsFinished As Boolean = False
  Private mLastUpdateTime As Date = Now
  Private mUpdateDataAvailable As Boolean = False
  Private mUpdateTimer As System.Threading.Timer
  Private mCallException As Exception

  Private mStreamData As Object
  Public ReadOnly Property StreamData As Object
    Get
      Return mStreamData
    End Get
  End Property

  ''' <summary>
  ''' Set data to be received by the client. This is meant for incremental updates as the data is available. The Object will be reset, and needs to be re-created / emptied on each call of StreamDataCreate.
  ''' </summary>
  ''' <param name="StreamDataCreate">Function to Create / Clear the data / list. This happens each time data is sent to the client.</param>
  ''' <param name="StreamDataSet">Method to add data to the stream. This may happen multiple times before data is sent to the client.</param>
  Public Sub SetStreamData(Of t)(StreamDataCreate As Func(Of t), StreamDataSet As Action(Of t))
    SyncLock Me
      If mStreamData Is Nothing Then
        mStreamData = StreamDataCreate()
      End If
      StreamDataSet(mStreamData)
    End SyncLock
  End Sub

  ''' <summary>
  ''' Uses stream data to add to a list of progress items.
  ''' </summary>
  Public Sub AddLogItem(Progress As String, Optional LogEntryType As Singular.Misc.LogEntry.LogEntryType = Singular.Misc.LogEntry.LogEntryType.Undefined, Optional UpdateImmediately As Boolean = False)
    SetStreamData(Function()
                    Return New List(Of Singular.Misc.LogEntry)
                  End Function,
                  Sub(pl)
                    pl.Add(New Singular.Misc.LogEntry(Progress, LogEntryType))
                    If UpdateImmediately Then
                      Update()
                    End If
                  End Sub)
  End Sub

  Public Property CurrentStep As Integer Implements IProgress.CurrentStep
    Get
      Return mCurrentStep
    End Get
    Set(value As Integer)
      If value <> mCurrentStep Then
        mCurrentStep = Math.Max(Math.Min(value, mMaxSteps), 0)
      End If
    End Set
  End Property

  Public Property MaxSteps As Integer Implements IProgress.MaxSteps
    Get
      Return mMaxSteps
    End Get
    Set(value As Integer)
      If value <> mMaxSteps Then
        mMaxSteps = Math.Max(value, mCurrentStep)
      End If
    End Set
  End Property

  Public Property CurrentStatus As String Implements IProgress.CurrentStatus
    Get
      Return mCurrentStatus
    End Get
    Set(value As String)
      If mCurrentStatus <> value Then
        mCurrentStatus = value
      End If
    End Set
  End Property

  Public Property Errors As String = "" Implements IProgress.Errors

  Public ReadOnly Property Guid As String
    Get
      Return mWorkerGuid.ToString
    End Get
  End Property

  Public Sub Increment() Implements IProgress.Increment
    CurrentStep += 1
  End Sub

  Private Sub Finish()
    mIsFinished = True
    Update()
  End Sub

  Private mWaiting As Boolean = False
  Private Sub Wait()
    SyncLock Me

      mWaiting = True

      Dim TimeToWait As Long
      Dim MsSinceLastUpdate As Long = Now.Subtract(mLastUpdateTime).TotalMilliseconds

      If mUpdateDataAvailable Then
       
        'Wait at least the min wait time.
        TimeToWait = UpdateLimitMin - MsSinceLastUpdate

      Else

        'There have been no updates, wait the max time
        TimeToWait = UpdateLimitMax - MsSinceLastUpdate
      End If

      If TimeToWait > 0 Then
        System.Threading.Monitor.Wait(Me, TimeToWait)
        'The update method will always release the wait lock, call wait again to make sure the required amount of time has elapsed.
        Wait()
      Else
        mUpdateDataAvailable = False
        mLastUpdateTime = Now
      End If

      mWaiting = False
    End SyncLock
  End Sub

  ''' <summary>
  ''' Updates all values and sends them to the client.
  ''' </summary>
  Public Sub Update()
    SyncLock Me

      mUpdateDataAvailable = True

      If mWaiting Then
        System.Threading.Monitor.Pulse(Me)
      End If

    End SyncLock
  End Sub

  ''' <summary>
  ''' Updates all values and sends them to the client.
  ''' </summary>
  Public Sub Update(StatusText As String, Optional CurrentStep As Integer = -1) Implements IUpdatableProgress.Update
    CurrentStatus = StatusText
    If CurrentStep <> -1 Then
      Me.CurrentStep = CurrentStep
    End If
    Update()
  End Sub

  ''' <summary>
  ''' True if the return value must be wrapped in a Singular.Web.Result
  ''' </summary>
  Public Property WrapResponse As Boolean = True

  Public Sub New(CurrentStatus As String, WorkerMethod As Func(Of AjaxProgress, Object), CompleteHandler As Action(Of Object, HttpContext))
    Setup(CurrentStatus, CompleteHandler)
    WorkerMethod.BeginInvoke(
      Me,
      Sub(ar)
        Try
          mReturnValue = WorkerMethod.EndInvoke(ar)
        Catch ex As Exception
          mCallException = ex
        End Try

        Finish()
      End Sub, Nothing)
  End Sub

  Public Sub New(CurrentStatus As String, WorkerMethod As Action(Of AjaxProgress), CompleteHandler As Action(Of Object, HttpContext))
    Setup(CurrentStatus, CompleteHandler)
    'mWorkerMethod = WorkerMethod
    'mCommandArgs = CommandArgs
    WorkerMethod.BeginInvoke(
      Me,
      Sub(ar)
        Try
          WorkerMethod.EndInvoke(ar)
        Catch ex As Exception
          mCallException = ex
        End Try

        Finish()
      End Sub, Nothing)

  End Sub

  Private Sub Setup(CurrentStatus As String, CompleteHandler As Action(Of Object, HttpContext))
    mCompleteHandler = CompleteHandler
    mCurrentStatus = CurrentStatus
    mWorkerGuid = System.Guid.NewGuid
    HttpContext.Current.Cache.Add(CachePrefix & mWorkerGuid.ToString, Me, Nothing, Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 2, 0), Caching.CacheItemPriority.Normal, Nothing)
  End Sub

  Friend Sub WriteHTTPResponse(Wait As Boolean, Context As HttpContext)

    SyncLock Me
      If mIsFinished Then

        If mCallException IsNot Nothing Then
          If Singular.Web.WebError.SupportsWebError Then Singular.Web.WebError.LogError(mCallException, "Async web request")
          Throw mCallException
        End If

        Context.Cache.Remove(CachePrefix & mWorkerGuid.ToString)

        mCompleteHandler(mReturnValue, Context)

      Else

        If Wait Then
          Me.Wait()
        End If

        Dim jw As New Data.JSonWriter
        jw.StartClass("")
        jw.WriteObject("AjaxProgressInfo", Me)
        jw.EndClass()
        mStreamData = Nothing

        Context.Response.Write(jw.ToString)

      End If
    End SyncLock
    

  End Sub

  Friend Shared Function GetInstance(Guid As String) As AjaxProgress
    Return HttpContext.Current.Cache.Get(CachePrefix & Guid)
  End Function

End Class




Public Class SaveImmediateArgs
  Inherits CommandArgs

  Private mIsDelete As Boolean
  Private mUpdatedObject As Object
  Private mSuccess As Boolean = True
  Private mSaveError As String

  Public ReadOnly Property UpdatedObject As Object
    Get
      Return mUpdatedObject
    End Get
  End Property

  Public ReadOnly Property IsDelete As Boolean
    Get
      Return mIsDelete
    End Get
  End Property

  Public Property SavedObject As Object

  ''' <summary>
  ''' Specifies if the save was successfull or not. This is ignored if SaveHelper is set.
  ''' </summary>
  Public Property Success As Boolean
    Get
      If SaveHelper IsNot Nothing Then
        Return SaveHelper.Success
      Else
        Return mSuccess
      End If
    End Get
    Set(value As Boolean)
      mSuccess = value
    End Set
  End Property

  ''' <summary>
  ''' Specifies the cause of the save failing. This is ignored if SaveHelper is set.
  ''' </summary>
  Public Property SaveError As String
    Get
      If SaveHelper IsNot Nothing Then
        Return SaveHelper.ErrorText
      Else
        Return mSaveError
      End If
    End Get
    Set(value As String)
      mSaveError = value
    End Set
  End Property

  ''' <summary>
  ''' Set to the save helper that was used to save the object.
  ''' </summary>
  Public Property SaveHelper As SaveHelper

  Public Sub New(ClientArgs As Object, IsAjax As Boolean, ModelUpdated As Boolean, TransferModel As Boolean, UpdatedObject As Object, IsDelete As Boolean, ViewModel As IViewModel)
    MyBase.New(ClientArgs, IsAjax, ModelUpdated, TransferModel, ViewModel)

    mUpdatedObject = UpdatedObject
    mIsDelete = IsDelete
  End Sub

End Class


Public Class UpdateInfo
  Public Property UpdatedObject As Object
  Public Property ObjectGuid As Guid
  Public Property Container As Data.JS.ObjectInfo.Member
End Class