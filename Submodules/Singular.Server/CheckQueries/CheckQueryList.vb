Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CheckQueries

  <Serializable()> _
  Public Class CheckQueryList
    Inherits SingularBusinessListBase(Of CheckQueryList, CheckQuery)

#Region " Business Methods "

    Public Function GetItem(Schema As String) As CheckQuery

      For Each child As CheckQuery In Me
        If child.Schema = Schema Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "s"

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

    Public Shared Function NewCheckQueriesList() As CheckQueryList

      Return New CheckQueryList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Sub BeginGetCheckQueriesList(ByVal CallBack As EventHandler(Of DataPortalResult(Of CheckQueryList)))

      Dim dp As New DataPortal(Of CheckQueryList)
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

    Public Shared Function GetCheckQueriesList() As CheckQueryList

      Return DataPortal.Fetch(Of CheckQueryList)(New Criteria)

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(CheckQuery.GetCheckQueries(sdr))
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
            cm.CommandText = "GetProcs.getCheckQueriesList"
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