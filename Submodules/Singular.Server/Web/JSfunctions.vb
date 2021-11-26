Namespace Web

  Public Class SingularJSFunction
    Inherits Attribute

  End Class

  ''' <summary>
  ''' Applying this attribute on a string property indicates that the string already contains JSON data. It wont be re serialised into JSon.
  ''' Applying it on a parameter indicates that the string version of the json object should be passed in.
  ''' </summary>
  ''' <remarks></remarks>
  <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Parameter)>
  Public Class JSonString
    Inherits Attribute

  End Class

  Public Class JSfunctions

    <SingularJSFunction()>
    Public Shared Function DoPostBack(Target As String, Parameter As String)
      Throw New Exception("JSfunctions are only placeholders, and hold no logic.")
    End Function

    <SingularJSFunction()>
    Public Shared Function SendCommand(CommandName As String, Parameter As String)
      Throw New Exception("JSfunctions are only placeholders, and hold no logic.")
    End Function

    <SingularJSFunction()>
    Public Shared Function SendROCommand(CommandName As String, Parameter As String)
      Throw New Exception("JSfunctions are only placeholders, and hold no logic.")
    End Function

    <SingularJSFunction()>
    Public Shared Function DownloadFile(Guid As String)
      Throw New Exception("JSfunctions are only placeholders, and hold no logic.")
    End Function

    <SingularJSFunction()>
    Public Shared Function DownloadPath() As String
      Throw New Exception("JSfunctions are only placeholders, and hold no logic.")
    End Function

    <SingularJSFunction()>
    Public Shared Function RootPath() As String
      Throw New Exception("JSfunctions are only placeholders, and hold no logic.")
    End Function

    Public Shared Property Guid As String

    Public Shared Function GetParent(Of T)() As T

    End Function

  End Class

End Namespace


