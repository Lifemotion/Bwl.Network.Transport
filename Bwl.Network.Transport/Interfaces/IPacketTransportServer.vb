Public Interface IPacketTransportServer
    Inherits IPortListener
    Event ReceivedPacket(transport As IPacketTransport, packet As BytePacket)
    Event SentPacket(transport As IPacketTransport, packet As BytePacket)
End Interface
