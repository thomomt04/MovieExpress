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
  Public Class ROGridUserInfo
    Inherits SingularReadOnlyBase(Of ROGridUserInfo)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared WebGridUserInfoIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.WebGridUserInfoID, "Web Grid User Info", 0)
    ''' <summary>
    ''' Gets the Web Grid User Info value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property WebGridUserInfoID() As Integer
      Get
        Return GetProperty(WebGridUserInfoIDProperty)
      End Get
    End Property

    Public Shared WebGridInfoIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.WebGridInfoID, "Web Grid Info", Nothing)
    ''' <summary>
    ''' Gets the Web Grid Info value
    ''' </summary>
    <Browsable(False)>
    Public ReadOnly Property WebGridInfoID() As Integer?
      Get
        Return GetProperty(WebGridInfoIDProperty)
      End Get
    End Property

    Public Shared UserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.UserID, "User", 0)
    ''' <summary>
    ''' Gets the User value
    ''' </summary>
    <Browsable(False)>
    Public ReadOnly Property UserID() As Integer
      Get
        Return GetProperty(UserIDProperty)
      End Get
    End Property

    Public Property CreatedByOtherUser As Boolean

    Public Shared LayoutInfoProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.LayoutInfo, "Layout Info", "")
    ''' <summary>
    ''' Gets the Layout Info value
    ''' </summary>
    <Display(Name:="Layout Info", Description:="JSON object of the grids layout / groupings and filters.")>
    Public ReadOnly Property LayoutInfo() As String
      Get
        Return GetProperty(LayoutInfoProperty)
      End Get
    End Property

    Public Shared LayoutNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.LayoutName, "Layout Name", "")
    ''' <summary>
    ''' Gets the Layout Name value
    ''' </summary>
    <Display(Name:="Layout Name", Description:="Users description of the layout.")>
    Public ReadOnly Property LayoutName() As String
      Get
        Return GetProperty(LayoutNameProperty)
      End Get
    End Property

    <DefaultValue(True)>
    Public Property Saved As Boolean = True

    Public Shared ChildListProperty As PropertyInfo(Of ROGridUserInfoList) = RegisterProperty(Of ROGridUserInfoList)(Function(c) c.ChildList, "ChildList")

#End Region

#Region " Child List "

    Public ReadOnly Property ChildList As ROGridUserInfoList
      Get
        If GetProperty(ChildListProperty) Is Nothing Then
          LoadProperty(ChildListProperty, ROGridUserInfoList.NewROGridUserInfoList)
        End If
        Return GetProperty(ChildListProperty)
      End Get
    End Property

#End Region

#Region "  Methods  "

    Friend Sub ClearLayout()
      LoadProperty(LayoutInfoProperty, "")
    End Sub

    Public Shared Function CreateDefaultLayout(LayoutName As String, LayoutInfo As String) As ROGridUserInfo
      Dim ui As New ROGridUserInfo
      ui.LoadProperty(LayoutNameProperty, LayoutName)
      ui.LoadProperty(LayoutInfoProperty, LayoutInfo)
      ui.Saved = False
      Return ui
    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(WebGridUserInfoIDProperty)

    End Function

    Public Overrides Function ToString() As String

      Return Me.LayoutName

    End Function

#End Region

#End Region

#Region "  Data Access & Factory Methods  "

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Friend Shared Function GetROGridUserInfo(dr As SafeDataReader) As ROGridUserInfo

      Dim r As New ROGridUserInfo()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      With sdr
        LoadProperty(WebGridUserInfoIDProperty, .GetInt32(0))
        LoadProperty(WebGridInfoIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
        LoadProperty(UserIDProperty, .GetInt32(2))
        LoadProperty(LayoutInfoProperty, .GetString(3))
        LoadProperty(LayoutNameProperty, .GetString(4))
      End With

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace