Public Interface IPacketTransport
    Inherits IConnection

    Event PacketReceived(packet As BytePacket)
    Event PacketSent(packet As BytePacket)
    Sub SendPacket(packet As BytePacket)
    Sub SendPacketAsync(packet As BytePacket)
    Property DefaultSettings As BytePacketSettings
End Interface
