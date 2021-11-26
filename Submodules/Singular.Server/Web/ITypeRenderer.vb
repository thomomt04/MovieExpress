Namespace Web.Data.JS

  Public Interface ITypeRenderer
    ReadOnly Property TypeRenderingMode As RenderTypeOption
    ReadOnly Property RendersData As Boolean

    Sub RenderModel(JW As Singular.Web.Utilities.JavaScriptWriter, Member As Data.JS.ObjectInfo.Member)
    Sub RenderData(Instance As Object, JW As JSonWriter, Member As Data.JS.ObjectInfo.Member, ByRef UseDefaultRenderer As Boolean)
  End Interface

  Public Class ClasslessRenderer
    Implements ITypeRenderer

    Public Sub RenderData(Instance As Object, JW As JSonWriter, Member As ObjectInfo.Member, ByRef UseDefaultRenderer As Boolean) Implements ITypeRenderer.RenderData

    End Sub

    Public Sub RenderModel(JW As Utilities.JavaScriptWriter, Member As ObjectInfo.Member) Implements ITypeRenderer.RenderModel
      JW.Write("CreateROProperty(self, '" & Member.JSonPropertyName & "');", True)
    End Sub

    Public ReadOnly Property RendersData As Boolean Implements ITypeRenderer.RendersData
      Get
        Return False
      End Get
    End Property

    Public ReadOnly Property TypeRenderingMode As RenderTypeOption Implements ITypeRenderer.TypeRenderingMode
      Get
        Return RenderTypeOption.RenderNoTypes
      End Get
    End Property
  End Class

  Public Enum RenderTypeOption
    RenderNoTypes = 1
    RenderChildTypesOnly = 2
    RenderThisTypeAndChildTypes = 3
  End Enum

  Public Class TypeRendererHelper

    Private Shared mTypes As New Hashtable

    Public Shared Sub AddCustomType(Type As Type, Renderer As ITypeRenderer)
      mTypes(Type.FullName) = Renderer
    End Sub

    Public Shared Function GetTypeRendererInstance(Type As Type) As ITypeRenderer
      SyncLock mTypes
        Dim MustCreate As Boolean = Singular.Reflection.TypeImplementsInterface(Type, GetType(Data.JS.ITypeRenderer))
        Dim TR As ITypeRenderer = mTypes(Type.FullName)
        If MustCreate AndAlso TR Is Nothing Then
          TR = Activator.CreateInstance(Type, True)
          mTypes(Type.FullName) = TR
        End If
        Return TR
      End SyncLock
    End Function

  End Class

End Namespace

