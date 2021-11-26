Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CSLALib

  <Serializable()> _
  Public Class HomeScreenList
    Inherits SingularBusinessListBase(Of HomeScreenList, HomeScreen)

#Region " Business Methods "

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewHomeScreenList() As HomeScreenList

      Return New HomeScreenList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function GetHomeScreenList() As HomeScreenList

      Dim hsl As New HomeScreenList

      Dim i = TryCast(Singular.Security.CurrentPrincipal.Identity, Singular.Security.Identity)
      If i IsNot Nothing Then
        hsl.Add(New HomeScreen() With {.UserName = i.UserNameReadable,
                                       .UserID = i.UserID})
      End If
      Return hsl

    End Function

    Public Shared Sub BeginGetHomeScreenList(ByVal CallBack As EventHandler(Of DataPortalResult(Of HomeScreenList)))

      Dim dp As New DataPortal(Of HomeScreenList)
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

    Public Shared Function GetHomeScreenList() As HomeScreenList

      Return DataPortal.Fetch(Of HomeScreenList)(New Criteria)

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(HomeScreen.GetHomeScreen(sdr))
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
            cm.CommandText = "GetProcs.getHomeScreenList"
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

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As HomeScreen In Me
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