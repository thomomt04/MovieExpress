Imports System.Runtime.CompilerServices
Imports System.Reflection

Public Module CSLAExtensions

  '<Extension()>
  'Public Function GetChildProperties(value As Csla.Core.FieldManager.FieldDataManager) As List(Of Csla.Core.FieldManager.IFieldData)

  '  For Each f In value.FIelds


  'End Function

#If SILVERLIGHT = False Then

  '<Extension()>
  'Public Sub ForEachVisibleProperty(Type As Type, ContextList As UIContextList, Action As Action(Of PropertyInfo), Optional IncludeIDProperty As Boolean = False, Optional Sort As Boolean = False)

  '  Dim miGVP = Type.GetMethod("GetVisibleProperties", BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy)
  '  If miGVP IsNot Nothing Then
  '    'If the object can tell us the properties it wants to be visible
  '    Dim plType As Type = GetType(Singular.PropertyList(Of ))
  '    plType = plType.MakeGenericType(Type)
  '    Dim AllowedProperties As ICollection = Activator.CreateInstance(plType)
  '    miGVP.Invoke(Nothing, {ContextList, AllowedProperties, IncludeIDProperty})

  '    If AllowedProperties.Count > 0 Then
  '      For Each pi As PropertyInfo In AllowedProperties
  '        Action(pi)
  '      Next
  '      Exit Sub
  '    End If

  '  End If

  '  'Otherwise loop through the properties.
  '  ForEachBrowsableProperty(Type, ContextList, Action, IncludeIDProperty, Sort)

  'End Sub

  <Extension()>
  Public Function Batch(Of T)(source As IEnumerable(Of T), batchSize As Integer) As List(Of List(Of T))

    Return source.Select(Function(x, i) New With {.Index = i, .Value = x}) _
            .GroupBy(Function(x) Math.Floor(x.Index / batchSize)) _
            .Select(Function(x) x.Select(Function(v) v.Value).ToList()) _
            .ToList()

  End Function


  <Extension()>
  Public Function GetXmlIDs(Of C As ISingularBase)(ByVal List As IEnumerable(Of C)) As String

    Return (New XElement("IDs", List.[Select](Function(i) New XElement("ID", i.GetIdValue)))).ToString()

	End Function

  '<Extension()>
  'Public Function GetGenericXmlIDs(Of C As ISingularBase)(ByVal List As IEnumerable(Of C)) As String

  '	Return (New XElement("DataSet", List.[Select](Function(i) New XElement("Table", i.GetIdValue)))).ToString()

  'End Function

  Public Function GetBrowsableProperties(ByVal Type As Type, ByVal ContextList As UIContextList,
                                      Optional ByVal IncludeIDProperty As Boolean = False, Optional ByVal Sort As Boolean = False,
                                      Optional ByVal ForDisplay As Boolean = False, Optional IncludeIsDirty As Boolean = False) As PropertyInfo()

    Dim miGVP = Type.GetMethod("GetVisibleProperties", BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy)
    If miGVP IsNot Nothing Then
      'If the object can tell us the properties it wants to be visible
      Dim plType As Type = GetType(Singular.PropertyList(Of ))
      plType = plType.MakeGenericType(Type)
      Dim AllowedProperties As ICollection = Activator.CreateInstance(plType)
      miGVP.Invoke(Nothing, {ContextList, AllowedProperties, IncludeIDProperty})

      If AllowedProperties.Count > 0 Then
        Dim pis(AllowedProperties.Count - 1) As PropertyInfo
        For i = 0 To pis.Length - 1
          pis(i) = AllowedProperties(i)
        Next
        Return pis
      End If

    End If

    'If the object cant tell us the properties, then figure it out.
    Dim Properties As IEnumerable(Of PropertyInfo) = Type.GetProperties(BindingFlags.Instance Or BindingFlags.Public)

    If Sort Then

      Dim SList As New SortedList(Of String, PropertyInfo)

      For Each pi As PropertyInfo In Properties
        Dim da = Singular.Reflection.GetAttribute(Of System.ComponentModel.DataAnnotations.DisplayAttribute)(pi)
        If da IsNot Nothing AndAlso da.GetOrder.HasValue Then
          SList.Add(Right("00" & da.GetOrder, 3) & pi.Name, pi)
        Else
          SList.Add("000" & pi.Name, pi)
        End If
      Next

      Properties = SList.Values

    End If

    Return Properties.Where(Function(childPi) (Not ForDisplay AndAlso Singular.ReflectionCached.SerlialiseField(childPi, IncludeIDProperty, IncludeIsDirty)) OrElse
                                              (ForDisplay AndAlso Singular.ReflectionCached.AutoGenerateField(childPi))).ToArray()

  End Function

  <Extension()>
  Public Sub ForEachBrowsableProperty(ByVal Type As Type, ByVal ContextList As UIContextList, ByVal Action As Action(Of PropertyInfo),
                                      Optional ByVal IncludeIDProperty As Boolean = False, Optional ByVal Sort As Boolean = False,
                                      Optional ByVal ForDisplay As Boolean = False, Optional IncludeIsDirty As Boolean = False)

    Dim miGVP = Type.GetMethod("GetVisibleProperties", BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy)
    If miGVP IsNot Nothing Then
      'If the object can tell us the properties it wants to be visible
      Dim plType As Type = GetType(Singular.PropertyList(Of ))
      plType = plType.MakeGenericType(Type)
      Dim AllowedProperties As ICollection = Activator.CreateInstance(plType)
      miGVP.Invoke(Nothing, {ContextList, AllowedProperties, IncludeIDProperty})

      If AllowedProperties.Count > 0 Then
        For Each pi As PropertyInfo In AllowedProperties
          Action(pi)
        Next
        Exit Sub
      End If

    End If

    'If the object cant tell us the properties, then figure it out.
    Dim Properties As IEnumerable(Of PropertyInfo) = Type.GetProperties(BindingFlags.Instance Or BindingFlags.Public)

    If Sort Then

      Dim SList As New SortedList(Of String, PropertyInfo)

      For Each pi As PropertyInfo In Properties
        Dim da = Singular.Reflection.GetAttribute(Of System.ComponentModel.DataAnnotations.DisplayAttribute)(pi)
        If da IsNot Nothing AndAlso da.GetOrder.HasValue Then
          SList.Add(Right("00" & da.GetOrder, 3) & pi.Name, pi)
        Else
          SList.Add("000" & pi.Name, pi)
        End If
      Next

      Properties = SList.Values

    End If

    For Each childPi As PropertyInfo In Properties
      If (Not ForDisplay AndAlso Singular.ReflectionCached.SerlialiseField(childPi, IncludeIDProperty, IncludeIsDirty)) OrElse
          (ForDisplay AndAlso Singular.ReflectionCached.AutoGenerateField(childPi)) Then

        Action(childPi)
      End If
    Next

  End Sub

#End If

End Module
