Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.HttpContext

Namespace CustomControls

  Public Class SiteMapDataSource
    Inherits System.Web.UI.WebControls.SiteMapDataSource

    Public Class RenderItemArgs
      Public Property ShowItem As Boolean = True
      Public Property DataBinding As String
      Public Property Item As SiteMapNode
      Public Property NewTitle As String
    End Class

    Public Property OnItemRender As Action(Of RenderItemArgs)

    Protected Overrides Function GetHierarchicalView(viewPath As String) As System.Web.UI.HierarchicalDataSourceView

      Dim root As SiteMapNode = Provider.RootNode

      Return New SiteMapHierarchicalDataSourceView(GetAllowedItems(root.ChildNodes))

    End Function

    Public Overloads Function GetHierarchicalView() As System.Web.UI.HierarchicalDataSourceView
      Dim root As SiteMapNode = Provider.RootNode
      Return New SiteMapHierarchicalDataSourceView(GetAllowedItems(root.ChildNodes))
    End Function

    Public Overloads Function GetHierarchicalViewAll() As System.Web.UI.HierarchicalDataSourceView
      Dim root As SiteMapNode = Provider.RootNode
      Return New SiteMapHierarchicalDataSourceView(root.ChildNodes)
		End Function

		Public Function GetAllNodes() As SiteMapNodeCollection
			Dim root As SiteMapNode = Provider.RootNode
			Return GetAllowedItems(root.ChildNodes)
		End Function

    Private Function GetAllowedItems(EntireList As SiteMapNodeCollection) As SiteMapNodeCollection
      Dim FilteredList As New SiteMapNodeCollection
      For Each item As SiteMapNode In EntireList

        Dim CanRenderItemArgs As New RenderItemArgs

        If OnItemRender IsNot Nothing Then
          item.ReadOnly = False
          CanRenderItemArgs.Item = item
          CanRenderItemArgs.ShowItem = True
          OnItemRender.Invoke(CanRenderItemArgs)
          If Not CanRenderItemArgs.ShowItem Then
            Continue For
          End If
        End If

        If item.Title = "" AndAlso item.ResourceKey = "" Then
          Continue For
        End If

        If item.Roles.Count > 0 Then
          'If the user doesnt have the required role, then hide the menu item.
          If Not (Current.Request.IsAuthenticated AndAlso Singular.Security.HasAccess(item.Roles(0))) Then
            Continue For
          End If
        End If

        If item("LoggedIn") IsNot Nothing Then
          'If the LoggedIn attribute is set to false, and the user is logged in, then hide the item.
          'Also if the LoggedIn attribute is set to true, and the user is not logged in, then hide the item.
          If Current.Request.IsAuthenticated <> item("LoggedIn") Then
            Continue For
          End If
        End If


        If item.ResourceKey IsNot Nothing Then
          'Localisation
          item.ReadOnly = False
          item.Title = Singular.Localisation.LocalText(item.ResourceKey)
        End If

        Dim NewItem As SiteMapNode = item.Clone
        If item.ChildNodes.Count > 0 Then

          Dim Filtered As SiteMapNodeCollection = GetAllowedItems(item.ChildNodes)
          Dim HideIfEmpty As Boolean = item.Url = ""
          If item("HideEmpty") IsNot Nothing Then
            HideIfEmpty = item("HideEmpty")
          End If

          If Filtered.Count = 0 AndAlso HideIfEmpty Then
            'Items with collapse, and no child items must be hidden.
            Continue For
          ElseIf item("Collapse") IsNot Nothing AndAlso item("Collapse") AndAlso Filtered.Count = 1 Then
            NewItem.ReadOnly = False
            NewItem.Title = Filtered(0).Title
            NewItem.Url = Filtered(0).Url
            NewItem.ChildNodes = New SiteMapNodeCollection
          Else
            NewItem.ChildNodes = Filtered
          End If

        End If
        If Not String.IsNullOrEmpty(CanRenderItemArgs.DataBinding) Then
          NewItem("data-bind") = CanRenderItemArgs.DataBinding
        End If

        FilteredList.Add(NewItem)
      Next
      Return FilteredList
    End Function

#Region " Security "

    Private Shared mSiteMaps As New List(Of System.Web.UI.HierarchicalDataSourceView)
    Private Shared mSiteMapNotFound As Boolean = False
    Private Shared mPageRoles As New Hashtable

    Private Class PageInfo
      Public Property PageRoles As New List(Of String)
      Public Property LoggedInOnly As Boolean
    End Class

    Public Shared Function HasAccess(PagePath As String) As Boolean
      SyncLock mPageRoles

        If mSiteMapNotFound Then
          Return True
        End If


        If Not mPageRoles.ContainsKey(PagePath) Then

          'if the page roles havent been cached yet.
          If mSiteMaps.Count = 0 Then
            Dim Settings As System.Web.Configuration.SiteMapSection = System.Web.HttpContext.Current.GetSection("system.web/siteMap")
            If Settings Is Nothing Then
              mSiteMapNotFound = True
            Else

              For i As Integer = 0 To Settings.Providers.Count - 1

                Dim smd As New Singular.Web.CustomControls.SiteMapDataSource
                smd.SiteMapProvider = Settings.Providers(i).Name

                Try
                  mSiteMaps.Add(smd.GetHierarchicalViewAll())
                Catch ex As Exception
                End Try

              Next

              If mSiteMaps.Count = 0 Then
                mSiteMapNotFound = True
                Return True
              End If

            End If

          End If

          For Each SiteMap As System.Web.UI.HierarchicalDataSourceView In mSiteMaps
            Dim pi = GetRoles(Utils.URL_ToAbsolute(PagePath).ToLower, SiteMap.Select())
            If pi IsNot Nothing Then
              mPageRoles(PagePath) = pi
              Exit For
            End If
          Next

        End If

        If mPageRoles(PagePath) IsNot Nothing Then

          If Not Singular.Security.HasAuthenticatedUser AndAlso mPageRoles(PagePath).LoggedInOnly Then
            Return False
          End If

          For Each role As String In mPageRoles(PagePath).PageRoles
            If Not Singular.Security.HasAccess(role) Then
              Return False
            End If
          Next
          'if the user has ALL the roles, return true
          Return True
        Else
          'if the page has no roles, return true
          Return True
        End If

      End SyncLock
    End Function

    Private Shared Function GetRoles(PagePath As String, Nodes As SiteMapNodeCollection) As PageInfo
      For Each smd As System.Web.SiteMapNode In Nodes
        If smd.Url <> "" AndAlso VirtualPathUtility.IsAbsolute(smd.Url) AndAlso Utils.URL_ToAbsolute(smd.Url).ToLower = PagePath Then
          Dim pi As New PageInfo
          For Each role As String In smd.Roles
            pi.PageRoles.Add(role)
          Next
          pi.LoggedInOnly = smd("LoggedIn") IsNot Nothing AndAlso smd("LoggedIn").ToLower = "true"
          Return pi
        End If
        If smd.ChildNodes.Count > 0 Then
          Dim pi = GetRoles(PagePath, smd.ChildNodes)
          If pi IsNot Nothing Then
            'Page was in the child nodes, add this parent nodes roles as well, as
            'the child node will not be visible if the parent is hidden.
            For Each role As String In smd.Roles
              pi.PageRoles.Add(role)
            Next
            Return pi
          End If
        End If
      Next
      Return Nothing
    End Function

#End Region

  End Class


End Namespace
