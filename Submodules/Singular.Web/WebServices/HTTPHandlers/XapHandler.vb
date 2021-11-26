Imports System.Web.Security

Namespace WebServices

  Public MustInherit Class XapHandler
    Implements IHttpHandler

    Public Class XapSettings

      Public Property ProtectAllXapFiles As Boolean = True

      Private mXapFileList As New List(Of String)

      Public Sub ProtectXapFile(FileName As String)
        ProtectAllXapFiles = False
        If Not mXapFileList.Contains(FileName) Then
          mXapFileList.Add(FileName)
        End If
      End Sub

      Public ReadOnly Property ProtectedXapFiles As List(Of String)
        Get
          Return mXapFileList
        End Get
      End Property

      Public Function IsFileProtected(Path As String) As Boolean
        If ProtectAllXapFiles Then
          Return True
        Else
          Dim FileName As String = IO.Path.GetFileName(Path)
          If ProtectedXapFiles.Contains(FileName) Then
            Return True
          End If
        End If
        Return False
      End Function

    End Class

    Private Shared mSettings As New XapSettings
    Public Shared ReadOnly Property Settings As XapSettings
      Get
        Return mSettings
      End Get
    End Property

    Public ReadOnly Property IsReusable As Boolean Implements System.Web.IHttpHandler.IsReusable
      Get
        Return True
      End Get
    End Property

    Public Sub ProcessRequest(context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
      If Not context.User.Identity.IsAuthenticated Then

        If Settings.IsFileProtected(context.Request.Path) Then
          FormsAuthentication.RedirectToLoginPage()
        End If

      Else
        context.Response.WriteFile(context.Request.Path)
      End If
    End Sub


  End Class

End Namespace


