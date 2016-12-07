Public Class StructuredPacket
    Private Shared _utf8 As Text.Encoding = Text.Encoding.UTF8

    Public ReadOnly Property Parts As New Dictionary(Of String, Object)

    Public Sub Add(key As String, value As Object)
        If key Is Nothing Then Throw New ArgumentNullException("value")
        If value Is Nothing Then Throw New ArgumentNullException("value")

        For Each part In Parts
            If part.Key.Trim.ToLower = key.Trim.ToLower Then Throw New Exception("Part with key=" + key.ToLower + " already exists")
        Next
        Parts.Add(key, value)
    End Sub

    Public Function ToBytePacket() As BytePacket
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
            header.Append(part.Key.ToString)
            header.Append(vbTab)
            header.Append(coder.TypeCode.ToString)
            header.Append(vbTab)
            header.Append(partBytes.Length)
            header.Append(vbTab)
        Next
        header.Append("End")
        header.Append(vbTab)
        header.Append(255)
        header.Append(vbTab)
        header.Append(0)

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
        Dim bp As New BytePacket(resultBytes)
        Return bp
    End Function

    Public Sub New()

    End Sub

    Public Sub New(bp As BytePacket)
        If bp.Bytes.Length < 8 Then Throw New Exception("BytePacket to short, it isn't StructuredPacket")
        Dim ident = _utf8.GetString(bp.Bytes, 0, 7)
        If ident <> "BwlSBP1" Then Throw New Exception("StructuredPacket identifier not found")
        Dim headerLength = -1
        For i = 7 To Math.Min(bp.Bytes.Length - 1, 1024 * 32)
            If bp.Bytes(i) = 0 Then headerLength = i : Exit For
        Next
        If headerLength = -1 Then Throw New Exception("StructuredPacket header end not found")
        Dim header = _utf8.GetString(bp.Bytes, 0, headerLength)
        Dim headerParts = header.Split(vbTab)
        Dim offset = headerLength + 1
        For i = 1 To headerParts.Length - 1 Step 3
            Dim key = headerParts(i)
            Dim partType = CByte(headerParts(i + 1))
            Dim partLength = CInt(headerParts(i + 2))
            If partType = 255 Then
                'end
                Return
            Else
                Dim coder = StructuredPacketPartCoders.Coders.Find(partType)
                If coder Is Nothing Then Throw New Exception("Part with key " + key + "  And type " + partType.ToString + "can't be decoded, coder not found")
                Dim partObject = coder.ToObject(bp.Bytes, offset, partLength)
                offset += partLength
                Parts.Add(key, partObject)
            End If
        Next
        Throw New Exception("StructuredPacket header End mark not found")
    End Sub
End Class
