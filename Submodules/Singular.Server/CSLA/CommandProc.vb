Imports System.Data.SqlClient
Imports Csla
Imports Csla.Core
Imports Csla.Data
Imports Csla.Properties
Imports Csla.Serialization

<Serializable()>
Public Class CommandProc
  Inherits Csla.CommandBase(Of CommandProc)
  Implements IDisposable

#Region " Parameters "

  <Serializable()>
  Public Class Parameter
    Public Property Name As String
    Public Property Value As Object
    Public Property Size As Integer
    Public Property Direction As System.Data.ParameterDirection
    Public Property SqlType As SqlDbType = SqlDbType.Structured
    Public Property TypeName As String = String.Empty
  End Class

  <Serializable()>
  Public Class ParameterList
    Inherits List(Of Parameter)

    Public Function AddWithValue(ParameterName As String, Value As Object, Optional Direction As System.Data.ParameterDirection = ParameterDirection.Input, Optional Size As Integer = 0) As Parameter
      Dim p As New Parameter
      p.Name = ParameterName
      p.Value = Value
      p.Direction = Direction
      p.Size = Size
      Me.Add(p)
      Return p

    End Function

    Public Function AddWithValue(ParameterName As String, Value As Object, SqlType As SqlDbType, Optional Direction As System.Data.ParameterDirection = 0, Optional Size As Integer = 0) As Parameter
      Dim p As New Parameter
      p.Name = ParameterName
      p.Value = Value
      p.Direction = Direction
      p.SqlType = SqlType
      p.Size = Size
      Me.Add(p)
      Return p
    End Function

    Default Public Overloads ReadOnly Property item(key As String) As Parameter
      Get
        For Each p As Parameter In Me
          If p.Name = key Then
            Return p
          End If
        Next
        Return Nothing
      End Get
    End Property

  End Class

  Public Shared ParametersProperty As PropertyInfo(Of ParameterList) = RegisterProperty(Of ParameterList)(Function(c) c.Parameters)

  Public ReadOnly Property Parameters As ParameterList
    Get
      If ReadProperty(ParametersProperty) Is Nothing Then
        LoadProperty(ParametersProperty, New ParameterList)
      End If
      Return ReadProperty(ParametersProperty)
    End Get
  End Property

#End Region

#Region " Transaction Sharing "

  Private Enum TransactionCommand
    None
    Commit
    Rollback
  End Enum

  Private mTransactionName As String = ""
  Private mTransactionCommand As TransactionCommand = TransactionCommand.None

  ''' <summary>
  ''' Allows a batch of commands to be executed on the same transaction. Once the batch is complete, call CommitBatch.
  ''' If an exception occurs, the transaction is rolled back, and the connection is closed.
  ''' </summary>
  ''' <param name="TransactionName">The common name used to link different commands to the same transaction.</param>
  Public Function ExecuteBatch(TransactionName As String) As CommandProc
    mTransactionName = TransactionName
    UseTransaction = True
    Return Execute()
  End Function

  ''' <summary>
  ''' Commits a batch started with ExecuteBatch
  ''' </summary>
  Public Shared Sub CommitBatch(TransactionName As String)
    EndBatch(TransactionName, TransactionCommand.Commit)
  End Sub

  ''' <summary>
  ''' Rolls back a batch started with ExecuteBatch
  ''' </summary>
  Public Shared Sub RollbackBatch(TransactionName As String)
    EndBatch(TransactionName, TransactionCommand.Rollback)
  End Sub

  ''' <summary>
  ''' Runs a stored procedure on the database and returns a dataset
  ''' </summary>
  Public Shared Function GetDataSet(ProcName As String, ParamNames() As String, ParamValues() As Object) As DataSet
    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.FetchType = FetchTypes.DataSet
    Return cProc.Execute.Dataset
  End Function

  ''' <summary>
  ''' Runs a stored procedure on the database and returns the first row
  ''' </summary>
  Public Shared Function GetDataRow(ProcName As String, ParamNames() As String, ParamValues() As Object) As DataRow
    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.FetchType = FetchTypes.DataRow
    Return cProc.Execute.DataRow
  End Function


  ''' <summary>
  ''' Runs the sql select query on the database and returns the first column of the first row
  ''' </summary>
  Public Shared Function GetDataValueFromQuery(QueryText As String, ParamNames() As String, ParamValues() As Object) As Object
    Dim cProc As New Singular.CommandProc(QueryText, ParamNames, ParamValues)
    cProc.FetchType = FetchTypes.DataRow
    cProc.CommandType = System.Data.CommandType.Text
    Return cProc.Execute.DataRowValue
  End Function

  ''' <summary>
  ''' Runs a stored procedure on the database and returns the first column of the first row
  ''' </summary>
  Public Shared Function GetDataValue(ProcName As String, ParamNames() As String, ParamValues() As Object) As Object
    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.FetchType = FetchTypes.DataRow
    Return cProc.Execute.DataRowValue
  End Function

  Public Shared Function GetDataValues(Of T1, T2)(ProcName As String, ParamNames() As String, ParamValues() As Object) As Tuple(Of T1, T2)
    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.FetchType = FetchTypes.DataRow
    cProc = cProc.Execute()
    If cProc.DataRow IsNot Nothing Then
      Return New Tuple(Of T1, T2)(cProc.DataRow(0), cProc.DataRow(1))
    Else
      Return New Tuple(Of T1, T2)(Nothing, Nothing)
    End If

  End Function

  Public Shared Function GetDataValues(Of T1, T2, T3)(ProcName As String, ParamNames() As String, ParamValues() As Object) As Tuple(Of T1, T2, T3)
    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.FetchType = FetchTypes.DataRow
    cProc = cProc.Execute()
    If cProc.DataRow IsNot Nothing Then
      Return New Tuple(Of T1, T2, T3)(cProc.DataRow(0), cProc.DataRow(1), cProc.DataRow(2))
    Else
      Return New Tuple(Of T1, T2, T3)(Nothing, Nothing, Nothing)
    End If

  End Function

  Public Shared Function GetDataValues(Of T1, T2, T3, T4)(ProcName As String, ParamNames() As String, ParamValues() As Object) As Tuple(Of T1, T2, T3, T4)
    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.FetchType = FetchTypes.DataRow
    cProc = cProc.Execute()
    If cProc.DataRow IsNot Nothing Then
      Return New Tuple(Of T1, T2, T3, T4)(cProc.DataRow(0), cProc.DataRow(1), cProc.DataRow(2), cProc.DataRow(3))
    Else
      Return New Tuple(Of T1, T2, T3, T4)(Nothing, Nothing, Nothing, Nothing)
    End If

  End Function

  ''' <summary>
  ''' Executes a stored procedure, and populates an object with the first row of the result. Returns null if there are no rows.
  ''' </summary>
  ''' <param name="Params">Dynamic object with Parameter names and values. e.g. new with {Param1, Param2}</param>
  Public Shared Function GetObject(Of T)(ProcName As String, Optional Params As Object = Nothing, Optional UseTransaction As Boolean = False) As T

    Dim cProc As New Singular.CommandProc(ProcName)
    cProc.AddDynamicParams(Params)
    cProc.UseTransaction = UseTransaction

    Dim ReturnObject As T

    cProc.ExecuteReaderLocal(
      Sub(sdr)

        If sdr.Read Then
          ReturnObject = Activator.CreateInstance(Of T)()
          Singular.Data.DataFunctions.PopulateObject(ReturnObject, sdr)
        End If

      End Sub)

    Return ReturnObject
  End Function

  ''' <summary>
  ''' Executes a stored procedure, and populates a list of the supplied object type. Object property names must match the column names returned by the proc.
  ''' Note, this isnt very fast. If your proc returns a large amount of rows, rather create a dedicated CSLA list.
  ''' </summary>
  ''' <param name="Params">Dynamic object with Parameter names and values. e.g. new with {Param1, Param2}</param>
  Public Shared Function GetList(Of T)(ProcName As String, Optional Params As Object = Nothing, Optional UseTransaction As Boolean = False) As List(Of T)

    Dim cProc As New Singular.CommandProc(ProcName)
    cProc.AddDynamicParams(Params)
    cProc.UseTransaction = UseTransaction

    Dim ReturnList As New List(Of T)

    cProc.ExecuteReaderLocal(
      Sub(sdr)
        Singular.Data.DataFunctions.PopulateList(Of T)(ReturnList, sdr)
      End Sub)

    Return ReturnList

  End Function

  Public Sub AddDynamicParams(Params As Object)

    If Params IsNot Nothing Then
      Dim Properties = Params.GetType.GetProperties(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
      If Params IsNot Nothing Then
        For Each Prop In Properties
          Parameters.AddWithValue("@" & Prop.Name, Prop.GetValue(Params, Nothing))
        Next
      End If
    End If

  End Sub

  ''' <summary>
  ''' Runs a stored procedure on the database. Use GetDataXXX if you need data returned.
  ''' </summary>
  Public Shared Sub RunCommand(ProcName As String, ParamNames() As String, ParamValues() As Object, Optional UseTransaction As Boolean = False)
    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.UseTransaction = UseTransaction
    cProc.FetchType = FetchTypes.None
    cProc.Execute()
  End Sub

  ''' <summary>
  ''' Runs a stored procedure on the database. Use GetDataXXX if you need data returned.
  ''' </summary>
  Public Shared Sub RunCommand(ProcName As String, ParamNames() As String, ParamValues() As Object, ParamTypes() As SqlDbType, ParamDirection() As ParameterDirection, ParamSizes() As Integer, Optional UseTransaction As Boolean = False)
    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues, ParamTypes, ParamDirection, ParamSizes)
    cProc.UseTransaction = UseTransaction
    cProc.FetchType = FetchTypes.None
    cProc.Execute()
  End Sub

  ''' <summary>
  ''' Runs a stored procedure on the database. Use GetDataXXX if you need data returned.
  ''' </summary>
  Public Shared Sub RunCommand(CommandString As String, Optional UseTransaction As Boolean = False)
    Dim cProc As New Singular.CommandProc(CommandString)
    cProc.CommandType = System.Data.CommandType.Text
    cProc.FetchType = FetchTypes.None
    cProc.UseTransaction = UseTransaction
    cProc.Execute()
  End Sub

  Private Shared Sub EndBatch(TransactionName As String, Type As TransactionCommand)
    Dim cProc As New Singular.CommandProc()
    cProc.mTransactionName = TransactionName
    cProc.UseTransaction = True
    cProc.mTransactionCommand = Type
    cProc.Execute()
  End Sub

  Private Function GetConnectionID() As String
    If mTransactionName = "" Then
      Return "cn"
    Else
      Return "cn_" & mTransactionName
    End If
  End Function

  Private Function GetTransactionID() As String
    If mTransactionName = "" Then
      Return "tr"
    Else
      Return "tr_" & mTransactionName
    End If
  End Function

  Private Function GetTransactionName() As String
    If mTransactionName = "" Then
      Return Left(CommandText, 32)
    Else
      Return mTransactionName
    End If
  End Function

#End Region

  Private mFetchType As FetchTypes
  Private mConnectionString As String = ""
  Private mNoOfRowsAffected As Integer = -1

  Private mCommandText As String
  Private mCommandType As System.Data.CommandType = System.Data.CommandType.StoredProcedure

  Private mExecTimeOut As Integer = 30
  Private mUseTransaction As Boolean = False

  Public Shared SingularDataSetProperty As PropertyInfo(Of SingularDataSet.DataSet) = RegisterProperty(Of SingularDataSet.DataSet)(Function(c) c.SingularDataSet, "SingularDataSet")

  Public ReadOnly Property SingularDataSet As SingularDataSet.DataSet
    Get
      Return ReadProperty(SingularDataSetProperty)
    End Get
  End Property

  <NonSerialized()> Private mDataRow As DataRow
  Private mDataObject As Object

  <NonSerialized()> Private mDataset As DataSet

  Public ReadOnly Property Dataset As DataSet
    Get
      Return mDataset
    End Get
  End Property

  ''' <summary>
  ''' Returns the number of rows affected when FetchType = CommandProc.FetchTypes.None (ExecuteNonQuery)
  ''' </summary>
  ''' <value></value>
  ''' <returns>Integer</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property NoOfRowsAffected As Integer
    Get
      Return mNoOfRowsAffected
    End Get
  End Property

  Public Property UseSingularDataSet As Boolean = False

  Public Property ConnectionString() As String
    Get
      Return mConnectionString
    End Get
    Set(ByVal value As String)
      mConnectionString = value
    End Set
  End Property

  Public Property CommandText() As String
    Get
      Return mCommandText
    End Get
    Set(ByVal value As String)
      If mCommandText <> value Then
        mCommandText = value
      End If
    End Set
  End Property

  <System.Obsolete("Please set the CommandText property instead.", True)>
  Public Sub SetNewCommandText(ByVal NewCommandText As String)
    mCommandText = NewCommandText
  End Sub

  Public Property CommandType() As System.Data.CommandType
    Get
      Return mCommandType
    End Get
    Set(ByVal value As System.Data.CommandType)
      If mCommandType <> value Then
        mCommandType = value
      End If
    End Set
  End Property

  Public Property CommandTimeout As Integer
    Get
      Return mExecTimeOut
    End Get

    Set(value As Integer)
      If mExecTimeOut <> value Then
        mExecTimeOut = value
      End If
    End Set
  End Property

  Public Enum FetchTypes
    DataSet
    DataRow
    None
    DataReader
    DataObject
  End Enum

  Public Property FetchType() As FetchTypes
    Get
      Return mFetchType
    End Get
    Set(ByVal value As FetchTypes)
      mFetchType = value
      If mFetchType = FetchTypes.DataReader Then
        mExecuteAsReader = True
      End If
    End Set
  End Property

  Public ReadOnly Property DataRow() As DataRow
    Get
      Return mDataRow
    End Get
  End Property

  Public ReadOnly Property DataObject As Object
    Get
      Return mDataObject
    End Get
  End Property

  ''' <summary>
  ''' Returns the value of the first cell in the datarow.
  ''' Returns nothing if there is no datarow.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public ReadOnly Property DataRowValue As Object
    Get
      If mDataRow Is Nothing Then
        Return Nothing
      Else
        Return mDataRow(0)
      End If
    End Get
  End Property

  Public Property UseTransaction() As Boolean
    Get
      Return mUseTransaction
    End Get
    Set(ByVal value As Boolean)
      mUseTransaction = value
    End Set
  End Property

  Public Sub New()

  End Sub

  Public Sub New(ByVal CommandText As String)

    mCommandText = CommandText
    mFetchType = FetchTypes.None

  End Sub

  Public Sub New(ByVal CommandText As String, ByVal Parameter As String, ByVal ParameterValue As Object)
    mCommandText = CommandText
    Me.Parameters.AddWithValue(Parameter, ParameterValue)
    mFetchType = FetchTypes.None

  End Sub

  Public Sub New(ByVal CommandText As String, ByVal Parameters() As String, ByVal ParameterValues() As Object)
    mCommandText = CommandText
    For i As Integer = 0 To Parameters.Length - 1
      Me.Parameters.AddWithValue(Parameters(i), ParameterValues(i))
    Next
    mFetchType = FetchTypes.None
  End Sub

  Public Sub New(ByVal CommandText As String, ByVal Parameters() As String, ByVal ParameterValues() As Object, ByVal FetchType As FetchTypes)
    mCommandText = CommandText
    For i As Integer = 0 To Parameters.Length - 1
      Me.Parameters.AddWithValue(Parameters(i), ParameterValues(i))
    Next
    mFetchType = FetchType
  End Sub

  Public Sub New(ByVal CommandText As String, ByVal Parameters() As String, ByVal ParameterValues() As Object, ByVal ParameterDirections() As System.Data.ParameterDirection)
    mCommandText = CommandText
    For i As Integer = 0 To Parameters.Length - 1
      Me.Parameters.AddWithValue(Parameters(i), ParameterValues(i), ParameterDirections(i))
    Next
    mFetchType = FetchTypes.None
  End Sub

  Public Sub New(ByVal CommandText As String, ByVal Parameters() As String, ByVal ParameterValues() As Object, ByVal ParameterDirections() As System.Data.ParameterDirection, ByVal ParameterSizes() As Integer)
    mCommandText = CommandText
    For i As Integer = 0 To Parameters.Length - 1
      Me.Parameters.AddWithValue(Parameters(i), ParameterValues(i), ParameterDirections(i), ParameterSizes(i))
    Next
    mFetchType = FetchTypes.None
  End Sub

  Public Sub New(ByVal CommandText As String, ByVal Parameters() As String, ByVal ParameterValues() As Object, ByVal ParameterTypes() As SqlDbType, ByVal ParameterDirections() As System.Data.ParameterDirection, ByVal ParameterSizes() As Integer)
    mCommandText = CommandText
    For i As Integer = 0 To Parameters.Length - 1
      Me.Parameters.AddWithValue(Parameters(i), ParameterValues(i), ParameterTypes(i), ParameterDirections(i), ParameterSizes(i))
    Next
    mFetchType = FetchTypes.None
  End Sub

  Public Sub New(Command As System.Data.SqlClient.SqlCommand, FetchTypes As FetchTypes, CommandType As CommandType)

    mCommandText = Command.CommandText
    mFetchType = FetchTypes
    mCommandType = CommandType
    For Each param As SqlClient.SqlParameter In Command.Parameters
      Me.Parameters.AddWithValue(param.ParameterName, param.Value, param.Direction, param.Size).SqlType = param.SqlDbType
    Next

  End Sub

  Public Overridable Function Execute(Optional ByVal Timeout As Integer = 30) As CommandProc
    If Timeout <> 30 Then
      mExecTimeOut = Timeout
    End If
    Return DataPortal.Execute(Of Singular.CommandProc)(Me)

    'If Singular.Security.CSLAConnectionMethod = Security.CSLAConnectionMethods.Remoting Then
    '  Select Case mFetchType
    '    Case FetchTypes.DataSet
    '      mDataset = SingularDataSet.ConvertSingularDataSetToDotNetDataSet(x.mSingularDataSet)
    '    Case FetchTypes.DataRow
    '      Dim tbl As DataTable = SingularDataSet.ConvertSingularDataTableToDotNetDataTable(x.mSingularDataTable)
    '      If tbl.Rows.Count > 0 Then
    '        mDataRow = tbl.Rows(0)
    '      End If
    '  End Select
    'End If

  End Function

  Public Sub ExecuteLocal()
    ExecuteCommandServer()
  End Sub

  Public Sub ExecuteReaderLocal(OnReadData As Action(Of Csla.Data.SafeDataReader))
    FetchType = FetchTypes.DataReader
    mExecuteAsReader = True
    mOnReadData = OnReadData
    ExecuteLocal()
  End Sub

  Public Event ConnectionInfoMessageReceived(ByVal sender As Object, ByVal e As SqlInfoMessageEventArgs)

  Private Sub Connection_InfoMessage(ByVal sender As Object, ByVal e As SqlInfoMessageEventArgs)

    OnConnectionInfoMessage(sender, e)
    RaiseEvent ConnectionInfoMessageReceived(sender, e)

  End Sub

  Protected Overridable Sub OnConnectionInfoMessage(ByVal sender As Object, ByVal e As SqlInfoMessageEventArgs)



  End Sub

  <NonSerialized> Private mReader As SqlDataReader
  <NonSerialized> Private mOnReadData As Action(Of Csla.Data.SafeDataReader)


  Private mExecuteAsReader As Boolean = False

  Public Property ExecuteAsReader() As Boolean
    Get
      Return mExecuteAsReader
    End Get
    Set(ByVal value As Boolean)
      mExecuteAsReader = value
    End Set
  End Property

  Private mSetArithAbortOn As Boolean

  Public Property SetArithAbortON() As Boolean
    Get
      Return mSetArithAbortOn
    End Get
    Set(ByVal value As Boolean)
      If mSetArithAbortOn <> value Then
        mSetArithAbortOn = value
      End If
    End Set
  End Property

  Protected Overrides Sub DataPortal_Execute()

    If mTransactionCommand = TransactionCommand.None Then
      ExecuteCommandServer()
    Else
      EndBatchServer()
    End If

  End Sub

  Private Sub ExecuteCommandServer()

    Dim cnn As SqlClient.SqlConnection = CType(Csla.ApplicationContext.LocalContext(GetConnectionID), SqlClient.SqlConnection)

    Dim bConnectionCreated As Boolean = False
    Dim HasRolledBack As Boolean = False

    If IsNothing(cnn) OrElse cnn.State = ConnectionState.Closed Then
      If mConnectionString = "" Then
        cnn = New SqlConnection(Settings.ConnectionString)
      Else
        cnn = New SqlConnection(mConnectionString)
      End If
      Csla.ApplicationContext.LocalContext(GetConnectionID) = cnn
      bConnectionCreated = True
      cnn.Open()
    End If

    AddHandler cnn.InfoMessage, AddressOf Connection_InfoMessage

    Dim mCmd = New SqlCommand(mCommandText, cnn)
    mCmd.CommandType = mCommandType
    mCmd.Connection = cnn

    For Each param As Parameter In Me.Parameters
      Dim sp = mCmd.Parameters.AddWithValue(param.Name, param.Value)
      If param.Direction <> 0 Then
        sp.Direction = param.Direction
      End If
      If param.Size <> 0 Then
        sp.Size = param.Size
      End If
      If param.SqlType <> SqlDbType.Structured Then
        sp.SqlDbType = param.SqlType
      Else
        If Not String.IsNullOrEmpty(param.TypeName) Then
          sp.SqlDbType = param.SqlType
          sp.TypeName = param.TypeName
        End If
      End If
    Next

    Try

      mCmd.Transaction = CType(Csla.ApplicationContext.LocalContext(GetTransactionID), SqlClient.SqlTransaction)

      Dim bTransactionCreated As Boolean = False

      If mCmd.Transaction Is Nothing AndAlso mUseTransaction Then
        mCmd.Transaction = mCmd.Connection.BeginTransaction(IsolationLevel.ReadUncommitted, GetTransactionName)
        Csla.ApplicationContext.LocalContext(GetTransactionID) = mCmd.Transaction
        bTransactionCreated = True
      End If

      Try
        If mCmd.CommandTimeout = 30 Then
          mCmd.CommandTimeout = mExecTimeOut
        End If
        'cnn.Open()

        If Me.SetArithAbortON Then
          Dim mCmdArithAbort As New SqlCommand
          mCmdArithAbort.CommandType = System.Data.CommandType.Text
          mCmdArithAbort.CommandText = "SET ARITHABORT ON"
          mCmdArithAbort.Connection = cnn
          mCmdArithAbort.Transaction = mCmd.Transaction
          mCmdArithAbort.ExecuteNonQuery()
        End If

        'Set the Context Info for Audit Trails
        CSLALib.ContextInfo.SetContextInfoOnConnection(mCmd.Connection, mCmd.Transaction)

        Select Case mFetchType
          Case FetchTypes.DataRow

            Using adp As New SqlDataAdapter(mCmd)
              Dim tbl As New DataTable
              adp.Fill(tbl)
              If tbl.Rows.Count > 0 Then
                mDataRow = tbl.Rows(0)
              End If
            End Using

          Case FetchTypes.DataObject
            Using adp As New SqlDataAdapter(mCmd)
              Dim tbl As New DataTable
              adp.Fill(tbl)
              If tbl.Rows.Count > 0 Then
                mDataObject = tbl.Rows(0)(0)
              End If
            End Using


          Case FetchTypes.DataSet

            Using adp As New SqlDataAdapter(mCmd)
              mDataset = New DataSet
              adp.Fill(mDataset)
            End Using

          Case FetchTypes.None, FetchTypes.DataReader
            If mExecuteAsReader Then
              mReader = mCmd.ExecuteReader()
              Dim sdr As New Csla.Data.SafeDataReader(mReader)
              If mOnReadData IsNot Nothing Then
                mOnReadData(sdr)
                mReader.Close()
              End If
              ReadDataFromReader(sdr)
              RaiseEvent ReadData(Me, New ReadDataEventArgs(mReader))
              If Not mReader.IsClosed Then
                mReader.Close()
              End If
            Else
              mNoOfRowsAffected = mCmd.ExecuteNonQuery()
            End If
        End Select

        For Each param As SqlParameter In mCmd.Parameters
          If param.Direction = ParameterDirection.Output OrElse param.Direction = ParameterDirection.InputOutput Then
            Me.Parameters.item(param.ToString).Value = param.Value
          End If
        Next

        If bTransactionCreated AndAlso mTransactionName = "" Then
          mCmd.Transaction.Commit()
        End If
      Catch ex As Exception
        If bTransactionCreated OrElse mTransactionName <> "" Then
          If mCmd.Transaction IsNot Nothing Then mCmd.Transaction.Rollback()
          HasRolledBack = True
        End If
        Throw ex
      End Try

    Finally

      If (bConnectionCreated AndAlso mTransactionName = "") OrElse HasRolledBack Then
        cnn.Close()
        Csla.ApplicationContext.LocalContext(GetConnectionID) = Nothing
        Csla.ApplicationContext.LocalContext(GetTransactionID) = Nothing
      End If
    End Try

    If UseSingularDataSet Then
      Select Case mFetchType
        Case FetchTypes.DataSet
          LoadProperty(SingularDataSetProperty, Singular.SingularDataSet.ConvertDotNetDataSetToSingularDataSet(mDataset))
        Case FetchTypes.DataRow
          If mDataRow IsNot Nothing Then
            Dim sd As New Singular.SingularDataSet.DataSet
            sd.Tables.Add(New Singular.SingularDataSet.DataTable)
            sd.Tables(0).Rows.Add(Singular.SingularDataSet.ConvertDotNetDataRowToSingularDataRow(mDataRow))
            LoadProperty(SingularDataSetProperty, sd)
          End If
      End Select
    End If

  End Sub
  Protected Overridable Sub ReadDataFromReader(sdr As SafeDataReader)
    ' do nothing
  End Sub

  Private Sub EndBatchServer()

    Dim cnn As SqlClient.SqlConnection = Csla.ApplicationContext.LocalContext(GetConnectionID)
    Dim tr As SqlClient.SqlTransaction = Csla.ApplicationContext.LocalContext(GetTransactionID)

    Try
      If tr IsNot Nothing Then
        If mTransactionCommand = TransactionCommand.Commit Then
          tr.Commit()
        Else
          tr.Rollback()
        End If
      End If
    Finally
      If cnn IsNot Nothing Then
        cnn.Close()
      End If
      Csla.ApplicationContext.LocalContext(GetConnectionID) = Nothing
      Csla.ApplicationContext.LocalContext(GetTransactionID) = Nothing
    End Try

  End Sub

  Public Class ReadDataEventArgs
    Inherits EventArgs

    Private mDataReader As SqlDataReader

    Public Sub New(ByVal dr As SqlDataReader)
      mDataReader = dr
    End Sub

    Public ReadOnly Property DataReader() As SqlDataReader
      Get
        Return mDataReader
      End Get
    End Property
  End Class

  Public Event ReadData(ByVal Sender As Object, ByVal e As ReadDataEventArgs)

  Protected Overrides Sub DataPortal_OnDataPortalException(ByVal e As Csla.DataPortalEventArgs, ByVal ex As System.Exception)

    'TODO CSLALib.Events.OnCSLADataPortalException(Me, ex)

    MyBase.DataPortal_OnDataPortalException(e, ex)

  End Sub

#Region "IDisposable Support"
  Private disposedValue As Boolean ' To detect redundant calls

  ' IDisposable
  Protected Overridable Sub Dispose(disposing As Boolean)
    If Not Me.disposedValue Then
      If disposing Then
        ' TODO: dispose managed state (managed objects).
      End If

      ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
      ' TODO: set large fields to null.
    End If
    Me.disposedValue = True
  End Sub

  ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
  'Protected Overrides Sub Finalize()
  '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
  '    Dispose(False)
  '    MyBase.Finalize()
  'End Sub

  ' This code added by Visual Basic to correctly implement the disposable pattern.
  Public Sub Dispose() Implements IDisposable.Dispose
    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    Dispose(True)
    GC.SuppressFinalize(Me)
  End Sub
#End Region

  Public Sub AddTableParameter(ParameterName As String, ParameterValue As DataTable)
    Dim param = Me.Parameters.AddWithValue(ParameterName, ParameterValue)
    param.SqlType = SqlDbType.Structured
  End Sub

End Class
