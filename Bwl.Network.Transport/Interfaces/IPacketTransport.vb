Public Interface IPacketTransport
    Inherits IConnectionControl, IStatsAndSettings
    Function Ping(maximumTimeoutMs As Integer) As Integer
    Event PacketReceived(transport As IPacketTransport, packet As BytePacket)
    Event PacketSent(transport As IPacketTransport, packet As BytePacket)
    Sub SendPacket(packet As BytePacket)
    Sub SendPacketAsync(packet As BytePacket)
End Interface
