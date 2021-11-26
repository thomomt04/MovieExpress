Imports Csla
Imports Csla.Data
Imports System.ComponentModel
Imports Singular.DataAnnotations

Namespace AuditTrails

  Public Interface IAuditTrailHeader
    ReadOnly Property ROAuditTrailTableList() As ROAuditTrailTableList
  End Interface

  <Serializable()> _
  Public Class ROAuditTrailHeaderList
    Inherits SingularReadOnlyListBase(Of ROAuditTrailHeaderList, ROAuditTrailHeader)

  End Class

  <Serializable()> _
  Public Class ROAuditTrailHeader
    Inherits SingularReadOnlyBase(Of ROAuditTrailHeader)
    Implements IAuditTrailHeader

#Region " Class Member Variables "

    Private mAuditTrailTableID As Integer
    Private mRowID As Integer
    Private mDescription As String = ""
    Private mParentName As String = ""
    Private mParentID As Object

    Private mLastAuditTrailType As Integer
    Private mLastChangeDateTime As DateTime
    Private mLastAuditTrailUserID As Integer

    'Private mStartDate As Object
    'Private mEndDate As Object
    'Private mCritUserID As Object

    Private mROAuditTrailTableList As ROAuditTrailTableList
    Private mROAuditTrailList As New AuditTrails.ROAuditTrailList

#End Region

#Region " Business Methods "

#Region " Properties "

    Public ReadOnly Property AuditTrailTableID() As String
      Get
        Return mAuditTrailTableID
      End Get
    End Property

    Public ReadOnly Property RowID() As Integer
      Get
        Return mRowID
      End Get
    End Property

    Public ReadOnly Property Description() As String
      Get
        Return mDescription
      End Get
    End Property

    Public ReadOnly Property ParentName() As String
      Get
        Return mParentName
      End Get
    End Property

    Public ReadOnly Property ParentID() As Integer?
      Get
        Return mParentID
      End Get
    End Property

    <ClientOnlyNoData, DateField(FormatString:="dd MMM yyyy HH:mm")>
    Public Property LastChangeDate() As Date

    <ClientOnlyNoData>
    Public Property LastChangedBy As String


    Public ReadOnly Property ROAuditTrailTableList() As ROAuditTrailTableList Implements IAuditTrailHeader.ROAuditTrailTableList
      Get
        Return mROAuditTrailTableList
      End Get
    End Property

    Public ReadOnly Property ROAuditTrailList() As ROAuditTrailList
      Get
        Return mROAuditTrailList
      End Get
    End Property

    <ClientOnlyNoData>
    Public Property IsLoading As Boolean

    Public Property IsExpanded As Boolean

    Public Property FetchedChildren As Boolean

    Public Type As Integer

#End Region

#Region " Methods "

    Public Shared Property ViewAuditTrailRole As String = "Audit Trails.Access"

    <Singular.Web.WebCallable>
    Public Shared Function FetchChildren(ParentTableID As Integer, ParentRowID As Integer,
                                         Optional ByVal Levels As Integer = 1,
                                         Optional ConnectionString As String = Nothing) As Tuple(Of ROAuditTrailTableList, ROAuditTrailUserList)

      'If ViewAuditTrailRole = "" OrElse Singular.Security.HasAccess(ViewAuditTrailRole) Then

      Dim ROAuditTrailTableList = AuditTrails.ROAuditTrailTableList.GetROAuditTrailTableList(Nothing, Nothing, ParentTableID, ParentRowID, Nothing, Nothing, Nothing, ConnectionString)
      Return Tuple.Create(ROAuditTrailTableList, ROAuditTrailTableList.AuditTrailUserList)

      'Else
      'Throw New UnauthorizedAccessException("Access denied")
      'End If

    End Function

    Public Sub FetchChildren(HeaderList As ROAuditTrailTableList, Optional ByVal Levels As Integer = 1, Optional ConnectionString As String = Nothing)

      With FetchChildren(mAuditTrailTableID, mRowID, Levels, ConnectionString)
        FetchedChildren = True
        mROAuditTrailTableList = .Item1
        HeaderList.AuditTrailUserList.Merge(.Item2)
      End With

      If Levels > 1 Then
        ROAuditTrailTableList.FetchChildren(HeaderList, Levels - 1, ConnectionString)
      End If

    End Sub

    Protected Overrides Function GetIdValue() As Object

      Return Me.mAuditTrailTableID

    End Function

#End Region

#End Region

#Region " Factory Methods "

    Friend Shared Function NewROAuditTrailHeader() As ROAuditTrailHeader

      Return New ROAuditTrailHeader

    End Function

    Friend Shared Function GetROAuditTrailHeader(ByVal dr As SafeDataReader) As ROAuditTrailHeader

      Return New ROAuditTrailHeader(dr)

    End Function

    Private Sub New()


    End Sub

    Private Sub New(ByVal dr As SafeDataReader)

      Fetch(dr)

    End Sub

#End Region

#Region " Data Access "

    Protected Sub Fetch(ByRef dr As SafeDataReader)

      With dr
        mAuditTrailTableID = .GetInt32(0)
        mRowID = .GetInt32(1)
        mDescription = .GetString(2)
        mParentName = .GetString(3)
        mParentID = .GetInt32(4)
      End With

    End Sub

#End Region

  End Class


End Namespace