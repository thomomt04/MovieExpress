Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Web.UI

Namespace CustomControls

  Public Class WebStatsInfo
    Inherits Controls.CustomWebControl

    Private Sub AddOwnSessionTable()

      Dim tbl As New Table
      tbl.CssClass = "Grid"
      tbl.Caption = "This Sessions Size"

      Dim rH As New TableRow
      rH.Cells.Add(New TableHeaderCell() With {.Text = "Key"})
      rH.Cells.Add(New TableHeaderCell() With {.Text = "Type"})
      rH.Cells.Add(New TableHeaderCell() With {.Text = "Size"})
      rH.TableSection = TableRowSection.TableHeader
      tbl.Rows.Add(rH)

      Dim totalSessionBytes As Long = 0

      Dim b As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
      Dim m As IO.MemoryStream

      For Each key As String In System.Web.HttpContext.Current.Session.Keys
        Dim Type As String = ""
        Dim Size As Integer = 0
        Try

          If System.Web.HttpContext.Current.Session(key) IsNot Nothing Then

            m = New IO.MemoryStream
            b.Serialize(m, System.Web.HttpContext.Current.Session(key))

            Size = m.Length
            totalSessionBytes += m.Length

            Type = System.Web.HttpContext.Current.Session(key).GetType.Name

          Else
            Type = "null"
          End If


        Catch ex As Exception
          Type = "unknown"
        End Try

        Dim r1 As New TableRow
        r1.Cells.Add(New TableCell() With {.Text = key})
        r1.Cells.Add(New TableCell() With {.Text = Type})
        r1.Cells.Add(New TableCell() With {.Text = Size})
        r1.TableSection = TableRowSection.TableBody
        tbl.Rows.Add(r1)

        totalSessionBytes += key.Length

      Next

      Me.Controls.Add(tbl)

      Me.Controls.Add(New Literal With {.Text = "Total Size: " & Singular.Files.GetReadableSize(totalSessionBytes) & "<p/>"})

    End Sub

    Public Shared Function GetTable(ss As Singular.Web.Misc.WebSiteStats.SessionStats) As Table

      Dim tbl As New Table
      tbl.CssClass = "Grid"
      Select Case ss.StatsResetType
        Case Misc.WebSiteStats.SessionStats.ResetType.Daily
          tbl.Caption = "Today Stats"
        Case Misc.WebSiteStats.SessionStats.ResetType.Never
          tbl.Caption = "All Time Stats"
      End Select

      Dim r1 As New TableRow
      r1.Cells.Add(New TableCell() With {.Text = "Last Reset"})
      r1.Cells.Add(New TableCell() With {.Text = ss.LastResetDate.ToString("dd MMM yyyy HH:mm")})

      Dim r2 As New TableRow
      r2.Cells.Add(New TableCell() With {.Text = "No of Sessions Started"})
      r2.Cells.Add(New TableCell() With {.Text = ss.TotalStartedSessions})

      Dim r2a As New TableRow
      r2a.Cells.Add(New TableCell() With {.Text = "Sessions Started /min"})
      r2a.Cells.Add(New TableCell() With {.Text = ss.TotalStartedSessions / (DateDiff(DateInterval.Minute, ss.LastResetDate, Now) + 1)})

      Dim r3 As New TableRow
      r3.Cells.Add(New TableCell() With {.Text = "Min Active Sessions"})
      r3.Cells.Add(New TableCell() With {.Text = ss.MinActiveSessions})

      Dim r4 As New TableRow
      r4.Cells.Add(New TableCell() With {.Text = "Max Active Sessions"})
      r4.Cells.Add(New TableCell() With {.Text = ss.MaxActiveSessions})

      tbl.Rows.Add(r1)
      tbl.Rows.Add(r2)
      tbl.Rows.Add(r2a)
      tbl.Rows.Add(r3)
      tbl.Rows.Add(r4)

      Return tbl
    End Function

    Protected Overrides Sub CreateChildControls()

      'This sessions stats.
      AddOwnSessionTable()

      'Session Summary
      Me.Controls.Add(New Label() With {.Text = Misc.WebSiteStats.TotalActiveSessions & " Current Active Sessions"})
      Me.Controls.Add(New LiteralControl("<br />"))
      Me.Controls.Add(New Label() With {.Text = "Your session has been open for " & DateDiff(DateInterval.Minute, Misc.WebSiteStats.Session(HttpContext.Current.Session.SessionID).StartDate, Now) & " minutes"})

      'No of sessions per day / all time etc.
      For Each ss As Misc.WebSiteStats.SessionStats In Misc.WebSiteStats.StatsList
        Me.Controls.Add(GetTable(ss))
      Next

      'Sessions Started by browser type.
      Dim tbl As New Table
      tbl.CssClass = "Grid Sortable"
      tbl.Caption = "Browser Stats"

      Dim rH As New TableRow
      rH.Cells.Add(New TableHeaderCell() With {.Text = "Browser"})
      rH.Cells.Add(New TableHeaderCell() With {.Text = "Version"})
      rH.Cells.Add(New TableHeaderCell() With {.Text = "Count"})
      rH.TableSection = TableRowSection.TableHeader
      tbl.Rows.Add(rH)

      For Each bs As Misc.WebSiteStats.BrowserStats In Misc.WebSiteStats.BrowserStatList

        Dim r1 As New TableRow
        r1.Cells.Add(New TableCell() With {.Text = bs.BrowserName})
        r1.Cells.Add(New TableCell() With {.Text = bs.BrowserVersion})
        r1.Cells.Add(New TableCell() With {.Text = bs.Count})
        r1.TableSection = TableRowSection.TableBody
        tbl.Rows.Add(r1)
      Next

      Me.Controls.Add(tbl)

    End Sub

    Public Function RenderAsString() As String
      Dim tw As New HtmlTextWriter(New System.IO.StringWriter)
      CreateChildControls()
      For Each ctl As Control In Me.Controls
        ctl.RenderControl(tw)
      Next

      Return tw.InnerWriter.ToString
    End Function

  End Class

End Namespace

Namespace Misc

  Public Class WebSiteStats

    Public Class SessionInfo
      Public Property StartDate As Date

    End Class

    Public Class BrowserStats
      Public Property BrowserName As String = ""
      Public Property BrowserVersion As String = ""
      Public Property Count As Integer = 1
    End Class

    Private Shared mSessionList As SortedList(Of String, SessionInfo)
    Private Shared mStatsList As List(Of SessionStats)
    Private Shared mBrowserList As List(Of BrowserStats)

    Shared Sub New()

      mSessionList = New SortedList(Of String, SessionInfo)
      mStatsList = New List(Of SessionStats)
      mBrowserList = New List(Of BrowserStats)

      mStatsList.Add(New SessionStats(SessionStats.ResetType.Daily, mSessionList))
      mStatsList.Add(New SessionStats(SessionStats.ResetType.Never, mSessionList))

    End Sub

    Public Class SessionStats
      Private mResetType As ResetType
      Private mSessionList As SortedList(Of String, SessionInfo)
      Private mLastResetDate As Date = Now

      Private mTotalSessionsStarted As Integer
      Private mMaxActiveSessions As Integer
      Private mMinActiveSessions As Integer

      Public Enum ResetType
        Daily = 1
        Never = 2
      End Enum

      Public Sub New(rt As ResetType, sl As SortedList(Of String, SessionInfo))
        mResetType = rt
        mSessionList = sl
      End Sub

      Public Sub Reset()
        Select Case mResetType
          Case ResetType.Daily
            If Now.Date <> mLastResetDate.Date Then
              mLastResetDate = Now.Date
              mTotalSessionsStarted = 0
              mMaxActiveSessions = mSessionList.Count
              mMinActiveSessions = mSessionList.Count
            End If
        End Select
      End Sub

      Public Sub SessionAdded()
        mMaxActiveSessions = Math.Max(mMaxActiveSessions, mSessionList.Count)
        mTotalSessionsStarted += 1
      End Sub

      Public Sub SessionRemoved()
        mMaxActiveSessions = Math.Min(mMinActiveSessions, mSessionList.Count)
      End Sub

      ''' <summary>
      ''' Returns the total number of sessions started since the application was last recycled.
      ''' </summary>
      ''' <value></value>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public ReadOnly Property TotalStartedSessions As Integer
        Get
          Return mTotalSessionsStarted
        End Get
      End Property

      ''' <summary>
      ''' Returns the Min Active Sessions for the period.
      ''' </summary>
      ''' <value></value>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public ReadOnly Property MinActiveSessions As Integer
        Get
          Return mMinActiveSessions
        End Get
      End Property

      ''' <summary>
      ''' Returns the Max Active Sessions for the period.
      ''' </summary>
      ''' <value></value>
      ''' <returns></returns>
      ''' <remarks></remarks>
      Public ReadOnly Property MaxActiveSessions As Integer
        Get
          Return mMaxActiveSessions
        End Get
      End Property

      Public ReadOnly Property LastResetDate As Date
        Get
          Return mLastResetDate
        End Get
      End Property

      Public ReadOnly Property StatsResetType As ResetType
        Get
          Return mResetType
        End Get
      End Property

    End Class

    Public Shared Sub AddSession(SessionID As String)

      Try

        If Not mSessionList.ContainsKey(SessionID) Then
          mSessionList.Add(SessionID, New SessionInfo() With {.StartDate = Now})

          For Each ss As SessionStats In mStatsList
            ss.Reset()
            ss.SessionAdded()
          Next

          Dim Browser As String = HttpContext.Current.Request.Browser.Browser
          If Browser = "Unknown" Then
            Browser = HttpContext.Current.Request.UserAgent
          End If

          Dim FoundBrowser As Boolean = False
          For Each bs As BrowserStats In mBrowserList
            If bs.BrowserName = Browser AndAlso bs.BrowserVersion = HttpContext.Current.Request.Browser.MajorVersion Then
              bs.Count += 1
              FoundBrowser = True
              Exit For
            End If
          Next
          If Not FoundBrowser Then
            mBrowserList.Add(New BrowserStats With {.BrowserName = Browser, .BrowserVersion = HttpContext.Current.Request.Browser.MajorVersion})
          End If

        End If

      Catch ex As Exception
        'Don't break the website cause stats are broken.
      End Try

    End Sub

    Public Shared Sub RemoveSession(SessionID As String)

      Try

        If mSessionList.ContainsKey(SessionID) Then
          mSessionList.Remove(SessionID)

          For Each ss As SessionStats In mStatsList
            ss.Reset()
            ss.SessionRemoved()
          Next

        End If

      Catch ex As Exception
        'Don't break the website cause stats are broken.
      End Try

    End Sub

    ''' <summary>
    ''' Returns the total number of Active sessions.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property TotalActiveSessions() As Integer
      Get
        Return mSessionList.Count
      End Get
    End Property

    Public Shared ReadOnly Property StatsList As List(Of SessionStats)
      Get
        Return mStatsList
      End Get
    End Property

    Public Shared ReadOnly Property BrowserStatList As List(Of BrowserStats)
      Get
        Return mBrowserList
      End Get
    End Property

    Public Shared ReadOnly Property Session(SessionID As String) As SessionInfo
      Get
        Return mSessionList(SessionID)
      End Get
    End Property


  End Class

End Namespace
