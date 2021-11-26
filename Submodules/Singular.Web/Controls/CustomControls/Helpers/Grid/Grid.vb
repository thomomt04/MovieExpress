Imports Singular.Web.Controls.HelperControls
Imports System.Reflection
Imports Singular.Web.Controls

Namespace CustomControls


  ''' <summary>
  ''' Creates a table / grid and creates a row for each item in the provided list.
  ''' </summary>
  Public Class Table(Of ObjectType, ChildControlObjectType)
    Inherits HelperBase(Of ObjectType, ChildControlObjectType)

    Public Class TableCaption
      Inherits HelperBase(Of ObjectType)

      Protected Friend Overrides Sub Render()
        MyBase.Render()

        WriteFullStartTag("caption", TagType.Normal)
        RenderChildren()
        Writer.WriteEndTag("caption")
      End Sub

    End Class

    Public Class TableColumnGroup
      Inherits HelperBase(Of ObjectType)

      Public Property GroupName As String
      Public Property ColSpan As Integer = 0
      Private mFirstRender As Boolean = True

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        Style.TextAlign = TextAlign.center
      End Sub

      Protected Friend Overrides Sub Render()
        MyBase.Render()

        If mFirstRender Then
          Attributes("colspan") = ColSpan

          WriteFullStartTag("th", TagType.Normal)
          Writer.Write(GroupName)
          Writer.WriteEndTag("th")

          mFirstRender = False
        End If

      End Sub

    End Class

    Public Class TableColumn
      Inherits HelperBase(Of ChildControlObjectType)

      Public Property HeaderText As String
      Public Property ColSpan As Integer = 1
      Public Property RowSpan As Integer = 1
      Public Property AutoFormat As Boolean = True
      Public Property AllowSort As Boolean = True


      Private mEditor As EditorBase(Of ChildControlObjectType)
      Private mFieldDisplay As FieldDisplay(Of ChildControlObjectType)
      Friend ReadOnlyColumn As Boolean = False
      Friend HardCodedContent As String = ""
      Friend StaticControl As HelperBase

      Friend Sub SetWidthFromAttribute(pi As PropertyInfo)
        Dim cw = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ColumnWidth)(pi)
        If cw IsNot Nothing AndAlso cw.DefaultWidth <> 0 Then
          Style.Width = cw.DefaultWidth
        End If
      End Sub

      Private mHeaderStyle As CSSStyle
      Public ReadOnly Property HeaderStyle As CSSStyle
        Get
          If mHeaderStyle Is Nothing Then
            mHeaderStyle = New CSSStyle
          End If
          Return mHeaderStyle
        End Get
      End Property

      Public ReadOnly Property Editor As EditorBase(Of ChildControlObjectType)
        Get
          Return mEditor
        End Get
      End Property

      Public ReadOnly Property FieldDisplay As FieldDisplay(Of ChildControlObjectType)
        Get
          Return mFieldDisplay
        End Get
      End Property

      ''' <summary>
      ''' The editor, or display control depending on if the field is read-only.
      ''' </summary>
      Public ReadOnly Property InnerControl As HelperBase(Of ChildControlObjectType)
        Get
          Return If(mEditor Is Nothing, mFieldDisplay, mEditor)
        End Get
      End Property

      Private mCellBindings As New KnockoutBindingManager(Of ChildControlObjectType)(Me, Nothing)
      Public ReadOnly Property CellBindings As KnockoutBindingManager(Of ChildControlObjectType)
        Get
          Return mCellBindings
        End Get
      End Property

      Private mHeaderBindings As New KnockoutBindingManager(Of ChildControlObjectType)(Me, Nothing)
      Public ReadOnly Property HeaderBindings As KnockoutBindingManager(Of ChildControlObjectType)
        Get
          Return mHeaderBindings
        End Get
      End Property

      Private mGroup As TableColumnGroup
      Public Property Group As TableColumnGroup
        Get
          Return mGroup
        End Get
        Set(value As TableColumnGroup)
          If mGroup IsNot Nothing Then mGroup.ColSpan -= 1
          mGroup = value
          mGroup.ColSpan += 1
        End Set
      End Property

      ''' <summary>
      ''' Hides the column below a certain size.
      ''' </summary>
      Public Function BootstrapHide(size As BootstrapSize) As TableColumn
        AddClass(String.Format("d-none d-{0}-table-cell", Singular.Reflection.GetEnumDisplayName(size)))
        Return Me
      End Function

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        If PropertyInfo IsNot Nothing Then

          Dim FormatString As String = ""
          Dim MainType As Reflection.SMemberInfo.MainType

          Dim smi As New Singular.Reflection.SMemberInfo(PropertyInfo)
          Dim IsDropDown As Boolean = Singular.ReflectionCached.GetCachedMemberInfo(PropertyInfo).DropDownWebAttribute IsNot Nothing
          MainType = smi.DataTypeMain

          If AutoFormat AndAlso PropertyInfo IsNot Nothing AndAlso Not IsDropDown Then
            Select Case MainType
              Case Reflection.SMemberInfo.MainType.Number
                Style.TextAlign = TextAlign.right
                If smi.IsInteger Then
                  FormatString = Singular.Formatting.GetCurrentFormats.Number_NoDecimals
                Else
                  FormatString = Singular.Formatting.GetCurrentFormats.Number_Decimals
                End If
              Case Reflection.SMemberInfo.MainType.Boolean
                Style.TextAlign = TextAlign.center
              Case Reflection.SMemberInfo.MainType.Date
                Style.TextAlign = TextAlign.center
                FormatString = Singular.Formatting.GetCurrentFormats.Date_OnlyDate
            End Select
          End If

          If ReadOnlyColumn Then

            Dim TableParent As Table(Of ObjectType, ChildControlObjectType) = CType(Parent, TableRow).Parent

            If TableParent.ReadOnlyColumnsHaveSpan OrElse smi.DataTypeMain = Reflection.SMemberInfo.MainType.Boolean Then

              mFieldDisplay = New Singular.Web.CustomControls.FieldDisplay(Of ChildControlObjectType)

              mFieldDisplay.DataType = MainType
              If mLinqExpression IsNot Nothing Then
                mFieldDisplay.For(mLinqExpression)
              Else
                mFieldDisplay.For(PropertyInfo)
              End If

              mFieldDisplay.TagType = FieldTagType.span
              'If TableParent.ServerBindObject Is Nothing Then
              '  TargetStyle = mFieldDisplay.Style
              'End If
              If FormatString <> "" Then
                mFieldDisplay.FormatString = FormatString
              End If

              AddControl(mFieldDisplay)

            Else

              Select Case MainType
                Case Reflection.SMemberInfo.MainType.Number
                  CellBindings.Add(KnockoutBindingString.NValue, Singular.Web.Controls.BindingHelpers.GetNumberBinding(GetForJS, Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(PropertyInfo), FormatString))
                Case Reflection.SMemberInfo.MainType.Date
                  CellBindings.Add(KnockoutBindingString.DateValue, Singular.Web.Controls.BindingHelpers.GetDateBinding(GetForJS, PathToContext, Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateField)(PropertyInfo), FormatString))
                Case Else
                  CellBindings.Add(KnockoutBindingString.text, GetForJS)
              End Select

            End If

          Else
            'Editable
            If mLinqExpression IsNot Nothing Then
              mEditor = Singular.Web.CustomControls.EditorBase(Of ChildControlObjectType).GetEditor(mLinqExpression)
            Else
              mEditor = Singular.Web.CustomControls.EditorBase(Of ChildControlObjectType).GetEditor(PropertyInfo)
            End If

            If TypeOf mEditor Is CheckBoxEditor(Of ChildControlObjectType) Then
              Style.TextAlign = TextAlign.center
            End If

            mEditor.Settings.Width = "100%"

            'mEditor.TableCellMode()

            mEditor.Tooltip = Singular.Reflection.GetDisplayName(PropertyInfo)
            AddControl(mEditor)
          End If


          If HeaderText = "" Then
            HeaderText = Singular.Reflection.GetDisplayName(PropertyInfo)
          End If
        End If

      End Sub

      Friend Sub RenderHeader(GroupLevel As Boolean, HasGroups As Boolean)
        MyBase.Render()

        If GroupLevel AndAlso Group IsNot Nothing Then
          Group.Render(Writer)

        ElseIf Not GroupLevel AndAlso Group Is Nothing Then
          'Do nothing
        Else

          Dim HeaderRowSpan = RowSpan
          If Group Is Nothing AndAlso HasGroups Then HeaderRowSpan += 1

          If ColSpan > 1 Then
            Attributes("colspan") = ColSpan
          End If
          If HeaderRowSpan > 1 Then
            Attributes("rowspan") = HeaderRowSpan
          End If

          Dim TableParent As Table(Of ObjectType, ChildControlObjectType) = CType(Parent, TableRow).Parent

          If PropertyInfo IsNot Nothing AndAlso TableParent.ServerBindObject Is Nothing AndAlso AllowSort Then
            Attributes("data-Property") = PropertyInfo.Name
          End If

          Dim TempStyle As CSSStyle = Nothing
          If mHeaderStyle IsNot Nothing Then
            TempStyle = Style
            Style = mHeaderStyle
          End If

          WriteFullStartTag("th", TagType.Normal, HeaderBindings)

          If TempStyle IsNot Nothing Then
            Style = TempStyle
          End If

          Writer.WriteEncodedText(HeaderText)

          Writer.WriteEndTag("th")

        End If

      End Sub

      Public Property OnRenderTD As Action(Of TableColumn, Object)

      Friend Sub RenderContent()
        MyBase.Render()

        If ColSpan > 1 Then
          Attributes("colspan") = ColSpan
        End If

        If RowSpan > 1 Then
          Attributes("rowspan") = RowSpan
        Else
          Attributes("rowspan") = Nothing
        End If

        If OnRenderTD IsNot Nothing Then
          OnRenderTD.Invoke(Me, ServerBindObject)
        End If

        WriteFullStartTag("td", TagType.Normal, CellBindings)
        If HardCodedContent <> "" Then
          Writer.Write(HardCodedContent)
        End If
        If StaticControl IsNot Nothing Then
          StaticControl.Render(Writer)
        End If

        For Each ctl As Controls.CustomWebControl In Me.Controls
          ctl.ServerBindObject = ServerBindObject
          ctl.Render()
        Next

        Writer.WriteEndTag("td")
        ServerBindObject = Nothing
      End Sub

    End Class

    Public MustInherit Class TableRow
      Inherits HelperBase(Of ChildControlObjectType)

      Protected mRowButtons As New List(Of Button(Of ChildControlObjectType))
      Friend mStaticRowInfo As IStaticRowInfo

      Public Property IncludeInHeader As Boolean = True

      Public Property AutoFormat As Boolean = True

      Friend Function AddButton(ButtonText As String) As Button(Of ChildControlObjectType)
        Dim mNewButton = Helpers.Button(ButtonText)
        mNewButton.Attributes("tabindex") = "-1"
        mRowButtons.Add(mNewButton)
        Return mNewButton
      End Function

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

      End Sub

#Region " Add Column Functions "

      Protected Sub AddAutoClass(col As TableColumn, IsReadOnly As Boolean)
        Dim tbl = CType(Parent, Table(Of ObjectType, ChildControlObjectType))
        If IsReadOnly Then
          If tbl.ReadOnlyCellClass <> "" Then
            col.AddClass(tbl.ReadOnlyCellClass)
          End If
        Else
          If tbl.EditableCellClass <> "" Then
            col.AddClass(tbl.EditableCellClass)
          End If
          If tbl.EditorClass <> "" AndAlso col.Editor IsNot Nothing Then
            col.Editor.AddClass(tbl.EditorClass)
          End If
        End If
      End Sub

#End Region

      Friend Function GetDataCellCount() As Integer
        Dim CellCount As Integer = 0
        For Each ctl As Controls.CustomWebControl In Controls
          If TypeOf ctl Is TableColumn Then
            CellCount += CType(ctl, TableColumn).ColSpan
          End If
        Next
        If mRowButtons.Count > 0 Then
          CellCount += 1
        End If
        Return CellCount
      End Function

      Protected Overridable Sub RenderPreCells(ByVal Index As Integer, ByVal RowCount As Integer, ByVal TableHasChildBands As Boolean)

      End Sub

      Protected Overridable Sub OnColRender(Col As TableColumn)

      End Sub

      Friend Sub RenderContent(ByVal Index As Integer, ByVal RowCount As Integer, ByVal TableHasChildBands As Boolean)
        MyBase.Render()


        If mStaticRowInfo IsNot Nothing Then
          mStaticRowInfo.RenderBeginRow()
        Else
          Writer.WriteBeginTag("tr")
          WriteClass()
          Bindings.WriteKnockoutBindings()
          WriteStyles()
        End If

        Writer.WriteCloseTag(True)

        RenderPreCells(Index, RowCount, TableHasChildBands)

        'Normal Cells
        Dim IsFirst As Boolean = True
        Dim ColIndex As Integer = 0

        For Each col As HelperBase In Controls
          If TypeOf col Is TableColumn Then
            Dim tc As TableColumn = col

            OnColRender(tc)

            If mStaticRowInfo IsNot Nothing Then
              tc.StaticControl = mStaticRowInfo.GetCell(ColIndex)
            Else
              tc.ServerBindObject = ServerBindObject
            End If

            tc.RenderContent()

            ColIndex += 1
          End If
        Next

        'Remove Button
        If Index = 0 AndAlso (mRowButtons.Count > 0) Then
          Writer.WriteBeginTag("td")
          If TableHasChildBands Then
            RowCount -= 1
          End If
          Writer.WriteAttribute("class", "RButtons")
          If RowCount > 1 Then
            Writer.WriteAttribute("rowspan", RowCount)
          End If
          Writer.WriteCloseTag(False)

          For Each btn In mRowButtons
            btn.Render()
          Next

          Writer.WriteEndTag("td")
        End If

        If mStaticRowInfo IsNot Nothing Then
          mStaticRowInfo.RenderEndRow()
        Else
          Writer.WriteEndTag("tr", True)
        End If

        ServerBindObject = Nothing
      End Sub

    End Class

    Public Class DataRowTemplate
      Inherits TableRow

#Region " Properties / Methods "

      Private mExpandButton As Button(Of ChildControlObjectType)
      Friend mRemoveButton As Button(Of ChildControlObjectType)
      Friend mIsChildBandRow As Boolean = False
      Private ParentTable As Table(Of ObjectType, ChildControlObjectType)
      Private mHasGroups As Boolean = False

      Public Property MergeChildCells As Boolean = False

      Friend Sub AddExpandButton(ExpandedProperty As String)
        mExpandButton = Helpers.Button("")
        mExpandButton.Style("width") = Singular.Web.Controls.ExpandButtonWidth & "px"
        mExpandButton.AddBinding(KnockoutBindingString.click, "Singular.Expand($data." & ExpandedProperty & "); e.stopPropagation();")
        mExpandButton.ButtonText.AddBinding(KnockoutBindingString.text, "($data." & ExpandedProperty & "() ? '-' : '+')")
        mExpandButton.AddBinding(KnockoutBindingString.title, "($data." & ExpandedProperty & "() ? 'Hide Child Rows.' : 'Expand Child Rows.')")

        mExpandButton.ButtonStyle = ButtonMainStyle.Default
        mExpandButton.ButtonSize = ButtonSize.ExtraSmall
      End Sub

      Public ReadOnly Property ExpandButton As Button(Of ChildControlObjectType)
        Get
          Return mExpandButton
        End Get
      End Property

      Private mHeaderStyle As CSSStyle
      Public ReadOnly Property HeaderStyle As CSSStyle
        Get
          If mHeaderStyle Is Nothing Then
            mHeaderStyle = New CSSStyle
          End If
          Return mHeaderStyle
        End Get
      End Property

      Private mHeaderBindings As New KnockoutBindingManager(Of ChildControlObjectType)(Me, Nothing)
      Public ReadOnly Property HeaderBindings As KnockoutBindingManager(Of ChildControlObjectType)
        Get
          Return mHeaderBindings
        End Get
      End Property

      Protected Friend Overrides Sub Setup()
        MyBase.Setup()

        ParentTable = Parent
        If ParentTable.AllowRemove Then
          mRemoveButton = AddButton("")
          mRemoveButton.Image.SrcDefined = DefinedImageType.TrashCan
          mRemoveButton.Image.Glyph = FontAwesomeIcon.trash
          mRemoveButton.Attributes("title") = Singular.LocalStrings.GetText(Singular.LocalStrings.Grid_Delete_Tooltip)

          mRemoveButton.ButtonSize = ButtonSize.ExtraSmall
          mRemoveButton.ButtonStyle = ButtonMainStyle.Danger

          mRemoveButton.AddRemoveBinding()
        End If
      End Sub

      ''' <summary>
      ''' Adds a column with nothing in it.
      ''' </summary>
      Public Function AddColumn(Optional HeaderText As String = "", Optional ColumnWidth As Integer = 0) As TableColumn
        Dim Col = AddColumn(CType(Nothing, PropertyInfo), HeaderText)
        AddAutoClass(Col, True)
        If ColumnWidth <> 0 Then
          Col.Style.Width = ColumnWidth & "px"
        End If
        Return Col
      End Function

      ''' <summary>
      ''' Adds an editable Column. 
      ''' </summary>
      ''' <param name="le">The property to bind the column to.</param>
      Public Function AddColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), Optional HeaderText As String = "") As TableColumn

        Dim Col As New TableColumn With {.HeaderText = HeaderText}
        Col.For(le)
        Col.SetWidthFromAttribute(Col.PropertyInfo)

        AddControl(Col)
        AddAutoClass(Col, False)
        Return Col

      End Function

      ''' <summary>
      ''' Adds an editable Column. 
      ''' </summary>
      Public Function AddColumn(pi As PropertyInfo, Optional HeaderText As String = "") As TableColumn

        Dim Col As New TableColumn With {.HeaderText = HeaderText}
        If pi IsNot Nothing Then
          Col.For(pi)
          Col.SetWidthFromAttribute(pi)
        End If

        AddControl(Col)
        AddAutoClass(Col, False)
        Return Col

      End Function

      ''' <summary>
      ''' Adds an editable Column. 
      ''' </summary>
      ''' <param name="le">The property to bind the column to.</param>
      Public Function AddColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ColumnWidth As Integer, Optional HeaderText As String = "") As TableColumn

        Dim col = AddColumn(le, HeaderText)
        col.Style.Width = ColumnWidth & "px"
        Return col

      End Function

      ''' <summary>
      ''' Adds a Column that displays the property but doesn't allow it to be edited.
      ''' </summary>
      ''' <param name="le">The Property to Display</param>
      Public Function AddReadOnlyColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), Optional HeaderText As String = "") As TableColumn

        Dim Col As New TableColumn With {.HeaderText = HeaderText}
        Col.ReadOnlyColumn = True
        Col.AutoFormat = AutoFormat
        Col.For(le)

        Col.SetWidthFromAttribute(Col.PropertyInfo)
        AddAutoClass(Col, True)
        Return AddControl(Col)

      End Function

      ''' <summary>
      ''' Adds a Column that displays the property but doesn't allow it to be edited.
      ''' </summary>
      Public Function AddReadOnlyColumn(pi As PropertyInfo, Optional HeaderText As String = "") As TableColumn

        Dim Col As New TableColumn With {.HeaderText = HeaderText}
        Col.ReadOnlyColumn = True
        Col.AutoFormat = AutoFormat
        If pi IsNot Nothing Then
          Col.For(pi)
          Col.SetWidthFromAttribute(pi)
        End If
        AddAutoClass(Col, True)
        Return AddControl(Col)

      End Function

      ''' <summary>
      ''' Adds a column Group. You must specify which columns are linked to this group using .AddColumn(...).Group = ThisGroup
      ''' </summary>
      Public Function AddColumnGroup(GroupName As String) As TableColumnGroup
        mHasGroups = True
        Return AddControl(New TableColumnGroup With {.GroupName = GroupName})
      End Function

      ''' <summary>
      ''' Adds a Column that displays the property but doesn't allow it to be edited.
      ''' </summary>
      ''' <param name="le">The Property to Display</param>
      Public Function AddReadOnlyColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ColumnWidth As Integer, Optional HeaderText As String = "") As TableColumn

        Dim Col = AddReadOnlyColumn(le, HeaderText)

        'If the inner span needs a set width, Ellipses
        Dim TParent = CType(Parent, Table(Of ObjectType, ChildControlObjectType))
        If TParent.EllipsesPadding <> -1 AndAlso Col.FieldDisplay IsNot Nothing Then
          Col.FieldDisplay.Style.Width = ColumnWidth - TParent.EllipsesPadding
        End If

        Col.Style.Width = ColumnWidth & "px"
        Return Col

      End Function

#End Region

#Region " Rendering "

      Private Iterator Function GetColumns() As IEnumerable(Of TableColumn)
        For Each col As HelperBase In Controls
          If TypeOf col Is TableColumn Then
            Yield col
          End If
        Next
      End Function

      Friend Sub RenderHeader(Index As Integer, RowCount As Integer)
        MyBase.Render()

        If IncludeInHeader Then

          Dim TempStyle As CSSStyle = Nothing
          If mHeaderStyle IsNot Nothing Then
            TempStyle = Style
            Style = mHeaderStyle
          End If

          WriteFullStartTag("tr", TagType.Normal, mHeaderBindings)

          If TempStyle IsNot Nothing Then
            Style = TempStyle
          End If

          If mHasGroups Then RowCount += 1

          'Expand Button Heading
          If mExpandButton IsNot Nothing Then
            Writer.WriteBeginTag("th")
            Writer.WriteAttribute("class", "LButtons")
            Writer.WriteAttribute("rowspan", RowCount)
            Writer.WriteCloseTag(False)
            Writer.WriteEndTag("th")
          End If

          'Normal Cells
          For Each col In GetColumns()
            col.RenderHeader(True, mHasGroups)
          Next

          'Remove Button Heading
          If Index = 0 AndAlso (mRowButtons.Count > 0) Then
            Writer.WriteBeginTag("th")
            Writer.WriteAttribute("class", "RButtons")
            If RowCount > 1 Then
              Writer.WriteAttribute("rowspan", RowCount)
            End If
            Writer.WriteCloseTag(False)

            If ParentTable.AddNewButtonLocation = TableAddButtonLocationType.InHeader Then
              ParentTable.mAddNewButton.Render()
            End If

            Writer.WriteEndTag("th")
          End If

          Writer.WriteEndTag("tr")

          If mHasGroups Then
            'If any of the columns have a group, the group would have been rendered in the first row.
            'Now the actual cells need to be rendered in a new row below the group cells.
            Writer.WriteFullBeginTag("tr")
            For Each col In GetColumns()
              col.RenderHeader(False, mHasGroups)
            Next
            Writer.WriteEndTag("tr")
          End If
        End If

      End Sub

      Protected Overrides Sub RenderPreCells(ByVal Index As Integer, ByVal RowCount As Integer, ByVal TableHasChildBands As Boolean)

        'Expand Button
        If mExpandButton IsNot Nothing Then
          Writer.WriteBeginTag("td")
          Writer.WriteAttribute("class", "LButtons")
          If RowCount > 2 AndAlso Index = 0 Then
            Writer.WriteAttribute("rowspan", RowCount - 1)
          End If
          Writer.WriteCloseTag(False)
          mExpandButton.Render()
          Writer.WriteEndTag("td")
        End If
        If mIsChildBandRow AndAlso Not MergeChildCells Then
          Writer.WriteFullBeginTag("td")
          Writer.WriteEndTag("td")
        End If

      End Sub

      Private IsFirst As Boolean = True

      Protected Overrides Sub OnColRender(Col As Table(Of ObjectType, ChildControlObjectType).TableColumn)
        MyBase.OnColRender(Col)

        If IsFirst Then
          IsFirst = False
          If MergeChildCells Then
            CType(Col, TableColumn).ColSpan += 1
          End If
        End If
      End Sub

#End Region

    End Class

    Public Class TableFooterRow
      Inherits TableRow

      Public Function AddColumn(Optional CellContent As String = "") As TableColumn

        Dim Col As New TableColumn
        Col.HardCodedContent = CellContent
        AddControl(Col)

        Return Col

      End Function

      ''' <summary>
      ''' Adds a footer column that displays the sum of all the items in the list. 
      ''' </summary>
      Public Function AddSumColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object))) As TableColumn

        Return AddSummaryColumn("Sum", le)

      End Function

      ''' <summary>
      ''' Adds a footer column that displays the average of all the items in the list. 
      ''' </summary>
      Public Function AddAverageColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object))) As TableColumn

        Return AddSummaryColumn("Avg", le)

      End Function

      ''' <summary>
      ''' Adds a footer column that displays the average of all the items in the list. Only items with a non zero value are included.
      ''' </summary>
      Public Function AddAverageNZColumn(le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object))) As TableColumn

        Return AddSummaryColumn("AvgNZ", le)

      End Function

      Private Function AddSummaryColumn(Type As String, le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object))) As TableColumn

        Dim Col = AddColumn()
        Dim Binding As String = CType(Parent, Table(Of ObjectType, ChildControlObjectType)).PropertyInfo.Name 'CType(Parent, Table(Of ObjectType, ChildControlObjectType)).GetForJS
        Dim Pi As PropertyInfo = Singular.Reflection.GetMember(le)
        Binding &= "()." & Type & "('" & Pi.Name & "')"

        Col.Style.TextAlign = TextAlign.right
        Col.CellBindings.Add(KnockoutBindingString.NValue,
                             Singular.Web.Controls.BindingHelpers.GetNumberBinding(Binding,
                                                                                   Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(Pi),
                                                                                    "#,##0.00;(#,##0.00)"))
        Return Col


      End Function

    End Class

    'Private Variables
    Private mAddNewButton As Button(Of ObjectType)
    Private mRowCount As Integer = 1
    Private mFirstRow As DataRowTemplate
    Private mFooterRows As New List(Of TableRow)
    Private mChildBandCell As TableColumn
    Private mChildBandRow As DataRowTemplate
    Private mAddNewButtonLocation As TableAddButtonLocationType

    Public Property NoDataContainer As HTMLDiv(Of ObjectType)

    Public Property EditableCellClass As String = ""
    Public Property EditorClass As String = ""
    Public Property ReadOnlyCellClass As String = ""

    Public Property BodyHeightMin As Integer = 0
    Public Property BodyHeightMax As Integer = 0

    Public Property ReadOnlyColumnsHaveSpan As Boolean = True
    Public Property EllipsesPadding As Integer = -1

    Public WriteOnly Property UseBootstrapStyling As Boolean
      Set(value As Boolean)
        If value Then
          RemoveClass("Grid")
          AddClass("table")
        Else
          AddClass("Grid")
          RemoveClass("table")
        End If
      End Set
    End Property


    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If PathToContext() <> "" Then
        AddBinding(KnockoutBindingString.with, PathToContext)
      End If
      BindingManager.HasBindingContext = True

      mFirstRow = New DataRowTemplate
      If ServerBindObject IsNot Nothing Then
        AllowRemove = False
      End If
      AddControl(mFirstRow)

      If AllowAddNew Then
        CreateAddNewButton(Singular.LocalStrings.GetText(Singular.LocalStrings.Grid_AddNew))
        AddNewButtonLocation = TableAddButtonLocation 'set from shared setting
      End If

      UseBootstrapStyling = AddBootstrapClasses

      AddClass("SUI-RuleBorder") 'Make broken rule controls appear with red border, rather than with an icon.

    End Sub

    'Public Properties
    Public Property AllowAddNew As Boolean = True
    Public Property AllowRemove As Boolean = True
    Public Property ShowHeader As Boolean = True
    Public Property AllowClientSideSorting As Boolean = True
    Public Property DataSourceString As String = ""
    Public Property AnimateExpansion As Boolean = True

    Public ReadOnly Property FirstRow As DataRowTemplate
      Get
        Return mFirstRow
      End Get
    End Property

    Private mStaticRows As List(Of IStaticRowInfo)

    ''' <summary>
    ''' Add static rows instead of having datasource. the controls in this control will act as cells. Setup headers by calling AddColumn.
    ''' </summary>
    Public Function AddStaticRow() As StaticRowInfo(Of ChildControlObjectType)
      If mStaticRows Is Nothing Then mStaticRows = New List(Of IStaticRowInfo)

      Dim sri As New StaticRowInfo(Of ChildControlObjectType)()
      mStaticRows.Add(sri)
      AddControl(sri)
      Return sri
    End Function

    Public Function AddStaticRows(Of ChildType)(Datasource As System.Linq.Expressions.Expression(Of Func(Of ObjectType, Object))) As StaticRowsInfo(Of ChildType)
      If mStaticRows Is Nothing Then mStaticRows = New List(Of IStaticRowInfo)

      Dim sri As New StaticRowsInfo(Of ChildType)()
      sri.Datasource = Singular.Linq.JavascriptExpressionParser(Of ObjectType).GetCachedOutput(Datasource, Linq.OutputModeType.KnockoutBinding, True, Linq.EqualsActionType.Compare, Linq.PropertyAccessType.AutoDetect).JavascriptString
      mStaticRows.Add(sri)
      AddControl(sri)
      Return sri
    End Function

    Public ReadOnly Property FooterRow As TableFooterRow
      Get
        If mFooterRows.Count = 0 Then
          Dim NewRow As New TableFooterRow
          mFooterRows.Add(NewRow)
          AddControl(NewRow)
        End If
        Return mFooterRows(0)
      End Get
    End Property

    Public Function AddFooterRow() As TableFooterRow

      If mFooterRows.Count = 0 Then
        Return FooterRow
      Else
        Dim NewRow As New TableFooterRow
        AddControl(NewRow)
        mFooterRows.Add(NewRow)
        Return NewRow
      End If
    End Function

    Public ReadOnly Property ParentCell As HelperBase
      Get
        If TypeOf Parent Is HelperBase Then
          Return Parent
        Else
          Return Nothing
        End If
      End Get
    End Property

    Public ReadOnly Property AddNewButton As Button(Of ObjectType)
      Get
        Return mAddNewButton
      End Get
    End Property


    Public Function CreateAddNewButton(ButtonText As String) As Button(Of ObjectType)
      AllowAddNew = True
      mAddNewButton = AddControl(New Button(Of ObjectType) With {.Text = ButtonText, .ButtonID = ""})
      mAddNewButton.ButtonSize = ButtonSize.Small
      mAddNewButton.Tooltip = Singular.LocalStrings.GetText(Grid_AddNew_Tooltip)
      If Singular.Web.Controls.DefaultButtonStyle = ButtonStyle.Bootstrap Then
        mAddNewButton.Image.Glyph = Singular.Web.Controls.DefaultAddNewButtonIcon
      End If
      mAddNewButton.Bindings.AddAddBinding(PropertyInfo)
      Return mAddNewButton
    End Function

    Public ReadOnly Property RemoveButton As Button(Of ChildControlObjectType)
      Get
        Return FirstRow.mRemoveButton
      End Get
    End Property

    Public ReadOnly Property ChildBandRow As DataRowTemplate
      Get
        Return mChildBandRow
      End Get
    End Property

    Public ReadOnly Property ChildBandCell As TableColumn
      Get
        Return mChildBandCell
      End Get
    End Property

    Public Property AddNewButtonLocation As TableAddButtonLocationType
      Get
        Return mAddNewButtonLocation
      End Get
      Set(value As TableAddButtonLocationType)
        If mAddNewButtonLocation <> value Then
          mAddNewButtonLocation = value
          If mAddNewButtonLocation = TableAddButtonLocationType.RowAtBottom Then
            mAddNewButton.ButtonSize = ButtonSize.Small
          Else
            mAddNewButton.ButtonSize = ButtonSize.ExtraSmall
            mAddNewButton.ButtonText.HTML = ""
          End If
        End If
      End Set
    End Property

    ''' <summary>
    ''' Adds a button to the data rows last cell. E.g. if you want to add a select button.
    ''' </summary>
    ''' <param name="ButtonText">The text of the button.</param>
    Public Function AddButton(ByVal ButtonText As String) As Button(Of ChildControlObjectType)
      Return FirstRow.AddButton(ButtonText)
    End Function

    ''' <summary>
    ''' Adds a row to each data row. E.g. If you want each item in a list to have 2 rows in the table.
    ''' </summary>
    Public Function AddRow() As DataRowTemplate
      If mChildBandCell IsNot Nothing Then
        Throw New Exception("Normal Rows cannot be added after Child Bands.")
      Else
        Dim row As New DataRowTemplate
        mRowCount += 1
        AddClass("grid-multi-row")
        Return AddControl(row)
      End If
    End Function

    Private mCaption As TableCaption
    Public Function AddCaption(Optional CaptionText As String = "") As TableCaption
      mCaption = AddControl(New TableCaption())
      mCaption.Helpers.HTML(CaptionText)
      Return mCaption
    End Function

    Public Function AddChildRow() As TableColumn

      'Remember what position the child band row container is in.
      If mChildBandCell Is Nothing Then

        'Add a new row for the child band
        mChildBandRow = AddRow()
        mChildBandRow.IncludeInHeader = False

        'Child Band only has one cell that spans the whole table.
        mChildBandCell = ChildBandRow.AddColumn
        mChildBandCell.ColSpan = FirstRow.GetDataCellCount
        mChildBandCell.AddClass("Grid-ChildCell")

        'Check if the object has an expanded property
        Dim piExpand As PropertyInfo = Singular.Reflection.GetProperty(OverrideChildType, "Expanded")
        If piExpand Is Nothing Then
          piExpand = Singular.Reflection.GetProperty(OverrideChildType, "IsExpanded")
        End If

        If piExpand IsNot Nothing Then
          mFirstRow.AddExpandButton(piExpand.Name)
          Dim eo = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ExpandOptions)(piExpand)
          If eo Is Nothing OrElse eo.RenderChildrenMode = Singular.DataAnnotations.ExpandOptions.RenderChildrenModeType.OnExpand Then
            ChildBandRow.AddBinding(KnockoutBindingString.if, "$data." & piExpand.Name)
            If AnimateExpansion Then
              mChildBandCell.TemplateRenderAnimation = VisibleFadeType.SlideUpDown
            End If
          Else
            If AnimateExpansion Then
              ChildBandRow.Bindings.AddVisibilityBinding("$data." & piExpand.Name, VisibleFadeType.SlideUpDown, VisibleFadeType.SlideUpDown)
            Else
              ChildBandRow.AddBinding(KnockoutBindingString.visible, "$data." & piExpand.Name)
            End If
          End If

          ChildBandRow.mIsChildBandRow = True
        End If

      End If

      Return mChildBandCell

    End Function

    Public Function AddChildTable(Of ChildTableObjectType)(pi As PropertyInfo, ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean, NoDataHTML As String) As Table(Of ChildControlObjectType, ChildTableObjectType)

      With AddChildRow()

        If NoDataHTML = "" Then
          Return .Helpers.TableFor(Of ChildTableObjectType)(pi, AllowAddNew, AllowRemove)
        Else
          Return .Helpers.TableReadOnlyFor(Of ChildTableObjectType)(pi, NoDataHTML)
        End If

      End With

    End Function

    Public Function AddChildTable(Of ChildTableObjectType)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), ByVal AllowAddNew As Boolean, ByVal AllowRemove As Boolean) As Table(Of ChildControlObjectType, ChildTableObjectType)

      Return AddChildTable(Of ChildTableObjectType)(Singular.Reflection.GetMember(Of ChildControlObjectType)(le), AllowAddNew, AllowRemove, "")

    End Function

    Public Function AddChildTableReadOnly(Of ChildTableObjectType)(ByVal le As System.Linq.Expressions.Expression(Of System.Func(Of ChildControlObjectType, Object)), NoDataHTML As String) As Table(Of ChildControlObjectType, ChildTableObjectType)

      Return AddChildTable(Of ChildTableObjectType)(Singular.Reflection.GetMember(Of ChildControlObjectType)(le), False, False, NoDataHTML)

    End Function

    Public ReadOnly Property HasChildTables As Boolean
      Get
        Return mChildBandCell IsNot Nothing
      End Get
    End Property

    ''' <summary>
    ''' Adds a column for each browsable property in the object. List properties are added as child tables.
    ''' </summary>
    Public Sub AutoGenerateColumns(Optional IncludeFieldPredicate As Func(Of PropertyInfo, Boolean) = Nothing)

      'Temp Child Lists
      Dim TempChildLists As New List(Of System.Reflection.PropertyInfo)

      Dim CurrentRow = FirstRow

      For Each pi As System.Reflection.PropertyInfo In OverrideChildType.GetProperties(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
        If Singular.Reflection.AutoGenerateField(pi) AndAlso Not Singular.Misc.In(pi.Name, "Expanded", "IsExpanded") Then
          If IncludeFieldPredicate Is Nothing OrElse IncludeFieldPredicate(pi) Then

            'Check Return Type
            If Singular.Reflection.TypeImplementsInterface(pi.PropertyType, GetType(ISingularListBase)) Then
              'For Lists, add a child table.
              TempChildLists.Add(pi)
            Else

              'Get AutoBindings
              Dim AutoBindings As Singular.Web.DataAnnotations.AutoBindings = pi.GetCustomAttributes(GetType(Singular.Web.DataAnnotations.AutoBindings), True).FirstOrDefault()
              Dim ColAttr = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ColumnWidth)(pi)
              Dim Align = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.Alignment)(pi)
              Dim Col As TableColumn

              If GetType(Singular.Documents.IDocumentProvider).IsAssignableFrom(pi.PropertyType) Then
                'Document
                Col = CurrentRow.AddColumn(Singular.Reflection.GetDisplayName(pi))
                Col.Helpers.DocumentManager(pi)
                Col.SetWidthFromAttribute(pi)

              ElseIf pi.CanWrite Then
                'Writable
                Col = CurrentRow.AddColumn(pi)
                If AutoBindings IsNot Nothing Then
                  For Each Binding In AutoBindings.Bindings
                    Col.Editor.AddBinding(Binding.Binding, Binding.JSString)
                  Next
                End If
              Else
                'Readonly
                Col = CurrentRow.AddReadOnlyColumn(pi)
                If AutoBindings IsNot Nothing Then
                  For Each Binding In AutoBindings.Bindings
                    Col.FieldDisplay.AddBinding(Binding.Binding, Binding.JSString)
                  Next
                End If
              End If

              If ColAttr IsNot Nothing Then
                If ColAttr.ColSpan > 1 Then Col.ColSpan = ColAttr.ColSpan
                If ColAttr.Bold Then Col.Style.FontWeight = FontWeight.semiBold
                If ColAttr.NewLineAfter Then CurrentRow = AddRow()
              End If

              If Align IsNot Nothing Then
                Col.Style.TextAlign = CInt(Align.Align)
                Col.InnerControl.Style.TextAlign = CInt(Align.Align)
              End If
            End If
          End If
        End If

      Next

      'Add Child Lists
      For Each pi As System.Reflection.PropertyInfo In TempChildLists

        Dim childAllowAddNew As Boolean = Singular.DataAnnotations.AllowAddRemoveEdit.AllowsAdd(pi.PropertyType, AllowAddNew)
        Dim childAllowRemove As Boolean = Singular.DataAnnotations.AllowAddRemoveEdit.AllowsRemove(pi.PropertyType, AllowRemove)
        If Singular.Reflection.TypeImplementsInterface(pi.PropertyType, GetType(Singular.ISingularReadOnlyListBase)) Then
          childAllowAddNew = False
          childAllowRemove = False
        End If
        Dim ChildTbl = AddChildTable(Of Object)(pi, childAllowAddNew, childAllowRemove, "")
        ChildTbl.OverrideChildType = Singular.Reflection.GetLastGenericType(pi.PropertyType)
        ChildTbl.AutoGenerateColumns()
      Next

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        If BodyHeightMin > 0 OrElse BodyHeightMax > 0 Then
          Attributes.Add("data-ScrollHeight", BodyHeightMin & "|" & BodyHeightMax)
        End If

        'Table
        WriteFullStartTag("table", TagType.IndentChildren)

        'Caption
        If mCaption IsNot Nothing Then
          mCaption.Render()
        End If

        'Header
        If ShowHeader Then
          .WriteBeginTag("thead")
          If HeadClasses <> "" Then
            .WriteAttribute("class", HeadClasses)
          End If
          .WriteCloseTag(True)

          'Header Rows
          For i As Integer = 0 To Me.Controls.Count - 1
            If TypeOf Controls(i) Is DataRowTemplate Then
              CType(Controls(i), DataRowTemplate).RenderHeader(i, mRowCount)
            End If
          Next

          .WriteEndTag("thead")
        End If

        'Start the Table Body
        .WriteBeginTag("tbody")

        If BodyClasses <> "" Then
          .WriteAttribute("class", BodyClasses)
        End If

        If mStaticRows IsNot Nothing Then
          RenderStaticRows(Writer)
        ElseIf ServerBindObject Is Nothing Then
          RenderClientBoundRows(Writer)
        Else
          RenderServerBoundRows(Writer)
        End If

        .WriteEndTag("tbody", True)

        'Footer section
        If mFooterRows IsNot Nothing OrElse AllowAddNew Then
          .WriteFullBeginTag("tfoot", True)
        End If

        'Summary row
        If mFooterRows.Count > 0 Then
          For Each fr As TableFooterRow In mFooterRows
            fr.RenderContent(0, 0, mChildBandCell IsNot Nothing)
          Next

        End If

        'Add New Button
        If AllowAddNew AndAlso AddNewButtonLocation = TableAddButtonLocationType.RowAtBottom Then

          .WriteBeginTag("tr")
          .WriteAttribute("class", "AddNewRow")
          .WriteCloseTag(True)
          .WriteBeginTag("td")
          .WriteAttribute("colspan", FirstRow.GetDataCellCount + If(FirstRow.ExpandButton IsNot Nothing, 1, 0))
          .WriteCloseTag(False)

          mAddNewButton.Render()

          .WriteEndTag("td")
          .WriteEndTag("tr", True)
        End If

        If mFooterRows.Count > 0 OrElse AllowAddNew Then
          .WriteEndTag("tfoot", True)
        End If

        .WriteEndTag("table", True)



      End With

    End Sub

    Public Property ApplyAlternateRowStyle As Boolean = True
    Public Property BodyClasses As String = ""
    Public Property HeadClasses As String = ""

    Private Sub RenderClientBoundRows(ByVal Writer As HtmlTextWriter)

      Dim BindingString As String = ""

      If DataSourceString = "" Then
        DataSourceString = PropertyInfo.Name
      End If

      If AllowClientSideSorting AndAlso PropertyInfo IsNot Nothing Then
        BindingString = "SFilter: " & PropertyInfo.Name & ", template: {foreach: Singular.ProcessList(" & DataSourceString & ", " & PropertyInfo.Name & "), "
      Else
        BindingString = "template: {foreach: " & DataSourceString & ", "
      End If

      If ApplyAlternateRowStyle Then
        BindingString &= "afterRender: function(c, o){ Singular.FormatTableRow(c, o); Singular.AfterTemplateRender(c, o) }}"
      Else
        BindingString &= "afterRender: function(c, o){ Singular.AfterTemplateRender(c, o) }}"
      End If

      Writer.WriteAttribute("data-bind", BindingString)
      Writer.WriteCloseTag(True)

      'Data Rows.
      For i As Integer = 0 To Me.Controls.Count - 1
        If TypeOf Controls(i) Is DataRowTemplate Then
          CType(Controls(i), TableRow).RenderContent(i, mRowCount, mChildBandCell IsNot Nothing)
        End If
      Next

    End Sub

    Private Sub RenderStaticRows(ByVal Writer As HtmlTextWriter)

      Writer.WriteCloseTag(True)

      Dim RowTemplate As DataRowTemplate = Nothing

      For i As Integer = 0 To Me.Controls.Count - 1
        If TypeOf Controls(i) Is DataRowTemplate Then
          RowTemplate = CType(Controls(i), TableRow)
        End If
      Next

      For Each sri In mStaticRows
        RowTemplate.mStaticRowInfo = sri
        RowTemplate.RenderContent(ID, 1, False)
      Next

    End Sub

    Private Sub RenderServerBoundRows(ByVal Writer As HtmlTextWriter)
      Writer.WriteCloseTag(True)

      For Each row As Object In ServerBindObject

        For i As Integer = 0 To Me.Controls.Count - 1
          If TypeOf Controls(i) Is DataRowTemplate Then
            CType(Controls(i), TableRow).ServerBindObject = row
            CType(Controls(i), TableRow).RenderContent(i, mRowCount, mChildBandCell IsNot Nothing)
          End If
        Next

      Next
    End Sub

  End Class

End Namespace
