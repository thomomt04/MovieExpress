Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

<Serializable()> _
Public Class ServerProgramProgress
  Inherits SingularBusinessBase(Of ServerProgramProgress)

#Region " Properties and Methods "

#Region " Properties "

  Public Shared ServerProgramProgressIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ServerProgramProgressID, "Server Program Progress", 0)
  ''' <Summary>
  ''' Gets the Server Program Progress value
  ''' </Summary>
  <Display(AutoGenerateField:=False, Name:="Server Program Progress", Description:="")> _
  Public ReadOnly Property ServerProgramProgressID() As Integer
    Get
      Return GetProperty(ServerProgramProgressIDProperty)
    End Get
  End Property

  Public Shared ServerProgramTypeIDProperty As PropertyInfo(Of Nullable(Of Integer)) = RegisterProperty(Of Nullable(Of Integer))(Function(c) c.ServerProgramTypeID, "Server Program Type", CType(Nothing, Nullable(Of Integer)))
  ''' <Summary>
  ''' Gets and sets the Server Program Type value
  ''' </Summary>
  <Display(Name:="Server Program Type", Description:=""),
  Required(ErrorMessage:="Server Program Type required")> _
  Public Property ServerProgramTypeID() As Nullable(Of Integer)
    Get
      Return GetProperty(ServerProgramTypeIDProperty)
    End Get
    Set(ByVal Value As Nullable(Of Integer))
      SetProperty(ServerProgramTypeIDProperty, Value)
    End Set
  End Property

  Public Shared ProgressProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Progress, "Progress", "")
  ''' <Summary>
  ''' Gets and sets the Progress value
  ''' </Summary>
  <Display(Name:="Progress", Description:=""),
  StringLength(512, ErrorMessage:="Progress cannot be more than 512 characters")> _
  Public Property Progress() As String
    Get
      Return GetProperty(ProgressProperty)
    End Get
    Set(ByVal Value As String)
      SetProperty(ProgressProperty, Value)
    End Set
  End Property

  Public Shared CreatedDateProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.CreatedDate, "Created Date", New SmartDate(Now()))
  ''' <Summary>
  ''' Gets the Created Date value
  ''' </Summary>
  <Display(AutoGenerateField:=False, Name:="Created Date", Description:="")> _
  Public ReadOnly Property CreatedDate() As SmartDate
    Get
      Return GetProperty(CreatedDateProperty)
    End Get
  End Property

  Public Shared VersionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Version, "Version", "")
  ''' <Summary>
  ''' Gets and sets the Version value
  ''' </Summary>
  <Display(Name:="Version", Description:=""),
  StringLength(50, ErrorMessage:="Version cannot be more than 50 characters")> _
  Public Property Version() As String
    Get
      Return GetProperty(VersionProperty)
    End Get
    Set(ByVal Value As String)
      SetProperty(VersionProperty, Value)
    End Set
  End Property

  Public Shared ProgressTypeIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ProgressTypeID, "Progress Type", 0)
  ''' <Summary>
  ''' Gets and sets the Progress Type value
  ''' </Summary>
  <Display(Name:="Progress Type", Description:=""),
  Required(ErrorMessage:="Progress Type required")> _
  Public Property ProgressTypeID() As Integer
    Get
      Return GetProperty(ProgressTypeIDProperty)
    End Get
    Set(ByVal Value As Integer)
      SetProperty(ProgressTypeIDProperty, Value)
    End Set
  End Property

#End Region

#Region " Child Lists "

  Public Shared ServerProgramProgressDetailListProperty As PropertyInfo(Of ServerProgramProgressDetailList) = RegisterProperty(Of ServerProgramProgressDetailList)(Function(c) c.ServerProgramProgressDetailList, "Server Program Progress Detail List")

  <Display(AutoGenerateField:=False)> _
  Public ReadOnly Property ServerProgramProgressDetailList() As ServerProgramProgressDetailList
    Get
      If Not FieldManager.FieldExists(ServerProgramProgressDetailListProperty) Then
        LoadProperty(ServerProgramProgressDetailListProperty, ServerProgramProgressDetailList.NewServerProgramProgressDetailList())
      End If
      Return GetProperty(ServerProgramProgressDetailListProperty)
    End Get
  End Property

  Public Sub AddProgressDetail(ByVal ProgressDetail As String, ByVal ProgressTypeID As Networking.NetworkServiceBase.ProgressType)

    Me.ServerProgramProgressDetailList.Add(ServerProgramProgressDetail.NewServerProgramProgressDetail(Me, ProgressDetail, ProgressTypeID))

  End Sub

#End Region

#Region " Methods "

  Protected Overrides Function GetIdValue() As Object

    Return GetProperty(ServerProgramProgressIDProperty)

  End Function

  Public Overrides Function ToString() As String

    If Me.Progress.Length = 0 Then
      If Me.IsNew Then
        Return "New Server Program Progress"
      Else
        Return "Blank Server Program Progress"
      End If
    Else
      Return Me.Progress
    End If

  End Function

  Protected Overrides ReadOnly Property TableReferencesToIgnore() As String()
    Get
      Return New String() {"ServerProgramProgressDetails"}
    End Get
  End Property

#End Region

#End Region

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()

    MyBase.AddBusinessRules()

  End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

  Public Shared Function NewServerProgramProgress() As ServerProgramProgress

    Return DataPortal.CreateChild(Of ServerProgramProgress)()

  End Function

  Public Sub New()

    '  MarkAsChild()

  End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

  Public Shared Function NewServerProgramProgress(ByVal ServerProgramTypeID As Integer, ByVal Progress As String, ByVal Version As String, ByVal ProgressTypeID As Networking.NetworkServiceBase.ProgressType) As ServerProgramProgress

    Dim spp As New ServerProgramProgress()
    spp.ServerProgramTypeID = ServerProgramTypeID
    spp.Progress = Singular.Strings.Left(Progress, 512)
    spp.Version = Version
    spp.ProgressTypeID = ProgressTypeID
    Return spp

  End Function

  Friend Shared Function GetServerProgramProgress(ByVal dr As SafeDataReader) As ServerProgramProgress

    Dim s As New ServerProgramProgress()
    s.Fetch(dr)
    Return s

  End Function

  Protected Sub Fetch(ByRef sdr As SafeDataReader)

    Using BypassPropertyChecks
      With sdr
        LoadProperty(ServerProgramProgressIDProperty, .GetInt32(0))
        LoadProperty(ServerProgramTypeIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
        LoadProperty(ProgressProperty, .GetString(2))
        LoadProperty(CreatedDateProperty, .GetSmartDate(3))
        LoadProperty(VersionProperty, .GetString(4))
        LoadProperty(ProgressTypeIDProperty, .GetInt32(5))
      End With
    End Using

    MarkAsChild()
    MarkOld()
    BusinessRules.CheckRules()

  End Sub

  Protected Overrides Sub DataPortal_Insert()

    ' if we're not dirty then don't update the database
    Using cm As SqlCommand = New SqlCommand
      cm.CommandText = "InsProcs.insServerProgramProgress"

      DoInsertUpdateParent(cm)

    End Using

  End Sub

  Protected Overrides Sub DataPortal_Update()

    ' if we're not dirty then don't update the database
    Using cm As SqlCommand = New SqlCommand
      cm.CommandText = "UpdProcs.updServerProgramProgress"

      DoInsertUpdateParent(cm)

    End Using

  End Sub

  Friend Sub Insert()

    ' if we're not dirty then don't update the database
    Using cm As SqlCommand = New SqlCommand
      cm.CommandText = "InsProcs.insServerProgramProgress"

      DoInsertUpdateChild(cm)

    End Using

  End Sub

  Friend Sub Update()

    ' if we're not dirty then don't update the database
    Using cm As SqlCommand = New SqlCommand
      cm.CommandText = "UpdProcs.updServerProgramProgress"

      DoInsertUpdateChild(cm)

    End Using

  End Sub

  Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

    If Me.IsSelfDirty Then

      With cm
        .CommandType = CommandType.StoredProcedure

        Dim paramServerProgramProgressID As SqlParameter = .Parameters.Add("@ServerProgramProgressID", SqlDbType.Int)
        paramServerProgramProgressID.Value = GetProperty(ServerProgramProgressIDProperty)
        If Me.IsNew Then
          paramServerProgramProgressID.Direction = ParameterDirection.Output
        End If
        .Parameters.AddWithValue("@ServerProgramTypeID", GetProperty(ServerProgramTypeIDProperty))
        .Parameters.AddWithValue("@Progress", GetProperty(ProgressProperty))
        .Parameters.AddWithValue("@Version", GetProperty(VersionProperty))
        .Parameters.AddWithValue("@ProgressTypeID", GetProperty(ProgressTypeIDProperty))

        .ExecuteNonQuery()

        If Me.IsNew() Then
          LoadProperty(ServerProgramProgressIDProperty, paramServerProgramProgressID.Value)
        End If
        ' update child objects
        Me.ServerProgramProgressDetailList.Update()
        MarkOld()
      End With
    Else
      ' update child objects
      Me.ServerProgramProgressDetailList.Update()
    End If

  End Sub

  Friend Sub DeleteSelf()

    ' if we're not dirty then don't update the database
    If Me.IsNew Then Exit Sub

    Using cm As SqlCommand = New SqlCommand
      cm.CommandText = "DelProcs.delServerProgramProgress"
      cm.CommandType = CommandType.StoredProcedure
      cm.Parameters.AddWithValue("@ServerProgramProgressID", GetProperty(ServerProgramProgressIDProperty))
      DoDeleteChild(cm)
    End Using

  End Sub

  Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)

    If Me.IsNew Then Exit Sub

    With cm
      .ExecuteNonQuery()
    End With
    MarkNew()

  End Sub

#End If

#End Region

#End Region


End Class
