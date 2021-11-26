Namespace WebServices

  Public Module Misc

    Public Class AjaxRequestArgs
      Public Property ClientArgs As Object
      Friend StopRequest As Boolean = False
      Friend ReturnData As String

      Public Sub New(ClientArgs As Object)
        Me.ClientArgs = ClientArgs
      End Sub

      Public Sub StopProcessing(ReturnData As String)
        StopRequest = True
        Me.ReturnData = ReturnData
      End Sub
    End Class

    Public Property OnAjaxRequest As Action(Of AjaxRequestArgs)

  End Module

  Public Interface IGetData

    Function GetData(Args As StatelessArguments) As Object


  End Interface

  Public Interface ISaveData

    Sub PrepareSave(Args As PrepareSaveArguments)

  End Interface

  Public Class DataInterface

    Private Shared mIGetDataInstances As New Hashtable

    Public Shared Function GetInstance(Type As Type) As Object

      Dim Instance As Object
      'If mIGetDataInstances.ContainsKey(Type.FullName) Then
      '  Instance = mIGetDataInstances(Type.FullName)
      'Else
      Instance = Activator.CreateInstance(Type, True)
      'mIGetDataInstances(Type.FullName) = Instance
      'End If
      Return Instance

    End Function

  End Class

End Namespace

Public Class Result

  Sub New()
    ' TODO: Complete member initialization 
  End Sub

  ''' <summary>
  ''' True if property marked with InitialDataOnly must be rendered.
  ''' </summary>
  Public Property IsInitialData As Boolean = True

  Public Property Success As Boolean
  Public Property ErrorText As String
  Public Property Data As Object

  Public Sub New(Success As Boolean)
    Me.Success = Success
  End Sub

  Public Sub New(Success As Boolean, ErrorText As String)
    Me.Success = Success
    Me.ErrorText = ErrorText
  End Sub

  Public Sub New(Func As Func(Of Object))
    Execute(Func)
  End Sub

  Protected Sub Execute(Func As Func(Of Object))
    Try
      Data = Func()
      If TypeOf Data Is Result Then
        HandleResultResult(Data)
      Else
        Success = True
      End If
    Catch ex As Exception
      HandleException(ex)
    End Try
  End Sub

  Public Sub New(Func As Func(Of Result))
    Try
      HandleResultResult(Func())
    Catch ex As Exception
      HandleException(ex)
    End Try
  End Sub

  Private Sub HandleResultResult(Data As Result)
    Me.Success = Data.Success
    Me.Data = Data.Data
    Me.ErrorText = Data.ErrorText
  End Sub

  Public Sub New(Action As Action)
    Try
      Action()
      Success = True
    Catch ex As Exception
      HandleException(ex)
    End Try
  End Sub

  Private Sub HandleException(ex As Exception)
    If Debugger.IsAttached OrElse Singular.Debug.IsCustomError(ex) Then
      ErrorText = Singular.Debug.RecurseExceptionMessage(ex)
    Else
      If Singular.Web.WebError.SupportsWebError Then
        ErrorText = "An error has occured. Error ID: " & Singular.Web.WebError.LogError(ex)
      Else
        ErrorText = "An error has occured."
      End If
    End If

    Success = False
  End Sub

End Class

Public Class DataResult
  Inherits Result

  Public Property TypeName As String

  Public Sub New(Success As Boolean, Data As Object)
    MyBase.New(Success)
    MyBase.Data = Data
    TypeName = Singular.ReflectionCached.GetCachedType(Data.GetType).DotNetTypeName
  End Sub

End Class

Public Class SaveResult
  Inherits Result

  Public Property MessageType As MessageType
  Public Property Message As String

	Public Sub New(Success As Boolean)
		MyBase.New(Success)

	End Sub

	Public Sub New(sr As Singular.Web.SaveHelper)
    MyBase.New(sr.Success)

    Setup(sr)
  End Sub

  Public Sub New(sr As Singular.SaveHelper)
    MyBase.New(sr.Success)

    Setup(New Singular.Web.SaveHelper(sr, Nothing))
  End Sub

  Private Sub Setup(sr As Singular.Web.SaveHelper)

    IsInitialData = False

    If sr.Result = Singular.SaveHelper.ResultType.Error AndAlso Not Singular.Debug.InDebugMode Then
      If Singular.Web.WebError.SupportsWebError Then
        Dim ErrorID As Integer = WebError.LogError(sr.Error, "StatelessHander")
        ErrorText = "Service Error. Error ID: " & ErrorID
      Else
        ErrorText = "An unknown error occurred."
      End If
    Else
      ErrorText = sr.ErrorText
    End If

    MessageType = sr.SaveMessageType
    Message = sr.SaveMessage
    Data = sr.SavedObject
  End Sub

  Public Sub New(r As Result)
    MyBase.New(r.Success, r.ErrorText)

    If r.Success Then
      MessageType = Singular.Web.MessageType.Success
      Message = "Saved Successfully"
    Else
      MessageType = Singular.Web.MessageType.Error
      Message = ErrorText
    End If
  End Sub

End Class

Public Class ASyncResult
  Inherits Result

  Private mAjaxProgress As AjaxProgress
  Public ReadOnly Property AjaxProgressInfo As AjaxProgress
    Get
      Return mAjaxProgress
    End Get
  End Property

  Public Sub New(ProgressText As String, Func As Func(Of AjaxProgress, Object))
    MyBase.New(True)

    Try
      mAjaxProgress = New AjaxProgress(ProgressText, Func,
                                       Sub(rv, context)
                                         If mAjaxProgress.WrapResponse Then
                                           context.Response.Write(Singular.Web.Data.JSonWriter.SerialiseObject(New Result(True) With {.Data = rv}))
                                         Else
                                           context.Response.Write(Singular.Web.Data.JSonWriter.SerialiseObject(rv))
                                         End If

                                       End Sub)

    Catch ex As Exception
      mAjaxProgress = Nothing
      ErrorText = Singular.Debug.RecurseExceptionMessage(ex)
      Success = False
    End Try
  End Sub

  Public Sub New(ProgressText As String, WorkerMethod As Action(Of AjaxProgress))
    MyBase.New(True)

    Try
      mAjaxProgress = New AjaxProgress(ProgressText, WorkerMethod,
                                       Sub(rv, context)

                                         context.Response.Write(Singular.Web.Data.JSonWriter.SerialiseObject(New Result(True)))
                                       End Sub)

    Catch ex As Exception
      mAjaxProgress = Nothing
      ErrorText = Singular.Debug.RecurseExceptionMessage(ex)
      Success = False
    End Try
  End Sub

End Class

Public Class MessageResult
  Inherits Result

  Public Sub New()
    MyBase.New(True)
  End Sub

  Public Sub New(MessageType As Singular.Web.MessageType, Title As String, Message As String)
    MyBase.New(True)
    AddMessage(MessageType, Title, Message)
  End Sub

  Public Property MessageList As New Singular.Web.CustomControls.MessageList

  Public Sub AddMessage(MessageType As Singular.Web.MessageType, Title As String, Message As String, Optional ContainerName As String = "")
    MessageList.AddMessage(ContainerName, "", MessageType, Title, Message)
  End Sub

End Class
