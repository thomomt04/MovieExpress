Imports System.Reflection

Namespace MaintenanceHelpers

  Public Class MaintenanceVM
    Inherits Singular.Web.ViewModel(Of MaintenanceVM)

    Public Property CurrentList As Object

    <System.ComponentModel.Browsable(False)>
     Public Property CurrentObject As Object

		<System.ComponentModel.Browsable(False)>
		Public Property CurrentMaintenancePage As MaintenancePage

		Protected Property UseDropDownTextValue As Boolean

		Private mListType As String
    Public ReadOnly Property ListType As String
      Get
        Return mListType
      End Get
    End Property

    Public Overridable ReadOnly Property CanEdit As Boolean
      Get
        Return True
      End Get
    End Property

    Protected Overrides Sub Setup()
      MyBase.Setup()

      ValidationMode = Singular.Web.ValidationMode.OnLoad
    End Sub

    Protected Friend Overrides Sub PostSetup()
      mListType = Page.Request.QueryString("Type")

      If ListType IsNot Nothing Then
        CurrentMaintenancePage = GetMaintenangePage(ListType)
        If CurrentMaintenancePage IsNot Nothing Then
          If CurrentMaintenancePage.EditMode = MaintenancePage.EditModeType.Grid OrElse CurrentMaintenancePage.EditMode = MaintenancePage.EditModeType.Custom Then
            CurrentList = Singular.Reflection.FetchList(CurrentMaintenancePage.ListType, Nothing)
            If Not IsStateless Then CurrentList.BeginEdit()
          End If
          If CurrentMaintenancePage.OnMaintPageLoad IsNot Nothing Then
            CurrentMaintenancePage.OnMaintPageLoad.Invoke(New MaintenancePage.OnLoadHelper(CurrentMaintenancePage))
          End If
        End If

      End If
    End Sub

    Protected Overrides Sub HandleCommand(Command As String, CommandArgs As CommandArgs)
      MyBase.HandleCommand(Command, CommandArgs)

      Select Case Command

        Case "Back"

          If CurrentObject IsNot Nothing Then
            'Show the new / find view
            CurrentList = Nothing
            CurrentObject = Nothing
            ResetSerialiser()

          End If


        Case "Undo"
          If CurrentObject IsNot Nothing Then
            CurrentObject.CancelEdit()
            CurrentObject.BeginEdit()
          Else
            CurrentList.CancelEdit()
            CurrentList.BeginEdit()
          End If

          AddMessage(MessageType.Information, "Undo", "Changes have been undone.")

        Case "New"
          'Create a new list, and make the list add a new item.
          'This is only when editing one object at a time.
          CurrentList = Activator.CreateInstance(CurrentMaintenancePage.ListType)
          CurrentObject = CType(CurrentList, Object).AddNew
          CurrentObject.BeginEdit()
          ResetSerialiser()

        Case "Save"
          Dim Result As Singular.Web.Result = Nothing
          PreSave(Result)
          If Result Is Nothing OrElse Result.Success = True Then
            PreSave()
            CurrentList.ApplyEdit()
            With TrySave(CurrentList, True, True)

              If .Success Then
                CurrentList = .SavedObject
              End If
            End With
            CurrentList.BeginEdit()
            ResetCommonDataLists()
            If Result IsNot Nothing AndAlso Result.ErrorText <> "" Then
              AddMessage(MessageType.Success, "Save", Result.ErrorText)
            End If
          Else
            AddMessage(MessageType.Error, "Save", Result.ErrorText)
          End If
        Case "Export"
          'Export to Excel
          Dim stream As IO.MemoryStream
					stream = CurrentList.GetExcelDocumentStream(True, UseDropDownTextValue)
					SendFile(CurrentList.ToString & ".xlsx", stream.ToArray)
        Case "ImportTemplate"
        Case "Import"
        Case "Delete"
          CType(CurrentList, ISingularBusinessListBase).RemoveAt(0)
          With TrySave(CurrentList, False, False)
            If .Success Then
              AddMessage(MessageType.Success, "Delete", "Delete Successful")
              CurrentList = Nothing
              CurrentObject = Nothing
            Else
              AddMessage(MessageType.Error, "Delete", "Delete Unsuccessful: " & .ErrorText)
            End If
          End With
          'Refresh common data lists.
          For Each t As Type In CurrentMaintenancePage.RefreshTypes
            CommonData.Refresh(t)
          Next
      End Select

      If Command.EndsWith("Find") Then
        FindRecord(CommandArgs.ClientArgs)
      End If

    End Sub

    Protected Overridable Sub PreSave()

    End Sub

    Protected Overridable Sub PreSave(ByRef Result As Singular.Web.Result)

    End Sub

    Private Sub ResetCommonDataLists()
      'Refresh common data lists.
      ResetCommonDataLists(CurrentMaintenancePage)
      
    End Sub

    Protected Shared Sub ResetCommonDataLists(MaintenancePage As MaintenancePage)
      'Refresh common data lists.

      Dim ROType = Type.GetType(String.Format("{0}, {1}", MaintenancePage.ListType.FullName.Replace(MaintenancePage.ListType.Name, "RO" & MaintenancePage.ListType.Name), MaintenancePage.ListType.Assembly.FullName))
      Dim RefreshedROList As Boolean = False

      For Each t As Type In MaintenancePage.RefreshTypes
        CommonData.Refresh(t)

        If ROType IsNot Nothing AndAlso t.GetType Is ROType Then
          RefreshedROList = True
        End If
      Next

      'Refresh the main type
      CommonData.Refresh(MaintenancePage.ListType)

      'Refresh the RO version.
      If Not RefreshedROList AndAlso ROType IsNot Nothing Then
        CommonData.Refresh(ROType)
      End If
    End Sub

    Private Sub FindRecord(ID As Object)

      'Find the ID property of the child type
      Dim IDProperty As System.Reflection.PropertyInfo = Nothing
      Dim IDPropertyName As String = ""

      For Each pi As System.Reflection.PropertyInfo In Singular.Reflection.GetLastGenericType(CurrentMaintenancePage.ListType).GetProperties(BindingFlags.Public Or BindingFlags.Instance)
        If Singular.ReflectionCached.GetCachedMemberInfo(pi).IsKey Then
          IDPropertyName = pi.Name
          Exit For
        End If
      Next

      If IDPropertyName = "" Then
        Throw New Exception("Object does not have its ID marked with Singular.DataAnnotations.IDProperty")
      Else
        Dim CritType = CurrentMaintenancePage.ListType.GetNestedType("Criteria", BindingFlags.Public Or BindingFlags.NonPublic)
        IDProperty = CritType.GetProperty(IDPropertyName, BindingFlags.Public Or BindingFlags.Instance)
        If IDProperty Is Nothing Then
          Throw New Exception("Criteria Object in Type " & CurrentMaintenancePage.ListType.Name & " does not have public property " & IDPropertyName)
        Else
          Dim Criteria = Activator.CreateInstance(CritType)
          IDProperty.SetValue(Criteria, Singular.Reflection.ConvertValueToType(IDProperty.PropertyType, ID), Nothing)

          CurrentList = Singular.Reflection.FetchList(CurrentMaintenancePage.ListType, Criteria)
          CurrentObject = CurrentList(0)
          CurrentObject.BeginEdit()
          ResetSerialiser()
        End If
      End If

    End Sub

#Region " Menu Structure "

    Private mMainSectionList As New List(Of MainSection)

    Private Function GetMaintenangePage(Hash As String) As MaintenancePage
      Return GetMaintenangePage(Hash, MainSectionList)
    End Function

    Protected Shared Function GetMaintenangePage(Hash As String, MainSectionList As List(Of MainSection)) As MaintenancePage
      For Each ms As MainSection In MainSectionList
        For Each ss As SubSection In ms.SubSectionList
          For Each mp As MaintenancePage In ss.MaintenancePageList
            If mp.Hash = Hash Then
              Return mp
            End If
          Next
        Next
      Next
      Return Nothing
    End Function

    <System.ComponentModel.Browsable(False)>
    Public ReadOnly Property MainSectionList As List(Of MainSection)
      Get
        Return mMainSectionList
      End Get
    End Property

    Protected Function AddMainSection(Heading As String) As MainSection
      Dim ms As New MainSection
      ms.Heading = Heading
      mMainSectionList.Add(ms)
      Return ms
    End Function

#End Region

  End Class

#Region " Menu Structure Classes "

  Public Class MainSection

    Public Property Heading As String
    Public Property ImagePath As String

    Private mSubSectionList As New List(Of SubSection)

    Public ReadOnly Property SubSectionList As List(Of SubSection)
      Get
        Return mSubSectionList
      End Get
    End Property

    Public Function AddSubSection(Heading As String) As SubSection
      Dim ss As New SubSection
      ss.Heading = Heading
      mSubSectionList.Add(ss)
      Return ss
    End Function

    Public Function AddMaintenancePage(Of ListType)(DisplayName As String, Optional AllowExport As Boolean = True) As MaintenancePage
      With AddSubSection("")
        Return .AddMaintenancePage(Of ListType)(DisplayName, AllowExport)
      End With
    End Function

    Public Function AddMaintenancePage(DisplayName As String, URL As String) As MaintenancePage
      With AddSubSection("")
        Return .AddMaintenancePage(DisplayName, URL)
      End With
    End Function

  End Class

  Public Class SubSection

    Public Property Heading As String

    Private mMaintenanceList As New List(Of MaintenancePage)

    Public ReadOnly Property MaintenancePageList As List(Of MaintenancePage)
      Get
        Return mMaintenanceList
      End Get
    End Property

    Public Function AddMaintenancePage(Of ListType)(DisplayName As String, Optional AllowExport As Boolean = True) As MaintenancePage
      Dim mp As New MaintenancePage
      mp.DisplayName = DisplayName
      mp.AllowExport = AllowExport
      mp.ListType = GetType(ListType)
      mp.Hash = Singular.Encryption.GetStringHash(mp.ListType.FullName, Encryption.HashType.MD5, , , True)
      mMaintenanceList.Add(mp)
      Return mp
    End Function

    Public Function AddMaintenancePage(DisplayName As String, URL As String) As MaintenancePage
      Dim mp As New MaintenancePage
      mp.DisplayName = DisplayName
      mp.URL = If(URL.Contains("~"), Utils.URL_ToAbsolute(URL), URL)
      mMaintenanceList.Add(mp)
      Return mp
    End Function

  End Class

  Public Class MaintenancePage

    Public Enum EditModeType
      Grid
      Form
      Custom
    End Enum

    Public Property EditMode As EditModeType = EditModeType.Grid

    Public Property Hash As String
    Public Property URL As String
    Public Property ListType As Type
    Public Property EndsSection As Boolean = False
    Public Property DisplayName As String
    Public Property CustomControlType As System.Type

    Public Property AllowAdd As Boolean = True
    Public Property AllowRemove As Boolean = True
    Public Property AllowExport As Boolean = True


    Public Property RemovePromptText As String = ""

    Public Property OnMaintPageLoad As Action(Of OnLoadHelper) = Nothing

    ''' <summary>
    ''' Text which is displayed at the top of the page when editing this maintenance list.
    ''' </summary>
    Public Property InfoText As String = ""

    ''' <summary>
    ''' Specifies which types in common data must be refreshed after this maintenance list is saved.
    ''' </summary>
    Public Property RefreshTypes As New List(Of Type)

    ''' <summary>
    ''' Adds a type in common data that must be refreshed after this maintenance list is saved.
    ''' </summary>
    Public Sub AddRefreshType(Of t)()
      RefreshTypes.Add(GetType(t))
    End Sub

    Private _ButtonList As New List(Of Singular.Web.CustomControls.Button(Of Object))

		Public ReadOnly Property ButtonList As List(Of Singular.Web.CustomControls.Button(Of Object))
			Get
				Return _ButtonList
			End Get
		End Property

		Public Class OnLoadHelper

      Private _MaintenancePage As MaintenancePage
      Public Sub New(MaintenancePage As MaintenancePage)
        _MaintenancePage = MaintenancePage
      End Sub

      Public Sub AddButton(ButtonText As String, ButtonStyle As Singular.Web.ButtonMainStyle, Icon As Singular.Web.FontAwesomeIcon, ClickJS As String, Optional ButtonSize As Singular.Web.ButtonSize = Singular.Web.ButtonSize.Normal)
        Dim Btn As New Singular.Web.CustomControls.Button(Of Object) With {.Text = ButtonText, .ButtonStyle = ButtonStyle, .ButtonSize = ButtonSize}
        Btn.Image.Glyph = Icon
        Btn.ClickJS = ClickJS
        _MaintenancePage._ButtonList.Add(Btn)
      End Sub

    End Class

  End Class

#End Region

  Public Class StatelessMaintenanceVM
    Inherits MaintenanceVM
    Implements IStatelessViewModel

    Private Shared mMenuList As New Hashtable

    Public Sub New()
      'DeserialiseMode = Singular.Web.DeserialiseMode.Stateless
    End Sub

    Protected Friend Overrides Sub PostSetup()
      MyBase.PostSetup()

      If mMenuList(Me.GetType) Is Nothing Then
        mMenuList(Me.GetType) = MainSectionList
      End If

    End Sub

    Private Sub EnsureLists()
      If mMenuList(Me.GetType) Is Nothing Then
        Setup()
        mMenuList(Me.GetType) = MainSectionList
      End If
    End Sub

    Private Function GetPage(ListType As String) As MaintenancePage
      'EnsureLists()
      Setup()
      Return GetMaintenangePage(ListType, MainSectionList)
    End Function

    'Used for stateless undo.
    Public Function FetchList(ListType As String) As Singular.Web.Result
      Return New Singular.Web.Result(
        Function()
          Return Singular.Reflection.FetchList(GetPage(ListType).ListType, Nothing)
        End Function)
    End Function

    Public Function SaveList(ListType As String, List As Object) As Singular.Web.SaveResult

      Dim MaintenanceType = GetPage(ListType)
      If MaintenanceType IsNot Nothing Then

        CurrentList = Activator.CreateInstance(MaintenanceType.ListType)
        Dim Serialiser As New Singular.Web.Data.JS.StatelessJSSerialiser(CurrentList)
        Serialiser.Deserialise(List)

        Dim Result As Singular.Web.Result = Nothing
        PreSave(Result)
        If Result Is Nothing OrElse Result.Success = True Then
          PreSave()

          Dim sh = TrySave(CurrentList, False, False)
          ResetCommonDataLists(MaintenanceType)
          Return New Singular.Web.SaveResult(sh)
        Else
          Return New Singular.Web.SaveResult(Result)
        End If

      Else
        Throw New Exception("No access to type: " & ListType)
      End If

    End Function

  End Class

End Namespace


