Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CorrespondanceTemplates

  <Serializable()> _
  Public Class CorrespondanceTemplate
    Inherits SingularBusinessBase(Of CorrespondanceTemplate)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared CorrespondanceTemplateIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.CorrespondanceTemplateID, "Correspondance Template", 0)
    ''' <Summary>
    ''' Gets the Correspondance Template value
    ''' </Summary>
    <Display(Name:="Correspondance Template", Description:="", AutoGenerateField:=False)> _
    Public ReadOnly Property CorrespondanceTemplateID() As Integer
      Get
        Return GetProperty(CorrespondanceTemplateIDProperty)
      End Get
    End Property

    Public Shared TemplateNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.TemplateName, "TemplateName", "")
    ''' <Summary>
    ''' Gets and sets the Template value
    ''' </Summary>
    <Display(Name:="Template Name", Description:="The Name of the Template."), _
     Required(ErrorMessage:="Template Name required"), StringLength(100, ErrorMessage:="Template Name cannot be more than 100 characters")> _
    Public Property TemplateName() As String
      Get
        Return GetProperty(TemplateNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(TemplateNameProperty, Value)
      End Set
    End Property

    Public Shared SendToProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SendTo, "Send To", "")
    ''' <Summary>
    ''' Gets and sets the SendTo value
    ''' </Summary>
    <Display(Name:="Send To", Description:="The Send To containing the Text, and the Tags which correspond to fields of the DataSource.", AutoGenerateField:=False)> _
    Public Property SendTo() As String
      Get
        Return GetProperty(SendToProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(SendToProperty, Value)
        ToPreview = GetMergedTemplateText(SendTo, DataSourceObject, DataObject)
      End Set
    End Property

    Public Shared SubjectProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Subject, "Subject", "")
    ''' <Summary>
    ''' Gets and sets the Template value
    ''' </Summary>
    <Display(Name:="Subject", Description:="The Subject containing the Text, and the Tags which correspond to fields of the DataSource.", AutoGenerateField:=False)> _
    Public Property Subject() As String
      Get
        Return GetProperty(SubjectProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(SubjectProperty, Value)
        SubjectPreview = GetMergedTemplateText(Subject, DataSourceObject, DataObject)
      End Set
    End Property

    Public Shared TemplateProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Template, "Template", "")
    ''' <Summary>
    ''' Gets and sets the Template value
    ''' </Summary>
    <Display(Name:="Template", Description:="The Template containing the Text, and the Tags which correspond to fields of the DataSource.", AutoGenerateField:=False)> _
    Public Property Template() As String
      Get
        Return GetProperty(TemplateProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(TemplateProperty, Value)
        BodyPreview = GetMergedTemplateText(Template, DataSourceObject, DataObject)
      End Set
    End Property

    Public Shared DataSourceProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DataSource, "Data Source", "")
    ''' <Summary>
    ''' Gets and sets the Data Source value
    ''' </Summary>
    <Display(Name:="Data Source", Description:="The DataSource where the data comes from. These are specified in the screen.", AutoGenerateField:=False),
    StringLength(100, ErrorMessage:="Data Source cannot be more than 100 characters")> _
    Public Property DataSource() As String
      Get
        Return GetProperty(DataSourceProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(DataSourceProperty, Value)
      End Set
    End Property

    <NonSerialized()> Private mDataSourceObject As CorrespondanceDataSource
    <Display(AutoGenerateField:=False)> _
    Public Property DataSourceObject As CorrespondanceDataSource
      Get
        Return mDataSourceObject
      End Get
      Set(value As CorrespondanceDataSource)
        mDataSourceObject = value
        SetTemplateText()
        OnPropertyChanged("DataSourceObject")
      End Set
    End Property

    <NonSerialized()> Private mDataObject As Object
    <Display(AutoGenerateField:=False)> _
    Public Property DataObject As Object
      Get
        Return mDataObject
      End Get
      Set(value As Object)
        mDataObject = value
        SetTemplateText()
        OnPropertyChanged("DataObject")
      End Set
    End Property

    Private mToPreview As String = ""
    <Display(AutoGenerateField:=False)> _
    Public Property ToPreview As String
      Get
        Return mToPreview
      End Get
      Private Set(value As String)
        If mToPreview <> value Then
          mToPreview = value
          OnPropertyChanged("ToPreview")
        End If
      End Set
    End Property

    Private mSubjectPreview As String = ""
    <Display(AutoGenerateField:=False)> _
    Public Property SubjectPreview As String
      Get
        Return mSubjectPreview
      End Get
      Private Set(value As String)
        If mSubjectPreview <> value Then
          mSubjectPreview = value
          OnPropertyChanged("SubjectPreview")
        End If
      End Set
    End Property

    Private mBodyPreview As String = ""
    <Display(AutoGenerateField:=False)> _
    Public Property BodyPreview As String
      Get
        Return mBodyPreview
      End Get
      Private Set(value As String)
        If mBodyPreview <> value Then
          mBodyPreview = value
          OnPropertyChanged("BodyPreview")
        End If
      End Set
    End Property

#End Region

#Region " Child Lists "

    Public Shared CorrespondanceTemplateAttachmentListProperty As PropertyInfo(Of CorrespondanceTemplateAttachmentList) = RegisterProperty(Of CorrespondanceTemplateAttachmentList)(Function(c) c.CorrespondanceTemplateAttachmentList, "Correspondance Template Attachment List")

    Public ReadOnly Property CorrespondanceTemplateAttachmentList() As CorrespondanceTemplateAttachmentList
      Get
        If GetProperty(CorrespondanceTemplateAttachmentListProperty) Is Nothing Then
          LoadProperty(CorrespondanceTemplateAttachmentListProperty, CorrespondanceTemplates.CorrespondanceTemplateAttachmentList.NewCorrespondanceTemplateAttachmentList())
        End If
        Return GetProperty(CorrespondanceTemplateAttachmentListProperty)
      End Get
    End Property

#End Region

#Region " Methods "

    Private Sub SetTemplateText()
      ToPreview = GetMergedTemplateText(SendTo, DataSourceObject, DataObject)
      SubjectPreview = GetMergedTemplateText(Subject, DataSourceObject, DataObject)
      BodyPreview = GetMergedTemplateText(Template, DataSourceObject, DataObject)
    End Sub

    Public Const StartTagChar As String = "<"
    Public Const EndTagChar As String = ">"

    Private Function GetMergedSendToText(CorrespondanceDS As CorrespondanceDataSource, DataObject As Object) As String
      Return GetMergedTemplateText(SendTo, CorrespondanceDS, DataObject)
    End Function

    Private Function GetMergedSubjectText(CorrespondanceDS As CorrespondanceDataSource, DataObject As Object) As String
      Return GetMergedTemplateText(Subject, CorrespondanceDS, DataObject)
    End Function

    Private Function GetMergedBodyText(CorrespondanceDS As CorrespondanceDataSource, DataObject As Object) As String
      Return GetMergedTemplateText(Template, CorrespondanceDS, DataObject)
    End Function

    Private Function GetMergedTemplateText(Text As String, CorrespondanceDS As CorrespondanceDataSource, DataObject As Object) As String

      If CorrespondanceDS Is Nothing OrElse DataObject Is Nothing Then
        Return ""
      End If

      If Singular.Reflection.GetLastGenericType(CorrespondanceDS.DataSource.GetType) IsNot DataObject.GetType Then
        Return ""
      End If

#If SILVERLIGHT Then
      If CorrespondanceDS Is Nothing Then
        Return ""
      Else

        Try

          Dim InTag As Boolean = False
          Dim Buffer As String = ""
          Dim Output As String = ""

          For Each ch As Char In Text

            Buffer &= ch

            Select Case ch

              Case StartTagChar

                If Not InTag Then
                  'This is the start of a tag, flush everything currently in the buffer to the output, except the start tag char.
                  Output &= Buffer.Substring(0, Buffer.Length - 1)
                  'Keep the start tag char in the buffer (incase this is not a proper field).
                  Buffer = StartTagChar
                  'Mark that we are now in a tag.
                  InTag = True
                End If

              Case EndTagChar
                If InTag Then
                  'If we were in a tag, then found an end tag, mark InTag = false for the next character.
                  InTag = False

                  'Strip the tags from the field name, and try find it in the field list.
                  Dim fieldName As String = Buffer.Replace(StartTagChar, "").Replace(EndTagChar, "").ToLower

                  Dim f = CorrespondanceDS.FieldList.FirstOrDefault(Function(pi) pi.DisplayName.ToLower = fieldName)



                  If f Is Nothing Then
                    'Field doesnt exists, so just flush the full buffer to the output.
                    Output &= Buffer
                  ElseIf f.PropertyInfo.DeclaringType.Equals(DataObject.GetType) Then
                    'the field exists, add any extra start tag characters the user might have added.
                    Output &= Buffer.Substring(0, Buffer.LastIndexOf(StartTagChar))
                    'add the actual row value from the data object.
                    Output &= f.PropertyInfo.GetValue(DataObject, Nothing)
                  Else
                    Output &= Buffer
                  End If

                Else
                  'If we encountered an end tag, but weren't in a tag (there was no start tag), then flush the buffer.
                  Output &= Buffer
                End If

                Buffer = ""

            End Select

          Next

          'Return the output, and the buffer in case there were any characters left in it.
          Return Output & Buffer

        Catch ex As Exception
          Return "error"
        End Try

      End If
#End If
      Return Nothing
    End Function

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(CorrespondanceTemplateIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If Me.Template.Length = 0 Then
        If Me.IsNew Then
          Return "New Correspondance Template"
        Else
          Return "Blank Correspondance Template"
        End If
      Else
        Return Me.Template
      End If

    End Function

    Protected Overrides ReadOnly Property TableReferencesToIgnore() As String()
      Get
        Return New String() {"CorrespondanceTemplateAttachments"}
      End Get
    End Property

#End Region

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()

    End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

    Public Sub New()

      MarkAsChild()
      BusinessRules.CheckRules()

    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Function NewCorrespondanceTemplate() As CorrespondanceTemplate

      Return New CorrespondanceTemplate()

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetCorrespondanceTemplate(ByVal dr As SafeDataReader) As CorrespondanceTemplate

      Dim c As New CorrespondanceTemplate()
      c.Fetch(dr)
      Return c

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(CorrespondanceTemplateIDProperty, .GetInt32(0))
          LoadProperty(TemplateNameProperty, .GetString(1))
          LoadProperty(SendToProperty, .GetString(2))
          LoadProperty(SubjectProperty, .GetString(3))
          LoadProperty(TemplateProperty, .GetString(4))
          LoadProperty(DataSourceProperty, .GetString(5))
        End With
      End Using

      MarkAsChild()
      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "InsProcs.insCorrespondanceTemplate"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "UpdProcs.updCorrespondanceTemplate"

        DoInsertUpdateChild(cm)

      End Using

    End Sub

    Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

      If MyBase.IsDirty Then

        With cm
          .CommandType = CommandType.StoredProcedure

          Dim paramCorrespondanceTemplateID As SqlParameter = .Parameters.Add("@CorrespondanceTemplateID", SqlDbType.Int)
          paramCorrespondanceTemplateID.Value = GetProperty(CorrespondanceTemplateIDProperty)
          If Me.IsNew Then
            paramCorrespondanceTemplateID.Direction = ParameterDirection.Output
          End If
          .Parameters.AddWithValue("@TemplateName", GetProperty(TemplateNameProperty))
          .Parameters.AddWithValue("@SendTo", GetProperty(SendToProperty))
          .Parameters.AddWithValue("@Subject", GetProperty(SubjectProperty))
          .Parameters.AddWithValue("@Template", GetProperty(TemplateProperty))
          .Parameters.AddWithValue("@DataSource", GetProperty(DataSourceProperty))

          .ExecuteNonQuery()

          If Me.IsNew() Then
            LoadProperty(CorrespondanceTemplateIDProperty, paramCorrespondanceTemplateID.Value)
          End If
          ' update child objects
          If GetProperty(CorrespondanceTemplateAttachmentListProperty) IsNot Nothing Then
            Me.CorrespondanceTemplateAttachmentList.Update()
          End If
          MarkOld()
        End With
      Else
        ' update child objects
        If GetProperty(CorrespondanceTemplateAttachmentListProperty) IsNot Nothing Then
          Me.CorrespondanceTemplateAttachmentList.Update()
        End If
      End If

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Me.IsNew Then Exit Sub

      Using cm As SqlCommand = New SqlCommand
        cm.CommandText = "DelProcs.delCorrespondanceTemplate"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.AddWithValue("@CorrespondanceTemplateID", GetProperty(CorrespondanceTemplateIDProperty))
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