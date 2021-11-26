Imports System.Web
Imports System.Web.Services
Imports System.Text
Imports System.Reflection

Namespace WebServices

  ''' <summary>
  ''' HTTP handler used to handle requests which require write access to the session.
  ''' Only one request can run at a time per session.
  ''' This class also contains all the helper functions relating to readonly session access.
  ''' </summary>
  Public Class DataHandler
    Implements System.Web.IHttpHandler
    Implements System.Web.SessionState.IRequiresSessionState

    'Private Shared GlobalLock As New Object

    Public ReadOnly Property IsReusable As Boolean Implements System.Web.IHttpHandler.IsReusable
      Get
        Return False
      End Get
    End Property

    Public Sub ProcessRequest(context As System.Web.HttpContext) Implements IHttpHandler.ProcessRequest

      Dim ArgumentText As String = ""
      Try

        HttpHandlerHelper.PrepareRequest(context)

        Dim ArgumentsObject As Object = Nothing
        If HttpHandlerHelper.GetArguments(context, ArgumentsObject, ArgumentText) Then

          ProcessSessionRequest(context, ArgumentsObject)

        End If

      Catch ex As Exception
        HttpHandlerHelper.HandleException(context, ex, "DataHandler", ArgumentText)
      End Try

    End Sub

    Friend Shared Sub ProcessSessionRequest(Context As System.Web.HttpContext, ArgumentsObject As Object)

      Dim Arguments As Object = ArgumentsObject.Args

      Dim Model As IViewModel
      If Arguments.PageGuid IsNot Nothing Then
        Model = Singular.Web.PageBase.GetModel(Arguments.PageGuid, Context)
      Else
        Model = DataInterface.GetInstance(Type.GetType(CStr(Arguments.VMType)))
      End If

      Context.Response.ContentType = "text/plain"

      If Model Is Nothing Then
        'Session has expired.
        Context.Response.StatusCode = 500
        Context.Response.Write("Session has Expired, Please reload the page")

      Else

        Dim Method As String = ArgumentsObject.Method
        If Method.StartsWith("VM_") Then
          Method = Method.Substring(3)
        End If

        Select Case Method


          'These methods require write access to the session and are accessed through this handler (which implements IRequiresSessionState).
          'Only 1 request can be processed at a time per session, as the session is locked at the start of the request.
          Case "HandleCommandAsync"
            Context.Response.Write(HandleCommandAsync(Context, Model, Arguments))
          Case "ObjectUpdate"
            Context.Response.Write(ObjectUpdate(Model, Arguments))


            'These methods only require readonly session access, and dont need to block the session, allowing for processing of multiple requests at once.
            'These methods are called from the StatelessHandler (which implements IReadOnlySessionState)
          Case "GetData"
            Context.Response.Write(GetData(Model, Arguments))
          Case "CheckRulesAsync"
            Context.Response.Write(CheckRulesAsync(Model, Arguments))
          Case "CanDelete"
            Context.Response.Write(CanDelete(Model, Arguments))
          Case "GetCommonData"
            Context.Response.Write(GetCommonData(Model, Arguments))

        End Select

      End If

    End Sub

    Public Shared Function GetData(Model As IViewModel, ParamObj As Object) As String

      'Pass the request to the page
      Dim gda As New GetDataArgs(ParamObj)
      Dim Obj As Object = Model.GetData(gda)
      If Obj IsNot Nothing Then
        If TypeOf Obj Is String Then
          'If the type of object returned is a string, then assume its a JSon string, and don't re-encode it. 
          Return Obj
        Else

          Dim Serialiser As New Data.JS.StatelessJSSerialiser(Obj)
          Serialiser.ContextList.AddContext(gda.Context)
          'Serialiser.RenderInfoArray = True
          Dim json As String = Serialiser.GetJSon()
          Return json
        End If
      Else
        Return ""
      End If

    End Function

    Public Shared Function CheckRulesAsync(Model As IViewModel, ParamObj As Object) As String

      'Pass the request to the page
      Dim ara As New AsyncRuleArgs() With {.ClientArgs = ParamObj, .RuleName = ParamObj.RuleName, .ObjectType = Type.GetType(ParamObj.ObjectType)}

      Return Singular.Web.Data.JSonWriter.SerialiseObject(Model.CheckRulesAsync(ara), "CheckRulesAsync")

    End Function

    Public Shared Function HandleCommandAsync(context As System.Web.HttpContext, Model As IViewModel, ParamObj As Object) As String

      If Singular.Debug.InDebugMode Then
        'Simulate some delay in debug mode, so that the progress bar appears.
        System.Threading.Thread.Sleep(200)
      End If

      Dim CArgs As New CommandArgs(ParamObj, True, CType(ParamObj, System.Dynamic.DynamicObject).GetDynamicMemberNames.Contains("Model"), ParamObj.FetchModel, Model)

      'Dim MsgList As Singular.Web.CustomControls.MessageList
      Model.ClearPageCycleVariables()

      'Check if the model is present.
      If CArgs.ModelHasBeenUpdated Then
        Model.JSSerialiser.Deserialise(CType(ParamObj.Model, System.Dynamic.DynamicObject))
        Model.JSSerialiser.IsInitial = False
        Model.FileManager.PopulateNewObjects()
      End If


      'Model.MessageList.Clear()
      Model.HandleCommandInternal(ParamObj.CommandName, CArgs)


      'Check if a wait instance has been created.
      If CArgs.LongActionInstance IsNot Nothing Then
        'Return initial progress info
        context.Response.ContentType = "application/json"
        context.Response.ContentEncoding = System.Text.Encoding.UTF8

        CArgs.LongActionInstance.WriteHTTPResponse(False, context)
        Return ""

      Else

        'Normal syncronous return.
        Return Model.GetAjaxResponse(CArgs)

      End If

    End Function

    Public Shared Function CanDelete(Model As IViewModel, ParamObj As Object) As String

      Dim cdi As New Singular.CanDeleteArgs(CanDeleteArgs.CanDeleteResultType.CanDelete, "")

      'Should be the guid of the object.
      Dim objtoDelete = Model.ServerObjectTracker.GetObjectInfo(New Guid(CStr(ParamObj.Data)))

      If objtoDelete IsNot Nothing Then

        cdi = CType(objtoDelete.ListItem, Singular.ISingularBusinessBase).CanDelete

        If cdi.CanDeleteResult = CanDeleteArgs.CanDeleteResultType.CantDelete Then
          'Add description to the start, and replace tabs, cause it looks silly on a browser message box.
          cdi.Detail = "Cannot delete " & objtoDelete.ListItem.ToString & ":<p/>" & cdi.Detail.ToHTML(NewLineHandlingType.PointList)
        End If

        If cdi.CanDeleteResult = CanDeleteArgs.CanDeleteResultType.CanDelete AndAlso Model.DeleteMode = SaveMode.Immediate Then
          objtoDelete.RemoveSelf()

          Dim sia As New SaveImmediateArgs(ParamObj, True, True, False, objtoDelete.ListItem, True, Model)
          Model.HandleCommandInternal("SaveImmediate", sia)

          'Update ItemBinder
          CType(Model.JSSerialiser, Singular.Web.Data.JS.SessionVMJSSerialiser).UpdateItemTracker()

        End If
      End If

      Return Singular.Web.Data.JSonWriter.SerialiseObject(cdi)

    End Function

    ''' <summary>
    ''' Used by the JS library to fetch drop down data.
    ''' </summary>
    Public Shared Function GetCommonData(Model As IViewModel, ParamObj As Object) As String

      'Returns the full JSON list from the client data provider
      If ParamObj.TypeOrName Is Nothing OrElse ParamObj.TypeOrName = "" Then
        'Fetch from the Client Data provider.
        Return Data.ClientDataProvider.GetCacheableJSon(Model.ClientDataProvider.DataSourceList.GetItem(ParamObj.SourceName).Data)
      Else

        Dim ListType As Type = Nothing
        Dim PropertyName As String = ""
        Dim Source As Singular.DataAnnotations.DropDownWeb.SourceType
        If CStr(ParamObj.TypeOrName).Contains(",") Then
          ListType = Type.GetType(ParamObj.TypeOrName)
        Else
          PropertyName = ParamObj.TypeOrName
        End If
        Select Case ParamObj.Source
          Case "C"
            Source = Singular.DataAnnotations.DropDownWeb.SourceType.CommonData
          Case "S"
            Source = Singular.DataAnnotations.DropDownWeb.SourceType.SessionData
          Case "F"
            Source = Singular.DataAnnotations.DropDownWeb.SourceType.Fetch
          Case "N"
            Source = Singular.DataAnnotations.DropDownWeb.SourceType.None
          Case "V"
            Source = Singular.DataAnnotations.DropDownWeb.SourceType.ViewModel
          Case "T"
            Source = Singular.DataAnnotations.DropDownWeb.SourceType.TempData
          Case "T"
        End Select

        Return Data.ClientDataProvider.GetCacheableJSon(ListType, PropertyName, Source, Model).JSon

      End If

    End Function

    Public Shared Function ObjectUpdate(Model As IViewModel, ParamObj As Object) As String

      Dim ObjectToUpdate = Model.UpdateAndGetObject(ParamObj)

      Dim Serialiser As Singular.Web.Data.JS.SessionVMJSSerialiser = Model.JSSerialiser

      'Call the Command Handler
      Dim sia As New SaveImmediateArgs(ParamObj, True, True, False, ObjectToUpdate.UpdatedObject, False, Model)
      Model.HandleCommandInternal("SaveImmediate", sia)


      'Update ItemBinder
      Serialiser.UpdateItemTracker()

      Dim sir As New SaveImmediateResult
      sir.FromArgs(sia)

      If ParamObj.ReturnProperty IsNot Nothing Then
        'Return the Objects ID back to the Client.
        Dim mReturnProperty = ObjectToUpdate.Container.MemberList.Find(Function(c) c.PropertyHelper.Name = ParamObj.ReturnProperty.ToString)
        Dim SavedObject = Model.ServerObjectTracker.GetObject(ObjectToUpdate.ObjectGuid)
        sir.SaveID = mReturnProperty.PropertyHelper.GetValue(SavedObject)
      End If

      Return Singular.Web.Data.JSonWriter.SerialiseObject(sir)

    End Function

    Private Class SaveImmediateResult
      Public Property Success As Boolean
      Public Property ErrorText As String
      Public Property SaveID As Object
      Public Property SavedObject As Object

      Public Sub FromArgs(sia As SaveImmediateArgs)
        Success = sia.Success
        ErrorText = sia.SaveError
        If sia.SaveHelper IsNot Nothing Then
          If sia.SaveHelper.Result = SaveHelper.ResultType.ObjectNotValid Then
            ErrorText = "Object not valid " & CType(sia.UpdatedObject, ISavable).ErrorsAsHTMLString
          End If
          SavedObject = sia.SavedObject
        End If
      End Sub
    End Class

  End Class

End Namespace

