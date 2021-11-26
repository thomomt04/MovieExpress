Namespace Localisation

  <Serializable>
  Public Class JSKeyManager

    Private mKeys As New Hashtable
    Private mKeysWithValues As New Dictionary(Of String, String)

    Public Sub AddKey(ParamArray Keys() As String)
      For Each Key As String In Keys
        mKeys(Key) = True
      Next
    End Sub

    Public Sub AddKeyWithValue(Key As String, Value As String)
      mKeysWithValues(Key) = Value
    End Sub

    Public Sub WriteKeyListJS(Writer As Singular.Web.Utilities.JavaScriptWriter)

      For Each key As String In mKeys.Keys
        Writer.RawWriteLine("LocalStrings['" & key & "'] = '" & Singular.Localisation.LocalTextJS(key) & "';")
      Next

      For Each kv In mKeysWithValues
        Writer.RawWriteLine("LocalStrings['" & kv.Key & "'] = '" & kv.Value & "';")
      Next

    End Sub

  End Class

End Namespace

