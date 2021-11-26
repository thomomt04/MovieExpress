Imports System.Reflection

Namespace CustomControls

  Public Class PagedGrid(Of ObjectType, ChildControlObjectType)
    Inherits Table(Of ObjectType, ChildControlObjectType)

    'This code is duplicated in the PagedData control. Unfortunately this was designed incorrectly, 
    'and cannot be converted to inherit from PagedData without breaking projects.

    Public Property PagingManager As System.Linq.Expressions.Expression(Of System.Func(Of ObjectType, Object))

    Private _PagingManagerProperty As PropertyInfo

    Private _LoadingBar As LoadingOverlay(Of ObjectType)
    Private _PageControls As Singular.Web.Controls.HelperControls.HelperBase(Of ObjectType)

    Public Property PageControlsType As PageControlsType = Singular.Web.Controls.DefaultPageControlsType

#Region " Old Crap "

    Public ReadOnly Property ButtonClass As String
      Get
        Return CType(_PageControls, PageControlsWithInput(Of ObjectType)).ButtonClass
      End Get
    End Property

    Public ReadOnly Property BtnFirst As HTMLTag(Of ObjectType)
      Get
        Return CType(_PageControls, PageControlsWithInput(Of ObjectType)).BtnFirst
      End Get
    End Property

    Public ReadOnly Property BtnPrev As HTMLTag(Of ObjectType)
      Get
        Return CType(_PageControls, PageControlsWithInput(Of ObjectType)).BtnPrev
      End Get
    End Property

    Public ReadOnly Property BtnNext As HTMLTag(Of ObjectType)
      Get
        Return CType(_PageControls, PageControlsWithInput(Of ObjectType)).BtnNext
      End Get
    End Property

    Public ReadOnly Property BtnLast As HTMLTag(Of ObjectType)
      Get
        Return CType(_PageControls, PageControlsWithInput(Of ObjectType)).BtnLast
      End Get
    End Property

#End Region

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AllowClientSideSorting = False

      _PagingManagerProperty = Singular.Reflection.GetMember(PagingManager)

      _PageControls = PageControlsBase(Of ObjectType).GetInstance(_PagingManagerProperty, PageControlsType)
      Helpers.Control(_PageControls)

      _LoadingBar = New LoadingOverlay(Of ObjectType)
      Helpers.Control(_LoadingBar)
      _LoadingBar.AddBinding(KnockoutBindingString.visible, _PagingManagerProperty.Name & "().IsLoading")

    End Sub

    Protected Friend Overrides Sub Render()

      If Me.Style("width") = "" Then
        Me.Style("width") = "100%"
      End If

      'Render the containing div
      Writer.Write("<div style=""width:" & Me.Style("width") & "; position: relative;"" data-bind=""PagedGrid: " & _PagingManagerProperty.Name & """>")

      'Reset the width to 100%, as the table will use the same style
      Me.Style("width") = "100%"
      'Render the table
      MyBase.Render()

      'Render the paging toolbar
      _PageControls.Render()
      _LoadingBar.Render()

      Writer.Write("</div>")

    End Sub

  End Class

End Namespace
