Public Class TCPServer
    Inherits TCPPortListener

    Public Event ReceivedPacket(transport As TCPTransport, packet As BytePacket)
    Public Event SentPacket(transport As TCPTransport, packet As BytePacket)

    Public Sub New(port As Integer)
        MyBase.New(port)
        AddHandler Me.NewConnection, AddressOf NewConnectionHandler
    End Sub

    Private Sub NewConnectionHandler(server As IPortListener, transport As IPacketTransport)
        AddHandler transport.ReceivedPacket, Sub(packet As BytePacket)
                                                 RaiseEvent ReceivedPacket(transport, packet)
                                             End Sub
        AddHandler transport.SentPacket, Sub(packet As BytePacket)
                                             RaiseEvent SentPacket(transport, packet)
                                         End Sub
    End Sub
End Class
