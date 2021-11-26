Public Interface ISavable

  Function ErrorsAsString() As String
  Function ErrorsAsHTMLString() As String

  Sub FlattenEditLevels()

End Interface

Public Interface IParentSavable
  Inherits ISavable
  Inherits System.Collections.IList

  Function TrySave() As SaveHelper

End Interface

Public Interface IChildSavable
  Inherits ISavable

  Function TrySave(ContainerListType As Type) As SaveHelper

End Interface

Public Class SaveHelper

  Public Property Success As Boolean
  Public Property SavedObject As ISavable
  Public Property Result As ResultType
  Public Property ErrorText As String = ""

  <System.ComponentModel.Browsable(False)>
  Public Property [Error] As Exception

  Public Enum ResultType
    SavedToDatabase = 1
    NoChangesMade = 2
    ObjectNotValid = 3
    CustomError = 4
    [Error] = 5
  End Enum

  Public Sub New()

  End Sub

  Public Function Save(Obj As ISavable, ContainerListType As Type, IsValid As Boolean, IsDirty As Boolean) As SaveHelper

    Dim ObjList As IParentSavable = Activator.CreateInstance(ContainerListType, True)
    ObjList.Add(Obj)
    DoSave(ObjList, IsValid, IsDirty)

    If SavedObject IsNot Nothing AndAlso CType(SavedObject, IParentSavable).Count > 0 Then
      SavedObject = CType(SavedObject, IParentSavable)(0)
    End If

    Return Me
  End Function

  Public Function Save(Obj As ISavable, IsValid As Boolean, IsDirty As Boolean)
    DoSave(Obj, IsValid, IsDirty)
    Return Me
  End Function

  Protected Overridable Sub DoSave(Obj As ISavable, IsValid As Boolean, IsDirty As Boolean)
    Try

      'Check if its valid
      If IsValid Then
        'Check if its dirty
        If IsDirty Then
          'If its dirty, then try save it.
          SavedObject = CType(Obj, Object).Save()

          Result = ResultType.SavedToDatabase
        Else
          SavedObject = Obj
          Result = ResultType.NoChangesMade
        End If
        Success = True
      Else
        Result = ResultType.ObjectNotValid
        ErrorText = Obj.ErrorsAsString().Replace(vbCrLf, "<br />")
        Success = False
      End If
    Catch ex As Exception
      If Singular.Debug.IsCustomError(ex) Then
        Result = ResultType.CustomError
        ErrorText = Singular.Debug.RecurseExceptionMessage(ex)
        Me.Error = ex
      Else
        Result = ResultType.Error
        ErrorText = ex.ToString
        Me.Error = ex
      End If

      Success = False
    End Try
  End Sub

End Class
