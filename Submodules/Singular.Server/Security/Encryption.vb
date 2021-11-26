Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text

Public Class Encryption

  Public Enum HashType
    MD5 = 1
    Sha256 = 2
    HmacSha256 = 3
    'BCrypt = 3
  End Enum

  Public Enum ASCIIEncoding
    ASCII = 1
    Unicode = 2
    UTF8 = 3
  End Enum

  Public Shared Function GetSilverlightStringHash(ByVal str As String, Optional ByVal IncludeOtherChars As Boolean = True) As String

    If IncludeOtherChars Then
      str = "other" & str & "chars"
    End If

    Dim HashValue() As Byte
    Dim UE As New UnicodeEncoding()
    Dim MessageBytes As Byte() = UE.GetBytes(str)
    Dim SHhash As New SHA256Managed()
    HashValue = SHhash.ComputeHash(MessageBytes)

    Dim HexString As String = ""
    For Each b As Byte In HashValue
      HexString &= b.ToString("X2")
    Next

    Return HexString

  End Function

#If SILVERLIGHT Then

#Else

  ' Key
  Private Shared bytKey() As Byte = {12, 12, 53, 24, 55, 236, 7, 98, 79, 110, 211, 112, 13, 114, 5, 116}
  ' IV
  Private Shared bytIV() As Byte = {23, 21, 32, 1, 52, 78, 12, 18, 92, 101, 11, 22, 23, 141, 215, 161}

  Public Shared Function LoadAndDecryptFromFile(ByRef objToLoad As Object, ByVal sPathAndFileName As String) As Boolean
    ' Serializing the SQLString s table to a file

    Try
      ' If the file does not exist then return false
      If Not File.Exists(sPathAndFileName) Then
        Return False
      End If

      ' Binary formatter used to convert the binary back into an object
      Dim bf As New BinaryFormatter

      ' File stream used to read the encrypted file
      Dim fs As FileStream
      fs = File.Open(sPathAndFileName, FileMode.Open)

      'Create a new instance of the RijndaelManaged class and dencrypt the stream.
      Dim RMCrypto As New RijndaelManaged


      'Create a CryptoStream, pass it the file stream, and encrypt 
      Dim cs As New CryptoStream(fs, RMCrypto.CreateDecryptor(bytKey, bytIV), CryptoStreamMode.Read)

      ' Deserialize the crypto stream into an object
      objToLoad = bf.Deserialize(cs)

      ' Close the streams
      cs.Close()
      fs.Close()

      Return True

    Catch ex As Exception
      Throw New Exception("FileEncryption > LoadAndDecryptFromFile: " & ex.Message)
    End Try

  End Function

  ' Encrypts a given string
  Public Shared Function EncryptString(ByVal data As String) As String
    Try
      Dim utf8 As New UTF8Encoding
      Dim inBytes() As Byte = utf8.GetBytes(data) ' ascii encoding uses 7 
      'bytes for characters whereas the encryption uses 8 bytes, thus we use utf8
      Dim ms As New MemoryStream   'instead of writing the encrypted 
      'string to a filestream, I will use a memorystream

      Dim aes As New RijndaelManaged
      Dim cs As New CryptoStream(ms, aes.CreateEncryptor(bytKey, bytIV), CryptoStreamMode.Write)
      cs.Write(inBytes, 0, inBytes.Length) ' encrypt
      cs.FlushFinalBlock()

      Return Convert.ToBase64String(ms.GetBuffer(), 0, ms.Length)

    Catch ex As Exception
      Throw ex
    End Try
  End Function

  ' decrypts a string that was encrypted using the Encrypt method
  Public Shared Function DecryptString(ByVal data As String) As String
    Try
      If data = "" Then
        Return ""
      End If
      Dim inBytes() As Byte = Convert.FromBase64String(data)
      Dim mStream As New MemoryStream(inBytes, 0, inBytes.Length) ' instead of writing the decrypted text

      Dim aes As New RijndaelManaged
      Dim cs As New CryptoStream(mStream, aes.CreateDecryptor(bytKey, bytIV), CryptoStreamMode.Read)

      Dim sr As New StreamReader(cs)

      Return sr.ReadToEnd()
    Catch ex As Exception
      Throw ex
    End Try
  End Function

  ' decrypts a string that was encrypted using the Encrypt method
  Public Shared Function DecryptString(ByVal data() As Byte) As String
    Try
      Dim mStream As New MemoryStream(data, 0, data.Length) ' instead of writing the decrypted text

      Dim aes As New RijndaelManaged
      Dim cs As New CryptoStream(mStream, aes.CreateDecryptor(bytKey, bytIV), CryptoStreamMode.Read)

      Dim sr As New StreamReader(cs)

      Return sr.ReadToEnd()
    Catch ex As Exception
      Throw ex
    End Try
  End Function

  Public Shared Sub SaveAndEncryptToFile(ByVal objToSave As Object, ByVal sPathAndFileName As String)

    ' objToSave must be serializable

    Try

      ' Class used to convert the object into binary
      Dim bf As New BinaryFormatter

      ' File stream used to save the file
      Dim fs As FileStream
      ' Overwrite file if it already exists
      fs = File.Open(sPathAndFileName, FileMode.Create)

      'Create a new instance of the RijndaelManaged class and encrypt the stream.
      Dim RMCrypto As New RijndaelManaged

      'Create a CryptoStream, pass it the file stream, and encrypt 
      Dim cs As New CryptoStream(fs, RMCrypto.CreateEncryptor(bytKey, bytIV), CryptoStreamMode.Write)

      ' Serialize the object to save into the crypto stream
      bf.Serialize(cs, objToSave)

      ' Close streams
      cs.Close()
      fs.Close()

    Catch ex As Exception
      Throw New Exception("FileEncryption > SaveAndEncryptToFile: " & ex.Message)
    End Try

  End Sub

  Public Shared Sub EncryptFile(ByVal FileToEncrypt As String, ByVal OutputFile As String)

    Dim sfs As FileStream = Nothing
    Dim ofs As FileStream = Nothing
    Dim cs As CryptoStream = Nothing

    Try


      ofs = File.Open(FileToEncrypt, FileMode.Open)
      sfs = File.Open(OutputFile, FileMode.Create)

      'Create a new instance of the RijndaelManaged class and encrypt the stream.
      Dim RMCrypto As New RijndaelManaged

      'Create a CryptoStream, pass it the file stream, and encrypt 
      cs = New CryptoStream(sfs, RMCrypto.CreateEncryptor(bytKey, bytIV), CryptoStreamMode.Write)

      Dim data(4095) As Byte


      Do

        ofs.Read(data, 0, data.Length)

        cs.Write(data, 0, Math.Min(data.Length, ofs.Length - sfs.Position))

      Loop While ofs.Length - ofs.Position > 0




    Catch ex As Exception
      Throw New Exception("FileEncryption > SaveAndEncryptToFile: " & ex.Message)
    Finally
      ' Close streams
      cs.Close()
      sfs.Close()
      ofs.Close()
    End Try

  End Sub

  Public Shared Function GetStringHash(ByVal str As String, HashType As HashType, Optional ByVal IncludeOtherChars As Boolean = True, Optional ByVal Encoding As ASCIIEncoding = ASCIIEncoding.Unicode, Optional ByVal LowerCase As Boolean = False, Optional SecretKey As String = "") As String

    ' If HashType <> Encryption.HashType.BCrypt Then

    Dim Hasher As HashAlgorithm

    Select Case HashType
      Case Encryption.HashType.MD5
        Hasher = MD5.Create
      Case Encryption.HashType.HmacSha256
        If SecretKey = "" Then
          Throw New System.ArgumentException("SecretKey required for HMAC hash.", "SecretKey")
        End If
        Hasher = New HMACSHA256(System.Text.ASCIIEncoding.UTF8.GetBytes(SecretKey))
      Case Else
        Hasher = SHA256.Create
    End Select

    If IncludeOtherChars Then
      str = "other" & str & "chars"
    End If

    Dim Bytes As Byte()

    Select Case Encoding
      Case ASCIIEncoding.Unicode
        Bytes = System.Text.ASCIIEncoding.Unicode.GetBytes(str)
      Case ASCIIEncoding.ASCII
        Bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(str)
      Case ASCIIEncoding.UTF8
        Bytes = System.Text.ASCIIEncoding.UTF8.GetBytes(str)
      Case Else
        Bytes = System.Text.ASCIIEncoding.Unicode.GetBytes(str)
    End Select

    Hasher.ComputeHash(Bytes)

    Dim formatString As String = ""
    If LowerCase Then
      formatString = "x2"
    Else
      formatString = "X2"
    End If

    Dim HexString As String = ""
    For Each b As Byte In Hasher.Hash
      HexString &= b.ToString(formatString)
    Next

    Return HexString

    'Else
    '  Dim HashString = BCrypt.HashPassword(str, BCrypt.GenerateSalt())

    '  Return HashString
    'End If

  End Function

  Public Shared Sub DecryptFile(ByVal EncryptedFile As String, ByVal OutputFile As String)

    Dim ofs As FileStream = Nothing
    Dim sfs As FileStream = Nothing
    Dim cs As CryptoStream = Nothing

    Try

      ' File stream used to read the encrypted file

      ofs = File.Open(EncryptedFile, FileMode.Open)
      sfs = File.Open(OutputFile, FileMode.Create)

      'Create a new instance of the RijndaelManaged class and dencrypt the stream.
      Dim RMCrypto As New RijndaelManaged


      'Create a CryptoStream, pass it the file stream, and encrypt 
      cs = New CryptoStream(ofs, RMCrypto.CreateDecryptor(bytKey, bytIV), CryptoStreamMode.Read)

      Dim data(4095) As Byte


      Do

        cs.Read(data, 0, data.Length)

        sfs.Write(data, 0, Math.Min(data.Length, ofs.Length - sfs.Position))

      Loop While ofs.Length - ofs.Position > 0


    Catch ex As Exception
      Throw New Exception("FileEncryption > LoadAndDecryptFromFile: " & ex.Message)
    Finally
      ' Close the streams
      cs.Close()
      ofs.Close()
      sfs.Close()
    End Try

  End Sub

  ' Attempts to identify whether a given string is encrypted or not
  Public Shared Function IsEncrypted(ByVal TestString As String) As Boolean

    Dim Score As Integer = 0

    If TestString.Length > 23 Then
      Score += 1
    End If

    If TestString.EndsWith("=") Then
      Score += 1
    End If

    If TestString.EndsWith("==") Then
      Score += 1
    End If

    Try
      DecryptString(TestString)
      Score += 2
    Catch ex As Exception
      Score -= 1
    End Try

    Return Score > 1

  End Function

  Private Shared mCryptoProvider As New AesCryptoServiceProvider
  Private Shared mEncrypter As ICryptoTransform

  Public Shared Sub Initialise(KeyInitialiseString As String, Optional SaltString As String = "Singular Systems Salt Value")
    Dim KeyGen As New Rfc2898DeriveBytes(KeyInitialiseString, Encoding.ASCII.GetBytes(SaltString))
    mCryptoProvider.Key = KeyGen.GetBytes(mCryptoProvider.KeySize / 8)
    mCryptoProvider.IV = KeyGen.GetBytes(mCryptoProvider.BlockSize / 8)
    mEncrypter = mCryptoProvider.CreateEncryptor
  End Sub

  Public Shared Function GetEncryptedToken(Contents As String) As String
    SyncLock mCryptoProvider
      Dim inBytes() As Byte = Encoding.UTF8.GetBytes(Contents)
      Return Convert.ToBase64String(mEncrypter.TransformFinalBlock(inBytes, 0, inBytes.Length))
    End SyncLock
  End Function

  Public Shared Function GetEncryptedTokenHex(Contents As String) As String
    SyncLock mCryptoProvider
      Dim inBytes() As Byte = Encoding.Default.GetBytes(Contents)
      Dim hexString = BitConverter.ToString(mEncrypter.TransformFinalBlock(inBytes, 0, inBytes.Length)).Replace("-", "")
      Return hexString
    End SyncLock
  End Function

  ''' <summary>
  ''' Gets an enrpyted value using a salt. The salt should be unique for each use of this function, e.g. an objects GUID.
  ''' </summary>
  ''' <param name="Salt">The salt should be unique for each use of this function, e.g. an objects GUID.</param>
  Public Shared Function GetEncryptedToken(Salt As String, ParamArray Content() As String) As String
    Return GetEncryptedToken(Salt & Chr(30) & String.Join(Chr(30), Content))
  End Function

  Public Shared Function GetEncryptedToken(Salt As Guid, Content As String) As String
    Return GetEncryptedToken(Salt.ToString, Content)
  End Function

  Public Shared Function GetEncryptedToken(ExpiryDate As Date, Content As String) As String
    Return GetEncryptedToken(ExpiryDate.ToBinary.ToString("X"), Content)
  End Function

  Public Shared Function DecryptToken(EncryptedValue As String) As String
    SyncLock mCryptoProvider
      Dim inBytes() As Byte = Convert.FromBase64String(EncryptedValue)
      'Decryptor must be re-created on each decryption due to a bug in windows.
      Return Encoding.UTF8.GetString(mCryptoProvider.CreateDecryptor.TransformFinalBlock(inBytes, 0, inBytes.Length))
    End SyncLock
  End Function

  Public Shared Function DecryptTokenHex(EncryptedValue As String) As String
    Dim NumberChars = EncryptedValue.Length
    Dim inBytes(NumberChars / 2 - 1) As Byte
    For i As Integer = 0 To NumberChars - 1 Step 2
      inBytes(i / 2) = Convert.ToByte(EncryptedValue.Substring(i, 2), 16)
    Next
    SyncLock mCryptoProvider
      'Decryptor must be re-created on each decryption due to a bug in windows.
      Return Encoding.UTF8.GetString(mCryptoProvider.CreateDecryptor.TransformFinalBlock(inBytes, 0, inBytes.Length))
    End SyncLock
  End Function

  Public Shared Function DecryptTokenParts(EncryptedValue As String) As String()
    Return DecryptToken(EncryptedValue).Split(Chr(30))
  End Function

  Public Shared Function DecryptToken(Salt As String, EncryptedValue As String) As String
    Dim TokenPair = DecryptTokenParts(EncryptedValue)
    If TokenPair(0) = Salt Then
      Return TokenPair(1)
    Else
      Throw New Exception("Invalid Salt")
    End If
  End Function

  Public Shared Function DecryptToken(Salt As Guid, EncryptedValue As String) As String
    Return DecryptToken(Salt.ToString, EncryptedValue)
  End Function

  ''' <summary>
  ''' Decrypts a token that was encrpyted with an expiry date.
  ''' </summary>
  ''' <param name="CurrentDate">The current date of the server that created the token. Use 'Now' in most cases</param>
  Public Shared Function TryDecryptToken(CurrentDate As Date, EncryptedValue As String, ByRef DecryptedValue As String) As Boolean
    Dim Parts = DecryptTokenParts(EncryptedValue)
    Dim ExpiryDate = Date.FromBinary(Long.Parse(Parts(0), Globalization.NumberStyles.HexNumber))
    If ExpiryDate > CurrentDate Then
      DecryptedValue = Parts(1)
      Return True
    Else
      Return False
    End If
  End Function

#End If

End Class
