Imports Csla.Core
Imports Csla.Rules
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Security

  <Serializable()> _
  Public Class PasswordChecker
    Inherits SingularBusinessBase(Of PasswordChecker)

    Public Sub New()

    End Sub

    Public Shared UserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.UserID, "User", 0)

    Public Property UserID() As Integer
      Get
        Return GetProperty(UserIDProperty)
      End Get
      Set(value As Integer)
        SetProperty(UserIDProperty, value)
      End Set
    End Property

    Public Shared PasswordToBeCheckedProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.PasswordToBeChecked, "Password To Be Checked", "")

    Public Property PasswordToBeChecked() As String
      Get
        Return GetProperty(PasswordToBeCheckedProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(PasswordToBeCheckedProperty, Value)
      End Set
    End Property

    Public Shared PasswordHasChangedProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.PasswordHasChanged, "Password has changed", False)

    Public Property PasswordHasChanged() As Boolean
      Get
        Return GetProperty(PasswordHasChangedProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(PasswordHasChangedProperty, Value)
      End Set
    End Property

    Public Shared MonthsPasswordNeedsToBeUniqueProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.MonthsPasswordNeedsToBeUnique, "User", 0)

    Public Property MonthsPasswordNeedsToBeUnique() As Integer
      Get
        Return GetProperty(MonthsPasswordNeedsToBeUniqueProperty)
      End Get
      Set(value As Integer)
        SetProperty(MonthsPasswordNeedsToBeUniqueProperty, value)
      End Set
    End Property

    Public Shared PasswordHasBeenUsedProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.PasswordHasBeenUsed, "Password has changed", False)

    Public Property PasswordHasBeenUsed() As Boolean
      Get
        Return GetProperty(PasswordHasBeenUsedProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(PasswordHasBeenUsedProperty, Value)
      End Set
    End Property

    <Serializable()>
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Shared UserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.UserID, "User", 0)

      Public Property UserID() As Integer
        Get
          Return ReadProperty(UserIDProperty)
        End Get
        Set(value As Integer)
          LoadProperty(UserIDProperty, value)
        End Set
      End Property

      Public Shared PasswordToBeCheckedProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.PasswordToBeChecked, "Password To Be Checked", "")

      Public Property PasswordToBeChecked() As String
        Get
          Return ReadProperty(PasswordToBeCheckedProperty)
        End Get
        Set(ByVal Value As String)
          LoadProperty(PasswordToBeCheckedProperty, Value)
        End Set
      End Property

      Public Shared MonthsPasswordNeedsToBeUniqueProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.MonthsPasswordNeedsToBeUnique, "User", 0)

      Public Property MonthsPasswordNeedsToBeUnique() As Integer
        Get
          Return ReadProperty(MonthsPasswordNeedsToBeUniqueProperty)
        End Get
        Set(value As Integer)
          LoadProperty(MonthsPasswordNeedsToBeUniqueProperty, value)
        End Set
      End Property

    End Class

#If SILVERLIGHT Then

    Public Sub BeginCheck(CheckCompletedHandler As EventHandler(Of DataPortalResult(Of PasswordChecker)))

      Dim dp As New DataPortal(Of PasswordChecker)
      AddHandler dp.FetchCompleted, CheckCompletedHandler
      dp.BeginFetch(New Criteria With {.UserID = Me.UserID, .PasswordToBeChecked = Me.PasswordToBeChecked, .MonthsPasswordNeedsToBeUnique = Me.MonthsPasswordNeedsToBeUnique})

    End Sub

#Else

    Protected Sub DataPortal_Fetch(ByVal criteria As Criteria)
      Me.CheckPassword(criteria)
    End Sub

    Private Sub CheckPassword(ByVal criteria As Criteria)

      Using cn As New SqlClient.SqlConnection(Settings.ConnectionString)
        cn.Open()
        'Call the stored proc that checks the credentials, and returns all the information about the user.
        Using cm As New SqlClient.SqlCommand("[CmdProcs].[cmdCheckPasswordChanged]", cn)
          cm.Parameters.AddWithValue("@UserID", criteria.UserID)
          cm.Parameters.AddWithValue("@PasswordToBeChecked", Singular.Encryption.GetStringHash(criteria.PasswordToBeChecked, Encryption.HashType.Sha256, True))
          cm.Parameters.AddWithValue("@MonthsPasswordNeedsToBeUnique", criteria.MonthsPasswordNeedsToBeUnique)
          cm.CommandType = CommandType.StoredProcedure

          Using sdr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
            If sdr.Read Then
              LoadProperty(PasswordHasChangedProperty, sdr.GetBoolean(0))
              LoadProperty(PasswordHasBeenUsedProperty, sdr.GetBoolean(1))
            End If
          End Using
        End Using
      End Using

    End Sub

    Public Sub CheckPassword(UserID As Integer, PasswordToBeChecked As String, MonthsPasswordNeedsToBeUnique As Integer)

      CheckPassword(New Criteria With {.UserID = UserID, .PasswordToBeChecked = PasswordToBeChecked, .MonthsPasswordNeedsToBeUnique = MonthsPasswordNeedsToBeUnique})

    End Sub


#End If

  End Class

End Namespace
