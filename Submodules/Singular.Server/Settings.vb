Imports System.IO

Public Enum CommonDataPlatform
  [Default]
  [Windows]
  [Web]
  [Silverlight]
End Enum

Public Class Settings

  Public Shared Property CheckTableReferences As Boolean = True

  Public Shared Property CurrentPlatform As CommonDataPlatform = CommonDataPlatform.Default

  Public Shared ReadOnly Property CurrentUser As Singular.Security.IIdentity
    Get
      Return Singular.Security.CurrentIdentity
    End Get
  End Property

  Public Shared ReadOnly Property CurrentUserID As Integer
    Get
      If Singular.Security.CurrentIdentity Is Nothing Then
        Return 0
      Else
        Return Singular.Security.CurrentIdentity.UserID
      End If
    End Get
  End Property

#If SILVERLIGHT Then

  'Private Shared mCurrentUser As Security.User

  'Public Shared ReadOnly Property CurrentUser As Security.User
  '  Get
  '    Return mCurrentUser
  '  End Get
  'End Property

#Else

  Public Overloads Shared ReadOnly Property ConnectionString As String
    Get
      If Not String.IsNullOrEmpty(mOverridingConnectionString) Then
        Return mOverridingConnectionString
      Else
        Return GetConnectionStringFromSettings("Connection")
      End If
    End Get
  End Property

  Private Shared mConnectionString As String = ""
  Private Shared mLastConnectionSectionName As String = ""

  Public Shared ReadOnly Property ServerName(Optional ByVal SectionName As String = "Connection") As String
    Get
      Return GetSetting(SectionName, "ServerName")
    End Get
  End Property


  Public Shared ReadOnly Property DatabaseName(Optional ByVal SectionName As String = "Connection") As String
    Get
      Return GetSetting(SectionName, "DatabaseName")
    End Get
  End Property

  Private Shared mAppExiting As Boolean = False

  Public Shared Property AppExiting() As Boolean
    Get
      Return mAppExiting
    End Get
    Set(ByVal value As Boolean)
      mAppExiting = value
    End Set
  End Property

  Public Shared Sub Refresh()


  End Sub


  Public Shared Sub ResetConnectionString()

    mConnectionString = ""

  End Sub

  Protected Shared Function GetConnectionStringFromSettings(ByVal SectionName As String) As String

    If Csla.ApplicationContext.ExecutionLocation = Csla.ApplicationContext.ExecutionLocations.Server Then
      Try
        Return System.Configuration.ConfigurationManager.ConnectionStrings("DataAccess").ConnectionString()
      Catch ex As Exception
        Throw New Exception("No Connection String found in Config File called 'DataAccess'")
      End Try
    End If

    If (mConnectionString = "" OrElse mConnectionString = "Data Source=;Integrated Security=SSPI;Initial Catalog=" OrElse mLastConnectionSectionName <> SectionName) AndAlso
        Csla.ApplicationContext.DataPortalProxy.Contains("Local") Then

      Dim sServerName As String = ServerName(SectionName)
      Dim sDatabaseName As String = DatabaseName(SectionName)
      Dim sUserName As String = GetSetting(SectionName, "UserName")
      Dim sPassword As String = GetSetting(SectionName, "Password")
      Dim sIntegratedSecurity As String = GetSetting(SectionName, "IntegratedSecurityInd", True)

      If sIntegratedSecurity = "" Then
        sIntegratedSecurity = "True"
      End If
      If sIntegratedSecurity Then
        mConnectionString = Data.Sql.GetConnectionString(sServerName, sDatabaseName)
      Else
        mConnectionString = Data.Sql.GetConnectionString(sServerName, sDatabaseName, sUserName, sPassword)
      End If

      mLastConnectionSectionName = SectionName

      If mConnectionString = "Data Source=;Integrated Security=SSPI;Initial Catalog=" Then
        ' try get it from config file
        If Singular.Debug.InDebugMode Then
          Try
            mConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings(Environment.MachineName).ConnectionString
          Catch ex As Exception
            Try
              mConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("Main").ConnectionString
            Catch ex2 As Exception

            End Try
          End Try
        Else
          mConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("Main").ConnectionString
        End If

      End If
    End If
    Return mConnectionString

  End Function

  Shared Sub New()
  End Sub

  Private Shared mOverridingConnectionString As String = ""

  Public Shared Sub SetConnectionString(ByVal value As String)
    mOverridingConnectionString = value
  End Sub



  Public Shared Sub GetUserID(ByRef UserID As Integer)

    If Singular.Security.CurrentIdentity IsNot Nothing Then
      UserID = Singular.Security.CurrentIdentity.UserID
    Else
      UserID = 0
    End If

  End Sub

#End If

#If SILVERLIGHT Then

#Else

#Region " Get and Set Settings "

  Private Shared mData As DataSet
  Private Shared mLockObject As New Object

  Public Shared Function HasSetting(ByVal SectionName As String, ByVal PropertyName As String) As Boolean

    Return GetSetting(SectionName, PropertyName, Nothing) IsNot Nothing

  End Function

  Public Shared Function GetSetting(ByVal SectionName As String, ByVal PropertyName As String, Optional ByVal DefaultValue As Object = "") As Object
    ' will fetch the property, and return null if it does not exist

    If Csla.ApplicationContext.ExecutionLocation = Csla.ApplicationContext.ExecutionLocations.Server Then
      Return Nothing
      Exit Function
    End If

    SyncLock mLockObject

      Try

        If mData Is Nothing Then
          ' need to load the settings file
          Dim primaryPath As String = GetSettingsPath() & "\Settings.xml"
          Dim secondarySource As Stream = GetSettingStreamFromEmbeddedResource()
          If IO.File.Exists(primaryPath) Then
            mData = New DataSet
            mData.ReadXml(primaryPath)
          ElseIf secondarySource IsNot Nothing Then
            mData = New DataSet
            mData.ReadXml(secondarySource)
          Else
            If Process.GetCurrentProcess.ProcessName <> "w3wp" Then
              'The w3wp process is a process associated with application pool in ISS
              'It tells us that the application is running on a web server and we NEVER want to try and save a settings file. use web.config
              SaveSetting(SectionName, PropertyName, DefaultValue)
            End If
          End If
        End If

        Dim tbl As DataTable
        If Not mData.Tables.Contains(SectionName) Then
          ' we dont have the table so return Nothing
          SaveSetting(SectionName, PropertyName, DefaultValue)
          Return DefaultValue
        Else
          ' just get the table
          tbl = mData.Tables(SectionName)
        End If

        ' now we have the table
        ' check if we have the column
        Dim clm As DataColumn
        If Not tbl.Columns.Contains(PropertyName) Then
          ' we dont have the column so return Nothing
          SaveSetting(SectionName, PropertyName, DefaultValue)
          Return DefaultValue
        Else
          ' we already have the column so just get it
          clm = tbl.Columns(PropertyName)
        End If

        ' now we have the table and the column
        ' get the row
        Dim drw As DataRow
        If tbl.Rows.Count = 0 Then
          ' we dont have the row so return Nothing
          SaveSetting(SectionName, PropertyName, DefaultValue)
          Return DefaultValue
        Else
          ' we already have the row so just get it
          drw = tbl.Rows(0)
        End If

        ' now we have everything so just return the property value
        If PropertyName.ToLower.Contains("password") Then
          Try
            Return Singular.Encryption.DecryptString(drw(clm))
          Catch ex As System.Security.Cryptography.CryptographicException
            'The password is probably not excrypted in the file, so just return it
            Return drw(clm)
          Catch ex As System.FormatException
            Return drw(clm)
          End Try
        Else
          Return drw(clm)
        End If


      Catch ex As Exception
        Throw New Exception("Settings > GetSetting (" & GetSettingsPath() & "): " & ex.Message)
      End Try

    End SyncLock

  End Function


  Public Shared Sub SaveSetting(ByVal SectionName As String, ByVal PropertyName As String, ByVal PropertyValue As Object)

    If Csla.ApplicationContext.ExecutionLocation = Csla.ApplicationContext.ExecutionLocations.Server Then Exit Sub

    If SectionName.ToLower = "settings" Then
      Throw New Exception("Cannot use 'Settings' as a Section Name for a property, it is reserved")
    End If

    If mData Is Nothing Then
      mData = New DataSet("Settings")
    End If

    Dim tbl As DataTable
    If Not mData.Tables.Contains(SectionName) Then
      ' we dont have the table so create it
      tbl = New DataTable(SectionName)
      ' add it to the dataset
      mData.Tables.Add(tbl)
    Else
      ' just get the table
      tbl = mData.Tables(SectionName)
    End If

    ' now we have the table
    ' check if we have the column
    Dim clm As DataColumn
    If Not tbl.Columns.Contains(PropertyName) Then
      ' we dont have the column so create it
      clm = tbl.Columns.Add(PropertyName)
    Else
      ' we already have the column so just get it
      clm = tbl.Columns(PropertyName)
    End If

    ' now we have the table and the column
    ' get the row
    Dim drw As DataRow
    If tbl.Rows.Count = 0 Then
      ' we dont have the row so create it and add it to the table
      drw = tbl.NewRow()
      tbl.Rows.Add(drw)
    Else
      ' we already have the row so just get it
      drw = tbl.Rows(0)
    End If

    ' now we have everything so just set the property
    If PropertyName.ToLower.Contains("password") Then
      drw(clm) = Singular.Encryption.EncryptString(PropertyValue)
    Else
      drw(clm) = PropertyValue
    End If

    ' fix to prevent tables with no column value being saved
    ' corrupts the xml file
    Dim AllFieldsNull As Boolean = True
    For Each clmNull As DataColumn In tbl.Columns
      AllFieldsNull = AllFieldsNull And IsDBNull(drw(clmNull))
    Next

    If AllFieldsNull Then
      mData.Tables.Remove(tbl)
    End If

    Try
      Dim primaryPath As String = GetSettingsPath()
      'Dim secondaryPath As String = Application.StartupPath
      'If Singular.Misc.Debug.InDebugMode Then
      'mData.WriteXml(secondaryPath & "\Settings.xml")
      'Else
      If Not IO.Directory.Exists(primaryPath) Then
        IO.Directory.CreateDirectory(primaryPath)
      End If

      mData.WriteXml(primaryPath & "\Settings.xml")
      'End If
    Catch ex As Exception
      Throw New Exception("Error Saving Settings", ex) '(ex.Message, MsgBoxStyle.Exclamation, "Error Saving Settings")
    End Try

  End Sub

  Public Shared Function GetSettingStreamFromEmbeddedResource() As Stream
    Dim assembly = System.Reflection.Assembly.GetEntryAssembly()
    If assembly IsNot Nothing Then
      Dim resName = assembly.GetManifestResourceNames.FirstOrDefault(Function(rn) rn.EndsWith(".Settings.xml"))
      If resName IsNot Nothing Then
        Return assembly.GetManifestResourceStream(resName)
      End If
    End If
    Return Nothing

  End Function

#End Region

#Region " Connection String "


  Public Shared Sub SetConnectionStringFromSettings(ByVal SectionName As String)

    Dim sServerName As String = GetSetting(SectionName, "ServerName")
    Dim sDatabaseName As String = GetSetting(SectionName, "DatabaseName")
    Dim sUserName As String = GetSetting(SectionName, "UserName")
    Dim sPassword As String = GetSetting(SectionName, "Password")
    Dim sIntegratedSecurity As String = GetSetting(SectionName, "IntegratedSecurityInd", True)

    If sIntegratedSecurity = "" Then
      sIntegratedSecurity = "True"
    End If
    If sIntegratedSecurity Then
      mOverridingConnectionString = Data.Sql.GetConnectionString(sServerName, sDatabaseName)
    Else
      mOverridingConnectionString = Data.Sql.GetConnectionString(sServerName, sDatabaseName, sUserName, sPassword)
    End If

  End Sub

#End Region

#Region " Test Connection "

  Public Shared ReadOnly Property GetDatabasesScript() As String
    Get
      Return My.Resources.GetDatabases
    End Get
  End Property

  Public Enum ConnectionTestResults
    Success
    CantFindServer
    CantFindDatabase
    ProxyAuthenticationRequired407
  End Enum

  ''' <summary>
  ''' Tests the default connection using the settings saved in the settings file
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function TestConnection(ByRef Exception As Exception) As ConnectionTestResults

    Return TestConnection("Connection", Exception)

  End Function

  ''' <summary>
  ''' Tests the default connection using the settings saved in the settings file
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function TestConnection(TimeoutSeconds As Integer) As ConnectionTestResults

    Return TestConnection("Connection", TimeoutSeconds)

  End Function

  ''' <summary>
  ''' Tests the default connection using the settings saved in the settings file
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function TestConnection() As ConnectionTestResults

    Return TestConnection("Connection")

  End Function

  ''' <summary>
  ''' Test the connection using the settings saved in the settings file for a particular section
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function TestConnection(ByVal SectionName As String) As ConnectionTestResults

    Return TestConnection(SectionName, CType(Nothing, Exception))

  End Function

  ''' <summary>
  ''' Test the connection using the settings saved in the settings file for a particular section
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function TestConnection(ByVal SectionName As String, TimeoutSeconds As Integer) As ConnectionTestResults

    Return TestConnection(SectionName, Nothing, TimeoutSeconds)

  End Function

  ''' <summary>
  ''' Test the connection using the settings saved in the settings file for a particular section
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function TestConnection(ByVal SectionName As String, ByRef Exception As Exception) As ConnectionTestResults

    Return TestConnection(SectionName, Exception, 7)

  End Function

  ''' <summary>
  ''' Test the connection using the settings saved in the settings file for a particular section
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Function TestConnection(ByVal SectionName As String, ByRef Exception As Exception, TimeoutSeconds As Integer) As ConnectionTestResults

    Dim sIntegratedSecurity As String = GetSetting(SectionName, "IntegratedSecurityInd", True)

    If sIntegratedSecurity = "" Then
      sIntegratedSecurity = "True"
    End If

    Return TestConnection(GetSetting(SectionName, "ServerName"), GetSetting(SectionName, "DatabaseName"), sIntegratedSecurity, GetSetting(SectionName, "UserName"), GetSetting(SectionName, "Password"), Exception, TimeoutSeconds)

  End Function

  Public Shared Function TestConnection(ByVal Server As String, ByVal Database As String, ByVal Integrated As Boolean, ByVal UserName As String, ByVal Password As String, ByRef Exception As Exception,
                                        Optional TimeoutSeconds As Integer = 7) As ConnectionTestResults

    Try
      If String.IsNullOrEmpty(Server) Then
        Throw New Exception("Error connecting to SQL Server, no Server Name specified")
      End If

      'set the mOverridingConnectionString so it takes preference while testing the connection
      Dim ConnectionString = ""
      If Integrated Then
        ConnectionString = Data.Sql.GetConnectionString(Server, "master") & ";Connect Timeout=" & TimeoutSeconds
      Else
        ConnectionString = Data.Sql.GetConnectionString(Server, "master", UserName, Password) & ";Connect Timeout=" & TimeoutSeconds
      End If

      Using cmd As New SqlClient.SqlCommand("SELECT TOP 1 * FROM dbo.sysdatabases WHERE name = @DBName")
        cmd.CommandType = CommandType.Text
        cmd.CommandTimeout = 5
        cmd.Parameters.AddWithValue("@DBName", Database)

        Using cnn As New SqlClient.SqlConnection(ConnectionString)
          cnn.Open()

          cmd.Connection = cnn


          cmd.ExecuteNonQuery()

        End Using

      End Using

      Return ConnectionTestResults.Success

    Catch ex As Exception
      Exception = ex
      Return ConnectionTestResults.CantFindServer
    Finally
      'clear the mOverridingConnectionString
      '  Singular.Settings.SetConnectionString("")
    End Try

    ' End If

  End Function

  Public Shared Function TestConnection(ByVal Server As String, ByVal Database As String, ByVal Integrated As Boolean, ByVal UserName As String, ByVal Password As String) As ConnectionTestResults

    Return TestConnection(Server, Database, Integrated, UserName, Password, Nothing)

  End Function

#End Region

  Private Shared mOverridingPath As String = ""
  Private Shared mOverridingAppName As String = ""

  ''' <summary>
  ''' Only use this if you really have to. Using "OverridingAppName" is better, as it will still use the normal windows app data path.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shared Property OverridingPath() As String
    Get
      Return mOverridingPath
    End Get
    Set(ByVal value As String)
      mOverridingPath = value
    End Set
  End Property

  Public Shared Property OverridingAppName() As String
    Get
      Return mOverridingAppName
    End Get
    Set(ByVal value As String)
      mOverridingAppName = value
    End Set
  End Property

  Public Shared Function GetSettingsPath() As String
    If OverridingPath <> "" Then
      Return OverridingPath
    ElseIf OverridingAppName <> "" Then
      Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & OverridingAppName
    Else
      Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & My.Application.Info.ProductName
    End If
  End Function

#End If

End Class
