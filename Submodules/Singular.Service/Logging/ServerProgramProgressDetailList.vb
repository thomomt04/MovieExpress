Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

<Serializable()> _
Public Class ServerProgramProgressDetailList
  Inherits SingularBusinessListBase(Of ServerProgramProgressDetailList, ServerProgramProgressDetail)

#Region " Parent "

  <NotUndoable()> Private mParent As ServerProgramProgress
#End Region

#Region " Business Methods "

  Public Function GetItem(ServerProgramProgressDetailID As Integer) As ServerProgramProgressDetail

    For Each child As ServerProgramProgressDetail In Me
      If child.ServerProgramProgressDetailID = ServerProgramProgressDetailID Then
        Return child
      End If
    Next
    Return Nothing

  End Function

  Public Overrides Function ToString() As String

    Return "Server Program Progress Details"

  End Function

#End Region

#Region " Data Access "

#Region " Common "

  Public Shared Function NewServerProgramProgressDetailList() As ServerProgramProgressDetailList

    Return New ServerProgramProgressDetailList()

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
      Me.Add(ServerProgramProgressDetail.GetServerProgramProgressDetail(sdr))
    End While
    Me.RaiseListChangedEvents = True

  End Sub

  Friend Sub Update()

    Me.RaiseListChangedEvents = False
    Try
      ' Loop through each deleted child object and call its Update() method
      For Each Child As ServerProgramProgressDetail In deletedList
        Child.DeleteSelf()
      Next

      ' Then clear the list of deleted objects because they are truly gone now.
      deletedList.Clear()

      ' Loop through each non-deleted child object and call its Update() method
      For Each Child As ServerProgramProgressDetail In Me
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
