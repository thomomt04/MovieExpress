Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace SmsSending

  <Serializable()> _
  Public Class SmsRecipientList
    Inherits SingularBusinessListBase(Of SmsRecipientList, SmsRecipient)

#Region " Parent "

    <NotUndoable()> Private mParent As Sms
#End Region

#Region " Business Methods "

    Public Function GetItem(SmsRecipientID As Integer) As SmsRecipient

      For Each child As SmsRecipient In Me
        If child.SmsRecipientID = SmsRecipientID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

#If SILVERLIGHT Then
#Else
    Protected Overrides Function AddNewCore() As Object 'SmsRecipient

      Dim obj As SmsRecipient = SmsRecipient.NewSmsRecipient()
      Me.Add(obj)
      Return obj

    End Function
#End If

    Public Overrides Function ToString() As String

      Return "Sms Recipients"

    End Function

#End Region

#Region " Factory Methods "

    Public Shared Function NewSmsRecipientList(ByRef Parent As Sms) As SmsRecipientList

      Return New SmsRecipientList(Parent)

    End Function

    Public Shared Function GetSmsRecipientList(ByRef Parent As Sms) As SmsRecipientList
#If SILVERLIGHT Then
      Return Nothing
#Else
      Dim list As SmsRecipientList = CType(DataPortal.Fetch(Of SmsRecipientList)(New Criteria(Parent)), SmsRecipientList)
      list.mParent = Parent
      Return list
#End If
    End Function

    Private Sub New(ByRef Parent As Sms)

      mParent = Parent
      AllowNew = True

    End Sub

#End Region

#Region " Data Access "

#Region " Common "

    <Serializable()> _
    Private Class Criteria

      Public Parent As Sms

      Public Sub New(ByVal Parent As Sms)

        Me.Parent = Parent

      End Sub

    End Class

    Public Shared Function NewSmsRecipientList() As SmsRecipientList

      Return New SmsRecipientList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

Public Sub New()

' require use of MobileFormatter

End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(SmsRecipient.GetSmsRecipient(sdr))
      End While
      Me.RaiseListChangedEvents = True

    End Sub

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As SmsRecipient In deletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        deletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As SmsRecipient In Me
          If Child.IsNew Then
            Child.Insert()
          Else
            Child.Update()
          End If
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace