Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Web.UI

Namespace CustomControls

  Public Class EditableHTMLControl
    Inherits Singular.Web.Controls.CustomWebControl

#Region " Create Controls / Rendering "

    Public Property EditRole As String = ""

    Private mCanEdit As Boolean = False
    Private mCanEditSet As Boolean = False
    Public ReadOnly Property CanEdit As Boolean
      Get
        If mCanEditSet Then
          Return mCanEdit
        End If
        If EditRole <> "" Then
          mCanEdit = Singular.Security.HasAccess(EditRole)
        Else
          mCanEdit = Singular.Security.HasAccess(ContentEditing.DefaultEditRole)
        End If
        mCanEditSet = True
        Return mCanEdit
      End Get
    End Property

    Protected Overrides Sub CreateChildControls()
      MyBase.CreateChildControls()

      If Mode = ModeType.View AndAlso Not CanEdit Then
        Exit Sub
      End If

      If Not mUseGlobalMode Then

        'Create the edit / save buttons in the control if Global Mode is set to false.
        Select Case Mode
          Case ModeType.View
            If CanEdit Then
              Dim btnEdit As New Button
              btnEdit.CommandName = "Edit"
              btnEdit.Text = "Edit " & ContentName
              AddHandler btnEdit.Click, AddressOf EditorButton_Click
              Me.Controls.Add(btnEdit)
            End If

          Case ModeType.Edit
            Dim btnSave As New Button
            btnSave.Text = "Save"
            btnSave.CommandName = "Save"
            AddHandler btnSave.Click, AddressOf EditorButton_Click
            Me.Controls.Add(btnSave)

            Dim btnCancel As New Button
            btnCancel.Text = "Cancel"
            btnCancel.CommandName = "Cancel"
            AddHandler btnCancel.Click, AddressOf EditorButton_Click
            Me.Controls.Add(btnCancel)

            Dim btnPreview As New Button
            btnPreview.Text = "Preview"
            btnPreview.CommandName = "Preview"
            AddHandler btnPreview.Click, AddressOf EditorButton_Click
            Me.Controls.Add(btnPreview)

          Case ModeType.Preview
            Dim btnDone As New Button
            btnDone.Text = "Done"
            btnDone.CommandName = "Done"
            btnDone.Style("position") = "fixed"
            btnDone.Style("top") = "5px"
            btnDone.Style("left") = "5px"
            AddHandler btnDone.Click, AddressOf EditorButton_Click
            Me.Controls.Add(btnDone)


        End Select

      End If

      Select Case Mode

        Case ModeType.View

          Dim lContent As New Literal()
          lContent.Text = ContentEditing.GetContent(ContentName).HTMLContentFormatted
          Me.Controls.Add(lContent)

        Case ModeType.Preview

          Dim divContent As New HtmlGenericControl("div")
          divContent.InnerHtml = ContentEditing.GetContent(ContentName).PreviewHTMLFormatted
          Me.Controls.Add(divContent)

        Case ModeType.Edit

          Me.Controls.Add(New HtmlGenericControl("br"))

          If EditRawHTMLOnly Then

            Dim editor As New HtmlTextArea
            editor.ID = "txtEditor"
            editor.Attributes("data-htmlEdit") = "encode"
            editor.InnerHtml = PreviewHTML
            editor.Style("width") = Width & "px"
            editor.Style("height") = Height & "px"
            Me.Controls.Add(editor)

          Else
            Dim editor As New Winthusiasm.HtmlEditor.HtmlEditor
            editor.ID = "txtEditor"
            editor.Text = PreviewHTML
            editor.Width = New Unit(Width, UnitType.Pixel)
            editor.Height = New Unit(Height, UnitType.Pixel)
            editor.OutputXHTML = False
            If DesignModeCSS <> "" Then
              editor.DesignModeCss = Utils.URL_ToAbsolute(DesignModeCSS)
            ElseIf ContentEditing.DesignModeCSS <> "" Then
              editor.DesignModeCss = Utils.URL_ToAbsolute(ContentEditing.DesignModeCSS)
            End If
            Me.Controls.Add(editor)
          End If




      End Select

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      If Mode = ModeType.View AndAlso Not CanEdit Then
        Writer.Write(ContentEditing.GetContent(ContentName).HTMLContentFormatted)
      End If

    End Sub

#End Region

#Region " Properties "

    Public Enum ModeType
      View = 1
      Edit = 2
      Preview = 3
    End Enum

    Public Property Mode As ModeType
      Get
        If mUseGlobalMode Then
          Return mGlobalMode
        Else
          Return If(State("Mode") Is Nothing, ModeType.View, State("Mode"))
        End If
      End Get
      Set(value As ModeType)
        State("Mode") = value
      End Set
    End Property

    Private Shared mGlobalMode As ModeType = ModeType.View
    Public Shared Property GlobalMode As ModeType
      Get
        Return mGlobalMode
      End Get
      Set(value As ModeType)
        mGlobalMode = value
      End Set
    End Property

    Public Shared Property EditRawHTMLOnly As Boolean = False

    Private Shared mUseGlobalMode As Boolean = False
    Public Shared Property UseGlobalMode As Boolean
      Get
        Return mUseGlobalMode
      End Get
      Set(value As Boolean)
        mUseGlobalMode = value
      End Set
    End Property

    Public Property ContentName As String
      Get
        Dim Value As String = State("ContentName")
        If Value Is Nothing Then
          Return ""
        Else
          Return Value
        End If
      End Get
      Set(value As String)
        State("ContentName") = value
      End Set
    End Property

    Private mDesignModeCSS As String = ""
    Public Property DesignModeCSS As String
      Get
        Return mDesignModeCSS
      End Get
      Set(value As String)
        mDesignModeCSS = value
      End Set
    End Property

    Public Property HTML As String
      Get
        Return ContentEditing.GetContent(ContentName).HTMLContent
      End Get
      Set(value As String)
        ContentEditing.GetContent(ContentName).HTMLContent = value
      End Set
    End Property

    Public Property PreviewHTML As String
      Get
        Return ContentEditing.GetContent(ContentName).PreviewHTML
      End Get
      Set(value As String)
        ContentEditing.GetContent(ContentName).PreviewHTML = value
      End Set
    End Property

    ''' <summary>
    ''' Width in Pixels
    ''' </summary>
    Public Property Width As Integer = 800

    ''' <summary>
    ''' Height in Pixels
    ''' </summary>
    Public Property Height As Integer = 600

#End Region

#Region " Button Click Logic "

    Private Sub EditorButton_Click(sender As Object, e As EventArgs)
      Dim btn As Button = sender

      Select Case btn.CommandName
        Case "Edit"
          PreviewHTML = HTML
          Mode = ModeType.Edit
          ContentEditing.ResetExpiry()

        Case "Save"
          If Mode = ModeType.Edit Then
            If EditRawHTMLOnly Then
              HTML = Server.HtmlDecode(CType(FindControl("txtEditor"), HtmlTextArea).InnerText)
            Else
              HTML = CType(FindControl("txtEditor"), Winthusiasm.HtmlEditor.HtmlEditor).Text
            End If

          ElseIf Mode = ModeType.Preview Then
            HTML = PreviewHTML
          End If
          Mode = ModeType.View

        Case "Preview"
          If EditRawHTMLOnly Then
            PreviewHTML = Server.HtmlDecode(CType(FindControl("txtEditor"), HtmlTextArea).InnerText)
          Else
            PreviewHTML = CType(FindControl("txtEditor"), Winthusiasm.HtmlEditor.HtmlEditor).Text
          End If

          Mode = ModeType.Preview

        Case "Cancel"
          Mode = ModeType.View

        Case "Done"
          Mode = ModeType.Edit

      End Select

      Me.Controls.Clear()
      CreateChildControls()

    End Sub

#End Region

    Public Shared Function GetHTMLContent(ContentName As String) As String

      Return ContentEditing.GetContent(ContentName).HTMLContent

    End Function

  End Class

End Namespace