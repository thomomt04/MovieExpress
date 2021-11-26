Namespace Security

  Public Interface IIdentity
    Inherits System.Security.Principal.IIdentity

    ReadOnly Property UserID As Integer
    ReadOnly Property IsAdministrator As Boolean
    ReadOnly Property UserName As String
    ReadOnly Property PasswordChangedDate As Csla.SmartDate
    Property Roles As Csla.Core.MobileList(Of String)

    Sub CopyRoles(List As Csla.Core.MobileList(Of String))

  End Interface


End Namespace

