Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace CorrespondanceTemplates

  <Serializable()> _
  Public Class CorrespondanceTemplateList
    Inherits SingularBusinessListBase(Of CorrespondanceTemplateList, CorrespondanceTemplate)

#Region " Business Methods "

    Public Function GetItem(CorrespondanceTemplateID As Integer) As CorrespondanceTemplate

      For Each child As CorrespondanceTemplate In Me
        If child.CorrespondanceTemplateID = CorrespondanceTemplateID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

    Public Overrides Function ToString() As String

      Return "Correspondance Templates"

    End Function

    Public Function GetCorrespondanceTemplateAttachment(ByVal CorrespondanceTemplateAttachmentID As Integer) As CorrespondanceTemplateAttachment

      Dim obj As CorrespondanceTemplateAttachment = Nothing
      For Each parent As CorrespondanceTemplate In Me
        obj = parent.CorrespondanceTemplateAttachmentList.GetItem(CorrespondanceTemplateAttachmentID)
        If obj IsNot Nothing Then
          Return obj
        End If
      Next
      Return Nothing

    End Function

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewCorrespondanceTemplateList() As CorrespondanceTemplateList

      Return New CorrespondanceTemplateList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Sub BeginGetCorrespondanceTemplateList(ByVal CallBack As EventHandler(Of DataPortalResult(Of CorrespondanceTemplateList)))

      Dim dp As New DataPortal(Of CorrespondanceTemplateList)
      AddHandler dp.FetchCompleted, CallBack
      dp.BeginFetch(New Criteria)

    End Sub

    Public Sub New()

      ' require use of MobileFormatter

    End Sub

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

    Public Shared Function GetCorrespondanceTemplateList() As CorrespondanceTemplateList

      Return DataPortal.Fetch(Of CorrespondanceTemplateList)(New Criteria)

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(CorrespondanceTemplate.GetCorrespondanceTemplate(sdr))
      End While
      Me.RaiseListChangedEvents = True

      Dim parent As CorrespondanceTemplate = Nothing
      If sdr.NextResult Then
        While sdr.Read
          If IsNothing(parent) OrElse parent.CorrespondanceTemplateID <> sdr.GetInt32(1) Then
            parent = Me.GetItem(sdr.GetInt32(1))
          End If
          parent.CorrespondanceTemplateAttachmentList.RaiseListChangedEvents = False
          parent.CorrespondanceTemplateAttachmentList.Add(CorrespondanceTemplateAttachment.GetCorrespondanceTemplateAttachment(sdr))
          parent.CorrespondanceTemplateAttachmentList.RaiseListChangedEvents = True
        End While
      End If


      For Each child As CorrespondanceTemplate In Me
        child.CheckRules()
        For Each CorrespondanceTemplateAttachment As CorrespondanceTemplateAttachment In child.CorrespondanceTemplateAttachmentList
          CorrespondanceTemplateAttachment.CheckRules()
        Next
      Next

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(Settings.ConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getCorrespondanceTemplateList"
            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As CorrespondanceTemplate In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As CorrespondanceTemplate In Me
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

    Protected Overrides Sub DataPortal_Update()

      UpdateTransactional(AddressOf Update)

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace