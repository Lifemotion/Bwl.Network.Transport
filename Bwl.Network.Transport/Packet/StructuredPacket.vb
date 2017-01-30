Public Class StructuredPacket
    Private Shared _utf8 As Text.Encoding = Text.Encoding.UTF8

    Public ReadOnly Property Parts As New Dictionary(Of String, Object)
    Public Property AddressFrom As String = ""
    Public Property AddressTo As String = ""
    Public Property ServiceID As String = ""
    Public Property EnableCRC As Boolean
    Private Shared _rnd As New Random
    Public ReadOnly Property MsgID As Integer = _rnd.NextDouble
    Public Property ReplyToID As Integer = 0

    Public Sub Add(key As String, value As Object)
        If key Is Nothing Then Throw New ArgumentNullException("value")
        If key = "" Then Throw New ArgumentNullException("value")
        If value Is Nothing Then Throw New ArgumentNullException("value")

        Parts.Add(key, value)
    End Sub

    Public Function ToBytePacket(settings As BytePacketSettings) As BytePacket
        Dim result = ToBytePacket()
        result.Settings = settings
        Return result
    End Function

    Private Sub AddToHeader(header As Text.StringBuilder, key As String, code As Byte, length As Integer)
        header.Append(key)
        header.Append(vbTab)
        header.Append(code)
        header.Append(vbTab)
        header.Append(length)
        header.Append(vbTab)
    End Sub

    Public Function ToBytes() As Byte()
        Dim header As New Text.StringBuilder
        Dim codedParts As New List(Of Byte())
        'идентификатор
        header.Append("BwlSBP1")
        header.Append(vbTab)
        Dim partsLength As Integer
        For Each part In Parts
            Dim type = part.Value.GetType
            Dim coder = StructuredPacketPartCoders.Coders.Find(type)
            If coder Is Nothing Then Throw New Exception("Part with key " + part.Key + "  And type " + type.ToString + "can't be coded, coder not found")
            Dim partBytes = coder.ToBytes(part.Value)
            partsLength += partBytes.Length
            codedParts.Add(partBytes)
            AddToHeader(header, part.Key, coder.TypeCode, partBytes.Length)
        Next
        If EnableCRC Then
            Dim crcval = CRC32.Calculate(codedParts)
            AddToHeader(header, crcval.ToString, 253, 0)
        End If
        Dim ids = MsgID.ToString + ";" + ReplyToID.ToString
        AddToHeader(header, ids, 249, 0)
        AddToHeader(header, AddressFrom, 250, 0)
        AddToHeader(header, AddressTo, 251, 0)
        AddToHeader(header, ServiceID, 252, 0)
        AddToHeader(header, "End", 255, 0)
        Dim headerString = header.ToString
        Dim ttt = headerString.Split(vbTab)
        Dim headerBytes = _utf8.GetBytes(headerString)

        Dim resultBytes(headerBytes.Length + partsLength) As Byte
        Array.Copy(headerBytes, 0, resultBytes, 0, headerBytes.Length)
        Dim offset = headerBytes.Length + 1 '+1 байт, равный нулю, для отделения заголовка
        For Each part In codedParts
            Array.Copy(part, 0, resultBytes, offset, part.Length)
            offset += part.Length
        Next
        Return resultBytes
    End Function


    Public Function ToBytePacket() As BytePacket
        Dim bp As New BytePacket(ToBytes)
        Return bp
    End Function

    Public Sub New()

    End Sub

    Public Sub New(bytePacket As BytePacket)
        Me.New(bytePacket.Bytes)
    End Sub

    Public Sub New(bytes As Byte())
        If bytes.Length < 8 Then Throw New Exception("BytePacket To Short, it isn't StructuredPacket")
        Dim ident = _utf8.GetString(bytes, 0, 7)
        If ident <> "BwlSBP1" Then Throw New Exception("StructuredPacket identifier not found")
        Dim headerLength = -1
        For i = 7 To Math.Min(bytes.Length - 1, 1024 * 32)
            If bytes(i) = 0 Then headerLength = i : Exit For
        Next
        If headerLength = -1 Then Throw New Exception("StructuredPacket header end not found")
        Dim header = _utf8.GetString(bytes, 0, headerLength)
        Dim headerParts = header.Split(vbTab)
        Dim offset = headerLength + 1
        Dim crcExpected As UInteger = 0
        Dim partsLength As Integer

        For i = 1 To headerParts.Length - 1 Step 3
            Dim key = headerParts(i)
            Dim partType = CByte(headerParts(i + 1))
            Dim partLength = CInt(headerParts(i + 2))
            partsLength += partLength
            Select Case partType
                Case 255
                    If crcExpected > 0 Then
                        Dim crcval = CRC32.Calculate(bytes, headerLength + 1, partsLength)
                        If crcval <> crcExpected Then Throw New Exception("StructuredPacket CRC was enabled, but not match")
                        EnableCRC = True
                    End If
                    'end
                    Return
                Case 250 : AddressFrom = key
                Case 251 : AddressTo = key
                Case 252 : ServiceID = key
                Case 253 : crcExpected = CUInt(key) 'crc
                Case 249
                    Dim ids = key.Split(";")
                    MsgID = CInt(ids(0))
                    ReplyToID = CInt(ids(1))
                Case 0 To 128
                    Dim coder = StructuredPacketPartCoders.Coders.Find(partType)
                    If coder Is Nothing Then Throw New Exception("Part with key " + key + "  And type " + partType.ToString + "can't be decoded, coder not found")
                    Dim partObject = coder.ToObject(bytes, offset, partLength)
                    offset += partLength
                    Parts.Add(key, partObject)
            End Select
        Next
        Throw New Exception("StructuredPacket header End mark not found")
    End Sub
End Class
