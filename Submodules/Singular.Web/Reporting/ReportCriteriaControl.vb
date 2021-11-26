Imports System.Reflection
Imports Singular.Reporting.Dynamic

Namespace Reporting

  Public Class ReportCriteriaControl
    Inherits Controls.HelperControls.HelperBase(Of ReportVM)

    Private mToolbar As Singular.Web.CustomControls.Toolbar(Of Singular.Web.Reporting.ReportVM)

    Public ReadOnly Property Toolbar As Singular.Web.CustomControls.Toolbar(Of Singular.Web.Reporting.ReportVM)
      Get
        Return mToolbar
      End Get
    End Property

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      mToolbar = Helpers.Toolbar()
      With mToolbar
        .Helpers.HTML.Heading2(Model.Report.ReportName)

      End With

      Helpers.MessageHolder()

			Dim Report As Singular.Reporting.IReport = Model.Report

			If Report.CustomCriteriaControlType Is Nothing Then

				Dim ParameterList As Singular.Reporting.Dynamic.ROParameterList = Report.ReportCriteriaGeneric.ParameterList()
				If ParameterList IsNot Nothing Then
					SetupDynamicReportCriteria(ParameterList)
				Else
					SetupStronglyTypedReportCriteria()
				End If

			Else

				Helpers.Control(Activator.CreateInstance(Report.CustomCriteriaControlType))

      End If

      Report.CustomButtons.Where(Function(c) Not c.AfterNormalButtons).ToList.ForEach(AddressOf AddCustomButton)

      If Report.CrystalReportType IsNot Nothing And Report.HideCrystalReport = False Then
        With Helpers.Button("PDF", "View as PDF")
          '.AddBinding(KnockoutBindingString.click, "ViewPDF()")
          .PostBackType = PostBackType.Full
          .Image.Src = Scripts.LibResourceHandler.GetLibImageURL("IconPDF.png")
          .Validate = True
        End With
        If Report.ShowWordExport Then
          With Helpers.Button("Word", "View as Word")
            .PostBackType = PostBackType.Full
            .Image.Src = Scripts.LibResourceHandler.GetLibImageURL("IconWord.png")
            .Validate = True
          End With
        End If
      End If
			If Report.AllowDataExport Then
				With Helpers.Button("Data", "Export Data")
					.PostBackType = PostBackType.Full
          .Image.Src = Scripts.LibResourceHandler.GetLibImageURL("IconExcelData.png")
          .Validate = True
				End With
			End If

			Dim gi = Report.GridInfo
			If gi IsNot Nothing Then
				With Helpers.Button("GridData", "View Data")
          .Image.Src = Scripts.LibResourceHandler.GetLibImageURL("IconExcelData.png")
          .AddBinding(KnockoutBindingString.click, "Singular.GridReport.ShowOptions(e)")
				End With
			End If

      Report.CustomButtons.Where(Function(c) c.AfterNormalButtons).ToList.ForEach(AddressOf AddCustomButton)

			With Helpers.Div
				.Style.ClearBoth()
				.Style("height") = "30px"
			End With

    End Sub

    Private Sub AddCustomButton(eb As Singular.Reporting.CustomButton)
      With Helpers.Button("Custom", eb.ButtonText)
        If eb.ImageURL <> "" Then
          If eb.ImageURL.StartsWith("fa fa") Then
            'font-awesome icon
            .Image.Glyph = FontAwesomeIcon.blank
            .Image.AddClass(eb.ImageURL)
          Else
            .Image.Src = eb.ImageURL
          End If

        End If
        .PostBackType = PostBackType.Full
        .AddBinding(KnockoutBindingString.ButtonArgument, eb.ButtonID.ToString.AddSingleQuotes)
        .Validate = True
      End With
    End Sub

    Private Sub SetupStronglyTypedReportCriteria()

      With Helpers.With(Of Singular.Reporting.ReportCriteria)(Function(c) c.Report.ReportCriteriaGeneric)

        Dim StartDatePI = Singular.Reflection.GetProperty(Model.Report.ReportCriteriaGeneric.GetType, "StartDate")
        Dim EndDatePI = Singular.Reflection.GetProperty(Model.Report.ReportCriteriaGeneric.GetType, "EndDate")

        If StartDatePI IsNot Nothing OrElse EndDatePI IsNot Nothing Then
          'Add Date container
          With .Helpers.FieldSet("Date Selection")

            'Start Date
            If StartDatePI IsNot Nothing Then
              With .Helpers.Div
                .Style.FloatLeft()
                .Style("padding") = "5px"
                With .Helpers.HTMLTag("span")
                  .Style.Padding(, , "5px")
                  .Style.Display = Display.block
                  .AddBinding(KnockoutBindingString.text, "'Start Date: '+ dateFormat(StartDate(), 'dd MMM yyyy')")
                End With

                .Helpers.Control(Singular.Web.CustomControls.EditorBase(Of ReportVM).GetEditor(StartDatePI))
              End With
            End If

            'End Date
            If EndDatePI IsNot Nothing Then
              With .Helpers.Div()
                .Style.FloatLeft()
                .Style("padding") = "5px"
                With .Helpers.HTMLTag("span")
                  .Style.Padding(, , "5px")
                  .Style.Display = Display.block
                  .AddBinding(KnockoutBindingString.text, "'End Date: '+ dateFormat(EndDate(), 'dd MMM yyyy')")
                End With
                .Helpers.Control(Singular.Web.CustomControls.EditorBase(Of ReportVM).GetEditor(EndDatePI))
              End With
            End If

          End With

        End If

        Dim mCriteriaGroup As Singular.Web.CustomControls.FieldSet(Of Singular.Reporting.ReportCriteria, Singular.Reporting.ReportCriteria) = Nothing

        Model.Report.ReportCriteriaGeneric.GetType.ForEachBrowsableProperty(Nothing,
           Sub(pi)

             If pi IsNot StartDatePI AndAlso pi IsNot EndDatePI Then

               If mCriteriaGroup Is Nothing Then
                 mCriteriaGroup = .Helpers.FieldSet("Criteria")
               End If

               With mCriteriaGroup.Helpers.EditorRowFor(pi)
                 .Editor.Style.Width = "300px"
               End With

             End If

           End Sub, , True, True)

      End With

    End Sub

    Private Sub SetupDynamicReportCriteria(ParameterList As Singular.Reporting.Dynamic.ROParameterList)

      With Helpers.With(Of Singular.Reporting.ReportCriteria)(Function(c) c.Report.ReportCriteriaGeneric)

        Dim StartDateP As ROParameter = ParameterList.Find("StartDate")
        Dim EndDateP As ROParameter = ParameterList.Find("EndDate")


        If StartDateP IsNot Nothing OrElse EndDateP IsNot Nothing Then
          'Add Date container
          With .Helpers.FieldSet("Date Selection")

            'Start Date
            If StartDateP IsNot Nothing Then
              With .Helpers.Div
                .Style.FloatLeft()
                .Style("padding") = "5px"
                With .Helpers.HTMLTag("span")
                  .Style.Padding(, , "5px")
                  .Style.Display = Display.block
                  .AddBinding(KnockoutBindingString.text, "'Start Date: '+ dateFormat(StartDate(), 'dd MMM yyyy')")
                End With

								Dim de As New Singular.Web.CustomControls.DateEditor(Of Object)("StartDate", "",
														New Singular.DataAnnotations.DateField() With {.AlwaysShow = True,
																																					 .AutoChange = Singular.DataAnnotations.AutoChangeType.StartOfMonth,
																																					 .MaxDateProperty = If(EndDateP IsNot Nothing, "EndDate", "")})
                .Helpers.Control(de)
              End With
            End If

            'End Date
            If EndDateP IsNot Nothing Then
              With .Helpers.Div()
                .Style.FloatLeft()
                .Style("padding") = "5px"
                With .Helpers.HTMLTag("span")
                  .Style.Padding(, , "5px")
                  .Style.Display = Display.block
                  .AddBinding(KnockoutBindingString.text, "'End Date: '+ dateFormat(EndDate(), 'dd MMM yyyy')")
                End With

								Dim de As New Singular.Web.CustomControls.DateEditor(Of Object)("EndDate", "",
														New Singular.DataAnnotations.DateField() With {.AlwaysShow = True,
																																					 .AutoChange = Singular.DataAnnotations.AutoChangeType.EndOfMonth,
																																					 .MinDateProperty = If(StartDateP IsNot Nothing, "StartDate", "")})
                .Helpers.Control(de)
              End With
            End If

          End With

        End If

        Dim mCriteriaGroup As Singular.Web.CustomControls.FieldSet(Of Singular.Reporting.ReportCriteria, Singular.Reporting.ReportCriteria) = Nothing

        For Each Param As ROParameter In ParameterList

          If Param IsNot StartDateP AndAlso Param IsNot EndDateP AndAlso Param.Visible Then

            If mCriteriaGroup Is Nothing Then
              mCriteriaGroup = .Helpers.FieldSet("Criteria")
            End If

            Dim Editor As CustomControls.EditorBase(Of Object) = Nothing

            Dim ddd As Singular.Reporting.Dynamic.DynamicDropDown = Nothing
            If Param.DropDownSource IsNot Nothing Then
              ddd = Singular.Reporting.Dynamic.Settings.DropDowns.IncludeDatabaseDropDowns.GetItem(Param.DropDownSource)
            End If

            If ddd IsNot Nothing Then
              'Drop down

              Dim di = ddd.GetDynamicInfo
              If di.Data IsNot Nothing Then
                CType(Model, ReportVM).ClientDataProvider.AddDataSource(di.DropDownInfo.ClientName, di.Data, False)
              End If
              Editor = New CustomControls.DropDownEditor(Of Object)(Param.ParameterName, "", di.DropDownInfo)
            Else

              'Other
              Select Case Param.ParamDataType
                Case Reflection.SMemberInfo.MainType.String
                  Editor = New CustomControls.TextEditor(Of Object)(Param.ParameterName, "", Nothing)
                Case Reflection.SMemberInfo.MainType.Number
                  Editor = New CustomControls.NumericEditor(Of Object)(Param.ParameterName, "", Nothing)
                Case Reflection.SMemberInfo.MainType.Date
                  Editor = New CustomControls.DateEditor(Of Object)(Param.ParameterName, "", Nothing)
                Case Reflection.SMemberInfo.MainType.Boolean
                  Editor = New CustomControls.CheckBoxEditor(Of Object)(Param.ParameterName, "")
              End Select
            End If



            mCriteriaGroup.Helpers.Control(New CustomControls.EditorRow(Of Object)(Editor, New CustomControls.FieldLabel(Of Object)(Param.ParameterName, Param.DisplayName)))
            Editor.Style.Width = 300

          End If

        Next

      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub


	End Class

	Public MustInherit Class CriteriaControlBase
		Inherits Controls.HelperControls.HelperBase(Of ReportVM)
		Protected Friend Overrides Sub Render()
			MyBase.Render()
			RenderChildren()
		End Sub
	End Class

End Namespace


