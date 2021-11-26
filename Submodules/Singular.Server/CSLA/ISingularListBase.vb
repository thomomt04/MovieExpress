Public Interface ISingularListBase
  Inherits System.Collections.ICollection

  Function GetXmlIDs() As String

#If SILVERLIGHT Then

  Function Sort(SortInfo As SingularDataGrid.SortInfo) As ISingularListBase

#Else

  'Event UpdateStatus As EventHandler(Of UpdateStatusEventArgs)
  'Event AskQuestion As EventHandler(Of AskQuestionEventArgs)

#End If

End Interface

#If SILVERLIGHT = False Then

Public Class UpdateStatusEventArgs
  Inherits EventArgs

  Public Enum NotifyTypes
    Normal = 0
    FlashText = 1
    MessageBox = 2
  End Enum

  Public Sub New(ByVal Text As String, ByVal NotifyType As NotifyTypes)
    mText = Text
    mNotifyType = NotifyType
  End Sub

  Private mText As String = ""
  Private mNotifyType As NotifyTypes

  Public ReadOnly Property Text() As String
    Get
      Return mText
    End Get
  End Property

  Public ReadOnly Property NotifyType() As NotifyTypes
    Get
      Return mNotifyType
    End Get
  End Property

End Class

Public Class AskQuestionEventArgs
  Inherits EventArgs

  Private mQuestionType As String
  Private mQuestion As String
  Private mAnswerResults() As Object

  Public Sub New(ByVal QuestionType As String, ByVal Question As String)
    mQuestionType = QuestionType
    mQuestion = Question
  End Sub

  Public ReadOnly Property QuestionType() As String
    Get
      Return mQuestionType
    End Get
  End Property

  Public ReadOnly Property Question() As String
    Get
      Return mQuestion
    End Get
  End Property

  Public Property AnswerResults() As Object()
    Get
      Return mAnswerResults
    End Get
    Set(ByVal value As Object())
      mAnswerResults = value
    End Set
  End Property

End Class

#End If
