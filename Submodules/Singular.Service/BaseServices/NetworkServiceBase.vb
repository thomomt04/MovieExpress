Namespace Networking

  Public MustInherit Class NetworkServiceBase
    Inherits ServerProgramBase

    Public ReadOnly Property Active() As Boolean
      Get
        Return Me.ServerProgramType.ActiveInd
      End Get
    End Property

    'Public Overrides Sub WriteProgress(ByVal Progress As String)

    '  Try
    '    mLastProgress = _
    '        Services.ServerProgramProgress.NewServerProgramProgress(Me.ServerProgramTypeID, Progress, Me.Version, ProgressType.Undefined)
    '    If mLastProgress.IsValid Then
    '      mLastProgress.Save()
    '    Else
    '      Throw New Exception("Cannot save progress: " & mLastProgress.GetBrokenRulesAsString())
    '    End If
    '  Catch ex As Exception
    '    Throw New Exception("Error updating progress: " & Singular.Debug.RecurseExceptionMessage(ex))
    '  End Try

    'End Sub

    Public Sub New(ByVal Name As String)

      MyBase.New(Name)

    End Sub

  End Class

End Namespace
