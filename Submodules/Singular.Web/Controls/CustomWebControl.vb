Namespace Controls

  Public Class CustomWebControl
    Inherits System.Web.UI.UserControl

    Friend Property ServerBindObject As Object

    Private mWriter As Singular.Web.Controls.HtmlTextWriter
    Friend WriterOverride As Singular.Web.Controls.HtmlTextWriter

    Protected Friend ReadOnly Property Writer As Singular.Web.Controls.HtmlTextWriter
      Get
        If WriterOverride IsNot Nothing Then
          Return WriterOverride
        Else
          If TypeOf Parent Is CustomWebControl Then
            Return DirectCast(Parent, CustomWebControl).Writer
          Else
            If mWriter Is Nothing Then
              mWriter = New Singular.Web.Controls.HtmlTextWriter
              If Singular.Debug.InDebugMode Then
                mWriter.WriteLine("<!-- Start of " & Me.GetType.Name & " -->")
              End If
            End If
            Return mWriter
          End If
        End If
      End Get
    End Property

    Private Property LocalState As Object
      Get
        Return Page.Session("LocalSession" & Me.UniqueID)
      End Get
      Set(ByVal value As Object)
        Page.Session("LocalSession" & Me.UniqueID) = value
      End Set
    End Property

    Private mState As Hashtable
    Protected ReadOnly Property State As Hashtable
      Get
        If mState Is Nothing Then
          If LocalState Is Nothing OrElse CanClearState Then
            LocalState = New Hashtable
          End If
          mState = LocalState
        End If
        Return mState
      End Get
    End Property

    Protected Overridable ReadOnly Property CanClearState As Boolean
      Get
        Return Not Page.IsPostBack
      End Get
    End Property

    Protected Sub WriteControlAttributes()
      For Each att As String In Me.Attributes.Keys
        Writer.WriteAttribute(att, Me.Attributes(att))
      Next
    End Sub

#Region " Styles "

    Public Property Style As New CSSStyle

    Public Class CSSStyle

      Private mStyles As New Hashtable

      Default Public Property Style(StyleName As String) As String
        Get
          Return mStyles(StyleName)
        End Get
        Set(value As String)
          mStyles(StyleName) = value
        End Set
      End Property

      Public ReadOnly Property Count As Integer
        Get
          Return mStyles.Count
        End Get
      End Property

      Public ReadOnly Property Keys As ICollection
        Get
          Return mStyles.Keys
        End Get
      End Property

      Public Property Width As String
        Get
          Return Style("width")
        End Get
        Set(value As String)
          'If the user says width = 200, make it 200px.
          If value.IsNumeric Then
            value = value.Trim & "px"
          End If
          Style("width") = value
        End Set
      End Property

      Public Property Height As String
        Get
          Return Style("height")
        End Get
        Set(value As String)
          If value.IsNumeric Then
            value = value.Trim & "px"
          End If
          Style("height") = value
        End Set
      End Property

      Public Property FontSize As String
        Get
          Return Style("font-size")
        End Get
        Set(value As String)
          If value.IsNumeric Then
            value = value.Trim & "px"
          End If
          Style("font-size") = value
        End Set
      End Property

      Public Property TextAlign As TextAlign
        Get
          Return Style("text-align")
        End Get
        Set(value As TextAlign)
          Style("text-align") = value.ToString
        End Set
      End Property

      Public Property VerticalAlign As VerticalAlign
        Get
          Return Style("vertical-align")
        End Get
        Set(value As VerticalAlign)
          Style("vertical-align") = value.ToString
        End Set
      End Property

      Public Property Display As Display
        Get
          Return Style("display")
        End Get
        Set(value As Display)
          Style("display") = Singular.Reflection.GetEnumDisplayName(value)
        End Set
      End Property

      Public Property BackgroundColour As String
        Get
          Return Style("background-color")
        End Get
        Set(value As String)
          Style("background-color") = value
        End Set
      End Property

      Public Property BackgroundImage As String
        Get
          Return Style("background-image")
        End Get
        Set(value As String)
          Style("background-image") = value
        End Set
      End Property

      Public Property Position As Position
        Get
          Return Style("position")
        End Get
        Set(value As Position)
          Style("position") = value.ToString
        End Set
      End Property

      Public Property Overflow As Overflow
        Get
          Return Style("overflow")
        End Get
        Set(value As Overflow)
          Style("overflow") = value.ToString
        End Set
      End Property

      Public Property OverflowY As Overflow
        Get
          Return Style("overflow-y")
        End Get
        Set(value As Overflow)
          Style("overflow-y") = value.ToString
        End Set
      End Property

      Public Property FontWeight As FontWeight
        Get
          Return Style("font-weight")
        End Get
        Set(value As FontWeight)
          Style("font-weight") = Singular.Reflection.GetEnumDisplayName(value)
        End Set
      End Property

      ''' <summary>
      ''' Adds a Right Margin
      ''' </summary>
      Public Sub MarginRight(Unit As String)
        Style("margin-right") = Unit
      End Sub

      ''' <summary>
      ''' Adds a Left Margin
      ''' </summary>
      Public Sub MarginLeft(ByVal Unit As String)
        Style("margin-left") = Unit
      End Sub

      ''' <summary>
      ''' Adds a margin style
      ''' </summary>
      ''' <remarks></remarks>
      Public Sub Margin(Optional ByVal top As Object = "0", Optional ByVal right As Object = "0", Optional ByVal bottom As Object = "0", Optional ByVal left As Object = "0")
        If TypeOf top Is Integer Then
          MarginI(top, right, bottom, left)
        Else
          Style("margin") = top & " " & right & " " & bottom & " " & left
        End If

      End Sub

      ''' <summary>
      ''' Adds a margin style in pixels (px)
      ''' </summary>
      ''' <remarks></remarks>
      Public Sub MarginI(Optional ByVal top As Integer = 0, Optional ByVal right As Integer = 0, Optional ByVal bottom As Integer = 0, Optional ByVal left As Integer = 0)
        Style("margin") = top & "px " & right & "px " & bottom & "px " & left & "px"
      End Sub

      Public Sub MarginAll(All As String)
        Style("margin") = All
      End Sub

      ''' <summary>
      ''' Adds margins for vertical (tb) and horizontal (lr)
      ''' </summary>
      Public Sub MarginVH(Vertical As Integer, Horizontal As Integer)
        Style("margin") = Vertical & "px " & Horizontal & "px"
      End Sub

      Public Sub Padding(Optional ByVal top As String = "0", Optional ByVal right As String = "0", Optional ByVal bottom As String = "0", Optional ByVal left As String = "0")
        Style("padding") = top & " " & right & " " & bottom & " " & left
      End Sub

      Public Sub PaddingAll(All As String)
        Style("padding") = All
      End Sub

      ''' <summary>
      ''' Adds the 'float:left' style
      ''' </summary>
      Public Sub FloatLeft()
        Me.Style("float") = "left"
        Me.Style("clear") = "none"
      End Sub

      ''' <summary>
      ''' Adds the 'float:right' style
      ''' </summary>
      Public Sub FloatRight()
        Me.Style("float") = "right"
      End Sub

      ''' <summary>
      ''' Adds the 'clear:both' style for use after floating divs.
      ''' </summary>
      ''' <remarks></remarks>
      Public Sub ClearBoth()
        Me.Style("clear") = "both"
      End Sub

    End Class

#End Region

    Public Property Tooltip As String
      Get
        Return Attributes("title")
      End Get
      Set(value As String)
        Attributes("title") = value
      End Set
    End Property

    Protected Shadows Sub RenderChildren()
      For Each ctl As CustomWebControl In Controls

        ctl.Render()
      Next
    End Sub

    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)
      MyBase.Render(writer)
      mWriter = New Singular.Web.Controls.HtmlTextWriter(writer)
      Render()
    End Sub

    Public Overloads Sub Render(Writer As Singular.Web.Controls.HtmlTextWriter)
      mWriter = Writer
      Render()
    End Sub

    Protected mHasRendered As Boolean = False
    Public ReadOnly Property HasRendered As Boolean
      Get
        Return mHasRendered
      End Get
    End Property

    Public Function GetHTMLString() As String

      If Parent IsNot Nothing AndAlso TypeOf Parent Is CustomWebControl Then
        Return CType(Parent, CustomWebControl).GetHTMLString()
      Else
        Render()
        Return Writer.ToString
      End If

    End Function

    ''' <summary>
    ''' Gets the rendered ouput of this control only, and removes it from the parent.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetString() As String
      If Parent IsNot Nothing Then
        Parent.Controls.Remove(Me)
      End If
      Render()
      Return Writer.ToString
    End Function

    Public Overrides Function ToString() As String
      Return GetHTMLString()
    End Function

    Protected Friend Overridable Overloads Sub Render()
      mHasRendered = True
    End Sub

  End Class

End Namespace


