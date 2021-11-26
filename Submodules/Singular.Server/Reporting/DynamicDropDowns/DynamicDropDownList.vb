Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace Reporting.Dynamic

  Public Class DynamicDropDownList
    Inherits SingularReadOnlyListBase(Of DynamicDropDownList, DynamicDropDown)

    ''' <summary>
    ''' Sets the database schema that contains dynamic drop down stored procs.
    ''' </summary>
    Public Property DatabaseSchema As String = ""

    ''' <summary>
    ''' Adds dropdown info for use with dynamic reports.
    ''' </summary>
    ''' <param name="Name">The name to appear in the dynamic reports setup screen.</param>
    ''' <param name="DropDownInfo">The information about the drop down. Specify as you would on a drop down property.</param>
    ''' <param name="DefaultParameterName">The parameter name that usually requires this drop down. E.g. BranchID will always require a Branch drop down.</param>
    Public Sub AddDropDown(Name As String, DropDownInfo As DataAnnotations.DropDownWeb, Optional DefaultParameterName As String = "")

      Dim ddd As New DynamicDropDown(Name, DropDownInfo) With {.DefaultParameterName = DefaultParameterName.Replace("@", "")}
      Add(ddd)

    End Sub

    Public Function FetchDatabaseDropDowns() As DynamicDropDownList

      If DatabaseSchema = "" Then
        Return Me
      Else
        Dim NewList = Csla.DataPortal.Fetch(Of DynamicDropDownList)(New Criteria With {.DatabaseSchema = DatabaseSchema})
        For Each ddd As DynamicDropDown In Me
          NewList.Add(ddd)
        Next
        Return NewList
      End If

    End Function

    Private mDatabaseDropDowns As DynamicDropDownList

    'Cached version of FetchDatabaseDropDowns()
    Public Function IncludeDatabaseDropDowns() As DynamicDropDownList

      If DatabaseSchema = "" Then
        Return Me
      Else
        If mDatabaseDropDowns Is Nothing Then
          mDatabaseDropDowns = Csla.DataPortal.Fetch(Of DynamicDropDownList)(New Criteria With {.DatabaseSchema = DatabaseSchema})
          For Each ddd As DynamicDropDown In Me
            mDatabaseDropDowns.Add(ddd)
          Next
        End If
        Return mDatabaseDropDowns
      End If

    End Function

    Public Sub ResetDatabaseDropDowns()
      mDatabaseDropDowns = Nothing
    End Sub

    Friend Function GetDefault(ParameterName As String) As DynamicDropDown
      For Each ddd As DynamicDropDown In Me
        If ddd.DefaultParameterName <> "" AndAlso ddd.DefaultParameterName = ParameterName Then
          Return ddd
        End If
      Next
      Return Nothing
    End Function

    Public Function GetItem(Name As String) As DynamicDropDown
      For Each ddd As DynamicDropDown In Me
        If ddd.Name = Name Then
          Return ddd
        End If
      Next
      Return Nothing
    End Function

    Public Class Criteria
      Inherits SingularCriteriaBase(Of Criteria)

      Public Property DatabaseSchema As String
    End Class

    Protected Overrides Sub DataPortal_Fetch(criteria As Object)

      Using cn As New SqlConnection(Singular.Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getDynamicDropDownList"
            cm.Parameters.AddWithValue("@DropDownSchema", CType(criteria, Criteria).DatabaseSchema)
            Using sdr As New SafeDataReader(cm.ExecuteReader)

              RaiseListChangedEvents = False
              IsReadOnly = False

              While sdr.Read

                Add(New DynamicDropDown(sdr.GetString(0), sdr.GetString(1), sdr.GetBoolean(2)))

              End While

            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using


    End Sub

  End Class

  Public Class DynamicDropDown
    Inherits SingularReadOnlyBase(Of DynamicDropDown)

    Private mName As String
    Private mStoredProcName As String
    Private mDropDownInfo As DataAnnotations.DropDownWeb
    Private mHasUserID As Boolean

    Public ReadOnly Property Name As String
      Get
        Return mName
      End Get
    End Property

    <System.ComponentModel.Browsable(False)>
    Public ReadOnly Property StoredProcName As String
      Get
        Return mStoredProcName
      End Get
    End Property

    Public Class DynamicInfo
      Public Property DropDownInfo As Singular.DataAnnotations.DropDownWeb
      Public Property Data As DataTable
    End Class

    Public Function GetDynamicInfo() As DynamicInfo

      If mDropDownInfo IsNot Nothing Then

        Return New DynamicInfo With {.DropDownInfo = mDropDownInfo}

      Else
        'This is a drop down derived from a stored proc. We need to exec the stored proc to get the columns.

        Dim ddi As New DataAnnotations.DropDownWeb("ClientData.Dynamic_" & mName)

        Dim cProc As New Singular.CommandProc(mStoredProcName)
        If Settings.DynamicReportDatabaseConnectionString <> "" Then
          cProc.ConnectionString = Settings.DynamicReportDatabaseConnectionString
        End If
        If mHasUserID Then
          cProc.Parameters.AddWithValue("@UserID", Singular.Settings.CurrentUserID)
        End If
        cProc.FetchType = CommandProc.FetchTypes.DataSet
        cProc = cProc.Execute()

        Dim tbl = cProc.Dataset.Tables(0)
       
        With ddi
          .Source = DataAnnotations.DropDownWeb.SourceType.None

          .ValueMember = tbl.Columns(0).ColumnName
          If tbl.Columns.Count = 1 Then
            .DisplayMember = .ValueMember
          Else
            .DisplayMember = tbl.Columns(1).ColumnName
            If tbl.Columns.Count > 2 Then
              Dim Cols(tbl.Columns.Count - 2) As String
              For i As Integer = 0 To Cols.Length - 1
                Cols(i) = tbl.Columns(i + 1).ColumnName
              Next
              .DropDownColumns = Cols
            End If
           
          End If

        End With

        Return New DynamicInfo With {.DropDownInfo = ddi, .Data = tbl}

      End If

    End Function

    <System.ComponentModel.Browsable(False)>
    Public Property DefaultParameterName As String = ""

    Public Sub New(Name As String, DropDownInfo As DataAnnotations.DropDownWeb)
      mName = Name
      mDropDownInfo = DropDownInfo
    End Sub

    Public Sub New(Name As String, StoredProcName As String, HasUserID As Integer)
      mName = Name
      mStoredProcName = StoredProcName
      mHasUserID = HasUserID
    End Sub

  End Class

End Namespace



