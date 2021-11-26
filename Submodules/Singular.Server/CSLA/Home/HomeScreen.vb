Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
Imports System.Windows.Media.Imaging
#End If

#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CSLALib

  <Serializable()> _
  Public Class HomeScreen
    Inherits SingularBusinessBase(Of HomeScreen)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared UserIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.UserID, "ID", 0)
    ''' <Summary>
    ''' Gets and sets the ID value
    ''' </Summary>
    <Display(Name:="ID", Description:="Current User of the System"),
    Required(ErrorMessage:="ID required")> _
    Public Property UserID() As Integer
      Get
        Return GetProperty(UserIDProperty)
      End Get
      Set(ByVal Value As Integer)
        SetProperty(UserIDProperty, Value)
      End Set
    End Property

    Public Shared UserNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.UserName, "ID", "")
    ''' <Summary>
    ''' Gets and sets the ID value
    ''' </Summary>
    <Display(Name:="ID", Description:="Current User of the System"),
    Required(ErrorMessage:="ID required")> _
    Public Property UserName() As String
      Get
        Return GetProperty(UserNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(UserNameProperty, Value)
      End Set
    End Property

    Public Shared ApplicationNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.ApplicationName, "Application Name", "")
    ''' <Summary>
    ''' Gets and sets the Application Name value
    ''' </Summary>
    <Display(Name:="Application Name", Description:="Application Name")> _
    Public Property ApplicationName() As String
      Get
        Return GetProperty(ApplicationNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(ApplicationNameProperty, Value)
      End Set
    End Property

#If SILVERLIGHT Then

    Public Shared ApplicationImageProperty As PropertyInfo(Of BitmapImage) = RegisterProperty(Of BitmapImage)(Function(c) c.ApplicationImage, "Application Image")
    ''' <Summary>
    ''' Gets and sets the Application Name value
    ''' </Summary>
    <Display(Name:="Application Image", Description:="Application Image")> _
    Public Property ApplicationImage() As BitmapImage
      Get
        Return GetProperty(ApplicationImageProperty)
      End Get
      Set(ByVal Value As BitmapImage)
        SetProperty(ApplicationImageProperty, Value)
      End Set
    End Property

#End If

    Public Shared WelcomeTextProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.WelcomeText, "Welcome Text", "Welcome %s")
    ''' <Summary>
    ''' Gets and sets the Welcome Text value
    ''' </Summary>
    <Display(Name:="Welcome Text", Description:="WelcomeText")> _
    Public Property WelcomeText() As String
      Get
        Return GetProperty(WelcomeTextProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(WelcomeTextProperty, Value)
      End Set
    End Property

#End Region

#Region " Child Objects "

    Public Shared UserProperty As PropertyInfo(Of Security.User) = RegisterProperty(Of Security.User)(Function(c) c.User, "User")

    Public Property User() As Security.User
      Get
#If SILVERLIGHT Then
        If GetProperty(UserProperty) Is Nothing Then
          LoadProperty(UserProperty, Security.User.NewUser())

          Security.UserList.BeginGetUserList(Me.UserID,
                                                            Sub(o, e)
                                                              If e.Error IsNot Nothing Then
                                                                Throw e.Error
                                                              End If
                                                              Me.User = e.Object(0)
                                                              OnPropertyChanged(UserProperty)
                                                            End Sub)
        End If
#End If
        Return GetProperty(UserProperty)
      End Get
      Private Set(value As Security.User)
        LoadProperty(UserProperty, value)
      End Set
    End Property

#End Region

#Region " Methods "


#End Region

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Shared Function NewHomeScreen() As HomeScreen

      Return DataPortal.CreateChild(Of HomeScreen)()

    End Function

    Public Sub New()

      MarkAsChild()


    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then


#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetHomeScreen(ByVal dr As SafeDataReader) As HomeScreen

      Dim h As New HomeScreen()
      h.Fetch(dr)
      Return h

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insHomeScreen"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updHomeScreen"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      If FieldManager.FieldExists(UserProperty) Then
        Me.User.Update()
      End If
      ' update child objects
      ' mChildList.Update()
      MarkOld()

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace