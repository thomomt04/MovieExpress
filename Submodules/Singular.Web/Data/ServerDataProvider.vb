Namespace Data

  Public Class ServerDataProvider
    Inherits System.Web.UI.DataSourceControl

    Public Enum SourceType
      Model = 1
      CommonData = 2
      SessionData = 3
    End Enum

    Public Property Source As SourceType

    Public Sub New(Source As SourceType)
      Me.Source = Source
      Me.ID = Source.ToString
    End Sub

    Protected Overrides Function GetView(viewName As String) As System.Web.UI.DataSourceView
      Return New DataSourceView(Me, viewName)
    End Function

    Public Class DataSourceView
      Inherits System.Web.UI.DataSourceView

      Private mViewModel As IViewModel
      Private mSource As ServerDataProvider.SourceType
      Private mPI As System.Reflection.PropertyInfo

      Public Sub New(Owner As System.Web.UI.IDataSource, View As String)
        MyBase.New(Owner, View)

        mSource = CType(Owner, ServerDataProvider).Source

        Select Case mSource
          Case ServerDataProvider.SourceType.Model

            If TypeOf CType(Owner, ServerDataProvider).Page Is Singular.Web.PageBase Then
              mViewModel = CType(CType(Owner, ServerDataProvider).Page, Singular.Web.PageBase).ModelNonGeneric
              mPI = mViewModel.GetType.GetProperty(Name, System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
            Else
              Throw New Exception("ViewModel ServerDataProvider Can Only be used on Pages that inherit from Singular.PageBase")
            End If

        End Select


      End Sub

      Public Function GetData() As System.Collections.IEnumerable
        Return ExecuteSelect(Nothing)
      End Function

      Protected Overrides Function ExecuteSelect(arguments As System.Web.UI.DataSourceSelectArguments) As System.Collections.IEnumerable
        Select Case mSource
          Case ServerDataProvider.SourceType.Model
            Return mPI.GetValue(mViewModel, Nothing)
          Case ServerDataProvider.SourceType.CommonData
            Return CommonData.GetList(Name)
          Case ServerDataProvider.SourceType.SessionData
            Return CommonData.GetSessionList(Name)
        End Select
        Return Nothing
      End Function
    End Class
  End Class

  

End Namespace