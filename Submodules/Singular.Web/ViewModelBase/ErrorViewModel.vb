Public Class ErrorViewModel
  Inherits ViewModel(Of ErrorViewModel)

  Protected Overrides Sub Setup()
    MyBase.Setup()



  End Sub

  Public ReadOnly Property AppErrorLocation As String
    Get
      Return System.Web.HttpContext.Current.Session("AppErrorLocation")
    End Get
  End Property

  Public ReadOnly Property AppErrorID As Integer
    Get
      Return System.Web.HttpContext.Current.Session("AppErrorID")
    End Get
  End Property

  Public ReadOnly Property AppErrorText As String
    Get
      Return System.Web.HttpContext.Current.Session("AppErrorText")
    End Get
  End Property

End Class
