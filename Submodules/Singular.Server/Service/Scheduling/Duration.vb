Imports Csla.Serialization
Imports Csla


Namespace Service.Scheduling

  <Serializable()> _
  Public Class Duration
    Inherits SingularBusinessBase(Of Duration)


    Public Shared StartDateProperty As PropertyInfo(Of DateTime) = RegisterProperty(Of DateTime)(Function(c) c.StartDate, "StartDate", Now.Date)

    Public Property StartDate() As DateTime
      Get
        Return GetProperty(StartDateProperty)
      End Get
      Set(value As DateTime)
        SetProperty(StartDateProperty, value)
      End Set
    End Property

    Public Shared EndDateProperty As PropertyInfo(Of DateTime?) = RegisterProperty(Of DateTime?)(Function(c) c.EndDate, "EndDate", CType(Nothing, DateTime?))

    Public Property EndDate() As DateTime?
      Get
        Return GetProperty(EndDateProperty)
      End Get
      Set(value As DateTime?)
        SetProperty(EndDateProperty, value)
      End Set
    End Property

    Public Shared HasEndDateProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.HasEndDate, "HasEndDate", False)
#If SILVERLIGHT Then
    Public Property HasEndDate() As Boolean
      Get
        Return GetProperty(HasEndDateProperty)
      End Get
      Set(value As Boolean)
        SetProperty(HasEndDateProperty, value)
      End Set
    End Property
#Else
    <System.ComponentModel.DisplayName("Has End Date?")>
    Public Property HasEndDate() As Boolean
      Get
        Return GetProperty(HasEndDateProperty)
      End Get
      Set(value As Boolean)
        SetProperty(HasEndDateProperty, value)
      End Set
    End Property
#End If

    Public Overrides Function ToString() As String

      Dim str As String = "Starting on " & Me.StartDate.ToString("dd-MMM-yy")
      If Me.EndDate IsNot Nothing Then str = str & IIf(Me.HasEndDate, " and ending on " & Me.EndDate.Value.ToString("dd-MMM-yy"), "")
      Return str

    End Function

    Public Sub New(ByVal StartDate As DateTime)
      Me.StartDate = StartDate
      Me.HasEndDate = False
    End Sub

    Public Sub New(ByVal StartDate As DateTime, ByVal EndDate As DateTime)
      Me.StartDate = StartDate
      Me.EndDate = EndDate
      Me.HasEndDate = True
    End Sub

    Public Sub New()

    End Sub

    Public Function IsValidDate(ByVal AtDate As DateTime) As Boolean

      If Me.HasEndDate Then
        Return AtDate >= Me.StartDate AndAlso AtDate <= Me.EndDate
      Else
        Return AtDate >= Me.StartDate
      End If

    End Function

  End Class

End Namespace