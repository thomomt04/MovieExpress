Namespace Reporting

  '<Serializable()>
  Public MustInherit Class ReportCriteria

#If SILVERLIGHT Then
#Else
    ''' <summary>
    ''' ToString Override for Web
    ''' </summary>
    Public Shared ToStringExpression As Csla.Core.IPropertyInfo = RegisterSProperty(Of String, ReportCriteria)(Function(c) c.ToString) _
                                                                    .GetExpression(Function(c) "Report Criteria")

    Protected Friend Overridable Function AddParameters(CProc As Singular.CommandProc) As DataTable

      Dim InfoTable = New DataTable
      InfoTable.TableName = "Information"
      InfoTable.ExtendedProperties.Add(Data.DataTables.ExtendedProperties.ReportCriteria.ToString, True)

      'Add the parameters to the command.
      For Each pi As System.Reflection.PropertyInfo In Me.GetType.GetProperties(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
        If Singular.Reflection.CanSerialiseField(pi) AndAlso Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ClientOnly)(pi) Is Nothing Then
          Dim ParamValue As Object = pi.GetValue(Me, Nothing)
          If GetType(IList).IsAssignableFrom(pi.PropertyType) Then
            If CType(ParamValue, IList).Count = 0 Then
              ParamValue = DBNull.Value
            Else
              If ListToXML_UseOldMethod Then
                ParamValue = Singular.Xml.ConvertListToXML(ParamValue, "DataSet", "Table", "ID")
              Else
                ParamValue = Singular.Xml.ConvertListToXML(ParamValue)
              End If

            End If

          End If

          CProc.Parameters.AddWithValue("@" & pi.Name, ParamValue)

          'Check if the property has a drop down attribute on it.
          Dim VisibleColumn As DataColumn = AddColumnToInfoTable(InfoTable, pi.Name, pi.PropertyType, ParamValue)

          Dim ddaWeb = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DropDownWeb)(pi)
          If ddaWeb IsNot Nothing Then
            If ddaWeb.LookupMember = "" Then
              VisibleColumn.ExtendedProperties(Singular.Data.DataTables.ExtendedProperties.AutoGenerate.ToString) = False
              VisibleColumn = AddColumnToInfoTable(InfoTable, pi.Name & "_Text", GetType(String), ddaWeb.GetDisplayFromID(ParamValue, Me))
            End If

          End If
          VisibleColumn.Caption = Singular.Reflection.GetDisplayName(pi)

          Singular.Data.DataTables.ApplyExtendedInfo(VisibleColumn, pi)
        End If
      Next

      ProcessInfoTable(InfoTable)

      If InfoTable.Columns.Count > 0 Then
        InfoTable.Rows.Add(InfoTable.NewRow())
      End If

      Return InfoTable

    End Function

    Protected Overridable Sub ProcessInfoTable(InfoTable As DataTable)

    End Sub

    Protected Function AddColumnToInfoTable(InfoTable As DataTable, PropertyName As String, PropertyType As Type, Value As Object) As DataColumn
      Dim col As New DataColumn(PropertyName, If(PropertyType Is GetType(Object) OrElse PropertyType.IsGenericType, GetType(String), PropertyType))
      col.DefaultValue = Value
      InfoTable.Columns.Add(col)
      Return col
    End Function

    Private mReportParent As IReport
    Protected Friend Property ReportParent As IReport
      Get
        Return mReportParent
      End Get
      Friend Set(value As IReport)
        mReportParent = value
      End Set
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Overridable ReadOnly Property ParameterList() As Dynamic.ROParameterList
      Get
        Return Nothing
      End Get
    End Property

    Public Sub SetCriteriaValues(QueryString As Specialized.NameValueCollection)
      For Each prop In Me.GetType().GetProperties
        If QueryString(prop.Name) IsNot Nothing Then
          Dim Value = Singular.Reflection.ConvertValueToType(prop.PropertyType, QueryString(prop.Name))
          prop.SetValue(Me, Value, Nothing)
        End If
      Next
    End Sub

#End If

  End Class

  ' <Serializable()>
  Public Class StartAndEndDateReportCriteria
    Inherits ReportCriteria

    <Singular.DataAnnotations.DateField(AlwaysShow:=True, AutoChange:=Singular.DataAnnotations.AutoChangeType.StartOfMonth, MaxDateProperty:="EndDate")>
    Public Property StartDate As Date = Singular.Dates.DateMonthStart(Now.Date)

    <Singular.DataAnnotations.DateField(AlwaysShow:=True, AutoChange:=Singular.DataAnnotations.AutoChangeType.EndOfMonth, MinDateProperty:="StartDate")>
    Public Property EndDate As Date = Singular.Dates.DateMonthEnd(Now.Date)

  End Class

  '<Serializable()>
  Public Class EndDateReportCriteria
    Inherits ReportCriteria

    <Singular.DataAnnotations.DateField(AlwaysShow:=True, AutoChange:=Singular.DataAnnotations.AutoChangeType.EndOfMonth)>
    Public Property EndDate As Date = Singular.Dates.DateMonthEnd(Now.Date)

  End Class

  '<Serializable()>
  Public Class DefaultCriteria
    Inherits ReportCriteria

  End Class

End Namespace


