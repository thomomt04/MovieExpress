Imports System.ComponentModel.DataAnnotations
Imports Singular.DataAnnotations

Namespace ServiceHelpers

  Public Class ServiceVM(Of VM)
    Inherits Singular.Web.StatelessViewModel(Of ServiceVM(Of VM))

    Public Property ServerProgramTypeList As Singular.Service.ServerProgramTypeList
    Public Property ServerProgramType As Singular.Service.ServerProgramType

    <Singular.DataAnnotations.ClientOnlyNoData>
    Public Property LastLogProgram As Singular.Service.ServerProgramType
    Public Property ServerProgressList As Singular.Service.Scheduling.ROScheduleProgressList

    <Display(Name:="Log Date"), SetExpression("GetScheduleProgress()")>
    Public Property LogToDate As Date = Now.Date

    Public ServiceUIList As New List(Of ServiceUI)
    Public Property CurrentUIProgram As Singular.Service.ServerProgramType

    Protected Overrides Sub Setup()
      MyBase.Setup()

      ServerProgramTypeList = Singular.Service.ServerProgramTypeList.GetServerProgramTypeList()

      ServerProgramTypeList.ToList.ForEach(Sub(c)
                                             If c.Info Is Nothing Then
                                               c.Info = New Singular.Service.Scheduling.Schedule
                                             End If
                                           End Sub)

    End Sub

    <Singular.Web.WebCallable>
    Public Function SaveList(List As Singular.Service.ServerProgramTypeList) As Singular.Web.SaveResult

      AssertAccess()

      Return New Singular.Web.SaveResult(List.TrySave)

    End Function

    <Singular.Web.WebCallable>
    Public Function ExportLog(ScheduleID As Integer, ToDate As Date) As Singular.Documents.Document

      AssertAccess()

      Dim ee As New Singular.Data.ExcelExporter
      ee.PopulateData(Singular.Service.Scheduling.ROScheduleProgressList.GetROScheduleProgressList(ScheduleID, ToDate), True, True)
      Return New Singular.Documents.Document("ServiceLog.xlsx", ee.GetStream().ToArray)

    End Function

    Public Function GetProgress(ScheduleID As Integer, ToDate As Date) As Singular.Web.Result

      AssertAccess()

      Return New Singular.Web.Result(
        Function()
          Return Singular.Service.Scheduling.ROScheduleProgressList.GetROScheduleProgressList(ScheduleID, ToDate.Date)
        End Function)

    End Function

    <Singular.Web.WebCallable>
    Public Sub SendServiceMessage(ServerProgramID As Integer, MethodName As String, <Singular.Web.JSonString> MethodArgs As String)

      AssertAccess()

      Dim Message = String.Format("{0}|{1}|{2}", ServerProgramID, MethodName, MethodArgs)

      Singular.Service.NotifyService(Service.ServiceUpdateMessageType.ServiceMessage, Message, True)

    End Sub

    Protected Sub AddServiceUI(Of C)(ServerProgramID As Integer,
                                    MenuText As String,
                                    BindProperty As System.Linq.Expressions.Expression(Of Func(Of VM, C)),
                                    Control As Singular.Web.Controls.HelperControls.HelperBase(Of C))

      Dim BindPropertyName As String = Singular.Reflection.GetMemberSpecific(BindProperty).Name

      ServiceUIList.Add(New ServiceUI With {.MenuText = MenuText,
                                             .BindPropertyName = BindPropertyName,
                                             .Control = Control})

      ServerProgramTypeList.GetItem(ServerProgramID).ServiceMenuItems.Add(New Singular.Service.ServiceMenuInfo With {.Text = MenuText, .PropertyName = BindPropertyName})

    End Sub

    Public Class ServiceUI
      Public Property MenuText As String
      Public Property Control As Singular.Web.Controls.HelperControls.HelperBase
      Public Property BindPropertyName As String
    End Class

  End Class

  Public Class ServiceVM
    Inherits ServiceVM(Of ServiceVM)
  End Class

  Public Class ServiceUIBase(Of ObjectType)
    Inherits Singular.Web.Controls.HelperControls.HelperBase(Of ObjectType)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()
      OnlyChildControls = True

    End Sub

    Protected Function RunServiceMethod(MethodName As String) As String
      Return "ServiceHelper.SendServiceMessage('" & MethodName & "')"
    End Function

  End Class

End Namespace
