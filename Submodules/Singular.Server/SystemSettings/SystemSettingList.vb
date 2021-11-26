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
  Public Class SystemSettingList
    Inherits SingularBusinessListBase(Of SystemSettingList, SystemSetting)

#Region "  Business Methods  "
#If Silverlight = False Then

    ''' <summary>
    ''' Checks for differences in each system setting, and populates the system settings values list from the system setting class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PrepareToSave()

      For Each ss As SystemSetting In Me
        Dim ssLocal = ss
				ss.InstanceName = ss.Settings.InstanceName

				ss.Settings.GetType.ForEachBrowsableProperty(
					Nothing,
					Sub(pi)

						Dim ssv = ssLocal.SystemSettingValueList.Find(pi.Name)
						If ssv Is Nothing Then
							'Create the value
							ssv = ssLocal.SystemSettingValueList.AddNew
							ssv.PropertyName = pi.Name
						End If
						' sdfgadf()

						If Attribute.IsDefined(pi, GetType(SystemSettings.EncryptedField)) Then
							ssv.IsEncrypted = True
						End If

						ssv.PropertyValue = ssLocal.Settings.GetProperty(pi)

					End Sub, , , True)
			Next

		End Sub


		''' <summary>
		''' Populates all registered settings classes.
		''' </summary>
		Friend Sub PopulateSettings(FilterName As String)

			For Each si In SystemSettings.SettingsTypeList

				Dim ss As ISettingsSection = Activator.CreateInstance(si.Type) 'Always create new instance.
				If String.IsNullOrEmpty(FilterName) OrElse ss.Name = FilterName Then
					PopulateSettings(ss)
				End If

			Next

		End Sub

		''' <summary>
		''' Populates the provided settings class.
		''' </summary>
		Friend Sub PopulateSettings(SingleSection As ISettingsSection)

			Dim ss = Find(SingleSection.Name)
			If ss Is Nothing Then
				'there was no record in the database, so create one
				ss = SystemSetting.NewSystemSetting(SingleSection.Name)
				Add(ss)
			Else
				'Populate the settings class from the values in the database.
				For Each ssv As SystemSettingValue In ss.SystemSettingValueList

					'Dim pi = Singular.Reflection.GetProperty(SingleSection.GetType, ssv.PropertyName)
					'If pi IsNot Nothing AndAlso pi.CanWrite Then
					'  pi.SetValue(SingleSection, ssv.PropertyValue, Nothing)
					'End If
					SingleSection.SetProperty(ssv.PropertyName, ssv.PropertyValue)
				Next

			End If

			CType(SingleSection, ISingularBusinessBase).MarkOld()

			ss.SetSettings(SingleSection)

		End Sub

#End If

		Public Function Find(SystemSettingName As String) As SystemSetting

			For Each child As SystemSetting In Me
				If child.SystemSetting = SystemSettingName Then
					Return child
				End If
			Next
			Return Nothing

		End Function

		Public Function GetItem(SystemSettingID As String) As SystemSetting

			For Each child As SystemSetting In Me
				If child.SystemSettingID = SystemSettingID Then
					Return child
				End If
			Next
			Return Nothing

		End Function

		Public Overrides Function ToString() As String

			Return "System Settings"

		End Function

#If Silverlight = False Then

		Public Function GetSystemSettingValue(SystemSettingValueID As Integer) As SystemSettingValue

			Dim obj As SystemSettingValue = Nothing
			For Each parent As SystemSetting In Me
				obj = parent.SystemSettingValueList.GetItem(SystemSettingValueID)
				If obj IsNot Nothing Then
					Return obj
				End If
			Next
			Return Nothing

		End Function

#End If

#End Region

#Region "  Data Access  "

		<Serializable()>
		Public Class Criteria
			Inherits CriteriaBase(Of Criteria)

			Public Property SettingName As String = ""
			Public Property InstanceName As String = Nothing

			Public Sub New()


			End Sub

#If Silverlight = False Then

			Property SettingsSection As ISettingsSection

#End If
		End Class

#Region "  Common  "

		Public Shared Function NewSystemSettingList() As SystemSettingList

			Return New SystemSettingList()

		End Function

		Public Shared Sub BeginGetSystemSettingList(CallBack As EventHandler(Of DataPortalResult(Of SystemSettingList)))

			Dim dp As New DataPortal(Of SystemSettingList)()
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

		Public Shared Function GetSystemSettingList() As SystemSettingList

			Dim ssl = DataPortal.Fetch(Of SystemSettingList)(New Criteria())
			Return ssl

		End Function

		''' <summary>
		''' Populates the provided settings class with the settings in the database.
		''' </summary>
		Public Shared Function GetSystemSettingList(Of Type As ISettingsSection)() As SystemSettingList

			Return GetSystemSettingList(GetType(Type))

		End Function

		Public Shared Function GetSystemSettingList(SettingName As String, Optional InstanceName As String = Nothing) As SystemSettingList
			Return DataPortal.Fetch(Of SystemSettingList)(New Criteria() With {.SettingName = SettingName, .InstanceName = InstanceName})
		End Function

		Public Shared Function GetSystemSettingList(SettingType As Type, Optional InstanceName As String = Nothing) As SystemSettingList

			Dim ss As ISettingsSection = Activator.CreateInstance(SettingType)
			Dim ssl = DataPortal.Fetch(Of SystemSettingList)(New Criteria() With {.SettingName = ss.Name, .SettingsSection = ss, .InstanceName = InstanceName})
			'ssl.PopulateSettings(ss)
			Return ssl

		End Function

		Private Sub Fetch(sdr As SafeDataReader)

			Me.RaiseListChangedEvents = False
			While sdr.Read
				Me.Add(SystemSetting.GetSystemSetting(sdr))
			End While
			Me.RaiseListChangedEvents = True

			Dim parent As SystemSetting = Nothing
			If sdr.NextResult() Then
				While sdr.Read
					If parent Is Nothing OrElse parent.SystemSettingID <> sdr.GetInt32(1) Then
						parent = Me.GetItem(sdr.GetInt32(1))
					End If
					parent.SystemSettingValueList.RaiseListChangedEvents = False
					parent.SystemSettingValueList.Add(SystemSettingValue.GetSystemSettingValue(sdr))
					parent.SystemSettingValueList.RaiseListChangedEvents = True
				End While
			End If

			' Decrypt Encrypted Fields
			For Each ssType In SystemSettings.SettingsTypeList
				Dim pis = ssType.Type.GetProperties(System.Reflection.BindingFlags.Public + System.Reflection.BindingFlags.Instance)
				Dim encryptedProperties = pis.Where(Function(pi) Attribute.IsDefined(pi, GetType(SystemSettings.EncryptedField))).ToList()

				If encryptedProperties.Count > 0 Then
					' find parent system settings object the property belongs to
					' instantiate to work out its name
					Dim ss = Me.Find(ssType.Instance.Name)

					If ss IsNot Nothing Then
						For Each ssv In (From ssvx In ss.SystemSettingValueList
														 Join ep In encryptedProperties On ep.Name Equals ssvx.PropertyName
														 Select SystemSettingsValue = ssvx, EncryptedProperty = ep).ToList()
							If Not ssv.EncryptedProperty.PropertyType.Equals(GetType(String)) Then
								Throw New Exception("Encryption only currently supported on String values")
							End If
							' if there is a value then decrypt it
							If ssv.SystemSettingsValue.PropertyValue IsNot Nothing Then
								If (ssv.SystemSettingsValue.PropertyValue.GetType.Equals(GetType(String)) AndAlso SystemSettings.DecryptStringTextHandler Is Nothing) Then
									Throw New Exception("DecryptStringTextHandler or DecryptStringTextHandler must be specified on SystemSettings (e.g. CS - Singular.SystemSettings.General.DecryptStringTextHandler = Singular.Encryption.DecryptString; VB: Singular.SystemSettings.DecryptTextHandler = Singular.Encryption.DecryptString)")
								End If
								If (ssv.SystemSettingsValue.PropertyValue.GetType.Equals(GetType(Byte())) AndAlso SystemSettings.DecryptByteTextHandler Is Nothing) Then
									Throw New Exception("DecryptByteTextHandler or DecryptStringTextHandler must be specified on SystemSettings (e.g. CS - Singular.SystemSettings.General.DecryptTextHandler = Singular.Encryption.DecryptString; VB: Singular.SystemSettings.DecryptTextHandler = Singular.Encryption.DecryptString)")
								End If
								ssv.SystemSettingsValue.IsEncrypted = True
								Try
									If ssv.SystemSettingsValue.PropertyValue.GetType.Equals(GetType(String)) Then
										ssv.SystemSettingsValue.PropertyValue = SystemSettings.DecryptStringTextHandler.Invoke(ssv.SystemSettingsValue.PropertyValue)
									Else
										ssv.SystemSettingsValue.PropertyValue = SystemSettings.DecryptByteTextHandler.Invoke(ssv.SystemSettingsValue.PropertyValue)
									End If
								Catch ex As Exception
									ssv.SystemSettingsValue.PropertyValue = ""
								End Try
							End If

						Next
					End If
					' find system settings values that match encrypted property

				End If
			Next

			For Each child As SystemSetting In Me
				child.CheckRules()
				For Each SystemSettingValue As SystemSettingValue In child.SystemSettingValueList
					SystemSettingValue.CheckRules()
				Next
			Next

		End Sub

		Protected Overrides Sub DataPortal_Fetch(criteria As Object)

			Dim crit As Criteria = criteria
			Using cn As New SqlConnection(Settings.ConnectionString)
				cn.Open()
				Try
					Using cm As SqlCommand = cn.CreateCommand
						cm.CommandType = CommandType.StoredProcedure
						cm.CommandText = "GetProcs.getSystemSettingList"
						cm.Parameters.AddWithValue("@SystemSetting", crit.SettingName)
						If Not String.IsNullOrEmpty(crit.InstanceName) Then
							cm.Parameters.AddWithValue("@InstanceName", crit.InstanceName)
						End If

						If AddExtraParameters IsNot Nothing Then
							AddExtraParameters(cm)
						End If

						Using sdr As New SafeDataReader(cm.ExecuteReader)
							Fetch(sdr)
						End Using
					End Using

					If crit.SettingsSection IsNot Nothing Then
						PopulateSettings(crit.SettingsSection)
					Else
						PopulateSettings(crit.SettingName)
					End If

					If Me.Count = 1 Then
						Me(0).Settings.InstanceName = crit.InstanceName
					End If

				Finally
					cn.Close()
				End Try
			End Using

		End Sub

		Public Shared AddExtraParameters As Action(Of SqlCommand)


		Friend Sub Update()

			Me.RaiseListChangedEvents = False
			Try
				' Loop through each deleted child object and call its Update() method
				For Each Child As SystemSetting In DeletedList
					Child.DeleteSelf()
				Next

				' Then clear the list of deleted objects because they are truly gone now.
				DeletedList.Clear()

				' Loop through each non-deleted child object and call its Update() method
				For Each Child As SystemSetting In Me
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

			CommonData.Refresh(GetType(SystemSettingList))

		End Sub

#End If

#End Region

#End Region

  End Class

End Namespace