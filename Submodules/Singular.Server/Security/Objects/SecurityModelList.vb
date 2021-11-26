Imports Csla
Imports Csla.Serialization
Imports Csla.Data
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
#If SILVERLIGHT Then
#Else
Imports System.Data.SqlClient
#End If

Namespace Security

  <Serializable()> _
  Public Class SecurityModelList
    Inherits SecurityModelListBase(Of SecurityModelList, 
                                   SecurityModel, 
                                   SecurityGroupList, 
                                   SecurityGroup)

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewSecurityModelList() As SecurityModelList

      Return New SecurityModelList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

    Public Shared Sub BeginGetSecurityModelList(ByVal CallBack As EventHandler(Of DataPortalResult(Of SecurityModelList)))

      Dim dp As New DataPortal(Of SecurityModelList)
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

    Public Shared Function GetSecurityModelList() As SecurityModelList

      Return DataPortal.Fetch(Of SecurityModelList)(New Criteria)

    End Function

    Protected Sub New()

      ' require use of factory methods

    End Sub

    Friend Sub Update()

      Me.RaiseListChangedEvents = False
      Try

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As SecurityModel In Me
          Child.Update()
        Next
      Finally
        Me.RaiseListChangedEvents = True
      End Try

    End Sub

    Protected Overrides Sub DataPortal_Update()

      UpdateTransactional(AddressOf Update)

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Me.Add(New SecurityModel)

    End Sub

#End If

#End Region

#End Region

  End Class


End Namespace