Public Interface IPacketServer
    Inherits IPacketPortListener
    Event ServerReceivedPacket(connection As IPacketChannel, packet As BytePacket)
    Event ServerSentPacket(connection As IPacketChannel, packet As BytePacket)
End Interface
