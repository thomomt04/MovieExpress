' Generated 25 Jan 2013 12:51 - Singular Systems Object Generator Version 2.1.661
Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT = False Then
Imports System.Data.SqlClient
#End If

Namespace SystemSettings.Objects

  <Serializable()> _
  Public Class SystemSettingValueList
    Inherits SingularBusinessListBase(Of SystemSettingValueList, SystemSettingValue)

#Region "  Business Methods  "

    Public Function Find(PropertyName As String) As SystemSettingValue

      For Each child As SystemSettingValue In Me
        If child.PropertyName = PropertyName Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Function GetItem(SystemSettingValueID As Integer) As SystemSettingValue

      For Each child As SystemSettingValue In Me
        If child.SystemSettingValueID = SystemSettingValueID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "System Setting Values"

    End Function

#End Region

#Region "  Data Access  "

#Region "  Common  "

    Public Shared Function NewSystemSettingValueList() As SystemSettingValueList

      Return New SystemSettingValueList()

    End Function

    Public Sub New()

      ' must have parameter-less constructor

    End Sub

#End Region

#Region "  Silverlight  "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region "  .Net Data Access  "

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As SystemSettingValue In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As SystemSettingValue In Me
          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

#End If

#End Region

#End Region

  End Class

End Namespace