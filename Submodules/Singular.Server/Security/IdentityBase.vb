Imports Csla
Imports Csla.Serialization
Imports System.ComponentModel.DataAnnotations
Imports System.Linq

Namespace Security

  <Serializable()> _
  Public MustInherit Class IdentityBase(Of i As IdentityBase(Of i))
    Inherits Csla.Security.CslaIdentityBase(Of i)
    Implements IIdentity

    Public Shared UserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.UserID)
    ''' <summary>
    ''' The Database ID of the User
    ''' </summary>
    Public Overridable ReadOnly Property UserID() As Integer Implements IIdentity.UserID
      Get
        Return GetProperty(UserIDProperty)
      End Get
    End Property

    Public Shared UserNameReadableProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.UserNameReadable)
    ''' <summary>
    ''' The Full Name of the User
    ''' </summary>
    Public ReadOnly Property UserNameReadable() As String Implements IIdentity.UserName
      Get
        Return GetProperty(UserNameReadableProperty)
      End Get
    End Property

    Public Shared IsAdministratorProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.IsAdministrator, "Is Administrator?", False)
    ''' <summary>
    ''' Indicates that the user is an administrator.
    ''' </summary>
    Public ReadOnly Property IsAdministrator() As Boolean Implements IIdentity.IsAdministrator
      Get
        Return GetProperty(IsAdministratorProperty)
      End Get
    End Property

    Public Shared PasswordChangedDateProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.PasswordChangedDate, "PasswordChangedDate")
    ''' <summary>
    ''' Password last changed on this date.
    ''' </summary>
    Public ReadOnly Property PasswordChangedDate() As SmartDate Implements IIdentity.PasswordChangedDate
      Get
        Return GetProperty(PasswordChangedDateProperty)
      End Get
    End Property

    Public Overridable Function IsInRole(Role As String) As Boolean

      Dim RoleSections() As String = Split(Role, ".", 3)

      If Me.Roles.Contains(Role) Then
        Return True
      Else
        ' If you have passed in 3 levels of security then validate on 3
        If RoleSections.Count = 3 Then
          Return Me.Roles.Contains(Role)
        ElseIf RoleSections.Count = 2 Then
          ' Validate on 2 levels of Security, strip off the last element from the roles returned from DB.
          Dim TrimmedRoles = From r In Me.Roles
                             Select r.Substring(0, r.LastIndexOf("."))
          Return TrimmedRoles.Contains(Role)
        End If
      End If
      Return False

    End Function

    Public Sub CopyRoles(Roles As Csla.Core.MobileList(Of String)) Implements IIdentity.CopyRoles
      For Each role As String In Me.Roles
        Roles.Add(role)
      Next
    End Sub

    Public Sub AddRole(Role As String)

      If Roles Is Nothing Then
        Roles = New Csla.Core.MobileList(Of String)
      End If

      Roles.Add(Role)

    End Sub

    <System.ComponentModel.Browsable(False)>
    Public Overloads Property Roles As Csla.Core.MobileList(Of String) Implements IIdentity.Roles
      Get
        Return MyBase.Roles
      End Get
      Set(value As Csla.Core.MobileList(Of String))
        MyBase.Roles = value
      End Set
    End Property

    Public Sub Populate(UserID As Integer, UserNameReadable As String)
      LoadProperty(UserIDProperty, UserID)
      LoadProperty(UserNameReadableProperty, UserNameReadable)
    End Sub

    Public Sub Populate(UserID As Integer, UserNameReadable As String, IsAuthenticated As Boolean)
      Populate(UserID, UserNameReadable)
      LoadProperty(IsAuthenticatedProperty, IsAuthenticated)
    End Sub

    Public Sub FromIIdentity(Identity As IIdentity)
      LoadProperty(UserIDProperty, Identity.UserID)
      LoadProperty(UserNameReadableProperty, Identity.UserName)
      LoadProperty(IsAdministratorProperty, Identity.IsAdministrator)
      LoadProperty(PasswordChangedDateProperty, Identity.PasswordChangedDate)
      Name = Identity.Name
      Me.IsAuthenticated = Identity.IsAuthenticated
      Me.Roles = New Csla.Core.MobileList(Of String)
      Identity.CopyRoles(Me.Roles)
    End Sub

    Public Sub New()
      LoadProperty(PasswordChangedDateProperty, New SmartDate(Date.Now))
    End Sub

#If SILVERLIGHT Then

#Else

    Protected MustOverride Sub SetupSqlCommand(cmd As SqlClient.SqlCommand, Criteria As IdentityCriterea)

    Protected Overridable Sub ReadProperties(sdr As Csla.Data.SafeDataReader)
      LoadProperty(UserIDProperty, sdr.GetInt32(0))
      LoadProperty(NameProperty, sdr.GetString(1))
      LoadProperty(UserNameReadableProperty, sdr.GetString(2))
      LoadProperty(IsAdministratorProperty, sdr.GetBoolean(3))
      LoadProperty(PasswordChangedDateProperty, sdr.GetSmartDate(4))

      ReadExtraProperties(sdr, 5)
    End Sub

    Protected Overridable Sub ReadExtraProperties(sdr As Csla.Data.SafeDataReader, ByRef StartIndex As Integer)

    End Sub

    Protected Overridable Sub ReadExtraResultSets(sdr As Csla.Data.SafeDataReader)

    End Sub

    Protected Overloads Sub DataPortal_Fetch(ByVal credentials As IdentityCriterea)

      AuthenticateUser(credentials)

    End Sub

    Protected Overridable ReadOnly Property ConnectionString As String
      Get
        Return Settings.ConnectionString
      End Get
    End Property


    Protected Overridable Sub AuthenticateUser(ByVal credentials As IdentityCriterea)

      'Tell the object that we are using custom authentication against our custom database tables.
      'Other options are Windows Authentication (Using a domain account) or ASP.Net Authentication (Which uses Microsoft specific tables and stored procedures).
      'AuthenticationType = "Custom"

      Using cn As New SqlClient.SqlConnection(ConnectionString)
        cn.Open()
        'Call the stored proc that checks the credentials, and returns all the information about the user.
        Using cm As New SqlClient.SqlCommand()
          cm.Connection = cn
          cm.Parameters.AddWithValue("@UserName", credentials.Username)
          cm.Parameters.AddWithValue("@Password", credentials.Password)
          If Singular.Misc.NothingDBNull(credentials.RefreshingRoles) IsNot Nothing Then
            cm.Parameters.AddWithValue("@RefreshingRoles", Singular.Misc.NothingDBNull(credentials.RefreshingRoles))
          End If
          cm.CommandType = CommandType.StoredProcedure

          'Tell specialised versions to setup the command
          SetupSqlCommand(cm, credentials)

          Using sdr As New Csla.Data.SafeDataReader(cm.ExecuteReader)

            If sdr.Read Then

              'If the stored proc returns a result, then the user is authenticated.
              IsAuthenticated = True

              'Get the Data about the user.
              ReadProperties(sdr)

              Roles = New Csla.Core.MobileList(Of String)

              'Get the list of roles that this user has.
              If sdr.NextResult Then
                While sdr.Read
                  'Roles.Add(sdr.GetString(0) & "." & sdr.GetString(1))
                  If sdr.FieldCount > 2 Then
                    Roles.Add(sdr.GetString(0) & "." & sdr.GetString(1) & "." & sdr.GetString(2))
                  Else
                    Roles.Add(sdr.GetString(0) & "." & sdr.GetString(1))
                  End If

                End While
              End If

              ReadExtraResultSets(sdr)

            Else
              'If the stored proc does not return a result, then the user is not authenticated.
              Name = ""
              IsAuthenticated = False
            End If

          End Using
        End Using
      End Using


    End Sub

    ''' <summary>
    ''' Creates an identity for use with services, where there is not an app user that logs in.
    ''' </summary>
    Public Sub MakeServiceUser(UserID As Integer)
      LoadProperty(UserIDProperty, UserID)
      LoadProperty(NameProperty, "Service")
    End Sub

#End If

  End Class

  <Serializable()>
  Public Class IdentityCriterea
    Inherits Csla.CriteriaBase(Of IdentityCriterea)

    Public Shared UsernameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Username)

    Public Property Username() As String
      Get
        Return ReadProperty(UsernameProperty)
      End Get
      Set(value As String)
        LoadProperty(UsernameProperty, value)
      End Set
    End Property

    Public Shared HashedUsernameProperty As PropertyInfo(Of Byte()) = RegisterProperty(Of Byte())(Function(c) c.HashedUsername)

    Public Property HashedUsername() As Byte()
      Get
        Return ReadProperty(HashedUsernameProperty)
      End Get
      Set(value As Byte())
        LoadProperty(HashedUsernameProperty, value)
      End Set
    End Property

    Public Shared PasswordProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Password)

    Public Property Password() As String
      Get
        Return ReadProperty(PasswordProperty)
      End Get
      Set(value As String)
        LoadProperty(PasswordProperty, value)
      End Set
    End Property

    Public Shared RefreshingRolesProperty As PropertyInfo(Of Boolean?) = RegisterProperty(Of Boolean?)(Function(c) c.RefreshingRoles)

    Public Property RefreshingRoles() As Boolean?
      Get
        Return ReadProperty(RefreshingRolesProperty)
      End Get
      Set(value As Boolean?)
        LoadProperty(RefreshingRolesProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' User data from the forms auth cookie. Set Singular.Security.DecryptCookie = True for this to be set.
    ''' </summary>
    ''' <returns></returns>
    Public Property AuthTicketUserData As String

    Public Sub New()

    End Sub

    Public Sub New(Username As String, Password As String, RefreshingRoles As Boolean)
      Me.Username = Username
      Me.Password = Password
      Me.RefreshingRoles = RefreshingRoles
    End Sub

    Public Sub New(Username As String, Password As String)
      Me.Username = Username
      Me.Password = Password
    End Sub

    Public Shared Function CreateEncyptedIdentity(EncryptedUsername As Byte(), Password As String, RefreshingRoles As Boolean) As IdentityCriterea

      Dim ic As New IdentityCriterea()
      ic.HashedUsername = EncryptedUsername
      ic.Password = Password
      ic.RefreshingRoles = RefreshingRoles
      Return ic

    End Function

    Public Shared Function CreateHashedUsernameIdentity(HashedUsername As Byte(), Password As String) As IdentityCriterea

      Dim ic As New IdentityCriterea()
      ic.HashedUsername = HashedUsername
      ic.Password = Password
      Return ic

    End Function

  End Class

End Namespace

