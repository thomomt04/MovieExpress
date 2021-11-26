Imports System.Linq.Expressions

Namespace Linq

  Public Enum OutputModeType
    'None = 0
    GetProperty = 1
    KnockoutBinding = 2
  End Enum

  Public Enum EqualsActionType
    Compare = 1
    SetValue = 2
  End Enum

  Public Enum PropertyAccessType
    AutoDetect = 1
    KOObservable = 2
    NormalField = 3
    PlainJSon = 4
  End Enum

  Public Interface IExpressionParser

  End Interface

  Public Class JavascriptExpressionOutput
    Public Property JavascriptString As String = ""
    Public Property MemberInfo As System.Reflection.MemberInfo
    Public Property DataPath As String = ""
  End Class

  Public Class ExpressionHelpers

    Public Shared Function RemoveReturnType(Of T, R)(StrongTypedExpression As Expression(Of Func(Of T, R))) As Expression(Of Func(Of T, Object))

      Return Expression.Lambda(Of Func(Of T, Object))(StrongTypedExpression, StrongTypedExpression.Parameters)

    End Function

  End Class

  <Serializable()>
  Public Class ExpressionParser(Of ObjectType)
    Implements IExpressionParser

    Public Property FoundMembers As New List(Of System.Reflection.MemberInfo)

    <NonSerialized()> Protected mExpression As LambdaExpression

    Public ReadOnly Property Expression As LambdaExpression
      Get
        Return mExpression
      End Get
    End Property

    <NonSerialized()> Protected GetMethod As Func(Of ObjectType, Object)

    Public Function GetValue(Instance As ObjectType) As Object
      Try
        If GetMethod Is Nothing Then
          GetMethod = mExpression.Compile
        End If
        Return GetMethod.Invoke(Instance)
      Catch ex As Exception
        Return Nothing
      End Try
    End Function

    Public Sub New(Expression As LambdaExpression)
      Me.mExpression = Expression
    End Sub

    Public Overridable Sub Parse()
      ParseExpression(mExpression.Body)
    End Sub

    'Get the base expression, ignoreing lambda and unary expressions.
    Public Shared Function GetBaseExpression(expr As Expression) As Expression
      If TypeOf expr Is LambdaExpression Then
        Return GetBaseExpression(DirectCast(expr, LambdaExpression).Body)
      ElseIf TypeOf expr Is UnaryExpression Then
        Return GetBaseExpression(DirectCast(expr, UnaryExpression).Operand)
      Else
        Return expr
      End If
    End Function

    Protected _MemberExpressionDepth As Integer

    Protected Sub ParseExpression(expr As Expression)

      If expr IsNot Nothing Then

        If TypeOf expr Is BinaryExpression Then
          ParseBinaryExpression(expr)
        ElseIf TypeOf expr Is UnaryExpression Then
          ParseUnaryExpression(expr)
        ElseIf TypeOf expr Is MemberExpression Then
          _MemberExpressionDepth += 1
          ParseMemberExpression(expr)
          _MemberExpressionDepth -= 1
        ElseIf TypeOf expr Is ConstantExpression Then
          ParseConstantExpression(expr)
        ElseIf TypeOf expr Is MethodCallExpression Then
          ParseMethodCallExpression(expr)
        ElseIf TypeOf expr Is ConditionalExpression Then
          ParseConditionalExpression(expr)
        ElseIf TypeOf expr Is NewArrayExpression Then
          ParseNewArrayExpression(expr)
        ElseIf TypeOf expr Is ParameterExpression Then
          ParseParameterExpression(expr)
        ElseIf TypeOf expr Is LambdaExpression Then
          ParseExpression(DirectCast(expr, LambdaExpression).Body)
        ElseIf TypeOf expr Is NewExpression Then
          ParseNewExpression(expr)
        Else
          Throw New Exception("Expression Parser: Cannot parse expression of type: " & expr.GetType.Name)
        End If
      End If

    End Sub

    Protected Overridable Sub ParseBinaryExpression(expr As BinaryExpression)

      ParseExpression(expr.Left)
      ParseExpression(expr.Right)

    End Sub

    Protected Overridable Sub ParseUnaryExpression(expr As UnaryExpression)

      ParseExpression(expr.Operand)

    End Sub

    Protected Overridable Sub ParseMemberExpression(ByVal expr As MemberExpression)

      Dim Path As String = ""

      While expr.Expression IsNot Nothing

        If expr.Expression.NodeType = Expressions.ExpressionType.MemberAccess Then
          expr = expr.Expression
        ElseIf expr.Expression.NodeType = Expressions.ExpressionType.Call Then
          Exit While
        Else
          Exit While
        End If

      End While

      FoundMembers.Add(expr.Member)

    End Sub

    Protected Overridable Sub ParseParameterExpression(ByVal expr As ParameterExpression)

    End Sub

    Protected Overridable Sub ParseConstantExpression(expr As ConstantExpression)

    End Sub

    Protected Overridable Sub ParseMethodCallExpression(expr As MethodCallExpression)

      For Each arg As Expressions.Expression In expr.Arguments
        ParseExpression(arg)
      Next

    End Sub

    Protected Overridable Sub ParseConditionalExpression(expr As ConditionalExpression)

      ParseExpression(expr.Test)
      ParseExpression(expr.IfTrue)
      ParseExpression(expr.IfFalse)

    End Sub

    Protected Overridable Sub ParseNewArrayExpression(expr As NewArrayExpression)

      For Each c In expr.Expressions
        ParseExpression(c)
      Next

    End Sub

    Protected Overridable Sub ParseNewExpression(expr As NewExpression)

    End Sub

  End Class

  <Serializable()>
  Public Class JavascriptExpressionParser(Of ObjectType)
    Inherits Singular.Linq.ExpressionParser(Of ObjectType)

    Public Class InvalidPropertyException
      Inherits Exception

      Private mPropertyName As String = ""
      Public ReadOnly Property PropertyName As String
        Get
          Return mPropertyName
        End Get
      End Property
      Public Sub New(PropertyName As String)
        mPropertyName = PropertyName
      End Sub
    End Class

    Public Sub New(Expression As LambdaExpression, OutputMode As OutputModeType)
      MyBase.New(Expression)
      JSWriter = New Singular.Web.Utilities.JavaScriptWriter

      mOutputMode = OutputMode
      If mOutputMode <> OutputModeType.KnockoutBinding Then
        mPropertyAccessMode = PropertyAccessType.NormalField
      End If
    End Sub

    Private Shared CachedExpressions As New Hashtable

    Public Shared Function GetCachedOutput(Expression As Expression(Of System.Func(Of ObjectType, Object)),
                                            OutputMode As OutputModeType,
                                            CheckBrowsable As Boolean,
                                            Optional EqualsAction As EqualsActionType = EqualsActionType.Compare,
                                            Optional PropertyAccessMode As PropertyAccessType = PropertyAccessType.AutoDetect) As JavascriptExpressionOutput
      Dim Key = Expression.Body.ToString & CInt(EqualsAction) & CInt(OutputMode) & CInt(PropertyAccessMode) & Singular.Localisation.CurrentCulture.TwoLetterISOLanguageName
      Dim Output As JavascriptExpressionOutput = CachedExpressions(Key)
      If Output Is Nothing Then
        'Create the binding expression and parse it
        Dim je As New JavascriptExpressionParser(Of ObjectType)(Expression, OutputMode)
        je.CheckBrowsable = CheckBrowsable
        je.EqualsAction = EqualsAction
        je.PropertyAccessMode = PropertyAccessMode
        je.Parse()

        'Store the results
        Output = New JavascriptExpressionOutput With {.JavascriptString = je.ToString}
        If je.FoundMembers.Count > 0 Then
          Output.MemberInfo = je.FoundMembers(0)
          Output.DataPath = je.FoundPath
        End If
        CachedExpressions(Key) = Output
      End If

      Return Output

    End Function

    Private mOutputMode As OutputModeType
    Private mWrapDates As Boolean = False

    Public Property IncludeReturnStatement As Boolean = True

    Public Property EqualsAction As EqualsActionType = EqualsActionType.Compare

    Private mPropertyAccessMode As PropertyAccessType = PropertyAccessType.AutoDetect
    ''' <summary>
    ''' If set to True, renders any properties without the () at the end, so that knockout can bind to the observable, not the value.
    ''' </summary>
    Public Property PropertyAccessMode As PropertyAccessType
      Get
        Return mPropertyAccessMode
      End Get
      Set(value As PropertyAccessType)
        If mPropertyAccessMode = PropertyAccessType.AutoDetect Then
          mPropertyAccessMode = value
        End If
      End Set
    End Property

    Private Sub SetPropertyAccessMode()
      If mPropertyAccessMode = PropertyAccessType.AutoDetect Then
        mPropertyAccessMode = PropertyAccessType.NormalField
      End If
    End Sub

    ''' <summary>
    ''' If set to True, and property used in an expression that is not browsable according to Singular.Reflection.AutoGenerateField will cause an exception to be raised.
    ''' </summary>
    Public Property CheckBrowsable As Boolean = False

    <NonSerialized()> Protected JSWriter As Singular.Web.Utilities.JavaScriptWriter
    'Public ReadOnly Property JSWriter As Singular.Web.Utilities.JavaScriptWriter
    '  Get
    '    If mJSWriter Is Nothing Then
    '      mJSWriter = New Singular.Web.Utilities.JavaScriptWriter
    '    End If
    '    Return mJSWriter
    '  End Get
    'End Property

    Private mFoundPath As String = ""
    Public ReadOnly Property FoundPath As String
      Get
        Return mFoundPath
      End Get
    End Property

    Public Overrides Sub Parse()

      If mOutputMode = OutputModeType.GetProperty AndAlso IncludeReturnStatement Then
        JSWriter.Write("return ", False)
      End If

      ParseExpression(mExpression.Body)

      If mOutputMode <> OutputModeType.KnockoutBinding AndAlso IncludeReturnStatement Then
        JSWriter.Write(";", False)
      End If

    End Sub

    Private mLastNodeType As ExpressionType
    Private mInSetBlock As Boolean = False
    Private mSkipBinaryPart2 As Boolean = False

    Protected Overrides Sub ParseBinaryExpression(expr As BinaryExpression)
      mLastNodeType = expr.NodeType
      SetPropertyAccessMode()

      If expr.NodeType = ExpressionType.ExclusiveOr Then
        'Javascript doesnt have an XOR operator, call the lib function instead.
        JSWriter.RawWrite("XOR(")
      Else
        JSWriter.RawWrite("(")
      End If

      mInSetBlock = True
      ParseExpression(expr.Left)

      Dim SkipRight As Boolean = False

      If Not mSkipBinaryPart2 Then

        Select Case expr.NodeType
          Case ExpressionType.Multiply, ExpressionType.MultiplyChecked
            JSWriter.RawWrite(" * ")
          Case Expressions.ExpressionType.Add, Expressions.ExpressionType.AddChecked
            JSWriter.RawWrite(" + ")
          Case Expressions.ExpressionType.Subtract, Expressions.ExpressionType.SubtractChecked
            JSWriter.RawWrite(" - ")
          Case Expressions.ExpressionType.Divide
            JSWriter.RawWrite(" / ")
          Case Expressions.ExpressionType.Equal
            If EqualsAction = EqualsActionType.SetValue Then
              JSWriter.RawWrite("(") 'In knockout you set a properties value by passing the value into the observable. e.g. self.FirstName('Bob') not self.FirstName = 'Bob'
            Else
              JSWriter.RawWrite(" == ")
            End If
          Case Expressions.ExpressionType.NotEqual
            JSWriter.RawWrite(" != ")
          Case Expressions.ExpressionType.Modulo
            JSWriter.RawWrite(" % ")
          Case Expressions.ExpressionType.GreaterThan
            JSWriter.RawWrite(" > ")
          Case Expressions.ExpressionType.LessThan
            JSWriter.RawWrite(" < ")
          Case Expressions.ExpressionType.GreaterThanOrEqual
            JSWriter.RawWrite(" >= ")
          Case Expressions.ExpressionType.LessThanOrEqual
            JSWriter.RawWrite(" <= ")
          Case Expressions.ExpressionType.And, Expressions.ExpressionType.AndAlso
            JSWriter.RawWrite(" && ")
          Case Expressions.ExpressionType.Or, Expressions.ExpressionType.OrElse, Expressions.ExpressionType.Coalesce
            JSWriter.RawWrite(" || ")
          Case ExpressionType.ExclusiveOr
            JSWriter.RawWrite(", ")

          Case Else
            Throw New Exception("Expression Parser: Unknown operator: " & expr.NodeType.ToString)
        End Select

        Dim ExtraBracket As Boolean = EqualsAction = EqualsActionType.SetValue
        EqualsAction = EqualsActionType.Compare

        If Not SkipRight Then
          ParseExpression(expr.Right)
        End If

        If ExtraBracket Then
          JSWriter.RawWrite(")")
        End If

        mInSetBlock = False

      End If
      mSkipBinaryPart2 = False

      JSWriter.RawWrite(")")
    End Sub

    Protected Overrides Sub ParseUnaryExpression(expr As UnaryExpression)

      If expr.NodeType = Expressions.ExpressionType.Not Then
        JSWriter.RawWrite("!(")
        SetPropertyAccessMode()
      End If

      If expr.NodeType = Expressions.ExpressionType.NegateChecked OrElse expr.NodeType = Expressions.ExpressionType.Negate Then
        JSWriter.RawWrite("-")
      End If

      ParseExpression(expr.Operand)

      If expr.NodeType = Expressions.ExpressionType.Not Then
        JSWriter.RawWrite(")")
      End If

    End Sub

    Private mPathToWrite As String = ""
    Protected Overrides Sub ParseMemberExpression(ByVal expr As MemberExpression)

      If expr.Type Is DBNull.Value.GetType Then
        'DBNULL Special Case
        JSWriter.RawWrite("null")

      ElseIf TypeOf expr.Member Is System.Reflection.FieldInfo Then

        'Weird cases
        Dim fi As System.Reflection.FieldInfo = expr.Member
        If fi.IsInitOnly Then
          If fi.Name = "One" Then JSWriter.RawWrite("1")
          If fi.Name = "Zero" Then JSWriter.RawWrite("0")
          If fi.Name = "MinusOne" Then JSWriter.RawWrite("-1")
          If fi.Name = "Empty" Then JSWriter.WriteJSEncoded("")
        End If

      Else

        FoundMembers.Add(expr.Member)

        If (expr.Member.Name = "Model" OrElse expr.Member.Name = "ViewModel" OrElse expr.Member.Name = "Root") AndAlso
          (GetType(System.Web.UI.Page).IsAssignableFrom(expr.Member.ReflectedType) OrElse GetType(System.Web.UI.Control).IsAssignableFrom(expr.Member.ReflectedType) OrElse GetType(Singular.Web.IBindingRoot).IsAssignableFrom(expr.Member.ReflectedType)) Then
          'Root special case
          JSWriter.RawWrite("$root")
          mPathToWrite = "$root"
        Else

          'Only leave the first one
          Dim LeaveThisObservable As Boolean = PropertyAccessMode <> PropertyAccessType.NormalField

          If PropertyAccessMode <> PropertyAccessType.PlainJSon Then
            PropertyAccessMode = PropertyAccessType.NormalField
          End If


          Dim HasPath As Boolean = False
          If expr.Expression IsNot Nothing Then
            'If the expression has a path, e.g. c.User.FirstName
            ParseExpression(expr.Expression)

            'TODO: this should check if $data was ommitted in the above ParseExpression. If so a period must not be added. the logic below is duplicated in ParseParameterExpression.
            If mForceParamName OrElse Not TypeOf expr.Expression Is ParameterExpression OrElse mOutputMode = OutputModeType.GetProperty Then
              HasPath = True
            End If

          End If

          Dim Name As String = expr.Member.Name

          If Singular.Misc.InSafe(Name, "Count", "Length") Then
            'JS Array uses length
            If Singular.ReflectionCached.GetCachedType(expr.Member.DeclaringType).IsEnumerable Then
              Name = "length"
            End If

          ElseIf Name.ToLower = "now" Then
            'Current date in JS
            WrapIfDate()
            Name = "new Date()"

          ElseIf Name.ToLower = "date" AndAlso expr.Member.DeclaringType Is GetType(Date) Then
            Name = "Date()"

          ElseIf Name = "HasValue" AndAlso expr.Member.DeclaringType.IsGenericType Then
            HasPath = False
            Name = ""
          Else


            If CheckBrowsable Then
              If TypeOf expr.Member Is System.Reflection.MethodInfo Then
                Throw New InvalidPropertyException(expr.Member.Name & " is a function, can only bind to properties.")
              ElseIf TypeOf expr.Member Is System.Reflection.PropertyInfo Then

                If Not Singular.Misc.InSafe(Name, "Guid", "IsNew", "IsClientNew", "IsDirty", "IsSelfDirty", "IsValid", "IsSelfValid", "IsBusy", "IsSelfBusy") AndAlso
                  Not Singular.ReflectionCached.SerlialiseField(expr.Member, True) Then

                  Throw New InvalidPropertyException(expr.Member.Name & " is not browsable")

                End If
              End If
            End If

            If LeaveThisObservable OrElse (EqualsAction = EqualsActionType.SetValue AndAlso mInSetBlock) Then

            Else
              Name &= "()"
            End If

          End If

          'Dim RawDataOnly = Singular.Reflection.GetAttribute(Of Singular.DataAnnotations.RawDataOnly)(expr.Member) IsNot Nothing

          If HasPath Then
            JSWriter.RawWrite(".")
          End If

          'If RawDataOnly Then
          'JSWriter.RawWrite("ClientData.ViewModel." & Name)
          'Else
          JSWriter.RawWrite(Name)
          'End If


          If mWrapDates AndAlso _MemberExpressionDepth = 1 Then
            JSWriter.RawWrite(")")
          End If
          'Else
          mFoundPath = mPathToWrite
          If HasPath Then
            mPathToWrite &= "."
          End If
          mPathToWrite &= Name



        End If

      End If

    End Sub

    Private mIncludeDataParam As Boolean = False
    Private mForceParamName As Boolean = False

    Protected Overrides Sub ParseParameterExpression(ByVal expr As ParameterExpression)
      WrapIfDate()
      If mForceParamName Then
        JSWriter.RawWrite(expr.Name)
      ElseIf mOutputMode = OutputModeType.GetProperty Then
        JSWriter.RawWrite("self")
      ElseIf mIncludeDataParam OrElse _MemberExpressionDepth = 0 Then
        JSWriter.RawWrite("$data")
      End If
    End Sub

    Private Sub WrapIfDate()
      If mWrapDates Then
        JSWriter.RawWrite("GetDTime(")
      End If
    End Sub

    Private Function GetMember(expr As Expression) As System.Reflection.MemberInfo
      If TypeOf expr Is MemberExpression Then
        Return CType(expr, MemberExpression).Member
      ElseIf TypeOf expr Is UnaryExpression Then
        Return GetMember(CType(expr, UnaryExpression).Operand)
      ElseIf TypeOf expr Is LambdaExpression Then
        Return GetMember(CType(expr, LambdaExpression).Body)
      ElseIf TypeOf expr Is MethodCallExpression Then
        Return CType(expr, MethodCallExpression).Method
      End If
      Return Nothing
    End Function

    Protected Overrides Sub ParseConstantExpression(expr As ConstantExpression)

      If expr.Value Is Nothing OrElse expr.Value Is DBNull.Value Then
        JSWriter.RawWrite("null")
      ElseIf TypeOf expr.Value Is String Then
        JSWriter.WriteJSEncoded(expr.Value)
      ElseIf TypeOf expr.Value Is Boolean Then
        JSWriter.RawWrite(expr.Value.ToString.ToLower)
      ElseIf TypeOf expr.Value Is Decimal Then
        JSWriter.RawWrite(CDec(expr.Value).ToString(System.Globalization.CultureInfo.InvariantCulture))
      ElseIf TypeOf expr.Value Is Double Then
        JSWriter.RawWrite(CDbl(expr.Value).ToString(System.Globalization.CultureInfo.InvariantCulture))
      Else
        JSWriter.RawWrite(expr.Value)
      End If

    End Sub

    Protected Overrides Sub ParseNewArrayExpression(expr As NewArrayExpression)
      SetPropertyAccessMode()

      JSWriter.RawWrite("[")
      Dim First As Boolean = True
      For Each c In expr.Expressions
        If First Then
          First = False
        Else
          JSWriter.RawWrite(",")
        End If
        ParseExpression(c)
      Next
      JSWriter.RawWrite("]")

    End Sub

    Protected Overrides Sub ParseNewExpression(expr As NewExpression)

      'Compiler sometimes changes a constant e.g. 12 to 'new Decimal(12)', or worse 0.7 to new Decimal(7, 0, 0, false, ...)'
      If expr.Type Is GetType(Decimal) Then
        Dim Value As Decimal
        If expr.Arguments.Count = 5 Then
          Value = New Decimal(CType(expr.Arguments(0), ConstantExpression).Value,
                                   CType(expr.Arguments(1), ConstantExpression).Value,
                                   CType(expr.Arguments(2), ConstantExpression).Value,
                                   CType(expr.Arguments(3), ConstantExpression).Value,
                                   CType(expr.Arguments(4), ConstantExpression).Value)

        ElseIf expr.Arguments.Count = 1 Then
          'Value = CType(expr.Arguments(0), ConstantExpression).Value
          ParseExpression(expr.Arguments(0))
          Exit Sub
        End If

        JSWriter.RawWrite(Value)
      End If

    End Sub

    Protected Overrides Sub ParseMethodCallExpression(expr As MethodCallExpression)
      SetPropertyAccessMode()

      Dim MethodName As String = expr.Method.Name.ToLower

      Select Case MethodName

        Case "sum"
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(".Sum('" & GetMember(expr.Arguments(1)).Name & "')")

        Case "where"
          If expr.Arguments.Count = 2 Then
            ParseExpression(expr.Arguments(0))

            Dim Filter As LambdaExpression = expr.Arguments(1)
            Dim Parameter As String = ""
            If Filter.Parameters.Count > 0 Then Parameter = Filter.Parameters(0).Name

            JSWriter.RawWrite(".Where(function(" & Parameter & ") { return ")
            mForceParamName = True
            ParseExpression(Filter)
            mForceParamName = False
            JSWriter.RawWrite("})")
          End If

        Case "count"
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(".length")

        Case "abs"
          JSWriter.RawWrite(FunctionNameMapping(MethodName) & "(")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(")")

        Case "concat", "concatenateobject"
          Dim Args As Object = expr.Arguments
          If expr.Arguments.Count = 1 AndAlso TypeOf expr.Arguments(0) Is NewArrayExpression Then
            Args = CType(expr.Arguments(0), NewArrayExpression).Expressions
          End If
          For i As Integer = 0 To Args.Count - 1
            If i > 0 Then
              JSWriter.RawWrite(" + ")
            End If
            ParseExpression(Args(i))
          Next

        Case "get_chars"

          If expr.Arguments.Count = 1 AndAlso expr.Arguments(0).NodeType = ExpressionType.ConvertChecked Then
            'Calling Tostring on a Nullable type converts to Obj.ToString.GetChars(ConvertChecked('formatString'))
            ParseToString(CType(expr.Object, MethodCallExpression), expr.Arguments)

          Else
            'When you index a string, e.g. 'Hello'(0)
            ParseExpression(expr.Object)
            JSWriter.RawWrite("[")
            ParseExpression(expr.Arguments(0))
            JSWriter.RawWrite("]")
          End If

        Case "get_now"
          JSWriter.RawWrite("new Date()")

        Case "tolower", "toupper"
          ParseExpression(expr.Object)
          JSWriter.Write("." & FunctionNameMapping(MethodName) & "()")

        Case "startswith"
          ParseExpression(expr.Object)
          JSWriter.RawWrite(".startsWith(")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(")")

        Case "contains"
          ParseExpression(expr.Object)
          JSWriter.RawWrite(".indexOf(")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(") >= 0")

        Case "comparestring"
          ParseExpression(expr.Arguments(0))
          'A function like: function(c) c.Test = 'some text' will parse as comparestring('some text', 'c.Test) == 0
          'Which is why we have to do this.
          Dim ExtraBracket As Boolean = False
          If EqualsAction = EqualsActionType.SetValue Then
            JSWriter.RawWrite("(")
            ExtraBracket = True
            EqualsAction = EqualsActionType.Compare
          Else
            If mLastNodeType = Expressions.ExpressionType.Equal Then
              JSWriter.RawWrite(" == ")
            Else
              JSWriter.RawWrite(" != ")
            End If

          End If
          ParseExpression(expr.Arguments(1))
          If ExtraBracket Then
            JSWriter.RawWrite(")")
          End If
          mSkipBinaryPart2 = True

        Case "tostring"
          'Check the type of property
          ParseToString(expr, expr.Arguments)

        Case "tostringhelper"
          JSWriter.RawWrite("ToStringHelper(self, ")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(", ")
          ParseExpression(expr.Arguments(1))
          JSWriter.RawWrite(")")

        Case "get_item"
          ParseExpression(expr.Object)
          JSWriter.RawWrite("[")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite("]")


        Case "compareobjectequal", "conditionalcompareobjectequal"
          ParseBinaryExpression(BinaryExpression.Equal(expr.Arguments(0), expr.Arguments(1)))
        Case "compareobjectnotequal"
          ParseBinaryExpression(BinaryExpression.NotEqual(expr.Arguments(0), expr.Arguments(1)))

        Case "format"
          If expr.Arguments.Count > 0 AndAlso expr.Arguments(0).Type Is GetType(String) Then
            ParseExpression(expr.Arguments(0))
            JSWriter.RawWrite(".format")
            WriteArguments(expr.Arguments, 1)
          End If

        Case "localtext"
          Dim Key As String = CType(expr.Arguments(0), ConstantExpression).Value
          ParseConstantExpression(Expressions.Expression.Constant(Singular.Localisation.LocalText(Key)))
          If expr.Arguments.Count = 2 Then
            JSWriter.RawWrite(".format(")
            ParseExpression(expr.Arguments(1))
            JSWriter.RawWrite(")")
          End If

        Case "round"
          JSWriter.RawWrite("parseFloat(")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(".toFixed(")
          If expr.Arguments.Count > 1 Then
            ParseExpression(expr.Arguments(1))
            JSWriter.RawWrite(")")
          Else
            JSWriter.RawWrite("2)")
          End If
          JSWriter.RawWrite(")")

        Case "substring"
          ParseExpression(expr.Object)
          JSWriter.RawWrite(".substr")
          WriteArguments(expr.Arguments)

        Case "getparent"
          If expr.Object IsNot Nothing Then
            mIncludeDataParam = True
            ParseExpression(expr.Object)
            mIncludeDataParam = False
          End If
          JSWriter.RawWrite(".GetParent()")

        Case "isnullnothing", "isnullnothingorempty"
          JSWriter.RawWrite("!(")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(")")

        Case "safecompare"
          If expr.Method.DeclaringType Is GetType(Singular.Dates) Then
            mWrapDates = True
            ParseExpression(expr.Arguments(0))
            mWrapDates = False
          End If

        Case "in"
          If expr.Method.DeclaringType Is GetType(Singular.Misc) Then
            ParseExpression(expr.Arguments(1))
            JSWriter.RawWrite(".indexOf(")
            ParseExpression(expr.Arguments(0))
            JSWriter.RawWrite(") >= 0")
          End If

        Case "take"
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(".slice(0, ")
          ParseExpression(expr.Arguments(1))
          JSWriter.RawWrite(")")

        Case "last"
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(".Last()")

        Case "isnullorempty"
          JSWriter.RawWrite("!(")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(")")

        Case "negate"
          JSWriter.RawWrite("-(")
          ParseExpression(expr.Arguments(0))
          JSWriter.RawWrite(")")

        Case "op_explicit"
          ParseExpression(expr.Arguments(0))

        Case "getvalueordefault"
          If Nullable.GetUnderlyingType(expr.Object.Type) IsNot Nothing Then
            ParseExpression(expr.Object)
          End If
        Case Else


          'Check if its a defines JSFunction
          If expr.Method.ReflectedType Is GetType(Singular.Web.JSfunctions) Then

            If Singular.Reflection.GetAttribute(Of Singular.Web.SingularJSFunction)(expr.Method) IsNot Nothing Then
              JSWriter.RawWrite("Singular.")
            End If

            JSWriter.RawWrite(expr.Method.Name)
            WriteArguments(expr.Arguments)


          ElseIf MethodName.StartsWith("get_") Then
            'Actually a property get access.

            JSWriter.Write(expr.Method.Name.Substring(4) & "()")

          ElseIf expr.Method.DeclaringType Is GetType(System.Math) Then

            JSWriter.RawWrite("Math.")
            JSWriter.RawWrite(expr.Method.Name.ToLower)
            WriteArguments(expr.Arguments)

          Else

            'Write the method... JS runtime will throw an exception if the method doesnt exist.
            JSWriter.RawWrite(expr.Method.Name)
            WriteArguments(expr.Arguments)

            'Throw New Exception("Expression Parser: Unsupported method: " & expr.Method.Name)
          End If


      End Select

    End Sub

    Private Sub WriteArguments(Args As System.Collections.ObjectModel.ReadOnlyCollection(Of Expressions.Expression), Optional StartIndex As Integer = 0)
      JSWriter.RawWrite("(")
      For i As Integer = StartIndex To Args.Count - 1
        If i > StartIndex Then
          JSWriter.RawWrite(", ")
        End If
        ParseExpression(Args(i))
      Next
      JSWriter.RawWrite(")")
    End Sub

    Private Sub ParseToString(expr As Expressions.MethodCallExpression, Arguments As System.Collections.ObjectModel.ReadOnlyCollection(Of Expressions.Expression))

      'Get the data type of the object on which toString was called.
      Dim DataType As Reflection.SMemberInfo.MainType = Reflection.SMemberInfo.MainType.String
      If expr.Object IsNot Nothing Then
        Dim m = GetMember(expr.Object)
        If m IsNot Nothing Then
          DataType = New Singular.Reflection.SMemberInfo(m).DataTypeMain
        ElseIf TypeOf expr.Object Is ParameterExpression Then
          DataType = Reflection.SMemberInfo.MainType.Other
        Else
          DataType = Reflection.SMemberInfo.MainType.Number
        End If
      Else
        If expr.Method.GetParameters.Length > 0 Then
          DataType = New Singular.Reflection.SMemberInfo(expr.Method.GetParameters(0).ParameterType).DataTypeMain
        End If
      End If

      'Default Date to dd MMM yyyy where there are no params.
      If DataType = Reflection.SMemberInfo.MainType.Date Then
        JSWriter.RawWrite("dateFormat(")
        ParseExpression(expr.Object)
        JSWriter.RawWrite(", ")
        If Arguments.Count = 0 Then
          JSWriter.RawWrite("'dd MMM yyyy'")
        Else
          ParseExpression(Arguments(0))
        End If
        JSWriter.RawWrite(")")

        'Default number to 2 decimals, and space as thousands seperator.
        'Number can take the usual #,## format string, or 3 paramters in a string (Decimals, Thousands, Negative)
      ElseIf DataType = Reflection.SMemberInfo.MainType.Number Then
        JSWriter.RawWrite("Number(")
        ParseExpression(expr.Object)
        JSWriter.RawWrite(").formatMoney(")
        If Arguments.Count = 1 Then
          'Make spaces non breaking.
          JSWriter.RawWrite("'" & Arguments(0).ToString.Replace("""", "") & "'")
        End If
        JSWriter.RawWrite(")")
      Else
        ParseExpression(expr.Object)
        JSWriter.RawWrite(".ToString()")
      End If

    End Sub

    Protected Overrides Sub ParseConditionalExpression(expr As ConditionalExpression)
      SetPropertyAccessMode()

      JSWriter.RawWrite("(")
      ParseExpression(expr.Test)
      JSWriter.RawWrite(" ? ")
      ParseExpression(expr.IfTrue)
      JSWriter.RawWrite(" : ")
      ParseExpression(expr.IfFalse)
      JSWriter.RawWrite(")")

    End Sub

    Public Overrides Function ToString() As String
      Return JSWriter.ToString
    End Function

    Private FunctionNameMapping As New Hashtable From {
      {"abs", "Math.abs"},
      {"tolower", "toLowerCase"},
      {"toupper", "toUpperCase"}
    }

  End Class

End Namespace

