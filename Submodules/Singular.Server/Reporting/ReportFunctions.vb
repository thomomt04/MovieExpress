Namespace Reporting

  Public Module ReportSettings

    Public Property ShowWordExport As Boolean = True
    Public Property GridExportEnabledByDefault As Boolean = False
    Public Property ListToXML_UseOldMethod As Boolean = False

  End Module

  Public Module ReportFunctions

    ''' <summary>
    ''' Maps Dot Net types to Crystal Report Field Types.
    ''' </summary>
    Public Function GetCrystalReportDataType(DotNetType As Type) As CrystalDecisions.Shared.FieldValueType

      If DotNetType Is GetType(String) Then
        Return CrystalDecisions.Shared.FieldValueType.StringField
      ElseIf DotNetType Is GetType(Decimal) OrElse DotNetType Is GetType(Double) Then
        Return CrystalDecisions.Shared.FieldValueType.NumberField
      ElseIf DotNetType Is GetType(Int32) Then
        Return CrystalDecisions.Shared.FieldValueType.Int32sField
      ElseIf DotNetType Is GetType(Int16) Then
        Return CrystalDecisions.Shared.FieldValueType.Int16sField
      ElseIf DotNetType Is GetType(Byte) Then
        Return CrystalDecisions.Shared.FieldValueType.Int8sField
      ElseIf DotNetType Is GetType(Date) Then
        Return CrystalDecisions.Shared.FieldValueType.DateTimeField
      Else
        Return CrystalDecisions.Shared.FieldValueType.UnknownField
      End If

    End Function

    Private LockObject As New Object
    Private ReportQueue As New Queue(Of CrystalDecisions.CrystalReports.Engine.ReportClass)
    ''' <summary>
    ''' Keeps track of the report, and when the job limit is exceeded, disposes the report if it hasn't been disposed already.
    ''' </summary>
    Public Sub RegisterReportForDisposal(Report As CrystalDecisions.CrystalReports.Engine.ReportClass)
      SyncLock LockObject

        If ReportQueue.Count > 70 Then
          Dim rpt = ReportQueue.Dequeue
          Try
            rpt.Close()
            rpt.Dispose()
          Catch ex As Exception

          End Try

        End If
        ReportQueue.Enqueue(Report)
      End SyncLock
    End Sub

    Public Function SetupCrystalReport(Report As IReport, ByRef CrystalReport As CrystalDecisions.CrystalReports.Engine.ReportClass) As Boolean

      If Report.CrystalReportType Is Nothing AndAlso CrystalReport Is Nothing Then
        Return False
      End If

      Dim ReportCreated As Boolean = False

      'create an instance of the crystal report object, and set its data source.
      If CrystalReport Is Nothing Then
        CrystalReport = Activator.CreateInstance(Report.CrystalReportType)
        RegisterReportForDisposal(CrystalReport)
        ReportCreated = True
      End If

      Dim ParamValues(CrystalReport.ParameterFields.Count - 1) As Object
      Dim MaxI As Integer
      Dim i As Integer = 0

      If ReportCreated Then

        For Each pf As CrystalDecisions.Shared.ParameterField In CrystalReport.ParameterFields
          If pf.CurrentValues.Count > 0 Then
            ParamValues(i) = CType(pf.CurrentValues(i), CrystalDecisions.Shared.ParameterDiscreteValue).Value
            i += 1
          End If
        Next
        MaxI = i

        'Check if the crystal report data source matches the structure of the supplied data source
        If CrystalReport.Database.Tables.Count = 0 AndAlso Report.DataSet.Tables.Count > 0 Then
          Return False
        End If
        For Each tbl As CrystalDecisions.CrystalReports.Engine.Table In CrystalReport.Database.Tables
          'If tbl.Name <> "Information" Then
          If Not Report.DataSet.Tables.Contains(tbl.Name) Then
              Return False
            Else
              For Each col As CrystalDecisions.CrystalReports.Engine.FieldDefinition In tbl.Fields
                If col.Kind = CrystalDecisions.Shared.FieldKind.DatabaseField Then
                  If Not Report.DataSet.Tables(tbl.Name).Columns.Contains(col.Name) Then
                    Return False
                  Else
                    Dim cRDataType = col.ValueType
                    Dim dsDataType = GetCrystalReportDataType(Report.DataSet.Tables(tbl.Name).Columns(col.Name).DataType)
                    If dsDataType <> CrystalDecisions.Shared.FieldValueType.UnknownField AndAlso cRDataType <> dsDataType Then
                      Return False
                    End If
                  End If
                End If
              Next
            End If
          'End If
        Next
      End If

      CrystalReport.SetDataSource(Report.DataSet)

      If ReportCreated Then

        For Each t As CrystalDecisions.CrystalReports.Engine.Table In CrystalReport.Database.Tables
          t.SetDataSource(Report.DataSet)
        Next

        For Each sr As CrystalDecisions.CrystalReports.Engine.ReportDocument In CrystalReport.Subreports
          For Each t As CrystalDecisions.CrystalReports.Engine.Table In sr.Database.Tables
            t.SetDataSource(Report.DataSet)
          Next
        Next

        i = 0
        For i = 0 To MaxI - 1
          Dim pv As New CrystalDecisions.Shared.ParameterDiscreteValue()
          pv.Value = ParamValues(i)
          CrystalReport.ParameterFields(i).CurrentValues.Add(pv)
        Next

      End If

      Return True

    End Function

    ''' <summary>
    ''' The Project specific report Report Hierarchy your UI is using.
    ''' </summary>
    Public Property ProjectReportHierarchy As ReportHierarchy

#Region " Report Hash IDs "

    Private mReportToHashList As Dictionary(Of Type, String)
    Private mHashToReportList As Dictionary(Of String, Type)

    Public Function GetHash(ReportType As Type) As String

      FindAllReports()
      Return mReportToHashList(ReportType)

    End Function

    Public Function GetReportType(Hash As String) As Type

      FindAllReports()
      Return mHashToReportList(Hash.ToLower)

    End Function

    ''' <summary>
    ''' Creates a hash code for all reports in the report assembly. Used on web to hide the report assembly names from the user.
    ''' </summary>
    Private Sub FindAllReports()

      If ProjectReportHierarchy Is Nothing Then
        Throw New Exception("ReportAssemblyType in Reports Library is not set.")
      End If

      If mReportToHashList Is Nothing Then

        mReportToHashList = New Dictionary(Of Type, String)
        mHashToReportList = New Dictionary(Of String, Type)()

        For Each type In System.Reflection.Assembly.GetAssembly(ProjectReportHierarchy.GetType).GetTypes()
          If Singular.Reflection.TypeImplementsInterface(type, GetType(IReport)) Then

            Dim Hash As String = Singular.Encryption.GetStringHash(type.AssemblyQualifiedName, Singular.Encryption.HashType.Sha256, True, , True)
            mReportToHashList.Add(type, Hash)
            mHashToReportList.Add(Hash, type)

          End If
        Next

      End If

    End Sub

#End Region

  End Module


End Namespace

