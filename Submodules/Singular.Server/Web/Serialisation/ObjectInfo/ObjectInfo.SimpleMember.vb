Imports Singular.Extensions

Namespace Web.Data.JS.ObjectInfo

  <Serializable()>
  Public Class SimpleMember
    Inherits Member

    Private mDDA As Singular.DataAnnotations.DropDownWeb
    Private mDefaultValue As Object
    Private mIsAlreadyJSon As Boolean = False
    Private mProtectKey As Boolean = False
    Private mSetExprBeforeChange As Singular.DataAnnotations.SetExpressionBeforeChange

    Public ReadOnly Property DropDownAttribute As Singular.DataAnnotations.DropDownWeb
      Get
        Return mDDA
      End Get
    End Property

    Public Sub New(ph As PropertyHelper, Obj As Object, Index As Integer, JSSerialiser As JSSerialiser, ti As Singular.ReflectionCached.TypeInfo)
      Dim Inst As Object = Setup(ph, Obj, Index, JSSerialiser, ti)

      If ph.IsProperty Then
        Dim cpi = mPH.CachedPropertyInfo
        mDefaultValue = cpi.DefaultValue
        mDDA = cpi.DropDownWebAttribute
        mIsAlreadyJSon = Singular.Reflection.GetAttribute(Of Singular.Web.JSonString)(ph.PropertyInfo) IsNot Nothing
        mSetExpr = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.SetExpression)(ph.PropertyInfo)
        mProtectKey = JSSerialiser.ProtectKeyProperties AndAlso cpi.IsProtectedKey AndAlso cpi.OnType.IsBusinessObject
        mSetExprBeforeChange = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.SetExpressionBeforeChange)(ph.PropertyInfo)

      End If

      'if this is the toString property, and there is no GetExpression, then just create one.
      If ph.IsMethod AndAlso ph.Name.ToLower = "tostring" AndAlso mPropertyInfo Is Nothing Then
        Dim ToStringString As String = ph.CachedPropertyInfo.OnType.ReadableName 'Singular.Strings.Readable(ph.OnType.Name)
        If ToStringString.EndsWith("VM") Then
          ToStringString = ToStringString.Substring(0, ToStringString.Length - 2)
        End If
        Dim spi As New Singular.SPropertyInfo(Of String, Object)("ToString", "")
        spi.GetExpression("return '" & ToStringString & "';")
        mPropertyInfo = spi
        mHasExtraPropertyInfo = True
      End If

    End Sub

    Friend Overrides Sub RenderModel(JW As Singular.Web.Utilities.JavaScriptWriter)

      If PreRenderModel(JW) AndAlso mPH.Name <> "IsNew" AndAlso mPH.Name <> "IsSelfDirty" Then

        If mPH.IsDynamicallyAdded Then
          JW.Write("self." & mPH.Name & " = ko.observable();", True)

        ElseIf mPH.CachedPropertyInfo.IsKey Then

          If mProtectKey Then
            JW.Write("CreateKeyProperty(self, '" & mPH.Name & "', true);", True)
          Else
            JW.Write("CreateKeyProperty(self, '" & mPH.Name & "');", True)
          End If

        Else

          If mPH.IsProperty OrElse (mHasExtraPropertyInfo AndAlso SPropertyInfo.HasDefaultValueExpression) Then

            'Check if its a ReadOnly property / Client Only property.
            Dim IsReadonly = (Not mPH.PropertyInfo.CanWrite AndAlso (mPH.CachedPropertyInfo.ClientOnly Is Nothing OrElse mPH.CachedPropertyInfo.ClientOnly)) OrElse (mPH.CachedPropertyInfo.ClientOnly.HasValue AndAlso mPH.CachedPropertyInfo.ClientOnly)
            Dim ObservableType As String
            If IsReadonly Then
              ObservableType = "CreateROProperty"
            Else
              ObservableType = "CreateProperty"
            End If

            If mPH.CachedPropertyInfo IsNot Nothing Then
              Select Case mPH.CachedPropertyInfo.MainDataType
                Case Reflection.SMemberInfo.MainType.Boolean
                  ObservableType &= "B"
                Case Reflection.SMemberInfo.MainType.Date
                  ObservableType &= "D"
                Case Reflection.SMemberInfo.MainType.Number
                  ObservableType &= "N"
              End Select
            End If

            JW.Write(ObservableType & "(self, '" & mPH.Name & "'", False)

            If mHasExtraPropertyInfo AndAlso SPropertyInfo.HasDefaultValueExpression Then

              JW.RawWrite(", ")
              SPropertyInfo.WriteDefaultValue(JW)

            ElseIf mDefaultValue IsNot Nothing AndAlso mDefaultValue IsNot DBNull.Value Then

              Dim DefaultValueString As String = ""
              DefaultValueString = Data.JSonWriter.GetJSonValue(mDefaultValue, """")

              If DefaultValueString <> "" Then
                JW.RawWrite(", " & DefaultValueString)
              End If

            End If

            JW.RawWrite(")")
            If mPH.CachedPropertyInfo.IsNullableType Then
              JW.RawWriteLine("")
              JW.AddLevel()
              JW.Write(".Nullable()", False)
              JW.RemoveLevel()
            End If
            If Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.AlwaysClean)(mPH.PropertyInfo) IsNot Nothing Then
              JW.RawWriteLine("")
              JW.AddLevel()
              JW.Write(".AlwaysClean()", False)
              JW.RemoveLevel()
            End If
            RenderPropertySet(JW)
            JW.RawWriteLine(";")

          Else
            JW.Write("self." & mPH.Name & " = ko.observable();", True)
          End If

        End If


      End If

    End Sub

    Friend Overrides Sub RenderSchema(jw As JSonWriter)
      If mPH.IsProperty Then

        jw.StartClass(JSonPropertyName)

        Dim cpi = mPH.CachedPropertyInfo

        If Not cpi.AutoGenerate Then
          jw.WriteProperty("Hidden", True)
        Else
          'Auto generate
          If cpi.DisplayName <> JSonPropertyName Then
            jw.WriteProperty("Display", cpi.DisplayName)
          End If
          If cpi.MainDataTypeShortString <> "s" Then
            jw.WriteProperty("Type", cpi.MainDataTypeShortString)
          End If

          'Number Format
          Dim na = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.NumberField)(mPH.PropertyInfo)
          If na IsNot Nothing AndAlso (na.WebFormatParameters <> "" OrElse na.FormatString <> "") Then
            If na.WebFormatParameters <> "" Then
              jw.WriteProperty("Format", na.WebFormatParameters)
            ElseIf na.FormatString <> "" Then
              jw.WriteProperty("Format", na.FormatString.AddSingleQuotes)
            End If
          End If
          If mPH.PropertyInfo.PropertyType Is GetType(Integer) OrElse mPH.PropertyInfo.PropertyType Is GetType(Long) Then
            jw.WriteProperty("Format", "0," & Chr(160))
          End If

          Dim da = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.DateField)(mPH.PropertyInfo)
          If da IsNot Nothing AndAlso da.FormatString <> "" Then
            jw.WriteProperty("Format", da.FormatString)
          End If

          'Alignment
          Dim al = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.Alignment)(mPH.PropertyInfo)
          If al IsNot Nothing Then
            jw.WriteProperty("Align", al.Align.ToString.ToLower)
          End If

          'Column Width
          Dim cw = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.ColumnWidth)(mPH.PropertyInfo)
          If cw IsNot Nothing AndAlso cw.DefaultWidth <> 0 Then
            jw.WriteProperty("Width", cw.DefaultWidth)
          End If
        End If

        'ID / Key
        If cpi.IsKey Then
          jw.WriteProperty("IDProperty", True)
        End If

        jw.EndClass()
      End If


    End Sub

    Private Sub RenderPropertySet(JW As Singular.Web.Utilities.JavaScriptWriter)

      If (DropDownAttribute IsNot Nothing AndAlso DropDownAttribute.AddAutoSetToModel) OrElse
        (mHasExtraPropertyInfo AndAlso SPropertyInfo.HasSetExpressions) OrElse
        mSetExpr IsNot Nothing Then

        Dim HasBeforeChanged As Boolean = False
        Dim HasAfterChanged As Boolean = (DropDownAttribute IsNot Nothing AndAlso DropDownAttribute.AddAutoSetToModel)

        Dim spi As Singular.ISingularPropertyInfo = SPropertyInfo
        Dim MaxDelay As Integer = 0
        If spi IsNot Nothing Then
          For Each sei In spi.SetExpressionList
            If sei.BeforeChange Then
              HasBeforeChanged = True
            Else
              HasAfterChanged = True
              MaxDelay = Math.Max(MaxDelay, sei.Delay)
            End If
          Next
        End If

        If mSetExpr IsNot Nothing Then
          If mSetExpr.BeforeChange Then HasBeforeChanged = True Else HasAfterChanged = True
          MaxDelay = Math.Max(MaxDelay, mSetExpr.DelayMS)
        End If

        'BeforeChange
        If HasBeforeChanged Then

          JW.RawWriteLine("")
          JW.AddLevel()
          JW.Write(".OnValueChanged(true, function(args) {")
          JW.AddLevel()

          If spi IsNot Nothing Then
            For Each sei In spi.SetExpressionList
              If sei.BeforeChange Then
                sei.WriteSet(JW)
              End If
            Next
          End If

          If mSetExpr IsNot Nothing Then mSetExpr.Write(JW)

          JW.RemoveLevel()
          JW.Write("})", False)
          JW.RemoveLevel()

        ElseIf mSetExprBeforeChange IsNot Nothing Then

          JW.RawWriteLine("")
          JW.AddLevel()
          JW.Write(".OnValueChanged(true, function(args) {")
          JW.AddLevel()

          'For Each sei In spi.SetExpressionList
          '  If sei.BeforeChange Then
          '    sei.WriteSet(JW)
          '  End If
          'Next

          If mSetExprBeforeChange IsNot Nothing Then mSetExprBeforeChange.Write(JW)

          JW.RemoveLevel()
          JW.Write("})", False)
          JW.RemoveLevel()

        End If

        'AfterChange
        If HasAfterChanged Then

          JW.RawWriteLine("")
          JW.AddLevel()
          JW.Write(".OnValueChanged(false, function(args) {")
          JW.AddLevel()

          'Drop Down Auto Property Set:
          If DropDownAttribute IsNot Nothing AndAlso DropDownAttribute.AddAutoSetToModel Then

            For i As Integer = 0 To DropDownAttribute.AutoSetProperties.Length - 1

              Dim Source As String = DropDownAttribute.ClientName.Replace("$parent", "self.GetParent()").Replace("$root", "ViewModel")
              JW.Write("var Obj = " & Source & ".FindValue('" & DropDownAttribute.ValueMember & "', arguments[0], '" & DropDownAttribute.GetAutoSetFromProperty(i) & "');")
              JW.Write("self." & DropDownAttribute.AutoSetProperties(i) & "(Obj ? ko.utils.unwrapObservable(Obj.Value) : null);")

            Next

          End If

          'Other Set Code
          If spi IsNot Nothing Then
            For Each sei In spi.SetExpressionList
              If Not sei.BeforeChange Then
                sei.WriteSet(JW)
              End If
            Next
          End If

          If mSetExpr IsNot Nothing Then mSetExpr.Write(JW)

          JW.RemoveLevel()
          If MaxDelay = 0 Then
            JW.Write("})", False)
          Else
            JW.Write("}, " & MaxDelay & ")", False)
          End If

          JW.RemoveLevel()

        End If

      End If

    End Sub

    Public Overrides Sub UpdateModel(Dynamic As System.Dynamic.DynamicObject, Model As Object)

      Dim CanWrite As Boolean = mPH.IsProperty AndAlso mPH.PropertyInfo.CanWrite

      If Not CanWrite AndAlso JSSerialiser.CurrentPropertySetMode = JS.JSSerialiser.SetMode.SetBackingField Then
        If mPH.CachedPropertyInfo.IsKey Then
          CanWrite = True
        ElseIf mPH.CachedPropertyInfo.ClientOnly IsNot Nothing AndAlso Not mPH.CachedPropertyInfo.ClientOnly Then
          CanWrite = True
        End If
      End If

      'Only if the Property is writable.
      If Model IsNot Nothing AndAlso CanWrite Then

        'Get the Value From the JSon Object
        Dim Value As Object = Nothing

        If Dynamic.TryGetMember(New Singular.Dynamic.MemberGetter(mPH.Name), Value) Then

          'Check if the value changed since it was sent to the client:
          If JSSerialiser.CurrentPropertySetMode = JS.JSSerialiser.SetMode.SetBackingField OrElse TempPropertyValue <> System.Web.Helpers.Json.Encode(Value) Then

            DecryptKey(Value)

            Dim SetValue As Object = Nothing
            Dim MustSetValue As Boolean = False

            If mTypeAtRuntime Is GetType(Date) Then
              If Value Is Nothing Then
                SetValue = Nothing
                MustSetValue = True
              Else
                Dim DateString As String = Value.ToString
                'If DateString.StartsWith("/") AndAlso DateString.EndsWith("/") Then
                '  SetValue = Helpers.Json.Decode("""\" & DateString.Substring(0, DateString.Length - 1) & "\/""")
                '  MustSetValue = True
                'Else
                If Date.TryParse(DateString, SetValue) Then
                  MustSetValue = True
                End If
                'End If
              End If

            Else
              SetValue = Singular.Reflection.ConvertValueToType(mTypeAtRuntime, Value)
              MustSetValue = True
            End If

            If JSSerialiser.CurrentPropertySetMode = JS.JSSerialiser.SetMode.SetProperty OrElse mPropertyInfo Is Nothing Then
              mPH.PropertyInfo.SetValue(Model, SetValue, Nothing)
            Else
              CType(Model, ISingularBase).SetBackingFieldValue(mPropertyInfo, SetValue, False)
            End If

          End If

        End If

      End If

    End Sub

    Public Shared Function EncryptKey(ID As Object, ObjectType As Type) As String
      Return EncryptKey(ID, Singular.ReflectionCached.GetCachedType(ObjectType).ProtectedKeySalt)
    End Function

    Public Shared Function EncryptKey(ID As Object, TypeSalt As String) As String
      Return Singular.Encryption.GetEncryptedToken(ID & Chr(30) & TypeSalt & Chr(30) & Singular.Settings.CurrentUserID)
    End Function

    Public Shared Function EncryptKeyHex(ID As Object, TypeSalt As String) As String
      Return Singular.Encryption.GetEncryptedTokenHex(ID & Chr(30) & TypeSalt & Chr(30) & Singular.Settings.CurrentUserID)
    End Function

    Public Shared Function DecryptKey(Key As String, ObjectType As Type, ByRef ID As Object) As Boolean
      Return DecryptKey(Key, Singular.ReflectionCached.GetCachedType(ObjectType).ProtectedKeySalt, ID)
    End Function

    Friend Shared Function DecryptKey(Key As String, TypeSalt As String, ByRef ID As Object) As Boolean
      Dim KeyParts As String() = {}
      Try
        Select Case JS.JSSerialiser.KeyEncodingType
          Case JSSerialiser.EncodingType.Hex
            KeyParts = Singular.Encryption.DecryptTokenHex(Key).Split(Chr(30))
          Case JSSerialiser.EncodingType.Base64
            KeyParts = Singular.Encryption.DecryptToken(Key).Split(Chr(30))
        End Select
        If KeyParts(1) = TypeSalt AndAlso (KeyParts(2) = 0 OrElse KeyParts(2) = Singular.Settings.CurrentUserID) Then
          ID = KeyParts(0)
          Return True
        End If
      Catch ex As Exception
      End Try
      Throw New Exception("Invalid object key")
    End Function

    Friend Sub DecryptKey(ByRef Key As Object)
      If mProtectKey AndAlso Key IsNot Nothing Then
        DecryptKey(Key, mPH.CachedPropertyInfo.OnType.ProtectedKeySalt, Key)
      End If
    End Sub


    Friend Overrides Sub RenderData(JW As JSonWriter, Model As Object)

      If CanRenderData(JW, Nothing) Then

        If mRenderPropertyName Then
          JW.WritePropertyName(mJSonPropertyName, False)
        End If

        If mIsAlreadyJSon Then
          JW.Write(GetPropertyValue(Model))
        Else
          Dim Value As Object = GetPropertyValue(Model)
          JW.WriteJSonValue(Value)

          'Key properties need a protection value
          If mProtectKey Then
            'Encrypt the KeyValue and append the Type Name as the salt, so that the hacker cant use this ID on another object type.
            'Adding GUID is pointless since the guid can be changed to the same as a previous object.
            Select Case JS.JSSerialiser.KeyEncodingType
              Case JSSerialiser.EncodingType.Hex
                JW.WriteProperty("_Key", EncryptKeyHex(Value, mPH.CachedPropertyInfo.OnType.ProtectedKeySalt))
              Case JSSerialiser.EncodingType.Base64
                JW.WriteProperty("_Key", EncryptKey(Value, mPH.CachedPropertyInfo.OnType.ProtectedKeySalt))
            End Select
          End If

        End If

      End If

    End Sub

    Public Property TempPropertyValue As String
    Public Sub SetTempPropertyValue(Instance)

      Dim CanWrite As Boolean = mPH.IsProperty AndAlso (mPH.PropertyInfo.CanWrite OrElse (JSSerialiser.CurrentPropertySetMode = JS.JSSerialiser.SetMode.SetBackingField AndAlso mPH.CachedPropertyInfo.IsKey))
      If CanWrite Then
        Dim Value As Object = GetPropertyValue(Instance)
        TempPropertyValue = Data.JSonWriter.GetJSonValue(Value, """")
      End If

    End Sub

    Public Function GetPropertyValue(Instance As Object) As Object

      Dim Value As Object = Nothing
      If Instance IsNot Nothing Then
        Return mPH.GetValue(Instance)
      End If

      Return Nothing

    End Function

  End Class

End Namespace