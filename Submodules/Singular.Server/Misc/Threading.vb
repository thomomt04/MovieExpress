Public Class Threading

  Public Shared Function LazySetSafe(Of Type)(LockObject As Object,
                                               CheckFunction As Func(Of Boolean),
                                               ReadFunction As Func(Of Type),
                                               WriteFunction As Action) As Type

    If CheckFunction() Then

      Return ReadFunction()
    Else
      SyncLock (LockObject)

        If CheckFunction() Then
          Return ReadFunction()
        Else
          WriteFunction()
          Return ReadFunction()
        End If

      End SyncLock

    End If

  End Function

End Class
