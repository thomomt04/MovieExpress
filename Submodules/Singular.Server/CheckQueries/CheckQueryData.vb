Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

#If SILVERLIGHT Then

Imports C1.Silverlight.Data
#Else
Imports System.Data.SqlClient
Imports System.Linq
#End If

Namespace CheckQueries

  <Serializable()> _
  Public Class CheckQueryData
    Inherits Singular.SingularReadOnlyBase(Of CheckQueryData)

#Region " Properties and Methods "

#Region " Properties "

#If SILVERLIGHT Then

  Private mCheckQueryData As DataSet

  Public ReadOnly Property CheckQueryDataSet() As DataSet
    Get
      Return mCheckQueryData
    End Get
  End Property

#End If

    Public Shared CheckQueryDataProperty As PropertyInfo(Of Byte()) = RegisterProperty(Of Byte())(Function(c) c.CheckQueryData, "Check Query Data")

    Public ReadOnly Property CheckQueryData() As Byte()
      Get
        Return GetProperty(CheckQueryDataProperty)
      End Get
    End Property

#End Region

#Region " Methods "

#If SILVERLIGHT Then

  Public Function GetOriginalDataSet() As DataSet

    ' reinstate our DataSet
    Dim ds As New DataSet("CheckQueryData")

    Dim strm As New IO.MemoryStream(Me.CheckQueryData)
    ds.ReadXml(strm)

    For Each tbl In ds.Tables
      For Each clm In tbl.Columns
        clm.Caption = Singular.Strings.Readable(clm.ColumnName)
      Next
    Next

    Return ds

  End Function

  Protected Overrides Sub OnDeserialized(context As System.Runtime.Serialization.StreamingContext)
    MyBase.OnDeserialized(context)

    mCheckQueryData = GetOriginalDataSet()

    End Sub
#Else

    Public Shadows Function GetDataSet() As DataSet

      Dim ds As New DataSet("CheckQueryData")

      Dim strm As New IO.MemoryStream(Me.CheckQueryData)
      ds.ReadXml(strm)

      For Each tbl As DataTable In ds.Tables
        For Each col As DataColumn In tbl.Columns
          col.Caption = Singular.Strings.Readable(col.ColumnName)
        Next
      Next

      Dim NoDataInd As Boolean = True

      For Each table As DataTable In ds.Tables
        If table.Rows.Count > 0 Then
          NoDataInd = False
          Exit For
        End If
      Next
      If NoDataInd Then
        Return Nothing
      Else
        Return ds
      End If

    End Function

#End If

#End Region

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Shared Sub BeginGetCheckQueryData(StoredProcName As String, View As Integer,
                                         ByVal CallBack As EventHandler(Of DataPortalResult(Of CheckQueryData)))

      Dim dp As New DataPortal(Of CheckQueryData)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria With {.ProcedureName = StoredProcName, .Type = View, .Timeout = 30})

    End Sub

    Public Sub New()

    End Sub

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared ProcedureNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ProcedureName, "Procedure Name")

      <Display(Name:="Procedure Name")> _
      Public Property ProcedureName() As String
        Get
          Return ReadProperty(ProcedureNameProperty)
        End Get
        Friend Set(ByVal value As String)
          LoadProperty(ProcedureNameProperty, value)
        End Set
      End Property

      Public Shared TypeProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Type, "Type")

      <Display(Name:="Parameter List")> _
      Public Property Type() As Integer
        Get
          Return ReadProperty(TypeProperty)
        End Get
        Friend Set(ByVal value As Integer)
          LoadProperty(TypeProperty, value)
        End Set
      End Property

      Public Shared TimeoutProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Timeout, "Type")

      Public Property Timeout() As Integer
        Get
          Return ReadProperty(TimeoutProperty)
        End Get
        Friend Set(ByVal value As Integer)
          LoadProperty(TimeoutProperty, value)
        End Set
      End Property

      Public Sub New()



      End Sub

    End Class

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewCheckQueryData() As CheckQueryData

      Return New CheckQueryData()

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Sub BeginGetCheckQueryData(ByVal CheckQueryDataSet As DataSet, ByVal CallBack As EventHandler(Of DataPortalResult(Of CheckQueryData)))

      Dim dp As New DataPortal(Of CheckQueryData)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New SingleCriteria(Of CheckQueryData, DataSet)(CheckQueryDataSet))

    End Sub

    Public Shared Function GetCheckQueryData(StoredProcName As String, Type As Integer, Timeout As Integer) As CheckQueryData

      Return DataPortal.Fetch(Of CheckQueryData)(New Criteria With {.ProcedureName = StoredProcName, .Type = Type, .Timeout = Timeout})

    End Function

    Protected Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand

            If crit.Type = 1 Then
              cm.CommandType = CommandType.Text
              cm.CommandText = "SELECT * FROM " & crit.ProcedureName
            Else
              cm.CommandType = CommandType.StoredProcedure
              cm.CommandText = crit.ProcedureName
            End If

            cm.CommandTimeout = crit.Timeout

            Dim da As New SqlDataAdapter(cm)
            Dim ds As New DataSet
            da.Fill(ds)

            Dim ms = New IO.MemoryStream()
            ds.WriteXml(ms, XmlWriteMode.WriteSchema)

            LoadProperty(CheckQueryDataProperty, ms.ToArray())

            'Using sdr As New SafeDataReader(cm.ExecuteReader)
            '  If sdr.Read() Then
            '    Fetch(sdr)
            '  End If
            'End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace


