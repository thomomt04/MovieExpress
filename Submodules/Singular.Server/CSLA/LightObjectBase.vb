Imports System.Reflection
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class LightListBase(Of ObjectType As LightObjectBase)
  Inherits List(Of ObjectType)

  Protected Function FetchData(ProcName As String, ParamNames As String(), ParamValues As Object()) As LightListBase(Of ObjectType)

    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.ExecuteReaderLocal(AddressOf ReadData)
    Return Me

  End Function

  Protected Overridable Sub ReadData(sdr As Csla.Data.SafeDataReader)

    Singular.Data.DataFunctions.PopulateList(Of ObjectType)(Me, sdr)

  End Sub

End Class

Public Class LightObjectBase(Of ThisType As LightObjectBase(Of ThisType))

  Public Shared Function CreateAndFetch(ProcName As String, ParamNames As String(), ParamValues As Object()) As ThisType
    Dim Instance = Activator.CreateInstance(Of ThisType)()
    Instance.FetchData(ProcName, ParamNames, ParamValues)
    Return Instance
  End Function

  Protected Sub FetchData(ProcName As String, ParamNames As String(), ParamValues As Object())

    Dim cProc As New Singular.CommandProc(ProcName, ParamNames, ParamValues)
    cProc.ExecuteReaderLocal(AddressOf ReadData)

  End Sub

  Protected Overridable Sub ReadData(sdr As Csla.Data.SafeDataReader)

    If sdr.Read Then
      Populate(sdr)
    End If

  End Sub

  Protected Friend Overridable Sub Populate(sdr As Csla.Data.SafeDataReader)

    Singular.Data.DataFunctions.PopulateObject(Me, sdr)

  End Sub

#Region " Validation "

  Private _ErrorString As String

  Public Function IsValid() As Boolean
    Return String.IsNullOrEmpty(_ErrorString)
  End Function

  Public Function ErrorString() As String
    Return _ErrorString
  End Function

  Protected Overridable Sub Validate()

    For Each Prop In Me.GetType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
      Dim Req = Attribute.GetCustomAttribute(Prop, GetType(RequiredAttribute))
      If Req IsNot Nothing Then
        Dim Value = Prop.GetValue(Me, Nothing)
        If Value Is Nothing OrElse (TypeOf Value Is String AndAlso String.IsNullOrWhiteSpace(Value)) Then
          AddError(Prop.Name & " is required")
        End If
      End If

      Dim Sl As StringLengthAttribute = Attribute.GetCustomAttribute(Prop, GetType(StringLengthAttribute))
      If Sl IsNot Nothing Then
        Dim Value As String = Prop.GetValue(Me, Nothing)

        If Value IsNot Nothing AndAlso Value.Length > Sl.MaximumLength Then
          AddError(Prop.Name & " must be less than " & Sl.MaximumLength & " characters in length")
        End If
      End If

    Next

  End Sub

  Protected Sub AddError(Description As String)
    If String.IsNullOrEmpty(_ErrorString) Then
      _ErrorString = Description
    Else
      _ErrorString &= vbCrLf & Description
    End If
  End Sub

  Public Function CheckRules() As Boolean
    _ErrorString = String.Empty
    Validate()
    Return IsValid()
  End Function

#End Region

End Class

Public Class LightObjectBase
  Inherits LightObjectBase(Of LightObjectBase)

End Class
