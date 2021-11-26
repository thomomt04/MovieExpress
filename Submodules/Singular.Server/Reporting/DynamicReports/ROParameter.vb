' Generated 09 Dec 2014 14:35 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace Reporting.Dynamic


  <Serializable()> _
  Public Class ROParameter
    Inherits SingularReadOnlyBase(Of ROParameter)

#Region "  Properties and Methods  "

#Region "  Properties  "

    <Display(Name:="Parameter Name", Description:=""), Key>
    Public Property ParameterName As String

    Public Shared DisplayNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DisplayName, "Display Name")
    ''' <summary>
    ''' Gets the Display Name value
    ''' </summary>
    <Display(Name:="Display Name", Description:="")>
    Public ReadOnly Property DisplayName() As String
      Get
        Return GetProperty(DisplayNameProperty)
      End Get
    End Property

    Public Shared DropDownSourceProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.DropDownSource, "Drop Down Source")
    ''' <summary>
    ''' Gets the Drop Down Source value
    ''' </summary>
    <Display(AutoGenerateField:=False)>
    Public ReadOnly Property DropDownSource() As String
      Get
        Return GetProperty(DropDownSourceProperty)
      End Get
    End Property

    Public Shared RequiredIndProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(c) c.RequiredInd, "Required")
    ''' <summary>
    ''' Gets the Required value
    ''' </summary>
    <Display(Name:="Required", Description:="")>
    Public ReadOnly Property RequiredInd() As Boolean
      Get
        Return GetProperty(RequiredIndProperty)
      End Get
    End Property

    Private mMainDataType As Singular.Reflection.SMemberInfo.MainType
    <Browsable(False)>
    Public ReadOnly Property ParamDataType As Singular.Reflection.SMemberInfo.MainType
      Get
        Return mMainDataType
      End Get
    End Property

    <Browsable(False)>
    Public Property Visible As Boolean

    Private mDefaultValue As Object
    Public ReadOnly Property DefaultValue As Object
      Get
        Return mDefaultValue
      End Get
    End Property

    Private mHasUserValue As Boolean = False
    Private mSelectedValue As Object
    Public Property SelectedValue As Object
      Get
        Return mSelectedValue
      End Get
      Set(value As Object)
        If Not Singular.Misc.CompareSafe(value, mSelectedValue) Then
          mSelectedValue = value
          mHasUserValue = True
        End If
      End Set
    End Property

#End Region

#Region "  Methods  "

    Friend Function GetUserValue() As Object
      If Not Visible Then
        Return DefaultValue
      Else
        If mHasUserValue Then
          Return SelectedValue
        Else
          Return DefaultValue
        End If
      End If
    End Function

    Protected Overrides Function GetIdValue() As Object

      Return ParameterName

    End Function

    Public Overrides Function ToString() As String

      Return ParameterName

    End Function

#End Region

#End Region

#Region "  Data Access & Factory Methods  "

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Friend Shared Function NewROParameter(rp As ReportParameter) As ROParameter

      Dim r As New ROParameter()
      r.ParameterName = rp.ParameterName
      r.LoadProperty(DisplayNameProperty, rp.DisplayName)
      r.LoadProperty(DropDownSourceProperty, rp.DropDownSource)
      r.LoadProperty(RequiredIndProperty, rp.RequiredInd)
      r.mMainDataType = rp.MainDataType

      Dim TempDefaultValue As Object = rp.GetDefaultValue
      If TypeOf TempDefaultValue Is String AndAlso TempDefaultValue = "NULL" Then
        If r.mMainDataType = Reflection.SMemberInfo.MainType.String Then
          TempDefaultValue = ""
        Else
          TempDefaultValue = DBNull.Value
        End If
      End If

      r.mDefaultValue = TempDefaultValue
      r.Visible = rp.Visible

      Return r

    End Function

#End If

#End Region

#End Region

  End Class

End Namespace