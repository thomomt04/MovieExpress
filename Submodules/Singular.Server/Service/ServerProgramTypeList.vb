Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Service

  <Serializable()> _
  Public Class ServerProgramTypeList
    Inherits SingularBusinessListBase(Of ServerProgramTypeList, ServerProgramType)

#Region " Business Methods "

    Public Function GetItem(ServerProgramTypeID As Integer) As ServerProgramType

      For Each child As ServerProgramType In Me
        If child.ServerProgramTypeID = ServerProgramTypeID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Server Program Types"

    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared ServiceProgramTypeIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.ServiceProgramTypeID)

      Public Property ServiceProgramTypeID() As Integer?
        Get
          Return ReadProperty(ServiceProgramTypeIDProperty)
        End Get
        Set(value As Integer?)
          LoadProperty(ServiceProgramTypeIDProperty, value)
        End Set
      End Property

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewServerProgramTypeList() As ServerProgramTypeList

      Return New ServerProgramTypeList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Sub BeginGetServerProgramTypeList(ByVal CallBack As EventHandler(Of DataPortalResult(Of ServerProgramTypeList)))

      Dim dp As New DataPortal(Of ServerProgramTypeList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria)

    End Sub

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetServerProgramTypeList(Criteria As Criteria) As ServerProgramTypeList

      Return DataPortal.Fetch(Of ServerProgramTypeList)(Criteria)

    End Function

    Public Shared Function GetServerProgramType(ServerProgramID As Integer) As ServerProgramType

      Return DataPortal.Fetch(Of ServerProgramTypeList)(New Criteria With {.ServiceProgramTypeID = ServerProgramID}).FirstAndOnly

    End Function

    Public Shared Function GetServerProgramTypeList() As ServerProgramTypeList

      Return DataPortal.Fetch(Of ServerProgramTypeList)(New Criteria)

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(ServerProgramType.GetServerProgramType(sdr))
      End While
      Me.RaiseListChangedEvents = True

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getServerProgramTypeList"

            If crit.ServiceProgramTypeID IsNot Nothing Then
              cm.Parameters.AddWithValue("@ServiceProgramTypeID", crit.ServiceProgramTypeID)
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

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As ServerProgramType In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()


        Dim DirtyIDs As New List(Of String)

        For Each Child As ServerProgramType In Me
          If Child.IsDirty Then
            DirtyIDs.Add(Child.ServerProgramTypeID)
          End If

          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next

        If DirtyIDs.Count > 0 Then
          NotifyService(ServiceUpdateMessageType.ServiceInfoUpdated, String.Join(",", DirtyIDs.ToArray()))
        End If

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
