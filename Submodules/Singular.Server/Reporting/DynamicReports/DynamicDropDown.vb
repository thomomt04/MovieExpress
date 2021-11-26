Namespace Reporting.DynamicReports

  Public Class DynamicDropDownList
    Inherits List(Of DynamicDropDown)

    ''' <summary>
    ''' Adds dropdown info for use with dynamic reports.
    ''' </summary>
    ''' <param name="Name">The name to appear in the dynamic reports setup screen.</param>
    ''' <param name="DropDownInfo">The information about the drop down. Specify as you would on a drop down property.</param>
    ''' <param name="DefaultParameterName">The parameter name that usually requires this drop down. E.g. BranchID will always require a Branch drop down.</param>
    Public Sub AddDropDown(Name As String, DropDownInfo As DataAnnotations.DropDownWeb, Optional DefaultParameterName As String = "")

      Dim ddd As New DynamicDropDown With {.Name = Name, .DropDownInfo = DropDownInfo, .DefaultParameterName = DefaultParameterName.Replace("@", "")}
      Add(ddd)

    End Sub

    Friend Function GetDefault(ParameterName As String) As DynamicDropDown
      For Each ddd As DynamicDropDown In Me
        If ddd.DefaultParameterName <> "" AndAlso ddd.DefaultParameterName = ParameterName Then
          Return ddd
        End If
      Next
      Return Nothing
    End Function

    Public Function GetItem(Name As String) As DynamicDropDown
      For Each ddd As DynamicDropDown In Me
        If ddd.Name = Name Then
          Return ddd
        End If
      Next
      Return Nothing
    End Function

  End Class

  Public Class DynamicDropDown

    Public Property Name As String

    <System.ComponentModel.Browsable(False)> Public Property DropDownInfo As DataAnnotations.DropDownWeb

    <System.ComponentModel.Browsable(False)> Public Property DefaultParameterName As String = ""

  End Class



End Namespace

