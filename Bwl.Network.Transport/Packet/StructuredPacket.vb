Public Class StructuredPacket
    Private Shared _utf8 As Text.Encoding = Text.Encoding.UTF8

    Public ReadOnly Property Parts As New Dictionary(Of String, Object)

    Public Sub Add(key As String, value As Object)
        If key Is Nothing Then Throw New ArgumentNullException("value")
        If value Is Nothing Then Throw New ArgumentNullException("value")

        For Each part In Parts
            If part.Key.Trim.ToLower = key.Trim.ToLower Then Throw New Exception("Part with key=" + key.ToLower + " already exists")
        Next
    End Sub

    Public Function ToBytePacket() As BytePacket
        Dim header As New Text.StringBuilder
        Dim codedParts As New List(Of Byte())
        'идентификатор
        header.Append("BwlSBP1_")
        header.Append(vbTab)
        For Each part In Parts
            Dim type = part.Value.GetType
            Dim coder = StructuredPacketPartCoders.Coders.Find(type)
            If coder Is Nothing Then Throw New Exception("Part with key " + part.Key + "  And type " + type.ToString + "can't be coded, coder not found")
            codedParts.Add(coder.ToBytes(part.Value))
            header.Append("BwlSBP1_")
            header.Append(vbTab)
        Next
        Dim ms As New IO.MemoryStream
        '   vbNull
        Dim bp As New BytePacket()
        Return bp
    End Function

    Public Sub New()

    End Sub

    Public Sub New(bp As BytePacket)

    End Sub
End Class
