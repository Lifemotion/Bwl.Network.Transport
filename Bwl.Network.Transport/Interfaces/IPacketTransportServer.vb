Public Interface IPacketTransportServer
    Inherits IPortListener
    Event ReceivedPacket(connection As IConnectionInfo, packet As BytePacket)
    Event SentPacket(connection As IConnectionInfo, packet As BytePacket)
End Interface
