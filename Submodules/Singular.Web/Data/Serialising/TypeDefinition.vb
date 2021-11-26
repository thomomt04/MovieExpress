Imports Singular.Web.Data.JS.ObjectInfo

Namespace Data.JS

  Public Class TypeDefinitionList
    Inherits Dictionary(Of String, TypeDefinition)

    Public Shared Function GetTypeName(ObjectType As Type, Optional CustomName As Object = Nothing) As String
      If CustomName IsNot Nothing Then
        Return CustomName
      ElseIf ObjectType.IsNested Then
        Return ObjectType.DeclaringType.Name & "_" & ObjectType.Name & "Object"
      Else
        Return ObjectType.Name & "Object"
      End If
    End Function

    Public Function AddTypeDefinition(ObjectType As Type, Member As Member, MustRender As Boolean, Optional CustomName As Object = Nothing) As Boolean

      Dim TypeName As String = GetTypeName(ObjectType, CustomName)
      If Not Me.ContainsKey(TypeName) Then
        Me.Add(TypeName, New TypeDefinition(ObjectType, TypeName, Member, MustRender))
        Return True
      End If
      Return False
    End Function

    Public Function GetItem(ObjectType As Type) As TypeDefinition
      Dim TypeName As String = GetTypeName(ObjectType, Nothing)
      Return Me(TypeName)
    End Function

    Public Sub RenderTypes(JW As Singular.Web.Utilities.JavaScriptWriter)

      For Each TypeName As String In Keys
        Me(TypeName).RenderType(JW)
      Next

    End Sub

  End Class

  Public Class TypeDefinition

    Private mMember As Member
    Private mTypeName As String
    Private mType As Type
    Private mDotNetTypeName As String
    Private mMustRender As Boolean

    Public Sub New(Type As Type, TypeName As String, Member As Member, MustRender As Boolean)
      mMember = Member
      mType = Type
      mTypeName = TypeName
      mMustRender = MustRender
      mDotNetTypeName = Singular.ReflectionCached.GetCachedType(mType).DotNetTypeName 'Singular.Encryption.GetStringHash(mType.FullName, Encryption.HashType.MD5)

    End Sub

    Public ReadOnly Property Type As Type
      Get
        Return mType
      End Get
    End Property

    Public ReadOnly Property Member As Member
      Get
        Return mMember
      End Get
    End Property

    Friend Sub RenderType(JW As Singular.Web.Utilities.JavaScriptWriter)
      If mMustRender Then
        JW.WriteStartClass(mTypeName, "Parent")
        JW.Write("SetupKOObject(this, Parent, function(self){")
        JW.AddLevel()

        mMember.RenderJSMethods(JW)
        mMember.RenderModelMembers(JW)
        mMember.RenderRules(JW)

        JW.RemoveLevel()
        JW.Write("});")

        JW.WriteEndClass(False)
        JW.Write(mTypeName & ".Type = '" & mDotNetTypeName & "';", True)
        JW.RawWriteLine("")
      End If
    End Sub

  End Class

  ''' <summary>
  ''' A lightweight version of Type Definition
  ''' </summary>
  Public Class SchemaDefinitionList

    Private Shared mSchemaJS As New Hashtable
    Private mSchemaList As New Hashtable

    Public Sub AddType(Type As Type)
      If Not mSchemaList.ContainsKey(Type) Then
        If Not mSchemaJS.ContainsKey(Type) Then
          Dim s As New Data.JS.StatelessJSSerialiser(Type)
          mSchemaJS(Type) = s.GetSchema(OutputType.Javascript)
        End If
        mSchemaList(Type) = mSchemaJS(Type)
      End If
    End Sub

    Public Sub WriteSchemas(JSW As Singular.Web.Utilities.JavaScriptWriter)

      For Each Key As Type In mSchemaList.Keys
        JSW.Write("ObjectSchema.Register('" & Singular.ReflectionCached.GetCachedType(Key).DotNetTypeName & "', " & mSchemaList(Key) & ")")
      Next

    End Sub

  End Class

End Namespace


