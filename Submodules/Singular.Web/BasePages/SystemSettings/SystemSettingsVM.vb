Namespace SystemSettings

  Public Class SystemSettingsVM
    Inherits Singular.Web.StatelessViewModel(Of SystemSettingsVM)

    <System.ComponentModel.Browsable(False)>
    Public Property SystemSettingList As Singular.SystemSettings.Objects.SystemSettingList
    Public Property SystemSetting As Singular.SystemSettings.ISettingsSection

    Public SelectInstance As Singular.SystemSettings.ISettingsSection

    Protected Overrides Sub Setup()

      Dim SettingName As String = Page.Request.QueryString("Name")

      If SettingName IsNot Nothing Then
        'Editing Setting

        Dim InstanceName = Page.Request.QueryString("Inst")

        If InstanceName Is Nothing Then
          SelectInstance = Singular.SystemSettings.GetDefaultInstance(SettingName)
          If SelectInstance.GetInstanceNames Is Nothing Then
            SelectInstance = Nothing
          End If
        End If

        If SelectInstance Is Nothing Then
          'There are no instances, or the instance has been specified.
          'Can now fetch the specific setting object.

          SystemSetting = GetSystemSettingList(SettingName, InstanceName).First.Settings
          If Not String.IsNullOrEmpty(SystemSetting.SecurityRole) AndAlso Not Singular.Security.HasAccess(SystemSetting.SecurityRole) Then
            Me.SystemSetting = Nothing
          End If
        End If

      End If

      If Me.SystemSetting Is Nothing AndAlso SelectInstance Is Nothing Then
        'Showing all Settings
        SystemSettingList = GetSystemSettingList(Nothing, Nothing)

        If SystemSettingList.Count = 1 Then
          SystemSetting = SystemSettingList(0).Settings
        End If

      End If

    End Sub

    Protected Overridable Function GetSystemSettingList(SettingName As String, InstanceName As String) As Singular.SystemSettings.Objects.SystemSettingList
      Return Singular.SystemSettings.Objects.SystemSettingList.GetSystemSettingList(SettingName, InstanceName)
    End Function

    Public Function SaveSetting(Setting As Object, SettingsType As String) As Singular.Web.SaveResult

      AssertAccess()

      Dim CustomSettingType As Type = Type.GetType(SettingsType)
      Dim ssl = Singular.SystemSettings.Objects.SystemSettingList.GetSystemSettingList(CustomSettingType, Setting.InstanceName)
      ssl(0).Settings.ReturnUnMaskedPassword = False
      Dim serialiser As New Singular.Web.Data.JS.StatelessJSSerialiser(ssl(0).Settings)
      ssl(0).Settings.ReturnUnMaskedPassword = True
      Setting.ReturnUnMaskedPassword = True
      serialiser.Deserialise(Setting)
      ssl.PrepareToSave()

      Dim sh = TrySave(ssl)
      If sh.Success Then
        Singular.SystemSettings.ResetSettings(CustomSettingType)
        AfterSave()
      End If

      ssl(0).Settings.ReturnUnMaskedPassword = False

      Return New Singular.Web.SaveResult(sh) With {.Data = ssl(0).Settings}

    End Function

    Protected Overridable Sub AfterSave()

    End Sub

  End Class

End Namespace

