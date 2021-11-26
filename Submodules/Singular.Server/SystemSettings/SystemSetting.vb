' Generated 25 Jan 2013 12:51 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace SystemSettings.Objects


  <Serializable()>
  Public Class SystemSetting
    Inherits SingularBusinessBase(Of SystemSetting)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared SystemSettingIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SystemSettingID, "System Setting", 0)
    ''' <summary>
    ''' Gets the System Setting value
    ''' </summary>
    <Display(AutoGenerateField:=False), Key>
    Public ReadOnly Property SystemSettingID() As Integer
      Get
        Return GetProperty(SystemSettingIDProperty)
      End Get
    End Property

    Public Shared SystemSettingProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SystemSetting, "System Setting", "")
    ''' <summary>
    ''' Gets and sets the System Setting value
    ''' </summary>
    <Display(Name:="System Setting", Description:="Class name that holds the properties defined by the system setting values."),
    StringLength(50, ErrorMessage:="System Setting cannot be more than 50 characters")>
    Public ReadOnly Property SystemSetting() As String
      Get
        Return GetProperty(SystemSettingProperty)
      End Get
    End Property

    Public Shared SettingsProperty As PropertyInfo(Of ISettingsSection) = RegisterProperty(Of ISettingsSection)(Function(c) c.Settings, "Settings")

    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property Settings As ISettingsSection
      Get
        Return GetProperty(SettingsProperty)
      End Get
    End Property

    Public InstanceName As String

#End Region

#Region "  Child Lists  "

#If Silverlight = False Then

    Public Shared SystemSettingValueListProperty As PropertyInfo(Of SystemSettingValueList) = RegisterProperty(Of SystemSettingValueList)(Function(c) c.SystemSettingValueList, "System Setting Value List")

    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property SystemSettingValueList() As SystemSettingValueList
      Get
        If GetProperty(SystemSettingValueListProperty) Is Nothing Then
          LoadProperty(SystemSettingValueListProperty, SystemSettings.Objects.SystemSettingValueList.NewSystemSettingValueList())
        End If
        Return GetProperty(SystemSettingValueListProperty)
      End Get
    End Property

#End If

#End Region

#Region "  Methods  "

    Friend Sub SetSettings(Settings As ISettingsSection)
      LoadProperty(SettingsProperty, Settings)
    End Sub

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SystemSettingIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.SystemSetting.Length = 0 Then
        If Me.IsNew Then
          Return String.Format("New {0}", "System Setting")
        Else
          Return String.Format("Blank {0}", "System Setting")
        End If
      Else
        Return Me.SystemSetting
      End If

    End Function

    Protected Overrides ReadOnly Property TableReferencesToIgnore() As String()
      Get
        Return New String() {"SystemSettingValues"}
      End Get
    End Property

#End Region

#End Region

#Region "  Validation Rules  "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region "  Data Access & Factory Methods  "

#Region "  Common  "

    Public Shared Function NewSystemSetting(SettingName As String) As SystemSetting

      Dim ss = DataPortal.CreateChild(Of SystemSetting)()
      ss.LoadProperty(SystemSettingProperty, SettingName)
      Return ss

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

    Friend Shared Function GetSystemSetting(dr As SafeDataReader) As SystemSetting

      Dim s As New SystemSetting()
      s.Fetch(dr)
      Return s

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(SystemSettingIDProperty, .GetInt32(0))
          LoadProperty(SystemSettingProperty, .GetString(1))

          If sdr.FieldCount > 2 Then
            InstanceName = .GetString(2)
          End If
        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insSystemSetting"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updSystemSetting"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Public Shared AddExtraUpdateParameters As Action(Of SqlCommand, SystemSetting)

    Protected Overrides Sub InsertUpdate(cm As SqlCommand)

      If IsDirty Then

        If Me.IsSelfDirty Then

          With cm
            .CommandType = CommandType.StoredProcedure

            Dim paramSystemSettingID As SqlParameter = .Parameters.Add("@SystemSettingID", SqlDbType.Int)
            paramSystemSettingID.Value = GetProperty(SystemSettingIDProperty)
            If Me.IsNew Then
              paramSystemSettingID.Direction = ParameterDirection.Output
            End If
            .Parameters.AddWithValue("@SystemSetting", GetProperty(SystemSettingProperty))
            If Not String.IsNullOrEmpty(InstanceName) Then
              .Parameters.AddWithValue("@InstanceName", InstanceName)
            End If

            If AddExtraUpdateParameters IsNot Nothing Then
              AddExtraUpdateParameters(cm, Me)
            End If

            .ExecuteNonQuery()

            If Me.IsNew Then
              LoadProperty(SystemSettingIDProperty, paramSystemSettingID.Value)
            End If
            ' update child objects
            If GetProperty(SystemSettingValueListProperty) IsNot Nothing Then
              Me.SystemSettingValueList.Update()
            End If
            MarkOld()
          End With
        Else
          ' update child objects
          If GetProperty(SystemSettingValueListProperty) IsNot Nothing Then
            Me.SystemSettingValueList.Update()
          End If
        End If

        Service.NotifyService(Service.ServiceUpdateMessageType.SystemSettingsUpdated, Singular.Reflection.GetTypeFullName(Settings.GetType) & "|" & InstanceName)

      End If


    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delSystemSetting"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@SystemSettingID", GetProperty(SystemSettingIDProperty))
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