Imports System.Web.UI

Namespace Controls

  Public Module Controls


    Public Enum RenderMode
      ''' <summary>
      ''' Specifies that the action should be performed on the server.
      ''' </summary>
      BuildOnServer = 1
      ''' <summary>
      ''' Specifies that the action should be performed on the clients browser.
      ''' </summary>
      BuildOnClient = 2
    End Enum

    Public Enum TableAddButtonLocationType
      RowAtBottom = 1
      InHeader = 2
    End Enum

    'Settings
    Public Property DefaultAddNewButtonIcon As FontAwesomeIcon = FontAwesomeIcon.plusFilled
    Public Property ExpandButtonWidth As Integer = 24
    Public Property EditorRowRowClass As String = "row"

    ''' <summary>
    ''' Adds inputmode=numeric attribute to editors bound to a number. This doesn't seem to work on all mobile browsers and can prevent the user typeing in decimals.
    ''' </summary>
    Public Property ShowMobileNumericKeyboard As Boolean = False

    Public Property AddBootstrapClasses As Boolean = False

    Public Property DefaultDropDownType As Singular.DataAnnotations.DropDownWeb.SelectType = Singular.DataAnnotations.DropDownWeb.SelectType.NormalDropDown

    Private mUsesComboDropDown As Boolean = True

    Public Property UsesComboDropDown As Boolean
      Get
        If DefaultDropDownType = Singular.DataAnnotations.DropDownWeb.SelectType.Combo Then
          Return True
        Else
          Return mUsesComboDropDown
        End If
      End Get
      Set(value As Boolean)
        mUsesComboDropDown = value
      End Set
    End Property

    Public Property DefaultButtonPostBackType As PostBackType = PostBackType.Full
    Public Property DefaultButtonSize As ButtonSize = ButtonSize.Normal

    Private mDefaultButtonStyle As ButtonStyle = ButtonStyle.JQuery
    Public Property DefaultButtonStyle As ButtonStyle
      Get
        Return mDefaultButtonStyle
      End Get
      Set(value As ButtonStyle)
        If mDefaultButtonStyle <> value Then
          mDefaultButtonStyle = value
          If Not AllowFontAwesomeGlyphs AndAlso mDefaultButtonStyle = ButtonStyle.Bootstrap Then
            AllowFontAwesomeGlyphs = True
          End If
        End If
      End Set
    End Property

    Public Property AllowFontAwesomeGlyphs As Boolean = False

    Public Property UsesPagedGrid As Boolean = False
    Public Property DefaultPageControlsType As CustomControls.PageControlsType = CustomControls.PageControlsType.ButtonsWithInput

    Public Property TableAddButtonLocation As TableAddButtonLocationType = TableAddButtonLocationType.RowAtBottom

    ''' <summary>
    ''' Tell the library controls to render using bootstrap classes
    ''' </summary>
    Public Sub UsesBootstrap()
      AddBootstrapClasses = True
      DefaultButtonStyle = ButtonStyle.Bootstrap
      EditorRowRowClass = "control-row form-group"
    End Sub


#Region " Legacy "

    ''' <summary>
    ''' Returns the first occurance of a control matching the specified control type.
    ''' </summary>
    ''' <param name="Control">The Root Control to look in.</param>
    ''' <param name="ControlType">The type of control you want to find.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindControl(Control As Control, ControlType As Type) As Control

      If Control.GetType Is ControlType Then
        Return Control
      Else
        For Each childControl As Control In Control.Controls
          Dim found As Control = FindControl(childControl, ControlType)
          If found IsNot Nothing Then
            Return found
          End If
        Next
      End If
      Return Nothing
    End Function

    ''' <summary>
    ''' Looks at all the controls in a control for the specified name.
    ''' </summary>
    ''' <param name="ID">The ID / Unique ID / Client ID to look for.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindControl(Control As Control, ID As String) As Control

      'Make sure the control isnt recreated.
      If Control.ID Is Nothing Then
        Return Nothing
      End If

      If Control.ID = ID Or Control.ClientID = ID Or Control.UniqueID = ID Then
        Return Control
      Else
        For Each childControl As Control In Control.Controls
          Dim found As Control = FindControl(childControl, ID)
          If found IsNot Nothing Then
            Return found
          End If
        Next
      End If
      Return Nothing
    End Function

#End Region

  End Module

End Namespace


