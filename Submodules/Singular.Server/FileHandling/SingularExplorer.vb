Imports System.IO
Imports Csla
Imports Csla.Serialization
Imports System.ComponentModel.DataAnnotations

Namespace SingularExplorer

  <Serializable()>
  Public Class SingularDirectoryInfo
    Inherits Singular.SingularBusinessBase(Of SingularDirectoryInfo)

    Public Shared DirectoryInfoProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DirectoryInfo)

    <Display(Name:="Directory")>
    Public Property DirectoryInfo() As String
      Get
        Return GetProperty(DirectoryInfoProperty)
      End Get
      Set(value As String)
        SetProperty(DirectoryInfoProperty, value)
      End Set
    End Property

    Public Shared ChildDirectoryListProperty As PropertyInfo(Of SingularDirectoryInfoList) = RegisterProperty(Of SingularDirectoryInfoList)(Function(c) c.ChildDirectoryList)

    <Display(AutoGenerateField:=False)>
    Public Property ChildDirectoryList() As SingularDirectoryInfoList
      Get
        Return GetProperty(ChildDirectoryListProperty)
      End Get
      Set(value As SingularDirectoryInfoList)
        SetProperty(ChildDirectoryListProperty, value)
      End Set
    End Property

    Public Shared FileInfoListProperty As PropertyInfo(Of SingularFileInfoList) = RegisterProperty(Of SingularFileInfoList)(Function(c) c.FileInfoList)

    <Display(AutoGenerateField:=False)>
    Public Property FileInfoList() As SingularFileInfoList
      Get
        Return GetProperty(FileInfoListProperty)
      End Get
      Set(value As SingularFileInfoList)
        SetProperty(FileInfoListProperty, value)
      End Set
    End Property

    Public Sub New()

      MarkAsChild()

    End Sub

  End Class

  <Serializable()>
  Public Class SingularDirectoryInfoList
    Inherits Singular.SingularBusinessListBase(Of SingularDirectoryInfoList, SingularDirectoryInfo)

    Public Sub New()

    End Sub

  End Class

  <Serializable()>
  Public Class SingularFileInfo
    Inherits Singular.SingularBusinessBase(Of SingularFileInfo)

    Public Shared FileNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.FileName)

    <Display(Name:="File")>
    Public Property FileName() As String
      Get
        Return GetProperty(FileNameProperty)
      End Get
      Set(value As String)
        SetProperty(FileNameProperty, value)
      End Set
    End Property

    Public Shared FileSizeProperty As PropertyInfo(Of Decimal) = RegisterProperty(Of Decimal)(Function(c) c.FileSize)

    <Display(Name:="Size"),
    Singular.DataAnnotations.FileSizeField()>
    Public Property FileSize() As Decimal
      Get
        Return GetProperty(FileSizeProperty)
      End Get
      Set(value As Decimal)
        SetProperty(FileSizeProperty, value)
      End Set
    End Property

    Public Shared PathProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Path)

    <Display(AutoGenerateField:=False)>
    Public Property Path() As String
      Get
        Return GetProperty(PathProperty)
      End Get
      Set(value As String)
        SetProperty(PathProperty, value)
      End Set
    End Property

    Public Sub New(ByVal Name As String, ByVal Size As Decimal, ByVal Path As String)

      Me.FileName = Name
      Me.FileSize = Size
      Me.Path = Path
      Me.MarkAsChild()

    End Sub

    Public Sub New()

      MarkAsChild()

    End Sub

  End Class

  <Serializable()>
  Public Class SingularFileInfoList
    Inherits Singular.SingularBusinessListBase(Of SingularFileInfoList, SingularFileInfo)

    Public Sub New()

    End Sub

  End Class

  <Serializable()>
  Public Class CmdFileDelete
    Inherits Singular.CommandBase(Of CmdFileDelete)

    Public Shared FilePathProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.FilePath)

    Public Property FilePath() As String
      Get
        Return ReadProperty(FilePathProperty)
      End Get
      Set(value As String)
        LoadProperty(FilePathProperty, value)
      End Set
    End Property

    Public Shared ErrorsProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Errors)

    Public Property Errors() As String
      Get
        Return ReadProperty(ErrorsProperty)
      End Get
      Set(value As String)
        LoadProperty(ErrorsProperty, value)
      End Set
    End Property

    Public Shared Function NewCmdFileDelete(FilePath As String) As CmdFileDelete

      Return New CmdFileDelete() With {.FilePath = FilePath}

    End Function

    Public Sub New()

    End Sub

#If SILVERLIGHT Then
#Else

    Protected Overrides Sub DataPortal_Execute()

      If FilePath <> "" AndAlso File.Exists(FilePath) Then
        Try
          File.Delete(FilePath)
        Catch ex As Exception
          Errors = ex.ToString
        End Try
      End If

    End Sub

#End If

  End Class

  <Serializable()>
  Public Class CmdFileDirectory
    Inherits Singular.CommandBase(Of CmdFileDirectory)

    Public Shared DirectoryInfoProperty As PropertyInfo(Of SingularDirectoryInfo) = RegisterProperty(Of SingularDirectoryInfo)(Function(c) c.DirectoryInfo)

    Public Property DirectoryInfo() As SingularDirectoryInfo
      Get
        Return ReadProperty(DirectoryInfoProperty)
      End Get
      Set(value As SingularDirectoryInfo)
        LoadProperty(DirectoryInfoProperty, value)
      End Set
    End Property

    Public Shared DirectoryPathProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DirectoryPath)

    Public Property DirectoryPath() As String
      Get
        Return ReadProperty(DirectoryPathProperty)
      End Get
      Set(value As String)
        LoadProperty(DirectoryPathProperty, value)
      End Set
    End Property

    Public Shared ErrorsProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Errors)

    Public Property Errors() As String
      Get
        Return ReadProperty(ErrorsProperty)
      End Get
      Set(value As String)
        LoadProperty(ErrorsProperty, value)
      End Set
    End Property

    Public Shared Function NewCmdFileDirectory(DirectoryPath As String) As CmdFileDirectory

      Return New CmdFileDirectory() With {.DirectoryPath = DirectoryPath}

    End Function

    Public Sub New()

    End Sub

#If SILVERLIGHT Then
#Else

    Protected Overrides Sub DataPortal_Execute()
      Try

        If Directory.Exists(DirectoryPath) Then
          DirectoryInfo = New SingularDirectoryInfo
          Dim di As System.IO.DirectoryInfo = New System.IO.DirectoryInfo(DirectoryPath)
          DirectoryInfo.DirectoryInfo = di.Name
          DirectoryInfo.ChildDirectoryList = GetChildDirectoryList(di)
        End If
      Catch e As Exception
        Throw e
      End Try

    End Sub

    Private Function GetChildDirectoryList(ByVal DirectoryInfo As System.IO.DirectoryInfo) As SingularDirectoryInfoList

      Dim ret As New SingularDirectoryInfoList

      For Each di In DirectoryInfo.EnumerateDirectories()
        Dim sdi As New SingularDirectoryInfo
        sdi.DirectoryInfo = di.Name
        sdi.ChildDirectoryList = GetChildDirectoryList(di)
        sdi.FileInfoList = GetChildFileList(di)
        ret.Add(sdi)
      Next

      Return ret

    End Function

    Private Function GetChildFileList(ByVal DirectoryInfo As System.IO.DirectoryInfo) As SingularFileInfoList

      Dim ret As New SingularFileInfoList

      For Each fi In DirectoryInfo.EnumerateFiles()
        ret.Add(New SingularFileInfo(fi.Name, fi.Length, fi.FullName))
      Next

      Return ret

    End Function

#End If

  End Class
End Namespace