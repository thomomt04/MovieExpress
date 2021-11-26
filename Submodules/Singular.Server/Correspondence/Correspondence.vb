Namespace Correspondence

  Public Interface IExtraInfoContainer
    Property ExtraInfo As ExtraInfo
  End Interface

#If Silverlight = False Then
  <Serializable>
  Public MustInherit Class ExtraInfo

    Public MustOverride Sub BeforeUpdate(cm As SqlClient.SqlCommand)

  End Class
#Else
  Public MustInherit Class ExtraInfo
  End Class
#End If


End Namespace
