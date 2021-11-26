Imports Singular.Web.Controls.HelperControls
Imports System.Reflection
Imports Singular.Web.Controls

Namespace CustomControls

  Public Class AddOnTheFly(Of ObjectType, ChildControlObjectType)
    Inherits HelperBase(Of ObjectType, ChildControlObjectType)

    Private mName As String
    Private mCallback As Action(Of HelperAccessors(Of ChildControlObjectType))
    Private mShowCancel As Boolean

    Public Sub New(CallBack As Action(Of HelperAccessors(Of ChildControlObjectType)), Optional ShowCancel As Boolean = True)
      mCallback = CallBack
      mShowCancel = ShowCancel
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      mName = "AOTF_" & GetType(ChildControlObjectType).FullName.Replace(".", "")
      Attributes("id") = mName

      AddClass("AOTFPanel")

      With Helpers.With(Of ChildControlObjectType)("$root." & mName)
        mCallback(.Helpers)

        With .Helpers.Div()
          .AddClass("FormButtons")

          With .Helpers.Button("Save", ButtonMainStyle.Success, ButtonSize.Small, FontAwesomeIcon.save)
            .AddBinding(Singular.Web.KnockoutBindingString.click, "$root." & mName & ".AOTFManager.SaveCurrent()")
            .AddClass("Main")

          End With
          If mShowCancel Then
            With .Helpers.Button("Cancel", ButtonMainStyle.Danger, ButtonSize.Small, FontAwesomeIcon.close)
              .AddBinding(Singular.Web.KnockoutBindingString.click, "$root." & mName & ".AOTFManager.Close()")

            End With
          End If

          Dim Loadbar As New Singular.Web.CustomControls.LoadingOverlay(Of ChildControlObjectType)()
          .Helpers.Control(Loadbar)
          Loadbar.AddBinding(Singular.Web.KnockoutBindingString.visible, "$root." & mName & ".AOTFManager.IsBusy")
        End With

      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      WriteFullStartTag("div", TagType.IndentChildren)

      RenderChildren()

      Writer.WriteEndTag("div", True)
    End Sub

  End Class

End Namespace
