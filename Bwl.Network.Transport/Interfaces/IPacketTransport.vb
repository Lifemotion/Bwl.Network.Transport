Public Interface IPacketTransport
    ReadOnly Property IsConnected() As Boolean
    Event ReceivedPacket(packet As BytePacket)
    Event SentPacket(packet As BytePacket)
    Sub SendPacket(packet As BytePacket)
    Sub SendPacketAsync(packet As BytePacket)
    Sub Open(address As String, options As String)
    Sub Close()
End Interface
