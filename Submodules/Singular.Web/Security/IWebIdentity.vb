Namespace Security

  Public Interface IWebIdentity
    Inherits Singular.Security.IIdentity

    ReadOnly Property LoginLabelHTML As String
    Property AuthType As AuthType
    Function GetAuthToken(ExpiryDate As Date) As String
    Function GetCSRFTokenValue() As String

  End Interface

End Namespace


