' Generated 14 Oct 2014 15:51 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace Reporting

  <Serializable()> _
  Public Class ROGridUserInfoList
    Inherits SingularReadOnlyListBase(Of ROGridUserInfoList, ROGridUserInfo)

#Region "  Business Methods  "

    Public Sub AddDefaultLayouts(LayoutList As List(Of Singular.Reporting.GridInfo.LayoutInfo))

      LayoutList.Reverse()
      For Each Layout As Singular.Reporting.GridInfo.LayoutInfo In LayoutList
        If Me.Find(Layout.LayoutName.Trim) Is Nothing Then
          Me.Insert(0, ROGridUserInfo.CreateDefaultLayout(Layout.LayoutName, Layout.Layout))
        End If
      Next

    End Sub

    Public Function GetItem(WebGridUserInfoID As Integer) As ROGridUserInfo

      For Each child As ROGridUserInfo In Me
        If child.WebGridUserInfoID = WebGridUserInfoID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Function Find(LayoutName As String) As ROGridUserInfo

      For Each child As ROGridUserInfo In Me
        If child.LayoutName.Trim = LayoutName Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Web Grid User Infos"

    End Function

#End Region

#Region "  Data Access  "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Property UniqueName As String

      Public Sub New()

      End Sub

    End Class

#Region "  Common  "

    Public Shared Function NewROGridUserInfoList() As ROGridUserInfoList

      Return New ROGridUserInfoList()

    End Function

    Public Shared Sub BeginGetROGridUserInfoList(criteria As Criteria, CallBack As EventHandler(Of DataPortalResult(Of ROGridUserInfoList)))

      Dim dp As New DataPortal(Of ROGridUserInfoList)()
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(criteria)

    End Sub


    Public Shared Sub BeginGetROGridUserInfoList(CallBack As EventHandler(Of DataPortalResult(Of ROGridUserInfoList)))

      Dim dp As New DataPortal(Of ROGridUserInfoList)()
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria())

    End Sub

    Public Sub New()

      ' must have parameter-less constructor

    End Sub

#End Region

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Public Shared Function GetROGridUserInfoList(GridInfo As Singular.Web.SGridInfo, NamesOnly As Boolean) As ROGridUserInfoList

      Dim UserLayoutList = GetROGridUserInfoList(GridInfo.UniqueKey)
      UserLayoutList.AddDefaultLayouts(GridInfo.LayoutList)

      If NamesOnly Then
        For Each gui As ROGridUserInfo In UserLayoutList
          gui.ClearLayout()
        Next
      End If

      Return UserLayoutList

    End Function

    Public Shared Function GetROGridUserInfoList(UniqueName As String) As ROGridUserInfoList
      Return DataPortal.Fetch(Of ROGridUserInfoList)(New Criteria() With {.UniqueName = UniqueName})
    End Function

    Private Sub Fetch(sdr As SafeDataReader)

      Dim CurrentUserID = Singular.Settings.CurrentUserID

      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Dim gui = ROGridUserInfo.GetROGridUserInfo(sdr)
        gui.CreatedByOtherUser = CurrentUserID <> gui.UserID
        Me.Add(gui)
      End While
      Me.IsReadOnly = False
      Me.RaiseListChangedEvents = True

      If sdr.NextResult Then
        Dim Parent As ROGridUserInfo = Nothing
        While sdr.Read
          If Parent Is Nothing OrElse sdr.GetInt32(5) <> Parent.WebGridUserInfoID Then
            Parent = GetItem(sdr.GetInt32(5))
          End If
          Parent.ChildList.RaiseListChangedEvents = False
          Parent.ChildList.IsReadOnly = False
          Parent.ChildList.Add(ROGridUserInfo.GetROGridUserInfo(sdr))
          Parent.ChildList.IsReadOnly = True
          Parent.ChildList.RaiseListChangedEvents = True
        End While


      End If

    End Sub

    Protected Overrides Sub DataPortal_Fetch(criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getROWebGridUserInfoList"
            cm.Parameters.AddWithValue("@UniqueName", crit.UniqueName)
            If Singular.Security.CurrentIdentity IsNot Nothing Then
              cm.Parameters.AddWithValue("@UserID", Singular.Security.CurrentIdentity.UserID)
            End If

            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace