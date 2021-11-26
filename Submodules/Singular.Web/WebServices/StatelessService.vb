Imports System.IO
Imports System.Reflection

Namespace WebServices

  Public Class APIArgs
    Public Property Args As Object
    Public Property Type As String
    Public Property Method As String = "GetData"
    Public Property QS As System.Collections.Specialized.NameValueCollection
    Public Property JSONArgs As System.Dynamic.DynamicObject

    Public Sub New(qs As System.Collections.Specialized.NameValueCollection)

      Me.QS = qs
      Type = qs("Type")
      If Type IsNot Nothing AndAlso Not Type.Contains(", ") Then
        Type = StatelessHandler.DefaultTypeAssemblyName & "." & Type & ", " & StatelessHandler.DefaultTypeAssemblyName
      End If

      Dim ArgsString = qs("Args")
      If ArgsString IsNot Nothing Then
        Args = System.Web.Helpers.Json.Decode(ArgsString)
      Else
        Args = New Object
      End If

    End Sub

    Public Sub New(Obj As System.Dynamic.DynamicObject)
      JSONArgs = Obj
    End Sub
  End Class

  Public Class BaseArgs
    Public Property Type As String
    Public Property Args As System.Dynamic.DynamicObject
    Public Property CallMethod As String
    Public Property PageGuid As String = Nothing
  End Class

  Public Class StatelessArguments

    Protected mType As Type
    Private mTypeName As String
    Friend StatelessInstance As Object
    Private mPageGuid As String
    Protected mMi As MethodInfo = Nothing

    Friend Sub New(DynamicObject As Object)

      If TypeOf DynamicObject Is APIArgs Then
        Dim aa As APIArgs = DynamicObject
        mTypeName = aa.Type
        mQueryString = aa.QS
        mJSonArgs = aa.JSONArgs
      Else
        mTypeName = DynamicObject.Type
        mJSonArgs = DynamicObject.Args
        mPageGuid = DynamicObject.PageGuid
      End If

      mType = Type.GetType(mTypeName)

      If mTypeName Is Nothing OrElse mTypeName = "" Then
        Throw New Exception("Type name not provided")
      End If
      If Type Is Nothing AndAlso Not mTypeName.Contains(",") AndAlso mTypeName.Contains(".") Then
        'Check if the first part of the assembly was provided, but not the assembly name.
        mTypeName = mTypeName & ", " & mTypeName.Substring(0, mTypeName.IndexOf("."))
        mType = Type.GetType(mTypeName)
      End If
      If Type Is Nothing AndAlso StatelessHandler.DefaultTypeAssemblyName <> "" Then
        'Check if no assembly info was provided.
        mType = Type.GetType(StatelessHandler.DefaultTypeAssemblyName & "." & mTypeName & "," & StatelessHandler.DefaultTypeAssemblyName)
      End If
      If Type Is Nothing Then
        'No idea what the type should be.
        Throw New Exception("Type not found: " & mTypeName)
      End If

      If mPageGuid IsNot Nothing Then
        StatelessInstance = Singular.Web.PageBase.GetModel(mPageGuid, System.Web.HttpContext.Current)
      End If

      If StatelessInstance Is Nothing OrElse Not StatelessInstance.GetType Is mType Then
        StatelessInstance = GetInstance(DynamicObject)
      End If

    End Sub

    Friend Sub New(mi As MethodInfo, DynamicObject As Object)
      If mi Is Nothing Then
        Throw New HttpException(404, "Method not found")
      Else
        mMi = mi
        If TypeOf DynamicObject Is APIArgs Then
          mQueryString = DynamicObject.QS
        Else
          mJSonArgs = DynamicObject
        End If
        mTypeName = mi.DeclaringType.Name
        mType = mi.DeclaringType
        StatelessInstance = GetInstance(Nothing)
      End If

    End Sub

    Protected Overridable Function GetInstance(DynamicObject As Object) As Object
      Return WebServices.DataInterface.GetInstance(mType)
    End Function

    Private mJSonArgs As System.Dynamic.DynamicObject
    Public ReadOnly Property JSonArgs As Object
      Get
        Return mJSonArgs
      End Get
    End Property

    Private mQueryString As System.Collections.Specialized.NameValueCollection
    Public ReadOnly Property QueryString As System.Collections.Specialized.NameValueCollection
      Get
        Return mQueryString
      End Get
    End Property

    Public ReadOnly Property Type As Type
      Get
        Return mType
      End Get
    End Property

    Public ReadOnly Property TypeName As String
      Get
        Return mTypeName
      End Get
    End Property

  End Class

  Public Class GetDataArguments
    Inherits StatelessArguments

    Public Property ImplementsIGetData As Boolean

    Public Sub New(DynamicObject As Object)
      MyBase.New(DynamicObject)

      ImplementsIGetData = Singular.Reflection.TypeImplementsInterface(Type, GetType(Singular.Web.WebServices.IGetData))

    End Sub

  End Class

  Public Class PrepareSaveArguments
    Inherits StatelessArguments

    Private mClientObject As System.Dynamic.DynamicObject

    Public Sub New(DynamicObject As Object)
      MyBase.New(DynamicObject)

      mClientObject = DynamicObject.Object

    End Sub

    Public ReadOnly Property ClientObject As System.Dynamic.DynamicObject
      Get
        Return mClientObject
      End Get
    End Property

    Public CreateInstance As Func(Of Object)

    Public SaveObject As Func(Of Object, SaveHelper)

  End Class

  Public Class HandleCommandArguments
    Inherits StatelessArguments

    Private mMethod As String
    Private mArgumentsJSONText As String

    Public ReadOnly Property Method As String
      Get
        Return mMethod
      End Get
    End Property

    Public ReadOnly Property ArgumentsJSONText As String
      Get
        Return mArgumentsJSONText
      End Get
    End Property

    Public Sub New(DynamicObject As Object, ArgumentsJSONText As String)
      MyBase.New(DynamicObject)
      mMethod = DynamicObject.CallMethod
      mArgumentsJSONText = ArgumentsJSONText
    End Sub

    Public Sub New(mi As MethodInfo, DynamicObject As Object, ArgumentsJSONText As String)
      MyBase.New(mi, DynamicObject)
      mArgumentsJSONText = ArgumentsJSONText
    End Sub

    Protected Overrides Function GetInstance(DynamicObject As Object) As Object

      If mMi Is Nothing Then
        mMi = mType.GetMethod(DynamicObject.CallMethod, BindingFlags.Static Or BindingFlags.Public)
      End If

      If mMi Is Nothing OrElse Not mMi.IsStatic Then

        'Check if the instance object was passed from the client.
        If DynamicObject IsNot Nothing AndAlso DynamicObject.Args IsNot Nothing Then
          Dim ClientInstance = DynamicObject.Args._Instance
          If ClientInstance IsNot Nothing Then

            Dim ServerObject = Activator.CreateInstance(mType, True)

            If TypeOf ServerObject Is ISingularBase AndAlso ClientInstance.Guid IsNot Nothing Then
              CType(ServerObject, ISingularBase).Guid = New Guid(CStr(ClientInstance.Guid))
            End If

            Dim Serialiser As New Singular.Web.Data.JS.StatelessJSSerialiser(ServerObject)
            Serialiser.Deserialise(ClientInstance)
            Return ServerObject
          End If
        End If

        Return MyBase.GetInstance(DynamicObject)
      Else
        'static methods dont need an instance
        Return Nothing
      End If

    End Function

    Friend ReadOnly Property MethodInfo As MethodInfo
      Get
        If mMi Is Nothing Then
          mMi = mType.GetMethod(mMethod, System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.FlattenHierarchy)
        End If
        Return mMi
      End Get
    End Property

  End Class

  Public Class DeleteArgs
    Inherits StatelessArguments

    Public Property ItemToDelete As ISingularBusinessBase
    Public Property DeleteNow As Boolean

    Public Sub New(DynamicObject As Object)
      MyBase.New(DynamicObject)

      ItemToDelete = Activator.CreateInstance(Type)
      Dim Serialiser As New Singular.Web.Data.JS.StatelessJSSerialiser(ItemToDelete)
      Serialiser.Deserialise(DynamicObject.Object)

      DeleteNow = DynamicObject.DeleteNow

    End Sub


  End Class

  Public Class StatelessService

    Public Shared Function ReadStream(Stream As Stream, CallBack As Func(Of StatelessArguments, String)) As String

      'Read the JSON arguments into a string
      Dim StringArgs As String
      Using sr As New StreamReader(Stream)
        StringArgs = sr.ReadToEnd
      End Using

      'Convert the JSON into a dynamic object.
      Dim ArgsObject As Object = System.Web.Helpers.Json.Decode(StringArgs)

      Return CallBack(New StatelessArguments(ArgsObject))

    End Function

#Region " Get Data "

    ''' <summary>
    ''' Executes a fetch on the type specified in the Input Arguments. Either using the lists criteria object, or calling GetData if the type implements IGetData
    ''' </summary>
    Public Shared Function GetData(InputStream As IO.Stream) As String

      Return ReadStream(InputStream, AddressOf GetData)

    End Function

    ''' <summary>
    ''' Executes a fetch on the type specified in the Input Arguments. Either using the lists criteria object, or calling GetData if the type implements IGetData
    ''' </summary>
    Public Shared Function GetData(Args As GetDataArguments) As String

      If Args.Type Is Nothing Then
        Return "List type " & CStr(Args.TypeName) & " not found"
      Else

        'Never allow this for stateless calls.
        Dim Obj As Object = Singular.Web.Data.ClientDataProvider.GetData(Args)

        If TypeOf Obj Is String Then
          Return Obj
        Else
          Dim Result As New Singular.Web.DataResult(True, Obj)
          Return Singular.Web.Data.JSonWriter.SerialiseObject(Result)
        End If

      End If

    End Function

#End Region

#Region " Save Data "

    Public Shared Function SaveData(InputStream As IO.Stream) As String

      Return ReadStream(InputStream, AddressOf SaveData)

    End Function

    Public Shared Function SaveData(Arguments As PrepareSaveArguments) As String


      If Singular.Reflection.TypeImplementsInterface(Arguments.Type, GetType(Singular.Web.WebServices.ISaveData)) Then
        Arguments.StatelessInstance.PrepareSave(Arguments)
      End If

      Dim ServerObject As Object
      If Arguments.CreateInstance IsNot Nothing Then
        'Function to create instance has been provided.
        ServerObject = Arguments.CreateInstance()
      Else

        'Check the user has access to save the obj.
        Dim wc = Singular.Reflection.GetAttribute(Of Singular.Web.WebSavable)(Arguments.Type)

        If wc Is Nothing Then
          Throw New Exception("Save object: Not callable from web interface.")
          Exit Function
        ElseIf Not wc.Allowed Then
          Throw New Exception("Save object: User doesnt have permission.")
          Exit Function
        End If

        'Try create the instance from the type.
        If TypeOf Arguments.ClientObject Is System.Web.Helpers.DynamicJsonArray Then
          Dim ListType = Type.GetType("Singular.GenericList`1, Singular")
          Dim GListType = ListType.MakeGenericType(Arguments.Type)
          ServerObject = Activator.CreateInstance(GListType)
        Else
          ServerObject = Activator.CreateInstance(Arguments.Type)
        End If

      End If

      'Deserialise the json into the server object.
      Dim Serialiser As New Singular.Web.Data.JS.StatelessJSSerialiser(ServerObject)
      Serialiser.Deserialise(Arguments.ClientObject)


      If Arguments.SaveObject IsNot Nothing Then
        'Function to save object has been provided.
        Return Singular.Web.Data.JSonWriter.SerialiseObject(Arguments.SaveObject(ServerObject))

      Else
        'Try save the object ourselves.
        Return Singular.Web.Data.JSonWriter.SerialiseObject(New SaveHelper().Save(ServerObject, ServerObject.IsValid, ServerObject.IsDirty))

      End If

    End Function

#End Region

#Region " Handle Command "

    Public Shared Function HandleCommand(Arguments As HandleCommandArguments) As RestResponse

      'Get the method to call on the Instance
      Dim mi As MethodInfo = Arguments.MethodInfo
      If mi IsNot Nothing Then

        'Check the security on the method
        Dim wc = Singular.Reflection.GetAttribute(Of Singular.Web.WebCallable)(mi)

        If (wc IsNot Nothing AndAlso wc.Allowed) OrElse (wc Is Nothing AndAlso Singular.Security.HasAuthenticatedUser AndAlso GetType(Singular.Web.Result).IsAssignableFrom(mi.ReturnType)) Then

          'Populate the parameters and call the method.
          Dim Params As ParameterInfo() = mi.GetParameters()
          Dim ParamValues(Params.Count - 1)
          Dim ReturnObject As Object = Nothing

          If Params.Count = 1 AndAlso (Params(0).ParameterType Is GetType(Object) OrElse Params(0).ParameterType Is GetType(System.Dynamic.DynamicObject)) Then
            'Single parameter which is of Type Object, or Dynamic Object, just pass through the raw dynamic object.
            ParamValues(0) = Arguments.JSonArgs
            ReturnObject = mi.Invoke(Arguments.StatelessInstance, ParamValues)

          Else
            For i As Integer = 0 To Params.Count - 1

              Dim ClientParamValue As Object = Nothing
              If Arguments.JSonArgs IsNot Nothing Then
                Arguments.JSonArgs.TryGetMember(New Singular.Dynamic.MemberGetter(Params(i).Name), ClientParamValue)
              Else
                ClientParamValue = Arguments.QueryString(Params(i).Name)
              End If


              If ClientParamValue IsNot Nothing Then
                Dim ti As Singular.ReflectionCached.TypeInfo = Singular.ReflectionCached.GetCachedType(Params(i).ParameterType)
                If ti.SerialisedType = ReflectionCached.SerialiseType.Simple OrElse ti.SerialisedType = ReflectionCached.SerialiseType.Enumeration Then
                  If Params(i).ParameterType Is GetType(Object) Then
                    ParamValues(i) = ClientParamValue
                  ElseIf Params(i).ParameterType Is GetType(String) AndAlso Params(i).GetCustomAttributes(GetType(Singular.Web.JSonString), False).Length > 0 Then
                    Dim jr As New Singular.Web.Data.JSonReader(Arguments.ArgumentsJSONText)
                    ParamValues(i) = jr.GetRawPropertyValue(Params(i).Name)
                  Else
                    ParamValues(i) = Singular.Reflection.ConvertValueToType(Params(i).ParameterType, ClientParamValue)
                  End If

                Else
                  Dim ServerObject = Activator.CreateInstance(Params(i).ParameterType, True)

                  If TypeOf ServerObject Is ISingularBase AndAlso ClientParamValue IsNot Nothing AndAlso ClientParamValue.Guid IsNot Nothing Then
                    CType(ServerObject, ISingularBase).Guid = New Guid(CStr(ClientParamValue.Guid))
                  End If

                  Dim Serialiser As New Singular.Web.Data.JS.StatelessJSSerialiser(ServerObject)
                  Serialiser.ContextList.Add(mi.DeclaringType.Name, True)
                  Serialiser.Deserialise(ClientParamValue)
                  ParamValues(i) = ServerObject
                End If

              Else
                If Params(i).IsOptional Then
                  ParamValues(i) = Params(i).DefaultValue
                Else
                  ParamValues(i) = Nothing
                End If

              End If

            Next

            ReturnObject = mi.Invoke(Arguments.StatelessInstance, ParamValues)
          End If

          If TypeOf ReturnObject Is RestResponse Then
            Return ReturnObject
          Else
            Return SuccessResponse(ReturnObject, wc)
          End If

          'Return Singular.Web.Data.JSonWriter.SerialiseObject(ReturnObject, , Arguments.RenderObjectGuids)

        Else
          Return AuthorisationErrorResponse("Method call access denied")
        End If
      Else
        Throw New Exception("Cannot find method '" & Arguments.Method & " on type '" & Arguments.Type.Name & "'")
      End If

    End Function

#End Region

#Region " Delete "

    Public Shared Function DeleteData(Arguments As DeleteArgs) As String

      Dim cdi = Arguments.ItemToDelete.CanDelete

      If cdi.CanDeleteResult = CanDeleteArgs.CanDeleteResultType.CanDelete Then
        If Arguments.DeleteNow Then
          CType(Arguments.ItemToDelete, Csla.Core.BusinessBase).Delete()
          CType(Arguments.ItemToDelete, Object).Save()
        End If
      Else

        cdi.Detail = "Cannot delete:<p/>" & cdi.Detail.ToHTML(NewLineHandlingType.PointList)

      End If

      Return Singular.Web.Data.JSonWriter.SerialiseObject(cdi)

    End Function

#End Region

#Region " Authentication "

    Private Class LoginResult
      Public Property Identity As Singular.Security.IIdentity
      Public Property AuthToken As String
      Public Property ExpiryDate As Date
      Public Property LoginError As String
    End Class

    Public Shared Function Login(ClientArguments As Object) As String

      Dim lr As New LoginResult

      Try
        If Not ClientArguments.Refreshing Then
          Singular.Web.Security.WebLoginMethod.Invoke(ClientArguments.UserName, ClientArguments.Password, Security.AuthType.HTTPHeader)
        End If

        lr.Identity = Singular.Security.CurrentIdentity
        If lr.Identity Is Nothing Then
          lr.LoginError = "Incorrect user name and/or password."
        Else
          Dim ExpiryDate As Date = Now.AddDays(1)
          If (ClientArguments.Remember) Then
            ExpiryDate = Now.AddDays(30)
          End If

          lr.AuthToken = Singular.Web.Security.GetMessageFromToken(New Security.TokenInfo() With {.ExpiryDate = ExpiryDate, .UserName = lr.Identity.Name})
          lr.ExpiryDate = ExpiryDate
        End If

      Catch ex As Exception
        lr.LoginError = Singular.Localisation.LocalText(Singular.Debug.RecurseExceptionMessage(ex))
      End Try

      Return Singular.Web.Data.JSonWriter.SerialiseObject(lr)

    End Function

#End Region

  End Class


End Namespace

Public Class SaveHelper
  Inherits Singular.SaveHelper

  Public Property UserData As Object
  Public Property SaveMessageType As MessageType
  Public Property SaveMessage As String

  Public Sub New()

  End Sub

  Public Sub New(sh As Singular.SaveHelper, Obj As ISavable)
    MyBase.Success = sh.Success
    MyBase.SavedObject = sh.SavedObject
    MyBase.Result = sh.Result
    MyBase.ErrorText = sh.ErrorText
    MyBase.Error = sh.Error

    SetMessage(Obj)
  End Sub

  <System.ComponentModel.Browsable(False)> Public Property ShowDetailErrorMessages As Boolean

  Protected Overrides Sub DoSave(Obj As ISavable, IsValid As Boolean, IsDirty As Boolean)
    MyBase.DoSave(Obj, IsValid, IsDirty)

    SetMessage(Obj)
  End Sub

  Private Sub SetMessage(Obj As ISavable)

    If Result = ResultType.ObjectNotValid AndAlso Obj IsNot Nothing Then
      ErrorText = Obj.ErrorsAsHTMLString
    End If

    If Result = SaveHelper.ResultType.SavedToDatabase Then
      SaveMessageType = MessageType.Success
      SaveMessage = "Saved successfully"
    ElseIf Result = SaveHelper.ResultType.NoChangesMade Then
      SaveMessageType = MessageType.Information
      SaveMessage = "No changes were made."
    ElseIf Result = SaveHelper.ResultType.ObjectNotValid Then
      SaveMessageType = MessageType.Validation
      SaveMessage = "Can't save due to validation errors:<br/>" & If(Obj Is Nothing, ErrorText, Obj.ErrorsAsHTMLString)
    ElseIf Result = SaveHelper.ResultType.CustomError Then
      SaveMessageType = MessageType.Error
      SaveMessage = ErrorText
    ElseIf Result = SaveHelper.ResultType.Error Then
      If ShowDetailErrorMessages OrElse Debugger.IsAttached Then
        SaveMessageType = MessageType.Error
        SaveMessage = "Error while saving: " & ErrorText
      Else
        SaveMessageType = MessageType.Error
        SaveMessage = "Error while saving" & If(WebError.SupportsWebError, ", Error ID: " & WebError.LogError([Error]), "")
      End If
    End If

  End Sub



End Class

