Imports System.ComponentModel.DataAnnotations

Namespace Web.Data.JS

  Friend Class JSRule
    Public Property Rule As String
    Public Property RuleArg As String
    Private mRuleDescription As String = ""
    Private mRuleArgIsFunction As Boolean = False

    Public Sub New()

    End Sub

    Public Sub New(attr As System.ComponentModel.DataAnnotations.ValidationAttribute, PropertyName As String, PropertyInfo As ReflectionCached.CachedMemberInfo)

      Dim FriendlyName As String = If(PropertyInfo Is Nothing, Singular.Strings.Readable(PropertyName), PropertyInfo.DisplayName)

      'Required
      If TypeOf attr Is RequiredAttribute Then

        Rule = "Required"
        RuleDescription = GetRuleErrorMessage(attr, FriendlyName & " is required.")
        If TypeOf attr Is Singular.DataAnnotations.RequiredIf Then
          RuleArg = CType(attr, Singular.DataAnnotations.RequiredIf).GetRuleArgument
          mRuleArgIsFunction = True
        End If

        'String Length
      ElseIf TypeOf attr Is StringLengthAttribute Then
        Dim sla As StringLengthAttribute = attr
        If sla.MinimumLength > 0 Then
          Rule = "MinLength"
          RuleDescription = GetRuleErrorMessage(attr, FriendlyName & " must be at least " & sla.MinimumLength & " characters.")
          RuleArg = sla.MinimumLength
        End If


        'Number Range
      ElseIf TypeOf attr Is RangeAttribute Then
        Dim ra As RangeAttribute = attr
        Rule = "Range"
        RuleDescription = GetRuleErrorMessage(attr)
        RuleArg = CDec(ra.Minimum).ToString(System.Globalization.CultureInfo.InvariantCulture) & "," & CDec(ra.Maximum).ToString(System.Globalization.CultureInfo.InvariantCulture)

        'Email
      ElseIf TypeOf attr Is Singular.DataAnnotations.EmailField Then
        Dim ra As Singular.DataAnnotations.EmailField = attr
        Rule = "Email"
        RuleDescription = GetRuleErrorMessage(attr, FriendlyName & " must be a valid email address.")

        'Regex
      ElseIf TypeOf attr Is RegularExpressionAttribute Then
        Rule = "RegEx"
        RuleDescription = GetRuleErrorMessage(attr, FriendlyName & " is not in the correct format.")
        RuleArg = CType(attr, RegularExpressionAttribute).Pattern

        'Rounded Number
      ElseIf TypeOf attr Is Singular.DataAnnotations.RoundedNumber Then
        Rule = "Rounded"
        RuleDescription = GetRuleErrorMessage(attr)
        RuleArg = CType(attr, Singular.DataAnnotations.RoundedNumber).DecimalPlaces
      End If

    End Sub

    Private Function GetRuleErrorMessage(Rule As ValidationAttribute, Optional DefaultMessage As String = "") As String
      If Rule.ErrorMessageResourceName <> "" Then
        Return Singular.Localisation.LocalText(Rule.ErrorMessageResourceName)
      ElseIf Rule.ErrorMessage <> "" Then
        Return Rule.ErrorMessage
      Else
        Return DefaultMessage
      End If
    End Function

    Public Property RuleDescription As String
      Get
        Return mRuleDescription
      End Get
      Set(value As String)
        If value IsNot Nothing Then
          mRuleDescription = value.Replace("'", "`")
        End If
      End Set
    End Property

    Public Function GetRuleArgsJS() As String

      Dim jsw As New JSonWriter
      jsw.OutputMode = OutputType.Javascript
      jsw.StartObject()

      jsw.WriteProperty("Description", mRuleDescription)
      If RuleArg <> "" Then
        If mRuleArgIsFunction Then
          jsw.WritePropertyRaw("Args", RuleArg)
        Else
          jsw.WriteProperty("Args", RuleArg)
        End If

      End If

      jsw.EndObject()
      Return jsw.ToString

    End Function

  End Class

End Namespace
