Public Interface IPacketTransport
    Sub Open(address As String, options As String)
    Sub Close()
    Event ReceivedPacket(packet As BytePacket)
    Sub SendPacket(packet As BytePacket)
    Sub SendPacketAsync(packet As BytePacket)

    ReadOnly Property Stats As PacketTransportStats
    Property DefaultSettings As BytePacketSettings
    ReadOnly Property IsConnected() As Boolean
    Event SentPacket(packet As BytePacket)
End Interface
