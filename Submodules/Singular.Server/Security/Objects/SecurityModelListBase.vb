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
  Public Class SecurityModelListBase(Of T As SecurityModelListBase(Of T, C, SGL, SG),
                                        C As SecurityModelBase(Of C, SGL, SG),
                                        SGL As SecurityGroupListBase(Of SGL, SG),
                                        SG As SecurityGroupBase(Of SG))
    Inherits SingularBusinessListBase(Of T, C)

#Region " Business Methods "

    Public Overrides Function ToString() As String

      Return "Security"

    End Function

#End Region

  End Class


End Namespace