Namespace CheckQueryHelpers

  Public Class CheckQueryVM
    Inherits ViewModel(Of CheckQueryVM)

    Public Shared CheckQueryListProperty As Csla.PropertyInfo(Of Singular.CheckQueries.CheckQueryList) = RegisterProperty(Of Singular.CheckQueries.CheckQueryList)(Function(c) c.CheckQueryList)

    Public Property CheckQueryList() As Singular.CheckQueries.CheckQueryList
      Get
        Return GetProperty(CheckQueryListProperty)
      End Get
      Set(value As Singular.CheckQueries.CheckQueryList)
        SetProperty(CheckQueryListProperty, value)
      End Set
    End Property

    Public Property IsLoading As Boolean = False
    Public Property Status As String = ""
    Public Property CQResult As New CheckQueryResult
    Public Property CurrentCQ As String

    Protected Overrides Sub Setup()
      MyBase.Setup()

      CheckQueryList = Singular.CheckQueries.CheckQueryList.GetCheckQueriesList()

    End Sub

    Protected Overrides Sub HandleCommand(Command As String, CommandArgs As Singular.Web.CommandArgs)
      MyBase.HandleCommand(Command, CommandArgs)

      AssertAccess()

      Select Case Command
        Case "Download"
          Dim cq = CheckQueryList.Where(Function(c) c.Description = CommandArgs.ClientArgs).FirstOrDefault()
          Dim ee As New Singular.Data.ExcelExporter
          ee.PopulateData(cq.CheckQueryDataSet)
          SendFile(cq.Name & ".xlsx", ee.GetStream.ToArray())
      End Select

    End Sub

    Public Function RunAllCheckQueries() As Singular.Web.ASyncResult

      AssertAccess()

      Return New Singular.Web.ASyncResult(
        "Running check queries",
        Function(ap)

          ap.MaxSteps = CheckQueryList.Count
          ap.Update()

          For Each cq As Singular.CheckQueries.CheckQuery In CheckQueryList

            ap.CurrentStatus = "Running " & cq.Description
            cq.Run()
            ap.Increment()
            ap.SetStreamData(Function()
                               Return New List(Of Tuple(Of String, Integer))
                             End Function,
                              Sub(sd)
                                sd.Add(New Tuple(Of String, Integer)(cq.Description, cq.Status))
                              End Sub)
            ap.Update()

          Next

          Return ap

        End Function)

    End Function

    Public Function RunCheckQuery(Description As String) As Singular.Web.Result

      AssertAccess()

      Return New Singular.Web.Result(
        Function()
          Dim cq = CheckQueryList.Where(Function(c) c.Description = Description).FirstOrDefault
          cq.Run()
          Return cq.Status
        End Function)
    End Function

  End Class

  Public Class CheckQueryResult
    Inherits Singular.Web.SGridInfo

    Public Sub New()
      MyBase.New("Check Queries", "")

      AllowDataExport = True
      AllowSaveLayout = False
      AllowGraph = False

    End Sub

    Public Overrides ReadOnly Property FriendlyName As String
      Get
        Return "Check Query Result"
      End Get
    End Property

    Public Overrides Function GetData(Arguments As Object) As DataSet

      If Arguments IsNot Nothing Then

        Dim cqList = Singular.CheckQueries.CheckQueryList.GetCheckQueriesList()
        Dim cq = cqList.Where(Function(c) c.Description = Arguments).FirstOrDefault

        If cq IsNot Nothing Then
          cq.Run()
          Return cq.CheckQueryDataSet
        End If

      End If

      Dim ds As New DataSet
      ds.Tables.Add()
      Return ds

    End Function
  End Class

End Namespace

