Namespace Security

  Public Class UsersControlSingular
    Inherits UsersControl(Of UsersVMSingular, Singular.Security.UserList, Singular.Security.User)

  End Class

  Public Class UsersControl(Of UserVM As UsersVM(Of UserVM, UList, UObj), UList As Singular.Security.UserListBase(Of UList, UObj), UObj As Singular.Security.UserBase(Of UObj))
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of UsersVM(Of UserVM, UList, UObj))

    ''' <summary>
    ''' Custom setup for the users grid.
    ''' </summary>
    Public Property UsersGridSetup As Action(Of Singular.Web.CustomControls.Table(Of UsersVM(Of UserVM, UList, UObj), UObj)) = Nothing

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      Dim h = Helpers

      With h.Toolbar

        .Helpers.HTML.Heading2("User Setup")

        .Helpers.Button(Singular.Web.DefinedButtonType.Save).PostBackType = PostBackType.Ajax
        .Helpers.Button(Singular.Web.DefinedButtonType.Undo).PostBackType = PostBackType.Ajax

      End With

      h.MessageHolder()


      With h.EditorRowFor(Function(c) c.NameFilter)
        .Style.Margin(, , "20px", )

      End With

			Dim UsersTable = h.TableFor(Of UObj)(Function(c) c.UserList, True, True)
      With UsersTable
        .AddClass("Grid")
        .FirstRow.AddBinding(KnockoutBindingString.visible, "(($root.NameFilter() == null) || (FirstName() + ' ' + Surname()).toLowerCase().indexOf($root.NameFilter().toLowerCase()) >= 0)")
        If UsersGridSetup Is Nothing Then
          With .FirstRow.AddColumn(Function(c) c.FirstName)
          End With
          With .FirstRow.AddColumn(Function(c) c.Surname)
          End With
          With .FirstRow.AddColumn(Function(c) c.LoginName)
          End With
          With .FirstRow.AddColumn(Function(c) c.Password)
          End With
          With .AddRow
            .AddBinding(KnockoutBindingString.visible, "(($root.NameFilter() == null) || (FirstName() + ' ' + Surname()).toLowerCase().indexOf($root.NameFilter().toLowerCase()) >= 0)")
						.AddColumn(Function(c) c.EmailAddress).ColSpan = 4
          End With
        Else
          UsersGridSetup.Invoke(UsersTable)
        End If

        With .AddChildTable(Of Singular.Security.SecurityGroupUser)(Function(c) c.SecurityGroupUserList, True, True)
          .FirstRow.AddColumn(Function(c) c.SecurityGroupID).Settings.Width = "300px"
        End With

      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class

End Namespace


