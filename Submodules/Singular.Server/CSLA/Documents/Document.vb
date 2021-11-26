Imports Csla
Imports Csla.Data
Imports Csla.Serialization
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Documents

  <Serializable()>
  Public Class Document
    Inherits SingularBusinessBase(Of Document)
    Implements Singular.Documents.IDocument

#Region " Properties and Methods "

#Region " Properties "

    Public Shared DocumentIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DocumentID, "DocumentID", 0)
    ''' <Summary>
    ''' Gets the Document value
    ''' </Summary>
    <Display(AutoGenerateField:=False, Name:="Document", Description:="")>
    Public ReadOnly Property DocumentID() As Integer
      Get
        Return GetProperty(DocumentIDProperty)
      End Get
    End Property

    Public Shared DocumentNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DocumentName, "Document Name", "")
    ''' <Summary>
    ''' Gets and sets the Document Name value
    ''' </Summary>
    <Display(Name:="Document Name", Description:=""),
    Required(ErrorMessage:="Document Name required"),
    StringLength(255, ErrorMessage:="Document Name cannot be more than 255 characters")>
    Public Property DocumentName() As String Implements Singular.Documents.IDocument.DocumentName
      Get
        Return GetProperty(DocumentNameProperty)
      End Get
      Set(ByVal Value As String)
        SetProperty(DocumentNameProperty, GetSafeDocumentName(Value))
      End Set
    End Property

    Public Shared DocumentProperty As PropertyInfo(Of Byte()) = RegisterProperty(Of Byte())(Function(c) c.Document, "Document")
    ''' <Summary>
    ''' Gets and sets the Document value
    ''' </Summary>
    <Display(AutoGenerateField:=False)>
    Public Property Document() As Byte() Implements Singular.Documents.IDocument.Document
      Get
        Return GetProperty(DocumentProperty)
      End Get
      Set(ByVal Value As Byte())
        SetProperty(DocumentProperty, Value)
#If Silverlight = False Then
        GenerateHash()
#End If
      End Set
    End Property

    Public Shared CreatedByProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.CreatedBy, "Created By", CInt(Csla.ApplicationContext.ClientContext("UserID")))
    ''' <Summary>
    ''' Gets the Created By value
    ''' </Summary>
    <Display(AutoGenerateField:=False), Browsable(False)>
    Public ReadOnly Property CreatedBy() As Integer
      Get
        Return GetProperty(CreatedByProperty)
      End Get
    End Property

    Public Shared CreatedDateTimeProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.CreatedDateTime, "Created Date Time", New SmartDate(Now()))
    ''' <Summary>
    ''' Gets the Created Date Time value
    ''' </Summary>
    <Display(AutoGenerateField:=False), Browsable(False)>
    Public ReadOnly Property CreatedDateTime() As SmartDate
      Get
        Return GetProperty(CreatedDateTimeProperty)
      End Get
    End Property

    Public Shared ModifiedByProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.ModifiedBy, "Modified By", CInt(Csla.ApplicationContext.ClientContext("UserID")))
    ''' <Summary>
    ''' Gets the Modified By value
    ''' </Summary>
    <Display(AutoGenerateField:=False), Browsable(False)>
    Public ReadOnly Property ModifiedBy() As Integer
      Get
        Return GetProperty(ModifiedByProperty)
      End Get
    End Property

    Public Shared OverridesModifiedByProperty As PropertyInfo(Of Integer?) = RegisterProperty(Of Integer?)(Function(c) c.OverridesModifiedBy, "Overrides Modified By", Nothing)
    ''' <Summary>
    ''' Gets the Modified By value
    ''' </Summary>
    <Display(AutoGenerateField:=False), Browsable(False)>
    Public Property OverridesModifiedBy() As Integer?
      Get
        Return GetProperty(OverridesModifiedByProperty)
      End Get
      Set(ByVal Value As Integer?)
        SetProperty(OverridesModifiedByProperty, Value)
      End Set
    End Property

    Public Shared ModifiedDateTimeProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(Function(c) c.ModifiedDateTime, "Modified Date Time", New SmartDate(Now()))
    ''' <Summary>
    ''' Gets the Modified Date Time value
    ''' </Summary>
    <Display(AutoGenerateField:=False), Browsable(False)>
    Public ReadOnly Property ModifiedDateTime() As SmartDate
      Get
        Return GetProperty(ModifiedDateTimeProperty)
      End Get
    End Property

    <Display(AutoGenerateField:=False)>
    Public Overrides ReadOnly Property IsValid As Boolean
      Get
        If Me.IsDeleted Then
          Return True
        Else
          Return MyBase.IsValid
        End If
      End Get
    End Property

#End Region

#Region " Document Hashing "

#If Silverlight = False Then

    Public ReadOnly Property HashBytes As Byte()
      Get
        Return mHashBytes
      End Get
    End Property

    Public ReadOnly Property HashCode As String
      Get
        Return mHashCode
      End Get
    End Property

    Private mHashBytes As Byte()
    Private mHashCode As String = ""

    Private Sub GenerateHash()
      If Settings.DocumentHashesEnabled Then
        mHashBytes = GenerateHash(GetProperty(DocumentProperty))
        mHashCode = GenerateHashCodeString(mHashBytes)
      End If
    End Sub

    Public Shared Function GenerateHash(DocumentBytes As Byte()) As Byte()

      Dim Hasher As System.Security.Cryptography.HashAlgorithm = System.Security.Cryptography.MD5.Create
      Return Hasher.ComputeHash(DocumentBytes)

    End Function

    Public Shared Function GenerateHashCodeString(HashBytes As Byte()) As String

      If HashBytes Is Nothing Then
        Return ""
      Else
        Dim HashCode = ""
        For Each b As Byte In HashBytes
          HashCode &= b.ToString("x2")
        Next
        Return HashCode
      End If

    End Function

#End If

#End Region

#Region " Methods "
#If SILVERLIGHT Then
    Private Shared InvalidChars() As Char = {"/", "\", "|", "*", "?", """", ":", ">", "<"}
#Else
    Private Shared InvalidChars() As Char = System.IO.Path.GetInvalidFileNameChars() '{"/", "\", "|", "*", "?", """", ":", ">", "<"}
#End If
    Public Shared Function GetSafeDocumentName(ByVal DocumentName As String) As String

      Dim tempDocName As String = DocumentName
      For Each invalidChar As Char In InvalidChars
        tempDocName = tempDocName.Replace(invalidChar, "_")
      Next
      Return tempDocName

    End Function

    Public Sub SetDocumentID(DocID As Integer)
      LoadProperty(DocumentIDProperty, DocID)
      MarkOld()
    End Sub

    Sub DeleteDocument()
      MarkDeleted()
    End Sub

    Public Sub Download(ByVal DownloadCompleted As System.Action(Of Singular.Documents.IDocument)) 'Implements Singular.Documents.IDocument.Download

    End Sub

    Public ReadOnly Property IsDownloaded As Boolean ' Implements Singular.Documents.IDocument.IsDownloaded
      Get
        Return Document IsNot Nothing
      End Get
    End Property

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(DocumentIDProperty)

    End Function

    Public Overrides Function ToString() As String

      If GetProperty(DocumentNameProperty).Length = 0 Then
        If Me.IsNew Then
          Return "New Document"
        Else
          Return "Blank Document"
        End If
      Else
        Return GetProperty(DocumentNameProperty)
      End If

    End Function

    Public Function GetFileExtension() As String

      Dim ReturnString As String = ""
      If DocumentName IsNot Nothing AndAlso DocumentName.Length <> 0 Then
        Dim DotSeparatedStrings As String() = DocumentName.Split("."c)
        ReturnString = DotSeparatedStrings.Last
      End If
      Return ReturnString

    End Function

#If Silverlight = False Then

    Public Shared Function GetTextFile(Text As String, FileName As String) As Document
      Return New Document(FileName, System.Text.Encoding.ASCII.GetBytes(Text))
    End Function

    Public Shared Function GetTextFileUTF(Text As String, FileName As String) As Document
      Return New Document(FileName, System.Text.Encoding.UTF8.GetBytes(Text))
    End Function

    Public Shared Function CreateZipFile(ZipFileName As String, Documents As List(Of Singular.Documents.IDocument)) As Document

      Using ms As New IO.MemoryStream

        Using zs As New ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ms)

          zs.SetLevel(3)

          For Each doc In Documents

            Dim ze As New ICSharpCode.SharpZipLib.Zip.ZipEntry(doc.DocumentName)
            Dim Buffer(4095) As Byte
            zs.PutNextEntry(ze)
            ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(New IO.MemoryStream(doc.Document), zs, Buffer)
            zs.CloseEntry()

          Next

          zs.Close()
        End Using

        Return New Document(ZipFileName, ms.ToArray)

      End Using

    End Function

    Public Shared Function CreateZipFile(ZipFileName As String, FileNames As String(), Files As Byte()()) As Document

      Dim DocList As New List(Of Singular.Documents.IDocument)
      For i As Integer = 0 To FileNames.Length - 1
        DocList.Add(New Singular.Documents.Document(FileNames(i), Files(i)))
      Next
      Return CreateZipFile(ZipFileName, DocList)

    End Function

#End If

#End Region

#End Region

#Region " Validation Rules "

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      MyBase.AddBusinessRules()
      BusinessRules.AddRule(New DocumentValid(DocumentProperty))

    End Sub

    Private Class DocumentValid
      Inherits Csla.Rules.BusinessRule

      Public Sub New(ByVal DocumentProperty As Csla.Core.IPropertyInfo)

        MyBase.New(DocumentProperty)
        Me.InputProperties = New List(Of Csla.Core.IPropertyInfo) From {PrimaryProperty}

      End Sub

      Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

        Dim Document() As Byte = context.InputPropertyValues(PrimaryProperty)

        If Document Is Nothing OrElse Document.Length = 0 Then
          context.AddErrorResult("Document is required")
        End If

      End Sub

    End Class

#End Region

#End Region

#Region " Data Access & Factory Methods "

    <Serializable()>
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Property ConnectionString As String = ""

      Public Shared DocumentIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.DocumentID, "DocumentID")

      <Display(AutoGenerateField:=False)>
      Public Property DocumentID() As Integer
        Get
          Return ReadProperty(DocumentIDProperty)
        End Get
        Set(ByVal value As Integer)
          LoadProperty(DocumentIDProperty, value)
        End Set
      End Property

      Public Shared HashCodeProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.HashCode, "HashCode", "")

      <Display(AutoGenerateField:=False)>
      Public Property HashCode() As String
        Get
          Return ReadProperty(HashCodeProperty)
        End Get
        Set(ByVal value As String)
          LoadProperty(HashCodeProperty, value)
        End Set
      End Property

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewDocument() As Document

      Return New Document()
      'Giving Error, Must Look into
      'Return DataPortal.Create(Of Document)()

    End Function

    Public Sub New()

    End Sub

    Public Sub New(FileName As String, FileBytes As Byte())
      DocumentName = FileName
      Document = FileBytes
    End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    'Public Shared Function BeginGetDocument(DocumentID As Integer, ByVal CallBack As EventHandler(Of DataPortalResult(Of Document)))

    '  Document.BeginGetDocument(New Criteria With {.DocumentID = DocumentID}, Sub(o, e)
    '                                                                            If e.Error IsNot Nothing Then
    '                                                                              CallBack.Invoke(Nothing, New DataPortalResult(Of Document)(Nothing, e.Error, e.UserState))
    '                                                                            Else
    '                                                                              If e.Object.Count = 0 Then
    '                                                                                CallBack.Invoke(Nothing, New DataPortalResult(Of Document)(Nothing, New Exception("No Document found for DocumentID " & DocumentID), e.UserState))
    '                                                                              Else
    '                                                                                CallBack.Invoke(Nothing, New DataPortalResult(Of Document)(e.Object(0), e.Error, e.UserState))
    '                                                                              End If
    '                                                                            End If
    '                                                                          End Sub)

    'End Function

    Public Shared Sub BeginGetDocument(DocumentID As Integer, ByVal CallBack As EventHandler(Of DataPortalResult(Of Document)))

      Dim dp As New DataPortal(Of Document)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria With {.DocumentID = DocumentID})

    End Sub


#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Sub DirectSave()

      If Me.IsDeleted Then
        DeleteSelfGeneric()
      Else
        InsertUpdateGeneric()
      End If

    End Sub

    Public Shared Function GetDocument(ByVal DocumentID As Integer, Optional ConnectionString As String = "") As Document

      Return DataPortal.Fetch(Of Document)(New Criteria With {.DocumentID = DocumentID, .ConnectionString = ConnectionString})

    End Function

    Public Shared Function GetDocumentFromHash(ByVal DocumentHash As String) As Document

      Return DataPortal.Fetch(Of Document)(New Criteria With {.HashCode = DocumentHash})

    End Function

    Public Shared Function GetDocument(ByVal dr As SafeDataReader) As Document

      Dim c As New Document()
      c.Fetch(dr)
      Return c

    End Function


    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      Using BypassPropertyChecks
        With sdr
          LoadProperty(DocumentIDProperty, .GetInt32(0))
          LoadProperty(DocumentNameProperty, .GetString(1))

          Dim DocumentBytes As Byte() = .GetValue(2)

          If DecryptBytes IsNot Nothing AndAlso DocumentBytes IsNot Nothing Then
            DocumentBytes = DecryptBytes(DocumentBytes)
          End If

          If DocumentBytes IsNot Nothing Then
            Dim ms As New IO.MemoryStream(DocumentBytes)
            Dim msOut As New IO.MemoryStream
            Try
              Dim zipFile As ICSharpCode.SharpZipLib.Zip.ZipFile = New ICSharpCode.SharpZipLib.Zip.ZipFile(ms)
              ms.Position = 0

              If IO.Path.GetExtension(GetProperty(DocumentNameProperty)) <> ".zip" AndAlso zipFile.TestArchive(True) AndAlso zipFile.Count = 1 Then
                ms.Position = 0
                For Each zipEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry In zipFile
                  If Not zipEntry.IsFile Then     ' Ignore directories
                    Continue For
                  End If
                  Dim entryFileName As [String] = zipEntry.Name
                  Dim buffer As Byte() = New Byte(4095) {}    ' 4K is optimum
                  Dim zipStream As IO.Stream = zipFile.GetInputStream(zipEntry)

                  ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipStream, msOut, buffer)
                  DocumentBytes = msOut.ToArray

                  Exit For

                Next

              End If
            Catch ex As ICSharpCode.SharpZipLib.Zip.ZipException
              'cannot extract zip file so file is probably not a zipped file
            Finally
              ms.Close()
              msOut.Close()
            End Try

          End If
          LoadProperty(DocumentProperty, DocumentBytes)

          LoadProperty(CreatedByProperty, .GetInt32(3))
          LoadProperty(CreatedDateTimeProperty, .GetSmartDate(4))
          LoadProperty(ModifiedByProperty, .GetInt32(5))
          LoadProperty(ModifiedDateTimeProperty, .GetSmartDate(6))
        End With
      End Using

      MarkOld()
      BusinessRules.CheckRules()

    End Sub

    Public Shared Property GetProcName As String = "GetProcs.getDocument"

    Protected Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(If(crit.ConnectionString = "", Singular.Settings.ConnectionString, crit.ConnectionString))
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.StoredProcedure
          cm.CommandTimeout = 360
          cm.CommandText = GetProcName
          cm.Parameters.AddWithValue("@DocumentID", Singular.Misc.ZeroDBNull(crit.DocumentID))
          If crit.HashCode <> "" Then
            Dim HashCode As String = crit.HashCode
            Dim ByteLength As Integer = (crit.HashCode.Length / 2) - 1
            Dim Bytes(ByteLength) As Byte
            For i As Integer = 0 To ByteLength
              Bytes(i) = Convert.ToByte(HashCode.Substring(i * 2, 2), 16)
            Next
            cm.Parameters.AddWithValue("@HashCode", Bytes)
          End If
          If Settings.PassUserIDToGetProc Then
            cm.Parameters.AddWithValue("@UserID", Singular.Settings.CurrentUserID)
          End If

          If Settings.AddParametersToFetch IsNot Nothing Then Settings.AddParametersToFetch(crit, cm)

          Using sdr As New SafeDataReader(cm.ExecuteReader)
            If sdr.Read Then
              Fetch(sdr)
            End If
          End Using
        End Using
      End Using

    End Sub

    ' Public Shared Property InsProcName As String = "InsProcs.insDocument"

    'Friend Sub Insert()

    '  ' if we're not dirty then don't update the database
    '  Using cm As SqlCommand = New SqlCommand
    '    cm.CommandText = "InsProcs.insDocument"

    '    DoInsertUpdateChild(cm)

    '  End Using

    'End Sub

    '' Public Shared Property UpdProcName As String = "UpdProcs.updDocument"

    'Friend Sub Update()

    '  ' if we're not dirty then don't update the database
    '  Using cm As SqlCommand = New SqlCommand
    '    cm.CommandText = "UpdProcs.updDocument"

    '    DoInsertUpdateChild(cm)

    '  End Using

    'End Sub

    Public Shared Property DefaultProcNameSuffix As String = "Document"
    Public Shared EncryptBytes As Func(Of Byte(), Byte())
    Public Shared DecryptBytes As Func(Of Byte(), Byte())

    Protected Friend Overrides Function ProcNameSuffix() As String
      Return DefaultProcNameSuffix
    End Function

    Public Function GetSizeInMB() As Single

      Return If(Document Is Nothing, 0, (Document.Length / 1024.0F) / 1024.0F)

    End Function

    Public Shared Function MBToBytes(MB As Single) As Single

      Return MB * 1024.0F * 1024.0F

    End Function

    Public Function GetSizeString() As Single

      Return If(Document Is Nothing, BytesToString(0), BytesToString(Document.Length))

    End Function

    Public Shared Function BytesToString(byteCount As Long) As String
      Dim suf = {"B", "KB", "MB", "GB", "TB", "PB", "EB"} '//Longs run out around EB
      If (byteCount = 0) Then
        Return "0" + suf(0)
      End If
      Dim bytes = Math.Abs(byteCount)
      Dim place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)))
      Dim num = Math.Round(bytes / Math.Pow(1024, place), 1)
      Return (Math.Sign(byteCount) * num).ToString() + suf(place)
    End Function


    Public Shared Property AddAdditionalCommandParameters As Action(Of Document, SqlCommand)

    Protected Friend Overrides Function SetupSaveCommand(cm As SqlCommand) As Action(Of SqlCommand)

      AddPrimaryKeyParam(cm, DocumentIDProperty)
      cm.Parameters.AddWithValue("@DocumentName", GetProperty(DocumentNameProperty))
      If EncryptBytes IsNot Nothing Then
        cm.Parameters.AddWithValue("@Document", EncryptBytes(Document))
      Else
        cm.Parameters.AddWithValue("@Document", Document)
      End If

      If Me.Parent IsNot Nothing AndAlso CType(Me.Parent, IDocumentProvider).OverridesModifiedBy.HasValue AndAlso CType(Me.Parent, IDocumentProvider).OverridesModifiedBy <> 0 Then
        cm.Parameters.AddWithValue("@ModifiedBy", CType(Me.Parent, IDocumentProvider).OverridesModifiedBy)
      ElseIf GetProperty(OverridesModifiedByProperty).HasValue Then
        cm.Parameters.AddWithValue("@ModifiedBy", GetProperty(OverridesModifiedByProperty))
      Else
        cm.Parameters.AddWithValue("@ModifiedBy", Singular.Settings.CurrentUserID)
      End If

      If Settings.DocumentHashesEnabled Then
        cm.Parameters.AddWithValue("@DocumentHash", HashBytes)
      End If

      If AddAdditionalCommandParameters IsNot Nothing Then
        AddAdditionalCommandParameters.Invoke(Me, cm)
      End If

      Return Sub()
               If IsNew Then
                 LoadProperty(DocumentIDProperty, cm.Parameters("@DocumentID").Value)
               End If
             End Sub

    End Function

    Protected Overrides Sub SetupDeleteCommand(cm As SqlCommand)
      cm.Parameters.AddWithValue("@DocumentID", GetProperty(DocumentIDProperty))
    End Sub

    'Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

    '  If MyBase.IsDirty Then

    '    With cm
    '      .CommandType = CommandType.StoredProcedure

    '      Dim paramDocumentID As SqlParameter = .Parameters.Add("@DocumentID", SqlDbType.Int)
    '      paramDocumentID.Value = GetProperty(DocumentIDProperty)
    '      If Me.IsNew Then
    '        paramDocumentID.Direction = ParameterDirection.Output
    '      End If


    '      .ExecuteNonQuery()

    '      If Me.IsNew() Then
    '        LoadProperty(DocumentIDProperty, paramDocumentID.Value)
    '      End If
    '      ' update child objects
    '      ' mChildList.Update()
    '      MarkOld()
    '    End With
    '  Else
    '    ' update child objects
    '    ' mChildList.Update()
    '  End If

    'End Sub

    ' Public Shared Property DelProcName As String = "DelProcs.delDocument"

    'Friend Sub DeleteSelf()

    '  ' if we're not dirty then don't update the database
    '  If Me.IsNew Then Exit Sub

    '  Using cm As SqlCommand = New SqlCommand
    '    cm.CommandText = "DelProcs.delDocument"
    '    cm.CommandType = CommandType.StoredProcedure
    '    cm.Parameters.AddWithValue("@DocumentID", GetProperty(DocumentIDProperty))
    '    DoDeleteChild(cm)
    '  End Using

    'End Sub

    'Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)

    '  If Me.IsNew Then Exit Sub

    '  With cm
    '    .ExecuteNonQuery()
    '  End With
    '  'MarkNew()

    'End Sub

#End If

#End Region

#End Region

  End Class


End Namespace