Imports System.Drawing

Namespace Colours

  Public Class Convert

    Public Shared Function ToHexString(Color As Color) As String
      Return "#" + Color.R.ToString("X2") + Color.G.ToString("X2") + Color.B.ToString("X2")
    End Function


    '    Private Static String HexConverter(System.Drawing.Color c)
    '{
    '    Return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    '}

    'Private Static String RGBConverter(System.Drawing.Color c)
    '{
    '    Return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
    '}
  End Class

  ''' <summary>
  ''' Provides methods for manipulating and generation colours
  ''' </summary>
  ''' <remarks></remarks>
  Public Class ColourGenerator


    Public Shared ReadOnly Property SoftColours As Color()
      Get
        Return New Color() {Color.FromArgb(215, 245, 255),
                                  Color.FromArgb(255, 235, 200),
                                  Color.FromArgb(215, 255, 215),
                                  Color.FromArgb(255, 235, 235),
                                  Color.FromArgb(255, 190, 210),
                                  Color.FromArgb(190, 210, 255),
                                  Color.FromArgb(215, 215, 255),
                                  Color.FromArgb(210, 190, 255)}
      End Get
    End Property

    Public Shared Function GenerateInterpolation(ByVal FromColor As System.Drawing.Color, ByVal ToColor As System.Drawing.Color, ByVal ArraySize As Integer) As System.Drawing.Color()

      Dim rMax As Integer = ToColor.R
      Dim rMin As Integer = FromColor.R
      Dim gMax As Integer = ToColor.G
      Dim gMin As Integer = FromColor.G
      Dim bMax As Integer = ToColor.B
      Dim bMin As Integer = FromColor.B

      Dim colorList(ArraySize - 1) As System.Drawing.Color
      For i As Integer = 0 To ArraySize - 1
        Dim rAverage = rMin + CInt((rMax - rMin) * i / ArraySize)
        Dim gAverage = gMin + CInt((gMax - gMin) * i / ArraySize)
        Dim bAverage = bMin + CInt((bMax - bMin) * i / ArraySize)

        colorList(i) = System.Drawing.Color.FromArgb(rAverage, gAverage, bAverage)
      Next

      Return colorList

    End Function

    Public Shared Function GetColourDifference(ByVal Color1 As Color, ByVal Color2 As Color) As Integer

      Return Math.Abs(CDec(Color1.R) - CDec(Color2.R)) + Math.Abs(CDec(Color1.G) - CDec(Color2.G)) + Math.Abs(CDec(Color1.B) - CDec(Color2.B))

    End Function

    Public Shared Function GenerateRandomColours(ByVal Count As Integer, Optional ByVal MinimumDifference As Integer = 5) As List(Of Color)

      Dim RandomColors As New List(Of Color)

      Dim rand As New Random()

      For i = 0 To Count - 1
        Dim NextColor = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256))

        While RandomColors.Count > 0 AndAlso RandomColors.Min(Function(rc) GetColourDifference(rc, NextColor)) < MinimumDifference
          NextColor = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256))
        End While

        RandomColors.Add(NextColor)
      Next

      Return RandomColors

    End Function

    Public Shared Function GenerateAdditionalRandomColours(ByVal StartingList As List(Of Color), ByVal AdditionalCount As Integer, Optional ByVal MinimumDifference As Integer = 5) As List(Of Color)

      Dim rand As New Random()

      For i = 0 To AdditionalCount - 1
        Dim NextColor = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256))

        While StartingList.Count > 0 AndAlso StartingList.Min(Function(rc) GetColourDifference(rc, NextColor)) < MinimumDifference
          NextColor = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256))
        End While

        StartingList.Add(NextColor)
      Next

      Return StartingList

    End Function

  End Class

End Namespace
