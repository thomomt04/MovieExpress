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
  Public Class TableReferenceList
    Inherits SingularReadOnlyListBase(Of TableReferenceList, TableReference)

#Region " Business Properties Methods "

    Public ReadOnly Property HasReferences(ByVal TablesToIgnore() As String) As Boolean
      Get

        ' check if any of the references has more than 0 links
        For Each child As TableReference In Me
          Dim bIgnore As Boolean = False
          For Each sTableName As String In TablesToIgnore
            If Singular.Debug.InDebugMode AndAlso sTableName.Contains(",") Then
              Throw New Exception("TableReferencesToIgnore() is returning an invalid string. Please check this method. Refer to Webber or Marlborough")
            End If
            If child.TableName = sTableName Then
              bIgnore = True
              Exit For
            End If
          Next
          If child.ConstraintDescription.ToLower <> "ignore" AndAlso child.NoOfReferences > 0 And Not bIgnore Then
            Return True
          End If
        Next
        Return False

      End Get
    End Property

    Public Function GetReferenceListAsString(ByVal TablesToIgnore() As String, Optional ByVal NumTabsIndented As Integer = 1) As String

      Dim Refs As New List(Of String)

      '  Dim bIgnore As Boolean = False
      For Each tr As TableReference In Me.Where(Function(c) c.ConstraintDescription.Trim <> "" And c.ConstraintDescription.ToLower <> "ignore")
        If TablesToIgnore Is Nothing OrElse Not TablesToIgnore.Contains(tr.TableName) Then
          If Not Refs.Contains(tr.ToString) Then
            Refs.Add(tr.ToString)
          End If
        End If
      Next
      For Each tr As TableReference In Me.Where(Function(c) c.ConstraintDescription.Trim = "")
        If TablesToIgnore Is Nothing OrElse Not TablesToIgnore.Contains(tr.TableName) Then
          If Not Refs.Contains(tr.ToString) Then
            Refs.Add(tr.ToString)
          End If
        End If
      Next

      Dim sReturn As String = ""
      For Each ref As String In Refs
        For i As Integer = 1 To NumTabsIndented
          sReturn &= vbTab
        Next
        sReturn &= ref & "," & vbCrLf
      Next

      If sReturn.Length > 3 Then
        Return Left(sReturn, sReturn.Length - 3)
      Else
        Return sReturn
      End If

    End Function

    Public Function GetItem(ByVal TableName As String) As TableReference

      For Each child As TableReference In Me
        If child.TableName = TableName Then
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

      Public Shared TableNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.TableName, "Entity Name")

      <Display(AutoGenerateField:=False)> _
      Public Property TableName() As String
        Get
          Return ReadProperty(TableNameProperty)
        End Get
        Set(ByVal value As String)
          LoadProperty(TableNameProperty, value)
        End Set
      End Property

      Public Shared KeyValueProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.KeyValue, "Entity Name")

      <Display(AutoGenerateField:=False)> _
      Public Property KeyValue() As Integer
        Get
          Return ReadProperty(KeyValueProperty)
        End Get
        Set(ByVal value As Integer)
          LoadProperty(KeyValueProperty, value)
        End Set
      End Property

      Public Sub New(ByVal TableName As String, ByVal KeyValue As Integer)

        Me.TableName = TableName
        Me.KeyValue = KeyValue

      End Sub

      Public Sub New()

      End Sub

    End Class

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Sub BeginGetTableReferenceList(ByVal TableName As String, ByVal KeyValue As Integer, ByVal CallBack As EventHandler(Of DataPortalResult(Of TableReferenceList)))

      Dim dp As New DataPortal(Of TableReferenceList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria(TableName, KeyValue))

    End Sub

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetTableReferenceList(ByVal TableName As String, ByVal KeyValue As Integer) As TableReferenceList

      Return DataPortal.Fetch(Of TableReferenceList)(New Criteria(TableName, KeyValue))

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      Me.IsReadOnly = False
      While sdr.Read
        Me.Add(TableReference.GetTableReference(sdr))
      End While
      Me.IsReadOnly = True
      Me.RaiseListChangedEvents = True

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)

      ' This method will attempt to run the get, if that fails it will alter the 
      ' stored procedure (if that fails it will run the create), after which it will try run the get again
      Try
        RunGet(Criteria)
        ' if we get here then all cool
      Catch GetEx As Exception
                If Singular.Debug.InDebugMode Then
                    ' error, maybe need to update the SP
                    Try
                        RunCreateAter(Criteria, True)
                    Catch AlterEx As Exception

                        ' error, might not exist, run create
                        Try
                            RunCreateAter(Criteria, False)
                        Catch CreateEx As Exception
                            Throw New Exception("Error creating Get Table Reference List Stored Procedure", CreateEx)
                        End Try
                    End Try

                    ' if we get here then the create or Alter worked, so run the get again
                    RunGet(Criteria)
                Else
                    Throw GetEx
                End If
      End Try

    End Sub

    Private Sub RunGet(ByVal Criteria As Criteria)

      Dim crit As Criteria = Criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getTableReferenceList"

            cm.Parameters.AddWithValue("@TableToCheck", Criteria.TableName)
            cm.Parameters.AddWithValue("@Key", Criteria.KeyValue)

            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Private Sub RunCreateAter(ByVal Criteria As Criteria, ByVal Alter As Boolean)

      Dim crit As Criteria = Criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.Connection = cn
            cm.CommandType = CommandType.Text
            cm.CommandText = My.Resources.CREATE_GetTableReferences
            If Alter Then
              cm.CommandText = cm.CommandText.Replace("CREATE PROCEDURE", "ALTER PROCEDURE")
            End If

            cm.ExecuteNonQuery()
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