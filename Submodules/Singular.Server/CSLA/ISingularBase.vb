Imports System.Drawing


Public Interface ISingularBase

  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
  ReadOnly Property FieldManager As Csla.Core.FieldManager.FieldDataManager

  Function GetIdValue() As Object

  Function GetFieldStyle(FieldName As String) As FieldStyle

  Function GetTableName() As String

#If Silverlight = False Then

  Sub SetBackingFieldValue(ByVal CslaPropertyInfo As Csla.Core.IPropertyInfo, ByVal Value As Object, IsLoad As Boolean)
  Function GetBackingFieldValue(CslaPropertyInfo As Csla.Core.IPropertyInfo) As Object

  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
  Property Guid As Guid

  Property LocalisationDataValues As Localisation.Data.DataValueList

#End If

End Interface

Public Class FieldStyle

  Public Property Foreground As Brush

  Public Property Background As Brush

End Class
