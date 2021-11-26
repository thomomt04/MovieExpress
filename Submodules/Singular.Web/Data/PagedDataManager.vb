Imports System.Reflection
Imports Singular.Paging

Namespace Data

  Public Class SelectedItem
    Public Property ID As Integer? = Nothing
    Public Property Description As String = ""
  End Class

  Public Class PagedDataManager(Of VMType)
    Implements Data.JS.ITypeRenderer

    Private mDataSourcePropertyName As String
    Private mCriteriaPropertyName As String = ""
    Private mListType As Type
    Private mCriteriaType As Type

    <System.ComponentModel.Browsable(False)> Public Property DataSourceProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object))
    <System.ComponentModel.Browsable(False)> Public Property InitialSortProperty As String
    <System.ComponentModel.Browsable(False)> Public Property PageSize As Integer
    <System.ComponentModel.Browsable(False)> Public Property SortAsc As Boolean
    <System.ComponentModel.Browsable(False)> Public Property SelectedItems As List(Of SelectedItem)
    <System.ComponentModel.Browsable(False)> Public Property SingleSelect As Boolean
    <System.ComponentModel.Browsable(False)> Public Property MultiSelect As Boolean

    Public Sub New(DataSourceProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)),
                   CriteriaProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)), InitialSortProperty As String, PageSize As Integer,
                   Optional SortAsc As Boolean = True)
      Setup(DataSourceProperty, CriteriaProperty, InitialSortProperty, PageSize, SortAsc)
    End Sub

    Public Sub New(DataSourceProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)), InitialSortProperty As String, PageSize As Integer,
                   Optional SortAsc As Boolean = True)
      Setup(DataSourceProperty, Nothing, InitialSortProperty, PageSize, SortAsc)
    End Sub

    Private Sub Setup(DataSourceProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)),
                   CriteriaProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)), InitialSortProperty As String, PageSize As Integer,
                   SortAsc As Boolean)
      Me.DataSourceProperty = DataSourceProperty
      Me.PageSize = PageSize
      Me.InitialSortProperty = InitialSortProperty
      Me.SortAsc = SortAsc

      Dim miDS = Singular.Reflection.GetMember(Of VMType)(DataSourceProperty)
      mListType = CType(miDS, System.Reflection.PropertyInfo).PropertyType

      If CriteriaProperty IsNot Nothing Then
        Dim miCrit = Singular.Reflection.GetMember(Of VMType)(CriteriaProperty)
        mCriteriaPropertyName = miCrit.Name
        mCriteriaType = CType(miCrit, System.Reflection.PropertyInfo).PropertyType
      End If

      mDataSourcePropertyName = miDS.Name

      SelectedItems = New List(Of SelectedItem)

    End Sub

    Public Function GetInitialData() As Object

      If mCriteriaType IsNot Nothing Then
        Return GetInitialData(mCriteriaType)
      Else
        Return GetInitialData(mListType.GetNestedType("Criteria", BindingFlags.Public Or BindingFlags.NonPublic))
      End If

    End Function

    Public Function GetInitialData(CriteriaType As Type) As Object

      If mCriteriaType Is Nothing Then
        mCriteriaType = CriteriaType
      ElseIf mCriteriaType <> CriteriaType Then
        Throw New Exception("Criteria Type is different from type specified in constructor")
      End If

      Dim Criteria As IPageCriteria = Activator.CreateInstance(mCriteriaType)
      Return GetInitialData(Criteria)

    End Function

    Public Function GetInitialData(CriteriaObject As IPageCriteria) As Object

      CriteriaObject.PageSize = PageSize
      CriteriaObject.SortAsc = SortAsc
      CriteriaObject.SortColumn = InitialSortProperty

      Return Singular.Reflection.FetchList(mListType, CriteriaObject)

    End Function

    Private Sub New()

    End Sub

    Public Sub RenderData(Inst As Object, JW As Singular.Web.Data.JSonWriter, Member As Data.JS.ObjectInfo.Member, ByRef UseDefaultRenderer As Boolean) Implements Data.JS.ITypeRenderer.RenderData

      If Member.JSSerialiser.IsInitial Then

        Dim TInst As PagedDataManager(Of VMType) = Inst

        JW.StartClass(Member.JSonPropertyName)
        JW.WriteProperty("PropertyName", TInst.mDataSourcePropertyName)
        JW.WriteProperty("SortColumn", TInst.InitialSortProperty)
        If TInst.mCriteriaType Is Nothing Then
          JW.WriteProperty("TypeName", TInst.mListType.FullName & ", " & TInst.mListType.Assembly.GetName.Name)
        Else
          JW.WriteProperty("TypeName", TInst.mCriteriaType.FullName & ", " & TInst.mCriteriaType.Assembly.GetName.Name)
        End If
        JW.WriteProperty("PageSize", TInst.PageSize)
        JW.WriteProperty("SortAsc", TInst.SortAsc)
        If TInst.mCriteriaPropertyName <> "" Then
          JW.WriteProperty("CriteriaProperty", TInst.mCriteriaPropertyName)
        End If

        JW.EndClass()

      End If

    End Sub

    Public Sub RenderModel(JW As Singular.Web.Utilities.JavaScriptWriter, Member As Data.JS.ObjectInfo.Member) Implements Data.JS.ITypeRenderer.RenderModel
      JW.Write("CreateTypedROProperty(self, '" & Member.JSonPropertyName & "', PageInfoManager, false);")
    End Sub

    Public ReadOnly Property TypeRenderingMode As Data.JS.RenderTypeOption Implements Data.JS.ITypeRenderer.TypeRenderingMode
      Get
        Return JS.RenderTypeOption.RenderNoTypes
      End Get
    End Property

    Public ReadOnly Property RendersData As Boolean Implements JS.ITypeRenderer.RendersData
      Get
        Return True
      End Get
    End Property

  End Class

  'Public Class EditablePagedDataManager(Of VMType)
  '  Implements Data.JS.ITypeRenderer

  '  Private mDataSourcePropertyName As String
  '  Private mCriteriaPropertyName As String = ""
  '  Private mListType As Type

  '  Public Property DataSourceProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object))
  '  Public Property InitialSortProperty As String
  '  Public Property PageSize As Integer

  '  Public Sub New(DataSourceProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)),
  '                 CriteriaProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)), InitialSortProperty As String, PageSize As Integer)
  '    Setup(DataSourceProperty, CriteriaProperty, InitialSortProperty, PageSize)
  '  End Sub

  '  Public Sub New(DataSourceProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)), InitialSortProperty As String, PageSize As Integer)
  '    Setup(DataSourceProperty, Nothing, InitialSortProperty, PageSize)
  '  End Sub

  '  Private Sub Setup(DataSourceProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)),
  '                 CriteriaProperty As System.Linq.Expressions.Expression(Of System.Func(Of VMType, Object)), InitialSortProperty As String, PageSize As Integer)
  '    Me.DataSourceProperty = DataSourceProperty
  '    Me.PageSize = PageSize
  '    Me.InitialSortProperty = InitialSortProperty

  '    Dim miDS = Singular.Reflection.GetMember(Of VMType)(DataSourceProperty)
  '    mListType = CType(miDS, System.Reflection.PropertyInfo).PropertyType
  '    mDataSourcePropertyName = miDS.Name

  '    If CriteriaProperty IsNot Nothing Then
  '      Dim miCrit = Singular.Reflection.GetMember(Of VMType)(CriteriaProperty)
  '      mCriteriaPropertyName = miCrit.Name
  '    End If

  '  End Sub

  '  Public Function GetInitialData() As Object

  '    Dim CritType = mListType.GetNestedType("Criteria", BindingFlags.Public Or BindingFlags.NonPublic)
  '    Dim Criteria As IEditablePageCriteria = Activator.CreateInstance(CritType)
  '    Criteria.PageSize = PageSize
  '    Criteria.SortColumn = InitialSortProperty

  '    Return Singular.Reflection.FetchList(mListType, Criteria)

  '  End Function

  '  Private Sub New()

  '  End Sub

  '  Public Sub RenderData(Inst As Object, JW As Singular.Web.Data.JSonWriter, Member As Data.JS.ObjectInfo.Member) Implements Data.JS.ITypeRenderer.RenderData

  '    Dim TInst As EditablePagedDataManager(Of VMType) = Inst

  '    JW.StartClass(Member.JSonPropertyName)
  '    JW.WriteProperty("PropertyName", TInst.mDataSourcePropertyName)
  '    JW.WriteProperty("SortColumn", TInst.InitialSortProperty)
  '    JW.WriteProperty("TypeName", TInst.mListType.FullName & ", " & TInst.mListType.Assembly.GetName.Name)
  '    JW.WriteProperty("PageSize", TInst.PageSize)
  '    If TInst.mCriteriaPropertyName <> "" Then
  '      JW.WriteProperty("CriteriaProperty", TInst.mCriteriaPropertyName)
  '    End If

  '    JW.EndClass()

  '  End Sub

  '  Public Sub RenderModel(JW As Singular.Web.Utilities.JavaScriptWriter, Member As Data.JS.ObjectInfo.Member) Implements Data.JS.ITypeRenderer.RenderModel
  '    JW.Write("CreateTypedProperty(self, '" & Member.JSonPropertyName & "', EditablePageInfoManager, false, true, 1);")
  '  End Sub

  '  Public ReadOnly Property RenderType As Boolean Implements Data.JS.ITypeRenderer.RenderType
  '    Get
  '      Return False
  '    End Get
  '  End Property

  '  Public ReadOnly Property RendersData As Boolean Implements JS.ITypeRenderer.RendersData
  '    Get
  '      Return True
  '    End Get
  '  End Property
  'End Class

End Namespace

