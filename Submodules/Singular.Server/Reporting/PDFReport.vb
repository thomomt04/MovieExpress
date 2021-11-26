Imports Infragistics.Documents
Imports Singular.Data.DataTables

Namespace Reporting

  Public Class PDFReportSettings

    Public Property Font As String = "Calibri"

    ''' <summary>
    ''' Main Heading at the top of the report. Usually report name
    ''' </summary>
    Public Property MainHeaderStyle As Reports.Report.Text.Style

    ''' <summary>
    ''' Sub Heading at the top of the report. Usually report criteria.
    ''' </summary>
    Public Property SubHeaderStyle As Reports.Report.Text.Style

    ''' <summary>
    ''' Normal text of data in the report. 
    ''' </summary>
    Public Property NormalStyle As Reports.Report.Text.Style

    Public Property TableHeaderStyle As Reports.Report.Text.Style
    Public Property GroupHeaderStyle As Reports.Report.Text.Style

    ''' <summary>
    ''' Small text like page numbers, print date etc.
    ''' </summary>
    Public Property SmallStyle As Reports.Report.Text.Style

    ''' <summary>
    ''' Orientation of the pages in the report. By default, the number of columns in the data will determine whether the report is portrait or landscape. 
    ''' </summary>
    Public Property Orientation As Reports.Report.Preferences.Printing.PaperOrientation = Reports.Report.Preferences.Printing.PaperOrientation.Auto

    ''' <summary>
    ''' Padding left and right for cells.
    ''' </summary>
    Public Property ColumnPadding As Integer = 1

    Public Function Copy() As PDFReportSettings
      Return Me.MemberwiseClone
    End Function

    Friend Sub Setup()

      If MainHeaderStyle Is Nothing Then
        MainHeaderStyle = New Reports.Report.Text.Style(New Reports.Graphics.Font(Font, 16, Reports.Graphics.FontStyle.Bold), Reports.Graphics.Brushes.Black)
      End If
      If SubHeaderStyle Is Nothing Then
        SubHeaderStyle = New Reports.Report.Text.Style(New Reports.Graphics.Font(Font, 11), Reports.Graphics.Brushes.Black)
      End If
      If TableHeaderStyle Is Nothing Then
        TableHeaderStyle = New Reports.Report.Text.Style(New Reports.Graphics.Font(Font, 10, Reports.Graphics.FontStyle.Bold), Reports.Graphics.Brushes.Black)
      End If
      If GroupHeaderStyle Is Nothing Then
        GroupHeaderStyle = New Reports.Report.Text.Style(New Reports.Graphics.Font(Font, TableHeaderStyle.Font.Size + 1, Reports.Graphics.FontStyle.Bold), Reports.Graphics.Brushes.Black)
      End If
      If NormalStyle Is Nothing Then
        NormalStyle = New Reports.Report.Text.Style(New Reports.Graphics.Font(Font, 10), Reports.Graphics.Brushes.Black)
      End If
      If SmallStyle Is Nothing Then
        SmallStyle = New Reports.Report.Text.Style(New Reports.Graphics.Font(Font, 9), Reports.Graphics.Brushes.Black)
      End If

    End Sub

  End Class

  Public Class PDFReport

    Private Shared mGlobalSettings As New PDFReportSettings
    Private mSettings As PDFReportSettings
    Private mReport As Reports.Report.Report
    Private mCurrentSection As Reports.Report.Section.ISection
    Private mCurrentBand As Reports.Report.Band.IBand
    Private mCurrentPageWidth As Single
    Private mGroupFinished As Boolean = False
    Private mInformationTable As DataTable

    Private mDataset As DataSet

#Region " Public Settings "

    ''' <summary>
    ''' If the dataset has more than 1 root table, and you want each table to appear in the report, specify the extra page headings here.
    ''' </summary>
    Public Property SectionTitles As String() = {}

    ''' <summary>
    ''' Gets the settings for this instance.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Settings As PDFReportSettings
      Get
        Return mSettings
      End Get
    End Property

    ''' <summary>
    ''' Gets the global report settings. All reports will be created with these settings.
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property GlobalSettings As PDFReportSettings
      Get
        Return mGlobalSettings
      End Get
    End Property

#End Region

    Public Sub New(ReportTitle As String)

      mSettings = mGlobalSettings.Copy

      mReport = New Infragistics.Documents.Reports.Report.Report()
      mReport.Info.Title = ReportTitle
      mReport.Info.Author = "Singular Systems (Pty) Ltd"
      mReport.Info.Creator = "Singular Systems (Pty) Ltd"

    End Sub

    Public Sub PopulateData(Dataset As DataSet)

      mSettings.Setup()
      'AddExtendedProperties(Dataset)

      'Find the information table
      For Each Table As DataTable In Dataset.Tables
        If Table.ExtendedProperties(ExtendedProperties.ReportCriteria.ToString) IsNot Nothing Then
          mInformationTable = Table
          Exit For
        End If
      Next

      Dim mAddedCount As Integer = -1
      'Add a section for each root table.
      For Each Table As DataTable In Dataset.Tables
        If Table.ExtendedProperties(ExtendedProperties.ReportCriteria.ToString) Is Nothing AndAlso Table.ParentRelations.Count = 0 Then

          If SectionTitles.Length > mAddedCount Then
            If mAddedCount = -1 Then
              CreateSection(Table, mReport.Info.Title)
            Else
              CreateSection(Table, SectionTitles(mAddedCount))
            End If
          End If

          mAddedCount += 1
        End If

      Next

    End Sub

    Private Function GetMaxWidth(DataTable As DataTable) As Integer
      Dim TotalWidth As Integer = 0
      Dim HasChildren As Boolean = DataTable.ChildRelations.Count > 0 AndAlso DataTable.ChildRelations(0).ChildTable.ExtendedProperties("NoPrint") Is Nothing

      For Each Column As DataColumn In DataTable.Columns
        If GetExtProperty(Column, "SummaryColumn", False) AndAlso HasChildren Then
          Column.ExtendedProperties(ExtendedProperties.AutoGenerate.ToString) = False
        Else
          TotalWidth += ColumnWidth(Column)
        End If
      Next
      Dim ChildWidth As Integer = 0
      If HasChildren Then
        ChildWidth = GetMaxWidth(DataTable.ChildRelations(0).ChildTable)
      End If
      Return Math.Max(TotalWidth, ChildWidth)
    End Function

    Private Sub CreateSection(DataTable As DataTable, Title As String)

      'Determine the orientation
      Dim TotalWidth As Integer = 0
      Dim SectionOrientation As Reports.Report.Preferences.Printing.PaperOrientation

      TotalWidth = GetMaxWidth(DataTable)
      If mSettings.Orientation = Reports.Report.Preferences.Printing.PaperOrientation.Auto Then
        If TotalWidth > 1000 Then
          SectionOrientation = Reports.Report.Preferences.Printing.PaperOrientation.Landscape
        Else
          SectionOrientation = Reports.Report.Preferences.Printing.PaperOrientation.Portrait
        End If
      End If

      If mCurrentSection Is Nothing Then
        'Set the report preferences if this is the first section
        mReport.Preferences.Printing.PaperSize = Reports.Report.Preferences.Printing.PaperSize.A4
        mReport.Preferences.Printing.PaperOrientation = SectionOrientation
        mReport.Preferences.Printing.FitToMargins = True
      End If

      'Create the section
      mCurrentSection = mReport.AddSection
      With mCurrentSection
        .PageMargins = New Infragistics.Documents.Reports.Report.Margins(10, 10, 10, 20)
        .PageNumbering.Template = "Page [Page #] of [TotalPages]"
        .PageNumbering.Style = mSettings.SmallStyle
        .PageNumbering.SkipFirst = False
        .PageNumbering.OffsetY = -10
        .PageNumbering.OffsetX = 10
        .PageOrientation = SectionOrientation

        If SectionOrientation = Reports.Report.Preferences.Printing.PaperOrientation.Landscape Then
          .PageSize = New Reports.Report.PageSize(.PageSize.Height, .PageSize.Width)
        End If
        mCurrentPageWidth = mCurrentSection.PageSize.Width - 20

        With .AddFooter
          .Height = 10
          With .Shapes.AddLine
            .Pen = Reports.Graphics.Pens.Black
            .X1 = 0
            .X2 = mCurrentPageWidth
            .Y1 = 5
            .Y2 = 5
          End With
        End With

      End With

      'Create headings
      CreateHeading(Title)

      'Create band
      mCurrentBand = mCurrentSection.AddBand()

      'Create table
      DataTable.ExtendedProperties("NewPageAfter") = True

      CreateTable(Nothing, DataTable.Columns, DataTable.Select(Nothing, DataTable.DefaultView.Sort), 0, True)

    End Sub

    Private Sub CreateHeading(Title As String)

      Dim Top As Integer = 0

      With mCurrentSection.AddHeader()
        .Repeat = False

        'Main report heading
        With .AddText(0, Top)
          '.Margins.Bottom = 5
          .Style = mSettings.MainHeaderStyle
          .AddContent(Title)
          .Alignment.Vertical = Reports.Report.Alignment.Top
          .Height = New Reports.Report.FixedHeight(25)
          Top += 25
        End With

        'Criteria
        If mInformationTable IsNot Nothing Then

          'Check if there is a main start / end date
          Dim StartDateCol As DataColumn = Nothing
          Dim EndDateCol As DataColumn = Nothing
          For Each Column As DataColumn In mInformationTable.Columns
            If Column.ColumnName = "StartDate" Then
              StartDateCol = Column
              Column.ExtendedProperties(ExtendedProperties.AutoGenerate.ToString) = False
            End If
            If Column.ColumnName = "EndDate" Then
              EndDateCol = Column
              Column.ExtendedProperties(ExtendedProperties.AutoGenerate.ToString) = False
            End If
          Next

          Dim DateCriteriaString As String = ""
          If StartDateCol IsNot Nothing AndAlso EndDateCol IsNot Nothing Then
            DateCriteriaString = "From " & CDate(mInformationTable.Rows(0)(StartDateCol)).ToString("dd MMM yyyy") & " to " & CDate(mInformationTable.Rows(0)(EndDateCol)).ToString("dd MMM yyyy")
          ElseIf EndDateCol IsNot Nothing Then
            DateCriteriaString = "As at " & CDate(mInformationTable.Rows(0)(EndDateCol)).ToString("dd MMM yyyy")
          ElseIf StartDateCol IsNot Nothing Then
            DateCriteriaString = "From " & CDate(mInformationTable.Rows(0)(StartDateCol)).ToString("dd MMM yyyy")
          End If

          If DateCriteriaString <> "" Then
            With .AddText(0, Top)
              .Style = mSettings.SubHeaderStyle
              .AddContent(DateCriteriaString)
              .Height = New Reports.Report.FixedHeight(15)
            End With
            Top += 15
          End If

          'Add general criteria
          For Each Column As DataColumn In mInformationTable.Columns
            If GetExtProperty(Column, ExtendedProperties.AutoGenerate.ToString, True) Then

              With .AddText(0, Top)
                .Style = mSettings.SubHeaderStyle
                .AddContent(Column.Caption)
                .Height = New Reports.Report.FixedHeight(15)
              End With
              With .AddText(70, Top)
                .Style = mSettings.SubHeaderStyle
                .AddContent(mInformationTable.Rows(0)(Column).ToString)
                .Height = New Reports.Report.FixedHeight(15)
              End With

              Top += 15
            End If
          Next
        End If

        Top += 5
        With .Shapes.AddLine()
          .Pen = New Reports.Graphics.Pen(Reports.Graphics.Colors.Black, 3)
          .X1 = 0
          .X2 = mCurrentPageWidth
          .Y1 = Top
          .Y2 = Top
        End With

        .Height = Top + 10
      End With

    End Sub

    Private mFooterMargin As Integer = 0
    Private mLowestColumns As DataColumnCollection
    Private mTotals As New Stack(Of Decimal())

    Private Sub AddTotalsLevel()
      Dim Totals(99) As Decimal
      For i As Integer = 0 To 99
        Totals(i) = 0
      Next
      mTotals.Push(Totals)
    End Sub

    Private Function PopTotals() As Decimal()
      Dim Totals = mTotals.Pop
      If mTotals.Count > 0 Then
        Dim ParentTotals = mTotals(0)
        For i As Integer = 0 To Totals.Length - 1
          ParentTotals(i) += Totals(i)
        Next
      End If
      Return Totals
    End Function

    Private Function CreateTable(Container As Reports.Report.Grid.IGridCell, Columns As DataColumnCollection, Rows() As DataRow,
                                 ParentMargin As Integer, AddTotals As Boolean) As Reports.Report.Grid.IGrid

      AddTotalsLevel()

      Dim PreviousRow As Reports.Report.Grid.IGridRow
      Dim ColumnCount As Integer = 0
      Dim TotalColumnWidth As Integer = 0
      Dim WidthRatio As Decimal
      Dim HasChildGrid As Boolean = Columns(0).Table.ChildRelations.Count > 0 AndAlso Columns(0).Table.ChildRelations(0).ChildTable.ExtendedProperties("NoPrint") Is Nothing

      Dim MarginLeft As Integer = If(Container Is Nothing Or Not HasChildGrid, 0, 10)

      Dim Grid As Reports.Report.Grid.IGrid
      If Container Is Nothing Then
        Grid = mCurrentBand.AddGrid
      Else
        Grid = Container.AddGrid
      End If

      With Grid
        .Margins.Left = MarginLeft

        'Get the total column width
        For Each Column As DataColumn In Columns
          TotalColumnWidth += ColumnWidth(Column)
        Next
        WidthRatio = (mCurrentPageWidth - MarginLeft - ParentMargin) / TotalColumnWidth
        Columns(0).Table.ExtendedProperties("WidthRatio") = WidthRatio

        'Add the columns
        For Each Column As DataColumn In Columns
          If AutoGenerateColumn(Column) Then
            Dim GridCol = .AddColumn

            GridCol.Width = New Reports.Report.FixedWidth(ColumnWidth(Column) * WidthRatio)
            ColumnCount += 1
          End If
        Next

        'Add the header cells
        If Not HasChildGrid Then

          'No child grids, so add the proper headers
          For Each Column As DataColumn In Columns
            If AutoGenerateColumn(Column) Then
              With .Header.AddCell
                .Borders.Bottom = New Reports.Report.Border(Reports.Graphics.Pens.Black)
                .Paddings.Left = mSettings.ColumnPadding
                .Paddings.Right = mSettings.ColumnPadding
                .Paddings.Top = 1
                .Paddings.Bottom = 1
                With .AddText
                  .Style = mSettings.TableHeaderStyle

                  .Alignment.Horizontal = GetExtProperty(Column, ExtendedProperties.TextAlign.ToString, 0)
                  .AddContent(Column.Caption)
                End With
              End With

            End If
          Next
          .Header.Margins.Bottom = 3
          .Header.Repeat = True

        End If

        If Not HasChildGrid Then
          mLowestColumns = Columns
        End If

        'Add the rows
        For Each Row As DataRow In Rows
          Dim CurrentRow = .AddRow

          Dim GroupText As String = ""
          If HasChildGrid AndAlso mGroupFinished Then
            CurrentRow.Margins.Top = 10
          End If
          mGroupFinished = False

          'Add the cells
          Dim ColumnIndex As Integer = 0
          For Each Column As DataColumn In Columns
            If AutoGenerateColumn(Column) Then

              With CurrentRow.AddCell
                .Paddings.Left = mSettings.ColumnPadding
                .Paddings.Right = mSettings.ColumnPadding

                If HasChildGrid Then
                  .Background = New Reports.Report.Background(Reports.Graphics.Brushes.WhiteSmoke)
                  .Paddings.All = 2
                End If

                Dim Text = .AddText
                With Text
                  If HasChildGrid Then
                    .Style = mSettings.GroupHeaderStyle
                  Else
                    .Style = mSettings.NormalStyle
                  End If

                  'Set the formatted value
                  Dim Value As Object = Row(Column)
                  If HasChildGrid AndAlso TypeOf Value Is Boolean Then
                    Value = If(Value = True, "", "Not ") & Column.Caption
                  End If

                  Dim StringValue = SetCellValue(Text, Value, Column)

                  If GetExtProperty(Column, "GroupColumn", False) Then
                    GroupText &= StringValue & " "
                  End If

                  'Add to totals
                  If Value IsNot Nothing AndAlso mLowestColumns IsNot Nothing AndAlso mLowestColumns(0).Table Is Column.Table AndAlso (Column.DataType Is GetType(Decimal) OrElse Column.DataType Is GetType(Integer)) Then
                    If Not Singular.Misc.IsNullNothing(Value) Then
                      mTotals(0)(Column.Ordinal) += Value
                    End If
                  End If

                End With
              End With

              ColumnIndex += 1
            End If
          Next

          If HasChildGrid Then
            'Add child grid
            Dim ChildColumns = Row.Table.ChildRelations(0).ChildTable.Columns
            Dim ChildContainerRow = .AddRow
            ChildContainerRow.Margins.Top = 5
            CurrentRow.KeepWithNext = True
            Dim ChildContainerCell = ChildContainerRow.AddCell
            ChildContainerCell.ColSpan = ColumnCount

            Dim ChildGrid = CreateTable(ChildContainerCell, ChildColumns, Row.GetChildRows(Row.Table.ChildRelations(0)), ParentMargin + MarginLeft, GroupText = "")

            'Add totals row for child grid
            If GroupText <> "" Then
              AddTotalsRow(Grid, "Totals for " & GroupText, ColumnCount)
            End If

            If Container IsNot Nothing Then
              mFooterMargin += 10
            End If

          Else
            mFooterMargin = 0
          End If

          PreviousRow = CurrentRow

        Next

        'Footer
        If AddTotals Then

          AddTotalsRow(Grid, If(Container Is Nothing, "Grand Totals", "Totals"), ColumnCount)

        End If

        mGroupFinished = True

      End With

      Return Grid
    End Function

    Private Sub AddTotalsRow(Grid As Reports.Report.Grid.IGrid, TotalsLabelText As String, Span As Integer)

      Dim Totals = PopTotals()

      With Grid.AddRow
        .Margins.Top = 3
        .Margins.Bottom = 3

        Dim ChildWidthRatio As Decimal = mLowestColumns(0).Table.ExtendedProperties("WidthRatio")

        With .AddCell
          .ColSpan = Span
          .Borders.Top = New Reports.Report.Border(Reports.Graphics.Pens.Black)

          With .AddTable()
            With .AddRow

              Dim TotalLabel = .AddCell
              Dim FirstTotal As Boolean = True
              With TotalLabel

                With .AddText
                  .Style = mSettings.TableHeaderStyle
                  .AddContent(TotalsLabelText)
                End With
              End With

              Dim Position As Integer = 0
              Dim Gap As Integer = 0
              For Each ChildCol As DataColumn In mLowestColumns
                If AutoGenerateColumn(ChildCol) Then
                  Dim ColWidth = ColumnWidth(ChildCol) * ChildWidthRatio
                  If Position > 200 AndAlso (ChildCol.DataType Is GetType(Decimal) OrElse ChildCol.DataType Is GetType(Integer)) Then

                    If FirstTotal Then
                      TotalLabel.Width = New Reports.Report.FixedWidth(Position + mFooterMargin)
                      FirstTotal = False
                      Gap = 0
                    End If
                    With .AddCell
                      .Width = New Reports.Report.FixedWidth((ColumnWidth(ChildCol) * ChildWidthRatio) + Gap)
                      Dim TotalText = .AddText
                      With TotalText
                        .Style = mSettings.TableHeaderStyle
                        SetCellValue(TotalText, Totals(ChildCol.Ordinal), ChildCol)

                      End With
                    End With

                    Gap = 0
                  Else
                    Gap += ColWidth
                  End If
                  Position += ColWidth

                End If
              Next

            End With
          End With

        End With

      End With

    End Sub

    Private Function SetCellValue(Text As Reports.Report.Text.IText, Value As Object, Column As DataColumn) As String

      Text.Alignment.Horizontal = GetExtProperty(Column, ExtendedProperties.TextAlign.ToString, 0)

      'Set the formatted value
      Dim StringValue As String = Singular.Data.DataTables.GetCellDisplayValue(Value, Column)

      Text.AddContent(StringValue)
      Return StringValue
    End Function

    Public Function GetStream() As IO.MemoryStream

      Using ms As New IO.MemoryStream
        mReport.Publish(ms, Reports.Report.FileFormat.PDF)
        Return ms
      End Using
    End Function

  End Class

End Namespace
