Imports Csla
Imports Csla.Core
Imports Csla.Properties
Imports Csla.Serialization
Imports System.Reflection

<Serializable()>
Public Class CommandBase(Of T As CommandBase(Of T))
  Inherits Csla.CommandBase(Of T)

  Public Sub BeginExecute(Callback As EventHandler(Of DataPortalResult(Of T)))

    Dim dp As New DataPortal(Of T)
    AddHandler dp.ExecuteCompleted, Callback
    dp.BeginExecute(Me)

  End Sub

#If SILVERLIGHT Then

#Else

  Public Sub Execute()

    DataPortal.Execute(Of T)(Me)

  End Sub

  Public Function ExecuteWithReturn() As T

    Dim o = DataPortal.Execute(Of T)(Me)
    Return o

  End Function

#End If

End Class

#If SILVERLIGHT = False Then

''' <summary>
''' Lazy option for creating a command base object with minimal code. E.g. on a class inheriting this one, you can call MyCommandObj.GetData({UserID, 123})
''' </summary>
''' <typeparam name="ThisType">The type you are busy creating</typeparam>
''' <remarks></remarks>
<Serializable>
Public MustInherit Class EasyCommand(Of ThisType As EasyCommand(Of ThisType))
  Inherits Singular.CommandBase(Of EasyCommand(Of ThisType))

  <System.ComponentModel.Browsable(False)>
  Public Property ParamValues As Object()

  Public Shared Function GetData(ParamArray ParamValues() As Object) As ThisType
    Dim ec = Activator.CreateInstance(Of ThisType)()
    ec.ParamValues = ParamValues
    Return ec.ExecuteWithReturn
  End Function

  Protected Sub PopulateObject(obj As Object, sdr As Csla.Data.SafeDataReader)
    Singular.Data.DataFunctions.PopulateObject(obj, sdr)
  End Sub

  Protected Sub PopulateList(Of ObjectType)(list As IList, sdr As Csla.Data.SafeDataReader)
    Singular.Data.DataFunctions.PopulateList(Of ObjectType)(list, sdr)
  End Sub

  Protected Overrides Sub DataPortal_Execute()
    'TODO: check if there is already a connection / transaction
    Using cn As New SqlClient.SqlConnection(ConnectionString)
      cn.Open()
      Try
        Using cm As SqlClient.SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.StoredProcedure
          SetupCommand(cm, ParamValues)

          ReadData(New Csla.Data.SafeDataReader(cm.ExecuteReader()))

        End Using
      Finally
        cn.Close()
      End Try
    End Using
  End Sub

  Protected MustOverride Sub SetupCommand(cmd As SqlClient.SqlCommand, ParamValues As Object())

  Protected MustOverride Sub ReadData(sdr As Csla.Data.SafeDataReader)

  Protected Overridable Function ConnectionString() As String
    Return Settings.ConnectionString
  End Function

End Class

<Serializable>
Public MustInherit Class EasyCommand_SingleObject(Of ThisType As EasyCommand(Of ThisType))
  Inherits EasyCommand(Of ThisType)

  Protected Overrides Sub ReadData(sdr As Csla.Data.SafeDataReader)
    If sdr.Read Then
      PopulateObject(Me, sdr)
    End If
  End Sub

End Class

#End If

