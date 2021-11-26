' Generated 10 Mar 2014 09:54 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Singular.DevicePush.DeviceMessageSender

#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace DevicePush


  <Serializable()> _
  Public Class DeviceNotification
    Inherits SingularBusinessBase(Of DeviceNotification)


#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared DeviceNotificationIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DeviceNotificationID, "ID")
    ''' <summary>
    ''' Gets and sets the ID value
    ''' </summary>
    <Display(AutoGenerateField:=False),
    Required(ErrorMessage:="ID required")>
    Public Property DeviceNotificationID() As Integer?
      Get
        Return GetProperty(DeviceNotificationIDProperty)
      End Get
      Set(ByVal Value As Integer?)
        SetProperty(DeviceNotificationIDProperty, Value)
      End Set
    End Property

    Public Shared MessageProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Message, "Message")
    ''' <summary>
    ''' Gets and sets the Message value
    ''' </summary>
    <Display(Name:="Message", Description:=""),
    StringLength(255, ErrorMessage:="Message cannot be more than 255 characters")>
    Public Property Message() As String
      Get
        Return GetProperty(MessageProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(MessageProperty, Value)
      End Set
    End Property

    Public Shared SendIDProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SendID, "Send")
    ''' <summary>
    ''' Gets and sets the Send value
    ''' </summary>
    <Display(Name:="Send", Description:=""),
    StringLength(255, ErrorMessage:="Send cannot be more than 255 characters")>
    Public Property SendID() As String
      Get
        Return GetProperty(SendIDProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(SendIDProperty, Value)
      End Set
    End Property

    Public Shared DeviceOSProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DeviceOS, "Device OS")
    ''' <summary>
    ''' Gets and sets the Device OS value
    ''' </summary>
    <Display(Name:="Device OS", Description:=""),
    Required(ErrorMessage:="Device OS required")>
    Public Property DeviceOS() As DeviceOS
      Get
        Return GetProperty(DeviceOSProperty)
      End Get
      Set(ByVal Value As DeviceOS)
        SetProperty(DeviceOSProperty, Value)
      End Set
    End Property

    Public Shared SentDateProperty As PropertyInfo(Of DateTime?) = RegisterProperty(Of DateTime?)(Function(c) c.SentDate, "Sent Date")
    ''' <summary>
    ''' Gets and sets the Send value
    ''' </summary>
    <Display(Name:="Send", Description:="")>
    Public ReadOnly Property SentDate() As DateTime?
      Get
        Return GetProperty(SentDateProperty)
      End Get
    End Property

    Public Shared ResultProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Result, "Result")
    ''' <summary>
    ''' Gets and sets the Send value
    ''' </summary>
    <Display(Name:="Send", Description:="")>
    Public ReadOnly Property Result() As String
      Get
        Return GetProperty(ResultProperty)
      End Get
    End Property

#End Region

#Region "  Methods  "

    Public Sub SetResult(Result As String)
      SetProperty(SentDateProperty, Now)
      If Result Is Nothing Then
        Result = ""
      End If
      SetProperty(ResultProperty, Result)
    End Sub

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(DeviceNotificationIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.Message.Length = 0 Then
        If Me.IsNew Then
          Return String.Format("New {0}", "Device Notification")
        Else
          Return String.Format("Blank {0}", "Device Notification")
        End If
      Else
        Return Me.Message
      End If

    End Function

#End Region

#End Region

#Region "  Validation Rules  "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region "  Data Access & Factory Methods  "

#Region "  Common  "

    Public Shared Function NewDeviceNotification() As DeviceNotification

      Return DataPortal.CreateChild(Of DeviceNotification)()

    End Function

    Public Sub New()

      MarkAsChild()

    End Sub

#End Region

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Friend Shared Function GetDeviceNotification(dr As SafeDataReader) As DeviceNotification

      Dim d As New DeviceNotification()
      d.Fetch(dr)
      Return d

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(DeviceNotificationIDProperty, .GetInt32(0))
          LoadProperty(MessageProperty, .GetString(1))
          LoadProperty(SendIDProperty, .GetString(2))
          LoadProperty(DeviceOSProperty, .GetInt32(3))
        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insDeviceNotification"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updDeviceNotification"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(cm As SqlCommand)

      If Me.IsSelfDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramDeviceNotificationID As SqlParameter = .Parameters.Add("@DeviceNotificationID", SqlDbType.Int)
          paramDeviceNotificationID.Value = GetProperty(DeviceNotificationIDProperty)
          If Me.IsNew Then
            paramDeviceNotificationID.Direction = ParameterDirection.Output
            .Parameters.AddWithValue("@Message", GetProperty(MessageProperty))
            .Parameters.AddWithValue("@DeviceOS", GetProperty(DeviceOSProperty))
          End If

          .Parameters.AddWithValue("@SendID", GetProperty(SendIDProperty))
          .Parameters.AddWithValue("@SentDate", GetProperty(SentDateProperty))
          .Parameters.AddWithValue("@Result", Result.Substring(0, Math.Min(240, Result.Length)))

          .ExecuteNonQuery()

          If Me.IsNew Then
            LoadProperty(DeviceNotificationIDProperty, paramDeviceNotificationID.Value)
          End If
          ' update child objects
          ' mChildList.Update()
          MarkOld()
        End With
      Else
      End If

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delDeviceNotification"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@DeviceNotificationID", GetProperty(DeviceNotificationIDProperty))
        DoDeleteChild(cm)
      End Using

    End Sub

    Protected Overrides Sub DeleteFromDB(cm As SqlCommand)

      If Me.IsNew Then Exit Sub

      cm.ExecuteNonQuery()
      MarkNew()

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace