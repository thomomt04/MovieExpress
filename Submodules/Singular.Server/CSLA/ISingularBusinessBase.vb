Imports System.ComponentModel

Public Interface ISingularBusinessBase
  Inherits ISingularBase
  Inherits IChildSavable

  Function CanEditField(ByVal FieldName As String, ByVal CanEditFieldArgs As Singular.CanEditFieldArgs) As Boolean

  Function GetEditLevel() As Integer
  Sub MarkOld()
  Sub MarkDirty()
  Sub MarkClean()
#If SILVERLIGHT = False Then
  Sub InsertUpdateGeneric()
#End If

  ReadOnly Property TableName() As String

  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False), DefaultValue(True)>
  ReadOnly Property IsNew As Boolean

  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
  ReadOnly Property IsChild As Boolean

  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
  ReadOnly Property IsValid As Boolean

  <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
  ReadOnly Property IsSelfDirty As Boolean

  Sub CheckRules()

#If Silverlight = False Then

  Sub SetKey(Value As Object)
  Function CanDelete() As CanDeleteArgs

#End If

End Interface

Public Enum BulkUpdateMethod
  AllRecords = 1
  InsertOnly = 2
  UpdateOnly = 3
End Enum
