Imports System.Reflection
Imports Csla.Serialization

<Serializable()>
Public Class SingularReadOnlyListBase(Of T As SingularReadOnlyListBase(Of T, C), C As SingularReadOnlyBase(Of C))
#If Silverlight Or WPF Then
  Inherits Csla.ReadOnlyListBase(Of T, C)
#Else
  Inherits Csla.ReadOnlyBindingListBase(Of T, C)
#End If

  Implements ISingularListBase
  Implements ISingularReadOnlyListBase

  Public Function GetXmlIDs() As String Implements ISingularListBase.GetXmlIDs

    Return (New System.Xml.Linq.XElement("IDs", Me.[Select](Function(i) New System.Xml.Linq.XElement("ID", i.GetId)))).ToString()

  End Function

  Public Function FindParent(Of Y)() As Y

    Return FindParent(Of Y)(Me)

  End Function

  Private Function FindParent(Of Y)(ByVal OfObject As Object) As Y

    Dim parent = OfObject.Parent

    If parent IsNot Nothing Then
      If TypeOf parent Is Y Then
        Return parent
      Else
        Return FindParent(Of Y)(parent)
      End If
    Else
      Return Nothing
    End If

  End Function

  ''' <summary>
  ''' Returns the first item in the list. If there is more than 1 item, and exception is thrown.
  ''' </summary>
  Public Function FirstAndOnly() As C
    If Me.Count > 1 Then
      Throw New Exception("List contains more than 1 item")
    ElseIf Me.Count = 1 Then
      Return Me(0)
    Else
      Return Nothing
    End If
  End Function

#Region " Sort Helper "

#If SILVERLIGHT Then

  Public Function Sort(SortInfo As SingularDataGrid.SortInfo) As ISingularListBase Implements ISingularListBase.Sort

    Dim SortedList As SingularReadOnlyListBase(Of T, C) = Activator.CreateInstance(Me.GetType, Nothing)

    For Each Child In Me.OrderBy(Function(c) GetSortItem(c, SortInfo))
      SortedList.Add(Child)
    Next

    Return SortedList

  End Function

  Private mSortProperty As PropertyInfo

  Private Function GetSortItem(Child As C, SortInfo As SingularDataGrid.SortInfo) As Object

    If mSortProperty Is Nothing OrElse mSortProperty.Name <> SortInfo.SortDescription.PropertyName Then
      mSortProperty = Child.GetType.GetProperty(SortInfo.SortDescription.PropertyName)
    End If
    If mSortProperty IsNot Nothing Then
      Dim Value = mSortProperty.GetValue(Child, Nothing)

      If Value IsNot Nothing Then
        If SortInfo.ComboList Is Nothing Then

          Dim LookupProperty As PropertyInfo = Nothing
          Dim DisplayProperty As PropertyInfo = Nothing

          Dim LookupObject = Enumerable.FirstOrDefault(Of Object)(SortInfo.ComboList, Function(lo)

                                                                                        If LookupProperty Is Nothing Then
                                                                                          LookupProperty = lo.GetType.GetProperty(SortInfo.ComboSelectedValuePath)
                                                                                        End If


                                                                                        If LookupProperty IsNot Nothing Then
                                                                                          If Singular.Misc.CompareSafe(LookupProperty.GetValue(lo, Nothing), Value) Then
                                                                                            ' this is the one
                                                                                            Return True
                                                                                          End If
                                                                                        End If

                                                                                        Return False

                                                                                      End Function)

          If LookupObject IsNot Nothing Then
            If DisplayProperty Is Nothing Then
              DisplayProperty = LookupObject.GetType.GetProperty(SortInfo.ComboDisplayMemberPath)
            End If

            If DisplayProperty IsNot Nothing Then
              Return DisplayProperty.GetValue(LookupObject, Nothing)
            End If
          End If

        End If
      End If

      Return Value
    Else
      Return Nothing
    End If


  End Function

#End If

#End Region

  Public Overridable ReadOnly Property PrimaryKeyField As String
    Get
      Return Me.GetType.Name.Replace("RO", "").Replace("List", "")
    End Get
  End Property

  Public Overloads Sub Add(ByVal Child As C)
    Dim OldReadOnly = Me.IsReadOnly
    Me.IsReadOnly = False
    MyBase.Add(Child)
    Me.IsReadOnly = OldReadOnly
  End Sub

  Public Shared Function GetDropDownInfo(ByVal Context As String) As DropDownInfo

    Return New DropDownInfo(GetType(C))

  End Function

#If SILVERLIGHT = False Then

  Public Function GetStream() As IO.Stream
    Dim ee As New Singular.Data.ExcelExporter
    ee.PopulateData(Me, True, True)
    Return ee.GetStream()
  End Function

  Public Shared Function FetchList(ByVal Criteria As Object) As T

    Return Csla.DataPortal.Fetch(Of T)(Criteria)

  End Function

  Protected Overrides Sub DataPortal_OnDataPortalInvokeComplete(e As Csla.DataPortalEventArgs)
    MyBase.DataPortal_OnDataPortalInvokeComplete(e)

    If e.Operation = Csla.DataPortalOperations.Fetch Then Localisation.Data.ObjectVisitor.FetchData(Me)
  End Sub

  ''' <summary>
  ''' Returns a list that only has the child items specified in the match criteria
  ''' </summary>
  ''' <param name="MatchColumn">The Property to Match On</param>
  ''' <param name="MatchValues">A List of values to match</param>
  ''' <returns>T</returns>
  ''' <remarks>B Marlborough 2 June 08</remarks>
  Public Function FilterList(ByVal MatchColumn As String, ByVal ParamArray MatchValues As Object()) As T
    Return FilterListMultipleValues(MatchColumn, MatchValues)
  End Function

  Public Function FilterListMultipleValues(ByVal MatchColumn As String, ByVal MatchValues As Object()) As T

    Dim fList As T = Activator.CreateInstance(Me.GetType(), True)
    fList.ListIsReadOnly = True
    Dim pi As PropertyInfo = GetType(C).GetProperty(MatchColumn, BindingFlags.Public Or BindingFlags.Instance)
    If pi Is Nothing Then
      Throw New Exception("FilterList -> Property: " & MatchColumn & " not found on object type " & GetType(C).Name)
    End If
    For Each child As C In Me
      For Each obj As Object In MatchValues
        If pi.GetValue(child, Nothing) = obj Then
          fList.Add(child)
        End If
      Next
    Next
    fList.ListIsReadOnly = False
    Return fList
  End Function

  ''' <summary>
  ''' Work around (hack) to get Singular.Server to build on Jenkins. Without this Jenkins has compile error "'Set' accessor of property 'IsReadOnly' is not accessible."
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Shadows Property ListIsReadOnly As Boolean
    Get
      Return MyBase.IsReadOnly
    End Get
    Set(value As Boolean)
      MyBase.IsReadOnly = value
    End Set
  End Property

  'Public Event AskQuestion(sender As Object, e As Singular.AskQuestionEventArgs) Implements ISingularListBase.AskQuestion

  'Public Event UpdateStatus(sender As Object, e As Singular.UpdateStatusEventArgs) Implements ISingularListBase.UpdateStatus

  Public Function GetDataset(IncludeChildren As Boolean) As DataSet
    Return Singular.CSLALib.GetDatasetFromBusinessListBase(Me, Not IncludeChildren, Not IncludeChildren)
  End Function

#Region " Index "

  Private mIndex As Hashtable

  Public Sub BuildIndex(Of ReturnType)([Property] As System.Linq.Expressions.Expression(Of Func(Of C, ReturnType)))
    Dim mi = Singular.Reflection.GetMemberSpecific([Property])
    Dim cmi = Singular.ReflectionCached.GetCachedMemberInfo(mi)

    mIndex = New Hashtable
    For Each Item As C In Me
      mIndex.Add(cmi.GetValueFast(Item), Item)
    Next
  End Sub

  Public Sub BuildIndexSafe(Of ReturnType)([Property] As System.Linq.Expressions.Expression(Of Func(Of C, ReturnType)))
    Dim mi = Singular.Reflection.GetMemberSpecific([Property])
    Dim cmi = Singular.ReflectionCached.GetCachedMemberInfo(mi)

    mIndex = New Hashtable
    Dim key As String = ""
    For Each Item As C In Me
      key = cmi.GetValueFast(Item)
      If Not mIndex.ContainsKey(key) Then
        mIndex.Add(key, Item)
      End If
    Next
  End Sub

  Public Function GetItemIndexed(Value As Object) As C
    Return mIndex(Value)
  End Function

#End Region

#End If

End Class
