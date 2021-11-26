Imports System.IO

Namespace Compression

  Public Class CompressionUtility

    Public Shared Function Compress(ByVal byteData As Byte()) As Byte()
      Dim compressedData As Byte() = Nothing
      If byteData IsNot Nothing Then
        Using ms As New MemoryStream()
          Dim defl As New ICSharpCode.SharpZipLib.Zip.Compression.Deflater(9, False)
          Using s As Stream = New ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms, defl)
            s.Write(byteData, 0, byteData.Length)
          End Using
          compressedData = ms.ToArray()
        End Using
      End If
      Return compressedData
    End Function

    Public Shared Sub CompressStream(ByRef InputStream As Stream, ByRef OutputStream As Stream)

      Dim inMS As IO.MemoryStream = InputStream
      Dim Bytes() = Compress(inMS.ToArray())
      If OutputStream Is Nothing Then
        OutputStream = New IO.MemoryStream()
      End If
      OutputStream.Write(Bytes, 0, Bytes.Length - 1)

      'Dim inMS As IO.MemoryStream = InputStream
      'OutputStream = New IO.MemoryStream(Compress(inMS.ToArray))

    End Sub

    Public Shared Function Decompress(ByVal byteInput As Byte()) As Byte()
      Dim bytResult As Byte() = Nothing
      If byteInput IsNot Nothing Then
        Using ms As New MemoryStream(byteInput, 0, byteInput.Length)
          Dim strResult As String = [String].Empty
          Dim writeData As Byte() = New Byte(4095) {}
          Dim s2 As Stream = New ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(ms)
          bytResult = ReadFullStream(s2)
        End Using
      End If
      Return bytResult
    End Function

    Public Shared Sub DecompressStream(ByRef InputStream As Stream, ByRef OutputStream As Stream)

      Dim inMS As IO.MemoryStream = InputStream
      Dim Bytes() = Decompress(inMS.ToArray)
      If OutputStream Is Nothing Then
        OutputStream = New IO.MemoryStream()
      End If
      OutputStream.Write(Bytes, 0, Bytes.Length - 1)

    End Sub

    Private Shared Function ReadFullStream(ByVal stream As Stream) As Byte()
      Dim buffer As Byte() = New Byte(32767) {}
      Using ms As New MemoryStream()
        While True
          Dim read As Integer = stream.Read(buffer, 0, buffer.Length)
          If read <= 0 Then
            Return ms.ToArray()
          End If
          ms.Write(buffer, 0, read)
        End While
      End Using
      Return Nothing
    End Function

    Public Shared Function CompressFiles(Files()() As Byte, FileNames() As String) As Byte()
      Try
        Dim ms As IO.MemoryStream = New IO.MemoryStream
        Dim zipOutputStream As New ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ms)

        For Each File As Byte() In Files

          Dim entry As New ICSharpCode.SharpZipLib.Zip.ZipEntry(FileNames(Array.IndexOf(Files, File)))
          entry.Size = File.Length
          zipOutputStream.PutNextEntry(entry)
          zipOutputStream.Write(File, 0, File.Length)
          zipOutputStream.CloseEntry()
        Next

        zipOutputStream.Close()

        Dim b() As Byte = ms.ToArray()

        ms.Close()

        Return b

      Catch ex As Exception
        Throw ex
      End Try
      Return Nothing
    End Function

    Public Shared Function CompressFiles(FileNames() As String, DestinationFile As String) As String

      Dim Files As New List(Of Byte())
      Dim ActualFileNames As New List(Of String)

      For Each File In FileNames
        Using fs As IO.FileStream = IO.File.Open(File, FileMode.Open)
          Using ms As New IO.MemoryStream
            fs.CopyTo(ms)
            Files.Add(ms.ToArray())
            ActualFileNames.Add(File.Substring(File.LastIndexOf("\") + 1))
          End Using
        End Using
      Next

      Dim zip() As Byte = CompressFiles(Files.ToArray, ActualFileNames.ToArray)

      Using zfs As IO.FileStream = IO.File.Open(DestinationFile, FileMode.Create)
        zfs.Write(zip, 0, zip.Length)
      End Using

      Return ""
    End Function

    ''' <summary>
    ''' Decompresses, and returns the first file in a zip file. Returns the original file if its not a zip file.
    ''' </summary>
    ''' <param name="CompressedBytes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DecompressFile(CompressedBytes As Byte()) As Byte()

      Dim ms As New IO.MemoryStream(CompressedBytes)
      Try
        Dim zipFile As ICSharpCode.SharpZipLib.Zip.ZipFile = New ICSharpCode.SharpZipLib.Zip.ZipFile(ms)
        ms.Position = 0

        If zipFile.TestArchive(True) AndAlso zipFile.Count = 1 Then
          ms.Position = 0
          For Each zipEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry In zipFile
            If zipEntry.IsFile Then

              Dim entryFileName As String = zipEntry.Name
              Dim buffer(4095) As Byte
              Dim zipStream As IO.Stream = zipFile.GetInputStream(zipEntry)

              Using dcStream As New IO.MemoryStream
                ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipStream, dcStream, buffer)
                Return dcStream.ToArray
              End Using

            End If
          Next

        End If
      Catch ex As ICSharpCode.SharpZipLib.Zip.ZipException

      Finally
        ms.Close()
      End Try

      Return CompressedBytes

    End Function



#If SILVERLIGHT Then
#Else

    Public Shared Function DeserialiseObject(CompressedBytes As Byte()) As ISingularBusinessBase

      If CompressedBytes Is Nothing OrElse CompressedBytes Is DBNull.Value Then
        Return Nothing

      Else

        Dim f As New Csla.Serialization.BinaryFormatterWrapper()
        Dim Decompressed As Byte()
        Dim DeserialisedObject As Object

        Try
          Decompressed = Singular.Compression.CompressionUtility.Decompress(CompressedBytes)
        Catch ex As Exception
          Decompressed = CompressedBytes
        End Try

        Using ms As New IO.MemoryStream(Decompressed)
          DeserialisedObject = f.Deserialize(ms)
        End Using

        DeserialisedObject.MarkOld()

        Return DeserialisedObject

      End If

    End Function

    Public Shared Function SerialiseObject(Obj As ISingularBusinessBase) As Byte()

      Using ms As New System.IO.MemoryStream()
        Dim f As New Csla.Serialization.BinaryFormatterWrapper()
        f.Serialize(ms, Obj)

        Return Singular.Compression.CompressionUtility.Compress(ms.ToArray())
      End Using

    End Function
#End If


  End Class


End Namespace
