Public Class TCPServer
    Inherits TCPPortListener
    Implements IPacketTransportServer

    Public Event ReceivedPacket(transport As IPacketTransport, packet As BytePacket) Implements IPacketTransportServer.ReceivedPacket
    Public Event SentPacket(transport As IPacketTransport, packet As BytePacket) Implements IPacketTransportServer.SentPacket

    Public Sub New(port As Integer)
        MyBase.New(port)
        AddHandler Me.NewConnection, AddressOf NewConnectionHandler
    End Sub

    Private Sub NewConnectionHandler(server As IPortListener, transport As IPacketTransport)
        AddHandler transport.PacketReceived, Sub(packet As BytePacket)
                                                 RaiseEvent ReceivedPacket(transport, packet)
                                             End Sub
        AddHandler transport.PacketSent, Sub(packet As BytePacket)
                                             RaiseEvent SentPacket(transport, packet)
                                         End Sub
    End Sub
End Class
