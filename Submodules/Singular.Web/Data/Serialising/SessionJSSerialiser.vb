Imports System.Reflection
Imports System.Text
Imports System.Web.Helpers
Imports System.Dynamic
Imports Singular.Web.Data.JS.ObjectInfo

Namespace Data.JS

  '  Public MustInherit Class JSSerialiser

  '    ''' <summary>
  '    ''' When true, encrypts properties that have a Key attribute, so that the key cannot be changed on the client.
  '    ''' </summary>
  '    Public Shared Property ProtectKeyProperties As Boolean = True

  '    Public Class StoreObjectInfo
  '      Public Guid As Guid
  '      Public PreviouslyAdded As Boolean = False
  '    End Class

  '    Private mTypeDefinitionList As New TypeDefinitionList
  '    Protected mRootObjectInfo As Member
  '    Private mRootData As Object
  '    Private mRootType As Type

  '    Friend ReadOnly Property RootObjectInfo As Member
  '      Get
  '        Return mRootObjectInfo
  '      End Get
  '    End Property

  '    Public Enum SetMode
  '      SetProperty = 1
  '      SetBackingField = 2
  '    End Enum

  '#Region " Properties "

  '    Public Property SortProperties As Boolean = False

  '    Public Property RootPropertyName As String = ""

  '    Public Property ContextList As New UIContextList

  '    Public Property PropertySetMode As SetMode = SetMode.SetProperty

  '    Public Property RenderDeletedList As Boolean = False

  '    Public Property IncludeIsDirty As Boolean = False

  '    ''' <summary>
  '    ''' Indicates that this is an inital request for data, e.g. a full page load, not an ajax request.
  '    ''' </summary>
  '    Friend Property IsInitial As Boolean = True

  '    Private mRenderGuid As Boolean = True
  '    Public Property RenderGuid As Boolean
  '      Get
  '        Return mRenderGuid
  '      End Get
  '      Set(value As Boolean)
  '        mRenderGuid = value
  '      End Set
  '    End Property

  '    Public ReadOnly Property Data As Object
  '      Get
  '        Return mRootData
  '      End Get
  '    End Property

  '#End Region

  '#Region " Library Methods "

  '    Protected Sub SetRootData(Data As Object)
  '      mRootData = Data
  '      If TypeOf Data Is Type Then
  '        mRootType = mRootData
  '      Else
  '        mRootType = mRootData.GetType
  '      End If

  '    End Sub

  '    Protected Sub SetRootType(Type As Type)
  '      mRootType = Type
  '    End Sub

  '    Friend Function AddTypeDefinition(Type As Type, Member As Member, MustRender As Boolean, Optional Instance As Object = Nothing) As Boolean
  '      Return mTypeDefinitionList.AddTypeDefinition(Type, Member, MustRender, Instance)
  '    End Function

  '    Friend Function GetTypeDefinition(Type As Type) As TypeDefinition
  '      Return mTypeDefinitionList.GetItem(Type)
  '    End Function

  '    Public Sub BuildObjectGraph()

  '      If mRootObjectInfo Is Nothing Then
  '        Dim ph As New Data.JS.ObjectInfo.PropertyHelper(Me.GetType.GetProperty("Data", BindingFlags.Instance Or BindingFlags.Public), "Data", Me.GetType, mRootType)
  '        mRootObjectInfo = MemberList.GetMember(ph, Me, 0, Me, True)
  '      End If

  '    End Sub

  '    Private mGuidTracker As New Hashtable

  '    Friend Overridable Function GetExistingObject(Guid As Guid) As Object
  '      Return mGuidTracker(Guid)
  '    End Function

  '    Friend Overridable Function StoreObject(Obj As Object, Parent As Object, Optional GuidString As String = "") As StoreObjectInfo

  '      Dim NewGuid As Guid

  '      If TypeOf Obj Is ISingularBase Then
  '        NewGuid = CType(Obj, ISingularBase).Guid
  '      Else
  '        NewGuid = Guid.NewGuid
  '      End If

  '      Dim soi As New StoreObjectInfo With {.Guid = NewGuid, .PreviouslyAdded = GetExistingObject(NewGuid) IsNot Nothing}
  '      mGuidTracker(NewGuid) = Obj
  '      Return soi
  '    End Function

  '#End Region

  '#Region " Public Methods "

  '    Public Overridable Sub Deserialise(Obj As System.Dynamic.DynamicObject)
  '      Dim ObjectContainer As New Singular.Dynamic.DynamicStorage
  '      CType(ObjectContainer, Object).Data = Obj

  '      BuildObjectGraph()

  '      mRootObjectInfo.UpdateModel(ObjectContainer, Me)
  '    End Sub

  '    Public Overridable Function Deserialise(JSon As String) As Boolean

  '      Deserialise(System.Web.Helpers.Json.Decode(JSon, GetType(Object)))
  '      Return True

  '    End Function

  '    Public Function GetJSon(Optional OutputMode As Data.OutputType = OutputType.JSon) As String
  '      Dim Writer As New Data.JSonWriter
  '      Writer.OutputMode = OutputMode
  '      GetJSon(Writer)
  '      Return Writer.ToString
  '    End Function

  '    Public Sub GetJSon(Writer As Data.JSonWriter)

  '      BuildObjectGraph()

  '      mRootObjectInfo.JSonPropertyName = RootPropertyName

  '      mRootObjectInfo.RenderData(Writer, Me)

  '    End Sub

  '    Public Function GetModelAsJavascript() As String
  '      Dim JavaScriptWriter = New Singular.Web.Utilities.JavaScriptWriter

  '      'Render the types
  '      mTypeDefinitionList.RenderTypes(JavaScriptWriter)

  '      mRootObjectInfo.JSonPropertyName = "ViewModel"
  '      mRootObjectInfo.RenderModel(JavaScriptWriter)
  '      Return JavaScriptWriter.ToString
  '    End Function

  '    Public Function GetSchema(OutputMode As OutputType) As String

  '      BuildObjectGraph()

  '      Dim JSONWriter = New Data.JSonWriter
  '      JSONWriter.OutputMode = OutputMode
  '      mRootObjectInfo.JSonPropertyName = ""
  '      mRootObjectInfo.RenderSchema(JSONWriter)
  '      Return JSONWriter.ToString
  '    End Function

  '#End Region

  '  End Class

  '  Public Class StatelessJSSerialiser
  '    Inherits JSSerialiser

  '    Private mServerObject As Object

  '    Public Sub New(ServerObject As Object)
  '      PropertySetMode = SetMode.SetBackingField
  '      SetRootData(ServerObject)
  '    End Sub

  '    Public Sub New(ObjectType As Type)
  '      PropertySetMode = SetMode.SetBackingField
  '      SetRootType(ObjectType)
  '    End Sub

  '    Public Shared Function DeserialiseObject(Of ObjectType)(JSonObject As Object)
  '      Return DeserialiseObject(GetType(ObjectType), JSonObject)
  '    End Function

  '    Public Shared Function DeserialiseObject(Type As Type, JSonObject As Object)
  '      Dim NewObj = Activator.CreateInstance(Type)
  '      Dim s As New StatelessJSSerialiser(NewObj)
  '      s.Deserialise(JSonObject)
  '      Return NewObj
  '    End Function

  '  End Class

  '  Public Class ServerJSSerialiser
  '    Inherits StatelessJSSerialiser

  '    Public Sub New(ServerObject As Object)
  '      MyBase.New(ServerObject)

  '      IncludeIsDirty = True
  '      RenderDeletedList = True

  '    End Sub

  '  End Class

  <Serializable()>
  Public Class SessionVMJSSerialiser
    Inherits JSSerialiser

		Private mViewModel As IViewModel
    Friend ReadOnly Property IViewModel As IViewModel
      Get
        Return mViewModel
      End Get
    End Property

    Public Sub New(ViewModel As IViewModel, Data As Object)
      mViewModel = ViewModel
      SetRootData(Data)
    End Sub

    Public Sub UpdateItemTracker()
      UpdateItemTracker(mRootObjectInfo, Me)
    End Sub

    Private Sub UpdateItemTracker(Member As Member, Model As Object)

      If Model IsNot Nothing Then
        Dim CurrentInstance = Member.PropertyHelper.GetValue(Model)

        If TypeOf Member Is ComplexListMember Then

          If CurrentInstance IsNot Nothing Then
            For Each Item As Object In CurrentInstance

              StoreObject(Item, CurrentInstance)

              For Each m As Member In Member.MemberList
                UpdateItemTracker(m, Item)
              Next
            Next
          End If

        ElseIf TypeOf Member Is ObjectMember Then

          StoreObject(CurrentInstance, Nothing)

          For Each m As Member In Member.MemberList
            UpdateItemTracker(m, CurrentInstance)
          Next

        End If

      End If

    End Sub

    Public Function Find(ObjectToFind As Object) As Member
      Return Find(mRootObjectInfo, Me, ObjectToFind)
    End Function

    Private Function Find(Member As Member, Model As Object, ObjectToFind As Object) As Member
      If Model IsNot Nothing Then
        Dim CurrentInstance = Member.PropertyHelper.GetValue(Model)

        If TypeOf Member Is ComplexListMember Then

          If CurrentInstance IsNot Nothing Then
            For Each Item As Object In CurrentInstance

              If Item Is ObjectToFind Then
                Return Member
              End If

              For Each m As Member In Member.MemberList
                Dim Found = Find(m, Item, ObjectToFind)
                If Found IsNot Nothing Then
                  Return Found
                End If
              Next

            Next
          End If

        ElseIf TypeOf Member Is ObjectMember Then

          If CurrentInstance Is ObjectToFind Then
            Return Member
          End If

          For Each m As Member In Member.MemberList
            Dim Found = Find(m, CurrentInstance, ObjectToFind)
            If Found IsNot Nothing Then
              Return Found
            End If
          Next

        End If
      End If

      Return Nothing
    End Function

    Public Overrides Function Deserialise(JSon As String) As Boolean

      Dim obj As DynamicJsonObject = System.Web.Helpers.Json.Decode(JSon, GetType(Object))

      'Check that the Server ViewModel Guid matches the Client
      Dim Guid As String = ""
      obj.TryGetMember(New Singular.Dynamic.MemberGetter("Guid"), Guid)
      If Guid IsNot Nothing AndAlso mViewModel.ServerObjectTracker.GetObjectInfo(New Guid(Guid)) Is Nothing Then
        'View Model is out of date with the client. Usually happens when the user clicks the back button.
        Return False
      End If

      Deserialise(obj)
      Return True
    End Function

    Protected Overrides Function GetExistingObject(Guid As Guid)
      Return mViewModel.ServerObjectTracker.GetObject(Guid)
    End Function

    Protected Overrides Function StoreObject(Obj As Object, Parent As Object, Optional GuidString As String = "") As StoreObjectInfo
      Dim Existed As Boolean = False
      Dim NewGuid As Guid = mViewModel.ServerObjectTracker.AddObject(Obj, Parent, GuidString, Existed)
      Return New StoreObjectInfo With {.Guid = NewGuid, .PreviouslyAdded = Existed}
    End Function

    Public Overrides Sub OnRenderData(Member As Member, ListOrObject As Object)

      For Each m As Member In Member.MemberList
        If TypeOf m Is SimpleMember Then

          'Check if its a drop down field.
          Dim dda = CType(m, SimpleMember).DropDownAttribute

          If dda IsNot Nothing Then

            If dda.DropDownType = Singular.DataAnnotations.DropDownWeb.SelectType.FindScreen AndAlso dda.LookupMember = "" Then
              Dim cds As ClientDataProvider.ClientDataSource = IViewModel.ClientDataProvider.DataSourceList.GetItem(dda.Name)
              If cds IsNot Nothing Then

                Dim PreFilterValues As New Hashtable

                If ListOrObject IsNot Nothing Then

                  'Get the pre filter values
                  If ListOrObject.GetType.GetInterfaces.Contains(GetType(IEnumerable)) Then
                    For Each item As Object In ListOrObject
                      Dim ChildValue As Object = m.PropertyHelper.PropertyInfo.GetValue(item, Nothing)
                      If ChildValue IsNot Nothing AndAlso ChildValue IsNot DBNull.Value Then
                        PreFilterValues(ChildValue) = True
                      End If
                    Next
                  Else
                    Dim ChildValue As Object = m.PropertyHelper.PropertyInfo.GetValue(ListOrObject, Nothing)
                    If ChildValue IsNot Nothing AndAlso ChildValue IsNot DBNull.Value Then
                      PreFilterValues(ChildValue) = True
                    End If
                  End If

                End If

                'Check if the list needs to be filtered
                Dim di As Singular.DataAnnotations.DropDownWeb.DataSourceInfo = Singular.DataAnnotations.DropDownWeb.GetDataSource(dda.GetCriteriaClass, dda.GetPropertyName, dda.Source, IViewModel)

                Dim FListType As Type = GetType(List(Of )).MakeGenericType(Singular.Reflection.GetLastGenericType(di.Data.GetType))
                Dim FList = Activator.CreateInstance(FListType)

                If PreFilterValues.Count > 0 Then
                  For Each item As Object In di.Data
                    If PreFilterValues.ContainsKey(dda.GetValueMemberPI.GetValue(item, Nothing)) Then
                      FList.Add(item)
                    End If
                  Next
                End If

                cds.Data = FList
                cds.Context = dda.FindContextKey

              End If
            End If

          End If

        End If
      Next

    End Sub

  End Class

End Namespace

















