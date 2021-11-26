Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.IO

Namespace WebServices

  <ServiceContract()>
  Public Interface IMobileWCFService

    <OperationContract()>
    Function GetData(ByVal Params As IO.Stream) As String

    <OperationContract()>
    Function SaveData(Args As IO.Stream) As String

    <OperationContract()>
    Function GetTest() As String

  End Interface

  <Activation.AspNetCompatibilityRequirements(RequirementsMode:=Activation.AspNetCompatibilityRequirementsMode.Allowed)>
  Public Class MobileWCFService
    Implements IMobileWCFService

    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json)>
    Public Function GetData(Args As IO.Stream) As String Implements IMobileWCFService.GetData

      Return StatelessService.GetData(Args)

    End Function

    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json)>
    Public Function SaveData(Args As IO.Stream) As String Implements IMobileWCFService.SaveData

      Return StatelessService.SaveData(Args)

    End Function

    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json)>
    Public Function GetTest() As String Implements IMobileWCFService.GetTest

      Return "{""Success"": 1}"
    End Function
  End Class

End Namespace




