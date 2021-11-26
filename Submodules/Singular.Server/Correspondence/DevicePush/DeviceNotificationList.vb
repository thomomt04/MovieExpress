' Generated 10 Mar 2014 09:54 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace DevicePush


  <Serializable()> _
  Public Class DeviceNotificationList
    Inherits SingularBusinessListBase(Of DeviceNotificationList, DeviceNotification)


#Region "  Business Methods  "

    Public Function GetItem(DeviceNotificationID As Integer) As DeviceNotification

      For Each child As DeviceNotification In Me
        If child.DeviceNotificationID = DeviceNotificationID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "s"

    End Function

#End Region

#Region "  Data Access  "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)


      Public Sub New()


      End Sub

    End Class

#Region "  Common  "

    Public Shared Function NewDeviceNotificationList() As DeviceNotificationList

      Return New DeviceNotificationList()

    End Function

    Public Shared Sub BeginGetDeviceNotificationList(CallBack As EventHandler(Of DataPortalResult(Of DeviceNotificationList)))

      Dim dp As New DataPortal(Of DeviceNotificationList)()
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria())

    End Sub

    Public Sub New()

      ' must have parameter-less constructor

    End Sub

#End Region

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Public Shared Function GetDeviceNotificationList() As DeviceNotificationList

      Return DataPortal.Fetch(Of DeviceNotificationList)(New Criteria())

    End Function

    Private Sub Fetch(sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(DeviceNotification.GetDeviceNotification(sdr))
      End While
      Me.RaiseListChangedEvents = True

    End Sub

    Protected Overrides Sub DataPortal_Fetch(criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Singular.Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getDeviceNotificationList"
            cm.Parameters.AddWithValue("@UnsentInd", True)
            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As DeviceNotification In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As DeviceNotification In Me
          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

    Protected Overrides Sub DataPortal_Update()

      UpdateTransactional(AddressOf Update)

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace