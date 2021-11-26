Imports System.Reflection
Imports System.Dynamic
Imports System.Linq.Expressions
Imports Singular.Web.Controls.HelperControls
Imports System.ComponentModel.DataAnnotations
Imports Singular.Web.Controls

Namespace CustomControls

  ''' <summary>
  ''' Renders a button that displays a find popup.
  ''' </summary>
  Public Class FindScreen(Of ListType, ObjectType)
    Inherits HelperBase

    Public Property ButtonText As String
    Public Property ListTypeOverride As Type
    Public Property AutoPopulate As Boolean = False

    ''' <summary>
    ''' Called before the find screen is opened.
    ''' </summary>
    Public Property PreFindJSFunction As String = ""

    ''' <summary>
    ''' Called when the user clicks the search button after entering criteria
    ''' </summary>
    Public Property PreSearchJSFunction As String = ""

    ''' <summary>
    ''' Called when a row is selected in the results grid.
    ''' </summary>
    Public Property OnRowSelectJSFunction As String = ""

    Private mButton As Button(Of ObjectType)
    Private mDialog As SearchDialog(Of ListType)
    Private mCriteriaType As Type
    Friend Property MultiSelect As Boolean
    Public Property IsAsync As Boolean

    ''' <summary>
    ''' Setting this will add a col-x class to each criteria input. Note, this must be divide into 12.
    ''' </summary>
    Public Property DialogBootstrapColumnCount As Integer

    Public ReadOnly Property Button As Button(Of ObjectType)
      Get
        Return mButton
      End Get
    End Property

    Public ReadOnly Property Dialog As SearchDialog(Of ListType)
      Get
        Return mDialog
      End Get
    End Property

    Private ReadOnly Property OverriddenListType As Type
      Get
        If ListTypeOverride IsNot Nothing Then
          Return ListTypeOverride
        Else
          Return GetType(ListType)
        End If
      End Get
    End Property

    Private ReadOnly Property VMCriteriaPropertyName As String
      Get
        Return OverriddenListType.Name & "Criteria"
      End Get
    End Property

    Private ReadOnly Property FindContext As String
      Get
        Return OverriddenListType.Name & "Find"
      End Get
    End Property

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If Singular.Reflection.IsDerivedFromGenericType(OverriddenListType, GetType(Csla.CriteriaBase(Of ))) Then
        mCriteriaType = OverriddenListType
        ListTypeOverride = mCriteriaType.DeclaringType
      Else
        mCriteriaType = OverriddenListType.GetNestedType("Criteria", BindingFlags.Public Or BindingFlags.NonPublic)
      End If

      mButton = New Button(Of ObjectType)

      mButton.ButtonID = ""
      mButton.ButtonType = DefinedButtonType.Find
      mButton.Text = ButtonText
      AddControl(mButton)
      mButton.AddBinding(KnockoutBindingString.click,
                        "Singular.ShowScreenFind($data, ViewModel." & VMCriteriaPropertyName & ", " &
                        FindContext.AddSingleQuotes & ", " &
                        IsAsync.ToString.ToLower & ", " &
                        AutoPopulate.ToString.ToLower & ", " &
                        PreFindJSFunction.IfEmpty("null") & ", " &
                        MultiSelect.ToString.ToLower & ", " &
                        OnRowSelectJSFunction.IfEmpty("null") & ", " &
                        PreSearchJSFunction.IfEmpty("null") & ")")



      If Helpers.IPage.LateResources.CanAddFindDialog(VMCriteriaPropertyName) Then
        mDialog = New SearchDialog(Of ListType)
        mDialog.ListType = OverriddenListType
        mDialog.CriteriaType = mCriteriaType
        mDialog.VMCriteriaPropertyName = VMCriteriaPropertyName
        mDialog.MultiSelect = MultiSelect
        mDialog.BootstrapColumnCount = DialogBootstrapColumnCount
        AddControl(mDialog)
      End If


    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      mButton.Render()
      mDialog.Render()

    End Sub

  End Class

  Public Class SearchDialog(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Public Property DialogCaption As String
      Get
        Return mContainerDiv.Attributes("title")
      End Get
      Set(value As String)
        mContainerDiv.Attributes("title") = value
      End Set
    End Property

    Public Property ListType As Type
    Public Property CriteriaType As Type

    ''' <summary>
    ''' Allow additional instructions text above the criteria.
    ''' </summary>
    Public Property DialogInstructions As String
      Get
        Return mInstructions.Content
      End Get
      Set(value As String)
        mInstructions.Content = value
        mCriteriaHeading.Style.Margin(If(value = "", "0", "10px"))
      End Set
    End Property

    ''' <summary>
    ''' Allow additional instructions to be added above the criteria.
    ''' </summary>
    Public ReadOnly Property InstructionsDiv As HTMLDiv(Of ObjectType)
      Get
        Return mInstructionsDiv
      End Get
    End Property

    Friend Property MultiSelect As Boolean = False

    Public ReadOnly Property CriteriaGroup As HTMLDiv(Of ObjectType)
      Get
        Return mCriteriaGroup
      End Get
    End Property

    Public ReadOnly Property CriteriaList As List(Of EditorRow(Of ObjectType))
      Get
        Return mCriteriaList
      End Get
    End Property

    Public Overloads ReadOnly Property Style As CSSStyle
      Get
        Return mContainerDiv.Style
      End Get
    End Property

    Public ReadOnly Property ButtonContainer As HTMLDiv(Of ObjectType)
      Get
        Return mButtonsDiv
      End Get
    End Property

    Friend Property VMCriteriaPropertyName As String
    Friend Property AddVMProperty As Boolean = True

    ''' <summary>
    ''' Setting this will add a col-x class to each criteria input. Note, this must be divide into 12.
    ''' </summary>
    Public Property BootstrapColumnCount As Integer

    Private mInstructions As HTMLSnippet
    'Private mCriteriaPIs As New List(Of PropertyInfo)
    Private mSearchButton As Button(Of ObjectType)
    Private mAcceptButton As Button(Of ObjectType)

    Private mInstructionsDiv As HTMLDiv(Of ObjectType)
    Private mContainerDiv As HTMLDiv(Of ObjectType)
    Private mButtonsDiv As HTMLDiv(Of ObjectType)
    Private mCriteriaHeading As HTMLDiv(Of ObjectType)
    Private mCriteriaContainer As HTMLDiv(Of ObjectType)
    Private mCriteriaGroup As HTMLDiv(Of ObjectType)
    Private mCriteriaList As New List(Of EditorRow(Of ObjectType))
    Private mResultsTable As HTMLTag(Of ObjectType)

    Public Sub New(VMPropertyName As String)
      VMCriteriaPropertyName = VMPropertyName
      If Not String.IsNullOrEmpty(VMCriteriaPropertyName) Then
        AddVMProperty = False
      End If
    End Sub

    Public Sub New()

    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If ListType Is Nothing Then
        ListType = GetType(ObjectType)
        If Singular.Reflection.IsDerivedFromGenericType(ListType, GetType(Csla.CriteriaBase(Of ))) Then
          CriteriaType = ListType
          ListType = CriteriaType.DeclaringType
        Else
          CriteriaType = ListType.GetNestedType("Criteria", BindingFlags.Public Or BindingFlags.NonPublic)
        End If
        If String.IsNullOrEmpty(VMCriteriaPropertyName) Then
          VMCriteriaPropertyName = ListType.Name & "Criteria"
        End If
      End If


      mContainerDiv = Helpers.Div
      mContainerDiv.Attributes("ID") = VMCriteriaPropertyName
      mContainerDiv.Attributes("data-search") = VMCriteriaPropertyName
      mContainerDiv.Style.Display = Display.none

      'INSTRUCTIONS
      mInstructions = mContainerDiv.Helpers.HTML("")
      mInstructionsDiv = mContainerDiv.Helpers.DivC("criteria-instructions")

      'CRITERIA
      If CriteriaType IsNot Nothing Then

        CriteriaType.ForEachBrowsableProperty(
          Helpers.ContextList,
          Sub(pi)

            If mCriteriaHeading Is Nothing Then

              'CRITERIA GROUP
              'Header
              mCriteriaHeading = mContainerDiv.Helpers.Div
              mCriteriaHeading.AddClass("ui-widget-header").AddClass("criteria-header")
              mCriteriaHeading.Helpers.HTML("Criteria")

              mCriteriaContainer = mContainerDiv.Helpers.Div
              mCriteriaContainer.AddClass("Criteria")

              'Criteria Editors Container
              mCriteriaGroup = mCriteriaContainer.Helpers.Div
              mCriteriaGroup.AddBinding(KnockoutBindingString.with, "$root." & VMCriteriaPropertyName)

              If BootstrapColumnCount > 0 Then mCriteriaGroup.AddClass("row")

            End If

            Dim EditorRow = mCriteriaGroup.Helpers.EditorRowFor(pi)
            EditorRow.Editor.Attributes("data-PropertyName") = pi.Name
            EditorRow.AddClass("rowMulti")

            If BootstrapColumnCount > 0 Then EditorRow.AddClass("col-" & 12 / BootstrapColumnCount)

            mCriteriaList.Add(EditorRow)

          End Sub, False, True, True)

        'Add a float clear after the last editor
        If mCriteriaGroup IsNot Nothing Then
          mCriteriaGroup.Helpers.Div.Style.ClearBoth()
        End If

        'Create a criteria property on the viewmodel.
        If AddVMProperty Then
          Dim VMInfo = CType(Model, Singular.Web.IViewModel).JSSerialiser.RootObjectInfo
          Dim mi As Data.JS.ObjectInfo.ObjectMember = VMInfo.CreateMember(VMCriteriaPropertyName, CriteriaType)
          If mi IsNot Nothing Then
            mi.CreateNew = True
          End If
        End If

      End If

      mButtonsDiv = mContainerDiv.Helpers.Div
      With mButtonsDiv

        'SEARCH BUTTON
        mSearchButton = .Helpers.Button("dlg-Search", "Search")
        If mCriteriaList.Count > 0 Then
          mSearchButton.Attributes("title") = "Search using the above criteria."
        End If
        mSearchButton.Image.SrcDefined = DefinedImageType.Find
        mSearchButton.Image.Glyph = FontAwesomeIcon.search
        mSearchButton.PostBackType = PostBackType.None

        'CLEAR BUTTON
        mSearchButton = .Helpers.Button("dlg-Clear", "Clear")
        If mCriteriaList.Count > 0 Then
          mSearchButton.Attributes("title") = "Clear the values in the filters."
        End If
        mSearchButton.Image.SrcDefined = DefinedImageType.Clear
        mSearchButton.Image.Glyph = FontAwesomeIcon.eraser
        mSearchButton.PostBackType = PostBackType.None

        'ACCEPT BUTTON
        If MultiSelect Then
          mAcceptButton = .Helpers.Button("dlg-Accept", "Accept")
          mAcceptButton.Attributes("title") = "Accept the selected rows"
          mAcceptButton.Image.SrcDefined = DefinedImageType.Yes_GreenTick
          mAcceptButton.PostBackType = PostBackType.None
        End If

      End With

      Dim TableContainer = mContainerDiv.Helpers.Div
      TableContainer.Style.Height = 400
      TableContainer.Style.Overflow = Overflow.auto
      TableContainer.Style.Margin("5px")

      mResultsTable = TableContainer.Helpers.HTMLTag("table")
      mResultsTable.AddClass("Grid")
      mResultsTable.Style.Width = "100%"


      'Add the schema of the data
      Helpers.AddSchemaType(ListType)

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      WriterOverride = Helpers.IPage.LateResources.EndPageHTML
      RenderChildren()

    End Sub

  End Class

End Namespace


