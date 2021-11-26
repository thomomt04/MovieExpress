Namespace CustomControls

  'TODO: Change so that document object is passed in, and not created. Also change ImageChooser to take in property of DocumentID

  Public Class ImageChooser(Of ObjectType)
    Inherits Controls.HelperControls.HelperBase(Of ObjectType)

    Public Property PromptText As String = "Choose Image"
    Public Property RequiredWidth As Integer
    Public Property RequiredHeight As Integer
    Public Property Scale As Decimal
    Public Property OnCompleteFunctionName As String

    ''' <summary>
    ''' If blank, the background will be transparent, and the image will be saved according to its original dimensions if smaller than the required size.
    ''' If a background color is specified, the image will be bordered with this color, and will be the required diminesions
    ''' </summary>
    Public Property BackColor As String

    Private mControl As Controls.HelperControls.HelperBase

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      Dim Params As String = RequiredWidth & ", " & RequiredHeight & ", " & Scale & ", " & GetForJS() & ", " & BackColor.AddSingleQuotes

      'The check for if a browser supports FileAPI should be done client side, but this results in less html being sent to the browser.

      If Singular.Web.Misc.BroswerInfo.SupportsFileAPI Then
        'Add a button that creates a file input control
        Dim Btn = Helpers.Button(PromptText, Singular.Web.ButtonMainStyle.Default, Singular.Web.ButtonSize.Normal, Singular.Web.FontAwesomeIcon.fileOutline)
        With Btn
          .AddBinding(Singular.Web.KnockoutBindingString.click, "ImageResizeHelper.Browse(" & Params & ")")
          .AddBinding(Singular.Web.KnockoutBindingString.visible, "GetIEVersion() > 9")
        End With
        mControl = Btn
      Else
        'Add a file input control
        Dim fe As New Singular.Web.CustomControls.FileEditor(Of Singular.Documents.TemporaryDocument)
        fe.OnChangeJS = "ImageChosen(this, " & Params & ")"
        fe.ImagesOnly = True
        Helpers.Control(fe)
        fe.AddBinding(Singular.Web.KnockoutBindingString.visible, "GetIEVersion() <= 9")
        mControl = fe
      End If

    End Sub

    Public Overloads ReadOnly Property Style As CSSStyle
      Get
        Return mControl.Style
      End Get
    End Property

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub
  End Class

  Public Class ImageViewer(Of ObjectType)
    Inherits Controls.HelperControls.HelperBase(Of ObjectType)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      AddBinding(Singular.Web.KnockoutBindingString.visible, GetForJS() & "()")
      AddBinding(Singular.Web.KnockoutBindingString.src, "ImageResizeHelper.GetImageURL(" & GetForJS() & "())")

      Style("max-width") = "100%"
    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      With Writer

        WriteFullStartTag("img", Singular.Web.Controls.HelperControls.TagType.SelfClosing)
      End With

    End Sub
  End Class

  Public Class ImageResizeDialog
    Inherits Controls.HelperControls.HelperBase(Of Object)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers.Dialog("ImageResizeHelper.IsVisible", "'" + Singular.Localisation.Localisation.LocalText("Resize Image") + "'", "ImageResizeHelper.Close")
        .AllowResize = False
        .WidthBinding = "ImageResizeHelper.GetDialogWidth()"

        .Helpers.Span(Singular.Localisation.Localisation.LocalTextDefaultOverride("Resize and Move Image Instructions", "Please move the image so that it fits in the box, you can move the image by dragging it, and zoom by clicking the + and - buttons."))

        With .Helpers.Div

          With .Helpers.HTMLTag("canvas")
            .Attributes("id") = "IRCanvas"
            .AddBinding(Singular.Web.KnockoutBindingString.attr, "ImageResizeHelper.GetCanvasAttributes()")
            .AddBinding(Singular.Web.KnockoutBindingString.style, "ImageResizeHelper.GetCanvasStyle()")
            .Style.BackgroundColour = "#222"
            .Style.MarginI(10)
            .Style("border") = "1px solid #222"
          End With

        End With

        With .Helpers.With(Of Object)("ImageResizeHelper.Current")

          With .Helpers.Div
            .AddBinding(KnockoutBindingString.visible, "CurrentDoc() == null")

            With .Helpers.Button("", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.ExtraSmall, Singular.Web.FontAwesomeIcon.zoomout)
              .AddBinding(Singular.Web.KnockoutBindingString.click, "Zoom(1/1.2)")
            End With
            With .Helpers.Button("", Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.ExtraSmall, Singular.Web.FontAwesomeIcon.zoomin)
              .AddBinding(Singular.Web.KnockoutBindingString.click, "Zoom(1.2)")
            End With
            With .Helpers.Button(Singular.Localisation.Localisation.LocalText("Fit"), Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.ExtraSmall, Singular.Web.FontAwesomeIcon.circleOutline)
              .Style.MarginLeft("15px")
              .AddBinding(Singular.Web.KnockoutBindingString.click, "Fit()")
            End With
            With .Helpers.Button(Singular.Localisation.Localisation.LocalText("Fill"), Singular.Web.ButtonMainStyle.Primary, Singular.Web.ButtonSize.ExtraSmall, Singular.Web.FontAwesomeIcon.circle)
              .AddBinding(Singular.Web.KnockoutBindingString.click, "Fill()")
            End With

            With .Helpers.Button(Singular.Localisation.Localisation.LocalText("Save"), Singular.Web.ButtonMainStyle.Success, Singular.Web.ButtonSize.ExtraSmall, Singular.Web.FontAwesomeIcon.download)
              .AddBinding(Singular.Web.KnockoutBindingString.click, "Save()")
              .Style.FloatRight()
            End With
          End With

        End With

      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()
      RenderChildren()
    End Sub

  End Class

End Namespace
