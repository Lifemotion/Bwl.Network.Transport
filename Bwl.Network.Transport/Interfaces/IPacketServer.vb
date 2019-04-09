Public Interface IPacketServer
    Inherits IPacketPortListener
    Event ServerReceivedPacket(connection As IConnectedChannel, packet As BytePacket)
    Event ServerSentPacket(connection As IConnectedChannel, packet As BytePacket)
    Sub DeleteOldConnection(id As String, channel As IPacketChannel)
End Interface
