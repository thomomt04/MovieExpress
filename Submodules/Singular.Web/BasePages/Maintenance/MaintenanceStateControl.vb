Namespace MaintenanceHelpers


  Public Class MaintenanceStateControl
		Inherits Controls.HelperControls.HelperBase(Of MaintenanceVM)

		Public Property OnRenderButton As Action(Of Controls.HelperControls.HelperAccessors(Of MaintenanceVM), MaintenancePage) = Nothing

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      If Model.CurrentMaintenancePage Is Nothing Then
        Helpers.Control(New MaintenanceMenu)
      Else
				Helpers.Control(New MaintenanceEditor(Model, OnRenderButton))
      End If

    End Sub

		Protected Friend Sub SetupNoMenu()
			MyBase.Setup()

		End Sub

		Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub


  End Class


  

End Namespace

