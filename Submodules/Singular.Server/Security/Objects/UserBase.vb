Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Singular.Localisation
Imports Csla.Core
Imports Csla.Serialization
Imports Csla

#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Security

  <Serializable()>
  Public MustInherit Class UserBase(Of T As UserBase(Of T))
    Inherits SingularBusinessBase(Of T)
    Implements IUser

    Protected mOldUserName As String
    Protected Const Password_Mask As String = "********"

#Region " Properties and Methods "

#Region " Properties "

    Public Shared UserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.UserID, "User", 0)
    ''' <Summary>
    ''' Gets the User value
    ''' </Summary>
    <Display(Name:="User", Description:="System generated unique ID", AutoGenerateField:=False), Key()>
    Public ReadOnly Property UserID() As Integer Implements IUser.UserID
      Get
        Return GetProperty(UserIDProperty)
      End Get
    End Property

    Public Shared FirstNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.FirstName, "First Name", "")
    ''' <Summary>
    ''' Gets and sets the First Name value
    ''' </Summary>
    <Display(Name:="First Name", Description:="User's first name"),
    Required(ErrorMessage:="First Name required"),
    StringLength(30, ErrorMessage:="First Name cannot be more than 30 characters"),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "FirstName")>
    Public Overridable Property FirstName() As String
      Get
        Return GetProperty(FirstNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(FirstNameProperty, Value)
      End Set
    End Property

    Public Shared SurnameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Surname, "Surname", "")
    ''' <Summary>
    ''' Gets and sets the Surname value
    ''' </Summary>
    <Display(Name:="Surname", Description:="User's surname"),
    Required(ErrorMessage:="Surname required"),
    StringLength(30, ErrorMessage:="Surname cannot be more than 30 characters"),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "Surname")>
    Public Overridable Property Surname() As String
      Get
        Return GetProperty(SurnameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(SurnameProperty, Value)
      End Set
    End Property

    <Display(AutoGenerateField:=False, Name:="Full Name", Description:="User's Full Name"),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "FullName"),
    Singular.DataAnnotations.DropDownColumn(ShowColumn:=False)>
    Public ReadOnly Property FullName() As String
      Get
        Return GetProperty(FirstNameProperty) + " " + GetProperty(SurnameProperty)
      End Get
    End Property

    Public Shared LoginNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.LoginName, "Login Name", "")
    ''' <Summary>
    ''' Gets and sets the Login Name value
    ''' </Summary>
    <Display(Name:="User Name", Description:="User's login name"),
    Required(ErrorMessage:="User Name required"),
    StringLength(50, ErrorMessage:="Login Name cannot be more than 50 characters"),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "LoginName"),
    Singular.DataAnnotations.DropDownColumn(ShowColumn:=True, IsDisplayMember:=True)>
    Public Overridable Property LoginName() As String
      Get
        Return GetProperty(LoginNameProperty)
      End Get
      Set(ByVal Value As String)
        ' SetProperty(LoginNameProperty, Value.Trim())
        SetProperty(LoginNameProperty, Value)
      End Set
    End Property

    Public Shared PasswordProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Password, "Password")

#If SILVERLIGHT Then
    <Display(Name:="Password"),
    StringLength(100, ErrorMessage:="Password cannot be more than 100 characters"),
    Singular.DataAnnotations.PasswordField(),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "Password")> _
    Public Property Password() As String
      Get
        Return Password_Mask
      End Get
      Set(ByVal Value As String)
        'Dim NewVal As String = ""
        '#If Silverlight Then
        '        NewVal = Singular.Encryption.GetSilverlightStringHash(Value, True)
        '#End If
        If Value <> Password_Mask Then
          SetProperty(PasswordProperty, Value)
        End If

      End Set
    End Property
#Else
    <Display(Name:="Password"),
    StringLength(100, ErrorMessage:="Password cannot be more than 100 characters"),
    PasswordPropertyText(),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "Password"),
    Singular.DataAnnotations.DropDownColumn(ShowColumn:=False)>
    Public Overridable Property Password() As String
      Get
        Return Password_Mask
      End Get
      Set(ByVal Value As String)
        'Dim NewVal As String = ""
        '#If Silverlight Then
        '        NewVal = Singular.Encryption.GetSilverlightStringHash(Value, True)
        '#End If
        If Value <> Password_Mask Then
          If Settings.CurrentPlatform = CommonDataPlatform.Windows Then
            SetProperty(PasswordProperty, Singular.Encryption.GetStringHash(Value, Encryption.HashType.Sha256))
          Else
            SetProperty(PasswordProperty, Value)
          End If
        End If

      End Set
    End Property


#End If

    Public Shared PasswordChangeDateProperty As PropertyInfo(Of Nullable(Of DateTime)) = RegisterProperty(Of Nullable(Of DateTime))(Function(c) c.PasswordChangeDate, "Password Change Date", Now)
    ''' <Summary>
    ''' Gets and sets the Password Change Date value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Password Change Date", Description:="The date the user last changed their password. Used with Password Expiry in misc to determine if a user must change their password."),
    Required(ErrorMessage:="Password Change Date required"),
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "PasswordChangeDate"),
    Singular.DataAnnotations.DropDownColumn(ShowColumn:=False)>
    Public Property PasswordChangeDate() As Nullable(Of DateTime)
      Get
        Return GetProperty(PasswordChangeDateProperty)
      End Get
      Set(ByVal value As Nullable(Of DateTime))
        SetProperty(PasswordChangeDateProperty, value)
        OnPropertyChanged("PasswordNotChangedDays")
        OnPropertyChanged("PasswordExpiresText")
      End Set
    End Property

    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property PasswordNotChangedDays() As String
      Get

        Return Now.Subtract(Me.PasswordChangeDate).Days


      End Get
    End Property

    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property PasswordExpiresText() As String
      Get
        Dim days As Integer = 0
        Singular.Security.PasswordPolicy.CheckPasswordHasExpired(Me.PasswordChangeDate, days)
        If days <= Singular.Security.PasswordPolicy.WarningPasswordExpiresDays Then
          If days < 0 Then

            Return " (Expired " + Singular.Strings.Pluralize(-days, "Day", , , , , False, True) + " ago)"
          Else
            Return " (Expires in " + Singular.Strings.Pluralize(days, "Day", , , , , False, True) + ")"

          End If
        Else
          Return ""
        End If
      End Get
    End Property

    '<Display(AutogenerateField:=False)> _
    'Public ReadOnly Property PasswordNotChangedColor() As String
    '  Get

    '  End Get
    'End Property

    Public Shared EmailAddressProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.EmailAddress, "Email Address", "")
    ''' <Summary>
    ''' Gets and sets the Email Address value
    ''' </Summary>
    <Display(Name:="Email Address", Description:="Email address of the user"),
    StringLength(100, ErrorMessage:="Email Address cannot be more than 100 characters"), Singular.DataAnnotations.EmailField,
    Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "EmailAddress"),
    Singular.DataAnnotations.DropDownColumn(ShowColumn:=False)>
    Public Overridable Property EmailAddress() As String
      Get
        Return GetProperty(EmailAddressProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(EmailAddressProperty, Value)
      End Set
    End Property

    Public Shared CreatedByProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.CreatedBy, "Created By", "")
    ''' <Summary>
    ''' Gets the Created By value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Created By", Description:="The logged in user that created the record (Surname, First Name)")>
    Public ReadOnly Property CreatedBy() As String
      Get
        Return GetProperty(CreatedByProperty)
      End Get
    End Property

    Public Shared CreatedDateProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.CreatedDate, "Created Date", New SmartDate(Now()))
    ''' <Summary>
    ''' Gets the Created Date value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Created Date", Description:="Date created")>
    Public ReadOnly Property CreatedDate() As SmartDate
      Get
        Return GetProperty(CreatedDateProperty)
      End Get
    End Property


#If SILVERLIGHT Then
#Else
    <DefaultValue(True), Browsable(True), Display(AutoGenerateField:=False),
    Singular.DataAnnotations.DropDownColumn(ShowColumn:=False)>
    Public Property Expanded As Boolean = False

#End If

#End Region

#Region " Child Lists "

    Public Shared SecurityGroupUserListProperty As PropertyInfo(Of SecurityGroupUserList) = RegisterProperty(Of SecurityGroupUserList)(Function(c) c.SecurityGroupUserList, "Security Group User List")

#If SILVERLIGHT Then
    <Display(AutoGenerateField:=False)> _
    Public ReadOnly Property SecurityGroupUserList() As SecurityGroupUserList
      Get
        If GetProperty(SecurityGroupUserListProperty) Is Nothing Then
          LoadProperty(SecurityGroupUserListProperty, SecurityGroupUserList.NewSecurityGroupUserList())
        End If
        Return GetProperty(SecurityGroupUserListProperty)
      End Get
    End Property
#Else
    'Like this because the web needs the list to be shown.

    Public ReadOnly Property SecurityGroupUserList() As SecurityGroupUserList
      Get
        If GetProperty(SecurityGroupUserListProperty) Is Nothing Then
          LoadProperty(SecurityGroupUserListProperty, SecurityGroupUserList.NewSecurityGroupUserList())
        End If
        Return GetProperty(SecurityGroupUserListProperty)
      End Get
    End Property
#End If

#End Region

#Region " Methods "

    Public Function GetOldUserName() As String
      Return mOldUserName
    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(UserIDProperty)

    End Function

#If SILVERLIGHT Then
#Else
    Public Shared ToStringField As Csla.PropertyInfo(Of String) = RegisterReadOnlyProperty(Of String)(Function(c) c.ToString, Function(c) c.ToStringHelper(c.LoginName, "User"))

#End If

    Public Overrides Function ToString() As String

      If Me.FirstName.Length = 0 Then
        If Me.IsNew Then
          Return "New User"
        Else
          Return "Blank User"
        End If
      Else
        Return Me.FirstName & " " & Surname
      End If

    End Function

    Protected Overrides ReadOnly Property TableReferencesToIgnore() As String()
      Get
        Return New String() {"SecurityGroupUsers"}
      End Get
    End Property

#End Region

#End Region

#Region " Validation Rules "

    Protected Overridable Sub AddCustomPasswordRule()
      BusinessRules.AddRule(New PasswordRule(PasswordProperty, UserIDProperty))
    End Sub

    Protected Overrides Sub AddBusinessRules()
      AddCustomPasswordRule()
      AddBaseRules()
      BusinessRules.AddRule(New Csla.Rules.CommonRules.Required(PasswordProperty))
    End Sub

    Protected Friend Sub AddBaseRules()
      MyBase.AddBusinessRules()
    End Sub

    Protected Class PasswordRule
      Inherits Csla.Rules.BusinessRule

      Public Property HashPassword As Boolean = True

      Public Sub New(ByVal PrimaryProperty As IPropertyInfo, UserIDProperty As IPropertyInfo, Optional HashPassword As Boolean = True)

        MyBase.New(PrimaryProperty)
        Me.HashPassword = HashPassword
        If UserIDProperty Is Nothing Then
          Me.InputProperties = New List(Of IPropertyInfo) From {PrimaryProperty}
        Else
          Me.InputProperties = New List(Of IPropertyInfo) From {PrimaryProperty, UserIDProperty}
        End If

        Me.AffectedProperties.Add(PrimaryProperty)

#If SILVERLIGHT Then
        Me.IsAsync = True
#End If

      End Sub

      Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

        If Not Singular.Security.PasswordPolicy.EnforcePasswordPolicy Then
          Exit Sub
        End If

        Dim Message As String = ""

        Dim NewPassword = context.InputPropertyValues(Me.PrimaryProperty)

        If NewPassword Is Nothing OrElse NewPassword = Password_Mask OrElse NewPassword = "" Then
#If SILVERLIGHT Then
          context.Complete()
#End If
          Exit Sub
        End If

        If Me.InputProperties.Count > 1 Then
          Dim UserID = CInt(context.InputPropertyValues(Me.InputProperties(1)))

#If SILVERLIGHT Then
        'if UserID <> 0 then
          context.AddOutValue(PrimaryProperty, Singular.Encryption.GetSilverlightStringHash(NewPassword, True))
        'End If
#Else
          If HashPassword Then
            context.AddOutValue(PrimaryProperty, Security.PasswordPolicy.GetHashedPassword(NewPassword))
          End If
#End If

          Dim PassChecker As New PasswordChecker() With {.UserID = UserID, .PasswordToBeChecked = NewPassword, .MonthsPasswordNeedsToBeUnique = Singular.Security.PasswordPolicy.MonthsPasswordNeedsToBeUnique}

#If SILVERLIGHT Then

        PassChecker.BeginCheck(Sub(o, e2)
                                 If e2.Error IsNot Nothing Then
                                   Throw e2.Error
                                 End If

                                 PassChecker = e2.Object

                                 If Not PassChecker.PasswordHasChanged Then
                                   context.AddErrorResult("Please supply a new password.")
                                 End If

                                 If PassChecker.PasswordHasBeenUsed Then
                                   context.AddErrorResult("This Password has been used in the past " + Singular.Security.PasswordPolicy.MonthsPasswordNeedsToBeUnique.ToString + " Months.")
                                 End If

                                 If Not Singular.Security.PasswordPolicy.ValidatePassword(NewPassword, Message) Then
                                   context.AddErrorResult(Message)
                                 End If

                                 context.Complete()

                               End Sub)




#Else
          If Singular.Security.PasswordPolicy.MonthsPasswordNeedsToBeUnique > 0 Then
            PassChecker.CheckPassword(UserID, NewPassword, Singular.Security.PasswordPolicy.MonthsPasswordNeedsToBeUnique)

            If Not PassChecker.PasswordHasChanged Then
              context.AddErrorResult("Please supply a new password.")
            End If

            If PassChecker.PasswordHasBeenUsed Then
              context.AddErrorResult("This Password has been used in the past " + Singular.Security.PasswordPolicy.MonthsPasswordNeedsToBeUnique.ToString + " Months.")
            End If
          End If


#End If
        End If

#If SILVERLIGHT Then
#Else
        If Not Singular.Security.PasswordPolicy.ValidatePassword(NewPassword, Message) Then
          context.AddErrorResult(Message)
        End If
#End If

      End Sub

    End Class

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    'Public Shared Function NewAuditFirm() As User

    '  Return DataPortal.CreateChild(Of User)()

    'End Function

    Public Sub New()

      MarkAsChild()
      'BusinessRules.CheckRules()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Protected Friend Overridable Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(UserIDProperty, .GetInt32(0))
          LoadProperty(FirstNameProperty, .GetString(1))
          LoadProperty(SurnameProperty, .GetString(2))
          LoadProperty(LoginNameProperty, .GetString(3))
          ' dont load actual property
          LoadProperty(PasswordProperty, Password_Mask)
          LoadProperty(PasswordChangeDateProperty, .GetValue(5))
          LoadProperty(EmailAddressProperty, .GetString(6))
          LoadProperty(CreatedByProperty, .GetString(7))
          LoadProperty(CreatedDateProperty, .GetSmartDate(8))

          mOldUserName = LoginName
        End With
      End Using

      FetchExtraProperties(sdr)

      MarkAsChild()
      MarkOld()
      'BusinessRules.CheckRules()

    End Sub

    Protected Overridable Sub FetchExtraProperties(ByRef sdr As SafeDataReader)

    End Sub

    Public Overridable Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = InsertProcName

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Public Overridable Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = UpdateProcName

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overridable ReadOnly Property UpdateProcName As String
      Get
        Return "UpdProcs.updUser"
      End Get
    End Property

    Protected Overridable ReadOnly Property InsertProcName As String
      Get
        Return "InsProcs.insUser"
      End Get
    End Property

    Protected Overridable ReadOnly Property DeleteProcName As String
      Get
        Return "DelProcs.delUser"
      End Get
    End Property

    Protected Overridable ReadOnly Property UserNameParamName As String
      Get
        Return "@LoginName"
      End Get
    End Property

    Protected Overridable ReadOnly Property SurnameParamName As String
      Get
        Return "@Surname"
      End Get
    End Property

    Protected Overridable ReadOnly Property CanUpdateUserName As Boolean
      Get
        Return True
      End Get
    End Property

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)
      If MyBase.IsDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramUserID As SqlParameter = .Parameters.Add("@UserID", SqlDbType.Int)
          paramUserID.Value = GetProperty(UserIDProperty)
          If Me.IsNew Then
            paramUserID.Direction = ParameterDirection.Output
          End If

          .Parameters.AddWithValue("@FirstName", GetProperty(FirstNameProperty))
          .Parameters.AddWithValue(SurnameParamName, GetProperty(SurnameProperty))

          If IsNew OrElse CanUpdateUserName Then
            .Parameters.AddWithValue(UserNameParamName, GetProperty(LoginNameProperty).Trim)
          End If

          If GetProperty(PasswordProperty) = Password_Mask Then
            'Password hasn't changed, the stored proc will ignore nulls.
            .Parameters.AddWithValue("@Password", DBNull.Value)
          Else
            .Parameters.AddWithValue("@Password", GetProperty(PasswordProperty))
          End If

          .Parameters.AddWithValue("@EmailAddress", GetProperty(EmailAddressProperty))
          .Parameters.AddWithValue("@CreatedBy", Singular.Misc.ZeroDBNull(Singular.Settings.CurrentUserID))
          AddExtraParameters(.Parameters)

          .ExecuteNonQuery()

          mOldUserName = LoginName

          If Me.IsNew() Then
            LoadProperty(UserIDProperty, paramUserID.Value)
          End If
          LoadProperty(PasswordProperty, Password_Mask)
          ' update child objects
          SecurityGroupUserList.Update()
          UpdateChildLists()
          MarkOld()
        End With
      Else
        ' update child objects
        SecurityGroupUserList.Update()
        UpdateChildLists()
      End If

    End Sub

    Protected Overridable Sub AddExtraParameters(ByRef Parameters As SqlClient.SqlParameterCollection)

    End Sub

    Protected Overridable Sub UpdateChildLists()

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = DeleteProcName
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@UserID", GetProperty(UserIDProperty))
        DoDeleteChild(cm)
      End Using

    End Sub

    Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)

      If Me.IsNew Then Exit Sub

      With cm
        .ExecuteNonQuery()
      End With
      MarkNew()

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace
