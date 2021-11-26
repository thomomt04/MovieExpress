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
  Public Class SmsList
    Inherits SingularBusinessListBase(Of SmsList, Sms)

#Region " Business Methods "

    Public Sub Send()
      For Each Sms As Sms In Me
        Sms.Send()
      Next
    End Sub

    Public Sub SendAsync(OnComplete As Action)
#If Silverlight = False Then

      For Each Sms As Sms In Me
        Sms.Send(, Sub()

                     'check if every smsrecipient has sent
                     For Each sentsms As Sms In Me
                       For Each smsr As SmsRecipient In sentsms.SmsRecipientList
                         If Not smsr.mHasComplete Then
                           Exit Sub
                         End If
                       Next
                     Next

                     OnComplete()

                   End Sub)
      Next

#End If
    End Sub

    Private mConnectionString As String

    Public Function FailedCount() As Integer

      Dim Count As Integer = 0

      For Each Sms As Sms In Me
        For Each smsr As SmsRecipient In Sms.SmsRecipientList
          If smsr.NotSentError <> "" Then
            Count += 1
          End If
        Next
      Next
      Return Count

    End Function

#If Silverlight = False Then

    Public Property OverrideConnectionString As String

    Protected Overrides Function GetConnectionString() As String
      If String.IsNullOrEmpty(OverrideConnectionString) Then
        Return MyBase.GetConnectionString()
      Else
        Return OverrideConnectionString
      End If
    End Function

#End If

    Public Function GetItem(SmsID As Integer) As Sms

      For Each child As Sms In Me
        If child.SmsID = SmsID Then
          Return child
        End If
      Next
      Return Nothing

    End Function

#If SILVERLIGHT Then
#Else
    'Protected Overrides Function AddNewCore() As Sms

    '  Dim obj As Sms = Sms.NewSms
    '  Me.Add(obj)
    '  Return obj

    'End Function
    Protected Overrides Function AddNewCore() As Object
      Dim obj As Sms = Sms.NewSms
      Me.Add(obj)
      Return obj
    End Function

#End If

    Public Overrides Function ToString() As String

      Return "Smses"

    End Function

    Public Function GetSmsRecipient(ByVal SmsRecipientID As Integer) As SmsRecipient

      Dim obj As SmsRecipient = Nothing
      For Each parent As Sms In Me
        obj = parent.SmsRecipientList.GetItem(SmsRecipientID)
        If obj IsNot Nothing Then
          Return obj
        End If
      Next
      Return Nothing

    End Function

    Public Function GetRecipientCount() As Integer

      Dim Total As Integer = 0

      For Each Sms In Me
        Total += Sms.SmsRecipientList.Count
      Next

      Return Total

    End Function

#End Region

#Region " Factory Methods "

    Public Shared Function NewSmsList(ByVal ConnectionString As String) As SmsList

      Return New SmsList()

    End Function

    Private Sub New(ByVal ConnectionString As String)

      AllowNew = True

    End Sub

#End Region

#Region " Data Access "

    <Serializable()> _
    Public Class Criteria
      Inherits CriteriaBase(Of Criteria)

      Public Property UnsentInd As Boolean? = Nothing
      Public Property SmsID As Integer? = Nothing

      Public Sub New()


      End Sub

    End Class

#Region " Common "

    Public Shared Function NewSmsList() As SmsList

      Return New SmsList()

    End Function

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

Public Shared Sub BeginGetSmsList(ByVal CallBack As EventHandler(Of DataPortalResult(Of SmsList)))

Dim dp As New DataPortal(Of SmsList)
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

    Public Shared Function GetUnsentSmsList() As SmsList
      Return CType(DataPortal.Fetch(Of SmsList)(New Criteria() With {.UnsentInd = True}), SmsList)
    End Function

    Public Shared Function GetSmsList(SmsID As Integer) As SmsList
      Return CType(DataPortal.Fetch(Of SmsList)(New Criteria() With {.SmsID = SmsID}), SmsList)
    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub Fetch(ByVal sdr As SafeDataReader)

      Me.RaiseListChangedEvents = False
      While sdr.Read
        Me.Add(Sms.GetSms(sdr))
      End While
      Me.RaiseListChangedEvents = True

      Dim parent As Sms = Nothing
      If sdr.NextResult Then
        While sdr.Read
          If IsNothing(parent) OrElse parent.SmsID <> sdr.GetInt32(1) Then
            parent = Me.GetItem(sdr.GetInt32(1))
          End If
          parent.SmsRecipientList.RaiseListChangedEvents = False
          parent.SmsRecipientList.Add(SmsRecipient.GetSmsRecipient(sdr))
          parent.SmsRecipientList.RaiseListChangedEvents = True
        End While
      End If


      For Each child As Sms In Me
        child.CheckRules()
        For Each SmsRecipient As SmsRecipient In child.SmsRecipientList
          SmsRecipient.CheckRules()
        Next
      Next

    End Sub

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = criteria
      Using cn As New SqlConnection(GetConnectionString)
        cn.Open()
        Try
          Using cm As SqlCommand = cn.CreateCommand
            cm.CommandType = CommandType.StoredProcedure
            cm.CommandText = "GetProcs.getSmsList"
            cm.Parameters.AddWithValue("@NotSentInd", crit.UnsentInd)
            cm.Parameters.AddWithValue("@SmsID", Singular.Misc.NothingDBNull(crit.SmsID))
            Using sdr As New SafeDataReader(cm.ExecuteReader)
              Fetch(sdr)
            End Using
          End Using
        Finally
          cn.Close()
        End Try
      End Using

    End Sub

    Public Sub Update()

      Me.RaiseListChangedEvents = False
      Try
        ' Loop through each deleted child object and call its Update() method
        For Each Child As Sms In DeletedList
          Child.DeleteSelf()
        Next

        ' Then clear the list of deleted objects because they are truly gone now.
        DeletedList.Clear()

        ' Loop through each non-deleted child object and call its Update() method
        For Each Child As Sms In Me
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