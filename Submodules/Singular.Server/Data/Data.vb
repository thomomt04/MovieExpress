Imports System.Reflection
Imports System.IO
Imports System
Imports Csla
Imports Infragistics.Documents.Excel
Imports System.Drawing
Imports System.Linq

#If SILVERLIGHT Then

#Else
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports Microsoft.VisualBasic.FileIO
#End If

Namespace Data

#If SILVERLIGHT Then
#Else

#Region " General Data Functions "

  ' Module DataFunctions

  Public Class DataFunctions

    Public Shared Function SubstituteParameterInString(ByRef SqlString As String, ByVal ParameterName As String, ByVal ParameterValue As String, Optional ByVal AddStringQuotes As Boolean = False) As String

      If AddStringQuotes Then
        SqlString = SqlString.Replace(ParameterName, "'" & ParameterValue & "'")
      Else
        SqlString = SqlString.Replace(ParameterName, ParameterValue)
      End If

      Return SqlString

    End Function

    Public Shared Function AddParameterToString(ByRef OriginalString As String, ByVal FieldName As String, ByVal Value As Object, ByVal TreatAsString As Boolean, Optional ByVal WildCard As Boolean = True, Optional ByVal CaseSensitive As Boolean = False) As String

      If OriginalString.EndsWith(";") Then
        OriginalString = OriginalString.TrimEnd(";".ToCharArray)
      End If

      If Not OriginalString.EndsWith(vbTab) Then
        ' no line feed and tab
        If OriginalString.EndsWith(vbCrLf) Then
          OriginalString &= vbTab
        Else
          OriginalString &= vbCrLf & vbTab
        End If
      End If

      If Not OriginalString.ToLower.Contains("where") Then
        ' no where clause, add it
        OriginalString &= "WHERE "
      ElseIf Not OriginalString.ToLower.EndsWith("and") Then
        ' does have a where, does not end with an and, check if anything after the where
        If OriginalString.Substring(OriginalString.ToLower.IndexOf("where") + 5).Replace(" ", "").Replace(vbCrLf, "").Replace(vbTab, "") <> "" Then
          ' is something after the where so add the AND
          OriginalString &= "AND "
        End If
      End If

      Dim ValueString As String = ""
      If TreatAsString Then
        If Not IsDBNull(Value) AndAlso Not IsNothing(Value) Then
          ValueString = CStr(Value)
        End If

        If Not CaseSensitive Then
          OriginalString &= "Upper(" & FieldName & ") "
          If Not IsNothing(ValueString) AndAlso ValueString <> "" Then
            ValueString = ValueString.ToUpper
          End If
        Else
          OriginalString &= FieldName
        End If
        If WildCard Then
          OriginalString &= " LIKE '%" & ValueString & "%'"
        Else
          OriginalString &= " = '" & ValueString & "'"
        End If
      Else
        If Not IsDBNull(Value) AndAlso Not IsNothing(Value) AndAlso (Not TypeOf Value Is String OrElse Not Value = "") Then
          ValueString = CStr(Value)
          OriginalString &= FieldName & " = " & ValueString
        End If
      End If

      OriginalString &= vbCrLf & vbTab

      Return OriginalString

    End Function

    Public Shared Sub AddParameterToCommand(ByRef Command As SqlClient.SqlCommand, ByVal ParameterName As String, ByVal ParameterValue As Object)

      If Not ParameterName.StartsWith("@") Then
        ParameterName = "@" & ParameterName
      End If
      Command.Parameters.AddWithValue(ParameterName, IIf(IsNothing(ParameterValue) OrElse (TypeOf ParameterValue Is String AndAlso ParameterValue = ""), DBNull.Value, ParameterValue))

    End Sub

    ''' <summary>
    ''' Populates an object based on the field names returned from a query
    ''' </summary>
    Public Shared Sub PopulateObject(obj As Object, sdr As Csla.Data.SafeDataReader)
      Dim ObjectType = obj.GetType()

      For i As Integer = 0 To sdr.FieldCount - 1
        Dim Name As String = sdr.GetName(i)
        Dim PropertyInfo = ObjectType.GetProperty(Name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
        If PropertyInfo IsNot Nothing Then
          PropertyInfo.SetValue(obj, sdr.GetValue(i), Nothing)
        End If
      Next
    End Sub

    ''' <summary>
    ''' Populates a list by creating an object of the supplied type for each row returned. Field names must match property names.
    ''' </summary>
    Public Shared Sub PopulateList(Of ObjectType)(list As IList, sdr As Csla.Data.SafeDataReader)
      While sdr.Read
        Dim obj = Activator.CreateInstance(Of ObjectType)()
        PopulateObject(obj, sdr)
        list.Add(obj)
      End While
    End Sub

  End Class

  ' End Module

#End Region

#Region " Files "

  Public Class Files

    ''' <summary>
    ''' Moves the given file to new the location folder
    ''' </summary>
    ''' <param name="FileName">Filename to be moved</param>
    ''' <param name="LocationFolder">Location file is moved to</param>
    ''' <param name="CreateFolder">Pass true to create folder if it does not exist</param>
    ''' <remarks></remarks>
    Public Shared Sub MoveFileToLocation(ByVal FileName As String, ByVal LocationFolder As String, Optional ByVal CreateFolder As Boolean = False)

      If Directory.Exists(LocationFolder) Or CreateFolder Then
        If Not Directory.Exists(LocationFolder) Then
          Directory.CreateDirectory(LocationFolder)
        End If
        If File.Exists(FileName) Then
          If Not LocationFolder.EndsWith("\") Then
            LocationFolder &= "\"
          End If
          File.Move(FileName, LocationFolder & GetFileNameFromFullFileName(FileName))
        Else
          Throw New Exception("File '" & FileName & "' not found")
        End If
      Else
        Throw New Exception("Folder '" & LocationFolder & "' not found")
      End If

    End Sub

    ''' <summary>
    ''' Moves the given file to new the location folder
    ''' </summary>
    ''' <param name="FileName">Filename to be moved</param>
    ''' <param name="LocationFolder">Location file is moved to</param>
    ''' <param name="CreateFolder">Pass true to create folder if it does not exist</param>
    ''' <remarks></remarks>
    Public Shared Sub MoveFileToLocation(ByVal FileName As String, ByVal LocationFolder As String, ByVal PrefixNameWith As String, Optional ByVal CreateFolder As Boolean = False)

      If Directory.Exists(LocationFolder) Or CreateFolder Then
        If Not Directory.Exists(LocationFolder) Then
          Directory.CreateDirectory(LocationFolder)
        End If
        If File.Exists(FileName) Then
          If Not LocationFolder.EndsWith("\") Then
            LocationFolder &= "\"
          End If
          File.Move(FileName, LocationFolder & PrefixNameWith & GetFileNameFromFullFileName(FileName))
        Else
          Throw New Exception("File '" & FileName & "' not found")
        End If
      Else
        Throw New Exception("Folder '" & LocationFolder & "' not found")
      End If

    End Sub

    ''' <summary>
    ''' Returnes the File Name from a full file name (file name with path)
    ''' </summary>
    ''' <param name="FullFileName">The full file name</param>
    ''' <param name="StripOffExtension">Pass in true to strip off the extension</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFileNameFromFullFileName(ByVal FullFileName As String, Optional ByVal StripOffExtension As Boolean = False) As String

      Dim FileName As String = FullFileName.Substring(FullFileName.LastIndexOf("\") + 1)
      If StripOffExtension Then
        FileName = FileName.Substring(0, FileName.ToLower.LastIndexOf("."))
      End If
      Return FileName

    End Function

    Public Shared Function GetFolderNameOffFullFileName(ByVal FullFileName As String) As String

      Return FullFileName.Substring(0, FullFileName.LastIndexOf("\"))

    End Function

    <Serializable()> _
    Public Class TextFileExporter

      Private mFullFileName As String = ""
      Private mStringBuilder As New System.Text.StringBuilder

      Public Property FileFullName() As String
        Get
          Return mFullFileName
        End Get
        Set(ByVal value As String)
          mFullFileName = value
        End Set
      End Property

      Public Sub New()

      End Sub

      Private mContentAdded As Boolean = False

      Public ReadOnly Property ContentAdded() As Boolean
        Get
          Return mContentAdded
        End Get
      End Property

      Public Overridable Sub AddLine(ByVal Line As String)

        mContentAdded = True

        If mAttemptedToClose Then
          Throw New Exception("Do not add lines after attempting to close, call clear first")
        End If

        mStringBuilder.AppendLine(Line)

      End Sub

      Public Sub Clear()

        mStringBuilder = New System.Text.StringBuilder()

        mStringBuilder.AppendLine(mHeader)

      End Sub

      Protected Sub AddHeaderLine(ByVal HeaderLine As String)

        mStringBuilder.AppendLine(HeaderLine)

        mHeader = mStringBuilder.ToString

      End Sub

      Protected mHeader As String = ""

      Private mAttemptedToClose As Boolean = False

      Public Overridable Sub SaveAndClose(Optional ByVal FileName As String = "")

        If FileName <> "" Then
          mFullFileName = FileName
        End If

        mAttemptedToClose = True
        If mFullFileName = "" Then
          Throw New Exception("Please specify a Full File Name for the Text File Exporter")
        End If

        Try
          Using Writer As New System.IO.StreamWriter(mFullFileName)
            Writer.WriteLine(mStringBuilder.ToString)
            Writer.Flush()
          End Using
        Catch ex As Exception
          Throw New Exception("Error writing file", ex)
        End Try

      End Sub

    End Class

    ''' <summary>
    ''' Imports tab delimited text file
    ''' </summary>
    ''' <param name="FileName">Full File Name to import</param>
    ''' <param name="FirstLineIsColumnHeaders">Pass true if the first line of the file contains the column headers</param>
    ''' <param name="TableName">Name of DataTable returned (</param>
    ''' <param name="Errors">Errors encountered during import process, Check if Empty to ensure correct import</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ImportTabDelimitedTextFile(ByVal FileName As String, ByVal FirstLineIsColumnHeaders As Boolean, ByRef Errors As String, Optional ByVal TableName As String = "") As DataTable

      Dim tfp As New TextFieldParser(FileName)
      tfp.Delimiters = New String() {ControlChars.Tab}
      tfp.HasFieldsEnclosedInQuotes = True
      tfp.TextFieldType = FieldType.Delimited
      tfp.TrimWhiteSpace = True

      Dim tbl As DataTable = Nothing
      If TableName = "" Then
        tbl = New DataTable("Table")
      Else
        tbl = New DataTable(TableName)
      End If

      Dim data() As String
      Dim FirstLine As Boolean = True
      Dim LineNo As Integer = 1
      While Not tfp.EndOfData()
        Try
          data = tfp.ReadFields()
          ' The string variable data now holds all the fields in the line of the file
          ' So if there are 3 fields in each line of the tab delimited text file data
          ' will have 3 elements in the array. Use each field to update the database.
          ' The following is an example not knowing the structure of the database
          If FirstLineIsColumnHeaders AndAlso FirstLine Then
            FirstLine = False
            For Each field As String In data
              tbl.Columns.Add(field)
            Next
          Else
            Dim drw As DataRow = tbl.NewRow()
            For i As Integer = 0 To data.Length - 1
              Try
                drw(i) = data(i)
              Catch ex As Exception
                Errors &= "Error importing Line " & LineNo & " Field " & i & ": " & ex.Message & vbCrLf
              End Try
            Next
            tbl.Rows.Add(drw)
          End If
        Catch ex As Exception
          Errors &= "Error importing Line " & LineNo & ": " & ex.Message & vbCrLf
        End Try
        LineNo += 1
      End While

      tfp.Close()
      ' Update the database
      Return tbl

    End Function

#Region "File Path to Unc conversion"

    Public Class UncPath

      Private Declare Function WNetGetConnection32 Lib "mpr.dll" Alias _
            "WNetGetConnectionA" (ByVal lpszLocalName As String, ByVal _
            lpszRemoteName As String, ByRef lSize As Integer) As Integer
      Const NO_ERROR As Long = 0

      Public Shared Function GetUncPath(ByVal FilePath As String) As String
        Dim OriginalFilePath As String = FilePath
        Try
          Dim lpszRemoteName As String = New String(" "c, 256)
          Dim Lsize As Integer = 256
          Dim PathInfo As FileInfo = New IO.FileInfo(FilePath)
          Dim FileArray() As String = FilePath.Split(":"c)
          If FileArray.GetUpperBound(0) <> 1 Then
            Return FilePath
          Else
            FileArray(0) += ":"
            Dim lStatus As Integer = WNetGetConnection32(FileArray(0), lpszRemoteName, Lsize)
            If lStatus = NO_ERROR Then
              lpszRemoteName = lpszRemoteName.Trim()
              Return lpszRemoteName.Substring(0, lpszRemoteName.Length - 1) & FileArray(1)
            Else
              Return FilePath
            End If
          End If
        Catch ex As Exception
          Return OriginalFilePath
        End Try
      End Function

    End Class

#End Region

    Public Shared Sub WriteStringToFile(ByVal Message As String, ByVal Filename As String)

      Using fw As New IO.FileStream(Filename, IO.FileMode.Create)
        Using sw As New IO.StreamWriter(fw)
          sw.Write(Message)
          sw.Flush()
          fw.Flush()
        End Using
      End Using

    End Sub

    Public Shared Function ReadStringFromFile(ByVal Filename As String) As String

      Using fw As New IO.FileStream(Filename, IO.FileMode.Open)
        Using sw As New IO.StreamReader(fw)
          Return sw.ReadToEnd()
        End Using
      End Using

    End Function

  End Class

#End Region

#Region " Data Tables "

  Public Class DataTables

    Public Class CrossTabCriteria
      Public RowColumns() As String
      Public RowSplits() As String
      Public ColumnColumn As String
      Public Aggregation As String
      Public ReplaceSpaceWithReturn As Boolean
      Public DontReturnForWordsLessThan As Integer
      Public DefaultValue As Object = 0

      Public Sub New(ByVal RowColumns() As String, _
          ByVal ColumnColumn As String, _
          ByVal Aggregation As String, Optional ByVal DefaultValue As Object = 0, _
          Optional ByVal RowSplits() As String = Nothing, _
          Optional ByVal ReplaceSpaceWithReturn As Boolean = False, _
          Optional ByVal DontReturnForWordsLessThan As Integer = 10)

        Me.RowColumns = RowColumns
        Me.ColumnColumn = ColumnColumn
        Me.Aggregation = Aggregation
        Me.RowSplits = RowSplits
        Me.ReplaceSpaceWithReturn = ReplaceSpaceWithReturn
        Me.DefaultValue = DefaultValue
        Me.DontReturnForWordsLessThan = DontReturnForWordsLessThan

      End Sub

    End Class

    Public Shared Function CrossTabTable(ByVal Table As DataTable, ByVal CrossTabCriteria As CrossTabCriteria, Optional ByVal TableName As String = "CrossTab") As DataTable

      Dim tblReturn As New DataTable(TableName)
      ' the first column must be description
      tblReturn.Columns.Add("Description", GetType(String))
      ' need to add the columns to the table
      Dim sColName As String
      For Each drw As DataRow In Table.Rows
        sColName = drw(CrossTabCriteria.ColumnColumn)
        If CrossTabCriteria.ReplaceSpaceWithReturn Then
          Dim sColParts() As String = sColName.Split
          sColName = ""
          For i As Integer = 0 To sColParts.Length - 1
            If i > 0 Then
              If (sColParts(i - 1) & " " & sColParts(i)).Length < CrossTabCriteria.DontReturnForWordsLessThan Then
                ' must not return 
                sColName &= " " & sColParts(i)
                sColParts(i - 1) &= " " & sColParts(i)
              Else
                ' too long, place return
                sColName &= vbCrLf & sColParts(i)
              End If
            Else
              sColName = sColParts(i)
            End If
          Next
        End If
        ' if we dont have this column then add it
        If Not tblReturn.Columns.Contains(sColName) Then
          tblReturn.Columns.Add(sColName, Table.Columns(CrossTabCriteria.Aggregation).DataType).DefaultValue = CrossTabCriteria.DefaultValue
        End If
      Next

      ' now we have all the columns, so populate the table
      Dim drws() As DataRow
      Dim sDescription As String
      For Each drw As DataRow In Table.Rows
        sDescription = ""
        For i As Integer = 0 To CrossTabCriteria.RowColumns.Length - 1
          If i > 0 Then
            If Not IsNothing(CrossTabCriteria.RowSplits) AndAlso i <= CrossTabCriteria.RowSplits.Length Then
              sDescription &= CrossTabCriteria.RowSplits(i - 1)
            Else
              sDescription &= " "
            End If
          End If
          sDescription &= drw(CrossTabCriteria.RowColumns(i))
        Next
        sColName = drw(CrossTabCriteria.ColumnColumn)
        If CrossTabCriteria.ReplaceSpaceWithReturn Then
          Dim sColParts() As String = sColName.Split
          sColName = ""
          For i As Integer = 0 To sColParts.Length - 1
            If i > 0 Then
              If (sColParts(i - 1) & " " & sColParts(i)).Length < CrossTabCriteria.DontReturnForWordsLessThan Then
                ' must not return 
                sColName &= " " & sColParts(i)
                sColParts(i - 1) &= " " & sColParts(i)
              Else
                ' too long, place return
                sColName &= vbCrLf & sColParts(i)
              End If
            Else
              sColName = sColParts(i)
            End If
          Next
        End If
        drws = tblReturn.Select("Description = '" & sDescription & "'")
        If drws.Length > 0 Then
          ' we already have this row, so just set the column
          drws(0)(sColName) = drw(CrossTabCriteria.Aggregation)
        Else
          ' does not exist so need to add it
          Dim drwNew As DataRow = tblReturn.NewRow
          drwNew("Description") = sDescription
          drwNew(sColName) = drw(CrossTabCriteria.Aggregation)
          tblReturn.Rows.Add(drwNew)
        End If
      Next
      Return tblReturn

    End Function

    Public Shared Function CrossTabTable(ByVal Table As DataTable, ByVal RowColumns() As String, _
                                                                    ByVal ColumnColumn As String, _
                                                                    ByVal Aggregation As String, _
                                                                    Optional ByVal DefaultValue As Object = 0, Optional ByVal RowSplits() As String = Nothing, _
                                                                    Optional ByVal TableName As String = "CrossTab", _
                                                                    Optional ByVal ReplaceSpaceWithReturn As Boolean = False, _
                                                                    Optional ByVal DontReturnForWordsLessThan As Integer = 10) As DataTable

      Return CrossTabTable(Table, New CrossTabCriteria(RowColumns, ColumnColumn, Aggregation, DefaultValue, RowSplits, ReplaceSpaceWithReturn, DontReturnForWordsLessThan), TableName)

    End Function

    ''' <summary>
    ''' Returns a DataTable with 2 columns: Value and Display. Usefull for creating a drop down datasource where you dont have a list / database table.
    ''' </summary>
    ''' <param name="Values"></param>
    ''' <param name="NoOfColumns"></param>
    ''' <param name="AllowNulls"></param>
    ''' <param name="Seperator"></param>
    ''' <param name="NullValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetValueSeparatedDataTable(ByVal Values As String, Optional ByVal NoOfColumns As Integer = 2, Optional ByVal AllowNulls As Boolean = False, Optional ByVal Seperator As String = ";", Optional ByVal NullValue As Object = Nothing) As DataTable

      Dim Vals As String() = Values.Split(Seperator)
      If NoOfColumns <> 1 And NoOfColumns <> 2 Then
        Throw New Exception("The number of columns can only be 1 or 2")
      End If
      If NoOfColumns = 2 Then
        If Vals.Length Mod 2 <> 0 Then
          Throw New Exception("There must be an even number of values for 2 columns")
        End If
      End If

      Dim tbl As New DataTable
      tbl.Columns.Add("Value")
      tbl.Columns.Add("Display")

      If AllowNulls Then
        If NullValue Is Nothing Then
          tbl.Rows.Add(New Object() {"", ""})
        Else
          tbl.Rows.Add(New Object() {0, ""})
        End If

      End If
      For i As Integer = 0 To Vals.Length - 1 Step NoOfColumns
        tbl.Rows.Add(New Object() {Vals(i), Vals(i + (NoOfColumns - 1))})
      Next

      Return tbl

    End Function

    Public Shared Function TableColumnsTheSame(ByVal Table1 As DataTable, ByVal Table2 As DataTable) As Boolean

      For Each clm As DataColumn In Table1.Columns
        If Not Table2.Columns.Contains(clm.ColumnName) Then
          Return False
        End If
      Next

      For Each clm As DataColumn In Table2.Columns
        If Not Table1.Columns.Contains(clm.ColumnName) Then
          Return False
        End If
      Next

      Return True

    End Function

    Public Shared Function GetEnumDataTable(ByVal EnumType As Type) As DataTable

      Dim tbl As New DataTable
      tbl.Columns.Add("Value", GetType(Integer))
      tbl.Columns.Add("Display")
      Dim drw As DataRow
      For Each enu As Object In [Enum].GetValues(EnumType)
        drw = tbl.NewRow
        drw("Value") = CInt(enu)
        drw("Display") = Strings.Readable(enu.ToString.Replace("_", ""))
        tbl.Rows.Add(drw)
      Next
      Return tbl

    End Function

    Public Class DataRowReader

      Private mDataRow As DataRow

      Public Function GetInteger(ByVal FieldName As String) As Integer

        If IsDBNull(mDataRow(FieldName)) OrElse CStr(mDataRow(FieldName)) = "NULL" Then
          Return 0
        Else
          Return CInt(mDataRow(FieldName))
        End If

      End Function

      Public Function GetDecimal(ByVal FieldName As String) As Decimal

        If IsDBNull(mDataRow(FieldName)) OrElse CStr(mDataRow(FieldName)) = "NULL" Then
          Return 0
        Else
          Return CDec(mDataRow(FieldName))
        End If

      End Function

      Public Function GetString(ByVal FieldName As String) As String

        If IsDBNull(mDataRow(FieldName)) OrElse CStr(mDataRow(FieldName)) = "NULL" Then
          Return 0
        Else
          Return CStr(mDataRow(FieldName))
        End If

      End Function

      Public Function GetSmartDate(ByVal FieldName As String) As SmartDate

        If IsDBNull(mDataRow(FieldName)) OrElse CStr(mDataRow(FieldName)) = "NULL" Then
          Return New SmartDate
        Else
          Return New SmartDate(CDate(mDataRow(FieldName)))
        End If

      End Function

      Public Function GetInteger(ByVal Index As Integer) As Integer

        If IsDBNull(mDataRow(Index)) OrElse CStr(mDataRow(Index)) = "NULL" Then
          Return 0
        Else
          Return CInt(mDataRow(Index))
        End If

      End Function

      Public Function GetDecimal(ByVal Index As Integer) As Decimal

        If IsDBNull(mDataRow(Index)) OrElse CStr(mDataRow(Index)) = "NULL" Then
          Return 0
        Else
          Return CDec(mDataRow(Index))
        End If

      End Function

      Public Function GetString(ByVal Index As Integer) As String

        If IsDBNull(mDataRow(Index)) OrElse CStr(mDataRow(Index)) = "NULL" Then
          Return 0
        Else
          Return CStr(mDataRow(Index))
        End If

      End Function

      Public Function GetSmartDate(ByVal Index As Integer) As SmartDate

        If IsDBNull(mDataRow(Index)) OrElse CStr(mDataRow(Index)) = "NULL" Then
          Return New SmartDate
        Else
          Return New SmartDate(CDate(mDataRow(Index)))
        End If

      End Function

      Public Sub New(ByVal DataRow As DataRow)

        mDataRow = DataRow

      End Sub

    End Class

    Public Enum ExtendedProperties
      AutoGenerate
      ReportCriteria
      Width
      FormatString
      TextAlign
      ExcludeFromTotal
      FormulaField
      ExcelFormat
      ''' <summary>
      ''' For column tooltips.
      ''' </summary>
      Description
    End Enum

    Public Shared Sub AddExtendedInfo(DataSet As DataSet, Optional HideIDColumns As Boolean = False)
      For Each Table As DataTable In DataSet.Tables
        AddExtendedInfo(Table, HideIDColumns)
      Next
    End Sub

    Public Shared Sub AddExtendedInfo(DataTable As DataTable, Optional HideIDColumns As Boolean = False)
      For Each Column As DataColumn In DataTable.Columns
        AddExtendedInfo(Column, HideIDColumns)
      Next
    End Sub

    Public Shared Sub ApplyExtendedInfo(DataColumn As DataColumn, pi As PropertyInfo)

      Dim nf = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(pi),
          df = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateField)(pi)

      If nf IsNot Nothing AndAlso nf.HasFormat Then
        SetExtProperty(DataColumn, ExtendedProperties.FormatString, nf.GetFormatString)
      End If
      If df IsNot Nothing AndAlso Not String.IsNullOrEmpty(df.FormatString) Then
        SetExtProperty(DataColumn, ExtendedProperties.FormatString, df.FormatString)
      End If

    End Sub

    Public Shared Function GetForumulaColumnFormula(Column As DataColumn) As CSLALib.FormulaField
      Return GetExtProperty(Column, ExtendedProperties.FormulaField.ToString, Nothing)
    End Function

    Public Shared Function AutoGenerateColumn(Column As DataColumn) As Boolean
      Return GetExtProperty(Column, ExtendedProperties.AutoGenerate.ToString, True)
    End Function

    Public Shared Function ExcludeFromTotal(Column As DataColumn) As Boolean
      Return GetExtProperty(Column, ExtendedProperties.ExcludeFromTotal.ToString, False)
    End Function

    Public Shared Function ColumnWidth(Column As DataColumn, Optional DefaultWidth As Integer = -1) As Integer
      If AutoGenerateColumn(Column) Then
        Return GetExtProperty(Column, ExtendedProperties.Width.ToString, 100)
      Else
        Return 0
      End If
    End Function

    Public Shared Function GetExtProperty(Column As DataColumn, Type As ExtendedProperties, DefaultValue As Object) As Object
      Return GetExtProperty(Column, Type.ToString, DefaultValue)
    End Function

    Public Shared Function GetExtProperty(Column As DataColumn, PropertyName As String, DefaultValue As Object) As Object
      If Column.ExtendedProperties.ContainsKey(PropertyName) Then
        Return Column.ExtendedProperties(PropertyName)
      Else
        Return DefaultValue
      End If
    End Function

    Public Shared Sub SetExtProperty(Column As DataColumn, Type As ExtendedProperties, Value As Object)
      Column.ExtendedProperties(Type.ToString) = Value
    End Sub

    ''' <summary>
    ''' Adds extended info based on the column type.
    ''' </summary>
    ''' <param name="Column"></param>
    ''' <remarks></remarks>
    Public Shared Sub AddExtendedInfo(Column As DataColumn, Optional HideIDColumns As Boolean = False)

      'Only if it hasnt been done.
      If Column.ExtendedProperties.Count = 0 Then

        'Display
        If Column.Caption = "" OrElse Column.Caption = Column.ColumnName Then
          Column.Caption = Singular.Strings.Readable(Column.ColumnName)
        End If

        'Hidden
        If Singular.Reflection.TypeImplementsInterface(Column.DataType, GetType(Singular.ISingularBusinessListBase)) OrElse
             Column.DataType Is GetType(Byte()) OrElse
             (Column.ExtendedProperties(ExtendedProperties.AutoGenerate.ToString) IsNot Nothing AndAlso Column.ExtendedProperties(ExtendedProperties.AutoGenerate.ToString) = True) OrElse
             (HideIDColumns AndAlso Column.Caption.EndsWith(" ID")) Then
          Column.ExtendedProperties(ExtendedProperties.AutoGenerate.ToString) = False
        End If

        'Type Formatting
        Dim smi As New Singular.Reflection.SMemberInfo(Column.DataType)

        Dim Format As String = ""
        Dim Width As Integer = 150
        Dim Alignment As Integer = 0

        Select Case smi.DataTypeMain
          Case Reflection.SMemberInfo.MainType.Number
            Width = 120
            Alignment = 2
            If smi.IsInteger Then
              Format = "#,##0;(#,##0)"
            Else
              Format = "#,##0.00;(#,##0.00)"
            End If
          Case Reflection.SMemberInfo.MainType.Date
            Width = 120
            Alignment = 1
            Format = "dd MMM yyyy"

        End Select

        If Format <> "" Then
          Column.ExtendedProperties(Singular.Data.DataTables.ExtendedProperties.FormatString.ToString) = Format
        End If
        Column.ExtendedProperties(Singular.Data.DataTables.ExtendedProperties.TextAlign.ToString) = Alignment
        Column.ExtendedProperties(Singular.Data.DataTables.ExtendedProperties.Width.ToString) = Width

      End If

    End Sub

    ''' <summary>
    ''' Returns a sorted array of child rows based on the child tables DefaultView.Sort property.
    ''' </summary>
    Public Shared Function GetSortedChildRows(ParentRow As DataRow, TableRelation As DataRelation) As DataRow()

      Dim ChildRows As DataRow() = ParentRow.GetChildRows(TableRelation)

      'Split the sort string into words
      Dim SortParams As String() = TableRelation.ChildTable.DefaultView.Sort.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
      If SortParams.Length > 0 Then
        'only look at the first word (as the column to sort by), and the last word if there are two or more words (for the sort direction).
        Dim Column As DataColumn = TableRelation.ChildTable.Columns(SortParams(0))
        Dim Desc As Boolean = SortParams.Length > 1 AndAlso SortParams(SortParams.Length - 1).ToUpper = "DESC"
        Dim NullReplaceValue = If(Column.DataType Is GetType(Integer), 0, If(Column.DataType Is GetType(Date), Date.MinValue, ""))
        Array.Sort(ChildRows, New System.Comparison(Of DataRow)(Function(a, b)
                                                                  Dim aVal = a(Column)
                                                                  Dim bVal = b(Column)
                                                                  aVal = If(aVal Is Nothing OrElse aVal Is DBNull.Value, NullReplaceValue, aVal)
                                                                  bVal = If(bVal Is Nothing OrElse bVal Is DBNull.Value, NullReplaceValue, bVal)

                                                                  If Desc Then
                                                                    Return CType(bVal, IComparable).CompareTo(aVal)
                                                                  Else
                                                                    Return CType(aVal, IComparable).CompareTo(bVal)
                                                                  End If
                                                                End Function))

      End If

      Return ChildRows

    End Function

    ''' <summary>
    ''' Gets the display value of a cell using the columns format string if there is one.
    ''' </summary>
    Public Shared Function GetCellDisplayValue(Value As Object, Column As DataColumn) As String

      'Set the formatted value
      Dim StringValue As String
      If Value Is Nothing OrElse Value Is DBNull.Value Then
        StringValue = ""
      Else
        Dim Format As String = Singular.Data.DataTables.GetExtProperty(Column, Singular.Data.DataTables.ExtendedProperties.FormatString.ToString, "")
        If Format <> "" Then
          If Column.DataType Is GetType(Date) Then
            StringValue = CDate(Value).ToString(Format)
          Else
            Dim NValue As Decimal = Value
            StringValue = NValue.ToString(Format)
          End If
        Else

          StringValue = Value.ToString
        End If
      End If

      Return StringValue
    End Function

    ''' <summary>
    ''' Clears the data out of a table. Recurses throught the child relations, and clears those tables as well.
    ''' </summary>
    Public Shared Sub ClearDataTableAndChildTables(Table As DataTable)

      If Table.ChildRelations.Count > 0 Then
        For Each Relation As DataRelation In Table.ChildRelations
          ClearDataTableAndChildTables(Relation.ChildTable)
        Next
      End If

      Table.Clear()

    End Sub

    ''' <summary>
    ''' Copies the data from a collection of rows into a datatable. Recurses throught the child relations, and copies those rows as well.
    ''' </summary>
    Public Shared Sub CopyDataTableAndChildTables(FromRows As DataRow(), ToTable As DataTable)

      For Each FromRow In FromRows
        ToTable.ImportRow(FromRow)

        For Each Relation As DataRelation In FromRow.Table.ChildRelations
          Dim ToRelation As DataRelation = ToTable.ChildRelations(Relation.RelationName)

          CopyDataTableAndChildTables(FromRow.GetChildRows(Relation), ToRelation.ChildTable)

        Next
      Next

    End Sub

  End Class

#End Region

#Region " ODBC "

  Public Class Odbc

    Public Shared Function GetOdbcConnectionString(ByVal OdbcProvider As String, ByVal OdbcServer As String, ByVal OdbcDatabase As String, ByVal OdbcUserID As String, ByVal OdbcPassword As String) As String

      Return "Driver={" & OdbcProvider & "};" & _
                "Server=" & OdbcServer & ";" & _
                "Database=" & OdbcDatabase & ";" & _
                "Uid=" & OdbcUserID & ";" & _
                "Pwd=" & OdbcPassword & ""

    End Function

    Public Shared Function GetOdbcConnectionString(ByVal OdbcProvider As String, ByVal OdbcDatabase As String, ByVal OdbcUserID As String, ByVal OdbcPassword As String) As String

      Return "Driver={" & OdbcProvider & "};" & _
                "Database=" & OdbcDatabase & ";" & _
                "Uid=" & OdbcUserID & ";" & _
                "Pwd=" & OdbcPassword & ""
      '   "Server=" & OdbcServer & ";" & _
    End Function

  End Class

#End Region

#Region " Dsn "

  Public Class Dsn

    Public Shared Function GetDsnConnectionString(ByVal Dsn As String, ByVal UserID As String, ByVal Password As String) As String

      Return "Dsn=" & Dsn & ";Uid=" & UserID & ";Pwd=" & Password

    End Function

  End Class

#End Region

#Region " XML "

  Public Class XML

    Public Shared Function StringToXML(ByVal StringInput As String) As String

      Dim ds As New DataSet("DataSet")
      ds.Tables.Add("Table")
      ds.Tables("Table").Columns.Add("ID")
      ds.Tables("Table").Columns("ID").ColumnMapping = MappingType.Attribute

      ds.Tables("Table").Rows.Add(New Object() {StringInput})

      Dim w As New IO.StringWriter
      ds.WriteXml(w)
      Return (w.ToString)

    End Function

    'wtf does this have to do with XML?
    Public Shared Function CSVFileToArrayList(ByVal FileName As String, Optional ByVal MakeElementsUpperCase As Boolean = False) As String()

      Dim strArray() As String
      Using sr As New System.IO.StreamReader(FileName)
        Dim str As String = sr.ReadToEnd()
        If str.IndexOf(",") > -1 Then
          strArray = str.Split(",")
        Else
          strArray = str.Split(vbCrLf)
        End If
      End Using

      For i As Integer = 0 To strArray.Length - 1
        If MakeElementsUpperCase Then
          strArray(i) = strArray(i).ToUpper ' Converts the insturments brought in as an array to uppercase if they come in as lower
        End If
        strArray(i) = strArray(i).Trim
      Next

      Return strArray

    End Function

    Public Shared Function StringArrayToXML(ByVal StrArray() As String) As String

      Try

        Dim ds As New DataSet("DataSet")
        ds.Tables.Add("Table")
        ds.Tables("Table").Columns.Add("ID")
        ds.Tables("Table").Columns("ID").ColumnMapping = MappingType.Attribute

        For J As Integer = 0 To StrArray.Length - 1

          ds.Tables("Table").Rows.Add(New Object() {StrArray(J)})

        Next

        Dim w As New IO.StringWriter
        ds.WriteXml(w)
        Return (w.ToString)

      Catch ex As Exception
        Return String.Empty
      End Try

    End Function

    ''' <summary>
    ''' Converts the entries in a dictionary to XML for use in SQL.
    ''' </summary>
    Public Shared Function DictionaryToXML(Of KeyType, ValueType)(Values As Dictionary(Of KeyType, ValueType)) As String

      'Use this to select the data in SQL:
      'SELECT [Rows].[Row].value('(@Key)[1]', 'Int') As [Key],
      '       [Rows].[Row].value('(@Value)[1]', 'varchar(max)') As Value,
      '	      [Rows].[Row].value('(@DateValue)[1]', 'datetime') As DateValue
      'FROM @Data.nodes('//Rows/Row') AS [Rows]([Row]) 

      If Values Is Nothing Then
        Return Nothing
      Else

        Dim sb As New System.Text.StringBuilder
        Dim xmlSettings As New System.Xml.XmlWriterSettings
        xmlSettings.Encoding = System.Text.Encoding.UTF8

        Using xmlW = System.Xml.XmlWriter.Create(sb, xmlSettings)

          xmlW.WriteStartDocument()
          xmlW.WriteStartElement("Rows")

          For Each Entry In Values
            xmlW.WriteStartElement("Row")

            xmlW.WriteStartAttribute("Key")
            xmlW.WriteValue(Entry.Key)
            xmlW.WriteEndAttribute()

            If TypeOf Entry.Value Is Date Then
              xmlW.WriteStartAttribute("DateValue")
              xmlW.WriteValue(CDate(CType(Entry.Value, Object)).ToString("yyyyMMdd HH:mm:ss"))
              xmlW.WriteEndAttribute()

            ElseIf Singular.Misc.IsNullNothing(Entry.Value) Then

            Else
              xmlW.WriteStartAttribute("Value")
              xmlW.WriteValue(Entry.Value)
              xmlW.WriteEndAttribute()
            End If

            xmlW.WriteEndElement()
          Next

          xmlW.WriteEndElement()
          xmlW.WriteEndDocument()
          xmlW.Flush()

          Return sb.ToString

        End Using

      End If

    End Function

  End Class

#End Region

#Region " Excel "

  Public Class Excel

    Public Shared Function ImportExcelToDataTableUsingInfragistics(ByVal FileName As String,
                                                                    MinRows As Integer, MinColumns As Integer, Optional ByVal ColNamesFirstRow As Boolean = False, Optional ByVal SheetName As String = "Sheet1") As DataTable

      Dim ExcelWorkbook As Infragistics.Documents.Excel.Workbook = Infragistics.Documents.Excel.Workbook.Load(FileName)
      Return ImportExcelToDataTableUsingInfragistics(ExcelWorkbook, MinRows, MinColumns, ColNamesFirstRow, SheetName)

    End Function

    Public Shared Function ImportExcelToDataTableUsingInfragistics(ByVal FileStream As Stream,
                                                                    MinRows As Integer, MinColumns As Integer, Optional ByVal ColNamesFirstRow As Boolean = False, Optional ByVal SheetName As String = "Sheet1") As DataTable

      Dim ExcelWorkbook As Infragistics.Documents.Excel.Workbook = Infragistics.Documents.Excel.Workbook.Load(FileStream)
      Return ImportExcelToDataTableUsingInfragistics(ExcelWorkbook, MinRows, MinColumns, ColNamesFirstRow, SheetName)

    End Function

    Public Shared Function ImportExcelToDataTableUsingInfragistics(ByVal FileName As String, Optional ByVal ColNamesFirstRow As Boolean = False, Optional ByVal SheetName As String = "Sheet1") As DataTable

      Dim ExcelWorkbook As Infragistics.Documents.Excel.Workbook = Infragistics.Documents.Excel.Workbook.Load(FileName)
      Return ImportExcelToDataTableUsingInfragistics(ExcelWorkbook, ColNamesFirstRow, SheetName)

    End Function

		Public Shared Function ImportExcelToDataSetUsingInfragistics(Stream As Stream, Optional ByVal ColNamesFirstRow As Boolean = False) As DataSet

			Dim ExcelWorkbook As Infragistics.Documents.Excel.Workbook = Infragistics.Documents.Excel.Workbook.Load(Stream)
			Dim ds As New DataSet()
			For Each ws In ExcelWorkbook.Worksheets
				ds.Tables.Add(ImportExcelSheetToDataTableUsingInfragistics(ws, ColNamesFirstRow))
			Next
			Return ds

		End Function


		Public Shared Function ImportExcelToDataSetUsingInfragistics(ByVal FileName As String, Optional ByVal ColNamesFirstRow As Boolean = False) As DataSet

      Dim ExcelWorkbook As Infragistics.Documents.Excel.Workbook = Infragistics.Documents.Excel.Workbook.Load(FileName)
      Dim ds As New DataSet(IO.Path.GetFileNameWithoutExtension(FileName))
      For Each ws In ExcelWorkbook.Worksheets
        ds.Tables.Add(ImportExcelSheetToDataTableUsingInfragistics(ws, ColNamesFirstRow))
      Next
      Return ds

    End Function

    Public Shared Function ImportExcelToDataTableUsingInfragistics(ByVal FileStream As Stream, Optional ByVal ColNamesFirstRow As Boolean = False, Optional ByVal SheetName As String = "Sheet1") As DataTable

      Dim ExcelWorkbook As Infragistics.Documents.Excel.Workbook = Infragistics.Documents.Excel.Workbook.Load(FileStream)
      Return ImportExcelToDataTableUsingInfragistics(ExcelWorkbook, ColNamesFirstRow, SheetName)

    End Function

    Private Shared Function ImportExcelToDataTableUsingInfragistics(ByVal ExcelWorkbook As Infragistics.Documents.Excel.Workbook,
                                                                    MinRows As Integer, MinColumns As Integer, Optional ByVal ColNamesFirstRow As Boolean = False, Optional ByVal SheetName As String = "Sheet1") As DataTable

      If SheetName = "Sheet1" Then
        SheetName = ExcelWorkbook.Worksheets(0).Name
      End If
      Dim Sheet As Infragistics.Documents.Excel.Worksheet = ExcelWorkbook.Worksheets(SheetName)
      Dim tbl As New DataTable(SheetName)
      'Dim NumCols As Integer = 0
      'Dim NumBlanks As Integer = 0

      '  Dim ColumnsBlank(MinColumns - 1) As Boolean

      '  Dim CellValue As Object = Nothing
      For iRow As Integer = 0 To MinRows
        Dim drw As DataRow = tbl.NewRow()
        For iCol As Integer = 0 To MinColumns
          If iRow = 0 Then
            tbl.Columns.Add(String.Format("Column{0}", iCol), GetType(String))
          End If

          drw(iCol) = Sheet.Rows(iRow).Cells(iCol).Value

        Next
        tbl.Rows.Add(drw)
      Next

      Return tbl

    End Function

    Private Shared Function ImportExcelToDataTableUsingInfragistics(ByVal ExcelWorkbook As Infragistics.Documents.Excel.Workbook, Optional ByVal ColNamesFirstRow As Boolean = False, Optional ByVal SheetName As String = "Sheet1") As DataTable

      If SheetName = "Sheet1" Then
        SheetName = ExcelWorkbook.Worksheets(0).Name
      End If
      Dim Sheet As Infragistics.Documents.Excel.Worksheet = ExcelWorkbook.Worksheets(SheetName)
      Return ImportExcelSheetToDataTableUsingInfragistics(Sheet, ColNamesFirstRow)

    End Function

    Private Shared Function ImportExcelSheetToDataTableUsingInfragistics(Sheet As Infragistics.Documents.Excel.Worksheet, Optional ByVal ColNamesFirstRow As Boolean = False) As DataTable

      Dim tbl As New DataTable(Sheet.Name)
      Dim NumCols As Integer = 0
      Dim NumBlanks As Integer = 0

      ' there are no columns, look for values
      For iCol As Integer = 0 To 10000
        If String.IsNullOrEmpty(CStr(Sheet.Rows(0).Cells(iCol).Value)) Then
          NumBlanks += 1
        End If
        NumCols += 1
        If NumBlanks > 5 AndAlso ColNamesFirstRow Then
          ' must be at the end
          NumCols -= NumBlanks
          Exit For
        End If
      Next

      For iCol = 0 To NumCols - 1
        If ColNamesFirstRow AndAlso Sheet.Rows(0).Cells(iCol).Value IsNot Nothing Then
          ' Change by Stewart Moss
          ' In our import the trailing spaces on the coloumn names was giving problems. 
          ' The client was not consistent with their export.
          tbl.Columns.Add(CStr(Sheet.Rows(0).Cells(iCol).Value.Trim), GetType(String))
        Else
          tbl.Columns.Add("Column" & iCol, GetType(String))
        End If
      Next

      Dim StartRow As Integer = 0
      If ColNamesFirstRow Then
        StartRow = 1
      End If
      Dim BlankRows As New List(Of DataRow)
      For iRow = StartRow To 1000000
        Dim Row = tbl.NewRow
        Dim AllValuesBlank As Boolean = True
        For iCol = 0 To NumCols - 1
          Dim Value As String = ""
          If Not (TypeOf Sheet.Rows(iRow).Cells(iCol).Value Is Infragistics.Documents.Excel.ErrorValue) Then
            Try
              Value = CStr(Sheet.Rows(iRow).Cells(iCol).Value)
            Catch
              Value = ""
            End Try
          End If
          If Not String.IsNullOrEmpty(Value) Then
            AllValuesBlank = False
          End If
          Row(iCol) = Value
        Next

        If Not AllValuesBlank Then
          If BlankRows.Count > 0 Then
            For Each brow In BlankRows
              tbl.Rows.Add(brow)
            Next
            BlankRows.Clear()
          End If

          tbl.Rows.Add(Row)
        Else
          BlankRows.Add(Row)
        End If

        If BlankRows.Count >= 3 Then
          Exit For
        End If
      Next

      Return tbl

    End Function

    Public Shared Function ImportExcelToDataTable(ByVal FileName As String, ByVal SheetIndex As Integer, Optional ByVal ColNamesFirstRow As Boolean = False) As DataTable

      Dim excel As Infragistics.Documents.Excel.Workbook = Workbook.Load(FileName)
      Return ImportExcelToDataTable(FileName, False, excel.Worksheets(0).Name)

    End Function

    Public Shared Function ImportExcelToDataTable(ByVal FileName As String, Optional ByVal ColNamesFirstRow As Boolean = False, Optional ByVal SheetName As String = "Sheet1") As DataTable
      Try

        ' Open the Excel Spreadsheet, and using ODBC
        ' Read it into a DataTable for later processing 

        Dim HDR As String = IIf(ColNamesFirstRow, "Yes", "No")

        Dim oleDbConnection As String = ""

        Select Case System.IO.Path.GetExtension(FileName).ToLower
          Case ".xls"
            oleDbConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & FileName & ";Extended Properties=""Excel 8.0;HDR=" & HDR & ";IMEX=1"""
          Case ".xlsx"
            oleDbConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & FileName & ";Extended Properties=""Excel 12.0;HDR=" & HDR & """"
          Case ".xlsm"
            oleDbConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & FileName & ";Extended Properties=""Excel 12.0 Macro;HDR=" & HDR & """;"

          Case Else
            Throw New Exception("ImportExcelToDataTable can only be called with .xls or .xlsx files.  File: " & FileName)
        End Select

        Dim cnn As New OleDb.OleDbConnection(oleDbConnection)
        Dim cmd As New OleDb.OleDbCommand("SELECT * FROM [" & SheetName & "$]", cnn)
        Dim adp As New OleDb.OleDbDataAdapter(cmd)
        Dim Dt As New DataTable
        cnn.Open()
        Try
          adp.Fill(Dt)
        Finally
          cnn.Close()
        End Try

        Return Dt
      Catch ex As Exception
        Throw New Exception("Import Excel: " & ex.Message)
      End Try

    End Function


    Public Class ExcelFile

      '''''''''''''''''''''''''''''''''''''''''''''''''
      ' LOCAL VARIABLES
      '''''''''''''''''''''''''''''''''''''''''''''''''
#Region " local vars "

      Private _ConnectionString As String = Nothing
      Private _ErrorMessage As String = Nothing
      Private _ExcelWorkSheets As SingularWorkSheets = Nothing
      Private _FileName As String = Nothing
      Private _FileType As String = Nothing
      Private _Path As String = Nothing

#End Region

      '''''''''''''''''''''''''''''''''''''''''''''''''
      ' PROPERTIES
      '''''''''''''''''''''''''''''''''''''''''''''''''
#Region " Properties "

      Private ReadOnly Property ConnectionString() As String
        Get
          _ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" &
          "Data Source=" & Path & FileName & ";" &
          "Extended Properties=Excel 8.0;"

          Return _ConnectionString
        End Get
      End Property

      Public Property ErrorMessage() As String
        Get
          Return _ErrorMessage
        End Get
        Set(ByVal value As String)
          _ErrorMessage = value
        End Set
      End Property

      Public Property ExcelWorkSheets() As SingularWorkSheets
        Get
          Return _ExcelWorkSheets
        End Get
        Set(ByVal value As SingularWorkSheets)
          _ExcelWorkSheets = value
        End Set
      End Property

      Public Property FileName() As String
        Get
          Return _FileName
        End Get
        Set(ByVal value As String)
          _FileName = value
        End Set
      End Property

      Public ReadOnly Property FileType() As String
        Get
          'sets the file type based on the file name
          If _FileType Is Nothing Then
            GetFileType()
          End If

          Return _FileType
        End Get
      End Property

      Public Property Path() As String
        Get
          Return _Path
        End Get
        Set(ByVal value As String)
          _Path = value
        End Set
      End Property

#End Region


      '''''''''''''''''''''''''''''''''''''''''''''''''
      ' CONSTRUCTORS
      '''''''''''''''''''''''''''''''''''''''''''''''''
#Region " Constructors "

      ''' <summary>Creates the object baised on the file you already uploaded. Populates the WorkSheet Property with worksheet names</summary>
      ''' <param name="path">The Pshyical path that could / should be baised on the web config Server.MapPath("excelreading")</param>
      ''' <param name="fileName">this is the name that you saved the file as not necassarly the original file name</param>
      ''' <remarks>Contains error checking incase the wrong file type is uploaded</remarks>
      Public Sub New(ByVal path As String, ByVal fileName As String)
        _Path = path
        _FileName = fileName

        'Check the file type before you get really excited. 
        If UCase(FileType) <> "XLS" Then
          ErrorMessage = "Wrong File Type Jerky"
        Else
          LoadWorkSheetNames()
        End If

      End Sub

      Public Sub New(ByVal fileName As String)
        _FileName = System.IO.Path.GetFileName(fileName)
        _Path = System.IO.Path.GetDirectoryName(fileName) & "\"

        'Check the file type before you get really excited. 
        If UCase(FileType) <> "XLS" Then
          ErrorMessage = "Wrong File Type"
        Else
          LoadWorkSheetNames()
        End If

      End Sub

#End Region

      '''''''''''''''''''''''''''''''''''''''''''''''''
      ' METHODS
      '''''''''''''''''''''''''''''''''''''''''''''''''
#Region " methods "

      ''' <summary>Populates the WorkSheet property with a collection of worksheet names</summary>
      ''' <remarks>more field could be added but are not really relative. The time stamp is accessable
      ''' but it is the time stamp of when the file was coppied</remarks>
      Private Sub LoadWorkSheetNames()
        'counter for loop 
        Dim i As Int16 = Nothing

        Dim dt As DataTable = Nothing

        Try

          Dim objConn As New OleDbConnection(ConnectionString)

          objConn.Open()

          dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)

          'always close Jerky
          objConn.Close()

          Dim tmpExcelWorkSheets As New SingularWorkSheets

          'if there are rows show them
          If dt.Rows.Count > 0 Then

            'loop through all the transactions and add aall the items
            For i = 0 To dt.Rows.Count - 1
              tmpExcelWorkSheets.Add(New SingularWorkSheet(dt.Rows(i)("TABLE_NAME")))
            Next

            _ExcelWorkSheets = tmpExcelWorkSheets

          End If

        Catch ex As Exception

          ErrorMessage = ex.Message

        End Try

      End Sub

#End Region

      '''''''''''''''''''''''''''''''''''''''''''''''''
      ' FUNCTIONS
      '''''''''''''''''''''''''''''''''''''''''''''''''
#Region " functions "

      ''' <summary>Gets data from select workbook</summary>
      ''' <param name="workSheetName">Should ab a name from the excellWorkSheets property</param>
      ''' <returns>dataset</returns>
      ''' <remarks>The worksheet name does not require a $ appended to the name</remarks>
      Public Function GetWorkSheetByName(ByVal workSheetName As String) As DataSet

        Dim ds As New DataSet

        Try

          'Dim myData As New OleDbDataAdapter("SELECT * FROM @WorkSheetName", ConnectionString) ' "[" & workSheetName & "]", ConnectionString)
          Dim myData As New OleDbDataAdapter("SELECT * FROM [" & workSheetName & "]", ConnectionString)

          myData.SelectCommand.Parameters.AddWithValue("@WorkSheetName", workSheetName)
          myData.TableMappings.Add("Table", workSheetName)
          myData.Fill(ds)

        Catch ex As Exception
          _ErrorMessage = ex.Message
        End Try

        Return ds
      End Function

      ''' <summary>Strips the file type out of the file name property</summary>
      ''' <returns>file type as string</returns>
      Public Function GetFileType() As String

        _FileType = Right(FileName, Len(FileName) - InStr(FileName, "."))

        Return _FileType
      End Function

#End Region

    End Class


    '''''''''''''''''''''''''''''''''''''''''''''''''
    ' WORKSHEET CLASS
    '''''''''''''''''''''''''''''''''''''''''''''''''
    ''' <summary>Simple class to hold info about the worksheets in the workbook</summary>
    Public Class SingularWorkSheet

      Private _TableName As String = Nothing

      ''' <summary>"TABLE_NAME" property loaded from OleDbSchemaGuid.Tables method</summary>
      Public Property TableName() As String
        Get
          Return _TableName
        End Get
        Set(ByVal value As String)
          _TableName = value
        End Set
      End Property

      ''' <summary>Constructor used to add new worksheets to a collection</summary>
      ''' <param name="tableName">worksheet name includes the appended $</param>
      Public Sub New(ByVal tableName As String)
        _TableName = tableName
      End Sub

    End Class

    '''''''''''''''''''''''''''''''''''''''''''''''''
    ' WORKSHEETS COLLECTION
    '''''''''''''''''''''''''''''''''''''''''''''''''
    ''' <summary>Simple Collection to store all the worksheets in the work book</summary>
    Public Class SingularWorkSheets
      Inherits Collections.ObjectModel.Collection(Of SingularWorkSheet)

      '''' <summary>method allows you to add items to the collection</summary>
      '''' <param name="WorkSheet"></param>
      '''' <remarks>Man I am good lookin</remarks>
      'Public Sub add(ByVal workSheet As WorkSheet)
      '  List.Add(workSheet)
      'End Sub

      '''' <summary>Allows you to return Items from the collection by index</summary>
      '''' <param name="index">Collection is zero based expects a int as an index in the array</param>
      'Default Public Overridable ReadOnly Property Item(ByVal index As Integer) As WorkSheet
      '  Get
      '    Return CType(Me.List(index), WorkSheet)
      '  End Get
      'End Property
    End Class

  End Class

  Public Class ExcelExporter

    Public Class AfterRowAddEventArgs
      Inherits EventArgs

      Public Property DataRow As DataRow
      Public Property WorksheetRow As WorksheetRow

    End Class

    Public Event AfterRowAdded(sender As Object, e As AfterRowAddEventArgs)

    Private mFormat As Infragistics.Documents.Excel.WorkbookFormat = Infragistics.Documents.Excel.WorkbookFormat.Excel2007

    Public Sub New()

    End Sub

    Public Sub New(Format As Infragistics.Documents.Excel.WorkbookFormat)
      mFormat = Format
    End Sub

    Public Sub MakeExcel2003()
      mFormat = Infragistics.Documents.Excel.WorkbookFormat.Excel97To2003
    End Sub

    Public Function AddFileExtension(DocumentName As String) As String
      If mFormat = WorkbookFormat.Excel97To2003 Then
        Return IO.Path.Combine(DocumentName, ".xls")
      Else
        Return IO.Path.Combine(DocumentName, ".xlsx")
      End If
    End Function

    Private mWorkBook As Infragistics.Documents.Excel.Workbook
    Public ReadOnly Property WorkBook As Infragistics.Documents.Excel.Workbook
      Get
        If mWorkBook Is Nothing Then
          mWorkBook = New Infragistics.Documents.Excel.Workbook(mFormat)
        End If
        Return mWorkBook
      End Get
    End Property

    Public Property ShowTotalsRow As Boolean = True
    Public Property ShowHeaderFilters As Boolean = True
    Public Property FormatAsTable As Boolean = True
    Public Property OverrideWorksheetNames As String()

    Public Sub PopulateData(DataSet As DataSet, Optional ShowGroupings As Boolean = True)

      DataTables.AddExtendedInfo(DataSet)

      Dim ChildCount As Integer = DataSet.Tables.Cast(Of DataTable)().Where(Function(c) c.ParentRelations IsNot Nothing AndAlso c.ParentRelations.Count > 0).Count

      If ShowGroupings AndAlso ChildCount > 0 Then
        PopulateDataWithGroupings(DataSet)
      Else
        For i As Integer = 0 To DataSet.Tables.Count - 1
          If OverrideWorksheetNames IsNot Nothing AndAlso OverrideWorksheetNames.Count > i Then
            PopulateData(DataSet.Tables(i), OverrideWorksheetNames(i))
          Else
            PopulateData(DataSet.Tables(i))
          End If
        Next
      End If

    End Sub

    Public Sub PopulateData(ByVal BusinessListBase As Object,
                            Optional ByVal IgnoreReturnedCSLASingleObjects As Boolean = False,
                            Optional ByVal IgnoreChildObjectsCompletely As Boolean = False,
                            Optional WorksheetName As String = "",
                            Optional SheetStartRow As Integer = 0,
                            Optional AddNonClsaListChildren As Boolean = False,
                            Optional UseDropDownTextValue As Boolean = False)

      Dim ds As DataSet = Singular.CSLALib.GetDatasetFromBusinessListBase(BusinessListBase, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, , , AddNonClsaListChildren, UseDropDownTextValue)
      'Dim ds As DataSet = Singular.CSLALib.ScanBusinessListBaseToDataSet(BusinessListBase, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely)
      If WorksheetName <> "" AndAlso ds.Tables.Count > 0 Then
        PopulateData(ds.Tables(0), Singular.Strings.Left(WorksheetName, 30), , SheetStartRow)
      Else
        PopulateData(ds)
      End If

    End Sub

    Public Sub PopulateData(ByVal BusinessListBase As Object,
                            Worksheet As Worksheet,
                            Optional ByVal IgnoreReturnedCSLASingleObjects As Boolean = False,
                            Optional ByVal IgnoreChildObjectsCompletely As Boolean = False,
                            Optional SheetStartRow As Integer = 0,
                            Optional AddNonClsaListChildren As Boolean = False,
                            Optional UseDropDownTextValue As Boolean = False,
                            Optional UseDisplayNameAsHeader As Boolean = False)

      Dim ds As DataSet = Singular.CSLALib.GetDatasetFromBusinessListBase(BusinessListBase, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, , , AddNonClsaListChildren, UseDropDownTextValue, UseDisplayNameAsHeader)
      'Dim ds As DataSet = Singular.CSLALib.ScanBusinessListBaseToDataSet(BusinessListBase, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely)
      If ds.Tables.Count > 0 Then
        PopulateData(ds.Tables(0), Worksheet, , SheetStartRow)
      End If

    End Sub

    Public Class Lookup
      Public Property LookupColumn As String
      Public Property Options As List(Of String)

      Public Sub New(LookupColumn As String, Options As List(Of String))

        Me.LookupColumn = LookupColumn
        Me.Options = Options

      End Sub

      Friend Property ListDataValidationRule As ListDataValidationRule = Nothing

    End Class

    ''' <summary>
    ''' Creates a worksheet using the table name, or the supplied Name.
    ''' </summary>
    Public Sub PopulateData(DataTable As DataTable, Optional WorksheetName As String = "", Optional Lookups As List(Of Lookup) = Nothing, Optional SheetStartRow As Integer = 0)

      If WorksheetName = "" Then
        WorksheetName = DataTable.TableName
      End If
      Dim ws = WorkBook.Worksheets.Add(WorksheetName)
      PopulateData(DataTable, ws, Lookups)

    End Sub

    Public Class FormulaColumn

      Public Property ColumnName As String
      Public Property Cells As New List(Of WorksheetCell)
      Public Property Formula As String

    End Class

    <AttributeUsage(AttributeTargets.Property)>
    Public Class ExcelFormatAttribute
      Inherits Attribute

      Public Property FontColor As Color = Color.Empty

      Public Property BackgroundColor As Color = Color.Empty

      Public Property FontColorString As String
        Get
          Return Me.FontColor.ToString()
        End Get
        Set(value As String)
          Me.FontColor = GetColorFromString(value)
        End Set
      End Property

      Public Property BackgroundColorString As String
        Get
          Return Me.BackgroundColor.ToString()
        End Get
        Set(value As String)
          Me.BackgroundColor = GetColorFromString(value)
        End Set
      End Property

      Public Sub New()

      End Sub

      'Public Sub New(Optional FontColorString As String = "", Optional BackgroundColorString As String = "")

      '  If FontColorString <> "" Then
      '    Me.FontColor = GetColorFromString(FontColorString)
      '  End If

      '  If BackgroundColor <> "" Then
      '    Me.BackgroundColor = GetColorFromString(BackgroundColor)
      '  End If

      'End Sub

      Private Function GetColorFromString(ColorString As String) As Color

        If ColorString.Contains(",") Then
          Dim ColorParts As String() = ColorString.Split(",")
          If ColorParts.Count = 4 Then
            Return Color.FromArgb(ColorParts(0), ColorParts(1), ColorParts(2), ColorParts(3))
          ElseIf ColorParts.Count = 3 Then
            Return Color.FromArgb(ColorParts(0), ColorParts(1), ColorParts(2))
          Else
            Throw New Exception("Color String must be a name or valid RGB value")
          End If
        Else
          Try
            Return Color.FromName(ColorString)
          Catch ex As Exception
            Throw New Exception("Color String must be a name or valid RGB value")
          End Try
        End If

      End Function

    End Class

    ''' <summary>
    ''' Populate a worksheet using a datatable.
    ''' </summary>
    Public Sub PopulateWorksheetFromDataTable(DataTable As DataTable, Worksheet As Worksheet)
      PopulateData(DataTable, Worksheet)
    End Sub

    Public Sub PopulateData(DataTable As DataTable, Worksheet As Worksheet, Optional Lookups As List(Of Lookup) = Nothing, Optional SheetStartRow As Integer = 0)

      Dim RowIndex As Integer
      Dim ColIndex As Integer = 0
      With Worksheet
        .DefaultRowHeight = 300
        .DefaultColumnWidth = 256 * 20

        Dim ColumnNameIndexes As New Hashtable

        Dim formulaColumns As New List(Of FormulaColumn)

        'Add the data
        For Each Col As DataColumn In DataTable.Columns
          If DataTables.AutoGenerateColumn(Col) Then
            ColumnNameIndexes.Add(Col.ColumnName, ColIndex)
            'Visible
            'Header
            If Col.Caption = "" Then
              .Rows(0 + SheetStartRow).Cells(ColIndex).Value = Singular.Strings.Readable(Col.ColumnName)
            Else
              .Rows(0 + SheetStartRow).Cells(ColIndex).Value = Col.Caption
            End If
            .Columns(ColIndex).CellFormat.VerticalAlignment = Infragistics.Documents.Excel.VerticalCellAlignment.Center
            'Rows
            For RowIndex = 0 To DataTable.Rows.Count - 1

              Dim fcf = DataTables.GetForumulaColumnFormula(Col)
              If fcf IsNot Nothing AndAlso fcf.SupportsExcel Then
                ' replace all column names with the cell references
                Dim fc = formulaColumns.FirstOrDefault(Function(fcx) fcx.ColumnName = Col.ColumnName)
                If fc Is Nothing Then
                  fc = New FormulaColumn With {
                                      .ColumnName = Col.ColumnName,
                                      .Formula = fcf.Formula
                                    }
                  formulaColumns.Add(fc)
                End If
                fc.Cells.Add(Worksheet.Rows(RowIndex + 1 + SheetStartRow).Cells(ColIndex))
              ElseIf TypeOf DataTable.Rows(RowIndex)(Col) Is Csla.SmartDate Then
                .Rows(RowIndex + 1 + SheetStartRow).Cells(ColIndex).Value = CType(DataTable.Rows(RowIndex)(Col), SmartDate).Date
              Else
                Try
                  .Rows(RowIndex + 1 + SheetStartRow).Cells(ColIndex).Value = DataTable.Rows(RowIndex)(Col)
                Catch ex As Exception
                  ' try again, but just convert to string
                  Dim value = DataTable.Rows(RowIndex)(Col)

                  If value.GetType.IsEnum Then
                    .Rows(RowIndex + 1 + SheetStartRow).Cells(ColIndex).Value = Singular.CommonData.Enums.Description(value)
                  Else
                    .Rows(RowIndex + 1 + SheetStartRow).Cells(ColIndex).Value = value.ToString()
                  End If

                End Try

              End If
              .Rows(RowIndex + 1 + SheetStartRow).CellFormat.Font.Height = 180
              .Rows(RowIndex + 1 + SheetStartRow).Height = 301
              RaiseEvent AfterRowAdded(Me, New AfterRowAddEventArgs With {.DataRow = DataTable.Rows(RowIndex), .WorksheetRow = Worksheet.Rows(RowIndex + 1)})
            Next

            If Lookups IsNot Nothing Then
              Dim lookup = Lookups.FirstOrDefault(Function(l) l.LookupColumn = Col.ColumnName)
              If lookup IsNot Nothing Then
                ' add as lookup column
                Dim t As New Infragistics.Documents.Excel.WorksheetReferenceCollection(Worksheet)
                Dim EndRow = Math.Max(1, RowIndex)
                t.Add(New Infragistics.Documents.Excel.WorksheetRegion(Worksheet, 0, ColIndex, EndRow, ColIndex))
                Worksheet.DataValidationRules.Add(GetDataValidationRule(lookup.Options), t)
              End If
            End If

            ColIndex += 1
          End If
        Next


        For Each fc In formulaColumns
          For Each cell In fc.Cells
            Dim formula = fc.Formula
            For Each c As DataColumn In DataTable.Columns.Cast(Of DataColumn).OrderByDescending(Function(cx) cx.ColumnName.Length)
              ' go in descending order because dont have brackets, safe to replace
              ' just in case there is the same name within quotes, first replace that, then replace back again
              formula = formula.Replace("""" & c.ColumnName & """", "<FULLQUOTEDCOLUMNNAME>")
              formula = formula.Replace("""" & c.ColumnName, "<STARTQUOTEDCOLUMNNAME>")
              formula = formula.Replace(c.ColumnName & """", "<ENDQUOTEDCOLUMNNAME>")
              ' now replace the columns for the cell reference
              formula = formula.Replace(c.ColumnName, .Rows(cell.RowIndex).Cells(CInt(ColumnNameIndexes(c.ColumnName))).ToString(CellReferenceMode.A1, False, True, True))
              ' replace back again
              formula = formula.Replace("<FULLQUOTEDCOLUMNNAME>", """" & c.ColumnName & """")
              formula = formula.Replace("<STARTQUOTEDCOLUMNNAME>", """" & c.ColumnName)
              formula = formula.Replace("<ENDQUOTEDCOLUMNNAME>", c.ColumnName & """")
            Next
            cell.ApplyFormula("=" & formula)
          Next
        Next



        'Get the region of the table
        If RowIndex = 0 AndAlso ColIndex = 0 Then
          'Do Nothing
        Else
          Dim dt As Infragistics.Documents.Excel.WorksheetTable = Nothing
          If FormatAsTable Then
            Dim r As New Infragistics.Documents.Excel.WorksheetRegion(Worksheet, 0 + SheetStartRow, 0, RowIndex + SheetStartRow, ColIndex - 1)
            dt = r.FormatAsTable(True)
            dt.IsTotalsRowVisible = ShowTotalsRow
            dt.IsFilterUIVisible = ShowHeaderFilters
          End If

          'Format the columns
          ColIndex = 0
          For Each Col As DataColumn In DataTable.Columns
            If DataTables.AutoGenerateColumn(Col) Then

              Dim smi As New Singular.Reflection.SMemberInfo(Col.DataType)

              '  .Columns(ColIndex).CellFormat

              Select Case smi.DataTypeMain
                Case Reflection.SMemberInfo.MainType.Number
                  If Not Col.ColumnName.EndsWith("ID") Then

                    'Number format.
                    .Columns(ColIndex).CellFormat.Alignment = Infragistics.Documents.Excel.HorizontalCellAlignment.Right

                    .Columns(ColIndex).Width = 256 * 12
                    If Col.DataType Is GetType(Integer) Then
                      .Columns(ColIndex).CellFormat.FormatString = "#,###,##0;(#,###,##0)"
                    Else
                      .Columns(ColIndex).CellFormat.FormatString = "#,###,##0.00;(#,###,##0.00)"
                    End If
                    If Col.ColumnName.EndsWith("Percent") Then
                      .Columns(ColIndex).CellFormat.FormatString = "#,###,##0.00%;(#,###,##0.00%)"
                    End If

                    Dim ColRef = GetColumnAplhaIndex(ColIndex)
                    If ShowTotalsRow AndAlso dt IsNot Nothing Then
                      Dim listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator
                      Try
                        If Not DataTables.ExcludeFromTotal(Col) Then
                          Dim formula = "=SUBTOTAL(109" & listSeparator & .Rows(1 + SheetStartRow).Cells(ColIndex).ToString & ":" & .Rows(dt.TotalsRowRegion.LastRow - 1).Cells(ColIndex).ToString & ")"
                          dt.Columns(ColIndex).TotalCell.ApplyFormula(formula)
                        Else
                          dt.Columns(ColIndex).TotalCell.Value = Nothing
                        End If
                      Catch ex As Exception
                        Try
                          listSeparator = If(listSeparator = ";", ",", ";")
                          If Not DataTables.ExcludeFromTotal(Col) Then
                            Dim formula = "=SUBTOTAL(109" & listSeparator & .Rows(1 + SheetStartRow).Cells(ColIndex).ToString & ":" & .Rows(dt.TotalsRowRegion.LastRow - 1).Cells(ColIndex).ToString & ")"
                            dt.Columns(ColIndex).TotalCell.ApplyFormula(formula)
                          Else
                            dt.Columns(ColIndex).TotalCell.Value = Nothing
                          End If
                        Catch ex2 As Exception
                          Dim ax = ex
                        End Try
                      End Try
                    End If

                  End If

                Case Reflection.SMemberInfo.MainType.Date
                  'Date Format.

                  .Columns(ColIndex).CellFormat.Alignment = Infragistics.Documents.Excel.HorizontalCellAlignment.Center

                  If Col.ColumnName.Replace(" ", "").ToLower.Contains("createddate") Then
                    .Columns(ColIndex).CellFormat.FormatString = "dd MMM yyyy hh:mm"
                    .Columns(ColIndex).Width = 256 * 18
                  Else
                    .Columns(ColIndex).CellFormat.FormatString = "dd MMM yyyy"
                    .Columns(ColIndex).Width = 256 * 15
                  End If

              End Select

              'Override if the column has extended info.
              .Columns(ColIndex).CellFormat.FormatString = DataTables.GetExtProperty(Col, DataTables.ExtendedProperties.FormatString, .Columns(ColIndex).CellFormat.FormatString)

              If dt IsNot Nothing Then
                'Set datatable formats to be the same as the column formats.
                dt.Columns(ColIndex).AreaFormats(0).FormatString = .Columns(ColIndex).CellFormat.FormatString
                dt.Columns(ColIndex).HeaderCell.CellFormat.Alignment = .Columns(ColIndex).CellFormat.Alignment
              End If

              If Col.ExtendedProperties.ContainsKey(DataTables.ExtendedProperties.ExcelFormat.ToString()) Then
                Dim excelFormat As ExcelFormatAttribute = Col.ExtendedProperties(DataTables.ExtendedProperties.ExcelFormat.ToString())

                ApplyFormatToColumn(.Columns(ColIndex), excelFormat)
              End If

              ColIndex += 1
            End If
          Next

        End If


        'Format the header.
        .Rows(0 + SheetStartRow).CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(System.Drawing.Color.Black)

        .Rows(0 + SheetStartRow).CellFormat.Font.ColorInfo = New Infragistics.Documents.Excel.WorkbookColorInfo(Drawing.Color.White)
        .Rows(0 + SheetStartRow).CellFormat.VerticalAlignment = Infragistics.Documents.Excel.VerticalCellAlignment.Center
        .Rows(0 + SheetStartRow).CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True
        .Rows(0 + SheetStartRow).Height = 600
        .Rows(0 + SheetStartRow).CellFormat.Font.Height = 180

      End With

    End Sub

    Private Sub ApplyFormatToColumn(Column As WorksheetColumn, excelFormat As ExcelFormatAttribute)

      If excelFormat.FontColor <> Color.Empty Then
        Column.CellFormat.Font.ColorInfo = New WorkbookColorInfo(excelFormat.FontColor)
      End If
      If excelFormat.BackgroundColor <> Color.Empty Then
        Column.CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(excelFormat.BackgroundColor)
      End If

    End Sub

#Region "Populate with Groupings"

    Private mCurrentWorksheet As Worksheet
    Private mCurrentTableIndex As Integer = 0

    Public RowColourList() As TableColorInfo = Nothing

    Public Shared DefaultRowColourList() As TableColorInfo =
      {New TableColorInfo(Color.FromArgb(221, 242, 247), Color.FromArgb(220, 235, 246), Color.FromArgb(182, 228, 238)),
       New TableColorInfo(Color.FromArgb(220, 248, 221), Color.FromArgb(228, 250, 229), Color.FromArgb(180, 240, 183)),
       New TableColorInfo(Color.FromArgb(248, 249, 217), Color.FromArgb(249, 251, 227), Color.FromArgb(237, 239, 171)),
       New TableColorInfo(Color.FromArgb(247, 235, 215), Color.FromArgb(250, 242, 224), Color.FromArgb(240, 217, 180)),
       New TableColorInfo(Color.FromArgb(247, 219, 215), Color.FromArgb(250, 228, 224), Color.FromArgb(240, 186, 180))}

    Private Sub PopulateDataWithGroupings(Dataset As DataSet)

      Dim count As Integer = 0

      For Each dt As DataTable In Dataset.Tables.Cast(Of DataTable)().Where(Function(c) c.ParentRelations Is Nothing OrElse c.ParentRelations.Count = 0)

        If OverrideWorksheetNames IsNot Nothing AndAlso OverrideWorksheetNames.Count > count Then
          mCurrentWorksheet = WorkBook.Worksheets.Add(OverrideWorksheetNames(count))
        Else
          mCurrentWorksheet = WorkBook.Worksheets.Add(dt.TableName)
        End If

        'Setup defaults
        For i As Integer = 0 To 100
          mCurrentWorksheet.Columns(i).CellFormat.Font.Height = 180
        Next
        mCurrentWorksheet.DefaultRowHeight = 320
        mCurrentWorksheet.DefaultColumnWidth = 256 * 20

        mCurrentTableIndex = 0
        AddTable(0, 0, dt)

        count += 1
      Next

      If WorkBook.Worksheets(0).Name = "Information" Then
        WorkBook.Worksheets(0).MoveToIndex(WorkBook.Worksheets.Count - 1)
        WorkBook.WindowOptions.SelectedWorksheet = WorkBook.Worksheets(0)
      End If

    End Sub

    Private Sub AddTable(ByRef RowIndex As Integer, ColIndex As Integer, table As DataTable)

      AddTable(RowIndex, ColIndex, table.Select(Nothing, table.DefaultView.Sort), table.Columns.Cast(Of DataColumn).ToArray())

    End Sub

    Private Sub AddTable(ByRef RowIndex As Integer, StartColIndex As Integer, rows As ICollection, Columns As DataColumn())

      Dim TCI As TableColorInfo = Nothing
      If rows.Count > 0 Then
        TCI = CType(rows(0), DataRow).Table.ExtendedProperties("ColorInfo")
      End If

      If TCI Is Nothing Then
        TCI = GetColourInfo(mCurrentTableIndex)
      End If

      AddHeaders(RowIndex, StartColIndex, Columns)

      Dim InitialRowIndex As Integer = RowIndex
      mCurrentTableIndex += 1

      Dim AltRow As Boolean = False
      For Each row In rows
        AddRow(RowIndex, StartColIndex, row, AltRow, TCI)
        AltRow = Not AltRow
      Next

      If ShowTotalsRow Then
        AddTotals(InitialRowIndex, RowIndex, StartColIndex, rows, Columns)
      End If

    End Sub

    Public Class TableColorInfo

      Public Sub New(RowColor As Color, AltRowColor As Color, CellBorderColor As Color)
        Me.RowColor = RowColor
        Me.AltRowColor = AltRowColor
        Me.CellBorderColor = CellBorderColor
      End Sub

      Public RowColor As Color
      Public AltRowColor As Color
      Public CellBorderColor As Color
    End Class

    Private Sub AddHeaders(ByRef RowIndex As Integer, ColIndex As Integer, Columns As DataColumn())
      Dim VisibleColIndex As Integer = ColIndex

      With mCurrentWorksheet

        For Each col As DataColumn In Columns

          If DataTables.AutoGenerateColumn(col) Then

            Dim Cell = .Rows(RowIndex).Cells(VisibleColIndex)
            Cell.Value = If(col.Caption = "", col.ColumnName, col.Caption)

            FormatCell(Cell, col, True)
            Cell.CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(System.Drawing.Color.Black)

            VisibleColIndex += 1
          End If

        Next

        'Format the Header Row
        .Rows(RowIndex).CellFormat.Font.ColorInfo = New Infragistics.Documents.Excel.WorkbookColorInfo(Drawing.Color.White)
        .Rows(RowIndex).CellFormat.VerticalAlignment = Infragistics.Documents.Excel.VerticalCellAlignment.Center
        .Rows(RowIndex).CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True
        .Rows(RowIndex).Height = 350
        .Rows(RowIndex).OutlineLevel = ColIndex

        If ColIndex > 0 Then
          .Rows(RowIndex).Hidden = True
        End If
        RowIndex += 1

      End With

    End Sub


    Private Sub AddRow(ByRef RowIndex As Integer, StartColIndex As Integer, Row As DataRow, IsAlternateRow As Boolean, TCI As TableColorInfo)
      Dim VisibleColIndex As Integer = StartColIndex

      With mCurrentWorksheet

        Dim wsr = .Rows(RowIndex)

        For Each col As DataColumn In Row.Table.Columns
          If DataTables.AutoGenerateColumn(col) Then

            Dim Cell = wsr.Cells(VisibleColIndex)

            If Row(col) IsNot DBNull.Value Then
              If TypeOf Row(col) Is Csla.SmartDate Then
                Cell.Value = CType(Row(col), SmartDate).Date
              Else
                Cell.Value = Row(col)
              End If
            Else
              Cell.Value = ""
            End If

            wsr.OutlineLevel = StartColIndex
            wsr.Hidden = StartColIndex > 0
            wsr.CellFormat.VerticalAlignment = VerticalCellAlignment.Center

            FormatCell(Cell, col, If(IsAlternateRow, TCI.AltRowColor, TCI.RowColor), TCI.CellBorderColor)

            VisibleColIndex += 1

          End If
        Next
        RowIndex += 1

        Dim OldTableIndex As Integer = mCurrentTableIndex
        For Each cr As DataRelation In Row.Table.ChildRelations

          If cr.ChildTable.ExtendedProperties("NoExport") Is Nothing Then
            Dim rows() As DataRow = Row.GetChildRows(cr)
            If rows.Length > 0 Then
              'Child band, add a blank row before the child header.
              .Rows(RowIndex).OutlineLevel = StartColIndex + 1
              .Rows(RowIndex).Hidden = True
              .Rows(RowIndex).Cells(0).Value = " " 'add space so excel filtering still works.
              RowIndex += 1
              AddTable(RowIndex, StartColIndex + 1, Row.GetChildRows(cr), cr.ChildColumns(0).Table.Columns.Cast(Of DataColumn).ToArray())
              'And after
              .Rows(RowIndex).OutlineLevel = StartColIndex + 1
              .Rows(RowIndex).Hidden = True
              .Rows(RowIndex).Cells(0).Value = " "
              RowIndex += 1

            End If
          End If

        Next
        mCurrentTableIndex = OldTableIndex
      End With

    End Sub

    Private Sub AddTotals(InitialRowIndex As Integer, ByRef RowIndex As Integer, ColIndex As Integer, Rows As ICollection, Columns As DataColumn())
      Dim VisibleColIndex As Integer = ColIndex

      With mCurrentWorksheet

        For Each col As DataColumn In Columns
          If DataTables.AutoGenerateColumn(col) Then

            If Not col.ColumnName.EndsWith("ID") Then

              'Totals have to be hard coded values, otherwise the excel totals range will include records in the child tables.
              If (col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(Decimal)) AndAlso Not DataTables.ExcludeFromTotal(col) Then

                Dim Total As Decimal = 0
                For Each row As DataRow In Rows
                  Total += Singular.Misc.IsNull(row(col), 0D)
                Next
                .Rows(RowIndex).Cells(VisibleColIndex).Value = Total

                If col.DataType Is GetType(Integer) Then
                  .Rows(RowIndex).Cells(VisibleColIndex).CellFormat.FormatString = "#,###,##0;(#,###,##0)"
                Else
                  .Rows(RowIndex).Cells(VisibleColIndex).CellFormat.FormatString = "#,###,##0.00;(#,###,##0.00)"
                End If
              End If

            End If
            .Rows(RowIndex).Cells(VisibleColIndex).CellFormat.TopBorderColorInfo = New WorkbookColorInfo(Color.Black)
            .Rows(RowIndex).Cells(VisibleColIndex).CellFormat.Fill = CellFill.CreateSolidFill(Color.WhiteSmoke)

            VisibleColIndex += 1
          End If
        Next

        .Rows(RowIndex).OutlineLevel = ColIndex
        If ColIndex > 0 Then
          .Rows(RowIndex).Hidden = True
        End If
        RowIndex += 1

      End With


    End Sub

    Private Sub FormatCell(ByRef Cell As Infragistics.Documents.Excel.WorksheetCell, Column As DataColumn, IsHeader As Boolean)
      If Column.DataType Is GetType(Integer) Then
        If IsHeader Then
          Cell.CellFormat.Alignment = HorizontalCellAlignment.Right 'Excel will auto align the number cells right
        Else
          Cell.CellFormat.FormatString = "#,###,##0;(#,###,##0)"
        End If

      End If
      If Column.DataType Is GetType(Decimal) Then
        If IsHeader Then
          Cell.CellFormat.Alignment = HorizontalCellAlignment.Right 'Excel will auto align the number cells right
        Else
          Cell.CellFormat.FormatString = "#,###,##0.00;(#,###,##0.00)"
        End If

      End If
      If Column.DataType Is GetType(Date) OrElse Column.DataType Is GetType(DateTime) OrElse Column.DataType Is GetType(SmartDate) Then
        Cell.CellFormat.Alignment = HorizontalCellAlignment.Center
        If Not IsHeader Then
          Cell.CellFormat.FormatString = "dd MMM yyyy"
        End If

      End If
    End Sub

    Private Sub FormatCell(ByRef Cell As Infragistics.Documents.Excel.WorksheetCell, pi As PropertyInfo, IsHeader As Boolean)
      If pi.PropertyType Is GetType(Integer) Then
        If IsHeader Then
          Cell.CellFormat.Alignment = HorizontalCellAlignment.Right 'Excel will auto align the number cells right
        Else
          Cell.CellFormat.FormatString = "#,###,##0;(#,###,##0)"
        End If

      End If
      If pi.PropertyType Is GetType(Decimal) Then
        If IsHeader Then
          Cell.CellFormat.Alignment = HorizontalCellAlignment.Right 'Excel will auto align the number cells right
        Else
          Cell.CellFormat.FormatString = "#,###,##0.00;(#,###,##0.00)"
        End If

      End If
      If pi.PropertyType Is GetType(Date) OrElse pi.PropertyType Is GetType(DateTime) OrElse pi.PropertyType Is GetType(SmartDate) Then
        Cell.CellFormat.Alignment = HorizontalCellAlignment.Center
        If Not IsHeader Then
          Cell.CellFormat.FormatString = "dd MMM yyyy"
        End If

      End If
    End Sub

    Private Sub FormatCell(ByRef Cell As Infragistics.Documents.Excel.WorksheetCell, Column As DataColumn, BackColour As Color, BorderColor As Color)
      FormatCell(Cell, Column, False)
      Cell.CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(BackColour)
      Cell.CellFormat.BottomBorderColorInfo = New WorkbookColorInfo(BorderColor)
      Cell.CellFormat.RightBorderColorInfo = New WorkbookColorInfo(BorderColor)
      Cell.CellFormat.LeftBorderColorInfo = New WorkbookColorInfo(BorderColor)
    End Sub

    Private Function GetColourInfo(Level As Integer) As TableColorInfo

      Dim UseList As TableColorInfo() = If(RowColourList IsNot Nothing, RowColourList, DefaultRowColourList)

      If UseList.Length > Level Then
        Return UseList(Level)
      Else
        Return UseList(0)
      End If

    End Function

#End Region

    ''' <summary>
    ''' Returns the excel alphabet character for the column. E.g. column index 0 is column A
    ''' </summary>
    Public Shared Function GetColumnAplhaIndex(Index As Integer) As String
      If Index > 625 Then
        Throw New Exception("only up to 625 columns supported.")
      End If

      If Index > 25 Then
        Return ChrW(Math.Floor(Index / 25) + 64) & ChrW(Index Mod 25 + 64)
      Else
        Return ChrW(Index + 65)
      End If

    End Function

    Public Function GetStream() As IO.MemoryStream

      Dim ms As New IO.MemoryStream
      If WorkBook.Worksheets.Count = 0 Then
        WorkBook.Worksheets.Add("No Data").Rows(0).Cells(0).Value = "No Data"
      End If
      WorkBook.Save(ms)
      Return ms

    End Function

#Region " Populate With Lookups "

    Public Sub PopulateWithLookups(List As Object)

      Dim listType = List.GetType()
      Dim childType = CType(listType.BaseType, Object).GenericTypeArguments(1)

      Dim ws As Infragistics.Documents.Excel.Worksheet = WorkBook.Worksheets.Add(listType.Name)

      Dim ColIndex = 0
      Dim RowIndex = 0

      With ws.Rows(RowIndex)
        'Format the Header Row
        .CellFormat.Font.ColorInfo = New Infragistics.Documents.Excel.WorkbookColorInfo(Drawing.Color.White)
        .CellFormat.VerticalAlignment = Infragistics.Documents.Excel.VerticalCellAlignment.Center
        .CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True
        .Height = 350
        .OutlineLevel = ColIndex
      End With

      For Each pi As System.Reflection.PropertyInfo In childType.GetProperties(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
        If Singular.Reflection.AutoGenerateField(pi) AndAlso Not Singular.Misc.In(pi.Name, "Expanded", "IsExpanded") AndAlso Not Singular.Reflection.TypeImplementsInterface(pi.PropertyType, GetType(ISingularBase)) AndAlso
          Not Singular.Reflection.TypeImplementsInterface(pi.PropertyType, GetType(ISingularListBase)) Then

          ws.Rows(RowIndex).Cells(ColIndex).Value = pi.Name
          FormatCell(ws.Rows(RowIndex).Cells(ColIndex), pi, True)
          ws.Rows(RowIndex).Cells(ColIndex).CellFormat.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(System.Drawing.Color.Black)
          RowIndex += 1

          Dim ddw As Singular.DataAnnotations.DropDownWeb = pi.GetCustomAttributes(GetType(Singular.DataAnnotations.DropDownWeb), False).FirstOrDefault()
          Dim dvr As ListDataValidationRule = Nothing
          If ddw IsNot Nothing Then
            'Get Lookup
            dvr = GetDataValidationRule(ddw)
          End If


          If ddw Is Nothing OrElse dvr Is Nothing Then
            'Add Value
            For Each item In List
              ws.Rows(RowIndex).Cells(ColIndex).Value = pi.GetValue(item, Nothing)
              RowIndex += 1
            Next
          Else
            Dim itemList = Singular.CommonData.GetList(ddw.ListType)
            Dim dispPI = Singular.Reflection.GetProperty(CType(ddw.ListType.BaseType, Object).GenericTypeArguments(1), ddw.DisplayMember)
            For Each item In List
              Dim value = pi.GetValue(item, Nothing)
              Dim disp = If(itemList IsNot Nothing, itemList.GetItem(value), Nothing)

              If disp IsNot Nothing Then
                ws.Rows(RowIndex).Cells(ColIndex).Value = dispPI.GetValue(disp, Nothing)
              End If
              'ws.DataValidationRules.Add(dvr, ws.Rows(RowIndex).Cells(ColIndex))
              RowIndex += 1
            Next

            Dim t As New Infragistics.Documents.Excel.WorksheetReferenceCollection(ws)
            t.Add(New Infragistics.Documents.Excel.WorksheetRegion(ws, 1, ColIndex, RowIndex, ColIndex))
            ws.DataValidationRules.Add(dvr, t)
          End If

          RowIndex = 0
          ColIndex += 1

        End If
      Next

    End Sub

    Private Function GetDataValidationRule(dda As Singular.DataAnnotations.DropDownWeb) As ListDataValidationRule

      Dim list = Singular.CommonData.GetList(dda.ListType)
      Dim pi = Singular.Reflection.GetProperty(CType(dda.ListType.BaseType, Object).GenericTypeArguments(1), dda.DisplayMember)

      If list Is Nothing OrElse list.Count = 0 OrElse pi Is Nothing Then
        Return Nothing
      End If

      Dim items As List(Of String) = New List(Of String)
      For Each li In list
        items.Add(pi.GetValue(li, Nothing))
      Next

      Return GetDataValidationRule(items)

    End Function

    Private Function GetDataValidationRule(items As List(Of String)) As ListDataValidationRule

      Dim ld As ListDataValidationRule = New ListDataValidationRule()
      'DataRule = ld
      ld.AllowNull = True
      ld.ShowDropdown = True

      ld.SetValues(items.ToArray())

      ld.ErrorMessageDescription = "Invalid Value Entered"
      ld.ErrorMessageTitle = "Validation Error"
      ld.ErrorStyle = DataValidationErrorStyle.Stop

      ld.InputMessageDescription = "Type or select a value from the list."
      ld.InputMessageTitle = "Value Selection"

      ld.ShowErrorMessageForInvalidValue = True
      ld.ShowInputMessage = True

      Return ld

    End Function

#End Region

  End Class
#End Region

#Region " CSV "

  Public Class CSV

#If Silverlight = False Then

    Public Class CSVLineReader

      Private mLinePos As Integer = 0
      Private mLinetext As String = ""
      Private mDelimeter As String

      Public Sub New(ByVal Line As String, Optional Delimeter As String = ",")
        mLinetext = Line
        mDelimeter = Delimeter
      End Sub

      Public Function GetNextField() As String
        Dim ReturnString As String = ""
        Dim InString As Boolean = False

        For Me.mLinePos = mLinePos To mLinetext.Length - 1

          If mLinetext(mLinePos) = mDelimeter And Not InString Then
            mLinePos += 1
            Return ReturnString
          Else

            If mLinetext(mLinePos) = """" Then
              If InString Then

                If mLinePos <> mLinetext.Length - 1 AndAlso mLinetext(mLinePos + 1) = """" Then
                  mLinePos += 1
                  ReturnString &= """"
                Else
                  InString = False
                End If

              Else
                InString = True
              End If

            Else
              ReturnString &= mLinetext(mLinePos)
            End If

          End If

        Next

        Return ReturnString
        '1,,2
        '1,"""hello"""

      End Function

      Public Function GetNextFieldAsInt() As Integer
        Dim str As String = GetNextField()
        If str = "" Then
          Return 0
        Else
          Return str
        End If
      End Function

      Public Function GetNextFieldAsDate(Optional NullDate As Date? = Nothing) As Date?
        Dim str As String = GetNextField()
        Dim Found As Date
        If Date.TryParse(str, Found) Then
          Return Found
        Else
          Return NullDate
        End If
      End Function

      Public Function GetNextFieldAsDateExact(DateFormat As String, Optional NullDate As Date? = Nothing) As Date?
        Dim str As String = GetNextField()
        Dim Found As Date
        If Date.TryParseExact(str, DateFormat, System.Globalization.DateTimeFormatInfo.InvariantInfo, Globalization.DateTimeStyles.None, Found) Then
          Return Found
        Else
          Return NullDate
        End If
      End Function

      Public Function GetNextFieldAsBoolean() As Boolean
        Dim str As String = GetNextField()
        Return str.ToLower = "y" OrElse str = "1" OrElse str.ToLower = "true"
      End Function

    End Class

    Public Class CSVLineWriter

      Private mWriter As IO.StreamWriter
      Private mCommaAtEnd As Boolean
      Private mStartOfLine As Boolean
      Private mStartOfFile As Boolean = True
      Private mDelimeter As String

      ''' <summary>
      ''' Specifies whether the last record on a line should have a comma after it.
      ''' </summary>
      Public Property CommaAtEnd As Boolean
        Get
          Return mCommaAtEnd
        End Get
        Set(ByVal value As Boolean)
          mCommaAtEnd = value
        End Set
      End Property

      Public Sub New(ByVal Writer As System.IO.StreamWriter, Optional Delimeter As String = ",")
        mWriter = Writer
        mDelimeter = Delimeter
        mStartOfLine = True
      End Sub

      ''' <summary>
      ''' Adds a normal field without inverted commas
      ''' </summary>
      Public Sub AddField(ByVal field As Object)
        If Not mStartOfLine Then
          mWriter.Write(mDelimeter)
        End If
        If mStartOfLine AndAlso Not mStartOfFile Then
          mWriter.WriteLine()
        End If
        mWriter.Write(GetFieldValue(field).Replace(vbCrLf, " "))
        mStartOfLine = False
        mStartOfFile = False
      End Sub

      Private Function GetFieldValue(ByVal field As Object) As String
        If field Is Nothing OrElse field Is DBNull.Value Then
          Return ""
        ElseIf field.GetType Is GetType(Date) Then
          Return CDate(field).ToString("d").Replace(vbCr, "").Replace(vbLf, "")
        Else
          Return field.ToString.Replace(vbCr, "").Replace(vbLf, "")
        End If
      End Function

      ''' <summary>
      ''' Adds a text field with inverted commas. Inverted comma's in the original string are replaced with double inverted comma's.
      ''' </summary>
      Public Sub AddTextField(ByVal field As Object)
        If GetFieldValue(field) = "" Then
          AddField("")
        Else
          AddField("""" & GetFieldValue(field).Replace("""", """""") & """")
        End If
      End Sub

      Public Sub AddBooleanField(ByVal field As Boolean)
        If field Then
          AddTextField("Yes")
        Else
          AddTextField("No")
        End If
      End Sub

      ''' <summary>
      ''' Closes off the line and adds a new line.
      ''' </summary>
      Public Sub NextRecord()
        If mCommaAtEnd Then
          mWriter.Write(mDelimeter)
        End If
        mStartOfLine = True
      End Sub

    End Class

    ''' <summary>
    ''' Converts a datatable into CSV. To convert to string, use System.Text.Encoding.ASCII.GetString(CSV.FromDataTable(.....)).
    ''' </summary>
    Public Shared Function FromDataTable(DataTable As DataTable, IncludeHeadings As Boolean) As Byte()

      Using ms As New System.IO.MemoryStream
        Using sw As New System.IO.StreamWriter(ms)
          Dim writer As New CSVLineWriter(sw)

          If IncludeHeadings Then
            For Each col As DataColumn In DataTable.Columns
              If DataTables.AutoGenerateColumn(col) Then
                writer.AddField(col.ColumnName)
              End If
            Next
            writer.NextRecord()
          End If

          For i As Integer = 0 To DataTable.Rows.Count - 1
            Dim row = DataTable.Rows(i)
            For Each col As DataColumn In DataTable.Columns
              If DataTables.AutoGenerateColumn(col) Then
                writer.AddField(row(col))
              End If
            Next
            If i < DataTable.Rows.Count - 1 Then
              writer.NextRecord()
            End If
          Next

        End Using

        Return ms.ToArray
      End Using

    End Function

#End If

    Public Shared Function ImportCSVToStringDataTable(ByVal File As Byte(), Optional ByVal ColHeadingsFirstRow As Boolean = False) As DataTable

      Dim ms As IO.MemoryStream = New MemoryStream(File)
      Dim fs As StreamReader = New StreamReader(ms)

      Dim FileLines As List(Of String) = New List(Of String)
      Dim str As String = fs.ReadLine
      While str IsNot Nothing
        FileLines.Add(str)
        str = fs.ReadLine
      End While

      Dim tbl As New DataTable()

      Dim readFirstRow As Boolean = True
      Dim isFirstRow As Boolean = True
      If ColHeadingsFirstRow AndAlso FileLines.Count > 0 Then
        Dim Fields() As String = FileLines(0).Split(",")
        For Each f In Fields
          tbl.Columns.Add(New DataColumn() With {
                          .ColumnName = f,
                          .DataType = GetType(String)
                        })
        Next

        readFirstRow = False
      End If

      For Each line As String In FileLines
        If Not isFirstRow OrElse readFirstRow Then
          Dim Fields() As String = line.Split(",")

          While tbl.Columns.Count < Fields.Length
            ' add new columns
            tbl.Columns.Add(New DataColumn() With {.DataType = GetType(String)})
          End While

          Dim drw As DataRow = tbl.NewRow
          tbl.Rows.Add(drw)
          For i As Integer = 0 To Fields.Length - 1
            drw(i) = Fields(i)
          Next

        End If
        isFirstRow = False
      Next

      Return tbl

    End Function

    Public Shared Function ImportCSVToStringDataTable(ByVal FileName As String, Optional ByVal ColHeadingsFirstRow As Boolean = False) As DataTable

      Dim FileLines() As String = IO.File.ReadAllLines(FileName)

      Dim tbl As New DataTable()

      For Each line As String In FileLines
        Dim Fields() As String = line.Split(",")

        While tbl.Columns.Count < Fields.Length
          ' add new columns
          tbl.Columns.Add(New DataColumn() With {.DataType = GetType(String)})
        End While

        Dim drw As DataRow = tbl.NewRow
        tbl.Rows.Add(drw)
        For i As Integer = 0 To Fields.Length - 1
          drw(i) = Fields(i)
        Next
      Next

      Return tbl

    End Function

  End Class

#End Region

#Region " Data Sructures "

  Public Class LazySortedDictionary(Of Key, Value)
    Inherits SortedDictionary(Of Key, Value)

    Public Sub New()

    End Sub

    Public Function CreateIfNew(k As Key, Optional PassKeyToConstructor As Boolean = False) As Value
      If PassKeyToConstructor Then
        Return CreateIfNewInternal(k, Nothing, {k})
      Else
        Return CreateIfNewInternal(k, Nothing)
      End If
    End Function

    Public Function CreateIfNew(k As Key, ParamArray ConstructorParams() As Object) As Value
      Return CreateIfNewInternal(k, Nothing, ConstructorParams)
    End Function

    Public Function CreateIfNew(k As Key, CreateCallback As Func(Of Value)) As Value
      Return CreateIfNewInternal(k, CreateCallback)
    End Function

    Private Function CreateIfNewInternal(k As Key, CreateCallback As Func(Of Value), ParamArray ConstructorParams() As Object) As Value
      Dim Val As Value

      If TryGetValue(k, Val) Then
        Return Val
      ElseIf CreateCallback IsNot Nothing Then
        Val = CreateCallback()
        Me.Add(k, Val)
        Return Val
      Else
        Val = Activator.CreateInstance(GetType(Value), ConstructorParams)
        Me.Add(k, Val)
        Return Val
      End If
    End Function

  End Class

  Public Class LazyDictionary(Of Key, Value)
    Inherits Dictionary(Of Key, Value)

    Public Function CreateIfNew(k As Key, Optional PassKeyToConstructor As Boolean = False) As Value
      If PassKeyToConstructor Then
        Return CreateIfNewInternal(k, Nothing, {k})
      Else
        Return CreateIfNewInternal(k, Nothing)
      End If
    End Function

    Public Function CreateIfNew(k As Key, ParamArray ConstructorParams() As Object) As Value
      Return CreateIfNewInternal(k, Nothing, ConstructorParams)
    End Function

    Public Function CreateIfNew(k As Key, CreateCallback As Func(Of Value)) As Value
      Return CreateIfNewInternal(k, CreateCallback)
    End Function

    Private Function CreateIfNewInternal(k As Key, CreateCallback As Func(Of Value), ParamArray ConstructorParams() As Object) As Value
      Dim Val As Value

      If TryGetValue(k, Val) Then
        Return Val
      ElseIf CreateCallback IsNot Nothing Then
        Val = CreateCallback()
        Me.Add(k, Val)
        Return Val
      Else
        Val = Activator.CreateInstance(GetType(Value), ConstructorParams)
        Me.Add(k, Val)
        Return Val
      End If
    End Function

  End Class


#End Region

#End If


End Namespace
