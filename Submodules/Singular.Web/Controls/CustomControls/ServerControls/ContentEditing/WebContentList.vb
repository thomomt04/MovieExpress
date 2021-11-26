Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace ContentEditing

  Public Module ContentEditing

    Friend LockObject As New Object

    Public Enum StorageModeType
      File
      Database
    End Enum

    Public Property StorageMode As StorageModeType = StorageModeType.File

    ''' <summary>
    ''' Directory where all the content files are located. If using File StorageMode.
    ''' </summary>
    Public Property FileStorageDirectory As String = "~\ContentFiles\"

    ''' <summary>
    ''' Number of minutes before the content is re fetched from the database. If using Database StorageMode
    ''' </summary>
    Public Property ContentExpires As Integer = 60

    ''' <summary>
    ''' The role the user must have to be able to edit web content.
    ''' </summary>
    Public Property DefaultEditRole As String = "WebContent.Edit"

    Public Property DesignModeCSS As String = "~/Styles/Site.css"

    Private mLastContentFetchTime As Date = Date.MinValue
    Private mWebContentList As WebContentList = WebContentList.NewWebContentList

    Public Function GetContent(ContentName As String) As WebContent

      If StorageMode = StorageModeType.Database Then

        SyncLock (LockObject)

          If DateDiff(DateInterval.Minute, mLastContentFetchTime, Now) > ContentExpires Then
            mWebContentList = WebContentList.GetWebContentList
            ResetExpiry()
          End If
        End SyncLock

      End If

      Return mWebContentList.GetItemSafe(ContentName)

    End Function

    Friend Sub ResetExpiry()
      mLastContentFetchTime = Now
    End Sub

    Friend KeyList As New Hashtable

    Public Sub AddKey(Key As String, Value As String)
      KeyList(Key) = Value
    End Sub

    Public ProcessContent As Func(Of String, String)

  End Module

  <Serializable()> _
  Public Class WebContentList
    Inherits SingularBusinessListBase(Of WebContentList, WebContent)

#Region " Business Methods "

    Public Function GetItem(WebContentID As Integer) As WebContent

      For Each child As WebContent In Me
        If child.WebContentID = WebContentID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Private mContentIndex As New Hashtable
    Public Function GetItemSafe(Name As String) As WebContent

      SyncLock (LockObject)
        Dim wc As WebContent = mContentIndex(Name)

        If wc Is Nothing Then
          wc = New WebContent
          wc.Name = Name
          Me.Add(wc)
          mContentIndex(Name) = wc
        End If

        Return wc
      End SyncLock

    End Function

    Public Overrides Function ToString() As String

      Return "Web Contents"

    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewWebContentList() As WebContentList

      Return New WebContentList()

    End Function

    Public Shared Sub BeginGetWebContentList(ByVal CallBack As EventHandler(Of DataPortalResult(Of WebContentList)))

      Dim dp As New DataPortal(Of WebContentList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria)

    End Sub

    Public Sub New()

      ' must have parameterless constructor

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetWebContentList() As WebContentList

      Return DataPortal.Fetch(Of WebContentList)(New Criteria)

    End Function

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Dim wc = WebContent.GetWebContent(sdr)
        Me.Add(wc)
        mContentIndex(wc.Name) = wc
      End While
      Me.RaiseListChangedEvents = True

      If sdr.NextResult Then

        Dim Parent As WebContent = Nothing
        While sdr.Read
          If Parent Is Nothing OrElse Parent.WebContentID <> sdr.GetInt32(1) Then
            Parent = GetItem(sdr.GetInt32(1))
          End If
          Parent.ContentLanguages.Add(sdr.GetInt32(2), sdr.GetString(3))

        End While

      End If

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getWebContentList"
            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As WebContent In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As WebContent In Me
          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

    Protected Overrides Sub DataPortal_Update()

      UpdateTransactional(AddressOf Update)

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace