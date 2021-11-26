Imports System.Reflection
Imports Singular.Web.Data.JS.ObjectInfo

Namespace Web.Data.JS

  Public MustInherit Class JSSerialiser

    ''' <summary>
    ''' When true, encrypts properties that have a Key attribute, so that the key cannot be changed on the client.
    ''' </summary>
    Public Shared Property ProtectKeyProperties As Boolean = True

    Public Shared Property KeyEncodingType As EncodingType = EncodingType.Base64

    Public Enum EncodingType
      Base64
      Hex
    End Enum

    Public Class StoreObjectInfo
      Public Guid As Guid
      Public PreviouslyAdded As Boolean = False
    End Class

    Private mTypeDefinitionList As New TypeDefinitionList
    Protected mRootObjectInfo As Member
    Private mRootData As Object
    Private mRootType As Type

    Public ReadOnly Property RootObjectInfo As Member
      Get
        Return mRootObjectInfo
      End Get
    End Property

    Public Enum SetMode
      SetProperty = 1
      SetBackingField = 2
    End Enum

#Region " Properties "

    Public Property SortProperties As Boolean = False

    Public Property RootPropertyName As String = ""

    Public Property ContextList As New UIContextList

    Private _DefaultPropertySetMode As SetMode = SetMode.SetProperty

    Public Property DefaultPropertySetMode As SetMode
      Get
        Return _DefaultPropertySetMode
      End Get
      Set(value As SetMode)
        _DefaultPropertySetMode = value
        CurrentPropertySetMode = value
      End Set
    End Property

    Public Property CurrentPropertySetMode As SetMode = SetMode.SetProperty

    Public Property RenderDeletedList As Boolean = False

    Public Property IncludeIsDirty As Boolean = True

    ''' <summary>
    ''' Indicates that this is an inital request for data, e.g. a full page load, not an ajax request.
    ''' This is for non stateless ViewModels.
    ''' </summary>
    Public Property IsInitial As Boolean = True

    Private mRenderGuid As Boolean = True
    Public Property RenderGuid As Boolean
      Get
        Return mRenderGuid
      End Get
      Set(value As Boolean)
        mRenderGuid = value
      End Set
    End Property

    Public ReadOnly Property Data As Object
      Get
        Return mRootData
      End Get
    End Property

#End Region

#Region " Library Methods "

    Protected Sub SetRootData(Data As Object)
      mRootData = Data
      If TypeOf Data Is Type Then
        mRootType = mRootData
      Else
        mRootType = mRootData.GetType
      End If

    End Sub

    Protected Sub SetRootType(Type As Type)
      mRootType = Type
    End Sub

    'Friend Function AddTypeDefinition(Type As Type, Member As Member, MustRender As Boolean, Optional Instance As Object = Nothing) As Boolean
    '  Return mTypeDefinitionList.AddTypeDefinition(Type, Member, MustRender, Instance)
    'End Function

    Friend ReadOnly Property TypeDefinitionList As TypeDefinitionList
      Get
        Return mTypeDefinitionList
      End Get
    End Property

    'Friend Function GetTypeDefinition(Type As Type) As TypeDefinition
    '  Return mTypeDefinitionList.GetItem(Type)
    'End Function

    Public Sub BuildObjectGraph()

      If mRootObjectInfo Is Nothing Then
        Dim ph As New Data.JS.ObjectInfo.PropertyHelper(Me.GetType.GetProperty("Data", BindingFlags.Instance Or BindingFlags.Public), "Data", Me.GetType, mRootType)
        mRootObjectInfo = MemberList.GetMember(ph, Me, 0, Me, True)
      End If

    End Sub

    Private mGuidTracker As New Hashtable

    Protected Friend Overridable Function GetExistingObject(Guid As Guid) As Object
      Return mGuidTracker(Guid)
    End Function

    Protected Friend Overridable Function StoreObject(Obj As Object, Parent As Object, Optional GuidString As String = "") As StoreObjectInfo

      Dim NewGuid As Guid

      If TypeOf Obj Is ISingularBase Then
        NewGuid = CType(Obj, ISingularBase).Guid
      Else
        NewGuid = Guid.NewGuid
      End If

      Dim soi As New StoreObjectInfo With {.Guid = NewGuid, .PreviouslyAdded = GetExistingObject(NewGuid) IsNot Nothing}
      mGuidTracker(NewGuid) = Obj
      Return soi
    End Function

#End Region

#Region " Public Methods "

    Public Overridable Sub Deserialise(Obj As System.Dynamic.DynamicObject)
      Dim ObjectContainer As New Singular.Dynamic.DynamicStorage
      CType(ObjectContainer, Object).Data = Obj

      BuildObjectGraph()

      mRootObjectInfo.UpdateModel(ObjectContainer, Me)
    End Sub

    Public Overridable Function Deserialise(JSon As String) As Boolean

      Deserialise(System.Web.Helpers.Json.Decode(JSon, GetType(Object)))
      Return True

    End Function

    Public Function GetJSon(Optional OutputMode As Data.OutputType = OutputType.JSon) As String
      Dim Writer As New Data.JSonWriter
      Writer.Serialiser = Me
      Writer.OutputMode = OutputMode
      GetJSon(Writer)
      Return Writer.ToString
    End Function

    Public Sub GetJSon(Writer As Data.JSonWriter)

      BuildObjectGraph()

      mRootObjectInfo.JSonPropertyName = RootPropertyName

      mRootObjectInfo.RenderData(Writer, Me)

    End Sub

    Public Function GetModelAsJavascript() As String
      Dim JavaScriptWriter = New Singular.Web.Utilities.JavaScriptWriter

      'Render the types
      mTypeDefinitionList.RenderTypes(JavaScriptWriter)

      mRootObjectInfo.JSonPropertyName = "ViewModel"
      mRootObjectInfo.RenderModel(JavaScriptWriter)
      Return JavaScriptWriter.ToString
    End Function

    Public Function GetSchema(OutputMode As OutputType) As String

      BuildObjectGraph()

      Dim JSONWriter = New Data.JSonWriter
      JSONWriter.OutputMode = OutputMode
      mRootObjectInfo.JSonPropertyName = ""
      mRootObjectInfo.RenderSchema(JSONWriter)
      Return JSONWriter.ToString
    End Function

    Public Overridable Sub OnRenderData(Member As ObjectInfo.Member, ListOrObject As Object)

    End Sub

#End Region

  End Class

  Public Class StatelessJSSerialiser
    Inherits JSSerialiser

    Private mServerObject As Object

    Public Sub New(ServerObject As Object)
      IncludeIsDirty = True
      DefaultPropertySetMode = SetMode.SetBackingField
      SetRootData(ServerObject)
    End Sub

    Public Sub New(ObjectType As Type)
      DefaultPropertySetMode = SetMode.SetBackingField
      SetRootType(ObjectType)
    End Sub

    Public Shared Function DeserialiseObject(Of ObjectType)(JSonObject As Object) As ObjectType
      Return DeserialiseObject(GetType(ObjectType), JSonObject)
    End Function

    Public Shared Function DeserialiseObject(Type As Type, JSonObject As Object) As Object
      Dim NewObj = Activator.CreateInstance(Type)
      Dim s As New StatelessJSSerialiser(NewObj)
      s.Deserialise(JSonObject)
      Return NewObj
    End Function

  End Class

  Public Class ServerJSSerialiser
    Inherits StatelessJSSerialiser

    Public Sub New(ServerObject As Object)
      MyBase.New(ServerObject)

      IncludeIsDirty = True
      RenderDeletedList = True

    End Sub

  End Class

End Namespace

















