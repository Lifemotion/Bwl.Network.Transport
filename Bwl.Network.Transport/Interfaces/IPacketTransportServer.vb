Public Interface IPacketTransportServer
    Inherits IPortListener
    Event ReceivedPacket(connection As IConnectedClient, packet As BytePacket)
    Event SentPacket(connection As IConnectedClient, packet As BytePacket)
End Interface
