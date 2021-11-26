Namespace Documents

  Public Class Settings

    ''' <summary>
    ''' True if the document hash must be calculated, and sent to the insDocument proc.
    ''' </summary>
    Public Shared Property DocumentHashesEnabled As Boolean = False

    ''' <summary>
    ''' True if the current userid must be passed to the getDocument proc.
    ''' </summary>
    Public Shared Property PassUserIDToGetProc As Boolean = False

    ''' <summary>
    ''' If you need to add extra parameters to the getDocument proc, specify the address of the method that will do this.
    ''' </summary>
    Public Shared AddParametersToFetch As Action(Of Document.Criteria, SqlClient.SqlCommand)

  End Class

End Namespace


