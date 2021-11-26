'Namespace Data

'  'This needs to be redesigned to store the json in the commondata CachedObject class
'  'Currently it wont handle different languages / context etc.

'  Public Class CommonDataJSonCache

'    Private Shared AppJSonStorage As New Hashtable

'    Public Shared ReadOnly Property SessionJSonStorage As Hashtable
'      Get
'        If System.Web.HttpContext.Current.Session("JSonStorage") Is Nothing Then
'          System.Web.HttpContext.Current.Session("JSonStorage") = New Hashtable
'        End If
'        Return System.Web.HttpContext.Current.Session("JSonStorage")
'      End Get
'    End Property

'    Shared Sub New()
'      AddHandler Singular.CommonData.Lists.PropertyRefreshed,
'        Sub(Name)

'          AppJSonStorage.Remove(Name)
'          SessionJSonStorage.Remove(Name)

'        End Sub
'    End Sub

'    Public Shared Function GetJSonCache(Name As String, FromSession As Boolean) As Data.ClientDataProvider.JSonCache

'      Dim ht = If(FromSession, SessionJSonStorage, AppJSonStorage)

'      If Not ht.ContainsKey(Name) Then
'        Dim Data As Object
'        If FromSession Then
'          Data = Singular.CommonData.SessionLists.GetList(Name)
'        Else
'          Data = Singular.CommonData.Lists.GetList(Name)
'        End If
'        ht(Name) = Singular.Web.Data.ClientDataProvider.GetCDataJSon(Data)
'      End If
'      Return ht(Name)

'    End Function

'    Public Shared Function GetJSonCache(ListType As Type, FromSession As Boolean) As Data.ClientDataProvider.JSonCache

'      If FromSession Then
'        Return GetJSonCache(Singular.CommonData.SessionLists.GetPropertyName(ListType), FromSession)
'      Else
'        Return GetJSonCache(Singular.CommonData.Lists.GetPropertyName(ListType), FromSession)
'      End If

'    End Function

'    Public Shared Function GetJSonCache(Name As String, ListType As Type, FromSession As Boolean) As Data.ClientDataProvider.JSonCache
'      If Name = "" Then
'        Return GetJSonCache(ListType, FromSession)
'      Else
'        Return GetJSonCache(Name, FromSession)
'      End If


'    End Function

'  End Class


'End Namespace

