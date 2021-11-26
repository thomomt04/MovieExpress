Imports System.Web.UI.WebControls

Namespace CustomControls

  Public Class SDropDownList
    Inherits DropDownList

    Public Sub New()
      EnableViewState = False
    End Sub

    Public Property Mode As Singular.Web.Controls.RenderMode = Singular.Web.Controls.RenderMode.BuildOnServer
    Public Property CaptionText As String = ""

    Protected Overrides Function LoadPostData(postDataKey As String, postCollection As System.Collections.Specialized.NameValueCollection) As Boolean

      If Mode = Singular.Web.Controls.RenderMode.BuildOnClient Then
        'If the Items are added on the client, the items collection on the server will be empty. 
        'The base control requires there to be an item for the selected item changed event to work, so we just add a dummy item with the correct value.
        If SelectedValue <> Page.Request.Form(Me.UniqueID) AndAlso (mSelectedValueTemp Is Nothing OrElse mSelectedValueTemp <> Page.Request.Form(Me.UniqueID)) Then
          Items.Add(New ListItem("dummy", Page.Request.Form(Me.UniqueID)))
          SelectedValue = Page.Request.Form(Me.UniqueID)
          Return True
        End If
      End If
      Return MyBase.LoadPostData(postDataKey, postCollection)

    End Function

    Private mSelectedValueTemp As Object = Nothing
    Public Overrides Property SelectedValue As String
      Get
        Return MyBase.SelectedValue
      End Get
      Set(value As String)
        MyBase.SelectedValue = value
        If UniqueID IsNot Nothing Then
          mSelectedValueTemp = value
        End If
      End Set
    End Property

    Protected Overrides Sub RenderContents(writer As System.Web.UI.HtmlTextWriter)

      If Mode = Singular.Web.Controls.RenderMode.BuildOnServer AndAlso Items.Count > 0 Then

        Dim selected As Boolean = False
        Dim optGroupStarted As Boolean = False

        'Write the first item if there is a Caption.
        If CaptionText <> "" Then
          writer.WriteBeginTag("option")
          HttpUtility.HtmlEncode(CaptionText, writer)
          writer.Write(">")
          writer.WriteEndTag("option")
          writer.WriteLine()
        End If

        For Each item As ListItem In Items

          If item.Enabled Then

            If Information.IsNumeric(item.Value) AndAlso item.Value = -1 Then
              'Option Group

              If optGroupStarted Then

                writer.WriteBeginTag("option")
                writer.WriteAttribute("disabled", "true")
                writer.Write(">")
                writer.WriteEndTag("option")
                writer.WriteLine()

                writer.WriteEndTag("optgroup")
              End If

              writer.WriteBeginTag("optgroup")
              writer.WriteAttribute("label", HttpUtility.HtmlEncode(item.Text))
              writer.Write(">")
              writer.WriteLine()
              optGroupStarted = True

            Else
              'Normal Item

              writer.WriteBeginTag("option")
              If item.Selected Then

                If selected Then
                  VerifyMultiSelect()
                End If
                selected = True
                writer.WriteAttribute("selected", "selected")

              End If

              writer.WriteAttribute("value", item.Value, True)

              If (item.Attributes.Count > 0) Then
                item.Attributes.Render(writer)
              End If

              If Page IsNot Nothing Then
                Page.ClientScript.RegisterForEventValidation(UniqueID, item.Value)
              End If

              writer.Write(">")
              HttpUtility.HtmlEncode(item.Text, writer)
              writer.WriteEndTag("option")
              writer.WriteLine()

            End If

          End If

        Next

        If optGroupStarted Then
          writer.WriteEndTag("optgroup")
        End If

      End If

    End Sub

    Public Overrides ReadOnly Property Items As System.Web.UI.WebControls.ListItemCollection
      Get
        Return MyBase.Items
      End Get
    End Property

    ''' <summary>
    ''' Returns a value Compatible with Singular CSLA Objects. E.g. DBNull.Value or an Integer Value.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Value As Object
      Get
        If SelectedValue Is Nothing OrElse SelectedValue Is DBNull.Value OrElse SelectedValue = "" Then
          Return DBNull.Value
        Else
          Return CInt(SelectedValue)
        End If
      End Get
    End Property

    Public Property DataSourceName As String
    Public Property DataGroupMember As String
    Public Property SelectedValueMember As String
    Public Property DataGroupChildList As String

    Public Sub Setup(DataSourceName As String, ValueMember As String, DisplayMember As String, DataGroupChildList As String, GroupMember As String, SelectedValue As String)
      Me.DataSourceName = DataSourceName
      Me.DataValueField = ValueMember
      Me.DataTextField = DisplayMember
      Me.DataGroupMember = GroupMember
      Me.DataGroupChildList = DataGroupChildList

      If GroupMember = "" Then
        Attributes("data-bind") = "options: " & DataSourceName & ", optionsValue: '" & ValueMember & "', optionsText: '" & DisplayMember & "', optionsCaption: '" & CaptionText & "', value: " & SelectedValue
      Else
        Attributes("data-bind") = "groupedOptions: " & DataSourceName & ", optionsValue: '" & ValueMember & "', optionsGroupList: '" & DataGroupChildList & "', groupText: '" & GroupMember & "', optionsText: '" & DisplayMember & "', optionsCaption: '" & CaptionText & "', value: " & SelectedValue
      End If

    End Sub

    Public Sub Setup(List As Object, ValueMember As String, DisplayMember As String)
      Setup(List, ValueMember, "", DisplayMember)
    End Sub

    Public Sub Setup(List As Object, ValueMember As String, GroupMember As String, DisplayMember As String)

      MyBase.DataValueField = ValueMember
      MyBase.DataTextField = DisplayMember

      Dim GroupValue As Object = Nothing

      If TypeOf List Is DataTable Then

        For Each row As DataRow In CType(List, DataTable).Rows

          If GroupMember <> "" Then
            GroupValue = row(GroupMember)
          End If

          AddItem(GroupValue, row(ValueMember), row(DisplayMember))

        Next

      Else

        Dim GenericArguments As Integer = Singular.Reflection.GetGenericArgumentCount(List.GetType)

        If GenericArguments > 0 Then
          Dim ObjectType As Type = Singular.Reflection.GetGenericArgumentType(List.GetType, GenericArguments - 1)

          Dim PIValue As System.Reflection.PropertyInfo = ObjectType.GetProperty(ValueMember, System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
          Dim PIDisplay As System.Reflection.PropertyInfo = ObjectType.GetProperty(DisplayMember, System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
          Dim PIGroup As System.Reflection.PropertyInfo = ObjectType.GetProperty(GroupMember, System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)

          For Each obj As Object In List

            If PIGroup IsNot Nothing Then
              GroupValue = PIGroup.GetValue(obj, Nothing)
            End If

            AddItem(GroupValue, PIValue.GetValue(obj, Nothing), PIDisplay.GetValue(obj, Nothing))

          Next

        End If

      End If

    End Sub

    Private mLastGroupValue As Object = Nothing
    Private Sub AddItem(GroupValue As Object, ItemValue As Object, Display As String)

      Dim AddAsOptGroup As Boolean = False
      If GroupValue IsNot Nothing Then
        If mLastGroupValue Is Nothing OrElse Not Singular.Misc.CompareSafe(GroupValue, mLastGroupValue) Then
          AddAsOptGroup = True
          mLastGroupValue = GroupValue
        End If
      End If

      If AddAsOptGroup Then
        Items.Add(New ListItem(GroupValue, -1))
      End If

      Items.Add(New ListItem(Display, ItemValue))

    End Sub


  End Class

End Namespace