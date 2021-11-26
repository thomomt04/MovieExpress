Imports System.Reflection

Namespace Data

  Public Class ClientDataProvider

    Private mViewModel As IViewModel

    Public Sub New(ViewModel As IViewModel)
      mViewModel = ViewModel
    End Sub

    ''' <summary>
    ''' Checks if the object implements WCDF.IGetData, if so, calls GetData on the object, if not, calls FetchList
    ''' </summary>
    Public Shared Function GetData(StatelessArgs As Singular.Web.WebServices.GetDataArguments) As Object

      If StatelessArgs.ImplementsIGetData Then

        Return CType(StatelessArgs.StatelessInstance, Singular.Web.WebServices.IGetData).GetData(StatelessArgs)

      Else

        Return FetchList(StatelessArgs.Type, StatelessArgs)

      End If

    End Function

    ''' <summary>
    ''' Returns a list of the supplied type using its criteria object.
    ''' </summary>
    Public Shared Function FetchList(ListOrCriteriaType As Type, Args As Object, Optional IgnoreSecurity As Boolean = False) As Object

      Dim ListType As Type = Nothing
      Dim CriteriaType As Type = nothing

      Singular.Reflection.ResolveCriteriaType(ListOrCriteriaType, ListType, CriteriaType)

      'Check if the user is allowed to fetch this list.
      If Not IgnoreSecurity Then
        Dim wc = Singular.Reflection.GetAttribute(Of Singular.Web.WebFetchable)(If(CriteriaType Is Nothing, ListType, CriteriaType))
        If wc Is Nothing Then
          Throw New Exception("Fetch not allowed via web interface on type '" & ListType.Name & "'")
        ElseIf Not wc.Allowed Then
          Throw New Exception("User does not have permission to Fetch on type '" & ListType.Name & "'")
        End If
      End If

      If CriteriaType IsNot Nothing Then

        'Create Criteria Object
        Dim Criteria As Object

        'Populate the Criteria Object
        If TypeOf Args Is WebServices.GetDataArguments AndAlso CType(Args, WebServices.GetDataArguments).JSonArgs Is Nothing Then

          Criteria = Activator.CreateInstance(CriteriaType)
          For Each member In Args.QueryString.AllKeys
            Dim pi As PropertyInfo = CriteriaType.GetProperty(member, BindingFlags.Public Or BindingFlags.Instance)
            If pi IsNot Nothing AndAlso pi.CanWrite Then
              pi.SetValue(Criteria, Singular.Reflection.ConvertValueToType(pi.PropertyType, Args.QueryString(member)), Nothing)
            End If
          Next
        Else
          Dim DArgs As System.Dynamic.DynamicObject = If(TypeOf Args Is WebServices.GetDataArguments, CType(Args, WebServices.GetDataArguments).JSonArgs, Args)

          Criteria = JS.StatelessJSSerialiser.DeserialiseObject(CriteriaType, DArgs)
          'For Each member As String In DArgs.GetDynamicMemberNames
          '  Dim pi As PropertyInfo = CriteriaType.GetProperty(member, BindingFlags.Public Or BindingFlags.Instance)
          '  If pi IsNot Nothing AndAlso pi.CanWrite Then
          '    Dim value As Object = Nothing
          '    If DArgs.TryGetMember(New Singular.Dynamic.MemberGetter(member), value) Then
          '      pi.SetValue(Criteria, Singular.Reflection.ConvertValueToType(pi.PropertyType, value), Nothing)
          '    End If
          '  End If
          'Next
        End If


        'Fetch the list
        Return Singular.Reflection.FetchList(ListType, Criteria)

      Else
        'if there is no criteria object, then create an instance and return it.
        Return Activator.CreateInstance(ListType, True)
      End If

      Return Nothing

    End Function

    Private mDatasourceList As ClientDataSourceList
    Public ReadOnly Property DataSourceList As ClientDataSourceList
      Get
        If mDatasourceList Is Nothing Then
          mDatasourceList = New ClientDataSourceList
        End If
        Return mDatasourceList
      End Get
    End Property

    ''' <summary>
    ''' Add some readonly data to be used on the client
    ''' </summary>
    Public Sub AddDataSource(SourceName As String, Data As Object, Optional CacheOnClient As Boolean = False)
      AddName(SourceName, Data, CacheOnClient, True)
    End Sub

    Public Sub AddVariable(VariableName As String, Data As Object)
      AddName(VariableName, Data, False, False)
    End Sub

    ''' <summary>
    ''' Adds a variable to ClientData. Encloses the provided string in double quotes.
    ''' </summary>
    Public Sub AddStringVariable(VariableName As String, Data As String)
      AddVariable(VariableName, """" & HttpUtility.JavaScriptStringEncode(Data) & """")
    End Sub

    Private Sub AddName(Name As String, Data As Object, CacheOnClient As Boolean, IsList As Boolean)
      If Data IsNot Nothing Then

        If Name.StartsWith("ClientData.") Then
          Name = Name.Substring(11)
        End If

        Dim ds = DataSourceList.GetOrAddDataSource(Name)
        ds.Data = Data
        ds.DontCache = Not CacheOnClient
        ds.IsList = IsList

      End If
    End Sub

    ''' <summary>
    ''' Adds some readonly data to be used on the client. Providing the source location of the data.
    ''' </summary>
    Public Sub AddDataSource(Source As Singular.DataAnnotations.DropDownWeb.SourceType, PropertyName As String)
      Dim ds = DataSourceList.GetOrAddDataSource(PropertyName)
      ds.Source = Source
      ds.PropertyName = PropertyName
    End Sub

    ''' <summary>
    ''' Adds some readonly data to be used on the client. Providing the source location of the data, and the property type to look for on the source.
    ''' </summary>
    Public Sub AddDataSource(Source As Singular.DataAnnotations.DropDownWeb.SourceType, ListType As Type)
      Dim ds = DataSourceList.GetOrAddDataSource(ListType.Name)
      ds.Source = Source
      ds.ListType = ListType
    End Sub

    ''' <summary>
    ''' Adds some readonly data to be used on the client. Providing the source location of the data, and the property type to look for on the source.
    ''' </summary>
    Public Sub AddDataSource(Of ListType)(Optional Source As Singular.DataAnnotations.DropDownWeb.SourceType = Singular.DataAnnotations.DropDownWeb.SourceType.All)
      Dim t = GetType(ListType)
      Dim ds = DataSourceList.GetOrAddDataSource(t.Name)
      ds.Source = Source
      ds.ListType = t
    End Sub

    Friend Sub AddDropDownDataSource(DDA As Singular.DataAnnotations.DropDownWeb)

      'Dont overwrite in case the datasource has been added manually in viewmodel setup.
      If Not DataSourceList.HasDataSource(DDA.Name) Then

        Dim ds = DataSourceList.GetOrAddDataSource(DDA.Name)
        ds.Source = DDA.Source

        If DDA.AddsCustomData Then

          'For Find Screen, just mark that there is a control that wants to use this data.
          'The data will actually be added in SimpleMember, when the model data is rendered.
          ds.DontCache = True
        Else

          ds.ListType = DDA.ListType
          ds.CriteriaType = DDA.GetCriteriaClass(False)
          If ds.CriteriaType Is Nothing Then
            ds.CriteriaType = DDA.ListType
          End If
          ds.PropertyName = DDA.GetPropertyName
          If DDA.Source = Singular.DataAnnotations.DropDownWeb.SourceType.Fetch Then
            ds.DontCache = True
          End If

        End If
      End If

    End Sub

    Public Function FetchList(Args As GetDataArgs) As Object

      If Args.CriteriaType IsNot Nothing Then

        Return ClientDataProvider.FetchList(Args.CriteriaType, Args.ClientArgs)

      Else
        Dim ds As ClientDataSource = DataSourceList.GetItem(Args.Context)
        If ds IsNot Nothing Then
          Return ClientDataProvider.FetchList(ds.ListType, Args.ClientArgs, True)
        Else
          Return Nothing
        End If
      End If

    End Function

    Public Shared Function GetCacheableJSon(Data As Object, Optional Context As String = "", Optional OutputMode As Data.OutputType = OutputType.JSon) As String
      Dim JSonData As String
      If TypeOf Data Is String Then
        JSonData = Data
      Else
        Dim Serialiser As New Data.JS.StatelessJSSerialiser(Data)
        Serialiser.RenderGuid = False
        Serialiser.SortProperties = True 'Properties must always be in the same order, otherwise the Hash version checking is pointless.
        If Context <> "" Then
          Serialiser.ContextList.AddContext(Context)
        End If

        JSonData = Serialiser.GetJSon(OutputMode)
      End If
      Return JSonData
    End Function

    Private Shared Function GetCacheableJSonWithHash(Data As Object, Optional Context As String = "", Optional OutputMode As Data.OutputType = OutputType.JSon) As JSonCache
      Dim jc As New JSonCache
      jc.JSon = GetCacheableJSon(Data, Context, OutputMode)
      jc.Hash = Singular.Encryption.GetStringHash(jc.JSon, Encryption.HashType.Sha256, False)
      Return jc
    End Function

    Private Shared Function GetCacheableJSonWithHash(CData As CommonData.CachedData) As JSonCache
      If CData.JSon = "" Then
        CData.JSon = GetCacheableJSon(CData.Data)
        CData.JSonHash = Singular.Encryption.GetStringHash(CData.JSon, Encryption.HashType.Sha256, False)
      End If
      Return New JSonCache With {.JSon = CData.JSon, .Hash = CData.JSonHash}
    End Function

    Public Shared Function GetCacheableJSon(ListOrCriteriaType As Type, PropertyName As String, Source As Singular.DataAnnotations.DropDownWeb.SourceType, ViewModel As Object) As JSonCache

      Dim dsi = Singular.DataAnnotations.DropDownWeb.GetDataSource(ListOrCriteriaType, PropertyName, Source, ViewModel)
      Dim ListType As Type = Nothing
      Singular.Reflection.ResolveCriteriaType(ListOrCriteriaType, ListType, Nothing)
      Dim jc As New JSonCache

      If dsi IsNot Nothing Then
        If dsi.RetreivedFrom = Singular.DataAnnotations.DropDownWeb.SourceType.CommonData Then
          'From CommonData, retreive the cached json.
          jc = GetCacheableJSonWithHash(CommonData.Lists.GetCachedData(ListType, PropertyName))
          'jc = Data.CommonDataJSonCache.GetJSonCache(PropertyName, ListType, False)
        ElseIf dsi.RetreivedFrom = Singular.DataAnnotations.DropDownWeb.SourceType.SessionData Then
          'From SessionData, retreive the cached json.
          'jc = Data.CommonDataJSonCache.GetJSonCache(PropertyName, ListType, True)
          jc = GetCacheableJSonWithHash(CommonData.SessionLists.GetCachedData(ListType, PropertyName))
        Else
          'Get the Json, and Hash from the data.
          jc = GetCacheableJSonWithHash(dsi.Data)
        End If
        jc.RetrievedFrom = dsi.RetreivedFrom
        Return jc
      Else
        Return Nothing
      End If

    End Function

#Region " Data Structures "

    Public Class ClientDataSource

      Public Property Source As Singular.DataAnnotations.DropDownWeb.SourceType = Singular.DataAnnotations.DropDownWeb.SourceType.None

      Public Property SourceName As String
      Public Property Data As Object

      Public Property ListType As Type
      Public Property CriteriaType As Type

      Public ReadOnly Property ListOrCriteriaType As Type
        Get
          If CriteriaType Is Nothing Then
            Return ListType
          Else
            Return CriteriaType
          End If
        End Get
      End Property

      ''' <summary>
      ''' E.g Look for a specific property Name on Commondata, not the type.
      ''' </summary>
      Public Property PropertyName As String
      Public Property DontCache As Boolean = False
      Public Property Context As String = ""
      'Public Property DDA As Singular.DataAnnotations.DropDownWeb
      Public Property IsList As Boolean = True

      Public Shared Sub WriteToPage(Writer As Singular.Web.Utilities.JavaScriptWriter,
                                          SourceName As String,
                                          ListType As Type,
                                          Optional PropertyName As String = "",
                                          Optional Source As Singular.DataAnnotations.DropDownWeb.SourceType = Singular.DataAnnotations.DropDownWeb.SourceType.All)

        Dim cds As New ClientDataSource
        cds.SourceName = SourceName
        cds.ListType = ListType
        cds.PropertyName = PropertyName
        cds.Source = Source
        cds.WriteToPage(Writer)

      End Sub

      Public Sub WriteToPage(Writer As Singular.Web.Utilities.JavaScriptWriter,
                                        Optional ViewModel As Object = Nothing)

        Dim jc As JSonCache
        Dim RetrievedFrom As Singular.DataAnnotations.DropDownWeb.SourceType = Singular.DataAnnotations.DropDownWeb.SourceType.None

        If Data IsNot Nothing Then
          'Data has been provided.
          jc = GetCacheableJSonWithHash(Data, , If(DontCache, Singular.Web.Data.OutputType.Javascript, Singular.Web.Data.OutputType.JSon))
        Else
          'No data provided, need to check where to get data from.
          jc = GetCacheableJSon(ListOrCriteriaType, PropertyName, Source, ViewModel)
        End If

        'Check if the browser supports local storage.

        If Misc.BroswerInfo.SupportsLocalStorage AndAlso Not DontCache Then

          Dim ListOrPropertyName As String = ""
          If PropertyName <> "" Then
            ListOrPropertyName = PropertyName
          ElseIf ListOrCriteriaType IsNot Nothing Then
            ListOrPropertyName = Singular.ReflectionCached.GetCachedType(ListOrCriteriaType).DotNetTypeName
          End If
          If SourceName = "" Then
            SourceName = ListOrPropertyName
          End If

          If jc IsNot Nothing Then
            RetrievedFrom = jc.RetrievedFrom
            Writer.Write("RegisterLookupData(""" & SourceName & """, """ &
                       ListOrPropertyName & """, """ &
                       RetrievedFrom.ToString.ToUpper.Chars(0) & """, """ &
                       jc.Hash & """);")
          Else
            Writer.Write("ClientData." & SourceName & " = Singular.CreateList([]);")
          End If

        Else
          If IsList Then
            Writer.Write("ClientData." & SourceName & " = Singular.CreateList(" & jc.JSon & ");")
          Else
            Writer.Write("ClientData." & SourceName & " = " & jc.JSon & ";")
          End If

        End If

      End Sub

    End Class

    Public Class ClientDataSourceList
      Inherits List(Of ClientDataSource)

      Public Function GetOrAddDataSource(SourceName As String) As ClientDataSource

        For Each source As ClientDataSource In Me
          If source.SourceName = SourceName Then
            Return source
          End If
        Next

        Dim NewDS As New ClientDataSource
        NewDS.SourceName = SourceName
        Add(NewDS)
        Return NewDS

      End Function

      Public Function HasDataSource(SourceName As String) As Boolean
        Return GetItem(SourceName) IsNot Nothing
      End Function

      Public Function GetItem(SourceName As String) As ClientDataSource
        For Each Item As ClientDataSource In Me
          If Item.SourceName = SourceName Then
            Return Item
          End If
        Next
        Return Nothing
      End Function

    End Class

#End Region

  End Class

End Namespace
