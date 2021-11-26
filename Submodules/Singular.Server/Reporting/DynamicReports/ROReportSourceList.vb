' Generated 23 Dec 2014 09:05 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace Reporting.Dynamic

  <Serializable()> _
  Public Class ROReportSourceList
    Inherits SingularReadOnlyListBase(Of ROReportSourceList, ROReportSource)

#Region "  Business Methods  "

    Public Function GetItem(SourceType As String) As ROReportSource

      For Each child As ROReportSource In Me
        If child.SourceType = SourceType Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "s"

    End Function

#End Region

#Region "  Data Access  "

    <Serializable(), Singular.Web.WebFetchable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      <Singular.DataAnnotations.PrimarySearchField>
      Public Property Name As String

      Public Sub New()


      End Sub

    End Class

#Region "  Common  "

    Public Shared Function NewROReportSourceList() As ROReportSourceList

      Return New ROReportSourceList()

    End Function

    Public Shared Sub BeginGetROReportSourceList(criteria As Criteria, CallBack As EventHandler(Of DataPortalResult(Of ROReportSourceList)))

      Dim dp As New DataPortal(Of ROReportSourceList)()
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(criteria)

    End Sub


    Public Shared Sub BeginGetROReportSourceList(CallBack As EventHandler(Of DataPortalResult(Of ROReportSourceList)))

      Dim dp As New DataPortal(Of ROReportSourceList)()
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

    Public Shared Function GetROReportSourceList() As ROReportSourceList

      Return DataPortal.Fetch(Of ROReportSourceList)(New Criteria())

    End Function

    Private Sub Fetch(sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Me.Add(ROReportSource.GetROReportSource(sdr))
      End While
      Me.IsReadOnly = True
      Me.RaiseListChangedEvents = True

    End Sub

    Protected Overrides Sub DataPortal_Fetch(criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Singular.Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getDynamicReportSourceList"
            cm.Parameters.AddWithValue("@Name", Singular.Misc.IsNull(crit.Name, ""))

            Singular.Data.Sql.AddTableParameter(cm, "@AllowedSchemas", Settings.DynamicReportsAllowedSchemas, "Value")

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