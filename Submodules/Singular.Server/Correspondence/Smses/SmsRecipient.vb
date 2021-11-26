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

	<Serializable()>
	Public Class SmsRecipient
		Inherits SingularBusinessBase(Of SmsRecipient)

#Region " Properties and Methods "

#Region " Properties "

		''' <summary>
		''' Change all smses to send to this number. Usefull for testing.
		''' </summary>
		Public Shared Property OverrideCellNumber As String = ""

		''' <summary>
		''' True if your SMSRecipient table and procs have a MessageID for tracking messages once sent.
		''' </summary>
		Public Shared Property SupportsMessageID As Boolean = False



		Public Shared SmsRecipientIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.SmsRecipientID, "Sms Recipient", 0)
		''' <Summary>
		''' Gets the Sms Recipient value
		''' </Summary>
		<Display(AutoGenerateField:=False, Name:="Sms Recipient", Description:="System generated unique ID")>
		Public ReadOnly Property SmsRecipientID() As Integer
			Get
				Return GetProperty(SmsRecipientIDProperty)
			End Get
		End Property

		Public Shared SmsIDProperty As PropertyInfo(Of Nullable(Of Integer)) = RegisterProperty(Of Nullable(Of Integer))(Function(c) c.SmsID, "Sms", CType(Nothing, Nullable(Of Integer)))
		''' <Summary>
		''' Gets the Sms value
		''' </Summary>
		<Display(Name:="Sms", Description:="Sms to be send")>
		Public ReadOnly Property SmsID() As Nullable(Of Integer)
			Get
				Return GetProperty(SmsIDProperty)
			End Get
		End Property

		Public Shared CellNoProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.CellNo, "Cell No", "")
		''' <Summary>
		''' Gets and sets the Cell No value
		''' </Summary>
		<Display(Name:="Cell No", Description:="Cell No to be sent to"),
		StringLength(15, ErrorMessage:="Cell No cannot be more than 15 characters")>
		Public Property CellNo() As String
			Get
				If OverrideCellNumber <> "" Then
					Return OverrideCellNumber
				Else
					Return GetProperty(CellNoProperty)
				End If
			End Get
			Set(ByVal Value As String)
				SetProperty(CellNoProperty, Value)
			End Set
		End Property

		Public Shared RecipientNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.RecipientName, "Recipient Name", "")
		''' <Summary>
		''' Gets and sets the Recipient Name value
		''' </Summary>
		<Display(Name:="Recipient Name", Description:="Name of recipient to recieve the message (not required"),
		StringLength(30, ErrorMessage:="Recipient Name cannot be more than 30 characters")>
		Public Property RecipientName() As String
			Get
				Return GetProperty(RecipientNameProperty)
			End Get
			Set(ByVal Value As String)
				SetProperty(RecipientNameProperty, Value)
			End Set
		End Property

		Public Shared SentDateProperty As PropertyInfo(Of Nullable(Of DateTime)) = RegisterProperty(Of Nullable(Of DateTime))(Function(c) c.SentDate, "Sent Date")
		''' <Summary>
		''' Gets and sets the Sent Date value
		''' </Summary>
		<Display(Name:="Sent Date", Description:="Date that sms was sent")>
		Public Property SentDate() As Nullable(Of DateTime)
			Get
				Return GetProperty(SentDateProperty)
			End Get
			Set(ByVal Value As Nullable(Of DateTime))
				SetProperty(SentDateProperty, Value)
			End Set
		End Property

		Public Shared NotSentErrorProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.NotSentError, "Not Sent Error", "")
		''' <Summary>
		''' Gets and sets the Not Sent Error value
		''' </Summary>
		<Display(Name:="Not Sent Error", Description:=""),
		StringLength(1024, ErrorMessage:="Not Sent Error cannot be more than 1024 characters")>
		Public Property NotSentError() As String
			Get
				Return GetProperty(NotSentErrorProperty)
			End Get
			Set(ByVal Value As String)
				SetProperty(NotSentErrorProperty, Value)
			End Set
		End Property

		Public Property MessageID As String = ""

#End Region

#Region " Methods "



#If SILVERLIGHT Then
#Else
		Private mBatchID As String = ""
		Private mFrom As String = ""
		Private mOnComplete As Action
		Friend mHasComplete As Boolean = False
		Private Shared mSmsQueue As New Queue
		Private Shared mSendingCount As Integer = 0
#End If


		Public Class SendEvent
			Public Property EventName As String
			Public Property EventTime As Date

			Public Overrides Function ToString() As String
				Return EventName & " " & EventTime.ToString("HH:mm:ss")
			End Function

			Public Sub New(EventName As String)
				Me.EventName = EventName
				Me.EventTime = Now
			End Sub
		End Class

		Public Sub Send(Optional ByVal From As String = "", Optional BatchID As String = "", Optional OnComplete As Action = Nothing)
#If Silverlight = False Then

			mBatchID = BatchID
			mFrom = [From]
			mOnComplete = OnComplete

			If OnComplete Is Nothing Then
				'Send syncronously
				SendInternal(Me)

			Else

				SyncLock mSmsQueue
					mSmsQueue.Enqueue(Me)
				End SyncLock
				CheckSendQueue()

			End If

#End If
		End Sub

		Public Sub SendSober(Optional ByVal From As String = "", Optional BatchID As String = "", Optional OnComplete As Action = Nothing)
#If Silverlight = False Then

			mBatchID = BatchID
			mFrom = [From]
			mOnComplete = OnComplete

			If OnComplete Is Nothing Then
				'Send syncronously
				SendInternalSober(Me)

			Else

				SyncLock mSmsQueue
					mSmsQueue.Enqueue(Me)
				End SyncLock
				CheckSendQueue()

			End If

#End If
		End Sub

		Private Shared Sub CheckSendQueue()
#If Silverlight = False Then

			'Send asyncronously, but only 10 threads at a time.
			SyncLock mSmsQueue
				If mSendingCount >= 10 OrElse mSmsQueue.Count = 0 Then
					Exit Sub
				End If

				mSendingCount += 1
				Dim smsr As SmsRecipient = mSmsQueue.Dequeue

				Dim AsyncMethod As Action(Of SmsRecipient) = AddressOf SendInternal
				AsyncMethod.BeginInvoke(smsr,
					Sub(args)

						smsr.mHasComplete = True
						smsr.mOnComplete()

						SyncLock mSmsQueue
							mSendingCount -= 1
							CheckSendQueue()
						End SyncLock

					End Sub, Nothing)

			End SyncLock

#End If
		End Sub

		Private Shared Sub SendInternal(SmsRecipient As SmsRecipient)
#If Silverlight = False Then

			Try

				Dim Result As SmsResult
				'Check if there is a batch id.
				If SmsRecipient.mBatchID = "" Then
					Result = SmsSender.SendSms(SmsRecipient.CellNo, CType(SmsRecipient.GetParent, Sms).Message)
				Else
					Result = ClickatellSender.QuickSend(SmsRecipient.mBatchID, SmsRecipient.CellNo)
				End If

				If Result.Sent Then
					SmsRecipient.SentDate = Now()
					SmsRecipient.MessageID = Result.MessageID
				Else
					SmsRecipient.NotSentError = Result.ErrorMessage
				End If

			Catch ex As Exception
				SmsRecipient.NotSentError = Singular.Debug.RecurseExceptionMessage(ex)
			End Try

#End If
		End Sub

		Private Shared Sub SendInternalSober(SmsRecipient As SmsRecipient)
#If Silverlight = False Then

			Try

				Dim Result As SmsResult
				'Check if there is a batch id.
				If SmsRecipient.mBatchID = "" Then
					Result = SmsSender.SendSms(SmsRecipient.CellNo, CType(SmsRecipient.GetParent, Sms).Message)
				Else
					Result = ClickatellSender.QuickSend(SmsRecipient.mBatchID, SmsRecipient.CellNo)
				End If

				If Result.Sent Then
					SmsRecipient.SentDate = Now()
				Else
					SmsRecipient.NotSentError = Result.ErrorMessage
				End If

			Catch ex As Exception
				SmsRecipient.NotSentError = Singular.Debug.RecurseExceptionMessage(ex)
			End Try

#End If
		End Sub

		Public Function GetParent() As Sms

			Return CType(CType(Me.Parent, SmsRecipientList).Parent, Sms)

		End Function

		Protected Overrides Function GetIdValue() As Object

			Return GetProperty(SmsRecipientIDProperty)

		End Function

		Public Overrides Function ToString() As String

			If Me.CellNo.Length = 0 Then
				If Me.IsNew Then
					Return "New Sms Recipient"
				Else
					Return "Blank Sms Recipient"
				End If
			Else
				Return Me.CellNo
			End If

		End Function

#End Region

#End Region

#Region " Validation Rules "

		Protected Overrides Sub AddBusinessRules()

			MyBase.AddBusinessRules()

		End Sub

#End Region

#Region " Data Access & Factory Methods "

#Region " Common "

		''' <summary>
		''' Override if your project has an inherited SMS class.
		''' </summary>
		Public Shared CreateSMSRecipient As Func(Of SmsRecipient) = Function()
																																	Return DataPortal.CreateChild(Of SmsRecipient)()
																																End Function

		Public Shared Function NewSmsRecipient() As SmsRecipient

			Return CreateSMSRecipient()

		End Function

		Public Sub New()

			MarkAsChild()

		End Sub

#End Region

#Region " Silverlight "

#If SILVERLIGHT Then

#End Region

#Region " .NET Data Access "

#Else

#End Region

#Region " .Net Data Access "

		Friend Shared Function GetSmsRecipient(ByVal dr As SafeDataReader) As SmsRecipient

			Dim s = CreateSMSRecipient()
			s.Fetch(dr)
			Return s

		End Function

		Protected Sub Fetch(ByRef sdr As SafeDataReader)

			Using BypassPropertyChecks
				With sdr
					Populate(sdr)
					If SupportsMessageID Then
						MessageID = .GetString(6)
					End If
				End With
			End Using

			MarkAsChild()
			MarkOld()
			BusinessRules.CheckRules()

		End Sub

		Protected Overridable Sub Populate(sdr As SafeDataReader)
			With sdr
				LoadProperty(SmsRecipientIDProperty, .GetInt32(0))
				LoadProperty(SmsIDProperty, Singular.Misc.ZeroNothing(.GetInt32(1)))
				LoadProperty(CellNoProperty, .GetString(2))
				LoadProperty(RecipientNameProperty, .GetString(3))
				LoadProperty(SentDateProperty, .GetValue(4))
				LoadProperty(NotSentErrorProperty, .GetString(5))
			End With
		End Sub

		Friend Sub Insert()

			' if we're not dirty then don't update the database
			Using cm As SqlCommand = New SqlCommand
				cm.CommandText = "InsProcs.insSmsRecipient"

				DoInsertUpdateChild(cm)

			End Using

		End Sub

		Friend Sub Update()

			' if we're not dirty then don't update the database
			Using cm As SqlCommand = New SqlCommand
				cm.CommandText = "UpdProcs.updSmsRecipient"

				DoInsertUpdateChild(cm)

			End Using

		End Sub

		Protected Overrides Sub InsertUpdate(ByVal cm As SqlCommand)

			If Me.IsSelfDirty Then

				With cm
					.CommandType = CommandType.StoredProcedure

					Dim paramSmsRecipientID As SqlParameter = .Parameters.Add("@SmsRecipientID", SqlDbType.Int)
					paramSmsRecipientID.Value = GetProperty(SmsRecipientIDProperty)
					If Me.IsNew Then
						paramSmsRecipientID.Direction = ParameterDirection.Output
					End If

					AddParams(cm)

					.ExecuteNonQuery()

					If Me.IsNew() Then
						LoadProperty(SmsRecipientIDProperty, paramSmsRecipientID.Value)
					End If
					' update child objects
					' mChildList.Update()
					MarkOld()
				End With
			Else
				' update child objects
				' mChildList.Update()
			End If

		End Sub

		Protected Overridable Sub AddParams(cm As SqlCommand)

			cm.Parameters.AddWithValue("@SmsID", Me.GetParent.SmsID)
			cm.Parameters.AddWithValue("@CellNo", GetProperty(CellNoProperty))
			cm.Parameters.AddWithValue("@RecipientName", GetProperty(RecipientNameProperty))
			cm.Parameters.AddWithValue("@NotSentError", GetProperty(NotSentErrorProperty))
			cm.Parameters.AddWithValue("@SentDate", (New SmartDate(GetProperty(SentDateProperty))).DBValue)
			If SupportsMessageID Then
				cm.Parameters.AddWithValue("@MessageID", MessageID)
			End If

		End Sub

		Friend Sub DeleteSelf()

			' if we're not dirty then don't update the database
			If Me.IsNew Then Exit Sub

			Using cm As SqlCommand = New SqlCommand
				cm.CommandText = "DelProcs.delSmsRecipient"
				cm.CommandType = CommandType.StoredProcedure
				cm.Parameters.AddWithValue("@SmsRecipientID", GetProperty(SmsRecipientIDProperty))
				DoDeleteChild(cm)
			End Using

		End Sub

		Protected Overrides Sub DeleteFromDB(ByVal cm As SqlCommand)

			If Me.IsNew Then Exit Sub

			With cm
				.ExecuteNonQuery()
			End With
			MarkNew()

		End Sub

#End If

#End Region

#End Region

	End Class


End Namespace