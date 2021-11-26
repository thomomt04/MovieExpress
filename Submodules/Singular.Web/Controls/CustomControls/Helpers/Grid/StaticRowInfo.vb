Imports Singular.Web.Controls.HelperControls
Imports System.Reflection
Imports Singular.Web.Controls
Imports System.Linq

Namespace CustomControls

  Public Interface IStaticRowInfo
    Sub RenderBeginRow()
    Sub RenderEndRow()
    Function GetCell(ColumnIndex As Integer) As HelperBase
  End Interface

  Public Class StaticRowInfo(Of ChildType)
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of ChildType)
    Implements IStaticRowInfo

    Public Sub New()

    End Sub

    Public Function EmptyCell() As HelperBase
      Return Helpers.HTML("")
    End Function

    Public Function GetCell(ColumnIndex As Integer) As HelperBase Implements IStaticRowInfo.GetCell

      If ColumnIndex < Controls.Count Then
        Return Controls(ColumnIndex)
      Else
        Return Nothing
      End If

    End Function

    Friend Overridable Sub RenderBeginRow() Implements IStaticRowInfo.RenderBeginRow
      Writer.WriteBeginTag("tr")
      WriteClass()
      Bindings.WriteKnockoutBindings()
      WriteStyles()
    End Sub

    Friend Overridable Sub RenderEndRow() Implements IStaticRowInfo.RenderEndRow
      Writer.WriteEndTag("tr", True)
    End Sub

  End Class

  Public Class StaticRowsInfo(Of ChildType)
    Inherits StaticRowInfo(Of ChildType)

    Public Property Datasource As String

    Friend Overrides Sub RenderBeginRow()

      Writer.Write(String.Format("<!-- ko foreach: {0} -->", Datasource))
      MyBase.RenderBeginRow()

    End Sub

    Friend Overrides Sub RenderEndRow()

      MyBase.RenderEndRow()
      Writer.Write("<!-- /ko -->")

    End Sub

  End Class

End Namespace
