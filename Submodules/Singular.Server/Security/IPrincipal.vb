Namespace Security

  Public Interface IPrincipal
    Inherits System.Security.Principal.IPrincipal

    Function GetIdentity(Of T As IIdentity)() As T

  End Interface

End Namespace


