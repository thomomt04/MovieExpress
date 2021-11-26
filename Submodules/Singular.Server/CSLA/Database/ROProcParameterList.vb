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
  Public Class ROProcParameterList
    Inherits SingularReadOnlyListBase(Of ROProcParameterList, ROProcParameter)

#Region " Parent "

    <NotUndoable()> Private mParent As ROProc
#End Region

#Region " Business Methods "

    Public Function GetItem(Schema As String) As ROProcParameter

      For Each child As ROProcParameter In Me
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

#Region " Common "

    Public Shared Function NewROProcParameterList() As ROProcParameterList

      Return New ROProcParameterList()

    End Function

    <Serializable()>
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared ParameterNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ParameterName)

      Public Property ParameterName() As String
        Get
          Return ReadProperty(ParameterNameProperty)
        End Get
        Set(value As String)
          LoadProperty(ParameterNameProperty, value)
        End Set
      End Property


      Public Shared ParameterValueProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ParameterValue)
     
      Public Property ParameterValue() As String
        Get
          Return ReadProperty(ParameterValueProperty)
        End Get
        Set(value As String)
          LoadProperty(ParameterValueProperty, value)
        End Set
      End Property

      Public Shared GetProcNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.GetProcName)
   
      Public Property GetProcName() As String
        Get
          Return ReadProperty(GetProcNameProperty)
        End Get
        Set(value As String)
          LoadProperty(GetProcNameProperty, value)
        End Set
      End Property

    End Class

    Public Shared Sub BeginFetch(GetProcedureName As String, ParameterName As String, ParameterValue As String, Handler As EventHandler(Of DataPortalResult(Of ROProcParameterList)))

      Dim dp As New DataPortal(Of ROProcParameterList)
      AddHandler dp.FetchCompleted, Handler
      dp.BeginFetch(New Criteria() With {.GetProcName = GetProcedureName, .ParameterName = ParameterName, .ParameterValue = ParameterValue})

    End Sub

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then




#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Me.Add(ROProcParameter.GetROProcParameter(sdr))
      End While
      Me.IsReadOnly = True
      Me.RaiseListChangedEvents = True

    End Sub

    Protected Overrides Sub DataPortal_Fetch(criteria As Object)

      Dim crit As Criteria = criteria
      Using cnn As New SqlConnection(Settings.ConnectionString)
        cnn.Open()
        Using cmd = cnn.CreateCommand()
          cmd.CommandText = crit.GetProcName
          cmd.CommandType = CommandType.StoredProcedure

          cmd.Parameters.AddWithValue(crit.ParameterName, crit.ParameterValue)

          Using sdr As New SafeDataReader(cmd.ExecuteReader)
            Fetch(sdr)
          End Using
        End Using
      End Using

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace
