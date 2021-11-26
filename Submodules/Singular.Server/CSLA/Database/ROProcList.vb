Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CSLALib.Database

  <Serializable()> _
  Public Class ROProcList
    Inherits SingularReadOnlyListBase(Of ROProcList, ROProc)

#Region " Business Methods "

    Private Function GetItem(Schema As String, ProcName As String) As ROProc

      For Each child As ROProc In Me
        If child.Schema = Schema AndAlso child.ProcName = ProcName Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "s"

    End Function

    Public Function GetROProcParameter(ByVal Schema As String) As ROProcParameter

      Dim obj As ROProcParameter = Nothing
      For Each parent As ROProc In Me
        obj = parent.ROProcParameterList.GetItem(Schema)
        If obj IsNot Nothing Then
          Return obj
        End If
      Next
      Return Nothing

    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared SchemaNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SchemaName, "Entity Name")

      <Display(AutoGenerateField:=False)> _
      Public Property SchemaName() As String
        Get
          Return ReadProperty(SchemaNameProperty)
        End Get
        Set(ByVal value As String)
          LoadProperty(SchemaNameProperty, value)
        End Set
      End Property

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewROProcList() As ROProcList

      Return New ROProcList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Sub BeginGetROProcList(SchemaName As String, ByVal CallBack As EventHandler(Of DataPortalResult(Of ROProcList)))

      Dim dp As New DataPortal(Of ROProcList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria With {.SchemaName = SchemaName})

    End Sub

    Public Sub New()

      ' require use of MobileFormatter

    End Sub




#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetROProcList() As ROProcList

      Return DataPortal.Fetch(Of ROProcList)(New Criteria)

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Me.Add(ROProc.GetROProc(sdr))
      End While
      Me.IsReadOnly = True
      Me.RaiseListChangedEvents = True

      If sdr.NextResult Then

        Dim parent As ROProc = Nothing

        While sdr.Read
          If parent Is Nothing OrElse parent.ProcName <> sdr.GetString(1) OrElse parent.Schema <> sdr.GetString(0) Then
            parent = Me.GetItem(sdr.GetString(0), sdr.GetString(1))
          End If

          parent.ROProcParameterList.RaiseListChangedEvents = False
          parent.ROProcParameterList.Add(ROProcParameter.GetROProcParameter(sdr))
          parent.ROProcParameterList.RaiseListChangedEvents = True
        End While
      End If

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.Text
            cm.CommandText = My.Resources.GetStoredProcedures

            cm.Parameters.AddWithValue("@SchemaName", crit.SchemaName)

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
