Imports System.Reflection
Imports Singular.Web.Controls.HelperControls

Namespace CustomControls.SGrid

  Public Class GridReportContainer
    Inherits GridContainer(Of Singular.Web.Reporting.GridReportVM)

    Public Sub New()
      MyBase.New(Function(c) c.GridInfo)
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      Helpers.Control(New GridHelpers)

    End Sub

    Protected Overrides Sub NoDataSourceSection(Helpers As HelperAccessors(Of SGridInfo))

      With Helpers.Div
        .AddClass("SGrid-Toolbar")

        .Helpers.HTMLTag("p", "<strong>No Report Selected...</strong>")

        With .Helpers.HTMLTag("p")
          .Helpers.LinkFor(, , Utils.URL_ToAbsolute("~/"), "Click here")
          .Helpers.HTML(" to go to the home page.")
        End With

      End With

    End Sub

  End Class

  Public Class GridContainer(Of ObjectType)
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of ObjectType, SGridInfo)

    Public Property Grid As SGrid
    Public Property Toolbar As HTMLDiv(Of SGridInfo)

    Public Sub New(GridInfoProperty As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
      [For](GridInfoProperty)
    End Sub

    Public Property BeforeAddLayoutDropDown As Action(Of HelperAccessors(Of SGridInfo))
    Public Property SaveLayoutButtonBeforeDropDown As Boolean = False
    Public Property BeforeAddButtons As Action(Of HelperAccessors(Of SGridInfo))
    Public Property AfterAddButtons As Action(Of HelperAccessors(Of SGridInfo))

    Public LoadViewContainer As HTMLTag(Of SGridInfo)
    Public SaveLayoutButton As Button(Of SGridInfo)
    Public ExportButton As Button(Of SGridInfo)
    Public PrintButton As Button(Of SGridInfo)
    Public GraphButton As Button(Of SGridInfo)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers.Div
        .AddClass("ReportContainer")
        .AddClass("SimplePopup")

        With .Helpers.Div
          .AddBinding(KnockoutBindingString.with, GetForJS)

          Toolbar = .Helpers.Div
          With Toolbar
            .AddClass("SGrid-Toolbar")
            .IsVisible = Function(c) c.ShowHeader

            With .Helpers.If("!HasLoaded()")
              'Loading...
              With .Helpers.Div
                .AddClass("sg-Loading")
                .AddBinding(KnockoutBindingString.text, "LoadingText")
              End With
            End With
            With .Helpers.If("HasLoaded()")
              'After load

              If BeforeAddLayoutDropDown IsNot Nothing Then
                BeforeAddLayoutDropDown()(.Helpers)
              End If

              Dim OldStyle As ButtonStyle = Singular.Web.Controls.DefaultButtonStyle
              If SaveLayoutButtonBeforeDropDown Then
                Try
                  Singular.Web.Controls.DefaultButtonStyle = ButtonStyle.Bootstrap
                  SaveLayoutButton = .Helpers.Button("Save Layout")
                  With SaveLayoutButton
                    .AddBinding(KnockoutBindingString.visible, Function(c) c.AllowSaveLayout)
                    .AddBinding(Singular.Web.KnockoutBindingString.click, "Singular.SGrid.ShowSaveLayout($data)")
                    .ButtonSize = ButtonSize.Small
                    .Image.Glyph = FontAwesomeIcon.image
                    .Style.MarginRight("20px")
                  End With
                Finally
                  Singular.Web.Controls.DefaultButtonStyle = OldStyle
                End Try

              End If

              'Layout drop down if multiple
              LoadViewContainer = .Helpers.HTMLTag("span")
              With LoadViewContainer
                .AddBinding(KnockoutBindingString.visible, "ShowLayoutSelect()")
                .Helpers.HTMLTag("span", "Load view:").Style.MarginRight("5px")
                With .Helpers.HTMLTag("select")
                  .Attributes("data-bind") = "options: LayoutList(), optionsValue: 'LayoutName', optionsText: 'LayoutName', value: SelectedLayoutName"
                  .Style.Width = 250
                End With
              End With

              'Layout name if only 1.
              With .Helpers.Div
                .AddBinding(KnockoutBindingString.visible, "!ShowLayoutSelect()")
                .AddBinding(KnockoutBindingString.text, "FriendlyName()")
                .AddClass("ReportName")
                .Style.Display = Display.inlineblock
              End With

              Try
                Singular.Web.Controls.DefaultButtonStyle = ButtonStyle.Bootstrap

                If BeforeAddButtons IsNot Nothing Then
                  BeforeAddButtons()(.Helpers)
                End If

                If Not SaveLayoutButtonBeforeDropDown Then
                  SaveLayoutButton = .Helpers.Button("Save Layout")
                  With SaveLayoutButton
                    .AddBinding(KnockoutBindingString.visible, Function(c) c.AllowSaveLayout)
                    .AddBinding(Singular.Web.KnockoutBindingString.click, "Singular.SGrid.ShowSaveLayout($data)")
                    .ButtonSize = ButtonSize.Small
                    .Image.Glyph = FontAwesomeIcon.image
                    .Style.MarginLeft("20px")
                  End With
                End If

                ExportButton = .Helpers.Button("Export", "Export")
                With ExportButton
                  .AddBinding(KnockoutBindingString.visible, Function(c) c.AllowDataExport)
                  .AddBinding(KnockoutBindingString.click, "Export(1)")
                  .AddBinding(KnockoutBindingString.enable, "!Exporting()")
                  .ButtonText.AddBinding(KnockoutBindingString.text, "ExportText")

                  .ButtonSize = ButtonSize.Small
                  .Image.Glyph = FontAwesomeIcon.grid9
                  .ButtonStyle = ButtonMainStyle.Success
                  .Style.MarginLeft("20px")
                End With

                PrintButton = .Helpers.Button("Print", "Print")
                With PrintButton
                  .AddBinding(KnockoutBindingString.visible, Function(c) c.AllowPrint)
                  .AddBinding(KnockoutBindingString.click, "Export(2)")
                  .AddBinding(KnockoutBindingString.enable, "!Exporting()")
                  .ButtonText.AddBinding(KnockoutBindingString.text, "PrintText")

                  .ButtonStyle = ButtonMainStyle.Warning
                  .ButtonSize = ButtonSize.Small
                  .Image.Glyph = FontAwesomeIcon.file
                  .Style.MarginLeft("20px")
                End With

                GraphButton = .Helpers.Button("Graph", "Build Chart")
                With GraphButton
                  .AddBinding(KnockoutBindingString.visible, Function(c) c.AllowGraph)
                  .ButtonText.AddBinding(KnockoutBindingString.text, "CurrentLayoutInfo().ChildList.length == 0 ? 'Build Chart' : 'View Chart'")
                  .AddBinding(Singular.Web.KnockoutBindingString.click, "ShowChart()")

                  .ButtonStyle = ButtonMainStyle.Danger
                  .ButtonSize = ButtonSize.Small
                  .Image.Glyph = FontAwesomeIcon.areaChart
                  .Style.MarginLeft("20px")
                End With

                If AfterAddButtons IsNot Nothing Then
                  AfterAddButtons()(.Helpers)
                End If

              Finally
                Singular.Web.Controls.DefaultButtonStyle = OldStyle
              End Try

            End With

          End With

          With .Helpers.If("!HasLoaded()")
            'Loading...
            With .Helpers.Div
              .AddBinding(KnockoutBindingString.text, "LoadingStatus")
              .Style.Margin("10px", , , "20px")
            End With
          End With

          Grid = New SGrid("$data")
          Grid.Style.Margin("5px", "5px", "1px", "5px")
          .Helpers.Control(Grid)

        End With

        With .Helpers.Div
          .AddBinding(KnockoutBindingString.visible, "!ko.utils.unwrapObservable(" & GetForJS() & ")")
          NoDataSourceSection(.Helpers)
        End With



      End With

    End Sub

    Protected Overridable Sub NoDataSourceSection(Helpers As HelperAccessors(Of SGridInfo))

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

  Public Class GridHelpers
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of Object)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      Helpers.Control(Of SGridHelpers)()
      Helpers.Control(Of SGridChartBuilder)()
    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

  Public Class SGrid
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of Object)


    Public Shared ExportAsTable As Boolean = True

    Public Property Width As Integer
    Public Property Height As Integer

    Private mDataSource As String

    ''' <summary>
    ''' Datasource must be the address of a JS function.
    ''' The function must return an object with the following properties:
    ''' Data: the data to show in the grid
    ''' (Optional):
    ''' UniqueKey: unique name in case the layout of the grid is going to be saved.
    ''' LayoutInfo: The band / column layout of the grid.
    ''' Schema: The schema of the datasource.
    ''' AutoHeight: The height of the grid will be (window height - auto height) when resizing the browser.
    ''' </summary>
    Public Sub New(Datasource As String)
      mDataSource = Datasource
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      Attributes.Add("width", Width)
      Attributes.Add("height", Height)
      Attributes.Add("tabindex", 0)
      Bindings.Add(Singular.Web.KnockoutBindingString.SGrid, mDataSource)

      WriteFullStartTag("canvas", Singular.Web.Controls.HelperControls.TagType.Normal)
      Writer.WriteEndTag("canvas")
    End Sub

  End Class

  Public Class SGridServer

    Public Property Bands As New List(Of Band)
    Friend DataSet As DataSet

    Private Function GetBandIndex(Band As Band) As Integer
      For i As Integer = 0 To Bands.Count - 1
        If Bands(i) Is Band Then
          Return i
        End If
      Next
      Return -1
    End Function

    Public Class Band

      Public Grid As SGridServer
      Public Property ID As Integer
      Public ParentBand As Band
      Public ShowInExport As Boolean = True
      Public ShowInReport As Boolean = True
      'Public ChildBands As List(Of Band)

      Public GroupByDataBand As Band

      Public ParentRelation As DataRelation
      Public Table As DataTable
      Private IDColumn As DataColumn
      Private ParentIDColumn As DataColumn

      Public Property GroupColumns As New List(Of GroupByColumn)
      Public Property SummaryColumns As New List(Of SummaryColumn)
      Public Property Columns As New List(Of Column)

      Public Sub New(Grid As SGridServer, ParentBand As Band, Table As DataTable)
        Me.ParentBand = ParentBand
        Me.Table = Table
        Me.Grid = Grid

        For Each Col As DataColumn In Table.Columns
          Columns.Add(New Column(Col))
        Next

        IDColumn = New DataColumn("_SGridRowGuid", GetType(Guid))
        IDColumn.AllowDBNull = True
        IDColumn.ExtendedProperties("AutoGenerate") = False
        Table.Columns.Add(IDColumn)

        ParentIDColumn = New DataColumn("_SGridParentGuid", GetType(Guid))
        ParentIDColumn.AllowDBNull = True
        ParentIDColumn.ExtendedProperties("AutoGenerate") = False
        Table.Columns.Add(ParentIDColumn)

        If ParentBand IsNot Nothing AndAlso Table.ParentRelations.Count > 0 Then
          ParentRelation = Table.ParentRelations(0)
        End If

        For Each row As DataRow In Table.Rows
          row(IDColumn) = Guid.NewGuid
          If ParentRelation IsNot Nothing Then
            row(ParentIDColumn) = row.GetParentRow(ParentRelation)(ParentBand.IDColumn)
          End If
        Next

      End Sub

      Friend Function GetNewRow() As DataRow
        Dim drw As DataRow = Table.NewRow
        drw(IDColumn) = Guid.NewGuid
        Return drw
      End Function

      Friend Sub SetParent(Parent As Band)
        ParentBand = Parent
        If Parent IsNot Nothing Then
          ParentRelation = New DataRelation("", Parent.IDColumn, ParentIDColumn, False)
          Grid.DataSet.Relations.Add(ParentRelation)
        End If

      End Sub

      Friend Enum GroupRestoreType
        NA = 0
        GroupBy = 1
        Summary = 2
      End Enum

      Friend Sub LoadLayout(DBand As Object, DataBand As Band, GroupType As GroupRestoreType)

        ID = DBand.ID

        'Set the column properties
        If DBand.Columns IsNot Nothing Then
          For Each Col As Object In DBand.Columns

            'Dim DataColumn As DataColumn = Nothing
            Dim SColumn As Column = Nothing

            If DataBand Is Nothing Then
              'Existing band
              'DataColumn = Table.Columns(CStr(Col.Key))
              SColumn = Columns.Find(Function(c)
                                       Return c.Key = Col.Key
                                     End Function)
              SColumn.LoadLayout(Col)

            Else
              'Group by band
              Dim OrigCol As DataColumn = Nothing
              If Col.OrigKey IsNot Nothing Then
                OrigCol = DataBand.Table.Columns(CStr(Col.OrigKey))
              End If

              If Col.SummaryType IsNot Nothing AndAlso GroupType = GroupRestoreType.Summary Then
                'if cross tab, then create each summary column based on the child data.
                If Col.CTKey IsNot Nothing Then

                  Dim AddedCount As Integer = 0
                  Dim AddedValues As New Hashtable
                  Dim CTCol As DataColumn = DataBand.Table.Columns(CStr(Col.CTKey))
                  For Each row As DataRow In Table.Rows
                    Dim ChildRows As DataRow() = Singular.Data.DataTables.GetSortedChildRows(row, Table.ChildRelations(0))
                    For Each crow As DataRow In ChildRows
                      Dim Value = crow(CTCol)
                      If AddedCount < 15 AndAlso AddedValues(Value) Is Nothing Then
                        AddedCount += 1
                        AddedValues(Value) = True
                        Col.FValue = Value
                        Col.FType = 1
                        Col.FKey = CTCol.ColumnName
                        Dim SCTCol As SummaryColumn = CreateSummaryColumn(Col, OrigCol)
                        SCTCol.DataColumn.Caption = Singular.Data.DataTables.GetCellDisplayValue(Value, CTCol) & " " & OrigCol.Caption
                        Columns.Add(SCTCol)
                      End If


                    Next

                  Next


                Else
                  SColumn = CreateSummaryColumn(Col, OrigCol)
                End If


              ElseIf Col.SummaryType Is Nothing AndAlso GroupType = GroupRestoreType.GroupBy Then
                SColumn = CreateGroupColumn(Col, OrigCol)
              End If

              If SColumn IsNot Nothing Then
                Columns.Add(SColumn)
                SColumn.LoadLayout(Col)
              End If

            End If

          Next
        End If

        If DataBand Is Nothing Then
          ApplyFilters()
          Table.AcceptChanges()
        End If

        For Each gbc As GroupByColumn In GroupColumns
          If gbc.DisplayColumn IsNot Nothing Then
            gbc.DisplayColumn.Caption = gbc.DataColumn.Caption
          End If
        Next

        'Set the column order
        If DBand.ColOrder IsNot Nothing Then
          For i As Integer = 0 To DBand.ColOrder.length - 1
            Dim Column As DataColumn = Table.Columns(CStr(DBand.ColOrder(i)))
            If Column IsNot Nothing Then
              Column.SetOrdinal(i)
            End If
          Next
        End If

        'Style
        If DBand.Colors IsNot Nothing Then
          Table.ExtendedProperties("ColorInfo") = New Singular.Data.ExcelExporter.TableColorInfo(
            System.Drawing.ColorTranslator.FromHtml(DBand.Colors(0)),
            System.Drawing.ColorTranslator.FromHtml(DBand.Colors(1)),
            System.Drawing.ColorTranslator.FromHtml(DBand.Colors(2)))
        End If
        If DBand.NoExport Then
          ShowInExport = False
          Table.ExtendedProperties("NoExport") = True
        End If
        If DBand.NoPrint Then
          ShowInReport = False
          Table.ExtendedProperties("NoPrint") = True
        End If

        'Load child band Layouts
        If DBand.Children IsNot Nothing Then

        End If

        If DataBand Is Nothing Then
          'Group by

          Dim GroupByInfo As Object = DBand.GroupBy
          Dim GroupByList As New List(Of Object)
          'Dim NextGroupByInfo As Object = Nothing

          While (GroupByInfo IsNot Nothing)
            If GroupByInfo.DataBandID = ID Then
              GroupByList.Insert(0, GroupByInfo)
            End If
            GroupByInfo = GroupByInfo.GroupBy
          End While

          CreateGroupByBands(GroupByList, Me)

        End If

      End Sub

      Private Sub CreateGroupByBands(GroupByList As List(Of Object), DataBand As Band)
        'create the group by bands.
        For Each GroupByInfo As Object In GroupByList
          Dim GroupTable As New DataTable()
          Grid.DataSet.Tables.Add(GroupTable)
          Dim GroupedBand As New Band(Grid, DataBand.ParentBand, GroupTable)
          GroupedBand.GroupByDataBand = DataBand
          GroupedBand.LoadLayout(GroupByInfo, DataBand, GroupRestoreType.GroupBy)
          GroupByInfo.Band = GroupedBand
          DataBand.CreateGrouping(GroupedBand)
        Next
        'now go through the list in reverse to create the summaries.
        GroupByList.Reverse()
        Dim LastBand As Band = DataBand
        For Each GroupByInfo As Object In GroupByList
          Dim Band As Band = GroupByInfo.Band
          Band.LoadLayout(GroupByInfo, LastBand, GroupRestoreType.Summary)
          Band.ReSummarise(True)

          If (GroupByInfo.GroupBy IsNot Nothing AndAlso GroupByInfo.GroupBy.DataBandID = Band.ID) Then
            'group by of group by
            Dim gb = GroupByInfo.GroupBy
            Dim dband As Band = GroupByInfo.Band
            Dim GroupTable As New DataTable()
            Grid.DataSet.Tables.Add(GroupTable)
            Band = New Band(Grid, dband.ParentBand, GroupTable)
            Band.GroupByDataBand = dband
            Band.LoadLayout(gb, dband, GroupRestoreType.GroupBy)
            dband.CreateGrouping(Band)
            Band.LoadLayout(gb, dband, GroupRestoreType.Summary)
            Band.ReSummarise(True)
          End If

          LastBand = Band
        Next

      End Sub

      Private Sub CreateGrouping(NewBand As Band)

        Dim Value As Object
        Dim LastParentGUID As Guid = Guid.Empty
        Dim CurrentParentGUID As Guid = Guid.NewGuid
        Dim ParentHT As New Hashtable
        Dim GroupHT As Hashtable = Nothing

        Table.ParentRelations.Remove(ParentRelation)

        For Each row As DataRow In Table.Rows

          'Get the group by value
          If NewBand.GroupColumns.Count = 1 Then
            Value = NewBand.GroupColumns(0).GetTransformedValue(row)
          Else
            Value = ""
            For Each Col As GroupByColumn In NewBand.GroupColumns
              Value &= Col.GetTransformedValue(row).ToString & ChrW(29)
            Next
          End If

          'Get the correct hashtable for the parent row.
          If ParentBand IsNot Nothing Then
            CurrentParentGUID = row(ParentIDColumn)
          End If
          If LastParentGUID = Guid.Empty OrElse LastParentGUID <> CurrentParentGUID Then
            LastParentGUID = CurrentParentGUID
            GroupHT = ParentHT(LastParentGUID)
            If GroupHT Is Nothing Then
              GroupHT = New Hashtable
              ParentHT(LastParentGUID) = GroupHT
            End If
          End If

          'Get the row and create a new row if the group value hasnt been added.
          Dim drw As DataRow = GroupHT(Value)
          If drw Is Nothing Then
            drw = NewBand.GetNewRow
            If NewBand.ParentBand IsNot Nothing Then
              drw(NewBand.ParentIDColumn) = CurrentParentGUID
            End If

            For Each Col As GroupByColumn In NewBand.GroupColumns
              drw(Col.DataColumn) = Col.LastTransformedValue
              'Check if the group value has an alternate display. e.g. number band (0-10, 10-20 etc).
              If Col.OtherValue IsNot Nothing Then
                drw(Col.DisplayColumn) = Col.OtherValue
              End If

            Next
            NewBand.Table.Rows.Add(drw)

            GroupHT(Value) = drw
          End If

          'This rows parent will now be the group row
          row(ParentIDColumn) = drw(NewBand.IDColumn)

        Next

        'Insert the new band level before the current band.
        Grid.Bands.Insert(Grid.GetBandIndex(Me), NewBand)

        NewBand.SetParent(ParentBand)
        SetParent(NewBand)

      End Sub

      Private Sub ReSummarise(Sort As Boolean)

        For Each row As DataRow In Table.Rows

          Dim ChildRows As DataRow() = Singular.Data.DataTables.GetSortedChildRows(row, Table.ChildRelations(0))

          'Initialise
          For Each SCol As SummaryColumn In SummaryColumns
            SCol.Calculator.Init()
          Next

          'Summarise
          For Each CRow As DataRow In ChildRows
            For Each SCol As SummaryColumn In SummaryColumns


              If SCol.Filter IsNot Nothing Then
                If Not SCol.Filter.Match(CRow(SCol.FilterColumn)) Then
                  Continue For
                End If
              End If

              If SCol.OriginalColumn Is Nothing Then
                SCol.Calculator.Add(Nothing)
              Else
                SCol.Calculator.Add(CRow(SCol.OriginalColumn))
              End If

            Next

          Next

          'Finalise
          For Each SCol As SummaryColumn In SummaryColumns
            row(SCol.DataColumn) = SCol.Calculator.Final
          Next

        Next

        ApplyFilters()

      End Sub

      'Public Shared Sub PopulateCalculatedFields(Table As DataTable)

      '  Dim s As New Singular.Web.Data.JS.ServerJSSerialiser(Table)
      '  s.GetJSon()

      '  For Each row As DataRow In Table.Rows

      '    Dim Engine As New Jint.Engine

      '    Dim jsObj As New Jint.Native.Object.ObjectInstance(Engine)

      '    For Each col As DataColumn In Table.Columns

      '      jsObj.FastAddProperty(col.ColumnName, New Jint.Native.JsValue(CStr(row(col))), False, False, False)

      '    Next

      '    Engine.SetValue("CalcObj", jsObj)

      '    Engine.SetValue("OnComplete", Sub(Result)

      '                                    Dim x As Integer = 0

      '                                  End Sub)

      '    Dim js As String = "OnComplete(CalcObj.CashTransactionType + ' ' + CalcObj.ShareHolderName);"

      '    Engine.Execute(js)

      '  Next



      'End Sub

      Private Sub ApplyFilters()

        For Each Col As Column In Columns
          Col.ApplyFilters()
        Next

      End Sub

      Private Function CreateNormalColumn(DataColumn As DataColumn) As Column

        Return New Column(DataColumn)

      End Function

      Private Function CreateSummaryColumn(Col As Object, OrigColumn As DataColumn) As SummaryColumn

        If OrigColumn Is Nothing AndAlso Col.SummaryType <> SummaryType.Count Then
          'needs to be fixed
          Return Nothing
        Else
          Dim Key = If(Col.SummaryType = SummaryType.Count, "Count", CStr(Col.Key))
          If Col.CTKey IsNot Nothing Then
            'Cross tab columns get a random key since they cant be grouped on.
            Key = Guid.NewGuid.ToString
          End If
          Dim SummaryColumnDataType As Type = OrigColumn.DataType
          If OrigColumn.DataType = GetType(Integer) Then SummaryColumnDataType = GetType(Int64)
          Dim SCol As New SummaryColumn(Table.Columns.Add(Key, If(Col.SummaryType = SummaryType.Count Or Col.SummaryType = SummaryType.DistinctCount, GetType(Integer), SummaryColumnDataType)))

          SCol.SummaryType = Col.SummaryType
          SCol.OriginalColumn = OrigColumn
          If Col.FKey IsNot Nothing Then

            SCol.FilterColumn = OrigColumn.Table.Columns(CStr(Col.FKey))
            SCol.Filter = New ColumnFilter(Col.FType, Col.FValue)

          End If

          SCol.DataColumn.ExtendedProperties("SummaryColumn") = True

          SummaryColumns.Add(SCol)
          Return SCol
        End If

      End Function

      Private Function CreateGroupColumn(Col As Object, OrigColumn As DataColumn) As GroupByColumn
        Dim Key = OrigColumn.ColumnName & "_G" & Col.TransformID
        Dim GBCol = New GroupByColumn(Table.Columns.Add(Key, OrigColumn.DataType))

        GBCol.TransformType = Col.TransformID
        GBCol.TransformParam = Col.TransformParam

        GBCol.DataColumn.ExtendedProperties("GroupColumn") = True
        GBCol.OriginalColumn = OrigColumn
        GroupColumns.Add(GBCol)
        Return GBCol
      End Function

    End Class

    Public Enum FilterType
      Equals = 1
      NotEqual = 2
      StartsWith = 100
      Contains = 101
      EndsWith = 102
      DoesntStartWith = 103
      DoesntContain = 104
      DoesntEndWith = 105
      LessThan = 200
      LessThanOrEqual = 201
      GreaterThan = 202
      GreaterThanOrEqual = 203
    End Enum

    Public Enum FilterOperator
      [And] = 1
      [Or] = 2
    End Enum

    Public Class ColumnFilter
      Private _Type As FilterType
      Private _FilterValue As Object
      Private _FilterIsNull As Boolean
      Private _FilterLooksLikeNull As Boolean

      Public ReadOnly Property FilterValue As Object
        Get
          Return _FilterValue
        End Get
      End Property

      Public Sub New(Type As FilterType, FilterValue As Object)
        _Type = Type
        _FilterValue = FilterValue

        _FilterIsNull = _FilterValue Is DBNull.Value OrElse _FilterValue Is Nothing
        _FilterLooksLikeNull = TypeOf _FilterValue Is String AndAlso String.IsNullOrEmpty(_FilterValue)
      End Sub

      Friend Function Match(Value As Object) As Boolean

        If _Type = FilterType.Equals OrElse _Type = FilterType.NotEqual Then

          'Only equal and not equal operators can be used when comparing nulls
          Return NullSafeCompare(Value) = (_Type = FilterType.Equals)

        ElseIf _FilterIsNull OrElse Value Is DBNull.Value OrElse Value Is Nothing Then

          'If there is a null here, return false because the below comparisons don't handle nulls.
          Return False

        End If

        Select Case _Type
          Case FilterType.StartsWith
            Return CStr(Value).StartsWith(_FilterValue)
          Case FilterType.Contains
            Return CStr(Value).Contains(_FilterValue)
          Case FilterType.EndsWith
            Return CStr(Value).EndsWith(_FilterValue)
          Case FilterType.DoesntStartWith
            Return Not CStr(Value).StartsWith(_FilterValue)
          Case FilterType.DoesntContain
            Return Not CStr(Value).Contains(_FilterValue)
          Case FilterType.DoesntEndWith
            Return Not CStr(Value).EndsWith(_FilterValue)

          Case FilterType.LessThan
            Return Value < _FilterValue
          Case FilterType.LessThanOrEqual
            Return Value <= _FilterValue
          Case FilterType.GreaterThan
            Return Value > _FilterValue
          Case FilterType.GreaterThanOrEqual
            Return Value >= _FilterValue
        End Select

        Return False
      End Function

      Private Function NullSafeCompare(Value As Object) As Boolean

        Dim LooksLikeNull = TypeOf Value Is String AndAlso String.IsNullOrEmpty(Value)
        Dim IsNull = Value Is Nothing OrElse Value Is DBNull.Value

        If Not IsNull AndAlso Not _FilterIsNull Then
          'Normal compare
          Return Value = _FilterValue
        ElseIf (IsNull OrElse LooksLikeNull) AndAlso (_FilterIsNull OrElse _FilterLooksLikeNull) Then
          'Both are null
          Return True
        Else
          'One value is null and the other isnt
          Return False
        End If

      End Function

      Friend Sub ConvertDateValue()
        _FilterValue = New Date(1970, 1, 1).AddMilliseconds(_FilterValue).Add(TimeZone.CurrentTimeZone.GetUtcOffset(Now))
      End Sub

    End Class

    Public Class Column

      Protected mDataColumn As DataColumn
      Friend Key As String

      Public Sub New(DataColumn As DataColumn)
        mDataColumn = DataColumn
        Key = mDataColumn.ColumnName

        SetDefaults()
      End Sub

      Public ReadOnly Property DataColumn As DataColumn
        Get
          Return mDataColumn
        End Get
      End Property

      Public Property HeaderText As String
        Get
          Return mDataColumn.Caption
        End Get
        Set(value As String)
          mDataColumn.Caption = value
        End Set
      End Property

      Friend Property Filters As New List(Of ColumnFilter)
      Friend Property FilterOperator As FilterOperator = SGridServer.FilterOperator.And

      Friend Sub SetDefaults()

        Singular.Data.DataTables.AddExtendedInfo(DataColumn)

      End Sub

      Friend Sub LoadLayout(ColLayout As Object)

        'Visible
        If Not Singular.Dynamic.GetValue(ColLayout, "Visible", True) Then
          DataColumn.ExtendedProperties("AutoGenerate") = False
        End If
        'Header text
        DataColumn.Caption = Singular.Dynamic.GetValue(ColLayout, "HeaderText", DataColumn.Caption)
        'Sort
        Dim SortDirection = Singular.Dynamic.GetValue(ColLayout, "SortDirection", -1)
        If SortDirection >= 0 Then
          DataColumn.ExtendedProperties("SortDirection") = SortDirection
          Dim Direction = If(SortDirection = 0, " ASC", " DESC")
          DataColumn.Table.DefaultView.Sort = DataColumn.ColumnName & Direction
        End If

        'Width
        Dim Width As Object = Singular.Dynamic.GetValue(ColLayout, Singular.Data.DataTables.ExtendedProperties.Width.ToString, Nothing)
        If Width IsNot Nothing Then
          DataColumn.ExtendedProperties(Singular.Data.DataTables.ExtendedProperties.Width.ToString) = Width
        End If
        'Alignment
        Dim TextAlign As Object = Singular.Dynamic.GetValue(ColLayout, Singular.Data.DataTables.ExtendedProperties.TextAlign.ToString, Nothing)
        If TextAlign IsNot Nothing Then
          DataColumn.ExtendedProperties(Singular.Data.DataTables.ExtendedProperties.TextAlign.ToString) = If(TextAlign = "left", 0, If(TextAlign = "center", 1, 2))
        End If
        'Format
        Dim FormatString As Object = Singular.Dynamic.GetValue(ColLayout, Singular.Data.DataTables.ExtendedProperties.FormatString.ToString, Nothing)
        If FormatString IsNot Nothing Then
          DataColumn.ExtendedProperties(Singular.Data.DataTables.ExtendedProperties.FormatString.ToString) = FormatString
        End If

        'Filters
        If ColLayout.Filters IsNot Nothing Then
          For Each Filter As Object In ColLayout.Filters
            Dim f As New ColumnFilter(Filter.Type, Filter.Value)

            If Me.DataColumn.DataType Is GetType(Date) AndAlso TypeOf (Filter.Value) Is Long Then
              f.ConvertDateValue()
            End If
            Filters.Add(f)
          Next
          FilterOperator = ColLayout.FilterOperator
        End If

      End Sub

      Friend Sub ApplyFilters()

        If Filters.Count > 0 Then

          For i As Integer = DataColumn.Table.Rows.Count - 1 To 0 Step -1
            Dim Row = DataColumn.Table.Rows(i)

            Dim MatchCount As Integer = 0
            Dim Value = Row(DataColumn)

            If DataColumn.DataType Is GetType(String) AndAlso Value IsNot DBNull.Value AndAlso Value IsNot Nothing Then
              Value = CStr(Value).ToLower
            End If

            For Each Filter As ColumnFilter In Filters

              If Filter.Match(Value) Then
                MatchCount += 1
              End If

            Next

            If If(FilterOperator = SGridServer.FilterOperator.And, MatchCount < Filters.Count, MatchCount = 0) Then
              Row.Delete()
            End If

          Next

        End If

      End Sub

    End Class

#Region " Summary Column / Calculators "

    Public Class SummaryColumn
      Inherits Column

      Public Sub New(DataColumn As DataColumn)
        MyBase.New(DataColumn)
      End Sub

      Private mSummaryType As SummaryType
      Public Property SummaryType As SummaryType
        Get
          Return mSummaryType
        End Get
        Set(value As SummaryType)
          mSummaryType = value
          mCalculator = SummaryCalculator.GetCalculator(mSummaryType)
        End Set
      End Property

      Private mCalculator As SummaryCalculator
      Public ReadOnly Property Calculator As SummaryCalculator
        Get
          Return mCalculator
        End Get
      End Property

      Public OriginalColumn As DataColumn
      Public Filter As ColumnFilter
      Public FilterColumn As DataColumn

    End Class



    Public MustInherit Class SummaryCalculator

      Protected Class CalculatorState
        Public State As Object
        Public Counter As Integer
        Public Values As New Hashtable
      End Class

      Protected mCalcObj As New CalculatorState

      Public MustOverride Sub Init()
      Public MustOverride Sub Add(Value As Object)
      Public MustOverride Function Final() As Object

      Public Shared Function GetCalculator(Type As SummaryType)
        Select Case Type
          Case SummaryType.Sum
            Return New SumCalculator
          Case SummaryType.Average
            Return New AvgCalculator
          Case SummaryType.Min
            Return New MinCalculator
          Case SummaryType.Max
            Return New MaxCalculator
          Case SummaryType.First
            Return New FirstCalculator
          Case SummaryType.Last
            Return New LastCalculator
          Case SummaryType.DistinctCount
            Return New DCountCalculator
          Case SummaryType.Count
            Return New CountCalculator
        End Select
        Return Nothing
      End Function
    End Class

    Public Class SumCalculator
      Inherits SummaryCalculator

      Public Overrides Sub Add(Value As Object)
        mCalcObj.State += If(Value Is DBNull.Value, 0, Value)
      End Sub

      Public Overrides Function Final() As Object
        Return mCalcObj.State
      End Function

      Public Overrides Sub Init()
        mCalcObj.State = 0
      End Sub
    End Class

    Public Class AvgCalculator
      Inherits SumCalculator

      Public Overrides Sub Add(Value As Object)
        MyBase.Add(Value)
        mCalcObj.Counter += 1
      End Sub

      Public Overrides Function Final() As Object
        Return If(mCalcObj.Counter = 0, 0, mCalcObj.State / mCalcObj.Counter)
      End Function

      Public Overrides Sub Init()
        MyBase.Init()
        mCalcObj.Counter = 0
      End Sub
    End Class

    Public Class MinCalculator
      Inherits SummaryCalculator

      Public Overrides Sub Add(Value As Object)
        If Value IsNot DBNull.Value Then
          mCalcObj.State = If(mCalcObj.State Is Nothing OrElse Value < mCalcObj.State, Value, mCalcObj.State)
        End If
      End Sub

      Public Overrides Function Final() As Object
        Return mCalcObj.State
      End Function

      Public Overrides Sub Init()
        mCalcObj.State = Nothing
      End Sub
    End Class

    Public Class MaxCalculator
      Inherits SummaryCalculator

      Public Overrides Sub Add(Value As Object)
        If Value IsNot DBNull.Value Then
          mCalcObj.State = If(mCalcObj.State Is Nothing OrElse Value > mCalcObj.State, Value, mCalcObj.State)
        End If
      End Sub

      Public Overrides Function Final() As Object
        Return mCalcObj.State
      End Function

      Public Overrides Sub Init()
        mCalcObj.State = Nothing
      End Sub
    End Class

    Public Class FirstCalculator
      Inherits SummaryCalculator

      Public Overrides Sub Add(Value As Object)
        If mCalcObj.State Is Nothing AndAlso Value IsNot DBNull.Value Then
          mCalcObj.State = Value
        End If
      End Sub

      Public Overrides Function Final() As Object
        Return mCalcObj.State
      End Function

      Public Overrides Sub Init()
        mCalcObj.State = Nothing
      End Sub
    End Class

    Public Class LastCalculator
      Inherits SummaryCalculator

      Public Overrides Sub Add(Value As Object)
        If Value IsNot DBNull.Value Then
          mCalcObj.State = Value
        End If
      End Sub

      Public Overrides Function Final() As Object
        Return mCalcObj.State
      End Function

      Public Overrides Sub Init()
        mCalcObj.State = Nothing
      End Sub
    End Class

    Public Class DCountCalculator
      Inherits SummaryCalculator

      Public Overrides Sub Add(Value As Object)
        If Not mCalcObj.Values.ContainsKey(Value) Then
          mCalcObj.Values(Value) = True
          mCalcObj.Counter += 1
        End If
      End Sub

      Public Overrides Function Final() As Object
        Return mCalcObj.Counter
      End Function

      Public Overrides Sub Init()
        mCalcObj.Counter = 0
        mCalcObj.Values = New Hashtable
      End Sub
    End Class

    Public Class CountCalculator
      Inherits SummaryCalculator

      Public Overrides Sub Add(Value As Object)
        mCalcObj.Counter += 1
      End Sub

      Public Overrides Function Final() As Object
        Return mCalcObj.Counter
      End Function

      Public Overrides Sub Init()
        mCalcObj.Counter = 0
      End Sub
    End Class

#End Region





    Public Class GroupByColumn
      Inherits Column

      Public Sub New(DataColumn As DataColumn)
        MyBase.New(DataColumn)
      End Sub

      Public TransformType As Integer
      Public TransformParam As Object
      'Public GroupColumn As DataColumn
      Public DisplayColumn As DataColumn
      Private mOriginalColumn As DataColumn
      Private mMainDataType As Singular.Reflection.SMemberInfo.MainType

      Public Property OriginalColumn As DataColumn
        Get
          Return mOriginalColumn
        End Get
        Set(value As DataColumn)
          mOriginalColumn = value
          Dim smi As New Singular.Reflection.SMemberInfo(mOriginalColumn.DataType)
          mMainDataType = smi.DataTypeMain

          If mMainDataType = Reflection.SMemberInfo.MainType.Number AndAlso TransformType = 2 Then
            DisplayColumn = New DataColumn(Key & "_Display", GetType(String))
            DataColumn.ExtendedProperties("AutoGenerate") = False
            DataColumn.Table.Columns.Add(DisplayColumn)
            DisplayColumn.SetOrdinal(DataColumn.Ordinal)
          End If
        End Set
      End Property

      Public LastTransformedValue As Object
      Public OtherValue As Object

      Public Function GetTransformedValue(Row As DataRow) As Object
        OtherValue = Nothing
        LastTransformedValue = GetTransformedValueInternal(Row)
        Return LastTransformedValue
      End Function

      Private Function GetTransformedValueInternal(Row As DataRow) As Object

        If TransformType = 1 Then
          Return Row(mOriginalColumn)
        End If

        If mMainDataType = Reflection.SMemberInfo.MainType.Number Then
          If TransformType = 2 Then
            Dim StartValue As Decimal = Math.Floor(Row(mOriginalColumn) / TransformParam) * TransformParam
            OtherValue = StartValue.ToString("#,##0;(#,##0)") & " - " & CDec(StartValue + TransformParam).ToString("#,##0;(#,##0)")
            Return StartValue
          End If

        ElseIf mMainDataType = Reflection.SMemberInfo.MainType.Date Then
          Dim Value As Date = Row(mOriginalColumn).Date
          If TransformType = 2 Then
            'Date only
            Return Value
          ElseIf TransformType = 3 Then
            'Month only
            Return New Date(Value.Year, Value.Month, 1)

          ElseIf TransformType = 4 Then
            'Year only
            Return New Date(Value.Year, 1, 1)

          End If

        Else
          'string

          Dim Value As String = Row(mOriginalColumn).ToString
          If Value Is Nothing Then
            Return ""
          End If

          If TransformType = 2 Then
            'First x chars
            Dim Chars As Integer = If(TransformParam > Value.Length, Value.Length, TransformParam)
            Return Value.Substring(0, Chars)

          ElseIf TransformType = 3 Then
            'first word
            Dim Index As Integer = Value.IndexOf(" ")
            Return If(Index = -1, Value, Value.Substring(0, Index))
          End If

        End If
        Return Row(OriginalColumn)
      End Function
    End Class

    Public Sub SetData(Data As DataSet)
      DataSet = Data

      Dim RootTable As DataTable = Nothing
      For Each Table As DataTable In Data.Tables
        If Not Table.ExtendedProperties.ContainsKey("ReportCriteria") AndAlso Table.ParentRelations.Count = 0 Then
          If RootTable IsNot Nothing Then
            'Throw New Exception("Dataset must only have one root table.")
          Else
            RootTable = Table
          End If
        End If
      Next

      Dim Band As New Band(Me, Nothing, RootTable)
      Bands.Add(Band)

    End Sub

    Public Sub LoadLayout(LayoutInfo As Object)

      Bands(0).LoadLayout(LayoutInfo.RootBand, Nothing, Band.GroupRestoreType.NA)

    End Sub

    Public Shared Function CreateLayout(ByRef DataSet As DataSet, LayoutInfo As Object) As DataSet

      Dim SGrid As New SGridServer
      SGrid.SetData(DataSet)
      SGrid.LoadLayout(LayoutInfo)

      Return DataSet

    End Function

  End Class

  Public Class SGridHelpers
    Inherits Singular.Web.Controls.HelperControls.HelperBase

    Private Shared HTMLContent As String = ""

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      SyncLock HTMLContent
        If HTMLContent = "" Then
          Dim WebLib = Assembly.GetAssembly(GetType(Singular.Web.Web))
          Using Str = WebLib.GetManifestResourceStream("Singular.Web.SGridHelpers.html")
            Using Reader As New IO.StreamReader(Str)
              HTMLContent = Reader.ReadToEnd
            End Using
          End Using
        End If
      End SyncLock

      Writer.Write(HTMLContent)

    End Sub

  End Class

  Public Class SGridChartBuilder
    Inherits Singular.Web.Controls.HelperControls.HelperBase

    Private Shared HTMLContent As String = ""

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      SyncLock HTMLContent
        If HTMLContent = "" Then
          Dim WebLib = Assembly.GetAssembly(GetType(Singular.Web.Web))
          Using Str = WebLib.GetManifestResourceStream("Singular.Web.ChartBuilder.html")
            Using Reader As New IO.StreamReader(Str)
              HTMLContent = Reader.ReadToEnd
            End Using
          End Using
        End If
      End SyncLock

      Writer.Write(HTMLContent)

    End Sub

  End Class

  Public Class GridInterface

    Public Class GridDataInfo
      Public Property Data As Object
      <Singular.Web.JSonString> Public Property Schema As String
      Public Property LayoutList As Reporting.ROGridUserInfoList
    End Class

    <Singular.Web.WebCallable()>
    Public Shared Function GetData(Arguments As Object) As Object

      Dim Instance As SGridInfo = Singular.Web.SGridInfo.GetGridInfo(Arguments)
      Dim Data = Instance.GetData(Arguments.GetDataParams)

      Dim gdi As New GridDataInfo

      'TODO, allow non dataset datatypes
      For Each tbl As DataTable In Data.Tables
        Singular.Data.DataTables.AddExtendedInfo(tbl)

        If tbl.ParentRelations.Count = 0 AndAlso tbl.TableName <> "Information" Then
          tbl.TableName = "Data"
          gdi.Data = tbl

          'gdi.TableName = tbl.TableName
          Exit For
        End If
      Next



      Dim js As New Singular.Web.Data.JS.StatelessJSSerialiser(gdi.Data)
      gdi.Schema = js.GetSchema(Singular.Web.Data.OutputType.JSon)
      gdi.LayoutList = Reporting.ROGridUserInfoList.GetROGridUserInfoList(Instance, False)

      Return gdi
    End Function

    Public Shared Function SaveLayout(UniqueKey As String, LayoutInfo As String, LayoutName As String, ParentLayout As String, VisibleToOthers As Boolean) As Singular.Web.Result

      Return New Singular.Web.Result(
        Sub()

          If LayoutName = "" AndAlso Not Debugger.IsAttached Then
            Throw New Exception("Description must be provided")
          End If

          If Singular.Security.CurrentIdentity Is Nothing Then
            Throw New Exception("User is logged out")
          End If
          Dim cProc As New Singular.CommandProc("InsProcs.InsWebGridLayout",
                                                {"@UserID", "@UniqueKey", "@LayoutInfo", "@LayoutName", "@ParentLayout", "@AllUsers"},
                                                {Singular.Security.CurrentIdentity.UserID, UniqueKey, Singular.Misc.NothingDBNull(LayoutInfo), LayoutName, ParentLayout, VisibleToOthers})
          cProc.Execute()

        End Sub)

    End Function

    Private Enum ExportType
      Excel = 1
      PDF = 2
    End Enum

    Private Shared Function ExportExcel(ReportData As Object, Layout As Object) As Singular.Web.Result

      Return Export(ReportData, Layout, ExportType.Excel)

    End Function

    Private Shared Function ExportPDF(ReportData As Object, Layout As Object) As Singular.Web.Result

      Return Export(ReportData, Layout, ExportType.PDF)

    End Function

    Private Shared Function Export(ReportData As Object, Layout As Object, Type As ExportType) As Singular.Web.Result

      Dim Cache = HttpContext.Current.Cache

      Return New Singular.Web.ASyncResult("Fetching Data...",
        Function(ap)

          Dim Instance As SGridInfo = Singular.Web.SGridInfo.GetGridInfo(ReportData)
          Dim Data = Instance.GetData(ReportData.GetDataParams)

          'Group / set widths alignments etc
          ap.Update("Grouping Data...")
          Singular.Web.CustomControls.SGrid.SGridServer.CreateLayout(Data, Layout)
          For Each Table As DataTable In Data.Tables
            If Table.ParentRelations.Count = 0 AndAlso Table.TableName <> "Information" Then
              Table.TableName = "Report Data"
              Exit For
            End If
          Next

          If Type = ExportType.Excel Then

            'Excel Export
            ap.Update("Creating Excel File...")
            Dim Exporter As New Singular.Data.ExcelExporter()
            Exporter.FormatAsTable = SGrid.ExportAsTable
            Exporter.PopulateData(Data)
            Return Singular.Web.WebServices.FileDownloadHandler.SaveTempFile(Exporter.AddFileExtension(Instance.ExportFileName), Exporter.GetStream.ToArray, , Cache)

          Else

            'PDF Export
            ap.Update("Creating Report File...")
            Dim Rpt As New Singular.Reporting.PDFReport(Instance.FriendlyName)
            Rpt.PopulateData(Data)
            Return Singular.Web.WebServices.FileDownloadHandler.SaveTempFile(Instance.ExportFileName & ".pdf", Rpt.GetStream.ToArray, , Cache)
          End If

        End Function)

    End Function

  End Class

End Namespace


