Namespace Reporting
  Public Class ReportMenuControl
    Inherits Controls.HelperControls.HelperBase(Of ReportVM)

    Protected Friend Overrides Sub Setup()
      MyBase.Setup()

      With Helpers.DivC("ReportSectionList")

        For Each MS As Singular.Reporting.MainSection In Singular.Reporting.ProjectReportHierarchy.GetMainSections
          If MS.IsAllowed() Then

            'Column
            With .Helpers.DivC("ReportSectionColumn")

              With .Helpers.Div
                .AddClass("ReportSection")

                'Heading
                With .Helpers.Div()
                  .AddClass("Heading")
                  If MS.ImagePath <> "" Then
                    .Helpers.Image(MS.ImagePath).Style("margin-right") = "5px"
                  End If
                  .Helpers.HTML(MS.Heading)
                End With

                'Body
                With .Helpers.Div()

                  Dim TopLevelUL = .Helpers.HTMLTag("ul")
                  With TopLevelUL

                    'Sub Sections
                    For Each SS As Singular.Reporting.SubSection In MS.SubSectionList
                      If SS.IsAllowed Then

                        Dim ReportUL As Singular.Web.CustomControls.HTMLTag(Of ReportVM) = TopLevelUL

                        If SS.Heading <> "" Then
                          With .Helpers.HTMLTag("li")
                            'If the sub section has a heading, then create another sub un-ordered list
                            .Helpers.HTML(SS.Heading)
                            ReportUL = .Helpers.HTMLTag("ul")
                          End With
                        End If

                        For Each R As Singular.Reporting.Report In SS.ReportList
                          If R.IsAllowed Then

                            With ReportUL.Helpers.HTMLTag("li")
                              .Attributes("title") = HttpUtility.HtmlEncode(R.Report.Description)

                              'Add a link to the report.

                              If R.Report.ReportURL = "" Then

                                If R.Report.UniqueKey = "" Then
                                  'Make a hash of the link so the user can't see the Type Name of the report.
                                  Dim LinkHash As String = Singular.Reporting.GetHash(R.Report.GetType)
                                  .Helpers.LinkFor(, , "?Type=" & Server.UrlEncode(LinkHash), R.Report.ReportName)
                                Else
                                  .Helpers.LinkFor(, , "?Key=" & Server.UrlEncode(R.Report.UniqueKey), R.Report.ReportName)
                                End If

                              Else
                                Dim Url As String = R.Report.ReportURL
                                .Helpers.LinkFor(, , If(Url.StartsWith("~"), Utils.URL_ToAbsolute(Url), Url), R.Report.ReportName, R.Report.LinkTargetType)
                              End If


                            End With

                          End If
                        Next

                      End If
                    Next

                  End With
                End With
              End With

            End With
            
          End If
        Next

      End With


      With Helpers.Div
        .Style.ClearBoth()
        .Style("height") = "30px"
      End With

    End Sub

    Protected Friend Overrides Sub Render()
      MyBase.Render()

      RenderChildren()
    End Sub

  End Class


End Namespace


