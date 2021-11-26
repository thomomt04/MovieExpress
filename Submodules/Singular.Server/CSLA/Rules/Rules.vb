Imports Csla.Rules
Imports Csla.Core
Imports Singular.Extensions.Strings
Imports Singular.Extensions.Numeric
Imports System.Reflection

Namespace Rules

#Region " String Numeric "

  Public Class StringNumericRule
    Inherits BusinessRule

    Public Property IgnoreEmptyString As Boolean

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, Optional ByVal IgnoreEmptyString As Boolean = False)

      MyBase.New(PrimaryProperty)
      Me.InputProperties = New List(Of IPropertyInfo) From {PrimaryProperty}

      Me.IgnoreEmptyString = IgnoreEmptyString

      RuleUri.AddQueryParameter("IgnoreEmptyString", IgnoreEmptyString.ToString())

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      Dim input = context.InputPropertyValues(PrimaryProperty)
      Dim strNumber = CStr(input)
      If Not strNumber.IsNumeric() Then
        If Not (IgnoreEmptyString AndAlso String.IsNullOrEmpty(strNumber)) Then
          context.AddErrorResult(PrimaryProperty.FriendlyName & " must be numeric")
        End If
      End If

    End Sub

  End Class

#End Region

#Region " Unique In Cache "

  Public Class UniqueInCachedList(Of T As SingularBusinessListBase(Of T, C), C As SingularBusinessBase(Of C))
    Inherits BusinessRule

    Public Sub New(ByVal ParamArray UniqueProperties() As IPropertyInfo)

      MyBase.New(UniqueProperties(0))

      Me.PrimaryProperty = UniqueProperties(0)

      Dim ipl As New List(Of IPropertyInfo)
      For Each prop In UniqueProperties
        ipl.Add(prop)
      Next
      Me.InputProperties = ipl

      If UniqueProperties.Length > 1 Then
        For i = 1 To UniqueProperties.Length - 1
          Me.AffectedProperties.Add(UniqueProperties(i))
        Next
      End If

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      Dim Target As C = context.Target
      Dim List As T = Target.Parent

      If List IsNot Nothing AndAlso List.Count > 1 Then
        Dim DuplicateFound As Boolean = True
        Dim DuplicateItem As C = Nothing
        For Each item In List
          ' make sure we are not looking at the target
          If item IsNot Target Then
            ' assume it's a duplicate until we prove otherwise
            DuplicateFound = True
            ' loop through the properties and compare them
            For Each p In Me.InputProperties
              Dim pi = item.GetType.GetProperty(p.Name)
              Dim Value = pi.GetValue(item, Nothing)
              If Not Singular.Misc.CompareSafe(Value, context.InputPropertyValues(p)) Then
                ' values dont match
                DuplicateFound = False
                Exit For
              End If
            Next
            If DuplicateFound Then
              DuplicateItem = item
              Exit For
            End If
          End If
        Next
        If DuplicateItem IsNot Nothing Then
          context.AddErrorResult("This item conflicts with another item: " & DuplicateItem.ToString())
        End If
      End If

    End Sub

  End Class

#End Region

#Region " Unique In DB Rule "

  Public Class UniqueInDBRule
    Inherits BusinessRule

    Public Property TableToCheck As String
    Public Property PrimaryKeyProperty As Csla.PropertyInfo(Of Integer)
    Public Property DisplayProperties As String()
    Public Property IgnoreEmptyStringOrNULL As Boolean = False


    Public Sub New(ByVal UniqueProperty As IPropertyInfo, ByVal PrimaryKeyProperty As Csla.PropertyInfo(Of Integer), ByVal TableToCheck As String, ByVal DisplayProperties() As String, Optional ByVal IgnoreEmptyStringOrNULL As Boolean = False)

      MyBase.New(UniqueProperty)

      SetupRule(New List(Of IPropertyInfo) From {UniqueProperty}, PrimaryKeyProperty, TableToCheck, DisplayProperties, IgnoreEmptyStringOrNULL)

    End Sub


    Public Sub New(ByVal UniqueProperties As List(Of IPropertyInfo), ByVal PrimaryKeyProperty As Csla.PropertyInfo(Of Integer), ByVal TableToCheck As String, ByVal DisplayProperties() As String, Optional ByVal IgnoreEmptyStringOrNULL As Boolean = False)

      MyBase.New(UniqueProperties(0))

      SetupRule(UniqueProperties, PrimaryKeyProperty, TableToCheck, DisplayProperties, IgnoreEmptyStringOrNULL)

    End Sub

    Private Sub SetupRule(ByVal UniqueProperties As List(Of IPropertyInfo), ByVal PrimaryKeyProperty As Csla.PropertyInfo(Of Integer), ByVal TableToCheck As String, ByVal DisplayProperties() As String, Optional ByVal IgnoreEmptyStringOrNULL As Boolean = False)

      Me.IsAsync = True

      Me.Priority = 1

      Me.PrimaryProperty = UniqueProperties(0)

      UniqueProperties.Add(PrimaryKeyProperty)

      Me.InputProperties = UniqueProperties

      Me.TableToCheck = TableToCheck
      Me.PrimaryKeyProperty = PrimaryKeyProperty
      Me.DisplayProperties = DisplayProperties
      Me.IgnoreEmptyStringOrNULL = IgnoreEmptyStringOrNULL

      RuleUri.AddQueryParameter("TableToCheck", TableToCheck)
      RuleUri.AddQueryParameter("PrimaryKeyProperty", PrimaryKeyProperty.ToString)
      RuleUri.AddQueryParameter("DisplayProperties", DisplayProperties.ToString)

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      ' this rule will run asynchronously on Silverlight, but synchronously otherwise

      ''Only call check this rule for new or dirty object's properties
      'If context.Target IsNot Nothing AndAlso context.Target.IsDirty Then
      '  Exit Sub
      'End If

#If SILVERLIGHT Then

      If Me.IgnoreEmptyStringOrNULL Then

        Dim mUniquePropertiesList As MobileList(Of String) = Me.GetUniqueProperties
        Dim mUniquePropertyValuesList As MobileList(Of String) = Me.GetUniquePropertyValues(context)

        Dim NonEmptyPropertiesList As New MobileList(Of String)
        Dim NonEmptyPropertyValuesList As New MobileList(Of String)


        For i As Integer = 0 To mUniquePropertyValuesList.Count - 1

          If mUniquePropertyValuesList(i).Length > 2 Then
            NonEmptyPropertiesList.Add(mUniquePropertiesList(i))
            NonEmptyPropertyValuesList.Add(mUniquePropertyValuesList(i))
          End If
        Next

        If NonEmptyPropertyValuesList.Count = 0 Then
          context.Complete()
          Exit Sub
        End If

        RecordChecker.DuplicateRecordExists(Me.PrimaryKeyProperty.Name, CInt(context.InputPropertyValues(Me.PrimaryKeyProperty)),
                                        Me.GetUniqueProperties, Me.GetUniquePropertyValues(context), Me.TableToCheck, Singular.Arrays.GetMobileList(Of String)(Me.DisplayProperties),
                                       Sub(o, e)
                                         If e.Error IsNot Nothing Then
                                           context.AddErrorResult("Error running unique rule " & Singular.Debug.RecurseExceptionMessage(e.Error))
                                         Else
                                           ' we have a result
                                           If e.Object.DuplicateExists Then
                                             context.AddErrorResult("Duplicate Found: " & e.Object.ResultDescription)
                                           End If
                                         End If

                                         context.Complete()
                                       End Sub)
      Else

        RecordChecker.DuplicateRecordExists(Me.PrimaryKeyProperty.Name, CInt(context.InputPropertyValues(Me.PrimaryKeyProperty)),
                                         Me.GetUniqueProperties, GetUniquePropertyValues(context), Me.TableToCheck, Singular.Arrays.GetMobileList(Of String)(Me.DisplayProperties),
                                         Sub(o, e)
                                           If e.Error IsNot Nothing Then
                                             context.AddErrorResult("Error running unique rule " & Singular.Debug.RecurseExceptionMessage(e.Error))
                                           Else
                                             ' we have a result
                                             If e.Object.DuplicateExists Then
                                               context.AddErrorResult("Duplicate Found: " & e.Object.ResultDescription)
                                             End If
                                           End If

                                           context.Complete()
                                         End Sub)

      End If

#Else

      Try

        If Me.IgnoreEmptyStringOrNULL Then

          Dim mUniquePropertiesList As MobileList(Of String) = Me.GetUniqueProperties
          Dim mUniquePropertyValuesList As MobileList(Of String) = Me.GetUniquePropertyValues(context)

          Dim NonEmptyPropertiesList As New MobileList(Of String)
          Dim NonEmptyPropertyValuesList As New MobileList(Of String)

          For i As Integer = 0 To mUniquePropertyValuesList.Count - 1

            If mUniquePropertyValuesList(i).Length > 2 Then
              NonEmptyPropertiesList.Add(mUniquePropertiesList(i))
              NonEmptyPropertyValuesList.Add(mUniquePropertyValuesList(i))
            End If
          Next

          If NonEmptyPropertyValuesList.Count = 0 Then
            context.Complete()
            Exit Sub
          End If

          Dim rc = RecordChecker.DuplicateRecordExists(Me.PrimaryKeyProperty.Name, CInt(context.InputPropertyValues(Me.PrimaryKeyProperty)),
                                 Me.GetUniqueProperties, Me.GetUniquePropertyValues(context), Me.TableToCheck, Singular.Arrays.GetMobileList(Of String)(Me.DisplayProperties))


          ' we have a result
          If rc.DuplicateExists Then
            context.AddErrorResult("Duplicate Found: " & rc.ResultDescription)
          End If

        Else

          Dim rc = RecordChecker.DuplicateRecordExists(Me.PrimaryKeyProperty.Name, CInt(context.InputPropertyValues(Me.PrimaryKeyProperty)),
                                   Me.GetUniqueProperties, GetUniquePropertyValues(context), Me.TableToCheck, Singular.Arrays.GetMobileList(Of String)(Me.DisplayProperties))

          ' we have a result
          If rc.DuplicateExists Then
            context.AddErrorResult("Duplicate Found: " & rc.ResultDescription)
          End If

        End If

      Catch ex As Exception
        context.AddErrorResult("Error running unique rule " & Singular.Debug.RecurseExceptionMessage(ex))
      End Try

      context.Complete()

#End If

    End Sub

    Private Function GetUniquePropertyValues(ByVal context As Csla.Rules.RuleContext) As MobileList(Of String)

      Dim ReturnValue As New MobileList(Of String)

      For i = 0 To context.InputPropertyValues.Count - 2
        If Me.InputProperties(i).Type.Equals(GetType(String)) Then
          'Emile Added this so that if the string as an apostrophy then it becomes a double apostrophy so that it doesnt break SQL
          Dim str As String = CStr(context.InputPropertyValues(Me.InputProperties(i)))
          str = str.Replace("'", "''")
          ReturnValue.Add("'" & str & "'")
        Else
          ReturnValue.Add(CStr(context.InputPropertyValues(Me.InputProperties(i))))
        End If
      Next

      Return ReturnValue

    End Function

    Private Function GetUniqueProperties() As MobileList(Of String)

      Dim ReturnValue As New MobileList(Of String)

      For i = 0 To Me.InputProperties.Count - 2
        ReturnValue.Add(Me.InputProperties(i).Name)
      Next

      Return ReturnValue

    End Function

  End Class

#End Region

#Region " ChildListUnique "

  Public Class ChildListUnique(Of Parent As SingularBusinessBase(Of Parent), ChildList As SingularBusinessListBase(Of ChildList, Child), Child As SingularBusinessBase(Of Child))
    Inherits BusinessRule

    Public Property UniqueProperties As String()
    Public Property IgnoreCase As Boolean = True
    Public Property IgnoreBlanks As Boolean = False
    Public Property RuleBrokenDescription As String = ""

    Public Sub New(ChildListProperty As IPropertyInfo, ByVal ParamArray UniqueProperties() As String)

      MyBase.New(ChildListProperty)

      Setup(ChildListProperty, UniqueProperties, False, False, "")

    End Sub

    Public Sub New(ChildListProperty As IPropertyInfo, IgnoreCase As Boolean, IgnoreBlanks As Boolean, ByVal ParamArray UniqueProperties() As String)

      MyBase.New(ChildListProperty)

      Setup(ChildListProperty, UniqueProperties, IgnoreCase, IgnoreBlanks, "")

    End Sub

    Private Sub Setup(ChildListProperty As IPropertyInfo, ByVal UniqueProperties() As String, IgnoreCase As Boolean, IgnoreBlanks As Boolean,
                      RuleBrokenDescription As String)

      Me.PrimaryProperty = ChildListProperty

      Dim ipl As New List(Of IPropertyInfo) From {ChildListProperty}

      Me.InputProperties = ipl
      Me.UniqueProperties = UniqueProperties

      For i = 0 To UniqueProperties.Length - 1
        RuleUri.AddQueryParameter("Unique_" & i, UniqueProperties(i))

      Next

      Me.RuleBrokenDescription = RuleBrokenDescription

      If String.IsNullOrWhiteSpace(Me.RuleBrokenDescription) Then
        Dim Properties As String = ""

        For i = 0 To Me.UniqueProperties.Length - 1
          Dim pi = GetType(Child).GetProperty(Me.UniqueProperties(i))
          If Properties = "" Then
            Properties = Singular.Reflection.GetDisplayName(pi)
          Else
            If i = Me.UniqueProperties.Length - 1 Then
              Properties &= " " & My.Resources.localstring.AndText & " " & Singular.Reflection.GetDisplayName(pi)
            Else
              Properties &= ", " & Singular.Reflection.GetDisplayName(pi)
            End If
          End If
        Next

        Dim piList = GetType(Parent).GetProperty(Me.PrimaryProperty.Name)
        Dim ListName = Singular.Reflection.GetDisplayName(piList)

        Me.RuleBrokenDescription = String.Format(My.Resources.localstring.PropertiesInChildListUnique, Properties, ListName)
      End If

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      Dim List As ChildList = context.InputPropertyValues(Me.PrimaryProperty)

      If List IsNot Nothing AndAlso List.Count > 1 Then
        Dim Match As Boolean = True

        For i As Integer = 0 To List.Count - 2
          For i2 As Integer = i + 1 To List.Count - 1
            Match = True
            For Each Field As String In Me.UniqueProperties

              Dim pi1 = GetType(Child).GetProperty(Field)
              Dim pi2 = GetType(Child).GetProperty(Field)

              Dim Value1 As Object = pi1.GetValue(List(i), Nothing)
              Dim Value2 As Object = pi2.GetValue(List(i2), Nothing)

              If Me.IgnoreCase Then
                If TypeOf Value1 Is String Then
                  Value1 = CStr(Value1).ToLower
                End If

                If TypeOf Value2 Is String Then
                  Value2 = CStr(Value2).ToLower
                End If
              End If

              Match = Match And Singular.Misc.CompareSafe(Value1, Value2) And _
                (Me.IgnoreBlanks And Not Singular.Misc.IsNullNothingOrEmpty(Value1) Or Not Me.IgnoreBlanks)
              If Not Match Then Exit For
            Next

            If Match Then

              context.AddErrorResult(Me.RuleBrokenDescription)

              Exit Sub
            End If
          Next
        Next
      End If

    End Sub

  End Class

#End Region

#Region " IP Address & Email Data Annotations"

  Public Class ValidIPAddress
    Inherits System.ComponentModel.DataAnnotations.RegularExpressionAttribute

    Public Sub New()

      MyBase.New(Singular.Misc.IPAddress.ValidIPAddressRegexString)

      Me.ErrorMessage = "Invalid IP Address"

    End Sub

  End Class

  Public Class ValidEmail
    Inherits System.ComponentModel.DataAnnotations.RegularExpressionAttribute

    Public Sub New()

      MyBase.New(Singular.Emails.ValidEmailRegexString)


      Me.ErrorMessage = "Invalid Email Address"

    End Sub

  End Class

#End Region

#Region " ChildListNotEmpty "

  Public Class ChildListNotEmpty
    Inherits BusinessRule

    Private mEmptyMessage As String

    Public Sub New(ByVal ChildListProperty As IPropertyInfo, ByVal EmptyMessage As String)

      MyBase.New(ChildListProperty)

      mEmptyMessage = EmptyMessage

      Setup(ChildListProperty)

    End Sub

    Private Sub Setup(ByVal ChildListProperty As IPropertyInfo)

      Me.PrimaryProperty = ChildListProperty

      Dim ipl As New List(Of IPropertyInfo) From {ChildListProperty}
      Me.InputProperties = ipl

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)
#If SILVERLIGHT Then
      Dim List As Object = context.InputPropertyValues(Me.PrimaryProperty)
#Else
      Dim List As IList = context.InputPropertyValues(Me.PrimaryProperty)
#End If

      If List IsNot Nothing AndAlso List.Count = 0 Then
        context.AddErrorResult(mEmptyMessage)
      End If

      context.Complete()

    End Sub

  End Class

#End Region

#Region " AtLeastOneValueSpecified "

  Public Class AtLeastOneValueSpecifiedRule
    Inherits BusinessRule

    Public Shared Sub AddAtLeastOneValueSpecifiedRule(Of C As SingularBusinessBase(Of C))(ToObject As C, BooleanNotSpecifiedValue As Boolean, ByVal RuleErrorDescription As String, ByVal ParamArray Properties() As IPropertyInfo)

      For Each prop In Properties
        Dim tmpProp = prop
        ToObject.BusinessRules.AddRule(New AtLeastOneValueSpecifiedRule(prop, BooleanNotSpecifiedValue, RuleErrorDescription, Properties.Where(Function(p) p IsNot tmpProp).ToArray))

        Array.ForEach(Properties, Sub(p)
                                    If p IsNot tmpProp Then
                                      ToObject.BusinessRules.AddRule(New Csla.Rules.CommonRules.Dependency(tmpProp, p))
                                    End If
                                  End Sub)
      Next

    End Sub

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, ByVal ParamArray OtherProperties() As IPropertyInfo)

      MyBase.New(PrimaryProperty)

      SetupRule(PrimaryProperty, OtherProperties)

    End Sub

    Public Property RuleErrorDescription As String = ""

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, ByVal RuleErrorDescription As String, ByVal ParamArray OtherProperties() As IPropertyInfo)

      MyBase.New(PrimaryProperty)

      SetupRule(PrimaryProperty, OtherProperties)

      Me.RuleErrorDescription = RuleErrorDescription

      RuleUri.AddQueryParameter("RuleErrorDescription", RuleErrorDescription.ToString())


    End Sub

    Public Property BooleanNotSpecifiedValue As Boolean?

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, BooleanNotSpecifiedValue As Boolean, ByVal ParamArray OtherProperties() As IPropertyInfo)

      MyBase.New(PrimaryProperty)

      SetupRule(PrimaryProperty, OtherProperties)

      Me.BooleanNotSpecifiedValue = BooleanNotSpecifiedValue

    End Sub

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, BooleanNotSpecifiedValue As Boolean, ByVal RuleErrorDescription As String, ByVal ParamArray OtherProperties() As IPropertyInfo)

      MyBase.New(PrimaryProperty)

      SetupRule(PrimaryProperty, OtherProperties)

      Me.RuleErrorDescription = RuleErrorDescription
      Me.BooleanNotSpecifiedValue = BooleanNotSpecifiedValue

      RuleUri.AddQueryParameter("RuleErrorDescription", RuleErrorDescription.ToString())
      RuleUri.AddQueryParameter("BooleanNotSpecifiedValue", BooleanNotSpecifiedValue.ToString())

    End Sub

    Private Sub SetupRule(ByVal PrimaryProperty As IPropertyInfo, ByVal ParamArray OtherProperties() As IPropertyInfo)

      Me.InputProperties = New List(Of IPropertyInfo) From {PrimaryProperty}

      For Each pi As IPropertyInfo In OtherProperties
        Me.InputProperties.Add(pi)
      Next

      For i = 1 To Me.InputProperties.Count - 1
        Me.AffectedProperties.Add(Me.InputProperties(i))
      Next

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      ' check if all null
      Dim ValuesSpecified As Integer = 0
      For Each pi In Me.InputProperties
        Dim input = context.InputPropertyValues(pi)
        If Not Singular.Misc.IsNullNothing(input) AndAlso (Not TypeOf input Is Boolean OrElse (Me.BooleanNotSpecifiedValue.HasValue AndAlso Me.BooleanNotSpecifiedValue <> input)) Then
          ValuesSpecified += 1
        End If
      Next

      If ValuesSpecified < 1 Then
        context.AddErrorResult(Me.GetRuleDescription)
      End If

    End Sub

    Private Function GetRuleDescription() As String

      If Me.RuleErrorDescription = "" Then
        Dim WordNumber As Integer = 1
        Dim ReturnString As String = ""

        For Each pi In (From pi2 In Me.InputProperties
                        Order By pi2.FriendlyName)
          If WordNumber = 1 Then
            ReturnString = "Either "
          ElseIf WordNumber = Me.InputProperties.Count - 2 Then
            ReturnString &= " or "
          Else
            ReturnString &= ", "
          End If
          ReturnString &= pi.FriendlyName

          WordNumber += 1
        Next

        ReturnString &= " must be specified"

        Return ReturnString
      Else
        Return RuleErrorDescription
      End If

    End Function

  End Class


#End Region

#Region " AtMostOneValueSpecified "

  Public Class AtMostOneValueSpecifiedRule
    Inherits BusinessRule

    Public Shared Sub AddAtMostOneValueSpecifiedRule(Of C As SingularBusinessBase(Of C))(ToObject As C, SetOtherPropertiesToNull As Boolean, BooleanNotSpecifiedValue As Boolean, ByVal RuleErrorDescription As String, ByVal ParamArray Properties() As IPropertyInfo)

      For Each prop In Properties
        Dim tmpProp = prop
        ToObject.BusinessRules.AddRule(New AtMostOneValueSpecifiedRule(prop, SetOtherPropertiesToNull, BooleanNotSpecifiedValue, RuleErrorDescription, Properties.Where(Function(p) p IsNot tmpProp).ToArray))

        Array.ForEach(Properties, Sub(p)
                                    If p IsNot tmpProp Then
                                      ToObject.BusinessRules.AddRule(New Csla.Rules.CommonRules.Dependency(tmpProp, p))
                                    End If
                                  End Sub)
      Next

    End Sub

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, SetOtherPropertiesToNull As Boolean, ByVal ParamArray OtherProperties() As IPropertyInfo)

      MyBase.New(PrimaryProperty)

      Me.RuleErrorDescription = RuleErrorDescription

      SetupRule(PrimaryProperty, SetOtherPropertiesToNull, OtherProperties)


    End Sub

    Public Property RuleErrorDescription As String = ""

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, SetOtherPropertiesToNull As Boolean, ByVal RuleErrorDescription As String, ByVal ParamArray OtherProperties() As IPropertyInfo)

      MyBase.New(PrimaryProperty)

      Me.RuleErrorDescription = RuleErrorDescription

      SetupRule(PrimaryProperty, SetOtherPropertiesToNull, OtherProperties)

    End Sub

    Public Property BooleanNotSpecifiedValue As Boolean?
    Public Property SetOtherPropertiesToNull As Boolean = False

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, SetOtherPropertiesToNull As Boolean, BooleanNotSpecifiedValue As Boolean, ByVal ParamArray OtherProperties() As IPropertyInfo)

      MyBase.New(PrimaryProperty)

      Me.BooleanNotSpecifiedValue = BooleanNotSpecifiedValue

      SetupRule(PrimaryProperty, SetOtherPropertiesToNull, OtherProperties)

    End Sub

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo, SetOtherPropertiesToNull As Boolean, BooleanNotSpecifiedValue As Boolean, ByVal RuleErrorDescription As String, ByVal ParamArray OtherProperties() As IPropertyInfo)

      MyBase.New(PrimaryProperty)

      Me.RuleErrorDescription = RuleErrorDescription
      Me.BooleanNotSpecifiedValue = BooleanNotSpecifiedValue

      SetupRule(PrimaryProperty, SetOtherPropertiesToNull, OtherProperties)

    End Sub

    Private Sub SetupRule(ByVal PrimaryProperty As IPropertyInfo, SetOtherPropertiesToNull As Boolean, ByVal ParamArray OtherProperties() As IPropertyInfo)

      Me.RuleErrorDescription = RuleErrorDescription
      Me.SetOtherPropertiesToNull = SetOtherPropertiesToNull

      For Each prop In OtherProperties
        Me.AffectedProperties.Add(prop)
      Next

      RuleUri.AddQueryParameter("RuleErrorDescription", RuleErrorDescription.ToString())
      RuleUri.AddQueryParameter("BooleanNotSpecifiedValue", BooleanNotSpecifiedValue.ToString())
      RuleUri.AddQueryParameter("SetOtherPropertiesToNull", BooleanNotSpecifiedValue.ToString())

      Me.InputProperties = New List(Of IPropertyInfo) From {PrimaryProperty}

      For Each pi As IPropertyInfo In OtherProperties
        Me.InputProperties.Add(pi)
      Next

      For i = 1 To Me.InputProperties.Count - 1
        Me.AffectedProperties.Add(Me.InputProperties(i))
      Next

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      ' check if all null
      Dim PrimaryPropertySpecified As Boolean = False
      Dim PrimaryPropertyValue = context.InputPropertyValues(Me.PrimaryProperty)
      Dim ValuesSpecified As Integer = 0
      If Not Singular.Misc.IsNullNothing(PrimaryPropertyValue) AndAlso (Not TypeOf PrimaryPropertyValue Is Boolean OrElse (Me.BooleanNotSpecifiedValue.HasValue AndAlso Me.BooleanNotSpecifiedValue <> PrimaryPropertyValue)) Then
        PrimaryPropertySpecified = True
        ValuesSpecified += 1
      End If
      For Each pi In Me.InputProperties
        If pi IsNot Me.PrimaryProperty Then
          Dim input = context.InputPropertyValues(pi)
          If Not Singular.Misc.IsNullNothing(input) AndAlso (Not TypeOf input Is Boolean OrElse (Me.BooleanNotSpecifiedValue.HasValue AndAlso Me.BooleanNotSpecifiedValue <> input)) Then
            ValuesSpecified += 1

            If PrimaryPropertySpecified Then
              context.AddOutValue(pi, Nothing)
            End If
          End If
        End If
      Next

      If ValuesSpecified > 1 AndAlso Not Me.SetOtherPropertiesToNull Then
        context.AddErrorResult(Me.RuleErrorDescription)
      End If

    End Sub

    Private Function GetRuleDescription() As String

      If Me.RuleErrorDescription = "" Then
        Dim WordNumber As Integer = 1
        Dim ReturnString As String = ""

        For Each pi In (From pi2 In Me.InputProperties
                        Order By pi2.FriendlyName)
          If WordNumber = 1 Then
            ReturnString = "Either "
          ElseIf WordNumber = Me.InputProperties.Count - 2 Then
            ReturnString &= " or "
          Else
            ReturnString &= ", "
          End If
          ReturnString &= pi.FriendlyName

          WordNumber += 1
        Next

        ReturnString &= " must be specified"

        Return ReturnString
      Else
        Return RuleErrorDescription
      End If

    End Function

  End Class


#End Region

#Region " Integer is not zero "

  Public Class IntegerIsNotZero
    Inherits BusinessRule

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo)

      MyBase.New(PrimaryProperty)
      Me.InputProperties = New List(Of IPropertyInfo) From {PrimaryProperty}

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      Dim input = context.InputPropertyValues(PrimaryProperty)
      Dim Number = CInt(input)
      If Number = 0 Then
        context.AddErrorResult(PrimaryProperty.FriendlyName & " cannot be zero ")
      End If

    End Sub

  End Class

#End Region

#Region " Decimal is not zero "

  Public Class DecimalIsNotZero
    Inherits BusinessRule

    Public Sub New(ByVal PrimaryProperty As IPropertyInfo)

      MyBase.New(PrimaryProperty)
      Me.InputProperties = New List(Of IPropertyInfo) From {PrimaryProperty}

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      Dim input = context.InputPropertyValues(PrimaryProperty)
      Dim Number = CDec(input)
      If Number = 0 Then
        context.AddErrorResult(PrimaryProperty.FriendlyName & " cannot be zero ")
      End If

    End Sub

  End Class

#End Region

#Region " Email Address Valid "

  Public Class EmailAddressValid
    Inherits BusinessRule

    Public Sub New(ByVal PrimaryProperty As Csla.Core.IPropertyInfo)

      MyBase.New(PrimaryProperty)

      Me.InputProperties = New List(Of Csla.Core.IPropertyInfo) From {PrimaryProperty}

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      Dim EmailAddress = context.InputPropertyValues(PrimaryProperty)

      Dim errorStr As String = ""
      If Not Singular.Emails.ValidEmailAddress(EmailAddress) Then
        context.AddErrorResult(PrimaryProperty.FriendlyName & " is not a valid email address")
      End If

    End Sub

  End Class

#End Region

#Region " Image Data Size "


  Public Class ImageDataSize
    Inherits BusinessRule

    Public Property MaxByteLength As Integer

    Public Sub New(ByVal ImageDataProperty As Csla.PropertyInfo(Of Byte()), MaxByteLength As Integer)

      MyBase.New(ImageDataProperty)
      Me.InputProperties = New List(Of IPropertyInfo) From {ImageDataProperty}

      Me.MaxByteLength = MaxByteLength

      RuleUri.AddQueryParameter("MaxByteLength", MaxByteLength.ToString())

    End Sub

    Protected Overrides Sub Execute(ByVal context As Csla.Rules.RuleContext)

      Dim input = TryCast(context.InputPropertyValues(PrimaryProperty), Byte())

      If input IsNot Nothing AndAlso input.Length > Me.MaxByteLength Then
        context.AddErrorResult(String.Format(CType(Me.PrimaryProperty, Csla.PropertyInfo(Of Byte())).FriendlyName & " cannot be bigger than {0} kb, current size {1} kb", MaxByteLength / 1000, input.Length / 1000))
      End If

    End Sub

  End Class

#End Region

#Region " Required / String Length overrides "

  Public Class LocalisedErrorHelper

    Private mMember As PropertyInfo
    Private mLocalisedDisplay As Singular.DataAnnotations.LocalisedDisplay
    Private mDisplayName As String
    Private mAttribute As System.ComponentModel.DataAnnotations.ValidationAttribute

    Public Sub New(Attribute As System.ComponentModel.DataAnnotations.ValidationAttribute, ValidationContext As ComponentModel.DataAnnotations.ValidationContext)
      mAttribute = Attribute
      mMember = Singular.Reflection.GetProperty(ValidationContext.ObjectType, ValidationContext.MemberName)
      If mMember IsNot Nothing Then
        mLocalisedDisplay = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.LocalisedDisplay)(mMember)
      End If
      If mLocalisedDisplay Is Nothing Then
        mDisplayName = Singular.Reflection.GetDisplayName(mMember)
      End If
    End Sub

    Public Function GetError(EnglishRule As String, GenericKey As String, ParamArray Args() As String) As String

      If mAttribute.ErrorMessageResourceName <> "" Then
        'Programmer has supplied custom error string key.
#If SILVERLIGHT Then
        Return EnglishRule
#Else
        Return Singular.Localisation.LocalText(mAttribute.ErrorMessageResourceName)
#End If
      ElseIf mAttribute.ErrorMessage <> "" Then
        'Programmer has supplied custom english error.
        Return mAttribute.ErrorMessage
      Else

        If mLocalisedDisplay Is Nothing Then
          'No localised display attribute on this property, so return a generic english message.
          Return String.Format("{0} is required.", mDisplayName)
        Else

#If SILVERLIGHT Then
          Return EnglishRule
#Else
           Dim Params(0) As String
          If Args.Length > 0 Then
            'Need to create a new array because the local display value of the property must be the first item.
            ReDim Params(Args.Length)
            Array.ConstrainedCopy(Args, 0, Params, 1, Args.Length)
          End If
          Params(0) = Singular.Localisation.LocalText(mLocalisedDisplay.ResourceName)

          Return Singular.Localisation.LocalText(GenericKey, Params)
#End If
       
        End If

      End If

    End Function

  End Class

  <Obsolete("Use System.ComponentModel.DataAnnotations.Required instead, there is funny behaviour with this one and localisation")>
  Public Class RequiredAttribute
    Inherits System.ComponentModel.DataAnnotations.RequiredAttribute

    Private mLRH As LocalisedErrorHelper

    Public Overrides Function FormatErrorMessage(name As String) As String

      Return mLRH.GetError("{0} is required.", "Rule_Required")

    End Function

    Protected Overrides Function IsValid(value As Object, validationContext As ComponentModel.DataAnnotations.ValidationContext) As ComponentModel.DataAnnotations.ValidationResult
      mLRH = New LocalisedErrorHelper(Me, validationContext)
      Return MyBase.IsValid(value, validationContext)
    End Function

  End Class

  Public Class StringLengthAttribute
    Inherits System.ComponentModel.DataAnnotations.StringLengthAttribute

    Private mLRH As LocalisedErrorHelper

    Public Sub New(MaximumLength As Integer)
      MyBase.New(MaximumLength)
    End Sub

    Public Overrides Function FormatErrorMessage(name As String) As String

      If Me.MinimumLength > 0 Then
        If Me.MinimumLength = Me.MaximumLength Then
          Return mLRH.GetError("{0} must be exactly {1} characters.", "Rule_StringLength_Min", MinimumLength, MaximumLength)
        Else
          Return mLRH.GetError("{0} must be at between {1} and {2} characters.", "Rule_StringLength_Min", MinimumLength, MaximumLength)
        End If
      Else
        Return mLRH.GetError("{0} must be less than {1} characters.", "Rule_StringLength", MaximumLength)
      End If

    End Function

    Protected Overrides Function IsValid(value As Object, validationContext As ComponentModel.DataAnnotations.ValidationContext) As ComponentModel.DataAnnotations.ValidationResult
      mLRH = New LocalisedErrorHelper(Me, validationContext)
      Return MyBase.IsValid(value, validationContext)
    End Function

  End Class

#End Region

#If SILVERLIGHT Then
#Else

  Public Class PasswordComplexity(Of C As Singular.SingularBusinessBase(Of C))
    Inherits Singular.Rules.JavascriptRule(Of C)

    Public Sub New(PasswordProperty As Csla.Core.IPropertyInfo, MinLength As Integer, LowerCase As Boolean, UpperCase As Boolean, Number As Boolean, Special As Boolean, NumRequired As Integer)
      MyBase.New(PasswordProperty, RuleSeverity.Error)

      ServerRuleFunction = Function(i)

                             Dim pc As New Singular.Misc.Password.PasswordChecker(MinLength, LowerCase, UpperCase, Number, Special, NumRequired)
                             If Not pc.CheckPassword(CurrentContext.InputPropertyValues(PasswordProperty)) Then
                               Return pc.ErrorMessage
                             End If

                             Return ""

                           End Function

      JavascriptRuleCode = "var pwd = CtlError.Object." & PasswordProperty.Name & "(); if (pwd.length < " & MinLength & ") { CtlError.AddError('Password must be at least " & MinLength & " characters.') }"

    End Sub



  End Class

#End If

End Namespace