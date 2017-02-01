Public Interface IPacketChannel
    Inherits IConnectionControl, IPacketStatsAndSettings
    Function Ping(maximumTimeoutMs As Integer) As Integer
    Event PacketReceived(channel As IPacketChannel, packet As BytePacket)
    Event PacketSent(channel As IPacketChannel, packet As BytePacket)
    Sub SendPacket(packet As BytePacket)
    Sub SendPacketAsync(packet As BytePacket)
End Interface
