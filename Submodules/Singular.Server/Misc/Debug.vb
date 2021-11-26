Public Class Debug

  Public Shared Property AutoUserName As String = ""

  Public Shared Property AutoPassword As String = ""

  ''' <summary>
  ''' Returns a string containing all the messages of exceptions that are in the specified exception's tree
  ''' </summary>
  ''' <param name="ex"></param>
  ''' <param name="OverrideForCustomErrors"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function RecurseExceptionMessage(ByVal ex As Exception, Optional ByVal OverrideForCustomErrors As Boolean = True) As String

    Dim sError As String = pRecurseExceptionMessage(ex)
    If OverrideForCustomErrors AndAlso sError.IndexOf("CustomError") <> -1 Then
      ' there is a custom error here, so strip out the rest of the rubbish
      sError = sError.Substring(sError.LastIndexOf("CustomError:") + 13)
    End If
    Return sError

  End Function

  Public Shared Function GetBaseException(ex As Exception) As Exception
    While ex.InnerException IsNot Nothing
      ex = ex.InnerException
    End While
    Return ex
  End Function

  Private Shared Function pRecurseExceptionMessage(ByVal ex As Exception) As String

#If SILVERLIGHT Then
      If ex.InnerException Is Nothing Then
        Return ex.Message
      Else
        Return ex.Message & vbCrLf & pRecurseExceptionMessage(ex.InnerException)
      End If
#Else
    If TypeOf ex Is System.Web.HttpException AndAlso ex.InnerException IsNot Nothing Then
      Return pRecurseExceptionMessage(ex.InnerException)
    Else
      If ex.InnerException Is Nothing Then
        Return ex.Message
      Else
        Return ex.Message & vbCrLf & pRecurseExceptionMessage(ex.InnerException)
      End If
    End If
#End If



  End Function

  ''' <summary>
  ''' Returns the stack trace of an exception without the System methods.
  ''' </summary>
  Public Shared Function GetCleanStackTrace(Ex As Exception) As String

    While Ex.InnerException IsNot Nothing AndAlso Ex.InnerException.StackTrace IsNot Nothing
      Ex = Ex.InnerException
    End While

    Dim NewString As String = ""
    If Ex.StackTrace IsNot Nothing Then
      Dim Frames As String() = Ex.StackTrace.Replace(vbCr, "").Split({vbLf}, StringSplitOptions.RemoveEmptyEntries)
      For Each Frame As String In Frames
        If Not String.IsNullOrEmpty(Frame) AndAlso Not Frame.Trim.StartsWith("at System.") Then
          NewString &= Frame.Trim & vbCrLf
        End If
      Next
    End If

    Return NewString
  End Function

  Public Shared Function InDebugMode() As Boolean

    Return System.Diagnostics.Debugger.IsAttached

  End Function

#If SILVERLIGHT Then
#Else

  ''' <summary>
  ''' Returns true if the visual studio development environment is executing.
  ''' This will happen when inherited forms are viewed etc, any code you dont want the designer to execute should be put in 
  ''' an if statement (if not InDesignMode)
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function InDesignMode() As Boolean

    Try
      Return System.Diagnostics.Process.GetCurrentProcess().ProcessName.StartsWith("devenv")
    Catch ex As Exception
      Return True
    End Try

  End Function

#End If

  Shared Function IsCustomError(ex As Exception) As Boolean
    If IsNothing(ex.InnerException) Then
      Return ex.Message.IndexOf("CustomError") <> -1
    Else
      Return (ex.Message.IndexOf("CustomError") <> -1) Or IsCustomError(ex.InnerException)
    End If
  End Function

  Public Shared Function ExceptionContainsExceptionType(ByRef ex As Exception, ByVal ExceptionType As Type)

    Dim dummy As Exception = Nothing
    Return ExceptionContainsExceptionType(ex, ExceptionType, dummy)

  End Function

  Public Shared Function ExceptionContainsExceptionType(ByRef ex As Exception, ByVal ExceptionType As Type, ByRef MatchingException As Exception)

    If ex.GetType.Equals(ExceptionType) Then
      MatchingException = ex
      Return True
    ElseIf ex.InnerException Is Nothing Then
      Return False
    Else
      Return ExceptionContainsExceptionType(ex.InnerException, ExceptionType, MatchingException)
    End If

  End Function

  ''' <summary>
  ''' Returns the message of the bottom most exception in the specified exception's tree
  ''' </summary>
  ''' <param name="ex">An exception</param>
  ''' <returns>Exception Message</returns>
  ''' <remarks></remarks>
  Public Shared Function GetBottomLevelExceptionMessage(ByVal ex As Exception) As String
    Return GetBottomLevelException(ex).Message
  End Function

  ''' <summary>
  ''' Returns the message of the bottom most exception in the specified exception's tree
  ''' </summary>
  ''' <param name="ex">An exception</param>
  ''' <returns>Exception Message</returns>
  ''' <remarks></remarks>
  Public Shared Function GetBottomLevelException(ByVal ex As Exception) As Exception
    Dim tmpEx As Exception = ex
    While tmpEx.InnerException IsNot Nothing
      tmpEx = tmpEx.InnerException
    End While
    Return tmpEx
  End Function

#If SILVERLIGHT Then
#Else

  Public Shared Function IsTimeoutException(ByVal ex As Exception) As Boolean

    If TypeOf ex Is System.Data.SqlClient.SqlException Then
      Return ex.Message.StartsWith("Timeout Expired")
    End If
    Return False

  End Function

  Public Class ActivityTimer
    Private Class TimeKeeper
      'Public mStartTime As Date
      Public mTimeElapsed As Double = 0

      Dim SW As New Stopwatch

      Public Sub Start()
        SW.Start()
        'mStartTime = Now
      End Sub

      Public Sub [Stop]()
        ' Dim TE As TimeSpan = Now.Subtract(mStartTime)
        'mTimeElapsed += TE.TotalMilliseconds
        SW.Stop()
        mTimeElapsed = SW.ElapsedTicks / Stopwatch.Frequency
      End Sub

    End Class

    Private Shared mTimes As Dictionary(Of String, TimeKeeper)

    Public Shared Sub ResetTimes()
      mTimes = New Dictionary(Of String, TimeKeeper)
    End Sub

    Private Shared Function GetTimer(ByVal ActivityName As String) As TimeKeeper
      Dim tk As TimeKeeper
      If mTimes Is Nothing Then
        mTimes = New Dictionary(Of String, TimeKeeper)
      End If
      If mTimes.ContainsKey(ActivityName) Then
        tk = mTimes.Item(ActivityName)
      Else
        tk = New TimeKeeper
        mTimes.Add(ActivityName, tk)
      End If
      Return tk
    End Function

    Public Shared Sub StartTimer(ByVal ActivityName As String)
      GetTimer(ActivityName).Start()
    End Sub

    Public Shared Sub StopTimer(ByVal ActivityName As String)
      GetTimer(ActivityName).Stop()
    End Sub

    Public Shared Function TimeTaken(ByVal ActivityName As String) As Double
      Return GetTimer(ActivityName).mTimeElapsed
    End Function

    Public Shared Function TimeTakenString(ByVal ActivityName As String) As String
      Return (TimeTaken(ActivityName)).ToString("#,##0.0000;(#,##0.0000)")
    End Function

    Public Shared Function AllTimesTakenString() As String
      Dim str As String = ""
      For Each key As String In mTimes.Keys
        str &= key & " " & (GetTimer(key).mTimeElapsed * 1000).ToString("#,##0.00") & " ms" & vbCrLf
      Next
      Return str
    End Function

  End Class
#End If

 

End Class
