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
  Public Class SystemSettingValue
    Inherits SingularBusinessBase(Of SystemSettingValue)

#Region "  Properties and Methods  "

#Region "  Properties  "

    Public Shared SystemSettingValueIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SystemSettingValueID, "System Setting Value", 0)
    ''' <summary>
    ''' Gets the System Setting Value value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property SystemSettingValueID() As Integer
      Get
        Return GetProperty(SystemSettingValueIDProperty)
      End Get
    End Property

    Public Shared SystemSettingIDProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.SystemSettingID, "System Setting", Nothing)
    ''' <summary>
    ''' Gets the System Setting value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property SystemSettingID() As Integer?
      Get
        Return GetProperty(SystemSettingIDProperty)
      End Get
    End Property

    Public Shared PropertyNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.PropertyName, "Property Name", "")
    ''' <summary>
    ''' Gets and sets the Property Name value
    ''' </summary>
    <Display(Name:="Property Name", Description:=""),
    StringLength(50, ErrorMessage:="Property Name cannot be more than 50 characters")>
    Public Property PropertyName() As String
      Get
        Return GetProperty(PropertyNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(PropertyNameProperty, Value)
      End Set
    End Property

    Public Shared PropertyValueProperty As PropertyInfo(Of Object) = RegisterProperty(Of Object)(Function(c) c.PropertyValue, "Property Value", "")
    ''' <summary>
    ''' Gets and sets the Property Value value
    ''' </summary>
    <Display(Name:="Property Value", Description:="")>
    Public Property PropertyValue() As Object
      Get
        Return GetProperty(PropertyValueProperty)
      End Get
      Set(ByVal Value As Object)
        SetProperty(PropertyValueProperty, Value)
      End Set
    End Property


    Public Shared IsEncryptedProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.IsEncrypted, "Property Value", False)
    ''' <summary>
    ''' Gets and sets the Property Value value
    ''' </summary>
    <Browsable(False)>
    Public Property IsEncrypted() As Boolean
      Get
        Return GetProperty(IsEncryptedProperty)
      End Get
      Set(ByVal Value As Boolean)
        SetProperty(IsEncryptedProperty, Value)
      End Set
    End Property

#End Region

#Region "  Methods  "

    Public Function GetParent() As SystemSetting

      Return CType(CType(Me.Parent, SystemSettingValueList).Parent, SystemSetting)

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SystemSettingValueIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.PropertyName.Length = 0 Then
        If Me.IsNew Then
          Return String.Format("New {0}", "System Setting Value")
        Else
          Return String.Format("Blank {0}", "System Setting Value")
        End If
      Else
        Return Me.PropertyName
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

    Public Shared Function NewSystemSettingValue() As SystemSettingValue

      Return DataPortal.CreateChild(Of SystemSettingValue)()

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

    Friend Shared Function GetSystemSettingValue(dr As SafeDataReader) As SystemSettingValue

      Dim s As New SystemSettingValue()
      s.Fetch(dr)
      Return s

    End Function

    Protected Sub Fetch(sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(SystemSettingValueIDProperty, .GetInt32(0))
          LoadProperty(SystemSettingIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
          LoadProperty(PropertyNameProperty, .GetString(2))

          Dim Value As Object = .GetValue(3)
          If Value Is Nothing Then
            Value = .GetValue(4)
          End If

          ' commented out by BWebber. These lines were preventing the value from being decrypted properly
          'If TypeOf (Value) Is System.Byte() Then
          '  LoadProperty(PropertyValueProperty, System.Text.Encoding.UTF8.GetString(Value))
          'Else
          ' End If
          LoadProperty(PropertyValueProperty, Value)

        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insSystemSettingValue"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updSystemSettingValue"

        DoInsertUpdateChild(cm)
      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(cm As SqlCommand)

      If Me.IsSelfDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramSystemSettingValueID As SqlParameter = .Parameters.Add("@SystemSettingValueID", SqlDbType.Int)
          paramSystemSettingValueID.Value = GetProperty(SystemSettingValueIDProperty)
          If Me.IsNew Then
            paramSystemSettingValueID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@SystemSettingID", Me.GetParent().SystemSettingID)
          .Parameters.AddWithValue("@PropertyName", GetProperty(PropertyNameProperty))
          Dim propValue As Object = Me.PropertyValue
          If Me.IsEncrypted Then
            ' need to decrypt this value
            If SystemSettings.EncryptTextHandler Is Nothing Then
              Throw New Exception("EncryptTextHandler must be specified on SystemSettings (e.g. CS - Singular.SystemSettings.General.EncryptTextHandler = Singular.Encryption.EncryptString; VB: Singular.SystemSettings.EncryptTextHandler = Singular.Encryption.EncryptString)")
            End If
            propValue = SystemSettings.EncryptTextHandler.Invoke(propValue)
          End If
          If TypeOf propValue Is System.Byte() Then
            .Parameters.AddWithValue("@PropertyValue", DBNull.Value)
            .Parameters.AddWithValue("@PropertyBytes", propValue)
          Else
            .Parameters.AddWithValue("@PropertyValue", Singular.Misc.NothingDBNull(propValue))
            .Parameters.AddWithValue("@PropertyBytes", DBNull.Value).DbType = DbType.Binary
          End If


          .ExecuteNonQuery()

          If Me.IsNew Then
            LoadProperty(SystemSettingValueIDProperty, paramSystemSettingValueID.Value)
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
        cm.CommandText = "DelProcs.delSystemSettingValue"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@SystemSettingValueID", GetProperty(SystemSettingValueIDProperty))
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