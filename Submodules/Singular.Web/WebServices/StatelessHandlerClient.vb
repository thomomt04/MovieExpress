Imports System.Net

Namespace WebServices

  Public Class StatelessHandlerClient

    Private _ServerPath As String

    Public Sub New(ServerPath As String)
      _ServerPath = ServerPath
    End Sub

    Public Function CallServerMethod(Mi As System.Reflection.MethodInfo, ParamArray Params() As Object) As Singular.Web.Result

      Dim ParamList As New Dictionary(Of String, Object)
      Dim MethodParams = Mi.GetParameters()
      For i As Integer = 0 To MethodParams.Length - 1
        ParamList.Add(MethodParams(i).Name, Params(i))
      Next

      Return CallServerMethod(Mi.DeclaringType, Mi.Name, ParamList)

    End Function

    Public Function CallServerMethod(Type As Type, Method As String, Parameters As Dictionary(Of String, Object)) As Singular.Web.Result
      Return CallServerMethod(Singular.Reflection.GetTypeFullName(Type), Method, Parameters)
    End Function

    Public Function CallServerMethod(Type As String, Method As String, Parameters As Dictionary(Of String, Object)) As Singular.Web.Result

      Dim wc = System.Net.HttpWebRequest.Create(_ServerPath & "/" & StatelessHandler.StatelessHandlerPath)
      wc.Method = "POST"

      Dim JSONParams As String = "{"
      Dim First As Boolean = True

      For Each param In Parameters
        If param.Value IsNot Nothing Then

          If First Then
            First = False
          Else
            JSONParams &= ", "
          End If

          JSONParams &= String.Format("""{0}"": {1}", param.Key, Singular.Web.Data.JSonWriter.SerialiseObject(param.Value, , False))

        End If
      Next

      JSONParams &= " }"

      Dim Args = String.Format("{{ ""Method"": ""Command"", ""Type"": ""{0}"", ""CallMethod"": ""{1}"", ""Args"": {2} }}",
                               Type, Method, JSONParams)

      Using req = wc.GetRequestStream()
        Using sw As New IO.StreamWriter(req)
          sw.Write(Args)
        End Using
      End Using

      Try
        Using response = wc.GetResponse().GetResponseStream
          Using sr As New IO.StreamReader(response)

            Dim JSonResponse As String = sr.ReadToEnd
            If Not String.IsNullOrEmpty(JSonResponse) Then
              Return Singular.Web.Data.JS.StatelessJSSerialiser.DeserialiseObject(Of Singular.Web.Result)(JSonResponse)
            Else
              Return New Singular.Web.Result(True)
            End If

          End Using
        End Using
      Catch ex As WebException
        Using response = ex.Response.GetResponseStream
          Using sr As New IO.StreamReader(response)
            Return New Singular.Web.Result(False, sr.ReadToEnd)
          End Using
        End Using
      End Try

    End Function

  End Class

End Namespace

