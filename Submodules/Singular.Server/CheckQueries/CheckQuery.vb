Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
Imports C1.Silverlight.Data
#Else
Imports System.Data.SqlClient
#End If

Namespace CheckQueries

  Public Enum CheckQueryStatus
   ''' <summary>
    ''' View / Stored Proc hasnt run yet.
    ''' </summary>
    Pending = 1
    ''' <summary>
    ''' View / Stored Proc returned no rows.
    ''' </summary>
    Passed = 2
    ''' <summary>
    ''' View / Stored Proc returned 1 or more rows.
    ''' </summary>
    Failed = 3
    ''' <summary>
    ''' View / Stored Proc threw exception.
    ''' </summary>
    Exception = 4
  End Enum

  <Serializable()> _
  Public Class CheckQuery
    Inherits SingularBusinessBase(Of CheckQuery)

    Public Event CheckQueryDone()
    Private _ExtraFields As Object()

#Region " Properties and Methods "

#Region " Properties "

    Public Shared SchemaProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Schema, "Schema")
    ''' <Summary>
    ''' Gets and sets the ID value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Schema", Description:="")> _
    Public ReadOnly Property Schema() As String
      Get
        Return GetProperty(SchemaProperty)
      End Get
    End Property

    Public Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Name, "Name")
    ''' <Summary>
    ''' Gets and sets the Name value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Name", Description:=""), Browsable(True)> _
    Public ReadOnly Property Name() As String
      Get
        Return GetProperty(NameProperty)
      End Get
    End Property

    Public Shared DescriptionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Description)
    ''' <Summary>
    ''' Gets and sets the Definition value
    ''' </Summary>
    <Display(Name:="Description")> _
    Public ReadOnly Property Description() As String
      Get
        If GetProperty(DescriptionProperty) = "" Then
          SetProperty(DescriptionProperty, Singular.Strings.Readable(Name))
        End If
        Return GetProperty(DescriptionProperty)
      End Get
    End Property

    Public Shared SourceTypeProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SourceType)
    ''' <Summary>
    ''' Source Type. 1=View, 2=Stored Procedure
    ''' </Summary>
    <Display(AutoGenerateField:=False)> _
    Public ReadOnly Property SourceType() As Integer
      Get
        Return GetProperty(SourceTypeProperty)
      End Get
    End Property

    Public Shared CallNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.CallName)

    Public ReadOnly Property CallName() As String
      Get
        Return GetProperty(CallNameProperty)
      End Get
    End Property

    Public Shared GroupProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Group)

    Public ReadOnly Property Group() As String
      Get
        Return GetProperty(GroupProperty)
      End Get
    End Property

    Public Shared SeverityProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Severity)

    Public ReadOnly Property Severity() As Integer
      Get
        Return GetProperty(SeverityProperty)
      End Get
    End Property


#If SILVERLIGHT Then

    Public Shared CheckQueryDataSetProperty As PropertyInfo(Of DataView) = RegisterProperty(Of DataView)(Function(c) c.CheckQueryDataSet)

    <Display(AutoGenerateField:=False)> _
    Public Property CheckQueryDataSet() As DataView
      Get
        Return GetProperty(CheckQueryDataSetProperty)
      End Get
      Set(value As DataView)
        SetProperty(CheckQueryDataSetProperty, value)
      End Set
    End Property

    Public Shared StatusProperty As PropertyInfo(Of Imaging.BitmapImage) = RegisterProperty(Of Imaging.BitmapImage)(Function(c) c.Status, "Status", Singular.Images.Pending)

    <Display(Name:="Status", Description:="")> _
    Public Property Status() As Imaging.BitmapImage
      Get
        'If GetProperty(StatusProperty) Is Nothing Then
        '  SetProperty(StatusProperty, Singular.Images.PaperClip16)
        'End If
        Return GetProperty(StatusProperty)
      End Get
      Set(value As Imaging.BitmapImage)
        SetProperty(StatusProperty, value)
      End Set
    End Property

#Else

    Public Shared CheckQueryDataSetProperty As PropertyInfo(Of DataSet) = RegisterProperty(Of DataSet)(Function(c) c.CheckQueryDataSet, "Data Set", CType(Nothing, DataSet))

    <Browsable(False)>
    Public Property CheckQueryDataSet() As DataSet
      Get
        Return GetProperty(CheckQueryDataSetProperty)
      End Get
      Set(value As DataSet)
        SetProperty(CheckQueryDataSetProperty, value)
      End Set
    End Property

    Public Shared StatusProperty As PropertyInfo(Of CheckQueryStatus) = RegisterProperty(Of CheckQueryStatus)(Function(c) c.Status, "Status", CheckQueryStatus.Pending)
    ''' <summary>
    ''' 1 - Pending; 2 - Passed; 3 - Failed
    ''' </summary>
    <Display(Name:="Status", Description:="")> _
    Public Property Status() As CheckQueryStatus
      Get
        Return GetProperty(StatusProperty)
      End Get
      Set(value As CheckQueryStatus)
        SetProperty(StatusProperty, value)
      End Set
    End Property

    Private _RunException As String

    Public ReadOnly Property FailDescription As String
      Get
        If Status = CheckQueryStatus.Exception Then
          Return Name & " failed to run: " & _RunException
        Else
          If CheckQueryDataSet Is Nothing Then
            Return Name & " failed with no reason."
          Else
            Return Name & " failed with " & CheckQueryDataSet.Tables(0).Rows.Count & " rows returned."
          End If
        End If
      End Get
    End Property

#End If

#End Region

#Region " Methods "

    Public Sub Run(Optional Timeout As Integer = 30)
#If SILVERLIGHT Then

      If Not Me.IsBusy Then
        Me.MarkBusy()
      End If
      OnPropertyChanged("IsBusy")
      CheckQueryData.BeginGetCheckQueryData(Me.CallName, Me.SourceType, Sub(o, e)
                                                                    CheckQueryDataSet = e.Object.CheckQueryDataSet.Tables(0).DefaultView
                                                                    If CheckQueryDataSet.Count = 0 Then
                                                                      Me.Status = Singular.Images.Accept32
                                                                    Else
                                                                      Me.Status = Singular.Images.Cancel32
                                                                    End If
                                                                    Me.MarkIdle()
                                                                    OnPropertyChanged("IsBusy")
                                                                    RaiseEvent CheckQueryDone()
                                                                  End Sub)
#Else
      Try

        Dim cqd As Singular.CheckQueries.CheckQueryData = Singular.CheckQueries.CheckQueryData.GetCheckQueryData(Me.CallName, Me.SourceType, Timeout)
        Me.CheckQueryDataSet = cqd.GetDataSet()
        Me.Status = If(Me.CheckQueryDataSet Is Nothing, CheckQueryStatus.Passed, CheckQueryStatus.Failed)

      Catch ex As Exception
        _RunException = Singular.Debug.RecurseExceptionMessage(ex)
        Me.Status = CheckQueryStatus.Exception
      End Try

#End If

    End Sub

    Public Function AdditionalValueCount() As Integer
      Return If(_ExtraFields Is Nothing, 0, _ExtraFields.Length)
    End Function

    Public Function GetAdditionalValue(Index As Integer) As Object
      Return _ExtraFields(Index)
    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(NameProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.Name.Length = 0 Then
        If Me.IsNew Then
          Return "New Check Queries"
        Else
          Return "Blank Check Queries"
        End If
      Else
        Return Me.Name
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

    Public Shared Function NewCheckQueries() As CheckQuery

      Return DataPortal.CreateChild(Of CheckQuery)()

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

    Friend Shared Function GetCheckQueries(ByVal dr As SafeDataReader) As CheckQuery

      Dim c As New CheckQuery()
      c.Fetch(dr)
      Return c

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(SchemaProperty, .GetString(0))
          LoadProperty(NameProperty, .GetString(1))
          LoadProperty(SourceTypeProperty, .GetInt32(2))
          LoadProperty(CallNameProperty, .GetString(3))

          If sdr.FieldCount > 4 Then
            LoadProperty(GroupProperty, .GetString(4))
            LoadProperty(DescriptionProperty, .GetString(5))
            LoadProperty(SeverityProperty, .GetInt32(6))
          End If

          If sdr.FieldCount > 7 Then
            ReDim _ExtraFields(sdr.FieldCount - 8)
            For i As Integer = 0 To sdr.FieldCount - 8
              _ExtraFields(i) = sdr.GetValue(i + 7)
            Next
          End If

        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace