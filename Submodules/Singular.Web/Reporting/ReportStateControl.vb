Namespace Reporting

  Public Class ReportStateControl
    Inherits Controls.HelperControls.HelperBase(Of ReportVM)

    Private mReportCriteriaControl As ReportCriteriaControl

    Public ReadOnly Property ReportCriteriaControl As ReportCriteriaControl
      Get
        Return mReportCriteriaControl
      End Get
    End Property

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If Model.Report Is Nothing Then
        Helpers.Control(New ReportMenuControl)
      Else
        mReportCriteriaControl = New ReportCriteriaControl
        Helpers.Control(mReportCriteriaControl)
      End If

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace

