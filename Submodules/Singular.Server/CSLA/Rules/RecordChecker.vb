Imports Csla.Core
Imports Csla.Rules
Imports Csla
Imports Csla.Serialization
Imports Csla.Data

#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Rules

  <Serializable()>
  Public Class RecordChecker
    Inherits Csla.BusinessBase(Of RecordChecker)

#Region " Properties "

    Public Shared ResultDescriptionProperty As Csla.PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ResultDescription)

    Public Property ResultDescription As String
      Get
        Return ReadProperty(ResultDescriptionProperty)
      End Get
      Private Set(ByVal value As String)
        LoadProperty(ResultDescriptionProperty, value)
      End Set
    End Property

    Public Shared DuplicateExistsProperty As Csla.PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.DuplicateExists)

    Public Property DuplicateExists As Boolean
      Get
        Return ReadProperty(DuplicateExistsProperty)
      End Get
      Private Set(ByVal value As Boolean)
        LoadProperty(DuplicateExistsProperty, value)
      End Set
    End Property

#End Region

#Region " Criteria "

    <Serializable()> _
    Public Class Criteria
      Inherits Csla.CriteriaBase(Of Criteria)

      Public Shared PrimaryKeyNameProperty As Csla.PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.PrimaryKeyName)

      Public Property PrimaryKeyName As String
        Get
          Return ReadProperty(PrimaryKeyNameProperty)
        End Get
        Set(ByVal value As String)
          LoadProperty(PrimaryKeyNameProperty, value)
        End Set
      End Property

      Public Shared PrimaryKeyValueProperty As Csla.PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.PrimaryKeyValue)

      Public Property PrimaryKeyValue As Integer
        Get
          Return ReadProperty(PrimaryKeyValueProperty)
        End Get
        Set(ByVal value As Integer)
          LoadProperty(PrimaryKeyValueProperty, value)
        End Set
      End Property

      Public Shared UniquePropertyNamesProperty As Csla.PropertyInfo(Of MobileList(Of String)) = RegisterProperty(Of MobileList(Of String))(Function(c) c.UniquePropertyNames)

      Public Property UniquePropertyNames As MobileList(Of String)
        Get
          Return ReadProperty(UniquePropertyNamesProperty)
        End Get
        Set(ByVal value As MobileList(Of String))
          LoadProperty(UniquePropertyNamesProperty, value)
        End Set
      End Property

      Public Shared UniquePropertyValuesProperty As Csla.PropertyInfo(Of MobileList(Of String)) = RegisterProperty(Of MobileList(Of String))(Function(c) c.UniquePropertyValues)

      Public Property UniquePropertyValues As MobileList(Of String)
        Get
          Return ReadProperty(UniquePropertyValuesProperty)
        End Get
        Set(ByVal value As MobileList(Of String))
          LoadProperty(UniquePropertyValuesProperty, value)
        End Set
      End Property

      Public Shared DisplayPropertyNamesProperty As Csla.PropertyInfo(Of MobileList(Of String)) = RegisterProperty(Of MobileList(Of String))(Function(c) c.DisplayPropertyNames)

      Public Property DisplayPropertyNames As MobileList(Of String)
        Get
          Return ReadProperty(DisplayPropertyNamesProperty)
        End Get
        Set(ByVal value As MobileList(Of String))
          LoadProperty(DisplayPropertyNamesProperty, value)
        End Set
      End Property


      Public Shared TableNameProperty As Csla.PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.TableName)

      Public Property TableName As String
        Get
          Return ReadProperty(TableNameProperty)
        End Get
        Set(ByVal value As String)
          LoadProperty(TableNameProperty, value)
        End Set
      End Property

      Public Sub New(ByVal PrimaryKeyName As String, ByVal PrimaryKeyValue As String, ByVal UniquePropertyNames As MobileList(Of String), ByVal UniquePropertyValues As MobileList(Of String), ByVal TableName As String, ByVal DisplayProperties As MobileList(Of String))

        Me.PrimaryKeyName = PrimaryKeyName
        Me.PrimaryKeyValue = PrimaryKeyValue
        Me.UniquePropertyNames = UniquePropertyNames
        Me.UniquePropertyValues = UniquePropertyValues
        Me.TableName = TableName
        Me.DisplayPropertyNames = DisplayProperties

      End Sub

      Public Sub New()

      End Sub

    End Class

#End Region

#Region " Data Access "

    Public Sub New()

    End Sub


#If SILVERLIGHT Then


    Public Shared Sub DuplicateRecordExists(ByVal PrimaryKeyProperty As String, ByVal PrimaryKeyValue As Integer,
                                                 ByVal UniqueProperties As MobileList(Of String), ByVal UniquePropertyValues As MobileList(Of String),
                                                 ByVal TableName As String, ByVal DescriptionProperties As MobileList(Of String), ByVal CallBack As EventHandler(Of DataPortalResult(Of RecordChecker)))

      Dim dp As New DataPortal(Of RecordChecker)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria(PrimaryKeyProperty, PrimaryKeyValue, UniqueProperties, UniquePropertyValues, TableName, DescriptionProperties))

    End Sub

#Else

    Public Shared Function DuplicateRecordExists(ByVal PrimaryKeyProperty As String, ByVal PrimaryKeyValue As Integer,
                                             ByVal UniqueProperties As MobileList(Of String), ByVal UniquePropertyValues As MobileList(Of String),
                                             ByVal TableName As String, ByVal DescriptionProperties As MobileList(Of String)) As RecordChecker

      Return DataPortal.Fetch(Of RecordChecker)(New Criteria(PrimaryKeyProperty, PrimaryKeyValue, UniqueProperties, UniquePropertyValues, TableName, DescriptionProperties))

    End Function

    Protected Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.StoredProcedure
          cm.CommandText = "GetProcs.getDuplicateRecord"

          cm.Parameters.AddWithValue("@XmlUniqueProperties", Singular.Xml.ConvertMobileListToXml(Of String)(crit.UniquePropertyNames))
          cm.Parameters.AddWithValue("@XmlUniqueValues", Singular.Xml.ConvertMobileListToXml(Of String)(crit.UniquePropertyValues))
          cm.Parameters.AddWithValue("@XmlDisplayProperties", Singular.Xml.ConvertMobileListToXml(Of String)(crit.DisplayPropertyNames))
          cm.Parameters.AddWithValue("@PrimaryKeyProperty", crit.PrimaryKeyName)
          cm.Parameters.AddWithValue("@PrimaryKeyValue", crit.PrimaryKeyValue)
          cm.Parameters.AddWithValue("@TableName", crit.TableName)

          Try
            Me.ResultDescription = ExecuteCommand(cm)
          Catch ex As Exception
            If ex.Message.ToLower.Contains("not find") And Debug.InDebugMode Then
              Try
                Using cmdCreateCommand As SqlCommand = cn.CreateCommand
                  cmdCreateCommand.CommandText = My.Resources.getDuplicateRecord
                  cmdCreateCommand.ExecuteNonQuery()
                End Using
                Me.ResultDescription = ExecuteCommand(cm)
              Catch ex2 As Exception
                Throw ex
              End Try
            Else
              Throw ex
            End If
          End Try

        End Using
      End Using

    End Sub

    Private Function ExecuteCommand(cm As SqlCommand) As String

      Dim ResultDescription As String = ""

      Using sdr As New SafeDataReader(cm.ExecuteReader)
        If sdr.Read Then
          ' we must have a duplicate
          Me.DuplicateExists = True
          If ResultDescription <> "" Then
            ResultDescription &= vbCrLf
          End If
          ' then we must have a duplicate

          For i = 0 To sdr.FieldCount - 1
            ResultDescription &= CStr(sdr.GetValue(i)) & "  "
          Next
        End If
      End Using

      Return ResultDescription
    End Function




#End If

#End Region

  End Class


End Namespace
