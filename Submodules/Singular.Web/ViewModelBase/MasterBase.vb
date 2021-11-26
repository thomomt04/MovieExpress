Public MustInherit Class MasterBase
  Inherits System.Web.UI.MasterPage

  Public Sub New()
    ID = "A" 'Otherwise its ctl$001
  End Sub

  Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    If Not Singular.Debug.InDebugMode AndAlso ScriptManager IsNot Nothing Then
      ScriptManager.EnableCdn = Singular.Web.Scripts.Settings.UseCDN
      ScriptManager.ScriptMode = UI.ScriptMode.Release
    End If

    If ApplicationSettings.Settings.RenameMasterContentControls Then
      'Rename the 'HeadContent' and 'MainContent' to 'A', 'B', 'C' etc
      For i As Integer = 0 To ContentTemplates.Count - 1
        CType(FindControl(ContentTemplates.Keys(i)), System.Web.UI.WebControls.ContentPlaceHolder).ID = ChrW(i + 65)
      Next
    End If

  End Sub

  Public ReadOnly Property ScriptManager As System.Web.UI.ScriptManager
    Get
      Return FindControl("SCMMain")
    End Get
  End Property


End Class
