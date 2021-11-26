Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace ContentEditing

  <Serializable()> _
  Public Class WebContent
    Inherits SingularBusinessBase(Of WebContent)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared WebContentIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.WebContentID, "Web Content", 0)
    ''' <Summary>
    ''' Gets the Web Content value
    ''' </Summary>
    <Display(AutoGenerateField:=False), System.ComponentModel.Browsable(True)>
    Public ReadOnly Property WebContentID() As Integer
      Get
        Return GetProperty(WebContentIDProperty)
      End Get
    End Property

    Public Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Name, "Name", "")
    ''' <Summary>
    ''' Gets and sets the Name value
    ''' </Summary>
    <Display(Name:="Name", Description:=""),
    StringLength(100, ErrorMessage:="Name cannot be more than 100 characters")>
    Public Property Name() As String
      Get
        Return GetProperty(NameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(NameProperty, Value)

        If ContentEditing.StorageMode = StorageModeType.File Then
          'Load the html file
          Dim FilePath As String = Utils.Server_MapPath(ContentEditing.FileStorageDirectory & Name & FileSuffix & ".txt")
          If IO.File.Exists(FilePath) Then
            LoadProperty(HTMLContentProperty, IO.File.ReadAllText(FilePath))
          End If
        End If
      End Set
    End Property

    Public Shared HTMLContentProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.HTMLContent, "HTML Content", "")
    ''' <Summary>
    ''' Gets and sets the HTML Content value
    ''' </Summary>
    <Browsable(False)>
    Public Property HTMLContent() As String
      Get
        If ContentEditing.StorageMode = StorageModeType.Database AndAlso Singular.Localisation.CurrentLanguageID <> 1 Then
          Return ContentLanguage(Singular.Localisation.CurrentLanguageID)
        Else
          Return GetProperty(HTMLContentProperty)
        End If
      End Get
      Set(ByVal Value As String)

        MarkDirty()

        If ContentEditing.StorageMode = StorageModeType.Database Then

          If Singular.Localisation.CurrentLanguageID = 1 Then
            SetProperty(HTMLContentProperty, Value)
          Else
            ContentLanguage(Singular.Localisation.CurrentLanguageID) = Value
          End If

          'Save to the database immediately
          Dim wcl As WebContentList = WebContentList.NewWebContentList
          wcl.Add(Me)
          wcl.Save()
        Else

          SetProperty(HTMLContentProperty, Value)

          'Save to file
          Dim FilePath = Utils.Server_MapPath(ContentEditing.FileStorageDirectory & Name & FileSuffix & ".txt")
          Dim Path As String = FilePath
          IO.File.WriteAllText(Path, Value)
        End If

        mHTMLFormatted.Clear()

      End Set
    End Property

    Private mPreviewHTML As String = ""

    <Browsable(False)>
    Public Property PreviewHTML As String
      Get
        Return mPreviewHTML
      End Get
      Set(value As String)
        If PreviewHTML <> value Then
          mPreviewHTML = value
          mPreviewFormatted.Clear()
        End If
      End Set
    End Property

    Private mHTMLFormatted As New System.Collections.Generic.Dictionary(Of Integer, String)
    <Browsable(False)>
    Public ReadOnly Property HTMLContentFormatted As String
      Get
        If Not mHTMLFormatted.ContainsKey(Singular.Localisation.CurrentLanguageID) Then
          Dim HTML = PopulateKeys(HTMLContent)
          If ProcessContent IsNot Nothing Then HTML = ProcessContent(HTML)
          mHTMLFormatted(Singular.Localisation.CurrentLanguageID) = HTML
        End If
        Return mHTMLFormatted(Singular.Localisation.CurrentLanguageID)
      End Get
    End Property

    Private mPreviewFormatted As New System.Collections.Generic.Dictionary(Of Integer, String)
    <Browsable(False)>
    Public ReadOnly Property PreviewHTMLFormatted As String
      Get
        If Not mPreviewFormatted.ContainsKey(Singular.Localisation.CurrentLanguageID) Then
          mPreviewFormatted(Singular.Localisation.CurrentLanguageID) = PopulateKeys(PreviewHTML)
        End If
        Return mPreviewFormatted(Singular.Localisation.CurrentLanguageID)
      End Get
    End Property

    Private ReadOnly Property FileSuffix As String
      Get
        If Singular.Localisation.CurrentLanguageID = 1 Then
          Return ""
        Else
          Return Singular.Localisation.CurrentCulture.TwoLetterISOLanguageName & "."
        End If
      End Get
    End Property

#End Region

#Region " Localisation Support "

    Private mContentLanguages As New System.Collections.Generic.Dictionary(Of Integer, String)
    Friend ReadOnly Property ContentLanguages As System.Collections.Generic.Dictionary(Of Integer, String)
      Get
        Return mContentLanguages
      End Get
    End Property

    Private Property ContentLanguage(LanguageID As Integer) As String
      Get
        If Not mContentLanguages.ContainsKey(LanguageID) Then
          mContentLanguages.Add(LanguageID, GetProperty(HTMLContentProperty))
        End If
        Return mContentLanguages(LanguageID)
      End Get
      Set(value As String)
        If mContentLanguages.ContainsKey(LanguageID) Then
          mContentLanguages(LanguageID) = value
        Else
          mContentLanguages.Add(LanguageID, value)
        End If
      End Set
    End Property

#End Region

#Region " Methods "

    Private Function PopulateKeys(Content As String) As String

      For Each key As String In KeyList.Keys
        Content = Content.Replace("[" & key & "]", KeyList(key))
      Next

      Return Content

    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(WebContentIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.Name.Length = 0 Then
        If Me.IsNew Then
          Return String.Format("New {0}", "Web Content")
        Else
          Return String.Format("Blank {0}", "Web Content")
        End If
      Else
        Return Me.Name
      End If

    End Function

#End Region

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Shared Function NewWebContent() As WebContent

      Return DataPortal.CreateChild(Of WebContent)()

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

    Friend Shared Function GetWebContent(ByVal dr As SafeDataReader) As WebContent

      Dim w As New WebContent()
      w.Fetch(dr)
      Return w

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(WebContentIDProperty, .GetInt32(0))
          LoadProperty(NameProperty, .GetString(1))
          LoadProperty(HTMLContentProperty, .GetString(2))
        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      If Me.IsSelfDirty Then

        cm.CommandText = "[InsProcs].[insUpdWebContent]"

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramWebContentID As SqlParameter = .Parameters.Add("@WebContentID", SqlDbType.Int)
          paramWebContentID.Value = GetProperty(WebContentIDProperty)
          If Me.IsNew Then
            paramWebContentID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@Name", GetProperty(NameProperty))
          .Parameters.AddWithValue("@LanguageID", Singular.Localisation.CurrentLanguageID)
          .Parameters.AddWithValue("@HTMLContent", HTMLContent)

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(WebContentIDProperty, paramWebContentID.Value)
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
        cm.CommandText = "DelProcs.delWebContent"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@WebContentID", GetProperty(WebContentIDProperty))
        DoDeleteChild(cm)
      End Using

    End Sub

    Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)

      If Me.IsNew Then Exit Sub

      With cm
        .ExecuteNonQuery()
      End With
      MarkNew()

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace