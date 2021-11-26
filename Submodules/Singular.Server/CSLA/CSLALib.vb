Imports Singular.Extensions
Imports System.Text.RegularExpressions


#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
Imports System.Reflection
Imports Csla
Imports System.IO
#End If

Namespace CSLALib

  Public Module CSLALib

    Public Function GetDisplayNameFromProperty(ByVal [Property] As System.Reflection.PropertyInfo) As String

      Dim DisplayName As String = [Property].Name.Readable()

#If SILVERLIGHT Then
      Dim da = Singular.Reflection.GetDisplayAttribute([Property])
      If da IsNot Nothing Then
        DisplayName = da.Name
      End If
#End If

      Return DisplayName

    End Function

    Public Function GetTableName(ByVal ObjName As String) As String

      If ObjName.EndsWith("List") Then
        ObjName = ObjName.Substring(0, ObjName.Length - 4)
      End If

      If ObjName.StartsWith("RO") Then
        ObjName = ObjName.Substring(2)
      End If

      Return Singular.Strings.Pluralize(ObjName)

    End Function

#If SILVERLIGHT Then

    Public Function BeginSave(Of T As SingularBusinessListBase(Of T, C), C As SingularBusinessBase(Of C))(ByVal ListToSave As T, ByVal CallBack As EventHandler(Of Csla.Core.SavedEventArgs)) As Boolean

      If ListToSave.IsValid Then
        ListToSave.BeginSave(CallBack)
        Return True
      Else
        Singular.StdMsgBoxes.CannotSave_InvalidData(ListToSave)
        Return False
      End If

    End Function

    Public Function BeginSave(Of C As SingularBusinessBase(Of C))(ByVal ObjectToSave As C, ByVal CallBack As EventHandler(Of Csla.Core.SavedEventArgs)) As Boolean

      If ObjectToSave.IsValid Then
        ObjectToSave.BeginSave(CallBack)
        Return True
      Else
        Singular.StdMsgBoxes.CannotSave_InvalidData(ObjectToSave)
        Return False
      End If

    End Function

#Else

#Region " Context Info "

    Public Class ContextInfo

      Private Shared mSetContextInfo As Boolean
      Public Shared Property SetContextInfo() As Boolean
        Get
          Return mSetContextInfo
        End Get
        Set(ByVal value As Boolean)
          mSetContextInfo = value
        End Set
      End Property

      Private Shared mLastConnection As SqlConnection

      Public Shared Sub SetContextInfoOnConnection(ByVal cn As SqlConnection, ByVal tr As SqlTransaction)
        If mSetContextInfo Then

          If mLastConnection Is Nothing OrElse mLastConnection IsNot cn Then

            Dim cmd As New SqlCommand(My.Resources.SetContextInfo)
            cmd.Parameters.AddWithValue("@UserID", Singular.Settings.CurrentUserID)
            cmd.Connection = cn
            cmd.Transaction = tr
            cmd.ExecuteNonQuery()

            mLastConnection = cn
          End If

        End If
      End Sub

      Public Shared Sub SetContextInfoOnConnection(Of T As SingularBusinessBase(Of T))(ByVal cn As SqlConnection, ByVal tr As SqlTransaction)
        SetContextInfoOnConnection(cn, tr)
      End Sub


      Private Shared mLastMySqlConnection As MySql.Data.MySqlClient.MySqlConnection

      Public Shared Sub SetContextInfoOnConnection(Of T As SingularBusinessBase(Of T))(ByVal cn As MySql.Data.MySqlClient.MySqlConnection, ByVal tr As MySql.Data.MySqlClient.MySqlTransaction)

        If mSetContextInfo Then

          If mLastMySqlConnection Is Nothing OrElse mLastMySqlConnection IsNot cn Then

            Dim cmd As New MySql.Data.MySqlClient.MySqlCommand(My.Resources.SetContextInfo)
            cmd.Parameters.AddWithValue("@UserID", Singular.Settings.CurrentUserID)
            cmd.Connection = cn
            cmd.Transaction = tr
            cmd.ExecuteNonQuery()

            mLastMySqlConnection = cn
          End If

        End If

      End Sub

    End Class

#End Region

#End If

#If SILVERLIGHT Then

#Else

    <Obsolete("Please Use 'GetDatasetFromBusinessListBase' instead")>
    Public Function ScanBusinessListBaseToDataSet(ByVal BusinessListBase As Object, Optional ByVal IgnoreReturnedCSLASingleObjects As Boolean = False, Optional ByVal IgnoreChildObjectsCompletely As Boolean = False) As DataSet

      Dim ds As New DataSet
      For Each child As Object In BusinessListBase
        ScanToDataSetForObject(child, ds, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely)
      Next
      Return ds

    End Function

    <Obsolete("Please Use 'GetDatasetFromBusinessBase' instead")>
    Public Sub ScanToDataSetForObject(ByVal BusinessBase As Object, ByRef DataSet As DataSet, Optional ByVal IgnoreReturnedCSLASingleObjects As Boolean = False, Optional ByVal IgnoreChildObjectsCompletely As Boolean = False)

      ' get the public properties of the object
      Dim pis() As PropertyInfo = BusinessBase.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.Public)

      Dim bAddChildren As Boolean = False '*** to handle children

      ' get or create the table associated with this 
      Dim tbl As DataTable
      If DataSet.Tables.Contains(BusinessBase.GetType.Name.Substring(Math.Max(0, BusinessBase.GetType.Name.LastIndexOf("."))) & "s") Then
        tbl = DataSet.Tables(BusinessBase.GetType.Name.Substring(Math.Max(0, BusinessBase.GetType.Name.LastIndexOf("."))) & "s")
        bAddChildren = True '*** Need to add child records

      Else
        tbl = New DataTable(BusinessBase.GetType.Name.Substring(Math.Max(0, BusinessBase.GetType.Name.LastIndexOf("."))) & "s")
        DataSet.Tables.Add(tbl)
        ' create the columns
        For Each pi As PropertyInfo In pis

          Dim PropertyIsListObject As Boolean = (Singular.CSLALib.InheritsFromBusinessListBase(pi.PropertyType) OrElse Singular.CSLALib.InheritsFromReadOnlyListBase(pi.PropertyType))

          If Not IsNothing(pi.PropertyType.BaseType) AndAlso PropertyIsListObject Then 'pi.PropertyType.BaseType.Name = "BusinessListBase`2" Then
            If Not IgnoreChildObjectsCompletely Then
              ' we have another collection
              ' make sure its not nothing
              Dim obj As Object = CallByName(BusinessBase, pi.Name, Microsoft.VisualBasic.CallType.Get)

              If Not IsNothing(obj) Then
                ScanBusinessListBaseIntoDataSet(obj, DataSet, IgnoreReturnedCSLASingleObjects)
              End If
            End If
          Else
            If ValidProperty(pi) Then
              If Not IgnoreReturnedCSLASingleObjects Or Not IgnoreChildObjectsCompletely Or (Not pi.PropertyType.IsSubclassOf(GetType(Csla.Core.BindableBase)) And Not pi.PropertyType.IsSubclassOf(GetType(Csla.Core.BindableBase))) Then
                If pi.Name.IndexOf("Date") <> -1 AndAlso pi.PropertyType.Equals(GetType(Object)) Then
                  tbl.Columns.Add(pi.Name, GetType(DateTime))
                Else
                  If pi.PropertyType.Equals(GetType(Object)) Then
                    Dim o As Object = CallByName(BusinessBase, pi.Name, Microsoft.VisualBasic.CallType.Get)
                    If IsDBNull(o) And Not IsNothing(o) Then
                      tbl.Columns.Add(pi.Name, GetType(Object))
                    Else
                      tbl.Columns.Add(pi.Name, o.GetType)
                    End If
                  Else
                    If pi.PropertyType.Equals(GetType(DateTime)) Then
                      tbl.Columns.Add(pi.Name, GetType(DateTime))
                    ElseIf pi.PropertyType.Equals(GetType(Object)) Then
                      tbl.Columns.Add(pi.Name, GetType(String))
                    ElseIf pi.PropertyType.Equals(GetType(Nullable(Of DateTime))) Then
                      tbl.Columns.Add(pi.Name, GetType(DateTime))
                    ElseIf pi.PropertyType.Equals(GetType(Nullable(Of Decimal))) Then
                      tbl.Columns.Add(pi.Name, GetType(String))
                    ElseIf pi.PropertyType.Equals(GetType(Nullable(Of Integer))) Then
                      tbl.Columns.Add(pi.Name, GetType(String))
                    Else
                      tbl.Columns.Add(pi.Name, pi.PropertyType)
                    End If
                  End If

                End If
              Else
                Dim s As String = ""
              End If
            End If
          End If
        Next
      End If

      ' add a row to the table
      Dim drw As DataRow = tbl.NewRow
      tbl.Rows.Add(drw)
      For Each pi As PropertyInfo In pis

        Dim PropertyIsListObject As Boolean = (Singular.CSLALib.InheritsFromBusinessListBase(pi.PropertyType) OrElse Singular.CSLALib.InheritsFromReadOnlyListBase(pi.PropertyType))


        If Not IsNothing(pi.PropertyType.BaseType) AndAlso Not PropertyIsListObject Then 'pi.PropertyType.BaseType.Name = "BusinessListBase`2" Then
          If ValidProperty(pi) Then
            If Not IgnoreReturnedCSLASingleObjects Or (Not pi.PropertyType.IsSubclassOf(GetType(Csla.Core.BusinessBase))) Then
              Dim o As Object = CallByName(BusinessBase, pi.Name, Microsoft.VisualBasic.CallType.Get)
              If IsNothing(o) Then
                o = DBNull.Value
              End If
              drw(pi.Name) = o
            End If
          End If
        Else
          '*** Loop through to add children
          If bAddChildren Then
            If Not IsNothing(pi.PropertyType.BaseType) AndAlso PropertyIsListObject Then 'pi.PropertyType.BaseType.Name = "BusinessListBase`2" Then
              Dim col As Object = CallByName(BusinessBase, pi.Name, Microsoft.VisualBasic.CallType.Get)
              If Not IsNothing(col) Then
                ScanBusinessListBaseIntoDataSet(col, DataSet, IgnoreReturnedCSLASingleObjects)  '*** Include Deleted 
              End If
            ElseIf pi.PropertyType.IsSubclassOf(GetType(Csla.Core.BindableBase)) Then
              Dim bb As Object = CallByName(BusinessBase, pi.Name, Microsoft.VisualBasic.CallType.Get)
              If Not IsNothing(bb) Then
                ScanToDataSetForObject(bb, DataSet, IgnoreReturnedCSLASingleObjects)
              End If
            End If
          End If
        End If
      Next

    End Sub


#Region "Get Dataset from Business Base New"

    Public Function GetDatasetFromBusinessListBase(ByVal List As Object, ByVal IgnoreReturnedCSLASingleObjects As Boolean, ByVal IgnoreChildObjectsCompletely As Boolean,
                                                   Optional AddExtendedProperties As Boolean = True, Optional UseDisplayAttributeOrder As Boolean = False, Optional AddNonClsaListChildren As Boolean = False, Optional UseDropDownTextValue As Boolean = False, Optional UseDisplayNameAsHeader As Boolean = False) As DataSet

      Dim ds As DataSet = New DataSet

      'Create The Root Table
      For Each obj In List
        AddBusinessBase(obj, ds, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, AddExtendedProperties, UseDisplayAttributeOrder, AddNonClsaListChildren, UseDropDownTextValue, UseDisplayNameAsHeader)
      Next

      Return ds

    End Function

    Public Function GetDatasetFromBusinessBase(ByVal Obj As Object, ByVal IgnoreReturnedCSLASingleObjects As Boolean, ByVal IgnoreChildObjectsCompletely As Boolean,
                                                   Optional AddExtendedProperties As Boolean = True, Optional AddNonClsaListChildren As Boolean = False) As DataSet
      Dim ds As DataSet = New DataSet

      AddBusinessBase(Obj, ds, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, AddExtendedProperties, , AddNonClsaListChildren)

      Return ds
    End Function
		'************-------------------################
		Private Sub AddBusinessBase(Obj As Object, ByRef ds As DataSet, ByVal IgnoreReturnedCSLASingleObjects As Boolean, ByVal IgnoreChildObjectsCompletely As Boolean,
																AddExtendedProperties As Boolean, Optional UseDisplayAttributeOrder As Boolean = False, Optional AddNonClsaListChildren As Boolean = False, Optional UseDropDownTextValue As Boolean = False, Optional UseDisplayNameAsHeader As Boolean = False)

			Dim Type As String = Obj.GetType.Name.Substring(Math.Max(0, Obj.GetType.Name.LastIndexOf(".")))

			Type = Singular.Strings.Pluralize(Type)

			Dim tbl As DataTable
			If ds.Tables.Contains(Type) Then
				tbl = ds.Tables(Type)
			Else
				tbl = New DataTable(Type)
				ds.Tables.Add(tbl)
			End If

			Dim pis As List(Of PropertyInfo) = Nothing
			If UseDisplayAttributeOrder Then
				Dim pis1 As List(Of PropertyInfo) = Obj.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.Public).ToList
				'********************--------------------#########################
				'Arranging Property Columns According To Order Value.
				Dim pis2 = (From pi In pis1
										From ca In pi.GetCustomAttributes(True)
										Where TypeOf ca Is ComponentModel.DataAnnotations.DisplayAttribute
										Select pi, Order = DirectCast(ca, ComponentModel.DataAnnotations.DisplayAttribute).GetOrder).ToList

				pis = (From pi1 In pis1
							 Group Join pi2 In pis2 On pi2.pi Equals pi1 Into Group
							 From pi2 In Group.DefaultIfEmpty
							 Order By If(pi2 Is Nothing OrElse Not pi2.Order.HasValue, Integer.MaxValue, pi2.Order)
							 Select pi1).ToList
			Else
				pis = Obj.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.Public).ToList
			End If
			'********************--------------------#########################

			'Find Primary Key
			Dim pk As PropertyInfo = Nothing
			'Look for Key Attribute
			Dim showField As Boolean = True
			For Each pi As PropertyInfo In pis
				Dim ca() As Object = pi.GetCustomAttributes(False)
				If ca.Length > 0 Then
					If ca.FirstOrDefault(Function(c) TypeOf c Is ComponentModel.DataAnnotations.KeyAttribute) IsNot Nothing Then
						pk = pi
						Exit For
					End If
				End If
			Next
			If pk Is Nothing Then
				'look for object name & ID
				pk = pis.FirstOrDefault(Function(c) c.Name = Type & "ID")
				'if still nothing just get first one
				If pk Is Nothing Then
					pk = pis(0)
				End If
			End If

			'Add columns
			Dim pkColumn As DataColumn = Nothing
			If tbl.Columns.Contains(pk.Name) Then
				pkColumn = tbl.Columns(pk.Name)
			Else
				pkColumn = tbl.Columns.Add(pk.Name, GetTableColumnType(pk.PropertyType))
				If AddExtendedProperties Then

					Dim ca() As Object = pk.GetCustomAttributes(False)
					Dim dda As ComponentModel.DataAnnotations.DisplayAttribute = ca.FirstOrDefault(Function(c) TypeOf c Is ComponentModel.DataAnnotations.DisplayAttribute)
					If dda IsNot Nothing AndAlso dda.GetAutoGenerateField IsNot Nothing Then
						pkColumn.ExtendedProperties.Add(Singular.Data.DataTables.ExtendedProperties.AutoGenerate.ToString(), dda.GetAutoGenerateField)
					End If
				End If
			End If

			Dim ChildObjectsToAdd As List(Of PropertyInfo) = AddColumns(pis, Obj, tbl, AddExtendedProperties, AddNonClsaListChildren, UseDropDownTextValue, UseDisplayNameAsHeader)

			'Add rows
			Dim row As DataRow = tbl.NewRow()
			tbl.Rows.Add(row)


			For Each col As DataColumn In tbl.Columns
				Dim pi As PropertyInfo = pis.FirstOrDefault(Function(c) c.Name = col.ColumnName)
				If pi IsNot Nothing Then
					Dim ddw = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DropDownWeb)(pi)
					Dim computed = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ComputedProperty)(pi)
					Dim value As Object = pi.GetValue(Obj, Nothing)
					If computed IsNot Nothing Then 'value IsNot Nothing AndAlso Singular.Reflection.IsDerivedFromGenericType(value.GetType, GetType(Expressions.Expression(Of ))) Then
						' expression

						' no need to do anything as it has been added to the extended properties of the column

						'Dim decom As New DelegateDecompiler.MethodBodyDecompiler(pi.GetMethod())
						'Dim expr = decom.Decompile

						'' Dim exp As Expressions.Expression(Of Func(Of Decimal)) = value
						'row(col) = ParseExpressionToText(expr)
						If Not computed.SupportsExcel Then
							row(col) = Singular.Misc.NothingDBNull(value)
						End If
					Else
						If UseDropDownTextValue = True And ddw IsNot Nothing Then
							row(col) = ddw.GetDisplayFromID(pi.GetValue(Obj, Nothing), Obj)
						Else
							row(col) = Singular.Misc.NothingDBNull(value)
						End If
					End If
				End If
			Next

			AddChildren(ChildObjectsToAdd, Obj, ds, pkColumn, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, AddExtendedProperties, AddNonClsaListChildren)

		End Sub

		Private Function ParseExpressionToText(expression As Expressions.LambdaExpression) As String

      Dim expressionText As String = expression.ToString()

      ' strip off "() => "
      expressionText = expressionText.Substring("this => ".Length)

      expressionText = expressionText.Replace("this.", "")

      ' replace fully qualified fields with simple fieldnames surrounded by []
      'Dim regex As New Regex("^value(" & sourceObject.GetType.FullName & ").$")

      'While expressionText.Contains("value(" & sourceObject.GetType.FullName & ").")

      'End While
      expressionText = expressionText.Replace("IIF(", "IF(") ' replace IIF
      expressionText = expressionText.Replace("==", "=") ' replace ==
      expressionText = expressionText.Replace("!=", "<>") ' replace ==

      expressionText = ReplaceConstants(expressionText)

      ' expressionText = expressionText.Replace(".Compile().Invoke()", "") ' replace IIF
      Return expressionText

    End Function

    Private Function ReplaceConstants(expression As String) As String

      Dim supportedTypes = {"Decimal", "Integer"}

      For Each supportedType In supportedTypes
        Dim startText As String = "new " & supportedType & "(Convert("
        Dim index As Integer = 0
        Dim numText As String = ""
        While expression.IndexOf(startText) > 0
          index = expression.IndexOf(startText) + startText.Length
          While IsNumeric(expression(index)) OrElse expression(index) = "."
            numText &= expression(index)
            index += 1
          End While
          ' now replace that text
          expression = expression.Replace(startText & numText & "))", numText)
        End While
      Next

      ' replace .Zeros
      expression = expression.Replace("Decimal.Zero", "0").Replace("Integer.Zero", "0")

      Return expression

    End Function

    Private Function ParseExpressionToText(Of T)(expression As Expressions.Expression(Of Func(Of T)), sourceObject As Object, table As DataTable) As String

      Dim expressionText As String = expression.ToString()

      ' strip off "() => "
      expressionText = expressionText.Substring("() => ".Length)

      expressionText = expressionText.Replace("value(" & sourceObject.GetType.FullName & ").", "")

      ' replace fully qualified fields with simple fieldnames surrounded by []
      'Dim regex As New Regex("^value(" & sourceObject.GetType.FullName & ").$")

      'While expressionText.Contains("value(" & sourceObject.GetType.FullName & ").")

      'End While

      expressionText = expressionText.Replace("IIF(", "IF(") ' replace IIF

      expressionText = expressionText.Replace(".Compile().Invoke()", "") ' replace IIF
      Return expressionText

    End Function

		''' <summary>
		''' Add Columns to DataTable and Returns List of Properties that are Business Objects
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function AddColumns(pis As List(Of PropertyInfo), obj As Object, ByRef tbl As DataTable,
																AddExtendedProperties As Boolean, Optional AddNonClsaListChildren As Boolean = False, Optional UseDropDownTextValue As Boolean = False, Optional UseDisplayNameAsHeader As Boolean = False) As List(Of PropertyInfo)

			Dim ret As New List(Of PropertyInfo)

			For Each pi In pis
				If IsBusinessObjectOrList(pi.PropertyType) OrElse (AddNonClsaListChildren AndAlso Singular.Reflection.IsDerivedFromGenericType(pi.PropertyType, GetType(List(Of )))) Then
					'Object Or List Object
					ret.Add(pi)
				ElseIf ValidProperty(pi) Then
					'Normal Property
					If Not tbl.Columns.Contains(pi.Name) Then
						Dim col As DataColumn = Nothing
						Dim computed = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ComputedProperty)(pi)
						Dim ddi = If(UseDropDownTextValue, Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DropDownWeb)(pi), Nothing)

						If ddi IsNot Nothing Then
							col = tbl.Columns.Add(pi.Name, GetType(String))
							col.Caption = Singular.Reflection.GetDisplayName(pi)
						ElseIf computed IsNot Nothing Then
							col = tbl.Columns.Add(pi.Name, pi.PropertyType)
							'If Singular.Reflection.IsDerivedFromGenericType(pi.PropertyType, GetType(Expressions.Expression(Of ))) Then
							'  col = tbl.Columns.Add(pi.Name, GetType(String))
							'  col.ExtendedProperties.Add(Singular.Data.DataTables.ExtendedProperties.FormulaField.ToString(), True)
						ElseIf pi.Name.IndexOf("Date") <> -1 AndAlso pi.PropertyType.Equals(GetType(Object)) Then
							col = tbl.Columns.Add(pi.Name, GetType(DateTime))
						Else
							If pi.PropertyType.Equals(GetType(Object)) Then
								Dim o As Object = CallByName(obj, pi.Name, Microsoft.VisualBasic.CallType.Get)
								If IsDBNull(o) And Not IsNothing(o) Then
									col = tbl.Columns.Add(pi.Name, GetType(Object))
								Else
									col = tbl.Columns.Add(pi.Name, o.GetType)
								End If
							Else
								col = tbl.Columns.Add(pi.Name, GetTableColumnType(pi.PropertyType))
							End If
						End If

						If UseDisplayNameAsHeader Then
							col.Caption = Singular.Reflection.GetDisplayName(pi)
						End If

						If AddExtendedProperties Then

							Dim piC = Singular.ReflectionCached.GetCachedMemberInfo(pi)

							If computed IsNot Nothing Then
								Dim expression = CType(piC.BackingField, ISingularPropertyInfo).GetParsedGetExpression()
								col.ExtendedProperties.Add(Singular.Data.DataTables.ExtendedProperties.FormulaField.ToString(),
																						New FormulaField() With {.Formula = ParseExpressionToText(expression),
																																		 .SupportsJS = computed.SupportsJS,
																																		 .SupportsExcel = computed.SupportsExcel})
							End If

							Dim excelFormat = Singular.Reflection.GetAttribute(Of Singular.Data.ExcelExporter.ExcelFormatAttribute)(pi)
							If excelFormat IsNot Nothing Then
								col.ExtendedProperties.Add(Singular.Data.DataTables.ExtendedProperties.ExcelFormat.ToString(), excelFormat)
							End If

							If piC.AutoGenerate = False OrElse (piC.ClientOnly.HasValue AndAlso piC.ClientOnly = True) Then
								col.ExtendedProperties.Add(Singular.Data.DataTables.ExtendedProperties.AutoGenerate.ToString(), False)
							End If

							Dim NumberField = Singular.Reflection.GetAttribute(Of DataAnnotations.NumberField)(pi)
							If NumberField IsNot Nothing Then
                col.ExtendedProperties.Add(Singular.Data.DataTables.ExtendedProperties.FormatString.ToString(), NumberField.FormatString)
              End If

						End If
					End If
				End If
			Next

			Return ret

		End Function

		Public Class FormulaField

      Public Property SupportsJS As Boolean
      Public Property SupportsExcel As Boolean
      Public Property Formula As String

    End Class

    Private Sub AddChildren(ChildObjectsToAdd As List(Of PropertyInfo), Obj As Object, ds As DataSet, pkColumn As DataColumn, IgnoreReturnedCSLASingleObjects As Boolean, IgnoreChildObjectsCompletely As Boolean,
                                AddExtendedProperties As Boolean, Optional AddNonClsaListChildren As Boolean = False)
      For Each pi In ChildObjectsToAdd
        If InheritsFromBusinessBase(pi.PropertyType) OrElse InheritsFromReadOnlyBase(pi.PropertyType) Then
          'Object
          If IgnoreReturnedCSLASingleObjects = False Then
            AddChildBusinessBase(pi.GetValue(Obj, Nothing), ds, pkColumn, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, AddExtendedProperties, AddNonClsaListChildren)
          End If
        Else
          'List Object
          If IgnoreChildObjectsCompletely = False Then
            AddChildBusinessListBase(pi.GetValue(Obj, Nothing), ds, pkColumn, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, AddExtendedProperties, AddNonClsaListChildren)
          End If
        End If
      Next
    End Sub

    Private Sub AddChildBusinessListBase(List As Object, ByRef ds As DataSet, ParentColumn As DataColumn, ByVal IgnoreReturnedCSLASingleObjects As Boolean, ByVal IgnoreChildObjectsCompletely As Boolean,
                                AddExtendedProperties As Boolean, Optional AddNonClsaListChildren As Boolean = False)

      For Each obj In List
        AddChildBusinessBase(obj, ds, ParentColumn, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, AddExtendedProperties, AddNonClsaListChildren)
      Next

    End Sub

    Private Sub AddChildBusinessBase(Obj As Object, ByRef ds As DataSet, ParentColumn As DataColumn, ByVal IgnoreReturnedCSLASingleObjects As Boolean, ByVal IgnoreChildObjectsCompletely As Boolean,
                                AddExtendedProperties As Boolean, Optional AddNonClsaListChildren As Boolean = False)

      Dim Type As String = Obj.GetType.Name.Substring(Math.Max(0, Obj.GetType.Name.LastIndexOf(".")))

      Dim tbl As DataTable
      If ds.Tables.Contains(Type & "s") Then
        tbl = ds.Tables(Type & "s")
      Else
        tbl = New DataTable(Type & "s")
        ds.Tables.Add(tbl)
      End If

      Dim pis As List(Of PropertyInfo) = Obj.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.Public).ToList
      'Find Primary Key
      Dim pk As PropertyInfo = Nothing
      'Look for Key Attribute
      For Each pi As PropertyInfo In pis
        Dim ca() As Object = pi.GetCustomAttributes(False)
        If ca.Length > 0 AndAlso ca.FirstOrDefault(Function(c) TypeOf c Is ComponentModel.DataAnnotations.KeyAttribute) IsNot Nothing Then
          pk = pi
          Exit For
        End If
      Next
      If pk Is Nothing Then
        'look for object name & ID
        pk = pis.FirstOrDefault(Function(c) c.Name = Type & "ID")
        'if still nothing just get first one
        If pk Is Nothing Then
          pk = pis(0)
        End If
      End If
      Dim pkColumn As DataColumn = Nothing
      If tbl.Columns.Contains(pk.Name) Then
        pkColumn = tbl.Columns(pk.Name)
      Else
        pkColumn = tbl.Columns.Add(pk.Name, pk.PropertyType)
        If AddExtendedProperties Then
          Dim ca() As Object = pk.GetCustomAttributes(False)
          Dim dda As ComponentModel.DataAnnotations.DisplayAttribute = ca.FirstOrDefault(Function(c) TypeOf c Is ComponentModel.DataAnnotations.DisplayAttribute)
          If dda IsNot Nothing AndAlso dda.GetAutoGenerateField() IsNot Nothing Then
            pkColumn.ExtendedProperties.Add(Singular.Data.DataTables.ExtendedProperties.AutoGenerate.ToString(), dda.AutoGenerateField)
          End If
        End If
      End If

      'Find Foreign Key
      'Match to Parent Column Name
      Dim fk As PropertyInfo = pis.FirstOrDefault(Function(c) c.Name = ParentColumn.ColumnName)
      'If Nothing, match to ending in parent column name
      If fk Is Nothing Then
        fk = pis.FirstOrDefault(Function(c) c.Name.EndsWith(ParentColumn.ColumnName))
      End If

      Dim fkColumn As DataColumn = Nothing
      If fk IsNot Nothing Then
        If Not tbl.Columns.Contains(fk.Name) Then
          If fk.PropertyType.IsGenericType Then
            fkColumn = tbl.Columns.Add(fk.Name, fk.PropertyType.GetGenericArguments(0))
          Else
            fkColumn = tbl.Columns.Add(fk.Name, fk.PropertyType)
          End If
          If AddExtendedProperties Then
            Dim ca() As Object = fk.GetCustomAttributes(False)
            Dim dda As ComponentModel.DataAnnotations.DisplayAttribute = ca.FirstOrDefault(Function(c) TypeOf c Is ComponentModel.DataAnnotations.DisplayAttribute)
            If dda IsNot Nothing AndAlso dda.GetAutoGenerateField() IsNot Nothing Then
              fkColumn.ExtendedProperties.Add(Singular.Data.DataTables.ExtendedProperties.AutoGenerate.ToString(), dda.AutoGenerateField)
            End If
          End If
          ds.Relations.Add(ParentColumn, fkColumn)
        End If
      End If

      Dim ChildObjectsToAdd As List(Of PropertyInfo) = AddColumns(pis, Obj, tbl, AddExtendedProperties, AddNonClsaListChildren)

      Dim row As DataRow = tbl.NewRow()
      tbl.Rows.Add(row)

      For Each col As DataColumn In tbl.Columns
        Dim pi As PropertyInfo = pis.FirstOrDefault(Function(c) c.Name = col.ColumnName)
        If pi IsNot Nothing Then
          row(col) = Singular.Misc.NothingDBNull(pi.GetValue(Obj, Nothing))
        End If
      Next

      AddChildren(ChildObjectsToAdd, Obj, ds, pkColumn, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely, AddExtendedProperties, AddNonClsaListChildren)

    End Sub

    Private Function GetTableColumnType(PropertyType As Type) As Type

      If PropertyType.Equals(GetType(DateTime)) Then
        Return GetType(DateTime)
      ElseIf PropertyType.Equals(GetType(Object)) Then
        Return GetType(String)
      ElseIf PropertyType.Equals(GetType(Nullable(Of DateTime))) Then
        Return GetType(DateTime)
      ElseIf PropertyType.Equals(GetType(Nullable(Of Decimal))) Then
        Return GetType(String)
      ElseIf PropertyType.Equals(GetType(Nullable(Of Integer))) Then
        Return GetType(String)
      ElseIf PropertyType.Name.StartsWith("Nullable") Then
        Return GetType(Object)
      Else
        Return PropertyType
      End If
    End Function

#End Region

    Public Function IsBusinessObjectOrList(ByVal Type As Type) As Boolean

      Return InheritsFromBusinessListBase(Type) OrElse
             InheritsFromReadOnlyListBase(Type) OrElse
             InheritsFromBusinessBase(Type) OrElse
             InheritsFromReadOnlyBase(Type)

    End Function

    Public Function InheritsFromBusinessBase(ByVal ObjectType As Type) As Boolean

      If ObjectType Is GetType(Csla.Core.BusinessBase) Then
        Return True
      ElseIf IsNothing(ObjectType.BaseType) Then
        Return False
      Else
        Return InheritsFromBusinessBase(ObjectType.BaseType)
      End If

    End Function

    Public Function InheritsFromBusinessListBase(ByVal ObjectType As Type) As Boolean

      If ObjectType.Name = "BusinessListBase`2" OrElse ObjectType.Name = "BusinessBindingListBase`2" Then
        Return True
      ElseIf IsNothing(ObjectType.BaseType) Then
        Return False
      Else
        Return InheritsFromBusinessListBase(ObjectType.BaseType)
      End If

    End Function

    <Obsolete("Please Use 'GetDatasetFromBusinessBase' instead")>
    Public Sub ScanBusinessListBaseIntoDataSet(ByVal BusinessListBase As Object, ByRef DataSet As DataSet, Optional ByVal IgnoreReturnedCSLASingleObjects As Boolean = False, Optional ByVal IgnoreChildObjectsCompletely As Boolean = False)

      ' get or create the table associated with this 
      For Each bb As Object In BusinessListBase
        ScanToDataSetForObject(bb, DataSet, IgnoreReturnedCSLASingleObjects, IgnoreChildObjectsCompletely)
      Next

    End Sub

    Public Function InheritsFromReadOnlyBase(ByVal ObjectType As Type) As Boolean

      If ObjectType.Name.StartsWith("ReadOnlyBase") Then
        Return True
      ElseIf IsNothing(ObjectType.BaseType) Then
        Return False
      Else
        Return InheritsFromReadOnlyBase(ObjectType.BaseType)
      End If

    End Function

    Public Function InheritsFromReadOnlyListBase(ByVal ObjectType As Type) As Boolean

      If ObjectType.Name = "ReadOnlyBindingList`1" Then
        Return True
      ElseIf IsNothing(ObjectType.BaseType) Then
        Return False
      Else
        Return InheritsFromReadOnlyListBase(ObjectType.BaseType)
      End If

    End Function

    Public Function ValidProperty(ByVal pi As PropertyInfo) As Boolean

      Dim validProp = Not (pi.Name.StartsWith("BrokenRules") Or _
                    pi.Name = "IsNew" Or _
                    pi.Name = "IsDeleted" Or _
                    pi.Name = "IsDirty" Or pi.Name = "IsRecentlyDirty" Or _
                    pi.Name = "IsSavable" Or _
                    pi.Name = "IsValid" Or pi.Name = "UseErrorsOrWarnings" Or _
                    pi.Name = "Parent" Or pi.Name = "FieldManager" Or _
                    pi.Name = "IsBusy" Or pi.Name = "IsSelfBusy" Or _
                    pi.Name = "BusinessRules" Or pi.Name = "StorageDynamic" Or _
                    pi.Name = "Storage" Or pi.Name = "IsSelfValid" Or _
                    pi.Name = "IsChild" Or pi.Name = "IsSelfDirty" Or _
                    pi.Name = "Guid")

      If validProp Then
        ' check if should be displayed 
        Dim da = Singular.Reflection.GetAttribute(Of ComponentModel.DataAnnotations.DisplayAttribute)(pi)
        If da IsNot Nothing Then
          validProp = Singular.Misc.IsNull(CType(da, ComponentModel.DataAnnotations.DisplayAttribute).GetAutoGenerateField, True)
        End If

        If validProp Then
          Dim ba = Singular.Reflection.GetAttribute(Of ComponentModel.BrowsableAttribute)(pi)
          If ba IsNot Nothing Then
            validProp = CType(ba, ComponentModel.BrowsableAttribute).Browsable
          End If
        End If
      End If
      Return validProp

    End Function

    Public Sub ExecuteProc(cmd As SqlCommand, ReadDataCallback As Action(Of Csla.Data.SafeDataReader))
      Using cn As New SqlConnection(Singular.Settings.ConnectionString)
        cn.Open()
        Try
          cmd.Connection = cn
          cmd.CommandType = CommandType.StoredProcedure

          Using sdr As New Csla.Data.SafeDataReader(cmd.ExecuteReader)
            ReadDataCallback(sdr)
          End Using
        Finally
          cn.Close()
        End Try
      End Using
    End Sub

#End If

  End Module

End Namespace