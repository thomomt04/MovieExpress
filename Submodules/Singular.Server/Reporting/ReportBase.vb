Imports System.Reflection
Imports System.ComponentModel

Namespace Reporting

#Region " Report Base Interface "

  Public Enum ReportDocumentType
    PDF = 5 'Maps to crystal reports enum
    Word = 3
    ExcelData = 50
  End Enum

  Public Class ModifyDataSetArgs

    Public Property DataSet As DataSet
    Public Property DocumentType As ReportDocumentType = ReportDocumentType.ExcelData

  End Class

  Public Interface IReport
    ReadOnly Property ReportName As String
    <Browsable(False)> ReadOnly Property UniqueKey As String
    <Browsable(False)> ReadOnly Property ReportURL As String
		<Browsable(False)> ReadOnly Property AllowDataExport As Boolean
		<Browsable(False)> ReadOnly Property HideCrystalReport As Boolean
    ReadOnly Property ReportCriteriaGeneric As ReportCriteria
    <Browsable(False)> ReadOnly Property LinkTargetType As Integer
    ReadOnly Property Description As String
    <Browsable(False)> ReadOnly Property ShowWordExport As Boolean

    <Browsable(False)> ReadOnly Property CustomButtons As List(Of CustomButton)
    Function ExecCustomButton(ButtonID As Guid) As Object

    ReadOnly Property GridInfo As GridInfo

#If SILVERLIGHT = False Then

		<Browsable(False)> ReadOnly Property DataSourceIsOutOfDate As Boolean
		<Browsable(False)> ReadOnly Property CrystalReportType As Type
		<Browsable(False)> ReadOnly Property CustomCriteriaControlType As Type
    <Browsable(False)> ReadOnly Property DataSet As DataSet

    Sub Setup()
    Function GetData() As DataSet
    Function GetAsDocument(DocumentType As ReportDocumentType) As IO.MemoryStream
    Function GetDatasetSchemaFile() As IO.MemoryStream
    Function GetDocumentFile(DocumentType As ReportDocumentType) As ReportFileInfo
    Function GetExportFileName() As String
    Sub ResetData()
    Function ReportHasData() As Boolean

#End If

  End Interface

  ''' <summary>
  ''' Put this interface on your report if you need to show it on the reports screen (to allow criteria selection), but dont want it in the main reports menu.
  ''' </summary>
  Public Interface IStandAloneReport
    Function HasAccess() As Boolean
  End Interface

#End Region

#Region " Report Base "

  Public Class ReportFileInfo
    Implements Singular.Documents.IDocument

    Public Property FileName As String = "" Implements Documents.IDocument.DocumentName
    Public Property FileStream As IO.MemoryStream
    Public Property IsXSD As Boolean

    Public Property FileBytes As Byte() Implements Documents.IDocument.Document
      Get
        Return FileStream.ToArray
      End Get
      Private Set(value As Byte())
        'This has to be writable because of IDocument interface
        Throw New Exception("FileBytes only available through FileStream")
      End Set
    End Property

  End Class

  Public Class CustomButton
    Public Property ButtonID As Guid = Guid.NewGuid
    Public Property ButtonText As String
    Public Property ImageURL As String
    Public Property CallBack As Func(Of Singular.Documents.TemporaryDocument)
    Public Property CallBackPlainDoc As Func(Of Singular.Documents.Document)
    Public Property CallBackMsg As Func(Of Singular.Message)
    Public Property CallBackObj As Func(Of Object)
    Public Property AfterNormalButtons As Boolean = True

    Public Function Invoke() As Object
      If CallBack IsNot Nothing Then
        Return CallBack.Invoke
      ElseIf CallBackPlainDoc IsNot Nothing Then
        Return CallBackPlainDoc.Invoke
      ElseIf CallBackMsg IsNot Nothing Then
        Return CallBackMsg.Invoke()
      Else
        Return CallBackObj.Invoke()
      End If
    End Function

  End Class

  Public MustInherit Class ReportBase(Of RC As ReportCriteria)
    Implements IReport
    Implements IDisposable

    ''' <summary>
    ''' Name of Report.
    ''' </summary>
    Public MustOverride ReadOnly Property ReportName As String Implements IReport.ReportName

    <System.ComponentModel.Browsable(False)>
    Protected Overridable ReadOnly Property ExportFileName As String
      Get
        Return ReportName
      End Get
    End Property

    Public Overridable ReadOnly Property UniqueKey As String Implements IReport.UniqueKey
      Get
        Return ""
      End Get
    End Property

    Public Function GetExportFileName() As String Implements IReport.GetExportFileName
      Return ExportFileName.Replace(" ", "")
    End Function

    ''' <summary>
    ''' When clicking on the report menu item, a new window will be opened at the url specified.
    ''' </summary>
    <System.ComponentModel.Browsable(False)>
    Public Overridable ReadOnly Property ReportURL As String Implements IReport.ReportURL
      Get
        Return ""
      End Get
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Overridable ReadOnly Property AllowDataExport As Boolean Implements IReport.AllowDataExport
      Get
        Return True
      End Get
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Overridable ReadOnly Property HideCrystalReport As Boolean Implements IReport.HideCrystalReport
      Get
        Return False
      End Get
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Overridable ReadOnly Property GridInfo As GridInfo Implements IReport.GridInfo
      Get
        If GridExportEnabledByDefault Then
          Return New GridInfo(Me)
        Else
          Return Nothing
        End If
      End Get
    End Property


#Region " Custom Buttons "

    Private mCustomButtonList As New List(Of CustomButton)
    <System.ComponentModel.Browsable(False)>
    Public ReadOnly Property CustomButtons As List(Of CustomButton) Implements IReport.CustomButtons
      Get
        Return mCustomButtonList
      End Get
    End Property

    Protected Function AddCustomButton(ButtonText As String, ImageURL As String, CallBack As Func(Of Singular.Documents.TemporaryDocument)) As CustomButton
      Dim eb As New CustomButton With {.ButtonText = ButtonText, .ImageURL = ImageURL, .CallBack = CallBack}
      mCustomButtonList.Add(eb)
      Return eb
    End Function

    Protected Function AddCustomButton(ButtonText As String, ImageURL As String, CallBack As Func(Of Singular.Documents.Document)) As CustomButton
      Dim eb As New CustomButton With {.ButtonText = ButtonText, .ImageURL = ImageURL, .CallBackPlainDoc = CallBack}
      mCustomButtonList.Add(eb)
      Return eb
    End Function

    Protected Function AddCustomButton(ButtonText As String, ImageURL As String, CallBack As Func(Of Singular.Message)) As CustomButton
      Dim eb As New CustomButton With {.ButtonText = ButtonText, .ImageURL = ImageURL, .CallBackMsg = CallBack}
      mCustomButtonList.Add(eb)
      Return eb
    End Function

    Protected Function AddCustomButton(ButtonText As String, ImageURL As String, CallBack As Func(Of Object)) As CustomButton
      Dim eb As New CustomButton With {.ButtonText = ButtonText, .ImageURL = ImageURL, .CallBackObj = CallBack}
      mCustomButtonList.Add(eb)
      Return eb
    End Function

    Public Function ExecCustomButton(ButtonID As Guid) As Object Implements IReport.ExecCustomButton
      For Each cb As CustomButton In mCustomButtonList
        If cb.ButtonID = ButtonID Then
          Return cb.Invoke
        End If
      Next
      Return Nothing
    End Function

#End Region

    <System.ComponentModel.Browsable(False)>
    Public Overridable ReadOnly Property LinkTargetType As Integer Implements IReport.LinkTargetType
      Get
        Return 1 '_blank
      End Get
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Overridable ReadOnly Property ShowWordExport As Boolean Implements IReport.ShowWordExport
      Get
        Return Singular.Reporting.ShowWordExport
      End Get
    End Property

    Private mReportCriteria As RC
    Public Sub New()

    End Sub

    Public Sub New(Criteria As RC)
      SetCriteria(Criteria)
    End Sub

    Protected Friend Overridable Sub Setup() Implements IReport.Setup

    End Sub

    <Browsable(False)> _
    Public ReadOnly Property ReportCriteria As RC
      Get
        If mReportCriteria Is Nothing Then
          mReportCriteria = Activator.CreateInstance(GetType(RC))
          mReportCriteria.ReportParent = Me
          Setup()
        End If
        Return mReportCriteria
      End Get
    End Property

    Public ReadOnly Property ReportCriteriaGeneric As ReportCriteria Implements IReport.ReportCriteriaGeneric
      Get
        Return ReportCriteria
      End Get
    End Property

    Public Sub SetCriteria(Criteria As RC)
      mReportCriteria = Criteria
      mReportCriteria.ReportParent = Me
      ResetData()
    End Sub

    Public Overridable ReadOnly Property Description As String Implements IReport.Description
      Get
        Return ""
      End Get
    End Property

#If SILVERLIGHT = False Then

    ''' <summary>
    ''' Use this to set the name of the sql command. The parameters are set automatically.
    ''' </summary>
    Protected MustOverride Sub SetupCommandProc(cmd As Singular.CommandProc)

    ''' <summary>
    ''' Override this if your report has a crystal report. 
    ''' </summary>
    <Browsable(False)> _
    Public Overridable ReadOnly Property CrystalReportType As Type Implements IReport.CrystalReportType
      Get
        Return Nothing
      End Get
    End Property

    <Browsable(False)> _
    Public Overridable ReadOnly Property CustomCriteriaControlType As Type Implements IReport.CustomCriteriaControlType
      Get
        Return Nothing
      End Get
    End Property

    ''' <summary>
    ''' Override this if your report must not have a Totals Row. 
    ''' </summary>
    <Browsable(False)> _
    Public Overridable ReadOnly Property ShowTotalsRow As Boolean
      Get
        Return True
      End Get
    End Property

    ''' <summary>
    ''' Override if you want to change any data / structure of the dataset.
    ''' </summary>
    Protected Overridable Sub ModifyDataSet(DataSet As DataSet)

    End Sub

    Protected Overridable Sub ModifyExcelWorkbook(Workbook As Infragistics.Documents.Excel.Workbook)

    End Sub

    Protected Overridable Function CreateExcelWorkbook(Data As DataSet) As Infragistics.Documents.Excel.Workbook

      Dim ee As New Singular.Data.ExcelExporter
      ee.ShowTotalsRow = ShowTotalsRow
      ee.PopulateData(Data)
      ModifyExcelWorkbook(ee.WorkBook)
      Return ee.WorkBook

    End Function

    Private mDataSet As DataSet
    '<NonSerialized()> 
    Private mCrystalReport As CrystalDecisions.CrystalReports.Engine.ReportClass
    Private mDataSourceIsOutOfDate As Boolean

    <Browsable(False)> _
    Public ReadOnly Property DataSet As DataSet Implements IReport.DataSet
      Get
        Return mDataSet
      End Get
    End Property

    ''' <summary>
    ''' Forces the data to be re-fetched the next time the report is shown.
    ''' </summary>
    Public Sub ResetData() Implements IReport.ResetData
      mDataSet = Nothing
    End Sub

    <Browsable(False)>
    Public Property ModifyDataSetHandler As Action(Of ModifyDataSetArgs)

    Public Sub FetchData(Optional DocumentType As ReportDocumentType = ReportDocumentType.ExcelData)

      If mDataSet Is Nothing Then

        Dim cProc As New Singular.CommandProc()
        cProc.CommandType = CommandType.StoredProcedure

        'Auto populate the parameters based on the report criteria.
        Dim InfoTable = ReportCriteria.AddParameters(cProc)

        mDataSet = GetDataSource()

        If mDataSet Is Nothing Then

          'Let the report object set the command text, alter any parameter values etc.
          SetupCommandProc(cProc)

          'Execute the stored proc.
          cProc.FetchType = Singular.CommandProc.FetchTypes.DataSet
          cProc = cProc.Execute()

          mDataSet = cProc.Dataset
          mDataSet.DataSetName = ReportName.Replace(" ", "")

        End If


        If InfoTable IsNot Nothing AndAlso InfoTable.Rows.Count = 1 Then
          mDataSet.Tables.Add(InfoTable)
        End If

        If ModifyDataSetHandler Is Nothing Then
          ModifyDataSet(mDataSet)
        Else
          ModifyDataSetHandler.Invoke(New ModifyDataSetArgs() With {.DataSet = mDataSet,
                                                                    .DocumentType = DocumentType})
        End If

      End If

    End Sub

    Public Overridable Function GetDataSource() As DataSet
      Return Nothing
    End Function

    ''' <summary>
    ''' Indicates that the data source in the crystal report does not match the data source returned from the database.
    ''' New columns and tables wont trigger this, only if the crystal report is missing a field, or the field type is different.
    ''' </summary>
    <Browsable(False)>
    Public ReadOnly Property DataSourceIsOutOfDate As Boolean Implements IReport.DataSourceIsOutOfDate
      Get
        Return mDataSourceIsOutOfDate
      End Get
    End Property

    Private Sub SetupCrystalReport(Optional ExistingReport As CrystalDecisions.CrystalReports.Engine.ReportClass = Nothing)

      If ExistingReport IsNot Nothing Then
        mCrystalReport = ExistingReport
      End If

      mDataSourceIsOutOfDate = Not Reporting.SetupCrystalReport(Me, mCrystalReport)

    End Sub

    Public Overridable Function GetData() As DataSet Implements IReport.GetData
      FetchData()
      Return mDataSet
    End Function

    ''' <summary>
    ''' Fetches the report dataset if needed, sets up the crystal report, and returns it as a pdf file in a memory stream
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAsDocument(DocumentType As ReportDocumentType) As IO.MemoryStream Implements IReport.GetAsDocument
      FetchData(DocumentType)

      If ReportHasData() Then
        If DocumentType = ReportDocumentType.PDF Or DocumentType = ReportDocumentType.Word Then
          SetupCrystalReport()

          If mCrystalReport IsNot Nothing AndAlso Not mDataSourceIsOutOfDate Then

            Dim pdfStream = mCrystalReport.ExportToStream(DocumentType)
            Dim ms As New IO.MemoryStream
            pdfStream.CopyTo(ms)
            Return ms
          End If
        ElseIf DocumentType = ReportDocumentType.ExcelData Then

          Dim ms As New IO.MemoryStream
          CreateExcelWorkbook(mDataSet).Save(ms)
          Return ms

        End If
      End If

      Return Nothing
    End Function

    Public Overridable Function GetDocumentFile(DocumentType As ReportDocumentType) As ReportFileInfo Implements IReport.GetDocumentFile

      Dim rfi As New ReportFileInfo

      rfi.FileStream = GetAsDocument(DocumentType)
      rfi.FileName = GetExportFileName() & GetFileExtension(DocumentType)

      If DataSourceIsOutOfDate Then
        rfi.FileStream = GetDataSetSchemaFile()
        rfi.FileName = GetExportFileName() & ".xsd"
        rfi.IsXSD = True
      End If

      Return rfi

    End Function

    Public Function GetFileExtension(DocumentType As ReportDocumentType) As String
      Select Case DocumentType
        Case Singular.Reporting.ReportDocumentType.PDF
          Return ".pdf"
        Case Singular.Reporting.ReportDocumentType.Word
          Return ".doc"
        Case Singular.Reporting.ReportDocumentType.ExcelData
          Return ".xlsx"
      End Select
      Return ""
    End Function

#Region " Multiple file processing "

    ''' <summary>
    ''' Executes the stored proc for the current criteria, and creates multiple documents based on the group value in GetGroupColumnSelector. 
    ''' A zip file containing a document per group is returned.
    ''' </summary>
    ''' <param name="DocumentType">Type of file to create</param>
    ''' <param name="ZipFileName">Name of Zip file including / excluding extension.</param>
    ''' <param name="GetGroupColumnSelector">Return the column that contains the values you want to split by.</param>
    ''' <param name="GetFileName">Return the file name based on the group. Function is run for each group. Current row, and group value is passed in.</param>
    Public Function GetAsMultipleFiles(DocumentType As ReportDocumentType,
                                       ZipFileName As String,
                                       GetGroupColumnSelector As Func(Of DataSet, DataColumn),
                                       GetFileName As Func(Of DataRow, String, String)) As Singular.Documents.Document

      ZipFileName = IO.Path.GetFileNameWithoutExtension(ZipFileName) & ".zip"

      Dim Files = GetAsMultipleFiles(DocumentType, GetGroupColumnSelector, GetFileName)

      Return Singular.Documents.Document.CreateZipFile(ZipFileName, Files)

    End Function

    ''' <summary>
    ''' Executes the stored proc for the current criteria, and creates multiple documents based on the group value in GetGroupColumnSelector. 
    ''' A List of documents is returned.
    ''' </summary>
    ''' <param name="DocumentType">Type of file to create</param>
    ''' <param name="GetGroupColumnSelector">Return the column that contains the values you want to split by.</param>
    ''' <param name="GetFileName">Return the file name based on the group. Function is run for each group. Current row, and group value is passed in.</param>
    Public Function GetAsMultipleFiles(DocumentType As ReportDocumentType,
                                       GetGroupColumnSelector As Func(Of DataSet, DataColumn),
                                       GetFileName As Func(Of DataRow, String, String)) As List(Of Singular.Documents.IDocument)

      Dim List As New List(Of Singular.Documents.IDocument)

      GetAsMultipleFiles(
        DocumentType,
        GetGroupColumnSelector,
        Sub(DocData, GroupedRow, GroupValue)

          List.Add(New Singular.Documents.Document(GetFileName(GroupedRow, GroupValue) & GetFileExtension(DocumentType), DocData.ToArray))

        End Sub)

      Return List

    End Function

    ''' <summary>
    ''' Executes the stored proc for the current criteria, and creates multiple documents based on the group value in GetGroupColumnSelector. 
    ''' The OnDocumentCreated callback is executed for each document created.
    ''' </summary>
    ''' <param name="DocumentType">Type of file to create</param>
    ''' <param name="GetGroupColumnSelector">Return the column that contains the values you want to split by.</param>
    ''' <param name="OnDocumentCreated">Callback with the document data, grouped row, and group value.</param>
    Public Sub GetAsMultipleFiles(DocumentType As ReportDocumentType,
                                  GetGroupColumnSelector As Func(Of DataSet, DataColumn),
                                  OnDocumentCreated As Action(Of IO.MemoryStream, DataRow, String))

      FetchData(DocumentType)

      Dim GroupColumn = GetGroupColumnSelector(mDataSet)

      Dim OriginalDataSet = mDataSet
      mDataSet = mDataSet.Clone
      Dim GroupIndex As New Concurrent.ConcurrentDictionary(Of String, List(Of DataRow))

      'Find the group values
      For Each Row As DataRow In GroupColumn.Table.Rows

        Dim GroupValue As String = Row(GroupColumn).ToString
        Dim Rows = GroupIndex.GetOrAdd(GroupValue, Function()
                                                     Return New List(Of DataRow)
                                                   End Function)
        Rows.Add(Row)

      Next

      'Copy data unrelated to the group table
      For Each Table As DataTable In OriginalDataSet.Tables
        If Table.ParentRelations.Count = 0 AndAlso Table IsNot GroupColumn.Table Then
          mDataSet.Tables(Table.TableName).Merge(Table)
        End If
      Next

      Dim GroupTable = mDataSet.Tables(GroupColumn.Table.TableName)
      Dim List As New List(Of Singular.Documents.IDocument)

      'Create a new report for each group
      For Each Group In GroupIndex

        Singular.Data.DataTables.ClearDataTableAndChildTables(GroupTable)
        Singular.Data.DataTables.CopyDataTableAndChildTables(Group.Value.ToArray, GroupTable)

        Using DocData = GetAsDocument(DocumentType)
          DocData.Position = 0
          OnDocumentCreated(DocData, Group.Value.First, Group.Key)
        End Using

      Next

    End Sub

#End Region

    ''' <summary>
    ''' Returns the schema definition of the data source
    ''' </summary>
    Public Function GetDataSetSchemaFile() As IO.MemoryStream Implements IReport.GetDatasetSchemaFile
      Dim DataSetSchemaFile = New IO.MemoryStream
      mDataSet.WriteXmlSchema(DataSetSchemaFile)
      Return DataSetSchemaFile
    End Function

    Private mTablesToIgnore As New List(Of String)
    <Browsable(False)>
    Public Property TablesToIgnore As List(Of String)
      Get
        Return mTablesToIgnore
      End Get
      Set(ByVal value As List(Of String))
        mTablesToIgnore = value
      End Set
    End Property

    Public Overridable Function ReportHasData() As Boolean Implements IReport.ReportHasData

      Dim ds As DataSet = mDataSet

      Dim hasData As Boolean = False
      If ds IsNot Nothing Then
        If ds.Tables.Count = 0 Then
          hasData = False
        Else
          For Each table As DataTable In ds.Tables
            If table.Rows.Count > 0 And table.TableName <> "Information" AndAlso Not TablesToIgnore.Contains(table.TableName) Then
              hasData = True
              Exit For
            End If
          Next
        End If
      End If

      Return hasData

    End Function

    Public Sub SetDataSet(NewDataSet As DataSet)
      mDataSet = NewDataSet
    End Sub

#End If

#Region "IDisposable Support"
    Private DisposedValue As Boolean = False

    Protected Overridable Sub Dispose(disposing As Boolean)
      If Not Me.DisposedValue Then

        Try
          If mCrystalReport IsNot Nothing Then
            mCrystalReport.Close()
            mCrystalReport.Dispose()
          End If
        Catch ex As Exception

        End Try
      End If
      Me.DisposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
      Dispose(False)
      MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub
#End Region

  End Class

#End Region

End Namespace


