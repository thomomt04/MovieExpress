Imports System.Web.UI
Imports System.Text

Namespace CustomControls

  Public Class PageModelRenderer
    Inherits Singular.Web.Controls.CustomWebControl

    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)
      MyBase.Render(writer)

      If TypeOf Page Is Data.IViewModelPage Then

        Dim Model = CType(Page, Data.IViewModelPage).ModelNonGeneric

        If Not Model.IsSendingFile Then

          'Context menu
          writer.Write(New Singular.Web.CustomControls.AjaxControlLoader(GetType(Singular.Web.CustomControls.ContextMenu), ContainerType.Script, "SMenuTemplate").GetHTMLString)

          CType(Page, PageBase).Timer.AddTime("Post Render")

          'Render the HTML that needs to go at the end of the page. e.g. The find Dialog.
          writer.WriteLine()
          writer.WriteLine("<!--End Page HTML-->")
          writer.Write(CType(Page, IPageBase).LateResources.EndPageHTML.ToString)

          'Register the hidden fields.
          If CType(Page, Singular.Web.PageBase).MaintainsState Then
            ScriptManager.RegisterHiddenField(Page, "hPageGuid", CType(Page, PageBase).PageGuid.ToString)
            ScriptManager.RegisterHiddenField(Page, "hModelData", "")
          End If
          Singular.Web.Security.AddCSRFToken(Page)

          'Register the late Script includes.
          CType(Page, IPageBase).LateResources.WriteLateResources(writer)

          'Check if it needs json
          If Not Misc.BroswerInfo.SupportsJSon Then
            writer.Write("<script type=""text/javascript"" src=""" & Utils.URL_ToAbsolute("~/Singular/Javascript/json2.js") & """></script>")
          End If

          Dim jsw As New Singular.Web.Utilities.JavaScriptWriter

          'Singular Libary setup
          jsw.Write("//Singular Library Setup")
          If Singular.Debug.InDebugMode Then
            jsw.Write("if (!window.Singular) { alert(""Cannot load Singular Javascript Library. Please check the .js files have loaded in developer tools.""); }")
          End If
          Dim RootPath As String = Utils.URL_ToAbsolute("~")
          jsw.Write("Singular.RootPath = '" & If(RootPath = "/", "", RootPath) & "';")
          jsw.Write("Singular.CurrentPath = '" & Utils.URL_ToAbsolute(Page.AppRelativeVirtualPath) & "';")
          If Singular.Debug.InDebugMode Then
            jsw.Write("Singular.DebugMode = true;")
          End If

          Model.WriteInitialiseVariables(jsw)
          WriteOtherData(jsw)

          CType(Page, PageBase).Timer.AddTime("Pre JSon")

          'Write the View Model Data
          jsw.Write("//ViewModel Data")
          jsw.Write("ClientData.ViewModel = " & Model.JSSerialiser.GetJSon(Data.OutputType.Javascript) & ";")
          jsw.Write("")

          CType(Page, PageBase).Timer.AddTime("Post JSon")

          'Write the Client Data (for drop downs etc.)
          WriteClientData(jsw)

          CType(Page, PageBase).Timer.AddTime("Post Client Data")

          Model.ClientLocalisation.WriteKeyListJS(jsw)

          jsw.Write("//ViewModel Setup")
          jsw.Write("var self = this;")

          'Write the View Model Structure
          jsw.Write(Model.JSSerialiser.GetModelAsJavascript)

          'Write other schema
          Model.SchemaList.WriteSchemas(jsw)

          CType(Page, PageBase).Timer.AddTime("Post JS Model")

          'Register the inline Javascript.
          ScriptManager.RegisterStartupScript(Me, Me.GetType, UniqueID, jsw.ToString, True)

          CType(Page, PageBase).Timer.AddTime("Post Render")
          writer.Write("<!-- " & vbCrLf & CType(Page, PageBase).Timer.ToString & " -->")

        End If

      End If

    End Sub

    Private Sub WriteClientData(Writer As Singular.Web.Utilities.JavaScriptWriter)

      Dim ViewModel As IViewModel = CType(Page, Data.IViewModelPage).ModelNonGeneric
      If ViewModel.ClientDataProvider.DataSourceList.Count > 0 Then

        Writer.Write("//Lookup Data")

        For Each ds As Data.ClientDataProvider.ClientDataSource In CType(Page, Data.IViewModelPage).ModelNonGeneric.ClientDataProvider.DataSourceList

          ds.WriteToPage(Writer, ViewModel)
          'Data.ClientDataProvider.WriteClientDataJS(Writer, ds.SourceName, ds.ListType, ds.PropertyName, ds.Source, ds.Data, ViewModel, ds.DontCache)

        Next

        Writer.Write("")

      End If

    End Sub

    Protected Overridable Sub WriteOtherData(Writer As Singular.Web.Utilities.JavaScriptWriter)

    End Sub

  End Class

End Namespace


