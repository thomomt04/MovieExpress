Imports Csla
Imports Csla.Serialization
Imports System.Threading

Namespace Service

#If SILVERLIGHT Then
   Public Interface IServerProgramInfo
    Inherits ISingularBusinessBase
#Else
  <Singular.DataAnnotations.ResolveType(GetType(ProgramInfoResolver))>
  Public Interface IServerProgramInfo
    Inherits ISingularBusinessBase
#End If

    ReadOnly Property IsDirty As Boolean
    <System.ComponentModel.DataAnnotations.Display(AutoGenerateField:=False)>
    ReadOnly Property Description() As String

  End Interface

  <Serializable()> _
  Public MustInherit Class ServerProgramInfoBase(Of T As ServerProgramInfoBase(Of T))
    Inherits SingularBusinessBase(Of T)
    Implements IServerProgramInfo

    Public Overrides ReadOnly Property IsDirty As Boolean Implements IServerProgramInfo.IsDirty
      Get
        Return MyBase.IsDirty
      End Get
    End Property

    Public MustOverride ReadOnly Property Description() As String Implements IServerProgramInfo.Description

  End Class

#If SILVERLIGHT = False Then
  Public Class ProgramInfoResolver
    Inherits Singular.DataAnnotations.TypeResolver

    Public Overrides Function GetActualType(Instance As Object, Parent As Object, Serialiser As Web.Data.JS.JSSerialiser) As Type
      Return GetType(Scheduling.Schedule)
    End Function
  End Class
#End If

  <Serializable>
  Public Class ServiceMenuInfo
    Public Property Text As String
    Public Property PropertyName As String
  End Class


End Namespace
