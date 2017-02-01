Public Interface IPacketServer
    Inherits IPacketPortListener
    Event ServerReceivedPacket(connection As IConnectedChannel, packet As BytePacket)
    Event ServerSentPacket(connection As IConnectedChannel, packet As BytePacket)
End Interface
