Imports System.Reflection

Namespace SystemSettings

  Public Class SystemSettingsControl
    Inherits Controls.HelperControls.HelperBase(Of SystemSettingsVM)

    Private mShowDescriptionsInline As Boolean

    Public Sub New()

    End Sub

    Public Sub New(ShowDescriptionsInline As Boolean)
      mShowDescriptionsInline = ShowDescriptionsInline
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers.Toolbar
        .Helpers.HTML.Heading2("System Settings")

        If Model.SystemSetting IsNot Nothing Then
          With .Helpers.Button(Singular.Web.DefinedButtonType.Undo)
            .ButtonText.AddBinding(KnockoutBindingString.text, "ViewModel.SystemSetting().IsDirty() ? 'Undo' : 'Back'")
            .AddBinding(KnockoutBindingString.click, "window.location = window.location.pathname;")
          End With
          .Helpers.Button(Singular.Web.DefinedButtonType.Save).AddBinding(KnockoutBindingString.click, "Save()")
        End If

      End With

      Helpers.MessageHolder()

      If Model.SelectInstance Is Nothing Then

        If Model.SystemSetting Is Nothing Then

          Helpers.HTML("Please select which settings to edit.")

          With Helpers.DivC("SettingsContainer")

            Dim HaveSettings As Boolean = False

            For Each ss As Singular.SystemSettings.Objects.SystemSetting In Model.SystemSettingList
              If ss.Settings IsNot Nothing Then

                If ss.Settings.SecurityRole = "" OrElse Singular.Security.HasAccess(ss.Settings.SecurityRole) Then

                  HaveSettings = True

                  With .Helpers.DivC("Section")

                    With .Helpers.DivC("Heading")
                      .Helpers.HTML(ss.SystemSetting)

                    End With

                    With .Helpers.DivC("Details")

                      .Helpers.HTML(ss.Settings.Description)
                      .Helpers.HTML.Gap()
                      With .Helpers.Button("Edit", ButtonMainStyle.Warning, ButtonSize.Small, FontAwesomeIcon.edit)
                        .AddBinding(KnockoutBindingString.click, "window.location = '?Name=" & ss.SystemSetting & "'")
                        .Image.SrcDefined = Singular.Web.DefinedImageType.BlankPage
                        .Image.Glyph = FontAwesomeIcon.edit
                      End With
                    End With

                  End With

                End If

              End If
            Next

            If Not HaveSettings Then
              If Model.SystemSettingList.count > 0 Then
                .Helpers.Span("You do not have access rights to edit System Settings")
              Else
                .Helpers.Span("No system settings available in this project.")
              End If
            End If

          End With

        Else

          Dim mCategoryList As New Hashtable


          'Edit
          'Dim x As New Dictionary(Of String, List(Of System.Reflection.PropertyInfo))
          'For Each pi As System.Reflection.PropertyInfo In Model.SystemSetting.GetType.GetProperties(BindingFlags.Public Or BindingFlags.Instance).Where(Function(f) Singular.Reflection.CanSerialiseField(f))
          '  Dim ca = Singular.Reflection.GetAttribute(Of System.ComponentModel.CategoryAttribute)(pi)
          '  Dim Category = "General"
          '  If ca IsNot Nothing Then
          '    Category = ca.Category
          '  End If
          '  If Not x.ContainsKey(Category) Then x.Add(Category, New List(Of System.Reflection.PropertyInfo))
          '  Dim Order As Integer? = Singular.Reflection.GetAttribute(Of System.ComponentModel.DataAnnotations.DisplayAttribute)(pi).GetOrder()
          '  If Order.HasValue AndAlso x(Category).Count > Order Then
          '    x(Category).Insert(Order - 1, pi)
          '  Else
          '    x(Category).Add(pi)
          '  End If
          'Next

          With Helpers.With(Of Singular.SystemSettings.ISettingsSection)(Function(c) c.SystemSetting)
            For Each pi As System.Reflection.PropertyInfo In Model.SystemSetting.GetType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
              If Singular.Reflection.AutoGenerateField(pi) Then

                'Get the category.
                Dim ca = Singular.Reflection.GetAttribute(Of System.ComponentModel.CategoryAttribute)(pi)
                Dim Category = "General"
                If ca IsNot Nothing Then
                  Category = ca.Category
                End If

                Dim caFS As CustomControls.FieldSet(Of Singular.SystemSettings.ISettingsSection, Singular.SystemSettings.ISettingsSection) = mCategoryList(Category)
                If caFS Is Nothing Then
                  caFS = .Helpers.FieldSet(Category)
                  mCategoryList.Add(Category, caFS)
                End If

                'Add the control to the category section
                If Singular.Reflection.TypeInheritsFromOrIsType(pi.PropertyType, GetType(Singular.Documents.TemporaryDocument)) Then

                  caFS.Helpers.LabelFor(pi)
                  Dim c = caFS.Helpers.With(Of Singular.Documents.TemporaryDocument)(pi)
                  c.Helpers.DocumentManager().AllowOverwrite = True

                Else
                  With caFS.Helpers.Div

                    Dim EditorRow = .Helpers.EditorRowFor(pi)
                    With EditorRow

                      Dim cw = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ColumnWidth)(pi)
                      If cw IsNot Nothing AndAlso cw.DefaultWidth <> 0 Then
                        .Editor.Style.Width = cw.DefaultWidth
                      End If

                      .Style.FloatLeft()
                    End With

                    If mShowDescriptionsInline AndAlso Not String.IsNullOrEmpty(EditorRow.Editor.Tooltip) Then
                      With .Helpers.HTMLTag("span", EditorRow.Editor.Tooltip)
                        .Style.FloatLeft()
                        .Style("padding") = "7px 10px"
                        .Style.FontWeight = FontWeight.light
                        .Style("max-width") = "500px"
                      End With
                    End If

                  End With
                  caFS.Helpers.Div.Style.ClearBoth()

                End If

              End If
            Next
          End With

          With Helpers.HTMLTag("script")
            .Attributes("type") = "text/javascript"

            Using Str = Assembly.GetAssembly(Me.GetType).GetManifestResourceStream("Singular.Web.Settings.js")
              Using Reader As New IO.StreamReader(Str)
                .HTML = Reader.ReadToEnd
              End Using
            End Using

          End With

        End If

      Else

        'Need to choose an instance.
        Helpers.Span("Please select which version of " & Model.SelectInstance.Name & " you would like to edit.")

        With Helpers.Table
          .Style.Margin("15px")
          .Style.Width = "100%"
          .FirstRow.AddColumn("Name")
          .FirstRow.AddColumn("Description")
          .FirstRow.AddColumn()

          For Each InstanceDesc In Model.SelectInstance.GetInstanceNames
            With .AddStaticRow()
              .Helpers.Span(InstanceDesc.DisplayName)
              .Helpers.Span(InstanceDesc.Description)
              With .Helpers.Button("Edit", ButtonMainStyle.Warning, ButtonSize.Small, FontAwesomeIcon.edit)
                .ClickJS = "window.location = '?Name=" & Model.SelectInstance.Name & "&Inst=" & InstanceDesc.InstanceName & "'"
              End With
            End With
          Next

        End With

      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace


