Namespace Scripts

  Public Class JavascriptLocaliser

    Private Shared _Scripts As New Dictionary(Of String, Byte())

    Public Shared Sub CreateFiles()

      If Singular.Localisation.LocalisationStringsSource = Singular.Localisation.SourceType.Database Then

        Dim ReadKeys As Boolean = False
        Dim DefaultKeys As New Hashtable

        Using ms As New IO.MemoryStream(LibResourceHandler.GetEmbeddedContent("Singular.Core.js"))
          Using sr As New IO.StreamReader(ms)

            'Find all the keys.
            While Not sr.EndOfStream

              Dim Line = sr.ReadLine

              If Line.Trim = "//[JS Key Definition Start]" Then
                ReadKeys = True
                Continue While
              End If

              If ReadKeys Then

                Dim i1 = Line.IndexOf("'") + 1

                If i1 > 0 Then
                  Dim i2 = Line.IndexOf("'", i1)
                  DefaultKeys.Add(Line.Substring(i1, i2 - i1), True)

                End If

              End If

              If Line.Trim = "//[JS Key Definition End]" Then
                Exit While
              End If

            End While

          End Using
        End Using

        'Create a file with the keys for each language.
        For Each l In Singular.Localisation.SupportedLanguages
          If l.Key <> 10 Then

            Dim NewKeys As New Hashtable
            Dim Culture = Singular.Localisation.CreateLanguageCulture(l.Value.CultureCode)

            For Each key In DefaultKeys.Keys
              Dim value = Singular.Localisation.LocalTextForCultureJS(key, Singular.Localisation.DefaultVariantID, Culture)
              If value IsNot Nothing Then
                NewKeys.Add(key, value)
              End If
            Next

            'If NewKeys.Count > 0 Then
            Using ms As New IO.MemoryStream
              Using sw As New IO.StreamWriter(ms)

                For Each key In NewKeys.Keys
                  sw.WriteLine("LocalStrings.Set('" & key & "', '" & NewKeys(key) & "');")
                Next

              End Using

              _Scripts.Add(l.Value.CultureCode, ms.ToArray())
            End Using

          End If

        Next

      End If

    End Sub

    Public Shared Function GetLocalisedResourceStrings(CultureCode As String) As Byte()
      Return _Scripts(CultureCode)
    End Function

    Friend Shared Function LocalisedScriptURL(CultureCode As String) As String
      Return LibResourceHandler.GetURL(CultureCode & ".js", CultureCode, "l", Singular.Web.ScriptsVersion)
    End Function

  End Class

End Namespace


