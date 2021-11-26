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
Public Class ServerProgramProgressDetail
  Inherits SingularBusinessBase(Of ServerProgramProgressDetail)

#Region " Properties and Methods "

#Region " Properties "

  Public Shared ServerProgramProgressDetailIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ServerProgramProgressDetailID, "Server Program Progress Detail", 0)
  ''' <Summary>
  ''' Gets the Server Program Progress Detail value
  ''' </Summary>
  <Display(AutoGenerateField:=False, Name:="Server Program Progress Detail", Description:="")> _
  Public ReadOnly Property ServerProgramProgressDetailID() As Integer
    Get
      Return GetProperty(ServerProgramProgressDetailIDProperty)
    End Get
  End Property

  Public Shared ServerProgramProgressIDProperty As PropertyInfo(Of Nullable(Of Integer)) = RegisterProperty(Of Nullable(Of Integer))(Function(c) c.ServerProgramProgressID, "Server Program Progress", CType(Nothing, Nullable(Of Integer)))
  ''' <Summary>
  ''' Gets the Server Program Progress value
  ''' </Summary>
  <Display(Name:="Server Program Progress", Description:="")> _
  Public ReadOnly Property ServerProgramProgressID() As Nullable(Of Integer)
    Get
      Return GetProperty(ServerProgramProgressIDProperty)
    End Get
  End Property

  Public Shared ProgressDetailProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ProgressDetail, "Progress Detail", "")
  ''' <Summary>
  ''' Gets and sets the Progress Detail value
  ''' </Summary>
  <Display(Name:="Progress Detail", Description:=""),
  StringLength(1000, ErrorMessage:="Progress Detail cannot be more than 1000 characters")> _
  Public Property ProgressDetail() As String
    Get
      Return GetProperty(ProgressDetailProperty)
    End Get
    Set(ByVal Value As String)
      SetProperty(ProgressDetailProperty, Value)
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

#Region " Methods "

  Public Function GetParent() As ServerProgramProgress

    Return CType(CType(Me.Parent, ServerProgramProgressDetailList).Parent, ServerProgramProgress)

  End Function

  Protected Overrides Function GetIdValue() As Object

    Return GetProperty(ServerProgramProgressDetailIDProperty)

  End Function

  Public Overrides Function ToString() As String

    If Me.ProgressDetail.Length = 0 Then
      If Me.IsNew Then
        Return "New Server Program Progress Detail"
      Else
        Return "Blank Server Program Progress Detail"
      End If
    Else
      Return Me.ProgressDetail
    End If

  End Function

#End Region

#End Region

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()

    MyBase.AddBusinessRules()

  End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

  Friend Shared Function NewServerProgramProgressDetail(ByVal Parent As ServerProgramProgress, ByVal ProgressDetail As String, ByVal ProgressTypeID As Networking.NetworkServiceBase.ProgressType) As ServerProgramProgressDetail

    Dim sppd = DataPortal.CreateChild(Of ServerProgramProgressDetail)()
    sppd.ProgressDetail = ProgressDetail
    sppd.ProgressTypeID = ProgressTypeID
    Return sppd

  End Function

  Public Shared Function NewServerProgramProgressDetail() As ServerProgramProgressDetail

    Return DataPortal.CreateChild(Of ServerProgramProgressDetail)()

  End Function

  Public Sub New()

    MarkAsChild()

  End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

  Friend Shared Function GetServerProgramProgressDetail(ByVal dr As SafeDataReader) As ServerProgramProgressDetail

    Dim s As New ServerProgramProgressDetail()
    s.Fetch(dr)
    Return s

  End Function

  Protected Sub Fetch(ByRef sdr As SafeDataReader)

    Using BypassPropertyChecks
      With sdr
        LoadProperty(ServerProgramProgressDetailIDProperty, .GetInt32(0))
        LoadProperty(ServerProgramProgressIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
        LoadProperty(ProgressDetailProperty, .GetString(2))
        LoadProperty(CreatedDateProperty, .GetSmartDate(3))
        LoadProperty(ProgressTypeIDProperty, .GetInt32(4))
      End With
    End Using

    MarkAsChild()
    MarkOld()
    BusinessRules.CheckRules()

  End Sub

  Friend Sub Insert()

    ' if we're not dirty then don't update the database
    Using cm As SqlCommand = New SqlCommand
      cm.CommandText = "InsProcs.insServerProgramProgressDetail"

      DoInsertUpdateChild(cm)

    End Using

  End Sub

  Friend Sub Update()

    ' if we're not dirty then don't update the database
    Using cm As SqlCommand = New SqlCommand
      cm.CommandText = "UpdProcs.updServerProgramProgressDetail"

      DoInsertUpdateChild(cm)

    End Using

  End Sub

  Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

    If Me.IsSelfDirty Then

      With cm
        .CommandType = CommandType.StoredProcedure

        Dim paramServerProgramProgressDetailID As SqlParameter = .Parameters.Add("@ServerProgramProgressDetailID", SqlDbType.Int)
        paramServerProgramProgressDetailID.Value = GetProperty(ServerProgramProgressDetailIDProperty)
        If Me.IsNew Then
          paramServerProgramProgressDetailID.Direction = ParameterDirection.Output
        End If
        .Parameters.AddWithValue("@ServerProgramProgressID", Me.GetParent.ServerProgramProgressID)
        .Parameters.AddWithValue("@ProgressDetail", GetProperty(ProgressDetailProperty))
        .Parameters.AddWithValue("@ProgressTypeID", GetProperty(ProgressTypeIDProperty))

        .ExecuteNonQuery()

        If Me.IsNew() Then
          LoadProperty(ServerProgramProgressDetailIDProperty, paramServerProgramProgressDetailID.Value)
        End If
        ' update child objects
        ' mChildList.Update()
        MarkOld()
      End With
    Else
      ' update child objects
      ' mChildList.Update()
    End If

  End Sub

  Friend Sub DeleteSelf()

    ' if we're not dirty then don't update the database
    If Me.IsNew Then Exit Sub

    Using cm As SqlCommand = New SqlCommand
      cm.CommandText = "DelProcs.delServerProgramProgressDetail"
      cm.CommandType = CommandType.StoredProcedure
      cm.Parameters.AddWithValue("@ServerProgramProgressDetailID", GetProperty(ServerProgramProgressDetailIDProperty))
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
