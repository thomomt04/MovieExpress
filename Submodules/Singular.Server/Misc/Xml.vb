Imports System.Linq
Imports System.Xml.Linq

Public Class Xml

  Public Shared Function ConvertMobileListToXml(Of T)(ByVal MobileList As Csla.Core.MobileList(Of T), ByVal NodeName As String, ByVal FieldName As String) As String

    Return (New XElement(NodeName, MobileList.[Select](Function(i) New XElement(FieldName, i)))).ToString()

  End Function

  Public Shared Function ConvertMobileListToXml(Of T)(ByVal MobileList As Csla.Core.MobileList(Of T)) As String

    Return ConvertMobileListToXml(Of T)(MobileList, "Node", "Value")

  End Function

  Public Shared Function ConvertListToXML(Of T)(List As List(Of T)) As String
    Return ConvertArrayToXml(List.ToArray, "IDs", "ID")
  End Function

  Public Shared Function ConvertListToXML(Of T)(List As List(Of T), ByVal NodeName As String, ByVal FieldName As String, Optional AttributeName As String = "") As String
    Return ConvertArrayToXml(List.ToArray, NodeName, FieldName, AttributeName)
  End Function

  Public Shared Function ConvertListToXML(Of T)(List As List(Of T), Properties As List(Of String), Optional ThrowExceptionIfPropertyAbsent As Boolean = True) As String

    Dim type As Type = GetType(T)

    Dim i As Integer = 0
    'Check if all properties are there
    'If Not ThrowExceptionIfPropertyAbsent remove from the list
    For i = Properties.Count - 1 To 0 Step -1
      Dim pi = type.GetProperty(Properties(i))
      If pi Is Nothing Then
        If ThrowExceptionIfPropertyAbsent Then
          Throw New Exception("Property " & Properties(i) & " not found on type " & type.Name)
        Else
          Properties.RemoveAt(i)
        End If
      End If
    Next

    Return New XElement(type.Name & "List", List.Select(Function(c)
                                                          Dim xAttList As New List(Of XAttribute)
                                                          For Each p In Properties
                                                            Dim pi = type.GetProperty(p)
                                                            xAttList.Add(New XAttribute(p, pi.GetValue(c, Nothing)))
                                                          Next
                                                          Return New XElement(type.Name, xAttList)
                                                        End Function).ToList()).ToString()

  End Function

  Public Shared Function ConvertArrayToXml(Of T)(ByVal Array() As T, ByVal NodeName As String, ByVal FieldName As String, Optional AttributeName As String = "") As String

    If AttributeName = "" Then
      Return (New XElement(NodeName, Array.[Select](Function(i) New XElement(FieldName, i)))).ToString()
    Else
      Return (New XElement(NodeName, Array.[Select](Function(i) New XElement(FieldName, New XAttribute(AttributeName, i))))).ToString()
    End If


  End Function

  Public Shared Function ConvertArrayToXml(Of T)(ByVal Array() As T) As String

    Return ConvertArrayToXml(Of T)(Array, "Node", "Value")

  End Function

  Public Shared Function ConvertXmlToList(ByVal Xml As String) As List(Of String)

    Dim LinqXml = System.Xml.Linq.XElement.Load(New IO.StringReader(Xml))

    Return LinqXml.Nodes.Select(Function(c) CType(c, XElement).Value).ToList
  End Function

  Public Shared Function CreateXML(PopulateBody As Action(Of XMLHelper)) As String

    ' How to write stored proc code:

    ' SELECT  [Rows].[Row].value('(@NoteTypeID)[1]', 'Int') As NoteTypeID,
    '         [Rows].[Row].value('(@CalcDate)[1]', 'Date') As CalcDate
    ' FROM @Data.nodes('//Rows/Row') AS [Rows]([Row]) 


    Dim XmlHelper = New XMLHelper
    PopulateBody(XmlHelper)
    Return XmlHelper.EndXml

  End Function

  Public Class XMLHelper

    Private _sb As Text.StringBuilder
    Private _xmlW As System.Xml.XmlWriter

    Public Sub New()
      _sb = New Text.StringBuilder

      _xmlW = System.Xml.XmlWriter.Create(_sb, New System.Xml.XmlWriterSettings With {.Encoding = System.Text.Encoding.UTF8})
      _xmlW.WriteStartDocument()
      _xmlW.WriteStartElement("Rows")

    End Sub

    Public Sub AddRow(PropertyNames As String(), PropertyValues() As Object)
      _xmlW.WriteStartElement("Row")
      For i As Integer = 0 To PropertyNames.Length - 1
        Dim Value = PropertyValues(i)

        If Not Singular.Misc.IsNullNothing(Value) Then
          _xmlW.WriteStartAttribute(PropertyNames(i).Replace("@", ""))
          If TypeOf Value Is Date Then
            _xmlW.WriteValue(CDate(Value).ToString("yyyyMMdd HH:mm:ss")) 'By default xml writer adds the timezone info. The default toString of Date doesn't.
          Else
            _xmlW.WriteValue(Value)
          End If
          _xmlW.WriteEndAttribute()
        End If

      Next
      _xmlW.WriteEndElement()
    End Sub

    Public Function EndXml() As String

      If _xmlW IsNot Nothing Then
        _xmlW.WriteEndElement()
        _xmlW.WriteEndDocument()
        _xmlW.Flush()
        _xmlW = Nothing
      End If

      Return _sb.ToString

    End Function

  End Class

End Class
