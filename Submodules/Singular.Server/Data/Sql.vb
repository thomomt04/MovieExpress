Imports System.Data.SqlClient
Imports System.Threading.Tasks

Namespace Data

  Public Class Sql

    Public Class SqlTypeDotNetType

      Public Property SqlType As String

      Private mDotNetDirectType As String

      Public Property DotNetDirectType As String
        Get
          Return mDotNetDirectType
        End Get
        Set(value As String)
          mDotNetDirectType = value
          If String.IsNullOrWhiteSpace(Me.DotNetUsedType) Then
            Me.DotNetUsedType = mDotNetDirectType
          End If
        End Set
      End Property

      Public Property DotNetUsedType As String

    End Class

    Public Shared SqlToDotNetTypes As List(Of SqlTypeDotNetType) = New List(Of SqlTypeDotNetType) From {
      New SqlTypeDotNetType() With {.SqlType = "bigint", .DotNetDirectType = "Int64", .DotNetUsedType = "Integer"},
      New SqlTypeDotNetType() With {.SqlType = "binary", .DotNetDirectType = "Byte()"},
      New SqlTypeDotNetType() With {.SqlType = "bit", .DotNetDirectType = "Boolean"},
      New SqlTypeDotNetType() With {.SqlType = "char", .DotNetDirectType = "String"},
      New SqlTypeDotNetType() With {.SqlType = "date", .DotNetDirectType = "DateTime"},
      New SqlTypeDotNetType() With {.SqlType = "datetime", .DotNetDirectType = "DateTime"},
      New SqlTypeDotNetType() With {.SqlType = "decimal", .DotNetDirectType = "Decimal"},
      New SqlTypeDotNetType() With {.SqlType = "float", .DotNetDirectType = "Double"},
      New SqlTypeDotNetType() With {.SqlType = "image", .DotNetDirectType = "Byte()"},
      New SqlTypeDotNetType() With {.SqlType = "int", .DotNetDirectType = "Int32", .DotNetUsedType = "Integer"},
      New SqlTypeDotNetType() With {.SqlType = "money", .DotNetDirectType = "Decimal"},
      New SqlTypeDotNetType() With {.SqlType = "nchar", .DotNetDirectType = "String"},
      New SqlTypeDotNetType() With {.SqlType = "ntext", .DotNetDirectType = "String"},
      New SqlTypeDotNetType() With {.SqlType = "numeric", .DotNetDirectType = "Decimal"},
      New SqlTypeDotNetType() With {.SqlType = "nvarchar", .DotNetDirectType = "String"},
      New SqlTypeDotNetType() With {.SqlType = "real", .DotNetDirectType = "Single"},
      New SqlTypeDotNetType() With {.SqlType = "smalldatetime", .DotNetDirectType = "DateTime"},
      New SqlTypeDotNetType() With {.SqlType = "smallint", .DotNetDirectType = "Int16", .DotNetUsedType = "Integer"},
      New SqlTypeDotNetType() With {.SqlType = "smallmoney", .DotNetDirectType = "Decimal"},
      New SqlTypeDotNetType() With {.SqlType = "sql_variant", .DotNetDirectType = "Object *"},
      New SqlTypeDotNetType() With {.SqlType = "text", .DotNetDirectType = "String"},
      New SqlTypeDotNetType() With {.SqlType = "timestamp", .DotNetDirectType = "Byte()"},
      New SqlTypeDotNetType() With {.SqlType = "tinyint", .DotNetDirectType = "Byte", .DotNetUsedType = "Integer"},
      New SqlTypeDotNetType() With {.SqlType = "uniqueidentifier", .DotNetDirectType = "Guid"},
      New SqlTypeDotNetType() With {.SqlType = "varbinary", .DotNetDirectType = "Byte()"},
      New SqlTypeDotNetType() With {.SqlType = "varchar", .DotNetDirectType = "String"}
    }

    Public Shared Function GetDotNetTypeFromSqlType(SqlType As String) As String

      Return (From st In SqlToDotNetTypes
              Where st.SqlType = SqlType.ToLower
              Select st.DotNetUsedType).FirstOrDefault

    End Function

    ''' <summary>
    ''' Will attempt to convert the SQL Command into an executable SQL String (only tested with normal business object commands)
    ''' </summary>
    ''' <param name="cm">The command that must be converted</param>
    ''' <returns>The executable SQL string that can be run directly against SQL</returns>
    ''' <remarks></remarks>
    Public Shared Function CommandAsSql(cm As SqlCommand, Optional IncludeOutputParameterDeclaration As Boolean = True) As String

      Dim sql As String = ""
      Dim preSql As String = ""
      If cm.CommandType = CommandType.StoredProcedure Then
        sql = "EXEC "
      End If
      sql &= cm.CommandText & vbCrLf
      For Each prm As SqlParameter In cm.Parameters
        If IncludeOutputParameterDeclaration AndAlso (prm.Direction = ParameterDirection.InputOutput OrElse prm.Direction = ParameterDirection.Output) Then
          If preSql = "" Then
            preSql = "DECLARE "
          Else
            preSql &= ", "
          End If
          preSql &= prm.ParameterName & " " & Singular.CommonData.Enums.Description(prm.SqlDbType)
          If prm.Value IsNot Nothing Then
            preSql &= " = " & GetSqlParameterValueString(prm)
          End If
        End If
        If prm.Value IsNot Nothing AndAlso Not (prm.Direction = ParameterDirection.InputOutput OrElse prm.Direction = ParameterDirection.Output) Then
          sql &= vbTab & prm.ParameterName & " = " & GetSqlParameterValueString(prm) & ", " & vbCrLf
        ElseIf prm.Direction = ParameterDirection.InputOutput OrElse prm.Direction = ParameterDirection.Output Then
          sql &= vbTab & prm.ParameterName & " = " & prm.ParameterName & " OUTPUT, " & vbCrLf
        End If
      Next
      Return If(preSql = "", "", preSql & vbCrLf) & Singular.Strings.RemoveCommaAndSpace(sql.TrimEnd())

    End Function

    Private Shared Function GetSqlParameterValueString(prm As SqlParameter) As String

      If IsDBNull(prm.Value) Then
        Return "NULL"
      Else
        Select Case prm.SqlDbType
          Case SqlDbType.Date
            Return "'" & CDate(prm.Value).ToString("yyyyMMdd") & "'"
          Case SqlDbType.DateTime, SqlDbType.DateTime2, SqlDbType.SmallDateTime
            Return "'" & CDate(prm.Value).ToString("yyyyMMdd HH:mm:ss") & "'"
          Case SqlDbType.VarChar, SqlDbType.NVarChar
            Return "'" & CStr(prm.Value) & "'"
          Case Else
            Return CStr(prm.Value)
        End Select
      End If
      

    End Function

    ''' <summary>
    ''' Will check if the connection settings seem valid without trying to connect to the database
    ''' </summary>
    ''' <param name="Problems">An output parameter listing the problems with the connection settings</param>
    ''' <param name="ServerName"></param>
    ''' <param name="DatabaseName"></param>
    ''' <param name="UserName"></param>
    ''' <param name="Password"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CheckConnectionSettings(ByRef Problems As String, ByVal ServerName As String, ByVal DatabaseName As String, Optional ByVal IntegratedSecurityInd As Boolean = True, Optional ByVal UserName As String = "", Optional ByVal Password As String = "") As Boolean

      Problems = ""
      If ServerName = "" Then
        Problems &= "A server name is required" & vbCrLf
      End If
      If DatabaseName = "" Then
        Problems &= "A database name is required" & vbCrLf
      End If

      If Not IntegratedSecurityInd Then
        If UserName = "" Then
          Problems &= "A username is required if not using Integrated Security"
        End If
        If Password = "" Then
          Problems &= "A password is required if not using Integrated Security"
        End If
      End If

      Return Problems = ""

    End Function

    ''' <summary>
    ''' Returns a sql connection string for a given server and database
    ''' </summary>
    ''' <param name="ServerName">The Server</param>
    ''' <param name="DatabaseName">The Database</param>
    ''' <param name="UserName">UserName, Leave blank for Integrated Security</param>
    ''' <param name="Password">Unencrypted Password</param>
    ''' <returns>A sql connection string</returns>
    ''' <remarks></remarks>
    Public Shared Function GetConnectionString(ByVal ServerName As String, ByVal DatabaseName As String, Optional ByVal UserName As String = "", Optional ByVal Password As String = "")
      If UserName.Length = 0 Then
        Dim strTemp As String = "Data Source=" & ServerName & ";Integrated Security=SSPI;"
        strTemp &= "Initial Catalog=" & DatabaseName
        Return strTemp
      Else
        Return "server=" & ServerName & ";uid=" & UserName & ";pwd=" & Password & ";database=" & DatabaseName
      End If

    End Function

    Public Shared Function GetSQLType(ByVal DotNetType As Type) As System.Data.SqlDbType

      If DotNetType Is GetType(String) Then
        Return SqlDbType.VarChar
      ElseIf DotNetType Is GetType(Integer) OrElse DotNetType Is GetType(Integer?) Then
        Return SqlDbType.Int
      ElseIf DotNetType Is GetType(Boolean) OrElse DotNetType Is GetType(Boolean?) Then
        Return SqlDbType.Bit
      ElseIf DotNetType Is GetType(DateTime) OrElse DotNetType Is GetType(DateTime?) Then
        Return SqlDbType.DateTime
      ElseIf DotNetType Is GetType(Decimal) OrElse DotNetType Is GetType(Decimal?) Then
        Return SqlDbType.Money
      Else
        Return SqlDbType.Variant
      End If

    End Function

    Public Shared Function GetSQLType(ByVal sSQLTypeName As String) As System.Data.SqlDbType
      Select Case sSQLTypeName.ToLower()
        Case "bigint"
          GetSQLType = SqlDbType.BigInt
        Case "binary"
          GetSQLType = SqlDbType.Binary
        Case "bit"
          GetSQLType = SqlDbType.Bit
        Case "char"
          GetSQLType = SqlDbType.Char
        Case "datetime"
          GetSQLType = SqlDbType.DateTime
        Case "decimal"
          GetSQLType = SqlDbType.Decimal
        Case "float"
          GetSQLType = SqlDbType.Float
        Case "image"
          GetSQLType = SqlDbType.Image
        Case "int"
          GetSQLType = SqlDbType.Int
        Case "money"
          GetSQLType = SqlDbType.Money
        Case "nchar"
          GetSQLType = SqlDbType.NChar
        Case "ntext"
          GetSQLType = SqlDbType.NText
        Case "nvarchar"
          GetSQLType = SqlDbType.NVarChar
        Case "real"
          GetSQLType = SqlDbType.Real
        Case "smalldatetime"
          GetSQLType = SqlDbType.SmallDateTime
        Case "smallint"
          GetSQLType = SqlDbType.SmallInt
        Case "smallmoney"
          GetSQLType = SqlDbType.SmallMoney
        Case "sql_variant"
          GetSQLType = SqlDbType.Variant
        Case "text"
          GetSQLType = SqlDbType.Text
        Case "timestamp"
          GetSQLType = SqlDbType.Timestamp
        Case "tinyint"
          GetSQLType = SqlDbType.TinyInt
        Case "uniqueidentifier"
          GetSQLType = SqlDbType.UniqueIdentifier
        Case "varbinary"
          GetSQLType = SqlDbType.VarBinary
        Case "varchar"
          GetSQLType = SqlDbType.VarChar
        Case Else
          GetSQLType = SqlDbType.Variant
      End Select
    End Function

    ''' <summary>
    ''' Adds a parameter to a command that is a table parameter with 1 column. The Type needs to be defined in SQL.
    ''' </summary>
    ''' <typeparam name="DataType"></typeparam>
    ''' <param name="cm">The SQLCommand</param>
    ''' <param name="ParamName">The name of the parameter in the datasource.</param>
    ''' <param name="List">The list of values to be passed in.</param>
    ''' <param name="ColumnName">The column name of the custom SQL data type.</param>
    Public Shared Sub AddTableParameter(Of DataType)(cm As SqlCommand, ParamName As String, List As List(Of DataType), ColumnName As String)

      Dim SqlList As New List(Of Microsoft.SqlServer.Server.SqlDataRecord)

      For Each Item As DataType In List
        Dim sdr As Microsoft.SqlServer.Server.SqlDataRecord
        If GetType(DataType) Is GetType(String) Then
          'Hack, length should be passed in as an argument.
          sdr = New Microsoft.SqlServer.Server.SqlDataRecord(New Microsoft.SqlServer.Server.SqlMetaData(ColumnName, SqlDbType.VarChar, 200))
        Else
          sdr = New Microsoft.SqlServer.Server.SqlDataRecord(New Microsoft.SqlServer.Server.SqlMetaData(ColumnName, GetSQLType(GetType(DataType))))
        End If

        sdr.SetValue(0, Item)
        SqlList.Add(sdr)
      Next

      With cm.Parameters.AddWithValue(ParamName, SqlList)
        .SqlDbType = SqlDbType.Structured
        '.TypeName = "StringList"
      End With

    End Sub


  End Class

#Region " Notify Events "

#If SILVERLIGHT = False Then

  'ALTER DATABASE x SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  'ALTER DATABASE x SET NEW_BROKER
  'ALTER DATABASE x SET ENABLE_BROKER
  'ALTER DATABASE x SET MULTI_USER

  'SELECT is_broker_enabled, service_broker_guid, name FROM sys.databases 

  'CREATE QUEUE QueueName;

  'CREATE SERVICE ServiceName
  '  ON QUEUE QueueName
  '([http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification]);

  'GRANT SUBSCRIBE QUERY NOTIFICATIONS TO [Singular\Domain Users];
  'GRANT REFERENCES ON CONTRACT::[http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification] TO [Singular\Domain Users];
  'GRANT RECEIVE ON MarketStatusQueue TO [Singular\Domain Users];
  'GRANT RECEIVE ON QueryNotificationErrorsQueue TO [Singular\Domain Users];

  Public Class SqlNotifier
    Public Property ProcName As String
    Public Property QueueName As String
    Public Property ServiceName As String
    Public Property Timeout As Integer
    Public Event ResultsChanged(sender As Object, args As SqlNotifierEventArgs)
    Private LockObject As New Object

    Public Enum ResultsChangedType
      Initial = 1
      Changed = 2
      Timeout = 3
      InvalidQuery = 4
      Exception = 5
    End Enum

    Public Class SqlNotifierEventArgs

      Private mChangeType As ResultsChangedType
      Private mReader As SqlDataReader
      Private mException As Exception

      Public Sub New(ChangeType As ResultsChangedType, Reader As SqlDataReader)
        mChangeType = ChangeType
        mReader = Reader
      End Sub

      Public Sub New(Exception As Exception)
        mException = Exception
        mChangeType = ResultsChangedType.Exception
      End Sub

      Public ReadOnly Property ChangeType As ResultsChangedType
        Get
          Return mChangeType
        End Get
      End Property

      Public ReadOnly Property DataReader As SqlDataReader
        Get
          Return mReader
        End Get
      End Property

      Public ReadOnly Property Exception As Exception
        Get
          Return mException
        End Get
      End Property
    End Class

    ''' <summary>
    ''' Creates a class that listens for when the data returned by the stored proc: ProcName changes.
    ''' See class definition for how to create the sql queues / services.
    ''' </summary>
    ''' <param name="ProcName">The stored proc name that returns the data you want to be notified of changes on. NB: This proc must execute as owner.</param>
    ''' <param name="QueueName">Name of the Sql Queue you created.</param>
    ''' <param name="ServiceName">Name of the Sql Service you created.</param>
    ''' <param name="TimeoutSeconds">Timeout in seconds before the Stored proc is run again if no data changed. Zero is not a valid value.</param>
    ''' <remarks></remarks>
    Public Sub New(ProcName As String, QueueName As String, ServiceName As String, TimeoutSeconds As Integer)

      Me.ProcName = ProcName
      Me.QueueName = QueueName
      Me.ServiceName = ServiceName
      Me.Timeout = TimeoutSeconds

    End Sub

    Private mHasStarted As Boolean = False
    Public Sub StartListening()
      If Not mHasStarted Then
        CreateQueueReader(ResultsChangedType.Initial)
        mHasStarted = True
      End If
    End Sub

    Private mQueueException As Exception = Nothing
    Private mClientID As Guid = Guid.NewGuid
    Private mMessageType As String = ""
    Private mMessageInfo As String = ""

    Public Sub CreateQueueReader(Type As ResultsChangedType)

      'Read all messages off the queue
      ClearQueue()

      mQueueException = Nothing

      'Create a thread to read from the queue.
      'The Queue will only return data when there is a message on the queue.
      Dim ListenTask As New Task(AddressOf WaitForMessage)

      'When there is a message on the queue, the above query will finish, and the code below will be called.
      ListenTask.ContinueWith(AddressOf MessageReceived)

      ListenTask.Start()

      SyncLock LockObject

        'Read the results of the query we want to be notified of changes.
        'Attach a notification request to this command.
        Dim parser As New SqlConnectionStringBuilder(Singular.Settings.ConnectionString)
        Dim req As New System.Data.Sql.SqlNotificationRequest(mClientID.ToString,
                                                              String.Format("Service={0}; local database={1}", ServiceName, parser.InitialCatalog),
                                                              Timeout + 10)

        Using cn As New SqlClient.SqlConnection(Singular.Settings.ConnectionString)
          Using cmd As New SqlClient.SqlCommand(ProcName, cn)
            cmd.CommandType = CommandType.StoredProcedure
            cn.Open()
            cmd.Notification = req

            Using dr As IDataReader = cmd.ExecuteReader()
              RaiseEvent ResultsChanged(Me, New SqlNotifierEventArgs(Type, dr))
            End Using

          End Using
        End Using

      End SyncLock

    End Sub

    Private Sub ClearQueue()
      'Check if there are multiple entries on the queue, and clear them all.
      Dim StillHasResults As Boolean = True

      Using cn As New SqlClient.SqlConnection(Singular.Settings.ConnectionString)
        cn.Open()
        Do
          Using cmd2 As New SqlCommand("RECEIVE * FROM " & QueueName & "", cn)
            Using dr As IDataReader = cmd2.ExecuteReader()
              StillHasResults = dr.Read
            End Using
          End Using
        Loop While StillHasResults
      End Using
    End Sub

    Private Sub WaitForMessage()

      SyncLock LockObject

        Using cn As New SqlClient.SqlConnection(Singular.Settings.ConnectionString)
          Using cmd As New SqlCommand("WAITFOR (RECEIVE *, CASE WHEN validation = 'X' THEN CAST(message_body as XML) ELSE NULL END As Msg FROM " & QueueName & ")", cn)
            cn.Open()
            cmd.CommandTimeout = Timeout

            Try
              Dim dr As SqlDataReader = cmd.ExecuteReader()
              While dr.Read()

                Dim Msg As String = dr.GetValue(dr.FieldCount - 1)

                If Msg IsNot Nothing Then

                  Dim XMLDoc As New System.Xml.XmlDocument
                  XMLDoc.LoadXml(Msg)

                  mMessageType = XMLDoc.FirstChild.Attributes("type").Value
                  mMessageInfo = XMLDoc.FirstChild.Attributes("info").Value

                End If

              End While
            Catch ex As Exception
              mQueueException = ex
            End Try
          End Using
        End Using

      End SyncLock

    End Sub

    Private Sub MessageReceived(T As System.Threading.Tasks.Task)

      Dim IsTimeout As Boolean = mQueueException IsNot Nothing AndAlso TypeOf mQueueException Is SqlException AndAlso CType(mQueueException, SqlException).Message.StartsWith("Timeout expired", StringComparison.InvariantCultureIgnoreCase)

      If mQueueException IsNot Nothing Then
        If IsTimeout Then
          CreateQueueReader(If(IsTimeout, ResultsChangedType.Timeout, ResultsChangedType.Changed))
        Else
          'An exception occurred, dont listen for messages again.
          RaiseEvent ResultsChanged(Me, New SqlNotifierEventArgs(mQueueException))
        End If
      Else
        If mMessageInfo = "invalid" Then
          'The query is invalid for notifications, dont listen for messages again.
          RaiseEvent ResultsChanged(Me, New SqlNotifierEventArgs(ResultsChangedType.InvalidQuery, Nothing))
        Else
          CreateQueueReader(If(IsTimeout, ResultsChangedType.Changed, ResultsChangedType.Changed))
        End If
      End If

    End Sub

  End Class

  ''' <summary>
  ''' Listens for messages being pushed onto a SQL queue. A callback is called when a message is received, and the listening starts again.
  ''' The process repeats every [Timeout] seconds in case a message is missed.
  ''' </summary>
  Public Class SQLQueueReader

    '1. Add this to enable Service Broker, and create a queue and service

    'ALTER DATABASE X SET ENABLE_BROKER with rollback immediate

    'CREATE QUEUE QueueName;

    'CREATE SERVICE ServiceName
    '  ON QUEUE QueueName
    '([DEFAULT]);x`

    'GRANT REFERENCES ON CONTRACT::[DEFAULT] TO [?User?];

    ' YOU MAY ALSO NEED TO DO THIS:
    ' GRANT RECEIVE ON QueueName TO [?User?]

    ' GRANT SEND ON SERVICE::ServiceName TO [?User?]


    '2. Use this to write to the queue

    'DECLARE @c UniqueIdentifier;

    'BEGIN DIALOG CONVERSATION @c
    'FROM SERVICE EmailService TO SERVICE 'EmailService'
    'WITH ENCRYPTION = OFF;

    'SEND ON CONVERSATION @c('SendEmails')


    '99. Debugging
    'Check if service broker enabled
    'select * from sys.transmission_queue;

    Private mQueueName As String
    Private mIsRunning As Boolean = True

    Private mSyncDelegate As Action(Of Action(Of Exception))
    Private mAsyncResult As IAsyncResult
    Private mLatestCommand As SqlCommand
    Private mErrorCount As Integer = 0

    'Timeout in seconds.
    Public Property Timeout As Integer = 300
    Public Property CallbackOnTimeout As Boolean = False

    ''' <summary>
    ''' The number of simultaneous threads allowed at once.
    ''' </summary>
    Public Property MaxConcurrentThreads As Integer = 1

    Public Sub New(QueueName As String)
      mQueueName = QueueName
    End Sub

    ''' <summary>
    ''' Starts waiting for a message without blocking the current thread.
    ''' </summary>
    ''' <param name="Callback">Method to run when a message is received. The next message will not be processed until the first one is complete, unless MaxConcurrentThreads is more than 1.</param>
    ''' <param name="ErrorCallback">Method to run when an exception occurs.</param>
    Public Sub BeginWaitForMessage(Callback As Action(Of String), ErrorCallback As Action(Of Exception))
      mCallback = Callback
      mSyncDelegate = AddressOf WaitForMessage

      mAsyncResult = mSyncDelegate.BeginInvoke(
        ErrorCallback,
        Sub()

          mSyncDelegate.EndInvoke(mAsyncResult)

        End Sub, Nothing)

    End Sub

    Private Sub WaitForMessage(ErrorCallback As Action(Of Exception))

      While mIsRunning

        Try

          Using cn As New SqlClient.SqlConnection(Singular.Settings.ConnectionString)
            cn.Open()

            Using mLatestCommand = New SqlClient.SqlCommand("WAITFOR (RECEIVE CAST(message_body as XML) As [Message] FROM " & mQueueName & ")", cn)
              mLatestCommand.CommandTimeout = Timeout

              Dim HasData As Boolean = False
              Dim Data As String = ""
              Try
                Dim Reader = mLatestCommand.ExecuteReader()
                While mIsRunning AndAlso Reader.Read
                  Data = Reader.GetString(0)
                  HasData = True
                End While
                mErrorCount = 0
              Catch ex As SqlClient.SqlException
                If CType(ex, SqlClient.SqlException).Number <> -2 Then

                  OnError(ex, ErrorCallback)

                Else
                  'Timeout
                  If CallbackOnTimeout Then CheckAsyncCallBack(Data)
                End If
              End Try

              If mIsRunning AndAlso HasData Then
                CheckAsyncCallBack(Data)
              End If

            End Using

          End Using

        Catch ex As Exception

          OnError(ex, ErrorCallback)

        End Try

      End While

    End Sub

    Private mCurrentThreads As Integer = 1
    Private mCallback As Action(Of String)

    Private Sub CheckAsyncCallBack(Message As String)

      If mCurrentThreads < MaxConcurrentThreads Then
        'Start callback on new thread.
        Threading.Interlocked.Increment(mCurrentThreads)
        Dim AsyncResult As IAsyncResult = Nothing

        AsyncResult = mCallback.BeginInvoke(
          Message,
          Sub()
            Try
              mCallback.EndInvoke(AsyncResult)
            Finally
              Threading.Interlocked.Decrement(mCurrentThreads)
            End Try
          End Sub, Nothing)
      Else
        'Too many threads.
        mCallback.Invoke(Message)
      End If

    End Sub

    Private Sub OnError(ex As Exception, ErrorCallback As Action(Of Exception))

      ErrorCallback(ex)

      mErrorCount += 1
      If mErrorCount > 3 Then
        [Stop]()
      Else
        System.Threading.Thread.Sleep(mErrorCount * 5000)
      End If

    End Sub

    Public Sub [Stop]()
      mIsRunning = False

      If mLatestCommand IsNot Nothing Then
        mLatestCommand.Cancel()
      End If
    End Sub

  End Class

#End If

#End Region



End Namespace
