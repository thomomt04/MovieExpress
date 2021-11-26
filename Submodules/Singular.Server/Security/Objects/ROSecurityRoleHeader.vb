Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Singular.Localisation
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Security

  <Serializable()> _
  Public Class ROSecurityRoleHeader
    Inherits SingularReadOnlyBase(Of ROSecurityRoleHeader)

#Region " Properties and Methods "

#Region " Properties "

    Public Shared SectionNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.SectionName, "ID")
    ''' <Summary>
    ''' Gets the ID value
    ''' </Summary>
    <Display(Name:="ID", Description:=""),
     Singular.DataAnnotations.LocalisedDisplay(GetType(SingularObservableResources), "ID")> _
    Public ReadOnly Property SectionName() As String
      Get
        Return GetProperty(SectionNameProperty)
      End Get
    End Property

    Public Sub SetSectionName(Name As String)
      LoadProperty(SectionNameProperty, Name)
    End Sub


    Public Shared IsExpandedProperty As Csla.PropertyInfo(Of Boolean) = RegisterProperty(Function(c) c.IsExpanded, "Expanded", True) 'DO NOT CHANGE THIS DEFAULT...

    Public Property IsExpanded() As Boolean
      Get
        Return GetProperty(IsExpandedProperty)
      End Get
      Set(ByVal value As Boolean)
        LoadProperty(IsExpandedProperty, value)
      End Set
    End Property

    Public Property SelectedInd As Nullable(Of Boolean)
      Get
        Dim Ret As Nullable(Of Boolean)
        For Each role As ROSecurityRole In ROSecurityRoleList
          If role.SelectedInd Then
            If Ret.HasValue AndAlso Not Ret.Value Then
              Return Nothing
            Else
              Ret = True
            End If
          Else
            If Ret.HasValue AndAlso Ret.Value Then
              Return Nothing
            Else
              Ret = False
            End If
          End If
        Next
        Return Ret
      End Get
      Set(value As Nullable(Of Boolean))
        If Not value.HasValue Then
          value = False
        End If
        mInSelectedSet = True
        For Each role As ROSecurityRole In ROSecurityRoleList
          role.SelectedInd = value
        Next
        mInSelectedSet = False
        OnPropertyChanged("SelectedInd")
      End Set
    End Property

    Private mInSelectedSet As Boolean = False

#End Region

#Region " Child Lists "

    Public Shared ROSecurityRoleListProperty As PropertyInfo(Of ROSecurityRoleList) = RegisterProperty(Of ROSecurityRoleList)(Function(c) c.ROSecurityRoleList, "RO Security Role List")

#If Silverlight = False Then
    <Singular.DataAnnotations.ClientOnly(False)>
    Public ReadOnly Property ROSecurityRoleList() As ROSecurityRoleList
#Else
    Public ReadOnly Property ROSecurityRoleList() As ROSecurityRoleList
#End If
      Get
        If Not FieldManager.FieldExists(ROSecurityRoleListProperty) Then
          Dim srl As ROSecurityRoleList = ROSecurityRoleList.NewROSecurityRoleList()
          AddHandler srl.ChildChanged, AddressOf ChildChanged
          LoadProperty(ROSecurityRoleListProperty, srl)
        End If
        Return GetProperty(ROSecurityRoleListProperty)
      End Get
    End Property

    Private Sub ChildChanged(sender As Object, e As Csla.Core.ChildChangedEventArgs)
      If Not mInSelectedSet Then
        OnPropertyChanged("SelectedInd")
      End If

    End Sub

#End Region

#Region " Methods "

    Protected Overrides Function GetIdValue() As Object

      Return GetProperty(SectionNameProperty)

    End Function

    Public Overrides Function ToString() As String

      Return Me.SectionName

    End Function


#End Region

#End Region

#Region " Data Access & Factory Methods "

#Region " Silverlight "

    Public Sub New()

    End Sub

    Friend Sub New(SectionName As String)
      LoadProperty(SectionNameProperty, SectionName)
    End Sub

#If SILVERLIGHT Then

    Public Shared Function NewROSecurityRoleHeader() As ROSecurityRoleHeader

      Return New ROSecurityRoleHeader()

    End Function

    Friend Shared Function NewROSecurityRoleHeader(SectionName As String) As ROSecurityRoleHeader

      Return New ROSecurityRoleHeader(SectionName)

    End Function

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Friend Shared Function GetROSecurityRoleHeader(ByVal dr As SafeDataReader) As ROSecurityRoleHeader

      Dim r As New ROSecurityRoleHeader()
      r.Fetch(dr)
      Return r

    End Function

    Protected Sub Fetch(ByRef sdr As SafeDataReader)

      With sdr
        LoadProperty(SectionNameProperty, .GetString(0))
        IsExpanded = True
      End With

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace