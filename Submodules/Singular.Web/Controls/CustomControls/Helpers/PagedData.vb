Imports System.Reflection

Namespace CustomControls

  Public Class PagedData(Of ObjectType, ChildControlObjectType, InnerControlType As Singular.Web.Controls.HelperControls.HelperBase(Of ObjectType, ChildControlObjectType))
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of ObjectType, ChildControlObjectType)

    Private _PagingManagerProperty As PropertyInfo
    Private _PageContainer As HTMLDiv(Of ChildControlObjectType)
    Private _LoadingBar As LoadingOverlay(Of ObjectType)
    Private _PageControls As Singular.Web.Controls.HelperControls.HelperBase(Of ObjectType)

    Protected _InnerControl As InnerControlType

    Public Property PageControlsType As PageControlsType = Singular.Web.Controls.DefaultPageControlsType

    Public Sub New(PagingManagerExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)),
                   InnerControl As InnerControlType)
      _PagingManagerProperty = Singular.Reflection.GetMember(PagingManagerExpression)
      _InnerControl = InnerControl
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      OnlyChildControls = True

      _PageContainer = Helpers.Div
      For Each c As String In GetCSSClasses.Keys
        If GetCSSClasses()(c) Then
          _PageContainer.AddClass(c)
        End If
      Next

      _PageContainer.Style.Position = Position.relative
      _PageContainer.Bindings.AddCustom("PagedGrid", _PagingManagerProperty.Name)

      _InnerControl.For(PropertyInfo)
      _PageContainer.Helpers.Control(_InnerControl)

      'Bottom toolbar
      _PageControls = PageControlsBase(Of ObjectType).GetInstance(_PagingManagerProperty, PageControlsType)
      _PageContainer.Helpers.Control(_PageControls)

      _LoadingBar = New LoadingOverlay(Of ObjectType)
      Helpers.Control(_LoadingBar)
      _LoadingBar.AddBinding(KnockoutBindingString.visible, _PagingManagerProperty.Name & "().IsLoading")

    End Sub

  End Class

  Public Enum PageControlsType
    ButtonsWithInput = 1
    ButtonsOnly = 2
  End Enum

  Public Class PageControlsBase(Of ObjectType)
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of ObjectType)

    Protected _PagingManagerProperty As PropertyInfo

    Public Sub New(PagingManagerProperty As PropertyInfo)
      _PagingManagerProperty = PagingManagerProperty
    End Sub

    Public Shared Function GetInstance(PagingManagerProperty As PropertyInfo, Type As PageControlsType) As PageControlsBase(Of ObjectType)
      If Type = PageControlsType.ButtonsWithInput Then
        Return New PageControlsWithInput(Of ObjectType)(PagingManagerProperty)
      ElseIf Type = PageControlsType.ButtonsOnly Then
        Return New PageControlsButtonsOnly(Of ObjectType)(PagingManagerProperty)
      Else
        Throw New Exception("Unknown PageControlsType")
      End If
    End Function

  End Class

  Public Class PageControlsWithInput(Of ObjectType)
    Inherits PageControlsBase(Of ObjectType)

    Public Property ButtonClass As String = "TBButton"

    Public Property BtnFirst As HTMLTag(Of ObjectType)
    Public Property BtnPrev As HTMLTag(Of ObjectType)
    Public Property BtnNext As HTMLTag(Of ObjectType)
    Public Property BtnLast As HTMLTag(Of ObjectType)

    Private _PagerDiv As HTMLDiv(Of ObjectType)

    Public Sub New(PagingManagerProperty As PropertyInfo)
      MyBase.New(PagingManagerProperty)
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      OnlyChildControls = True

      _PagerDiv = Helpers.Div()
      _PagerDiv.AddClass("Pager")
      With _PagerDiv.Helpers

        'Left section
        With .Div
          .Style.FloatLeft()
          .AddClass("Buttons")

          'First / Prev
          BtnFirst = .Helpers.HTMLTag("button")
          With BtnFirst
            .AddClass("TBElement " & ButtonClass)
            .Style.BackgroundImage = "url(" & Utils.URL_ToAbsolute("~/Images/TBFirst.png") & ")"
          End With
          BtnPrev = .Helpers.HTMLTag("button")
          With BtnPrev
            .AddClass("TBElement " & ButtonClass)
            .Style.BackgroundImage = "url(" & Utils.URL_ToAbsolute("~/Images/TBPrev.png") & ")"
          End With
          .Helpers.HTMLTag("span").AddClass("TBElement TBSeperator")

          'Page x of x
          .Helpers.HTMLTag("span", "Page ").AddClass("TBElement")

          Dim PageXOf = .Helpers.HTMLTag("input")
          With PageXOf
            .Style.Width = 30
            .Attributes("type") = "text"
            .AddBinding(KnockoutBindingString.NValue, _PagingManagerProperty.Name & "().PageNo")
          End With
          With .Helpers.HTMLTag("span")
            .AddClass("TBElement")
            .AddBinding(KnockoutBindingString.text, "' of ' + " & _PagingManagerProperty.Name & "().Pages()")
          End With


          'Next / Last
          .Helpers.HTMLTag("span").AddClass("TBElement TBSeperator")

          BtnNext = .Helpers.HTMLTag("button")
          With BtnNext
            .AddClass("TBElement " & ButtonClass)
            .Style.BackgroundImage = "url(" & Utils.URL_ToAbsolute("~/Images/TBNext.png") & ")"
          End With
          BtnLast = .Helpers.HTMLTag("button")
          With BtnLast
            .AddClass("TBElement " & ButtonClass)
            .Style.BackgroundImage = "url(" & Utils.URL_ToAbsolute("~/Images/TBLast.png") & ")"
          End With

        End With
        'Right section
        With .Div
          .Style.FloatRight()
          .AddClass("Status")

          .Helpers.HTMLTag("span").AddClass("TBElement TBSeperator")
          .Helpers.HTMLTag("span").AddBinding(KnockoutBindingString.text, _PagingManagerProperty.Name & "().PageDescription()")
        End With
        .Div.Style.ClearBoth()

      End With

    End Sub

  End Class

  Public Class PageControlsButtonsOnly(Of ObjectType)
    Inherits PageControlsBase(Of ObjectType)

    Public Sub New(PagingManagerProperty As PropertyInfo)
      MyBase.New(PagingManagerProperty)
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      OnlyChildControls = True

      Dim PageManagerJS = "ViewModel." & _PagingManagerProperty.Name & "()"

      With Helpers.DivC("page-control-buttons")

        With .Helpers.Button("Previous", ButtonMainStyle.Default, ButtonSize.Small, FontAwesomeIcon.None)
          .ClickJS = PageManagerJS & ".Prev()"
        End With

        'Create a button for each page
        With .Helpers.ForEach("new Array(" & PageManagerJS & ".Pages())")

          With .Helpers.Button("", ButtonMainStyle.Default, ButtonSize.Small, FontAwesomeIcon.None)

            'Set the text to the index.
            .ButtonText.AddBinding(KnockoutBindingString.text, "$index() + 1")

            'Add 'active' class to selected page.
            .AddBinding(KnockoutBindingString.css, PageManagerJS & ".PageNo() == $index() + 1 ? 'active' : ''")

            'On click, set the page no to the index.
            .ClickJS = PageManagerJS & ".PageNo($index() + 1)"
          End With

        End With

        With .Helpers.Button("Next", ButtonMainStyle.Default, ButtonSize.Small, FontAwesomeIcon.None)
          .ClickJS = PageManagerJS & ".Next()"
        End With

      End With

    End Sub

  End Class

  Public Class PagedForEach(Of ObjectType, ChildControlObjectType)
    Inherits PagedData(Of ObjectType, ChildControlObjectType, ForEach(Of ObjectType, ChildControlObjectType))

    Public Sub New(PagingManagerExpression As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object)))
      MyBase.New(PagingManagerExpression, New ForEach(Of ObjectType, ChildControlObjectType)())
    End Sub

    Public ReadOnly Property ForEach As ForEach(Of ObjectType, ChildControlObjectType)
      Get
        Return _InnerControl
      End Get
    End Property

  End Class

End Namespace
