Imports System.Reflection
Imports System.Dynamic
Imports System.Linq.Expressions
Imports Singular.Web.Controls.HelperControls
Imports System.ComponentModel.DataAnnotations
Imports Singular.Web.Controls

Public Class Message
  Inherits Singular.Message

  Public Sub New(MessageType As MessageType, MessageTitle As String, Message As String)
    Me.MessageType = MessageType
    Me.MessageTitle = MessageTitle
    Me.Message = Message
  End Sub

  Public Sub New()

  End Sub

End Class

Namespace CustomControls

  <Serializable()>
  Public Class MessageList
    Inherits List(Of MessageInfo)

    Public Sub AddMessage(mi As MessageInfo)
      If mi.MessageHolderName = "" Then
        mi.MessageHolderName = MessageInfo.MainHolderName
      End If
      Dim existing As MessageInfo = Me.Where(Function(C) C.MessageName = mi.MessageName AndAlso C.MessageHolderName = mi.MessageName).FirstOrDefault
      If existing Is Nothing Then
        Me.Add(mi)
      Else
        existing.MessageType = mi.MessageType
        existing.Message = mi.Message
        existing.MessageTitle = mi.MessageTitle
      End If
    End Sub

    Public Function AddMessage(MessageHolderName As String, MessageName As String, MessageType As MessageType, MessageTitle As String, Message As String, Optional FadeTime As Integer = 0) As MessageInfo
      Dim mi As New MessageInfo
      mi.MessageType = MessageType
      mi.MessageHolderName = MessageHolderName
      mi.MessageType = MessageType
      mi.MessageTitle = MessageTitle
      mi.Message = Message
      mi.FadeAfter = FadeTime
      AddMessage(mi)
      Return mi
    End Function

  End Class

  <Serializable()>
  Public Class MessageInfo
    Inherits Message

    Public Property MessageHolderName As String
    Public Property MessageName As String
    Public Const MainHolderName As String = "Main"

  End Class

  ''' <summary>
  ''' Renders a message holder and the messages it contains.
  ''' </summary>
  <Serializable()>
  Public Class MessageHolderControl(Of ObjectType)
    Inherits HelperBase(Of ObjectType)

    Private mMessageInfoList As New MessageList
    Public Property Name As String = MessageInfo.MainHolderName
    Public Property ClientOnly As Boolean = False

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      'If this affects you, tell Marlborough. It causes more irritation than help though.
      'If Name = MessageInfo.MainHolderName Then
      '  AddMessage(MessageType.Validation, "ValidationMain", "", "")
      'End If

      Attributes("id") = "MsgControl" & Name

      If Name <> MessageInfo.MainHolderName AndAlso (Page Is Nothing OrElse (Model IsNot Nothing AndAlso CType(Me.Model, Singular.Web.IViewModel).IsStateless)) Then
        ClientOnly = True
      End If
    End Sub

    Public Function AddMessage(MessageType As MessageType, MessageTitle As String, Message As String) As MessageControl

      Return AddMessage(MessageType, "", MessageTitle, Message)

    End Function

    Public Function AddMessage(MessageType As MessageType, MessageName As String, MessageTitle As String, Message As String) As MessageControl

      Dim mc As New MessageControl()
      Dim mi As MessageInfo = mMessageInfoList.AddMessage(Name, MessageName, MessageType, MessageTitle, Message)
      mc.MessageInfo = mi
      'Controls.Add(mc)
      Return Helpers.SetupControl(mc)
      'mc.Setup()
      'Return mc

    End Function

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      AddClass("Msg")
      If ID <> "" Then
        Attributes("id") = ID
      End If
      If ClientOnly Then
        Attributes("data-ClientOnly") = True
      End If

      WriteFullStartTag("div", TagType.IndentChildren)

      For Each mc As MessageControl In Controls
        mc.Render()
      Next

      Writer.WriteEndTag("div", True)

    End Sub

    Public ReadOnly Property MessageInfoList As MessageList
      Get
        Return mMessageInfoList
      End Get
    End Property

    Public Class MessageControl
      Inherits HelperBase(Of ObjectType)

      Public Property MessageInfo As MessageInfo

      Protected Friend Overrides Sub Render()
        MyBase.Render()

        AddClass("Msg-" & MessageInfo.MessageType.ToString)
        If MessageInfo.MessageType = MessageType.Validation Then
          If CType(Parent, MessageHolderControl(Of ObjectType)).Name = MessageInfo.MainHolderName Then
            Attributes("data-validation-summary") = "1"
          End If
        End If

        If MessageInfo.Message = "" AndAlso MessageInfo.MessageTitle = "" Then
          Style("display") = "none"
        End If

        WriteFullStartTag("div", TagType.IndentChildren)

        If MessageInfo.MessageTitle <> "" Then
          Writer.WriteFullBeginTag("strong")
          Writer.Write(MessageInfo.MessageTitle)
          Writer.WriteEndTag("strong")
          Writer.Write("<br />")
        End If
        Writer.WriteLine(MessageInfo.Message)
        Writer.WriteEndTag("div", True)

      End Sub

    End Class

  End Class

  Public Class SimpleMessage(Of ObjectType)
    Inherits Controls.HelperControls.HelperBase(Of ObjectType)

    Private _MessageType As MessageType
    Private _TitleText As String
    Private _MessageText As String

    Public Property Title As HTMLTag(Of ObjectType)
    Public Property Message As HTMLTag(Of ObjectType)
    Public Property MessageContainer As HTMLDiv(Of ObjectType)

    Public Sub New(MessageType As MessageType, Title As String, Message As String)
      _MessageType = MessageType
      _TitleText = Title
      _MessageText = Message
    End Sub

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      MessageContainer = Helpers.DivC("Msg-" & _MessageType.ToString)

      If Not String.IsNullOrEmpty(_TitleText) Then
        Title = MessageContainer.Helpers.HTMLTag("strong", _TitleText)
        Title.Style.Display = Display.block
      End If
      Message = MessageContainer.Helpers.Span(_MessageText)

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      AddClass("Msg")
      Attributes("data-ClientOnly") = True

      WriteFullStartTag("div", TagType.IndentChildren)

      RenderChildren()

      Writer.WriteEndTag("div", True)

    End Sub

  End Class

End Namespace

