
Imports Singular.Web
Imports Singular.Web.Reporting

Namespace METTWeb.Reports
	Public Class METTReportStateControl_old
		Inherits Controls.HelperControls.HelperBase(Of ReportVM)

		Private mReportCriteriaControl As METTReportCriteriaControl

		Public ReadOnly Property ReportCriteriaControl As METTReportCriteriaControl
			Get
				Return mReportCriteriaControl
			End Get
		End Property

		Protected Overrides Sub Setup()
			MyBase.Setup()

			If Model.Report Is Nothing Then
				Helpers.Control(New METTWeb.METTReportMenuControl)
			Else
				mReportCriteriaControl = New METTWeb.METTReportCriteriaControl
				Helpers.Control(mReportCriteriaControl)
			End If

		End Sub

		Protected Overrides Sub Render()
			MyBase.Render()

			RenderChildren()
		End Sub
	End Class

End Namespace
